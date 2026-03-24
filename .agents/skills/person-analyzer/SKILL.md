---
name: Person Analyzer
description: Trigger on person analysis requests and produce a single auditable person-analysis profile with reproducible evidence trails and appendices.
---

# Precedence

- This skill is operational and lower-priority than:
  - constitution/skill-1-immutable-authority.md
  - constitution/skill-2-execution-evidence.md
  - constitution/skill-3-domain-law.md
  - constitution/skill-4-local-work-protection-case-law.md
- If any instruction in this skill conflicts with Constitution layers 1-4, execution is blocked and the exact conflict reason must be stated.

# Person Analyzer Skill

## Trigger criteria
- This skill MUST be used when the user explicitly requests to analyze, profile, or evaluate a person.
- This skill MUST be used for phrases equivalent to `analyze a person`, `person analysis`, `analyze [name]`, `build a profile of [name]`.
- This skill MUST accept URL anchors in the initial request and use them as identity evidence, including (non-exhaustive) profile pages on LinkedIn, Wikipedia, official bios, company pages, and registry filings.
- URLs are preferred but not required at trigger time.
- When one or more URLs are provided, this skill MUST treat them as a primary identity lock and require all subsequent candidate generation to remain constrained to the same identity context.
- All expansion searches triggered from a provided URL MUST include at least one anchor-derived disambiguator (for example: employer, location, handle, title, industry, or verified organization name) in every query string.
- If URLs are not available, the trigger MUST include at least two independent identity descriptors (for example: occupation + employer, occupation + location, name variant + organization, alias + domain).
- If no URLs are provided and descriptors are insufficiently unique, this skill MUST run a quick lookup pass (using reproducible query patterns and recording in `search_log.md`) to produce grounded candidate identities and their evidence, then generate a disambiguation question with candidate summaries and wait for explicit user confirmation before collecting evidence.
- Accepted trigger examples MUST include:
  - `Analyze a person: Michael Johnson. LinkedIn: https://...`
  - `Build a profile of Michael Johnson (Wikipedia: https://..., LinkedIn: https://...)`
  - `Person analysis: Michael Johnson, official site: https://...`
- This skill MUST NOT be used for non-human entities, organizations, products, or fictional groups unless explicitly requested as a named person.
- If the target cannot be uniquely identified, the skill MUST pause and request disambiguation before analysis begins. The request MUST include candidate summaries rather than assuming a single person.

## Scope and output contract
- This skill MUST perform only person-level analysis and MUST NOT infer facts from unlabeled or unverified claims.
- This skill MUST produce:
  - one atomic main profile document in Markdown only (`person_analysis/output/profile_main.md`)
  - appendices containing sources, uncertainty, conflict tracking, corpus statistics, and evidence mappings
  - a reproducible audit trail of all transformations from raw inputs to outputs
- This skill MUST NOT include private family details, minors unless explicitly requested, or unverifiable rumors as facts.
- This skill MUST NOT produce PDF or non-Markdown artifacts for the final profile.

## Enforced model: separate state from views
- State artifacts MUST be preserved as iterated inputs:
  - evidence registry
  - fact ledger
  - corpus index
  - raw text and preprocessing artifacts
  - feature tables
  - excerpt mappings
  - conflict tables
- Views MUST be regenerated from state assets:
  - biography narrative
  - values map
  - decision model
  - stylistic signature
  - psychometrics hypotheses
  - final profile document

## Data model and taxonomy
- Source tier tags MUST be one of: `Tier1`, `Tier2`, `Tier3`.
- Authorship certainty MUST be one of: `High`, `Medium`, `Low`.
- Editing risk MUST be one of: `Low`, `Medium`, `High`.
- Every fact and inference MUST cite source/evidence identifiers.

