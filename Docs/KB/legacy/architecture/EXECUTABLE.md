# Legacy Executable Specification — the VB6 Application

**Purpose:** Characterisation of the reference system's executable layer: PE structure, the VB6
project layout, the object model and its evolution between versions, the embedded SQL literal
corpus, the string pool, the runtime dependencies and the external integrations.
**Scope:** `real lab system.exe` (V-CD-2019 / v1.605), its V-ROOT-1.00 predecessor, and the
companion executables.
**Audience:** Developers and architects; the primary input to any re-implementation or port.
**Version applicability:** V-CD-2019 (v1.605) is canonical. V-ROOT-1.00 figures are the baseline.
**Evidence:** `txt nd sql/objects-full*.txt`, `analysis/BATCH-04/vb6/v1605/objects.json`,
`analysis/BATCH-04/vb6/v1605/object-diff-vs-v1.json`,
`analysis/BATCH-04/sql_v1605/statements.csv`, `analysis/BATCH-01/sql/statements.csv`,
`txt nd sql/session-2026-10-31-executable.log`, `txt nd sql/pe-summary.txt`,
`analysis/BATCH-01/pe-summary.csv`, `analysis/BATCH-11/pe-envelope.md`.

---

## 1. The canonical executable — V-CD-2019

| Property | Value | Evidence |
|---|---|---|
| File | `real lab system.exe` | `session-2026-10-31-executable.log` |
| Size | **76,292,096 B** | SHA-verified in `batch1-hashes.txt` |
| SHA-256 | `c54ce6c8c98a679efc5ab485bc4b313b452026d6a40f9bda36eca7ea18df93c0` | `batch1-hashes.txt`, `archive-checksums.txt` |
| Format | PE32, i386, GUI, **3 sections**, unsigned | session log |
| `.text` section | 76,119,116 B (**97 %** of the file) | session log |
| Entry point | RVA `0x7142C`; image base `0x400000`; `DllCharacteristics 0x0000` | session log |
| Entry stub | `68 80 8e 4b 00 e8 ee ff ff ff` → `push <VBHeader>; call ThunRTMain` | session log |
| VB header | offset `0xB8E80`, magic `VB5!`, runtime build **9782 (SP6)** | session log |
| Project name | `Real_lab_system` | session log |
| Imports | **`MSVBVM60.DLL` only** — 231 functions | session log, `Documenting the Executable File.md` |
| Resources | 10 × ICON + 1 GROUP_ICON + 1 VERSION (804 B); **no manifest** | session log |
| VERSION resource | `FileDescription` **`RLS 1.605`**, `FileVersion` **1.605**, `OriginalFilename` `real lab system.exe`, `CompanyName` `RealLab`, `Comments` `ais_soft_group@yahoo.com` | session log |
| Strings | 21,875 ASCII + 9,554 UTF-16LE = **31,429** | session log |
| Object table GUID | `6c1467af-…` | session log |

**★ The `VERSION` resource is the single strongest version discriminator in the whole corpus:**
it carries the release number in the product description itself.

---

## 2. Version evolution of the executable

| Property | V-ROOT-1.00 | V-CD-2019 (v1.605) | Delta |
|---|---:|---:|---:|
| Size | 59,150,336 B | 76,292,096 B | +17,141,760 |
| SHA-256 | `12828d0a0eea121c0a6e08bdaf5f369d4d1d4d2b4f8b65320ac308fbc01fd885` | `c54ce6c8…` | — |
| `FileVersion` | `1.00` | `1.605` | — |
| VB6 project objects | **169** | **210** | **+41** |
| Declared methods | **6,528** | **8,026** | **+1,498** |
| Form-kind objects | 118 | 132 | +14 |
| DataReport objects | 45 | 66 | +21 |
| Module objects | 5 | 9 | +4 |
| Class objects | 1 | 3 | +2 |
| VB header `wFormCount` | 163 | **198** | +35 |
| VB header external components | 21 | **25** | +4 |
| VB header `dwLCID` | `0x00000409` (en-US only) | **1033 / 4105 (ar-SA)** | localised |
| Embedded DDL | 0 | **268** | +268 |
| SQL statements | 1,337 | **1,797** | +460 |
| PE timestamp | 2013-12-27 | **2013-12-27** | **identical** |

> **★ The PE timestamp does not discriminate the versions.** Both executables carry
> **2013-12-27** and linker 6.0. The v1.605 executable was never recompiled from source after 2013;
> only its embedded strings, resources and installer changed. Any claim of the form "v1.605 is
> newer because its build date is later" is **false**.

> **★ The `dwLCID` change is a first-class version discriminator.** V1 is en-US only; V2 is
> localised to Arabic (Saudi Arabia). This is the structural trace of the Arabic-only product
> deployment.

