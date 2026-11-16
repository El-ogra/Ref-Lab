# Data Flow and Control Flow — Batch B3 of RealLab CD 2019 v1.605

**Batch:** B3 · **Archive:** `v1.605-B3.zip` · **SHA-256:** `2fb942c377d1e06b6ba473302e3ad8e8ba8a6faa39513bb9a296a73d64ee3a7b`
**Analysis date:** 2026-10-31 (UTC) · **Method:** static only; nothing was executed

> B3 contains no code, so this document reconstructs the **data flow that B3's
> files participate in**, distinguishing what B3 evidences directly from what is
> inherited from B1/B2 and what is inference.

---

## 1. Evidence classes used throughout

| Marker | Meaning |
| --- | --- |
| **[OBSERVED]** | Directly established from B3's file content |
| **[INHERITED]** | Documented by B1 or B2 and not re-verified in this session |
| **[INFERRED]** | Reasoned from observed facts; confidence stated |
| **[UNRESOLVED]** | Open; the evidence needed is named |

---

## 2. The configuration-read flow

**[OBSERVED]** B3's templates are **flat, path-addressed INI files with numeric
names**. Each is addressed by a test ID, not by a path expression. The
consuming code must therefore compute a filename from a test ID.

**[INHERITED]** B1 established the read pattern: the main executable reads
`D:\real lab system\Data\REPORT.ini` and other `Data\*.ini` files by absolute
path, and its `Data\` path is a fixed installation-relative location.

**[INFERRED, High confidence]** The read flow is:

```
test ID (from the result record)
   └─> Data\<ID>.ini                (primary layout)
       Data\<ID>_S.ini              (secondary layout, when selected)
