# Installation and Deployment Reference — RealLab CD 2019 v1.605

**Consolidated:** 2026-10-31 (UTC) · **Static analysis only.** Nothing was
installed, registered or executed.

---

## 1. The media

| Artefact | Bytes | Digest |
| --- | ---: | --- |
| `v1.605-B4.zip` | 63,839,512 | `7aff65e1…f5e4a6c` |
| `real lab system.zip` | 10,490,454 | `17a266e0…734bb6` |
| `v1.605-B3.zip` | 3,523,212 | `2fb942c3…ee3a7b` |

**Every archive is Windows-authored and stores its paths with backslashes.**
A naive `unzip` on Linux flattens the tree into single files with literal
backslashes in their names. The project's safe extractor
(`executable-analysis/tools/extract_safe_b4.py`) normalises the separators,
rejects traversal, absolute paths, symlinks and duplicate destinations,
enforces extraction limits, refuses to clobber pre-existing files, and
verifies every CRC32.

---

## 2. What the installation contains

| Directory | Files | Content |
| --- | ---: | --- |
| `System/` | 9 | the seven prerequisite scripts, `ERROR.log` (0 bytes), the user manual |
| `System/PDF/` | 3 | the Bullzip PDF printer installer and two scripts |
| `System/datareport_vh/` | 46 | the VS6 Service Pack 6 redistributable — 30 cabinets, the registration driver, the SP6 launcher and its scripts, the SP6 licence and documentation, two MS `.dbg` symbol files, and two orphaned Avira descriptors |
| `WIN7/`, `WINXP/` | 4 | the Setup Wizard manifest and the 48-file cabinet, per OS |
| `bin/` | 14 | the PCR report graphics and the nine stationery bands |
| `Data/` | 109 | the report-layout templates |
| `Cache/` | 29 | the clinical reference documents |
| `Fonts/` | 7 | the product fonts |
| `AUTORUN.INF` | 1 | `shellexecute=RL_SETUP.exe` · `ICON=ark.ico` · `Label="RealLab CD"` |

---

## 3. The script chain

```
Setup1.bat  → Windows Installer 3.1   (WI-KB893803-v2-x86.exe — a 2004 hotfix)
Setup2.bat  → MSXML 6.0               (msxml6_x86.msi)
Setup3.bat  → .NET Framework 2.0      (dotnetfx.exe)
Setup4.bat  → SQL Server Express 2005 (SQLEXPR.EXE …)
Setup5.bat  → MySQL Connector/ODBC 3.51.30
SetupPZ.bat → .NET Framework 4         (dotNetFx40_Full_x86_x64.exe)
          → Bullzip PDF Printer        (…PRO_EXP.exe /VERYSILENT /NORESTART)
PDF/SetupFN4.bat → .NET Framework 4 only
PDF/SetupP.bat   → Bullzip PDF Printer only
Setup.bat   → 1 + 2 + 3 + 4, then the database attach
```

**The database attach, verbatim:**

```bat
D:
cd\Lab_data
OSQL -S %COMPUTERNAME% -E /i Attaching.sql
start /wait Attach_database.bat
```

Three consequences: the clinical databases ship **detached**; the path is
hardcoded to the **D:** drive; and the attach itself uses **Windows integrated
authentication** (`-E`) — no SQL credential is needed *for the attach*, which
is why the `sa` password installed by `Setup4.bat` matters later.

**None of the referenced payloads is in any analysed portion.** Their
provenance could not be assessed.

---

## 4. The installer manifest — what goes where

`SETUP.LST` is the classic VB6 Setup Wizard manifest:

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

**This is the first statement of the product's executable name and install
location**, and it agrees exactly with the executable's own VERSION resource
(`OriginalFilename = "real lab system.exe"`).

### 4.1 The `FileN` grammar

```
FileN=@<source>,<destination>,<registration>,<shared>,<timestamp>,<size>,<file version>
```

| Field | Values observed |
| --- | --- |
| destination | `$(WinSysPath)`, `$(WinSysPathSysFile)`, `$(MSDAOPath)`, `$(Font)`, `$(ProgramFiles)\real lab system` |
| registration | `$(DLLSelfRegister)`, `$(TLBRegister)`, or blank |
| shared | `$(Shared)` or blank |
| file version | the component's own version, e.g. `6.1.97.82`, `4.0.9502.0`, `6.0.84.50` |

