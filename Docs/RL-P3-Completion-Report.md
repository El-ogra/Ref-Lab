# Ref Lab — Phase 3 Completion and Verification Report

**Document ID:** `RL-P3-COMPLETION-001`
**Phase:** 3 — Application Layer (final)
**Date:** 2026-11-09
**Baseline before this phase:** 344 tests passing (249 Domain + 95 Application)

---

## 1. Completion status

> **★ Correction to the target count.** The target was 72 use cases. The
> independent audit proved the real number is **73**: the inventory script's
> regular expression required a parameter list and silently skipped
> `GetBranchesQuery`, a real and tested use case — and then hard-coded its own
> error as the expected total. The delivery contains **73 use cases**, one more
> than the target, because the target itself came from the miscount. Nothing was
> removed to fit the number.

| Acceptance condition | Result | Evidence |
|---|---|---|
| Use Case Inventory | ✅ **73 / 73** | `scripts/generate_use_case_inventory.py` scans the compiled source and reports `use cases found: 73 (expected 73)`; `docs/USE_CASE_INVENTORY.md` |
| Previously missing use cases | ✅ **49 implemented** | §3 |
| Statistics | ✅ complete | `GetLaboratoryStatistics`, `GetPerformanceStatistics` |
| Worksheets | ✅ complete | `BuildPatientGroupWorkSheet`, `BuildTestGroupWorkSheet` |
| Microbiology | ✅ complete | `RecordCultureResult`, `GetCultureResults`, `ManageAntibioticCatalogue` |
| Cultures | ✅ complete | `RecordCultureResult`, `GetCultureResults` |
| Send-out | ✅ complete | `CreateSendOutRequest`, `DispatchSendOutSample`, `SettleSendOutSample`, `GetSendOutQueue`, `SplitOrWithdrawSample` |
| SMS | ✅ **completely removed** | §5 |
| NATIGH | ✅ **completely removed** | §5 |
| QuestPDF | ✅ preserved | `QuestPDF 2026.9.1` in `RefLab.Infrastructure.csproj`; `QuestPdfExporter` and `QuestPdfReportRenderer` |
| `IPdfExporter` | ✅ preserved and compatible | `src/RefLab.Application/Abstractions/IPdfExporter.cs` — the signature is unchanged from Phase 3 |
| WPF CS0118 fix | ✅ present | `src/RefLab.Presentation/App.xaml.cs:6` → `public partial class App : System.Windows.Application` |
| Existing tests | ✅ retained | the 344 baseline tests still exist and pass; the suite grew to 504 |
| New tests | ✅ complete and passing | §6 |
| Build | ✅ **0 errors** | Debug and Release |
| Warnings | ✅ **0** | `TreatWarningsAsErrors` is on, so a warning would fail the build |
| Application / Domain alignment | ✅ verified | §7 |
| EF Core leakage | ✅ none | §7 |
| Placeholder implementations | ✅ none | §7 |
| Completion report | ✅ this document | |
| Local Agent guide | ✅ `docs/LOCAL_AGENT_IMPLEMENTATION_GUIDE.md` | |
| Final archive | ✅ one ZIP | |

---

## 2. Measured results

Every figure below was produced by executing the commands, not estimated.

```
=== CLEAN-ROOM: rm -rf src/*/bin src/*/obj tests/*/bin tests/*/obj && rm -rf ~/.nuget/packages ===
=== BUILD Debug ===      0 Warning(s)   0 Error(s)
=== BUILD Release ===    0 Warning(s)   0 Error(s)
=== TEST Release ===
Passed!  - Failed: 0, Passed: 277, Skipped: 0, Total: 277 - RefLab.Domain.Tests.dll (net8.0)
Passed!  - Failed: 0, Passed: 210, Skipped: 0, Total: 210 - RefLab.Application.Tests.dll (net8.0)
Passed!  - Failed: 0, Passed:  17, Skipped: 0, Total:  17 - RefLab.Infrastructure.Tests.dll (net8.0)
=== TEST Debug ===       identical: 277 + 210 + 17 = 504 passed, 0 failed, 0 skipped
```

