# Application Data Access Architecture — REAL LAB SYSTEM v1.605

**Reconstruction scope.** Client-side data-access model of the v1.605 executable
(prior-work identifier V-CD-2019). The v1.605 binary is not present locally in this
session; all statements below derive from the read-only extracted inventories in
`D:\Collective work`. Static evidence only — no query was executed, no database was
contacted, no installer or script was run.

**Confidence legend.** VERIFIED = directly present in a named evidence field/row;
STRONGLY_SUPPORTED = multiple independent evidence classes agree; INFERRED =
reasoned reconstruction with no direct call-site proof; UNVERIFIED = asserted by a
prior report but not re-derived here; CONFLICTED = two evidence sets disagree.

---

## 1. Evidence base

| Evidence file | What it supplies | Confidence |
| --- | --- | ---: |
| `statements__[analysis_BATCH-04_sql_v1605].csv` | 1,797 rows: `va, verb, tables, has_star, length, text` — the authoritative SQL-literal inventory for this reconstruction | VERIFIED |
| `tables__[analysis_BATCH-04_sql_v1605].csv` | Per-table `statements, select, delete, shape, distinct_columns, sample_statement` | VERIFIED |
| `columns__[analysis_BATCH-04_sql_v1605].csv` | Qualified-column reference counts | VERIFIED |
| `ddl-evidence__[analysis_BATCH-04_sql_v1605].txt` | 268 DDL-shaped literals, with VA, in both SQL Server and Jet dialects | VERIFIED |
| `database-and-data-access-analysis.md` | Connection-string templates, CREATE/ALTER analysis, authorisation/operator stores | VERIFIED (as prior report) |
| `06-configuration.md`, `configuration-and-resources.md` | `BASE.ini`, `data_connection*.udl`, `SETTING.ini`, `Data\*.ini` | VERIFIED (as prior reports) |
| `dependency-inventory.json`, `msado27.tlb.json` | Provider/component inventory, ADO 2.7 typelib version 2.81.1132.0 | VERIFIED |
| `objects-full__[analysis_BATCH-04_vb6_v1605].txt`, `vb6-objects.json` | 210 compiled VB6 objects; `labdb`, `labdb1` modules; 8,026 declared methods | VERIFIED |
| `normalrange-coderefs.json` | Maps a subset of SQL string literals to code-site addresses and string VAs | VERIFIED |
| `branch-data.json`, `branch-profile-schemas.csv` | 49-table schema-only per-branch template database | VERIFIED |
| `08-deployment-installer.md`, `12-install-media.md` | Database-type selection, `Attach_*.bat`, install targets | VERIFIED (as prior reports) |

**Scope conflict, recorded explicitly.** `database-and-data-access-analysis.md`
reports 1,370 embedded SELECTs, 14 CREATE and 120 ALTER statements and ranks
`patienttest` first (138 targets). The BATCH-04 CSV reports 1,381 SELECT, 27 CREATE,
241 ALTER and ranks `patientinfo` first (428 statements). The two sets use different
extraction rules (the older report counts strings beginning with `SELECT`; the CSV
attributes a statement to every table it names). Both denominators are retained;
the BATCH-04 CSV is used for all counts in this document. Status: CONFLICTED by
method, not by evidence.

---

## 2. Overall model (STRONGLY_SUPPORTED)

```
VB6 SP6 client (real lab system.exe)
  ├─ bound data controls: MSADODC.OCX / MSDATGRD.OCX / MSDATLST.OCX
  ├─ DataReport: MSDBRPT.DLL (+ datareport_vh resources)
  ├─ shared modules: labdb, labdb1, labfunction, otherfun, saveIOtemp
  └─ string-built SQL literals  ──▶  ADO 2.7 (msado27.tlb, MDAC 2.8)
                                       ├─ outer provider: MSDataShape / MSDataShape.1
                                       ├─ SQL Server: SQLOLEDB.1  → database "Patients"
                                       └─ Access: Microsoft.Jet.OLEDB.4.0 → Patients.mdb
```

There is no ORM, no stored-procedure data API and no parameterisation visible in
the 1,797 literals. The only stored-procedure calls are two backup-device commands
(§6). All application data manipulation is expressed as concatenated SQL text or,
by inference, as ADO recordset methods.

---

## 3. Connection and provider layer

### 3.1 Providers (VERIFIED)

| Provider | Role | Evidence |
| --- | --- | --- |
| `SQLOLEDB.1` | SQL Server data provider | `06-configuration.md` §2–3; `dependency-inventory.json` `framework_dependencies[0]` |
| `MSDataShape.1` / `MSDataShape` | Outer shaping provider; enables `SHAPE … APPEND … RELATE` hierarchical recordsets | `06-configuration.md` §3; `database-and-data-access-analysis.md` §2.4 |
| `Microsoft.Jet.OLEDB.4.0` | Access mirror provider | `08-deployment-installer.md` §2; `database-and-data-access-analysis.md` §2.3 |
| ADO 2.7 typelib | `msado27.tlb`, version `2.81.1132.0`, ProductName "Microsoft Data Access Components" | `msado27.tlb.json` → `pe.version_info` |

