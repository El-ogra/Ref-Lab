# Runtime Observations

**Result: NO RUNTIME OBSERVATION WAS PERFORMED — for either the target or the baseline executable.** This document is the authoritative record of that fact and of the boundary on all behavioral claims in this knowledge repository.

## 1. What was and was not executed

| Item | Executed? | Note |
|---|---|---|
| `real lab system.exe` v1.605 (76,292,096 B) | NO | **Binary is not present in `D:\Collective work`** — only its PE dump and extracted object/SQL/literal/DDL data. Execution is physically impossible with the available evidence. |
| `real lab system.exe` v1.00 (59,150,336 B, present) | NO | Static analysis only. It is read-only evidence and a comparative baseline, not the target version. |
| `RL_SETUP.exe`, `001.exe`–`012.exe`, `007a.exe`, support tools, remote-access tools | NO | No target or third-party binary was launched. |
| Ghidra 12.1.4 headless | YES (tool only) | Ghidra *loaded bytes* of a support component for static analysis; the analyzed program was never run. This is static analysis, not runtime observation of the target. |
| SQL Server / PostgreSQL instances | NO | No connection to any live database was made. |
| Any installer, script, or macro from the evidence set | NO | Treated strictly as untrusted data. |

## 2. Why runtime observation was not attempted

1. **Primary blocker — missing artifact.** The target v1.605 executable does not exist in the evidence repository. The single most important runtime question ("what does the application actually do on startup?") cannot be put to an absent binary.
2. **No disposable environment for the available baseline.** The host is the working development machine (Windows 10 Pro, no admin rights, SQL Server/PostgreSQL listening, other projects and credentials present). Running a 32-bit VB6 medical application that reads INI/UDL configuration, writes INI files and registry keys, talks to printers and serial analysers, and may connect using shipped credentials would create exactly the cross-contamination risk the evidence-integrity rules prohibit.
3. **Required prerequisites are not assembled.** `MSVBVM60.DLL`, ~20 registered ActiveX controls, a configured Jet/SQL data store, mock analyser and mock SMTP sink are not present as an isolated stack. A failed launch would yield loader errors, not behavioral evidence.
4. **Mandate.** MASTER DIRECTIVE §9.1 requires a safety assessment and isolated environment *before* execution; §3.3 forbids modifying originals and executing embedded scripts/binaries without a justified isolated procedure. Those conditions are not met.

The prior investigation reached the same decision (evidence: `D:\Collective work\behavioural-analysis.md`, §§1–2: "no runtime execution of the target was performed"; the 13 executables "were never run"). **Provenance caveat:** that document describes a Linux/Kubernetes analysis sandbox, which does not match the present Windows host. Its *reasoning* is revalidated here; its environment-specific statements are NOT treated as facts about the current environment.

## 3. What can legitimately be asserted about behavior

Every behavioral statement in this repository is one of:

- **STATIC OBSERVATION** — a literal was read from a dump (e.g. SQL text at VA `66E8FE`, `SELECT pricemethod, patientnamear … FROM patientinfo GROUP BY …`). This tells us the code *contains* that statement, not that it executed.
- **STRUCTURAL RECONSTRUCTION** — an object/method/control structure was parsed from a VB6 object dump (e.g. `labmain` form with 71 methods / 184 controls).
- **INFERENCE** — a workflow was reconstructed from SQL + object names + documentation + schema, explicitly labeled INFERRED.
- **DOCUMENTED CLAIM** — a prior report says X; attributed to that report, and marked as such when not independently re-derived.

No statement in this repository is labeled "runtime observed". Any future document that claims otherwise must cite a process trace, API monitor log, debugger session, or controlled experiment actually executed against the target.

## 4. Safe runtime design (NOT EXECUTED — specification only)

For completeness, the controlled experiment design is recorded in `Controlled_Experiments.md`. It is a **plan**, with no results, because no experiment was run. Filling it with invented outcomes would be fabrication.

## 5. Confidence of this document

- "No target binary present" — VERIFIED (filesystem enumeration of all 973 files; exact match by SHA-256 to the known 1.605 digest is absent).
- "No execution performed in this investigation" — VERIFIED (session tool-execution log).
- Prior investigation's decision not to execute — STRONGLY_SUPPORTED (`behavioural-analysis.md` self-report, consistent across multiple prior reports).
