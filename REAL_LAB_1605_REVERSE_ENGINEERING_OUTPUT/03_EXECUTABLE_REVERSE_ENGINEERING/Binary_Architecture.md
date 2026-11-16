# Binary Architecture — PE headers, runtime, and the VB6 p-code interpretation model

**Evidence basis.** `real lab system.exe (2).json` (1.605 PE dump), `real lab system.exe.json` (1.00 PE dump), `pe-structure.json`, `pe-resources.json`, `pe-summary.csv`, `vb6-structure.json`, `vb6-objects.json`, `function_inventory.csv`, `ghidra-main-funcs.txt`, `primary-executable-reverse-engineering.md`, `primary-executable-technical-reference.md`, `Version_Identity_and_Scope.md`. All paths under `D:\Collective work\`. **The 1.605 binary itself is not on disk; nothing was executed.**

## 1. PE headers (VERIFIED)

| Field | 1.605 (target) | 1.00 (baseline) | Evidence field |
|---|---|---|---|
| Format | PE32, `optional_magic 0x010b`, machine `0x14c` / `IMAGE_FILE_MACHINE_I386` — **32-bit** | same | `pe-structure.json`, both `*.exe.json` |
| Size | 76,292,096 B | 59,150,336 B | `real lab system.exe (2).json → size_bytes`, `real lab system.exe.json → size_bytes` |
| SHA-256 | `C54CE6C8…93C0` | `12828D0A…FD885` | same files |
| `TimeDateStamp` | `1595435962` → **2020-07-22 16:39:22 Z** | `1388267701` → **2013-12-28 21:55:01 Z** | `pe-structure.json → timestamp`, `real lab system.exe.json → pe.timestamp` |
| Characteristics | `0x010f`; `is_exe true`, `is_dll false` | `0x010f` | `pe-structure.json`, dumps |
| Linker / OS version | `6.0` / `4.0` | `6.0` / `4.0` | dumps `→ pe.linker_version / os_version` |
| Image base | `0x00400000` | `0x00400000` | `pe-structure.json`, dumps |
| Entry point RVA | `0x7142c` (`pe-structure.json` also reports decimal `39649281` = `0x025C0001` for a different field — see caveat below) | `0x4680c` | `real lab system.exe (2).json → pe.entry_point_rva` |
| Size of image | 76,595,200 | 59,412,480 | dumps |
| Size of headers | 4,096 | 4,096 | dumps |
| Subsystem | `IMAGE_SUBSYSTEM_WINDOWS_GUI` | same | dumps |
| `DllCharacteristics` | `0x0000` — no ASLR, no DEP/NX, no CFG; SEH enabled | `0x0000` | `pe-structure.json → dll_characteristics/aslr/dep_nx/cfg/seh` |
| Authenticode | absent (Security directory size 0) | absent | `pe-structure.json → authenticode_present:false` |
| Overlay | `overlay_offset 76292096`, `overlay_bytes 0` | `overlay_offset 59150336`, `overlay_bytes 0` | dumps |
| Data directories | 16; import RVA `0x744...` (`76119300`), resource RVA `76431360` size `161682`, IAT size `928` | — | `pe-structure.json → data_directories` |
| Symbol table | `symbol_table_ptr 0`, `symbol_table_count 0` (no COFF symbols) | — | `pe-structure.json` |

**Timestamp conflict (CONFLICTED).** `Version_Identity_and_Scope.md §3.1` states both executables carry PE timestamp **2013-12-27** and instructs "Do not use the PE timestamp as a version discriminator." The raw dumps contradict this for the target: `pe-structure.json → timestamp = 1595435962` (2020-07-22) and the baseline `1388267701` (2013-12-28). The raw parsed value is authoritative for the binary's own header; the document's claim is a lead, not evidence. Either way, the timestamp must **not** be treated as a reliable release/build date: it is a single DWORD that can be set by the build environment, and the installer (`RL_SETUP.exe`, reported build 2020-09-01 in `Version_Identity_and_Scope.md §3.1`) and payload dates are the stronger chronology indicators. Both binaries share linker `6.0` (VB6), which is a toolchain marker, not a date.

**Note on `pe-structure.json → entry_point_rva 39649281`.** This value (`0x025C0001`) disagrees with `real lab system.exe (2).json → pe.entry_point_rva 0x7142c` and with `vb6-structure.json → entry_point_rva 463916 (0x7142c)` and with the byte-level entry stub analyzed in `primary-executable-reverse-engineering.md §2.1`. Two of three independent dumps agree on `0x7142c`; the `39649281` figure is treated as a parsing artifact and marked UNVERIFIED for that field.

## 2. Sections (VERIFIED)

From `real lab system.exe (2).json → pe.sections` and `pe-structure.json → sections`:

| Section | Virtual size | Virtual address | Raw size | Characteristics | Contents |
|---|---:|---|---:|---|---|
| `.text` | 76,119,116 | `0x1000` | 76,120,064 | `0x60000020` R/X, code — **≈97% of the 76,292,096-byte file** | All native code, compiled form data, string-literal pool |
| `.data` | 304,852 | `0x4899000` | 4,096 | `0xc0000040` R/W, has relocs | Almost entirely uninitialised (BSS-like); raw size far below virtual size |
| `.rsrc` | 161,682 | `0x48e4000` | 163,840 | `0x40000040` R, has relocs | 10 ICON + 1 GROUP_ICON + 1 VERSION (12 entries; `pe-resources.json → by_type {ICON:10, GROUP_ICON:1, VERSION:1}`) |

Baseline 1.00 (`real lab system.exe.json`): `.text` virtual 58,976,996 / raw 58,978,304 at `0x1000`; `.data` virtual 265,340 / raw 4,096 at `0x3840000`; `.rsrc` virtual 161,638 / raw 163,840 at `0x3881000`; same 12-resource shape.

The string-literal pool is contiguous inside `.text`. **String adjacency carries no semantic meaning** — flagged in `primary-executable-reverse-engineering.md §3`, so no conclusion here rests on proximity of two literals.

## 3. Imports and runtime (VERIFIED)

- **Exactly one imported DLL: `MSVBVM60.DLL`** — 231 functions in 1.605 (`real lab system.exe (2).json → pe.import_dll_count:1, import_function_count:231`; `pe-structure.json → imports[0].count:231`), 186 in 1.00 (`real lab system.exe.json → import_function_count:186`). No `KERNEL32`, `USER32`, `OLEAUT32`, or `ADVAPI32` import descriptor exists in either main executable.
- Import families: VB runtime helpers `__vbaVarMove`, `__vbaVarSub`, `__vbaStrCat`, `__vbaInStr`, `__vbaRedim`, `__vbaVarForInit`, `__vbaOnError`, `__vbaExceptHandler`, `__vbaHresultCheckObj`; event sinks `EVENT_SINK_AddRef/Release/QueryInterface/Invoke/GetIDsOfNames`; math helpers `_CIcos`, `_CIsin`, `_CIsqrt`, `_adj_fdiv_*`; late-bound dispatcher `DllFunctionCall`; many unresolved `ord:###` / `ordinal####` imports (e.g. `ord:696`, `ord:516`, `ord:100`). Full lists in `real lab system.exe (2).json → pe.imports.MSVBVM60.DLL` and `pe-structure.json`.
- Consequence: **every Win32 call is routed through the VB runtime's late-bound `DllFunctionCall`**, which is why the import table is a single DLL and why static import analysis cannot enumerate the application's actual Win32 API surface (`primary-executable-reverse-engineering.md §2.3`).
- 1.605 adds file-I/O helpers absent from the 1.00 import list: `__vbaFileOpen`, `__vbaFileClose`, `__vbaPrintFile`, `__vbaLineInputVar`, `__vbaNameFile`, `__vbaErase`, `__vbaRedimPreserve`, `__vbaOnGoCheck`, `__vbaStrDate`, `__vbaDateStr`, `__vbaDateR8` (compare the two `pe.imports.MSVBVM60.DLL` arrays). This is a direct, observed import-level difference between target and baseline.

