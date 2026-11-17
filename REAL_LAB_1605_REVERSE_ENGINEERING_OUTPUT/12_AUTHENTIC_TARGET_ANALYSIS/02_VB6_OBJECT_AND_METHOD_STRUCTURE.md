# VB6 native structure — object names, method names and the failure that hid them

**Target:** `F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe`
**SHA-256:** `c54ce6c8c98a679efc5ab485bc4b313b452026d6a40f9bda36eca7ea18df93c0` · 76,292,096 B · PE32 / i386 / GUI
**Tools:** purpose-written read-only Python parsers (`scripts/vb6_deep.py`, `vb6_names.py`, `vb6_deep2.py`, `vb6_strpool.py`), REA 6.7.0 CLI (PE resources), an independent PE import parser (`scripts/pe_imports.py`).

---

## 1. PE facts (independently re-derived)

| Fact | Value | Source |
|---|---|---|
| Magic / machine | `0x10B` / 332 (i386) | PE header |
| Image base | `0x00400000` | PE header |
| Sections | 3 — `.text` (rva `0x1000`, vsize 76,119,116, raw 76,120,064), `.data` (rva `0x4899000`, 304,852), `.rsrc` (rva `0x48E4000`, 161,682) | PE header |
| Imported DLLs | **1** — `MSVBVM60.DLL`, **231** functions | import directory parser |
| v1.00 comparison | 186 `MSVBVM60` functions | same parser on the old build |
| Entry stub | `68 80 8E 4B 00 E8 EE FF FF FF` = `push 0x004B8E80 ; call ThunRTMain` | bytes at the entry point |
| PE timestamp | `1595435962` = 2020-07-22T16:39:22Z | optional header (all three timestamped directories) |
| Version | major 0 / minor **605** | optional header; `FileDescription 'RLS 1.605'` |
| Resources | 12 entries — 10 `RT_ICON` (IDs 30001–30010), 1 `RT_GROUP_ICON`, 1 `RT_VERSION` (code page 1200) | REA `inspect-pe-resources` (evidence `ev_28c1e4416381366e7486da0ae73785570ba114fafce21989a5487dc81e3b858d`) |

Because only `MSVBVM60.DLL` is imported, every other component (ADO, DAO, Jet, OCX, Win32 print API) is resolved at run time through `LoadLibrary`/`CreateObject` — which is why the control set had to be established from the shipped binaries rather than from an import table.

---

## 2. Why the prior pass recovered nothing

Running the prior extraction scripts against the **authentic** binary reproduces their failure exactly:

```
python vb6_project.py RLS_v1605.exe v1605_project.json
{
  "vb_header_file_offset": "0xb8e80",
  "project_name": null,
  "language_dll": "*",
  "form_count_header": 198,
  "object_count_header": 28578400,      ← nonsense
  "objects": []                          ← nothing enumerated
}
```

Two independent defects:

1. **Header discovery.** `vb6_project.py` finds the header by scanning the whole file for the first literal `VB5!`. In this image the scan lands on the *correct* offset (`0x0B8E80` happens to be right), but the script then reads `ProjectInfo` at `VBHeader+0x04` as the object array, `+0x08` as compile type and `+0x0C` as the object count, which yields `28,578,400`.
2. **The object-array offset.** The prior `vb6_parse.py` reads `aObjectArray` from `ObjectTable+0x38`, which is **zero** in the shipped image — hence `object_array_va: 0` and an empty object list. The correct field is `ObjectTable+0x30`.

---

## 3. Correct structure walk (VERIFIED)

