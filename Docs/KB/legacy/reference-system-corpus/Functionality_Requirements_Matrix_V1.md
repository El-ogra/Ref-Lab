# Functionality and Requirements Matrix — RealLab V-ROOT-1.00

**Document ID:** RL-V1-FRM-001
**Version documented:** `V-ROOT-1.00`
**Evidence basis:** 364 verified artefacts · static analysis only
**Companion documents:** `Functional_Specification_V1.md`, `Business_Rules_V1.md`, `Data_Model_and_Workflow_Entities_V1.md`

---

## 1. Method, conventions and reading guide

### 1.1 Identifier scheme

| Prefix | Scope | Notes |
|---|---|---|
| `V1-FUNC-nnn` | Function | One per documented capability of this version. |
| `V1-REQ-nnn` | Requirement | One per behaviour that can be stated as a verifiable system obligation. |
| `V1-BR-nnn` | Business rule | Defined in `Business_Rules_V1.md`. |
| `V1-ENT-nnn` | Data entity | Defined in `Data_Model_and_Workflow_Entities_V1.md`. |
| `F-V1-nnn` | Function (functional specification) | Defined in `Functional_Specification_V1.md`. |

### 1.2 Requirement wording policy

> **The phrase "The system shall…" is used only where the requirement is directly
> supported by observed or otherwise established system behaviour.** Requirements that
> are inferred are written as **stated behaviour** with an explicit confidence marker,
> and are never phrased as an obligation.

### 1.3 Behaviour classification

| Class | Meaning |
|---|---|
| **CONFIRMED FUNCTIONAL** | Directly established by shipped artefacts. |
| **TECHNICALLY SUPPORTED** | Corroborated by code, schema or configuration but not observed in operation. |
| **INFERRED** | A reasonable reading of available evidence. |
| **UNKNOWN** | Evidence insufficient. |
| **CONFLICTING** | Evidence contradicts; both readings are given. |

### 1.4 Structure of this document

- **§2** — master inventory: one row per function, with the identifying fields.
- **§3** — requirement cards: one card per requirement, with the complete field set.
- **§4** — behaviour classification summary.
- **§5** — version applicability and the V1/V2 delta pointer.
- **§6** — traceability index: function → workflow → rule → requirement → entity → evidence.

---

## 2. Master function inventory — V-ROOT-1.00

| Function ID | Req IDs | Domain | Function name | Arabic label | Actor | Trigger | Preconditions | Behaviour class | Confidence |
|---|---|---|---|---|---|---|---|---|---|
| **V1-FUNC-001** | V1-REQ-001, 002, 003 | Startup | Start the system and connect to the laboratory database | «كيف يتم تشغيل نظام ريال لاب سيستم ؟» | System administrator | Application launch | Server machine running; English regional date format | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-002** | V1-REQ-004, 005, 006 | Security | Sign in / sign out | «كيف يتم تسجيل الدخول لنظام ريال لاب سيستم ؟» | Any operator | Credential entry | User account exists | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-003** | V1-REQ-007, 008 | Patient | Register a new patient | «كيف يتم اضافة بيانات مريض جديد ؟» | Lab operator / reception | Add-patient action | Signed in | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-004** | V1-REQ-009, 010 | Patient | Generate and print the patient barcode | «كيف يتم طباعة باركود للمريض؟» | Lab operator | Barcode request | Patient record exists | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-005** | V1-REQ-011 | Patient | Search for a patient | «كيف تتم عملية البحث عن مريض فى نظام ريال لاب سيستم ؟» | Any operator | Search term entry | Signed in | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-006** | V1-REQ-012 | Patient | Review patient visit and medical history | «التاريخ المرضي للمريض» | Lab operator | History view opened | Patient selected | TECHNICALLY SUPPORTED | STRONGLY SUPPORTED |
| **V1-FUNC-007** | V1-REQ-013 | Results | Open the profile result-entry window | «كيف يتم الوصول لنوافذ ادخال نتائج التحاليل ؟» | Lab operator | Test selected twice | Test requested on `patienttest` | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-008** | V1-REQ-014, 015, 016, 017, 018 | Results | Enter a result with automatic classification | «كيف يتم ادخال نتائج التحاليل (تحاليل الكبد وما شابه مثلاً) ؟» | Lab operator | Value entry | Profile window open | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-009** | V1-REQ-019 | Results | Confirm a result as reviewed | — (F8 = «اعتماد مُراجَع») | Lab operator | F8 | Result saved | TECHNICALLY SUPPORTED | STRONGLY SUPPORTED |
| **V1-FUNC-010** | V1-REQ-020, 021 | Finance | Hand over results and settle the account | «كيف يتم تسليم نتائج التحاليل للمريض ؟» | Reception clerk | Account window opened | Reviewed results outstanding | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-011** | V1-REQ-022, 023 | Catalogue | Add or edit a test definition | «كيف يتم اضافة أو تعديل بيانات تحليل ؟» | Laboratory administrator | Test editor opened | Administrator rights | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-012** | V1-REQ-024, 025, 026 | Clinical knowledge | Administer normal reference ranges | «كيف يتم اضافة أوmodifier المعدلات الطبيعية لتحليل معين ؟» | Laboratory administrator | Normal-rate editor opened | Test exists | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-013** | V1-REQ-027, 028 | Reporting | Customise report text, dimensions and colours | «كيف يتم التعديل فى نصوص وأبعاد وألوان التقرير ؟» | Laboratory administrator | Report setup opened | Profile `.ini` present | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-014** | V1-REQ-029, 030, 031 | Send-out | Manage samples referred to an external laboratory | «كيف يتم ادارة العينات المرسلة للخارج من داخل النظام ؟» | Accounting user | F7 / accounting menu | Test flagged `inotherlab` | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-015** | V1-REQ-032, 033, 034 | Finance | Run accounting and periodic stock-take | «كيف يتم معرفة حساب الدرج وعمل جرد لفترة محددة ؟» | Accounting user | Period selected | Signed in | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-016** | V1-REQ-035 | Administration | Back up the database | «كيف يتم عمل نسخ احتياطي لقاعدة بيانات النظام ؟» | Administrator | Backup invoked | No exclusive lock | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-017** | V1-REQ-036 | Clinical knowledge | Manage units and unit conversions | — | Laboratory administrator | Unit editor opened | Administrator rights | TECHNICALLY SUPPORTED | STRONGLY SUPPORTED |
| **V1-FUNC-018** | V1-REQ-037 | Clinical knowledge | Manage the abbreviation dictionary | — | Laboratory administrator | Dictionary opened | Administrator rights | TECHNICALLY SUPPORTED | STRONGLY SUPPORTED |
| **V1-FUNC-019** | V1-REQ-038 | Catalogue | Manage clinical sub-groups | — | Laboratory administrator | Sub-group editor opened | Administrator rights | TECHNICALLY SUPPORTED | STRONGLY SUPPORTED |
| **V1-FUNC-020** | V1-REQ-039 | Security | Manage users, administrators and login groups | — | Laboratory administrator | User administration opened | Administrator rights | TECHNICALLY SUPPORTED | STRONGLY SUPPORTED |
| **V1-FUNC-021** | V1-REQ-040, 041 | Reporting | Configure printers, paper and report headers | — | Laboratory administrator | Configuration edited | Printer exists | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-022** | V1-REQ-042 | Patient / Finance | Manage referring parties and physicians | — | Laboratory administrator | Party editor opened | Administrator rights | TECHNICALLY SUPPORTED | STRONGLY SUPPORTED |
| **V1-FUNC-023** | V1-REQ-043 | Patient | Attach and view patient pictures | — | Lab operator | Picture frame opened | Patient selected | INFERRED | INFERRED |
| **V1-FUNC-024** | V1-REQ-044 | Clinical knowledge | Consult the method sheet and critical values | — | Lab operator | Sheet consulted | Signed in | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-025** | V1-REQ-045 | Training | Use the interactive training simulators | — | Trainee | Simulator launched | Flash Player 8 available | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-026** | V1-REQ-046 | Reporting | Print a report to a bound printer | — | Any operator | F11 / F12 | Result reviewed; printer bound | CONFIRMED FUNCTIONAL | CONFIRMED |
| **V1-FUNC-027** | V1-REQ-047 | Finance | Print the patient invoice / card | — | Reception clerk | Invoice requested | Patient account exists | TECHNICALLY SUPPORTED | STRONGLY SUPPORTED |

---

## 3. Requirement cards

> Each card carries the complete field set required by the specification. Fields that
> the evidence does not support read **NOT ESTABLISHED** rather than being filled in.

### V1-REQ-001 — Application start-up

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-001 |
| **Function ID** | V1-FUNC-001 |
| **Domain** | Startup and deployment |
| **Description** | On launch the application shows a start-up window and then the login screen. |
| **Arabic label** | «الصفحة الرئيسية» then «شاشة التسجيل» |
| **Actor** | System administrator |
| **Trigger** | Double-click on the program icon or the desktop shortcut. |
| **Preconditions** | Workstation started; server machine running. |
| **Workflow** | Icon double-click → «الصفحة الرئيسية» while loading → «شاشة التسجيل». |
| **Inputs** | None at this step. |
| **Outputs** | A loaded application awaiting credentials. |
| **Resulting state** | Application started; no session yet. |
| **Related business rules** | V1-BR-045 |
| **Related entities** | — |
| **Relevant fields** | — |
| **Permissions** | None required to reach the login screen. |
| **Exceptions** | «خادم قواعد البيانات غير معرف» → application closes on **موافق**. |
| **Dependencies** | `BASE.ini` (`loca`, `DataSource`, `Databasetype`); the SQL Server host. |
| **Related screens** | `start` (form, 8 controls); `login` (form, 7 controls) |
| **Related modules** | — |
| **Technical mechanisms** | Jet OLE DB or `SQLOLEDB.1` connection at start-up. |
| **Evidence** | CHM `001.htm` (verbatim); `analysis/BATCH-01/vb6/main/forms-blob.txt` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The connection sequence and timeout behaviour are not observable statically. |

### V1-REQ-002 — Server identification is configurable; the remaining server settings are not

