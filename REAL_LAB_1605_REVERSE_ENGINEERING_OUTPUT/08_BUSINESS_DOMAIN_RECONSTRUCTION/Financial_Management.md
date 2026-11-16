# Financial Management — REAL LAB SYSTEM v1.605 (V-CD-2019)

**Scope.** Prices, discounts, payments, referral commission and partner accounting as
recoverable from the schema and SQL corpus. All figures are counts; no patient data are
shown. Static evidence only.

**Primary sources.** `v2019-Patients.mdb.sql`, `statements__[analysis_BATCH-04_sql_v1605].csv`,
`tables__[analysis_BATCH-04_sql_v1605].csv`, `pricelist.csv`, `payment.csv`,
`testtable-stats.json`, `Business_Rules_V2.md`, `MASTER_BUSINESS_LOGIC.md`,
`Database_Business_Logic.md` §2, `04-clinical-rules.md`.

## 1. Price storage — two parallel mechanisms — VERIFIED

| Mechanism | Table / columns | Evidence | Confidence |
|---|---|---|---|
| Per-test price fields | `testtable.patientprice`, `.labprice`, `.otherlabprice`, `.otherlabname` (Double / Text in DDL) | `v2019-Patients.mdb.sql`; VA `6AA272` selects all four with the order-picker | VERIFIED |
| Named price lists | `pricelist.pricenum`, `.testsimp`, `.pricelistname`, `.testprice` | `pricelist.csv` header (5 redacted sample rows); `select * from pricelist where testsimp='…'`; 11 statements in table inventory | VERIFIED |

V2-BR-018 records that "the price list is a distinct artefact from the per-test price
fields" — established directly by the two schemas. `testtable-stats.json`: **591 of 609**
tests carry a `patientprice` and **591 of 609** carry `otherlab_price`; **1** test is
enrolled for send-out. The mapping between a `pricelistname` and a `pricemethod` is
UNKNOWN.

## 2. Order-level totals and discounts — VERIFIED columns, INFERRED computation

`patientinfo` fields (`v2019-Patients.mdb.sql`): `pricemethod` Text(10), `totalprice`
Double, `totalpricedisc` Double, `discount` Double, `discount100` Double, `elmadfoo3`
Double (collected amount), `elbakyma3mal` Double (balance), `mowzaf` Text(40). The
`tables__[analysis_BATCH-04_sql_v1605].csv` inventory attributes 428 statements to
`patientinfo`, many of them aggregate finance queries.

Recovered aggregate SQL (exact column aliases):

| VA | Expression | Reading | Confidence |
|---|---|---|---|
| `696A66`, `697446` | `Round(sum(totalprice),2) AS tot1`, `Round(sum(totalpricedisc),2) AS tot2`, `Round((sum(totalprice)-sum(totalpricedisc)),2) AS tot3`, `Round(sum(elmadfoo3),2) AS tot4`, `Round(sum(elbakyma3mal),2) AS tot5` grouped by requestdate/pricemethod | Gross − discount = net; **2-decimal rounding**; five named totals | VERIFIED |
| `6943E2`, `6944F6` | `sum(totalprice) as tot1, sum(totalpricedisc) as tot2` | Unrounded variant of the same aggregation (Access dialect) | VERIFIED |
| `6913B6`, `6917B6` | `round(patientinfo.elmadfoo3,1)`, `round(sum(payment.money),1) as ta7seel` with `payment.type=0` | Collection reconciliation: expected vs actually paid | VERIFIED |
| `691F86`–`69232E` | `sum(totalpricedisc)`, `sum(elmadfoo3)`, `sum(elbakyma3mal)` by `referredbyname` | Referral-level finance totals | VERIFIED |

`discount100` is likely a percentage from its name only — INFERRED; UNR-002 leaves the
meaning of the `grantN` / pricing-adjacent fields unresolved. Per-invoice application of
`discount`, refund behaviour and adjustment semantics remain UNKNOWN.

## 3. Payments — VERIFIED structure

`payment` columns (header of `payment.csv`, matching `v2019-Patients.mdb.sql`):
`patientid` Text(13), `paiddate` DateTime, `type` Integer, `gehah` Text(50), `testsimp`
Text(50), `money` Double, `mowzaf` Text(40), `editdate` Text(40), `comment` Text(100),
plus `paymentnum` Long Integer in the DDL. `tables__[analysis_BATCH-04_sql_v1605].csv`
attributes **32 statements** to `payment` (29 SELECT) — this is the handoff's "payment 32".
`payment.type=0` is the collection filter used at VA `6913B6`; other `type` values are
UNKNOWN. Deletion is manual: `Delete FROM payment where patientid='` appears in the
64-table cascade (`State_Transitions.md` §5).

