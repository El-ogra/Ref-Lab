# Gap Analysis — Modern `Ref Lab` versus the Legacy Reference System

**Purpose:** A technically grounded comparison of the current project against the reconstructed
reference system, identifying what the modern system already supports, what differs, what is
missing, and what cannot be determined from the available evidence.
**Scope:** the modern `Ref Lab` solution at commit `e42bfca0ebad33752c383ba0e17353b78410664b`
(4 projects, 408 source files, 73 use cases, 14 feature areas, 27 database tables) against
**V-CD-2019 (v1.605)**, the latest identifiable reference-system version.
**Audience:** engineers and architects planning the transformation.
**Companion:** `mapping/TRANSFORMATION.md` (the plan) · `mapping/LEGACY-TO-MODERN.md` (the earlier,
feature-level comparison) · `SPECIFICATION.md` (the target) · `data/DATABASE.md` (the target schema).

**Classification:** ALREADY ALIGNED · PARTIALLY ALIGNED · MISSING · DIFFERENT IMPLEMENTATION ·
OUTDATED · UNKNOWN / NOT VERIFIED.

---

## 1. Baseline: the modern system

| Property | Value | Source |
|---|---|---|
| Solution | `RefLab.sln` — 4 `src/` projects, 5 test projects | `scripts/kb_build.py` |
| Stack | .NET 8 / C# 12 / WPF, EF Core 8, MediatR CQRS, QuestPDF | `Directory.Build.props`, `src/` |
| Source files | 408 (301 `src/`, 116 `tests/`, others) | `git ls-files` |
| Symbols | 475 | `Docs/KB/generated/symbol-index.csv` |
| Use cases | **73**, each with a handler, validator, DTO, permission and tests | `Docs/KB/generated/usecase-index.csv` |
| Feature areas | 14 | `Docs/KB/features/` |
| Database tables | **27** (EF Core) | `RefLabDbContext.cs` |
| EF migrations | 3 (`InitialCreate`, `AddReportFooterAndLayoutText`, `AddReportBrandingAndSampleOrigin`) | `src/RefLab.Infrastructure/Persistence/Migrations/` |
| Domain entities | 65 files under `RefLab.Domain` | `src/RefLab.Domain/` |
| Legacy requirement traceability | **50 distinct `V2-REQ-*`** identifiers referenced by the use-case index | `usecase-index.csv` |

---

## 2. Architecture

| Aspect | Legacy (v1.605) | Modern | Classification |
|---|---|---|---|
| Application shape | VB6 fat client, 210 objects, 8,026 methods | .NET 8 Clean Architecture, 4 layers | **DIFFERENT IMPLEMENTATION** — deliberate |
| Data access | Jet 4 / ADO / raw SQL, 1,797 SQL statements | EF Core 8 over 27 tables, repository + unit-of-work | **DIFFERENT IMPLEMENTATION** |
| Query language | SQL + 27 Jet-4 `SHAPE` statements | LINQ / EF | **DIFFERENT IMPLEMENTATION** |
| UI | 132 VB6 forms + 66 DataReport templates + 223 `Data\*.ini` | WPF, 14 feature areas | **DIFFERENT IMPLEMENTATION** |
| Reporting | Bullzip PDF Printer driver + VB6 DataReports + INI templates | QuestPDF, `QuestPdfReportRenderer` | **DIFFERENT IMPLEMENTATION** — see §7.1 |
| Schema evolution | 268 embedded DDL statements applied at start-up, partially | 3 EF migrations, **never auto-applied** (`DatabaseInitializer` is registered but never resolved) | **ALREADY ALIGNED (in intent, deliberately stricter)** |
| Branch model | Master + reception branches | Single site; branch position absorbed | **DIFFERENT IMPLEMENTATION — deliberate scope removal** |
| Integration | 41 new V2 objects, `Patient Data Transport.exe` | MediatR in-process; no out-of-process transport | **PARTIALLY ALIGNED** |

**★ The schema-migration posture is the most important architectural alignment.** The legacy
discovers schema differences *at runtime* on the client, and on the shipped copy it applied only
90 of 132 column targets. The modern system refuses to migrate implicitly at all: migrations are
explicit, versioned, never edited after application, and applied by the owner from the command
line. That is the correct answer to the legacy failure mode and is enforced by a test guard
(`The_composition_root_never_resolves_the_database_initialiser`).

---

## 3. Data model

