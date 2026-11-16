# Tool Usage Log

This is the operational record of tools actually used in the current investigation. It is distinct from the capability inventory.

## Commands and operations executed

| # | Tool / command | Purpose | Result | Output location |
|---|---|---|---|---|
| 1 | PowerShell `Get-ChildItem -Recurse` on `D:\Collective work` | Discover evidence files | 973 files, 0 directories | — |
| 2 | PowerShell `Get-FileHash -Algorithm SHA256` over all 973 files (background job `sh_1e52cb6d1001rtge6P4ILEFLq3`) | Integrity manifest + duplicate detection | 973/973 hashed; 22 duplicate groups (58 files) | `02_SOURCE_INVENTORY\Complete_Artifact_Inventory.csv`, `SHA256_Manifest.csv` |
| 3 | PowerShell `VersionInfo` over all 37 `.exe` files | Fingerprint executables, separate target components from third-party tools | v1.00 main exe identified; 13 Flash Player 8 projectors; remote-access tools flagged | `Tool_Execution_Log.md` |
| 4 | PowerShell version probes (node, npm, python, dotnet, java, rea, sqlcmd, git, psql, tshark) | Independent tool verification | Most OK; `psql`/`tshark` not on PATH (psql works by absolute path) | `Tool_Execution_Log.md` |
| 5 | REA `binary_session({})` via Code Mode `execute` | Verify MCP server and provider availability | Server live, 207 tools; Ghidra provider `runtime_unavailable` (Java fault) | `Environment_Verification.md` |
| 6 | .NET `ZipFile.OpenRead` over all 8 ZIPs | Archive inventory, nested-archive scan | 4,395 entries; 0 nested archives | `Archive_Extraction_Report.md` |
| 7 | 16-byte magic-byte scan of all 973 files | Detect mislabeled artifacts | 0 real mismatches | `Archive_Extraction_Report.md` |
| 8 | PowerShell `Import-Csv` + regex over 1,797-row SQL statement corpus | Recover verb distribution, result flags, DELETE cascade, write-path finding | 0 INSERT/0 UPDATE; 72 DELETE across 64 tables | `07_...\State_Transitions.md` |
| 9 | Ghidra 12.1.4 headless with `JAVA_HOME` set in shell (background job `sh_1e5304657001ZOvhhmljJ5EZ7Q`) | Test direct native analysis alternative | Engine started; failed on an incorrect postScript path (analyst error) | `Failed_Analysis_Operations.md` F-03/F-04 |
| 10 | Corrected Ghidra headless job (background `sh_1e532657e001X8GkWWQPPQekTP`) | Export functions/imports of `Patient Data Transport.exe` | **CANCELLED by server restart** | `Failed_Analysis_Operations.md` F-05 |
| 11 | `grep`/`read` over prior reports and primary dumps | Cross-source correlation, business-rule recovery | numerous evidence links | `07_`, `08_`, `09_` documents |
| 12 | Subagent delegation (4 workstreams, relaunched once) | Parallel deep-dive across disjoint domains | in progress | `03_`–`09_` |

## Notes

- After the server restart, the `rea` tool namespace became unavailable. No further REA calls were made; all subsequent work uses shell/read/grep and the already-extracted primary dumps.
- No target or third-party binary was ever executed. Ghidra loads bytes statically; the analyzed program does not run.
- No file in `D:\Collective work` was written, renamed, deleted, or executed.
