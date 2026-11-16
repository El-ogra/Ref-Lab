# Executable Business Logic — rules traceable to code evidence (1.605)

**Method.** Only rules with a named evidence file and an exact locator are recorded. SQL VAs are the `va` column of `statements__[analysis_BATCH-04_sql_v1605].csv` (the 1.605 SQL inventory, 1,797 rows); object names and method counts come from `objects-full__[analysis_BATCH-04_vb6_v1605].txt` and `vb6-objects.json`; connection/string context from `primary-executable-reverse-engineering.md` and `primary-executable-technical-reference.md`. **No rule is presented as observed runtime behaviour** — the investigation was static-only (`Version_Identity_and_Scope.md §5.1`). No patient data values are reproduced.

**Confidence rule.** VERIFIED = a literal SQL row / object name exists as cited. STRONGLY_SUPPORTED = the rule follows from multiple consistent literals. INFERRED = only names or string shape support the interpretation. UNVERIFIED = the binding to an object or execution path is missing (UNR-008).

## 1. Test catalogue and reference ranges (VERIFIED literals; rule INFERRED)

| Evidence (file → va) | Literal / value | Rule supported |
|---|---|---|
| `statements__[analysis_BATCH-04_sql_v1605].csv` → `672E3E` | `SHAPE {SELECT testcode,testsimp,patientprice,testtime,typeofsample FROM testtable where testorno='True' and testsimp <> '' GROUP BY …} AS cmdbill APPEND ({SELECT testcode,sex,firstold,secondold,ageunit,normalrang FROM normalrange where normalran…` | The active test catalogue is `testtable`, filtered by `testorno` and non-empty `testsimp`; each test is joined to **age- and sex-dependent reference ranges** in `normalrange` (`sex`, `firstold`, `secondold`, `ageunit`, `normalrang`). STRONGLY_SUPPORTED for data shape; INFERRED that the age bracket is evaluated at result entry. |
| same → `673172` | same SHAPE with `testorno=True` (unquoted boolean variant) | Mixed Jet/SQL boolean spellings coexist — consistent with the dual-engine design (`Dependency_Graph.md §3`). VERIFIED as a literal pair. |
| same → `6A9632` | `Select testcode, testabrev, testname, subgroupname, testtime, inotherlab, testnamear, typeofsample, otherlabname, otherlabprice, patientprice, labprice from testtable where testcode =` | Per-test configuration includes: patient price, lab price, external-lab price, turnaround `testtime`, `typeofsample`, send-out flag `inotherlab`, and Arabic name `testnamear`. STRONGLY_SUPPORTED. |
| same → `6AA272` / `6AA466` | `Select testcode,testsimp,testorno,subgroupcode,patientprice,labprice,otherlabprice,testtime,seereport,arrangnum,printwith,loggroup,inotherlab,otherlabname,anyqus,thequse,testabrev,testname,historyname,REFRANGE from testtable where testsimp like '…` | Catalogue supports search (`like`), print grouping (`printwith`, `arrangnum`, `loggroup`), a "see report" flag, patient questions (`anyqus`, `thequse`) and a reference-range field. STRONGLY_SUPPORTED. |
| `tables__[analysis_BATCH-04_sql_v1605].csv` row `testtable` | 74 statements, 62 SELECT, 8 SHAPE, 27 distinct columns | Test catalogue is the second-most-referenced table after `patientinfo`/`patienttest`. VERIFIED count. |

## 2. Pricing and billing (VERIFIED literals; aggregation rule STRONGLY_SUPPORTED)

| Evidence → va | Literal | Rule |
|---|---|---|
| `statements__[analysis_BATCH-04_sql_v1605].csv` → `69EA76` | `Select pricemethod, Round(sum(totalprice),2) as tot1, Round(sum(totalpricedisc),2) as tot2, Round((sum(totalprice) - sum(totalpricedisc)),2) as tot3, Round(sum(elmadfoo3),2) as tot4, Round(sum(elbakyma3mal),2) as tot5 from patientinfo where requestdate >= '…` | Daily/period billing aggregates **gross**, **discount**, **net (gross − discount)**, plus two extra adjustment buckets (`elmadfoo3`, `elbakyma3mal`), grouped by `pricemethod`, rounded to 2 decimals. STRONGLY_SUPPORTED. |
| same → `69EC7E` | same sums without `Round`, `requestdate = #…` (Jet date-literal syntax) | Two dialect variants of the same aggregation exist; STRONGLY_SUPPORTED evidence of Jet/SQL dual use. |
| same → `6A5796` / `6A597E` | `SELECT patientid, totalprice, totalpricedisc, elbakyma3mal, discount100, discount, elmadfoo3, completes FROM patientinfo where patientid='…` | Invoice header carries both absolute `discount` and `discount100` (likely percentage — INFERRED from name), adjustment fields and a `completes` completion flag. |
| same → `66E8FE` | `SELECT pricemethod, patientnamear, patientphone, patienttel FROM patientinfo GROUP BY pricemethod, …` | Pricing method is a per-patient attribute, used as a grouping key. VERIFIED literal; business meaning of `pricemethod` values UNKNOWN. |
| same → `6A8C52` | `SELECT * FROM pricelist where testsimp='…` | A separate `pricelist` table provides per-test pricing lookup (`tables…csv` row `pricelist`: 11 statements, 5 distinct columns). VERIFIED. |
| same → `6A9BC2` | `SELECT customtesttable.testcode, testtable.testabrev, … testtable.patientprice as patpricindved, customtesttable.patient…` | Per-patient **individual price override** via `customtesttable` alongside standard `testtable.patientprice`. STRONGLY_SUPPORTED (alias `patpricindved` = "patient price individual" — name-based reading INFERRED). |
| `objects-full__[…v1605].txt` | `frm_money` 38 methods, `moneypaid` 18, `moenysharekat` 27 (new), `moenymozafeen` 27 | Code objects corresponding to cash receipt and company/employee money handling exist. INFERRED mapping to the SQL above (no call chain recovered). |

