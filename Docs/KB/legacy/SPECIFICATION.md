# Legacy Reference System — Technical Specification (V-CD-2019 / v1.605)

**Purpose:** The canonical engineering description of the legacy reference system: what it is, how
it is built, what it stores, how it behaves, and how it integrates. This is the single authoritative
entry point for the reference system; every other legacy document is a specialised view of a
section of this one.
**Scope:** The complete `RealLab` clinical Laboratory Information System as reconstructed from the
reverse-engineering corpus, at its **latest identifiable version**.
**Audience:** Developers, architects, analysts, and the engineers planning the transformation of the
modern `Ref Lab` project.
**Version applicability:** **V-CD-2019 (v1.605) is the canonical target.** V-ROOT-1.00 is documented
as the historical baseline. Section 9 states exactly what is version-specific.
**Evidence:** `Docs/KB/legacy/evidence/SOURCES.md` (source traceability), `Docs/KB/legacy/evidence/COVERAGE.md`
(coverage), `Docs/KB/legacy/evidence/VALIDATION.md` (validation and open issues), `Docs/KB/legacy/evidence/CONTRADICTIONS.md`
(conflicts).

**Confidence vocabulary:** VERIFIED · CORROBORATED · INFERRED · PARTIALLY VERIFIED · UNRESOLVED ·
CONTRADICTED.

---

## 1. System identity

| Property | Value |
|---|---|
| Product | RealLab — `real lab system` |
| Vendor | `ais group` (V1) → **`RealLab`** (V2) |
| Latest identifiable version | **V-CD-2019**, product build **1.605**, installer build **19.00** (2020-09-01) |
| Historical version | **V-ROOT-1.00**, product build 1.00 |
| Technology | **Visual Basic 6 native-compiled** (32-bit PE32, `MSVBVM60.DLL` SP6 runtime build 9782) |
| Data engines | **Access Jet 4** (MDB 4.0) · **SQL Server Express 2005** · **MySQL** (Connector/ODBC 3.51) |
| Deployment shape | Client-server: shared SQL Server catalogue + a local Access mirror per client |
| Localisation | Arabic (Saudi), `dwLCID` 1033 / 4105 (ar-SA); V1 was en-US only |
| Manufacturer contact | `ais_soft_group@yahoo.com`, Tel 002 014 484 86 86 |
| Official documentation | `real lab system help.chm` — 17 topics, **byte-identical in both versions** |

**VERIFIED** by the PE `VERSION` resource (`FileDescription "RLS 1.605"`, `FileVersion 1.605`), the
`NOTEX.dll` 1.00→2.00 version bump with the company rename, and the `RL_SETUP.exe` v19.00
installer date — three independent witnesses.

**★ Only two file-bearing versions exist.** The `DEMO\RLS_Demo2011\RL_CD2019Access` tree contains
three empty directories and no files; it is not a version. The install-media CABs carry database
engines and runtime only — **no third application version** (ISS-01, closed).

**★ The PE timestamp does not discriminate.** Both executables carry **2013-12-27**. V1.605 was
never recompiled from source after 2013; only strings, resources and packaging changed. This is a
data-and-workflow evolution, not a rewrite. See §9.3.

---

## 2. Architecture

### 2.1 The five layers

```
┌───────────────────────────────────────────────────────────────────────────┐
│ L0  Presentation   real lab system.exe  72.76 MB · 3 sections · VB6       │
│     210 objects · 132 forms · 66 DataReports · 9 modules · 3 classes      │
│     8,026 declared methods · 31,429 strings · 1,797 SQL statements        │
├───────────────────────────────────────────────────────────────────────────┤
│ L1  Identity        132 form objects + 66 report templates                │
│     25 external-component slots · 19 distinct ActiveX controls            │
├───────────────────────────────────────────────────────────────────────────┤
│ L2  Logic           1,797 SQL statements · 268 embedded DDL statements    │
│     27 CREATE TABLE · 228 ALTER ADD · 13 ALTER COLUMN                    │
├───────────────────────────────────────────────────────────────────────────┤
│ L3  Data            103 tables / 1,682 columns / 3,809 rows (Access)      │
│     117 tables (SQL Server) · 49 branch-template tables · 0 PKs           │
├───────────────────────────────────────────────────────────────────────────┤
│ L4  Behaviour       15 documented capabilities (CHM) + ~60 vendor         │
│     capabilities in 7 modules · 41 new objects vs V1 · 88 changed        │
└───────────────────────────────────────────────────────────────────────────┘
```

