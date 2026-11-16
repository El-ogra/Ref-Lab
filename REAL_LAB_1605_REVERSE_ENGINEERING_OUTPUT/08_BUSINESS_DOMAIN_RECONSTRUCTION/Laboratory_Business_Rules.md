# Laboratory Business Rules — REAL LAB SYSTEM v1.605 (V-CD-2019)

**Consolidation scope.** This document merges the findings of the original stub with
`Business_Rules_V2.md` (V2-BR-001…045), `04-clinical-rules.md`,
`MASTER_BUSINESS_LOGIC.md`, `Database_Business_Logic.md`, `1605-phase2-findings.md` and
the primary dumps. Prior identifiers are preserved; nothing is renumbered. Confidence
labels follow `09_CROSS_SOURCE_EVIDENCE/Confidence_Assessment.md`.

## 1. Rule inventory (cross-domain)

| ID | Rule (condensed) | Primary evidence | Confidence |
|---|---|---|---|
| V2-BR-034 | Abnormal classification driven by `normalrange` limits | `normalrange.csv` columns; CHM `004A.htm` | VERIFIED (data + prior doc) |
| V2-BR-035 | Critical classification overrides abnormal; visual only | `report-structure.md` §2 thresholds; prior doc | STRONGLY_SUPPORTED |
| V2-BR-036 | Ranges banded by sex + age + `pregsmok` | `normalrange.csv` 852 rows / 304 tests; VAs `68AB1E`… | VERIFIED |
| V2-BR-037 | Derivation factors stored as data (`CBCCONS`, `PTCONS`, `RATIOS`) | `constanttable.csv`; exact manual concordance in `04-clinical-rules.md` §1 | VERIFIED |
| V2-BR-038 | Only reviewed results eligible for release | `patienttest.resultreview`; print refusal literal | STRONGLY_SUPPORTED |
| V2-BR-039 / DB §6 | Seventeen `grantN` bits + operator branch descriptor | `patients.grant1…grant17` DDL; VAs `6DC31C`…`6DC64C` | VERIFIED (columns); meaning UNVERIFIED (UNR-002) |
| V2-BR-040 | 13-char `patientid` is an order/visit id with positional segments | DDL Text(13); VAs `6F7ED8`, `68313E`, `72714C` | VERIFIED for positions 3–4, 9 (`as bran`, 18 statements) and 10–12 |
| V2-BR-042 | Referral commission is a percentage of a patient-level base | VAs `694C92`, `69C6E6`, `69CC8A`, `69CFB6`, `69D57E` | VERIFIED (two bases — see §4) |
| V2-BR-044 | Send-out flag-driven with two settlement states | `testtable.inotherlab`; `patienttest.sentout/send/testpriceout*` | VERIFIED |
| V2-BR-045 | Printed output audited per operator, dated, sequenced | VAs `6865A6`, `6B037C`, `6B2D70` | VERIFIED |
| V2-BR-023 | 268 embedded DDL statements; shipped DB out of step | `statements__[analysis_BATCH-04_sql_v1605].csv` (27 CREATE + 241 ALTER) | VERIFIED (DDL count); application gap STRONGLY_SUPPORTED |
| V2-BR-024 | DDL written in SQL Server syntax; some statements cannot run on Jet 4 | dual-syntax pairs in DDL corpus | STRONGLY_SUPPORTED |
| V2-BR-017 | Cross-profile consolidation requires `MSDataShape.1` | 27 SHAPE statements; `data_connection1.udl` | VERIFIED (mechanism); Access behaviour UNKNOWN |
| V2-BR-032 | DB credential shared, static, cleartext | `BASE.ini`, `.udl`, installer scripts | STRONGLY_SUPPORTED (prior work) |

## 2. Catalogue governance

`testtable-stats.json`: **609** tests, **94 active**, **605 delete-protected**, **591**
with `patientprice`, **591** with `otherlab_price`, **48** distinct subgroups, **1**
send-out. `grouptable.csv` / `subgrouptable.csv` carry `deleteprotect` columns; the flat
samples show 12 redacted rows each, so the 46-vs-48 subgroup discrepancy is a matter of
"declared subgroups" versus "subgroups actually used by tests" — recorded as CONFLICTED
pending full (redacted) data. `select testcode, testsimp, … from testtable where testorno
like '…'` (VA `6AA272`) and `SHAPE … RELATE subgroupcode TO subgroupcode` (VA `672722`)
establish the orderable hierarchy. Rule V2-BR-043 ("test number 4 restriction") is visible
in the corpus as repeated `WHERE testcode=4` reads (VAs `6AD3A8`, `6AF6B8`) — STRONGLY_SUPPORTED.
Delete veto itself is application-side — INFERRED, since no DB-level CHECK/FK is declared
(`Constraints_And_Indexes.md` §1).

## 3. Clinical knowledge layer

- **Units and conversions** — `converts.csv` (167 rows; `testsimp, Funit, Sunit, Factors,
  Ffix, Sfix`), `testunits.csv` (`unitsid, unitsname`). `04-clinical-rules.md` §5 lists
  sample factors (Albumin g/dL→g/L = 10; Acetaminophen µg/mL→µmol/L = 6.62). Formula
  application is INFERRED from column names.