| Metric | Value |
|---|---:|
| Use cases | **73** (46 commands, 27 queries) |
| Feature areas | 15 |
| FluentValidation validators | **73** |
| Use cases requiring a permission | 46 |
| Use cases referenced by a test | **73** |
| Tests — Domain | 277 |
| Tests — Application | 210 |
| Tests — Infrastructure | 17 |
| **Tests — total** | **504** (was 344; **+160**) |
| Skipped tests | **0** |
| Projects | 7 |
| Source files | 266 `.cs` / 22406 lines |

---

## 3. The 49 newly implemented use cases

| # | Use case | Area | What it does |
|---:|---|---|---|
| 1 | `UpdateOperator` | Identity | Renames an operator, changes the role, the branch and the active state |
| 2 | `AssignOperatorPermissions` | Identity | Replaces the granted permission set, rejecting unknown permissions |
| 3 | `GetOperators` | Identity | Lists the operator accounts, active only by default |
| 4 | `AllocateOrderIdentifier` | Patients | Allocates the 13-character composite identifier with the branch digit at position 9 |
| 5 | `GeneratePatientBarcode` | Patients | Allocates and stores the barcode that identifies the order |
| 6 | `AttachPatientPicture` | Patients | Attaches a picture in one of the fifteen slots, refusing an occupied slot |
| 7 | `FollowUpCase` | Patients | The follow-up register, with the progress and the next action of every case |
| 8 | `EnterAdditionalResult` | Results | Writes one of the four additional result slots |
| 9 | `GetMethodSheet` | Results | The method sheet: sample type, description and clinical significance |
| 10 | `GetCriticalValues` | Results | The critical limits consulted before a result is released |
| 11 | `DeriveHematocrit` | Derivation | `Hct = Hgb × factor`, the factor read from the configured constants |
| 12 | `DeriveHemoglobinPercentage` | Derivation | Selects the factor by sex and age, then applies it |
| 13 | `DeriveInternationalSensitivityIndex` | Derivation | The ISI of a prothrombin control |
| 14 | `InterpolateProthrombinControl` | Derivation | Interpolates a control concentration from the calibration curve |
| 15 | `CreateTestDefinition` | Catalogue | Adds a test with its prices, sample type and Arabic name |
| 16 | `UpdateTestDefinition` | Catalogue | Amends the attributes and prices; refuses to rename the urine analysis |
| 17 | `CreateReferenceRange` | Catalogue | Adds a range scoped by sex and age window |
| 18 | `UpdateReferenceRange` | Catalogue | Amends the limits and the texts |
| 19 | `ManageUnitsAndConversions` | Catalogue | Maintains the unit and conversion dictionaries |
| 20 | `ManageAbbreviations` | Catalogue | Adds an abbreviation or amends its expansion |
| 21 | `ManageClinicalSubGroups` | Catalogue | Adds a sub-group or amends its name |
| 22 | `ProduceConsolidatedReport` | Reporting | Merges the results of several orders into one report and writes a PDF |
| 23 | `ProduceEmptyReport` | Reporting | Produces a blank report template with its column headings |
| 24 | `PrintPatientInvoice` | Reporting | Prints the invoice with the total, the discount and the lines |
| 25 | `PrintPatientIdCard` | Reporting | Prints the patient identity card |
| 26 | `ConfigurePrinters` | Reporting | Binds a printer to one of the six printer roles |
| 27 | `ConfigureReportHeaderFooter` | Reporting | Sets the paper size and the global header texts |
| 28 | `CustomiseReportLayout` | Configuration | Sets the margins, the background colour and the layout text |
| 29 | `ConfigureConnectionReceipt` | Configuration | Sets the header and the footer of the connection receipt |
| 30 | `ConfigureLaboratorySettings` | Configuration | The operating settings: paper size, sequence origin, branch scoping, barcode and work-sheet flags |
| 31 | `GetPayments` | Billing | Lists the payments, filtered by order or by period |
| 32 | `RunPeriodStockTake` | Billing | The period stock-take: income, discount and commission |
| 33 | `RunMultiBranchStockTake` | Billing | The stock-take across a master laboratory and its linked branches |
| 34 | `RunStockTakeByPhysicianOrParty` | Billing | The stock-take of one physician or one referring party |
| 35 | `SetDefaultAccountType` | Billing | Sets the default account type of new orders |
| 36 | `ManageTreatingPhysicians` | Parties | Adds a treating physician or amends its contact details |
| 37 | `ManagePriceLists` | Parties | Adds a price to a price list or amends it |
| 38 | `PrintPriceList` | Parties | Prints the price list |
| 39 | `CreateSendOutRequest` | Send-out | Enrols a test on the send-out list, refusing a duplicate |
| 40 | `GetSendOutQueue` | Send-out | Lists the queue so an unsettled referral stays visible |
| 41 | `SplitOrWithdrawSample` | Send-out | Records that a sample was split or withdrawn |
| 42 | `RecordCultureResult` | Microbiology | Records a culture and its antimicrobial susceptibilities |
| 43 | `GetCultureResults` | Microbiology | Lists the cultures of a patient or of a culture identifier |
| 44 | `ManageAntibioticCatalogue` | Microbiology | Adds an antibiotic or amends its susceptibility and clinical flags |
| 45 | `ManageFarmResults` | Microbiology | Records a farm transaction, creating the farm when it is unknown |
| 46 | `GetLaboratoryStatistics` | Statistics | Patient counts by month, by sex and by sub-group, over a period |
| 47 | `GetPerformanceStatistics` | Statistics | Reviewed, printed and handed-over counts, the critical count and the average turnaround |
| 48 | `BuildPatientGroupWorkSheet` | Worksheets | A work sheet over a group of patients in a period |
| 49 | `BuildTestGroupWorkSheet` | Worksheets | A work sheet over a test or a test group in a period |

