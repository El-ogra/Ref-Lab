# Native Code Analysis — function inventory, imports, xrefs, and limits

**Caveat first.** The native function inventory available here (`function_inventory.csv`) is for the **1.00 baseline** (`program = real lab system.exe`, `executable_sha256 = 12828d0a…`). There is **no completed Ghidra function inventory or decompilation for the 1.605 target** in this evidence set. `primary-executable-reverse-engineering.md §9` records that the 1.605 Ghidra headless run did not complete within the session and that UNR-008 (statement-to-caller mapping) and UNR-010 (report call chain) remain open; `primary-executable-technical-reference.md §7` states the conclusion plainly: "Ghidra produced no usable result for this binary." Native analysis therefore establishes envelopes, imports, stubs and auxiliary-component xrefs, but not the application's function-level semantics.

## 1. Function inventory statistics (VERIFIED — baseline 1.00 only)

Source: `function_inventory.csv` (6,522 data rows; columns `program,executable_sha256,md5,image_base,function_index,entry_point,body_min,body_max,body_address_count,body_byte_length,name,category,is_thunk,is_external,symbol_source,signature`).

| Measure | Value | Denominator |
|---|---:|---|
| Functions | 6,522 | 6,522 rows, all `program = real lab system.exe` |
| Category `auto-generated` | 6,516 | / 6,522 |
| Category `thunk` | 6 | / 6,522 |
| `symbol_source DEFAULT` | 6,521 | / 6,522 |
| `symbol_source IMPORTED` | 1 | / 6,522 (the `entry` function) |
| `is_external = false` | 6,522 | / 6,522 |
| `is_thunk = true` | 6 | / 6,522 |
| Names starting `FUN_` | 6,515 | / 6,522 |
| Named functions (non-`FUN_`) | 7 | / 6,522: `__vbaChkstk`, `adj_fdiv_m32`, `adj_fdiv_m64`, `adj_fdivr_m64`, `DllFunctionCall`, `ThunRTMain`, `entry` |
| Body byte length | min 6, max 440,459, mean 6,113.5 | over 6,522 functions |

`ghidra-main-funcs.txt` is the plain-text form of the same head (6 thunks + `entry` + `FUN_005b61a4`, `FUN_005b8e24`, …). The 6 thunks are the VB6 runtime trampolines at `0x4463b0`–`0x446806`; the `entry` at `0x4680c` is 40 bytes.

**Symbol sources.** `DEFAULT` is Ghidra's own naming fallback; `IMPORTED` was assigned only to `entry`. No PDB, no debug directory (`pe-structure.json → debug rva 0`, `common_pe.json` equivalent absent), no COFF symbols (`symbol_table_count 0`). There is no source-level symbol information anywhere in the image.

**What this means:** the native function list is dominated by anonymous stubs whose bodies forward into `MSVBVM60` imports. It cannot be read as a list of application procedures. See `Binary_Architecture.md §5`.

## 2. Import-table analysis (VERIFIED for both main executables)

- 1.605: **1 DLL, 231 functions** (`real lab system.exe (2).json → pe.import_dll_count:1, import_function_count:231`; `pe-structure.json → imports[0]`). 1.00: **1 DLL, 186 functions** (`real lab system.exe.json`).
- Functional families (full arrays in the two JSON files): variant math/string helpers (`__vbaVarMove`, `__vbaVarSub`, `__vbaStrCat`, `__vbaInStr`, `__vbaVarCmpEq`, `__vbaBoolVar`), object/late binding (`__vbaObjSet`, `__vbaLateIdCall`, `__vbaLateMemCall`, `DllFunctionCall`), collections/arrays (`__vbaAryVar`, `__vbaRedim`, `__vbaForEachCollObj`, `__vbaVarForInit`), error/exception (`__vbaOnError`, `__vbaExceptHandler`, `__vbaHresultCheckObj`), event sinks (`EVENT_SINK_*`), floating-point adjustment stubs (`_adj_fdiv_*`, `_CI*`), and raw ordinals (`ord:100`, `ord:516`, …).
- 1.605-only additions (compared element-by-element between the two import arrays): `__vbaFileOpen`, `__vbaFileClose`, `__vbaPrintFile`, `__vbaLineInputVar`, `__vbaNameFile`, `__vbaErase`, `__vbaRedimPreserve`, `__vbaOnGoCheck`, `__vbaStrDate`, `__vbaDateStr`, `__vbaDateR8`, `__vbaRefVarAry`, `__vbaVarSetVar`, `__vbaNextEachVar`, `__vbaForEachVar`, `__vbaForEachCollAd`, `__vbaAryConstruct2`, `__vbaVarLateMemCallLdRf`, `__vbaLateIdCallSt`, `__vbaR8IntI2`, `__vbaStrUI1`, `__vbaBoolStr`. These point to added file I/O, date handling and collection processing in the 1.605 build (STRONGLY_SUPPORTED — import presence is observed; which object calls them is UNVERIFIED).
- Because all Win32 access goes through `DllFunctionCall`, the import table **cannot** be used to prove which APIs the application calls (e.g. printer APIs are visible only in strings of the baseline build, `Version_Identity_and_Scope.md §2.5`). IMPORTS = VERIFIED; inferred API usage beyond the runtime = UNVERIFIED.
- IAT data directory is 928 bytes in 1.605 (`pe-structure.json → data_directories.iat.size 928`), consistent with ~231 imported slots plus bound-import entries.

## 3. Ghidra xref dumps — what exists (VERIFIED observations)

Xref dumps were produced for **auxiliary native components only**, not for the main executable. Counts are physical lines in each `*.xrefs.txt` (header line included):

