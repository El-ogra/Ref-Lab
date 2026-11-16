# Investigation Overview — REAL LAB SYSTEM v1.605

## Mission

Recover, reconstruct, correlate, verify, and document the maximum evidence-supported technical, architectural, behavioral, and operational knowledge about **REAL LAB SYSTEM v1.605**, using primary executable evidence, database evidence, prior reverse-engineering outputs, configuration artifacts, and the verified local toolchain.

## Target identity (established from evidence)

| Attribute | Value | Evidence | Confidence |
|---|---|---|---|
| Product | Real Lab System / "real lab system" | PE original filename, CHM, docs | VERIFIED |
| Target version identifier | **v1.605** (prior-work ID `V-CD-2019`) | `real lab system.exe (2).json` PE dump; `Version_Identity_and_Scope.md` §3 | VERIFIED |
| Main executable | `real lab system.exe`, **76,292,096 B**, PE32 i386 GUI, SHA-256 `C54CE6C8C98A679EFC5AB485BC4B313B452026D6A40F9BDA36ECA7EA18DF93C0` | `(2).json` dump | VERIFIED |
| File description / company | `RLS 1.605` / `RealLab` | `Version_Identity_and_Scope.md` §3.1 | STRONGLY_SUPPORTED |
| Runtime | Visual Basic 6 (`MSVBVM60.DLL`, 231 imports) | PE dump + prior analysis | VERIFIED |
| Primary data store | Microsoft Access (Jet 4) `Patients.mdb`, 103 tables / 1,682 columns; optional SQL Server | schema dumps + docs | STRONGLY_SUPPORTED |
| Object model | 210 VB6 objects: 132 Forms, 66 DataReports, 9 Modules, 3 Classes; 8,026 declared methods | `objects-full__[analysis_BATCH-04_vb6_v1605].txt` | VERIFIED |
| SQL corpus | 1,797 statements (1,381 SELECT, 241 ALTER, 72 DELETE FROM, 49 OTHER, 27 CREATE, 27 SHAPE) | `statements__[analysis_BATCH-04_sql_v1605].csv` | VERIFIED |
| Embedded DDL | 268 statements; shipped database out of step with 38 Access-applicable changes | `ddl-evidence__[analysis_BATCH-04_sql_v1605].txt` + prior docs | STRONGLY_SUPPORTED |

## The decisive evidence constraint

**The v1.605 executable is not present in `D:\Collective work`.** The 59,150,336-byte `real lab system.exe` in the repository is **version 1.00** (SHA `12828D0A…`), a comparative baseline. The 1.605 binary was exhaustively searched for — by filename, size, hash, and inside all 8 ZIP archives — and found only as *extracted data*: the PE dump `real lab system.exe (2).json`, VB6 object dumps, the SQL statement corpus, DDL evidence, and string/function inventories.

Consequently:
- All executable findings about v1.605 are **derived from primary extraction outputs of that binary**, not from a fresh decompilation performed in this run.
- **No runtime observation** of any target binary was performed or is claimed.
- The v1.00 binary was available for structural comparison but was also analyzed statically only.

This constraint is documented rather than papered over; it is the principal determinant of the achieved coverage.

## Evidence repository

- Path: `D:\Collective work`
- **973 files**, flat directory, **0 subdirectories**, **0 zero-byte files**
- Composition: 298 `.md`, 236 `.csv`, 150 `.json`, 107 `.txt`, 75 `.py`, 37 `.exe`, 18 `.log`, 10 `.sql`, 9 `.sha256`, 8 `.sh`, 8 `.zip`, 5 `.png`, 3 `.emf`, 3 `.wmf`, 3 `.java`, 2 `.xlsx`, 1 `.tsv`
- All 973 hashed with SHA-256; duplicates identified (not discarded — location/context can be meaningful)
- No original `.mdb` or `.accdb` files; database reconstruction relies on extracted DDL and table CSVs
- No nested archives inside the 8 ZIPs (4,395 entries scanned)

## Method

1. Read the full infrastructure audit and independently re-probe every relevant tool.
2. Inventory and hash the complete evidence repository.
3. Parallel deep-dives across four disjoint domains: executable/code, database, application-to-database + configuration, business domain + cross-source evidence.
4. Reconstruct behavior from primary dumps with explicit VERIFIED / STRONGLY_SUPPORTED / INFERRED / UNVERIFIED / CONFLICTED / UNKNOWN labels.
5. Iterate on gaps; record every failure, negative finding, and unresolved limitation.
6. Consolidate into a single auditable knowledge repository with an evidence-based completion matrix.

## Repository map

See `Final_Reverse_Engineering_Report.md` and `Evidence_Coverage_Report.md` in this directory, and the 12-directory structure rooted at `REAL_LAB_1605_REVERSE_ENGINEERING_OUTPUT\`.

## Integrity statement

No file under `D:\Collective work` was modified, renamed, deleted, or executed. No database connection was made. No secret value or patient-identifying information is reproduced in any output document. Every important conclusion is traceable to a named evidence artifact.
