# Legacy Security — V-ROOT-1.00 and V-CD-2019

**Purpose:** Curated security findings and vulnerabilities of the legacy systems.
**Scope:** Credentials, permissions, audit, authentication, known vulnerabilities.
**Audience:** Developers, security reviewers.
**Version applicability:** Most findings are SHARED; V2 adds new vulnerabilities.

---

## 1. User and permission structures

### V1
- `useraccounts` — user account records and permission flags.
- `loggroup` — login group (associates a session with a group/branch).
- `users` form (29 controls) — user administration.
- `userspw` form (13 controls) — password change.
- Default administrative account `admin`, password equal to user name, documented in the manual
  (CHM `002.htm`: «لأول مرة اختر admin … أدخل نفس الكلمة ككلمة مرور»). Literal `admin` at
  executable offset `0x7BA378`.
- **12 function-level permission bits** (`grant1..grant12`) on the operator table (`patients`).
- Operator attendance recorded on the operator record itself (`startwork`, `endwork`, `breakyorn`,
  `breaktime`).
- Login group is the unit of session scoping and workload attribution, not a role hierarchy
  (V1-BR-032, INFERRED).

### V2
- `useraccounts` — **not present in any `.mdb`** (SQL Server only); referenced by DDL with
  `Gtype`, `HDD1`, `HDD2`.
- `loggroup` — unchanged.
- **`loger`** — new attendance/event log table.
- `patients` operator table: **26 columns (V1) → 35 columns (V2)** — adds `branche`, `SecWord`,
  `grant13..grant17`.
- **17 function-level permission bits** (`grant1..grant17`) + `branche` + `SecWord`.
- `users` form: 57 methods in V2.
- `userspw` form: password change.

**Evidence:** external: Complete_Reverse_Engineering_Reference_V1.md §8.1; `Docs/Complete_Reverse_Engineering_Reference_V2.md` §8.1.

## 2. Access-control mechanism

| Layer | Mechanism | Evidence |
|---|---|---|
| Application | `useraccounts` rows + `loggroup`; login form gates entry | CHM `002.htm`; `login`, `users`, `userspw` forms |
| Database connection | A **fixed** database user name and password, identical on every client, set at install time and documented as unchangeable | CHM `001.htm` — «ويمنع تغيرها» |
| Server reachability | Server identified by IP or machine name; client must be permitted to reach it | CHM `001.htm` |
| Branch scoping | `SETTING.ini [setup] labbranchx=1` | `SETTING.ini` |
| Table-level row security | **NOT ESTABLISHED** — no row-level or column-level security evidenced | — |

## 3. Authentication mechanism (both versions)

- Login window presenting user name and password.
- Default administrative account `admin`, initial password equals the user name.
- Database connection identity is **shared across all client machines** and documented as immutable.
- **No evidence** of password hashing, salting, expiry, lockout, or session timeout.

## 4. Vulnerability register

