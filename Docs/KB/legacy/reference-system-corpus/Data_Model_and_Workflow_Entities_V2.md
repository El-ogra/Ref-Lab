# Data Model and Workflow Entities — RealLab V-CD-2019 (v1.605)

**Document ID:** RL-V2-DM-001
**Version documented:** `V-CD-2019`
**Evidence basis:** 473 verified artefacts · static analysis only
**Companion documents:** `Complete_Reverse_Engineering_Reference_V2.md`, `Functional_Specification_V2.md`, `Business_Rules_V2.md`

---

## 1. Scope, method and certainty model

### 1.1 What this document covers

The **shipped 2019 `Patients.mdb` in full** — 103 tables, 1,682 columns — together
with the 49-table branch template, the `data.mdb` stores, the SQL Server catalogues
referenced by the installer, and the 268 embedded schema-migration statements
reconciled against the shipped schema.

### 1.2 Certainty tiers

| Tier | Meaning |
|---|---|
| **DIRECTLY OBSERVED** | Read from the extracted schema, row counts or sample data. |
| **SOURCE-CODE IDENTIFIED** | Present in the executable's SQL, literals or object model. |
| **INFERRED FROM ESTABLISHED BEHAVIOUR** | A reasonable reading of documented behaviour. |
| **UNCERTAIN / INCOMPLETE** | Known to exist but incompletely characterised. |

> **★ Structural warning — carried forward unchanged from the Version 1 data model.**
> Microsoft Access (Jet 4) does **not** enforce foreign keys, and the shipped
> `.mdb` contains **no enforced referential constraints**. Every relationship in §6
> is classified as OBSERVED (exact column-name pairing **plus** a real join in an
> extracted SQL statement) or INFERRED (exact column-name pairing only).

### 1.3 Identifier scheme

`V2-ENT-nnn` (entity) · `V2-FLD-nnn` (field) · `V2-REL-nnn` (relationship) ·
`V2-WF-nnn` (workflow-to-entity mapping).

---

## 2. Entity catalogue

### 2.1 Store-level summary