`patienttotal` (15 columns) is the per-patient aggregate sheet:
`SELECT * FROM patienttotal where patientid= '` (table inventory: 12 statements).

## 4. Referral commission — two verified bases (refines V2-BR-042)

V2-BR-042 in `Business_Rules_V2.md` states `SUM(Gehats.omola * patientinfo.elmadfoo3 / 100)`.
The v1.605 SQL corpus contains **both** that expression and a second one based on
`totalpricedisc`:

| VA | Expression | Confidence |
|---|---|---|
| `694C92`, `694E86` | `ROUND(SUM(Gehats.omola * patientinfo.totalpricedisc / 100),2) AS Expr1` | VERIFIED |
| `69C6E6`, `69CC8A`, `69CFB6`, `69D57E` | `ROUND(SUM(Gehats.omola * patientinfo.totalpricedisc / 100),2) AS Expr1, ROUND(SUM(Gehats.omola * patientinfo.elmadfoo3 / 100),2) AS Expr5, … Gehats.omola & ' %' AS Expr3` | VERIFIED |

Both are joined through `patientinfo.referredbyname = Gehats.gehahname` and filtered by
`requestdate`. The prior rule is therefore **preserved but incomplete**, not contradicted:
`totalpricedisc` and `elmadfoo3` appear as *different measures in the same report*
(Expr1 and Expr5). Which one is posted to which ledger is UNKNOWN. `Gehats` itself carries
`gehahname`, `morsel` (0/1/2 filter values seen at VAs `684F7A`, `685016`, `6B2984`),
`phone`, `mobil`, `masolphone`, `omola`, `ReferredID`, `ReferredPW` (DDL + VAs `67797A`,
`6779DA`).

## 5. Partner / company accounting — VERIFIED

`sharekat` (partners) and `sharekathesab` (partner transactions) from `v2019-Patients.mdb.sql`:
`sherkaid`, `sherkaname`, `taamelid`, `taameldate`, `wareed` (in), `saader` (out),
`taamelnote`. SQL evidence:

- VA `722674` / `722810`: `SELECT wareed, saader, (wareed - saader) as farq, taamelnote,
  sherkaname, taameldate FROM sharekathesab …`
- VA `722084` / `722BB8`: `SELECT sum(wareed) as sumw, sum(saader) as sums FROM
  sharekathesab where sherkaid= …` / by date
- VA `7222A0`: `Delete FROM sharekat where sherkaid=`

Numerous `taamelid` filters (`-5`, `2`, …) exist; the **meaning of `taamelid` codes is
UNKNOWN**. Earlier stub's "sharekathesab (partner accounting)" is preserved and revalidated.

## 6. Financial side tables and send-out settlement

`gehats` = referring parties; `phone` (11 statements) = contact list; `mowzaf` appears as
a text field on `patientinfo` and `payment` (employee/cashier, INFERRED from name).
Send-out settlement uses `patienttest.testpriceout` (quoted price) and
`.testpriceoutmadfo` (paid amount), alongside `sentout`, `send`, `otherlabname` — two
settlement states per V2-BR-044. `testtable.otherlabprice` supplies the catalogue value.

## 7. Prior knowledge preserved

| Prior claim | Source | Revalidation |
|---|---|---|
| `patientprice`, `labprice`, `otherlabprice` + `pricelist` | original stub; V2-BR-018 | VERIFIED |
| `payment` table; `sharekathesab`; `moneypaid` form; `Khasmlimit` | original stub | Tables VERIFIED; `moneypaid` form and `khasmlimit` (on `patients`, DDL) STRONGLY_SUPPORTED as prior UI/field claims |
| Referral commission = rate × patient base ÷ 100 | V2-BR-042 | VERIFIED but **two bases** recovered; preserved with correction |
| Net = gross − discount, rounded to 2 decimals | `MASTER_BUSINESS_LOGIC.md` §5, VA `69EA76` | VERIFIED with VAs `696A66`/`697446` in this corpus |
| Per-invoice formula/refund/adjustment UNKNOWN | `MASTER_BUSINESS_LOGIC.md` §5 | Preserved — UNKNOWN |

## 8. Unknowns

`pricemethod` domain; `type` value domain in `payment`; `taamelid` codes; meaning of
`discount100`/`grantN` pricing bits (UNR-002); which commission measure is posted where;
any runtime observation of a financial transaction. All UNKNOWN / UNVERIFIED.

**Cross-references:** `Laboratory_Business_Rules.md`, `Visits_And_Orders.md`,
`09_CROSS_SOURCE_EVIDENCE/Conflicting_Findings.md`.
