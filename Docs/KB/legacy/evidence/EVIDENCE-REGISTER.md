# Legacy Evidence Register — Index

**Purpose:** Navigable index of evidence artefacts supporting legacy-system findings.
**Scope:** Evidence items, SHA-256, paths, version applicability, domain, status.
**Audience:** Auditors, developers, analysts.
**Version applicability:** Mixed (noted per entry).

---

## 1. Source evidence files (preserved, not replaced)

| Artefact | Location | Type | Version | SHA-256 (from `FINAL_EXECUTION_REPORT_AR.md` / `Database documentation.md`) |
|---|---|---|---|---|
| `Patients.mdb` | `F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\bin\Patients.mdb` (external, read-only) | Access Jet 4 database | V2 | `98D51F5294496E03F352FF329F099572915CCB2E670C1F143D6BA368EEE898A3` (2,682,880 B) |
| `real lab system.exe` | `F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe` (external, read-only) | PE32 VB6 executable | V2 | `C54CE6C8C98A679EFC5AB485BC4B313B452026D6A40F9BDA36ECA7EA18DF93C0` (76,292,096 B) |
| `RefLab_Reference_V2_Consolidated.zip` | repo root | 12-member cumulative knowledge base | V1+V2 | `603823F0D9ED3E7A3D60847A74A3C5D6443F16A23E5CBCBE085624A9A905C028` (477,540 B) |
| `Database documentation.md` | repo root | Deliverable 1 (209,465 B) | V2 | `EDE0F159BE67C99A7F76B768…` |
| `Documenting the Executable File.md` | repo root | Deliverable 2 (172,287 B) | V2 | `A5903D938BBA0D3F80F919A9…` |

## 2. Evidence registers (preserved, referenced)

| Register | Location | Entries | Status |
|---|---|---:|---|
| `reverse-engineering-evidence-register.csv` | `evidence/reverse-engineering/` | 36 | PRESERVED (REFERENCE) |
| `reverse-engineering-file-accountability.csv` | `evidence/reverse-engineering/` | 28 | PRESERVED (REFERENCE) |
| `export_ledger.csv` | `evidence/reverse-engineering/metadata/` | 103 tables | PRESERVED (REFERENCE) |
| `pseudonym_map.txt` | `evidence/reverse-engineering/metadata/` | 18 pseudonyms | PRESERVED (REFERENCE, restricted) |

## 3. Companion exports

- `evidence/reverse-engineering/exports/tables/*.json` — 103 JSON files (one per table), 3,809 records,
  redacted per policy v3 (credentials redacted by column name, phones masked, 13-digit identifiers
  pseudonymised by value shape).
- `evidence/reverse-engineering/exe/*` — PE/VB6 analysis artefacts: `exe_findings.md` (42 KB),
  `README.md`, `tool_log.md`, `exe_inventory.json`, `strings_classified.json` (101,812 strings),
  `resources.json`, plus Ghidra outputs (`*.functions.txt`, `*.xrefs.txt`, `*.strings.txt`,
  `*.program.json`, `*.ghidra.log`).
- `evidence/reverse-engineering/phase0/*` — `PHASE-0-SPEC.md`, `PHASE-0-REPORT.md`,
  `PHASE-0-SPEC-ADDENDUM-01.md`.
- `evidence/reverse-engineering/scripts/*` — 5 Python scripts (redaction, verification).

## 4. Evidence type codes

ET-DOC, ET-DOC+DB, ET-DB, ET-CODE, ET-DDL, ET-CFG, ET-INST, ET-DERIVED, ET-PRES.
(See `Docs/KB/legacy/VERSIONS.md` §8 for definitions.)

## 5. Confidence levels

CONFIRMED · STRONGLY SUPPORTED · INFERRED · UNKNOWN · CONFLICTING.

## 6. Traceability standard

Every important technical claim in the curated knowledge base must cite:
- Repository-relative path for internal sources.
- Explicitly labelled `EXTERNAL` for sources outside the repository.
- Evidence type code and confidence level.
- The original source's verification status (do not upgrade an interpretation to a verified fact).

## 7. External V1 material

See `UNRESOLVED.md` section 6 for the V1 source accessibility limitation. V1 consolidated documents
are not in this repository and are labelled `EXTERNAL SOURCE REQUIRED`.

## 8. Related documents

- `Docs/KB/legacy/evidence/CONTRADICTIONS.md` — contradiction register.
- `Docs/KB/legacy/evidence/UNRESOLVED.md` — unresolved questions register.
- `evidence/reverse-engineering/*` — original evidence artefacts (preserved).
- `Docs/KB/legacy/mapping/LEGACY-TO-MODERN.md` — legacy vs modern.
- Original detailed sources: `Database documentation.md`, `Documenting the Executable File.md` (root).
