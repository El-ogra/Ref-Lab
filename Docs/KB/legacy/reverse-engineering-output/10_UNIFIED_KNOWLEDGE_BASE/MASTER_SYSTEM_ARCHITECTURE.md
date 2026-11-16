# MASTER SYSTEM ARCHITECTURE — REAL LAB SYSTEM v1.605

## 1. Architectural classification

Classic **two-tier, fat-client Windows desktop LIS**. There is no application server, no web tier, and no service layer in the shipped target. The VB6 executable contains presentation, business logic, data access and report rendering. [STRONGLY_SUPPORTED: object inventory + SQL corpus + prior architecture reports]

```
┌──────────────────────────────────────────────────────────────────────┐
│ PRESENTATION (in-process)                                            │
│ 132 VB6 Forms (labmain main menu; patientmain; addpatient; Result*;  │
│ Report*; frm_money; …) + 66 DataReport designers + ActiveX controls  │
│ (MSCOMCTL, MSFLXGRD, DBGRID32, MSADODC, MSDBRPTR, MSCOMM32, …)      │
└───────────────┬──────────────────────────────────────────────────────┘
                │ VB6 events (Form_Load, *_Click)
┌───────────────▼──────────────────────────────────────────────────────┐
│ BUSINESS LOGIC (in-process VB6 modules/classes)                      │
│ labdb, labdb1, labfunction, printerselect, BCWFonts + 3 classes;     │
│ 8,026 declared methods (only 1,268 addressable in the dumps)         │
└───────────────┬──────────────────────────────────────────────────────┘
                │ ADO 2.7 / MSDataShape hierarchical recordsets
┌───────────────▼──────────────────────────────────────────────────────┐
│ DATA ACCESS                                                          │
│ 1,797 SQL literals (1,381 SELECT / 27 SHAPE / 72 DELETE FROM /       │
│ 241 ALTER / 27 CREATE / 49 OTHER) + bound recordsets for writes;     │
│ 268 embedded DDL for schema migration                                │
└───────────────┬──────────────────────────────────────────────────────┘
                │ SQLOLEDB.1 / MSDataShape.1 / Jet OLEDB / UDL
┌───────────────▼──────────────────────────────────────────────────────┐
│ DATA STORES                                                          │
│ Patients.mdb (103 tables/1,682 cols) · Branch_Data.mdb (49/969, 0 rows│
│ template) · data.mdb · SQL Server catalogues patients/Interface/     │
│ VIDAS (uncharacterized)                                              │
└───────────────┬──────────────────────────────────────────────────────┘
                │
┌───────────────▼──────────────────────────────────────────────────────┐
│ CROSS-CUTTING                                                        │
│ Reporting: DataReport → Win32 spooler / Bullzip PDF (Ghostscript,     │
│ Xpdf, PDF Power Tool) · Config: INI/UDL · Comms: serial (MSCOMM32),   │
│ TCP (MSWINSCK), SMS, .NET SMTP sender · Deploy: RL_SETUP/Setup*.bat   │
└──────────────────────────────────────────────────────────────────────┘
```

## 2. Component responsibilities

| Component | Role | Evidence |
|---|---|---|
| `labmain` (71 methods) | Main menu / shell form | object dump; training docs |
| `patientmain` (84), `addpatient` (192), `patientview` (33) | Patient/order UI | object dump |
| `labdb` (24), `labdb1` (20) | Data-access modules | object dump; prior architecture |
| `printerselect` | Printer selection module | object dump; `Version_Identity_and_Scope.md` §2.5 |
| `Result*` forms | Result entry per discipline | object dump + prior reports |
| `Report*` + 66 DataReport | Report rendering | object dump |
| `labtolab` (32) | Send-out to other laboratory | object dump |
| Auxiliary EXEs | Gallery, data transport, DB tools, uploader, e-mail sender, installer | `Executable_Inventory.md` |

## 3. Runtime model

- PE32 / i386 / Windows GUI, linker 6.0, 3 sections (`.text` ~58.98 MB raw for v1.00), entry point via VB6 runtime (`ThunRTMain`). `MSVBVM60.DLL` interprets VB6 p-code/method structures; native decompilation therefore recovers runtime scaffolding (thunks, imports, `FUN_…`) rather than business logic. [VERIFIED: both PE dumps + `function_inventory.csv`]
- No ASLR/DEP/NX in the shipped binaries (prior-work finding; `DllCharacteristics=0`).
- Configuration is resolved at startup from INI/UDL files in the application directory; English regional date format is a documented requirement.

## 4. Data architecture

- Dual-engine capability: Access/Jet for branch/offline use; SQL Server for the central store. `SQLOLEDB.1`, `MSDataShape.1`, Jet OLEDB connection strings; `data_connection*.udl` files.
- Hierarchical recordsets via `SHAPE {parent} APPEND {child} RELATE …` (27 statements) bind grouped test catalogues and reference ranges.
- Writes are bound-recordset operations (no literal `INSERT`/`UPDATE`); deletes use literal `DELETE FROM` across 64 tables.
- Schema migration is embedded (268 DDL statements), with partial application to the shipped database.

## 5. Deployment architecture

`RL_SETUP.exe` (VB6 Setup Wizard v19, 2020-09-01, 23,060,480 B, 57 objects/599 methods) installs from WIN7/ and WINXP/ CAB targets with `setup.exe` + `real lab system.CAB` (48 members) + `SETUP.LST` (39 files). Dependencies: VB6 SP6 redistributable (73 files), MDAC 2.8, .NET Framework 4.0 (Bullzip stub), Bullzip PDF Printer 11.1, Ghostscript Lite, Xpdf Lite, PDF Power Tool. Five Microsoft installers referenced by `Setup*.bat` are absent; a 22.95 MiB embedded payload inside `RL_SETUP.exe` remains uncharacterized. [STRONGLY_SUPPORTED: `Version_Identity_and_Scope.md`, `12-install-media.md`, B01–B10 archive inventory]

## 6. Version relationship

v1.00 (bare file tree) and v1.605 (installer package) are the same product line: identical executable name/layout, shared modules, byte-identical CHM and 29 RTF sheets, 43 identical ActiveX controls. v1.605 adds 41 objects, 17 SQL-referenced tables, SMS, statistics, worksheets and attendance features. They are a feature superset, **not** a data-compatible upgrade. [STRONGLY_SUPPORTED: `Version_Identity_and_Scope.md` §4]

## 7. Architectural boundaries of confidence

- VERIFIED: object/method/table/SQL counts, PE structure, dependency names.
- INFERRED: layer responsibilities, navigation/event flow, transaction boundaries, deployment-time vs startup-time DDL execution.
- NOT APPLICABLE: managed-code architecture in the target (the .NET 4 `RealLab E-Mail Sender.exe` is an auxiliary SMTP component, not the main application).
- EXCLUDED: the C#/.NET 8/WPF reconstruction in the local repository is a different system.
