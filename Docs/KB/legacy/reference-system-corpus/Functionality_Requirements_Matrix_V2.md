# Functionality and Requirements Matrix — RealLab V-CD-2019 (v1.605)

**Document ID:** RL-V2-FRM-001
**Version documented:** `V-CD-2019`
**Evidence basis:** 473 verified artefacts · static analysis only
**Companion documents:** `Functional_Specification_V2.md`, `Business_Rules_V2.md`, `Data_Model_and_Workflow_Entities_V2.md`

---

## 1. Method, conventions and reading guide

### 1.1 Identifier scheme

| Prefix | Scope |
|---|---|
| `V2-FUNC-nnn` | Function |
| `V2-REQ-nnn` | Requirement |
| `V2-BR-nnn` | Business rule — defined in `Business_Rules_V2.md` |
| `V2-ENT-nnn` | Data entity — defined in `Data_Model_and_Workflow_Entities_V2.md` |
| `F-V2-nnn` | Function (functional specification) — defined in `Functional_Specification_V2.md` |

### 1.2 Requirement wording policy

> **"The system shall…" is used only where the requirement is directly supported by
> observed or otherwise established system behaviour.** Inferred behaviour is written
> as a stated behaviour with an explicit confidence marker and is never phrased as an
> obligation.

### 1.3 Behaviour classification

**CONFIRMED FUNCTIONAL** · **TECHNICALLY SUPPORTED** · **INFERRED** · **UNKNOWN** · **CONFLICTING**

### 1.4 Structure

- **§2** — master inventory (one row per function).
- **§3** — requirement cards with the complete field set.
- **§4** — behaviour classification summary and the requirements explicitly **not** asserted.
- **§5** — version applicability and the V1/V2 delta.
- **§6** — traceability index.

---

## 2. Master function inventory — V-CD-2019

| Function ID | Req IDs | Module | Function name | Arabic label | Actor | Trigger | Preconditions | Behaviour class | Confidence |
|---|---|---|---|---|---|---|---|---|---|
| **V2-FUNC-001** | V2-REQ-001, 002, 003 | General | Start, connect and select the database engine | «كيف يتم تشغيل نظام ريال لاب سيستم ؟» | System administrator | Application launch | Server running; English date locale | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-002** | V2-REQ-004, 005, 006 | System | Sign in / sign out with permission bits and attendance | «كيف يتم تسجيل الدخول لنظام ريال لاب سيستم ؟» | Any operator | Credential entry | Operator record exists | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-003** | V2-REQ-007, 008 | Patient | Register a new patient, including branch | «كيف يتم اضافة بيانات مريض جديد ؟» | Lab operator | Add-patient action | Signed in | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-004** | V2-REQ-009 | Patient | Generate and print the patient barcode | «كيف يتم طباعة باركود للمريض؟» | Lab operator | Barcode request | Patient record exists | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-005** | V2-REQ-010 | Patient | Search for a patient | «كيف تتم عملية البحث عن مريض فى نظام ريال لاب سيستم ؟» | Any operator | Search term entry | Signed in | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-006** | V2-REQ-011 | Patient | Review all visits with dates and tests | «البحث عن مريض معنى وإظهار جميع زياراته للمعمل بتواريخها والتحاليل» | Lab operator | History view opened | Patient selected | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-007** | V2-REQ-012 | Patient | Open the profile result-entry window | «كيف يتم الوصول لنوافذ ادخال نتائج التحاليل ؟» | Lab operator | Test selected twice | Test requested | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-008** | V2-REQ-013, 014, 015, 016 | Patient | Enter a result with automatic classification | «كيف يتم ادخال نتائج التحاليل (تحاليل الكبد وما شابه مثلاً) ؟» | Lab operator | Value entry | Profile window open | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-009** | V2-REQ-017 | Patient | Confirm a result as reviewed | — (F8 = «اعتماد مُراجَع») | Lab operator | F8 | Result saved | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-010** | V2-REQ-018, 019 | Patient | Hand over results and settle the account | «كيف يتم تسليم نتائج التحاليل للمريض ؟» | Reception clerk | Account window opened | Reviewed results outstanding | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-011** | V2-REQ-020 | System | Add or edit a test definition | «كيف يتم اضافة أو تعديل بيانات تحليل ؟» | Laboratory administrator | Test editor opened | Administrator rights | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-012** | V2-REQ-021, 022 | System | Administer normal reference ranges | «كيف يتم اضافة أو modifier المعدلات الطبيعية لتحليل معين ؟» | Laboratory administrator | Range editor opened | Test exists | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-013** | V2-REQ-023 | General | Customise report text, dimensions and colours | «كيف يتم التعديل فى نصوص وأبعاد وألوان التقرير ؟» | Laboratory administrator | Report setup opened | Administrator rights | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-014** | V2-REQ-024 | Accounting | Manage send-out samples | «كيف يتم ادارة العينات المرسلة للخارج من داخل النظام ؟» | Accounting user | F7 / accounting menu | Test flagged `inotherlab` | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-015** | V2-REQ-025, 026, 027 | Accounting | Run accounting and stock-take, single or multi-branch | «كيف يتم معرفة حساب الدرج وعمل جرد لفترة محددة ؟» · «عمل جرد على فرع ضمن عدة فروع مرتبطة معا» | Accounting user | Period selected | Signed in; branches registered | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-016** | V2-REQ-028 | General | Back up the database | «كيف يتم عمل نسخ احتياطي لقاعدة بيانات النظام ؟» | Administrator | Backup invoked | No exclusive lock | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-017** | V2-REQ-029 | System | Manage units and unit conversions | — | Laboratory administrator | Unit editor opened | Administrator rights | TECHNICALLY SUPPORTED | STRONGLY SUPPORTED |
| **V2-FUNC-018** | V2-REQ-030 | System | Manage abbreviations | — | Laboratory administrator | Dictionary opened | Administrator rights | TECHNICALLY SUPPORTED | STRONGLY SUPPORTED |
| **V2-FUNC-019** | V2-REQ-031 | System | Manage clinical sub-groups | — | Laboratory administrator | Group editor opened | Administrator rights | TECHNICALLY SUPPORTED | STRONGLY SUPPORTED |
| **V2-FUNC-020** | V2-REQ-032, 033 | System | Manage operators, permission bits, roles and login groups | «اضافة مستخدمين ومديرين للنظام» · «تعديل بيانات وصلاحيات المستخدمين والمديرين» | Laboratory administrator | User administration opened | Administrator rights | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-021** | V2-REQ-034, 035, 036 | General | Configure printers, paper, margins, headers, colours, background | «ضبط الطابعات المخصصة للطباعة» · «حجم الورق المستخدم في الطباعة» · «إعدادات هوامش التقرير» · «إعدادات رأس وذيل التقرير» · «تعديل ألوان رأس وذيل التقرير» · «إعدادات الخلفية» | Laboratory administrator | Configuration change | Administrator rights | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-022** | V2-REQ-037 | Accounting | Manage referring parties and treating physicians | «إضافة جهة إحالة» · «إضافة طبيب معالج» | Laboratory administrator | Party editor opened | Administrator rights | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-023** | V2-REQ-038 | Patient | Attach and view patient pictures | — | Lab operator | Picture frame opened | Patient selected | INFERRED | INFERRED |
| **V2-FUNC-024** | V2-REQ-039 | Patient | Consult method sheets and critical values | — | Lab operator | Sheet consulted | Signed in | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-025** | V2-REQ-040 | Training | Use the interactive training simulators | — | Trainee | Simulator launched | Flash Player 8 available | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-026** | V2-REQ-041 | General | Print a report to a bound printer | — | Any operator | F11 / F12 | Result reviewed; printer bound | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-027** | V2-REQ-042 | General | Print the patient invoice and ID card | — | Reception clerk | Invoice or card printed | Patient record exists | TECHNICALLY SUPPORTED | STRONGLY SUPPORTED |
| **V2-FUNC-028** | V2-REQ-043, 044, 045, 046 | General | Produce a PDF report | — | Lab operator | Print with the PDF printer selected | Bullzip installed; result reviewed | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-029** | V2-REQ-047, 048 | Patient | Deliver a result through NATIGH.COM | «تسليم نتائج المرضى عن طريق موقع نتتيجة (NATIGH.COM)» | Lab operator | Publication action | NATIGH account exists; result reviewed | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-030** | V2-REQ-049, 050 | Patient | Send an SMS result notification | — | Lab operator | Send action | `smslist` / `smsgroups` populated | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-031** | V2-REQ-051, 052, 053 | Accounting | Manage branches and create a branch | «نظام الفروع للمعامل» | Master-laboratory manager | Branch creation | Administrator rights | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-032** | V2-REQ-054, 055 | Server | Run the statistics and performance module | «الإحصائيات ومتابعة أداء المعمل» | Master-laboratory manager | Statistic selected | Signed in | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-033** | V2-REQ-056 | System | Register attendance and departure | «تسجيل الحضور والانصراف لمعرفة حركة المستخدمين في الدخول والخروج» | Any operator | Sign-in / sign-out | Signed in | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-034** | V2-REQ-057 | Work Sheet | Produce a work sheet over a patient group | «عمل ورقة عمل لمجموعة من المرضى في فترة زمنية محددة» | Lab operator | Work sheet requested | Signed in | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-035** | V2-REQ-058 | Work Sheet | Produce a work sheet over a test group | «عمل ورقة عمل لتحليل معين أو مجموعة تحاليل في فترة زمنية محددة» | Lab operator | Work sheet requested | Signed in | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-036** | V2-REQ-059 | Patient | Report farm results | «إدخال نتائج تقارير المزارع» | Lab operator | Farm result entry | Farm registered | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-037** | V2-REQ-060 | System | Maintain the antibiotic catalogue | «إضافة مضاد حي للمزرعة» | Laboratory administrator | Catalogue opened | Administrator rights | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-038** | V2-REQ-061 | General | Split and withdraw samples | «فصل وسحب العينات» | Lab operator | Laboratory icon | Signed in | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-039** | V2-REQ-062 | Patient | Produce a consolidated report | «عمل تقرير مجمع» | Lab operator | Report request | Results selected | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-040** | V2-REQ-063 | Patient | Produce an empty report | «عمل تقرير فارغ» | Lab operator | Report request | Signed in | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-041** | V2-REQ-064, 065 | System | Maintain and print price lists | «إضافة قائمة أسعار جديدة» · «طباعة قائمة أسعار التحاليل» | Laboratory administrator | Price-list administration | Administrator rights | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-042** | V2-REQ-066 | General | Set the default account type | «ضبط نوع الحساب الافتراضي للنظام» | Laboratory administrator | Setting changed | Administrator rights | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-043** | V2-REQ-067 | General | Configure the connection receipt | «إعدادات الاتصال (رأس وذيل الاتصال - هوامش الاتصال)» | Laboratory administrator | Configuration change | Administrator rights | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-044** | V2-REQ-068 | General | Configure the background | «إعدادات الخلفية» | Laboratory administrator | Setting changed | Administrator rights | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-045** | V2-REQ-069 | General | Configure the report margins | «إعدادات هوامش التقرير» | Laboratory administrator | Setting changed | Administrator rights | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-046** | V2-REQ-070 | General | Configure the report header and footer | «إعدادات رأس وذيل التقرير» | Laboratory administrator | Setting changed | Administrator rights | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V2-FUNC-047** | V2-REQ-071 | Patient | Follow up case status | «متابعة تسجيل الحالات والتعديل عليها وإدخال النتائج وطباعة التقارير» | Lab operator | Case opened | Patient selected | STRONGLY SUPPORTED | STRONGLY SUPPORTED |
| **V2-FUNC-048** | V2-REQ-072 | Patient | Create a NATIGH account for a physician or laboratory | «إنشاء حساب للطبيب المعالج لاستخدام خدمة نتيجة» | Laboratory administrator | Account creation | Administrator rights | CONFIRMED FUNCTIONAL | CONFIRMED |

---

## 3. Requirement cards

### V2-REQ-001 — Application start-up

> **The system shall** display a start-up window and then a login screen on launch.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-001 |
| **Function ID** | V2-FUNC-001 |
| **Domain** | Startup |
| **Description** | Start-up sequence. |
| **Arabic label** | «الصفحة الرئيسية» then «شاشة التسجيل» |
| **Actor** | System administrator |
| **Trigger** | Application launch. |
| **Preconditions** | Server running; English date locale. |
| **Workflow** | Icon → «الصفحة الرئيسية» while loading → «شاشة التسجيل». |
| **Inputs / Outputs** | None / an application awaiting credentials. |
| **Resulting state** | Loaded, no session. |
| **Related business rules** | V2-BR-001…004 (deployment chain) |
| **Related entities** | — |
| **Relevant fields** | — |
| **Permissions** | None to reach the login screen. |
| **Exceptions** | «خادم قواعد البيانات غير معرف»; «رجاء تحويل التاريخ الى اللغة الانجليزية» — both terminate the application on **موافق**. |
| **Dependencies** | `BASE.ini`; the database host. |
| **Related screens** | `start` (14 controls), `login` |
| **Related modules** | — |
| **Technical mechanisms** | OLE DB connection at start-up. |
| **Evidence** | CHM `001.htm` — byte-identical to V1 |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The connection sequence and timeout are not observable statically. |

### V2-REQ-002 — Database engine selection

> **The system shall** connect to the database engine selected at installation, and the selection shall be recorded in the configuration as a toggle-encoded engine name.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-002 |
| **Function ID** | V2-FUNC-001 |
| **Domain** | Deployment |
| **Description** | Access, SQL Server Express 2005 or MySQL. |
| **Arabic label** | — |
| **Actor** | System administrator (at install time) |
| **Trigger** | Installation |
| **Preconditions** | The chosen engine's prerequisites installed. |
| **Workflow** | Installer presents the choice → writes `Databasetype=XMSACCESSX`-style value → the application connects to it. |
| **Inputs** | Engine choice. |
| **Outputs** | A configured connection. |
| **Resulting state** | The application talks to one engine. |
| **Related business rules** | V2-BR-027, V2-BR-024 |
| **Related entities** | `BASE.ini` |
| **Relevant fields** | `Databasetype`, `InitialCatalog`, `DataSource`, `loca` |
| **Permissions** | — |
| **Exceptions** | «خادم قواعد البيانات غير معرف» |
| **Dependencies** | `Setup*.bat` for the SQL Server and MySQL prerequisites. |
| **Related screens** | The installer |
| **Related modules** | — |
| **Technical mechanisms** | Jet OLE DB · `SQLOLEDB.1` · `MSDataShape.1` · MySQL ODBC 3.51 |
| **Evidence** | `RL_SETUP.exe` engine-selection strings; `Setup*.bat`; `BASE.ini` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The toggle encoding for the non-Access values is NOT ESTABLISHED. |