### 3.2 Connection templates (field names and purposes only; no values reproduced)

`database-and-data-access-analysis.md` §2 identifies four templates assembled from
separate UTF-16LE literals at run time:

1. `SQLOLEDB.1` + `Initial Catalog=Patients` + `Data Source=<server>` with explicit
   user/password fragments and `Persist Security Info=True`.
2. `MSDataShape.1` wrapping template 1 via `Data Provider=SQLOLEDB.1`.
3. `SQLOLEDB.1` + `Integrated Security=SSPI` + `Persist Security Info=False`.
4. `MSDataShape.1` wrapping template 3.

Credential *shape* is evidenced; credential *values* are deliberately not recorded
here. `06-configuration.md` §1–3 gives the on-disk descriptors:

- `bin\BASE.ini` → `[oledb]` fields `Databasetype`, `loca`, `DataSource`,
  `PersistSecurityInfo`, `UserID`, `Password`, `InitialCatalog`; `[Initial]` fields
  `Initialw`, `nrls`, `drls`. `nrls=605` is the embedded version marker (VERIFIED).
- `bin\data_connection.udl` → `Provider=SQLOLEDB.1`, `Integrated Security`,
  `Persist Security Info`, `User ID`, `Initial Catalog`, `Data Source`,
  `Initial File Name`. The `Initial File Name` points into `D:\Lab_data` at a
  BF6500 analyser database (an instrument vendor path, not first-party logic).
- `bin\data_connection1.udl` → `Provider=MSDataShape.1`, `Data Provider=SQLOLEDB.1`,
  `Initial Catalog=Patients`, `Data Source=<server>`.

### 3.3 Database-type selection and per-branch switching (STRONGLY_SUPPORTED)

`08-deployment-installer.md` §2 documents a runtime choice between
**Microsoft Access database** (`Microsoft.JET.OLEDB.4.0`, `D:\real lab
system\bin\Patients.mdb`) and **Microsoft SQL server database** (SQL Server 2005,
`D:\Lab_data\Patients.mdf`), with the explicit warning that converting between
types does not carry stored data across. §2.1 lists three SQL databases:
`patients` → `Attach_Patients.bat`, `Interface` → `Attach_Interface.bat`, and
`VIDAS` → `Attach_VIDAS.bat`. `06-configuration.md` §1 additionally shows
`loca=E:\Lab_data` as the configured data location — a **drive-letter conflict**
with the installer's `D:\Lab_data` (CONFLICTED; installation-dependent).

Per-branch evidence:

- `Patients.branche`, `Patients.SecWord`, `Patients.grant13..grant17` added by DDL
  (`ddl-evidence__[analysis_BATCH-04_sql_v1605].txt`, VAs `6DC264`, `6DC2BC`,
  `6DC31C`–`6DC460`, and Jet forms `6DC4B0`–`6DC714`). VERIFIED.
- `branch-data.json` → 49-table, **zero-row** schema-only template used to seed a
  new branch's per-test result tables. VERIFIED.
- `SUBSTRING(patientinfo.patientid,9,1)` (VA `6843E2`) and its Jet twin
  `mid(patientinfo.patientid,10,3)` (VA `68463A`) — position 9/10 of the 13-character
  patient ID used as a branch discriminator in the legacy system. VERIFIED.
- Supporting tables named in `08-deployment-installer.md` §5 and object inventory:
  `Labbranch`, `grouptable`, `loggroup` (objects `z_branchs`, `z_group`). VERIFIED as
  names; the switching algorithm itself is INFERRED.

> Note: the current-generation project rule "one laboratory site, no branch column"
> does not apply to this legacy target; the v1.605 evidence above is branch-aware.

---

## 4. SQL construction and dialect duplication (VERIFIED)

The 1,797 literals are assembled by string concatenation. Literal pairs differ only
by server dialect or quote style; examples:

| Concern | SQL Server form (VA) | Jet/Access form (VA) |
| --- | --- | --- |
| Date literal | `requestdate >= '` (e.g. `691BBA`) | `requestdate >= #` (e.g. `691C86`) |
| Substring | `SUBSTRING(patientinfo.patientid,10,3)` (`6843E2`) | `mid(patientinfo.patientid,10,3)` (`68463A`) |
| Boolean | `testorno='True'` (`672722`) | `testorno=True` (`6729D6`) |
| DDL identity | `ID bigint IDENTITY(1,1)` (`6755EA`) | `ID AUTOINCREMENT(1, 1)` (`67B7AE`) |
| DDL memo | `normalrang varchar(MAX)` (`677D3E`) | `normalrang MEMO` (`67413A`) |

Two consequences follow directly: **the client carries two SQL dialects and selects
one per configured database type** (STRONGLY_SUPPORTED), and **values are
interpolated into SQL text rather than bound** — an observable SQL-injection
exposure (VERIFIED at the string level; exploitability not tested).

