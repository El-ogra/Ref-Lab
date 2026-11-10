# Local Agent Implementation Guide — Phase 4

**Project:** Ref Lab — Medical Laboratory Management System
**Phase:** 4 — Branch removal (Part 1) and the persistence layer (Part 2)
**Edition:** Phase 4 — supersedes the Phase 3 edition
**Status:** complete — 0 build errors, 0 build warnings, 535 tests passing

> This guide is written for the agent or developer who applies the Phase 4
> delivery on the owner machine. It is deliberately explicit about what to
> delete, what to run, and what the result must look like, so that the
> delivery can be applied without re-reading the implementation.

---

## 0. What Phase 4 contains

Phase 4 has two parts.

**Part 1 — the branch removal.** The laboratory is a single-site installation.
Every trace of the legacy multi-branch product is removed from the Domain
layer and from the Application layer, and the 13-character order identifier is
re-laid-out so that it no longer carries a branch digit.

**Part 2 — the persistence layer.** The Infrastructure layer gains an Entity
Framework Core context that implements `IAppDbContext` and `IUnitOfWork`, the
fluent configuration of every table, the generated SQL Server migration, and
the integration tests that prove the adapter works against a real database.

---

## 1. Part 1 — the files to delete

These files are removed in their entirety. Do not merge them, do not keep a
stub, and do not leave a partially-used version behind.

| # | File | Reason |
|---|---|---|
| 1 | `src/RefLab.Domain/Identity/Branch.cs` | The branch entity itself. |
| 2 | `src/RefLab.Application/Features/Branches/CreateBranch.cs` | The branch creation use case. |
| 3 | `src/RefLab.Application/Features/Branches/GetBranches.cs` | The branch listing use case. |
| 4 | `src/RefLab.Application/Features/Branches/Dtos.cs` | The branch DTOs. |
| 5 | `src/RefLab.Application/Validation/BranchValidators.cs` | The branch validators. |
| 6 | `src/RefLab.Application/Features/Billing/RunMultiBranchStockTake.cs` | The multi-branch stock-take use case. A single-site laboratory has one stock-take, already covered by `RunPeriodStockTake` and `RunStockTakeByPhysicianOrParty`. |
| 7 | `tests/RefLab.Application.Tests/Features/Branches/BranchHandlerTests.cs` | The tests of the deleted use cases. |
| 8 | `tests/RefLab.Domain.Tests/Identity/BranchTests.cs` | The tests of the deleted entity. |

The two folders `src/RefLab.Application/Features/Branches/` and
`tests/RefLab.Application.Tests/Features/Branches/` become empty and are
removed with them.

> If you are applying this delivery to a tree that still holds the Phase 3
> code, deleting the eight files above is enough to remove the branch
> capability. Everything else is an edit, listed in section 2.

---

## 2. Part 1 — the files to edit

### 2.1 The Domain layer

| File | Change |
|---|---|
| `src/RefLab.Domain/ValueObjects/OrderIdentifier.cs` | **Rewritten.** The layout is now 1–2 leading segment · 3–4 person key · 5–9 middle segment · 10–12 display code · 13 trailing character. `BranchCode` is gone; `FromSegments` takes five arguments instead of six. |
| `src/RefLab.Domain/Rules/IdentifierAllocator.cs` | **Rewritten.** `Allocate` no longer takes a `Branch`; the branch digit argument is dropped. |
| `src/RefLab.Domain/Identity/Operator.cs` | The `BranchId` property and the `AssignToBranch` method are removed. |
| `src/RefLab.Domain/Identity/Permission.cs` | `Permission.ManageBranches` is removed. The remaining permissions are renumbered 1–15 without a gap. |
| `src/RefLab.Domain/Patients/PatientOrder.cs` | The `BranchId` property and the `branchId` argument of `Create` are removed, together with the branch range validation. |
| `src/RefLab.Domain/Configuration/LaboratorySettings.cs` | `IsBranchScoped` and `SetBranchScoped` are removed. |
| `src/RefLab.Domain/ClinicalKnowledge/TestDefinition.cs` | The `Branch` string property is removed. |

### 2.2 The Application layer

