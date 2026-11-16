# Business Rules — RealLab V-ROOT-1.00

**Document ID:** RL-V1-BR-001
**Version documented:** `V-ROOT-1.00`
**Evidence basis:** 364 verified artefacts · static analysis only
**Companion documents:** `Functional_Specification_V1.md`, `Data_Model_and_Workflow_Entities_V1.md`

---

## 1. How to read this catalogue

### 1.1 Identifier scheme

`V1-BR-nnn` — stable, version-scoped, never re-used. A rule that applies to both
versions carries **two** identifiers (`V1-BR-nnn` and `V2-BR-nnn`); the applicability
column records the shared scope explicitly.

### 1.2 Evidence-type classification

| Code | Meaning |
|---|---|
| **ET-DOC** | Stated in the shipped Arabic user manual (`help.chm`) |
| **ET-DOC+DB** | Stated in the manual **and** present verbatim as stored data |
| **ET-DB** | Present as stored data in the extracted schema |
| **ET-CODE** | Present in the executable's literals, SQL or object model |
| **ET-CFG** | Present in a shipped configuration file |
| **ET-PRES** | Present in the presentation library (RTF sheets, Flash simulators) |
| **ET-DERIVED** | Derived by cross-comparing two evidence classes |

### 1.3 Confidence

CONFIRMED · STRONGLY SUPPORTED · INFERRED · UNKNOWN · CONFLICTING

### 1.4 Classification of rule status

| Status | Meaning |
|---|---|
| **CONFIRMED PRESENT** | Established for V-ROOT-1.00 by evidence. |
| **CONFIRMED ABSENT** | Established by exhaustive negative search of the Version 1 artefacts. |
| **VERSION-SPECIFIC** | Established for one version and demonstrated to differ from the other. |
| **NOT ESTABLISHED** | Evidence insufficient; no conclusion drawn. |
| **CONFLICTING** | Sources disagree; both are preserved. |

> **Absence of evidence is not evidence of absence.** Every **CONFIRMED ABSENT** entry
> below names the search that established it.

---

## 2. Rule index

| ID | Rule name | Domain | Evidence type | Confidence | Status |
|---|---|---|---|---|---|
| V1-BR-001 | Server address is the only mutable server setting | Deployment | ET-DOC | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-002 | Logon-permission type, database identity and database name are immutable | Deployment / Security | ET-DOC | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-003 | First run uses the `admin` account with password equal to the user name | Security | ET-DOC+CODE | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-004 | The session is scoped to a branch | Deployment | ET-CFG | STRONGLY SUPPORTED | CONFIRMED PRESENT |
| V1-BR-005 | The short patient identifier is positionally derived from `patientid` | Patient | ET-CODE | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-006 | The patient barcode is the direct patient identifier carried on a physical artefact | Patient / Security | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-007 | `history` stores 20 free-form columns as generic attribute slots | Patient / Data | ET-DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-008 | Result entry is scoped to a clinical profile's analyte set | Results | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-009 | Result classification is driven by `normalrange` limits, not hard-coded values | Results | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-010 | A critical limit overrides the abnormal classification | Results | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-011 | Reference ranges are banded by sex and age | Clinical knowledge | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-012 | The abnormality exception text is stored on the test row | Results | ET-DB | STRONGLY SUPPORTED | CONFIRMED PRESENT |
| V1-BR-013 | A result comment may be sourced from three distinct places | Results | ET-DOC | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-014 | Only reviewed results are eligible for hand-over | Results / Finance | ET-DOC | STRONGLY SUPPORTED | CONFIRMED PRESENT |
| V1-BR-015 | The patient account total derives from `totalprice` and `totalpricedisc` | Finance | ET-DB | STRONGLY SUPPORTED | CONFIRMED PRESENT |
| V1-BR-016 | Test number 4 — the urine analysis — is immutable in both number and name | Catalogue | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-017 | A test flagged `inotherlab` is enrolled automatically into the send-out workflow | Send-out | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-018 | Exception text and critical comment are stored per range row | Clinical knowledge | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-019 | The urine analysis's reference range is bound to the immutable test number 4 | Clinical knowledge | ET-DOC | STRONGLY SUPPORTED | CONFIRMED PRESENT |
| V1-BR-020 | The report colour palette is a 19-entry table stored in a text file named `REPORT.dll` | Reporting | ET-CFG+DERIVED | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-021 | Each report profile binds its own printer | Reporting | ET-CFG | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-022 | Paper size is a global setting, not per profile | Reporting | ET-CFG | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-023 | «دائماً إليه» makes the destination the standing default for that test | Send-out | ET-DOC | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-024 | Send-out settlement carries two independent states: sent, and settled | Send-out | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-025 | The collection scope for an accounting period is an explicit operator decision | Finance | ET-DOC | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-026 | Referring-party commission is stored on the referring-party record | Finance | ET-DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-027 | Per-user workload is a first-class accounting output | Finance | ET-DOC | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-028 | Database backup is copy-based, not a transactionally consistent snapshot | Administration | ET-DERIVED | INFERRED | CONFIRMED PRESENT |
| V1-BR-029 | Unit conversion factors are stored per analyte-and-unit-pair, not globally | Clinical knowledge | ET-DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-030 | Abbreviations are maintained independently of the test catalogue | Clinical knowledge | ET-DERIVED | STRONGLY SUPPORTED | CONFIRMED PRESENT |
| V1-BR-031 | The clinical sub-group is the routing key for both the entry window and the report layout | Catalogue | ET-DERIVED | INFERRED | CONFIRMED PRESENT |
| V1-BR-032 | The login group is the unit of session scoping and workload attribution, not a role hierarchy | Security | ET-DERIVED | INFERRED | CONFIRMED PRESENT |
| V1-BR-033 | The report sequence origin is the absolute value `reportstart = 1984` | Reporting | ET-CFG | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-034 | Barcode auto-creation and work-sheet printing are independently toggleable | Reporting / Patient | ET-CFG | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-035 | Critical thresholds are stated in the method sheet, separately from `normalrange` | Clinical knowledge | ET-PRES+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-036 | The training library is a separate executable set that does not touch the live database | Training | ET-DERIVED | STRONGLY SUPPORTED | CONFIRMED PRESENT |
| V1-BR-037 | Haematocrit is derived as `Hct = Hgb × 3.3` | Clinical | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-038 | Haemoglobin percentage is derived from `Hgb` and an age/sex factor | Clinical | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-039 | The ISI is computed as `PT control / patient PT` with a control time of 12 s (secondary 35 s) | Clinical | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-040 | The Westgard `2CVJW` rejection rule is applied to coagulation control values | Clinical | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-041 | Per-analyte critical-value thresholds are stated in the method sheets | Clinical | ET-PRES | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-042 | The database password is stored in clear text in shipped configuration | Security | ET-CFG | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-043 | The application connects to the database as the administrative account `sa` | Security | ET-CFG | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-044 | The application refuses to proceed when the Windows regional date format is not English | Deployment | ET-DOC | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-045 | The database server must be running before any client can sign in | Deployment | ET-DOC | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-046 | PDF export capability is not present in this version | Reporting | ET-DERIVED | CONFIRMED | **CONFIRMED ABSENT** |
| V1-BR-047 | Each operator carries twelve function-level permission bits (`grant1`…`grant12`) | Security | ET-DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-048 | Operator attendance is recorded on the operator record itself | Security / Workforce | ET-DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-049 | Referring-party commission is applied as a percentage of the patient-level commission base | Finance | ET-CODE | CONFIRMED | CONFIRMED PRESENT |

---

## 3. Detailed rule specifications

### 3.1 Deployment and environment rules

#### V1-BR-001 — Server address is the only mutable server setting

| Field | Content |
|---|---|
| **Rule name** | Server address is the only mutable server setting |
| **Arabic label** | «النقطة الأولى: رقم الأي بي الخاص بالسيرفر أو اسم جهاز السيرفر» (from the server-settings window description) |
| **Domain** | Deployment |
| **Description** | Of the four items in the server-settings window, only the server's IP address or machine name may be changed by the operator. |
| **Business purpose** | Prevent an operator from breaking a working multi-site deployment by altering the shared database identity. |
| **Trigger / applicable event** | Configuration of a client workstation |
| **Preconditions** | The application is installed and the database server exists |
| **Actors and permissions** | System administrator |
| **Expected behaviour** | The machine name is preferred over the IP address so that the installation survives a NIC or cable failure: «ويفضل على جهاز السيرفر استخدام اسم الجهاز» |
| **Restrictions** | See V1-BR-002 for the three immutable items |
| **Validation conditions** | Not documented |
| **Exceptions** | Not documented |
| **Consequences** | A wrong address produces «خادم قواعد البيانات غير معرف» and the application closes |
| **Related workflows** | WF-V1-01 |
| **Related functions** | F-V1-001 |
| **Related entities** | `BASE.ini` (`DataSource`) |
| **Relevant fields** | `DataSource` |
| **Related statuses** | — |
| **Evidence** | CHM topic `001.htm`, server-settings section, verbatim |
| **Evidence type** | ET-DOC |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00. In V-CD-2019 the same manual text applies (the CHM is byte-identical), but the deployment model is extended — see the V2 catalogue. |
| **Known limitations** | The client's fallback behaviour when the address is unreachable is not documented beyond the message. |

