# Startup and Initialization (Static Reconstruction — NOT Runtime Observed)

## 1. Authoritative startup facts

| Fact | Evidence | Confidence |
|---|---|---|
| VB project name is `Real_lab_system`; no `Sub Main`; the application starts from a form | `Batch1_Analysis_Report.md` §3.2 / C-3: "lpSubMain 0x00000000 (no Sub Main; the app starts from a form)" — derived from the VB project structure of the v1.00 executable, corroborated by the shared project identity | STRONGLY_SUPPORTED for v1.00; INFERRED for v1.605 (same product line and identical core module names, but the v1.605 binary is not locally available for re-check) |
| The principal form is `labmain` (71 methods in v1.605; 184 controls per training/report docs) | `objects-full__[analysis_BATCH-04_vb6_v1605].txt` object [0] `labmain form methods=71`; `10-training-video.md` and `Stage-5-V-CD-2019-Cumulative-Report.md` | VERIFIED (method count); STRONGLY_SUPPORTED (control count) |
| Core startup-adjacent modules: `labdb`, `labdb1`, `labfunction`, `printerselect`, `BCWFonts` | object dumps for both versions; `system-architecture.md`, `master-engineering-reference.md` | VERIFIED (presence); INFERRED (exact init ordering) |
| Configuration is read from INI files in the application directory (`BASE.ini`, `SETTING.ini`, `IDCARD.INI`, `NOTE.ini`, `REPORT.dll`, 223 `Data\*.ini`) plus UDL connection files | `ini-analysis.json`, `ini-key-delta.csv`, `06-configuration.md`, `configuration-and-resources.md`, prior reports | STRONGLY_SUPPORTED |
| Win32 profile API `GetPrivateProfileStringA` / `WritePrivateProfileStringA` is part of the resolved API set | `behavioural-analysis.md` §§2.2, 3 (S-1, S-5) | STRONGLY_SUPPORTED (mechanism); the specific app call sites were not observed |
| Data providers available to the startup connection logic: `SQLOLEDB.1`, `MSDataShape.1`, Jet/ACE via `Provider=Microsoft.Jet.OLEDB…` | SQL/connection-string evidence in `statements__[analysis_BATCH-04_sql_v1605].csv`, UDL/INI dumps, prior reports | STRONGLY_SUPPORTED |
| Startup environment expectation documented in the official guide: English regional date format, server machine running | `Functionality_Requirements_Matrix_V1.md` V1-FUNC-001; `Functional_Specification_V1.md` F-V1-001 | STRONGLY_SUPPORTED (documented requirement), not runtime verified |

## 2. Reconstructed (INFERRED) initialization sequence

The following ordering is an **inference** from static structure and documented usage. No step was observed executing.

1. Windows loads the PE; the VB6 runtime entry (`ThunRTMain`) transfers control to the VB6 project's startup form (`labmain`). [INFERRED from VB6 runtime model + `Sub Main` absence]
2. The startup form and its 184 controls are instantiated; Arabic UI labels come from the verified label set / INI resources. [INFERRED]
3. Configuration is read (`BASE.ini`, `SETTING.ini`, `IDCARD.INI`, `REPORT.dll`, `Data\*.ini`) through the profile API. [STRONGLY_SUPPORTED as capability; INFERRED as exact sequence]
4. A data connection is selected (Jet/Access `.mdb` or SQL Server via UDL/`SQLOLEDB.1`), driven by configuration. [STRONGLY_SUPPORTED as capability]
5. Printer selection is resolved from `[selectprinter]` settings and `printerselect`. [STRONGLY_SUPPORTED]
6. The operator reaches the sign-in screen; the `patients` table holds user/login attributes; authorization gates subsequent forms. [INFERRED; see `08_BUSINESS_DOMAIN_RECONSTRUCTION\Security_And_Permissions.md`]
7. The main menu exposes the laboratory modules (patients/orders, results, catalogue, billing, reporting, settings, statistics, send-out, microbiology). [INFERRED from form inventory + CHM topics + functional specification]

## 3. What is explicitly unknown

- The exact startup form field of the v1.605 VB project (the v1.605 `.vbp` is not present; the claim is inherited from v1.00 plus shared identity).
- Whether any first-run registration/migration prompt exists in v1.605 (268 embedded DDL statements suggest schema migration capability, but the trigger condition — installer-time vs application-start — is **UNVERIFIED**).
- Ordering between configuration load, connection open, printer resolution, and login form display.
- Behavior on connection failure (an error string exists in the SQL corpus: `"An error has occurred, Please make sure your browser, Or select other"` at VA `6717E2`, but its UI context is not established).

## 4. Contrast with the C# reconstruction repository

The local `Ref Lab` repository is a **clean-architecture reconstruction** (WPF/MediatR/EF Core), not the target. Its startup (`App.xaml.cs`, `BuildHost()`, no `StartupUri`, first-run database-setup window) is documented in evidence files such as `Unit-1-Reference-Conformance-Report.md` and `05-Architecture-and-Dependency-Blueprint.md`. Those facts must **never** be attributed to REAL LAB SYSTEM v1.605. This distinction is a deliberate guard against a high-risk cross-source confusion.
