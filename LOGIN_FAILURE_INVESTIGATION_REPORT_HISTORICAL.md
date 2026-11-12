# RefLab — Complete Historical Login Investigation Report

**Document type:** Retrospective (historical) record of a completed login-failure investigation.
**Project:** RefLab — Medical Laboratory Management System.
**Solution root:** `C:\Users\LAP LINK\source\repos\Ref Lab`
**Solution file:** `RefLab.sln`
**Investigation performed:** 2026-11-12 (local time, UTC+02:00; UTC equivalents given where the evidence recorded UTC).
**Author of this document:** retrospective compilation from the retained historical evidence.

> **Filename note.** The repository root already contained `LOGIN_FAILURE_INVESTIGATION_REPORT.md` — the
> live investigation report written *during* the original investigation. That file was inspected
> (read-only) and **not modified**, because it is the primary historical artefact this document cites and
> overwriting it would destroy the record being reconstructed. In accordance with the deliverable rules,
> this retrospective document is therefore written to the alternative name
> **`LOGIN_FAILURE_INVESTIGATION_REPORT_HISTORICAL.md`**. Exactly one new documentation file was created.

> **Scope of this document.** This is a documentation-only deliverable. While compiling it, the RefLab
> application was **not** launched, the login window was **not** opened or interacted with, the Login button
> was **not** clicked, **no** new login attempt was performed, the solution was **not** built or tested, **no**
> migration was executed, **no** database was created, altered, attached, detached, restored or repaired, and
> **no** application source file, project file or configuration was modified. Only read-only inspection of
> retained artefacts (files, screenshots, engine error logs, prior report text) was performed.

---

## 4.1 Executive Summary

### The original problem

RefLab started normally, displayed its sign-in window, accepted an operator name and a password, and then —
when the **Login** button was pressed — displayed the generic message:

> **“The operation could not be completed.”**

(The task brief quoted this as “The operation was not completed.”. The literal string emitted by the
application is “The operation could not be completed.”, produced by the default arm of
`RequestExecutor.Describe`. The discrepancy between the reported wording and the emitted wording is
documented in §4.5 and is material, because the emitted wording is a *catch-all fallback*, not a
description of the real failure.)

### Attempts performed

| Metric | Value |
|---|---|
| **Total actual login attempts** | **2** (of a permitted maximum of 10) |
| Failed attempts | **1** — Attempt #1, 2026-11-12 12:48:50 +02:00 |
| Successful attempts | **1** — Attempt #2, 2026-11-12 12:52:35 +02:00 (10:52:35.1512877 UTC) |
| Application launches | 3 (one aborted before any Login click — see §4.4.1; it was **not** a login attempt) |

### Confirmed root cause

A **storage-layer fault, not an authentication fault**. The database the application resolves to —
`(localdb)\MSSQLLocalDB.RefLab` — still had its catalog entry, but both of its files
(`C:\Users\LAP LINK\RefLab.mdf` and `C:\Users\LAP LINK\RefLab_log.ldf`) had been deleted from disk.
Every connection therefore failed at logon time with SQL Server error **17204** (`FCB::Open failed … OS
error: 2`) followed by error **18456** (`Failed to open the explicitly specified database 'RefLab'`),
surfacing to the client as **4060** (`Cannot open database "RefLab" requested by the login`). The failure
occurred while *opening the connection* — before the operator record could be read and before the PBKDF2
password comparison — which is why it was independent of the credentials and why it reproduced on every
attempt.

### Corrective actions actually performed

1. Removed the orphaned `RefLab` catalog entry from `(localdb)\MSSQLLocalDB`.
2. Recreated the database `RefLab` with its original file layout (`C:\Users\LAP LINK\RefLab.mdf`,
   `C:\Users\LAP LINK\RefLab_log.ldf`).
3. Applied the project's own three EF Core migrations through `DatabaseInitializer` / `Database.MigrateAsync()`.
4. Restored the development administrator `developer` through the application's own domain API, using the
   password hash recorded by the development environment.

### Final outcome

**SUCCESS.** After the repair, the same administrator credentials were accepted by the real login window,
the sign-in window closed, the authenticated main window opened (`User Name : developer`, `Last Login :
Thu., 12 Nov. 2026 12:52`), the sign-in handler wrote its attendance record, and the administrator-only
*Operator administration* module loaded the operator register showing `developer | Development administrator |
True`.

### Was application source code modified?

**No.** `git status --porcelain` at the end of the investigation returned exactly one entry — the untracked
`LOGIN_FAILURE_INVESTIGATION_REPORT.md` — and `git diff --stat` was empty. The defect was not in the code:
the connection-string resolution, the authentication handler, the password hasher and the authorization
checks were all correct and were left untouched. All corrective work was performed against the **database**,
not against the source.

---

## 4.2 Investigation Scope and Environment

All values below were established during the original investigation and are quoted from its retained
records. No new environment discovery was performed while compiling this document.

### Application under investigation

| Item | Value |
|---|---|
| Application name | RefLab (Medical Laboratory Management System) |
| Technology | C# 12, .NET 8, WPF (`net8.0-windows10.0.17763.0`), MVVM, Clean Architecture, MediatR (CQRS), EF Core 8 |
| Entry point | `src/RefLab.Presentation/App.xaml.cs` → `CompositionRoot.BuildHost()` → `SignInWindow` |
| Sign-in window | `SignInWindow` hosting `Views/SignInView.xaml` (`src/RefLab.Presentation/`) |
| Build configuration used | `Debug` (`dotnet build RefLab.sln -c Debug`) |

### Database environment

| Item | Value |
|---|---|
| Database engine | Microsoft SQL Server Express / LocalDB |
| Instance resolved by the application | `(localdb)\MSSQLLocalDB` |
| Instance version (as reported by `SqlLocalDB info`) | `17.0.1000.7`, owner `DESKELTEL\LAP LINK`, `Auto-create: Yes` |
| Database name | `RefLab` |
| Database file path recorded in the LocalDB catalog | `C:\Users\LAP LINK\RefLab.mdf` (log file `C:\Users\LAP LINK\RefLab_log.ldf`) |
| State of that file path **before** the repair | **Files absent from disk** (catalog entry present, files deleted) |
| State of that file path **after** the repair | Files present: `RefLab.mdf` and `RefLab_log.ldf`, 8 388 608 bytes each, creation time 2026-11-12 12:51:09 |
| Other LocalDB instances present (not used by the application) | `OpenLabUITests`, `ProjectModels` — both contained only the four system databases |

### Connection configuration and fallback behaviour (confirmed)

Source: `src/RefLab.Presentation/Bootstrap/CompositionRoot.cs`.

| Item | Value |
|---|---|
| Environment variable | `REFLAB_CONNECTION_STRING` — **unset** (confirmed during the investigation; it was deliberately left unset afterwards) |
| Resolution order | explicit argument → environment variable → hard-coded fallback |
| Fallback string (verbatim from source, lines 77–78) | `"Server=(localdb)\\mssqllocaldb;Database=RefLab;Trusted_Connection=True;TrustServerCertificate=True"` |
| Conclusion recorded at the time | The unset environment variable was **not** the defect; the fallback resolved to the intended LocalDB `RefLab` database, and it was that database whose files were missing. |
| Design-time factory | `RefLabDbContextFactory` reads the same variable and uses the same fallback for `dotnet ef` tooling. |

### Authentication and data components involved

| Layer | Component | Role in the failure / repair |
|---|---|---|
| Presentation | `ViewModels/SignInViewModel.cs` | `SignInAsync` → `RequestExecutor.SendAsync(new SignInCommand(OperatorName, Password))`; on success calls `OperatorSession.Establish`, `ShellViewModel.OnSessionEstablished()` and `INavigationService.NavigateTo(ScreenKeys.PatientWork)`. |
| Presentation | `Common/RequestExecutor.cs` | Translates exceptions into operator-visible text. `SendAsync` translates only `RequestValidationException` and `DomainException`; `Describe` has no case for data-access failures. |
| Presentation | `Common/ViewModelBase.cs` | `RunAsync` catches **every** exception and stores `RequestExecutor.Describe(exception)` as `ErrorMessage`. |
| Application | `Features/Identity/SignIn.cs` | `SignInHandler.Handle` — operator lookup by upper-cased name → `IPasswordHasher.Verify` → `IsActive` check → attendance start/last-visit → `SaveChangesAsync`. |
| Application | `Features/Identity/GetOperators.cs`, `CreateOperator.cs` | Operator-register read and creation used by the Operator administration screen. |
| Application | `Behaviors/*` (Validation, Authorization, DomainException, Logging) | MediatR pipeline behaviours; the authorization behaviour checks the session principal's permissions. |
| Domain | `Domain/Identity/Operator.cs` | `Operator.Create`, `SetAdministrator`, `SetActive`, `Grant`, `Has`, `RecordAttendanceStart`, `RecordLastVisit`; `Has(p) => IsAdministrator \|\| _permissions.Contains(p)`. |
| Domain | `Domain/Identity/PasswordPolicy.cs` | Password strength rules used by the operator-creation / change-password use cases. |
| Infrastructure | `Security/Pbkdf2PasswordHasher.cs` | PBKDF2-HMAC-SHA256, 210 000 iterations, 16-byte salt, 32-byte hash, stored as `pbkdf2$sha256$<iterations>$<salt>$<hash>`. |
| Infrastructure | `Persistence/RefLabDbContext.cs` | EF Core 8 context; `public DbSet<Operator> Operators => Set<Operator>();` (line 45). |
| Infrastructure | `Persistence/DatabaseInitializer.cs` | `MigrateAsync()` → `GetPendingMigrationsAsync()` + `Database.MigrateAsync()` — the operator tool used during the repair. |
| Infrastructure | `Persistence/Migrations/*` | Three migrations: `20261110103458_InitialCreate`, `20261111083113_AddReportFooterAndLayoutText`, `20261111122338_AddReportBrandingAndSampleOrigin`. |
| Infrastructure | `Session/SessionCurrentUser.cs`, `Presentation/Common/OperatorSession.cs` | Desktop session principal and current-user abstraction. |
| Presentation | `Views/SignInView.xaml` | Operator-name `TextBox` (`AutomationProperties.Name` = “Operator name”), masked `PasswordBox` (`x:Name="MaskedPasswordBox"`), reveal toggle, `ErrorMessage` `TextBlock`, and the **Login** `Button` bound to `SignInCommand`. |

