# Dependency Graph — direct, transitive, runtime-only, and third-party

**Legend.** DIRECT = present in the PE import table or in a shipped config that the application reads. TRANSITIVE = reached through another component (not directly imported). RUNTIME-ONLY = late-bound / COM / dynamically created, not an import descriptor. THIRD-PARTY = not part of target logic. Evidence paths under `D:\Collective work\`.

## 1. Direct dependencies of the main executable (VERIFIED)

```
real lab system.exe (1.605)
   └── DIRECT (only import descriptor): MSVBVM60.DLL, 231 functions
        (real lab system.exe (2).json → pe.imports; pe-structure.json → imports[0])
```
1.00 baseline: same single DLL, 186 functions (`real lab system.exe.json`). The shipped `msvbvm60.dll` is 1,384,479 B, file version `6.0.98.2` / ProductVersion `6.00.9802` (`version-resources.json`), and is deployed by the installer and the VB6 SP6 redistributable (`12-install-media.md`). The runtime build recorded in the VB header is `9782` (SP6), consistent with that DLL version (`vb6-structure.json`, `primary-executable-reverse-engineering.md §2.2`).

No other DLL is directly imported by either main executable. Consequently all Win32, OLE/COM, printer, file and database calls are TRANSITIVE through `MSVBVM60.DLL` → `DllFunctionCall` (`primary-executable-reverse-engineering.md §2.3`).

## 2. Transitive / runtime-only active controls (VERIFIED versions; binding INFERRED)

`version-resources.json` lists the shipped native components with their version resources. They are **not** in the main import table — VB6 binds them via COM at runtime. Observed class-name strings in the auxiliary `NOTEX.dll` corroborate the pattern: `MCI.MMControl`, `XPbuttons.XPButton`, `MSComCtl2.DTPicker` (`RL_CD2019Access__bin__NOTEX.strings.txt`).

| Component | Version (`version-resources.json`) | Role | Dependency class |
|---|---|---|---|
| `MSDBRPTR.DLL` | 6.1.97.82 | VB6 DataReport engine | RUNTIME-ONLY; `primary-executable-technical-reference.md §8` calls the link "INFERRED (late-bound, not by import)" |
| `MSCOMCTL.OCX` | 6.1.97.82 | common controls (TreeView/ListView etc.) | RUNTIME-ONLY |
| `MSCOMCT2.OCX` | 6.1.97.82 | additional common controls (e.g. `DTPicker` observed in NOTEX strings) | RUNTIME-ONLY |
| `TABCTL32.OCX` | 6.1.97.82 | tab control | RUNTIME-ONLY |
| `COMDLG32.OCX` | 6.1.97.82 | common dialog | RUNTIME-ONLY |
| `MSFLXGRD.OCX` | 6.1.98.12 | flex grid | RUNTIME-ONLY |
| `MSDATGRD.OCX` / `MSDATLST.OCX` | 6.1.97.82 / 6.0.84.18 | data-bound grid/list | RUNTIME-ONLY |
| `DBGRID32.OCX` | 5.1.81.4 | data grid | RUNTIME-ONLY |
| `MSADODC.OCX` | 6.1.97.82 | ADO data control | RUNTIME-ONLY |
| `MSBIND.DLL` | 6.1.97.82 | data binding | RUNTIME-ONLY |
| `MSCHRT20.OCX` | 6.0.88.4 | charting | RUNTIME-ONLY |
| `MSCOMM32.OCX` | 6.1.98.13 | serial communications | RUNTIME-ONLY |
| `MCI32.OCX` | 6.0.84.18 | multimedia control (`MCI.MMControl` string observed) | RUNTIME-ONLY |
| `RICHTX32.OCX` / `RICHED32.DLL` | 6.1.97.82 / 4.0.993.4 | rich text | RUNTIME-ONLY |
| `MSSTDFMT.DLL` | 6.1.97.82 | standard formatter | RUNTIME-ONLY |
| `mswless.ocx` | 6.1.97.82 | visual component (`System/datareport_vh/`) | RUNTIME-ONLY |
| `msscript.ocx`, `dispex.dll`, `jscript.dll`, `vbscript.dll` | 1.0.0.3715, 5.0.0.3715, 5.1.0.8513, 5.1.0.8513 | HTML/script designer support (`primary-executable-technical-reference.md §8`: CONFIRMED same directory) | RUNTIME-ONLY |
| `msado27.tlb`, `STDOLE2.TLB`, `OLEAUT32.DLL`, `OLEPRO32.DLL`, `ASYCFILT.DLL` | 2.81.1132.0, 2.40.4275.1, 2.40.4275.1, 5.0.4275.1, 2.40.4275.1 | OLE automation/type libraries | TRANSITIVE |
| `.oca` caches (`ChamaleonButton`, `RgheedButtonControlV2`, `vkUserControlsXP`, `XP_BUTTONS`, `Redsoul911XPCOmponents`, …) | no version resource | VB6 control type caches | RUNTIME-ONLY |
| `unrar.dll` | 3.50.3.111 | archive extraction | RUNTIME-ONLY; `RL_CD2019Access__OCX__unrar.xrefs.txt` (9,139 xref lines) |
| `RL_CD2019Access/bin/NOTEX.dll` | ProductVersion 2.00, CompanyName RealLab | RealLab auxiliary DLL; VB6-native (`VB5!` string, MSVBVM60 thunks in its xrefs) | RUNTIME-ONLY |

The header's `wExternalComponentCount = 25` (`vb6-structure.json`) is the declared external-component count; the **names** of those 25 descriptors were not decoded (UNR-009), so the table above is the shipped-component inventory, not a proven 1:1 list of those 25 references.

## 3. Data-store dependencies (VERIFIED connection strings; application binding INFERRED)

Source: `primary-executable-reverse-engineering.md §6.1–6.2`, `primary-executable-technical-reference.md §5`, `v1.605-package-manifest.csv`, `Version_Identity_and_Scope.md §3.3`.

```
MSDataShape.1  (outer provider, shapes the hierarchical recordsets)
   ├── Data Provider = SQLOLEDB.1   ──▶ SQL Server catalogue "Patients"   [SQL path]
   │        · Persist Security Info=True; User ID=<user>; Password=<password> fragments
   │        · or Integrated Security=SSPI
   └── Data Provider = Microsoft.Jet.OLEDB.4.0
            ──▶ D:\real lab system\bin\Patients.mdb              [Access/Jet mirror]
            ──▶ ...\Patients_backup.mdb  (offline backup)
            ──▶ ...\repairedDB.mdb       (repair copy)
