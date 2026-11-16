# Results Management — REAL LAB SYSTEM v1.605 (V-CD-2019)

**Scope.** Entry, classification, review, release and audit of results. No runtime
observation was performed. All VAs are from
`statements__[analysis_BATCH-04_sql_v1605].csv`; field declarations from
`v2019-Patients.mdb.sql`.

## 1. Where results live

Two storage layers coexist:

| Layer | Tables | Evidence | Confidence |
|---|---|---|---|
| Per-order test row | `patienttest` — `RESULTT` Text(50), `resultt1`…`resultt4` Text(50), `RESULTCASEPRINT` Text(15), `normalrang` Text(255), `lowlimit`/`highlimit` Double, `nolowexe`/`nohighexe` Text(20), `testunit` Text(15) | DDL; VA `6895B6` selects `resultt, resultt1..resultt4, resultprint, testcode, printwith, testabrev, testsimp, testun…` | VERIFIED |
| Wide profile tables | `cbc`, `kidney`, `liver`, `pt`, `urine`, `semen`, … keyed by `patientid`+`requestdate`, one column per analyte | `v2019-Patients.mdb.sql`; `Database_Business_Logic.md` §10 (53 such tables) | VERIFIED |

`comments` holds per-test free text (`commentid`, `testcode`, `subgroupcode`, `testsimp`,
`comments`); the SQL inventory (`tables__[analysis_BATCH-04_sql_v1605].csv`) attributes
**28 statements** to it, e.g. `SELECT comments FROM comments where testcode in (`.
`history` (25 columns, 20 free-form slots) and `noteic` are side tables for longitudinal
text. Confidence: VERIFIED.

## 2. The result flag tuple and its audit columns — VERIFIED

The single most important result-lifecycle evidence is a set of paired SELECTs:

| VA | Exact SQL (truncated at the literal's end) | Flag + audit pair | Confidence |
|---|---|---|---|
| `681ED2` | `Select patientid,printwith,resultfinsh,resultreview,resultprint from patienttest where patientid= '` | all four flags together | VERIFIED |
| `6860BE` / `6861D2` | `Select testcode,patientid,resultfinsh,edituser,editdate from patienttest where patientid= '` | `resultfinsh` + editor/date | VERIFIED |
| `686312` / `68647E` | `Select testcode,patientid,resultreview,prevuser,prevdate from patienttest where patientid= '` | `resultreview` + reviewer/date | VERIFIED |
| `6865A6` / `68666E` | `Select testcode,patientid,resultprint,printuser,printdate from patienttest where patientid= '` | `resultprint` + printer/date | VERIFIED |
| `6867AA` / `686862` | `Select testcode,patientid,give,taslimuser,taslimdate from patienttest where patientid= '` | hand-over flag + auditor/date | VERIFIED |
| `686EC2` | `Select testcode, testsimp, arrangnum, RESULTT, RESULTCASEPRINT, resultfinsh, resultprint, give, resultreview, NatighU from patienttest …` | full tuple incl. delivery flag | VERIFIED |
| `6AD3A8`, `6B8EBC`, `6BB7CC`, `6C066C` | `SELECT patientid,testcode,RESULTCASEPRINT,resultfinsh,resultreview,edituser,editdate,prevuser,prevdate FROM patienttest WHERE testcode=…` | per-discipline flag reads (testcode 4, 5, 300, …) | VERIFIED |

`RESULTCASEPRINT` Text(15) and `printnum` Double are additional print-state fields;
`printnum` is the sequenced print count used as an anti-copy control
(`13-pdf-print-architecture.md` §4).

> **Explicit non-claim.** These are independent Boolean flags read together, **not a
> sequential status enum**. The ordering `resultfinsh → resultreview → resultprint` is an
> INFERRED natural reading of the names and of the entry→review→print data flow, as
> reconstructed in `07_RUNTIME_AND_BEHAVIORAL_ANALYSIS/State_Transitions.md` §2. Whether
> `resultreview` can precede `resultfinsh`, whether flags can be reset (recheck/reprint),
> and the Boolean encoding are all **not established**. No state machine is invented here.

## 3. Classification against reference ranges

`04-clinical-rules.md` §3 and `Business_Rules_V2.md` V2-BR-034…036 establish the rule:
an entered value is compared with the limits held in `normalrange` (852 rows / 304 test
codes), banded by `sex`, `firstold`/`secondold`, `ageunit` and `pregsmok`; the applicable
limits are **frozen onto `patienttest`** at order time as `normalrang`, `lowlimit`,
`highlimit`, `nolowexe`, `nohighexe`. A change to the range table therefore does not affect
already-ordered tests. Confidence: STRONGLY_SUPPORTED (structure + prior report).

| Sub-rule | Evidence | Confidence |
|---|---|---|
| Band selection pins the unit to the patient's own unit; **no unit conversion** in the age path | `1605-phase2-findings.md` U-01 (disassembly of six procedures, no `fmul`/`fdiv`) | STRONGLY_SUPPORTED (prior disassembly) |
| Overlapping bands have **no tie-break**; selector emits `SELECT Top 1 …` with no `ORDER BY` | U-02, procedure `0x035796B0` | STRONGLY_SUPPORTED (prior disassembly) |
| No 365-day/30-day conversion constants in the reference-interval path | U-03 (negative result) | STRONGLY_SUPPORTED |
| `normalrange.REFRANGE` is the row-kind discriminator (`'1'` pregnancy, `'2'` age band, `'3'` second pregnancy variant) | U-04 | STRONGLY_SUPPORTED |
| Critical thresholds override abnormal classification; critical state is **visual only**, no escalation workflow evidenced | `Business_Rules_V2.md` V2-BR-035; critical values in `report-structure.md` §2 (`056.rtf` CBC, `013.rtf` BUN/Urea, `175.rtf`/`176.rtf` calcium, `180.rtf` sodium) | STRONGLY_SUPPORTED |

Range prose is assembled at print time — e.g. "Male Up To 45 U/L", "Adult 3.8 - 5.2 g/dL"
(`ui-labels-verified.md` §6.5, from recorded frames, not a running binary).

## 4. Derived results

`constanttable.csv`: `CBCCONS` (`Hct = Hgb × 3.3`; `Hgb%` factors 6.25 male >12 y,
6.75 female >12 y, 7.5 for 1–12 y, 8.25 for <1 y) and `PTCONS` (12, 35, ISI 1.33,
Westgard token `2CVJW`). Exact numeric concordance with the manual is a VERIFIED fact from
`04-clinical-rules.md` §1. The `pt` profile table carries `pt`, `concent`, `inr`, `ratios`;
`cbc` carries `hgb`, `hct`; a third constant group `RATIOS` is queried at VA `72F81C`.
The **derivation code path and the ISI formula are UNKNOWN** (`Business_Rules_V2.md`
V2-BR-037 limitations). QC is supported by the 33-row `PTC` control curve
(`04-clinical-rules.md` §2) and by `Reportqc`-type forms in the 66-DataReport inventory;
no independent QC workflow rule is evidenced.

## 5. Release gate and print audit

- `resultreview` is the explicit quality gate: the print path refuses unreviewed results
  with the literal `Not found any Verified selected tests or result to print it`
  (`Business_Rules_V2.md` V2-BR-038; `13-pdf-print-architecture.md` §4).
- `give` is the acknowledgement queue (`6D4848`, etc.); `resultfinsh='False'` is the
  result-entry queue (`6843F2`, etc.); `resultprint` is the printed queue.
- Print audit is per operator, dated and sequenced: `printuser`, `printdate`, `printnum`
  (`681ED2`, `6B037C`, `6B2D70`, `6B6194`-family).
- `NatighU` is requested in DDL (`ALTER Table patienttest ADD NatighU Bit`) but **absent**
  from the shipped database — code-present, schema-absent (`Business_Rules_V2.md`
  V2-BR-005; `Database_Business_Logic.md` §8). The executable nevertheless SELECTs it at
  VA `686EC2`; the runtime failure mode is UNVERIFIED.

## 6. Prior knowledge preserved and revalidated

| Prior claim (source) | Revalidation | Status |
|---|---|---|
| Four result flags with edit/prev/print audit pairs (original stub; `MASTER_BUSINESS_LOGIC.md` §4) | VAs `681ED2`, `6860BE`, `686312`, `6865A6` | VERIFIED |
| States entered → validated → printed → finished (original stub) | Flags exist; strict ordering NOT evidenced | CONFLICTED as a *sequence*; flags VERIFIED |
| Range validation by sex/age (`04-clinical-rules.md` §3) | `normalrange.csv` columns + joins | VERIFIED |
| Re-issue/re-print supported (`Report* 's'` variants) | 117 template identities base+`_S`; `printnum` | STRONGLY_SUPPORTED |
| 0 INSERT/0 UPDATE; writes via bound recordsets | verb census of 1,797 statements | VERIFIED negative |
| No runtime observation | static-only scope in all cited reports | VERIFIED |

## 7. Unknowns

Boolean encoding of flags; flag reset/recheck behaviour; exact trigger for
`patientinfo.completes`; comparison code path for low/high limits; multi-component result
write path; per-invoice or per-test validation in the UI; any dynamic behaviour. All
UNVERIFIED / UNKNOWN.

**Cross-references:** `Simple_And_Composite_Tests.md`, `Printing_And_Reporting.md`,
`Laboratory_Business_Rules.md`, `09_CROSS_SOURCE_EVIDENCE/Verified_Findings.md`.
