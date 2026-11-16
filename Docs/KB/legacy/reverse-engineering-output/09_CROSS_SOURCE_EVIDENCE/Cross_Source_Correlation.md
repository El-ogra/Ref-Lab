# Cross-Source Correlation — Real Lab System v1.605 (V-CD-2019)

**Method.** Each chain links, in order: **method/object** (category A) → **SQL statement and VA**
(category B) → **table/column** (category B dump) → **prior-report finding** (category C) →
**config/runtime** (category D/E) where one exists. Evidence IDs (`E001`…) refer to
`Evidence_Catalog.csv`, which also gives the exact filename and SHA-256. A chain is called
*verified* when at least three independent categories agree; otherwise the chain is labelled as
reconstruction at the weak link.

---

## Chain C1 — Patient identifier and branch position

Patient/barcode forms (A) → SQL
`SUBSTRING(patientid,2,1) + SUBSTRING(patientid,9,1) as bran` (VA `72714C`, `727AA0`, `7283A4`,
`728C98`, `729578`, `72A774`, `72BFC0`, `72C3C4`; **E001**) → `patientinfo.patientid
varchar(13)` and `testtable.testcodebill nvarchar(13)` (**E046**, **E001**) →
`Version_Identity_and_Scope.md` §2.5 and `06-security.md` F-05 identify position 9 as the
branch marker (**E056**, **E081**) → `Patient Data Transport.exe` transfers by that position
(**E025**, behaviour from prior report) → successor design note `Docs/KB/START-HERE.md`
invariant 8 repositions character 9 (external spec, not in catalog).
**Result:** v1.605 branch extraction CONFIRMED; other positions UNKNOWN.

## Chain C2 — Result status queue and handover

`Result*` forms / DataReports (A, **E004**) → SQL `resultfinsh='False'` (VA `6843F2`),
`resultfinsh='True' AND resultreview='False'` (`687B4E`), `... AND resultprint='False'`
(`688256`), `give='False'` (`6D4848`) (**E001**) → `patienttest` 46-column schema with
`edituser/editdate/prevuser/prevdate`, `printuser/printdate/printnum`, `taslimuser/taslimdate`
(**E037**, **E046**) → `1605-phase2-findings.md` U-08 (**E055**) → `05-workflows.md` WF-1/WF-2
F8/F12 and delivery (**E059**, INFERRED) → INI print flags `printcritical`/`printhistory`
(**E051**). **Result:** four independent flags + audit pairs VERIFIED; any ordering of them INFERRED.

## Chain C3 — Reference-range selection

Age/validation routines (A) → SQL `SELECT * FROM normalrange WHERE testcode=33 AND
REFRANGE='1' AND pregsmok='…'` (`6BE964`), `... REFRANGE='2' AND ageunit='…'` (`6BEBD0`),
`... sex='…'` (`6BEA80`), `SELECT Top 1 *` (`738FD4`) (**E001**) → `normalrange.csv` columns
and `normalrange-stats.json` 852 ranges / 304 tests (**E029**, **E030**) →
`1605-phase2-findings.md` U-01/U-02/U-03 (no unit conversion, no tie-break; **E055**) →
`04-clinical-rules.md` §3 (**E058**) → conflict NEW-3: shipped mirror has no `REFRANGE='2'`
rows (**E055**). **Result:** selection SQL CONFIRMED; live-server band data UNVERIFIED locally.

## Chain C4 — CBC / PT derivation constants

Calculation modules (A, method counts only) → constants read from `constanttable.csv`
`PTCONS (12,35,1.33,'2CVJW')` and `CBCCONS (3.3,6.25,6.75,7.5,8.25)` (**E043**) → `pt` (13
analytes incl. `pt, concentr, inr, ratios`) and `cbc` (17 analytes incl. `hgb, hct`) in
`branch-profile-schemas.csv` (**E046**) → `04-clinical-rules.md` §1–2 gives `Hct = Hgb × 3.3`
and ISI 1.33 (**E058**) → `1605-template-corpus.md` `PT.ini→pt`, `CBC.ini→cbc` (**E054**).
**Result:** constants CONFIRMED; formulas STRONGLY_SUPPORTED; INR exponentiation INFERRED.

