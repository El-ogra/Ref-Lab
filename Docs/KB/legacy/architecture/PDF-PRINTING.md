# Legacy PDF and Printing Architecture — V-ROOT-1.00 and V-CD-2019

**Purpose:** Curated reference for legacy printing and PDF generation.
**Scope:** Print workflow, printer roles, PDF export path, Bullzip integration, defects, audit.
**Audience:** Developers.
**Version applicability:** V1 has NO PDF capability (V1-BR-046, CONFIRMED ABSENT); V2 adds full PDF path.

---

## 1. Executive summary

**RealLab has no PDF engine of its own.** It sends its report to an external Windows printer
driver — `Bullzip PDF Printer` — whose device name and version are **hardcoded as string literals**
in the executable. The PDF capability is entirely an OS-layer integration, not application logic.

V1 has **zero** PDF capability (zero `PDF` literals in 104,997 strings). V2 adds 141 `PDF` literals.

## 2. Print workflow (both versions)

1. Operator presses **F11** (preview) or **F12** (print).
2. `printerselect` module resolves the target printer — from `Data\*.ini [setupx] reportprinterx`
   for the profile, or from `SETTING.ini [selectprinter]` for the role.
3. The `DataReport` engine renders the page using `Data\*.ini [reportdata]`.
4. Dispatch to a physical printer, to "Send To OneNote 2007", or (V2) to `Bullzip PDF Printer`.
5. The print is recorded on the test row (`resultprint`, `printuser`, `printdate`, `printnum`).

## 3. Printer roles

| Role | Identifier | V1 | V2 |
|---|---|---|---|
| Report printer | `reportprinter` | ✔ | ✔ |
| Report printer (alternate) | `reportprinterx` | ✔ | ✔ |
| Barcode printer | `barcodeprinter` | ✔ | ✔ |
| ZARF | `zarfprinter` | ✔ | ✔ |
| WASL | `waslprinter` | ✘ | ✔ (V2 only) |
| ID card | `IDprinter` | ✘ | ✔ (V2 only) |

V2 adds 6 comboboxes (`cboPrinters`…`cboPrinters5`) and 6 `imlPrinters` (V1 had 4). V2 uses
`EnumPrintersA` + `SetDefaultPrinterA` (V1 used `OpenPrinterA`/`SetPrinterA`/`GetPrinterA`/`ClosePrinter`).

## 4. PDF export path (V2 only)

**Mechanism:** the application prints to the Windows printer named `Bullzip PDF Printer`. The
Bullzip port driver (`bzpdf.dll`) renders the output via its bundled Ghostscript and writes the
PDF to disk.

**Hard-coded dependencies in `real lab system.exe` v1.605:**

| Offset | Literal | Consequence |
|---|---|---|
| `0x6A2486` | `Bullzip PDF Printer` | The device name must match exactly. |
| `0x6A255E` | `Bullzip PDF Printer 11.1 - Create File` | The default output filename is version-pinned to 11.1. |

**Output taxonomy:** `D:\PDF\Contracts\`, `D:\PDF\Free\`, `D:\PDF\Individual\`, `D:\PDF\LabToLab\`
— four categories matching the accounting methods (Lab To Lab / Individual / Free / contracted).

**Installation chain:** `SetupPZ.bat` → .NET Framework 4.0 → Bullzip 11.1.

**★ Three documented installation defects:**
1. `RL_SETUP.exe` calls `System\PDF\SetupP.bat` (Bullzip only) **before** `System\SetupPZ.bat`
   (.NET 4 then Bullzip) — Bullzip installed before its .NET 4 dependency exists.
2. `SetupFN4.bat` installs .NET 4 but is invoked by nobody; its relative path
   (`dotNetFx40_Full_x86_x64.exe`) is missing the `System/PDF/` prefix.
3. `setup.exe` launched with `/sc:\setup.log` and **without** `/wait`.

**No fallback evidenced:** if the Bullzip printer is absent or renamed, the PDF path fails.

**Independent tools shipped alongside (never invoked by the application — confirmed by literal
search):** GPL Ghostscript Lite 9.14.17 (AGPL v3), Xpdf Lite 3.3.4 (`pdftops.exe`), PDF Power Tool
4.0.0.11 (commercial). Bullzip itself is signed (BioPDF ApS, SHA-1).

## 5. Quality gates and audit

| Control | Evidence | Importance |
|---|---|---|
| No unverified result printed | "Not found any Verified selected tests or result to print it" | Quality gate |
| Only 12 test codes printable | `testcode in (6,7,8,9,14,15,16,17,18,19,20,21)` | Deliberate restriction |
| Print audit record | `resultprint`, `printuser`, `printdate`, `printnum` | Individual accountability |
| Numbered prints | `printnum` (Double — sequential counter) | Detect unauthorised copies |
| 5 output paths by category | `Contracts`, `Free`, `Individual`, `LabToLab` | Data separation |

**★ Design requirement:** the print audit (`printuser`/`printdate`/`printnum`) **must be preserved**
in any replacement — it is the strongest accountability control inherited. The modern system
already implements `PrintAudits` (EF Core table) with `Operator`, `PrintedAt`, `SequenceNumber`,
`Category`.

## 6. Report types (V2)

| Kind | Count | Mechanism |
|---|---:|---|
| `DataReport` design objects | 66 (V1: 45) | VB6 DataReport bound to `Data\*.ini` layouts |
| Printed clinical profiles | 114 categories | One `.ini` layout each (223 files) |
| Report layouts by discipline | — | MICROBIOLOGY (18), MOLECULAR BIOLOGY (16), BIOCHEMISTRY (15), HEMATOLOGY (9), ENDOCRINOLOGY (6), SEROLOGY (5), IMMUNOLOGY (2) + 17 more |
| Method sheets | 29 | `Cache\*.rtf` — byte-identical to V1 |
| Interactive training simulators | 13 | `Cache\*.exe` — byte-identical to V1 |
| Consolidated reports | — | `Reportanyreport` using `SHAPE … APPEND` union queries |
| Price lists | — | `pricelistsN` (V2 only) |
| Specialty reports | — | `ReportPCR`, `ReportIGE1`, `ReportFMF`, `ReportLBC`, `ReporturineAuto`, `ReportZN`, `ReportTestTime`, `ReportPatPhone`, `Reportliver7his`, `Reportmony01/11/5` |
| Patient ID card | — | `CardFrm`, `CardFrmSetx` (V2 only) |
| Patient visit report | — | `RPTPatientVisit` (V2 only) |
| SMS report | — | `ReportSMS` (V2 only) |

## 7. Report generation mechanism (unchanged in principle)

```
DataReport design object (66)
  + Data\*.ini layout for the profile (114)
  + patienttest / profile-table values
  ──────────────────────────────────────► rendered page
