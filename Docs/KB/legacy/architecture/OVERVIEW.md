# Legacy Architecture Overview — V-ROOT-1.00 and V-CD-2019

**Purpose:** Curated architecture reference for the legacy systems.
**Scope:** Layers, modules, dependencies, startup, error handling, runtime.
**Audience:** Developers, architects.

---

## 1. Layered architecture (both versions)

```
L0  WRAPPER     PE32, MSVBVM60.DLL, ActiveX controls, database providers
                (V2 adds: RL_SETUP.exe, installation media, PDF stack, VB6 SP6 redistributable)
L1  IDENTITY    VB6 objects (forms, DataReport, modules, classes)
L2  LOGIC       Declared methods, string literals, SQL, embedded DDL (V2 only)
L3  DATA        Access Jet 4 (+ SQL Server Express 2005 + MySQL in V2)
L4  BEHAVIOUR   Documented capabilities (15 in V1, ~60 in V2)
```

## 2. Component counts

| Component | V1 | V2 |
|---|---:|---:|
| VB6 objects | 169 | 210 |
| Forms | 118 | 132 |
| DataReport | 45 | 66 |
| Modules | 5 | 9 |
| Classes | 1 | 3 |
| Declared methods | 6,528 | 8,026 |
| String literals | 104,997 | 270,166 |
| SQL statements | 1,337 | 1,797 |
| Embedded DDL | 0 | 268 |
| Controls (forms) | 9,281 | (not separately counted) |

## 3. Modules

### V1 (5 modules)

| Module | Methods | Role |
|---|---:|---|
| `labdb` | 20 | Database maintenance — backup, restore, table creation |
| `labdb1` | 19 | Secondary database-maintenance procedures |
| `labfunction` | 13 | Accounting / stock-take calculations |
| `printerselect` | 16 | Printer enumeration and selection, print dispatch |
| `BCWFonts` | — | Laboratory display font handling |

### V2 (9 modules: 5 original + 4 new)

| Module | Role | Evidence |
|---|---|---|
| `labdb` | Database maintenance | BATCH-01/02/04 |
| `labdb1` | Secondary database-maintenance procedures | BATCH-01/02/04 |
| `labfunction` | Accounting and stock-take calculations | BATCH-01/02/04 |
| `printerselect` | Printer enumeration and selection | BATCH-01/02/04 |
| `BCWFonts` | Laboratory display font handling | BATCH-01/02 |
| (+4 V2-only modules) | Object count rose 5 → 9; names recorded in `analysis/BATCH-04/vb6/v1605/objects-full.txt` | BATCH-04 |

**Evidence:** external: Complete_Reverse_Engineering_Reference_V1.md §2.4; `Docs/Complete_Reverse_Engineering_Reference_V2.md` §2.3.

## 4. Runtime and library dependencies

| Component | Version / build | Evidence |
|---|---|---|
| `MSVBVM60.DLL` | 186 imports (V1) / 231 imports (V2) | `analysis/BATCH-04/vb6/v1605/` |
| Jet 4 engine | `msjet40.dll` + 11 companions | `analysis/BATCH-10/cab/` |
| 19 ActiveX controls | MSComctl, MSComct2, MSChrt20, MSDataGrd, MSDataLst, MSAdoDc, TabCtl32, ComDlg32, RichTx32, MCI32, MSComm32, MSFlxGrd, DBGrid32, RichEd32, MSDBRPtr, MSStdFmt, MSBind, + 3 Arabic-labelled controls | `analysis/BATCH-10/cab/` |
| .NET Framework 4.0 (V2) | RTM, box stub, 50,449,456 B | `analysis/BATCH-11/pe-envelope.csv` |
| Bullzip PDF Printer (V2) | 11.1.0.2600 (BioPDF ApS) | `analysis/BATCH-11/` |
| GPL Ghostscript Lite (V2) | 9.14.17 (AGPL v3) | `analysis/BATCH-11/` |
| Xpdf Lite (V2) | 3.3.4 (GPL v2/v3) | `analysis/BATCH-11/` |
| PDF Power Tool (V2) | 4.0.0.11 (commercial) | `analysis/BATCH-11/` |
| OLE/COM | `oleaut32.dll`, `olepro32.dll`, `asycfilt.dll`, `stdole2.tlb` | `analysis/BATCH-10/cab/` |

## 5. Screens and windows (V2, with V1 baseline)

