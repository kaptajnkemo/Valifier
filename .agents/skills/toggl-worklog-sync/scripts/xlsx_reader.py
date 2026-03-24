from __future__ import annotations

import re
import zipfile
from datetime import date, time
from pathlib import Path
from xml.etree import ElementTree as ET

from models import WorklogRow


MAIN_NS = "http://schemas.openxmlformats.org/spreadsheetml/2006/main"
DOC_REL_NS = "http://schemas.openxmlformats.org/officeDocument/2006/relationships"
PKG_REL_NS = "http://schemas.openxmlformats.org/package/2006/relationships"
NS = {"main": MAIN_NS, "pkgrel": PKG_REL_NS}

REQUIRED_HEADERS = ("Date", "Description", "Start Time", "End Time")
TIME_RE = re.compile(r"^(?P<hour>\d{1,2})[.:](?P<minute>\d{2})$")


def load_worklog_rows(workbook_path: str | Path) -> list[WorklogRow]:
    workbook_path = Path(workbook_path)
    rows = _read_first_sheet_rows(workbook_path)
    if not rows:
        raise ValueError(f"Workbook '{workbook_path}' does not contain any rows.")

    headers = [cell.strip() for cell in rows[0]]
    missing_headers = [header for header in REQUIRED_HEADERS if header not in headers]
    if missing_headers:
        raise ValueError(f"Missing required columns: {', '.join(missing_headers)}")

    header_index = {header: headers.index(header) for header in REQUIRED_HEADERS}
    worklog_rows: list[WorklogRow] = []
    for row_number, row in enumerate(rows[1:], start=2):
        if _row_is_empty(row):
            continue

        date_text = _cell_value(row, header_index["Date"])
        description_text = _normalize_description(_cell_value(row, header_index["Description"]))
        start_time_text = _cell_value(row, header_index["Start Time"])
        end_time_text = _cell_value(row, header_index["End Time"])

        if not (date_text and description_text and start_time_text and end_time_text):
            raise ValueError(f"Row {row_number} is missing one or more required values.")

        parsed_start = _parse_time(start_time_text)
        parsed_end = _parse_time(end_time_text)
        if (parsed_end.hour, parsed_end.minute) <= (parsed_start.hour, parsed_start.minute):
            raise ValueError(f"Row {row_number} end time must be after start time.")

        worklog_rows.append(
            WorklogRow(
                source_row=row_number,
                date=_parse_date(date_text),
                description=description_text,
                start_time=parsed_start,
                end_time=parsed_end,
            )
        )

    return worklog_rows


def _row_is_empty(row: list[str]) -> bool:
    return not any(cell.strip() for cell in row)


def _cell_value(row: list[str], index: int) -> str:
    return row[index].strip() if index < len(row) else ""


def _normalize_description(value: str) -> str:
    return " ".join(value.split())


def _parse_date(value: str) -> date:
    parts = value.split("-")
    if len(parts) != 3:
        raise ValueError(f"Date '{value}' must use YYYY-MM-DD.")
    year, month, day = [int(part) for part in parts]
    return date(year, month, day)


def _parse_time(value: str) -> time:
    match = TIME_RE.fullmatch(value.strip())
    if not match:
        raise ValueError(f"Time '{value}' must use H.MM, HH.MM, H:MM, or HH:MM.")
    hour = int(match.group("hour"))
    minute = int(match.group("minute"))
    if hour > 23 or minute > 59:
        raise ValueError(f"Time '{value}' is out of range.")
    return time(hour, minute)


def _read_first_sheet_rows(workbook_path: Path) -> list[list[str]]:
    with zipfile.ZipFile(workbook_path) as workbook:
        shared_strings = _load_shared_strings(workbook)
        sheet_xml = _read_first_sheet_xml(workbook)
        return _sheet_rows(sheet_xml, shared_strings)


def _load_shared_strings(workbook: zipfile.ZipFile) -> list[str]:
    if "xl/sharedStrings.xml" not in workbook.namelist():
        return []

    root = ET.fromstring(workbook.read("xl/sharedStrings.xml"))
    values: list[str] = []
    for string_item in root.findall("main:si", NS):
        values.append("".join(text_node.text or "" for text_node in string_item.findall(".//main:t", NS)))
    return values


def _read_first_sheet_xml(workbook: zipfile.ZipFile) -> bytes:
    workbook_root = ET.fromstring(workbook.read("xl/workbook.xml"))
    workbook_rels = ET.fromstring(workbook.read("xl/_rels/workbook.xml.rels"))
    rel_map = {
        relationship.attrib["Id"]: relationship.attrib["Target"]
        for relationship in workbook_rels.findall("pkgrel:Relationship", NS)
    }
    first_sheet = workbook_root.find("main:sheets/main:sheet", NS)
    if first_sheet is None:
        raise ValueError("Workbook does not contain any sheets.")

    relationship_id = first_sheet.attrib[f"{{{DOC_REL_NS}}}id"]
    target = rel_map[relationship_id]
    target_path = target if target.startswith("xl/") else f"xl/{target}"
    return workbook.read(target_path)


def _sheet_rows(sheet_xml: bytes, shared_strings: list[str]) -> list[list[str]]:
    root = ET.fromstring(sheet_xml)
    rows: list[list[str]] = []
    for row in root.findall("main:sheetData/main:row", NS):
        row_values: dict[int, str] = {}
        for cell in row.findall("main:c", NS):
            reference = cell.attrib.get("r", "")
            row_values[_column_index(reference)] = _cell_text(cell, shared_strings)
        if not row_values:
            rows.append([])
            continue
        max_index = max(row_values)
        rows.append([row_values.get(index, "") for index in range(max_index + 1)])
    return rows


def _column_index(reference: str) -> int:
    column_letters = "".join(character for character in reference if character.isalpha())
    column = 0
    for letter in column_letters:
        column = (column * 26) + (ord(letter.upper()) - 64)
    return column - 1


def _cell_text(cell: ET.Element, shared_strings: list[str]) -> str:
    cell_type = cell.attrib.get("t")
    if cell_type == "inlineStr":
        return "".join(text_node.text or "" for text_node in cell.findall(".//main:t", NS))

    value_node = cell.find("main:v", NS)
    raw_value = value_node.text if value_node is not None and value_node.text is not None else ""
    if cell_type == "s":
        index = int(raw_value)
        return shared_strings[index]
    return raw_value
