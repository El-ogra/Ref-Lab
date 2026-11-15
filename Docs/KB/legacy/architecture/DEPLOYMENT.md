# Legacy Deployment Architecture — V-ROOT-1.00 and V-CD-2019

**Purpose:** Curated deployment reference for the legacy systems.
**Scope:** Database engines, installer, licensing, paths, prerequisites, installation media, defects.
**Audience:** Developers, deployers, architects.
**Version applicability:** V2 ONLY for installer/media; V1 had no installer in the artefact set.

---

## 1. Database engines supported (V2)

Three engines, selected at install time via `BASE.ini` `Databasetype=X…X`:

| Engine | Evidence |
|---|---|
| Microsoft Access (Jet 4) | `BASE.ini` → `Databasetype=XMSACCESSX`; `Patients.mdb`; `Branch_Data.mdb` |
| Microsoft SQL Server Express 2005 | `Setup.bat`/`Setup4.bat` → `SQLEXPR.EXE INSTANCENAME=MSSQLSERVER`; `data_connection.udl` → `SQLOLEDB.1` |
| MySQL | `Setup5.bat` → MySQL Connector ODBC 3.51 |

**Confirmed (previously hypothesised):** now a VERIFIED FACT from the install scripts themselves.

**Evidence:** external: 05-deployment.md; external: 08-deployment-installer.md.

## 2. SQL Server catalogues (V2)

- `patients` — application catalogue (`D:\Lab_data\Patients.mdf`, attached via `Attach_Patients.bat`).
- `Interface` — separate database, belongs to a separately installed "RealLab Interface" product (`D:\Lab_data\Interface.mdf`, attached via `Attach_Interface.bat`).
- `VIDAS` — third attached database (`D:\Lab_data\Attach_VIDAS.bat`); never structurally characterised (V2-Q-01).

Server name default: `reallabserver` (customisable via `servername`).

## 3. Installer — `RL_SETUP.exe` (V2)

| Attribute | Value |
|---|---|
| Technology | Visual Basic 6.0 SP6 (single import `MSVBVM60.DLL`, 161 functions; linker 6.0) |
| Size | 23,060,480 bytes |
| PE sections | `.text` 22,953,984 B · `.data` 4,096 B · `.rsrc` 98,304 B |
| `.text` entropy | 7.39 — compressed payload inside code (no overlay) |
| Build date | 2020-09-01 10:59:33 |
| Version | 19.00 (`InternalName: RL_SETUP`, `CompanyName: RealLab`, `ProductName: RealLab CD`) |
| Objects | 57 |
| Methods | 599 |
| Standard archive signatures | none (no ZIP/CAB/RAR/7z/Inno/NSIS) — custom VB6 container |
| Embedded payload | 22.95 MiB at entropy 7.39 (characterised, not decomposed) |

Object structure (installer is a multi-step wizard, English + Arabic in parallel):
- Main processor: `Form1` (43), `Form2` (10), `frm_start` (2).
- System steps: `Frm_rls1`…`Frm_rls8` (5–17 methods) + Arabic twins `Frm_rlsAr1`…`Frm_rlsAr8`.
- Interaction steps: `Frm_rli1`…`Frm_rli7` + Arabic twins.
- SQL Server steps: `Frm_mssql1`…`Frm_mssql4` + Arabic twins.
- Repair: `Frm_repair0`…`Frm_repair5` (17 methods).
- Help: `Frm_help1`…`Frm_help4`.
- Modules: `FontInstaller` (38), `RegistryInteraction` (7), `Module1` (14), `modFonts` (12), `boisais` (16).
- Classes: `aaaClass` (15), `BiosData` (8).

**Evidence:** external: 08-deployment-installer.md; external: 1605-phase2-findings.md (NEW-7).

## 4. Database engine selection (installer)

The installer presents two options:
- **Microsoft Access database** → `Microsoft.JET.OLEDB.4.0` → `D:\real lab system\bin\Patients.mdb`
- **Microsoft SQL Server database** → `MS SQL Server 2005` → `D:\Lab_data\Patients.mdf`

**★ Critical documented constraint:** "If you convert from one type to another you will not be able to
see the stored data on the first database type, But they remain stored on the HDD." — conversion
does **not** migrate data.

**Evidence:** external: 08-deployment-installer.md §2.

## 5. Installation paths

- Program path: `C:\Program Files\real lab system` (`SETUP.LST` `DefaultDir`).
- Data path: `D:\Lab_data` (`Setup.bat` `cd\Lab_data`; `BASE.ini` `loca=E:\Lab_data`).
- These are **separate roots**, and the data path varies by installation.

Two installation targets on the CD: `WIN7` (48 members) and `WINXP` (48 members). 32 members are
byte-identical; 16 differ (entire Jet 4 engine differs between targets, plus VB6 runtime, two
ActiveX controls, one TypeLib).

**Evidence:** `Docs/Complete_Reverse_Engineering_Reference_V2.md` §2.6–2.7.

## 6. Installation scripts (`Setup*.bat`)

