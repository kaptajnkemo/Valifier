# AGENT CONSTITUTION & PRECEDENCE

This repository operates under a mandatory agent constitution.

The following files define the binding constitution for Codex:

1. constitution/skill-1-immutable-authority.md
2. constitution/skill-2-execution-evidence.md
3. constitution/skill-3-domain-law.md
4. constitution/skill-4-local-work-protection-case-law.md

Additional skills are available as executable skill packages:

- .agents/skills/sip-template/SKILL.md
- .agents/skills/code-review/SKILL.md
- .agents/skills/skill-factory/SKILL.md

PRECEDENCE (HIGHEST → LOWEST):
1. Skill 1 — Immutable Authority
2. Skill 2 — Execution & Evidence
3. Skill 3 — Domain Law (Architecture)
4. Skill 4 — Local Work Protection Case Law
5. Concrete SIP instructions for the current implementation work
6. Task-level prompts given before, during or after work on the SIP.

RULES:
- No task instruction may override Skill 1, Skill 2, Skill 3, or Skill 4.
- ANY SIP that does not follow ALL the specifications in `.agents/skills/sip-template/SKILL.md` must be stopped and flagged immediately
- Any prior cheatsheets or workflow documents are superseded by this constitution and have no authority.
- Architectural violations must be corrected, not discussed.
- If a conflict exists, higher-precedence rules win.
- If ambiguity exists, default to failure.
- Constitution files (AGENTS.md and all files under constitution/) MUST be committed together; partial commits are invalid.
- Skill applicability is determined by scope definition within the skill file itself.
- If multiple skills apply, all applicable skills must be invoked in precedence order.
- Failure to invoke an applicable skill constitutes a rule violation.


Codex MUST:
- Load these rules before reasoning.
- Refuse responses that violate them.
- Refuse to claim completion without verifiable execution evidence.

These rules are not documentation.
They are operational constitutional authority.