```
- Four connection-string templates (two `SQLOLEDB.1`, two `MSDataShape.1` variants) are quoted verbatim in `primary-executable-reverse-engineering.md §6.1`; the Jet mirror and `.mdb` paths in §6.2. The credential is built by concatenating three separate UTF-16LE literals (`;User ID=`, `;Password=`) — VERIFIED as literals.
- UDL files in `v1.605-package-manifest.csv`: `data_connection.udl` (674 B, SHA `31DE843B…`), `data_connection1.udl` (430 B, SHA `66D80D6E…`), each present in both `bin\` trees.
- `Version_Identity_and_Scope.md §3.3` names SQL catalogues `patients`, `Interface`, and `VIDAS`; `VIDAS` is referenced but never structurally characterised (UNKNOWN, RL-UNC-003).
- `MDAC_TYP.EXE` (8,124,720 B; `version-resources.json` → `InternalName Wextract`, `4.71.1015.0`) is the shipped MDAC redistributable supplying Jet/OLE DB components.
- Evidence of use: 1,797 SQL/SHAPE statements in `statements__[analysis_BATCH-04_sql_v1605].csv` (SELECT 1,381 / ALTER 241 / DELETE FROM 72 / OTHER 49 / CREATE 27 / SHAPE 27). Which object issues which statement is UNVERIFIED (UNR-008).

## 4. Print / PDF dependency chain (THIRD-PARTY, STRONGLY_SUPPORTED)

```
printerselect module (11 methods, objects-full [140])
   └── Win32 spooler APIs  [baseline: OpenPrinterA / SetPrinterA / GetPrinterA / ClosePrinter —
                            Version_Identity_and_Scope.md §2.5, string evidence only]
   └── Bullzip PDF Printer 11.1.0.2600  (Setup_BullzipPDFPrinter_…exe, pe-envelope.csv)
        ├── .NET Framework 4.0 redistributable (dotNetFx40_Full_x86_x64.exe, "Box Stub")
        ├── GPL Ghostscript Lite 9.14.17 (gslite.exe)
        ├── Xpdf Lite 3.3.4 (xpdfsetup.exe)
        └── PDF Power Tool 4.0.0.11 (pdfpowertool_setup.exe)