```

The `_S` suffix is confirmed to exist for 73 of 146 IDs. The **selection
criterion** is **[UNRESOLVED]** — it is decided in B1's main executable. The
evidence suggests it is a per-test or per-site choice rather than a
content-driven one, because the `_S` variant differs from its base in both
geometry and which optional item groups are printed (`CheckT07`, `CheckT08`
flip to 0).

**[OBSERVED]** Two independent directory trees carry the same files:

| Tree | `Data/` | `Cache/` | `Fonts/` |
| --- | ---: | ---: | ---: |
| root | 37 | 29 | 7 |
| `RL_CD2019Access/` | 109 | 29 | 7 |

All 37 root `Data/` templates are **byte-identical** to their Access-tree
counterparts. The 72 additional Access-tree templates carry IDs 100–115,
152, 230, 400, 426 and 450–465 — precisely the microbiology, serology,
biochemistry, haematology and molecular-biology disciplines.

**[INFERRED, Medium confidence]** The application reads from **one** of the two
trees depending on which front end is running — the VB6 executable against the
root tree, the Access mirror against the `RL_CD2019Access/` tree. B1's
executive summary describes exactly this two-tree structure. **[UNRESOLVED]**
which tree a given run reads.

---

## 3. The report-assembly flow

### 3.1 Layer 1 — the query (B1's templates)

**[INHERITED]** B1's `Data/*.ini` templates carry the SQL-assembly key family:
`sqlstatement`, `sqlselect`, `sqlselect1..3`, `sqlfrom`, `sqlwhere`,
`sqlorder`, `sqlgroup`, `sqlsum`, `sqlexpression`, `SELECT_1..SELECT_10`,
`SELECT_D1..D3`.

**[OBSERVED]** **B3's 146 templates contain none of these keys.** A search for
`sqlstatement`, `sqlselect`, `sqlfrom` and `sqlexpression` across all 146
files returns **zero matches**.

This is an important negative result: B3's templates are **not** the templates
B1 documented, and the two template families are **complementary layers**, not
two copies of the same thing.

### 3.2 Layer 2 — the result schema (B3's templates)

**[OBSERVED]** B3's `[FORM]` section defines, for one test:

* the **items** the result consists of (`LabelT01`…`LabelT15`, `LabelX1`…`LabelX4`);
* the **selectable values** for each item (`ITEM01_1`…`ITEM15_N`);
* the **secondary/display value** for each (`ITEMS1_1`…`, `ITEMXS1_1`…`);
* the **default** selection (`ITEM01_D1`…`D3`);
* the **reference interval** and **reference limits** (`ITEM01_Ref`, `RefLimit1..3`);
* **display bounds** (`ITEM01_DH`, `ITEM01_DL`);
* **titration dilutions** (`titr_1`…`titr_15`);
* **pH-category boundaries** (`ITEM01_AcidicL/H`, `_NeutralL/H`, `_AlkalineL/H`);
* **vocabulary lists** (`Acidic05_1..20`, `Alkalin05_1..20`, `Nitrl05_1..20`);
* **which optional items are shown and printed** (`Check1x..7x`, `Check1f..`,
  `CheckT01..T08`).

**[INFERRED, High confidence]** The result-capture form is **generated from
this template at run time**. The `Check*` flags are form-visibility switches
and the `ITEM##_Value` families are dropdown contents. A template with
`CheckT07=0` does not present that item group at all.

### 3.3 Layer 3 — the layout (B3's templates)

**[OBSERVED]** `[reportdata]` supplies the print geometry as absolute
coordinates in printer device units:

* seven column origins `left1`…`left7` and `titleleft`;
* per-column `width1`…`width7`;
* vertical band `topvalue` / `bottomvalue` / `leftvalue` / `rightvalue`;
* spacing `spacevalue` / `spacetitle` / `spacenormal`;
* initial offset `reportstart`;
* 20 base colours `colora`…`colort`;
* per-slot typography `colorXfn` / `colorXfs` / `colorXfb` / `colorXul`.

### 3.4 Layer 4 — the narrative (B3's templates and cache)

**[OBSERVED]** Two independent narrative sources feed the same report:

* `[report]` in 34 templates — an **interpretive note** attached to the test
  (for example the HbA1c/ADA interpretation band with its reference ranges and
  recommendations);
* `Cache/<n>.rtf` — the **reference sheet** for the test, supplying reference
  ranges by population, SI conversions, critical levels, specimen type and
  clinical-significance bullets.

**[OBSERVED]** The two use **different addressing**: `[report]` is keyed by
test ID (inside the template), while `Cache/` is keyed by a **separate numeric
namespace** (`006`, `007`, … `500`) that does not overlap the template ID
space.

**[UNRESOLVED]** How a cache document is bound to a test. The cache IDs
(006–500) look like they belong to a *test-catalogue* numbering rather than the
template IDs, and the two numbering schemes are not reconcilable from B3 alone.
The most likely reading — that the cache is keyed by the same catalogue the
`testtable` uses (B1's `testtable.testcode`) — cannot be confirmed here.

---

## 4. The rendering flow

```
result record
    │
    ├─ template  Data\<ID>.ini  /  Data\<ID>_S.ini        [B3]
    │      ├─ [reportdata] ── geometry, colours, fonts ─────────┐
    │      ├─ [FORM]      ── item schema, defaults, references  │
    │      └─ [report]    ── interpretive text                  │
    │                                                          │
    ├─ reference  Cache\<n>.rtf                                [B3]
    │      └─ reference ranges, critical levels, notes         │
    │                                                          │
    ├─ fonts     Fonts\*.ttf                                   [B3]
    │      ├─ report text (Tahoma / Times New Roman / Arial)   │
    │      ├─ Arabic clinical labels (MCS Taybah)              │
    │      ├─ barcode symbologies (Code 39, UPC/EAN)           │
    │      └─ decorative borders, digital digits               │
    │                                                          │
    ├─ graphics  bin\PCR*.jpg, bin\*.jpg                       [B3]
    │      ├─ PCR amplification chart (BELOW/LOW/MODERATE/HIGH) │
    │      └─ PCR detection-principle schematic                 │
    │                                                          ▼
    │                                             report renderer
    │                                    (MSDBRPTR.DLL — Microsoft Data
    │                                     Report, B2; driven by the
    │                                     VB6 main executable, B1)
    │                                                          │
    │                                    ┌─────────────────────┴──────────┐
    │                                    ▼                                ▼
    └──────────────────────►      printed report                   HTML preview /
                                 (printer canvas,                report designer
                                  absolute twip grid)             (System/datareport_vh,
                                                                 B2 engine + B3 assets)
```

**[OBSERVED]** The stationery band (`Header.jpg`, `Wasl.jpg`, `Tailer.jpg`,
`Text.jpg`, `ZHeader.jpg`, `ZTailer.jpg`) is a **pre-rendered raster** that
must be composited onto the page — it is not drawn from the template. Its
identity fields are placeholders.

**[INHERITED]** B2 established that `MSDBRPTR.DLL` is the Microsoft Data
Report rendering engine and that the VB6 application renders through the
chart/grid/rich-text controls.

**[UNRESOLVED]** Whether the report is composed once into a bitmap/canvas and
printed, or rendered to a device context incrementally. The absolute
twip-coordinate layout is consistent with the former.

---

## 5. The HTML report-designer flow

This is the flow B2 could not complete and B3 supplies the second half of.

```
B2 (engine)                                  B3 (assets)
─────────────────────────────                ────────────────────────────
RL_CD2019Access/System/datareport_vh/
  jscript.dll   5.1.0.8513                     anchorwi.gif   (anchor)
  vbscript.dll  5.1.0.8513                     br.gif         (bold)
  msscript.ocx  1.0.0.3715                     clientsc.gif   (client script)
  mswless.ocx   + mswless.dep                   comments.gif   (comments)
  RICHTX32.OCX  (stub)                         style.gif      (stylesheet)
  selfreg.dll / sp698vbo.dll /                pbgn.gif/pend.gif
    dispex.dll / mssetup.dll                   unknownt.gif   (unknown tag)
                                                divbgn.gif / divend.gif
                                                formbgn.gif / formend.gif
                                                spanbgn.gif / spanend.gif
```

**[OBSERVED]** The 14 GIFs form a coherent **HTML authoring toolbar and HTML
element palette**. The names of the toolbar buttons map one-to-one onto
standard HTML-editor commands.

**[INFERRED, Medium confidence]** The designer control loads its button images
from its own directory at run time — the standard Windows practice, and the
reason the assets and the engine share a directory. The control that hosts
them is most plausibly `mswless.ocx` (B2), whose `.dep` descriptor marks it as
a VB6 setup-wizard dependency.

**[UNRESOLVED]** Whether the designer is a *live* feature of the deployed
product, and whether the report preview executes client script. B2's
`msscript.ocx` provides the script host; the assets prove the authoring
surface exists. The two facts together make the surface plausible but do not
prove script execution in the deployed configuration. See **FND-B3-002**.

---

## 6. The barcode flow

**[OBSERVED]** B3 ships two barcode symbology font families:
`FREE3OF9.TTF` (Code 39 / "3 of 9") and `WBCW_UPCEAN.ttf` +
`WBCWS_UPCEAN.ttf` (UPC-A / EAN).

**[INHERITED]** B1 records the application's barcode configuration keys —
`barcodeport`, `barcodeprinter`, `barcodefile`, `barcodeauto` — and a
`barcode` column on the `patientinfo` table. B2 records that the VB6
application drives the printer and barcode device through `RICHTX32.OCX` and
`MSCHRT20.OCX`.

**[INFERRED, Medium confidence]** The flow is:

```
patient / sample record
   └─> barcode value  (patientinfo.barcode, B1)
       └─> symbology chosen from the configured type
           └─> rendered as glyphs by FREE3OF9 / WBCW_UPCEAN
               └─> printed on the specimen label
```

**[UNRESOLVED]** Whether a check digit is computed and embedded in the encoded
value. The font renders symbols only. See **FND-B3-006** — recorded as
**Suspected**, not confirmed.

---

## 7. The identity flow — `wasl`

**[OBSERVED]** `RL_CD2019Access/bin/Wasl.jpg` is a pre-rendered band carrying
the laboratory's name, a telephone field and an examination line, in Arabic.

**[INHERITED]** B1 lists `wasl` among the Access mirror's reference tables,
alongside `abbreviations`, `constanttable`, `barcodes`, `comments`,
`customtesttable` and `daytip`.

**[INFERRED, Low-to-Medium confidence]** `Wasl.jpg` is the **rendered form of
a row in the `wasl` table**, and the stationery set is generated from the
laboratory's identity record. The alternative — that `Wasl` is simply the name
of a specific customer laboratory whose letterhead was captured as an image —
is equally consistent with the evidence.

**[UNRESOLVED]** Whether the identity is drawn from the database or baked into
the JPEG. The placeholders in the shipped images ("Laboratory Name",
"Tel: 0000000") suggest the shipped files are *templates to be replaced*, not
*renders to be substituted*. See **FND-B3-005**.

---

## 8. Flows B3 does NOT participate in

For completeness, these are the flows of the wider system that **do not touch
any B3 file** — stated so that B3's contribution is not overstated:

| Flow | Where it lives |
| --- | --- |
| Patient registration, billing, payment | B1's executable and the Access mirror |
| Result capture from analysers | B1's executable and the `instrument` table |
| SQL Server synchronisation | B1's executable (`Create Table` statements for `testtemp`, `MyLabSetting`, `smsgroups`, `smslist`, `SMStemplate`, `LBC`, `IgEallergy`) |
| SMS notification | B1's executable and the `.NET` e-mail component |
| COM registration and deployment | B2's `.dep` descriptors and setup helpers |
| Script engine hosting | B2's `datareport_vh` group |
| Report rendering engine | B2's `MSDBRPTR.DLL` |