| File | Change |
|---|---|
| `src/RefLab.Application/Abstractions/ICurrentUser.cs` | `BranchId` is removed from the interface. |
| `src/RefLab.Application/Features/Patients/AllocateOrderIdentifier.cs` | **Rewritten.** The query no longer reads the branch of the signed-in operator and the middle segment is five characters long. |
| `src/RefLab.Application/Features/Patients/RegisterPatient.cs` | The branch identifier is no longer read from `ICurrentUser` and no longer passed to `PatientOrder.Create`. |
| `src/RefLab.Application/Features/Patients/Dtos.cs` | `PatientOrderDto.BranchId` and `OrderIdentifierDto.BranchCode` are removed. |
| `src/RefLab.Application/Features/Identity/CreateOperator.cs` | The `BranchId` argument and the `AssignToBranch` call are removed. |
| `src/RefLab.Application/Features/Identity/UpdateOperator.cs` | The `BranchId` argument and the `AssignToBranch` call are removed. |
| `src/RefLab.Application/Features/Identity/Dtos.cs` | `OperatorDto.BranchId` is removed. |
| `src/RefLab.Application/Features/Configuration/ConfigureLaboratorySettings.cs` | The `IsBranchScoped` argument and the `SetBranchScoped` call are removed. |
| `src/RefLab.Application/Features/Configuration/Dtos.cs` | `LaboratorySettingsDto.IsBranchScoped` is removed. |
| `src/RefLab.Application/Features/Billing/Dtos.cs` | `StockTakeDto.BranchId` is removed. |
| `src/RefLab.Application/Validation/BranchValidators.cs` | Deleted — see section 1. |
| `src/RefLab.Application/Validation/BillingValidators.cs` | `RunMultiBranchStockTakeValidator` is removed. |
| `src/RefLab.Application/Validation/IdentityValidators.cs` | The two branch range rules are removed. |
| `src/RefLab.Application/Validation/PatientValidators.cs` | The middle segment is now five characters long. |

### 2.3 The Infrastructure layer

| File | Change |
|---|---|
| `src/RefLab.Infrastructure/Session/SessionCurrentUser.cs` | The branch field is removed and the session no longer carries a branch. |

### 2.4 The test projects

| File | Change |
|---|---|
| `tests/RefLab.Application.Tests/Fakes/TestData.cs` | `CreateOperator` and `CreateOrder` lose the branch argument; `CreateBranch` is deleted. |
| `tests/RefLab.Application.Tests/Fakes/Fakes.cs` | The fake current user loses `BranchId`. |
| `tests/RefLab.Application.Tests/Features/Branches/BranchHandlerTests.cs` | Deleted — see section 1. |
| `tests/RefLab.Application.Tests/Features/Billing/AccountingTests.cs` | The two multi-branch stock-take tests are removed. |
| `tests/RefLab.Application.Tests/Features/Configuration/SettingsTests.cs` | The `IsBranchScoped` argument and assertion are removed. |
| `tests/RefLab.Application.Tests/Features/Identity/IdentityHandlerTests.cs` | The branch arguments and assertions are removed. |
| `tests/RefLab.Application.Tests/Features/Identity/OperatorManagementTests.cs` | The branch argument, the branch assertion and the branch validation test are removed. |
| `tests/RefLab.Application.Tests/Features/Patients/PatientHandlerTests.cs` | The branch assertion is removed. |
| `tests/RefLab.Application.Tests/Features/Patients/PatientRecordsTests.cs` | The identifier tests are rewritten for the branchless layout. |
| `tests/RefLab.Domain.Tests/Identity/BranchTests.cs` | Deleted — see section 1. |
| `tests/RefLab.Domain.Tests/Identity/OperatorTests.cs` | The two branch tests are removed. |
| `tests/RefLab.Domain.Tests/Identity/PermissionTests.cs` | The expected permission count becomes fifteen. |
| `tests/RefLab.Domain.Tests/Patients/PatientOrderTests.cs` | The branch argument, the branch assertion and the branch validation test are removed. |
| `tests/RefLab.Domain.Tests/Configuration/LaboratorySettingsTests.cs` | The branch digit test is removed. |
| `tests/RefLab.Domain.Tests/Rules/IdentifierAllocatorTests.cs` | **Rewritten** for the branchless layout. |
| `tests/RefLab.Domain.Tests/ValueObjects/OrderIdentifierTests.cs` | **Rewritten** for the branchless layout. |
| `tests/RefLab.Application.Tests/DependencyInjectionTests.cs` | The expected handler and validator count becomes seventy. |
| `tests/RefLab.Infrastructure.Tests/InfrastructureServiceTests.cs` | The branch assignment and the branch assertion are removed. |

---

## 3. The new order identifier

The identifier stays thirteen characters long. Two positions were established
by the legacy analysis and are kept exactly where they were; one position
became free and is absorbed by the adjacent segment whose meaning was never
established.

| Position | Segment | Status |
|---|---|---|
| 1–2 | Leading segment | Semantics never established. |
| 3–4 | **Person key** | Established by the legacy analysis (`MID(patientid,3,2)`). |
| 5–9 | Middle segment | Semantics never established. **Absorbs the freed position.** |
| 10–12 | **Display code** | Established by the legacy analysis (`MID(patientid,10,3)`). |
| 13 | Trailing character | Semantics never established. |