#### V1-BR-002 — Logon-permission type, database identity and database name are immutable

| Field | Content |
|---|---|
| **Rule name** | Logon-permission type, database identity and database name are immutable |
| **Arabic label** | «نوع سماحية الدخول للنظام وهى ثابتة دائما فى حالة استخدام برنامج التثبيت الخاص بنا» · «اسم وكلمة مرور المستخدم ويمنع تغيرها» · «اسم قاعدة البيانات ويمنع تغيرها» |
| **Domain** | Deployment / Security |
| **Description** | Three of the four server-settings items are documented as unchangeable by the operator. |
| **Business purpose** | The database identity and permission type are part of the vendor's installed configuration; changing them voids the supported configuration. |
| **Trigger / applicable event** | Installation by the vendor installer |
| **Preconditions** | Vendor installer used |
| **Actors and permissions** | Vendor installer; operator thereafter has no control |
| **Expected behaviour** | The values remain as installed |
| **Restrictions** | The manual states «ويمنع تغيرها» for items 3 and 4 and «ثابتة دائما» for item 2 |
| **Validation conditions** | Not documented |
| **Exceptions** | Not documented |
| **Consequences** | **Security consequence:** a single shared database credential is used by every client workstation and can only be changed by editing the shipped configuration file. See V1-BR-042, V1-BR-043. |
| **Related workflows** | WF-V1-01 |
| **Related functions** | F-V1-001 |
| **Related entities** | `useraccounts`, `loggroup`; `BASE.ini` |
| **Relevant fields** | `UserID`, `Password`, `InitialCatalog` |
| **Related statuses** | — |
| **Evidence** | CHM topic `001.htm`, verbatim |
| **Evidence type** | ET-DOC |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00; the CHM is byte-identical in V-CD-2019 |
| **Known limitations** | The manual does not describe who — if anyone — may change these values outside the application. |

#### V1-BR-044 — The application refuses to proceed when the Windows regional date format is not English

| Field | Content |
|---|---|
| **Rule name** | The application refuses to proceed when the Windows regional date format is not English |
| **Arabic label** | «رجاء تحويل التاريخ الى اللغة الانجليزية» |
| **Domain** | Deployment |
| **Description** | If the operating system's date format is not English, the application displays this message and, on confirmation, closes. The manual's remedy is `Start menu → Control Panel → Regional and Language Options → Regional Options → Standards and formats`. |
| **Business purpose** | The application depends on English-language date parsing; a locale mismatch would mis-parse dates. |
| **Trigger / applicable event** | Application start |
| **Preconditions** | None |
| **Actors and permissions** | System administrator fixes the workstation configuration |
| **Expected behaviour** | Message shown; application closes on **موافق** |
| **Restrictions** | The date format must be English |
| **Validation conditions** | The locale check itself is not observable statically |
| **Exceptions** | — |
| **Consequences** | The laboratory cannot use the workstation until the regional setting is changed |
| **Related workflows** | WF-V1-01 |
| **Related functions** | F-V1-001 |
| **Related entities** | — |
| **Relevant fields** | — |
| **Related statuses** | — |
| **Evidence** | CHM topic `001.htm`, verbatim message and remedy steps |
| **Evidence type** | ET-DOC |
| **Confidence** | CONFIRMED for the message and remedy; INFERRED for the underlying check |
| **Version applicability** | V-ROOT-1.00; CHM byte-identical in V-CD-2019 |
| **Known limitations** | The specific date fields affected are not established. **The remedy is a workstation-level change that affects every application on the machine.** |

#### V1-BR-045 — The database server must be running before any client can sign in

| Field | Content |
|---|---|
| **Rule name** | The database server must be running before any client can sign in |
| **Arabic label** | «التأكد من عمل "تشغيل" جهاز السيرفر (لا يتتطلب فتح البرنامج على السيرفر ليعمل على باقى الأجهزة)» |
| **Domain** | Deployment |
| **Description** | Clients require a running server machine; the server application itself need not be open. On the server machine, the operator must wait for operating-system start-up programs to finish and then press OK twice. |
| **Business purpose** | Establishes a hard availability dependency between the server machine and every workstation. |
| **Trigger / applicable event** | Application start |
| **Preconditions** | Server machine powered on and booted |
| **Actors and permissions** | System administrator |
| **Expected behaviour** | After the five-second wait, the application starts against the shared database |
| **Restrictions** | The server application must not be required on the server machine |
| **Validation conditions** | Not documented |
| **Exceptions** | «خادم قواعدabases غير معرف» on failure |
| **Consequences** | Every workstation is blocked until the server is available |
| **Related workflows** | WF-V1-01 |
| **Related functions** | F-V1-001 |
| **Related entities** | `loggroup` |
| **Relevant fields** | — |
| **Related statuses** | — |
| **Evidence** | CHM topic `001.htm`, verbatim |
| **Evidence type** | ET-DOC |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00; CHM byte-identical in V-CD-2019 |
| **Known limitations** | The actual connection timeout value is not established. |

### 3.2 Security rules

#### V1-BR-003 — First run uses the `admin` account with password equal to the user name

| Field | Content |
|---|---|
| **Rule name** | First run uses the `admin` account with password equal to the user name |
| **Arabic label** | «لأول مرة اختر admin … أدخل نفس الكلمة ككلمة مرور» |
| **Domain** | Security |
| **Description** | On the first run of the system the operator selects `admin` and enters the same word as the password. |
| **Business purpose** | Bootstrap access before individual accounts exist. |
| **Trigger / applicable event** | First installation |
| **Preconditions** | Fresh installation |
| **Actors and permissions** | Installing administrator |
| **Expected behaviour** | The `admin` account signs in with password `admin` |
| **Restrictions** | — |
| **Validation conditions** | Not documented |
| **Exceptions** | — |
| **Consequences** | **If the password is not changed, the laboratory runs with a publicly documented administrative credential.** The manual does not state that a change is mandatory — no such instruction appears in the 15 topics. |
| **Related workflows** | WF-V1-01 |
| **Related functions** | F-V1-002, F-V1-020 |
| **Related entities** | `useraccounts` |
| **Relevant fields** | — |
| **Related statuses** | — |
| **Evidence** | CHM topic `002.htm`, verbatim; literal `admin` at executable offset `0x7BA378`; form `userspw` (password change) exists |
| **Evidence type** | ET-DOC+CODE |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00; CHM byte-identical in V-CD-2019 |
| **Known limitations** | Whether the system enforces a password change on first use is NOT ESTABLISHED. |

#### V1-BR-042 — The database password is stored in clear text in shipped configuration

| Field | Content |
|---|---|
| **Rule name** | The database password is stored in clear text in shipped configuration |
| **Arabic label** | — (configuration rule; no Arabic label) |
| **Domain** | Security |
| **Description** | The application reads the database password from a plain-text INI/UDL file shipped with the product. The shipped value is `12345678`. |
| **Business purpose** | *(none — this is a defect, not a designed behaviour)* |
| **Trigger / applicable event** | Application start (connection establishment) |
| **Preconditions** | The configuration file is readable by the operator — it always is, being in the application folder |
| **Actors and permissions** | Anyone with file access to the workstation or to the installation media |
| **Expected behaviour** | The connection is established with the stored credential |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | The credential is readable without any privilege escalation, and because the same credential is shared across all clients (V1-BR-002), its exposure compromises every client. |
| **Related workflows** | WF-V1-01 |
| **Related functions** | F-V1-001, F-V1-002 |
| **Related entities** | `BASE.ini`, `data_connection.udl`, `data_connection1.udl` |
| **Relevant fields** | `Password` |
| **Related statuses** | — |
| **Evidence** | `bin\BASE.ini` `Password=12345678`; `bin\data_connection1.udl` `Password=12345678` |
| **Evidence type** | ET-CFG |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 **and** V-CD-2019 — the same values are present in the 2019 payload copy of `data_connection1.udl` (verified by comparison in BATCH-07) |
| **Known limitations** | Whether the shipped value is a placeholder that the installer replaces is NOT ESTABLISHED for the Version 1 tree (no installer is present). |

#### V1-BR-043 — The application connects to the database as the administrative account `sa`

| Field | Content |
|---|---|
| **Rule name** | The application connects to the database as the administrative account `sa` |
| **Domain** | Security |
| **Description** | The OLE DB connection strings specify `UserID=sa` / `User ID=sa`. |
| **Business purpose** | *(none — this is a defect)* |
| **Trigger / applicable event** | Every database connection |
| **Preconditions** | SQL Server deployment mode |
| **Actors and permissions** | The application itself holds full administrative rights on the database |
| **Expected behaviour** | The application connects with full privilege |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | A compromise of the application host yields full database administrative rights; there is no per-user database identity and therefore no database-level separation of duties. |
| **Related workflows** | WF-V1-01 |
| **Related functions** | F-V1-001 |
| **Related entities** | `BASE.ini`, `data_connection*.udl` |
| **Relevant fields** | `UserID`, `User ID` |
| **Related statuses** | — |
| **Evidence** | `BASE.ini` `UserID=sa`; `data_connection.udl` and `data_connection1.udl` `User ID=sa` |
| **Evidence type** | ET-CFG |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 |
| **Known limitations** | The Access (Jet) deployment mode does not use this identity; the rule applies to the SQL Server path. |

