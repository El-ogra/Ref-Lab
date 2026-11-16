# Data Flow Reconstruction — REAL LAB SYSTEM v1.605

**Purpose.** End-to-end clinical data flow from login through registration, order,
result entry, validation, printing and billing, expressed as SQL literals and tables
from the BATCH-04 inventory. Each step carries its own confidence label.

**Reading rule.** VERIFIED = the SQL literal and table are in the evidence file;
STRONGLY_SUPPORTED = multiple independent evidence classes agree on the step;
INFERRED = the step is reconstructed from naming/ordering, not from a call trace;
UNVERIFIED / UNKNOWN = stated where the evidence stops.

---

## 0. Entry and configuration preconditions

| Step | Evidence | Status |
| --- | --- | --- |
| Startup selects database type (Access `Microsoft.JET.OLEDB.4.0` vs SQL Server 2005) | `08-deployment-installer.md` §2; `BASE.ini` `[oledb] Databasetype`, `loca`, `InitialCatalog` | STRONGLY_SUPPORTED |
| Connection object built from provider fragments (`SQLOLEDB.1`, `MSDataShape.1`) and credentials assembled at run time | `database-and-data-access-analysis.md` §2 | STRONGLY_SUPPORTED (values withheld) |
| Runtime DDL migration literals available | `ddl-evidence__[analysis_BATCH-04_sql_v1605].txt` (268 literals) | VERIFIED; execution trigger UNKNOWN |
| Operator login reads `patients` (operator store) | VA `6EF640` `SELECT * FROM patients where patientusname= '`; VA `6D6F18` reads work-time fields | VERIFIED |
| Permission grants stored as `grant1..grant17` columns | `Patients ADD grant13 Bit` (`6DC31C`) and Jet `YESNO` (`6DC4B0`); prior report §4.3 | VERIFIED (column names); semantics INFERRED |

---

## 1. Patient registration

| Action | SQL / table | Evidence | Status |
| --- | --- | --- | --- |
| Duplicate check by Arabic name | VA `687166` `Select patientnamear from patientinfo where patientnamear = '` | statement CSV | VERIFIED |
| Duplicate check by lab ID | VA `687106` `Select labid from patientinfo where labid = '` | statement CSV | VERIFIED |
| Search-as-you-type | VA `6AAAA06` `Select Top 25 patientnamear from patientinfo WHERE patientnamear LIKE '` | statement CSV | VERIFIED |
| Full record load | VA `6AAB0A` `Select * from patientinfo where patientnamear = '`; VA `6AAD36` `…where labid ='` | statement CSV | VERIFIED |
| Registration detail columns | `patientinfo.patientnamear` (272 ref), `.labid` (213), `.patientsex`, `.patientage`, `.patientageunit` (28 each), `.requestdate` (103) | `columns__[analysis_BATCH-04_sql_v1605].csv` | VERIFIED |
| **Write of the new patient row** | no `INSERT` literal (0 / 1,797) | statement CSV | **UNVERIFIED** — likely ADO `AddNew`/`Update` (INFERRED) |

---

## 2. Order creation (test selection)

| Action | SQL / table | Evidence | Status |
| --- | --- | --- | --- |
| Catalogue grouped for selection | VA `672722` `SHAPE {… FROM subgrouptable …} AS cmdbill APPEND ({SELECT * FROM testtable where testorno='True' …} RELATE subgroupcode TO subgroupcode)` | statement CSV | VERIFIED |
| Price-aware catalogue (patient price) | VA `672E3E` `SHAPE {… patientprice … FROM testtable …} APPEND ({… FROM normalrange …} RELATE testcode TO testcode)` | statement CSV | VERIFIED |
| Lab-price variant | VA `673662` (`labprice` instead of `patientprice`) | statement CSV | VERIFIED |
| Ordered tests for a patient | VA `68403E` `SELECT * FROM patienttest where patientid= '` | statement CSV | VERIFIED |
| Status/flag columns on the order | VA `681ED2` `printwith, resultfinsh, resultreview, resultprint`; VA `686EC2` adds `RESULTT, RESULTCASEPRINT, give, NatighU` | statement CSV | VERIFIED |
| Temporary selection table | `Create Table testtemp (patientid varchar(13) …` (`674396`); `Delete FROM testtemp` (`6A868A`); `Delete FROM testname` (`6B7B84`) | DDL evidence + statement CSV | VERIFIED |
| **Write of the order rows** | no `INSERT` literal | statement CSV | **UNVERIFIED** (ADO recordset write INFERRED) |