---

## 4.3 Previous Investigation Timeline

All timestamps are local time (UTC+02:00) unless the evidence recorded UTC explicitly. Entries marked
*(exact timestamp unavailable)* are ordered by their recorded sequence only.

| # | Time | Activity | Evidence anchor |
|---|---|---|---|
| 1 | 2026-11-11 14:18:57 | Development environment wrote the operator password hash to `C:\Users\LAP LINK\AppData\Local\Temp\opencode\devhash.txt`. | File timestamp + content |
| 2 | 2026-11-11 14:19:57 – 14:23:43 | Development environment created and edited `…\Temp\opencode\verifylogin\Program.cs`, a harness that signs in as `developer` / `DevAdmin#2026!RefLab` against the LocalDB `RefLab` database. | File timestamps + content |
| 3 | 2026-11-11 17:01:56, 17:49:18, 17:58:17 | LocalDB instance started the `RefLab` database successfully (`Starting up database 'RefLab'`, `Parallel redo is started/shutdown`) — the database was healthy on the evening before the failure. | `error6.log`, `error4.log`, `error3.log` (Nov 11 entries) |
| 4 | 2026-11-11 (earlier session, exact time unavailable) | An earlier development session (model id `deepseek-v4.1-flash-free`) tested candidate passwords against the *other* installation's `admin` hash with a PowerShell PBKDF2 loop; the recorded output was `segments=5 iterations=210000` followed by `done` and **no** `MATCH:` line. | Retained session store `~/.local/share/opencode/opencode.db` |
| 5 | 2026-11-12 11:45:49 – 11:49:07 | Repeated connection failures to `RefLab` recorded by the engine (17204 + 18456). These precede the investigation session and correspond to the reported reproduction of the symptom; the acting party is **not available in the retained historical evidence**. | `error5.log` |
| 6 | 2026-11-12 11:55:16 – 11:57:43 | Further repeated 17204 + 18456 failures for `C:\Users\LAP LINK\RefLab.mdf`. | `error4.log` |
| 7 | 2026-11-12 11:58 – 12:26 (exact minute-level timestamps unavailable) | Read-only solution inspection: entry point, composition root, connection-string resolution, sign-in view model, request executor, sign-in handler, password hasher, EF context and migrations, sign-in XAML, domain `Operator`, operator-administration screen, and the integration-test fixtures (`SyntheticData.OperatorPassword`, `IntegrationTestHostFixture`). | Source files listed in §4.2 |
| 8 | same window | Credential archaeology: `devhash.txt` read and its hash re-derived offline with PBKDF2 to confirm the password; the `verifylogin` / `RefLabLoginE2E` harnesses and the `build.log` / `test.log` of 2026-11-11 inspected; the retained session store searched for the seeding session. | Artefacts listed in §4.7 |
| 9 | same window | Database inventory: LocalDB instance list, `sys.databases`, `sys.master_files`, `Test-Path "C:\Users\LAP LINK\RefLab.mdf"` → `False`, Recycle Bin search (no `RefLab` entry), shadow-copy check (not permitted), and the `.\SQLEXPRESS` inventory. | §4.5, §4.11 |
| 10 | 12:26:37.33 / .42 | Two failed connections to `(localdb)\MSSQLLocalDB.RefLab` during the investigation's own connectivity check via `sqlcmd -S "(localdb)\MSSQLLocalDB" -E -d RefLab` → `Login failed for user 'DESKELTEL\LAP LINK'. Cannot open database "RefLab" requested by the login.` | `error3.log` + command output |
| 11 | 12:26–12:45 | Candidate-password sweep against the `.\SQLEXPRESS\RefLab` operator `admin` hash. The first sweep (72 candidates) completed with no match. A second, larger generated sweep was started as a background job and terminated with the shell error `Shell.NotFoundError` before producing any output — **its result is not available in the retained historical evidence.** | Command transcript |
| 12 | **12:45:51** | The investigation report `LOGIN_FAILURE_INVESTIGATION_REPORT.md` was created in the solution root (header: *Start timestamp 2026-11-12 12:45:51 +02:00*), before any login attempt. | File header + creation |
| 13 | 12:46:13 – 12:48:37 | `dotnet build RefLab.sln -c Debug` → **Build succeeded. 0 Warning(s), 0 Error(s). Time Elapsed 00:01:06.17.** | Build output |
| 14 | 12:48:17 | UI-automation driver `reflab_login_attempt.ps1` created in `%TEMP%\opencode` (launches the real WPF executable, types into the real fields, invokes the real **Login** button, captures screenshots). | File timestamp |
| 15 | 12:48:2x | **Application launch A** (PID 14456): window `Ref Lab - Sign in` found, operator name typed — the driver then aborted with `Cannot convert value "128065" to type "System.Char"` while comparing the reveal-toggle glyph (`[char]0x1F441`). **No password was entered and the Login button was never clicked; no authentication request was issued.** The process was subsequently terminated. | Driver output + engine log (no RefLab entry at this time) |
| 16 | 12:48:31 | `attempt1.png` written (sign-in window populated, before the click). | File timestamp + image |
| 17 | **12:48:50.44** | **LOGIN ATTEMPT #1** — application launch B (PID 15316): operator name `developer`, password `DevAdmin#2026!RefLab`, **Login clicked**. Engine recorded `17204` + `18456` for `C:\Users\LAP LINK\RefLab.mdf`. The application displayed “The operation could not be completed.” | `error1.log` + `attempt1_after.png` |
| 18 | 12:49:23 | Read-only diagnostic program `reflabdiag` created (query only, no password verification) to obtain the raw exception and stack trace for the failing operator lookup. | Source of `reflabdiag\Program.cs` |
| 19 | 12:49:51.76 and 12:50:01.81 | Two further failed connections recorded by the engine — the diagnostic's connection attempts. | `error1.log` |
| 20 | 12:49–12:50 | Diagnostic produced the full exception: `System.InvalidOperationException` wrapping `Microsoft.Data.SqlClient.SqlException`, **Error Number 4060**, with the stack trace through `SqlConnection.Open()` → `RelationalConnection.Open` → `SqlServerExecutionStrategy.Execute` → `SingleQueryingEnumerable.Enumerator.InitializeReader`. | Diagnostic output |
| 21 | 12:50:47.90 – 12:50:47.92 | Engine events while removing the orphaned entry: `Starting up database 'RefLab'` → `17204 … for file number 1` → `17207 FileMgr::StartLogFiles: Operating system error 2 … 'C:\Users\LAP LINK\RefLab_log.ldf'`. | `error1.log` |
| 22 | 12:51:09 | The orphaned catalog entry was removed and the database recreated with its original file layout; the recreated files carry a creation timestamp of 12:51:09. Engine: `Starting up database 'RefLab'` + parallel redo. | `error1.log` + file timestamps |
| 23 | 12:51:44 | Repair program `reflabrestore` created (migrations → domain-API seeding of the administrator → verification). | Source of `reflabrestore\Program.cs` |
| 24 | 12:51:54 – 12:52:11 | Engine `Starting up database 'RefLab'` events with successful parallel redo — the recreated database was brought online and the migrations were applied. | `error1.log` |
| 25 | 12:52:11 – 12:52:35 | `reflabrestore` executed: 3 migrations applied, 28 base tables, operator `developer` inserted with the recorded hash; verification printed `IsAdministrator : True`, `IsActive : True`, hash verify `True` / wrong password `False`, administrator holds every permission `True`. | Program output |
| 26 | 12:52:27 | `attempt2.png` written. | File timestamp |
| 27 | **12:52:33.13 / 12:52:35.1512877 UTC** | **LOGIN ATTEMPT #2** — application launch C (PID 2604): the real sign-in window was driven again with `developer` / `DevAdmin#2026!RefLab` and **Login** was clicked. Engine: `Starting up database 'RefLab'` at 12:52:33.13 (the database opened normally). The handler wrote the attendance row at **2026-11-12 10:52:35.1512877 +00:00**. The sign-in window closed and the authenticated main window opened. | `error1.log` + `Operators`/`AttendanceEntries` rows + `window1.png` |
| 28 | 12:53:29 / 12:53:35 | Window inspector `reflab_windows.ps1` used to enumerate the process's top-level windows and capture `window1.png` (authenticated main window, 1360×860). | File timestamps + image |
| 29 | 12:53:51 / 12:54:03 | Module driver `reflab_click_module.ps1` invoked **Operator administration**; `attempt2_operator_admin.png` captured the administrator-only screen with the permission list populated and Add/Edit/Apply/Save enabled. | File timestamps + image |
| 30 | 12:54:35 | **Refresh** invoked on that screen; `attempt2_operator_refresh.png` captured the operator register row `developer | Development administrator | True` — read live from the restored database by the running application. | Image |
| 31 | *(exact start/end unavailable)* | `dotnet test RefLab.sln -c Debug` executed → **738 passed, 0 failed** (Domain 317, Application 277, Infrastructure 65, Integration 63, Presentation 16). | Test output |
| 32 | 12:57:15 | The live investigation report was finalised (last write). | File timestamp |
| 33 | after 12:57:15 | The application process (PID 2604) was left running with the authenticated main window displayed. Its subsequent termination state is **not available in the retained historical evidence**. | — |