### 4.2 The deployment plan

| Destination | Count | Components |
| --- | ---: | --- |
| `$(WinSysPath)` | 17 | `MSCOMM32.OCX`, `MSFLXGRD.OCX`, `mscomctl.ocx`, `RICHTX32.OCX`, `MCI32.OCX`, `MSCHRT20.OCX`, `COMDLG32.OCX`, `MSDATLST.OCX`, `TABCTL32.OCX`, `MSCOMCT2.OCX`, `MSADODC.OCX`, `MSDATGRD.OCX`, `DBGRID32.OCX`, `XP_BUTTONS.ocx`, `Redsoul911XPCOmponents.ocx`, `RgheedButtonControlV2.ocx` — **all `$(DLLSelfRegister)`** — plus `msado27.tlb` |
| `$(WinSysPathSysFile)` | 14 | `VB6STKIT.DLL`, `COMCAT.DLL`, `STDOLE2.TLB`, `ASYCFILT.DLL`, `OLEPRO32.DLL`, `OLEAUT32.DLL`, `RICHED32.DLL`, the Jet 4.0 engine (8), `expsrv.dll`, `vbajet32.dll` |
| `$(MSDAOPath)` | 1 | `dao360.dll` — `$(DLLSelfRegister)` |
| `$(Font)` | 5 | `MCS Taybah.ttf`, `ais symbol.ttf`, `ais lab.ttf`, `WBCWS_UPCEAN.ttf`, `WBCW_UPCEAN.ttf` — all `$(Shared)` |

**Every ActiveX control is installed into the 32-bit system directory and
self-registers.** This is the mechanism behind B2's FND-B2-005, and it is why
B2's component inventory and B4's install manifest describe the same set.

**The five fonts are installed into the shared font directory.** That is
B3's FND-B3-008 — the renamed Microsoft Tahoma goes into every user's profile.

---

## 5. The two OS variants

| | WINXP | WIN7 |
| --- | --- | --- |
| `msjet40.dll` | 1,516,568 B — Jet 4.0 **SP2** (4.0.9502.0) | 1,589,248 B — Jet 4.0 **SP3** (4.0.9756.0) |
| `msado27.tlb` | 81,920 B — **ADO 2.81** (2.81.1132.0) | 98,304 B — 6.1.7601.17514 |
| `dao360.dll` | 554,008 B — 3.60.9512.0 | 610,304 B — 3.60.9756.0 |
| `msvbvm60.dll` | 1,384,479 B | 1,386,496 B |
| `MSCOMM32.OCX` | 6.1.98.13 | 6.0.81.69 |
| `MSFLXGRD.OCX` | 6.1.98.12 | 6.0.81.69 |

**Jet 4.0 SP3 is the first Jet release with a 64-bit build.** The two
installers differ **only** in the data-access stack.

**B2's catalogued `msado27.tlb` version 2.81.1132.0 is exactly the WINXP
variant**, so the environment B2 characterised was the XP deployment.

---

## 6. `datareport_vh/` — assembled from three batches

| Batch | Contribution |
| --- | --- |
| **B2** | the engine: `jscript.dll`, `vbscript.dll`, `msscript.ocx`, `mswless.ocx`, `mswless.dep`, `selfreg.dll`, `sp698vbo.dll`, `dispex.dll`, `mssetup.dll`, and a **675-byte** `RICHTX32.OCX` stub |
| **B3** | the interface: 14 GIF assets — an HTML authoring toolbar and HTML element markers |
| **B4** | the redistributables: 30 VS6 SP6 cabinets, `acmsetup.exe`, `setupsp6.exe`, `sp698vbo.inf`/`.stf`, `eula.txt`, `redist.txt`, `readme.htm`, `toc.htm`, `setup.tdf`, `msvbvm60.dbg`, `oleaut32.dbg`, `avipbb.inf`, `avipbb.cat` |

**It is a VS6 Service Pack 6 redistribution target.** The identification is
documentary, not inferential: `readme.htm` is titled *"Service Pack 6 for
Visual Basic 6.0, Visual C++ 6.0 and Visual Source Safe 6.0"*, `eula.txt` is
the Service Pack EULA addendum, `redist.txt` is the SP6 file manifest, and
`sp698vbo.inf` lists the SP6 cabinets.

