# Evidence Coverage Report — REAL LAB SYSTEM v1.605

Every percentage below has an explicit denominator and an auditable completion definition. "BLOCKED" domains are not reported as percentages of an unavailable sample.

## 1. Coverage matrix by investigation domain

| Domain | Denominator | Completed | Coverage | Completion definition | Status |
|---|---:|---:|---:|---|---|
| Artifact discovery | 973 files in `D:\Collective work` | 973 | **100%** | every file enumerated by recursive walk | COMPLETE |
| Artifact hashing (SHA-256) | 973 | 973 | **100%** | hash computed for every file; manifest written | COMPLETE |
| Duplicate identification | 973 | 973 | **100%** | grouped by SHA-256; 22 duplicate groups (58 files) recorded | COMPLETE |
| Archive enumeration | 8 ZIPs / 4,395 entries | 8 / 4,395 | **100%** | all entries listed read-only; zero nested archives | COMPLETE |
| Magic-byte / extension validation | 973 | 973 | **100%** | 16-byte header read for every file | COMPLETE |
| Executable fingerprinting | 37 `.exe` | 37 | **100%** | version resource, size, SHA-256 for every EXE | COMPLETE |
| PE structural analysis (v1.605) | 1 authoritative dump | 1 | **100% of available evidence** | `real lab system.exe (2).json` fully parsed; original binary absent | COMPLETE (evidence-limited) |
| PE structural analysis (v1.00 baseline) | 1 binary + 1 dump | 1 | **100%** | local 59 MB binary hashed + dump parsed | COMPLETE |
| SQL statement corpus mining | 1,797 rows | 1,797 | **100%** | verb distribution, table refs, VAs, targeted pattern scans | COMPLETE |
| Embedded DDL corpus | 268 statements | 268 | **100%** | full corpus available and classified (27 CREATE / 241 ALTER) | COMPLETE |
| VB6 object inventory | 210 objects | 210 | **100%** | all objects enumerated with type and method count | COMPLETE |
| VB6 method **names** | 8,026 declared | 0 recovered (1,268 addresses) | **0% of names; 15.8% addressable** | names not stored in evidence — information-theoretic limit, not analysis incompleteness | BLOCKED |
| Database schema (v1.605) | 103 tables / 1,682 columns | 103 / 1,682 | **100%** | parsed from `v2019-Patients.mdb.sql` into `parsed_schema.json` | COMPLETE |
| Database data pages / query plans | not available | 0 | **N/A** | no `.mdb` file present | BLOCKED |
| Configuration inventory | INI/UDL artifacts in evidence | enumerated | **100% of present artifacts** | `ini-analysis.json`, key deltas, config reports; secrets withheld | COMPLETE |
| Dependency inventory | OCX/OCA/DLL artifacts | enumerated | **100% of present artifacts** | `*.ocx.json`, `*.oca.json`, typelib surface, dependency graphs | COMPLETE |
| Native decompilation of v1.605 | 1 binary absent | 0 | **BLOCKED** | no bytes to analyze; REA Ghidra provider also unavailable | BLOCKED |
| Runtime verification | target never executed | 0 observations | **BLOCKED** | target binary absent; no isolated environment; safety rules | BLOCKED |
| Cross-source correlation | major findings | all | **COMPLETE** | evidence graph in `09_CROSS_SOURCE_EVIDENCE\` | COMPLETE |
| Knowledge documentation | 12 output directories | all populated | **COMPLETE** | every required document has substantive content; no stubs | COMPLETE |

## 2. Domain maturity statuses (acceptance criteria)

| Investigation domain | Status | Grounds |
|---|---|---|
| Source discovery | COMPLETE | 973/973 recursive inventory + hashes |
| Tooling | COMPLETE | independent verification of all relevant tools; REA server state recorded |
| Executable analysis | PARTIALLY COMPLETE | full analysis of available dumps; v1.605 binary absent |
| Managed code | NOT APPLICABLE | target is VB6 native; auxiliary .NET sender is not the main app |
| Native code | PARTIALLY COMPLETE | existing Ghidra dumps used; REA provider blocked; VB6 p-code limits semantics |
| Database | COMPLETE (from dumps) | 103/1,682 reconstructed; original MDBs absent → data-page analysis blocked |
| Data access | COMPLETE | 1,797 SQL statements mapped to objects/verbs/tables; 0 INSERT/UPDATE finding |
| Business logic | PARTIALLY COMPLETE | strong field/SQL-level recovery; formula/algorithm gaps remain |
| UI | PARTIALLY COMPLETE | 132 forms/66 DataReports/controls enumerated; no live navigation |
| Runtime | BLOCKED | target binary absent — documented technical reason |
| Configuration | COMPLETE | all present INI/UDL/asset artifacts inventoried |
| Cross-source analysis | COMPLETE | 8+ multi-source chains, conflicts and confidence assessment |
| Knowledge repository | COMPLETE | 12 directories, integrated master KB |
| Remaining gaps | COMPLETE | every gap explicitly investigated and classified |

## 3. Separately measured coverage dimensions (final mission report inputs)

| Dimension | Denominator | Covered | Value | Definition |
|---|---:|---:|---:|---|
| Artifact discovery | 973 files | 973 | 100.0% | files present in repository |
| Artifact extraction | 8 archives / 4,395 entries | 8 / 4,395 | 100.0% | read-only enumerated; no nested archives |
| Executable analysis | 1 target PE (absent) + 1 baseline PE | 1 dump + 1 binary | 100% of available; 0% of target bytes | distinguishes evidence-limited analysis from full binary analysis |
| Database analysis | 103 tables / 1,682 cols | 103 / 1,682 | 100.0% schema; 0% data pages | DDL-derived reconstruction |
| Business logic recovery | 11 rule groups identified | 9 with direct evidence; 2 partial | ~82% | see `MASTER_BUSINESS_LOGIC.md` confidence map |
| Application-to-database mapping | 1,797 SQL statements | 1,797 verb/VA/table classified; 0 named-method bindings | 100% statement-level; 0% method-name-level | method bodies absent |
| Behavioral reconstruction | core workflows identified (registration→billing, 9 workflows) | all with static evidence | 100% reconstructed; 0% observed | static-only |
| Runtime verification | target executions possible | 0 | BLOCKED (0/0 sample) | no executable available |
| Evidence traceability | major conclusions | all | 100.0% | every major finding cites an evidence file |
| Knowledge documentation | required master docs (7) + directory docs | all | 100.0% | no empty/stub documents |

**Note on the "~82% business logic" figure:** it is derived from 11 identified rule groups of which 9 have direct SQL/DDL evidence and 2 (composite-test storage, financial per-invoice computation) are only partially evidenced. It is not an estimate of overall system understanding.

## 4. Why no single overall percentage is reported

A single percentage would conflate (a) complete coverage of available evidence, (b) absence of primary artifacts (target binary, MDBs), and (c) genuine analytical gaps. Those are different quantities with different denominators. This report keeps them separate, per MASTER DIRECTIVE §16.
