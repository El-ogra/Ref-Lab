# Stored Procedures, Triggers and Queries — REAL LAB SYSTEM v1.605

## 0. Method

Keyword census over every available schema dump and DDL/SQL corpus:
`v2019-Patients.mdb.sql`, `v1-Patients.mdb.sql`, `schema-Branch_Data.sql`,
`v1-data.mdb.sql`, `v2019-data.mdb.sql`, `ddl-all.txt`,
`ddl-evidence__[analysis_BATCH-04_sql_v1605].txt`,
`statements__[analysis_BATCH-04_sql_v1605].csv` (1,797 statements). No database was
contacted. All SQL quoted below is a literal recovered from the executable or a schema
dump, not a live query result.

## 1. Microsoft Access / Jet 4 (`Patients.mdb`)

| Object class | Finding | Confidence |
|---|---|---|
| Stored procedures | **NOT APPLICABLE to Jet 4.** Jet 4 has no stored-procedure object. Confirmed by 0 occurrences of `CREATE PROCEDURE` in all schema dumps and DDL files. | VERIFIED |
| Triggers | **NOT APPLICABLE to Jet 4.** Jet 4 has no trigger object. 0 `CREATE TRIGGER` occurrences. No `ON INSERT/UPDATE/DELETE` trigger syntax in any dump. | VERIFIED |
| Stored queries (Access `QueryDef` objects) | **UNKNOWN / NOT EXPORTED.** `mdb-tables`/`mdb-schema` (the tools that produced `v2019-Patients.mdb.sql`) emit only `CREATE TABLE`. The dump contains exactly 103 `CREATE TABLE` and no `CREATE VIEW`. Stored Access queries cannot be confirmed or excluded from the available evidence. | UNKNOWN |
| VBA / event procedures inside the MDB | Not evidenced — only the VB6 executable's SQL literals are available. | UNKNOWN |

`v2019-Patients.mdb.sql` is a pure DDL table dump: 103 `CREATE TABLE` blocks, 0
`CREATE PROCEDURE`, 0 `CREATE VIEW`, 0 `CREATE TRIGGER`, 0 `CREATE INDEX`. The same is
true of `schema-Branch_Data.sql` (49 `CREATE TABLE`) and the `data.mdb` dumps (3 each).

### 1.1 The 1,797 SQL statements are executable literals, not stored DB objects

`statements__[analysis_BATCH-04_sql_v1605].csv` supplies a **VA address** for every
statement (e.g. `66FCB2`, `66FD56`, `672722`, `6CD858`). That is a virtual address in the
`real lab system.exe` image, proving these are embedded VB6 string literals recovered
by static analysis — **not** Access `QueryDef` objects retrieved from the `.mdb`. Verb
distribution (recomputed from that CSV):

| Verb | Statements |
|---|---:|
| SELECT | 1,381 |
| DELETE FROM | 72 |
| ALTER TABLE | 241 |
| CREATE TABLE | 27 |
| SHAPE | 27 |
| OTHER (incl. `EXEC …`) | 49 |
| **Total** | **1,797** |

`tables__[analysis_BATCH-04_sql_v1605].csv` and `columns__[analysis_BATCH-04_sql_v1605].csv`
are statement-usage indexes over this same corpus, not database catalogue exports.

### 1.2 `SHAPE … APPEND/RELATE` as the closest analogue to a stored query plan

27 `SHAPE` statements build hierarchical recordsets through the `MSDataShape` outer
provider (configured by `data_connection1.udl`, per `Business_Rules_V2.md`). Example,
verbatim from `statements__[analysis_BATCH-04_sql_v1605].csv` VA `672722`:

```sql
SHAPE {SELECT subgrouptable.subgroupcode, subgrouptable.subgroupname FROM subgrouptable
 GROUP BY subgrouptable.subgroupcode, subgrouptable.subgroupname order by subgroupname}
 AS cmdbill
APPEND ({SELECT * FROM testtable where testorno= 'True' and testsimp <> '' order by testsimp}
 AS cmdbill_d RELATE subgroupcode TO subgroupcode) AS cmdbill_d
```

and VA `672E3E` uses `RELATE testcode TO testcode` to append `normalrange` rows under
`testtable`. These are **client-side shape definitions**; `database-engineering-reference.md`
§1 states that the shape vocabulary lives in the report templates and that "the report
subsystem and the data-access subsystem are one thing in this product". They are not
stored in the database.

## 2. Microsoft SQL Server (`patients` catalogue)

| Object class | Finding | Confidence |
|---|---|---|
| User-defined stored procedures | **No `CREATE PROCEDURE` in any recovered literal.** No user-defined procedure is evidenced. | VERIFIED (absence in corpus) |
| Triggers | **No `CREATE TRIGGER` in any recovered literal.** Not evidenced. | VERIFIED (absence in corpus) |
| System stored procedures invoked from VB6 | **EVIDENCED:** `EXEC sp_addumpdevice 'disk', '` at VA `66FCB2` and `EXEC sp_dropdevice '` at VA `66FD56`, both verb `OTHER` in `statements__[analysis_BATCH-04_sql_v1605].csv`. These are SQL Server **system** procedures for backup-device management, invoked by the client — not procedures created by the application. | VERIFIED |
| Owning module | `Complete_Reverse_Engineering_Reference_V1.md` §2.5 identifies the VB6 `labdb` module (20 methods) as "Database maintenance — backup, restore, …", consistent with the `sp_addumpdevice`/`sp_dropdevice` literals. | STRONGLY_SUPPORTED |
| Views | No `CREATE VIEW` in any recovered literal. Not evidenced. | VERIFIED (absence in corpus) |

## 3. Reconciliation and conflict notes

- `database-engineering-reference.md` §3 and `database-and-data-access-analysis.md` §3
  report **14 `CREATE TABLE` / 120 `ALTER TABLE`** statements, whereas
  `ddl-evidence__[analysis_BATCH-04_sql_v1605].txt` contains **27 / 241**. These are
  different extraction passes over different executable corpora (`statements.csv`,
  1,337 statements, vs `statements__[analysis_BATCH-04_sql_v1605].csv`, 1,797 statements).
  Both agree that the DDL is **client-embedded mutation of the schema**, not a stored
  database procedure.
- `Version_Identity_and_Scope.md` §3.5: of the 268 embedded DDL changes, **61 are absent
  from the shipped database — 23 are SQL-Server-only syntax that cannot apply to Jet 4
  and 38 are Access-applicable but were never executed.** This is why, for example,
  `patienttest.natighu` is DDL-defined but absent from the shipped table
  (`Data_Model_and_Workflow_Entities_V2.md` §3.2).
- No transaction-control statements (`BEGIN TRANSACTION`, `COMMIT`, `ROLLBACK`) were
  found in the schema or DDL evidence; transaction behaviour is ADO-level, not
  database-level. **INFERRED** from `MSDataShape`/ADO usage; the statement corpus was not
  exhaustively searched for these tokens beyond the sources named above.

## 4. Verdict

- **Access database:** stored procedures and triggers — **NOT APPLICABLE** (Jet 4 has no
  such objects); stored queries — **UNKNOWN** (not exported by the available tooling).
- **SQL Server database:** user procedures, triggers and views — **not evidenced**;
  the only procedure references are the two system calls `sp_addumpdevice` /
  `sp_dropdevice` used for backup device management from the VB6 client.
- **Business logic location:** overwhelmingly in the VB6 application and in embedded
  SQL strings; the database layer contributes column-level flags and denormalised
  snapshots rather than executable objects (see `Database_Business_Logic.md`).
