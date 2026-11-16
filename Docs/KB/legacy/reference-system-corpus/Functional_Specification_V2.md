# Functional Specification — RealLab V-CD-2019 (v1.605)

**Document ID:** RL-V2-FUN-001
**Version documented:** `V-CD-2019`
**Evidence basis:** 473 verified artefacts · static analysis only, no execution
**Companion documents:** `Complete_Reverse_Engineering_Reference_V2.md`, `Business_Rules_V2.md`, `Data_Model_and_Workflow_Entities_V2.md`

---

## 1. Scope, method and reading conventions

### 1.1 Method

Version 2 has **two** documentation corpora, and both were used:

| Corpus | Content | Role |
|---|---|---|
| **`RLS_Learn.pdf`** — 212 pages, 510 headings, 7 modules | The vendor's own functional catalogue, in Arabic | **The primary source for V2 functionality** |
| **`real lab system help.chm`** — 15 topics | The in-application manual | **Byte-identical to Version 1** — documents only the common core |
| Executable analysis | 210 objects, 8,026 methods, 270,166 literals, 1,797 SQL, 268 DDL | Mechanism evidence |
| Shipped database | `Patients.mdb` — 103 tables / 1,682 columns | Data evidence |
| Deployment artefacts | Installer, `SETUP.LST`, CABs, PDF stack | Installation evidence |
| Live screen recording | `RLS_Learn.wmv` — 35:20, 106 frames | Behavioural corroboration (Version 1 interface — see `Documentation_Quality_Report.md` §3, correction X-11) |

### 1.2 Confidence

CONFIRMED · STRONGLY SUPPORTED · INFERRED · UNKNOWN · CONFLICTING

### 1.3 Arabic label convention

Original Arabic labels appear in **double guillemets**, reproduced exactly as
transcribed from the vendor guide or the in-application manual. English glosses are
explanatory and never replace the original.

### 1.4 Identifier scheme

| Prefix | Meaning |
|---|---|
| `F-V2-nnn` | Function |
| `WF-V2-nn` | Workflow |
| `RL-V2-FUN-nnn` | Functional requirement |
| `V2-BR-nnn` | Business rule |
| `V2-ENT-nnn` | Data entity |

### 1.5 ★ Scope warning — the CHM does not describe this version

The in-application manual shipped with v1.605 is **byte-identical** to the 2013
manual. It documents **15** capabilities. The vendor's own learning guide documents
**~60**. Any reader relying on the CHM alone would be unaware of **11 documented
capabilities** and of the entire deployment, PDF, branch, statistics, attendance,
work-sheet, messaging and antibiotic features.

**This document uses the vendor guide as the authority for what the product does,
and the executable and database as the authority for how.**

---

## 2. Actor model — V-CD-2019

| Actor | V2 scope | Established by |
|---|---|---|
| **Lab operator / technologist** | Registers patients, enters results, confirms results, prints and hands over reports | Vendor guide 1-1 … 1-13; result forms |
| **Reception / cash clerk** | Collects payment, hands over results, prints the invoice and ID card | `frm_money`, `moneypaid`, `CardFrm`, `CardFrmSetx` |
| **Laboratory administrator** | Test catalogue, reference ranges, prices, users, printers, report layouts, backup, stock-take | Vendor guide 3-1 … 3-11, 6-1 … 9-7 |
| **System administrator** | Install and maintain the workstation, the database server, the network; select the database engine | `RL_SETUP.exe`; CHM `001.htm` |
| **Master-laboratory manager** | Runs the statistics module, the attendance register, the multi-branch stock-take | Vendor guide 4-1 … 4-6, 5-2, 3-11 |
| **Referring physician** | External actor; now **separate from the referring party** | `mowzaf`, `Gehats.ReferredPW`, `Gehats.ReferredID` |
| **Referring party / clinic** | External actor; holds a commission | `Gehats` with `omola` |
| **Send-out laboratory** | External actor receiving referred samples | `otherlabname`, `sarfz` |
| **Patient (remote)** | External actor receiving a result through **NATIGH.COM** | `Natigh`, `SMSSender`, `ReportSMS` |
| **Audit reader** | Reads `loger`, `loggroup`, `patienttest` print and handover audit | Schema |

> **⚠ V2 extends a permission model that Version 1 already had.** The Access-mode
> operator table `patients` carries **twelve** boolean permission bits
> (`grant1`…`grant12`), an administrator flag (`patientad`), an active flag
> (`patientus`) and a role caption (`patientuscap`) — **in both versions**.
> V-CD-2019 adds five further bits (`grant13`…`grant17`) and a role descriptor on
> `useraccounts` (`gtype`, `HDD1`, `HDD2`). **The mapping of any bit to a
> specific capability is UNKNOWN**.

---

## 3. Function index — V-CD-2019