> **The system shall** present a server-settings window («نافذة اعدادات وبيانات السيرفر») containing the server IP address or machine name, the logon-permission type, the database user name and password, and the database name; **the system shall** permit the operator to change only the server address, and **shall** treat the logon-permission type, the database identity and the database name as fixed.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-002 |
| **Function ID** | V1-FUNC-001 |
| **Domain** | Deployment |
| **Description** | Server identification is operator-configurable; the database connection identity and permission type are not. |
| **Arabic label** | «نافذة اعدادات وبيانات السيرفر» |
| **Actor** | System administrator |
| **Trigger** | Opening the server-settings window. |
| **Preconditions** | Signed in. |
| **Workflow** | Open window → set the address → the other three fields remain as installed. |
| **Inputs** | Server IP address or machine name. |
| **Outputs** | Updated `BASE.ini` `DataSource` (persistence of the edit is INFERRED). |
| **Resulting state** | The workstation points at the configured server. |
| **Related business rules** | V1-BR-001, V1-BR-002 |
| **Related entities** | `BASE.ini` |
| **Relevant fields** | `DataSource`; fixed: `UserID`, `Password`, `InitialCatalog` |
| **Permissions** | Signed-in session. |
| **Exceptions** | Unreachable server → «خادم قواعد البيانات غير معرف». |
| **Dependencies** | Network reachability of the server. |
| **Related screens** | `server` (form, 27 controls in V2 / 0 in V1) |
| **Related modules** | — |
| **Technical mechanisms** | OLE DB connection string construction. |
| **Evidence** | CHM `001.htm`, verbatim four-item list and the closing sentence «كل ما سيتم تغيره هو اسم أو أى بي الخاص بالسيرفر» |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The manual describes the *operator's* window; whether the installer writes these values or the application does is NOT ESTABLISHED for Version 1 (no installer ships with this version). |

### V1-REQ-003 — Refusal to start under a non-English date locale

> **The system shall** display «رجاء تحويل التاريخ الى اللغة الانجليزية» and close when the Windows regional date format is not English, and the documentation shall prescribe changing `Control Panel → Regional and Language Options → Regional Options → Standards and formats` to English.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-003 |
| **Function ID** | V1-FUNC-001 |
| **Domain** | Deployment |
| **Description** | A locale guard prevents start-up. |
| **Arabic label** | «رجاء تحويل التاريخ الى اللغة الانجليزية» |
| **Actor** | System administrator |
| **Trigger** | Application start. |
| **Preconditions** | Non-English date format. |
| **Workflow** | Start → error message → **موافق** → application closes. |
| **Inputs** | Operating-system regional setting. |
| **Outputs** | Error message. |
| **Resulting state** | Application not running. |
| **Related business rules** | V1-BR-044 |
| **Related entities** | — |
| **Relevant fields** | — |
| **Permissions** | None. |
| **Exceptions** | — |
| **Dependencies** | Windows regional settings. |
| **Related screens** | Message box. |
| **Related modules** | — |
| **Technical mechanisms** | Locale interrogation of the operating system. |
| **Evidence** | CHM `001.htm`, verbatim message and remedy path |
| **Confidence** | CONFIRMED for the message and remedy; INFERRED for the check mechanism |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | Which date fields are affected is NOT ESTABLISHED. |

### V1-REQ-004 — Sign-in gate

> **The system shall** require a user name and password at a login screen before presenting the operational interface.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-004 |
| **Function ID** | V1-FUNC-002 |
| **Domain** | Security |
| **Description** | All operational functions are behind a credential gate. |
| **Arabic label** | «شاشة التسجيل» |
| **Actor** | Any operator |
| **Trigger** | Credential entry. |
| **Preconditions** | Application loaded. |
| **Workflow** | Enter user name → enter password → confirm → main patient window. |
| **Inputs** | User name; password. |
| **Outputs** | Authenticated session. |
| **Resulting state** | Session bound to a login group and a branch. |
| **Related business rules** | V1-BR-003, V1-BR-004, V1-BR-032 |
| **Related entities** | `useraccounts`, `loggroup` |
| **Relevant fields** | — |
| **Permissions** | This requirement *is* the access-control gate. |
| **Exceptions** | Failure behaviour NOT ESTABLISHED. |
| **Dependencies** | `useraccounts` populated. |
| **Related screens** | `login`, `users`, `userspw`, `logindete` |
| **Related modules** | — |
| **Technical mechanisms** | Credential comparison against `useraccounts`. |
| **Evidence** | CHM `002.htm`; form inventory; `useraccounts` schema |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | Password hashing, expiry, lockout, and failed-attempt auditing are all NOT ESTABLISHED. |

### V1-REQ-005 — Documented first-run administrative access

> **The system shall** permit the documented first run using the `admin` account with the password set equal to the user name.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-005 |
| **Function ID** | V1-FUNC-002 |
| **Domain** | Security |
| **Description** | The shipped manual documents the initial administrative sign-in. |
| **Arabic label** | «لأول مرة اختر admin … أدخل نفس الكلمة ككلمة مرور» |
| **Actor** | Installing administrator |
| **Trigger** | First sign-in after installation. |
| **Preconditions** | Fresh installation. |
| **Workflow** | Select `admin` → enter `admin` as the password → sign in. |
| **Inputs** | — |
| **Outputs** | Administrative session. |
| **Resulting state** | The `admin` account is usable. |
| **Related business rules** | V1-BR-003 |
| **Related entities** | `useraccounts` |
| **Relevant fields** | — |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `login`, `userspw` |
| **Related modules** | — |
| **Technical mechanisms** | Literal `admin` at executable offset `0x7BA378`. |
| **Evidence** | CHM `002.htm`, verbatim; executable literal |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | Whether a mandatory password change is enforced is NOT ESTABLISHED. **This requirement is a documented security weakness in the shipped product.** |

### V1-REQ-006 — Shared, static database credential

> **The system shall** connect to the laboratory database using the single credential pair held in its configuration files, and that credential shall be identical on every client workstation.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-006 |
| **Function ID** | V1-FUNC-001, V1-FUNC-002 |
| **Domain** | Security / Deployment |
| **Description** | Every workstation authenticates to the database as the same administrative identity. |
| **Arabic label** | «اسم وقـمة مرور المستخدم ويمنع تغيرها وهى خاصة ببيانات الاتصال بخادم قواعد البيانات» |
| **Actor** | The application |
| **Trigger** | Every database connection. |
| **Preconditions** | — |
| **Workflow** | Read `UserID` / `Password` from the configuration file → connect. |
| **Inputs** | Configuration file content. |
| **Outputs** | Database connection with administrative rights. |
| **Resulting state** | Connected. |
| **Related business rules** | V1-BR-002, V1-BR-042, V1-BR-043 |
| **Related entities** | `BASE.ini`, `data_connection.udl`, `data_connection1.udl` |
| **Relevant fields** | `UserID`, `Password`, `PersistSecurityInfo` |
| **Permissions** | The application holds full database rights. |
| **Exceptions** | — |
| **Dependencies** | The configuration file must be readable. |
| **Related screens** | `server` |
| **Related modules** | — |
| **Technical mechanisms** | OLE DB connection string. |
| **Evidence** | `BASE.ini`; `data_connection*.udl` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The Access/Jet deployment path does not use this identity. |

### V1-REQ-007 — Patient master record creation

> **The system shall** create a patient master record capturing basic identifying data, age, sex, referring party, barcode and request date.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-007 |
| **Function ID** | V1-FUNC-003 |
| **Domain** | Patient |
| **Description** | Registration of the patient master record. |
| **Arabic label** | «كيف يتم اضافة بيانات مريض جديد ؟» |
| **Actor** | Lab operator / reception clerk |
| **Trigger** | Add-patient action. |
| **Preconditions** | Signed in. |
| **Workflow** | Open `addpatient` (125 controls) → enter basic data → age and sex → referring party → barcode → request date → save. |
| **Inputs** | Demographics, age, sex, referral, barcode, request date. |
| **Outputs** | `patientinfo` row. |
| **Resulting state** | Patient is selectable for ordering and result entry. |
| **Related business rules** | V1-BR-005, V1-BR-006 |
| **Related entities** | `patientinfo` (33 columns), `barcodes`, `Gehats` |
| **Relevant fields** | `patientid`, `patientname`, `patientsex`, `patientage`, `referredbyname`, `requestdate`, barcode |
| **Permissions** | Signed-in session. |
| **Exceptions** | NOT ESTABLISHED. |
| **Dependencies** | `IDCARD.INI`; `SETTING.ini [setup] barcodeauto`. |
| **Related screens** | `addpatient`, `patient`, `patientmain` |
| **Related modules** | — |
| **Technical mechanisms** | INSERT into `patientinfo`. |
| **Evidence** | CHM `003.htm` (6,549 chars, 3 images); `patientinfo` schema |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | Mandatory-field set, validation rules and duplicate detection are NOT ESTABLISHED. |

### V1-REQ-008 — Barcode allocation

> **The system shall** allocate a barcode to a patient record and permit the operator to print it on the configured barcode printer.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-008 |
| **Function ID** | V1-FUNC-003, V1-FUNC-004 |
| **Domain** | Patient |
| **Description** | Barcode allocation and printing. |
| **Arabic label** | «كيف يتم طباعة باركود للمريض؟» |
| **Actor** | Lab operator |
| **Trigger** | Barcode request. |
| **Preconditions** | Patient record exists. |
| **Workflow** | Open `barcode` (71 controls) → allocate from `barcodes` → print via the barcode printer → link to the patient. |
| **Inputs** | Patient record; `barcodeauto` setting. |
| **Outputs** | `barcodes` row; printed label. |
| **Resulting state** | Patient carries a barcode. |
| **Related business rules** | V1-BR-006, V1-BR-034 |
| **Related entities** | `barcodes` (35 rows), `patientinfo` |
| **Relevant fields** | barcode field |
| **Permissions** | Signed-in session. |
| **Exceptions** | «Can't find any printer.» |
| **Dependencies** | `SETTING.ini [selectprinter] barcodeprinter`, `barcodeport=LPT1:`. |
| **Related screens** | `barcode`, `barcodeform`, `z_barcodes` |
| **Related modules** | `printerselect` |
| **Technical mechanisms** | Win32 spooler dispatch. |
| **Evidence** | CHM `003A.htm`; `barcodes` table; `SETTING.ini` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | Symbology and generation algorithm NOT ESTABLISHED. |

### V1-REQ-009 — Barcode as physical patient identifier

