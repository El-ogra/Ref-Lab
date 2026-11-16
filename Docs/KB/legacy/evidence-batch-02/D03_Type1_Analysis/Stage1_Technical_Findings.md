# Stage 1 — Evidence Type 1 Analysis: Program & Resource Binary Evidence

**Product:** RealLab v1.605 ("RealLab CD 2019") — Clinical Laboratory Information System, by *ais group corporation*
**Evidence type:** 1 of 3 — compiled program, runtime/OCX dependencies, help, report templates, configuration, assets
**Files:** 121 · **Bytes:** 92,745,469 (dominated by `rtf-analysis.json`, 80.4 MB)
**Method:** static analysis only; nothing in the batch was executed. Full per-file disposition in `file_disposition.csv`; machine-readable findings in `Stage1_Technical_Findings.json`.

---

## 1. What this evidence family contains

Type 1 covers every artefact that is *program-and-resource* evidence: PE analyses of the main executable and its sibling executables, the OCX/DLL/TLB control set, the VB6 project/object table, extracted strings, the CHM help file, the RTF report-template corpus, the 146 per-test INI files, fonts/media assets, and the component/dependency catalogues. It does **not** contain the database schema or data (Type 2) nor the prior investigators' consolidated findings (Type 3).

Two builds of the main executable are present. They are **different files**, not duplicates:

| Artefact | Evidence ID | Size (bytes) | PE timestamp | SHA-256 (head) | Build |
|---|---|---:|---|---|---|
| `real lab system.exe.json` | EVD-000313 | 59,150,336 | 1388267701 (2013-12-28) | `12828D0A…` | **V-ROOT-1.00** (origin) |
| `real lab system.exe (2).json` | EVD-000312 | 76,292,096 | 1595435962 (2020-07-22) | `C54CE6C8…` | **v1.605** (target) |
| `pe-structure.json` | EVD-000284 | 76,292,096 | 1595435962 | `C54CE6C8…` | v1.605 (same as above) |

`real lab system help.chm.json` and `…help.chm (2).json` are **byte-identical** re-analyses (same SHA-256), as are several other `… (2).json` files.

## 2. Executable identity

- **Format:** PE32 i386, `IMAGE_SUBSYSTEM_WINDOWS_GUI`, 3 sections (`.text`, `.data`, `.rsrc`), image base `0x400000`. (EVD-000313, EVD-000284)
- **VB6 signature:** imports **only** `MSVBVM60.DLL` (231 named imports); VBHeader signature `VB5!`, runtime build **9782**, revision 10, LCID 1033. This is a Visual Basic 6 native application. (EVD-000313, EVD-000437, EVD-000246)
- **Resources:** 10 `ICON` entries (incl. a 69,012-byte 256×256 icon and a 67,624-byte icon), one `GROUP_ICON`, one `VERSION` resource. `DllCharacteristics=0x0000` → **no ASLR, no DEP/NX**; SEH present, CFG absent. (EVD-000283, EVD-000284)
- **Sibling executables analysed:** `real lab Image Gallery.exe` (10,219,798 B, PE32, entropy 7.95), `Patient Data Transport.exe` (1,294,336 B, PE32), `xcopy.exe` (30,720 B). All VB6 (import `MSVBVM60.DLL`).

## 3. VB6 project structure (FIND-T1-003, FIND-T1-004)

- Project name `Real_lab_system`, description `real lab system`.
- **212 objects** (210 named), **8,026 methods** declared.
- VBHeader `form_count = 198`. A second classifier resolves the 210 named objects as **132 form + 66 datareport + 9 module + 3 class**. Since `132 + 66 = 198`, the two counts reconcile exactly: the project ships 132 forms plus 66 `DataReport` designers.
- **Naming convention = per-domain result-entry + per-domain print report.** Examples: `Resultliver` / `Reportliver`, `Resultkidney`, `Resulturine` / `Reporturine`, `Resultsugar` / `ReportSugar`, `Resultlipids`, `resultculutre` / `reportculutres`, `ReportFatora`, `Reporttotal`, `Reporthistory`, `Reportinstrument`, `Reportsemen`, `statisReport`, `Reportqc`, `Reportcross`.
- **Core modules:** `labmain` (71 methods), `patientmain` (84), `labdb` and `labdb1` (data access), `tools`, `testmain`, `addpatient`, `barcode`, `phone`, `moneypaid`, `testrange`, `workpaper`, `labfunction`, `printerselect`, `BCWFonts`, `saveIOtemp`, `otherfun`, `mInputbox`; classes `aaaClass`, `CRect`, `BiosData`.

