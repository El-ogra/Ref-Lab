# MASTER BUSINESS LOGIC — REAL LAB SYSTEM v1.605

Every rule below is tied to an evidence source. Rules derived only from naming or prior documentation are labeled accordingly. No invented rules.

## 1. Patient identity and registration

- Demographics in `patientinfo`; 13-character `patientid`; worklists compute `SUBSTRING(patientid,10,3) AS patid` (VAs `682F56`, `683C72`, `6843F2`). [VERIFIED: SQL]
- **Position 9 is the branch code in v1.605 (VERIFIED):** 18 SQL statements compute `SUBSTRING(patientid,2,1) + SUBSTRING(patientid,9,1) AS bran` (e.g. VA `72714C`); prior reports identify `patients.branche` as its source. AGENTS.md rule 8 ("position 9 absorbed into the middle segment") describes the **C# successor design**, not this legacy target.
- Duplicate-detection algorithm, ID generation algorithm: UNKNOWN.

## 2. Ordering

- `patienttest` holds one row per ordered test with `patientid`, `testcode`, `testsimp`, `requestdate`, result flags. [VERIFIED: DDL + 343 statements]
- Catalogue selection uses `testtable.testorno='True' AND testsimp<>''`; hierarchical SHAPE by `subgroupcode`. [VERIFIED: VA `672722`]
- Order modification/cancellation rules: UNKNOWN.

## 3. Simple vs composite tests

- `testtable` (609 tests) and `template-corpus-228.json` are the primary definition sources; `constanttable.CBCCONS` holds CBC constants and `PTCONS` PT/INR constants (prior work).
- `patienttest` carries `RESULTT` + `resultt1..resultt4` (VA `6C3AFC`, `testcode=42`), so **up to five components can share one order/result row**; single-component tests use only `RESULTT` (VA `6AD3A8`, `testcode=4`). Additional analytes live in per-discipline profile tables (e.g. `cbc`, `kidney`). [VERIFIED structurally]
- **No field declares "composite"**; the component-count mapping per `testcode` is UNKNOWN without form code. Validation/display/printing differences are UNVERIFIED. Do not assume identical processing.
- Reference ranges: `normalrange` (852 rows / 304 tests) stratified by `sex`, `firstold`, `secondold`, `ageunit`; `normalrang<>''` filter used in SHAPE. [VERIFIED: VA `672E3E`]
- Units from `testunits` (69) and conversions from `converts` (167). [prior work, STRONGLY_SUPPORTED]

## 4. Result lifecycle

- Flags: `printwith`, `resultfinsh`, `resultreview`, `resultprint` on `patienttest`; audit pairs `edituser/editdate`, `prevuser/prevdate`, `printuser/printdate`. [VERIFIED: VAs `681ED2`, `6860BE`, `686312`, `6865A6`]
- Derived formula recovery: PT/INR and CBC constants exist in `constanttable`; exact computations UNKNOWN.
- Abnormal-result flagging and reference-range comparison logic: evidenced as data (`normalrange`) but the comparison code path is UNKNOWN.

## 5. Financial rules

