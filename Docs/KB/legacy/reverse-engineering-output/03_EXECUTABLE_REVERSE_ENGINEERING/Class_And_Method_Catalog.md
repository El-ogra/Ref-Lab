# Class and Method Catalog — VB6 object register (1.605)

**Sources.** `objects-full__[analysis_BATCH-04_vb6_v1605].txt` (210-row register with a 4-way kind classification and method counts), `vb6-objects.json` (212-slot raw object table with `index`, `name`, `classification`, `type_flags`, `method_count`, `method_names_va`, `public_bytes_va`), `object-diff-vs-v1.json` (v1→v1.605 method-count deltas), `function_inventory.csv` (native functions of the 1.00 baseline), `vb6-procedures-summary.txt`, `primary-executable-reverse-engineering.md`. All under `D:\Collective work\`.

## 1. Catalog integrity and confidence (READ FIRST)

| Field | Value | Source |
|---|---:|---|
| Named objects | 210 | `objects-full__[…v1605].txt` header `objects 210` |
| Raw table slots | 212 (2 unnamed trailing) | `vb6-structure.json → object_table.object_count 212`; `vb6-objects.json → summary.object_count 212, named 210, unnamed 2` |
| Declared methods | 8,026 | `objects-full` header `methods 8026`; `vb6-objects.json → summary.total_methods 8026` |
| Method entries read | **1,268** | `objects-full` header `method_entries_read 1268` |
| Methods without address | **1,268** | `objects-full` header `methods_without_address 1268` |
| Unique entries | 0 | `objects-full` header `unique_entries 0` |
| Per-object method **names** recovered | **0** | `vb6-procedures-summary.txt` header "169 objects, 0 procedures"; `primary-executable-reverse-engineering.md §10` |

**What "1,268 of 8,026" means.** The extraction instrument could resolve/read only 1,268 of the 8,026 declared method entries (and its own header reports 1,268 methods without address; the two fields are internally ambiguous — exact semantics UNVERIFIED). Practical consequence: **method-level reconstruction has low coverage and low confidence**. Object names and method *counts* are reliable (VERIFIABLE from two independent dumps); method identities, signatures, call order and bodies are NOT recoverable from this evidence. The native Ghidra inventory does not fill the gap: it is for the 1.00 baseline, has 6,522 mostly `FUN_`-named stubs, and contains no VB procedure names (`Native_Code_Analysis.md §1`).

The tables below are a **grouped extract** of the 210-object register. All names and counts are taken verbatim from the two cited files; the complete 210-row list is in `objects-full__[analysis_BATCH-04_vb6_v1605].txt`.

## 2. Modules (9 objects — VERIFIED names/counts)

| Object | Methods | v1→v1.605 delta | Evidence |
|---|---:|---|---|
| `labdb` | 24 | 20→24 | objects-full [3]; `object-diff-vs-v1.json → changed` |
| `labdb1` | 20 | 19→20 | objects-full [20] |
| `labfunction` | 25 | 13→25 | `vb6-objects.json [78]`, diff |
| `printerselect` | 11 | 16→11 | `vb6-objects.json [140]`, diff |
| `BCWFonts` | 7 | — | `vb6-objects.json [65]` |
| `mInputbox` | 12 | new (0→12) | `vb6-objects.json [189]`, diff `added` |
| `saveIOtemp` | 10 | new | `vb6-objects.json [174]`, diff `added` |
| `boisais` | 16 | new | `vb6-objects.json [184]`, diff `added` |
| `otherfun` | 2 | new | `vb6-objects.json [208]`, diff `added` |

`moenysharekat` (27) and `moenymozafeen` (27) are marked `module` in `vb6-objects.json` but counted as forms by the `objects-full` classifier — a CONFLICTED classification; both names and counts are evidenced, the kind label is not.

## 3. Classes (3 objects — VERIFIED)

| Object | Methods | Notes |
|---|---:|---|
| `aaaClass` | 15 | new in 1.605 (`object-diff-vs-v1.json → added`) |
| `CRect` | 20 | `type_flags 0x118003` in `vb6-objects.json [165]` |
| `BiosData` | 8 | new in 1.605; `type_flags 0x118003` (`vb6-objects.json [183]`) |

## 4. DataReports (66 objects — VERIFIED names, methods mostly 0)

All carry `type_flags 0x1018083` in `vb6-objects.json` and `methods=0` in `objects-full` except where noted. Representative groups (full set in `objects-full`):

| Group | Objects (methods in parentheses) |
|---|---|
| Invoices / money | `ReportFatora` (0), `ReportFatoraS` (0), `ReportFatoraC` (0), `ReportFatorareset` (0), `ReportFatorareset8` (0), `Reportmony1` (0), `Reportmony2` (0), `Reportmony3` (0), `Reportmony4` (0), `Reportmony5` (0), `Reportmony01` (0), `Reportmony11` (1) |
| Patient / visit | `ReportPatPhone` (0), `RPTPatientVisit` (0), `Reporthistory` (1), `Reporthistory2` (1) |
| Discipline reports | `Reporturine` (0), `Reporturines` (0), `Reportliver` (0), `Reportlivers` (0), `Reportliver7his` (0), `Reportlivers7his` (0), `ReportSugar` (0), `ReportSugars` (0), `Reportsemen` (0), `Reportsemen1` (0), `Reportsemen2` (0), `Reportsemen1s` (0), `Reportsemen2s` (0), `ReportPCR` (0), `ReportPCRs` (0), `ReportIGE1` (0), `ReportIGE1s` (0), `ReportLBC` (0), `ReportLBCs` (0), `ReportZN` (0), `ReportZNs` (0), `Reportcross` (0), `Reportcrosss` (0), `Reportqc` (0), `Reportinstrument` (0), `Reportlog` (0), `Reportshareka` (0), `ReportSMS` (0), `ReportTestTime` (0), `Reporttotals` (0), `Reporttotal` (0) |
| Work paper / misc | `Reportworkp` (0), `Reportwp` (0), `Reportwpm` (0), `Reportwpt` (0), `Reportwpot` (0), `rpttotaliss` (0), `rpttotalisss` (0), `reportstatic1` (0), `statisReport` (0), `reportculutre` (0), `reportculutres` (0), `repotrwidal` (0), `repotrwidals` (0), `Reportanyreport` (0), `Reportanyreports` (0), `pricelists` (0), `pricelistsN` (0), `zarf` (1), `logindetect` (0) |

**Interpretation (STRONGLY_SUPPORTED).** Zero-method DataReports are data-driven report definitions, not code-containing forms; the `Report*` / `Result*` pairing is described in `Reconstructed_Code_Structure.md §4`.

## 5. Forms (132 objects — VERIFIED names/counts; grouped extract)

### 5.1 Core application and workflow

| Object | Methods | Object | Methods |
|---|---:|---|---:|
| `labmain` | 71 | `patientmain` | 84 |
| `addpatient` | 192 | `patient` | 58 |
| `patientview` | 33 | `Patient_samples` | 89 |
| `patienthistory` | 22 | `patienthistory2` | 17 |
| `login` | 13 | `users` | 57 |
| `userspw` | 12 | `logindete` | 6 |
| `server` | 17 | `instrument` | 29 |
| `reportsetup` | 113 | `reportsetup2` | 69 |
| `barcode` | 76 | `barcodeform` | 0 |
| `workpaper` | 17 | `workpaper2` | 18 |
| `start` | 10 | `setup` | 16 |
| `setup1` | 56 | `setup2` | 58 |
| `setup3` | 42 | `qcfrm` | 27 |
| `labtolab` | 32 | `outtest` | 11 |
| `SMSfrmsenderos` | 139 | `normal_test` | 36 |
| `testrange` | 17 | `calctest` | 2 |
| `learntest` | 6 | `abbrevi` | 41 |
| `converter` | 14 | `commentsetup` | 5 |
| `noteic` | 10 | `Frmcomment` | 17 |
| `frm_money` | 38 | `moneypaid` | 18 |
| `shoppingfrm` | 15 | `anyreport` | 16 |
| `qcfrm` | 27 | `labmainTools` | 6 |

### 5.2 Result-capture forms (discipline methods; sample of the ~55 `Result*` objects)

| Object | Methods | Object | Methods | Object | Methods |
|---|---:|---|---:|---|---:|
| `Resultsemen` | 222 | `Resultsemen5` | 216 | `resultIgeAllergy` | 231 |
| `resulthepatitis` | 187 | `ResulturineAuto` | 184 | `ResultTrace` | 183 |
| `resultcbc` | 172 | `Resulturine` | 167 | `resultselectro` | 144 |
| `resultcrossm` | 140 | `resultimmun` | 138 | `ResultFertlity` | 125 |
| `ResultTORCH` | 123 | `ResultSuprarenal` | 117 | `Resultkidney` | 117 |
| `resulthormon` | 116 | `resultculutre` | 115 | `resultprostata` | 113 |
| `resultVitamins` | 110 | `resultpt` | 110 | `Resultliver` | 101 |
| `resultabuse` | 95 | `ResultInsulin` | 92 | `resultretios` | 97 |
| `resultdrugs` | 98 | `resultcrp` | 97 | `resulthpylori` | 83 |
| `resultcsf` | 83 | `resultbloodgas` | 81 | `resultcoagfactor` | 79 |
| `resultsugacurve` | 78 | `resultImmunoglob` | 72 | `resultstone` | 72 |
| `resultcardiac` | 66 | `resultbilharzias` | 64 | `resultmarrow` | 64 |
| `resultiron` | 62 | `resultpostcoital` | 60 | `resulthgbelec` | 51 |
| `resultha1c` | 39 | `ResultPCR` | 37 | `resultzn` | 36 |
| `resultTUBERCULIN` | 32 | `ResultESR` | 32 | `resultLBC` | 33 |
| `resultABO` | 47 | `resultwidal` | 41 | `Resultsugar` | 61 |
| `resultwidal2` | 51 | `ResultFMF` | 48 | `resultrenal` | 80 |
| `resultprotelec` | 61 | `Resultpregnancy` | 44 | `resultanytest` | 33 |

### 5.3 Administration, lookup and utility forms

| Object | Methods | Object | Methods | Object | Methods |
|---|---:|---|---:|---|---:|
| `z_gehats` | 56 | `z_customgroup` | 31 | `z_pricelists` | 27 |
| `z_titles` | 15 | `z_units` | 14 | `z_barcodes` | 14 |
| `z_branchs` | 14 | `z_logs` | 14 | `z_group` | 14 |
| `z_labaccount` | 18 | `sarfz` | 26 | `moenysharekat` | 27 |
| `moenymozafeen` | 27 | `static1` | 30 | `static2` | 26 |
| `static3` | 15 | `static4` | 18 | `static5` | 21 |
| `static6` | 24 | `tqrermog` | 41 | `phone` | 33 |
| `testmain` | 30 | `tools` | 1 | `outtest` | 11 |
| `Frm_pic` | 55 | `Frm_pic2` | 31 | `Frmac` | 26 |
| `CardFrm` | 0 | `CardFrmSetx` | 22 | `frmTip` | 5 |
| `barcodeform` | 0 | `ais` | 0 | `labxxyyzz` | 3 |
| `antibioticfrm` | 29 | `learntest` | 6 | `logindete` | 6 |

All object names above are reproduced verbatim from `objects-full__[…v1605].txt` and `vb6-objects.json`; no object name has been invented.

## 6. Native-function catalog (baseline 1.00 — VERIFIED counts, no names)

For contrast, `function_inventory.csv` catalogues 6,522 native functions of the **1.00** binary: 6,516 `auto-generated`, 6 `thunk`; 6,515 are `FUN_*` placeholders; the only 7 named functions are `__vbaChkstk`, `adj_fdiv_m32`, `adj_fdiv_m64`, `adj_fdivr_m64`, `DllFunctionCall`, `ThunRTMain`, `entry`. There is **no 1.605 function catalog** in the evidence set. See `Native_Code_Analysis.md §1`.

## 7. Confidence legend for this catalog

- VERIFIED: object names, method counts, v1→v1.605 deltas (two independent dumps agree).
- STRONGLY_SUPPORTED: the 132/66/9/3 kind split from `objects-full` and the interpretation of 0-method DataReports.
- CONFLICTED: `moenysharekat` / `moenymozafeen` kind (module vs form across the two classifiers).
- UNVERIFIED / LOW CONFIDENCE: any method-level detail — only 1,268 of 8,026 method entries were read, 0 method names were recovered, and no call graph exists.
