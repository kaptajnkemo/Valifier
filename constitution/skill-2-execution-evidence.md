# SKILL 2 — EXECUTION & EVIDENCE

This file defines how execution, verification, and reporting are performed.
It is procedural and subordinate to SKILL 1.

====================================================================
SCOPE
====================================================================

This skill applies whenever:
- Code is written or modified
- Tests are executed
- A SIP is claimed as completed
- PR or change-log text is generated

SIP PRECONDITION — ACCEPTANCE TESTS

- A SIP is INVALID unless Acceptance Tests are explicitly defined.
- An Acceptance Criteria (AC) item is treated as covered only if an existing Acceptance Test (AT) explicitly asserts it (implicit coverage not counted).
- If Acceptance Tests are missing, implementation MUST NOT begin.
- This is a HARD STOP and MUST be reported as a failure of SIP definition.
- Once a test has been implemented and is stable it must NEVER be modified, altered or deleted in any way without explicit human confirmation.

====================================================================
EXECUTION REQUIREMENTS
====================================================================

Every test execution MUST include:
1. Exact command executed
2. Full stdout/stderr (verbatim, unabridged)
3. Explicit process exit code
4. Explicit number of tests executed

If ANY item is missing:
- Execution is invalid
- GREEN is not achieved
- DoD is not satisfied

====================================================================
BUILD-FIRST TEST EXECUTION
====================================================================

- The agent MUST NOT run any required `.NET` test command before the required `dotnet build` command has completed successfully.
- The agent MUST NOT run a required `dotnet test` command without `--no-build` when a successful prior build already exists for the same repository state.
- The agent MUST NOT run `dotnet build` and `dotnet test` concurrently.
- The agent MUST NOT treat a prior build as valid if any repository file has changed after that build completed.
- The agent MUST NOT continue the verification flow after repository state has changed until the required `dotnet build` command has been executed again successfully.

====================================================================
DoD PROGRESSION ENFORCEMENT
====================================================================

- If SIP-level DoD is not satisfied and the missing verification steps are known, the agent MUST execute them.
- If EPIC-closure DoD is not satisfied because the current task closes the final incomplete SIP of one EPIC, the agent MUST execute the required build and full C# test run.
- The agent MUST NOT run the full repository test suite for SIP-only completion unless the current task closes the final incomplete SIP of one EPIC or the human explicitly requests the full suite.
- The agent MUST NOT ask for permission or offer options when the next required step is unambiguous.
- Escalation to the user is permitted ONLY if:
  - Required commands are unknown, or
  - Execution would be destructive or irreversible.

====================================================================
EVIDENCE HANDLING
====================================================================

- Test results MUST NOT be summarized
- Test results MUST NOT be paraphrased
- Evidence MUST be shown verbatim

Textual claims without evidence are meaningless.

SIP-LEVEL EXECUTION EVIDENCE
- A SIP completion claim requires verbatim evidence for the SIP-defined Acceptance Test commands only.

EPIC-CLOSURE EXECUTION EVIDENCE
- If the current task closes the final incomplete SIP of one EPIC, the agent MUST also provide verbatim evidence for:
  1. one fresh repository build command
  2. one fresh full C# test run command
- The build command evidence MUST include:
  1. Exact command executed
  2. Full stdout/stderr (verbatim, unabridged)
  3. Explicit process exit code
- The full C# test run evidence MUST satisfy the standard test-execution evidence contract.
- These commands MUST be executed against the final repository state for the task.

SCOPING RULE
- If no EPIC transitions to complete in the current task, repository-wide build and full-suite evidence are not required for SIP completion claims.

====================================================================
FAILURE MEMORY
====================================================================

For every fix attempt, the following MUST be stated:
- Which specific test failed before
- Where it failed
- Where it is now proven to pass

Failure memory may NOT be omitted.

====================================================================
PR / CHANGE REPORTING
====================================================================

PR or status text MAY ONLY be generated after:
- The applicable DoD is satisfied
- Verifiable execution evidence is present

PR text MUST:
- Reflect exact scope completed
- Explicitly state deferred or out-of-scope work
- Avoid speculative or success language without proof

====================================================================
COMPLETION CLAIM VALIDATION (ADDITION)
====================================================================

REJECT any claim of completion if:
- any DoD condition defined in Skill 1 is violated
- any known incompleteness exists and has not been explicitly reported
- any user-visible behavior remains placeholder, stubbed, or non-meaningful
- multiple active implementations exist for the same logical behavior
- transitional or compatibility code exists without explicit human authorization
- EPIC-closure evidence is missing when the current task closes the final incomplete SIP of one EPIC
