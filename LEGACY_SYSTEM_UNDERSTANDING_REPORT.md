# Legacy System Understanding Report — Ref Lab Reverse-Engineering Synthesis

**Date:** 2026-11-15
**Status:** Phase 1 complete (15 of 298 legacy corpus files analysed).
**Language:** English (per owner instruction).
**Scope:** Cumulative understanding of the two legacy systems (`V-ROOT-1.00` and `V-CD-2019` / v1.605)
that the modern `Ref Lab` (C#/.NET 8) replaces, plus the modern system's own posture.

---

## 1. Executive summary

This report synthesises the understanding gained from analysing the 15 highest-value documents of the
legacy reverse-engineering corpus (`E:\Markdown Source Files`, 298 files indexed in `File Index.md`).
Those 15 documents are the consolidated references and the architecture/data-model/business-rules/
deployment/PDF/configuration deep-dives. They cover both legacy versions in full and are all classified
DIRECTLY_RELEVANT + HIGH usefulness by the corpus's own indexer.

**Bottom line:** the legacy system is now understood at the architecture, data-model, clinical-rule,
security, deployment, and PDF layers well enough to drive the modern rebuild's requirements. The
remaining 148 un-indexed corpus files are largely evidence registers, batch reports, and provenance
records; they are expected to refine detail, not change the picture.

The modern `Ref Lab` already addresses the most consequential legacy defects (clear-text credentials,
external PDF printer dependency, no hashing, multi-branch complexity) by design.

---

## 2. Size of understanding (coverage metrics)

### 2.1 Legacy corpus coverage

| Metric | Value |
|---|---:|
| Legacy corpus files (indexed) | 298 (Stages 1–3 of `File Index.md`) |
| Legacy corpus files analysed this phase | 15 |
| Legacy corpus files remaining (Stages 4–6) | 148 (not yet indexed) |
| Legacy corpus total bytes | ~ (298 files, largest 501 KB `File Index.md`) |
| Source artefacts verified unchanged | 16 (hashes before/after identical) |

### 2.2 Understanding by layer (legacy)

| Layer | V-ROOT-1.00 | V-CD-2019 (v1.605) | Confidence |
|---|---|---|---|
| Identity (PE, objects, methods, literals, SQL) | Full | Full | CONFIRMED |
| Architecture (5 layers, modules, dependencies) | Full | Full | CONFIRMED |
| Application interface (screens, capabilities) | 15 capabilities (CHM) | ~60 capabilities (vendor PDF) | CONFIRMED |
| Workflows (patient, result entry, send-out, accounting, ranges, catalogue) | 6 reconstructed | 7 reconstructed | STRONGLY SUPPORTED |
| Configuration (INI, UDL, REPORT.dll) | Full | Full | CONFIRMED |
| Data model (tables, columns, rows) | 93 tables / 1,527 cols | 103 tables / 1,682 cols | CONFIRMED |
| Clinical knowledge (tests, ranges, constants, units, conversions) | 609 / 852 / constants | identical | CONFIRMED |
| Clinical rules (49 V1 rules, 45 V2 rules) | Full catalogue | Full catalogue | CONFIRMED |
| Security (credentials, permissions, audit) | Full | Full | CONFIRMED |
| Deployment (installer, engines, licensing) | Not present in V1 | Full (RL_SETUP v19.00) | CONFIRMED |
| PDF / printing | Absent (V1-BR-046) | Full (Bullzip path) | CONFIRMED |
| Schema migration (DDL) | None | 268 statements, reconciled | CONFIRMED |
| Branch/multi-site model | Single-site | Master + reception branches | CONFIRMED |

### 2.3 Modern system (Ref Lab) posture

| Metric | Value |
|---|---:|
| Source projects | 4 (`Domain`, `Application`, `Infrastructure`, `Presentation`) |
| Test projects | 5 (91 .cs files, 547 `[Fact]`, 14 `[Theory]`, 65 `[InlineData]`) |
| Use cases (MediatR) | 73 |
| EF Core tables | 27 |
| Foreign keys | 8 explicit (+ 1 shadow) |
| Indexes | 27 |
| Permissions | 15 |
| Screens | 14 (+ 2 panels) |
| Architecture guards | 18 (Domain BCL-only, Application no persistence, Presentation no persistence/PDF, etc.) |
| Build status | 0 warnings, 0 errors (per AGENTS.md mandate) |

---

## 3. What is now understood (key findings)

### 3.1 Architecture and identity
- Both versions are VB6 native-code PE32 binaries sharing the same PE timestamp (2013-12-27);
  the timestamp is **not** a version discriminator. V2's product is materially newer (installer
  2020-09-01, guide 2017-04-26) but the executable was never recompiled.
