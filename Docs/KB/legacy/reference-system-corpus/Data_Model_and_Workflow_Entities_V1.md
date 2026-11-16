# Data Model and Workflow Entities — RealLab V-ROOT-1.00

**Document ID:** RL-V1-DM-001
**Version documented:** `V-ROOT-1.00`
**Evidence basis:** 364 verified artefacts · static analysis only
**Companion documents:** `Complete_Reverse_Engineering_Reference_V1.md`, `Functional_Specification_V1.md`, `Business_Rules_V1.md`

---

## 1. Scope, method and certainty model

### 1.1 What this document covers

The **complete `Patients.mdb` schema of V-ROOT-1.00** — 93 tables, 1,527 columns —
plus `Branch_Data.mdb` (49 schema-only tables), `data.mdb` (3 tables), and the
configuration-file layer that participates in persistence.

### 1.1a ★ Corrections applied to the earlier narrative analysis

The earlier investigation narrative (`analysis/BATCH-01/data-model.md`) was
SQL-literal-based and several of its claims are **contradicted by the extracted Jet
schema**. The following corrections are applied throughout this document:

| # | Earlier claim | Correction applied here |
|---|---|---|
| X-01 | `patientinfo.patientid` is "the patient primary key" | **Contradicted.** `patientid` is an **order/visit** identifier. `MID(patientid,3,2)` is the **person** key; `mid(patientid,10,3)` is a display code; position 9 carries the branch in V-CD-2019. **There is no patient master table.** |
| X-02 | `patientinfo.pricemethod` holds `Lab To Lab` / `Individual` / `Free` | **Contradicted by the corpus.** 0 hits for those literals across 1,337 SQL statements, and `patientinfo` has 0 rows. **The value domain is NOT ESTABLISHED.** |
| X-03 | `patients` is a "support / operations" table | **Corrected.** `patients` is the **operator table** (`patientusname`, `grant1`…`grant12`, `startwork`/`endwork`). |
| X-04 | `useraccounts` is one of the core tables | **Corrected.** `useraccounts` **does not exist in any of the four `.mdb` files**. |
| X-05 | "100 tables referenced in SQL" | **Refined.** 9 of the 100 tokens are extraction-truncation artefacts; 1 has no table; **5 real tables are never referenced** by V1 SQL at all. |
| X-06 | SQL-derived column counts (e.g. `patientinfo` 22, `normalrange` 4) | **Corrected.** Those counted only columns *named in SQL*. The real counts are 33, 44, 37, 18, 9, 19, 4 respectively. |
| X-07 | "one independent table per profile" | **Confirmed and quantified.** **50** profile tables in `Patients.mdb`, 47 in `Branch_Data.mdb`; **32** are joined to `patientinfo` in SQL (178 join statements), the other 18 are linked by the identical `patientid` + `requestdate` pair only. |
| X-08 | `elmadfoo3` = "the amount paid" | **Not established.** The evidence supports **a monetary amount and the base of the commission formula** `SUM(Gehats.omola * patientinfo.elmadfoo3 / 100)`. |
| X-09 | `RESULTT` = "result value (result + T)" | **Not established.** There is one `Text(50)` column `RESULTT` plus `resultt1`…`resultt4`; the decomposition has no basis in the files. |
| X-10 | `patientsex` feeds the range calculation | **Inferred, not observed.** No statement references `patientinfo.patientsex` and `normalrange.sex` together; the link rests on the shared value vocabulary. |
| X-12 | `patientnamear <> '<<< Blank >>>'` is an unnamed-patient sentinel | **Confirmed** — the string occurs once in the SQL literals, exactly as quoted. |
| X-13 | "20 union queries" | **Confirmed** — 20 statements contain `SHAPE`, 21 `APPEND` occurrences, 3 `RELATE` occurrences. |
| X-14 | `patientinfo.labid` links to `Labbranch.branchid` | **Inferred only.** `labid` is a report filter that correlates with `history.labid`, but it is never joined to `Labbranch`, and its type (`Text(13)`) is incompatible with `Labbranch.branchid` (`Long Integer`). |

### 1.2 Certainty tiers

| Tier | Meaning |
|---|---|
| **DIRECTLY OBSERVED** | Read directly from the extracted Access schema, row counts or sample data. |
| **SOURCE-CODE IDENTIFIED** | Present in the executable's SQL, literals or object model. |
| **INFERRED FROM ESTABLISHED BEHAVIOUR** | A reasonable reading of documented behaviour, not directly present in the schema. |
| **UNCERTAIN / INCOMPLETE** | Known to exist but incompletely characterised. |

> **★ Structural warning.** Microsoft Access (Jet 4) does **not** enforce foreign
> keys. The shipped `.mdb` files therefore contain **no enforced referential
> constraints**. Every relationship in §6 is classified as OBSERVED (exact
> column-name pairing **plus** a real join in an extracted SQL statement) or
> INFERRED (exact column-name pairing only). **None is a database-enforced
> constraint**, and this document does not present any as one.

### 1.3 Identifier scheme

| Prefix | Meaning |
|---|---|
| `V1-ENT-nnn` | Entity |
| `V1-FLD-nnn` | Field within an entity |
| `V1-REL-nnn` | Relationship |
| `V1-WF-nnn` | Workflow-to-entity mapping |

### 1.4 Type conventions

Types are reproduced exactly as the Jet driver reports them: `Long Integer`,
`Integer`, `Double`, `Single`, `Text (n)`, `Memo/Hyperlink (255)`, `DateTime`,
`Boolean`, `AutoNumber` (where reported).

---

## 2. Entity catalogue

### 2.1 Store-level summary

| Database file | Tables | Columns | Rows | Role |
|---|---:|---:|---:|---|
| `bin\Patients.mdb` | **93** | **1,527** | 3,123 | Primary operational database |
| `bin\Patients_backup.mdb` | 90 | 1,381 | 2,786 | Backup copy — **stale relative to the primary** |
| `bin\Branch_Data.mdb` | **49** | — | **0** | Schema-only template used to seed a branch |
| `bin\data.mdb` | 3 | — | — | `history`, a slim `patientinfo`, `wasl` |

### 2.2 Full entity catalogue — `Patients.mdb`