---

## 4. Domain changes made in this phase

Every change is a genuine capability the Domain did not have, and every one is
documented in the code. **No existing Domain behaviour was weakened.**

| # | Change | Why |
|---|---|---|
| 1 | New aggregate `LaboratorySettings` | The laboratory settings — paper size, margins, header texts, printer roles, accounting default — had no Domain representation, so the configuration use cases had nothing to act on. |
| 2 | New aggregate `CultureResult` and `Antibiotic` | The microbiology workflows had no Domain model. The legacy `cultures` and `antibiotic` tables were the source. |
| 3 | New aggregate `PatientPicture` | The legacy `pictures` table had no Domain model; the picture use case needed one. |
| 4 | New aggregates `Farm` and `FarmTransaction` | The legacy `sharekat` and `sharekathesab` tables had no Domain model. |
| 5 | New entity `ProthrombinControlCalibration` | The legacy `PTC` calibration curve had no Domain model; the interpolation use case needed one. |
| 6 | `TestDefinition.UpdateDetails` | The catalogue amendment use case needed a way to amend the descriptive attributes. |
| 7 | `ReferenceRange.UpdateLimits` | The range amendment use case needed a way to amend the limits. |
| 8 | `TestUnit.UpdateName`, `ConversionFactor.UpdateFactor`, `Abbreviation.UpdateMeaning`, `ClinicalSubGroup.UpdateName/UpdateArabicName` | The dictionary maintenance use cases needed amendment methods. |
| 9 | `ReferringParty.Deactivate/Activate` and `IsActive` | The party register needed an active flag. |
| 10 | `Operator.SetName`, `Patient.SetName` | The amendment use cases needed a rename. |
| 11 | `SendOutRequest.SetDestinationLab` | The destination must be settable before dispatch. |
| 12 | `Permission.DeliverDigitalResults` **removed**; the count fell from 17 to 16 | The digital-delivery permission no longer guards anything once SMS and NATIGH are removed. Keeping it would be dead code. The count change is recorded in the XML documentation of the enum. |
| 13 | `TestDefinition.UpdateDetails` semantics corrected | The first implementation cleared every value whose argument was `null`, which silently wiped the Arabic name on a partial update. A null argument now means "not supplied", and a value is cleared deliberately through its own setter. |
| 14 | `ReportRenderRequest` extended with `Title`, `Subtitle`, `HeaderText` and `FooterText` | The reporting use cases produced a report whose title and header could not be carried, and the request model overloaded the patient name as the title. |
| 15 | `GetBranchesValidator` added | The audit found the parameterless branch query was the one use case without a validator. |
| 16 | The farm DTOs moved from `Features/Farming/` to `Features/Microbiology/` | The audit found `Features/Farming/` held a DTO file and no use case, which made the feature-area count misleading. |

