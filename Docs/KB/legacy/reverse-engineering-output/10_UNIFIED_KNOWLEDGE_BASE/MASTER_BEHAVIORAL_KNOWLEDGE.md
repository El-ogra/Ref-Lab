# MASTER BEHAVIORAL KNOWLEDGE — REAL LAB SYSTEM v1.605

## 0. Governing fact

**No runtime observation of the target exists.** The v1.605 binary is absent from the evidence repository; no target binary was executed in this or the prior investigation. Every behavioral statement below is a *static reconstruction* from SQL literals, VB6 object structures, schema, configuration, and attributed prior documentation. See `07_RUNTIME_AND_BEHAVIORAL_ANALYSIS\Runtime_Observations.md`.

## 1. Startup / initialization (reconstructed)

1. Windows PE loads; VB6 runtime `ThunRTMain` transfers to the startup form — `labmain` (no `Sub Main` in the v1.00 project; v1.605 startup form INFERRED by product-line continuity). [v1.00 VERIFIED; v1.605 INFERRED]
2. Main form and controls instantiate; Arabic labels from `ui-labels-verified.md` / INI resources.
3. Configuration read from `BASE.ini`, `SETTING.ini`, `IDCARD.INI`, `NOTE.ini`, `REPORT.dll`, `Data\*.ini` via profile API. [mechanism STRONGLY_SUPPORTED; exact order INFERRED]
4. Data connection chosen (Access Jet vs SQL Server via `SQLOLEDB.1`/`MSDataShape.1`/UDL).
5. Printer resolution via `printerselect` and `[selectprinter]` settings.
6. Operator login against the `patients` account table; grant flags gate forms.
7. Main menu exposes modules: patients/orders, results, catalogue, billing, reporting, settings, statistics, send-out, microbiology.

Documented environmental requirement: English regional date format, server machine running (`Functionality_Requirements_Matrix_V1.md` F-V1-001).

## 2. Event-flow model (INFERRED)

VB6 event-driven flow: Form_Load → config/DB open; command click → form handler → `labdb`/`labdb1` → SQL or bound recordset → grid/DataReport/printer. Writes use bound recordsets (no literal INSERT/UPDATE). Per-method handler names are unavailable for most of the 8,026 methods, so no per-method event table is asserted.

## 3. Core workflows

| Workflow | Evidence | Confidence |
|---|---|---|
| Worklist by `requestdate` + `completes`, joined to `patienttest` | SQL VAs `682F56`, `683C72`, `6843F2`, `68463A` | STRONGLY_SUPPORTED (query structure) |
| Test selection via SHAPE subgroup→testtable | VA `672722`, `6729D6` | VERIFIED (literal) |
| Reference-range binding via SHAPE normalrange→testtable | VA `672E3E`, `673172` | VERIFIED (literal) |
| Result entry → `resultfinsh` + `edituser`/`editdate` | VA `6860BE` | VERIFIED (fields); timing INFERRED |
| Review → `resultreview` + `prevuser`/`prevdate` | VA `686312` | VERIFIED (fields); rule INFERRED |
| Print → `resultprint` + `printuser`/`printdate` | VA `6865A6` | VERIFIED (fields); release semantics INFERRED |
| Patient deletion cascade over 64 tables | VAs `6A740A`+ | VERIFIED (statements); transaction INFERRED |
| Backup device management | VAs `66FCB2`, `66FD56` | VERIFIED (statements) |
| Reporting/printing via DataReport + spooler/Bullzip | object dump + prior docs | COMPONENTS VERIFIED; pipeline INFERRED |

## 4. State model

Result state is carried by `patienttest.printwith`, `resultfinsh`, `resultreview`, `resultprint` (read together at VA `681ED2`) with three operator/date audit pairs, plus `patientinfo.completes` for patient-level completion. The natural reading is entry → review → print, but **no observed state machine exists** and no alternative priority model is invented. Reset/recheck/reprint semantics are UNKNOWN.

## 5. Error/exception behavior

- One literal error string at VA `6717E2` ("An error has occurred, Please make sure your browser, Or select other") — context unestablished.
- Locale sensitivity: documented English date-format requirement; SQL uses both quoted dates and `#…#` Jet literals (VAs `68313E` vs `6834FE`).
- 268 embedded DDL statements with partial application to the shipped DB indicate a migration capability; its trigger (install-time vs startup) is UNKNOWN.

## 6. Communication behavior

Serial analyser link (`MSCOMM32.OCX`, `CreateFileA`, `DeviceIoControl`), TCP (`MSWINSCK.OCX`), SMS provider, and e-mail via the auxiliary .NET component are evidenced as *capabilities*; protocols and actual message flows are UNKNOWN.

## 7. What dynamic analysis would resolve (NOT EXECUTED)

Nine controlled experiment designs (E-1…E-9) are specified in `07_RUNTIME_AND_BEHAVIORAL_ANALYSIS\Controlled_Experiments.md`: startup/connection, result lifecycle, simple vs composite handling, write path, deletion cascade transactionality, financial formula, printing, permissions, migration trigger. All have **status NOT EXECUTED**; no fabricated outcomes exist.
