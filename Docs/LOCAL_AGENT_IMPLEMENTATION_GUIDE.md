# Local Agent Implementation Guide — Phase 5

**Document ID:** `RL-P5-LOCAL-GUIDE-001`
**Applies to:** `RefLab-Phase5-Presentation.zip`
**Project root:** `C:\Users\LAP LINK\source\repos\Ref Lab`
**Issued:** 2026-11-10
**Edition:** Phase 5 — supersedes the Phase 4 edition

> This guide is written for the agent or developer who applies the Phase 5
> delivery on the owner machine. It is deliberately explicit about what to
> delete, what to run and what the result must look like, so that the
> delivery can be applied without re-reading the implementation.

---

## ⚠ 0. Read this first — the `System.Windows.Application` collision

Inside `src/RefLab.Presentation`, the **namespace** `RefLab.Application` shadows the
**WPF type** `System.Windows.Application`. Any unqualified reference to
`Application` in that project resolves to the namespace and fails to compile
with **CS0118**, and `TreatWarningsAsErrors=true` turns that into a build
break.

**The rule:** whenever a file of the presentation layer needs the WPF type, it
must be written in full:

```csharp
public partial class App : System.Windows.Application
```

The delivered `App.xaml.cs` already does this. **Keep it that way in every new
file.** This is the single most likely way to lose an hour on the first build.

> ⚠ **The second build trap.** `RefLab.Application` also appears inside
> `RefLab.Presentation` as the *namespace of the application layer*. Always
> write `RefLab.Application.Abstractions.ICurrentUser` in full; never write
> `Application.ICurrentUser`.

---

## 1. What Phase 5 contains

Phase 5 adds the WPF presentation layer: the composition root, the shell, the
fourteen screens, the two in-place panels, the navigation, the localisation
resources and the presentation boundary guards.

| | |
|---|---|
| Screens | **14** windows and **2** in-place panels |
| Use cases reached | **70 of 70** |
| Files added | 77 files under `src/RefLab.Presentation`, plus the branding file, the boundary test and two documents |
| Files replaced | 7 |
| Files deleted | 6 |
| Project files changed | **None** — `RefLab.sln`, `Directory.Build.props`, `global.json`, `.gitignore` and `scripts/generate_use_case_inventory.py` are untouched, and `RefLab.Presentation.csproj` is **unchanged** |

**The application layer, the domain layer and the migrations are not modified**,
apart from the one scoped Infrastructure change of §4.2.

---

## 2. Extraction procedure

1. **Verify the archive before you extract it.** The SHA-256 of the archive is
   published in the delivery report. Compare it with the file you received:

   ```powershell
   Get-FileHash -Algorithm SHA256 .\RefLab-Phase5-Presentation.zip
   ```

2. **Extract to a temporary folder, not over the working tree.** This lets you
   compare and lets you roll back:

   ```powershell
   $stage = "$env:TEMP\RefLab-Phase5"
   Remove-Item -Recurse -Force $stage -ErrorAction SilentlyContinue
   Expand-Archive -Path .\RefLab-Phase5-Presentation.zip -DestinationPath $stage
   ```

3. **Check the layout.** The archive contains a single root folder:

   ```
   RefLab-Phase5-Presentation\
       src\RefLab.Presentation\...
       src\RefLab.Infrastructure\Reports\...
       tests\RefLab.Application.Tests\Architecture\...
       Docs\...
       tools\verify_manifest.py
   ```

   > ⚠ **Keep the capital `D` in `Docs`.** The repository folder is `Docs`,
   > not `docs`. The Phase 4 manifest wrote `docs` in lower case, and on a
   > case-sensitive file system that creates a second folder instead of
   > overwriting the existing documents. The delivered paths use the real
   > casing throughout.

4. **Verify every file against the manifest** before you integrate anything:

   ```powershell
   python "$stage\RefLab-Phase5-Presentation\tools\verify_manifest.py" `
       --root "$stage\RefLab-Phase5-Presentation" `
       --manifest "$stage\RefLab-Phase5-Presentation\Docs\RL-P5-MAN-001-Phase5-Delivery-Manifest.md"
   ```

   The tool recomputes the SHA-256 of every file and exits with a non-zero
   code on the first mismatch.

