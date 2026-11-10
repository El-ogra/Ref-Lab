# Ref Lab — Phase 3 Delivery Manifest: Application Layer

**Document ID:** `RL-P3-MAN-001`  
**Scope:** the complete Phase 3 Application layer, the Domain changes it required, the QuestPDF infrastructure adapters, and the complete test suite.

---

## 1. Verification status

| Gate | Result |
|---|---|
| Use cases implemented | **73 / 73** |
| Use cases with a FluentValidation validator | **73 / 73** |
| Use cases covered by a test | **73 / 73** |
| Commands / queries | 46 / 27 |
| Feature areas | 15 |
| Clean-room restore (cold NuGet cache) | PASS |
| Build `Debug` | 0 errors, 0 warnings |
| Build `Release` | 0 errors, 0 warnings |
| Tests | **504 passed, 0 failed, 0 skipped** (277 Domain + 210 Application + 17 Infrastructure) |
| SMS / NATIGH residual references | none |
| EF Core leakage into the Application layer | none |
| `RefLab.Domain` package / project references | 0 / 0 |
| WPF `CS0118` fix | present |

## 2. File manifest

**Total files: 286**

| # | Path | Bytes | SHA-256 (first 16) |
|---:|---|---:|---|
| 1 | `.gitignore` | 591 | `af5a9644052ba7b1` |
| 2 | `Directory.Build.props` | 1249 | `2c0c54ee74261bc2` |
| 3 | `README.md` | 9371 | `d3e59233fc026cd6` |
| 4 | `RefLab.sln` | 9576 | `6f5df42a3376b5dd` |
| 5 | `docs/LOCAL_AGENT_IMPLEMENTATION_GUIDE.md` | 11916 | `bdb7d54b14de18fe` |
| 6 | `docs/RL-P0-RPT-001-Discovery-Report.md` | 59387 | `e67da9235bfbdb69` |
| 7 | `docs/RL-P2-MAN-001-Domain-Layer-Manifest.md` | 13989 | `cfa3c00b56001597` |
| 8 | `docs/RL-P3-Completion-Report.md` | 23949 | `d67af248e63456d0` |
| 9 | `docs/USE_CASE_INVENTORY.md` | 31034 | `937ae22061a23273` |
| 10 | `global.json` | 108 | `546b7a626451526a` |
| 11 | `scripts/generate_use_case_inventory.py` | 14071 | `eda00829e6522361` |
| 12 | `src/RefLab.Application/Abstractions/IAppDbContext.cs` | 616 | `996d943b59cdef83` |
| 13 | `src/RefLab.Application/Abstractions/IClock.cs` | 391 | `f51ea9eb72b27ffc` |
| 14 | `src/RefLab.Application/Abstractions/ICurrentUser.cs` | 768 | `25d452ed4d9f567a` |
| 15 | `src/RefLab.Application/Abstractions/IPasswordHasher.cs` | 640 | `cfa8ecb8a628423e` |
| 16 | `src/RefLab.Application/Abstractions/IPdfExporter.cs` | 768 | `64e9acf630213213` |
| 17 | `src/RefLab.Application/Abstractions/IPrintSequenceGenerator.cs` | 488 | `00e55622769e4f44` |
| 18 | `src/RefLab.Application/Abstractions/IReportRenderer.cs` | 356 | `87f18e0706c60b56` |
| 19 | `src/RefLab.Application/Abstractions/IUnitOfWork.cs` | 631 | `0fb3f4f7fd0bfd04` |
| 20 | `src/RefLab.Application/ApplicationServiceCollectionExtensions.cs` | 1316 | `97060d447bdc44d0` |
| 21 | `src/RefLab.Application/Behaviors/AuthorizationBehavior.cs` | 1608 | `3feaa215fb4ecd5b` |
| 22 | `src/RefLab.Application/Behaviors/DomainExceptionBehavior.cs` | 1001 | `d7b09492d7fc30c5` |
| 23 | `src/RefLab.Application/Behaviors/LoggingBehavior.cs` | 1711 | `80849225b6883dc5` |
| 24 | `src/RefLab.Application/Behaviors/ValidationBehavior.cs` | 1760 | `09330093a6973ed4` |
| 25 | `src/RefLab.Application/Common/ApplicationException.cs` | 831 | `5d84393a7bb23b0e` |
| 26 | `src/RefLab.Application/Common/ConcurrencyException.cs` | 705 | `6307a9b0d7e09a3e` |
| 27 | `src/RefLab.Application/Common/ConflictException.cs` | 340 | `10c3c5b76667f8dc` |
| 28 | `src/RefLab.Application/Common/ForbiddenException.cs` | 348 | `1812d749457edf7b` |
| 29 | `src/RefLab.Application/Common/NoResult.cs` | 152 | `bcf4c41086669ea4` |
| 30 | `src/RefLab.Application/Common/NotFoundException.cs` | 613 | `ef740ea96c2e784b` |
| 31 | `src/RefLab.Application/Common/PageRequest.cs` | 633 | `c1486f8cedd1c54d` |
| 32 | `src/RefLab.Application/Common/PagedResult.cs` | 1493 | `c25197653f407baf` |
| 33 | `src/RefLab.Application/Common/ReportModels.cs` | 1326 | `cf91e1f4705c955b` |
| 34 | `src/RefLab.Application/Common/RequestValidationException.cs` | 715 | `1af6a990a6b903be` |
| 35 | `src/RefLab.Application/Features/Auditing/Dtos.cs` | 1130 | `0967a68abd1826cf` |
| 36 | `src/RefLab.Application/Features/Auditing/GetPrintAuditTrail.cs` | 1676 | `0be6a11d87e162b1` |
| 37 | `src/RefLab.Application/Features/Auditing/RecordAttendance.cs` | 2746 | `aa44026bd8a1d888` |
| 38 | `src/RefLab.Application/Features/Billing/ComputeCommission.cs` | 2427 | `04985e5efc109e92` |
| 39 | `src/RefLab.Application/Features/Billing/Dtos.cs` | 3884 | `b40d68872fbe5f23` |
| 40 | `src/RefLab.Application/Features/Billing/GetPayments.cs` | 1861 | `582fbd52e74ba8c7` |
| 41 | `src/RefLab.Application/Features/Billing/GetReferringParty.cs` | 1486 | `03e311facfddad53` |
| 42 | `src/RefLab.Application/Features/Billing/RecordPayment.cs` | 2374 | `3961e3ad147a4a6b` |
| 43 | `src/RefLab.Application/Features/Billing/RunMultiBranchStockTake.cs` | 3252 | `781771435c1358ad` |
| 44 | `src/RefLab.Application/Features/Billing/RunPeriodStockTake.cs` | 2789 | `ac28d32005d90996` |
| 45 | `src/RefLab.Application/Features/Billing/RunStockTakeByPhysicianOrParty.cs` | 3282 | `b9862f60e08d4677` |
| 46 | `src/RefLab.Application/Features/Billing/SetDefaultAccountType.cs` | 2018 | `565cbe7a3883d300` |
| 47 | `src/RefLab.Application/Features/Branches/CreateBranch.cs` | 1532 | `f917f1c34479231c` |
| 48 | `src/RefLab.Application/Features/Branches/Dtos.cs` | 399 | `f23e515ad500bc49` |
| 49 | `src/RefLab.Application/Features/Branches/GetBranches.cs` | 1042 | `98397ef637c53764` |
| 50 | `src/RefLab.Application/Features/Catalogue/CreateReferenceRange.cs` | 2621 | `bbceadfcdc616c31` |
| 51 | `src/RefLab.Application/Features/Catalogue/CreateTestDefinition.cs` | 2380 | `b4ed53f597f1ed4f` |
| 52 | `src/RefLab.Application/Features/Catalogue/Dtos.cs` | 4767 | `7e22d31cd404d405` |
| 53 | `src/RefLab.Application/Features/Catalogue/GetReferenceRangeForPatient.cs` | 1880 | `4cc5165827edf6c4` |
| 54 | `src/RefLab.Application/Features/Catalogue/GetTestCatalogue.cs` | 2266 | `67276ef3e19f78b3` |
| 55 | `src/RefLab.Application/Features/Catalogue/GetTestProfile.cs` | 1985 | `efd7720e113e90e2` |
| 56 | `src/RefLab.Application/Features/Catalogue/ManageAbbreviations.cs` | 1948 | `70fe28e99ba0bc66` |
| 57 | `src/RefLab.Application/Features/Catalogue/ManageClinicalSubGroups.cs` | 2293 | `9a46d9fceadb27d0` |
| 58 | `src/RefLab.Application/Features/Catalogue/ManageUnitsAndConversions.cs` | 4198 | `e59d1b965d5b660c` |
| 59 | `src/RefLab.Application/Features/Catalogue/UpdateReferenceRange.cs` | 2035 | `1e5f7a15a1b0a696` |
| 60 | `src/RefLab.Application/Features/Catalogue/UpdateTestDefinition.cs` | 3024 | `90d5365f9f5ac93d` |
| 61 | `src/RefLab.Application/Features/Configuration/ConfigureConnectionReceipt.cs` | 2026 | `248884dc308e2b27` |
| 62 | `src/RefLab.Application/Features/Configuration/ConfigureLaboratorySettings.cs` | 3218 | `aeefae65bc57e279` |
| 63 | `src/RefLab.Application/Features/Configuration/CustomiseReportLayout.cs` | 2828 | `d6c12beccd27bddc` |
| 64 | `src/RefLab.Application/Features/Configuration/Dtos.cs` | 1647 | `4098423540ca8bd5` |
| 65 | `src/RefLab.Application/Features/Derivation/DeriveHematocrit.cs` | 1690 | `23642fc14fffa361` |
| 66 | `src/RefLab.Application/Features/Derivation/DeriveHemoglobinPercentage.cs` | 1889 | `8c4df329e6b894f4` |
| 67 | `src/RefLab.Application/Features/Derivation/DeriveInternationalSensitivityIndex.cs` | 1783 | `920b9e8200920ea4` |
| 68 | `src/RefLab.Application/Features/Derivation/InterpolateProthrombinControl.cs` | 1885 | `5989b25885dfd9ae` |
| 69 | `src/RefLab.Application/Features/Identity/AssignOperatorPermissions.cs` | 2482 | `f2f4aca5e96c0d31` |
| 70 | `src/RefLab.Application/Features/Identity/ChangePassword.cs` | 2194 | `0b4d104bc59cb882` |
| 71 | `src/RefLab.Application/Features/Identity/CreateOperator.cs` | 2930 | `d75c2c9334a6e5db` |
| 72 | `src/RefLab.Application/Features/Identity/Dtos.cs` | 970 | `3fc899c63cfa4c7e` |
| 73 | `src/RefLab.Application/Features/Identity/GetOperators.cs` | 1602 | `9a45d3c5426077de` |
| 74 | `src/RefLab.Application/Features/Identity/SignIn.cs` | 2432 | `ddeaa3ec59dc3104` |
| 75 | `src/RefLab.Application/Features/Identity/UpdateOperator.cs` | 2043 | `f531ebe5c9c79e1d` |
| 76 | `src/RefLab.Application/Features/Microbiology/Dtos.cs` | 2249 | `93dec2b3398d51b4` |
| 77 | `src/RefLab.Application/Features/Microbiology/FarmDtos.cs` | 1185 | `b7bfb9176d410acd` |
| 78 | `src/RefLab.Application/Features/Microbiology/GetCultureResults.cs` | 1807 | `e9f290317185844c` |
| 79 | `src/RefLab.Application/Features/Microbiology/ManageAntibioticCatalogue.cs` | 2640 | `18aeecdd9d587d3c` |
| 80 | `src/RefLab.Application/Features/Microbiology/ManageFarmResults.cs` | 2878 | `3667e4377fcd15aa` |
| 81 | `src/RefLab.Application/Features/Microbiology/RecordCultureResult.cs` | 2774 | `63ce64b4fa0058e2` |
| 82 | `src/RefLab.Application/Features/Parties/ManagePriceLists.cs` | 2006 | `9d69b951fbcf03de` |
| 83 | `src/RefLab.Application/Features/Parties/ManageTreatingPhysicians.cs` | 2117 | `a736bdef009db021` |
| 84 | `src/RefLab.Application/Features/Parties/PrintPriceList.cs` | 2729 | `9705f58dcf64765a` |
| 85 | `src/RefLab.Application/Features/Patients/AllocateOrderIdentifier.cs` | 2239 | `480aed1ff4ec70d5` |
| 86 | `src/RefLab.Application/Features/Patients/AttachPatientPicture.cs` | 2365 | `3379037f4fea81fa` |
| 87 | `src/RefLab.Application/Features/Patients/Dtos.cs` | 6974 | `ba4f03f4c07d26dd` |
| 88 | `src/RefLab.Application/Features/Patients/FollowUpCase.cs` | 3618 | `331ffb1e0c520df6` |
| 89 | `src/RefLab.Application/Features/Patients/GeneratePatientBarcode.cs` | 2162 | `a3e1c839b24ded80` |
| 90 | `src/RefLab.Application/Features/Patients/GetPatientHistory.cs` | 1252 | `73808f50acdea9de` |
| 91 | `src/RefLab.Application/Features/Patients/GetPatientOrder.cs` | 1450 | `e2e138b3c4dec00f` |
| 92 | `src/RefLab.Application/Features/Patients/RegisterPatient.cs` | 2827 | `4d0795214054fe73` |
| 93 | `src/RefLab.Application/Features/Patients/SearchPatients.cs` | 1919 | `de9cda037cad0fd7` |
| 94 | `src/RefLab.Application/Features/Reporting/ConfigurePrinters.cs` | 2100 | `5b83d6e274527b8e` |
| 95 | `src/RefLab.Application/Features/Reporting/ConfigureReportHeaderFooter.cs` | 2611 | `c186947673f0fc94` |
| 96 | `src/RefLab.Application/Features/Reporting/PrintPatientIdCard.cs` | 3140 | `1ae9e38c27da9681` |
| 97 | `src/RefLab.Application/Features/Reporting/PrintPatientInvoice.cs` | 3605 | `ed3cf035a9cc16c3` |
| 98 | `src/RefLab.Application/Features/Reporting/ProduceConsolidatedReport.cs` | 4519 | `248581f99011dfea` |
| 99 | `src/RefLab.Application/Features/Reporting/ProduceEmptyReport.cs` | 2406 | `33bab35668a4b742` |
| 100 | `src/RefLab.Application/Features/Results/Dtos.cs` | 2226 | `a9b2f8b68a370296` |
| 101 | `src/RefLab.Application/Features/Results/EnterAdditionalResult.cs` | 2135 | `a9a1ee48b5a3d8b9` |
| 102 | `src/RefLab.Application/Features/Results/EnterResult.cs` | 2174 | `9347fb37a9eaa34f` |
| 103 | `src/RefLab.Application/Features/Results/GetCriticalValues.cs` | 1692 | `25a638e612c62662` |
| 104 | `src/RefLab.Application/Features/Results/GetMethodSheet.cs` | 2015 | `677870be09fedb11` |
| 105 | `src/RefLab.Application/Features/Results/HandOverResults.cs` | 3223 | `e9b47acf2ce5233e` |
| 106 | `src/RefLab.Application/Features/Results/OrderTests.cs` | 4641 | `cb2bdc7423935094` |
| 107 | `src/RefLab.Application/Features/Results/PrintResultReport.cs` | 4566 | `570b731f11947407` |
| 108 | `src/RefLab.Application/Features/Results/ReviewResult.cs` | 1814 | `f8f6dbc7099d3ecf` |
| 109 | `src/RefLab.Application/Features/SendOut/CreateSendOutRequest.cs` | 2347 | `1c92e22380a4860e` |
| 110 | `src/RefLab.Application/Features/SendOut/DispatchSendOutSample.cs` | 1682 | `032004738f3cdb37` |
| 111 | `src/RefLab.Application/Features/SendOut/Dtos.cs` | 1663 | `08f36ce4a0a9f87e` |
| 112 | `src/RefLab.Application/Features/SendOut/GetSendOutQueue.cs` | 1705 | `693fcb29cc52ebcb` |
| 113 | `src/RefLab.Application/Features/SendOut/SettleSendOutSample.cs` | 1668 | `2e76d77e2e244f93` |
| 114 | `src/RefLab.Application/Features/SendOut/SplitOrWithdrawSample.cs` | 2284 | `f7d49ef5aef34e23` |
| 115 | `src/RefLab.Application/Features/Statistics/Dtos.cs` | 1511 | `fd62fdf9c0405be3` |
| 116 | `src/RefLab.Application/Features/Statistics/GetLaboratoryStatistics.cs` | 2978 | `4bf858d8bd215350` |
| 117 | `src/RefLab.Application/Features/Statistics/GetPerformanceStatistics.cs` | 2511 | `b6a6333b308a439b` |
| 118 | `src/RefLab.Application/Features/Worksheets/BuildPatientGroupWorkSheet.cs` | 4296 | `c9429a8ac129c41c` |
| 119 | `src/RefLab.Application/Features/Worksheets/BuildTestGroupWorkSheet.cs` | 4536 | `45159cd5742caf47` |
| 120 | `src/RefLab.Application/Features/Worksheets/Dtos.cs` | 801 | `a351c73aa11845ea` |
| 121 | `src/RefLab.Application/RefLab.Application.csproj` | 1218 | `e481fcca02cfc4f1` |
| 122 | `src/RefLab.Application/Validation/AuditingValidators.cs` | 1232 | `00e3e5e19a6434f2` |
| 123 | `src/RefLab.Application/Validation/BillingValidators.cs` | 4858 | `80fc1ed8effcffe3` |
| 124 | `src/RefLab.Application/Validation/BranchValidators.cs` | 1177 | `de338d9239ec89fd` |
| 125 | `src/RefLab.Application/Validation/CatalogueValidators.cs` | 8749 | `b5ded3da18a70f27` |
| 126 | `src/RefLab.Application/Validation/ConfigurationValidators.cs` | 2721 | `8f4be8d964697145` |
| 127 | `src/RefLab.Application/Validation/DerivationValidators.cs` | 2637 | `820c6fb886f19d46` |
| 128 | `src/RefLab.Application/Validation/IdentityValidators.cs` | 4654 | `3f2710b826aceb4a` |
| 129 | `src/RefLab.Application/Validation/MicrobiologyValidators.cs` | 3136 | `5367cde8f84c095b` |
| 130 | `src/RefLab.Application/Validation/PartiesValidators.cs` | 2203 | `2ebb0f2bca99fbcb` |
| 131 | `src/RefLab.Application/Validation/PatientValidators.cs` | 5567 | `296e0a0155015569` |
| 132 | `src/RefLab.Application/Validation/ReportingValidators.cs` | 3674 | `381d9758e0140d18` |
| 133 | `src/RefLab.Application/Validation/ResultValidators.cs` | 4317 | `fd4019b2b2a26afb` |
| 134 | `src/RefLab.Application/Validation/SendOutValidators.cs` | 2821 | `ed9c49d15506bc63` |
| 135 | `src/RefLab.Application/Validation/StatisticsValidators.cs` | 1058 | `8d65fbbd5241af8c` |
| 136 | `src/RefLab.Application/Validation/WorksheetValidators.cs` | 1576 | `b612a8d839ce0e7e` |
| 137 | `src/RefLab.Domain/Auditing/AttendanceEntry.cs` | 2473 | `d67be84c89c93b72` |
| 138 | `src/RefLab.Domain/Auditing/PrintAudit.cs` | 2509 | `52f55f1f27ad24f2` |
| 139 | `src/RefLab.Domain/Billing/Payment.cs` | 3640 | `47f2f16718a7be03` |
| 140 | `src/RefLab.Domain/Billing/PriceList.cs` | 2277 | `1498cb710c9c34b4` |
| 141 | `src/RefLab.Domain/Billing/ReferringParty.cs` | 6647 | `4418977fc68af622` |
| 142 | `src/RefLab.Domain/Billing/TreatingPhysician.cs` | 2355 | `44f605c9f89209e0` |
| 143 | `src/RefLab.Domain/ClinicalKnowledge/Abbreviation.cs` | 1798 | `4fb208dfe59a9eef` |
| 144 | `src/RefLab.Domain/ClinicalKnowledge/ClinicalConstant.cs` | 4018 | `357def56e9fa5f1b` |
| 145 | `src/RefLab.Domain/ClinicalKnowledge/ClinicalSubGroup.cs` | 2574 | `22ee1a7813e7772a` |
| 146 | `src/RefLab.Domain/ClinicalKnowledge/ConversionFactor.cs` | 3501 | `5130c89e8c356917` |
| 147 | `src/RefLab.Domain/ClinicalKnowledge/InvalidClinicalConstantException.cs` | 700 | `3e6e38eb85fe85ef` |
| 148 | `src/RefLab.Domain/ClinicalKnowledge/ProthrombinControlCalibration.cs` | 1945 | `91d62506012811d0` |
| 149 | `src/RefLab.Domain/ClinicalKnowledge/ReferenceRange.cs` | 6792 | `9d1e92ac215655f7` |
| 150 | `src/RefLab.Domain/ClinicalKnowledge/TestDefinition.cs` | 12006 | `1de77ece159090fe` |
| 151 | `src/RefLab.Domain/ClinicalKnowledge/TestUnit.cs` | 1605 | `27b4296f0c5278d4` |
| 152 | `src/RefLab.Domain/Configuration/LaboratorySettings.cs` | 8676 | `8e75869703fa8876` |
| 153 | `src/RefLab.Domain/Enums/AgeUnit.cs` | 523 | `f3663535b8a9ba31` |
| 154 | `src/RefLab.Domain/Enums/AttendanceEventType.cs` | 703 | `ebaefb43f37c8fc2` |
| 155 | `src/RefLab.Domain/Enums/RecordStatus.cs` | 1001 | `f81677873039949b` |
| 156 | `src/RefLab.Domain/Enums/ReportCategory.cs` | 884 | `3d65d63979e55bad` |
| 157 | `src/RefLab.Domain/Enums/ResultClassification.cs` | 1321 | `42782ae886076ac8` |
| 158 | `src/RefLab.Domain/Enums/ResultState.cs` | 939 | `2939116b6b9a66ca` |
| 159 | `src/RefLab.Domain/Enums/SampleDispositionAction.cs` | 825 | `ab5c6a5003fa6ceb` |
| 160 | `src/RefLab.Domain/Enums/SendOutState.cs` | 994 | `5f9983f2e9a103d4` |
| 161 | `src/RefLab.Domain/Enums/Sex.cs` | 415 | `05bcc85c0d0c0cf0` |
| 162 | `src/RefLab.Domain/Enums/SexScope.cs` | 559 | `a2a83461264aa12c` |
| 163 | `src/RefLab.Domain/Exceptions/BillingExceptions.cs` | 2043 | `1669c8c43d3f56a5` |
| 164 | `src/RefLab.Domain/Exceptions/CatalogueExceptions.cs` | 1541 | `120585ff95359f44` |
| 165 | `src/RefLab.Domain/Exceptions/DomainException.cs` | 823 | `573eeb0a82fa16a7` |
| 166 | `src/RefLab.Domain/Exceptions/PatientExceptions.cs` | 2537 | `cc4366b044355c3d` |
| 167 | `src/RefLab.Domain/Exceptions/ReferenceRangeExceptions.cs` | 2749 | `5ce0b36141e3dddb` |
| 168 | `src/RefLab.Domain/Exceptions/SecurityExceptions.cs` | 1515 | `83d36196d5a9e1f1` |
| 169 | `src/RefLab.Domain/Exceptions/SendOutExceptions.cs` | 690 | `5f02926fc2d0808c` |
| 170 | `src/RefLab.Domain/Farming/FarmRecord.cs` | 5086 | `d207acba1e45328d` |
| 171 | `src/RefLab.Domain/Identity/Branch.cs` | 1947 | `19196d3d173d8307` |
| 172 | `src/RefLab.Domain/Identity/Operator.cs` | 7783 | `8e9a4ef78729ce45` |
| 173 | `src/RefLab.Domain/Identity/PasswordPolicy.cs` | 2574 | `d7be0735b6375f4c` |
| 174 | `src/RefLab.Domain/Identity/Permission.cs` | 2535 | `ecc83a2229d1c7f9` |
| 175 | `src/RefLab.Domain/Microbiology/Antibiotic.cs` | 4816 | `57a647a0a94391cc` |
| 176 | `src/RefLab.Domain/Microbiology/CultureResult.cs` | 6319 | `5888c980ef9347f3` |
| 177 | `src/RefLab.Domain/Patients/OrderTest.cs` | 15502 | `509e056332b4b966` |
| 178 | `src/RefLab.Domain/Patients/Patient.cs` | 5454 | `a0372c1958ee3e31` |
| 179 | `src/RefLab.Domain/Patients/PatientHistory.cs` | 3443 | `38ec0aafddc61a51` |
| 180 | `src/RefLab.Domain/Patients/PatientOrder.cs` | 10009 | `4e4e408e64ea1b24` |
| 181 | `src/RefLab.Domain/Patients/PatientPicture.cs` | 3029 | `c0ca6613286691d6` |
| 182 | `src/RefLab.Domain/Patients/ReferenceSnapshot.cs` | 2768 | `9f00ff1a2647606b` |
| 183 | `src/RefLab.Domain/RefLab.Domain.csproj` | 607 | `fc92f445c1fc5cf4` |
| 184 | `src/RefLab.Domain/Rules/ClinicalConstantResolver.cs` | 2277 | `c88a23e8f19377b8` |
| 185 | `src/RefLab.Domain/Rules/ClinicalDerivationPolicy.cs` | 8516 | `5ce3a4a5db75704c` |
| 186 | `src/RefLab.Domain/Rules/CommissionCalculator.cs` | 2959 | `d71cb977acb6746b` |
| 187 | `src/RefLab.Domain/Rules/IdentifierAllocator.cs` | 3163 | `c0290379fc14632e` |
| 188 | `src/RefLab.Domain/Rules/InvalidClinicalDerivationException.cs` | 362 | `cd4f878d9f60f9d0` |
| 189 | `src/RefLab.Domain/Rules/ProthrombinControlPoint.cs` | 515 | `01881411c867bc80` |
| 190 | `src/RefLab.Domain/Rules/ReferenceRangeSelector.cs` | 2338 | `fccdd151ab6f1858` |
| 191 | `src/RefLab.Domain/Rules/ResultClassificationPolicy.cs` | 2661 | `ad43a314a97a0985` |
| 192 | `src/RefLab.Domain/SendOut/SampleDisposition.cs` | 2760 | `5612fb8125c3aa55` |
| 193 | `src/RefLab.Domain/SendOut/SendOutRequest.cs` | 5002 | `64f621c16aabcb06` |
| 194 | `src/RefLab.Domain/ValueObjects/AgeBand.cs` | 2938 | `13a2e80e2d2f25dd` |
| 195 | `src/RefLab.Domain/ValueObjects/CriticalInterval.cs` | 4185 | `b9279ee3bf12e844` |
| 196 | `src/RefLab.Domain/ValueObjects/Money.cs` | 6145 | `81edda591710dbc5` |
| 197 | `src/RefLab.Domain/ValueObjects/OrderIdentifier.cs` | 6242 | `b1923fcb83f491fe` |
| 198 | `src/RefLab.Domain/ValueObjects/PatientId.cs` | 2360 | `eecebb8240e9aee8` |
| 199 | `src/RefLab.Domain/ValueObjects/Percentage.cs` | 1719 | `05156115f366e1c0` |
| 200 | `src/RefLab.Domain/ValueObjects/ReferenceInterval.cs` | 6187 | `96ca1080f93f1b14` |
| 201 | `src/RefLab.Domain/ValueObjects/ResultValue.cs` | 3385 | `3eb16b9d799665ba` |
| 202 | `src/RefLab.Infrastructure/InfrastructureServiceCollectionExtensions.cs` | 2011 | `6e3e66cfb0bea5a7` |
| 203 | `src/RefLab.Infrastructure/RefLab.Infrastructure.csproj` | 1131 | `2a53e267d34179a3` |
| 204 | `src/RefLab.Infrastructure/Reports/PrintSequenceGenerator.cs` | 659 | `dbc8aa204311215e` |
| 205 | `src/RefLab.Infrastructure/Reports/QuestPdfExporter.cs` | 4224 | `98c0cbe3063c3d84` |
| 206 | `src/RefLab.Infrastructure/Reports/QuestPdfReportRenderer.cs` | 4560 | `a5a39ae70c42c0be` |
| 207 | `src/RefLab.Infrastructure/Security/Pbkdf2PasswordHasher.cs` | 2810 | `15a8dd22e33e52d3` |
| 208 | `src/RefLab.Infrastructure/Session/SessionCurrentUser.cs` | 1664 | `f24c77aab6f88ea5` |
| 209 | `src/RefLab.Infrastructure/Time/SystemClock.cs` | 257 | `79de42cbb393e255` |
| 210 | `src/RefLab.Presentation/App.xaml` | 282 | `617d7fe83141a537` |
| 211 | `src/RefLab.Presentation/App.xaml.cs` | 155 | `5561558eac4dbf3c` |
| 212 | `src/RefLab.Presentation/MainWindow.xaml` | 828 | `6e1cfdec2382f33e` |
| 213 | `src/RefLab.Presentation/MainWindow.xaml.cs` | 239 | `c4d330391fdce80b` |
| 214 | `src/RefLab.Presentation/RefLab.Presentation.csproj` | 1409 | `e9c8852dbcc4f3c4` |
| 215 | `tests/RefLab.Application.Tests/Architecture/ApplicationLayerBoundaryTests.cs` | 1951 | `c5265ae219e31d84` |
| 216 | `tests/RefLab.Application.Tests/Behaviors/BehaviorTests.cs` | 4843 | `920d446fceebffe0` |
| 217 | `tests/RefLab.Application.Tests/DependencyInjectionTests.cs` | 5258 | `0ff8da443a1c5fd3` |
| 218 | `tests/RefLab.Application.Tests/Fakes/ApplicationTestHost.cs` | 3265 | `a9676feb934e3297` |
| 219 | `tests/RefLab.Application.Tests/Fakes/FakeAppDbContext.cs` | 3549 | `4f377d8f1501ed47` |
| 220 | `tests/RefLab.Application.Tests/Fakes/Fakes.cs` | 4709 | `2d9f668cc0db8d89` |
| 221 | `tests/RefLab.Application.Tests/Fakes/TestData.cs` | 6086 | `93684043e1ec1d1e` |
| 222 | `tests/RefLab.Application.Tests/Features/Auditing/AuditingHandlerTests.cs` | 2630 | `11a6bf7133fcfa43` |
| 223 | `tests/RefLab.Application.Tests/Features/Billing/AccountingTests.cs` | 7375 | `1a652536ce32a349` |
| 224 | `tests/RefLab.Application.Tests/Features/Billing/BillingHandlerTests.cs` | 5971 | `14b1558671ebf95c` |
| 225 | `tests/RefLab.Application.Tests/Features/Branches/BranchHandlerTests.cs` | 1688 | `ea67317045c3f7f0` |
| 226 | `tests/RefLab.Application.Tests/Features/Catalogue/CatalogueHandlerTests.cs` | 3875 | `6cf2616163502780` |
| 227 | `tests/RefLab.Application.Tests/Features/Catalogue/CatalogueManagementTests.cs` | 7869 | `ba762567959a702c` |
| 228 | `tests/RefLab.Application.Tests/Features/Configuration/SettingsTests.cs` | 3214 | `f3220babe91ac97b` |
| 229 | `tests/RefLab.Application.Tests/Features/Derivation/DerivationTests.cs` | 3963 | `4add29fbd95c85fe` |
| 230 | `tests/RefLab.Application.Tests/Features/Identity/IdentityHandlerTests.cs` | 6753 | `e1c2bb487fc2df7e` |
| 231 | `tests/RefLab.Application.Tests/Features/Identity/OperatorManagementTests.cs` | 4761 | `f3cac21a1060093f` |
| 232 | `tests/RefLab.Application.Tests/Features/Microbiology/MicrobiologyTests.cs` | 4650 | `41b20b4351b0f2da` |
| 233 | `tests/RefLab.Application.Tests/Features/Parties/PartiesAndPricesTests.cs` | 3501 | `33b8f18a124058f6` |
| 234 | `tests/RefLab.Application.Tests/Features/Patients/PatientHandlerTests.cs` | 7040 | `e67352c9c7ea742a` |
| 235 | `tests/RefLab.Application.Tests/Features/Patients/PatientRecordsTests.cs` | 6471 | `12e1df4ee338ba92` |
| 236 | `tests/RefLab.Application.Tests/Features/Reporting/ReportingTests.cs` | 6275 | `29de4fa2901807e7` |
| 237 | `tests/RefLab.Application.Tests/Features/Results/ResultEntryTests.cs` | 3191 | `af82a3ce40aaa60d` |
| 238 | `tests/RefLab.Application.Tests/Features/Results/ResultHandlerTests.cs` | 10405 | `efa1879d9880a243` |
| 239 | `tests/RefLab.Application.Tests/Features/SendOut/SendOutHandlerTests.cs` | 1930 | `f0973adba2999ba8` |
| 240 | `tests/RefLab.Application.Tests/Features/SendOut/SendOutWorkflowTests.cs` | 3634 | `29e5c721365d979b` |
| 241 | `tests/RefLab.Application.Tests/Features/Statistics/StatisticsTests.cs` | 3670 | `1deede2611472aa5` |
| 242 | `tests/RefLab.Application.Tests/Features/Worksheets/WorksheetTests.cs` | 2939 | `dfb7fb29fa4886d1` |
| 243 | `tests/RefLab.Application.Tests/PermissionEnforcementTests.cs` | 5825 | `4629c07fa6a1ef30` |
| 244 | `tests/RefLab.Application.Tests/RefLab.Application.Tests.csproj` | 1487 | `37dc33ce19f17370` |
| 245 | `tests/RefLab.Domain.Tests/Architecture/DomainLayerBoundaryTests.cs` | 2222 | `bb6e8a7222b5a1b3` |
| 246 | `tests/RefLab.Domain.Tests/Auditing/PrintAuditTests.cs` | 1629 | `b648f6f66f4fb851` |
| 247 | `tests/RefLab.Domain.Tests/Billing/PaymentTests.cs` | 1500 | `3cfbdbbaa03c9146` |
| 248 | `tests/RefLab.Domain.Tests/Billing/ReferringPartyTests.cs` | 1736 | `33cce6a39e18cdd0` |
| 249 | `tests/RefLab.Domain.Tests/ClinicalKnowledge/CatalogueUpdateTests.cs` | 2991 | `c00a01994d6b9bee` |
| 250 | `tests/RefLab.Domain.Tests/ClinicalKnowledge/ClinicalConstantTests.cs` | 2039 | `f8272dcc8bc1331d` |
| 251 | `tests/RefLab.Domain.Tests/ClinicalKnowledge/ConversionFactorTests.cs` | 1139 | `df4bbf8129053d0a` |
| 252 | `tests/RefLab.Domain.Tests/ClinicalKnowledge/ProthrombinControlCalibrationTests.cs` | 694 | `691befa332a23c7d` |
| 253 | `tests/RefLab.Domain.Tests/ClinicalKnowledge/ReferenceRangeTests.cs` | 2569 | `49198379c6945508` |
| 254 | `tests/RefLab.Domain.Tests/ClinicalKnowledge/TestDefinitionTests.cs` | 4319 | `b1611276f05e0bba` |
| 255 | `tests/RefLab.Domain.Tests/Configuration/LaboratorySettingsTests.cs` | 2256 | `eabd5584c60824ca` |
| 256 | `tests/RefLab.Domain.Tests/Farming/FarmTests.cs` | 1278 | `ce27c394cadf2f23` |
| 257 | `tests/RefLab.Domain.Tests/Identity/BranchTests.cs` | 1085 | `f170a2f18aa5a3a1` |
| 258 | `tests/RefLab.Domain.Tests/Identity/OperatorTests.cs` | 4314 | `abecec3bdbdc447f` |
| 259 | `tests/RefLab.Domain.Tests/Identity/PasswordPolicyTests.cs` | 1760 | `1f7b293f81435ea6` |
| 260 | `tests/RefLab.Domain.Tests/Identity/PermissionTests.cs` | 759 | `de8e7f58270a884f` |
| 261 | `tests/RefLab.Domain.Tests/Microbiology/MicrobiologyTests.cs` | 3199 | `b842ac4fa3fa4d3f` |
| 262 | `tests/RefLab.Domain.Tests/Patients/OrderTestTests.cs` | 7746 | `517529d17e3dbf9e` |
| 263 | `tests/RefLab.Domain.Tests/Patients/PatientHistoryTests.cs` | 1825 | `4ed9e2bd8b41d33e` |
| 264 | `tests/RefLab.Domain.Tests/Patients/PatientOrderTests.cs` | 4259 | `0b5ec78d8897204b` |
| 265 | `tests/RefLab.Domain.Tests/Patients/PatientPictureTests.cs` | 978 | `607bbd5a8c5bd73d` |
| 266 | `tests/RefLab.Domain.Tests/Patients/PatientTests.cs` | 2094 | `062e607144cd80f0` |
| 267 | `tests/RefLab.Domain.Tests/RefLab.Domain.Tests.csproj` | 1215 | `df3cbecf303de8f0` |
| 268 | `tests/RefLab.Domain.Tests/Rules/ClinicalConstantResolverTests.cs` | 1681 | `7042f212265a0e85` |
| 269 | `tests/RefLab.Domain.Tests/Rules/ClinicalDerivationPolicyTests.cs` | 4965 | `1d17cf6012e3bd58` |
| 270 | `tests/RefLab.Domain.Tests/Rules/CommissionCalculatorTests.cs` | 2373 | `1b12cebde8540809` |
| 271 | `tests/RefLab.Domain.Tests/Rules/IdentifierAllocatorTests.cs` | 2390 | `409dcbd2e85fd132` |
| 272 | `tests/RefLab.Domain.Tests/Rules/ReferenceRangeSelectorTests.cs` | 2801 | `7663809319b29fc3` |
| 273 | `tests/RefLab.Domain.Tests/Rules/ResultClassificationPolicyTests.cs` | 5414 | `911ed95748874517` |
| 274 | `tests/RefLab.Domain.Tests/SendOut/SampleDispositionTests.cs` | 1350 | `4e599e74a538d52e` |
| 275 | `tests/RefLab.Domain.Tests/SendOut/SendOutRequestTests.cs` | 2460 | `685459470dabdc8e` |
| 276 | `tests/RefLab.Domain.Tests/ValueObjects/AgeBandTests.cs` | 1804 | `d734fa035541b624` |
| 277 | `tests/RefLab.Domain.Tests/ValueObjects/CriticalIntervalTests.cs` | 1453 | `10eecdf3def9483f` |
| 278 | `tests/RefLab.Domain.Tests/ValueObjects/MoneyTests.cs` | 3005 | `de7b51ababc3cfac` |
| 279 | `tests/RefLab.Domain.Tests/ValueObjects/OrderIdentifierTests.cs` | 4469 | `70d239129442c1f3` |
| 280 | `tests/RefLab.Domain.Tests/ValueObjects/PercentageTests.cs` | 1031 | `6690ffbd58dad59f` |
| 281 | `tests/RefLab.Domain.Tests/ValueObjects/ReferenceIntervalTests.cs` | 2915 | `4f4d8d5776e9e36a` |
| 282 | `tests/RefLab.Domain.Tests/ValueObjects/ResultValueTests.cs` | 1927 | `983578486bfa0dff` |
| 283 | `tests/RefLab.Infrastructure.Tests/InfrastructureServiceTests.cs` | 2293 | `f6f13d6906554f80` |
| 284 | `tests/RefLab.Infrastructure.Tests/Pbkdf2PasswordHasherTests.cs` | 1812 | `b7eabc66796df91e` |
| 285 | `tests/RefLab.Infrastructure.Tests/QuestPdfExporterTests.cs` | 3505 | `fd0bf730ac1ddcac` |
| 286 | `tests/RefLab.Infrastructure.Tests/RefLab.Infrastructure.Tests.csproj` | 1509 | `1f5c614419521839` |

## 3. How to apply this delivery

1. Extract the archive **into** `C:\Users\LAP LINK\source\repos\Ref Lab`.
2. Choose *overwrite existing files*.
3. `README.md` and the files listed in `docs/LOCAL_AGENT_IMPLEMENTATION_GUIDE.md`
   section 5 already exist and are replaced; everything else is new.
4. Run the build and test commands in `docs/LOCAL_AGENT_IMPLEMENTATION_GUIDE.md` section 4.

---

*End of `RL-P3-MAN-001`.*
