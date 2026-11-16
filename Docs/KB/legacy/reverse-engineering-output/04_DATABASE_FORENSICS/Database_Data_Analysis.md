# Database Data Analysis — REAL LAB SYSTEM v1.605

## 0. Privacy boundary

All table contents are treated as potentially sensitive. **No patient name, patient ID,
barcode value, phone number or other PII is reproduced.** Only schema, row counts,
enumerations, knowledge-base statistics and synthetic/anonymised structural descriptions
appear below. The per-table sample CSVs are themselves redacted (`<redacted>` values in
`pricelist.csv`, `grouptable.csv`, `subgrouptable.csv`, `testunits.csv`, `abbreviations.csv`,
etc.). Where a percentage is given, the denominator is stated in the same phrase.

## 1. Row counts by store (with denominators)

### 1.1 v1.00 `Patients.mdb` sample — 93 tables, 3,123 rows in total

Source: `rowcounts__[analysis_BATCH-02_db_Patients].csv` (totals recomputed by summing
the `rows` column across all 93 rows); corroborated by `db-summary.csv` row 1.

| Table | Rows | Columns | Note |
|---|---:|---:|---|
| `testtable` | 609 | 37 | test catalogue |
| `normalrange` | 852 | 18 | reference intervals, 304 analytes |
| `abbreviations` | 650 | 3 | dictionary |
| `converts` | 167 | 6 | conversion factors |
| `patienttest` | 222 | 44 | ordered tests |
| `testunits` | 69 | 2 | units dictionary |
| `subgrouptable` | 46 | 4 | sub-groups |
| `Gehats` | 10 | 19 | referring parties |
| `loggroup` | 8 | 2 | log groups |
| `payment` | 6 | 9 | receipts |
| `constanttable` | 2 | 7 | PTCONS / CBCCONS |
| `patients` | 2 | 26 | **operator** accounts, not patients |
| `patientinfo` | **0** | 33 | patient sample redacted/empty |
| `pictures` | 2 | 73 | attachment staging |

### 1.2 `Patients_backup.mdb` — 90 tables, 2,786 rows in total

Source: `rowcounts__[analysis_BATCH-02_db_Patients_backup].csv` (recomputed);
`db-summary.csv` row 2.

| Table | Rows | Columns |
|---|---:|---:|
| `normalrange` | 836 | 22 |
| `testtable` | 609 | 36 |
| `abbreviations` | 650 | 3 |
| `insulin` | 113 | 2 |
| `converts` | 50 | 6 |
| `subgrouptable` | 45 | 5 |
| `grouptable` | 15 | 4 |
| `Gehats` | 10 | 19 |
| `patienttest` | 0 | 44 |
| `patientinfo` | 0 | 33 |

**CONFLICTED:** `normalrange` is 852 rows / 18 columns in the `Patients` sample but 836
rows / 22 columns in the backup; `converts` is 167 vs 50. These are different snapshots
of the same logical store, not one database. `rowcounts.csv` (BATCH-04 corpus) lists 47
tables all with 0 rows; it describes a *statement-derived* surface, not a MDB row count —
do not mix the two.

### 1.3 `Branch_Data.mdb` and `data.mdb`

Both 0 rows (`db-summary.csv` rows 3–4; `rowcounts__[analysis_BATCH-02_db_data].csv`
shows `history` 0, `patientinfo` 0, `wasl` 0). They are templates/empty stores.

## 2. Lookup and reference table semantics

| Table | Rows (source) | Semantics | Confidence |
|---|---:|---|---|
| `abbreviations` | 650 (`rowcounts__[analysis_BATCH-02_db_Patients].csv`) | `abbid`, `abbre`, `mean` — result abbreviation → meaning | STRONGLY_SUPPORTED |
| `testunits` | 69 (same) | `unitsid`, `unitsname` — unit dictionary | STRONGLY_SUPPORTED |
| `converts` | 167 (same) | `testsimp`, `sunit`, `funit`, `sfix`, `ffix`, `factors` — per-analyte conversion | STRONGLY_SUPPORTED |
| `subgrouptable` | 46 (same) | `subgroupcode`, `subgroupname`, `arname`, `deleteprotect` — catalogue grouping | STRONGLY_SUPPORTED |
| `grouptable` | 15 (`rowcounts__[analysis_BATCH-02_db_Patients_backup].csv`) | `groupcode`, `groupname`, `arname`, `deleteprotect` | STRONGLY_SUPPORTED |
| `antibio` / `antibiotic` | 95 / 38 (Patients sample rowcounts) | reduced / full antibiotic susceptibility dictionaries | STRONGLY_SUPPORTED |
| `constanttable` | 2 (`constanttable.csv`) | `PTCONS`, `CBCCONS` clinical computation constants with six numeric slots | VERIFIED |
| `pricelist` | 5 (`rowcounts__[analysis_BATCH-02_db_Patients].csv`) | `pricenum`, `testsimp`, `pricelistname`, `testprice` — values redacted in CSV | STRONGLY_SUPPORTED |

