# Executable Inventory — RealLab PE / executable artifacts

**Scope.** Every executable or PE artifact encountered in the evidence set, with detected type, version resource, role, and target-version attribution. All evidence paths below are under `D:\Collective work\` unless stated otherwise. SHA-256 values are taken from `C:\Users\LAP LINK\source\repos\Ref Lab\REAL_LAB_1605_REVERSE_ENGINEERING_OUTPUT\02_SOURCE_INVENTORY\SHA256_Manifest.csv` (columns `id,sha256,full`); the manifest `id` is given where the file exists on disk.

## 0. CRITICAL scope statement (VERIFIED)

The **76,292,096-byte version 1.605 binary is NOT present on disk** in `D:\Collective work`. Only its extracted analysis dumps exist: `real lab system.exe (2).json`, `pe-structure.json`, `vb6-structure.json`, `vb6-objects.json`, `pe-summary.csv`, `pe-resources.json`, `version-resources.json`, and the `statements__[analysis_BATCH-04_sql_v1605].csv` string-derived SQL inventory. The 59,150,336-byte `real lab system.exe` that **is** on disk is version **1.00** (V-ROOT-1.00), the comparative baseline (SHA-256 `12828D0A…`; manifest `A0667`). Every finding in this document about 1.605 is derived from a named dump, never from direct analysis of the 1.605 file bytes. This analysis did not execute any binary.

## 1. Shipped application components

| Name | Size (bytes) | SHA-256 / manifest id | Detected type | Version resource | Role | Target version |
|---|---:|---|---|---|---|---|
| `real lab system.exe` | 76,292,096 | `C54CE6C8C98A679EFC5AB485BC4B313B452026D6A40F9BDA36ECA7EA18DF93C0` — **no manifest row; file absent from work dir** | PE32 native, i386 (0x14c), GUI, VB6 (`MSVBVM60.DLL` only import, 231 functions) — `real lab system.exe (2).json`, `pe-structure.json` | FileVersion/ProductVersion `1.605`; FileDescription `RLS 1.605`; ProductName `real lab system`; CompanyName `RealLab`; Comments `ais_soft_group@yahoo` — `real lab system.exe (2).json → pe.version_info` | Main clinical LIS application | **V-CD-2019 / 1.605 (target)** |
| `real lab system.exe` | 59,150,336 | `12828D0A0EEA121C0A6E08BDAF5F369D4D1D4D2B4F8B65320AC308FBC01FD885` — `A0667` | PE32 native, i386, GUI, VB6 (186 `MSVBVM60.DLL` imports) — `real lab system.exe.json` | `1.00`; FileDescription `ais_soft_group@yahoo`; ProductName `real lab system`; CompanyName `RealLab` — `real lab system.exe.json → pe.version_info` | Comparative baseline executable | **V-ROOT-1.00 (baseline, not target)** |
| `Patient Data Transport.exe` | 1,294,336 | `4BDFB8FBC5C9DA7EB40C7395802E3FF8A66D8F6692BE2D84165CB24BCB69F024` — `A0582` | PE32 native, i386, GUI, VB6 (84 `MSVBVM60.DLL` imports, 10 resources) — `Patient Data Transport.exe.json`, `pe-summary.csv` | FileVersion `1.00`; ProductName `Patient Data Transport`; description `ais group` — `pe-summary.csv` | Patient-data transport utility shipped in `RL_CD2019Access\System\` | **V-CD-2019** (path in `pe-summary.csv`) |
| `real lab Image Gallery.exe` | 10,219,798 | `91556B519915391FBECCD90A61F207240C73DDEF02A8051A1BD9F8C6A8007E5A` — `A0663` | PE32 native, i386, GUI, VB6 (42 `MSVBVM60.DLL` imports) — `real lab Image Gallery.exe.json` | FileVersion `1.00` — `real lab Image Gallery.exe.json` | Auxiliary image-viewer executable (baseline tree) | V-ROOT-1.00 |
| `real lab timer.exe` | 135,168 | `37CC4831567F0374697CA5496F77C4FD820F9719AAD9FD2349C297917DFC216D` — `A0670` | Type UNVERIFIED — no PE dump in evidence set | UNKNOWN | Auxiliary timer utility (name only) | UNKNOWN |
| `RealLab E-Mail Sender.exe` | 74,240 | `60530EDC429BA2C158FB20F3F0C34CC6CC38B138699635168A239B359F5DA8FC` — `A0672` | Type UNVERIFIED — no PE dump | UNKNOWN | Mail-sender utility (name only) | UNKNOWN |
| `RealLab_Show_Info.exe` | 204,800 | `FF4A34D07DF89378F55B48DFA09BE424174D725C61329A19D0A492EF5D1DEDD8` — `A0678` | Type UNVERIFIED — no PE dump | `6.00` (reported in `version_map.json`) | CD-level information utility | V-CD-2019 |
| `RLS ACCESS Database Tools.exe` | 1,531,904 | `E887A0BEC182792FE2CB83AD07DDDB50FA608C95F7F36AE45D87998E27E0CFA9` — `A0759` | Type UNVERIFIED — no PE dump | UNKNOWN | Access/ Jet database utility (name only) | V-CD-2019 (inferred from name) |
| `RLS SQL Database Tools.exe` | 114,688 | `D2EE697C96EEE0426B23A131F2391AE373211A6A2CD3B997E2FBC683AE515AFD` — `A0760` | Type UNVERIFIED — no PE dump | UNKNOWN | SQL Server database utility (name only) | V-CD-2019 (inferred from name) |
| `RLUTool.exe` | 126,976 | `1ABD85277E9C12A827163380FC7DF2A3AE12AA0908A610BC8B8D3DBAFAD63D20` — `A0766` | Type UNVERIFIED — no PE dump | UNKNOWN | Named utility | UNKNOWN |
| `calculator.exe` | 114,688 | `37121ECB7C1E112B735BD21B0D0FE3E526352ECB98C434C5F40E6A2A582380CDD` — `A0215` | Type UNVERIFIED — no PE dump | UNKNOWN | Calculator utility | UNKNOWN |
| `RL_SETUP.exe` | 23,060,480 | `4A89958E1A32B9BAE35FAA96F36B78F5AC5A4B98DB8D5797AC46B0B20B30CC2D` — `A0755` | VB6-compiled installer (reported `v19.00 'RealLab CD'`, 57 objects / 599 methods in `Version_Identity_and_Scope.md §3.1`; not re-verified from a dump here) | `Version_Identity_and_Scope.md` reports build 2020-09-01 | Application installer for the 1.605 CD | V-CD-2019 |

## 2. Microsoft / third-party installer engines present in the media set

These are deployment mechanisms, **not target application logic** (confidence VERIFIED for classification where a PE dump or version resource exists).

| Name | Size | SHA-256 / id | Detected type | Version resource | Role |
|---|---:|---|---|---|---|
| `setup.exe` | 139,776 | `3D0041832E8B6F5B95CB33D286C24C53CCC9341549589AE8822C6084E8D2AA5C` — `A0802` | Microsoft setup stub (`12-install-media.md` states WIN7/WINXP `setup.exe` are Microsoft files) | UNVERIFIED | Install-media bootstrap |
| `acmsetup.exe` | 390,144 | `A7CE3F9F…17FA7E31` — `A0096` | PE32 i386, 8 sections, GUI, 9 import DLLs / 372 functions, heavy `MSSETUP.dll` (152 functions) — `pe-analysis.json` | `{}` in dump | Microsoft InstallShield/ACME setup engine |
| `setupsp6.exe` | 74,352 | `E40A42FAFEB4D353F54AA766714577A14956C063450058CB70D48B41F5739063` — `A0809` | PE reported but `pe_error: "no PE signature"` in `pe-analysis.json` — self-extracting stub, type UNVERIFIED | UNKNOWN | VB6 SP6 redistributable launcher |
| `xcopy.exe` | 30,720 | `F1F905558BD1B8FED7683B1DE778E616B95C713027AD79E4921147D22FF1198A` — `A0967` | PE32 i386, GUI/CUI 6 DLLs / 163 functions — `pe-summary.csv` (`IMAGE_SUBSYSTEM_WINDOWS_CUI`) | `5.1.2600.5512 (xpsp.080413-2111)`; Company `Microsoft Corporation`; description `Extended Copy Utility` | Microsoft copy utility used by `Setup*.bat` |

## 3. Flash training simulators (shipped content, not target logic)

Thirteen distinct names `001.exe`–`012.exe` plus `007a.exe` appear twice in `v1.605-package-manifest.csv` (under both `Cache\` and `RL_CD2019Access\Cache\`, byte-identical SHAs within each pair). All are in the manifest's **B3 — "Report Engine & Visual Assets"** batch. Representative rows (full set in `v1.605-package-manifest.csv` and manifest ids in `SHA256_Manifest.csv`):

| Name | Size (bytes) | SHA-256 / id (work-dir copy) |
|---|---:|---|
| `001.exe` | 7,001,308 | `2BE865A4…E7E31` — `A0008` |
| `007a.exe` | 6,920,620 | `F409242A…CEC3` — `A0017` |
| `012.exe` | 4,733,541 | `3BDE3DDB…2E9C` — `A0028` |

Detected internal type of these simulators is **UNVERIFIED** (no PE dump for them in the evidence set); prior reports describe them as Flash simulators. They are training content, not part of the main application's code.

## 4. Third-party / open-source installers and remote-access tools — FLAGGED, NOT target logic

| Name | Size | SHA-256 / id | Detected type / version | Why flagged |
|---|---:|---|---|---|
| `AnyDesk.exe` | 3,668,944 | `4DE898C1…130C97` — `A0109` | Product `AnyDesk`, version `6.0.7.0` (`version_map.json`) | Third-party **remote-access** tool; `version_map.json` lists it among "Top-level 2019 CD utilities", not application logic |
| `TeamViewerQS.exe` | 17,696,568 | `7FA526AA…24D03` — `A0876` | Product `TeamViewerQS` (`version_map.json`); no PE dump | Third-party **remote-access** tool |
| `Supremo.exe` | 7,861,016 | `86567544…F80F6C45` — `A0866` | Product `Supremo Remote Control`, version `4.1.3.2348` (`version_map.json`) | Third-party **remote-access** tool |
| `dotNetFx40_Full_x86_x64.exe` | 50,449,456 | `65E06425…38659F` — `A0348` | PE32 x86, 5 sections, `10.0.21009.0`, Company `Microsoft Corporation`, Product `Microsoft .NET Framework`, Description **`Box Stub`**, Microsoft signature, 50,258,992-byte overlay (`pe-envelope.csv`) | Microsoft redistributable installed for the Bullzip PDF printer (`12-install-media.md`); **not target code** — see `Managed_Assembly_Analysis.md` |
| `MDAC_TYP.EXE` | 8,124,720 | `DC14F871…13782` — `A0527` | Microsoft CAB self-extracting MDAC installer; version resource `InternalName Wextract`, file version `4.71.1015.0` (`version-resources.json`, `12-install-media.md`) | Microsoft data-access redistributable |
| `Setup_BullzipPDFPrinter_11_1_0_2600_PRO_EXP.exe` | 11,075,872 | `FB892EC3…33310F4` — `A0800` | PE32 i386, 8 sections, ASLR+DEP, Authenticode present (5,296 B), `11.1.0.2600`, Company `Bullzip` (`pe-envelope.csv`, `pe-analysis.json`) | Third-party PDF printer driver |
| `gslite.exe` | 15,499,441 | `FED78051…88DF9` — `A0452` | `GPL Ghostscript Lite 9.14.17`, Company `Free Distribution` (`pe-envelope.csv`) | Open-source Ghostscript redistribution |
| `xpdfsetup.exe` | 1,081,385 | `7C55C27E…F13` — `A0971` | `Xpdf Lite 3.3.4`, Company `Free Distributions` (`pe-envelope.csv`) | Open-source PDF renderer redistribution |
| `pdfpowertool_setup.exe` | 2,287,977 | `56892E7D…73A3` — `A0597` | `PDF Power Tool 4.0.0.11`, Company `pdfpowertool.com` (`pe-envelope.csv`) | Third-party PDF utility |

## 5. Native runtime / ActiveX components (dependency artifacts, not target code)

Listed with version evidence from `version-resources.json` (each entry has `path`, `size_bytes`, `strings`, `fixed.file_version`):

- VB6 runtime: `msvbvm60.dll` 1,384,479 B, file version `6.0.98.2` / ProductVersion `6.00.9802` — the main executable's only direct import.
- Report/data controls: `MSDBRPTR.DLL` `6.1.97.82` (DataReport engine), `MSDATGRD.OCX` `6.1.97.82`, `MSDATLST.OCX` `6.0.84.18`, `DBGRID32.OCX` `5.1.81.4`, `MSFLXGRD.OCX` `6.1.98.12`.
- Common controls: `MSCOMCTL.OCX` `6.1.97.82`, `MSCOMCT2.OCX` `6.1.97.82`, `TABCTL32.OCX` `6.1.97.82`, `COMDLG32.OCX` `6.1.97.82`, `MSCHRT20.OCX` `6.0.88.4`, `mswless.ocx` `6.1.97.82`.
- Data/binding: `MSADODC.OCX` `6.1.97.82`, `MSBIND.DLL` `6.1.97.82`, `msado27.tlb` `2.81.1132.0`, `STDOLE2.TLB` `2.40.4275.1`, `OLEAUT32.DLL` `2.40.4275.1`, `OLEPRO32.DLL` `5.0.4275.1`, `ASYCFILT.DLL` `2.40.4275.1`.
- Text/media: `RICHTX32.OCX` `6.1.97.82`, `RICHED32.DLL` `4.0.993.4`, `MSCOMM32.OCX` `6.1.98.13`, `MCI32.OCX` `6.0.84.18`, `MSSTDFMT.DLL` `6.1.97.82`.
- Scripting for HTML designer: `msscript.ocx` `1.0.0.3715`, `dispex.dll` `5.0.0.3715`, `jscript.dll` `5.1.0.8513`, `vbscript.dll` `5.1.0.8513` (all under `RL_CD2019Access/System/datareport_vh/`).
- Installer/self-registration: `VB6STKIT.DLL` `6.0.84.50`, `selfreg.dll` `8.0.0.3214`, `mssetup.dll` `3.1.0.1918`, `sp698vbo.dll` `1.20.0.2119`.
- Compression: `unrar.dll` `3.50.3.111` (original filename `Unrar.dll`).
- RealLab-authored auxiliary DLL: `RL_CD2019Access/bin/NOTEX.dll` 45,056 B, CompanyName `RealLab`, ProductVersion `2.00`, InternalName `NOTEX`, OriginalFilename `NOTEX.exe` (`version-resources.json`).
- `.oca` type-library caches with no version resource: `ChamaleonButton.oca`, `RgheedButtonControlV2.oca`, `vkUserControlsXP.oca`, `XP_BUTTONS.oca`, `Redsoul911XPCOmponents.oca`, `MSCOMCT2.oca`, `MSCOMCTL.oca`, `RICHTX32.oca`, `MSWINSCK.oca` (same file names also under `RL_CD2019Access/OCX/` in `version-resources.json`).

## 6. Confidence notes

- VERIFIED: identities, sizes, version strings and import counts of the two main executables and of `Patient Data Transport.exe` / `real lab Image Gallery.exe` (dedicated JSON dumps exist).
- STRONGLY_SUPPORTED: roles and target-version attribution where a named manifest path or `version_map.json` entry exists.
- UNKNOWN: internal PE type and version resources of executables that have only a SHA-manifest row (`real lab timer.exe`, `RealLab E-Mail Sender.exe`, `RLUTool.exe`, `calculator.exe`, the Flash simulators, `RLS ACCESS/SQL Database Tools.exe`, `RealLab_Show_Info.exe`).
- CONFLICTED: build date of the 1.605 executable — raw PE timestamp is 2020-07-22 (see `Binary_Architecture.md`), while `Version_Identity_and_Scope.md §3.1` asserts 2013-12-27 for both versions. Use the raw dump value.