> **The system shall** print a barcode label that directly identifies the patient, and that label shall accompany the specimen.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-009 |
| **Function ID** | V1-FUNC-004 |
| **Domain** | Patient / Security |
| **Description** | The physical label carries a direct patient identifier. |
| **Arabic label** | «كيف يتم طباعة باركود للمريض؟» |
| **Actor** | Lab operator |
| **Trigger** | Label printing. |
| **Preconditions** | — |
| **Workflow** | Print → label applied to specimen. |
| **Inputs** | — |
| **Outputs** | A label bearing a patient identifier. |
| **Resulting state** | — |
| **Related business rules** | V1-BR-006 |
| **Related entities** | `barcodes`, `patientinfo` |
| **Relevant fields** | barcode |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `barcode` |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | CHM `003A.htm`; `IDCARD.INI` barcode field layout |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **Security consequence:** the label is an uncontrolled physical artefact bearing a direct identifier. |

### V1-REQ-010 — Barcode auto-creation toggle

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-010 |
| **Function ID** | V1-FUNC-003, V1-FUNC-021 |
| **Domain** | Configuration |
| **Description** | Automatic barcode creation is a configuration toggle. |
| **Actor** | Laboratory administrator |
| **Trigger** | Configuration change. |
| **Preconditions** | — |
| **Workflow** | Edit `SETTING.ini [setup] barcodeauto`. |
| **Inputs** | `barcodeauto=1` / `0` |
| **Outputs** | Updated `SETTING.ini` |
| **Resulting state** | Barcodes are auto-created, or must be supplied manually. |
| **Related business rules** | V1-BR-034 |
| **Related entities** | `SETTING.ini` |
| **Relevant fields** | `barcodeauto` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `addpatient`, `barcode` |
| **Related modules** | — |
| **Technical mechanisms** | Configuration flag read at registration. |
| **Evidence** | `SETTING.ini [setup] barcodeauto=1` |
| **Confidence** | CONFIRMED for the setting; INFERRED for its effect |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | — |

### V1-REQ-011 — Patient search with a derived short identifier

> **The system shall** locate a patient record by name, patient number or date, and shall derive a shortened identifier from the stored `patientid` by position.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-011 |
| **Function ID** | V1-FUNC-005 |
| **Domain** | Patient |
| **Description** | Search and the derived short identifier. |
| **Arabic label** | «كيف تتم عملية البحث عن مريض فى نظام ريال لاب سيستم ؟» |
| **Actor** | Any operator |
| **Trigger** | Search term entry. |
| **Preconditions** | Signed in. |
| **Workflow** | Enter a term → SQL using `mid(patientid,…)` → matching record displayed. |
| **Inputs** | Name / number / date fragment. |
| **Outputs** | Selected patient. |
| **Resulting state** | Current-patient context. |
| **Related business rules** | V1-BR-005 |
| **Related entities** | `patientinfo` |
| **Relevant fields** | `patientid` |
| **Permissions** | Signed-in session. |
| **Exceptions** | NOT ESTABLISHED. |
| **Dependencies** | — |
| **Related screens** | `patientmain`, `patientview` |
| **Related modules** | — |
| **Technical mechanisms** | `mid(patientid,…)` substring extraction in SQL. |
| **Evidence** | CHM `006.htm`; extracted SQL |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The segment semantics of `patientid` are NOT ESTABLISHED. |

### V1-REQ-012 — Patient history display

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-012 |
| **Function ID** | V1-FUNC-006 |
| **Domain** | Patient |
| **Description** | The system displays the patient's visits with their dates and tests, and the persistent medical-history attributes. |
| **Arabic label** | «التاريخ المرضي للمريض» |
| **Actor** | Lab operator |
| **Trigger** | History view opened. |
| **Preconditions** | Patient selected. |
| **Workflow** | Open `patienthistory` / `patienthistory2` → read `patienttest` and `history` → display. |
| **Inputs** | Patient identifier. |
| **Outputs** | Visit list; history attributes. |
| **Resulting state** | Read-only. |
| **Related business rules** | V1-BR-007 |
| **Related entities** | `patienttest`, `history` |
| **Relevant fields** | `requestdate`, `testcode`; 20 free-form history columns |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | `SETTING.ini [setup] historytype=0` |
| **Related screens** | `patienthistory`, `patienthistory2` |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Form inventory; `history` schema |
| **Confidence** | STRONGLY SUPPORTED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | The meaning of each history column is NOT ESTABLISHED. |

### V1-REQ-013 — Profile-scoped entry window

> **The system shall** open a discipline-specific result-entry window for the selected test, presenting that clinical profile's analyte set.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-013 |
| **Function ID** | V1-FUNC-007 |
| **Domain** | Results |
| **Description** | Entry is per-profile, not a generic grid. |
| **Arabic label** | «كيف يتم الوصول لنوافذ ادخال نتائج التحاليل ؟» |
| **Actor** | Lab operator |
| **Trigger** | The test is selected a second time. |
| **Preconditions** | Test requested on `patienttest`. |
| **Workflow** | Select patient → select test twice → profile window opens with analyte rows. |
| **Inputs** | Patient; test. |
| **Outputs** | An active entry session. |
| **Resulting state** | Operator positioned on the first analyte row. |
| **Related business rules** | V1-BR-008, V1-BR-031 |
| **Related entities** | `patienttest`, per-profile result table, `subgrouptable` |
| **Relevant fields** | `testcode`, `subgroupcode` |
| **Permissions** | Signed-in session. |
| **Exceptions** | NOT ESTABLISHED. |
| **Dependencies** | `Data\*.ini` for the profile. |
| **Related screens** | `resulthepatitis` (185 controls), `resultcbc` (150), `resultselectro` (159), `resultTrace` (203), `resultanytest` (32), `resultimmun` (136), `resultbloodgas` (64) |
| **Related modules** | — |
| **Technical mechanisms** | Profile-table read by sub-group. |
| **Evidence** | CHM `004.htm` (4,670 chars, 3 images); form inventory; 46 profile tables |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The form-to-profile mapping is not established for all 46 profiles. |

### V1-REQ-014 — Automatic abnormal classification

> **The system shall** classify an entered result value by comparing it against the low and high limits stored for the analyte in the reference-range table, and shall mark the result abnormal when it falls outside those limits.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-014 |
| **Function ID** | V1-FUNC-008 |
| **Domain** | Results / Clinical safety |
| **Description** | Range-driven classification. |
| **Arabic label** | «يميز النتائج الغير طبيعية» |
| **Actor** | Lab operator |
| **Trigger** | Value entry. |
| **Preconditions** | A matching `normalrange` row exists. |
| **Workflow** | Enter value → compare against `lowlimit` / `highlimit` → highlight as abnormal if outside. |
| **Inputs** | Measured value. |
| **Outputs** | Abnormal highlighting. |
| **Resulting state** | The result carries an abnormal flag. |
| **Related business rules** | V1-BR-009, V1-BR-011 |
| **Related entities** | `normalrange`, `patienttest` |
| **Relevant fields** | `lowlimit`, `highlimit` |
| **Permissions** | Signed-in session. |
| **Exceptions** | Behaviour with no matching range row NOT ESTABLISHED. |
| **Dependencies** | `normalrange` completeness. |
| **Related screens** | `result*` |
| **Related modules** | — |
| **Technical mechanisms** | Numeric comparison at entry time. |
| **Evidence** | CHM `004A.htm` verbatim; `normalrange` schema (852 rows) |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The range-selection algorithm (which of several matching bands wins) is NOT ESTABLISHED. |

### V1-REQ-015 — Critical classification

> **The system shall** distinguish a critical result from an abnormal result and present the critical state in preference to the abnormal state.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-015 |
| **Function ID** | V1-FUNC-008 |
| **Domain** | Results / Clinical safety |
| **Description** | Three-valued classification with critical at the top. |
| **Arabic label** | «…والحرجة… (عالٍ أو منخفضة أو حرجة)» |
| **Actor** | Lab operator |
| **Trigger** | Value entry beyond the critical limit. |
| **Preconditions** | Critical limit populated for the range row. |
| **Workflow** | Value breaches the critical limit → critical flag set. |
| **Inputs** | Measured value. |
| **Outputs** | Critical flag and critical comment. |
| **Resulting state** | Result is critical. |
| **Related business rules** | V1-BR-010 |
| **Related entities** | `normalrange`, `patienttest` |
| **Relevant fields** | Critical range, Critical comment, Critical flag |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | `normalrange` critical fields. |
| **Related screens** | `result*` |
| **Related modules** | — |
| **Technical mechanisms** | Comparison against the critical limit. |
| **Evidence** | CHM `004A.htm`; `normalrange` critical columns |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **No escalation workflow (notification, repeat, callback) is evidenced** — the critical state is visual only. |

### V1-REQ-016 — Exception text retrieval

> **The system shall** display, alongside an abnormal result, the stored abnormality text for that analyte and direction.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-016 |
| **Function ID** | V1-FUNC-008 |
| **Domain** | Results |
| **Description** | The exception wording accompanies the flag. |
| **Arabic label** | «عالٍ أو منخفضة» |
| **Actor** | Lab operator |
| **Trigger** | Abnormal classification. |
| **Preconditions** | Exception text populated. |
| **Workflow** | Classify → display the stored exception text. |
| **Inputs** | — |
| **Outputs** | Exception text. |
| **Resulting state** | — |
| **Related business rules** | V1-BR-012, V1-BR-018 |
| **Related entities** | `patienttest` (`nolowexe`, `nohighexe`), `normalrange` (Low/High comment) |
| **Relevant fields** | `nolowexe`, `nohighexe` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `result*` |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | `patienttest` schema; `normalrange` schema (19 distinct low / 19 distinct high comments) |
| **Confidence** | STRONGLY SUPPORTED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | Whether the text is read from the range row or copied to the test row at entry is NOT ESTABLISHED. |

### V1-REQ-017 — Comment sourcing