## 4. Entry points and VB6 project header (VERIFIED)

- 1.605 entry stub bytes `68 80 8E 4B 00 E8 EE FF FF FF 00…` = `push 0x004B8E80; call ThunRTMain` (`vb6-structure.json → entry_stub_hex`; `primary-executable-reverse-engineering.md §2.1`). 1.00 stub `68 f4 70 45 00 e8 f0 ff ff ff 00` (`vb6-chain-analysis.txt`; that file is a v1.00 dump).
- At the pushed address sits signature `VB5!` (`vb6-structure.json → vb_header.signature`; `primary-executable-reverse-engineering.md §2.2`).
- Header fields from `vb6-structure.json → vb_header`: `runtime_build 9782` (VB6 **Service Pack 6**), `runtime_revision 10`, `lcid 1033`, secondary LCID `4105` (ar-SA, reported by `primary-executable-reverse-engineering.md §2.2`), `project_data_va 4970060`, `form_count 198`, `external_component_count 25`, `gui_table_va 5711188`, project GUID `6c1467afa02291459ed352beb9d44d32` (`vb6-structure.json → object_table.project_uuid`).
- Project name `Real_lab_system`, description/exe name `real lab system` (`primary-executable-reverse-engineering.md §2.2`; `vb6-object-array.txt` for the baseline).
- Object table (`vb6-structure.json → object_table`): `object_count 212`, `compiled_objects 210`, `objects_in_use 3052`, `first_object 102`, `compile_type 0xd2000a`. Code span `code_start_va 28578400` to `code_end_va 80313600` (`→ project_info`).
- The `form_count 198` header field is a **count of form/registration entries in the project header**, not the same measure as the 132 forms in the recovered object table (`objects-full__[analysis_BATCH-04_vb6_v1605].txt`) or the 131 forms whose data blobs were recovered (`forms-blob__[analysis_BATCH-04_vb6_v1605].txt`). These are different instruments; see `Reconstructed_Code_Structure.md`.