| ID | Finding | Evidence | Classification | Version |
|---|---|---|---|---|
| V-01 | Database password `12345678` stored in clear text in shipped config (`BASE.ini`, `data_connection1.udl`) and in the installer (`SAPWD=12345678`) | `BASE.ini` `Password=12345678`; `data_connection1.udl`; `Setup.bat`/`Setup4.bat` | CONFIRMED | both |
| V-02 | Application connects as administrative account `sa` (full DB admin rights) on the SQL Server path | `BASE.ini` `UserID=sa`; UDLs `User ID=sa` | CONFIRMED | both (SQL path) |
| V-03 | Default `admin` account with password equal to user name, documented in shipped manual | CHM `002.htm`; literal `admin` @ `0x7BA378` | CONFIRMED | both |
| V-04 | `PersistSecurityInfo=True` — credential re-supplied on every connection | `BASE.ini`; `data_connection1.udl` | CONFIRMED | both |
| V-05 | Connection strings leak pre-rebranding product identity and install layout | `Initial Catalog=C:\PROGRAM FILES\AUTOMATIC HEMATOLOGY ANALYZER\…\BF6500.DBMDF`; `Initial File Name=D:\Lab_data\bf6500.db.mdf` | CONFIRMED | both |
| V-06 | Cleartext laboratory data path | `loca=E:\Lab_data`; `D:\Lab_data\*.mdf` | CONFIRMED | both |
| V-07 | Shared, immutable database credential across all clients | CHM `001.htm` (items 2–4) | CONFIRMED | both |
| V-08 | Vendor contact and site details embedded in shipped config file | `IDCARD.INI` (address, phone, website, e-mail, Facebook) | CONFIRMED | both |
| V-09 | Patient barcode is the direct patient identifier on a physical artefact | `barcodes` table; `barcode` form; `IDCARD.INI` | CONFIRMED | both |
| V-10 | `history` uses 20 free-form columns as generic attribute store — no type enforcement | `data.mdb` `history` | CONFIRMED | both |
| V-11 | Manual advises not connecting the server to the internet to preserve confidentiality | CHM `001.htm` — «يفضل عدم توصيل انترنت على جهاز السيرفر … للحفاظ علي سرية البيانات» | CONFIRMED | both |
| V-12 | V2 `BASE.ini [lastlog]` logs last 10 connection attempts including password in cleartext, with workstation name | `BASE.ini` V2 | CONFIRMED (F-09) | V2 ONLY |
| V-13 | `SMSSender` stores mobile numbers and message bodies in clear text, indefinitely, no retention limit | `SMSSender` schema (14 cols) | CONFIRMED | V2 ONLY |
| V-14 | NATIGH digital delivery requires an outbound internet channel the manual advises against (internal contradiction) | Vendor guide 1-12; CHM `001.htm` | CONFIRMED (contradiction) | V2 ONLY |
| V-15 | Hardware-bound licence (HDD + motherboard + CPU) | `RL_SETUP.exe` strings; `Win32_Processor` query | STRONGLY SUPPORTED | V2 ONLY |
| V-16 | No password hashing, salting, expiry, lockout, or session timeout evidenced | CHM + schema | CONFIRMED (absence) | both |
| V-17 | Table-level row security not established | — | NOT ESTABLISHED | both |
| V-18 | The **database-maintenance password defaults to `123`**, is published in the shipped manual, and guards system-zeroing and restore | CHM `backup.htm` — «قم بإدخال كلمة المرور الاساسية 123 ثم موافق … ويفضل تغير كلمة المرور بعد ذلك لضمان حماية النظام من العبث» | CONFIRMED | both |
| V-19 | The SMS gateway is called over **plain HTTP with the credentials as query-string parameters**, through browser automation (`InternetExplorer.Application`) — a third-party provider is inside the trust boundary | `session-2026-10-31-executable.log`; `txt nd sql/reallab_copy_log.txt` | CONFIRMED | V2 ONLY |
| V-20 | The branch code occupies **position 9 of the 13-character `patientid`**, so the identifier alone discloses the branch | `data/IDENTIFIERS.md`; `patientinfo.patientid` schema | CONFIRMED | V2 ONLY |

**★ Architectural assessment (V1):** security model is **application-level user accounts over a shared,
statically-credentialed database connection**. No encryption at rest, no per-user database identity,
no password hashing, no network transport protection. The manual itself acknowledges exposure by
recommending isolation from the internet.

**Evidence:** external: Complete_Reverse_Engineering_Reference_V1.md §8.5; `Docs/Complete_Reverse_Engineering_Reference_V2.md` §8.1; external: 11-deployment-payload-2019.md §3 (F-09).

## 5. User-related workflows

1. User administrator creates the account and assigns it to a login group.
2. At each session the operator authenticates at the login window.
3. `loggroup` records the group; `labbranchx` scopes the session to a branch.
4. `labfunction` (accounting) aggregates per-user workload and per-user collections.

## 6. Security-relevant technical findings (V1, 11 items)

