# Legacy Target System — Reverse-Engineering Understanding (V-ROOT-1.00 and V-CD-2019)

**Purpose:** Navigation layer for the modern `Ref Lab` (C#/.NET 8) rebuild. This document
summarises what is known about the two legacy systems that `Ref Lab` replaces.
**Code remains the reference** — this document is navigation, not a substitute.

**Sources:** 15 high-value documents from the legacy reverse-engineering corpus
(`E:\Markdown Source Files`), selected as DIRECTLY_RELEVANT + HIGH usefulness:
`Complete_Reverse_Engineering_Reference_V1.md`, `Complete_Reverse_Engineering_Reference_V2.md`,
`01-architecture.md`, `02-data-model.md`, `04-clinical-rules.md`, `05-deployment.md`,
`08-deployment-installer.md`, `09-vendor-capability-catalogue.md`,
`11-deployment-payload-2019.md`, `13-pdf-print-architecture.md`,
`1605-phase2-findings.md`, `architecture.md`, `Business_Rules_V1.md`,
`Business_Rules_V2.md`, `06-configuration.md`.

---

## 1. Identity

| Attribute | V-ROOT-1.00 | V-CD-2019 (v1.605) |
|---|---|---|
| Main executable | `real lab system.exe` 59.15 MB | `real lab system.exe` 76.29 MB |
| Runtime | `MSVBVM60.DLL` (186 imports) | `MSVBVM60.DLL` (231 imports) |
| VB6 objects | 169 (118 forms, 45 DataReport, 5 modules, 1 class) | 210 (132 forms, 66 DataReport, 9 modules, 3 classes) |
| Declared methods | 6,528 | 8,026 |
| String literals | 104,997 | 270,166 |
| SQL statements | 1,337 | 1,797 |
| PE timestamp | 2013-12-27 | 2013-12-27 (not a discriminator) |
| Vendor | RealLab / AIS, El-Mansoura, Egypt | RealLab / AIS, El-Mansoura, Egypt |

## 2. Layered architecture (both versions)

```
L0 Wrapper    PE32, MSVBVM60.DLL, ActiveX controls, database providers
L1 Identity   VB6 forms / DataReport / modules / classes
L2 Logic      methods, literals, SQL, embedded DDL (V2 only: 268 statements)
L3 Data       Jet 4 MDB (+ SQL Server Express 2005 + MySQL in V2)
L4 Behaviour  documented capabilities (15 in V1, ~60 in V2)
```

## 3. Data model

- V1: `Patients.mdb` 93 tables / 1,527 columns / 3,123 rows; `Branch_Data.mdb` 49 schema-only tables; `data.mdb` 3 tables.
- V2: `Patients.mdb` 103 tables / 1,682 columns; +10 tables, +155 columns vs V1.
- Core tables (both): `patientinfo` (order/visit header, 33 cols), `patienttest` (transactional centre, 44 cols), `testtable` (609 tests), `normalrange` (852 ranges), `payment`, `Gehats` (referring parties), `history` (20 free-form slots), `constanttable` (CBC/PT constants), `converts` (167), `abbreviations` (650), `subgrouptable` (46), `testunits` (69).
- V2 additions: `Helicobacter`, `insulin`, `IgEallergy`, `patienttestgroup`, `SMSSender`, `SMStemplate`, `smsgroups`, `smslist`, `MyLabSetting`, `testtemp`, `Paste Errors`.
- V2 extended `patienttest` with `Timeout`, `Taketime`; `patientinfo` with `gehaname` (referring-party name) and `branch`.
- Knowledge layer is **identical** in both versions (609 tests, 852 ranges, same constants) — strongest evidence that only the data/workflow layer changed.

## 4. Key clinical rules

- Result classification is driven by `normalrange` (low/high limits, sex, age band), not hard-coded.
- Critical limit overrides abnormal classification (visual only — no escalation workflow evidenced).
- Haematocrit `Hct = Hgb × 3.3`; Hgb% factors 6.25/6.75/7.5/8.25 by age/sex; PT control 12 s (secondary 35 s); ISI 1.33; Westgard `2CVJW`.
- Only reviewed results (`resultreview`) are releasable/printable.
- Per-test frozen copy of the range on the test row (`normalrang`, `lowlimit`, `highlimit`, `nolowexe`, `nohighexe`) — 112 of 222 rows carry it.
- `normalrange.pregsmok` populated on all 852 rows (OBSERVED, not documented).
- Reference-range band selection algorithm is NOT ESTABLISHED (no ORDER BY in the runtime selector; engine-arbitrary at overlaps).

## 5. Identifier contract

`patientid` is Text(13) and is an **order/visit identifier**, not a patient key. There is no patient master table.
- Positions 3–4: person key (`MID(patientid,3,2)`).
- Position 9: branch code in V2 (`MID(patientid,9,1)`), from the operator's `patients.branche`.
- Positions 10–12: display code (`mid(patientid,10,3)`).
- The identifier layout is a hard inter-version data contract.

## 6. Multi-engine / deployment

- Three database engines: Access Jet 4, SQL Server Express 2005, MySQL (Connector ODBC 3.51), selected at install time via `BASE.ini` `Databasetype=X…X`.
- V2 installer `RL_SETUP.exe` v19.00 (build 2020-09-01), VB6, 57 objects, 599 methods, hardware-bound licence (HDD + motherboard + CPU).
- Default `sa` password `12345678` is hardcoded in the installer (`SAPWD=12345678`) and shipped in `BASE.ini` / `data_connection*.udl`.
- Network protocols NOT disabled (`DISABLENETWORKPROTOCOLS=0`).
- Branch model: master laboratory + reception branches; branch code at position 9; `Branch_Data.mdb` (49 schema-only tables, 0 rows) seeds a new branch.
- Two copies of `Branch_Data.mdb` ship on the 2019 CD: `bin\` (byte-identical to V1) and `System\` (narrower, 35 vs 46 columns).
- V2 ships a runtime schema self-migration: 268 embedded DDL statements (12 CREATE TABLE, 228 ALTER ADD, 13 ALTER COLUMN) in SQL Server syntax with Access-compatible variants. Reconciliation shows 10 of 12 tables and 109 of 132 column targets applied; 2 tables (`FMF`, `LBC`) and 19 columns absent from the shipped DB. `IgEallergy` and `useraccounts` tables absent entirely.

## 7. PDF / printing (V2 only)

- V1 has **zero** PDF capability. V2 adds PDF via an external printer driver: the literal `Bullzip PDF Printer` at offset `0x6A2486` and `Bullzip PDF Printer 11.1 - Create File` at `0x6A255E`.
- Output categories: `D:\PDF\Contracts\`, `D:\PDF\Free\`, `D:\PDF\Individual\`, `D:\PDF\LabToLab\`.
- Six printer roles: `reportprinter`, `reportprinterx`, `zarfprinter`, `barcodeprinter`, `waslprinter` (V2), `IDprinter` (V2).
- Print audit: `resultprint`, `printuser`, `printdate`, `printnum` (origin `reportstart=1984`).
- Quality gate: «Not found any Verified selected tests or result to print it».
- No fallback evidenced if Bullzip is absent or renamed.

## 8. Security findings (both versions)

- Database password `12345678` stored in clear text in `BASE.ini`, `data_connection1.udl`, and the installer.
- Connection as `sa` (full administrative rights) on the SQL Server path.
- Default `admin` account with password equal to the user name, documented in the manual.
- `PersistSecurityInfo=True` re-supplies the credential on every connection.
- Connection strings leak the pre-rebranding product name (`BF6500` / `AUTOMATIC HEMATOLOGY ANALYZER`) and the installation layout (`D:\Lab_data\`).
- Shared, immutable credential across all clients.
- V2 adds `[lastlog]` in `BASE.ini` logging the last 10 connection attempts including the password in clear text.
- `SMSSender` stores mobile numbers and message bodies in clear text with no evidenced retention limit.
- NATIGH digital delivery contradicts the manual's advice to keep the server off the internet.
- No evidence of password hashing, salting, expiry, lockout, or session timeout in the legacy system.

## 9. Configuration surface

- `BASE.ini`: engine selection, data path, connection identity, last range id (`nrls`/`drls`).
- `SETTING.ini`: printer roles, paper size (`A4`), report header text (`Textr1txt=المنظومة البرمجية`, `Textr2txt=ريال لاب سيستم`), toggles (`labbranchx=1`, `barcodeauto=1`, `printworkpaper=1`, `historytype=0`), margin positions.
- `IDCARD.INI`: card/invoice layout and text (address, phone, website, e-mail, Facebook).
- `REPORT.dll`: a 562-byte text file holding a 19-entry BGR colour palette (not a binary).
- `Data\*.ini`: 223 files / 114 clinical profiles — per-profile report layout (7 column positions/widths, 24 colours, per-position font, print flags) plus `reportprinterx`.
- `Cache\*.rtf`: 29 method sheets (byte-identical in both versions).
- `Cache\*.exe`: 13 Flash training simulators (byte-identical in both versions).
- `help.chm`: 15 topics, byte-identical in both versions (stale: documents V1 only).

## 10. What the modern Ref Lab already addresses

The modern `Ref Lab` (C#/.NET 8, Clean Architecture, MediatR, EF Core 8) already implements:
- Single-site laboratory (no branch column, no branch table — branch/multi-site deliberately dropped).
- PBKDF2-HMAC-SHA256 (210,000 iterations, 16-byte salt, constant-time compare) replacing the clear-text `sa`/`12345678`.
- No SMS/NATIGH/online publishing/WhatsApp/email-sending (explicitly excluded and guarded by 6+ architecture tests).
- QuestPDF-based PDF generation replacing the external Bullzip printer dependency.
- 27 EF Core tables, 8 FKs, 27 indexes, no seeding, one migration.
- 73 use cases, 14 screens, 15 permissions, 4 behaviours, 18 guards.
- Environment-variable-driven connection strings (`REFLAB_CONNECTION_STRING`), no `appsettings.json`.
- Architecture boundary tests (Domain references only BCL; Application no persistence; Presentation no persistence/PDF).

## 11. Remaining gaps in understanding

See `LEGACY-UNDERSTANDING-REPORT.md` §4 for the full gap register. Summary:
- `VIDAS` database purpose and structure UNKNOWN.
- `win32_processor` table purpose UNKNOWN (likely WMI licensing probe).
- `loger` table (attendance vs clinical events) UNKNOWN.
- `Patient_samples` vs sample store UNKNOWN.
- `grant13..grant17` and `useraccounts.gtype/hdd1/hdd2` semantics UNKNOWN.
- `LBC` clinical profile UNKNOWN.
- `insulin` and `antibio` vs `antibiotic` UNKNOWN.
- `Samples`/`reportview`/`mylabsetting` shared-with-V1 question RESOLVED (V2 additions).
- `pricemethod` value domain NOT ESTABLISHED.
- Band-selection algorithm among overlapping reference ranges UNKNOWN.
- `pregsmok` dimension UNKNOWN.
- `IgEallergy`/`FMF`/`LBC` tables absent from shipped DB (schema-present, storage-absent).
- `natighu` column absent (NATIGH flag has no storage).
- `BloodGases` missing all 8 new columns.
- `useraccounts` table absent from Access DB entirely.
- `Patients_backup.mdb` stale in both versions (never updated).
- `Branch_Data.mdb` in `bin\` byte-identical to V1; `System\` copy narrower — which is loaded NOT ESTABLISHED.
- 148 of 298 legacy corpus files not yet indexed (Stages 4–6 of `File Index.md`).

## 12. Companion documents

**The canonical legacy knowledge base** — read `Docs/KB/legacy/SPECIFICATION.md` first.

| Subject | Canonical document |
|---|---|
| The reference system as a whole | `Docs/KB/legacy/SPECIFICATION.md` |
| Architecture, executable, object model | `Docs/KB/legacy/architecture/OVERVIEW.md` · `architecture/EXECUTABLE.md` |
| Database schema and migration engine | `Docs/KB/legacy/data/DATABASE.md` · `data/MIGRATION.md` |
| Clinical rules and the reference-range screen | `Docs/KB/legacy/clinical/RULES.md` · `business/WORKFLOWS.md` |
| Version identity and evolution | `Docs/KB/legacy/VERSIONS.md` |
| Source traceability, coverage and validation | `Docs/KB/legacy/evidence/SOURCES.md` · `Docs/KB/legacy/evidence/COVERAGE.md` · `Docs/KB/legacy/evidence/VALIDATION.md` |
| Gap analysis and the transformation plan | `Docs/KB/legacy/mapping/GAP-ANALYSIS.md` · `mapping/TRANSFORMATION.md` |

**Modern-side companions**

- `Docs/KB/ARCHITECTURE.md`, `DATA-MODEL.md`, `DOMAIN-RULES.md`, `OPERATIONS.md`, `CONVENTIONS.md`.
- `Docs/KB/features/*.md` — 14 feature files.
- `Database documentation.md`, `Documenting the Executable File.md` (at repo root) — full reverse-engineering deliverables.
- `evidence/reverse-engineering/` — companion exports, registers, tracking records.
- `RefLab_Reference_V2_Consolidated.zip` — 12-member cumulative knowledge base.
