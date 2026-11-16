# Environment Verification — REAL LAB SYSTEM v1.605 Investigation

**Verification date:** 2026-11-16  
**Method:** Independent re-execution of tool probes in the current session; results compared with the supplied infrastructure audit (`C:\Users\LAP LINK\source\repos\Ref Lab\Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md`).  
**Evidence level:** VERIFIED for every row marked as such; the audit itself is treated as an inventory reference, not as proof (per MASTER DIRECTIVE §4.2).

---

## 1. Host and operating environment

| Item | Independently observed | Audit agreement | Confidence |
|---|---|---|---|
| Shell | PowerShell Core 7.6.10 (`pwsh`) | Agrees | VERIFIED |
| OS | Windows 10 Pro, x64 (audit: build 19045) | No contradiction | STRONGLY_SUPPORTED |
| CPU | 2 logical processors (audit) | Not re-probed | STRONGLY_SUPPORTED |
| RAM | ~8 GB (audit) | Not re-probed | STRONGLY_SUPPORTED |
| Administrative rights | None (audit: `IsAdmin=False`) | Not re-probed | STRONGLY_SUPPORTED |
| Working directory | `C:\Users\LAP LINK\source\repos\Ref Lab` | Agrees | VERIFIED |
| Git repository | Yes, `main`, `El-ogra/Ref-Lab.git` (audit) | Not re-probed | STRONGLY_SUPPORTED |

## 2. Tool availability — independent probe results

| Tool / capability | Probe result | Path / version | Relevance to this investigation | Confidence |
|---|---|---|---|---|
| PowerShell | `pwsh 7.6.10` | session shell | Primary execution channel (inventory, hashing, forensics) | VERIFIED |
| Node.js | `v24.19.0` | `C:\Program Files\nodejs\node.exe` | REA runtime; CLI orchestration | VERIFIED |
| npm | `11.17.0` | global prefix `C:\Users\LAP LINK\npm-global` | Package ecosystem | VERIFIED |
| Python | `3.13.1` | `C:\Program Files\Python313\python.exe` | Parsing/forensic helper scripts | VERIFIED |
| .NET SDK | `9.0.321` | `C:\Program Files\dotnet` | Build/test of the *reconstruction* repo (not the VB6 target) | VERIFIED |
| Java | `openjdk 21.0.13` (LTS) | `C:\Program Files\Microsoft\jdk-21.0.13.21-hotspot\bin\java.exe` | Required by Ghidra | VERIFIED |
| `JAVA_HOME` | **unset** in the session and at REA MCP server start | — | Root cause of REA/Ghidra provider fault | VERIFIED |
| REA CLI | `6.7.0` | npm-global (`rea`, `rea.cmd`) | Reverse-engineering CLI fallback | VERIFIED |
| REA MCP server | LIVE — `tools.rea.binary_session({})` returned full capability inventory | registered in `C:\Users\LAP LINK\.config\opencode\opencode.json` | 207 tools available via `execute` | VERIFIED |
| Ghidra | `12.1.4` installed, `analyzeHeadless.bat` present, native decompiler reports PE x86_64 | `C:\Tools\ghidra_12.1.4_PUBLIC` | Native static analysis | VERIFIED (installed) |
| REA→Ghidra provider | `runtime_unavailable`: "Select a 64-bit full JDK 21 or newer on PATH, or set JAVA_HOME … before starting REA"; probe failure `java.exe: Owned Windows process requires an absolute executable path (Win32 87)` | from `binary_session` diagnostics | Blocks REA-driven native decompilation; direct headless invocation with `JAVA_HOME` set in the shell was attempted (see §4) | VERIFIED |
| `sqlcmd` | Present (ODBC 17) | `...\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE` | SQL Server read-only queries | VERIFIED |
| `psql` | Not on PATH; **present and operational by absolute path** — `psql (PostgreSQL) 17.6` | `C:\Program Files\PostgreSQL\17\bin\psql.exe` | PostgreSQL (not used; target is Access/SQL Server) | VERIFIED |
| `tshark` / `capinfos` | Not found at `C:\Program Files\Wireshark\`; not on PATH | — | PCAP analysis; not needed (no capture evidence in repository) | VERIFIED (absent at probed location) |
| Git | `2.49.0.windows.1` | `C:\Program Files\Git\cmd\git.exe` | Source repository operations | VERIFIED |
| Dotnet EF / QuestPDF previewer / xUnit | Present per audit; not re-probed | global .NET tools | Relevant to the *reconstruction* repo only | STRONGLY_SUPPORTED |

## 3. Tool-to-task matrix (actual selection for this investigation)

| Investigation activity | Capability required | Tool actually selected | Status |
|---|---|---|---|
| Recursive artifact discovery + SHA-256 | filesystem walk + hashing | PowerShell `Get-ChildItem -Recurse`, `Get-FileHash` | COMPLETE (973 files) |
| PE/version fingerprinting of local binaries | PE resource + hash parsing | PowerShell `VersionInfo`, JSON PE dumps, REA `binary_session` | COMPLETE |
| Managed (.NET) analysis | CLI metadata reader | REA `inspect_managed_artifact` (available) | AVAILABLE; target binaries are VB6 native → NOT APPLICABLE |
| Native binary analysis | disassembly/decompilation | Ghidra 12.1.4 headless (direct) + existing Ghidra output dumps in evidence; REA provider blocked | PARTIAL |
| Database schema reconstruction | DDL/SQL/CSV parsing | PowerShell + Python parsing of `v2019-Patients.mdb.sql`, schema/CSV dumps; `sqlcmd` available but no live target DB | COMPLETE (from dumps) |
| SQL/business-logic recovery | large CSV mining | PowerShell/`Import-Csv` over 1,797-row statement inventory | COMPLETE |
| Cross-source correlation | document + data mining | `grep`/`read`/Code Mode `execute` + subagents | COMPLETE |
| Runtime observation of v1.605 | executable + DB + isolation | — | BLOCKED: v1.605 binary absent from evidence repository (see `07_RUNTIME_AND_BEHAVIORAL_ANALYSIS\Runtime_Observations.md`) |

## 4. Ghidra remediation attempt

Because the audit identified the Java fault as fixable without installation, a direct headless Ghidra invocation was attempted with `JAVA_HOME` set in the **shell** (not the already-running MCP server) against a small shipped component (`RLUTool.exe`, 126,976 B), writing only to the approved temp area `C:\Users\LAP LINK\AppData\Local\Temp\opencode\ghidra_probe`. The outcome is logged in `11_INVESTIGATION_LOGS\Tool_Execution_Log.md`. Setting the variable in the shell cannot repair the MCP server's already-started process; it can only enable **direct** headless runs. This distinction is material to the coverage claim for native decompilation.

## 5. Limitations that affect this mission

1. **The 1.605 executable is not present as a file.** Only its extracted PE/object/SQL/literal dumps are. No tool — REA, Ghidra, or debugger — can analyze bytes that are not in the evidence repository. This is the single most consequential limitation.
2. **No .mdb file is present.** Database reconstruction is from authoritative extracted DDL/CSV dumps, not from a live Jet engine.
3. **REA's Ghidra provider is unavailable** because `JAVA_HOME` was unset when the MCP server started; direct headless invocation was attempted as the independent fallback.
4. **No administrative privileges**, so no service/driver installation; irrelevant to static evidence work but relevant to any attempted runtime reproduction.
5. Third-party remote-access binaries (AnyDesk, TeamViewerQS, Supremo) and installers (Bullzip, Ghostscript, Xpdf, PDF Power Tool, .NET 4, MDAC, VC runtimes) are **not** target application logic and were not executed.
