# MASTER DATABASE KNOWLEDGE — REAL LAB SYSTEM v1.605

## 1. Stores

| Store | Tables | Columns | Rows | Role | Evidence |
|---|---:|---:|---:|---|---|
| `Patients.mdb` (v1.605 shipped) | 103 | 1,682 | — | Primary Access database | `v2019-Patients.mdb.sql` (`04_...\_work\parsed_schema.json`) |
| `Patients.mdb` (v1.00) | 93 | 1,527 | 3,123 | Primary DB, earlier build | `v1-Patients.mdb.sql`, `Version_Identity_and_Scope.md` §2.3 |
| `Patients_backup.mdb` | 90 | 1,381 | 2,786 | Backup (version attribution UNKNOWN) | `schema__[analysis_BATCH-02_db_Patients_backup].sql` |
| `Branch_Data.mdb` | 49 | 969 | 0 | Schema-only branch seeding template | `schema-Branch_Data.sql` |
| `data.mdb` | 3 | 34 | 0 | `history` (dynamic 20-col), `patientinfo`, `wasl` | `schema__[analysis_BATCH-02_db_data].sql` |
| SQL Server catalogue `patients` | — | — | — | Central store option | prior docs; `sp_addumpdevice` SQL |
| SQL Server catalogue `Interface` | — | — | — | Attached by `Attach_Interface.bat` | prior docs |
| `VIDAS` | — | — | — | Referenced, structurally UNKNOWN | prior docs |

**No original `.mdb` file is present locally** — all schema facts come from extracted DDL/CSV dumps.

## 2. Schema families

- **Per-test profile result tables (~96):** `ABO`, `Abuse`, `anyreport`, `BILH`, `BloodGases`, `BONEMAR`, `cardiac`, `CBC`, `CoagFactors`, `CRP`, `CSFT`, `cultureanti`, `cultures`, `Drugs`, `Electrolytes`, `ESR`, `Fertlity`, `GLUCOSETOLERANCE`, `hepatitis`, `Hgbectrophoresis`, `hormones`, `Immunoglobulin`, `IMMUNOLOGY`, `iron`, `kidney`, `lipids`, `liver`, `Pancreas`, `postcoitalt`, `pregnancy`, `Proelectrophoresis`, `prostaticx`, `pt`, `renalfailure`, `samples`, `seana`, `semen`, `sharekat`, `stone`, `stool`, `Sugar`, `Suprarenal`, `tasgel`, `testname`, `testtemp`, `TORCH`, `trace`, `TUBERCULINT`, `Tumors`, `Urine`, `Vitamins`, `wasl`, `widal`, `ZN`, … Typically keyed by `patientid` + `requestdate`, with analyte columns and a comment column (prior work; VERIFIED for key columns where DDL shows them).
- **Core workflow tables (v1.605 column counts from `v2019-Patients.mdb.sql` / `parsed_schema.json`):** `patientinfo` (**39** cols: demographics, 13-char `patientid`, price/paid/balance, `completes`), `patienttest` (**46** cols: `testcode`, `testsimp`, `resultfinsh`, `resultreview`, `resultprint`, `printwith`, audit columns), `payment`, `history`, `comments`, `samples`, `Labbranch`.
- **Catalogue/knowledge tables:** `testtable` (609 tests; `testorno`, `testsimp`, patient/lab/other-lab prices, sample type), `normalrange` (852 rows / 304 tests; `sex`, `firstold`, `secondold`, `ageunit`, `normalrang`), `pricelist`, `testunits` (69), `converts` (167), `abbreviations` (650), `subgrouptable` (46), `grouptable`, `customtesttable`, `constanttable` (PTCONS/CBCCONS), `instrument`, `barcodes`, `sections`, `phone`, `daytip`.
- **Operator/security tables:** `patients` — **operator accounts**, `patientusname` + `grant1..grant12` + shift schedule; `loger` — logging (`SELECT * FROM loger where lognum=` at VA `66F746`); `SMSSender`.
- **Configuration:** `REPORT.dll`, `printersettings`, `reportview` (VA `66F882`: strip settings where `stripdiscrp='MAIN'`).

## 3. Critical naming trap

`patients` is NOT patient demographics. Demographics = `patientinfo`; orders = `patienttest`. Any reconstruction that joins patient identity to `patients` is wrong. [VERIFIED from DDL columns `patientusname`/`grant*`]

## 4. Relationships

- Explicit joins in SQL: `patientinfo INNER JOIN patienttest ON patienttest.patientid = patientinfo.patientid` (VA `6843F2`, `68463A`).
- Hierarchical `SHAPE … APPEND … RELATE`: `subgrouptable.subgroupcode → testtable.subgroupcode` (VA `672722`); `normalrange.testcode → testtable.testcode` (VA `672E3E`).
- Naming-based (INFERRED): per-test tables keyed by `patientid` (+`requestdate`) relate to `patienttest` and `patientinfo`; `pricelist.testcode → testtable.testcode`; `payment.patientid → patientinfo.patientid`.
- No declared foreign keys were recovered from DDL; deletion is implemented in the application (§6).

## 5. Identifier scheme

13-character `patientid`. SQL repeatedly computes `SUBSTRING(patientid,10,3) AS patid` / `Mid(patientid,10,3)` (VAs `682F56`, `68313E`, `683316`, `683C72`, …). **Position 9 is the branch code in the legacy system:** 18 v1.605 statements compute `SUBSTRING(patientid,2,1) + SUBSTRING(patientid,9,1) AS bran` (e.g. VA `72714C`), and prior reports name the branch source as `patients.branche`. [VERIFIED from SQL] The C# successor's rule (AGENTS.md rule 8: position 9 folded into the middle segment) describes the **successor design**, not the legacy target; do not apply it to v1.605.

## 6. Write and delete behavior

- 0 `INSERT INTO`, 0 `UPDATE … SET` in 1,797 statements → writes via bound recordsets (ADO/DAO `AddNew`/`Update`).
- 72 `DELETE FROM` statements across **64 distinct tables**, many `WHERE patientid='…'` (contiguous block VAs `6A740A`–`6A760E` deleting `patientinfo`, `payment`, `patienttest`, `ABO`, …, `widal`, `ZN`) → application-level manual cascade, transaction semantics UNKNOWN.
- 268 embedded DDL (27 CREATE + 241 ALTER) for migration; shipped DB out of step (23 SQL-Server-only + 38 Access-applicable changes not applied).

## 7. Data-level knowledge (anonymized)

- `testtable`: 609 tests, 48 subgroups, 591 priced, 605 delete-protected, 94 active (prior work, STRONGLY_SUPPORTED).
- `normalrange`: 852 rows, 304 tests; sex M 422 / F 421; age units Years 708 / Days 70 / Mons 65 (prior work). `pregsmok` is populated on all rows but its calculation use is UNVERIFIED.
- `constanttable`: PTCONS (PT/INR constants 12, 35, 1.33, …), CBCCONS (CBC 3.3, 6.25, 6.75, 7.5, 8.25) — prior work; exact formulas UNKNOWN.
- No patient-identifying values are reproduced anywhere in this repository.

## 8. Confidence summary

| Area | Confidence |
|---|---|
| Table/column counts and names | VERIFIED (DDL dumps) |
| Explicit joins / SHAPE RELATE | VERIFIED (SQL literals) |
| Naming-based relationships | INFERRED |
| Result flag semantics | STRONGLY_SUPPORTED (fields) / INFERRED (order) |
| Financial formula/rounding | UNKNOWN |
| VIDAS structure | UNKNOWN |
| Live data pages / query plans | NOT ASSESSABLE (no .mdb) |
