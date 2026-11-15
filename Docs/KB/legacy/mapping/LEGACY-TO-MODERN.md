# Legacy-to-Modern Mapping — Ref Lab

**Purpose:** Compare legacy system behaviour with the modern Ref Lab implementation.
**Scope:** Architecture, database, identifiers, clinical rules, workflows, security, configuration,
deployment, reporting/PDF, external integrations.
**Audience:** Architects, developers.
**Version applicability:** Mixed.

---

## 1. How to read this document

For each meaningful difference, four categories are distinguished:

1. **Original legacy behaviour** — how the legacy system actually worked.
2. **Reverse-engineering findings** — what was discovered about that behaviour.
3. **Actual modern Ref Lab implementation** — what Ref Lab does.
4. **Intentional architectural differences** — whether the difference is intentional and why.

For each difference:
- Legacy behaviour.
- Modern implementation.
- Whether the difference is intentional or simply observed.
- Documented reason, if available (`RATIONALE NOT DOCUMENTED` if not).
- Relevant legacy source.
- Relevant modern KB source.
- Confidence / verification status.

**Do not invent architectural decisions or rationales.** If the modern implementation differs but
its rationale is undocumented, state `RATIONALE NOT DOCUMENTED`.

---

## 2. Comparison matrix

### 2.1 Architecture

| Aspect | Legacy behaviour | Modern Ref Lab | Intentional? | Rationale | Legacy source | Modern source |
|---|---|---|---|---|---|---|
| Runtime | VB6 native, `MSVBVM60.DLL` | .NET 8, WPF, MediatR, EF Core | Yes | Modern stack migration (project mandate) | `Complete_Reverse_Engineering_Reference_V1/V2` §2 | `Docs/KB/ARCHITECTURE.md` |
| Layers | 5-layer (wrapper/identity/logic/data/behaviour) | 4-layer Clean Architecture (Domain/Application/Infrastructure/Presentation) | Yes | Clean Architecture mandate | `Complete_Reverse_Engineering_Reference_V1/V2` §2.1 | `Docs/KB/ARCHITECTURE.md` |
| Object count | 169 (V1) / 210 (V2) VB6 objects | 73 MediatR use cases, 14 screens | Yes | Reimplementation, not port | `Complete_Reverse_Engineering_Reference_V1/V2` §2 | `Docs/KB/START-HERE.md` |
| DB access | Raw SQL strings, no ORM | EF Core 8 code-first | Yes | Modern data access | `Complete_Reverse_Engineering_Reference_V1` §5.6 | `Docs/KB/DATA-MODEL.md` |
| Seeding | (none in either) | None (`DatabaseInitializer` migrates only) | Yes | Gated migration policy | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md §A.5 | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md §A.5 |

### 2.2 Database

| Aspect | Legacy behaviour | Modern Ref Lab | Intentional? | Rationale | Legacy source | Modern source |
|---|---|---|---|---|---|
| Engine | Access Jet 4 / SQL Server Express 2005 / MySQL | SQL Server (EF Core 8) | Yes | Single engine, modern | `Complete_Reverse_Engineering_Reference_V2` §5.4 | `Docs/KB/DATA-MODEL.md` |
| Tables | 93 (V1) / 103 (V2) | 27 | Yes | Redesigned schema | `Database documentation.md` | `Docs/KB/DATA-MODEL.md` |
| Multi-branch | Master + reception branches, branch code at position 9 | Single-site (no branch column, no branch table) | Yes | Owner decision: "المعمل موقع واحد" (AGENTS.md rule 8) | `Complete_Reverse_Engineering_Reference_V2` §3.5, §6.4 | `Docs/KB/DATA-MODEL.md`, `Docs/KB/ARCHITECTURE.md` |
| Schema migration | 268 embedded DDL at start-up (V2); 0 (V1) | Single EF Core migration, applied by `DatabaseInitializer` (gated, never at start-up) | Yes | Gated migration policy (AGENTS.md rule 4) | `Complete_Reverse_Engineering_Reference_V2` §4.2 | `Docs/KB/OPERATIONS.md` |
| Backup | Copy-based (`labdb`/`labdb1`) | (not evidenced in modern system) | RATIONALE NOT DOCUMENTED | — | `Complete_Reverse_Engineering_Reference_V1` §6.8 | — |

### 2.3 Identifiers

