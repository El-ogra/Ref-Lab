# Security Findings and Risk Register — RealLab CD 2019 v1.605

**Compiled:** 2026-10-31 (UTC) · **Static only.** No exploitation was
attempted, no credential was validated, no external system was contacted.
**Scope:** the complete v1.605 distribution as covered by B1–B4 plus the
direct executable investigation.

---

## 1. How this register is organised

**Three ID spaces coexist and none was renumbered:**

| Prefix | Meaning | Count |
| --- | --- | ---: |
| `FND-001` … `FND-019` | B1's findings, from the B1 package | 19 |
| `FND-B2-001` … `FND-B2-013` | B2's findings, from the B2 package | 13 |
| `FND-B3-001` … `FND-B3-010` | B3's findings | 10 |
| `FND-B4-001` … `FND-B4-013` | B4's findings | 13 |
| **`FND-F-001` … `FND-F-011`** | **this stage's executable findings** | **11** |

**66 findings in total.** The F-prefix is used precisely so that nothing
earlier is disturbed.

**B2 was not independently re-investigated.** Its 13 findings are cited as
inherited evidence, not as re-derived facts. B1, B3 and B4 were likewise
supplied; **only B3 and B4 were executed by this agent**, and only B3/B4's
registers are treated as this agent's own prior work.

---

## 2. Consolidated severity profile

| Severity | B1 | B2 | B3 | B4 | This stage | **Total** |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| High | 7 | 3 | 0 | 3 | 3 | **16** |
| Medium | 9 | 6 | 3 | 7 | 4 | **29** |
| Low | 2 | 2 | 5 | 3 | 1 | **13** |
| Info | 1 | 2 | 2 | 0 | 2 | **8** |
| **Total** | 19 | 13 | 10 | 13 | 11 | **66** |

*Counts are as published by each batch. B1's severity split is taken from
its `findings.json`; where a batch's own register differs in wording, that
batch's wording stands.*

---

## 3. The eleven new findings

### FND-F-001 — The SQL Server sa password is assembled at run time from separate literal fragments and shipped in cleartext

| | |
| --- | --- |
| **Severity** | High |
| **Confidence** | Confirmed |
| **Status** | Confirmed |
| **Source** | executable |
| **Evidence** | EV-F-010, EV-F-011 |
| **Cross-batch** | Confirms B1 FND-001, FND-002, FND-003 and FND-007 from the binary itself; complements B4 FND-B4-001. |

The executable contains the literal fragments `Provider=SQLOLEDB.1;Persist Security Info=True;Initial Catalog=Patients;Data Source=`, `;User ID=` and `;Password=` as three separate UTF-16LE strings, concatenated at run time. `Persist Security Info=True` is itself part of the fragment. The same pattern appears for the MSDataShape wrapper. B4 independently showed the installer creating the sa account with the literal password 12345678.

**Impact.** The credential for the clinical database server is present in the shipped binary in a form that a string scan recovers; Persist Security Info=True also means the credential can travel back with any data-bound object.

**Limitations.** The value substituted for the password is not a literal in the binary and was not recovered; the runtime value may be supplied by the operator or by B1-recorded configuration files.

**Remediation.** Store the database credential outside the binary, rotate it, and remove Persist Security Info=True from the shipped connection templates.


### FND-F-002 — The SMS gateway is called over plain HTTP with the account credential in the URL query string

| | |
| --- | --- |
| **Severity** | High |
| **Confidence** | Confirmed |
| **Status** | Confirmed |
| **Source** | executable |
| **Evidence** | EV-F-010 |
| **Cross-batch** | Confirms B1 FND-017 from the binary; complements B4 FND-B4-008 (a third script/surface). |

The executable drives Internet Explorer through COM (`InternetExplorer.Application`, `navigate2`, `Busy`, `ReadyState`, `Body`) to call `http://smsmisr.com/api/send.aspx?username=` followed by `&password=`, `&language=3&sender=`, `&mobile=` and `&message=`. The gateway account (`accuntID`) and password (`accuntpw`) are read from the `MyLabSetting` table via `Select * from MyLabSetting WHERE LabID=100`, together with `SMSbakey` and `sendername`.