---

## 3. Result entry

| Action | SQL / table | Evidence | Status |
| --- | --- | --- | --- |
| Per-discipline form loads its result table | VA `6A3336` joins `patientinfo` ↔ `liver` by `patientid`; VA `6B348C` joins `patientinfo` ↔ `sugar` | statement CSV | VERIFIED |
| Reference text shown during entry | VA `68AB1E` `SELECT normalrange.normalrang, normalrange.testunit, testtable.testabrev, testtable.testname … where normalrange.testcode=564 and normalrange.sex='` (and `565`, `550`, `562` at `68AEEA`, `68B0DE`, `68B2D2`) | statement CSV | VERIFIED |
| Shared range lookup | `SELECT * FROM normalrange where testcode=` reused at many code sites | `normalrange-coderefs.json` → `procedures` | VERIFIED (string + sites); owning object UNKNOWN |
| Antibiogram setup | VA `674D7E` `SELECT * FROM culturesec where secabre='RA'` (and `HB`, `MB`, `RB`, `LA`, `LB`, `HC`, `MC`, `LC`, `RC`) | statement CSV | VERIFIED |
| Result-table corpus | ~50 per-discipline tables each 9–13 statements; many have `Delete FROM <table> where patientid='` in cascade block | `tables__[analysis_BATCH-04_sql_v1605].csv` | VERIFIED |
| **Write/update of result values** | no `UPDATE` literal | statement CSV | **UNVERIFIED** (bound controls / ADO update INFERRED) |

---

## 4. Validation and review

| Action | SQL / table | Evidence | Status |
| --- | --- | --- | --- |
| Outstanding results work list | VA `6843E2` `… FROM patientinfo INNER JOIN patienttest … Where patienttest.resultfinsh = 'False' …` (SQL Server form); VA `68463A` Jet twin with `=False` | statement CSV | VERIFIED |
| Finished-flag read | VA `681ED2`, `6860BE` (`resultfinsh, edituser, editdate`) | statement CSV | VERIFIED |
| Review stamp | VA `686312` (`resultreview, prevuser, prevdate`) | statement CSV | VERIFIED |
| Print stamp | VA `6865A6` (`resultprint, printuser, printdate`) | statement CSV | VERIFIED |
| Result flagged for critical/abnormal display | `normalrange` fields `normalrang, testunit, REFRANGE` (`ALTER Table normalrange ADD COLUMN REFRANGE varchar(1)` at `680F22`); `REFRANGE='1' and pregsmok` / `REFRANGE='2' and ageunit='` code refs | DDL evidence + `normalrange-coderefs.json` | VERIFIED |
| Range statistics (database content, not SQL counts) | 852 ranges, 304 distinct tests; sex `Male 422 / Female 421 / All 5 / blank 4` | `normalrange-stats.json` | VERIFIED |
| **Flag-write statements** | no `UPDATE` literal | statement CSV | **UNVERIFIED** |

---

## 5. Printing / report assembly

| Action | SQL / table | Evidence | Status |
| --- | --- | --- | --- |
| Report banner / strip text | VA `66F882` `SELECT stripnum,stripdiscrp,strip1 FROM reportview where stripdiscrp='MAIN'`; VA `6DA5AC` adds `strip2..4` | statement CSV | VERIFIED |
| Barcode metadata join | VA `6B18C8` `SELECT … testtable.barcode1, barcode2, barcode3, barcodesymb FROM patienttest INNER JOIN testtable …` | statement CSV | VERIFIED |
| Barcode table | VA `6B28A4` `SELECT * FROM barcodes`; `z_barcodes` object | statement CSV + objects file | VERIFIED |
| Hierarchical report dataset | 27 `SHAPE` literals; aliases `cmdbill`/`cmdbill_d`; DataReport objects (`ReportFatora`, `ReportTestTime`, `ReportPCR`, …) | statement CSV + objects file | VERIFIED (SQL); render link INFERRED |
| Receipt ("wasl") data | VA `6B7ED0` grouped `patientinfo` + `patienttest` SHAPE; `wasl` widened by 12 ALTERs | statement CSV + DDL evidence | VERIFIED |
| Layout / fonts / printer from INI | `Data\*.ini` `[reportdata]`, `[FORM]`, `[report]`, `[setupx] reportprinterx`; `configuration-and-resources.md` §§3–6 | config report | VERIFIED |
| RTF narrative cache | 58 RTF files / 29 distinct documents; `\ansicpg 1256`, `\adeflang 3073` (`configuration-and-resources.md` §8) | config report | VERIFIED |
| **Printed page** (Bullzip PDF / physical printer) | `12-install-media.md` §10, `13-pdf-print-architecture.md` | dependency report | STRONGLY_SUPPORTED; not executed |

