# Legacy Knowledge Base — Maintenance Guide

**Purpose:** Governance and maintenance rules for `Docs/KB/legacy/`.
**Scope:** Update rules, budgets, validation, evidence citation, version labelling.
**Audience:** Maintainers, contributors, AI coding agents.

---

## 1. Purpose

This guide defines how the legacy knowledge base under `Docs/KB/legacy/` is maintained. It reuses the
conventions of `Docs/KB/CONVENTIONS.md` and the governance of `scripts/kb_build.py` /
`scripts/kb_check.py`.

## 2. Core rules

1. **Code is the reference.** Documentation is a navigation layer, not a substitute. (AGENTS.md rule 1.)
2. **Rules live in `Domain/Rules/` only** — never duplicated in Application or presentation.
   (AGENTS.md rule 2.)
3. **Do not modify an applied migration** — create a new one. (AGENTS.md rule 3.)
4. **Do not apply migrations automatically** — schema application is the owner's decision.
   (AGENTS.md rule 4.)
5. **Arabic strings come from `Docs/ui-labels-verified.md` only** — never invented.
   (AGENTS.md rule 7.)
6. **Single-site laboratory** — no branch column, no branch table, no per-branch schema.
   (AGENTS.md rule 8.)

## 3. Update rules for legacy docs

1. **One fact, one home.** Each technical fact lives in exactly one domain doc. Other docs
   cross-reference by repository-relative path. Never duplicate a full explanation.
2. **Version label every fact.** Use V1 ONLY / V2 ONLY / SHARED / VERSION-DEPENDENT / UNKNOWN.
   Never assume a V2 observation applies to V1, or vice versa.
3. **Cite sources.** Every non-trivial claim cites its source path and evidence type code
   (ET-DOC, ET-DB, ET-CODE, ET-DDL, ET-CFG, ET-INST, ET-DERIVED, ET-PRES) plus confidence level.
4. **Preserve verification status.** A statement in an existing report is not automatically a
   verified fact. Carry over the original confidence level.
5. **Do not upgrade interpretations.** Interpretations stay marked as INFERRED or HYPOTHESIS.
6. **Register contradictions.** New contradictions go into `Docs/KB/legacy/evidence/CONTRADICTIONS.md` first,
   then are referenced from domain docs. Never silently resolve.
7. **Register unresolved items.** New open questions go into `Docs/KB/legacy/evidence/UNRESOLVED.md` first.
8. **Preserve originals.** The large source documents (`Database documentation.md`,
   `Documenting the Executable File.md`, `Docs/Complete_Reverse_Engineering_Reference_V2.md`,
   `Docs/Business_Rules_V2.md`, etc.) are retained as evidence. Merging means extracting facts,
   not deleting sources.
9. **External sources.** The V1 consolidated documents are **accessible** — they are present in the
   reverse-engineering corpus and are already integrated (resolved 2026-11-16). Their per-version
   facts live in `VERSIONS.md`; their dispositions in `Docs/KB/legacy/evidence/SOURCES.md`. Do not fabricate
   contents. A statement in this knowledge base that has no entry in `Docs/KB/legacy/evidence/SOURCES.md` has **no
   traceable origin** and must not be relied upon.

10. **Register every source.** Any source added to the corpus or to a deliverables archive must get
    a row in `Docs/KB/legacy/evidence/SOURCES.md`, a review status and an explicit integration decision —
    including the decision *"excluded, because …"*. `Docs/KB/legacy/evidence/COVERAGE.md` records the accounting.

11. **Canonical descriptions.** Every major subject has exactly one authoritative document
    (`SPECIFICATION.md`, `data/DATABASE.md`, `architecture/EXECUTABLE.md`, `VERSIONS.md`). Other
    documents reference the canonical one; they do not restate it.
10. **No secrets.** Never reproduce actual credentials (`sa`/`12345678`, `admin`/`admin`) in a
    way that could be used; preserve findings in redacted form.