---

## 3. Integration procedure

### 3.1 Back up first

```powershell
cd "C:\Users\LAP LINK\source\repos\Ref Lab"
Copy-Item -Recurse src\RefLab.Presentation "$env:TEMP\RefLab.Presentation.backup"
Copy-Item src\RefLab.Infrastructure\Reports\QuestPdfExporter.cs "$env:TEMP\QuestPdfExporter.cs.backup"
Copy-Item Docs\ui-labels-verified.md "$env:TEMP\ui-labels-verified.md.backup"
Copy-Item Docs\LOCAL_AGENT_IMPLEMENTATION_GUIDE.md "$env:TEMP\LOCAL_AGENT_IMPLEMENTATION_GUIDE.md.backup"
```

### 3.2 Copy the delivery over the project

Copy the whole content of the staged `RefLab-Phase5-Presentation` folder over
the project root, preserving the relative paths:

```powershell
$source = "$stage\RefLab-Phase5-Presentation\*"
$target = "C:\Users\LAP LINK\source\repos\Ref Lab"
Copy-Item -Path $source -Destination $target -Recurse -Force
```

This overwrites the replaced files, adds the new files and replaces the two
documents. It does **not** touch anything else.

### 3.3 Delete the six obsolete files

These files belong to the SMS and NATIGH capability that the owner removed
permanently. An archive cannot delete a file, so they are removed here.

| # | Path | Why it is removed |
|---:|---|---|
| # | Path | Why it is removed |
|---:|---|---|
| 1 | `src/RefLab.Domain/Enums/DeliveryChannel.cs` | The SMS and NATIGH delivery channels — a permanently removed capability. |
| 2 | `src/RefLab.Domain/Auditing/DeliveryRecord.cs` | The digital-delivery audit entity of a permanently removed capability. |
| 3 | `src/RefLab.Application/Abstractions/ISmsGateway.cs` | The SMS port. No implementation, no registration, no consumer. |
| 4 | `src/RefLab.Application/Abstractions/INatighGateway.cs` | The NATIGH.COM port. No implementation, no registration, no consumer. |
| 5 | `src/RefLab.Application/Common/GatewayModels.cs` | The four SMS and NATIGH result models, consumed only by the two ports above. |
| 6 | `tests/RefLab.Domain.Tests/Auditing/DeliveryRecordTests.cs` | Three tests of the removed delivery entity. |

Delete them exactly:

```powershell
cd "C:\Users\LAP LINK\source\repos\Ref Lab"
Remove-Item src\RefLab.Domain\Enums\DeliveryChannel.cs
Remove-Item src\RefLab.Domain\Auditing\DeliveryRecord.cs
Remove-Item src\RefLab.Application\Abstractions\ISmsGateway.cs
Remove-Item src\RefLab.Application\Abstractions\INatighGateway.cs
Remove-Item src\RefLab.Application\Common\GatewayModels.cs
Remove-Item tests\RefLab.Domain.Tests\Auditing\DeliveryRecordTests.cs
```

**Why they are removed.** The Phase 4 handoff lists all six as deleted, but
they were still present in the pinned commit
`366aeb48a146efe02c05731773630cada143bda6`. They are the last remnants of the
removed capability, and their presence is why the test suites differed by
three tests between the build agent and the owner's machine. The removal is
**closed**: nothing outside the six files refers to any of them, so no other
file needs to change.

### 3.4 Check that nothing else changed

```powershell
git status --short
```

The only paths that may appear are the ones listed in §4. If a path outside
that list appears, restore it from the backup.

---

## 4. The file lists

### 4.1 Files added

