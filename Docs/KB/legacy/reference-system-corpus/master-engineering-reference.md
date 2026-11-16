# Master Engineering Reference — RealLab CD 2019 v1.605

**Consolidated:** 2026-10-31 (UTC) · **Static analysis only.** Nothing from
any batch was executed, installed or registered; no database, URL or credential
was contacted or validated.

> **This is the central document.** It explains the product as one integrated
> application and links to the detailed technical references, which carry the
> file-level, table-level and object-level evidence.

---

## 1. Identity and foundations

### 1.1 What it is

**RealLab CD 2019** — internal product name **`real lab system`**, version
**1.605** — a clinical **Laboratory Information System** developed by
**ais group corporation**, a vendor based in **Egypt** (country code 002;
the project's own `.eg` domain `RealLabeg.com`; the manual's telephone number
`002 014 484 86 86`).

Its purpose is the full laboratory cycle: register a patient, order tests,
capture results per discipline, validate them against reference ranges,
produce a signed printed report, deliver it (including by SMS), and bill for
it — with an audit trail, a work-paper, referral to other laboratories and a
back-up of the clinical database.

### 1.2 Technology and runtime — proven four independent ways

| Evidence | Result |
| --- | --- |
| PE entry stub `68 80 8E 4B 00 / E8 EE FF FF FF` | `push <VBHeader>` ; `call ThunRTMain` — the VB6 native-compilation startup sequence |
| VB project header at the pushed address | signature **`VB5!`**, runtime build **9782** = the **VB6 Service Pack 6** runtime (`msvbvm60.dll` 6.00.9782) |
| Import table | **exactly one DLL**: `MSVBVM60.DLL`, 231 functions — every Win32 call goes through the runtime's late-bound `DllFunctionCall` |
| VB6 object table | 212 named objects with 8,026 methods, decoding cleanly from the documented `ObjectTable` layout |

**It is Visual Basic 6, natively compiled, 32-bit x86.** The project header
declares `wFormCount = 198`, `wExternalComponentCount = 25`, `wThunkCount = 233`,
project GUID `6c1467af-a022-9145-9ed352beb9d44d32`, and LCID 1033 with
secondary LCID **4105 (Arabic, Saudi Arabia)** — the product's own
locale declaration.

> **A note on the localisation tension.** The vendor's manual requires the host's
> Regional Options to be **English (Canada)**, while the project header
> declares Arabic and every template and document is stored in **Windows-1256**.
> The regional-format setting governs number and date rendering; the codepage
> governs the Arabic text. Whether the application reads its templates
> *explicitly* as cp1256 rather than as ANSI could not be determined, and it is
> the mechanism behind U-002 in the risk register.

### 1.3 Executable structure

| Property | Value |
| --- | --- |
| Size | 76,292,096 bytes (72.76 MiB) |
| SHA-256 | `c54ce6c8c98a679efc5ab485bc4b313b452026d6a40f9bda36eca7ea18df93c0` |
| Format | PE32, i386, Windows GUI |
| Image base / entry | `0x00400000` / RVA `0x0007142C` |
| Sections | 3 — `.text` **76,119,116 B (97.0 %)**, `.data` 304,852 B virtual / 4,096 B raw, `.rsrc` 161,682 B |
| Imports | `MSVBVM60.DLL` only (231 functions) |
| `DllCharacteristics` | **`0x0000`** — no ASLR, no DEP/NX, no CFG |
| Authenticode | **absent** |
| Manifest | **absent** — no UAC level, DPI awareness or common-controls v6 is declared |
| Resources | 10 icons (incl. a 256×256 **PNG**), 1 group icon, 1 VERSION resource (804 B) |

Full detail: `primary-executable-technical-reference.md`.

### 1.4 Deployment and installation architecture

The product is installed by a **VB6 Setup Wizard** project in **two OS
variants**, `WINXP/` and `WIN7/`, each with a `SETUP.LST` manifest and a
48-file `real lab system.CAB` payload. A set of **nine batch scripts** drives
the prerequisite chain.