RL-V1-SEC-01..11: clear-text DB password (two shipped files), `sa` admin identity, default `admin`/
`admin` account, `PersistSecurityInfo=True`, connection strings leaking pre-rebranding product identity
and install layout, cleartext data path, shared immutable credential, vendor contact in config, barcode
as direct identifier, `history` EAV without type enforcement, manual advises no internet on server.

### 6.1 Maintenance and backup exposure (both versions)

The database-maintenance screen is reached via **«اعدادات» → «Database Maintenance»** and is
protected by a **maintenance password whose documented default is `123`**. The manual advises
changing it *after* the first use, but the shipped default is a three-character literal published
in the product's own help file. The screen exposes the most destructive operations in the product:
zeroing all or part of the system, and restoring a backup — including restoring a single patient by
their work code (كود الشغل).

Backup files are named **`Patient_yyyymmdd.bak`** and are written **inside the database path**; the
manual advises moving them to a separate disc or machine.

**Evidence:** CHM `backup.htm`; `help-manual-ar.md` §14.

## 7. NATIGH / SMS delivery (V2) — security notes

- NATIGH.COM publication and SMS notification require outbound internet — contradicts the manual's
  advice to keep the server off the internet (V2-BR-006, internal contradiction).
- `SMSSender` stores mobile number + message body in cleartext, indefinitely, no retention limit
  (V2-BR-007). In a laboratory context a mobile number is a personal identifier.
- NATIGH delivery flag (`patienttest.natighu`) is DDL-defined but absent from the shipped DB
  (V2-BR-005) — feature is code-present but schema-absent.
- NATIGH accounts are provisioned for external parties (physician and laboratory) (V2-BR-022).

## 8. Modern Ref Lab security model (contrast)

The modern system deliberately departs from every legacy vulnerability:
- PBKDF2-HMAC-SHA256 (210,000 iterations, 16-byte random salt, constant-time compare) —
  `src/RefLab.Infrastructure/Security/Pbkdf2PasswordHasher.cs`.
- Environment-variable-driven connection strings (`REFLAB_CONNECTION_STRING`); no `appsettings.json`.
- No SMS, NATIGH, online publishing, WhatsApp, or email-sending (explicitly excluded and guarded
  by 6+ architecture tests in `PresentationLayerBoundaryTests.cs`).
- No patient barcode as identifier (GUIDs used).
- Typed `PatientHistory` slots (no 20-column EAV).
- Print audit preserved (`PrintAudits` table: `Operator`, `PrintedAt`, `SequenceNumber`, `Category`).
- Permission model: 15 permissions, enforced via `IRequirePermission` + `AuthorizationBehavior`.
- No hardware-bound licensing.

**Evidence:** Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md §A.8, §A.10; `Docs/KB/ARCHITECTURE.md`; `src/RefLab.Domain/Identity/Permission.cs`.

## 9. Version applicability

- Credential model (`sa`/`12345678`, `PersistSecurityInfo`, shared credential): **SHARED**.
- `lastlog` cleartext logging, SMS/NATIGH data, hardware-bound licence: **V2 ONLY**.
- `admin`/`admin` default account, no hashing/lockout: **SHARED**.
- Permission bits 13–17, `branche`, `SecWord`: **V2 ONLY**.

## 10. Related documents

- `Docs/KB/legacy/data/IDENTIFIERS.md` — identifier contracts.
- `Docs/KB/legacy/business/WORKFLOWS.md` — workflows.
- `Docs/KB/legacy/evidence/CONTRADICTIONS.md` — contradictions (C-01, etc.).
- `Docs/KB/legacy/mapping/LEGACY-TO-MODERN.md` — legacy vs modern.
- Original source evidence: external: Business_Rules_V1.md (external), `Docs/Business_Rules_V2.md`,
  external: Complete_Reverse_Engineering_Reference_V1.md (external), `Docs/Complete_Reverse_Engineering_Reference_V2.md`,
  external: 11-deployment-payload-2019.md.
- `Database documentation.md`, `Documenting the Executable File.md` (root).
- Modern implementation: `src/RefLab.Domain/Identity/*`, `src/RefLab.Infrastructure/Security/*`,
  `src/RefLab.Application/Behaviors/*`.