`database-and-data-access-analysis.md` §2.4 and `data-flow-and-control-flow.md`
§§2–6 are the prior analyses of this same chain: configuration-read → query →
result schema → layout → narrative → render → barcode → `wasl` identity.

---

## 6. Billing / payment

| Action | SQL / table | Evidence | Status |
| --- | --- | --- | --- |
| Patient bill header | `patientinfo.totalprice`, `.totalpricedisc`, `.elmadfoo3` (paid), `.elbakyma3mal` (remaining) | `columns__[analysis_BATCH-04_sql_v1605].csv` | VERIFIED |
| Receipts by patient | VA `6879A2` `Select * from payment where patientid = '` | statement CSV | VERIFIED |
| Collected total | VA `691DAA` `SELECT round(sum(money),1) as ta7seel FROM payment where patientid = '` | statement CSV | VERIFIED |
| Reconciliation report | VA `6913B6` joins `patientinfo` ↔ `payment` with `payment.type=0` and date filter | statement CSV | VERIFIED |
| Daily totals | VA `692DB6` `Select sum(money) as tot1 from payment where paiddate >= '` | statement CSV | VERIFIED |
| Referring-party commission | VA `694C92` `SUM(Gehats.omola * patientinfo.totalpricedisc / 100)` joined on `referredbyname = gehahname` | statement CSV | VERIFIED |
| Shared accounts | `sharekathesab` 19 statements; `moenysharekat` object (27 methods) | tables CSV + objects file | VERIFIED names; flow INFERRED |
| Installments / attendance | `tasgel` table; `Delete FROM tasgel WHERE tasgeldate >= '` (`6D70E4`) | statement CSV | VERIFIED |
| **Receipt write** | no `INSERT` literal | statement CSV | **UNVERIFIED** |

---

## 7. Post-processing, transport and backup

| Step | Evidence | Status |
| --- | --- | --- |
| SMS notification: `SMSSender` / `smslist` / `smsgroups` / `SMStemplate` / `MyLabSetting` | 10+10 statements, CREATE set, `SMSfrmsenderos` (139 methods); `database-and-data-access-analysis.md` §5 | STRONGLY_SUPPORTED |
| Send-out to other labs | `patienttest.otherlabname`, `.sent`, `.send`; `outtest` object (11 methods) | VERIFIED fields; workflow INFERRED |
| Access mirror / backup copies | `Patients.mdb`, `Patients_backup.mdb`, `repairedDB.mdb` (`database-and-data-access-analysis.md` §2.3) | VERIFIED |
| SQL backup devices | `EXEC sp_addumpdevice` (`66FCB2`), `EXEC sp_dropdevice` (`66FD56`) | VERIFIED |
| Patient data transport / attach scripts | `Patient Data Transport.exe`, `Attach_*.bat` (see `Database_Transactions.md` §3) | tool presence VERIFIED; internals UNVERIFIED |

---

## 8. Consolidated flow with confidence

```
[login: patients]                         VERIFIED
   ↓
[registration: patientinfo]               VERIFIED (read) / UNVERIFIED (write)
   ↓
[order: testtable→patienttest]            VERIFIED (read/SHAPE) / UNVERIFIED (write)
   ↓
[result: profile table + patientinfo]     VERIFIED (read) / UNVERIFIED (write)
   ↓
[validation: patienttest flags +          VERIFIED (read)
 normalrange + history]
   ↓
[print: SHAPE + reportview + barcodes +   VERIFIED (SQL/config) / INFERRED (render)
 Data\*.ini + RTF]
   ↓
[billing: patientinfo + payment +         VERIFIED (read) / UNVERIFIED (write)
 gehats + sharekathesab]
   ↓
[SMS / send-out / mirror / backup]        STRONGLY_SUPPORTED (tools/tables) /
                                          UNVERIFIED (internals)
```

**Denominator caveat.** The four UNVERIFIED write steps are a consequence of the
inventory containing 0 `INSERT` and 0 `UPDATE` literals out of 1,797 — not evidence
that writes do not occur.