> **The system shall** allow a result comment to be entered as free text, selected from the stored comment list, or taken from the reference-range list.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-017 |
| **Function ID** | V1-FUNC-008 |
| **Domain** | Results |
| **Description** | Three comment sources. |
| **Arabic label** | «زر تعليق (البرتقالي)» |
| **Actor** | Lab operator |
| **Trigger** | Comment entry. |
| **Preconditions** | — |
| **Workflow** | Type · pick from `comments` · pick from the range list. |
| **Inputs** | Comment text. |
| **Outputs** | Stored comment. |
| **Resulting state** | Result carries a comment. |
| **Related business rules** | V1-BR-013 |
| **Related entities** | `comments`, `normalrange` |
| **Relevant fields** | Low/High comment, Critical comment |
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

### V1-REQ-018 — Clinical derivation constants

> **The system shall** hold the haematology and coagulation derivation factors as configurable data, and shall derive `Hct` as `Hgb × 3.3`, the haemoglobin percentage as `Hgb` multiplied by the age- and sex-selected factor, and the ISI from the prothrombin control time.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-018 |
| **Function ID** | V1-FUNC-008 |
| **Domain** | Clinical |
| **Description** | Derivation factors live in `constanttable`. |
| **Arabic label** | «Hct = Hgb × 3.3» and the Hgb% factors, as stated in the manual's notes topic |
| **Actor** | Lab operator |
| **Trigger** | Result derivation. |
| **Preconditions** | Source result exists. |
| **Workflow** | Read the constant → select the factor by age and sex → multiply. |
| **Inputs** | `Hgb`; patient age and sex; control time. |
| **Outputs** | Derived `Hct`, `Hgb%`, ISI. |
| **Resulting state** | Derived values available for the report. |
| **Related business rules** | V1-BR-037, V1-BR-038, V1-BR-039, V1-BR-040 |
| **Related entities** | `constanttable` (`CBCCONS`, `PTCONS`), `control` |
| **Relevant fields** | `constant1`–`constant5` |
| **Permissions** | — |
| **Exceptions** | Age-band boundary at 12 years is ambiguous in the source wording. |
| **Dependencies** | `constanttable` correctness. |
| **Related screens** | `resultcbc` |
| **Related modules** | — |
| **Technical mechanisms** | Stored factors; derived value |
| **Evidence** | CHM `nots.htm`; `constanttable` — **exact numeric concordance** (3.3 / 6.25 / 6.75 / 7.5 / 8.25 / 12 / 35 / 1.33 / 2CVJW) |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The exact ISI formula and the QC rule evaluation algorithm are NOT ESTABLISHED. |

### V1-REQ-019 — Result review gate

> **The system shall** record a review state on a saved result, set by the operator with the F8 key, and only reviewed results shall be eligible for hand-over.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-019 |
| **Function ID** | V1-FUNC-009, V1-FUNC-010 |
| **Domain** | Results / Quality |
| **Description** | The release gate between measurement and hand-over. |
| **Arabic label** | «F8=اعتماد مُراجَع» |
| **Actor** | Lab operator; reception clerk |
| **Trigger** | F8 on a saved result. |
| **Preconditions** | Result saved. |
| **Workflow** | F8 → review state written → result becomes hand-over eligible. |
| **Inputs** | A saved result. |
| **Outputs** | Reviewed result. |
| **Resulting state** | Result released. |
| **Related business rules** | V1-BR-014 |
| **Related entities** | `patienttest` |
| **Relevant fields** | The review-state field — **not isolated in the Version 1 schema** |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `result*` |
| **Related modules** | — |
| **Technical mechanisms** | F8 handler. |
| **Evidence** | CHM `004A.htm` |
| **Confidence** | STRONGLY SUPPORTED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | The storage location of the review state is unknown in this version. |

### V1-REQ-020 — Result hand-over and payment

> **The system shall** present the patient's outstanding results at the account window, record the payment, print the report and mark the results as given to the patient.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-020 |
| **Function ID** | V1-FUNC-010 |
| **Domain** | Finance / Results |
| **Description** | Hand-over closes the patient's episode. |
| **Arabic label** | «كيف يتم تسليم نتائج التحاليل للمريض ؟» |
| **Actor** | Reception clerk |
| **Trigger** | Account window opened. |
| **Preconditions** | Signed in; reviewed results outstanding. |
| **Workflow** | Open `frm_money` → review outstanding → collect → print → mark given (`patienttest.give`). |
| **Inputs** | Patient; payment amount. |
| **Outputs** | `payment` row; invoice; printed report; `give` flag. |
| **Resulting state** | Results released; account settled. |
| **Related business rules** | V1-BR-014, V1-BR-015 |
| **Related entities** | `patienttest`, `payment`, `patientinfo` |
| **Relevant fields** | `give`, `resultprint`, `totalprice`, `totalpricedisc` |
| **Permissions** | Signed-in session. |
| **Exceptions** | «Can't find any printer.» |
| **Dependencies** | `IDCARD.INI`; `Data\*.ini` |
| **Related screens** | `frm_money` (22 controls), `moneypaid` (18 controls) |
| **Related modules** | `printerselect` |
| **Technical mechanisms** | — |
| **Evidence** | CHM `005.htm` (4,178 chars, 2 images); `payment` schema; `patienttest.give` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The pricing and discount computation is NOT ESTABLISHED. |

### V1-REQ-021 — Patient account totals

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-021 |
| **Function ID** | V1-FUNC-010, V1-FUNC-015 |
| **Domain** | Finance |
| **Description** | The account carries a gross total and a discounted total. |
| **Actor** | Reception clerk; accounting user |
| **Trigger** | Account view. |
| **Preconditions** | — |
| **Workflow** | Read `totalprice` and `totalpricedisc` → present → settle. |
| **Inputs** | — |
| **Outputs** | Account total. |
| **Resulting state** | — |
| **Related business rules** | V1-BR-015 |
| **Related entities** | `patientinfo`, `payment` |
| **Relevant fields** | `totalprice`, `totalpricedisc` |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `frm_money`, `moneypaid` |
| **Related modules** | `labfunction` |
| **Technical mechanisms** | — |
| **Evidence** | `patientinfo` schema; CHM `005.htm` |
| **Confidence** | STRONGLY SUPPORTED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | Which total is authoritative is NOT ESTABLISHED. |

### V1-REQ-022 — Test definition maintenance

> **The system shall** allow a laboratory administrator to add or modify a test definition, capturing its name, abbreviation, unit, clinical sub-group, price and send-out flag.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-022 |
| **Function ID** | V1-FUNC-011 |
| **Domain** | Catalogue |
| **Description** | Test-catalogue maintenance. |
| **Arabic label** | «كيف يتم اضافة أو تعديل بيانات تحليل ؟» |
| **Actor** | Laboratory administrator |
| **Trigger** | Test editor opened. |
| **Preconditions** | Administrator rights. |
| **Workflow** | Open `setup1`/`setup2`/`setup3` → set the attributes → save to `testtable` (or `customtesttable`). |
| **Inputs** | Name, abbreviation, unit, sub-group, price, send-out flag. |
| **Outputs** | `testtable` row. |
| **Resulting state** | The test is orderable and printable. |
| **Related business rules** | V1-BR-016, V1-BR-017 |
| **Related entities** | `testtable` (609 rows), `customtesttable`, `testunits`, `subgrouptable` |
| **Relevant fields** | `testcode`, `testname`, `testabrev`, `testunit`, `subgroupcode`, `testprice`, `inotherlab` |
| **Permissions** | Administrator rights (exact flag NOT ESTABLISHED). |
| **Exceptions** | — |
| **Dependencies** | `Data\*.ini` layout for the profile. |
| **Related screens** | `setup`, `setup1`, `setup2`, `setup3`, `tools`, `abbrevi`, `converter`, `instrument`, `commentsetup` |
| **Related modules** | — |
| **Technical mechanisms** | INSERT / UPDATE on `testtable`. |
| **Evidence** | CHM `007.htm` (4,460 chars, 2 images); `testtable` schema and 609 rows |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | Whether a new test also requires an explicit `.ini` layout edit is INFERRED. |

### V1-REQ-023 — Immutability of the urine analysis test

> **The system shall** not permit the number or the name of the urine analysis (test number 4) to be changed.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-023 |
| **Function ID** | V1-FUNC-011, V1-FUNC-012 |
| **Domain** | Catalogue |
| **Description** | A documented standing restriction. |
| **Arabic label** | «ملاحظات يجب مراعتها عند استخدام نظام ريال لاب سيستم» — «تحليل البول يحمل الرقم 4 في النظام ولا يجوز تغيير رقمه ولا اسمه» |
| **Actor** | Laboratory administrator |
| **Trigger** | Any attempt to edit the urine analysis. |
| **Preconditions** | — |
| **Workflow** | Edit attempt → the restriction applies. |
| **Inputs** | — |
| **Outputs** | — |
| **Resulting state** | The test keeps number 4 and its name. |
| **Related business rules** | V1-BR-016, V1-BR-019 |
| **Related entities** | `testtable`, `normalrange` |
| **Relevant fields** | `testcode`, `testname` |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `setup*`, `testrange` |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | CHM `nots.htm`, verbatim |
| **Confidence** | CONFIRMED as a documented rule; UNKNOWN whether it is enforced in code |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | Code enforcement NOT ESTABLISHED. |

### V1-REQ-024 — Reference-range maintenance

> **The system shall** allow an administrator to add, modify, delete and undo reference-range rows scoped by sex, age window and age unit.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-024 |
| **Function ID** | V1-FUNC-012 |
| **Domain** | Clinical knowledge |
| **Description** | Range administration. |
| **Arabic label** | «كيف يتم اضافة أو تعديل المعدلات الطبيعية لتحليل معين ؟» |
| **Actor** | Laboratory administrator |
| **Trigger** | Normal-rate editor opened. |
| **Preconditions** | Test exists. |
| **Workflow** | Setup → Test data → [test] → «المعدل الطبيعي» → add / edit / save / undo / delete. |
| **Inputs** | The 14 range attributes. |
| **Outputs** | `normalrange` rows. |
| **Resulting state** | Subsequent classification for that analyte uses the new limits. |
| **Related business rules** | V1-BR-011, V1-BR-018 |
| **Related entities** | `normalrange` (852 rows, 18 columns) |
| **Relevant fields** | `Sex`, `Age From`, `Age To`, `Age unit`, `Normal range`, `Low/High limit`, `Low/High flag`, `Low/High comment`, `Critical comment`, `Critical range`, critical `Low/High limit`, `Critical flag` |
| **Permissions** | Administrator rights. |
| **Exceptions** | Overlap validation NOT ESTABLISHED. |
| **Dependencies** | `testunits` |
| **Related screens** | `testrange` (17), `outtest` (11), `normal_test` (35) |
| **Related modules** | — |
| **Technical mechanisms** | INSERT / UPDATE / DELETE on `normalrange`. |
| **Evidence** | CHM `008.htm` (3,042 chars, 2 images) — the documented 14 attributes map one-to-one onto the extracted schema |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The `pregsmok` dimension's role is NOT ESTABLISHED. |

