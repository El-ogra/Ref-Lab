# Legacy Contradiction Register

**Purpose:** Structured register of conflicting findings in the legacy systems.
**Scope:** All 15 contradictions identified in the architecture audit plus any from the source docs.
**Audience:** All.
**Version applicability:** Mixed.

---

## Status vocabulary

- **OPEN** — conflicting claims, no resolution attempted.
- **PARTIALLY RESOLVED** — some evidence gathered, not conclusive.
- **RESOLVED** — evidence explicitly supports a conclusion.
- **SUPERSEDED** — a later finding supersedes an earlier one.

**Rule:** Do not classify a contradiction as RESOLVED unless the existing evidence explicitly
supports that conclusion. Do not silently resolve.

---

## C-01 — NATIGH/SMS delivery requires internet; manual advises keeping server off internet

- **Subject:** Digital delivery vs network posture.
- **Conflicting claims:** V2 adds NATIGH.COM publication and SMS notification (internet-dependent),
  while the shipped manual (byte-identical to V1) advises keeping the server machine off the
  internet to preserve confidentiality and OS stability.
- **Source A:** Vendor guide module 1 (capability 1-12) — external: 09-vendor-capability-catalogue.md.
- **Source B:** CHM `001.htm` — «يفضل عدم توصيل انترنت على جهاز السيرفر … للحفاظ علي سرية البيانات».
- **Version applicability:** V2 ONLY (contradiction created by V2's additions against V1's guidance).
- **Status:** OPEN (internal contradiction; evidence does not resolve which).
- **Affected documents:** `business/CAPABILITIES.md`, `business/WORKFLOWS.md`, `business/SECURITY.md`.
- **V2-BR:** V2-BR-006.

## C-02 — Two `Branch_Data.mdb` copies on the 2019 CD

