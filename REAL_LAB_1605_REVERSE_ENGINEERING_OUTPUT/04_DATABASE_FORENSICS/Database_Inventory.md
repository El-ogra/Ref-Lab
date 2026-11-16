# Database Inventory — REAL LAB SYSTEM v1.605 (V-CD-2019) vs v1.00 (V-ROOT-1.00)

## 0. Provenance and hard boundary

**No `.mdb` file was opened and no database was contacted.** The `.mdb` binaries are not
present in `D:\Collective work`. This inventory is reconstructed exclusively from the
authoritative extracted schema dumps, per-table CSV exports, DDL literal extracts and
prior database reports listed below. Confidence labels follow the task convention:
VERIFIED / STRONGLY_SUPPORTED / INFERRED / UNVERIFIED / CONFLICTED / UNKNOWN.

## 1. Access / Jet 4 stores

### 1.1 `Patients.mdb` — shipped primary store

| Version | Tables | Columns | Rows | Source |
|---|---:|---:|---:|---|
| v1.00 (V-ROOT-1.00) | **93** | **1,527** | **3,123** (sum of 93 table row counts) | `v1-Patients.mdb.sql` / `schema-Patients.sql` (42,934 B, 93 `CREATE TABLE`); `db-summary.csv` row 1; `rowcounts__[analysis_BATCH-02_db_Patients].csv` (93 rows, total recomputed 3,123) |
| v1.605 (V-CD-2019, shipped) | **103** | **1,682** | not dumped for 2019 | `v2019-Patients.mdb.sql` (47,464 B, 103 `CREATE TABLE`, 1,682 parsed columns); `Version_Identity_and_Scope.md` §3.3; `Data_Model_and_Workflow_Entities_V2.md` §2.1 |
| Delta | **+10** | **+155** | — | direct diff of the two dumps; independently restated in the two reports above |

Jet version **Jet 4 (Access 2000 / MDB 4.0)**, size **2,531,328 B** for the v1.00 sample
(`db-summary.csv` row 1). Path in the shipped product:
`real lab system\bin\Patients.mdb`. **Local status: schema dump + CSV samples present;
binary absent.** The connection literal
`PROVIDER=MSDataShape;Data PROVIDER=Microsoft.Jet.OLEDB.4.0;Data Source=D:\real lab system\bin\Patients.mdb`
is quoted in `database-and-data-access-analysis.md` §2.3.

### 1.2 `Patients_backup.mdb`

**90 tables / 1,381 columns / 2,786 rows** — `db-summary.csv` row 2, corroborated by
`schema__[analysis_BATCH-02_db_Patients_backup].sql` (parsed: 90 tables / 1,381 columns)
and `rowcounts__[analysis_BATCH-02_db_Patients_backup].csv` (90 rows, total recomputed
2,786). Size 2,146,304 B, Jet 4. `db-summary.csv` describes it as "byte-identical to the
2019 CD copy"; `Version_Identity_and_Scope.md` §4.2 marks its version affiliation
**UNKNOWN** (its column count is below v1.00 `Patients.mdb`, consistent with either a
stale V1 point-in-time backup or a V1 artefact). **Local status: schema dump + CSV samples
present; binary absent.**

### 1.3 `Branch_Data.mdb` — schema-only multi-branch template

**49 tables / 969 columns / 0 rows** — `schema-Branch_Data.sql` (= `schema.sql`, 26,262 B,
parsed: 49 tables / 969 columns); `db-summary.csv` row 3; `rowcounts__[analysis_BATCH-02_db_data].csv`
is the *data.mdb* file, so the zero-row figure comes from `db-summary.csv` and
`Version_Identity_and_Scope.md` §3.3 ("Schema-only template, 0 rows"). Size 970,752 B, Jet 4.

