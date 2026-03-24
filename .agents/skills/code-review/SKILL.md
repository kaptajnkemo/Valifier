---
name: Code Review
description: Use when requesting a review-only audit of production code and associated tests. Use this for findings-only review, quality, architecture, security, performance, and test adequacy checks without proposing fixes or patches.
---

# Precedence

- This skill is operational and lower-priority than constitution/skill-1-immutable-authority.md, constitution/skill-2-execution-evidence.md, constitution/skill-3-domain-law.md, and constitution/skill-4-local-work-protection-case-law.md.
- If any instruction in this skill conflicts with any instruction in Constitution layers 1-4, the conflicting instruction is not applicable and must be ignored.

# Review-Only Code Review Skill

Use this skill for static analysis of production code and associated tests. Report findings only. Do not provide fixes, refactors, implementations, patches, diffs, or execution output.

## Non-negotiable boundaries

This skill is strictly review-only.

It MUST:
- analyze provided production code and associated tests
- identify and classify observable issues
- report findings only

It MUST NOT:
- output source code, patches, diffs, or rewritten examples
- propose concrete refactorings or implementation steps
- modify APIs, classes, or logic
- run, simulate, or benchmark execution

If the user requests edits anyway:
- state this skill is review-only
- continue with findings-only review

## Evidence policy

- Include short excerpts only as evidence and always with file path and location.
- Each excerpt must be no more than 25 words.
- Do not invent speculative issues.
- Findings must be grounded in visible code or tests.

## Input and scope handling

Preferred inputs:
- production code and tests in the workspace
- optional: target paths, PR diff, or commit range

If scope is unspecified:
- review explicitly provided files
- if none are provided, review the smallest reasonable scope from the workspace (changed files first, then entrypoints and closest tests)
- always list exactly what was reviewed

If tests are missing from scope:
- note it as a limitation
- include this finding in the Testing Requirements section

## Topic order (required)

Evaluate topics in this exact order:
1. Code Correctness
2. Code Stability
3. Architectural Correctness
4. Security
5. Performance
6. Testing Requirements

## Required output structure

### Review Scope
- production: <Full | Explicit list>
- tests: <Full | Explicit list>
- limitations: <fact-based constraints>

### 1. Code Correctness
- No issues found OR findings

### 2. Code Stability
- No issues found OR findings

### 3. Architectural Correctness
- No issues found OR findings

### 4. Security
- No issues found OR findings

### 5. Performance
- No issues found OR findings

### 6. Testing Requirements
- No issues found OR findings

## Finding schema

Each finding must include:
- Finding ID: <TopicPrefix>-<incrementing integer> (examples: CC-1)
- Location: <path>:<line range if available> - <identifier>
- Defect class/pattern
- Evidence: short excerpt plus explanation
- Likely scope: Isolated or Systemic
- Preventive guardrail category: test, linter, architecture rule, security scanner, etc.

No fix instructions, implementation guidance, or code changes.

## Topic checks

### 1. Code Correctness
- Report observable violations and contradictions between declared contracts and tests.

### 2. Code Stability
- Report observable side effects, temporal coupling, hidden state, ordering assumptions, and excessive conditional complexity.

### 3. Architectural Correctness
- Report dependency boundary violations, framework leakage in domain/use-case layers, or other architectural drift.

### 4. Security
- Report observable unsafe input handling, secret exposure, or missing validation at proven trust boundaries.

### 5. Performance
- Report observable inefficiencies such as unbounded loops, repeated expensive operations in loops, blocking I/O in critical paths, or asymptotic risk tied to unbounded inputs.

### 6. Testing Requirements
- Require visible tests for accepted, rejected, and edge outcomes at key public boundaries.
- Missing coverage must be reported from the reviewed scope.
