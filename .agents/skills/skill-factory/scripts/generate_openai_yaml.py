from __future__ import annotations

import argparse
import sys
from pathlib import Path

from _skill_common import (
    ValidationError,
    ensure_slug,
    parse_interface_values,
    render_openai_yaml,
    required_interface_values,
)


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(
        description="Generate agents/openai.yaml for a skill package."
    )
    parser.add_argument("skill_dir", help="Path to the skill directory.")
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
        skill_dir = Path(args.skill_dir).resolve()
        slug = ensure_slug(skill_dir.name)
        values = parse_interface_values(args.interface)
        values = required_interface_values(values, slug)
    except ValidationError as exc:
        print(str(exc), file=sys.stderr)
        return 1

    agents_dir = skill_dir / "agents"
    agents_dir.mkdir(parents=True, exist_ok=True)
    output_path = agents_dir / "openai.yaml"
    output_path.write_text(render_openai_yaml(values), encoding="utf-8")
    print(f"Created {output_path}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
