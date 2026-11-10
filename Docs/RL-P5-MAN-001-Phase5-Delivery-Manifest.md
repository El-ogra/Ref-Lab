# Ref Lab — Phase 5 Delivery Manifest

**Document ID:** `RL-P5-MAN-001`
**Scope:** the complete Phase 5 delivery — the WPF presentation layer.
**Archive:** `RefLab-Phase5-Presentation.zip`
**Baseline:** `https://github.com/El-ogra/Ref-Lab.git` @ `366aeb48a146efe02c05731773630cada143bda6`
**Issued:** 2026-11-10

> The archive contains a single root folder, `RefLab-Phase5-Presentation/`, and
> every path below is relative to that folder. The paths use the real casing of
> the repository: the documentation folder is `Docs`, with a capital `D`.

---

## 1. Verification status

| Gate | Result |
|---|---|
| Use cases implemented | **70 / 70** (unchanged) |
| Commands / queries | **45 / 25** — see `Docs/RL-P5-COR-001-Documentation-Corrigendum.md` |
| Feature areas | **14** (unchanged) |
| Use cases with a validator | **70 / 70** (unchanged) |
| Screens delivered | **14 windows + 2 in-place panels** |
| Pre-existing test baseline | **535 passed** (270 Domain + 204 Application + 61 Infrastructure) |
| New Phase 5 tests | **22 passed** (the presentation boundary guards and the architecture contract tests) |
| Total tests after the delivery | **557 passed, 0 failed, 0 skipped** |
| Build `Debug` | **0 errors, 0 warnings** |
| Build `Release` | **0 errors, 0 warnings** |
| Use-case drift guard | `use cases found: 70 (expected 70)`, exit code 0 |
| Assertions **B1–B9** | **all pass** |
| Arabic traceability outside the authorised source | **none** — every Arabic literal in `src/` is listed in `Docs/ui-labels-verified.md` |
| Residual references to the six removed SMS/NATIGH artefacts | **0 occurrences** across `src/` and `tests/` |
| Application abstractions | **8** (the two SMS and NATIGH ports are gone) |
| Branch table or branch column in the schema | none — the migrations are byte-identical to the pinned commit |
| Migrations created, modified or executed | **none** |
| Database schema or data changed | **none** |
| `RefLab.sln`, `Directory.Build.props`, `global.json`, `.gitignore`, `scripts/generate_use_case_inventory.py` | **untouched** |
| `src/RefLab.Presentation/RefLab.Presentation.csproj` | **unchanged** — no package reference was added |
| Domain, Application and Infrastructure | **not modified**, apart from the single scoped change of §3.2 |

### 1.1 The environment note

The build agent has the .NET **9.0.18** runtime and no .NET 8 runtime, so the
`net8.0` test host is started with `DOTNET_ROLL_FORWARD=LatestMajor`. This is a
runtime roll-forward and not a code difference; the owner machine runs .NET 8
natively. The WPF assembly is **compiled** on the agent and **executed** on the
owner machine, which is why the manual checklist in the implementation guide
carries the run-time checks.

---

## 2. The file manifest

The SHA-256 of every file below is the **full 64-character** digest of the file
as it is delivered. `tools/verify_manifest.py` recomputes them over the
extracted tree.