**VERIFIED** from `session-2026-10-31-executable.log`, `txt nd sql/version-summary.txt`,
`txt nd sql/vb6-chain-analysis.txt` and `analysis/BATCH-04/vb6/v1605/objects.json`.

### 2.1 Reconciling the three object counters

| Counter | V1 | V2 | What it counts |
|---|---:|---:|---|
| Object-table entries | 169 | 210 (212 slots, 210 compiled) | Compiled VB6 project objects |
| `wFormCount` (VB header) | 163 | 198 | Forms declared in the header, including non-compiled forms |
| Forms with a recoverable form-data blob | 117 | 131 | Forms whose layout could be extracted |

These are three different measurements and must not be interchanged. The knowledge base's
"Forms 118 / 132" is the **object-table** count.

---

## 3. The VB6 object model

### 3.1 V-CD-2019 composition

| Type | Count |
|---|---:|
| Form | 132 |
| DataReport (print template) | 66 |
| Standard module | 9 |
| Class module | 3 |
| **Total** | **210** |
| Declared methods | **8,026** |

**Method-entry recovery is incomplete.** Of the 8,026 declared methods, only **1,268 method entries**
were read (with names but no addresses): `method_entries_read: 1268`,
`methods_without_address: 1268`. **The method bodies were never decompiled** — this environment had
no VB6-aware decompiler. The object model is therefore recovered at *declaration* level, not at
*implementation* level. **PARTIALLY VERIFIED** — see §6.

### 3.2 The V1 → V2 object delta — authoritative

`analysis/BATCH-04/vb6/v1605/object-diff-vs-v1.json` is the most precise implementation-level
artefact in the corpus:

| Metric | Value |
|---|---:|
| V1 object count | 169 |
| V1.605 object count | 210 |
| Objects **added** | **41** |
| Objects **removed** | **0** |
| Objects with **changed** method counts | **88** |
| Net method delta on the 88 changed objects | **+296** (5,780 → 6,076) |

**The 41 added objects**, grouped by function:

| Group | Objects |
|---|---|
| **New clinical profiles** | `ResultFMF`, `ResultPCR`, `ResulturineAuto`, `Resultsemen5`, `resultIgeAllergy`, `resultLBC`, `resulthpylori` |
| **New report templates** | `ReportIGE1`, `ReportIGE1s`, `ReportLBC`, `ReportLBCs`, `ReportPCR`, `ReportPCRs`, `ReportZN`, `ReportZNs`, `ReportSMS`, `ReportPatPhone`, `ReportTestTime`, `ReportFatoraC`, `ReportFatorareset`, `ReportFatorareset8`, `Reportliver7his`, `Reportlivers7his`, `Reportmony01`, `Reportmony05`, `Reportmony11`, `RPTPatientVisit` |
| **SMS subsystem** | `SMSfrmsenderos` |
| **Patient / identity** | `CardFrm`, `CardFrmSetx`, `Frmac`, `Patient_samples`, `RPTPatientVisit` |
| **Money / accounting** | `moenysharekat`, `pricelistsN` |
| **Laboratory configuration** | `labmainTools`, `BiosData` |
| **Infrastructure / utilities** | `server`, `otherfun`, `mInputbox`, `saveIOtemp`, `static6`, `aaaClass`, `boisais` |

**★ Five objects went from zero declared methods to fully functional in V2** — these are *newly
enabled* capabilities, not refactors:

| Object | V1 methods | V2 methods | What it suggests |
|---|---:|---:|---|
| `antibioticfrm` | **0** | **29** | The antibiotic-susceptibility screen is a V2-only feature |
| `moenysharekat` | **0** | **27** | The share/referral money screen is a V2-only feature |
| `server` | **0** | **17** | Server/engine configuration is a V2-only feature |
| `learntest` | **0** | **6** | A training/test path exists only in V2 |
| `logindete` | **0** | **6** | Login-detail handling exists only in V2 |

**Largest method-count increases** (all in V2):

| Object | V1 | V2 | Delta | Area |
|---|---:|---:|---:|---|
| `addpatient` | 125 | 192 | **+67** | Patient registration |
| `patientmain` | 54 | 84 | +30 | Patient list / lookup |
| `antibioticfrm` | 0 | 29 | +29 | Antibiotic susceptibility |
| `users` | 29 | 57 | +28 | User and permission management |
| `moenysharekat` | 0 | 27 | +27 | Referral money |
| `resultretios` | 74 | 97 | +23 | Reticulocytes |
| `resultcbc` | 150 | 172 | +22 | CBC entry window |
| `setup3` | 22 | 42 | +20 | Installation configuration |
| `resultbloodgas` | 64 | 81 | +17 | Blood-gas entry window |
| `server` | 0 | 17 | +17 | Server / engine configuration |
| `frm_money` | 22 | 38 | +16 | Money / collection |