### V2-REQ-003 — Runtime schema migration

> **The system shall** apply an embedded set of schema-migration statements to the live database at start-up.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-003 |
| **Function ID** | V2-FUNC-001 |
| **Domain** | Data / Deployment |
| **Description** | 268 embedded DDL statements. |
| **Arabic label** | — |
| **Actor** | The application |
| **Trigger** | Application start-up |
| **Preconditions** | Write access to the database |
| **Workflow** | Execute 27 `CREATE TABLE`, 228 `ALTER TABLE ADD` and 13 `ALTER COLUMN` statements, in SQL Server and Access variants. |
| **Inputs** | The embedded DDL. |
| **Outputs** | A partially migrated schema. |
| **Resulting state** | 10 of 12 created tables and 109 of 132 added columns present. |
| **Related business rules** | **V2-BR-023, V2-BR-024** |
| **Related entities** | 29 affected tables |
| **Relevant fields** | See `Data_Model_and_Workflow_Entities_V2.md` §6.9 |
| **Permissions** | Database write rights. |
| **Exceptions** | SQL-Server-only statements fail against Jet 4 — silently, as far as the evidence shows. |
| **Dependencies** | The DDL must be re-appliable without error on an already-migrated database. |
| **Related screens** | — |
| **Related modules** | — |
| **Technical mechanisms** | `CREATE TABLE`, `ALTER TABLE ADD`, `ALTER COLUMN`, `sp_rename` |
| **Evidence** | `analysis/BATCH-04/schema/ddl-all.txt`; reconciliation against the shipped database |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **★ The migration is incomplete and its failure handling is not evidenced.** 21 objects were never applied. |

### V2-REQ-004 — Sign-in with a permission model

> **The system shall** require a user name and password at the login screen before presenting the operational interface, and **shall** evaluate the operator's permission bits against the requested action.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-004 |
| **Function ID** | V2-FUNC-002 |
| **Domain** | Security |
| **Description** | The credential gate plus the permission-bit check. |
| **Arabic label** | «شاشة التسجيل» |
| **Actor** | Any operator |
| **Trigger** | Credential entry |
| **Preconditions** | An operator record exists in `patients`. |
| **Workflow** | Enter user name → password → confirm → the session's permission bits are loaded → each guarded action is checked. |
| **Inputs** | User name; password. |
| **Outputs** | An authenticated session with a permission set. |
| **Resulting state** | Session bound to a login group, a branch and a permission set. |
| **Related business rules** | V2-BR-039, V2-BR-041 |
| **Related entities** | `patients` (the operator table — **not** `patienttest`), `loggroup` |
| **Relevant fields** | `patientusname`, `patientword`, `patientad`, `patientus`, `grant1`…`grant12`, `grant13`…`grant17` |
| **Permissions** | This requirement *is* the access-control gate. |
| **Exceptions** | Failure behaviour NOT ESTABLISHED. |
| **Dependencies** | `patients` (Access mode) or `useraccounts` (SQL mode). |
| **Related screens** | `login`, `users`, `userspw`, `logindete` |
| **Related modules** | — |
| **Technical mechanisms** | Credential comparison; bit check. |
| **Evidence** | `patients` schema (26 columns, including `grant1`…`grant12`); DDL adding `grant13`…`grant17`; CHM `002.htm` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The mapping of each bit to a named capability is UNKNOWN.** |

### V2-REQ-005 — Attendance recording

> **The system shall** record operator attendance — work start, work end and break state — against the operator record.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-005 |
| **Function ID** | V2-FUNC-002, V2-FUNC-033 |
| **Domain** | Security / Workforce |
| **Description** | Two attendance mechanisms: fields on the operator record, and the separate `loger` event table. |
| **Arabic label** | «تسجيل الحضور والانصراف لمعرفة حركة المستخدمين في الدخول والخروج» |
| **Actor** | Any operator (automatic); the manager reads it. |
| **Trigger** | Sign-in / sign-out / break |
| **Preconditions** | Signed in |
| **Workflow** | Sign in → attendance recorded → sign out → departure recorded → `loger` event written. |
| **Inputs** | Operator; timestamp. |
| **Outputs** | `patients.startwork` / `endwork` / `breakyorn` / `breaktime`; `loger` rows. |
| **Resulting state** | Presence recorded. |
| **Related business rules** | V2-BR-012, V2-BR-041 |
| **Related entities** | `patients`, `loger`, `loggroup` |
| **Relevant fields** | `startwork`, `endwork`, `breakyorn`, `breaktime` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | `SETTING.ini [setup] labbranchx` |
| **Related screens** | — |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | `patients` schema; `loger` table; vendor guide 3-11 |
| **Confidence** | CONFIRMED for the fields; INFERRED for the write behaviour |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The `loger` column structure was not extracted.** The attendance timestamps are `Text (15)`, not `DateTime`. |

### V2-REQ-006 — Shared static database credential

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-006 |
| **Function ID** | V2-FUNC-001, V2-FUNC-002 |
| **Domain** | Security / Deployment |
| **Description** | Every workstation authenticates to the database with the same credential pair, read from a plain-text configuration file. |
| **Arabic label** | «اسم وكلمة مرور المستخدم ويمنع تغيرها» |
| **Actor** | The application |
| **Trigger** | Every database connection. |
| **Preconditions** | — |
| **Workflow** | Read `UserID` / `Password` from the configuration → connect with administrative rights. |
| **Inputs** | Configuration file content. |
| **Outputs** | A connection with full rights. |
| **Resulting state** | Connected. |
| **Related business rules** | **V2-BR-032** |
| **Related entities** | `BASE.ini`, `data_connection*.udl` |
| **Relevant fields** | `UserID`, `Password`, `PersistSecurityInfo` |
| **Permissions** | The application holds full database rights. |
| **Exceptions** | — |
| **Dependencies** | The configuration file must be readable. |
| **Related screens** | `server` |
| **Related modules** | — |
| **Technical mechanisms** | OLE DB connection string. |
| **Evidence** | `BASE.ini`; `data_connection*.udl`; CHM `001.htm` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The Access/Jet path does not use this identity. |

### V2-REQ-007 — Patient registration with branch