| Path | Change | Bytes | SHA-256 | Purpose |
|---|---|---:|---|---|
| 1 | `Docs/RL-P5-COR-001-Documentation-Corrigendum.md` | Added | 4753 | `4b9cce8ed27ae6c33e059c6634db25ad18c80219b599165699095927fe67c920` | The correction of the two erroneous command and query figures in RL-HANDOFF-003. |
| 2 | `src/RefLab.Infrastructure/Reports/ReportBranding.cs` | Added | 6396 | `ea041505cf52916f135d65d78923c6eb8d374c9c085072229e98fe9d578d2c81` | The Arabic and the English branding of every printed document, and the registration of an Arabic-capable font. |
| 3 | `src/RefLab.Presentation/Bootstrap/CompositionRoot.cs` | Added | 3465 | `5e2a222988df3ad9650d2b0f10e17a7581d36d6ac3204fc2e79b5d553a587f9f` | The composition root. Registers the Application, Infrastructure and Presentation layers and shows the main window. |
| 4 | `src/RefLab.Presentation/Bootstrap/PresentationServiceCollectionExtensions.cs` | Added | 3255 | `1ac189ba2f6a72c3672db82104c5c2be3fa9c6ab3e07ab081f91611d0a833a94` | The registration of the presentation layer: navigation, the request executor, the error presenter, the view models and the views. |
| 5 | `src/RefLab.Presentation/Common/ErrorPresenter.cs` | Added | 756 | `960e14a7db9db90601c441ac24df9718d519c1590e537461af9ee448676e9b5d` | The WPF implementation of the error presenter. |
| 6 | `src/RefLab.Presentation/Common/IErrorPresenter.cs` | Added | 982 | `47abbc78c6ced509d7247fd36e0af3c4f8ed1a5b0a88996ad0353e6f5a648c2c` | The abstraction that keeps a view model free of WPF dialogs. |
| 7 | `src/RefLab.Presentation/Common/LabCurrency.cs` | Added | 1092 | `fe96bc8de8bbeb920fc938989f831fe30290f8dee933d11515e6abf86ca87b78` | The single declaration of the laboratory currency, EGP. |
| 8 | `src/RefLab.Presentation/Common/OperatorSession.cs` | Added | 2914 | `a105afc03fa95f353602dc6c31a2248e7d6ea1409b62a52d0bbf60be772a5d1a` | Establishes and clears the session of the signed-in operator. |
| 9 | `src/RefLab.Presentation/Common/RequestExecutor.cs` | Added | 3269 | `b0b1cff283ca7f7a082acbd42fb219371e7ad7595d74fb5c9a38df9d3b7a990c` | The only channel to the Application layer, and the translator of the Application exception hierarchy. |
| 10 | `src/RefLab.Presentation/Common/UiExceptions.cs` | Added | 1077 | `90c65ba3dcbb295710e60ae1fa1fab3090aa1540a7904f20dc2d69266d600f30` | The two presentation-level exception types. |
| 11 | `src/RefLab.Presentation/Common/ViewModelBase.cs` | Added | 2624 | `c26e3f2d8ca74a04cfdfca26ee371a8cb6c7945517272aac5246ce5ab99c4610` | The base class of every view model: the busy flag, the error message and the status message. |
| 12 | `src/RefLab.Presentation/Navigation/INavigationAware.cs` | Added | 742 | `03538c52b701a28e2e67a4587d6355455a4bcaa6b3b835a430a3bf8b23cdfd95` | The contract a view model implements to receive a navigation parameter. |
| 13 | `src/RefLab.Presentation/Navigation/INavigationService.cs` | Added | 1142 | `25a40758121ef4a1d5c9ae385af86558109f8903abb5bc30a3792ede698d207a` | The navigation abstraction of the shell. |
| 14 | `src/RefLab.Presentation/Navigation/IViewLocator.cs` | Added | 539 | `5347388f037e0d3f7d2ca408e71d5dce22a07deb6006986878ebdfec89ccb9d5` | The abstraction that maps a screen key onto a view. |
| 15 | `src/RefLab.Presentation/Navigation/NavigationService.cs` | Added | 2634 | `08d1b33ffbafa682212fac8a2b4f4678cce5316ddcde1eb5b1f9ff8605b232fa` | The navigation service, with a short history so that the shell can go back. |
| 16 | `src/RefLab.Presentation/Navigation/ScreenKeys.cs` | Added | 1731 | `26d783de121f52a0ad9596ef25ec869302f549493dc1896a13a378c4e3a4f517` | The stable identifiers of the fourteen screens. |
| 17 | `src/RefLab.Presentation/Navigation/ScreenRegistry.cs` | Added | 1558 | `d7ced4d66eae1b16eb9aae343b81941cb6241ab54d580d7982ea7982a0ad5d05` | The navigation map of the application. |
| 18 | `src/RefLab.Presentation/Navigation/ViewLocator.cs` | Added | 956 | `5c4f8390f8527da22596c00c241d65bf0947fc648a2bb9cd8d1a29f15d8310d3` | The view locator, which resolves a view from the container. |
| 19 | `src/RefLab.Presentation/Resources/Converters/LabConverters.cs` | Added | 3023 | `62879dcdf902d29371e3dfa5ede3e95db99986a855ea3ebeb07d0cb2c8a1402d` | The value converters of the presentation layer. |
| 20 | `src/RefLab.Presentation/Resources/Strings.ar.xaml` | Added | 5387 | `02e96213151e7550dcffcbf121a10ee25aa04333f1c9e03c5298b35848e21779` | The Arabic labels of the patient panel and of the Arabic action row. Every string is authorised by Docs/ui-labels-verified.md. |
| 21 | `src/RefLab.Presentation/Resources/Strings.xaml` | Added | 12296 | `a418fe6f9780bee86fc9a1c56c53fdd3f139fe74ad6ff410dc3c2a1bb19b0f92` | The English labels of the whole application. |
| 22 | `src/RefLab.Presentation/Resources/Themes/Dark.xaml` | Added | 1933 | `93ec6056cca378ca5648e4e40b747896354530103c233e32f9b9c749ac842a00` | The dark palette. |
| 23 | `src/RefLab.Presentation/Resources/Themes/Light.xaml` | Added | 7668 | `72cebdf3095af29e658e1d1b14c5a3b8a6325df32ee9527523edf8def90e608c` | The light palette and the shared styles. |
| 24 | `src/RefLab.Presentation/ViewModels/AccountingViewModel.cs` | Added | 6935 | `50526093241ce5b6221a3de4dd9b664a3b2e49e6c1da40807645083bcf69062c` | The view model of the Accounting screen. |
| 25 | `src/RefLab.Presentation/ViewModels/AuditViewModel.cs` | Added | 2388 | `f533810b672d520e62c44465d415e2690eb2cb8891f0190d4279e7bff2491c4f` | The view model of the Audit screen. |
| 26 | `src/RefLab.Presentation/ViewModels/CatalogueAdministrationViewModel.cs` | Added | 13570 | `d46bc13ff20dcb6a3a1c01ed3b3a58eacc723cb37ae93de1e1501c17f144c982` | The view model of the CatalogueAdministration screen. |
| 27 | `src/RefLab.Presentation/ViewModels/CataloguePanelViewModel.cs` | Added | 1454 | `564025f32175bc733e4a38ab25a08fc77540c91192e95a2cc770d7688f7f2714` | The view model of the CataloguePanel screen. |
| 28 | `src/RefLab.Presentation/ViewModels/EnumSource.cs` | Added | 1517 | `a08b347bcd229fa8a003b5385aa85fe38947768cfda7c00cb443c6616907c8fa` | The enumeration values the presentation layer binds to. |
| 29 | `src/RefLab.Presentation/ViewModels/LaboratorySettingsViewModel.cs` | Added | 4213 | `500e82f363153b3994d6a8b7ed1a955cc4f1b4948e9b84d344966867489687b5` | The view model of the LaboratorySettings screen. |
| 30 | `src/RefLab.Presentation/ViewModels/MicrobiologyViewModel.cs` | Added | 6502 | `2e4f55a0de8652bdb48eea5afb01e085c89e92d075ffc5a71aded38ee732dc6a` | The view model of the Microbiology screen. |
| 31 | `src/RefLab.Presentation/ViewModels/OperatorAdministrationViewModel.cs` | Added | 6048 | `67a1cbe7c3e8fc4f052b1b0878ad75abb310bda28aca002a19a1ab523d24d933` | The view model of the OperatorAdministration screen. |
| 32 | `src/RefLab.Presentation/ViewModels/Panels/DerivationPanelViewModel.cs` | Added | 3767 | `14413113d32e1e2d9b62d6967991ccf2fc0cc9939b4b026bd0d6496869a37ded` | The view model of the DerivationPanel screen. |
| 33 | `src/RefLab.Presentation/ViewModels/Panels/MethodSheetPanelViewModel.cs` | Added | 2151 | `b4c5c1f0f047a53e0dfabd2b6c0b4d9e01d6d506611c08107bb28d927473fd1d` | The view model of the MethodSheetPanel screen. |
| 34 | `src/RefLab.Presentation/ViewModels/PartiesPricingViewModel.cs` | Added | 5379 | `b4a073ffc3ca9fb2b8d2dbe21047af74ee17007076bafcde28282d9413c35209` | The view model of the PartiesPricing screen. |
| 35 | `src/RefLab.Presentation/ViewModels/PatientPanelViewModel.cs` | Added | 3139 | `51f22b97df4e8edefb6ce1586de8ab72d4bbeac56ea25347e4a0a04b6127df36` | The view model of the PatientPanel screen. |
| 36 | `src/RefLab.Presentation/ViewModels/PatientWorkViewModel.cs` | Added | 22062 | `7fd12b9db77ed785d631c8384383bab2b68a5c50778bca913597d0e498222ba4` | The view model of the PatientWork screen. |
| 37 | `src/RefLab.Presentation/ViewModels/ReportSetupViewModel.cs` | Added | 6043 | `71c4c8acb7d1b14c9108abda15cf2218e4e066d7f66dfb3c32c8fc9ab087f636` | The view model of the ReportSetup screen. |
| 38 | `src/RefLab.Presentation/ViewModels/SendOutQueueViewModel.cs` | Added | 5170 | `8cabd9da5b6ddd506d57768edcea2285bf73f54003023b4eede3a6f3fa46d404` | The view model of the SendOutQueue screen. |
| 39 | `src/RefLab.Presentation/ViewModels/ShellViewModel.cs` | Added | 3686 | `e690aedd22bf4dae0f6137512e834b5c4d7df27ee7a7adf3e6da3f3b91834e88` | The view model of the Shell screen. |
| 40 | `src/RefLab.Presentation/ViewModels/SignInViewModel.cs` | Added | 2294 | `d047cc11ceaa14eb1fcd3fb7e713e066a9f04e43b4b564b3fb8a1291443e35f6` | The view model of the SignIn screen. |
| 41 | `src/RefLab.Presentation/ViewModels/StatisticsViewModel.cs` | Added | 3278 | `0fb79b1ba92aa8bdfbb798be49e351d71a17af27c2020d07d5018ce3a527bc57` | The view model of the Statistics screen. |
| 42 | `src/RefLab.Presentation/ViewModels/TestCatalogueViewModel.cs` | Added | 2828 | `35d9890e1d6d7ff2cb186d1d3d68fee46d470bb6a2d9469e93f17daeb6bfd3e9` | The view model of the TestCatalogue screen. |
| 43 | `src/RefLab.Presentation/Views/AccountingView.xaml` | Added | 5494 | `29e9e4beb571ecc57100c5555a8f3f3ff94a842d4e3c01a44bbe503891d171cf` | The markup of AccountingView. |
| 44 | `src/RefLab.Presentation/Views/AccountingView.xaml.cs` | Added | 515 | `adb8b9d3df3ad2e67f3e7ecacc0aae21be6829ca5d6c7de16e664b6f1d7aacd3` | The code-behind of AccountingView.. |
| 45 | `src/RefLab.Presentation/Views/AuditView.xaml` | Added | 3291 | `cccd4f962894e97d1e8040e68c3ca01ade92586342fe0606359938e9beb56814` | The markup of AuditView. |
| 46 | `src/RefLab.Presentation/Views/AuditView.xaml.cs` | Added | 495 | `65a4e6a157379b7d409c264d12f25c3ab770ebe361be3ade6dc78bc1305edff8` | The code-behind of AuditView.. |
| 47 | `src/RefLab.Presentation/Views/CatalogueAdministrationView.xaml` | Added | 8571 | `85f6a8b8a406ba852eea8821ce0fd4b696b932640e91e21dcf3e6fc942a038bc` | The markup of CatalogueAdministrationView. |
| 48 | `src/RefLab.Presentation/Views/CatalogueAdministrationView.xaml.cs` | Added | 567 | `2a18340460925bd70857f95ac32594052c7f0f22a23271224c467bb5595c9dfd` | The code-behind of CatalogueAdministrationView.. |
| 49 | `src/RefLab.Presentation/Views/Controls/CataloguePanelControl.xaml` | Added | 4016 | `21ae60abe903ae0d35052109f0ba6147f7620dba411fdbbf6156ca5934081d8f` | The CataloguePanelControl shared component. |
| 50 | `src/RefLab.Presentation/Views/Controls/CataloguePanelControl.xaml.cs` | Added | 333 | `233eed1ca619436a8f6ec0db980e45583fd82ab0e63aee581051d115ce9f4cd8` | The CataloguePanelControl shared component. |
| 51 | `src/RefLab.Presentation/Views/Controls/PatientPanelControl.xaml` | Added | 10572 | `ab3af789d7720afe4a66f750a010591a754caa0de2f127545e80cc8ef79e8136` | The PatientPanelControl shared component. |
| 52 | `src/RefLab.Presentation/Views/Controls/PatientPanelControl.xaml.cs` | Added | 327 | `58451200f1027cfefd8b7e5299bb7436d84f98e17a6c5ad97b46b01deeee53c0` | The PatientPanelControl shared component. |
| 53 | `src/RefLab.Presentation/Views/Controls/ResultsGridControl.xaml` | Added | 2022 | `630ad77e38dd5ff1d5791da1ca20e3fbd81027a84beb85d0fac225062f0d25c4` | The ResultsGridControl shared component. |
| 54 | `src/RefLab.Presentation/Views/Controls/ResultsGridControl.xaml.cs` | Added | 324 | `d2616d86d8d795dcc1e2d57e9d788008325171c80ddd3b65c3efbfc899b2faad` | The ResultsGridControl shared component. |
| 55 | `src/RefLab.Presentation/Views/LaboratorySettingsView.xaml` | Added | 3616 | `a6680b0e5061adac69628607853b1328d3ed2e7b3b5ace36eec29af1545d4210` | The markup of LaboratorySettingsView. |
| 56 | `src/RefLab.Presentation/Views/LaboratorySettingsView.xaml.cs` | Added | 547 | `e5a2a87fd5a60e9b7c3ac6294fc5079031d909b4e6f7178617f36b0e9447a40e` | The code-behind of LaboratorySettingsView.. |
| 57 | `src/RefLab.Presentation/Views/MicrobiologyView.xaml` | Added | 5588 | `f6a46d4fef3af0daf325d1dc2bd7a83c1a70c4e77ba2cadd5655516979c2358d` | The markup of MicrobiologyView. |
| 58 | `src/RefLab.Presentation/Views/MicrobiologyView.xaml.cs` | Added | 523 | `f14ec0c1df802cc714055fd62ef91344762592c99ac58f2195d13fe1f2b0233c` | The code-behind of MicrobiologyView.. |
| 59 | `src/RefLab.Presentation/Views/OperatorAdministrationView.xaml` | Added | 4963 | `363884fcfa83ebbab30c50b1163b64b47847f5b5507e742b85df92350da0fb1d` | The markup of OperatorAdministrationView. |
| 60 | `src/RefLab.Presentation/Views/OperatorAdministrationView.xaml.cs` | Added | 563 | `3a6bf4f04d756a3ae231524d231f8eb57990a7355830800904dca083b0b187b7` | The code-behind of OperatorAdministrationView.. |
| 61 | `src/RefLab.Presentation/Views/Panels/DerivationPanelView.xaml` | Added | 4127 | `fe6478065b9f605600dea49c69fd6268247b432ea9d2d0a70eb5cc858fe9c176` | The DerivationPanelView in-place panel. |
| 62 | `src/RefLab.Presentation/Views/Panels/DerivationPanelView.xaml.cs` | Added | 325 | `e6f18697e349a65454e60141dc3de179c07eac2f9c7c476a771c4047a4e65ee7` | The DerivationPanelView in-place panel. |
| 63 | `src/RefLab.Presentation/Views/Panels/MethodSheetPanelView.xaml` | Added | 2183 | `9faa6a35ef5099dcf7782a76ac689dcd2dbcfa504ed41b1cc5efd979d734f43c` | The MethodSheetPanelView in-place panel. |
| 64 | `src/RefLab.Presentation/Views/Panels/MethodSheetPanelView.xaml.cs` | Added | 328 | `2699fdd2b57adce0dccec61693fa925287c5f577b0034a1e299d574397e4e8c4` | The MethodSheetPanelView in-place panel. |
| 65 | `src/RefLab.Presentation/Views/PartiesPricingView.xaml` | Added | 5828 | `8725a5106157ac9d0ce31fc1126a893bd0771f8a508b59a4ca43aa36e539bd42` | The markup of PartiesPricingView. |
| 66 | `src/RefLab.Presentation/Views/PartiesPricingView.xaml.cs` | Added | 531 | `dd16079be34315ac1606417afa1ab426f220d6bf1d6ffe164e7fc30c4dbf6336` | The code-behind of PartiesPricingView.. |
| 67 | `src/RefLab.Presentation/Views/PatientWorkView.xaml` | Added | 11236 | `042b9b1a262efdb1573c2eac6d8984ff8bfc7dc705de38cbbf7ecbf06f29ed4b` | The markup of PatientWorkView. |
| 68 | `src/RefLab.Presentation/Views/PatientWorkView.xaml.cs` | Added | 519 | `248051ebc61f72ae43b79afbc4a651d188e82e090eb2b1e5526e3adbab3999fa` | The code-behind of PatientWorkView.. |
| 69 | `src/RefLab.Presentation/Views/ReportSetupView.xaml` | Added | 5442 | `7be6ec7f9a6bd45a02ae311cdcad40fe894d07eac67b4858d84f9fb16ffc5af9` | The markup of ReportSetupView. |
| 70 | `src/RefLab.Presentation/Views/ReportSetupView.xaml.cs` | Added | 519 | `ec7e3c199128b71b66006d3d9414955b23381fd531848b1ed0e876c9c02c069e` | The code-behind of ReportSetupView.. |
| 71 | `src/RefLab.Presentation/Views/SendOutQueueView.xaml` | Added | 4848 | `6a7c5850b7a36b49a0c4ac64aaecb980012a8bed8d6ab0c951993d65236c2eb8` | The markup of SendOutQueueView. |
| 72 | `src/RefLab.Presentation/Views/SendOutQueueView.xaml.cs` | Added | 523 | `9e84589ee9f1b94b5c064ffe4d63846f89e6e3f66e9f8d741fff84d411ba531c` | The code-behind of SendOutQueueView.. |
| 73 | `src/RefLab.Presentation/Views/SignInView.xaml` | Added | 2797 | `d3c02543981204fda184d2f88332896b42b036ee7cbc909ecc663ec53cd1748f` | The markup of SignInView. |
| 74 | `src/RefLab.Presentation/Views/SignInView.xaml.cs` | Added | 516 | `3d8800f29712e9763a0667d83cb41c7c5d578c3c8c94eece22b05fc6c0f0ed10` | The code-behind of SignInView.. |
| 75 | `src/RefLab.Presentation/Views/StatisticsView.xaml` | Added | 4054 | `d2843f0a792566a2fc4e923bed445f09e0475ae86bfb46710719707180d6f73b` | The markup of StatisticsView. |
| 76 | `src/RefLab.Presentation/Views/StatisticsView.xaml.cs` | Added | 515 | `f01f67e2a817e27a815437f07b10bdaed59143479ab2ebe3deb6eced56045267` | The code-behind of StatisticsView.. |
| 77 | `src/RefLab.Presentation/Views/TestCatalogueView.xaml` | Added | 3959 | `6d6149ad13cc799a3245e20bc459bfba34ac7e96b2155c45b6fe11ce1cfe0239` | The markup of TestCatalogueView. |
| 78 | `src/RefLab.Presentation/Views/TestCatalogueView.xaml.cs` | Added | 527 | `7e41de6163c760cb19fa80231612f8bd3727028f171747246403e24db2730a5f` | The code-behind of TestCatalogueView.. |
| 79 | `tests/RefLab.Application.Tests/Architecture/PresentationLayerBoundaryTests.cs` | Added | 22739 | `a9553a953e12cc55afa342c42b37e4a056ef6af218b66e80b4565dfd8d9ef67c` | The presentation boundary guards B1 to B9 and the Phase 5 architecture contract tests. |
| 80 | `tools/verify_manifest.py` | Added | 5298 | `a1279644fff6c8c2ee5c37c7ce0fbdb20059bf8cc6283b326086555c71de6e66` | Recomputes the SHA-256 of every file of the delivery over the extracted tree. |
| 81 | `Docs/LOCAL_AGENT_IMPLEMENTATION_GUIDE.md` | Replaced | 27213 | `ff6f116b14586aad9feb45a137922dada8b6d1e7e648c1abfb204563516653ae` | This guide. Replaced: the Phase 5 edition. |
| 82 | `Docs/ui-labels-verified.md` | Replaced | 17888 | `c4404a7344ff9be05f64daa68c763fbe72ddc72cec16e985759333e07aa1e9e8` | The authorised source of Arabic labels. Replaced: it now carries section 9, the four owner-approved strings from the V2 corpus. |
| 83 | `src/RefLab.Infrastructure/Reports/QuestPdfExporter.cs` | Replaced | 4576 | `a3dcb4848c646ad3940e14490abc610b712ee96516b3f10db8095ce952693145` | The PDF exporter. Replaced: it now applies the branding block to every document. |
| 84 | `src/RefLab.Presentation/App.xaml` | Replaced | 1230 | `716558690d4bdb4aa5930e11db4e39a2441ccfa9626a6e85da2fe786492197c1` | The markup of App. |
| 85 | `src/RefLab.Presentation/App.xaml.cs` | Replaced | 1372 | `6195a2d4027f7bdf6f0cfc88ffaaf5dcc2a3d39328dd71031d15de54c125325d` | The code-behind of App.. |
| 86 | `src/RefLab.Presentation/MainWindow.xaml` | Replaced | 6721 | `bff2cee732c309dbadf0017ccabb9c3c36573303ddefc01cf67719c00cfd73c9` | The markup of MainWindow. |
| 87 | `src/RefLab.Presentation/MainWindow.xaml.cs` | Replaced | 835 | `5449a222ff794f4c91691c19def08d5d1cfc536a6e593d201f7c3c542467e551` | The code-behind of MainWindow.. |
| — | `Docs/RL-P5-MAN-001-Phase5-Delivery-Manifest.md` | Added | — | — | This manifest. It cannot carry its own hash; its SHA-256 is published in the delivery report and is printed by the verification tool. |