### 2.2 The five modules

The vendor guide (`RLS_Learn.pdf`, 212 pages) and the CHM organise the product into functional
modules. The authoritative numbering is **4-x = users and attendance · 5-x = statistics ·
6-x = accounting and stock-take · 7-x = settings**, with the core laboratory operation
occupying the earlier chapters.

> **CORRECTION.** The previous knowledge base assigned "Server 4-1…4-6", "Accounting 5-1…5-5"
> and "General 6-1…9-7". The vendor guide's own table of contents assigns
> **statistics 5-1…5-4**, **accounting 6-1…6-5**, **settings 7-1…7-9**, and **users/attendance
> 4-1…4-3**. See `Docs/KB/legacy/evidence/CONTRADICTIONS.md` **C-18**.

### 2.3 Communication and cooperation

The system is a **fat-client** architecture. Each client owns:

- a local Access mirror (`Patients.mdb`) holding the patient and result data,
- a `data.mdb` holding the test catalogue, ranges, constants and prices,
- a `Branch_Data.mdb` template used to seed a branch,
- the 223 `Data\*.ini` report templates that define every printable report,
- `Cache\*.rtf` method sheets (29) and `Cache\*.exe` helpers (13) holding clinical reference data.

The **SQL Server catalogue is the shared truth**; the Access mirror is what the client actually
reads and writes. The embedded schema-migration DDL reconciles the client's mirror with the
catalogue at start-up — and, on the shipped copy, only partially (§6.4).

### 2.4 Companion processes

`Patient Data Transport.exe` (2 VB6 objects, 1.23 MB, dated 2011-01-04) carries data between the
client and the server. `real lab Image Gallery.exe` exists in V1 only. `RL_SETUP.exe` (57 objects,
599 methods) is a custom VB6 self-extractor, not a standard installer framework.

---

## 3. Application implementation

The complete object model, its version delta and the SQL literal census are in
**`Docs/KB/legacy/architecture/EXECUTABLE.md`**. Summary of what matters for re-implementation:

| Aspect | Finding |
|---|---|
| Object model | 210 compiled VB6 objects: 132 forms, 66 DataReports, 9 modules, 3 classes |
| Methods | 8,026 declared; **1,268 entries read, no method bodies recovered** |
| Version delta | +41 objects (0 removed), 88 objects with changed method counts, +296 net methods on those |
| Newly enabled objects | `antibioticfrm` (0→29), `moenysharekat` (0→27), `server` (0→17), `learntest` (0→6), `logindete` (0→6) |
| Largest growth | `addpatient` +67, `patientmain` +30, `users` +28 |
| SQL | 1,797 statements over 117 tables; 27 Jet-4 `SHAPE` statements |
| Runtime | `MSVBVM60.DLL` only; all other dependencies are late-bound and invisible in the import table |

**★ The implementation-level gap is method bodies.** No VB6-aware decompiler was available in the
analysis environment. Every behavioural statement in this knowledge base is inferred from
declarations, string literals, SQL, schema and the official manual. That constraint is inherited by
any transformation plan and is stated again in `mapping/TRANSFORMATION.md` §7.

---

## 4. Data model

Full specification: **`data/DATABASE.md`**. The essential shape:

| Database | Tables | Columns | Rows | Role |
|---|---:|---:|---:|---|
| `Patients.mdb` | **103** | **1,682** | **3,809** | Patient and result store |
| `Patients_backup.mdb` | 90 | 1,381 | 2,786 | Older structural ancestor |
| `Branch_Data.mdb` | 49 | 969 | 0 | Branch template |
| `data.mdb` | 3 | 34 | 0 | Catalogue, ranges, constants, prices |

- **96 of 103 tables are per-test result tables** (one per profile, one column per analyte).
- **0 of 103 tables declare a primary key.** There are no foreign keys, no views, no stored
  procedures, no triggers.
- `normalrange` (852 rows / 304 tests) is the safety-critical asset; `patienttest` (46 columns)
  freezes a copy of the range at order time.
- The shipped Access schema **does not match the executable that reads it**: 42 of the 132
  migration column targets were never applied.

---

## 5. Business logic

### 5.1 Clinical rules

45 V2 rules (`V2-BR-001…045`) and 49 V1 rules (`V1-BR-001…049`) are catalogued in
`Docs/Business_Rules_V2.md` and `Business_Rules_V1.md`. The clinical subset is rendered in
`clinical/RULES.md`. The load-bearing rules:

| Rule | Statement | Status |
|---|---|---|
| V1-BR-009 / V2-BR-034 | Result classification is driven by the `normalrange` table, not by hard-coded values | CONFIRMED |
| V1-BR-010 / V2-BR-035 | A critical limit overrides the abnormal classification | CONFIRMED |
| V1-BR-011 / V2-BR-036 | Reference ranges are banded by sex and age | CONFIRMED |
| V1-BR-012 | The abnormality exception text is stored on the range row | STRONGLY SUPPORTED |
| V1-BR-013 | A result comment may come from three distinct places | CONFIRMED |
| V1-BR-018 | Exception text and critical comment are stored per range row | CONFIRMED |
| V1-BR-019 | The urine analysis is bound to immutable test number 4 | CONFIRMED |
| V1-BR-035 | Critical thresholds are stated in the method sheet, separately from `normalrange` | **CONFIRMED — now verified at schema level** |
| V1-BR-037…040 | `Hct = Hgb × 3.3` · `Hgb%` factors · ISI · Westgard 2CV | CONFIRMED |
| V1-BR-029 | Unit conversions are stored per analyte-and-unit pair, not globally | CONFIRMED |
| V1-BR-046 | V1 has no PDF capability | CONFIRMED |

### 5.2 The classification model

Three-valued, ordered: **normal → abnormal (high / low) → critical**. A value breaching a
critical limit is presented as critical.

> **★ There is no critical-result escalation workflow.** No callback, no repeat, no critical-result
> log separate from the report. The critical state is **visual only**. For a clinical laboratory
> this is the single most important functional gap in the reference system. It is carried in
> `clinical/RULES.md` §5 and in `mapping/LEGACY-TO-MODERN.md` as gap V-12.

### 5.3 Reference-range administration — the operator-facing procedure

Derived from the official manual (topic 10, `008.htm`) — **the single most safety-critical screen
in the system**, and previously undocumented at this level.

**Navigation.** Login → main window → top toolbar **«بيانات النظام»** (System Data) →
**«بيانات التحاليل»** (Test Data) → select the test → **«المعدل الطبيعي»** (Normal Range) →
the range editor.

**The duplication rule.** Where a test does **not** differ by sex, the same range must be entered
**six times**:

| # | Band | Age window |
|---:|---|---|
| 1 | Male | 0–120 years |
| 2 | Female | 0–120 years |
| 3 | Male | 1–29 days |
| 4 | Female | 1–29 days |
| 5 | Male | 1–11 months |
| 6 | Female | 1–11 months |

Where the range does differ by age, the age window is split instead (e.g. male 0–12 and 12–120
years, and the female equivalents).

> **★ This explains the 852-row arithmetic.** 304 tests carry ranges, but not every test uses all
> three age units — a test with only a years-based band needs 2 rows, not 6. The row count is
> therefore a function of the *age-unit vocabulary per test*, not a fixed multiple of the test
> count. The knowledge base previously reported the population without this rule.

**Editor commands** (in order): **«إضافة مدى»** (Add Range) · **«تعديل»** (Edit) · **«حفظ»** (Save) ·
**«تراجع»** (Undo) · **«حذف»** (Delete) · **«قائمة التحاليل»** (Test List). There are **no keyboard
shortcuts** in this window.

**Range record fields:** test name · test unit · sex · age from · age to · age unit · normal-range
printed text · low limit · high limit · low flag · high flag · low comment · high comment ·
critical comment · critical range text · critical low limit · critical high limit · critical flag.

**Presentation behaviour.** An out-of-range result is rendered with **background text shading** in
the report. Flags appear during result entry and during printing. Comments appear when the test
name is clicked. The critical-range text prints only when *"print critical ranges"* is enabled in
the report settings.

**★ Schema conflict.** The shipped `normalrange` table has **18** columns and **no** `critical*`
column. The five critical fields documented above exist only in the `Patients_backup.mdb` ancestor
schema (22 columns). The shipped schema therefore **cannot persist** critical limits on the range
row — which is exactly why the product stores them in the `Cache\*.rtf` method sheets instead. See
`data/DATABASE.md` §6.1.

### 5.4 The clinical constants

