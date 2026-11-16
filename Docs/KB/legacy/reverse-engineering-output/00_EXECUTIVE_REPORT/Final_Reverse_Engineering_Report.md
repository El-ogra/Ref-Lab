# Final Reverse Engineering Report — REAL LAB SYSTEM v1.605

**Mission:** full-spectrum static reverse engineering, behavioral reconstruction, database forensics and engineering knowledge consolidation.  
**Evidence repository:** `D:\Collective work` (973 files, all inventoried and hashed).  
**Repository:** `C:\Users\LAP LINK\source\repos\Ref Lab\REAL_LAB_1605_REVERSE_ENGINEERING_OUTPUT`.  
**Date:** 2026-11-16.

---

## 1. How many relevant artifacts were discovered?

**973 files** (298 `.md`, 236 `.csv`, 150 `.json`, 107 `.txt`, 75 `.py`, 37 `.exe`, 18 `.log`, 10 `.sql`, 9 `.sha256`, 8 `.sh`, 8 `.zip`, 5 `.png`, 3 `.emf`, 3 `.wmf`, 3 `.java`, 2 `.xlsx`, 1 `.tsv`), all in one flat directory, 0 zero-byte files. All hashed with SHA-256; 22 duplicate-hash groups (58 files) identified. The 8 ZIPs contain 4,395 entries, none of which is a nested archive.

## 2. How many executable components were identified?

**37 `.exe` files**, classified as: 13 Macromedia Flash Player 8 projector simulations (`001.exe`–`012.exe`, `007a.exe`); 1 v1.00 main application (`real lab system.exe`, 59,150,336 B); 1 v1.605 installer (`RL_SETUP.exe`, 23,060,480 B); 8 target auxiliary tools (Image Gallery, Patient Data Transport, E-Mail Sender, Show_Info, RLS ACCESS/SQL Database Tools, RLUTool, real lab timer, calculator); 3 shipped remote-access tools (AnyDesk, TeamViewerQS, Supremo — **not target logic**); 11 third-party/installer components (Bullzip, Ghostscript, Xpdf, PDF Power Tool, .NET 4, MDAC, acmsetup, setupsp6, xcopy, setup.exe, dotNetFx40).

## 3. How many executable components were successfully analyzed?

All **37** were fingerprinted (version resource, size, SHA-256). The **v1.00 main executable** and the **support components** were analyzed structurally from PE dumps and version data. The **v1.605 main executable was not available** — only its extracted dumps; therefore it was analyzed at the dump level (PE structure, 210 objects, 8,026 methods, 1,797 SQL statements, 268 DDL, 270,166 literals per prior work), not at the byte level in this run.

## 4. Which programming languages and runtime technologies were confirmed?

- **Visual Basic 6 native** PE32/i386 for the main application and most tools (`MSVBVM60.DLL`, 231 imports in v1.605; `VB5!` header; runtime build 9782/SP6 per prior work).
- **VB.NET / .NET Framework 4.0** for the auxiliary `RealLab E-Mail Sender.exe` (not the main app).
- **Data technologies:** ADO 2.7 (`msado27.tlb`), `MSDataShape.1`, `SQLOLEDB.1`, Jet 4 / Access `.mdb`, optional SQL Server.
- **Native Win32 APIs:** printer spooler, profile (INI), serial/COM, Winsock.
- No C/C++ source, no Java, no web stack in the target.

## 5. How many assemblies, classes, methods, and code structures were recovered?

- VB6 **objects: 210** (132 Forms, 66 DataReports, 9 Modules, 3 Classes).
- **Declared methods: 8,026**; only 1,268 method entries addressable (15.8%); **0 method names recovered** for these entries.
- **String literals: 270,166** (prior-work count from the v1.605 binary).
- **SQL statements: 1,797**; **embedded DDL: 268**; **tables referenced in SQL: 117**.
- Native function inventory for v1.00: **6,522 functions**, of which 6,515 are `FUN_*` placeholders and 7 named (runtime thunks) — confirming that native decompilation cannot recover VB6 business logic.
- .NET assemblies: not applicable to the target main app.

## 6. What was reconstructed about internal architecture?

Two-tier fat-client: 132 VB6 forms + 66 DataReports + ActiveX controls → VB6 modules (`labdb`, `labdb1`, `labfunction`, `printerselect`, `BCWFonts`) → ADO/MDAC hierarchical recordsets → Access/Jet and/or SQL Server → DataReport/Win32 spooler/Bullzip PDF. Configuration via INI/UDL; deployment via VB6 Setup Wizard (`RL_SETUP.exe`, WIN7/WINXP CABs, `SETUP.LST` 39 files). Details in `10_UNIFIED_KNOWLEDGE_BASE\MASTER_SYSTEM_ARCHITECTURE.md`.

