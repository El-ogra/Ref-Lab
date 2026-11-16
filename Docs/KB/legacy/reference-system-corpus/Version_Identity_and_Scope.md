# Version Identity and Scope

**Document ID:** RL-DOC-001
**Package:** RealLab_Final_Documentation
**Scope:** Establishes the exact identity, investigation scope, boundaries and inter-relationship of the two RealLab systems documented in this package.

---

## 1. Purpose

This package documents **two distinct products** that share the RealLab name. They are
not a "before and after" of a single continuously-evolved application; they are two
separate deployment artifacts that differ in executable version, object model, schema,
deployment model, documentation set and integration surface.

This document establishes, from accumulated investigation evidence, which product each
documented finding belongs to. Every other document in this package attributes its
findings using the identifiers defined here.

---

## 2. Version 1 — V-ROOT-1.00

### 2.1 Exact identity

| Attribute | Value | Evidence |
|---|---|---|
| **Documentation identifier** | `V-ROOT-1.00` | Assigned by this package from the investigation's inventory classification (`inventory/version_map.json`) |
| **Application name** | `real lab system` | PE original filename / directory name |
| **Main executable** | `real lab system.exe` | BATCH-01, BATCH-10 |
| **File version** | **1.00** | PE `VS_VERSIONINFO.FileVersion` — `analysis/BATCH-01/pe-summary.csv` |
| **File size** | **59,150,336 bytes (56.41 MiB)** | `analysis/BATCH-01/pe-summary.csv` |
| **PE format** | PE32, 3 sections, linker 6.0 | `analysis/BATCH-01/pe-reports/` |
| **Build timestamp** | **2013-12-27** | PE `TimeDateStamp` — `analysis/CUMULATIVE/01-architecture.md` |
| **Runtime** | `MSVBVM60.DLL`, 186 imported functions | `analysis/BATCH-01/vb6/main/pe.csv` |
| **Second executable** | `real lab Image Gallery.exe`, 10.22 MB, build 2010-06-30 | `analysis/CUMULATIVE/01-architecture.md` |
| **Product description** | "real lab system version 1.0" | Inside the official help file, topic `001.htm` |
| **Vendor identity in artefacts** | `ais group` (e.g. `NOTEX.dll` CompanyName "ais group") | `inventory/version_meta.json` |
| **Vendor contact (from `IDCARD.INI`)** | RealLab / AIS, El-Mansoura, Egypt — `http://www.RealLabeg.com` | `analysis/CUMULATIVE/06-configuration.md` |

### 2.2 Object model

| Attribute | Value |
|---|---:|
| VB6 objects | **169** |
| — Forms | 118 (9,281 controls extracted) |
| — DataReport | 45 |
| — Module | 5 (`labdb`, `labdb1`, `labfunction`, `printerselect`, `BCWFonts`) |
| — Class | 1 |
| Declared methods | **6,528** |
| String literals | **104,997** |
| SQL statements | **1,337** |
| Tables referenced in SQL | 100 |
| Embedded DDL statements | **0** |

### 2.3 Data stores

| File | Tables | Columns | Rows | Role |
|---|---:|---:|---:|---|
| `bin\Patients.mdb` | **93** | **1,527** | 3,123 | Primary database — patients, results, tests |
| `bin\Patients_backup.mdb` | 90 | 1,381 | 2,786 | Backup copy of the primary database |
| `bin\Branch_Data.mdb` | **49** | — | **0** | Schema-only template used to seed new branches |
| `bin\data.mdb` | 3 | — | — | `history` (dynamic 20-column), `patientinfo` (3 cols), `wasl` |

### 2.4 Investigation scope

| Dimension | Coverage |
|---|---|
| Artefacts verified | **364** files across BATCH-01, BATCH-02, BATCH-03 (plus shared runtime artefacts recovered in later batches) |
| Executable | Fully analysed — PE envelope, 169 objects, 6,528 methods, literals, SQL |
| Database | Fully extracted — `Patients.mdb` schema and sample data, `Branch_Data.mdb`, `data.mdb` |
| Knowledge layer | Fully extracted — `testtable` (609 tests), `normalrange` (852 ranges), `constanttable`, `testunits` (69), `converts` (167), `abbreviations` (650), `subgrouptable` (46) |
| Configuration | Fully enumerated — `BASE.ini`, `SETTING.ini`, `IDCARD.INI`, `REPORT.dll`, `NOTE.ini`, 223 `Data\*.ini` files |
| Presentation | Fully enumerated — 13 Flash simulators, 29 RTF method sheets |
| Documentation | Fully extracted — `real lab system help.chm`, 15 topics |
| Printing | `printerselect` module and Win32 spooler API usage identified; PDF export absent |