| Structure | VA | Verified content |
|---|---|---|
| `VBHeader` | `0x004B8E80` | signature `VB5!`; runtime build **9782**; `wRuntimeRevision` 10; LCID 1033; `lpProjectData` (`+0x30`) = `0x004BD64C`; `+0x44` = 198; `+0x46` = 25; `+0x4C` = `0x00572554`; `+0x50` = `0x00521778` |
| `ProjectInfo` | `0x004BD64C` | version 500; `lpObjectTable` (`+0x04`) = `0x00660B98`; `lpCodeStart` (`+0x0C`) = `0x01B41260`; `lpCodeEnd` (`+0x10`) = `0x04C97D00`; `dwDataSize` (`+0x14`) = 304,844 |
| `ObjectTable` | `0x00660B98` | `dwObjectCount` (u16 `+0x2C`) = **212**; `dwCompiledObjects` (u16 `+0x2E`) = **210**; **`aObjectArray` (dword `+0x30`) = `0x00660BEC`** |
| `PublicObjectDescriptor` | `0x00660BEC + i·0x30` | `+0x18` = object name (ANSI pointer) · `+0x1C` = **method count** (dword) · `+0x20` = `aMethodNames` pointer array · `+0x00` = per-object `ObjectInfo` · `+0x08` = external-component block · `+0x28` = object-type flags |

**Discriminating evidence for the layout** (not assumed, but validated): the descriptor at `+0x1C` yields `labmain` = 71, `patientmain` = 84, `labdb` = 24, `frm_money` = 38 — matching the prior evidence record's per-object method counts exactly — and the Σ over 212 objects equals the prior figure **8,026** exactly. A wrong offset could not reproduce both.

---

## 4. Object inventory (210 of 212 named)

Machine-readable: `evidence/vb6-objects-and-method-names.json`.

First 20 in array order:

`labmain`, `labxxyyzz`, `patientmain`, `labdb`, `frm_money`, `tools`, `Resultliver`, `moneypaid`, `addpatient`, `Resulturine`, `Reporturine`, `Reportliver`, `tqrermog`, `ReportFatora`, `Reporttotal`, `Reporthistory`, `barcode`, `phone`, `testmain`, `Resultsugar`.

Further names include `labdb1`, `Reportworkp`, `workpaper`, `Reportwp`, `Resultstool`, `userspw`, `testrange`, `ReportSugar`, `Resultkidney`, `resultculutre`, `Resultlipids`, `reportculutre`, `Resultsemen`, `resultABO`, `resultwidal`, `resulthepatitis`, `ReportFatoraS`, `ResultESR`, `resultcbc`, `repotrwidal`, `Reporthistory2`, `ResultTORCH`, `resultcrp`, `resultpt`, `resulthormon`, `Reportinstrument`, `patient`, `Reportshareka`, `patientview`, `outtest`, `setup`, `anyreport`, `calctest`, `statisReport`, `Reportwpm`, `Reportwpt`, `Reportwpot`, `barcodeform`, `ResultInsulin`, `start`, `Resultbilharzias`, `Reportcross`, `Reportmony1`, `Reportsemen`, `Reportanyreport`, `BCWFonts`, `reportsetup`, `patienthistory`, `resultselectro`, `resultcardiac`, `resultTumors`, `resultVitamins`, `resultTrace`, `resultanytest`, `Resultpregnancy`, `pricelists`, `resultiron`.

Object-name pool location: VA `0x0066AECC` onwards — a contiguous ANSI pool holding `Real_lab_system`, then the object names in array order with per-object padding.

---

## 5. Method names and counts

| Measure | Value | Denominator |
|---|---:|---:|
| Σ declared methods | **8,026** | 212 objects |
| Prior claim "8,026 declared methods" | **VERIFIED** | — |
| Method names recovered | **444** | 8,026 → 5.53 % |
| Objects with ≥ 1 named method | 93 | 212 → 43.9 % |
| Prior recovery | **0** | 8,026 → 0 % |

Named methods recovered, grouped:

*Shared business procedures:* `calculateprice`, `printout`, `printouts`, `position`, `positions`, `stoptimer2`, `fill_title`
*Test-specific handlers:* `Hgb_test`, `pt_test`, `altratio_test`, `WIDAEL_test`, `BRUCELLA_test`, `esr_test`, `esr_tests`, `pt_tests`, `FBG_test`
*Designer lifecycle:* `DataReport_Initialize`
*Notification:* `SMSresultmsg`

`moneypaid` exposes exactly one named public member — `calculateprice` at index 11 of its `aMethodNames` array.

---

