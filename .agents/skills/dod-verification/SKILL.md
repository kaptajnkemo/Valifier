---
name: DoD Verification
description: Verify SIP-level and EPIC-closure DoD using generic execution-evidence rules without requiring repository-specific manifests.
---

# Precedence

- This skill is operational and lower-priority than:
  - constitution/skill-1-immutable-authority.md
  - constitution/skill-2-execution-evidence.md
  - constitution/skill-3-domain-law.md
  - constitution/skill-4-local-work-protection-case-law.md
- If any instruction in this skill conflicts with Constitution layers 1-4, output MUST explicitly state execution is blocked and cite the exact conflict reason, or the action must be rejected.

# DoD Verification Workflow

- This skill MUST be used when the user asks to verify DoD, asks whether SIP-level DoD has been satisfied, asks whether EPIC-closure DoD has been satisfied, or asks to close a SIP or EPIC with execution proof.
- The skill MUST determine whether the requested verification scope is SIP-level DoD, EPIC-closure DoD, or both.
- The skill MUST classify whether a DoD request is verification-only or execution-to-DoD based on the user's verb phrase.
- Verification-only phrasing includes requests such as `verify DoD`, `check DoD`, `audit DoD`, `review DoD`, `is this at DoD`, or `do we have DoD`.
- Execution-to-DoD phrasing includes requests such as `go for DoD`, `bring to DoD`, `take to DoD`, `get to DoD`, `reach DoD`, or `close this with DoD`.
- When the user's phrasing is execution-to-DoD, the agent MUST treat the request as authorization to perform the implementation, test, SIP-status, and verification work required to move the target from its current state to DoD.
- When the user's phrasing is verification-only, the agent MUST limit the response to assessing the current state against the applicable DoD rules unless the user separately authorizes implementation.
- The noun `DoD` does not by itself imply verification-only behavior; imperative phrasing that targets a state transition takes precedence.
- The skill MUST discover build and test entrypoints from task context, prior execution evidence, and standard repository entrypoints without requiring repository-specific manifests or governance artifacts.
- The skill MUST NOT invent commands when no authoritative entrypoint can be determined from the task context, prior repository usage in the thread, or standard repository entrypoints.
- If no authoritative verification entrypoint can be determined safely, execution MUST stop and output MUST state that DoD verification is blocked by unknown verification commands.
- The skill MUST execute verification only against the final repository state for the task under review.
- The skill MUST execute the required build step when the applicable DoD definition requires a fresh build.
- The skill MUST prefer one authoritative top-level verification command when one is already established by the task context, prior execution evidence, or a standard repository entrypoint.
- The skill MAY use multiple child commands only when no single authoritative command can be determined and the required verification scope can still be completed without inventing repository-specific governance.
- The skill MUST collect verbatim execution evidence for every authoritative command, including the exact command, full stdout/stderr, explicit exit code, and explicit test count when the command is a test run.
- The skill MUST reject any completion claim when any required evidence element is missing, any required suite was not executed, any exit code is non-zero, or any test run executed zero tests.
- The skill MUST state failure memory for every fix attempt by naming which test failed before, where it failed, and where it is now proven to pass.
- The skill MUST report whether SIP-level DoD has been verified and whether EPIC-closure DoD has been verified. The skill MUST NOT collapse those statuses into one claim when only one scope is proven.
- The skill MUST explicitly report any known blockers that prevent DoD verification, including unknown verification commands, failing suites, ambiguous scope, or incomplete evidence.
