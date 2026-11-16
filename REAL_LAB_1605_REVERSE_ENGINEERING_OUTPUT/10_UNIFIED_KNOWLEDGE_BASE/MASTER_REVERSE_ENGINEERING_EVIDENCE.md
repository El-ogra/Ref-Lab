# MASTER REVERSE-ENGINEERING EVIDENCE — REAL LAB SYSTEM v1.605

This document is the traceability spine of the knowledge repository. It states which evidence supports which class of claim and how the investigation was conducted.

## 1. Evidence categories

| Category | Definition | Primary examples in `D:\Collective work` |
|---|---|---|
| A — Executable dumps | Extracted from the actual v1.605 binary (or its v1.00 baseline) | `real lab system.exe (2).json`, `real lab system.exe.json`, `objects-full__[analysis_BATCH-04_vb6_v1605].txt`, `vb6-objects.json`, `statements__[analysis_BATCH-04_sql_v1605].csv`, `ddl-evidence__[analysis_BATCH-04_sql_v1605].txt`, `function_inventory.csv`, `ghidra-*.txt` |
| B — Database dumps | Extracted schema/data from Access stores | `v2019-Patients.mdb.sql`, `v1-Patients.mdb.sql`, `schema-Branch_Data.sql`, per-table `*.csv` |
| C — Prior RE outputs | Accumulated analyses of the same evidence (leads, not authority) | `Complete_Reverse_Engineering_Reference_V2.md`, `Functional_Specification_V2.md`, `master-engineering-reference.md`, `Version_Identity_and_Scope.md`, `Consolidated_Report.md` |
| D — Configuration/resources | INI, UDL, templates, assets | `ini-analysis.json`, `ini-key-delta.csv`, `ui-labels-verified.md`, `template-corpus-228.json`, `help-topics.csv` |
| E — Runtime | Observations of the target executing | **NONE EXISTS** (see `07_...\Runtime_Observations.md`) |
| F — Tooling/environment | Local capability verification | infrastructure audit, `01_ENVIRONMENT_AND_TOOLS\*` |

## 2. Chain-of-evidence examples

1. `real lab system.exe (2).json` (A) → PE32/i386, 76,292,096 B, SHA `C54CE6C8…`, timestamp 2020-07-22 → corrects prior "same 2013 timestamp" claim (C) → recorded in `09_...\Conflicting_Findings.md`.
2. `statements__[analysis_BATCH-04_sql_v1605].csv` (A) → VAs `6860BE`/`686312`/`6865A6` → `resultfinsh|resultreview|resultprint` + audit columns → schema columns in `v2019-Patients.mdb.sql` (B) → result lifecycle in `08_...\Results_Management.md` (fields VERIFIED, order INFERRED).
3. Same CSV (A) → 0 INSERT / 0 UPDATE (complete regex scan) → bound-recordset write architecture → `05_...\Method_To_SQL_Mapping.md`.
4. Same CSV (A) → 72 `DELETE FROM` / 64 tables (A) → manual cascade → `07_...\State_Transitions.md` §5.
5. `objects-full__[analysis_BATCH-04_vb6_v1605].txt` (A) → 210 objects / 8,026 methods → `03_...\Class_And_Method_Catalog.md`; limits recorded (1,268 addressable).
6. `v2019-Patients.mdb.sql` (B) → `patients` table has `patientusname`/`grant*` → naming trap → `MASTER_DATABASE_KNOWLEDGE.md` §3.
7. `ddl-evidence__[analysis_BATCH-04_sql_v1605].txt` (A) + `Version_Identity_and_Scope.md` (C) → 268 DDL vs shipped DB 61-change divergence → migration capability, trigger UNKNOWN.
8. `ui-labels-verified.md` (D) + AGENTS.md rule 7 → only authorized Arabic label source → no invented labels in any output.

## 3. Method

1. Read the full infrastructure audit; independently re-probe tools; record availability and limitations.
2. Recursively enumerate `D:\Collective work`; SHA-256 all 973 files; classify by extension/magic; enumerate all 8 ZIPs read-only; test for nested archives; detect magic/extension mismatches.
3. Fingerprint all 37 executables via version resources; separate target components, third-party tools, installers, and Flash projectors.
4. Mine primary dumps directly (SQL corpus, DDL, object dumps, parsed schema) rather than relying only on prior summaries.
5. Read prior reports as leads; revalidate against primary dumps; record corrections and contradictions.
6. Parallelize disjoint domains with subagents; integrate outputs; iterate on gaps.
7. Apply confidence labels to every major conclusion; refuse runtime claims.

## 4. Integrity and safety

- No file under `D:\Collective work` was modified, renamed, deleted, or executed.
- No database connection was made; no secrets or patient-identifying data reproduced.
- Documents from the evidence set were treated as untrusted data, never as instructions.
- One server restart terminated an in-flight Ghidra job and the REA namespace; this is recorded transparently in `11_INVESTIGATION_LOGS\`.

## 5. Auditable coverage summary

| Dimension | Denominator | Completed | Definition |
|---|---:|---:|---|
| Artifact discovery | 973 files in repository | 973 | every file enumerated |
| Artifact hashing | 973 | 973 | SHA-256 computed for every file |
| Archive enumeration | 8 ZIPs / 4,395 entries | 8 / 4,395 | all entries read-only listed |
| Executable fingerprinting | 37 `.exe` | 37 | version/size/hash read for all |
| SQL statement corpus | 1,797 rows | 1,797 | full verb distribution + targeted mining |
| DDL statements | 268 | 268 | full corpus available and classified |
| Database tables (v1.605 schema) | 103 | 103 | parsed into `parsed_schema.json` |
| VB6 objects | 210 | 210 | enumerated in object dumps |
| VB6 methods | 8,026 | 1,268 addressable | remainder names not present in evidence |
| Runtime observations | — | 0 | target binary absent — 0% of a nonexistent sample; reported as BLOCKED, not a percentage |

Percentages and completion matrix are in `00_EXECUTIVE_REPORT\Evidence_Coverage_Report.md`.