## 5. Why native Ghidra decompilation yields mostly thunks and `FUN_` names (STRONGLY_SUPPORTED)

Measured, not assumed, on the **baseline** (`function_inventory.csv`, whose `executable_sha256` is `12828d0a…` = the 1.00 binary; this is the only completed Ghidra function inventory in the evidence set):

- **6,522 functions total.**
- Categories: `auto-generated` **6,516 / 6,522**, `thunk` **6 / 6,522**.
- Symbol sources: `DEFAULT` **6,521 / 6,522**, `IMPORTED` **1 / 6,522**.
- `is_external` is `false` for **6,522 / 6,522** rows; `is_thunk` true for 6.
- Names: **6,515 / 6,522** begin `FUN_` (e.g. `FUN_005b61a4`). The only 7 named functions are `__vbaChkstk`, `adj_fdiv_m32`, `adj_fdiv_m64`, `adj_fdivr_m64`, `DllFunctionCall`, `ThunRTMain`, `entry` — i.e. VB6 runtime thunks and the startup stub. `ghidra-main-funcs.txt` reproduces the same head of the list.
- Body sizes range 6–440,459 bytes, mean 6,113.5 bytes (denominator 6,522).

**Interpretation.** VB6 "native" compilation emits short stubs that push arguments and call `MSVBVM60` helpers; the application's real logic is stored as **VB6 project structures** — object descriptors, method tables, form/control blobs, and literal/string pools — which the runtime interprets at load and dispatch time via its own internal representations (commonly described as p-code/structured method data). A native disassembler sees the stub layer and the runtime imports, not the named VB procedures. This is corroborated by:

1. The single-DLL import surface (§3).
2. The 212-object / 8,026-method table recovered by walking the VB header directly (`vb6-objects.json → summary.total_methods 8026`), while Ghidra recovered only 6,522 mostly anonymous native functions for the older, smaller binary.
3. `vb6-procedures-summary.txt` header "VB6 PROCEDURE-NAME EXTRACTION – 169 objects, 0 procedures" — per-object **method names were not recoverable** from the native image.
4. `primary-executable-reverse-engineering.md §9.3` and `primary-executable-technical-reference.md §7`: the Ghidra headless run did not complete within the session; UNR-008 (statement-to-caller mapping) and UNR-010 (report call chain) remain open, and the document's own conclusion is "Ghidra produced no usable result for this binary."

Therefore the authoritative architecture of this target is the **VB6 structural model**, not a decompiled C-like call graph. Native analysis can establish envelopes, imports, stubs and xrefs of auxiliary DLLs; it cannot currently enumerate the application's methods or call graph.

## 6. Hardening and resource notes (VERIFIED)

- No ASLR, no DEP/NX, no CFG, SEH present, no Authenticode (`pe-structure.json`). No manifest resource in either executable (`real lab system.exe (2).json → pe.resource_types` lists only `RT_GROUP_ICON`, `RT_ICON`, `RT_VERSION`), so no declared execution level, DPI awareness or common-controls v6.
- The largest icon (resource `30006`, 69,012 B, magic `89504e47…` = PNG) is a 256×256 PNG-shaped icon; its byte length matches the standalone `icon-embedded.png` (69,012 B) in the work directory. `primary-executable-reverse-engineering.md §7` also reports embedded Adobe Photoshop CS3 XMP metadata in the string corpus.
- No debug entries (`real lab system.exe (2).json → pe.debug_entries: []`) and no export table (`pe-structure.json → exports: []`).

## 7. Confidence summary

| Claim | Label |
|---|---|
| PE32 / i386 / GUI / image base / sections / imports / resources of both executables | VERIFIED (dedicated JSON dumps) |
| VB6 runtime identification, entry stub, header fields, project GUID | VERIFIED (`vb6-structure.json`, entry-stub bytes, reports) |
| 2020-07-22 timestamp for 1.605 and rejection of the "2013-12-27 for both" lead | VERIFIED on raw field + CONFLICTED with the lead document |
| `entry_point_rva 39649281` in `pe-structure.json` being a parsing artifact | UNVERIFIED (2-of-3 dumps disagree with it) |
| Ghidra 6,522-function statistics and their meaning | VERIFIED for the 1.00 binary only |
| Interpretation that real logic lives in VB6 structures interpreted by `MSVBVM60` | STRONGLY_SUPPORTED (convergent evidence §5) |
| Runtime behavior of any kind | UNVERIFIED — nothing was executed |
