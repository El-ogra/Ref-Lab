# Constraints and Indexes — REAL LAB SYSTEM v1.605

## 0. Method

Keyword census and column-level parse of `v2019-Patients.mdb.sql` (103 tables / 1,682
columns), `v1-Patients.mdb.sql`, `schema-Branch_Data.sql`, the `data.mdb` dumps,
`ddl-all.txt` and `ddl-evidence__[analysis_BATCH-04_sql_v1605].txt`
(268 DDL literals). No database was contacted.

## 1. Declared constraints — census result

| Constraint class | Occurrences in all schema dumps + DDL evidence | Confidence |
|---|---:|---|
| `PRIMARY KEY` | **0** | VERIFIED |
| `FOREIGN KEY` / `REFERENCES` | **0** | VERIFIED |
| `UNIQUE` | **0** | VERIFIED |
| `CHECK` | **0** (the single `CHECK` string in `v2019-Patients.mdb.sql` is the mdbtools licence comment "Check out http://mdbtools.sourceforge.net") | VERIFIED |
| `DEFAULT` | **0** | VERIFIED |
| `CREATE INDEX` / `CREATE UNIQUE INDEX` | **0** | VERIFIED |
| `NOT NULL` | **132** in `v2019-Patients.mdb.sql` | VERIFIED |
| `ON DELETE` / `ON UPDATE` | **0** | VERIFIED |
| `VALIDATION RULE` (Access field-level) | not represented in mdbtools DDL output | UNKNOWN |

**Conclusion:** the shipped database declares no keys, no referential integrity, no
validation rules and no indexes in its extracted DDL. Whatever integrity exists is
implemented in the VB6 application and in the column-level `NOT NULL` declarations.

## 2. `NOT NULL` — the only declared column constraint

132 of 1,682 shipped columns carry `NOT NULL` (parsed from
`v2019-Patients.mdb.sql`). Representative verbatim declarations:

```sql
CREATE TABLE [ABO] (
  [patientid]    Text (13) NOT NULL,
  ...
);
CREATE TABLE [Abuse] (
  [patientid]    Text (13) NOT NULL,
  ...
);
CREATE TABLE [antibio] (
  [antisymbol]   Text (20) NOT NULL,
  ...
);
CREATE TABLE [barcodes] (
  [barcode]      Text (50) NOT NULL,
  ...
);
```

Pattern: `patientid Text (13) NOT NULL` on the per-test profile tables,
`antisymbol NOT NULL` on the antibiotic dictionary, `barcode NOT NULL` on barcode
allocation. Full per-column detail is in `Tables_And_Columns.md`.

## 3. Candidate keys (INFERRED — not declared)

| Table | Candidate key | Basis | Confidence |
|---|---|---|---|
| `patientinfo` | `patientid` | `Text (13) NOT NULL` on profile tables; 273 references in `columns.csv`; 60 explicit joins in `statements__[analysis_BATCH-04_sql_v1605].csv` | STRONGLY_SUPPORTED (logical key), INFERRED (declared PK) |
| `patienttest` | `patientid` (+ `requestdate` suspected) | 216 statements in `tables.csv`; joins use `patientid` only | STRONGLY_SUPPORTED / composite INFERRED |
| `testtable` | `testcode` | 609 rows; `normalrange.testcode = testtable.testcode` joined 13× | STRONGLY_SUPPORTED |
| `normalrange` | `normalid` | 18-column table with `normalid` first column | INFERRED |
| `patients` | `patientusid` | login queries use `patientusname`/`usersnum`; `patientusid` Long first column | INFERRED |
| `Labbranch` | `branchid` | 2-column master with `branchid` Long / `branchname` Text (`Data_Model_and_Workflow_Entities_V2.md` §3.4 — the report labels it PK, but the dump has no PK clause) | STRONGLY_SUPPORTED (logical), INFERRED (declared) |
| `constanttable` | `constantdiscrp` | 2 rows `PTCONS`, `CBCCONS` in `constanttable.csv` | STRONGLY_SUPPORTED |

## 4. Default values and domain constraints

- **No `DEFAULT` clause** in any dump or DDL literal. Application-supplied defaults are
  the rule (e.g. VB6 form pre-population); they are not database-enforced.
- **No `CHECK` / validation rule** is exportable from these dumps. Field-level Access
  `ValidationRule` / `ValidationText` properties are not represented by mdbtools, so
  their absence cannot be proven — **UNKNOWN**.
- Boolean/YESNO columns (`80 Boolean` in the shipped dump; 7 `YESNO` tokens in the DDL
  evidence) provide a two-valued domain by type alone, not by a check constraint.
  `testtable.active`, `testtable.deleteprotect`, `testtable.inotherlab`,
  `patienttest.give`, `patienttest.sentout`, `patienttest.send`, `patients.grant1`…
  `grant17` are examples (`v2019-Patients.mdb.sql`; `testtable-stats.json`).

## 5. Indexes