```
Setup1.bat  → Windows Installer 3.1  (a 2004 hotfix)
Setup2.bat  → MSXML 6.0
Setup3.bat  → .NET Framework 2.0
Setup4.bat  → SQL Server Express 2005   ← creates the sa account
Setup5.bat  → MySQL Connector/ODBC 3.51.30
SetupPZ.bat → .NET Framework 4 + Bullzip PDF Printer
Setup.bat   → 1+2+3+4, then attaches the clinical database from D:\Lab_data
```

Installation destinations, from `[Setup1 Files]`:

| Destination | Contents | Count |
| --- | --- | ---: |
| `$(ProgramFiles)\real lab system` | the application | — |
| `$(WinSysPath)` | 16 ActiveX controls, all `$(DLLSelfRegister)`, plus `msado27.tlb` | 17 |
| `$(WinSysPathSysFile)` | the OLE Automation stack, the Jet 4.0 engine, `expsrv`, `vbajet32` | 14 |
| `$(MSDAOPath)` | `dao360.dll` | 1 |
| `$(Font)` | the five product fonts, all `$(Shared)` | 5 |

Full detail: `installation-and-deployment-reference.md`.

### 1.5 External dependencies

| Dependency | Version | Source |
| --- | --- | --- |
| `MSVBVM60.DLL` | 6.00.9782 (SP6) | the product's own installer; the executable's only import |
| `MSDBRPTR.DLL` | 6.01.9782 | the report engine, deployed by the installer |
| `ADO` (`msado27.tlb`) | **2.81.1132.0** (MDAC 2.8) in the XP variant; 6.1.7601.17514 in the Win7 variant | the installer |
| **Jet 4.0** | **SP2** 4.0.9502.0 (XP) / **SP3** 4.0.9756.0 (Win7) | the installer |
| 16 ActiveX controls | 1998–2010 vintages | the installer |
| `jscript.dll` / `vbscript.dll` / `msscript.ocx` | 5.1.0.8513 / 1.0.0.3715 | the `datareport_vh` folder |
| **Internet Explorer** | the host's browser | driven through COM for outbound SMS |
| Bullzip PDF Printer | 11.1.0.2600 **Professional Evaluation** | the installer |
| MySQL Connector/ODBC | 3.51.30 (2006) | the installer — **no counterpart in the executable** |

### 1.6 Supported operating environment

Windows 32-bit and 64-bit. The distribution carries a **16-bit NE launcher**
(`setupsp6.exe`, Windows 3.x) in the VS6 SP6 path, which cannot run on 64-bit
Windows without 16-bit support; the product's own image base is the classic
`0x00400000`; and the executable declares no manifest, so it inherits the
legacy common-control rendering and the default UAC virtualisation.

---

## 2. Application architecture

### 2.1 Entry points and object model

The application is a single 32-bit GUI executable. Its 212 VB6 objects divide
into coherent families — the full register with method counts is in
`functional-module-catalogue.md` and `machine-readable/functional-module-catalogue.csv`.

| Family | Count | Role |
| --- | ---: | --- |
| `Result<discipline>` | ~55 | **capture and validate** the result for one discipline |
| `Report<discipline>` | ~40 | **render** that discipline's report |
| Core application | ~20 | `labmain` (71), `patientmain` (84), `labdb` (24), `addpatient` (192), `patientview` (33) |
| Configuration / setup | ~25 | `reportsetup` (113), `reportsetup2` (69), `normal_test` (36), `setup1/2/3`, `testrange` (17) |
| Clinical reference | ~20 | `abbrevi` (41), `commentsetup`, `noteic`, `converter`, `testmain` |
| Administration | ~20 | `users` (57), `userspw` (12), `login` (13), `server` (17), `instrument` (29), `pricelists` |
| Output / print | ~15 | `barcode` (76), `barcodeform`, `workpaper` (17), `Reportworkp` |
| Finance / referral | ~10 | `frm_money` (38), `moneypaid` (18), `moenysharekat` (27), `moenymozafeen` (27) |
| Audit / master data | ~10 | `z_logs`, `z_gehats` (56), `z_branchs`, `z_units`, `z_pricelists`, `z_titles`, `z_customgroup` |
| Communications | ~5 | `SMSfrmsenderos` (139) |
| Utilities | ~12 | `tools`, `labfunction` (25), `BCWFonts` (7), `ais`, `labxxyyzz` |

