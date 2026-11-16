# Call Graph — what can actually be reconstructed

**Scope rule.** This document separates **OBSERVED** edges (present in a dump as bytes, imports, or xref rows) from **INFERRED** flow (suggested by names, SQL strings or architecture). No complete application call graph is asserted: none can be produced from this evidence. All paths under `D:\Collective work\`.

## 1. OBSERVED: main-executable startup chain (VERIFIED bytes)

```
PE entry RVA 0x7142c  (real lab system.exe (2).json → pe.entry_point_rva; vb6-structure.json)
   │  bytes: 68 80 8E 4B 00   E8 EE FF FF FF   (vb6-structure.json → entry_stub_hex)
   ▼
push 0x004B8E80        ; address of VB project header
call ThunRTMain        ; MSVBVM60.DLL import (function_inventory.csv shows ThunRTMain as a 6-byte thunk)
   ▼
signature "VB5!" at pushed address   (vb6-structure.json → vb_header.signature)
   ▼
VB runtime reads ProjectInfo → ObjectTable (212 slots / 210 compiled) → 8,026 method entries
   (vb6-structure.json → object_table; vb6-objects.json → summary.total_methods 8026)
```
This is the **only** main-executable control-flow sequence directly evidenced, and it is a runtime-initialisation chain, not application logic. It is also observed in the 1.00 baseline (`vb6-chain-analysis.txt`: entry `0x4680c`, `push 0x004570f4`, `VB5!`) and in the auxiliary `NOTEX.dll` (`RL_CD2019Access__bin__NOTEX.xrefs.txt` line with `entry → DATA → s_VB5!…`).

## 2. OBSERVED: import-level dispatch edges (VERIFIED import table)

```
every application call to Win32  ──late-bound──▶  DllFunctionCall (MSVBVM60.DLL import)
event sink callbacks             ──────────────▶  EVENT_SINK_Invoke / GetIDsOfNames / AddRef / Release / QueryInterface
variant/string/array operations  ──────────────▶  __vbaVarMove, __vbaStrCat, __vbaRedim, __vbaForEachCollObj, …
```
The main executable imports **only** `MSVBVM60.DLL` (231 functions in 1.605, 186 in 1.00 — `real lab system.exe (2).json`, `real lab system.exe.json`, `pe-structure.json`). Win32 functions are therefore not direct import edges; they are reached at runtime through the VB runtime's dispatcher. CONFLICTED note for one field: `pe-structure.json → entry_point_rva 39649281` disagrees with two other dumps; see `Binary_Architecture.md §1`.

## 3. OBSERVED: xref edges in auxiliary components (VERIFIED xref rows)

Format `from → to : type`, from `RL_CD2019Access__*__.xrefs.txt`:

| Component | Observed edge example | Type | Interpretation |
|---|---|---|---|
| `NOTEX.dll` | `0040408c → 00401330 (DllFunctionCall)` | `DATA` + `COMPUTED_CALL` | native function loads thunk pointer and calls through it |
| `NOTEX.dll` | `004012ac → 004010bc (PTR___vbaFPException)` | `INDIRECTION` | import thunk indirection |
| `NOTEX.dll` | `004012ac → EXTERNAL:00000030` | `COMPUTED_JUMP` | jump into runtime import |
| `NOTEX.dll` | `00401468 (entry) → 00402a74 (s_VB5!…)` | `DATA` | project header reference |
| `NOTEX.dll` | `00401468 (entry) → 00401462 (Ordinal_100)` | `UNCONDITIONAL_CALL` | startup calls runtime ordinal |
| `NOTEX.dll` | `0040408c → 0040934c` | `READ` | data access inside native function |
| `mswless.ocx` (18,239 xref lines), `unrar.dll` (9,139), `sp698vbo.dll` (5,066), `msscript.ocx` (2,926), `selfreg.dll` (2,084), `dispex.dll` (848) | same `FUN_* → DllFunctionCall / __vba* / ordinal` patterns | various | these are third-party/helper components, **not** RealLab business logic |

These xrefs are real observations but describe **auxiliary VB6 DLLs/OCXs**, not the 76 MB application. They demonstrate the runtime-mediation pattern in miniature; they cannot be lifted onto the main executable's objects.

## 4. INFERRED: the application-level flow (NOT observed)

The following layered flow is a **reasonable reconstruction from structure and strings only**. Each arrow is INFERRED; confidence per layer is given.

```
[VB6 Form object, e.g. Result* / addpatient / patientmain]        STRONGLY_SUPPORTED (object exists, has methods)
        │  user event handlers (Click/Change/Validate) — names NOT recovered
        ▼