## 6. Negative result with a positive control — why the remaining 7,582 names cannot exist

| Probe | ASCII | UTF-16LE | UTF-16BE |
|---|---:|---:|---:|
| `Form_Load` | 0 | 0 | 0 |
| `_Click` | 0 | 0 | 0 |
| `cmdOK` | **2** | 0 | — |
| `Command1` | **35** | — | — |
| `Text1` | **54** | — | — |
| `Attribute VB_Name` | 0 | — | — |
| `End Sub` | 0 | — | — |
| `Private Sub ` | 0 | — | — |
| `Option Explicit` | 0 | — | — |

**Control names are present; procedure names are not.** The binary stores the ANSI names of form controls (`cmdOK`, `cmd_sendAll`, `cmd_tasfeer`, `cmd_moneyedit`, `Label26`, `DataGrid1`, `DTPicker2`, `txt_timeout`, `Txt_SecPW`, `cmd_save2`, `Frame_list`, `lbl_User_Enter_Time`, `RedsoulFrame1`, `Chk_serchPos`, `Opt_CG`, …) and a set of derived `IDPasswordL/T/F/B/S/V/A` names, but the compiler retained **no** event-handler or private-procedure names. The seven `IDPassword*` names are therefore **control names on the internal-ID/password form**, not separate credential columns.

**Consequence.** A per-method call graph is not merely "not yet recovered" — it is **not recoverable from this artifact**, because the artifact does not contain the names that would label its call graph. What *is* recoverable, and was recovered, is the complete address-level structure: every object's declared member count, its public-member name array, its control table with per-control event-handler code addresses, and every SQL literal's code site.

---

## 7. Per-object control tables (form → control → event-handler address)

`ObjectInfo+0x78` holds the form's control table as 16-byte records:

```
+0x00  aEventInfo      → control descriptor
+0x04  dwEventAddr     → code address of the control's event handler
+0x08  aControlName    → ANSI control name
+0x0C  dwControlFlags  → e.g. 0x0003003F, 0x00030075
```

Observed for `labmain`: `Image9` (flags `0x0003003F`), `Label34` (flags `0x00030075`), with event-handler addresses in the `0x001Axxxx`–`0x001Bxxxx` code region — the same region in which the SQL/DDL code sites were found. This is the mechanism by which a form's control can be tied to a code address even though its handler *name* is not stored.

---

## 8. External component table

