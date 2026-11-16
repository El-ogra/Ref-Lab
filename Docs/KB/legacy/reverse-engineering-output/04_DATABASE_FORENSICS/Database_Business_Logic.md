# Database Business Logic — REAL LAB SYSTEM v1.605

## 0. Scope and evidence rule

"Database-layer logic" here means semantics carried by **column definitions, declared
constraints, reference data and the embedded SQL/DDL corpus** — not VB6 procedure bodies.
Each rule below is tied to a DDL column definition or a verbatim SQL statement and given
a confidence label: VERIFIED / STRONGLY_SUPPORTED / INFERRED / UNVERIFIED / CONFLICTED.
Primary sources: `v2019-Patients.mdb.sql`, `ddl-evidence__[analysis_BATCH-04_sql_v1605].txt`,
`statements__[analysis_BATCH-04_sql_v1605].csv`, `tables.csv`, `testtable-stats.json`,
`constanttable.csv`, `normalrange.csv`, plus the cited reports. No database was contacted.

## 1. Catalogue governance and delete protection

| Rule | Evidence | Confidence |
|---|---|---|
| Catalogue rows carry a deletion guard; 605 of 609 tests are `delete_protected` | `testtable.deleteprotect` Boolean in `v2019-Patients.mdb.sql`; `testtable-stats.json` → `delete_protected: 605`, `total_tests: 609` (99.3% of 609) | STRONGLY_SUPPORTED (flag + statistic); the actual delete veto is application-side — INFERRED |
| A test may be flagged active; 94 of 609 are `active` | `testtable.active` Boolean; `testtable-stats.json` `active: 94` (15.4% of 609) | STRONGLY_SUPPORTED |
| Groups and sub-groups carry the same guard | `grouptable.deleteprotect`, `subgrouptable.deleteprotect` in `v2019-Patients.mdb.sql`; redacted values in `grouptable.csv` / `subgrouptable.csv` | STRONGLY_SUPPORTED |
| Result comments are keyed by test | `SELECT comments FROM comments where testcode in (` — `tables.csv` row `comments` (25 statements) | STRONGLY_SUPPORTED |

## 2. Pricing, discount and referral accounting

| Rule | Evidence | Confidence |
|---|---|---|
| Each catalogue row may carry in-house and send-out prices | `testtable.patientprice`, `testtable.labprice`, `testtable.otherlabprice`, `testtable.otherlabname`; `testtable-stats.json` → 591 of 609 have `patientprice` (97.0%), 591 of 609 have `otherlab_price` | VERIFIED (columns + counts) |
| Send-out disposition is recorded per ordered test | `patienttest.sentout`, `patienttest.send`, `patienttest.otherlabname`, `patienttest.testpriceout`, `patienttest.testpriceoutmadfo` in `v2019-Patients.mdb.sql`; `SELECT … patienttest.sentout, patienttest.otherlabname, patienttest.testpriceout … FROM patientinfo INNER JOIN patienttest …` at VA `6F3700` | VERIFIED |
| Price lists are selected by test abbreviation | `SELECT * FROM pricelist where testsimp='` — `tables.csv` row `pricelist` (11 statements); `pricelist.csv` header `pricenum, testsimp, pricelistname, testprice` | VERIFIED |
| Order-level totals and discounts are persisted | `patientinfo.totalprice`, `patientinfo.totalpricedisc`, `patientinfo.discount`, `patientinfo.discount100`, `patientinfo.pricemethod` in `v2019-Patients.mdb.sql`; `columns.csv`: `patientinfo.totalpricedisc` 13 references | VERIFIED (columns); **the computation behind `pricemethod`/`discount` is INFERRED — UNR-002** (`database-engineering-reference.md` §5) |
| Referring-party commission accounting | `Gehats` (19/21 cols, 10 rows), `sharekat`, `sharekathesab`; `SELECT sum(wareed) as sumw , sum(saader) as sums FROM sharekathesab where sherkaid=` — `tables.csv` row `sharekathesab` (15 statements); `columns.csv`: `patientinfo.referredbyname` 32 | VERIFIED |
| Per-patient aggregate sheet | `patienttotal` (15 cols); `SELECT * FROM patienttotal where patientid= '` — `tables.csv` row `patienttotal` (12 statements) | VERIFIED |

