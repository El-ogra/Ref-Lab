# Method-to-Table Mapping — REAL LAB SYSTEM v1.605

**Basis.** `tables__[analysis_BATCH-04_sql_v1605].csv` (per-table statement counts),
`columns__[analysis_BATCH-04_sql_v1605].csv`, `statements__[analysis_BATCH-04_sql_v1605].csv`,
`objects-full__[analysis_BATCH-04_vb6_v1605].txt` (210 objects, 8,026 declared
methods), `vb6-objects.json`, and prior reports. No method body was decompiled:
every object → table association below is **INFERRED** from object naming and SQL
table-name correlation. Confidence labels apply to the *evidence*, not to a proven
call graph.

---

## 1. Top tables by statement count (VERIFIED from both CSVs)

Counts from `tables__[analysis_BATCH-04_sql_v1605].csv` columns
`statements, select, delete, shape`; identical to the table-name tally computed over
all 1,797 rows of the statement CSV.

| Rank | Table | Statements | SELECT | DELETE | SHAPE | Distinct columns | Dominant role (INFERRED) |
| ---: | --- | ---: | ---: | ---: | ---: | ---: | --- |
| 1 | `patientinfo` | 428 | 410 | 0 | 17 | 25 | patient master, search, discounts, print status |
| 2 | `patienttest` | 343 | 312 | 0 | 3 | 33 | ordered tests, result/status flags, user stamps |
| 3 | `normalrange` | 95 | 91 | 0 | 4 | 4 | reference intervals by test / sex / age band |
| 4 | `testtable` | 74 | 62 | 0 | 8 | 27 | test catalogue and barcode metadata |
| 5 | `payment` | 32 | 29 | 0 | 0 | 6 | collections, receipts |
| 6 | `comments` | 28 | 28 | 0 | 0 | 0 | free-text result comments |
| 7 | `gehats` | 27 | 27 | 0 | 0 | 2 | referring-party commissions (and anti-HCV result table) |
| 8 | `insulin` | 23 | 23 | 0 | 0 | 0 | discipline-specific results |
| 9 | `sharekathesab` | 19 | 19 | 0 | 0 | 0 | shared-account billing |
| 10 | `patients` | 14 | 13 | 0 | 0 | 0 | **operator accounts** (not patients) |
| 11 | `subgrouptable` | 14 | 12 | 0 | 2 | 2 | catalogue grouping |
| 12 | `antibiotic` | 13 | 13 | 0 | 0 | 0 | susceptibility catalogue |
| 13 | `history` | 13 | 12 | 0 | 0 | 0 | interpretive history / audit text |
| 14 | `patienttotal` | 12 | 10 | 0 | 0 | 1 | billing totals |
| 15 | `phone` | 11 | 11 | 0 | 0 | 0 | contact directory |

**Name trap (VERIFIED):** in this system `patients` is the *operator/store* table
(columns `patientusname`, `patientusid`, `startwork`, `grant13..grant17`), while the
*human patient* table is `patientinfo`. Any reconstruction that equates them is
wrong.

**Dialect duplication:** many profile tables have paired SELECT/DELETE literals for
SQL Server and Jet (`'` vs `#` date delimiters). The per-table counts therefore
include both dialect variants. VERIFIED at statement level.

---

## 2. Object → table matrix (INFERRED)

Method counts are from `objects-full__[analysis_BATCH-04_vb6_v1605].txt`. `labdb`
and `labdb1` are modules; everything else is a form or DataReport unless marked.

