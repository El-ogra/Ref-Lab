# Remaining Knowledge Gaps — REAL LAB SYSTEM v1.605

Each gap is classified with the required confidence vocabulary and the specific technical reason it remains open. Every gap is investigated; none is silently abandoned.

| ID | Gap | Classification | Why it matters | Why it remains | Evidence source that could resolve it | Tool required |
|---|---|---|---|---|---|---|
| GAP-01 | v1.605 executable binary | **BLOCKED — artifact absent** | All byte-level decompilation, debugging and runtime work | Only the PE dump exists; exhaustive search of 973 files + 8 ZIPs found no 76,292,096-byte binary | Authentic binary SHA `C54CE6C8…` | Ghidra/REA/debugger |
| GAP-02 | Original `.mdb` databases | **BLOCKED — artifacts absent** | Data pages, indexes, stored QueryDefs, query plans, live data validation | Only DDL/CSV dumps present | `Patients.mdb`, `Branch_Data.mdb`, `data.mdb` originals | Jet/Access engine / `mdbtools` |
| GAP-03 | All runtime behavior | **BLOCKED — no executable + no isolated environment** | Confirm actual startup, navigation, workflows, printing, errors | Safety rules; no disposable VM with VB6/OCX/DB/mock analyser | Licensed deployment + controlled lab (see `Controlled_Experiments.md`) | API monitor, debugger, DB tracer |
| GAP-04 | VB6 method names/bodies (8,026 declared; only 1,268 addressable; 0 names) | **BLOCKED — information not stored in evidence** | Complete call graph, named method→SQL mapping | Prior extraction did not preserve names/bodies | Decompiled VB6 p-code from authentic binary | VB6 decompiler / Ghidra + VB6 runtime analysis |
| GAP-05 | Composite-test component storage and validation | **PARTIAL** — `patienttest` structurally supports up to 5 components (`RESULTT` + `resultt1..resultt4`, VA `6C3AFC`), plus per-discipline profile tables; no field declares "composite" | Full CBC-style processing fidelity | Component-count mapping per `testcode` and the write/validation path require missing form code | Authentic binary + runtime experiment E-3 | Debugger / DB capture |
| GAP-06 | Per-invoice financial computation, `discount100` semantics, refunds | **PARTIAL** — aggregate formula verified at VA `69EA76` | Financial fidelity | Per-invoice code path not recovered; `discount100` meaning inferred only from name | Binary + experiment E-6 | DB capture + code analysis |
| GAP-07 | VIDAS database structure | **UNKNOWN** | Deployment completeness, third backend hypothesis | Referenced in `Attach_VIDAS.bat` mentions only; no artifact | VIDAS database or connection spec | DB inspection |
| GAP-08 | `RL_SETUP.exe` 22.95 MiB embedded payload | **UNKNOWN** | Installer completeness, possible hidden components | Characterized (entropy 7.39 in `.text`) but not decomposed | Payload extraction from authentic installer | Archive/carving tools |
| GAP-09 | Grant-bit semantics (`grant1..grant12`) | **INFERRED** | Authorization model fidelity | Column names present; no code mapping each bit to a permission | Binary + experiment E-8 | Code/runtime analysis |
| GAP-10 | Five missing Microsoft installers & second-disc hypothesis | **UNVERIFIED** | Complete deployment reconstruction | Absent from inventoried tree | Original installation media | Inventory |
| GAP-11 | 25 external component names undecoded; 5 unnamed components | **UNVERIFIED** | Full dependency graph | Typelib/OCX names not fully resolved in dumps | Original OCX/typelib files | COM typelib inspection |
| GAP-12 | DDL migration trigger (install-time vs startup) and exact order | **UNKNOWN** | Schema migration fidelity | 268 DDL literals recovered; execution trigger not | Binary + experiment E-9 | Code/runtime analysis |
| GAP-13 | `normalrange.pregsmok` calculation use | **UNVERIFIED** | Clinical rule completeness | Column populated on all 852 rows but no traced use | Binary/code search | Code analysis |
| GAP-14 | Locale/error-handling behavior | **UNVERIFIED** | Robustness reconstruction | One error literal at VA `6717E2`, context unknown; documented date-format requirement | Binary + runtime | Code/runtime analysis |
| GAP-15 | MySQL third-backend hypothesis | **UNVERIFIED** (low support) | Data architecture completeness | Mentioned in prior docs; no primary evidence located | Authentic config/artifacts | Evidence discovery |

## Gap status summary

| Classification | Count | Disposition |
|---|---:|---|
| BLOCKED (missing artifact / information) | 4 | Cannot be resolved with available evidence; explicit technical reason recorded |
| UNKNOWN | 3 | Referenced but uncharacterized; needs new artifacts |
| UNVERIFIED | 5 | No primary confirmation; leads preserved |
| PARTIAL | 1 | Aggregate formula recovered; per-record details open |
| INFERRED (usable but not confirmed) | 2 | Labeled as such wherever used |

## Iteration record

The gap-iteration cycle (MASTER DIRECTIVE §9) was executed: each gap was described, its evidence source identified, the relevant tool applied where possible, results compared with existing knowledge, and the repository updated. Gaps resolved during this run (target-vs-baseline identity, archive nesting, write path, result flags, financial aggregate, timestamp conflict, tool availability) are recorded in `11_INVESTIGATION_LOGS\Knowledge_Gap_Resolution_Log.md`. The gaps above are the residual set after that cycle.
