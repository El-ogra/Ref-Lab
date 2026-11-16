# System Architecture — RealLab v1.605

## Pattern

Classic **two-tier desktop LIS**: a rich VB6 Windows client talking to SQL Server, with a local Access (Jet) mirror for branch/offline work. No service-oriented layer is evident; the client embeds ADO data access, SHAPE queries, and a runtime DDL migration engine.

## Layers

1. **Presentation (VB6):** 132 forms + 66 DataReport designers; per-discipline `Result*`/`Report*` naming. Arabic RTL. Barcode-capable.
2. **Business logic (VB6 modules):** `labmain` (71 methods), `patientmain` (84), `labdb`/`labdb1`, `labfunction`, `testmain`, `addpatient`, `barcode`, `phone`, `moneypaid`, `testrange`, `workpaper`, etc. 8,026 methods total; procedure names are **not stored** in the binary (F-07).
3. **Data access:** ADO 2.7 / `SQLOLEDB.1` against SQL Server; Jet 4 for the mirror; SHAPE hierarchies for reports; runtime DDL at startup.
4. **Reporting:** 66 DataReports + 228 RTF templates + 146 per-test INI configs + barcode fonts.
5. **Configuration:** `BASE.ini`, `SETING.ini`, `IDCARD.INI`, `NOTE.ini`, `data_connection*.udl`, `Data\*.ini`.
6. **Deployment:** Setup scripts, VS6 Setup Wizard, Bullzip PDF printer, Avira AV, unrar 3.50.

## Runtime & security posture

- VB6 runtime (build 9782, out of support), no ASLR, no DEP/NX, unsigned binaries.
- Self-registering COM controls; operator default `admin`/`admin`; cleartext operator passwords; coarse grant1..grant12 authorisation.

## Diagram

See `Architecture_Diagrams/architecture.mmd` (also in `04_Cross_Evidence_Integration/`).

## Evidence

FIND-T1-001..004, FIND-T1-005, FIND-T1-011, FIND-T2-002, FIND-T3-002.