### V1-REQ-025 — Range-scoped classification

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-025 |
| **Function ID** | V1-FUNC-008, V1-FUNC-012 |
| **Domain** | Clinical knowledge |
| **Description** | The applicable range is selected by analyte, patient sex and age band. |
| **Actor** | — (system behaviour) |
| **Trigger** | Result entry. |
| **Preconditions** | Range rows exist. |
| **Workflow** | Match analyte → match sex → match age window → apply limits. |
| **Inputs** | Analyte, patient sex, patient age. |
| **Outputs** | The applicable range row. |
| **Resulting state** | — |
| **Related business rules** | V1-BR-011 |
| **Related entities** | `normalrange` |
| **Relevant fields** | `Sex`, `Age From`, `Age To`, `Age unit` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `result*` |
| **Related modules** | — |
| **Technical mechanisms** | Multi-criteria lookup. |
| **Evidence** | CHM `008.htm`; `normalrange` distribution (Male 422 / Female 421 / All 5 / empty 4) |
| **Confidence** | CONFIRMED for the banding; UNKNOWN for the selection algorithm |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | **The selection algorithm among overlapping bands is not established — this is material to reproducing classification exactly.** |

### V1-REQ-026 — Range-scoped exception text

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-026 |
| **Function ID** | V1-FUNC-008, V1-FUNC-012 |
| **Domain** | Clinical knowledge |
| **Description** | Exception text and critical comment are held per range row, so different bands of the same analyte can carry different wording. |
| **Actor** | Laboratory administrator |
| **Trigger** | Range maintenance |
| **Preconditions** | — |
| **Workflow** | Store the comment on the range row. |
| **Inputs** | Comment text per band. |
| **Outputs** | Per-band wording. |
| **Resulting state** | — |
| **Related business rules** | V1-BR-018 |
| **Related entities** | `normalrange` |
| **Relevant fields** | Low/High comment, Critical comment |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `testrange`, `normal_test` |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | CHM `008.htm`; `normalrange` comment columns (19 / 19 distinct values) |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | — |

### V1-REQ-027 — Report layout customisation

> **The system shall** allow an administrator to modify the report headings, the seven column positions and widths, the colour assignments and the per-position font for each clinical profile, and shall persist those changes to the profile's configuration file.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-027 |
| **Function ID** | V1-FUNC-013 |
| **Domain** | Reporting |
| **Description** | Per-profile report layout editing. |
| **Arabic label** | «كيف يتم التعديل فى نصوص وأبعاد وألوان التقرير ؟» |
| **Actor** | Laboratory administrator |
| **Trigger** | Report setup opened. |
| **Preconditions** | Administrator rights; profile `.ini` present. |
| **Workflow** | Open `reportsetup` → edit headings, positions, widths, colours, fonts → edit static blocks 1–5 → save to `Data\*.ini`. |
| **Inputs** | Headings, coordinates, colours, font names. |
| **Outputs** | Updated `Data\*.ini`. |
| **Resulting state** | The profile prints with the new layout. |
| **Related business rules** | V1-BR-020, V1-BR-021 |
| **Related entities** | `Data\*.ini` (223 files / 114 profiles), `REPORT.dll` |
| **Relevant fields** | `[reportdata]` heading/position/width/colour/font keys; `color1`..`color19` |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | `SETTING.ini [setup] papersize` |
| **Related screens** | `reportsetup` (108 controls), `reportsetup2` (69), `static1`–`static5` |
| **Related modules** | — |
| **Technical mechanisms** | 45 `DataReport` design objects; VB6 `Printer` object. |
| **Evidence** | CHM `009.htm` (4,001 chars, 3 images); `Data\*.ini` structure; `REPORT.dll` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The mapping between the 45 DataReport designs and the 223 layouts is NOT ESTABLISHED. The precedence between the two colour sources is NOT ESTABLISHED. |

### V1-REQ-028 — Report head/footer colour palette

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-028 |
| **Function ID** | V1-FUNC-013 |
| **Domain** | Reporting |
| **Description** | The report head/footer colour scheme is held centrally in a 19-entry palette. |
| **Arabic label** | «تعديل ألوان رأس وذيل التقرير» |
| **Actor** | Laboratory administrator |
| **Trigger** | Colour edit. |
| **Preconditions** | — |
| **Workflow** | Edit `REPORT.dll` `color1`..`color19` → the change applies to the head/footer regions. |
| **Inputs** | BGR colour integers. |
| **Outputs** | Updated palette. |
| **Resulting state** | Head/footer colours change. |
| **Related business rules** | V1-BR-020 |
| **Related entities** | `REPORT.dll`, `Data\*.ini` |
| **Relevant fields** | `color1`..`color19` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `reportsetup2` |
| **Related modules** | — |
| **Technical mechanisms** | Text-file palette read by VB6 code. |
| **Evidence** | `bin\REPORT.dll` — 562 bytes, plain text, despite the `.dll` extension |
| **Confidence** | CONFIRMED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | **Precedence against the 24-entry per-profile palette is NOT ESTABLISHED.** |

### V1-REQ-029 — Send-out enrolment

> **The system shall** automatically include every test flagged as sent-out in the send-out working list.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-029 |
| **Function ID** | V1-FUNC-014 |
| **Domain** | Send-out |
| **Description** | Flag-driven enrolment. |
| **Arabic label** | «كيف يتم ادارة العينات المرسلة للخارج من داخل النظام ؟» |
| **Actor** | Accounting user |
| **Trigger** | Opening the send-out screen. |
| **Preconditions** | — |
| **Workflow** | Query `testtable.inotherlab = True` → list the matching `patienttest` rows. |
| **Inputs** | — |
| **Outputs** | The send-out working list. |
| **Resulting state** | — |
| **Related business rules** | V1-BR-017 |
| **Related entities** | `testtable`, `patienttest` |
| **Relevant fields** | `inotherlab`, `sentout`, `send`, `otherlabname` |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `sarfz` (14), `moenysharekat` (0), `moenymozafeen` (27) |
| **Related modules** | — |
| **Technical mechanisms** | Flag-driven query. |
| **Evidence** | CHM `010.htm`; `testtable.inotherlab`; `patienttest` columns |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | — |

### V1-REQ-030 — Send-out grouping and settlement

> **The system shall** group send-out items by destination laboratory, total them, record the settled amount, and support marking an item settled («خالص») and setting a standing destination («دائماً إليه»).

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-030 |
| **Function ID** | V1-FUNC-014 |
| **Domain** | Send-out / Finance |
| **Description** | Grouping, settlement and the two Arabic actions. |
| **Arabic label** | «خالص» · «دائماً إليه» |
| **Actor** | Accounting user |
| **Trigger** | Send-out screen. |
| **Preconditions** | Send-out list populated. |
| **Workflow** | Group by `otherlabname` → total → enter the settled amount → «خالص» → optionally «دائماً إليه» → follow up unsent and unsettled. |
| **Inputs** | Destination; settled amount. |
| **Outputs** | Updated send-out states; totals. |
| **Resulting state** | Item settled; standing default set. |
| **Related business rules** | V1-BR-023, V1-BR-024 |
| **Related entities** | `patienttest`, `otherlabname` |
| **Relevant fields** | `sentout`, `send`, `otherlabname` |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `sarfz`, `moenysharekat`, `moenymozafeen` |
| **Related modules** | `labfunction` |
| **Technical mechanisms** | — |
| **Evidence** | CHM `010.htm` (2,196 chars, 2 images), verbatim «خالص» and «دائماً إليه» |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The storage field for the standing-default flag is NOT ESTABLISHED. |

### V1-REQ-031 — Two-state send-out settlement

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-031 |
| **Function ID** | V1-FUNC-014 |
| **Domain** | Send-out |
| **Description** | Dispatch and settlement are tracked independently. |
| **Actor** | Accounting user |
| **Trigger** | Send-out workflow |
| **Preconditions** | — |
| **Workflow** | `send` marks dispatch; the settled state is recorded separately. |
| **Inputs** | — |
| **Outputs** | Two independent indicators. |
| **Resulting state** | Unsent and unsettled items remain listable. |
| **Related business rules** | V1-BR-024 |
| **Related entities** | `patienttest` |
| **Relevant fields** | `sentout`, `send` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `sarfz` |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | CHM `010.htm`; `patienttest` schema |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | — |

### V1-REQ-032 — Period stock-take with an explicit collection scope

> **The system shall** present, for a selected period, patient accounts, send-out accounts, expenses and remittances, patients with residual balances, referring-party commissions, per-user workload and per-user collections, and shall ask the operator to decide whether the counted money is restricted to that period's patients.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-032 |
| **Function ID** | V1-FUNC-015 |
| **Domain** | Finance |
| **Description** | The six sub-reports of the stock-take. |
| **Arabic label** | «كيف يتم معرفة حساب الدرج وعمل جرد لفترة محددة ؟» · «هل الفلوس المحصلة من مرضى الفترة فقط؟» |
| **Actor** | Accounting user |
| **Trigger** | Period selected. |
| **Preconditions** | Signed in. |
| **Workflow** | Choose the period → answer the scope question → review the six listings. |
| **Inputs** | Period; scope decision. |
| **Outputs** | Six accounting listings. |
| **Resulting state** | Read-only. |
| **Related business rules** | V1-BR-025, V1-BR-026, V1-BR-027 |
| **Related entities** | `payment`, `patientinfo`, `patienttest`, `Gehats`, `otherlabname` |
| **Relevant fields** | `totalprice`, `totalpricedisc`, `omola`, `printuser` |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | `SETTING.ini [setup] labbranchx` |
| **Related screens** | Accounting and stock-take window; cash movement window |
| **Related modules** | `labfunction` (13 methods) |
| **Technical mechanisms** | Aggregation queries. |
| **Evidence** | CHM `011.htm` (1,682 chars, 2 images), verbatim scope question |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The arithmetic of each listing is NOT ESTABLISHED. |