## Required repository layout
- This skill MUST generate and/or use:
  - `person_analysis/spec.yml`
  - `person_analysis/data/identity_profile.json`
  - `person_analysis/data/evidence_registry.csv`
  - `person_analysis/data/search_log.md`
  - `person_analysis/data/fact_ledger.csv`
  - `person_analysis/data/contradiction_table.csv`
  - `person_analysis/data/corpus_index.csv`
  - `person_analysis/data/corpus_raw/`
  - `person_analysis/data/corpus_clean/` (optional)
  - `person_analysis/data/preprocess_log.md`
  - `person_analysis/data/stylometry_features.csv`
  - `person_analysis/data/values_evidence.csv`
  - `person_analysis/data/decision_evidence.csv`
  - `person_analysis/data/psychometrics_hypotheses.csv`
  - `person_analysis/data/coverage_report.md`
  - `person_analysis/appendices/sources.md`
  - `person_analysis/appendices/uncertainty_register.md`
  - `person_analysis/appendices/contradictions.md`
  - `person_analysis/appendices/preprocessing.md`
  - `person_analysis/appendices/corpus_stats.md`
  - `person_analysis/appendices/evidence_to_construct_matrix.md`
  - `person_analysis/appendices/manifest.json`
  - `person_analysis/appendices/changelog.md`
  - `person_analysis/output/profile_main.md`

## Step 0 — Pre-flight specification
- This skill MUST create `spec.yml` before collecting or inferring any psychological claims.
- `spec.yml` MUST define:
  - target identity scope with aliases, handles, do-not-confuse list
  - languages, platforms, capture constraints, paywall policy, privacy exclusions
  - final document sections and construct set
  - quality gates for corpus size and fact-source minimums
  - required target coverage grid (platform × time × genre)
- This skill MUST explicitly mark out-of-scope elements.

## Step 1A — Background acquisition and disambiguation
- This skill MUST collect identity evidence before other acquisition.
- This skill MUST parse and prioritize user-supplied identity anchors (URL, ID, role, organization) as identity candidates.
- This skill MUST always ground candidate identities in recorded evidence from `search_log.md` before presenting them for user confirmation.
- When a high-trust URL anchor is present, this skill MUST create or confirm `identity_profile.json` directly from that source first, then require one additional corroborating evidence source before proceeding.
- If no URL anchors are supplied, this skill MUST rely on the user-supplied identity descriptors and only proceed when at least two independent corroborating descriptors are present.
- This skill MUST create `identity_profile.json` with confirmed vs assumed identifiers.
- This skill MUST require at least two independent confirming identifiers before proceeding.
- If identity ambiguity remains after a single-pass query, this skill MUST STOP and request user resolution using explicit options.

## Step 1A Category and evidence capture requirements
- This skill MUST harvest categories in this order:
  1. Authoritative biographies
  2. Corporate/ownership/roles
  3. Projects and activities
  4. Authored communications
  5. Legal/regulatory artifacts
- This skill MUST maintain `search_log.md` with query strings, dates, results, and extracted material.
- Each captured artifact MUST record:
  - ref/URL
  - publisher
  - date accessed
  - capture artifact or PDF/screenshot reference when possible
  - relevance note
- This skill MUST enforce the verification policy:
  - Tier1 sources are acceptable alone
  - Tier2 sources require corroboration by another Tier1 or Tier2 source
- This skill MUST create a gap log for missing time periods, genres, languages, and long-form authored artifacts.

## Step 1B — Evidence harvesting (implementation pass)
- This skill MUST collect raw artifacts and update `search_log.md` before any inference.
- This skill MUST prioritize Tier1 and long-form authored text first.
- This skill MUST NOT copy or store content that violates access restrictions or privacy constraints.

## Step 2 — Evidence registry
- This skill MUST create `evidence_registry.csv` with schema:
  - `source_id,url_or_ref,source_type,tier,authorship_certainty,editing_risk,language,date_published,date_accessed,topic_tags,use_facts,use_stylometry,use_values,notes`
- This skill MUST keep source tier, authorship certainty, and editing risk as separate fields.
- This skill MUST NOT store psychological interpretation in registry rows.

## Step 3 — Fact ledger
- This skill MUST create `fact_ledger.csv` with schema:
  - `claim_id,claim_text,claim_type,start_date,end_date,date_precision,confidence,evidence_source_ids,contradicts_claim_ids,notes`
- This skill MUST represent only atomic, time-bounded claims in the ledger.
- This skill MUST link disagreements via `contradicts_claim_ids`.
- This skill MUST record controversies as events with outcomes, not speculative motive conclusions.