**Total: 87 files** — 80 added and 7 replaced.

### 2.1 The files that are unchanged on purpose

| Path | Why it is not in the archive |
|---|---|
| `src/RefLab.Presentation/RefLab.Presentation.csproj` | The project already referenced CommunityToolkit.Mvvm 8.4.2 and Microsoft.Extensions.Hosting 8.0.1, and already referenced the Application, Domain and Infrastructure projects. No package was added and no project reference changed, so the file is byte-identical to the pinned commit. |
| `RefLab.sln` | The boundary guards live in the **existing** test project, so the solution keeps its seven projects. |
| `Directory.Build.props`, `global.json`, `.gitignore` | The build contract is unchanged. |
| `scripts/generate_use_case_inventory.py` | The use-case inventory is unchanged: 70 use cases, 14 feature areas. |
| Every file under `src/RefLab.Domain/` and `src/RefLab.Application/` | Phase 5 adds the presentation layer; it does not revise the layers beneath it. |
| Every migration under `src/RefLab.Infrastructure/Persistence/Migrations/` | The migration is already applied to the owner's database and is never modified. |

---

## 3. The deletions

A ZIP cannot delete a file. The six files below are removed by the owner's
machine, as §3.3 of `Docs/LOCAL_AGENT_IMPLEMENTATION_GUIDE.md` instructs.