| VB6 object | Methods | Tables touched (INFERRED) | Basis in SQL evidence |
| --- | ---: | --- | --- |
| `labdb` (module) | 24 | cross-cutting: `patientinfo`, `patienttest`, `normalrange`, `testtable` | shared data helpers; `normalrange-coderefs.json` shows one literal reused at many code sites |
| `labdb1` (module) | 20 | cross-cutting: `patientinfo`, `patienttest`, dialect variants | secondary helper module |
| `labfunction` (module) | 25 | `constanttable`, lookup tables | general helper; prior report lists `constanttable` targets |
| `saveIOtemp` (module) | 10 | `testtemp` | temp-table lifecycle |
| `addpatient` | 192 | `patientinfo`, `payment`, `patienttest`, `phone` | `SELECT * from patientinfo where patientnamear = '` (`6AAB0A`), `where labid ='` (`6AAD36`) |
| `patientmain` / `patientview` / `patient` | 84 / 33 / 58 | `patientinfo`, `patienttest`, `normalrange`, `wasl` | work-list joins `6843E2`, status pulls `681ED2`, `686EC2` |
| `patienthistory` / `patienthistory2` | 22 / 17 | `history`, `patientinfo`, per-discipline result tables | `6A2872`, `6DF2A8` |
| `testmain` | 30 | `testtable`, `subgrouptable`, `pricelist`, `customtesttable` | catalogue/pricing SHAPEs `672722`, `672E3E` |
| `testrange` | 17 | `normalrange`, `testtable` | `68B2D2`, SHAPE `672E3E` |
| `z_pricelists` / `pricelists` (DataReport) | 27 / 0 | `pricelist`, `testtable`, `customtesttable` | `SELECT * FROM pricelist where testsimp='` |
| `frm_money` / `moneypaid` / `moenysharekat` / `moenymozafeen` | 38 / 18 / 27 / 27 | `payment`, `sharekathesab`, `patienttotal`, `gehats`, `tasgel` | `6879A2`, `691DAA`, `694C92` |
| `z_gehats` | 56 | `gehats` | `SELECT gehahname,morsel,phone,mobil FROM Gehats where gehahname= '` |
| `login` / `users` / `userspw` / `logindete` | 13 / 57 / 12 / 6 | `patients`, `useraccounts`, `loger` | `6EF640`, `6EF8D0`, `6DC31C` (grant columns) |
| `z_logs` | 14 | `loger` | `66F746`, `72C568` |
| `Reportlog` (DataReport) | 0 | `loger` | log reporting |
| `barcode` / `z_barcodes` / `barcodeform` | 76 / 14 / 0 | `barcodes`, `testtable`, `patientinfo` | `6B28A4`, `6F0998`, `6B18C8` |
| `reportsetup` / `reportsetup2` | 113 / 69 | `reportview`, `titless`, `history` | `66F882`, `6DA5AC`, `6DEA28` |
| `printerselect` (module) | 11 | none (printer selection only) | no SQL; config-driven (`SETTING.ini` `[selectprinter]`) |
| `z_branchs` | 14 | `Labbranch`, `Patients.branche`, `grouptable`, `loggroup` | DDL `6DC264`; `SUBSTRING(patientid,9,1)` |
| `z_labaccount` / `server` | 18 / 17 | `MyLabSetting`, `Interface` database | `Select * from MyLabSetting WHERE LabID=100 ;` (prior report §5) |
| `SMSfrmsenderos` | 139 | `SMSSender`, `smslist`, `smsgroups`, `SMStemplate`, `MyLabSetting` | 10+10 statements; DDL CREATE set |
| `anyreport` / `z_customgroup` | 16 / 31 | `anyreport`, `customtesttable`, `customgrouptable` | `Delete FROM anyreport where patientid='`, `Delete from customtesttable where customgroupcode=` |
| `commentsetup` / `Frmcomment` | 5 / 17 | `comments` | `SELECT comments FROM comments where testcode in (` |
| `instrument` / `Resultinstrument*` | 29 | `seana`, `instrument`, result tables | `Delete FROM seana where instrumentid=` |
| `Frm_pic` / `Frm_pic2` | 55 / 31 | `pictures` | 84 ALTER widenings; `Delete FROM pictures where patientid= '` |
| `Patient_samples` | 89 | `samples`, `patienttest` | `Delete from samples where patientid= '` |
| `outtest` | 11 | `patienttest`, `otherlabname`, send-out tables | `patienttest.otherlabname`, `patienttest.sent`, `patienttest.send` columns |
| `Report*` family (60+ DataReports) | 0–1 | `patientinfo`, `patienttest`, `history`, per-discipline tables and `wasl` | SHAPE parent/child aliases `cmdbill`, `cmdbill_d` |
| `Result*` family (60+ forms) | 11–231 | one profile table each + `patientinfo`, `patienttest`, `normalrange`, `comments` | e.g. `Resultliver` ↔ `liver` (`6A3336`); `Resultsugar` ↔ `sugar` (`6B348C`); `resultculutre` ↔ `cultures`, `cultureanti`, `culturesec` |

