# Runtime Requirements — REAL LAB SYSTEM v1.605

**Scope.** What the shipped artefacts require in order to install and run, derived
from static evidence. Nothing was installed or executed. Confidence labels:
VERIFIED / STRONGLY_SUPPORTED / INFERRED / UNVERIFIED / CONFLICTED / UNKNOWN.

---

## 1. Operating system targets (VERIFIED)

| Target | Evidence | Detail |
| --- | --- | --- |
| Windows 7 | `WIN7\setup.exe`, `WIN7\SETUP.LST` (6,906 B, SHA-256 in `BATCH-10.md` row 75), `WIN7\real lab system.CAB` | VB6 Setup Toolkit bootstrap; CAB has 48 members |
| Windows XP | `WINXP\setup.exe`, `WINXP\SETUP.LST` (6,901 B, `BATCH-10.md` row 78), `WINXP\real lab system.CAB` | same structure; 48 members |
| Jet version split | `database-and-data-access-analysis.md` §6 | WINXP Jet `4.0.9502.0` (SP2) vs WIN7 Jet `4.0.9756.0` (SP3) |

`08-deployment-installer.md` §3.1 shows both bootstrap commands:
`cmd.exe /C start /B WIN7\setup.exe /sc:\setup.log` and the `/wait` variant for
`WINXP\setup.exe`. 32-bit x86 target throughout (`dependency-inventory.json` →
`runtime_platform.target_os`). No Windows 8/10/11 target is evidenced.

---

## 2. Visual Basic 6 runtime (VERIFIED)