```
All four PDF components are under `RL_CD2019Access\System\PDF\` in `v1.605-package-manifest.csv` and are enumerated as the PDF toolchain in `12-install-media.md`. They are **third-party redistributables, not target logic**; see `Managed_Assembly_Analysis.md` for the .NET determination. 1.00 has no PDF export (`Version_Identity_and_Scope.md §2.5`: zero `PDF`/`Bullzip` literals).

## 5. VB6 SP6 redistributable and installer chain (TRANSITIVE / deployment)

`12-install-media.md §5` documents 73 files under `RL_CD2019Access\System\datareport_vh\` (`VS6sp6B1.cab`, `vbrun60.cab` → `VBRUN60.EXE`, `Msvbvm60.cab` → `MSVBVM60.DLL`, `Oleaut.cab`, the `MSMask32/MSHFlxGd/MSFlxGrd/MSComCtl/MSChrt20/MSDatGrd/MSDatLst/Mci32/Msrdc20/MSAdoDc/MSBind/RichTx32/TabCtl32` CABs, `setupsp6.exe`, `sp698vbo.dll`, `readme.htm` "Service Pack 6 for Visual Basic 6.0…"). Install media `WIN7\` and `WINXP\` each carry `setup.exe`, `real lab system.CAB` (48 members) and `SETUP.LST` (39-file specification), per `12-install-media.md` and `Version_Identity_and_Scope.md §3.1`.

`RL_SETUP.exe` (23,060,480 B, reported VB6 `v19.00 'RealLab CD'`) is the application installer. Its reported 22.95 MiB embedded high-entropy payload was characterised but not decomposed (`Version_Identity_and_Scope.md §5.3`) — contents UNKNOWN.

## 6. Remote-access tools — FLAGGED THIRD-PARTY, NOT part of the target logic

`version_map.json` explicitly groups these as "Top-level 2019 CD utilities" alongside `RL_SETUP.exe` and `RealLab_Show_Info.exe` (rationale field). None is imported or referenced by the application's PE or VB object structure in any evidence examined.

| Tool | Evidence | Version |
|---|---|---|
| `AnyDesk.exe` | `version_map.json` → product_name `AnyDesk`; `v1.605-package-manifest.csv` path `RL_CD2019Access\AnyDesk.exe` | `6.0.7.0` |
| `TeamViewerQS.exe` | `version_map.json`; manifest path `RL_CD2019Access\TeamViewerQS.exe` | not recorded |
| `Supremo.exe` | `version_map.json` → product_name `Supremo Remote Control`; manifest path `RL_CD2019Access\Supremo.exe` | `4.1.3.2348` |

They are remote-support tooling shipped on the CD. Their presence is a **deployment/security observation**, not evidence that the target application contains remote-access logic. No runtime behaviour was observed.

## 7. Dependency-class summary

| Class | Items |
|---|---|
| DIRECT (import) | `MSVBVM60.DLL` only (231 functions in 1.605) |
| TRANSITIVE | Win32/OLE APIs via `DllFunctionCall`; OLE automation DLLs and type libs; MDAC/Jet; VB6 SP6 CAB chain |
| RUNTIME-ONLY (late-bound COM) | the OCX/OCX-like and DataReport/grid controls in §2, `unrar.dll`, `NOTEX.dll`, script designers |
| CONFIG-SELECTED | Jet `.mdb` vs SQL Server `SQLOLEDB.1`/`MSDataShape.1` via `.udl` / `BASE.ini` literals |
| THIRD-PARTY | Bullzip, Ghostscript Lite, Xpdf Lite, PDF Power Tool, .NET 4 redistributable, AnyDesk, TeamViewerQS, Supremo, `xcopy.exe`, `acmsetup.exe` |
| UNKNOWN | names of the 25 declared external components; nature of `VIDAS`; contents of the `RL_SETUP.exe` embedded payload |
