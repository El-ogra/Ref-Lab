# Database Transactions and Multi-Statement Workflows — REAL LAB SYSTEM v1.605

**Scope.** Transaction boundaries *as evidenced by SQL text and adjacent tooling*.
Static evidence only; no database was contacted, no executable or batch file was run.

**Confidence legend.** VERIFIED / STRONGLY_SUPPORTED / INFERRED / UNVERIFIED /
CONFLICTED / UNKNOWN as defined in `Application_Data_Access_Architecture.md`.

---

## 1. Transaction-control statements: negative result (VERIFIED)

A case-sensitive keyword sweep over all 1,797 `text` fields of
`statements__[analysis_BATCH-04_sql_v1605].csv` produced:

| Keyword | Rows containing it |
| --- | ---: |
| `BEGIN TRAN` | 0 |
| `TRANSACTION` | 0 |
| `COMMIT` | 0 |
| `ROLLBACK` | 0 |
| `INSERT` (statement prefix) | 0 |
| `UPDATE` (statement prefix) | 0 |
| `EXEC ` | 2 |
| `sp_` | 2 |

Therefore **no SQL-level transaction boundary is evidenced**. Any claim that the
application opens an ADO `Connection.BeginTrans` / `CommitTrans` / `RollbackTrans`
scope is **INFERRED only** — no such call is present in the extracted inventory,
and no method body was decompiled.

The only two `EXEC` literals are backup-device management:

| VA | SQL text | Evidence |
| --- | --- | --- |
| `66FCB2` | `EXEC sp_addumpdevice 'disk', '` | statement CSV, verb `OTHER`, length 30 |
| `66FD56` | `EXEC sp_dropdevice '` | statement CSV, verb `OTHER`, length 20 |

`dependency-inventory.json` → `external_services[4]` independently attributes these
to "SQL Server backup devices" used by `RLS SQL Database Tools.exe`. Status:
STRONGLY_SUPPORTED.

---

## 2. Evidenced multi-statement workflows

### 2.1 Patient-deletion cascade (STRONGLY_SUPPORTED)

72 rows have verb `DELETE FROM`. Of these, a contiguous block at VAs
`6A740A`–`6A84EE` contains ~50 statements of the form
`Delete FROM <table> where patientid='`, covering:

`patientinfo`, `payment`, `patienttest`, `ABO`, `Abuse`, `BILH`, `cardiac`, `CBC`,
`CoagFactors`, `CRP`, `cultureanti`, `cultures`, `Drugs`, `Electrolytes`, `ESR`,
`Fertlity`, `iron`, `hepatitis`, `lipids`, `IMMUNOLOGY`, `liver`, `kidney`,
`hormones`, `Immunoglobulin`, `Pancreas`, `pregnancy`, `pt`, `stool`, `semen`,
`Sugar`, `Suprarenal`, `TORCH`, `trace`, `Tumors`, `Urine`, `widal`, `BloodGases`,
`Vitamins`, `Hgbectrophoresis`, `Proelectrophoresis`, `crossmat`,
`GLUCOSETOLERANCE`, `stone`, `prostaticx`, `CSFT`, `TUBERCULINT`, `ZN`, `BONEMAR`,
`postcoitalt`, `renalfailure`, `anyreport`.

The contiguous VA layout and the uniform predicate are strong evidence of a single
cascade routine. **Whether it executes inside one transaction is UNKNOWN** — no
`BEGIN TRANSACTION` literal exists. A partial failure could therefore leave
orphaned result rows (INFERRED risk, not an observed defect).

Related deletes:

| VA | SQL | Likely purpose (INFERRED) |
| --- | --- | --- |
| `6A868A` | `Delete FROM testtemp` | clear temp results |
| `6B7B84` | `Delete FROM testname` | catalogue maintenance |
| `6BCBB8` | `Delete FROM cultureanti where culutrenum=` | remove one culture/antibiogram |
| `6D70E4` | `Delete FROM tasgel WHERE tasgeldate >= '` | installment/attendance purge |
| `6DEB40` | `DELETE FROM history` | interpretive-history purge |
| `6EF2AC` | `Delete FROM patients where patientusid=` | remove operator |
| `6F6360` / `6F6C44` | `DELETE FROM payment where patientid= '` / `Delete FROM payment where paymentnum=` | receipt voiding |
| `6F7238` | `Delete FROM seana where instrumentid=` | instrument configuration |
| `70952C` | `Delete from customtesttable where customgroupcode=` | custom panel maintenance |
| `7222A0` | `Delete FROM sharekat where sherkaid=` | company/account maintenance |
| `723C64` / `724EF8` | `Delete FROM patienttotal` / `…where patientid= '` | totals maintenance |
| `72EFD0` | `Delete FROM pictures where patientid= '` | attachment cleanup |
| `73265C` / `7326D0` | `Delete FROM patienttest where testcode=` / `Delete FROM anyreport where testcode=` | remove a test from catalogue |
| `733468` | `Delete from samples where patientid= '` | sample cleanup |
| `72C568` | `Delete  loger where datein >= '` | log pruning |
| `7352CC`, `7353D4`, `7354FC` | `Delete FROM SMSSender` (+ predicates) | SMS queue maintenance |

