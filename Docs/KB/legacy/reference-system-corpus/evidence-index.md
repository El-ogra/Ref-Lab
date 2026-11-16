# Evidence Index — Batch B3 of RealLab CD 2019 v1.605

**Batch:** B3 · **Archive:** `v1.605-B3.zip` · 3,523,212 bytes
**SHA-256:** `2fb942c377d1e06b6ba473302e3ad8e8ba8a6faa39513bb9a296a73d64ee3a7b`
**Analysis date:** 2026-10-31 (UTC)

> Every substantive claim in this package maps to an evidence item below. Each
> item names the artefact that carries it and the command that regenerates it.
> **Negative results are indexed too** — an absence that was searched for is
> evidence.

---

## 1. Archive and extraction

| ID | Claim | Evidence artefact | Reproduction |
| --- | --- | --- | --- |
| EV-B3-001 | The archive is `v1.605-B3.zip`, 3,523,212 bytes, SHA-256 `2fb942c3…ee3a7b`, MD5 `1867efcb…71fcac` | `manifests/b3-extraction-report.json`; `batches/B3/reports/analysis-manifest.json` | `sha256sum /workspace/attachments/70c7c33f32ec35d4/v1.605-B3.zip` |
| EV-B3-002 | 251 entries; 0 absolute paths, 0 UNC paths, 0 `..` traversal, 0 symlinks, 0 encrypted entries | `manifests/b3-extraction-report.json` → `skipped: []` | `python3 manifests/extract_safe.py … --report …` |
| EV-B3-003 | Entries store **backslash** path separators, so a naive extraction flattens the tree | `manifests/b3-extraction-report.json` → `relative_path` fields | `unzip -Z -1 v1.605-B3.zip \| head` |
| EV-B3-004 | 251 files extracted, 0 skipped, **251 / 251 CRC32-verified**, 45,005,002 bytes written | `manifests/b3-extraction-report.json` → `crc_verified: true` for every file | as EV-B3-002 |
| EV-B3-005 | The original archive is preserved unmodified in the attachment directory | `batches/B3/reports/analysis-manifest.json` → `archive.preserved_unmodified: true` | `sha256sum` before and after |
| EV-B3-006 | Maximum single-entry compression ratio 40.76:1 — no archive-bomb condition | `manifests/b3-extraction-report.json`; `archive-inventory.md` §1 | `python3 -c "import zipfile; …"` |

---

## 2. Inventory and hashes

| ID | Claim | Evidence artefact | Reproduction |
| --- | --- | --- | --- |
| EV-B3-007 | 251 files, 45,005,002 bytes total | `batches/B3/inventory/inventory-summary.json` → `total_files`, `total_bytes` | `python3 manifests/build_inventory.py …` |
| EV-B3-008 | **143 distinct SHA-256 values**; 71 duplicate groups covering 179 files | `batches/B3/inventory/inventory-summary.json` → `duplicate_map` | as EV-B3-007 |
| EV-B3-009 | Per-file SHA-256 for all 251 files, at stable relative paths | `batches/B3/inventory/file-hashes.sha256` (251 lines) | `sha256sum -c` from the source root |
| EV-B3-010 | Per-file inventory with size, hash, type, magic, analysis state and method | `batches/B3/inventory/file-inventory.csv` (251 rows) | as EV-B3-007 |
| EV-B3-011 | Every file carries the analysis state `ANALYSED` — 251 / 251, 0 unanalysed | `batches/B3/inventory/file-analysis.json` → `analysis_state` | `python3 -c "import json; …"` |
| EV-B3-012 | `Cache/` and `RL_CD2019Access/Cache/` are **byte-identical directory mirrors** (29 / 29) | `batches/B3/inventory/inventory-summary.json` → `duplicate_map` | `sha256sum` comparison of both trees |
| EV-B3-013 | `Fonts/` and `RL_CD2019Access/Fonts/` are byte-identical mirrors (7 / 7) | as EV-B3-012 | as EV-B3-012 |
| EV-B3-014 | All 37 root `Data/` templates are byte-identical to their Access-tree counterparts; the Access tree adds 72 more | `batches/B3/inventory/inventory-summary.json`; `reports/archive-inventory.md` §6 | as EV-B3-012 |
| EV-B3-015 | `bin/` differs between trees: 5 files at the root, 14 in the Access tree | `batches/B3/inventory/file-inventory.csv` | as EV-B3-010 |