| # | Path | Purpose |
|---:|---|---|
| 1 | `Docs/RL-P5-COR-001-Documentation-Corrigendum.md` | The correction of the two erroneous command and query figures in RL-HANDOFF-003. |
| 2 | `src/RefLab.Infrastructure/Reports/ReportBranding.cs` | The Arabic and the English branding of every printed document, and the registration of an Arabic-capable font. |
| 3 | `src/RefLab.Presentation/Bootstrap/CompositionRoot.cs` | The composition root. Registers the Application, Infrastructure and Presentation layers and shows the main window. |
| 4 | `src/RefLab.Presentation/Bootstrap/PresentationServiceCollectionExtensions.cs` | The registration of the presentation layer: navigation, the request executor, the error presenter, the view models and the views. |
| 5 | `src/RefLab.Presentation/Common/ErrorPresenter.cs` | The WPF implementation of the error presenter. |
| 6 | `src/RefLab.Presentation/Common/IErrorPresenter.cs` | The abstraction that keeps a view model free of WPF dialogs. |
| 7 | `src/RefLab.Presentation/Common/LabCurrency.cs` | The single declaration of the laboratory currency, EGP. |
| 8 | `src/RefLab.Presentation/Common/OperatorSession.cs` | Establishes and clears the session of the signed-in operator. |
| 9 | `src/RefLab.Presentation/Common/RequestExecutor.cs` | The only channel to the Application layer, and the translator of the Application exception hierarchy. |
| 10 | `src/RefLab.Presentation/Common/UiExceptions.cs` | The two presentation-level exception types. |
| 11 | `src/RefLab.Presentation/Common/ViewModelBase.cs` | The base class of every view model: the busy flag, the error message and the status message. |
| 12 | `src/RefLab.Presentation/Navigation/INavigationAware.cs` | The contract a view model implements to receive a navigation parameter. |
| 13 | `src/RefLab.Presentation/Navigation/INavigationService.cs` | The navigation abstraction of the shell. |
| 14 | `src/RefLab.Presentation/Navigation/IViewLocator.cs` | The abstraction that maps a screen key onto a view. |
| 15 | `src/RefLab.Presentation/Navigation/NavigationService.cs` | The navigation service, with a short history so that the shell can go back. |
| 16 | `src/RefLab.Presentation/Navigation/ScreenKeys.cs` | The stable identifiers of the fourteen screens. |
| 17 | `src/RefLab.Presentation/Navigation/ScreenRegistry.cs` | The navigation map of the application. |
| 18 | `src/RefLab.Presentation/Navigation/ViewLocator.cs` | The view locator, which resolves a view from the container. |
| 19 | `src/RefLab.Presentation/Resources/Converters/LabConverters.cs` | The value converters of the presentation layer. |
| 20 | `src/RefLab.Presentation/Resources/Strings.ar.xaml` | The Arabic labels of the patient panel and of the Arabic action row. Every string is authorised by Docs/ui-labels-verified.md. |
| 21 | `src/RefLab.Presentation/Resources/Strings.xaml` | The English labels of the whole application. |
| 22 | `src/RefLab.Presentation/Resources/Themes/Dark.xaml` | The dark palette. |
| 23 | `src/RefLab.Presentation/Resources/Themes/Light.xaml` | The light palette and the shared styles. |
| 24 | `src/RefLab.Presentation/ViewModels/AccountingViewModel.cs` | The view model of the Accounting screen. |
| 25 | `src/RefLab.Presentation/ViewModels/AuditViewModel.cs` | The view model of the Audit screen. |
| 26 | `src/RefLab.Presentation/ViewModels/CatalogueAdministrationViewModel.cs` | The view model of the CatalogueAdministration screen. |
| 27 | `src/RefLab.Presentation/ViewModels/CataloguePanelViewModel.cs` | The view model of the CataloguePanel screen. |
| 28 | `src/RefLab.Presentation/ViewModels/EnumSource.cs` |  |
| 29 | `src/RefLab.Presentation/ViewModels/LaboratorySettingsViewModel.cs` | The view model of the LaboratorySettings screen. |
| 30 | `src/RefLab.Presentation/ViewModels/MicrobiologyViewModel.cs` | The view model of the Microbiology screen. |
| 31 | `src/RefLab.Presentation/ViewModels/OperatorAdministrationViewModel.cs` | The view model of the OperatorAdministration screen. |
| 32 | `src/RefLab.Presentation/ViewModels/Panels/DerivationPanelViewModel.cs` | The view model of the DerivationPanel screen. |
| 33 | `src/RefLab.Presentation/ViewModels/Panels/MethodSheetPanelViewModel.cs` | The view model of the MethodSheetPanel screen. |
| 34 | `src/RefLab.Presentation/ViewModels/PartiesPricingViewModel.cs` | The view model of the PartiesPricing screen. |
| 35 | `src/RefLab.Presentation/ViewModels/PatientPanelViewModel.cs` | The view model of the PatientPanel screen. |
| 36 | `src/RefLab.Presentation/ViewModels/PatientWorkViewModel.cs` | The view model of the PatientWork screen. |
| 37 | `src/RefLab.Presentation/ViewModels/ReportSetupViewModel.cs` | The view model of the ReportSetup screen. |
| 38 | `src/RefLab.Presentation/ViewModels/SendOutQueueViewModel.cs` | The view model of the SendOutQueue screen. |
| 39 | `src/RefLab.Presentation/ViewModels/ShellViewModel.cs` | The view model of the Shell screen. |
| 40 | `src/RefLab.Presentation/ViewModels/SignInViewModel.cs` | The view model of the SignIn screen. |
| 41 | `src/RefLab.Presentation/ViewModels/StatisticsViewModel.cs` | The view model of the Statistics screen. |
| 42 | `src/RefLab.Presentation/ViewModels/TestCatalogueViewModel.cs` | The view model of the TestCatalogue screen. |
| 43 | `src/RefLab.Presentation/Views/AccountingView.xaml` | The markup of AccountingView. |
| 44 | `src/RefLab.Presentation/Views/AccountingView.xaml.cs` | The code-behind of AccountingView.. |
| 45 | `src/RefLab.Presentation/Views/AuditView.xaml` | The markup of AuditView. |
| 46 | `src/RefLab.Presentation/Views/AuditView.xaml.cs` | The code-behind of AuditView.. |
| 47 | `src/RefLab.Presentation/Views/CatalogueAdministrationView.xaml` | The markup of CatalogueAdministrationView. |
| 48 | `src/RefLab.Presentation/Views/CatalogueAdministrationView.xaml.cs` | The code-behind of CatalogueAdministrationView.. |
| 49 | `src/RefLab.Presentation/Views/Controls/CataloguePanelControl.xaml` | The markup of CataloguePanelControl. |
| 50 | `src/RefLab.Presentation/Views/Controls/CataloguePanelControl.xaml.cs` | The code-behind of CataloguePanelControl.. |
| 51 | `src/RefLab.Presentation/Views/Controls/PatientPanelControl.xaml` | The markup of PatientPanelControl. |
| 52 | `src/RefLab.Presentation/Views/Controls/PatientPanelControl.xaml.cs` | The code-behind of PatientPanelControl.. |
| 53 | `src/RefLab.Presentation/Views/Controls/ResultsGridControl.xaml` | The markup of ResultsGridControl. |
| 54 | `src/RefLab.Presentation/Views/Controls/ResultsGridControl.xaml.cs` | The code-behind of ResultsGridControl.. |
| 55 | `src/RefLab.Presentation/Views/LaboratorySettingsView.xaml` | The markup of LaboratorySettingsView. |
| 56 | `src/RefLab.Presentation/Views/LaboratorySettingsView.xaml.cs` | The code-behind of LaboratorySettingsView.. |
| 57 | `src/RefLab.Presentation/Views/MicrobiologyView.xaml` | The markup of MicrobiologyView. |
| 58 | `src/RefLab.Presentation/Views/MicrobiologyView.xaml.cs` | The code-behind of MicrobiologyView.. |
| 59 | `src/RefLab.Presentation/Views/OperatorAdministrationView.xaml` | The markup of OperatorAdministrationView. |
| 60 | `src/RefLab.Presentation/Views/OperatorAdministrationView.xaml.cs` | The code-behind of OperatorAdministrationView.. |
| 61 | `src/RefLab.Presentation/Views/Panels/DerivationPanelView.xaml` | The markup of DerivationPanelView. |
| 62 | `src/RefLab.Presentation/Views/Panels/DerivationPanelView.xaml.cs` | The code-behind of DerivationPanelView.. |
| 63 | `src/RefLab.Presentation/Views/Panels/MethodSheetPanelView.xaml` | The markup of MethodSheetPanelView. |
| 64 | `src/RefLab.Presentation/Views/Panels/MethodSheetPanelView.xaml.cs` | The code-behind of MethodSheetPanelView.. |
| 65 | `src/RefLab.Presentation/Views/PartiesPricingView.xaml` | The markup of PartiesPricingView. |
| 66 | `src/RefLab.Presentation/Views/PartiesPricingView.xaml.cs` | The code-behind of PartiesPricingView.. |
| 67 | `src/RefLab.Presentation/Views/PatientWorkView.xaml` | The markup of PatientWorkView. |
| 68 | `src/RefLab.Presentation/Views/PatientWorkView.xaml.cs` | The code-behind of PatientWorkView.. |
| 69 | `src/RefLab.Presentation/Views/ReportSetupView.xaml` | The markup of ReportSetupView. |
| 70 | `src/RefLab.Presentation/Views/ReportSetupView.xaml.cs` | The code-behind of ReportSetupView.. |
| 71 | `src/RefLab.Presentation/Views/SendOutQueueView.xaml` | The markup of SendOutQueueView. |
| 72 | `src/RefLab.Presentation/Views/SendOutQueueView.xaml.cs` | The code-behind of SendOutQueueView.. |
| 73 | `src/RefLab.Presentation/Views/SignInView.xaml` | The markup of SignInView. |
| 74 | `src/RefLab.Presentation/Views/SignInView.xaml.cs` | The code-behind of SignInView.. |
| 75 | `src/RefLab.Presentation/Views/StatisticsView.xaml` | The markup of StatisticsView. |
| 76 | `src/RefLab.Presentation/Views/StatisticsView.xaml.cs` | The code-behind of StatisticsView.. |
| 77 | `src/RefLab.Presentation/Views/TestCatalogueView.xaml` | The markup of TestCatalogueView. |
| 78 | `src/RefLab.Presentation/Views/TestCatalogueView.xaml.cs` | The code-behind of TestCatalogueView.. |
| 79 | `tests/RefLab.Application.Tests/Architecture/PresentationLayerBoundaryTests.cs` | The presentation boundary guards B1 to B9 and the Phase 5 architecture contract tests. |
| 80 | `tools/verify_manifest.py` | Recomputes the SHA-256 of every file of the delivery over the extracted tree. |