### 3.3 Patient rules

#### V1-BR-005 — The short patient identifier is positionally derived from `patientid`

| Field | Content |
|---|---|
| **Rule name** | The short patient identifier is positionally derived from `patientid` |
| **Arabic label** | — |
| **Domain** | Patient |
| **Description** | The identifier is **composite and position-dependent**. Three distinct derivations are evidenced in the extracted SQL: **`MID(patientid,3,2)`** is the **person key** — used both as the patient filter and as `COUNT(MID(patientid,3,2))` in the turnover reports; **`mid(patientid,10,3)`** is a short **display** code (`as patid`); **`MID(patientid,9,1)`** is the **branch** code (`as bran`, V-CD-2019). **The identifier is an order/visit identifier; there is no separate patient master table.** |
| **Business purpose** | A compact display key derived from a composite identifier, avoiding a separate lookup. |
| **Trigger / applicable event** | Patient search, patient display |
| **Preconditions** | — |
| **Actors and permissions** | — |
| **Expected behaviour** | The system derives a short key from the stored identifier |
| **Restrictions** | Any change to the identifier's layout silently changes the meaning of every derived value |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **Structural constraint:** the identifier layout is a de-facto data contract. In V-CD-2019 the branch code occupies **position 9** (`SUBSTRING(patientid,9,1)`), so the same mechanism serves two different purposes in the two versions. |
| **Related workflows** | WF-V1-01 |
| **Related functions** | F-V1-005 |
| **Related entities** | `patientinfo`, `patienttest` |
| **Relevant fields** | `patientid` — `MID(patientid,3,2)` = person key · `mid(patientid,10,3)` = display code · position 9 = branch code (V-CD-2019) |
| **Related statuses** | — |
| **Evidence** | Extracted SQL using `MID(patientid,3,2)`, `MID(patientid,9,1) as bran` and `mid(patientid,10,3) as patid` — `analysis/BATCH-01/sql/statements.csv`; CHM `006.htm` |
| **Evidence type** | ET-CODE |
| **Confidence** | CONFIRMED that the derivation exists and is positional; UNKNOWN as to the exact segment semantics |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 (same mechanism; V2 adds the branch segment at position 9) |
| **Known limitations** | **The remaining segment semantics of `patientid` are not fully established** — only positions 3–4, 9 and 10–12 are evidenced. RL-V1-Q-11. |

#### V1-BR-006 — The patient barcode is the direct patient identifier carried on a physical artefact

| Field | Content |
|---|---|
| **Rule name** | The patient barcode is the direct patient identifier carried on a physical artefact |
| **Arabic label** | «كيف يتم طباعة باركود للمريض؟» |
| **Domain** | Patient / Security |
| **Description** | A barcode identifying the patient is printed on a label that accompanies the specimen. The barcode table holds allocated barcodes and the patient record carries the barcode. |
| **Business purpose** | Physical specimen-to-record binding at the bench. |
| **Trigger / applicable event** | Patient registration, barcode printing |
| **Preconditions** | — |
| **Actors and permissions** | — |
| **Expected behaviour** | Every specimen carries a barcode that resolves to one patient record |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **The barcode is a direct identifier on an uncontrolled physical artefact.** A label removed from the laboratory discloses the patient's identity to any holder of the label. |
| **Related workflows** | WF-V1-01 |
| **Related functions** | F-V1-004 |
| **Related entities** | `barcodes`, `patientinfo` |
| **Relevant fields** | barcode field in `patientinfo`; barcode field in `IDCARD.INI [IDdata]` |
| **Related statuses** | — |
| **Evidence** | CHM `003A.htm`; `barcodes` table (35 rows); `IDCARD.INI` barcode layout entry; form `barcode` (71 controls) |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 (both ship a `barcode` form and a `barcodes` table) |
| **Known limitations** | The symbology is not established. |

#### V1-BR-007 — `history` stores 20 free-form columns as generic attribute slots

| Field | Content |
|---|---|
| **Rule name** | `history` stores 20 free-form columns as generic attribute slots |
| **Arabic label** | «التاريخ المرضي للمريض» |
| **Domain** | Patient / Data |
| **Description** | The `history` table holds 20 columns with no fixed semantics, used as attribute slots — one row per patient. The `historytype` setting in `SETTING.ini` selects the display mode. |
| **Business purpose** | To store patient medical-history attributes without a schema migration per attribute. |
| **Trigger / applicable event** | Storing or reading patient history attributes |
| **Preconditions** | — |
| **Actors and permissions** | — |
| **Expected behaviour** | Attributes are written into positional slots |
| **Restrictions** | **No type enforcement, no per-column meaning, no validation.** The same table appears in `Patients.mdb` (4 rows) and `data.mdb` (0 rows) — which of them is authoritative is UNKNOWN. |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | Column meaning is positional and therefore fragile; a change in insertion order silently mis-attributes historical data. |
| **Related workflows** | WF-V1-01 |
| **Related functions** | F-V1-006 |
| **Related entities** | `history` |
| **Relevant fields** | 20 free-form columns |
| **Related statuses** | — |
| **Evidence** | `analysis/BATCH-02/db/data/` and `Patients` schema; `SETTING.ini [setup] historytype=0`; CHM references to «التاريخ المرضي» |
| **Evidence type** | ET-DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 |
| **Known limitations** | The per-slot meaning of the 20 columns is not established. RL-V1-Q-02. |

### 3.4 Result-classification rules

#### V1-BR-009 — Result classification is driven by `normalrange` limits, not hard-coded values

| Field | Content |
|---|---|
| **Rule name** | Result classification is driven by `normalrange` limits, not hard-coded values |
| **Arabic label** | «يميز النتائج الغير طبيعية والحرجة ويكتب تمييز لها (عالٍ أو منخفضة أو حرجة)» |
| **Domain** | Results |
| **Description** | On entry, a result value is compared against the low and high limits held in `normalrange` for the analyte. The limits are **data**, administered through the «المعدل الطبيعي» editor, not compiled constants. |
| **Business purpose** | The laboratory can change its reference policy without a software change — the core extensibility property of the system. |
| **Trigger / applicable event** | Result value entry |
| **Preconditions** | A `normalrange` row exists matching the analyte, the patient's sex and the patient's age band |
| **Actors and permissions** | Lab operator (entry); administrator (range maintenance) |
| **Expected behaviour** | Value within limits → normal; outside → abnormal with the stored exception text; beyond the critical limit → critical |
| **Restrictions** | Classification is impossible without a matching range row |
| **Validation conditions** | Numeric comparison of the entered value against `lowlimit` / `highlimit` |
| **Exceptions** | Behaviour when no range row matches is **NOT ESTABLANCED** |
| **Consequences** | A missing or mis-banded range silently removes classification for that analyte. This makes the range table the system's principal safety-critical knowledge asset. |
| **Related workflows** | WF-V1-02 |
| **Related functions** | F-V1-008, F-V1-012 |
| **Related entities** | `normalrange`, `patienttest`, profile result tables |
| **Relevant fields** | `lowlimit`, `highlimit`, `nolowexe`, `nohighexe` |
| **Related statuses** | The normal / low / high / critical flags |
| **Evidence** | CHM `004A.htm` verbatim; `normalrange` schema (852 rows); `patienttest` columns |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 — the same model, same table, same field names; V2 adds 10 new tables but does not change the classification model |
| **Known limitations** | The `pregsmok` dimension (populated on all 852 range rows) may narrow the comparison; its use is NOT traced. |

#### V1-BR-010 — A critical limit overrides the abnormal classification

| Field | Content |
|---|---|
| **Rule name** | A critical limit overrides the abnormal classification |
| **Arabic label** | «…الغير طبيعية والحرجة… (عالٍ أو منخفضة أو حرجة)» |
| **Domain** | Results / Clinical safety |
| **Description** | The classification set is three-valued and ordered: normal → abnormal (high/low) → **critical**. A value breaching the critical limit is presented as critical rather than merely abnormal. |
| **Business purpose** | Distinguishes values requiring immediate escalation from values merely outside the reference interval. |
| **Trigger / applicable event** | Result value entry |
| **Preconditions** | Critical limit and critical range populated for the matching range row |
| **Actors and permissions** | Lab operator |
| **Expected behaviour** | Critical results are visibly distinct from abnormal results |
| **Restrictions** | — |
| **Validation conditions** | Comparison against the critical limit / critical range fields |
| **Exceptions** | — |
| **Consequences** | The critical threshold is the highest-consequence value in the system; getting it wrong puts a life-critical value in the same visual class as a mildly abnormal one. |
| **Related workflows** | WF-V1-02 |
| **Related functions** | F-V1-008 |
| **Related entities** | `normalrange`, `patienttest` |
| **Relevant fields** | `Critical range`, `Critical comment`, `Critical flag`, critical `Low/High limit` |
| **Related statuses** | Critical flag |
| **Evidence** | CHM `004A.htm` verbatim; `normalrange` critical columns; `patienttest` limit columns |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 |
| **Known limitations** | The exact flag precedence when a value is both above the critical limit and below the low limit is not established. |