`VBHeader+0x4C` → `0x00572554` and `+0x50` → `0x00521778` both resolve and contain four-GUID blocks plus pointers into the per-object external-component blocks (e.g. `labmain`'s block at `0x0066C620` contains the ANSI strings `kernel32` and `Sleep`, i.e. the Win32 API calls that object makes). The individual OCX/typ-library identities are established from the shipped binaries and their version resources rather than from this table — see `01_TARGET_TREE_INVENTORY.md` §4.5.

---

## 9. Ghidra 12.1.4 — four attempts, and the inventory that finally came out

| Attempt | Configuration | Result |
|---|---|---|
| 1 | `JAVA_HOME` set, `GHIDRA_HEADLESS_MAXMEM=5G`, full auto-analysis | **Exit `3221225725` = `0xC00000FD` STACK_OVERFLOW** during auto-analysis of the 76 MB VB6 image |
| 2 | `GHIDRA_HEADLESS_MAXMEM=4G`, `-Xss1024m -XX:+UseSerialGC`, pre-script `ConfigureAnalysis.java` intended to disable ten analyzers | **The pre-script FAILED TO COMPILE** — Ghidra 12.1.4 has no 2-argument `setAnalysisOption`, and `DumpVbInventory.java` failed on six further API mismatches. **No analyzer was disabled**, so this ran with the default profile. Auto-analysis **timed out at 7,200 s** (9,077 s of analyzer time), but the program was **still saved to the project** |
| 3 | `-process` the saved project, `-noanalysis`, corrected `DumpVbInventory2.java` | Script still failed to compile on two API errors (`Data` lives in `ghidra.program.model.listing`; `Function.getSymbolType()` does not exist) |
| 4 | `-process` the saved project, `-noanalysis`, corrected script | **SUCCESS** — the inventory was extracted from the already-analysed program without re-running auto-analysis |

**Attempt 2 produced the decisive diagnosis.** Because no analyzer was disabled, the recorded timings are the *default-profile* costs for this image:

| Analyzer | Seconds |
|---|---:|
| **x86 Constant Reference Analyzer** | **3,582.079** |
| **Stack** | **2,984.146** |
| Disassemble | 778.923 |
| Reference | 364.355 |
| Scalar Operand References | 305.082 |
| Data Reference | 165.956 |
| Function ID | 158.297 |
| Decompiler Switch Analysis | 122.702 |
| Non-Returning Functions - Discovered | 125.706 |
| Create Function | 98.815 |
| X86 Function Callee Purge | 109.159 |
| Create Address Tables (+ One Time) | 40.637 / 68.419 |
| **Total** | **9,077** |

The two dominant analyzers account for **6,566 s — 72 % of the total**. A run that disables exactly those two and raises `-analysisTimeoutPerFile` would very likely complete on this host; the corrected, API-correct `ConfigureAnalysis.java` is shipped in `scripts/` for that purpose.

### 9.1 The recovered native function inventory

| Measure | Value |
|---|---:|
| Memory blocks | `Headers` 0x00400000–0x00400FFF · `.text` 0x00401000–0x04C98FFF (76,120,064 B) · `.data` 0x04C99000–0x04CE36D3 (304,852 B) · `.rsrc` 0x04CE4000–0x04D0BFFF (163,840 B) · `tdb` 0xFFDFF000–0xFFDFFFFF |
| **Functions recovered** | **8,021** |
| — named `FUN_<hex>` (no symbols in the image) | 8,014 |
| — MSVBVM60 runtime imports | 7 (`__vbaChkstk`, `adj_fdiv_m32`, `adj_fdiv_m64`, `adj_fdivr_m64`, `DllFunctionCall`, `ThunRTMain`, `entry`) |
| Defined data items | 35,063 |
| **SQL/DDL code sites inside a function** | **3,566 / 3,718 (95.9 %)** |

**8,021 recovered functions against 8,026 VB6-declared methods** — a 5-difference across two entirely independent derivations (the VB6 object table and the disassembler). That is strong mutual confirmation of both the method census and the function census.

Evidence: `evidence/ghidra-functions.tsv`, `evidence/ghidra-blocks.tsv`, `evidence/ghidra-strings.tsv`, `evidence/ghidra-sql-site-to-function.tsv`, `evidence/ghidra-string-xrefs.tsv`.

### 9.2 Reproducing this without re-running auto-analysis

The Ghidra project survives in the working area and can be re-read with **no analysis at all**, which is what makes this cheap and repeatable:

```powershell
$env:JAVA_HOME = "C:\Program Files\Microsoft\jdk-21.0.13.21-hotspot"
$env:GHIDRA_HEADLESS_MAXMEM = "4G"
& "C:\Tools\ghidra_12.1.4_PUBLIC\support\analyzeHeadless.bat" `
    "<projectDir>" "RLS1605" -process RLS_v1605.exe -noanalysis `
    -scriptPath "<scriptDir>" -postScript DumpVbInventory2.java "<outDir>" "<codeSites.txt>"
```

`scripts/DumpVbInventory2.java` emits `blocks.tsv`, `functions.tsv`, `strings.tsv`, `func_refs.tsv`, `site_to_function.tsv` and `string_xrefs.tsv`; `scripts/sql_by_function.py` distils the 547 MB `func_refs.tsv` into `sql_by_function.tsv` (function → its SQL statements) and `sql_by_function.tsv.nonsql.tsv` (function → its other literals).

For a **full** analysis on a larger machine, run `scripts/ConfigureAnalysis.java` as a `-preScript` — it disables *x86 Constant Reference Analyzer* and *Stack* (72 % of the default-profile cost) and the other heavy analyzers, with the corrected 12.1.4 `setAnalysisOption(Program, String, String)` signature.