| Function ID | Function | Arabic label | Module | V1 status | Confidence |
|---|---|---|---|---|---|
| F-V2-001 | Start the system and connect to the server | «كيف يتم تشغيل نظام ريال لاب سيستم ؟» | General | inherited | CONFIRMED |
| F-V2-002 | Sign in / sign out | «كيف يتم تسجيل الدخول لنظام ريال لاب سيستم ؟» | System | inherited | CONFIRMED |
| F-V2-003 | Register a new patient | «كيف يتم اضافة بيانات مريض جديد ؟» | Patient | inherited | CONFIRMED |
| F-V2-004 | Generate and print the patient barcode | «كيف يتم طباعة باركود للمريض؟» | Patient | inherited | CONFIRMED |
| F-V2-005 | Search for a patient | «كيف تتم عملية البحث عن مريض فى نظام ريال لاب سيستم ؟» | Patient | inherited | CONFIRMED |
| F-V2-006 | Review a patient's visits and results | «البحث عن مريض معنى وإظهار جميع زياراته للمعمل بتواريخها والتحاليل» | Patient | inherited | CONFIRMED |
| F-V2-007 | Open the profile result-entry window | «كيف يتم الوصول لنوافذ ادخال نتائج التحاليل ؟» | Patient | inherited | CONFIRMED |
| F-V2-008 | Enter a result with automatic classification | «كيف يتم ادخال نتائج التحاليل (تحاليل الكبد وما شابه مثلاً) ؟» | Patient | inherited | CONFIRMED |
| F-V2-009 | Confirm a result as reviewed | — (F8) | Patient | inherited | CONFIRMED |
| F-V2-010 | Hand over results and settle the account | «كيف يتم تسليم نتائج التحاليل للمريض ؟» | Patient | inherited | CONFIRMED |
| F-V2-011 | Add or edit a test definition | «كيف يتم اضافة أو تعديل بيانات تحليل ؟» | System | inherited | CONFIRMED |
| F-V2-012 | Administer normal reference ranges | «كيف يتم اضافة أو تعديل المعدلات الطبيعية لتحليل معين ؟» | System | inherited | CONFIRMED |
| F-V2-013 | Customise report text, dimensions and colours | «كيف يتم التعديل فى نصوص وأبعاد وألوان التقرير ؟» | General | inherited | CONFIRMED |
| F-V2-014 | Manage send-out samples | «كيف يتم ادارة العينات المرسلة للخارج من داخل النظام ؟» | Accounting | inherited | CONFIRMED |
| F-V2-015 | Run accounting and stock-take | «كيف يتم معرفة حساب الدرج وعمل جرد لفترة محددة ؟» | Accounting | inherited | CONFIRMED |
| F-V2-016 | Back up the database | «كيف يتم عمل نسخ احتياطي لقاعدة بيانات النظام ؟» | General | inherited | CONFIRMED |
| F-V2-017 | Manage units and unit conversions | — | System | inherited | STRONGLY SUPPORTED |
| F-V2-018 | Manage abbreviations | — | System | inherited | STRONGLY SUPPORTED |
| F-V2-019 | Manage clinical sub-groups | — | System | inherited | STRONGLY SUPPORTED |
| F-V2-020 | Manage users, administrators, permissions and login groups | «اضافة مستخدمين ومديرين للنظام» · «تعديل بيانات وصلاحيات المستخدمين والمديرين» | System | **extended** | CONFIRMED |
| F-V2-021 | Configure printers, paper, report headers and colours | «ضبط الطابعات المخصصة للطباعة» · «حجم الورق المستخدم في الطباعة» · «تعديل ألوان رأس وذيل التقرير» | General | **extended** | CONFIRMED |
| F-V2-022 | Manage referring parties and treating physicians | «إضافة جهة إحالة» · «إضافة طبيب معالج» | Accounting | **extended** | CONFIRMED |
| F-V2-023 | Attach and view patient pictures | — | Patient | inherited | INFERRED |
| F-V2-024 | Consult method sheets and critical values | — | Patient | inherited | CONFIRMED |
| F-V2-025 | Use the interactive training simulators | — | — | inherited | CONFIRMED |
| F-V2-026 | Print a report to a bound printer | — | General | inherited | CONFIRMED |
| F-V2-027 | Print the patient invoice and ID card | — | General | inherited | STRONGLY SUPPORTED |
| **F-V2-028** | **Produce a PDF report** | — | General | **NEW** | CONFIRMED |
| **F-V2-029** | **Deliver a result through NATIGH.COM** | «تسليم نتائج المرضى عن طريق موقع نتتيجة (NATIGH.COM)» | Patient | **NEW** | CONFIRMED |
| **F-V2-030** | **Send an SMS result notification** | — | Patient | **NEW** | CONFIRMED |
| **F-V2-031** | **Manage branches (master / reception)** | «نظام الفروع للمعامل» | Accounting | **NEW** | CONFIRMED |
| **F-V2-032** | **Run the laboratory statistics module** | «الإحصائيات ومتابعة أداء المعمل» | Server | **NEW** | CONFIRMED |
| **F-V2-033** | **Register attendance and departure** | «تسجيل الحضور والانصراف لمعرفة حركة المستخدمين في الدخول والخروج» | System | **NEW** | CONFIRMED |
| **F-V2-034** | **Produce a work sheet** | «عمل ورقة عمل لمجموعة من المرضى في فترة زمنية محددة» | Work Sheet | **NEW** | CONFIRMED |
| **F-V2-035** | **Produce a work sheet for a test group** | «عمل ورقة عمل لتحليل معين أو مجموعة تحاليل في فترة زمنية محددة» | Work Sheet | **NEW** | CONFIRMED |
| **F-V2-036** | **Report farm results** | «إدخال نتائج تقارير المزارع» | Patient | **NEW** | CONFIRMED |
| **F-V2-037** | **Maintain the antibiotic catalogue** | «إضافة مضاد حي للمزرعة» | System | **NEW** | CONFIRMED |
| **F-V2-038** | **Split and withdraw samples** | «فصل وسحب العينات» | General | **NEW** | CONFIRMED |
| **F-V2-039** | **Produce a consolidated report** | «عمل تقرير مجمع» | Patient | **NEW** | CONFIRMED |
| **F-V2-040** | **Produce an empty report** | «عمل تقرير فارغ» | Patient | **NEW** | CONFIRMED |
| **F-V2-041** | **Maintain and print price lists** | «إضافة قائمة أسعار جديدة» · «طباعة قائمة أسعار التحاليل» | System | **NEW** | CONFIRMED |
| **F-V2-042** | **Set the default account type** | «ضبط نوع الحساب الافتراضي للنظام (أسعار وخصومات وعولات وعينات مسحوبة)» | General | **NEW** | CONFIRMED |
| **F-V2-043** | **Configure the connection receipt (NATIGH header/footer)** | «إعدادات الاتصال (رأس وذيل الاتصال - هوامش الاتصال)» | General | **NEW** | CONFIRMED |
| **F-V2-044** | **Configure the background** | «إعدادات الخلفية» | General | **NEW** | CONFIRMED |
| **F-V2-045** | **Configure the report margins** | «إعدادات هوامش التقرير» | General | **NEW** | CONFIRMED |
| **F-V2-046** | **Configure the report header and footer** | «إعدادات رأس وذيل التقرير» | General | **NEW** | CONFIRMED |
| **F-V2-047** | **Follow up case status and issue results** | «متابعة تسجيل الحالات والتعديل عليها وإدخال النتائج وطباعة التقارير» | Patient | **NEW** | CONFIRMED |
| **F-V2-048** | **Create a NATIGH account for a physician or laboratory** | «إنشاء حساب للطبيب المعالج لاستخدام خدمة نتيجة» | Patient | **NEW** | CONFIRMED |

---

## 4. Detailed function specifications — V2-specific capabilities

> **★ Document design — read this before using §4.**
>
> The **27 inherited functions** (`F-V2-001` … `F-V2-027`) are **not re-specified here**.
> Their full behavioural specification — the 32-field card for each — lives in
> `../RealLab_Version_1/Functional_Specification_V1.md` under the corresponding
> `F-V1-001` … `F-V1-027` identifiers, and **their Version 2 deltas are tabulated in
> §5 below**. This is a deliberate choice: the two versions share the same core
> behaviour, and duplicating 27 cards would create two documents that could drift
> apart. **The mapping is 1:1 and positional** — `F-V2-nnn` ↔ `F-V1-nnn`.
>
> The **21 genuinely new functions** (`F-V2-028` … `F-V2-048`) are specified in full in
> §4, with the complete field set each.

### F-V2-028 — Produce a PDF report

| Field | Content |
|---|---|
| **Function ID** | F-V2-028 |
| **Name** | Produce a PDF report through the Bullzip PDF Printer |
| **Arabic label** | — (no Arabic label; the driver name is a Latin product name) |
| **Domain** | Reporting / PDF export |
| **Purpose** | Produce a patient report as a PDF file, filed into one of four business categories. |
| **Business context** | PDF is how results reach a modern recipient — a portal, a referring physician, an archive. The four output directories mirror the laboratory's commercial categories. |
| **Actor** | Lab operator; reception clerk |
| **Entry point** | Print action (F11 preview / F12 print) with the Bullzip printer selected |
| **Preconditions** | A result is reviewed; `Bullzip PDF Printer` is installed under that exact name; ‹.NET Framework 4.0› is installed; a PDF category is selected. |
| **Trigger** | Print request |
| **Step-by-step workflow** | 1. The operator selects the PDF output category. 2. The application resolves the printer named `Bullzip PDF Printer`. 3. The report is rendered by the `DataReport` engine using the profile's `Data\*.ini` layout. 4. The job is dispatched to the Bullzip port driver `bzpdf.dll`. 5. Bullzip renders the PostScript through its bundled Ghostscript. 6. The PDF is written to `D:\PDF\<Contracts\|Free\|Individual\|LabToLab>\`. 7. The print is audited on `patienttest`. |
| **Inputs** | Patient; test selection; PDF category; output file name |
| **Input validation** | The result must be reviewed — otherwise «Not found any Verified selected tests or result to print it». |
| **System responses** | A PDF file on disk; a print-audit record |
| **Outputs** | PDF file; `resultprint`, `printuser`, `printdate`, `printnum` updated |
| **Resulting state** | The result has been issued in PDF form, into a named business category |
| **Postconditions** | The PDF is deliverable to the patient, the physician or the archive |
| **Business rules** | V2-BR-001 (the PDF device name is a hard-coded literal) · V2-BR-002 (the default output filename is version-pinned to Bullzip 11.1) · V2-BR-003 (PDF output is filed into exactly one of four categories) · V2-BR-004 (PDF output depends on an externally installed printer driver) |
| **Permissions** | Signed-in session |
| **Entities** | `patienttest`; `D:\PDF\*` on the filesystem |
| **Data read** | `patientinfo`, `patienttest`, profile table, `Data\*.ini` |
| **Data created** | The PDF file on disk |
| **Data updated** | `patienttest.resultprint`, `printuser`, `printdate`, `printnum` |
| **Data deleted** | None |
| **Reports / printed output** | The PDF itself |
| **Configuration dependencies** | `SETTING.ini [setup] reportstart`; `Data\*.ini [setupx] reportprinterx`; the Windows printer subsystem |
| **Exceptions** | «Can't find any printer.» if Bullzip is absent or renamed; «Not found any Verified selected tests or result to print it» if nothing reviewed is selected. **No fallback path is evidenced.** |
| **Alternative paths** | Printing to a physical printer, to `Send To OneNote 2007`, or to a PDF category |
| **Related functions** | F-V2-026, F-V2-021, F-V2-029 |
| **Technical dependencies** | Bullzip PDF Printer 11.1.0.2600; its bundled Ghostscript; ‹.NET Framework 4.0› |
| **Evidence** | Literal `Bullzip PDF Printer` @ `0x6A2486` and `Bullzip PDF Printer 11.1 - Create File` @ `0x6A255E`; `D:\PDF\*` literals; `SetupPZ.bat`; `analysis/BATCH-11/pe-envelope.csv` |
| **Confidence** | CONFIRMED |
| **Known limitations** | **The application contains no PDF logic of its own.** The dependency on a specific vendor's driver and a pinned version string is a fragility with no fallback. |