| Component | Evidence | Status |
| --- | --- | --- |
| `MSVBVM60.DLL`, runtime build **9782** | `vb6-objects.json` → `vb_header.runtime_build 9782`, `signature "VB5!"`; `dependency-inventory.json` → `runtime_dependencies[0]` | VERIFIED |
| VB6 SP6 redistributable | `BATCH-10.md` §17: `System\datareport_vh\` holds `msvbvm60.cab`, `vbrun60.cab`, `VS6sp6B1/B2/B3.cab`, `setupsp6.exe` (73 files) | VERIFIED |
| `VBA6.DLL` | `dependency-inventory.json` → `runtime_dependencies[1]`; component-name string in main executable | VERIFIED as string; role INFERRED |
| `VB6STKIT.DLL`, `VB5DB.DLL` | CAB member inventory (`12-install-media.md` §1) | VERIFIED |
| Support status | `dependency-inventory.json`: "out of support" | VERIFIED (prior assessment) |

`RL_SETUP.exe` itself is VB6 with linker 6.0 and 161 `MSVBVM60.DLL`-related strings
(`08-deployment-installer.md` §1). ASLR and DEP are disabled in both
`RL_SETUP.exe` (`DllCharacteristics 0x0000`, `rl-setup-analysis.md` §1) and the VB6
runtime model — an architectural property, not a tested exploit.

---

## 3. MDAC / ADO (VERIFIED)

| Component | Evidence |
| --- | --- |
| MDAC 2.8 TypeLib installer `System\MDAC_TYP\MDAC_TYP.EXE`, **8,124,720 B** | `12-install-media.md` §6; file present in `D:\Collective work` with same size |
| `msado27.tlb`, **81,920 B**, "Microsoft Data Access - ActiveX Data Objects 2.7 Type Library", version **2.81.1132.0** | `msado27.tlb.json` → `pe.version_info` |
| Providers `SQLOLEDB.1`, `MSDataShape(.1)`, `Microsoft.Jet.OLEDB.4.0` | `06-configuration.md` §§2–3; `08-deployment-installer.md` §2 |
| MDAC installer missing from inventoried tree | `12-install-media.md` §7: `MDAC_TYP.EXE` is **not present** in the staged set (referenced but absent) — status: missing |

**CONFLICTED (scope):** the MDAC executable is present in the local read-only
evidence directory (8,124,720 B) but the install-media report records it as absent
from the *staged 2019 CD tree*. Both statements are true in their own scope.

---

## 4. Database engines (STRONGLY_SUPPORTED)

| Option | Evidence |
| --- | --- |
| **Microsoft Access / Jet 4** — `msjet40.dll` (1,589,248 B), `msjter40.dll`, `msjint40.dll`, `msjtes40.dll`, `msrd2x40.dll`, `msrd3x40.dll`, `msrepl40.dll`, `vbajet32.dll`, `mswdat10.dll`, `mswstr10.dll`, `expsrv.dll`, `dao360.dll` | `12-install-media.md` §1; `BASE.ini` `Databasetype=XMSACCESSX` (`06-configuration.md` §1) |
| **SQL Server 2005 Express** — `Setup.bat` / `Setup4.bat` install with mixed-mode authentication, auto-start, SQL Browser enabled, network protocols enabled | `08-deployment-installer.md` §§2, 3.1; a cleartext default administrative password is embedded in the script — **value withheld** |
| SQL Server database `patients` plus `Interface` and `VIDAS` | `08-deployment-installer.md` §2.1; `Attach_*.bat` |
| MySQL Connector/ODBC 3.51 | `Setup5.bat`; installer absent from tree (`12-install-media.md` §7) |
| Runtime DDL migration | 268 DDL literals (`ddl-evidence__[analysis_BATCH-04_sql_v1605].txt`); client owns schema changes |

**Jet dialect discrepancy (CONFLICTED by evidence class):** the `.mdb` files are
described as Jet 3 / Access 97 format in `dependency-inventory.json` (framework
`Microsoft Access (Jet 3)`), while the install media ships Jet 4 DLLs, the task
brief states Jet 4, and the embedded DDL contains both Jet 3-style `MEMO`/
`AUTOINCREMENT` and SQL Server-style `varchar(MAX)`/`IDENTITY`. Best reading:
Jet 4 provider with legacy-format database files; UNVERIFIED definitively.

---

## 5. .NET Framework (VERIFIED)

| Component | Evidence |
| --- | --- |
| .NET Framework **2.0** prerequisite | `Setup3.bat` (`08-deployment-installer.md` §3.1); `dotnetfx.exe` installer absent from tree (`12-install-media.md` §7) |
| .NET Framework **4.0** (`dotNetFx40_Full_x86_x64.exe`, 48,111,808 B) | `SetupPZ.bat`; `12-install-media.md` §10.1; present in `D:\Collective work` |
| VB.NET 10.0 compiler / `Microsoft.VisualBasic` namespace | `dependency-inventory.json` → `framework_dependencies[4]` |
| `RealLab E-Mail Sender.exe` (74,240 B) targets net40; assembly refs include `System.Net.Mail`, `System.Data.SqlClient`, `System.Windows.Forms` | `dependency-inventory.json` → `framework_dependencies[3]`, `dotnet_assembly_references` |

The .NET 4.0 payload is required specifically for the **Bullzip PDF Printer** stub
(`SetupPZ.bat`, `SetupFN4.bat`), not for the VB6 client itself (INFERRED linkage,
STRONGLY_SUPPORTED by script co-location).

---

## 6. Printer and locale requirements (VERIFIED)

- Printer ports `LPT1:` for both report and barcode in `SETTING.ini`
  `[selectprinter]` (`06-configuration.md` §4).
- Per-report printer field `[setupx] reportprinterx` — predominantly
  `HP LaserJet 1100 (MS)` (138 / 146 B3 templates; `configuration-and-resources.md`
  §6). `printerselect` module has 11 methods (`objects-full__[…_v1605].txt`).
- **English (Canada) locale is imposed by the installer**: `08-deployment-installer.md`
  §6 step 4; `sShortDate = dd/MM/yyyy`, `iDate = 1` verified in
  `Credential-And-Fix-Report_2026-11-01_00-51.md` §3. The RTF cache is Arabic
  (`\ansicpg 1256`), so both an Arabic-capable font set and the imposed date format
  are required simultaneously.
- Fonts must be copied manually from `real lab system\Fonts` to
  `Control Panel\Fonts` per `RLS_Setup.pdf` §42 (`08-deployment-installer.md` §6),
  despite the installer containing a `FontInstaller` component.

---

## 7. Administrative rights and licensing assumptions (STRONGLY_SUPPORTED)

- `RLS_Setup.pdf` instructs *Run as administrator* for `RealLab_Show_Info`
  (`08-deployment-installer.md` §4, §6).
- `SETUP.LST` writes 22 files to `$(WinSysPathSysFile)` (System32/SysWOW64) and
  self-registers **24** OCX/DLL components with `$(DLLSelfRegister)`
  (`12-install-media.md` §§2.1–2.2) — administrator rights are required.
- Hardware fingerprinting: WMI `Hardware\Description\System`, `Win32_Processor`,
  hard disk, motherboard and CPU; EULA states the vendor is not responsible for
  hardware loss and a new licence would be required
  (`08-deployment-installer.md` §4). Reinstalling on changed hardware may therefore
  require a new licence (INFERRED consequence).
- Master/reception topology: SQL Server installed **once** on the master computer;
  reception and branches connect over the network/internet; RDP port `3389`
  referenced (`08-deployment-installer.md` §5).

---

## 8. Requirement matrix

| Requirement | Source | Confidence |
| --- | --- | ---: |
| 32-bit Windows XP or Windows 7 | `WIN7\`, `WINXP\` CAB targets | VERIFIED |
| VB6 SP6 runtime (`MSVBVM60.DLL` build 9782) | `vb6-objects.json`, `dependency-inventory.json` | VERIFIED |
| MDAC 2.8 / ADO 2.7 (`msado27.tlb` 2.81.1132.0) | `msado27.tlb.json` | VERIFIED |
| Jet 4 (or legacy Jet-format `.mdb` mirror) | install media + DDL | CONFLICTED by evidence class |
| SQL Server 2005 Express (optional back end) | `Setup.bat`, `Setup4.bat` | VERIFIED |
| .NET 2.0 and .NET 4.0 | `Setup3.bat`, `SetupPZ.bat` | VERIFIED |
| 24 self-registering ActiveX components | `SETUP.LST` | VERIFIED |
| LPT1-compatible or named printer, HP LaserJet 1100 driver | `SETTING.ini`, `Data\*.ini` | VERIFIED |
| English (Canada) locale, `dd/MM/yyyy` | installer PDF + fix report | VERIFIED |
| Administrator rights | `SETUP.LST`, setup PDF | STRONGLY_SUPPORTED |

---

## 9. What is NOT evidenced

- No Windows Server target, no 64-bit-native target, no Windows 8/10/11 support.
- No minimum CPU/RAM/disk figures are stated in any cited artefact (UNKNOWN).
- No proof that the five absent prerequisite installers were supplied on a second
  disc (`Documentation_Quality_Report.md` §224 — not established).
- No compatibility testing was performed; all requirements are static inferences
  from shipped scripts, manifests and binaries.
