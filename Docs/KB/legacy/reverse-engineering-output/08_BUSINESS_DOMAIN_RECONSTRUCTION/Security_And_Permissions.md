# Security and Permissions — REAL LAB SYSTEM v1.605 (V-CD-2019)

**Scope.** Operator identity, authorisation data, credential handling and audit, as
recoverable statically. No credential value is reproduced; no system was contacted or
executed. Prior finding identifiers are preserved from `unified-findings-register.md`.

**Primary sources.** `v2019-Patients.mdb.sql` (`CREATE TABLE [patients]`),
`statements__[analysis_BATCH-04_sql_v1605].csv`, `tables__[analysis_BATCH-04_sql_v1605].csv`,
`patients.csv` (header), `unified-findings-register.md`, `unified-unresolved-questions.csv`,
`security-and-technical-risk-register.md`, `Business_Rules_V2.md`,
`Database_Business_Logic.md` §6, `1605-phase2-findings.md`.

## 1. The operator table (misnamed `patients`) — VERIFIED

`patients.csv` header and `v2019-Patients.mdb.sql` agree on these columns:

| Group | Columns | Confidence |
|---|---|---|
| Identity | `patientusid` Long, `patientusname` Text(40), `patientword` Text(40), `patientuscap` Text(40), `lastvisit` Text(40) | VERIFIED |
| Flags | `patientad` Boolean (admin), `patientus` Boolean (active) | VERIFIED |
| Permissions | `grant1`…`grant12` Boolean (shipped sample header); **DDL adds `grant13`…`grant17`** (VAs `6DC31C`–`6DC64C`, both `Bit` and `YESNO` dialect pairs) → **17 in v1.605** | VERIFIED |
| Shift/attendance | `startwork` Text(15), `endwork` Text(15), `breakyorn` Boolean, `breaktime` Text(15), `tipnum` Long, `showtip` Boolean | VERIFIED |
| Branch/role | `branche` Integer, `SecWord` Text(10), `khasmlimit` Long | VERIFIED |
| ADD (DDL only) | `gtype`, `HDD1`, `HDD2` per `Functional_Specification_V2.md` F-V2-002 | STRONGLY_SUPPORTED (prior work) |

Login/attendance query: `select startwork,endwork,breakyorn,breaktime from patients where
patientusname= '` (VA `6D6F18`; table inventory: 14 statements). Admin enumeration:
`SELECT * FROM patients where patientad=True` (VAs `6EF0FC`, `6EF184`). Deletion:
`Delete FROM patients where patientusid=` (VA `6EF2AC`) — part of the manual cascade.

**Critical distinction preserved:** `patients` is the **operator** table; there is no
patient master table (`Data_Model_and_Workflow_Entities_V2.md` V2-ENT-003). Grant bits
live on the operator, not on the test row — a correction recorded in
`Database_Business_Logic.md` §11.

## 2. What the grant bits mean — UNKNOWN

- `unified-unresolved-questions.csv` **UNR-002**: names and Boolean/YESNO types are facts;
  the meaning of each `grantN` is not. The pricing association is only an INFERENCE from
  adjacent `discount` / `discount100` / `Discount=` literals.
- `unified-unresolved-questions.csv` **UNR-003**: seven `IDPassword*` identifiers and the
  "Digits only" prompt are established; which report section each unlocks, and whether
  the check is server-side, were not determined statically. `1605-phase2-findings.md` U-07
  confirms no credential token exists in MSDBRPTR.DLL, RL_SETUP.exe or the 228 templates.
- `unified-findings-register.md` **U-011**: "17 boolean `grantN` columns … and seven shared
  `IDPassword*` print secrets form a coarse, undocumented authorisation model."
  Confidence in the *model shape*: STRONGLY_SUPPORTED; in the *semantics*: UNVERIFIED.
- Form gating by grant flags is INFERRED only (`MASTER_BUSINESS_LOGIC.md` §7).

## 3. Credential handling — prior findings, revalidated

| Unified finding | Subject | Primary corroboration in this corpus | Confidence |
|---|---|---|---|
| U-001 | Vendor manual documents a default operator account/password pair (literal values withheld per no-secrets rule) | Manual topic 2 (B4); `security-and-technical-risk-register.md` §2 | STRONGLY_SUPPORTED (prior doc); operational use UNVERIFIED |
| U-003 | Hardcoded remote patient-DB credential in the e-mail component | B1 FND-003; prior report | STRONGLY_SUPPORTED (prior work) |
| U-004 / U-037 | SQL Server `sa` credential, cleartext, assembled from literals at run time | `Setup.bat`/`Setup4.bat` `SAPWD=` argument; `RL_SETUP.exe` string sweep (NEW-8) | STRONGLY_SUPPORTED (value redacted) |
| U-005 | Bulk-deletion maintenance tools targeting every clinical table | **72 `DELETE FROM` across 64 tables** in this corpus | VERIFIED for the statements; intent as "maintenance tool" prior work |
| U-018 | 41 of 62 PE binaries unsigned | Prior B2 finding; no binaries executed here | STRONGLY_SUPPORTED (prior work) |
| U-016 | SMS gateway credential in a plain-HTTP URL | `http://smsmisr.com/api/send.aspx?username=<accuntID>&password=<accuntpw>…` literal; `MyLabSetting.accuntpw` | VERIFIED from literal |
| — | Operator passwords stored in the table | `patients.patientword` Text(40), no hash/salt column; direct SELECTs | VERIFIED (structure); hashing before write UNVERIFIED (UNR-005) |
| — | No ASLR / no DEP | `DllCharacteristics 0x0000` (U-007 / U-015) | STRONGLY_SUPPORTED (prior work) |

