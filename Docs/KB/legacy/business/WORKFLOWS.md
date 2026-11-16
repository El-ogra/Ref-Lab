# Legacy Business Workflows — V-ROOT-1.00 and V-CD-2019

**Purpose:** Curated reference for reconstructed legacy workflows.
**Scope:** Patient lifecycle, result entry, send-out, accounting, reference ranges, catalogue.
**Audience:** Developers, analysts.
**Version applicability:** Mixed (noted per workflow).

---

## 1. Workflows are reconstructed, not observed

All workflows below are reconstructed from evidence (code literals, extracted schema, configuration
files, official manual). They are **not** observed executions. Confidence is stated per workflow.

**Evidence:** external: Complete_Reverse_Engineering_Reference_V1.md §4; `Docs/Complete_Reverse_Engineering_Reference_V2.md` §4.

## 2. WF-01 — Patient episode

### V1 (WF-V1-01)

| Step | Action | Entities | Evidence | Confidence |
|---|---|---|---|---|
| 1 | Sign in | `useraccounts`, `loggroup` | CHM `002.htm`; forms `login`, `users`, `userspw` | CONFIRMED |
| 2 | Register a new patient | `patientinfo` (33 columns) | form `addpatient` (125 controls); CHM `003.htm` | CONFIRMED |
| 3 | Generate the patient barcode | `barcodes` (35 rows) | forms `barcode` (71 controls), `z_barcodes`; CHM `003A.htm` | CONFIRMED |
| 4 | Request tests | `patienttest` (`testcode`, `subgroupcode`, `arrangnum`, `testprice`) | SQL inventory | STRONGLY SUPPORTED |
| 5 | Enter the result | per-profile result table + `patienttest` | forms `result*`; CHM `004.htm` | CONFIRMED |
| 6 | Review and confirm | `patienttest` review flag | F8 shortcut; CHM `004A.htm` | STRONGLY SUPPORTED |
| 7 | Print the report | `DataReport` objects + `Data\*.ini` | CHM `009.htm`; F11/F12 | CONFIRMED |
| 8 | Hand over the result | `patienttest.give`, `resultprint` | forms `frm_money`, `moneypaid`; CHM `005.htm` | CONFIRMED |

### V2 (WF-V2-01, extended)

Adds step 9: **Deliver digitally** → NATIGH.COM / SMS via `SMSSender`.

Review flag becomes explicit (`patienttest.resultreview`). New profile windows (IgE, H. pylori, PCR,
LBC, FMF, automated urine). Frozen range copy on the test row.

## 3. WF-02 — Profile result entry

| Step | Detail | Evidence | Confidence |
|---|---|---|---|
| a | Select the patient from the main window | `patientmain` → `patientview` | CONFIRMED |
| b | Select the test twice ⇒ the profile entry window opens | forms `result*` | CONFIRMED |
| c | Tests in the selected group are shown flagged | `.Normalbold` / `Check*x` literals | STRONGLY SUPPORTED |
| d | On entry the value is compared against `lowlimit`/`highlimit` from `normalrange`; the system displays abnormal (highlighted), the exception text (`nolowexe`/`nohighexe`), and critical | `normalrange` schema + `patienttest` columns | CONFIRMED |
| e | Comment entry — typed, or chosen from stored comments, or from the normal-ranges list («زر تعليق (البرتقالي)») | `comments` table; `normal_test` form | STRONGLY SUPPORTED |
| f | F9 save · F8 confirm · F11 preview · F12 print · Enter next-and-save · Esc cancel | CHM `004A.htm` | CONFIRMED |

## 4. WF-03 — Send-out samples

1. Tests marked `inotherlab = True` in `testtable` are discovered automatically.
2. Open via F7 or accounting → send-out samples (`sarfz`, `moenysharekat`, `moenymozafeen`).
3. Group by referring laboratory (`otherlabname`) and total.
4. Send — enter the amount settled, «خالص» (mark as sent), «دائماً إليه» (always to it).
5. Follow up unsent and unsettled.

Two independent states: `sent` (dispatched) and `sentout`/settled (V1-BR-024, V2-BR-044).

**Evidence:** CHM `010.htm`; `testtable`, `patienttest` schemas.

## 5. WF-04 — Accounting and stock-take

Reconstructed from CHM `011.htm`, `labfunction`, `payment` / `Gehats` schema:
1. Patient accounts for a period — "هل الفلوس المحصلة من مرضى الفترة فقط؟" (operator decision).
2. Send-out sample accounts — aggregated or per referring laboratory.
3. Expenses and remittances — separate cash withdrawal/deposit window.
4. Patients with residual balances + referring-party commissions (`Gehats.omola`).
5. Per-user workload and amounts collected per user.
6. Claims / volume of dealings with other parties.

V2 adds a **branch dimension** to the stock-take (V2-BR-010): a stock-take may span a master
laboratory and its linked branches.

