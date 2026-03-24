from __future__ import annotations

import json
from dataclasses import dataclass
from pathlib import Path


@dataclass(frozen=True)
class AppConfig:
    toggl_api_token: str
    workspace_id: int
    project_id: int
    project_name: str
    source_file: Path
    timezone: str
    duplicate_policy: str
    report_format: str
    report_output_dir: Path
    created_with: str


def load_config(config_path: str | Path) -> AppConfig:
    config_path = Path(config_path)
    with config_path.open("r", encoding="utf-8") as handle:
        raw = json.load(handle)

    report_output_dir = Path(raw.get("report_output_dir", "output"))
    if not report_output_dir.is_absolute():
        report_output_dir = config_path.parent / report_output_dir

    source_file = Path(raw["source_file"])
    if not source_file.is_absolute():
        source_file = (config_path.parent / source_file).resolve()

    return AppConfig(
        toggl_api_token=raw.get("toggl_api_token", "").strip(),
        workspace_id=int(raw["workspace_id"]),
        project_id=int(raw["project_id"]),
        project_name=str(raw.get("project_name", "")),
        source_file=source_file,
        timezone=str(raw.get("timezone", "Europe/Copenhagen")),
        duplicate_policy=str(raw.get("duplicate_policy", "skip_exact_match")),
        report_format=str(raw.get("report_format", "pdf")).lower(),
        report_output_dir=report_output_dir.resolve(),
        created_with=str(raw.get("created_with", "Codex Toggl Worklog Skill")),
    )
