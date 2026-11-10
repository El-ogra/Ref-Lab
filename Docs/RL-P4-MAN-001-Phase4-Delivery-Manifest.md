# Ref Lab — Phase 4 Delivery Manifest

**Document ID:** `RL-P4-MAN-001`  
**Scope:** the complete Phase 4 delivery — the branch removal (Part 1) and the Entity Framework Core persistence layer with its generated SQL Server migration (Part 2).

---

## 1. Verification status

| Gate | Result |
|---|---|
| Use cases implemented | **70 / 70** |
| Use cases with a FluentValidation validator | **70 / 70** |
| Use cases covered by a test | **70 / 70** |
| Commands / queries | 45 / 25 |
| Feature areas | 14 |
| Named permissions | 15 (the branch permission was removed) |
| Mapped tables | 27 |
| Mapped columns | 338 |
| Foreign keys | 11 |
| Indexes | 26 |
| Build `Debug` | 0 errors, 0 warnings |
| Build `Release` | 0 errors, 0 warnings |
| Tests | **535 passed, 0 failed, 0 skipped** (270 Domain, 204 Application, 61 Infrastructure) |
| Branch table or branch column in the schema | none |
| SMS / NATIGH residual references | none |
| EF Core leakage into the Application layer | none |
| `RefLab.Domain` package / project references | 0 / 0 |

## 2. File manifest

**Total files: 298**

