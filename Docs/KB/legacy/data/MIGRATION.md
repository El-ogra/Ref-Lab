# Legacy Schema Migration — V-CD-2019 (v1.605)

**Purpose:** Curated reference for the runtime schema self-migration mechanism.
**Scope:** 268 embedded DDL statements, reconciliation, partial application.
**Audience:** Developers, DBAs.
**Version applicability:** V2 ONLY (V1 has zero DDL).

---

## 1. The mechanism

V2 embeds **268 DDL statements** in its string table and applies them at application start-up,
migrating the database in place. V1 has zero DDL.

| Statement class | Count | Effect |
|---|---:|---|
| `CREATE TABLE` | **27 statements → 14 distinct tables** | Create new tables |
| `ALTER TABLE ADD` | **228 statements → 132 distinct column targets across 29 tables** | Add columns |
| `ALTER COLUMN` (type changes) | **13** | Change column type |

`27 + 228 + 13 = 268` — the statement-class census reconciles exactly with the total.

> **CORRECTION (2026-11-16).** The earlier form of this table read "`CREATE TABLE` | 27 statements →
> 12 distinct tables". The **12** is the number of `CREATE` *statement rows* that create a table
> genuinely absent from V1; the number of *distinct tables named* by `CREATE TABLE` is **14**.
> The statement-class count is **27**. See `Docs/KB/legacy/evidence/CONTRADICTIONS.md` C-16 and
> `data/DATABASE.md` §5.1.

The DDL runs at start-up and migrates the database in place. This is the single most consequential
architectural difference between the two versions.

**Evidence:** `Docs/Complete_Reverse_Engineering_Reference_V2.md` §4.2.

## 2. SQL Server syntax vs Jet 4

The DDL is written in **SQL Server syntax**. Concretely:
- `bigint IDENTITY(1,1) NOT NULL` — SQL Server identity syntax; Jet 4 rejects it.
- `AUTOINCREMENT(1, 1)` — the Access equivalent, emitted as a second variant.
- `sp_rename` — SQL Server system procedure; not available in Jet 4.
- `ALTER COLUMN` — SQL Server type-change form; Jet 4 uses it differently.
- `varchar(MAX)` — SQL Server 2005+; Jet 4 has no `MAX` length.

The shipped `Patients.mdb` shows **both variants** of each statement (SQL Server form + Access
form), indicating the application attempts the SQL-Server-specific statement first and then an
Access-compatible one.

**★ Consequence:** on a Jet 4 installation the SQL-Server-specific statements fail, and the
Access-compatible variants are used where they exist. Where only the SQL Server form exists, the
change is **silently not applied**.

**Evidence:** `Docs/Complete_Reverse_Engineering_Reference_V2.md` §4.2; `Docs/Business_Rules_V2.md` V2-BR-024.

## 3. Reconciliation against the shipped database

Every one of the 268 statements was reconciled against the actual shipped `Patients.mdb`
(103 tables / 1,682 columns).

**Created tables — 27 statements → 14 distinct tables:**

| Result | Statement rows | Distinct tables | Tables |
|---|---:|---:|---|
| CREATED-BY-MIGRATION | 12 | **7** | `Helicobacter`, `MyLabSetting`, `SMSSender`, `SMStemplate`, `smsgroups`, `smslist`, `testtemp` |
| ALREADY-EXISTED | 9 | **4** | `hba1c`, `pictures`, `ratios`, `widal2` |
| NOT applied — absent | 6 | **3** | **`FMF`**, **`IgEallergy`**, **`LBC`** |

> **CORRECTION (2026-11-16).** The previous list named `hba1c`, `pictures`, `ratios` and `widal2`
> as "applied — present in shipped DB". They were **already present in V1** — the migration did
> not create them. Conversely `MyLabSetting` **was** created by the migration and was missing from
> the list, and `IgEallergy` was missing from the "absent" list. The migration evidence is
> `analysis/BATCH-07/migration-proof.json`.

**Added columns — 228 statements → 132 distinct targets across 29 tables:**

| Result | Statement rows | Distinct targets |
|---|---:|---:|
| PRE-EXISTED | 145 | 74 |
| ADDED-BY-MIGRATION | 22 | 16 |
| NOT applied — absent | 61 | **42** |
| **Present in the shipped DB (total)** | 167 | **90** (across 18 tables) |
| **Absent from the shipped DB (total)** | 61 | **42** (across 14 tables) |

> **CORRECTION (2026-11-16).** The previous text read "109 of 132 column targets applied" and
> "19 … never applied", and the table below was captioned "19" while listing 23 columns. The
> reconciliation evidence gives **90 of 132 present** and **42 absent**. The corrected list is
> below.

The 42 unapplied column targets:

| Table | Column(s) | Reason |
|---|---|---|
| `BloodGases` | `aniongap`, `ctco2`, `cthb`, `fio2`, `hco3std`, `po2aa1`, `po2aa2`, `temprat` | 8 of 8 absent — whole blood-gas extension missing |
| `cultures` | `few5`, `few6`, `few7`, `few8` | Absent — culture sensitivity extension |
| `prostaticx` | `crystalp`, `crystalp2`, `crystalp2r`, `crystalpr` | Absent — crystal-pattern extension |
| `urine` | `t56`, `t57`, `testid` | Absent |
| `Patients` | `secword`, `branche`, `grant13`, `grant14`, `grant15`, `grant16`, `grant17` | **SQL-Server-only table** (the Access table is `patientinfo`) — cannot apply to Jet 4 |
| `useraccounts` | `gtype`, `hdd1`, `hdd2` | `useraccounts` table absent from shipped Access DB entirely |
| `IgEallergy` | `item12`, `item24`, `item36` | `IgEallergy` table absent from shipped DB entirely |
| `Helicobacter` | `hpab`, `hpbiop`, `hpiga` | Table exists but 3 extended columns do not |
| `Proelectrophoresis` | `beta2`, `immunolgo` | Absent |
| `pregnancy` | `normalrange3` | Absent |
| `patientinfo` | `gehaname` | Absent |
| `patienttest` | `natighu` | Absent — NATIGH delivery flag has no column |
| `lipids` | `nonhdl` | Absent |
| `Kidney` | `egfr` | Absent |

