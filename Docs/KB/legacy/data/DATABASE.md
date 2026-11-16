# Legacy Database Specification — V-CD-2019 (v1.605)

**Purpose:** Consolidated, evidence-backed reference for the legacy Jet 4 / SQL Server schema, its
evolution from V-ROOT-1.00, its embedded self-migration engine, and the link between each table and
the application behaviour it supports.
**Scope:** The complete physical data model of the legacy reference system: databases, tables,
columns, types, nullability, row populations, indexes, relationships, the runtime schema-migration
DDL, and the per-table SQL usage profile.
**Audience:** Developers, DBAs, architects planning the transformation.
**Version applicability:** V-CD-2019 (v1.605) is the canonical target. V-ROOT-1.00 figures are given
as the comparison baseline. Where the two differ, the difference is marked **VERSION DELTA**.
**Canonical subject:** this document. `data/MODEL.md` is the navigational summary; `data/MIGRATION.md`
covers the migration mechanism in depth; `clinical/RULES.md` covers clinical semantics.

**Confidence vocabulary:** VERIFIED · CORROBORATED · INFERRED · PARTIALLY VERIFIED · UNRESOLVED ·
CONTRADICTED (see `Docs/KB/legacy/evidence/VALIDATION.md`).

---

## 1. Source basis

Every figure in this document is derived from primary schema evidence, not from prose descriptions:

| Evidence | Archive path | What it proves |
|---|---|---|
| V1 `Patients.mdb` DDL | `txt nd sql/v1-Patients.mdb.sql` | V-ROOT-1.00 live schema |
| V2 `Patients.mdb` DDL | `txt nd sql/v2019-Patients.mdb.sql` | V-CD-2019 live schema |
| `Patients_backup.mdb` DDL | `txt nd sql/schema__[analysis_BATCH-02_db_Patients_backup].sql` | An older structural ancestor |
| `Branch_Data.mdb` DDL | `txt nd sql/schema-Branch_Data.sql` (= `schema.sql`) | Branch template schema |
| `data.mdb` DDL | `txt nd sql/v1-data.mdb.sql` (= `v2019-data.mdb.sql`) | System-data schema |
| Embedded DDL | `txt nd sql/ddl-all.txt` (22,216 B) | The 268 runtime migration statements |
| Migration reconciliation | `analysis/BATCH-07/migration-proof.json` + `.md` | Which DDL statements actually landed |
| Table/column census | `analysis/BATCH-02/db/Patients/rowcounts.csv`, `…/columns.csv` | Row and column populations |
| Role classification | `analysis/CUMULATIVE/cumulative-schema.csv` (133 rows) | Business role per table |
| SQL usage profile | `analysis/BATCH-04/sql_v1605/tables.csv` (117 rows) | Which tables the application touches, and how hard |
| Schema evolution | `analysis/BATCH-04/schema/column-evolution.csv` (103 rows) | Per-table column delta |
| Archive census | `analysis/BATCH-02/db/db-summary.md`, `table-overlap.csv` | Database-level inventory |

All `.sql` files are `mdbtools` schema dumps (`mdb-schema`). The archive paths are given relative to
the supplied `txt nd sql.zip` and the `RealLab-Deliverables-B01-B11.zip` archives; see
`Docs/KB/legacy/evidence/SOURCES.md` for the full disposition register and
`Docs/KB/legacy/evidence/COVERAGE.md` for how each source was integrated.

---

## 2. Database topology

The reference system ships **four Jet 4 (Access 2000 / MDB 4.0) databases** alongside a SQL Server
Express 2005 catalogue.

