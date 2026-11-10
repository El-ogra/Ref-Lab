# Ref Lab — Medical Laboratory Management System

نظام إدارة مختبرات طبي — إعادة بناء حديثة على أساس أدلة مستخرجة من نظامين
مرجعيين حقيقيين (`V-ROOT-1.00` و `V-CD-2019`).

> A modern re-implementation of the RealLab legacy laboratory information system,
> rebuilt from verified reverse-engineering evidence rather than from legacy code.

---

## 1. Target stack

Fixed by `IDEA.md` and not to be changed without explicit owner approval.

| Concern | Technology |
|---|---|
| Language | C# 12 |
| Framework | .NET 8 |
| Desktop UI | WPF (`net8.0-windows10.0.17763.0`) |
| Presentation pattern | MVVM |
| Architecture | Clean Architecture |
| Request handling | CQRS with MediatR |
| ORM | Entity Framework Core 8 |
| Database | Microsoft SQL Server |

---

## 2. Repository layout

```
Ref Lab/
├── RefLab.sln
├── Directory.Build.props        Shared build settings for every project
├── global.json                  Pinned SDK baseline
├── .gitignore
├── README.md
├── src/
│   ├── RefLab.Domain/           Entities, value objects, enums, domain rules
│   │   ├── ClinicalKnowledge/   Test catalogue, reference ranges, constants
│   │   ├── Identity/            Operators, permissions
│   │   ├── Patients/            Patients, orders, requested tests, history
│   │   ├── Billing/             Payments, referring parties, physicians, prices
│   │   ├── Auditing/            Print audit, delivery audit, attendance
│   │   ├── SendOut/             Send-out requests and sample dispositions
│   │   ├── Rules/               Policies, selectors, calculators
│   │   ├── ValueObjects/        Identifiers, money, measurements, intervals
│   │   ├── Enums/               Sex, classification, lifecycle states
│   │   └── Exceptions/          Domain exception hierarchy
│   ├── RefLab.Application/      CQRS commands/queries, MediatR handlers, DTOs
│   │   ├── Abstractions/        IAppDbContext, IUnitOfWork, ICurrentUser, IClock,
│   │   │                        IPdfExporter, IReportRenderer, IPasswordHasher,
│   │   │                        IPrintSequenceGenerator
│   │   ├── Behaviors/           Validation, authorization, logging, exception translation
│   │   ├── Common/              Paging, the application exception hierarchy, report models
│   │   ├── Features/            One folder per functional area, one file per use case
│   │   └── Validation/          FluentValidation validators
│   ├── RefLab.Infrastructure/   EF Core persistence, QuestPDF, PBKDF2 hashing, desktop session
│   │   ├── Persistence/         RefLabDbContext, fluent configuration, value converters
│   │   │   ├── Configurations/  One fluent configuration per aggregate
│   │   │   ├── Converters/      Value-object and collection conversions
│   │   │   └── Migrations/      The generated SQL Server migration
│   └── RefLab.Presentation/     WPF views and view models (MVVM)
├── docs/
│   ├── USE_CASE_INVENTORY.md    Generated from the source — 70 use cases
│   ├── RL-P3-Completion-Report.md
│   ├── RL-P4-Completion-Report.md
│   └── LOCAL_AGENT_IMPLEMENTATION_GUIDE.md
├── scripts/
│   └── generate_use_case_inventory.py
└── tests/
    ├── RefLab.Domain.Tests/         Domain unit tests and architecture guards
    ├── RefLab.Application.Tests/    Application use-case tests and architecture guards
    └── RefLab.Infrastructure.Tests/ Persistence integration tests, QuestPDF, hashing, session
```

**Phase 4 status: 70 / 70 use cases implemented, 70 / 70 with a validator,
70 / 70 covered by a test, 535 tests passing, 0 build warnings.**

**The laboratory is a single-site installation.** The legacy `Labbranch` table,
the branch column of the operator table, the branch digit at position 9 of the
13-character order identifier and the multi-branch stock-take were all removed
by owner decision in Phase 4. The order identifier keeps its 13-character
layout, but position 9 now belongs to the middle segment.

## 2b. Domain layer — Phase 2 delivery

The Domain layer carries **zero package references and zero project references**.
It is a pure business model. Its content is derived from the verified legacy
schema rather than invented:

| Folder | Legacy source |
|---|---|
| `ClinicalKnowledge/` | `testtable` (38 cols), `normalrange` (18 cols), `constanttable`, `subgrouptable`, `testunits`, `converts`, `abbreviations` |
| `Identity/` | `patients` — the **operator** table (33 cols) |
| `Patients/` | `patientinfo` (39 cols), `patienttest` (46 cols), `history` (25 cols) |
| `Billing/` | `Gehats` (21 cols), `payment` (10 cols), `pricelist`, `mowzaf` |
| `Auditing/` | `printuser`/`printdate`/`printnum`, `SMSSender`, `loger` |
| `SendOut/` | `patienttest.sentout`/`send`/`otherlabname`, microbiology sample tables |

**Every domain decision is traceable to a documented legacy artefact.** Where
the legacy behaviour is not established, the domain says so in an XML
documentation comment rather than inventing a rule.

---

## 2c. Persistence layer — Phase 4 delivery

`RefLab.Infrastructure` implements the two Application abstractions that
describe the store, over a single Entity Framework Core context.

| Abstraction | Implementation | Contract |
|---|---|---|
| `IAppDbContext` | `RefLabDbContext` | `IQueryable<T>` only — the Application layer never sees a `DbSet<T>` |
| `IUnitOfWork` | `RefLabDbContext` | The same scoped instance as the query side, so a handler sees one consistent snapshot |

