from __future__ import annotations

import re
import textwrap
from pathlib import Path


SKILL_NAME_PATTERN = re.compile(r"^[a-z0-9]+(?:-[a-z0-9]+)*$")
FRONTMATTER_PATTERN = re.compile(r"\A---\n(.*?)\n---\n", re.DOTALL)
ALLOWED_OPTIONAL_RESOURCES = {"scripts", "references", "assets"}
REQUIRED_INTERFACE_KEYS = ("display_name", "short_description", "default_prompt")


class ValidationError(Exception):
    pass


def ensure_slug(skill_name: str) -> str:
    if not SKILL_NAME_PATTERN.fullmatch(skill_name):
        raise ValidationError(
            "Skill name must use lowercase letters, digits, and hyphens only."
        )
    return skill_name


def parse_interface_values(items: list[str]) -> dict[str, str]:
    values: dict[str, str] = {}
    for item in items:
        key, sep, value = item.partition("=")
        if not sep or not key or not value:
            raise ValidationError(
                f"Invalid --interface value {item!r}. Expected key=value."
            )
        values[key] = value
    return values


def parse_resources(raw: str | None) -> list[str]:
    if not raw:
        return []

    values = [item.strip() for item in raw.split(",") if item.strip()]
    invalid = [item for item in values if item not in ALLOWED_OPTIONAL_RESOURCES]
    if invalid:
        raise ValidationError(
            "Unsupported resource names: " + ", ".join(sorted(invalid))
        )
    return values


def required_interface_values(values: dict[str, str], slug: str) -> dict[str, str]:
    missing = [key for key in REQUIRED_INTERFACE_KEYS if not values.get(key)]
    if missing:
        raise ValidationError(
            "Missing required interface values: " + ", ".join(missing)
        )

    prompt = values["default_prompt"]
    if f"${slug}" not in prompt:
        raise ValidationError(
            f"default_prompt must explicitly mention ${slug}."
        )

    return {key: values[key] for key in REQUIRED_INTERFACE_KEYS}


def default_interface_values(slug: str, description: str) -> dict[str, str]:
    display_name = " ".join(part.capitalize() for part in slug.split("-"))
    short_description = description.rstrip(".")
    if len(short_description) > 64:
        short_description = short_description[:61].rstrip() + "..."
    if len(short_description) < 25:
        short_description = f"Create or use the {display_name} skill"

    return {
        "display_name": display_name,
        "short_description": short_description,
        "default_prompt": f"Use ${slug} to perform the primary task.",
    }


def render_skill_md(display_name: str, description: str) -> str:
    return textwrap.dedent(
        f"""\
        ---
        name: {display_name}
        description: {description}
        ---

        # Precedence

        - This skill is operational and lower-priority than:
          - constitution/skill-1-immutable-authority.md
          - constitution/skill-2-execution-evidence.md
          - constitution/skill-3-domain-law.md
          - constitution/skill-4-local-work-protection-case-law.md
        - If any instruction in this skill conflicts with Constitution layers 1-4, execution is blocked and the exact conflict reason must be stated.

        # Purpose

        Add the operational instructions for this skill here.
        """
    )


def render_openai_yaml(values: dict[str, str]) -> str:
    return textwrap.dedent(
        f"""\
        interface:
          display_name: "{values['display_name']}"
          short_description: "{values['short_description']}"
          default_prompt: "{values['default_prompt']}"

        policy:
          allow_implicit_invocation: true
        """
    )


def read_frontmatter(text: str) -> dict[str, str]:
    match = FRONTMATTER_PATTERN.match(text)
    if not match:
        raise ValidationError("SKILL.md must begin with YAML frontmatter.")

    values: dict[str, str] = {}
    for raw_line in match.group(1).splitlines():
        line = raw_line.strip()
        if not line:
            continue
        key, sep, value = line.partition(":")
        if not sep:
            raise ValidationError(f"Invalid frontmatter line: {raw_line!r}")
        values[key.strip()] = value.strip()
    return values


def validate_skill_md(skill_md: Path) -> list[str]:
    errors: list[str] = []
    if not skill_md.exists():
        return ["Missing SKILL.md."]

    text = skill_md.read_text(encoding="utf-8")
    try:
        frontmatter = read_frontmatter(text)
    except ValidationError as exc:
        return [str(exc)]

    required = {"name", "description"}
    missing = sorted(required - set(frontmatter))
    if missing:
        errors.append("Missing frontmatter fields: " + ", ".join(missing))

    extra = sorted(set(frontmatter) - required)
    if extra:
        errors.append("Unexpected frontmatter fields: " + ", ".join(extra))

    if "\n# Precedence\n" not in text:
        errors.append("SKILL.md must contain a '# Precedence' section.")

    if not re.search(r"\n# (?!Precedence\b).+\n", text):
        errors.append("SKILL.md must contain a purpose section after '# Precedence'.")

    return errors


def validate_openai_yaml(openai_yaml: Path, slug: str) -> list[str]:
    if not openai_yaml.exists():
        return ["Missing agents/openai.yaml."]

    text = openai_yaml.read_text(encoding="utf-8")
    required_lines = (
        "interface:",
        '  display_name: "',
        '  short_description: "',
        '  default_prompt: "',
        "policy:",
        "  allow_implicit_invocation: true",
    )

    errors = [
        f"agents/openai.yaml must contain {line!r}."
        for line in required_lines
        if line not in text
    ]

    prompt_match = re.search(r'^\s*default_prompt:\s*"([^"]+)"\s*$', text, re.MULTILINE)
    if prompt_match and f"${slug}" not in prompt_match.group(1):
        errors.append(f"agents/openai.yaml default_prompt must mention ${slug}.")

    return errors