## 3. Reference-range selection and abnormal-result evaluation

| Rule | Evidence | Confidence |
|---|---|---|
| Reference intervals are keyed by test, sex, age band and (optionally) pregnancy/smoking | `normalrange.testcode`, `.sex`, `.firstold`, `.secondold`, `.ageunit`, `.pregsmok`, `.normalrang` in `v2019-Patients.mdb.sql`; 852 rows / 304 testcodes in `normalrange.csv`; sex Male 422/852, Female 421/852; ageunit Years 708/852, Days 70/852, Mons. 65/852 | VERIFIED (structure + distributions) |
| Low/high limits, exception flags and comments drive the abnormal flag | `normalrange.lowlimit`, `.highlimit`, `.nolowexe`, `.nohighexe`, `.lowcomment`, `.highcomment`, `.REFRANGE`; same columns denormalised into `patienttest` (§3.2 of `Data_Model_and_Workflow_Entities_V2.md`) | VERIFIED (columns); exact evaluation formula INFERRED from names |
| The range table is joined to the catalogue on `testcode` | `normalrange.testcode = testtable.testcode` — 13 explicit joins; `SHAPE … RELATE testcode TO testcode` at VA `672E3E` | VERIFIED |
| The range is frozen onto the result row at order time | `patienttest.normalrang`, `.lowlimit`, `.highlimit`, `.nolowexe`, `.nohighexe` in `v2019-Patients.mdb.sql`; `Data_Model_and_Workflow_Entities_V2.md` §3.2 calls it a "denormalised reference-range freeze" | STRONGLY_SUPPORTED |
| The same limits drive screen entry and print | `database-engineering-reference.md` §4 and `database-and-data-access-analysis.md` §4.2 (corroborated by B4 manual topic 11) | STRONGLY_SUPPORTED (report-level) |
| Unit conversion uses per-analyte factors | `converts.testsimp`, `.sunit`, `.funit`, `.sfix`, `.ffix`, `.factors`; 167 rows in `rowcounts__[analysis_BATCH-02_db_Patients].csv` | STRONGLY_SUPPORTED (structure); formula INFERRED |

## 4. Clinical derivation constants

| Rule | Evidence | Confidence |
|---|---|---|
| Named clinical constants are stored in two rows | `constanttable.csv`: `PTCONS` and `CBCCONS` with six numeric slots; `constanttable.constant1`…`constant6`, `.constantdiscrp` in `v2019-Patients.mdb.sql` | VERIFIED |
| Derivation control records exist per constant set | `PTC.concent`, `PTC.control`, 33 rows in `cumulative-schema.csv` / `rowcounts__[analysis_BATCH-02_db_Patients].csv`; runtime migration adds `PTC.ConstID` (`ddl-evidence…` VA `677FA6`) | VERIFIED (columns + counts); specific formula INFERRED |
| eGFR and albumin/creatinine ratio columns are first-class derived results | `Kidney.egfr` (`added-columns.csv`), `ratios.albu`, `ratios.albcreat` (12-column `ratios` table in `v2019-Patients.mdb.sql`) | STRONGLY_SUPPORTED |

## 5. Result lifecycle and audit

| Rule | Evidence | Confidence |
|---|---|---|
| Result states are tracked as independent Boolean/text flags, not an enum column | `patienttest.resultfinsh`, `.resultreview`, `.resultprint`, `.give`, `.sentout`, `.send`, `.RESULTT`, `.RESULTCASEPRINT`, `resultt1`…`resultt4` in `v2019-Patients.mdb.sql`; queries at VAs `6DEB6C`, `6D4848` (`give = 'False'`), `6F3700` (`sentout`) | VERIFIED |
| Multi-stage user/date audit trail is denormalised onto the result row | `patienttest.edituser`/`.editdate`, `.prevuser`/`.prevdate`, `.printuser`/`.printdate`/`.printnum`, `.taslimuser`/`.taslimdate` (`Data_Model_and_Workflow_Entities_V2.md` §3.2; `v2019-Patients.mdb.sql`) | VERIFIED |
| Audit/notice side tables exist | `loggroup` (2 cols), `noteic` (6 cols), `history` (25 cols, 20 free-form slots) in `v2019-Patients.mdb.sql` | VERIFIED |

