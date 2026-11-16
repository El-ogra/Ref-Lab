# Deployment Architecture — REAL LAB SYSTEM v1.605

**Scope.** Installation medium, bootstrap chain, target layout and version
comparison for V-CD-2019 v1.605, reconstructed from static evidence. No installer,
batch file or executable was run; no database was contacted.

**Confidence legend.** VERIFIED / STRONGLY_SUPPORTED / INFERRED / UNVERIFIED /
CONFLICTED / UNKNOWN.

---

## 1. Installer identity (VERIFIED)

| Property | Value | Evidence |
| --- | --- | --- |
| File | `RL_SETUP.exe` | `rl-setup-analysis.md`, `08-deployment-installer.md` |
| Size | **23,060,480 B** | both reports; `12-install-media.md` tree |
| SHA-256 | `4a89958e1a32b9bae35faa96f36b78f5ac5a4b98db8d5797ac46b0b20b30cc2d` | `rl-setup-analysis.md` finding 1 |
| Version | FileVersion **19.00**, `InternalName: RL_SETUP`, `ProductName: RealLab CD`, CompanyName `RealLab` | `08-deployment-installer.md` §1 |
| Build timestamp | **2020-09-01 10:59:33** (PE TimeDateStamp `0x5F4E2995`) | `08-deployment-installer.md` §1; `rl-setup-analysis.md` §1 |
| Framework | VB6 SP6, linker 6.0, PE32, 3 sections; `.text` 22,953,984 B, entropy 7.39 | both reports |
| Content | 57 forms, 599 controls, 175 menus | `08-deployment-installer.md` §1 |
| Embedded archives | **0** CAB/ZIP/RAR/7z/Inno/NSIS containers found | `rl-setup-analysis.md` findings 3–5 |

`rl-setup-analysis.md` §4 reports **0** `[reportdata]` / `[setupx]` markers and **0**
`.rpt`/`.drp` members in the installer — report templates are not embedded in
RL_SETUP.exe. Caveat from that report: LZX/Quantum-compressed members cannot be
decoded by the pure-Python parser, so the negative is conclusive only for the tokens
and encodings searched.

RL_SETUP.exe is a **front-end/wizard**, not the application payload: it selects the
database type, runs prerequisite batches and launches the target-specific VB6 Setup
Toolkit bootstrap.

---

## 2. Wizard structure (VERIFIED from `08-deployment-installer.md` §1.1)

| Form group | Count | Purpose |
| --- | ---: | --- |
| `Form1`, `Form2`, `frm_start` | 3 | launcher and licence acceptance |
| `Frm_rls1..8` / `Frm_rlsAr1..8` | 8 | installation walkthrough, English and Arabic |
| `Frm_rli1..7` / `Frm_rliAr1..7` | 7 | installation type (English/Arabic) |
| `Frm_mssql1..4` / `Frm_mssqlAr1..4` | 4 | SQL Server configuration |
| `Frm_repair0..5` | 6 | "Repair Installed Software" |
| `Frm_help1..4` | 4 | help |
| `FontInstaller`, `RegistryInteraction`, `Module1`, `modFonts`, `boisais` | 5 | fonts, registry, utilities |

The wizard offers a choice of **Microsoft Access database** or **Microsoft SQL
Server database**, with the explicit warning that converting between them does not
carry stored data across (§2). The sampled connection strings target `patients`,
`Interface`, and a third `VIDAS` database; values are withheld here.

---

## 3. Bootstrap chain and `SETUP.LST` (VERIFIED)

`12-install-media.md` §2 quotes the VB6 Setup Toolkit descriptor:

```ini
[Bootstrap]
SetupTitle=Install
SetupText=Copying Files, please stand by.
CabFile=real lab system.CAB
Spawn=Setup1.exe
Uninstal=st6unst.exe
TmpDir=msftqws.pdw
Cabs=1

[Setup]
Title=RealLab
DefaultDir=$(ProgramFiles)\real lab system
AppExe=real lab system.exe
AppToUninstall=real lab system.exe
```

| Target | `SETUP.LST` size | Evidence |
| --- | ---: | --- |
| `RL_CD2019Access\WIN7\SETUP.LST` | 6,906 B | `BATCH-10.md` row 75 |
| `RL_CD2019Access\WINXP\SETUP.LST` | 6,901 B | `BATCH-10.md` row 78 |

