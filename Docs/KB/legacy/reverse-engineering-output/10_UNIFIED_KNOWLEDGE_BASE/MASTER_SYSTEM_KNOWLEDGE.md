# MASTER_SYSTEM_KNOWLEDGE.md — Real Lab System 1.605

**Target version:** Real Lab System 1.605 ("RealLab CD 2019"), Clinical Laboratory Information System, by *ais group corporation*
**Purpose:** Single unified reference enabling another agent/engineer to understand and reconstruct the target system.
**Evidence basis:** `Sources.zip` (2,843 extracted files, 5 nested archives, 11 prior batches) + `RealLab-Reverse-Engineering-Evidence-Batch-02.zip`.
**Confidence:** Facts (Class 1), inferences (Class 2), hypotheses (Class 3), unresolved (Class 4) are explicitly distinguished throughout.

---

## 1. System identity

| Property | Value |
|---|---|
| Product | "real lab system" (RealLab) |
| Vendor | ais group corporation (© 2010) |
| Version | 1.605 ("RealLab CD 2019") |
| Domain | Clinical Laboratory Information System (LIS) |
| Platform | 32-bit Windows, VB6 native code, ActiveX/COM |
| UI language | Arabic (RTL) + English |
| Data layer | SQL Server (system of record) + Access/Jet 4 (offline/branch mirror) |

Two shipped builds are present in the evidence: **V-ROOT-1.00** (2013, 59,150,336 B, FileVersion 1.00) and **V-CD-2019 / v1.605** (2020, 76,292,096 B). The v1.605 executable is authoritative (SHA-256 `c54ce6c8…` matches the B1-recorded digest).

## 2. Architecture

Classic **two-tier desktop LIS**: a rich VB6 Windows client (native code) talking to SQL Server via ADO 2.7/SQLOLEDB.1, with a local Access (Jet 4) mirror for branch/offline work. No service layer; the client embeds data access, SHAPE queries, and a runtime DDL migration engine.

Layers: Presentation (132 forms + 66 DataReports) → Business logic (VB6 modules, 8,026 methods) → Data access (ADO) → Reporting (DataReports + 228 RTF templates + 146 INI configs) → Configuration (INI/UDL) → Deployment (Setup scripts, VS6 installer).

## 3. Technology stack

- **Language:** Visual Basic 6, compiled to **native code** (6 independent evidence lines; F-01).
- **Runtime:** MSVBVM60.DLL build 9782 (SP6), VBA6.DLL — out of support.
- **Data access:** ADO 2.7 (msado27.tlb), SQLOLEDB.1, Jet 4 (Access).
- **Database:** SQL Server (`Patients` catalog; servers REAL_LAB/DESKELTEL/REALLAB_AIS) + Access Jet 4 MDBs.
- **UI:** 132 forms + 66 DataReports; Microsoft OCX set (MSCOMCTL, MSADODC, MSDBRPT, MSCOMM32, …) + 5 third-party control packs (ChamaleonButton, RgheedButtonControlV2, XP_BUTTONS, vkUserControlsXP, Redsoul911XPCOmponents) + vendor NOTEX.dll/REPORT.dll/unrar.dll.
- **Reporting:** MSDBRPT.DLL DataReports, 228 cached RTF templates, 146 per-test INI configs, barcode fonts (FREE3OF9, WBCW_UPCEAN), Arabic fonts (MCS Taybah, ais lab, ais symbol).
- **.NET side:** "RealLab E-Mail Sender.exe" (.NET 4.0, VB.NET) — SMTP + SQL client.
- **Auxiliary apps:** real lab Image Gallery.exe, Patient Data Transport.exe, RLS Access/SQL DB Tools, RLUTool, RL_SETUP.exe, Setup*.bat; (calculator.exe, real lab timer.exe referenced but missing).
- **Deployment:** VS6 Setup Wizard, Bullzip PDF Printer (evaluation mode), Avira AV engine (orphaned), unrar 3.50, unsigned system DLL copies.
- **No ASLR/DEP/NX** on any binary (DllCharacteristics=0); 41 of 62 PE binaries unsigned.

## 4. Database knowledge

### 4.1 Backends and roles
- **SQL Server** — system of record (catalog `Patients`).
- **Access Jet 4** — offline/branch mirror: `Patients.mdb` (93 tables, 1527 cols, 3123 rows), `Patients_backup.mdb` (90, 1381, 2786), `Branch_Data.mdb` (49, 969, 0 — template), `data.mdb` (3, 34, 0 — shared masters).