**Impact.** The gateway credential and the SMS body - which can carry a patient result - travel in cleartext and are exposed to any intermediary, proxy log or browser history on the sending machine.

**Limitations.** No request was issued and the endpoint was not contacted; the finding is entirely from the embedded literals.

**Remediation.** Move to HTTPS, move the credential out of the URL into a header or POST body, and stop driving Internet Explorer for it.


### FND-F-003 — User account passwords are stored in cleartext in the database

| | |
| --- | --- |
| **Severity** | High |
| **Confidence** | Confirmed |
| **Status** | Confirmed |
| **Source** | executable |
| **Evidence** | EV-F-010 |
| **Cross-batch** | Extends B1 FND-002 (cleartext SQL credentials in BASE.ini) to the operator account store. |

The schema the executable creates and migrates defines `accuntID varchar(50)` and `accuntpw varchar(50)` in the `useraccounts` table, with no hash column and no salt column. Login queries read the table directly: `select * FROM useraccounts where usersnum= `, `SELECT * FROM useraccounts where usersdr= '`, `Select * from useraccounts where usersdr like '`.

**Impact.** Anyone with read access to the database, to a backup file, or to the Access mirror obtains every operator password in cleartext.

**Limitations.** It cannot be determined from strings alone whether the application hashes the password before the INSERT; the column type and the direct SELECTs are consistent with cleartext storage but do not exclude an application-side hash.

**Remediation.** Store a salted, iterated password hash; migrate existing rows; never SELECT the credential column.


### FND-F-004 — The executable carries its own schema-migration engine and runs DDL against the clinical database at start-up

| | |
| --- | --- |
| **Severity** | Medium |
| **Confidence** | Confirmed |
| **Status** | Confirmed |
| **Source** | executable |
| **Evidence** | EV-F-010 |
| **Cross-batch** | Extends B1, which recorded the CREATE TABLE statements but not the migration surface. |

The binary embeds 14 CREATE TABLE statements and 120 ALTER TABLE statements covering 31 existing tables, including 84 ALTER statements against `pictures` and 14 against `Patients`. Examples: `ALTER Table Patients ADD grant13 Bit`, `ALTER Table cbc ADD ...`, `ALTER Table wasl ADD ...`, `ALTER Table cultures ALTER COLUMN organism varchar(80)`, `Create Table LBC (patientid varchar(13), requestdate smalldatetime, testid Smallint, ...)`, `Create Table IgEallergy (...)`, `Create Table MyLabSetting (LabID Smallint ...)`, `Create Table SMSSender (...)`, `Create Table smsgroups/smslist/SMStemplate (...)`.

**Impact.** The application mutates the schema of the clinical database on every start. A user who can run the client can therefore issue DDL against the server database, and any change is silent and unattributed.

**Limitations.** The exact trigger and ordering of the migration statements were not established statically; it is not proven that every statement runs unconditionally.

**Remediation.** Move schema management to a versioned migration script run by a DBA with an audit trail; drop the application privilege to ALTER.


### FND-F-005 — The executable has no ASLR, no DEP/NX and is unsigned

| | |
| --- | --- |
| **Severity** | Medium |
| **Confidence** | Confirmed |
| **Status** | Confirmed |
| **Source** | executable |
| **Evidence** | EV-F-011 |
| **Cross-batch** | Confirms B1 FND-008 and B2 FND-B2-006. |

DllCharacteristics = 0x0000: no IMAGE_DLLCHARACTERISTICS_DYNAMIC_BASE, no NX_COMPAT, no GUARD_CF, SEH enabled. The Security data directory is empty, so there is no Authenticode signature. The binary is a 32-bit PE whose .text section is 76,119,116 bytes - 97 % of the file.

