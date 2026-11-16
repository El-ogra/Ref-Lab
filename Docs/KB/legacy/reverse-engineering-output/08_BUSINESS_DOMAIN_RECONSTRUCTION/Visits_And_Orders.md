# Visits and Orders — REAL LAB SYSTEM v1.605 (V-CD-2019)

**Scope.** How a request becomes one or more ordered tests, how the visit is grouped, and
how the order is routed. All statements cited are from
`statements__[analysis_BATCH-04_sql_v1605].csv` unless noted. Static evidence only.

## 1. Core entities

| Entity | Primary source | Role | Confidence |
|---|---|---|---|
| `patientinfo` | `v2019-Patients.mdb.sql` | Visit/order header: `patientid`, `requestdate`, `labid`, `referredbyname`, `pricemethod`, totals | VERIFIED |
| `patienttest` | `v2019-Patients.mdb.sql` | **One row per ordered test**: `patientid`, `testcode`, `testsimp`, `subgroupcode`, `arrangnum`, `requestdate`, price fields, result flags | VERIFIED |
| `testtable` | `v2019-Patients.mdb.sql`; `testtable-stats.json` | The 609-row test catalogue; `testorno`, `active`, `inotherlab` drive what can be ordered | VERIFIED / STRONGLY_SUPPORTED |
| `subgrouptable`, `testname` | `v2019-Patients.mdb.sql`; `grouptable.csv`, `subgrouptable.csv` | Hierarchical grouping and the `num`/`groupcode` report map | VERIFIED |

`patienttest` has **46 declared columns** in the shipped v1.605 dump (the older stub's
"343 statements" figure is the per-table statement count from
`tables__[analysis_BATCH-04_sql_v1605].csv`: **343 statements, 312 SELECT, 3 SHAPE**).
The same file gives `patientinfo` 428, `normalrange` 95, `testtable` 74, `payment` 32.
These are the handoff counts, now source-attributed.

## 2. Order selection and catalogue gating

The catalogue is filtered and shaped for the order-entry grid:

- VA `672722` / `6729D6`: `SHAPE {SELECT subgrouptable.subgroupcode, subgrouptable.subgroupname
  FROM subgrouptable …} AS cmdbill APPEND ({SELECT * FROM testtable where testorno= 'True'
  and testsimp <> '' order by testsimp} …) … RELATE subgroupcode TO subgroupcode`.
  → **subgroup → test** hierarchy; only `testorno='True'` tests with a non-empty `testsimp`
  are orderable. Confidence: VERIFIED.
- VA `6AA272` / `6AA466`: `SELECT testcode,testsimp,arrangnum,subgroupcode,patientprice,
  labprice,otherlabprice,testtime,seereport,arrangnum,printwith,loggroup,inotherlab,
  otherlabname,anyqus,thequse,testabrev,testname,historyname,REFRANGE FROM testtable
  where testsimp like …` → the order-picker carries prices, send-out flags and the
  report selector. Confidence: VERIFIED.
- `testtable-stats.json`: 609 tests; **94 active**; **605 delete-protected**; 591 carry a
  `patientprice` and an `otherlab_price` value; 48 distinct subgroups; 1 send-out test.
  Confidence: STRONGLY_SUPPORTED (derived statistic over the full dump).

`arrangnum` is the display/sequence number; the earlier finding that "Arrange number"
plus `testid` maps numeric report identities to tests is recorded in
`unified-findings-register.md` (extension of B3 findings) and `1605-phase2-findings.md` U-05
(54 of 55 numeric template identities equal a `testtable.testcode`). Confidence:
STRONGLY_SUPPORTED.

## 3. Multi-test order grouping

A visit is grouped by the pair `(patientid, requestdate)`:

| VA | Fragment | Establishes | Confidence |
|---|---|---|---|
| `6B6378`, `6B6750`, `6B6934` | `SHAPE {SELECT patientid,barcode,requestdate,patientnamear,patientsex,patientage,patientageunit,referredbyname FROM patientinfo where requestdate…}` | Worklists keyed on request date; demographics shape the parent recordset | VERIFIED |
| `70ECE4`, `70F278` | `SHAPE {SELECT testname.num, testname.testcode, testname.testsimp FROM testname INNER JOIN patienttest ON testname.testcode = patienttest.testcode …} AS cmdbill APPEND ({SELECT patientinfo.patientid, …})` | Tests are appended under a `testname` grouping inside one shaped recordset | VERIFIED |
| `6B44F0` | `SHAPE {SELECT patientinfo.patientid,patientinfo.barcode,patientinfo.requestdate,… FROM patientinfo INNER JOIN patienttest ON patientinfo.patientid = patienttest.patientid …}` | Barcode/report consolidation joins header and ordered tests | VERIFIED |
| `686C32`, `686D22` | `Select SUBSTRING(patientid,10,3) as patid, patientid, patientnamear, completes from patientinfo where requestdate = …` | Display code + completion flag on the request-date worklist | VERIFIED |