### 4.2 Schema shape (133 tables)
- 96 per-test profile result tables (ABO, BILH, BloodGases, CRP, CSFT, CoagFactors, cbc, liver, kidney, lipids, stool, urine, hepatitis, hormones, Tumors, TORCH, ZN, semen, …) — each keyed by patientid+requestdate with analyte columns + comment.
- 33 configuration/master tables: patientinfo (33 cols), patienttest (44), testtable (37), normalrange (18), grouptable, subgrouptable, constanttable, pricelist, testunits, testname, instrument, barcodes, phone, sections, payment, history, comments, …
- 1 generic 20-column dynamic history table, 1 patient master, 1 test-order/result index, 1 work-analysis list with prices.

### 4.3 Critical naming trap
The table named **`patients`** is the **operator/user account** table (patientusname + grant1..grant12 + shift schedule) — NOT patient demographics. Demographics live in `patientinfo`; orders in `patienttest`. This corroborates the default `admin`/`admin` operator account finding.

### 4.4 Catalogue and reference data
- **testtable:** 609 tests, 48 subgroups, 591 priced, 605 delete-protected, 94 active; subgroups: BLOOD PICTURE, SERODIAGNOSIS, Trace Elements, Autoimmune diseases, KIDNEY FUNCTIONS, MOLECULAR BIOLOGY, CULTURE AND SENSITIVITY, Hepatitis Markers, LIVER FUNCTION, …
- **normalrange:** 852 rows / 304 tests; sex-stratified (M 422 / F 421); age units Years 708 / Days 70 / Mons 65.
- **constanttable:** PTCONS (PT/INR: 12, 35, 1.33, …) and CBCCONS (CBC: 3.3, 6.25, 6.75, 7.5, 8.25).
- **pricelist:** per-test pricing (patient/lab/other-lab).

### 4.5 Schema evolution (V1 → V2019)
103 table/column deltas; new tables `FMF`, `Helicobacter`; new columns `Abuse.tramadol/ecstasy`, `BloodGases.pO2AA1/ctCO2`. Branch_Data.mdb is a 0-row structural template.

## 5. Behavioral knowledge

### 5.1 Core workflows
1. **Login:** operator authenticates (default `admin`/`admin`, grant1..grant12 authorisation).
2. **Registration:** patient demographics → `patientinfo`.
3. **Ordering:** tests selected from `testtable` → `patienttest`.
4. **Result entry:** per-discipline `Result*` form writes analyte columns into the matching profile table (keyed patientid+requestdate).
5. **Validation:** entered values compared against `normalrange` (sex/age).
6. **Derivation:** PT/INR and CBC derived via `constanttable`.
7. **Pricing:** patient/lab/other-lab prices; send-out to other labs (`inotherlab`, `otherlabprice`).
8. **Reporting:** `Report*` DataReport renders via RTF template + per-test INI config + barcode print.
9. **Notification:** SMS (smsmisr.com) and/or e-mail (SMTP via .NET component).
10. **Send-out:** tests with inotherlab=True routed to external lab.
11. **Sync:** local Access mirror for branch/offline.
12. **Backup:** generated `.bat`/`.sql` to configurable server/path.
13. **Startup:** schema-migration engine runs DDL against the clinical DB.

### 5.2 Business rules
- Catalogue & grouping: 609 tests in 48 subgroups; hierarchical group→subgroup→test.
- Reference-range validation: 852 ranges, sex/age stratified.
- Derived values: PT/INR and CBC constants shipped as user-editable (U-035).
- Pricing: patient/lab/other-lab per test; send-out routing.
- Authorisation: per-patient grant1..grant12 flags + per-section IDPassword* print passwords (coarse, undocumented).
- Multi-branch: Branch_Data.mdb template + branch column.
- Microbiology: cultures, cultureanti, culturesec; CULTURE AND SENSITIVITY subgroup.
- QC: Reportqc DataReport.
- Delete protection: 605/609 tests; active: 94/609.

### 5.3 Simple vs composite tests
The system distinguishes simple (single-component, e.g. creatinine, urea, uric acid) and composite (multi-component, e.g. CBC) tests. Each profile table carries its own analyte columns; composite profiles (cbc, liver, kidney, …) have many columns, simple ones (ABO, CRP) have few. The exact representation differs per profile — do not assume uniform handling.

### 5.4 State transitions
Per-patient grant model + per-section print passwords form a coarse authorisation scheme. The mapping of grant bits and IDPassword* values to specific sections is unresolved (UNR-002, UNR-003). Result states: entered → validated (vs normalrange) → printed (resultprint) → finished (resultfinsh).

## 6. User interface knowledge

