from __future__ import annotations

import argparse
import sys
from pathlib import Path

from _skill_common import (
    ValidationError,
    ensure_slug,
    validate_openai_yaml,
    validate_skill_md,
)


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description="Validate a skill package layout.")
    parser.add_argument("skill_dir", help="Path to the skill directory.")
    return parser


def main(argv: list[str] | None = None) -> int:
    parser = build_parser()
    args = parser.parse_args(argv)

    try:
        skill_dir = Path(args.skill_dir).resolve()
        slug = ensure_slug(skill_dir.name)
    except ValidationError as exc:
        print(str(exc), file=sys.stderr)
        return 1

    errors: list[str] = []
    errors.extend(validate_skill_md(skill_dir / "SKILL.md"))
    errors.extend(validate_openai_yaml(skill_dir / "agents" / "openai.yaml", slug))

    if errors:
        for error in errors:
            print(error, file=sys.stderr)
        return 1

    print(f"Validation passed: {skill_dir}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