---

## 3. INI template analysis

| ID | Claim | Evidence artefact | Reproduction |
| --- | --- | --- | --- |
| EV-B3-016 | 146 INI files; sections `[reportdata]` 146, `[setupx]` 146, `[FORM]` 100, `[report]` 34; **1,394 distinct keys** | `batches/B3/evidence/ini-analysis.json` → `sections_observed`, `distinct_key_count` | `python3 manifests/analyse_ini.py …` |
| EV-B3-017 | **134 ASCII, 12 Windows-1256 (Arabic ANSI)**; 144 CRLF, 2 mixed line endings | `batches/B3/evidence/ini-analysis.json` → `files[].encoding`, `files[].line_endings` | as EV-B3-016 |
| EV-B3-018 | The 12 non-ASCII files are the four IgE allergy panels (164, 166, 167 × 2 × 2 trees) | `batches/B3/evidence/ini-analysis.json` → `files[].encoding` | as EV-B3-016 |
| EV-B3-019 | `LabelT01=Almond [F20] \xe1\xe6\xd2` decodes as `لوز` under **cp1256** and is gibberish under cp1252 and cp864 | `batches/B3/reports/security-review.md` §3 (FND-B3-003); raw bytes in `Data/164.ini` at offset ~127 | `python3 -c "print(bytes([0xe1,0xe6,0xd2]).decode('cp1256'))"` |
| EV-B3-020 | `printsen` distinguishes the base layout (1) from the `_S` variant (0) | `batches/B3/evidence/ini-analysis.json` → `files[].reportdata.printsen` | `grep -h '^printsen=' Data/116.ini Data/116_S.ini` |
| EV-B3-021 | The `_S` variant differs in geometry and item flags, not in content: `titleleft`, `left1`–`left7`, `width7`, `bottomvalue`, `leftvalue`, `reportstart`, `CheckT07`, `CheckT08` | `batches/B3/reports/software-architecture.md` §3.7 | `diff <(tr -d '\r' < Data/116.ini) <(tr -d '\r' < Data/116_S.ini)` |
| EV-B3-022 | **146 / 146** templates set `[setupx] reportprinterx` — 138 × `HP LaserJet 1100 (MS)`, 8 × `HP LaserJet 1100` | `batches/B3/evidence/ini-analysis.json` → `printer_defaults` | as EV-B3-016 |
| EV-B3-023 | Report titles across the set: 40 × `MICROBIOLOGY  REPORT`, 32 × `MOLECULAR BIOLOGY REPORT`, 20 × `Report`, 18 × `HEMATOLOGY REPORT`, 16 × `Immunoglobulin E - Allergy Screen`, + 7 others | `batches/B3/evidence/ini-analysis.json` → `report_maintitles` | as EV-B3-016 |
| EV-B3-024 | `testtype` values: 46 × `r`, 32 × `pcr`, 8 × `pnt`, 60 absent | `batches/B3/evidence/ini-analysis.json` → `testtype_values` | as EV-B3-016 |
| EV-B3-025 | Font slots referenced: **Arial 675, Tahoma 597, Times New Roman 328** | `batches/B3/evidence/ini-analysis.json` → `font_names_referenced` | as EV-B3-016 |
| EV-B3-026 | B3's templates contain **no `sql*` keys** — 0 matches across all 146 files | `batches/B3/reports/software-architecture.md` §3.1 | `grep -ril "sqlstatement\|sqlselect\|sqlfrom\|sqlexpression" --include=*.ini .` |
| EV-B3-027 | IDs 100–115 are the **16 microbiology cultures** (urine, stool, blood, semen, eye, ear discharge, pus from wound, nasal swab, throat swab, sputum, prostatic secretion, vaginal swab, conjunctival swab, gum discharge, synovial fluid, breast discharge) | `batches/B3/evidence/ini-analysis.json` → `files[].reportdata.formsubtitle` | `grep -h '^formsubtitle=' RL_CD2019Access/Data/10*.ini` |
| EV-B3-028 | IDs 450–465 are the **16 molecular-biology templates**; 450 = HBV-DNA qualitative, 451 = quantitative, 452 = HCV-RNA qualitative, 453 = quantitative, 454 = HCV genotyping | `batches/B3/evidence/ini-analysis.json` → `files[].reportdata` | `grep -h '^formsubtitle=' RL_CD2019Access/Data/45*.ini` |
| EV-B3-029 | The `[FORM]` block defines items, values, defaults, references, limits, titration dilutions and pH-category boundaries — 8,140 `ITEM##_#` keys alone | `batches/B3/reports/configuration-and-resources.md` §4 | as EV-B3-016 |
| EV-B3-030 | The `[report]` block holds up to 12 formatted interpretive lines; `400.ini` carries the HbA1c / ADA interpretation band | `RL_CD2019Access/Data/400.ini`; `batches/B3/reports/configuration-and-resources.md` §5 | `sed -n '/^\[report\]/,$p' RL_CD2019Access/Data/400.ini` |
| EV-B3-031 | 7 files contain a key duplicated inside one section, **all with identical values** | `batches/B3/reports/security-review.md` §3 (FND-B3-010) | `grep -c '^reportprinterx=' RL_CD2019Access/Data/400.ini` → `2` |
| EV-B3-032 | `Data/165.ini` duplicates `[FORM] ITEMS1_1` = `Not Detected`; `RL_CD2019Access/Data/450.ini` duplicates `[reportdata] normal` = `Evaluation Criteria :` | as EV-B3-031 | `grep -n '^ITEMS1_1=' Data/165.ini` |