| # | Path | Why it is removed | Evidence |
|---:|---|---|---|
| 1 | `src/RefLab.Domain/Enums/DeliveryChannel.cs` | The SMS and NATIGH delivery channels of a permanently removed capability. | Referenced only by `DeliveryRecord.cs` and `DeliveryRecordTests.cs`; not mapped by EF Core; 0 occurrences in the migration and the model snapshot; no use case. |
| 2 | `src/RefLab.Domain/Auditing/DeliveryRecord.cs` | The digital-delivery audit entity of a permanently removed capability. | Referenced only by its own test; no `DbSet`, no configuration, no DI registration, no use case. |
| 3 | `src/RefLab.Application/Abstractions/ISmsGateway.cs` | The SMS port. | 0 consumers; no implementation; not registered by `AddInfrastructureLayer()`. |
| 4 | `src/RefLab.Application/Abstractions/INatighGateway.cs` | The NATIGH.COM port. | 0 consumers; no implementation; not registered by `AddInfrastructureLayer()`. |
| 5 | `src/RefLab.Application/Common/GatewayModels.cs` | The four SMS and NATIGH result models. | Consumed only by the two ports above; nothing else references the file or any of its four types. |
| 6 | `tests/RefLab.Domain.Tests/Auditing/DeliveryRecordTests.cs` | Three tests of the removed delivery entity. | The only consumer of `DeliveryRecord`. |