#### V1-BR-011 — Reference ranges are banded by sex and age

| Field | Content |
|---|---|
| **Rule name** | Reference ranges are banded by sex and age |
| **Arabic label** | «كيف يتم اضافة أو تعديل المعدلات الطبيعية لتحليل معين ؟» |
| **Domain** | Clinical knowledge |
| **Description** | Each `normalrange` row is scoped by `Sex` (`Male` / `Female` / `All`), an age window (`Age From`, `Age To`) and an age unit (`Years` / `Days` / `Mons.`). |
| **Business purpose** | Paediatric and sex-specific physiology require different reference intervals for the same analyte. |
| **Trigger / applicable event** | Range administration; result classification |
| **Preconditions** | The patient's sex and age are known |
| **Actors and permissions** | Laboratory administrator |
| **Expected behaviour** | The applicable range is selected by analyte + sex + age band |
| **Restrictions** | Age bands must not overlap for the same analyte and sex; overlap detection is **NOT ESTABLANCED** |
| **Validation conditions** | — |
| **Exceptions** | Only 5 of 852 rows are `All` and 4 rows carry an empty sex — so a patient whose sex is unrecorded may match only those 9 rows for that analyte |
| **Consequences** | The selection rule — first match, best match, or most specific match — is **NOT ESTABLANCED** and materially affects classification. |
| **Related workflows** | WF-V1-02, WF-V1-04 |
| **Related functions** | F-V1-008, F-V1-012 |
| **Related entities** | `normalrange` |
| **Relevant fields** | `Sex`, `Age From`, `Age To`, `Age unit` |
| **Related statuses** | — |
| **Evidence** | CHM `008.htm` field list; `normalrange` distribution — `Male` 422 / `Female` 421 / `All` 5 / empty 4; age units `Years` 708 / `Days` 70 / `Mons.` 65 / empty 9 |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED for the banding; UNKNOWN for the selection algorithm |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 (identical table, identical row count of 852) |
| **Known limitations** | The selection algorithm and overlap validation are not established. |

#### V1-BR-013 — A result comment may be sourced from three distinct places

| Field | Content |
|---|---|
| **Rule name** | A result comment may be sourced from three distinct places |
| **Arabic label** | «زر تعليق (البرتقالي)» |
| **Domain** | Results |
| **Description** | The comment attached to a result may be typed by the operator, selected from the stored `comments` table, or taken from the normal-ranges list through the orange comment button. |
| **Business purpose** | Standardises the wording of common findings while allowing free text. |
| **Trigger / applicable event** | Comment entry during result entry |
| **Preconditions** | — |
| **Actors and permissions** | Lab operator |
| **Expected behaviour** | The result carries a comment regardless of its source |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | Comment text quality depends on the completeness of the `comments` table and of the range rows' comment fields. |
| **Related workflows** | WF-V1-02 |
| **Related functions** | F-V1-008 |
| **Related entities** | `comments`, `normalrange`, `patienttest` |
| **Relevant fields** | Low/High comment, Critical comment |
| **Related statuses** | — |
| **Evidence** | CHM `004A.htm`; `comments` table (12 rows); `normalrange` comment columns (19 distinct low / 19 distinct high) |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 |
| **Known limitations** | — |

### 3.5 Catalogue rules

#### V1-BR-016 — Test number 4 — the urine analysis — is immutable in both number and name

| Field | Content |
|---|---|
| **Rule name** | Test number 4 — the urine analysis — is immutable in both number and name |
| **Arabic label** | «تحليل البول يحمل الرقم 4 في النظام ولا يجوز تغيير رقمه ولا اسمه» |
| **Domain** | Catalogue |
| **Description** | The manual's standing notes state that the urine analysis carries number 4 in the system and that **neither its number nor its name may be changed**. |
| **Business purpose** | Preserves a stable key for the most-used test, on which printed report layouts and historic records depend. |
| **Trigger / applicable event** | Any attempt to edit the urine analysis test |
| **Preconditions** | — |
| **Actors and permissions** | Laboratory administrator; the restriction is documented as absolute |
| **Expected behaviour** | The edit is not permitted |
| **Restrictions** | Number **and** name both immutable |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | The `Data\*.ini` report layouts and the `normalrange` rows for urine are keyed to this identity and cannot be re-pointed. |
| **Related workflows** | WF-V1-04 |
| **Related functions** | F-V1-011, F-V1-012 |
| **Related entities** | `testtable`, `normalrange`, `Data\*.ini` |
| **Relevant fields** | `testtable.testcode`, `testtable.testname` |
| **Related statuses** | — |
| **Evidence** | CHM topic `nots.htm`, verbatim; `testtable` 609 rows with `testcode` |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 — the CHM is byte-identical, so the same restriction applies to the 2020 product |
| **Known limitations** | Whether the restriction is enforced in code or only documented is NOT ESTABLISHED. |

#### V1-BR-031 — The clinical sub-group is the routing key for both the entry window and the report layout

| Field | Content |
|---|---|
| **Rule name** | The clinical sub-group is the routing key for both the entry window and the report layout |
| **Arabic label** | — |
| **Domain** | Catalogue / Reporting |
| **Description** | A test's membership in a clinical sub-group determines which result-entry window the operator lands in and which `Data\*.ini` report layout is used to print it. The sub-group is therefore the pivot of the whole system. |
| **Business purpose** | Keeps entry, printing and catalogue consistent without a separate mapping. |
| **Trigger / applicable event** | Test assignment; result entry; printing |
| **Preconditions** | The test belongs to exactly one sub-group |
| **Actors and permissions** | Laboratory administrator |
| **Expected behaviour** | Sub-group determines window and layout |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | 46 sub-groups exist while 223 `.ini` layout files exist; the mapping is many-to-one and is **not established** (RL-V1-Q-07). |
| **Related workflows** | WF-V1-02, WF-V1-05 |
| **Related functions** | F-V1-007, F-V1-011, F-V1-013, F-V1-019 |
| **Related entities** | `subgrouptable`, `testtable`, `Data\*.ini` |
| **Relevant fields** | `subgroupcode` on `patienttest`; `subgroupcode` on `testtable` |
| **Related statuses** | — |
| **Evidence** | `subgrouptable` 46 rows; `patienttest.subgroupcode`; `Data\*.ini` 223 files; CHM `004.htm` |
| **Evidence type** | ET-DERIVED |
| **Confidence** | INFERRED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 |
| **Known limitations** | The mapping algorithm between sub-groups and `.ini` files is not established. |

### 3.6 Send-out rules

#### V1-BR-017 — A test flagged `inotherlab` is enrolled automatically into the send-out workflow

| Field | Content |
|---|---|
| **Rule name** | A test flagged `inotherlab` is enrolled automatically into the send-out workflow |
| **Arabic label** | «كيف يتم ادارة العينات المرسلة للخارج من داخل النظام ؟» |
| **Domain** | Send-out |
| **Description** | Tests marked `inotherlab = True` in `testtable` are discovered automatically by the send-out screen; the operator does not select them manually. |
| **Business purpose** | Removes a manual classification step from a recurring workflow. |
| **Trigger / applicable event** | Test ordering; opening the send-out screen |
| **Preconditions** | The test's `inotherlab` flag is set |
| **Actors and permissions** | Laboratory administrator sets the flag; accounting user works the queue |
| **Expected behaviour** | Flagged tests appear in the send-out list |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | The flag is a **cataloguing** decision with **operational** consequences — it puts a test into a money- and quality-tracking queue. |
| **Related workflows** | WF-V1-03 |
| **Related functions** | F-V1-011, F-V1-014 |
| **Related entities** | `testtable`, `patienttest` |
| **Relevant fields** | `testtable.inotherlab`, `patienttest.sentout`, `patienttest.send`, `patienttest.otherlabname` |
| **Related statuses** | `sentout`, `send` |
| **Evidence** | CHM `010.htm`; `testtable` and `patienttest` schemas |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 |
| **Known limitations** | — |

#### V1-BR-023 — «دائماً إليه» makes the destination the standing default for that test

