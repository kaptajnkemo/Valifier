---
name: Toggl Worklog Sync
description: Import worklog rows from the local SoT Excel sheet into Toggl Track with duplicate skipping and attempt official detailed PDF report export for the monthly billing window.
---

# Precedence

- This skill is operational and lower-priority than:
  - constitution/skill-1-immutable-authority.md
  - constitution/skill-2-execution-evidence.md
  - constitution/skill-3-domain-law.md
  - constitution/skill-4-local-work-protection-case-law.md
- If any instruction in this skill conflicts with Constitution layers 1-4, output MUST explicitly state execution is blocked and cite the exact conflict reason, or the action must be rejected.

# Purpose

- This skill MUST be used when the task is to sync local worklog data into Toggl Track and export the official Toggl detailed report as PDF.
- The source of truth MUST be the local Excel workbook referenced by `config.local.json`.
- The workbook MUST contain the columns `Date`, `Description`, `Start Time`, and `End Time`.
- The `Date` column MUST use `YYYY-MM-DD`.
- The `Start Time` and `End Time` columns MUST use `H.MM`, `HH.MM`, `H:MM`, or `HH:MM`.
- Duplicate handling MUST default to exact-match skipping using `date`, `start time`, `end time`, `description`, and `project_id`.
- The workflow MUST minimize Toggl API usage:
  - one request to fetch existing entries for the workbook date range,
  - one request per missing entry to create,
  - one request to export the official PDF report.
- The detailed report export MUST use the official Toggl Reports API.
- If Toggl returns that the report feature is unavailable, execution MUST stop and report that manual report extraction is required.

# Workflow

1. Validate the workbook locally before any API call:
   - `python .agents/skills/toggl-worklog-sync/scripts/toggl_worklog.py validate`
2. Preview what would be created without POST requests:
   - `python .agents/skills/toggl-worklog-sync/scripts/toggl_worklog.py sync --dry-run`
3. Sync missing entries into Toggl:
   - `python .agents/skills/toggl-worklog-sync/scripts/toggl_worklog.py sync`
4. Export the official Toggl PDF report for the billing window:
   - `python .agents/skills/toggl-worklog-sync/scripts/toggl_worklog.py export-pdf`
5. Run both steps in sequence when a single command is preferred:
   - `python .agents/skills/toggl-worklog-sync/scripts/toggl_worklog.py sync-and-export`

# Files

- Runtime settings MUST be read from `.agents/skills/toggl-worklog-sync/config.local.json`.
- Script entrypoint: `.agents/skills/toggl-worklog-sync/scripts/toggl_worklog.py`