### F-V2-029 — Deliver a result through NATIGH.COM

| Field | Content |
|---|---|
| **Function ID** | F-V2-029 |
| **Name** | Deliver a patient result through the NATIGH.COM web service |
| **Arabic label** | «تسليم نتائج المرضى عن طريق موقع نتتيجة (NATIGH.COM)» · «كيفية فتح الموقع وعرض نتيجة المريض» |
| **Domain** | Result delivery / external service |
| **Purpose** | Publish a patient's result to the NATIGH.COM site so the patient can retrieve it. |
| **Business context** | The laboratory reduces the counter queue and gives the patient remote access to the result — the vendor's own guide makes this capability 1-12, the twelfth patient-module item. |
| **Actor** | Lab operator / reception clerk |
| **Entry point** | The NATIGH facility within the patient module |
| **Preconditions** | Signed in; a NATIGH account exists for the patient or the physician or the laboratory; the result is reviewed. |
| **Trigger** | Operator publishes the result |
| **Step-by-step workflow** | 1. Select the patient and the result. 2. The result is published to NATIGH.COM. 3. The patient opens the site and views the result. 4. A receipt with the result is printed using the configured connection header/footer. |
| **Inputs** | Patient identity; result data; NATIGH account |
| **Input validation** | **The `patienttest.natighu` flag that would record this is ABSENT from the shipped database** — the DDL that adds it was never applied. |
| **System responses** | The result appears on the NATIGH site; a receipt is printable |
| **Outputs** | Published result; printed receipt |
| **Resulting state** | The result is externally published |
| **Postconditions** | — |
| **Business rules** | V2-BR-005 (the NATIGH delivery flag column is defined by the DDL but is absent from the shipped database) · V2-BR-006 (NATIGH delivery requires an outbound internet channel, which the shipped manual advises against) |
| **Permissions** | Signed-in session |
| **Entities** | `Natigh` (installed file), `IDCARD.INI`, `SETTING.ini` |
| **Data read** | `patientinfo`, `patienttest` |
| **Data created** | The external publication |
| **Data updated** | **None recorded in the shipped schema** — the `natighu` flag does not exist |
| **Data deleted** | None |
| **Reports / printed output** | The NATIGH receipt, laid out by `IDCARD.INI` / `SETTING.ini` connection settings |
| **Configuration dependencies** | `IDCARD.INI` connection header/footer; `SETTING.ini` connection settings |
| **Exceptions** | Unknown — the internet dependency and its failure handling are not documented |
| **Alternative paths** | SMS delivery (F-V2-030) |
| **Related functions** | F-V2-030, F-V2-043 |
| **Technical dependencies** | The installed `Natigh` file; outbound HTTPS |
| **Evidence** | Vendor guide 1-12, 1-12a, 1-12b; `Natigh` file in the payload; `IDCARD.INI` connection settings; DDL for `patienttest.natighu` |
| **Confidence** | CONFIRMED for the capability and its documentation; CONFIRMED that the schema flag is missing from the shipped database |
| **Known limitations** | **The documentation is three times more precise than the implementation evidence** — the guide describes opening the site and viewing the result in detail, but the shipped database cannot record that a delivery happened. |

### F-V2-030 — Send an SMS result notification

| Field | Content |
|---|---|
| **Function ID** | F-V2-030 |
| **Name** | Send an SMS result notification |
| **Arabic label** | — |
| **Domain** | Notification / external service |
| **Purpose** | Notify a patient or a physician by SMS that a result is available. |
| **Business context** | Complements NATIGH delivery with a push channel the patient does not have to remember to check. |
| **Actor** | Lab operator |
| **Entry point** | `SMSfrmsenderos` (139 methods) |
| **Preconditions** | Signed in; `smslist` recipients and `smsgroups` defined; a `SMStemplate` exists. |
| **Trigger** | Operator sends the notification |
| **Step-by-step workflow** | 1. Select the recipient from `smslist` or `smsgroups`. 2. Compose from `SMStemplate`. 3. Send — the message is queued in `SMSSender` with `smsMobile`, `SMStext`, `SMSLength` and `requestdate`. 4. `ReportSMS` reports on the sent messages. |
| **Inputs** | Recipient mobile number; message template; patient context |
| **Input validation** | `SMSLength` is stored alongside the text, implying a length constraint is applied — the exact limit is UNKNOWN. |
| **System responses** | A queued message row; a sent-message report |
| **Outputs** | SMS to the recipient; `SMSSender` row |
| **Resulting state** | The notification is queued and auditable |
| **Postconditions** | — |
| **Business rules** | V2-BR-007 (every SMS is queued and auditable, including the recipient number, the text and the timestamp) · V2-BR-006 (the SMS channel requires an outbound internet path) |
| **Permissions** | Signed-in session |
| **Entities** | `SMSSender`, `SMStemplate`, `smslist`, `smsgroups` |
| **Data read** | `smslist`, `smsgroups`, `SMStemplate`, `patientinfo` |
| **Data created** | `SMSSender` row (14 columns) |
| **Data updated** | `smslist` / `smsgroups` state — NOT ESTABLISHED |
| **Data deleted** | None |
| **Reports / printed output** | `ReportSMS` |
| **Configuration dependencies** | SMS gateway credentials — location NOT ESTABLISHED |
| **Exceptions** | Not documented |
| **Alternative paths** | NATIGH delivery (F-V2-029) |
| **Related functions** | F-V2-029, F-V2-048 |
| **Technical dependencies** | An SMS gateway; `SMSfrmsenderos` |
| **Evidence** | DDL for `SMSSender`, `SMStemplate`, `smsgroups`, `smslist`; the `sms*` object family; `SMSfrmsenderos` (139 methods) in `analysis/BATCH-04/vb6/v1605/objects-full.txt` |
| **Confidence** | CONFIRMED for the data structures and the form; INFERRED for the workflow |
| **Known limitations** | The gateway provider, protocol and credentials are not established. **`SMSSender` stores the recipient's mobile number in clear text.** |

### F-V2-031 — Manage branches (master / reception)