## 4. Document budgets

Budgets are in **characters** (not bytes), per `kb_check.py` convention. The audit suggested
3–8 KB as a guideline, not a hard limit. Content requirements take priority.

| Document | Budget (chars) |
|---|---:|
| `legacy/INDEX.md` | 6,000 |
| `legacy/VERSIONS.md` | 8,000 |
| `legacy/architecture/OVERVIEW.md` | 8,000 |
| `legacy/architecture/DEPLOYMENT.md` | 8,000 |
| `legacy/architecture/CONFIGURATION.md` | 8,000 |
| `legacy/architecture/PDF-PRINTING.md` | 10,000 |
| `legacy/data/MODEL.md` | 10,000 |
| `legacy/data/MIGRATION.md` | 8,000 |
| `legacy/data/IDENTIFIERS.md` | 6,000 |
| `legacy/data/BRANCHES.md` | 6,000 |
| `legacy/clinical/RULES.md` | 10,000 |
| `legacy/clinical/KNOWLEDGE-LAYER.md` | 8,000 |
| `legacy/clinical/PROFILES.md` | 8,000 |
| `legacy/business/WORKFLOWS.md` | 10,000 |
| `legacy/business/SECURITY.md` | 10,000 |
| `legacy/business/CAPABILITIES.md` | 8,000 |
| `legacy/evidence/EVIDENCE-REGISTER.md` | 8,000 |
| `legacy/evidence/CONTRADICTIONS.md` | 12,000 |
| `legacy/evidence/UNRESOLVED.md` | 12,000 |
| `legacy/mapping/LEGACY-TO-MODERN.md` | 12,000 |
| `legacy/MAINTENANCE.md` | 6,000 |

A document may exceed its budget when the additional detail is necessary to preserve meaningful
technical knowledge. If it grows too large, split along genuine domain boundaries and update
navigation.

## 5. Governance integration

- `scripts/kb_build.py` and `scripts/kb_check.py` must be extended to cover `Docs/KB/legacy/*`.
- `CURATED_DOCS` in `kb_check.py` must include all 18 legacy curated docs.
- `BUDGETS` in `kb_check.py` must include the budgets above.
- Internal links (V6) must resolve.
- Legacy evidence path existence (V1) must distinguish repository sources, external references,
  and intentionally unavailable historical sources.
- Fingerprinting (V3) must cover the legacy subtree.
- Duplicate-content (V7) and numeric-claims (V9) checks apply.
- Convention guards (V10) apply where relevant.

## 6. Validation commands

```powershell
python scripts/kb_build.py    # regenerate indexes from code
python scripts/kb_check.py    # fingerprint · drift · coverage · links · budgets
```

`kb_check.py` exits non-zero on the first failing category. Do not bypass or disable it.

## 7. When to update

- When new legacy findings are consolidated (after explicit authorization to resume corpus analysis).
- When a contradiction is resolved (update `CONTRADICTIONS.md` status + reasoning).
- When an unresolved question is answered (move from `UNRESOLVED.md` to the relevant domain doc).
- When the modern system's implementation changes (update `LEGACY-TO-MODERN.md`).
- When source evidence is added or removed (update `EVIDENCE-REGISTER.md`).

## 8. Navigation

- Entry point: `Docs/KB/START-HERE.md` → `Docs/KB/legacy/INDEX.md`.
- Each domain doc links to related docs and source evidence.
- `INDEX.md` is the legacy-only entry point; it must remain concise and navigation-oriented.

## 9. Related documents

- `Docs/KB/START-HERE.md` — modern entry point (extended with legacy pointer).
- `Docs/KB/CONVENTIONS.md` — modern conventions (traps, guards).
- `Docs/KB/ARCHITECTURE.md` — modern architecture.
- `AGENTS.md` — agent rules.
- `Docs/KB/legacy/INDEX.md` — legacy entry point.
- `scripts/kb_build.py`, `scripts/kb_check.py` — governance tooling.
