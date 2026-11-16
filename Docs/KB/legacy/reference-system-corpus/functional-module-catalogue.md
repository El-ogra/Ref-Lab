# Functional Module Catalogue — RealLab CD 2019 v1.605

**Consolidated:** 2026-10-31 (UTC) · **Static analysis only.**
Machine-readable: `machine-readable/functional-module-catalogue.csv` (212 rows).

---

## 1. The register

Every object in the VB6 project table, recovered by walking
`ObjectTable → lpObjectArray` and each `PublicObjectDescriptor` to its name
pointer. The names are **facts**; the family grouping is an **interpretation**
of them, stated as such.

| | |
| --- | ---: |
| Objects in the project | **212** |
| Named | 210 |
| Total methods | **8,026** |
| Declared forms (`wFormCount`) | 198 |
| Declared external components | 25 (not decoded — UNR-009) |

---

## 2. Families

| Code | Family | Objects | Methods |
| --- | --- | ---: | ---: |
| D | Core application | 12 | 327 |
| C | Form - user interface | 9 | 243 |
| A | Result - clinical result capture | 57 | 5520 |
| H | Finance, referral and stock | 5 | 98 |
| J | Utility and other | 26 | 687 |
| B | Report - report definition / rendering | 63 | 185 |
| G | Barcodes, work papers and inter-laboratory | 5 | 143 |
| F | Configuration and administration | 19 | 411 |
| E | Reference / master data | 10 | 217 |
| I | Communications and clinical notes | 4 | 195 |
| Z | unnamed | 2 | 0 |

---

## 3. The `Result` / `Report` pairing — the structural spine

For each discipline the project carries a **capture** object and a **render**
object:

| Discipline | `Result*` | Methods | `Report*` | Methods |
| --- | --- | ---: | --- | ---: |
| Liver | `Resultliver` | 101 | `Reportliver` | 0 |
| Urine | `Resulturine` | 167 | `Reporturine` | 0 |
| Urine auto | `ResulturineAuto` | 184 | `Reporturines` | 0 |
| Sugar | `Resultsugar` | 61 | `ReportSugar` / `ReportSugars` | 0 |
| Stool | `Resultstool` | 139 | — | — |
| Kidney | `Resultkidney` | 117 | — | — |
| Culture | `resultculutre` | 115 | `reportculutre` / `reportculutres` | 0 |
| Lipids | `Resultlipids` | 81 | — | — |
| Semen | `Resultsemen` | 222 | `Reportsemen` / `Reportsemen1` / `Reportsemen2` / `Reportsemen1s` / `Reportsemen2s` | 0 |
| ABO | `resultABO` | 47 | — | — |
| Widal | `resultwidal` / `resultwidal2` | 41 / 51 | `repotrwidal` / `repotrwidals` | 0 |
| Hepatitis | `resulthepatitis` | 187 | — | — |
| ESR | `ResultESR` | 32 | — | — |
| CBC | `resultcbc` | 172 | — | — |
| TORCH | `ResultTORCH` | 123 | — | — |
| CRP | `resultcrp` | 97 | — | — |
| PT | `resultpt` | 110 | — | — |
| Hormone | `resulthormon` | 116 | — | — |
| **PCR** | `ResultPCR` | 37 | `ReportPCR` / `ReportPCRs` | 0 |
| **LBC** | `resultLBC` | 33 | `ReportLBC` / `ReportLBCs` | 0 |
| **FMF** | `ResultFMF` | 48 | `ReportFatora` / `ReportFatoraS` / `ReportFatoraC` / `ReportFatorareset` | 0 |
| **IgE allergy** | `resultIgeAllergy` | 231 | `ReportIGE1` / `ReportIGE1s` | 0 |
| Coagulation factors | `resultcoagfactor` | 79 | `Reportcross` / `Reportcrosss` | 0 |
| Cross-match | `resultcrossm` | 140 | — | — |
| Blood gas | `resultbloodgas` | 81 | — | — |
| Iron | `resultiron` | 62 | — | — |
| Insulin | `ResultInsulin` | 92 | — | — |
| Bilharzia | `Resultbilharzias` | 64 | — | — |
| Pregnancy | `Resultpregnancy` | 44 | — | — |
| Fertility | `ResultFertlity` | 125 | — | — |
| Suprarenal | `ResultSuprarenal` | 117 | — | — |
| Cardiac | `resultcardiac` | 66 | — | — |
| Tumours | `resultTumors` | 100 | — | — |
| Vitamins | `resultVitamins` | 110 | — | — |
| Trace | `resultTrace` | 183 | — | — |
| Abuse | `resultabuse` | 95 | — | — |
| Drugs | `resultdrugs` | 98 | — | — |
| Immunology | `resultimmun` | 138 | — | — |
| Immunoglobulins | `resultImmunoglob` | 72 | — | — |
| Hb electrophoresis | `resulthgbelec` | 51 | — | — |
| Protein electrophoresis | `resultprotelec` | 61 | — | — |
| Zinc | `resultzn` | 36 | — | — |
| Tuberculin | `resultTUBERCULIN` | 32 | — | — |
| Marrow | `resultmarrow` | 64 | — | — |
| Post-coital | `resultpostcoital` | 60 | — | — |
| Renal | `resultrenal` | 80 | — | — |
| Stone | `resultstone` | 72 | — | — |
| Prostatate | `resultprostata` | 113 | — | — |
| CSF | `resultcsf` | 83 | — | — |
| Retics | `resultretios` | 97 | — | — |
| H. pylori | `resultha1c` / `resulthpylori` | 39 / 83 | — | — |
| Sugar curve | `resultsugacurve` | 78 | — | — |