| Screen | Arabic title as found | Technical name | Evidence |
|---|---|---|---|
| Start-up | «الصفحة الرئيسية» | `start` (14 controls) | CHM `001.htm` |
| Login | «شاشة التسجيل» | `login` | CHM `002.htm` |
| Server settings | «نافذة اعدادات وبيانات السيرفر» | `server` (27 controls) | CHM `001.htm` |
| Main patient window | «الصفحة الرئيسية» | `patientmain` (84 methods V2) | CHM `003.htm` |
| Add patient | — | `addpatient` (192 methods V2) | CHM `003.htm` |
| Patient card | — | `CardFrm` (3), `CardFrmSetx` (29) | V2-only |
| Result entry (per profile) | «نوافذ ادخال نتائج التحاليل» | `result*` (30+ forms) | CHM `004.htm` |
| Reference ranges | «المعدل الطبيعي» | `testrange`, `normal_test`, `outtest` | CHM `008.htm` |
| Report setup | — | `reportsetup` (108), `reportsetup2` (69) | CHM `009.htm` |
| Send-out | «العينات المرسلة للخارج» | `sarfz` (35), `moenysharekat`, `moenymozafeen` | CHM `010.htm` |
| Accounting / stock-take | «حساب الدرج وعمل جرد» | `labfunction` | CHM `011.htm` |
| Backup | — | `labdb`, `labdb1` | CHM `backup.htm` |
| Work sheet | — | `workpaper`, `workpaper2` | V2-only |
| Statistics | «الإحصائيات ومتابعة أداء المعمل» | `statisReport` | V2-only |
| SMS sender | — | `SMSfrmsenderos` (139 methods) | V2-only |
| Attendance log | «تسجيل الحضور والانصراف» | `loger` | V2-only |
| Branch administration | — | `z_branchs` (21) | V2-only |
| Price list | «طباعة قائمة أسعار التحاليل» | `z_pricelists` (28), `pricelistsN` | V2-only |

**Evidence:** external: Complete_Reverse_Engineering_Reference_V1.md §3.1; `Docs/Complete_Reverse_Engineering_Reference_V2.md` §3.1.

## 6. Official module taxonomy (V2, from `RLS_Learn.pdf`)

| Module | Capability range | Content summary |
|---|---|---|
| Patient | 1-1 … 1-13a | Registration, account settlement, result entry, consolidated reports, farm reports, patient search, visit history, follow-up, NATIGH.COM delivery, medical history |
| Work Sheet | 2-1 … 2-2 | Work sheet over a patient group in a period; work sheet over a test or test group in a period |
| System | 3-1 … 3-11 | Test data and prices, new test, reference values, price lists, printing price lists, attendance, farm reporting, antibiotic catalogue |
| Users & attendance | **4-1 … 4-3** | User management, permissions, attendance logging (`loger`) |
| Statistics | **5-1 … 5-4** | Laboratory statistics and performance monitoring, patient-count statistics by month/year/sex, request-rate statistics, sample counting |
| Accounting | **6-1 … 6-5** | Period stock-take, multi-branch stock-take, stock-take by treating physician or referring party, send-out sample reconciliation |
| Settings | **7-1 … 7-9** | System settings, report settings, margins, paper size (A4/A5), header/footer, colours, printer configuration, default account type, connection settings, background, sample splitting/withdrawal |

> **CORRECTION (2026-11-16).** The module numbering was previously given as Server 4-1…4-6, Accounting
> 5-1…5-5, General 6-1…9-7. The vendor guide's own table of contents (`RLS_Learn.pdf`, 212 pp.)
> assigns users/attendance to 4-x, statistics to 5-x, accounting to 6-x and settings to 7-x.
> See `Docs/KB/legacy/evidence/CONTRADICTIONS.md` C-18.
| (outside index) | — | NATIGH account creation, user/admin management, attendance registration, referring-party creation, farm creation, antibiotic catalogue, fixed comments, test-group creation, treating-physician creation, send-out samples |

**★ Gap:** the in-application `help.chm` documents only 15 capabilities; `RLS_Learn.pdf` documents ~60 in 7 modules. The CHM is byte-identical in both versions and is stale (documents V1 only).

**Evidence:** external: 09-vendor-capability-catalogue.md; `Docs/Complete_Reverse_Engineering_Reference_V2.md` §3.2.

## 7. Startup and execution

### V1 startup sequence (CHM `001.htm`)
Double-click the application icon or desktop shortcut → start-up window → login screen.
Two documented failure modes:
- «خادم قواعد البيانات غير معرف» (database server not defined) → closes on OK.
- «رجاء تحويل التاريخ الى اللغة الانجليزية» (non-English date locale) → closes on OK.