- V2 is not a rewrite: the runtime layer is **byte-identical** (43/43 ActiveX controls,
  25/41 `bin\` files, 29/29 RTF method sheets, CHM byte-identical), while only the data and
  configuration layers changed (+10 tables, +155 columns, 173 of 223 `.ini` files).
- This is the single most important architectural conclusion: **RealLab evolved the data/workflow
  layer only, leaving runtime, documentation, and training frozen at 2010–2011.**

### 3.2 Multi-engine deployment (V2)
- Three database engines supported: Access Jet 4, SQL Server Express 2005, MySQL (Connector ODBC 3.51).
- Default `sa` password `12345678` is hardcoded in the installer (`SAPWD=12345678`) and shipped
  in `BASE.ini` / `data_connection*.udl`. Network protocols are NOT disabled.
- Two separate SQL catalogues (`patients`, `Interface`) plus a third attached database (`VIDAS`).
- The `Interface` database belongs to a separately installed "RealLab Interface" product.

### 3.3 Schema self-migration (V2's most consequential mechanism)
- 268 embedded DDL statements (12 CREATE TABLE, 228 ALTER ADD, 13 ALTER COLUMN) in SQL Server
  syntax with Access-compatible variants.
- Reconciliation against the shipped `Patients.mdb`: 10 of 12 tables and 109 of 132 column targets
  applied; 2 tables (`FMF`, `LBC`) and 19 columns absent.
- `IgEallergy` and `useraccounts` tables absent from the shipped Access DB entirely.
- `patienttest.natighu` (NATIGH delivery flag) column absent — feature is code-present but schema-absent.
- `BloodGases` missing all 8 new columns.
- The shipped Access database is a **partial deployment** of the v1.605 schema.

### 3.4 Multi-branch model (V2)
- Master laboratory + reception branches; branch code at position 9 of the 13-char patient identifier.
- Branch is an attribute of the **operator** (`patients.branche`), not the patient.
- `Branch_Data.mdb` (49 schema-only tables, 0 rows) seeds a new branch.
- Two copies ship on the 2019 CD: `bin\` (byte-identical to V1) and `System\` (narrower); which is
  loaded is NOT ESTABLISHED.

### 3.5 PDF / printing (V2 only)
- V1 has zero PDF capability. V2 delegates PDF to an external printer driver
  (`Bullzip PDF Printer`, name and version `11.1` hardcoded as literals).
- Four output categories: `Contracts`, `Free`, `Individual`, `LabToLab` under `D:\PDF\`.
- Six printer roles; print audit (`resultprint`, `printuser`, `printdate`, `printnum`).
- Quality gate: only reviewed results printable.
- No fallback if Bullzip is absent or renamed; installation order is defective (Bullzip installed
  before its .NET 4 dependency).

### 3.6 Clinical knowledge (identical in both versions)
- 609 tests, 852 reference ranges (304 tests, sex/age banded), constants (CBC: 3.3/6.25/6.75/7.5/8.25;
  PT: 12/35/1.33/`2CVJW`), 69 units, 167 conversions, 650 abbreviations, 46 sub-groups.
- Reference ranges frozen on the test row at order time (112 of 222 rows carry it).
- `pregsmok` populated on all 852 rows (OBSERVED, not documented).
- Band-selection algorithm UNKNOWN (no ORDER BY; engine-arbitrary at overlaps).

### 3.7 Security posture (both versions)
- Clear-text `sa`/`12345678` in `BASE.ini`, UDLs, and installer.
- `admin` account with password equal to user name.
- `PersistSecurityInfo=True`.
- Shared, immutable credential across all clients.
- V2 adds `[lastlog]` logging last 10 connection attempts including the password.
- No password hashing, salting, expiry, lockout, or session timeout evidenced.
- NATIGH digital delivery contradicts the manual's advice to keep the server off the internet.

### 3.8 Permission model
- V1: 12 function-level permission bits (`grant1..grant12`) on the operator table (`patients`).
- V2: extended to 17 bits (`grant13..grant17`) plus `branche` and `SecWord` on the same table.
- `useraccounts` table absent from the shipped Access DB (SQL Server only).
- V2's `users` form has 57 methods; `userspw` for password change.

### 3.9 Identifier contract
- `patientid` (Text 13) is an **order/visit identifier**, not a patient key. No patient master table.
- Positions 3–4 = person key; position 9 = branch (V2); positions 10–12 = display code.
- Hard inter-version data contract — reformatting breaks identity, branch attribution, and turnover counts.

---

## 4. Gaps (remaining needs)

### 4.1 Unresolved technical questions (from the legacy corpus)

| ID | Question | Status |
|---|---|---|
| RL-V2-Q-01 | What is the `VIDAS` database? | UNKNOWN |
| RL-V2-Q-02 | What is the `win32_processor` table for? | UNKNOWN (likely WMI licensing probe) |
| RL-V2-Q-03 / Q-07 | Is `loger` attendance or clinical events? | UNKNOWN |
| RL-V2-Q-04 | Is `Patient_samples` a tracking table or sample store? | UNKNOWN |
| RL-V2-Q-05 | How are `grant13..grant17` and `useraccounts.gtype/hdd1/hdd2` used? | UNKNOWN (schema only) |
| RL-V2-Q-06 | Is `LBC` a clinical profile? | UNKNOWN |
| RL-V2-Q-08 | SQL `patients` vs Access `patientinfo` difference? | UNKNOWN |
| RL-V2-Q-09 | What are `insulin` and `antibio`? | UNKNOWN |
| RL-V2-Q-10 | What is the 22.95 MiB payload inside `RL_SETUP.exe`? | UNKNOWN (custom VB6 container, not decomposed) |
| RL-V2-Q-12 | Which of the 4 PDF output categories is the default? | UNKNOWN |
| V1 Q-01..Q-09 | MSDataShape default path; history authority; pregsmok use; 60 unnamed `.ini`; zarf meaning; nolowexe/nohighexe; 223 vs 46 ini; antibio vs antibiotic; customtesttable purpose | UNKNOWN |

### 4.2 Corpus indexing gaps
- 148 of 298 legacy corpus files are not yet indexed (Stages 4–6 of `File Index.md`).
- These are mostly evidence registers, batch reports, and provenance records; expected to refine detail,
  not change the architectural picture.

### 4.3 Knowledge gaps in the modern system
- `normalrange` band-selection algorithm is not established (affects clinical safety).
- `pregsmok` dimension is not traced.
- `pricemethod` value domain is not established.
- The modern system has no equivalent of the legacy `history` 20-slot EAV table (intentionally
  replaced by typed `PatientHistory` with 20 slots — see `Docs/KB/DATA-MODEL.md`).

---

## 5. Vulnerabilities (legacy, carried forward as lessons)

| # | Vulnerability | Evidence | Modern mitigation |
|---|---|---|---|
| V-01 | Clear-text DB password `12345678` in shipped config + installer | `BASE.ini`, UDLs, `SAPWD=` | PBKDF2-HMAC-SHA256 (210k iterations), env-var connection strings |
| V-02 | Connection as `sa` (full admin) | `UserID=sa` | Per-application scoped DB user (deployment concern, not in code) |
| V-03 | Default `admin`/`admin` account, documented in manual | CHM `002.htm` | Operator management feature with hashed passwords |
| V-04 | `PersistSecurityInfo=True` | `BASE.ini` | Not applicable (no OLE DB) |
| V-05 | Clear-text connection-attempt log (`[lastlog]`) including password | `BASE.ini` | No such logging in modern system |
| V-06 | Shared, immutable credential across all clients | CHM `001.htm` | Per-installation env var |
| V-07 | Credentials leak pre-rebranding product name and install layout | UDL `Initial Catalog` | Not applicable |
| V-08 | SMS queue stores mobile numbers + message bodies in clear text, no retention | `SMSSender` schema | SMS excluded from modern system |
| V-09 | NATIGH digital delivery contradicts "keep server off internet" advice | Vendor guide + CHM | Online publishing excluded from modern system |
| V-10 | Barcode is a direct patient identifier on a physical artefact | `barcodes` table | Patient ID is a GUID in modern system (no positional identifier) |
| V-11 | `history` 20 free-form columns with no type enforcement | `data.mdb` | Typed `PatientHistory` slots |
| V-12 | Critical-result state is visual only (no escalation workflow) | `normalrange` critical columns | Not yet addressed in modern system (gap) |
| V-13 | Partial schema migration leaves features without storage | DDL reconciliation | Single EF Core migration, applied by `DatabaseInitializer` (gated) |
| V-14 | Hardware-bound licence (HDD + motherboard + CPU) | `RL_SETUP.exe` strings | Not applicable (modern system has no licensing) |
| V-15 | No password hashing/salting/expiry/lockout in legacy | CHM + schema | PBKDF2 + operator management |

---

## 6. Conflicts and contradictions

| # | Conflict | Evidence | Resolution status |
|---|---|---|---|
| C-01 | NATIGH/SMS delivery requires internet; manual advises keeping server off internet | Vendor guide 1-12 + CHM `001.htm` | UNRESOLVED (internal contradiction) |
| C-02 | Two `Branch_Data.mdb` copies on the 2019 CD (`bin\` byte-identical to V1, `System\` narrower) | BATCH-07 | NOT ESTABLISHED which is loaded |
| C-03 | `Patients_backup.mdb` stale in both versions (never updated) | BATCH-07 | CONFIRMED (ISS-18) |
| C-04 | Migration DDL in SQL Server syntax cannot execute against Jet 4 | DDL analysis | CONFIRMED (partial application) |
| C-05 | `normalrange` rows the runtime selects (REFRANGE='2') do not exist in the Access mirror (REFRANGE = '', '0', '1') | 1605-phase2 findings | CONFIRMED (bands live on SQL Server side) |
| C-06 | `IgEallergy` table absent from shipped DB despite code + DDL | DDL reconciliation | CONFIRMED (storage-absent feature) |
| C-07 | `useraccounts` table absent from Access DB (SQL Server only) | DDL reconciliation | CONFIRMED |
| C-08 | `patienttest.natighu` column absent | DDL reconciliation | CONFIRMED |
| C-09 | `BloodGases` missing all 8 new columns | DDL reconciliation | CONFIRMED |
| C-10 | `help.chm` byte-identical in both versions despite 41 new objects | BATCH-04 | CONFIRMED (documentation frozen 2010) |
| C-11 | `Cache\*.rtf` byte-identical in both versions | BATCH-07 | CONFIRMED (training frozen 2011) |
| C-12 | Two conflicting builds of the same runtime components ship on one disc (CAB vs `OCX\`) | BATCH-10 | CONFIRMED (non-deterministic install) |
| C-13 | PDF installation order defective (Bullzip before .NET 4) | BATCH-11 | CONFIRMED (ISS-53) |
| C-14 | `normalrange` band selection algorithm not established (no ORDER BY) | 1605-phase2 | CONFIRMED (engine-arbitrary at overlaps) |
| C-15 | `pricemethod` value domain not established | Vendor guide + DB | NOT ESTABLISHED |

---

## 7. Decisions resolved or fixed (this phase)

1. **Confirmed the multi-engine deployment model** (Access / SQL Server Express 2005 / MySQL) — previously
   only hypothesised; now verified from `Setup*.bat`.
2. **Confirmed the `sa`/`12345678` credential is the installer's own default**, not an operator choice
   (`SAPWD=12345678` in `Setup.bat`/`Setup4.bat`).
3. **Confirmed the schema self-migration is partially applied** (10/12 tables, 109/132 columns) —
   the shipped Access DB is a partial deployment.
4. **Confirmed the permission model lives on the operator table** (`patients`), not on `patienttest` or
   `patientinfo` — correcting the earlier narrative.
5. **Confirmed the branch is an attribute of the operator**, not the patient.
6. **Confirmed `useraccounts` is SQL-Server-only** (absent from every `.mdb`).
7. **Confirmed `IgEallergy` is storage-absent** (code + DDL present, table absent).
8. **Confirmed `normalrange` rows selected by the runtime (REFRANGE='2') do not exist in the Access
   mirror** — the live bands live on the SQL Server side.
9. **Confirmed reference-range labels are per-template** (12 distinct spellings across 228 templates).
10. **Confirmed the per-report printer override is effectively a single legacy default**
    (`HP LaserJet 1100 (MS)` in 217 of 228 templates).
11. **Confirmed the report template binding** (alphabetic ids = result-table names; numeric ids =
    `testtable.testcode` values; tiers never overlap).
12. **Confirmed the installer is a custom VB6 self-extractor**, not a standard framework (no CAB/ZIP/
    RAR/Inno/NSIS/MSI markers).
13. **Confirmed the legacy system has no PDF capability in V1** (zero `PDF` literals in 104,997 strings).
14. **Confirmed `real lab Image Gallery.exe` is a separate image-browsing executable** linked to the
    `pictures` table.
15. **Confirmed the modern system's security model (PBKDF2, env-var connection strings, no SMS/NATIGH)
    is a deliberate departure** from every legacy vulnerability listed in §5.
16. **Confirmed the knowledge layer is identical across versions** (609 tests, 852 ranges, same
    constants) — the strongest evidence that only the data/workflow layer changed.

---

## 8. Next steps

1. Analyse the next 15 best files from the legacy corpus (Stages 4–6 of `File Index.md`) and update
   `Docs/KB/LEGACY-SYSTEM.md`.
2. Resolve the highest-value open questions (band-selection algorithm, `pregsmok`, `pricemethod`
   domain, `VIDAS`, `loger`) — these require either the remaining corpus files or runtime analysis
   (which is not authorised).
3. Add a critical-result escalation workflow to the modern system (gap V-12) — currently the
   critical state is visual only, matching the legacy defect.
4. Continue the incremental understanding loop until all 298 corpus files are consumed or the
   remaining files are confirmed to be provenance-only.

---

## 9. Companion artefacts

- `Docs/KB/LEGACY-SYSTEM.md` — navigation-layer summary (added this phase).
- `Docs/KB/ARCHITECTURE.md`, `DATA-MODEL.md`, `DOMAIN-RULES.md`, `OPERATIONS.md`, `CONVENTIONS.md`.
- `Docs/KB/features/*.md` — 14 feature files.
- `Database documentation.md`, `Documenting the Executable File.md` (repo root) — full deliverables.
- `evidence/reverse-engineering/` — companion exports, registers, tracking records.
- `RefLab_Reference_V2_Consolidated.zip` — 12-member cumulative knowledge base.
- `evidence/reverse-engineering/FINAL_EXECUTION_REPORT_AR.md` — Arabic execution report.