---

## 4. RTF reference-document analysis

| ID | Claim | Evidence artefact | Reproduction |
| --- | --- | --- | --- |
| EV-B3-033 | 58 RTF files = **29 distinct documents**, byte-identically mirrored | `batches/B3/evidence/rtf-analysis.json`; `batches/B3/inventory/inventory-summary.json` | `python3 manifests/analyse_rtf.py …` |
| EV-B3-034 | Every document declares `\*\generator Microsoft Word 10.0.2627`, `\vern 16437`, `\ansicpg1256`, `\adeflang3073` | `batches/B3/evidence/rtf-analysis.json` → `files[].header`, `files[].generator` | `head -c 200 Cache/006.rtf` |
| EV-B3-035 | `\info` metadata: `\author` = `real lab`; `\operator` = `real lab` (17 docs) or `ais` (12 docs); `\company` = **`ais group`** | `batches/B3/evidence/rtf-info-metadata.json` | `python3 -c "…"` (brace-matched `\info` extraction) |
| EV-B3-036 | The 29 document titles — ALP, Albumin, ALT, AST, BUN, Bilirubin, A/G Ratio, Ammonia, Cholesterol, Coombs Direct/Indirect, **CBC with diff**, Bleeding Time, **ABGs**, CRP/hs-CRP, ASO, Blood Cultures, Ca ionised/total, Chloride, Aldosterone, CEA, AFP, CA 15.3 / 19.9 / 125, AT-III, C3/C4, Coagulation Factors | `batches/B3/evidence/rtf-analysis.json` → `files[].text_lines[0]` | `python3 -c "import json; …"` |
| EV-B3-037 | Documents carry reference ranges by population, SI conversions, critical levels, specimen type and clinical-significance notes | `batches/B3/evidence/rtf-analysis.json` → `files[].text` | `python3 -c "…json.load(…)['files'][0]['text']"` |
| EV-B3-038 | `Cache/056.rtf` = 16,953,057 B with **4** embedded vector pictures; `Cache/070.rtf` = 3,495,666 B with **2** | `batches/B3/evidence/rtf-analysis.json`; `batches/B3/evidence/rtf-pictures/` | `ls -l batches/B3/evidence/rtf-pictures/` |
| EV-B3-039 | The 6 embedded pictures are genuine **EMF** and **placeable WMF** (independently confirmed by `file`) | `batches/B3/evidence/rtf-pictures/*.emf`, `*.wmf` | `file batches/B3/evidence/rtf-pictures/*` |
| EV-B3-040 | The 6 pictures account for **97.3 %** of the batch's bytes | `batches/B3/inventory/inventory-summary.json` → `total_bytes` | `python3 -c "…"` |
| EV-B3-041 | The 29 documents are 41.5 MB in total — the `.rtf` extension accounts for 92.2 % of the batch | `batches/B3/reports/archive-inventory.md` §4 | `python3 manifests/build_inventory.py …` |