**Every `Report*` object has zero methods.** In a compiled VB6 binary that is
the signature of a **data-driven report definition**: the object binds a
template, and the layout lives in `Data/*.ini`. The plural forms
(`Reporturines`, `Reportsugars`, `ReportPCRs`, `ReportIGE1s`, `ReportLBCs`,
`ReportZNs`, `Reportanyreports`, `Reporthistory2`) are the **summary /
aggregate** reports.

**This is the mechanism behind B3's 146 templates**, and it is the clearest
explanation of the product's organisation.

---

## 4. The non-`Result`/`Report` objects

| Object | Methods | Role |
| --- | ---: | --- |
| `labmain` | 71 | the application's entry module |
| `patientmain` | 84 | the patient master screen |
| `addpatient` | **192** | the largest single object — patient registration |
| `labdb` / `labdb1` | 24 / 20 | the database layer |
| `patientview` / `patient` / `patienthistory` / `patienthistory2` | 33 / 58 / 22 / 17 | patient browsing and history |
| `barcode` | **76** | the barcode engine |
| `barcodeform`, `z_barcodes` | 0 / 14 | the barcode form and the barcode reference table |
| `reportsetup` / `reportsetup2` | 113 / 69 | the Report Settings window and its second variant |
| `normal_test` / `testrange` / `calctest` | 36 / 17 / 2 | the reference-range editor |
| `users` / `userspw` / `login` / `logindete` | 57 / 12 / 13 / 6 | user administration and authentication |
| `server` | 17 | the server-settings window |
| `instrument` | 29 | the analyser registry |
| `workpaper` / `workpaper2` | 17 / 18 | the work-paper |
| `labtolab` | 32 | inter-laboratory referral |
| `moenysharekat` / `moenymozafeen` | 27 / 27 | referral accounting |
| `moneypaid` / `frm_money` / `qcfrm` | 18 / 38 / 27 | billing |
| `sarfz` / `sarf` / `shoppingfrm` | 26 / 1 / 15 | stock and supplies |
| `SMSfrmsenderos` | **139** | the SMS sender |
| `antibioticfrm` | 29 | the antibiotic-susceptibility form |
| `Frmcomment` / `commentsetup` / `abbrevi` / `noteic` | 17 / 5 / 41 / 10 | comments, abbreviations, clinical notes |
| `z_logs` / `z_gehats` / `z_branchs` / `z_units` / `z_pricelists` / `z_titles` / `z_customgroup` / `z_group` / `z_labaccount` | 14–56 | the `z_*` reference-data family |
| `pricelists` / `pricelistsN` | 0 / 0 | price lists |
| `setup` / `setup1` / `setup2` / `setup3` | 16 / 56 / 58 / 42 | the system-settings screens |
| `testmain` / `learntest` / `converter` | 30 / 6 / 14 | the test catalogue and utilities |
| `patientmain` → `patientview` | | the patient workflow |
| `Frm_pic` / `Frm_pic2` / `CardFrm` / `CardFrmSetx` / `CRect` | 55 / 31 / 0 / 22 / 20 | picture and card screens |
| `labmainTools` / `labfunction` / `otherfun` / `tools` / `mInputbox` / `BCWFonts` / `ais` / `labxxyyzz` | 1–25 | shared utilities |
| `aaaClass` / `BiosData` / `boisais` / `saveIOtemp` | 15 / 8 / 16 / 10 | helper classes |

---

## 5. How the catalogue connects to the other evidence

| Catalogue evidence | Corroborates | Batch |
| --- | --- | --- |
| `Result*` covering ~55 disciplines | the per-test INI templates and the discipline result tables | B3, B1 |
| `Report*` with zero methods | the data-driven report mechanism | B3 |
| `reportsetup` (113) / `normal_test` (36) | the manual's Report Settings and reference-range dialogs | B4 |
| `barcode` (76) / `barcodeform` / `z_barcodes` | the manual's three-code barcode scheme | B4 |
| `labtolab` / `moenysharekat` / `moenymozafeen` | the manual's send-out-samples workflow | B4 |
| `users` (57) / `userspw` / `login` | B1's `useraccounts` finding | B1 |
| `instrument` (29) | B1's `instrument` table | B1 |
| `SMSfrmsenderos` (139) | the reconstructed SMS gateway path | this stage |
| `labdb` / `labdb1` | the SQL Server and Access mirror | B1, this stage |

---

## 6. Limitations

1. **Method names within each object were not recovered** — only the counts.
2. **The 25 external components were not decoded** (UNR-009).
3. **The family grouping is an interpretation** of the recovered names.
4. **No call graph underlies these assignments.** They are structural facts
   plus reasoning about naming.