`Docs/RL-P5-MAN-001-Phase5-Delivery-Manifest.md` is the manifest itself. It
carries the SHA-256 of every other file of the delivery; it cannot carry its
own hash, and its own SHA-256 is published in the delivery report.

### 4.2 Files replaced

| # | Path | What changed |
|---:|---|---|
| 1 | `src/RefLab.Presentation/App.xaml` | The startup URI is removed — the main window is resolved from the container by the composition root — and the resource dictionaries are merged. |
| 2 | `src/RefLab.Presentation/App.xaml.cs` | The application now builds the generic host, shows the main window from it and disposes the host on exit. The `System.Windows.Application` form is unchanged. |
| 3 | `src/RefLab.Presentation/MainWindow.xaml` | The Phase 1 placeholder is replaced by the shell: the module toolbar, the operator identity strip, the status strip and the navigation region. |
| 4 | `src/RefLab.Presentation/MainWindow.xaml.cs` | The window receives the shell view model from the container. |
| 5 | `src/RefLab.Infrastructure/Reports/QuestPdfExporter.cs` | One wiring call is added: the branding block is applied to every document. **This is the whole of the Infrastructure change.** |
| 6 | `Docs/ui-labels-verified.md` | Section 9 is added: the four owner-approved Arabic strings from the V2 corpus, each with its provenance. |
| 7 | `Docs/LOCAL_AGENT_IMPLEMENTATION_GUIDE.md` | This guide replaces the Phase 4 edition. |

