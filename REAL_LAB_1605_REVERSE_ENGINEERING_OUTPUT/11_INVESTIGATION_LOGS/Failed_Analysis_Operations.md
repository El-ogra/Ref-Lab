# Failed, Blocked, and Negative Analysis Operations

This log distinguishes **failures of the investigation** from **negative findings** (a bounded search that legitimately returned nothing).

| # | Operation | Outcome | Classification | Impact | Remediation attempted |
|---|---|---|---|---|---|
| F-01 | Locate the v1.605 executable (76,292,096 B, SHA `C54CE6C8…`) in `D:\Collective work` | NOT FOUND among 973 files and inside all 8 ZIP archives | BLOCKED — missing primary artifact | No direct decompilation/debugging/runtime of target version | Exhaustive filename + size + archive-entry search. Only `real lab system.exe (2).json` (PE dump) exists. |
| F-02 | Locate original `.mdb` / `.accdb` databases | NOT FOUND | BLOCKED — missing primary artifact | Schema reconstructed from authoritative DDL/CSV dumps; no live Jet validation | Extension scan + magic-byte scan; none present |
| F-03 | REA Ghidra provider decompilation | `runtime_unavailable`: Java absolute-path fault; `JAVA_HOME` unset when MCP server started | BLOCKED — tool configuration | REA-driven native decompilation unavailable | Verified via `binary_session`. Direct Ghidra headless with `JAVA_HOME` set in shell succeeded in starting analysis (probe F-04). |
| F-04 | First Ghidra headless probe (`RLUTool.exe`) | Failed at script loading: `DecompilerParameterId.java : Script not found` | FAILURE — analyst error (wrong script path), not a tool defect | None; corrected in F-05 | Supplied a purpose-written Jython export script in the temp directory |
| F-05 | Corrected Ghidra headless analysis of a shipped support component | Launched successfully; see `Tool_Execution_Log.md` for final output | SUCCESS (in progress at time of this entry) | Independent native function/import inventory for one support tool; does not substitute for the absent v1.605 binary | — |
| F-06 | `psql --version` from PATH | Not recognized | MINOR — PATH configuration | None; PostgreSQL is irrelevant to the target | Executed by absolute path `C:\Program Files\PostgreSQL\17\bin\psql.exe` → `psql (PostgreSQL) 17.6` |
| F-07 | `tshark --version` from PATH / standard install path | Not found | MINOR — tool absent at probed location | None; no packet capture evidence exists in the repository | No remediation needed |
| F-08 | Find `INSERT INTO` / `UPDATE … SET` literals in the 1,797-statement SQL corpus | 0 matches | **NEGATIVE FINDING** (bounded, complete scan) | Establishes that writes use bound recordsets, not literal SQL — an architectural result, not a failure | Verified with case-insensitive regex over the complete `text` column |
| F-09 | Find nested archives inside the 8 ZIP files | 0 matches across 4,395 entries | **NEGATIVE FINDING** | No recursive extraction required | Full entry-name scan for zip/rar/7z/cab/tar/gz/mdb/accdb |
| F-10 | Detect magic-byte/extension mismatches across 973 files | 0 real mismatches (2 `.xlsx` are legitimately ZIP containers) | **NEGATIVE FINDING** | Artifact typing is reliable | 16-byte header scan of every file |
| F-11 | Runtime observation of the target | NOT ATTEMPTED | BLOCKED — missing binary + no isolated environment + safety rules | All behavior remains reconstructed, not observed | Documented safe experiment designs in `Controlled_Experiments.md` |
| F-12 | Recover per-method names for all 8,026 v1.605 methods | Not possible from available dumps: prior work records only 1,268 methods with resolvable addresses and that names were not stored for the remainder | BLOCKED — information not present in evidence | No fabricated per-method call graph | Object-level and control-descriptor-level catalog used instead |

## Interpretation rules applied

- A bounded search with a defined corpus and zero matches is recorded as a **negative finding**, never as "incomplete analysis".
- A missing primary artifact is recorded as **BLOCKED**, with the exact missing SHA-256/size where known, never as a percentage of coverage.
- Analyst errors (F-04) are recorded transparently and corrected; they are not hidden.