## 7. How many database artifacts were identified?

**4 Access databases** (`Patients.mdb` v1.605 103/1,682, v1.00 93/1,527, `Patients_backup.mdb` 90/1,381, `Branch_Data.mdb` 49/969/0 rows, `data.mdb` 3/34 — counting `Patients.mdb` per version), **2 SQL Server catalogues** (`patients`, `Interface`), **1 referenced-but-uncharacterized `VIDAS`**, plus `data_connection*.udl` files and all extracted schema/DDL/CSV dumps. **No original `.mdb` file is present locally.**

## 8. What database technologies were discovered?

Microsoft Access **Jet 4** (primary shipped store), **SQL Server** (optional central store, `SQLOLEDB.1`, backup device commands), Jet/ACE OLEDB providers, and `MSDataShape.1` hierarchical shaping. No SQLite/MySQL/Access-97-only engine is established for the target; a MySQL hypothesis remains UNVERIFIED.

## 9. How much of the database structure was recovered?

**103/103 tables and 1,682/1,682 columns (100%)** for the v1.605 shipped schema, plus the v1.00 baseline (93/1,527). Relationships reconstructed from 60 explicit joins and 27 `SHAPE … RELATE` commands; **no declared PK/FK/UNIQUE/CHECK/DEFAULT/indexes exist in any dump**. Data-page content, indexes and query plans were not available (no `.mdb`).

## 10. How much of application-to-database interaction was reconstructed?

**1,797/1,797 SQL statements** classified by verb, VA, target tables and textual content; verb distribution SELECT 1,381 / ALTER 241 / DELETE 72 / OTHER 49 / CREATE 27 / SHAPE 27; **0 INSERT INTO and 0 UPDATE … SET** — a verified architectural finding (bound-recordset writes). Object→table mapping is structural/inferred; **0 statements could be bound to a named VB6 method** because method bodies/names are absent from the evidence.

## 11. Which business rules were recovered directly from executable code?

- Result lifecycle fields and audit pairs (`resultfinsh`/`edituser`/`editdate`, `resultreview`/`prevuser`/`prevdate`, `resultprint`/`printuser`/`printdate`, all at VAs `681ED2`, `6860BE`, `686312`, `6865A6`).
- **Financial aggregate formula with rounding (VA `69EA76`):** `Round(sum(totalprice),2)`, `Round(sum(totalpricedisc),2)`, `Round(sum(totalprice)-sum(totalpricedisc),2)`, `Round(sum(elmadfoo3),2)`, `Round(sum(elbakyma3mal),2)` by `pricemethod` — net = gross − discount, 2-decimal rounding.
- Per-test pricing (`patientprice`, `labprice`, `otherlabprice`, VA `6A9632`), send-out flag `inotherlab`.
- Reference-range query (`normalrang<>''`, sex/age at VA `672E3E`), hierarchical catalogue (VA `672722`).
- **64-table patient-deletion cascade** (VAs `6A740A`+).
- 268 embedded DDL with partial application to shipped DB.
- Backup device management (`sp_addumpdevice`/`sp_dropdevice`, VAs `66FCB2`/`66FD56`).

## 12. Which behaviors were independently observed at runtime?

**None.** No target binary was executed and none is available for v1.605. All behavioral content is static reconstruction with explicit confidence labels. This is the mission's hard limitation, documented in `07_RUNTIME_AND_BEHAVIORAL_ANALYSIS\Runtime_Observations.md`.

## 13. Which previous reverse-engineering findings were confirmed?

- Target identity/version/size/hash; VB6 native PE32; 210 objects/8,026 methods; 1,797 SQL; 268 DDL; 103/1,682 schema; testtable 609; normalrange 852; 66 DataReports; Bullzip/Ghostscript/Xpdf/PDF Power Tool stack; 13-char patient ID; `patients` = operator table; v1.00→v1.605 delta 10 tables/155 columns; 41 objects added; CHM byte-identical; installer details; 22.95 MiB embedded payload characterization.

## 14. Which previous findings were corrected or expanded?

- **PE timestamp:** prior docs claim both builds carry 2013-12-27; primary dumps show v1.605 `timestamp=1595435962` → **2020-07-22**, v1.00 `1388267701` → 2013-12-28. Raw dump preferred (CONFLICTED).
- **Column delta:** prior `00-baseline-summary.md` says +90 columns (1,617 total); primary dumps give **+155 (1,682)** — primary preferred.
- **Statement counts:** a prior report's 1,370 SELECT/14 CREATE/120 ALTER contradicts the BATCH-04 CSV's 1,381/27/241 — CSV preferred; both denominators retained.
- **Financial formula:** prior work treated formulas as unknown; the aggregate formula and 2-decimal rounding were recovered at VA `69EA76`.
- **Write path:** prior work did not emphasize the 0 INSERT/0 UPDATE finding; it is now an architectural conclusion.
- **Database constraints:** prior docs implied schema constraints; primary DDL shows **none declared**.

