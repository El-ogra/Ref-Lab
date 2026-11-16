# Tables and Columns — RealLab v1.605

## Master core (Access)

| Table | Cols | Role | Key columns |
|---|---:|---|---|
| `patientinfo` | 33 | patient demographics | patientid, patientnamear, labid, requestdate, patientsex, patientage, patientageunit, referredbyname, barcode |
| `patienttest` | 44 | test orders / results index | patientid, testcode, testsimp, requestdate, resultfinsh, resultprint |
| `testtable` | 37 | test catalogue | testcode, arrangnum, subgroupcode, testsimp, testabrev, testname, testnamear, loggroup, active, inotherlab, otherlabprice, patientprice, labprice, deleteprotect, testtime, barcode1-3, branch, seereport, normaltype, addorno, testorno, REFRANGE |
| `normalrange` | 18 | reference ranges | normalid, testcode, testsimp, subgroupcode, sex, ageunit, REFRANGE |
| `grouptable` | 4 | test groups | groupcode, groupname, arname, deleteprotect |
| `subgrouptable` | 4 | test subgroups | subgroupcode, subgroupname, arname, deleteprotect |
| `constanttable` | 7 | derivation constants | constantdiscrp (PTCONS, CBCCONS), constant1..6 |
| `pricelist` | 4 | pricing lists | pricenum, testsimp, pricelistname, testprice |
| `testunits` | 2 | units of measure | unitsid, unitsname |
| `patients` | 26 | **operator accounts** (naming trap) | patientusid, patientusname, patientword, grant1..grant12, startwork, endwork, breakyorn, breaktime |
| `payment` | 6+ | payments | — |
| `history` | — | history/audit | — |
| `comments` | — | result comments | — |
| `phone` | — | phone book | — |
| `sections` | — | sections | — |
| `instrument` | — | instrument config | — |
| `barcodes` | 2 | barcode config | — |

## Per-test profile result tables (96)

One table per test profile; each carries `patientid`, `requestdate`, analyte columns, `comment`. Examples: `ABO`, `BILH`, `BloodGases`, `CRP`, `CSFT`, `CoagFactors`, `cbc`, `liver`, `kidney`, `lipids`, `stool`, `urine`, `hepatitis`, `hormones`, `Tumors`, `TORCH`, `ZN`, `semen`, etc.

## Data types / SQL references

Top referenced columns (from `columns.csv`): `patientinfo.patientid` (273 refs), `patientinfo.patientnamear` (233), `patientinfo.labid` (198), `patienttest.patientid` (71), `patientinfo.requestdate` (58).

Full per-table column lists and sample row counts: `type2_tables.csv` in `03_Independent_Analysis/Evidence_Type_2/`.