| Database | File size (V1 → V2) | Tables | Columns | Rows | Role |
|---|---:|---:|---:|---:|---|
| `Patients.mdb` | 2,531,328 → **2,682,880 B** | 93 → **103** | 1,527 → **1,682** | 3,123 → **3,809** | Patient and result store; the live clinical database |
| `Patients_backup.mdb` | 2,146,304 B (identical in both CDs) | 90 | 1,381 | 2,786 | Structural ancestor; byte-identical across both releases |
| `Branch_Data.mdb` | 970,752 B (`bin\`) + 966,656 B (`System\`) | 49 | 969 | 0 | Empty branch template; seeds a new branch |
| `data.mdb` | 249,856 → **266,240 B** (+16,384 = one Access page) | 3 | 34 | 0 | Test catalogue, ranges, constants, prices |
| SQL Server catalogue `Patients` | — | 117 | — | — | The V2 multi-engine target; `Interface` and `VIDAS` are separate catalogues |

**VERIFIED.** Table/column/row counts are recomputed from the DDL and the `rowcounts.csv` export.

### 2.1 Database overlap

`Patients_backup.mdb` and `Branch_Data.mdb` are **not independent schemas**:

- All **49** `Branch_Data.mdb` table names also exist in `Patients.mdb` — in **both** V1 and V2.
  `Branch_Data.mdb` is therefore a *branch template* holding a subset of the live schema, empty of
  data. Its value is as a reproducible starting point for a new branch, not as a distinct model.
- `Patients_backup.mdb`'s 90 tables are a **subset of neither** V1 nor V2 `Patients.mdb`; it is a
  genuine older generation (see §6.1).
- `patientinfo` exists in **four** databases (`Patients.mdb`, `Patients_backup.mdb`,
  `Branch_Data.mdb`, `data.mdb`) — the single most duplicated name in the system.

### 2.2 `data.mdb` — the three system-data tables

| Table | Columns | Role |
|---|---:|---|
| `history` | 20 | Generic 20-column dynamic history table (`column1..column20`); one row per captured event |
| `patientinfo` | 10 | Patient master stub (empty in `data.mdb`) |
| `wasl` | 4 | Work-analysis list with prices (empty in `data.mdb`) |

`v1-data.mdb.sql` and `v2019-data.mdb.sql` are **byte-identical**: the `data.mdb` *schema* did not
change between versions even though the file grew by exactly 16,384 bytes. **INFERRED:** the growth
is data, not structure.

---

## 3. The live clinical schema — V-CD-2019 `Patients.mdb`

### 3.1 Core clinical tables

**`normalrange` — 18 columns, 852 rows, 304 distinct tests.**
This is the system's principal safety-critical asset.

| Column | Type | Purpose |
|---|---|---|
| `normalid` | Long Integer | Surrogate key (**no primary index** — see §7) |
| `testcode` | Long Integer | FK to `testtable.testcode` |
| `testsimp` | Text (50) | Simplified test name carried on the range row |
| `subgroupcode` | Integer | Profile / sub-group FK |
| `sex` | Text (10) | `Male` · `Female` · `All` · empty |
| `firstold`, `secondold` | Double | Age window bounds |
| `ageunit` | Text (10) | `Years` · `Days` · `Mons.` · empty |
| `pregsmok` | Single | Pregnancy/smoking dimension — populated on **all 852** rows |
| `normalrang` | Text (255) | The text printed in the report's "normal range" cell |
| `testunit` | Text (15) | Unit shown beside the result |
| `lowlimit`, `highlimit` | Double | Numeric bounds used for classification |
| `nolowexe`, `nohighexe` | Text (20) | Exception text stored on the range row |
| `lowcomment`, `highcomment` | Memo | Result comments |
| `REFRANGE` | Text (1) | Runtime-band marker (see §5.4) |

Populations (VERIFIED, recomputed from `normalrange-stats.json` and independently from the export):
852 rows · 304 tests · sex `Male` 422 / `Female` 421 / `All` 5 / empty 4 · age unit `Years` 708 /
`Days` 70 / `Mons.` 65 / empty 9 · age span Years 0–120, Days 1–120, Mons. 0–24 · numeric low
limit on 459 rows, high limit on 643 · exception text 511 low / 581 high · 19 distinct low and 19
distinct high comments · **51 distinct measurement units** · `pregsmok` populated on all 852 rows.

> **★ There is no `critical*` column on `normalrange` in the live schema.** The `Patients_backup.mdb`
> ancestor carries `criticalrang`, `lowlcrit`, `highlcrit`, `criticalexe` and `criticalcomment`
> (see §6.1). The official manual nevertheless documents critical-limit fields on this screen. The
> structural consequence — critical thresholds are therefore stored in the **method sheets**
> (`Cache\*.rtf`), not in the database — is recorded as **V1-BR-035** in `clinical/RULES.md` and is
> now **VERIFIED at schema level**.

**`patienttest` — 46 columns.** The test-order / result index: one row per ordered test.

Its 46 columns include the full result lifecycle (`resultfinsh`, `resultreview`, `resultprint`,
`give`, `sentout`, `send`), the print audit (`printuser`, `printdate`, `printnum`, `edituser`,
`editdate`, `prevuser`, `prevdate`, `taslimuser`, `taslimdate`), the price triple (`testprice`,
`testpriceout`, `testpriceoutmadfo`), and — critically — the **frozen reference-range snapshot**:
`normalrang`, `testunit`, `lowlimit`, `highlimit`, `nolowexe`, `nohighexe`. A range change made
after ordering therefore does **not** affect an already-ordered test.

**`patientinfo` — 39 columns.** Patient master: identity (`patientnamear`, `patientsefa`,
`patientsex`, `patientaddress`, `patientphone`, `patienttel`, `email`, `barcode`), the 13-character
`patientid`, `labid`, referral (`referredbysefa`, `referredbyname`, `gehaName`), age
(`patientage`, `patientageunit`, `PBirthDay`), and commercial fields (`pricemethod`, `totalprice`,
`totalpricedisc`, `discount`, `discount100`, `mowzaf`, `patientprice`).

**`testtable` — 38 columns, 609 tests.** The test catalogue. 94 active · 591 carry a patient price ·
605 delete-protected · 48 distinct sub-groups in use · three barcodes (`barcode1..3`) · per-test
prices (`patientprice`, `labprice`, `otherlabprice`) · `normaltype`, `addorno`, `testorno`,
`printwith`, `seereport`, `branch`, `active`, `inotherlab`, `deleteprotect`.

### 3.2 Knowledge and configuration tables

| Table | Columns | Rows | Role |
|---|---:|---:|---|
| `constanttable` | 8 | 2 | Clinical constants, **row-oriented**: `constantdiscrp` holds the group name, `constant1..7` the values |
| `converts` | 6 | 167 | Unit conversions, per analyte-and-unit pair: `testsimp`, `Funit`, `Sunit`, `Factors`, `Ffix`, `Sfix` |
| `abbreviations` | 3 | 650 | Abbreviation → meaning (`abbid`, `abbre`, `mean`) |
| `subgrouptable` | 4 | 48 used | Profile catalogue (`subgroupcode`, `subgroupname`, `arname`, `deleteprotect`) |
| `comments` | 5 | 5 | Shared result comments (`commentid`, `testcode`, `subgroupcode`, `testsimp`, `comments`) |
| `testunits` | 2 | — | Unit dictionary (`unitsid`, `unitsname`) |
| `wasl` | 4 | — | Work-analysis list with prices |
| `history` | 20 | — | Generic 20-column dynamic history |

`constanttable` in full (VERIFIED against the manual via `constants-mapped.csv`):

| Group (`constantdiscrp`) | Slot | Value | Formula | Matches manual |
|---|---|---:|---|---|
| `PTCONS` | constant1 | 12 | PT control time (primary) | yes |
| `PTCONS` | constant2 | 35 | PT control time (secondary) | yes |
| `PTCONS` | constant3 | 1.33 | ISI | yes |
| `PTCONS` | constant4 | `2CVJW` | Westgard 2CV rejection rule | yes |
| `CBCCONS` | constant1 | 3.3 | `Hct = Hgb × 3.3` | yes |
| `CBCCONS` | constant2 | 6.25 | `Hgb% = Hgb × 6.25` (male, age > 12) | yes |
| `CBCCONS` | constant3 | 6.75 | `Hgb% = Hgb × 6.75` (female, age > 12) | yes |
| `CBCCONS` | constant4 | 7.5 | `Hgb% = Hgb × 7.50` (age 1–12) | yes |
| `CBCCONS` | constant5 | 8.25 | `Hgb% = Hgb × 8.25` (age < 1) | yes |

This is a **triple-corroborated** fact: the database values, the official manual formulas and the
KB's `clinical/RULES.md` §3 table agree exactly.

### 3.3 Profile (per-test) result tables — 49 tables

96 of the 103 tables are **per-test result tables**; 33 are configuration/master tables
(`cumulative-schema.csv`). The profile tables share a common shape and are the reason the system has
49 of them:

| Column | Type | Meaning |
|---|---|---|
| `patientid` | Text (13) NOT NULL | Patient key |
| `requestdate` | DateTime | Order date |
| *analyte columns* | varied | One column per analyte, named per the profile |
| `comment` | Text (255) | Free-text result comment |

`branch-profile-schemas.csv` gives the full 49-row profile-to-analyte mapping (column count and
analyte list per profile). Examples: `Abuse` → 7 analytes (`Alcohol`, `Cannabinoid`, `Opitate`,
`Cocaine`, `Barbtiurate`, `Benzodiazipine`, `Amphetamine`); `ABO` → 5 (`abo`, `rhfactor`, `direct`,
`indirect`, `rhtitre`).

**Architectural consequence.** 96 near-identical tables with per-analyte columns cannot be expressed
as a relational schema without either (a) 96 tables, (b) a generic EAV result table, or (c) a
profile-per-analyte JSON/child-table model. The modern system has chosen (c) — see
`Docs/KB/legacy/mapping/GAP-ANALYSIS.md` §4.

### 3.4 Table inventory by role

| Role | Count | Representative tables |
|---|---:|---|
| Per-test result table (profile) | 96 | `ABO`, `cbc`, `cultures`, `urine`, `semen`, `lipids` |
| Configuration / master data | 33 | `testtable`, `testunits`, `subgrouptable`, `abbreviations`, `converts`, `constanttable`, `comments`, `pricelist`, `wasl` |
| Patient master | 1 | `patientinfo` |
| Test order / result index | 1 | `patienttest` |
| Generic dynamic history | 1 | `history` |
| Work-analysis list with prices | 1 | `wasl` |

---

## 4. Version delta — V-ROOT-1.00 → V-CD-2019

### 4.1 Structural delta

| Metric | V1 | V2 | Delta |
|---|---:|---:|---:|
| Tables | 93 | 103 | **+10** |
| Columns | 1,527 | 1,682 | **+155** |
| Rows | 3,123 | 3,809 | **+686** |
| `data.mdb` size | 249,856 B | 266,240 B | +16,384 B (one Access page) |

### 4.2 The 10 tables added in V2

| Table | Columns | Created by the embedded DDL? | Function |
|---|---:|---|---|
| `Helicobacter` | 6 | **yes** | H. pylori IgG/IgM/Ag results |
| `MyLabSetting` | 20 | **yes** | Per-laboratory settings: `LabID`, `sendername`, `accuntID`, `accuntpw`, `updateflage`, `Hourflag` … |
| `SMSSender` | 8 | **yes** | Outbound SMS queue: `SMSautonum`, `patientid`, `PatDrName`, `smsMobile`, `SMStext` |
| `SMStemplate` | 2 | **yes** | SMS body templates |
| `smsgroups` | 3 | **yes** | SMS recipient groups |
| `smslist` | 6 | **yes** | SMS recipient list (`smsphone`, `smsname`, `smsgroup`, `send`) |
| `testtemp` | 28 | **yes** | Scratch result table with the full range snapshot |
| `insulin` | 2 | **no** | Insulin result lookup |
| `patienttestgroup` | 46 | **no** | Grouped test order — V2 group ordering, also carries `nolowexe`/`nohighexe` |
| `Paste Errors` | 6 | **no** | Clipboard-paste error landing table (`Field0..Field5`) |

**★ Three of the ten were added directly to the Access file, not by the runtime migration.**
This is only detectable by comparing the shipped DDL against the embedded DDL — and it is new
knowledge not present in the previous knowledge base.

### 4.3 Net column-type changes on shared tables (10)

| Table | Column | V1 | V2 |
|---|---|---|---|
| `PTC` | `concent` | Integer NOT NULL | Double NOT NULL |
| `cultureanti` | `Sensitivityvalue` | Double | Text (10) |
| `cultures` | `organism` | Text (50) | Text (80) |
| `cultures` | `organism2` | Text (50) | Text (80) |
| `normalrange` | `normalrang` | Memo/Hyperlink (255) | Text (255) |
| `patienttest` | `normalrang` | Memo/Hyperlink (255) | Text (255) |
| `patientinfo` | `patientid` | Text (13) NOT NULL | Text (13) |
| `widal` | `patientid` | Text (13) NOT NULL | Text (13) |
| `semen` | `Viscosity` | Text (10) | Text (15) |
| `semen` | `Liquefaction` | Text (5) | Text (10) |

Two of these are **data-integrity relevant**: `patientinfo.patientid` and `widal.patientid` lose
their `NOT NULL` constraint in V2. `cultures.organism3` is created at Text (80) by the DDL, matching
the "third organism" feature confirmed across three independent layers.

### 4.4 V2-only additions in the executable

41 new VB6 objects, 0 removed, 88 objects with changed method counts (see
`Docs/KB/legacy/architecture/EXECUTABLE.md`). The database-side counterpart is the SMS subsystem
(`SMSSender`/`SMStemplate`/`smsgroups`/`smslist`), `MyLabSetting`, the `Helicobacter` profile, the
IgE-allergy and LBC and PCR and FMF result windows, and the `patienttestgroup` grouped ordering.

---

## 5. The embedded schema-migration engine

### 5.1 Statement census — corrected

The executable embeds **268** DDL statements in its string pool (`ddl-all.txt`, header
`# 268 statements (CREATE/ALTER) found in the executable's string pool`).

| Statement class | Count | Distinct targets |
|---|---:|---|
| `CREATE TABLE` | **27** | **14** distinct tables |
| `ALTER TABLE … ADD …` | **228** | **132** distinct (table, column) pairs across **29** tables |
| `ALTER TABLE … ALTER COLUMN …` | **13** | 8 distinct tables, 10 distinct columns |
| **Total** | **268** | 31 distinct tables touched by any `ALTER` |

> **CORRECTION.** The previous knowledge base stated "268 embedded DDL statements (**12** CREATE
> TABLE, 228 ALTER ADD, 13 ALTER COLUMN)". That breakdown sums to **253**, not 268, and the true
> statement-class count for `CREATE TABLE` is **27**. The figure **12** is the number of `CREATE`
> *statement rows* that create a table genuinely absent from V1 — it is not a statement-class count.
> See `Docs/KB/legacy/evidence/CONTRADICTIONS.md` **C-16**.

