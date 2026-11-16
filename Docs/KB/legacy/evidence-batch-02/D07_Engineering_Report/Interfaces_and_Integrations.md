# Interfaces and Integrations — RealLab v1.605

## Internal interfaces

- ADO 2.7 / `SQLOLEDB.1` (SQL Server); Jet 4 OLEDB (Access).
- COM/ActiveX control stack (Microsoft + 5 third-party packs + vendor DLLs).
- DataReport (MSDBRPT.DLL) rendering engine.
- MSDataShape (SHAPE) for hierarchical reports.

## External integrations

| Integration | Mechanism | Evidence | Confidence |
|---|---|---|---|
| SQL Server | ADO 2.7 / SQLOLEDB.1 | FIND-T1-005 | High |
| Access mirror | Jet 4 OLEDB | FIND-T2-001 | High |
| SMTP e-mail | System.Net.Mail (.NET 4.0) | FIND-T1-007 | Medium |
| SMS | smsmisr.com cleartext HTTP, creds in URL | U-016 | High |
| Serial analysers | MSCOMM32.OCX; BF6500 hematology | FIND-T1-007 | Medium |
| IE COM automation | InternetExplorer object | FND-F-006 | Medium |
| MySQL (possible) | ODBC connector referenced | FIND-T3-011 | Low (hypothesis) |

## Sibling applications

`real lab Image Gallery.exe`, `Patient Data Transport.exe`, `RealLab E-Mail Sender.exe`, `RLS Access/SQL DB Tools`, `RLUTool`, `calculator.exe` (missing), `real lab timer.exe` (missing).