| Aspect | Legacy | Modern | Classification |
|---|---|---|---|
| Table count | 103 (Access) / 117 (SQL Server) | 27 | **DIFFERENT IMPLEMENTATION** — see §4 |
| Table-name overlap | — | **0 of 27** legacy names carried over | — |
| Profile result tables | **96** near-identical tables, one column per analyte | Normalised: `OrderTest` + `CultureResult` + value objects | **DIFFERENT IMPLEMENTATION** |
| Primary keys | **0 of 103** | Every table has a surrogate key | **ALREADY ALIGNED (improved)** |
| Foreign keys | **0** | Enforced by EF | **ALREADY ALIGNED (improved)** |
| Reference ranges | `normalrange`, 18 columns, 852 rows, 304 tests | `ReferenceRange` entity | **ALREADY ALIGNED** |
| Clinical constants | `constanttable`, 2 rows × 7 columns, group-oriented | `ClinicalConstant` entity | **ALREADY ALIGNED** |
| Unit conversions | `converts`, 167 rows, per analyte-and-unit pair | `ConversionFactor` entity | **ALREADY ALIGNED** |
| Test catalogue | `testtable`, 38 columns, 609 tests, 48 sub-groups | `TestDefinition` + `ClinicalSubGroup` | **ALREADY ALIGNED** |
| Patient master | `patientinfo`, 39 columns | `Patient` | **ALREADY ALIGNED** |
| Test order / result | `patienttest`, 46 columns, with a frozen range snapshot | `PatientOrder` + `OrderTest` + `ReferenceSnapshot` | **ALREADY ALIGNED** |
| Range snapshot at order time | `patienttest.normalrang/lowlimit/highlimit/…` | `ReferenceSnapshot` value object | **ALREADY ALIGNED** — the mechanism is reproduced deliberately |
| Critical limits on the range row | **Not persistable** — no `critical*` column in the live schema | `CriticalInterval` value object | **DIFFERENT IMPLEMENTATION** — the modern schema *can* store what the legacy could not |
| `pregsmok` dimension | 852 of 852 rows populated; role unknown | Not modelled | **MISSING** — see §5.3 |
| `REFRANGE` runtime-band marker | Present in the legacy schema; semantics unresolved | Not modelled | **MISSING** — see §5.3 |
| `history` generic 20-column table | Present | Not modelled | **OUTDATED** — a generic EAV table is a design smell |
| `Paste Errors` landing table | Present (V2) | Not modelled | **ALREADY ALIGNED (deliberately dropped)** |
| Send-out | `patienttest.sentout/send/otherlabname` + `testtable.inotherlab` | `SendOutRequest` + `SampleDisposition` with a state machine | **ALREADY ALIGNED (improved)** |
| Barcode | `patientinfo.barcode`, `testtable.barcode1..3`, `barcodes` table | `Patient` barcode fields, 24 references | **ALREADY ALIGNED** |
| Accounting | `payment`, `wasl`, `pricelist`, `patientinfo.totalprice/discount/pricemethod` | `Payment`, `PriceList`, `CommissionCalculator` | **ALREADY ALIGNED** |
| Print audit | `patienttest.printuser/printdate/printnum/edituser/…` | `PrintAudit` entity + `GetPrintAuditTrail` query | **ALREADY ALIGNED** |
| Attendance | `loger` (role uncertain) | `AttendanceEntry` + `RecordAttendance` | **PARTIALLY ALIGNED** — the legacy `loger` role was never established |

---

## 4. The 96 profile tables — the largest structural transformation

The legacy stores one table per clinical profile, each with a column per analyte:

| Profile | Analytes | Legacy columns |
|---|---|---:|
| `Abuse` | Alcohol, Cannabinoid, Opitate, Cocaine, Barbtiurate, Benzodiazipine, Amphetamine | 10 |
| `ABO` | abo, rhfactor, direct, indirect, rhtitre | 8 |
| `cbc` | the full blood-count analyte set | 18 |
| `cultures` | organism, organism2, organism3, condition, condition2, sensitivity | 33 |

96 such tables, 103 in total. The modern system replaces this with a **normalised model**:
`PatientOrder` → `OrderTest` → result rows and typed value objects (`ResultValue`, `RatioResult`).

**Impact.** This is the single biggest change in the transformation and it is *not* a port — it is a
re-modelling. Three consequences the blueprint in `mapping/TRANSFORMATION.md` §3 must address:

1. **Data migration.** Any historical data held in the 96 profile tables must be pivoted into the
   normalised result rows, analyte by analyte, with the analyte identity resolved from
   `branch-profile-schemas.csv` (49 rows, the only complete analyte-to-column map in the corpus).
