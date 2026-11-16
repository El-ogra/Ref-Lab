# Data Flow Architecture — RealLab v1.605

## End-to-end flow

1. **Login:** operator authenticates (default `admin`/`admin`, grant1..grant12 authorisation).
2. **Registration:** patient demographics captured into `patientinfo` (patientid, name, sex, age, labid, requestdate, …).
3. **Ordering:** tests selected from `testtable` → `patienttest` (patientid, testcode, testsimp, requestdate).
4. **Result entry:** per-discipline `Result*` form writes analyte columns into the matching **profile table** (ABO, cbc, liver, …) keyed by patientid+requestdate.
5. **Validation:** entered values compared against `normalrange` (sex/age stratified).
6. **Derivation:** PT/INR and CBC derived via `constanttable` (PTCONS, CBCCONS).
7. **Pricing:** `testtable.patientprice/labprice/otherlabprice` + `pricelist`.
8. **Reporting:** `Report*` DataReport renders via RTF template + per-test INI config + barcode fonts; `barcode1-3` printed.
9. **Notification:** SMS (smsmisr.com) and/or e-mail (SMTP via .NET component).
10. **Send-out:** tests with `inotherlab=True` routed to external lab (`otherlabprice`).
11. **Sync:** local Access mirror (Jet) for branch/offline.
12. **Backup:** `.bat`/`.sql` generated backups to configurable server/path.
13. **Startup:** schema-migration engine runs DDL against the clinical DB.

## Data stores touched

SQL Server (`Patients`) ↔ Access mirror (`Patients.mdb`, `Patients_backup.mdb`, `Branch_Data.mdb`, `data.mdb`).

## Diagram

See `04_Cross_Evidence_Integration/Cross_Evidence_Integration.md` §5 for the sequence diagram.