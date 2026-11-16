# Software Architecture — Batch B3 of RealLab CD 2019 v1.605

**Batch:** B3 · **Archive:** `v1.605-B3.zip` · **SHA-256:** `2fb942c377d1e06b6ba473302e3ad8e8ba8a6faa39513bb9a296a73d64ee3a7b`
**Analysis date:** 2026-10-31 (UTC) · **Method:** static only; nothing was executed

> **Batch semantics.** B1–B5 are portions of one program directory for one
> release. This document describes what B3 contributes to that single system and
> how it integrates with the parts B1 and B2 already documented. It is not a
> comparison between batches.

---

## 1. Where B3 sits in the system

B1 analysed the **application** — the VB6 main executable, the Access/Jet
mirror and the SQL Server integration. B2 analysed the **platform and
deployment payload** — the ActiveX controls, their type libraries, the VB6
runtime, the script engines and the setup helpers.

B3 is the **content and presentation layer**: the data that the application
reads to decide *what a laboratory report looks like and what it says*.

```
                    ┌──────────────────────────────────────────────┐
                    │              RealLab CD 2019 v1.605          │
                    │        one installation, five portions      │
                    └──────────────────────────────────────────────┘

   B1  ┌─────────────────────┐        B2  ┌──────────────────────┐
        │  main executable    │             │  VB6 runtime, OCX,   │
        │  (VB6)              │             │  typelibs, setup     │
        │  Access/Jet mirror  │             │  helpers, script     │
        │  SQL Server client  │             │  engines             │
        └──────────┬──────────┘             └──────────┬───────────┘
                   │                                    │
                   │  reads                              │  populates
                   ▼                                    ▼
   B3  ┌──────────────────────────────────────────────────────────────┐
        │  Data/*.ini        per-test report layout + result schema   │
        │  Cache/*.rtf       clinical reference library              │
        │  Fonts/*.ttf       report rendering fonts                   │
        │  bin/*.jpg         report graphics and stationery           │
        │  System/datareport_vh/*.gif   HTML report-designer assets   │
        └──────────────────────────────────────────────────────────────┘
```

**The architectural contribution of B3 is that it closes the loop between the
two earlier portions.** B1 established that reports are *configuration-driven*;
B3 supplies the configuration. B2 established that the report viewer is a
*scriptable HTML subsystem*; B3 supplies its user interface.

---

## 2. The two deployment trees are not symmetric

B1's executive summary states the product "ships as **two parallel deployment
trees** — `bin/` + `Data/` and `RL_CD2019Access/` (with its own `bin/`,
`Data/`, `System/`)". B3 measures the relationship precisely, and it is **not**
a mirror:

| Folder | Root tree | `RL_CD2019Access/` tree | Relationship |
| --- | ---: | ---: | --- |
| `bin/` | 5 | 14 | Access tree is a **superset** — it adds 9 stationery files |
| `Cache/` | 29 | 29 | **byte-identical mirror** |
| `Data/` | 37 | 109 | Access tree is a **superset** — 37 shared, 72 additional |
| `Fonts/` | 7 | 7 | **byte-identical mirror** |
| `System/datareport_vh/` | — | 14 | **Access tree only** |

Every file present in both trees is **byte-identical** — not merely similar.
The `RL_CD2019Access/` tree is the complete deployment tree; the root-level
folders are a reduced view.

