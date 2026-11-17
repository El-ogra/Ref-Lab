# Target-tree inventory — `F:\REAL_LAB_COPY`

**Method:** recursive enumeration of every file with SHA-256, byte size, extension and UTC modification time. Full machine-readable manifest: `evidence/target-tree-sha256-manifest.csv` (837 rows). Duplicate hashes are retained with their location context, never collapsed.

```
Get-ChildItem -LiteralPath "F:\REAL_LAB_COPY" -Recurse -File -Force |
  Get-FileHash -Algorithm SHA256 → evidence/target-tree-sha256-manifest.csv
```

---

## 1. Totals

| Measure | Value |
|---|---:|
| Files | **837** |
| Total bytes | **776,373,737** (740.3 MiB) |
| Distinct SHA-256 values | 671 |
| Hash-duplicate groups | **166** |
| Top-level roots | `DEMO\`, `real lab system\` |

---

## 2. By extension

| Extension | Count | Extension | Count | Extension | Count |
|---|---:|---|---:|---|---:|
| `.ini` | 464 | `.ocx` | 41 | `.tlb` | 4 |
| `.rtf` | 58 | `.cab` | 34 | `.INF` | 3 |
| `.exe` | 58 | `.DLL` | 33 | `.log` | 3 |
| `.jpg` | 28 | `.oca` | 20 | `.lst` | 3 |
| `.ttf` | 14 | `.gif` | 14 | `.db` | 3 |
| `.mdb` | 9 | `.bat` | 9 | `.pdf` | 3 |
| `.DEP` | 7 | `.udl` | 4 | `.chm` / `.htm` / `.txt` / `.dbg` / `.cdd` / `.wmv` | 2 each |
| `.stf` / `.tdf` / `.hlp` / `.cat` / `.ico` | 1 each | | | | |

---

## 3. Directory map

```
F:\REAL_LAB_COPY\
├─ DEMO\
│  └─ RLS_Demo2011\RL_CD2019Access\      0 files — empty skeleton
└─ real lab system\
   ├─ real lab system.exe                 59,150,336 B — EXCLUDED old v1.00 build
   ├─ Cache\                              001–012.exe, 007a.exe, 58 .rtf, .jpg/.gif/.wmv/.stf/.tdf
   └─ RL_CD2019Access\
      ├─ RL_SETUP.exe                      23,060,480 B  VB6 Setup Wizard v19
      ├─ AnyDesk.exe · Supremo.exe · TeamViewerQS.exe
      ├─ WIN7\    SETUP.LST + real lab system.CAB (7,211,180 B — 48 files)
      ├─ WINXP\   SETUP.LST + real lab system.CAB (7,057,945 B)
      ├─ bin\     5 MDB · calculator.exe · real lab timer.exe · RealLab E-Mail Sender.exe
      │           RLS ACCESS Database Tools.exe · RLS SQL Database Tools.exe · RLUTool.exe
      │           NOTEX.dll · REPORT.dll · ASNS.log · 4 .ini
      ├─ OCX\     34 control / runtime components
      ├─ Data\    223 report-template .ini   (+ Data\data_emp\ 5 employee .ini)
      └─ System\  real lab system.exe (TARGET) · Branch_Data.mdb
                  Setup.bat … SetupPZ.bat · PDF\ · MDAC_TYP\ · datareport_vh\