## Chain C5 — Financial totals and commission

Accounting forms (A) → SHAPE `Round(sum(totalprice),2) tot1, Round(sum(totalpricedisc),2)
tot2, Round((sum(totalprice)-sum(totalpricedisc)),2) tot3, Round(sum(elmadfoo3),2) tot4,
Round(sum(elbakyma3mal),2) tot5` (VA `697446`) and
`ROUND(SUM(Gehats.omola*totalpricedisc/100),2)` (VA `694C92`) (**E001**) → `payment.csv`
(**E039**) and `patientinfo` 33 columns (**E038**) → `05-workflows.md` WF-4 (**E059**) →
`test-catalogue.csv` report titles (**E052**). **Result:** formulas CONFIRMED; screen
composition INFERRED; Access rounding UNKNOWN.

## Chain C6 — CBC as a composite test

DataReport `CBC` (66 DataReports, **E004**) → SQL `RESULTT1..RESULTT4 FROM patienttest WHERE
testcode=42` (VA `6C3AFC`) (**E001**) → `patienttest.RESULTT..resultt4` (**E037**) and `cbc` =
17 analytes (**E046**) → `1605-phase2-findings.md` U-12 (**E055**) →
`template-corpus-228.json` `CBC.ini joined_table='cbc'`, `reportmaintitle='HEMATOLOGY REPORT'`
(**E051**). **Result:** multi-component storage and report binding CONFIRMED; simple-code ↔
profile-column mapping UNKNOWN.

## Chain C7 — Microbiology culture and sensitivity

Culture-entry forms (A) → SQL `SELECT * FROM culturesec WHERE secabre='RA'` (`6749AA`…`67513E`),
`SHAPE ... INNER JOIN cultureanti ON culturesec.secabre = cultureanti.atisens` (`6BDE24`),
`WHERE antiid NOT IN (SELECT antiid FROM cultureanti ...)` (`6BC834`), `Delete FROM cultures
where patientid= '` (`6A77AA`) (**E001**) → `cultureanti.csv`, `culturesec.csv`,
`antibiotic.csv` schemas (**E047**, **E048**, **E049**) → `template-corpus-228.json`
`100.ini` = Urine Culture with `Organism A/B/C, Colony Count` labels (**E051**) and
`1605-template-corpus.md` (**E054**) → `04-clinical-rules.md` §4 critical values from RTFs
(**E058**). **Result:** schema and queries CONFIRMED; S/I/R meaning of `secabre` codes UNKNOWN.

## Chain C8 — Report engine and PDF output

66 DataReport objects (**E004**) → SQL print queues with `printuser/printdate/printnum`
(e.g. VA `6B037C`, **E001**) → `MSDBRPTR.DLL` PE parse (v6.0 SP4, GDI spool imports) in
`1605-phase2-findings.md` U-06/U-09 (**E055**) → literals `Bullzip PDF Printer` (`0x6A2486`)
and `Bullzip PDF Printer 11.1 - Create File` (`0x6A255E`) in `pdf-print-stack.md` (**E072**),
corroborated by `13-pdf-print-architecture.md` (**E071**) → `RL_SETUP.exe` /
`Setup_BullzipPDFPrinter_...exe` (**E020**, **E021**) and four `D:\PDF\` dirs →
`template-corpus-228.json` `[setupx] reportprinterx` 217/228 (**E051**). **Result:** full chain
CONFIRMED; no .NET reporting framework.

## Chain C9 — Operator accounts and audit

Login/operator forms (A) → SQL `startwork,endwork,breakyorn,breaktime FROM patients WHERE
patientusname= '` (`6D6F18`), `SELECT * FROM patients WHERE patientad=True` (`6EF0FC`), DDL
`ALTER TABLE Patients ADD grant13..17 Bit / SecWord / branche` (`6DC31C`–`6DC714`),
`SELECT * FROM loger WHERE datein >= '` (`72B7A4`), `Delete loger where datein >= '`
(`72C568`) (**E001**) → `07-security-findings.md` F-02 default credential (values withheld) and UNR-002/003/005
(**E082**), `06-security.md` F-05 (**E081**) → `useraccounts.HDD1/HDD2/Gtype` DDL (`677A7E`…)
(**E001**). **Result:** schema and queries CONFIRMED; grant semantics and password hashing UNKNOWN.

