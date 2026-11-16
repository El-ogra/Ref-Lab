# Knowledge Gap Resolution Log

Iterative cycle: describe gap → identify why it matters → identify evidence source/tool → execute → compare → update → reassess.

| ID | Gap (before) | Why it matters | Evidence / tool applied | Result | Confidence after | Status |
|---|---|---|---|---|---|---|
| G-01 | Which version is the 59 MB `real lab system.exe`? | Prevents attributing v1.00 findings to v1.605 | `VersionInfo` + SHA-256 + `real lab system.exe.json` vs `(2).json` | Size 59,150,336 B, file version 1.00, SHA `12828D0A…`; v1.605 is 76,292,096 B, SHA `C54CE6C8…` and absent | VERIFIED | RESOLVED (gap is now "target binary missing", explicitly documented) |
| G-02 | Is the v1.605 binary hidden in the ZIPs? | Would enable direct analysis/runtime | Read-only enumeration of all 8 ZIPs (4,395 entries), extension filter | No `.exe` matching 76,292,096 B; only install-media support EXEs/DLLs/OCXs | VERIFIED | RESOLVED (negative) |
| G-03 | Are there original Access databases available for live inspection? | Schema fidelity, data types and relationships | Extension + magic-byte scan of 973 files | None; authoritative DDL/CSV dumps used instead | VERIFIED | RESOLVED with documented limitation |
| G-04 | Can REA/Ghidra analyze the target natively? | Native verification of code structure | `binary_session` diagnostics; direct headless probe with `JAVA_HOME` set | REA provider blocked; direct Ghidra works for present binaries. Neither can analyze the absent v1.605 binary | VERIFIED | PARTIALLY RESOLVED (tool issue understood; target absence remains) |
| G-05 | Does the application issue literal INSERT/UPDATE statements? | Determines data-access architecture | Exhaustive regex over 1,797 statements | 0 INSERT, 0 UPDATE; 72 DELETE FROM across 64 tables ⇒ bound-recordset writes | VERIFIED | RESOLVED |
| G-06 | What are the result status fields? | Core lab workflow | SQL VA cluster `681ED2`, `6860BE`, `686312`, `6865A6` | Four-flag tuple `printwith/resultfinsh/resultreview/resultprint` + six audit columns | STRONGLY_SUPPORTED (existence), INFERRED (ordering) | RESOLVED with residual uncertainty recorded |
| G-07 | Simple vs composite test storage | Central to reconstructing test processing | `patienttest` columns in `branch-profile-schemas.csv`; SQL VAs `6C3AFC` (four `RESULTTn` columns, `testcode=42`) and `6AD3A8` (single-component case); `testtable-stats.json`; `template-corpus-228.json` | Four representations identified; `patienttest` supports up to 5 result components on one row; **no field declares "composite"**, so per-`testcode` component mapping remains UNKNOWN | STRUCTURE VERIFIED; declaration UNKNOWN | PARTIALLY RESOLVED |
| G-08 | Financial formula and rounding | Financial reconstruction | `statements__[analysis_BATCH-04_sql_v1605].csv` VA `69EA76`; `Executable_Business_Logic.md` | Verbatim aggregate formula recovered: `Round(sum(totalprice),2)`, `Round(sum(totalpricedisc),2)`, net = gross − discount, plus two adjustment buckets, grouped by `pricemethod`; rounding = 2 decimals. Per-invoice `discount100` semantics still INFERRED | VERIFIED (aggregate); PARTIAL overall | PARTIALLY RESOLVED |
| G-09 | VIDAS database nature | Deployment completeness | prior docs only; no artifact | No structural evidence — remains UNKNOWN | UNVERIFIED | UNRESOLVED (technical limitation: artifact absent) |
| G-10 | 22.95 MiB RL_SETUP embedded payload | Installer completeness | prior docs; no decomposition | Characterized but not decomposed | UNKNOWN | UNRESOLVED (technical limitation) |
| G-11 | Arabic label authenticity | UI documentation integrity | `ui-labels-verified.md` + AGENTS.md rule 7 | Only that file is used as label source; no invented text | VERIFIED | RESOLVED |
| G-12 | Whether prior `behavioural-analysis.md` describes this host | Evidence provenance | Read §§1–2; compare with infrastructure audit | It describes a Linux/Kubernetes sandbox, not the present Windows host; reasoning reused, environment facts rejected | VERIFIED | RESOLVED (conflict recorded in `Conflicting_Findings.md`) |

## Open-gap policy

An OPEN or UNRESOLVED gap is never silently dropped. Each is carried into:
- `00_EXECUTIVE_REPORT\Remaining_Knowledge_Gaps.md`
- `09_CROSS_SOURCE_EVIDENCE\Confidence_Assessment.md`
- `10_UNIFIED_KNOWLEDGE_BASE\MASTER_SYSTEM_KNOWLEDGE.md` (final section)