**B2's FND-B2-011 is resolved here**: the 675-byte `RICHTX32.OCX` in this
folder is a marker; the **real 212,240-byte control** is in the product's OCX
register and is what the installer deploys.

### 6.1 The two unexpected members

| File | What it is | Finding |
| --- | --- | --- |
| `avipbb.inf` | an **Avira Operations GmbH & Co. KG kernel-driver** descriptor for `avipbb.sys` — driver version 07/31/2013, catalog 058-213-132 | U-021 |
| `avipbb.cat` | **not a Windows catalog** — an Avira *engine manifest* listing 14 antivirus modules (2012–2013 versions), a VDF file and a libinfo file, followed by a signature blob | U-021 |

**No Avira binary exists in any analysed portion.**

---

## 7. The third-party PDF printer

| | |
| --- | --- |
| Package | Inno Setup 5.5.7, 11,075,872 bytes |
| Product | Bullzip PDF Printer **11.1.0.2600**, built 2016-01-15 |
| Edition | **Professional EVALUATION** (from the file name and the Inno metadata) |
| Vendor | **BioPDF ApS**, Bohusvej 6, Ålsgård, **Denmark** |
| Signature | Authenticode — signer *BioPDF ApS*, COMODO RSA Code Signing CA, timestamped by a COMODO SHA-1 signer. **The code-signing certificate expired 2020-10-11** |
| Hardening | ASLR and DEP/NX enabled — the only properly hardened binary in the batch |
| Payload | 110 files: the port monitor `bzpdf.dll`, the .NET writer assemblies for .NET 2.0 and 4.0, `msxml6.dll`, the MFC OCX set, three `BULLZIP.PPD` PostScript driver files, `syswow64/msscript.ocx`, and `app/Macros/Examples/base.vbs` |

**`syswow64/msscript.ocx` and the VBScript macro are the security-relevant
members** — a script host and an automation surface added to the clinical
workstation by an unrelated component (U-009).

---

## 8. The deployment's risks

| Risk | Severity | Evidence |
| --- | --- | --- |
| 16 ActiveX controls self-registered into the system directory | Medium | `SETUP.LST` `$(DLLSelfRegister)` ×16 |
| Local copies of Windows system DLLs in a system directory | Medium | `$(WinSysPathSysFile)` |
| A renamed Microsoft typeface installed machine-wide | Medium | `$(Font)` `$(Shared)`; B3's font provenance |
| The registration driver has no ASLR, no DEP and no signature | Medium | `acmsetup.exe`, `DllCharacteristics 0x0000` |
| The installer runs unverified payloads by absolute path | Medium | the nine scripts; no signature or hash check |
| A fixed `D:` drive letter for the database attach | Medium | `Setup.bat` |
| An evaluation-edition commercial printer installed silently | Medium | the file name, the Inno metadata, the expired certificate |
| A 16-bit Windows 3.1 launcher in the required path | Low | `setupsp6.exe` (NE) |
| Orphaned Avira driver descriptors | Low | `avipbb.inf` / `avipbb.cat` |
| A 1996 Win-3.1 help file in the deployment | Low | `acmsetup.hlp` |
| Microsoft debug symbol files in the clinical installation | Low | `msvbvm60.dbg`, `oleaut32.dbg` |

---

## 9. The deployment prerequisites the vendor states

From B4's recovered manual, topic 2:

1. The **server's** IP or computer name is configured in a server-settings
   window; the **database user name and password** and the **database name**
   are fixed and must not be changed.
2. The host's **Regional Options → Standards and formats must be English
   (Canada)**.
3. *"It is preferable not to connect the internet to the server machine, to
   preserve the operating system for as long as possible and to preserve the
   confidentiality of the data from the possibility of intrusion."*

**Point 3 is the vendor's own acknowledgement of the internet-facing risk**,
written for a product that ships an SMS gateway call over plain HTTP.

---

## 10. Deployment unknowns

| Question | Status |
| --- | --- |
| The provenance, versions and signatures of the nine referenced payloads | **Open** — not in any portion |
| `RL_SETUP.exe` (the AUTORUN target) and `ark.ico` | **Not present** in any analysed portion |
| Whether the MySQL connector is ever used | **Open** — no connection string, DSN or code path |
| The deployment path on a 64-bit host | **Open** — the 16-bit launcher cannot run without 16-bit support |
