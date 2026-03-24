from __future__ import annotations

from dataclasses import dataclass
from datetime import date, datetime, time


@dataclass(frozen=True)
class WorklogRow:
    source_row: int
    date: date
    description: str
    start_time: time
    end_time: time


@dataclass(frozen=True)
class ExistingEntry:
    description: str
    project_id: int | None
    start: datetime
    stop: datetime | None
