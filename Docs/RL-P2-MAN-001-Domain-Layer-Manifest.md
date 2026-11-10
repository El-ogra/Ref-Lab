# Ref Lab — Phase 2 Delivery Manifest: Domain Layer

**Document ID:** `RL-P2-MAN-001`  
**Generated:** 2026-11-09  
**Scope:** the complete Domain layer (sub-units 2.1 to 2.6) plus its xUnit test suite

---

## 1. Verification status

| Gate | Result |
|---|---|
| Clean-room restore (cold NuGet cache) | PASS |
| Build `Debug` | 0 errors, 0 warnings |
| Build `Release` | 0 errors, 0 warnings |
| `dotnet test` (Debug) | 256 passed, 0 failed |
| `dotnet test` (Release) | 256 passed, 0 failed |
| `RefLab.Domain` package references | 0 |
| `RefLab.Domain` project references | 0 |

## 2. How to apply this delivery

1. Extract this archive **into** `C:\Users\LAP LINK\source\repos\Ref Lab`.
2. Choose *overwrite existing files*. Exactly two files already exist and are updated:
   - `README.md`
   - `tests/RefLab.Domain.Tests/Architecture/DomainLayerBoundaryTests.cs`
3. All other files are new; their directories are created automatically.
4. Run the build and test commands in section 6.

## 3. File manifest

**Total files: 93** — 91 new, 2 replaced.