The 14 tables named by `CREATE TABLE` are: `FMF`, `Helicobacter`, `IgEallergy`, `LBC`,
`MyLabSetting`, `SMSSender`, `SMStemplate`, `hba1c`, `pictures`, `ratios`, `smsgroups`, `smslist`,
`testtemp`, `widal2`.

### 5.2 Dual-dialect emission

Each object is emitted in both a SQL Server and an Access form, so a Jet 4 installation fails the
SQL-Server statement and falls through to the Access-compatible one:

| Dialect marker | `CREATE` | `ALTER ADD` |
|---|---:|---:|
| `bigint IDENTITY(1,1) NOT NULL` (SQL Server) | 4 | 2 |
| `AUTOINCREMENT(1, 1)` (Access) | 4 | 2 |
| `varchar(MAX)` (SQL Server 2005+) | — | paired with `Memo` / `varchar(255)` |

Concrete pair: `Create Table SMSSender (SMSautonum bigint IDENTITY(1,1) NOT NULL, …)` **and**
`Create Table SMSSender (SMSautonum AUTOINCREMENT(1, 1), …)`.

Where only the SQL Server form exists, the change is **silently not applied** on a Jet 4
installation. **VERIFIED** from `ddl-all.txt`.

### 5.3 Reconciliation against the shipped V2 database — corrected