**[INFERRED, Medium confidence]** The root-level folders are most likely the
working set of the **VB6 executable** (B1's portion), while
`RL_CD2019Access/` is the working set of the **Access/Jet mirror application**
(B1 also documented). The 72 additional Access-tree templates are exactly the
disciplines whose result tables live in the Access mirror: the 16 microbiology
culture panels, the 16 molecular-biology panels, and four named one-offs.

---

## 3. The report-layout subsystem

### 3.1 Two distinct template families — do not conflate them

This is the single most important structural point in B3, because **B1
documented a different template family** and the two are easy to merge
mistakenly.

| | **B1's templates** | **B3's templates** |
| --- | --- | --- |
| Location | `Data/` and `RL_CD2019Access/Data/` | same folders |
| Names | `CBC.ini`, `URINE.ini`, `URINEauto.ini`, `Helicobacter.ini`, `SEMEN5.ini`, `FMF.ini`, `ABO.ini`, `BLOODGAS.ini`, … | `60.ini`, `65.ini`, `100.ini`, `116.ini`, `450.ini`, … |
| Count | 134 | **146** |
| Keyed by | **discipline / report name** | **numeric test ID** |
| Contains `sql*` keys | **yes** (`sqlstatement`, `sqlselect`, `sqlfrom`, `sqlwhere`, `sqlorder`, `sqlgroup`, `sqlsum`, `sqlexpression`, `SELECT_1..10`, `SELECT_D1..D3`) | **no — zero occurrences in all 146 files** |
| Contains `FORM` / `report` blocks | no | **yes** — 100 and 34 files respectively |
| Role | assembles the **SQL** that fetches the result | defines the **result-item schema, reference limits, interpretive text and print geometry** |

**B3's templates contain no SQL at all.** They are a different layer of the
same system: the *shape and vocabulary* of a result, not the query that fetches
it. B1's §2.4 "Report generation — configuration-driven" describes the SQL
assembly; B3 describes what the report looks like once the data is in hand.

The two families are related but not redundant: B1's
`RL_CD2019Access/Data/data_emp/R.ini` and `R_S.ini` — which B1 recorded as
generic prototypes in a `data_emp/` repository folder — reappear in B3 as the
**instantiated** templates `65.ini`, `66.ini` and `418.ini`, all
SHA-256-identical to the prototype. **The prototype becomes the deployment.**

### 3.2 The `[reportdata]` section — print geometry as data

Every one of the 146 templates carries a `[reportdata]` section with the
report's **absolute layout in printer-device units (twips)**:

```ini
[reportdata]
formsubtitle=Cervical Swab Culture
reportmaintitle=MICROBIOLOGY  REPORT
reportsubtitle1=Culture and Sensitivity:
reportsubtitleF1=Culture:
reportsubtitleF2=Sensitivity:
reportsubtitleF3=Microscopic Examination:
main_symb=E
sub_symb=K
normal=Reference Range:
comment=Comment:
statusflag=1
backcolor=1
printcritical=1
printhistory=1
printsen=1
printcommer=1
titleleft=226
left1=567   left2=963   left3=3005
left4=3175  left5=5386  left6=5159  left7=7200
width1=2097
topvalue=0   bottomvalue=600
leftvalue=600 rightvalue=0
spacevalue=453  spacetitle=567  spacenormal=0
reportstart =2000
```

Seven column origins (`left1`…`left7`) and per-column widths describe a
**fixed seven-column report grid**. `topvalue`/`bottomvalue`/`leftvalue`/
`rightvalue` and `spacevalue`/`spacetitle`/`spacenormal` set the vertical
rhythm and the gaps around the title block. `reportstart` is an initial page
offset (1800–2000 across the set).

**Rendering is a printer-canvas, not a paginated HTML flow.** The geometry is
absolute and is calibrated against a specific device (§3.5).

### 3.3 Colour and font as per-column data

The same section carries a complete colour and typeface matrix:

```ini
colora=0  colorb=0  …  colort=0            ; 20 base colours (BGR integers)
colorafn=Times New Roman  colorafs=18    colorafb=1  coloraul=1
colorbfn=Times New Roman  colorbfs=15.75 colorbfb=1  colorbul=1
colordfn=Tahoma            colordfs=11.25 colordfb=1  colordul=0
colorefn=Arial             colorefs=9.75  colorefb=0  coloreul=0
…
```

Each column (`a`…`t`) has: a base colour, and for the five **styled** slots
(`a`–`e`) a font name, a font size in points, a bold flag and an underline
flag. Font usage across the whole set: **Arial 675 · Tahoma 597 · Times New
Roman 328**.

`colorafs=18` means 18 pt; `colorbfs=15.75` means 15.75 pt. These are
quarter-point-precision sizes — a typographic detail system.

The `colorXfn` slots also align with the fonts B3 actually ships (§5).

### 3.4 The `[FORM]` section — the result-item schema

100 of the 146 templates add a `[FORM]` block that defines the **result items**
for that test. It is the most content-dense part of the batch.

| Key family | Instances | Meaning |
| --- | ---: | --- |
| `LabelT01…` / `LabelX1…` | 1,200 + 240 | the human-readable item label ("Pus Cells", "R.B.Cs", "Crystal", "Fungi", "Bacteria") |
| `ITEM01_1…ITEM15_N` | 8,140 | the selectable result values for item 01 |
| `ITEMS1_1…ITEMS9_N` | 6,396 | the **secondary** value set — the printable rendering of each value |
| `ITEMXS1_1…` | 2,000 | the displayed form of each selectable value |
| `ITEM01_D1…D3` | 1,170 | the **default** value(s) for an item |
| `ITEM01_Ref` | 480 | the item's reference interval |
| `RefLimit1…3` | 96 | explicit reference limits |
| `Item_1…Item_15` | 600 | a flat 15-slot option list (used by `613.ini`) |
| `Item_D1…D3` | 120 | the default for that flat list |
| `titr_1…titr_15` | 480 | titration dilutions (for titre-based assays) |
| `ITEM01_DH` / `_DL` | 120 | display-high / display-low |
| `ITEM01_AcidicL/H`, `_NeutralL/H`, `_AlkalineL/H` | 240 | **pH-category boundaries** per item |
| `Acidic05_1…20`, `Alkalin05_1…20`, `Nitrl05_1…20` | 600 | vocabulary lists per group |
| `Check1x…Check7x`, `Check1f…` | 396 + 240 | which optional items are displayed on the form |
| `CheckT01…T08` | 584 | which optional item groups are printed |
| `ITEMV#`, `ITEMD#`, `ITEMR#`, `ITEMT#` | 720 each | per-item value / default / reference / type slots |

Two worked examples show how concrete this is.

**Urine sediment (test 116 — "Cervical Swab Culture", a microbiology template):**

```ini
LabelT01=Reaction
LabelT02=Pus Cells
LabelT03=R.B.Cs
LabelT04=Epithelial Cells
LabelT05=Crystal
LabelT06=Fungi
LabelT07=Bacteria
LabelT08=Others
Nitrl05_1=Absent
Nitrl05_2=Ammonium Urates
Nitrl05_3=Amorphous Phosphates
Nitrl05_4=Amorphous Urates
Nitrl05_5=Calcium oxalate
Nitrl05_6=Cholestrol
Nitrl05_7=Cysteine
```

**Urine culture (test 100 — "Urine Culture"):**

```ini
LabelX1=Sample
LabelX2=Organism A
LabelX02=Organism B
LabelX002=Organism C
LabelX3=Culture Condition
LabelX4=Colony Count
LabelT09=Gram stain film
LabelT10=ZN film
```

The `LabelX` family is **per-organism** (A, B, C — up to three isolates),
with a Gram-stain and Ziehl-Neelsen film slot each. This is a **structured
microbiology result**, expressed as data.

### 3.5 The `[setupx]` section — the printer binding

All 146 templates set exactly one key:

```ini
[setupx]
reportprinterx=HP LaserJet 1100 (MS)
```

138 templates name the printer with the `(MS)` suffix; 8 name it without.
The binding to a **specific legacy device** is what the absolute twip geometry
in §3.2 is calibrated against. See **FND-B3-004**.

### 3.6 The `[report]` section — interpretive text

34 templates — all of them in the `RL_CD2019Access/Data/` tree — add a fourth
section holding **free-text interpretive notes** with per-line formatting:

```ini
[report]
ext=1
titlebold=1  titleitalic=1  titlesize=10
titlecolor=255  wordxcolor=16711680  printxo=0
title1=Interpretation of HbA1c test :
LINE1_1=Result interpretation according to American Diabetets Association
LINE1_2=HbA1c  4.2 - 6.2 Normal
LINE1_5=6.8  -  7.6  %  Satisfactory Controlled Diabees
LINE1_4=5.5  -  6.8  %  Good Controlled Diabees
LINE1_6=>  7.6  %  Unsatisfactory Controlled Diabees
LINE1_8=- Glycated Hb. , Blood Glucose and M.A.U are recommended once every three months.
LINE1_9=Fructosamine is recommended once every months for diabetic patients.
LINEBOLD1_2=1   LINEBOLD1_8=1   LINEBOLD1_9=1
LINESIZE1_3=8   LINESIZE1_5=8   LINESIZE1_8=8
```

Up to **12 numbered lines** (`LINE_1`…`LINE_12`), each with an independent
bold flag and point size, plus a titled block (`title`, `titlebold`,
`titleitalic`, `titlesize`, `titlecolor`). A second block (`LINE1_1`…`LINE1_12`,
`title1`…) supports a second independent note group.

This is **machine-formatted clinical interpretation embedded in a
configuration file** — and it is entirely editable without recompiling.
See **FND-B3-001**.

### 3.7 The `_S` variant

73 of the 146 templates have an `_S` sibling (`450.ini` / `450_S.ini`).
A direct diff of `Data/116.ini` against `Data/116_S.ini` shows the variant is
the **same report with a different layout and a different item selection**:

| Key | `116.ini` | `116_S.ini` |
| --- | ---: | ---: |
| `printsen` | 1 | **0** |
| `titleleft` | 226 | 170 |
| `left1`…`left7` | 567, 963, 3005, 3175, 5386, 5159, 7200 | 226, 567, 2154, 2268, 4422, 5159, 5499 |
| `width7` | 2608 | 2324 |
| `bottomvalue`, `leftvalue` | 600, 600 | 500, 500 |
| `reportstart` | 1800 | 2000 |
| `CheckT07`, `CheckT08` | 1 | **0** |

`printsen` is the **sensitivity-panel print flag** — the `_S` variant suppresses
the antibiotic-sensitivity section. The column origins shift left and the
vertical margins tighten by 100 units. The `CheckT` flags drop optional item
groups.

**Interpretation.** The `_S` suffix is a **secondary layout** of the same test
— a narrower/alternate presentation, and in the microbiology templates the one
that omits the susceptibility panel. B1's own template family uses the same
`_S` convention (`FMF.ini` / `FMF_S.ini`, `SEMEN5.ini` / `SEMEN5_S.ini`,
`URINEauto.ini` / …), so the convention is **product-wide**, not a B3 artefact.
What selects between the two variants is not in this batch.

---

## 4. The clinical reference library (`Cache/`)

58 RTF files, **29 distinct documents**, mirrored into both trees
byte-identically.

### 4.1 Provenance

Every document's `\info` group records the same authorship:

| Field | Value |
| --- | --- |
| `\generator` | `Microsoft Word 10.0.2627` — **Word 2002** |
| `\vern` | 16437 |
| `\ansicpg` | **1256** (Arabic ANSI) |
| `\adeflang` | **3073** (Arabic, Saudi Arabia) |
| `\deflang` | 1033 (US English) |
| `\deflangfe` | 2052 (Simplified Chinese) or 1033 |
| `\author` | `real lab` |
| `\operator` | `real lab` (17 docs) or `ais` (12 docs) |
| `\company` | **`ais group`** |

`ais group` is the vendor. B1 independently records
`AssemblyCompany("ais group")` in the .NET e-mail component, and B3 ships two
fonts named `ais lab.ttf` and `ais symbol.ttf`. **Three independent sources in
three different batches name the same vendor.**

### 4.2 Content

Each document is a **test reference sheet**: the test name, reference ranges
split by population, SI-unit conversions, critical levels, specimen type, and
bulleted clinical-significance notes.

Extracted from `Cache/006.rtf`:

```
Alkaline Phosphatase (ALP)
Adult: 42–136 U/L
Child: 50–230 U/L
► Red-top tube.
■ Enzyme found predominately in the liver, biliary tract, and bone.
■ Useful in assessing liver and bone disease.
■ ALP isoenzymes distinguish between liver and bone disease. ALP1 is hepatic;
  ALP2 is from bone.
```

From `Cache/013.rtf`:

```
Blood Urea Nitrogen (BUN)
Adult: 10–20 mg/dL; SI units: 3.6–7.1 mmol/L
Child: 4–16 mg/dL; SI units: 1.4–5.7 mmol/L
Critical Levels:
> 40 mg/dL (not dehydrated/no history of renal disease)
> 100 mg/dL (patient with history of renal disease)
> 20 mg/dL increase in 24 hr (indicates acute renal failure)
► Red-top tube.
```

The 29 documents cover:

| # | Document | # | Document |
| --- | --- | --- | --- |
| 006 | Alkaline Phosphatase (ALP) | 175 | Calcium, Ionized |
| 007 | Albumin | 176 | Calcium, Total |
| 008 | Alanine aminotransferase (ALT) | 180 | Chloride (Cl) |
| 009 | Aspartate Aminotransferase (AST) | 295 | Aldosterone, Serum |
| 013 | Blood Urea Nitrogen (BUN) | 310 | Carcinoembryonic Antigen (CEA) |
| 015 | Bilirubin, Total, Direct, Indirect | 311 | Alpha-Fetoprotein (AFP) |
| 020 | Albumin/Globulin Ratio (A/G Ratio) | 315 | CA 15.3 |
| 023 | Ammonia | 316 | CA 19.9 |
| 030 | Cholesterol, Total | 317 | CA 125 |
| 038 | Coombs Test, Direct | 420 | Antithrombin III (AT-III) |
| 039 | Coombs Test, Indirect | 495 | Complement C3 and C4 |
| 056 | **Complete Blood Count with Differential** | 500 | Coagulation Factors |
| 057 | Bleeding Time | 077 | C-reactive Protein / hs-CRP |
| 070 | **Arterial Blood Gases (ABGs)** | 079 | Antistreptolysin O (ASO) |
| 102 | Blood Cultures | | |

The population splits used are **Adult / Child / Newborn**, with separate
**critical levels** distinguished by clinical context (dehydration status,
renal history).

### 4.3 The two oversized documents

`Cache/056.rtf` (16,953,057 bytes) and `Cache/070.rtf` (3,495,666 bytes)
account for **97.3 % of the entire batch's bytes**. They are large because they
embed **vector illustrations**, not text:

| Document | Pictures | Format | Bytes |
| --- | ---: | --- | ---: |
| `056.rtf` | 4 | 2 × EMF + 2 × placeable WMF | 1,901,528 + 1,121,456 + 3,337,858 + 1,969,330 |
| `070.rtf` | 2 | 1 × EMF + 1 × placeable WMF | 619,616 + 1,087,998 |

The CBC-with-differential document carries blood-cell morphology
illustrations; the ABG document carries a blood-gas dissociation chart. All
six were extracted and validated as genuine `Windows Enhanced Metafile` and
`Windows metafile` images (`evidence/rtf-pictures/`).

**Consequence:** the reference library's weight is concentrated in two files
that must be loaded whole into a Rich Text control. See **FND-B3-007**.

---

## 5. The font set

Seven distinct fonts, shipped twice.

| File | TrueType family | Version | Vendor string | Purpose |
| --- | --- | --- | --- | --- |
| `ais lab.ttf` | **Tahomax** (= Microsoft Tahoma) | 3.14, June 17 2004 | **Matthew Carter** | UI/report text — **Microsoft's Tahoma, renamed**, with Microsoft's copyright, trademark and DSIG signature intact. See **FND-B3-008** |
| `ais symbol.ttf` | **Borderbats-Fleur2** | — | Print Artist: **Sierra On-Line, Inc.** | decorative borders and fleurons — *"Redistribution strictly prohibited"* |
| `Digiface.ttf` | **Digiface** | — | Print Artist: **Sierra On-Line, Inc.** | seven-segment / digital readout digits — *"Redistribution strictly prohibited"* |
| `FREE3OF9.TTF` | New | 1.00 | — | **Code 39** barcode symbology |
| `MCS Taybah.ttf` | **MCS Taybah S_U normal.** (subfamily *Agfa MCS*) | 1.0, Tue Jan 07 17:41:25 1997 | unique ID *Jeddah K.S.A. 10-jun-95* | bespoke Arabic/Latin clinical labels |
| `WBCW_UPCEAN.ttf` | WBCW_UPCEAN | 1.00, December 22 2009 | GetMySystem.com, Inc. | **UPC-A / EAN** barcode symbology (BarcodeWiz) |
| `WBCWS_UPCEAN.ttf` | WBCW_UPCEAN | 1.00, December 22 2009 | GetMySystem.com, Inc. | UPC-A / EAN barcode symbology (variant) |

Two of these close gaps left open by earlier batches:

* **B1's FND-009** recorded that the installer "installs fonts and can request a
  reboot". These are those fonts. `MCS Taybah.ttf` is a **bespoke clinical
  typeface** and its own metadata dates and places it: `name[2]` = *"Agfa
  MCS"*, `name[3]` (unique ID) = *"Jeddah K.S.A. 10-jun-95"*, `name[5]` =
  *"1.0 Tue Jan 07 17:41:25 1997"*. **Jeddah, Saudi Arabia, June 1995** —
  the font set is part of the per-site installation rather than a generic
  redistribution, and the origin of the product's Arabic typography is
  explicitly Saudi, not Levantine as the name alone might suggest.
* **B1's** barcode keys (`barcodeport`, `barcodeprinter`, `barcodefile`,
  `barcodeauto`) and the `barcode` column on `patientinfo` gain their
  rendering counterpart in `FREE3OF9.TTF` and `WBCW_UPCEAN.ttf`.

