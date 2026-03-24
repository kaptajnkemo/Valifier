import subprocess
import sys
import tempfile
import textwrap
import unittest
from pathlib import Path


SCRIPT_DIR = Path(__file__).resolve().parent
VALIDATOR = SCRIPT_DIR / "quick_validate.py"


class QuickValidateTests(unittest.TestCase):
    def test_accepts_valid_skill_package(self) -> None:
        with tempfile.TemporaryDirectory() as tmp:
            skill_dir = Path(tmp) / "demo-skill"
            (skill_dir / "agents").mkdir(parents=True)
            (skill_dir / "SKILL.md").write_text(
                textwrap.dedent(
                    """\
                    ---
                    name: Demo Skill
                    description: Do one concrete thing well.
                    ---

                    # Precedence

                    - Lower priority than repository law files.

                    # Purpose

                    Execute the requested task.
                    """
                ),
                encoding="utf-8",
            )
            (skill_dir / "agents" / "openai.yaml").write_text(
                textwrap.dedent(
                    """\
                    interface:
                      display_name: "Demo Skill"
                      short_description: "Do one concrete thing well"
                      default_prompt: "Use $demo-skill to do one concrete thing well."

                    policy:
                      allow_implicit_invocation: true
                    """
                ),
                encoding="utf-8",
            )

            result = subprocess.run(
                [sys.executable, str(VALIDATOR), str(skill_dir)],
                capture_output=True,
                text=True,
            )

            self.assertEqual(0, result.returncode, msg=result.stderr)

    def test_rejects_missing_openai_yaml(self) -> None:
        with tempfile.TemporaryDirectory() as tmp:
            skill_dir = Path(tmp) / "demo-skill"
            skill_dir.mkdir(parents=True)
            (skill_dir / "SKILL.md").write_text(
                textwrap.dedent(
                    """\
                    ---
                    name: Demo Skill
                    description: Do one concrete thing well.
                    ---

                    # Precedence

                    - Lower priority than repository law files.

                    # Purpose

                    Execute the requested task.
                    """
                ),
                encoding="utf-8",
            )

            result = subprocess.run(
                [sys.executable, str(VALIDATOR), str(skill_dir)],
                capture_output=True,
                text=True,
            )

            self.assertNotEqual(0, result.returncode)
            self.assertIn("agents/openai.yaml", result.stderr)


if __name__ == "__main__":
    unittest.main()