**★ Forensic conclusion:**
1. The migration was applied **partially** — **90 of 132** column targets are present and **42**
   are not; of the 14 tables named by `CREATE TABLE`, **3** are absent from the shipped database.
2. **42 column targets and 3 tables were never applied.**
3. Two failures are **structurally explained**: `IgEallergy` and `useraccounts` are referenced by code
   but do not exist in the shipped Access database at all — so the features depending on them
   (IgE allergy reporting, the permission-bit model) would fail against this database.
4. `patienttest.natighu` missing means the **NATIGH delivery flag has no column** in the shipped
   schema — the feature is code-present but schema-absent.
5. `BloodGases` is missing **all 8** of its new columns.
6. **23 of the 61 unapplied statements target SQL-Server-only tables** (`Patients`, `useraccounts`,
   `IgEallergy`, `Kidney`, `lipids`) and could never apply to a Jet 4 database. The remaining **38**
   are columns in existing Access tables that were never added — those are the genuine partial
   application.

**This is not a configuration difference — it is a shipped database that does not match the
executable that reads it.**

**Evidence:** `Docs/Complete_Reverse_Engineering_Reference_V2.md` §4.3, §6.9;
`Docs/Business_Rules_V2.md` V2-BR-023, V2-BR-024; external: 11-deployment-payload-2019.md §2.3;
**`analysis/BATCH-07/migration-proof.json` and `migration-proof.md`** (primary reconciliation);
`txt nd sql/ddl-all.txt` (the 268 statements themselves). Full schema: `data/DATABASE.md` §5.

## 4. Migration proof (BATCH-07)

`analysis/BATCH-07/migration-proof.md`:
- 12 of 27 `CREATE TABLE` statements in the DDL were **created by the migration** in the shipped DB.
- 22 of 228 `ALTER TABLE ADD` statements were **added by the migration**; 145 existed already;
  61 were not applied.
- Of the 61 unapplied: 23 belong to SQL-Server-only tables (`Patients` ≠ `patientinfo`,
  `useraccounts`, `IgEallergy`, `Kidney`, `lipids`) — cannot apply to Access.
- The remaining 38 are columns in existing Access tables that were never added
  (`BloodGases.pO2AA1/ctCO2/Aniongap/HCO3std/FiO2/ctHB/TempRAT`, `cultures.Few5..Few8`,
  `prostaticx.crystalp*`, `urine.T56`, `patientinfo.gehaname`, `patienttest.NatighU`).

**★ VERIFIED FACT:** the DDL is not dead code — 12 tables and 22 columns were actually created by
the migration. But the application is **partial** on this particular copy: 38 columns in existing
Access tables were never added. This indicates the migration runs on the client's first run, not at
disc build time.

**Evidence:** external: 11-deployment-payload-2019.md §2.3; `analysis/BATCH-07/migration-proof.md`.

## 5. Cross-confirmed triple-layer change (microbiology)

Three independent layers agree on the "third organism" feature:
- `.ini` templates (32 files): `LabelX002=Organism C`, `ITEMXV002`, `ITEMXD2=…`, `Check6f=1`.
- SQL schema: `cultures.organism3` Text(80); `organism`/`organism2` → Text(80) (was 50).
- Embedded DDL: `ALTER TABLE cultures ALTER COLUMN organism3 → varchar(80)`.

**★ STRONGLY SUPPORTED:** a single feature confirmed across three independent layers. This is the
strongest evidence that the three layers are maintained consistently.

**Evidence:** external: 11-deployment-payload-2019.md §2.4, §9.

## 6. Version applicability

- Schema self-migration: **V2 ONLY** (V1 has zero DDL).
- Partial application: **V2 ONLY** (specific to the shipped Access database).

## 7. Known limitations

- Whether the migration is retried on a later run is NOT ESTABLISHED (V2-BR-023).
- The exact failure-handling path (silent skip vs logged error) is NOT ESTABLISHED (V2-BR-024).
- `VIDAS` database never structurally characterised.
- `useraccounts` table absent from all `.mdb` (SQL Server only).

## 8. Related documents

- `Docs/KB/legacy/data/MODEL.md` — data model.
- `Docs/KB/legacy/data/IDENTIFIERS.md` — identifier contracts.
- `Docs/KB/legacy/data/BRANCHES.md` — branch model.
- `Docs/KB/legacy/clinical/PROFILES.md` — profile tables.
- Original source evidence: `Docs/Complete_Reverse_Engineering_Reference_V2.md` §4,
  `Docs/Business_Rules_V2.md` V2-BR-023/024, external: 11-deployment-payload-2019.md §2,
  `Database documentation.md` (root).
- `evidence/reverse-engineering/exe/exe_findings.md` — exe RE findings.