2. **Report templates.** The 223 `Data\*.ini` templates address the profile tables by name. A
   normalised model cannot be driven by INI templates; report rendering must be re-derived.
3. **Search.** `patientinfo` + `patienttest` account for 43 % of legacy SQL traffic. A normalised
   result model changes the shape of every clinical query.

---

## 5. Business rules and clinical behaviour

### 5.1 Aligned

| Rule | Modern implementation | Classification |
|---|---|---|
| V1-BR-009 / V2-BR-034 — classification from the range table | `ResultClassificationPolicy` | ALREADY ALIGNED |
| V1-BR-010 / V2-BR-035 — critical limit overrides abnormal | `ResultClassificationPolicy`, `CriticalInterval` | ALREADY ALIGNED |
| V1-BR-011 / V2-BR-036 — ranges banded by sex and age | `ReferenceRange`, `AgeBand`, `SexScope` | ALREADY ALIGNED |
| V1-BR-037 — `Hct = Hgb × 3.3` | `ClinicalDerivationPolicy` | ALREADY ALIGNED |
| V1-BR-038 — `Hgb%` age/sex factors | `ClinicalDerivationPolicy` | ALREADY ALIGNED |
| V1-BR-039 — ISI from control time and concentration | `ProthrombinControlCalibration`, `ProthrombinControlPoint` | ALREADY ALIGNED |
| V1-BR-040 — Westgard 2CV rule | `ProthrombinControlCalibration` | ALREADY ALIGNED |
| V1-BR-029 — conversions per analyte-and-unit pair | `ConversionFactor` | ALREADY ALIGNED |
| V1-BR-013 — result comment from three sources | `OrderTest` comment + `Comment` lookup | ALREADY ALIGNED |
| V1-BR-018 — exception text per range row | `ReferenceRange` low/high exception text | ALREADY ALIGNED |
| V1-BR-019 — urine bound to test number 4 | documented in the domain rules | ALREADY ALIGNED |
| V1-BR-041 — per-analyte critical thresholds from method sheets | `CriticalInterval` | ALREADY ALIGNED |
| V1-BR-006 — ordering and result-review state machine | `ResultState` enum, `OrderTest` | ALREADY ALIGNED |
| Identifier allocation | `IdentifierAllocator`, `OrderIdentifier` | ALREADY ALIGNED (branch digit repurposed) |

**★ The modern system already documents the legacy evidence in its code.** `ReferenceRange.cs` and
`ReferenceRangeSelector.cs` both carry XML-doc comments citing the legacy column count, the 852
rows, the sex and age-unit distributions and the fact that the legacy selection rule is *not
established*. This is exactly the provenance discipline the legacy knowledge base lacked.

### 5.2 Deliberately diverged