`constanttable.csv` values are clinical numbers, not PII; the two rows are
`PTCONS` (constants in slots 1–3 populated) and `CBCCONS` (slots 1–6 populated). Used by
derivation rules (see `Database_Business_Logic.md`).

## 3. `testtable` knowledge-base statistics

Source: `testtable-stats.json` (computed over **609 test rows** — the denominator for
every line below); corroborated by `rowcounts__[analysis_BATCH-02_db_Patients].csv`
(`testtable` = 609) and `Data_Model_and_Workflow_Entities_V2.md` §2.2.

| Property | Count of 609 | Share | Interpretation |
|---|---:|---:|---|
| `delete_protected` | 605 | 99.3% of 609 | deletion guard on catalogue rows |
| `with_patient_price` | 591 | 97.0% of 609 | priced tests |
| `with_otherlab_price` | 591 | 97.0% of 609 | send-out price present |
| `active` | 94 | 15.4% of 609 | active flag set |
| `sent_out_to_other_lab` | 1 | 0.16% of 609 | `inotherlab` set |
| `tests_with_sample_type` | 292 | 48.0% of 609 | `typeofsample` populated |
| `tests_with_ref_range_flag` | 13 | 2.1% of 609 | `REFRANGE` populated |
| `tests_with_arabic_name` | 6 | 1.0% of 609 | `testnamear` populated |
| `tests_with_clinical_description` | 1 | 0.16% of 609 | `testdiscrp` populated |
| `tests_with_normal_type` | 0 | 0% of 609 | `normaltype` unused |
| `distinct_subgroups_used` | 48 | — | 48 distinct `subgroupcode` values |

Top subgroups by test count (denominator 609): unassigned/`?0` 118 (19.4% of 609),
`Separate` 50 (8.2%), `BLOOD PICTURE` 39 (6.4%), `SERODIAGNOSIS` 37 (6.1%),
`Trace Elements` 22 (3.6%), `Autoimmune diseases` 20 (3.3%),
`KIDNEY FUNCTIONS` 19 (3.1%), `MOLECULAR BIOLOGY` 18 (3.0%),
`CULTURE AND SENSITIVITY` 16 (2.6%), `Hepatitis Markers` 16 (2.6%). The full 48-entry
distribution is in `testtable-stats.json`.

## 4. `normalrange` knowledge-base statistics

Source: `normalrange.csv` — **852 data rows over 304 distinct `testcode` values**
(denominator for every line below); corroborated by
`rowcounts__[analysis_BATCH-02_db_Patients].csv` (`normalrange` 852 rows, 18 columns)
and `Data_Model_and_Workflow_Entities_V2.md` §2.2 ("852 rows over 304 analytes").
Columns: `normalid`, `testcode`, `testsimp`, `subgroupcode`, `sex`, `firstold`,
`secondold`, `ageunit`, `pregsmok`, `normalrang`, `testunit`, `lowlimit`, `highlimit`,
`nolowexe`, `nohighexe`, `lowcomment`, `highcomment`, `REFRANGE`.

**Sex domain (852 rows):** `Male` 422 (49.5%), `Female` 421 (49.4%), `All` 5 (0.6%),
blank 4 (0.5%). This is a sex-stratified reference model: roughly one interval per sex
per analyte/age band.

**Age-unit domain (852 rows):** `Years` 708 (83.1%), `Days` 70 (8.2%), `Mons.` 65
(7.6%), blank 9 (1.1%). Age bands are expressed by `firstold`/`secondold` plus
`ageunit`, not by a numeric age column on the patient record alone.

**Top `testunit` values (852 rows):** blank 215 (25.2%), `mg/dL` 105 (12.3%),
`U/L` 55 (6.5%), `ug/dL` 51 (6.0%), `ng/mL` 41 (4.8%), `%` 40 (4.7%),
`ng/dL` 38 (4.5%), `pg/mL` 35 (4.1%), `ulU/mL` 28 (3.3%), `U/mL` 27 (3.2%),
`Ul/mL` 22 (2.6%), `IU/mL` 18 (2.1%), `min` 16 (1.9%), `mlU/mL` 13 (1.5%),
`g/dL` 12 (1.4%). The `Ul/mL` vs `uIU/mL` vs `mlU/mL` variants are
**UNVERIFIED** as distinct units versus data-entry variants — do not normalise without
domain confirmation.

`pregsmok` (pregnancy/smoking qualifier) exists as a column; its use in computation is
listed as an open question in `08-unknowns.md` (N-3) — **UNVERIFIED**.

## 5. Status values and enumerations (schema-grounded)