**CREATE TABLE (27 statement rows → 14 distinct tables):**

| Verdict | Statement rows | Distinct tables | Tables |
|---|---:|---:|---|
| CREATED-BY-MIGRATION | 12 | **7** | `Helicobacter`, `MyLabSetting`, `SMSSender`, `SMStemplate`, `smsgroups`, `smslist`, `testtemp` |
| ALREADY-EXISTED | 9 | **4** | `hba1c`, `pictures`, `ratios`, `widal2` |
| ABSENT from the shipped DB | 6 | **3** | **`FMF`**, **`IgEallergy`**, **`LBC`** |

**ALTER TABLE ADD (228 statement rows → 132 distinct targets across 29 tables):**

| Verdict | Statement rows | Distinct targets |
|---|---:|---:|
| PRE-EXISTED | 145 | 74 |
| ADDED-BY-MIGRATION | 22 | 16 |
| NOT-APPLIED | 61 | **42** |
| **Present in the shipped V2 database** | 167 | **90** (across 18 tables) |
| **Absent from the shipped V2 database** | 61 | **42** (across 14 tables) |

> **CORRECTION.** The previous knowledge base stated "**109 of 132** column targets applied" and
> "**19** column targets … were never applied", and listed 23 columns in a table captioned "19".
> The reconciliation evidence (`migration-proof.json`) gives **90 of 132 present** and **42
> absent**. See `Docs/KB/legacy/evidence/CONTRADICTIONS.md` **C-17**.

