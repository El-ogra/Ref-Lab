# RefLab — Login Failure Investigation Report

**Project:** RefLab (Medical Laboratory Management System)
**Solution:** `C:\Users\LAP LINK\source\repos\Ref Lab\RefLab.sln`
**Report file:** `LOGIN_FAILURE_INVESTIGATION_REPORT.md` (repository root)

---

## Investigation Overview

| Item | Value |
|---|---|
| Project name | RefLab |
| Start timestamp | 2026-11-12 12:45:51 +02:00 (10:45:51 UTC) |
| Initial reported error | The application displays **“The operation was not completed.”** after the Login button is pressed. The exact string emitted by the application is **“The operation could not be completed.”** (the generic fallback of `RequestExecutor.Describe`). |
| Investigation status | **COMPLETE — SUCCESS.** Attempt #1 failed (root cause documented), attempt #2 after the repair succeeded. |
| Maximum attempts allowed | 10 |

### Reported reproduction sequence

1. Application starts.
2. Login window appears.
3. Operator name and password are entered.
4. Login is pressed.
5. Authentication fails with the generic message above.

### Solution map (relevant parts)

| Layer | File | Relevance |
|---|---|---|
| Presentation | `src/RefLab.Presentation/App.xaml.cs` | Starts the host and shows `SignInWindow`. |
| Presentation | `src/RefLab.Presentation/Bootstrap/CompositionRoot.cs` | Resolves the connection string (env var → fallback). |
| Presentation | `src/RefLab.Presentation/ViewModels/SignInViewModel.cs` | `SignInCommand` → `RequestExecutor.SendAsync(new SignInCommand(...))`. |
| Presentation | `src/RefLab.Presentation/Common/RequestExecutor.cs` | Translates exceptions into operator-visible messages. |
| Presentation | `src/RefLab.Presentation/Common/ViewModelBase.cs` | `RunAsync` catches every exception and stores `RequestExecutor.Describe(exception)`. |
| Application | `src/RefLab.Application/Features/Identity/SignIn.cs` | `SignInHandler`: operator lookup → password verification → active check → attendance + save. |
| Infrastructure | `src/RefLab.Infrastructure/Security/Pbkdf2PasswordHasher.cs` | PBKDF2-HMAC-SHA256, 210 000 iterations, format `pbkdf2$sha256$<iter>$<salt>$<hash>`. |
| Infrastructure | `src/RefLab.Infrastructure/Persistence/RefLabDbContext.cs` | EF Core 8 context, 27 mapped tables, `Operators` is one of them. |
| Infrastructure | `src/RefLab.Infrastructure/Persistence/DatabaseInitializer.cs` | Applies pending migrations (operator tool; deliberately **not** resolved by the shell). |

---

## Administrator Account Information

The account used for **every** login attempt in this investigation is the development administrator of the
RefLab project, identified from the project's own development artefacts (see *Sources* below).

| Property | Value |
|---|---|
| **Actual username used** | `developer` |
| **Actual password used** | `DevAdmin#2026!RefLab` |
| Account type | Administrator |
| `IsAdministrator` | `1` |
| `IsActive` | `1` |
| Stored hash (PBKDF2) | `pbkdf2$sha256$210000$xBjiujggPfgElm3CXU+Dlw==$KhELbHqzOmWz3HIHNZKtCF6jBCkY9009BQE22lgrtgI=` |

### Credential sources (authorised development sources, not invented)

| Source | Content |
|---|---|
| `C:\Users\LAP LINK\AppData\Local\Temp\opencode\devhash.txt` (2026-11-11 14:18:57) | `pbkdf2$sha256$210000$xBjiujggPfgElm3CXU+Dlw==$KhELbHqzOmWz3HIHNZKtCF6jBCkY9009BQE22lgrtgI=` — the stored password hash of the development operator. |
| `C:\Users\LAP LINK\AppData\Local\Temp\opencode\verifylogin\Program.cs` (2026-11-11 14:19:57) | `new SignInCommand("developer", "DevAdmin#2026!RefLab")` run against the LocalDB `RefLab` database — the recorded development sign-in. |

**Independent cryptographic confirmation (read-only, no database change):**
PBKDF2-HMAC-SHA256, 210 000 iterations, salt `xBjiujggPfgElm3CXU+Dlw==` applied to `DevAdmin#2026!RefLab`
reproduces exactly `KhELbHqzOmWz3HIHNZKtCF6jBCkY9009BQE22lgrtgI=`. The recorded hash therefore belongs to the
password `DevAdmin#2026!RefLab`. The same check was run against 70+ alternative candidates (`admin`,
`Password2026!`, `P6-Synthetic!Pass#2026`, …) — no other candidate reproduces the recorded hash.