**Unchanged on purpose:** `src/RefLab.Presentation/RefLab.Presentation.csproj`.
The presentation project already referenced CommunityToolkit.Mvvm 8.4.2 and
Microsoft.Extensions.Hosting 8.0.1, and already referenced the Application,
Domain and Infrastructure projects. No package reference was added, so no
dependency surface changed.

### 4.3 Files deleted

| # | Path | Why it is removed |
|---:|---|---|
| # | Path | Why it is removed |
|---:|---|---|
| 1 | `src/RefLab.Domain/Enums/DeliveryChannel.cs` | The SMS and NATIGH delivery channels — a permanently removed capability. |
| 2 | `src/RefLab.Domain/Auditing/DeliveryRecord.cs` | The digital-delivery audit entity of a permanently removed capability. |
| 3 | `src/RefLab.Application/Abstractions/ISmsGateway.cs` | The SMS port. No implementation, no registration, no consumer. |
| 4 | `src/RefLab.Application/Abstractions/INatighGateway.cs` | The NATIGH.COM port. No implementation, no registration, no consumer. |
| 5 | `src/RefLab.Application/Common/GatewayModels.cs` | The four SMS and NATIGH result models, consumed only by the two ports above. |
| 6 | `tests/RefLab.Domain.Tests/Auditing/DeliveryRecordTests.cs` | Three tests of the removed delivery entity. |