| # | Path | Layer | Bytes | SHA-256 (first 16) |
|---:|---|---|---:|---|
| 1 | `README.md` | Project documentation | 7987 | `e1bfeae505919e9e` |
| 2 | `src/RefLab.Domain/Auditing/AttendanceEntry.cs` | src/RefLab.Domain/Auditing | 2473 | `d67be84c89c93b72` |
| 3 | `src/RefLab.Domain/Auditing/DeliveryRecord.cs` | src/RefLab.Domain/Auditing | 3688 | `d43c6ede15480ea8` |
| 4 | `src/RefLab.Domain/Auditing/PrintAudit.cs` | src/RefLab.Domain/Auditing | 2509 | `52f55f1f27ad24f2` |
| 5 | `src/RefLab.Domain/Billing/Payment.cs` | src/RefLab.Domain/Billing | 3640 | `47f2f16718a7be03` |
| 6 | `src/RefLab.Domain/Billing/PriceList.cs` | src/RefLab.Domain/Billing | 2277 | `1498cb710c9c34b4` |
| 7 | `src/RefLab.Domain/Billing/ReferringParty.cs` | src/RefLab.Domain/Billing | 6299 | `37e57447971e6a27` |
| 8 | `src/RefLab.Domain/Billing/TreatingPhysician.cs` | src/RefLab.Domain/Billing | 2355 | `44f605c9f89209e0` |
| 9 | `src/RefLab.Domain/ClinicalKnowledge/Abbreviation.cs` | src/RefLab.Domain/ClinicalKnowledge | 1626 | `714f92855c9f0d4d` |
| 10 | `src/RefLab.Domain/ClinicalKnowledge/ClinicalConstant.cs` | src/RefLab.Domain/ClinicalKnowledge | 4018 | `357def56e9fa5f1b` |
| 11 | `src/RefLab.Domain/ClinicalKnowledge/ClinicalSubGroup.cs` | src/RefLab.Domain/ClinicalKnowledge | 1983 | `9abea6ce46fb0257` |
| 12 | `src/RefLab.Domain/ClinicalKnowledge/ConversionFactor.cs` | src/RefLab.Domain/ClinicalKnowledge | 2984 | `80c87ca58a90ab23` |
| 13 | `src/RefLab.Domain/ClinicalKnowledge/InvalidClinicalConstantException.cs` | src/RefLab.Domain/ClinicalKnowledge | 700 | `3e6e38eb85fe85ef` |
| 14 | `src/RefLab.Domain/ClinicalKnowledge/ReferenceRange.cs` | src/RefLab.Domain/ClinicalKnowledge | 5511 | `c007f53884f57374` |
| 15 | `src/RefLab.Domain/ClinicalKnowledge/TestDefinition.cs` | src/RefLab.Domain/ClinicalKnowledge | 10035 | `48ecef84ae40b87c` |
| 16 | `src/RefLab.Domain/ClinicalKnowledge/TestUnit.cs` | src/RefLab.Domain/ClinicalKnowledge | 1231 | `7a9aab300ef6a486` |
| 17 | `src/RefLab.Domain/Enums/AgeUnit.cs` | src/RefLab.Domain/Enums | 523 | `f3663535b8a9ba31` |
| 18 | `src/RefLab.Domain/Enums/AttendanceEventType.cs` | src/RefLab.Domain/Enums | 703 | `ebaefb43f37c8fc2` |
| 19 | `src/RefLab.Domain/Enums/DeliveryChannel.cs` | src/RefLab.Domain/Enums | 736 | `913d19bce249df92` |
| 20 | `src/RefLab.Domain/Enums/RecordStatus.cs` | src/RefLab.Domain/Enums | 1001 | `f81677873039949b` |
| 21 | `src/RefLab.Domain/Enums/ReportCategory.cs` | src/RefLab.Domain/Enums | 884 | `3d65d63979e55bad` |
| 22 | `src/RefLab.Domain/Enums/ResultClassification.cs` | src/RefLab.Domain/Enums | 1321 | `42782ae886076ac8` |
| 23 | `src/RefLab.Domain/Enums/ResultState.cs` | src/RefLab.Domain/Enums | 939 | `2939116b6b9a66ca` |
| 24 | `src/RefLab.Domain/Enums/SampleDispositionAction.cs` | src/RefLab.Domain/Enums | 825 | `ab5c6a5003fa6ceb` |
| 25 | `src/RefLab.Domain/Enums/SendOutState.cs` | src/RefLab.Domain/Enums | 994 | `5f9983f2e9a103d4` |
| 26 | `src/RefLab.Domain/Enums/Sex.cs` | src/RefLab.Domain/Enums | 415 | `05bcc85c0d0c0cf0` |
| 27 | `src/RefLab.Domain/Enums/SexScope.cs` | src/RefLab.Domain/Enums | 559 | `a2a83461264aa12c` |
| 28 | `src/RefLab.Domain/Exceptions/BillingExceptions.cs` | src/RefLab.Domain/Exceptions | 2043 | `1669c8c43d3f56a5` |
| 29 | `src/RefLab.Domain/Exceptions/CatalogueExceptions.cs` | src/RefLab.Domain/Exceptions | 1541 | `120585ff95359f44` |
| 30 | `src/RefLab.Domain/Exceptions/DomainException.cs` | src/RefLab.Domain/Exceptions | 823 | `573eeb0a82fa16a7` |
| 31 | `src/RefLab.Domain/Exceptions/PatientExceptions.cs` | src/RefLab.Domain/Exceptions | 2537 | `cc4366b044355c3d` |
| 32 | `src/RefLab.Domain/Exceptions/ReferenceRangeExceptions.cs` | src/RefLab.Domain/Exceptions | 2749 | `5ce0b36141e3dddb` |
| 33 | `src/RefLab.Domain/Exceptions/SecurityExceptions.cs` | src/RefLab.Domain/Exceptions | 1515 | `83d36196d5a9e1f1` |
| 34 | `src/RefLab.Domain/Exceptions/SendOutExceptions.cs` | src/RefLab.Domain/Exceptions | 690 | `5f02926fc2d0808c` |
| 35 | `src/RefLab.Domain/Identity/Branch.cs` | src/RefLab.Domain/Identity | 1947 | `19196d3d173d8307` |
| 36 | `src/RefLab.Domain/Identity/Operator.cs` | src/RefLab.Domain/Identity | 7427 | `337900b4a0c1a015` |
| 37 | `src/RefLab.Domain/Identity/PasswordPolicy.cs` | src/RefLab.Domain/Identity | 2574 | `d7be0735b6375f4c` |
| 38 | `src/RefLab.Domain/Identity/Permission.cs` | src/RefLab.Domain/Identity | 2359 | `dccb8a41750b095e` |
| 39 | `src/RefLab.Domain/Patients/OrderTest.cs` | src/RefLab.Domain/Patients | 15502 | `509e056332b4b966` |
| 40 | `src/RefLab.Domain/Patients/Patient.cs` | src/RefLab.Domain/Patients | 5121 | `3d65680a71ace55a` |
| 41 | `src/RefLab.Domain/Patients/PatientHistory.cs` | src/RefLab.Domain/Patients | 3443 | `38ec0aafddc61a51` |
| 42 | `src/RefLab.Domain/Patients/PatientOrder.cs` | src/RefLab.Domain/Patients | 10009 | `4e4e408e64ea1b24` |
| 43 | `src/RefLab.Domain/Patients/ReferenceSnapshot.cs` | src/RefLab.Domain/Patients | 2768 | `9f00ff1a2647606b` |
| 44 | `src/RefLab.Domain/Rules/ClinicalConstantResolver.cs` | src/RefLab.Domain/Rules | 2277 | `c88a23e8f19377b8` |
| 45 | `src/RefLab.Domain/Rules/ClinicalDerivationPolicy.cs` | src/RefLab.Domain/Rules | 8516 | `5ce3a4a5db75704c` |
| 46 | `src/RefLab.Domain/Rules/CommissionCalculator.cs` | src/RefLab.Domain/Rules | 2959 | `d71cb977acb6746b` |
| 47 | `src/RefLab.Domain/Rules/IdentifierAllocator.cs` | src/RefLab.Domain/Rules | 3163 | `c0290379fc14632e` |
| 48 | `src/RefLab.Domain/Rules/InvalidClinicalDerivationException.cs` | src/RefLab.Domain/Rules | 362 | `cd4f878d9f60f9d0` |
| 49 | `src/RefLab.Domain/Rules/ProthrombinControlPoint.cs` | src/RefLab.Domain/Rules | 515 | `01881411c867bc80` |
| 50 | `src/RefLab.Domain/Rules/ReferenceRangeSelector.cs` | src/RefLab.Domain/Rules | 2338 | `fccdd151ab6f1858` |
| 51 | `src/RefLab.Domain/Rules/ResultClassificationPolicy.cs` | src/RefLab.Domain/Rules | 2661 | `ad43a314a97a0985` |
| 52 | `src/RefLab.Domain/SendOut/SampleDisposition.cs` | src/RefLab.Domain/SendOut | 2760 | `5612fb8125c3aa55` |
| 53 | `src/RefLab.Domain/SendOut/SendOutRequest.cs` | src/RefLab.Domain/SendOut | 4254 | `14dd63a72662940c` |
| 54 | `src/RefLab.Domain/ValueObjects/AgeBand.cs` | src/RefLab.Domain/ValueObjects | 2938 | `13a2e80e2d2f25dd` |
| 55 | `src/RefLab.Domain/ValueObjects/CriticalInterval.cs` | src/RefLab.Domain/ValueObjects | 4185 | `b9279ee3bf12e844` |
| 56 | `src/RefLab.Domain/ValueObjects/Money.cs` | src/RefLab.Domain/ValueObjects | 6145 | `81edda591710dbc5` |
| 57 | `src/RefLab.Domain/ValueObjects/OrderIdentifier.cs` | src/RefLab.Domain/ValueObjects | 6242 | `b1923fcb83f491fe` |
| 58 | `src/RefLab.Domain/ValueObjects/PatientId.cs` | src/RefLab.Domain/ValueObjects | 2360 | `eecebb8240e9aee8` |
| 59 | `src/RefLab.Domain/ValueObjects/Percentage.cs` | src/RefLab.Domain/ValueObjects | 1719 | `05156115f366e1c0` |
| 60 | `src/RefLab.Domain/ValueObjects/ReferenceInterval.cs` | src/RefLab.Domain/ValueObjects | 6187 | `96ca1080f93f1b14` |
| 61 | `src/RefLab.Domain/ValueObjects/ResultValue.cs` | src/RefLab.Domain/ValueObjects | 3385 | `3eb16b9d799665ba` |
| 62 | `tests/RefLab.Domain.Tests/Architecture/DomainLayerBoundaryTests.cs` | tests/RefLab.Domain.Tests | 2222 | `bb6e8a7222b5a1b3` |
| 63 | `tests/RefLab.Domain.Tests/Auditing/DeliveryRecordTests.cs` | tests/RefLab.Domain.Tests | 1528 | `f476cc19571ccd5e` |
| 64 | `tests/RefLab.Domain.Tests/Auditing/PrintAuditTests.cs` | tests/RefLab.Domain.Tests | 1629 | `b648f6f66f4fb851` |
| 65 | `tests/RefLab.Domain.Tests/Billing/PaymentTests.cs` | tests/RefLab.Domain.Tests | 1500 | `3cfbdbbaa03c9146` |
| 66 | `tests/RefLab.Domain.Tests/Billing/ReferringPartyTests.cs` | tests/RefLab.Domain.Tests | 1736 | `33cce6a39e18cdd0` |
| 67 | `tests/RefLab.Domain.Tests/ClinicalKnowledge/ClinicalConstantTests.cs` | tests/RefLab.Domain.Tests | 2039 | `f8272dcc8bc1331d` |
| 68 | `tests/RefLab.Domain.Tests/ClinicalKnowledge/ConversionFactorTests.cs` | tests/RefLab.Domain.Tests | 1139 | `df4bbf8129053d0a` |
| 69 | `tests/RefLab.Domain.Tests/ClinicalKnowledge/ReferenceRangeTests.cs` | tests/RefLab.Domain.Tests | 2569 | `49198379c6945508` |
| 70 | `tests/RefLab.Domain.Tests/ClinicalKnowledge/TestDefinitionTests.cs` | tests/RefLab.Domain.Tests | 4319 | `b1611276f05e0bba` |
| 71 | `tests/RefLab.Domain.Tests/Identity/BranchTests.cs` | tests/RefLab.Domain.Tests | 1085 | `f170a2f18aa5a3a1` |
| 72 | `tests/RefLab.Domain.Tests/Identity/OperatorTests.cs` | tests/RefLab.Domain.Tests | 4314 | `abecec3bdbdc447f` |
| 73 | `tests/RefLab.Domain.Tests/Identity/PasswordPolicyTests.cs` | tests/RefLab.Domain.Tests | 1760 | `1f7b293f81435ea6` |
| 74 | `tests/RefLab.Domain.Tests/Identity/PermissionTests.cs` | tests/RefLab.Domain.Tests | 690 | `458595b409c89f46` |
| 75 | `tests/RefLab.Domain.Tests/Patients/OrderTestTests.cs` | tests/RefLab.Domain.Tests | 7746 | `517529d17e3dbf9e` |
| 76 | `tests/RefLab.Domain.Tests/Patients/PatientHistoryTests.cs` | tests/RefLab.Domain.Tests | 1825 | `4ed9e2bd8b41d33e` |
| 77 | `tests/RefLab.Domain.Tests/Patients/PatientOrderTests.cs` | tests/RefLab.Domain.Tests | 4259 | `0b5ec78d8897204b` |
| 78 | `tests/RefLab.Domain.Tests/Patients/PatientTests.cs` | tests/RefLab.Domain.Tests | 2094 | `062e607144cd80f0` |
| 79 | `tests/RefLab.Domain.Tests/Rules/ClinicalConstantResolverTests.cs` | tests/RefLab.Domain.Tests | 1681 | `7042f212265a0e85` |
| 80 | `tests/RefLab.Domain.Tests/Rules/ClinicalDerivationPolicyTests.cs` | tests/RefLab.Domain.Tests | 4965 | `1d17cf6012e3bd58` |
| 81 | `tests/RefLab.Domain.Tests/Rules/CommissionCalculatorTests.cs` | tests/RefLab.Domain.Tests | 2373 | `1b12cebde8540809` |
| 82 | `tests/RefLab.Domain.Tests/Rules/IdentifierAllocatorTests.cs` | tests/RefLab.Domain.Tests | 2390 | `409dcbd2e85fd132` |
| 83 | `tests/RefLab.Domain.Tests/Rules/ReferenceRangeSelectorTests.cs` | tests/RefLab.Domain.Tests | 2801 | `7663809319b29fc3` |
| 84 | `tests/RefLab.Domain.Tests/Rules/ResultClassificationPolicyTests.cs` | tests/RefLab.Domain.Tests | 5414 | `911ed95748874517` |
| 85 | `tests/RefLab.Domain.Tests/SendOut/SampleDispositionTests.cs` | tests/RefLab.Domain.Tests | 1350 | `4e599e74a538d52e` |
| 86 | `tests/RefLab.Domain.Tests/SendOut/SendOutRequestTests.cs` | tests/RefLab.Domain.Tests | 2460 | `685459470dabdc8e` |
| 87 | `tests/RefLab.Domain.Tests/ValueObjects/AgeBandTests.cs` | tests/RefLab.Domain.Tests | 1804 | `d734fa035541b624` |
| 88 | `tests/RefLab.Domain.Tests/ValueObjects/CriticalIntervalTests.cs` | tests/RefLab.Domain.Tests | 1453 | `10eecdf3def9483f` |
| 89 | `tests/RefLab.Domain.Tests/ValueObjects/MoneyTests.cs` | tests/RefLab.Domain.Tests | 3005 | `de7b51ababc3cfac` |
| 90 | `tests/RefLab.Domain.Tests/ValueObjects/OrderIdentifierTests.cs` | tests/RefLab.Domain.Tests | 4469 | `70d239129442c1f3` |
| 91 | `tests/RefLab.Domain.Tests/ValueObjects/PercentageTests.cs` | tests/RefLab.Domain.Tests | 1031 | `6690ffbd58dad59f` |
| 92 | `tests/RefLab.Domain.Tests/ValueObjects/ReferenceIntervalTests.cs` | tests/RefLab.Domain.Tests | 2915 | `4f4d8d5776e9e36a` |
| 93 | `tests/RefLab.Domain.Tests/ValueObjects/ResultValueTests.cs` | tests/RefLab.Domain.Tests | 1927 | `983578486bfa0dff` |