- **Abbreviations** — `abbreviations.csv` (650 rows; `abbid, abbre, mean`).
- **Critical limits** — from cached RTF sheets: CBC (`056.rtf`), BUN/Urea (`013.rtf`),
  Bilirubin (`015.rtf`), ionised/ total calcium (`175.rtf`, `176.rtf`), sodium (`180.rtf`).
  No callback or critical-result log beyond the report — STRONGLY_SUPPORTED gap.
- **Microbiology** — `cultures` (35 cols), `cultureanti` (12), `culturesec` (3) with
  `SHAPE {… culturesec INNER JOIN cultureanti ON culturesec.secabre = cultureanti.atisens …}`
  at VA `6BDE24`; `antibiotic.anticomprint1…8` Boolean print flags. `Database_Business_Logic.md`
  §10 — STRONGLY_SUPPORTED. The earlier stub's "Reportqc" and C&S subgroup are preserved
  as prior-work claims; no primary report object was inspected.
- **PT control curve** — `04-clinical-rules.md` §2: 33 `control`↔`concent` points
  (12→100, 12.5→92, …, 16.2→56). STRONGLY_SUPPORTED (prior data analysis).

## 4. Pricing and referral rules

Per-test prices live on `testtable` (`patientprice`, `labprice`, `otherlabprice`,
`otherlabname`); named price lists live in `pricelist` (`pricenum`, `testsimp`,
`pricelistname`, `testprice`; `select * from pricelist where testsimp='` in the corpus).
Order/patient totals are persisted on `patientinfo` (`totalprice`, `totalpricedisc`,
`elmadfoo3` = paid, `elbakyma3mal` = balance, `discount`, `discount100`, `pricemethod`).
Aggregates use 2-decimal rounding: VA `696A66` selects `Round(sum(totalprice),2)`,
`Round(sum(totalpricedisc),2)`, `Round(sum(totalprice)-sum(totalpricedisc),2)`,
`Round(sum(elmadfoo3),2)`, `Round(sum(elbakyma3mal),2)` grouped by request date.

**Commission — two verified bases** (this refines V2-BR-042, which named only one):
`SUM(Gehats.omola * patientinfo.totalpricedisc / 100)` (VAs `694C92`, `694E86`) **and**
`SUM(Gehats.omola * patientinfo.elmadfoo3 / 100)` (VAs `69C6E6`, `69CC8A`, `69CFB6`,
`69D57E`). Both exist in the same statements. See `Financial_Management.md` §4 and
`09_CROSS_SOURCE_EVIDENCE/Conflicting_Findings.md`.

Partner accounting: `sharekat` / `sharekathesab` with `wareed`, `saader`,
`(wareed - saader) as farq`, `sherkaid`, `taamelid` (VAs `722084`, `722674`, `722810`).
`payment` columns are `patientid, paiddate, type, gehah, testsimp, money, mowzaf,
editdate, comment` (`payment.csv` header) — 32 statements in the table inventory.

## 5. Schema migration as a business rule

The executable embeds **268 DDL statements** (27 `CREATE TABLE` + 241 `ALTER TABLE`);
verb census of `statements__[analysis_BATCH-04_sql_v1605].csv` confirms 27 CREATE and
241 ALTER. `Version_Identity_and_Scope.md` §3.5 and `Database_Business_Logic.md` §9 record
that **61 changes are absent from the shipped database** (23 SQL-Server-only syntax; 38
Access-applicable but not applied). Consequences recorded in prior work: IgE allergy
reporting, `useraccounts` permission model, extended blood-gas panel and NATIGH delivery
recording have no storage in the shipped file. Confidence: STRONGLY_SUPPORTED (the
23/38 split is not independently recomputable from the primary dump alone).

## 6. Version-scoped deltas preserved

`Version_Identity_and_Scope.md` §3.2: v1.605 has **210 VB6 objects (+41)**, **132 forms**,
**66 DataReports**, **1,797 SQL statements**, **117 SQL-referenced tables**. The CHM help
is **byte-identical** to v1.00 (SHA-256 `624308649E523400…`, 5,193,985 bytes) while 41
objects were added — documentation was never updated. `RL_SETUP.exe` (23,060,480 bytes)
carries an embedded payload of unidentified compression (≈22.9 MB reported); it cannot be
enumerated without running the installer, which is prohibited. New in v1.605: SMS
subsystem (`SMSSender`, `SMStemplate`, `smslist`, `smsgroups`, `MyLabSetting`), statistics,
worksheets, attendance (`loger`), `IgEallergy`, `Helicobacter`, `insulin`, `LBC`, `FMF`.

## 7. Explicit non-findings

No DB-level DEFAULT, CHECK, PRIMARY KEY, FOREIGN KEY, UNIQUE, index or trigger is declared
(`Constraints_And_Indexes.md` §1). No `INSERT`/`UPDATE` literal exists. No patient PII is
reproduced. Runtime behaviour of every rule above is NOT OBSERVED (binary absent).

**Cross-references:** all eight documents in `08_BUSINESS_DOMAIN_RECONSTRUCTION/`,
`09_CROSS_SOURCE_EVIDENCE/Verified_Findings.md`, `.../Conflicting_Findings.md`.