## 3. Result entry, review and print status (VERIFIED literals; workflow INFERRED)

| Evidence → va | Literal | Rule |
|---|---|---|
| `statements__[analysis_BATCH-04_sql_v1605].csv` → `681ED2` | `Select patientid,printwith,resultfinsh,resultreview,resultprint from patienttest where patientid= '…` | Each ordered test line carries three independent status flags: **finished** (`resultfinsh`), **reviewed** (`resultreview`), **printed** (`resultprint`), plus a report-grouping key `printwith`. STRONGLY_SUPPORTED. |
| same → `6860BE`, `6861D2` | `Select testcode,patientid,resultfinsh,edituser,editdate from patienttest where patientid= '…` | Finishing a result records `edituser` + `editdate` (audit trail). STRONGLY_SUPPORTED for stored fields; who sets them INFERRED. |
| same → `686312`, `68647E` | `Select testcode,patientid,resultreview,prevuser,prevdate from patienttest where patientid= '…` | Review stage records `prevuser` + `prevdate` — a second approver/date pair distinct from the entry pair. STRONGLY_SUPPORTED. |
| same → `6843F2` | `SELECT SUBSTRING(patientinfo.patientid,10,3) as patid, patientinfo.patientid, patientinfo.patientnamear, patientinfo.completes FROM patientinfo INNER JOIN patienttest … Where patienttest.resultfinsh = 'False' and (patienttest.requestdate = '…` | Worklist query selects **unfinished** tests for a request date; derives a 3-character `patid` from character positions 10–12 of `patientid`. STRONGLY_SUPPORTED. |
| same → `68463A` | same query with `mid(patientinfo.patientid,10,3)` and `resultfinsh =False` and `requestdate = #…` | Jet `mid()`/`#date#` variant of the same rule; VERIFIED pair. The 13-character order-id shape is referenced in `AGENTS.md` (project instruction) but its parsing rule here is evidenced only by this SQL. |
| `tables__[analysis_BATCH-04_sql_v1605].csv` rows `pt`, `torch`, `abuse`, `bloodgases`, `cardiac`, `coagfactors`, `crp` | `Delete FROM pt where patientid='…`, `Delete FROM TORCH where patientid='…`, etc. | Per-discipline result tables are cleared by `patientid` before being repopulated — a **replace-on-save** pattern. INFERRED (deletes are VERIFIED literals; the save sequence is not observed). Counts: e.g. `cbc` 18 distinct columns, `torch` 12, `abuse` 11. |
| `objects-full__[…v1605].txt` | `resultcbc` 172, `resulthepatitis` 187, `Resultsemen` 222, `resultIgeAllergy` 231, `ResulturineAuto` 184, `ResultPCR` 37, etc. | A dedicated capture form exists per discipline, matching the per-discipline result tables. STRONGLY_SUPPORTED as a structural pairing; exact form↔table binding INFERRED. |

**Derived workflow (INFERRED from the flags above):**
`order (resultfinsh=False)` → `result entry sets resultfinsh + edituser/editdate` → `review sets resultreview + prevuser/prevdate` → `resultprint` marks output → report rendered by a `Report*` DataReport and `printerselect`. This is the most defensible reconstruction, but the **actual sequence and any guards are UNVERIFIED** (UNR-008; no method names, no call graph).

## 4. Patient, order and completion logic (VERIFIED literals)

