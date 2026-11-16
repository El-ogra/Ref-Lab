# Stage 2 — Evidence Type 2 Analysis: Database & Data Evidence

**Product:** RealLab v1.605 ("RealLab CD 2019")
**Evidence type:** 2 of 3 — Access/Jet database schema, SQL statements, row counts, data samples
**Files:** 180 · **Bytes:** 1,014,215
**Method:** static parsing of prior schema/SQL exports and redacted samples; no live database opened. Per-table statistics in `type2_tables.csv`; machine-readable findings in `Stage2_Technical_Findings.json`.

---

## 1. The four local databases (FIND-T2-001)

| Database | Jet version | Tables | Columns | Rows | Role |
|---|---|---:|---:|---:|---|
| `Patients.mdb` | Jet 4 (Access 2000) | 93 | 1527 | 3123 | primary patient + test store |
| `Patients_backup.mdb` | Jet 4 | 90 | 1381 | 2786 | backup of patient store |
| `Branch_Data.mdb` | Jet 4 | 49 | 969 | 0 | branch / multi-branch configuration |
| `data.mdb` | Jet 4 | 3 | 34 | 0 | test catalogue, ranges, constants, prices |

These Access MDBs are the **offline/branch mirror** (Type 1 FIND-T1-005: SQL Server is the primary system of record).

## 2. Schema shape (FIND-T2-002, FIND-T2-004)

`cumulative-schema.csv` covers 133 tables by role:

- **96 per-test result (profile) tables** — one per test profile (ABO, BILH, BloodGases, CRP, CSFT, CoagFactors, cbc, liver, kidney, lipids, …). Each carries `patientid` + `requestdate` + analyte columns + `comment`.
- **33 configuration / master-data tables** — `testtable`, `grouptable`, `subgrouptable`, `normalrange`, `constanttable`, `pricelist`, `testunits`, `testname`, `instrument`, `barcodes`, `phone`, `sections`, …
- **1 generic 20-column dynamic history table**, **1 patient master**, **1 test-order/result index**, **1 work-analysis list with prices**.

**Master core:** `patientinfo` (demographics, 33 cols), `patienttest` (orders/results, 44 cols), `testtable` (catalogue, 37 cols), `normalrange` (18 cols). SQL-reference ranking: `testtable` 352, `patientinfo` 307, `patienttest` 216, `normalrange` 52.

## 3. Naming trap — `patients` is the operator table (FIND-T2-003)

The Access table named **`patients`** is **not** patient demographics. Its columns are `patientusid, patientusname, lastvisit, patientuscap, patientword, patientad, patientus, grant1..grant12, showtip, tipnum, startwork, endwork, breakyorn, breaktime, khasmlimit`. The SQL is `select … from patients where patientusname = …`. This is the **operator login + 12 permission-grant slots + shift schedule** table. Real patient demographics are in `patientinfo`. This corroborates Type 1 security finding U-001 (default `admin`/`admin` operator account).

## 4. SQL / data-access pattern (FIND-T2-005)

`statements.csv` → **1,337 statements**: SELECT 1220, DELETE FROM 72, SHAPE 20, OTHER 25. The SHAPE statements build hierarchical parent-child result sets for reports (`SHAPE {SELECT … FROM subgrouptable …} APPEND ({SELECT * FROM testtable …} RELATE subgroupcode TO subgroupcode)`), i.e. VB6 ADO / MSDataShape feeding DataReports. This is a **client-side, SQL-string-built** data-access style with no parameterisation visible in the captured statements.

## 5. Catalogue and reference data (FIND-T2-006, FIND-T2-007)

- **testtable:** 609 tests, 48 subgroups, 591 priced, 605 delete-protected, 94 active. Subgroups: BLOOD PICTURE, SERODIAGNOSIS, Trace Elements, Autoimmune diseases, KIDNEY FUNCTIONS, MOLECULAR BIOLOGY, CULTURE AND SENSITIVITY, Hepatitis Markers, LIVER FUNCTION, etc.
- **normalrange:** 852 rows / 304 tests; sex-stratified (M 422 / F 421); age units Years 708 / Days 70 / Months 65; numeric low/high limits for 459/643 rows.

These disciplines map directly to the Type 1 per-domain `Result*`/`Report*` forms — a cross-type corroboration to be formalised in integration.

## 6. Schema evolution (FIND-T2-008)

`column-evolution.csv` (103 rows) compares V1 Jet schema vs V2019 DDL, listing added/removed columns per table; `created-tables.csv` / `added-columns.csv` record concrete DDL deltas (new tables `FMF`, `Helicobacter`; new columns `Abuse.tramadol/ecstasy`, `BloodGases.pO2AA1/ctCO2`, …). This documents the schema delta between the two shipped versions.

## 7. Privacy and data quality (FIND-T2-010)

Patient-bearing samples (`patients`, `patientinfo`, `patienttest`, `testtable`, `pricelist`, `grouptable`, …) are **redacted** (`<redacted>`); reference/config data (`normalrange`, `constanttable`) is not. Several sample exports have partially redacted **column headers** (`patientinfo`, `patienttest`); full column lists are reconstructed from `columns__[...].csv` / `columns.csv`.

## 8. Contradiction (CON-001)

**Jet 3 vs Jet 4.** Type 1 (`dependency-inventory.json`) says "Jet 3 / Access 97"; Type 2 (`db-summary.csv`, read from the MDB header) says "Jet 4 (Access 2000)". Resolution: **Jet 4**, because the MDB header is authoritative for the files present. Both are recorded; neither is silently discarded.

## 9. Findings

| ID | Finding | Class | Confidence |
|---|---|---|---|
| FIND-T2-001 | Four Jet 4 MDBs (local mirror) | 1 | High |
| FIND-T2-002 | Master core + 96 profile result tables | 1 | High |
| FIND-T2-003 | `patients` = operator accounts (naming trap) | 1 | High |
| FIND-T2-004 | patientinfo / patienttest / testtable / normalrange core | 1 | High |
| FIND-T2-005 | 1,337 ADO statements, SHAPE + SELECT + DELETE | 1 | High |
| FIND-T2-006 | 609-test catalogue, 48 subgroups | 1 | High |
| FIND-T2-007 | 852 reference ranges, sex/age stratified | 1 | High |
| FIND-T2-008 | 103 schema-evolution deltas V1→V2019 | 1 | High |
| FIND-T2-009 | PTCONS/CBCCONS derivation constants | 2 | Medium |
| FIND-T2-010 | PII redacted; reference data not | 1 | High |
| FIND-T2-011 | Branch_Data.mdb = 0-row template | 2 | Medium |
| FIND-T2-012 | data.mdb = 3 master tables | 2 | Medium |