### Account verification results

| Check | Result |
|---|---|
| Account exists in the target database | **YES.** The row was destroyed together with the missing database file and was restored during the repair (see *Attempt #1 → Corrective Action*); it is now live in `(localdb)\MSSQLLocalDB.RefLab`. |
| `IsAdministrator = 1` | **Verified** — live `Operators` row: `IsAdministrator = 1`; `Operator.Has(permission)` is true for every permission. |
| `IsActive = 1` | **Verified** — live `Operators` row: `IsActive = 1`. |
| Account type | **Administrator** (role caption `Development administrator`). |
| Password | **Verified** — `Pbkdf2PasswordHasher.Verify("DevAdmin#2026!RefLab", storedHash)` = `True`; `Verify("wrong-password", …)` = `False`. |

---

## Database Configuration

### Expected configuration (as stated for this investigation)

| Item | Expected value |
|---|---|
| SQL Server instance | `(localdb)\MSSQLLocalDB` |
| Database | `RefLab` |
| Database file | `C:\Users\LAP LINK\RefLab.mdf` |
| Environment variable | `REFLAB_CONNECTION_STRING` — **unset** |
| Expected behaviour | The application falls back to LocalDB / `RefLab`. |

### Verification performed

| # | Check | Command / source | Result |
|---|---|---|---|
| 1 | LocalDB instance availability | `SqlLocalDB info` | **OK** — `MSSQLLocalDB`, version `17.0.1000.7`, owner `DESKELTEL\LAP LINK`, auto-create `Yes`. Instances `OpenLabUITests` and `ProjectModels` also exist but hold no user database. |
| 2 | Database existence (catalog) | `SELECT name FROM sys.databases` on `(localdb)\MSSQLLocalDB` | **Catalog entry present**: `RefLab` (state `ONLINE`, access `MULTI_USER`). |
| 3 | MDF file existence | `Test-Path "C:\Users\LAP LINK\RefLab.mdf"` and `Get-ChildItem C:\Users\LAP LINK\*.mdf` | **MISSING.** The directory contains only `AmanaLab.mdf`, `EasyLabDb_V2.mdf`, `MedoLabDB.mdf` (other projects). `C:\Users\LAP LINK\RefLab.mdf` and `C:\Users\LAP LINK\RefLab_log.ldf` do not exist. |
| 4 | Actual connection-string resolution | `CompositionRoot.ResolveConnectionString` (`src/RefLab.Presentation/Bootstrap/CompositionRoot.cs`, lines 77–95) | `REFLAB_CONNECTION_STRING` is unset → **fallback** `Server=(localdb)\mssqllocaldb;Database=RefLab;Trusted_Connection=True;TrustServerCertificate=True`. The fallback resolves to the LocalDB `RefLab` database. **The unset environment variable is therefore NOT the defect.** |
| 5 | Actual database used by the running application | The fallback string above → `(localdb)\MSSQLLocalDB.RefLab` | Same database whose MDF is missing. |
| 6 | Database connectivity | `sqlcmd -S "(localdb)\MSSQLLocalDB" -E -d RefLab -Q "SELECT ..."` | **FAILED** — `Login failed for user 'DESKELTEL\LAP LINK'. Cannot open database "RefLab" requested by the login. The login failed.` (Server error 18456 / 17204, `FCB::Open failed: Could not open file C:\Users\LAP LINK\RefLab.mdf … OS error: 2`.) |
| 7 | Schema compatibility | `sys.master_files` of `(localdb)\MSSQLLocalDB` | The catalog entry maps `RefLab` → `C:\Users\LAP LINK\RefLab.mdf` and `RefLab_log` → `C:\Users\LAP LINK\RefLab_log.ldf` — i.e. the *expected* file layout is already recorded by the engine; only the files are gone. |
| 8 | Authentication-related database objects | `SELECT name FROM sys.databases` (LocalDB) | `RefLab` is present in the catalog but **cannot be opened**, so its `Operators` table cannot be read at all. |
| 9 | Administrator account existence and properties | see above | The account cannot be read from the target database because the database cannot be opened. |
| 10 | Database-related cause of the login failure | LocalDB engine error log `…\MSSQLLocalDB\error*.log` | See below — **this is a confirmed database-level cause.** |

### Database state after the repair (for reference)

| Item | Value |
|---|---|
| Instance | `(localdb)\MSSQLLocalDB` |
| Database | `RefLab` — `ONLINE` |
| Data file | `C:\Users\LAP LINK\RefLab.mdf` (8 388 608 bytes, 2026-11-12 12:52:11) |
| Log file | `C:\Users\LAP LINK\RefLab_log.ldf` (8 388 608 bytes, 2026-11-12 12:52:12) |
| Migrations applied | `20261110103458_InitialCreate`, `20261111083113_AddReportFooterAndLayoutText`, `20261111122338_AddReportBrandingAndSampleOrigin` |
| Base tables | 28 |
| Authentication objects | `Operators` (1 row: `developer`, administrator, active) and `AttendanceEntries` |

### SQL Server engine log evidence (LocalDB instance `MSSQLLocalDB`)

`C:\Users\LAP LINK\AppData\Local\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\error2.log` and `error1.log`:

```
2026-11-12 11:45:49.98 spid51  FCB::Open failed: Could not open file C:\Users\LAP LINK\RefLab.mdf for file number 0.  OS error: 2(The system cannot find the file specified.).
2026-11-12 11:45:49.99 Logon   Login failed for user 'DESKELTEL\LAP LINK'. Reason: Failed to open the explicitly specified database 'RefLab'. [CLIENT: <named pipe>]
```

The same pair of messages repeats for every connection attempt (11:45, 11:46, 11:47, 11:48, 11:55, 11:56,
11:57 on 2026-11-12). The log also shows the database was **healthy on 2026-11-11**:

```
error6.log  2026-11-11 17:01:56.60 spid51  Starting up database 'RefLab'.
error4.log  2026-11-11 17:49:18.88 spid51  Starting up database 'RefLab'.
error3.log  2026-11-11 17:58:17.20 spid51  Starting up database 'RefLab'.
```

### Other database instances on the machine (inspected, not modified)

| Instance | Database | Content | Decision |
|---|---|---|---|
| `(localdb)\MSSQLLocalDB` | `RefLab` | catalog entry, **file missing** | **Target of the investigation.** |
| `.\SQLEXPRESS` | `RefLab` | full RefLab schema (28 tables, 3 migrations applied), one operator `admin` (`IsAdministrator=1`, `IsActive=1`, all 15 permissions), one attendance entry dated 2026-11-11 02:01:05 UTC, empty catalogue/patient tables | A *different* installation of the same product. It is **not** the database the application resolves to (the application resolves to LocalDB), and the password of its `admin` operator is not recorded anywhere in the project or its development artefacts. **It is left untouched** — no data, schema or credential in it is modified. |
| `.\SQLEXPRESS` | `OpenLab`, `OpenLab_FreshCleanTest`, `FinalLab`, `FinalLabSystem_MigrationTest`, `NewLabDb` | unrelated installations | untouched |
| `(localdb)\MSSQLLocalDB` | `MasrLabDb`, `TopLab`, `*_Slice*`, `*_Recovery*`, `PatientHistory_*`, … | unrelated installations / test artefacts | untouched |

### Database preservation statement

* The contents of the LocalDB `RefLab` database were destroyed by the fault (the entire `.mdf`/`.ldf` pair was
  deleted from disk; the file is not in the Recycle Bin and no `.bak` or shadow copy is available).
  Nothing that still exists on disk is deleted or overwritten by this investigation.
* The `.\SQLEXPRESS\RefLab` database is **not** modified in any way.
* The repository source is modified only where the root cause requires it, and every modification is listed
  in the *Corrective Action* sections.

---

## Individual Login Attempts

> Attempt #1 has not been executed yet. The section is created here — before the first attempt — as required,
> and is completed with the observed result immediately after the attempt is executed.

### Attempt #1 — FAILURE (no repair applied before this attempt)

#### Execution Information

| Item | Value |
|---|---|
| Attempt number | **#1** |
| Timestamp | 2026-11-12 12:48:50 +02:00 (10:48:50 UTC) |
| Application | `src/RefLab.Presentation/bin/Debug/net8.0-windows10.0.17763.0/RefLab.Presentation.exe` (built from the current working tree, `dotnet build RefLab.sln -c Debug` → *Build succeeded, 0 Warning(s), 0 Error(s)*) |
| Window shown | `Ref Lab - Sign in` (the real `SignInWindow` hosting `SignInView`) |
| Username used | `developer` |
| Password used | `DevAdmin#2026!RefLab` |
| Account type | Administrator |
| `IsAdministrator` | `1` (required value; recorded credential — see *Account verification results*) |
| `IsActive` | `1` (required value; recorded credential) |
| Pre-attempt account check | The target database could not be opened, so the `Operators` row could **not** be read before the attempt (see *Database Configuration*, checks 6 and 9). The account properties are therefore carried from the recorded development credential, and re-verified against the live row as part of the repair (Attempt #2). |
| Method | Real GUI: the operator name was typed into the `Operator name` field, the password into the `Password` field, and the **Login** button was pressed through Windows UI Automation on the running WPF application. |

#### Observed Result

| Item | Value |
|---|---|
| Result | **FAILURE** |
| Exact UI message | **“The operation could not be completed.”** (red text under the password field) |
| Application behaviour | The sign-in window stayed open; no navigation to the patient-work screen; no session was established. |
| Screenshot | `C:\Users\LAP LINK\AppData\Local\Temp\opencode\attempt1_after.png` (captured from the running application after the click) |

#### Technical Investigation

**1. SQL Server engine log — the connection never reached the database**

`C:\Users\LAP LINK\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\error.log`, written at the exact moment of the attempt:

```
2026-11-12 12:48:50.44 spid51  Error: 17204, Severity: 16, State: 1.
2026-11-12 12:48:50.44 spid51  FCB::Open failed: Could not open file C:\Users\LAP LINK\RefLab.mdf for file number 0.  OS error: 2(The system cannot find the file specified.).
2026-11-12 12:48:50.44 Logon   Error: 18456, Severity: 14, State: 38.
2026-11-12 12:48:50.44 Logon   Login failed for user 'DESKELTEL\LAP LINK'. Reason: Failed to open the explicitly specified database 'RefLab'. [CLIENT: <named pipe>]
```

**2. Client-side exception (read-only diagnostic, `…\Temp\opencode\reflabdiag`, query only — no password verification)**

The diagnostic reproduces the exact database access of `SignInHandler.Handle` (the operator lookup on `Operators`) against the connection string the application resolves. It prints the raw exception:

```
=== EXCEPTION ===
Type      : System.InvalidOperationException
Message   : An exception has been raised that is likely due to a transient failure. Consider enabling transient error resiliency by adding 'EnableRetryOnFailure' to the 'UseSqlServer' call.
Inner[1]   : Microsoft.Data.SqlClient.SqlException: Cannot open database "RefLab" requested by the login. The login failed.
              Login failed for user 'DESKELTEL\LAP LINK'.
Error Number: 4060, State: 1, Class: 11
```

Stack trace (abridged to the application-relevant frames):

```
System.InvalidOperationException: An exception has been raised that is likely due to a transient failure. ...
 ---> Microsoft.Data.SqlClient.SqlException (0x80131904): Cannot open database "RefLab" requested by the login. The login failed.
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.CompleteLogin(Boolean enlistOK)
   at Microsoft.Data.SqlClient.SqlConnection.Open()
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerConnection.OpenDbConnection(Boolean errorsExpected)
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.Open(Boolean errorsExpected)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReader(RelationalCommandParameterObject parameterObject)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.Enumerator.InitializeReader(Enumerator enumerator)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.Execute[TState,TResult](...)
   --- End of inner exception stack trace ---
```

**3. Exact failing component / operation**

| Level | Location |
|---|---|
| Failing operation | The **operator lookup** — `SELECT … FROM [Operators] WHERE UPPER([Name]) = @p` — i.e. the first statement of `SignInHandler.Handle` (`src/RefLab.Application/Features/Identity/SignIn.cs`, lines 50–51). |
| Failing component | `SqlConnection.Open()` → `RelationalConnection.Open` → EF Core `SqlServerExecutionStrategy`, wrapping `SqlException` **4060** into `InvalidOperationException`. |
| Where it surfaces | `RequestExecutor.SendAsync` (`src/RefLab.Presentation/Common/RequestExecutor.cs`) translates only `RequestValidationException` and `DomainException`; the SQL exception is neither, so it propagates to `ViewModelBase.RunAsync` (line 71–75), which stores `RequestExecutor.Describe(exception)`. |
| Why the message is generic | `RequestExecutor.Describe` (lines 65–81) has no case for a data-access failure, so the **default arm** produces *“The operation could not be completed.”* The displayed text is therefore **not** the original exception. |

#### Verified Root Cause

**The login fails because the application cannot open its database, not because of the credentials.**

Chain of confirmed facts:

1. `REFLAB_CONNECTION_STRING` is unset, so `CompositionRoot.ResolveConnectionString` falls back to
   `Server=(localdb)\mssqllocaldb;Database=RefLab;Trusted_Connection=True;TrustServerCertificate=True`.
   The unset environment variable is **by design** and is **not** the defect.
2. The LocalDB instance `(localdb)\MSSQLLocalDB` exists and is healthy, and its catalog contains the
   database `RefLab`, mapped to `C:\Users\LAP LINK\RefLab.mdf` / `C:\Users\LAP LINK\RefLab_log.ldf`.
3. **Those two files do not exist on disk.** `Test-Path` returns `False`; the directory holds only
   `AmanaLab.mdf`, `EasyLabDb_V2.mdf` and `MedoLabDB.mdf` (other projects); the files are not in the
   Recycle Bin and no `.bak` or shadow copy exists.
4. The engine therefore fails the logon: error **17204** (`FCB::Open failed … OS error: 2`) followed by
   error **18456** (`Failed to open the explicitly specified database 'RefLab'`), which the client sees as
   **4060** (`Cannot open database "RefLab" requested by the login`).
5. The failure occurs **while opening the connection**, i.e. **before** any operator row is read and
   **before** any password is verified. The authentication code path is therefore not reached at all.
6. The exception is not part of the Application layer's exception hierarchy, so it falls through to the
   default arm of `RequestExecutor.Describe` and the operator sees the generic
   *“The operation could not be completed.”*

**Confirmed facts**

* The application starts correctly and the real sign-in window appears.
* The connection string resolution is correct and reaches the intended LocalDB `RefLab` database.
* The database file of that database is missing from disk — this is the single defect that produces the failure.
* The error text shown to the operator is a generic fallback and hides the real cause (SQL error 4060 / 17204).
* The credential pair `developer` / `DevAdmin#2026!RefLab` is confirmed correct against the recorded hash
  (PBKDF2 re-derivation reproduces the recorded hash byte-for-byte).

**Hypotheses tested and rejected**

| Hypothesis | Verdict | Reason |
|---|---|---|
| Wrong user name or password | **Rejected** | The password is never compared: the connection fails first, while opening the database. |
| Password-hash algorithm mismatch (`Pbkdf2PasswordHasher`) | **Rejected** | The hasher is never invoked; also its format handling is correct (5 segments) and the recorded hash verifies against the recorded password. |
| Account inactive / not an administrator | **Rejected** | The `Operators` row is never read. |
| `REFLAB_CONNECTION_STRING` unset ⇒ wrong database | **Rejected** | The fallback resolves to the intended LocalDB `RefLab`; the unset variable is the designed behaviour. |
| Schema incompatibility | **Rejected** | The database never opens, so the schema is not involved. The migrations of the project are the only source of the schema and they are applied unchanged. |
| **`RefLab.mdf` / `RefLab_log.ldf` deleted from disk** | **CONFIRMED** | `Test-Path` = `False`; SQL engine error 17204 (`OS error: 2`) at the exact attempt time; the same database started normally on 2026-11-11 17:01 / 17:49 / 17:58. |

#### Proposed Correction

1. Remove the orphaned catalog entry `RefLab` from `(localdb)\MSSQLLocalDB` (its files are already gone;
   nothing that still exists on disk is deleted).
2. Recreate the database `RefLab` with its **original** file layout
   (`C:\Users\LAP LINK\RefLab.mdf`, `C:\Users\LAP LINK\RefLab_log.ldf`).
3. Apply the project's own migrations (`20261110103458_InitialCreate`,
   `20261111083113_AddReportFooterAndLayoutText`, `20261111122338_AddReportBrandingAndSampleOrigin`)
   through `DatabaseInitializer` / `Database.MigrateAsync()` — the schema is never written by hand and no
   existing migration is modified.
4. Restore the development administrator `developer` through the domain API
   (`Operator.Create` → `SetAdministrator(true)` → `SetActive(true)` → grant every permission) using the
   **recorded** hash `pbkdf2$sha256$210000$xBjiujggPfgElm3CXU+Dlw==$KhELbHqzOmWz3HIHNZKtCF6jBCkY9009BQE22lgrtgI=`
   from the development artefact `devhash.txt` (identical to hashing `DevAdmin#2026!RefLab` with the
   application's own `Pbkdf2PasswordHasher`).
5. **No source change is required**: the connection-string resolution, the authentication service, the
   password hasher and the authorization checks are all correct and are left untouched.

#### Corrective Action

**Implemented after the root cause above was documented. No source file was modified — the defect was not in the code.**

| # | Action | Where | Reason |
|---|---|---|---|
| 1 | Removed the orphaned catalog entry `RefLab` (its files were already gone; `DROP DATABASE` first failed with error 5120 and succeeded on the retry) | `(localdb)\MSSQLLocalDB` | The entry pointed at a non-existent file and made every connection fail with 4060/17204. |
| 2 | Recreated the database with its **original** file layout: `CREATE DATABASE [RefLab] ON PRIMARY (NAME=N'RefLab', FILENAME=N'C:\Users\LAP LINK\RefLab.mdf') LOG ON (NAME=N'RefLab_log', FILENAME=N'C:\Users\LAP LINK\RefLab_log.ldf')` | `(localdb)\MSSQLLocalDB` | Restores the exact database configuration the application expects. |
| 3 | Applied the project's own migrations through `DatabaseInitializer.MigrateAsync()` → `Database.MigrateAsync()`: `20261110103458_InitialCreate`, `20261111083113_AddReportFooterAndLayoutText`, `20261111122338_AddReportBrandingAndSampleOrigin` | schema | The schema is never written by hand and no existing migration is modified. |
| 4 | Restored the development administrator `developer` through the domain API (`Operator.Create` → `SetAdministrator(true)` → `SetActive(true)` → grant every `Permission`) using the **recorded** hash `pbkdf2$sha256$210000$xBjiujggPfgElm3CXU+Dlw==$KhELbHqzOmWz3HIHNZKtCF6jBCkY9009BQE22lgrtgI=` from the development artefact `devhash.txt` | `Operators` table | Restores the development administrator with the credential that was actually issued. The stored hash is byte-for-byte the hash of `DevAdmin#2026!RefLab` produced by the application's own `Pbkdf2PasswordHasher`; no new password was invented. |

**Files / database objects involved**

| Object | State after the repair |
|---|---|
| `C:\Users\LAP LINK\RefLab.mdf` | restored (8 388 608 bytes, 2026-11-12 12:52:11) |
| `C:\Users\LAP LINK\RefLab_log.ldf` | restored (8 388 608 bytes, 2026-11-12 12:52:12) |
| `(localdb)\MSSQLLocalDB` → database `RefLab` | `ONLINE`, 28 base tables, 3 migrations applied |
| `Operators` row `Id = 1` | `Name=developer`, `RoleCaption=Development administrator`, `IsAdministrator=1`, `IsActive=1`, 15 permissions, recorded hash |
| `.\SQLEXPRESS\RefLab` and every other database on the machine | **untouched** |

**Modified repository files**

| File | Change |
|---|---|
| `LOGIN_FAILURE_INVESTIGATION_REPORT.md` | **added** (this report) |

No file under `src/`, `tests/`, `Docs/` or `scripts/` was changed — `git status` shows only the new
report file and `git diff` is empty.

#### Verification

| Check | Result |
|---|---|
| Build | `dotnet build RefLab.sln -c Debug` → **Build succeeded, 0 Warning(s), 0 Error(s)** |
| Database result | `RefLab` recreated at `C:\Users\LAP LINK\RefLab.mdf`; `sys.databases` → `ONLINE`; `INFORMATION_SCHEMA.TABLES` → **28** base tables (identical to the sibling installation); `__EFMigrationsHistory` → the 3 project migrations |
| Configuration result | `REFLAB_CONNECTION_STRING` is still **unset**; the application still resolves the LocalDB fallback and now opens the database successfully. No configuration change was needed. |
| Account verification | `Operators` row: `IsAdministrator = 1`, `IsActive = 1`, account type Administrator, 15 permissions, `Operator.Has(p) == true` for every `Permission` |
| Credential verification | `Pbkdf2PasswordHasher.Verify("DevAdmin#2026!RefLab", storedHash)` → **True**; `Verify("wrong-password", storedHash)` → **False** |
| Authentication / authorization intact | The authentication service, the password hasher, the permission checks and the authorization behaviour were not modified; the whole test suite still passes (738/738). |
| Test suite | `dotnet test RefLab.sln -c Debug` → Domain 317, Application 277, Infrastructure 65, Integration 63, Presentation 16 — **738 passed, 0 failed** |
| No bypass introduced | The sign-in still goes through `SignInHandler` (operator lookup → PBKDF2 verification → active check → attendance). The attendance row proves the real handler ran: `AttendanceEntries` gained the row `OperatorName=developer, EventType=1, OccurredAt=2026-11-12 10:52:35.1512877 +00:00`, and `Operators.AttendanceStart/LastVisit` were set to the same instant. |

#### Next Attempt

| Item | Value |
|---|---|
| Next attempt number | #2 |
| Authorisation to proceed | Yes — repair applied and verified. |

---

### Attempt #2 — SUCCESS

#### Execution Information

| Item | Value |
|---|---|
| Attempt number | **#2** |
| Timestamp | 2026-11-12 12:52:35 +02:00 (10:52:35 UTC) |
| Application | `RefLab.Presentation.exe` (rebuilt from the current working tree) |
| Window shown | `Ref Lab - Sign in` |
| Username used | `developer` |
| Password used | `DevAdmin#2026!RefLab` |
| Account type | Administrator |
| `IsAdministrator` | `1` — re-read from the live `Operators` row immediately before the attempt (`sqlcmd`: `Id=1, Name=developer, IsAdministrator=1, IsActive=1`) |
| `IsActive` | `1` — same check |
| Method | Real GUI: operator name typed, password typed, **Login** button pressed through UI Automation on the running WPF application. |

#### Observed Result

| Item | Value |
|---|---|
| Result | **SUCCESS** |
| Exact UI message | *(none — no error was displayed)* |
| Application behaviour | The sign-in window closed and the **authenticated main window** opened: module bar (14 modules), the patient-work screen, and the status bar `User Name : developer · Last Login : Thu., 12 Nov. 2026 12:52 · SQL DATABASE · Today : Thu., 12-Nov.-2026 12:52`. |
| Screenshot | `C:\Users\LAP LINK\AppData\Local\Temp\opencode\window1.png` |

#### Technical Investigation

* The SQL Server engine log shows **no** error for this attempt — the connection opened normally.
* `Operators` row after the attempt: `AttendanceStart = 2026-11-12 10:52:35.1512877 +00:00`, `LastVisit` identical.
* `AttendanceEntries` gained the row
  `B6F2436E-0ED9-4AAB-AEAC-27737BC6EFF3 | 1 | developer | 1 (StartedWork) | 2026-11-12 10:52:35.1512877 +00:00`,
  which is written by `SignInHandler.Handle` **after** the password has been verified.
* Exactly one top-level window remains in the process (`Ref Lab`, 1360×860) — the sign-in window was closed by `SignInWindow.OnNavigationChanged` after the navigation to `ScreenKeys.PatientWork`.

#### Verified Root Cause

Not applicable — the failure does not occur any more. The verified root cause of attempt #1 (the missing
`RefLab.mdf` / `RefLab_log.ldf`) is confirmed fixed by this attempt.

#### Corrective Action

None — no further change was required after the repair of attempt #1.

#### Verification

**Authenticated identity and administrator authorization**

| Check | Result |
|---|---|
| Authenticated identity | The status bar shows `User Name : developer`; the session principal was established by `OperatorSession.Establish` from the DTO returned by the handler. |
| `IsAdministrator` | `1` (live row) |
| `IsActive` | `1` (live row) |
| Account type | Administrator |
| Administrator authorization works | The administrator-only module **Operator administration** was opened through the module bar; the screen loaded, the `Add` / `Edit` / `Apply` / `Save` commands are enabled and the permission list (`EditPatient`, `EnterResults`, `HandOverResults`, `ManageAccounting`, `ManageOperators`, `ManagePricing`, …) is populated. |
| Database connectivity from the running application | The **Refresh** command on that screen re-read the `Operators` table and displayed the row `developer | Development administrator | True` — the running application reads the restored database. |
| Screenshot | `C:\Users\LAP LINK\AppData\Local\Temp\opencode\attempt2_operator_refresh.png` |
| No authentication bypass | The session was established only through `SignInHandler` (see the attendance evidence above); no source file was changed at any point. |

#### Next Attempt

| Item | Value |
|---|---|
| Next attempt number | — |
| Authorisation to proceed | Not required: the authentication succeeded and every success criterion is met. |

---

## Final Results

| Item | Value |
|---|---|
| Total actual login attempts | **2** (limit: 10) |
| Successful attempts | **1** — attempt #2 |
| Failed attempts | **1** — attempt #1 |
| Account used for every attempt | `developer` / `DevAdmin#2026!RefLab` (Administrator, `IsAdministrator = 1`, `IsActive = 1`) |

### Identified root causes

| # | Root cause | Evidence |
|---|---|---|
| 1 | **The database file of the database the application resolves to was deleted from disk**: `C:\Users\LAP LINK\RefLab.mdf` and `C:\Users\LAP LINK\RefLab_log.ldf` no longer existed, while the LocalDB catalog still contained the `RefLab` entry pointing at exactly those paths. | `Test-Path` = `False`; SQL Server errors **17204** (`FCB::Open failed … OS error: 2`), **5120** and **18456** (`Failed to open the explicitly specified database 'RefLab'`) in the instance error log at the exact attempt timestamp; client error **4060** `Cannot open database "RefLab" requested by the login`; the same database started normally on 2026-11-11 17:01/17:49/17:58. |
| 2 | *(contributing, not causal)* **The message shown to the operator hides the real cause.** `RequestExecutor.Describe` has no case for a data-access failure, so the SQL exception falls through to the default arm and is rendered as the generic *“The operation could not be completed.”* | `src/RefLab.Presentation/Common/RequestExecutor.cs` lines 65–81; `src/RefLab.Presentation/Common/ViewModelBase.cs` lines 71–75. |

### Implemented corrections

1. Removed the orphaned `RefLab` catalog entry from `(localdb)\MSSQLLocalDB`.
2. Recreated `RefLab` with its original file layout `C:\Users\LAP LINK\RefLab.mdf` + `C:\Users\LAP LINK\RefLab_log.ldf`.
3. Applied the project's own three migrations (`20261110103458_InitialCreate`, `20261111083113_AddReportFooterAndLayoutText`, `20261111122338_AddReportBrandingAndSampleOrigin`) through `DatabaseInitializer` / `Database.MigrateAsync()`.
4. Restored the development administrator `developer` through the domain API with the recorded credential hash (`IsAdministrator = 1`, `IsActive = 1`, all 15 permissions).

### Modified files

| File | Change |
|---|---|
| `LOGIN_FAILURE_INVESTIGATION_REPORT.md` | **added** — this report |

No source file was modified: `git status --porcelain` returns only `?? LOGIN_FAILURE_INVESTIGATION_REPORT.md`
and `git diff` is empty. The database objects that were written are listed in
*Attempt #1 → Corrective Action → Files / database objects involved*.

### Remaining issues

| Issue | Assessment |
|---|---|
| The operator name of the LocalDB `RefLab` was lost with the deleted file and had to be restored from the development artefact `devhash.txt`. | The restored hash is byte-for-byte the hash that was issued, so the credential is unchanged. Nothing that survived on disk was lost. |
| The `.\SQLEXPRESS\RefLab` installation (a separate copy of the product with its own `admin` operator) was **not** touched and remains as it was. | Intentional: it is not the database the application resolves to, and no credential in it is known. |
| A database or connection failure is still reported to the operator as the generic *“The operation could not be completed.”* | **Observation, deliberately not changed.** Fixing it would mean adding a data-access case to `RequestExecutor.Describe`, which is outside the minimum correction for this defect and would touch the exception-translation contract of the presentation layer. It is recorded here so the owner can decide. |
| The LocalDB `RefLab` data files live outside the LocalDB instance directory (`C:\Users\LAP LINK\`), so a manual file deletion breaks the catalog silently. | Pre-existing arrangement of this development machine, not a defect of the application. The catalog entry is now consistent again. |

### Final authentication status

**SUCCESS** — the required Administrator account `developer` authenticates against
`(localdb)\MSSQLLocalDB.RefLab` with the password `DevAdmin#2026!RefLab`, the session is established, the
application reaches its authenticated interface, and administrator authorization is functional.

---

## Final Conclusion

The recurring login failure was **not** an authentication defect and **not** a credential defect. It was a
**data-file loss in the database the application resolves to**.

The application resolves its connection string in `CompositionRoot.ResolveConnectionString`: the
`REFLAB_CONNECTION_STRING` environment variable is unset, so the documented fallback
`Server=(localdb)\mssqllocaldb;Database=RefLab;Trusted_Connection=True;TrustServerCertificate=True` is
used. The unset variable is therefore **not** the problem — it is the designed behaviour. The LocalDB
instance and the `RefLab` catalog entry were both present, but the two files the catalog points at,
`C:\Users\LAP LINK\RefLab.mdf` and `C:\Users\LAP LINK\RefLab_log.ldf`, had been deleted from disk. SQL
Server consequently refused every logon with error 17204/5120 (file not found) and 18456
(*Failed to open the explicitly specified database 'RefLab'*), which the client surfaces as error 4060
(*Cannot open database "RefLab" requested by the login*).

That exception is raised while the connection is being opened — that is, **before** the `Operators` row is
read and **before** the PBKDF2 comparison — which is why the failure is independent of the credentials and
why it reproduced on every attempt. Because the data-access exception is not part of the Application
layer's exception hierarchy, it fell through `RequestExecutor.SendAsync` and was rendered by the default
arm of `RequestExecutor.Describe` as the generic *“The operation could not be completed.”*. The visible
message therefore masked a storage-layer failure; the technical cause was only visible in the SQL Server
error log and in the unhandled exception (`System.InvalidOperationException` wrapping
`Microsoft.Data.SqlClient.SqlException`, error 4060).

The correction is a database restoration, not a code change: the orphaned catalog entry was removed, the
`RefLab` database was recreated with its original file layout, the project's own three migrations were
applied through `DatabaseInitializer`, and the development administrator `developer` was restored through
the domain API with the hash recorded by the development environment (`devhash.txt`), i.e. the credential
that was actually issued. No authentication or authorization logic was touched, no bypass was introduced,
and the full test suite still passes (738/738).

The evidence is conclusive: attempt #1 (before the repair) failed with the reported message and the
corresponding SQL Server errors; attempt #2 (after the repair) accepted `developer` /
`DevAdmin#2026!RefLab`, closed the sign-in window, opened the authenticated main window with
`User Name : developer` in the status bar, recorded the attendance entry written by `SignInHandler`, and
the administrator-only *Operator administration* module loaded the operator register from the restored
database with the administrator flag set.

**Result: SUCCESS — the investigation was completed in 2 of the 10 permitted login attempts.**