| Aspect | Legacy behaviour | Modern Ref Lab | Intentional? | Rationale | Legacy source | Modern source |
|---|---|---|---|---|---|---|
| Patient ID | Text(13) positional composite (person key, branch, display code); no patient master table | GUID (`PatientId` value object) | Yes | Eliminate positional contract fragility; modern identity model | `Business_Rules_V1/V2` (V1-BR-005, V2-BR-008/040) | `Docs/KB/DATA-MODEL.md` |
| Order ID | Part of `patientid` | GUID (`PatientOrder.Id`) | Yes | Modern identity model | (as above) | `Docs/KB/DATA-MODEL.md` |
| Barcode | Direct patient identifier on physical artefact | (not evidenced as barcode in modern system) | RATIONALE NOT DOCUMENTED | — | `Business_Rules_V1` V1-BR-006 | — |

### 2.4 Clinical rules

| Aspect | Legacy behaviour | Modern Ref Lab | Intentional? | Rationale | Legacy source | Modern source |
|---|---|---|---|---|---|---|
| Result classification | Driven by `normalrange` (low/high, sex, age band); three-valued (normal/abnormal/critical) | `ReferenceRangeSelector`, `ResultClassificationPolicy` (Domain Rules) | Yes (preserved) | Preserve clinical logic | `clinical/RULES.md` | `Docs/KB/DOMAIN-RULES.md`, `Docs/KB/features/Results.md` |
| Critical-result escalation | Visual only; no escalation workflow | (not evidenced) | RATIONALE NOT DOCUMENTED | Gap identified (V-12) | `clinical/RULES.md` (V2-BR-035) | — |
| Clinical constants | Stored as data (`constanttable`, Text(50)) | `ClinicalConstant` aggregate, `ClinicalConstantResolver` | Yes (preserved) | Preserve data-driven constants | `clinical/KNOWLEDGE-LAYER.md` | `Docs/KB/features/Catalogue.md` |
| Reference ranges | 852 rows, sex/age banded, frozen on test row | `ReferenceRange` aggregate with `AgeBand`/`Interval` value objects | Yes (preserved) | Modern typed model | `clinical/RULES.md` | `Docs/KB/DATA-MODEL.md` |
| Band-selection algorithm | `SELECT Top 1 …` no ORDER BY (engine-arbitrary) | (not evidenced) | RATIONALE NOT DOCUMENTED | Gap (C-14) | `clinical/RULES.md` (U-02) | — |
| `pregsmok` dimension | Populated on all 852 rows, role unknown | (not evidenced) | RATIONALE NOT DOCUMENTED | Gap | `clinical/RULES.md` (V2-BR-036) | — |

### 2.5 Business workflows

| Aspect | Legacy behaviour | Modern Ref Lab | Intentional? | Rationale | Legacy source | Modern source |
|---|---|---|---|---|---|---|
| Patient lifecycle | Register → barcode → order → enter result → review → print → hand over → (V2) digital delivery | `RegisterPatient`, `AllocateOrderIdentifier`, result entry, `PrintResultReport`, `HandOverResults` | Yes (preserved, minus digital delivery) | Digital delivery excluded (owner decision) | `business/WORKFLOWS.md` | `Docs/KB/features/Patients.md`, `Results.md`, `Reporting.md` |
| Send-out | Flag-driven (`inotherlab`), two settlement states | `SendOutRequest`, `SampleDisposition` | Yes (preserved) | Modernised | `business/WORKFLOWS.md` (WF-03) | `Docs/KB/features/SendOut.md` |
| Accounting/stock-take | Period stock-take, per-user workload, referring-party commission, send-out reconciliation | `GetLaboratoryStatistics`, `GetPerformanceStatistics`, accounting use cases | Yes (preserved) | Modernised | `business/WORKFLOWS.md` (WF-04) | `Docs/KB/features/Billing.md`, `Statistics.md` |
| Reference-range administration | «المعدل الطبيعي» editor, 14 attributes | (not evidenced as a screen in modern system) | RATIONALE NOT DOCUMENTED | — | `business/WORKFLOWS.md` (WF-05) | — |
| Catalogue interlock | `normalrange` ↔ `converts` ↔ `testunits` ↔ `testtable` ↔ `Data\*.ini` ↔ `Cache\*.rtf` | `TestDefinition`, `ReferenceRange`, `TestUnit`, `ConversionFactor` | Yes (preserved) | Modern typed model | `business/WORKFLOWS.md` (WF-06) | `Docs/KB/features/Catalogue.md` |

### 2.6 Authentication and security

