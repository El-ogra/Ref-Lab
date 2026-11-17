# Coverage measurements — per dimension, with explicit denominators

**Rule applied.** No single aggregate "percentage complete" figure is given. Each dimension states its own definition of completeness, its numerator, its denominator and the artefact that proves it. A dimension is only counted as complete when the *definition* is met for **every** item in the denominator.

**Date:** 2026-11-16.

---

## 1. Measurement table

| # | Dimension | Definition of "complete" | Numerator | Denominator | Coverage | Proof artefact |
|---:|---|---|---:|---:|---:|---|
| 1 | Target-tree inventory | every file carries path, byte size, SHA-256, extension and UTC mtime | 837 | 837 | **100 %** | `evidence/target-tree-sha256-manifest.csv` |
| 2 | PE characterisation | magic, machine, image base, section table, import table, resources, timestamp, version | 7 | 7 | **100 %** | `evidence/rea-pe-resources.json`; `scripts/pe_imports.py` |
| 3 | VB6 object identification | descriptor resolves to a printable object identifier | 210 | 212 | **99.1 %** | `evidence/vb6-objects-and-method-names.json` |
| 4 | VB6 method-count fidelity | Σ `dwMethodCount` equals the independently-recorded prior figure | 8,026 | 8,026 | **100 %** | `evidence/vb6-objects-and-method-names.json` |
| 5 | VB6 method identification | method name recovered as a string | 444 | 8,026 | **5.5 %** | same |
| 6 | Objects carrying named members | ≥ 1 named member recovered | 93 | 212 | **43.9 %** | same |
| 7 | SQL literal extraction | SQL/DDL literal recovered from the authentic image | 1,758 | 1,758 | **100 % of extractable** | `evidence/sql-literals-utf16.json` |
| 8 | SQL negative findings | `INSERT INTO` absent; `UPDATE … SET` absent | 2 | 2 | **100 %** | same |
| 9 | SQL literal → code binding | literal has ≥ 1 verified immediate-operand code site | 1,750 | 1,758 | **99.5 %** | `evidence/sql-literal-to-code-sites.json` |
| 10 | Code-site instruction classification | site classified from raw instruction bytes | 3,718 | 3,718 | **100 %** | `scripts/bind_verify.py` |
| 11 | DDL localisation | DDL code site assigned to the migration region | 291 | 307 | **94.8 %** | `evidence/sql-literal-to-code-sites.json` |
| 12 | Database schema coverage | tables, columns, indexes, primary keys and row counts read from the real file | 103 / 1,682 / 68 / 5 / 103 | 103 / 1,682 / — / — / 103 | **100 %** | `evidence/mdb-forensics/` |
| 13 | Database files covered | every shipped `.mdb` opened and dumped | 5 | 5 | **100 %** | same |
| 14 | Stored QueryDefs / Access system tables | readable | 0 | unknown | **0 % — permission-blocked** | `evidence/mdb-forensics/*/errors.txt` |
| 15 | Prior corpus content re-verification | prior text found verbatim in the authentic image | 1,960 | 2,065 | **94.9 %** | `evidence/prior-corpus-audit-vs-authentic-binary.json` |
| 16 | Prior corpus address re-verification | prior VA resolves to its claimed statement | **0** | 2,065 | **0 % — systematically wrong, corrected** | same + `evidence/va-reanchoring.json` |
| 17 | Financial rules | each documented rule recovered with its exact text | 4 | 4 | **100 %** of the aggregate/invoice rules | `03_SQL_CORPUS_AND_CODE_BINDING.md` §6 |
| 18 | Per-invoice financial arithmetic | the computation of one invoice recovered | 0 | 1 | **0 % — computation is in VB code, not SQL** | same |
| 19 | `discount100` semantics | semantics established beyond "a double column named discount100" | 0 | 1 | **0 % — INFERRED only** | `04_DATABASE_FORENSICS_REAL_MDB.md` §7 |
| 20 | Composite-test declarative mapping | subgroup→profile mapping and package membership | 48 + 54 | 48 + 54 | **100 %** | `04_DATABASE_FORENSICS_REAL_MDB.md` §9 |
| 21 | Composite-test per-`testcode` analyte list | the analyte columns belonging to each `testcode` | 0 | 609 | **0 % — no declaration column exists** | same |
| 22 | Composite-test write/validation path | the write code identified | 0 | 1 | **0 % — bound-recordset write, no literal SQL** | `03_SQL_CORPUS_AND_CODE_BINDING.md` §2 |
| 23 | External component identification | every shipped OCX/DLL/TLB identified by product name and version | 78 | 78 | **100 %** | `01_TARGET_TREE_INVENTORY.md` §4.5 |
| 24 | Native function inventory (Ghidra) | decompiled function list with entry points and body extents | 8,021 | 8,021 | **100 %** | `evidence/ghidra-functions.tsv` |
| 24a | Function ↔ SQL statement map | function matched to the SQL statements it executes | 928 | 8,021 | **11.6 % of functions carry SQL; 100 % of SQL-bearing functions mapped** | `evidence/ghidra-sql-by-function.tsv` |
| 24b | SQL code site inside a function | site attributed to a containing function | 3,566 | 3,718 | **95.9 %** | `evidence/ghidra-sql-site-to-function.tsv` |
| 24c | Function → VB6 object / control name | function labelled with its owning VB6 object | 0 | 8,021 | **0 % — procedure-address table did not resolve** | `02_VB6_OBJECT_AND_METHOD_STRUCTURE.md` §9 |
| 25 | Runtime behaviour | observed at run time | 0 | unknown | **0 % — deliberately not executed** | `05_GAP_RESOLUTION_LOG.md` gap 8 |
| 26 | Report-template corpus | every template's binding key read | 228 | 228 | **100 %** | `01_TARGET_TREE_INVENTORY.md` §4.6 |
| 27 | Deployment model | install chain, dependency set and target directory established | 45 | 45 `SETUP.LST` entries | **100 %** | `01_TARGET_TREE_INVENTORY.md` §6 |