**The 42 absent column targets, by table:**

| Table | Absent columns | Reason |
|---|---|---|
| `BloodGases` | `Aniongap`, `FiO2`, `HCO3std`, `TempRAT`, `ctCO2`, `ctHB`, `pO2AA1`, `pO2AA2` | Whole blood-gas extension never applied |
| `cultures` | `Few5`, `Few6`, `Few7`, `Few8` | Culture sensitivity extension never applied |
| `prostaticx` | `crystalp`, `crystalp2`, `crystalp2r`, `crystalpr` | Crystal-pattern extension never applied |
| `urine` | `T56`, `T57`, `testid` | Urine extension never applied |
| `Patients` | `SecWord`, `branche`, `grant13`, `grant14`, `grant15`, `grant16`, `grant17` | **SQL-Server-only table** (the Access table is `patientinfo`) |
| `useraccounts` | `Gtype`, `HDD1`, `HDD2` | **SQL-Server-only**; table absent from Access entirely |
| `IgEallergy` | `Item12`, `Item24`, `Item36` | **SQL-Server-only**; table absent from Access entirely |
| `Helicobacter` | `hpab`, `hpbiop`, `hpiga` | Table created, but 3 extended columns not applied |
| `Proelectrophoresis` | `Beta2`, `immunolgo` | Never applied |
| `pregnancy` | `normalrange3` | Never applied |
| `patientinfo` | `gehaname` | Never applied |
| `patienttest` | `NatighU` | **NATIGH delivery flag has no column** |
| `lipids` | `nonhdl` | Never applied |
| `Kidney` | `egfr` | Never applied |