**The removal is closed.** Nothing outside these six files refers to any of
them, so no other file has to change. The Phase 4 handoff already lists all
six as deleted; they were still present in the pinned commit.

### 3.1 The effect on the test baseline

`DeliveryRecordTests` carries three `[Fact]` tests. Their removal takes the
Domain suite from **273 to 270** and the total from **538 to 535**, which is
exactly the figure the Phase 4 handoff records for the build agent. **The
535-versus-538 difference between the build agent and the owner's machine was
never a runner artefact: it was these three tests.**

### 3.2 The one Infrastructure change

| Path | Change |
|---|---|
| `src/RefLab.Infrastructure/Reports/ReportBranding.cs` | **Added.** The Arabic and the English branding of every printed document, and the registration of an Arabic-capable font from the operating system. |
| `src/RefLab.Infrastructure/Reports/QuestPdfExporter.cs` | **Replaced.** One wiring call: `ReportBranding.Apply(column)` at the top of the page content. **Nothing else in the file changes.** |

No interface, no DTO and no behaviour of the Application or the Domain layer
changes. The branding is applied behind `IReportRenderer` and `IPdfExporter`,
so the presentation layer never sees the strings and never assembles report
content.

> **Why the exporter and not `QuestPdfReportRenderer`.** The renderer produces
> the `ReportDocument` model; the **exporter** is the component that composes
> the page and writes the PDF. The Arabic lines need an Arabic-capable font
> and a right-to-left direction, and both can only be set where the page is
> composed. The change is therefore one call in the exporter and one new file.