| Evidence → va | Literal | Rule |
|---|---|---|
| `statements__[analysis_BATCH-04_sql_v1605].csv` → `6843F2` / `68463A` | `patientinfo INNER JOIN patienttest ON patienttest.patientid = patientinfo.patientid` | Orders are stored in `patienttest` keyed by `patientid` to a `patientinfo` header row. VERIFIED join. |
| `tables__[…v1605].csv` row `patientinfo` | 428 statements (410 SELECT, 17 SHAPE), 25 distinct columns; sample `SELECT pricemethod, patientnamear, …` | `patientinfo` is the central header table (demographics, pricing, totals, completion). VERIFIED counts. |
| `tables__[…v1605].csv` row `patienttest` | 343 statements (312 SELECT, 3 SHAPE), 33 distinct columns; sample `Select patientid,printwith,resultfinsh,resultreview,resultprint from patienttest` | `patienttest` is the central order-line table. VERIFIED counts. |
| `statements__[…v1605].csv` → `6A5796` | field `completes` in `patientinfo` | Explicit patient-level completion flag exists; its exact semantics are UNKNOWN (name only). |
| `statements__[…v1605].csv` → `6860BE` | `edituser`, `editdate`, `prevuser`, `prevdate` | User/date audit columns are part of the result workflow. VERIFIED as columns; identity semantics INFERRED. |

## 5. Send-out, SMS and configuration rules (INFERRED where name-only)

| Evidence | Finding | Confidence |
|---|---|---|
| `statements__[…v1605].csv` → `6A9632`, `6AA272` | `inotherlab`, `otherlabname`, `otherlabprice` fields in `testtable`; object `labtolab` 32 methods, `outtest` 11 (`objects-full`) | Tests can be routed to an external lab with its own price — STRONGLY_SUPPORTED data model; routing rule INFERRED. |
| `primary-executable-reverse-engineering.md §6.3` | literals `Select * from MyLabSetting WHERE LabID=100`, `SMSbakey`, `sendername`, `accuntID`, `accuntpw`, `http://smsmisr.com/api/send.aspx?username=…&password=…&language=3&sender=…&mobile=…&message=…`, `InternetExplorer.Application`, `navigate2`, `Busy`, `ReadyState`, `Body`; object `SMSfrmsenderos` 139 methods | SMS sending path reconstructed from literals + object name. The transport (plain HTTP, credentials in URL) is a **security observation**, not a business rule. Rule INFERRED (no executed call chain); literal presence VERIFIED. |
| `tables__[…v1605].csv` rows `smslist` (10 statements), `smssender` (10) | SMS queue/sender tables | STRONGLY_SUPPORTED existence; scheduling semantics UNKNOWN. |
| `statements__[…v1605].csv` → `normalrange` rows (95 statements in `tables…csv`), fields `sex`, `firstold`, `secondold`, `ageunit`, `normalrang` | Age/sex-specific normal range storage | STRONGLY_SUPPORTED storage; threshold comparison at result entry INFERRED. |
| `primary-executable-reverse-engineering.md §6.4` | print-password identifiers `IDPasswordL/T/F/B/S/V/A`, `Print Result Password`; prompts `Please Enter Your Internal Password. (Digits only)` | Section-level print authorisation likely exists. Mapping of each identifier to a report section is **UNVERIFIED** (UNR-003). |
| `Version_Identity_and_Scope.md §5.3` | `normalrange.pregsmok` populated on all 852 rows but use in calculation not traced | Reported as OBSERVED data, not a working rule — UNVERIFIED. |

## 6. Schema migration embedded in 1.605 (VERIFIED counts; intent INFERRED)

`statements__[analysis_BATCH-04_sql_v1605].csv` verb distribution over 1,797 rows: **SELECT 1,381 / ALTER 241 / DELETE FROM 72 / OTHER 49 / CREATE 27 / SHAPE 27**. The 27 `CREATE` + 241 `ALTER` = **268 embedded DDL statements** — a migration embedded in the executable, absent from 1.00 (`Version_Identity_and_Scope.md §2.2 / §3.2`: 0 → 268; `statements.csv` has 1,337 rows and no DDL). `Version_Identity_and_Scope.md §3.5` notes the shipped `Patients.mdb` received only part of these changes (23 SQL-Server-only statements cannot apply to Jet; 38 Access-applicable ones were never executed). The migration's intended target schema is INFERRED; which VB6 object runs the DDL is UNVERIFIED.

## 7. Confidence legend and explicit gaps

| Label | Meaning here |
|---|---|
| VERIFIED | SQL row with `va`, object name, or count read directly from the named dump |
| STRONGLY_SUPPORTED | Rule follows from ≥2 consistent literals / counts |
| INFERRED | Name- or string-based interpretation only |
| UNVERIFIED | Object binding or execution path missing (UNR-008/010) |
| UNKNOWN | Field exists but meaning not established (`pricemethod`, `completes`, `grant1`–`grant17`) |

The following are **not established**: which object issues any given SQL statement (UNR-008); per-object method names (0 recovered); report call chain (UNR-010); any runtime behaviour; the values of `pricemethod` or `completes`; the section mapping of `IDPassword*`; whether `accuntpw` is hashed before storage (UNR-005). No patient identifiers or names from any database sample are reproduced in this document.
