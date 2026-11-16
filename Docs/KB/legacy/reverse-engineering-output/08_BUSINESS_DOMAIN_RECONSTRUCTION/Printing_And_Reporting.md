# Printing and Reporting — REAL LAB SYSTEM v1.605 (V-CD-2019)

> **Correction to a common misreading.** The reporting stack is **VB6 DataReport +
> Win32 spooler + Bullzip PDF Printer**, not .NET Reporting / Crystal / QuestPDF. Any
> reconstruction that assumes a .NET report engine would be wrong. This is VERIFIED from
> the object inventory, the DLL analysis and the installer payload.

**Evidence basis.** `Version_Identity_and_Scope.md` §3.2/§3.4,
`1605-template-corpus.md`, `template-corpus-228.json`, `13-pdf-print-architecture.md`,
`report-structure.md`, `pdf-print-stack.md`, `1605-phase2-findings.md` U-06/U-09/U-10,
`Business_Rules_V2.md` V2-BR-001…004/017/020/021/045,
`statements__[analysis_BATCH-04_sql_v1605].csv`, `reportview` queries.

## 1. Object inventory — VERIFIED

| Artefact | Count | Source |
|---|---:|---|
| VB6 objects | 210 (+41 vs v1.00) | `Version_Identity_and_Scope.md` §3.2 |
| Forms | **132** | same |
| DataReport designers | **66** | same |
| INI report templates | **228** files / **117** identities (base + `_S`) | `template-corpus-228.json`, `1605-template-corpus.md` |
| Files joining to a result table | 72 (36 identities) | same |
| Cached RTF method sheets | **29** | `report-structure.md` §1 |
| Embedded SQL statements | 1,797 (27 SHAPE) | `statements__[analysis_BATCH-04_sql_v1605].csv` |

`msdbrptr.dll` is "Microsoft Data Report Runtime DLL", version 6.01.9782 (SP4), an OLE
in-process server for the DataReport coclass; it imports the GDI print stack
(`StartDocA`/`StartPage`/`EndPage`/`EndDoc`/`AbortDoc`, `CreateDCA`/`CreateICA`,
`GetDeviceCaps`, `BitBlt`) and `comdlg32.PrintDlgA`. It also exposes `ExportReport` with
HTML/UTF-8/text formats. Confidence: VERIFIED (PE import parse in U-06/U-09).

## 2. Template anatomy

Each identity is a **base** template plus an **`_S` variant** with different geometry and
print flags; all three sections `[reportdata]`, `[FORM]`, `[setupx]` are present
(`1605-template-corpus.md` §1). Print-behaviour flags across the 228 files:

| Flag | Set to 1 | Source confidence |
|---|---:|---|
| `printcritical` | 218 | VERIFIED |
| `printhistory` | 222 | VERIFIED |
| `printsen` | 6 | VERIFIED |
| `printcommer` | 35 | VERIFIED |
| `check1x` / `check2x` / `check3x` | 46 / 222 / 222 | VERIFIED |

`normal` (reference-range heading) has 12 distinct spellings, including
`Reference Range :` (161 files), `Ref Range (Class):`, `Evaluation Criteria :`, and the
vendor typo `Nolrmals according to WHO` (NEW-4). The per-report printer override
`reportprinterx` is `HP LaserJet 1100 (MS)` in 217 of 228 files (NEW-5) — effectively a
single legacy default, not a per-discipline choice. `form_item_count` ranges 0–561;
files with 0 items rely on the `reportdata`/`setupx` sections and the joined profile table
rather than a `[FORM]` item grid — STRONGLY_SUPPORTED from `template-corpus-228.json`.

Discipline titles present in the corpus: MICROBIOLOGY, MOLECULAR BIOLOGY, BIOCHEMISTRY,
HEMATOLOGY, ENDOCRINOLOGY, SEROLOGY, IMMUNOLOGY, CYTOPATHOLOGY, VIROLOGY, CROSS MATCHING,
CSF EXAMINATION, BONE MARROW EXAMINATION, SERODIAGNOSIS, SEMEN ANALYSIS, POST-COITAL,
PROSTATIC DISCHARGHE, CALCULI ANALYSIS, TORCH SCREEN, MANTOUX PPD, URINALYSIS,
AUTOMATED URINALYSIS, ZN Film (`1605-template-corpus.md` §2).

## 3. Data assembly for composite reports — `SHAPE`

`SHAPE {…} APPEND ({…}) … RELATE … TO …` is the MSDataShape mechanism used to merge
profile tables into one report dataset. The v1.605 corpus contains **27 SHAPE statements**;
representative VAs:

| VA | Shape | Confidence |
|---|---|---|
| `672722`, `6729D6` | `subgrouptable` parent → `testtable` child, `RELATE subgroupcode TO subgroupcode` | VERIFIED |
| `672E3E`, `673172`, `673662`, `673986` | `testtable` parent (patientprice / labprice + `typeofsample`) → `normalrange` child, `RELATE testcode TO testcode`, `normalrang <> ''` | VERIFIED |
| `70ECE4`, `70F278` | `testname` parent → `patientinfo`/`patienttest` child | VERIFIED |
| `6BDE24` | `culturesec INNER JOIN cultureanti ON culturesec.secabre = cultureanti.atisens` (culture & sensitivity) | VERIFIED |
| `697446`, `697B32`, `69DAA6`… | patientinfo financial aggregates shaped for bills | VERIFIED |