| Field | Content |
|---|---|
| **Function ID** | F-V2-031 |
| **Name** | Manage the branch (master / reception) model |
| **Arabic label** | «نظام الفروع للمعامل» · «Allow to connect the lab branches through the internet» (installer string) |
| **Domain** | Deployment / organisation |
| **Purpose** | Operate one laboratory from a master site with reception branches, each able to request and collect work. |
| **Business context** | The vendor's own installer text advertises the ability to connect branches through the internet. This is the commercial differentiator of the 2020 product. |
| **Actor** | Master-laboratory manager; system administrator |
| **Entry point** | `z_branchs` (21 controls); `Labbranch` table |
| **Preconditions** | A branch database seeded from `Branch_Data.mdb` |
| **Trigger** | Branch creation or branch selection |
| **Step-by-step workflow** | 1. Create the branch master record in `Labbranch`. 2. Seed the branch's tables from `Branch_Data.mdb` (49 schema-only tables, 0 rows). 3. Assign the branch code. 4. The branch code occupies **position 9** of the patient identifier. 5. The stock-take can then be run across the master site and its linked branches. |
| **Inputs** | Branch master data; branch code |
| **Input validation** | Not documented |
| **System responses** | Branch record; seeded schema; branch-scoped identifiers |
| **Outputs** | `Labbranch` rows; branch-scoped `patientinfo` |
| **Resulting state** | A branch is operational |
| **Postconditions** | Multi-branch accounting becomes available (F-V2-015) |
| **Business rules** | V2-BR-008 (the branch code occupies position 9 of the patient identifier) · V2-BR-009 (a branch's schema is seeded from a schema-only template with no data) · V2-BR-010 (a stock-take may span a master laboratory and its linked branches) |
| **Permissions** | Administrator rights |
| **Entities** | `Labbranch`, **`patients.branche`** (the operator's branch), `Branch_Data.mdb` |
| **Data read** | `Labbranch`, `patientinfo`, `patienttest` |
| **Data created** | `Labbranch` rows |
| **Data updated** | `patients.branche` (the operator's branch assignment); the branch digit inside `patientid` |
| **Data deleted** | None |
| **Reports / printed output** | Multi-branch stock-take |
| **Configuration dependencies** | `SETTING.ini [setup] labbranchx=1` |
| **Exceptions** | Not documented |
| **Alternative paths** | — |
| **Related functions** | F-V2-015, F-V2-003 |
| **Technical dependencies** | `SUBSTRING(patientid,9,1)`; `Branch_Data.mdb` |
| **Evidence** | Vendor guide 5-2; `Labbranch` in the shipped schema; `Patient.branche`; the installer string; `Branch_Data.mdb` |
| **Confidence** | CONFIRMED |
| **Known limitations** | **The branch digit occupies position 9 of the 13-character `patientid` — the identifier layout is therefore a hard inter-version contract.** The 2019 `Branch_Data.mdb` template is **9 columns narrower** in `patienttest` than the 2013 one, because the application re-adds them at install time. |

### F-V2-032 — Run the laboratory statistics module

| Field | Content |
|---|---|
| **Function ID** | F-V2-032 |
| **Name** | Run the laboratory statistics and performance monitoring module |
| **Arabic label** | «الإحصائيات ومتابعة أداء المعمل» |
| **Domain** | Management reporting |
| **Purpose** | Measure the laboratory's activity and performance over time. |
| **Business context** | A laboratory that cannot report its own volume cannot manage its capacity or defend its revenue. |
| **Actor** | Master-laboratory manager |
| **Entry point** | `statisReport` |
| **Preconditions** | Signed in |
| **Trigger** | Operator selects a statistic and a period |
| **Step-by-step workflow** | The vendor guide gives four worked examples: **4-2** count patients in a given year, sorted by month and by sex; **4-3** count patients in a given month; **4-4** count the request rate of a test group; **4-5** count the number of samples in the year. |
| **Inputs** | Statistic type; period; optional test group |
| **Input validation** | Not documented |
| **System responses** | Counted and grouped figures |
| **Outputs** | Statistical listings |
| **Resulting state** | Read-only |
| **Postconditions** | — |
| **Business rules** | V2-BR-011 (statistics are computed from the transactional tables rather than from a dedicated aggregate store) |
| **Permissions** | Signed-in session |
| **Entities** | `patientinfo` (`requestdate`, `patientsex`), `patienttest` (`subgroupcode`) |
| **Data read** | `patientinfo`, `patienttest` |
| **Data created / updated / deleted** | None established |
| **Reports / printed output** | Statistical listings — formats NOT ESTABLISHED |
| **Configuration dependencies** | — |
| **Exceptions** | — |
| **Alternative paths** | — |
| **Related functions** | F-V2-033, F-V2-015 |
| **Technical dependencies** | `statisReport`; `loger` |
| **Evidence** | Vendor guide 4-1 … 4-5; the `statisReport` object; `loger` table |
| **Confidence** | CONFIRMED |
| **Known limitations** | The statistic definitions are given only as worked examples; the full statistic catalogue is NOT ESTABLISHED. |

### F-V2-033 — Register attendance and departure

| Field | Content |
|---|---|
| **Function ID** | F-V2-033 |
| **Name** | Register operator attendance and departure |
| **Arabic label** | «تسجيل الحضور والانصراف لمعرفة حركة المستخدمين في الدخول والخروج» |
| **Domain** | Workforce / audit |
| **Purpose** | Record when each user signs in and out, so the laboratory can monitor user movement. |
| **Business context** | Given that the stock-take reports **per-user workload and per-user collections** (V1-BR-027), attendance data is the natural complement. |
| **Actor** | Any operator (automatic at sign-in); master-laboratory manager (reads it) |
| **Entry point** | Sign-in / sign-out |
| **Preconditions** | Signed in |
| **Trigger** | Sign-in or sign-out |
| **Step-by-step workflow** | 1. The user signs in. 2. An attendance record is written to `loger`. 3. The user signs out. 4. The departure is recorded. 5. The manager reviews movement in and out. |
| **Inputs** | User; timestamp |
| **Input validation** | Not documented |
| **System responses** | An attendance record |
| **Outputs** | `loger` rows |
| **Resulting state** | The user's presence is recorded |
| **Postconditions** | — |
| **Business rules** | V2-BR-012 (the attendance register is a distinct structure from the login-group table) |
| **Permissions** | — |
| **Entities** | `loger`, `loggroup` |
| **Data read** | `useraccounts` (SQL mode) |
| **Data created** | `loger` rows |
| **Data updated** | — |
| **Data deleted** | None |
| **Reports / printed output** | Attendance listings — NOT ESTABLISHED |
| **Configuration dependencies** | `SETTING.ini [setup] labbranchx` |
| **Exceptions** | — |
| **Alternative paths** | — |
| **Related functions** | F-V2-002, F-V2-032 |
| **Technical dependencies** | `loger` |
| **Evidence** | Vendor guide 3-11; the `loger` table in the shipped schema |
| **Confidence** | CONFIRMED for the capability; UNKNOWN for the schema of `loger` |
| **Known limitations** | **`loger` appears in the shipped schema but its column structure was not extracted in this consolidation pass — the field-level detail is UNKNOWN.** |

### F-V2-034 / F-V2-035 — Work sheets

| Field | Content |
|---|---|
| **Function ID** | F-V2-034 (patient-group work sheet), F-V2-035 (test-group work sheet) |
| **Name** | Produce a work sheet over a patient group or a test group in a period |
| **Arabic label** | «عمل ورقة عمل لمجموعة من المرضى في فترة زمنية محددة» · «عمل ورقة عمل لتحليل معين أو مجموعة تحاليل في فترة زمنية محددة» |
| **Domain** | Batch reporting |
| **Purpose** | Produce a consolidated listing of work performed over a period, for review, audit or referral. |
| **Business context** | Work sheets are the laboratory's substitute for re-querying the transactional database by hand. |
| **Actor** | Lab operator; master-laboratory manager |
| **Entry point** | `workpaper`, `workpaper2`; `wasl` |
| **Preconditions** | Signed in; a period selected |
| **Trigger** | Operator requests a work sheet |
| **Step-by-step workflow** | 1. Select the scope — a patient group, or a test or test group. 2. Select the date period. 3. The work sheet is generated. |
| **Inputs** | Scope; period |
| **Input validation** | Not documented |
| **System responses** | A work sheet |
| **Outputs** | Work sheet listing; `wasl` rows (12 columns) |
| **Resulting state** | Read-only |
| **Postconditions** | — |
| **Business rules** | V2-BR-013 (work-sheet printing is independently toggleable — `SETTING.ini [setup] printworkpaper=1`) |
| **Permissions** | Signed-in session |
| **Entities** | `wasl`, `patientinfo`, `patienttest` |
| **Data read** | `patientinfo`, `patienttest` |
| **Data created** | `wasl` rows |
| **Data updated** | — |
| **Data deleted** | None |
| **Reports / printed output** | The work sheet |
| **Configuration dependencies** | `SETTING.ini [setup] printworkpaper` |
| **Exceptions** | — |
| **Alternative paths** | `workpaper` vs `workpaper2` — the distinction is NOT ESTABLISHED |
| **Related functions** | F-V2-032 |
| **Technical dependencies** | `workpaper`, `workpaper2`, `wasl` |
| **Evidence** | Vendor guide 2-1, 2-2; the object and table families |
| **Confidence** | CONFIRMED for the capability; INFERRED for the mechanism |
| **Known limitations** | The `wasl` column semantics are not established. |

### F-V2-036 — Report farm results

| Field | Content |
|---|---|
| **Function ID** | F-V2-036 |
| **Name** | Enter and report farm (agricultural laboratory) results |
| **Arabic label** | «إدخال نتائج تقارير المزارع» · «إضافة مزرعة جديدة للبرنامج» |
| **Domain** | Veterinary / agricultural laboratory |
| **Purpose** | Support laboratories that serve the agricultural sector, where the "patient" is a farm or an animal and the reporting unit is different. |
| **Business context** | A distinct vertical: the client is a farm, the report follows a different template, and the economics are per-farm rather than per-patient. |
| **Actor** | Lab operator |
| **Entry point** | `sharekat`, `sharekathesab`; `sarfz` |
| **Preconditions** | A farm is registered |
| **Trigger** | Operator enters a farm result |
| **Step-by-step workflow** | 1. Register the farm. 2. Select the farm. 3. Enter the result. 4. Produce the farm report. |
| **Inputs** | Farm; result |
| **Input validation** | Not documented |
| **System responses** | Farm record; result; report |
| **Outputs** | `sharekat` rows; farm report |
| **Resulting state** | — |
| **Postconditions** | — |
| **Business rules** | V2-BR-014 (farm reporting is a parallel reporting vertical that shares the result-entry machinery but not the report template) |
| **Permissions** | Signed-in session |
| **Entities** | `sharekat`, `sharekathesab` |
| **Data read / created / updated / deleted** | `sharekat` — structure not extracted |
| **Reports / printed output** | The farm report |
| **Configuration dependencies** | — |
| **Exceptions** | — |
| **Alternative paths** | — |
| **Related functions** | F-V2-037, F-V2-007, F-V2-008 |
| **Technical dependencies** | `sharekat`, `sharekathesab`, `sarfz` |
| **Evidence** | Vendor guide 1-8, 3-9; the object family |
| **Confidence** | CONFIRMED for the capability; UNKNOWN for the data structures |
| **Known limitations** | The `sharekat` / `sharekathesab` schemas were not extracted in this pass. |

### F-V2-037 — Maintain the antibiotic catalogue

| Field | Content |
|---|---|
| **Function ID** | F-V2-037 |
| **Name** | Maintain the antibiotic catalogue used in microbiology reporting |
| **Arabic label** | «إضافة مضاد حي للمزرعة» |
| **Domain** | Clinical / microbiology |
| **Purpose** | Maintain the antibiotic reference used when reporting culture-and-sensitivity results. |
| **Business context** | Culture-and-sensitivity reporting depends on a maintained antibiotic catalogue; the catalogue is clinical knowledge and must be current. |
| **Actor** | Laboratory administrator |
| **Entry point** | The antibiotic editor |
| **Preconditions** | Administrator rights |
| **Trigger** | Operator opens the antibiotic catalogue |
| **Step-by-step workflow** | 1. Open the catalogue. 2. Add or edit an antibiotic with its properties. 3. Save — the entry becomes selectable in the microbiology result workflow. |
| **Inputs** | Antibiotic name; properties |
| **Input validation** | Not documented |
| **System responses** | Catalogue updated |
| **Outputs** | `antibiotic` rows (38 rows / 25 columns); `antibio` (96 rows / 2 columns) |
| **Resulting state** | The catalogue changes |
| **Postconditions** | — |
| **Business rules** | V2-BR-015 (two antibiotic structures coexist — a wide 25-column catalogue and a narrow 2-column one; their relationship is NOT ESTABLISHED) |
| **Permissions** | Administrator rights |
| **Entities** | `antibiotic`, `antibio`, `cultures`, `cultureanti` |
| **Data read / created / updated / deleted** | `antibiotic`, `antibio` |
| **Reports / printed output** | Indirect — culture-and-sensitivity reports |
| **Configuration dependencies** | — |
| **Exceptions** | — |
| **Alternative paths** | — |
| **Related functions** | F-V2-036, F-V2-038 |
| **Technical dependencies** | — |
| **Evidence** | Vendor guide 3-10; `antibiotic` (38 rows / 25 cols) and `antibio` (96 rows) in the shipped schema |
| **Confidence** | CONFIRMED for the capability and the two tables; UNKNOWN for their relationship |
| **Known limitations** | **What distinguishes the 25-column `antibiotic` from the 2-column `antibio` is UNKNOWN** — this is inherited from the Version 1 investigation as RL-V1-Q-08 and remains open. |

### F-V2-038 — Split and withdraw samples

| Field | Content |
|---|---|
| **Function ID** | F-V2-038 |
| **Name** | Split and withdraw samples |
| **Arabic label** | «فصل وسحب العينات» (icon: Laboratory) |
| **Domain** | Laboratory logistics |
| **Purpose** | Split a sample into aliquots, or withdraw a sample, within the microbiology workflow. |
| **Business context** | Microbiology samples are frequently split — a portion sent for confirmation, a portion retained. |
| **Actor** | Lab operator |
| **Entry point** | The Laboratory icon in the microbiology area |
| **Preconditions** | Signed in |
| **Trigger** | Operator initiates a split or withdrawal |
| **Step-by-step workflow** | 1. Open the Laboratory function. 2. Select the sample. 3. Record the split or withdrawal. |
| **Inputs** | Sample; destination or disposition |
| **Input validation** | Not documented |
| **System responses** | The split or withdrawal is recorded |
| **Outputs** | Updated microbiology records |
| **Resulting state** | Sample disposition recorded |
| **Postconditions** | — |
| **Business rules** | V2-BR-016 (sample disposition is recorded against the microbiology tables) |
| **Permissions** | Signed-in session |
| **Entities** | `cultures`, `cultureanti`, `sample` |
| **Data read / created / updated / deleted** | `cultures`, `cultureanti`; `sample` — structures not fully extracted |
| **Reports / printed output** | Indirect |
| **Configuration dependencies** | — |
| **Exceptions** | — |
| **Alternative paths** | — |
| **Related functions** | F-V2-036, F-V2-037 |
| **Technical dependencies** | — |
| **Evidence** | Vendor guide 6-x «فصل وسحب العينات»; `cultures`, `cultureanti`, `sample` |
| **Confidence** | CONFIRMED for the capability; INFERRED for the mechanism |
| **Known limitations** | The `sample` table structure is not established. |

### F-V2-039 — Produce a consolidated report

| Field | Content |
|---|---|
| **Function ID** | F-V2-039 |
| **Name** | Produce a consolidated report across profiles |
| **Arabic label** | «عمل تقرير مجمع» |
| **Domain** | Reporting |
| **Purpose** | Produce one report spanning several clinical profiles or many results. |
| **Business context** | A consolidated view is what a physician wants — one document, not seven. |
| **Actor** | Lab operator |
| **Entry point** | `Reportanyreport` |
| **Preconditions** | Signed in; results selected |
| **Trigger** | Operator requests a consolidated report |
| **Step-by-step workflow** | 1. Select the results to consolidate. 2. The system assembles a union across the selected result sets. 3. The consolidated report is produced. |
| **Inputs** | Result selection |
| **Input validation** | Not documented |
| **System responses** | The consolidated report |
| **Outputs** | A combined report |
| **Resulting state** | Read-only |
| **Postconditions** | — |
| **Business rules** | V2-BR-017 (cross-profile consolidation is implemented with `SHAPE … APPEND` union queries, which require the `MSDataShape.1` provider — so the capability is tied to the SQL Server path) |
| **Permissions** | Signed-in session |
| **Entities** | `anyreport` (26 columns, 15 rows), multiple profile tables |
| **Data read** | `anyreport`, profile tables |
| **Data created / updated / deleted** | None established |
| **Reports / printed output** | The consolidated report |
| **Configuration dependencies** | `Data\*.ini` layouts of the participating profiles |
| **Exceptions** | Not documented |
| **Alternative paths** | F-V2-040 (empty report) |
| **Related functions** | F-V2-040, F-V2-026 |
| **Technical dependencies** | `MSDataShape.1`; `SHAPE {…} APPEND (…)` |
| **Evidence** | Vendor guide 1-6; `Reportanyreport`; the `SHAPE … APPEND` pattern in the extracted SQL; `MSDataShape.1` in `data_connection1.udl` |
| **Confidence** | CONFIRMED |
| **Known limitations** | **The capability appears to depend on the SQL Server path.** Whether it works against Access Jet 4 is NOT ESTABLISHED. |

### F-V2-040 — Produce an empty report

| Field | Content |
|---|---|
| **Function ID** | F-V2-040 |
| **Name** | Produce an empty report |
| **Arabic label** | «عمل تقرير فارغ» |
| **Domain** | Reporting |
| **Purpose** | Produce a report with no patient data — a blank or template form. |
| **Business context** | Blank forms are needed for manual work, for pre-printed stationery, and for reference copies. |
| **Actor** | Lab operator |
| **Entry point** | `anyreport` |
| **Preconditions** | Signed in |
| **Trigger** | Operator requests an empty report |
| **Step-by-step workflow** | The report template is produced without patient rows. |
| **Inputs** | Template selection |
| **Input validation** | — |
| **System responses** | An empty report |
| **Outputs** | A blank report |
| **Resulting state** | Read-only |
| **Postconditions** | — |
| **Business rules** | — |
| **Permissions** | Signed-in session |
| **Entities** | `anyreport` |
| **Data read** | `anyreport` |
| **Data created / updated / deleted** | None |
| **Reports / printed output** | The blank report |
| **Configuration dependencies** | `Data\*.ini` |
| **Exceptions** | — |
| **Alternative paths** | F-V2-039 |
| **Related functions** | F-V2-039, F-V2-026 |
| **Technical dependencies** | `anyreport` |
| **Evidence** | Vendor guide 1-7; `anyreport` (26 columns, 15 rows) |
| **Confidence** | CONFIRMED |
| **Known limitations** | The 15 `anyreport` rows are report templates, not patient data — their content is not established. |

### F-V2-041 — Maintain and print price lists

| Field | Content |
|---|---|
| **Function ID** | F-V2-041 |
| **Name** | Maintain price lists and print the test price list |
| **Arabic label** | «إضافة قائمة أسعار جديدة» · «طباعة قائمة أسعار التحاليل» |
| **Domain** | Finance / catalogue |
| **Purpose** | Maintain the pricing structures and produce a printed price list for clients. |
| **Business context** | The price list is published to referring clinics — it is a commercial document, not just a configuration. |
| **Actor** | Laboratory administrator |
| **Entry point** | `pricelist`, `pricemenu`, `shopping`; `z_pricelists` (28 controls); report `pricelistsN` |
| **Preconditions** | Administrator rights |
| **Trigger** | Operator opens price-list administration or prints the list |
| **Step-by-step workflow** | 1. Open price-list administration. 2. Define or modify a price list. 3. Print the price list through the dedicated report. |
| **Inputs** | Price list definition; prices |
| **Input validation** | Not documented |
| **System responses** | Price list saved; list printed |
| **Outputs** | `pricelist` rows; the printed price list |
| **Resulting state** | The price list changes |
| **Postconditions** | Test prices used in billing may reference it |
| **Business rules** | V2-BR-018 (the price list is a distinct artefact from the per-test price fields on `testtable`) |
| **Permissions** | Administrator rights |
| **Entities** | `pricelist`, `pricemenu`, `shopping`, `testtable` (`patientprice`, `labprice`, `otherlabprice`) |
| **Data read / created / updated / deleted** | `pricelist`, `pricemenu` — structures not fully extracted |
| **Reports / printed output** | `pricelistsN` — the printed price list |
| **Configuration dependencies** | `Data\*.ini` for the price-list layout |
| **Exceptions** | — |
| **Alternative paths** | — |
| **Related functions** | F-V2-011, F-V2-015 |
| **Technical dependencies** | `z_pricelists`, `pricelistsN` |
| **Evidence** | Vendor guide 3-4, 3-5; `pricelist`, `pricemenu`, `shopping` in the shipped schema; the `pricelistsN` report object |
| **Confidence** | CONFIRMED for the capability |
| **Known limitations** | The price-list structures were not extracted at field level in this pass. |

### F-V2-042 — Set the default account type

| Field | Content |
|---|---|
| **Function ID** | F-V2-042 |
| **Name** | Set the default account type for the system |
| **Arabic label** | «ضبط نوع الحساب الافتراضي للنظام (أسعار وخصومات وعولات وعينات مسحوبة)» |
| **Domain** | Finance / configuration |
| **Purpose** | Choose the accounting method the system applies by default — prices, discounts, commissions and withdrawn samples. |
| **Business context** | The default account type governs how every patient invoice is priced, so it is the single most consequential financial setting. |
| **Actor** | Laboratory administrator |
| **Entry point** | The accounting-method setting |
| **Preconditions** | Administrator rights |
| **Trigger** | Configuration change |
| **Step-by-step workflow** | 1. Open the setting. 2. Select the default account type. 3. Save. |
| **Inputs** | Account type. **The `pricemethod` value domain is NOT ESTABLISHED** — the three values previously cited (`Lab To Lab`, `Individual`, `Free`) have **zero occurrences** across the 1,337 extracted SQL literals, and `patientinfo` has 0 rows, so no value set can be read. The **four PDF output categories** (`Contracts`, `Free`, `Individual`, `LabToLab`) are a separate, evidenced taxonomy |
| **Input validation** | Not documented |
| **System responses** | The default is stored and applied. **★ Note the correspondence with the four PDF output categories** — the invoice taxonomy and the PDF destination taxonomy align, but only the PDF one is evidenced by literal |
| **Outputs** | Updated configuration |
| **Resulting state** | New invoices price by the new default |
| **Postconditions** | — |
| **Business rules** | V2-BR-019 (the account type is a single system-wide default; **the `pricemethod` value domain is NOT ESTABLISHED**, and the correspondence with the four evidenced PDF output categories is an observation, not a verified equivalence) |
| **Permissions** | Administrator rights |
| **Entities** | `patientinfo.pricemethod`, `Gehats.pricemethod`, `pricemethod` configuration |
| **Data read / created / updated / deleted** | Configuration; `patientinfo.pricemethod` |
| **Reports / printed output** | Indirect — the invoice and the PDF category |
| **Configuration dependencies** | `SETTING.ini` / `BASE.ini` |
| **Exceptions** | — |
| **Alternative paths** | A per-patient `pricemethod` and a per-party `Gehats.pricemethod` override the default |
| **Related functions** | F-V2-028, F-V2-042, F-V2-015 |
| **Technical dependencies** | — |
| **Evidence** | Vendor guide 7-7; `patientinfo.pricemethod` Text(10); `Gehats.pricemethod` Text(20); the four `D:\PDF\` categories |
| **Confidence** | CONFIRMED |
| **Known limitations** | The exact override precedence between the system default, the per-party method and the per-patient method is NOT ESTABLISHED. |

### F-V2-043 — Configure the connection receipt

| Field | Content |
|---|---|
| **Function ID** | F-V2-043 |
| **Name** | Configure the connection receipt header, footer and margins |
| **Arabic label** | «إعدادات الاتصال (رأس وذيل الاتصال - هوامش الاتصال)» |
| **Domain** | Reporting / configuration |
| **Purpose** | Lay out the printed receipt that accompanies a result delivered through NATIGH.COM. |
| **Business context** | The receipt is the physical evidence that a result was published; it is what the patient keeps. |
| **Actor** | Laboratory administrator |
| **Entry point** | `IDCARD.INI` / `SETTING.ini` connection settings |
| **Preconditions** | Administrator rights |
| **Trigger** | Configuration change |
| **Step-by-step workflow** | 1. Open the connection settings. 2. Set the receipt header, footer and margins. 3. Save. |
| **Inputs** | Header/footer text; margin values |
| **Input validation** | Not documented |
| **System responses** | The configuration is stored |
| **Outputs** | Updated `IDCARD.INI` / `SETTING.ini` |
| **Resulting state** | The receipt layout changes |
| **Postconditions** | — |
| **Business rules** | — |
| **Permissions** | Administrator rights |
| **Entities** | `IDCARD.INI`, `SETTING.ini` |
| **Data read / created / updated / deleted** | Configuration files |
| **Reports / printed output** | The connection receipt |
| **Configuration dependencies** | NATIGH account configuration |
| **Exceptions** | — |
| **Alternative paths** | — |
| **Related functions** | F-V2-029 |
| **Technical dependencies** | — |
| **Evidence** | Vendor guide 8-7 |
| **Confidence** | CONFIRMED |
| **Known limitations** | The individual key names for the connection settings are not established. |

### F-V2-044 — Configure the background

| Field | Content |
|---|---|
| **Function ID** | F-V2-044 |
| **Name** | Configure the application background |
| **Arabic label** | «إعدادات الخلفية» |
| **Domain** | Configuration |
| **Purpose** | Set the background colour of the application interface. |
| **Business context** | Cosmetic, but documented as a formal capability by the vendor. |
| **Actor** | Laboratory administrator |
| **Entry point** | The background setting |
| **Preconditions** | — |
| **Trigger** | Configuration change |
| **Step-by-step workflow** | 1. Open the setting. 2. Select the background colour. 3. Save. |
| **Inputs** | Colour |
| **Input validation** | — |
| **System responses** | The background changes |
| **Outputs** | Updated `Data\*.ini` (`backcolor`) |
| **Resulting state** | — |
| **Postconditions** | — |
| **Business rules** | — |
| **Permissions** | — |
| **Entities** | `Data\*.ini` |
| **Data read / created / updated / deleted** | `Data\*.ini` |
| **Reports / printed output** | None |
| **Configuration dependencies** | — |
| **Exceptions** | — |
| **Alternative paths** | — |
| **Related functions** | F-V2-021 |
| **Technical dependencies** | — |
| **Evidence** | Vendor guide 9-7; `Data\*.ini` `backcolor` key |
| **Confidence** | CONFIRMED |
| **Known limitations** | — |

### F-V2-045 — Configure report margins

| Field | Content |
|---|---|
| **Function ID** | F-V2-045 |
| **Name** | Configure the report margins |
| **Arabic label** | «إعدادات هوامش التقرير» |
| **Domain** | Reporting |
| **Purpose** | Set the printable area of the report. |
| **Business context** | Printer-specific margins must match the physical device or content is lost. |
| **Actor** | Laboratory administrator |
| **Entry point** | `Data\*.ini` margin keys |
| **Preconditions** | Administrator rights |
| **Trigger** | Configuration change |
| **Step-by-step workflow** | 1. Open report setup. 2. Set `leftvalue`, `rightvalue`, `topvalue`, `bottomvalue`. 3. Save. |
| **Inputs** | Four margin values |
| **Input validation** | Not documented |
| **System responses** | Margins applied to the layout |
| **Outputs** | Updated `Data\*.ini` |
| **Resulting state** | The report's printable area changes |
| **Postconditions** | — |
| **Business rules** | V2-BR-020 (margins are stored per profile, while paper size is global) |
| **Permissions** | Administrator rights |
| **Entities** | `Data\*.ini` |
| **Data read / created / updated / deleted** | `Data\*.ini` |
| **Reports / printed output** | All prints of that profile |
| **Configuration dependencies** | `SETTING.ini [setup] papersize` |
| **Exceptions** | — |
| **Alternative paths** | — |
| **Related functions** | F-V2-013, F-V2-046, F-V2-021 |
| **Technical dependencies** | VB6 `Printer` object |
| **Evidence** | Vendor guide 6-3; `Data\*.ini` margin keys |
| **Confidence** | CONFIRMED |
| **Known limitations** | The unit of the margin values is not established. |

### F-V2-046 — Configure the report header and footer

| Field | Content |
|---|---|
| **Function ID** | F-V2-046 |
| **Name** | Configure the report header and footer |
| **Arabic label** | «إعدادات رأس وذيل التقرير» |
| **Domain** | Reporting |
| **Purpose** | Set the text and layout of the report's header and footer regions. |
| **Business context** | The header carries the laboratory's identity and the report's context — it is what makes a printed result usable as a clinical document. |
| **Actor** | Laboratory administrator |
| **Entry point** | `Data\*.ini` header/footer keys |
| **Preconditions** | Administrator rights |
| **Trigger** | Configuration change |
| **Step-by-step workflow** | 1. Open report setup. 2. Set `formsubtitle`, `reportmaintitle`. 3. Save. |
| **Inputs** | Header and footer texts |
| **Input validation** | — |
| **System responses** | The header/footer changes |
| **Outputs** | Updated `Data\*.ini` |
| **Resulting state** | — |
| **Postconditions** | — |
| **Business rules** | V2-BR-021 (the global header texts `Textr1txt=المنظومة البرمجية` and `Textr2txt=ريال لاب سيستم` coexist with per-profile header texts) |
| **Permissions** | Administrator rights |
| **Entities** | `Data\*.ini`, `SETTING.ini` |
| **Data read / created / updated / deleted** | `Data\*.ini`, `SETTING.ini` |
| **Reports / printed output** | All prints |
| **Configuration dependencies** | `SETTING.ini` global header texts |
| **Exceptions** | — |
| **Alternative paths** | — |
| **Related functions** | F-V2-013, F-V2-045 |
| **Technical dependencies** | — |
| **Evidence** | Vendor guide 6-5; `Data\*.ini` `formsubtitle` / `reportmaintitle`; `SETTING.ini` `Textr1txt` / `Textr2txt` |
| **Confidence** | CONFIRMED |
| **Known limitations** | The precedence between global and per-profile header text is NOT ESTABLISHED. |

### F-V2-047 — Follow up case status

| Field | Content |
|---|---|
| **Function ID** | F-V2-047 |
| **Name** | Follow up case status, amend, enter results and print reports |
| **Arabic label** | «متابعة تسجيل الحالات والتعديل عليها وإدخال النتائج وطباعة التقارير» |
| **Domain** | Case management |
| **Purpose** | Track a case through its lifecycle — registration, amendment, result entry and reporting — as a unit. |
| **Business context** | A "case" is a longitudinal episode rather than a single visit; it is the unit a clinician thinks in. |
| **Actor** | Lab operator; laboratory administrator |
| **Entry point** | `patienttest.resultreview` / `resultprint` workflow; `patienthistory` |
| **Preconditions** | Signed in; patient selected |
| **Trigger** | Operator opens the case record |
| **Step-by-step workflow** | 1. Open the case. 2. Review the registration and amend it. 3. Enter results. 4. Print the reports. |
| **Inputs** | Case identifier |
| **Input validation** | Not documented |
| **System responses** | The case record and its reports |
| **Outputs** | Updated `patienttest` rows; printed reports |
| **Resulting state** | The case advances |
| **Postconditions** | — |
| **Business rules** | — |
| **Permissions** | Signed-in session |
| **Entities** | `patienttest`, `patientinfo`, `history` |
| **Data read | All of the above |
| **Data created | `patienttest` rows |
| **Data updated** | `patienttest` result and status fields |
| **Data deleted** | None |
| **Reports / printed output** | The case reports |
| **Configuration dependencies** | — |
| **Exceptions** | — |
| **Alternative paths** | — |
| **Related functions** | F-V2-006, F-V2-008, F-V2-026 |
| **Technical dependencies** | `patienthistory`, `patienthistory2` |
| **Evidence** | Vendor guide 1-11; `patienttest.resultreview` / `resultprint` |
| **Confidence** | STRONGLY SUPPORTED |
| **Known limitations** | The "case" entity is not a separate table — the concept is realised through the patient's test rows. |

### F-V2-048 — Create a NATIGH account

| Field | Content |
|---|---|
| **Function ID** | F-V2-048 |
| **Name** | Create a NATIGH service account for a physician or a laboratory |
| **Arabic label** | «إنشاء حساب للطبيب المعالج لاستخدام خدمة نتيجة» · «إنشاء حساب للمعمل لاستخدام خدمة نتيجة» |
| **Domain** | External service administration |
| **Purpose** | Provision an account that lets a physician or a laboratory access the NATIGH result service on a patient's behalf. |
| **Business context** | This turns the laboratory into a **service provider** to other laboratories and physicians — an architectural business change, not a feature. |
| **Actor** | Laboratory administrator |
| **Entry point** | The NATIGH account administration screen |
| **Preconditions** | NATIGH account configuration available |
| **Trigger** | Administrator creates an account |
| **Step-by-step workflow** | 1. Open the NATIGH account administration. 2. Choose the account type — physician or laboratory. 3. Enter the account details. 4. Save. 5. Configure the connecting receipt for that account. |
| **Inputs** | Account type; account holder details |
| **Input validation** | Not documented |
| **System responses** | The account is created |
| **Outputs** | Account record |
| **Resulting state** | The holder can use the result service |
| **Postconditions** | — |
| **Business rules** | V2-BR-022 (NATIGH accounts are provisioned for external parties — physicians and laboratories — which extends the system's trust boundary beyond the laboratory itself) |
| **Permissions** | Administrator rights |
| **Entities** | `SMSSender`, `smslist`, `smsgroups`; `Natigh` configuration |
| **Data read / created / updated / deleted** | `smslist` / `smsgroups` — structure not fully extracted |
| **Reports / printed output** | The connection receipt |
| **Configuration dependencies** | `IDCARD.INI` connection settings |
| **Exceptions** | — |
| **Alternative paths** | — |
| **Related functions** | F-V2-029, F-V2-030, F-V2-043 |
| **Technical dependencies** | The NATIGH service |
| **Evidence** | Vendor guide — outside the numbered index, «إنشاء حساب للطبيب المعالج لاستخدام خدمة نتيجة» and «إنشاء حساب للمعمل لاستخدام خدمة نتيجة» |
| **Confidence** | CONFIRMED for the capability; INFERRED for the mechanism |
| **Known limitations** | The account store is not established — it may live outside the Access database entirely, in the `Natigh` installed file. |

---

## 5. V2 deltas to the inherited functions

| Function | V2 delta | Evidence |
|---|---|---|
| F-V2-001 Start | The installer now selects the **database engine**; the `Databasetype=XMSACCESSX` toggle encoding is the multi-engine selector | `RL_SETUP.exe`, `BASE.ini` |
| F-V2-002 Sign in | Adds a **permission-bit model** (`grant13`…`grant17`) and account descriptors (`gtype`, `HDD1`, `HDD2`); adds the **attendance register** `loger` | DDL, vendor guide 3-11 |
| F-V2-003 Register patient | Adds the **branch** field; the identifier's position 9 now carries the branch code | `Patient.branche`, `SUBSTRING(patientid,9,1)` |
| F-V2-006 History | Extended to show **all the patient's visits with dates and tests** explicitly | Vendor guide 1-10 |
| F-V2-007/008 Result entry | Adds the **IgE allergy** pattern (`Item`/`RESULT`/`Class`/`TypeF`), and the H. pylori, PCR, LBC, FMF and automated-urine profiles | DDL, `resultIgeAllergy` (231 methods), `resulthpylori` (83), `ResultPCR` (37), `resultLBC` (33), `ResultFMF` (48), `ResulturineAuto` (184) |
| F-V2-009 Confirm | The review flag is **explicit** — `patienttest.resultreview` | Schema |
| F-V2-010 Hand over | Adds **digital delivery** as an alternative to physical hand-over | Vendor guide 1-3, 1-12 |
| F-V2-011 Test definition | Adds `branch`, `normaltype`, `seereport`, `addorno`/`testorno`, `REFRANGE` | Schema |
| F-V2-012 Reference ranges | **Unchanged** — the same 852 rows | Table comparison |
| F-V2-013 Report customisation | Adds margins, header/footer text, 20 colour keys, background | Vendor guide 6-3, 6-5, 6-6, 9-7 |
| F-V2-014 Send-out | Adds the `waslprinter` role | Literal census |
| F-V2-015 Accounting | Adds the **multi-branch** stock-take and the **treating-physician / referring-party** split | Vendor guide 5-2, 5-3 |
| F-V2-020 Users | Adds the permission model and `useraccounts.gtype/hdd1/hdd2` | DDL, vendor guide |
| F-V2-021 Printers | **Six** roles instead of three; `EnumPrintersA` / `SetDefaultPrinterA` instead of the older spooler quartet; **five** selection comboboxes instead of three | Literal census |
| F-V2-022 Referring parties | Splits into **party** (`Gehats`, with `omola` commission) and **physician** (`mowzaf`, `ReferredPW`, `ReferredID`) | Schema, vendor guide |
| F-V2-024 Method sheets | **Unchanged** — 29 sheets byte-identical | Artefact comparison |
| F-V2-025 Training | **Unchanged** — 13 simulators byte-identical | Artefact comparison |
| F-V2-026 Print | Adds the **PDF path** and the four output categories | Literal analysis |
| F-V2-027 Invoice / card | Adds the `CardFrm` / `CardFrmSetx` ID card and the connection receipt | Object list, vendor guide 8-7 |

---

## 6. Workflow specifications — V2

### WF-V2-01 — Patient episode with digital delivery

```
1. Sign in                → loggroup + attendance loger
2. Register patient       → patientinfo (incl. branch)
3. Barcode                → barcodes
4. Order tests            → patienttest (testcode, subgroupcode)
5. Enter result           → profile table + patienttest
6. Confirm (F8)           → patienttest.resultreview
7. Print                  → physical printer | Send To OneNote | Bullzip → D:\PDF\<category>\
8. Hand over              → payment + patienttest.give
9. [Alternative] Deliver → NATIGH.COM publication  /  SMS via SMSSender
```

### WF-V2-02 — Branch operation

```
Labbranch master record
   └─► Branch_Data.mdb (49 schema-only tables, 0 rows) seeds the branch schema
        └─► branch code assigned
             └─► SUBSTRING(patientid,9,1) extracts it on every patient record
                  └─► the stock-take may then span master + linked branches
```

### WF-V2-03 — PDF production

```
reviewed result
   └─► print request → category selected (Contracts | Free | Individual | LabToLab)
        └─► printer "Bullzip PDF Printer" (hard-coded literal @0x6A2486)
             └─► Bullzip port driver bzpdf.dll
                  └─► bundled Ghostscript renders
                       └─► PDF at D:\PDF\<category>\<name>
                            └─► audit: printuser / printdate / printnum
```

### WF-V2-04 — Runtime schema migration

```
application start
   └─► 268 embedded DDL statements executed against the live database
        ├─ 12 CREATE TABLE  (SQL Server form, then Access form)
        ├─ 228 ALTER TABLE ADD
        └─ 14 ALTER COLUMN (type changes)
             ├─ against Jet 4  → SQL-Server-only statements fail silently
             └─ against SQL Server → applied in full
```

### WF-V2-05 — Statistics

```
statisReport
   ├─ patient count by year, sorted by month and by sex  → patientinfo
   ├─ patient count by month                              → patientinfo
   ├─ request rate for a test group                        → patienttest.subgroupcode
   └─ sample count for the year                            → patienttest
```

### WF-V2-06 — Work sheet

```
workpaper / workpaper2 / wasl
   ├─ scope = a patient group  + date period
   └─ scope = a test or test group + date period
```

### WF-V2-07 — NATIGH / SMS delivery

```
SMSSender  ── queued message: patientid, PatDrName, requestdate,
              smsMobile, SMSLength, SMStext
SMStemplate ── templatenum, smstext
smsgroups   ── ID, grouptype, smsgroup
smslist     ── ID, smstype, smsphone, smsname, smsgroup
ReportSMS   ── sent-message report
```

---

## 7. Functions explicitly NOT established for V-CD-2019

Recorded so that their absence is explicit rather than assumed.

| Capability | Status | Basis |
|---|---|---|
| Foreign-key enforcement | **ABSENT** | Jet 4 does not enforce; no constraint objects exist |
| Password hashing / expiry / lockout | **NOT ESTABLISHED** | No evidence in either version |
| Failed-login auditing | **NOT ESTABLISHED** | `loger` exists but its schema was not extracted |
| Encryption at rest | **NOT ESTABLISHED** | No evidence of encryption |
| Automatic unit conversion on entry | **NOT ESTABLISHED** | Unchanged from V1 |
| Overlap validation of reference ranges | **NOT ESTABLISHED** | Unchanged from V1 |
| Duplicate-patient detection | **NOT ESTABLISHED** | Unchanged from V1 |
| A second signature for critical results | **NOT ESTABLISHED** | No mechanism evidenced |
| Deleting a patient's data | **NOT ESTABLISHED** | No erasure path evidenced in either version |
| Consent / data-subject rights handling | **NOT ESTABLISHED** | No mechanism evidenced |
| Backup scheduling | **NOT ESTABLISHED** | The shipped backup is stale but nothing establishes the mechanism |

---

**End of Functional_Specification_V2.md**
