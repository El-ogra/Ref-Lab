# RL-P4-CMP-001 — Phase 4 Completion Report

**Project:** Ref Lab — Medical Laboratory Management System
**Phase:** 4 — Branch removal (Part 1) and the persistence layer (Part 2)
**Date:** 2026-11-10
**Status:** complete

---

## 1. Outcome

Phase 4 delivered both parts in full.

| Measure | Phase 3 | Phase 4 |
|---|---|---|
| Use cases | 73 | **70** |
| Validators | 73 | **70** |
| Named permissions | 16 | **15** |
| Mapped tables | — | **27** |
| Mapped columns | — | **338** |
| Foreign keys | — | **11** |
| Indexes | — | **26** |
| Tests | 504 | **535** |
| Build errors | 0 | **0** |
| Build warnings | 0 | **0** |

| Project | Source files |
|---|---|
| `RefLab.Domain` | 63 |
| `RefLab.Application` | 119 |
| `RefLab.Infrastructure` | 20 |
| `RefLab.Presentation` | 2 |
| `RefLab.Domain.Tests` | 36 |
| `RefLab.Application.Tests` | 28 |
| `RefLab.Infrastructure.Tests` | 8 |

---

## 2. Part 1 — the branch removal

### 2.1 The decision that drove it

The laboratory is a **single-site installation**. The owner decision removed the
legacy multi-branch product from scope rather than modelling it and hiding it.

This is a removal, not a deprecation. There is no `Branch` entity, no
`BranchId` on any entity, no branch table in the schema, no per-branch
schema, and no branch-scoped flag on the laboratory settings.

### 2.2 What was deleted

Eight files, in full:

| File | Reason |
|---|---|
| `src/RefLab.Domain/Identity/Branch.cs` | The entity itself. |
| `src/RefLab.Application/Features/Branches/CreateBranch.cs` | The branch creation use case. |
| `src/RefLab.Application/Features/Branches/GetBranches.cs` | The branch listing use case. |
| `src/RefLab.Application/Features/Branches/Dtos.cs` | The branch DTOs. |
| `src/RefLab.Application/Validation/BranchValidators.cs` | The branch validators. |
| `src/RefLab.Application/Features/Billing/RunMultiBranchStockTake.cs` | The multi-branch stock-take. A single-site laboratory keeps one stock-take, already covered by `RunPeriodStockTake` and `RunStockTakeByPhysicianOrParty`. |
| `tests/RefLab.Application.Tests/Features/Branches/BranchHandlerTests.cs` | The tests of the deleted use cases. |
| `tests/RefLab.Domain.Tests/Identity/BranchTests.cs` | The tests of the deleted entity. |

### 2.3 The order identifier

The identifier stays thirteen characters long. The two positions the legacy
analysis established are kept exactly where they were, and the position the
branch digit occupied is absorbed by the adjacent segment whose meaning was
never established.

| Position | Segment | Provenance |
|---|---|---|
| 1–2 | Leading segment | Semantics never established. |
| 3–4 | Person key | **Established** — `MID(patientid,3,2)`. |
| 5–9 | Middle segment | Semantics never established. Absorbs the freed position. |
| 10–12 | Display code | **Established** — `MID(patientid,10,3)`. |
| 13 | Trailing character | Semantics never established. |

The legacy middle segment ran from position 5 to position 8. It now runs from
position 5 to position 9.

`OrderIdentifier.FromSegments` takes five arguments instead of six, and
`IdentifierAllocator.Allocate` no longer takes a `Branch`.

### 2.4 The counts that fell

| Measure | Before | After | Why |
|---|---|---|---|
| Use cases | 73 | 70 | `CreateBranch`, `GetBranches` and `RunMultiBranchStockTake` were deleted. |
| Validators | 73 | 70 | The three validators of the deleted use cases went with them. |
| Permissions | 16 | 15 | `Permission.ManageBranches` guarded the branch administration that no longer exists. The remaining permissions were renumbered 1–15 without a gap. |
| Domain source files | 64 | 63 | `Branch.cs` was deleted. |

The `Permission` numbering was changed deliberately rather than left with a
hole. The enumeration is persisted by name — never by ordinal — so the
renumbering is safe, and a hole would have been a permanent reminder of a
removed capability in the middle of the list.

### 2.5 Verification

`grep -ri branch src/ tests/` returns only the XML documentation that
explains *why* the legacy position is free. No code path reads or writes a
branch.

---

## 3. Part 2 — the persistence layer

### 3.1 The context