> **Why the currency appears in the branding.** The report request carries no
> currency — it is a frozen Application-layer contract that Phase 5 does not
> change — so the renderer has no access to the order. The laboratory's own
> currency, `EGP`, is therefore stated in the branding block of every
> document, which is also where the legacy product printed its identity. The
> per-order currency remains the authority in the domain and in the user
> interface, where it is supplied by `Common/LabCurrency.cs` and formatted from
> the `CurrencyCode` of the DTO.

### 3.3 The Arabic labels

`Docs/ui-labels-verified.md` is replaced with a new section 9 that records the
four owner-approved Arabic strings with their provenance:

| # | String | Provenance |
|---:|---|---|
| 1 | «الإحصائيات ومتابعة أداء المعمل» | `RL-V2-FRM-001` §2, `V2-REQ-054`/`V2-REQ-055` |
| 2 | «هل الفلوس المحصلة من مرضى الفترة فقط؟» | `RL-V2-FRM-001` §3, `V2-REQ-025` |
| 3 | «عالٍ أو منخفضة أو حرجة» | `RL-V2-FRM-001` §3, `V2-REQ-014` |
| 4 | «شاشة الوحدات و عوامل التحويل» | the visual reference material, approved by the owner; **not** in the V2 corpus |

The three classification terms «عالٍ», «منخفضة» and «حرجة» are the words of the
approved triple and are recorded individually in section 9.2.

