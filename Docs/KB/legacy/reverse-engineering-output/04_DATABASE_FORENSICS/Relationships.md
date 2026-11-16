# Relationships — REAL LAB SYSTEM v1.605

## 0. Method and confidence rubric

No `.mdb` was opened; **no declared foreign keys exist** (0 `FOREIGN KEY` / `REFERENCES`
tokens in `v2019-Patients.mdb.sql`, `ddl-all.txt` and
`ddl-evidence__[analysis_BATCH-04_sql_v1605].txt`). Relationships are therefore
reconstructed from three evidence classes:

- **EVIDENCED** — an explicit `JOIN … ON` or `SHAPE … RELATE` clause in
  `statements__[analysis_BATCH-04_sql_v1605].csv` (VA address cited) or in
  `tables__[analysis_BATCH-04_sql_v1605].csv`;
- **INFERRED** — exact column-name pairing / naming convention only, with no explicit
  join found;
- **UNVERIFIED** — asserted in a prior report but not reproducible from the primary
  dumps or statement corpus.

Jet 4 does not enforce referential integrity; every relationship below is
**application-enforced**.

## 1. EVIDENCED relationships (explicit joins / RELATE clauses)

Counts are recomputed from the 305 statements containing `JOIN` in
`statements__[analysis_BATCH-04_sql_v1605].csv`.

| # | Parent → child | Join predicate | Occurrences | Example VA / source |
|---|---|---|---:|---|
| R01 | `patientinfo` → `patienttest` | `patientinfo.patientid = patienttest.patientid` | **60** (39 written child-first, 21 parent-first) | `6CD858`, `6D4848`, `6DEB6C`, `700DCC` (`statements__[analysis_BATCH-04_sql_v1605].csv`) |
| R02 | `normalrange` → `testtable` | `normalrange.testcode = testtable.testcode` | **13** | `tables__[analysis_BATCH-04_sql_v1605].csv` row `normalrange` (sample SHAPE statement); `columns.csv` shows `normalrange.testcode` referenced 16× |
| R03 | `patienttest` → `testtable` | `patienttest.testcode = testtable.testcode` | **6** | `statements__[analysis_BATCH-04_sql_v1605].csv` (join-pairs census) |
| R04 | `testname` → `patienttest` | `testname.testcode = patienttest.testcode` | **2** (SHAPE + INNER JOIN) | `70ECE4`, `70F278` |
| R05 | `subgrouptable` → `testtable` | `RELATE subgroupcode TO subgroupcode` (SHAPE APPEND) | **3** | `672722`, `6729D6` |
| R06 | `testtable` → `normalrange` | `RELATE testcode TO testcode` (SHAPE APPEND) | **4** | `672E3E` |
| R07 | `patientinfo` → each per-test profile table | `patientinfo.patientid = <profile>.patientid` | **6 per table**, ≥26 profile tables | `cbc` `6DE45C`; `pt` `6CAFD4`; `crp` `6C9D18`; `hormones` `6CC6A8`; `Pancreas` `6D9588`; `cardiac` `6E3374`; `Tumors` `6E472C`; `Vitamins` `6E5CA4`; `trace` `6E727C`; `pregnancy` `6E9BE8`; `iron` `6EAEC4`; `Fertlity` `6EC4DC`; `Suprarenal` `6EDD10`; `Abuse` `7101AC`; `Drugs` `7116D8`; `CoagFactors` `7129C8`; `Immunoglobulin` `713FC8`; `IMMUNOLOGY` `715558`; `Hgbectrophoresis` `717D3C`; `Proelectrophoresis` `71A518`; `BloodGases` `71BCF4`; `GLUCOSETOLERANCE` `71DCA8`; `renalfailure` `723CAC`; `hba1c` `72DA74`; `ratios` `73003C`; `Helicobacter` `7314DC` |
| R08 | `patientinfo` → `payment` | `patientinfo.patientid = payment.patientid` | **6** | `tables.csv` / join-pairs census |
| R09 | `patienttest` → `testtable` (profile SHAPE parent) | `RELATE testcode TO testcode` in barcode/report SHAPE | included in R04/R06 corpus | `70ECE4` |

**Important negative finding on the composite key.** Although each profile table carries
both `patientid` and `requestdate` (`v2019-Patients.mdb.sql`), **every explicit join
found uses `patientid` alone**; `requestdate` occurs only in `WHERE` clauses
(e.g. `6C9D18`, `6D4848`, `6FFAFC`). A two-column key
`(patientid, requestdate)` is therefore **INFERRED**, not EVIDENCED.

## 2. INFERRED relationships (naming / schema-shape only)