Both targets ship `setup.exe` (bootstrap, 6.00.9782) and `real lab system.CAB` with
**48 members**. **39 files** are specified in `SETUP.LST`, distributed as
(`12-install-media.md` §2.1):

| Destination | Files | Examples |
| --- | ---: | --- |
| `$(WinSysPathSysFile)` | 22 | `OLEAUT32.DLL`, `OLEPRO32.DLL`, `ASYCFILT.DLL`, `VB5DB.DLL`, 12 Jet DLLs |
| `$(WinSysPath)` | 16 | the ActiveX controls, `MSDBRPTR.DLL`, `MSSTDFMT.DLL`, `MSBIND.DLL` |
| `$(Font)` | 5 | shipped font files |
| `$(MSDAOPath)` | 1 | `dao360.dll` |

Registration directives: **24 × `$(DLLSelfRegister)`**, 1 × `$(TLBRegister)`
(`STDOLE2.TLB`), 31 × `$(Shared)` (`12-install-media.md` §2.2). `DefaultDir` is
`$(ProgramFiles)\real lab system` while the data tree is on `D:\` — **a deliberate
two-root layout** (CONFLICTED in practice with `BASE.ini` `loca=E:\Lab_data`).

---

## 4. Prerequisite batch sequence (VERIFIED commands, `08-deployment-installer.md` §3.1)

```
System\SETUP1.bat                 Windows Installer 3.1
System\SETUP2.bat                 MSXML 6.0
System\SETUP3.bat                 .NET Framework 2.0
System\SETUP4.bat                 SQL Server Express 2005
System\SETUP5.bat                 MySQL Connector/ODBC 3.51
System\SetupPZ.bat                .NET Framework 4.0 + Bullzip PDF Printer
System\PDF\SetupP.bat             PDF stack
XCOPY                             application files
WIN7\setup.exe   /sc:\setup.log   Windows 7 target
WINXP\setup.exe  /sc:\setup.log   Windows XP target
D:\Lab_data\Attach_Patients.bat   attach "patients" MDF
D:\Lab_data\Attach_Interface.bat  attach "Interface" MDF
D:\Lab_data\Attach_VIDAS.bat      attach "VIDAS" MDF
```

The `Attach_Interface.bat` and `Attach_VIDAS.bat` names are therefore **part of the
install chain**, not optional operator tools. The target databases and their
connection descriptors are given in `08-deployment-installer.md` §2.1.

---

## 5. Missing prerequisites (VERIFIED absence in inventoried tree)

`12-install-media.md` §7 and `Version_Identity_and_Scope.md` §158 list **five large
Microsoft installers** referenced by `Setup*.bat` but absent from the staged tree:

1. `System\MSSQL_Server_Express_2005\SQLEXPR.EXE`
2. `System\Microsoft_xml6.0\msxml6_x86.msi`
3. `System\Microsoft.NET_Framework2.0\dotnetfx.exe`
4. `System\Microsoft_Windows_Installer_3.1\WI-KB893803-v2-x86.exe`
5. `System\mysql-connector-odbc-3.51.30-win32.msi`

Additionally absent: `MDAC_TYP\MDAC_TYP.EXE` from the staged tree (§6 — though the
file is present in the local read-only evidence directory, 8,124,720 B) and
`D:\Lab_data\Attaching.sql` / `Attach_database.bat` (§7). `Documentation_Quality_Report.md`
§224 states that whether these were shipped on a second disc is **not established**.

`12-install-media.md` §7 conclusion: **28 of the 39 `SETUP.LST` files** are present
in the CABs; the remainder are Microsoft *prerequisites* that must be obtained
separately. STRONGLY_SUPPORTED — a clean install from the inventoried tree alone
cannot succeed.

---

## 6. Installed tree (VERIFIED from `08-deployment-installer.md` §3, `12-install-media.md` §8)

```
D:\real lab system\                        (application root; D: imposed)
├── real lab system.exe                    (v1.605)
├── real lab system help.chm
├── bin\                                   Patients.mdb, Branch_Data.mdb,
│                                          RLUTool.exe, real lab timer.exe,
│                                          Patient Data Transport.exe
├── Data\  (and Data\data_emp\)
├── Cache\                                 Flash projector stubs
├── Images\
├── Fonts\
├── System\
│   ├── Setup*.bat (7)                     prerequisite chain
│   ├── PDF\                               .NET 4 + Bullzip + GS + Xpdf + PPT
│   ├── datareport_vh\                     VB6 SP6 redistributable (73 files)
│   ├── MDAC_TYP\                          MDAC installer (staged reference)
│   └── real lab system help.chm
├── WIN7\   setup.exe + SETUP.LST + real lab system.CAB
└── WINXP\  setup.exe + SETUP.LST + real lab system.CAB