| Field | Content |
|---|---|
| **Rule name** | «دائماً إليه» makes the destination the standing default for that test |
| **Arabic label** | «دائماً إليه» |
| **Domain** | Send-out |
| **Description** | A destination laboratory marked «دائماً إليه» (always to it) becomes the standing default destination for the associated test, so the operator need not re-select it. |
| **Business purpose** | Removes repetition in the recurring send-out queue. |
| **Trigger / applicable event** | Send-out settlement |
| **Preconditions** | A destination exists for the test |
| **Actors and permissions** | Accounting user |
| **Expected behaviour** | The destination is pre-selected for subsequent occurrences of that test |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | — |
| **Related workflows** | WF-V1-03 |
| **Related functions** | F-V1-014 |
| **Related entities** | `otherlabname`, `testtable`, `patienttest` |
| **Relevant fields** | `otherlabname` |
| **Related statuses** | — |
| **Evidence** | CHM `010.htm`, verbatim «دائماً إليه» |
| **Evidence type** | ET-DOC |
| **Confidence** | CONFIRMED for the label and its effect; INFERRED for the storage mechanism |
| **Version applicability** | V-ROOT-1.00; CHM byte-identical in V-CD-2019 |
| **Known limitations** | The field that stores the standing-default flag is not isolated. |

#### V1-BR-024 — Send-out settlement carries two independent states: sent, and settled

| Field | Content |
|---|---|
| **Rule name** | Send-out settlement carries two independent states: sent, and settled |
| **Arabic label** | «خالص» |
| **Domain** | Send-out / Finance |
| **Description** | A send-out item carries two independent states: `sent` (dispatched) and `sentout`/settled. The manual's workflow enters the settled amount and then marks «خالص». The system can therefore track what was dispatched but not yet settled — which is precisely what the manual's follow-up step requires. |
| **Business purpose** | Separates dispatch from payment, so an unsettled referral remains visible. |
| **Trigger / applicable event** | Send-out settlement |
| **Preconditions** | — |
| **Actors and permissions** | Accounting user |
| **Expected behaviour** | Unsent and unsettled items remain listable |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | — |
| **Related workflows** | WF-V1-03, WF-V1-04 |
| **Related functions** | F-V1-014, F-V1-015 |
| **Related entities** | `patienttest` |
| **Relevant fields** | `sentout`, `send` |
| **Related statuses** | Sent / not sent; settled / not settled |
| **Evidence** | CHM `010.htm`; `patienttest.sentout` and `patienttest.send` columns |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 |
| **Known limitations** | — |

### 3.7 Finance rules

#### V1-BR-025 — The collection scope for an accounting period is an explicit operator decision

| Field | Content |
|---|---|
| **Rule name** | The collection scope for an accounting period is an explicit operator decision |
| **Arabic label** | «هل الفلوس المحصلة من مرضى الفترة فقط؟» |
| **Domain** | Finance |
| **Description** | The stock-take asks the operator to decide whether the money counted is only that collected from patients of the selected period, or all money held. The rule is a **prompt**, not an automatic filter. |
| **Business purpose** | Cash-based laboratories collect money for work performed earlier; the period close is therefore a judgement. |
| **Trigger / applicable event** | Period stock-take |
| **Preconditions** | A period selected |
| **Actors and permissions** | Accounting user |
| **Expected behaviour** | The operator chooses the scope; the result follows that choice |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **Two laboratories using the same data can produce different stock-take figures** depending on this choice. The figure is therefore not reproducible without recording the choice. |
| **Related workflows** | WF-V1-04 |
| **Related functions** | F-V1-015 |
| **Related entities** | `payment`, `patientinfo`, `patienttest` |
| **Relevant fields** | `totalprice`, `totalpricedisc`, `payment` date fields |
| **Related statuses** | — |
| **Evidence** | CHM `011.htm`, verbatim |
| **Evidence type** | ET-DOC |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00; CHM byte-identical in V-CD-2019. V-CD-2019 adds a **branch** dimension to the same stock-take (see the V2 catalogue). |
| **Known limitations** | The arithmetic of each sub-report is not established. |

#### V1-BR-026 — Referring-party commission is stored on the referring-party record

| Field | Content |
|---|---|
| **Rule name** | Referring-party commission is stored on the referring-party record |
| **Arabic label** | «عمولات جهات الإحالة» |
| **Domain** | Finance |
| **Description** | The commission owed to a referring party is held on the `Gehats` record in the field `omola`, and is applied against the patient-level base `patientinfo.elmadfoo3` by the formula `SUM(Gehats.omola * patientinfo.elmadfoo3 / 100)` — as it appears in the extracted SQL. |
| **Business purpose** | Allows a negotiated commission rate to be agreed per party independently of volume. |
| **Trigger / applicable event** | Stock-take |
| **Preconditions** | The referring party exists |
| **Actors and permissions** | Accounting user |
| **Expected behaviour** | The commission figure is read from `Gehats.omola` |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | The commission is a **negotiated attribute of the party**, so it is independent of how much work that party sent. |
| **Related workflows** | WF-V1-04 |
| **Related functions** | F-V1-015, F-V1-022 |
| **Related entities** | `Gehats` |
| **Relevant fields** | `Gehats.omola`, `patientinfo.elmadfoo3` |
| **Related statuses** | — |
| **Evidence** | `Gehats` schema with `omola`; CHM `011.htm` |
| **Evidence type** | ET-DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 |
| **Known limitations** | The base against which the commission is computed is not established. |

#### V1-BR-027 — Per-user workload is a first-class accounting output

| Field | Content |
|---|---|
| **Rule name** | Per-user workload is a first-class accounting output |
| **Arabic label** | «حجم عمل كل مستخدم» |
| **Domain** | Finance |
| **Description** | The stock-take reports, per user, the volume of work performed and the amounts collected with that user. |
| **Business purpose** | Makes individual performance and cash accountability measurable from the same data set. |
| **Trigger / applicable event** | Period stock-take |
| **Preconditions** | Users sign in individually (V1-BR-032) |
| **Actors and permissions** | Accounting user |
| **Expected behaviour** | Per-user workload and per-user collections are listed |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **This rule is the reason individual sign-in matters operationally, not merely administratively.** It also makes the print-attribution fields (`printuser`) and the login group load-bearing for finance. |
| **Related workflows** | WF-V1-04 |
| **Related functions** | F-V1-015, F-V1-002 |
| **Related entities** | `patienttest`, `payment`, `useraccounts`, `loggroup` |
| **Relevant fields** | `printuser`, `resultprint` |
| **Related statuses** | — |
| **Evidence** | CHM `011.htm`, verbatim; `printuser` on `patienttest` |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 |
| **Known limitations** | — |

#### V1-BR-015 — The patient account total derives from `totalprice` and `totalpricedisc`

| Field | Content |
|---|---|
| **Rule name** | The patient account total derives from `totalprice` and `totalpricedisc` |
| **Arabic label** | «تصفية حساب المريض» |
| **Domain** | Finance |
| **Description** | The patient account carries two totals: `totalprice` and `totalpricedisc` (discounted). |
| **Business purpose** | Supports both gross and discounted patient accounts. |
| **Trigger / applicable event** | Hand-over; account settlement |
| **Preconditions** | — |
| **Actors and permissions** | Reception clerk |
| **Expected behaviour** | The settled amount reflects the discounted total |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | The derivation rule (which of the two is authoritative) is **NOT ESTABLANCED**. |
| **Related workflows** | WF-V1-01 |
| **Related functions** | F-V1-010, F-V1-015 |
| **Related entities** | `patientinfo`, `payment` |
| **Relevant fields** | `totalprice`, `totalpricedisc` |
| **Related statuses** | — |
| **Evidence** | `patientinfo` schema; CHM `005.htm` |
| **Evidence type** | ET-DB |
| **Confidence** | STRONGLY SUPPORTED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 |
| **Known limitations** | The pricing and discount computation is not established from static evidence. |

### 3.8 Reporting and configuration rules

#### V1-BR-020 — The report colour palette is a 19-entry table stored in a text file named `REPORT.dll`

| Field | Content |
|---|---|
| **Rule name** | The report colour palette is a 19-entry table stored in a text file named `REPORT.dll` |
| **Arabic label** | «تعديل ألوان رأس وذيل التقرير» |
| **Domain** | Reporting |
| **Description** | `bin\REPORT.dll` is **not a binary library**. It is a 562-byte plain-text INI with a `[reportdata]` section holding `color1`..`color19` as BGR integers (e.g. `14737632` = 0xE0E0E0, `12632256` = 0xC0C0C0, `16777215` = 0xFFFFFF, `46444020` = 0x2C4C64, `4210752` = 0x404040). |
| **Business purpose** | Centralises the report colour scheme so that head/footer colours are edited in one place. |
| **Trigger / applicable event** | Report customisation; report rendering |
| **Preconditions** | — |
| **Actors and permissions** | Laboratory administrator |
| **Expected behaviour** | Report head/footer colours come from this table; per-position colours come from each profile's `.ini` (24 entries there) |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **Two colour sources coexist:** a 19-entry global palette and a 24-entry per-profile palette. Their precedence is **NOT ESTABLANCED**. |
| **Related workflows** | WF-V1-05 |
| **Related functions** | F-V1-013 |
| **Related entities** | `REPORT.dll`, `Data\*.ini` |
| **Relevant fields** | `color1`..`color19` |
| **Related statuses** | — |
| **Evidence** | `bin\REPORT.dll` content; `Data\*.ini [reportdata]` colour entries |
| **Evidence type** | ET-CFG+DERIVED |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 |
| **Known limitations** | The file extension is misleading and could be mistaken for a binary during analysis; the precedence of the two palettes is unknown. |