The manual establishes V1 as a **networked client/server application over a shared database**, with the
database server reachable by IP or machine name, and a hard dependency on the OS regional date format.
Server settings window has 4 items, of which only the server address/IP is mutable (V1-BR-001/002).

### V2 startup (adds)
- Database engine choice at install (Access / SQL Server Express 2005 / MySQL).
- Schema self-migration at start-up (268 DDL statements).
- `loger` attendance logging.
- `labbranchx` branch scoping.

**Evidence:** external: Complete_Reverse_Engineering_Reference_V1.md §2.6; `Docs/Complete_Reverse_Engineering_Reference_V2.md` §2.5, §4.2.

## 8. Error handling

| Condition | Behaviour | Evidence |
|---|---|---|
| Database server not defined | «خادم قواعد البيانات غير معرف» → closes on OK | CHM `001.htm` (identical in both) |
| Non-English date locale | «رجاء تحويل التاريخ الى اللغة الانجليزية» → closes | CHM `001.htm` |
| No printer available | "Can't find any printer." | literals |
| Printer API failure | "SetPrinter Failed. Error code: " + code | literals (V1 only) |
| Result has no verified tests | "Not found any Verified selected tests or result to print it" | literals (V2 PDF cluster) |
| Migration DDL cannot apply to Jet | silently inapplicable | DDL analysis |
| Bullzip printer absent | PDF path fails — no fallback evidenced | literal analysis |

## 9. External integrations (both versions)

| System | Kind | Interaction | Evidence |
|---|---|---|---|
| SQL Server | Database server | OLE DB `SQLOLEDB.1` over LAN; `MSDataShape.1` for shape/union queries | `data_connection.udl`, `data_connection1.udl` |
| Jet 4 / Access | Embedded database | Default store | `Patients.mdb` |
| Windows spooler | OS service | Print dispatch via `OpenPrinterA`/`SetPrinterA`/`GetPrinterA`/`ClosePrinter` | `printerselect` module |
| Windows regional settings | OS | Date-locale guard | CHM `001.htm` |
| Send To OneNote 2007 | Local application | Bound as a printer destination | `SETTING.ini`, `Data\*.ini` |
| RealLabeg.com | Website | Published in invoice layout | `IDCARD.INI` |
| Facebook page | Social | Published in invoice layout | `IDCARD.INI` |
| (V2) NATIGH.COM | Web service | Result delivery | `Natigh`, `SMSSender`, `sms*` tables, `ReportSMS` |
| (V2) SMS gateway | (provider unknown) | Outbound SMS | `SMSSender`, `SMStemplate`, `smslist`, `smsgroups` |

## 10. Known limitations

- Runtime behaviour was never observed (static analysis only).
- `VIDAS` database never structurally characterised.
- Band-selection algorithm among overlapping reference ranges not established.
- `pregsmok` dimension not traced.
- `pricemethod` value domain not established.
- Which `Branch_Data.mdb` copy is loaded not established.
- 38 columns in Access tables never applied by migration (partial deployment).
- `Patients_backup.mdb` stale in both versions.

## 11. Version applicability

- Layered architecture, modules, dependencies: **SHARED** (V2 extends V1).
- NATIGH/SMS, PDF, multi-branch, statistics, work sheets, farm reporting, schema migration, extended permission model: **V2 ONLY**.
- `MSVBVM60.DLL` import count differs (186 vs 231) — V2 uses more runtime functions.

## 12. Related documents

- `Docs/KB/legacy/VERSIONS.md` — version identity and delta.
- `Docs/KB/legacy/data/MODEL.md` — data dictionary.
- `Docs/KB/legacy/data/IDENTIFIERS.md` — identifier contracts.
- `Docs/KB/legacy/architecture/DEPLOYMENT.md` — deployment.
- `Docs/KB/legacy/architecture/CONFIGURATION.md` — configuration.
- `Docs/KB/legacy/architecture/PDF-PRINTING.md` — PDF/printing.
- `Docs/KB/legacy/clinical/RULES.md` — clinical rules.
- `Docs/KB/legacy/business/SECURITY.md` — security.
- Original source evidence: external: Complete_Reverse_Engineering_Reference_V1.md (external), `Docs/Complete_Reverse_Engineering_Reference_V2.md`, external: 01-architecture.md, external: architecture.md.
- `Database documentation.md`, `Documenting the Executable File.md` (root).