## Step 4 — Corpus construction (two-track)
- This skill MUST build a stylometry corpus and a values/psychometrics corpus as distinct paths.
- Stylometry corpus inclusion MUST require `authorship_certainty=High`.
- Stylometry corpus SHOULD reject translations, paraphrases, and ghostwritten summaries.
- Stylistic corpus MUST keep raw and cleaned variants; raw must remain canonical source truth.
- Psychometrics/values corpus MUST include broader material with explicit weighting notes and author-certainty tags.

## Step 5 — Preprocessing
- This skill MUST preserve punctuation, case, emojis, and intentional errors in cleaned text.
- This skill MUST normalize whitespace and replace URLs with `<URL>`.
- This skill MUST never perform spell correction, paraphrasing, or translation.
- This skill MUST maintain both raw and cleaned copies where possible.

## Step 6 — Stylometry and linguistic profiling
- This skill MUST generate:
  - `stylometry_features.csv`
  - `stylometry_signature.md`
  - `mimic_detection_checklist.md`
- Stylometry features MUST be auditable and grouped across:
  1) orthography
  2) punctuation
  3) formatting
  4) structure
  5) function words
  6) rhythm
- This skill MUST compare within-platform stability before cross-platform drift.
- This skill MUST require multi-signal matches for mimic detection decisions.
- This skill MUST NOT use issue stance as identity signal.
- This skill MUST separate language-specific models.

## Step 7 — Values mapping (Schwartz)
- This skill MUST generate:
  - `values_evidence.csv`
  - `values_summary.md`
- The values schema MUST include:
  - `evidence_id,doc_id/source_id,excerpt,candidate_value,confidence,confounds,counter_evidence_refs,notes`
- The skill MUST track stated vs revealed values separately.
- The skill MUST include value tension pairs and counter-evidence references.
- The skill MUST NOT collapse findings into one score without documented weights.

## Step 8 — Decision-model reconstruction
- This skill MUST generate:
  - `decision_operators.md`
  - `argument_templates.md`
  - `decision_evidence.csv`
- Operators MUST be behavior-based (`trigger → move → output`) and evidence-linked.
- The skill MUST map each operator to multiple observed instances.
- The skill MUST never output trait labels in decision output.

## Step 9 — Psychometrics hypotheses
- This skill MUST generate:
  - `psychometrics_hypotheses.csv`
  - `psychometrics_report.md`
- `psychometrics_hypotheses.csv` MUST use schema:
  - `hypothesis_id,construct,statement,supporting_evidence_ids,counter_evidence_ids,confidence,confounds,notes`
- Evidence MUST be stratified by stakes and genre first, then compared for stability vs flips.
- Every hypothesis MUST include at least one counter-evidence reference.
- This skill MUST NOT present trait output as diagnosis.
- MBTI-type output MUST remain provisional and MUST be marked MBTI-style descriptor unless corpus minimums and balance are met.

## Step 10 — Main profile assembly
- The final output MUST be Markdown-only and include:
  1) executive synthesis (stable findings only)
  2) biography and timeline
  3) activity and institution map
  4) writing style profile
  5) values map with stated vs revealed tension
 6) decision model
 7) psychometrics hypotheses
- This skill MUST provide explicit evidence IDs for every meaningful statement in main output.
- This skill MUST keep facts and hypotheses visually and structurally separate.
- This skill MUST include required appendices listed above.

## Step 11 — QA and reproducibility
- This skill MUST produce:
  - `manifest.json`
  - `changelog.md`
  - `coverage_report.md`
- QA checks MUST verify:
  - each fact has evidence and confidence
  - contradictions are explicit
  - stylometry corpus excludes unclear authorship, translations, and low-certainty editing categories
  - psychometric hypotheses include support and counter-evidence plus confounds
  - no reverse inference from psychometrics to facts

## Non-negotiable controls
- This skill MUST stop and request missing information when:
  - identity ambiguity remains unresolved
  - required source tiers are absent for key claims
  - corpus size is below gates
  - high-conflict evidence is unresolved
- This skill MUST keep uncertainty, gaps, and limitations in structured sections, not in narrative-only form.
- This skill MUST preserve evidence identifiers unchanged across outputs so claims can be traced and re-verified.
