# Reconstructed Code Structure — the VB6 project behind the 1.605 executable

**Evidence:** `vb6-structure.json`, `vb6-objects.json`, `objects-full__[analysis_BATCH-04_vb6_v1605].txt`, `forms-blob__[analysis_BATCH-04_vb6_v1605].txt`, `object-diff-vs-v1.json`, `vb6-procedures-summary.txt`, `primary-executable-reverse-engineering.md`, `primary-executable-technical-reference.md`, `Version_Identity_and_Scope.md`. All under `D:\Collective work\`. The 1.605 binary itself is not on disk; this is a structural reconstruction from its dumps.

## 1. Project identity (VERIFIED)

| Property | Value | Evidence field |
|---|---|---|
| Project name | `Real_lab_system` | `primary-executable-reverse-engineering.md §2.2`; `vb6-chain-analysis.txt` (baseline) |
| Project description / exe name | `real lab system` | same |
| VB header signature | `VB5!` | `vb6-structure.json → vb_header.signature` |
| Runtime build | `9782` = VB6 **Service Pack 6**; runtime revision `10` | `vb6-structure.json → vb_header.runtime_build/runtime_revision` |
| Locale | `lcid 1033` (US English), secondary `4105` (ar-SA) | `vb6-structure.json → vb_header.lcid`; `primary-executable-reverse-engineering.md §2.2` |
| Project GUID | `6c1467afa02291459ed352beb9d44d32` | `vb6-structure.json → object_table.project_uuid` |
| Object table | `object_count 212`, `compiled_objects 210`, `objects_in_use 3052`, `first_object 102` | `vb6-structure.json → object_table` |
| Declared methods | **8,026** | `vb6-objects.json → summary.total_methods 8026`; `objects-full__[…v1605].txt` header `methods 8026` |
| Header `form_count` | **198** | `vb6-structure.json → vb_header.form_count` |
| External components | **25** (names not decoded — UNR-009) | `vb6-structure.json → vb_header.external_component_count`; `primary-executable-reverse-engineering.md §5` |
| Thunks | **233** | `primary-executable-reverse-engineering.md §2.2` |

## 2. Object classification — the authoritative count (VERIFIED)

Source: `objects-full__[analysis_BATCH-04_vb6_v1605].txt`, header:
```
name Real_lab_system
objects 210
declared_objects 210
methods 8026
method_entries_read 1268
unique_entries 0
methods_without_address 1268
```
Counting the 210 numbered object lines by that file's own classification field gives:

| Kind | Count | Sum check |
|---|---:|---|
| `form` | 132 | |
| `datareport` | 66 | |
| `module` | 9 | |
| `class` | 3 | |
| **Total** | **210** | 132 + 66 + 9 + 3 = 210 |

**Counts are instrument-dependent — do not conflate them:**
- `vb6-structure.json` raw object table = **212** slots (210 named/compiled + 2 trailing unnamed), and its header `form_count` = **198**.
- `forms-blob__[analysis_BATCH-04_vb6_v1605].txt` header says "**# 131 forms from form-data blobs**" and lists 131 `name,controls,width,height` rows (e.g. `labmain,183,14715,10215`; `Resultsemen,266,14775,10230`; `frm_money,101,14775,10230`). That is a count of forms whose *data blobs* were recovered, not the declaration count.
- `vb6-objects.json → summary.by_classification` = `{module_or_class:203, form:3, module:4, unnamed:2}`. This comes from the naive prefix classifier in `vb6_objects.py` (`name.startswith('frm_') → form`, `startswith('cls') → class`, `startswith('m') → module`, else `module_or_class`) and **must not be used as the project type breakdown**. The `objects-full` classifier is the one that matches the project structure (132/66/9/3) and is used here. VERIFIED for the classifier source; the 132/66/9/3 split is therefore STRONGLY_SUPPORTED.

## 3. Key modules and classes actually present (VERIFIED names and counts)

From `objects-full__[…v1605].txt` and `vb6-objects.json → objects[].method_count`:

| Object | Kind | Methods | Role suggested by name/evidence |
|---|---|---:|---|
| `labdb` | module | 24 | main database access layer (`object-diff-vs-v1.json`: 20→24) |
| `labdb1` | module | 20 | secondary database layer (19→20) |
| `labfunction` | module | 25 | shared VB functions (13→25) |
| `printerselect` | module | 11 | printer selection (16→11) |
| `BCWFonts` | module | 7 | report font handling (`method_names_va 0x0` in `vb6-objects.json`) |
| `mInputbox` | module | 12 | custom input box |
| `saveIOtemp` | module | 10 | temporary I/O save |
| `boisais` | module | 16 | auxiliary utility (new in 1.605) |
| `otherfun` | module | 2 | miscellaneous functions (new in 1.605) |
| `moenysharekat` | module | 27 | company/revenue calculations (new in 1.605) |
| `moenymozafeen` | module | 27 | employee/revenue calculations |
| `aaaClass` | class | 15 | new class in 1.605 |
| `CRect` | class | 20 | rectangle helper class |
| `BiosData` | class | 8 | new class in 1.605 (tools) |

True VB6 standard modules have `method_names_va = 0x0` in `vb6-objects.json` (`BCWFonts`, `labfunction`, `printerselect`, `saveIOtemp`, `boisais`, `mInputbox`, `otherfun`), consistent with modules whose procedures are not stored in a per-object name table — one reason per-method names were not recovered (`vb6-procedures-summary.txt`: 0 procedures).

## 4. The `Result*` / `Report*` architecture (STRONGLY_SUPPORTED)

`primary-executable-reverse-engineering.md §4` identifies the key structural insight: for each laboratory discipline there is a **capture** object (`Result<discipline>`) and a **render** object (`Report<discipline>`). Examples from the object table:

| Discipline | Capture object (methods) | Render object (methods) |
|---|---|---|
| CBC | `resultcbc` (172) | — (report via `Reporttotal`/`anyreport`) |
| Hormones | `resulthormon` (116) | — |
| Semen | `Resultsemen` (222), `Resultsemen5` (216) | `Reportsemen`, `Reportsemen1`, `Reportsemen2`, `Reportsemen1s`, `Reportsemen2s` (all 0) |
| Liver | `Resultliver` (101) | `Reportliver`, `Reportlivers`, `Reportliver7his`, `Reportlivers7his` (0) |
| Urine | `Resulturine` (167), `ResulturineAuto` (184) | `Reporturine`, `Reporturines` (0) |
| PCR | `ResultPCR` (37) | `ReportPCR`, `ReportPCRs` (0) |
| IgE allergy | `resultIgeAllergy` (231) | `ReportIGE1`, `ReportIGE1s` (0) |
| LBC | `resultLBC` (33) | `ReportLBC`, `ReportLBCs` (0) |
| Zinc | `resultzn` (36) | `ReportZN`, `ReportZNs` (0) |
| Money/invoices | `frm_money` (38), `moneypaid` (18) | `Reportmony1`…`Reportmony5`, `Reportmony01`, `Reportmony11`, `ReportFatoraC` (mostly 0) |

The **plural/`s`-suffixed report objects carry `methods = 0`** and use `type_flags 0x1018083` — data-driven VB6 DataReport definitions with no code of their own. `primary-executable-reverse-engineering.md §4` states the 146 `Data/*.ini` templates are the layout these `Report*` objects consume; `primary-executable-technical-reference.md §6` corroborates the report-template vocabulary. The exact binding between a given `Report*` object and a given template is INFERRED (no per-object call chain was recovered).

## 5. Feature mapping (INFERRED where only names support it)

| Feature area | Objects / modules evidenced | Confidence |
|---|---|---|
| Patient registration & orders | `addpatient` (192), `patient` (58), `patientmain` (84), `patientview` (33), `Patient_samples` (89), `patienthistory` (22), `patienthistory2` (17) | STRONGLY_SUPPORTED by object names + `patientinfo`/`patienttest` SQL (`statements__[analysis_BATCH-04_sql_v1605].csv`) |
| Test results capture | ~55 `Result*` forms, e.g. `resultcbc` 172, `resulthepatitis` 187, `Resultsemen` 222, `resultIgeAllergy` 231 | STRONGLY_SUPPORTED |
| Report rendering / printing | 66 DataReport objects; `printerselect`; `barcode` (76), `barcodeform`; `workpaper` (17), `workpaper2` (18); `reportsetup` (113), `reportsetup2` (69) | STRONGLY_SUPPORTED |
| Billing / pricing | `frm_money` (38), `moneypaid` (18), `moenysharekat` (27), `moenymozafeen` (27), `z_pricelists` (27), `pricelists`; SQL on `totalprice`, `totalpricedisc`, `discount`, `pricemethod` | STRONGLY_SUPPORTED |
| Identity / access | `login` (13), `users` (57), `userspw` (12), `logindete` (6), `logindetect` (DataReport) | STRONGLY_SUPPORTED |
| Configuration / setup | `setup` (16), `setup1` (56), `setup2` (58), `setup3` (42), `normal_test` (36), `testrange` (17), `calctest` (2), `learntest` (6), `abbrevi` (41), `converter` (14), `commentsetup` (5) | STRONGLY_SUPPORTED |
| SMS subsystem (new in 1.605) | `SMSfrmsenderos` (139), `ReportSMS`, `smslist`/`smssender` SQL, plus the `smsmisr.com` URL literals in `primary-executable-reverse-engineering.md §6.3` | STRONGLY_SUPPORTED |
| External lab send-out | `labtolab` (32), `outtest` (11), SQL fields `inotherlab`, `otherlabname`, `otherlabprice` | STRONGLY_SUPPORTED |
| Administration / audit | `server` (17), `instrument` (29), `z_logs` (14), `z_gehats` (56), `z_branchs` (14), `z_units` (14), `z_group` (14), `z_customgroup` (31), `z_titles` (15), `z_labaccount` (18) | STRONGLY_SUPPORTED |
| Quality control | `qcfrm` (27), `Reportqc` | STRONGLY_SUPPORTED |
| Microbiology / culture | `resultculutre` (115), `cultureanti`/`culturesec` SQL, `antibioticfrm` (29) | INFERRED (object/SQL names only) |

## 6. Version delta from 1.00 (VERIFIED from `object-diff-vs-v1.json`)

- `v1_object_count: 169` → `v1605_object_count: 210`; **41 added**, **0 removed**; ~90 objects changed in method count.
- Added objects include `BiosData`, `CardFrm`, `CardFrmSetx`, `Frmac`, `Patient_samples`, `RPTPatientVisit`, `ReportFatoraC`, `ReportIGE1`, `ReportLBC`, `ReportPCR`, `ReportSMS`, `ReportZN`, `ResultFMF`, `ResultPCR`, `Resultsemen5`, `ResulturineAuto`, `SMSfrmsenderos`, `aaaClass`, `boisais`, `labmainTools`, `mInputbox`, `otherfun`, `pricelistsN`, `resultIgeAllergy`, `resultLBC`, `resulthpylori`, `saveIOtemp`, `static6`, `server`.
- Largest method-count changes: `addpatient 125→192`, `patientmain 54→84`, `users 29→57`, `labfunction 13→25`, `labdb 20→24`, `reportsetup 108→113`, `Resultsemen 255→222`, `ResultFertlity 136→125`, `ResultTrace 203→183`, `resultsemen` family split into `Resultsemen5` (216).
- `ais 8→0` and several `Report*` additions with `methods 0` confirm movement of logic into new objects and data-driven reports rather than deletion.

## 7. Limitations (UNKNOWN / UNVERIFIED)

- Per-object **method names** were not recovered (`vb6-procedures-summary.txt`: 0 procedures; `primary-executable-reverse-engineering.md §10`). Only counts and object names exist.
- Only **1,268 of 8,026** method entries were read/resolvable according to the `objects-full` header (`method_entries_read 1268`, `methods_without_address 1268`); the two fields are internally ambiguous, so the exact proportion of resolvable method addresses is UNVERIFIED, but the instrument could not resolve most method entries.
- The 25 external component names behind `external_component_count` were not decoded (UNR-009).
- Object-to-feature mapping rests on names plus SQL/string evidence; no executed behaviour and no per-object call graph exists.
