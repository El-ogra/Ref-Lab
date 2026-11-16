# Business Rules and Functional Workflows — RealLab v1.605

## Business rules (evidence-supported)

- **Catalogue & grouping:** 609 tests in 48 subgroups; hierarchical group → subgroup → test.
- **Reference-range validation:** 852 ranges, sex/age stratified (M 422 / F 421); age units Years/Days/Mons.
- **Derived values:** PT/INR (`PTCONS`) and CBC (`CBCCONS`) constants shipped as user-editable values (U-035).
- **Pricing:** patient/lab/other-lab prices per test; send-out to other labs.
- **Authorisation:** per-patient `grant1..grant12` flags + per-section `IDPassword*` print passwords (coarse, undocumented) — FIND-T3-013.
- **Multi-branch:** `Branch_Data.mdb` template + `branch` column on testtable.
- **Microbiology:** cultures, cultureanti, culturesec tables; CULTURE AND SENSITIVITY subgroup; molecular biology subgroup.
- **QC:** `Reportqc` DataReport; QC workflow.
- **Delete protection:** 605 of 609 tests have `deleteprotect` set.
- **Active flag:** 94 of 609 tests active.
- **Send-out:** 1 test sent to another lab.

## Functional workflows (inferred from forms/reports)

- **Result entry:** per-discipline `Result*` form → profile table write → validation vs normalrange.
- **Reporting:** per-discipline `Report*` DataReport → RTF template + INI config → barcode print.
- **Allergen panel:** Arabic labels stored in Windows-1256 (U-027).
- **Patient notification:** SMS (smsmisr.com) and e-mail (SMTP).
- **Backup:** generated `.bat`/`.sql` to configurable server/path.
- **Schema migration:** DDL at startup (U-038).

## Rules location (per project convention)

Per the RefLab project's AGENTS.md, **business rules belong in `Domain/Rules/` only**. This KB documents the *legacy* RealLab rules; they are not duplicated into the new RefLab Domain layer. Translating any of these rules into the new system requires explicit owner approval and a migration.