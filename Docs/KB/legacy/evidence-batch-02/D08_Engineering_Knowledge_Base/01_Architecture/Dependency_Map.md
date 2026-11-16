# Dependency Map — RealLab v1.605

## Runtime dependencies

- `MSVBVM60.DLL` (build 9782), `VBA6.DLL` — out of support.
- `MSADO27.TLB` / ADO 2.7; `SQLOLEDB.1` provider; Jet 4 (Access).
- `.NET Framework 4.0` for the e-mail component; `System.Net.Mail`, `System.Data.SqlClient`.

## Database dependencies

- SQL Server: catalog `Patients`; servers `REAL_LAB`, `DESKELTEL`, `REALLAB_AIS`.
- Access Jet 4: `Patients.mdb`, `Patients_backup.mdb`, `Branch_Data.mdb`, `data.mdb`.
- Possible third backend: MySQL (connector referenced) — Class 3 hypothesis (FIND-T3-011).

## External services

- SMTP relay (outbound lab-result notification).
- `smsmisr.com` SMS gateway (cleartext HTTP, credentials in query string) — U-016.
- Serial-port analysers (MSCOMM32, `AUTOMATIC HEMATOLOGY ANALYZER … BF6500`).
- Internet Explorer COM automation for outbound HTTP (FND-F-006).
- Facebook pages (vendor presence), launched in Chrome.

## Deployment dependencies

- VS6 Setup Wizard; `Setup*.bat` (absolute paths, `D:\Lab_data` attach).
- Bullzip PDF Printer (evaluation mode, expired cert — U-033).
- Avira AV engine (orphaned descriptors — U-015).
- unrar 3.50 (U-019).
- Unsigned local copies of Windows system DLLs (U-020).