> **The system shall** create a patient master record capturing basic identifying data, age, sex, referring party, barcode, request date and branch.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-007 |
| **Function ID** | V2-FUNC-003 |
| **Domain** | Patient / Branches |
| **Description** | Registration, extended with branch attribution. |
| **Arabic label** | «كيف يتم اضافة بيانات مريض جديد ؟» |
| **Actor** | Lab operator / reception clerk |
| **Trigger** | Add-patient action. |
| **Preconditions** | Signed in; a branch context selected. |
| **Workflow** | Open `addpatient` → enter data → age and sex → referring party → barcode → request date → save. |
| **Inputs** | Demographics; age; sex; referral; barcode; date; branch. |
| **Outputs** | A `patientinfo` row; a barcode allocation. |
| **Resulting state** | The patient is selectable for ordering. |
| **Related business rules** | **V2-BR-008**, V2-BR-040 |
| **Related entities** | `patientinfo`, `barcodes`, `Gehats`, `Labbranch` |
| **Relevant fields** | `patientid` (position 9 = branch), **`patients.branche`** (the operator's branch), `patientnamear`, `patientsex`, `patientage`, `referredbyname`, `mowzaf`, `barcode`, `requestdate` |
| **Permissions** | Signed-in session. |
| **Exceptions** | NOT ESTABLISHED. |
| **Dependencies** | `IDCARD.INI`; `SETTING.ini [setup] barcodeauto`, `labbranchx` |
| **Related screens** | `addpatient`, `patient`, `patientmain` |
| **Related modules** | — |
| **Technical mechanisms** | INSERT into `patientinfo`. |
| **Evidence** | CHM `003.htm`; `patientinfo` schema; `Labbranch`; `MID(patientid,9,1)` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **Validation rules, mandatory fields and duplicate detection are NOT ESTABLISHED.** |

### V2-REQ-008 — Identifier segment allocation

> **The system shall** allocate the composite order identifier so that the person key occupies positions 3–4 and the branch code occupies position 9.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-008 |
| **Function ID** | V2-FUNC-003 |
| **Domain** | Patient / Data |
| **Description** | The positional identifier contract. |
| **Arabic label** | — |
| **Actor** | The application |
| **Trigger** | Patient registration |
| **Preconditions** | — |
| **Workflow** | Allocate the identifier → write the person sequence at 3–4 → write the branch digit at 9 → write the display code at 10–12. |
| **Inputs** | Person sequence; branch code. |
| **Outputs** | A 13-character `patientid`. |
| **Resulting state** | The record is addressable by all three derivations. |
| **Related business rules** | **V2-BR-008, V2-BR-040** |
| **Related entities** | `patientinfo`, `patienttest` |
| **Relevant fields** | `patientid` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `addpatient` |
| **Related modules** | — |
| **Technical mechanisms** | String composition. |
| **Evidence** | `MID(patientid,3,2)`, `MID(patientid,9,1) as bran`, `mid(patientid,10,3) as patid` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The allocation algorithm for the person sequence is NOT ESTABLISHED**, as are the segments at positions 1–2 and 5–8. |

### V2-REQ-009 — Barcode as the physical patient identifier

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-009 |
| **Function ID** | V2-FUNC-004 |
| **Domain** | Patient / Security |
| **Description** | A barcode label identifying the patient, printed on the configured barcode printer and accompanying the specimen. |
| **Arabic label** | «كيف يتم طباعة باركود للمريض؟» |
| **Actor** | Lab operator |
| **Trigger** | Barcode request |
| **Preconditions** | A patient record exists. |
| **Workflow** | Allocate from `barcodes` → print via the barcode printer → link to the patient. |
| **Inputs** | Patient record. |
| **Outputs** | `barcodes` row; printed label. |
| **Resulting state** | The specimen is matched to the record. |
| **Related business rules** | — |
| **Related entities** | `barcodes`, `patientinfo` |
| **Relevant fields** | barcode field; `testtable.barcodesymb`, `barcode1`–`barcode3` |
| **Permissions** | Signed-in session. |
| **Exceptions** | «Can't find any printer.» |
| **Dependencies** | `SETTING.ini [selectprinter] barcodeprinter`, `barcodeport` |
| **Related screens** | `barcode`, `barcodeform`, `z_barcodes` |
| **Related modules** | `printerselect` |
| **Technical mechanisms** | Spooler dispatch. |
| **Evidence** | CHM `003A.htm`; `barcodes`; `testtable.barcodesymb` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The symbology is NOT ESTABLISHED. **The barcode is a direct identifier on an uncontrolled physical artefact.** |

### V2-REQ-010 — Patient search

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-010 |
| **Function ID** | V2-FUNC-005 |
| **Domain** | Patient |
| **Description** | Search by name, number or date, using the positional person key. |
| **Arabic label** | «كيف تتم عملية البحث عن مريض فى نظام ريال لاب سيستم ؟» |
| **Actor** | Any operator |
| **Trigger** | Search term entry |
| **Preconditions** | Signed in. |
| **Workflow** | Search → `MID(patientid,3,2)` filter → matching record. |
| **Inputs** | Name / number / date fragment. |
| **Outputs** | A selected patient. |
| **Resulting state** | Current-patient context. |
| **Related business rules** | V2-BR-040 |
| **Related entities** | `patientinfo` |
| **Relevant fields** | `patientid` |
| **Permissions** | Signed-in session. |
| **Exceptions** | NOT ESTABLISHED. |
| **Dependencies** | — |
| **Related screens** | `patientmain`, `patientview` |
| **Related modules** | — |
| **Technical mechanisms** | `MID()` substring extraction in SQL. |
| **Evidence** | CHM `006.htm`; the SQL corpus |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The match semantics (exact / partial / prefix) are NOT ESTABLISHED. |

### V2-REQ-011 — All-visits history

> **The system shall** display all of a patient's laboratory visits with their dates and tests.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-011 |
| **Function ID** | V2-FUNC-006 |
| **Domain** | Patient |
| **Description** | The longitudinal visit view. |
| **Arabic label** | «البحث عن مريض معنى وإظهار جميع زياراته للمعمل بتواريخها والتحاليل» |
| **Actor** | Lab operator |
| **Trigger** | History view opened |
| **Preconditions** | Patient selected. |
| **Workflow** | Open `patienthistory` → read the visit/test rows → display with dates. |
| **Inputs** | Patient identifier. |
| **Outputs** | Visit list with dates and tests. |
| **Resulting state** | Read-only. |
| **Related business rules** | — |
| **Related entities** | `patienttest`, `patientinfo`, `history` |
| **Relevant fields** | `requestdate`, `testcode`, `testname` |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | `SETTING.ini [setup] historytype` |
| **Related screens** | `patienthistory`, `patienthistory2` |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide 1-10; `patienthistory` / `patienthistory2` forms |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The `history` column semantics remain UNKNOWN. |

### V2-REQ-012 — Profile-scoped entry window

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-012 |
| **Function ID** | V2-FUNC-007 |
| **Domain** | Results |
| **Description** | Entry opens a discipline-specific window carrying that profile's analyte set. |
| **Arabic label** | «كيف يتم الوصول لنوافذ ادخال نتائج التحاليل ؟» |
| **Actor** | Lab operator |
| **Trigger** | The test is selected a second time. |
| **Preconditions** | Test requested. |
| **Workflow** | Select patient → select test twice → profile window opens with analyte rows. |
| **Inputs** | Patient; test. |
| **Outputs** | An active entry session. |
| **Resulting state** | Operator positioned on the first analyte. |
| **Related business rules** | — |
| **Related entities** | `patienttest`, profile tables, `subgrouptable` |
| **Relevant fields** | `testcode`, `subgroupcode` |
| **Permissions** | Signed-in session. |
| **Exceptions** | NOT ESTABLISHED. |
| **Dependencies** | `Data\*.ini` for the profile. |
| **Related screens** | `result*` (30+ forms in V2) |
| **Related modules** | — |
| **Technical mechanisms** | Profile-table read by sub-group. |
| **Evidence** | CHM `004.htm`; `result*` object family |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The form-to-profile mapping is not established for every profile. |

### V2-REQ-013 — Automatic abnormal classification

> **The system shall** classify an entered result by comparing it against the low and high limits held for the analyte, and mark it abnormal when it falls outside them.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-013 |
| **Function ID** | V2-FUNC-008 |
| **Domain** | Results / Clinical safety |
| **Description** | Range-driven classification. |
| **Arabic label** | «يميز النتائج الغير طبيعية» |
| **Actor** | Lab operator |
| **Trigger** | Value entry |
| **Preconditions** | A matching range row, and the frozen limits on the test row. |
| **Workflow** | Enter value → compare against `lowlimit` / `highlimit` → highlight as abnormal. |
| **Inputs** | Measured value. |
| **Outputs** | Abnormal flag; exception text. |
| **Resulting state** | The result carries a flag. |
| **Related business rules** | **V2-BR-034, V2-BR-036** |
| **Related entities** | `normalrange`, `patienttest` |
| **Relevant fields** | `lowlimit`, `highlimit`, `nolowexe`, `nohighexe` |
| **Permissions** | Signed-in session. |
| **Exceptions** | No matching row ⇒ behaviour NOT ESTABLISHED. |
| **Dependencies** | `normalrange` completeness. |
| **Related screens** | `result*` |
| **Related modules** | — |
| **Technical mechanisms** | Numeric comparison against the frozen limits. |
| **Evidence** | CHM `004A.htm`; `normalrange` and `patienttest` schemas |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The band-selection algorithm is NOT ESTABLISHED. |

### V2-REQ-014 — Critical classification

> **The system shall** distinguish a critical result from an abnormal result and present the critical state in preference to the abnormal state.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-014 |
| **Function ID** | V2-FUNC-008 |
| **Domain** | Results / Clinical safety |
| **Description** | Three-valued classification. |
| **Arabic label** | «عالٍ أو منخفضة أو حرجة» |
| **Actor** | Lab operator |
| **Trigger** | Value entry beyond the critical limit |
| **Preconditions** | Critical limit populated. |
| **Workflow** | Value breaches the critical limit → critical flag set. |
| **Inputs** | Measured value. |
| **Outputs** | Critical flag and comment. |
| **Resulting state** | The result is critical. |
| **Related business rules** | **V2-BR-035** |
| **Related entities** | `normalrange`, `patienttest` |
| **Relevant fields** | Critical range, Critical comment, Critical flag |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `result*` |
| **Related modules** | — |
| **Technical mechanisms** | Comparison against the critical limit. |
| **Evidence** | CHM `004A.htm`; `normalrange` critical columns |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **No escalation workflow is evidenced.** |

### V2-REQ-015 — Clinical derivation from stored constants

> **The system shall** derive haematocrit, haemoglobin percentage and the ISI from the stored constants, and hold those constants as configurable data.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-015 |
| **Function ID** | V2-FUNC-008 |
| **Domain** | Clinical |
| **Description** | Derivation from `constanttable`. |
| **Arabic label** | «Hct = Hgb × 3.3» and the accompanying factors |
| **Actor** | Lab operator |
| **Trigger** | Result derivation |
| **Preconditions** | Source result exists. |
| **Workflow** | Read the constant → select the factor by age and sex → multiply. |
| **Inputs** | `Hgb`; age; sex; control time. |
| **Outputs** | Derived `Hct`, `Hgb%`, ISI. |
| **Resulting state** | Derived values available for the report. |
| **Related business rules** | **V2-BR-037** |
| **Related entities** | `constanttable` (`CBCCONS`, `PTCONS`), `control` |
| **Relevant fields** | `constant1`…`constant5` |
| **Permissions** | — |
| **Exceptions** | The age-band boundary at 12 years is ambiguous in the source wording. |
| **Dependencies** | `constanttable` correctness. |
| **Related screens** | `resultcbc` |
| **Related modules** | — |
| **Technical mechanisms** | Stored factors; derived value |
| **Evidence** | CHM `nots.htm`; `constanttable` — **exact numeric concordance** |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The ISI formula and the QC evaluation algorithm are NOT ESTABLISHED. |

### V2-REQ-016 — Comment sourcing

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-016 |
| **Function ID** | V2-FUNC-008 |
| **Domain** | Results |
| **Description** | A comment may be typed, selected from the stored comment list, or taken from the reference-range list. |
| **Arabic label** | «زر تعليق (البرتقالي)» |
| **Actor** | Lab operator |
| **Trigger** | Comment entry |
| **Preconditions** | — |
| **Workflow** | Type · pick from `comments` · pick from the range list. |
| **Inputs** | Comment text. |
| **Outputs** | Stored comment. |
| **Resulting state** | The result carries a comment. |
| **Related business rules** | — |
| **Related entities** | `comments`, `normalrange`, `patienttest` |
| **Relevant fields** | Low/High comment, Critical comment, `commentx` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `result*`, `normal_test` |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | CHM `004A.htm`; `comments` (12 rows) |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | — |

### V2-REQ-017 — Review gate

> **The system shall** record an explicit review state on a saved result, and only reviewed results shall be eligible for release or printing.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-017 |
| **Function ID** | V2-FUNC-009, V2-FUNC-010, V2-FUNC-028 |
| **Domain** | Results / Quality |
| **Description** | The release gate. |
| **Arabic label** | «F8=اعتماد مُراجَع» · «Not found any Verified selected tests or result to print it» |
| **Actor** | Lab operator; reception clerk |
| **Trigger** | F8 |
| **Preconditions** | Result saved. |
| **Workflow** | F8 → `resultreview` set → the result becomes printable and hand-over eligible. |
| **Inputs** | A saved result. |
| **Outputs** | A reviewed result. |
| **Resulting state** | Released. |
| **Related business rules** | **V2-BR-038** |
| **Related entities** | `patienttest` |
| **Relevant fields** | `resultreview` |
| **Permissions** | Signed-in session. |
| **Exceptions** | The PDF path refuses with «Not found any Verified selected tests or result to print it». |
| **Dependencies** | — |
| **Related screens** | `result*` |
| **Related modules** | — |
| **Technical mechanisms** | Boolean filter. |
| **Evidence** | `patienttest.resultreview` (Boolean); the refusal literal |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | Whether a second signatory is required is NOT ESTABLISHED. |

### V2-REQ-018 — Hand-over and settlement

> **The system shall** present the patient's outstanding results, record the payment, print the report and mark the results as given to the patient.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-018 |
| **Function ID** | V2-FUNC-010 |
| **Domain** | Finance / Results |
| **Description** | The hand-over closes the episode. |
| **Arabic label** | «كيف يتم تسليم نتائج التحاليل للمريض ؟» |
| **Actor** | Reception clerk |
| **Trigger** | Account window opened |
| **Preconditions** | Signed in; reviewed results outstanding. |
| **Workflow** | Open `frm_money` → review outstanding → collect → print → mark given. |
| **Inputs** | Patient; amount. |
| **Outputs** | `payment` row; invoice; printed report; `give` flag. |
| **Resulting state** | Results released; account settled. |
| **Related business rules** | V2-BR-038, V2-BR-042 |
| **Related entities** | `patienttest`, `payment`, `patientinfo` |
| **Relevant fields** | `give`, `resultprint`, `totalprice`, `totalpricedisc`, `elmadfoo3` |
| **Permissions** | Signed-in session. |
| **Exceptions** | «Can't find any printer.» |
| **Dependencies** | `IDCARD.INI`; `Data\*.ini` |
| **Related screens** | `frm_money`, `moneypaid` |
| **Related modules** | `printerselect` |
| **Technical mechanisms** | — |
| **Evidence** | CHM `005.htm`; `payment` (9 columns); `patienttest.give` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The pricing and discount computation is NOT ESTABLISHED. |

### V2-REQ-019 — Commission computation

> **The system shall** compute the referring-party commission as the sum over patients of the party's commission rate multiplied by the patient-level commission base, divided by one hundred.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-019 |
| **Function ID** | V2-FUNC-015, V2-FUNC-022 |
| **Domain** | Finance |
| **Description** | The commission formula. |
| **Arabic label** | «عمولات جهات الإحالة» |
| **Actor** | Accounting user |
| **Trigger** | Stock-take |
| **Preconditions** | The patient has an `elmadfoo3` value and a referring party with a rate. |
| **Workflow** | Read `Gehats.omola` and `patientinfo.elmadfoo3` → compute `SUM(omola * elmadfoo3 / 100)` per party. |
| **Inputs** | `omola`, `elmadfoo3` |
| **Outputs** | Commission totals per referring party. |
| **Resulting state** | Read-only. |
| **Related business rules** | **V2-BR-042, V2-BR-026** |
| **Related entities** | `Gehats`, `patientinfo`, `payment` |
| **Relevant fields** | `omola`, `elmadfoo3`, `referredbyname`, `referredbysefa` |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | Accounting window |
| **Related modules** | `labfunction` |
| **Technical mechanisms** | Aggregation query. |
| **Evidence** | The SQL literal `SUM(Gehats.omola * patientinfo.elmadfoo3 / 100)` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The business meaning of `elmadfoo3` is NOT ESTABLISHED. |

### V2-REQ-020 — Test definition maintenance

> **The system shall** allow a laboratory administrator to add or modify a test definition, capturing its name, Arabic name, abbreviation, unit, clinical sub-group, prices, send-out flag, sample type and methodology text.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-020 |
| **Function ID** | V2-FUNC-011 |
| **Domain** | Catalogue |
| **Description** | Test-catalogue maintenance, extended with branch ownership and methodology fields. |
| **Arabic label** | «كيف يتم اضافة أو تعديل بيانات تحليل ؟» |
| **Actor** | Laboratory administrator |
| **Trigger** | Test editor opened |
| **Preconditions** | Administrator rights. |
| **Workflow** | Open `setup1`/`setup2`/`setup3` → set the attributes → save to `testtable`. |
| **Inputs** | Name, Arabic name, abbreviation, unit, sub-group, prices, send-out flag, sample type, description, effect, usage. |
| **Outputs** | `testtable` row. |
| **Resulting state** | The test is orderable and printable. |
| **Related business rules** | **V2-BR-043** |
| **Related entities** | `testtable` (37 columns, 609 rows), `customtesttable` |
| **Relevant fields** | `testcode`, `testname`, `testnamear`, `testabrev`, `testunit`, `subgroupcode`, `inotherlab`, `patientprice`, `labprice`, `otherlabprice`, `typeofsample`, `testdiscr`, `testeffect`, `testusage`, `branch`, `normaltype` |
| **Permissions** | Administrator rights (exact flag NOT ESTABLISHED). |
| **Exceptions** | The urine analysis (code 4) may not be renumbered or renamed. |
| **Dependencies** | `Data\*.ini` layout for the profile. |
| **Related screens** | `setup`, `setup1`, `setup2`, `setup3`, `tools`, `abbrevi`, `converter`, `instrument`, `commentsetup` |
| **Related modules** | — |
| **Technical mechanisms** | INSERT / UPDATE on `testtable`. |
| **Evidence** | CHM `007.htm`; `testtable` schema and 609 rows |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | Whether a new test also requires an explicit layout edit is INFERRED. |

### V2-REQ-021 — Reference-range maintenance

> **The system shall** allow an administrator to add, modify, delete and undo reference-range rows scoped by sex, age window and age unit.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-021 |
| **Function ID** | V2-FUNC-012 |
| **Domain** | Clinical knowledge |
| **Description** | Range administration. |
| **Arabic label** | «كيف يتم اضافة أو تعديل المعدلات الطبيعية لتحليل معين ؟» |
| **Actor** | Laboratory administrator |
| **Trigger** | Range editor opened |
| **Preconditions** | Test exists. |
| **Workflow** | Setup → Test data → [test] → «المعدل الطبيعي» → add / edit / save / undo / delete. |
| **Inputs** | The 14 range attributes. |
| **Outputs** | `normalrange` rows. |
| **Resulting state** | Subsequent classification for that analyte uses the new limits. |
| **Related business rules** | **V2-BR-036, V2-BR-043** |
| **Related entities** | `normalrange` (852 rows, 18 columns) |
| **Relevant fields** | `sex`, `firstold`, `secondold`, `ageunit`, `normalrang`, `lowlimit`, `highlimit`, `nolowexe`, `nohighexe`, `lowcomment`, `highcomment`, `pregsmok`, `REFRANGE` |
| **Permissions** | Administrator rights. |
| **Exceptions** | Overlap validation NOT ESTABLISHED. |
| **Dependencies** | `testunits` |
| **Related screens** | `testrange`, `outtest`, `normal_test` |
| **Related modules** | — |
| **Technical mechanisms** | INSERT / UPDATE / DELETE on `normalrange`. |
| **Evidence** | CHM `008.htm`; `normalrange` schema — the documented attributes map one-to-one onto the schema |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The `pregsmok` dimension's role is NOT traced. |

### V2-REQ-022 — Range-scoped classification

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-022 |
| **Function ID** | V2-FUNC-008, V2-FUNC-012 |
| **Domain** | Clinical knowledge |
| **Description** | The applicable range is selected by analyte, patient sex and age band. |
| **Actor** | — (system behaviour) |
| **Trigger** | Result entry / range maintenance |
| **Preconditions** | Range rows exist. |
| **Workflow** | Match analyte → match sex → match age window → freeze the limits onto the test row. |
| **Inputs** | Analyte; sex; age. |
| **Outputs** | The applicable range row, frozen onto `patienttest`. |
| **Resulting state** | — |
| **Related business rules** | **V2-BR-036** |
| **Related entities** | `normalrange`, `patienttest` |
| **Relevant fields** | `sex`, `firstold`, `secondold`, `ageunit`; `patienttest.lowlimit`, `highlimit`, `normalrang` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `result*` |
| **Related modules** | — |
| **Technical mechanisms** | Multi-criteria lookup; freeze-on-order copy. |
| **Evidence** | CHM `008.htm`; `normalrange` distributions; 112 of 222 `patienttest` rows carry a frozen `normalrang` |
| **Confidence** | CONFIRMED for the banding and the freeze; UNKNOWN for the selection algorithm |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | **The band-selection algorithm among overlapping ranges is not established.** |

### V2-REQ-023 — Report layout customisation

> **The system shall** allow an administrator to modify, per clinical profile, the report headings, the seven column positions and widths, the colour assignments, the per-position font, the margins, the header and footer texts, and the printer binding.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-023 |
| **Function ID** | V2-FUNC-013, F-V2-045, F-V2-046, F-V2-044 |
| **Domain** | Reporting |
| **Description** | The full per-profile layout surface. |
| **Arabic label** | «كيف يتم التعديل فى نصوص وأبعاد وألوان التقرير ؟» · «إعدادات هوامش التقرير» · «إعدادات رأس وذيل التقرير» · «إعدادات الخلفية» |
| **Actor** | Laboratory administrator |
| **Trigger** | Report setup opened |
| **Preconditions** | Administrator rights; profile `.ini` present. |
| **Workflow** | Open `reportsetup` → edit headings, positions, widths, colours, fonts, margins, header/footer, background → edit static blocks → save to `Data\*.ini`. |
| **Inputs** | Headings, coordinates, colours, fonts, margins, header/footer texts. |
| **Outputs** | Updated `Data\*.ini`. |
| **Resulting state** | The profile prints with the new layout. |
| **Related business rules** | **V2-BR-020, V2-BR-021** |
| **Related entities** | `Data\*.ini` (223 files / 114 profiles), `REPORT.dll` |
| **Relevant fields** | `[reportdata]` keys; `leftvalue`, `rightvalue`, `topvalue`, `bottomvalue`, `formsubtitle`, `reportmaintitle`, `colora`…`colort`, `backcolor`; `[setupx] reportprinterx` |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | `SETTING.ini [setup] papersize` |
| **Related screens** | `reportsetup` (108), `reportsetup2` (69), `static1`–`static5` |
| **Related modules** | — |
| **Technical mechanisms** | 66 `DataReport` design objects; VB6 `Printer` object. |
| **Evidence** | CHM `009.htm`; vendor guide 6-3, 6-5, 6-6, 9-7; `Data\*.ini` keys |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The mapping between the 66 DataReport designs and the 223 layouts is NOT ESTABLISHED. The precedence between the two colour sources is NOT ESTABLISHED. |

### V2-REQ-024 — Send-out management

> **The system shall** automatically include every test flagged as send-out in the send-out working list, group the items by destination laboratory, total them, and record dispatch and settlement as two independent states.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-024 |
| **Function ID** | F-V2-014 |
| **Domain** | Send-out |
| **Description** | Flag-driven enrolment with two settlement states. |
| **Arabic label** | «كيف يتم ادارة العينات المرسلة للخارج من داخل النظام ؟» · «خالص» · «دائماً إليه» |
| **Actor** | Accounting user |
| **Trigger** | Opening the send-out screen |
| **Preconditions** | The test is flagged `inotherlab`. |
| **Workflow** | Query the flagged tests → group by `otherlabname` → total → enter the settled amount → «خالص» → optionally «دائماً إليه». |
| **Inputs** | Destination; settled amount. |
| **Outputs** | Updated send-out states; totals. |
| **Resulting state** | Dispatch and settlement recorded independently. |
| **Related business rules** | **V2-BR-044** |
| **Related entities** | `testtable`, `patienttest`, `otherlabname` |
| **Relevant fields** | `inotherlab`, `sentout`, `send`, `otherlabname`, `testpriceout`, `testpriceoutmadfo` |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `sarfz`, `moenysharekat`, `moenymozafeen` |
| **Related modules** | `labfunction` |
| **Technical mechanisms** | Flag-driven query. |
| **Evidence** | CHM `010.htm`; `testtable` / `patienttest` schemas |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The standing-default flag's storage field is NOT ESTABLISHED. |

### V2-REQ-025 — Period stock-take

> **The system shall** present, for a selected period, patient accounts, send-out accounts, expenses and remittances, patients with residual balances, referring-party commissions, per-user workload and per-user collections, and shall ask the operator to decide whether the counted money is restricted to that period's patients.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-025 |
| **Function ID** | V2-FUNC-015 |
| **Domain** | Finance |
| **Description** | The six sub-reports of the stock-take. |
| **Arabic label** | «كيف يتم معرفة حساب الدرج وعمل جرد لفترة محددة ؟» · «هل الفلوس المحصلة من مرضى الفترة فقط؟» |
| **Actor** | Accounting user |
| **Trigger** | Period selected |
| **Preconditions** | Signed in. |
| **Workflow** | Choose the period → answer the scope question → review the six listings. |
| **Inputs** | Period; scope decision. |
| **Outputs** | Six accounting listings. |
| **Resulting state** | Read-only. |
| **Related business rules** | V2-BR-010, V2-BR-042 |
| **Related entities** | `payment`, `patientinfo`, `patienttest`, `Gehats`, `otherlabname` |
| **Relevant fields** | `money`, `paiddate`, `totalprice`, `totalpricedisc`, `omola`, `elmadfoo3`, `printuser`, `sentout`, `send` |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | `SETTING.ini [setup] labbranchx` |
| **Related screens** | Accounting and stock-take window; cash movement window |
| **Related modules** | `labfunction` (13 methods) |
| **Technical mechanisms** | Aggregation queries. |
| **Evidence** | CHM `011.htm`; `payment`, `Gehats` schemas |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The arithmetic of each listing is NOT ESTABLISHED. |

### V2-REQ-026 — Multi-branch stock-take

> **The system shall** permit the stock-take scope to cover a master laboratory together with its linked branches.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-026 |
| **Function ID** | V2-FUNC-015, F-V2-031 |
| **Domain** | Accounting / Branches |
| **Description** | The branch aggregation dimension. |
| **Arabic label** | «عمل جرد على فرع ضمن عدة فروع مرتبطة معا (نظام الفروع للمعامل)» |
| **Actor** | Master-laboratory manager |
| **Trigger** | Branch scope selected |
| **Preconditions** | Branches registered and connected. |
| **Workflow** | Select the branch scope → the listings aggregate across the selected branches. |
| **Inputs** | Branch selection. |
| **Outputs** | Group-level figures. |
| **Resulting state** | Read-only. |
| **Related business rules** | **V2-BR-010, V2-BR-008** |
| **Related entities** | `Labbranch`, `patientinfo`, `patienttest`, `payment` |
| **Relevant fields** | `patientid` position 9, **`patients.branche`** |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | Accounting window |
| **Related modules** | `labfunction` |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide 5-2; `Labbranch`; `MID(patientid,9,1)` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The branch-filtering mechanism is NOT ESTABLISHED. |

### V2-REQ-027 — Stock-take by treating physician or referring party

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-027 |
| **Function ID** | V2-FUNC-015, F-V2-022 |
| **Domain** | Finance |
| **Description** | Stock-take scoped to a physician or a referring party. |
| **Arabic label** | «عمل جرد لطبيب معالج معين أو جهة إحالة» |
| **Actor** | Accounting user |
| **Trigger** | Counterparty selected |
| **Preconditions** | Signed in. |
| **Workflow** | Select the physician or party → the period listing is scoped to that counterparty. |
| **Inputs** | Counterparty; period. |
| **Outputs** | A counterparty-scoped listing. |
| **Resulting state** | Read-only. |
| **Related business rules** | V2-BR-042 |
| **Related entities** | `patientinfo` (`mowzaf`, `referredbyname`), `payment` (`gehah`, `mowzaf`), `Gehats` |
| **Relevant fields** | `mowzaf`, `referredbyname`, `gehah`, `omola` |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | Accounting window |
| **Related modules** | `labfunction` |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide 5-3; `patientinfo.mowzaf`; `payment.gehah`, `payment.mowzaf` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The distinction between `mowzaf` and the referring party is a V2 addition. |

### V2-REQ-028 — Database backup

> **The system shall** provide an administrative procedure that produces a restorable copy of the laboratory database.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-028 |
| **Function ID** | V2-FUNC-016 |
| **Domain** | Administration |
| **Description** | Backup procedure. |
| **Arabic label** | «كيف يتم عمل نسخ احتياطي لقاعدة بيانات النظام ؟» |
| **Actor** | Administrator |
| **Trigger** | Backup invoked |
| **Preconditions** | No exclusive write lock. |
| **Workflow** | Invoke → the database is copied → a backup file results. |
| **Inputs** | The live database. |
| **Outputs** | A backup file. |
| **Resulting state** | A copy exists. |
| **Related business rules** | **V2-BR-026** |
| **Related entities** | `Patients.mdb`, `Patients_backup.mdb`, `data.mdb` |
| **Relevant fields** | — |
| **Permissions** | Administrator rights. |
| **Exceptions** | NOT ESTABLISHED. |
| **Dependencies** | Free space at the destination. |
| **Related screens** | Administrative backup function |
| **Related modules** | `labdb` (20), `labdb1` (19) |
| **Technical mechanisms** | File copy. |
| **Evidence** | CHM `backup.htm`; `labdb` / `labdb1`; the shipped `Patients_backup.mdb` |
| **Confidence** | CONFIRMED for the function; INFERRED for the mechanism |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The shipped backup is identical to the 2013 backup — 90 tables / 1,381 columns / 2,786 rows — and is stale against a 103-table live database.** |

### V2-REQ-029 — Unit and conversion dictionaries

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-029 |
| **Function ID** | V2-FUNC-017 |
| **Domain** | Clinical knowledge |
| **Description** | The unit dictionary and the analyte-specific conversion factors. |
| **Arabic label** | — |
| **Actor** | Laboratory administrator |
| **Trigger** | Unit / conversion editor opened |
| **Preconditions** | Administrator rights. |
| **Workflow** | Open `z_units` or `converter` → define → save. |
| **Inputs** | Unit; source unit; target unit; factor; fix decimals. |
| **Outputs** | `testunits` rows (69); `converts` rows (167). |
| **Resulting state** | The dictionaries are extended. |
| **Related business rules** | — |
| **Related entities** | `testunits`, `converts`, `testtable` |
| **Relevant fields** | `Funit`, `Sunit`, `Factors`, `Ffix`, `Sfix` |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `z_units` (21), `converter` (14) |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Form inventory; `testunits` 69 rows; `converts` 167 rows with `Ffix = Sfix = 2` on every row |
| **Confidence** | STRONGLY SUPPORTED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | **No manual topic documents this function** in either version. Whether conversion is applied on entry or on display is NOT ESTABLISHED. |

### V2-REQ-030 — Abbreviation dictionary

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-030 |
| **Function ID** | V2-FUNC-018 |
| **Domain** | Clinical knowledge |
| **Description** | The abbreviation dictionary, maintained independently of the test catalogue. |
| **Arabic label** | — |
| **Actor** | Laboratory administrator |
| **Trigger** | Dictionary opened |
| **Preconditions** | Administrator rights. |
| **Workflow** | Open `abbrevi` → add / edit → save. |
| **Inputs** | Abbreviation; expansion. |
| **Outputs** | `abbreviations` rows (650). |
| **Resulting state** | Abbreviation rendering changes. |
| **Related business rules** | — |
| **Related entities** | `abbreviations`, `testtable` |
| **Relevant fields** | `abbre`, `mean` |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `abbrevi` (41) |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Form inventory; `abbreviations` 650 rows |
| **Confidence** | STRONGLY SUPPORTED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | **The table contains no test-code column**, so the linkage to an analyte is NOT ESTABLISHED. |

### V2-REQ-031 — Clinical sub-group catalogue

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-031 |
| **Function ID** | V2-FUNC-019 |
| **Domain** | Catalogue |
| **Description** | Sub-groups are maintained and act as the routing key for entry windows and report layouts. |
| **Arabic label** | — |
| **Actor** | Laboratory administrator |
| **Trigger** | Group editor opened |
| **Preconditions** | Administrator rights. |
| **Workflow** | Open `z_group` → define → associate tests → save. |
| **Inputs** | Sub-group name; member tests. |
| **Outputs** | `subgrouptable` rows (46). |
| **Resulting state** | Test-to-profile routing changes. |
| **Related business rules** | — |
| **Related entities** | `subgrouptable`, `customgrouptable`, `testtable` |
| **Relevant fields** | `subgroupcode`, `subgroupname`, `arname`, `deleteprotect` |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | `Data\*.ini` naming convention |
| **Related screens** | `z_group` (21), `z_customgroup` (37) |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Form inventory; `subgrouptable` 46 rows |
| **Confidence** | STRONGLY SUPPORTED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | **The `arname` column is empty for all 46 shipped rows** — the Arabic interface uses the English sub-group names. The mapping onto 223 layouts is NOT ESTABLISHED. |

### V2-REQ-032 — Operator, permission and role management

> **The system shall** allow an administrator to create and modify operator records, assigning each a role caption, an administrator flag, an active flag, a permission-bit set and an attendance state.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-032 |
| **Function ID** | V2-FUNC-020 |
| **Domain** | Security administration |
| **Description** | Operator administration. |
| **Arabic label** | «اضافة مستخدمين ومديرين للنظام» · «تعديل بيانات وصلاحيات المستخدمين والمديرين» |
| **Actor** | Laboratory administrator |
| **Trigger** | User administration opened |
| **Preconditions** | Administrator rights. |
| **Workflow** | Open `users` → create / edit → assign the role, flags and permission bits → save; reset a password via `userspw`. |
| **Inputs** | Operator name; password; role caption; flags; permission bits. |
| **Outputs** | `patients` rows (Access mode) or `useraccounts` rows (SQL mode). |
| **Resulting state** | The operator can sign in with the assigned rights. |
| **Related business rules** | **V2-BR-039** |
| **Related entities** | `patients`, `useraccounts`, `loggroup` |
| **Relevant fields** | `patientusname`, `patientuscap`, `patientad`, `patientus`, `grant1`…`grant17`, `khasmlimit` |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | `SETTING.ini [setup] labbranchx` |
| **Related screens** | `users` (57 methods in V2), `userspw`, `z_group` |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide; `patients` schema; DDL for `useraccounts.gtype/hdd1/hdd2` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The mapping of each permission bit to a named capability is UNKNOWN.** `useraccounts` exists in no `.mdb` file. |

### V2-REQ-033 — Password reset

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-033 |
| **Function ID** | V2-FUNC-020 |
| **Domain** | Security administration |
| **Description** | Operator password reset. |
| **Arabic label** | — |
| **Actor** | Laboratory administrator |
| **Trigger** | Password form opened |
| **Preconditions** | Administrator rights. |
| **Workflow** | Open `userspw` → set the new password → save to the operator record. |
| **Inputs** | New password. |
| **Outputs** | The stored credential. |
| **Resulting state** | — |
| **Related business rules** | V2-BR-032 (the *database* credential is a different matter from the operator credential) |
| **Related entities** | `patients.patientword` |
| **Relevant fields** | `patientword` |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `userspw` (15 controls in V2) |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Form inventory; `patients.patientword` Text(40) |
| **Confidence** | STRONGLY SUPPORTED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | **The storage form of the operator password is NOT ESTABLISHED** — the field is `Text (40)`, and no hashing evidence exists. |

### V2-REQ-034 — Printer role binding

> **The system shall** bind printers to the report, barcode, ZARF, WASL and ID-card roles, and shall permit each report profile to bind its own printer.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-034 |
| **Function ID** | V2-FUNC-021 |
| **Domain** | Reporting / Deployment |
| **Description** | Five role bindings plus a per-profile binding. |
| **Arabic label** | «ضبط الطابعات المخصصة للطباعة» |
| **Actor** | Laboratory administrator |
| **Trigger** | Configuration change |
| **Preconditions** | The printer exists on the workstation. |
| **Workflow** | Bind `reportprinter`/`reportport`, `barcodeprinter`/`barcodeport`, `zarfprinter`, `waslprinter`, `IDprinter`; set `reportprinterx` per profile. |
| **Inputs** | Printer names; ports. |
| **Outputs** | Updated configuration. |
| **Resulting state** | Output routes to the intended devices. |
| **Related business rules** | — |
| **Related entities** | `SETTING.ini`, `Data\*.ini` |
| **Relevant fields** | `reportprinter`, `reportport`, `barcodeprinter`, `barcodeport`, `zarfprinter`, `waslprinter`, `IDprinter`, `reportprinterx` |
| **Permissions** | Administrator rights. |
| **Exceptions** | «Can't find any printer.» |
| **Dependencies** | Windows spooler. |
| **Related screens** | Printer selection (via `printerselect`) |
| **Related modules** | `printerselect` (16 methods) |
| **Technical mechanisms** | `EnumPrintersA`, `SetDefaultPrinterA`, `OpenPrinterA`/`SetPrinterA`/`GetPrinterA`/`ClosePrinter` |
| **Evidence** | `SETTING.ini`; `Data\*.ini`; the literal census |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The precedence between the role binding and the profile binding is NOT ESTABLISHED. |

### V2-REQ-035 — Global paper size and header configuration

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-035 |
| **Function ID** | V2-FUNC-021 |
| **Domain** | Reporting |
| **Description** | Paper size and the global header texts. |
| **Arabic label** | «حجم الورق المستخدم في الطباعة» · «المنظومة البرمجية» · «ريال لاب سيستم» |
| **Actor** | Laboratory administrator |
| **Trigger** | Configuration change |
| **Preconditions** | — |
| **Workflow** | Set `papersize` (A4/A5) and the `Textr*txt` / `Text*` keys. |
| **Inputs** | Paper size; header strings. |
| **Outputs** | Updated `SETTING.ini`. |
| **Resulting state** | Page geometry and global headers change. |
| **Related business rules** | **V2-BR-021** |
| **Related entities** | `SETTING.ini` |
| **Relevant fields** | `papersize`, `reportstart`, `Textr1txt`–`Textr4txt`, `Textl1`–`Textl4`, `Textc1`–`Textc4` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | — |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | `SETTING.ini`; vendor guide 6-4 |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The shipped global header text is the vendor's own product identity and prints on every report until changed.** |

### V2-REQ-036 — Report header/footer colour and background configuration

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-036 |
| **Function ID** | V2-FUNC-021, F-V2-044 |
| **Domain** | Reporting |
| **Description** | The 20 per-profile colour keys, the 19-entry global palette and the background colour. |
| **Arabic label** | «تعديل ألوان رأس وذيل التقرير» · «إعدادات الخلفية» |
| **Actor** | Laboratory administrator |
| **Trigger** | Configuration change |
| **Preconditions** | — |
| **Workflow** | Edit `colora`…`colort` per profile; edit `color1`…`color19` in `REPORT.dll`; set `backcolor`. |
| **Inputs** | Colour values. |
| **Outputs** | Updated configuration. |
| **Resulting state** | Report colours and the interface background change. |
| **Related business rules** | — |
| **Related entities** | `Data\*.ini`, `REPORT.dll` |
| **Relevant fields** | `colora`…`colort`, `color1`…`color19`, `backcolor` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `reportsetup2` |
| **Related modules** | — |
| **Technical mechanisms** | BGR colour integers. |
| **Evidence** | `REPORT.dll` (562-byte text file); `Data\*.ini` keys; vendor guide 6-6, 9-7 |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The precedence between the 19-entry global palette and the 20-entry per-profile palette is NOT ESTABLISHED.** |

### V2-REQ-037 — Referring party and treating physician management

> **The system shall** allow an administrator to maintain referring parties with their commission terms, and separately to maintain treating physicians.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-037 |
| **Function ID** | V2-FUNC-022 |
| **Domain** | Patient / Finance |
| **Description** | The V2 split of referring party and physician. |
| **Arabic label** | «إضافة جهة إحالة» · «إضافة طبيب معالج» |
| **Actor** | Laboratory administrator |
| **Trigger** | Party editor opened |
| **Preconditions** | Administrator rights. |
| **Workflow** | Add / edit a referring party with `omola`; add / edit a treating physician. |
| **Inputs** | Party details; commission; physician identity. |
| **Outputs** | `Gehats` rows; physician entries. |
| **Resulting state** | Both become selectable. |
| **Related business rules** | **V2-BR-042** |
| **Related entities** | `Gehats`, `patientinfo` (`mowzaf`, `referredbyname`, `referredbysefa`) |
| **Relevant fields** | `omola`, `ReferredPW`, `ReferredID`, `pricemethod`, `mowzaf` |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `z_gehats` |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide — «إضافة جهة إحالة» and «إضافة طبيب معالج»; `Gehats` schema (19 columns) with `ReferredPW`/`ReferredID` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The physician master data's storage location is NOT ESTABLISHED — `mowzaf` is a column on `patientinfo` and `payment`, and no physician table was located. |

### V2-REQ-038 — Patient picture attachment and display

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-038 |
| **Function ID** | V2-FUNC-023 |
| **Domain** | Patient |
| **Description** | Images associated with a patient, displayed in the application and in a standalone gallery. |
| **Arabic label** | — |
| **Actor** | Lab operator |
| **Trigger** | Picture frame opened |
| **Preconditions** | Patient selected. |
| **Workflow** | Open `Frm_pic` / `Frm_pic2` → read the patient's images → display; optionally launch the gallery executable. |
| **Inputs** | Patient identifier. |
| **Outputs** | Displayed images. |
| **Resulting state** | Read-only. |
| **Related business rules** | — |
| **Related entities** | `pictures` (73 columns, 2 rows) |
| **Relevant fields** | — |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `Frm_pic` (49), `Frm_pic2` (38) |
| **Related modules** | — |
| **Technical mechanisms** | The standalone gallery executable |
| **Evidence** | Form inventory; `pictures` table present in the 2019 schema (created by the applied DDL) |
| **Confidence** | INFERRED |
| **Behaviour class** | INFERRED |
| **Known limitations** | The linkage is inferred from the two image frames and the gallery executable. |

### V2-REQ-039 — Method sheet and critical-value consultation

> **The system shall** make the method sheet for the analyte under entry available to the operator, including the sample type, measurement principle, clinical significance and critical-value thresholds.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-039 |
| **Function ID** | V2-FUNC-024 |
| **Domain** | Clinical knowledge |
| **Description** | The method-sheet layer. |
| **Arabic label** | — (the sheets are Arabic documents) |
| **Actor** | Lab operator |
| **Trigger** | Consulting the sheet |
| **Preconditions** | Signed in. |
| **Workflow** | The sheet is retrieved from `Cache\*.rtf` and shown beside the entry field. |
| **Inputs** | The test identifier. |
| **Outputs** | Method text; critical thresholds. |
| **Resulting state** | Read-only. |
| **Related business rules** | — |
| **Related entities** | `Cache\*.rtf` (29 sheets), `normalrange`, `testtable` (`typeofsample`, `testdiscr`, `testeffect`, `testusage`) |
| **Relevant fields** | — |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | `Cache\` presence. |
| **Related screens** | `result*` |
| **Related modules** | — |
| **Technical mechanisms** | RTF rendering. |
| **Evidence** | `analysis/BATCH-02/rtf/`; `testtable` methodology columns |
| **Confidence** | CONFIRMED for the artefact; INFERRED for the presentation |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **Only 29 sheets exist for 609 tests, and the methodology text also exists as four `testtable` columns — the two sources are not shown to be reconciled.** |

### V2-REQ-040 — Interactive training simulators

> **The system shall** ship standalone interactive simulators that rehearse clinical workflows without access to the live laboratory database.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-040 |
| **Function ID** | V2-FUNC-025 |
| **Domain** | Training |
| **Description** | The training library. |
| **Arabic label** | — |
| **Actor** | Trainee |
| **Trigger** | Launching a simulator |
| **Preconditions** | Flash Player 8 available. |
| **Workflow** | Run → the Flash presentation runs → the trainee navigates and receives feedback. |
| **Inputs** | Navigational input. |
| **Outputs** | Training feedback. |
| **Resulting state** | Read-only. |
| **Related business rules** | — |
| **Related entities** | `Cache\*.exe` (13) |
| **Relevant fields** | — |
| **Permissions** | None. |
| **Exceptions** | — |
| **Dependencies** | Flash Player 8. |
| **Related screens** | 13 simulations, 379 frames, 640×480 |
| **Related modules** | — |
| **Technical mechanisms** | Flash; Word-binary projector tables |
| **Evidence** | `analysis/BATCH-03/flash/`; `analysis/BATCH-09/` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The simulators are byte-identical between the two versions** — the training material was not updated in seven years. |

### V2-REQ-041 — Report printing and audit

> **The system shall** render the clinical report using the profile's layout, dispatch it to the bound printer, and record the print against the test row with the printing user, the date and the report sequence number.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-041 |
| **Function ID** | V2-FUNC-026 |
| **Domain** | Reporting / Audit |
| **Description** | Print dispatch and print audit. |
| **Arabic label** | — |
| **Actor** | Any operator |
| **Trigger** | F11 / F12 |
| **Preconditions** | Result reviewed; a printer bound. |
| **Workflow** | `printerselect` resolves the printer → the `DataReport` renders using `Data\*.ini` → dispatch → print audit written. |
| **Inputs** | — |
| **Outputs** | Printed report; audit record. |
| **Resulting state** | The report is issued and attributed. |
| **Related business rules** | **V2-BR-045** |
| **Related entities** | `patienttest`, `Data\*.ini` |
| **Relevant fields** | `printwith`, `printuser`, `printdate`, `printnum`, `resultprint` |
| **Permissions** | Signed-in session. |
| **Exceptions** | «Can't find any printer.» |
| **Dependencies** | A bound, present printer. |
| **Related screens** | Print preview |
| **Related modules** | `printerselect` |
| **Technical mechanisms** | VB6 `Printer` object; spooler API |
| **Evidence** | CHM `009.htm`; `patienttest` print columns |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | — |

### V2-REQ-042 — Patient invoice and ID card

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-042 |
| **Function ID** | V2-FUNC-027 |
| **Domain** | Finance / Reporting |
| **Description** | The invoice and the ID card. |
| **Arabic label** | «Welcome To RealLab» · «Thank you for visiting RealLab!» |
| **Actor** | Reception clerk |
| **Trigger** | Invoice or card printed |
| **Preconditions** | Patient record exists. |
| **Workflow** | Read `IDCARD.INI` → render the card/invoice → print; the ID card uses `CardFrm` / `CardFrmSetx`. |
| **Inputs** | Patient data. |
| **Outputs** | Printed card or invoice. |
| **Resulting state** | — |
| **Related business rules** | — |
| **Related entities** | `patientinfo`, `barcodes` |
| **Relevant fields** | `IDCARD.INI` layout keys |
| **Permissions** | Signed-in session. |
| **Exceptions** | «Can't find any printer.» |
| **Dependencies** | `IDCARD.INI` |
| **Related screens** | `frm_money`, `moneypaid`, `CardFrm`, `CardFrmSetx` |
| **Related modules** | `printerselect` |
| **Technical mechanisms** | — |
| **Evidence** | `IDCARD.INI`; CHM `005.htm`; the `CardFrm` / `CardFrmSetx` objects |
| **Confidence** | STRONGLY SUPPORTED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | **The vendor's address, telephone, website, e-mail and Facebook page are printed on the invoice by default.** |

### V2-REQ-043 — PDF production

> **The system shall** produce a PDF of the clinical report by dispatching the print job to the Windows printer named `Bullzip PDF Printer` and filing the resulting file under one of the four PDF output categories.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-043 |
| **Function ID** | V2-FUNC-028 |
| **Domain** | Reporting / PDF |
| **Description** | The PDF output path. |
| **Arabic label** | — |
| **Actor** | Lab operator; reception clerk |
| **Trigger** | Print request with the PDF printer selected |
| **Preconditions** | Bullzip installed under that exact name; ‹.NET Framework 4.0› installed; a category selected; the result reviewed. |
| **Workflow** | Select the category → resolve the printer → render via `DataReport` → dispatch to the Bullzip port driver → bundled Ghostscript renders → the PDF is written to `D:\PDF\<category>\` → the print is audited. |
| **Inputs** | Patient; test selection; category; file name. |
| **Outputs** | The PDF file; the audit record. |
| **Resulting state** | The result is issued in PDF form into a named business category. |
| **Related business rules** | **V2-BR-001, V2-BR-002, V2-BR-003, V2-BR-004** |
| **Related entities** | `patienttest`; the filesystem at `D:\PDF\` |
| **Data read** | `patientinfo`, `patienttest`, profile table, `Data\*.ini` |
| **Data created** | The PDF file |
| **Data updated** | `patienttest.resultprint`, `printuser`, `printdate`, `printnum` |
| **Data deleted** | None |
| **Permissions** | Signed-in session. |
| **Exceptions** | «Can't find any printer.»; «Not found any Verified selected tests or result to print it». **No fallback is evidenced.** |
| **Dependencies** | Bullzip PDF Printer 11.1; ‹.NET 4.0›; the Windows spooler. |
| **Related screens** | Print preview |
| **Related modules** | `printerselect` |
| **Technical mechanisms** | Bullzip port driver `bzpdf.dll`; bundled Ghostscript. **No PDF logic in the application.** |
| **Evidence** | Literals at `0x6A2486` and `0x6A255E`; the `D:\PDF\*` literals; `SetupPZ.bat`; `analysis/BATCH-11/` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The device name and the driver's version are both hard-coded.** |

### V2-REQ-044 — PDF output categorisation

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-044 |
| **Function ID** | V2-FUNC-028 |
| **Domain** | Reporting / PDF |
| **Description** | The four output categories. |
| **Arabic label** | — |
| **Actor** | Lab operator |
| **Trigger** | PDF print request |
| **Preconditions** | — |
| **Workflow** | Select one of `Contracts`, `Free`, `Individual`, `LabToLab` → the file is written under `D:\PDF\<category>\`. |
| **Inputs** | The category. |
| **Outputs** | The file path. |
| **Resulting state** | — |
| **Related business rules** | **V2-BR-003** |
| **Related entities** | `patientinfo.pricemethod` (parallel taxonomy) |
| **Relevant fields** | `pricemethod` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | The `D:\PDF\` root |
| **Related screens** | The print dialog |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | The `D:\PDF\` literal cluster |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The mapping between a `pricemethod` value and a PDF directory is NOT ESTABLISHED.** The default category is UNKNOWN. |

### V2-REQ-045 — PDF default filename

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-045 |
| **Function ID** | V2-FUNC-028 |
| **Domain** | Reporting / PDF |
| **Description** | The default output filename is a hard-coded literal. |
| **Arabic label** | — |
| **Actor** | — (system behaviour) |
| **Trigger** | PDF print request |
| **Preconditions** | — |
| **Workflow** | The default name is taken from the literal `Bullzip PDF Printer 11.1 - Create File`. |
| **Inputs** | — |
| **Outputs** | The default file name. |
| **Resulting state** | — |
| **Related business rules** | **V2-BR-002** |
| **Related entities** | — |
| **Relevant fields** | — |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | Bullzip 11.1 installed for the name to correspond to anything |
| **Related screens** | The print dialog |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Literal at `0x6A255E` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The filename construction rule is NOT ESTABLISHED. |

### V2-REQ-046 — PDF dependency on the external driver

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-046 |
| **Function ID** | V2-FUNC-028 |
| **Domain** | Reporting / PDF / Deployment |
| **Description** | PDF capability depends on the installation of a third-party printer driver and its .NET prerequisite. |
| **Arabic label** | — |
| **Actor** | System administrator |
| **Trigger** | Installation |
| **Preconditions** | Administrator rights. |
| **Workflow** | Install ‹.NET Framework 4.0› → install Bullzip PDF Printer 11.1 → the printer registers with Windows. |
| **Inputs** | — |
| **Outputs** | A registered PDF printer. |
| **Resulting state** | The PDF path is available. |
| **Related business rules** | **V2-BR-004, V2-BR-031** |
| **Related entities** | — |
| **Relevant fields** | — |
| **Permissions** | Administrator rights. |
| **Exceptions** | The installation order is defective — see V2-BR-031. |
| **Dependencies** | `System\PDF\dotNetFx40_Full_x86_x64.exe`, `Setup_BullzipPDFPrinter_11_1_0_2600_PRO_EXP.exe` |
| **Related screens** | The installer |
| **Related modules** | — |
| **Technical mechanisms** | Bullzip port driver; bundled Ghostscript |
| **Evidence** | `SetupPZ.bat`, `SetupP.bat`, `SetupFN4.bat`; `analysis/BATCH-11/pe-envelope.csv` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **Three of the five PDF-stack executables are unsigned; the two signed ones use SHA-1 and fail verification.** |

### V2-REQ-047 — NATIGH result publication

> **The system shall** publish a reviewed patient result to the NATIGH.COM service so that the patient can retrieve it, and shall print a receipt configured for that channel.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-047 |
| **Function ID** | V2-FUNC-029 |
| **Domain** | Result delivery |
| **Description** | Remote result delivery. |
| **Arabic label** | «تسليم نتائج المرضى عن طريق موقع نتتيجة (NATIGH.COM)» |
| **Actor** | Lab operator / reception clerk |
| **Trigger** | Publication action |
| **Preconditions** | Signed in; a NATIGH account exists; the result is reviewed; outbound internet connectivity. |
| **Workflow** | Select the patient and result → publish → the patient opens the site and views the result → a receipt is printed with the configured connection header and footer. |
| **Inputs** | Patient identity; result data; NATIGH account. |
| **Outputs** | The published result; the printed receipt. |
| **Resulting state** | The result is externally published. |
| **Related business rules** | **V2-BR-005, V2-BR-006, V2-BR-022** |
| **Related entities** | `Natigh` (installed file), `IDCARD.INI`, `SETTING.ini` |
| **Data read** | `patientinfo`, `patienttest` |
| **Data created** | The external publication |
| **Data updated** | **None recorded — the `NatighU` flag column is absent from the shipped database.** |
| **Data deleted** | None |
| **Permissions** | Signed-in session. |
| **Exceptions** | Unknown — the failure handling for an unreachable service is NOT documented. |
| **Dependencies** | Outbound HTTPS; the NATIGH account configuration. |
| **Related screens** | The NATIGH facility; the receipt |
| **Related modules** | — |
| **Technical mechanisms** | The installed `Natigh` file |
| **Evidence** | Vendor guide 1-12, 1-12a, 1-12b; the `Natigh` file; the DDL for `patienttest.natighu` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The feature is code-present but schema-absent in the shipped Access database.** |

### V2-REQ-048 — NATIGH result retrieval by the patient

> **The system shall** open the NATIGH.COM site so that the patient can display the published result.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-048 |
| **Function ID** | V2-FUNC-029 |
| **Domain** | Result delivery |
| **Description** | The patient-side view of the delivery. |
| **Arabic label** | «كيفية فتح الموقع وعرض نتيجة المريض» |
| **Actor** | The patient, or an operator demonstrating it |
| **Trigger** | Opening the site |
| **Preconditions** | The result has been published |
| **Workflow** | Open the site → navigate to the result → display. |
| **Inputs** | The patient's access details. |
| **Outputs** | The displayed result. |
| **Resulting state** | — |
| **Related business rules** | **V2-BR-006** |
| **Related entities** | `Natigh` |
| **Relevant fields** | — |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | Outbound internet connectivity. |
| **Related screens** | The NATIGH web page |
| **Related modules** | — |
| **Technical mechanisms** | The installed `Natigh` file |
| **Evidence** | Vendor guide 1-12a |
| **Confidence** | CONFIRMED as a documented capability; UNKNOWN as an implementation |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The application-side behaviour is not evidenced — only the vendor's description of the patient-facing page. |

### V2-REQ-049 — SMS queueing

> **The system shall** record every outbound SMS message in a queue carrying the recipient number, the message text, its length, the patient, the doctor's name and a timestamp.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-049 |
| **Function ID** | V2-FUNC-030 |
| **Domain** | Delivery / Audit |
| **Description** | The SMS audit queue. |
| **Arabic label** | — |
| **Actor** | Lab operator |
| **Trigger** | SMS dispatch |
| **Preconditions** | — |
| **Workflow** | Select the recipient → compose from a template → send → write the queue row. |
| **Inputs** | Recipient; template; patient context. |
| **Outputs** | An `SMSSender` row; the SMS. |
| **Resulting state** | The message is queued and auditable. |
| **Related business rules** | **V2-BR-007** |
| **Related entities** | `SMSSender`, `SMStemplate`, `smslist`, `smsgroups` |
| **Relevant fields** | `SMSautonum`, `patientid`, `PatDrName`, `requestdate`, `smsMobile`, `SMSLength`, `SMStext` |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | The SMS gateway |
| **Related screens** | `SMSfrmsenderos` |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | DDL for `SMSSender`; `SMSfrmsenderos` (139 methods) |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The mobile number and the message body are stored in clear text with no evidenced retention limit.** |

### V2-REQ-050 — SMS recipients and templates

> **The system shall** maintain SMS recipient lists organised into groups, and maintain reusable message templates.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-050 |
| **Function ID** | V2-FUNC-030 |
| **Domain** | Delivery |
| **Description** | The SMS configuration tables. |
| **Arabic label** | — |
| **Actor** | Laboratory administrator |
| **Trigger** | Recipient or template administration |
| **Preconditions** | Administrator rights |
| **Workflow** | Maintain `smslist` (ID, smstype, smsphone, smsname, smsgroup) and `smsgroups` (ID, grouptype, smsgroup); maintain `SMStemplate` (templatenum, smstext). |
| **Inputs** | Phone number; name; group; template text. |
| **Outputs** | The configuration rows. |
| **Resulting state** | — |
| **Related business rules** | **V2-BR-007** |
| **Related entities** | `smslist`, `smsgroups`, `SMStemplate` |
| **Relevant fields** | `smstype`, `smsphone`, `smsname`, `smsgroup`, `grouptype`, `templatenum`, `smstext` |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | The SMS configuration screens |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | DDL for the three tables; the `sms*` object family |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The `smstype` and `grouptype` value domains are NOT ESTABLISHED. |

### V2-REQ-051 — Branch creation

> **The system shall** allow an administrator to create a branch and seed its database schema from the branch template.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-051 |
| **Function ID** | V2-FUNC-031 |
| **Domain** | Branches |
| **Description** | Branch provisioning. |
| **Arabic label** | «نظام الفروع للمعامل» |
| **Actor** | System administrator |
| **Trigger** | Branch creation |
| **Preconditions** | Administrator rights. |
| **Workflow** | Create the `Labbranch` record → apply the `Branch_Data.mdb` schema → assign the branch code → the code is written into position 9 of every patient identifier created for that branch. |
| **Inputs** | Branch master data; branch code. |
| **Outputs** | A `Labbranch` row; a seeded schema. |
| **Resulting state** | The branch is operational. |
| **Related business rules** | **V2-BR-008, V2-BR-009, V2-BR-025** |
| **Related entities** | `Labbranch`, `Branch_Data.mdb` |
| **Relevant fields** | `branchid`, `branch` |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | `SETTING.ini [setup] labbranchx` |
| **Related screens** | `z_branchs` (21) |
| **Related modules** | — |
| **Technical mechanisms** | Schema application from a 49-table, 0-row template. |
| **Evidence** | Vendor guide 5-2; `Labbranch`; `Branch_Data.mdb` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The template carries **no reference data**, so the new branch starts with no catalogue, ranges or prices. |

### V2-REQ-052 — Branch code in the identifier

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-052 |
| **Function ID** | V2-FUNC-031, V2-FUNC-003 |
| **Domain** | Branches / Data |
| **Description** | The branch segment's position. |
| **Arabic label** | — |
| **Actor** | — (system behaviour) |
| **Trigger** | Identifier allocation |
| **Preconditions** | — |
| **Workflow** | Write the branch digit at position 9; read it back with `MID(patientid,9,1) as bran`. |
| **Inputs** | Branch code. |
| **Outputs** | The identifier. |
| **Resulting state** | — |
| **Related business rules** | **V2-BR-008, V2-BR-040** |
| **Related entities** | `patientinfo`, `patienttest` |
| **Relevant fields** | `patientid` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `addpatient` |
| **Related modules** | — |
| **Technical mechanisms** | `MID()` extraction. |
| **Evidence** | `MID(patientid,9,1) as bran` in the SQL corpus |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | — |

### V2-REQ-053 — Branch-scoped accounting

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-053 |
| **Function ID** | F-V2-031, V2-FUNC-015 |
| **Domain** | Accounting / Branches |
| **Description** | Accounting figures are attributable to a branch. |
| **Arabic label** | «عمل جرد على فرع ضمن عدة فروع مرتبطة معا» |
| **Actor** | Master-laboratory manager |
| **Trigger** | Branch scope selected |
| **Preconditions** | Branches registered. |
| **Workflow** | Select the branch scope → the accounting listings are scoped accordingly. |
| **Inputs** | Branch selection. |
| **Outputs** | Branch-scoped figures. |
| **Resulting state** | Read-only. |
| **Related business rules** | **V2-BR-010** |
| **Related entities** | `Labbranch`, `payment`, `patientinfo`, `patienttest` |
| **Relevant fields** | `patientid` position 9; **`patients.branche`** |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | Accounting window |
| **Related modules** | `labfunction` |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide 5-2 |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The filtering mechanism is NOT ESTABLISHED. |

### V2-REQ-054 — Laboratory statistics

> **The system shall** produce laboratory statistics computed from the transactional tables, including patient counts by year sorted by month and by sex, patient counts by month, request rates for a test group, and annual sample counts.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-054 |
| **Function ID** | V2-FUNC-032 |
| **Domain** | Management reporting |
| **Description** | The four worked-example statistics. |
| **Arabic label** | «الإحصائيات ومتابعة أداء المعمل» |
| **Actor** | Master-laboratory manager |
| **Trigger** | Statistic and period selected |
| **Preconditions** | Signed in. |
| **Workflow** | Select the statistic → select the period → aggregate over `patientinfo` and `patienttest`. |
| **Inputs** | Statistic type; period; optional test group. |
| **Outputs** | Counted and grouped figures. |
| **Resulting state** | Read-only. |
| **Related business rules** | **V2-BR-011** |
| **Related entities** | `patientinfo` (`requestdate`, `patientsex`), `patienttest` (`subgroupcode`) |
| **Data read** | `patientinfo`, `patienttest` |
| **Data created / updated / deleted** | None established |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | The statistics screen |
| **Related modules** | `statisReport` |
| **Technical mechanisms** | Aggregation queries over the transactional tables. |
| **Evidence** | Vendor guide 4-1 … 4-5; the `statisReport` object |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The full statistic catalogue is NOT ESTABLISHED — only four worked examples are documented. **No aggregate store exists, so every statistic rescans the transactional tables.** |

### V2-REQ-055 — Performance monitoring

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-055 |
| **Function ID** | V2-FUNC-032 |
| **Domain** | Management reporting |
| **Description** | The statistics module is described as covering both statistics and performance monitoring. |
| **Arabic label** | «الإحصائيات ومتابعة أداء المعمل» |
| **Actor** | Master-laboratory manager |
| **Trigger** | — |
| **Preconditions** | Signed in. |
| **Workflow** | — |
| **Inputs** | — |
| **Outputs** | Performance figures. |
| **Resulting state** | Read-only. |
| **Related business rules** | V2-BR-011 |
| **Related entities** | `patientinfo`, `patienttest`, `loger` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | The statistics screen |
| **Related modules** | `statisReport`, `loger` |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide 4-1; the `statisReport` object; the `loger` table |
| **Confidence** | STRONGLY SUPPORTED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | **The performance indicators themselves are NOT ESTABLISHED** — the guide names the capability but does not enumerate the measures. |

### V2-REQ-056 — Attendance register

> **The system shall** maintain an attendance register recording operator movement in and out of the laboratory.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-056 |
| **Function ID** | V2-FUNC-033 |
| **Domain** | Security / Workforce |
| **Description** | The `loger` register. |
| **Arabic label** | «تسجيل الحضور والانصراف لمعرفة حركة المستخدمين في الدخول والخروج» |
| **Actor** | Any operator (automatic) |
| **Trigger** | Sign-in / sign-out |
| **Preconditions** | Signed in. |
| **Workflow** | Sign in → a `loger` row is written → sign out → the departure is recorded. |
| **Inputs** | Operator; timestamp. |
| **Outputs** | `loger` rows. |
| **Resulting state** | Presence recorded. |
| **Related business rules** | **V2-BR-012, V2-BR-041** |
| **Related entities** | `loger`, `loggroup` |
| **Relevant fields** | — (the `loger` schema was not extracted) |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | — |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide 3-11; the `loger` table |
| **Confidence** | CONFIRMED for the capability; UNKNOWN for the schema |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The `loger` column structure was not extracted in this consolidation.** Note that the operator record also carries attendance fields in **both** versions. |

### V2-REQ-057 — Patient-group work sheet

> **The system shall** produce a work sheet covering a group of patients within a specified date period.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-057 |
| **Function ID** | V2-FUNC-034 |
| **Domain** | Batch reporting |
| **Description** | Work sheet over a patient group. |
| **Arabic label** | «عمل ورقة عمل لمجموعة من المرضى في فترة زمنية محددة» |
| **Actor** | Lab operator |
| **Trigger** | Work sheet requested |
| **Preconditions** | Signed in. |
| **Workflow** | Select the patient group and the period → the work sheet is generated. |
| **Inputs** | Patient group; period. |
| **Outputs** | The work sheet; `wasl` rows. |
| **Resulting state** | Read-only. |
| **Related business rules** | **V2-BR-013** |
| **Related entities** | `wasl`, `patientinfo`, `patienttest` |
| **Relevant fields** | `wasl` — 12 columns, semantics UNKNOWN |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | `SETTING.ini [setup] printworkpaper` |
| **Related screens** | The work-sheet screen |
| **Related modules** | `workpaper`, `workpaper2` |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide 2-1; the `workpaper` / `workpaper2` objects; the `wasl` table |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The distinction between `workpaper` and `workpaper2` is NOT ESTABLISHED. |

### V2-REQ-058 — Test-group work sheet

> **The system shall** produce a work sheet covering a specified test or test group within a specified date period.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-058 |
| **Function ID** | V2-FUNC-035 |
| **Domain** | Batch reporting |
| **Description** | Work sheet over a test group. |
| **Arabic label** | «عمل ورقة العمل لتحليل معين أو مجموعة تحاليل في فترة زمنية محددة» |
| **Actor** | Lab operator |
| **Trigger** | Work sheet requested |
| **Preconditions** | Signed in. |
| **Workflow** | Select the test or test group and the period → the work sheet is generated. |
| **Inputs** | Test or group; period. |
| **Outputs** | The work sheet; `wasl` rows. |
| **Resulting state** | Read-only. |
| **Related business rules** | **V2-BR-013** |
| **Related entities** | `wasl`, `testtable`, `patienttest` |
| **Relevant fields** | `subgroupcode`, `testcode` |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | `SETTING.ini [setup] printworkpaper` |
| **Related screens** | The work-sheet screen |
| **Related modules** | `workpaper`, `workpaper2`, `sarfz` |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide 2-2 |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | — |

### V2-REQ-059 — Farm result reporting

> **The system shall** allow a laboratory to register a farm and enter and report results against it.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-059 |
| **Function ID** | V2-FUNC-036 |
| **Domain** | Veterinary / agricultural laboratory |
| **Description** | The farm vertical. |
| **Arabic label** | «إدخال نتائج تقارير المزارع» · «إضافة مزرعة جديدة للبرنامج» |
| **Actor** | Lab operator |
| **Trigger** | Farm result entry |
| **Preconditions** | A farm is registered. |
| **Workflow** | Register the farm → select it → enter the result → produce the farm report. |
| **Inputs** | Farm; result. |
| **Outputs** | `sharekat` rows; the farm report. |
| **Resulting state** | — |
| **Related business rules** | **V2-BR-014** |
| **Related entities** | `sharekat`, `sharekathesab` |
| **Relevant fields** | — (structures not extracted) |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | The farm screens |
| **Related modules** | `sharekat`, `sharekathesab`, `sarfz` |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide 1-8, 3-9; the `sharekat` / `sharekathesab` tables |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The table structures were not extracted. |

### V2-REQ-060 — Antibiotic catalogue maintenance

> **The system shall** allow an administrator to maintain the antibiotic catalogue used in culture-and-sensitivity reporting.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-060 |
| **Function ID** | V2-FUNC-037 |
| **Domain** | Clinical / microbiology |
| **Description** | The antibiotic catalogue. |
| **Arabic label** | «إضافة مضاد حي للمزرعة» |
| **Actor** | Laboratory administrator |
| **Trigger** | Catalogue opened |
| **Preconditions** | Administrator rights. |
| **Workflow** | Open the catalogue → add or edit an antibiotic → save → it becomes selectable in the microbiology workflow. |
| **Inputs** | Antibiotic identity and properties. |
| **Outputs** | `antibiotic` rows (38 / 25 cols); `antibio` (96 rows). |
| **Resulting state** | The catalogue changes. |
| **Related business rules** | **V2-BR-015** |
| **Related entities** | `antibiotic`, `antibio`, `cultures`, `cultureanti` |
| **Relevant fields** | — (not extracted at field level) |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | The antibiotic editor |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide 3-10; the two tables |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The relationship between the 25-column and the 2-column table is UNKNOWN.** |

### V2-REQ-061 — Sample splitting and withdrawal

> **The system shall** record the splitting and withdrawal of samples within the microbiology workflow.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-061 |
| **Function ID** | V2-FUNC-038 |
| **Domain** | Laboratory logistics |
| **Description** | Sample disposition. |
| **Arabic label** | «فصل وسحب العينات» |
| **Actor** | Lab operator |
| **Trigger** | The Laboratory icon |
| **Preconditions** | Signed in. |
| **Workflow** | Open the Laboratory function → select the sample → record the split or withdrawal. |
| **Inputs** | Sample; disposition. |
| **Outputs** | Updated microbiology records. |
| **Resulting state** | Disposition recorded. |
| **Related business rules** | **V2-BR-016** |
| **Related entities** | `cultures`, `cultureanti`, `sample` |
| **Relevant fields** | — (structures not extracted) |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | The Laboratory function |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide — «فصل وسحب العينات» |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The `sample` table structure is NOT ESTABLISHED. |

### V2-REQ-062 — Consolidated report

> **The system shall** produce a single report consolidating results drawn from several clinical profiles.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-062 |
| **Function ID** | V2-FUNC-039 |
| **Domain** | Reporting |
| **Description** | Cross-profile consolidation. |
| **Arabic label** | «عمل تقرير مجمع» |
| **Actor** | Lab operator |
| **Trigger** | Report request |
| **Preconditions** | Signed in; results selected. |
| **Workflow** | Select the results → the system assembles a union across the selected result sets → the consolidated report is produced. |
| **Inputs** | Result selection. |
| **Outputs** | A combined report. |
| **Resulting state** | Read-only. |
| **Related business rules** | **V2-BR-017** |
| **Related entities** | `anyreport`, multiple profile tables |
| **Relevant fields** | `text1`…`text22` |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | The `MSDataShape.1` provider |
| **Related screens** | The report screen |
| **Related modules** | `Reportanyreport` |
| **Technical mechanisms** | `SHAPE {…} APPEND (…)` |
| **Evidence** | Vendor guide 1-6; the `SHAPE`/`APPEND` statements; `data_connection1.udl` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **Appears bound to the SQL Server path**; the Access behaviour is NOT ESTABLISHED. |

### V2-REQ-063 — Empty report

> **The system shall** produce a report with no patient data.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-063 |
| **Function ID** | V2-FUNC-040 |
| **Domain** | Reporting |
| **Description** | The blank report. |
| **Arabic label** | «عمل تقرير فارغ» |
| **Actor** | Lab operator |
| **Trigger** | Report request |
| **Preconditions** | Signed in. |
| **Workflow** | The report template is produced without patient rows. |
| **Inputs** | Template selection. |
| **Outputs** | A blank report. |
| **Resulting state** | Read-only. |
| **Related business rules** | — |
| **Related entities** | `anyreport` (26 columns, 15 rows) |
| **Relevant fields** | — |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | `Data\*.ini` |
| **Related screens** | The report screen |
| **Related modules** | `anyreport` |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide 1-7; the `anyreport` table |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The 15 `anyreport` rows are report templates; their content is NOT ESTABLISHED. |

### V2-REQ-064 — Price list maintenance

> **The system shall** allow an administrator to create and maintain price lists.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-064 |
| **Function ID** | V2-FUNC-041 |
| **Domain** | Finance / Catalogue |
| **Description** | Price-list administration. |
| **Arabic label** | «إضافة قائمة أسعار جديدة» |
| **Actor** | Laboratory administrator |
| **Trigger** | Price-list administration opened |
| **Preconditions** | Administrator rights. |
| **Workflow** | Open `z_pricelists` → define or modify a price list → save. |
| **Inputs** | Price-list definition. |
| **Outputs** | `pricelist`, `pricemenu`, `shopping` rows. |
| **Resulting state** | The price list changes. |
| **Related business rules** | **V2-BR-018** |
| **Related entities** | `pricelist`, `pricemenu`, `shopping` |
| **Relevant fields** | — (not extracted) |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `z_pricelists` (28) |
| **Related modules** | `pricemenu`, `shopping` |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide 3-4; the three tables |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The table structures were not extracted at field level. |

### V2-REQ-065 — Price list printing

> **The system shall** print the test price list.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-065 |
| **Function ID** | V2-FUNC-041 |
| **Domain** | Finance / Reporting |
| **Description** | The printed price list. |
| **Arabic label** | «طباعة قائمة أسعار التحاليل» |
| **Actor** | Laboratory administrator |
| **Trigger** | Print action |
| **Preconditions** | A price list exists. |
| **Workflow** | The dedicated report object renders the list. |
| **Inputs** | — |
| **Outputs** | The printed price list. |
| **Resulting state** | Read-only. |
| **Related business rules** | **V2-BR-018** |
| **Related entities** | `pricelist`, `testtable` |
| **Relevant fields** | `patientprice`, `labprice` |
| **Permissions** | Signed-in session. |
| **Exceptions** | «Can't find any printer.» |
| **Dependencies** | A bound printer |
| **Related screens** | The price-list screen |
| **Related modules** | `pricelistsN` |
| **Technical mechanisms** | `DataReport` |
| **Evidence** | Vendor guide 3-5; the `pricelistsN` report object |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The price list is a client-facing commercial document — its layout is not established. |

### V2-REQ-066 — Default account type

> **The system shall** hold a single system-wide default account type governing how prices, discounts, commissions and withdrawn samples are applied.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-066 |
| **Function ID** | V2-FUNC-042 |
| **Domain** | Finance / Configuration |
| **Description** | The default account method. |
| **Arabic label** | «ضبط نوع الحساب الافتراضي للنظام (أسعار وخصومات وعولات وعينات مسحوبة)» |
| **Actor** | Laboratory administrator |
| **Trigger** | Configuration change |
| **Preconditions** | Administrator rights. |
| **Workflow** | Open the setting → select the account type → save → it applies to new invoices. |
| **Inputs** | The account type. |
| **Outputs** | The stored default. |
| **Resulting state** | New invoices price by the new default. |
| **Related business rules** | **V2-BR-019** |
| **Related entities** | `patientinfo.pricemethod`, `Gehats.pricemethod` |
| **Relevant fields** | `pricemethod` |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | The accounting-method setting |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide 7-7; `patientinfo.pricemethod` Text(10); `Gehats.pricemethod` Text(20) |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The `pricemethod` value domain is NOT ESTABLISHED** — 0 hits for the three previously-cited values across 1,337 SQL literals and 0 rows in `patientinfo`. The override precedence between the system default, the party method and the patient method is NOT ESTABLISHED. |

### V2-REQ-067 — Connection receipt configuration

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-067 |
| **Function ID** | V2-FUNC-043 |
| **Domain** | Reporting / Configuration |
| **Description** | The receipt layout for NATIGH-delivered results. |
| **Arabic label** | «إعدادات الاتصال (رأس وذيل الاتصال - هوامش الاتصال)» |
| **Actor** | Laboratory administrator |
| **Trigger** | Configuration change |
| **Preconditions** | Administrator rights. |
| **Workflow** | Set the receipt header, footer and margins → save to `IDCARD.INI` / `SETTING.ini`. |
| **Inputs** | Header/footer text; margins. |
| **Outputs** | Updated configuration. |
| **Resulting state** | The receipt layout changes. |
| **Related business rules** | — |
| **Related entities** | `IDCARD.INI`, `SETTING.ini` |
| **Relevant fields** | — |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | NATIGH account configuration |
| **Related screens** | The connection settings screen |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide 8-7 |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The individual key names are NOT ESTABLISHED. |

### V2-REQ-068 — Background configuration

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-068 |
| **Function ID** | V2-FUNC-044 |
| **Domain** | Configuration |
| **Description** | The application background colour. |
| **Arabic label** | «إعدادات الخلفية» |
| **Actor** | Laboratory administrator |
| **Trigger** | Configuration change |
| **Preconditions** | — |
| **Workflow** | Set `backcolor` in the profile configuration → save. |
| **Inputs** | Colour. |
| **Outputs** | Updated configuration. |
| **Resulting state** | The interface background changes. |
| **Related business rules** | — |
| **Related entities** | `Data\*.ini` |
| **Relevant fields** | `backcolor` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | — |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide 9-7; the `backcolor` key |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | — |

### V2-REQ-069 — Report margin configuration

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-069 |
| **Function ID** | V2-FUNC-045 |
| **Domain** | Reporting |
| **Description** | Per-profile report margins. |
| **Arabic label** | «إعدادات هوامش التقرير» |
| **Actor** | Laboratory administrator |
| **Trigger** | Configuration change |
| **Preconditions** | Administrator rights. |
| **Workflow** | Set `leftvalue`, `rightvalue`, `topvalue`, `bottomvalue` → save. |
| **Inputs** | Four margin values. |
| **Outputs** | Updated `Data\*.ini`. |
| **Resulting state** | The printable area changes. |
| **Related business rules** | **V2-BR-020** |
| **Related entities** | `Data\*.ini` |
| **Relevant fields** | `leftvalue`, `rightvalue`, `topvalue`, `bottomvalue` |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | `SETTING.ini [setup] papersize` |
| **Related screens** | `reportsetup` |
| **Related modules** | — |
| **Technical mechanisms** | VB6 `Printer` object |
| **Evidence** | Vendor guide 6-3; the `Data\*.ini` keys |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The unit of the margin values is NOT ESTABLISHED. |

### V2-REQ-070 — Report header and footer configuration

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-070 |
| **Function ID** | V2-FUNC-046 |
| **Domain** | Reporting |
| **Description** | Per-profile header and footer texts. |
| **Arabic label** | «إعدادات رأس وذيل التقرير» |
| **Actor** | Laboratory administrator |
| **Trigger** | Configuration change |
| **Preconditions** | Administrator rights. |
| **Workflow** | Set `formsubtitle` and `reportmaintitle` → save. |
| **Inputs** | Header/footer texts. |
| **Outputs** | Updated `Data\*.ini`. |
| **Resulting state** | — |
| **Related business rules** | **V2-BR-021** |
| **Related entities** | `Data\*.ini`, `SETTING.ini` |
| **Relevant fields** | `formsubtitle`, `reportmaintitle`, `Textr1txt`, `Textr2txt` |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | `SETTING.ini` global header texts |
| **Related screens** | `reportsetup` |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide 6-5; the `Data\*.ini` and `SETTING.ini` keys |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The precedence and layout order of the global and per-profile levels is NOT ESTABLISHED. |

### V2-REQ-071 — Case follow-up

> **The system shall** allow a case to be tracked through registration, amendment, result entry and report printing as a unit.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-071 |
| **Function ID** | V2-FUNC-047 |
| **Domain** | Case management |
| **Description** | The longitudinal case. |
| **Arabic label** | «متابعة تسجيل الحالات والتعديل عليها وإدخال النتائج وطباعة التقارير» |
| **Actor** | Lab operator |
| **Trigger** | Case record opened |
| **Preconditions** | Patient selected. |
| **Workflow** | Open the case → review and amend the registration → enter results → print the reports. |
| **Inputs** | Case identifier. |
| **Outputs** | Updated `patienttest` rows; printed reports. |
| **Resulting state** | The case advances. |
| **Related business rules** | — |
| **Related entities** | `patienttest`, `patientinfo`, `history` |
| **Relevant fields** | `resultreview`, `resultprint`, `edituser`, `prevuser` |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `patienthistory`, `patienthistory2` |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide 1-11 |
| **Confidence** | STRONGLY SUPPORTED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | **The "case" is not a separate entity** — it is realised through the patient's test rows. |

### V2-REQ-072 — NATIGH account provisioning

> **The system shall** allow an administrator to create a NATIGH service account for a treating physician or for a laboratory.

| Field | Content |
|---|---|
| **Requirement ID** | V2-REQ-072 |
| **Function ID** | V2-FUNC-048 |
| **Domain** | External service administration |
| **Description** | External account provisioning. |
| **Arabic label** | «إنشاء حساب للطبيب المعالج لاستخدام خدمة نتيجة» · «إنشاء حساب للمعمل لاستخدام خدمة نتيجة» |
| **Actor** | Laboratory administrator |
| **Trigger** | Account creation |
| **Preconditions** | Administrator rights. |
| **Workflow** | Open the NATIGH account administration → choose the account type → enter the details → save → configure the connecting receipt for that account. |
| **Inputs** | Account type; holder details. |
| **Outputs** | An account record. |
| **Resulting state** | The holder can use the result service. |
| **Related business rules** | **V2-BR-022, V2-BR-006** |
| **Related entities** | `smslist`, `smsgroups`, `Natigh` configuration |
| **Relevant fields** | — |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | NATIGH service configuration |
| **Related screens** | The NATIGH account screen |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Vendor guide — the two account-creation entries |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The account store is not established** — it may live entirely in the `Natigh` installed file, outside the Access database. |

---

## 4. Behaviour classification summary

| Behaviour class | Count | Requirement IDs |
|---|---:|---|
| CONFIRMED FUNCTIONAL | 63 | V2-REQ-001 … V2-REQ-022, 024 … 030, 032 … 064, 066 … 070, 072 |
| TECHNICALLY SUPPORTED | 9 | V2-REQ-023 (partially), 031, 033, 042, 055, 071, and the remainder of the mixed-class entries |

> **Note.** The counts above reflect the per-field classification of each card's
> **Behaviour class** row. Several cards carry a single class; the mixed entries are
> listed explicitly in each card.

### 4.1 Requirements explicitly NOT asserted

| Candidate requirement | Why it is not asserted |
|---|---|
| The system shall reject overlapping reference ranges | No validation logic recoverable statically |
| The system shall detect duplicate patient registration | No uniqueness rule evidenced |
| The system shall convert units automatically on entry | Not determinable from the data alone |
| The system shall require a second signature to release a critical result | No mechanism evidenced; absence of evidence is not evidence of absence |
| The system shall escalate critical results | **No escalation mechanism of any kind is evidenced** |
| The system shall enforce password expiry or complexity | Not evidenced |
| The system shall audit failed login attempts | The `loger` schema was not extracted |
| The system shall encrypt data at rest | No evidence of encryption |
| The system shall sign every installer with SHA-256 | **Three of the five shipped PDF installers are unsigned** — the opposite is established |
| The system shall complete the schema migration on every installation | **21 migration objects were never applied** |
| The system shall provide a fallback PDF printer | No fallback is evidenced |
| The system shall delete or anonymise a patient's data on request | No erasure path is evidenced |
| The system shall retain SMS messages for a limited period | No retention mechanism is evidenced |
| The system shall schedule backups | The shipped backup is stale, but no scheduling mechanism is evidenced |

---

## 5. Version applicability

| Requirement family | V-ROOT-1.00 | V-CD-2019 | Basis |
|---|---|---|---|
| Startup, connection, locale guard | Present | Present | CHM byte-identical |
| Sign-in, operator record, permission bits | Present (12 bits) | Present (17 bits) | Schema + DDL |
| Attendance | Present (on the operator record) | Present (operator record **and** `loger`) | Schema + vendor guide |
| Patient registration, barcode, search, history | Present | Present, **+ branch** | Schema |
| Result entry, classification, comment, review | Present | Present, **+ new profiles** | Schema + DDL |
| Hand-over and settlement | Present | Present | CHM byte-identical |
| Test catalogue, ranges, units, abbreviations, groups | Present | Present — **knowledge layer unchanged** | Table comparison |
| Report customisation, printing, invoice | Present | Present, **+ margins / header / background** | INI diff |
| Send-out | Present | Present | CHM byte-identical |
| Accounting and stock-take | Present | Present, **+ branch scope** | Vendor guide |
| Backup | Present | Present | CHM byte-identical |
| **PDF export** | **Absent** | **Present** | Literal analysis |
| **NATIGH / SMS delivery** | **Absent** | **Present** | DDL + object family |
| **Branches** | **Absent** | **Present** | Schema + vendor guide |
| **Statistics** | **Absent** | **Present** | Object + vendor guide |
| **Work sheets** | **Absent** | **Present** | Object + vendor guide |
| **Farm reporting / antibiotics** | **Absent** | **Present** | Tables + vendor guide |
| **Sample splitting** | **Absent** | **Present** | Vendor guide |
| **Price lists** | **Absent** | **Present** | Tables + vendor guide |
| **Consolidated / empty reports** | **Absent** (mechanism present) | **Present** (documented) | Vendor guide |
| **Runtime schema migration** | **Absent** | **Present** | DDL |
| **Multi-engine deployment** | **Absent** | **Present** | Installer |

---

## 6. Traceability index — V-CD-2019

| Function | Workflow | Related business rules | Requirements | Primary entities | Evidence classes |
|---|---|---|---|---|---|
| V2-FUNC-001 | WF-V2-01 | V2-BR-023, 024, 027, 028, 029, 030, 031, 032 | V2-REQ-001, 002, 003 | `BASE.ini`, `SETUP.LST` | ET-DOC, ET-CFG, ET-INST, ET-DDL |
| V2-FUNC-002 | WF-V2-01 | V2-BR-039, 041, 032 | V2-REQ-004, 005, 006 | `patients`, `loggroup`, `loger` | ET-DOC, ET-DB |
| V2-FUNC-003 | WF-V2-01 | V2-BR-008, 040 | V2-REQ-007, 008 | `patientinfo`, `barcodes`, `Labbranch` | ET-DOC, ET-DB, ET-CODE |
| V2-FUNC-004 | WF-V2-01 | — | V2-REQ-009 | `barcodes`, `patientinfo` | ET-DOC, ET-DB |
| V2-FUNC-005 | WF-V2-01 | V2-BR-040 | V2-REQ-010 | `patientinfo` | ET-DOC, ET-CODE |
| V2-FUNC-006 | WF-V2-01 | — | V2-REQ-011 | `patienttest`, `history` | ET-DOC, ET-DB |
| V2-FUNC-007 | WF-V2-01 | — | V2-REQ-012 | `patienttest`, profile tables | ET-DOC, ET-DB |
| V2-FUNC-008 | WF-V2-01 | V2-BR-034, 035, 036, 037 | V2-REQ-013, 014, 015, 016 | `patienttest`, `normalrange`, `constanttable`, `comments` | ET-DOC+DB |
| V2-FUNC-009 | WF-V2-01 | V2-BR-038 | V2-REQ-017 | `patienttest` | ET-DOC+DB |
| V2-FUNC-010 | WF-V2-01 | V2-BR-038, 042 | V2-REQ-018 | `patienttest`, `payment`, `patientinfo` | ET-DOC, ET-DB |
| V2-FUNC-011 | — | V2-BR-043 | V2-REQ-020 | `testtable` | ET-DOC+DB |
| V2-FUNC-012 | — | V2-BR-036, 043 | V2-REQ-021, 022 | `normalrange` | ET-DOC+DB |
| V2-FUNC-013 | — | V2-BR-020, 021 | V2-REQ-023, 069, 070 | `Data\*.ini`, `REPORT.dll` | ET-DOC, ET-CFG |
| V2-FUNC-014 | — | V2-BR-044 | V2-REQ-024 | `patienttest`, `testtable` | ET-DOC+DB |
| V2-FUNC-015 | — | V2-BR-010, 042 | V2-REQ-025, 026, 027 | `payment`, `Gehats`, `patientinfo` | ET-DOC+DB |
| V2-FUNC-016 | — | V2-BR-026 | V2-REQ-028 | `Patients.mdb`, `Patients_backup.mdb` | ET-DOC, ET-DB |
| V2-FUNC-017 | — | — | V2-REQ-029 | `testunits`, `converts` | ET-DB |
| V2-FUNC-018 | — | — | V2-REQ-030 | `abbreviations` | ET-DB |
| V2-FUNC-019 | — | — | V2-REQ-031 | `subgrouptable` | ET-DB |
| V2-FUNC-020 | — | V2-BR-039 | V2-REQ-032, 033 | `patients`, `useraccounts`, `loggroup` | ET-DOC, ET-DB |
| V2-FUNC-021 | — | V2-BR-020, 021 | V2-REQ-034, 035, 036 | `SETTING.ini`, `Data\*.ini` | ET-DOC, ET-CFG |
| V2-FUNC-022 | — | V2-BR-042 | V2-REQ-037 | `Gehats`, `patientinfo` | ET-DOC+DB |
| V2-FUNC-023 | — | — | V2-REQ-038 | `pictures` | ET-DB |
| V2-FUNC-024 | — | — | V2-REQ-039 | `Cache\*.rtf`, `testtable` | ET-PRES |
| V2-FUNC-025 | — | — | V2-REQ-040 | `Cache\*.exe` | ET-PRES |
| V2-FUNC-026 | — | V2-BR-045 | V2-REQ-041 | `patienttest` | ET-DOC, ET-DB |
| V2-FUNC-027 | — | — | V2-REQ-042 | `patientinfo`, `barcodes` | ET-CFG |
| V2-FUNC-028 | WF-V2-03 | **V2-BR-001, 002, 003, 004, 031** | V2-REQ-043, 044, 045, 046 | `patienttest`; `D:\PDF\` | ET-CODE, ET-INST |
| V2-FUNC-029 | WF-V2-07 | **V2-BR-005, 006, 022** | V2-REQ-047, 048 | `Natigh`, `IDCARD.INI` | ET-DOC, ET-DDL |
| V2-FUNC-030 | WF-V2-07 | **V2-BR-006, 007** | V2-REQ-049, 050 | `SMSSender`, `SMStemplate`, `smslist`, `smsgroups` | ET-DDL, ET-CODE |
| V2-FUNC-031 | WF-V2-02 | **V2-BR-008, 009, 010, 025** | V2-REQ-051, 052, 053 | `Labbranch`, `Branch_Data.mdb` | ET-DOC, ET-DB, ET-CODE |
| V2-FUNC-032 | WF-V2-05 | V2-BR-011 | V2-REQ-054, 055 | `patientinfo`, `patienttest`, `loger` | ET-DOC, ET-CODE |
| V2-FUNC-033 | WF-V2-05 | **V2-BR-012, 041** | V2-REQ-005, 056 | `loger`, `patients` | ET-DOC, ET-DB |
| V2-FUNC-034 | WF-V2-06 | V2-BR-013 | V2-REQ-057 | `wasl`, `patientinfo` | ET-DOC |
| V2-FUNC-035 | WF-V2-06 | V2-BR-013 | V2-REQ-058 | `wasl`, `testtable` | ET-DOC |
| V2-FUNC-036 | — | V2-BR-014 | V2-REQ-059 | `sharekat`, `sharekathesab` | ET-DOC |
| V2-FUNC-037 | — | V2-BR-015 | V2-REQ-060 | `antibiotic`, `antibio` | ET-DOC+DB |
| V2-FUNC-038 | — | V2-BR-016 | V2-REQ-061 | `cultures`, `cultureanti`, `sample` | ET-DOC |
| V2-FUNC-039 | — | **V2-BR-017** | V2-REQ-062 | `anyreport`, profile tables | ET-DOC, ET-CODE |
| V2-FUNC-040 | — | — | V2-REQ-063 | `anyreport` | ET-DOC+DB |
| V2-FUNC-041 | — | V2-BR-018 | V2-REQ-064, 065 | `pricelist`, `pricemenu`, `shopping` | ET-DOC |
| V2-FUNC-042 | — | V2-BR-019 | V2-REQ-066 | `patientinfo`, `Gehats` | ET-DOC+DB |
| V2-FUNC-043 | WF-V2-07 | — | V2-REQ-067 | `IDCARD.INI`, `SETTING.ini` | ET-DOC, ET-CFG |
| V2-FUNC-044 | — | — | V2-REQ-068 | `Data\*.ini` | ET-DOC, ET-CFG |
| V2-FUNC-045 | — | V2-BR-020 | V2-REQ-069 | `Data\*.ini` | ET-DOC, ET-CFG |
| V2-FUNC-046 | — | V2-BR-021 | V2-REQ-070 | `Data\*.ini`, `SETTING.ini` | ET-DOC, ET-CFG |
| V2-FUNC-047 | WF-V2-01 | — | V2-REQ-071 | `patienttest`, `history` | ET-DOC |
| V2-FUNC-048 | WF-V2-07 | V2-BR-022 | V2-REQ-072 | `smslist`, `smsgroups` | ET-DOC |

---

**End of Functionality_Requirements_Matrix_V2.md**