23 of the 61 unapplied statements target SQL-Server-only tables (`Patients`, `useraccounts`,
`IgEallergy`, `Kidney`, `lipids`) and **cannot** apply to a Jet 4 database at all; the remaining 38
are columns in existing Access tables that were never added.

### 5.4 `REFRANGE` — the runtime-band marker

`REFRANGE Text (1)` exists on both `normalrange` and `testtable`. The migration evidence and the
Phase-2 findings record that rows with `REFRANGE = '2'` are **runtime-selected bands that are absent
from the Access mirror**. This is the concrete mechanism behind the band-selection uncertainty
recorded as **C-05** and **C-14**: the runtime may select a band row that the shipped Access copy
does not contain. **PARTIALLY VERIFIED** — the column and the `'2'` marker are VERIFIED; the exact
selection semantics remain UNRESOLVED (see `Docs/KB/legacy/evidence/UNRESOLVED.md`).

---

## 6. Structural ancestors and dead ends

### 6.1 `Patients_backup.mdb` — an older structural generation

`Patients_backup.mdb` is byte-identical in both the V1 and the 2019 CD. Its `normalrange` has
**22** columns, including five that the live schema does not have:

```
criticalrang, lowlcrit, highlcrit, criticalexe, criticalcomment
```

and it lacks `REFRANGE`.

**★ This is the mechanism behind V1-BR-035.** The official manual documents a reference-range
editor with critical-limit, critical-flag and critical-comment fields. The **live** V1 and V2
schemas have nowhere to store them; the **backup** schema does. The critical thresholds therefore
live in the `Cache\*.rtf` method sheets, and the manual's critical-limit UI cannot persist into the
shipped database. The previous knowledge base stated this as a documentation-derived rule; it is now
**VERIFIED at schema level**.

### 6.2 The `RL_SETUP.exe` database engine

`RL_SETUP.exe` (v19.00, dated 2020-09-01) installs SQL Server Express 2005 with **mixed-mode
authentication**, **auto-start enabled**, **network protocols enabled**, and a hardcoded `sa`
password — see `business/SECURITY.md` V-01.

### 6.3 `VIDAS` — never characterised

A third attached database, `VIDAS`, is referenced by the connection templates but was **never
structurally characterised**; the Phase-0 gap register records it as KNOWN-UNANSWERABLE. A BATCH-10
hypothesis proposes Visual FoxPro 6.0, which would put it outside the reach of `mdb-schema`
entirely. **UNRESOLVED.**

---

## 7. Keys, indexes and integrity constraints