---

## 4.4 Detailed Login Attempt History

### 4.4.1 Preparatory run that was **not** a login attempt (recorded for completeness)

| Item | Value |
|---|---|
| Time | 2026-11-12, shortly after 12:48:17 (exact second not available in the retained historical evidence) |
| Through the real login window? | **Yes** — the real `RefLab.Presentation.exe` was launched (PID 14456) and the real `Ref Lab - Sign in` window was found through UI Automation. |
| Was the Login button clicked? | **No.** |
| Account identity used | `developer` was typed into the operator-name field; **no password was entered**. |
| Outcome | The automation driver aborted with `Cannot convert value "128065" to type "System.Char"` while comparing the password-reveal button's glyph against `[char]0x1F441`. The script was corrected to `[System.Char]::ConvertFromUtf32(0x1F441)` and the process was terminated. |
| Was any authentication request issued? | **No.** The application never sent anything to the Application layer; the LocalDB engine log contains **no** `RefLab` entry at this time. |
| Classification | **Not a login attempt.** Counted only to make the attempt accounting auditable. |

### 4.4.2 Attempt #1 — FAILURE

| Item | Value |
|---|---|
| **Attempt number** | **#1** |
| **Date and time** | **2026-11-12 12:48:50 +02:00** (engine log time, second resolution). The click itself was issued by the driver at approximately 12:48:50; the driver captured the post-click screenshot `attempt1_after.png` with a file timestamp of **12:48:50**. |
| **Through the real application login window?** | **Yes.** `RefLab.Presentation.exe` (PID 15316), the real `Ref Lab - Sign in` window, the real operator-name `TextBox`, the real masked `PasswordBox` (revealed via the real toggle button) and the real **Login** `Button` (bound to `SignInCommand`). |
| **Was the Login button actually clicked?** | **Yes** — invoked through `InvokePattern` on the real button. |
| **Account identity used** | Username `developer`, password `DevAdmin#2026!RefLab`. Account type: Administrator; `IsAdministrator = 1`; `IsActive = 1` (values carried from the recorded development credential — the live row could **not** be read before this attempt because the database could not be opened). |
| **Outcome** | **FAILURE** |
| **Exact message displayed by the application** | **“The operation could not be completed.”** — rendered in red under the password field (screenshot `attempt1_after.png`). No other message was displayed. |
| **Underlying exception / error** | `System.InvalidOperationException: An exception has been raised that is likely due to a transient failure. Consider enabling transient error resiliency by adding 'EnableRetryOnFailure' to the 'UseSqlServer' call.` → inner `Microsoft.Data.SqlClient.SqlException (0x80131904): Cannot open database "RefLab" requested by the login. The login failed. / Login failed for user 'DESKELTEL\LAP LINK'.` with **`Error Number: 4060, State: 1, Class: 11`**. Server-side: **17204** (`FCB::Open failed … OS error: 2`) and **18456** (`Login failed … Failed to open the explicitly specified database 'RefLab'`). |
| **Technical stage at which it failed** | **Connection opening**, inside the operator-lookup query of `SignInHandler.Handle` (`src/RefLab.Application/Features/Identity/SignIn.cs`, lines 50–51). Stack: `SqlConnection.Open()` → `SqlServerConnection.OpenDbConnection` → `RelationalConnection.Open` → `RelationalCommand.ExecuteReader` → `SingleQueryingEnumerable.Enumerator.InitializeReader` → `SqlServerExecutionStrategy.Execute`. **The operator record was never retrieved and the password was never verified.** |
| **Investigation performed after the attempt** | (a) Read the LocalDB instance error logs for the exact timestamp; (b) re-ran the connectivity check with `sqlcmd` and confirmed the same 18456/4060 failure; (c) ran the read-only `reflabdiag` program to obtain the raw exception, inner exception and full stack trace for the failing query; (d) reviewed `RequestExecutor.SendAsync` / `RequestExecutor.Describe` / `ViewModelBase.RunAsync` to establish why the message shown was the generic fallback; (e) reviewed `SignInHandler` to establish that the failure preceded the password check. |
| **Confirmed root cause** | **The `RefLab` database files were missing from disk** (`C:\Users\LAP LINK\RefLab.mdf`, `C:\Users\LAP LINK\RefLab_log.ldf`), while the LocalDB catalog still listed the database at those paths. Every connection therefore failed at logon. The generic message was a *presentation-layer consequence*, not a second cause. |
| **Corrective action performed afterwards** | See §4.6 (four actions: catalog-entry removal, database recreation with the original file layout, migration application, administrator restoration). |
| **Evidence supporting the recorded outcome** | `attempt1_after.png` (message on screen); `error1.log` lines at `12:48:50.44`; `reflabdiag` output (exception + stack); `sqlcmd` output `Login failed for user 'DESKELTEL\LAP LINK'. Cannot open database "RefLab" requested by the login. The login failed.` |

### 4.4.3 Attempt #2 — SUCCESS

| Item | Value |
|---|---|
| **Attempt number** | **#2** |
| **Date and time** | **2026-11-12 12:52:35 +02:00**, recorded by the application itself as `2026-11-12 10:52:35.1512877 +00:00` in the `AttendanceEntries` row and in `Operators.AttendanceStart` / `Operators.LastVisit`. The engine opened the database at `12:52:33.13`. |
| **Through the real application login window?** | **Yes.** `RefLab.Presentation.exe` (PID 2604), the real `Ref Lab - Sign in` window, the real fields, the real **Login** button. |
| **Was the Login button actually clicked?** | **Yes** — invoked through `InvokePattern`. |
| **Account identity used** | Username `developer`, password `DevAdmin#2026!RefLab`. Immediately before the attempt the live row was re-read and confirmed: `Id = 1`, `Name = developer`, `IsAdministrator = 1`, `IsActive = 1`, 15 permissions, hash `pbkdf2$sha256$210000$xBjiujggPfgElm3CXU+Dlw==$KhELbHqzOmWz3HIHNZKtCF6jBCkY9009BQE22lgrtgI=`. |
| **Outcome** | **SUCCESS** |
| **Exact message displayed by the application** | **None.** No error was rendered; the `ErrorMessage` text block remained empty. The sign-in window closed. |
| **Underlying exception / error** | **None.** The LocalDB engine log contains no `17204`/`18456`/`4060` entry for this attempt; the only `RefLab` events are `Starting up database 'RefLab'` and the parallel-redo pair. |
| **Technical stage** | Completed the full path: operator lookup → PBKDF2 verification → active check → attendance start + last visit → `SaveChangesAsync` → session establishment → navigation to `ScreenKeys.PatientWork` → `MainWindow` creation and display. |
| **Investigation performed after the attempt** | Window enumeration (top-level windows of the process), capture of the authenticated main window, invocation of the administrator-only **Operator administration** module, and a **Refresh** on that screen to prove that the running application reads the restored `Operators` table. |
| **Confirmed root cause** | Not applicable — no failure occurred. The root cause of Attempt #1 (missing database files) was confirmed fixed by this attempt. |
| **Corrective action performed** | None beyond the §4.6 actions already applied before the attempt. |
| **Evidence supporting the recorded outcome** | `window1.png` (authenticated main window, status bar `User Name : developer · Last Login : Thu., 12 Nov. 2026 12:52 · SQL DATABASE · Today : Thu., 12-Nov.-2026 12:52`); `attempt2_after.png`; `attempt2_operator_admin.png`; `attempt2_operator_refresh.png`; the `AttendanceEntries` row and the updated `Operators` row; `error1.log` showing a normal database start with no error. |

---

## 4.5 Root Cause Analysis