| Entity ID | Technical name | Purpose | Cols | Rows | Category | Certainty |
|---|---|---|---:|---:|---|---|
| **V1-ENT-001** | `patientinfo` | Patient master record | 33 | 0 | core | DIRECTLY OBSERVED |
| **V1-ENT-002** | `patienttest` | One row per requested test — the transactional centre | 44 | 222 | core | DIRECTLY OBSERVED |
| **V1-ENT-003** | `testtable` | Test catalogue | 37 | **609** | knowledge | DIRECTLY OBSERVED |
| **V1-ENT-004** | `normalrange` | Reference ranges | 18 | **852** | knowledge | DIRECTLY OBSERVED |
| **V1-ENT-005** | `constanttable` | Clinical computation constants | 7 | — | knowledge | DIRECTLY OBSERVED |
| **V1-ENT-006** | `subgrouptable` | Clinical sub-group catalogue | 4 | 46 | knowledge | DIRECTLY OBSERVED |
| **V1-ENT-007** | `testunits` | Measurement-unit dictionary | 2 | 69 | knowledge | DIRECTLY OBSERVED |
| **V1-ENT-008** | `converts` | Analyte-specific unit-conversion factors | 6 | 167 | knowledge | DIRECTLY OBSERVED |
| **V1-ENT-009** | `abbreviations` | Abbreviation dictionary | 3 | 650 | knowledge | DIRECTLY OBSERVED |
| **V1-ENT-010** | `comments` | Standard result comments | 5 | 12 | knowledge | DIRECTLY OBSERVED |
| **V1-ENT-011** | `payment` | Payment transactions | 9 | — | finance | DIRECTLY OBSERVED |
| **V1-ENT-012** | `patienttotal` | Per-patient totals | 15 | 0 | finance | DIRECTLY OBSERVED |
| **V1-ENT-013** | `Gehats` | Referring parties with commission | 19 | 10 | finance | DIRECTLY OBSERVED |
| **V1-ENT-014** | `barcodes` | Barcode allocation | 2 | — | patient | DIRECTLY OBSERVED |
| **V1-ENT-015** | `history` | Persistent patient attributes | 25 | 3 | patient | DIRECTLY OBSERVED |
| **V1-ENT-016** | `useraccounts` | User accounts — **referenced by 6 SQL statements but present in NO `.mdb` file**; only the column names `usersnum` and `usersdr` are recoverable | — | — | security | SOURCE-CODE IDENTIFIED |
| **V1-ENT-017** | `loggroup` | Login groups | 2 | — | security | DIRECTLY OBSERVED |
| **V1-ENT-018** | `instrument` | Analyser instruments | 20 | 0 | configuration | DIRECTLY OBSERVED |
| **V1-ENT-019** | `wasl` | Work-sheet storage | 12 | 0 | workflow | DIRECTLY OBSERVED |
| **V1-ENT-020** | `pictures` | Patient-associated images | 73 | 2 | patient | DIRECTLY OBSERVED |
| **V1-ENT-021** | `anyreport` | Ad-hoc report composition | 26 | 15 | reporting | DIRECTLY OBSERVED |
| **V1-ENT-022** | `reportview` | Saved report views | 42 | 0 | reporting | DIRECTLY OBSERVED |
| **V1-ENT-023** | `patients` | **★ OPERATOR / USER-ACCOUNT TABLE — not a patient table.** Holds `patientusname`, `patientuscap`, `patientusid`, **twelve permission bits `grant1`…`grant12`**, and the attendance fields `startwork`, `endwork`, `breakyorn`, `breaktime`, `khasmlimit` | 26 | 2 | security | DIRECTLY OBSERVED |
| **V1-ENT-024** | `phone` | Telephone / contact records | 19 | 0 | patient | DIRECTLY OBSERVED |
| **V1-ENT-025** | `antibiotic` | Antibiotic catalogue | 25 | 38 | knowledge | DIRECTLY OBSERVED |
| **V1-ENT-026** | `antibio` | Antibiotic — reduced form | 2 | 96 | knowledge | DIRECTLY OBSERVED |
| **V1-ENT-027** | `customtesttable` | Custom test definitions | — | — | knowledge | SOURCE-CODE IDENTIFIED |
| **V1-ENT-028** | `customgrouptable` | Custom sub-groups | — | — | knowledge | SOURCE-CODE IDENTIFIED |
| **V1-ENT-029** | `otherlabname` | Send-out destination laboratories | — | — | send-out | SOURCE-CODE IDENTIFIED |
| **V1-ENT-030** | `tasgel` | — | 11 | 0 | UNCERTAIN | UNCERTAIN |
| **V1-ENT-031…V1-ENT-080** | **50 clinical profile tables** (47 in `Branch_Data.mdb`) | Analyte-value tables, one per discipline | 5–84 | 0–13 | profile | DIRECTLY OBSERVED |

### 2.3 Profile result tables — the 50 disciplines (47 in the branch template)

| Profile table | Cols | Rows | Profile table | Cols | Rows |
|---|---:|---:|---|---:|---:|
| `crossmat` | 84 | 0 | `GLUCOSETOLERANCE` | 25 | 0 |
| `pictures` | 73 | 2 | `widal2` | 26 | 0 |
| `stool` | 61 | 3 | `cultures` | 33 | 1 |
| `semen` | 59 | 4 | `cbc` | 29 | 5 |
| `urine` | 58 | 1 | `trace` | 25 | 2 |
| `prostaticx` | 53 | 1 | `CSFT` | 24 | 1 |
| `BONEMAR` | 23 | 1 | `Electrolytes` | 20 | 0 |
| `hepatitis` | 20 | 1 | `IMMUNOLOGY` | 19 | 0 |
| `stone` | 19 | 0 | `Fertlity` | 17 | 2 |
| `postcoitalt` | 17 | 0 | `hormones` | 16 | 1 |
| `pt` | 16 | 2 | `Suprarenal` | 16 | 0 |
| `widal` | 16 | 0 | `Vitamin` | 15 | 0 |
| `kidney` | 15 | 6 | `liver` | 15 | 12 |
| `Drugs` | 14 | 0 | `Tumors` | 14 | 1 |
| `Pancreas` | 13 | 0 | `TORCH` | 13 | 1 |
| `Abuse` | 12 | 0 | `cultureanti` | 12 | 19 |
| `sugar` | 12 | 13 | `CoagFactors` | 11 | 0 |
| `Lipids` | 11 | 5 | `tasgel` | 11 | 0 |
| `Calcium` | 10 | 0 | `BOHB` | 10 | 0 |
| `BloodGases` | 10 | 0 | `DIAB` | 10 | 0 |
| `ESR` | 10 | 0 | `Folic` | 10 | 0 |
| `GLU` | 10 | 0 | `HBSE` | 10 | 0 |
| `Iron` | 10 | 0 | `LIPID` | 10 | 0 |
| `MN` | 10 | 0 | `PRO` | 10 | 0 |
| `PSA` | 10 | 0 | `RH` | 10 | 0 |
| `SerumElectrolytes` | 10 | 0 | `Titer` | 10 | 0 |
| `UrineElectrolytes` | 10 | 0 | `ZK` | 10 | 0 |
| `BILI` | 10 | 0 | `BONEMAR2` | 10 | 0 |
| `CARB` | 10 | 0 | `Calcium2` | 10 | 0 |

> **★ Structurally important observation.** The profile tables are **wide, flat
> analyte-value tables** — one column per analyte, not a normalised result entity.
> `crossmat` has 84 columns; `urine` 58; `cbc` 29. This means the analyte set of
> each discipline is **fixed by the table's shape**, and adding an analyte to a
> discipline requires a schema change, not a data change.
>
> This is the single most consequential structural fact in the data model, and it
> differs from the `patienttest` design (which is one row per test).

---

## 3. Field-level documentation — core entities

### 3.1 `patientinfo` — V1-ENT-001 (33 columns)