---

## 5. Image and font assets

| ID | Claim | Evidence artefact | Reproduction |
| --- | --- | --- | --- |
| EV-B3-042 | The 14 GIFs are two classes: **8 toolbar buttons** (20 × 15, 32 colours) and **6 HTML tag markers** (43 × 14, 8 colours) | `batches/B3/evidence/asset-analysis.json` → `assets[].gif` | `python3 manifests/analyse_assets.py …` |
| EV-B3-043 | The toolbar buttons are anchor, **bold**, **client script**, comments, page-begin/end, **style** and **unknown tag** | `batches/B3/evidence/preview/datareport_vh_toolbar.png` | *view the PNG* |
| EV-B3-044 | The tag markers render the literal strings `div`/`/div`, `form`/`/form`, `span`/`/span` | `batches/B3/evidence/preview/datareport_vh_tags.png` | *view the PNG* |
| EV-B3-045 | `PCRD/PCRH/PCRL/PCRM.jpg` are real-time PCR **amplification charts** labelled BELOW / HIGH / LOW / MODERATE | `batches/B3/evidence/preview/pcr_group.png` | *view the PNG* |
| EV-B3-046 | `PCRF.jpg` is the qPCR **detection-principle schematic** — Target Sequence, Primer 1/2, Taq DNA Polymerase, Biotin, 5′/3′ | `batches/B3/evidence/preview/pcr_group.png` | *view the PNG* |
| EV-B3-047 | The 9 stationery bands are bilingual letterhead/footer/logo images carrying placeholder identity text | `batches/B3/evidence/preview/stationery_group.png` | *view the PNG* |
| EV-B3-048 | `Logo.jpg`, `Logo1.jpg`, `Logo2.jpg` are byte-identical | `batches/B3/inventory/inventory-summary.json` → `duplicate_map` | `sha256sum RL_CD2019Access/bin/Logo*.jpg` |
| EV-B3-049 | The stationery asserts a **ministry-of-health** oversight line in Arabic next to a `Tel: 0000000` placeholder | `batches/B3/evidence/preview/stationery_group.png` | *view the PNG* |
| EV-B3-050 | 7 distinct fonts across 14 files; `name` tables identify "Tahomax by Matthew Carter", "Print Artist: Sierra On-Line, Inc.", "MCS Taybah S_U normal." | `batches/B3/evidence/asset-analysis.json` → `assets[].ttf.names` | `python3 manifests/analyse_assets.py …` |
| EV-B3-051 | `MCS Taybah.ttf` carries OS/2 `fsSelection` = NO_SUBSETTING, NO_PDF_EMBED → **installable embedding** | `batches/B3/evidence/asset-analysis.json` → `assets[].ttf.embedding` | as EV-B3-050 |
| EV-B3-052 | The barcode fonts are **Code 39** (`FREE3OF9.TTF`) and **UPC-A/EAN** (`WBCW_UPCEAN.ttf`, `WBCWS_UPCEAN.ttf`) | `batches/B3/evidence/asset-analysis.json` → font family names | as EV-B3-050 |
| EV-B3-053 | No JPEG in the batch carries an EXIF block | `batches/B3/evidence/asset-analysis.json` → `assets[].exif_present: false` | as EV-B3-049 |
| EV-B3-054 | **CORRECTED** — `ais lab.ttf` **does** carry a 6,944-byte DSIG table; the other 13 fonts carry none | `batches/B3/evidence/asset-analysis.json` → `assets[].ttf.has_dsig`; `batches/B3/evidence/font-dsig-analysis.txt` | `python3 -c "import json; …"` |

