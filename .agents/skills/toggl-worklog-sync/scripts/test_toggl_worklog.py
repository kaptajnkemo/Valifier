import sys
import unittest
from datetime import date, datetime, time
from pathlib import Path
from zoneinfo import ZoneInfo


SCRIPT_DIR = Path(__file__).resolve().parent
if str(SCRIPT_DIR) not in sys.path:
    sys.path.insert(0, str(SCRIPT_DIR))

from toggl_worklog import (  # noqa: E402
    ExistingEntry,
    WorklogRow,
    build_create_payload,
    build_report_export_request,
    find_rows_to_create,
)


class TogglWorklogTests(unittest.TestCase):
    def test_skips_exact_match_entries(self) -> None:
        timezone_name = "Europe/Copenhagen"
        row = WorklogRow(
            source_row=2,
            date=date(2026, 3, 10),
            description="Kickoff",
            start_time=time(15, 30),
            end_time=time(16, 0),
        )
        existing_entry = ExistingEntry(
            description="Kickoff",
            project_id=209983126,
            start=datetime(2026, 3, 10, 14, 30, tzinfo=ZoneInfo("UTC")),
            stop=datetime(2026, 3, 10, 15, 0, tzinfo=ZoneInfo("UTC")),
        )

        pending = find_rows_to_create(
            rows=[row],
            existing_entries=[existing_entry],
            project_id=209983126,
            timezone_name=timezone_name,
        )

        self.assertEqual([], pending)

    def test_builds_manual_time_entry_payload(self) -> None:
        row = WorklogRow(
            source_row=2,
            date=date(2026, 3, 10),
            description="Kickoff",
            start_time=time(15, 30),
            end_time=time(16, 0),
        )

        payload = build_create_payload(
            row=row,
            workspace_id=9320737,
            project_id=209983126,
            timezone_name="Europe/Copenhagen",
        )

        self.assertEqual(9320737, payload["workspace_id"])
        self.assertEqual(209983126, payload["project_id"])
        self.assertEqual(1800, payload["duration"])
        self.assertEqual("2026-03-10T14:30:00Z", payload["start"])
        self.assertEqual("2026-03-10T15:00:00Z", payload["stop"])

    def test_builds_pdf_export_request(self) -> None:
        request = build_report_export_request(
            workspace_id=9320737,
            project_id=209983126,
            start_date=date(2026, 2, 20),
            end_date=date(2026, 3, 19),
        )

        self.assertTrue(request["url"].endswith("/reports/api/v3/workspace/9320737/search/time_entries.pdf"))
        self.assertEqual([209983126], request["payload"]["project_ids"])
        self.assertEqual("2026-02-20", request["payload"]["start_date"])
        self.assertEqual("2026-03-19", request["payload"]["end_date"])


if __name__ == "__main__":
    unittest.main()