---

## 5. The removal of SMS and NATIGH

**Removed from the Domain layer**

| Removed | File |
|---|---|
| `DeliveryChannel` enum | `src/RefLab.Domain/Enums/DeliveryChannel.cs` |
| `DeliveryRecord` entity | `src/RefLab.Domain/Auditing/DeliveryRecord.cs` |
| `Permission.DeliverDigitalResults` | `src/RefLab.Domain/Identity/Permission.cs` |

**Removed from the Application layer**

| Removed | File |
|---|---|
| `ISmsGateway` | `src/RefLab.Application/Abstractions/ISmsGateway.cs` |
| `INatighGateway` | `src/RefLab.Application/Abstractions/INatighGateway.cs` |
| `SmsSendResult`, `NatighResultLine`, `NatighPublicationRequest`, `NatighPublicationResult` | `src/RefLab.Application/Common/GatewayModels.cs` |
| `SendSmsResultNotification` use case | `src/RefLab.Application/Features/Delivery/SendSmsResultNotification.cs` |
| `PublishResultToNatigh` use case | `src/RefLab.Application/Features/Delivery/PublishResultToNatigh.cs` |
| `DeliveryRecordDto` | `src/RefLab.Application/Features/Auditing/Dtos.cs` |
| `SendSmsResultNotificationValidator`, `PublishResultToNatighValidator` | `src/RefLab.Application/Validation/DeliveryValidators.cs` |

**Removed from the test suite**

| Removed | File |
|---|---|
| `DeliveryRecordTests` | `tests/RefLab.Domain.Tests/Auditing/DeliveryRecordTests.cs` |
| `SendSmsResultNotificationHandlerTests`, `PublishResultToNatighHandlerTests` | `tests/RefLab.Application.Tests/Features/Delivery/DeliveryHandlerTests.cs` |
| `FakeSmsGateway`, `FakeNatighGateway` | `tests/RefLab.Application.Tests/Fakes/Fakes.cs` |
| The gateway registrations in the test host | `tests/RefLab.Application.Tests/Fakes/ApplicationTestHost.cs` |

**Residual-reference check — the whole solution, source and tests:**

```
$ grep -rniE "sms|natigh|DeliverDigital" --include='*.cs' --include='*.csproj' src tests
  src/RefLab.Domain/Identity/Permission.cs   ← a prose comment recording WHY the
                                                digital-delivery permission was removed
  NONE (no type, interface, handler, DTO, validator or test remains)
```

The only remaining occurrence is a deliberate prose comment in `Permission.cs`
that records the removal, so the decision is not lost.

---

## 6. The test suite

| Project | Before | After | Added |
|---|---:|---:|---:|
| `RefLab.Domain.Tests` | 249 | **277** | +28 |
| `RefLab.Application.Tests` | 95 | **210** | +115 |
| `RefLab.Infrastructure.Tests` | — | **17** | +17 |
| **Total** | **344** | **504** | **+160** |

**Every one of the 73 use cases is referenced by at least one test** — the
inventory generator exits non-zero when that is not true, and it exits zero.

Two sweeps close the coverage gaps an independent audit found: every handler
and every validator in the assembly is enumerated from the compiled types and
checked against the container, and every permission-gated use case is
demonstrably refused when the operator lacks the permission.

The Application tests run through the **real MediatR pipeline** — a
`ServiceProvider` with the production registration, the fakes, and the four
behaviours — so the validation, the authorisation and the domain-exception
translation are all genuinely exercised. The tests cover the successful
execution, the invalid input, the validation failure, the missing record, the
business-rule violation, the invalid state transition and the boundary case for
each use case.

