# Legacy Capabilities — V-ROOT-1.00 and V-CD-2019

**Purpose:** Curated capability catalogue for the legacy systems.
**Scope:** V1 capabilities (15, from CHM) and V2 capabilities (~60, from vendor guide).
**Audience:** Product, planning, developers.
**Version applicability:** Mixed.

---

## 1. V1 capabilities (15, from `help.chm`)

| ID | Capability | Arabic label | Evidence |
|---|---|---|---|
| V1-CAP-01 | Run the system and sign in | «كيف يتم تشغيل نظام ريال لاب سيستم ؟» / «كيف يتم تسجيل الدخول» | CHM `001.htm`, `002.htm` |
| V1-CAP-02 | Register a new patient | «كيف يتم اضافة بيانات مريض جديد ؟» | CHM `003.htm`; `patientinfo` |
| V1-CAP-03 | Print the patient barcode | «كيف يتم طباعة باركود للمريض؟» | CHM `003A.htm`; `barcodes` |
| V1-CAP-04 | Enter results per clinical profile | «كيف يتم الوصول لنوافذ ادخال نتائج التحاليل ؟» / «كيف يتم ادخال نتائج التحاليل» | CHM `004.htm`, `004A.htm`; `patienttest` + 46 profile tables |
| V1-CAP-05 | Hand over results to the patient | «كيف يتم تسليم نتائج التحاليل للمريض ؟» | CHM `005.htm`; `patienttest.give` |
| V1-CAP-06 | Search for a patient | «كيف تتم عملية البحث عن مريض» | CHM `006.htm` |
| V1-CAP-07 | Add / edit test definitions | «كيف يتم اضافة أو تعديل بيانات تحليل ؟» | CHM `007.htm`; `testtable` |
| V1-CAP-08 | Administer normal reference ranges | «كيف يتم اضافة أو تعديل المعدلات الطبيعية» | CHM `008.htm`; `normalrange` (852 rows) |
| V1-CAP-09 | Customise report text, dimensions and colours | «كيف يتم التعديل فى نصوص وأبعاد وألوان التقرير ؟» | CHM `009.htm`; 45 DataReport + `Data\*.ini` |
| V1-CAP-10 | Manage send-out samples | «كيف يتم ادارة العينات المرسلة للخارج» | CHM `010.htm`; `sentout`/`send`/`otherlabname` |
| V1-CAP-11 | Accounting and stock-take | «كيف يتم معرفة حساب الدرج وعمل جرد» | CHM `011.htm`; `payment`, `Gehats.omola` |
| V1-CAP-12 | Back up the database | «كيف يتم عمل نسخ احتياطي لقاعدة بيانات النظام ؟» | CHM `backup.htm`; `labdb`/`labdb1` |
| V1-CAP-13 | Clinical constants and formulas | «ملاحظات يجب مراعتها» | CHM `nots.htm`; `constanttable` |
| V1-CAP-14 | Fixed test-numbering restriction (urine = 4) | «ملاحظات يجب مراعتها» | CHM `nots.htm`; `testtable.testcode` |
| V1-CAP-15 | Keyboard shortcuts and result-entry interface | — | CHM `004A/008/010/011.htm`; `daytip` |

**Evidence:** external: Complete_Reverse_Engineering_Reference_V1.md §10.1.

## 2. V2 capabilities (~60, from `RLS_Learn.pdf`, 212 pp., 510 headings, 7 modules)

The vendor guide documents ~60 capabilities in 7 modules. See external: 09-vendor-capability-catalogue.md for
the full catalogue. Summary by module:

| Module | Range | Content |
|---|---|---|
| Patient | 1-1 … 1-13a | Registration, account settlement, result entry, consolidated reports, farm reports, patient search, visit history, follow-up, NATIGH.COM delivery, medical history |
| Work Sheet | 2-1 … 2-2 | Work sheet over a patient group in a period; work sheet over a test/test group in a period |
| System | 3-1 … 3-5 | Test data and prices, new test, reference values, price lists, printing price lists |
| Server | 4-1 … 4-6 | Statistics and performance monitoring, patient-count statistics, request-rate statistics, sample counting, stock-take |
| Accounting | 5-1 … 5-5 | Period stock-take, multi-branch stock-take, stock-take by treating physician/referring party, send-out sample reconciliation |
| General | 6-1 … 9-7 | System settings, report settings, margins, paper size (A4/A5), header/footer, colours, printers, default account type, connection settings, background, sample splitting/withdrawal |
| (outside index) | — | NATIGH account creation, user/admin management, attendance registration, referring-party creation, farm creation, antibiotic catalogue, fixed comments, test-group creation, treating-physician creation, send-out samples |

