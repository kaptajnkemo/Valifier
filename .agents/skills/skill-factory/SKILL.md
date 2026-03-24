---
name: Skill Factory
description: Create a new Codex skill package under .agents/skills with required SKILL.md structure and OpenAI skill standards.
---

# Precedence

- This skill MUST be treated as operational only and lower-priority than `constitution/skill-1-immutable-authority.md`, `constitution/skill-2-execution-evidence.md`, `constitution/skill-3-domain-law.md`, and `constitution/skill-4-local-work-protection-case-law.md`.
- Constitution layers 1-4 always take precedence. If any requested action conflicts with any requirement in Constitution layer 1, Constitution layer 2, Constitution layer 3, or Constitution layer 4, execution is forbidden to proceed; output must state execution is blocked with the exact conflict reason, request clarification, and/or reject the request.
- This skill MUST NOT modify `constitution/skill-1-immutable-authority.md`, `constitution/skill-2-execution-evidence.md`, `constitution/skill-3-domain-law.md`, or `constitution/skill-4-local-work-protection-case-law.md`.
- If any requested action conflicts with any requirement in Constitution layer 1, Constitution layer 2, Constitution layer 3, or Constitution layer 4, all output MUST include that execution is blocked with the exact conflict reason, or the request must be rejected.

# OpenAI skill standard (required summary)

- A skill is a folder under `.agents/skills`.
- The folder must contain a top-level `SKILL.md`.
- Optional skill subfolders MAY only be created from this set when requested: `scripts/`, `references/`, `assets/`, and `agents/openai.yaml`.
- `SKILL.md` MUST include YAML frontmatter with `name` and `description`.

This skill MUST be used when the user asks to make a new skill or says "let's make X into a skill" (for example: `mySmartFileFilter`).

# Instruction hardening (required)

- All user-provided operational instructions MUST be normalized into enforceable form before being written into `<Skill purpose heading>`.
- Weak language (for example: `should`, `prefer`, `try`, `avoid when possible`, or similar) MUST be rewritten into explicit enforcement language using `MUST`, `MUST NOT`, and explicit rejection or block language.
- Positive-only directives MUST be converted to negative-control form where equivalent. Example:
  - Input: `all numbers should be spelled instead of written as digits`
  - Output: `All output MUST NOT contain digits. All numeric values MUST be spelled out in words.`
- If normalization would change meaning, execution is forbidden until the user clarifies intent.

# Scope

Create exactly one skill package per request.

1. MUST generate the skill directory at:
   - `.agents/skills/<slug>/`
2. MUST create `.agents/skills/<slug>/SKILL.md`.
3. MUST use only the requested standard content for the new skill package.
4. MUST NOT change existing files outside the generated path unless the user explicitly asks.

# Input fields (derive from user request)

- `desired_skill_name` (required): user-facing name or identifier to appear in `name:`. Missing value MUST trigger a required follow-up and must not proceed.
- `description` (required): concise purpose line for `description:`. Missing value MUST trigger a required follow-up and must not proceed.
- `make_optional`: optional list from `{scripts, references, assets, agents-openai-yaml}`.
- `requested_slug` (optional): exact folder name override. If provided, this value MUST be used exactly.

# Canonicalization

- If `requested_slug` is missing, derive `<slug>` by:
  - converting `desired_skill_name` to lowercase,
  - replacing spaces, underscores, and camel-case boundaries with `-`,
  - removing non-alphanumeric characters,
  - collapsing multiple `-` and trimming.
- Example: `mySmartFileFilter` -> `my-smart-file-filter`.
- If slug is empty after canonicalization, execution is forbidden to continue; a valid name MUST be requested before proceeding.

# Required output shape for new skill `SKILL.md`

```markdown
---
name: <desired_skill_name>
description: <description>
---

# Precedence

- This skill is operational and lower-priority than:
  - constitution/skill-1-immutable-authority.md
  - constitution/skill-2-execution-evidence.md
  - constitution/skill-3-domain-law.md
  - constitution/skill-4-local-work-protection-case-law.md
- If any instruction in this skill conflicts with Constitution layers 1-4, output MUST explicitly state execution is blocked and cite the exact conflict reason, or the action must be rejected.

# <Skill purpose heading>

... user-provided operational instructions ...
```

# Validation rules

- The directory must be under `.agents/skills`.
- `SKILL.md` must exist at the generated path.
- `SKILL.md` must include:
  - YAML frontmatter with `name` and `description`;
  - an explicit precedence block as above;
  - an implementation purpose section.
- If `make_optional` includes a value, the listed folder/file paths MUST be created.
- If target path already exists, execution is forbidden to overwrite existing files.

# Operation sequence

1. MUST parse requested `desired_skill_name`, optional `description`, and optional `make_optional`.
2. MUST resolve `<slug>` from `requested_slug` or canonicalized name.
3. MUST validate no conflict with existing constitution files.
4. MUST harden all user-provided instructions into enforceable form, including negative-control conversion where required.
5. MUST create the folder and write `SKILL.md` using the required template plus hardened user-provided purpose.
6. MUST confirm created paths with exact file locations and what was skipped.

# Failure handling

- If any requested behavior violates Constitution layers 1-4, output MUST stop and return the exact conflict reason; all output MUST include this rejection path or the action is rejected.
- If required inputs are missing, execution is forbidden to continue. A targeted follow-up question MUST be asked before creating any path.