## 6. Operator permissions, attendance and branch

| Rule | Evidence | Confidence |
|---|---|---|
| Permissions are per-operator Boolean bits, 17 in v1.605 | `patients.grant1`…`grant17`, `patientad`, `patientus`, `SecWord`, `patientword` in `v2019-Patients.mdb.sql`; DDL adds `grant13 Bit` / `grant14 YESNO`…`grant17` (VAs `6DC31C`–`6DC460`); `Data_Model_and_Workflow_Entities_V2.md` §3.3 | VERIFIED (columns); semantic meaning of individual bits UNVERIFIED (UNR-002) |
| Operator passwords are stored in the table | `patients.patientword` Text (40); login query `select startwork,endwork,breakyorn,breaktime from patients where patientusname= '` (`tables.csv` row `patients`); `database-engineering-reference.md` §6 ("no hash column and no salt column") | VERIFIED (structure); hashing question UNVERIFIED (UNR-005) |
| Attendance fields live on the operator record | `patients.startwork`, `.endwork`, `.breakyorn`, `.breaktime` (`v2019-Patients.mdb.sql`; `tables.csv` query above) | VERIFIED |
| Branch identity is an operator attribute written into the identifier | `patients.branche` Integer; `Labbranch` 2-column master; `SUBSTRING(patientid,9,1)` = branch (`Business_Rules_V2.md` V2-BR-008; `Functional_Specification_V2.md`); `Branch_Data.mdb` 49-table template | STRONGLY_SUPPORTED |
| Type mismatch between operator branch and order "lab" | `patients.branche` Integer vs `patientinfo.labid` Text (13) — `Data_Model_and_Workflow_Entities_V2.md` §3.4 | VERIFIED |

## 7. Hierarchical result assembly (`SHAPE … RELATE`)

| Rule | Evidence | Confidence |
|---|---|---|
| Catalogue is shaped hierarchically: sub-groups contain tests | 3 `SHAPE` statements with `RELATE subgroupcode TO subgroupcode`, e.g. VA `672722` (`subgrouptable` parent `APPEND testtable`) | VERIFIED |
| Tests contain their reference ranges in the shaped recordset | 4 `SHAPE` statements with `RELATE testcode TO testcode`, e.g. VA `672E3E` | VERIFIED |
| Barcode/report SHAPE joins the test-name map to ordered tests | VA `70ECE4`, `70F278`: `testname INNER JOIN patienttest ON testname.testcode = patienttest.testcode` inside `SHAPE` | VERIFIED |
| The shape provider is configured in the shipped UDL | `data_connection1.udl` → `Provider=MSDataShape.1` (`Business_Rules_V2.md`); four connection templates with `MSDataShape`/`MSDataShape.1` outer provider (`database-and-data-access-analysis.md` §2) | VERIFIED |

There are **27** `SHAPE` statements in `statements__[analysis_BATCH-04_sql_v1605].csv`.
They are client-side plans, not stored database queries (see
`Stored_Procedures_And_Triggers.md`).

## 8. Messaging subsystem (new in v1.605)

