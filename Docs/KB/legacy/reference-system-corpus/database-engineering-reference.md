# Database Engineering Reference — RealLab CD 2019 v1.605

**Consolidated:** 2026-10-31 (UTC) · **Static analysis only.** No database was
contacted; no query was executed.

---

## 1. The two backends

| | Live clinical store | Offline mirror |
| --- | --- | --- |
| Technology | **Microsoft SQL Server**, database `Patients` | **Microsoft Access / Jet 4** |
| Provider | `SQLOLEDB.1`, wrapped by `MSDataShape.1` | `Microsoft.Jet.OLEDB.4.0`, wrapped by `MSDataShape` |
| Path | `Initial Catalog=Patients`, `Data Source=<server>` | `D:\real lab system\bin\Patients.mdb` |
| Credentials | `;User ID=` + `;Password=` fragments, or `Integrated Security=SSPI` | none (file-based) |
| Extra files | — | `Patients_backup.mdb`, `repairedDB.mdb` |
| Installed by | B4 `Setup4.bat` → SQL Server Express 2005 | — |
| ODBC/Jet versions | Jet 4.0 **SP2** (XP) / **SP3** (Win7); ADO **2.81** (XP) | same |

**The outer `MSDataShape` provider is the key architectural fact.** ADO's
Data Shape provider turns a **textual shape definition** into a recordset —
and B1 documented that shape vocabulary (`SELECT_1..SELECT_10`,
`SELECT_D1..D3`) in the B1 templates. **The shape definitions live in the
report templates.** The report subsystem and the data-access subsystem are
one thing in this product.

---

## 2. The embedded SQL corpus

| Class | Count |
| --- | ---: |
| `SELECT …` statements | **1,370** |
| `CREATE TABLE` | **14** |
| `ALTER TABLE` | **120** across **31** tables |
| Connection-string templates | **4** |

### 2.1 Most-queried tables

| Table | SELECT targets | Role | Corroborated by |
| --- | ---: | --- | --- |
| `patienttest` | **138** | the ordered tests per patient — the busiest table in the system | B1's schema |
| `normalrange` | **78** | reference intervals per test, sex and age band | B4's manual topic 11 |
| `patientinfo` | 65 | patient master record | B1's schema |
| `comments` | 28 | result comments | B1 |
| `testtable` | 26 | the test catalogue | B1 |
| `insulin` | 23 | a discipline result table | B1 |
| `Gehats` | 22 | the anti-HCV table | B1's `Gehats` |
| `payment` | 14 | billing | B1 |
| `antibiotic` | 13 | susceptibility | B1 |
| `patients` | 13 | the mirror's patient table | B1 |
| `subgrouptable` | 12 | test sub-groups | B1 |
| `phone` | 11 | contact numbers | B1 |
| `culturesec` | 10 | culture sensitivities | B1 |
| `titless` | 10 | test titles | B1 |
| `smslist` | 10 | SMS outbox | B1 |
| `sharekathesab` | 9 | referral accounting | B1 |
| `pricelist` / `barcodes` / `loggroup` / `testunits` | 7–8 | reference data | B1 |
| `patienttotal` | 7 | per-patient totals | B1 |
| `customgrouptable` / `Labbranch` / `seana` / `abbreviations` / `shopping` / `useraccounts` / `smsgroups` | 6 | administration | B1 |
| `loger` / `noteic` / **`wasl`** / `instrument` / `customtesttable` | 5 | audit, notes, **referral targets**, analysers, custom tests | B1 |
| `SMSSender` / `SMStemplate` / `testtemp` / `cultureanti` / `history` / `sharekat` / `constanttable` | 3–4 | SMS, temp results, patient history, constants | B1 |

**`normalrange` at 78 and `patienttest` at 138 are the system's hot path** —
every result is validated against a reference range before rendering.

---

## 3. The schema the executable creates and migrates

### 3.1 `CREATE TABLE` — 14 statements

```sql
Create Table testtemp (patientid varchar(13), …)
Create Table MyLabSetting (LabID Smallint, …)
Create Table smsgroups (ID bigint IDENTITY(1,1) NOT NULL, grouptype Smallint, smsgroup varchar(50));
Create Table smslist  (ID bigint IDENTITY(1,1) NOT NULL, smstype Smallint, smsphone varchar(20), smsname varchar(50), smsgroup varchar(50));
Create Table SMStemplate (templatenum bigint IDENTITY(1,1) NOT NULL, smstext varchar(MAX));
Create Table SMSSender (SMSautonum bigint IDENTITY(1,1) NOT NULL, patientid varchar(13),
                         PatDrName varchar(60), requestdate smalldatetime, smsMobile varchar(20),
                         SMSLength Smallint, SMStext varchar(MAX));
Create Table LBC (patientid varchar(13), requestdate smalldatetime, testid Smallint,
                   RESULT1 varchar(75), RESULT01 varchar(MAX), RESULT2 varchar(75), RESULT02 varchar(MAX), …)
Create Table IgEallergy (patientid varchar(13), requestdate smalldatetime, testid Smallint,
                         itemNo Smallint, Item1 varchar(50), RESULT1 varchar(25),
                         Class1 Smallint, TypeF1 Smallint, …)
```

Plus `pictures`, `Helicobacter`, `hba1c`, `FMF`, `widal2`, `ratios`.

**B1 recorded the first eight of these.** They match verbatim.

**Two structures are architecturally significant:**

