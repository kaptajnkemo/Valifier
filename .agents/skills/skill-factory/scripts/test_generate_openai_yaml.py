import subprocess
import sys
import tempfile
import textwrap
import unittest
from pathlib import Path


SCRIPT_DIR = Path(__file__).resolve().parent
GENERATOR = SCRIPT_DIR / "generate_openai_yaml.py"


class GenerateOpenAiYamlTests(unittest.TestCase):
    def test_generates_openai_yaml_from_interface_values(self) -> None:
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
                [
                    sys.executable,
                    str(GENERATOR),
                    str(skill_dir),
                    "--interface",
                    "display_name=Demo Skill",
                    "--interface",
                    "short_description=Do one concrete thing well",
                    "--interface",
                    "default_prompt=Use $demo-skill to do one concrete thing well.",
                ],
                capture_output=True,
                text=True,
            )

            self.assertEqual(0, result.returncode, msg=result.stderr)

            output = (skill_dir / "agents" / "openai.yaml").read_text(encoding="utf-8")
            self.assertIn('display_name: "Demo Skill"', output)
            self.assertIn('allow_implicit_invocation: true', output)


if __name__ == "__main__":
    unittest.main()
