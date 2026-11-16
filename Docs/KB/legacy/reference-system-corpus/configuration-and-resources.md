# Configuration and Resources — Batch B3 of RealLab CD 2019 v1.605

**Batch:** B3 · **Archive:** `v1.605-B3.zip` · **SHA-256:** `2fb942c377d1e06b6ba473302e3ad8e8ba8a6faa39513bb9a296a73d64ee3a7b`
**Analysis date:** 2026-10-31 (UTC) · **Method:** static only; nothing was executed

---

## 1. The configuration surface of B3

B3 contains **no XML, no JSON, no YAML, no `.config`, no registry hive, no
environment-variable script and no environment file.** Its entire configuration
surface is **146 INI files**, and its entire resource surface is **images and
fonts**.

| Surface | Mechanism | Count |
| --- | --- | ---: |
| Report layout & result schema | Windows INI | 146 |
| Clinical reference text | RTF (Word 2002) | 58 |
| Report-designer imagery | GIF | 14 |
| Report graphics & stationery | JPEG | 19 |
| Typography | TrueType | 14 |

---

## 2. The INI format as B3 uses it

### 2.1 Structure

Every one of the 146 templates has exactly the same shape:

```ini
; (optional leading blank line)
[reportdata]
; ... 40-120 keys ...
[setupx]
reportprinterx=HP LaserJet 1100 (MS)
```

with `[FORM]` appended in 100 files and `[report]` in 34. Eight distinct
section-shape families exist, all of them combinations of those four sections.

**Section inventory**

| Section | Files | Role |
| --- | ---: | --- |
| `[reportdata]` | 146 | layout geometry, colours, typography, report titles, print flags |
| `[setupx]` | 146 | the printer binding — exactly one key in every file |
| `[FORM]` | 100 | the result-item schema: labels, values, defaults, references, limits, flags |
| `[report]` | 34 | the interpretive text block, with per-line formatting |

### 2.2 Encoding and line endings — a real inconsistency

| Encoding | Files | Which |
| --- | ---: | --- |
| ASCII | 134 | all except the four allergen panels |
| **Windows-1256 (Arabic ANSI)** | **12** | `164`, `164_S`, `166`, `166_S`, `167`, `167_S` in both trees |

| Line endings | Files |
| --- | ---: |
| CRLF | 144 |
| mixed | 2 — `RL_CD2019Access/Data/103.ini` and `103_S.ini` |

The 12 Arabic files are the four **Immunoglobulin-E allergy panels** (Food,
Inhalant, Mixed, Pediatric). Their Arabic allergen names are stored as
Windows-1256:

```
LabelT01=Almond [F20] \xe1\xe6\xd2      →  LabelT01=Almond [F20] لوز
LabelT03=Peanut [F13] …                →  LabelT03=Peanut [F13] …
Nitrl05_2=Ammonium Urates
Nitrl05_3=Amorphous Phosphates
```

Decoding the same bytes under three candidate codepages makes the choice
unambiguous:

| Codepage | `LabelT01=Almond [F20] …` |
| --- | --- |
| **cp1256** | `LabelT01=Almond [F20] لوز` ✅ |
| cp1252 | `LabelT01=Almond [F20] áæÒ` ✗ |
| cp864 | `LabelT01=Almond [F20] ﻓﻧﺯ` ✗ |

See **FND-B3-003**.

### 2.3 A formatting anomaly

Most keys are written `key=value`. A minority use a space before the equals
sign — for example `reportstart =2000` and `reportstart =1800`. This is
inconsistent with the rest of the file and suggests the templates were
assembled by concatenating hand-edited fragments. It is harmless for a
standard INI reader that trims the key, but it is a marker of the generation
process. Related: **7 files contain a key repeated inside a single section**
(§5).

---

## 3. The `[reportdata]` key vocabulary

### 3.1 Report identity

| Key | Meaning |
| --- | --- |
| `reportmaintitle` | the discipline banner on the report |
| `formsubtitle` | the test name |
| `reportsubtitle1` … `reportsubtitle3` | sub-sections |
| `reportsubtitleF1` … `reportsubtitleF3` | field captions (e.g. `Culture:`, `Sensitivity:`, `Microscopic Examination:`) |
| `normal` | the reference-range caption (e.g. `Reference Range:`) |
| `comment` | the comment caption |
| `main_symb`, `sub_symb` | marker characters (usually `E`, `K`, `B`) |
| `signature`, `signature2`, `signature3` | signature captions (e.g. `Doctor's signature:`, `Dr. `, `Thanks`) |

