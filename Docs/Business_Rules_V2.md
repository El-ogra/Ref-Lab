# Business Rules — RealLab V-CD-2019 (v1.605)

**Document ID:** RL-V2-BR-001
**Version documented:** `V-CD-2019`
**Evidence basis:** 473 verified artefacts · static analysis only
**Companion documents:** `Functional_Specification_V2.md`, `Data_Model_and_Workflow_Entities_V2.md`

---

## 1. How to read this catalogue

### 1.1 Identifier scheme

`V2-BR-nnn` — stable, version-scoped, never re-used. Where a rule also applies to
Version 1, the Version 1 identifier is given in the **Version applicability** field,
and the two documents carry independent entries so that neither inherits from the
other.

### 1.2 Evidence-type classification

| Code | Meaning |
|---|---|
| **ET-DOC** | Stated in a shipped document (`RLS_Learn.pdf`, `RLS_Setup.pdf`, `help.chm`) |
| **ET-DOC+DB** | Stated in a shipped document **and** present as stored data |
| **ET-DB** | Present as stored data in the extracted schema |
| **ET-CODE** | Present in the executable's literals, SQL or object model |
| **ET-DDL** | Present in the 268 embedded schema-migration statements |
| **ET-CFG** | Present in a shipped configuration file |
| **ET-INST** | Present in the installation media or installer |
| **ET-DERIVED** | Derived by cross-comparing two evidence classes |

### 1.3 Confidence

CONFIRMED · STRONGLY SUPPORTED · INFERRED · UNKNOWN · CONFLICTING

### 1.4 Rule status

**CONFIRMED PRESENT** · **CONFIRMED ABSENT** · **VERSION-SPECIFIC** ·
**NOT ESTABLISHED** · **CONFLICTING**

> **Absence of evidence is not evidence of absence.** Every **CONFIRMED ABSENT**
> entry names the search that established it.

---

## 2. Rule index

