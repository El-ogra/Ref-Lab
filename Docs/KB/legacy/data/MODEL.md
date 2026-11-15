# Legacy Data Model — V-ROOT-1.00 and V-CD-2019

**Purpose:** Curated data dictionary for the legacy reference systems.
**Scope:** Database engines, files, tables, columns, rows, keys, constraints, relationships.
**Audience:** Developers, DBAs, architects.

---

## 1. Technology

Access Jet 4 (`.mdb`), with SQL Server Express 2005 and MySQL as alternatives (V2).
Connectivity via Jet OLE DB provider, `SQLOLEDB.1` and `MSDataShape.1` (the "shape"
provider that executes `SHAPE {…} APPEND (…)` union queries).

**Evidence:** external: 06-configuration.md; `Docs/Complete_Reverse_Engineering_Reference_V2.md` §6.1.

## 2. Database inventory

### V-ROOT-1.00

| Database | Tables | Columns | Rows | Purpose |
|---|---:|---:|---:|---|
| `Patients.mdb` | 93 | 1,527 | 3,123 | Primary operational database |
| `Patients_backup.mdb` | 90 | 1,381 | 2,786 | Backup copy (stale in both versions) |
| `Branch_Data.mdb` | 49 | — | 0 | Schema-only branch template |
| `data.mdb` | 3 | — | — | Dynamic history, slim patient index, work-sheet storage |

### V-CD-2019 (v1.605)

| Store | Tables | Columns | Rows | Change vs V1 |
|---|---:|---:|---:|---|
| `Patients.mdb` (2019) | 103 | 1,682 | (shipped partial) | +10 tables, +155 columns |
| `Branch_Data.mdb` (bin copy) | 49 | — | 0 | byte-identical to V1 (SHA-256 `f6d6d5e2…`) |
| `Branch_Data.mdb` (System copy) | 49 | — | 0 | narrower template (966,656 B, SHA-256 `e6c3953e…`) |
| `Patients_backup.mdb` | 90 | 1,381 | 2,786 | unchanged — still stale |
| `data.mdb` | 3 | — | — | +16,384 B (pages) |
| SQL `patients` | — | — | — | (attached, not collected) |
| SQL `Interface` | — | — | — | (attached, belongs to "RealLab Interface" product) |
| SQL `VIDAS` | — | — | — | (attached, never structurally characterised) |

**Evidence:** external: Complete_Reverse_Engineering_Reference_V1.md §6.2; `Docs/Complete_Reverse_Engineering_Reference_V2.md` §6.2.

## 3. Core entities (both versions)

| Table | Columns (V1) | Role |
|---|---:|---|
| `patientinfo` | 33 | Order/visit header — demographics, referral, dates, order-level pricing. `patientid` is the order identifier, not the patient key. |
| `patienttest` | 44 (V1) / 46 (V2) | One row per requested test — the transactional centre. |
| `testtable` | 37 | Test catalogue — 609 rows. |
| `normalrange` | 18 | Reference ranges — 852 rows across 304 tests. |
| `payment` | 6–9 | Payment transactions. |
| `barcodes` | — | Barcode allocation — 35 rows. |
| `history` | 25 (20 dynamic) | Persistent patient attributes (EAV-like). |
| `useraccounts` | — | User accounts — referenced by SQL but present in **no** `.mdb` (SQL Server only). |
| `loggroup` | — | Login group / session grouping. |
| `instrument` | — | Analyser instruments. |
| `abbreviations` | 3 | Abbreviation dictionary — 650 rows. |
| `testunits` | 2 | Unit dictionary — 69 rows. |
| `converts` | 6 | Unit conversion factors — 167 rows. |
| `subgrouptable` | 4 | Clinical sub-group catalogue — 46 rows. |
| `constanttable` | 7 | Clinical constants (CBC, PT). |
| `PTC` | 2 | PT control curve (time ↔ concentration, 33 points). |
| `Gehats` | 2 | Referring parties — 10 rows, including commission rate `omola`. |
| `anyreport` | — | Report selection — 17 rows. |
| `comments` | — | Standard result comments — 12 rows. |
| `antibiotic` / `antibio` | 25 / 2 | Antibiotics (38 rows) and sensitivity scenarios (95 rows). |
| `customtesttable` / `customgrouptable` | 5 / 3 | Custom profiles with prices (54 / 6 rows). |