### 2.2 Runtime DDL migration sequence (VERIFIED content; UNKNOWN ordering)

`ddl-evidence__[analysis_BATCH-04_sql_v1605].txt` holds 268 DDL-shaped literals
(241 `ALTER`, 27 `CREATE` in the CSV). The SQL Server family (VAs ~`67413A`–
`67B522`) and the Jet family (~`67B7AE`–`681456`, `6DC264`–`6DC714`) each form a
long sequential chain of `ALTER TABLE … ADD/ALTER COLUMN`. `database-and-data-access-analysis.md`
§3.2 states there is no migration script and no versioning table; the client
mutates the schema directly. **The trigger, guard conditions and execution order
are UNKNOWN** — it is not proven that every statement runs on every start.

### 2.3 Hierarchical SHAPE materialisation (VERIFIED syntax; INFERRED execution unit)

27 `SHAPE` statements combine a parent query and child query with
`APPEND (…) RELATE … TO …`, e.g. VA `672722` (`subgrouptable` parent → `testtable`
child) and VA `672E3E` (`testtable` parent → `normalrange` child). In ADO, the
Data Shape provider materialises these as a single hierarchical `Recordset`.
Treating that recordset as a consistent read snapshot is INFERRED from the ADO
model; no isolation-level configuration is evidenced.

### 2.4 SMS send path — database read becomes HTTP request (STRONGLY_SUPPORTED)

`database-and-data-access-analysis.md` §5: `Select * from MyLabSetting WHERE LabID=100 ;`
yields gateway settings which are interpolated into an `http://smsmisr.com/api/send.aspx?…`
URL and requested by driving Internet Explorer through COM. No credential values
are reproduced here. This is a cross-boundary workflow with **no transactional
relationship** to the clinical database write (INFERRED).

---

## 3. Data-transport and backup tooling

| Tool / script | Evidence | Evidenced role |
| --- | --- | --- |
| `RLS SQL Database Tools.exe` (114,688 B) | file present in `D:\Collective work`; `dependency-inventory.json` `external_services[4]` | SQL backup-device management; the two `EXEC sp_addumpdevice` / `sp_dropdevice` literals |
| `RLS ACCESS Database Tools.exe` (1,531,904 B) | file present; install media §3 lists `RealLab support.exe`, `RLUTool.exe` alongside | Access mirror maintenance; exact operations UNVERIFIED (binaries not executed) |
| `Patient Data Transport.exe` (1,294,336 B, SHA-256 in `Patient Data Transport.exe.json`) | file present; `dependency-inventory.json` lists it as VB6 runtime consumer | patient data transfer; protocol and transaction semantics UNVERIFIED |
| `D:\Lab_data\Attach_Patients.bat`, `Attach_Interface.bat`, `Attach_VIDAS.bat` | `08-deployment-installer.md` §2.1, §3.1; `12-install-media.md` §7 | attach SQL `.mdf` files (`patients`, `Interface`, `VIDAS`) |
| `D:\Lab_data\Attaching.sql`, `Attach_database.bat` | `12-install-media.md` §7 | referenced by installer scripts but **not present** in the inventoried tree (status: missing) |
| `RL_SETUP.exe` (23,060,480 B) | `rl-setup-analysis.md`, `08-deployment-installer.md` | chosen at install time between Access and SQL Server database types; conversion does not carry stored data |
| `Patients_backup.mdb`, `repairedDB.mdb` | `database-and-data-access-analysis.md` §2.3 | Access mirror's own backup/repair copies |

`08-deployment-installer.md` §2 warns: *"If you convert from one type to another
you will not able to see the stored data on the first database type, But they remain
stored on the HDD."* This is an observed UI string, not a tested data-migration
behaviour.

---

## 4. What is NOT evidenced — explicit list

1. **No `BEGIN TRANSACTION`, `COMMIT`, `ROLLBACK`, `SAVEPOINT` or isolation-level
   statement** in any of the 1,797 SQL literals (VERIFIED negative).
2. **No `INSERT` or `UPDATE` SQL literal** (0 / 1,797). Write paths are unrecovered.
3. **No proof that the patient-deletion cascade is atomic.** Contiguity of VAs
   suggests one routine but does not prove one transaction.
4. **No ADO method-level evidence** (`BeginTrans`, `CommitTrans`, `Execute`,
   `AddNew`, `Update`, `BatchUpdate`) — no method body was decompiled.
5. **No deadlock/locking/retry logic**, no error-handling paths, no rollback log.
6. **No proof of execution order for the 268 DDL literals** (UNKNOWN).
7. **No runtime trace, no SQL Server Profiler capture, no live connection.**
8. Tool executables were **not run**; their internal commit/rollback behaviour is
   UNVERIFIED.

Corroborating prior assessment: the placeholder this document replaced recorded
"transaction boundaries not evidenced at SQL level (no BEGIN TRANSACTION in
extracted statements)" — that negative remains accurate.