- Price columns: `pricelist` (`patientprice`, `labprice`, `otherlabprice`; VA `6A9632`); `patientinfo.totalprice`, `totalpricedisc`, `elmadfoo3` (paid), `elbakyma3mal` (balance), `discount`, `discount100`. [VERIFIED: VAs `6977A6`, `69DE12`, `69E4E6`, `69F48A`, `69FB46`, `6A16A2`, `6A5796`]
- **Recovered aggregate formula (VERIFIED, VA `69EA76):** `Round(sum(totalprice),2) AS tot1`, `Round(sum(totalpricedisc),2) AS tot2`, `Round(sum(totalprice)-sum(totalpricedisc),2) AS tot3`, `Round(sum(elmadfoo3),2) AS tot4`, `Round(sum(elbakyma3mal),2) AS tot5`, grouped by `pricemethod`, filtered by `requestdate >= '…'`. Net = gross − discount; **rounding is to 2 decimal places** in the SQL Server dialect (`#date#`/Memo variants exist for Access, whose rounding is not separately evidenced).
- **Referrer commission (VERIFIED, VAs `694C92`, `694E86`, `69C6E6`):** `ROUND(SUM(Gehats.omola * patientinfo.totalpricedisc / 100),2)` and the paid counterpart `ROUND(SUM(Gehats.omola * patientinfo.elmadfoo3 / 100),2)`, joined on `patientinfo.referredbyname = Gehats.gehahname` — i.e. commission rate × discount/paid amount ÷ 100.
- **Company account difference (VERIFIED, VA `722674`):** `(wareed - saader) AS farq` from `sharekathesab` (received − paid).
- **Outstanding-balance predicate (VERIFIED, VAs `6CEA20`, `6CEC14`, `6CEE00`):** `WHERE (totalpricedisc <> elmadfoo3 OR elbakyma3mal <> 0)` — the source of the "unpaid patients" worklist.
- `payment` table (32 statements) for transactions; payment-type signs 0/2 positive and 1/3 negative are reported by the final business-domain analysis (attribute to that analysis; per-type semantics UNKNOWN). `discount100` is likely a percentage (INFERRED from name only).
- **Per-invoice formula application, refund behavior, and adjustment semantics remain UNKNOWN.**

## 6. Printing and reporting

- 66 VB6 DataReport designers; `printerselect` module; Win32 spooler APIs; Bullzip PDF Printer 11.1 with Ghostscript Lite/Xpdf/PDF Power Tool; 228 RTF method-sheet templates; INI-driven `REPORT.dll`. [VERIFIED components; pipeline INFERRED]
- 29 RTF sheets byte-identical across versions; report strip config in `reportview` (`stripdiscrp='MAIN'`, VA `66F882`).
- Multi-component result presentation details: UNKNOWN.

## 7. Security and permissions

- Operator accounts in the `patients` table (`patientusname`, `grant1..grant12`, shift schedule); `loger` logging table (VA `66F746`). [VERIFIED: DDL + SQL]
- Prior work reports a default operator credential pair — PRIOR-WORK CLAIM; literal values withheld here per the no-secrets rule.
- Form gating by grant flags is INFERRED; password handling mechanism UNKNOWN.

## 8. Data maintenance rules

- Writes via bound recordsets (0 literal INSERT/UPDATE). [VERIFIED negative finding]
- Manual cascade deletion over 64 tables by `patientid`. [VERIFIED: 72 DELETE FROM statements]
- Backup via SQL Server backup devices (`sp_addumpdevice`/`sp_dropdevice`, VAs `66FCB2`/`66FD56`) and transport utilities (`Patient Data Transport.exe`, RLS ACCESS/SQL Database Tools, `Attach_*.bat`).
- Embedded schema migration: 27 CREATE + 241 ALTER; shipped DB out of step by 61 changes (23 SQL-Server-only, 38 Access-applicable not applied). [STRONGLY_SUPPORTED]

## 9. Cross-version business deltas

v1.605 adds ~41 objects, 17 SQL-referenced tables, SMS subsystem, statistics, worksheets, attendance, new knowledge tables (`IgEallergy`, `Helicobacter`, `insulin`, `LBC`, `FMF`, …). The CHM help is byte-identical to v1.00 — documentation was not updated for these features. [STRONGLY_SUPPORTED: `Version_Identity_and_Scope.md`]

## 10. Confidence map

| Domain | Confidence |
|---|---|
| Field-level data model and SQL structure | VERIFIED |
| Catalogue/reference data counts | STRONGLY_SUPPORTED (prior dumps) |
| Result flag existence | VERIFIED |
| Result flag ordering | INFERRED |
| Financial formulas | UNKNOWN |
| Composite result storage | UNKNOWN |
| Permission enforcement mechanism | INFERRED |
| Runtime behavior of any rule | NOT OBSERVED |
