# Verified Findings — Real Lab System v1.605 (V-CD-2019)

**Definition.** A finding is **VERIFIED** when two or more independent evidence items (different
files, preferably different categories) assert the same fact and no primary dump contradicts it.
**STRONGLY_SUPPORTED** = one primary dump plus a consistent prior report. IDs map to
`Evidence_Catalog.csv` (exact filename + SHA-256). "Verified" means corroboration of the source
assertion, not runtime proof: nothing was executed.

---

## V-01 — Product identity and version resource
**VERIFIED.** `pe-metadata.json` and `Version_Identity_and_Scope.md` §3.1 report
`FileVersion 1.605`, `FileDescription "RLS 1.605"`, `CompanyName RealLab`; the v1.605 object
dump is `objects-full__[analysis_BATCH-04_vb6_v1605].txt`. Evidence: **E012, E016, E056, E004**.
**Limitation:** the flat `real lab system.exe` hashes to the **v1.00** identity (**E019**, per
`RealLab_Reverse_Engineering_Master_Handoff_v2.md`, E079); the v1.605 binary is not local.

## V-02 — Object model counts
**VERIFIED.** 210 VB6 objects, 132 forms, **66 DataReports**, 9 modules, 3 classes, 8,026
declared methods, 270,166 string literals. Evidence: **E004, E005, E006, E007, E009, E056**.
The SQL count (1,797) and DDL counts (27 CREATE + 241 ALTER) were independently reproduced from
the SQL CSV (**E001**) — an exact numeric cross-check.

## V-03 — 1,797 SQL literals and dual dialect
**VERIFIED.** 1,797 rows; verbs 1,381 SELECT / 241 ALTER / 72 DELETE FROM / 49 OTHER / 27
SHAPE / 27 CREATE. Major queries exist in SQL-Server and Access/Jet forms (`ROUND(...,2)` +
`'date'` vs unrounded + `#date#`; `varchar(MAX)/IDENTITY` vs `Memo/AUTOINCREMENT`). Evidence:
**E001, E056, E081, E055**.

## V-04 — 13-character patient ID with branch at position 9
**VERIFIED for the extraction.** `SUBSTRING(patientid,2,1)+SUBSTRING(patientid,9,1) as bran`
in nine SQL literals (VA `72714C`…`72C3C4`); `patientid varchar(13)` in DDL;
`06-security.md` F-05 and `Version_Identity_and_Scope.md` §2.5 independently name position 9
as the branch marker. Evidence: **E001, E046, E056, E081, E025**. Other positions UNKNOWN.

## V-05 — Result status flags and queue predicates
**VERIFIED.** `patienttest.resultfinsh`, `resultreview`, `resultprint`, `give` with audit
columns `edituser/editdate/prevuser/prevdate`, `printuser/printdate/printnum`, `taslimuser/
taslimdate`; exact queue predicates occur in the SQL CSV and are summarised in
`1605-phase2-findings.md` U-08. Evidence: **E001, E037, E055, E059**.

## V-06 — Catalogue and range counts
**VERIFIED.** 609 tests / 38 columns in `v2019-Patients.mdb.sql` (37 in the prior mdb export) / 48 distinct subgroups / 94 active / 605 delete-protected
/ 1 send-out (**E027, E028**); 852 ranges over 304 tests with sex/age/unit distributions
(**E029, E030**); 167 conversion records (**E032**); 12 unit rows (**E033**); 650 abbreviations
(**E034**). Corroborated by `04-clinical-rules.md` (**E058**).

## V-07 — Derivation constants
**VERIFIED.** `constanttable.csv`: `PTCONS (12,35,1.33,'2CVJW',0,0)` and
`CBCCONS (3.3,6.25,6.75,7.5,8.25,0)`; `04-clinical-rules.md` §1 lists the same values with
formula readings; `pt` and `cbc` profile tables provide destination columns. Evidence:
**E043, E058, E046**. Formulas STRONGLY_SUPPORTED, not runtime-verified.

## V-08 — Financial formulas
**VERIFIED.** Net = `totalprice − totalpricedisc`; paid = `elmadfoo3`; balance =
`elbakyma3mal`; outstanding filter `totalpricedisc <> elmadfoo3 OR elbakyma3mal <> 0`;
commission = `omola × amount / 100`; two-decimal `ROUND` in SQL Server. Evidence: **E001**
(VA `697446`, `694C92`, `6CEC14`), **E039, E038, E059**. Access dialect lacks `ROUND` — UNKNOWN.

## V-09 — Payment sign convention
**VERIFIED.** `CASE type WHEN 0 THEN money WHEN 1 THEN money * -1 WHEN 2 THEN money WHEN 3
THEN money * -1 END` (VA `6926E2`) and Access `IIF` twin (`692966`); `payment.csv` columns
`type, money, paiddate, mowzaf, gehah`. Evidence: **E001, E039, E059**. Type *names* not evidenced.