| Aspect | Legacy behaviour | Modern Ref Lab | Intentional? | Rationale | Legacy source | Modern source |
|---|---|---|---|---|---|---|
| Password storage | Clear text (`sa`/`12345678`, `admin`/`admin`) | PBKDF2-HMAC-SHA256 (210k iterations, 16-byte salt) | Yes | Security hardening | `business/SECURITY.md` (V-01..V-03) | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md §A.8 |
| Connection string | `BASE.ini`, `data_connection*.udl` (cleartext) | Env var `REFLAB_CONNECTION_STRING` | Yes | Remove cleartext config | `business/SECURITY.md` (V-01, V-04, V-06) | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md §A.2, §A.10 |
| Permission model | 12 (V1) / 17 (V2) bits on operator table | 15 permissions, `IRequirePermission` + `AuthorizationBehavior` | Yes (redesigned) | Modern permission model | `business/SECURITY.md` §1 | `Docs/KB/ARCHITECTURE.md`, `Docs/KB/features/Identity.md` |
| SMS/NATIGH/online publishing | Present (V2) | Explicitly excluded (6+ guards) | Yes | Owner decision (AGENTS.md: removed capabilities) | `business/CAPABILITIES.md` §3 | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md (excluded-feature scan) |
| Barcode as patient ID | Direct identifier on physical artefact | (not evidenced) | RATIONALE NOT DOCUMENTED | — | `business/SECURITY.md` (V-09) | — |
| Hardware-bound licence | HDD + motherboard + CPU | (none) | Yes | Modern deployment model | `business/SECURITY.md` (V-15) | — |
| Print audit | `resultprint`, `printuser`, `printdate`, `printnum` | `PrintAudits` table (`Operator`, `PrintedAt`, `SequenceNumber`, `Category`) | Yes (preserved) | Preserve accountability | `business/WORKFLOWS.md`, `architecture/PDF-PRINTING.md` §5 | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md §A.7 |
| Critical-result escalation | Visual only | (not evidenced) | RATIONALE NOT DOCUMENTED | Gap (V-12) | `business/SECURITY.md` (V-12) | — |

### 2.7 Configuration

| Aspect | Legacy behaviour | Modern Ref Lab | Intentional? | Rationale | Legacy source | Modern source |
|---|---|---|---|---|---|---|
| Config mechanism | `.ini` files (`BASE.ini`, `SETTING.ini`, `IDCARD.INI`, `REPORT.dll`, `Data\*.ini`) | Env var `REFLAB_CONNECTION_STRING`; no `appsettings.json` | Yes | Remove cleartext config; modern config model | `architecture/CONFIGURATION.md` | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md §A.2, §A.10 |
| Report branding | `Textr1txt=المنظومة البرمجية`, `Textr2txt=ريال لاب سيستم` (vendor branding on every report) | `ReportBranding.cs` compile-time constants (Arabic system name, etc.) | Yes | Rebrand; verified labels from `ui-labels-verified.md` | `architecture/CONFIGURATION.md` §4 | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md §A.7 |
| Report layout | 223 `.ini` files, 114 profiles, per-profile printer binding | QuestPDF renderer + per-document layout in code | Yes | Integrated PDF, no external printer | `architecture/CONFIGURATION.md` §8 | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md §A.7 |
| Colour palette | `REPORT.dll` (562-byte text file, 19 BGR values) | QuestPDF styling in code | Yes | Modern rendering | `architecture/CONFIGURATION.md` §6 | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md §A.7 |

### 2.8 Deployment

| Aspect | Legacy behaviour | Modern Ref Lab | Intentional? | Rationale | Legacy source | Modern source |
|---|---|---|---|---|---|---|
| Installer | `RL_SETUP.exe` (VB6, hardware-bound licence, 57 objects) | (none evidenced; `dotnet` run) | RATIONALE NOT DOCUMENTED | Modern deployment model not documented | `architecture/DEPLOYMENT.md` | — |
| Database engines | 3 (Access / SQL Server Express 2005 / MySQL) | SQL Server (EF Core 8) | Yes | Single modern engine | `architecture/DEPLOYMENT.md` §1 | `Docs/KB/DATA-MODEL.md` |
| Licensing | Hardware-bound (HDD + motherboard + CPU) | (none evidenced) | RATIONALE NOT DOCUMENTED | — | `architecture/DEPLOYMENT.md` §7 | — |
| PDF dependency | External `Bullzip PDF Printer` (hardcoded name + version) | Integrated QuestPDF | Yes | Remove external printer dependency | `architecture/PDF-PRINTING.md` | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md §A.7 |

### 2.9 Reporting / PDF

