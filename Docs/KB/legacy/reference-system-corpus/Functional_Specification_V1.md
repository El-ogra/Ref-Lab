# Functional Specification — RealLab V-ROOT-1.00

**Document ID:** RL-V1-FUN-001
**Version documented:** `V-ROOT-1.00`
**Evidence basis:** 364 verified artefacts · static analysis only, no execution
**Companion documents:** `Complete_Reverse_Engineering_Reference_V1.md` (technical), `Business_Rules_V1.md` (rules), `Data_Model_and_Workflow_Entities_V1.md` (data)

---

## 1. Scope, method and reading conventions

### 1.1 Method

Each function below was reconstructed from at least two of the following evidence
classes. **No function in this document was observed running.**

| Evidence class | Source |
|---|---|
| **E-CHM** | `real lab system help.chm` — the shipped Arabic user manual, 15 topics, 34,777 Arabic characters (`analysis/BATCH-01/help/help-manual-ar.md`) |
| **E-CODE** | Executable analysis — 169 VB6 objects, 6,528 methods, 104,997 string literals (`analysis/BATCH-01/vb6/main/`) |
| **E-DB** | Extracted Access schema and sample data (`analysis/BATCH-02/db/`) |
| **E-CFG** | Shipped configuration files (`analysis/CUMULATIVE/06-configuration.md`) |
| **E-PRES** | Presentation library — 13 Flash simulators, 29 RTF method sheets (`analysis/BATCH-03/`) |

### 1.2 Confidence

| Level | Meaning in this document |
|---|---|
| **CONFIRMED** | Behaviour is stated in the shipped manual and corroborated by code or schema. |
| **STRONGLY SUPPORTED** | Corroborated by two independent evidence classes, at least one of which is code or schema. |
| **INFERRED** | A single evidence class supports it, or the reconstruction is a reasonable reading of available artefacts. |
| **UNKNOWN** | Evidence insufficient. |
| **CONFLICTING** | Evidence contradicts; both readings are given. |

### 1.3 Arabic label convention

Original Arabic labels appear in **double guillemets** and are reproduced exactly as
transcribed. English glosses are in parentheses and are explanatory only.

### 1.4 Identifier scheme

| Prefix | Meaning |
|---|---|
| `F-V1-nnn` | Function |
| `WF-V1-nn` | Workflow |
| `RL-V1-FUN-nnn` | Functional requirement |
| `V1-BR-nnn` | Business rule |
| `V1-ENT-nnn` | Data entity |

---

## 2. Actor model

| Actor | Description | Established by |
|---|---|---|
| **Lab operator / technologist** | The primary user. Registers patients, enters results, confirms results, prints and hands over reports. | CHM topics `003`–`006`; result-entry forms |
| **Reception / cash clerk** | Collects payment and hands over results. | `frm_money` (22 controls), `moneypaid` (18 controls), CHM `005.htm` |
| **Laboratory administrator** | Maintains the test catalogue, reference ranges, prices, users, printers, report layouts; performs backup; runs stock-take. | CHM `007`–`011`, `backup.htm`; `setup*`, `users`, `userspw`, `z_*` forms |
| **System administrator** | Installs and maintains the workstation, the database server and the network; sets the regional date format. | CHM `001.htm`, `backup.htm` |
| **Referring physician / referring party** | External actor; data is recorded against the patient via `Gehats` and `mowzaf`-style referral fields. | `patientinfo.referredbyname`; `Gehats.omola` |
| **Send-out laboratory** | External actor receiving referred samples and returning results. | `otherlabname`; CHM `010.htm` |
| **Audit reader** | Reads `patienttest` print-audit fields and `loggroup`. | Schema |

> **⚠ Permission granularity is NOT ESTABLISHED.** The system has a login gate and
> a `useraccounts` table with permission fields, but the evidence does not show a
> per-function or per-role permission matrix. Every entry in this document that
> names a permission states "actor must be signed in"; finer control is **UNKNOWN**.

---

## 3. Function index

| Function ID | Function | Arabic label | Domain | Confidence |
|---|---|---|---|---|
| F-V1-001 | Start the system and configure the server | «كيف يتم تشغيل نظام ريال لاب سيستم ؟» | Startup | CONFIRMED |
| F-V1-002 | Sign in / sign out | «كيف يتم تسجيل الدخول لنظام ريال لاب سيستم ؟» | Security | CONFIRMED |
| F-V1-003 | Register a new patient | «كيف يتم اضافة بيانات مريض جديد ؟» | Patient | CONFIRMED |
| F-V1-004 | Generate and print the patient barcode | «كيف يتم طباعة باركود للمريض؟» | Patient | CONFIRMED |
| F-V1-005 | Search for a patient | «كيف تتم عملية البحث عن مريض فى نظام ريال لاب سيستم ؟» | Patient | CONFIRMED |
| F-V1-006 | Review a patient's visit and test history | «التاريخ المرضي للمريض» | Patient | STRONGLY SUPPORTED |
| F-V1-007 | Open the result-entry window for a profile | «كيف يتم الوصول لنوافذ ادخال نتائج التحاليل ؟» | Results | CONFIRMED |
| F-V1-008 | Enter a result in a profile window | «كيف يتم ادخال نتائج التحاليل (تحاليل الكبد وما شابه مثلاً) ؟» | Results | CONFIRMED |
| F-V1-009 | Confirm (review) a result | — | Results | STRONGLY SUPPORTED |
| F-V1-010 | Hand over results to the patient | «كيف يتم تسليم نتائج التحاليل للمريض ؟» | Finance | CONFIRMED |
| F-V1-011 | Add / edit a test definition | «كيف يتم اضافة أو تعديل بيانات تحليل ؟» | Catalogue | CONFIRMED |
| F-V1-012 | Administer normal reference ranges | «كيف يتم اضافة أو تعديل المعدلات الطبيعية لتحليل معين ؟» | Catalogue | CONFIRMED |
| F-V1-013 | Customise report text, dimensions and colours | «كيف يتم التعديل فى نصوص وأبعاد وألوان التقرير ؟» | Reporting | CONFIRMED |
| F-V1-014 | Manage send-out samples | «كيف يتم ادارة العينات المرسلة للخارج من داخل النظام ؟» | Send-out | CONFIRMED |
| F-V1-015 | Run accounting and stock-take | «كيف يتم معرفة حساب الدرج وعمل جرد لفترة محددة ؟» | Finance | CONFIRMED |
| F-V1-016 | Back up the database | «كيف يتم عمل نسخ احتياطي لقاعدة بيانات النظام ؟» | Administration | CONFIRMED |
| F-V1-017 | Manage test units and unit conversions | — | Catalogue | STRONGLY SUPPORTED |
| F-V1-018 | Manage abbreviations | — | Catalogue | STRONGLY SUPPORTED |
| F-V1-019 | Manage clinical sub-groups | — | Catalogue | STRONGLY SUPPORTED |
| F-V1-020 | Manage users, admins and login groups | — | Security | STRONGLY SUPPORTED |
| F-V1-021 | Configure printers, paper and report headers | — | Reporting | CONFIRMED |
| F-V1-022 | Manage referring parties and physicians | — | Patient | STRONGLY SUPPORTED |
| F-V1-023 | Manage the picture / image gallery | — | Patient | INFERRED |
| F-V1-024 | Work with method sheets and critical values | — | Results | CONFIRMED |
| F-V1-025 | Work with the interactive training simulators | — | Training | CONFIRMED |

---

## 4. Detailed function specifications

### F-V1-001 — Start the system and configure the server

| Field | Content |
|---|---|
| **Function ID** | F-V1-001 |
| **Name** | Start the system and configure the server connection |
| **Arabic label** | «كيف يتم تشغيل نظام ريال لاب سيستم ؟» · window «نافذة اعدادات وبيانات السيرفر» |
| **Domain** | Startup and deployment |
| **Purpose** | Bring the workstation up, reach the shared laboratory database, and confirm the server identity. |
| **Business context** | Every laboratory workstation connects to one shared database. A failure to reach the server blocks all work. |
| **Actor** | System administrator |
| **Entry point** | Double-click on the program icon inside the `real lab system` folder, or on the desktop shortcut. |
| **Preconditions** | The workstation is started. The database server machine is running. The network path to the server is available. The Windows regional date format is English. |
| **Trigger** | Application launch |
| **Step-by-step workflow** | 1. Double-click the icon. 2. The main page «الصفحة الرئيسية» appears while loading. 3. The login screen appears. 4. If a server-settings window is presented, confirm the server IP / machine name, the logon-permission type, the database user and password, and the database name. |
| **Inputs** | Server IP address or machine name. The manual prefers the machine name: «ويفضل على جهاز السيرفر استخدام اسم الجهاز» |
| **Input validation** | The manual documents no client-side validation of the server address. The four server-settings items are documented as fixed except the address. |
| **System responses** | Main page → login screen. On failure, a message box and termination. |
| **Outputs** | An authenticated session at the login screen. |
| **Resulting state** | Application loaded, database connection established or failed. |
| **Postconditions** | The user can sign in. |
| **Business rules** | V1-BR-001 (server address is the only mutable server setting) · V1-BR-002 (logon-permission type, database identity and database name are immutable) · V1-BR-044 (the application refuses to proceed when the Windows regional date format is not English) · V1-BR-045 (the database server must be running before any client can sign in) |
| **Permissions** | Signed-in session required for the settings window |
| **Entities** | `useraccounts`, `loggroup` |
| **Data read** | `BASE.ini` (`[oledb]`), `SETTING.ini` (`[setup] labbranchx`) |
| **Data created** | Session record in `loggroup` (INFERRED) |
| **Data updated** | None established |
| **Data deleted** | None |
| **Reports / printed output** | None |
| **Configuration dependencies** | `BASE.ini` (`loca`, `DataSource`, `Databasetype`), `SETTING.ini` (`labbranchx`) |
| **Exceptions** | «خادم قواعد البيانات غير معرف» — database server not defined; application closes on **موافق**. «رجاء تحويل التاريخ الى اللغة الانجليزية» — non-English date locale; application closes on **موافق**. |
| **Alternative paths** | On a **client** machine the server machine must be started manually, and the server application need not be opened. On a **server** machine, wait for start-up programs to complete, then press OK twice. |
| **Related functions** | F-V1-002, F-V1-016 |
| **Technical dependencies** | Jet OLE DB provider or `SQLOLEDB.1`; Windows regional settings |
| **Evidence** | E-CHM `001.htm` (verbatim); E-CFG `BASE.ini`, `SETTING.ini`; E-CODE forms `start`, `server` |
| **Confidence** | CONFIRMED |
| **Known limitations** | The actual connection sequence and retry behaviour are not observable without execution. The manual gives operator instructions, not a protocol. |