- **Subject:** Which branch template is loaded.
- **Conflicting claims:** `bin\` copy (970,752 B) is byte-identical to V1; `System\` copy (966,656 B)
  is narrower (35 vs 46 columns in `patienttest`). The "template narrowed by 9 columns" conclusion
  was drawn against the `System\` copy, not the one the application loads from `bin\`.
- **Source A:** `analysis/BATCH-04/schema/column-evolution.csv`.
- **Source B:** external: 11-deployment-payload-2019.md §2.3, §6.1.
- **Version applicability:** V2 ONLY.
- **Status:** OPEN (which copy is loaded NOT ESTABLISHED).
- **Affected documents:** `data/BRANCHES.md`, `data/MODEL.md`.
- **V2-BR:** V2-BR-025.

## C-03 — `Patients_backup.mdb` stale in both versions

- **Subject:** Backup currency.
- **Conflicting claims:** `Patients.mdb` grew (+151,552 B in V2) but `Patients_backup.mdb` is
  byte-identical in both versions (2,146,304 B).
- **Source A:** external: 11-deployment-payload-2019.md §6.1.
- **Source B:** `Docs/Complete_Reverse_Engineering_Reference_V2.md` §6.2.
- **Version applicability:** SHARED.
- **Status:** RESOLVED (confirmed stale; ISS-18).
- **Affected documents:** `data/MODEL.md`.

## C-04 — Migration DDL is SQL Server syntax; cannot execute against Jet 4

- **Subject:** Schema self-migration portability.
- **Conflicting claims:** DDL uses `bigint IDENTITY(1,1)`, `varchar(MAX)`, `sp_rename`, SQL Server
  `ALTER COLUMN` form. Jet 4 rejects these. Both variants (SQL Server + Access) appear in the
  shipped schema, indicating the app tries SQL Server first then Access.
- **Source A:** `analysis/BATCH-04/schema/ddl-all.txt`.
- **Source B:** shipped `Patients.mdb` schema (both variants present).
- **Version applicability:** V2 ONLY (V1 has zero DDL).
- **Status:** RESOLVED (partial application: 10/12 tables, 109/132 columns).
- **Affected documents:** `data/MIGRATION.md`.
- **V2-BR:** V2-BR-023, V2-BR-024.

## C-05 — Runtime-selected reference bands absent from Access mirror

- **Subject:** Reference-range authority.
- **Conflicting claims:** Runtime query filters `normalrange.REFRANGE='2'`, but the mirror's 1,152
  rows carry REFRANGE = '' (797), '0' (345), '1' (10) — none with '2'. The 404 rows with
  non-zero `firstold` are marked '' or '0'.
- **Source A:** Runtime SQL literals (`0x035796B0` selector).
- **Source B:** mdb-export of `normalrange` (1,152 rows).
- **Version applicability:** V2 (mirror is V2 shipped DB).
- **Status:** RESOLVED (bands live on SQL Server side, not the Access mirror).
- **Affected documents:** `clinical/RULES.md`, `clinical/KNOWLEDGE-LAYER.md`.
- **V2-BR:** V2-BR-034, V2-BR-036.

## C-06 — `IgEallergy` table absent from shipped DB

- **Subject:** Storage-absent feature.
- **Conflicting claims:** Complete feature (40-item allergy panel, entry form, report) exists in code;
  table definition exists in embedded DDL; table is not in the shipped database.
- **Source A:** `analysis/BATCH-04/schema/created-tables.csv`.
- **Source B:** `analysis/BATCH-07/db/v2019-Patients.mdb.sql` (table absent).
- **Version applicability:** V2 ONLY.
- **Status:** RESOLVED (storage-absent).
- **Affected documents:** `data/MODEL.md`, `clinical/PROFILES.md`.
- **V2-BR:** V2-BR-023.

## C-07 — `useraccounts` table absent from Access DB

- **Subject:** Operator table authority.
- **Conflicting claims:** DDL references `useraccounts` (SQL Server table with `Gtype`, `HDD1`, `HDD2`);
  the Access-mode operator table is `patients` (26 cols V1, 35 cols V2). `useraccounts` is absent
  from every `.mdb`.
- **Source A:** DDL statements.
- **Source B:** mdb schema (all 9 databases).
- **Version applicability:** V2 ONLY (SQL-Server-only in both, but referenced by V2 DDL).
- **Status:** RESOLVED (SQL-Server-only).
- **Affected documents:** `business/SECURITY.md`, `data/MODEL.md`.
- **V2-BR:** V2-BR-039.

## C-08 — `patienttest.natighu` column absent

- **Subject:** NATIGH delivery flag.
- **Conflicting claims:** DDL contains `ALTER TABLE patienttest ADD NatighU`; column not present in
  shipped `Patients.mdb`.
- **Source A:** `analysis/BATCH-04/schema/added-columns.csv`.
- **Source B:** `analysis/BATCH-07/db/v2019-Patients.mdb.sql`.
- **Version applicability:** V2 ONLY.
- **Status:** RESOLVED (storage-absent).
- **Affected documents:** `data/MODEL.md`, `business/CAPABILITIES.md`.
- **V2-BR:** V2-BR-005.

## C-09 — `BloodGases` missing all 8 new columns

- **Subject:** Extended blood-gas panel.
- **Conflicting claims:** DDL adds 8 columns (`aniongap`, `ctco2`, `cthb`, `fio2`, `hco3std`,
  `po2aa1`, `po2aa2`, `temprat`); all 8 absent from shipped DB.
- **Source A:** DDL statements.
- **Source B:** shipped `Patients.mdb` schema.
- **Version applicability:** V2 ONLY.
- **Status:** RESOLVED (storage-absent).
- **Affected documents:** `data/MODEL.md`, `clinical/PROFILES.md`.
- **V2-BR:** V2-BR-023.

## C-10 — `help.chm` byte-identical in both versions despite 41 new objects

- **Subject:** Documentation currency.
- **Conflicting claims:** 41 new objects, full SMS subsystem, 10 new tables in V2; CHM unchanged
  (byte-identical, 15 topics).
- **Source A:** BATCH-04 object counts.
- **Source B:** CHM SHA-256 (identical in both versions).
- **Version applicability:** SHARED (CHM byte-identical).
- **Status:** RESOLVED (documentation frozen 2010).
- **Affected documents:** `business/CAPABILITIES.md`, `architecture/OVERVIEW.md`.
- **V2-BR:** V2-BR-033.

## C-11 — `Cache\*.rtf` byte-identical in both versions

- **Subject:** Method-sheet currency.
- **Conflicting claims:** 29 RTF method sheets unchanged byte-for-byte despite V2 additions.
- **Source A:** BATCH-07 (29/29 match).
- **Source B:** file hashes.
- **Version applicability:** SHARED.
- **Status:** RESOLVED (training frozen 2011).
- **Affected documents:** `clinical/RULES.md`, `clinical/KNOWLEDGE-LAYER.md`.

## C-12 — Conflicting runtime builds ship on one disc (CAB vs `OCX\`)

- **Subject:** Installation determinism.
- **Conflicting claims:** CAB ships newer `msvbvm60.dll` (1,386,496 B, 2009) and `msado27.tlb`
  (98,304 B, 2010); `OCX\` ships older builds (1,384,479 B, 2004; 81,920 B, 2008). `Setup.bat`
  copies `OCX\` before the CAB executes, so the final installed version depends on execution order.
- **Source A:** `analysis/BATCH-10/cab/` hashes.
- **Source B:** `analysis/BATCH-10/ocx/` hashes.
- **Version applicability:** V2 ONLY.
- **Status:** RESOLVED (non-deterministic install).
- **Affected documents:** `architecture/DEPLOYMENT.md`.

## C-13 — PDF installation order defective

- **Subject:** Installation correctness.
- **Conflicting claims:** `RL_SETUP.exe` calls `System\PDF\SetupP.bat` (Bullzip only) before
  `System\SetupPZ.bat` (.NET 4 then Bullzip); `SetupFN4.bat` is invoked by nobody and has a
  broken relative path.
- **Source A:** `Setup*.bat` scripts.
- **Source B:** `RL_SETUP.exe` strings (XCOPY command).
- **Version applicability:** V2 ONLY.
- **Status:** RESOLVED (ISS-53).
- **Affected documents:** `architecture/PDF-PRINTING.md`, `architecture/DEPLOYMENT.md`.

## C-14 — Reference-range band-selection algorithm not established

- **Subject:** Clinical safety.
- **Conflicting claims:** The runtime selector emits `SELECT Top 1 * FROM normalrange WHERE …`
  with **no ORDER BY**. Selection at an overlapping boundary is engine-arbitrary. The `pregsmok`
  dimension is populated on all 852 rows but its role is not traced.
- **Source A:** Runtime SQL literals (`0x035796B0`).
- **Source B:** mdb-export of `normalrange`.
- **Version applicability:** V2 (mirror is V2 shipped DB); V1 shares the same table.
- **Status:** OPEN (algorithm unknown; safety-critical).
- **Affected documents:** `clinical/RULES.md`, `clinical/KNOWLEDGE-LAYER.md`.
- **Related:** U-01, U-02, U-03 (1605-phase2).

## C-15 — `pricemethod` value domain not established

- **Subject:** Account type.
- **Conflicting claims:** The vendor guide lists "ضبط نوع الحساب الافتراضي" (default account type)
  with prices, discounts, commissions, and withdrawn samples. The `pricemethod` field values that
  drive the four PDF output categories (`Contracts`, `Free`, `Individual`, `LabToLab`) are not
  established; 0 hits for the three previously-cited values across 1,337 SQL literals.
- **Source A:** Vendor guide 7-7.
- **Source B:** SQL literal search (0 hits).
- **Version applicability:** V2 (documented as V2 capability).
- **Status:** OPEN (value domain unknown).
- **Affected documents:** `clinical/PROFILES.md`, `architecture/PDF-PRINTING.md`, `business/CAPABILITIES.md`.

---

## Summary

| Status | Count |
|---|---:|
| RESOLVED | 12 (C-03, C-04, C-05, C-06, C-07, C-08, C-09, C-10, C-11, C-12, C-13) |
| OPEN | 3 (C-01, C-14, C-15) |

**Note:** C-02 (two branch templates) is listed as OPEN in the audit but the resolution status
in the source docs is "NOT ESTABLISHED" — treated as OPEN here.

## Related documents

- `Docs/KB/legacy/evidence/UNRESOLVED.md` — open questions.
- `Docs/KB/legacy/evidence/EVIDENCE-REGISTER.md` — evidence index.
- `Docs/KB/legacy/mapping/LEGACY-TO-MODERN.md` — legacy vs modern.
- Original source evidence: `LEGACY_SYSTEM_UNDERSTANDING_REPORT.md` §6,
  `Docs/Business_Rules_V2.md` (conflicts noted inline), `evidence/reverse-engineering/reverse-engineering-unresolved-issues.md`.