**Evidence:** external: 02-data-model.md; external: Complete_Reverse_Engineering_Reference_V1.md §6.3; `Docs/Complete_Reverse_Engineering_Reference_V2.md` §6.3–6.7.

## 4. V2 new tables (10)

| Table | Columns | Purpose | Evidence |
|---|---:|---|---|
| `Helicobacter` | 7 | *H. pylori* results (`hpab`, `hpagi`, `hpigm`, `hpag`, `hpbiop`, `hpiga`, `comment`) | DDL + SQL |
| `insulin` | — | Insulin assay results | SQL |
| `IgEallergy` | — | IgE allergy (repeatable-group pattern, `Item1..Item40+`) | DDL; **ABSENT from shipped DB** |
| `patienttestgroup` | — | Test-grouping entity | DDL |
| `SMSSender` | 14 | SMS message queue | DDL |
| `SMStemplate` | 4 | SMS message templates | DDL |
| `smsgroups` | 6 | SMS recipient groups | DDL |
| `smslist` | 10 | SMS recipient list | DDL |
| `MyLabSetting` | — | Laboratory-level settings (`LabID`) | SQL |
| `testtemp` | 1 | Temporary test rows | DDL |
| `Paste Errors` | — | Clipboard/paste error capture | SQL |

**Evidence:** `Docs/Complete_Reverse_Engineering_Reference_V2.md` §6.3.

## 5. Clinical profile tables (46 in V1; extended in V2)

46 profile result tables, each carrying the analyte set for one clinical discipline. Full catalogue
and per-table field detail is in `Data_Model_and_Workflow_Entities_V1.md` §3 (external) and
`Docs/Data_Model_and_Workflow_Entities_V2.md`.

Representative members: `cbc` (20 cols: hgb, rbc, hct, plt, …), `liver`, `kidney`, `urine` (58),
`stool` (58), `semen` (59), `crossmat` (84), `trace` (25), `cultures` (25), `cultureanti`,
`hepatitis` (20), `IMMUNOLOGY` (19), `hormones`, `Lipids`, `TORCH`, `BloodGases`,
`Electrolytes` (20), `iron`, `Diabetes` set, `Pregnancy`, `ProstaticX` (45), `Stone`,
`Widal`, `CSFT` (22), `MOH`, `Abuse`, `Drugs`, `Fertlity`, `Pancreas`, `Suprarenal`,
`Tumors`, `Vitamins`, `ZN`, `ESR`, `CRP`, `Cardiac`, `GlucoseTolerance`,
`HgbElectrophoresis`, `ProteinElectrophoresis`, `TB`/`TUBERCULINT`, `BONEMAR` (23), `BILH`.

V2 additions: `hba1c`, `ratios`, `widal2`, `FMF` (absent), `LBC` (absent), `insulin`,
`IgEallergy` (absent), `culturesec`.

**Evidence:** external: Complete_Reverse_Engineering_Reference_V1.md §6.4; `Docs/Complete_Reverse_Engineering_Reference_V2.md` §6.7.

## 6. Knowledge layer (identical in both versions)

| Table | Rows | Contents |
|---|---:|---|
| `testtable` | 609 | Test name, abbreviation, price, profile, send-out flag |
| `normalrange` | 852 | Per test: sex × age window; low/high abnormality limits; exception text; critical range and critical comment |
| `constanttable` | 9 (2 groups) | `CBCCONS` (5 values) + `PTCONS` (4 values) |
| `testunits` | 69 | Unit dictionary |
| `converts` | 167 | Unit conversion factors |
| `abbreviations` | 650 | Abbreviation dictionary |
| `subgrouptable` | 46 | Clinical sub-groups |
| `PTC` | 33 | PT control curve |

`normalrange` distribution: sex Male 422 / Female 421 / All 5 / empty 4; age unit Years 708 /
Days 70 / Mons. 65 / empty 9; age span Years 0–120, Days 1–120, Mons. 0–24; numeric low limit
on 459 rows, high on 643; exception text 511 low / 581 high; 19 distinct low and 19 distinct
high comments; 51 distinct measurement units.

**★ OBSERVED (not a documented rule):** all 852 `normalrange` rows carry a value in `pregsmok`
(Single). This implies a pregnancy/smoking-dependent range dimension. The manual does not mention
it and the code path that consults it was not traced.

**Evidence:** external: Complete_Reverse_Engineering_Reference_V1.md §6.6; external: 04-clinical-rules.md.