### 2.5 Known boundaries of Version 1

- **No PDF export capability.** Zero occurrences of `PDF` or `Bullzip` in the executable's string literal table (`analysis/BATCH-01/vb6/main/literals.tsv`). Printing is limited to physical/Windows printers via `OpenPrinterA` / `SetPrinterA` / `GetPrinterA` / `ClosePrinter`.
- **No SQL Server deployment path evidenced in the shipped configuration.** SQL Server appears only as an alternative connectivity option via `SQLOLEDB.1` / `MSDataShape.1` connection strings; the installer that selects the engine ships with Version 2 only.
- **No branch master/reception model.** The `Branch_Data.mdb` schema-only template exists in Version 1, but `Patient.branche`, `Labbranch` and `SUBSTRING(patientid,9,1)` are Version 2 additions.
- **No embedded schema migration.** Zero DDL statements in the executable.
- **No attendance log, no statistics module, no work-sheet module, no SMS subsystem.** These are Version 2 additions.

---

## 3. Version 2 — V-CD-2019

### 3.1 Exact identity

| Attribute | Value | Evidence |
|---|---|---|
| **Documentation identifier** | `V-CD-2019` | Assigned by this package from the investigation's inventory classification (`inventory/version_map.json`) |
| **Application name** | `real lab system` | Directory `RL_CD2019Access` |
| **Main executable** | `real lab system.exe` | BATCH-04 |
| **File version** | **1.605** | PE `VS_VERSIONINFO.FileVersion` — `analysis/BATCH-04/pe-summary.csv` |
| **Product description** | **`RLS 1.605`** | PE `VS_VERSIONINFO.FileDescription` |
| **Company name** | **`RealLab`** | PE `VS_VERSIONINFO.CompanyName` |
| **File size** | **76,292,096 bytes (72.76 MiB)** | `analysis/BATCH-04/pe-summary.csv` |
| **PE format** | PE32, 3 sections, linker 6.0 | `analysis/BATCH-04/pe-reports/` |
| **Build timestamp** | **2013-12-27** (same build date as Version 1) | PE `TimeDateStamp` |
| **Runtime** | `MSVBVM60.DLL`, 231 imported functions | `analysis/BATCH-04/vb6/v1605/` |
| **Installer** | `RL_SETUP.exe`, VB6 v19.00, build **2020-09-01**, 23,060,480 bytes, 57 objects, 599 methods | BATCH-05 |
| **Installer install media** | `WIN7\` and `WINXP\` targets, each with `setup.exe` + `real lab system.CAB` (48 members) + `SETUP.LST` | BATCH-10 |
| **Official learning guide** | `RLS_Learn.pdf` — "Real Lab System Guide Book", RealLab Co., **212 pages**, 7 modules, 510 headings | BATCH-05 |
| **Setup guide** | `RLS_Setup.pdf` — 33 pages | BATCH-05 |

> **★ Note on the build timestamp.** Both executables carry the PE timestamp
> **2013-12-27**. Version 2's *product* is materially newer — its installer is dated
> 2020-09-01, its learning guide 2017-04-26, and its payload files 2019–2020 — but
> the executable itself was not recompiled after the shared 2013-12-27 build base.
> **Do not use the PE timestamp as a version discriminator.**

### 3.2 Object model

| Attribute | Value | Δ vs V1 |
|---|---:|---:|
| VB6 objects | **210** | +41 |
| — Forms | 132 | +14 |
| — DataReport | 66 | +21 |
| — Module | 9 | +4 |
| — Class | 3 | +2 |
| Declared methods | **8,026** | +1,498 |
| String literals | **270,166** | +165,169 |
| SQL statements | **1,797** | +460 |
| Tables referenced in SQL | 117 | +17 |
| Imported runtime functions | 231 | +45 |
| **Embedded DDL statements** | **268** (27 `CREATE TABLE` + 241 `ALTER TABLE`) | +268 |

### 3.3 Data stores

| Store | Tables | Columns | Notes |
|---|---:|---:|---|
| `Patients.mdb` (shipped 2019) | **103** | **1,682** | Primary Access database; **+10 tables, +155 columns** vs Version 1 |
| `Branch_Data.mdb` (2019) | 49 | — | Schema-only template, 0 rows; **9 columns removed** from `patienttest` vs the Version 1 template |
| SQL Server catalogue `patients` | — | — | One of two SQL catalogues |
| SQL Server catalogue `Interface` | — | — | Second SQL catalogue, attached by `D:\Lab_data\Attach_Interface.bat` |
| `VIDAS` | — | — | Third attached database; **referenced but not structurally characterised** |

### 3.4 Investigation scope

| Dimension | Coverage |
|---|---|
| Artefacts verified | **473** files across BATCH-04 → BATCH-11 |
| Executable | Fully analysed — PE envelope, 210 objects, 8,026 methods, 270,166 literals, 1,797 SQL, 268 DDL |
| Database | Shipped `Patients.mdb` extracted — 103 tables / 1,682 columns; schema migration diffed against shipped state |
| Knowledge layer | Same knowledge tables as Version 1 plus new 2019 tables (`IgEallergy`, `Helicobacter`, `insulin`, `LBC`, `FMF`, …) |
| Configuration | `BASE.ini`, `SETTING.ini`, `IDCARD.INI`, `REPORT.dll`, `NOTE.ini`, 223 `Data\*.ini`, `data_connection*.udl` |
| Installation media | `SETUP.LST` (39-file specification), both CAB targets fully extracted and hashed, VB6 SP6 redistributable (73 files) |
| Print / PDF | **Fully characterised** — Bullzip PDF Printer 11.1 driver, ‹.NET Framework 4.0› box stub, Ghostscript Lite, Xpdf Lite, PDF Power Tool |
| Documentation | `RLS_Learn.pdf` (212 pp.) and `RLS_Setup.pdf` (33 pp.) extracted; official CHM confirmed byte-identical to Version 1 |

### 3.5 Known boundaries of Version 2

- **The official CHM is byte-identical to Version 1.** `SHA-256 624308649E523400…`, 5,193,985 bytes, 15 topics, zero changed characters. **41 new objects, a complete SMS subsystem and 17 new tables were added without a single line of user-manual change.**
- **The shipped Access database is out of step with the executable.** The 268 embedded DDL statements were applied only partially to the shipped `Patients.mdb` — 61 of the changes are not present (23 are SQL-Server-only syntax and cannot apply to Jet 4; 38 are Access-applicable but were never executed).
- **Five large Microsoft installers referenced by the deployment scripts are absent from the inventoried tree:** `SQLEXPR.EXE`, `msxml6_x86.msi`, `dotnetfx.exe`, `WI-KB893803-v2-x86.exe`, `mysql-connector-odbc-3.51.30-win32.msi`. The installation media is therefore partial.
- **`VIDAS` is referenced but never structurally characterised.** Its nature — including whether it is Visual FoxPro — is not established.
- **No dynamic behaviour observation.** See §5.

---

## 4. Relationship between the two versions

### 4.1 What is established

| Relationship | Evidence | Certainty |
|---|---|---|
| They are the **same product line** | Identical executable name, identical directory layout, identical PE build date, identical linker version, shared module names (`labdb`, `labfunction`, `printerselect`), identical CHM bit-for-bit, identical knowledge-layer table names | CONFIRMED |
| Version 2 **supersedes** Version 1 | 62% of Version 1's 1,337 SQL statements appear verbatim in Version 2's 1,797; Version 2 adds 41 objects with **zero deletions**; same installation directory name | STRONGLY SUPPORTED |
| The two are **not** in a linear upgrade path | Both ship `Patients.mdb` with **different** schemas (93 vs 103 tables) and different row counts (3,123 vs shipped-2019 values); the executable embeds a migration that Version 1 does not have | CONFIRMED |
| Version 2 is distributed as an **installer package**, Version 1 as a **bare file tree** | `RL_SETUP.exe` + `WIN7\`/`WINXP\` CAB targets exist only in Version 2 | CONFIRMED |

> **★ "Supersedes" means feature-set superset, not data-compatible upgrade.**
> The evidence does **not** establish that a Version 1 database can be opened by
> Version 2 unchanged. The reverse is also not established.

### 4.2 What is NOT established

- The **release chronology** of the two versions is not established from file evidence alone. Both executables carry PE timestamp 2013-12-27; Version 1's payload and Version 2's learning guide are dated 2010–2013, while Version 2's installer and payload are dated 2019–2020.
- Whether Version 1 and Version 2 were ever **installed concurrently** on the same machine.
- Whether `Patients_backup.mdb` (90 tables, 2,786 rows) represents a **Version 1 point-in-time backup of a Version 2 database** or a Version 1 artefact. Its column count (1,381) is below Version 1's `Patients.mdb` (1,527), which is consistent with either reading. **Marked UNKNOWN.**
- The **migration path** from a Version 1 database to a Version 2 database is not documented in any shipped artefact.

---

## 5. Cross-version limitations affecting both documents

### 5.1 Static-only analysis

No reference executable, installer, script or ActiveX control was executed at any point
across the 11 investigation batches. All findings are derived from static parsing of
file content. This is a deliberate investigation constraint, and it means:

- **No runtime-verified behaviour.** Every behavioural statement in this package is
  derived from code, schema, documentation or screen-recording evidence.
- **No confirmed control flow.** Reconstructed workflows in the Functional
  Specifications are inferences from evidence, not observed executions.

### 5.2 Confidence classification used throughout

| Classification | Meaning |
|---|---|
| **CONFIRMED** | Directly established by reliable evidence (a shipped file's content, an extracted schema, a transcribed screen). |
| **STRONGLY SUPPORTED** | Supported by multiple consistent independent evidence items, but not directly confirmed in every relevant aspect. |
| **INFERRED** | A reasonable interpretation derived from available evidence, not directly established. |
| **UNKNOWN** | The available evidence is insufficient to establish the finding. |
| **CONFLICTING** | Available evidence contains unresolved contradictions. Both sides are preserved. |

### 5.3 Evidence-access limitations

| Limitation | Effect |
|---|---|
| `RLS_Learn.pdf` embedded Arabic font has **no `ToUnicode` map** | Arabic text extracts in scrambled glyph order. Headings in the vendor catalogue were corrected **visually from page images**. The raw extraction is preserved as `analysis/BATCH-05/manuals/RLS_Learn.txt` |
| `RL_Show.pdf` contains 77 characters over 77 pages | Effectively image-only; read visually, not textually |
| `RL_SETUP.exe` carries a **22.95 MiB embedded payload** in `.text` at entropy 7.39 | The payload was characterised but not decomposed; its contents are not documented |
| `Branch_Data.mdb` is **schema-only (0 rows)** | Relationship and lookup semantics of branch tables are inferred from DDL, not from data |
| `normalrange.pregsmok` is populated on **all 852 rows** but its use in calculation is not traced | Reported as OBSERVED, not as a working rule |
| 162 of 837 artefacts remain at `PRELIMINARY_EXAMINATION` | Their format is identified but their content is not deeply analysed |

---

## 6. Attribution rules applied in this package

1. A finding is attributed to **V-ROOT-1.00** only if the evidence comes from a
   Version 1 artefact or from a shared artefact whose content is byte-identical in
   both versions.
2. A finding is attributed to **V-CD-2019** only if the evidence comes from a
   Version 2 artefact.
3. A finding is marked **APPLIES TO BOTH** only where the shared artefact's content
   was **compared byte-for-byte** across both versions, or where the relevant code
   path exists in both executables and was separately observed in each.
4. **No inheritance is assumed.** Where a Version 1 capability was not located in
   Version 2, it is recorded as *not established in Version 2* — never as *removed*,
   and never as *present*.
5. Where a difference was **not** established, the difference is recorded as
   UNKNOWN rather than assumed to be equivalence or difference.

### 6.1 Shared artefacts confirmed byte-identical

| Artefact | V1 path | V2 path | SHA-256 (both) |
|---|---|---|---|
| `real lab system help.chm` | `real lab system\real lab system help.chm` | `RL_CD2019Access\real lab system help.chm` | `624308649E523400…` |
| `Data\*.ini` (104 of 223 differ) | `real lab system\Data\` | `RL_CD2019Access\Data\` | per-file |
| `Cache\*.rtf` (29 of 29 identical) | `real lab system\Cache\` | `RL_CD2019Access\Cache\` | per-file |
| ActiveX controls (43 of 43 identical) | `real lab system\*.OCX` | `RL_CD2019Access\OCX\` | per-file |
| Flash simulators (13 of 13 identical) | `real lab system\Cache\*.exe` | `RL_CD2019Access\Cache\*.exe` | per-file |

---

## 7. Unresolved identity and scope uncertainties

| ID | Uncertainty | Status |
|---|---|---|
| RL-UNC-001 | Actual commercial release name of V-ROOT-1.00 — the product is described internally as "real lab system version 1.0" and the vendor as "ais group"; no version manifest exists | UNKNOWN |
| RL-UNC-002 | Whether `Patients_backup.mdb` is a Version 1 artefact or an early Version 2 backup | UNKNOWN |
| RL-UNC-003 | Nature of the `VIDAS` database (attached by `Attach_VIDAS.bat`); a Visual FoxPro 6.0 runtime is present in the shipped VB6 SP6 redistributable manifest but no structural link is established | UNKNOWN |
| RL-UNC-004 | Purpose of the 22.95 MiB embedded payload inside `RL_SETUP.exe` | UNKNOWN |
| RL-UNC-005 | Exact composition of the 4 large Microsoft installers absent from the inventoried installation tree | UNKNOWN |
| RL-UNC-006 | Whether the 15 Microsoft installers referenced by `Setup*.bat` were shipped on a separate disc not covered by the inventory | UNKNOWN |
| RL-UNC-007 | Whether Version 1 and Version 2 share a common ancestor codebase or were forked | UNKNOWN |

---

**End of Version_Identity_and_Scope.md**