```

Report numbering starts from the absolute value `1984` (`SETTING.ini [setup] reportstart`).
Each profile binds its own printer (`Data\*.ini [setupx] reportprinterx`). Paper size is global
(`SETTING.ini [setup] papersize=A4`). Margins are per-profile (`leftvalue`, `rightvalue`,
`topvalue`, `bottomvalue`). Global header texts coexist with per-profile header texts
(`Textr1txt`, `Textr2txt` vs `formsubtitle`, `reportmaintitle`).

## 8. Export mechanisms

| Mechanism | V1 | V2 |
|---|---|---|
| Print to Windows printer | CONFIRMED | CONFIRMED |
| Print to "Send To OneNote 2007" | CONFIRMED | CONFIRMED |
| PDF export via Bullzip | ABSENT (V1-BR-046) | CONFIRMED |
| Print to file / image | NOT ESTABLISHED | NOT ESTABLISHED |
| `real lab Image Gallery.exe` | CONFIRMED (separate executable) | CONFIRMED |

## 9. Requirements for the modern replacement (extracted)

1. Do not depend on an externally installed printer driver with a hardcoded name and version.
2. Generate PDF inside the application (integrated rendering engine).
3. Replace .NET 4.0 RTM with .NET 4.8+ (or .NET 8, as done).
4. Preserve the print audit (`printuser`/`printdate`/`printnum`).
5. Preserve the "verified results only" quality gate.
6. Preserve the four output categories or make them configurable.
7. Sign all installers with SHA-256+.
8. Single sequential install with post-install verification.
9. Attach licence attributions (AGPL/GPL/commercial).
10. Support Arabic RTL in output.

**Evidence:** external: 13-pdf-print-architecture.md §6.

## 10. Version applicability

- PDF export, Bullzip integration, printer roles `waslprinter`/`IDprinter`, specialty reports,
  ID card, visit report, SMS report, print audit `printnum`: **V2 ONLY**.
- Print workflow, `printerselect` module, `DataReport` mechanism, `Data\*.ini` layouts,
  `SETTING.ini` printer binding: **SHARED** (V1 had 45 DataReport vs V2's 66).

## 11. Known limitations

- The mapping between a `pricemethod` value and a PDF directory is not established.
- Whether a printer-rename tolerance exists elsewhere in the code is not established.
- The exact filename construction (literal + timestamp, or literal alone) is not established.
- The unit of margin values is not established.

## 12. Related documents

- `Docs/KB/legacy/architecture/OVERVIEW.md`, `CONFIGURATION.md`, `DEPLOYMENT.md`.
- `Docs/KB/legacy/clinical/RULES.md` (print/quality gates).
- `Docs/KB/legacy/data/MODEL.md` (`patienttest` audit columns).
- Original source evidence: external: 13-pdf-print-architecture.md, `Docs/Complete_Reverse_Engineering_Reference_V2.md` §7, external: Complete_Reverse_Engineering_Reference_V1.md §7.
- `Database documentation.md`, `Documenting the Executable File.md` (root).
- Modern implementation: `src/RefLab.Infrastructure/Reports/QuestPdfExporter.cs`, `QuestPdfReportRenderer.cs`, `ReportBranding.cs`, `PrintSequenceGenerator.cs`; `src/RefLab.Application/Features/Reporting/*`; `src/RefLab.Application/Features/Results/PrintResultReport.cs`.
