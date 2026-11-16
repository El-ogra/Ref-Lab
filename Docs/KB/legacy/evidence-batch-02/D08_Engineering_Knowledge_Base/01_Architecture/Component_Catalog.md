# Component Catalog — RealLab v1.605

## Core binaries

| Component | Role | Evidence | Confidence |
|---|---|---|---|
| `real lab system.exe` | main VB6 client, native code | FIND-T1-001/002/003 | High |
| `msvbvm60.dll` (build 9782) | VB6 runtime | FIND-T1-001 | High |
| `VBA6.DLL` | VBA6 runtime | dependency-inventory | Medium |
| `NOTEX.dll`, `REPORT.dll` | vendor note/report helpers | FIND-T1-006 | High |
| `unrar.dll` (3.50) | archive extraction (orphaned) | U-019 | Medium |

## Satellite utilities

| Component | Role | Evidence | Confidence |
|---|---|---|---|
| `real lab Image Gallery.exe` | image/gallery consumer (pictures table) | dependency-inventory, upload matrix | Medium |
| `Patient Data Transport.exe` | data transfer utility | PE analysis | Medium |
| `RealLab E-Mail Sender.exe` (.NET 4.0) | SMTP sender + .NET data access | FIND-T1-007 | Medium |
| `RLS Access Database Tools.exe` | Access DB maintenance | dependency text | Medium |
| `RLS SQL Database Tools.exe` | SQL DB maintenance (backup devices) | dependency text | Medium |
| `RLUTool.exe` | workstation tool | dependency text | Medium |
| `RL_SETUP.exe`, `Setup*.bat` | installer | Type 3 | High |
| `calculator.exe`, `real lab timer.exe` | referenced siblings, **missing** | upload matrix | Low (gap) |

## ActiveX / COM controls (external_component_count = 25)

Microsoft: `MSADODC`, `MSDATGRD`, `MSDATLST`, `MSDBRPT`, `MSFLXGRD`, `MSCHRT20`, `MSCOMCT2`, `mscomctl`, `COMDLG32`, `RICHTX32`, `MSCOMM32`, `MCI32`, `mswinsck`, `msado27.tlb`, `STDOLE2.TLB`, `MSBIND`, `MSSTDFMT`, `MSDBRPTR`, plus `msvbvm60`, `ASYCFILT`, `COMCAT`, `OLEAUT32`, `OLEPRO32`, `RICHED32`, `DBGRID32`, `TABCTL32`.

Third-party: `ChamaleonButton.ocx` (144 members), `RgheedButtonControlV2.ocx` (100), `XP_BUTTONS.ocx` (114), `vkUserControlsXP.ocx` (524), `Redsoul911XPCOmponents.ocx` (410).

## VB6 object inventory (v1.605)

- 212 objects, 8,026 methods: 132 forms, 66 DataReports, 9 modules, 3 classes.
- Largest by method count: `patientmain` (84), `labmain` (71), `frm_money` (38), `Resultliver` (…).
- V-ROOT-1.00 build: 169 objects / 163 forms (provisional version delta, CON-002).