`constanttable` is **row-oriented**: two rows, each holding a group name in `constantdiscrp` and up
to seven values in `constant1..7`. The `CBCCONS` and `PTCONS` groups reproduce the manual's
formulas exactly (values verified against the manual by the analysis evidence). They are
**live-editable** from the **«Constants» button at the bottom-right of the result-entry window** —
so the haematocrit and coagulation derivations are operator-configurable at runtime.

### 5.5 Identifier contract

The `patientid` is a **13-character positional string**: branch code at position 9, then a serial.
`labid` is a separate 13-character laboratory key. `patientinfo` also carries `barcode`. See
`data/IDENTIFIERS.md`.

---

## 6. Workflows

### 6.1 Patient lifecycle

```
Order patient  →  patientinfo (39 columns: identity, age, referral, commercial)
   ↓
Order tests    →  patienttest (one row per test; freezes the range snapshot)
   ↓
Enter results  →  per-profile table (patientid + requestdate + analyte columns + comment)
   ↓
Classify       →  normalrange lookup by testcode / sex / age window / ageunit / pregsmok
   ↓
Review         →  patienttest.resultreview = True
   ↓
Release        →  resultprint, printuser, printdate, printnum
   ↓
Hand over      →  give
   ↓
Send out       →  sentout / send + otherlabname (+ SMS via SMSSender)
```

State columns on `patienttest`: `resultfinsh` (entry complete) → `resultreview` (approved) →
`resultprint` (printed) → `give` (handed over) → `sentout` / `send` (sent out).

### 6.2 Result entry

Double-click a test in the patient's test list to open its entry window. If the test belongs to a
profile (بروفيل), **all** profile tests are displayed and only the selected ones are enabled. The
window shows the test's normal range, applies automatic classification, and exposes the live
**«Constants»** button.

### 6.3 Reference-range resolution

```sql
SELECT Top 1 * FROM normalrange
WHERE testcode = <n> AND sex = '<s>' AND ageunit = '<u>'
  AND firstold < <age> AND secondold >= <age> AND pregsmok = <p>
```

**with no `ORDER BY`.** Selection at an overlapping boundary is **engine-arbitrary**. The `REFRANGE`
column marks bands the runtime selects that are absent from the Access mirror. This is **C-14** and
is the highest-severity open question in the corpus.

### 6.4 Schema self-migration (V2 only)

268 DDL statements embedded in the executable, applied at start-up to the client's Access mirror.
The shipped copy shows the migration ran **partially**: 90 of 132 column targets are present, 42 are
not, and 3 of the 14 created tables (`FMF`, `IgEallergy`, `LBC`) are absent. Full detail in
`data/MIGRATION.md`.

### 6.5 Send-out and external laboratories

`testtable.inotherlab`, `otherlabname`, `otherlabprice` mark a test as outsourced;
`patienttest.sentout`, `send`, `otherlabname` record the movement. The **SMS** subsystem
(`SMSSender`, `SMStemplate`, `smsgroups`, `smslist`) delivers results to the patient via an external
gateway (§8.2).

### 6.6 Accounting and stock-take

Reached via **«حسابات» → «الجرد وحساب الدرج»**, or **F7** from any main window. Backed by
`payment`, `wasl`, `pricelist`, `patientinfo.totalprice` / `totalpricedisc` / `discount` /
`discount100` / `pricemethod`. See `business/WORKFLOWS.md`.

### 6.7 Backup

**«اعدادات» → «Database Maintenance»**, protected by a **maintenance password whose documented
default is `123`** (the manual advises changing it). Eight-part maintenance screen for large
installations: change the maintenance password, zero all or part of the system, select the database
path, list previous backups, count them, show their size, auto-name today's backup file
(**`Patient_yyyymmdd.bak`**, stored inside the database path), delete, and restore. Small
installations additionally allow **restoring a single patient by their work code** (كود الشغل).
The manual advises moving backups to a separate disc or machine.

---

## 7. Configuration and runtime behaviour

| Artefact | Count / size | Role |
|---|---|---|
| `BASE.ini` | 200 B (V1) → **381 B** (V2) | Server name/IP, database identity, credential fragments |
| `SETTING.ini` | 3,644 B → **4,886 B** | System settings |
| `Data\*.ini` | **223** per version | Per-report templates; 172 changed and 51 identical between V1 and V2 |
| `Cache\*.rtf` | **29** | Method sheets: reference ranges, critical values, sample and clinical notes |
| `Cache\*.exe` | **13** | Charting/analysis helpers |
| Fonts | 7 TTF | Presentation |
| `help.chm` | 5,193,985 B | The official manual — **byte-identical in both versions** |

