from __future__ import annotations

import argparse
import json
from dataclasses import asdict
from datetime import UTC, date, datetime, time
from pathlib import Path
from zoneinfo import ZoneInfo

from config import AppConfig, load_config
from models import ExistingEntry, WorklogRow
from report_window import billing_window_for
from toggl_client import TogglClient, TogglFeatureUnavailableError
from xlsx_reader import load_worklog_rows


DEFAULT_CONFIG_PATH = Path(__file__).resolve().parent.parent / "config.local.json"


def build_create_payload(
    row: WorklogRow,
    workspace_id: int,
    project_id: int,
    timezone_name: str,
    created_with: str = "Codex Toggl Worklog Skill",
) -> dict:
    start_at, stop_at = row_times_in_utc(row, timezone_name)
    return {
        "created_with": created_with,
        "description": normalize_description(row.description),
        "duration": int((stop_at - start_at).total_seconds()),
        "project_id": project_id,
        "pid": project_id,
        "start": isoformat_z(start_at),
        "stop": isoformat_z(stop_at),
        "workspace_id": workspace_id,
    }


def build_report_export_request(workspace_id: int, project_id: int, start_date: date, end_date: date) -> dict:
    return {
        "url": f"https://api.track.toggl.com/reports/api/v3/workspace/{workspace_id}/search/time_entries.pdf",
        "payload": {
            "order_by": "date",
            "order_dir": "ASC",
            "page_size": 50,
            "project_ids": [project_id],
            "start_date": start_date.isoformat(),
            "end_date": end_date.isoformat(),
        },
    }


def find_rows_to_create(
    rows: list[WorklogRow],
    existing_entries: list[ExistingEntry],
    project_id: int,
    timezone_name: str,
) -> list[WorklogRow]:
    existing_signatures = {
        entry_signature(
            description=entry.description,
            project_id=entry.project_id,
            start_at=entry.start,
            stop_at=entry.stop,
        )
        for entry in existing_entries
        if entry.stop is not None
    }

    pending: list[WorklogRow] = []
    for row in rows:
        start_at, stop_at = row_times_in_utc(row, timezone_name)
        signature = entry_signature(
            description=row.description,
            project_id=project_id,
            start_at=start_at,
            stop_at=stop_at,
        )
        if signature not in existing_signatures:
            pending.append(row)
    return pending


def row_times_in_utc(row: WorklogRow, timezone_name: str) -> tuple[datetime, datetime]:
    timezone = ZoneInfo(timezone_name)
    start_local = datetime.combine(row.date, row.start_time, tzinfo=timezone)
    stop_local = datetime.combine(row.date, row.end_time, tzinfo=timezone)
    return start_local.astimezone(UTC), stop_local.astimezone(UTC)


def normalize_description(value: str) -> str:
    return " ".join(value.split())


def isoformat_z(value: datetime) -> str:
    return value.astimezone(UTC).replace(microsecond=0).isoformat().replace("+00:00", "Z")


def entry_signature(
    description: str,
    project_id: int | None,
    start_at: datetime,
    stop_at: datetime | None,
) -> tuple[str, int | None, str, str | None]:
    normalized_start = isoformat_z(start_at)
    normalized_stop = isoformat_z(stop_at) if stop_at is not None else None
    return (normalize_description(description), project_id, normalized_start, normalized_stop)


def existing_entries_from_api(payload: list[dict]) -> list[ExistingEntry]:
    entries: list[ExistingEntry] = []
    for item in payload:
        start_text = item.get("start")
        if not start_text:
            continue
        stop_text = item.get("stop")
        entries.append(
            ExistingEntry(
                description=str(item.get("description") or ""),
                project_id=_extract_project_id(item),
                start=parse_iso_datetime(start_text),
                stop=parse_iso_datetime(stop_text) if stop_text else None,
            )
        )
    return entries


def parse_iso_datetime(value: str) -> datetime:
    normalized = value.replace("Z", "+00:00")
    return datetime.fromisoformat(normalized)


def _extract_project_id(item: dict) -> int | None:
    project_id = item.get("project_id")
    if project_id is None:
        project_id = item.get("pid")
    return int(project_id) if project_id is not None else None