## 3. V2 capabilities new vs V1 (11 documented)

| Capability | V1 CHM | V2 guide | Code/DB evidence |
|---|---|---|---|
| NATIGH.COM result delivery | ✘ | ✔ | `Natigh`, SMS subsystem, `SMSSender` |
| Branch management (master/reception) | ✘ | ✔ | `Patient.branche`, `Labbranch`, `Branch_Data.mdb` |
| Statistics and performance monitoring | ✘ | ✔ | `statisReport`, `loger` |
| Attendance registration | ✘ | ✔ | `loger`, `loggroup` |
| Work sheet | ✘ | ✔ | `workpaper`, `workpaper2` |
| Farm reporting + antibiotic catalogue | ✘ | ✔ | `sharekat`, `antibiotic`, `antibio` |
| Reference-value editing | ✔ | ✔ | `normalrange` (852) |
| Default account type | ✘ | ✔ | `pricemethod` |
| Connection settings (receipt) | ✘ | ✔ | `IDCARD.INI`, `SETTING.ini` |
| Sample splitting/withdrawal | ✘ | ✔ | `cultures`, `sample` |
| Test data and prices editing | ✔ | ✔ | `testtable` (609) |

**★ STRONGLY SUPPORTED:** what appeared as "capabilities" in V1 is in fact the *core system*; V2
extends it with 11 documented new capabilities never in the CHM. This makes the CHM effectively
obsolete for the 2019 product.

**Evidence:** external: 09-vendor-capability-catalogue.md §3.

## 4. External integrations (vendor capabilities)

| System | Kind | Interaction | Evidence |
|---|---|---|---|
| SQL Server | Database server | OLE DB `SQLOLEDB.1` over LAN; `MSDataShape.1` for shape/union queries | `data_connection.udl`, `data_connection1.udl` |
| Jet 4 / Access | Embedded database | Default store | `Patients.mdb` |
| Windows spooler | OS service | Print dispatch | `printerselect` module |
| Windows regional settings | OS | Date-locale guard | CHM `001.htm` |
| Send To OneNote 2007 | Local application | Printer destination | `SETTING.ini`, `Data\*.ini` |
| RealLabeg.com | Website | Published in invoice layout | `IDCARD.INI` |
| Facebook page | Social | Published in invoice layout | `IDCARD.INI` |
| NATIGH.COM (V2) | Web service | Result delivery | `Natigh`, `SMSSender`, `sms*` tables, `ReportSMS` |
| SMS gateway (V2) | Unknown provider | Outbound SMS | `SMSSender`, `SMStemplate`, `smslist`, `smsgroups` |

## 5. CHM vs vendor guide gap

| Document | Pages | Capabilities |
|---|---:|---:|
| `real lab system help.chm` (in-application) | 15 topics | 15 |
| `RLS_Learn.pdf` (vendor guide) | 212 pages | ~60 in 7 modules |
| Difference | — | 11 capabilities in the PDF but absent from the CHM |

The CHM is byte-identical in both versions and documents V1 only. Every capability added in V2 is
undocumented in the CHM.

## 6. Version applicability

- V1 capabilities (15): **V1 ONLY** (CHM byte-identical in V2, but documents V1 features).
- V2 capabilities (~60): **V2 ONLY** (vendor guide is V2-specific).
- External integrations (SQL Server, Jet, spooler, regional settings, OneNote, website, Facebook):
  **SHARED**.
- NATIGH.COM, SMS gateway: **V2 ONLY**.

## 7. Known limitations

- `VIDAS` database purpose unknown (RL-V2-Q-01).
- SMS gateway provider, protocol, and delivery-status handling not established (V2-BR-007).
- `pricemethod` value domain not established.
- Band-selection algorithm not established.
- Which `Branch_Data.mdb` copy is loaded not established.

## 8. Related documents

- `Docs/KB/legacy/business/WORKFLOWS.md` — workflows.
- `Docs/KB/legacy/business/SECURITY.md` — security.
- `Docs/KB/legacy/architecture/OVERVIEW.md` — architecture.
- `Docs/KB/legacy/architecture/DEPLOYMENT.md` — deployment.
- Original source evidence: external: 09-vendor-capability-catalogue.md,
  external: Complete_Reverse_Engineering_Reference_V1.md §10, `Docs/Complete_Reverse_Engineering_Reference_V2.md` §3, §10.
- Modern implementation: `src/RefLab.Application/Features/*` (73 use cases).