Only **one** failure occurred (Attempt #1), so only one root cause had to be established. The historical
evidence supports a single causal chain; the analyses below distinguish observation from interpretation.

### 4.5.1 The orphaned catalog entry and the missing MDF file

**Observation.** `sys.master_files` on `(localdb)\MSSQLLocalDB` mapped the database `RefLab` to
`C:\Users\LAP LINK\RefLab.mdf` (ROWS) and `C:\Users\LAP LINK\RefLab_log.ldf` (LOG). `sys.databases`
reported the database as present (`state_desc = ONLINE`, `user_access_desc = MULTI_USER`). The files
themselves were absent:

* `Test-Path "C:\Users\LAP LINK\RefLab.mdf"` → `False`; `Test-Path "C:\Users\LAP LINK\RefLab_log.ldf"` → `False`.
* A directory listing of `C:\Users\LAP LINK\*.mdf` returned only `AmanaLab.mdf`, `EasyLabDb_V2.mdf` and
  `MedoLabDB.mdf` (8 388 608 bytes each) — none of them RefLab.
* A Recycle Bin scan for `*RefLab*` found no deleted `RefLab` file (the only recycle-bin entry newer than
  2026-11-01 was `$ICK95YX.zip`).
* A search for `RefLab*.mdf` under the user profile, `F:\REAL_LAB_COPY` and the SQL Server program folder
  returned nothing.
* A shadow-copy listing could not be produced (`vssadmin list shadows` → `You don't have the correct
  permissions to run this command`), so the existence of a shadow copy is **not available in the retained
  historical evidence**.

**Interpretation (confirmed by the engine log).** The catalog entry survived the deletion of its files. The
database therefore remained listed but physically unopenable — an orphaned catalog entry pointing at a
non-existent MDF.

### 4.5.2 SQL Server error 17204 / OS error 2

**Observation** — verbatim from the LocalDB instance error log (`error5.log`, `error4.log`, `error3.log`,
`error1.log`):

```
2026-11-12 12:48:50.44 spid51      Error: 17204, Severity: 16, State: 1.
2026-11-12 12:48:50.44 spid51      FCB::Open failed: Could not open file C:\Users\LAP LINK\RefLab.mdf for file number 0.  OS error: 2(The system cannot find the file specified.).
```

A second variant, produced during the removal of the orphaned entry, names the **log** file and error
**17207**:

```
2026-11-12 12:50:47.91 spid61      Error: 17204, Severity: 16, State: 1.
2026-11-12 12:50:47.91 spid61      FCB::Open failed: Could not open file C:\Users\LAP LINK\RefLab.mdf for file number 1.  OS error: 2(The system cannot find the file specified.).
2026-11-12 12:50:47.92 spid61      Error: 17207, Severity: 16, State: 1.
2026-11-12 12:50:47.92 spid61      FileMgr::StartLogFiles: Operating system error 2(The system cannot find the file specified.) occurred while creating or opening file 'C:\Users\LAP LINK\RefLab_log.ldf'. Diagnose and correct the operating system error, and retry the operation.
```

**Interpretation.** `file number 0` is the data file, `file number 1` the log file. Both were missing. The
engine could not attach either file, so the database could not be brought online.

### 4.5.3 SQL Server error 18456

**Observation** — same log, immediately after each 17204:

```
2026-11-12 12:48:50.44 Logon       Error: 18456, Severity: 14, State: 38.
2026-11-12 12:48:50.44 Logon       Login failed for user 'DESKELTEL\LAP LINK'. Reason: Failed to open the explicitly specified database 'RefLab'. [CLIENT: <named pipe>]
```

**Interpretation.** The Windows-authenticated logon itself succeeded; the *database* specified in the
connection string could not be opened, and SQL Server therefore rejected the logon with reason
*“Failed to open the explicitly specified database”*. The same pair of errors repeats at 11:45–11:49,
11:55–11:57, 12:26:37, 12:48:50, 12:49:51 and 12:50:01 — i.e. for every connection attempt made against
the LocalDB `RefLab` database, which is consistent with the symptom being reproducible on every login.

### 4.5.4 SQL exception 4060 at the client

**Observation** — from the read-only diagnostic `reflabdiag`, which reproduced the sign-in handler's
operator lookup against the resolved connection string:

```
=== EXCEPTION ===
Type      : System.InvalidOperationException
Message   : An exception has been raised that is likely due to a transient failure. Consider enabling transient error resiliency by adding 'EnableRetryOnFailure' to the 'UseSqlServer' call.
Inner[1]   : Microsoft.Data.SqlClient.SqlException: Cannot open database "RefLab" requested by the login. The login failed.
Login failed for user 'DESKELTEL\LAP LINK'.
Error Number:4060,State:1,Class:11
```

**Stack trace (as recorded, application-relevant frames):**

```
System.InvalidOperationException: An exception has been raised that is likely due to a transient failure. ...
 ---> Microsoft.Data.SqlClient.SqlException (0x80131904): Cannot open database "RefLab" requested by the login. The login failed.
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.CompleteLogin(Boolean enlistOK)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.AttemptOneLogin(...)
   at Microsoft.Data.SqlClient.SqlConnection.Open()
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerConnection.OpenDbConnection(Boolean errorsExpected)
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.Open(Boolean errorsExpected)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReader(RelationalCommandParameterObject parameterObject)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.Enumerator.InitializeReader(Enumerator enumerator)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.Execute[TState,TResult](TState state, Func`3 operation, Func`3 verifySucceeded)
   --- End of inner exception stack trace ---
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.Execute[TState,TResult](...)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.Enumerator.MoveNext()
   at System.Linq.Enumerable.TryGetSingle[TSource](IEnumerable`1 source, Boolean& found)
   at lambda_method7(Closure, QueryContext)
   at Microsoft.EntityFrameworkCore.Query.Internal.QueryCompiler.Execute[TResult](Expression expression)
   at Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider.Execute[TResult](Expression expression)