The register consolidates **66 original findings into 41 unified subjects**, of which
**9 are High** (U-001…U-006, U-016, U-018, U-037). These numbers are from
`unified-findings-register.md` §1–§2 and are preserved, not recomputed here.

## 4. Audit and logging

- Per-test audit pairs are on `patienttest`: `edituser`/`editdate`, `prevuser`/`prevdate`,
  `printuser`/`printdate`, `taslimuser`/`taslimdate` (VAs `6860BE`, `686312`, `6865A6`,
  `6867AA`). See `Results_Management.md` §2.
- A general log table exists: `SELECT * FROM loger where lognum=` (VA `66F746`);
  `History` is a 25-column free-form side table. Confidence: VERIFIED for existence;
  retention policy and write triggers UNKNOWN.
- Print audit includes `printnum` sequence numbers (`13-pdf-print-architecture.md` §4).
- `patientinfo.edituser2`, `.editdate2`, `.editdate`, `.editnum` provide header-level
  edit tracking (DDL) — VERIFIED for columns.

## 5. Branch / single-site tension

`patients.branche` Integer exists in the schema and is referenced as an attribute in
prior work (`V2-BR-008`). The v1.605 SQL corpus **directly confirms** the legacy branch
extraction: 18 statements alias `SUBSTRING(patientid,2,1) + SUBSTRING(patientid,9,1) as
bran` (VAs `72714C`, `727AA0`, `7283A4`, `729578`, `72A774`, `72BFC0`, `72C3C4`, …). The
present project rule (`Docs/KB/START-HERE.md` rule 8) is that the laboratory is a **single
site** in the successor design, with position 9 "formerly" a branch code now absorbed
into the middle segment. The two statements concern different targets: v1.605 legacy
behaviour is VERIFIED; the successor scoping is STRONGLY_SUPPORTED as an external design
note. No branch-based *permission* rule is asserted. `patientinfo.labid` Text(13) versus
`patients.branche` Integer remains a declared type mismatch (`Database_Business_Logic.md`
§6).

## 6. Runtime posture — why confidence is bounded

No binary was executed, no installer run, no database contacted, no credential validated.
`contradictions-limitations-and-unresolved-questions.md` §2 states: static only;
Ghidra headless analysis was OOM-killed (exit 137), so per-object decompilation was not
obtained. Consequently every statement about *enforcement* is INFERRED; statements about
*stored structure* and *literal presence* are VERIFIED/STRONGLY_SUPPORTED.

## 7. Prior knowledge preserved

| Prior stub claim | Revalidation | Status |
|---|---|---|
| `patients` table with `grant1..grant12`, `patientusname`, `patientword`, `patientuscap` | `patients.csv` header + DDL | VERIFIED; 17 grants in full DDL |
| Default operator credential pair (U-001) | Prior manual reports a default account/password for the operator login; the literal values are deliberately withheld here per the no-secrets rule | PRIOR-WORK CLAIM |
| Cleartext passwords (U-003) | No hash column; direct SELECT | STRONGLY_SUPPORTED, hash question UNVERIFIED |
| Per-section `IDPassword*` print passwords | UNR-003 | UNVERIFIED identifiers only |
| 41 consolidated security findings (9 High) | `unified-findings-register.md` | STRONGLY_SUPPORTED (prior register) |
| No ASLR/DEP; 41/62 unsigned | Prior PE analysis | STRONGLY_SUPPORTED (prior work) |

## 8. Unknowns

Meaning of each `grantN`; mapping of `IDPassword*` to report sections; whether
`SecWord`/`SecWord` gates anything; hashing of `accuntpw`/`patientword`; runtime permission
enforcement; branch semantics for v1.605. All UNVERIFIED / CONFLICTED / UNKNOWN as labelled.

**Cross-references:** `Printing_And_Reporting.md`, `Patient_Management.md`,
`09_CROSS_SOURCE_EVIDENCE/Confidence_Assessment.md`, `.../Conflicting_Findings.md`.