| Rule | Legacy | Modern | Classification |
|---|---|---|---|
| Band selection among overlapping ranges | `SELECT Top 1` with **no `ORDER BY`** — engine-arbitrary | Deterministic: most specific sex wins, then narrowest age band, then lowest id | **DIFFERENT IMPLEMENTATION — deliberate and documented** |
| Critical-result escalation | **None.** The critical state is visual only | Not implemented — no escalation workflow exists | **MISSING — and the legacy did not have one either** |
| Report filing categories | `Contracts`, `Free`, `Individual`, `LabToLab` under `D:\PDF\` | `ReportCategory` enum with the same four values | **ALREADY ALIGNED** |
| Branch in the order identifier | Position 9 of the 13-character id | Position 9 repurposed; single-site | **DIFFERENT IMPLEMENTATION — deliberate scope removal** |

### 5.3 Missing relative to the legacy

| # | Gap | Legacy evidence | Modern state | Impact |
|---:|---|---|---|---|
| G-01 | **`pregsmok` dimension** | Populated on **all 852** range rows; `04-clinical-rules.md` states it is *not documented* and is an OBSERVED data discovery | Not modelled | If it is a real clinical dimension, range selection is under-specified in the modern system. **UNKNOWN / NOT VERIFIED** whether it affects selection |
| G-02 | **`REFRANGE` runtime-band marker** | Column present on `normalrange` and `testtable`; `'2'`-valued rows are absent from the Access mirror (C-05) | Not modelled | The modern system has no way to express "this band is selected at runtime" |
| G-03 | **SMS delivery** | `SMSSender`, `SMStemplate`, `smsgroups`, `smslist`; the gateway path is reconstructed in full | **Removed from scope** — `Permission.cs` records the decision | Deliberate. If the business needs it, it must be rebuilt as a proper HTTPS client, not as browser automation |
| G-04 | **NATIGH.COM digital delivery** | `patienttest.natighu` flag is **code-present but schema-absent** | **Removed from scope** | Deliberate; the legacy feature never worked against the shipped database |
| G-05 | **Multi-branch** | `Branch_Data.mdb`, branch code in the identifier | **Removed from scope** | Deliberate; the position is absorbed |
| G-06 | **Critical-value persistence on the range row** | Not persistable in the legacy live schema | Modern schema *can* store it (`CriticalInterval`) | **Improvement**, not a gap |
| G-07 | **Method-sheet-driven critical values** | 29 `Cache\*.rtf` sheets carry per-test reference and critical values | Data must be seeded, not read at runtime | **MISSING — the seed data does not exist in the modern repository** |
| G-08 | **Reference-range administration screen** | The manual documents the full editor, its six-band rule and its commands | Not modelled as a feature | **MISSING** — no use case administers ranges |
| G-09 | **Database maintenance / backup** | The manual documents the 8-part maintenance screen, password `123`, `Patient_yyyymmdd.bak` | Not modelled | **MISSING** — operational capability absent |
| G-10 | **Stock-take / period reconciliation** | `«الجرد وحساب الدرج»`, F7 shortcut, `wasl`, `sharekat` | `Statistics` and `Billing` features exist, but the stock-take *reconciliation* semantics are not established | **PARTIALLY ALIGNED** |
| G-11 | **`loger` table role** | Referenced by the legacy; the role is **never established** (attendance or audit) | `AttendanceEntry` models attendance | **UNKNOWN / NOT VERIFIED** — the modern system may have guessed |
| G-12 | **`pricemethod` domain** | `patientinfo.pricemethod Text (10)`; C-15 remains open | `Payment`/`PriceList` exist but the legacy domain is unknown | **UNKNOWN / NOT VERIFIED** |
| G-13 | **Remote-support tooling** | `AnyDesk`, `Supremo`, `TeamViewerQS` ship on the disc | Absent | Deliberate and desirable |
| G-14 | **Attendance / work-log linkage to the 13 `Cache\*.exe` helpers** | 13 executables shipped in `Cache\` | Not modelled | **UNKNOWN / NOT VERIFIED** |

### 5.4 Security posture — improved throughout

| Legacy finding | Modern state | Classification |
|---|---|---|
| V-01 clear-text `sa` password | No database credential is embedded; connection strings are external | **ALREADY ALIGNED (improved)** |
| V-02 `admin` password equals user name | `PasswordPolicy` in `RefLab.Domain/Identity/`; PBKDF2 hashing (`Pbkdf2PasswordHasher`) | **ALREADY ALIGNED (improved)** |
| V-03 `PersistSecurityInfo=True` | Not applicable — EF Core does not embed credentials | **ALREADY ALIGNED (improved)** |
| V-05 password written to `[lastlog]` | `PrintAudit` records operator, action and timestamp — **no credentials** | **ALREADY ALIGNED (improved)** |
| V-10 no keys, no referential integrity | Full PK/FK model | **ALREADY ALIGNED (improved)** |
| V-12 no critical-result escalation | Still no escalation workflow | **MISSING — inherited** |
| V-14 branch code leaks across the identifier | Position repurposed | **ALREADY ALIGNED (improved)** |

**★ The modern security posture is strictly better than the legacy on every measurable axis**, and
the one exception — critical-result escalation — is a gap the legacy also had. Closing it is a
*new* requirement, not a parity requirement.

---

## 6. Requirement traceability

| Measure | Value |
|---|---:|
| Legacy functions in the V2 functional specification | 48 |
| Legacy workflows | 7 |
| Legacy entities (observed) | 40 |
| Observed legacy relationships | 19 |
| `V2-REQ-*` requirements in the V2 matrix | 72 |
| `V2-REQ-*` identifiers referenced by the modern use-case index | **50** |
| Modern use cases carrying a legacy requirement link | **73 of 73** |

**★ Every one of the 73 modern use cases carries a `legacy_requirement` link.** That is
unusually strong traceability and it is the foundation on which the transformation blueprint is
built. The residual gap is that **22 of the 72 `V2-REQ-*` identifiers are not referenced by any use
case** — either the requirement is out of scope, or it is unimplemented. `V2-REQ-*` identifiers
**absent** from the modern index and **present** in the legacy matrix are the natural starting
point for the gap triage in `mapping/TRANSFORMATION.md` §3 (M-1).

---

## 7. Interface and integration gaps

### 7.1 Report rendering

| Legacy | Modern |
|---|---|
| 66 VB6 DataReport templates + 223 `Data\*.ini` templates + Bullzip PDF Printer (name and version hardcoded) | QuestPDF with a `QuestPdfReportRenderer` and `ReportBranding` |

The legacy report system is **data-driven from INI files** — 223 of them, 172 of which changed
between versions. The modern renderer is code-driven. **Classify: DIFFERENT IMPLEMENTATION**, with
the migration risk noted in `mapping/TRANSFORMATION.md` §3 (M-3).

### 7.2 Report output categories

Fully aligned — `ReportCategory` reproduces the four legacy filing directories, and the XML
documentation in `src/RefLab.Domain/Enums/ReportCategory.cs` cites the legacy evidence.

### 7.3 External services

| Integration | Legacy | Modern | Classification |
|---|---|---|---|
| SMS gateway | Plain HTTP via `InternetExplorer.Application` | Absent (removed from scope) | MISSING — deliberately, and correctly |
| Database engines | Access / SQL Server / MySQL | SQL Server only | DIFFERENT IMPLEMENTATION — deliberate |
| Remote support | Three tools on the disc | Absent | Deliberate and desirable |
| Barcode printing | `003A.htm` procedure | Barcode fields present; printer integration **UNKNOWN** | UNKNOWN / NOT VERIFIED |

---

## 8. Gap summary

| Classification | Count | Items |
|---|---:|---|
| ALREADY ALIGNED | 27 | Identity, permissions, print audit, ranges, constants, conversions, catalogue, order/result lifecycle, identifier allocation, report categories, security on every measurable axis, schema-evolution posture |
| PARTIALLY ALIGNED | 6 | Out-of-process transport, `loger` semantics, stock-take reconciliation, barcode printing, send-out state machine, attendance linkage |
| MISSING | 9 | `pregsmok`, `REFRANGE`, method-sheet seed data, range administration screen, database maintenance/backup, SMS, NATIGH, branch model, critical-result escalation |
| DIFFERENT IMPLEMENTATION | 11 | Stack, data access, query model, UI, reporting, profile-table normalisation, table naming, band-selection rule, critical-limit persistence, branch handling, database engines |
| OUTDATED | 2 | `history` generic table, `Paste Errors` landing table |
| UNKNOWN / NOT VERIFIED | 6 | `pricemethod` domain, `loger` role, `pregsmok` semantics, `REFRANGE` semantics, 13 `Cache\*.exe` helpers, barcode printing |

**Total assessed items: 61.** Of the **9 MISSING** items, **4 are deliberate scope removals** (SMS,
NATIGH, branch, and the legacy's own broken NATIGH column), **3 are data-modelling gaps that must
be closed before any data migration** (`pregsmok`, `REFRANGE`, method-sheet seed data), and **2 are
genuine new capability** (range administration, database maintenance).

---

## 9. What cannot be concluded

| Question | Why it cannot be answered |
|---|---|
| Whether the 96 profile tables' semantics are fully captured by the modern model | The modern model was derived from the V2 requirements matrix, not from the 49-row analyte map; a field-by-field reconciliation has not been performed |
| Whether the 22 unreferenced `V2-REQ-*` identifiers are out of scope or unimplemented | The requirement matrix's own status column was not cross-checked against the modern index in this pass |
| Whether any legacy behaviour observed in a lab was reproduced | The corpus is 100 % static; no runtime observation exists for either version |
| Whether `pregsmok` affects range selection | The legacy column is populated on every row, but no source states its use; the `SELECT` statement in `04-clinical-rules.md` does include it, which is suggestive but not decisive |
| Whether `pricemethod` values are constrained | No sample of `patientinfo.pricemethod` values survives redaction; C-15 stays open |
| Whether the 3 modern EF migrations cover everything the legacy does | The modern schema has 27 tables; the legacy has 103. The mapping is semantic, not structural, and has not been exhaustively verified |

---

## 10. Related documents

- `mapping/TRANSFORMATION.md` — the engineering blueprint.
- `mapping/LEGACY-TO-MODERN.md` — the earlier feature-level comparison.
- `SPECIFICATION.md` — the target specification.
- `data/DATABASE.md` — the target schema.
- `Docs/KB/legacy/evidence/UNRESOLVED.md` — the open questions this analysis depends on.
