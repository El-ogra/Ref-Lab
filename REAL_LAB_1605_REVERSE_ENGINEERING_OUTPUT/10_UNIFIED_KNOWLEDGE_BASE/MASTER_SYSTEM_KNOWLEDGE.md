# MASTER SYSTEM KNOWLEDGE — REAL LAB SYSTEM v1.605

**Central engineering reference.** This document integrates all recovered knowledge into one technical and behavioral model. It is written to let a senior engineer understand or reconstruct the target without re-deriving the investigation.

**Evidence basis:** `D:\Collective work` (973 inventoried artifacts; see `02_SOURCE_INVENTORY\Complete_Artifact_Inventory.csv`). Prior reports in that repository are used as leads and attributed as prior-work claims; primary dumps take precedence.  
**Date:** 2026-11-16.  
**Confidence vocabulary:** VERIFIED (direct primary evidence) · STRONGLY_SUPPORTED (multiple consistent sources) · INFERRED (logical derivation) · UNVERIFIED (insufficient evidence) · CONFLICTED (contradictory evidence) · UNKNOWN (undetermined).

---

## 1. What the system is

Real Lab System is a **clinical laboratory information system (LIS)** for medical laboratories, distributed by RealLab / "ais group" (Egypt). The target version, **v1.605** (prior-work identifier `V-CD-2019`), is a 32-bit Windows desktop application written in **Visual Basic 6**, using **Microsoft Access (Jet 4)** databases with an optional **SQL Server** backend, Arabic/English UI, and an integrated print/PDF reporting pipeline. [VERIFIED: `Version_Identity_and_Scope.md`, `real lab system.exe (2).json`, schema dumps]

It manages the full laboratory workflow: patient registration, test ordering, result entry and review, test catalogue and reference ranges, financial/billing operations, reporting/printing, backup/transport, SMS/e-mail notification, and operator permissions.

## 2. How it is architected

Two-tier desktop LIS (no application server in the shipped architecture):

```
Presentation  (132 VB6 Forms + 66 DataReport designers + ActiveX controls)
      │  VB6 event handlers
Business logic (VB6 modules: labdb, labdb1, labfunction, printerselect, BCWFonts, …; 8,026 declared methods)
      │  ADO 2.7 / MSADODC / MSDataShape hierarchical recordsets
Data access   (SQL statements + SHAPE commands; bound recordsets for writes)
      │  SQLOLEDB.1 / Jet OLEDB / UDL
Data stores   (Patients.mdb 103 tables/1682 cols; Branch_Data.mdb template; data.mdb; SQL Server catalogues patients/Interface/VIDAS)
      │
Reporting/Print (DataReport → Win32 spooler and/or Bullzip PDF Printer; RTF method sheets; INI templates)
Configuration   (BASE.ini, SETTING.ini, IDCARD.INI, NOTE.ini, REPORT.dll, 223 Data\*.ini)
```

