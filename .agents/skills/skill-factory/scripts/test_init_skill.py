import subprocess
import sys
import tempfile
import unittest
from pathlib import Path


SCRIPT_DIR = Path(__file__).resolve().parent
INIT_SKILL = SCRIPT_DIR / "init_skill.py"


class InitSkillTests(unittest.TestCase):
    def test_creates_skill_with_metadata_and_optional_resources(self) -> None:
        with tempfile.TemporaryDirectory() as tmp:
            output_dir = Path(tmp)

            result = subprocess.run(
                [
                    sys.executable,
                    str(INIT_SKILL),
                    "demo-skill",
                    "--path",
                    str(output_dir),
                    "--resources",
                    "scripts,references",
                    "--interface",
                    "display_name=Demo Skill",
                    "--interface",
                    "short_description=Create a demo skill package",
                    "--interface",
                    "default_prompt=Use $demo-skill to create a demo skill package.",
                ],
                capture_output=True,
                text=True,
            )

            self.assertEqual(0, result.returncode, msg=result.stderr)

            skill_dir = output_dir / "demo-skill"
            self.assertTrue((skill_dir / "SKILL.md").exists())
            self.assertTrue((skill_dir / "agents" / "openai.yaml").exists())
            self.assertTrue((skill_dir / "scripts").exists())
            self.assertTrue((skill_dir / "references").exists())

    def test_rejects_existing_target_path(self) -> None:
        with tempfile.TemporaryDirectory() as tmp:
            output_dir = Path(tmp)
            existing = output_dir / "demo-skill"
            existing.mkdir()

            result = subprocess.run(
                [
                    sys.executable,
                    str(INIT_SKILL),
                    "demo-skill",
                    "--path",
                    str(output_dir),
                ],
                capture_output=True,
                text=True,
            )

            self.assertNotEqual(0, result.returncode)
            self.assertIn("already exists", result.stderr)


if __name__ == "__main__":
    unittest.main()
