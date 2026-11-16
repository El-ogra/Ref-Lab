# Application Workflows (Static Reconstruction — NOT Runtime Observed)

Each workflow below is reconstructed from converging static evidence: SQL statement literals (with virtual addresses), VB6 object/method inventory, schema DDL, and prior functional specifications. Every workflow carries a confidence label. No workflow was executed.

## 1. Registration → order → result → review → print → bill (INFERRED end-to-end)

| Step | Evidence | Confidence |
|---|---|---|
| Patient registered; 13-character `patientid` assigned | `patientinfo` columns in DDL; `SUBSTRING(patientid,10,3) AS patid` in SQL (VAs `682F56`, `683C72`, `6843F2`); prior ID-scheme documentation | STRONGLY_SUPPORTED for ID format and storage; INFERRED for assignment algorithm |
| Tests selected and written to `patienttest` (one row per test, per order) | 343 statements reference `patienttest`; `patienttest.patientid`, `.testcode`, `.testsimp`, `.requestdate` in column-count CSV | STRONGLY_SUPPORTED for structure |
| Result entry sets `resultfinsh` + `edituser`/`editdate` | SQL VA `6860BE` | STRONGLY_SUPPORTED for field; INFERRED for timing |
| Review sets `resultreview` + `prevuser`/`prevdate` | SQL VA `686312` | STRONGLY_SUPPORTED for field; INFERRED for authorization rule |
| Printing sets `resultprint` + `printuser`/`printdate` | SQL VA `6865A6` | STRONGLY_SUPPORTED for field; INFERRED for release semantics |
| Pricing/billing uses `pricelist`, `patientinfo.totalprice`, `patientinfo.totalpricedisc`, `elmadfoo3` (paid), `elbakyma3mal` (remaining) | GROUP BY clauses at VAs `6977A6`, `69DE12`, `69E4E6`, `69F48A`, `69FB46`, `6A16A2`; `payment` table (32 statements) | STRONGLY_SUPPORTED for columns; INFERRED for calculation formula/rounding |

## 2. Worklist workflow (STRONGLY_SUPPORTED query structure)

Main worklists filter on `requestdate` and read `completes`, `patientnamear`, `referredbyname`, `gehaName`:

- `SELECT … completes FROM patientinfo WHERE requestdate = '…'` (VA `682F56`)
- `… requestdate >= '…'` (VA `683C72`)
- `SELECT patientinfo.patientid, patientinfo.patientnamear, patientinfo.completes FROM patientinfo INNER JOIN patienttest ON patienttest.patientid = patientinfo.patientid WHERE patienttest…` (VA `6843F2`)

The `INNER JOIN` demonstrates that test-level state is aggregated to patient-level worklist display. [VERIFIED: literal join in evidence]

## 3. Hierarchical catalogue/result binding (SHAPE commands, VERIFIED)

27 `SHAPE` statements build hierarchical recordsets:

```
SHAPE {SELECT subgrouptable.subgroupcode, subgrouptable.subgroupname FROM subgrouptable …}
  APPEND ({SELECT * FROM testtable WHERE testorno='True' AND testsimp<>'' …} AS cmdbill_d
          RELATE subgroupcode TO subgroupcode) AS cmdbill
```
(VA `672722`, `6729D6`; analogous `normalrange`→`testtable` shapes at `672E3E`, `673172`.)

This is the mechanism for grouped test catalogues and reference-range display in a DataCombo/DataGrid hierarchy. [VERIFIED: literal SHAPE strings; INFERRED: exact UI control binding]

## 4. Patient deletion / cleanup (VERIFIED literal sequence; INFERRED transaction)

A contiguous block of `DELETE FROM <table> WHERE patientid='…'` spans 64 tables (VAs from `6A740A`): `patientinfo`, `payment`, `patienttest`, `ABO`, `Abuse`, `BILH`, `cardiac`, …, `widal`, `ZN`. This is an application-level cascade (see `State_Transitions.md` §5). [VERIFIED: statements; INFERRED: single-transaction semantics and triggering screen]

## 5. Backup / restore / data transport (STRONGLY_SUPPORTED mechanism)

- `EXEC sp_addumpdevice 'disk', '…'` (VA `66FCB2`) and `EXEC sp_dropdevice '…'` (VA `66FD56`) — SQL Server backup device management.
- `DELETE FROM wasl` (VA `6894F2`) — clearing the `wasl` (link/connection) table.
- Companion executables: `RLS ACCESS Database Tools.exe`, `RLS SQL Database Tools.exe`, `Patient Data Transport.exe`, `RLUTool.exe`, plus `Attach_Interface.bat`/`Attach_VIDAS.bat` references in prior work.
- Conclusion: the product ships dedicated data movement/maintenance utilities. [STRONGLY_SUPPORTED] Their exact UI workflows are UNKNOWN (binaries not executed).

## 6. Reporting / printing (INFERRED pipeline)

SQL → hierarchical recordset → VB6 DataReport designer (66 objects) → Win32 spooler (`OpenPrinterA`/`SetPrinterA`/`GetPrinterA`/`ClosePrinter`) and/or Bullzip PDF Printer → physical or PDF output. RTF method sheets and INI-based `REPORT.dll` templates supply layout. [STRUCTURE VERIFIED; ORDER INFERRED]

## 7. Cross-cutting configuration guard

English regional date format is a documented requirement (`Functionality_Requirements_Matrix_V1.md`); SQL literals use both quoted dates and `#…#` delimiters (e.g. VA `68313E` vs `6834FE`), consistent with Jet/Access date literal handling and a locale-sensitive path. [STRONGLY_SUPPORTED for the two literal forms; INFERRED for failure behavior]

## 8. Confidence summary

| Workflow | Confidence | Reason |
|---|---|---|
| Worklist query structure | STRONGLY_SUPPORTED | exact SQL literals |
| SHAPE hierarchy | STRONGLY_SUPPORTED | exact SQL literals |
| Deletion cascade | STRONGLY_SUPPORTED (statements) / INFERRED (transaction) | 64 literal DELETEs |
| Result flag data flow | STRONGLY_SUPPORTED (fields) / INFERRED (sequence) | paired SELECT statements |
| Financial calculation | INFERRED | columns exist; formula and rounding not recovered |
| Reporting pipeline | INFERRED | components identified; runtime path not observed |
| Registration/ID assignment | INFERRED | format evidenced; algorithm not |