#### V1-BR-021 — Each report profile binds its own printer

| Field | Content |
|---|---|
| **Rule name** | Each report profile binds its own printer |
| **Arabic label** | «ضبط الطابعات المخصصة للطباعة» (documented in the V-CD-2019 guide; the V1 CHM covers the equivalent under report customisation) |
| **Domain** | Reporting |
| **Description** | Every `Data\*.ini` carries `[setupx] reportprinterx=<printer>`, binding a printer to that clinical profile independently of the global role bindings. 108 of the profiles bind `HP LaserJet 1100 (MS)`, 2 bind `HP LaserJet 1100`, and 2 bind `Send To OneNote 2007`. |
| **Business purpose** | Different disciplines can print on different devices — for example barcoded slips on a dedicated printer. |
| **Trigger / applicable event** | Report printing |
| **Preconditions** | The bound printer exists on the workstation |
| **Actors and permissions** | Laboratory administrator |
| **Expected behaviour** | The profile's print goes to its bound printer |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | «Can't find any printer.»; «SetPrinter Failed. Error code: » |
| **Exceptions / alternative** | The global role binding in `SETTING.ini [selectprinter]` is the fallback; the precedence is **NOT ESTABLANCED** |
| **Consequences** | Printer migration requires editing 223 configuration files — a material operational cost. |
| **Related workflows** | WF-V1-05 |
| **Related functions** | F-V1-013, F-V1-021 |
| **Related entities** | `Data\*.ini` |
| **Relevant fields** | `reportprinterx` |
| **Related statuses** | — |
| **Evidence** | `Data\*.ini [setupx]` values, counted across 223 files |
| **Evidence type** | ET-CFG |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 |
| **Known limitations** | Precedence between profile binding and global binding is unknown. |

#### V1-BR-033 — The report sequence origin is the absolute value `reportstart = 1984`

| Field | Content |
|---|---|
| **Rule name** | The report sequence origin is the absolute value `reportstart = 1984` |
| **Arabic label** | — |
| **Domain** | Reporting |
| **Description** | The report numbering sequence starts from a fixed absolute value, `1984`, configured in `SETTING.ini [setup] reportstart`. |
| **Business purpose** | Produces a continuous, non-repeating report numbering scheme independent of the patient's visit. |
| **Trigger / applicable event** | Report printing |
| **Preconditions** | — |
| **Actors and permissions** | Laboratory administrator |
| **Expected behaviour** | Printed reports carry a sequence number derived from the configured origin |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | The number is a **document identifier**. It is an absolute counter and therefore also a covert confidentiality mechanism: a missing or duplicated number reveals a missing report. In V-CD-2019 this role is taken by `patienttest.printnum`. |
| **Related workflows** | WF-V1-05 |
| **Related functions** | F-V1-013, F-V1-021 |
| **Related entities** | `SETTING.ini`, `patienttest` |
| **Relevant fields** | `reportstart`, `printnum` |
| **Related statuses** | — |
| **Evidence** | `SETTING.ini [setup] reportstart=1984` |
| **Evidence type** | ET-CFG |
| **Confidence** | CONFIRMED for the setting; INFERRED for its effect on numbering |
| **Version applicability** | V-ROOT-1.00. V-CD-2019 retains `reportstart` **and** adds `patienttest.printnum` — the two coexist and their relationship is NOT ESTABLANCED. |
| **Known limitations** | The increment rule and the persistence location are not established. |

#### V1-BR-014 — Only reviewed results are eligible for hand-over

| Field | Content |
|---|---|
| **Rule name** | Only reviewed results are eligible for hand-over |
| **Arabic label** | «F8=اعتماد مُراجَع» |
| **Domain** | Results / Finance |
| **Description** | A result must carry the reviewed state (set with F8) before it can be handed to the patient. |
| **Business purpose** | Prevents release of an unreviewed or unverified result. |
| **Trigger / applicable event** | Result confirmation; hand-over |
| **Preconditions** | Result saved |
| **Actors and permissions** | Lab operator confirms; reception clerk hands over |
| **Expected behaviour** | Unreviewed results are not hand-over eligible |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | This is the system's **principal quality gate between measurement and release**. |
| **Related workflows** | WF-V1-01 |
| **Related functions** | F-V1-008, F-V1-009, F-V1-010 |
| **Related entities** | `patienttest` |
| **Relevant fields** | `resultreview` (Boolean) |
| **Related statuses** | Reviewed / not reviewed |
| **Evidence** | CHM `004A.htm` (F8 = اعتماد مُراجَع) and `005.htm` (hand-over) |
| **Evidence type** | ET-DOC |
| **Confidence** | STRONGLY SUPPORTED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019. **Both versions carry the explicit `patienttest.resultreview` Boolean field**, so the gate is directly observable in both. |
| **Known limitations** | The **storage field is identified** (`patienttest.resultreview`). What remains unknown is whether the review requires a second signatory and whether the field is cleared on re-edit. |

### 3.9 Clinical computation rules

#### V1-BR-037 — Haematocrit is derived as `Hct = Hgb × 3.3`

