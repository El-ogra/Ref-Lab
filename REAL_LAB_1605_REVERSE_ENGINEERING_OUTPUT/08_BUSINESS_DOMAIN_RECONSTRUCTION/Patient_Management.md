# Patient Management — REAL LAB SYSTEM v1.605 (V-CD-2019)

**Scope.** Registration and identity of the person/order as represented in the shipped
Access dump and the extracted SQL corpus of v1.605. No binary was executed; no patient
data are reproduced. All examples below are synthetic.

**Evidence basis (primary).** `v2019-Patients.mdb.sql` (CREATE TABLE `[patientinfo]`),
`statements__[analysis_BATCH-04_sql_v1605].csv` (1,797 statements),
`tables__[analysis_BATCH-04_sql_v1605].csv` (per-table statement inventory),
`patientinfo.csv` (column header/redacted sample), `payment.csv`, `patienttest.csv`.

**Confidence labels.** VERIFIED = direct primary artefact · STRONGLY_SUPPORTED =
primary structure + corroborating prior report · INFERRED = plausible reading, not
observed · UNVERIFIED / CONFLICTED / UNKNOWN = stated explicitly where applicable.

---

## 1. The registration entity

`patientinfo` is the only patient-facing demographic table. It has **39 columns** in
`v2019-Patients.mdb.sql`; the flat `patientinfo.csv` sample carries the same header with
values redacted. Column groups, all VERIFIED from the DDL:

| Group | Columns (exact names from `v2019-Patients.mdb.sql`) |
|---|---|
| Identity | `patientsefa` Text(20), `patientnamear` Text(40), `patientid` Text(13), `labid` Text(13), `kawmy` Text(14), `barcode` Text(25) |
| Demographics | `patientsex` Text(7), `patientage` Double, `patientageunit` Text(5), `PBirthDay` DateTime |
| Contact | `patientaddress` Text(70), `patientphone` Text(12), `patienttel` Text(12), `PFax` Text(15), `SMSMob` Text(15), `email` Text(40) |
| Referral | `referredbysefa` Text(20), `referredbyname` Text(40), `gehaName` Text(50), `PatRPW` Text(20) |
| Visit/timing | `requestdate` DateTime, `outdate` DateTime, `timein` Text(10) |
| Finance | `pricemethod` Text(10), `totalprice` Double, `totalpricedisc` Double, `elmadfoo3` Double, `elbakyma3mal` Double, `discount` Double, `discount100` Double, `mowzaf` Text(40) |
| Workflow/audit | `completes` Text(1), `recepnote` Text(100), `moreinfo` Text(22), `moreinfo2` Text(30), `edituser2` Text(40), `editdate2` Text(40), `editdate` Text(40), `editnum` Integer |

There is **no patient master table**. The table named `patients` holds *operators*
(`patientusname`, `patientword`, `grant1`…`grant17`), not patients — a naming trap
already recorded in `Data_Model_and_Workflow_Entities_V2.md` and
`Database_Business_Logic.md` §6. Confidence: VERIFIED (DDL + prior report).

## 2. The 13-character identifier

The identifier is an **order/visit identifier**, not a pure person key. Exact SQL
fragments from `statements__[analysis_BATCH-04_sql_v1605].csv`:

| VA | Fragment | What it establishes | Confidence |
|---|---|---|---|
| `68313E`, `6834FE`, `686C32`, `686D22`, `6A6EDA`, `6B7670` | `Select mid(patientid,10,3) as patid …` / `SUBSTRING(patientid, 10, 3) AS patid` | Positions 10–12 are surfaced as a display code `patid` on worklists | VERIFIED |
| `6F7ED8` | `SELECT patientid From patientinfo WHERE (MID(patientid, 3, 2) ='` | Positions 3–4 are used as a person-level filter | VERIFIED |
| `72714C`, `727AA0`, `7283A4`, `729578`, `72A774`, `72BFC0`, `72C3C4` | `SUBSTRING(patientid,2,1) + SUBSTRING(patientid,9,1) as bran` | Position 9 is extracted as `bran` in 18 statements (SQL-Server and Access dialect pairs) | VERIFIED |
| `6843F2`, `687B4E`, `688256` | `INNER JOIN patienttest ON patienttest.patientid = patientinfo.patientid` | The same field links demographics to ordered tests | VERIFIED |
| — | `patientinfo.labid` Text(13) | A separate lab/site label coexists with the positional scheme | VERIFIED (DDL) |