| Property | Finding | Evidence |
|---|---|---|
| Primary keys | **0 of the 103 user tables declare a primary index.** The remaining 103 have **no primary key at all** | `Database documentation.md` §10, `evidence/reverse-engineering/metadata/adox_indexes.csv` |
| Foreign keys | **None declared.** Referential integrity is enforced only in application code | `adox_relations.csv` is 0 bytes; `adox_procedures.csv` is 0 bytes |
| Views / procedures / triggers | **None.** `adox_procedures.csv` and `adox_relations.csv` are empty | `evidence/reverse-engineering/metadata/` |
| Identity generation | Only in the DDL (`IDENTITY`/`AUTOINCREMENT` on `SMSSender.SMSautonum`, `SMStemplate.templatenum`, `smsgroups.ID`, `smslist.ID`) | `ddl-all.txt` |
| Data integrity | Enforced by application logic and by the frozen range snapshot on `patienttest` | §3.1 |

**★ Architectural consequence for the transformation.** The legacy system has **no referential
integrity, no primary keys, and no database-side business logic**. Every constraint the modern
system relies on — PKs, FKs, check constraints, cascade behaviour — is new engineering, not a
port. `mapping/TRANSFORMATION.md` treats this as the single largest structural risk.

---

## 8. Table-to-application behaviour link

The V2 executable issues **1,797 SQL statements**; `analysis/BATCH-04/sql_v1605/tables.csv` records
which tables they touch.

| Table | Statements | SELECT | SHAPE | Distinct columns touched |
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

117 distinct tables are touched in total. `patientinfo` + `patienttest` alone account for **43 %** of
all SQL traffic — the system is overwhelmingly a patient-order/result store. `normalrange` is read
91 times, which is consistent with the manual's documented behaviour of resolving a range at
result-entry time.

**No table has zero SELECT statements** — there is no write-only table in the access profile.

---

## 9. Migration mechanics — operational reading

1. The DDL block occupies roughly `0x67413A`–`0x6DC714` of the executable's string pool
   (≈ 458 KB of address space) and is ordered by descending virtual address.
2. The application attempts the SQL-Server statement first and then the Access-compatible variant,
   so on Jet 4 the first attempt fails silently and the second succeeds where it exists.
3. The migration runs **on the client's first run**, not at disc-build time — the shipped Access
   database is a *partial* deployment of the v1.605 schema. **CORROBORATED** by the 38 unapplied
   columns in existing Access tables.
4. Whether the migration is **retried** on a later run is **UNRESOLVED** (V2-BR-023); the
   failure-handling path (silent skip vs logged error) is **UNRESOLVED** (V2-BR-024).

---

## 10. Known limitations of this specification

- The binary artefacts themselves (`Patients.mdb`, `real lab system.exe`) were not available to
  this documentation pass; all schema claims derive from `mdbtools` dumps and analysis exports,
  which are themselves derived from the binaries. Confidence is therefore **CORROBORATED** at best
  for raw byte counts and **VERIFIED** for structure.
- `VIDAS` and the `Interface` catalogue are structurally uncharacterised.
- 709 of 837 inventoried files (84.7 %) are attributed to a version by path-prefix rule rather than
  by in-file evidence.
- Runtime behaviour of the migration is unobserved; the corpus is 100 % static analysis.

---

## 11. Related documents

- `Docs/KB/legacy/data/MODEL.md` — navigational data-model summary.
- `Docs/KB/legacy/data/MIGRATION.md` — the migration mechanism and its failure modes.
- `Docs/KB/legacy/data/BRANCHES.md` — branch model and `Branch_Data.mdb` usage.
- `Docs/KB/legacy/data/IDENTIFIERS.md` — the `patientid` positional contract.
- `Docs/KB/legacy/clinical/RULES.md` — clinical semantics, constants and critical values.
- `Docs/KB/legacy/clinical/KNOWLEDGE-LAYER.md` — the knowledge tables in detail.
- `Docs/KB/legacy/architecture/EXECUTABLE.md` — the VB6 object model and SQL literal census.
- `Docs/KB/legacy/evidence/SOURCES.md` — source traceability and disposition register.
- `Docs/KB/legacy/evidence/CONTRADICTIONS.md` — the contradiction register.
- `Docs/KB/legacy/mapping/GAP-ANALYSIS.md` — this schema against the modern project.