This is reconstructed from object dumps, SQL corpus and prior architecture reports; **no service layer exists in the target** (the C# reconstruction in the local repo is a different system — see §21). [STRONGLY_SUPPORTED]

## 3. Technologies actually used

| Layer | Technology | Evidence |
|---|---|---|
| Language/runtime | Visual Basic 6 native PE32, `MSVBVM60.DLL` (231 imports in v1.605 vs 186 in v1.00) | `real lab system.exe (2).json`, `real lab system.exe.json` |
| Data access | ADO 2.7 (`msado27.tlb`), `MSADODC.OCX`, `MSDataShape.1`, `SQLOLEDB.1`, Jet 4 | statement corpus, `dependency-inventory.json`, UDL/INI dumps |
| Database | Access `Patients.mdb` (103 tables/1,682 cols), `Branch_Data.mdb` (49/969/0 rows), `data.mdb`; SQL Server catalogues `patients`, `Interface`, `VIDAS` (uncharacterized) | `v2019-Patients.mdb.sql`, `schema-Branch_Data.sql`, `Version_Identity_and_Scope.md` |
| UI controls | MSCOMCTL, MSCOMCT2, MSFLXGRD, DBGRID32, MSADODC, MSDBRPTR, MSCOMM32, RICHTX32, MSCHRT20 + 5 third-party control packs + NOTEX.DLL / REPORT.DLL / unrar.dll | `*.ocx.json`, `control-typelib-surface.json`, prior component report |
| Reporting | VB6 DataReport (66 designers), Win32 spooler APIs, Bullzip PDF Printer 11.1, Ghostscript Lite 9.14.17, Xpdf Lite 3.3.4, PDF Power Tool 4.0, 228 RTF templates | `13-pdf-print-architecture.md`, `pdf-print-stack.md`, `template-corpus-228.json` |
| Notification | SMS (vendor provider) and e-mail via auxiliary .NET 4.0 `RealLab E-Mail Sender.exe` | prior report `09-...`/`behavioural-analysis.md` (auxiliary component evidence: `RealLab E-Mail Sender.exe` present locally) |
| Deployment | VB6 Setup Wizard (`RL_SETUP.exe`, 23,060,480 B, v19, 2020-09-01), WIN7/WINXP CAB targets, 39-file `SETUP.LST`, `Setup*.bat` | `Version_Identity_and_Scope.md` §3, `12-install-media.md`, B01–B10 archive contents |

## 4. Executable components

All executable artifacts were inventoried (`03_EXECUTABLE_REVERSE_ENGINEERING\Executable_Inventory.md`). Target components:

- `real lab system.exe` — main application. v1.605: 76,292,096 B, SHA `C54CE6C8…`, PE32/i386/GUI, 3 sections, linker 6.0, 231 MSVBVM60 imports. **This binary is absent locally; characterized from `real lab system.exe (2).json`.**
- v1.00 baseline present locally: 59,150,336 B, SHA `12828D0A…`, file version 1.00, 186 MSVBVM60 imports.
- `real lab Image Gallery.exe` (10,219,798 B), `Patient Data Transport.exe` (1,294,336 B), `RealLab E-Mail Sender.exe` (74,240 B, .NET 4.0 auxiliary), `RealLab_Show_Info.exe` (204,800 B), `RLS ACCESS Database Tools.exe` (1,531,904 B), `RLS SQL Database Tools.exe` (114,688 B), `RLUTool.exe` (126,976 B), `real lab timer.exe` (135,168 B), `calculator.exe` (114,688 B), `RL_SETUP.exe` (23,060,480 B), `001.exe`–`012.exe` + `007a.exe` (13 Macromedia Flash Player 8 projector simulations).
- Third-party/not target logic: AnyDesk, TeamViewerQS, Supremo, installers for Bullzip, Ghostscript, Xpdf, PDF Power Tool, .NET 4.0, MDAC, VB6 SP6, `xcopy.exe`, `acmsetup.exe`, `setupsp6.exe`.

## 5. Class and method interactions

210 VB6 objects: 132 forms, 66 DataReports, 9 modules, 3 classes (`objects-full__[analysis_BATCH-04_vb6_v1605].txt`). Largest core objects include `patientmain` (84 methods), `addpatient` (192), `labmain` (71), `patientview` (33), `labtolab` (32), `labdb` (24), `labdb1` (20), `labmainTools` (6). Interface object names and method counts are VERIFIED; control names (`cmdOK`, `Command1`, …) are known, but **method names for most of the 8,026 declared methods were not stored in the evidence** (only 1,268 have resolvable addresses; `Consolidated_Report.md` §224). Therefore no per-method call graph is asserted. Events flow: Form_Load → config/DB open → command handler → `labdb*` → SQL/recordset → DataReport/grid/printer. [STRUCTURE VERIFIED, FLOW INFERRED]

## 6. Database structure

`Patients.mdb` v1.605: 103 tables / 1,682 columns. v1.00: 93 / 1,527. Branch template: 49 tables / 969 columns / 0 rows. Backup: 90 / 1,381. Reconstructed from `v2019-Patients.mdb.sql`, `v1-Patients.mdb.sql`, `schema-Branch_Data.sql`, and the 1.4 MB parsed schema at `04_DATABASE_FORENSICS\_work\parsed_schema.json`. Major groups: `patientinfo` (demographics; **39 cols**), `patienttest` (orders/result status; **46 cols**), `testtable` (**38 cols**, 609 tests), `cbc` (**29 cols**), `kidney` (15 cols) — column counts from the primary v1.605 DDL via `parsed_schema.json`, `normalrange` (852 rows / 304 tests; sex/age stratified), `pricelist`, `payment`, `constanttable` (PTCONS/CBCCONS), ~96 per-test profile result tables (one per panel: `cbc`, `kidney`, `lipids`, `liver`, …). **Critical naming trap:** the table named `patients` is the operator/account table (`patientusname`, `grant1..grant12`), not patient demographics. [VERIFIED from DDL]

## 7. Application ↔ database communication

ADO/OLEDB via `labdb`/`labdb1`. **Dual-dialect evidence:** major queries exist in both SQL Server form (`ROUND(...,2)`, `'date'` literals, `varchar(MAX)`) and Access/Jet form (`#date#`, `Memo`) — e.g. `SUBSTRING` vs `mid` at VAs `682F56`/`68313E`. The runtime selects `normalrange.REFRANGE='2'` rows that are absent from the shipped Access mirror, implying authoritative reference ranges reside on an uncollected SQL Server database. [VERIFIED from literal pairs; INFERRED which backend was active at runtime]
Read path: literal `SELECT` (1,381) and `SHAPE` (27) statements with virtual addresses, selected into hierarchical recordsets. Write path: **bound recordsets (`AddNew`/`Update`) — there are 0 literal `INSERT INTO` and 0 `UPDATE … SET` statements** in the 1,797-statement corpus. Delete path: 72 `DELETE FROM` statements across 64 tables (manual cascade by `patientid`). DDL path: 268 embedded statements (27 CREATE + 241 ALTER), applied only partially to the shipped database (23 SQL-Server-only + 38 Access-applicable not applied). Backup: `EXEC sp_addumpdevice` / `sp_dropdevice`. [VERIFIED: `statements__[analysis_BATCH-04_sql_v1605].csv`, `ddl-evidence__[analysis_BATCH-04_sql_v1605].txt`]

## 8. Major business operations

Registration → `patientinfo`; ordering → `patienttest` (one row per test/order); result entry → per-test profile tables plus `resultfinsh` flag; review → `resultreview`; printing → `resultprint`; billing → `pricelist` + `payment` + `patientinfo` price/paid/balance columns; backup → SQL Server backup devices or file transport utilities; notification → SMS/e-mail; send-out → `inotherlab`/`otherlabprice`; branch mirror → `Branch_Data.mdb` template. [INFERRED from SQL + schema + docs]

## 9. Laboratory order processing

Worklists query `patientinfo` by `requestdate` and `completes`, often joining `patienttest` (VA `6843F2`). Test selection uses `SHAPE {subgrouptable} APPEND {testtable … WHERE testorno=True} RELATE subgroupcode` (VA `672722`). Results are written to analyte columns of per-test profile tables keyed by `patientid`+`requestdate` (prior-work finding, STRONGLY_SUPPORTED). The exact order-state machine beyond the result flags is UNKNOWN.

## 10. Simple and composite tests

Four representations coexist and must not be conflated: (1) the 609-row `testtable` catalogue, (2) the `patienttest` order/result row, (3) ~96 per-discipline profile tables with many analyte columns, (4) per-test INI/RTF report templates. **Structural finding:** `patienttest` carries a primary result `RESULTT` plus `resultt1..resultt4`, `RESULTCASEPRINT`, `normalrang`, `testunit`, `lowlimit`/`highlimit` — i.e. **up to five result components can live on one row** (VA `6C3AFC` selects `RESULTT1..T4` for `testcode=42`; VA `6AD3A8` shows a single-component case). However, **no field declares "this is a composite test"** — component count is implicitly encoded by which `resulttN` columns a `testcode` uses, and that mapping requires the missing form code. Simple tests (e.g. Creatinine/Urea/Uric Acid) and composite panels (e.g. CBC) therefore share a storage mechanism but are not provably processed identically. [STRUCTURE VERIFIED; composite declaration UNKNOWN]

## 11. Results calculation and management

Reference ranges from `normalrange` stratified by sex (`sex` column; prior counts M 422 / F 421) and age (`firstold`, `secondold`, `ageunit`). Derived values use `constanttable` (PTCONS for PT/INR; CBCCONS for CBC). Result lifecycle evidence: `resultfinsh`+`edituser/editdate` (VA `6860BE`), `resultreview`+`prevuser/prevdate` (VA `686312`), `resultprint`+`printuser/printdate` (VA `6865A6`), plus a fourth audited handover stage `give`+`taslimuser/taslimdate` (VA `6867AA`); all are read together in queue predicates (VAs `681ED2`, `686EC2`, `6B3C00`, `6B3FA8`). `NatighU` (result value) is selected alongside the flags. Flag ordering is a reconstruction, not an observation. [FIELDS VERIFIED; SEQUENCE INFERRED]

## 12. Financial transactions

Columns `totalprice`, `totalpricedisc`, `elmadfoo3` (paid), `elbakyma3mal` (balance) appear in billing GROUP BY clauses (VAs `6977A6`, `69DE12`, `69E4E6`, `69F48A`, `69FB46`, `6A16A2`). `pricelist` holds per-test patient/lab/other-lab prices (`patientprice`, `labprice`, `otherlabprice`, VA `6A9632`); `payment` holds transactions. **A daily/period aggregate formula is recovered verbatim** at VA `69EA76`: `Round(sum(totalprice),2)`, `Round(sum(totalpricedisc),2)`, `Round(sum(totalprice)-sum(totalpricedisc),2)`, `Round(sum(elmadfoo3),2)`, `Round(sum(elbakyma3mal),2)` grouped by `pricemethod` — i.e. net = gross − discount, rounded to 2 decimals. Invoice headers also carry `discount` and `discount100` (VA `6A5796`); whether `discount100` is a percentage is INFERRED from its name. Per-invoice computation and rounding of `discount100` remain UNVERIFIED. [FORMULA VERIFIED for aggregate; PARTIAL overall]

## 13. Printing and reporting

66 VB6 DataReport designers render hierarchical recordsets through `MSDBRPTR.DLL` 6.0 SP4; output goes to Win32 printer spooler (`OpenPrinterA`/`SetPrinterA`/`GetPrinterA`/`ClosePrinter`) and/or a hard-coded `Bullzip PDF Printer 11.1` with four `D:\PDF\` output directories. Template corpus: 228 templates / 117 identities in two disjoint tiers — alphabetic identities bind to a table, numeric identities bind to a `testcode` (37 INI `joined_table` bindings connect templates to per-test profile tables). Ghostscript/Xpdf/PDF Power Tool are manual side tools with no evidenced application call path. No .NET reporting stack is used in the target. [COMPONENTS/TEMPLATES VERIFIED; PIPELINE ORDER INFERRED]

## 14. Application states

Four result flags plus `patientinfo.completes` are the state-bearing fields (§11, §9). No evidence supports a richer priority/sequential state model; none is invented. `patients` account table holds `grant1..grant12` permission flags and shift fields.

## 15. Errors and exceptional situations

A literal error string exists at VA `6717E2` ("An error has occurred, Please make sure your browser, Or select other"), but its UI context is unestablished. Documented environmental requirement: English regional date format; SQL uses both quoted and `#…#` date literals. Locale-failure behavior is UNVERIFIED. 0 embedded DDL applies cleanly to the shipped DB (38 mismatches) — a concrete configurationStateException capability exists but its trigger is UNKNOWN.

## 16. Permissions and authentication

Login uses the `patients` operator table (`patientusname`, `grant1..grant12`, shift schedule). `loger` table is referenced in SQL (VA `66F746`, `SELECT * FROM loger WHERE lognum=`) for logging. Prior work reports a default operator credential pair (literal values withheld here per the no-secrets rule) — treated as a PRIOR-WORK CLAIM, not re-derived locally. Form-level gating by grant flags is INFERRED.

## 17. Actual operational behavior

**Not observed.** No target binary was executed. All behavioral descriptions are static reconstructions from SQL literals, object structures, schema, configuration and prior documentation. This is the single largest limitation of the knowledge base. See `07_RUNTIME_AND_BEHAVIORAL_ANALYSIS\Runtime_Observations.md`.

## 18. Confirmed findings

- Target identity/version/size/hash and VB6/PE structure (from the two PE dumps).
- 210 objects / 8,026 methods / 1,797 SQL / 268 DDL / 103 tables / 1,682 columns.
- Result flag tuple and six audit columns; 64-table deletion cascade; 0 INSERT/0 UPDATE.
- SHAPE hierarchical queries; 13-character patient ID with `SUBSTRING(...,10,3)`.
- 973 artifacts hashed; 8 ZIPs with 4,395 entries; no nested archives.
- **Correction of a prior claim:** `Version_Identity_and_Scope.md` §3.1 and the ★ note there state that both executables carry PE timestamp 2013-12-27 and that this must not be used as a version discriminator. The two primary PE dumps disagree: `real lab system.exe.json` has `timestamp=1388267701` (2013-12-28T21:55:01Z) while `real lab system.exe (2).json` has `timestamp=1595435962` (2020-07-22T16:39:22Z). The v1.605 timestamp is VERIFIED from the primary dump; the prior "same timestamp" claim is CONFLICTED/corrected. See `09_CROSS_SOURCE_EVIDENCE\Conflicting_Findings.md`.

## 19. Inferred findings

- End-to-end order/result/print/bill workflow ordering.
- VB6 event→module→SQL→DataReport flow.
- Sequential result flag progression; manual cascade as a single transaction; financial calculation shape; branch/offline sync model; the role of `completes`.

## 20. Unknown behaviors and why

- **Composite-test component storage** — template/dump evidence does not show the write path (target binary absent).
- **Financial formula/rounding** — columns exist; calculation code not recovered.
- **VIDAS database nature** — referenced only; no artifact.
- **RL_SETUP 22.95 MiB embedded payload** — characterized but not decomposed.
- **Runtime behavior of any kind** — target binary absent; no isolated environment; safety rules.
- **Most method names and complete call graph** — not stored in evidence (1,268/8,026 addressable).
- **DDL migration trigger** — capability evidenced; trigger not.

## 21. Guard against cross-source confusion

The local repository `C:\Users\LAP LINK\source\repos\Ref Lab` contains a **C#/.NET 8/WPF/MediatR/EF Core clean-architecture reconstruction** of the lab domain (73 use cases, 27 tables, 14 screens). It is **not** the target. Facts about `App.xaml.cs`, MediatR, QuestPDF, migrations, or DI in that repository must never be attributed to REAL LAB SYSTEM v1.605. [VERIFIED: `Docs/KB/START-HERE.md`, infrastructure audit]

---

**Companion documents:** `MASTER_SYSTEM_ARCHITECTURE.md`, `MASTER_DATABASE_KNOWLEDGE.md`, `MASTER_BEHAVIORAL_KNOWLEDGE.md`, `MASTER_BUSINESS_LOGIC.md`, `MASTER_TECHNICAL_SPECIFICATION.md`, `MASTER_REVERSE_ENGINEERING_EVIDENCE.md`.