`SHAPE`/`APPEND` is an `MSDataShape.1` client-side construct; `data_connection1.udl`
configures that provider. `Business_Rules_V2.md` V2-BR-017 records the mechanism and notes
that the consolidated path appears bound to the SQL Server catalogue, with Access behaviour
NOT ESTABLISHED. The v1.605 corpus contains **27 SHAPE statements** (verb column), of which
the catalogue/range shapes above are explicit.

## 4. Order acknowledgement and the three queues

`patienttest.give` Boolean is the order-acknowledgement ("hand-over") flag.
`1605-phase2-findings.md` U-08 and `MASTER_BUSINESS_LOGIC.md` §4 identify three distinct
patient queues driven by columns queried directly by the executable:

| Queue | Filter | Exact SQL evidence | Confidence |
|---|---|---|---|
| Not acknowledged | `patienttest.give = 'False'` | VA `6D4848`, `6D4C30`, `6D5014`, `6D55E4` | VERIFIED |
| Result entry pending | `patienttest.resultfinsh = 'False'` | VA `6843F2`, `68463A`, plus `70BED4`–`70D38C` SHAPE family | VERIFIED |
| Already printed | `resultprint` / `printnum` / `printuser` / `printdate` | VA `681ED2`, `6B037C`, `6B2D70` | VERIFIED |

Hand-over audit columns `taslimuser`, `taslimdate` accompany `give` (VA `6867AA`,
`686862`). **Do not read these flags as a strict sequential state machine**: the evidence
supports independent Boolean flags read together, not an ordered enum. See
`07_RUNTIME_AND_BEHAVIORAL_ANALYSIS/State_Transitions.md` §2 and `Results_Management.md`.

## 5. Send-out routing

Per-test send-out is flag-driven and fully evidenced in the schema and SQL:

- `testtable.inotherlab` Boolean, `.otherlabname` Text(50), `.otherlabprice` Double
  (`v2019-Patients.mdb.sql`); `testtable-stats.json` reports **1** send-out enrolment.
- `patienttest.sentout` Boolean, `.send` Boolean, `.otherlabname` Text(40),
  `.testpriceout` Double, `.testpriceoutmadfo` Double; query at VA `6F3700` selects them
  via `patientinfo INNER JOIN patienttest`.
- `Business_Rules_V2.md` V2-BR-044 records "send-out enrolment is flag-driven with two
  settlement states" — the two states map cleanly to `testpriceout` (quoted) versus
  `testpriceoutmadfo` (paid), INFERRED from column names.
- Delete protection includes `Delete FROM patienttest where patientid='` in the 64-table
  manual cascade (`State_Transitions.md` §5).

## 6. Lab/site dimension

`patientinfo.labid` is Text(13) while the operator's branch attribute `patients.branche`
is Integer — a declared type mismatch noted in `Database_Business_Logic.md` §6. The v1.605
SQL itself extracts position 9 as a branch code:
`SUBSTRING(patientid,2,1) + SUBSTRING(patientid,9,1) as bran` in 18 statements (VAs
`72714C`, `727AA0`, `7283A4`, `729578`, `72A774`, `72BFC0`, `72C3C4`, …), paired with
period codes `SUBSTRING(patientid,5,2)` or `SUBSTRING(patientid,7,2) as mmmd` and with
`ROUND(SUM(testprice),2)`-style statistics — VERIFIED for the *legacy* system. The current
project rule (`Docs/KB/START-HERE.md` rule 8) states the laboratory is a **single site** for
the successor design, with position 9 absorbed into the middle segment; that is a scope
decision for the replacement, not a denial of the v1.605 extraction. No multi-branch
*access-control* rule is asserted here.

## 7. Unknowns

Order modification/cancellation rules; the meaning of `testorno` values beyond the
`'True'` filter; whether `requestdate` alone defines a visit or a further session key
exists; whether `give` can be reset; any runtime observation of ordering. All UNKNOWN /
not established.

**Cross-references:** `Patient_Management.md`, `Simple_And_Composite_Tests.md`,
`Laboratory_Business_Rules.md`, `09_CROSS_SOURCE_EVIDENCE/Cross_Source_Correlation.md`.