**★ The configuration layer changed fundamentally; the runtime and documentation layers did not
change at all.** 43/43 ActiveX components, 42/42 training artefacts and the whole CHM are
byte-identical between V1.00 and v1.605. 174 files differ, concentrated in `Data\` and `bin\`.

### 7.1 Start-up sequence

1. The client reads `BASE.ini` for the server IP/machine name, the login-permission type, the
   database credentials and the database name. The manual states the permission type, the
   credentials and the database name are **fixed constants of the system** and must not be changed;
   only the server name varies per machine.
2. The server settings window offers a **«احضار»** (fetch) button for the server address when the
   NIC or cable fails.
3. The client waits for the database server; the manual advises allowing the machine to finish
   loading and then waiting **five seconds** for the server.
4. On a non-English Windows locale the application refuses to start with
   **«رجاء تحويل التاريخ الى اللغة الانجليزية»** — the documented remedy is to set the regional
   format to **English (Canada)**.

---

## 8. Interfaces and integrations

### 8.1 Database engines

| Engine | Role | Configuration |
|---|---|---|
| Access Jet 4 | The client's local mirror — what the application reads and writes | `Patients.mdb` |
| SQL Server Express 2005 | The shared catalogue; the multi-branch host | Mixed-mode auth, auto-start, network protocols **enabled**, hardcoded `sa` password |
| MySQL | Supported as a third engine | Connector/ODBC 3.51 |
| `Interface` catalogue | A separately installed "RealLab Interface" product | — |
| `VIDAS` | Referenced by the connection set; **never structurally characterised** | — |

### 8.2 External services

| Integration | Mechanism | Status |
|---|---|---|
| SMS delivery | `InternetExplorer.Application` → `http://smsmisr.com/api/send.aspx?username=…&password=…` (**plain HTTP**, credentials in the query string) | V2 only |
| Remote support | `AnyDesk.exe`, `Supremo.exe`, `TeamViewerQS.exe` shipped on the 2019 disc | V2 only |
| SQL Server | ODBC / ADO | — |
| CHM help | Local `help.chm` | — |

### 8.3 Internal interfaces

`Patient Data Transport.exe` moves data between client and server. The `Data\*.ini` template set is
the internal contract between the report engine and the clinical configuration: **223 templates**,
of which 172 changed between versions.

---

## 9. Version model

### 9.1 The two versions

| | V-ROOT-1.00 | V-CD-2019 (v1.605) |
|---|---|---|
| Evidence class | STRONGLY SUPPORTED | **CONFIRMED** |
| Files | 364 (207.78 MiB) | **473 (532.61 MiB)** |
| Exe version | 1.00 | **1.605** |
| Installer | none present | `RL_SETUP.exe` 19.00 (2020-09-01) |
| Tables / columns / rows | 93 / 1,527 / 3,123 | **103 / 1,682 / 3,809** |
| Objects / methods | 169 / 6,528 | **210 / 8,026** |
| Embedded DDL | 0 | **268** |
| SQL statements | 1,337 | **1,797** |
| `NOTEX.dll` | 1.00 | **2.00** |
| LCID | en-US only | **ar-SA** |
| PDF printing | absent | **Bullzip PDF Printer path** |
| SMS / NATIGH delivery | absent | **present** |
| Branch model | single site | **master + reception branches** |
| Remote-support tools | absent | **present** |

**837 files inventoried in total; 837 verified; 0 unassigned; 11 of 11 batches accepted.**

### 9.2 Feature introduction and removal in V2

**Introduced:** SMS delivery and its four tables · schema self-migration (268 DDL statements) ·
multi-branch support and `Branch_Data.mdb` · PDF/print architecture (Bullzip) · the
`Helicobacter`, `insulin`, `LBC`, `IgEallergy`, `PCR` and `FMF` result windows · `MyLabSetting` ·
the `patienttestgroup` grouped ordering · 41 new objects · remote-support tooling.

**Removed:** `NOTEXS.dll` · `real lab Image Gallery.exe` · `real lab system.exe`,
`real lab system help.chm` and `real lab Image Gallery.exe` from the V1 root layout (moved, not
deleted) · **0 VB6 objects removed**.

**Modified:** 88 objects with changed method counts (+296 net) · 10 tables added, 0 removed,
155 columns added · 10 column-type changes · 172 of 223 `Data\*.ini` templates rewritten · the
`data.mdb` file grown by exactly one Access page.

