# MASTER TECHNICAL SPECIFICATION — REAL LAB SYSTEM v1.605

## 1. Target binary

| Property | Value | Source |
|---|---|---|
| File | `real lab system.exe` (v1.605) | `real lab system.exe (2).json` |
| Size | 76,292,096 bytes | same |
| SHA-256 | `C54CE6C8C98A679EFC5AB485BC4B313B452026D6A40F9BDA36ECA7EA18DF93C0` | same |
| Format | PE32, `IMAGE_FILE_MACHINE_I386`, GUI subsystem | same |
| Sections | 3 (`.text`, `.data`, `.rsrc`), linker 6.0, OS version 4.0 | same |
| Entry point RVA | `0x7142c` | same |
| Image base | `0x400000` | same |
| PE timestamp | 1,595,435,962 → 2020-07-22T16:39:22Z | same (corrects prior "2013-12-27" claim) |
| File description / company | `RLS 1.605` / `RealLab` (prior-work attribution) | `Version_Identity_and_Scope.md` §3.1 |
| Runtime imports | `MSVBVM60.DLL`, 231 functions | PE dump |
| Local availability | **Binary absent**; only the dump exists | exhaustive search of 973 files + 8 ZIPs |

Baseline v1.00: 59,150,336 B, SHA `12828D0A…`, timestamp 1,388,267,701 (2013-12-28T21:55:01Z), 186 MSVBVM60 imports, entry RVA `0x4680c`, `.text` entropy 6.1911.

## 2. Code model

- VB6 native compilation: program logic is stored as VB6 object/method structures interpreted by `MSVBVM60.DLL`. 210 objects (132 forms, 66 DataReports, 9 modules, 3 classes); 8,026 declared methods, 1,268 with resolvable addresses; 270,166 string literals (prior work).
- Native Ghidra function inventory (`function_inventory.csv`, 1.6 MB, for v1.00) is dominated by thunks and auto-generated `FUN_` addresses — expected for VB6; it does not replace object-level analysis.
- Custom/3rd-party ActiveX controls: MSCOMCTL, MSCOMCT2, MSFLXGRD, DBGRID32, MSADODC, MSDBRPTR, MSCOMM32, MSCHRT20, RICHTX32, TABCTL32, MCI32, COMDLG32 + ChamaleonButton, RgheedButtonControlV2, XP_BUTTONS, vkUserControlsXP, Redsoul911XPCOmponents + vendor NOTEX.DLL, REPORT.DLL, unrar.dll.

## 3. Data layer specification

- Providers: `SQLOLEDB.1`, `MSDataShape.1`, Jet OLEDB; UDL connection files.
- Statement corpus (`statements__[analysis_BATCH-04_sql_v1605].csv`): 1,797 rows; columns `va,verb,tables,has_star,length,text`; distribution SELECT 1,381 / ALTER 241 / DELETE FROM 72 / OTHER 49 / CREATE 27 / SHAPE 27; 0 INSERT / 0 UPDATE.
- Most referenced tables: `patientinfo` 428, `patienttest` 343, `normalrange` 95, `testtable` 74, `payment` 32, `comments` 28, `gehats` 27, `insulin` 23, `sharekathesab` 19, `patients` 14, `subgrouptable` 14 (from `tables__[analysis_BATCH-04_sql_v1605].csv`).
- DDL corpus (`ddl-evidence__[analysis_BATCH-04_sql_v1605].txt`): 27 CREATE TABLE + 241 ALTER TABLE.
- Schema: `Patients.mdb` 103 tables / 1,682 columns (parsed JSON at `04_DATABASE_FORENSICS\_work\parsed_schema.json`).

## 4. Reporting/printing stack

VB6 DataReport designers (66) → Win32 spooler (`OpenPrinterA`, `SetPrinterA`, `GetPrinterA`, `ClosePrinter`) → physical printers; Bullzip PDF Printer 11.1 (with Ghostscript Lite 9.14.17, Xpdf Lite 3.3.4, PDF Power Tool 4.0) for PDF output. 228 RTF templates in `template-corpus-228.json`; 29 method sheets identical across versions; INI-driven `REPORT.dll` with strip settings (`reportview`).

## 5. Deployment/runtime requirements

Windows (WIN7/WINXP install targets); VB6 SP6 runtime (`MSVBVM60.DLL` build 9782 per prior work); MDAC 2.8 (`MDAC_TYP.EXE`); .NET Framework 4.0 (`dotNetFx40_Full_x86_x64.exe`, for the Bullzip stub); Jet 4; optional SQL Server; 4 logical print queues per `SETTING.ini`; serial/TCP links to analysers.

## 6. Toolchain used for this investigation

Verified: PowerShell 7.6.10, Node 24.19.0, Python 3.13.1, .NET SDK 9.0.321, Java 21, REA CLI 6.7.0 (MCP namespace became unavailable after a server restart), Ghidra 12.1.4 headless (works when `JAVA_HOME` is set in the shell; the REA provider remained blocked), `sqlcmd`, `psql` (absolute path), Git 2.49.0. See `01_ENVIRONMENT_AND_TOOLS\`.

## 7. Limitations that constrain the specification

1. Target binary absent → no IL/native decompilation of v1.605 in this run; no runtime verification.
2. No `.mdb` files → no live schema/data validation.
3. Most method names not stored in evidence → no complete call graph.
4. REA namespace loss after server restart → no further MCP-based analysis.
5. VIDAS structure and RL_SETUP embedded payload unknown.