**Largest method-count decreases** (method consolidation or removal in V2):

| Object | V1 | V2 | Delta |
|---|---:|---:|---:|
| `Resultsemen` | 255 | 222 | −33 |
| `resultTrace` | 203 | 183 | −20 |
| `resultselectro` | 159 | 144 | −15 |
| `ResultFertlity` | 136 | 125 | −11 |
| `ResultSuprarenal` | 128 | 117 | −11 |
| `Resultkidney` | 127 | 117 | −10 |

The removals cluster in *result-entry windows* while the additions cluster in *patient
administration*, *users* and *newly enabled* screens. **INFERRED:** V1.605 concentrated effort on
registration, permissions and new profiles, and consolidated older result-window code.
Confidence: MEDIUM.

### 3.3 Companion executables

| Executable | Objects | Methods | Role |
|---|---:|---:|---|
| `real lab system.exe` | 210 | 8,026 | The application |
| `RL_SETUP.exe` | **57** | **599** | Custom VB6 self-extracting installer, v19.00 |
| `Patient Data Transport.exe` | 2 | — | Inter-process transport, 1.23 MB, 2011-01-04 |
| `real lab system help.chm` | — | — | Official manual, byte-identical in both versions |
| `real lab Image Gallery.exe` | — | — | **V1 only — removed in V2** |

---

## 4. The SQL literal corpus

### 4.1 Statement census

| | V1 | V2 |
|---|---:|---:|
| **Total SQL statements** | **1,337** | **1,797** |
| `SELECT` | 1,220 | 1,381 |
| `ALTER` (schema migration) | 0 | 241 |
| `CREATE` (schema migration) | 0 | 27 |
| `DELETE FROM` | 72 | 72 |
| `SHAPE` (Jet 4 row-source) | 20 | 27 |
| `OTHER` | 25 | 49 |
| Distinct tables touched | 100 | 117 |

> **Note on a conflicting figure.** `Documenting the Executable File.md` reports **2,375 SQL-bearing
> literals**. That is a *literal-string* count (every string in the pool that looks SQL-shaped),
> not a *statement* count. The statement census recovered by parsing the executable's SQL is
> **1,797** for V2 and **1,337** for V1. Both figures are correct for their own measure; they must
> not be used interchangeably. The session log additionally records **1,370 SELECT** statements for
> V2 — a subset of the 1,797, and a third measure again.

### 4.2 The `SHAPE` statements

27 `SHAPE` statements in V2 use Jet 4's `SHAPE … APPEND … RELATE` syntax to build hierarchical
result sets. Example (from `analysis/BATCH-01/sql/statements.csv`):

```sql
SHAPE {SELECT subgrouptable.subgroupcode, subgrouptable.subgroupname
       FROM subgrouptable GROUP BY subgrouptable.subgroupcode, subgrouptable.subgroupname
       ORDER BY subgroupname} AS cmdbill
APPEND ({SELECT * FROM testtable WHERE testorno = True AND testsimp <> ''
       ORDER BY testabrev} AS cmdbill_d RELATE subgroupcode TO subgroupcode) AS cmdbill_d
```

This is a **Jet-4-only** construct with no SQL Server equivalent; any port must re-implement the
grouped result set explicitly.

### 4.3 Where the SQL goes

`analysis/BATCH-04/sql_v1605/tables.csv` — V2 access profile, 117 tables, 1,699 attributed
statements:

| Table | Statements | SELECT | SHAPE | Distinct columns |
|---|---:|---:|---:|---:|
| `patientinfo` | 428 | 410 | 17 | 25 |
| `patienttest` | 343 | 312 | 3 | 33 |
| `normalrange` | 95 | 91 | 4 | 4 |
| `testtable` | 74 | 62 | 8 | 27 |
| `payment` | 32 | 29 | 0 | 6 |
| `comments` | 28 | 28 | 0 | — |
| `gehats` | 27 | 27 | 0 | 2 |
| `insulin` | 23 | 23 | 0 | — |
| `sharekathesab` | 19 | 19 | 0 | — |
| `patients` (SQL Server) | 14 | 13 | 0 | — |

`patientinfo` + `patienttest` = **43 %** of all SQL. The V1 profile has the same shape
(`patientinfo` 307, `patienttest` 138) at smaller volume.

---

## 5. Runtime, resources and integrations

### 5.1 Runtime dependency

