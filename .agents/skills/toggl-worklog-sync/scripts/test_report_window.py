import sys
import unittest
from datetime import date
from pathlib import Path


SCRIPT_DIR = Path(__file__).resolve().parent
if str(SCRIPT_DIR) not in sys.path:
    sys.path.insert(0, str(SCRIPT_DIR))

from report_window import billing_window_for  # noqa: E402


class BillingWindowTests(unittest.TestCase):
    def test_uses_20th_previous_month_to_19th_current_month(self) -> None:
        start_date, end_date = billing_window_for(date(2026, 3, 20))

        self.assertEqual(date(2026, 2, 20), start_date)
        self.assertEqual(date(2026, 3, 19), end_date)

    def test_rolls_back_year_in_january(self) -> None:
        start_date, end_date = billing_window_for(date(2026, 1, 5))

        self.assertEqual(date(2025, 12, 20), start_date)
        self.assertEqual(date(2026, 1, 19), end_date)


if __name__ == "__main__":
    unittest.main()