| ID | Rule name | Domain | Evidence type | Confidence | Status |
|---|---|---|---|---|---|
| V2-BR-001 | The PDF printer device name is a hard-coded literal | Reporting / PDF | ET-CODE | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-002 | The default PDF output filename is version-pinned to Bullzip 11.1 | Reporting / PDF | ET-CODE | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-003 | PDF output is filed into exactly one of four categories | Reporting / PDF | ET-CODE | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-004 | PDF output depends on an externally installed printer driver | Reporting / PDF | ET-INST | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-005 | The NATIGH delivery flag is defined by the DDL but absent from the shipped database | Data / Delivery | ET-DDL | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-006 | Digital delivery requires an outbound internet channel the manual advises against | Delivery / Security | ET-DOC | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-007 | Every SMS is queued and auditable | Delivery | ET-DDL | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-008 | The branch code occupies position 9 of the patient identifier | Data / Branches | ET-CODE | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-009 | A branch's schema is seeded from a schema-only template with no data | Branches | ET-DB | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-010 | A stock-take may span a master laboratory and its linked branches | Accounting | ET-DOC | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-011 | Statistics are computed from the transactional tables, not from an aggregate store | Statistics | ET-CODE | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-012 | The attendance register is a distinct structure from the login-group table | Security / Workforce | ET-DOC+DB | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-013 | Work-sheet printing is independently toggleable | Reporting | ET-CFG | CONFIRMED | CONFIRMED PRESENT |
| V2-BR-014 | Farm reporting is a parallel reporting vertical | Reporting | ET-DOC | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-015 | Two antibiotic structures coexist | Clinical knowledge | ET-DB | CONFIRMED | CONFIRMED PRESENT |
| V2-BR-016 | Sample disposition is recorded against the microbiology tables | Laboratory logistics | ET-DOC+DB | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-017 | Cross-profile consolidation requires the `MSDataShape.1` provider | Reporting | ET-CODE | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-018 | The price list is a distinct artefact from the per-test price fields | Finance / Catalogue | ET-DOC+DB | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-019 | The account type is a single system-wide default | Finance | ET-DOC+DB | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-020 | Report margins are stored per profile while paper size is global | Reporting | ET-DOC+CFG | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-021 | Global header texts coexist with per-profile header texts | Reporting | ET-DOC+CFG | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-022 | NATIGH accounts are provisioned for external parties | Security / Delivery | ET-DOC | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-023 | The runtime schema migration is applied only partially | Data | ET-DDL+DB | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-024 | The migration DDL is SQL Server syntax and cannot execute against Jet 4 | Data | ET-DDL | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-025 | Two branch templates ship on the 2019 CD; the one in `bin\` is byte-identical to Version 1 | Branches / Data | ET-DB | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-026 | The shipped database backup is stale | Administration | ET-DB | CONFIRMED | CONFIRMED PRESENT |
| V2-BR-027 | Three database engines are supported | Deployment | ET-INST | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-028 | Two SQL catalogues plus a third attached database | Deployment | ET-INST | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-029 | Installation self-registers 24 ActiveX components and overwrites system files | Deployment / Security | ET-INST | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-030 | Two conflicting builds of the same runtime components ship on one disc | Deployment | ET-DB | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-031 | The PDF installation order is defective | Deployment | ET-CODE+INST | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-032 | The database credential is shared, static and stored in clear text | Security | ET-CFG | CONFIRMED | CONFIRMED PRESENT |
| V2-BR-033 | The official user manual is not updated for this version | Documentation | ET-DB | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-034 | Result classification is driven by the reference-range table | Results | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V2-BR-035 | A critical limit overrides the abnormal classification | Results | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V2-BR-036 | Reference ranges are banded by sex and age | Clinical knowledge | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V2-BR-037 | Clinical computation constants are held as data | Clinical | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V2-BR-038 | Only reviewed results are eligible for release | Results / Quality | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V2-BR-039 | The operator carries seventeen function-level permission bits, and the operator's branch assignment | Security | ET-DOC+DB | CONFIRMED | VERSION-SPECIFIC |
| V2-BR-040 | The patient identifier is an order/visit identifier with positional segments | Patient | ET-CODE | CONFIRMED | CONFIRMED PRESENT |
| V2-BR-041 | Operator attendance is recorded on the operator record | Security / Workforce | ET-DB | CONFIRMED | CONFIRMED PRESENT |
| V2-BR-042 | Referring-party commission is a percentage of a patient-level base | Finance | ET-CODE | CONFIRMED | CONFIRMED PRESENT |
| V2-BR-043 | The test number 4 restriction remains in force | Catalogue | ET-DOC | CONFIRMED | CONFIRMED PRESENT |
| V2-BR-044 | Send-out enrolment is flag-driven with two settlement states | Send-out | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V2-BR-045 | Printed output is audited with a per-operator, dated, sequenced record | Reporting / Audit | ET-DB | CONFIRMED | CONFIRMED PRESENT |

---

## 3. Detailed rule specifications

### 3.1 PDF export rules

#### V2-BR-001 — The PDF printer device name is a hard-coded literal

| Field | Content |
|---|---|
| **Rule name** | The PDF printer device name is a hard-coded literal |
| **Arabic label** | — (Latin product name) |
| **Domain** | Reporting / PDF |
| **Description** | The application resolves the PDF printer by the literal string `Bullzip PDF Printer`, stored in the executable's string table at offset `0x6A2486` in UTF-16LE. |
| **Business purpose** | *(none — this is a coupling artefact, not a designed behaviour)* |
| **Trigger / applicable event** | Every PDF print request |
| **Preconditions** | A Windows printer with exactly that name must exist. |
| **Actors and permissions** | The application |
| **Expected behaviour** | The PDF job is dispatched to the printer named `Bullzip PDF Printer`. |
| **Restrictions** | The name must match **exactly**, including capitalisation. |
| **Validation conditions** | None — no fallback lookup is evidenced. |
| **Exceptions** | «Can't find any printer.» |
| **Consequences** | **Renaming, uninstalling or replacing the Bullzip printer silently breaks the PDF export path.** A laboratory cannot substitute a different PDF printer without editing the executable. |
| **Related workflows** | WF-V2-03 |
| **Related functions** | F-V2-028 |
| **Related entities** | — (a literal, not a table) |
| **Relevant fields** | — |
| **Related statuses** | — |
| **Evidence** | UTF-16LE literal `Bullzip PDF Printer` at `0x6A2486` in `real lab system.exe` v1.605 — `analysis/BATCH-04/vb6/v1605/literals.tsv` |
| **Evidence type** | ET-CODE |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC.** **Confirmed ABSENT in V-ROOT-1.00** — zero occurrences of `PDF` in 104,997 literals (V1-BR-046). |
| **Known limitations** | Whether a printer-rename tolerance exists elsewhere in the code was not established. |

#### V2-BR-002 — The default PDF output filename is version-pinned to Bullzip 11.1

| Field | Content |
|---|---|
| **Rule name** | The default PDF output filename is version-pinned to Bullzip 11.1 |
| **Arabic label** | — |
| **Domain** | Reporting / PDF |
| **Description** | The literal `Bullzip PDF Printer 11.1 - Create File` at offset `0x6A255E` supplies the default PDF output filename. The string embeds the driver's **version number 11.1**. |
| **Business purpose** | *(none — a coupling artefact)* |
| **Trigger / applicable event** | PDF print request |
| **Preconditions** | — |
| **Actors and permissions** | — |
| **Expected behaviour** | The output file is named from that string. |
| **Restrictions** | The installed Bullzip version must be 11.1 for the default name to correspond to anything meaningful. |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **Upgrading Bullzip to 12 or later silently invalidates the default filename.** This is a version-pinned external dependency expressed as a string literal. |
| **Related workflows** | WF-V2-03 |
| **Related functions** | F-V2-028 |
| **Related entities** | — |
| **Relevant fields** | — |
| **Related statuses** | — |
| **Evidence** | UTF-16LE literal at `0x6A255E` — `analysis/BATCH-04/vb6/v1605/literals.tsv` |
| **Evidence type** | ET-CODE |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC** |
| **Known limitations** | The exact filename construction (this string plus a timestamp, or the string alone) is NOT ESTABLISHED. |

#### V2-BR-003 — PDF output is filed into exactly one of four categories

| Field | Content |
|---|---|
| **Rule name** | PDF output is filed into exactly one of four categories |
| **Arabic label** | — (the directory names are Latin) |
| **Domain** | Reporting / PDF |
| **Description** | The application files PDFs under `D:\PDF\` into one of four sub-directories: `Contracts\`, `Free\`, `Individual\`, `LabToLab\`. The literal `Lab To Lab` is the display form; `LabToLab` is the directory name. |
| **Business purpose** | Files the output by the laboratory's commercial relationship with the case. |
| **Trigger / applicable event** | PDF print request |
| **Preconditions** | The root `D:\PDF\` and its sub-directories exist or are creatable. |
| **Actors and permissions** | Lab operator selects the category |
| **Expected behaviour** | The PDF lands in the selected category directory. |
| **Restrictions** | The root is **hard-coded to `D:\PDF\`** — it is not configurable through the observed settings. |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | The output taxonomy mirrors the accounting taxonomy (contracts / free / individual / lab-to-lab), so **the PDF destination is a commercial classification, not a functional one**. |
| **Related workflows** | WF-V2-03 |
| **Related functions** | F-V2-028, F-V2-042 |
| **Related entities** | `patientinfo.pricemethod`, `Gehats.pricemethod` (the parallel accounting taxonomy) |
| **Relevant fields** | `pricemethod` |
| **Related statuses** | — |
| **Evidence** | Literals `D:\PDF\`, `D:\PDF\Contracts\`, `D:\PDF\Free\`, `D:\PDF\Individual\`, `D:\PDF\LabToLab\`, `Contracts`, `Free`, `Individual`, `Lab To Lab`, `LabToLab` |
| **Evidence type** | ET-CODE |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC** |
| **Known limitations** | The mapping between a `pricemethod` value and a PDF directory is NOT ESTABLISHED — the `pricemethod` value domain itself is not established. |

#### V2-BR-004 — PDF output depends on an externally installed printer driver

| Field | Content |
|---|---|
| **Rule name** | PDF output depends on an externally installed printer driver |
| **Arabic label** | — |
| **Domain** | Reporting / PDF / Deployment |
| **Description** | The application contains **no PDF logic**. PDF production is performed entirely by the Bullzip PDF Printer 11.1 driver, its port DLL `bzpdf.dll`, and the Ghostscript copy bundled inside it. The application contributes only the print job and the output directory. |
| **Business purpose** | *(none — an architectural dependency)* |
| **Trigger / applicable event** | Every PDF print request |
| **Preconditions** | Bullzip installed; ‹.NET Framework 4.0› installed; the driver registered with Windows. |
| **Actors and permissions** | The installation process provisions the driver. |
| **Expected behaviour** | The driver renders the PostScript into a PDF. |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | «Can't find any printer.» |
| **Consequences** | **The PDF capability can be replaced wholesale without touching application logic** — the integration surface is exactly two string literals and a directory path. The reverse is not true: the application cannot substitute a different PDF engine at run time. |
| **Related workflows** | WF-V2-03 |
| **Related functions** | F-V2-028 |
| **Related entities** | — |
| **Relevant fields** | — |
| **Related statuses** | — |
| **Evidence** | Literal analysis (0 PDF-generation symbols in the executable); `analysis/BATCH-11/pe-envelope.csv`; the Bullzip Inno payload (`bzpdf.dll`, `app/gs/`) |
| **Evidence type** | ET-INST |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC** |
| **Known limitations** | — |

### 3.2 Digital delivery rules

#### V2-BR-005 — The NATIGH delivery flag is defined by the DDL but absent from the shipped database

| Field | Content |
|---|---|
| **Rule name** | The NATIGH delivery flag is defined by the DDL but absent from the shipped database |
| **Arabic label** | — |
| **Domain** | Data / Delivery |
| **Description** | The embedded DDL contains an `ALTER TABLE patienttest ADD NatighU` statement. Reconciliation against the shipped `Patients.mdb` shows **the column is not present**. |
| **Business purpose** | The flag would record that a result was published through NATIGH.COM. |
| **Trigger / applicable event** | Application start-up (migration), and NATIGH publication |
| **Preconditions** | — |
| **Actors and permissions** | — |
| **Expected behaviour** | The delivery flag is recorded on the test row. |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **★ The NATIGH delivery feature is code-present but schema-absent in the shipped Access database.** Against this database the feature cannot record its own state. Whether it degrades gracefully or raises an error is NOT ESTABLISHED. |
| **Related workflows** | WF-V2-07 |
| **Related functions** | F-V2-029 |
| **Related entities** | `patienttest` |
| **Relevant fields** | `NatighU` |
| **Related statuses** | — |
| **Evidence** | `analysis/BATCH-04/schema/added-columns.csv`; reconciliation against `analysis/BATCH-07/db/v2019-Patients.mdb.sql` |
| **Evidence type** | ET-DDL |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC** |
| **Known limitations** | The runtime failure mode is NOT ESTABLISHED. |

#### V2-BR-006 — Digital delivery requires an outbound internet channel the manual advises against

| Field | Content |
|---|---|
| **Rule name** | Digital delivery requires an outbound internet channel the manual advises against |
| **Arabic label** | «تسليم نتائج المرضى عن طريق موقع نتتيجة (NATIGH.COM)» · the manual's «يفضل عدم توصيل انترنت على جهاز السيرفر للحفاظ علي نظام التشغيل أطول فترة ممكنة وللحفاظ على سرية البيانات من احتمالية الأختراق» |
| **Domain** | Delivery / Security |
| **Description** | Version 2 adds two internet-dependent delivery channels — NATIGH.COM publication and SMS notification — while the shipped manual (byte-identical to Version 1) advises keeping the server machine off the internet to preserve confidentiality and operating-system stability. |
| **Business purpose** | Remote result delivery; the guidance is a legacy operational caution. |
| **Trigger / applicable event** | Application start; delivery actions |
| **Preconditions** | Outbound internet connectivity from the server machine |
| **Actors and permissions** | System administrator governs the network posture. |
| **Expected behaviour** | Delivery works; the guidance says it should not. |
| **Restrictions** | The two conflict. |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **A documented internal contradiction.** Either the guidance is obsolete for the 2020 deployment, or the delivery features are unsafe on a compliant installation. **The evidence does not resolve which.** |
| **Related workflows** | WF-V2-07 |
| **Related functions** | F-V2-029, F-V2-030 |
| **Related entities** | `Natigh`, `SMSSender` |
| **Relevant fields** | — |
| **Related statuses** | — |
| **Evidence** | Vendor guide 1-12; `help.chm` topic `001.htm` (byte-identical to V1) |
| **Evidence type** | ET-DOC |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC** — the contradiction is created by V2's additions against V1's guidance. |
| **Known limitations** | — |

#### V2-BR-007 — Every SMS is queued and auditable

| Field | Content |
|---|---|
| **Rule name** | Every SMS is queued and auditable |
| **Arabic label** | — |
| **Domain** | Delivery / Audit |
| **Description** | Outbound SMS messages are written to the `SMSSender` table with the recipient mobile number (`smsMobile`), the full text (`SMStext`), a length (`SMSLength`), the patient (`patientid`), the doctor's name (`PatDrName`) and a timestamp (`requestdate`). |
| **Business purpose** | Keeps a record of what was sent to whom, and when. |
| **Trigger / applicable event** | SMS dispatch |
| **Preconditions** | — |
| **Actors and permissions** | Lab operator |
| **Expected behaviour** | A queue row precedes or accompanies the send. |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **The queue stores a mobile number and a message body in clear text, indefinitely, with no evidenced retention limit.** In a laboratory context a mobile number is a personal identifier. |
| **Related workflows** | WF-V2-07 |
| **Related functions** | F-V2-030 |
| **Related entities** | `SMSSender`, `SMStemplate`, `smslist`, `smsgroups` |
| **Relevant fields** | `smsMobile`, `SMStext`, `SMSLength`, `PatDrName` |
| **Related statuses** | — |
| **Evidence** | DDL for `SMSSender`; the `SMSfrmsenderos` form (139 methods) |
| **Evidence type** | ET-DDL |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC** |
| **Known limitations** | The gateway provider, protocol and delivery-status handling are NOT ESTABLISHED. |

### 3.3 Branch rules

#### V2-BR-008 — The branch code occupies position 9 of the patient identifier

| Field | Content |
|---|---|
| **Rule name** | The branch code occupies position 9 of the patient identifier |
| **Arabic label** | «نظام الفروع للمعامل» |
| **Domain** | Data / Branches |
| **Description** | The SQL extracts the branch with `MID(patientid,9,1) as bran`. The identifier is 13 characters, so position 9 is the ninth character. The same identifier carries the **person key at positions 3–4** (`MID(patientid,3,2)`) and a **display code at positions 10–12** (`mid(patientid,10,3) as patid`). **The branch value written there comes from the signed-in operator's own branch** (`patients.branche`, Integer) — the branch is a property of the workstation's operator, not of the patient. |
| **Business purpose** | Lets one shared database serve several branches while each record still carries its branch. |
| **Trigger / applicable event** | Every read or write of a patient record |
| **Preconditions** | The identifier must be allocated with the branch digit in position 9. |
| **Actors and permissions** | The application allocates and interprets the identifier. |
| **Expected behaviour** | The branch is recoverable from the identifier without a separate lookup. |
| **Restrictions** | The identifier layout is a **hard inter-version contract** — reformatting it breaks branch attribution and the person key simultaneously. |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **★ The most consequential data contract in the product.** A record imported without its branch digit in position 9 is silently attributed to the wrong branch. |
| **Related workflows** | WF-V2-02 |
| **Related functions** | F-V2-031, F-V2-003 |
| **Related entities** | **`patients` (the operator table, which carries `branche`)**, `patientinfo`, `patienttest`, `Labbranch` |
| **Relevant fields** | `patientid` (position 9), **`patients.branche`** |
| **Related statuses** | — |
| **Evidence** | `MID(patientid,9,1) as bran` in the extracted SQL; `Patient.branche`; `Labbranch` |
| **Evidence type** | ET-CODE |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC.** Version 1 has positional identifier semantics but **no documented branch segment** — see V1-BR-005. |
| **Known limitations** | The remaining identifier segments are not established. |

#### V2-BR-009 — A branch's schema is seeded from a schema-only template with no data

| Field | Content |
|---|---|
| **Rule name** | A branch's schema is seeded from a schema-only template with no data |
| **Arabic label** | — |
| **Domain** | Branches / Data |
| **Description** | `Branch_Data.mdb` contains **49 tables with 0 rows in every table**. It is a pure schema carrier used to create a branch's profile tables. |
| **Business purpose** | Lets a new branch be created without a manual schema build. |
| **Trigger / applicable event** | Branch creation |
| **Preconditions** | — |
| **Actors and permissions** | System administrator |
| **Expected behaviour** | The template's tables are created in the branch's database, empty. |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | The template carries **no reference data**, so a new branch starts with no test catalogue, no ranges and no prices until those are loaded separately. |
| **Related workflows** | WF-V2-02 |
| **Related functions** | F-V2-031 |
| **Related entities** | `Branch_Data.mdb` (49 tables) |
| **Relevant fields** | — |
| **Related statuses** | — |
| **Evidence** | `analysis/BATCH-02/db/Branch_Data/rowcounts.csv` — 0 rows in every table; `analysis/BATCH-04/schema/` |
| **Evidence type** | ET-DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC** for the branch-seeding purpose; the template file itself also ships in Version 1 |
| **Known limitations** | The mechanism by which the schema is applied is NOT ESTABLISHED. |

#### V2-BR-025 — Two branch templates ship on the 2019 CD; the one in `bin\` is byte-identical to Version 1

| Field | Content |
|---|---|
| **Rule name** | Two branch templates ship on the 2019 CD; the one in `bin\` is byte-identical to Version 1 |
| **Arabic label** | — |
| **Domain** | Branches / Data |
| **Description** | The 2019 CD ships **two** copies of `Branch_Data.mdb`: the `bin\` copy (970,752 B, SHA-256 `f6d6d5e2…`) is **byte-identical to the Version 1 file**, and the separate `System\` copy (966,656 B, SHA-256 `e6c3953e…`) is the **narrower** one whose `patienttest` carries 35 columns against the live 46. **★ CORRECTION to the earlier narrative:** the "template narrowed by 9 columns" conclusion was drawn against the `System\` copy, not against the copy the application actually loads from `bin\`. Which copy the running application uses is **NOT ESTABLISHED**. |
| **Business purpose** | The template is the **pre-migration** shape; the application brings it up to date at install time. |
| **Trigger / applicable event** | Branch creation |
| **Preconditions** | — |
| **Actors and permissions** | The application |
| **Expected behaviour** | The template is applied, then the DDL adds the missing columns. |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **The branch-seeding path depends on which template the application loads.** If it loads the narrower `System\` copy, a new branch depends entirely on the DDL succeeding to become schema-compatible with the live database. Which copy is loaded is **NOT ESTABLISHED**. |
| **Related workflows** | WF-V2-02, WF-V2-04 |
| **Related functions** | F-V2-031 |
| **Related entities** | `Branch_Data.mdb` (**two copies**), `patienttest` |
| **Relevant fields** | The 9 columns absent from the narrower `System\` template: `highlimit`, `historyname`, `lowlimit`, `nohighexe`, `nolowexe`, `normalrang`, `testabrev`, `testname`, `testunit` |
| **Related statuses** | — |
| **Evidence** | `analysis/BATCH-04/schema/column-evolution.csv`; the template and live schemas |
| **Evidence type** | ET-DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC** — the 2013 `bin\` template is byte-identical to the 2019 `bin\` template; the narrowing exists only in the additional 2019 `System\` copy |
| **Known limitations** | — |

#### V2-BR-010 — A stock-take may span a master laboratory and its linked branches

| Field | Content |
|---|---|
| **Rule name** | A stock-take may span a master laboratory and its linked branches |
| **Arabic label** | «عمل جرد على فرع ضمن عدة فروع مرتبطة معا (نظام الفروع للمعامل)» |
| **Domain** | Accounting |
| **Description** | The stock-take scope can be a single laboratory or a set of linked branches. The version-1 stock-take asks only whether collections are restricted to the period's patients; version 2 adds a **branch dimension** to the same question. |
| **Business purpose** | Supports group laboratories that reconcile several sites together. |
| **Trigger / applicable event** | Period stock-take |
| **Preconditions** | Branches registered in `Labbranch` and connected |
| **Actors and permissions** | Master-laboratory manager |
| **Expected behaviour** | The figures cover the selected branches. |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **Financial reporting can be produced at a group level.** The accounting model therefore has two aggregation dimensions — time period and branch — and both are optional operator decisions. |
| **Related workflows** | WF-V2-02 |
| **Related functions** | F-V2-015, F-V2-031 |
| **Related entities** | `payment`, `patientinfo`, `patienttest`, `Labbranch` |
| **Relevant fields** | `patientid` position 9, **`patients.branche`** |
| **Related statuses** | — |
| **Evidence** | Vendor guide 5-2 |
| **Evidence type** | ET-DOC |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC** |
| **Known limitations** | The branch-filtering mechanism is NOT ESTABLISHED. |

### 3.4 Schema migration rules

#### V2-BR-023 — The runtime schema migration is applied only partially

| Field | Content |
|---|---|
| **Rule name** | The runtime schema migration is applied only partially |
| **Arabic label** | — |
| **Domain** | Data / Deployment |
| **Description** | The executable embeds **268 DDL statements**. Reconciled against the shipped `Patients.mdb`: of **12 `CREATE TABLE` targets, 10 are present and 2 are absent** (`FMF`, `LBC`); of **132 distinct `ALTER TABLE ADD` column targets, 109 are present and 19 are absent**. Two of the absent targets belong to tables entirely absent from the database (`IgEallergy`, `useraccounts`). |
| **Business purpose** | Keep the database schema in step with the executable across versions. |
| **Trigger / applicable event** | Application start-up |
| **Preconditions** | Write access to the database |
| **Actors and permissions** | The application |
| **Expected behaviour** | The schema is brought up to date automatically. |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **★ The shipped Access database is a partial deployment of the v1.605 schema.** 21 migration objects were never applied. The features depending on them — IgE allergy reporting, the permission-bit model on `useraccounts`, the extended blood-gas panel, NATIGH delivery recording — have **no storage** in the shipped file. |
| **Related workflows** | WF-V2-04 |
| **Related functions** | F-V2-029, F-V2-037, F-V2-039 |
| **Related entities** | 29 affected tables |
| **Relevant fields** | See `Data_Model_and_Workflow_Entities_V2.md` §6.9 |
| **Related statuses** | — |
| **Evidence** | `analysis/BATCH-04/schema/created-tables.csv`, `added-columns.csv` reconciled against `analysis/BATCH-07/db/v2019-Patients.mdb.sql` |
| **Evidence type** | ET-DDL |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC** — Version 1 has zero DDL |
| **Known limitations** | Whether the migration is retried on a later run is NOT ESTABLISHED. |

#### V2-BR-024 — The migration DDL is SQL Server syntax and cannot execute against Jet 4

| Field | Content |
|---|---|
| **Rule name** | The migration DDL is SQL Server syntax and cannot execute against Jet 4 |
| **Arabic label** | — |
| **Domain** | Data / Deployment |
| **Description** | The DDL uses SQL Server constructs: `bigint IDENTITY(1,1) NOT NULL`, `varchar(MAX)`, `sp_rename`, and SQL Server's `ALTER COLUMN` type-change form. The shipped schema shows **both variants** of each statement — the SQL Server form and an Access-compatible form (`AUTOINCREMENT(1, 1)`, `varchar(255)`) — indicating the application attempts the SQL Server statement and then an Access one. |
| **Business purpose** | Support two database back ends with one code path. |
| **Trigger / applicable event** | Application start-up |
| **Preconditions** | — |
| **Actors and permissions** | — |
| **Expected behaviour** | The applicable variant executes for the active engine. |
| **Restrictions** | Where only the SQL Server form exists, the change cannot be applied to Jet 4. |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **★ On an Access installation, roughly one in six of the column additions cannot be applied at all.** The dual-statement pattern shows the vendor knew the two dialects differ; the residual gap is where only one variant was written. |
| **Related workflows** | WF-V2-04 |
| **Related functions** | — |
| **Related entities** | — |
| **Relevant fields** | — |
| **Related statuses** | — |
| **Evidence** | `analysis/BATCH-04/schema/ddl-all.txt` — SQL Server and Access variants of the same statement |
| **Evidence type** | ET-DDL |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC** |
| **Known limitations** | The exact failure-handling path (silent skip vs logged error) is NOT ESTABLISHED. |

### 3.5 Reporting rules

#### V2-BR-017 — Cross-profile consolidation requires the `MSDataShape.1` provider

| Field | Content |
|---|---|
| **Rule name** | Cross-profile consolidation requires the `MSDataShape.1` provider |
| **Arabic label** | «عمل تقرير مجمع» |
| **Domain** | Reporting |
| **Description** | The consolidated report is produced with `SHAPE {…} APPEND (…)` union queries. The SQL corpus contains **20 statements using `SHAPE`, 21 `APPEND` occurrences and 3 `RELATE` occurrences**. `SHAPE`/`APPEND` is a `MSDataShape.1` construct, and `data_connection1.udl` configures `Provider=MSDataShape.1`. |
| **Business purpose** | Combines results from several profile tables into one report. |
| **Trigger / applicable event** | Consolidated report request |
| **Preconditions** | The shape provider must be configured and reachable. |
| **Actors and permissions** | Lab operator |
| **Expected behaviour** | The results are merged into one dataset and reported. |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **★ The capability appears bound to the SQL Server path.** `data_connection1.udl` targets `Initial Catalog=Patients` with `Data Provider=SQLOLEDB.1`, i.e. SQL Server. Whether consolidated reporting works against the Access default is NOT ESTABLISHED. |
| **Related workflows** | — |
| **Related functions** | F-V2-039 |
| **Related entities** | `anyreport` |
| **Relevant fields** | `text1`…`text22` |
| **Related statuses** | — |
| **Evidence** | The `SHAPE`/`APPEND` statements in `analysis/BATCH-01/sql/statements.csv`; `data_connection1.udl` |
| **Evidence type** | ET-CODE |
| **Confidence** | CONFIRMED for the mechanism; UNKNOWN for the Access behaviour |
| **Version applicability** | CONFIRMED PRESENT in V-ROOT-1.00 as well — the same 20/21/3 counts were recovered from the V1 SQL corpus. **The V2 novelty is the documented vendor capability, not the mechanism.** |
| **Known limitations** | The Access behaviour is NOT ESTABLISHED. |

#### V2-BR-020 — Report margins are stored per profile while paper size is global

| Field | Content |
|---|---|
| **Rule name** | Report margins are stored per profile while paper size is global |
| **Arabic label** | «إعدادات هوامش التقرير» · «حجم الورق المستخدم في الطباعة» |
| **Domain** | Reporting |
| **Description** | Four margin values (`leftvalue`, `rightvalue`, `topvalue`, `bottomvalue`) are stored in each profile's `Data\*.ini`; the paper size (`papersize=A4`) is stored once in `SETTING.ini`. |
| **Business purpose** | Different disciplines can need different printable areas on a shared printer, while the paper itself is a laboratory-wide fact. |
| **Trigger / applicable event** | Report setup / configuration change |
| **Preconditions** | Administrator rights |
| **Actors and permissions** | Laboratory administrator |
| **Expected behaviour** | Per-profile margins apply; the paper size applies to all. |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **A printer change that alters the paper size invalidates every profile's margins simultaneously** — 223 files, no batch mechanism. |
| **Related workflows** | — |
| **Related functions** | F-V2-045, F-V2-021 |
| **Related entities** | `Data\*.ini`, `SETTING.ini` |
| **Relevant fields** | `leftvalue`, `rightvalue`, `topvalue`, `bottomvalue`, `papersize` |
| **Related statuses** | — |
| **Evidence** | Vendor guide 6-3, 6-4; `Data\*.ini` keys; `SETTING.ini` |
| **Evidence type** | ET-DOC+CFG |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC** — the margin keys are documented as a V2 capability |
| **Known limitations** | The unit of the margin values is NOT ESTABLISHED. |

#### V2-BR-021 — Global header texts coexist with per-profile header texts

| Field | Content |
|---|---|
| **Rule name** | Global header texts coexist with per-profile header texts |
| **Arabic label** | «إعدادات رأس وذيل التقرير» |
| **Domain** | Reporting |
| **Description** | `SETTING.ini` holds two global header strings — `Textr1txt=المنظومة البرمجية`, `Textr2txt=ريال لاب سيستم` — while each profile's `Data\*.ini` holds its own `formsubtitle` and `reportmaintitle`. |
| **Business purpose** | A laboratory-wide identity plus per-discipline subtitles. |
| **Trigger / applicable event** | Configuration change |
| **Preconditions** | Administrator rights |
| **Actors and permissions** | Laboratory administrator |
| **Expected behaviour** | Both global and per-profile header texts appear on the report. |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **★ The shipped global header text is the vendor's own product identity** — «المنظومة البرمجية» ("the software system") and «ريال لاب سيستم» ("RealLab System"). Any laboratory using the product unchanged **prints the vendor's branding on every report.** This is a configuration-content exposure, and it is one of the clearest Arabic strings in the entire evidence base. |
| **Related workflows** | — |
| **Related functions** | F-V2-046, F-V2-013 |
| **Related entities** | `SETTING.ini`, `Data\*.ini` |
| **Relevant fields** | `Textr1txt`, `Textr2txt`, `formsubtitle`, `reportmaintitle` |
| **Related statuses** | — |
| **Evidence** | `SETTING.ini`; `Data\*.ini` keys; vendor guide 6-5 |
| **Evidence type** | ET-DOC+CFG |
| **Confidence** | CONFIRMED |
| **Version applicability** | CONFIRMED PRESENT in V-ROOT-1.00 as well — the global header texts are identical in both versions. **The V2 novelty is the documented per-profile header keys.** |
| **Known limitations** | The precedence and layout order of the two levels is NOT ESTABLISHED. |

### 3.6 Knowledge-layer rules — applicable to both versions

> These rules are documented in both catalogues independently. **Neither version inherits
> them from the other** — each was established from its own artefacts.

#### V2-BR-034 — Result classification is driven by the reference-range table

| Field | Content |
|---|---|
| **Rule name** | Result classification is driven by the reference-range table |
| **Arabic label** | «يميز النتائج الغير طبيعية والحرجة ويكتب تمييز لها (عالٍ أو منخفضة أو حرجة)» |
| **Domain** | Results |
| **Description** | An entered value is compared against the low and high limits held in `normalrange` for the analyte. **The limits are data, not code.** The comparison itself is not performed in the code but in the frozen copy of the limits carried on the test row (`patienttest.lowlimit`, `highlimit`, `nolowexe`, `nohighexe`) — 112 of the 222 `patienttest` rows carry a frozen `normalrang`. |
| **Business purpose** | The laboratory can change its reference policy without a software change. |
| **Trigger / applicable event** | Result value entry |
| **Preconditions** | A matching `normalrange` row exists |
| **Actors and permissions** | Lab operator (entry); administrator (range maintenance) |
| **Expected behaviour** | Outside the limits ⇒ abnormal, with the stored exception text |
| **Restrictions** | No matching row ⇒ no classification. |
| **Validation conditions** | — |
| **Exceptions** | Behaviour with no matching row is NOT ESTABLISHED. |
| **Consequences** | **The range table is the system's principal safety-critical knowledge asset**, and the freeze-on-order mechanism means a range change does not affect already-ordered tests. |
| **Related workflows** | WF-V2-01 |
| **Related functions** | F-V2-008 |
| **Related entities** | `normalrange` (852 rows), `patienttest` |
| **Relevant fields** | `lowlimit`, `highlimit`, `nolowexe`, `nohighexe`, `normalrang` |
| **Related statuses** | Normal / abnormal / critical |
| **Evidence** | CHM `004A.htm` (byte-identical to V1); `normalrange` and `patienttest` schemas |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 (V1-BR-009) and V-CD-2019 |
| **Known limitations** | The band-selection algorithm is NOT ESTABLISHED. |

#### V2-BR-035 — A critical limit overrides the abnormal classification

| Field | Content |
|---|---|
| **Rule name** | A critical limit overrides the abnormal classification |
| **Arabic label** | «عالٍ أو منخفضة أو حرجة» |
| **Domain** | Results / Clinical safety |
| **Description** | The classification is three-valued and ordered: normal → abnormal → **critical**. A value breaching the critical limit is presented as critical. |
| **Business purpose** | Distinguishes values requiring escalation from values merely outside the reference interval. |
| **Trigger / applicable event** | Result value entry |
| **Preconditions** | Critical limit populated |
| **Actors and permissions** | Lab operator |
| **Expected behaviour** | Critical results are visually distinct |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **★ No escalation workflow is evidenced** — no callback, no repeat, no critical-result log separate from the report. The critical state is visual only. For a laboratory this is the single most important functional gap in the system. |
| **Related workflows** | WF-V2-01 |
| **Related functions** | F-V2-008 |
| **Related entities** | `normalrange`, `patienttest` |
| **Relevant fields** | Critical range, Critical comment, Critical flag |
| **Related statuses** | Critical |
| **Evidence** | CHM `004A.htm`; `normalrange` critical columns |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 (V1-BR-010) and V-CD-2019 |
| **Known limitations** | The flag precedence when a value breaches both limits is NOT ESTABLISHED. |

#### V2-BR-036 — Reference ranges are banded by sex and age

| Field | Content |
|---|---|
| **Rule name** | Reference ranges are banded by sex and age |
| **Arabic label** | «كيف يتم اضافة أو تعديل المعدلات الطبيعية لتحليل معين ؟» |
| **Domain** | Clinical knowledge |
| **Description** | Each `normalrange` row is scoped by `Sex` (`Male` 422 / `Female` 421 / `All` 5 / empty 4), an age window (`firstold`, `secondold`) and an age unit (`Years` 708 / `Days` 70 / `Mons.` 65 / empty 9). |
| **Business purpose** | Paediatric and sex-specific physiology require different reference intervals. |
| **Trigger / applicable event** | Range maintenance; result entry |
| **Preconditions** | The patient's sex and age are known |
| **Actors and permissions** | Laboratory administrator |
| **Expected behaviour** | The applicable range is selected by analyte + sex + age band |
| **Restrictions** | Overlap detection NOT ESTABLISHED |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | The selection rule among overlapping bands is NOT ESTABLISHED and materially affects classification. |
| **Related workflows** | — |
| **Related functions** | F-V2-012, F-V2-008 |
| **Related entities** | `normalrange` |
| **Relevant fields** | `sex`, `firstold`, `secondold`, `ageunit` |
| **Related statuses** | — |
| **Evidence** | CHM `008.htm`; `normalrange` distributions |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED for the banding; UNKNOWN for the selection algorithm |
| **Version applicability** | V-ROOT-1.00 (V1-BR-011) and V-CD-2019 — **the 852 rows are identical in both versions** |
| **Known limitations** | `pregsmok` is populated on all 852 rows; its role is NOT traced. |

#### V2-BR-037 — Clinical computation constants are held as data

| Field | Content |
|---|---|
| **Rule name** | Clinical computation constants are held as data |
| **Arabic label** | «Hct = Hgb × 3.3» and the accompanying factors, in the manual's notes topic |
| **Domain** | Clinical |
| **Description** | `constanttable` holds the derivation factors as text in seven columns (`constantdiscrp`, `constant1`…`constant6`), in two groups: `CBCCONS` (3.3, 6.25, 6.75, 7.5, 8.25) and `PTCONS` (12, 35, 1.33, `2CVJW`). The values are stored as **Text (50)** so that a non-numeric token such as `2CVJW` can coexist with the numbers. |
| **Business purpose** | A laboratory can correct a factor without a software change. |
| **Trigger / applicable event** | Result derivation |
| **Preconditions** | The source result exists |
| **Actors and permissions** | Lab operator |
| **Expected behaviour** | Derived `Hct`, `Hgb%`, ISI |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | The age-band boundary at 12 years is ambiguous in the source wording. |
| **Consequences** | **★ The factors are also a silent-propagation risk** — a wrong factor propagates into every report of that discipline with no indication. |
| **Related workflows** | WF-V2-01 |
| **Related functions** | F-V2-008 |
| **Related entities** | `constanttable` |
| **Relevant fields** | `constant1`…`constant6` |
| **Related statuses** | — |
| **Evidence** | CHM `nots.htm` (byte-identical to V1); `constanttable` — **exact numeric concordance** |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 (V1-BR-037…040) and V-CD-2019 — **identical values in both** |
| **Known limitations** | The ISI formula and the QC rule evaluation algorithm are NOT ESTABLISHED. |

#### V2-BR-038 — Only reviewed results are eligible for release

| Field | Content |
|---|---|
| **Rule name** | Only reviewed results are eligible for release |
| **Arabic label** | «F8=اعتماد مُراجَع» |
| **Domain** | Results / Quality |
| **Description** | A result carries an explicit `resultreview` Boolean. The hand-over path and the print path both filter on reviewed results, and the PDF path additionally refuses with «Not found any Verified selected tests or result to print it». |
| **Business purpose** | Prevents release of an unreviewed or unverified result. |
| **Trigger / applicable event** | Result confirmation; hand-over; print |
| **Preconditions** | Result saved |
| **Actors and permissions** | Lab operator confirms; reception clerk releases |
| **Expected behaviour** | Unreviewed results are not releasable or printable |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **The single quality gate between measurement and release.** In version 1 the storage field was not isolated; in version 2 it is explicit — so the rule is more directly observable, not different. |
| **Related workflows** | WF-V2-01 |
| **Related functions** | F-V2-009, F-V2-010, F-V2-028 |
| **Related entities** | `patienttest` |
| **Relevant fields** | `resultreview` |
| **Related statuses** | Reviewed / not reviewed |
| **Evidence** | `patienttest.resultreview` (Boolean); the PDF refusal literal |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 (V1-BR-014) and V-CD-2019 |
| **Known limitations** | Whether a second signatory is required is NOT ESTABLISHED. |

#### V2-BR-040 — The patient identifier is an order/visit identifier with positional segments

| Field | Content |
|---|---|
| **Rule name** | The patient identifier is an order/visit identifier with positional segments |
| **Arabic label** | — |
| **Domain** | Patient / Data |
| **Description** | `patientid` (Text 13) is **not** a patient key. It is a composite **order/visit** identifier: `MID(patientid,3,2)` is the **person key** (used both as the patient filter and as `COUNT(MID(patientid,3,2))` in the turnover reports); `MID(patientid,9,1)` is the **branch**; `mid(patientid,10,3)` is a **display code**. **There is no patient master table** — the table named `patients` holds operators. |
| **Business purpose** | Carries the identity, the branch and a display code in one 13-character field. |
| **Trigger / applicable event** | Every patient record read or write |
| **Preconditions** | — |
| **Actors and permissions** | The application |
| **Expected behaviour** | Each segment is recovered positionally |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **★ Any data migration that reformats the identifier silently breaks three separate functions at once** — patient identity, branch attribution and the turnover counts. |
| **Related workflows** | WF-V2-01, WF-V2-02 |
| **Related functions** | F-V2-003, F-V2-005, F-V2-031 |
| **Related entities** | `patientinfo`, `patienttest` |
| **Relevant fields** | `patientid` |
| **Related statuses** | — |
| **Evidence** | `MID(patientid,3,2)`, `MID(patientid,9,1) as bran`, `mid(patientid,10,3) as patid` in the extracted SQL |
| **Evidence type** | ET-CODE |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 (V1-BR-005) and V-CD-2019 — **the branch segment is a V2 addition at the same position** |
| **Known limitations** | The remaining segments are NOT ESTABLISHED. |

#### V2-BR-042 — Referring-party commission is a percentage of a patient-level base

| Field | Content |
|---|---|
| **Rule name** | Referring-party commission is a percentage of a patient-level base |
| **Arabic label** | «عمولات جهات الإحالة» |
| **Domain** | Finance |
| **Description** | The commission is computed as `SUM(Gehats.omola * patientinfo.elmadfoo3 / 100)`. The referring party's `omola` rate is applied to the patient-level amount `elmadfoo3`. |
| **Business purpose** | Makes the referral commission proportional to the value of the work generated. |
| **Trigger / applicable event** | Stock-take |
| **Preconditions** | The patient has an `elmadfoo3` value and a referring party with a rate. |
| **Actors and permissions** | Accounting user |
| **Expected behaviour** | The commission total is Σ(rate × base ÷ 100) over patients. |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | The commission base is a **separate field** from `totalprice` and `totalpricedisc` — the three amount fields on `patientinfo` are distinct, and only `elmadfoo3` participates in the formula. |
| **Related workflows** | — |
| **Related functions** | F-V2-015, F-V2-022 |
| **Related entities** | `Gehats`, `patientinfo` |
| **Relevant fields** | `Gehats.omola`, `patientinfo.elmadfoo3` |
| **Related statuses** | — |
| **Evidence** | The SQL literal in `analysis/BATCH-01/sql/statements.csv` |
| **Evidence type** | ET-CODE |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 (V1-BR-049) and V-CD-2019 |
| **Known limitations** | The business meaning of `elmadfoo3` is NOT ESTABLISHED — only its role in the formula is. |

#### V2-BR-043 — The test number 4 restriction remains in force

| Field | Content |
|---|---|
| **Rule name** | The test number 4 restriction remains in force |
| **Arabic label** | «ملاحظات يجب مراعتها عند استخدام نظام ريال لاب سيستم» — «تحليل البول يحمل الرقم 4 في النظام ولا يجوز تغيير رقمه ولا اسمه» |
| **Domain** | Catalogue |
| **Description** | The urine analysis carries test number 4 and **neither its number nor its name may be changed**. |
| **Business purpose** | Preserves a stable key for the most-used test. |
| **Trigger / applicable event** | Any attempt to edit the urine analysis |
| **Preconditions** | — |
| **Actors and permissions** | Laboratory administrator |
| **Expected behaviour** | The edit is not permitted |
| **Restrictions** | Number **and** name immutable |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | The report layouts and the range rows for urine are keyed to this identity and cannot be re-pointed. |
| **Related workflows** | — |
| **Related functions** | F-V2-011, F-V2-012 |
| **Related entities** | `testtable`, `normalrange` |
| **Relevant fields** | `testcode`, `testname` |
| **Related statuses** | — |
| **Evidence** | CHM `nots.htm` — **byte-identical to V1** |
| **Evidence type** | ET-DOC |
| **Confidence** | CONFIRMED as a documented rule; UNKNOWN whether it is code-enforced |
| **Version applicability** | V-ROOT-1.00 (V1-BR-016) and V-CD-2019 — **the manual is byte-identical, so the same restriction applies to the 2020 product** |
| **Known limitations** | Code enforcement NOT ESTABLISHED. |

#### V2-BR-044 — Send-out enrolment is flag-driven with two settlement states

| Field | Content |
|---|---|
| **Rule name** | Send-out enrolment is flag-driven with two settlement states |
| **Arabic label** | «خالص» · «دائماً إليه» |
| **Domain** | Send-out |
| **Description** | Tests flagged `testtable.inotherlab = True` enter the send-out list automatically. Each carries two independent states — `sentout` (settled) and `send` (dispatched) — and a destination in `otherlabname`, which may be made the standing default with «دائماً إليه». |
| **Business context** | Dispatch and payment are tracked separately so an unsettled referral stays visible. |
| **Trigger / applicable event** | Test ordering; send-out settlement |
| **Preconditions** | The test is flagged send-out |
| **Actors and permissions** | Accounting user |
| **Expected behaviour** | Flagged tests appear; unsent and unsettled remain listable |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | The catalogue flag is an operational decision with financial consequences. |
| **Related workflows** | — |
| **Related functions** | F-V2-014, F-V2-011 |
| **Related entities** | `testtable`, `patienttest` |
| **Relevant fields** | `inotherlab`, `sentout`, `send`, `otherlabname` |
| **Related statuses** | Sent / not sent; settled / not settled |
| **Evidence** | CHM `010.htm` (byte-identical to V1); `testtable` and `patienttest` schemas |
| **Evidence type** | ET-DOC+DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 (V1-BR-017, 023, 024) and V-CD-2019 |
| **Known limitations** | — |

#### V2-BR-045 — Printed output is audited with a per-operator, dated, sequenced record

| Field | Content |
|---|---|
| **Rule name** | Printed output is audited with a per-operator, dated, sequenced record |
| **Arabic label** | — |
| **Domain** | Reporting / Audit |
| **Description** | Every print records `resultprint` (flag), `printuser` (Text 40), `printdate` (Text 40) and `printnum` (Double) on the test row. The sequence origin is the absolute value `SETTING.ini [setup] reportstart=1984`. |
| **Business purpose** | Makes every issued report attributable and sequentially identifiable. |
| **Trigger / applicable event** | Every print |
| **Preconditions** | — |
| **Actors and permissions** | The application writes the audit; the operator is the subject. |
| **Expected behaviour** | The issued report can be tied to an operator, a moment and a sequence number. |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **★ `printnum` is a confidentiality mechanism as much as an identifier** — a missing or duplicated number reveals a missing report. The audit also makes the per-user workload accounting (V2-BR-042's sibling) possible. Note the timestamps are `Text`, not `DateTime`. |
| **Related workflows** | WF-V2-01, WF-V2-03 |
| **Related functions** | F-V2-026, F-V2-028, F-V2-015 |
| **Related entities** | `patienttest`, `SETTING.ini` |
| **Relevant fields** | `resultprint`, `printuser`, `printdate`, `printnum`, `reportstart` |
| **Related statuses** | — |
| **Evidence** | `patienttest` schema; `SETTING.ini` |
| **Evidence type** | ET-DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | V-ROOT-1.00 (V1-BR-033) and V-CD-2019 |
| **Known limitations** | The increment rule and the persistence location of the counter are NOT ESTABLISHED. |

### 3.7 Deployment and security rules

#### V2-BR-027 — Three database engines are supported

| Field | Content |
|---|---|
| **Rule name** | Three database engines are supported |
| **Arabic label** | — |
| **Domain** | Deployment |
| **Description** | The installer offers **Access (Jet 4)**, **SQL Server Express 2005** and **MySQL (Connector ODBC 3.51)**. The engine is selected at install time and encoded in `BASE.ini` by the `Databasetype=XMSACCESSX` toggle pattern — the engine name wrapped in `X…X`, where `X` marks it enabled. |
| **Business purpose** | Lets a laboratory choose the database platform it already operates. |
| **Trigger / applicable event** | Installation |
| **Preconditions** | — |
| **Actors and permissions** | System administrator |
| **Expected behaviour** | The application connects to the selected engine. |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **★ Multi-engine support is the reason the SQL dialect differences exist at all** — including the migration DDL that only partly works against Jet 4. It is a feature with a substantial hidden cost. |
| **Related workflows** | — |
| **Related functions** | F-V2-001 |
| **Related entities** | `BASE.ini` |
| **Relevant fields** | `Databasetype`, `InitialCatalog`, `DataSource`, `loca` |
| **Related statuses** | — |
| **Evidence** | `RL_SETUP.exe` engine-selection strings; `Setup*.bat`; `BASE.ini` |
| **Evidence type** | ET-INST |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC** — Version 1 has no installer and no engine selector |
| **Known limitations** | The full encoding of the toggle pattern for the SQL Server and MySQL values is NOT ESTABLISHED. |

#### V2-BR-028 — Two SQL catalogues plus a third attached database

| Field | Content |
|---|---|
| **Rule name** | Two SQL catalogues plus a third attached database |
| **Arabic label** | — |
| **Domain** | Deployment / Data |
| **Description** | The installer attaches three databases from `D:\Lab_data\`: `Attach_Patients.bat` → `patients`; `Attach_Interface.bat` → `Interface`; `Attach_VIDAS.bat` → **`VIDAS`**. |
| **Business context** | `Interface` is attached by the installer but **the application's own SQL references only `Patients`** — `Interface` appears to be the database of the separately installed "RealLab Interface" product. `VIDAS` is referenced but **never characterised**. |
| **Trigger / applicable event** | Installation |
| **Preconditions** | `D:\Lab_data\` populated with the `.mdf` files and the attach scripts |
| **Actors and permissions** | System administrator |
| **Expected behaviour** | The three databases are attached and usable. |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **★ The `VIDAS` database is a documented open question.** A Visual FoxPro 6.0 runtime (`vfp6run.exe`, `vfp6t.dll`, `vfp6rrus.dll`) is present in the shipped VB6 SP6 redistributable manifest, which makes a FoxPro-based component plausible — **but no structural link to `VIDAS` is established, and the proposition remains a HYPOTHESIS.** |
| **Related workflows** | — |
| **Related functions** | F-V2-001 |
| **Related entities** | `VIDAS`, `Interface`, `patients` |
| **Relevant fields** | — |
| **Related statuses** | — |
| **Evidence** | `RL_SETUP.exe` strings `cmd.exe /C start /B /wait D:\Lab_data\Attach_Interface.bat` / `Attach_Patients.bat` / `Attach_VIDAS.bat`; `analysis/BATCH-10/` VB6 SP6 manifest |
| **Evidence type** | ET-INST |
| **Confidence** | CONFIRMED for the attach scripts; UNKNOWN for the content of `VIDAS` |
| **Version applicability** | **VERSION-SPECIFIC** |
| **Known limitations** | The schema, purpose and technology of `VIDAS` are UNKNOWN. |

#### V2-BR-029 — Installation self-registers 24 ActiveX components and overwrites system files

| Field | Content |
|---|---|
| **Rule name** | Installation self-registers 24 ActiveX components and overwrites system files |
| **Arabic label** | — |
| **Domain** | Deployment / Security |
| **Description** | `SETUP.LST` specifies **24 `$(DLLSelfRegister)` actions** — each registering an OCX or DLL in the Windows registry — and writes **22 files to `$(WinSysPathSysFile)`** (i.e. `System32`) and **16 to `$(WinSysPath)`**. The Bullzip installer additionally ships `msxml6.dll`, `oleaut32.dll`, `olepro32.dll` and `asycfilt.dll` into the system directory. |
| **Business purpose** | *(none — an installation side effect)* |
| **Trigger / applicable event** | Installation |
| **Preconditions** | Administrative rights |
| **Actors and permissions** | The installer |
| **Expected behaviour** | The components are registered and the files are in place |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **★ The installation requires administrative privileges and modifies protected operating-system directories.** It also registers 24 COM components permanently, with **no evidenced uninstaller path** beyond `st6unst.exe`. |
| **Related workflows** | — |
| **Related functions** | F-V2-001 |
| **Related entities** | `SETUP.LST` |
| **Relevant fields** | `$(DLLSelfRegister)`, `$(WinSysPathSysFile)`, `$(WinSysPath)` |
| **Related statuses** | — |
| **Evidence** | `analysis/BATCH-10/cab/`; `SETUP.LST`; the Bullzip Inno payload listing |
| **Evidence type** | ET-INST |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC** — Version 1 ships no installation media |
| **Known limitations** | Whether the installer rolls back on failure is NOT ESTABLISHED. |

#### V2-BR-030 — Two conflicting builds of the same runtime components ship on one disc

| Field | Content |
|---|---|
| **Rule name** | Two conflicting builds of the same runtime components ship on one disc |
| **Arabic label** | — |
| **Domain** | Deployment |
| **Description** | The disc carries two different builds of four components: `msvbvm60.dll` (CAB 1,386,496 / 2009 versus `OCX\` 1,384,479 / 2004), `msado27.tlb` (CAB 98,304 / 2010 versus `OCX\` 81,920 / 2008), `MSCOMM32.OCX` (CAB 103,744 / 1998 versus `OCX\` 120,648 / 2008), `MSFLXGRD.OCX` (CAB 244,024 / 1998 versus `OCX\` 259,912 / 2008). |
| **Business purpose** | *(none — a packaging artefact)* |
| **Trigger / applicable event** | Installation |
| **Preconditions** | — |
| **Actors and permissions** | — |
| **Expected behaviour** | — |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **★ Because `Setup.bat` copies `OCX\` to the installation root before the CAB executes, the installed version of each component depends on execution order — the installation is not deterministic.** |
| **Related workflows** | — |
| **Related functions** | F-V2-001, F-V2-021 |
| **Related entities** | — |
| **Relevant fields** | — |
| **Related statuses** | — |
| **Evidence** | `analysis/BATCH-10/cab/cab-inventory.csv` (96 rows, SHA-256 per member); the `OCX\` file comparison |
| **Evidence type** | ET-DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC** |
| **Known limitations** | The actual copy order at run time is NOT ESTABLISHED — it is inferred from the script sequence. |

#### V2-BR-031 — The PDF installation order is defective

| Field | Content |
|---|---|
| **Rule name** | The PDF installation order is defective |
| **Arabic label** | «welcome in RealLab installation wizard» (the banner of both scripts) |
| **Domain** | Deployment / PDF |
| **Description** | `RL_SETUP.exe` invokes `System\PDF\SetupP.bat` (which installs **Bullzip only**) **before** `System\SetupPZ.bat` (which installs **‹.NET 4› then Bullzip**). Bullzip PDF Printer 11 requires ‹.NET Framework 4›. A third script, `System\PDF\SetupFN4.bat`, installs ‹.NET 4› alone but is **invoked by nobody** and its relative path (`dotNetFx40_Full_x86_x64.exe`) is missing the `System/PDF/` prefix. Separately, `setup.exe` is launched with `/sc:\setup.log` and **without `/wait`**. |
| **Business context** | Both scripts carry the banner «welcome in RealLab installation wizard» — they are the vendor's own deployment scripts. |
| **Trigger / applicable event** | Installation |
| **Preconditions** | — |
| **Actors and permissions** | Installer |
| **Expected behaviour** | ‹.NET 4› is installed, then Bullzip, once, in that order. |
| **Restrictions** | — |
| **Validation conditions** | **None — there is no post-install check.** |
| **Exceptions** | — |
| **Consequences** | **★ Three distinct defects in one chain: a duplicate install in the wrong order, an orphan script with a broken path, and an un-awaited invocation that writes a log to the root of `C:`.** The net effect is a first, probably failing Bullzip install followed by a second, correct one. |
| **Related workflows** | — |
| **Related functions** | F-V2-001, F-V2-028 |
| **Related entities** | `SetupP.bat`, `SetupFN4.bat`, `SetupPZ.bat` |
| **Relevant fields** | — |
| **Related statuses** | — |
| **Evidence** | The three `.bat` files; the `RL_SETUP.exe` invocation strings |
| **Evidence type** | ET-CODE+INST |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC** |
| **Known limitations** | Whether the first Bullzip attempt actually fails is NOT ESTABLISHED — Bullzip's behaviour without ‹.NET 4› is not evidenced. |

#### V2-BR-032 — The database credential is shared, static and stored in clear text

| Field | Content |
|---|---|
| **Rule name** | The database credential is shared, static and stored in clear text |
| **Arabic label** | «اسم وقـمة مرور المستخدم ويمنع تغيرها وهى خاصة ببيانات الاتصال بخادم قواعد البيانات» |
| **Domain** | Security |
| **Description** | The application connects with `UserID=sa` / `Password=[REDACTED]`, read from `BASE.ini` and `data_connection1.udl`, with `PersistSecurityInfo=True`. The manual states the database identity **must not be changed** and that it is the same on every client. |
| **Business purpose** | *(none — a defect)* |
| **Trigger / applicable event** | Every database connection |
| **Preconditions** | — |
| **Actors and permissions** | Anyone with file access to the workstation or the installation media |
| **Expected behaviour** | The connection is established with the stored credential |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **Unchanged from Version 1, and now compounded** — with SQL Server Express 2005 installed and network-listening, the same static credential is exposed on the network. |
| **Related workflows** | WF-V2-01 |
| **Related functions** | F-V2-001, F-V2-002 |
| **Related entities** | `BASE.ini`, `data_connection*.udl` |
| **Relevant fields** | `UserID`, `Password`, `PersistSecurityInfo` |
| **Related statuses** | — |
| **Evidence** | `BASE.ini`; `data_connection*.udl`; CHM `001.htm` |
| **Evidence type** | ET-CFG |
| **Confidence** | CONFIRMED |
| **Version applicability** | CONFIRMED PRESENT in V-ROOT-1.00 (V1-BR-042, 043) and V-CD-2019 — **identical values in both** |
| **Known limitations** | — |

#### V2-BR-033 — The official user manual is not updated for this version

| Field | Content |
|---|---|
| **Rule name** | The official user manual is not updated for this version |
| **Arabic label** | — |
| **Domain** | Documentation |
| **Description** | `real lab system help.chm` is **byte-identical** between the two versions — SHA-256 `624308649E523400…`, 5,193,985 bytes, 15 topics, **zero changed characters** — despite 41 new objects, a complete SMS subsystem, 10 new tables, 268 new DDL statements and 21 new documented capabilities. |
| **Business context** | The in-application manual is the only documentation a user sees while operating the system. |
| **Trigger / applicable event** | — |
| **Preconditions** | — |
| **Actors and permissions** | — |
| **Expected behaviour** | — |
| **Restrictions** | — |
| **Validation conditions** | — |
| **Exceptions** | — |
| **Consequences** | **★ Every capability added in v1.605 is undocumented in the CHM.** The vendor's own 212-page learning guide is the only complete description of the product — and it is not installed with the application. |
| **Related workflows** | — |
| **Related functions** | — |
| **Related entities** | `real lab system help.chm` |
| **Relevant fields** | — |
| **Related statuses** | — |
| **Evidence** | SHA-256 comparison of the two CHM copies |
| **Evidence type** | ET-DB |
| **Confidence** | CONFIRMED |
| **Version applicability** | **VERSION-SPECIFIC** |
| **Known limitations** | — |

### 3.8 Remaining V2 rules — consolidated entries

| ID | Rule | Evidence | Confidence | Note |
|---|---|---|---|---|
| **V2-BR-011** | Statistics are computed from the transactional tables, not from an aggregate store | Vendor guide 4-1…4-5; `statisReport`; `patientinfo.requestdate`, `patientinfo.patientsex`, `patienttest.subgroupcode` | CONFIRMED | No dedicated aggregate or summary table is evidenced |
| **V2-BR-012** | The attendance register is a distinct structure from the login-group table | Vendor guide 3-11; `loger` table | CONFIRMED | **Important:** Version 1 already records attendance on the operator table (`startwork`, `endwork`, `breakyorn`, `breaktime`). `loger` is an **additional** event log, not the first attendance mechanism |
| **V2-BR-013** | Work-sheet printing is independently toggleable | `SETTING.ini [setup] printworkpaper=1` | CONFIRMED | Present in both versions |
| **V2-BR-014** | Farm reporting is a parallel reporting vertical | Vendor guide 1-8, 3-9; `sharekat`, `sharekathesab` | CONFIRMED | Shares the result-entry machinery but not the report template |
| **V2-BR-015** | Two antibiotic structures coexist | `antibiotic` (38 rows / 25 cols) and `antibio` (96 rows / 2 cols) | CONFIRMED | Their relationship is UNKNOWN — inherited from RL-V1-Q-08 |
| **V2-BR-016** | Sample disposition is recorded against the microbiology tables | Vendor guide 6-x; `cultures`, `cultureanti`, `sample` | CONFIRMED | The `sample` table structure is UNKNOWN |
| **V2-BR-018** | The price list is a distinct artefact from the per-test price fields | Vendor guide 3-4, 3-5; `pricelist`, `pricemenu`, `shopping` versus `testtable.patientprice`/`labprice`/`otherlabprice` | CONFIRMED | Price-list structures not extracted at field level |
| **V2-BR-019** | The account type is a single system-wide default | Vendor guide 7-7; `patientinfo.pricemethod` Text(10), `Gehats.pricemethod` Text(20) | CONFIRMED | **The `pricemethod` value domain is NOT ESTABLISHED** — the previously-cited values have 0 hits in the SQL corpus and `patientinfo` has 0 rows. The per-party and per-patient overrides exist; their precedence is NOT ESTABLISHED |
| **V2-BR-022** | NATIGH accounts are provisioned for external parties | Vendor guide — «إنشاء حساب للطبيب المعالج لاستخدام خدمة نتيجة» and «إنشاء حساب للمعمل لاستخدام خدمة نتيجة» | CONFIRMED | **Extends the system's trust boundary beyond the laboratory itself.** The account store is not established |
| **V2-BR-026** | The shipped database backup is stale | `Patients_backup.mdb` — 90 tables / 1,381 columns / 2,786 rows, identical in the 2019 payload | CONFIRMED | Inherited from V1-BR-028. The backup mechanism and schedule are NOT ESTABLISHED |
| **V2-BR-039** | The operator carries seventeen function-level permission bits | `patients.grant1`…`grant12` (both versions); the v1.605 DDL adds `grant13`…`grant17` (plus `branche` and `SecWord`) to the **same operator table `patients`** | CONFIRMED | **Corrects an earlier characterisation:** Version 1 already had **twelve** bits. The mapping of any bit to a capability is UNKNOWN |
| **V2-BR-041** | Operator attendance is recorded on the operator record | `patients.startwork`, `endwork`, `breakyorn`, `breaktime` | CONFIRMED | Present in **both** versions — attendance is **not** a V2 addition |

---

## 4. Rules NOT established for V-CD-2019

| ID | Candidate rule | Why not established | What would settle it |
|---|---|---|---|
| RL-V2-BR-U01 | Each `grant` bit maps to a named capability | The bit names are numeric only | Runtime observation of the permission model |
| RL-V2-BR-U02 | The `loger` table records clinical events rather than attendance | The vendor guide describes attendance; the table schema was not extracted | Field-level extraction of `loger` |
| RL-V2-BR-U03 | The migration is retried on a subsequent run | No evidence of a retry or a migration ledger | Runtime observation of the start-up sequence |
| RL-V2-BR-U04 | The NATIGH feature degrades gracefully without its flag column | No failure path is evidenced | Runtime observation |
| RL-V2-BR-U05 | Consolidated reporting works against Access | `SHAPE`/`APPEND` is bound to the shape provider | Runtime observation on an Access installation |
| RL-V2-BR-U06 | The `VIDAS` database is Visual FoxPro | A FoxPro 6.0 runtime is present in the SP6 manifest, but no structural link exists | Access to a `VIDAS` data file, or the attach script's contents |
| RL-V2-BR-U07 | The `antibio` table is a subset of `antibiotic` | Both exist; no join located | Field-level comparison |
| RL-V2-BR-U08 | The `sharekat` / `sharekathesab` schemas relate to the standard test tables | Not extracted | Field-level extraction |
| RL-V2-BR-U09 | `MyLabSetting` holds a laboratory identifier only | Not extracted | Field-level extraction |
| RL-V2-BR-U10 | The five absent Microsoft installers were shipped on a second disc | The installation tree is partial | Inspection of the physical media |

---

## 5. Version-specific rule summary

| Rule family | V-ROOT-1.00 | V-CD-2019 |
|---|---|---|
| Result classification by reference range | Present (V1-BR-009) | Present (V2-BR-034) |
| Critical classification | Present (V1-BR-010) | Present (V2-BR-035) |
| Range banding by sex and age | Present (V1-BR-011) | Present (V2-BR-036) — identical 852 rows |
| Clinical constants as data | Present (V1-BR-037…040) | Present (V2-BR-037) — identical values |
| Review gate before release | Present (V1-BR-014) | Present (V2-BR-038) — explicit field |
| Test number 4 immutability | Present (V1-BR-016) | Present (V2-BR-043) |
| Send-out two-state settlement | Present (V1-BR-024) | Present (V2-BR-044) |
| Print audit with sequence number | Present (V1-BR-033) | Present (V2-BR-045) |
| Commission percentage formula | Present (V1-BR-049) | Present (V2-BR-042) |
| Cleartext shared database credential | Present (V1-BR-042, 043) | Present (V2-BR-032) |
| **Operator permission bits** | **12 bits** (V1-BR-047) | **17 bits** (V2-BR-039) |
| **Operator attendance on the operator record** | **Present** (V1-BR-048) | Present (V2-BR-041) |
| **PDF export** | **ABSENT** (V1-BR-046) | Present (V2-BR-001…004) |
| **Branch code in the identifier** | **ABSENT** | Present (V2-BR-008) |
| **Runtime schema migration** | **ABSENT** | Present, partial (V2-BR-023, 024) |
| **Digital result delivery** | **ABSENT** | Present (V2-BR-005…007, 022) |
| **Statistics module** | **ABSENT** | Present (V2-BR-011) |
| **Multi-branch accounting** | **ABSENT** | Present (V2-BR-010) |
| **Manual currency** | — | **Stale** (V2-BR-033) |

---

**End of Business_Rules_V2.md**