### F-V1-002 — Sign in / sign out

| Field | Content |
|---|---|
| **Function ID** | F-V1-002 |
| **Name** | Sign in to the laboratory system |
| **Arabic label** | «كيف يتم تسجيل الدخول لنظام ريال لاب سيستم ؟» |
| **Domain** | Security and session |
| **Purpose** | Restrict the workstation to registered users and associate the session with a login group. |
| **Business context** | Each operator has their own account so that print attribution and workload accounting work. |
| **Actor** | Any operator; laboratory administrator for account creation |
| **Entry point** | Login screen shown after application start |
| **Preconditions** | Application loaded; a user account exists |
| **Trigger** | Operator enters user name and password and confirms |
| **Step-by-step workflow** | 1. Enter the user name. 2. Enter the password. 3. Confirm. 4. The main patient window opens. |
| **Inputs** | User name, password |
| **Input validation** | Credential check against `useraccounts`. **Password hashing, expiry, lockout and minimum-length rules are NOT ESTABLISHED.** |
| **System responses** | On success, the main patient window. On failure, the documented behaviour is **UNKNOWN** — the manual documents only the first-run case. |
| **Outputs** | An authenticated session |
| **Resulting state** | `loggroup` associates the session with a group; `labbranchx` scopes the session to a branch |
| **Postconditions** | The operator's identity is available for print attribution (`printuser`) and workload accounting |
| **Business rules** | V1-BR-003 (first run uses the `admin` account with password equal to the user name) · V1-BR-004 (session is scoped to a branch) · V1-BR-032 (the login group is the unit of session scoping and workload attribution) · **V1-BR-047 (each operator carries twelve function-level permission bits `grant1`…`grant12`)** · **V1-BR-048 (operator attendance is recorded on the operator record itself)** · V1-BR-042 (the database password is stored in clear text in shipped configuration) · V1-BR-043 (the application connects to the database as the administrative account `sa`) |
| **Permissions** | This function *is* the permission gate |
| **Entities** | **`patients` (the operator table — `patientusname`, `patientword`, `patientad`, `grant1`…`grant12`)**, `loggroup` |
| **Data read** | `patients` (operator credential and permission bits) |
| **Data created** | `loggroup` session entry — INFERRED from schema and form presence; `patients.startwork` / `endwork` attendance timestamps — INFERRED |
| **Data updated** | `loggroup` last-activity — INFERRED |
| **Data deleted** | None established |
| **Reports / printed output** | None |
| **Configuration dependencies** | `SETTING.ini [setup] labbranchx` |
| **Exceptions** | Unknown; not documented |
| **Alternative paths** | A logout control is documented in the topic's window description |
| **Related functions** | F-V1-020 |
| **Technical dependencies** | `login` form (7 controls), `users` form (29), `userspw` form (13), `logindete` form, `logindetect` data object |
| **Evidence** | E-CHM `002.htm` (verbatim: «لأول مرة اختر admin … أدخل نفس الكلمة ككلمة مرور»); E-CODE form inventory; E-DB `useraccounts`, `loggroup`; literal `admin` @ `0x7BA378` |
| **Confidence** | CONFIRMED for the existence and the first-run procedure; UNKNOWN for failure handling |
| **Known limitations** | No evidence of session timeout, inactivity lock, or audit of failed attempts. |

### F-V1-003 — Register a new patient

| Field | Content |
|---|---|
| **Function ID** | F-V1-003 |
| **Name** | Register a new patient |
| **Arabic label** | «كيف يتم اضافة بيانات مريض جديد ؟» |
| **Domain** | Patient master data |
| **Purpose** | Create the patient master record that every test, result, print and invoice references. |
| **Business context** | A patient may be registered once and visit many times; `patientid` is the key that links every downstream record. |
| **Actor** | Lab operator or reception clerk |
| **Entry point** | Main patient window «الصفحة الرئيسية» (`patientmain`, 54 controls) |
| **Preconditions** | Signed in |
| **Trigger** | Operator selects the add-patient action |
| **Step-by-step workflow** | 1. Open the add-patient window (`addpatient`, 125 controls). 2. Enter the basic identifying data. 3. Enter age and sex. 4. Enter the referring party. 5. Assign or generate the barcode. 6. Enter the request date. 7. Save. |
| **Inputs** | Patient demographic data; age; sex; referring party; barcode; request date |
| **Input validation** | The manual describes the fields but **the specific validation rules, mandatory-field set and duplicate-detection behaviour are NOT ESTABLISHED**. |
| **System responses** | A new `patientinfo` row; the patient becomes selectable for test ordering and result entry |
| **Outputs** | A patient master record; a patient identifier |
| **Resulting state** | `patientinfo` row created |
| **Postconditions** | Barcode printable (F-V1-004); tests orderable |
| **Business rules** | V1-BR-005 (the search mechanism derives a short identifier from `patientid` via `mid(patientid,…)`) — the identifier is composite and position-dependent |
| **Permissions** | Signed-in session |
| **Entities** | `patientinfo` (33 columns), `barcodes`, `Gehats` (referring party) |
| **Data read** | `Gehats` (referring-party list), `barcodes` (next free barcode) |
| **Data created** | `patientinfo` row; `barcodes` allocation row |
| **Data updated** | `barcodes` allocation state |
| **Data deleted** | None |
| **Reports / printed output** | Patient card / invoice layout via `IDCARD.INI` (name, barcode, password fields) |
| **Configuration dependencies** | `IDCARD.INI` `[IDdata]`; `SETTING.ini [setup] barcodeauto=1` |
| **Exceptions** | Not documented |
| **Alternative paths** | Barcode auto-generation is controlled by `barcodeauto`; if disabled the operator supplies the barcode |
| **Related functions** | F-V1-004, F-V1-005, F-V1-006, F-V1-010 |
| **Technical dependencies** | `patientinfo` 33 columns; forms `addpatient`, `patient`, `patientmain` |
| **Evidence** | E-CHM `003.htm` (6,549 chars, 3 images); E-CODE form inventory; E-DB `patientinfo` schema; E-CFG `SETTING.ini` |
| **Confidence** | CONFIRMED for the function and its inputs; INFERRED for the validation rules |
| **Known limitations** | The `patientid` format and the source of the composite segments are not established. |

### F-V1-004 — Generate and print the patient barcode

| Field | Content |
|---|---|
| **Function ID** | F-V1-004 |
| **Name** | Generate and print the patient barcode |
| **Arabic label** | «كيف يتم طباعة باركود للمريض؟» |
| **Domain** | Patient master data |
| **Purpose** | Produce the physical label that binds the specimen tube, request slip and patient to the electronic record. |
| **Business context** | The barcode is the sole physical identifier carried with a sample in the laboratory. |
| **Actor** | Lab operator |
| **Entry point** | Patient record |
| **Preconditions** | A patient record exists |
| **Trigger** | Operator requests barcode printing |
| **Step-by-step workflow** | 1. Open the barcode window (`barcode`, 71 controls; `barcodeform`, 8 controls). 2. The barcode is allocated from the `barcodes` table. 3. Print via the barcode printer bound in `SETTING.ini [selectprinter] barcodeprinter` / `barcodeport`. 4. The barcode is linked to the patient record. |
| **Inputs** | Patient record |
| **Input validation** | Not documented. |
| **System responses** | Barcode allocated and printed; linked to `patientinfo` |
| **Outputs** | A printed barcode label |
| **Resulting state** | `barcodes` row created or marked used; `patientinfo` carries the barcode |
| **Postconditions** | The specimen can be matched to the record at the bench |
| **Business rules** | V1-BR-006 (the barcode is the direct patient identifier on a physical artefact) |
| **Permissions** | Signed-in session |
| **Entities** | `barcodes` (35 rows), `patientinfo` |
| **Data read** | `barcodes` |
| **Data created** | `barcodes` row |
| **Data updated** | `barcodes` allocation state |
| **Data deleted** | None |
| **Reports / printed output** | Barcode label via the barcode printer |
| **Configuration dependencies** | `SETTING.ini [selectprinter] barcodeport=LPT1:`, `barcodeprinter` |
| **Exceptions** | «Can't find any printer.» if no printer is available |
| **Alternative paths** | Automatic barcode creation is toggled by `SETTING.ini [setup] barcodeauto=1` |
| **Related functions** | F-V1-003, F-V1-021 |
| **Technical dependencies** | `barcode` form (71 controls), `z_barcodes` form (14) |
| **Evidence** | E-CHM `003A.htm` (2,444 chars, 1 image); E-CODE forms; E-DB `barcodes`; E-CFG `SETTING.ini` |
| **Confidence** | CONFIRMED |
| **Known limitations** | Barcode symbology and generation algorithm are NOT ESTABLISHED. |