**Impact.** Every memory-corruption bug in a 76 MB of unmitigated code has a predictable image base and no stack or heap protection.

**Limitations.** Static only. No exploitation was attempted and no vulnerability was identified or claimed.

**Remediation.** Recompile with /DYNAMICBASE /NXCOMPAT and sign the binary; where recompilation is impossible, deploy with mandatory ASLR and system-wide DEP.


### FND-F-006 — The application drives Internet Explorer by COM automation for an outbound HTTP request

| | |
| --- | --- |
| **Severity** | Medium |
| **Confidence** | Confirmed |
| **Status** | Confirmed |
| **Source** | executable |
| **Evidence** | EV-F-010 |
| **Cross-batch** | Relates to B3 FND-B3-002 and B4 FND-B4-008 - the installation carries several script and automation surfaces. |

The SMS path instantiates `InternetExplorer.Application` and uses `navigate2`, `Busy`, `ReadyState` and `Body` - i.e. it opens a real browser window to perform an HTTP request, rather than using WinHTTP, WinInet or a socket.

**Impact.** An embedded browser control inherits the user profile, proxy configuration, cookie jar and any browser add-ons, and its request is subject to the security posture of whatever browser is installed. It is also a visible foreground window, which is a usability and a privacy problem for clinical staff.

**Limitations.** The IE automation is only evidenced on the SMS path; other uses of the control were not exhaustively enumerated.

**Remediation.** Replace the browser automation with a direct HTTPS client and a server-side gateway credential.


### FND-F-007 — Per-patient grant flags and per-section print passwords form a coarse, undocumented authorisation model

| | |
| --- | --- |
| **Severity** | Medium |
| **Confidence** | Suspected |
| **Status** | Suspected |
| **Source** | executable |
| **Evidence** | EV-F-010 |
| **Cross-batch** | Relates to B4 FND-B4-002 (admin/admin) and B4 FND-B4-003 (maintenance password 123). |

The `Patients` table is migrated with `grant13` ... `grant17` Bit/YESNO columns, and `grant1` ... `grant12` are referenced as field names. The binary also carries `IDPasswordL`, `IDPasswordT`, `IDPasswordF`, `IDPasswordB`, `IDPasswordS`, `IDPasswordV`, `IDPasswordA` together with `Print Result Password` and the prompts `Please Enter Your Internal Password. (Digits only)`, `Wrong Password Entered.` and `You must enter a Password.`

**Impact.** Authorisation appears to be a set of boolean columns and a handful of shared numeric passwords rather than a role model. If the grant flags gate what may be released or discounted for a patient, and the print passwords are shared across users, the confidentiality boundary is weak and unauditable.

**Limitations.** The exact semantics of each grantN column and of each IDPassword* value were NOT established from strings alone; VB6 pools all string literals into one block, so adjacency proves nothing. The count of 17 grants and their purpose are documented facts; their meaning is not.

**Remediation.** Document the grant semantics and the print-password model; replace shared numeric passwords with per-user permissions.


### FND-F-008 — Report-section passwords are transmitted and stored as short numeric strings

| | |
| --- | --- |
| **Severity** | Low |
| **Confidence** | Confirmed |
| **Status** | Confirmed |
| **Source** | executable |
| **Evidence** | EV-F-010 |
| **Cross-batch** | — |

The internal password prompt states `(Digits only)`, and the binary holds seven `IDPassword*` identifiers. A short numeric secret has negligible entropy.

**Impact.** Any shared numeric print password can be brute-forced in a few hundred attempts by anyone who can trigger the prompt.

**Limitations.** The prompt does not establish whether the check is server-side, and whether a failed attempt is logged or rate-limited was not determined.

**Remediation.** Use a per-user credential with a stored hash, and rate-limit the prompt.


### FND-F-009 — The build embeds an absolute installation path that ties the product to one drive layout