## Chain C10 — Dual SQL-Server / Access backend

Same query in two dialects: `Round(sum(...),2)` with `'date'` (VA `697446`) vs unrounded with
`#date#` (VA `697B32`); `varchar(MAX)/IDENTITY/smalldatetime` (VA `6756BE`) vs
`Memo/AUTOINCREMENT/DATETIME` (VA `67B88A`); 268 DDL = 27 CREATE + 241 ALTER (**E001**) →
`Version_Identity_and_Scope.md` §3.5: 61 embedded changes absent from shipped DB, 23
SQL-Server-only (**E056**) → `06-security.md` F-01 (**E081**) and
`1605-phase2-findings.md` U-11 (SQL Server 2005 Express, attached MDFs, **E055**).
**Result:** dual backend CONFIRMED; active dialect at runtime UNKNOWN.

## Chain C11 — Send-out tests and their balance

Send-out forms (A) → SQL `SELECT testcode,inotherlab FROM testtable WHERE testcode =` (`6F52E8`),
`SELECT testpriceout, testpriceoutmadfo, sentout, otherlabname, send FROM patienttest`
(`6F4674`, `6F4920`), `Round(SUM(testpriceout - testpriceoutmadfo),2)` (`699F92`) (**E001**) →
`testtable.csv` `inotherlab/otherlabprice` (**E027**) and `testtable-stats.json` 1 of 609
(**E028**) → `05-workflows.md` WF-3 (**E059**, INFERRED) → `sharekathesab.csv` company ledger
(**E041**). **Result:** fields and formula CONFIRMED; operator workflow INFERRED.

## Chain C12 — Template corpus completeness and test-code binding

Report-definition deploy step (`RL_SETUP.exe` XCOPY literals, **E020**) → 228 INI identities
parsed with `joined_table` and `reportmaintitle` (**E051**) → `1605-phase2-findings.md` U-05
(54 of 55 numeric IDs = `testtable.testcode`), NEW-2 (109 new files) (**E055**) →
`testtable-stats.json` 609 testcodes (**E028**) → `1605-template-corpus.md` title/printer
distributions (**E054**). **Result:** two disjoint template tiers CONFIRMED.

## Chain C13 — Version identity and object-count delta

PE version resources `FileVersion 1.605`, `FileDescription RLS 1.605` (**E012**, **E016**) →
`objects-full__[analysis_BATCH-04_vb6_v1605].txt` 210 objects / 66 DataReports (**E004**) →
`statements__[analysis_BATCH-04_sql_v1605].csv` 1,797 rows with 27 CREATE + 241 ALTER
(**E001**) → `Version_Identity_and_Scope.md` §3.2 identical figures (**E056**) →
`object-diff-vs-v1.json` +41 / 0 deletions (**E005**). **Result:** v1.605 identity and counts
CONFIRMED; build timestamp CONFLICTED (see Conflicting_Findings.md C-01).

---

## Coverage summary (denominator: 13 chains)

| Chain type | Count | Verified (≥3 categories) |
|---|---:|---:|
| Patient / order / result | 4 (C1, C2, C6, C11) | 4 |
| Clinical rules | 3 (C3, C4, C7) | 3 |
| Financial | 1 (C5) | 1 |
| Printing / reporting | 2 (C8, C12) | 2 |
| Security / architecture / version | 3 (C9, C10, C13) | 3 |

All 13 chains include an A-source and a B-source; 12 of 13 include a C-source; 9 of 13 include
a D/E config or runtime source. Chains whose last link is a workflow label (C2, C11) remain
INFERRED at that link only.