### 3.2 Print control flags

| Key | Values seen | Meaning |
| --- | --- | --- |
| `printcritical` | `1` | print critical values |
| `printhistory` | `1` | print previous results |
| `printsen` | `1` / `0` | print the sensitivity (antibiotic susceptibility) panel |
| `printcommer` | `1` | print the commercial/remarks block |
| `statusflag` | `1` | — |
| `backcolor` | `1` | — |
| `reportstart` | `1800`–`2000` | initial page offset |

`printsen` is the flag that distinguishes the `_S` layout variants
(§4 of `software-architecture.md`): 138 templates set it to `1` in the base
layout, and the `_S` variant of the microbiology templates sets it to `0`.

### 3.3 Geometry

| Key | Example | Meaning |
| --- | --- | --- |
| `titleleft` | `226` | title-block origin |
| `left1` … `left7` | `567`, `963`, `3005`, `3175`, `5386`, `5159`, `7200` | the seven column origins |
| `width1` … `width7` | `2097`, `2608` | column widths |
| `topvalue`, `bottomvalue` | `0`, `600` | vertical band |
| `leftvalue`, `rightvalue` | `600`, `0` | horizontal band |
| `spacevalue` | `453` | body spacing |
| `spacetitle`, `spacenormal` | `567`, `0` | title and body spacing |
| `reportstart` | `2000` | initial offset |

### 3.4 Colour and typography

| Key | Meaning |
| --- | --- |
| `colora` … `colort` | 20 base colours as BGR integers |
| `colorXfn` | font name for slot X (a–e) |
| `colorXfs` | font size in points, quarter-point precision (`18`, `15.75`, `9.75`) |
| `colorXfb` | bold flag |
| `colorXul` | underline flag |

Font slot usage across the whole set: **Arial 675 · Tahoma 597 · Times New
Roman 328**. All three are Windows system fonts; the 7 fonts B3 actually ships
are a separate set (§6).

### 3.5 Section shape families

| Shape | Files |
| --- | ---: |
| `[FORM]` + `[reportdata]` + `[setupx]` | 68 |
| `[reportdata]` + `[setupx]` | 44 |
| `[FORM]` + `[report]` + `[reportdata]` + `[setupx]` | 32 |
| `[report]` + `[reportdata]` + `[setupx]` | 2 |

---

## 4. The `[FORM]` key vocabulary

| Family | Instances | Meaning |
| --- | ---: | --- |
| `LabelT01`…`LabelT15` | 1,200 | the item label |
| `LabelX1`…`LabelX4` | 240 | the contextual label (sample, organism, culture condition, colony count) |
| `ITEM01_1`…`ITEM15_N` | 8,140 | selectable value for an item |
| `ITEMS1_1`…`ITEMS9_N` | 6,396 | the printable rendering of a value |
| `ITEMXS1_1`… | 2,000 | the displayed form of a selectable value |
| `ITEM01_D1`…`D3` | 1,170 | the default value(s) |
| `Item_D1`…`D3` | 120 | default for the flat option list |
| `ITEM01_Ref` | 480 | the reference interval |
| `RefLimit1`…`3` | 96 | explicit reference limits |
| `ITEM01_Value`, `ITEM01_Value1/2` | 720 + 160 | numeric value slots |
| `ITEM01_DH` / `_DL` | 120 | display-high / display-low |
| `ITEM01_AcidicL/H`, `_NeutralL/H`, `_AlkalineL/H` | 240 | pH-category boundaries |
| `Acidic05_1`…`20`, `Alkalin05_1`…`20`, `Nitrl05_1`…`20` | 600 | vocabulary lists |
| `titr_1`…`titr_15` | 480 | titration dilutions |
| `Item_1`…`Item_15` | 600 | a flat 15-slot option list |
| `ITEMV#`, `ITEMD#`, `ITEMR#`, `ITEMT#` | 720 each | per-item value / default / reference / type |
| `Check1x`…`Check7x`, `Check1f`… | 396 + 240 | form-visibility switches |
| `CheckT01`…`CheckT08` | 584 | print-visibility switches |
| `ITEM01_XD2`, `ITEM01_DH`, `ITEMXD2` | 40 + 40 | display-variant slots |
| `itemsNo` | 16 | the item count |
| `x1`, `x2`, `x3` | 44 | auxiliary slots |

