# Database Architecture — RealLab v1.605

## Backends

Two backends with distinct roles (FIND-T1-005, FIND-T2-001):

1. **SQL Server** — system of record. Catalog `Patients`; servers `REAL_LAB` / `DESKELTEL` / `REALLAB_AIS`. Reached via ADO 2.7 + `SQLOLEDB.1`.
2. **Access Jet 4 (MDB)** — offline/branch mirror: `Patients.mdb` (93 tables, 1527 cols, 3123 rows), `Patients_backup.mdb` (90, 1381, 2786), `Branch_Data.mdb` (49, 969, 0), `data.mdb` (3, 34, 0).

> Note (CON-001): the MDBs are **Jet 4 (Access 2000)**, not Jet 3/Access 97 as an earlier Type 1 statement suggested. The MDB header is authoritative.

## Schema shape

`cumulative-schema.csv` covers 133 tables:

- **96 per-test profile result tables** (ABO, BILH, BloodGases, CRP, CSFT, CoagFactors, cbc, liver, kidney, lipids, …) — each `patientid` + `requestdate` + analytes + `comment`.
- **33 configuration/master-data tables** — `patientinfo`, `patienttest`, `testtable`, `grouptable`, `subgrouptable`, `normalrange`, `constanttable`, `pricelist`, `testunits`, `testname`, `instrument`, `barcodes`, `phone`, `sections`, …
- 1 generic 20-column dynamic history table, 1 patient master, 1 test-order/result index, 1 work-analysis list with prices.

## Naming trap (critical)

The table named **`patients`** is the **operator/user account** table (`patientusname` + `grant1..grant12` + shift schedule) — not patient demographics. Demographics live in `patientinfo`; orders in `patienttest` (FIND-T2-003).

## Catalogue & reference data

- **testtable:** 609 tests, 48 subgroups, 591 priced, 605 delete-protected, 94 active.
- **normalrange:** 852 rows / 304 tests; sex-stratified (M 422 / F 421); age units Years 708 / Days 70 / Mons 65.
- **constanttable:** `PTCONS` (PT/INR) and `CBCCONS` (CBC) derivation constants.
- **pricelist:** per-test pricing (patient/lab/other-lab).

## Schema evolution (V1 → V2019)

`column-evolution.csv` (103 rows) compares Jet vs DDL schemas; `created-tables.csv`/`added-columns.csv` record concrete deltas (new tables `FMF`, `Helicobacter`; new columns `Abuse.tramadol/ecstasy`, `BloodGases.pO2AA1/ctCO2`).

## Data privacy

Patient-bearing samples are redacted (`<redacted>`); reference/config data is not. Several sample exports have partially redacted column headers; full column lists reconstructed from `columns__[...].csv` / `columns.csv`.