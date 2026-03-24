from __future__ import annotations

from datetime import date


def billing_window_for(anchor: date) -> tuple[date, date]:
    previous_month_year = anchor.year
    previous_month = anchor.month - 1
    if previous_month == 0:
        previous_month = 12
        previous_month_year -= 1

    return date(previous_month_year, previous_month, 20), date(anchor.year, anchor.month, 19)