| Rule | Evidence | Confidence |
|---|---|---|
| Outbound SMS uses a queue, templates, recipient lists and groups | `SMSSender` (8 cols, incl. `SMSautonum`, `patientid`, `smsMobile`, `SMSLength`, `SMStext`), `SMStemplate` (2), `smslist` (6), `smsgroups` (3) in `v2019-Patients.mdb.sql`; all four appear as runtime `CREATE TABLE` strings (VAs `6774AE`, `6773DA`, `6772BA`, `6755EA`) | VERIFIED |
| Gateway settings live in a lab-level table | `MyLabSetting` (20 cols) with `LabID`, `SMSbakey`, `sendername`, `accuntID`, `accuntpw`, `SMSNo`, `totalsms`, `SMSSend`; `Select * from MyLabSetting WHERE LabID=100 ;` (`database-and-data-access-analysis.md` §5) | VERIFIED |
| Gateway credential is interpolated into a plain-HTTP URL | `http://smsmisr.com/api/send.aspx?username=<accuntID>&password=<accuntpw>&language=3&sender=<sendername>&mobile=<mobile>&message=<smstext>` (`database-and-data-access-analysis.md` §5) | VERIFIED from literal |
| Patient mobile is the delivery target | `patientinfo.SMSMob` added in v1.605 (`Data_Model_and_Workflow_Entities_V2.md` §3.1; `added-columns.csv`) | VERIFIED |
| NATIGH delivery column is code-present but schema-absent | DDL `ALTER Table patienttest ADD NatighU Bit` (VA `67764E`); column missing from shipped `patienttest` (`Data_Model_and_Workflow_Entities_V2.md` §3.2) | VERIFIED |

## 9. Runtime schema migration as a business rule

| Rule | Evidence | Confidence |
|---|---|---|
| The client owns and mutates the schema at run time | 268 DDL literals in `ddl-evidence__[analysis_BATCH-04_sql_v1605].txt`: 27 CREATE TABLE + 241 ALTER TABLE (228 ADD, 13 ALTER COLUMN); `database-engineering-reference.md` §3 ("no migration script, no versioning table, no audit trail") | VERIFIED |
| Migration is only partially applied to the shipped DB | 61 of 268 changes absent: 23 SQL-Server-only (cannot apply to Jet 4), 38 Access-applicable but unexecuted (`Version_Identity_and_Scope.md` §3.5) | STRONGLY_SUPPORTED (report-level; the 23/38 split is not independently recomputable from the primary dump alone) |
| The DDL is written in SQL Server syntax and fails silently on Jet | `Business_Rules_V2.md` V2-BR-024 (CONFIRMED); syntax examples in `Constraints_And_Indexes.md` §6 | STRONGLY_SUPPORTED |
| New tables are created at runtime, not by the installer | `created-tables.csv` (`Helicobacter`, `LBC`, `SMSSender`, `FMF`, …); `database-engineering-reference.md` §3.1 | VERIFIED |

## 10. Microbiology and specialty panels

| Rule | Evidence | Confidence |
|---|---|---|
| Culture workflow uses three linked tables | `cultures` (35 cols, runtime `ALTER COLUMN organism varchar(80)` at VAs `67770A`/`67777E`/`677892`-adjacent), `cultureanti` (12 cols), `culturesec` (3 cols) in `v2019-Patients.mdb.sql`; 22 ALTER statements on `cultures` (`database-engineering-reference.md` §3.2) | STRONGLY_SUPPORTED |
| Anti-biogram print flags are data-driven | `antibiotic.anticomprint1`…`anticomprint8` Boolean NOT NULL in `v2019-Patients.mdb.sql`; `antibiotic` 38 rows, `antibio` 95 rows | VERIFIED |
| Specialty result panels are wide text grids | 53 profile tables with `patientid` + `requestdate` + analyte columns (`v2019-Patients.mdb.sql`); e.g. `crossmat` 84 cols, `semen` 60, `urine` 60 | VERIFIED |

## 11. Explicit non-findings

- **No database-level `DEFAULT`, `CHECK`, `PRIMARY KEY`, `FOREIGN KEY`, `UNIQUE`,
  `CREATE INDEX` or trigger** is declared (`Constraints_And_Indexes.md` §1). Any
  "validation rule" is application-side unless a field-level Access property exists that
  mdbtools did not export (**UNKNOWN**).
- **`grantN` semantics, `pricemethod` domain, hash/salt status and `pregsmok` usage are
  all UNVERIFIED** and are not asserted here.
- The permission model is **not** on the test row; it is on the operator table
  (`Data_Model_and_Workflow_Entities_V2.md` §3.3, correcting an earlier V2-characterisation).
- Most business logic lives in VB6, not in the database; this document enumerates only
  the portion that leaves an artefact trace in the database layer.