No `CREATE INDEX` statement exists in any evidence file. Jet can create implicit indexes
for relationships, but since no relationships are declared in the dump there is no
index evidence to recover. **Index state: UNKNOWN beyond "none declared".**
`columns.csv` (reference frequencies such as `patientinfo.patientid` 273,
`patientinfo.patientnamear` 233, `patientinfo.labid` 198) shows which columns are hot in
the application, but is **not** an index catalogue.

## 6. Jet 4 vs SQL Server syntax — the 23 inapplicable statements

`Version_Identity_and_Scope.md` §3.5 states that of the **268** embedded DDL literals,
**61 did not reach the shipped database: 23 are SQL-Server-only syntax that cannot apply
to Jet 4, and 38 are Access-applicable but were never executed.** The same figure appears
in `00-baseline-summary.md` (§"DDL: 61 changes not present — 23 SQL-Server, 38 Access").

Syntax classes that cannot apply to Jet 4, with verbatim examples from
`ddl-evidence__[analysis_BATCH-04_sql_v1605].txt`:

| SQL Server token | Jet 4 equivalent | Verbatim DDL evidence (VA) |
|---|---|---|
| `bigint IDENTITY(1,1) NOT NULL` | `AUTOINCREMENT(1,1)` / `COUNTER` | `Create Table smsgroups (ID bigint IDENTITY(1,1) NOT NULL, …)` `6755EA`; `Create Table smslist (…)` `6772BA`; `Create Table SMStemplate (templatenum bigint IDENTITY(1,1) NOT NULL, …)` `6773DA`; `Create Table SMSSender (SMSautonum bigint IDENTITY(1,1) NOT NULL, …)` `6774AE`; `ALTER Table payment ADD paymentnum bigint IDENTITY(1,1) NOT NULL` `677EC2`; `ALTER Table PTC ADD ConstID bigint IDENTITY(1,1) NOT NULL` `677FA6` |
| `varchar(MAX)` | `MEMO` | `Create Table LBC (… RESULT01 varchar(MAX) …)` `6756BE`; `ALTER Table normalrange ALTER COLUMN normalrang varchar(MAX)` `677D3E`; `ALTER Table wasl ADD normalrang varchar(MAX)` `678436` / `681382`; `ALTER Table pregnancy ADD normalrange3 varchar(MAX)` `67B73E` |
| `nvarchar` | `TEXT` | `ALTER Table Proelectrophoresis ADD Beta2 nvarchar(10)` `677892`; `… ADD immunolgo nvarchar(10)` `677902`; `ALTER Table testtable ADD testcodebill nvarchar(13)` `678036`; `ALTER Table testtable ADD REFRANGE nvarchar(1)` `67AB12` |
| `smalldatetime` | `DATETIME` | `Create Table IgEallergy (… requestdate smalldatetime …)` `6759F6`; `Create Table SMSSender (… requestdate smalldatetime …)` `6774AE`; `Create Table LBC (… requestdate smalldatetime …)` `6756BE` |
| `Bit` | `YESNO` | `ALTER Table patienttest ADD NatighU Bit` `67764E`; `ALTER Table Patients ADD grant13 Bit` `6DC31C`; `grant14`…`grant17` `6DC36C`–`6DC460`; `ALTER Table prostaticx ADD crystalpt bit` `67B006` / `67E12E` |
| `int` / `bigint` | `INTEGER` / `LONG INTEGER` | `ALTER Table Gehats ADD ReferredID int` `6779DA` |

The DDL corpus also contains **paired Jet-adaptable forms** (6 `AUTOINCREMENT`, 7 `YESNO`,
10 `MEMO` tokens), e.g. `ALTER Table PTC ADD ConstID AUTOINCREMENT(1, 1)` and
`ALTER Table Patients ADD COLUMN grant14 YESNO`. That pairing, plus the documented
23/38 split, explains why some migration changes *are* present in the shipped schema
(`testcodebill`, `branche`, `grant13`…`grant17`) while others are not (e.g.
`patienttest.natighu` — `Data_Model_and_Workflow_Entities_V2.md` §3.2).

`ALTER COLUMN <type>` itself is a SQL Server form; Jet 4's `ALTER COLUMN` does not support
the same type-change semantics (`Complete_Reverse_Engineering_Reference_V2.md` §6.9,
"Migration DDL cannot apply to Jet — silently inapplicable").

## 7. Conflict note

`Business_Rules_V2.md` rule V2-BR-024 ("The migration DDL is SQL Server syntax and cannot
execute against Jet 4", CONFIRMED) agrees with §6. However,
`database-engineering-reference.md` §3.2 counts 120 ALTER statements while
`ddl-evidence__[analysis_BATCH-04_sql_v1605].txt` has 241; the 23-statement figure is
attached to the larger corpus in `Version_Identity_and_Scope.md` §3.5. Both corpora agree
on the syntax classes and on the "silently inapplicable" behaviour.