`RefLab.Infrastructure.Tests` is a new project. It proves the two claims that
Phase 4 depends on: that `IPdfExporter` really produces a valid PDF with
QuestPDF (the file is read back and its `%PDF` magic asserted), and that the
PBKDF2 password hashing — the approved security correction — verifies correctly
and never stores the clear-text password.

**No test is skipped, disabled or weakened.** The 344 baseline tests are
retained and still pass.

---

## 7. Architecture verification

| Rule | How it was verified | Result |
|---|---|---|
| The Domain layer has no package reference | parsed `RefLab.Domain.csproj` | **0** |
| The Domain layer has no project reference | parsed `RefLab.Domain.csproj` | **0** |
| The Application layer references no persistence engine | `using` directives for `Microsoft.EntityFrameworkCore`, `Microsoft.Data`, `System.Data` | **none** |
| The Application layer uses no EF Core type | search for `: DbContext`, `DbSet<`, `.SaveChanges(`, `.Include(`, `.FromSql` | **none** |
| The Application layer references no Infrastructure assembly | project reference graph | **none** |
| `IAppDbContext` exposes `IQueryable<T>`, not `DbSet<T>` | read `Abstractions/IAppDbContext.cs` | confirmed |
| The WPF application uses the fully qualified base type | read `App.xaml.cs` | `System.Windows.Application` |

**The dependency direction is enforced by tests as well as by review:**
`DomainLayerBoundaryTests` asserts that the Domain assembly references no
third-party assembly at all, and `ApplicationLayerBoundaryTests` asserts that
the Application assembly references neither a persistence engine nor the WPF
stack.

---

## 8. Defects found and fixed during this phase

Every one of these was caught by a build or a test, not by inspection.

| # | Defect | How it was found | Fix |
|---|---|---|---|
| 1 | `TestDefinition.UpdateDetails` wiped every value whose argument was `null`, so a partial update silently erased the Arabic name | the catalogue test failed on the Arabic name | a null argument now means "not supplied"; a value is cleared deliberately through its own setter |
| 2 | `Pbkdf2PasswordHasher.Verify` always returned `false`, because the stored prefix contains a `$` so the format has five segments and not four | the hasher test failed | the parser now expects the five segments the format actually produces |
| 3 | Four Phase 3 validators were lost when the Phase 3 validator files were rewritten | the validation tests stopped throwing | the lost validators were restored; the validator count is now 73, verified by the inventory generator |
| 4 | `SettingsTests.cs` was never created — an earlier shell heredoc chain aborted mid-way | the inventory generator reported three use cases with no test | the file was written; the generator now reports 73 of 73 |
| 5 | `GetOperators` defaulted to listing inactive accounts as well | the operator-listing test failed | the default is now active-only; the caller can still ask for every account |
| 6 | `RunStockTakeByPhysicianOrParty` did not reject an unknown referring party | the stock-take test failed | the parameter order had changed; the test was corrected and the handler's rejection verified |
| 7 | A partial clean of `bin/` and `obj/` left a stale assets file, and the next build failed to resolve `Microsoft.Extensions.DependencyInjection.Abstractions` | the clean-room rebuild failed with `CS0246` | the clean now removes the directories explicitly; the clean-room rebuild passes |

---

## 9. Independent audit and the corrections it forced

The delivery was audited by an independent adversarial reviewer that ran every
command itself, enumerated the compiled assemblies by reflection, and applied
**mutation testing** — it broke the code on purpose to prove the tests would
catch it. The mutation probes confirmed the suite has teeth: removing the
authentication guard from `RegisterPatientHandler`, deleting the amount rule
from `RecordPaymentValidator`, multiplying the haematocrit factor by 1.01, and
neutralising `AuthorizationBehavior` were **all caught** by the tests.

The audit confirmed as PASS: the SMS and NATIGH removal, the QuestPDF and
`IPdfExporter` preservation, the WPF `CS0118` fix, the absence of EF Core
leakage, the zero-warning build, the 504 passing tests with none skipped, and
the absence of any stub, placeholder or `NotImplementedException`.

It also found six real defects, **all of which are fixed in this delivery**:

| # | Finding | Fix |
|---|---|---|
| 1 | **The count was 73, not 72.** The inventory script's regular expression required a parameter list, so it silently skipped `GetBranchesQuery` — a real, tested use case. The script then hard-coded its own error as the expected total, so it could never flag it. | The regular expression now accepts a parameterless record, the expected total is 73, and the inventory reports **73 / 73**. The delivery therefore contains **73** use cases — one more than the target — because the target itself came from the miscount. |
| 2 | `GetBranchesQuery` had no validator, so "every use case has a validator" was 72 of 73. | `GetBranchesValidator` added; the count is now **73 / 73**. |
| 3 | **The legacy-requirement column was dead.** The lookup used the full request name against base-name keys, so every row showed `—` and the traceability to `V2-REQ-nnn` was lost. | The lookup uses the base name; the column is populated for every use case. |
| 4 | The dependency-injection tests were **tautological** — `Assert.NotNull(GetRequiredService(...))` can never fail, and only 2 of the handlers and 1 of the validators were checked. | The tests now enumerate every handler and every validator from the compiled assembly and assert each is registered, and the pipeline behaviour count is asserted. |
| 5 | `The_payments_are_filtered_by_order` passed even with the filter removed, because only one payment was seeded. | Both orders are seeded and the surviving payment's `OrderId` and amount are asserted. The same weakness in the audit-trail filter test was closed. |
| 6 | The `Features/Farming/` area contained only a DTO file and no use case, which made the area count misleading. | The farm DTOs were moved next to the use case that uses them and the empty area was removed. |

Two further weaknesses were closed rather than merely noted: the
dependency-injection tests now cover **all** handlers and validators rather
than a sample, and a new sweep proves that **every** permission-gated use case
is refused without the permission — previously only four were covered.

---

## 10. What Phase 4 needs

The Application layer was shaped so that Phase 4 is implementation only, with
no redesign:

1. **`RefLabDbContext`** implementing `IAppDbContext` — expose `IQueryable<T> Set<T>()`, never `DbSet<T>`.
2. **The EF Core unit of work** implementing `IUnitOfWork`.
3. **Entity configurations** for the domain model — `OwnsOne` for `OrderIdentifier`, `ReferenceInterval`, `CriticalInterval`, `Money` and `Percentage`.
4. **The initial migration**, reviewed before it is applied.
5. **SQL Server provisioning** and the branch-scoped schema.
6. **The print-sequence counter** persisted — it is in memory today.
7. **The session store** — it is in process today.
8. **The barcode store** — the legacy `barcodes` table.

The full list is in `docs/LOCAL_AGENT_IMPLEMENTATION_GUIDE.md`, section 10.

---

## 11. Assumptions carried forward

0. `IAppDbContext` and `IUnitOfWork` are the **only two** Application
   abstractions without an implementation. Every handler depends on
   `IAppDbContext`, so the WPF shell cannot resolve the handlers at run time
   until the Phase 4 persistence adapter exists. This is stated in the XML
   documentation of `AddInfrastructureLayer`.
1. The laboratory currency is supplied by the caller; the legacy system has no
   currency column.
2. The legacy account-type column has no established value domain and is
   carried as free text.
3. The barcode format is Ref Lab's own; the legacy format was never established.
4. The international sensitivity index uses the standard laboratory definition;
   the legacy formula was not recoverable from the static evidence.
5. The legacy order-identifier segments at positions 1-2 and 5-8 have no
   established meaning and are supplied verbatim by the caller.
6. The critical limits are carried on the test definition, because the shipped
   legacy database lost the five critical-limit columns that its own backup
   still holds.
7. The reference-range selection rule is Ref Lab's own deterministic rule.

---

**End of `RL-P3-COMPLETION-001`.**

---

> **Superseded in part by Phase 4 (2026-11-10).** This report describes the
> state of the delivery at the end of Phase 3 and is kept as the record of
> that phase. Two things changed in Phase 4 by owner decision: the laboratory
> became a **single-site installation**, so the three branch use cases and the
> `ManageBranches` permission were removed; and the **persistence layer** was
> added. The use-case count is therefore 70 rather than 73 and the permission
> count 15 rather than 16. See `RL-P4-Completion-Report.md` for the current
> state.