[data-access modules: labdb (24), labdb1 (20), labfunction (25)]  STRONGLY_SUPPORTED (modules exist; SQL strings in .text)
        │  ADO DataShape / Jet or SQLOLEDB connection
        ▼
[SQL / SHAPE statements in literal pool, e.g. VA 0x681ED2]        VERIFIED as strings; issuer object UNVERIFIED (UNR-008)
        │
        ▼
[MSVBVM60.DLL runtime: __vba* helpers, DllFunctionCall]           VERIFIED import; specific call sites UNVERIFIED
        ▼
[Jet OLEDB 4.0 .mdb  OR  SQLOLEDB.1 / MSDataShape.1 SQL Server]   STRONGLY_SUPPORTED (connection templates, UDL files)
        ▼
[Report* DataReport objects (66, mostly methods=0) + printerselect module + Bullzip PDF printer]  STRONGLY_SUPPORTED / INFERRED binding
```
Evidence for each inferred layer:
- Forms and modules: `objects-full__[…v1605].txt` object list with method counts; `vb6-objects.json`.
- SQL strings: `statements__[analysis_BATCH-04_sql_v1605].csv` (1,797 rows) with VAs such as `66E8FE`, `672E3E`, `681ED2`, `69EA76`; verb distribution SELECT 1,381 / ALTER 241 / DELETE FROM 72 / OTHER 49 / CREATE 27 / SHAPE 27 (denominator 1,797).
- Connection templates: `primary-executable-reverse-engineering.md §6.1–6.2` (SQLOLEDB.1, MSDataShape.1, Jet OLEDB 4.0) and `data_connection.udl` / `data_connection1.udl` rows in `v1.605-package-manifest.csv`.
- Report objects and templates: `primary-executable-reverse-engineering.md §4`; `primary-executable-technical-reference.md §6`.
- The exact event-handler → SQL mapping is explicitly an **open question (UNR-008)** in `primary-executable-reverse-engineering.md §9.2, §10`.

## 5. What cannot be reconstructed (UNVERIFIED / UNKNOWN)

| Missing edge | Why | Evidence of the gap |
|---|---|---|
| Form/method → SQL statement | Method names not recovered; Ghidra incomplete | `vb6-procedures-summary.txt` (0 procedures); `primary-executable-reverse-engineering.md §10` UNR-008 |
| Method → method calls inside the 8,026 methods | Only 1,268 of 8,026 method entries readable; no method names | `objects-full` header; `Class_And_Method_Catalog.md §1` |
| Application call graph from native disassembly | 6,515/6,522 native functions are `FUN_*` placeholders (1.00 baseline); no 1.605 function inventory | `function_inventory.csv`; `Native_Code_Analysis.md §1, §5` |
| Report rendering chain (UNR-010) | Ghidra did not complete | `primary-executable-reverse-engineering.md §9.2` |
| Win32 API set called via `DllFunctionCall` by the target | No direct imports; only inferred from strings for the baseline | `primary-executable-reverse-engineering.md §2.3`; `Version_Identity_and_Scope.md §2.5` |
| Runtime behaviour (call order, dynamic dispatch) | Static-only investigation, nothing executed | `Version_Identity_and_Scope.md §5.1` |

## 6. Conclusion

The reconstructable "call graph" is three small, verified primitives — the VB6 startup chain, the single-DLL import/dispatch surface, and auxiliary-DLL xrefs — plus one clearly-labelled INFERRED layered application flow. Any diagram showing named VB6 methods calling each other would be fabricated from object names and is deliberately not provided.