The executable imports **`MSVBVM60.DLL` only** — the VB6 runtime — and nothing else. All other
system interaction goes through the VB6 runtime's late-bound dispatch (COM, ADO, Shell, file I/O).
**Architectural consequence:** the application's real dependency surface is not visible in the
import table; it is in the string pool.

### 5.2 Third-party components

| Counter | Value | Meaning |
|---|---:|---|
| VB header `wExternalComponentCount` | 21 (V1) / **25** (V2) | Declared external-component slots |
| Distinct third-party ActiveX controls | **19** | `MSComctl`, `MSComct2`, `MSChrt20`, `MSDataGrd`, `MSDataLst`, `MSAdoDc`, `TabCtl32`, `ComDlg32`, `RichTx32`, `MC`… |
| Component files inventoried | **25** | `.ocx` / `.oca` / `.tlb` / `.dll` |

The three counters measure different things; the knowledge base's "19 ActiveX controls" is the
distinct-control count.

### 5.3 Connection templates

Four connection templates are embedded: **two carry credentials**, two use SSPI. Credential
fragments are assembled at run time; the shipped fragment sets **`Persist Security Info=True`**.
The four databases the connection set addresses are `Patients` (the SQL Server catalogue),
`patientinfo`/`Access` (the Jet 4 mirror), `Interface` (a separately installed "RealLab Interface"
product) and `VIDAS` (never structurally characterised).

### 5.4 The SMS integration — reconstructed path

```
InternetExplorer.Application
  → http://smsmisr.com/api/send.aspx?username=…&password=…
```

The application drives **Windows Internet Explorer via COM** to call an **unencrypted HTTP** SMS
gateway, passing the username and password as query-string parameters. The V2 database adds
`SMSSender`, `SMStemplate`, `smsgroups` and `smslist` to support it.

**★ Security consequence.** Credentials traverse the network in clear text, and the HTTP call is
routed through a browser automation object rather than a proper HTTP client — a third-party SMS
provider is inside the trust boundary of a clinical system. See `business/SECURITY.md` V-19.

### 5.5 `NOTEX.dll` — an independent version witness

| | V-ROOT-1.00 | V-CD-2019 |
|---|---|---|
| `bin\NOTEX.dll` | 1.00, description empty, company `ais group` | **2.00**, description **`RLS Notes`**, company **`RealLab`** |
| `bin\NOTEXS.dll` | present (40,960 B) | **removed** |

The `NOTEX.dll` version bump and the company rename (`ais group` → `RealLab`) corroborate the
executable's own `VERSION` resource from a completely independent file.

---

## 6. What this evidence does and does not establish

| Claim | Status |
|---|---|
| Executable identity, size, hash, PE and VB6 header layout | **VERIFIED** |
| Object counts, type breakdown, method counts | **VERIFIED** (declaration level) |
| V1 → V2 object delta (41 added, 0 removed, 88 changed) | **VERIFIED** |
| SQL statement census (1,797 V2 / 1,337 V1) and per-table profile | **VERIFIED** |
| Embedded DDL census (268 = 27 + 228 + 13) | **VERIFIED** |
| Connection templates, SMS path, `PersistSecurityInfo` | **VERIFIED** (string-pool evidence) |
| **Method bodies / business logic of individual methods** | **NOT RECOVERED** — no VB6-aware decompiler was available; 1,268 of 8,026 entries read, no addresses |
| **Runtime behaviour of any window** | **NOT OBSERVED** — the corpus is 100 % static analysis |
| `RICHTX32.OCX` as a compiled library | **CONTRADICTED** — `richttx32-stub-pe-structure.txt` shows a 675-byte synthetic marker artefact with a bogus `e_lfanew` and the marker string `99111B9908`; `pe-summary.txt` mis-describes it as a real PE32 library |

**The single largest gap in the legacy knowledge base** is the absence of method-level
implementation evidence. Everything this document states about *behaviour* is inferred from
declarations, string literals, SQL and the official manual — never from decompiled code.

---

## 7. Related documents

- `Docs/KB/legacy/architecture/OVERVIEW.md` — five-layer architecture, modules, screens.
- `Docs/KB/legacy/architecture/DEPLOYMENT.md` — installers, engines, licensing, paths.
- `Docs/KB/legacy/architecture/CONFIGURATION.md` — `BASE.ini`, `SETTING.ini`, `Data\*.ini`.
- `Docs/KB/legacy/data/DATABASE.md` — the database specification.
- `Docs/KB/legacy/data/MIGRATION.md` — the 268-statement schema-migration engine.
- `Docs/KB/legacy/business/SECURITY.md` — credentials, permissions, vulnerabilities.
- `Docs/KB/legacy/SPECIFICATION.md` — the canonical reference-system specification.
