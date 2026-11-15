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
| `CREATE TABLE` | 27 statements → 12 distinct tables | Create new tables |
| `ALTER TABLE ADD` | 228 statements → 132 distinct column targets across 29 tables | Add columns |
| `ALTER COLUMN` (type changes) | 13 | Change column type |

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

**Created tables — 12 targets:**

| Result | Tables |
|---|---|
| Applied — present in shipped DB (10) | `Helicobacter`, `SMSSender`, `SMStemplate`, `hba1c`, `pictures`, `ratios`, `smsgroups`, `smslist`, `testtemp`, `widal2` |
| NOT applied — absent (2) | **`FMF`**, **`LBC`** |

**Added columns — 132 distinct targets across 29 tables:**

| Result | Count | Detail |
|---|---:|---|
| Applied — present in shipped DB | 109 | across 26 tables |
| NOT applied — absent | 19 | see below |

The 19 unapplied column targets:

| Table | Column(s) | Reason |
|---|---|---|
| `BloodGases` | `aniongap`, `ctco2`, `cthb`, `fio2`, `hco3std`, `po2aa1`, `po2aa2`, `temprat` | 8 of 8 absent — whole blood-gas extension missing |
| `Helicobacter` | `hpab`, `hpbiop`, `hpiga` | Table exists but 3 extended columns do not |
| `IgEallergy` | `item12`, `item24`, `item36` | `IgEallergy` table absent from shipped DB entirely |
| `Proelectrophoresis` | `beta2`, `immunolgo` | Absent |
| `lipids` | `nonhdl` | Absent |
| `patienttest` | `natighu` | Absent — NATIGH delivery flag has no column |
| `pregnancy` | `normalrange3` | Absent |
| `urine` | `testid` | Absent |
| `useraccounts` | `gtype`, `hdd1`, `hdd2` | `useraccounts` table absent from shipped Access DB entirely |

**★ Forensic conclusion:**
1. The migration was applied **partially** — 109 of 132 column targets and 10 of 12 created tables.
2. **19 column targets and 2 tables were never applied.**
3. Two failures are **structurally explained**: `IgEallergy` and `useraccounts` are referenced by code
   but do not exist in the shipped Access database at all — so the features depending on them
   (IgE allergy reporting, the permission-bit model) would fail against this database.
4. `patienttest.natighu` missing means the **NATIGH delivery flag has no column** in the shipped
   schema — the feature is code-present but schema-absent.
5. `BloodGases` is missing **all 8** of its new columns.

**This is not a configuration difference — it is a shipped database that does not match the
executable that reads it.**

**Evidence:** `Docs/Complete_Reverse_Engineering_Reference_V2.md` §4.3, §6.9;
`Docs/Business_Rules_V2.md` V2-BR-023, V2-BR-024; external: 11-deployment-payload-2019.md §2.3.

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

**Evidence:** external: 11-deployment-payload-2019.md §2.3.

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