The presentation boundary guard **B9** asserts that **every Arabic literal in
`src/` appears in `Docs/ui-labels-verified.md`**, so an unauthorised Arabic
string cannot reach the user interface. The guard is green.

---

## 4. The preserved contracts

| Contract | How it is preserved | How it is enforced |
|---|---|---|
| MediatR **12.5.0** | No package reference changes anywhere. | The package reference is unchanged in every project file. |
| `TreatWarningsAsErrors=true` | Every file of the delivery compiles without a warning. | 0 warnings in Debug and in Release. |
| `RefLab.Domain` has **0** package and **0** project references | The domain is untouched by the delivery. | `DomainLayerBoundaryTests`. |
| The UI communicates through **MediatR** | The only channel from a view model to the Application layer is `RequestExecutor`, which sends through `IMediator`. | `B4`, `B7`. |
| No business logic in a view model | Every displayed value is copied from a DTO. | Review; the Application layer's 204 pre-existing tests carry the logic. |
| `IAppDbContext.Set<T>()` returns `IQueryable<T>` | The Application and the Domain are untouched. | `ApplicationLayerBoundaryTests`. |
| The 13-character `OrderIdentifier` layout | The screen never assembles an identifier; it splits the entered value on the published layout and the domain reassembles and validates it. | The domain raises `InvalidOrderIdentifierException`, which `DomainExceptionBehavior` translates. |
| The collection-instance replacement rule | No collection-valued property is mutated in place. | The Domain and the Infrastructure are untouched. |
| Converters return the backing-field type | The presentation converters return the type the bound property holds. | Review. |
| `System.Windows.Application` fully qualified | `App.xaml.cs` uses the fully qualified form, and every file of the delivery that mentions the type uses it. | `B8`. |
| The keyboard shortcuts **F8 / F9 / F11 / F12 / F7** | Bound as WPF input bindings: F8 reviews, F9 saves, F11 prepares the print, F12 prints, F7 moves to the send-out queue. | `The_binding_keyboard_shortcuts_are_declared`. |
| `SessionCurrentUser` stays the session singleton | The presentation layer establishes and clears the session through it. | `Common/OperatorSession.cs`. |
| `PrintSequenceGenerator` stays process-scoped and in memory | Unchanged. The presentation layer only reads the sequence number. | The Infrastructure is untouched. |
| The language and `FlowDirection` boundaries | The Arabic panel is right-to-left, the catalogue panel and the application chrome are left-to-right, and the direction is set per panel — never globally. | Review; `B9` and the manual checklist. |
| **Branch, SMS and NATIGH stay removed** | The six residual files are deleted; the delivery contains no reference to any of them. | `B5`, `B6` and the negative grep of §1. |
| No migration is created, modified or executed | The migrations are byte-identical to the pinned commit, and the composition root never resolves the `DatabaseInitializer`. | `The_composition_root_never_resolves_the_database_initialiser`. |

---

## 5. The clean-room evidence

The sequence below was executed on the delivery source tree before the archive
was written.

The same sequence was then executed **again on the extracted archive** — that
is, on the pinned repository with this archive overlaid and the six deletions
applied — and the result is published in the delivery report, together with the
SHA-256 of the archive. It is not repeated here because the manifest is itself
a member of the archive: an archive cannot contain the evidence of its own
extraction.

