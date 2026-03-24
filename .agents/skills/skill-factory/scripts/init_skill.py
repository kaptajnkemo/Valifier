from __future__ import annotations

import argparse
import subprocess
import sys
from pathlib import Path

from _skill_common import (
    ValidationError,
    default_interface_values,
    ensure_slug,
    parse_interface_values,
    parse_resources,
    render_skill_md,
    required_interface_values,
)


EXAMPLE_CONTENT = {
    "scripts": ("example.py", "def main() -> None:\n    print('example')\n"),
    "references": ("example.md", "# Example Reference\n"),
    "assets": ("example.txt", "example asset\n"),
}


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description="Initialize a new skill package.")
    parser.add_argument("skill_name", help="Folder slug for the skill package.")
    parser.add_argument(
        "--path",
        required=True,
        help="Directory where the skill package should be created.",
    )
    parser.add_argument(
        "--description",
        default="Describe when this skill should be used.",
        help="Frontmatter description for SKILL.md.",
    )
    parser.add_argument(
        "--resources",
        default="",
        help="Comma-separated optional resource directories: scripts,references,assets",
    )
    parser.add_argument(
        "--examples",
        action="store_true",
        help="Create example files in requested resource directories.",
    )
    parser.add_argument(
        "--interface",
        action="append",
        default=[],
        help="Interface values in key=value form.",
    )
    return parser


def main(argv: list[str] | None = None) -> int:
    parser = build_parser()
    args = parser.parse_args(argv)

    try:
        slug = ensure_slug(args.skill_name)
        base_path = Path(args.path).resolve()
        resources = parse_resources(args.resources)
    except ValidationError as exc:
        print(str(exc), file=sys.stderr)
        return 1

    skill_dir = base_path / slug
    if skill_dir.exists():
        print(f"Target path already exists: {skill_dir}", file=sys.stderr)
        return 1

    try:
        interface_values = default_interface_values(slug, args.description)
        interface_values.update(parse_interface_values(args.interface))
        interface_values = required_interface_values(interface_values, slug)
    except ValidationError as exc:
        print(str(exc), file=sys.stderr)
        return 1

    skill_dir.mkdir(parents=True)
    (skill_dir / "SKILL.md").write_text(
        render_skill_md(interface_values["display_name"], args.description),
        encoding="utf-8",
    )

    generate_cmd = [
        sys.executable,
        str(Path(__file__).with_name("generate_openai_yaml.py")),
        str(skill_dir),
    ]
    for key, value in interface_values.items():
        generate_cmd.extend(["--interface", f"{key}={value}"])

    result = subprocess.run(generate_cmd, capture_output=True, text=True)
    if result.returncode != 0:
        print(result.stderr, file=sys.stderr, end="")
        return result.returncode

    for resource in resources:
        resource_dir = skill_dir / resource
        resource_dir.mkdir(parents=True, exist_ok=True)
        if args.examples:
            filename, content = EXAMPLE_CONTENT[resource]
            (resource_dir / filename).write_text(content, encoding="utf-8")

    print(f"Created {skill_dir}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