---

## 2. Movement against the prior pass

| Dimension | Prior pass | This pass | Change |
|---|---:|---:|---|
| Target binary available for analysis | no | **yes** | decisive enabler |
| VB6 object names | 210 | 210 | — |
| VB6 method names | **0** | **444** | +444 |
| SQL literals bound to code sites | 0 (unverified) | **1,750 / 1,758** | +1,750 |
| SQL code sites attributed to a function | 0 | **3,566 / 3,718** | +3,566 |
| Native functions recovered | 0 | **8,021** | +8,021 |
| Functions mapped to their SQL statements | 0 | **928** | +928 |
| DDL sites localised | 0 | **291 / 307**, and **one function `FUN_01b61cf0`** | +291 |
| Declared primary keys | 0 | **5** | +5 |
| Indexes | 0 | **68** | +68 |
| Grant flags documented | 12 | **17** | +5 |
| `normalrange` rows | 852 (wrong) | **1,152** | corrected |
| Prior VAs valid | — | **0 / 2,065** | all re-anchored |
| Databases opened directly | 0 | **5 / 5** | all dumped |
| Runtime observations | 0 | 0 | unchanged, by design |

---

## 3. What the denominators mean

- **8,026** — the sum of `dwMethodCount` over all 212 VB6 object descriptors. Independently corroborated by the prior evidence record, so it is a *measured* denominator rather than an assumed one.
- **1,758** — SQL/DDL literals recovered by UTF-16LE scanning of the authentic `.text`. This is the *extractable* population; the prior 1,797 differs by 39 rows of classification, so the two corpora are comparable but not identical, and both prior negatives (0 `INSERT`, 0 `UPDATE`) reproduce exactly.
- **2,065** — 1,797 prior statement rows + 268 prior DDL rows, the full prior addressable corpus.
- **609** — `testtable` rows in the shipped database, i.e. the population of catalogue tests for which a per-`testcode` analyte list would be required.
- **837** — every file under `F:\REAL_LAB_COPY`, hashed individually.
- **78** — 41 OCX + 33 DLL + 4 TLB shipped control/runtime components.

---

## 4. Known measurement limitations

1. **Method-name coverage (5.5 %) is capped by the artifact**, not by effort: the compiled image stores no procedure names (§2 of `02_VB6_OBJECT_AND_METHOD_STRUCTURE.md`). Raising this number is impossible without a different build.
2. **Per-invoice financial arithmetic and `discount100` semantics** are 0 % because they are computed in VB code. Recovering them requires decompilation, which is blocked by the host (§9 of `02_VB6_OBJECT_AND_METHOD_STRUCTURE.md`).
3. **Runtime behaviour (0 %)** is a deliberate choice, not an oversight.
4. **Access system-table metadata (0 %)** is blocked by Jet ACLs; the specific error strings are recorded in `evidence/mdb-forensics/*/errors.txt`.
5. **Ghidra auto-analysis cannot complete on this host** — the default profile costs 9,077 s for this image, of which *x86 Constant Reference Analyzer* (3,582 s) and *Stack* (2,984 s) alone account for 72 %, and the run is cut off at 7,200 s. The saved project is nonetheless reusable, which is how the function inventory was obtained without re-analysis; the API-correct `scripts/ConfigureAnalysis.java` disables precisely those two analyzers and is the recommended remediation on a larger machine.
6. **Function → VB6 object attribution (0 %)** needs the per-object procedure→code table, which this build stores in a form that did not resolve to a usable address.