**Total distinct keys across the whole set: 1,394.**

---

## 5. The `[report]` key vocabulary

| Key | Meaning |
| --- | --- |
| `ext` | extension flag (all `1`) |
| `title`, `titlebold`, `titleitalic`, `titlesize`, `titlecolor` | the note block's heading |
| `wordxcolor` | the body text colour |
| `printxo` | print-flag |
| `LINE_1`…`LINE_12` | the note body (21 files) |
| `LINEBOLD_1`…`LINEBOLD_12` | per-line bold |
| `LINESIZE_1`…`LINESIZE_12` | per-line size (all `9`) |
| `title1`, `titlebold1`, …, `printxo1` | a second heading block |
| `LINE1_1`…`LINE1_12` | a second note body |
| `LINEBOLD1_1`…`LINEBOLD1_12` | per-line bold |
| `LINESIZE1_1`…`LINESIZE1_12` | per-line size (8 or 9) |

A second, independently numbered block (`1`-suffixed) allows two note groups
in one report. Only `400.ini` / `400_S.ini` (HbA1c) use it, carrying the
American Diabetes Association interpretation band.

---

## 6. The `[setupx]` key vocabulary

One key only: `reportprinterx`.

| Value | Files |
| --- | ---: |
| `HP LaserJet 1100 (MS)` | 138 |
| `HP LaserJet 1100` | 8 |

The 8 without the `(MS)` suffix are the `_S` variants of the microbiology
cultures (100–115), whose base templates use the suffixed form.

---

## 7. Duplicate keys

Seven files repeat a key within a single section. **In every observed instance
the duplicated key carries an identical value**, so the effect today is benign.

| Files | Section | Duplicated key | Value |
| --- | --- | --- | --- |
| `Data/165.ini`, `Data/165_S.ini` + 2 Access copies | `[FORM]` | `ITEMS1_1` | `Not Detected` |
| `RL_CD2019Access/Data/400.ini`, `400_S.ini` | `[setupx]` | `reportprinterx` | `HP LaserJet 1100 (MS)` |
| `RL_CD2019Access/Data/450.ini` | `[reportdata]` | `normal` | `Evaluation Criteria :` |

See **FND-B3-010**.

---

## 8. Resources — the RTF cache

58 files, 29 distinct documents, byte-identically mirrored.

### 8.1 Header

| Control word | Value | Meaning |
| --- | --- | --- |
| `\rtf1` | — | RTF version 1 |
| `\adeflang` | `3073` | Arabic (Saudi Arabia) |
| `\ansicpg` | `1256` | Windows-1256 Arabic ANSI |
| `\uc1` | — | 1 fallback character per unicode escape |
| `\deflang` | `1033` | US English |
| `\deflangfe` | `2052` (14 files) / `1033` (15 files) | Far-East language |
| `\adeff0` | — | default font |

### 8.2 `\info` metadata — identical across all 29 documents

| Field | Value |
| --- | --- |
| `\author` | `real lab` |
| `\operator` | `real lab` (17 documents) / `ais` (12 documents) |
| `\company` | **`ais group`** |
| `\title` | *(empty)* |
| `\vern` | 16437 (Word 2002) |
| `\nofpages` | 1, except `056.rtf` (3) and `070.rtf` (3) and `500.rtf` (2) |

`\operator` switching between `real lab` and `ais` across the set suggests the
documents were authored by two different people, or in two different
organisations' templates.

### 8.3 Font table

Every document declares a font table containing **Times New Roman**, **Courier
New**, **Symbol**, **Wingdings** and **SimSun** (with the East-Asian fallback
`@SimSun`), plus the full script-specific Times New Roman variants
(Central European, Cyrillic, Greek, Turkish, Hebrew, Arabic, Baltic,
Vietnamese).

### 8.4 Embedded pictures