## 15. What contradictions were discovered?

- PE timestamp (above).
- Column-count delta (+90 vs +155).
- "17 new tables" (SQL-referenced delta) vs **10 new tables** (schema delta) — different denominators, not necessarily an error.
- DDL counts (14/120 vs 27/241).
- `column-evolution.csv` is a migration-DDL view, not a full-schema comparison (conflicts on `bloodgases`).
- Jet 3 vs Jet 4 descriptions; `D:` vs `E:\Lab_data` paths (installation-dependent).
- Prior `behavioural-analysis.md` describes a Linux/Kubernetes sandbox, not the present Windows host — reasoning reused, environment facts rejected.
- An earlier output claimed "17 phases COMPLETE" while documents were stubs; corrected in `11_INVESTIGATION_LOGS\Resume_Checkpoint.md`.

## 16. Which knowledge gaps remain?

Composite-test component storage; per-invoice financial computation and `discount100` semantics; VIDAS structure; RL_SETUP 22.95 MiB payload; original MDB data pages; grant-bit semantics; 5 undecoded external component names; most VB6 method names; complete call graph; DDL migration trigger; all runtime behavior.

## 17. Why does each remaining gap exist?

- **Missing primary artifacts:** v1.605 binary and original `.mdb` files are not in the repository — no byte-level or data-page analysis is possible.
- **Information not stored in evidence:** method names and bodies were not preserved by the prior extraction (1,268/8,026 addressable, 0 names).
- **Uncharacterized references:** VIDAS and the embedded installer payload are referenced but not decomposed.
- **Safety/environment:** no isolated disposable Windows VM with VB6 runtime, OCX set, synthetic DB and mock analysers; running the host's binaries would risk cross-contamination.
- **Tooling:** REA Ghidra provider blocked (Java fault at server start); REA namespace became unavailable after a server restart; direct Ghidra works for present binaries but cannot analyze absent ones.

## 18. Which available tools were actually used?

PowerShell 7.6.10 (inventory, hashing, CSV mining, version info), .NET `ZipFile` (archive enumeration), Python 3.13.1 (schema parsing helper), Node/Code Mode `execute` (parallel tool orchestration), REA `binary_session` (capability verification before namespace loss), Ghidra 12.1.4 headless (direct probe; job cancelled by server restart), `git`, `sqlcmd`/`psql` (availability checks only), grep/read/write/edit, and subagents (4 original + 4 duplicate, duplicates stopped). Full list in `01_ENVIRONMENT_AND_TOOLS\Tool_Usage_Log.md`.

## 19. Which investigation activities were blocked and why?

- Native decompilation of v1.605 — binary absent; REA provider blocked.
- Runtime observation — binary absent; no isolated environment; safety rules.
- Data-page/query-plan analysis — no `.mdb` files.
- Per-method call graph — method names/bodies absent from evidence.
- VIDAS characterization, installer payload decomposition — no artifacts/incomplete decomposition.

## 20. Where is the completed knowledge repository located?

`C:\Users\LAP LINK\source\repos\Ref Lab\REAL_LAB_1605_REVERSE_ENGINEERING_OUTPUT\` — 12 directories: executive report, environment/tools, source inventory, executable RE, database forensics, application–database mapping, configuration/dependencies, runtime/behavioral analysis, business domain, cross-source evidence, unified knowledge base, investigation logs.

---

## Professional assessment of reverse-engineering maturity

This investigation achieved **complete coverage of the evidence that exists locally** and **high but not total reconstruction of the target system**. The dominant limiting factor is not analytical effort or tooling but the **absence of the v1.605 executable and original databases** from the evidence repository. Within that boundary:

- **Structural maturity: high.** PE, object model, schema, dependency and configuration layers are fully reconstructed from primary dumps.
- **Data-access maturity: high.** All 1,797 SQL literals are classified; a verified write-path conclusion (bound recordsets) and a verified financial formula were recovered.
- **Business-logic maturity: medium-high.** Field-level and SQL-level rules are strong; algorithms behind composite results and per-invoice calculations remain open.
- **Behavioral maturity: medium.** Workflows are coherently reconstructed but **zero runtime verification** exists; all behavioral claims are explicitly static.
- **Evidence maturity: high.** Every major finding is traceable to a named artifact, with confidence labels, contradictions and limitations preserved.

No single overall percentage is asserted; the separate, denominator-backed measurements are in `Evidence_Coverage_Report.md`. The repository is independently auditable and honestly bounded.