## 4. Component & dependency model (FIND-T1-005, FIND-T1-006, FIND-T1-012)

- **Runtime:** `MSVBVM60.DLL` (build 9782) and `VBA6.DLL`, both out of support.
- **Data access:** ADO 2.7 (`msado27.tlb`, `MSADODC.OCX`), OLE DB `SQLOLEDB.1`. **SQL Server is the system of record** (`Initial Catalog=Patients`); **Access/Jet 3 MDBs are an offline/branch mirror**.
- **Third-party VB6 controls (external_component_count = 25):** `ChamaleonButton.ocx` (144 typelib members), `RgheedButtonControlV2.ocx` (100), `XP_BUTTONS.ocx` (114), `vkUserControlsXP.ocx` (524), `Redsoul911XPCOmponents.ocx` (410), plus Microsoft `MSCOMCTL`, `MSCOMCT2`, `COMDLG32`, `RICHTX32`, `MSCOMM32`, `MSFLXGRD`, `MSCHRT20`, `MSDATGRD`, `MSDATLST`, `MSDBRPT`, and vendor `NOTEX.dll` / `REPORT.dll`. (EVD-000136, EVD-000097, EVD-000418)
- **.NET side:** `RealLab E-Mail Sender.exe` is a VB.NET / .NET 4.0 component (31 assembly refs incl. `System.Net.Mail`, `System.Data.SqlClient`).
- **External services (evidence-supported, not runtime-verified):** SMTP relay; `smsmisr.com` SMS HTTP API (cleartext, credentials in query string, exe offset `0x27d79a`); serial-port analysers via `MSCOMM32.OCX`; an `AUTOMATIC HEMATOLOGY ANALYZER … BF6500` catalog in a UDL.

## 5. Resources, help and configuration

- **Help (EVD-000076/077):** `real lab system help.chm` — HTML Help Workshop 4.74.8702, 17 topics, 79 images, Arabic RTL. Copyright `ais group corporation 2010`.
- **Report configuration (EVD-000193):** 146 `Data\*.ini` files; sections `reportdata`/`setupx` (+ `FORM`/`report`); report main titles MICROBIOLOGY, MOLECULAR BIOLOGY, HEMATOLOGY, IMMUNOLOGY, ENDOCRINOLOGY, CYTOPATHOLOGY, SEROLOGY, BIOCHEMISTRY REPORT; default printer `HP LaserJet 1100 (MS)`; test types `r`/`pcr`/`pnt`.
- **Report templates (EVD-000398, EVD-000350):** a corpus of **228 cached RTF report templates**; `rtf-info-metadata.json` describes the per-template structure.
- **Fonts/media (EVD-000024, EVD-000023):** 47 assets — barcode fonts `FREE3OF9`, `WBCW_UPCEAN`, `WBCWS_UPCEAN`; Arabic fonts `MCS Taybah`, `ais lab`, `ais symbol`; `Digiface`; and training video `RLS_Learn.wmv` (85,279,236 B ASF/WMV).

## 6. Findings (summary)

| ID | Finding | Class | Confidence |
|---|---|---|---|
| FIND-T1-001 | VB6 native 32-bit GUI app | 1 | High |
| FIND-T1-002 | Two distinct builds (V1.00 2013 vs v1.605 2020) | 1 | High |
| FIND-T1-003 | 212 objects / 8,026 methods / 132 forms + 66 DataReports | 1 | High |
| FIND-T1-004 | Per-domain Result*/Report* organisation | 2 | High |
| FIND-T1-005 | Dual DB: SQL Server primary, Access/Jet mirror | 2 | High |
| FIND-T1-006 | Third-party UI control packs + MS OCX set | 1 | High |
| FIND-T1-007 | SMTP / SMS / serial analyser integrations | 2 | Medium |
| FIND-T1-008 | Arabic CHM help (17 topics) | 1 | High |
| FIND-T1-009 | 146 INI + 228 RTF report templates | 1 | High |
| FIND-T1-010 | Barcode + Arabic fonts shipped | 1 | High |
| FIND-T1-011 | 10 icons + VERSION; no ASLR/DEP | 1 | High |
| FIND-T1-012 | Component/dependency map (B3 slice) | 2 | Medium |
| FIND-T1-013 | RLS_Learn.wmv training video | 1 | High |

## 7. Limitations

No binary was executed; method bodies were not decompiled (names/counts come from the VB6 object table); p-code vs native was not resolved; the exact VB6 service-pack of the build toolchain is inferred from `runtime_build 9782` (SP6) rather than proven.