**Prior-work reconciliation (important).** `Business_Rules_V2.md` V2-BR-008 and V2-BR-040
state that position 9 of the 13-character identifier is the **branch**, and that the value
comes from the operator's `patients.branche`. This is **revalidated directly** in the
v1.605 SQL corpus: **18 statements** extract it as
`SUBSTRING(patientid,2,1) + SUBSTRING(patientid,9,1) as bran` (and the `MID` twin), e.g.
VAs `72714C`, `727AA0`, `7283A4`, `728C98`, `729578`, `72A774`, `72BFC0`, `72C3C4`. The
same statements use positions 5–2 or 7–2 as a period code (`mmmd`) and positions 3–4 as a
person/count segment. The current project rule (`Docs/KB/START-HERE.md` rule 8; restated in
`MASTER_BUSINESS_LOGIC.md` §1 and `State_Transitions.md` §3) says position 9 **formerly**
encoded a branch and is now part of the middle segment — that is a statement about the
*successor design*, not a correction of the v1.605 evidence. Therefore: position 9 = branch
in v1.605 is VERIFIED; the successor-system repositioning is a separate design decision.
See `09_CROSS_SOURCE_EVIDENCE/Conflicting_Findings.md`.

Synthetic illustration of field *length only* (no real data): `9999999999999` is a
13-character string matching the declared `Text (13)` width.

## 3. Registration and barcode pathway

`addpatient`-type forms are named in the earlier stub and in the functional documentation
(`Functional_Specification_V2.md`, `Form` inventory in `Version_Identity_and_Scope.md` §3.2:
132 forms in v1.605). The **write mechanism** is not literal SQL: the extracted corpus has
**0 `INSERT INTO` and 0 `UPDATE … SET`** across 1,797 statements (verb distribution:
SELECT 1,381 · ALTER 241 · DELETE FROM 72 · OTHER 49 · CREATE 27 · SHAPE 27). Registration
therefore proceeds through bound recordsets, not text statements — STRONGLY_SUPPORTED
architectural finding, also in `State_Transitions.md` §4.

Barcode handling: `patientinfo.barcode` Text(25) plus `testtable.barcode1/barcode2/barcode3`;
`ui-labels-verified.md` §5 records the observed controls `Print File Barcode`,
`Print Lab Id Barcode`, `Print All Barcode` and `Print File Barcode with all` in a recorded
walkthrough. The `printwith` flag on `patienttest` is the per-test barcode/print selector
(VA `681ED2`). Confidence: VERIFIED for columns and flags; INFERRED for the exact UI binding.

## 4. Worklists, completion and deletion

- `patientinfo.completes` Text(1) is selected alongside identity on request-date worklists
  (VAs `682F56`, `68313E`, `683C72`, `686C32`, `686D22`) and in `INNER JOIN patienttest`
  queries (VAs `6843F2`, `687B4E`). Confidence: VERIFIED for data flow; the UI meaning of
  `completes` is INFERRED.
- `tables__[analysis_BATCH-04_sql_v1605].csv` attributes **428 statements to `patientinfo`
  (410 SELECT, 17 SHAPE)** — the highest of any table. This is the authoritative source of
  the "patientinfo 428" figure in the handoff. A raw substring recount of the same CSV
  returns a different number; the difference is counting methodology, not data.
- Deletion is an **application-level manual cascade**: 72 `DELETE FROM` statements target
  64 distinct tables, including `Delete FROM patientinfo where patientid='` in the
  contiguous block around VAs `6A740A`–`6A760E`. No foreign key with `ON DELETE CASCADE` is
  declared (`MSysRelationships` empty — `1605-phase2-findings.md` K-02). Confidence:
  VERIFIED for the statements; INFERRED that the block is one atomic transaction.

## 5. Prior knowledge preserved and revalidated

| Prior claim (source) | Revalidation against primary dumps | Status |
|---|---|---|
| 13-char `patientid`, worklist alias `patid` (`MASTER_BUSINESS_LOGIC.md` §1) | DDL Text(13) + VAs `68313E`/`6A6EDA` | VERIFIED |
| Position 9 branch (`Business_Rules_V2.md` V2-BR-008) | 18 v1.605 SQL statements alias `... SUBSTRING(patientid,9,1) as bran` | VERIFIED for v1.605 |
| Position 9 absorbed into middle segment (`START-HERE.md` rule 8) | Successor-design note; not a claim about v1.605 | STRONGLY_SUPPORTED as external scope note |
| `patients` is the operator table (`Data_Model_and_Workflow_Entities_V2.md`) | `CREATE TABLE [patients]` with `grant1..grant17` | VERIFIED |
| No patient master / duplicate detection unknown (`MASTER_BUSINESS_LOGIC.md` §1) | No such table in `v2019-Patients.mdb.sql` | VERIFIED absence; algorithm UNKNOWN |
| Addpatient form + barcode identity (original stub) | 132 forms in `Version_Identity_and_Scope.md` §3.2; barcode columns in DDL | STRONGLY_SUPPORTED |

## 6. Explicitly unknown

ID-allocation algorithm; duplicate-detection rule; meaning of `completes` values;
whether `labid` and the positional branch segment ever agree; `pricemethod` value domain
(UNR-002-adjacent, unresolved); any runtime observation of registration. None is asserted.

**Cross-references:** `Visits_And_Orders.md`, `Results_Management.md`,
`09_CROSS_SOURCE_EVIDENCE/Evidence_Catalog.csv`, `.../Conflicting_Findings.md`.