| Document | Pictures | Format | Bytes |
| --- | ---: | --- | ---: |
| `056.rtf` | 4 | 2 × EMF, 2 × placeable WMF | 1,901,528 / 1,121,456 / 3,337,858 / 1,969,330 |
| `070.rtf` | 2 | 1 × EMF, 1 × placeable WMF | 619,616 / 1,087,998 |

All six were extracted and confirmed with `file` as genuine
`Windows Enhanced Metafile` and `Windows metafile` data. The `056` images are
blood-cell morphology illustrations; the `070` image is a blood-gas chart.

---

## 9. Resources — images

### 9.1 GIF (14) — `RL_CD2019Access/System/datareport_vh/`

| Class | Dimensions | Palette | Files |
| --- | --- | ---: | --- |
| Toolbar button | 20 × 15 | 32 | `anchorwi`, `br`, `clientsc`, `comments`, `pbgn`, `pend`, `style`, `unknownt` |
| HTML tag marker | 43 × 14 | 8 | `divbgn`/`divend`, `formbgn`/`formend`, `spanbgn`/`spanend` |

All 14 are single-frame, non-animated, non-transparent GIF87a/GIF89a.

### 9.2 JPEG (19)

| Group | Files | Dimensions |
| --- | --- | --- |
| PCR graphics | `PCRD`, `PCRF`, `PCRH`, `PCRL`, `PCRM` | 800 × 320 (×4), 400 × 266 (×1) |
| Stationery | `Header`, `Tailer`, `Text`, `Wasl`, `ZHeader`, `ZTailer` | 680 × 140, 680 × 50, 460 × 60, 720 × 50, 720 × 140, 720 × 50 |
| Logo placeholders | `Logo`, `Logo1`, `Logo2` | 145 × 95 |

All are baseline sequential JFIF. **No EXIF block is present in any of the
19**, so no capture device, author or copyright metadata is carried.

---

## 10. Resources — fonts (14 files, 7 distinct)

| File | Family | Version | Vendor | OS/2 `fsSelection` |
| --- | --- | --- | --- | --- |
| `ais lab.ttf` | Tahomax | 3.14, 2004-06-17 | Matthew Carter | — |
| `ais symbol.ttf` | Borderbats-Fleur2 | — | Print Artist: Sierra On-Line, Inc. | — |
| `Digiface.ttf` | Digiface | — | Print Artist: Sierra On-Line, Inc. | — |
| `FREE3OF9.TTF` | New | 1.00 | — | — |
| `MCS Taybah.ttf` | MCS Taybah S_U normal. | 1.0, 1997-01-07 | — | NO_SUBSETTING, NO_PDF_EMBED |
| `WBCW_UPCEAN.ttf` | WBCW_UPCEAN | 1.00, 2009-12-22 | — | — |
| `WBCWS_UPCEAN.ttf` | WBCW_UPCEAN | 1.00, 2009-12-22 | — | — |

**One carries a digital signature.** `ais lab.ttf` holds a 6,944-byte **DSIG**
table containing a Microsoft PKCS#7 `SignedData` (signer: Microsoft
Corporation; PCA: Microsoft Code Signing PCA; root: Microsoft Root
Authority; timestamper: Thawte). The remaining 13 carry no signature.
`MCS Taybah.ttf` is the only one with an explicit `fsSelection` embedding
policy — **installable embedding** (no restriction bit set, but subsetting and
PDF embedding disallowed).

Two fonts — `ais symbol.ttf` and `Digiface.ttf` — carry the Weatherly Systems
**"Redistribution strictly prohibited"** notice in `name` ID 0. See
**FND-B3-008**.

---

## 11. Configuration hygiene summary

| Check | Result |
| --- | --- |
| Credentials, connection strings, provider strings | **0** (only "secreted" in clinical prose) |
| RTF OLE objects, embedded packages, DDE links, macros, field codes | **0** |
| Duplicate keys within a section | 7 files (all same-value) |
| Mixed line endings within a file | 2 files |
| Non-ASCII encodings mixed into an ASCII set | 12 files (cp1256) |
| Keys with a space before `=` | present (`reportstart =2000`) |
| Files with any integrity metadata | **0** of 251 |
| Section names outside the four known | **0** |
| Unrecognised key families | **0** — every key falls into a catalogued family |
