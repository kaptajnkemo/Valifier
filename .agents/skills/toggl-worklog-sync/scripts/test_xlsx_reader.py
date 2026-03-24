import io
import sys
import tempfile
import unittest
import zipfile
from pathlib import Path


SCRIPT_DIR = Path(__file__).resolve().parent
if str(SCRIPT_DIR) not in sys.path:
    sys.path.insert(0, str(SCRIPT_DIR))

from xlsx_reader import load_worklog_rows  # noqa: E402


WORKBOOK_XML = """<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"
 xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
  <sheets>
    <sheet name="Sheet1" sheetId="1" r:id="rId1"/>
  </sheets>
</workbook>
"""

WORKBOOK_RELS_XML = """<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1"
    Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"
    Target="worksheets/sheet1.xml"/>
</Relationships>
"""

CONTENT_TYPES_XML = """<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
  <Default Extension="xml" ContentType="application/xml"/>
  <Override PartName="/xl/workbook.xml"
    ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>
  <Override PartName="/xl/worksheets/sheet1.xml"
    ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
</Types>
"""

ROOT_RELS_XML = """<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1"
    Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"
    Target="xl/workbook.xml"/>
</Relationships>
"""


def build_sheet_xml(rows):
    lines = [
        '<?xml version="1.0" encoding="UTF-8" standalone="yes"?>',
        '<worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">',
        "  <sheetData>",
    ]
    for row_index, values in enumerate(rows, start=1):
        lines.append(f'    <row r="{row_index}">')
        for column_index, value in enumerate(values, start=1):
            column = chr(64 + column_index)
            escaped = (
                value.replace("&", "&amp;")
                .replace("<", "&lt;")
                .replace(">", "&gt;")
            )
            lines.append(
                f'      <c r="{column}{row_index}" t="inlineStr"><is><t>{escaped}</t></is></c>'
            )
        lines.append("    </row>")
    lines.extend(["  </sheetData>", "</worksheet>"])
    return "\n".join(lines)


def make_workbook(path: Path, rows) -> None:
    sheet_xml = build_sheet_xml(rows)
    with zipfile.ZipFile(path, "w", compression=zipfile.ZIP_DEFLATED) as archive:
        archive.writestr("[Content_Types].xml", CONTENT_TYPES_XML)
        archive.writestr("_rels/.rels", ROOT_RELS_XML)
        archive.writestr("xl/workbook.xml", WORKBOOK_XML)
        archive.writestr("xl/_rels/workbook.xml.rels", WORKBOOK_RELS_XML)
        archive.writestr("xl/worksheets/sheet1.xml", sheet_xml)


class LoadWorklogRowsTests(unittest.TestCase):
    def test_loads_rows_from_first_sheet(self) -> None:
        rows = [
            ["Date", "Description", "Start Time", "End Time"],
            ["2026-03-10", "Kickoff", "15.30", "16.00"],
            ["2026-03-11", "Deep work", "9.45", "10:00"],
        ]
        with tempfile.TemporaryDirectory() as temp_dir:
            workbook_path = Path(temp_dir) / "source.xlsx"
            make_workbook(workbook_path, rows)

            worklog_rows = load_worklog_rows(workbook_path)

        self.assertEqual(2, len(worklog_rows))
        self.assertEqual("2026-03-10", worklog_rows[0].date.isoformat())
        self.assertEqual("15:30:00", worklog_rows[0].start_time.isoformat())
        self.assertEqual("16:00:00", worklog_rows[0].end_time.isoformat())
        self.assertEqual("Kickoff", worklog_rows[0].description)
        self.assertEqual("09:45:00", worklog_rows[1].start_time.isoformat())
        self.assertEqual("10:00:00", worklog_rows[1].end_time.isoformat())

    def test_rejects_missing_required_headers(self) -> None:
        rows = [
            ["Date", "Description", "Duration"],
            ["2026-03-10", "Kickoff", "30"],
        ]
        with tempfile.TemporaryDirectory() as temp_dir:
            workbook_path = Path(temp_dir) / "source.xlsx"
            make_workbook(workbook_path, rows)

            with self.assertRaisesRegex(ValueError, "Missing required columns"):
                load_worklog_rows(workbook_path)


if __name__ == "__main__":
    unittest.main()