---

## 6. Cross-batch evidence

| ID | Claim | Evidence artefact | Reproduction |
| --- | --- | --- | --- |
| EV-B3-077 | `ais lab.ttf` carries a **6,944-byte DSIG table** with a PKCS#7 `SignedData`; the chain is **Microsoft Corporation** → **Microsoft Code Signing PCA** → **Microsoft Root Authority**, timestamped by Thawte, digest SHA-1 with Microsoft's SPC OIDs (1.3.6.1.4.1.311.2.1.4 / …1.28); DSIG flags `0x00010001` | `batches/B3/evidence/font-dsig-analysis.txt` | `python3 manifests/analyse_assets.py …` then `openssl asn1parse -inform DER` |
| EV-B3-078 | `ais lab.ttf` is **Microsoft's Tahoma**: `name[0]` "© 2004 Microsoft Corporation. All rights reserved.", `name[7]` "Tahoma is a trademark of Microsoft Corporation.", `name[8]` "Microsoft Corp.", `name[9]` "Matthew Carter", `name[11]` "http://www.microsoft.com/typography" — while `name[1]`/`name[4]` have been changed to "Tahomax" | `batches/B3/evidence/asset-analysis.json` → `assets[].ttf.names` | `python3 manifests/analyse_assets.py …` |
| EV-B3-079 | `ais symbol.ttf` and `Digiface.ttf` carry the Weatherly Systems notice "From the WSI-Font Collection. Copyright (c)1995 Weatherly Systems, Inc. All Rights Reserved. **Redistribution strictly prohibited.**" / "…(c)1993 W.S.I… **Redistribution strictly prohibited.**"; `WBCW_UPCEAN.ttf` / `WBCWS_UPCEAN.ttf` carry "GetMySystem.com, Inc. 2005. All Rights Reserved" with vendor URL `http://www.barcodewiz.com` | `batches/B3/evidence/asset-analysis.json` → `assets[].ttf.names` | `python3 manifests/analyse_assets.py …` |