---

## 5. Build steps

Open `RefLab.sln` in Visual Studio 2022, restore and build. From the command
line:

```powershell
cd "C:\Users\LAP LINK\source\repos\Ref Lab"
dotnet restore RefLab.sln
dotnet build   RefLab.sln -c Debug
dotnet build   RefLab.sln -c Release
```

**Expected result: 0 errors and 0 warnings in both configurations.**

`TreatWarningsAsErrors=true` is in force for every project, so any warning
fails the build. The two warnings that a Phase 5 delivery most easily provokes
are:

- **CS0118** — an unqualified `Application` inside the presentation layer.
  See §0.
- **CS0108** — a member that hides an inherited member. The Infrastructure
  layer uses explicit interface implementations for exactly this reason; do
  not "fix" a CS0108 by suppressing it.

---

## 6. Test steps

```powershell
dotnet test RefLab.sln -c Release
```

**Expected result:**

| Suite | Tests | Result |
|---|---:|---|
| `RefLab.Domain.Tests` | 270 | passed |
| `RefLab.Application.Tests` | 226 | passed — 204 pre-existing and 22 new Phase 5 guards |
| `RefLab.Infrastructure.Tests` | 61 | passed |
| **Total** | **557** | **0 failed, 0 skipped** |

The **535 pre-existing tests** (270 + 204 + 61) must all still pass, unchanged.
The 535 figure is the baseline the Phase 4 handoff records; it is reached
again once the three obsolete `DeliveryRecordTests` are removed.

> **If the build agent has no .NET 8 runtime**, the `net8.0` test host cannot
> start. Set `DOTNET_ROLL_FORWARD=LatestMajor` for the test run. This is a
> runtime roll-forward, not a code difference, and it does not affect the
> Windows build.

The use-case drift guard must still report the expected total:

```powershell
python scripts\generate_use_case_inventory.py
```

Expected: `use cases found: 70 (expected 70)`, exit code 0.

---

## 7. Verification steps on the owner machine

### 7.1 The database must be untouched

The Phase 5 delivery changes **no** migration and **no** schema. Before the
first launch, note the migration state:

```powershell
dotnet ef migrations list --project src\RefLab.Infrastructure\RefLab.Infrastructure.csproj --startup-project src\RefLab.Infrastructure\RefLab.Infrastructure.csproj
```

`20261110103458_InitialCreate` must already be applied to `RefLabDb`, as the
Phase 4 handoff records.

> ⚠ **Do not run `dotnet ef database update`.** The application never applies
> a migration: the `DatabaseInitializer` is registered by the persistence layer
> but is never resolved, and the presentation boundary test
> `The_composition_root_never_resolves_the_database_initialiser` guards that.

### 7.2 The connection string

The application reads the SQL Server connection string from the
`REFLAB_CONNECTION_STRING` environment variable:

```powershell
$env:REFLAB_CONNECTION_STRING = "Server=.;Database=RefLabDb;Trusted_Connection=True;TrustServerCertificate=True"
```

If the variable is not set, the application refuses to start with a clear
message rather than guessing.

### 7.3 The manual checklist