| Enumeration | Values | Source / denominator |
|---|---|---|
| `normalrange.sex` | `Male`, `Female`, `All`, blank | `normalrange.csv`, 852 rows |
| `normalrange.ageunit` | `Years`, `Days`, `Mons.`, blank | `normalrange.csv`, 852 rows |
| Result lifecycle (`patienttest`) | `resultfinsh`, `resultreview`, `resultprint`, `give`, `sentout`, `send`, `RESULTT`, `RESULTCASEPRINT`, `resultt1`…`resultt4` | `v2019-Patients.mdb.sql`; `tables.csv` sample statements; `Data_Model_and_Workflow_Entities_V2.md` §3.2 |
| Catalogue flags (`testtable`) | `active`, `deleteprotect`, `inotherlab`, `testorno`, `seereport`, `printwith`, `barcode1`…`barcode3` | `v2019-Patients.mdb.sql`; `testtable-stats.json` (609 tests) |
| Boolean/YESNO | two-valued by type; no CHECK constraint | 80 `Boolean` columns in `v2019-Patients.mdb.sql`; 7 `YESNO` tokens in DDL evidence |
| `titless.titlesex` | queried with `titlesex='Male'` | `tables.csv` row `titless` (10 statements) |
| `patients` permission bits | `grant1`…`grant17` Boolean, `patientad` Boolean, `patientus` Boolean | `v2019-Patients.mdb.sql`; `Data_Model_and_Workflow_Entities_V2.md` §3.3 |

Enumerations above are the values *observed in the data or schema*; where a column is
free text (`pricemethod`, `testusage`, `resultt`), no closed domain is asserted.

## 6. Identifier patterns and the branch-position change

- `patientinfo.patientid` is **`Text (13)`** — a 13-character **composite order/visit
  identifier**, not a patient master key (`v2019-Patients.mdb.sql`;
  `Data_Model_and_Workflow_Entities_V1.md` §1.1a; `Documentation_Quality_Report.md`).
  No real identifier value is reproduced here.
- **Positional derivations evidenced in SQL:**
  `MID(patientid,3,2)` = person key (`Batch1_Analysis_Report.md` §5.3;
  `Data_Model_and_Workflow_Entities_V1.md` — used both as patient filter and as
  `COUNT(MID(patientid,3,2))` in turnover reports);
  `mid(patientid,10,3)` = display code, and `SUBSTRING(patientid,9,1)` = branch code
  (`02-data-model.md`; `Stage-3-V-CD-2019-Baseline-Report.md` §8;
  `Data_Model_and_Workflow_Entities_V2.md` §3.3).
- **Branch-position change described in prior work:** in V-CD-2019 (v1.605) the branch
  digit occupies **position 9** and its source is the operator's `patients.branche`
  Integer (`Business_Rules_V1.md` "Structural constraint"; `Business_Rules_V2.md`
  V2-BR-008; `Functional_Specification_V2.md` "Known limitations"). In the modern
  single-site Ref Lab reconstruction, **position 9 was freed and folded into the middle
  segment** while the identifier remains 13 characters (`AGENTS.md` rule 8;
  `From-Phase-0-to-4-Handoff.md` §3.2.3 "The 13-character `OrderIdentifier` with position 9
  freed"; `06-Transformation-Baseline-and-Handover.md` — layout 2/2/5/3/1). This is the
  documented cross-era change; it is a **design decision in the new system**, not an
  observed mutation of the legacy MDB.
- **Type mismatch warning:** `patients.branche` is Integer while `patientinfo.labid` is
  `Text (13)`; they are different columns, not interchangeable branch keys
  (`Data_Model_and_Workflow_Entities_V2.md` §3.4).
- Other SQL idioms on the identifier: `RIGHT`/`MID`/`INSTR` extraction and the sentinel
  `'<<< Blank >>>'` for a patient with no name (`02-data-model.md`;
  `Stage-2b-Cumulative-Analysis-Report.md`).

## 7. Profile result tables

53 of the 103 shipped tables follow the "wide per-test panel" shape: a `patientid
Text (13)` (usually `NOT NULL`), a `requestdate DateTime`, a set of analyte text columns
and a `comment` column (`v2019-Patients.mdb.sql`). Examples with their shipped column
counts: `crossmat` 84, `stool` 61, `semen` 60, `urine` 60, `prostaticx` 53, `patienttest`
46 (transactional), `cultures` 35, `cbc` 29, `widal2` 26. Their result values are stored
as **Text**, not typed numerics (`RLS_v1.00_Analysis_Report.md` §6.1) — validation lives
in the application.

`pictures` (73 columns) is the widest non-profile table and is heavily migrated at
runtime (84 ALTER statements in `database-engineering-reference.md` §3.2) — it is an
attachment staging structure, not a patient master.