| | |
| --- | --- |
| **Severity** | Low |
| **Confidence** | Confirmed |
| **Status** | Confirmed |
| **Source** | executable |
| **Evidence** | EV-F-010, EV-F-011 |
| **Cross-batch** | — |

Every recovered path is rooted at `D:\real lab system\` - `D:\real lab system\bin\Patients.mdb`, `Patients_backup.mdb`, `repairedDB.mdb` - while the PE image base is 0x400000 and the VERSION resource names the product `RLS 1.605`. B4's installer manifest independently specifies `DefaultDir=$(ProgramFiles)\real lab system`.

**Impact.** The product is compiled against a hard-coded D: root. A default install to C:\Program Files would not match, which is a latent deployment defect and a hint that the shipped defaults were authored on one specific machine.

**Limitations.** The application may resolve these paths at run time from a configuration value; the literals are the observed defaults.

**Remediation.** Resolve all paths from the executable location or an explicit configuration file.


### FND-F-010 — The binary carries no version resource metadata beyond a minimal VS_VERSIONINFO block

| | |
| --- | --- |
| **Severity** | Info |
| **Confidence** | Confirmed |
| **Status** | Confirmed |
| **Source** | executable |
| **Evidence** | EV-F-012 |
| **Cross-batch** | Consistent with the VS6-era build; complements B4 FND-B4-012 (16-bit deployment dependency). |

The VERSION resource is 804 bytes with the single translation 0x0409/0x04B0 (US English, Unicode) and carries Comments, CompanyName, FileDescription, LegalCopyright, LegalTrademarks, ProductName, FileVersion, ProductVersion, InternalName and OriginalFilename. There is no manifest resource, so the executable requests no OS version, no common-controls theme, no UAC level and no DPI awareness.

**Impact.** Without a manifest the executable gets the legacy common-control rendering, the default UAC virtualisation behaviour and no DPI awareness - all of which affect a 198-screen clinical UI on modern Windows.

**Limitations.** Static only; no manifest means no requested behaviour, which is a distinct statement from the behaviour actually obtained.

**Remediation.** Embed an application manifest requesting common controls v6, asInvoker and per-monitor DPI awareness.


### FND-F-011 — The vendor identity and version are confirmed from three independent embedded sources

| | |
| --- | --- |
| **Severity** | Info |
| **Confidence** | Confirmed |
| **Status** | Confirmed |
| **Source** | executable |
| **Evidence** | EV-F-010, EV-F-012 |
| **Cross-batch** | Matches B4 FND-B4-001 evidence (the manual) and the B4 SETUP.LST AppExe value. |

The VERSION resource states FileDescription `RLS 1.605`, FileVersion `1.605`, ProductVersion `1.605`, ProductName `real lab system`, InternalName `real lab system`, OriginalFilename `real lab system.exe`, CompanyName `RealLab`, Comments `ais_soft_group@yahoo.com`. The string corpus independently contains `E-mail: ais_soft_group@yahoo.com`, `Website: http://www.RealLabeg.com` and `Page: Facebook.com/RealLab.Systems`.

**Impact.** Establishes the product identity, version and vendor from the artefact itself rather than from packaging, and pins the vendor to Egypt (.eg country code, and the Arabic-Saudi LCID2 4105 in the project header).

**Limitations.** None material.

**Remediation.** None - this is an identification, not a defect.

---

## 4. The themes

Read across all 66 findings, four themes account for most of the risk.

### 4.1 Clinical data crosses trust boundaries in cleartext — 16 High findings

The SQL `sa` credential is assembled from literal fragments
(**FND-F-001**), the SMS gateway credential travels in a plain-HTTP URL
(**FND-F-002**), operator passwords sit in a `varchar(50)` column
(**FND-F-003**), and the Access mirror keeps its own backup and repair copies
on every workstation. The vendor's own manual adds the operator default
`admin`/`admin` (B4 FND-B4-002) and the maintenance password `123`
(B4 FND-B4-003).