| Field | Content |
|---|---|
| **Rule name** | Haematocrit is derived as `Hct = Hgb × 3.3` |
| **Arabic label** | «Hct = Hgb × 3.3» (as stated in the manual's notes topic) |
| **Domain** | Clinical / Haematology |
| **Description** | Packed-cell volume is computed from the haemoglobin concentration using the factor **3.3**, held in `constanttable` group `CBCCONS`, slot `constant1`. |
| **Business purpose** | Derives Hct where the analyser does not report it, using the conventional approximation. |
| **Trigger / applicable event** | Result derivation in the CBC profile |
| **Preconditions** | A haemoglobin result exists |
| **Actors and permissions** | Lab operator |
| **Expected behaviour** | Hct is computed as 3.3 × Hgb |
| **Restrictions** | The factor is configurable in `constanttable` |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | The factor is **data**, so a laboratory can correct it without a software change — but it is also **silent**: a wrong factor propagates into every CBC report without any indication. |
| **Related workflows** | WF-V1-02 |
| **Related functions** | F-V1-008 |
| **Related entities** | `constanttable` (`CBCCONS`), `patienttest`, CBC profile table |
| **Relevant fields** | `constant1` |
| **Related statuses** | — |
| **Evidence** | CHM `nots.htm` verbatim; `constanttable` row `CBCCONS`/`constant1` = 3.3 — **exact numeric concordance** |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 — identical `constanttable` values |
| **Known limitations** | Whether the derived value is stored or computed on display is NOT ESTABLISHED. |

#### V1-BR-038 — Haemoglobin percentage is derived from `Hgb` and an age/sex factor

| Field | Content |
|---|---|
| **Rule name** | Haemoglobin percentage is derived from `Hgb` and an age/sex factor |
| **Arabic label** | «Hgb% = Hgb × 8.25 (<1y) / ×7.50 (1–12y) / ×6.25 (M>12y) / ×6.75 (F>12y)» |
| **Domain** | Clinical / Haematology |
| **Description** | Haemoglobin percentage is computed from the haemoglobin value multiplied by one of four factors held in `constanttable` group `CBCCONS`, slots `constant2`–`constant5`: **6.25** (male > 12 y), **6.75** (female > 12 y), **7.50** (1–12 y), **8.25** (< 1 y). |
| **Business purpose** | Converts an absolute haemoglobin concentration into the percentage form used by some reporting conventions and by paediatric practice. |
| **Trigger / applicable event** | Result derivation in the CBC profile |
| **Preconditions** | A haemoglobin result exists; the patient's age and sex are known to select the factor |
| **Actors and permissions** | Lab operator |
| **Expected behaviour** | The factor is selected by age band and sex, and Hgb% = Hgb × factor |
| **Restrictions** | The factor set is fixed at four; no factor exists for patients aged exactly 12 or with unknown sex |
| **Validation conditions** | Age band and sex selection |
| **Exceptions** | The boundary case of age 12 is covered by both the 1–12 and the >12 rules — the precedence is not documented |
| **Consequences** | The age-band boundary at 12 years is ambiguous in the source wording. This is the kind of detail that must be pinned down before a deterministic re-implementation. |
| **Related workflows** | WF-V1-02 |
| **Related functions** | F-V1-008 |
| **Related entities** | `constanttable` (`CBCCONS`), `patienttest` |
| **Relevant fields** | `constant2`, `constant3`, `constant4`, `constant5` |
| **Related statuses** | — |
| **Evidence** | CHM `nots.htm` verbatim; `constanttable` `CBCCONS` constants 2–5 = 6.25 / 6.75 / 7.5 / 8.25 — **exact numeric concordance** |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 |
| **Known limitations** | Boundary precedence at age 12 is not documented. |

#### V1-BR-039 — The ISI is computed from the PT control and the patient PT, with control times of 12 s (secondary 35 s)

| Field | Content |
|---|---|
| **Rule name** | The ISI is computed from the PT control and the patient PT |
| **Arabic label** | «ISI» (as recorded in the manual's notes topic) |
| **Domain** | Clinical / Coagulation |
| **Description** | The International Sensitivity Index is derived using a control time of **12** seconds (a secondary control time of **35** seconds is also held), and the resulting ISI factor is **1.33**. |
| **Business purpose** | Standardises prothrombin time reporting across laboratories and reagent lots. |
| **Trigger / applicable event** | PT result derivation |
| **Preconditions** | A PT result and the control time exist |
| **Actors and permissions** | Lab operator |
| **Expected behaviour** | The ISI is derived and reported |
| **Restrictions** | The control time is configurable in `constanttable` group `PTCONS` |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | The `PTCONS` constants are **reagent-lot dependent in reality**; holding them as data is correct, but the shipped set is a single fixed pair. |
| **Related workflows** | WF-V1-02 |
| **Related functions** | F-V1-008 |
| **Related entities** | `constanttable` (`PTCONS`), coagulation profile table, `control` |
| **Relevant fields** | `constant1` = 12, `constant2` = 35, `constant3` = 1.33 |
| **Related statuses** | — |
| **Evidence** | CHM `nots.htm`; `constanttable` `PTCONS` constants 1–3 — exact numeric concordance |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 |
| **Known limitations** | The exact ISI formula (control/patient ratio against the ISI factor) is **NOT ESTABLISHED** from the manual wording. |

#### V1-BR-040 — The Westgard `2CVJW` rejection rule is applied to coagulation control values

| Field | Content |
|---|---|
| **Rule name** | The Westgard `2CVJW` rejection rule is applied to coagulation control values |
| **Arabic label** | «2CVJW» (the rule name as stored) |
| **Domain** | Clinical / Coagulation / Quality control |
| **Description** | The system holds the Westgard multi-rule identifier `2CVJW` as a coagulation constant. This denotes the **2CV rule with a warning-level check on consecutive control observations** — a recognised rule set for rejecting an analytical run. |
| **Business purpose** | Prevents reporting patient results from an analytically unacceptable control run. |
| **Trigger / applicable event** | Coagulation control evaluation |
| **Preconditions** | Control observations available |
| **Actors and permissions** | Lab operator / QC reviewer |
| **Expected behaviour** | The rule set governs acceptance or rejection of the run |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **The rule is stored as a single opaque string, not as structured criteria.** The system therefore either hard-codes the evaluation or stores only the label. The detailed rule semantics are **NOT ESTABLANCED** — this is a genuine limitation, not an omission in the analysis. |
| **Related workflows** | WF-V1-02 |
| **Related functions** | F-V1-008 |
| **Related entities** | `constanttable` (`PTCONS`), `control` (33-point control curve), `ptc-curve.csv` |
| **Relevant fields** | `constant4` = `2CVJW` |
| **Related statuses** | Run accepted / rejected |
| **Evidence** | CHM `nots.htm`; `constanttable` `PTCONS`/`constant4` = `2CVJW`; `control` table with 33 control/concentration pairs |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED that the rule identifier is held; UNKNOWN for its executable semantics |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 |
| **Known limitations** | **The QC evaluation algorithm is not recoverable from static evidence.** |

#### V1-BR-041 — Per-analyte critical-value thresholds are stated in the method sheets

| Field | Content |
|---|---|
| **Rule name** | Per-analyte critical-value thresholds are stated in the method sheets |
| **Arabic label** | — (the thresholds are Arabic prose in the method sheets) |
| **Domain** | Clinical |
| **Description** | The 29 `Cache\*.rtf` method sheets state critical thresholds in prose, for example: CBC — `Hgb < 5` or `> 20 g/dL`, `Hct < 15%` or `> 60%`, `WBC < 500` or `> 50,000/mm³`, `Platelets < 50,000` or `> 999,000/mm³`; BUN — `> 40 mg/dL` without dehydration or renal history, `> 100 mg/dL` with renal history, `> 20 mg/dL` rise over 24 h; Bilirubin — newborn `> 15 mg/dL`, SI `> 257 µmol/L`; Ionised Calcium — `> 6.29 mg/dL` / `< 3.1 mg/dL`; Total Calcium — `> 12 mg/dL` (SI `> 2.99`) / `< 7 mg/dL` (SI `< 1.75`); Sodium — `< 80 mEq/L` / `> 115 mEq/L`. |
| **Business purpose** | Gives the operator the escalation thresholds at the moment of result entry. |
| **Trigger / applicable event** | Result entry |
| **Preconditions** | — |
| **Actors and permissions** | Lab operator |
| **Expected behaviour** | The operator compares the entered value with the stated threshold |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **★ Two independent sources of criticality exist — the method sheet (prose) and `normalrange` (structured columns).** They are not shown to be reconciled, and only 29 of 609 tests have a sheet. The effective critical set for any given analyte therefore depends on which source the operator consults. |
| **Related workflows** | WF-V1-02 |
| **Related functions** | F-V1-024, F-V1-008 |
| **Related entities** | `Cache\*.rtf`, `normalrange` |
| **Relevant fields** | Critical range, Critical comment, Critical flag |
| **Related statuses** | Critical |
| **Evidence** | `analysis/BATCH-02/rtf/` parsed thresholds; `normalrange` critical columns |
| **Evidence type** | ET-PRES+DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 — the 29 sheets are byte-identical between versions |
| **Known limitations** | Which analytes have a sheet is not established (29 sheets against 609 tests). |

### 3.10 Other confirmed rules

| ID | Rule | Evidence | Confidence | Version applicability |
|---|---|---|---|---|
| **V1-BR-004** | The session is scoped to a branch — `SETTING.ini [setup] labbranchx=1` controls branch scoping of the session and of the accounting period | `SETTING.ini` | STRONGLY SUPPORTED | V1 and V2. **V2 extends this materially** — `Labbranch`, `Patient.branche` and a multi-branch stock-take. |
| **V1-BR-008** | Result entry is scoped to a clinical profile's analyte set — the operator reaches a discipline-specific window | CHM `004.htm`; 46 profile tables | CONFIRMED | V1 and V2 |
| **V1-BR-012** | The abnormality exception text is stored on the test row (`nolowexe` / `nohighexe`), not looked up at display time | `patienttest` schema | STRONGLY SUPPORTED | V1 and V2 |
| **V1-BR-018** | Exception text and critical comment are stored per range row, not per test | `normalrange` schema; CHM `008.htm` | CONFIRMED | V1 and V2 |
| **V1-BR-019** | The urine analysis's reference range is bound to the immutable test number 4 | CHM `nots.htm` + `008.htm` | STRONGLY SUPPORTED | V1 and V2 |
| **V1-BR-022** | Paper size is a global setting in `SETTING.ini`, not per profile | `SETTING.ini [setup] papersize=A4` | CONFIRMED | V1 and V2 |
| **V1-BR-028** | Database backup is copy-based, not a transactionally consistent snapshot — evidenced by the shipped backup having fewer tables, fewer columns and fewer rows than the shipped primary database | `Patients_backup.mdb` (90/1,381/2,786) vs `Patients.mdb` (93/1,527/3,123) | INFERRED | V1 and V2 (V2 ships the same stale backup — see the V2 catalogue) |
| **V1-BR-029** | Unit conversion factors are stored per analyte-and-unit-pair, not globally — e.g. Acetaminophen µg/mL→µmol/L 6.62, Albumin g/dL→g/L 10, Aldosterone ng/dL→nmol/L 0.0277 | `converts` (167 rows) | CONFIRMED | V1 and V2 |
| **V1-BR-030** | Abbreviations are maintained independently of the test catalogue | `abbreviations` (650 rows) as a separate table from `testtable` | STRONGLY SUPPORTED | V1 and V2 |
| **V1-BR-032** | The login group is the unit of session scoping and workload attribution, not a role hierarchy | `loggroup`; CHM `002.htm`; `labfunction` workload report | INFERRED | V1 and V2. **V2 adds `Patient.grant13..grant17` bit flags and `useraccounts.gtype/hdd1/hdd2`, extending the permission model.** |
| **V1-BR-034** | Barcode auto-creation (`barcodeauto=1`) and work-sheet printing (`printworkpaper=1`) are independently toggleable | `SETTING.ini [setup]` | CONFIRMED | V1 and V2 |
| **V1-BR-035** | Critical thresholds are stated in the method sheet, separately from `normalrange` — two independent sources of criticality | `Cache\*.rtf` vs `normalrange` | CONFIRMED | V1 and V2 |
| **V1-BR-036** | The training library is a separate executable set that does not touch the live database | 13 standalone Flash-hosted executables in `Cache\` | STRONGLY SUPPORTED | V1 and V2 — **byte-identical in both versions** |

### 3.11 Confirmed-absent rule

#### V1-BR-046 — PDF export capability is not present in this version

| Field | Content |
|---|---|
| **Rule name** | PDF export capability is not present in this version |
| **Domain** | Reporting |
| **Description** | Version 1 cannot produce a PDF. |
| **Evidence** | Exhaustive negative search of the Version 1 executable: **0** occurrences of the string `PDF` in 104,997 literals; no PDF-capable printer in `SETTING.ini` or any of the 223 `Data\*.ini` profile printer bindings; no PDF printer driver in the artefact set. The printing path is exclusively the Win32 spooler API (`OpenPrinterA`, `SetPrinterA`, `GetPrinterA`, `ClosePrinter`). |
| **Evidence type** | ET-DERIVED |
| **Confidence** | CONFIRMED |
| **Version applicability** | **CONFIRMED ABSENT in V-ROOT-1.00.** V-CD-2019 is **VERSION-SPECIFIC** the other way: it adds PDF export via a pinned external driver. See the V2 catalogue. |
| **Known limitations** | The search covers the executable and the shipped configuration. A PDF path loaded from an unregistered external component cannot be excluded absolutely, but no such component ships with Version 1. |

### 3.11 Operator permission and attendance rules

#### V1-BR-047 — Each operator carries twelve function-level permission bits

| Field | Content |
|---|---|
| **Rule name** | Each operator carries twelve function-level permission bits |
| **Arabic label** | — (schema field, no Arabic label) |
| **Domain** | Security |
| **Description** | The operator table `patients` carries **twelve Boolean permission flags `grant1` … `grant12`**, alongside `patientad` (administrator flag), `patientus` (active flag), `patientuscap` (role caption) and `patientword` (password). |
| **Business purpose** | A laboratory must be able to restrict what each role may do — record a result, settle an account, change a reference range, adjust a price. |
| **Trigger / applicable event** | Session start; every privileged action |
| **Preconditions** | — |
| **Actors and permissions** | The application checks the bits before allowing an action. |
| **Expected behaviour** | An action is offered or refused according to the operator's bits. |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **This corrects an earlier characterisation.** Version 1 is **not** a bare login gate — it has a twelve-bit function-level permission model. The mapping of each bit to a specific capability is **NOT ESTABLISHED**; only the bit count and the field names are evidenced. |
| **Related workflows** | WF-V1-01 |
| **Related functions** | F-V1-002, F-V1-020 |
| **Related entities** | `patients` |
| **Relevant fields** | `grant1`…`grant12`, `patientad`, `patientus`, `patientuscap` |
| **Related statuses** | — |
| **Evidence** | `analysis/BATCH-02/db/Patients/columns.csv` — the `patients` table, 26 columns |
| **Evidence type** | ET-DB |
| **Confidence** | CONFIRMED for the flags; UNKNOWN for their semantics |
| **Version applicability** | V-ROOT-1.00 **and** V-CD-2019. **V-CD-2019 extends the model to seventeen bits** — the v1.605 DDL adds `grant13`…`grant17` (plus `branche` and `SecWord`) to the **same operator table `patients`**. |
| **Known limitations** | Which capability each bit guards is UNKNOWN. |

#### V1-BR-048 — Operator attendance is recorded on the operator record itself

| Field | Content |
|---|---|
| **Rule name** | Operator attendance is recorded on the operator record itself |
| **Arabic label** | — (schema fields) |
| **Domain** | Security / Workforce |
| **Description** | The operator table carries `startwork` (Text 15), `endwork` (Text 15), `breakyorn` (Boolean) and `breaktime` (Text 15). |
| **Business purpose** | Establish who was present and when — the basis for any attendance-based management and for the per-user workload accounting. |
| **Trigger / applicable event** | Operator sign-in, sign-out, break start and end |
| **Preconditions** | Signed in |
| **Actors and permissions** | The application writes the timestamps; the operator's own actions drive them |
| **Expected behaviour** | The operator's current presence state is held on their record |
| **Restrictions** | The timestamps are **Text (15)**, not `DateTime` — a data-type inconsistency with the typed date columns elsewhere |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **This corrects an earlier characterisation.** Attendance tracking is **not** a V-CD-2019 addition — it exists in Version 1 on the operator record. V-CD-2019's separate `loger` table is therefore an **additional event log**, not the first appearance of attendance. |
| **Related workflows** | WF-V1-01 |
| **Related functions** | F-V1-002, F-V1-015 |
| **Related entities** | `patients` |
| **Relevant fields** | `startwork`, `endwork`, `breakyorn`, `breaktime` |
| **Related statuses** | — |
| **Evidence** | `analysis/BATCH-02/db/Patients/columns.csv` |
| **Evidence type** | ET-DB |
| **Confidence** | CONFIRMED for the fields; INFERRED for the write behaviour |
| **Version applicability** | V-ROOT-1.00 and V-CD-2019 |
| **Known limitations** | The write trigger is not directly evidenced. |

#### V1-BR-049 — Referring-party commission is applied as a percentage of the patient-level commission base

| Field | Content |
|---|---|
| **Rule name** | Referring-party commission is applied as a percentage of the patient-level commission base |
| **Arabic label** | «عمولات جهات الإحالة» |
| **Domain** | Finance |
| **Description** | The extracted SQL computes the commission as `SUM(Gehats.omola * patientinfo.elmadfoo3 / 100)` — the referring party's `omola` rate applied to the patient-level amount `elmadfoo3`, divided by 100. |
| **Business purpose** | Makes the referring party's commission proportional to the value of the work it generated. |
| **Trigger / applicable event** | Stock-take |
| **Preconditions** | The patient has an `elmadfoo3` value and a referring party with an `omola` rate. |
| **Actors and permissions** | Accounting user |
| **Expected behaviour** | The commission total is the sum over patients of rate × base ÷ 100 |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **The commission base is a separate field from `totalprice` and `totalpricedisc`** — the three amount fields on `patientinfo` are distinct, and only `elmadfoo3` participates in the commission formula. |
| **Related workflows** | WF-V1-04 |
| **Related functions** | F-V1-015, F-V1-022 |
| **Related entities** | `Gehats`, `patientinfo` |
| **Relevant fields** | `Gehats.omola`, `patientinfo.elmadfoo3` |
| **Related statuses** | — |
| **Evidence** | Extracted SQL literal — `analysis/BATCH-01/sql/statements.csv` |
| **Evidence type** | ET-CODE |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00; the same fields exist in V-CD-2019 |
| **Known limitations** | The meaning of `elmadfoo3` in business terms (what the base actually represents) is NOT ESTABLISHED — only its role in the formula is. |

---

## 4. Rules NOT established for V-ROOT-1.00

The following are candidate rules that the available evidence neither confirms nor refutes.
They are recorded so that a reader does not mistake their absence from §2–§3 for a
negative finding.

| ID | Candidate rule | Why not established | What would settle it |
|---|---|---|---|
| RL-V1-BR-U01 | Overlapping reference ranges for the same analyte and sex are rejected | No validation logic is recoverable statically | Runtime observation of the range editor |
| RL-V1-BR-U02 | Duplicate patients are detected on registration | No uniqueness rule evidenced | Runtime observation of the add-patient window |
| RL-V1-BR-U03 | A test must have a range before it can be ordered | No ordering-time check evidenced | Runtime observation |
| RL-V1-BR-U04 | `pregsmok` participates in range selection | The field is populated on all 852 rows but its consumer was not traced | Code-path trace of the range lookup |
| RL-V1-BR-U05 | The range lookup selects the most specific matching band | Multiple candidate algorithms fit the schema | Code-path trace |
| RL-V1-BR-U06 | Unit conversion is applied automatically on entry | Not determinable from the data alone | Code-path trace of the conversion lookup |
| RL-V1-BR-U07 | The derived Hct is stored rather than computed on display | Both are consistent with the schema | Code-path trace |
| RL-V1-BR-U08 | A second signature is required to release a critical result | No such mechanism evidenced, but absence of evidence is not absence | Runtime observation of the hand-over path |
| RL-V1-BR-U09 | The `history` attribute slots have a fixed canonical order | The schema is free-form | Runtime observation |
| RL-V1-BR-U10 | The `zarfprinter` role has a distinct output format | The role exists but no format is evidenced | Runtime observation |

---

## 5. Version-specific rule summary

| Rule family | V-ROOT-1.00 | V-CD-2019 |
|---|---|---|
| Clinical computation constants (`CBCCONS`, `PTCONS`) | Present, values identical | Present, values identical |
| Reference-range banding model | Present | Present, unchanged |
| Criticality from method sheets | Present (29 sheets) | Present (29 sheets, byte-identical) |
| Server-configuration mutability | Present | Present (CHM byte-identical) |
| `admin` default credential | Present | Present (CHM byte-identical) |
| Cleartext database credential (`sa` / `12345678`) | Present | Present |
| **PDF export** | **ABSENT** | **PRESENT — pinned external driver** |
| **Branch-scoped accounting** | **ABSENT** | **PRESENT** |
| **Attendance log** | **ABSENT** | **PRESENT** |
| **Per-user permission bit flags (`grant13..grant17`)** | **ABSENT** | **PRESENT** |
| **Statistics module** | **ABSENT** | **PRESENT** |
| **Work-sheet module** | **ABSENT** | **PRESENT** |
| **SMS / NATIGH result delivery** | **ABSENT** | **PRESENT** |
| **Embedded schema migration** | **ABSENT** | **PRESENT** |
| **Test-number-4 immutability** | Present | Present (identical CHM) |

---

**End of Business_Rules_V1.md**