### 2.2 The singular/plural pattern — how data-driven reporting works

**Many `Report*` objects have zero methods.** `Reportliver` carries the
rendering logic; `Reportlivers` has **0 methods**. That is the signature of a
VB6 **data-driven report definition**: the object binds a template and the
layout comes from `Data/*.ini`.

The plural forms are the summary/aggregate reports (`Reporturines`,
`Reportsugars`, `Reporttotals`, `ReportPCRs`, `ReportIGE1s`, `ReportLBCs`,
`ReportZNs`, `Reportanyreports`, `Reporthistory2`).

**This is the mechanism behind B3's 146 templates, and it is the clearest
single explanation of how the product is organised.**

### 2.3 Navigation and interaction

Recovered from the object names and the manual:

* **Menu-driven** top-level bars — `المرضى` (Patients), `بيانات النظام`
  (System data), `حسابات` (Accounts), `اعدادات` (Settings) — with
  **F7** bound to the send-out-samples and the grading/inventory windows.
* **Patient → result entry → test → double-click** opens the per-test result
  window, where the orange **test-group name button** opens the Report
  Settings window.
* The **Constants** button in the lower-right of the result window edits the
  laboratory's calculation constants.

### 2.4 External components

The header declares **25** external components. **Their names were not
recovered** — the descriptor layout for that table was not established
(UNR-009). What *is* established is the installed set, from B2's inventory
and B4's manifest: 16 ActiveX controls, the VB6 runtime, the OLE Automation
and Jet/ADO stacks, the Microsoft Data Report engine, the script engines, and
the setup helpers.

---

## 3. Functional architecture

Each area below connects the executable's objects, the database operations,
the configuration and the batch evidence.

### 3.1 Patient management

**Objects:** `patientmain` (84), `addpatient` (192), `patient`, `patientview`
(33), `patienthistory` (22), `patienthistory2` (17).
**Database:** `patientinfo` (65 SELECT targets), `patienttest` (138 — the
busiest table in the system), `payment` (14), `patienttotal` (7).
**Configuration:** `Data/*.ini` templates; the `wasl` reference table
(5 SELECT targets, 12 ALTER statements).
**Evidence:** B1's schema reconstruction; B4's manual topics 4 and 9; the
executable's DDL `ALTER Table patientinfo ADD gehaname varchar(50)`.

### 3.2 Test ordering and the catalogue

**Objects:** `testmain` (30), `testrange` (17), `learntest` (6), `calctest` (2),
`abbrevi` (41).
**Database:** `testtable` (26), `subgrouptable` (12), `customtesttable` (5),
`customgrouptable` (6), `testunits` (7), `titless` (10), `constanttable` (4),
`abbreviations` (6).
**The numeric test-ID namespace** — the template filenames are keyed by the
`testid Smallint` column that `LBC` and `IgEallergy` declare. B4's manual
confirms the numbers are **permanent** and that empty slots are deliberate.
That closes B3's DEP-B3-U03 and U05 to the extent the database allows.

### 3.3 Disciplines

Recovered from the `Result*` objects — the system's own list of what it
supports: liver, urine, urine-auto, sugar (and a sugar curve), stool, kidney,
culture, lipids, semen, ABO, widal, hepatitis, ESR, CBC, TORCH, CRP, PT,
hormone, PCR, FMF, LBC, IgE allergy, cardiac, tumours, vitamins, trace
elements, fertility, suprarenal, pregnancy, iron, abuse, drugs, coagulation
factors, immunoglobulins, immunology, Hb electrophoresis, protein
electrophoresis, blood gas, cross-match, stone, prostate, CSF, zinc,
tuberculin, marrow, post-coital, renal, retics, H. pylori, Haemoglobin A1c,
**ResultFMF** and **ResulturineAuto**.

**Every one of these corresponds to a report template family in B3 and a
discipline result table in B1.** The three views agree.

### 3.4 Result entry, validation and approval