| # | Check | Expected |
|---:|---|---|
| 1 | The application starts | The shell appears with the module toolbar and the sign-in screen. **No database write occurs at start-up.** |
| 2 | Sign-in | A wrong password, an unknown operator and an inactive account each produce an operator-facing message, not a crash. |
| 3 | The signed-in operator | The operator's name appears in the identity strip; the sign-out button ends the session and returns to the sign-in screen. |
| 4 | **Every screen** | All fourteen screens open from the module toolbar: patient work, test catalogue, catalogue administration, send-out, microbiology, accounting, parties and pricing, report setup, laboratory settings, operator administration, statistics, audit. |
| 5 | **The patient work screen** | The **Arabic patient panel is on the right and reads right to left**; the **English catalogue panel is on the left and reads left to right**. The two directions must differ inside the one window. |
| 6 | **F8** | Reviews the selected result. The result becomes printable and the state column shows the reviewed state. |
| 7 | **F9** | Saves the result that is being entered. |
| 8 | **F11** | Prepares the print operation: the destination and the category are confirmed. |
| 9 | **F12** | Produces the report. |
| 10 | **F7** | Moves to the send-out queue. |
| 11 | **A real print** | Choose a destination directory and press F12. The produced PDF **starts with `%PDF`**, carries the **Arabic branding** («ريال لاب سيستم», «التقرير المختبرى») and the **English branding**, and states **`Currency: EGP`**. |
| 12 | **The print audit** | The print writes a `PrintAudit` row: the sequence number, the operator and the timestamp. |
| 13 | **The classification** | The result column shows the classification the Application layer returned — it is never computed in the screen. |
| 14 | **The identifier** | The order identifier is exactly **13 characters** and is displayed as the Application layer returned it. |
| 15 | **The database** | The row counts and the schema are unchanged after the whole session. |

### 7.4 A check of the delivered PDF

```powershell
pdftotext -enc UTF-8 .\produced-file.pdf - | Select-Object -First 8
```

The output must contain the English branding line, the line `Currency: EGP`,
and the two Arabic branding lines.

> **A note on reading the Arabic back.** A PDF stores Arabic in visual order,
> so a text extractor returns the words of the Arabic branding in their visual
> order: the word «لاب» comes back as «الب». The letters are identical and the
> rendered document is correct; only the extracted ordering differs. Compare the
> letters, not the byte order.

---

## 8. Required owner actions

1. **Extract and verify** the archive (§2), and run `tools/verify_manifest.py`
   before integrating anything.
2. **Back up** the four presentation files, the exporter and the two documents
   (§3.1).
3. **Copy** the delivery over `C:\Users\LAP LINK\source\repos\Ref Lab` (§3.2).
4. **Delete the six obsolete files** of §3.3. This is the only step that is
   not a copy, and it is the step that completes the removal of the SMS and
   NATIGH capability.
5. **Set `REFLAB_CONNECTION_STRING`** (§7.2). It is not in the archive, because
   a connection string with a password must never travel in a delivery.
6. **Build** in Debug and Release and confirm 0 errors and 0 warnings (§5).
7. **Run the tests** and confirm 557 passed (§6).
8. **Run the manual checklist** (§7.3) and confirm the printed PDF carries the
   Arabic branding and EGP, and that a print-audit row was written.
9. **Confirm the database is unchanged** (§7.1).

---

## 9. What the delivery deliberately does **not** do

| | Why |
|---|---|
| It does not modify the domain, the application or the migrations. | The migration is already applied to the owner's database and is never modified. The presentation layer is the only layer Phase 5 adds. |
| It does not add a package reference. | The presentation project already had CommunityToolkit.Mvvm 8.4.2 and Microsoft.Extensions.Hosting 8.0.1. |
| It does not change `RefLab.sln`. | The boundary guards live in the existing test project, so the solution keeps its seven projects. |
| It does not migrate the print-sequence counter or the session singleton to a durable store. | Both are acknowledged open items; changing them is not a Phase 5 decision. |
| It does not reproduce the legacy barcode-template editor or the legacy per-profile report layout designer. | The owner has placed both out of scope. They are recorded in the manifest as known legacy gaps and are Phase 7 candidates. |
| It does not apply any migration. | Applying a migration is an owner-gated operation, and the application never does it. |

---

*End of the Phase 5 Local Agent Implementation Guide.*