The legacy branch digit was written into position 9. In the legacy system the
middle segment ran from position 5 to position 8; it now runs from position 5
to position 9.

**This layout is a hard data contract.** Reformatting it breaks the patient
identity, the display code and the turnover reporting at the same time. Do not
change it without owner approval.

---

## 4. Part 2 — the new files

| Path | Content |
|---|---|
| `src/RefLab.Infrastructure/Persistence/RefLabDbContext.cs` | The context. Implements `IAppDbContext` and `IUnitOfWork` explicitly, so the Application layer sees only `IQueryable<T>`. |
| `src/RefLab.Infrastructure/Persistence/RefLabDbContextFactory.cs` | The design-time factory the `dotnet ef` tools call. |
| `src/RefLab.Infrastructure/Persistence/DatabaseInitializer.cs` | Applies the pending migrations. |
| `src/RefLab.Infrastructure/Persistence/Configurations/*.cs` | One fluent configuration per aggregate, plus the shared value-object mappings. |
| `src/RefLab.Infrastructure/Persistence/Converters/PersistenceConverters.cs` | The `PatientId`, permission, printer, susceptibility and JSON collection conversions, with the comparers that make the change tracker see a replaced collection. |
| `src/RefLab.Infrastructure/Persistence/Migrations/*_InitialCreate.cs` | The generated SQL Server migration. |
| `src/RefLab.Infrastructure/Persistence/Migrations/*_InitialCreate.Designer.cs` | The model snapshot of that migration. |
| `src/RefLab.Infrastructure/Persistence/Migrations/RefLabDbContextModelSnapshot.cs` | The current model snapshot. |
| `tests/RefLab.Infrastructure.Tests/Persistence/SqliteTestDatabase.cs` | The in-memory SQLite fixture. |
| `tests/RefLab.Infrastructure.Tests/Persistence/RefLabDbContextTests.cs` | The schema tests, including the guard against a branch table or a branch column reappearing. |
| `tests/RefLab.Infrastructure.Tests/Persistence/PersistenceIntegrationTests.cs` | The round-trip tests over a real database. |
| `tests/RefLab.Infrastructure.Tests/Persistence/MigrationTests.cs` | The migration tests. |
| `tests/RefLab.Infrastructure.Tests/Persistence/PersistenceRegistrationTests.cs` | The container registration tests. |

### 4.1 Files also edited in Part 2

| Path | Change |
|---|---|
| `src/RefLab.Infrastructure/RefLab.Infrastructure.csproj` | `Microsoft.EntityFrameworkCore.Design` 8.0.33 is added, with `PrivateAssets=all`. |
| `src/RefLab.Infrastructure/InfrastructureServiceCollectionExtensions.cs` | `AddPersistenceLayer` is added — one overload for a SQL Server connection string, one for a caller-chosen provider. |
| `tests/RefLab.Infrastructure.Tests/RefLab.Infrastructure.Tests.csproj` | `Microsoft.EntityFrameworkCore.Sqlite` 8.0.33 is added, for the integration tests. |
| `src/RefLab.Domain/ValueObjects/CriticalInterval.cs` | A private parameterless constructor is added, because the persistence layer fills the properties in. |
| `src/RefLab.Domain/ValueObjects/ReferenceInterval.cs` | The same, plus the `CriticalInterval` property gains a private setter. |
| `src/RefLab.Domain/Identity/Operator.cs` | `Permissions` becomes a writable property that **replaces** the collection instance on `Grant` and `Revoke`. |
| `src/RefLab.Domain/Configuration/LaboratorySettings.cs` | `Printers` becomes a writable property that **replaces** the map instance on `BindPrinter` and `UnbindPrinter`. |
| `src/RefLab.Domain/Microbiology/CultureResult.cs` | `Susceptibilities` becomes a writable property that **replaces** the list instance on add and remove. |
| `src/RefLab.Domain/Patients/PatientHistory.cs` | `Slots` becomes a writable property that **replaces** the array instance on `SetSlot`. |

> **Why the replacement matters.** The persistence layer maps these four
> collections onto single columns and detects a change by comparing the stored
> value with the current one. Mutating the same instance in place would leave
> the comparison blind, and the change would never reach the database.

---

## 5. The commands to run

### 5.1 Restore, build and test

```powershell
cd <repository root>

dotnet restore RefLab.sln
dotnet build   RefLab.sln -c Debug
dotnet build   RefLab.sln -c Release
dotnet test    RefLab.sln -c Release
```

**Expected result:** 0 errors, 0 warnings, **535 tests passing**
(270 domain, 204 application, 61 infrastructure).