* **`LBC` and `IgEallergy` are keyed by `testid Smallint` and `itemNo Smallint`.**
  This is the **numeric test-ID namespace** that B3's template filenames use
  and that B4's manual describes as the permanent *"Arrange number"*. **The
  executable, the templates and the manual all use the same key.**
* **`IgEallergy` carries `Class1 Smallint, TypeF1 Smallint`** — the IgE
  classification, matching the executable's `resultIgeAllergy` object (231
  methods) and B3's four `Data/164..167` allergy templates.

### 3.2 `ALTER TABLE` — 120 statements across 31 tables

| Table | Statements | Table | Statements |
| --- | ---: | --- | ---: |
| `pictures` | **84** | `BloodGases` | 8 |
| `cultures` | 22 | `Abuse` / `CSFT` / `Proelectrophoresis` | 4 each |
| `cbc` | 18 | `Helicobacter` / `IgEallergy` / `normalrange` / `testtable` / `useraccounts` | 3 each |
| `Patients` | 14 | `Gehats` / `Kidney` / `patienttest` / `pregnancy` / `ratios` / `semen` / `smslist` / `testtemp` / `urine` | 2 each |
| `wasl` | 12 | `cultureanti` / `lipids` / `payment` | 1 each |
| `prostaticx` | 12 | | |
| `patientinfo` | 8 | | |

Representative statements:

```sql
ALTER Table Patients ADD grant13 Bit
ALTER Table Patients ADD COLUMN grant14 YESNO
ALTER Table cultures ALTER COLUMN organism varchar(80)
ALTER Table patientinfo ADD gehaname varchar(50)
ALTER Table testtemp ALTER COLUMN normalrang MEMO
ALTER Table useraccounts ADD HDD1 varchar(20)
ALTER Table semen ADD testcode Smallint
ALTER Table SMSSender ALTER COLUMN smsMobile varchar(100)
```

**The client mutates the server's schema at run time.** There is no
migration script, no versioning table and no audit trail. See U-004.

---

## 4. The reference-range model

Reconstructed from the executable's DDL and literals, and corroborated by B4's
manual topic 11:

| Attribute | Stored as |
| --- | --- |
| Test | `testid` (Smallint) |
| Unit | `Test unit` — printed next to the result |
| Sex | `Sex` |
| Age band | `Age From` / `Age To` + `Age unit` |
| Reference text | `Normal range` — printed in the report's reference field |
| Limits | `Low limit` / `High limit` — what makes a result abnormal |
| Flags | `Low flag` / `High flag` — the expression shown for an abnormal result |
| Comments | `Low comment` / `High comment` / `Critical comment` |
| Critical range | `Critical range` — printed only when "print critical ranges" is selected |
| Critical limits | `Critical limit` / `High limit` |

**The same limits drive the on-screen entry window and the printed report.**
That is the mechanical link between `normalrange` and B3's
`[reportdata]` / `[FORM]` template sections, and it is why the manual's
report-settings window exposes a *Print Critical Ranges If Found* switch.

---

## 5. The authorisation columns

`Patients` is migrated with `grant13`…`grant17` as `Bit` / `YESNO`, and
`grant1`…`grant12` appear as field names. `patientinfo` carries `discount`,
`discount100` and a `Discount=` literal. The natural reading is a set of
**per-patient price/discount grants** — but that is an **inference**, not a
determination (UNR-002).

---

## 6. The operator store

```sql
, accuntID varchar(50)
, accuntpw varchar(50), updateflage BYTE
ALTER Table useraccounts ADD HDD1 varchar(20)
ALTER Table useraccounts ADD HDD2 varchar(20)
ALTER Table useraccounts ADD Gtype Smallint
```

with login queries:

```sql
SELECT * FROM useraccounts where usersdr= '
SELECT * FROM useraccounts where usersnum=
Select * from useraccounts where usersdr like '
```

**No hash column and no salt column exist.** See U-003.

---

## 7. The SMS data path

```sql
Select * from MyLabSetting WHERE LabID=100 ;
```

returns `SMSbakey`, `sendername`, `accuntID`, `accuntpw`, which are
interpolated into the plain-HTTP URL. **The gateway credential is stored in
the clinical database and transmitted in a URL.** See U-002.

---

## 8. Confirmed versus inferred schema

| **CONFIRMED from the binary** | **INFERRED** |
| --- | --- |
| The 14 `CREATE TABLE` and 120 `ALTER TABLE` statements, verbatim | Which of them run on every start |
| The `testid`/`itemNo` numeric keying | The full table inventory of the mirror |
| The `useraccounts` column types | The full column set of every table |
| The `MyLabSetting` gateway fields | Which object issues each of the 1,370 SELECTs |
| The four connection templates | The complete `normalrange` population |
| The 1,370 SELECT statements and their target tables | The relationship between the cache ID namespace and the template IDs |
| The `Patients.grantN` column names and types | **What** `grantN` means |

---

## 9. Open database questions

| Question | Status |
| --- | --- |
| The full schema of every table | **PARTIAL** — only the DDL the executable carries is known |
| The Access mirror's own file and schema | **UNAVAILABLE** — not in any investigated portion |
| Which object issues each SELECT | **OPEN** — UNR-008, needs decompilation |
| The `grantN` semantics | **OPEN** — UNR-002 |
| Whether `accuntpw` is hashed | **OPEN** — UNR-005 |
| How `Cache/<n>.rtf` binds to a test | **OPEN** — a separate numeric namespace (006–500) that does not reconcile with the template IDs |
| `Data/REPORT.ini` | **ABSENT from every portion** |
