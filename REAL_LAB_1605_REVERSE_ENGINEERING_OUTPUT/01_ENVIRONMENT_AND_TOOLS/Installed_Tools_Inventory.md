# Installed Tools Inventory

This inventory is the reproducible basis for `Tool_Capability_Matrix.md`. "Verified now" means the executable was invoked or its presence confirmed during this investigation (2026-11-16). "Audit-only" means the fact is taken from the supplied infrastructure audit and was not re-probed; no contradiction was observed.

## 1. Execution, scripting, and orchestration

| Tool | Version | Install path | Formats / capability | Operational | Relevant | Evidence |
|---|---|---|---|---|---|---|
| PowerShell Core (`pwsh`) | 7.6.10 | session | scripting, filesystem, hashing, CSV/JSON | Yes | Critical | Verified now |
| Windows PowerShell | 5.1 | `C:\WINDOWS\System32\WindowsPowerShell\v1.0` | legacy scripting | Audit-only | Low | Audit §2.1 |
| Git Bash | bundled with Git 2.49 | `C:\Program Files\Git\bin` | POSIX shell | Audit-only | Low | Audit §2.1 |
| Node.js | 24.19.0 | `C:\Program Files\nodejs\node.exe` | JS/ESM runtime, REA host | Yes | Critical | Verified now |
| npm / npx | 11.17.0 | global prefix `C:\Users\LAP LINK\npm-global` | package mgmt | Yes | Medium | Verified now |
| Python | 3.13.1 | `C:\Program Files\Python313\python.exe` | parsing, pypdf/pymupdf, PIL | Yes | High | Verified now |
| uv | 0.7.15 | PATH | Python interpreter mgmt | Audit-only | Low | Audit §9 |
| Code Mode `execute` | — | harness | JS orchestration of all tool calls | Yes | Critical | Session capability |
| Git | 2.49.0.windows.1 | `C:\Program Files\Git\cmd\git.exe` | VCS | Yes | Medium | Verified now |

## 2. .NET / build / test

| Tool | Version | Capability | Relevant | Evidence |
|---|---|---|---|---|
| .NET SDKs | 9.0.321 (verified), 8.0.428 (audit) | build/test of the C# reconstruction repo | Low for VB6 target | Verified now / audit §4 |
| MSBuild | 17.14.45 | VS 2022 build | Low | Audit §5.3 |
| dotnet-ef | 8.0.30 (audit) | EF migrations | Low | Audit §8 |
| xUnit + Microsoft.NET.Test.Sdk | 2.9.3 / 17.11.1 (audit) | tests of reconstruction | Low | Audit §5.1 |
| QuestPDF / questpdf.previewer | 2026.9.1 / 2024.7.1 (audit) | PDF in reconstruction; preview | Low for target (target uses Bullzip/DataReport) | Audit §5.1, §8 |

## 3. Reverse engineering

| Tool | Version | Input formats | Operational | Relevant | Evidence |
|---|---|---|---|---|---|
| REA CLI (`rea`) | 6.7.0 | PE/ELF/Mach-O/APK/ASAR/WASM/EVM/firmware/SQLite/JS | Yes | Critical | Verified now (`rea --version`) |
| REA MCP server | 6.7.0 | 207 tools; `rea-artifact-graph`, `rea-dotnet-static`, `sqlite` providers available | Yes | Critical | `binary_session` verified now |
| REA → Ghidra provider | 12.1.4 engine | native PE decompilation | **No** — `runtime_unavailable`, Java absolute-path fault | Critical gap | `binary_session` diagnostics |
| Ghidra (direct) | 12.1.4 | PE/ELF native disassembly + decompiler | Headless present; direct run attempted | High | Verified path; outcome in Tool_Execution_Log |
| REA → Hopper / IDA / JADX / Kuna | — | macOS / IDA / Android / Kuna | Not installed/configured | Low (host/target mismatch) | Audit §6.5 + `binary_session` |
| REA `inspect_managed_artifact` / `inspect_managed_members` | 6.7.0 | PE/CLI .NET metadata | Available | Not applicable to VB6 target | `binary_session` groups available |
| REA `inspect_sqlite_database` | 6.7.0 | SQLite files | Available | Not applicable (Jet/Access) | `binary_session` capability |
| Python forensic scripts in evidence | — | PE, RTF, CHM, JET, INI, VB6 | Present as artifacts | High | `pe_analyse.py`, `rtf_analyse.py`, `analyse_ini.py`, `jet_analyse.py`, `vb6_*.py`, etc. |

## 4. Databases

| Tool | Version | Path | Operational | Relevant | Evidence |
|---|---|---|---|---|---|
| SQL Server 2022 Express | 16.0.1220.3 | `MSSQL$SQLEXPRESS`, dynamic TCP 56496 | Running (audit) | Medium — target can use SQLOLEDB; no target DB attached here | Audit §7.1 |
| `sqlcmd` | ODBC 17 | `...\ODBC\170\Tools\Binn\SQLCMD.EXE` | Yes | Medium | Verified now |
| PostgreSQL | 17.6 | `C:\Program Files\PostgreSQL\17\bin\psql.exe` | Yes (absolute path; not on PATH) | Not applicable to target | Verified now |
| Jet/Access engine | Jet 4 (target runtime) | not installed as a standalone tool on this host | — | High — target's primary store | Evidence-only |
| SQLite | EF Core provider 8.0.33 / REA provider | library | Available | Not applicable | Audit §7.1 |
| GUI DB clients (SSMS/DBeaver/pgAdmin) | — | absent | No | Low | Audit §8 |

## 5. Network, UI, and capture

| Tool | Status | Relevant | Evidence |
|---|---|---|---|
| Browser automation (45 tools) | Available | Low — no web UI in target | Session capability |
| Wireshark / tshark / capinfos | Installed per audit; tshark/capinfos not at probed standard path and not on PATH | Not applicable — no packet capture in evidence | Verified now / audit §9.2 |
| webfetch / websearch | Available | Low — no external lookups needed for artifact evidence | Session capability |
| Image generation | Available | Not used | Session capability |

## 6. Provenance and interpretation rules

- Every row that says "Verified now" was produced by an actual command executed in this session; the raw output is preserved in `11_INVESTIGATION_LOGS\Tool_Execution_Log.md`.
- The audit document's ≈85% readiness estimate applies to the **C# reconstruction stack**, not to the VB6 target; it was not reused as a coverage claim for this investigation.
- Tool presence does not imply tool suitability. The target is a 32-bit VB6 executable whose program logic is stored as VB6 object/method structures interpreted by `MSVBVM60.DLL`; native decompilation alone cannot recover that p-code-level logic. This is why the investigation privileges extracted object/SQL/DDL evidence over native disassembly.