- 132 forms + 66 DataReports; per-discipline `Result*`/`Report*` naming (Resultliver/Reportliver, Resultkidney, Resulturine/Reporturine, Resultsugar/ReportSugar, Resultlipids, resultculutre/reportculutres, ReportFatora, Reporttotal, Reporthistory, Reportinstrument, Reportsemen, statisReport, Reportqc, Reportcross).
- Core modules: labmain (71 methods), patientmain (84), labdb/labdb1 (data access), tools, testmain, addpatient, barcode, phone, moneypaid, testrange, workpaper, labfunction, printerselect, BCWFonts, saveIOtemp, otherfun, mInputbox; classes aaaClass, CRect, BiosData.
- Arabic RTL UI; report main titles: MICROBIOLOGY, MOLECULAR BIOLOGY, HEMATOLOGY, IMMUNOLOGY, ENDOCRINOLOGY, CYTOPATHOLOGY, SEROLOGY, BIOCHEMISTRY REPORT.
- Help: Arabic CHM (HTML Help Workshop 4.74, 17 topics, 79 images), © ais group corporation 2010.
- Barcode fonts (FREE3OF9, WBCW_UPCEAN, WBCWS_UPCEAN, Digiface); Arabic fonts (MCS Taybah, ais lab, ais symbol); training video RLS_Learn.wmv (85 MB).

## 7. Engineering knowledge

### 7.1 Data flow
Operator login → patient registration (patientinfo) → test order (patienttest) → result entry (Result* → profile table) → validation (normalrange) → derivation (constants) → pricing → report print (Report* + RTF/INI + barcode) → SMS/e-mail → sync (Jet mirror) → backup. Startup runs DDL migration.

### 7.2 Interfaces & integrations
| Integration | Mechanism | Confidence |
|---|---|---|
| SQL Server | ADO 2.7 / SQLOLEDB.1 | High |
| Access mirror | Jet 4 OLEDB | High |
| SMTP e-mail | System.Net.Mail (.NET 4.0) | Medium |
| SMS | smsmisr.com cleartext HTTP, creds in URL | High |
| Serial analysers | MSCOMM32.OCX; BF6500 hematology | Medium |
| IE automation | COM (InternetExplorer) | Medium |
| MySQL (possible) | ODBC connector referenced | Low (hypothesis) |

### 7.3 Configuration
BASE.ini (Initial Catalog=Patients, DataSource), data_connection*.udl (SQLOLEDB.1), SETING.ini, IDCARD.INI, NOTE.ini, 146 Data\*.ini (report layout), default printer HP LaserJet 1100 (MS), fonts Arial/Tahoma/Times New Roman.

### 7.4 Error handling
Bulk-deletion maintenance tools over every clinical table; schema-migration engine runs DDL at startup; IE COM automation for HTTP. No structured exception handling evident beyond VB6 On Error.

## 8. Evidence and traceability

- All findings traceable via EVD-* IDs (461 evidence items in Sources.zip + prior batch) and FIND-* (39 findings across 3 stages).
- Registries: findings_registry (39), evidence_registry (461), relationship_registry (15), contradiction_registry (5).
- Contradictions: CON-001 (Jet 4, resolved), CON-002 (object count, provisional version delta), CON-003 (SELECT count, immaterial), CON-004 (native code, resolved), CON-005 (25 vs 20 components, resolved).
- 41 consolidated security findings (9 High): default admin/admin, hardcoded SQL sa password, cleartext operator passwords, bulk-deletion tools, unverified installer payloads, SMS cleartext, unsigned binaries, Avira/unrar/PDF licensing.

## 9. Gaps and conflicts

- Unresolved: UNR-002 (grant bits), UNR-003 (IDPassword*), UNR-004 (32-char literals), UNR-005 (accuntpw hashing), UNR-008 (SELECT-to-backend mapping), UNR-009 (5 unnamed components).
- Missing evidence: RL_CD2019Access/System/real lab system.exe, calculator.exe, real lab timer.exe, real lab Image Gallery.exe (binary), vkUserControlsXP.ocx, Avira engine, Attaching.sql, VS6sp6B cabinets, Patients_backup.mdb (as received file).
- Possible third backend: MySQL (hypothesis, low confidence).

## 10. Knowledge completeness assessment

| Domain | Coverage | Notes |
|---|---|---|
| File extraction | High | 2,843 files extracted, 0 errors |
| Text/SQL analysis | High | schemas, xrefs, strings, ghidra funcs parsed |
| Architecture | High | PE/VB6 structure, dependencies, components |
| Database | High | 133 tables, schema, evolution |
| Behavioral | Medium-High | workflows inferred from forms/SQL; runtime not observed |
| Business rules | Medium | inferred from schema/SQL, not decompiled |
| UI/interaction | Medium | form names + CHM text; no runtime screenshots |
| Financial | Medium | pricing/consts inferred |
| Printing/reports | High | 66 DataReports + 228 RTF + 146 INI |
| Security/permissions | High | 41 consolidated findings |
| Error handling | Low-Medium | VB6 On Error; no structured handling evident |
| Relationships/deps | High | 15 relationships registered |

**Overall: ~80-85% of recoverable knowledge from the provided evidence.** Remaining gaps require the missing binaries, decompiled method bodies, and runtime observation. No claim of 100% completeness is made.