### V1-REQ-033 — Referring-party commission

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-033 |
| **Function ID** | V1-FUNC-015, V1-FUNC-022 |
| **Domain** | Finance |
| **Description** | Commission is held on the referring-party record and reported in the stock-take. |
| **Actor** | Accounting user; laboratory administrator |
| **Trigger** | Stock-take / party maintenance |
| **Preconditions** | Party exists. |
| **Workflow** | Read `Gehats.omola` → report. |
| **Inputs** | Commission value. |
| **Outputs** | Commission figure. |
| **Resulting state** | — |
| **Related business rules** | V1-BR-026 |
| **Related entities** | `Gehats` |
| **Relevant fields** | `omola` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `z_gehats` |
| **Related modules** | `labfunction` |
| **Technical mechanisms** | — |
| **Evidence** | `Gehats` schema with `omola`; CHM `011.htm` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | The commission base is NOT ESTABLISHED. |

### V1-REQ-034 — Per-user workload and collections

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-034 |
| **Function ID** | V1-FUNC-015, V1-FUNC-002 |
| **Domain** | Finance / Security |
| **Description** | The stock-take reports each user's workload and the amounts collected with that user. |
| **Actor** | Accounting user |
| **Trigger** | Stock-take |
| **Preconditions** | Users sign in individually. |
| **Workflow** | Aggregate `patienttest` and `payment` by the attributed user. |
| **Inputs** | — |
| **Outputs** | Per-user workload and collection figures. |
| **Resulting state** | — |
| **Related business rules** | V1-BR-027, V1-BR-032 |
| **Related entities** | `patienttest`, `payment`, `useraccounts`, `loggroup` |
| **Relevant fields** | `printuser` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | Accounting window |
| **Related modules** | `labfunction` |
| **Technical mechanisms** | Group-by aggregation. |
| **Evidence** | CHM `011.htm`, verbatim «حجم عمل كل مستخدم»; `printuser` column |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | — |

### V1-REQ-035 — Database backup

> **The system shall** provide an administrative procedure that produces a restorable copy of the laboratory database.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-035 |
| **Function ID** | V1-FUNC-016 |
| **Domain** | Administration |
| **Description** | Backup procedure. |
| **Arabic label** | «كيف يتم عمل نسخ احتياطي لقاعدة بيانات النظام ؟» |
| **Actor** | Administrator |
| **Trigger** | Backup invoked. |
| **Preconditions** | No exclusive write lock on the `.mdb`. |
| **Workflow** | Invoke → the database is copied → a backup file results. |
| **Inputs** | The live database. |
| **Outputs** | `Patients_backup.mdb`. |
| **Resulting state** | A backup copy exists. |
| **Related business rules** | V1-BR-028 |
| **Related entities** | `Patients.mdb`, `Patients_backup.mdb`, `data.mdb` |
| **Relevant fields** | — |
| **Permissions** | Administrator rights. |
| **Exceptions** | NOT ESTABLISHED. |
| **Dependencies** | Free disk space at the destination. |
| **Related screens** | Administrative backup function |
| **Related modules** | `labdb` (20 methods), `labdb1` (19 methods) |
| **Technical mechanisms** | File copy. |
| **Evidence** | CHM `backup.htm` (3,058 chars, 6 images); `labdb`/`labdb1`; shipped `Patients_backup.mdb` |
| **Confidence** | CONFIRMED for the function; INFERRED for the mechanism |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The shipped backup is smaller and older than the shipped primary database (90/1,381/2,786 vs 93/1,527/3,123), indicating the backup is not kept current.** The backup destination path is NOT ESTABLISHED. |

### V1-REQ-036 — Unit and conversion dictionaries

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-036 |
| **Function ID** | V1-FUNC-017 |
| **Domain** | Clinical knowledge |
| **Description** | The unit dictionary and the analyte-specific conversion factors. |
| **Arabic label** | — |
| **Actor** | Laboratory administrator |
| **Trigger** | Unit / conversion editor opened. |
| **Preconditions** | Administrator rights. |
| **Workflow** | Open `z_units` or `converter` → define → save. |
| **Inputs** | Unit; source unit; target unit; factor. |
| **Outputs** | `testunits` rows (69); `converts` rows (167). |
| **Resulting state** | The dictionaries are extended. |
| **Related business rules** | V1-BR-029 |
| **Related entities** | `testunits`, `converts`, `testtable` |
| **Relevant fields** | Conversion factor column |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `z_units` (21), `converter` (14) |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Form inventory; `testunits` 69 rows; `converts` 167 rows |
| **Confidence** | STRONGLY SUPPORTED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | **No manual topic documents this function.** Whether conversion is applied on entry or on display is NOT ESTABLISHED. |

### V1-REQ-037 — Abbreviation dictionary

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-037 |
| **Function ID** | V1-FUNC-018 |
| **Domain** | Clinical knowledge |
| **Description** | The abbreviation dictionary is maintained independently of the test catalogue. |
| **Actor** | Laboratory administrator |
| **Trigger** | Dictionary opened. |
| **Preconditions** | Administrator rights. |
| **Workflow** | Open `abbrevi` → add / edit → save. |
| **Inputs** | Abbreviation; expansion. |
| **Outputs** | `abbreviations` rows (650). |
| **Resulting state** | Abbreviation rendering changes. |
| **Related business rules** | V1-BR-030 |
| **Related entities** | `abbreviations`, `testtable` |
| **Relevant fields** | — |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `abbrevi` (41) |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Form inventory; `abbreviations` 650 rows |
| **Confidence** | STRONGLY SUPPORTED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | The linkage rule between an abbreviation and an analyte is NOT ESTABLISHED. |

### V1-REQ-038 — Clinical sub-group catalogue

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-038 |
| **Function ID** | V1-FUNC-019 |
| **Domain** | Catalogue |
| **Description** | Sub-groups are maintained and act as the routing key for entry windows and report layouts. |
| **Actor** | Laboratory administrator |
| **Trigger** | Sub-group editor opened. |
| **Preconditions** | Administrator rights. |
| **Workflow** | Open `z_group` → define → associate tests → save. |
| **Inputs** | Sub-group name; member tests. |
| **Outputs** | `subgrouptable` rows (46). |
| **Resulting state** | Test-to-profile routing changes. |
| **Related business rules** | V1-BR-031 |
| **Related entities** | `subgrouptable`, `customgrouptable`, `testtable` |
| **Relevant fields** | `subgroupcode` |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | `Data\*.ini` naming convention |
| **Related screens** | `z_group` (21), `z_customgroup` (37) |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Form inventory; `subgrouptable` 46 rows |
| **Confidence** | STRONGLY SUPPORTED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | The mapping to 223 `.ini` files is NOT ESTABLISHED. |

### V1-REQ-039 — User, administrator and login-group management

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-039 |
| **Function ID** | V1-FUNC-020 |
| **Domain** | Security administration |
| **Description** | Accounts, permission flags and login groups are administered. |
| **Actor** | Laboratory administrator |
| **Trigger** | User administration opened. |
| **Preconditions** | Administrator rights. |
| **Workflow** | Open `users` → create / edit → assign group and flags → save; reset a password via `userspw`. |
| **Inputs** | User name; password; group; flags. |
| **Outputs** | `useraccounts` rows; `loggroup` rows. |
| **Resulting state** | The account can sign in. |
| **Related business rules** | V1-BR-003, V1-BR-032 |
| **Related entities** | `useraccounts`, `loggroup` |
| **Relevant fields** | `useraccounts` permission fields (semantics NOT ESTABLISHED) |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | `SETTING.ini [setup] labbranchx` |
| **Related screens** | `users` (29), `userspw` (13), `z_group` (21) |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | Form inventory; `useraccounts` and `loggroup` schemas; CHM `002.htm` |
| **Confidence** | STRONGLY SUPPORTED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | **The permission flag semantics are NOT ESTABLISHED.** Version 1 has no `grant13..grant17` fields — they are a Version 2 addition. |

### V1-REQ-040 — Printer role binding

> **The system shall** bind printers to the report, barcode and third (`zarf`) roles, and shall permit each report profile to bind its own printer.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-040 |
| **Function ID** | V1-FUNC-021, V1-FUNC-013 |
| **Domain** | Reporting / Deployment |
| **Description** | Three role bindings plus a per-profile binding. |
| **Actor** | Laboratory administrator |
| **Trigger** | Configuration change |
| **Preconditions** | The printer exists on the workstation. |
| **Workflow** | Set `reportprinter`/`reportport`, `barcodeprinter`/`barcodeport`, `zarfprinter` in `SETTING.ini`; set `reportprinterx` in each `Data\*.ini`. |
| **Inputs** | Printer names; ports. |
| **Outputs** | Updated configuration. |
| **Resulting state** | Output is routed to the intended devices. |
| **Related business rules** | V1-BR-021 |
| **Related entities** | `SETTING.ini`, `Data\*.ini` |
| **Relevant fields** | `reportprinter`, `reportport`, `barcodeprinter`, `barcodeport`, `zarfprinter`, `reportprinterx` |
| **Permissions** | Administrator rights. |
| **Exceptions** | «Can't find any printer.»; «SetPrinter Failed. Error code: » |
| **Dependencies** | Windows spooler. |
| **Related screens** | Printer selection (via `printerselect`) |
| **Related modules** | `printerselect` (16 methods) |
| **Technical mechanisms** | Win32 spooler API |
| **Evidence** | `SETTING.ini`; `Data\*.ini [setupx]`; the two English error literals |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The precedence between profile binding and role binding is NOT ESTABLISHED. |

### V1-REQ-041 — Global paper size and report header configuration

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-041 |
| **Function ID** | V1-FUNC-021 |
| **Domain** | Reporting |
| **Description** | Paper size, header texts and the twelve header/footer text positions are globally configured. |
| **Arabic label** | «المنظومة البرمجية» · «ريال لاب سيستم» |
| **Actor** | Laboratory administrator |
| **Trigger** | Configuration change |
| **Preconditions** | — |
| **Workflow** | Edit `SETTING.ini [setup] papersize` and the `Textr*txt` / `Text*` keys. |
| **Inputs** | Paper size; header strings; positions and fonts. |
| **Outputs** | Updated `SETTING.ini`. |
| **Resulting state** | Page geometry and headers change for all output. |
| **Related business rules** | V1-BR-022, V1-BR-033 |
| **Related entities** | `SETTING.ini` |
| **Relevant fields** | `papersize`, `reportstart`, `Textr1txt`–`Textr4txt`, `Textl1`–`Textl4`, `Textc1`–`Textc4` |
| **Permissions** | — |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | — |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | `SETTING.ini` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | The twelve text positions' semantics are NOT ESTABLANCED beyond the 12-position count. |