`RefLabDbContext` implements `IAppDbContext` and `IUnitOfWork` **explicitly**.

That is not a stylistic choice. `DbContext` already declares
`public virtual DbSet<T> Set<T>()`. Implementing the interface implicitly
would have hidden the base method and produced a `CS0108` warning — and the
project builds with `TreatWarningsAsErrors=true`. The explicit form also makes
the boundary unmistakable: the Application layer receives `IQueryable<T>` and
never a `DbSet<T>`.

Both interfaces are served by **one scoped instance**:

```csharp
services.AddDbContext<RefLabDbContext>(configure);
services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<RefLabDbContext>());
services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<RefLabDbContext>());
```

A handler that reads through the query side and writes through the unit of
work therefore sees one consistent snapshot. A test asserts that the two
resolve to the same object.

### 3.2 The schema

27 tables, one per aggregate:

| Area | Tables |
|---|---|
| Identity | `Operators`, `LaboratorySettings` |
| Clinical knowledge | `TestDefinitions`, `ReferenceRanges`, `ClinicalConstants`, `ClinicalSubGroups`, `TestUnits`, `ConversionFactors`, `Abbreviations`, `ProthrombinControlCalibrations` |
| Patients | `Patients`, `PatientOrders`, `OrderTests`, `PatientHistories`, `PatientPictures` |
| Billing | `Payments`, `PriceLists`, `ReferringParties`, `TreatingPhysicians` |
| Microbiology | `Antibiotics`, `CultureResults` |
| Send-out | `SendOutRequests`, `SampleDispositions` |
| Auditing | `PrintAudits`, `AttendanceEntries` |
| Farming | `Farms`, `FarmTransactions` |

**No branch table and no branch column.** Two tests assert this directly, so
the capability cannot quietly return.

### 3.3 The value objects

The value objects are mapped with `OwnsOne`, which puts their columns in the
owning table:

| Value object | Where it lives |
|---|---|
| `Money` | `PatientOrder`, `OrderTest`, `Payment` |
| `Percentage` | `PatientOrder`, `ReferringParty` |
| `OrderIdentifier` | `PatientOrder` |
| `ResultValue` | `OrderTest` |
| `AgeBand`, `ReferenceInterval`, `CriticalInterval` | `ReferenceRange`, and nested inside the `ReferenceSnapshot` of `OrderTest` |
| `PatientId` | A value **converter** onto `Guid`, used by every patient reference |

The nested owned types were the hard part. `ReferenceSnapshot` owns an
`AgeBand` and a `ReferenceInterval`, and the `ReferenceInterval` owns a
`CriticalInterval`. EF Core needs a constructor it can call without arguments
before it fills the properties in, and a constructor parameter that points at
another owned type cannot be bound. `CriticalInterval` and `ReferenceInterval`
therefore gained a private parameterless constructor. The public factory
remains the only way to build a validated instance.

### 3.4 The collections

Four domain collections cannot be mapped as relations without weakening the
domain, so they are mapped onto single columns:

| Collection | Column | Conversion |
|---|---|---|
| `Operator.Permissions` | `Permissions` | The permission **names**, separated by commas. |
| `LaboratorySettings.Printers` | `Printers` | `Role=Printer`, separated by semicolons. |
| `CultureResult.Susceptibilities` | `Susceptibilities` | JSON. |
| `PatientHistory.Slots` | `Slots` | JSON. |
| `ClinicalConstant.Values` | `Values` | JSON. |
| `OrderTest.AdditionalResults` | `AdditionalResults` | JSON. |
| `Antibiotic.CommonNames` | `CommonNames` | JSON. |

Two problems had to be solved to make this work.

**First, the change tracker.** A collection that is mutated in place is
invisible to the change tracker when the collection is stored as a single
column: the stored snapshot and the current value are the same object. The
domain therefore **replaces** the collection instance whenever it changes —
`Operator.Grant` and `Revoke`, `LaboratorySettings.BindPrinter` and
`UnbindPrinter`, `CultureResult.AddSusceptibility` and
`RemoveSusceptibility`, and `PatientHistory.SetSlot`. The persistence layer
additionally installs an explicit `ValueComparer` per property that compares
the **contents** rather than the identity.

**Second, the materialisation type.** The operator's backing field is a
`Permission[]`, so the converter must hand back an array and not a `List<>`.
The first implementation returned a list and failed at materialisation with
`InvalidCastException`; the converter now returns an array.

### 3.5 The collection navigation

`PatientOrder.Tests` is the one real collection navigation. The property is
read-only — `IReadOnlyCollection<OrderTest>` over a `List<OrderTest>` field —
and it is mapped through **field access**:

```csharp
builder.HasMany(order => order.Tests)
    .WithOne()
    .HasForeignKey(test => test.OrderId);

builder.Navigation(order => order.Tests)
    .UsePropertyAccessMode(PropertyAccessMode.Field);
```

This matters for behaviour, not only for storage. The `OrderTests` use case
reads `order.Tests.Count` to number the arrangement of the next test. Without
the navigation, a loaded order would come back with an empty collection and
the numbering would restart at zero on every request.

### 3.6 The migration

`Persistence/Migrations/20261110103458_InitialCreate.cs` creates the whole
schema: 27 tables, 338 columns, 11 foreign keys and 26 indexes, against SQL
Server.

The migration is generated, reviewed and **not applied**. The schema approval
gate stands: applying it to an installation that already holds data is an
owner-gated operation.

---

## 4. The tests

61 infrastructure tests, of which 44 are new.

| Group | Tests | What it proves |
|---|---|---|
| `RefLabDbContextTests` | 7 | The model builds, every entity has a key, the schema covers every aggregate, **no branch table and no branch column exist**, and the identifier column is thirteen characters wide. |
| `PersistenceIntegrationTests` | 16 | The full write-and-read-back path: operators and their permissions, patients and orders, the money, the percentage and the identifier, the result and the frozen reference range with its nested owned types, the additional results, the history slots, the clinical constants, the susceptibilities, the antibiotic names, the printers, a removal and a query. |
| `MigrationTests` | 10 | Exactly one migration; the model and the migration agree; the script creates every table; the script and the rollback script carry no branch table; the script targets SQL Server types; the order-pipeline foreign keys are present. |
| `PersistenceRegistrationTests` | 6 | The container registers both abstractions over one context; an empty connection string is rejected; all eight Application abstractions resolve; the context is registered against SQL Server. |

### 4.1 The provider used by the tests

The integration tests run against an **in-memory SQLite** database. The build
agent has no SQL Server, and a container image with one is not available.

The fluent configuration is provider-agnostic on purpose: it declares lengths
and precisions but never a provider-specific column type. The same model
therefore builds the SQL Server schema that the migration targets.

The migration itself is **never applied by the test suite** — the SQL Server
DDL uses `nvarchar(max)` and `uniqueidentifier`, which SQLite cannot parse.
The migration tests instead generate the SQL Server script and assert on its
content. Applying the schema to a real SQL Server instance remains an open
item and is listed in section 6.

### 4.2 The model-versus-migration check is provider-specific

`HasPendingModelChanges()` compares the current model against the stored
snapshot, and a column type is a provider-specific part of the model. The
comparison is therefore made against a **SQL Server** context, not against the
SQLite context the other tests use. Comparing against SQLite reports a
spurious difference on every column.

---

## 5. What the QuestPDF adapter needed

Nothing. The `IPdfExporter` adapter and the `IReportRenderer` adapter already
existed from Phase 3, and they already satisfied their abstractions. Phase 4
verified them against the abstraction contract rather than redesigning them:
the exporter still produces a real PDF — the file starts with `%PDF` — and the
renderer still produces the sectioned document the abstraction describes.

---

## 6. The open items Phase 4 did not close

| Item | Note |
|---|---|
| The schema approval gate | The generated schema still has to be reviewed and approved by the laboratory owner before it is applied. |
| The SQL Server smoke test | No SQL Server instance was available on the build agent. Applying the migration to a real instance remains open. |
| The International Sensitivity Index formula | The standard laboratory definition is used. The legacy formula was never recovered and still needs confirmation. |
| The laboratory currency | A configuration decision, still open. |
| The barcode format | Ref Lab's own format, still open. |
| `PrintSequenceGenerator` and `SessionCurrentUser` | Both are process-scoped and in-memory. They need a durable store before a laboratory runs more than one terminal. |
| The Arabic user-interface labels | They come from `ui-labels-verified.md` and are never invented. A Phase 5 concern. |

---

## 7. What was deliberately not done

- **Phase 5, 6 and 7 were not started.** The user-interface work, the
  presentation layer and the deployment work are out of scope for this phase.
- **No migration was applied.** The schema was generated and reviewed only.
- **The legacy installation was not touched.** It stays strictly read-only and
  was never executed.
- **No Arabic string was invented.** The Arabic labels remain a Phase 5
  concern and come only from `ui-labels-verified.md`.
- **The SMS and NATIGH.COM channels were not reintroduced.** They remain
  permanently removed by owner decision.