| Field | Type | Key | Null | Business meaning | Related rules | Evidence | Confidence |
|---|---|---|---|---|---|---|---|
| `patientid` | Text (13) | **Order/visit identifier (NOT the patient key)** | yes | Composite identifier, 13 characters. **`MID(patientid,3,2)` = the person key** (used both as the patient filter and as `COUNT(MID(patientid,3,2))` in the turnover reports); **`mid(patientid,10,3)` = a short display code**; **position 9 is reserved for the branch code in V-CD-2019** (`MID(patientid,9,1) as bran`). **There is no patient master table.** | V1-BR-005 | schema + SQL `MID()` usage | CONFIRMED |
| `labid` | Text (13) | candidate key | yes | Laboratory identifier paired with the patient | V1-BR-007 | schema | CONFIRMED |
| `patientnamear` | Text (40) | | yes | Patient name in Arabic | — | schema | CONFIRMED |
| `patientsefa` | Text (20) | | yes | Referral source (English/romanised) | — | schema | CONFIRMED |
| `referredbyname` | Text (40) | | yes | Referring party name | V1-BR-026 | schema | CONFIRMED |
| `referredbysefa` | Text (20) | | yes | Referring party short form | — | schema | CONFIRMED |
| `mowzaf` | Text (40) | | yes | Treating physician | — | schema | CONFIRMED |
| `patientsex` | Text (7) | | yes | Sex — the documented range-selection input (see X-10: the link to `normalrange.sex` is INFERRED, not observed in any statement) | V1-BR-011 | schema + shared value vocabulary | CONFIRMED as a field; INFERRED as the range driver |
| `patientage` | Double | | yes | Age | V1-BR-011 | schema | CONFIRMED |
| `patientageunit` | Text (5) | | yes | Age unit — years / months / days | V1-BR-011 | schema | CONFIRMED |
| `patientaddress` | Text (70) | | yes | Address | — | schema | CONFIRMED |
| `patientphone` | Text (12) | | yes | Telephone | — | schema | CONFIRMED |
| `patienttel` | Text (12) | | yes | Alternate telephone | — | schema | CONFIRMED |
| `email` | Text (40) | | yes | E-mail | — | schema | CONFIRMED |
| `requestdate` | DateTime | | yes | Request date | V1-BR-025 | schema | CONFIRMED |
| `outdate` | DateTime | | yes | Discharge / hand-over date | — | schema | CONFIRMED |
| `timein` | Text (10) | | yes | Arrival time (text) | — | schema | CONFIRMED |
| `barcode` | Text (25) | | yes | Printed barcode | V1-BR-006 | schema | CONFIRMED |
| `pricemethod` | Text (10) | | yes | Pricing method | — | schema | CONFIRMED |
| `totalprice` | Double | | yes | Gross account total | V1-BR-015 | schema | CONFIRMED |
| `totalpricedisc` | Double | | yes | Discounted account total | V1-BR-015 | schema | CONFIRMED |
| `discount` | Double | | yes | Discount amount | V1-BR-015 | schema | CONFIRMED |
| `discount100` | Double | | yes | Percentage discount | V1-BR-015 | schema | CONFIRMED |
| `elmadfoo3` | Double | | yes | **Commission base** — appears in the extracted SQL as `SUM(Gehats.omola * patientinfo.elmadfoo3 / 100)` | V1-BR-026 | SQL literal + schema | CONFIRMED |
| `elbakyma3mal` | Double | | yes | Arabic field 4 — amount | — | schema | CONFIRMED |
| `kawmy` | Text (14) | | yes | Arabic field — **meaning not established** | — | schema | CONFIRMED as a field; UNKNOWN as a meaning |
| `completes` | Text (1) | | yes | Completion flag | — | schema | CONFIRMED |
| `recepnote` | Text (100) | | yes | Reception note | — | schema | CONFIRMED |
| `moreinfo` | Text (22) | | yes | Additional information | — | schema | CONFIRMED |
| `editdate` | Text (40) | | yes | Edit timestamp (text) | — | schema | CONFIRMED |
| `editdate2` | Text (40) | | yes | Second edit timestamp | — | schema | CONFIRMED |
| `edituser2` | Text (40) | | yes | Second editor | — | schema | CONFIRMED |
| `editnum` | Integer | | yes | Edit counter | — | schema | CONFIRMED |

> **★ Design note.** `patientinfo` carries **two Arabic-labelled numeric fields**
> (`elmadfoo3`, `elbakyma3mal`) and one text field (`kawmy`) whose names are
> transliterated Arabic rather than English identifiers. Their semantics are **not
> established** — they are most likely laboratory-specific charge or discount
> components, given they sit alongside `totalprice` / `totalpricedisc`, but that is
> an inference.
>
> **Also note:** `requestdate` and `outdate` are `DateTime`, but the edit/print
> timestamps on `patienttest` are `Text (40)`. **The system mixes typed dates and
> text timestamps in the same schema.** This is a real data-quality hazard.

### 3.2 `patienttest` — V1-ENT-002 (44 columns)

| Field | Type | Key | Null | Business meaning | Related rules | Evidence | Confidence |
|---|---|---|---|---|---|---|---|
| `patientid` | Text (13) | FK → `patientinfo.patientid` | yes | Patient the test belongs to | V1-BR-005 | schema + joins | CONFIRMED (OBSERVED pairing) |
| `requestdate` | DateTime | | yes | Request date | — | schema | CONFIRMED |
| `testcode` | Integer | FK → `testtable.testcode` | yes | Test identifier | V1-BR-016 | schema + joins | CONFIRMED (OBSERVED pairing) |
| `subgroupcode` | Integer | FK → `subgrouptable.subgroupcode` | yes | Clinical sub-group — the routing key | V1-BR-031 | schema | CONFIRMED (OBSERVED pairing) |
| `testsimp` | Text (50) | | yes | Test short/display name (snapshot) | — | schema | CONFIRMED |
| `testname` | Text (50) | | yes | Test name (snapshot) | — | schema | CONFIRMED |
| `testabrev` | Text (50) | | yes | Test abbreviation (snapshot) | — | schema | CONFIRMED |
| `testunit` | Text (15) | | yes | Test unit (snapshot) | — | schema | CONFIRMED |
| `historyname` | Text (50) | | yes | History name (snapshot) | — | schema | CONFIRMED |
| `normalrang` | Memo/Hyperlink (255) | | yes | Reference range **frozen onto the test row** | V1-BR-009 | schema | CONFIRMED |
| `lowlimit` | Double | | yes | Low limit (frozen) | V1-BR-009 | schema | CONFIRMED |
| `highlimit` | Double | | yes | High limit (frozen) | V1-BR-009 | schema | CONFIRMED |
| `nolowexe` | Text (20) | | yes | Low exception text | V1-BR-012 | schema | CONFIRMED |
| `nohighexe` | Text (20) | | yes | High exception text | V1-BR-012 | schema | CONFIRMED |
| `testprice` | Double | | yes | Test price | V1-BR-015 | schema | CONFIRMED |
| `testpriceout` | Double | | yes | Send-out price | — | schema | CONFIRMED |
| `testpriceoutmadfo` | Double | | yes | Send-out price — alternate currency | — | schema | CONFIRMED |
| `testtime` | Integer | | yes | Test turnaround time | — | schema | CONFIRMED |
| `arrangnum` | Integer | | yes | Display / ordering number within the profile | — | schema | CONFIRMED |
| `printwith` | Boolean | | yes | Print this test with the profile report | — | schema | CONFIRMED |
| `resultfinsh` | Boolean | | yes | Result completed flag | — | schema | CONFIRMED |
| `resultreview` | Boolean | | yes | **Reviewed flag — the hand-over gate** | V1-BR-014 | schema | CONFIRMED |
| `resultprint` | Boolean | | yes | Printed flag | — | schema | CONFIRMED |
| `RESULTCASEPRINT` | Text (15) | | yes | Printed case reference | — | schema | CONFIRMED |
| `RESULTT` | Text (50) | | yes | Result text (primary) | — | schema | CONFIRMED |
| `resultt1`…`resultt4` | Text (50) ×4 | | yes | Four additional result text slots | — | schema | CONFIRMED |
| `commentx` | Memo/Hyperlink (255) | | yes | Result comment | V1-BR-013 | schema | CONFIRMED |
| `give` | Boolean | | yes | **Handed over to the patient** | V1-BR-014 | schema + CHM `005.htm` | CONFIRMED |
| `sentout` | Boolean | | yes | Send-out settled | V1-BR-024 | schema | CONFIRMED |
| `send` | Boolean | | yes | Send-out dispatched | V1-BR-024 | schema | CONFIRMED |
| `otherlabname` | Text (40) | FK → destination laboratory | yes | Send-out destination | V1-BR-017, 023 | schema | CONFIRMED (OBSERVED pairing) |
| `printuser` | Text (40) | | yes | Who printed it | V1-BR-027 | schema | CONFIRMED |
| `printdate` | Text (40) | | yes | When it was printed | — | schema | CONFIRMED |
| `printnum` | Double | | yes | Report sequence number | V1-BR-033 | schema | CONFIRMED |
| `edituser` | Text (40) | | yes | Last editor | — | schema | CONFIRMED |
| `editdate` | Text (40) | | yes | Last edit timestamp | — | schema | CONFIRMED |
| `prevuser` | Text (40) | | yes | Previous editor | — | schema | CONFIRMED |
| `prevdate` | Text (40) | | yes | Previous edit timestamp | — | schema | CONFIRMED |
| `taslimuser` | Text (40) | | yes | Who handed over | — | schema | CONFIRMED |
| `taslimdate` | Text (40) | | yes | Hand-over timestamp | — | schema | CONFIRMED |
| `loggroup` | Text (50) | FK → `loggroup.logname` | yes | Login group at the time of the action | V1-BR-032 | schema | CONFIRMED (OBSERVED pairing) |