### F-V1-005 — Search for a patient

| Field | Content |
|---|---|
| **Function ID** | F-V1-005 |
| **Name** | Search for a patient |
| **Arabic label** | «كيف تتم عملية البحث عن مريض فى نظام ريال لاب سيستم ؟» |
| **Domain** | Patient master data |
| **Purpose** | Locate an existing patient record to continue work on a visit. |
| **Business context** | Patients return; the operator must reach the right record quickly and without creating a duplicate. |
| **Actor** | Any operator |
| **Entry point** | Main patient window |
| **Preconditions** | Signed in |
| **Trigger** | Operator enters a search term |
| **Step-by-step workflow** | 1. Open the patient search. 2. Search by name, number or date. 3. The system extracts a shortened identifier from `patientid` using `mid(patientid,…)`. 4. The matching patient record is displayed. |
| **Inputs** | Name fragment, patient number, or date |
| **Input validation** | The `mid(patientid,…)` extraction implies positional structure; the exact match/partial-match rules are NOT ESTABLISHED. |
| **System responses** | Matching patient record(s) |
| **Outputs** | A selected patient |
| **Resulting state** | Current-patient context set |
| **Postconditions** | Result entry, printing and hand-over become available |
| **Business rules** | V1-BR-005 (short identifier is positionally derived from `patientid`) |
| **Permissions** | Signed-in session |
| **Entities** | `patientinfo` |
| **Data read** | `patientinfo` |
| **Data created / updated / deleted** | None established |
| **Reports / printed output** | None |
| **Configuration dependencies** | None specific |
| **Exceptions** | Not documented; duplicate-creation risk is not addressed in the manual |
| **Alternative paths** | Navigation by the visit-history path (F-V1-006) is an alternative route to a patient record |
| **Related functions** | F-V1-003, F-V1-006 |
| **Technical dependencies** | `patientmain` (54), `patientview` (29); SQL `mid(patientid,…)` |
| **Evidence** | E-CHM `006.htm` (2,879 chars, 3 images); E-CODE; E-DB |
| **Confidence** | CONFIRMED for the function and the identifier mechanism; INFERRED for exact matching semantics |

### F-V1-006 — Review a patient's visit and test history

| Field | Content |
|---|---|
| **Function ID** | F-V1-006 |
| **Name** | Review a patient's visit and test history |
| **Arabic label** | «التاريخ المرضي للمريض» |
| **Domain** | Patient master data |
| **Purpose** | Display the patient's prior visits with their dates and tests, and the persistent medical-history attributes. |
| **Business context** | Clinicians need longitudinal context when interpreting a new result. |
| **Actor** | Lab operator, reviewing physician |
| **Entry point** | Patient view |
| **Preconditions** | Signed in; a patient selected |
| **Trigger** | Operator opens the history view |
| **Step-by-step workflow** | 1. Open the patient history window (`patienthistory`, 22 controls). 2. The system reads the visit/test history. 3. The persistent attribute history is read from the `history` table. 4. Display. |
| **Inputs** | Patient identifier |
| **Input validation** | Not applicable |
| **System responses** | Visit list with dates and tests; history attributes |
| **Outputs** | Historical context for the current visit |
| **Resulting state** | Read-only |
| **Postconditions** | Operator can proceed to result entry |
| **Business rules** | V1-BR-007 (the `history` table uses 20 free-form columns as generic attribute slots) |
| **Permissions** | Signed-in session |
| **Entities** | `patienttest`, `history`, `patientinfo` |
| **Data read** | `patienttest`, `history` |
| **Data created / updated / deleted** | None established |
| **Reports / printed output** | None established for this function specifically |
| **Configuration dependencies** | `SETTING.ini [setup] historytype=0` — the history display mode |
| **Exceptions** | Not documented |
| **Alternative paths** | `patienthistory2` (17 controls) provides a second, narrower history view |
| **Related functions** | F-V1-005, F-V1-008 |
| **Technical dependencies** | `patienthistory`, `patienthistory2` forms |
| **Evidence** | E-CODE form inventory; E-DB `history` (20 columns) and `patienttest`; E-CFG `SETTING.ini` |
| **Confidence** | STRONGLY SUPPORTED |
| **Known limitations** | The semantics of each `history` column are not established — they are positional attribute slots. |

### F-V1-007 — Open the result-entry window for a clinical profile