### V1-REQ-042 — Referring-party maintenance

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-042 |
| **Function ID** | V1-FUNC-022 |
| **Domain** | Patient / Finance |
| **Description** | Referring parties are maintained with their commission. |
| **Actor** | Laboratory administrator |
| **Trigger** | Party editor opened. |
| **Preconditions** | Administrator rights. |
| **Workflow** | Add / edit a party and its commission → save. |
| **Inputs** | Party name; commission. |
| **Outputs** | `Gehats` rows. |
| **Resulting state** | The party becomes selectable at registration. |
| **Related business rules** | V1-BR-026 |
| **Related entities** | `Gehats`, `patientinfo` |
| **Relevant fields** | `omola`, `referredbyname` |
| **Permissions** | Administrator rights. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `z_gehats` |
| **Related modules** | — |
| **Technical mechanisms** | — |
| **Evidence** | `Gehats` schema with `omola`; `patientinfo.referredbyname` |
| **Confidence** | STRONGLY SUPPORTED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | The distinction between a referring party and a referring physician is a Version 2 distinction. |

### V1-REQ-043 — Patient picture attachment and display

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-043 |
| **Function ID** | V1-FUNC-023 |
| **Domain** | Patient |
| **Description** | Images are associated with a patient and displayed, with a standalone gallery executable for enlarged viewing. |
| **Actor** | Lab operator |
| **Trigger** | Picture frame opened. |
| **Preconditions** | Patient selected. |
| **Workflow** | Open `Frm_pic` / `Frm_pic2` → read the patient's images → display; optionally launch `real lab Image Gallery.exe`. |
| **Inputs** | Patient identifier. |
| **Outputs** | Displayed images. |
| **Resulting state** | Read-only. |
| **Related business rules** | — |
| **Related entities** | `pictures` |
| **Relevant fields** | — |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | — |
| **Related screens** | `Frm_pic` (49), `Frm_pic2` (38) |
| **Related modules** | — |
| **Technical mechanisms** | `real lab Image Gallery.exe` |
| **Evidence** | Form inventory; `analysis/CUMULATIVE/01-architecture.md` |
| **Confidence** | INFERRED |
| **Behaviour class** | INFERRED |
| **Known limitations** | **The `pictures` table was not located in the extracted `Patients.mdb` schema** — the linkage is inferred from the two image frames and the standalone executable. |

### V1-REQ-044 — Method sheet and critical-value consultation