**Objects:** the `Result*` family; `normal_test` (36) for the reference ranges;
`resultselectro` (144) for the reference-range lookup.
**Database:** `normalrange` — **78 SELECT targets, the second-busiest table**.
**Mechanism, reconstructed:** the `Result*` object captures a value, reads the
matching row from `normalrange` by test ID + sex + age band, applies
`Low limit` / `High limit`, and raises the abnormal/critical flags; the
template then decides how the flag is **rendered** (`backcolor`,
`statusflag`, `printcritical`, `main_symb`/`sub_symb`).
**The same limits drive the on-screen window and the printed report** — the
manual says so explicitly, and the template keys confirm it.
**Evidence:** B4's manual topic 11 (the reference-range editor); B3's
`[reportdata]`/`[FORM]` keys; the executable's `normalrange` queries.

### 3.5 Report generation, printing and export

**Objects:** the `Report*` family; `reportsetup` (113) and `reportsetup2` (69).
**Configuration:** `Data/*.ini` (146) and `Cache/*.rtf` (29).
**Engine:** `MSDBRPTR.DLL` — the Microsoft Data Report engine (B2).
**Devices:** the printer named in `[setupx] reportprinterx`
(`HP LaserJet 1100 (MS)`), and the Bullzip PDF printer B4 installs.
**Design:** the report-settings window edits the report name, the side title,
the reference-range caption, the doctor's signature, the comment, page and
element alignment, element colours, the vertical spacing, the printer, and
five show/hide switches. **Every one of those is a key in B3's templates.**

### 3.6 Barcode functions

**Objects:** `barcode` (76), `barcodeform`, `z_barcodes` (14).
**Database:** `barcodes` (7 SELECT targets).
**The scheme, from the manual:** every patient carries **three** codes — a
**visit/day code** (sequential, ordering of arrival, from digit 1), a **file/vial
code** (from digit 3, printed on the barcode and affixed to the vial with the
patient's name, type, age and visit date), and a **laboratory code** (from
digit 5, for quick access and history printing). The composite is **13
digits**, packing a sequence, the arrival order, the **branch number**, the
**date read from the right** (DDMMYY) and the **day-of-week number from
Sunday**.
**Rendering:** the Code 39 (`FREE3OF9.TTF`) and UPC-A/EAN (`WBCW_UPCEAN.ttf`)
fonts B3 analysed — which is *why* a fixed-width symbology is needed.

### 3.7 Financial and billing operations

**Objects:** `frm_money` (38), `moneypaid` (18), `qcfrm` (27),
`moenysharekat` (27), `moenymozafeen` (27), `shoppingfrm` (15).
**Database:** `payment` (14), `pricelist` (8), `patienttotal` (7),
`sharekathesab` (9), `sharekat` (4).
**Evidence:** B1's `patientinfo.pricemethod` / `totalprice` / `discount`
columns; the executable's `SELECT patientid, elbakyma3mal, discount100,
discount, elmadfoo3 FROM patientinfo where patientid='` and its
`Patients.grant1..grant17` flags; B4's manual topics 13 and 14.

### 3.8 Users, permissions and administration

**Objects:** `users` (57), `userspw` (12), `login` (13), `logindete` (6),
`logindetect` (0), `server` (17).
**Database:** `useraccounts(accuntID varchar(50), accuntpw varchar(50),
updateflage BYTE)` with `HDD1`, `HDD2`, `Gtype`.
**Authorisation:** 17 boolean `grantN` columns on `Patients`; seven shared
`IDPassword*` print secrets; the maintenance dialog behind a single password;
**no role model**.
**Evidence:** the executable's DDL and login queries; B1's FND-010; B4's
manual topics 3 and 15.

### 3.9 Administrative and maintenance functions

| Function | Object / evidence |
| --- | --- |
| Database maintenance | `Database Maintenance` window — zero all data, backup, restore, delete backups, restore one patient by work code. Guarded by a single password (**B4 manual topic 15**) |
| Backup | `Patient_yyyymmdd.bak` in the database directory; the backup path is operator-configurable (**B1 FND-018/FND-019**) |
| Send-out samples | `labtolab` (32), `moenysharekat`, `moenymozafeen`, `labmainTools` (6); **F7** |
| Grading / period inventory | Accounts → grading and inventory (**B4 manual topic 14**) |
| Instrument registry | `instrument` (29) — B1's `instrument` table |
| SMS notification | `SMSfrmsenderos` (139) + the `MyLabSetting` gateway credentials |
| Notes | `noteic` (10) — B1's `noteic` table |
| Comments | `comments` (28 SELECT targets) |