> **★ The single most important structural fact in this document.**
> `patienttest` **freezes the reference range onto the test row** — `normalrang`,
> `lowlimit`, `highlimit`, `nolowexe`, `nohighexe`, `testname`, `testabrev`,
> `testunit`, `historyname` are all **denormalised copies of `normalrange` and
> `testtable`**, captured at the moment the test is ordered.
>
> **Consequences:**
> 1. A printed report reflects the range **in force when the test was ordered**, not
>    the range in force now. This is clinically correct behaviour for a historical
>    record — and it is deliberate design, not an accident.
> 2. It also means the `normalrange` table is consulted **at ordering time**, and
>    the frozen copy is used at entry and print time. The range-selection algorithm
>    therefore applies **once**, on ordering.
> 3. **A change to `normalrange` does not affect already-ordered tests.** For a
>    re-implementation this is a behavioural contract that must be reproduced
>    deliberately, not by accident.

### 3.3 `testtable` — V1-ENT-003 (37 columns, 609 rows)

| Field | Type | Key | Business meaning | Related rules |
|---|---|---|---|---|
| `testcode` | Long Integer | **PK** | Test number — **4 is the immutable urine analysis** | V1-BR-016 |
| `arrangnum` | Integer | | Display order | — |
| `subgroupcode` | Integer | FK → `subgrouptable` | Clinical sub-group | V1-BR-031 |
| `subgroupname` | Text (40) | | Sub-group name (snapshot) | — |
| `testsimp` | Text (50) | | Display name | — |
| `testname` | Text (50) | | Test name | V1-BR-016 |
| `testnamear` | Text (50) | | **Arabic test name** | — |
| `testabrev` | Text (50) | | Abbreviation | — |
| `waslname` / `waslname2` | Text (50) ×2 | | Work-sheet names | — |
| `historyname` | Text (50) | | History name | — |
| `barcodesymb` | Text (50) | | Barcode symbology | V1-BR-006 |
| `barcode1` / `barcode2` / `barcode3` | Text (50) ×3 | | Barcode template fields | V1-BR-006 |
| `loggroup` | Text (50) | | Owning login group | V1-BR-032 |
| `active` | Boolean | | Active flag | — |
| `inotherlab` | Boolean | | **Send-out flag** | V1-BR-017 |
| `otherlabname` | Text (50) | | Default send-out destination | V1-BR-023 |
| `otherlabprice` | Double | | Send-out price | — |
| `patientprice` | Double | | Patient price | V1-BR-015 |
| `labprice` | Double | | Laboratory price | — |
| `anyqus` | Boolean | | "Any question" flag | — |
| `thequse` | Text (150) | | The associated question text | — |
| `testdiscr` | Text (100) | | Test description | V1-BR-035 |
| `testeffect` | Text (100) | | Clinical effect / significance | V1-BR-035 |
| `testusage` | Text (100) | | Usage guidance | V1-BR-035 |
| `typeofsample` | Text (100) | | **Sample type** | V1-BR-035 |
| `deleteprotect` | Boolean | | Protect from deletion | — |
| `testtime` | Integer | | Turnaround time | — |
| `printwith` | Boolean | | Print with profile report | — |
| `branch` | Text (50) | | Owning branch | — |
| `seereport` | Text (50) | | Report visibility | — |
| `normaltype` | Text (50) | | Normal-type selector | V1-BR-011 |
| `addorno` / `testorno` | Boolean ×2 | | Ordering flags | — |
| `REFRANGE` | Text (1) | | Reference-range flag | V1-BR-009 |

