---
name: sip-template
description: Use when creating, reading, updating, deleting, or changing status for Strategic Implementation Plan (SIP) and EPIC entries. Use this for SIP structure, formatting, acceptance criteria, and status transitions.
---

# Precedence

- This skill is operational and lower-priority than constitution/skill-1-immutable-authority.md, constitution/skill-2-execution-evidence.md, constitution/skill-3-domain-law.md, and constitution/skill-4-local-work-protection-case-law.md.
- If any instruction in this skill conflicts with any instruction in Constitution layers 1-4, the conflicting instruction is not applicable and must be ignored.

# SIP Template Skill

You are the authoritative editor for the Strategic Implementation Plan (SIP) list. You perform SIP CRUD and status operations. Enforce the SIP authoring format and vertical integrity rules without exception.

## Prohibitions (SIP scope)

- NO qualitative language: reject words like "clean", "robust", "maintainable", "simple", "elegant", "good".
- NO implementation advice: do not describe how work is performed, architectural choices, development flow, or TDD flow.
- NO narrative in status fields.
- NO "and" in Acceptance Criteria items unless it denotes conditions that must all be true at the same time.
- NO extension of the SIP format.
- NO deviation from the EXACT SIP template. Any SIP that does not follow the exact template is invalid and CANNOT be accepted.
- NO ambiguous intent. The intent MUST be explicit and unambiguous.
- NO unverifiable claims in the SIP.
- NO process-oriented claims in the SIP.
- NO prosaic, narrative, or descriptive claims in the SIP that cannot be objectively verified.
- NO claims without AT coverage. Every claim in the SIP MUST be covered by at least one AT.
- NO redundant ATs. Multiple ATs MUST NOT validate the same claim unless they cover distinct conditions.
- NO ATs that restate claims without testing them.
- NO ATs that test process-oriented claims.
- NO ATs that test more than one claim.
- NO ATs that test prosaic, narrative, or non-verifiable claims.
- NO ATs that test non-verifiable or non-falsifiable claims.

## SIP format

- EPIC identifier: EPIC-<n> (example: EPIC-1)
- SIP identifier: SIP-<n>.<m> (example: SIP-1.1)
- Every generated or edited SIP must use this format:

### **EPIC-<n>: [Title]**
- **Category:** [USER-FACING | SYSTEM-ONLY]
- *Objective: [Single paragraph describing the overarching intent. State the problem, not the mechanism.]*

- [ ] **SIP-<n>.<m>: [Concise, observable change]**
  - **Summary:** [Exactly one sentence describing what changes. No adjectives.]
  - **Objective (WHY):** [Human, business, compliance, or operational intent.]
  - **Scope Guard:** This SIP authorizes only the changes strictly required to satisfy the acceptance criteria. Incidental fixes, cleanups, refactors, or improvements outside the described change are not permitted.
  - **Dependencies:** [List of SIPs or contracts only.]
  - **Scope & Acceptance Criteria (WHAT must be true):**
    1. [Binary requirement 1]
    2. [Binary requirement 2]
  - **Acceptance Tests (THIS must work):**
    1. **AT1 - [Observable outcome via UI, API, or logs]**
    2. **AT2 - [Observable outcome via UI, API, or logs]**
  - **Explicit Non-Goals (OUT OF SCOPE):** [List what is not changed.]
  - **Status:** [Planned / Completed] [Date YYYY-MM-DD].

## Operational logic

- CREATE: increment identifiers based on the last entry (for example, EPIC-36, SIP-36.1).
- READ: return the requested EPIC/SIP block unchanged.
- UPDATE: rewrite only the requested SIP block while preserving all mandatory headers and Scope Guard.
- DELETE: remove only the specified SIP or EPIC and do not re-index.
- STATUS:
  - Planned -> Completed: set checkbox to [x] and status to Completed YYYY-MM-DD.
  - Completed -> Planned: set checkbox to [ ] and status to Planned YYYY-MM-DD.

## Validation (Structural & Integrity)

- **OBSERVABILITY:** If a requirement is not observable, rewrite it to be observable or reject it.
- **AT COVERAGE:** An Acceptance Criteria (AC) item is invalid if no existing Acceptance Test (AT) explicitly asserts it (implicit coverage not counted).
- **NEGATIVE PATHS:** Acceptance Tests for runtime behavior must include at least one negative-path test.
- **FORMAT ADHERENCE:** If a SIP is edited and no longer matches the structure defined in the template, the edit must be discarded.

## Vertical Integrity (Epic Cohesion)

- **CATEGORY DEFINITIONS:**
  - `USER-FACING`: The feature requires interaction by a human agent via a Graphical User Interface (GUI).
  - `SYSTEM-ONLY`: The feature is an API, service, or infrastructure component with no direct human UI.

- **COHESION RULES:**
  - **PROHIBITED:** `USER-FACING` EPICS that deliver logic/backend SIPs without a corresponding SIP for User Interface, UX Wiring, and Browser-based Verification.
  - **PROHIBITED:** `SYSTEM-ONLY` EPICS that deliver UI components.
  - **MANDATORY:** Every `USER-FACING` EPIC must include at least one SIP dedicated to the **Interface Layer** (Wiring the domain logic to the UI components).
  - **MANDATORY:** Every `SYSTEM-ONLY` EPIC must include at least one SIP dedicated to **Contract/Schema Verification** (API specs or DTO validation).

- **TESTING PARITY:**
  - For `USER-FACING` EPICS, the final "Completion" SIP must include Acceptance Tests (ATs) observable via the browser or UI-automation tool (e.g., Playwright). 
  - Backend-only tests (unit/integration) do NOT satisfy the AT requirements for the final delivery of a `USER-FACING` EPIC.

- **FAILURE STATE:**
  - If an EPIC is defined without satisfying these cohesion rules, the Agent MUST refuse implementation and report an **"Architectural Incompleteness"** error.