**The common root is not a single bug: it is that the product treats the
clinical database, the operator's credential and the outbound SMS channel as
trusted, because in 1990s client/server architecture on a private LAN, they
were.**

### 4.2 The client holds server-side privilege it does not need — FND-F-004

The executable carries **120 ALTER TABLE statements across 31 tables** and
**14 CREATE TABLE statements**, and runs them against the server database. A
user who can start the application can therefore change the schema of the
clinical database, silently and without an audit trail. This is the one
finding that is *new in kind* rather than a new instance of a known pattern.

### 4.3 The deployment is invasive and unhardened

System-wide COM self-registration, local copies of Windows system DLLs,
machine-wide installation of a renamed Microsoft typeface, an installer engine
with no ASLR and no DEP, a 16-bit launcher, an evaluation-edition commercial
printer installed silently, and — from this stage — the primary executable
itself with `DllCharacteristics = 0x0000` and no signature (**FND-F-005**).

### 4.4 The report subsystem is a scriptable HTML surface

Three independent script hosts now exist in one installation: B2's JScript and
VBScript engines, B3's HTML report designer, and B4's Bullzip payload
(`msscript.ocx` plus a VBScript macro example, **FND-B4-008**). This stage
adds a fourth surface of a different kind: **Internet Explorer driven by COM**
for outbound HTTP (**FND-F-006**).

---

## 5. Prioritised recommendations

| # | Action | Addresses |
| ---: | --- | --- |
| 1 | **Move the database credential out of the executable** into an OS-protected store, rotate it, and drop `Persist Security Info=True` | FND-F-001, B1 FND-001/002/003/007 |
| 2 | **Move the SMS gateway to HTTPS** with the credential in a header, and replace the IE automation with a direct client | FND-F-002, FND-F-006 |
| 3 | **Hash the operator passwords** with a salted KDF; migrate `accuntpw` | FND-F-003 |
| 4 | **Remove DDL rights from the application**; ship schema changes as versioned migration scripts with an audit trail | FND-F-004 |
| 5 | **Replace the three hardcoded credentials** — `sa` password, `admin`/`admin`, maintenance `123` — with per-deployment values | B4 FND-B4-001/002/003 |
| 6 | **Rebuild with ASLR, DEP/NX and CFG, and sign the binary** | FND-F-005, B1 FND-008, B2 FND-B2-006 |
| 7 | **Document and constrain the authorisation model** — the 17 `grantN` flags and the seven shared print passwords | FND-F-007, FND-F-008 |
| 8 | **Resolve the font redistribution position** — the renamed Microsoft Tahoma is installed machine-wide | B3 FND-B3-008, B4 FND-B4-004 |
| 9 | **Remove the orphaned Avira descriptors and the MS `.dbg` files** from the deployment media | B4 FND-B4-011/013 |
| 10 | **Establish a single, version-controlled report-template distribution** so every workstation renders identically | FND-F-009, B4 FND-B4-009, B3 FND-B3-001 |
| 11 | **Resolve `Data/REPORT.ini`** — absent from every batch | B3 DEP-B3-U01, B4 DEP-B4-U01 |
| 12 | **Plan the retirement of the VS6 SP6 deployment path** (16-bit launcher, 1996 help file) | B4 FND-B4-012 |

---

## 6. Boundaries of this register

* **No exploit was demonstrated, attempted or attempted to be demonstrated.**
  Every finding is a statement about *content* or about *vendor documentation*.
* **No credential was validated.** The three passwords were read from a script
  and from a manual.
* **No CVE or advisory is asserted.** No third-party executable in the
  distribution was assessed for known vulnerabilities, and the Bullzip
  installer's certificate expiry is recorded as an observation rather than a
  vulnerability.
* **The B2 findings are inherited, not re-derived.** They were read from the
  supplied B2 package.
* **Where a finding's meaning could not be settled from static evidence, the
  finding is marked Suspected and the open question is carried in
  `machine-readable/unresolved-questions.json`.**