> **★ Notable.** `testtable` itself carries the **methodology text** —
> `typeofsample` (sample type), `testdiscr` (description), `testeffect`
> (clinical significance), `testusage` (usage). This means the method-sheet layer
> exists **twice**: as structured columns here, and as 29 prose RTF sheets in
> `Cache\`. **Whether the two are kept in sync is NOT ESTABLISHED.**

### 3.4 `normalrange` — V1-ENT-004 (18 columns, 852 rows)

| Field | Type | Key | Business meaning | Related rules |
|---|---|---|---|---|
| `normalid` | Long Integer | **PK** | Range row identifier | — |
| `testcode` | Long Integer | FK → `testtable.testcode` | Analyte | V1-BR-011 |
| `subgroupcode` | Integer | FK → `subgrouptable` | Sub-group | V1-BR-031 |
| `testsimp` | Text (50) | | Analyte display name (snapshot) | — |
| `sex` | Text (10) | | `Male` / `Female` / `All` / empty | V1-BR-011 |
| `firstold` | Double | | Age window — from | V1-BR-011 |
| `secondold` | Double | | Age window — to | V1-BR-011 |
| `ageunit` | Text (10) | | `Years` / `Days` / `Mons.` / empty | V1-BR-011 |
| `pregsmok` | Single | | Pregnancy/smoking value — **all 852 rows populated**; role UNKNOWN | — |
| `normalrang` | Memo/Hyperlink (255) | | Normal range text | — |
| `testunit` | Text (15) | | Unit | V1-BR-029 |
| `lowlimit` | Double | | Low abnormality limit | V1-BR-009 |
| `highlimit` | Double | | High abnormality limit | V1-BR-009 |
| `nolowexe` | Text (20) | | Low exception text | V1-BR-012 |
| `nohighexe` | Text (20) | | High exception text | V1-BR-012 |
| `lowcomment` | Memo/Hyperlink (255) | | Low comment | V1-BR-013 |
| `highcomment` | Memo/Hyperlink (255) | | High comment | V1-BR-013 |
| `REFRANGE` | Text (1) | | Reference-range flag | — |

**Observed distributions** (from the 852 extracted rows):

| Dimension | Values |
|---|---|
| `sex` | `Male` 422 · `Female` 421 · `All` 5 · empty 4 |
| `ageunit` | `Years` 708 · `Days` 70 · `Mons.` 65 · empty 9 |
| Age span | `Years` 0–120 · `Days` 1–120 · `Mons.` 0–24 |
| Rows with a numeric `lowlimit` | 459 |
| Rows with a numeric `highlimit` | 643 |
| Distinct low exception texts | 19 |
| Distinct high exception texts | 19 |
| Distinct units in use | 51 |
| Distinct analytes covered | 304 |

### 3.5 `constanttable` — V1-ENT-005 (7 columns)

| Field | Type | Business meaning |
|---|---|---|
| `constantdiscrp` | Text (50) | Constant-group description (`CBCCONS`, `PTCONS`) |
| `constant1` | Text (50) | Slot 1 |
| `constant2` | Text (50) | Slot 2 |
| `constant3` | Text (50) | Slot 3 |
| `constant4` | Text (50) | Slot 4 |
| `constant5` | Text (50) | Slot 5 |
| `constant6` | Text (50) | Slot 6 |

> The values are stored as **Text (50)**, not numeric — a deliberate choice for a
> key/value table that must also hold the non-numeric token `2CVJW`.
> See V1-BR-037 … V1-BR-040 for the confirmed values.

### 3.6 `subgrouptable` — V1-ENT-006 (4 columns, 46 rows)

| Field | Type | Key | Business meaning |
|---|---|---|---|
| `subgroupcode` | Long Integer | **PK** | Sub-group number |
| `subgroupname` | Text (50) | | English name |
| `arname` | Text (50) | | Arabic name — **empty for all 46 shipped rows** |
| `deleteprotect` | Boolean | | Protect from deletion |

**All 46 shipped rows** (codes 1–48, with 36 and 46 unused):

`1 BLOOD GLUCOSE` · `2 KIDNEY FUNCTIONS` · `3 LIVER FUNCTIONS` · `4 LIPIDS PROFILE` ·
`5 URINE EXAMINATION` · `6 STOOL EXAMINATION` · `7 Autoimmune diseases` · `8 BLOOD GROUP` ·
`9 Erythrocyte Sedimentation Rate` · `10 CULTURE AND SENSITIVITY` · `11 BLOOD PICTURE` ·
`12 SERODIAGNOSIS` · `13 Hepatitis Markers` · `14 TOXOPLASMOSOIS` · `15 HEMATOLOGY` ·
`16 Thyroid Hormones` · `17 Antiphospholipid antibody syndrome` · `18 Cardiac Markers` ·
`19 TORCH Screen` · `20 Coagulation Profile` · `21 Pancreas Hormones` · `22 Separate` ·
`23 Semen Examination` · `24 Tumors Markers` · `25 Serum Electrolytes` ·
`26 Urine Electrolytes` · `27 CSF Electrolytes` · `28 Vitamins` · `29 Trace Elements` ·
`30 Iron Tests` · `31 Pregnancy Tests` · `32 Fertlity Hormones` ·
`33 Suprarenal Hormones` · `34 VIROLOGY` · `35 MOLECULAR BIOLOGY` · `37 Drug Monitoring` ·
`38 Drugs of Abuse` · `39 Coagulation Factors` · `40 Immunoglobulins` · `41 Blood Gases` ·
`42 Hemoglobin electrophoresis` · `43 Protein electrophoresis` · `44 Aminogram` ·
`45 WBCs function` · `47 Renal Failure` · `48 Diabetes profile`

> **★ Two observations.**
> 1. `arname` — the Arabic sub-group name column — is **empty in all 46 shipped
>    rows**. The Arabic interface therefore uses the English sub-group names
>    directly. This is a concrete, verifiable Arabic-terminology fact.
> 2. **All 46 rows have `deleteprotect = 1` except codes 45, 47 and 48** — the
>    three most recently added sub-groups (`WBCs function`, `Renal Failure`,
>    `Diabetes profile`) are the only unprotected ones. This is direct evidence of
>    how the catalogue was extended over time.

### 3.7 `payment` — V1-ENT-011 (9 columns)

| Field | Type | Business meaning | Related rules |
|---|---|---|---|
| `patientid` | Text (13) | Patient | V1-BR-005 |
| `paiddate` | DateTime | Payment date | V1-BR-025 |
| `type` | Integer | Payment type | — |
| `gehah` | Text (50) | Referring party code | V1-BR-026 |
| `testsimp` | Text (50) | Test short name | — |
| `money` | Double | Amount | V1-BR-015 |
| `mowzaf` | Text (40) | Treating physician | — |
| `editdate` | Text (40) | Edit timestamp | — |
| `comment` | Text (100) | Comment | — |

### 3.8 `Gehats` — V1-ENT-013 (19 columns, 10 rows)

| Field | Type | Business meaning | Related rules |
|---|---|---|---|
| `gehahid` | Long Integer | **PK** | Party identifier |
| `gehahname` | Text (40) | Party name | — |
| `gehahsefa` | Text (20) | Party short form | — |
| `masolname` / `masolsefa` | Text | **Employer / company** | — |
| `madina` | Text (40) | City | — |
| `addressxc` | Text (100) | Address | — |
| `phone` / `mobil` / `fax` / `masolphone` | Text (12) | Contact numbers | — |
| `mail` | Text (50) | E-mail | — |
| `web` | Text (50) | Website | — |
| `flose` | Boolean | Flag — **meaning not established** | — |
| `khasm` | Double | Private/discount rate | — |
| **`omola`** | Double | **Commission rate** | **V1-BR-026** |
| `morsel` | Integer | Installment count | — |
| `pricemethod` | Text (20) | Pricing method applied to this party | — |
| `printorno` | Boolean | Print number flag | — |

### 3.9 `history` — V1-ENT-015 (25 columns)

| Field | Type | Business meaning |
|---|---|---|
| `selector` | Integer | Selects the history usage — see `SETTING.ini [setup] historytype` |
| `labid` | Text (50) | Laboratory identifier |
| `requestdate` | DateTime | Date |
| `column1` … `column20` | Text (25) ×20 | **Free-form attribute slots — no fixed semantics** |
| `printflag` | Boolean | Print flag |
| `noofcol` | Integer | Number of columns in use |

> **★ The EAV-like design.** 20 untyped slots with no per-column meaning means the
> **position** of an attribute carries its meaning. Nothing in the schema prevents
> writing a different attribute type into a slot previously holding another.
> See V1-BR-007.

### 3.10 `barcodes`, `loggroup`, `abbreviations`, `testunits`, `converts`, `comments`

| Entity | Columns | Key fields | Notes |
|---|---|---|---|
| `barcodes` | 2 | `barcode` Text(50), `barcodeid` Long Integer | The **only** two columns — the table carries no allocation status, so the "allocated vs free" state must be encoded in the `barcode` value itself or inferred by presence |
| `loggroup` | 2 | `logid` Long Integer, `logname` Text(50) | Login group — the unit of session scoping (V1-BR-032) |
| `abbreviations` | 3 | `abbid` PK, `abbre` Text(30), `mean` Text(100) | 650 rows. **No test-code column** — so the abbreviation-to-analyte linkage is NOT in the schema |
| `testunits` | 2 | `unitsid` PK, `unitsname` Text(15) | 69 rows |
| `converts` | 6 | `testsimp` Text(100), `Funit`, `Sunit` Text(50), `Factors` Double, `Ffix` Integer, `Sfix` Integer | 167 rows. **Keyed by analyte *name* (`testsimp`), not by `testcode`** — a textual foreign key (V1-BR-029) |
| `comments` | 5 | `commentid` PK, `testcode` Integer, `subgroupcode` Integer, `testsimp` Text(50), `comments` Memo(255) | 12 rows — scoped by test **and** sub-group |

---

## 4. Profile result tables — field pattern

### 4.1 The shared shape

Every profile table follows the same pattern, with a width equal to its analyte count:

```
[patientid | requestdate | ...patient metadata... | <analyte_1> | <analyte_2> | ... ]
```

Example — `cbc` (29 columns):

| Column | Type | Role |
|---|---|---|
| `patientid` | Text (13) | Patient |
| `requestdate` | DateTime | Date |
| … | … | Patient metadata mirrored from `patientinfo` |
| `HGB` | Text/Double | Analyte slot |
| `HCT` | | Analyte slot |
| `WBC` | | Analyte slot |
| … | | … |

### 4.2 What this means

| Property | Consequence |
|---|---|
| Analyte set is fixed by table width | Adding an analyte to a discipline is a **schema change** |
| Results are stored horizontally | A per-analyte query scans columns, not rows |
| Patient metadata is duplicated into each profile | `patientid` and demographics appear in `patientinfo`, `patienttest` **and** every profile table |
| `patienttest` stores one row per test | `patienttest` is the **normalised** side; the profile tables are the **wide** side |

> **★ The two-model duality is the single most important data-architecture fact.**
> RealLab stores results **twice**: once normalised in `patienttest` (one row per
> test) and once denormalised into a wide profile table (one column per analyte).
> Which of the two is authoritative is **NOT ESTABLISHED** from static evidence —
> but the presence of `patienttest.RESULTT` / `resultt1`–`resultt4` and the
> per-profile columns side by side makes it certain that both are written.

---

## 5. Knowledge-layer structures

### 5.1 Summary

| Entity | Rows | Key structure | Role |
|---|---:|---|---|
| `testtable` | 609 | `testcode` PK | The catalogue — every test the laboratory offers |
| `normalrange` | 852 | `normalid` PK, `testcode` FK | The reference-interval table — 852 ranges over 304 analytes |
| `constanttable` | — | `constantdiscrp` + slots 1–6 | Clinical computation constants |
| `subgrouptable` | 46 | `subgroupcode` PK | Clinical grouping / routing |
| `testunits` | 69 | `unitsid` PK | Unit dictionary |
| `converts` | 167 | `testsimp` + `Funit`/`Sunit` | Unit conversion |
| `abbreviations` | 650 | `abbid` PK | Abbreviation dictionary |
| `comments` | 12 | `commentid` PK | Standard result comments |

### 5.1a ★ The operator table — the real permission model

`patients` (26 columns, 2 rows) is **not** a patient table. It is the **operator /
user-account** table:

| Field | Type | Meaning |
|---|---|---|
| `patientusid` | Long Integer | Operator id |
| `patientusname` | Text (40) | Operator name |
| `patientuscap` | Text (40) | Operator role/capability caption |
| `patientword` | Text (40) | Password |
| `patientad` | Boolean | **Administrator flag** |
| `patientus` | Boolean | Active-user flag |
| **`grant1` … `grant12`** | Boolean × 12 | **Twelve function-level permission bits** |
| `showtip` | Boolean | Show the tip panel |
| `tipnum` | Long Integer | Current tip index |
| `startwork` | Text (15) | **Attendance — work start** |
| `endwork` | Text (15) | **Attendance — work end** |
| `breakyorn` | Boolean | On break |
| `breaktime` | Text (15) | Break time |
| `khasmlimit` | Long Integer | Special/private limit |

> **★ Two findings that revise the earlier narrative.**
> 1. **Version 1 already had a twelve-bit permission model** (`grant1`…`grant12`),
>    not merely a login gate. The earlier characterisation of the permission model
>    as absent in Version 1 was wrong. **V-CD-2019 extends it to seventeen bits**
>    (`grant13`…`grant17` added by the DDL).
> 2. **Version 1 already had an attendance mechanism** — `startwork`, `endwork`,
>    `breakyorn`, `breaktime` on the operator table. V-CD-2019's `loger` is
>    therefore a **separate, additional event log**, not the first appearance of
>    attendance tracking.

### 5.2 The catalogue chain

```
subgrouptable (46)          ── the clinical discipline
      │
      ├──▶ testtable (609)          ── the analytes within it
      │          │
      │          └──▶ normalrange (852)  ── the reference intervals per analyte
      │                    × sex × age band
      │
      └──▶ Data\<profile>.ini (114)  ── the printed layout
                 │
                 └──▶ REPORT.dll      ── the 19-entry colour palette