> **The system shall** make the method sheet for the analyte under entry available to the operator, including the sample type, measurement principle, clinical significance and critical-value thresholds.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-044 |
| **Function ID** | V1-FUNC-024 |
| **Domain** | Clinical knowledge |
| **Description** | The method-sheet layer. |
| **Arabic label** | — (the sheets themselves are Arabic documents) |
| **Actor** | Lab operator |
| **Trigger** | Consulting the sheet during entry. |
| **Preconditions** | Signed in. |
| **Workflow** | The sheet for the analyte is retrieved from `Cache\*.rtf` and displayed beside the entry field. |
| **Inputs** | The test identifier. |
| **Outputs** | Method text; critical thresholds. |
| **Resulting state** | Read-only. |
| **Related business rules** | V1-BR-035, V1-BR-041 |
| **Related entities** | `Cache\*.rtf` (29 sheets), `normalrange` |
| **Relevant fields** | — |
| **Permissions** | Signed-in session. |
| **Exceptions** | — |
| **Dependencies** | `Cache\` directory presence. |
| **Related screens** | `result*` |
| **Related modules** | — |
| **Technical mechanisms** | RTF rendering in the VB6 form. |
| **Evidence** | `analysis/BATCH-02/rtf/` — 29 sheets with parsed thresholds |
| **Confidence** | CONFIRMED for the artefact and its content; INFERRED for the in-window presentation |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **Only 29 sheets exist for 609 tests; the selection rule is NOT ESTABLISHED.** |

### V1-REQ-045 — Interactive training simulators

> **The system shall** ship standalone interactive simulators that rehearse clinical workflows without access to the live laboratory database.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-045 |
| **Function ID** | V1-FUNC-025 |
| **Domain** | Training |
| **Description** | The training library. |
| **Actor** | Trainee |
| **Trigger** | Launching a simulator from `Cache\`. |
| **Preconditions** | Flash Player 8 available. |
| **Workflow** | Run the executable → the Flash presentation runs → the trainee navigates and receives feedback. |
| **Inputs** | Navigational input. |
| **Outputs** | Training feedback. |
| **Resulting state** | Read-only. |
| **Related business rules** | V1-BR-036 |
| **Related entities** | `Cache\*.exe` (13) |
| **Relevant fields** | — |
| **Permissions** | None. |
| **Exceptions** | — |
| **Dependencies** | Flash Player 8. |
| **Related screens** | 13 simulation screens, 379 frames, 640×480 |
| **Related modules** | — |
| **Technical mechanisms** | Flash; Word-binary projector tables |
| **Evidence** | `analysis/BATCH-03/flash/`; `analysis/CUMULATIVE/09-presentation-library.md` |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **The simulators are byte-identical in Version 2** — the training material was not updated in seven years. |

### V1-REQ-046 — Report printing

> **The system shall** render the clinical report using the profile's layout and dispatch it to the bound printer, and shall record the print against the test row together with the printing user, the date and the report sequence number.

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-046 |
| **Function ID** | V1-FUNC-026 |
| **Domain** | Reporting |
| **Description** | Print dispatch and print audit. |
| **Actor** | Any operator |
| **Trigger** | F11 (preview) or F12 (print). |
| **Preconditions** | Result reviewed; a printer bound. |
| **Workflow** | `printerselect` resolves the printer → the `DataReport` renders using `Data\*.ini` → spooler dispatch → print audit written. |
| **Inputs** | — |
| **Outputs** | Printed report; print audit record. |
| **Resulting state** | The report has been issued and is attributed. |
| **Related business rules** | V1-BR-021, V1-BR-033 |
| **Related entities** | `patienttest`, `Data\*.ini` |
| **Relevant fields** | `resultprint`, `printuser`, `printdate`, `printnum` |
| **Permissions** | Signed-in session. |
| **Exceptions** | «Can't find any printer.»; «SetPrinter Failed. Error code: » |
| **Dependencies** | A bound, present printer. |
| **Related screens** | Print preview |
| **Related modules** | `printerselect` |
| **Technical mechanisms** | VB6 `Printer` object; Win32 spooler API |
| **Evidence** | CHM `009.htm`; `patienttest` print columns; the two error literals |
| **Confidence** | CONFIRMED |
| **Behaviour class** | CONFIRMED FUNCTIONAL |
| **Known limitations** | **PDF output is absent — see V1-BR-046 in the Business Rules catalogue.** |

### V1-REQ-047 — Patient invoice and card printing

| Field | Content |
|---|---|
| **Requirement ID** | V1-REQ-047 |
| **Function ID** | V1-FUNC-010, V1-FUNC-027 |
| **Domain** | Finance / Reporting |
| **Description** | The patient card and invoice are laid out and printed from a dedicated configuration file. |
| **Arabic label** | «Welcome To RealLab» · «Thank you for visiting RealLab!» (the invoice's own English strings) |
| **Actor** | Reception clerk |
| **Trigger** | Invoice or card printed. |
| **Preconditions** | Patient record exists. |
| **Workflow** | Read `IDCARD.INI` `[IDdata]` (name, barcode, password positions/fonts) and `[Billdata]` (invoice texts) → render → print. |
| **Inputs** | Patient data. |
| **Outputs** | Printed card or invoice. |
| **Resulting state** | — |
| **Related business rules** | V1-BR-006 |
| **Related entities** | `patientinfo`, `barcodes` |
| **Relevant fields** | `IDCARD.INI` `L`/`T`/`F`/`B`/`S` keys per field |
| **Permissions** | Signed-in session. |
| **Exceptions** | «Can't find any printer.» |
| **Dependencies** | `IDCARD.INI` |
| **Related screens** | `frm_money`, `moneypaid` |
| **Related modules** | `printerselect` |
| **Technical mechanisms** | — |
| **Evidence** | `IDCARD.INI`; CHM `005.htm` |
| **Confidence** | STRONGLY SUPPORTED |
| **Behaviour class** | TECHNICALLY SUPPORTED |
| **Known limitations** | **The vendor's address, telephone, website, e-mail and Facebook page are printed on the invoice by default** — a configuration-content exposure for any laboratory using it unchanged. |

---

## 4. Behaviour classification summary

| Behaviour class | Count | Requirement IDs |
|---|---:|---|
| CONFIRMED FUNCTIONAL | 34 | V1-REQ-001, 002, 003, 004, 005, 006, 007, 008, 009, 011, 013, 014, 015, 016, 017, 018, 020, 022, 023, 024, 026, 027, 029, 030, 031, 032, 034, 035, 040, 041, 044, 045, 046, 047 |
| TECHNICALLY SUPPORTED | 11 | V1-REQ-010, 012, 019, 021, 025, 028, 033, 036, 037, 038, 039 |
| INFERRED | 1 | V1-REQ-043 |
| UNKNOWN | 0 | — |
| CONFLICTING | 0 | — |

### 4.1 Requirements explicitly NOT asserted

The following are **not** written as "The system shall…" requirements because the
evidence does not support them. They are recorded as open questions in
`../Documentation_Quality_Report.md` §5 and as candidate rules in
`Business_Rules_V1.md` §4.

| Candidate requirement | Why it is not asserted |
|---|---|
| The system shall reject overlapping reference ranges | No validation logic recoverable statically |
| The system shall detect duplicate patient registration | No uniqueness rule evidenced |
| The system shall require a reference range before a test can be ordered | No ordering-time check evidenced |
| The system shall convert units automatically on entry | Not determinable from the data alone |
| The system shall require a second signature to release a critical result | No such mechanism evidenced, and absence of evidence is not evidence of absence |
| The system shall encrypt data at rest | No evidence of encryption; not asserted |
| The system shall enforce password expiry or complexity | Not evidenced |
| The system shall audit failed login attempts | Not evidenced |
| The system shall produce a PDF | **Confirmed absent** — V1-BR-046 |

---

## 5. Version applicability

| Function | V-ROOT-1.00 | V-CD-2019 | Basis for the difference |
|---|---|---|---|
| V1-FUNC-001 Start / connect | Present | Present, **extended** — the installer introduces a three-engine selection | Installer evidence (BATCH-05) |
| V1-FUNC-002 Sign in | Present | Present, **extended** — `grant13..grant17` permission flags, `useraccounts.gtype/hdd1/hdd2` | Schema and literal diff |
| V1-FUNC-003 Patient registration | Present | Present, **extended** — branch field, sample tracking (`Patient_samples`) | Schema diff |
| V1-FUNC-004 Barcode | Present | Present | — |
| V1-FUNC-005 Search | Present | Present | — |
| V1-FUNC-006 History | Present | Present | — |
| V1-FUNC-007/008 Result entry | Present | Present, **extended** — new profiles (IgE, H. pylori, PCR, LBC, FMF, urine-auto) | Table diff |
| V1-FUNC-009 Review | Present | Present — **explicit `patienttest.resultreview` field** | Schema |
| V1-FUNC-010 Hand-over | Present | Present, **extended** — NATIGH.COM / SMS delivery | Object diff |
| V1-FUNC-011 Test definition | Present | Present | — |
| V1-FUNC-012 Reference ranges | Present | Present — **identical 852 rows** | Table diff |
| V1-FUNC-013 Report customisation | Present | Present, **extended** — new `Data\*.ini` keys (`colora..colort`, `formsubtitle`, `reportmaintitle`, `backcolor`) | INI diff |
| V1-FUNC-014 Send-out | Present | Present, **extended** — `waslprinter`, `IDprinter` roles | Literal diff |
| V1-FUNC-015 Accounting | Present | Present, **extended** — multi-branch stock-take | Vendor guide 5:2 |
| V1-FUNC-016 Backup | Present | Present — **same stale backup shipped** | Artefact comparison |
| V1-FUNC-017 Units / conversions | Present | Present | — |
| V1-FUNC-018 Abbreviations | Present | Present | — |
| V1-FUNC-019 Sub-groups | Present | Present | — |
| V1-FUNC-020 Users | Present | Present, **extended** | Schema diff |
| V1-FUNC-021 Printers / paper | Present | Present, **extended** — 6 printer roles instead of 3 | Literal diff |
| V1-FUNC-022 Referring parties | Present | Present, **extended** — `mowzaf` (referring physician) separate from `Gehats` | Table diff |
| V1-FUNC-023 Pictures | Present (inferred) | Present | — |
| V1-FUNC-024 Method sheets | Present | Present — **29 sheets byte-identical** | Artefact comparison |
| V1-FUNC-025 Training | Present | Present — **13 simulators byte-identical** | Artefact comparison |
| V1-FUNC-026 Print | Present | Present, **extended** — PDF path added | Literal + installer evidence |
| V1-FUNC-027 Invoice / card | Present | Present, **extended** — `IDCARD.INI` gains NATIGH receipt header/footer | Config diff |

### 5.1 Functions present in V-CD-2019 but **not** in V-ROOT-1.00

| V2 function | Arabic label | Evidence |
|---|---|---|
| NATIGH.COM / SMS result delivery | «تسليم نتائج المرضى عن طريق موقع نتتيجة (NATIGH.COM)» | `Natigh`, `SMSSender`, `SMSfrmsenderos`, `smsgroups`, `smslist`, `SMStemplate`, `ReportSMS` |
| Branch master / reception management | «عمل جرد على فرع ضمن عدة فروع مرتبطة معا (نظام الفروع للمعامل)» | `Labbranch`, `Patient.branche`, `SUBSTRING(patientid,9,1)`, `Branch_Data.mdb` |
| Laboratory statistics and performance monitoring | «الإحصائيات ومتابعة أداء المعمل» | `statisReport`, `loger` |
| Attendance and departure registration | «تسجيل الحضور والانصراف لمعرفة حركة المستخدمين في الدخول والخروج» | `loger`, `loggroup` |
| Work sheet generation | «عمل ورقة عمل لمجموعة من المرضى في فترة زمنية محددة» | `workpaper`, `workpaper2`, `wasl` |
| Farm / microbiology reporting | «إدخال نتائج تقارير المزارع» | `sharekat`, `sharekathesab`, `sarfz` |
| Antibiotic catalogue | «إضافة مضاد حي للمزرعة» | `antibiotic` (38 rows / 25 cols), `antibio` (96 rows), `cultures` |
| Sample splitting and withdrawal | «فصل وسحب العينات» | `cultures`, `cultureanti`, `sample` |
| Patient ID card | — | `CardFrm`, `CardFrmSetx` |
| Patient sample tracking | — | `Patient_samples` |
| PDF export | — | `Bullzip PDF Printer` literal + installer |

---

## 6. Traceability index — V-ROOT-1.00

| Function | Workflow | Related business rules | Requirements | Primary entities | Evidence classes |
|---|---|---|---|---|---|
| V1-FUNC-001 | WF-V1-01 | V1-BR-001, 002, 042, 043, 044, 045 | V1-REQ-001, 002, 003, 006 | — (configuration) | ET-DOC, ET-CFG, ET-CODE |
| V1-FUNC-002 | WF-V1-01 | V1-BR-003, 004, 032, 042, 043 | V1-REQ-004, 005 | `useraccounts`, `loggroup` | ET-DOC+CODE, ET-DB |
| V1-FUNC-003 | WF-V1-01 | V1-BR-005, 006, 034 | V1-REQ-007, 008, 010 | `patientinfo`, `barcodes`, `Gehats` | ET-DOC, ET-DB |
| V1-FUNC-004 | WF-V1-01 | V1-BR-006, 021 | V1-REQ-009, 010 | `barcodes` | ET-DOC, ET-DB, ET-CFG |
| V1-FUNC-005 | WF-V1-01 | V1-BR-005 | V1-REQ-011 | `patientinfo` | ET-DOC, ET-CODE |
| V1-FUNC-006 | WF-V1-01 | V1-BR-007 | V1-REQ-012 | `patienttest`, `history` | ET-DB, ET-CODE |
| V1-FUNC-007 | WF-V1-02 | V1-BR-008, 031 | V1-REQ-013 | `patienttest`, profile tables, `subgrouptable` | ET-DOC, ET-DB |
| V1-FUNC-008 | WF-V1-02 | V1-BR-009, 010, 011, 012, 013, 037, 038, 039, 040, 041 | V1-REQ-014, 015, 016, 017, 018 | `patienttest`, `normalrange`, `constanttable`, `comments` | ET-DOC+DB, ET-PRES |
| V1-FUNC-009 | WF-V1-01 | V1-BR-014 | V1-REQ-019 | `patienttest` | ET-DOC |
| V1-FUNC-010 | WF-V1-01 | V1-BR-014, 015 | V1-REQ-020, 021, 047 | `patienttest`, `payment`, `patientinfo` | ET-DOC, ET-DB |
| V1-FUNC-011 | WF-V1-06 | V1-BR-016, 017 | V1-REQ-022, 023 | `testtable`, `customtesttable` | ET-DOC+DB |
| V1-FUNC-012 | WF-V1-04 | V1-BR-011, 018, 019 | V1-REQ-024, 025, 026 | `normalrange` | ET-DOC+DB |
| V1-FUNC-013 | WF-V1-05 | V1-BR-020, 021, 022 | V1-REQ-027, 028 | `Data\*.ini`, `REPORT.dll` | ET-DOC, ET-CFG |
| V1-FUNC-014 | WF-V1-03 | V1-BR-017, 023, 024 | V1-REQ-029, 030, 031 | `patienttest`, `testtable`, `otherlabname` | ET-DOC+DB |
| V1-FUNC-015 | WF-V1-04 | V1-BR-025, 026, 027 | V1-REQ-032, 033, 034 | `payment`, `Gehats`, `patientinfo` | ET-DOC+DB |
| V1-FUNC-016 | — | V1-BR-028 | V1-REQ-035 | `Patients.mdb`, `Patients_backup.mdb` | ET-DOC, ET-DB |
| V1-FUNC-017 | WF-V1-06 | V1-BR-029 | V1-REQ-036 | `testunits`, `converts` | ET-DB, ET-CODE |
| V1-FUNC-018 | WF-V1-06 | V1-BR-030 | V1-REQ-037 | `abbreviations` | ET-DB, ET-CODE |
| V1-FUNC-019 | WF-V1-06 | V1-BR-031 | V1-REQ-038 | `subgrouptable` | ET-DB, ET-CODE |
| V1-FUNC-020 | — | V1-BR-003, 032 | V1-REQ-039 | `useraccounts`, `loggroup` | ET-DB, ET-CODE |
| V1-FUNC-021 | WF-V1-05 | V1-BR-021, 022, 033, 034 | V1-REQ-040, 041 | `SETTING.ini`, `Data\*.ini` | ET-CFG, ET-CODE |
| V1-FUNC-022 | — | V1-BR-026 | V1-REQ-042 | `Gehats` | ET-DB, ET-CODE |
| V1-FUNC-023 | — | — | V1-REQ-043 | `pictures` (inferred) | ET-CODE |
| V1-FUNC-024 | WF-V1-02 | V1-BR-035, 041 | V1-REQ-044 | `Cache\*.rtf`, `normalrange` | ET-PRES |
| V1-FUNC-025 | — | V1-BR-036 | V1-REQ-045 | `Cache\*.exe` | ET-PRES |
| V1-FUNC-026 | WF-V1-05 | V1-BR-021, 033 | V1-REQ-046 | `patienttest` | ET-DOC, ET-DB, ET-CODE |
| V1-FUNC-027 | WF-V1-01 | V1-BR-006 | V1-REQ-047 | `patientinfo`, `barcodes` | ET-CFG |

---

**End of Functionality_Requirements_Matrix_V1.md**