```

---

## 4. Component set

### 4.1 Application executables (target product)

| File | Bytes | SHA-256 (first 16) | Note |
|---|---:|---|---|
| `System\real lab system.exe` | 76,292,096 | `C54CE6C8C98A679E` | **TARGET** — PE32/i386/GUI, VB6 native, `RLS 1.605` |
| `System\Patient Data Transport.exe` | 1,294,336 | `4BDFB8FBC5C9DA7E` | patient-record transport helper |
| `bin\RealLab E-Mail Sender.exe` | 74,240 | `60530EDC429BA2C1` | .NET 4.0 auxiliary notification component |
| `bin\RLS ACCESS Database Tools.exe` | 1,531,904 | `E887A0BEC182792F` | Access administration utility |
| `bin\RLS SQL Database Tools.exe` | 114,688 | `D2EE697C96EEE042` | SQL Server administration utility |
| `bin\RLUTool.exe` | 126,976 | `1ABD85277E9C12A8` | utility |
| `bin\real lab timer.exe` | 135,168 | `37CC4831567F0374` | timer/shift helper |
| `bin\calculator.exe` | 114,688 | `37121ECB7C1E112B7` | calculator |
| `RealLab_Show_Info.exe` | 204,800 | `FF4A34D07DF89378` | about dialog |
| `System\xcopy.exe` | 30,720 | `F1F905558BD1B8FE` | OS tool used by the backup batch chain |

### 4.2 Installers and infrastructure

| File | Bytes | Note |
|---|---:|---|
| `RL_SETUP.exe` | 23,060,480 | VB6 Setup Wizard **v19**; `ProductName 'RealLab CD'`, `CompanyName 'RealLab'`, `FileDescription 'ais_soft_group@yahoo'`; payload is a proprietary compressed stream (no `MSCF` signature, no `SETUP.LST` string) |
| `WIN7\real lab system.CAB` | 7,211,180 | 48 files: VB6 runtime, Jet-4.0 stack, ADO/DAO, 12 OCX, 5 fonts, `SETUP1.EXE`, `ST6UNST.EXE`, `VB6STKIT.DLL` |
| `WINXP\real lab system.CAB` | 7,057,945 | WinXP variant of the same CAB |
| `System\datareport_vh\*.cab` (32) | 22,815 – 10,010,624 | VB6 SP6 redistributables: `VS6sp6B1.cab` (9,036,800), `VS6sp6B2.cab` (10,010,624), `VS6sp6B3.cab` (995,042), `vbrun60.cab` (989,512), `msvbvm60.cab` (697,692), plus 26 OCX-specific CABs |
| `System\PDF\Setup_BullzipPDFPrinter_11_1_0_2600_PRO_EXP.exe` | 11,075,872 | Bullzip PDF Printer 11.1 |
| `System\PDF\gslite.exe` | 15,499,441 | Ghostscript Lite 9.14.17 |
| `System\PDF\xpdfsetup.exe` | 1,081,385 | Xpdf Lite 3.3.4 |
| `System\PDF\pdfpowertool_setup.exe` | 2,287,977 | PDF Power Tool 4.0 |
| `System\PDF\dotNetFx40_Full_x86_x64.exe` | 50,449,456 | .NET 4.0 runtime for the e-mail helper |
| `System\MDAC_TYP\MDAC_TYP.EXE` | 8,124,720 | MDAC 2.8 / Jet 4.0 |
| `System\datareport_vh\setupsp6.exe` | 74,352 | VB6 SP6 runtime setup |

### 4.3 Third-party and remote-access (not target logic)

`AnyDesk.exe` (3,668,944) · `Supremo.exe` (7,861,016) · `TeamViewerQS.exe` (17,696,568) — remote-support tooling shipped alongside the product.

### 4.4 Flash projectors and cache

`Cache\001.exe` … `Cache\012.exe` plus `Cache\007a.exe` (13 files, 3.4 – 11.3 MB each) and 58 `.rtf` documents — assets of the Macromedia Flash Player 8 projector simulations. The `.rtf` files here are **not** report templates; the 228 report templates are the `.ini` files under `Data\`.

### 4.5 Control / runtime components (41 OCX + 33 DLL + 4 TLB)

| Component | Product name | Version |
|---|---|---|
| `OCX\ChamaleonButton.ocx` | ChamaleonButton | 1.00 |
| `OCX\Redsoul911XPCOmponents.ocx` | Redsoul911 XP Components 2 | 1.00 |
| `OCX\RgheedButtonControlV2.ocx` | RgheedButtonControlV2 | 1.00 |
| `OCX\XP_BUTTONS.ocx` | XPbutton | 1.00 |
| `bin\NOTEX.dll` | RLS Notes | 2.00 |
| `bin\REPORT.dll` | *(no version resource)* | **562 bytes — anomalously small; flagged as a stub** |
| `OCX\unrar.dll` | *(no version resource)* | 3.50 |
| `OCX\MSCOMCTL.OCX` / `MSCOMCT2.OCX` | MSCOMCTL / MSCommon Controls 2 | 6.01.9782 |
| `OCX\MSFLXGRD.OCX` | Microsoft FlexGrid Control | 6.01.9812 |
| `OCX\MSDATGRD.OCX` / `MSDATLST.OCX` | MS DataGrid / MS DataList | 6.01.9782 / 6.00.8418 |
| `OCX\MSADODC.OCX` / `MSBIND.DLL` / `MSSTDFMT.DLL` | ADO Data Control / MSBind / MSSTDFMT | 6.01.9782 |
| `OCX\DBGRID32.OCX` | DBGrid32 | 5.01.8104 |
| `OCX\MSCHRT20.OCX` | Microsoft Chart | 6.00.8804 |
| `OCX\COMDLG32.OCX` | CMDIALOG | 6.01.9782 |
| `OCX\TABCTL32.OCX` | TabCtl32 | 6.01.9782 |
| `OCX\RICHTX32.OCX` / `RICHED32.DLL` | RichText | 6.01.9782 / 4.00.993.4 |
| `OCX\MSCOMM32.OCX` | MSComm | 6.01.9813 |
| `OCX\MCI32.OCX` | MCI | 6.00.8418 |
| `OCX\MSWINSCK.OCX` | Microsoft Winsock Control | 6.00.8169 |
| `OCX\msvbvm60.dll` | Visual Basic | 6.00.9802 |
| `OCX\VB6STKIT.DLL` | Visual Basic for Windows | 6.00.8450 |
| `OCX\msado27.tlb` | Microsoft Data Access Components | 2.81.1132.0 |
| `OCX\STDOLE2.TLB` | Microsoft OLE 2 | 2.40.4275 |
| `OCX\COMCAT.DLL` / `ASYCFILT.DLL` / `OLEAUT32.DLL` / `OLEPRO32.DLL` | OLE / OLE Automation | 2.40.4275 / 5.0.4275 |
| `System\datareport_vh\mswless.ocx` | Microsoft Windowless Controls | 6.01.9782 |
| `System\datareport_vh\msscript.ocx` | Microsoft Script Control | 1.0.0.3715 |
| `System\datareport_vh\dispex.dll` / `jscript.dll` / `vbscript.dll` | MS Dispex / JScript 5.1 / VBScript 5.1 | — |

The five previously *unnamed* third-party components are therefore the four XP-style button packs plus `NOTEX.dll`; `REPORT.dll` is a 562-byte placeholder that carries no version resource.

### 4.6 Databases, configuration and templates

| File | Bytes | Role |
|---|---:|---|
| `bin\Patients.mdb` | 2,682,880 | main patient database (103 tables / 1,682 columns) |
| `bin\Patients_backup.mdb` | 2,146,304 | previous-generation backup (90 / 1,381) |
| `bin\Branch_Data.mdb` | 970,752 | branch mirror template (49 / 969, empty) |
| `System\Branch_Data.mdb` | 966,656 | second branch template (49 / 960) |
| `bin\data.mdb` | 266,240 | 3 tables (`history`, `patientinfo`, `wasl`), all empty |
| `Data\*.ini` | 223 | report-template definitions, `[reportdata] testtype=` binding |
| `Data\data_emp\*.ini` | 5 | employee/referral template variants |
| `bin\*.ini`, `System\*.ini` | 5 | runtime configuration |
| `*.udl` | 4 | OLE DB connection definitions (SQL Server / Jet) |
| `*.DEP` | 7 | module dependency descriptors |

---

## 5. Duplicate-hash groups (location context preserved)

The 166 duplicate groups are dominated by:

| Group | Members | Note |
|---|---|---|
| `datareport_vh\*.cab` | 32 CABs | VB6/OCX redistributables, each paired with the installed copy under `OCX\` |
| `OCX\` vs `datareport_vh\` OCX copies | 12 | identical binaries shipped twice (install source + runtime copy) |
| `MCI32.OCX` | 2 (`bin\`, `OCX\`) | identical |
| `RICHTX32.OCX` | 2 | identical (one is 675 bytes in `datareport_vh\` — an anomaly) |
| `WIN7` vs `WINXP` Setup media | 2 | different sizes, different content |

---

## 6. Deployment model (VERIFIED from `System\Setup.bat` and `WIN7\SETUP.LST`)

`Setup.bat` installs, in order: Windows Installer 3.1 → XML 6.0 → .NET Framework 2.0 → **Microsoft SQL Server Express 2005** (`INSTANCENAME=MSSQLSERVER`, `SECURITYMODE=SQL`, `SAPWD=<hard-coded value — redacted>`), and then attaches databases:

```
D:
cd \Lab_data
OSQL -S %COMPUTERNAME% -E /i Attaching.sql
start /wait Attach_database.bat
```

`WIN7\SETUP.LST` records `AppExe=real lab system.exe`, `DefaultDir=$(ProgramFiles)\real lab system`, `CabFile=real lab system.CAB`, `Spawn=Setup1.exe`, `Uninstal=st6unst.exe`, and **45 `File#=` entries** across `[Bootstrap Files]` (6), `[Setup1 Files]` (36) and the Jet-4.0 group (3).

**The `VIDAS` name enters only through this installation chain** — the prior deployment report records `D:\Lab_data\Attach_VIDAS.bat`, and `Attaching.sql` attaches the databases. The string `VIDAS` occurs **zero** times in the authentic application binary (ASCII and UTF-16LE both checked), so `VIDAS` is an installer-side database attachment and not an application database.