| Store | Tables | Columns | Rows | Change vs V-ROOT-1.00 |
|---|---:|---:|---:|---|
| `Patients.mdb` (2019) | **103** | **1,682** | — | **+10 tables, +155 columns** |
| `Branch_Data.mdb` (2019, `bin\` copy) | 49 | — | 0 | **★ byte-identical to the Version 1 file** (SHA-256 `f6d6d5e2…`) — it did **not** narrow |
| `Branch_Data.mdb` (2019, `System\` copy) | — | — | 0 | **★ A SECOND, DIFFERENT template** (966,656 B, SHA-256 `e6c3953e…`) — the narrower one |
| `Patients_backup.mdb` | 90 | 1,381 | 2,786 | **unchanged — still stale** |
| `data.mdb` | 3 | — | — | — |
| SQL catalogue `patients` | — | — | — | **new** |
| `Interface` database | — | — | — | **attached by the installer; belongs to the separately installed "RealLab Interface" product** |
| SQL database `VIDAS` | — | — | — | **new — nature UNKNOWN** |

### 2.2 The 103 tables of the shipped `Patients.mdb`

**Core (18):**

| Entity ID | Technical name | Purpose | Cols | Category | Certainty |
|---|---|---|---:|---|---|
| **V2-ENT-001** | `patientinfo` | Order/visit header — demographics, referral, order-level pricing | **39** | core | DIRECTLY OBSERVED |
| **V2-ENT-002** | `patienttest` | One row per requested test — the transactional centre | **46** | core | DIRECTLY OBSERVED |
| **V2-ENT-003** | `patients` | **★ OPERATOR TABLE** — `patientusname`, `patientword`, `patientad`, **`grant1`…`grant17`**, `branche`, `SecWord`, `startwork`, `endwork`, `breakyorn`, `breaktime`, `khasmlimit` | **35** | security | DIRECTLY OBSERVED |
| **V2-ENT-004** | `testtable` | Test catalogue — 609 rows | 37 | knowledge | DIRECTLY OBSERVED |
| **V2-ENT-005** | `normalrange` | Reference ranges — 852 rows over 304 analytes | 18 | knowledge | DIRECTLY OBSERVED |
| **V2-ENT-006** | `constanttable` | Clinical computation constants | 7 | knowledge | DIRECTLY OBSERVED |
| **V2-ENT-007** | `subgrouptable` | Clinical sub-group catalogue — 46 rows | 4 | knowledge | DIRECTLY OBSERVED |
| **V2-ENT-008** | `testunits` | Unit dictionary — 69 rows | 2 | knowledge | DIRECTLY OBSERVED |
| **V2-ENT-009** | `converts` | Analyte-specific conversion factors — 167 rows | 6 | knowledge | DIRECTLY OBSERVED |
| **V2-ENT-010** | `abbreviations` | Abbreviation dictionary — 650 rows | 3 | knowledge | DIRECTLY OBSERVED |
| **V2-ENT-011** | `comments` | Standard result comments | 5 | knowledge | DIRECTLY OBSERVED |
| **V2-ENT-012** | `payment` | Payment/receipt lines | 9 | finance | DIRECTLY OBSERVED |
| **V2-ENT-013** | `patienttotal` | Per-patient aggregate test sheet | 15 | finance | DIRECTLY OBSERVED |
| **V2-ENT-014** | `Gehats` | Referring parties with commission | 19 | finance | DIRECTLY OBSERVED |
| **V2-ENT-015** | `Labbranch` | **Branch master — 2 columns only** | 2 | branches | DIRECTLY OBSERVED |
| **V2-ENT-016** | `barcodes` | Barcode allocation | 2 | patient | DIRECTLY OBSERVED |
| **V2-ENT-017** | `history` | Persistent patient attributes — 20 free-form slots | 25 | patient | DIRECTLY OBSERVED |
| **V2-ENT-018** | `wasl` | Work-sheet storage | 12 | workflow | DIRECTLY OBSERVED |

**Messaging (4) — new in v1.605:**

| Entity ID | Technical name | Purpose | Cols | Certainty |
|---|---|---|---:|---|
| **V2-ENT-019** | `SMSSender` | **Outbound SMS queue and audit** | 8 | DIRECTLY OBSERVED |
| **V2-ENT-020** | `SMStemplate` | SMS message templates | 2 | DIRECTLY OBSERVED |
| **V2-ENT-021** | `smslist` | SMS recipient list | 6 | DIRECTLY OBSERVED |
| **V2-ENT-022** | `smsgroups` | SMS recipient groups | 3 | DIRECTLY OBSERVED |

**Laboratory configuration (3) — new in v1.605:**

| Entity ID | Technical name | Purpose | Cols | Certainty |
|---|---|---|---:|---|
| **V2-ENT-023** | `MyLabSetting` | Laboratory-level settings — **`accuntID` and `accuntpw`** (NATIGH account), `sendername`, `SMSNo`, `totalsms`, `SMSSend`, six phone numbers, `Timess`, `FridaySend`, `shortsms` | 20 | DIRECTLY OBSERVED |
| **V2-ENT-024** | `pricelist` | Price lists | — | SOURCE-CODE IDENTIFIED |
| **V2-ENT-025** | `pricemenu` | Price menu | — | SOURCE-CODE IDENTIFIED |

> **★ Security note on `MyLabSetting`.** The table stores a NATIGH account
> identifier **and its password** (`accuntID`, `accuntpw`) in plain text, alongside
> the laboratory's own telephone numbers and SMS accounting counters. This is a
> **new credential store introduced in v1.605** — no equivalent existed in
> Version 1.

**Clinical and operational (new in v1.605):**

| Entity ID | Technical name | Purpose | Cols | Certainty |
|---|---|---|---:|---|
| **V2-ENT-026** | `Helicobacter` | *H. pylori* results | 6 | DIRECTLY OBSERVED |
| **V2-ENT-027** | `patienttestgroup` | Test grouping — **46 columns mirroring `patienttest`**, keyed by `maintestcode` + `testcode` | 46 | DIRECTLY OBSERVED |
| **V2-ENT-028** | `insulin` | Insulin assay list — **2 columns only** (`id`, `insulin`) | 2 | DIRECTLY OBSERVED |
| **V2-ENT-029** | `testtemp` | Temporary test rows — 28 columns mirroring `patienttest` with `detelethis` | 28 | DIRECTLY OBSERVED |
| **V2-ENT-030** | `reportview` | Saved report views — 30 `strip` text blocks + 10 `stripb` flags | 42 | DIRECTLY OBSERVED |
| **V2-ENT-031** | `sharekat` | Farm records | — | SOURCE-CODE IDENTIFIED |
| **V2-ENT-032** | `sharekathesab` | Farm account records | — | SOURCE-CODE IDENTIFIED |
| **V2-ENT-033** | `shopping` | Shopping / purchase records | — | SOURCE-CODE IDENTIFIED |
| **V2-ENT-034** | `antibiotic` | Antibiotic catalogue — 38 rows / 25 cols | 25 | DIRECTLY OBSERVED |
| **V2-ENT-035** | `antibio` | Antibiotic — reduced form — 96 rows | 2 | DIRECTLY OBSERVED |
| **V2-ENT-036** | `daytip` | Localised help strings (20 Arabic tips with F2/F3/F7 shortcuts) | — | SOURCE-CODE IDENTIFIED |
| **V2-ENT-037** | `noteic` | Notice records | — | SOURCE-CODE IDENTIFIED |
| **V2-ENT-038** | `testname` | Test-name mapping for the reporting sheets | 8 | DIRECTLY OBSERVED |
| **V2-ENT-039** | `titless` | Title records | — | SOURCE-CODE IDENTIFIED |
| **V2-ENT-040** | `sarfeda3` | — | — | UNCERTAIN |

**Profile result tables (51 — the 50 of Version 1 plus `Helicobacter`)** — `ABO`, `Abuse`, `BILH`, `BloodGases`, `BONEMAR`,
`cardiac`, `cbc`, `CoagFactors`, `crossmat`, `CRP`, `CSFT`, `cultureanti`,
`cultures`, `culturesec`, `Drugs`, `Electrolytes`, `ESR`, `Fertlity`,
`GLUCOSETOLERANCE`, `hba1c`, `hepatitis`, `Hgbectrophoresis`, `hormones`,
`Immunoglobulin`, `IMMUNOLOGY`, `iron`, `kidney`, `Lipids`, `liver`, `Pancreas`,
`postcoitalt`, `pregnancy`, `Proelectrophoresis`, `prostaticx`, `pt`, `ratios`,
`renalfailure`, `semen`, `stone`, `stool`, `sugar`, `Suprarenal`, `TORCH`,
`trace`, `TUBERCULINT`, `urine`, `Vitamin`, `widal`, `widal2`, `ZN`.

> **★ `patienttestgroup` is a structural repeat of `patienttest`.** All 46 columns
> mirror the transactional table, but it is keyed by **two** test codes —
> `maintestcode` and `testcode` — instead of one. This is the schema's only
> **many-to-many** structure: a *group* of tests recorded as a unit, each with its
> own result slot. It is the data-side counterpart of the `subgrouptable` grouping,
> and it explains the `arrangnum` ordering field on `patienttest`.

> **`testtemp` is the transaction table's shadow.** Its 28 columns mirror
> `patienttest` exactly, plus `detelethis` (a "delete this" marker). It is a
> staging table for rows that are assembled and then either committed or discarded.

### 2.3 The ten new tables

| Table | Cols | Created by the DDL? | In the shipped DB? |
|---|---:|---|---|
| `Helicobacter` | 6 | ✔ yes | ✔ present |
| `MyLabSetting` | 20 | no (base schema) | ✔ present |
| `Paste Errors` | — | no | ✔ present |
| `SMSSender` | 8 | ✔ yes | ✔ present |
| `SMStemplate` | 2 | ✔ yes | ✔ present |
| `insulin` | 2 | no | ✔ present |
| `patienttestgroup` | 46 | no | ✔ present |
| `smsgroups` | 3 | ✔ yes | ✔ present |
| `smslist` | 6 | ✔ yes | ✔ present |
| `testtemp` | 28 | ✔ yes | ✔ present |

---

## 3. Field-level documentation — core entities

### 3.1 `patientinfo` — V2-ENT-001 (39 columns)

**Version 1 baseline:** 33 columns. **Version 2 additions (6):**

| Field | Type | Business meaning | Related rules |
|---|---|---|---|
| `PFax` | Text | Patient fax number | — |
| `PatRPW` | Text | Referring physician — **the V2 split of the referral concept** | V2-BR-042 |
| `SMSMob` | Text | **Patient mobile number — the SMS delivery target** | V2-BR-007 |
| `gehaName` | Text | Referring-party name (the V2 name field; the DDL target is spelled `gehaname` — **a different identifier**) | V2-BR-042 |
| `PBirthDay` | DateTime | **Date of birth — new in V2** | — |
| `moreinfo2` | Text | Second additional-information field | — |

> **★ The most consequential V2 addition to `patientinfo` is `PBirthDay`.**
> Version 1 carried `patientage` and `patientageunit` — an **age snapshot**.
> Version 2 adds an actual **date of birth**, alongside the age fields. The
> reference-range band selection uses `firstold`/`secondold` against the patient's
> age, so with a birth date the system can — at least in principle — recompute the
> age rather than rely on a stored snapshot. **Whether it actually does so is NOT
> ESTABLISHED**, but the field's presence is direct evidence that the age-vs-birth-date
> ambiguity was recognised.
>
> `patientinfo` also gains the SMS mobile number, which is the **delivery target**
> for the messaging subsystem.

### 3.2 `patienttest` — V2-ENT-002 (46 columns)

**Version 1 baseline:** 44 columns. **Version 2 additions (2):**

| Field | Type | Business meaning | Related rules |
|---|---|---|---|
| `Timeout` | Text | Per-test turnaround timeout (free text) | — |
| `Taketime` | Integer | Time taken for the test | — |

**Version 1 baseline (44 columns) retained unchanged**, including the denormalised
reference-range freeze (`normalrang`, `lowlimit`, `highlimit`, `nolowexe`,
`nohighexe`), the four result text slots (`RESULTT`, `resultt1`–`resultt4`), the
lifecycle flags (`resultfinsh`, `resultreview`, `resultprint`, `give`, `sentout`,
`send`) and the full audit set (`edituser`/`editdate`, `prevuser`/`prevdate`,
`printuser`/`printdate`/`printnum`, `taslimuser`/`taslimdate`).

> **★ `patienttest.natighu` does NOT exist in the shipped 2019 database.**
> The DDL defines it; the shipped table does not have it. The NATIGH delivery
> feature is therefore **code-present but schema-absent** (V2-BR-005).

### 3.3 `patients` — V2-ENT-003 — ★ THE OPERATOR TABLE (35 columns)

| Field | Type | Business meaning | Related rules |
|---|---|---|---|
| `patientusid` | Long | Operator id | — |
| `patientusname` | Text (40) | Operator name | — |
| `patientuscap` | Text (40) | Role / capability caption | V2-BR-039 |
| `patientword` | Text (40) | **Operator password — plain text** | V2-BR-032 |
| `patientad` | Boolean | **Administrator flag** | V2-BR-039 |
| `patientus` | Boolean | Active flag | — |
| **`grant1` … `grant17`** | Boolean × 17 | **★ Seventeen function-level permission bits** | **V2-BR-039** |
| `showtip` | Boolean | Show the tip panel | — |
| `tipnum` | Long | Current tip index | — |
| `startwork` | Text (15) | **Attendance — work start** | V2-BR-041 |
| `endwork` | Text (15) | **Attendance — work end** | V2-BR-041 |
| `breakyorn` | Boolean | On break | V2-BR-041 |
| `breaktime` | Text (15) | Break time | V2-BR-041 |
| `khasmlimit` | Long | Special / private limit | — |
| **`branche`** | Integer | **★ The operator's branch — the source of the branch digit at identifier position 9** | **V2-BR-008** |
| **`SecWord`** | Text | Security word | — |
| `lastvisit` | Text (40) | Last visit (V1 also carries it) | — |

> **★★ Three structural findings that revise the Version 1 reading.**
>
> 1. **The table named `patients` holds operators, not patients.** The name is a
>    legacy artefact of the pre-rebranding product. Any reader who assumes
> otherwise will mis-map the entire schema.
> 2. **The permission model is on the operator, not on the test row.** Version 1
>    already carried **twelve** bits; v1.605 extends it to **seventeen**. The
>    earlier characterisation of the permission model as a V2-only feature was
>    wrong.
> 3. **The branch belongs to the operator.** `patients.branche` is an Integer on
>    the operator record; the branch digit is written into position 9 of every
>    patient identifier that operator creates. **A patient inherits its branch
>    from the operator who registered it** — which means a patient's branch is
>    only as reliable as the registration context.

### 3.4 `Labbranch` — V2-ENT-015 (2 columns)

| Field | Type | Key | Business meaning |
|---|---|---|---|
| `branchid` | Long | **PK** | Branch identifier |
| `branchname` | Text | | Branch name |

> **★ A two-column table carries the entire branch master.** There is no address,
> no contact, no accounting configuration, no opening date. The branch is
> essentially a **numeric code and a label** — and its operational substance lives
> in the per-branch database schema seeded from `Branch_Data.mdb`.

> **★ Note the type mismatch.** `patients.branche` is `Integer`, while
> `patientinfo.labid` is `Text (13)`. The branch recorded on the operator
> (`patients.branche`, integer) and the "lab" recorded on the patient order
> (`patientinfo.labid`, text) are **different columns with different types** — the
> link between them is inferred, not observed, and no join was located.

### 3.5 `SMSSender` — V2-ENT-019 (8 columns)

| Field | Type | Business meaning |
|---|---|---|
| `SMSautonum` | Long | Message sequence number |
| `username` | Text | The operator who sent it |
| `patientid` | Text | The patient the message concerns |
| `PatDrName` | Text | The doctor's name shown in the message |
| `requestdate` | DateTime | When the message was queued |
| `smsMobile` | Text | **Recipient mobile number — clear text** |
| `SMSLength` | Integer | Message length |
| `SMStext` | Text | **Message body — clear text** |

> The DDL describes a 14-column variant (`SMSautonum` identity, `smsMobile`
> varchar(20), `SMSLength` smallint, `SMStext` varchar(MAX)); the shipped Access
> table is the **8-column** form with `varchar(255)` equivalents. This is one of
> the two places where the Access-specific variant of the DDL was applied.

### 3.6 `smslist`, `smsgroups`, `SMStemplate`

| Table | Columns | Notes |
|---|---|---|
| `smslist` | `ID` Long · `smstype` Integer · `smsphone` Text · `smsname` Text · `smsgroup` Text · **`send` Integer** | The `send` column is **new relative to the DDL** — a per-recipient send state |
| `smsgroups` | `ID` Long · `grouptype` Integer · `smsgroup` Text | The value domains of `smstype` and `grouptype` are NOT ESTABLISHED |
| `SMStemplate` | `templatenum` Long · `smstext` Text | Reusable message templates |

### 3.7 `MyLabSetting` — V2-ENT-023 (20 columns) — ★ a new credential store

| Field | Type | Business meaning |
|---|---|---|
| `LabID` | Integer | Laboratory identifier |
| `sendername` | Text | The sender's name on outgoing messages |
| **`accuntID`** | Text | **NATIGH service account identifier** |
| **`accuntpw`** | Text | **★ NATIGH service account PASSWORD — plain text** |
| `updateflage` | Byte | Update flag |
| `Hourflag` | Integer | Hour-based flag |
| `SMSNo` | Integer | SMS number |
| `totalsms` | Double | Total SMS counter |
| `SMSSend` | Double | Sent-SMS counter |
| `SMSbakey` | Double | SMS balance key / counter |
| `phoneNo1` … `phoneNo6` | Text | **Six laboratory telephone numbers** |
| `phonenum` | Integer | Telephone-number count |
| `Timess` | Text | SMS timing schedule |
| `FridaySend` | Byte | Friday send flag |
| `shortsms` | Byte | Short-SMS flag |

> **★ New in v1.605, and a security-relevant store.** `accuntID` and `accuntpw`
> hold the NATIGH service credential in plain text inside the laboratory database.
> This is a **second** cleartext credential store, in addition to the database
> connection credential in `BASE.ini`. Neither existed in this form in Version 1.

### 3.8 `Helicobacter` — V2-ENT-026 (6 columns)

| Field | Type | Business meaning |
|---|---|---|
| `patientid` | Text | Patient |
| `requestdate` | DateTime | Date |
| `hpigi` | Text | *H. pylori* antigen — IgG |
| `hpigm` | Text | *H. pylori* antigen — IgM |
| `hpag` | Text | *H. pylori* antigen |
| `comment` | Memo | Comment |

> The DDL's `ALTER TABLE Helicobacter ADD hpab, hpbiop, hpiga` targets **three
> further antigen columns — and all three are absent** from the shipped table.
> The shipped table has only the original three.

### 3.9 `patienttestgroup` — V2-ENT-027 (46 columns) — ★ the only many-to-many structure

Mirrors `patienttest` exactly, with one addition: **`maintestcode`** (Integer),
placed alongside `testcode`.

| Field | Type | Business meaning |
|---|---|---|
| `requestdate` | DateTime | Date |
| `patientid` | Text | Patient |
| **`maintestcode`** | Integer | **The group / parent test** |
| `testcode` | Integer | The member test |
| `subgroupcode` | Integer | Clinical sub-group |
| `RESULTT`, `resultt1`–`resultt4` | Text | Result slots |
| `lowlimit`, `highlimit`, `nolowexe`, `nohighexe`, `normalrang` | | **The same range freeze as `patienttest`** |
| `resultfinsh`, `resultreview`, `resultprint`, `give`, `sentout`, `send` | Boolean | The same lifecycle flags |
| `printuser`, `printdate`, `printnum`, `edituser`, `editdate`, `prevuser`, `prevdate`, `taslimuser`, `taslimdate` | | The same audit set |
| `commentx`, `testabrev`, `testname`, `historyname`, `testunit` | | The same denormalised snapshots |
| `arrangnum`, `testtime`, `testprice`, `testpriceout`, `testpriceoutmadfo`, `otherlabname`, `loggroup`, `printwith`, `RESULTCASEPRINT`, `Timeout`, `Taketime` | | The same operational fields |

> **★ The significance.** A test can be a **member of a group**, and the group
> records its own members' results. `maintestcode` → `testcode` is the schema's
> only many-to-many relationship, and it explains the `arrangnum` ordering field.
> A laboratory that runs a "renal function panel" or a "pre-operative screen" can
> record the panel as one order while still capturing each member's result
> individually — with the same range-freeze, the same audit trail and the same
> review gate as a standalone test.

### 3.10 `testtemp` — V2-ENT-029 (28 columns)

| Field | Type | Business meaning |
|---|---|---|
| `patientid` | Text | Patient |
| `requestdate` | DateTime | Date |
| `testcode` | Long | Test |
| `subgroupcode` | Long | Sub-group |
| `testsimp`, `testabrev`, `testname`, `historyname`, `testunit` | Text | Snapshots |
| `RESULTT` | Text | Result |
| `normalrang` | Memo | Range freeze |
| `lowlimit`, `highlimit`, `nolowexe`, `nohighexe` | | Range freeze |
| `resultfinsh`, `resultreview`, `resultprint`, `give`, `sentout` | Boolean | Lifecycle flags |
| `testprice`, `testpriceout`, `testpriceoutmadfo` | Double | Pricing |
| `otherlabname`, `loggroup`, `arrangnum`, `testtime`, `printwith` | | Operational |
| **`detelethis`** | Long | **Marker — "delete this row"** |

> **★ `testtemp` is a staging table with an explicit discard marker.**
> `detelethis` is the only field in the entire schema whose name states a
> deletion intent. The pattern — assemble rows here, mark them for deletion, then
> commit or discard — is the schema's own acknowledgement that a request can be
> abandoned before it becomes a transaction.

---

## 4. Knowledge-layer structures

### 4.1 Unchanged between the versions

| Table | V1 | V2019 | Verdict |
|---|---:|---:|---|
| `testtable` | 609 rows | **609 rows** | **identical** |
| `normalrange` | 852 rows | **852 rows** | **identical** |
| `subgrouptable` | 46 rows | **46 rows** | **identical** |
| `testunits` | 69 rows | **69 rows** | **identical** |
| `converts` | 167 rows | **167 rows** | **identical** |
| `abbreviations` | 650 rows | **650 rows** | **identical** |
| `constanttable` | 5 + 4 values | **identical** | **identical** |

> **★ The single most important data-model conclusion of this package:**
> **the clinical knowledge layer is byte-for-byte the same in 2013 and 2020.**
> Seven years and 10 new tables later, the catalogue of 609 tests, the 852
> reference ranges, the 46 clinical sub-groups, the 69 units, the 167 conversion
> factors and the 650 abbreviations are unchanged. **Whatever changed in the
> product, the laboratory's clinical knowledge did not.**

### 4.2 `subgrouptable` — the Arabic-name gap

| Field | Type | Content |
|---|---|---|
| `subgroupcode` | Long Integer | PK — codes 1–48 with 36 and 46 unused |
| `subgroupname` | Text (50) | The English name |
| `arname` | Text (50) | **The Arabic name — EMPTY for all 46 shipped rows** |
| `deleteprotect` | Boolean | `1` for 43 rows; `0` for codes 45, 47, 48 |

> The three unprotected sub-groups — `WBCs function`, `Renal Failure`,
> `Diabetes profile` — are the three most recently added, which is direct
> evidence of how the catalogue was extended.

---

## 5. Workflow-to-entity mapping

### V2-WF-001 — Patient episode

| Aspect | Detail |
|---|---|
| **Entities read** | `barcodes`, `Gehats`, `testtable`, `normalrange`, `subgrouptable`, `patients` (the operator's branch) |
| **Entities created** | `patientinfo`, `barcodes`, `patienttest`, profile result rows |
| **Entities updated** | `patientinfo` (edit fields, `totalprice`, `totalpricedisc`), `patienttest` (results, `resultreview`, `give`, `print*`), `history` |
| **Entities deleted** | None established |
| **Fields involved** | `patientid` (position 9 = the operator's branch), `patientnamear`, `patientsex`, `patientage`, `patientageunit`, `PBirthDay`, `barcode`, `requestdate`, `testcode`, `subgroupcode`, `resultreview`, `give`, `printuser`, `printdate`, `printnum` |
| **State changes** | Patient created → tests ordered → results entered → `resultreview` → printed → `give` |
| **Relationships traversed** | V2-REL-001 … V2-REL-010 |
| **Validation dependencies** | A matching `normalrange` row |
| **Relevant business rules** | V2-BR-008, 034, 035, 038, 040, 045 |
| **Evidence** | CHM `003.htm`–`005.htm`; the shipped schemas |

### V2-WF-002 — Branch operation

| Aspect | Detail |
|---|---|
| **Entities read** | `Labbranch`, `patients` |
| **Entities created** | `Labbranch` rows; a branch database seeded from `Branch_Data.mdb` |
| **Entities updated** | `patients.branche` (the operator's branch assignment) |
| **Entities deleted** | None |
| **Fields involved** | `Labbranch.branchid`, `Labbranch.branchname`, `patients.branche`, `patientid` position 9 |
| **State changes** | An operator is assigned to a branch; identifiers created by that operator thereafter carry the branch digit |
| **Relationships traversed** | V2-REL-011, V2-REL-012 |
| **Validation dependencies** | — |
| **Relevant business rules** | V2-BR-008, 009, 010, 025 |
| **Evidence** | Vendor guide 5-2; `Labbranch`; `patients.branche`; `MID(patientid,9,1)` |

### V2-WF-003 — PDF production

| Aspect | Detail |
|---|---|
| **Entities read** | `patientinfo`, `patienttest`, profile table, `Data\*.ini` |
| **Entities created** | The PDF file under `D:\PDF\<category>\` |
| **Entities updated** | `patienttest.resultprint`, `printuser`, `printdate`, `printnum` |
| **Entities deleted** | None |
| **Fields involved** | `printwith`, `printuser`, `printdate`, `printnum`, `resultprint` |
| **State changes** | The printed flag is set and a sequence number allocated |
| **Relationships traversed** | V2-REL-001, V2-REL-002 |
| **Validation dependencies** | Result reviewed; the `Bullzip PDF Printer` device present |
| **Relevant business rules** | V2-BR-001, 002, 003, 004, 045 |
| **Evidence** | The literals; `SetupPZ.bat`; `analysis/BATCH-11/` |

### V2-WF-004 — Runtime schema migration

| Aspect | Detail |
|---|---|
| **Entities read** | The database catalogue |
| **Entities created** | 12 candidate tables — **10 created, 2 (`FMF`, `LBC`) not** |
| **Entities updated** | 29 candidate tables — **109 of 132 column targets applied, 19 not** |
| **Entities deleted** | None |
| **Fields involved** | See §6.9 |
| **State changes** | The schema advances toward the v1.605 shape |
| **Relationships traversed** | — |
| **Validation dependencies** | The engine must support the statement dialect |
| **Relevant business rules** | V2-BR-023, V2-BR-024 |
| **Evidence** | `analysis/BATCH-04/schema/`; the shipped schema |

### V2-WF-005 — Digital delivery

| Aspect | Detail |
|---|---|
| **Entities read** | `patientinfo` (`SMSMob`, `patientnamear`), `patienttest`, `smslist`, `smsgroups`, `SMStemplate`, `MyLabSetting` (`accuntID`, `accuntpw`, `sendername`) |
| **Entities created** | `SMSSender` rows; the external publication |
| **Entities updated** | `smslist.send` |
| **Entities deleted** | None |
| **Fields involved** | `smsMobile`, `SMStext`, `SMSLength`, `PatDrName`, `smsphone`, `smsgroup`, `templatenum`, `smstext` |
| **State changes** | A message is queued and dispatched; a result is published to NATIGH.COM |
| **Relationships traversed** | V2-REL-013, V2-REL-014, V2-REL-015 |
| **Validation dependencies** | **`patienttest.natighu` does not exist in the shipped database** |
| **Relevant business rules** | V2-BR-005, 006, 007, 022 |
| **Evidence** | The DDL; `MyLabSetting`; the `SMSfrmsenderos` form |

### V2-WF-006 — Statistics

| Aspect | Detail |
|---|---|
| **Entities read** | `patientinfo` (`requestdate`, `patientsex`), `patienttest` (`subgroupcode`) |
| **Entities created / updated / deleted** | None established — computed on demand |
| **Fields involved** | `requestdate`, `patientsex`, `subgroupcode` |
| **State changes** | None |
| **Relationships traversed** | V2-REL-001, V2-REL-003 |
| **Validation dependencies** | — |
| **Relevant business rules** | V2-BR-011 |
| **Evidence** | Vendor guide 4-1 … 4-5; the `statisReport` object |

### V2-WF-007 — Accounting and stock-take

| Aspect | Detail |
|---|---|
| **Entities read** | `payment`, `patientinfo`, `patienttest`, `Gehats`, `otherlabname`, `patients` (operator names), `loggroup` |
| **Entities created / updated / deleted** | None established — presented as a calculation |
| **Fields involved** | `money`, `paiddate`, `totalprice`, `totalpricedisc`, `omola`, `elmadfoo3`, `printuser`, `sentout`, `send`, `patientid` position 9 |
| **State changes** | None |
| **Relationships traversed** | V2-REL-006, V2-REL-007, V2-REL-008, V2-REL-016 |
| **Validation dependencies** | The operator's period-scope decision; the branch scope |
| **Relevant business rules** | V2-BR-010, 026, 042 |
| **Evidence** | CHM `011.htm`; the SQL literal for the commission formula |

---

## 6. Relationships

### 6.1 OBSERVED — exact column pairing **and** a join in extracted SQL

| Rel ID | Parent | Child | Cardinality | Join columns | Evidence |
|---|---|---|---|---|---|
| **V2-REL-001** | `patientinfo` | `patienttest` | 1:N | `patientinfo.patientid` = `patienttest.patientid` | Extracted SQL joins; `Text` ↔ `Text` |
| **V2-REL-002** | `testtable` | `patienttest` | 1:N | `testtable.testcode` = `patienttest.testcode` | Extracted SQL joins |
| **V2-REL-003** | `subgrouptable` | `patienttest` | 1:N | `subgrouptable.subgroupcode` = `patienttest.subgroupcode` | Extracted SQL joins |
| **V2-REL-004** | `testtable` | `normalrange` | 1:N | `testtable.testcode` = `normalrange.testcode` | Exact pairing, `Long Integer` ↔ `Long Integer`; 852 rows over 304 analytes |
| **V2-REL-005** | `subgrouptable` | `testtable` | 1:N | `subgrouptable.subgroupcode` = `testtable.subgroupcode` | Exact pairing |
| **V2-REL-006** | `patientinfo` | `payment` | 1:N | `patientinfo.patientid` = `payment.patientid` | Exact pairing |
| **V2-REL-007** | `patienttest` | `payment` | 1:N | `patienttest.patientid` = `payment.patientid` | Exact pairing; `payment.testsimp` mirrors `patienttest.testsimp` |
| **V2-REL-008** | `Gehats` | `payment` | 1:N | `Gehats.gehahsefa` = `payment.gehah` | Exact pairing; the commission formula joins on the same pair |
| **V2-REL-009** | `patientinfo` | `history` | 1:1 | `patientinfo.labid` = `history.labid` | Exact pairing |
| **V2-REL-010** | `patienttest` | profile tables | 1:1 | `patientid` + `requestdate` | Profile tables carry the same patient key and date |
| **V2-REL-011** | `Labbranch` | `patients` | 1:N | `Labbranch.branchid` = `patients.branche` | Exact pairing; `Long` ↔ `Integer` — **the branch assignment** |
| **V2-REL-012** | `Labbranch` | `patientinfo` | 1:N | **NOT ESTABLISHED** — `patientinfo.labid` is `Text (13)` and `Labbranch.branchid` is `Long`; the types are incompatible and no join was located | — |
| **V2-REL-013** | `smsgroups` | `smslist` | 1:N | `smsgroups.smsgroup` = `smslist.smsgroup` | Exact pairing, `Text` ↔ `Text` |
| **V2-REL-014** | `patientinfo` | `SMSSender` | 1:N | `patientinfo.patientid` = `SMSSender.patientid` | Exact pairing |
| **V2-REL-015** | `patientinfo` | `smslist` | 1:N | `patientinfo.SMSMob` ≈ `smslist.smsphone` | Exact-name pairing; no join located |
| **V2-REL-016** | `patients` (operator) | `patienttest` | 1:N | `patients.patientusname` ↔ `patienttest.edituser` / `printuser` / `taslimuser` | Exact-name pairing; **no join located** |
| **V2-REL-017** | `patienttest` | `patienttestgroup` | 1:N | `patienttest.testcode` = `patienttestgroup.maintestcode` | Exact pairing — **the group-to-member link** |
| **V2-REL-018** | `patienttest` | `testtemp` | 1:1 | `patientid` + `requestdate` + `testcode` | The staging table mirrors the transaction table |
| **V2-REL-019** | `comments` | `testtable` | N:1 | `comments.testcode` = `testtable.testcode` | Exact pairing |

### 6.2 INFERRED — exact column pairing only

| Rel ID | Parent | Child | Cardinality | Basis | Why only inferred |
|---|---|---|---|---|---|
| **V2-REL-020** | `testtable` | `converts` | 1:N | `testtable.testsimp` = `converts.testsimp` | Textual key; no join located |
| **V2-REL-021** | `testtable` | `comments` | 1:N | `testtable.testsimp` = `comments.testsimp` | Textual key; no join located |
| **V2-REL-022** | `patientinfo` | `patienttotal` | 1:N | Both keyed by `patientid` | Table semantics not established |
| **V2-REL-023** | `testtable` | `otherlabname` | 1:N | `testtable.otherlabname` | Destination table's own columns not established |
| **V2-REL-024** | `smslist` | `SMSSender` | 1:N | `smslist.smsphone` ≈ `SMSSender.smsMobile` | No join located |
| **V2-REL-025** | `SMStemplate` | `SMSSender` | 1:N | `SMStemplate.smstext` ≈ `SMSSender.SMStext` | No join located; no template id on the queue row |

### 6.3 NOT ESTABLISHED

| Expected relationship | Why it is not established |
|---|---|
| `patientinfo` → `patienttest` **enforced** | Jet 4 does not enforce foreign keys; no constraint object exists |
| `abbreviations` → `testtable` | `abbreviations` has **no** test-code column — unchanged from V1 |
| `normalrange` → `patienttest` | The link is by frozen copy, not by key |
| `patientinfo` → profile tables | Profile tables carry their own patient metadata copy |
| `useraccounts` → anything | **`useraccounts` exists in no `.mdb` file** |
| `loger` → anything | The `loger` column structure was not extracted |
| `patienttest` → `payment` at the line-item level | `payment` has no `testcode` |
| `insulin` → `patienttest` | `insulin` has only `id` and `insulin`; no test-code column |
| `MyLabSetting` → `Labbranch` | `MyLabSetting.LabID` is `Integer` and `Labbranch.branchid` is `Long`; no join located |

---

## 7. ★ The schema migration, reconciled — the definitive table

### 7.1 Created tables — 12 DDL targets

| Table | In the shipped 2019 DB | Note |
|---|---|---|
| `Helicobacter` | ✔ present | 3 of its 7 DDL columns absent |
| `SMSSender` | ✔ present | 8-column Access variant applied |
| `SMStemplate` | ✔ present | |
| `hba1c` | ✔ present | |
| `pictures` | ✔ present | 73 columns |
| `ratios` | ✔ present | |
| `smsgroups` | ✔ present | |
| `smslist` | ✔ present | plus a `send` column not in the DDL |
| `testtemp` | ✔ present | |
| `widal2` | ✔ present | |
| **`FMF`** | ✘ **ABSENT** | DDL created it; it is not in the shipped database |
| **`LBC`** | ✘ **ABSENT** | DDL created it; it is not in the shipped database |

### 7.2 Added columns — 132 distinct targets across 29 tables

| Result | Count | Detail |
|---|---:|---|
| **Applied and present** | **109** | across 26 tables |
| **NOT applied / absent** | **19 targets / 23 columns** | 17 columns on tables that exist, plus 6 columns (3 on `useraccounts`, 3 on `IgEallergy`) on the two tables that do not — listed below |

**The 19 unapplied column targets:**

| Table | Column(s) | Reason |
|---|---|---|
| `BloodGases` | `aniongap`, `ctco2`, `cthb`, `fio2`, `hco3std`, `po2aa1`, `po2aa2`, `temprat` | **8 of 8 absent** — the whole blood-gas extension is missing |
| `Helicobacter` | `hpab`, `hpbiop`, `hpiga` | The table exists; its three extended antigen columns do not |
| `IgEallergy` | `item12`, `item24`, `item36` | **The `IgEallergy` table is absent from the shipped database entirely** |
| `Proelectrophoresis` | `beta2`, `immunolgo` | Absent |
| `lipids` | `nonhdl` | Absent |
| `patienttest` | `natighu` | Absent — **the NATIGH delivery flag does not exist** |
| `pregnancy` | `normalrange3` | Absent |
| `urine` | `testid` | Absent |
| `useraccounts` | `gtype`, `hdd1`, `hdd2` | **The `useraccounts` table is absent from the shipped database entirely** |

### 7.3 The full picture

| Object class | In the DDL | Applied | Not applied |
|---|---:|---:|---:|
| Created tables | 12 | **10** | **2** |
| Added columns | 132 | **109** | **19** |
| | | **119 / 144 = 82.6%** | **21 / 144 = 17.4%** |

> **★ Consequences visible in the shipped file.**
>
> | Feature | Storage state in the shipped 2019 Access database |
> |---|---|
> | IgE allergy panel | **No table at all** — the 40-item panel, its entry form (`resultIgeAllergy`, 231 methods) and its report (`ReportIGE1`) exist in code only |
> | User accounts / permission descriptors | **No table at all** — the operator table is `patients` |
> | NATIGH delivery recording | **No column** — `patienttest.natighu` absent |
> | Extended blood-gas panel | **All 8 new columns absent** |
> | *H. pylori* extended antigens | **3 of 7 absent** |
> | Mediterranean fever (FMF) profile | **No table** |
> | LBC profile | **No table** |
>
> **The shipped Access database is a partial deployment of the v1.605 schema.**
> This is not a configuration difference — it is a database that does not match the
> executable that reads it.

---

## 8. Configuration as a persistence layer

| Artefact | Records | Role | Change vs V1 |
|---|---:|---|---|
| `Data\*.ini` | 223 files / 114 profiles | Report layout, margins, colours, fonts, header/footer, background, printer binding | **173 of 223 differ** |
| `Cache\*.rtf` | 29 sheets | Method text and critical thresholds | **byte-identical** |
| `BASE.ini` | 1 | Engine selection, data path, connection identity | unchanged |
| `SETTING.ini` | 1 | Printer roles, paper, headers, toggles | extended |
| `IDCARD.INI` | 1 | Card / invoice / connection-receipt layout | extended |
| `REPORT.dll` | 1 (text) | 19-entry report colour palette | unchanged |
| `NOTE.ini` | 1 | Last-notice state | unchanged |
| `data_connection*.udl` | 2 | SQL Server connection strings | unchanged |
| `SETUP.LST` | 39-file specification | Installation | **new** |
| `System\Setup*.bat` | 7 | Prerequisite installers | **new** |
| `System\PDF\*` | 7 | PDF stack | **new** |
| `System\datareport_vh\*` | 73 | VB6 SP6 redistributable | **new** |
| `System\MDAC_TYP\*` | 1 | MDAC TypeLib installer | **new** |

---

## 9. Data-model certainty summary

| Area | Directly observed | Source-code identified | Inferred | Uncertain |
|---|---|---|---|---|
| Table inventory (103 / 1,682) | ✔ complete | — | — | — |
| Core entity fields | ✔ complete | — | — | 4 fields (`kawmy`, `elmadfoo3` role in business terms, `flose`, `grouptype`/`smstype` domains) |
| The 10 new tables | ✔ complete | — | — | — |
| The 50 profile tables | ✔ inventory | — | — | — |
| Knowledge layer (7 tables) | ✔ complete | — | — | — |
| Operator table `patients` | ✔ complete (35 cols) | — | — | — |
| Branch template (49 tables) | ✔ schema | ✔ seeding purpose | ✔ purpose | — |
| `data.mdb` (3 tables) | ✔ schema | — | — | `history` slot semantics |
| Relationships | 19 observed | 6 inferred | — | 9 expected but unsupported |
| Workflow mappings | 7 documented | — | — | — |
| Foreign-key enforcement | — | — | — | **None exists** |
| Migration reconciliation | ✔ complete | — | — | — |
| `loger` structure | — | ✔ table exists | — | ✘ **columns not extracted** |
| `useraccounts` structure | — | ✔ SQL-referenced | — | ✘ **not in any `.mdb`** |
| `VIDAS` structure | — | ✔ attached | — | ✘ **entirely UNKNOWN** |
| `sharekat` / `sharekathesab` / `pricelist` / `insulin` structures | partial | ✔ tables exist | — | ✘ **not fully extracted** |

---

## 10. Open data-model questions

| ID | Question | Status |
|---|---|---|
| RL-V2-DM-Q-01 | What is the `VIDAS` database, and is it Visual FoxPro? | UNKNOWN |
| RL-V2-DM-Q-02 | What are the `loger` columns? | UNKNOWN — the table was not extracted at field level |
| RL-V2-DM-Q-03 | Where are the user accounts stored in Access mode? | **`patients` is the operator table; `useraccounts` exists in no `.mdb`** |
| RL-V2-DM-Q-04 | How does `patientinfo.labid` relate to `Labbranch.branchid`? | UNKNOWN — the types are incompatible and no join was located |
| RL-V2-DM-Q-05 | What is the relationship between `antibiotic` (25 cols) and `antibio` (2 cols)? | UNKNOWN |
| RL-V2-DM-Q-06 | What are the `smstype` and `grouptype` value domains? | UNKNOWN |
| RL-V2-DM-Q-07 | What is `insulin` used for — a catalogue or a result table? | UNKNOWN |
| RL-V2-DM-Q-08 | What is `smslist.send` (not in the DDL)? | UNKNOWN |
| RL-V2-DM-Q-09 | What is `patienttotal` for? | UNKNOWN — 15 columns, no semantics established |
| RL-V2-DM-Q-10 | What is `ratios`, and what is computed in it? | UNKNOWN |
| RL-V2-DM-Q-11 | Why is the DDL's `gehaname` a different identifier from the shipped `gehaName`? | UNKNOWN — a probable spelling divergence between the two schema sources |
| RL-V2-DM-Q-12 | How is the `patients.branche` assignment made — per operator, per session, or per installation? | UNKNOWN |
| RL-V2-DM-Q-13 | What is `sarfeda3`? | UNKNOWN |
| RL-V2-DM-Q-14 | What is `SecWord` used for? | UNKNOWN |

---

**End of Data_Model_and_Workflow_Entities_V2.md**