| Piece | Content |
|---|---|
| `RefLabDbContext` | 27 mapped tables, one per aggregate of the laboratory |
| `Persistence/Configurations/` | One fluent configuration per aggregate; value objects are mapped with `OwnsOne` |
| `Persistence/Converters/` | The `PatientId`, permission, printer and collection conversions |
| `Persistence/Migrations/` | `InitialCreate` — the SQL Server migration that creates the whole schema |
| `DatabaseInitializer` | Applies the pending migrations |
| `AddPersistenceLayer` | Registers the context, `IAppDbContext`, `IUnitOfWork` and the initialiser |

**Applying the schema to an installation is owner-gated.** The migration is
generated and reviewed; it is never applied automatically, and an existing
migration is never modified.

---

## 3. Dependency rule

```
Presentation ──▶ Application ──▶ Domain ◀── Infrastructure
        └──────────── Composition Root only ────────────┘
```

| Rule | Enforcement |
|---|---|
| The Domain layer has **no** package references | `DomainLayerBoundaryTests` |
| The Application layer references no persistence or WPF assembly | `ApplicationLayerBoundaryTests` |
| The Application layer does not reference the Infrastructure layer | `ApplicationLayerBoundaryTests` |

The `Infrastructure` layer is wired to the rest of the solution from the
composition root only. It never leaks upward.

| Rule | Enforcement |
|---|---|
| The Domain layer has **no** package references | `Domain_references_only_the_base_class_library` |
| The Domain layer has **no** project references | `RefLab.Domain.csproj` |
| The Application layer references no persistence or WPF assembly | `Application_does_not_reference_persistence_or_presentation_frameworks` |
| The Application layer does not reference the Infrastructure layer | `Application_does_not_reference_the_infrastructure_layer` |

---

## 4. Prerequisites

| Requirement | Version |
|---|---|
| Windows | 10 (build 1809 or later) |
| Visual Studio | 2022 (17.11 or later) with the **.NET desktop development** workload |
| .NET SDK | 8.0.100 or later (`global.json` permits any newer major) |
| SQL Server | 2019 or later — required from Phase 4 onward |
| `dotnet-ef` | 8.0.x — `dotnet tool install --global dotnet-ef --version 8.*` |

---

## 5. Build and test

From the repository root:

```powershell
dotnet restore RefLab.sln
dotnet build   RefLab.sln -c Debug
dotnet build   RefLab.sln -c Release
dotnet test    RefLab.sln -c Release
```

Or from Visual Studio 2022: open `RefLab.sln`, select the `Debug|Any CPU`
configuration, and build.

---

## 6. Approved design decisions

| ID | Decision | Rationale |
|---|---|---|
| **Q1** | A unified `OrderTestResult` table paired with `TestDefinition` replaces the 51 legacy profile tables. | Removes the legacy structural bloat while preserving the clinical data. |
| **Q2** | Only security vulnerabilities and data-integrity flaws are fixed. All other business logic, workflows and clinical behaviour is matched exactly to the legacy system. | Preserves verified behaviour while closing real defects. |
| **Q3** | `MediatR` is pinned to the **12.x** line (MIT). | MediatR 13 and later require a commercial licence. |
| **Q4** | The legacy PDF path (a hard-coded `Bullzip PDF Printer` device name) is replaced, never reproduced. | It was a third-party driver dependency, not vendor code. |
| **Q5** | The Domain layer carries no package reference and no project reference. | The business model must be testable without any framework. |
| **Q6** | Monetary amounts use `decimal`; the legacy system used `Double`. | Binary floating point cannot represent decimal currency exactly. A data-integrity correction. |
| **Q7** | The reference range is banded by sex and age, and the selection among overlapping bands is resolved deterministically. | The legacy selection rule was never established. |

---

## 7. Governance

- The Domain layer carries no package references. A reviewer finding one must
  reject the change.
- Business rules live in the Domain layer only and are never duplicated.
- Arabic user-interface labels come from `ui-labels-verified.md`. New Arabic
  strings must be documented there or explicitly approved; they are never
  invented.
- Database schema changes require explicit owner approval and are never applied
  automatically. Existing migrations are never modified.
- The reference system at `F:\REAL_LAB_COPY` is strictly read-only and is never
  executed.

---

## 8. Known issue — .NET 8 end of support

**.NET 8 reaches end of support on 10 November 2026.** `IDEA.md` mandates .NET 8,
so the `NETSDK1138` end-of-life warning is suppressed in `Directory.Build.props`
via `CheckEolTargetFramework=false` in order to keep the build green while the
framework-version decision rests with the project owner.

**.NET 10 (LTS)** is supported until 14 November 2028 and is the recommended
target once approved. Removing the suppression and retargeting is a contained
change.

---

## 9. Project documentation

| Document | Content |
|---|---|
| `RL-P0-RPT-001` — Phase 0 Discovery Report | Input verification, architectural assessment, roadmap |
| `RL-P4-Completion-Report.md` — Phase 4 Completion Report | Branch removal, the persistence layer, the schema and the migration |
| `IDEA.md` | Authoritative project specification |
| `ui-labels-verified.md` | Verified Arabic user-interface labels |
| `Data_Model_and_Workflow_Entities_V2.md` | Verified data model (40 entities, 19 observed relationships) |
| `Business_Rules_V2.md` | 45 verified business rules |
| `Functional_Specification_V2.md` | 48 functions and 7 workflows |
| `Functionality_Requirements_Matrix_V2.md` | 48 functions mapped to 72 requirements |