`data_connection1.udl` sets `Provider=MSDataShape.1`; the consolidated path appears bound
to the SQL Server catalogue (`Initial Catalog=Patients`, `SQLOLEDB.1`). Whether it works
against the Access default is NOT ESTABLISHED (V2-BR-017). Report strip configuration is
read from `reportview` (`SELECT stripnum,stripdiscrp,strip1 FROM reportview where
stripdiscrp='MAIN'`, VA `66F882`) — VERIFIED.

## 4. Printer roles and PDF path

`13-pdf-print-architecture.md` §3 lists six printer roles: `reportprinter`,
`reportprinterx`, `zarfprinter`, `barcodeprinter`, `waslprinter`, `IDprinter` (the last two
new in 2020). PDF production is entirely external:

```
VB6 DataReport (MSDBRPTR.DLL) → Win32 printer queue "Bullzip PDF Printer"
  → bzpdf.dll + bundled Ghostscript → D:\PDF\{Contracts,Free,Individual,LabToLab}\
```

- Literal `Bullzip PDF Printer` (exact device name) and `Bullzip PDF Printer 11.1 - Create
  File` (default output name) are hard-coded in `real lab system.exe` v1.605
  (V2-BR-001/002, VERIFIED from literals). The driver version number is embedded — a
  version-pinned coupling.
- `SetupPZ.bat` installs `System\PDF\Setup_BullzipPDFPrinter_11_1_0_2600_PRO_EXP.exe` under
  .NET Framework 4; side tools (manual use only) are Ghostscript Lite 9.14.17, Xpdf Lite
  3.3.4 and PDF Power Tool 4.0.0.11 (`13-pdf-print-architecture.md` §1).
- v1.00 has **zero** occurrences of `PDF`/`Bullzip`; the whole PDF path is a v1.605
  addition (141 `PDF` occurrences, 2 `Bullzip`) — VERIFIED cross-version delta.
- Per-profile margins (`leftvalue`, `rightvalue`, `topvalue`, `bottomvalue`) live in each
  `Data\*.ini`; paper size is global in `SETTING.ini` (V2-BR-020). Global headers
  `Textr1txt` / `Textr2txt` coexist with per-profile titles (V2-BR-021).

## 5. Print control, audit and quality gate

- `Not found any Verified selected tests or result to print it` — the release gate that
  refuses unreviewed results (V2-BR-038; `13-pdf-print-architecture.md` §4).
- `testcode in (6,7,8,9,14,15,16,17,18,19,20,21)` — a hard-coded 12-test print restriction
  in one path (same source). Other lists exist: `testcode in (1,2,3)` (VA `6B2D70`),
  `(10,11,12,594,24,25,26,27,28,29,197)` (VA `6BA22C`), and larger panels — evidence that
  print routing is per-discipline, not uniform.
- Audit: `resultprint`, `printuser`, `printdate`, `printnum` (VAs `681ED2`, `6B037C`,
  `6B2D70`, `6865A6`). `printnum` is a sequence counter for detecting unauthorised copies.
- Barcode fonts and Arabic fonts (FREE3OF9, WBCW_UPCEAN, WBCWS_UPCEAN, Digiface; MCS
  Taybah, ais lab, ais symbol) are prior-work claims from the stub and `09-presentation-library.md`;
  the font install is corroborated by `SETUP.LST` (`$(Font)`/`$(Shared)` in
  `unified-findings-register.md` §4). STRONGLY_SUPPORTED.

## 6. Method sheets / critical levels

`report-structure.md` §1: 29 RTF files, cp1256, `\adeflang3073`. `056.rtf` (CBC) is
16,953,057 bytes with 4 embedded images and the critical thresholds
`Hgb < 5 or > 20 g/dL`, `Hct < 15% or > 60%`, `WBC < 500 or > 50,000/mm³`,
`Platelets < 50,000 or > 999,000/mm³`. `013.rtf` (BUN/Urea), `175.rtf`/`176.rtf` (calcium),
`180.rtf` (sodium) carry further thresholds. These are **reference documents**, not
runtime validators; no link between a specific RTF and a specific test was established
(`contradictions-limitations-and-unresolved-questions.md` §2.3 item 9).

## 7. Prior knowledge preserved

| Prior stub claim | Revalidation | Status |
|---|---|---|
| 66 DataReports; 228 cached RTF templates | 66 DataReports VERIFIED; templates are **INI**, RTF sheets are 29 | PARTLY CORRECTED |
| 146 per-test INI configs | Original stub claim; 228 INI corpus / 117 identities is the verified figure | SUPERSEDED |
| Report titles list | `1605-template-corpus.md` §2 | VERIFIED |
| Default printer HP LaserJet 1100 (MS) | 217 of 228 templates | VERIFIED |
| SHAPE hierarchies feed DataReports | VAs `672722`, `672E3E`, etc. | VERIFIED |
| Per-workstation layout (U-034); PDF via Bullzip (evaluation) | `1605-phase2-findings.md` U-14/NEW-6; `13-pdf-print-architecture.md` | STRONGLY_SUPPORTED |

## 8. Unknowns

Exact call chain of the renderer (UNR-010 — Ghidra runs were OOM-killed); how the `_S`
variant is selected (UNR-014); per-print-path differences between `printout`/`printout2`/
`printout3` (U-06); app-level preview behaviour (U-09); where the "Print Result Password"
is stored (U-07 — no credential token found in MSDBRPTR.DLL, RL_SETUP.exe or the 228
templates); mapping of `pricemethod` to PDF category directory. All UNVERIFIED / UNKNOWN.

**Cross-references:** `Results_Management.md`, `Simple_And_Composite_Tests.md`,
`Security_And_Permissions.md`, `09_CROSS_SOURCE_EVIDENCE/Verified_Findings.md`.