Commission formula: `SUM(Gehats.omola * patientinfo.elmadfoo3 / 100)`.

**Evidence:** CHM `011.htm`; `Gehats` schema; extracted SQL.

## 6. WF-05 — Reference-range administration

Path in UI: بيانات النظام → بيانات التحاليل → [test] → «المعدل الطبيعي».
Operations: add a range · edit · save · undo · delete.

14 range attributes, all present in `normalrange` schema: test name, test unit, sex, age from/to,
age unit, normal range, low/high limit, low/high flag, low/high comment, critical comment,
critical range, critical low/high limit, critical flag.

> **★ SCHEMA CORRECTION (2026-11-16).** The sentence above is **not** accurate as written: the
> attributes are "all present in the manual", **not** "all present in the `normalrange` schema".
> The live `normalrange` table has **18 columns and no `critical*` column at all**. The five
> critical attributes exist only in the `Patients_backup.mdb` ancestor schema (22 columns). The
> shipped database therefore **cannot persist** a critical limit on the range row — which is why
> the product keeps critical thresholds in the `Cache\*.rtf` method sheets (V1-BR-035), now
> **VERIFIED at schema level**. Evidence: `txt nd sql/v2019-Patients.mdb.sql`,
> `txt nd sql/schema__[analysis_BATCH-02_db_Patients_backup].sql`. See `data/DATABASE.md` §6.1.

**★ The six-band duplication rule (2026-11-16).** The manual states that where a test does **not**
differ by sex, the same range must be entered **six times**:

| # | Sex | Age window |
|---:|---|---|
| 1 | Male | 0–120 years |
| 2 | Female | 0–120 years |
| 3 | Male | 1–29 days |
| 4 | Female | 1–29 days |
| 5 | Male | 1–11 months |
| 6 | Female | 1–11 months |

Where the range does differ by age, the age window is split instead (e.g. male 0–12 and 12–120
years, and the female equivalents). **This is why 304 tests yield 852 range rows** — the row count
is a function of the age-unit vocabulary each test actually uses, not a fixed multiple of the test
count. Evidence: CHM `008.htm`; `help-manual-ar.md` §10.

**★ Presentation behaviour.** An out-of-range result is rendered with **background text shading**
in the report. Flags appear during entry and during printing. Comments appear when the test name is
clicked. The critical-range text prints only when *"print critical ranges"* is enabled in the
report settings. The editor window has **no keyboard shortcuts**.

**★ Standing restriction:** topic `nots.htm` states the urine analysis carries test number 4, and
neither its number nor its name may be changed.

**Evidence:** CHM `008.htm`; `help-manual-ar.md` §10; `normalrange` schema (`v2019-Patients.mdb.sql`).

## 6.1 Operator procedures recovered from the official manual (2026-11-16)

The 17-topic CHM is the product's own operating manual. Its procedures were not previously
represented in this knowledge base at procedure level. The following are now recorded.

### Result entry

Double-click a test in the patient's test list to open its entry window. If the test belongs to a
profile (بروفيل), **all** profile tests are shown and only the selected ones are enabled. The
**«Constants»** button at the bottom-right of the entry window opens the live clinical-constants
editor.

**Evidence:** CHM `004A.htm`; `help-manual-ar.md` §6.

### Accounting and stock-take

**«حسابات» → «الجرد وحساب الدرج»**, or the **F7** shortcut from any main window.

**Evidence:** CHM `011.htm`; `help-manual-ar.md` §13.

### Backup

**«اعدادات» → «Database Maintenance»**, guarded by the maintenance password (default `123`).
Eight-part screen for large installations: change the maintenance password; zero all or part of the
system; select the database path; list previous backups; count them; show their size; the
auto-named backup file for today (**`Patient_yyyymmdd.bak`**, stored inside the database path);
delete; and restore. Small installations additionally allow **restoring a single patient by their
work code** (كود الشغل). The manual advises moving backups to a separate disc or machine.

**Evidence:** CHM `backup.htm`; `help-manual-ar.md` §14.

### Start-up and sign-in

1. The server-settings window carries **four** items: the server IP or machine name (with an
   «احضار» fetch button for use when the NIC or cable fails); the login-permission type; the user
   name and password; and the database name. The manual states that the permission type, the
   credentials and the database name are **fixed constants of the system** and must not be changed —
   only the server address varies per machine.
2. Allow the machine to finish loading, then wait **five seconds** for the database server.
3. On a non-English Windows locale the application refuses to start with **«رجاء تحويل التاريخ الى
   اللغة الانجليزية»**; the documented remedy is to set the regional format to **English (Canada)**.
4. Sign in by choosing the user name from the drop-down (`admin` on first use) and entering the
   password (case-sensitive; on first use the password equals the user name). «تسجيل دخول» signs in,
   «خروج» exits.

**Evidence:** CHM `001.htm`, `002.htm`; `help-manual-ar.md` §1–§2.