```

**Interpretation.** `4060` is the client-side projection of the server's 18456/17204 pair. The outer
`InvalidOperationException` is produced by EF Core's `SqlServerExecutionStrategy`, which wraps exceptions
that *look* transient; the message is therefore misleading about the nature of the fault (it is not a
transient/network problem — the file is permanently absent).

### 4.5.5 The application-level exception and where it occurred

**Observation (code).** `RequestExecutor.SendAsync` translates exactly two exception types:

```csharp
try { return await _mediator.Send(request, cancellationToken).ConfigureAwait(false); }
catch (RequestValidationException exception) { throw new UiValidationException(exception.Errors); }
catch (DomainException exception) { throw new UiOperationException(exception.Message, exception); }
```

A `SqlException`/`InvalidOperationException` is neither, so it propagates unchanged to
`ViewModelBase.RunAsync`:

```csharp
catch (Exception exception)
{
    ErrorMessage = RequestExecutor.Describe(exception);
    return false;
}
```

**Observation (code).** `RequestExecutor.Describe` has no case for a data-access failure, so the default arm
is selected:

```csharp
_ => "The operation could not be completed.",
```

**Interpretation.** The application-level exception was raised inside `SignInHandler.Handle` at the very
first statement that touches the database — the operator lookup — and it surfaced as the generic fallback
string. The operator-visible message was therefore a *translation artefact*, not a description of the fault.

### 4.5.6 Why the failure preceded the operator record and the password check

**Observation (code).** `SignInHandler.Handle` performs, in order: (1) the operator lookup
`_db.Set<Operator>().FirstOrDefault(candidate => candidate.Name.ToUpper() == comparisonName)`; (2)
`_passwordHasher.Verify(request.Password, @operator.PasswordHash)`; (3) the `IsActive` check; (4) the
attendance write and `SaveChangesAsync`.

**Observation (stack trace).** The exception was raised inside `RelationalConnection.Open` /
`RelationalCommand.ExecuteReader` — i.e. while executing statement (1).

**Interpretation.** Statements (1)–(4) never executed. Because the connection could not be opened, the
`Operators` row was never read, and therefore:

* the password was never compared against any hash;
* the `IsActive` flag was never evaluated;
* no attendance row could be written.

This is the reason the failure was completely independent of the credentials and reproduced on every
attempt, and the reason the recorded password verification (`Verify("DevAdmin#2026!RefLab", …) = True`)
could only be performed after the database was restored.

### 4.5.7 Why the generic message concealed the underlying failure

Summarising the chain established above:

1. The storage layer failed to attach the database's files → 17204 / 17207 / 18456.
2. The provider surfaced 4060; EF Core's execution strategy re-wrapped it as `InvalidOperationException`
   with a "transient failure" hint that does not apply here.
3. The Application layer's exception hierarchy does not cover infrastructure failures, so
   `RequestExecutor.SendAsync` did not translate it.
4. `RequestExecutor.Describe` had no matching case, so the default arm produced
   “The operation could not be completed.”
5. `ViewModelBase.RunAsync` stored that string as `ErrorMessage` and rendered it, while the original
   exception — including its 4060 code and the entire stack trace — was discarded with no log sink.

**Conclusion for this section.** The single confirmed root cause of the failed login was the deletion of
the `RefLab` database files from disk, leaving an orphaned catalog entry that made every connection fail
at logon. The generic on-screen message was a secondary *observability* weakness of the presentation layer
— it did not cause the failure, but it removed the only clue an operator would have had. The
investigation recorded this as an observation and **deliberately did not change it**, because the minimal
correction for the actual defect was a database restoration and modifying the exception-translation
contract of the presentation layer was outside that scope.

---

## 4.6 Previous Corrective Actions

Each action below was actually executed during the original investigation, after the root cause had been
documented. This section is a record; nothing was repeated while compiling this document.

### Action 1 — Removal of the orphaned database catalog entry

1. **What was changed.** The `RefLab` database was dropped from `(localdb)\MSSQLLocalDB`, removing the
   catalog entry that pointed at the deleted files. The first `DROP DATABASE` attempt failed with
   `Msg 5120 … Unable to open the physical file "C:\Users\LAP LINK\RefLab.mdf". Operating system error 2 …
   File activation failure. The physical file name "C:\Users\LAP LINK\RefLab_log.ldf" may be incorrect.`
   An `ALTER DATABASE … SET EMERGENCY ON` was rejected with `Msg 102 … Incorrect syntax near 'EMERGENCY'`
   and `ALTER DATABASE … SET SINGLE_USER WITH ROLLBACK IMMEDIATE` with `Msg 5011 … User does not have
   permission to alter database 'RefLab' …`; the subsequent `DROP DATABASE` reported `Msg 3701 … Cannot
   drop the database 'RefLab', because it does not exist or you do not have permission.` A subsequent
   `SELECT name FROM sys.databases WHERE name='RefLab'` and a matching query against `sys.master_files`
   both returned **no rows**, establishing that the entry had in fact been removed.
2. **Why it was necessary.** The orphaned entry made every connection to the intended database fail with
   4060/17204; it also blocked the creation of a correctly configured replacement.
3. **Evidence of completion.** `sys.databases` → 0 rows for `RefLab`; `sys.master_files` → 0 rows matching
   `%RefLab%`; the file `C:\Users\LAP LINK\RefLab.mdf` still `False` (nothing existed to lose).
4. **Nature of the change.** **Database catalog metadata** only. No application source code, no
   configuration, no connection string.

### Action 2 — Recreation of the `RefLab` database with its original MDF/LDF layout

1. **What was changed.** The database was recreated with an explicit file layout:

   ```sql
   CREATE DATABASE [RefLab]
   ON PRIMARY (NAME = N'RefLab',        FILENAME = N'C:\Users\LAP LINK\RefLab.mdf')
   LOG ON     (NAME = N'RefLab_log',    FILENAME = N'C:\Users\LAP LINK\RefLab_log.ldf');
   ```

2. **Why it was necessary.** The application resolves the database by name on `(localdb)\MSSQLLocalDB`; the
   database had to exist again, and it had to live at the paths the LocalDB catalog had already recorded
   for it, so that the installation matched the documented configuration
   (`Database file: C:\Users\LAP LINK\RefLab.mdf`).
3. **Evidence of completion.** `sys.databases` → `RefLab`, `state_desc = ONLINE`; a file listing showed
   `RefLab.mdf` and `RefLab_log.ldf` (8 388 608 bytes each) with a creation timestamp of
   **2026-11-12 12:51:09**; the engine log recorded `Starting up database 'RefLab'` followed by a
   successful parallel-redo pair.
4. **Nature of the change.** **Database structure** (a new empty database and its files). No application
   source code, no configuration, no connection string.

### Action 3 — Execution of the previously applied database migrations

1. **What was changed.** The project's own migrations were applied through the application's own
   initialiser — `DatabaseInitializer.MigrateAsync()` → `GetPendingMigrationsAsync()` +
   `Database.MigrateAsync()` — executed by the repair program `reflabrestore`:

   ```
   --- 1. migrations ---
   migrations applied by this run : 3
     applied: 20261110103458_InitialCreate
     applied: 20261111083113_AddReportFooterAndLayoutText
     applied: 20261111122338_AddReportBrandingAndSampleOrigin
   tables in the restored schema  : 44
   ```

   Independently confirmed by `sqlcmd` against the restored database:

   ```
   MigrationId
   -----------
   20261110103458_InitialCreate
   20261111083113_AddReportFooterAndLayoutText
   20261111122338_AddReportBrandingAndSampleOrigin
   (3 rows affected)
   ```

   and

   ```
   tables
   ------
   28
   (1 row affected)
   ```

2. **Why it was necessary.** The schema of `RefLab` is created exclusively by the reviewed migrations; the
   project forbids hand-written schemas and forbids modifying existing migrations. Applying them through
   `DatabaseInitializer` was therefore the only sanctioned way to rebuild the schema.
3. **Evidence of completion.** The two outputs above agree on the three migration identifiers; the
   migration-history table `__EFMigrationsHistory` contains exactly those three rows.
4. **Nature of the change.** **Database structure.** No migration file was created, edited or deleted — the
   migration sources in `src/RefLab.Infrastructure/Persistence/Migrations/` were read but never written.

**Number reconciliation (documented discrepancy).** Three different counts appear in the retained evidence
and they measure different things: *44* is `db.Model.GetEntityTypes().Count()` (EF Core entity types,
including owned types and join entities); *28* is the number of base tables reported by
`INFORMATION_SCHEMA.TABLES`; *27* is the count of mapped domain tables stated in the project README and in
the solution map. 28 = 27 domain tables + `__EFMigrationsHistory`. The same 28-table inventory was
independently observed in the sibling installation `.\SQLEXPRESS\RefLab`, confirming schema parity.

### Action 4 — Restoration of the Administrator account through the application's domain API

1. **What was changed.** One row was inserted into `Operators` of the restored database, using the
   application's own domain API — `Operator.Create(1, "developer", RecordedHash, "Development
   administrator")`, then `SetAdministrator(true)`, `SetActive(true)` and `Grant(...)` for every
   `Permission` enum value — and saved through the EF Core unit of work. The stored hash was the hash
   **recorded by the development environment** (`devhash.txt`), not a newly generated one. The recorded
   program output:

   ```
   --- 2. restore the development administrator ---
   operator 'developer' restored with the recorded hash

   --- 3. verification ---
   Name            : developer
   IsAdministrator : True
   IsActive        : True
   RoleCaption     : Development administrator
   Permissions     : 15
   PasswordHash    : pbkdf2$sha256$210000$xBjiujggPfgElm3CXU+Dlw==$KhELbHqzOmWz3HIHNZKtCF6jBCkY9009BQE22lgrtgI=
   recorded hash verifies 'DevAdmin#2026!RefLab' : True
   recorded hash verifies 'wrong-password'      : False
   the administrator holds every permission      : True
   ```

   Independently confirmed by `sqlcmd` against the restored database:

   ```
   Id|Name|RoleCaption|IsAdministrator|IsActive|PasswordHash|Permissions
   1|developer|Development administrator|1|1|pbkdf2$sha256$210000$xBjiujggPfgElm3CXU+Dlw==$KhELbHqzOmWz3HIHNZKtCF6jBCkY9009BQE22lgrtgI=|RegisterPatient,EditPatient,SearchPatients,OrderTests,EnterResults,ReviewResults,PrintReports,HandOverResults,ManageTestCatalogue,ManageReferenceRanges,ManagePricing,ManageOperators,ViewStatistics,ManageSettings,ManageAccounting
   ```

2. **Why it was necessary.** The `Operators` row was stored inside the deleted `.mdf`; with the file gone,
   the administrator account was gone with it. Without it no authentication could succeed, and the
   investigation's mandatory Administrator account had to exist again with `IsAdministrator = 1` and
   `IsActive = 1`.
3. **Evidence of completion.** The two outputs above (program output and `sqlcmd` row) agree on the name,
   the role caption, both flags, the permission set (15 permissions) and the stored hash.
4. **Nature of the change.** **Database record** (one inserted row). The password was not reset to a new
   value: the hash stored is the hash that the development environment had recorded for this account.

### Summary of the nature of all corrective actions

| Change type | Occurred? |
|---|---|
| Application source code | **No** — `git diff --stat` was empty at the end of the investigation. |
| Project files, configuration, connection strings | **No.** `REFLAB_CONNECTION_STRING` remained unset and the source fallback string was left as written. |
| Database structure (catalog, files, schema, migrations) | **Yes** — Actions 1–3. |
| Database records | **Yes** — Action 4 (one inserted `Operators` row). |
| Migration sources | **No** — read only. |
| Existing migrations | **No** — none was modified; the three project migrations were applied as they are. |

---

## 4.7 Authentication and Administrator Account Verification

### Account identity and how it was recovered

The account used for both login attempts was **`developer`**, with the password **`DevAdmin#2026!RefLab`**.
It was recovered from the project's own development artefacts — not from the user, and not invented:

| Source | File timestamp | Recovered content |
|---|---|---|
| `C:\Users\LAP LINK\AppData\Local\Temp\opencode\devhash.txt` | 2026-11-11 14:18:57 | `pbkdf2$sha256$210000$xBjiujggPfgElm3CXU+Dlw==$KhELbHqzOmWz3HIHNZKtCF6jBCkY9009BQE22lgrtgI=` (90 bytes, single line) |
| `C:\Users\LAP LINK\AppData\Local\Temp\opencode\verifylogin\Program.cs` | 2026-11-11 14:19:57 (edited 14:23:43) | `new SignInCommand("developer", "DevAdmin#2026!RefLab")` executed against `Server=(localdb)\mssqllocaldb;Database=RefLab;Trusted_Connection=True;TrustServerCertificate=True` |
| `C:\Users\LAP LINK\AppData\Local\Temp\opencode\RefLabLoginE2E\Program.cs` | 2026-11-11 17:31:50 (edited 17:32:05) | A separate in-process end-to-end harness using an isolated SQLite database with the operator `e2e.operator` / `E2E!Pass#2026` — **this is a different, synthetic account and was not used for any login attempt** |

### Password hashing algorithm and parameters

| Item | Value |
|---|---|
| Algorithm | PBKDF2-HMAC-SHA256 (`Rfc2898DeriveBytes.Pbkdf2`) |
| Iterations | **210 000** |
| Salt | 16 random bytes (`SaltSize = 16`) |
| Hash length | 32 bytes (`HashSize = 32`) |
| Stored format | `pbkdf2$sha256$<iterations>$<base64 salt>$<base64 hash>` — five `$`-separated segments |
| Comparison | `CryptographicOperations.FixedTimeEquals` |
| Implementation | `src/RefLab.Infrastructure/Security/Pbkdf2PasswordHasher.cs` |
| Stored hash of the account | `pbkdf2$sha256$210000$xBjiujggPfgElm3CXU+Dlw==$KhELbHqzOmWz3HIHNZKtCF6jBCkY9009BQE22lgrtgI=` |

### Historical verification procedure and recorded results

The recorded hash was verified offline (without any database access) by re-deriving PBKDF2-HMAC-SHA256
with the recorded salt and iteration count and comparing byte-for-byte:

| Candidate password | Result |
|---|---|
| `DevAdmin#2026!RefLab` | **True** — reproduces the recorded hash exactly |
| `developer`, `Developer`, `admin`, `DevAdmin#2026`, `DevAdmin#2026!`, `RefLab#Dev2026` | False |
| 70+ further common candidates (`admin`, `Password2026!`, `P6-Synthetic!Pass#2026`, `12345678`, `P@ssw0rd`, `RefLab2026!`, …) | False — no alternative matched |

After the database was restored, the same verification was repeated **through the application's own
hasher** against the live row, producing the recorded results:

| Check | Recorded result |
|---|---|
| `Pbkdf2PasswordHasher.Verify("DevAdmin#2026!RefLab", storedHash)` | **True** |
| `Pbkdf2PasswordHasher.Verify("wrong-password", storedHash)` | **False** |
| `Enum.GetValues<Permission>().All(restored.Has)` | **True** — the administrator holds every permission |

### Recorded account properties

| Property | Value | Source of the confirmation |
|---|---|---|
| `Name` | `developer` | Live `Operators` row (post-repair) and the live application (`User Name : developer`) |
| Account type | Administrator | `IsAdministrator = 1` on the live row; role caption `Development administrator`; `Has(p) == true` for all 15 permissions |
| `IsAdministrator` | **1** | Live `Operators` row read with `sqlcmd` before Attempt #2 and re-read by the running application (Operator administration → Refresh) |
| `IsActive` | **1** | Same live row; additionally the application's own `GetOperatorsQuery` (which filters on `IsActive = true`) returned the row |
| Permissions | 15 (all of `Permission`) | Live row `Permissions` column |
| `Id` | 1 | Live row |

### Documented discrepancy — two different RefLab installations, two different accounts

The retained evidence shows **two distinct databases named `RefLab`** on this machine, each with a
different operator account. They must not be conflated:

| Attribute | LocalDB `RefLab` — **the target of this investigation** | `.\SQLEXPRESS\RefLab` — a separate installation |
|---|---|---|
| Resolved by the application? | **Yes** (the `REFLAB_CONNECTION_STRING` fallback) | No |
| Data files | `C:\Users\LAP LINK\RefLab.mdf` (missing before the repair; recreated 2026-11-12 12:51:09) | `C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\RefLab.mdf` |
| Operator account | `developer`, `IsAdministrator = 1`, `IsActive = 1`, 15 permissions | `admin`, `IsAdministrator = 1`, `IsActive = 1`, 15 permissions |
| Password | `DevAdmin#2026!RefLab` — **verified** | **Not recovered.** The stored hash is `pbkdf2$sha256$210000$APhWWeN7MqlHSljPQodWzw==$8Q3ZPMHHYQmd1/5O9cigmLov/CB+uvgIJDiTFKYmkZk=`. A 72-candidate sweep returned no match; a larger sweep produced **no recorded result** (terminated with `Shell.NotFoundError`); an earlier development session's sweep also returned no `MATCH:` line. |
| Data present at inspection | schema only after restoration; before the repair the files were gone | 28 tables, the three migrations applied, one operator (`admin`), one attendance row dated `2026-11-11 02:01:05.8876235 +00:00`; `Patients` and `TestDefinitions` both at row count 0 |
| Disposition | repaired and used | **left completely untouched** |

**Interpretation.** The `admin` account of the SQL Express installation was *not* the account used in any
login attempt, because (a) the application never resolved to that instance, and (b) its password could not
be established from any retained source. The investigation therefore used, and restored, the `developer`
account of the database the application actually targets.

### Documented discrepancy — reported message wording

The task brief quoted the symptom as “The operation was not completed.” The string actually emitted and
captured in `attempt1_after.png` is “The operation could not be completed.” The two readings differ in one
word and the difference matters only because the emitted string is the *default fallback* of
`RequestExecutor.Describe` (§4.5.7); the conclusion drawn from it is identical in both cases.

### Documented discrepancy — account properties before the repair

The live `Operators` row for `developer` could **not** be read before Attempt #1, because the database
could not be opened (§4.5.6). The values `IsAdministrator = 1` and `IsActive = 1` for that account were
therefore carried from the recorded development credential at that time and could only be *confirmed
against the live row* after the database was restored. The investigation report of that time stated this
limitation explicitly rather than presenting the pre-repair values as independently verified.

---

## 4.8 Final Successful Login Verification

The following observations were recorded for Attempt #2 and remain in the retained artefacts.

| # | Observation | Where recorded | What it proves | What it does **not** prove |
|---|---|---|---|---|
| 1 | The sign-in window closed and the process was left with exactly **one** top-level window, named `Ref Lab`, sized 1360×860 (`Window enumeration` output: `top level windows: 1`). | `reflab_windows.ps1` output; `window1.png` | The sign-in flow completed and the application transitioned to its authenticated main window via `SignInWindow.OnNavigationChanged` → `MainWindow`. | It does not by itself prove *which* account was authenticated — see #4. |
| 2 | The authenticated main window rendered the full operational interface: module bar with 14 modules (Patient work, Test catalogue, Catalogue administration, Send-out queue, Microbiology and farm results, Accounting, Parties and pricing, Report setup, Laboratory settings, Operator administration, Statistics and performance, Audit trail and attendance, Reception), the patient-work screen, `Log out`, and the patient panel. | `window1.png`; UI Automation text dump | The post-authentication interface was constructed and is functional, i.e. the navigation and view-construction path ran to completion. | It does not prove that every screen behind those buttons was exercised — only the Operator administration module was opened. |
| 3 | The status bar displayed `User Name : developer`, `Last Login : Thu., 12 Nov. 2026 12:52`, `SQL DATABASE`, `Today : Thu., 12-Nov.-2026 12:52`. | `window1.png` | The session principal is `developer`, and the "last login" value written by the application matches the time of Attempt #2. | It does not independently prove the administrator flag — the flag is proven by #6/#7. |
| 4 | The Application layer wrote an attendance row: `B6F2436E-0ED9-4AAB-AEAC-27737BC6EFF3 | 1 | developer | 1 (StartedWork) | 2026-11-12 10:52:35.1512877 +00:00`, and `Operators.AttendanceStart` / `Operators.LastVisit` were set to the same instant. | Live `AttendanceEntries` / `Operators` rows | The write occurs in `SignInHandler.Handle` **after** the operator lookup, the PBKDF2 verification and the `IsActive` check. Its presence therefore proves that the real handler ran to completion — it cannot occur for a failed or bypassed authentication. | It does not by itself prove that *no* bypass exists elsewhere in the codebase; that was addressed by the unchanged source (`git diff` empty) plus the full test suite. |
| 5 | The engine log for the attempt contains `Starting up database 'RefLab'` (12:52:33.13) with a successful parallel-redo pair and **no** 17204/18456/4060 entry. | `error1.log` | The database opened normally during the successful attempt — the defect of Attempt #1 was genuinely gone, not bypassed. | It does not prove anything about other databases. |
| 6 | The administrator-only module **Operator administration** was opened from the module bar; the screen loaded with `Add`, `Edit`, `Apply` and `Save` enabled and the permission list populated (`EditPatient`, `EnterResults`, `HandOverResults`, `ManageAccounting`, `ManageOperators`, `ManagePricing`, …). | `attempt2_operator_admin.png` | The session principal satisfies the authorization check for the `ManageOperators` permission, i.e. **administrator authorization works** for the authenticated identity. | It does not prove that every permission in the catalogue was honoured by every command — the permission-level behaviour is covered by the application test suite (§4.9). |
| 7 | Invoking **Refresh** on that screen re-read the `Operators` table through the Application layer and rendered the row `developer | Development administrator | True` (Name / Code / Status columns, `Status = IsActive`). | `attempt2_operator_refresh.png` | The running application reads the restored database and the restored account carries `IsAdministrator = 1` (`Development administrator`) and `IsActive = 1` (`True`). It also demonstrates a live, authenticated, authorized database read. | It does not prove that the *other* installation's `admin` account is affected in any way — that installation was never touched. |
| 8 | No error text was present in the UI after the click, and no exception was recorded in the engine log or in the diagnostic channels for this attempt. | `attempt2_after.png`; `error1.log` | The failure mode of Attempt #1 did not recur. | — |

---

## 4.9 Build, Test, and Verification Results

### Results reported by the previous investigation

| Check | Reported result | Evidence anchor |
|---|---|---|
| Build | `dotnet build RefLab.sln -c Debug` → **Build succeeded. 0 Warning(s), 0 Error(s). Time Elapsed 00:01:06.17.** | Build output retained in the investigation transcript |
| Tests | `dotnet test RefLab.sln -c Debug` → **738 passed, 0 failed, 0 skipped**: Domain 317, Application 277, Infrastructure 65, Integration 63, Presentation 16 | Test output retained in the investigation transcript. The exact start/end timestamps of the test run are **not available in the retained historical evidence** (the run is bracketed by the 12:54:35 artefact and the 12:57:15 final report write). |
| Authentication verification (positive) | `Pbkdf2PasswordHasher.Verify("DevAdmin#2026!RefLab", storedHash)` → `True` | `reflabrestore` output + live `Operators` row |
| Authentication verification (negative) | `Pbkdf2PasswordHasher.Verify("wrong-password", storedHash)` → `False` | `reflabrestore` output |
| Administrator completeness | `Enum.GetValues<Permission>().All(restored.Has)` → `True`; 15 permissions stored | `reflabrestore` output + `sqlcmd` row |
| Schema | 3 migrations applied; 28 base tables; 44 EF Core entity types | `reflabrestore` output + `sqlcmd` |
| Database connectivity from the running application | Operator register loaded live from the restored database | `attempt2_operator_refresh.png` |
| Source integrity | `git status --porcelain` → only `?? LOGIN_FAILURE_INVESTIGATION_REPORT.md`; `git diff --stat` → empty | Command output retained in the transcript; independently corroborated by the current repository-root listing, which shows the investigation report as the only new file in the root |

### Distinction between reported and independently verifiable results

* **Reported by the previous investigation and not reproducible without re-running** (re-running is out of
  scope for this document): the build result, the 738-test result, the `reflabrestore` program output, and
  the `reflabdiag` exception dump. Their verbatim text is reproduced above because it is part of the
  retained transcript.
* **Independently verifiable from retained artefacts, without executing anything** (re-inspected while
  compiling this document):
  * the LocalDB engine error logs still contain the 17204 / 18456 pair for `2026-11-12 12:48:50.44`
    (now located in `error1.log` after the instance's log rotation), the pre-investigation repetitions in
    `error5.log` / `error4.log`, the investigation's own checks in `error3.log`, and the successful
    `Starting up database 'RefLab'` events at 12:52:10–12:52:33;
  * the restored database files exist with the recorded sizes and the creation timestamp 12:51:09;
  * the screenshot artefacts exist with the recorded timestamps (`attempt1_after.png` 12:48:50,
    `attempt2_after.png` 12:52:35, `window1.png` 12:53:35, `attempt2_operator_admin.png` 12:54:03,
    `attempt2_operator_refresh.png` 12:54:35);
  * the driver programs (`reflab_login_attempt.ps1`, `reflab_windows.ps1`, `reflab_click_module.ps1`) and
    the two helper programs (`reflabdiag`, `reflabrestore`) exist with their recorded content;
  * the repository root contains exactly one added file, the investigation report.
* **Not available in the retained historical evidence:** the shadow-copy state of the machine
  (`vssadmin` was not permitted); the result of the larger candidate-password sweep against the
  SQL Express `admin` hash (the background job terminated with `Shell.NotFoundError` before producing
  output); the precise second-level timestamps of the preparatory application launch and of the test run;
  and the identity of the actor that produced the 11:45–11:57 engine errors.

---

## 4.10 Files and Components Affected

### Files created or modified during the original investigation

| Path | Nature | Notes |
|---|---|---|
| `C:\Users\LAP LINK\source\repos\Ref Lab\LOGIN_FAILURE_INVESTIGATION_REPORT.md` | **Repository file — created** | The live investigation report written during the original task; 36 836 bytes; last written 2026-11-12 12:57:15. Untracked in Git. **Not modified by the present documentation task.** |
| `C:\Users\LAP LINK\source\repos\Ref Lab\LOGIN_FAILURE_INVESTIGATION_REPORT_HISTORICAL.md` | **Repository file — created by the present documentation task** | This retrospective document. |
| `C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflab_login_attempt.ps1` | Helper script — created (outside the repository) | UI-Automation driver for the real login window; created 12:48:17. |
| `C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflab_windows.ps1` | Helper script — created (outside the repository) | Window enumeration / screenshot tool; created 12:53:29. |
| `C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflab_click_module.ps1` | Helper script — created (outside the repository) | Module invocation / screenshot tool; created 12:53:51. |
| `C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabdiag\` | Diagnostic program — created (outside the repository) | Read-only reproduction of the failing operator lookup; produced the 4060 exception and stack trace. |
| `C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabrestore\` | Repair program — created (outside the repository) | Migrations + domain-API seeding of the administrator + verification. |
| `C:\Users\LAP LINK\AppData\Local\Temp\opencode\attempt1.png`, `attempt1_after.png`, `attempt2.png`, `attempt2_after.png`, `window1.png`, `attempt2_operator_admin.png`, `attempt2_operator_refresh.png` | Screenshots — created (outside the repository) | The visual evidence cited throughout this document. |
| `C:\Users\LAP LINK\AppData\Local\Temp\opencode\oc.txt` | Scratch dump — created and then deleted during the original investigation | A 46 MB textual extraction of the retained session store, used for credential archaeology and removed afterwards. |

### Application source files

**None were modified.** The end-of-investigation `git status --porcelain` output contained exactly one
entry — the untracked investigation report — and `git diff --stat` produced no output. The present
inspection of the repository root is consistent with this: the only file added at the root is the
investigation report (plus this retrospective document).

### Database files affected

| Object | Change | Nature |
|---|---|---|
| `C:\Users\LAP LINK\RefLab.mdf` | Recreated — 8 388 608 bytes, creation time 2026-11-12 12:51:09 | Database data file (new empty database) |
| `C:\Users\LAP LINK\RefLab_log.ldf` | Recreated — 8 388 608 bytes, creation time 2026-11-12 12:51:09 | Database log file (new empty database) |
| `(localdb)\MSSQLLocalDB` → catalog entry `RefLab` | Dropped and recreated | Catalog metadata |

### Database objects affected (in `(localdb)\MSSQLLocalDB.RefLab` only)

| Object | Change |
|---|---|
| `__EFMigrationsHistory` | Created by the migration pipeline; 3 rows (`20261110103458_InitialCreate`, `20261111083113_AddReportFooterAndLayoutText`, `20261111122338_AddReportBrandingAndSampleOrigin`) |
| 27 mapped domain tables (28 base tables including the migration-history table) | Created by the migrations |
| `Operators` | One row inserted: `Id = 1`, `developer`, `IsAdministrator = 1`, `IsActive = 1`, 15 permissions, recorded hash |
| `AttendanceEntries` | One row inserted **by the application** during Attempt #2 (`StartedWork`, 2026-11-12 10:52:35.1512877 +00:00) |

### Other components or resources changed

| Resource | Change |
|---|---|
| LocalDB instance state | The instance was started and stopped repeatedly during the investigation; its error logs were rotated by SQL Server itself (`error.log` … `error6.log`), which is why the 12:48:50 evidence now resides in `error1.log`. |
| `C:\Users\LAP LINK\AppData\Local\Temp\opencode\oc.txt` | Created and deleted during the investigation (scratch file). |
| Application process | Launched three times (PIDs 14456, 15316, 2604). PID 2604 was still running with the authenticated main window at the end of the investigation. |

---

## 4.11 Unmodified Resources and Scope Boundaries

The original investigation explicitly recorded the following as **inspected but untouched**. These
statements are reproduced from its retained report and are corroborated where possible by the retained
artefacts.

| Resource | Reported status |
|---|---|
| `.\SQLEXPRESS\RefLab` | **Untouched.** The original report lists it as “A *different* installation of the same product … It is **not** the database the application resolves to … **It is left untouched** — no data, schema or credential in it is modified.” Its operator `admin` (`IsAdministrator = 1`, `IsActive = 1`, all 15 permissions) and its single attendance row of `2026-11-11 02:01:05.8876235 +00:00` were read only, and the `admin` password was never recovered, changed or reset. |
| Other databases on the machine | **Untouched.** `.\SQLEXPRESS` also holds `OpenLab`, `OpenLab_FreshCleanTest`, `FinalLab`, `FinalLabSystem_MigrationTest` and `NewLabDb`; `(localdb)\MSSQLLocalDB` also holds `MasrLabDb`, `TopLab`, the `M2/M4/Slice*`, `PatientHistory_*` and `M13Repo_*` families; the LocalDB instances `OpenLabUITests` and `ProjectModels` hold only system databases. None were read for data, altered or dropped. |
| Application source files | **Untouched** — `git diff --stat` empty; no file under `src/`, `tests/`, `Docs/` or `scripts/` was edited. |
| `REFLAB_CONNECTION_STRING` | **Left unset**, exactly as found. The environment variable was neither set nor cleared by the investigation. |
| The fallback connection string in `CompositionRoot.cs` | **Unchanged.** The application still resolves to `(localdb)\MSSQLLocalDB.RefLab` after the repair. |
| Migration sources | **Unchanged** — the three migration files were read and applied, never edited or regenerated. |
| `RequestExecutor.Describe` / the exception-translation contract | **Unchanged**, by deliberate decision. The generic-message weakness was recorded as an observation and as a *remaining issue*, not changed, because it was outside the minimum correction for the actual defect. |
| The legacy reference copy `F:\REAL_LAB_COPY` | Read-only inspection only (no database files were found there); nothing was executed from it and nothing was modified — consistent with the project's standing rule that the reference system is strictly read-only. |

---

## 4.12 Data Restoration and Limitations

This section draws a distinction that must not be blurred.

| Aspect | What actually happened | Evidence |
|---|---|---|
| **Database schema recreation** | **Yes.** The schema was rebuilt from scratch. The database was empty when created; the schema was produced exclusively by the project's three migrations. | `CREATE DATABASE` with the original file layout; `reflabrestore` output “migrations applied by this run : 3”; `__EFMigrationsHistory` with 3 rows; `INFORMATION_SCHEMA.TABLES` → 28 base tables. |
| **Database migrations** | **Yes, executed** — `20261110103458_InitialCreate`, `20261111083113_AddReportFooterAndLayoutText`, `20261111122338_AddReportBrandingAndSampleOrigin` — through `DatabaseInitializer.MigrateAsync()` / `Database.MigrateAsync()`. | `reflabrestore` output and the independent `sqlcmd` listing of `__EFMigrationsHistory`. |
| **Administrator account restoration** | **Yes.** The `developer` row was re-created with the recorded hash and with `IsAdministrator = 1`, `IsActive = 1` and all 15 permissions. | `reflabrestore` output; independent `sqlcmd` row; the live application screen `developer | Development administrator | True`. |
| **Restoration of historical business data** | **No.** **The previous investigation did not recover the original business data of the LocalDB `RefLab` database, and did not claim to.** The `.mdf`/`.ldf` pair was deleted from disk; no copy was found in the Recycle Bin, in the user profile, in `F:\REAL_LAB_COPY`, in the SQL Server program folder or in any `.bak` file; the shadow-copy state of the machine could not be established (`vssadmin list shadows` was refused). The `Operators` row and the attendance history of the original database were therefore **not** recovered — they were re-created from the development artefacts and from the application's own behaviour. The original report states this explicitly: “The contents of the LocalDB `RefLab` database were destroyed by the fault … Nothing that still exists on disk is deleted or overwritten by this investigation.” | `Test-Path` = `False`; Recycle-Bin scan with no `RefLab` entry; the `vssadmin` permission error; the original report's “Database preservation statement”. |

**Scope of the loss.** The original LocalDB `RefLab` database had been created on 2026-11-11 by the
development environment and, at the last moment the engine could still open it (2026-11-11 17:01, 17:49,
17:58), contained the schema, the `developer` account and, on the evidence of the sibling installation's
shape, little or no clinical business data (the SQL Express installation of the same product held
`Patients = 0` and `TestDefinitions = 0` rows). **The precise row counts of the original LocalDB
`RefLab` database at the moment of deletion are not available in the retained historical evidence**, so
the extent of the unrecoverable business data cannot be quantified from the retained record.

**What this means for the conclusion.** The restoration recreated a *working, correctly configured
installation* with the correct schema and the correct Administrator account — it did not resurrect the
original database contents. Any statement that the original data was restored would be unsupported.

---

## 4.13 Final Conclusion

**What originally caused the login failure.** The RefLab sign-in did not fail because of the credentials,
the password hashing, the account flags or the unset environment variable. It failed because the
application's target database — `(localdb)\MSSQLLocalDB.RefLab` — had lost both of its files
(`C:\Users\LAP LINK\RefLab.mdf` and `C:\Users\LAP LINK\RefLab_log.ldf`) while its LocalDB catalog entry
survived. Every connection to that database therefore failed at logon with server errors **17204**
(`FCB::Open failed … OS error: 2`) and **18456** (`Failed to open the explicitly specified database
'RefLab'`), which the client received as **4060** (`Cannot open database "RefLab" requested by the
login`). The exception was raised while the connection was being opened — inside the operator-lookup query
at the top of `SignInHandler.Handle` — so the operator record was never retrieved and the PBKDF2
comparison was never performed. The generic on-screen text “The operation could not be completed.” was
produced by the default arm of `RequestExecutor.Describe`, because the data-access exception is not part
of the Application layer's exception hierarchy and was therefore discarded without a trace.

**How the cause was identified.** By triangulation across four independent sources: (i) the LocalDB
instance's `sys.master_files` mapping, which proved that the catalog still expected the database at
exactly the two paths; (ii) direct filesystem inspection, which proved the files were absent while the
catalog entry persisted; (iii) the SQL Server engine error logs, which showed the 17204/18456 pair at the
exact second of every failed connection — including the second of Attempt #1 — and which also showed the
same database starting normally on the evening of 2026-11-11; and (iv) a read-only diagnostic that
reproduced the handler's own query and captured the full `InvalidOperationException` → `SqlException`
(4060) chain with its stack trace.

**What was changed during the previous investigation.** No application source code, no project file, no
configuration and no connection string. Four database-level actions: the orphaned catalog entry was
removed; the `RefLab` database was recreated with its original `C:\Users\LAP LINK\RefLab.mdf` /
`RefLab_log.ldf` layout; the project's own three migrations were applied through
`DatabaseInitializer.MigrateAsync()`; and the development administrator `developer` was restored through
the application's domain API with the hash recorded by the development environment, with
`IsAdministrator = 1`, `IsActive = 1` and all 15 permissions.

**How the successful login was established.** Attempt #2 drove the real `Ref Lab - Sign in` window of the
real WPF application with `developer` / `DevAdmin#2026!RefLab` and invoked the real **Login** button. The
sign-in window closed; the authenticated main window opened, showing `User Name : developer` and
`Last Login : Thu., 12 Nov. 2026 12:52`; the handler wrote its attendance row at
`2026-11-12 10:52:35.1512877 +00:00`; and the administrator-only *Operator administration* module
re-read the restored register and displayed `developer | Development administrator | True`.

**What the investigation conclusively demonstrated.** (1) The recurring login failure was a storage-layer
fault, reproducible on every attempt and independent of the credentials. (2) The message shown to the
operator was a generic fallback that concealed the real fault; the true exception (4060 / 17204 / 18456)
was recoverable only from the engine log and from an unhandled-exception dump. (3) The unset
`REFLAB_CONNECTION_STRING` was functioning exactly as designed, and the fallback resolution was correct.
(4) The credential pair `developer` / `DevAdmin#2026!RefLab` is cryptographically consistent with the
hash recorded by the development environment, and the application's own hasher confirms it positively and
negatively. (5) After a purely database-level restoration, the same account authenticates through the
real login window with full administrator authorisation and no change to the authentication code. (6) The
complete test suite (738 tests) remained green throughout, and `git diff` was empty, proving that no
authentication check was weakened, disabled or bypassed.

**What remains outside the scope of the available evidence.** (1) The original business contents of the
deleted LocalDB `RefLab` database were **not** recovered, and their extent at the moment of deletion is
not quantifiable from the retained record; only the schema and the Administrator account were re-created.
(2) The password of the `admin` account in the separate `.\SQLEXPRESS\RefLab` installation was never
recovered — 72 candidates were tested without a match and a larger sweep produced no recorded result.
(3) The machine's shadow-copy state could not be established, so it cannot be excluded that a
point-in-time copy of the deleted database might once have existed. (4) The actor behind the
11:45–11:57 connection failures recorded on 2026-11-12 (before the investigation began) is not
identified in the retained evidence. (5) The exact second-level timestamps of the preparatory application
launch and of the test-suite run are not recorded. (6) The end-of-investigation state of the still-running
application process (PID 2604) after the investigation concluded is not part of the retained record.

**Net outcome.** Two of the ten permitted login attempts were used. Attempt #1 failed and its root cause was
documented before anything was changed; Attempt #2 succeeded. The defect was environmental rather than
programmatic, the correction was correspondingly confined to the database, and the successful result was
demonstrated through the application's real login interface and independently corroborated by the
handler's own attendance record, the live administrator screen and the unchanged source tree.

---

*End of report.*