---

## 3. Profile-table families (INFERRED one-to-one form mapping)

The `Result*` form names and the SQL table names correspond almost one-to-one; this
is the strongest structural signal in the object inventory. Representative pairs
(VERIFIED names on both sides, INFERRED linkage):

| Form | Methods | Table |
| --- | ---: | --- |
| `Resultliver` | 101 | `liver` |
| `Resulturine` | 167 | `urine` |
| `Resultsugar` | 61 | `sugar` |
| `Resultkidney` | 117 | `kidney` |
| `Resultlipids` | 81 | `lipids` |
| `resultcbc` | 172 | `cbc` |
| `resultpt` | 110 | `pt` |
| `resulthormon` | 116 | `hormones` |
| `resultcrp` | 97 | `crp` |
| `ResultTORCH` | 123 | `TORCH` |
| `resultbloodgas` | 81 | `BloodGases` |
| `resultcrossm` | 140 | `crossmat` |
| `resultabuse` / `resultdrugs` | 95 / 98 | `Abuse` / `Drugs` |
| `resulthgbelec` / `resultprotelec` | 51 / 61 | `Hgbectrophoresis` / `Proelectrophoresis` |
| `ResultFMF` / `resultLBC` / `resultIgeAllergy` | 48 / 33 / 231 | `FMF` / `LBC` / `IgEallergy` |
| `ResultPCR` / `resultwidal2` / `resultha1c` | 37 / 51 / 39 | `PCR`-related / `widal2` / `hba1c` |

---

## 4. Column-level evidence for the top tables (VERIFIED)

From `columns__[analysis_BATCH-04_sql_v1605].csv` (qualified reference counts):

| Qualified column | References |
| --- | ---: |
| `patientinfo.patientid` | 341 |
| `patientinfo.patientnamear` | 272 |
| `patientinfo.labid` | 213 |
| `patienttest.patientid` | 116 |
| `patientinfo.requestdate` | 103 |
| `patienttest.testsimp` | 50 |
| `patientinfo.referredbyname` | 44 |
| `patientinfo.totalpricedisc` | 37 |
| `patienttest.requestdate` | 36 |
| `patienttest.testcode` | 36 |
| `patientinfo.completes` | 35 |
| `patientinfo.elmadfoo3` | 29 |
| `patientinfo.patientsex` / `patientageunit` / `patientage` | 28 each |
| `normalrange.testcode` | 26 |
| `subgrouptable.subgroupname` | 16 |

These are the join keys that connect the object groupings in §2: `patientid` links
master ↔ orders ↔ results; `testcode`/`testsimp` link orders ↔ catalogue ↔
reference ranges; `elmadfoo3`/`totalpricedisc` link master ↔ billing.

---

## 5. What is NOT evidenced

- **No call graph.** No decompiled body proves that object X executes statement Y.
- **No per-object statement counts.** The CSV has no object column.
- **Module-level claims are structural inferences**, not observed dispatch.
- The two database dialects mean a single logical object likely owns two literal
  variants; which variant runs is configuration-dependent (INFERRED).
- `vnUserControlsXP` appears as an OCA/OCX control set in dependency evidence but
  **no VB6 object by that name** appears in `objects-full__[…_v1605].txt` — it is a
  UI dependency, not a data-access owner.