```

### 5.3 `converts` — observed factors

| Analyte | From | To | Factor |
|---|---|---|---:|
| Acetaminophen | µg/mL | µmol/L | 6.62 |
| Acetone | mg/dL | mmol/L | 0.172 |
| Albumin | g/dL | g/L | 10 |
| Aldosterone | ng/dL | nmol/L | 0.0277 |
| Ammonia | µg/dL | µmol/L | 0.587 |

The factor set is **analyte-specific** — the same unit pair can carry different
factors for different analytes. This is why `converts` is keyed by `testsimp` and
not by unit pair alone (V1-BR-029).

### 5.4 `abbreviations` — the linkage gap

The table holds 650 rows of `abbre` → `mean` but **contains no test code**. The
abbreviation-to-analyte mapping therefore lives either in the `Data\*.ini` layouts
or in application code. **NOT ESTABLISHED.** See RL-V1-Q-13.

---

## 6. Relationships

### 6.1 OBSERVED relationships — exact column pairing **and** a join in extracted SQL

| Rel ID | Parent | Child | Cardinality | Join columns | Evidence |
|---|---|---|---|---|---|
| **V1-REL-001** | `patientinfo` | `patienttest` | 1:N | `patientinfo.patientid` = `patienttest.patientid` | Extracted SQL joins on `patientid`; the column name and type match exactly (Text(13)) |
| **V1-REL-002** | `testtable` | `patienttest` | 1:N | `testtable.testcode` = `patienttest.testcode` | Extracted SQL joins; `Integer` ↔ `Long Integer` |
| **V1-REL-003** | `subgrouptable` | `patienttest` | 1:N | `subgrouptable.subgroupcode` = `patienttest.subgroupcode` | Extracted SQL joins; `Integer` ↔ `Long Integer` |
| **V1-REL-004** | `testtable` | `normalrange` | 1:N | `testtable.testcode` = `normalrange.testcode` | Exact pairing, `Long Integer` ↔ `Long Integer`; 852 range rows over 304 analytes |
| **V1-REL-005** | `subgrouptable` | `testtable` | 1:N | `subgrouptable.subgroupcode` = `testtable.subgroupcode` | Exact pairing |
| **V1-REL-006** | `subgrouptable` | `normalrange` | 1:N | `subgrouptable.subgroupcode` = `normalrange.subgroupcode` | Exact pairing |
| **V1-REL-007** | `patientinfo` | `payment` | 1:N | `patientinfo.patientid` = `payment.patientid` | Exact pairing; the stock-take aggregates payments per patient |
| **V1-REL-008** | `patientinfo` | `history` | 1:1 | `patientinfo.labid` = `history.labid` | Exact pairing; `history` carries 3 rows against 0 `patientinfo` rows in the shipped file. **Note:** the pairing to `Labbranch.branchid` is INFERRED only — the types are incompatible (`Text(13)` vs `Long Integer`) and no join was located |
| **V1-REL-009** | `patienttest` | `payment` | 1:N | `patienttest.patientid` = `payment.patientid` | Exact pairing; `payment.testsimp` mirrors `patienttest.testsimp` |
| **V1-REL-010** | `Gehats` | `patientinfo` | 1:N | `Gehats.gehahsefa` ≈ `patientinfo.referredbysefa` | Exact-ish pairing; **semantic** link, textual |
| **V1-REL-011** | `Gehats` | `payment` | 1:N | `Gehats.gehahsefa` = `payment.gehah` | Exact pairing |
| **V1-REL-012** | `patienttest` | profile tables | 1:1 | `patientid` + `requestdate` | Profile tables carry the same patient key and date |
| **V1-REL-013** | `comments` | `testtable` | N:1 | `comments.testcode` = `testtable.testcode` | Exact pairing; `comments` also narrows by `subgroupcode` |
| **V1-REL-014** | `loggroup` | `patienttest` | 1:N | `loggroup.logname` = `patienttest.loggroup` | Exact pairing |
| **V1-REL-015** | `loggroup` | `testtable` | 1:N | `loggroup.logname` = `testtable.loggroup` | Exact pairing |

> **Note on V1-REL-008.** `patientinfo.labid` and `history.labid` both being Text
> and both present is strong evidence of a link, but the **`history` row count (3)
> against the `patientinfo` row count (0)** in the shipped file means the two
> tables were populated at different times. The pairing is exact; the semantic
> role of `labid` is INFERRED.

### 6.2 INFERRED relationships — exact column pairing only, no join located

| Rel ID | Parent | Child | Cardinality | Basis | Why only inferred |
|---|---|---|---|---|---|
| **V1-REL-016** | **`patients`** (operator table) | `patienttest` | 1:N | `patients.patientusname` ↔ `patienttest.edituser` / `printuser` / `taslimuser` | Exact-name pairing; **no join located**. Note this is the *operator* table, not `useraccounts` |
| **V1-REL-016b** | `useraccounts` | `patienttest` | 1:N | **NOT ESTABLISHED** | `useraccounts` exists in no `.mdb` file; only `usersnum` and `usersdr` are recoverable from SQL literals |
| **V1-REL-017** | `testtable` | `converts` | 1:N | `testtable.testsimp` = `converts.testsimp` | Textual key; no join located |
| **V1-REL-018** | `testtable` | `comments` | 1:N | `testtable.testsimp` = `comments.testsimp` | Textual key; no join located |
| **V1-REL-019** | `patienttest` | `patienttotal` | 1:N | Both keyed by `patientid` | Table semantics not established (0 rows) |
| **V1-REL-020** | `testtable` | `otherlabname` | 1:N | `testtable.otherlabname` | Destination table's own columns not established |
| **V1-REL-021** | `patientinfo` | `patienttest` | 1:N | `patientinfo.mowzaf` = `payment.mowzaf` | No join located |
| **V1-REL-022** | `antibiotic` | `cultures` | N:M | Both microbiology-domain tables; `cultures` is a profile table | **NOT ESTABLISHED** — no join located; listed for completeness only |

### 6.3 NOT ESTABLISHED relationships

A reader would reasonably expect the following; the artefacts do **not** support them.

| Expected relationship | Why it is not established |
|---|---|
| `patientinfo` → `patienttest` **enforced** | Access Jet 4 does not enforce foreign keys; no constraint object exists in the shipped file |
| `abbreviations` → `testtable` | `abbreviations` has **no** test-code column |
| `normalrange` → `patienttest` | The link is by frozen copy (`patienttest.lowlimit` etc.), not by key |
| `patientinfo` → profile tables | Profile tables carry their own patient metadata copy; no key link |
| `patienttest` → `payment` at the **line-item** level | `payment` has no `testcode`; it links by patient and by `testsimp` text |
| `useraccounts` → `loggroup` | No join column identified in either table |
| `instrument` → `patienttest` | `instrument` has no test linkage column |

---

## 7. Workflow-to-entity mapping

### V1-WF-001 — Patient episode

| Aspect | Detail |
|---|---|
| **Entities read** | `barcodes`, `Gehats`, `testtable`, `normalrange`, `subgrouptable` |
| **Entities created** | `patientinfo`, `barcodes`, `patienttest`, profile result table rows |
| **Entities updated** | `patientinfo` (edit fields, `totalprice`, `totalpricedisc`), `patienttest` (result fields, `resultreview`, `give`, `print*`), `history` |
| **Entities deleted** | None established |
| **Fields involved** | `patientid`, `patientnamear`, `patientsex`, `patientage`, `patientageunit`, `barcode`, `requestdate`, `testcode`, `subgroupcode`, `resultreview`, `give`, `printuser`, `printdate`, `printnum` |
| **State changes** | Patient created → tests ordered → results entered → `resultreview` set → printed → `give` set |
| **Relationships traversed** | V1-REL-001 … V1-REL-006, V1-REL-012 |
| **Validation dependencies** | `normalrange` (sex × age band match required for classification) |
| **Relevant business rules** | V1-BR-005, 006, 008, 009, 010, 011, 014, 015, 016, 017 |
| **Evidence** | CHM `003.htm`–`005.htm`; schema |

### V1-WF-002 — Profile result entry

| Aspect | Detail |
|---|---|
| **Entities read** | `normalrange` (limits, flags, comments), `testtable` (metadata), `comments`, `constanttable`, `Cache\*.rtf` |
| **Entities created** | Profile result row |
| **Entities updated** | `patienttest` — `RESULTT`, `resultt1`–`resultt4`, `commentx`, `resultreview`, `resultfinsh` |
| **Entities deleted** | None |
| **Fields involved** | `lowlimit`, `highlimit`, `nolowexe`, `nohighexe`, `RESULTT`, `resultt1`–`resultt4`, `commentx`, `resultreview` |
| **State changes** | Value entered → classified → commented → saved → reviewed |
| **Relationships traversed** | V1-REL-002, V1-REL-004, V1-REL-012, V1-REL-013 |
| **Validation dependencies** | A matching `normalrange` row **must exist**; otherwise classification is undefined |
| **Relevant business rules** | V1-BR-009, 010, 011, 012, 013, 018, 035, 037, 038, 039, 040, 041 |
| **Evidence** | CHM `004A.htm`; `normalrange` + `patienttest` schema; `constanttable` values |

### V1-WF-003 — Send-out

| Aspect | Detail |
|---|---|
| **Entities read** | `testtable` (`inotherlab`), `patienttest`, `otherlabname` |
| **Entities created** | `otherlabname` entry when a new destination is named |
| **Entities updated** | `patienttest.sentout`, `patienttest.send`, `patienttest.otherlabname` |
| **Entities deleted** | None |
| **Fields involved** | `inotherlab`, `sentout`, `send`, `otherlabname`, `testpriceout`, `testpriceoutmadfo` |
| **State changes** | Queued → dispatched (`send`) → settled (`sentout`) |
| **Relationships traversed** | V1-REL-002, V1-REL-020 |
| **Validation dependencies** | None established |
| **Relevant business rules** | V1-BR-017, 023, 024 |
| **Evidence** | CHM `010.htm`; `patienttest` schema |

### V1-WF-004 — Reference-range administration

| Aspect | Detail |
|---|---|
| **Entities read** | `testtable`, `testunits`, `subgrouptable` |
| **Entities created** | `normalrange` |
| **Entities updated** | `normalrange` |
| **Entities deleted** | `normalrange` |
| **Fields involved** | All 18 `normalrange` columns |
| **State changes** | Range row added / edited / undone / deleted |
| **Relationships traversed** | V1-REL-004, V1-REL-005, V1-REL-006 |
| **Validation dependencies** | Test must exist; urine analysis (code 4) is immutable |
| **Relevant business rules** | V1-BR-011, 016, 018, 019 |
| **Evidence** | CHM `008.htm`, `nots.htm`; `normalrange` schema |

### V1-WF-005 — Report printing

| Aspect | Detail |
|---|---|
| **Entities read** | `patientinfo`, `patienttest`, profile table, `Data\*.ini`, `REPORT.dll`, `SETTING.ini` |
| **Entities created** | None |
| **Entities updated** | `patienttest.resultprint`, `printuser`, `printdate`, `printnum`, `RESULTCASEPRINT` |
| **Entities deleted** | None |
| **Fields involved** | `printwith`, `printuser`, `printdate`, `printnum`, `resultprint` |
| **State changes** | Printed flag set; sequence number allocated |
| **Relationships traversed** | V1-REL-001, V1-REL-002, V1-REL-012 |
| **Validation dependencies** | Result must be reviewed; a printer must be bound and present |
| **Relevant business rules** | V1-BR-020, 021, 022, 033 |
| **Evidence** | CHM `009.htm`; `patienttest` schema; `Data\*.ini` |

### V1-WF-006 — Accounting and stock-take

| Aspect | Detail |
|---|---|
| **Entities read** | `payment`, `patientinfo`, `patienttest`, `Gehats`, `otherlabname`, `useraccounts`, `loggroup` |
| **Entities created** | None established — the stock-take is presented as a calculation |
| **Entities updated** | None established |
| **Entities deleted** | None |
| **Fields involved** | `money`, `paiddate`, `totalprice`, `totalpricedisc`, `omola`, `printuser`, `sentout`, `send` |
| **State changes** | None |
| **Relationships traversed** | V1-REL-007, 009, 011, 016 |
| **Validation dependencies** | The operator's period-scope decision (V1-BR-025) |
| **Relevant business rules** | V1-BR-025, 026, 027 |
| **Evidence** | CHM `011.htm`; `payment`, `Gehats` schema |

### V1-WF-007 — Catalogue maintenance

| Aspect | Detail |
|---|---|
| **Entities read** | `testunits`, `subgrouptable`, `abbreviations`, `converts` |
| **Entities created** | `testtable`, `customtesttable`, `testunits`, `converts`, `abbreviations` |
| **Entities updated** | Same |
| **Entities deleted** | Same (except `testtable`, which carries `deleteprotect`) |
| **Fields involved** | `testcode`, `testname`, `testnamear`, `subgroupcode`, `inotherlab`, `patientprice`, `labprice`, `typeofsample` |
| **State changes** | Test added or modified; range eligibility changed |
| **Relationships traversed** | V1-REL-005, 017, 018 |
| **Validation dependencies** | Test number 4 immutable |
| **Relevant business rules** | V1-BR-016, 017, 029, 030, 031 |
| **Evidence** | CHM `007.htm`, `nots.htm`; catalogue schemas |

---

## 8. Branch template — `Branch_Data.mdb` (49 tables, 0 rows)

| Aspect | Detail |
|---|---|
| **Purpose** | A **schema-only** database used to create the tables of a new branch |
| **Tables** | 49 — the same clinical profile family plus `patientinfo` and `patienttest` |
| **Rows** | **0** — structure only |
| **`patientinfo`** | 33 columns — **identical** to `Patients.mdb` |
| **`patienttest`** | **44 columns** in V-ROOT-1.00 — identical to `Patients.mdb` |
| **Certainty** | DIRECTLY OBSERVED for the schema; INFERRED for the seeding purpose |

> The 2019 template **differs** — 9 columns were removed from `patienttest` — and
> the executable carries `ALTER` statements that add them back. That is documented
> in the V-CD-2019 data model document.

---

## 9. Configuration as a persistence layer

Configuration is not a side concern — it **is** a persistence layer, and for several
functions it is the only store.

| Artefact | Records | Function |
|---|---:|---|
| `Data\*.ini` | 223 files / 114 profiles | Report layout, colours, fonts, printer binding |
| `Cache\*.rtf` | 29 sheets | Method text and critical thresholds |
| `BASE.ini` | 1 | Engine selection, data path, connection identity |
| `SETTING.ini` | 1 | Printer roles, paper, headers, toggles |
| `IDCARD.INI` | 1 | Card / invoice layout and text |
| `REPORT.dll` | 1 (text) | 19-entry report colour palette |
| `NOTE.ini` | 1 | Last-notice state |
| `data_connection*.udl` | 2 | SQL Server connection strings |

> **★ Architectural consequence.** RealLab's knowledge base is **split across the
> database and the file system**. A laboratory that exports only its `.mdb` files
> loses its report layouts, its method sheets, its printer bindings and its
> connection configuration. **Any re-implementation must treat the file-based
> knowledge layer as first-class data.**

---

## 10. Data-model certainty summary

| Area | Directly observed | Source-code identified | Inferred | Uncertain |
|---|---|---|---|---|
| Table and column inventory (93 / 1,527) | ✔ complete | — | — | — |
| Core entity field semantics | ✔ complete | — | — | 4 fields (`kawmy`, `elmadfoo3`, `elbakyma3mal`, `flose`) |
| Profile tables (46) | ✔ complete | — | — | — |
| Knowledge layer (8 tables) | ✔ complete | — | — | — |
| Branch template (49 tables) | ✔ schema | ✔ seeding purpose | ✔ purpose | — |
| `data.mdb` (3 tables) | ✔ schema | — | — | `history` slot semantics |
| Relationships | 15 observed | 7 inferred | — | 7 expected but unsupported |
| Workflow mappings | 7 documented | — | — | — |
| Foreign-key enforcement | — | — | — | **None exists — Jet 4 does not enforce** |
| `pregsmok` participation | ✔ field present | — | — | ✘ **consumer not traced** |
| `abbreviations` → analyte link | — | — | ✘ | ✔ **no column exists** |

---

## 11. Open data-model questions

| ID | Question | Status |
|---|---|---|
| RL-V1-DM-Q-01 | What is the exact segment layout of `patientid`? | UNKNOWN — the `mid(patientid,…)` usage shows positional structure; the semantics are not established |
| RL-V1-DM-Q-02 | Which table is authoritative for `history` — `Patients.mdb` or `data.mdb`? | UNKNOWN |
| RL-V1-DM-Q-03 | How are `normalrange` bands selected when several match? | UNKNOWN |
| RL-V1-DM-Q-04 | Does `pregsmok` participate in range selection? | UNKNOWN — populated on all 852 rows |
| RL-V1-DM-Q-05 | How is `abbreviations` linked to an analyte? | UNKNOWN — no key column exists |
| RL-V1-DM-Q-06 | Are `patienttest` or the profile tables authoritative for a result? | UNKNOWN — both are written |
| RL-V1-DM-Q-07 | What do `elmadfoo3`, `elbakyma3mal` and `kawmy` mean? | UNKNOWN |
| RL-V1-DM-Q-08 | What is `flose` on `Gehats`? | UNKNOWN |
| RL-V1-DM-Q-09 | How are the 46 sub-groups mapped onto 223 `.ini` layouts? | UNKNOWN |
| RL-V1-DM-Q-10 | What is `tasgel`? | UNKNOWN — 11 columns, 0 rows, no references located |
| RL-V1-DM-Q-11 | How are the 45 `DataReport` designs mapped onto the 114 layouts? | UNKNOWN |
| RL-V1-DM-Q-12 | What is `patienttotal` for? | UNKNOWN — 15 columns, 0 rows |

---

**End of Data_Model_and_Workflow_Entities_V1.md**