## 4. Sub-unit coverage

| Folder | Files | Content |
|---|---:|---|
| `Project documentation` | 1 |  |
| `src/RefLab.Domain/Auditing` | 3 | Auditing |
| `src/RefLab.Domain/Billing` | 4 | Billing |
| `src/RefLab.Domain/ClinicalKnowledge` | 8 | Clinical knowledge |
| `src/RefLab.Domain/Enums` | 11 | Enumerations |
| `src/RefLab.Domain/Exceptions` | 7 | Domain exceptions |
| `src/RefLab.Domain/Identity` | 4 | Identity |
| `src/RefLab.Domain/Patients` | 5 | Patient cycle |
| `src/RefLab.Domain/Rules` | 8 | Domain rules |
| `src/RefLab.Domain/SendOut` | 2 | Send-out |
| `src/RefLab.Domain/ValueObjects` | 8 | Value objects |
| `tests/RefLab.Domain.Tests` | 32 | Domain tests |

## 5. Design decisions carried into this phase

| # | Decision |
|---|---|
| Q1 | A unified result model replaces the 51 legacy profile tables. The legacy `patienttest` remains the transactional centre. |
| Q2 | Only security and data-integrity flaws are corrected. All other behaviour is matched exactly. |
| Q5 | The Domain layer carries no package reference and no project reference. |
| Q6 | Monetary amounts use `decimal`; the legacy system used `Double`. |
| Q7 | Reference-range selection is deterministic: sex-specific beats sex-agnostic, then the narrowest age band, then the lowest range identifier. |
| Q8 | The `Branch` identifier is restricted to a single digit because it is written into position 9 of the 13-character order identifier. |

## 6. Build and verify commands

```powershell
cd "C:\Users\LAP LINK\source\repos\Ref Lab"
dotnet restore RefLab.sln
dotnet build   RefLab.sln -c Debug
dotnet build   RefLab.sln -c Release
dotnet test    RefLab.sln -c Release
```

**Expected:** restore succeeds · both builds report **0 errors, 0 warnings** · **256 tests pass, 0 fail**.

---

*End of `RL-P2-MAN-001`.*

---

> **Superseded in part by Phase 4 (2026-11-10).** This manifest describes the
> Domain layer as it was delivered in Phase 2 and is kept as the record of
> that phase. In Phase 4 the laboratory became a **single-site installation**,
> so `src/RefLab.Domain/Identity/Branch.cs` and
> `tests/RefLab.Domain.Tests/Identity/BranchTests.cs` were deleted and the
> `OrderIdentifier` layout changed. The file manifest above therefore no longer
> matches the tree. See `RL-P4-Completion-Report.md` for the current state.