### 5.2 The migration commands

The `dotnet ef` tools need the design-time factory in
`src/RefLab.Infrastructure/Persistence/RefLabDbContextFactory.cs`. Install the
tool once:

```powershell
dotnet tool install --global dotnet-ef --version 8.*
```

Refresh the environment afterwards if the tool is not found:

```powershell
$env:PATH += ";$env:USERPROFILE\.dotnet\tools"
```

**To list the migrations:**

```powershell
dotnet ef migrations list `
  --project src/RefLab.Infrastructure/RefLab.Infrastructure.csproj `
  --startup-project src/RefLab.Infrastructure/RefLab.Infrastructure.csproj
```

**To add a migration** (only after a model change, and only when the owner has
approved the schema):

```powershell
dotnet ef migrations add <MigrationName> `
  --project src/RefLab.Infrastructure/RefLab.Infrastructure.csproj `
  --startup-project src/RefLab.Infrastructure/RefLab.Infrastructure.csproj `
  --output-dir Persistence/Migrations
```

**To generate the SQL script** without touching a database — this is the
artefact the owner reviews before anything is applied:

```powershell
dotnet ef migrations script `
  --project src/RefLab.Infrastructure/RefLab.Infrastructure.csproj `
  --startup-project src/RefLab.Infrastructure/RefLab.Infrastructure.csproj `
  --idempotent `
  --output <path>\RefLab-InitialCreate.sql
```

**To apply the migration to a database** — this is the owner-gated step. It is
run only after the generated schema has been reviewed and approved:

```powershell
$env:REFLAB_CONNECTION_STRING = "Server=<server>;Database=RefLab;User Id=<user>;Password=<password>;TrustServerCertificate=True"

dotnet ef database update `
  --project src/RefLab.Infrastructure/RefLab.Infrastructure.csproj `
  --startup-project src/RefLab.Infrastructure/RefLab.Infrastructure.csproj
```

> **The schema approval gate still stands.** Phase 4 generates and reviews the
> schema. It does not apply it to an installation that already holds data.

### 5.3 The use-case inventory

```powershell
python scripts/generate_use_case_inventory.py
```

**Expected result:** `use cases found: 70 (expected 70)` and exit code 0.

---

## 6. The expected numbers after the delivery

| Measure | Phase 3 | Phase 4 |
|---|---|---|
| Use cases | 73 | **70** |
| Validators | 73 | **70** |
| Named permissions | 16 | **15** |
| Domain source files | 64 | **63** (the `Branch` entity is gone) |
| Infrastructure tables | — | **27** |
| Tests | 504 | **535** |
| Build warnings | 0 | **0** |

The use-case count falls by three: `CreateBranch`, `GetBranches` and
`RunMultiBranchStockTake` were deleted because the capability they served no
longer exists. The permission count falls by one: `Permission.ManageBranches`
guarded the branch administration that has been removed.

---

## 7. How to verify the delivery

| Check | Expected |
|---|---|
| `dotnet build RefLab.sln -c Release` | 0 errors, 0 warnings |
| `dotnet test RefLab.sln -c Release` | 535 passed, 0 failed |
| `grep -ri branch src/ tests/` | Only the XML documentation that explains why the legacy branch position is free |
| `dotnet ef migrations list` | Exactly one migration, `…_InitialCreate` |
| `dotnet ef migrations script` | 27 `CREATE TABLE` statements, no `Branches` table |
| `python scripts/generate_use_case_inventory.py` | 70 / 70, exit code 0 |

The integration tests run against an **in-memory SQLite** database, because the
build agent has no SQL Server. The migration itself targets SQL Server and is
never applied by the test suite; the migration tests validate the generated
SQL Server script instead. Applying the schema to a real SQL Server instance
is the owner-gated step in section 5.2.

---

## 8. The open items that Phase 4 did not close

| Item | Note |
|---|---|
| The schema approval gate | The generated schema still has to be reviewed and approved by the laboratory owner before it is applied to an installation. |
| The SQL Server smoke test | No SQL Server instance was available on the build agent. Applying the migration to a real instance remains a Phase 6 item. |
| The International Sensitivity Index formula | The standard laboratory definition is used. The legacy formula was never recovered and still needs confirmation. |
| The laboratory currency | A configuration decision, still open. |
| The barcode format | Ref Lab's own format, still open. |
| `PrintSequenceGenerator` and `SessionCurrentUser` | Both are process-scoped and in-memory. They need a durable store before a laboratory runs more than one terminal. |
| The Arabic user-interface labels | They come from `ui-labels-verified.md` and are never invented. This is a Phase 5 concern. |

Phase 5, 6 and 7 are out of scope for this delivery and have not been started.
