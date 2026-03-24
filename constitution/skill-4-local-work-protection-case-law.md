# SKILL 4 — LOCAL WORK PROTECTION CASE LAW

This file defines repository safety rules derived from a concrete destructive-action failure.
It is subordinate to Skill 1, Skill 2, and Skill 3.

====================================================================
RULE
====================================================================

- Local files and local repository state are user-owned.
- The agent MUST NOT delete, overwrite, reset, restore, discard, replace, or otherwise destroy uncommitted local work by using Git, `HEAD`, `origin`, or any other local or remote reference without explicit human approval for that exact destructive action.
- The agent MUST treat staged changes, unstaged tracked changes, untracked files, and ignored files as protected local work whenever they could be affected.

====================================================================
INFERENCE
====================================================================

- Whenever the agent infers a destructive repository action from the user's instruction, the agent MUST state the destructive action it believes the user means before taking that action.
- The agent MUST NOT execute destructive intent based only on inference.
- If inferred or explicit destructive intent could affect local work, the agent MUST STOP, inspect the working tree, report which categories of local work may be affected, and ask for explicit confirmation before proceeding.
- This rule applies even when the agent does not consider the user's wording ambiguous.
- Multi-part requests MUST be split into safe and destructive portions. Safe portions MAY proceed only if they do not depend on destructive repository actions.

====================================================================
APPROVAL GATE
====================================================================

- The agent MUST NOT run `git clean`, `git reset --hard`, `git checkout --`, `git restore`, force checkout, force pull, rebase, merge, stash drop, branch deletion, or any equivalent destructive command if local work could be removed or overwritten, unless the human has first typed exactly: `I APPROVE DELETION OF LOCAL FILES`
- The approval phrase applies only to the specific destructive action under discussion. It MUST NOT be reused.
- If uncertainty remains about what would be affected, the agent MUST NOT proceed.

====================================================================
BRANCH WORKFLOW
====================================================================

- The agent MUST NOT commit work on `main`.
- The agent MUST NOT push directly to `main`.
- The agent MUST create a local branch before implementation work starts.
- The name of the branch MUST be a shortened but meaningful version of the EPIC name.
- All implementation work MUST be committed to that branch.
- The agent MUST push the branch and prepare a PR for human review instead of publishing directly to `main`.
- Merge to `main` is reserved for the human after review and human oversight.
- If the agent is on `main` when implementation work is about to begin, the agent MUST STOP and create a branch before making changes.
- If the agent is on `main` after implementation work has already been performed, the agent MUST STOP and report that the required branch workflow has already been violated.

====================================================================
PR TEMPLATE
====================================================================

- The agent MUST prepare a PR description for implementation work committed to a branch.
- The agent MUST provide the PR title and PR description text in chat immediately after the branch is pushed, unless the user explicitly asked only for branch creation or branch push.
- The agent MUST NOT treat branch creation or branch push as completion of PR preparation.
- `Prepare the PR` MUST be interpreted to include both PR-ready Git state and the PR title and description content.
- The PR description MUST contain:
  - EPIC name
  - list of included SIPs
  - the intent of the EPIC
  - scope
  - brief DoD status
  - known deviations or blockers
  - risks
- The brief DoD status MUST include test counts in the form: tests passed, tests failed, tests run.
- The PR description MUST keep verification information aggregated and brief. It MUST NOT dump raw test output or long command-by-command logs into the PR body.

====================================================================
INTERPRETATION
====================================================================

- `Commit the branch and clean local repo` MUST be interpreted as separate intents.
- The commit portion MAY proceed only if it does not require destructive repository actions.
- The cleanup portion MUST remain blocked until destructive intent is clarified and the approval gate is satisfied when local work may be affected.

====================================================================
FAILURE
====================================================================

- Any destructive overwrite or deletion of protected local work without the exact approval phrase is a constitutional failure.