| # | Parent → child | Basis | Confidence |
|---|---|---|---|
| R10 | `grouptable.groupcode` → `subgrouptable.groupcode` | column naming; `grouptable` 4 cols, `subgrouptable` 4 cols in `v2019-Patients.mdb.sql`; used together in the catalogue SHAPE (`tables.csv` row `subgrouptable`) | INFERRED |
| R11 | `testtable.subgroupcode` → `subgrouptable.subgroupcode` | naming; `tables.csv` shows `subgrouptable.subgroupcode` 8 references and the SHAPE at `672722` relates `subgroupcode` | STRONGLY_SUPPORTED (RELATE present, direction from naming) |
| R12 | `testtable.testsimp` → `pricelist.testsimp` | naming; `SELECT * FROM pricelist where testsimp='` (`tables.csv` row `pricelist`, 11 statements) | INFERRED |
| R13 | `testtable.testsimp` → `converts.testsimp` | naming; 6-column conversion table (`v2019-Patients.mdb.sql`) | INFERRED |
| R14 | `testtable.testcode` → `comments.testcode` | `SELECT comments FROM comments where testcode in (` (`tables.csv` row `comments`, 25 statements) | STRONGLY_SUPPORTED |
| R15 | `testtable.testcode` → `customtesttable.testcode` | joined projection in `tables.csv` row `customtesttable`: `customtesttable.testcode, testtable.testabrev, …` | STRONGLY_SUPPORTED |
| R16 | `patients.patientusname` ↔ `patientinfo.labid` / `patientinfo.mowzaf` | naming; `columns.csv`: `patientinfo.labid` 198 references, `patientinfo.referredbyname` 32; operator login query `select startwork,endwork,breakyorn,breaktime from patients where patientusname= '` (`tables.csv` row `patients`) | INFERRED |
| R17 | `Labbranch.branchid` ↔ `patients.branche` | naming; **type mismatch**: `patients.branche` Integer vs `patientinfo.labid` Text(13) (`Data_Model_and_Workflow_Entities_V2.md` §3.4) | INFERRED / type-mismatch warning |
| R18 | `testtable.testcode` → `patienttestgroup.maintestcode` and `.testcode` | structural repetition of `patienttest` (46 cols) with two test-code columns (`Data_Model_and_Workflow_Entities_V2.md` §2.2, §2.3) | STRONGLY_SUPPORTED |
| R19 | `patienttest` → `testtemp` (staging shadow) | 28-column mirror of `patienttest` plus `detelethis` (`Data_Model_and_Workflow_Entities_V2.md` §2.2) | INFERRED |
| R20 | `patienttest.patientid` + `requestdate` → profile tables | both columns present in every profile table; explicit joins use `patientid` only (R07) | INFERRED |
| R21 | `normalrange.testcode` → `testtable.testcode` denormalised into `patienttest` (`normalrang`, `lowlimit`, `highlimit`, `nolowexe`, `nohighexe`) | snapshot columns in `patienttest`; `Data_Model_and_Workflow_Entities_V2.md` §3.2 | STRONGLY_SUPPORTED |
| R22 | `testtable.testcode` → `testname.testcode` | R04 explicit join covers `testname` → `patienttest`; `testname` is a reporting-name map (8 cols) | STRONGLY_SUPPORTED |

## 3. Cross-store relationships (described, not directly verifiable)

| Relationship | Evidence | Confidence |
|---|---|---|
| SQL Server catalogue `patients` ↔ Jet mirror `Patients.mdb` | two backends documented in `database-engineering-reference.md` §1 and `database-and-data-access-analysis.md` §1–2; no synchronisation mechanism evidenced | INFERRED |
| `Branch_Data.mdb` template → per-branch database | 49-table, 0-row schema-only template (`db-summary.csv`; `Version_Identity_and_Scope.md` §3.3) | STRONGLY_SUPPORTED |
| `data.mdb` shared masters (`history`, `patientinfo`, `wasl`) ↔ primary store | `db-summary.csv` row 4; `table-overlap.csv` shows `patientinfo` present in all four Jet stores | STRONGLY_SUPPORTED |
| `table-overlap.csv`: `ABO`, `Abuse`, `BILH`, `BONEMAR`, `BloodGases`, `CRP`, `CSFT`, `CoagFactors`, `Drugs`, `Electrolytes`, `ESR`, `Fertlity`, `GLUCOSETOLERANCE`, `Hgbectrophoresis`, `IMMUNOLOGY`, `Immunoglobulin`, `Lipids`, `liver`, `Pancreas`, `Proelectrophoresis`, `Suprarenal`, `TORCH`, `TUBERCULINT`, `Tumors`, `Vitamins`, `ZN`, `cardiac`, `cbc`, `crossmat` — each in `Patients.mdb`, `Patients_backup.mdb`, `Branch_Data.mdb` | `table-overlap.csv` rows; `db-summary.md` §"common tables" | VERIFIED (overlap), INFERRED (purpose) |

## 4. What is explicitly NOT evidenced

- No declared `FOREIGN KEY`, `REFERENCES`, `ON DELETE CASCADE` or `ON UPDATE CASCADE`
  clause anywhere in `v2019-Patients.mdb.sql`, `ddl-all.txt` or
  `ddl-evidence__[analysis_BATCH-04_sql_v1605].txt`.
- No supported relation between `patients` (operator table) and any patient table via a
  declared key.
- No relationship involving `VIDAS`, `Interface` or `repairedDB.mdb` — those stores are
  undescribed (see `Database_Inventory.md`).
- The relationship set in the stub prior note "`patienttest.patientid+requestdate →
  profile tables`" is downgraded here to INFERRED (R20) because no join uses both columns.
