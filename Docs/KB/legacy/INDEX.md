# Legacy Knowledge Base — Index

**Purpose:** Single entry point for all legacy-system knowledge (`V-ROOT-1.00` and `V-CD-2019`).
**Scope:** Navigation only — no detailed technical knowledge.
**Audience:** All developers, architects, analysts, AI coding agents.

---

## 1. What this knowledge base covers

The legacy knowledge base documents the two legacy systems that the modern `Ref Lab`
(C#/.NET 8) replaces:

- **V-ROOT-1.00** — the original single-site medical laboratory information system (VB6 + Access Jet 4).
- **V-CD-2019 (v1.605)** — the second generation (VB6 + Access/SQL Server/MySQL, multi-branch, NATIGH/SMS, PDF).

It is a **navigation and curation layer** over the original reverse-engineering evidence. The
original large source documents are preserved as evidence and are **not** replaced.

## 2. How to navigate

```
Docs/KB/START-HERE.md  (modern entry point)
        |
        +-- Modern Ref Lab Knowledge Base (Docs/KB/*)
        |
        +-- Legacy System Knowledge Base (this directory)
                    |
                    +-- INDEX.md  (this file)
                    +-- VERSIONS.md
                    +-- architecture/
                    +-- data/
                    +-- clinical/
                    +-- business/
                    +-- evidence/
                    +-- mapping/
                    +-- MAINTENANCE.md
```

A coding agent should:
1. Read `Docs/KB/START-HERE.md` for the modern system context.
2. Read `Docs/KB/legacy/INDEX.md` (this file) for legacy navigation.
3. Read **`Docs/KB/legacy/SPECIFICATION.md`** for the reference system as a whole.
4. Read only the domain doc(s) matching the task (see §4).
5. Follow cross-references to evidence files only when verifying a claim.

> **★ Canonical rule (2026-11-16).** For every major technical subject there is exactly **one**
> authoritative description. `SPECIFICATION.md` is canonical for the system as a whole;
> `data/DATABASE.md` for the schema; `architecture/EXECUTABLE.md` for the executable;
> `VERSIONS.md` for version identity. Other documents **reference** the canonical one and must not
> restate it.

## 3. Document inventory

| Path | Purpose |
|---|---|
| **`SPECIFICATION.md`** | **★ The canonical reference-system specification — start here** |
| `VERSIONS.md` | Version identity, shared vs specific, deltas, version-evolution model |
| `architecture/OVERVIEW.md` | 5 layers, modules, dependencies, screens, startup, errors |
| `architecture/EXECUTABLE.md` | ★ PE/VB6 structure, the 169→210 object delta, the 1,797 SQL statements, the string pool |
| `architecture/DEPLOYMENT.md` | Engines, installer, licensing, paths, prerequisites, defects |
| `architecture/CONFIGURATION.md` | BASE.ini, SETTING.ini, IDCARD.INI, REPORT.dll, Data\*.ini, NOTE.ini |
| `architecture/PDF-PRINTING.md` | Bullzip path, printer roles, categories, audit, requirements |
| **`data/DATABASE.md`** | **★ The consolidated database reference — schema, constraints, migration engine, table→behaviour link** |
| `data/MODEL.md` | Database inventory, tables, columns, rows, keys, constraints |
| `data/MIGRATION.md` | (see MODEL.md §; dedicated migration doc — TBD content) |
| `data/IDENTIFIERS.md` | `patientid` positional contract, branch, person key, barcode |
| `data/BRANCHES.md` | (branch model — see DEPLOYMENT.md §9 and IDENTIFIERS.md §2) |
| `clinical/RULES.md` | Clinical and laboratory business rules (V1-BR, V2-BR) |
| `clinical/KNOWLEDGE-LAYER.md` | Tests, ranges, constants, units, conversions, abbreviations, sub-groups |
| `clinical/PROFILES.md` | 46 profile tables, template binding, per-template printer/labels |
| `business/WORKFLOWS.md` | Patient lifecycle, result entry, send-out, accounting, ranges, catalogue |
| `business/SECURITY.md` | Credentials, permissions, audit, vulnerabilities |
| `business/CAPABILITIES.md` | 15 V1 + ~60 V2 capabilities, vendor taxonomy, integrations |
| `Docs/KB/legacy/evidence/EVIDENCE-REGISTER.md` | Evidence index (registers, exports, hashes) |
| `Docs/KB/legacy/evidence/SOURCES.md` | ★ Source traceability and disposition register — every source, its canonical path and its disposition |
| `Docs/KB/legacy/evidence/COVERAGE.md` | ★ Knowledge integration and coverage accounting |
| `Docs/KB/legacy/evidence/VALIDATION.md` | ★ Checks performed, their results, and the outstanding issues |
| `Docs/KB/legacy/evidence/CONTRADICTIONS.md` | 22 contradictions with status and reasoning |
| `Docs/KB/legacy/evidence/UNRESOLVED.md` | Open questions, hypotheses, unknowns |
| `mapping/LEGACY-TO-MODERN.md` | Legacy vs modern comparison (4 categories) |
| `mapping/GAP-ANALYSIS.md` | ★ Gap analysis — modern `Ref Lab` against the reconstructed reference system (61 assessed items) |
| `mapping/TRANSFORMATION.md` | ★ Engineering transformation blueprint (plan only — nothing implemented) |
| `MAINTENANCE.md` | Governance, budgets, validation, update rules |

## 4. Task-oriented loading guide

| Task | Load these documents |
|---|---|
| Database schema change | `VERSIONS.md` → `data/MODEL.md` → `data/IDENTIFIERS.md` |
| Clinical calculation | `VERSIONS.md` → `clinical/RULES.md` → `clinical/KNOWLEDGE-LAYER.md` |
| Reference-range logic | `VERSIONS.md` → `clinical/RULES.md` §5 → `clinical/KNOWLEDGE-LAYER.md` §2 |
| PDF / reporting | `architecture/PDF-PRINTING.md` → `architecture/CONFIGURATION.md` → `clinical/PROFILES.md` |
| Authentication / security | `business/SECURITY.md` → `data/IDENTIFIERS.md` → `VERSIONS.md` |
| Deployment | `architecture/DEPLOYMENT.md` → `architecture/CONFIGURATION.md` |
| Configuration | `architecture/CONFIGURATION.md` → `architecture/DEPLOYMENT.md` |
| Workflows | `business/WORKFLOWS.md` → `clinical/RULES.md` → `data/MODEL.md` |
| Capabilities / product scope | `business/CAPABILITIES.md` → `VERSIONS.md` |
| Version comparison | `VERSIONS.md` → `mapping/LEGACY-TO-MODERN.md` |
| Evidence verification | `Docs/KB/legacy/evidence/EVIDENCE-REGISTER.md` → `Docs/KB/legacy/evidence/CONTRADICTIONS.md` → `Docs/KB/legacy/evidence/UNRESOLVED.md` |
| Resolving a contradiction | `Docs/KB/legacy/evidence/CONTRADICTIONS.md` → relevant domain doc → source evidence |
| Investigating an open question | `Docs/KB/legacy/evidence/UNRESOLVED.md` → relevant domain doc → source evidence |
| Modern-system change touching legacy concepts | `mapping/LEGACY-TO-MODERN.md` → relevant domain doc → `Docs/KB/*` |

## 5. Version awareness

Every fact in this knowledge base carries a version label:

- **V1 ONLY** — established for V-ROOT-1.00 only.
- **V2 ONLY** — established for V-CD-2019 only.
- **SHARED** — established for both versions.
- **VERSION-DEPENDENT** — applies to both but behaves differently.
- **UNKNOWN** — not established for either version.

See `VERSIONS.md` for the full version identity and delta.

## 6. Evidence and traceability

- Every non-trivial claim cites its source path and evidence type code.
- Evidence types: ET-DOC, ET-DOC+DB, ET-DB, ET-CODE, ET-DDL, ET-CFG, ET-INST, ET-DERIVED, ET-PRES.
- Confidence levels: CONFIRMED · STRONGLY SUPPORTED · INFERRED · UNKNOWN · CONFLICTING.
- Original source documents are preserved (not replaced). See `Docs/KB/legacy/evidence/EVIDENCE-REGISTER.md`.
- **V1 consolidated documents are accessible** (resolved 2026-11-16) — they are present in the
  reverse-engineering corpus and are already integrated. See `Docs/KB/legacy/evidence/SOURCES.md` §3 and
  `Docs/KB/legacy/evidence/UNRESOLVED.md` §6. **Do not** label V1 references `EXTERNAL SOURCE REQUIRED`.

## 7. Contradictions and unresolved questions

- 22 contradictions registered in `Docs/KB/legacy/evidence/CONTRADICTIONS.md` (19 RESOLVED, 3 OPEN).
- 73 unresolved-question rows registered in `Docs/KB/legacy/evidence/UNRESOLVED.md`
  (15 RESOLVED, 58 not fully resolved).
- Do not silently resolve — update the register status with reasoning.

## 8. Original source evidence (preserved)

These large source documents are retained as evidence and are **not** replaced by this knowledge base:

- `Database documentation.md` (root, 209,465 B) — full DB reference.
- `Documenting the Executable File.md` (root, 172,287 B) — full PE/VB6 RE.
- `Docs/Complete_Reverse_Engineering_Reference_V2.md` (64,412 B) — V2 consolidated.
- `Docs/Business_Rules_V2.md` (76,641 B) — V2 business rules.
- `Docs/Data_Model_and_Workflow_Entities_V2.md` (40,598 B) — V2 entities.
- `Docs/Functional_Specification_V2.md` (65,753 B) — V2 functional spec.
- `evidence/reverse-engineering/*` — registers, scripts, exports, phase reports.

## 9. Maintenance

See `MAINTENANCE.md` for update rules, budgets, governance integration, and validation commands.

## 10. Related documents

- `Docs/KB/START-HERE.md` — modern system entry point.
- `Docs/KB/legacy/MAINTENANCE.md` (modern) — modern KB conventions.
- `Docs/KB/ARCHITECTURE.md`, `DATA-MODEL.md`, `DOMAIN-RULES.md`, `OPERATIONS.md`, `CONVENTIONS.md`.
- `AGENTS.md` — agent rules.
- `scripts/kb_build.py`, `scripts/kb_check.py` — governance tooling.
- `Docs/KB/legacy/MAINTENANCE.md` — legacy maintenance guide.
- `Docs/KB/legacy/mapping/LEGACY-TO-MODERN.md` — legacy vs modern.


## 9. Evidence Batch 02 (reverse engineering)

Reverse-engineering knowledge base for the reference system (V-ROOT-1.00 + V-CD-2019): `Docs/KB/legacy/evidence-batch-02/D12_Executive_Summary/Executive_Summary.md`.

## 10. Reverse Engineering Output (MASTER DIRECTIVE)

Full-spectrum RE output for RealLab 1.605: `Docs/KB/legacy/reverse-engineering-output/10_UNIFIED_KNOWLEDGE_BASE/MASTER_SYSTEM_KNOWLEDGE.md`.


## 11. Reference System Source Corpus (Agent B)

Master engineering reference: `Docs/KB/legacy/reference-system-corpus/master-engineering-reference.md` (36 detailed reports).