Two distinct 2019 copies are described:
- `bin\` copy: **byte-identical to v1.00**, SHA-256 `f6d6d5e2…`
  (`Data_Model_and_Workflow_Entities_V2.md` §2.1);
- `System\` copy: 966,656 B, SHA-256 `e6c3953e…`, **9 columns narrower in `patienttest`**
  (`Data_Model_and_Workflow_Entities_V2.md` §2.1; `Version_Identity_and_Scope.md` §3.3;
  `Functional_Specification_V2.md` "Known limitations").

**CONFLICTED:** "the 2019 `Branch_Data.mdb` template is 9 columns narrower" versus
"the `bin\` copy is byte-identical to V1". Both are recorded; the reports disagree on
which copy they describe. **Local status: one schema dump present; the two binaries and
their hashes are not present in the flat evidence.**

### 1.4 `data.mdb`

**3 tables / 34 columns / 0 rows** — `v1-data.mdb.sql` and `v2019-data.mdb.sql` (1,420 B
each) and `schema__[analysis_BATCH-02_db_data].sql`; `db-summary.csv` row 4 (249,856 B,
Jet 4). Tables: `history`, `patientinfo` (3 cols), `wasl`
(`rowcounts__[analysis_BATCH-02_db_data].csv`: 0 rows each). Described as "system data:
test catalogue, normal ranges, constants, prices". **Local status: schema dump + row-count
CSV present; binary absent.**

### 1.5 `repairedDB.mdb`

Named as a fourth Jet path in the connection literals:
`D:\real lab system\bin\repairedDB.mdb` (`database-and-data-access-analysis.md` §2.3;
`database-engineering-reference.md` §1 "Extra files"). **No schema, no counts, no binary —
UNKNOWN / described only.**

## 2. SQL Server stores (not accessible in this environment)

| Store | Evidence | Status |
|---|---|---|
| Catalogue **`patients`** (live clinical store) | Connection fragments `Provider=SQLOLEDB.1;…;Initial Catalog=Patients;Data Source=<server>` and `Provider=MSDataShape.1;…;Data Provider=SQLOLEDB.1;Initial Catalog=Patients` (`database-and-data-access-analysis.md` §2.1–2.2; `database-engineering-reference.md` §1). Installed by `Setup4.bat` → SQL Server Express 2005. Servers named `REAL_LAB`, `DESKELTEL`, `REALLAB_AIS` (`01-architecture.md`; `database-engineering-reference.md` §1). | **STRONGLY_SUPPORTED — described only**; no server, no schema dump |
| Catalogue **`Interface`** | Attached by `D:\Lab_data\Attach_Interface.bat` (`Version_Identity_and_Scope.md` §3.3; `1605-phase2-findings.md`: `D:\Lab_data\Interface.mdf`). `Version_Identity_and_Scope.md` §3.3: belongs to the separately installed "RealLab Interface" product. | **STRONGLY_SUPPORTED — described only**; `D:\Lab_data` is outside the collected tree |
| Database **`VIDAS`** | Referenced by `D:\Lab_data\Attach_VIDAS.bat` (`Version_Identity_and_Scope.md` §3.3, §3.5; `12-install-media.md` hypothesis: possibly Visual FoxPro, based on the product line). | **UNKNOWN — referenced but never structurally characterised**; no tables/columns/rows known. Do not assert a type. |
| **MySQL** backend | `Setup5.bat` → `mysql-connector-odbc-3.51.30-win32.msi` (`05-deployment.md`; `Version_Identity_and_Scope.md` §3.5). | Installer absent from the inventoried tree — **UNVERIFIED** as an actual deployed store |

## 3. Connection templates (`data_connection*.udl` and equivalents)

| Artifact | Evidence | Status |
|---|---|---|
| `data_connection.udl` | 674 B, SHA-256 `31DE843BC475A9C1CFE24B7898537A49AF8108D95F4B8ACF0F1D76A4BA3C06CE`; inventoried in `BATCH-02-files.csv`, `BATCH-07-files.csv`, `batch1-hashes.txt`. Contents reported in `06-configuration.md` §2: `Provider=SQLOLEDB.1;Integrated Security=SSP;…;User ID=<account>;Initial Catalog=…` (account value withheld per the no-secrets rule). | **Inventoried/described; UDL file itself not in flat evidence** |
| `data_connection1.udl` | Contains a `User ID` / `Password` credential pair for the `sa` account (specific value deliberately **withheld** per the no-secrets rule; see `06-security.md`; `07-security-findings.md`; `Batch3_Analysis_Report.md`); configures `Provider=MSDataShape.1` (`Business_Rules_V2.md`). | **STRONGLY_SUPPORTED — described only** |
| Credential-bearing SQL pair | `;User ID=<user>` + `;Password=<password>` literals, `Persist Security Info=True` (`database-and-data-access-analysis.md` §2.1) | VERIFIED from embedded literals |
| SSPI pair | `Integrated Security=SSPI;Persist Security Info=False` (`database-and-data-access-analysis.md` §2.2) | VERIFIED from embedded literals |
| Jet mirror templates | Four paths: `Patients.mdb`, `Patients_backup.mdb`, `patients.mdb`, `repairedDB.mdb` (`database-and-data-access-analysis.md` §2.3) | VERIFIED from embedded literals |

Credential values are intentionally not reproduced here beyond their existence and
structure; see the cited security reports for the documented finding.

## 4. Extracted schema / CSV evidence present locally in `D:\Collective work`

| File | Bytes | Content used here |
|---|---:|---|
| `v2019-Patients.mdb.sql` | 47,464 | v1.605 shipped schema: 103 CREATE TABLE, 1,682 columns |
| `schema-Patients.sql` = `v1-Patients.mdb.sql` = `schema__[analysis_BATCH-02_db_Patients].sql` | 42,934 | v1.00 schema: 93 CREATE TABLE, 1,527 columns (all three parse identically) |
| `schema__[analysis_BATCH-02_db_Patients_backup].sql` | 39,174 | backup schema: 90 tables / 1,381 columns |
| `schema-Branch_Data.sql` = `schema.sql` | 26,262 | branch template: 49 tables / 969 columns |
| `v1-data.mdb.sql` = `v2019-data.mdb.sql` = `schema__[analysis_BATCH-02_db_data].sql` | 1,420 | `data.mdb`: 3 tables / 34 columns |
| `ddl-evidence__[analysis_BATCH-04_sql_v1605].txt` | — | 268 DDL literals: 27 CREATE TABLE + 241 ALTER TABLE (228 ADD, 13 ALTER COLUMN) |
| `ddl-all.txt` | — | 27 CREATE TABLE (same DDL corpus, alternative extraction) |
| `statements__[analysis_BATCH-04_sql_v1605].csv` | 284,077 | 1,797 SQL statements with VA addresses (1,381 SELECT, 72 DELETE, 241 ALTER, 27 CREATE, 27 SHAPE, 49 OTHER) |
| `tables__[analysis_BATCH-04_sql_v1605].csv`, `columns__[analysis_BATCH-04_sql_v1605].csv` | 9,706 / — | statement-derived table/column surface |
| `tables.csv`, `columns.csv`, `created-tables.csv`, `added-columns.csv`, `column-evolution.csv`, `cumulative-schema.csv`, `table-overlap.csv` | — | cross-version and usage analytics (see `Tables_And_Columns.md`) |
| `db-summary.csv` / `db-summary.md`, `rowcounts.csv`, `rowcounts__[analysis_BATCH-02_db_{Patients,Patients_backup,data}].csv` | — | store sizes, table/column/row counts |
| `testtable-stats.json`, `normalrange.csv`, `testtable.csv`, `constanttable.csv`, `pricelist.csv` and other per-table CSVs | — | knowledge-base statistics and enumerations (see `Database_Data_Analysis.md`) |

The per-table sample CSVs are **redacted/partial** (`<redacted>` values in `pricelist.csv`,
`grouptable.csv`, `subgrouptable.csv`, `testunits.csv`, etc.); they are used only for
schema, counts and enumerations, never for patient data.

## 5. Summary of uncertainty

- VERIFIED locally: the four Jet schema dumps and the CSV/DDL analytics listed in §4.
- STRONGLY_SUPPORTED but not locally present: SQL Server `patients` and `Interface`
  catalogues, `data_connection*.udl` contents, `repairedDB.mdb`.
- UNKNOWN: `VIDAS` structure; MySQL deployment; `repairedDB.mdb` schema.
- CONFLICTED: 1,617 vs 1,682 columns and +90 vs +155 columns (`00-baseline-summary.md`
  vs primary dump + `Version_Identity_and_Scope.md` §3.3); the two `Branch_Data.mdb`
  copies; the "17 new tables" figure in `Version_Identity_and_Scope.md` §3.5 versus the
  **10** new tables in the direct dump diff.