```
### 1. RESTORE
    projects restored: 0

### 2. DEBUG BUILD
    Build succeeded.
        0 Warning(s)
        0 Error(s)

### 3. RELEASE BUILD
    Build succeeded.
        0 Warning(s)
        0 Error(s)

### 4. RELEASE TEST
    Passed!  - Failed:     0, Passed:   270, Skipped:     0, Total:   270, Duration: 512 ms - RefLab.Domain.Tests.dll (net8.0)
    Passed!  - Failed:     0, Passed:    61, Skipped:     0, Total:    61, Duration: 18 s - RefLab.Infrastructure.Tests.dll (net8.0)
    Passed!  - Failed:     0, Passed:   226, Skipped:     0, Total:   226, Duration: 28 s - RefLab.Application.Tests.dll (net8.0)

### 5. USE CASE INVENTORY DRIFT GUARD
    use cases found: 70 (expected 70)
    inventory written: /run/csi/mount-root/nas/eab0d61a99b6696edb3d2aff87b585e8/work/docs/USE_CASE_INVENTORY.md
    exit code: 0

### 6. NEGATIVE GREP — the six removed SMS/NATIGH artefacts, across src/ and tests/
    DeliveryRecord             0 occurrence(s)
    DeliveryChannel            0 occurrence(s)
    ISmsGateway                0 occurrence(s)
    INatighGateway             0 occurrence(s)
    SmsSendResult              0 occurrence(s)
    NatighPublicationRequest   0 occurrence(s)

### 7. THE APPLICATION ABSTRACTIONS AFTER THE REMOVAL
    count: 8
      IAppDbContext.cs
      IClock.cs
      ICurrentUser.cs
      IPasswordHasher.cs
      IPdfExporter.cs
      IPrintSequenceGenerator.cs
      IReportRenderer.cs
      IUnitOfWork.cs

### 8. THE DOMAIN ENUMERATIONS AFTER THE REMOVAL
      AgeUnit.cs
      AttendanceEventType.cs
      RecordStatus.cs
      ReportCategory.cs
      ResultClassification.cs
      ResultState.cs
      SampleDispositionAction.cs
      SendOutState.cs
      Sex.cs
      SexScope.cs

### 9. MIGRATIONS — untouched
      20261110103458_InitialCreate.Designer.cs
      20261110103458_InitialCreate.cs
      RefLabDbContextModelSnapshot.cs
    migration content changed since the pinned commit:
      (no output above = the migrations are byte-identical to the pinned commit)
```

---

## 6. Known legacy gaps

Two surfaces of the reference system have no Application-layer use case and
were placed **out of scope** by the owner. They are recorded here so that they
are not lost, and they are Phase 7 candidates.

| # | Legacy surface | Why there is no use case | What Ref Lab does instead |
|---:|---|---|---|
| 1 | The **barcode-label template editor** — per-sample-type barcode templates, `Top`/`Left` offsets, `Print File Barcode` against `Print Lab Id Barcode`, `Print All Barcode` and `Other Barcode`. | `UC-043 GeneratePatientBarcodeCommand` generates a barcode; nothing among the seventy use cases models a template, an offset or a sample-type layout. | `UC-043` generates the barcode of the order and the barcode printer role prints it. |
| 2 | The **per-profile report layout designer** — column positions, column widths, a per-position font and the twenty colour keys of each profile. | `UC-022 CustomiseReportLayoutCommand` carries the four margins, the background colour and one layout string. | `UC-022` and `UC-048`/`UC-049` configure the margins, the paper size, the printer bindings and the global header and footer texts. |

---

## 7. The presentation boundary guards

The guards live in the **existing** test project
`tests/RefLab.Application.Tests/Architecture/PresentationLayerBoundaryTests.cs`.
No test project was added and the solution keeps its seven projects.

| Guard | What it proves |
|---|---|
| **B1** | The presentation project declares no persistence, data-access or PDF package. |
| **B2** | The presentation project references only the Application, Domain and Infrastructure projects. |
| **B3** | The only MVVM framework is the approved `CommunityToolkit.Mvvm` 8.4.2. |
| **B4** | No source file of the presentation layer reaches a persistence concern. |
| **B5** | No source file of the presentation layer brings back the SMS or the NATIGH capability. |
| **B6** | No source file of the presentation layer brings back the branch capability. |
| **B7** | No type of the presentation layer exposes a persistence type. |
| **B8** | The WPF application type is always fully qualified. |
| **B9** | Every Arabic string in the source of the laboratory is authorised by `Docs/ui-labels-verified.md`. |

Eight further tests carry the Phase 5 architecture contracts: the startup URI
is absent, the application class derives from the fully qualified type, the
composition root registers the three approved layers, the composition root
never resolves the database initialiser, the currency is `EGP`, the five
keyboard shortcuts are declared, the two resource dictionaries use disjoint key
sets, and the report branding carries the verified branding.

**Why the guards are a static analysis rather than a runtime inspection of the
compiled assembly.** The presentation project targets
`net8.0-windows10.0.17763.0` and the test project targets `net8.0`; a `net8.0`
project cannot reference or load a `net8.0-windows` assembly — the restore
fails with **NU1201**, which was reproduced during the delivery. The static
analysis is also the stronger guarantee: it inspects the *declared* dependencies,
so it catches a transitive dependency that a runtime reference inspection cannot
see, and it runs on the build agent, which has no Windows desktop runtime.

---

**End of `RL-P5-MAN-001`.**