---

## 4. Database architecture

**Two backends.** The live record is **SQL Server, database `Patients`**,
reached through `SQLOLEDB.1` (and `MSDataShape.1` for shaped recordsets). The
offline mirror is **Microsoft Access / Jet 4**, reached through
`Microsoft.Jet.OLEDB.4.0`, with its own `Patients_backup.mdb` and
`repairedDB.mdb` siblings.

**The credential is assembled at run time** from three separate literals —
`Provider=SQLOLEDB.1;Persist Security Info=True;Initial Catalog=Patients;Data Source=`,
`;User ID=`, `;Password=` — which is why a naive string scan of the binary
yields no usable password while the *shape* of the credential is recoverable.

**The client owns the schema**: 14 `CREATE TABLE` and **120 `ALTER TABLE`
statements across 31 tables** run against the server database.

Full detail, including the table inventory and the SQL corpus statistics, is
in `database-engineering-reference.md`.

---

## 5. Security architecture

Consolidated: **66 original findings → 41 unified subjects**, 17 High, 27
Medium, 15 Low, 7 Info. The full provenance-preserving register is
`unified-findings-register.md`; the risk view is
`security-and-technical-risk-register.md`.

**The four structural weaknesses:**

1. **Clinical data crosses trust boundaries in cleartext** — the database
   credential, the operator password, the SMS gateway password and the report
   passwords.
2. **The client holds server-side privilege it does not need** — it runs DDL
   against the clinical database.
3. **The deployment is invasive and unhardened** — system-wide COM
   registration, system-directory DLL copies, machine-wide installation of a
   renamed Microsoft typeface, a 16-bit launcher, a 1996 help file, an
   evaluation-edition commercial printer installed silently, and binaries with
   no ASLR, no DEP and no signature.
4. **The report subsystem is a scriptable HTML surface** — B2's script
   engines, B3's HTML designer, B4's Bullzip macro host, and — from the
   executable — Internet Explorer driven by COM.

---

## 6. What the batches contribute to the whole

| Artefact family | B1 | B2 | B3 | B4 | Executable |
| --- | :-: | :-: | :-: | :-: | :-: |
| The main executable | ● | | | | ●● |
| The Access/Jet mirror + schema | ●● | | | | ● |
| The ActiveX/component payload | | ●● | | ● | ● |
| The report templates | ● | | ●● | | ● |
| The clinical reference library | ● | | ●● | | ● |
| The fonts | | | ●● | ● | ● |
| The installer and manifests | ● | ● | | ●● | ● |
| The user manual | | | | ●● | |
| The SQL and data model | ● | | | | ●● |
| The security findings | ●● | ●● | ●● | ●● | ●● |

**● = contributes, ●● = primary source**

The overlaps are not decorative — they are **cryptographic**:
B2 ↔ B4-derived **25** identical components, B3 ↔ B4-derived **5** identical
fonts, B1 ↔ B3 **2**, B1 ↔ executable **1**. See
`cross-batch-correlation-matrix.md`.

---

## 7. Detailed references

| Document | Scope |
| --- | --- |
| `unified-application-architecture.md` | Components, relationships, dependency and data-flow diagrams |
| `functional-module-catalogue.md` | The 212-object register and the functional areas |
| `primary-executable-technical-reference.md` | The executable in full, and its integration with every other part |
| `database-engineering-reference.md` | Schema, tables, SQL, connections, data flows |
| `installation-and-deployment-reference.md` | Installers, manifests, dependencies, configuration, deployment structure |
| `security-and-technical-risk-register.md` | The consolidated risk view |
| `unified-findings-register.md` | Every finding with its origin and cross-references |
| `cross-batch-correlation-matrix.md` | How the batches relate, with the SHA-256 evidence |
| `unresolved-questions-and-evidence-gaps.md` | Seventeen open questions and what would close them |
| `verification-and-integrity-report.md` | Hashes, archives and validation results |
| `project-evidence-and-source-inventory.md` | Every source artefact, with purpose and verification |