### 9.3 What the delta means

> **★ RealLab evolved the data and workflow layer only.** The runtime layer (43/43 ActiveX),
> the training layer (42/42) and the entire documentation layer (`help.chm`, byte-identical) were
> **frozen at 2010–2011**. The CHM therefore documents **V1 only, in both versions**.

### 9.4 Version-relevance uncertainties

| Question | Status |
|---|---|
| Does a 2011 demo version exist? | **Possible historically, not established.** The `DEMO` tree is empty on disk |
| Does any CAB carry a third application build? | **No** — ISS-01 closed |
| What is `VIDAS`? | **UNKNOWN.** BATCH-10 hypothesises Visual FoxPro 6.0 |
| Runtime behaviour of either version | **NOT OBSERVED** — the corpus is 100 % static |
| Version attribution of 709/837 files (84.7 %) | Rests on **path-prefix rules**, not in-file evidence |

---

## 10. Security and data protection

| Finding | Detail | Evidence status |
|---|---|---|
| V-01 | The `sa` password is **clear text** in `BASE.ini`, in the UDL files and in the installer (`SAPWD=12345678`) | CONFIRMED |
| V-02 | The `admin` account's password **equals its user name**; the manual confirms this as the first-time default | CONFIRMED |
| V-03 | `PersistSecurityInfo=True` is set in the shipped connection fragment | CONFIRMED |
| V-04 | A single shared, immutable credential across all clients | CONFIRMED |
| V-05 | V2 logs the last 10 connection attempts **including the password** (`[lastlog]`) | CONFIRMED |
| V-06 | No password hashing, salting, expiry, lockout or session timeout | CONFIRMED |
| V-07 | The **maintenance password defaults to `123`** | CONFIRMED (manual) |
| V-08 | SMS credentials traverse the network in **plain HTTP** and are routed through browser automation | CONFIRMED |
| V-09 | The manual advises **keeping the server off the internet** — contradicted by the NATIGH/SMS feature that requires internet | CONTRADICTION C-01 |
| V-10 | 0 primary keys and no foreign keys; referential integrity is application-enforced only | CONFIRMED |
| V-11 | Patient identifiers, names, phones and addresses are stored in clear text | CONFIRMED |
| V-12 | **No critical-result escalation**: the critical state is visual only | CONFIRMED |
| V-13 | Remote-support executables (`AnyDesk`, `Supremo`, `TeamViewerQS`) ship on the distribution disc | CONFIRMED |
| V-14 | The `patientid` embeds the branch code at a fixed position — an information leak across branches | CONFIRMED |

**Do not read more into this than the evidence supports.** No statement is made here about
cryptographic strength, patching posture or network exposure, none of which the static corpus can
establish.

---

## 11. Documentation map

| Subject | Canonical document |
|---|---|
| **This document** | `Docs/KB/legacy/SPECIFICATION.md` |
| Architecture | `architecture/OVERVIEW.md` · `architecture/EXECUTABLE.md` |
| Deployment | `architecture/DEPLOYMENT.md` |
| Configuration | `architecture/CONFIGURATION.md` |
| PDF and printing | `architecture/PDF-PRINTING.md` |
| Database | `data/DATABASE.md` · `data/MODEL.md` · `data/MIGRATION.md` |
| Identifiers and branches | `data/IDENTIFIERS.md` · `data/BRANCHES.md` |
| Clinical | `clinical/RULES.md` · `clinical/KNOWLEDGE-LAYER.md` · `clinical/PROFILES.md` |
| Business | `business/WORKFLOWS.md` · `business/SECURITY.md` · `business/CAPABILITIES.md` |
| Versions | `VERSIONS.md` |
| Evidence | `Docs/KB/legacy/evidence/SOURCES.md` · `Docs/KB/legacy/evidence/COVERAGE.md` · `Docs/KB/legacy/evidence/EVIDENCE-REGISTER.md` · `Docs/KB/legacy/evidence/CONTRADICTIONS.md` · `Docs/KB/legacy/evidence/UNRESOLVED.md` · `Docs/KB/legacy/evidence/VALIDATION.md` |
| Transformation | `mapping/GAP-ANALYSIS.md` · `mapping/TRANSFORMATION.md` · `mapping/LEGACY-TO-MODERN.md` |
| Governance | `INDEX.md` · `MAINTENANCE.md` |