### Pre-flight constants checklist

The manual's closing notes require the operator to confirm, before the day's work: Prothrombin Time
(ISI, control time, concentration table), PTT (control time), and CBC (Hct). The documented
formulas are `Hct = Hgb × 3.3`; `Hgb% = Hgb × 8.25` (age < 1 yr), `× 7.50` (1–12 yr),
`× 6.25` (male, > 12 yr), `× 6.75` (female, > 12 yr). The urine analysis is fixed at **test
number 4** and cannot be renumbered or renamed.

**Evidence:** CHM `nots.htm`; `help-manual-ar.md` §15.

## 7. WF-06 — Catalogue interlock

```
normalrange (range)  ←→  converts (unit conversion)  ←→  testunits (unit dictionary)
          ↓
testtable (definition)  ←→  Data\*.ini (report layout)  ←→  Cache\*.rtf (method sheet)
```

The clinical sub-group is the routing key for both the entry window and the report layout
(V1-BR-031, INFERRED). 46 sub-groups exist while 223 `.ini` layout files exist; the mapping is
many-to-one and is not established (RL-V1-Q-07).

## 8. WF-V2-02 — Multi-branch operation

```
patientid layout:  <prefix><branch digit at position 9><suffix>
   └─ SUBSTRING(patientid,9,1) extracts the branch
   └─ Patient.branche names the branch
   └─ Labbranch holds branch master data
   └─ Branch_Data.mdb (49 schema-only tables) seeds a new branch's profile tables
   └─ The stock-take can be run across several linked branches
```

## 9. WF-V2-03 — PDF report production

```
Result reviewed (resultreview)
   └─► print request
        └─► printer resolution: Bullzip PDF Printer (hard-coded device name)
             └─► Bullzip port driver bzpdf.dll
                  └─► bundled Ghostscript renders the PostScript
                       └─► file written to D:\PDF\<Contracts|Free|Individual|LabToLab>\
                            └─► audit: printuser / printdate / printnum
```

Independent tools shipped alongside (never invoked by the application): Ghostscript Lite 9.14.17,
Xpdf Lite 3.3.4 (`pdftops.exe`), PDF Power Tool 4.0.0.11.

## 10. WF-V2-04..07 — V2 additions

- **WF-V2-04 — Statistics:** `statisReport` — patient count by month/year split by sex; request
  rate for a test group; sample count for the year.
- **WF-V2-05 — Attendance:** `loggroup` + `loger` (new table) — «تسجيل الحضور والانصراف».
- **WF-V2-06 — Work sheet:** `workpaper` / `workpaper2` / `wasl` — work sheet over a patient
  group in a period, or over a test/test group in a period.
- **WF-V2-07 — NATIGH/SMS delivery:** `Natigh` (installed file), `SMSSender`, `SMStemplate`,
  `smsgroups`, `smslist`, `SMSfrmsenderos` (139 methods), `ReportSMS` → patient receives the
  result through the NATIGH.COM site; receipt header/footer rendered per `IDCARD.INI`/`SETTING.ini`.

## 11. V1 error handling

| Condition | Behaviour | Evidence |
|---|---|---|
| Database server not reachable | «خادم قواعد البيانات غير معرف»; application closes on OK | CHM `001.htm` |
| Non-English date locale | «رجاء تحويل التاريخ الى اللغة الانجليزية»; application closes on OK | CHM `001.htm` |
| No printer available | "Can't find any printer." (English literal) | literals |
| Printer API failure | "SetPrinter Failed. Error code: " + error code | literals |
| Client machine, server not started | Manual requires starting the server machine | CHM `001.htm` |
| Network share/permissions suspect | Manual instructs checking switches and share permissions first | CHM `001.htm` |

## 12. Version applicability

- Patient lifecycle, profile result entry, send-out, accounting, reference-range administration,
  catalogue interlock: **SHARED** (V2 extends V1).
- Multi-branch operation, PDF report production, NATIGH/SMS delivery, statistics, attendance,
  work sheet: **V2 ONLY**.

## 13. Related documents

- `Docs/KB/legacy/clinical/RULES.md` — clinical rules.
- `Docs/KB/legacy/clinical/KNOWLEDGE-LAYER.md` — knowledge tables.
- `Docs/KB/legacy/data/MODEL.md`, `IDENTIFIERS.md`, `BRANCHES.md`.
- `Docs/KB/legacy/architecture/PDF-PRINTING.md` — PDF/printing.
- `Docs/KB/legacy/business/SECURITY.md` — security.
- Original source evidence: external: Complete_Reverse_Engineering_Reference_V1.md §4,
  `Docs/Complete_Reverse_Engineering_Reference_V2.md` §4, external: 05-workflows.md.
- Modern implementation: `src/RefLab.Application/Features/Patients/*`, `Results/*`, `SendOut/*`,
  `Billing/*`, `Reporting/*`, `Worksheets/*`, `Statistics/*`.