| EV-B3-055 | **12** B3 files share a SHA-256 digest with a B1 file | `batches/B3/reports/archive-inventory.md` §6 | `python3 -c "…"` (cross-hash correlation) |
| EV-B3-056 | `Data/65.ini`, `66.ini`, `418.ini` and their `_S` variants (both trees) are **identical to B1's `RL_CD2019Access/Data/data_emp/R.ini` / `R_S.ini`** | `context/B1/file-inventory.csv`; `batches/B3/inventory/file-hashes.sha256` | `grep '<digest>' context/B1/file-inventory.csv` |
| EV-B3-057 | All three carry `HEMATOLOGY REPORT` / `Reticulocyte Count` — one prototype deployed under three test IDs | `batches/B3/evidence/ini-analysis.json` | `grep -h '^formsubtitle=' Data/65.ini Data/66.ini Data/418.ini` |
| EV-B3-058 | **0** B3 files share a digest with any of B2's 70 files | `context/B2/file-hashes.sha256`; the correlation run | as EV-B3-055 |
| EV-B3-059 | B3 populates **the same path** B2 documents for the report-viewer group: `RL_CD2019Access/System/datareport_vh/` | `context/B2/archive-inventory.md` (directory layout) | `grep -n 'datareport_vh' context/B2/archive-inventory.md` |
| EV-B3-060 | B2 records `jscript.dll` 5.1.0.8513, `vbscript.dll` 5.1.0.8513, `msscript.ocx` 1.0.0.3715 in that directory, and lists the missing consumer as an open limitation | `context/B2/executive-summary.md` §3; `context/B2/coverage-and-limitations.md` | `grep -n 'datareport_vh' context/B2/*.md` |
| EV-B3-061 | B1 documents the barcode configuration keys `barcodeport`, `barcodeprinter`, `barcodefile`, `barcodeauto` and the `patientinfo.barcode` column | `context/B1/software-architecture.md` | `grep -n 'barcode' context/B1/software-architecture.md` |
| EV-B3-062 | B1's FND-009 records that the installer "installs fonts and can request a reboot" | `context/B1/finding-register.md` | `grep -n -A4 'FND-009' context/B1/finding-register.md` |
| EV-B3-063 | B1 lists `wasl` among the Access mirror's reference tables | `context/B1/software-architecture.md` §3.1 | `grep -n 'wasl' context/B1/software-architecture.md` |
| EV-B3-064 | B1 and B2 both establish that the main executable reads `D:\real lab system\Data\REPORT.ini` for `color1..color19` | `context/B1/software-architecture.md`; `context/B2/configuration-and-resources.md` | `grep -rn 'REPORT.ini' context/` |
| EV-B3-065 | B1's templates carry the `sql*` assembly keys that B3's do not | `context/B1/software-architecture.md` §2.4 | `grep -n 'sqlstatement' context/B1/software-architecture.md` |

---

## 7. Negative results (verified absences)

| ID | Absent | Scope | How it was searched |
| --- | --- | --- | --- |
| EV-B3-066 | **Any credential, connection string, provider string, API key or token** | all 251 files | Search for `password`, `passwd`, `pwd=`, `user id=`, `uid=`, `secret`, `apikey`, `api_key`, `token=`, `Provider=`, `SQLOLEDB`, `Data Source=`, `Initial Catalog` — **1 hit, a false positive** ("secreted" in `Cache/079.rtf`) |
| EV-B3-067 | **Any OLE object, embedded package, DDE link, macro or field code** in the RTF cache | all 58 RTF | Search for `\object`, `\objdata`, `\objemb`, `\objautlink`, `\objclass`, `\macrobutton`, `\field`, `\mmath`, `\datastore`, `\dde`, `\atnid`, `\atnauthor` — **0 hits** |
| EV-B3-068 | **Any executable, script, macro-enabled document or archive** | all 251 files | Extension and magic-byte classification — 0 |
| EV-B3-069 | **Any database file** | all 251 files | Extension classification and `mdb-tables` sweep — 0 |
| EV-B3-070 | **Any integrity metadata** — signature, checksum, manifest reference | all 251 files | Hash/signature block scan; no file names another file's hash — 0 |
| EV-B3-071 | **Any network endpoint** — URL, host or port | all 251 files' text content | Grep for URL/host patterns — 0 |
| EV-B3-072 | **Any EXIF metadata** | 19 JPEGs | `exif_present: false` for all |
| EV-B3-073 | **Any `sql*` key** in B3's templates | 146 INI | `grep -ril "sqlstatement\|sqlselect\|sqlfrom\|sqlexpression"` — 0 |
| EV-B3-074 | **Any duplicate section name or unknown key family** | 146 INI | Section-shape families: 4 known combinations; every key falls into a catalogued family |
| EV-B3-075 | **Any file shared with B2** | 251 B3 vs 70 B2 digests | 0 overlaps |
| EV-B3-076 | **Any unsafe archive entry** | 251 entries | 0 absolute / 0 UNC / 0 traversal / 0 symlink / 0 encrypted / 0 CRC failures |

---

## 8. Mapping — findings to evidence