## V-10 — Company ledger arithmetic
**VERIFIED.** `sharekathesab` balance = `wareed − saader` as `farq` (VA `722674`, `722810`,
`722DA4`, `72301C`). Evidence: **E001, E041, E042**.

## V-11 — Composite-result storage
**VERIFIED.** `patienttest.RESULTT` + `resultt1..resultt4` (VA `6C3AFC`, `6895B6`); 49 profile
tables in a prior derived inventory with up to 81 analyte columns (`crossmat`), 56 (`urine`,
`stool`, `semen`), 17 (`cbc`); the primary `v2019-Patients.mdb.sql` gives `cbc` 29 columns;
36 distinct `joined_table` values in `template-corpus-228.json`. Evidence: **E003, E037, E046, E051, E054, E055**.

## V-12 — Report template corpus
**VERIFIED.** 228 INI files, 117 identities (base + `_S`), 3 sections, 72 files / 36 identities
joining a result table, two disjoint ID tiers (alphabetic = table name; numeric = `testcode`,
54 of 55), 217/228 printer override `HP LaserJet 1100 (MS)`, 12 spellings of the range label.
Evidence: **E051, E054, E055** (U-05/U-12/NEW-2/NEW-4/NEW-5), **E052**.

## V-13 — VB6 DataReport + Bullzip PDF pipeline
**VERIFIED.** 66 DataReports; `MSDBRPTR.DLL` (v6.0 SP4) with GDI spool imports; hard-coded
"Bullzip PDF Printer" (`0x6A2486`) and "Bullzip PDF Printer 11.1 - Create File" (`0x6A255E`);
four `D:\PDF\` output dirs; six printer roles; 141 `PDF` literals in v1.605 vs 0 in v1.00.
Evidence: **E004, E056, E055** (U-06/U-09/NEW-6), **E072, E071, E020, E021, E014**.
Ghostscript/Xpdf/PDF Power Tool are manual-only: **E022, E023, E024, E072**.

## V-14 — Operator table `patients` and audit `loger`
**VERIFIED.** `patients.patientusname/patientword/patientad/startwork/endwork/breakyorn/
breaktime`, `grant13..17`, `SecWord`, `branche`; `loger.lognum/compname/username/datein` plus
a date purge; `useraccounts.usersnum/usersdr/HDD1/HDD2/Gtype`. Evidence: **E001, E056, E082,
E081**. Grant semantics UNKNOWN (UNR-002).

## V-15 — Default operator credential (values withheld)
**STRONGLY_SUPPORTED.** Official help topic `002.htm` instructs choosing a documented default
operator account with a matching password; a credential literal is reported at `0x7BA378`. The
literal values are deliberately **withheld** here per the no-secrets rule. Evidence: **E087,
E056, E082**. No write path in the SQL dump confirms how it is stored.

## V-16 — Config credentials and SQL Server deployment
**STRONGLY_SUPPORTED.** A cleartext SQL Server account/password pair in `BASE.ini` and `data_connection1.udl`, and an `SAPWD` value in setup scripts — literal values deliberately **withheld** per the no-secrets rule (record existence and location only). `SECURITYMODE=SQL`, `DISABLENETWORKPROTOCOLS=0`, SQL Server
2005 Express with attached MDFs. Evidence: **E081, E082, E083, E055** (U-11). The `.ini`/`.udl`/
`.bat` files are not flat here; findings are file-level from prior reports.

## V-17 — No declared primary keys or foreign keys
**VERIFIED (negative).** `1605-phase2-findings.md` K-01/K-02 report no `MSysIndexes` and empty
`MSysRelationships` across nine databases. Evidence: **E055, E050**.

## V-18 — Training animations are not report components
**VERIFIED (negative correction).** `Cache\*.exe` = 13 Flash Player 8 training animations;
`09-presentation-library.md` overturns the earlier interactive-report hypothesis. Evidence:
**E075, E074, E058**. `Cache\*.rtf` = 29 method sheets with critical values — **E085, E086,
E058**.

## V-19 — No client-side print-result password
**VERIFIED (negative).** `1605-phase2-findings.md` U-07: credential sweep of `MSDBRPTR.DLL`,
`RL_SETUP.exe` and all 228 templates returned zero hits. Evidence: **E055, E020, E051**.

## V-20 — Build timestamp conflict
**VERIFIED as a conflict, not as a fact.** `Version_Identity_and_Scope.md` §3.1/§4 give
`2013-12-27` for both executables; `contradictions-limitations-and-unresolved-questions.md`
§1.2 gives PE link timestamp `0x5F186BBA` = `2020-07-24`. Evidence: **E056, E057, E012, E010**.
See `Conflicting_Findings.md` C-01.

---

## Counts (denominators stated)

- 20 findings: 18 VERIFIED (including 4 verified negatives/conflicts), 2 STRONGLY_SUPPORTED.
- Domains with at least one verified finding: **8 of 8** assigned business domains.
- Domains still containing UNKNOWN sub-fields: **8 of 8** (see `Confidence_Assessment.md`).