| # | Path | Bytes | SHA-256 (first 16) |
|---:|---|---:|---|
| 1 | `.gitignore` | 591 | `af5a9644052ba7b1` |
| 2 | `Directory.Build.props` | 1249 | `2c0c54ee74261bc2` |
| 3 | `From-Phase-0-to-3-Handoff.md` | 150238 | `6dfd37fa9e8ff43c` |
| 4 | `README.md` | 11619 | `6f96071020c102f0` |
| 5 | `RefLab.sln` | 9576 | `6f5df42a3376b5dd` |
| 6 | `docs/LOCAL_AGENT_IMPLEMENTATION_GUIDE.md` | 17963 | `b8accb1d81fcf257` |
| 7 | `docs/RL-P0-RPT-001-Discovery-Report.md` | 59387 | `e67da9235bfbdb69` |
| 8 | `docs/RL-P2-MAN-001-Domain-Layer-Manifest.md` | 14506 | `61707422705af71c` |
| 9 | `docs/RL-P3-Completion-Report.md` | 24504 | `a68e9f29f6c14bd2` |
| 10 | `docs/RL-P4-Completion-Report.md` | 14082 | `369deebaf827d0e2` |
| 11 | `docs/USE_CASE_INVENTORY.md` | 29995 | `87e1b618ad7714ac` |
| 12 | `global.json` | 108 | `546b7a626451526a` |
| 13 | `scripts/generate_use_case_inventory.py` | 13963 | `87dbc182a333b0d3` |
| 14 | `src/RefLab.Application/Abstractions/IAppDbContext.cs` | 616 | `996d943b59cdef83` |
| 15 | `src/RefLab.Application/Abstractions/IClock.cs` | 391 | `f51ea9eb72b27ffc` |
| 16 | `src/RefLab.Application/Abstractions/ICurrentUser.cs` | 656 | `848644cb13c219f1` |
| 17 | `src/RefLab.Application/Abstractions/IPasswordHasher.cs` | 640 | `cfa8ecb8a628423e` |
| 18 | `src/RefLab.Application/Abstractions/IPdfExporter.cs` | 768 | `64e9acf630213213` |
| 19 | `src/RefLab.Application/Abstractions/IPrintSequenceGenerator.cs` | 488 | `00e55622769e4f44` |
| 20 | `src/RefLab.Application/Abstractions/IReportRenderer.cs` | 356 | `87f18e0706c60b56` |
| 21 | `src/RefLab.Application/Abstractions/IUnitOfWork.cs` | 631 | `0fb3f4f7fd0bfd04` |
| 22 | `src/RefLab.Application/ApplicationServiceCollectionExtensions.cs` | 1316 | `97060d447bdc44d0` |
| 23 | `src/RefLab.Application/Behaviors/AuthorizationBehavior.cs` | 1608 | `3feaa215fb4ecd5b` |
| 24 | `src/RefLab.Application/Behaviors/DomainExceptionBehavior.cs` | 1001 | `d7b09492d7fc30c5` |
| 25 | `src/RefLab.Application/Behaviors/LoggingBehavior.cs` | 1711 | `80849225b6883dc5` |
| 26 | `src/RefLab.Application/Behaviors/ValidationBehavior.cs` | 1760 | `09330093a6973ed4` |
| 27 | `src/RefLab.Application/Common/ApplicationException.cs` | 831 | `5d84393a7bb23b0e` |
| 28 | `src/RefLab.Application/Common/ConcurrencyException.cs` | 705 | `6307a9b0d7e09a3e` |
| 29 | `src/RefLab.Application/Common/ConflictException.cs` | 340 | `10c3c5b76667f8dc` |
| 30 | `src/RefLab.Application/Common/ForbiddenException.cs` | 348 | `1812d749457edf7b` |
| 31 | `src/RefLab.Application/Common/NoResult.cs` | 152 | `bcf4c41086669ea4` |
| 32 | `src/RefLab.Application/Common/NotFoundException.cs` | 613 | `ef740ea96c2e784b` |
| 33 | `src/RefLab.Application/Common/PageRequest.cs` | 633 | `c1486f8cedd1c54d` |
| 34 | `src/RefLab.Application/Common/PagedResult.cs` | 1493 | `c25197653f407baf` |
| 35 | `src/RefLab.Application/Common/ReportModels.cs` | 1326 | `cf91e1f4705c955b` |
| 36 | `src/RefLab.Application/Common/RequestValidationException.cs` | 715 | `1af6a990a6b903be` |
| 37 | `src/RefLab.Application/Features/Auditing/Dtos.cs` | 1130 | `0967a68abd1826cf` |
| 38 | `src/RefLab.Application/Features/Auditing/GetPrintAuditTrail.cs` | 1676 | `0be6a11d87e162b1` |
| 39 | `src/RefLab.Application/Features/Auditing/RecordAttendance.cs` | 2746 | `aa44026bd8a1d888` |
| 40 | `src/RefLab.Application/Features/Billing/ComputeCommission.cs` | 2427 | `04985e5efc109e92` |
| 41 | `src/RefLab.Application/Features/Billing/Dtos.cs` | 3865 | `46f944f4cadc2af5` |
| 42 | `src/RefLab.Application/Features/Billing/GetPayments.cs` | 1861 | `582fbd52e74ba8c7` |
| 43 | `src/RefLab.Application/Features/Billing/GetReferringParty.cs` | 1486 | `03e311facfddad53` |
| 44 | `src/RefLab.Application/Features/Billing/RecordPayment.cs` | 2374 | `3961e3ad147a4a6b` |
| 45 | `src/RefLab.Application/Features/Billing/RunPeriodStockTake.cs` | 2771 | `7c428dcd8a4c8f89` |
| 46 | `src/RefLab.Application/Features/Billing/RunStockTakeByPhysicianOrParty.cs` | 3264 | `fac6de902c216778` |
| 47 | `src/RefLab.Application/Features/Billing/SetDefaultAccountType.cs` | 2018 | `565cbe7a3883d300` |
| 48 | `src/RefLab.Application/Features/Catalogue/CreateReferenceRange.cs` | 2621 | `bbceadfcdc616c31` |
| 49 | `src/RefLab.Application/Features/Catalogue/CreateTestDefinition.cs` | 2380 | `b4ed53f597f1ed4f` |
| 50 | `src/RefLab.Application/Features/Catalogue/Dtos.cs` | 4767 | `7e22d31cd404d405` |
| 51 | `src/RefLab.Application/Features/Catalogue/GetReferenceRangeForPatient.cs` | 1880 | `4cc5165827edf6c4` |
| 52 | `src/RefLab.Application/Features/Catalogue/GetTestCatalogue.cs` | 2266 | `67276ef3e19f78b3` |
| 53 | `src/RefLab.Application/Features/Catalogue/GetTestProfile.cs` | 1985 | `efd7720e113e90e2` |
| 54 | `src/RefLab.Application/Features/Catalogue/ManageAbbreviations.cs` | 1948 | `70fe28e99ba0bc66` |
| 55 | `src/RefLab.Application/Features/Catalogue/ManageClinicalSubGroups.cs` | 2293 | `9a46d9fceadb27d0` |
| 56 | `src/RefLab.Application/Features/Catalogue/ManageUnitsAndConversions.cs` | 4198 | `e59d1b965d5b660c` |
| 57 | `src/RefLab.Application/Features/Catalogue/UpdateReferenceRange.cs` | 2035 | `1e5f7a15a1b0a696` |
| 58 | `src/RefLab.Application/Features/Catalogue/UpdateTestDefinition.cs` | 3024 | `90d5365f9f5ac93d` |
| 59 | `src/RefLab.Application/Features/Configuration/ConfigureConnectionReceipt.cs` | 2026 | `248884dc308e2b27` |
| 60 | `src/RefLab.Application/Features/Configuration/ConfigureLaboratorySettings.cs` | 3044 | `59e64052be1b3442` |
| 61 | `src/RefLab.Application/Features/Configuration/CustomiseReportLayout.cs` | 2828 | `d6c12beccd27bddc` |
| 62 | `src/RefLab.Application/Features/Configuration/Dtos.cs` | 1585 | `b10900cf3017777d` |
| 63 | `src/RefLab.Application/Features/Derivation/DeriveHematocrit.cs` | 1690 | `23642fc14fffa361` |
| 64 | `src/RefLab.Application/Features/Derivation/DeriveHemoglobinPercentage.cs` | 1889 | `8c4df329e6b894f4` |
| 65 | `src/RefLab.Application/Features/Derivation/DeriveInternationalSensitivityIndex.cs` | 1783 | `920b9e8200920ea4` |
| 66 | `src/RefLab.Application/Features/Derivation/InterpolateProthrombinControl.cs` | 1885 | `5989b25885dfd9ae` |
| 67 | `src/RefLab.Application/Features/Identity/AssignOperatorPermissions.cs` | 2482 | `f2f4aca5e96c0d31` |
| 68 | `src/RefLab.Application/Features/Identity/ChangePassword.cs` | 2194 | `0b4d104bc59cb882` |
| 69 | `src/RefLab.Application/Features/Identity/CreateOperator.cs` | 2795 | `869f10f16c7af96b` |
| 70 | `src/RefLab.Application/Features/Identity/Dtos.cs` | 919 | `7dbcef85c08a9b27` |
| 71 | `src/RefLab.Application/Features/Identity/GetOperators.cs` | 1602 | `9a45d3c5426077de` |
| 72 | `src/RefLab.Application/Features/Identity/SignIn.cs` | 2432 | `ddeaa3ec59dc3104` |
| 73 | `src/RefLab.Application/Features/Identity/UpdateOperator.cs` | 1866 | `9ba1f57d41c4b4be` |
| 74 | `src/RefLab.Application/Features/Microbiology/Dtos.cs` | 2249 | `93dec2b3398d51b4` |
| 75 | `src/RefLab.Application/Features/Microbiology/FarmDtos.cs` | 1185 | `b7bfb9176d410acd` |
| 76 | `src/RefLab.Application/Features/Microbiology/GetCultureResults.cs` | 1807 | `e9f290317185844c` |
| 77 | `src/RefLab.Application/Features/Microbiology/ManageAntibioticCatalogue.cs` | 2640 | `18aeecdd9d587d3c` |
| 78 | `src/RefLab.Application/Features/Microbiology/ManageFarmResults.cs` | 2878 | `3667e4377fcd15aa` |
| 79 | `src/RefLab.Application/Features/Microbiology/RecordCultureResult.cs` | 2774 | `63ce64b4fa0058e2` |
| 80 | `src/RefLab.Application/Features/Parties/ManagePriceLists.cs` | 2006 | `9d69b951fbcf03de` |
| 81 | `src/RefLab.Application/Features/Parties/ManageTreatingPhysicians.cs` | 2117 | `a736bdef009db021` |
| 82 | `src/RefLab.Application/Features/Parties/PrintPriceList.cs` | 2729 | `9705f58dcf64765a` |
| 83 | `src/RefLab.Application/Features/Patients/AllocateOrderIdentifier.cs` | 1924 | `c5729994b4a3ec21` |
| 84 | `src/RefLab.Application/Features/Patients/AttachPatientPicture.cs` | 2365 | `3379037f4fea81fa` |
| 85 | `src/RefLab.Application/Features/Patients/Dtos.cs` | 6905 | `ad7bc49b5c97475a` |
| 86 | `src/RefLab.Application/Features/Patients/FollowUpCase.cs` | 3618 | `331ffb1e0c520df6` |
| 87 | `src/RefLab.Application/Features/Patients/GeneratePatientBarcode.cs` | 2162 | `a3e1c839b24ded80` |
| 88 | `src/RefLab.Application/Features/Patients/GetPatientHistory.cs` | 1252 | `73808f50acdea9de` |
| 89 | `src/RefLab.Application/Features/Patients/GetPatientOrder.cs` | 1450 | `e2e138b3c4dec00f` |
| 90 | `src/RefLab.Application/Features/Patients/RegisterPatient.cs` | 2753 | `f98ac0314857beae` |
| 91 | `src/RefLab.Application/Features/Patients/SearchPatients.cs` | 1919 | `de9cda037cad0fd7` |
| 92 | `src/RefLab.Application/Features/Reporting/ConfigurePrinters.cs` | 2100 | `5b83d6e274527b8e` |
| 93 | `src/RefLab.Application/Features/Reporting/ConfigureReportHeaderFooter.cs` | 2611 | `c186947673f0fc94` |
| 94 | `src/RefLab.Application/Features/Reporting/PrintPatientIdCard.cs` | 3140 | `1ae9e38c27da9681` |
| 95 | `src/RefLab.Application/Features/Reporting/PrintPatientInvoice.cs` | 3605 | `ed3cf035a9cc16c3` |
| 96 | `src/RefLab.Application/Features/Reporting/ProduceConsolidatedReport.cs` | 4519 | `248581f99011dfea` |
| 97 | `src/RefLab.Application/Features/Reporting/ProduceEmptyReport.cs` | 2406 | `33bab35668a4b742` |
| 98 | `src/RefLab.Application/Features/Results/Dtos.cs` | 2226 | `a9b2f8b68a370296` |
| 99 | `src/RefLab.Application/Features/Results/EnterAdditionalResult.cs` | 2135 | `a9a1ee48b5a3d8b9` |
| 100 | `src/RefLab.Application/Features/Results/EnterResult.cs` | 2174 | `9347fb37a9eaa34f` |
| 101 | `src/RefLab.Application/Features/Results/GetCriticalValues.cs` | 1692 | `25a638e612c62662` |
| 102 | `src/RefLab.Application/Features/Results/GetMethodSheet.cs` | 2015 | `677870be09fedb11` |
| 103 | `src/RefLab.Application/Features/Results/HandOverResults.cs` | 3223 | `e9b47acf2ce5233e` |
| 104 | `src/RefLab.Application/Features/Results/OrderTests.cs` | 4641 | `cb2bdc7423935094` |
| 105 | `src/RefLab.Application/Features/Results/PrintResultReport.cs` | 4566 | `570b731f11947407` |
| 106 | `src/RefLab.Application/Features/Results/ReviewResult.cs` | 1814 | `f8f6dbc7099d3ecf` |
| 107 | `src/RefLab.Application/Features/SendOut/CreateSendOutRequest.cs` | 2347 | `1c92e22380a4860e` |
| 108 | `src/RefLab.Application/Features/SendOut/DispatchSendOutSample.cs` | 1682 | `032004738f3cdb37` |
| 109 | `src/RefLab.Application/Features/SendOut/Dtos.cs` | 1663 | `08f36ce4a0a9f87e` |
| 110 | `src/RefLab.Application/Features/SendOut/GetSendOutQueue.cs` | 1705 | `693fcb29cc52ebcb` |
| 111 | `src/RefLab.Application/Features/SendOut/SettleSendOutSample.cs` | 1668 | `2e76d77e2e244f93` |
| 112 | `src/RefLab.Application/Features/SendOut/SplitOrWithdrawSample.cs` | 2284 | `f7d49ef5aef34e23` |
| 113 | `src/RefLab.Application/Features/Statistics/Dtos.cs` | 1511 | `fd62fdf9c0405be3` |
| 114 | `src/RefLab.Application/Features/Statistics/GetLaboratoryStatistics.cs` | 2978 | `4bf858d8bd215350` |
| 115 | `src/RefLab.Application/Features/Statistics/GetPerformanceStatistics.cs` | 2511 | `b6a6333b308a439b` |
| 116 | `src/RefLab.Application/Features/Worksheets/BuildPatientGroupWorkSheet.cs` | 4296 | `c9429a8ac129c41c` |
| 117 | `src/RefLab.Application/Features/Worksheets/BuildTestGroupWorkSheet.cs` | 4536 | `45159cd5742caf47` |
| 118 | `src/RefLab.Application/Features/Worksheets/Dtos.cs` | 801 | `a351c73aa11845ea` |
| 119 | `src/RefLab.Application/RefLab.Application.csproj` | 1218 | `e481fcca02cfc4f1` |
| 120 | `src/RefLab.Application/Validation/AuditingValidators.cs` | 1232 | `00e3e5e19a6434f2` |
| 121 | `src/RefLab.Application/Validation/BillingValidators.cs` | 4113 | `fb661a6676211a58` |
| 122 | `src/RefLab.Application/Validation/CatalogueValidators.cs` | 8749 | `b5ded3da18a70f27` |
| 123 | `src/RefLab.Application/Validation/ConfigurationValidators.cs` | 2721 | `8f4be8d964697145` |
| 124 | `src/RefLab.Application/Validation/DerivationValidators.cs` | 2637 | `820c6fb886f19d46` |
| 125 | `src/RefLab.Application/Validation/IdentityValidators.cs` | 4197 | `dbcc59c90d8640b6` |
| 126 | `src/RefLab.Application/Validation/MicrobiologyValidators.cs` | 3136 | `5367cde8f84c095b` |
| 127 | `src/RefLab.Application/Validation/PartiesValidators.cs` | 2203 | `2ebb0f2bca99fbcb` |
| 128 | `src/RefLab.Application/Validation/PatientValidators.cs` | 5567 | `9de4f79904fa308b` |
| 129 | `src/RefLab.Application/Validation/ReportingValidators.cs` | 3674 | `381d9758e0140d18` |
| 130 | `src/RefLab.Application/Validation/ResultValidators.cs` | 4317 | `fd4019b2b2a26afb` |
| 131 | `src/RefLab.Application/Validation/SendOutValidators.cs` | 2821 | `ed9c49d15506bc63` |
| 132 | `src/RefLab.Application/Validation/StatisticsValidators.cs` | 1058 | `8d65fbbd5241af8c` |
| 133 | `src/RefLab.Application/Validation/WorksheetValidators.cs` | 1576 | `b612a8d839ce0e7e` |
| 134 | `src/RefLab.Domain/Auditing/AttendanceEntry.cs` | 2473 | `d67be84c89c93b72` |
| 135 | `src/RefLab.Domain/Auditing/PrintAudit.cs` | 2509 | `52f55f1f27ad24f2` |
| 136 | `src/RefLab.Domain/Billing/Payment.cs` | 3640 | `47f2f16718a7be03` |
| 137 | `src/RefLab.Domain/Billing/PriceList.cs` | 2277 | `1498cb710c9c34b4` |
| 138 | `src/RefLab.Domain/Billing/ReferringParty.cs` | 6647 | `4418977fc68af622` |
| 139 | `src/RefLab.Domain/Billing/TreatingPhysician.cs` | 2355 | `44f605c9f89209e0` |
| 140 | `src/RefLab.Domain/ClinicalKnowledge/Abbreviation.cs` | 1798 | `4fb208dfe59a9eef` |
| 141 | `src/RefLab.Domain/ClinicalKnowledge/ClinicalConstant.cs` | 4018 | `357def56e9fa5f1b` |
| 142 | `src/RefLab.Domain/ClinicalKnowledge/ClinicalSubGroup.cs` | 2574 | `22ee1a7813e7772a` |
| 143 | `src/RefLab.Domain/ClinicalKnowledge/ConversionFactor.cs` | 3501 | `5130c89e8c356917` |
| 144 | `src/RefLab.Domain/ClinicalKnowledge/InvalidClinicalConstantException.cs` | 700 | `3e6e38eb85fe85ef` |
| 145 | `src/RefLab.Domain/ClinicalKnowledge/ProthrombinControlCalibration.cs` | 1945 | `91d62506012811d0` |
| 146 | `src/RefLab.Domain/ClinicalKnowledge/ReferenceRange.cs` | 6792 | `9d1e92ac215655f7` |
| 147 | `src/RefLab.Domain/ClinicalKnowledge/TestDefinition.cs` | 11875 | `ac207c6e43b63aa7` |
| 148 | `src/RefLab.Domain/ClinicalKnowledge/TestUnit.cs` | 1605 | `27b4296f0c5278d4` |
| 149 | `src/RefLab.Domain/Configuration/LaboratorySettings.cs` | 9100 | `705ffde126ba8abd` |
| 150 | `src/RefLab.Domain/Enums/AgeUnit.cs` | 523 | `f3663535b8a9ba31` |
| 151 | `src/RefLab.Domain/Enums/AttendanceEventType.cs` | 703 | `ebaefb43f37c8fc2` |
| 152 | `src/RefLab.Domain/Enums/RecordStatus.cs` | 1001 | `f81677873039949b` |
| 153 | `src/RefLab.Domain/Enums/ReportCategory.cs` | 884 | `3d65d63979e55bad` |
| 154 | `src/RefLab.Domain/Enums/ResultClassification.cs` | 1321 | `42782ae886076ac8` |
| 155 | `src/RefLab.Domain/Enums/ResultState.cs` | 939 | `2939116b6b9a66ca` |
| 156 | `src/RefLab.Domain/Enums/SampleDispositionAction.cs` | 825 | `ab5c6a5003fa6ceb` |
| 157 | `src/RefLab.Domain/Enums/SendOutState.cs` | 994 | `5f9983f2e9a103d4` |
| 158 | `src/RefLab.Domain/Enums/Sex.cs` | 415 | `05bcc85c0d0c0cf0` |
| 159 | `src/RefLab.Domain/Enums/SexScope.cs` | 559 | `a2a83461264aa12c` |
| 160 | `src/RefLab.Domain/Exceptions/BillingExceptions.cs` | 2043 | `1669c8c43d3f56a5` |
| 161 | `src/RefLab.Domain/Exceptions/CatalogueExceptions.cs` | 1541 | `120585ff95359f44` |
| 162 | `src/RefLab.Domain/Exceptions/DomainException.cs` | 823 | `573eeb0a82fa16a7` |
| 163 | `src/RefLab.Domain/Exceptions/PatientExceptions.cs` | 2537 | `cc4366b044355c3d` |
| 164 | `src/RefLab.Domain/Exceptions/ReferenceRangeExceptions.cs` | 2749 | `5ce0b36141e3dddb` |
| 165 | `src/RefLab.Domain/Exceptions/SecurityExceptions.cs` | 1515 | `83d36196d5a9e1f1` |
| 166 | `src/RefLab.Domain/Exceptions/SendOutExceptions.cs` | 690 | `5f02926fc2d0808c` |
| 167 | `src/RefLab.Domain/Farming/FarmRecord.cs` | 5086 | `d207acba1e45328d` |
| 168 | `src/RefLab.Domain/Identity/Operator.cs` | 8018 | `b3ea5074939d1f3b` |
| 169 | `src/RefLab.Domain/Identity/PasswordPolicy.cs` | 2574 | `d7be0735b6375f4c` |
| 170 | `src/RefLab.Domain/Identity/Permission.cs` | 2473 | `283fe9971eab3f6f` |
| 171 | `src/RefLab.Domain/Microbiology/Antibiotic.cs` | 4816 | `57a647a0a94391cc` |
| 172 | `src/RefLab.Domain/Microbiology/CultureResult.cs` | 6936 | `138c3b9cb05e6a9c` |
| 173 | `src/RefLab.Domain/Patients/OrderTest.cs` | 15502 | `509e056332b4b966` |
| 174 | `src/RefLab.Domain/Patients/Patient.cs` | 5454 | `a0372c1958ee3e31` |
| 175 | `src/RefLab.Domain/Patients/PatientHistory.cs` | 3941 | `a61ea6d9ac0d8c89` |
| 176 | `src/RefLab.Domain/Patients/PatientOrder.cs` | 9644 | `5e0688aaff795098` |
| 177 | `src/RefLab.Domain/Patients/PatientPicture.cs` | 3029 | `c0ca6613286691d6` |
| 178 | `src/RefLab.Domain/Patients/ReferenceSnapshot.cs` | 2768 | `9f00ff1a2647606b` |
| 179 | `src/RefLab.Domain/RefLab.Domain.csproj` | 607 | `fc92f445c1fc5cf4` |
| 180 | `src/RefLab.Domain/Rules/ClinicalConstantResolver.cs` | 2277 | `c88a23e8f19377b8` |
| 181 | `src/RefLab.Domain/Rules/ClinicalDerivationPolicy.cs` | 8516 | `5ce3a4a5db75704c` |
| 182 | `src/RefLab.Domain/Rules/CommissionCalculator.cs` | 2959 | `d71cb977acb6746b` |
| 183 | `src/RefLab.Domain/Rules/IdentifierAllocator.cs` | 1769 | `37ee496347131d6d` |
| 184 | `src/RefLab.Domain/Rules/InvalidClinicalDerivationException.cs` | 362 | `cd4f878d9f60f9d0` |
| 185 | `src/RefLab.Domain/Rules/ProthrombinControlPoint.cs` | 515 | `01881411c867bc80` |
| 186 | `src/RefLab.Domain/Rules/ReferenceRangeSelector.cs` | 2338 | `fccdd151ab6f1858` |
| 187 | `src/RefLab.Domain/Rules/ResultClassificationPolicy.cs` | 2661 | `ad43a314a97a0985` |
| 188 | `src/RefLab.Domain/SendOut/SampleDisposition.cs` | 2760 | `5612fb8125c3aa55` |
| 189 | `src/RefLab.Domain/SendOut/SendOutRequest.cs` | 5002 | `64f621c16aabcb06` |
| 190 | `src/RefLab.Domain/ValueObjects/AgeBand.cs` | 2938 | `13a2e80e2d2f25dd` |
| 191 | `src/RefLab.Domain/ValueObjects/CriticalInterval.cs` | 4573 | `68ef2fefecbcd751` |
| 192 | `src/RefLab.Domain/ValueObjects/Money.cs` | 6145 | `81edda591710dbc5` |
| 193 | `src/RefLab.Domain/ValueObjects/OrderIdentifier.cs` | 6179 | `22de3f13f9e15139` |
| 194 | `src/RefLab.Domain/ValueObjects/PatientId.cs` | 2360 | `eecebb8240e9aee8` |
| 195 | `src/RefLab.Domain/ValueObjects/Percentage.cs` | 1719 | `05156115f366e1c0` |
| 196 | `src/RefLab.Domain/ValueObjects/ReferenceInterval.cs` | 6666 | `667194d69dfd8852` |
| 197 | `src/RefLab.Domain/ValueObjects/ResultValue.cs` | 3385 | `3eb16b9d799665ba` |
| 198 | `src/RefLab.Infrastructure/InfrastructureServiceCollectionExtensions.cs` | 4599 | `888e66de996fe1ce` |
| 199 | `src/RefLab.Infrastructure/Persistence/Configurations/BillingConfigurations.cs` | 5517 | `172237780f0b5f17` |
| 200 | `src/RefLab.Infrastructure/Persistence/Configurations/ClinicalKnowledgeConfigurations.cs` | 9802 | `f4b9ce442e5d7f38` |
| 201 | `src/RefLab.Infrastructure/Persistence/Configurations/IdentityConfigurations.cs` | 4024 | `e242699dfdcd7384` |
| 202 | `src/RefLab.Infrastructure/Persistence/Configurations/OperationalConfigurations.cs` | 9879 | `01f8a6481a2b6ae6` |
| 203 | `src/RefLab.Infrastructure/Persistence/Configurations/PatientConfigurations.cs` | 11681 | `789b1c5628b0738d` |
| 204 | `src/RefLab.Infrastructure/Persistence/Configurations/ValueObjectConfigurations.cs` | 10240 | `2360119c0bebd5fb` |
| 205 | `src/RefLab.Infrastructure/Persistence/Converters/PersistenceConverters.cs` | 16098 | `714884ba11cb9861` |
| 206 | `src/RefLab.Infrastructure/Persistence/DatabaseInitializer.cs` | 1694 | `2cbf49a4bd6fb9b9` |
| 207 | `src/RefLab.Infrastructure/Persistence/Migrations/20261110103458_InitialCreate.Designer.cs` | 77374 | `d7f47d44742b1a2a` |
| 208 | `src/RefLab.Infrastructure/Persistence/Migrations/20261110103458_InitialCreate.cs` | 54894 | `c45a87311f2de651` |
| 209 | `src/RefLab.Infrastructure/Persistence/Migrations/RefLabDbContextModelSnapshot.cs` | 77276 | `fe0e41a23c9f6a83` |
| 210 | `src/RefLab.Infrastructure/Persistence/RefLabDbContext.cs` | 6957 | `e95b51aa94fa813e` |
| 211 | `src/RefLab.Infrastructure/Persistence/RefLabDbContextFactory.cs` | 1806 | `7473c150b6b1f220` |
| 212 | `src/RefLab.Infrastructure/RefLab.Infrastructure.csproj` | 1385 | `648fab3e88464d26` |
| 213 | `src/RefLab.Infrastructure/Reports/PrintSequenceGenerator.cs` | 659 | `dbc8aa204311215e` |
| 214 | `src/RefLab.Infrastructure/Reports/QuestPdfExporter.cs` | 4224 | `98c0cbe3063c3d84` |
| 215 | `src/RefLab.Infrastructure/Reports/QuestPdfReportRenderer.cs` | 4560 | `a5a39ae70c42c0be` |
| 216 | `src/RefLab.Infrastructure/Security/Pbkdf2PasswordHasher.cs` | 2810 | `15a8dd22e33e52d3` |
| 217 | `src/RefLab.Infrastructure/Session/SessionCurrentUser.cs` | 1506 | `a152458c9a016ad5` |
| 218 | `src/RefLab.Infrastructure/Time/SystemClock.cs` | 257 | `79de42cbb393e255` |
| 219 | `src/RefLab.Presentation/App.xaml` | 282 | `617d7fe83141a537` |
| 220 | `src/RefLab.Presentation/App.xaml.cs` | 155 | `5561558eac4dbf3c` |
| 221 | `src/RefLab.Presentation/MainWindow.xaml` | 828 | `6e1cfdec2382f33e` |
| 222 | `src/RefLab.Presentation/MainWindow.xaml.cs` | 239 | `c4d330391fdce80b` |
| 223 | `src/RefLab.Presentation/RefLab.Presentation.csproj` | 1409 | `e9c8852dbcc4f3c4` |
| 224 | `tests/RefLab.Application.Tests/Architecture/ApplicationLayerBoundaryTests.cs` | 1951 | `c5265ae219e31d84` |
| 225 | `tests/RefLab.Application.Tests/Behaviors/BehaviorTests.cs` | 4843 | `920d446fceebffe0` |
| 226 | `tests/RefLab.Application.Tests/DependencyInjectionTests.cs` | 5258 | `b51718631ae5d8fe` |
| 227 | `tests/RefLab.Application.Tests/Fakes/ApplicationTestHost.cs` | 3265 | `a9676feb934e3297` |
| 228 | `tests/RefLab.Application.Tests/Fakes/FakeAppDbContext.cs` | 3549 | `4f377d8f1501ed47` |
| 229 | `tests/RefLab.Application.Tests/Fakes/Fakes.cs` | 4599 | `931a0c3c58fe7a27` |
| 230 | `tests/RefLab.Application.Tests/Fakes/TestData.cs` | 5745 | `9a3592a7ad7bc756` |
| 231 | `tests/RefLab.Application.Tests/Features/Auditing/AuditingHandlerTests.cs` | 2630 | `11a6bf7133fcfa43` |
| 232 | `tests/RefLab.Application.Tests/Features/Billing/AccountingTests.cs` | 6113 | `a466789d72b67db7` |
| 233 | `tests/RefLab.Application.Tests/Features/Billing/BillingHandlerTests.cs` | 5971 | `14b1558671ebf95c` |
| 234 | `tests/RefLab.Application.Tests/Features/Catalogue/CatalogueHandlerTests.cs` | 3875 | `6cf2616163502780` |
| 235 | `tests/RefLab.Application.Tests/Features/Catalogue/CatalogueManagementTests.cs` | 7869 | `ba762567959a702c` |
| 236 | `tests/RefLab.Application.Tests/Features/Configuration/SettingsTests.cs` | 3158 | `5a8373f833ee41af` |
| 237 | `tests/RefLab.Application.Tests/Features/Derivation/DerivationTests.cs` | 3963 | `4add29fbd95c85fe` |
| 238 | `tests/RefLab.Application.Tests/Features/Identity/IdentityHandlerTests.cs` | 6617 | `f04827ff43ec95ab` |
| 239 | `tests/RefLab.Application.Tests/Features/Identity/OperatorManagementTests.cs` | 4302 | `95437fc37eb5647f` |
| 240 | `tests/RefLab.Application.Tests/Features/Microbiology/MicrobiologyTests.cs` | 4650 | `41b20b4351b0f2da` |
| 241 | `tests/RefLab.Application.Tests/Features/Parties/PartiesAndPricesTests.cs` | 3501 | `33b8f18a124058f6` |
| 242 | `tests/RefLab.Application.Tests/Features/Patients/PatientHandlerTests.cs` | 6998 | `dfd5c02f5f6aec74` |
| 243 | `tests/RefLab.Application.Tests/Features/Patients/PatientRecordsTests.cs` | 6791 | `0db77daaf9308e41` |
| 244 | `tests/RefLab.Application.Tests/Features/Reporting/ReportingTests.cs` | 6275 | `29de4fa2901807e7` |
| 245 | `tests/RefLab.Application.Tests/Features/Results/ResultEntryTests.cs` | 3191 | `af82a3ce40aaa60d` |
| 246 | `tests/RefLab.Application.Tests/Features/Results/ResultHandlerTests.cs` | 10405 | `efa1879d9880a243` |
| 247 | `tests/RefLab.Application.Tests/Features/SendOut/SendOutHandlerTests.cs` | 1930 | `f0973adba2999ba8` |
| 248 | `tests/RefLab.Application.Tests/Features/SendOut/SendOutWorkflowTests.cs` | 3634 | `29e5c721365d979b` |
| 249 | `tests/RefLab.Application.Tests/Features/Statistics/StatisticsTests.cs` | 3670 | `1deede2611472aa5` |
| 250 | `tests/RefLab.Application.Tests/Features/Worksheets/WorksheetTests.cs` | 2939 | `dfb7fb29fa4886d1` |
| 251 | `tests/RefLab.Application.Tests/PermissionEnforcementTests.cs` | 5825 | `4629c07fa6a1ef30` |
| 252 | `tests/RefLab.Application.Tests/RefLab.Application.Tests.csproj` | 1487 | `37dc33ce19f17370` |
| 253 | `tests/RefLab.Domain.Tests/Architecture/DomainLayerBoundaryTests.cs` | 2222 | `bb6e8a7222b5a1b3` |
| 254 | `tests/RefLab.Domain.Tests/Auditing/PrintAuditTests.cs` | 1629 | `b648f6f66f4fb851` |
| 255 | `tests/RefLab.Domain.Tests/Billing/PaymentTests.cs` | 1500 | `3cfbdbbaa03c9146` |
| 256 | `tests/RefLab.Domain.Tests/Billing/ReferringPartyTests.cs` | 1736 | `33cce6a39e18cdd0` |
| 257 | `tests/RefLab.Domain.Tests/ClinicalKnowledge/CatalogueUpdateTests.cs` | 2991 | `c00a01994d6b9bee` |
| 258 | `tests/RefLab.Domain.Tests/ClinicalKnowledge/ClinicalConstantTests.cs` | 2039 | `f8272dcc8bc1331d` |
| 259 | `tests/RefLab.Domain.Tests/ClinicalKnowledge/ConversionFactorTests.cs` | 1139 | `df4bbf8129053d0a` |
| 260 | `tests/RefLab.Domain.Tests/ClinicalKnowledge/ProthrombinControlCalibrationTests.cs` | 694 | `691befa332a23c7d` |
| 261 | `tests/RefLab.Domain.Tests/ClinicalKnowledge/ReferenceRangeTests.cs` | 2569 | `49198379c6945508` |
| 262 | `tests/RefLab.Domain.Tests/ClinicalKnowledge/TestDefinitionTests.cs` | 4319 | `b1611276f05e0bba` |
| 263 | `tests/RefLab.Domain.Tests/Configuration/LaboratorySettingsTests.cs` | 2040 | `bedc3923ecd06534` |
| 264 | `tests/RefLab.Domain.Tests/Farming/FarmTests.cs` | 1278 | `ce27c394cadf2f23` |
| 265 | `tests/RefLab.Domain.Tests/Identity/OperatorTests.cs` | 3885 | `32bf4f6473ec30bc` |
| 266 | `tests/RefLab.Domain.Tests/Identity/PasswordPolicyTests.cs` | 1760 | `1f7b293f81435ea6` |
| 267 | `tests/RefLab.Domain.Tests/Identity/PermissionTests.cs` | 759 | `308e3c924739d135` |
| 268 | `tests/RefLab.Domain.Tests/Microbiology/MicrobiologyTests.cs` | 3199 | `b842ac4fa3fa4d3f` |
| 269 | `tests/RefLab.Domain.Tests/Patients/OrderTestTests.cs` | 7746 | `517529d17e3dbf9e` |
| 270 | `tests/RefLab.Domain.Tests/Patients/PatientHistoryTests.cs` | 1825 | `4ed9e2bd8b41d33e` |
| 271 | `tests/RefLab.Domain.Tests/Patients/PatientOrderTests.cs` | 3918 | `85729ece9980d1bf` |
| 272 | `tests/RefLab.Domain.Tests/Patients/PatientPictureTests.cs` | 978 | `607bbd5a8c5bd73d` |
| 273 | `tests/RefLab.Domain.Tests/Patients/PatientTests.cs` | 2094 | `062e607144cd80f0` |
| 274 | `tests/RefLab.Domain.Tests/RefLab.Domain.Tests.csproj` | 1215 | `df3cbecf303de8f0` |
| 275 | `tests/RefLab.Domain.Tests/Rules/ClinicalConstantResolverTests.cs` | 1681 | `7042f212265a0e85` |
| 276 | `tests/RefLab.Domain.Tests/Rules/ClinicalDerivationPolicyTests.cs` | 4965 | `1d17cf6012e3bd58` |
| 277 | `tests/RefLab.Domain.Tests/Rules/CommissionCalculatorTests.cs` | 2373 | `1b12cebde8540809` |
| 278 | `tests/RefLab.Domain.Tests/Rules/IdentifierAllocatorTests.cs` | 2164 | `cb595bfc05fcc5fa` |
| 279 | `tests/RefLab.Domain.Tests/Rules/ReferenceRangeSelectorTests.cs` | 2801 | `7663809319b29fc3` |
| 280 | `tests/RefLab.Domain.Tests/Rules/ResultClassificationPolicyTests.cs` | 5414 | `911ed95748874517` |
| 281 | `tests/RefLab.Domain.Tests/SendOut/SampleDispositionTests.cs` | 1350 | `4e599e74a538d52e` |
| 282 | `tests/RefLab.Domain.Tests/SendOut/SendOutRequestTests.cs` | 2460 | `685459470dabdc8e` |
| 283 | `tests/RefLab.Domain.Tests/ValueObjects/AgeBandTests.cs` | 1804 | `d734fa035541b624` |
| 284 | `tests/RefLab.Domain.Tests/ValueObjects/CriticalIntervalTests.cs` | 1453 | `10eecdf3def9483f` |
| 285 | `tests/RefLab.Domain.Tests/ValueObjects/MoneyTests.cs` | 3005 | `de7b51ababc3cfac` |
| 286 | `tests/RefLab.Domain.Tests/ValueObjects/OrderIdentifierTests.cs` | 3980 | `a82403487052a83a` |
| 287 | `tests/RefLab.Domain.Tests/ValueObjects/PercentageTests.cs` | 1031 | `6690ffbd58dad59f` |
| 288 | `tests/RefLab.Domain.Tests/ValueObjects/ReferenceIntervalTests.cs` | 2915 | `4f4d8d5776e9e36a` |
| 289 | `tests/RefLab.Domain.Tests/ValueObjects/ResultValueTests.cs` | 1927 | `983578486bfa0dff` |
| 290 | `tests/RefLab.Infrastructure.Tests/InfrastructureServiceTests.cs` | 2213 | `5303b1b3c9dd3c8e` |
| 291 | `tests/RefLab.Infrastructure.Tests/Pbkdf2PasswordHasherTests.cs` | 1812 | `b7eabc66796df91e` |
| 292 | `tests/RefLab.Infrastructure.Tests/Persistence/MigrationTests.cs` | 8049 | `12ac6c2cf05c49ef` |
| 293 | `tests/RefLab.Infrastructure.Tests/Persistence/PersistenceIntegrationTests.cs` | 17576 | `cb79ddd7196c0d86` |
| 294 | `tests/RefLab.Infrastructure.Tests/Persistence/PersistenceRegistrationTests.cs` | 3491 | `75c4490224eadc13` |
| 295 | `tests/RefLab.Infrastructure.Tests/Persistence/RefLabDbContextTests.cs` | 5229 | `6f2dc52d0aa0c7db` |
| 296 | `tests/RefLab.Infrastructure.Tests/Persistence/SqliteTestDatabase.cs` | 2306 | `bf608072657621b2` |
| 297 | `tests/RefLab.Infrastructure.Tests/QuestPdfExporterTests.cs` | 8274 | `775fedee009671b8` |
| 298 | `tests/RefLab.Infrastructure.Tests/RefLab.Infrastructure.Tests.csproj` | 1598 | `d6621b51e29ff398` |

## 3. How to apply this delivery

1. Extract the archive **into** `C:\Users\LAP LINK\source\repos\Ref Lab`.
2. Choose *overwrite existing files*.
3. `README.md`, `docs/LOCAL_AGENT_IMPLEMENTATION_GUIDE.md`, `docs/USE_CASE_INVENTORY.md`
   and the files listed in `docs/LOCAL_AGENT_IMPLEMENTATION_GUIDE.md` section 1
   already exist and are replaced; everything else is new.
4. Run the build, test and migration commands in
   `docs/LOCAL_AGENT_IMPLEMENTATION_GUIDE.md` sections 5.1 and 5.2.

---

*End of `RL-P4-MAN-001`.*