| Field | Content |
|---|---|
| **Function ID** | F-V1-007 |
| **Name** | Open the result-entry window for a clinical profile |
| **Arabic label** | «كيف يتم الوصول لنوافذ ادخال نتائج التحاليل ؟» |
| **Domain** | Result entry |
| **Purpose** | Reach the correct per-discipline entry window for the test requested. |
| **Business context** | RealLab has a **separate entry window per clinical profile** (hepatitis, CBC, immunology, blood gases, …) rather than one generic grid. Each window is shaped to its analyte set. |
| **Actor** | Lab operator |
| **Entry point** | Selected patient and requested test |
| **Preconditions** | Signed in; patient selected; a test requested on `patienttest` |
| **Trigger** | Operator selects the test |
| **Step-by-step workflow** | 1. From the main patient window select the requested test. 2. Select the test a second time. 3. The profile entry window opens, pre-populated with the profile's analyte list. |
| **Inputs** | Patient + requested test |
| **Input validation** | Not applicable |
| **System responses** | The profile window opens with analyte rows |
| **Outputs** | An active entry session |
| **Resulting state** | The operator is positioned on one analyte row |
| **Postconditions** | Values can be entered (F-V1-008) |
| **Business rules** | V1-BR-008 (entry is profile-scoped: a result is entered against a profile's analyte set, not against an arbitrary test) |
| **Permissions** | Signed-in session |
| **Entities** | `patienttest`, per-profile result table, `subgrouptable`, `testtable` |
| **Data read** | `patienttest`, profile table, `testtable`, `normalrange` |
| **Data created / updated / deleted** | None at this step |
| **Reports / printed output** | None |
| **Configuration dependencies** | `Data\*.ini` for the profile's print layout |
| **Exceptions** | Not documented |
| **Alternative paths** | `resultanytest` (32 controls) provides a generic path; `resultselectro` (159) and `resultTrace` (203) provide specialised paths |
| **Related functions** | F-V1-008, F-V1-024 |
| **Technical dependencies** | 46 profile result tables; forms `result*` |
| **Evidence** | E-CHM `004.htm` (4,670 chars, 3 images); E-CODE form inventory; E-DB profile tables |
| **Confidence** | CONFIRMED |
| **Known limitations** | The full form-to-profile mapping is not established for every one of the 46 profiles. |

### F-V1-008 — Enter a result in a profile window

| Field | Content |
|---|---|
| **Function ID** | F-V1-008 |
| **Name** | Enter a result value with automatic abnormal / critical classification |
| **Arabic label** | «كيف يتم ادخال نتائج التحاليل (تحاليل الكبد وما شابه مثلاً) ؟» |
| **Domain** | Result entry |
| **Purpose** | Capture the measured value and let the system classify it against the reference range. |
| **Business context** | This is the core clinical act of the system. The classification is the safety mechanism — an unflagged abnormal result is the primary patient-safety risk in a laboratory information system. |
| **Actor** | Lab operator / technologist |
| **Entry point** | Profile result-entry window (F-V1-007) |
| **Preconditions** | Profile window open; the operator is signed in |
| **Trigger** | Operator types a value |
| **Step-by-step workflow** | 1. Operator types the value into the analyte field. 2. The system compares the value against `lowlimit` / `highlimit` from `normalrange` for the patient's sex and age. 3. If outside the range, the cell is highlighted as abnormal and the exception text is taken from `nolowexe` / `nohighexe`. 4. If the value also breaches the critical limit, the result is flagged as critical («حرجة»). 5. The operator adds a comment — typed, selected from stored comments, or taken from the normal-ranges list via «زر تعليق (البرتقالي)». 6. F9 to save, F8 to confirm as reviewed. |
| **Inputs** | Measured value (numeric or text); optional comment |
| **Input validation** | Numeric range check against `normalrange`; the exact type rules for text-valued analytes are NOT ESTABLISHED. |
| **System responses** | Abnormal highlighting; exception text; critical flag; the value is written to the profile table and mirrored onto `patienttest` |
| **Outputs** | Stored result with classification |
| **Resulting state** | The analyte row carries a value, a flag, and a comment |
| **Postconditions** | F11 preview / F12 print are available |
| **Business rules** | V1-BR-009 (result classification is driven by `normalrange` limits, not by hard-coded values) · V1-BR-010 (a critical limit overrides the abnormal classification) · V1-BR-011 (results are range-scoped by sex and age window) · V1-BR-012 (the exception text shown is stored on the test row) · V1-BR-013 (an entry may be commented from three distinct sources) · V1-BR-037 (`Hct = Hgb × 3.3`) · V1-BR-038 (`Hgb% = Hgb ×` age/sex factor) · V1-BR-039 (ISI from PT control) · V1-BR-040 (Westgard `2CVJW`) · V1-BR-041 (per-analyte critical thresholds stated in the method sheets) |
| **Permissions** | Signed-in session |
| **Entities** | `patienttest` (44 columns), `normalrange`, per-profile result table, `comments` |
| **Data read** | `normalrange` (limit, flag, comment), `patienttest`, `comments` |
| **Data created** | Result row in the profile table; result values on `patienttest` |
| **Data updated** | `patienttest` result fields and flags |
| **Data deleted** | None |
| **Reports / printed output** | The profile report, laid out by the profile's `Data\*.ini` |
| **Configuration dependencies** | `Data\*.ini` (profile layout); `Cache\*.rtf` (method sheet shown alongside) |
| **Exceptions** | Not documented for a value that matches no range row |
| **Alternative paths** | Comment sources: free text / stored comment / normal-range list |
| **Related functions** | F-V1-007, F-V1-009, F-V1-013, F-V1-024 |
| **Technical dependencies** | `patienttest` columns `lowlimit`, `highlimit`, `nolowexe`, `nohighexe`; `normalrange` 18 columns |
| **Evidence** | E-CHM `004A.htm` (2,572 chars, 1 image, verbatim «يميز النتائج الغير طبيعية والحرجة ويكتب تمييز لها (عالٍ أو منخفضة أو حرجة)»); E-CODE literals `.Normalbold`, `Check*x`; E-DB `normalrange` + `patienttest` schema |
| **Confidence** | CONFIRMED for the classification model; INFERRED for the exact flag precedence |
| **Known limitations** | The `pregsmok` dimension is populated on all 852 range rows but its use in the comparison is NOT traced — if it participates, the effective comparison is narrower than the documented sex × age model. |

### F-V1-009 — Confirm (review) a result

| Field | Content |
|---|---|
| **Function ID** | F-V1-009 |
| **Name** | Confirm a result as reviewed |
| **Arabic label** | — (documented as the F8 shortcut: «F8=اعتماد مُراجَع») |
| **Domain** | Result entry |
| **Purpose** | Mark a result as reviewed and released for printing and hand-over. |
| **Business context** | Separation of entry from release. Unconfirmed results are the ones a laboratory must not hand over. |
| **Actor** | Lab operator, typically a senior technologist |
| **Entry point** | Profile result-entry window |
| **Preconditions** | The result is saved |
| **Trigger** | F8 |
| **Step-by-step workflow** | 1. The operator presses F8 on the result. 2. The review status is written to the test row. |
| **Inputs** | A saved result |
| **Input validation** | Not documented |
| **System responses** | Review state recorded |
| **Outputs** | Reviewed result |
| **Resulting state** | Result becomes printable and hand-over eligible |
| **Postconditions** | The result can be printed (F-V1-013) and handed over (F-V1-010) |
| **Business rules** | V1-BR-014 (only reviewed results are eligible for hand-over) — the hand-over function enforces this gate |
| **Permissions** | Signed-in session. Whether a second signature is required is **UNKNOWN**. |
| **Entities** | `patienttest` (review/status field) |
| **Data read** | `patienttest` |
| **Data created** | None |
| **Data updated** | `patienttest` review field |
| **Data deleted** | None |
| **Reports / printed output** | None directly |
| **Configuration dependencies** | None specific |
| **Exceptions** | Not documented |
| **Alternative paths** | — |
| **Related functions** | F-V1-008, F-V1-010, F-V1-013 |
| **Technical dependencies** | F8 handler in the result forms |
| **Evidence** | E-CHM `004A.htm`; E-CODE keyboard-literal set |
| **Confidence** | STRONGLY SUPPORTED |
| **Known limitations** | The storage field is identified (`patienttest.resultreview`, Boolean). Whether the review is single-operator or requires a second signatory is **UNKNOWN**. |

### F-V1-010 — Hand over results to the patient

| Field | Content |
|---|---|
| **Function ID** | F-V1-010 |
| **Name** | Hand over results to the patient and settle the account |
| **Arabic label** | «كيف يتم تسليم نتائج التحاليل للمريض ؟» |
| **Domain** | Finance and result release |
| **Purpose** | Review the outstanding results, collect payment, print the report and mark the results as given to the patient. |
| **Business context** | This is the revenue and release point. It closes the patient's episode and produces the invoice. |
| **Actor** | Reception / cash clerk |
| **Entry point** | Patient account window (`frm_money`, 22 controls) |
| **Preconditions** | Signed in; patient selected; at least one reviewed result outstanding |
| **Trigger** | Operator opens the patient's account |
| **Step-by-step workflow** | 1. Open the patient's account. 2. Review the outstanding results. 3. Collect the amount. 4. Print the report. 5. Mark the results as given (`patienttest.give`). |
| **Inputs** | Patient; payment amount |
| **Input validation** | The existence of `patientinfo.totalprice` and `totalpricedisc` implies a computed total and a discount; the exact computation is NOT ESTABLISHED. |
| **System responses** | Payment recorded in `payment`; results flagged as handed over; invoice printable |
| **Outputs** | `payment` row; invoice; printed report |
| **Resulting state** | Result released; account settled |
| **Postconditions** | The patient episode is closed for billing purposes |
| **Business rules** | V1-BR-014 (results must be reviewed before hand-over) · V1-BR-015 (the patient account total is derived from `totalprice` and `totalpricedisc`) |
| **Permissions** | Signed-in session |
| **Entities** | `patienttest` (`.give`, `.resultprint`), `payment`, `patientinfo` (`totalprice`, `totalpricedisc`) |
| **Data read** | `patienttest`, `payment`, `patientinfo` |
| **Data created** | `payment` row |
| **Data updated** | `patienttest.give`; `patientinfo` totals |
| **Data deleted** | None |
| **Reports / printed output** | Patient report; invoice via `IDCARD.INI [Billdata]` («Welcome To RealLab», «Thank you for visiting RealLab!») |
| **Configuration dependencies** | `IDCARD.INI`; `Data\*.ini` |
| **Exceptions** | «Can't find any printer.» |
| **Alternative paths** | — |
| **Related functions** | F-V1-009, F-V1-013, F-V1-015 |
| **Technical dependencies** | Forms `frm_money`, `moneypaid` |
| **Evidence** | E-CHM `005.htm` (4,178 chars, 2 images); E-CODE forms; E-DB `payment` (6 columns), `patientinfo` |
| **Confidence** | CONFIRMED |
| **Known limitations** | The discount and price rules are not established. |

### F-V1-011 — Add or edit a test definition

| Field | Content |
|---|---|
| **Function ID** | F-V1-011 |
| **Name** | Add or edit a test definition |
| **Arabic label** | «كيف يتم اضافة أو تعديل بيانات تحليل ؟» |
| **Domain** | Test catalogue |
| **Purpose** | Define a test — its name, abbreviation, unit, clinical profile, price and send-out flag. |
| **Business context** | The catalogue is the spine of the whole system: adding a test here makes it orderable, printable and rangeable without touching code. |
| **Actor** | Laboratory administrator |
| **Entry point** | `setup`, `setup1`, `setup2`, `setup3`, `tools` forms |
| **Preconditions** | Signed in with administrator rights |
| **Trigger** | Operator chooses add/edit on a test |
| **Step-by-step workflow** | 1. Open the test-definition editor. 2. Enter or select the test. 3. Set the name, abbreviation, unit, profile (sub-group), price and the `inotherlab` send-out flag. 4. Save — `testtable` is updated; `customtesttable` holds custom entries. |
| **Inputs** | Test name, abbreviation, unit, sub-group, price, send-out flag |
| **Input validation** | **The urine analysis is number 4 and neither its number nor its name may be changed** — the documented standing restriction. Other uniqueness rules are NOT ESTABLISHED. |
| **System responses** | The test becomes selectable in ordering and reporting |
| **Outputs** | A `testtable` row |
| **Resulting state** | `testtable` updated; the test appears in the ordering list and in the affected profile |
| **Postconditions** | Reference ranges can then be attached (F-V1-012) |
| **Business rules** | V1-BR-016 (test number 4 — urine analysis — is immutable in both number and name) · V1-BR-017 (a test may be flagged as send-out, which automatically enrols it in the send-out workflow) |
| **Permissions** | Administrator rights — exact permission flag is UNKNOWN |
| **Entities** | `testtable` (37 columns, 609 rows), `customtesttable`, `testunits`, `subgrouptable` |
| **Data read** | `testunits`, `subgrouptable`, `abbreviations` |
| **Data created** | `testtable` row |
| **Data updated** | `testtable` row |
| **Data deleted** | None established |
| **Reports / printed output** | The test appears in profile reports via the profile layout |
| **Configuration dependencies** | `Data\*.ini` — the profile layout must contain the analyte slot for the new test |
| **Exceptions** | Not documented |
| **Alternative paths** | — |
| **Related functions** | F-V1-012, F-V1-017, F-V1-019 |
| **Technical dependencies** | Forms `setup`(8), `setup1`(54), `setup2`(60), `setup3`(22), `tools`(1), `abbrevi`(41), `converter`(14), `instrument`(29), `commentsetup`(5) |
| **Evidence** | E-CHM `007.htm` (4,460 chars, 2 images); E-CHM `nots.htm`; E-DB `testtable` 609 rows; E-CODE forms |
| **Confidence** | CONFIRMED |
| **Known limitations** | Whether adding a test also requires an explicit edit of the profile's `Data\*.ini` layout is INFERRED, not documented. |

### F-V1-012 — Administer normal reference ranges

| Field | Content |
|---|---|
| **Function ID** | F-V1-012 |
| **Name** | Administer normal reference ranges |
| **Arabic label** | «كيف يتم اضافة أو تعديل المعدلات الطبيعية لتحليل معين ؟» |
| **Domain** | Clinical knowledge |
| **Purpose** | Maintain the sex- and age-banded reference ranges that drive result classification. |
| **Business context** | Reference ranges are the safety-critical knowledge base. They are per test, per sex, per age band. |
| **Actor** | Laboratory administrator |
| **Entry point** | بيانات النظام → بيانات التحاليل → [test] → «المعدل الطبيعي» |
| **Preconditions** | Signed in with administrator rights; the test exists in `testtable` |
| **Trigger** | Operator opens the range editor for a test |
| **Step-by-step workflow** | 1. Open the normal-rate editor. 2. Add or edit a range. 3. Set: `Test name`, `Test unit`, `Sex`, `Age From`, `Age To`, `Age unit`, `Normal range`, `Low/High limit`, `Low/High flag`, `Low/High comment`, `Critical comment`, `Critical range`, critical `Low/High limit`, `Critical flag`. 4. Save. 5. Undo or delete as required. |
| **Inputs** | The 14 range attributes above |
| **Input validation** | The manual documents the fields; overlapping range detection is **NOT ESTABLISHED**. |
| **System responses** | The range is stored and becomes available to the classification engine |
| **Outputs** | `normalrange` rows |
| **Resulting state** | Subsequent result entry for that test uses the new limits |
| **Postconditions** | — |
| **Business rules** | V1-BR-011 (ranges are sex- and age-banded) · V1-BR-018 (the exception text and the critical comment are stored per range, not per test) · V1-BR-019 (the urine analysis's range is bound to the immutable test number 4) |
| **Permissions** | Administrator rights |
| **Entities** | `normalrange` (852 rows, 18 columns), `testtable` |
| **Data read** | `testtable`, `testunits` |
| **Data created** | `normalrange` row |
| **Data updated** | `normalrange` row |
| **Data deleted** | `normalrange` row |
| **Reports / printed output** | The range is not printed; it drives highlighting and comment selection |
| **Configuration dependencies** | `testunits` for the unit list |
| **Exceptions** | Not documented |
| **Alternative paths** | — |
| **Related functions** | F-V1-008, F-V1-011, F-V1-024 |
| **Technical dependencies** | Forms `testrange`(17), `outtest`(11), `normal_test`(35) |
| **Evidence** | E-CHM `008.htm` (3,042 chars, 2 images, verbatim field list); E-DB `normalrange` schema and distributions; E-CHM `nots.htm` |
| **Confidence** | CONFIRMED — the documented 14 attributes map one-to-one onto the extracted schema |
| **Known limitations** | `pregsmok` present on all 852 rows; its role is UNKNOWN. |

### F-V1-013 — Customise report text, dimensions and colours

| Field | Content |
|---|---|
| **Function ID** | F-V1-013 |
| **Name** | Customise report text, dimensions and colours |
| **Arabic label** | «كيف يتم التعديل فى نصوص وأبعاد وألوان التقرير ؟» |
| **Domain** | Reporting |
| **Purpose** | Adapt the printed report to the laboratory's paper, printer and branding. |
| **Business context** | The report is the laboratory's product. Its layout is per-clinical-profile and must survive printer and paper changes. |
| **Actor** | Laboratory administrator |
| **Entry point** | `reportsetup` (108 controls), `reportsetup2` (69 controls) |
| **Preconditions** | Signed in with administrator rights; the profile's `.ini` present |
| **Trigger** | Operator opens report setup for a profile |
| **Step-by-step workflow** | 1. Open report setup. 2. Edit the heading texts, the 7 column positions and 7 column widths, the colour assignments, and the font for each position. 3. Edit the five static blocks (`static1`–`static5`). 4. Save — the change is written into the profile's `Data\*.ini`. 5. Bind a printer. |
| **Inputs** | Headings, positions, widths, colours, fonts, printer name, paper size |
| **Input validation** | Not documented |
| **System responses** | The stored `.ini` changes; the next print uses the new layout |
| **Outputs** | An updated `Data\*.ini` |
| **Resulting state** | The profile's report layout changes |
| **Postconditions** | — |
| **Business rules** | V1-BR-020 (the report colour palette is a 19-entry table stored in a text file named `REPORT.dll`) · V1-BR-021 (each profile binds its own printer) · V1-BR-022 (the paper size is a global setting in `SETTING.ini`, not per profile) |
| **Permissions** | Administrator rights |
| **Entities** | `Data\*.ini` (223 files / 114 profiles), `REPORT.dll` (colour palette) |
| **Data read** | `Data\*.ini`, `REPORT.dll`, `SETTING.ini` |
| **Data created / updated / deleted** | The `.ini` file is rewritten |
| **Reports / printed output** | All prints of that profile change |
| **Configuration dependencies** | `SETTING.ini [setup] papersize=A4`; `SETTING.ini [selectprinter]`; `Data\*.ini [setupx] reportprinterx` |
| **Exceptions** | «Can't find any printer.» |
| **Alternative paths** | The 108-control `reportsetup` and the 69-control `reportsetup2` provide two editing depths — the manual does not distinguish them |
| **Related functions** | F-V1-021, F-V1-008 |
| **Technical dependencies** | 45 `DataReport` design objects; VB6 `Printer` object |
| **Evidence** | E-CHM `009.htm` (4,001 chars, 3 images); E-CODE forms; E-CFG `Data\*.ini`, `REPORT.dll`, `SETTING.ini` |
| **Confidence** | CONFIRMED |
| **Known limitations** | The interaction between the 45 DataReport designs and the 223 `.ini` layouts is not established — it is not evident which DataReport serves which profile. |

### F-V1-014 — Manage send-out samples

| Field | Content |
|---|---|
| **Function ID** | F-V1-014 |
| **Name** | Manage samples referred to an external laboratory |
| **Arabic label** | «كيف يتم ادارة العينات المرسلة للخارج من داخل النظام ؟» |
| **Domain** | Send-out / referral laboratory |
| **Purpose** | Track tests that leave the laboratory, group them by destination, settle the amounts, and reconcile what is outstanding. |
| **Business context** | Send-out is both a revenue stream and a quality risk: the referring laboratory's result enters the patient's record. |
| **Actor** | Laboratory administrator; accounting user |
| **Entry point** | F7 → accounting → send-out samples; forms `sarfz`(14), `moenysharekat`(0), `moenymozafeen`(27) |
| **Preconditions** | Signed in; the test is flagged `inotherlab` |
| **Trigger** | Operator opens the send-out screen |
| **Step-by-step workflow** | 1. Tests flagged `inotherlab = True` in `testtable` are discovered automatically. 2. Open the send-out screen (F7, or accounting → send-out samples). 3. Group by destination laboratory (`otherlabname`) and total. 4. Send — enter the amount settled, mark «خالص» (settled), mark «دائماً إليه» (always send to it). 5. Follow up unsent and unsettled items. |
| **Inputs** | Destination laboratory, settled amount |
| **Input validation** | Not documented |
| **System responses** | Send-out rows grouped and totalled; settlement recorded |
| **Outputs** | `patienttest.sentout` / `.send` state |
| **Resulting state** | Send-out state per test |
| **Postconditions** | Accounting (F-V1-015) can report on send-out |
| **Business rules** | V1-BR-017 (send-out enrolment is driven by the test's `inotherlab` flag, not by a manual selection) · V1-BR-023 (the «دائماً إليه» flag makes the destination the standing default for that test) · V1-BR-024 (settlement has two independent states: sent and settled) |
| **Permissions** | Signed-in session |
| **Entities** | `patienttest` (`sentout`, `send`, `otherlabname`), `testtable` (`inotherlab`), `otherlabname` |
| **Data read** | `testtable`, `patienttest` |
| **Data created** | `otherlabname` entry when a new destination is named |
| **Data updated** | `patienttest.sentout`, `patienttest.send` |
| **Data deleted** | None |
| **Reports / printed output** | Send-out settlement listing (structure NOT ESTABLISHED) |
| **Configuration dependencies** | `SETTING.ini [setup] labbranchx` |
| **Exceptions** | Not documented |
| **Alternative paths** | F7 keyboard route or accounting-menu route |
| **Related functions** | F-V1-011, F-V1-015 |
| **Technical dependencies** | Forms `sarfz`, `moenysharekat`, `moenymozafeen` |
| **Evidence** | E-CHM `010.htm` (2,196 chars, 2 images, verbatim «خالص» / «دائماً إليه»); E-DB `patienttest` columns `sentout`, `send`, `otherlabname`; E-DB `testtable.inotherlab` |
| **Confidence** | CONFIRMED |
| **Known limitations** | The reconciliation of a returning external result into the patient's record is NOT ESTABLISHED. |

### F-V1-015 — Run accounting and stock-take

| Field | Content |
|---|---|
| **Function ID** | F-V1-015 |
| **Name** | Accounting and periodic stock-take |
| **Arabic label** | «كيف يتم معرفة حساب الدرج|work جرد لفترة محددة ؟» — the manual's own wording: «كيف يتم معرفة حساب الدرج وعمل جرد لفترة محددة ؟» |
| **Domain** | Finance |
| **Purpose** | Reconcile income, expenses, referral commissions and sample volumes for a chosen period. |
| **Business context** | The periodic financial close of a laboratory. The manual poses the defining question explicitly: «هل الفلوس المحصلة من مرضى الفترة فقط؟» |
| **Actor** | Laboratory administrator; accounting user |
| **Entry point** | F7 → accounting and stock-take window; `labfunction` module |
| **Preconditions** | Signed in; a reporting period chosen |
| **Trigger** | Operator opens the accounting window for a period |
| **Step-by-step workflow** | 1. Choose the period. 2. **Patient accounts** — decide and apply whether collection is restricted to that period's patients. 3. **Send-out accounts** — aggregated or broken down per destination laboratory. 4. **Expenses and remittances** — the separate accounting window for cash withdrawal and deposit. 5. **Residual balances** for patients, plus **referring-party commissions** (`Gehats.omola`). 6. **Per-user workload** and per-user collections. 7. **Claims / volume of dealings with other parties.** |
| **Inputs** | Reporting period; patient / send-out / referral-party filters |
| **Input validation** | Not documented |
| **System responses** | Accounting listings and totals per period |
| **Outputs** | Stock-take figures, commission figures, workload figures |
| **Resulting state** | Read-only reporting |
| **Postconditions** | — |
| **Business rules** | V1-BR-025 (the collection scope for a period is an explicit operator decision, not an automatic rule) · V1-BR-026 (referring-party commission is carried on the `Gehats` record, not computed from the referral) · V1-BR-027 (per-user workload is a first-class accounting output, which is why print attribution matters) |
| **Permissions** | Signed-in session |
| **Entities** | `payment`, `patientinfo` (`totalprice`, `totalpricedisc`), `patienttest` (`sentout`, `send`), `Gehats` (`omola`), `otherlabname` |
| **Data read** | All of the above |
| **Data created / updated / deleted** | None established — the function is presented as a calculation over existing data |
| **Reports / printed output** | Stock-take and commission listings |
| **Configuration dependencies** | `SETTING.ini [setup] labbranchx` (branch scoping) |
| **Exceptions** | Not documented |
| **Alternative paths** | — |
| **Related functions** | F-V1-010, F-V1-014, F-V1-022 |
| **Technical dependencies** | `labfunction` module (13 methods) |
| **Evidence** | E-CHM `011.htm` (1,682 chars, 2 images, verbatim period-scope question); E-DB `payment`, `Gehats`; E-CODE `labfunction` |
| **Confidence** | CONFIRMED for the six sub-reports; INFERRED for the underlying formulas |
| **Known limitations** | The arithmetic behind each sub-report is not established from static evidence. |

### F-V1-016 — Back up the database

| Field | Content |
|---|---|
| **Function ID** | F-V1-016 |
| **Name** | Back up the laboratory database |
| **Arabic label** | «كيف يتم عمل نسخ احتياطي لقاعدة بيانات النظام ؟» |
| **Domain** | Administration |
| **Purpose** | Produce a restorable copy of the live database. |
| **Business context** | The database is the sole record of all patient results. Loss is unrecoverable. |
| **Actor** | Laboratory administrator; system administrator |
| **Entry point** | Administrative backup function; `labdb` (20 methods), `labdb1` (19 methods) |
| **Preconditions** | Signed in with administrator rights; no exclusive write lock on the `.mdb` |
| **Trigger** | Operator initiates the backup |
| **Step-by-step workflow** | 1. Invoke the backup. 2. The system copies the database. 3. A backup file is produced. |
| **Inputs** | The live database |
| **Input validation** | Not documented |
| **System responses** | A backup file |
| **Outputs** | `Patients_backup.mdb` — the shipped artefact has 90 tables / 1,381 columns / 2,786 rows |
| **Resulting state** | A backup copy exists |
| **Postconditions** | — |
| **Business rules** | V1-BR-028 (backup is copy-based; it is not a transactionally consistent snapshot of a live Jet 4 database) |
| **Permissions** | Administrator rights |
| **Entities** | `Patients.mdb`, `Patients_backup.mdb`, `data.mdb` |
| **Data read** | The whole database |
| **Data created** | The backup file |
| **Data updated / deleted** | None established |
| **Reports / printed output** | None |
| **Configuration dependencies** | The backup destination path is NOT ESTABLISHED |
| **Exceptions** | Not documented |
| **Alternative paths** | `labdb` and `labdb1` provide two backup-related modules |
| **Related functions** | F-V1-001 |
| **Technical dependencies** | Jet 4 file copy |
| **Evidence** | E-CHM `backup.htm` (3,058 chars, 6 images); E-CODE `labdb`/`labdb1`; E-DB `Patients_backup.mdb` |
| **Confidence** | CONFIRMED for the existence of the function; INFERRED for the mechanism |
| **Known limitations** | **The shipped backup is older than the shipped primary database** — it has 90 tables against 93 and 1,381 columns against 1,527, and 2,786 rows against 3,123. Whether backup is scheduled, manual, or incidental is NOT ESTABLISHED. See RL-V1-Q-12. |

### F-V1-017 — Manage test units and unit conversions

| Field | Content |
|---|---|
| **Function ID** | F-V1-017 |
| **Name** | Manage the unit dictionary and unit-conversion factors |
| **Arabic label** | — (no manual topic; inferred from the `z_units` and `converter` forms) |
| **Domain** | Clinical knowledge |
| **Purpose** | Maintain the measurement-unit dictionary and the conversion coefficients between units. |
| **Business context** | Results arrive in one unit and must be interpretable in another; conversion factors are clinical knowledge. |
| **Actor** | Laboratory administrator |
| **Entry point** | `z_units` (21 controls), `converter` (14 controls) |
| **Preconditions** | Signed in with administrator rights |
| **Trigger** | Operator opens the unit or conversion editor |
| **Step-by-step workflow** | 1. Open the unit dictionary. 2. Add or edit a unit. 3. Open the conversion editor. 4. Define a source unit, a target unit and the factor. 5. Save. |
| **Inputs** | Unit name, source unit, target unit, factor |
| **Input validation** | Not documented |
| **System responses** | The dictionaries are updated |
| **Outputs** | `testunits` rows (69), `converts` rows (167) |
| **Resulting state** | The dictionaries are extended |
| **Postconditions** | — |
| **Business rules** | V1-BR-029 (conversion factors are stored per analyte-and-unit-pair, not globally) — e.g. Acetaminophen µg/mL→µmol/L 6.62, Albumin g/dL→g/L 10 |
| **Permissions** | Administrator rights |
| **Entities** | `testunits`, `converts`, `testtable` |
| **Data read** | `testtable` (which analyte the conversion belongs to) |
| **Data created / updated / deleted** | Dictionary rows |
| **Reports / printed output** | The converted value may be shown on the report — NOT ESTABLISHED |
| **Configuration dependencies** | None specific |
| **Exceptions** | Not documented |
| **Alternative paths** | — |
| **Related functions** | F-V1-011, F-V1-012 |
| **Technical dependencies** | Forms `z_units`, `converter` |
| **Evidence** | E-CODE form inventory; E-DB `testunits` 69 rows, `converts` 167 rows |
| **Confidence** | STRONGLY SUPPORTED for the data structures; INFERRED for the interactive workflow |
| **Known limitations** | **No manual topic documents this function.** Whether conversion is applied automatically during entry or only on display is NOT ESTABLISHED. |

### F-V1-018 — Manage abbreviations

| Field | Content |
|---|---|
| **Function ID** | F-V1-018 |
| **Name** | Manage the abbreviation dictionary |
| **Arabic label** | — (no manual topic) |
| **Domain** | Clinical knowledge |
| **Purpose** | Maintain the dictionary that expands abbreviations in result display and reports. |
| **Business context** | Abbreviations are how a laboratory fits an analyte name into a narrow printed column. |
| **Actor** | Laboratory administrator |
| **Entry point** | `abbrevi` (41 controls) |
| **Preconditions** | Signed in with administrator rights |
| **Trigger** | Operator opens the abbreviation editor |
| **Step-by-step workflow** | 1. Open the dictionary. 2. Add or edit an abbreviation and its expansion. 3. Save. |
| **Inputs** | Abbreviation, expansion, analyte association |
| **Input validation** | Not documented |
| **System responses** | Dictionary updated |
| **Outputs** | `abbreviations` rows (650) |
| **Resulting state** | Abbreviation rendering changes |
| **Postconditions** | — |
| **Business rules** | V1-BR-030 (abbreviations are maintained independently of the test catalogue) |
| **Permissions** | Administrator rights |
| **Entities** | `abbreviations`, `testtable` |
| **Data read / created / updated / deleted** | `abbreviations` |
| **Reports / printed output** | The abbreviation appears in printed reports |
| **Configuration dependencies** | `Data\*.ini` column widths interact with abbreviation length |
| **Exceptions** | Not documented |
| **Alternative paths** | — |
| **Related functions** | F-V1-011, F-V1-013 |
| **Technical dependencies** | Form `abbrevi` |
| **Evidence** | E-CODE form inventory; E-DB `abbreviations` 650 rows |
| **Confidence** | STRONGLY SUPPORTED |
| **Known limitations** | No manual topic; the exact linkage rule is not established. |

### F-V1-019 — Manage clinical sub-groups

| Field | Content |
|---|---|
| **Function ID** | F-V1-019 |
| **Name** | Manage clinical sub-groups (test profiles) |
| **Arabic label** | — (no manual topic) |
| **Domain** | Test catalogue |
| **Purpose** | Define the clinical groupings that determine which entry window and which report layout a test belongs to. |
| **Business context** | The sub-group is the pivot between the test catalogue and the profile report. |
| **Actor** | Laboratory administrator |
| **Entry point** | `z_group` (21 controls), `z_customgroup` (37 controls) |
| **Preconditions** | Signed in with administrator rights |
| **Trigger** | Operator opens the sub-group editor |
| **Step-by-step workflow** | 1. Open the sub-group editor. 2. Add or edit a sub-group. 3. Associate tests with it. 4. Save. |
| **Inputs** | Sub-group name (e.g. «BLOOD GLUCOSE», «KIDNEY FUNCTIONS», «LIVER FUNCTIONS», «LIPIDS PROFILE», «URINE EXAMINATION», «STOOL EXAMINATION», «Autoimmune diseases», «BLOOD GROUP», «Erythrocyte Sedimentation Rate», «CULTURE AND SENSITIVITY», «BLOOD PICTURE»), member tests |
| **Input validation** | Not documented |
| **System responses** | The grouping is updated and affects entry-window routing and report selection |
| **Outputs** | `subgrouptable` rows (46), `customgrouptable` |
| **Resulting state** | Test-to-profile mapping changes |
| **Postconditions** | — |
| **Business rules** | V1-BR-031 (the sub-group is the routing key for both the entry window and the report layout) |
| **Permissions** | Administrator rights |
| **Entities** | `subgrouptable`, `customgrouptable`, `testtable`, `Data\*.ini` |
| **Data read / created / updated / deleted** | `subgrouptable` |
| **Reports / printed output** | Indirect — the sub-group selects the layout |
| **Configuration dependencies** | `Data\*.ini` file naming convention is aligned to sub-groups |
| **Exceptions** | Not documented |
| **Alternative paths** | — |
| **Related functions** | F-V1-007, F-V1-011, F-V1-013 |
| **Technical dependencies** | Forms `z_group`, `z_customgroup` |
| **Evidence** | E-CODE form inventory; E-DB `subgrouptable` 46 rows with the names listed |
| **Confidence** | STRONGLY SUPPORTED |
| **Known limitations** | The relationship between 46 sub-groups and 223 `.ini` files is not established (see RL-V1-Q-07). |

### F-V1-020 — Manage users, administrators and login groups

| Field | Content |
|---|---|
| **Function ID** | F-V1-020 |
| **Name** | Manage user accounts, administrators and login groups |
| **Arabic label** | — (no manual topic) |
| **Domain** | Security administration |
| **Purpose** | Create accounts, assign administrative rights and group users for session and workload attribution. |
| **Business context** | Every operator must be individually identifiable for print attribution and workload accounting. |
| **Actor** | Laboratory administrator |
| **Entry point** | `users` (29 controls), `userspw` (13 controls), `z_group` (21 controls) |
| **Preconditions** | Signed in with administrator rights |
| **Trigger** | Operator opens user administration |
| **Step-by-step workflow** | 1. Open user administration. 2. Create or edit a user; assign the login group and permission flags. 3. Reset a password via the password form. |
| **Inputs** | User name, password, group, permission flags |
| **Input validation** | Not documented |
| **System responses** | The account is created or updated |
| **Outputs** | `useraccounts` rows, `loggroup` rows |
| **Resulting state** | The account can sign in |
| **Postconditions** | — |
| **Business rules** | V1-BR-003 (the `admin` account is the documented initial administrator) · V1-BR-032 (the login group is the unit of session scoping and workload attribution, not a role hierarchy) |
| **Permissions** | Administrator rights |
| **Entities** | **`patients` (operator table)**, `loggroup` |
| **Data read / created / updated / deleted** | `patients` — including the twelve permission bits `grant1`…`grant12`; `loggroup` |
| **Reports / printed output** | None |
| **Configuration dependencies** | `SETTING.ini [setup] labbranchx` |
| **Exceptions** | Not documented |
| **Alternative paths** | — |
| **Related functions** | F-V1-002, F-V1-015 |
| **Technical dependencies** | Forms `users`, `userspw` |
| **Evidence** | E-CODE form inventory; E-DB `useraccounts`, `loggroup`; E-CHM `002.htm` |
| **Confidence** | STRONGLY SUPPORTED |
| **Known limitations** | The permission flag semantics are NOT ESTABLISHED. Version 1 has no `grant13..grant17` fields — those are a Version 2 addition. |

### F-V1-021 — Configure printers, paper and report headers

| Field | Content |
|---|---|
| **Function ID** | F-V1-021 |
| **Name** | Configure printers, paper size and report headers |
| **Arabic label** | — (no manual topic) |
| **Domain** | Reporting / deployment |
| **Purpose** | Bind printers to roles, set the paper size, and set the report header text. |
| **Business context** | A laboratory runs several printers — a report printer, a barcode printer, and a third role `zarfprinter`. Each must be bound correctly or output lands on the wrong device. |
| **Actor** | Laboratory administrator |
| **Entry point** | `SETTING.ini [selectprinter]` and `[setup]`; `printerselect` module (16 methods) |
| **Preconditions** | Signed in with administrator rights; the Windows printer exists |
| **Trigger** | Operator edits the configuration or the printer selection window |
| **Step-by-step workflow** | 1. Bind the report printer (`reportprinter`, `reportport`), the barcode printer (`barcodeprinter`, `barcodeport`) and the third role (`zarfprinter`). 2. Set `papersize` (A4/A5). 3. Set the header texts (`Textr1txt` = «المنظومة البرمجية», `Textr2txt` = «ريال لاب سيستم») and the 12 header/footer text positions. 4. Set `reportstart`, `historytype`, `labbranchx`, `barcodeauto`, `printworkpaper`. |
| **Inputs** | Printer names, ports, paper size, header strings |
| **Input validation** | The spooler reports an error via «SetPrinter Failed. Error code: » |
| **System responses** | Configuration saved; the printer is set as the current device |
| **Outputs** | Updated `SETTING.ini` |
| **Resulting state** | Printing targets and page geometry change |
| **Postconditions** | — |
| **Business rules** | V1-BR-021 (three printer roles are bound: report, barcode, and `zarfprinter`) · V1-BR-022 (paper size is global, not per profile) · V1-BR-033 (the report sequence origin is an absolute value `reportstart=1984`) · V1-BR-034 (barcode auto-creation and work-sheet printing are independently toggleable) |
| **Permissions** | Administrator rights |
| **Entities** | `SETTING.ini`, `Data\*.ini` `[setupx] reportprinterx` |
| **Data read / created / updated / deleted** | `SETTING.ini` |
| **Reports / printed output** | All output |
| **Configuration dependencies** | Windows printer subsystem |
| **Exceptions** | «Can't find any printer.»; «SetPrinter Failed. Error code: » |
| **Alternative paths** | A profile may bind a printer in its own `.ini`, overriding the global role binding |
| **Related functions** | F-V1-004, F-V1-013 |
| **Technical dependencies** | `printerselect` module; Win32 spooler API |
| **Evidence** | E-CFG `SETTING.ini`; E-CODE `printerselect`; E-CODE literals «Can't find any printer.», «SetPrinter Failed. Error code: » |
| **Confidence** | CONFIRMED |
| **Known limitations** | The precedence between the global role binding and a profile's `reportprinterx` is NOT ESTABLISHED. |

### F-V1-022 — Manage referring parties and referring physicians

| Field | Content |
|---|---|
| **Function ID** | F-V1-022 |
| **Name** | Manage referring parties and referring physicians |
| **Arabic label** | — (no manual topic) |
| **Domain** | Patient master data / finance |
| **Purpose** | Maintain the list of clinics and physicians that refer work to the laboratory, together with their commission terms. |
| **Business context** | Referral is the laboratory's commercial channel; commission is the price of that channel. |
| **Actor** | Laboratory administrator |
| **Entry point** | `z_gehats` form (Version 2 evidence: `z_gehats` and `Gehats`); Version 1 evidence: `Gehats` table |
| **Preconditions** | Signed in with administrator rights |
| **Trigger** | Operator opens the referring-party editor |
| **Step-by-step workflow** | 1. Open the referring-party list. 2. Add or edit a referring party, with its commission (`omola`). 3. Save. |
| **Inputs** | Party name, commission |
| **Input validation** | Not documented |
| **System responses** | The party becomes selectable during patient registration |
| **Outputs** | `Gehats` rows |
| **Resulting state** | The referring-party list changes |
| **Postconditions** | Commission enters the stock-take calculation (F-V1-015) |
| **Business rules** | V1-BR-026 (commission is stored on the referring-party record) |
| **Permissions** | Administrator rights |
| **Entities** | `Gehats` (2 rows in the shipped data), `patientinfo.referredbyname` |
| **Data read / created / updated / deleted** | `Gehats` |
| **Reports / printed output** | Stock-take commission listing |
| **Configuration dependencies** | None specific |
| **Exceptions** | Not documented |
| **Alternative paths** | — |
| **Related functions** | F-V1-003, F-V1-015 |
| **Technical dependencies** | Form `z_gehats` |
| **Evidence** | E-DB `Gehats` with `omola`; E-CODE form inventory; E-DB `patientinfo.referredbyname` |
| **Confidence** | STRONGLY SUPPORTED |
| **Known limitations** | The distinction between a referring party (`Gehats`) and a referring physician (`mowzaf`) is a Version 2 distinction; in Version 1 the evidence supports `Gehats` only. |

### F-V1-023 — Manage the picture / image gallery

| Field | Content |
|---|---|
| **Function ID** | F-V1-023 |
| **Name** | Attach and view patient-related pictures |
| **Arabic label** | — |
| **Domain** | Patient master data |
| **Purpose** | Store and display images associated with a patient record. |
| **Business context** | Evidence in a laboratory record is often an image (a slide, a gel, a photograph of a specimen). |
| **Actor** | Lab operator |
| **Entry point** | `Frm_pic` (49 controls), `Frm_pic2` (38 controls); `real lab Image Gallery.exe` |
| **Preconditions** | Signed in; a patient selected |
| **Trigger** | Operator opens the picture view for a patient |
| **Step-by-step workflow** | 1. Open the picture frame. 2. The images linked to the patient are read from the `pictures` table. 3. Display; the standalone `real lab Image Gallery.exe` provides an enlarged viewer. |
| **Inputs** | Patient identifier; image file(s) |
| **Input validation** | Not documented |
| **System responses** | Images displayed |
| **Outputs** | — |
| **Resulting state** | Read-only display |
| **Postconditions** | — |
| **Business rules** | Not established |
| **Permissions** | Signed-in session |
| **Entities** | `pictures` |
| **Data read** | `pictures` |
| **Data created / updated / deleted** | Not established |
| **Reports / printed output** | None |
| **Configuration dependencies** | None specific |
| **Exceptions** | Not documented |
| **Alternative paths** | The standalone gallery executable is an alternative viewer |
| **Related functions** | F-V1-003 |
| **Technical dependencies** | `real lab Image Gallery.exe` |
| **Evidence** | E-CODE form inventory; `analysis/CUMULATIVE/01-architecture.md` |
| **Confidence** | INFERRED |
| **Known limitations** | The `pictures` table was not present in the extracted `Patients.mdb` schema — the linkage is inferred from the two image frames and the standalone executable. |

### F-V1-024 — Work with method sheets and critical values

| Field | Content |
|---|---|
| **Function ID** | F-V1-024 |
| **Name** | Consult the method sheet and critical-value thresholds for a test |
| **Arabic label** | — (the sheets are Arabic documents; see the artefact catalogue) |
| **Domain** | Clinical knowledge / result entry |
| **Purpose** | Give the operator the measurement principle, sample type and critical thresholds for the test being entered. |
| **Business context** | Critical values are the values that must be escalated immediately. They are stated per analyte in the method sheet. |
| **Actor** | Lab operator |
| **Entry point** | The result-entry window, alongside the analyte field |
| **Preconditions** | Signed in |
| **Trigger** | Operator consults the sheet for the current test |
| **Step-by-step workflow** | 1. The method sheet for the analyte is retrieved from `Cache\*.rtf`. 2. It is displayed beside the entry field. 3. The operator applies the stated thresholds. |
| **Inputs** | The test identifier |
| **Input validation** | Not applicable |
| **System responses** | The Arabic method sheet is displayed |
| **Outputs** | Method text; critical-value thresholds |
| **Resulting state** | Read-only |
| **Postconditions** | — |
| **Business rules** | V1-BR-035 (critical thresholds are stated in the method sheet, not in `normalrange`) — the sheet and the range table are **two separate sources of criticality**, and the shipped data shows both: `normalrange` has a `Critical range` / `Critical comment` / `Critical flag` per row, while `Cache\*.rtf` states thresholds in prose |
| **Permissions** | Signed-in session |
| **Entities** | `Cache\*.rtf` (29 sheets), `normalrange` |
| **Data read** | `Cache\*.rtf` |
| **Data created / updated / deleted** | None |
| **Reports / printed output** | The method sheet may be printed — NOT ESTABLISHED |
| **Configuration dependencies** | `Cache\` directory presence |
| **Exceptions** | Not documented |
| **Alternative paths** | — |
| **Related functions** | F-V1-007, F-V1-008, F-V1-012 |
| **Technical dependencies** | RTF rendering in the VB6 form |
| **Evidence** | E-PRES `analysis/BATCH-02/rtf/` — 29 sheets with parsed critical thresholds; E-DB `normalrange` critical columns |
| **Confidence** | CONFIRMED for the existence of the sheets and their content; INFERRED for the in-window presentation |
| **Known limitations** | **Which analytes have a sheet is not established** — 29 sheets against 609 tests. The mapping rule is UNKNOWN. |

### F-V1-025 — Use the interactive training simulators

| Field | Content |
|---|---|
| **Function ID** | F-V1-025 |
| **Name** | Use the interactive training simulators |
| **Arabic label** | — |
| **Domain** | Training |
| **Purpose** | Let a new operator rehearse clinical workflows in a safe simulation. |
| **Business context** | The laboratory trains staff without risking live patient data. |
| **Actor** | Trainee; trainer |
| **Entry point** | `Cache\*.exe` — 13 Flash-hosted simulators |
| **Preconditions** | Flash Player 8 runtime available |
| **Trigger** | Operator launches a simulator |
| **Step-by-step workflow** | 1. Launch the simulator executable. 2. The Flash presentation runs. 3. The trainee navigates the simulated workflow and receives feedback. |
| **Inputs** | Navigational input in the simulation |
| **Input validation** | Not applicable |
| **System responses** | Simulated screen responses |
| **Outputs** | Training feedback |
| **Resulting state** | Read-only |
| **Postconditions** | — |
| **Business rules** | V1-BR-036 (the training library is a separate executable set and does not touch the live database) |
| **Permissions** | None — the simulators are standalone |
| **Entities** | `Cache\*.exe` (13), plus `projectors.csv` — Word binary tables used as the Flash projectors |
| **Data read** | The simulator's own embedded content |
| **Data created / updated / deleted** | None |
| **Reports / printed output** | None |
| **Configuration dependencies** | Flash Player 8 |
| **Exceptions** | Not documented |
| **Alternative paths** | — |
| **Related functions** | F-V1-008, F-V1-011 |
| **Technical dependencies** | Flash Player 8; the Word-binary projector mechanism |
| **Evidence** | E-PRES `analysis/BATCH-03/flash/` — 13 simulators, 379 frames, 640×480; `analysis/CUMULATIVE/09-presentation-library.md` |
| **Confidence** | CONFIRMED |
| **Known limitations** | **The simulators are byte-identical in Version 2** — the training material was not updated in seven years. The exact workflow simulated by each of the 13 executables is documented in `analysis/BATCH-03/flash/projectors.csv` but not in this package's scope. |

---

## 5. Workflow specifications

### WF-V1-01 — Patient episode, end to end

| Step | Function | Entity written | Gate |
|---:|---|---|---|
| 1 | F-V1-002 Sign in | `loggroup` session | Must succeed |
| 2 | F-V1-003 Register patient | `patientinfo`, `barcodes` | — |
| 3 | F-V1-004 Print barcode | `barcodes` | — |
| 4 | Order tests | `patienttest` | — |
| 5 | F-V1-008 Enter result | profile table, `patienttest` | Range classification applied |
| 6 | F-V1-009 Confirm | `patienttest` review state | **Gate for release** |
| 7 | F-V1-013 Print report | print audit on `patienttest` | Printer bound |
| 8 | F-V1-010 Hand over | `payment`, `patienttest.give` | **Step 6 must be complete** |

### WF-V1-02 — Result classification decision

```
value entered
   │
   ├─ within [lowlimit, highlimit] ──────────► normal, no flag
   │
   ├─ < lowlimit ─────────────────────────────► abnormal (low)  + exception text (nolowexe)
   │        └─ also beyond critical limit ────► CRITICAL
   ├─ > highlimit ────────────────────────────► abnormal (high) + exception text (nohighexe)
   │        └─ also beyond critical limit ────► CRITICAL
   │
   └─ no matching range row ──────────────────► behaviour NOT ESTABLISHED
```

### WF-V1-03 — Send-out lifecycle

```
test.inotherlab = True
   └─► appears in the send-out screen automatically
        ├─► grouped by otherlabname
        ├─► amount settled entered
        ├─► «خالص»  → settled state
        └─► «دائماً إليه» → standing default destination for this test
```

### WF-V1-04 — Reference-range edit

```
Setup → Test data → [test] → «المعدل الطبيعي»
   ├─ Add    → new normalrange row
   ├─ Edit   → update row
   ├─ Save   → commit
   ├─ Undo   → revert
   └─ Delete → remove row
   ⚠ Test number 4 (urine analysis): number and name are immutable
```

### WF-V1-05 — Report print

```
F11 (preview) or F12 (print)
   └─► printerselect resolves the printer
         ├─ profile binding: Data\*.ini [setupx] reportprinterx
         └─ role binding:    SETTING.ini [selectprinter] reportprinter
   └─► DataReport renders using Data\*.ini [reportdata]
   └─► spooler dispatch (OpenPrinterA / SetPrinterA / …)
   └─► print audit written to patienttest
```

---

## 6. Functions explicitly **not** present in V-ROOT-1.00

The following are Version 2 capabilities. They are recorded here so that the absence
is explicit rather than assumed. **Absence of a feature in Version 1 is established
by exhaustive literal and object search in the Version 1 executable.**

| Capability | Status in V-ROOT-1.00 | Basis |
|---|---|---|
| PDF export | **ABSENT — CONFIRMED** | See V1-BR-046. Zero occurrences of `PDF` in 104,997 string literals; no PDF-capable printer bound |
| Branch master/reception model | **ABSENT — CONFIRMED** | No `Patient.branche`, no `Labbranch`, no `SUBSTRING(patientid,9,1)` |
| Attendance / departure log | **ABSENT — CONFIRMED** | No `loger` table reference |
| Laboratory statistics module | **ABSENT — CONFIRMED** | No `statisReport` object |
| Work-sheet module | **ABSENT — CONFIRMED** | No `workpaper` / `workpaper2` objects |
| SMS subsystem | **ABSENT — CONFIRMED** | No `SMSSender` / `smsgroups` / `smslist` / `SMStemplate` references |
| NATIGH.COM result delivery | **ABSENT — CONFIRMED** | No `Natigh` reference |
| Embedded schema migration | **ABSENT — CONFIRMED** | Zero DDL statements |
| MySQL database option | **NOT ESTABLISHED in V1** | `BASE.ini` encodes a multi-engine selector, but no MySQL artefact ships with Version 1 |
| `grant13..grant17` per-user permission flags | **ABSENT — CONFIRMED** | `patientinfo` has no such columns in the V1 schema |
| Statistics-driven monthly reporting (`pricelistsN`) | **ABSENT — CONFIRMED** | No `pricelistsN` object |

---

## 7. Documented-but-not-modelled functions

| Item | Why it is not modelled as a function |
|---|---|
| «ملاحظات يجب مراعتها عند استخدام نظام ريال لاب سيستم» (`nots.htm`) | It is a **notes topic**, not a function. Its content is decomposed into V1-BR-016 (test number 4), V1-BR-001…004 (server settings), and the constants in V1-BR-037…V1-BR-041. |
| The 114 clinical profiles | Each profile is a **data-driven instantiation** of F-V1-007 / F-V1-008, not a separate function. The profile catalogue is in `Data_Model_and_Workflow_Entities_V1.md` §3. |
| The 46 clinical sub-groups | A catalogue of `subgrouptable`, not functions. Covered by F-V1-019. |
| The 20 `daytip` help strings | A localised help table, not functions. Covered under F-V1-008 and §3.3 of the reference. |

---

**End of Functional_Specification_V1.md**
