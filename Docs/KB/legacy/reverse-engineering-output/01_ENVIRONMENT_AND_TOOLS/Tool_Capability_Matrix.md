# Tool Capability Matrix

Capability requirements are stated first (MASTER DIRECTIVE §4.3); the actual tool selected follows from verified availability, not from a preferred product.

| Investigation activity | Required capability | Available tool(s) | Selected for this mission | Rationale / limit |
|---|---|---|---|---|
| PE executable inspection | PE headers, sections, imports, exports, version resources | PowerShell `VersionInfo`; JSON PE dumps; REA `inspect_pe_resources`; Ghidra headless; REA `open_binary`/`binary_overview` | PowerShell + primary JSON PE dumps; REA session check | The two authoritative PE dumps for v1.00 and v1.605 already exist in the evidence repository; re-parsing them adds traceability without touching originals. |
| Managed .NET analysis | CLI metadata, IL, decompilation | REA `inspect_managed_artifact`, `inspect_managed_members` (available) | None (NOT APPLICABLE) | All target components are VB6 native PE32; the only .NET artifact is the `dotNetFx40_Full_x86_x64.exe` redistributable installer. |
| Native binary analysis | Disassembly, CFG, decompilation, xrefs | Ghidra 12.1.4 headless; REA Ghidra provider (blocked); existing Ghidra output dumps (`function_inventory.csv`, `ghidra-*.txt`, `RL_CD2019Access__*.txt`) | Existing Ghidra dumps + direct headless probe | Native analysis of a VB6 program recovers thunks/imports/runtime scaffolding; most business logic lives in VB6 p-code structures. Existing dumps are primary evidence from the actual v1.605 binary. |
| Dependency discovery | Assembly/module/import enumeration | `pe-analysis.json`, `pe-metadata.json`, `dependency-inventory.json`, `*.ocx.json`, `control-typelib-surface.json`, REA | Evidence dumps + PowerShell | Dependency set is fully enumerated in the repository. |
| Database investigation | Schema, constraints, relationships | `v2019-Patients.mdb.sql`, `schema-*.sql`, DDL evidence, table/column CSVs; `sqlcmd` | DDL/CSV parsing | No `.mdb` file and no attached target database exist locally. |
| SQL analysis | Query parsing, dependency extraction | `statements__[analysis_BATCH-04_sql_v1605].csv` (1,797 statements), `tables__...csv`, `columns__...csv` | PowerShell `Import-Csv` aggregation | Direct, reproducible, and bound to actual string literals in the executable. |
| Configuration analysis | INI/UDL/resource parsing | `ini-analysis.json`, `ini-key-delta.csv`, INI-related reports, `analyse_ini.py` | JSON/CSV + report cross-check | Secrets are never reproduced. |
| Runtime investigation | Debugger, tracer, process monitor | REA dynamic tools (Windows-limited), .NET debugger, process tools | BLOCKED for v1.605 (binary absent); static-only for all target behavior | See `07_RUNTIME_AND_BEHAVIORAL_ANALYSIS`. No runtime claim is made. |
| UI investigation | Screen/event-flow reconstruction | 132 VB6 forms in object dumps, RTF method sheets, Flash simulators, CHM, OCR inventory, UI label file | Object dumps + documentation cross-check | Reconstruction is structural; no live UI observation. |
| File analysis | Signature, hash, content typing | `Get-FileHash`, PE dumps, `file-analysis.json`, `asf-metadata.json`, Python analyzers | PowerShell + evidence JSON | 973 artifacts hashed; duplicates identified by hash. |
| Cross-source correlation | Evidence graph | Code Mode `execute` (parallel tool composition), grep/read, subagents | Used throughout | Every major finding cites an evidence file. |

## Coverage consequences of the matrix

- **COMPLETE:** artifact discovery, hashing, PE fingerprinting from dumps, database reconstruction from DDL/CSV, SQL statement inventory mining, configuration/dependency inventory, cross-source correlation, documentation.
- **PARTIAL:** native decompilation (Ghidra provider blocked; existing dumps used instead; VB6 p-code limits semantic recovery even with a working provider).
- **BLOCKED:** runtime/dynamic verification of the v1.605 executable — the binary is not in the evidence repository. No tool can analyze an absent artifact.