def sync_worklog(config: AppConfig, dry_run: bool) -> dict:
    rows = load_worklog_rows(config.source_file)
    if not rows:
        return {"source_rows": 0, "existing_matches": 0, "pending_rows": 0, "created_rows": 0}

    range_start = min(row.date for row in rows)
    range_end = max(row.date for row in rows)
    start_at = datetime.combine(range_start, time(0, 0), tzinfo=ZoneInfo(config.timezone))
    end_at = datetime.combine(range_end, time(23, 59), tzinfo=ZoneInfo(config.timezone))

    client = TogglClient(api_token=config.toggl_api_token, created_with=config.created_with)
    existing_entries = existing_entries_from_api(client.fetch_time_entries(start_at, end_at))
    pending_rows = find_rows_to_create(rows, existing_entries, config.project_id, config.timezone)

    created_rows = 0
    if not dry_run:
        for row in pending_rows:
            payload = build_create_payload(
                row=row,
                workspace_id=config.workspace_id,
                project_id=config.project_id,
                timezone_name=config.timezone,
                created_with=config.created_with,
            )
            client.create_time_entry(config.workspace_id, payload)
            created_rows += 1

    return {
        "source_rows": len(rows),
        "existing_matches": len(rows) - len(pending_rows),
        "pending_rows": len(pending_rows),
        "created_rows": created_rows,
        "range_start": range_start.isoformat(),
        "range_end": range_end.isoformat(),
    }


def export_pdf_report(config: AppConfig, anchor_date: date | None = None) -> Path:
    anchor = anchor_date or date.today()
    start_date, end_date = billing_window_for(anchor)
    report_request = build_report_export_request(
        workspace_id=config.workspace_id,
        project_id=config.project_id,
        start_date=start_date,
        end_date=end_date,
    )

    client = TogglClient(api_token=config.toggl_api_token, created_with=config.created_with)
    output_path = config.report_output_dir / (
        f"toggl-detailed-report-{start_date.isoformat()}-to-{end_date.isoformat()}.pdf"
    )
    return client.export_detailed_pdf(config.workspace_id, report_request["payload"], output_path)


def validate_source(config: AppConfig) -> dict:
    rows = load_worklog_rows(config.source_file)
    return {
        "source_rows": len(rows),
        "source_file": str(config.source_file),
        "first_row": asdict(rows[0]) if rows else None,
        "last_row": asdict(rows[-1]) if rows else None,
    }


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description="Sync SoT Excel worklogs into Toggl and export the official PDF report.")
    parser.add_argument(
        "command",
        choices=("validate", "sync", "export-pdf", "sync-and-export"),
        help="Action to execute.",
    )
    parser.add_argument(
        "--config",
        default=str(DEFAULT_CONFIG_PATH),
        help="Path to config.local.json",
    )
    parser.add_argument(
        "--dry-run",
        action="store_true",
        help="Preview matching and pending rows without creating Toggl entries.",
    )
    parser.add_argument(
        "--today",
        help="Override the anchor date used to compute the billing window, format YYYY-MM-DD.",
    )
    return parser


def parse_anchor_date(value: str | None) -> date | None:
    if value is None:
        return None
    return date.fromisoformat(value)


def main() -> int:
    parser = build_parser()
    args = parser.parse_args()
    config = load_config(args.config)
    anchor_date = parse_anchor_date(args.today)

    if args.command == "validate":
        print(json.dumps(validate_source(config), indent=2, default=str))
        return 0

    if args.command == "sync":
        print(json.dumps(sync_worklog(config, dry_run=args.dry_run), indent=2))
        return 0

    if args.command == "export-pdf":
        try:
            output_path = export_pdf_report(config, anchor_date)
            print(json.dumps({"report_path": str(output_path)}, indent=2))
            return 0
        except TogglFeatureUnavailableError as error:
            print(
                json.dumps(
                    {
                        "manual_report_extraction_required": True,
                        "reason": str(error),
                    },
                    indent=2,
                )
            )
            return 2

    if args.command == "sync-and-export":
        result = {"sync": sync_worklog(config, dry_run=args.dry_run)}
        if args.dry_run:
            print(json.dumps(result, indent=2))
            return 0
        try:
            result["report_path"] = str(export_pdf_report(config, anchor_date))
        except TogglFeatureUnavailableError as error:
            result["manual_report_extraction_required"] = True
            result["report_reason"] = str(error)
            print(json.dumps(result, indent=2))
            return 2
        print(json.dumps(result, indent=2))
        return 0

    parser.error(f"Unsupported command: {args.command}")
    return 1


if __name__ == "__main__":
    raise SystemExit(main())