| Component | Xref lines | Function lines | Xref dump file |
|---|---:|---:|---|
| `mswless.ocx` | 18,239 | 1,211 | `RL_CD2019Access__System__datareport_vh__mswless.xrefs.txt` |
| `unrar.dll` | 9,139 | 791 | `RL_CD2019Access__OCX__unrar.xrefs.txt` |
| `sp698vbo.dll` | 5,066 | 316 | `RL_CD2019Access__System__datareport_vh__sp698vbo.xrefs.txt` |
| `msscript.ocx` | 2,926 | 266 | `RL_CD2019Access__System__datareport_vh__msscript.xrefs.txt` |
| `selfreg.dll` | 2,084 | 119 | `RL_CD2019Access__System__datareport_vh__selfreg.xrefs.txt` |
| `NOTEX.dll` | 1,558 | 20 | `RL_CD2019Access__bin__NOTEX.xrefs.txt` |
| `dispex.dll` | 848 | 131 | `RL_CD2019Access__System__datareport_vh__dispex.xrefs.txt` |

Xref record format: `from<TAB>to<TAB>type<TAB>function`. Observed `type` values include `INDIRECTION`, `COMPUTED_JUMP`, `DATA`, `UNCONDITIONAL_CALL`, `READ`, `CONDITIONAL_JUMP`, `COMPUTED_CALL`.

Example from `RL_CD2019Access__bin__NOTEX.xrefs.txt` (lines 2–11, VERIFIED observations):
```
004012ac  004010bc        INDIRECTION       __vbaFPException  PTR___vbaFPException_004010bc
004012ac  EXTERNAL:00000030 COMPUTED_JUMP    __vbaFPException  __vbaFPException
00401330  00401084        INDIRECTION       DllFunctionCall   PTR_DllFunctionCall_00401084
00401330  EXTERNAL:00000022 COMPUTED_JUMP    DllFunctionCall   DllFunctionCall
00401468  00402a74        DATA              entry             s_VB5!6&*_00402a74
00401468  00401462        UNCONDITIONAL_CALL entry             Ordinal_100
0040408c  0040934c        READ              FUN_0040408c      DAT_0040934c
0040408c  00404097        CONDITIONAL_JUMP  FUN_0040408c      LAB_00404097
0040408c  00401330        DATA              FUN_0040408c      DllFunctionCall
0040408c  00401330        COMPUTED_CALL     FUN_0040408c      DllFunctionCall
```
This demonstrates the same architecture in a small auxiliary VB6 DLL: a `VB5!` data reference in `entry`, indirect thunks to `DllFunctionCall`/`__vbaFPException`/`Ordinal_100`, and native functions that call through those thunks. `RL_CD2019Access__bin__NOTEX.strings.txt` additionally contains `MSVBVM60.DLL`, `MCI.MMControl`, `XPbuttons.XPButton`, `MSComCtl2.DTPicker`, i.e. ActiveX control class names.

Function dumps for the same seven components are `RL_CD2019Access__*__.functions.txt` (counts in table above) plus `ghidra-funcs-NOTEX.dll.txt` and `ghidra-funcs-sp698vbo.dll.txt`.

## 4. What native analysis DID establish

- Exact PE envelope, sections, resources, hardening flags and import surface of both main executables (`pe-structure.json`, both `*.exe.json`).
- The VB6 entry stub and project-header chain, object-count/method-count register (`vb6-structure.json`, `vb6-objects.json`).
- A complete native function census for the **1.00** baseline: 6,522 functions, 6,515 of them anonymous `FUN_` stubs (`function_inventory.csv`).
- Observed call/data xrefs in seven auxiliary native DLLs/OCXs, showing runtime mediation (`DllFunctionCall`, `EVENT_SINK_*`, ordinals) and `VB5!` project headers.
- Version resources and file identities of the native runtime/ActiveX components (`version-resources.json`).

## 5. What native analysis did NOT establish

| Question | Status | Evidence of the gap |
|---|---|---|
| Method names of the 8,026 VB6 methods | **not established** | `vb6-procedures-summary.txt` header "169 objects, 0 procedures"; `primary-executable-reverse-engineering.md §10` |
| Which object/method issues which SQL statement | **not established** (UNR-008) | `primary-executable-reverse-engineering.md §9.2, §10` |
| Report rendering call chain | **not established** (UNR-010) | same |
| Completed decompilation for 1.605 | **not established** | Ghidra run incomplete; no v1.605 function CSV exists |
| Runtime behaviour of any component | **not established** | static-only investigation, `Version_Identity_and_Scope.md §5.1` |
| 25 external component names from header descriptors | **not established** (UNR-009) | `primary-executable-reverse-engineering.md §5, §10` |
| Semantics of `grant1`–`grant17`, `IDPasswordL/T/F/B/S/V/A` | **not established** (UNR-002, UNR-003) | `primary-executable-reverse-engineering.md §10` |
| Main-executable call graph | **not established** | no main-exe xref dump exists |

## 6. Limitations (UNVERIFIED / UNKNOWN)

- Function inventory is limited to the older, smaller baseline; any function count quoted for 1.605 would be fabrication.
- Native function names are placeholders, not symbols; body-size statistics measure stub layout, not procedure complexity.
- Xref dumps are for auxiliary components; they cannot be projected onto the main application's control flow.
- The 981-line `RL_CD2019Access__OCX__unrar.functions.txt` and similar dumps describe third-party libraries (unrar, mswless, msscript, dispex, selfreg, sp698vbo), not RealLab business logic.
- `setupsp6.exe` could not be parsed as PE in `pe-analysis.json` (`pe_error: "no PE signature"`) — its true envelope is UNVERIFIED.
- All analysis is static; nothing was executed (no sandbox run, no runtime instrumentation).