## 7. Keys, constraints and relationships

Access Jet 4 does not enforce foreign keys; relationships are declarative/observed. Foreign-key
constraints are **not enforced** in the shipped files. Relationships in `Data_Model_and_Workflow_Entities_V1.md`
§6 are classified OBSERVED (exact column-name pairing plus a real join in an extracted SQL statement)
or INFERRED (exact column-name pairing only).

No `MSysIndexes` or `MSysRelationships` table exists in any of the 9 databases, so Access declares
no primary keys and no foreign keys at all (K-01, K-02 UNRESOLVED).

**Evidence:** external: Complete_Reverse_Engineering_Reference_V1.md §6.5; external: 1605-phase2-findings.md (K-01, K-02).

## 8. Status, history and audit structures

| Structure | Description |
|---|---|
| `history` | 25 columns in `data.mdb` — `selector`, `labid`, `requestdate`, `column1..column20` (free-form attribute slots), `printflag`, `noofcol`. One row per patient. |
| `loggroup` | Login grouping. |
| `patienttest.resultprint` / `.printuser` / `.printdate` / `.printnum` | Print audit trail on the test row. |
| `SETTING.ini [setup] reportstart=1984` | Absolute report numbering origin. |
| `labdb` / `labdb1` | Programmatic backup/restore. |
| `SMSSender` (V2) | Outbound-message audit: `smsMobile`, `SMStext`, `SMSLength`, `requestdate`. |
| `reportview` (V2) | 42 columns — saved report views. |
| `MyLabSetting` (V2) | Laboratory-level configuration. |
| `loger` (V2) | New attendance/event log. |

## 9. Configuration tables

| Table | Contents |
|---|---|
| `constanttable` | Clinical computation constants (see `clinical/KNOWLEDGE-LAYER.md` / `RULES.md`). |
| `subgrouptable` | 46 clinical sub-groups (e.g. `BLOOD GLUCOSE`, `KIDNEY FUNCTIONS`, `LIVER FUNCTIONS`, `LIPIDS PROFILE`, `URINE EXAMINATION`, `STOOL EXAMINATION`, `Autoimmune diseases`, `BLOOD GROUP`, `Erythrocyte Sedimentation Rate`, `CULTURE AND SENSITIVITY`, `BLOOD PICTURE`). |
| `patientinfo` price fields | `totalprice`, `totalpricedisc` — patient account totals. |
| `Gehats` | Referring parties, with `omola` = commission. |
| `otherlabname` | Send-out destination laboratories. |
| `anyreport` | 17 report selections. |

## 10. Version applicability

- Core tables, knowledge layer, and clinical rules: **SHARED** (identical in both versions).
- DDL/migration, multi-branch, NATIGH/SMS, PDF, statistics, work sheets, farm reporting, extended permission model, new profile windows, new tables: **V2 ONLY**.
- `patientid` positional identifier: **VERSION-DEPENDENT** (branch segment is a V2 addition at position 9; person key and display code are SHARED).
- `useraccounts` table: **V2 ONLY** (SQL-Server-only in both, but referenced by V2 DDL).
- `Branch_Data.mdb` `bin\` copy: **SHARED** (byte-identical); `System\` copy: **V2 ONLY**.

## 11. Known limitations

- No primary keys or foreign keys are declared in any Access database (K-01, K-02).
- `VIDAS` database never structurally characterised.
- `win32_processor` table purpose unknown.
- `loger` table purpose (attendance vs clinical) unknown.
- `insulin` and `antibio` vs `antibiotic` unknown.
- `pricemethod` value domain not established.
- `pregsmok` dimension not traced.
- Band-selection algorithm among overlapping reference ranges not established.
- Which `Branch_Data.mdb` copy is loaded not established.
- 38 columns in Access tables were never applied by the migration (partial deployment).
- `Patients_backup.mdb` stale in both versions.

## 12. Source preservation

Original detailed data dictionary is preserved in `Database documentation.md` (root, 209 KB) and
`Docs/Data_Model_and_Workflow_Entities_V2.md`. This `MODEL.md` is a curated reference, not a
replacement. Do not delete or truncate the originals.

**Evidence:** external: 02-data-model.md; external: Complete_Reverse_Engineering_Reference_V1.md §6;
`Docs/Complete_Reverse_Engineering_Reference_V2.md` §6; `Database documentation.md` (root).