| Finding | Evidence |
| --- | --- |
| FND-B3-001 | EV-B3-009, EV-B3-016, EV-B3-033, EV-B3-034, EV-B3-037, EV-B3-038, EV-B3-070 |
| FND-B3-002 | EV-B3-042, EV-B3-043, EV-B3-044, EV-B3-059, EV-B3-060 |
| FND-B3-003 | EV-B3-017, EV-B3-018, EV-B3-019, EV-B3-034 |
| FND-B3-004 | EV-B3-022, EV-B3-021 |
| FND-B3-005 | EV-B3-047, EV-B3-048, EV-B3-049 |
| FND-B3-006 | EV-B3-052, EV-B3-061 |
| FND-B3-007 | EV-B3-038, EV-B3-039, EV-B3-040 |
| FND-B3-008 | EV-B3-050, EV-B3-051, EV-B3-054, EV-B3-072, EV-B3-077, EV-B3-078, EV-B3-079 |
| FND-B3-009 | EV-B3-064, EV-B3-016 |
| FND-B3-010 | EV-B3-031, EV-B3-032 |

---

## 9. Mapping — conclusions to evidence

| Conclusion | Evidence |
| --- | --- |
| B3 is the content/presentation layer of the LIS | EV-B3-007, EV-B3-016, EV-B3-033, EV-B3-042, EV-B3-045, EV-B3-047 |
| The installation's `RL_CD2019Access/` tree is a strict superset of the root tree | EV-B3-012, EV-B3-013, EV-B3-014, EV-B3-015 |
| B3's templates are a **different layer** from B1's, not a duplicate | EV-B3-026, EV-B3-065, EV-B3-029, EV-B3-030 |
| The report designer is HTML-based and script-capable | EV-B3-042, EV-B3-043, EV-B3-044, EV-B3-059, EV-B3-060 |
| The report is laid out on an absolute printer canvas | EV-B3-016, EV-B3-021, EV-B3-022, EV-B3-025 |
| Report layout and interpretation are data, editable without recompiling | EV-B3-016, EV-B3-030, EV-B3-037, EV-B3-070 |
| The product is Arabic-locale-targeted | EV-B3-017, EV-B3-019, EV-B3-034, EV-B3-047, EV-B3-050 |
| The vendor is "ais group" | EV-B3-035, EV-B3-050, and B1's `AssemblyCompany("ais group")` |
| B3 contributes no credential material to the installation | EV-B3-066 |

---

## 10. Evidence artefact index

| Path | What it holds |
| --- | --- |
| `manifests/b3-extraction-report.json` | per-entry extraction record, CRC results, path-normalisation record |
| `manifests/handover-extraction-report.json` | the same for the B1/B2 handover archive |
| `batches/B3/inventory/file-inventory.csv` | 251 rows — path, size, hash, type, magic, analysis state, method, role, duplicates |
| `batches/B3/inventory/file-hashes.sha256` | 251 digest lines at stable relative paths |
| `batches/B3/inventory/inventory-summary.json` | counts, extension/type breakdown, full duplicate map |
| `batches/B3/inventory/file-analysis.json` | 251 per-file analysis records with format-specific detail |
| `batches/B3/evidence/ini-analysis.json` | per-file INI structure, the 1,394-key vocabulary, printer/title/type/font tallies |
| `batches/B3/evidence/rtf-analysis.json` | per-file RTF structure, extracted text, headers, `\info` metadata |
| `batches/B3/evidence/rtf-info-metadata.json` | the `\info` block of all 29 distinct documents |
| `batches/B3/evidence/asset-analysis.json` | per-asset GIF / JPEG / TTF structure |
| `batches/B3/evidence/rtf-pictures/` | the 6 extracted embedded vector images (3 EMF, 3 WMF) |
| `batches/B3/evidence/emf/` | alternate extraction path for the same pictures |
| `batches/B3/evidence/preview/` | 4 rendered montages used for visual identification |
| `batches/B3/reports/analysis-manifest.json` | archive identity, environment, recovery record, artefact hashes |
| `batches/B3/reports/{components,dependencies,findings,coverage}.json` | the machine-readable records |