| Aspect | Legacy behaviour | Modern Ref Lab | Intentional? | Rationale | Legacy source | Modern source |
|---|---|---|---|---|---|---|
| PDF generation | External Bullzip driver (OS layer) | QuestPDF (in-process) | Yes | Integrated PDF (requirement from RE findings) | `architecture/PDF-PRINTING.md` | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md §A.7 |
| Report audit | `resultprint`, `printuser`, `printdate`, `printnum` | `PrintAudits` table | Yes (preserved) | Preserve accountability | `architecture/PDF-PRINTING.md` §5 | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md §A.7 |
| Quality gate | "Not found any Verified selected tests…" | (not evidenced) | RATIONALE NOT DOCUMENTED | — | `architecture/PDF-PRINTING.md` §5 | — |
| Output categories | 4 (`Contracts`, `Free`, `Individual`, `LabToLab`) | Single `ExportedDocument` with category (hardcoded `"Individual"` at exporter level) | Yes (simplified) | RATIONALE NOT DOCUMENTED beyond simplification | `architecture/PDF-PRINTING.md` §4 | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md §A.7 |
| Arabic/RTL support | Bullzip `labels.txt [ara]` | `ReportBranding` with `DirectionFromRightToLeft()` (font-dependent) | Yes | Arabic support preserved | `architecture/PDF-PRINTING.md` §4 | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md §A.7 |

### 2.10 External integrations

| Aspect | Legacy behaviour | Modern Ref Lab | Intentional? | Rationale | Legacy source | Modern source |
|---|---|---|---|---|---|---|
| NATIGH.COM delivery | Present (V2) | Excluded | Yes | Owner decision (removed capabilities) | `business/CAPABILITIES.md` §3 | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md (excluded-feature scan) |
| SMS | Present (V2) | Excluded | Yes | Owner decision | `business/CAPABILITIES.md` §3 | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md (excluded-feature scan) |
| Online result publishing | Present (V2) | Excluded | Yes | Owner decision | `business/CAPABILITIES.md` §3 | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md (excluded-feature scan) |
| WhatsApp / email sending | (not evidenced) | Excluded | Yes | Owner decision | — | Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md (excluded-feature scan) |
| SQL Server | OLE DB `SQLOLEDB.1` over LAN | EF Core 8 (SQL Server provider) | Yes | Modern data access | `business/WORKFLOWS.md` (WF-01) | `Docs/KB/DATA-MODEL.md` |
| Jet 4 / Access | Default store | Not used (SQLite for tests only) | Yes | Single modern engine | `business/WORKFLOWS.md` | `Docs/KB/DATA-MODEL.md` |

---

## 3. Undocumented rationales (gaps)

The following modern differences have **no documented rationale** in the available material:

1. Critical-result escalation workflow (legacy visual-only → modern not evidenced) — `RATIONALE NOT DOCUMENTED`.
2. Barcode as patient identifier (legacy) → modern uses GUIDs without documented barcode handling.
3. Reference-range administration screen (legacy «المعدل الطبيعي» editor → not evidenced in modern).
4. Band-selection algorithm improvement (legacy engine-arbitrary → modern not documented).
5. `pregsmok` dimension handling (modern not documented).
6. Installer/deployment model (legacy `RL_SETUP.exe` → modern none evidenced).
7. Licensing model (legacy hardware-bound → modern none evidenced).
8. Quality gate "verified results only" (legacy → modern not evidenced).
9. Output categories (legacy 4 → modern simplified to single category at exporter level).
10. Critical-result second signatory (legacy unknown → modern not evidenced).

These are recorded as gaps, not invented decisions.

## 4. Version applicability

- Legacy behaviour: see `VERSIONS.md`.
- Modern implementation: see `Docs/KB/*` (modern system).
- Differences listed here are predominantly **V2 vs modern**; V1 vs modern differences are the same
  minus V2-only features.

## 5. Related documents

- `Docs/KB/legacy/VERSIONS.md` — version identity.
- `Docs/KB/legacy/data/MODEL.md`, `IDENTIFIERS.md`, `BRANCHES.md`.
- `Docs/KB/legacy/clinical/RULES.md`, `KNOWLEDGE-LAYER.md`, `PROFILES.md`.
- `Docs/KB/legacy/business/WORKFLOWS.md`, `SECURITY.md`, `CAPABILITIES.md`.
- `Docs/KB/legacy/architecture/*`.
- `Docs/KB/legacy/evidence/CONTRADICTIONS.md`, `UNRESOLVED.md`.
- Original source evidence: external: Complete_Reverse_Engineering_Reference_V1.md / Docs/Complete_Reverse_Engineering_Reference_V2.md,
  external: Business_Rules_V1.md / Docs/Business_Rules_V2.md, `Database documentation.md`, `Documenting the Executable File.md`.
- Modern implementation: `Docs/KB/ARCHITECTURE.md`, `Docs/KB/DATA-MODEL.md`, `Docs/KB/DOMAIN-RULES.md`,
  Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md, `Docs/KB/OPERATIONS.md`.
- Modern source: `src/`, `tests/`.