---

## 5. Hierarchical recordsets (SHAPE)

27 statements carry verb `SHAPE` in the BATCH-04 CSV (VERIFIED). Representative
literal, VA `672722`:

```sql
SHAPE {SELECT subgrouptable.subgroupcode, subgrouptable.subgroupname
       FROM subgrouptable GROUP BY ... order by subgroupname} AS cmdbill
APPEND ({SELECT * FROM testtable where testorno= 'True' and testsimp <> ''
         order by testsimp} AS cmdbill_d
        RELATE subgroupcode TO subgroupcode) AS cmdbill_d
```

(Truncated safely at 300 characters; full text is in the CSV row.)

The parent/child aliases `cmdbill`, `cmdbill_d`, `cmdbill_d` recur. `testtable`
appears in 8 SHAPE statements, `normalrange` in 4, `patientinfo` in 17,
`subgrouptable` in 2 — see `tables__[analysis_BATCH-04_sql_v1605].csv` `shape`
column. These hierarchies are the data source for VB6 DataReport objects
(`ReportFatora`, `ReportTestTime`, `ReportPCR`, etc., in
`objects-full__[analysis_BATCH-04_vb6_v1605].txt`). Link to
`database-and-data-access-analysis.md` §2.4: the report template *is* the data plan.

---

## 6. Runtime DDL migration engine (VERIFIED)

`ddl-evidence__[analysis_BATCH-04_sql_v1605].txt` header: "DDL-shaped SQL literals:
268" (269 lines including header). The BATCH-04 CSV classifies 241 as `ALTER` and
27 as `CREATE`. Observations:

- Two complete dialect families: SQL Server (`varchar(MAX)`, `IDENTITY(1,1)`,
  `smalldatetime`) at VAs ~`67413A`–`67B522`; Jet (`MEMO`, `AUTOINCREMENT(1, 1)`,
  `DATETIME`, `YESNO`) at VAs ~`67B7AE`–`681456` and `6DC264`–`6DC714`.
- CREATE TABLE targets 8 tables per the prior report: `testtemp`, `MyLabSetting`,
  `smsgroups`, `LBC`, `IgEallergy`, `smslist`, `SMStemplate`, `SMSSender`; the
  BATCH-04 CREATE set additionally includes `pictures`, `hba1c`, `FMF`, `widal2`,
  `ratios`, `Helicobacter` (different extraction scope; CONFLICTED by method).
- `pictures` is widened by ~84 ALTER statements (prior report count) — repeated
  column additions `numsx1..15`, `numsxt1..15`, `shapesx1..12`.
- No versioning table and no migration script are shipped; the executable itself
  mutates the schema (prior report §3.2). The precise startup trigger and ordering
  are **UNKNOWN**.

---

## 7. Module-level ownership (INFERRED)

`objects-full__[analysis_BATCH-04_vb6_v1605].txt` records 210 compiled objects,
including two data-access modules:

| Object | Type | Methods | Inferred role |
| --- | --- | ---: | --- |
| `labdb` | module | 24 | shared database helpers |
| `labdb1` | module | 20 | secondary shared database helpers |
| `labfunction` | module | 25 | general helpers |
| `saveIOtemp` | module | 10 | temporary I/O |
| `addpatient` | form | 192 | patient registration |
| `patientmain` / `patientview` | form | 84 / 33 | patient search, work list |
| `testmain` | form | 30 | order/test selection |
| `frm_money` / `moneypaid` | form | 38 / 18 | payment |
| `testrange` | form | 17 | normal-range maintenance |
| `Result*` forms (60+) | form | 11–231 each | per-discipline result entry |
| `Report*` datareports (60+) | datareport | 0–1 each | printing |

`vb6-objects.json` supplies method counts and name-table VAs but **not method names or
bodies**. `normalrange-coderefs.json` maps a subset of SQL literals to code-site
addresses (e.g. `SELECT * FROM normalrange where testcode=` → sites `0x024e1568`,
`0x033fdb62`, …) but those procedures are unnamed. A prior batch records an
independent negative result on recovering procedure names. Therefore
**statement-to-named-method mapping is not established** (UNR-008); all object
associations above are INFERRED from object naming and table-name correlation.

---

## 8. What is NOT evidenced

- No `INSERT`, `UPDATE`, `BEGIN TRAN`, `COMMIT` or `ROLLBACK` literal in any of the
  1,797 `text` fields (VERIFIED negative). Data writes are therefore attributed to
  ADO recordset methods or to unextracted literals — INFERRED, not proven.
- No proof that every ALTER statement executes at every start (UNKNOWN).
- No decompiled method body proving which object issues which statement (UNR-008).
- No live-server configuration, no network capture, no runtime trace.

See `Method_To_SQL_Mapping.md`, `Method_To_Table_Mapping.md`,
`Database_Transactions.md` and `Data_Flow_Reconstruction.md` in this directory.