| Script | Installs |
|---|---|
| `Setup.bat` | Main installer: Windows Installer 3.1 → MSXML 6.0 → .NET 2.0 → SQL Server Express 2005 → OSQL attach (`Attaching.sql`) → `Attach_database.bat` (requires `D:\Lab_data`) |
| `Setup1.bat` | Windows Installer 3.1 |
| `Setup2.bat` | MSXML 6.0 |
| `Setup3.bat` | .NET Framework 2.0 |
| `Setup4.bat` | SQL Server Express 2005 |
| `Setup5.bat` | MySQL Connector ODBC 3.51 |
| `SetupPZ.bat` | .NET Framework 4 → Bullzip PDF Printer |
| `System\PDF\SetupP.bat` | Bullzip PDF Printer only (redundant, runs before .NET) |
| `System\PDF\SetupFN4.bat` | .NET 4 (invoked by nobody; relative path missing `System/PDF/` prefix) |

**Three documented installation defects:**
1. `RL_SETUP.exe` calls `System\PDF\SetupP.bat` (Bullzip only) **before** `System\SetupPZ.bat` (.NET 4 then Bullzip) — Bullzip installed before its .NET 4 dependency exists.
2. `SetupFN4.bat` installs .NET 4 but is invoked by nobody; its relative path is missing the `System/PDF/` prefix.
3. `setup.exe` launched with `/sc:\setup.log` and **without** `/wait`.

**Evidence:** external: 05-deployment.md; external: 13-pdf-print-architecture.md §7.4.

## 7. Licensing (V2)

Hardware-bound licence: `Hardware\Description\System` (WMI fingerprint), `Select * from Win32_Processor`.
The licence is bound to **HDD + motherboard + CPU**. `RealLab_Show_Info.exe` (v19.00) displays the
system serial. EULA: "2010-2020 by RealLab, all rights reserved." Scope covers both V1 and V2.

**★ Design implication:** any modern replacement must explicitly state its licensing model (or
declare it is not hardware-bound).

**Evidence:** external: 08-deployment-installer.md §4.

## 8. `SETUP.LST` — installation specification (V2)

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

| Destination | Files | Contents |
|---|---:|---|
| `$(WinSysPathSysFile)` | 22 | `OLEAUT32.DLL`, `OLEPRO32.DLL`, `ASYCFILT.DLL`, `VB5DB.DLL` + 12 Jet 4 files |
| `$(WinSysPath)` | 16 | 19 ActiveX controls + `MSDBRPTR.DLL`, `MSSTDFMT.DLL`, `MSBIND.DLL` |
| `$(Font)` | 5 | Laboratory fonts: `ais lab.ttf`, `ais symbol.ttf`, `MCS Taybah.ttf`, `WBCW_UPCEAN.ttf`, `WBCWS_UPCEAN.ttf` |
| `$(MSDAOPath)` | 1 | `dao360.dll` |
| **Total** | **39** | |

Actions: `$(DLLSelfRegister)` × 24 · `$(TLBRegister)` × 1 (`stdole2.tlb`) · `$(Shared)` × 31.

**Evidence:** `Docs/Complete_Reverse_Engineering_Reference_V2.md` §2.6.

## 9. Branch model (V2)

- Master laboratory + reception branches.
- SQL Server installed once in the laboratory; database on the master computer only.
- Branches may connect through the internet.
- `Branch_Data.mdb` (49 schema-only tables, 0 rows) seeds a new branch's profile tables.
- `Patient.branche` (operator attribute) + `SUBSTRING(patientid,9,1)` (branch in identifier).
- `grouptable` / `labbranch` / `loggroup` support multi-branch.
- Stock-take may span master + linked branches.

**Evidence:** external: 08-deployment-installer.md §5; `Docs/Complete_Reverse_Engineering_Reference_V2.md` §3.5, §6.4.

## 10. Missing installers (V2)

Five large Microsoft installers referenced by deployment scripts are absent from the inventoried tree:
`SQLEXPR.EXE`, `msxml6_x86.msi`, `dotnetfx.exe`, `WI-KB893803-v2-x86.exe`,
`mysql-connector-odbc-3.51.30-win32.msi`. (ISS-33; closed by BATCH-10.)

## 11. Version applicability

- Multi-engine deployment, installer, licensing, installation media, branch model, `Branch_Data.mdb`: **V2 ONLY**.
- V1 had no installer in its artefact set; V1 deployment is inferred from the manual (client/server over shared DB).

## 12. Known limitations

- `VIDAS` database never structurally characterised (RL-V2-Q-01).
- `win32_processor` table purpose unknown (RL-V2-Q-02) — likely WMI licensing probe.
- `Attaching.sql` and `Attach_database.bat` not in the inventory (created at install time or inside CAB).
- `RealLab support.exe` not in inventory (first-party secondary executable).
- Embedded 22.95 MiB payload in `RL_SETUP.exe` not decomposed.

## 13. Related documents

- `Docs/KB/legacy/architecture/OVERVIEW.md` — architecture.
- `Docs/KB/legacy/architecture/CONFIGURATION.md` — configuration files.
- `Docs/KB/legacy/architecture/PDF-PRINTING.md` — PDF/printing.
- `Docs/KB/legacy/data/BRANCHES.md` — branch model.
- `Docs/KB/legacy/data/MODEL.md` — data model.
- Original source evidence: external: 05-deployment.md, external: 08-deployment-installer.md, external: 11-deployment-payload-2019.md, `Docs/Complete_Reverse_Engineering_Reference_V2.md` §2, §5.
- `Database documentation.md`, `Documenting the Executable File.md` (root).