`MCS Taybah.ttf` carries `fsSelection = NO_SUBSETTING | NO_PDF_EMBED` with no
embedding restriction bit set — that is **installable embedding**: the font
may be embedded whole, but must not be subset and must not be embedded in PDF.
This is consistent with a font intended to be embedded whole in documents.

See **FND-B3-006** (barcode check digit) and **FND-B3-008** (licence and
provenance — two of these fonts carry the notice "Redistribution strictly
prohibited", and `ais lab.ttf` is Microsoft's Tahoma under a renamed identity,
complete with Microsoft's own digital signature block).

---

## 6. The HTML report designer (`System/datareport_vh/`)

This is the batch's most important architectural finding.

### 6.1 What B2 left open

B2's `archive-inventory.md` documents the directory:

```
RL_CD2019Access/
└── System/
    └── datareport_vh/    9 files — the report-viewer deployment group
        ├── RICHTX32.OCX (675 B stub)  dispex.dll   jscript.dll  msscript.ocx
        ├── mssetup.dll  mswless.dep  mswless.ocx  selfreg.dll  sp698vbo.dll
        └── vbscript.dll
```

B2's executive summary reports that "the report-viewer subsystem embeds two
script engines" and lists as an open limitation: "the script engines in
`datareport_vh/` are present but their consumer is not identified".

### 6.2 What B3 supplies

B3 places **14 GIF files in the identical directory path**:
`RL_CD2019Access/System/datareport_vh/`. Rendered, they are:

**Eight toolbar buttons** (20 × 15 px, 32-colour palette):

| File | Image |
| --- | --- |
| `anchorwi.gif` | an anchor with a wrapping arrow |
| `br.gif` | a bold **B** with a text baseline |
| `clientsc.gif` | a script/page glyph with a horizontal rule — **client script** |
| `comments.gif` | a speech-bubble glyph |
| `pbgn.gif` / `pend.gif` | matched page-begin and page-end markers |
| `style.gif` | a stylesheet glyph |
| `unknownt.gif` | a question-mark glyph — **unknown tag** |

**Six HTML element markers** (43 × 14 px, 8-colour palette) — each rendering
its literal tag text on an arrow-shaped plate:

`div` · `/div` · `form` · `/form` · `span` · `/span`

### 6.3 What that means

A toolbar with **bold**, **comments**, **style**, **client script**, **page
break** and **unknown tag** commands, paired with a palette of **HTML element
start/end markers**, is an **HTML authoring user interface**. The report
designer is HTML-based. `clientsc.gif` is the "insert client script"
command; `unknownt.gif` is the "unknown HTML tag" indicator.

**Combined with B2's evidence — JScript 5.1.0.8513, VBScript 5.1.0.8513 and
Microsoft Script Control 1.0.0.3715, in the same directory — the report
subsystem is a scriptable HTML editor/viewer.** B2's open question has its
answer, and it is recorded as **FND-B3-002**.

**[INFERRED, Medium confidence]** `mswless.ocx` + `mswless.dep` (B2) are
most plausibly the VB6 control that hosts this designer; the name suggests a
"Web/less" report host. The GIF set would then be that control's resource
files, loaded from its own directory. This is not confirmed — the control's
resources are in B2 and were not re-analysed here.

---

## 7. The report graphics

### 7.1 Quantitative-PCR graphics

Five files, 800 × 320 px except `PCRF.jpg` (400 × 266), shipped in **both**
trees (10 copies):

| File | Content |
| --- | --- |
| `PCRD.jpg` | real-time PCR **amplification chart** with the overlay label **BELOW** |
| `PCRF.jpg` | the **detection-principle schematic** — "Target Sequence", "Primer 1", "Primer 2", "Taq DNA Polymerase", "Biotin", 5′/3′ strand ends |
| `PCRH.jpg` | amplification chart with the overlay label **HIGH** |
| `PCRL.jpg` | amplification chart with the overlay label **LOW** |
| `PCRM.jpg` | amplification chart with the overlay label **MODERATE** |

The charts plot fluorescence (RFU) against amplification cycles with a
threshold line, and a family of amplification curves. The four status overlays
— BELOW / LOW / MODERATE / HIGH — are the **quantitative viral-load bands**.
`PCRF.jpg` is the assay-principle diagram, showing a biotin-labelled primer
and a Taq-polymerase extension, i.e. the detection chemistry the report
describes.

These pair with the **16 molecular-biology templates** (IDs 450–465:
HBV-DNA qualitative and quantitative, HCV-RNA qualitative and quantitative,
HCV genotyping, and eleven more). Only five graphics ship for sixteen
templates, so the graphics are **shared**, and the band that applies to a given
result is presumably chosen at run time. Which graphic binds to which template
is not stated in the data — see `dependencies.json` DEP-B3-U04.

### 7.2 Report stationery

Nine files in `RL_CD2019Access/bin/` only:

| File | Size | Content |
| --- | ---: | --- |
| `Header.jpg` | 680 × 140 | bilingual letterhead: "Clinical laboratory", "Laboratory Name", "Laboratory Logo", "For medical analysis", "Tel: 0000000 - 0000000", Arabic "مختبرات التحاليل الطبية" / "اسم المعمل" / "أ.د. فلان الفلاني" |
| `ZHeader.jpg` | 720 × 140 | Arabic-only header |
| `Tailer.jpg` | 680 × 50 | footer: telephone line and an Arabic oversight line |
| `ZTailer.jpg` | 720 × 50 | Arabic-only footer |
| `Text.jpg` | 460 × 60 | introduction band ("Clinical laboratory / Laboratory Name / Laboratory Logo / For medical analysis") |
| `Wasl.jpg` | 720 × 50 | the laboratory-identity band: Arabic "اسم المعمل" + telephone + an examination line |
| `Logo.jpg`, `Logo1.jpg`, `Logo2.jpg` | 145 × 95 | **byte-identical** placeholder logo boxes reading "Laboratory Logo" |

Three observations follow.

1. **The stationery is a pre-rendered template with placeholders**, not a
   parameterised draw. The laboratory name, the logo and the telephone number
   are baked into a JPEG, so per-site identity must be supplied by replacing
   the image files. See **FND-B3-005**.
2. **`Wasl.jpg` is named after B1's reference table `wasl`** — B1's
   `software-architecture.md` lists `wasl` among the Access reference tables
   alongside `abbreviations`, `constanttable`, `barcodes`, `comments`,
   `customtesttable` and `daytip`. The correspondence suggests the laboratory's
   identity (name, logo, telephone) is held as a **database row** and rendered
   into a matching letterhead band. The image is the *rendered* form of that
   row. This is a name correspondence only — it is recorded in
   `dependencies.json` as DEP-B3-009 with **Medium** confidence.
3. **The `Z` prefix denotes the Arabic-only variants.** `ZHeader`/`ZTailer`
   are wider (720 px vs 680 px) and carry only the Arabic text.

The letterhead and footer also assert a **ministry-of-health oversight line**
in Arabic, next to a telephone field whose value is the placeholder
`0000000`.

---

## 8. The report pipeline, reconstructed

```mermaid
flowchart TD
    DB[(SQL Server<br/>clinical results)] -->|B1: SQL assembled from<br/>sql* keys in Data/*.ini| EXE[VB6 main executable<br/>(B1)]
    MDB[(Access/Jet mirror<br/>reference tables incl. wasl<br/>(B1)] --> EXE

    EXE -->|read by test ID| TPL["Data/&lt;id&gt;.ini<br/>or &lt;id&gt;_S.ini<br/>(B3)"]
    EXE -->|read| CACHE["Cache/&lt;n&gt;.rtf<br/>reference text<br/>(B3)"]
    EXE -->|load| FONT["Fonts/*.ttf<br/>(B3)"]
    EXE -->|load| IMG["bin/PCR*.jpg<br/>stationery *.jpg<br/>(B3)"]

    TPL --> LAYOUT["[reportdata] geometry, colour, font matrix<br/>[FORM] result-item schema<br/>[report] interpretive text<br/>[setupx] printer"]
    CACHE --> REFTXT["reference ranges, critical levels,<br/>specimen type, clinical notes"]

    LAYOUT --> RD[Report renderer]
    REFTXT --> RD
    FONT --> RD
    IMG --> RD
    RD --> PRN[Printer / barcode device]

    subgraph designer["HTML report designer - RL_CD2019Access/System/datareport_vh/"]
      GIF["14 GIF assets (B3)<br/>toolbar + HTML tag markers"]
      ENG["jscript.dll, vbscript.dll,<br/>msscript.ocx, mswless.ocx (B2)"]
      GIF --- ENG
    end
    designer -.-> RD
```

---

## 9. Architectural conclusions

1. **B3 is the data layer of a configuration-driven report system.** Report
   layout, colour, typography, result vocabulary, reference limits,
   interpretive text and printer binding are all *data*, editable without
   recompiling. This confirms and completes B1's §2.4.
2. **Two template families coexist.** B1's `sql*`-bearing, name-keyed templates
   assemble the query; B3's numeric-ID templates define the result schema and
   the print layout. They are complementary layers, not duplicates.
3. **The installation's `RL_CD2019Access/` tree is the complete one**; the
   root-level `bin/`, `Cache/`, `Data/` and `Fonts/` folders are a reduced,
   byte-identical subset — with the single exception of `bin/`, where the
   Access tree adds the nine stationery files and the root tree adds nothing.
4. **The report viewer is a scriptable HTML subsystem**, and B3 supplies the
   half of it that B2 could not see.
5. **The reference library, the template catalogue and the font set are all
   plain-text or plain-asset content** — the parts of the product most likely
   to be customised per site, and the parts with no integrity protection.
6. **The product is Arabic-locale-targeted** — evidenced independently by the
   cp1256 encodings, `\adeflang3073`, the Arabic stationery, the Arabic
   allergen labels and the Arabic clinical font `MCS Taybah.ttf`.

---

## 10. Unresolved architectural questions

| ID | Question | Where the answer would be |
| --- | --- | --- |
| DEP-B3-U01 | Where is `Data/REPORT.ini`, the global report colour theme the main executable reads? | B4/B5, or the installer |
| DEP-B3-U02 | Where is B1's `RL_CD2019Access/Data/data_emp/` generic-template repository? | B1's own portion |
| DEP-B3-U03 | Which code selects the numeric template and the `_S` variant? | B1's main executable |
| DEP-B3-U04 | Which of the 5 PCR graphics binds to which of the 16 PCR templates? | B1 or the instrument-interface configuration |
| — | What is the test-ID → `testtable` row mapping? | the database (B1's portion) |
| — | Is `Wasl.jpg` genuinely rendered from the `wasl` reference table? | B1's Access schema and B1's executable |
| — | Does the report designer execute client script in the deployed configuration? | runtime; B2's `msscript.ocx` and the designer host |
