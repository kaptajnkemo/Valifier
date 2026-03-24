# SKILL 1 — IMMUTABLE AUTHORITY

This file defines absolute authority for AI-assisted development.
It is LAW.

This file may NOT be rewritten, summarized, reordered, or token-optimized.

====================================================================
LAYER 0 — CONSTITUTION (IMMUTABLE)
====================================================================

This section is LAW.
It may NOT be rewritten, summarized, reordered, or token-optimized.

AUTHORITY
- Execution beats claims.
- Tests define truth.
- Exit codes define success.
- Files are user-owned.

COMPLETION
- Nothing is done without proof.
- Uncertainty == failure.
- Silence or omission == failure.

SAFETY
- Never delete, clean, stash, or modify untracked files without explicit permission.
- Never claim success without verifiable execution evidence.
- The original code we are converting MUST be treated READ-ONLY

PRECEDENCE
- These rules override all others.

====================================================================
LAYER 1 — OPERATING SYSTEM (STABLE)
====================================================================

Defines how work is conducted.
Procedural, mechanical, non-negotiable.

--------------------------------------------------
TDD & BUGFIX LOOP
--------------------------------------------------

RED
- Write or adjust tests until they FAIL.
- A test that passes immediately is invalid.

GREEN
- Implement the MINIMUM change required to pass the failing test.
- No refactors.
- No anticipatory fixes.

REFACTOR
- Behavior-preserving only.
- Tests must remain green.

--------------------------------------------------
TDD TEMPORAL ENFORCEMENT
--------------------------------------------------

- Implementation or modification of behavior is FORBIDDEN unless a relevant test has already been executed and observed to FAIL (RED) in the current working tree.
- RED is a precondition, not a retrospective requirement.
- If no failing test exists, the next action is to write or adjust a test until it FAILS, then run it and record full execution evidence.
- If implementation has already occurred without prior RED, STOP and report the violation. Do not propose retroactive fixes, reconstructions, or remediation options unless explicitly instructed.
- No further work may proceed until RED has been observed.

--------------------------------------------------
VERIFICATION & TERMINATION CONTRACT
--------------------------------------------------

GREEN DEFINITION
- GREEN == (process exit code == 0 AND tests executed > 0)
- Any non-zero exit code == NOT GREEN
- Zero tests executed == NOT GREEN
- Output text is irrelevant without explicit exit code and test count

NOTE
- dotnet test and Playwright test runners are authoritative.
- Exit codes emitted by these tools are binding.
- Once a test is implemented it may never be altered without human consent.

EXECUTION INTEGRITY
Every test run MUST include:
1. Exact command executed
2. Full stdout/stderr (verbatim, unabridged)
3. Explicit exit code
4. Explicit number of tests executed

Test results may NEVER be summarized or paraphrased.

BANNED WITHOUT PROOF
The following words are FORBIDDEN unless immediately preceded by verbatim successful execution output:
- fixed
- resolved
- working
- done
- all tests green

FAILURE MEMORY
For every fix attempt, you MUST state:
- Which specific test failed before
- Where it failed
- Where it is now proven to pass

DEFAULT BELIEF
- Assume the system is BROKEN unless execution proves otherwise.
- Optimism is a bug.
- Uncertainty counts as failure.

--------------------------------------------------
DEFINITION OF DONE (DoD)
--------------------------------------------------

SIP-LEVEL DoD
ALL of the following are required to claim one SIP complete:
- All SIP-defined Acceptance Tests have been executed and passed green
- Verbatim evidence shown (Exit code == 0 AND tests executed > 0)
- SIP has been marked complete in the SIP list
- Confirmation message in chat, stating whether SIP-level DoD has been verified or not

EPIC-CLOSURE DoD
ALL of the following are additionally required when the current task completes the final incomplete SIP of one EPIC:
- Fresh build of code executed successfully (dotnet build) against the final repository state for the task
- Fresh full C# test run executed successfully and passed green against the final repository state for the task
- Verbatim evidence shown for the build and full C# test run
- Confirmation message in chat, stating whether EPIC-closure DoD has been verified or not

EPIC-CLOSURE SCOPING
- Full-suite verification is required ONLY for EPICs that transition from incomplete to complete in the current task
- Previously completed EPICs do NOT trigger additional full-suite verification
- If one task closes multiple EPICs, one final-state build and one final-state full C# test run satisfy EPIC-closure DoD for all EPICs closed by that task
- The agent MUST NOT run the full repository test suite for SIP-only completion unless the current task closes the final incomplete SIP of one EPIC or the human explicitly requests the full suite

No exceptions.

--------------------------------------------------
DoD — COMPLETENESS & INTEGRITY EXTENSIONS
--------------------------------------------------

REJECT any SIP as complete if:
- any SIP-defined user-visible workflow is not implemented end-to-end

REJECT any SIP as complete if:
- passing tests is the only evidence while user-visible behavior remains incomplete

REJECT any SIP as complete if any user-reachable path contains:
- placeholder behavior
- stub implementations
- mock or fake data presented as real behavior
- empty or non-meaningful UI where meaningful output is expected

REJECT any SIP as complete if:
- implementation scope was limited to only what is required to pass tests while SIP-defined behavior remains unimplemented

REJECT any SIP as complete if:
- any known deviation exists between implemented behavior and SIP-defined behavior and has not been explicitly reported

REJECT any SIP as complete if:
- multiple active implementations exist for the same logical behavior

REJECT any SIP as complete if:
- transitional or compatibility code exists, OR
- such code is retained without explicit human authorization

REJECT any SIP as complete if any user-accessible path:
- produces non-meaningful output
- contains placeholder or scaffold behavior
- does not reflect fully implemented SIP-defined behavior

--------------------------------------------------
INTERPRETATION CLARIFICATION
--------------------------------------------------

- “Done”, “DoD”, or any derivative MAY ONLY refer to completion of the full SIP intent and all stated acceptance criteria when applied to one SIP.
- “Done”, “DoD”, or any derivative MAY ONLY refer to completion of the full EPIC intent and EPIC-closure DoD when applied to one EPIC.
- Partial implementation, incremental work, or intermediate code states are NOT “Done” under any circumstance.
- Execution evidence without fulfilled SIP intent does NOT constitute SIP-level DoD.
- Execution evidence without fulfilled EPIC intent and EPIC-closure verification does NOT constitute EPIC completion.
- "Next SIP" means any SIP that is ready and is not complete and not depending on any incomplete SIPs.

====================================================================
META-GOVERNANCE (CRITICAL)
====================================================================

INTERPRETATION ORDER
1. Constitution
2. Operating System
3. Domain Law
4. Case Law

RULE CREATION POLICY
A rule may be added ONLY if:
- A concrete failure occurred
- The rule prevents that exact failure
- The rule does not contradict higher layers

MAINTENANCE
- Rule consolidation or removal requires explicit user approval.
- Layer 0 and Layer 1 may NEVER auto-expand.

====================================================================
SYSTEM INTENT (NON-NORMATIVE)
====================================================================

This system does not trust confidence.
It trusts evidence.