D:\Lab_data\                               (data root)
└── Patients.mdf, Interface.mdf, VIDAS*, Attach_*.bat
```

`SETUP.LST` places the executable under `C:\Program Files\real lab system`, while
the wizard installs the application tree to `D:\real lab system\` — the two paths
are both evidenced; which one is authoritative at a given site is
CONFLICTED/installation-dependent.

---

## 7. Bare-tree v1.00 versus installer-based v1.605 (STRONGLY_SUPPORTED)

| Dimension | V-ROOT-1.00 (bare tree) | V-CD-2019 v1.605 (installer-based) |
| --- | --- | --- |
| Delivery | extracted file tree, no bootstrap | `RL_SETUP.exe` wizard + `WIN7`/`WINXP` CABs + `SETUP.LST` |
| Installation spec | none shipped | `SETUP.LST`, 39 files, 24 self-registrations |
| Database | Access `Patients.mdb` / `Branch_Data.mdb` | Access **or** SQL Server 2005 Express; `patients` + `Interface` + `VIDAS` |
| Report INI files | 223 cumulative `Data\*.ini` in the deliverable set | 146 in B3; 173 of 223 changed vs V1 (`ini-diff-summary.md`) |
| SQL inventory | earlier extraction (1,370 SELECT figure in prior report) | BATCH-04: 1,797 literals / 1,381 SELECT |
| DDL | fewer migration literals | 268 DDL-shaped literals including `branche`, `SecWord`, `grant13..17` |
| PDF stack | present as files | wrapped in `SetupPZ.bat` / `SetupP.bat` installers |
| Remote tools | `AnyDesk`/`Supremo`/`TeamViewerQS` in V2019 root (12 root files) | same; **not application logic** |

`cross-version-matrix.md` row 41 records 12 root-level files in V2019 versus 0 in
V1 for the remote-support group. `Complete_Reverse_Engineering_Reference_V2.md`
§1167 records "Installation specification: none shipped (v1.00) / `SETUP.LST`
(39 files) (v1.605)".

---

## 8. Licensing and privilege caveats (STRONGLY_SUPPORTED)

- EULA: "2010–2020 by RealLab"; hardware-bound licensing via WMI
  (`Win32_Processor`, disk, motherboard, CPU). The EULA states the vendor is not
  responsible for hardware loss and a new licence would be required
  (`08-deployment-installer.md` §4).
- `RLS_Setup.pdf` §42 instructs *Run as administrator* and manual font copying to
  `Control Panel\Fonts`; English (Canada) locale is imposed (§6).
- 24 self-registrations into System32/SysWOW64 require elevated rights.
- Three of the shipped PDF installers are unsigned/expired-certificate
  (`12-install-media.md` §10.1 context) — supply-chain caveat, not a logic defect.

---

## 9. Confidence and gaps

| Claim | Confidence |
| --- | ---: |
| RL_SETUP.exe identity, hash, size, build date | VERIFIED |
| Two CAB targets with 48 members and 39-file specs | VERIFIED |
| Batch chain and `Attach_*.bat` commands | VERIFIED (as script strings) |
| Five missing Microsoft installers | VERIFIED absence in staged tree |
| V1 bare-tree vs v1.605 installer-based split | STRONGLY_SUPPORTED |
| Version comparison counts | VERIFIED with differing denominators noted |
| Why the data root is `D:` (and `BASE.ini` says `E:`) | UNKNOWN |
| Whether a complete second installation disc existed | UNKNOWN |
| Actual install-time behaviour, registry writes, success rate | UNVERIFIED (never executed) |
| Member-by-member CAB binary identity of the 48 files | VERIFIED via `BATCH-10.md` hashes; contents not re-decompiled here |
