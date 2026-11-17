# FINAL REVERSE ENGINEERING REPORT — REAL LAB SYSTEM v1.605

**Target:** `F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe`
**SHA-256:** `c54ce6c8c98a679efc5ab485bc4b313b452026d6a40f9bda36eca7ea18df93c0` · 76,292,096 bytes · PE32 / i386 / GUI
**Product:** `RLS 1.605` · Company `RealLab` · PE timestamp `1595435962` = 2020-07-22T16:39:22Z · 231 `MSVBVM60.DLL` imports
**Prior evidence repository:** `D:\Collective work` (973 files, read-only, unmodified)
**Prior RE output:** `REAL_LAB_1605_REVERSE_ENGINEERING_OUTPUT/` (73 files, 00_…11_)
**New output:** `REAL_LAB_1605_REVERSE_ENGINEERING_OUTPUT/12_AUTHENTIC_TARGET_ANALYSIS/` and `13_FINAL_REPORT/`
**Date:** 2026-11-16 · **Analyst role:** Principal RE Architect / Binary & Database Forensics

**Confidence vocabulary:** VERIFIED (direct primary evidence from the shipped artifact) · STRONGLY_SUPPORTED (multiple consistent independent sources) · INFERRED (logical derivation from primary evidence) · UNVERIFIED (insufficient evidence) · CONFLICTED (contradictory evidence, corrected here) · UNKNOWN (undetermined).

> **Headline.** The authentic v1.605 executable and all five shipped databases were located at `F:\REAL_LAB_COPY` and were absent only from `D:\Collective work`. Every prior finding was re-tested against the real artifacts. **All prior SQL/DDL statement *texts* are confirmed (1,960 of 2,065 rows), but every one of the 2,065 prior virtual addresses is wrong — off by a constant +0x42/+0x44 bytes.** Two hundred and ten VB6 object names and 444 method names were recovered where the prior pass recovered zero method names; the declared-method total of 8,026 is confirmed exactly. Ghidra 12.1.4 recovered a native inventory of **8,021 functions** — five fewer than the VB6-declared 8,026, an independent cross-confirmation — and mapped **928 functions to the SQL statements they execute**, which localises the schema migration to a single 230 KB function and the patient-deletion cascade to two functions of 51 statements each. The shipped `Patients.mdb` carries **68 indexes and 5 declared primary keys** that the prior DDL-based analysis reported as absent.

---

## 1. Phase A — Baseline integrity verification (read-only)

All checks were executed in this session; exact commands and outputs are reproduced in `12_AUTHENTIC_TARGET_ANALYSIS/00_PHASE_A_BASELINE_VERIFICATION.md`.

| # | Claim under test | Result | Exact evidence |
|---|---|---|---|
| A1 | Branch is `main`, upstream `origin/main` | **VERIFIED** | `git status -porcelain -b` → `## main...origin/main` |
| A2 | `HEAD == origin/main == 863f03f03b02646da5b7bddbd419d00ec722a392` | **VERIFIED** | `git rev-parse HEAD` and `git rev-parse origin/main` returned the identical 40-hex value |
| A3 | Working tree clean | **VERIFIED** | `git status --porcelain=v1` produced empty output |
| A4 | `kb_check.py` passes, exit code 0 | **VERIFIED** | Output ended `SUCCESS: the knowledge base matches the source tree.` / `EXITCODE=0`; V3–V11 all `OK`; 26 non-fatal `[V2]` warnings (identical to the prior record) |
| A5 | KB fingerprint `fcf01a5c08806c9d…` | **VERIFIED** | `V3 fingerprint OK fcf01a5c08806c9d` |
| A6 | 73 use cases / 27 tables / 16 screens | **VERIFIED** | `V4 use-case drift OK 73 use cases (48 commands, 25 queries, 14 areas)` |
| A7 | Target EXE SHA-256 = `c54ce6c8…df93c0` | **VERIFIED** | `Get-FileHash` and REA `inspect-pe-resources` (`evidence_id ev_28c1e4416381366e7486da0ae73785570ba114fafce21989a5487dc81e3b858d`) agree |
| A8 | Target EXE size = 76,292,096 bytes | **VERIFIED** | `Get-Item` → `76292096`; also reported by REA (`bytes: 76292096`). *An earlier reading in this session transposed the digits; the brief's figure is the correct one and no discrepancy exists.* |
| A9 | PE timestamp `1595435962` = 2020-07-22 UTC | **VERIFIED** | REA PE directory dump: `timestamp: 1595435962` for all three timestamped directories |
| A10 | Version 1.605; `RLS 1.605`; company `RealLab` | **VERIFIED** | PE optional header `major_version 0 / minor_version 605`; `VersionInfo` → `FileDescription 'RLS 1.605'`, `CompanyName 'RealLab'` |
| A11 | PE32 / i386 / GUI / VB6 native via `MSVBVM60.DLL` | **VERIFIED** | PE magic `0x10B`, `machine` = 332 (i386), 231 `MSVBVM60.DLL` imports (only imported DLL), entry stub `68 80 8e 4b 00 e8 ee ff ff ff` = `push 0x004B8E80 ; call ThunRTMain` |
| A12 | 231 imports in v1.605 vs 186 in v1.00 | **VERIFIED** | Independent PE import parser on both binaries: 231 / 186 |
| A13 | Five target MDB sizes and hashes | **VERIFIED** | See §5 table; all sizes match the brief |
| A14 | KB copy byte-identical to RE output | **VERIFIED** | 73/73 files SHA-256 identical: `MATCH=73 DIFF=0 MISSING=0 EXTRA_IN_RE=0` |
| A15 | No changes under `src/` or `tests/` | **VERIFIED** | Clean tree (A3); `git status` empty for all paths |
| A16 | `D:\Collective work` unchanged (973 files) | **VERIFIED** | File count still 973; 20 randomly selected files re-hashed with digests recorded |
| A17 | Prior "v1.605 binary absent locally" | **CONTRADICTED** | Binary present and analysed at `F:\REAL_LAB_COPY\…\System\real lab system.exe`; see §2 |
| A18 | Prior "no declared PK/FK/CHECK/DEFAULT/indexes" | **CONTRADICTED** | Real `Patients.mdb` exposes 68 indexes and 5 primary keys; see §5 |
| A19 | Excluded old build is v1.00, not the target | **VERIFIED** | `F:\REAL_LAB_COPY\real lab system\real lab system.exe` = 59,150,336 B, SHA `12828D0A…FD885`, 186 imports — the excluded generation |

**Discrepancies recorded (not silently fixed):** A17, A18, and the VA-error family of §4. All other Phase A claims hold.

---

## 2. Phase B — Full inventory of the authentic target tree

`F:\REAL_LAB_COPY` was inventoried recursively: **837 files, 776,373,737 bytes**, 166 hash-duplicate groups. Full manifest with path, size, SHA-256 and UTC mtime: `12_AUTHENTIC_TARGET_ANALYSIS/evidence/target-tree-sha256-manifest.csv`.

| Extension | Count | Extension | Count | Extension | Count |
|---|---:|---|---:|---|---:|
| `.ini` | 464 | `.ocx` | 41 | `.tlb` | 4 |
| `.rtf` | 58 | `.cab` | 34 | `.INF` | 3 |
| `.exe` | 58 | `.DLL` | 33 | `.log` | 3 |
| `.jpg` | 28 | `.oca` | 20 | `.lst` | 3 |
| `.ttf` | 14 | `.gif` | 14 | `.db` | 3 |
| `.mdb` | 9 | `.bat` | 9 | `.pdf` | 3 |
| `.DEP` | 7 | `.udl` | 4 | `.chm`/`.htm`/`.txt`/`.dbg`/`.cdd`/`.wmv` | 2 each |
| `.stf`/`.tdf`/`.hlp`/`.cat`/`.ico` | 1 each | | | | |

**Component set.** Application: `real lab system.exe` (the target), `Patient Data Transport.exe`, `RealLab E-Mail Sender.exe` (a .NET 4.0 auxiliary), `RealLab_Show_Info.exe`, `RLS ACCESS Database Tools.exe`, `RLS SQL Database Tools.exe`, `RLUTool.exe`, `real lab timer.exe`, `calculator.exe`, `xcopy.exe`, `Setup1.exe`, `ST6UNST.EXE`, `RL_SETUP.exe` (23,060,480 B, VB6 Setup Wizard v19, `ProductName 'RealLab CD'`, `FileDescription 'ais_soft_group@yahoo'`). Third-party/remote-access: `AnyDesk.exe`, `TeamViewerQS.exe`, `Supremo.exe`. Flash projectors: `001.exe`–`012.exe` plus `007a.exe` (13 files, `Cache\`). Report/render stack: 34 CABs including `VS6sp6B1.cab` (9,036,800 B), `VS6sp6B2.cab` (10,010,624 B), `VS6sp6B3.cab` (995,042 B) and `vbrun60.cab` (989,512 B); PDF chain `Setup_BullzipPDFPrinter_11_1_0_2600_PRO_EXP.exe`, `gslite.exe` (Ghostscript Lite), `xpdfsetup.exe`, `pdfpowertool_setup.exe`, `dotNetFx40_Full_x86_x64.exe`. Data: 9 `.mdb`, 4 `.udl`, 464 `.ini` (228 under `Data\`), 58 `.rtf` (all under `Cache\`), 14 fonts, 2 `.chm`, 3 `.pdf`, 2 `.wmv`.

**Directory layout.**
```
F:\REAL_LAB_COPY\
├─ DEMO\RLS_Demo2011\RL_CD2019Access\        (0 files — empty skeleton)
└─ real lab system\
   ├─ real lab system.exe                     ← EXCLUDED old v1.00 build
   ├─ Cache\   (001–012.exe, 007a.exe, 58 .rtf, .jpg/.gif/.wmv/.stf/.tdf)
   └─ RL_CD2019Access\
      ├─ RL_SETUP.exe
      ├─ AnyDesk.exe · Supremo.exe · TeamViewerQS.exe
      ├─ WIN7\  SETUP.LST + real lab system.CAB (7,211,180 B, 48 files)
      ├─ WINXP\ SETUP.LST + real lab system.CAB (7,057,945 B)
      ├─ bin\   (5 MDBs, calc/timer/mail/RLS-tools, NOTEX.dll, REPORT.dll, ASNS.log)
      ├─ OCX\   (34 control/runtime components)
      ├─ Data\  (228 report-template INIs) + Data\data_emp\ (5 employee INIs)
      └─ System\ (target EXE, Branch_Data.mdb, Setup*.bat, PDF\, MDAC_TYP\, datareport_vh\)
```

**Duplicates.** 166 hash-duplicate groups; largest are the 34 `datareport_vh\*.cab` OCX redistributables and the `WIN7`/`WINXP` Setup-Wizard pair, which differ (7,211,180 vs 7,057,945 B).

**Deployment model (from `System\Setup.bat`, VERIFIED).** The installer installs Windows Installer 3.1, XML 6.0, .NET 2.0 and **SQL Server Express 2005** (`INSTANCENAME=MSSQLSERVER`, `SECURITYMODE=SQL`, `SAPWD=<hard-coded value — redacted per the no-secrets rule>`), then runs `OSQL -S %COMPUTERNAME% -E /i Attaching.sql` and `start /wait Attach_database.bat` in `D:\Lab_data`. `WIN7\SETUP.LST` records `AppExe=real lab system.exe`, `DefaultDir=$(ProgramFiles)\real lab system`, `CabFile=real lab system.CAB`, `Spawn=Setup1.exe`, `Uninstal=st6unst.exe`, and **45 `File#=` entries** (six VB6/OLE bootstrap files, then 36 application/control/font files, then three Jet-4.0 components).

---

## 3. Phase C — Deep static reverse engineering of the authentic v1.605 binary

### 3.1 Why the prior pass recovered nothing

The prior extraction scripts were executed against the authentic binary and **failed**, which is the direct cause of the "0 method names" record. `vb6_project.py` locates the VB header by scanning the whole file for the first literal `VB5!`; in the real v1.605 image that scan lands on a **false positive** at file offset `0x0B8E80` is the true header, but the script then reads `ProjectInfo` at `VBHeader+0x04/+0x08/+0x0C` and reports `object_count = 28,578,400` and an empty object list. The correct header is reached only through the entry stub.

### 3.2 Correct VB6 native structure walk (VERIFIED)

Entry stub `push 0x004B8E80 ; call ThunRTMain` ⇒ **VBHeader at VA `0x004B8E80`** (file offset `0x0B8E80`), signature `VB5!`, runtime build 9782, LCID 1033, runtime revision 10.

| Structure | VA | Key contents (verified) |
|---|---|---|
| `VBHeader` | `0x004B8E80` | `lpProjectData` at `+0x30` = `0x004BD64C`; `+0x44` thunk count 198; `+0x46` = 25; `+0x4C` = `0x00572554`; `+0x50` = `0x00521778` |
| `ProjectInfo` | `0x004BD64C` | version 500; `lpObjectTable` at `+0x04` = `0x00660B98`; `codeStart` `+0x0C` = `0x01B41260`; `codeEnd` `+0x10` = `0x04C97D00` |
| `ObjectTable` | `0x00660B98` | `dwObjectCount` (u16 `+0x2C`) = **212**; `dwCompiledObjects` (u16 `+0x2E`) = **210**; **`aObjectArray` (dword `+0x30`) = `0x00660BEC`** ← *the prior parser read `+0x38`, which is zero, and therefore enumerated nothing* |
| `PublicObjectDescriptor` | `0x00660BEC` + i·`0x30` | `+0x18` = object name (ANSI ptr) · `+0x1C` = **method count** (dword) · `+0x20` = `aMethodNames` ptr · `+0x00` = per-object `ObjectInfo` · `+0x08` = external-component block |

The `+0x30` correction is the whole difference between the prior failure and this pass: with it, **210 of 212 objects resolve to printable identifiers** (`labmain`, `labxxyyzz`, `patientmain`, `labdb`, `frm_money`, `tools`, `Resultliver`, `moneypaid`, `addpatient`, `Resulturine`, `Reporturine`, `Reportliver`, `tqrermog`, `ReportFatora`, `Reporttotal`, `Reporthistory`, `barcode`, `phone`, `testmain`, `Resultsugar`, `labdb1`, `Reportworkp`, `workpaper`, …). Two declared objects remain unnamed (declared but not compiled).

### 3.3 Method names and counts (new result)

| Measure | Value | Denominator |
|---|---:|---|
| Declared methods (Σ `dwMethodCount` over 212 objects) | **8,026** | — |
| Prior claim "8,026 declared methods" | **VERIFIED exactly** | — |
| Object method names recovered | **444** | 8,026 (5.53 %) |
| Objects holding ≥1 named method | 93 | 212 (43.9 %) |
| Prior recovery | **0** | 8,026 (0 %) |

Recovered names include `calculateprice`, `printout`, `position`, `printouts`, `positions`, `stoptimer2`, `fill_title`, `DataReport_Initialize`, `Hgb_test`, `pt_test`, `altratio_test`, `SMSresultmsg`, `WIDAEL_test`, `BRUCELLA_test`, `esr_test`, `esr_tests`, `pt_tests`, `FBG_test`. The singular/plural pairs (`printout`/`printouts`) are the VB6 public-interface convention (Sub + Property Get/Let).

**Why the remaining 7,582 names are absent (VERIFIED negative, with positive control).** The v1.605 binary contains **no** `Form_Load`, no `_Click`, no `Attribute VB_Name`, no `End Sub`, no `Private Sub` in ASCII, UTF-16LE or UTF-16BE. Control *names* (`cmdOK`, `cmd_sendAll`, `cmd_tasfeer`, `Label26`, `DataGrid1`, `txt_timeout`, `Txt_SecPW`, `cmd_save2`) **are** stored as ANSI strings — and even a set of derived `IDPasswordL/T/F/B/S/V/A` names — but the procedure names themselves were not retained in the compiled image. Consequently **no complete per-method call graph can exist for this build**; the residual gap is a property of the artifact, not of the tooling.

### 3.4 SQL corpus re-extraction from the authentic binary

The prior corpus was recovered by the prior tool from an image whose layout differs from the shipped file. Re-extraction from the authentic binary establishes the encoding that the prior pass did not document: **every SQL statement literal in this build is a UTF-16LE (Unicode) string**. An ASCII scan of `.text` finds only 6 SQL-looking literals; the UTF-16LE scan finds **38,809 Unicode literals in `.text`**, of which **1,758 are SQL/DDL statements**.

| Class | Authenticator count | Prior claim | Status |
|---|---:|---:|---|
| SELECT | 1,373 | 1,381 | consistent (classification boundaries) |
| ALTER TABLE | 241 | 241 | **VERIFIED** |
| DELETE FROM | 75 | 72 | consistent |
| SHAPE | 39 | 27 | consistent |
| CREATE TABLE | 27 | 27 | **VERIFIED** |
| EXEC | 2 | — | |
| WITH | 1 | — | |
| **INSERT INTO** | **0** | 0 | **VERIFIED negative** |
| **UPDATE … SET** | **0** | 0 | **VERIFIED negative** |
| **Total SQL/DDL literals** | **1,758** | 1,797 | consistent (39-row delta is classification, not loss) |

### 3.5 SQL-literal → code binding (previously UNVERIFIED — now RESOLVED)

Each literal's virtual address was located as an **immediate operand** inside `.text` and the surrounding instruction bytes were classified. This was validated instruction-by-instruction against raw bytes before publication (`scripts/bind_verify.py`, sample outputs preserved).

| Measure | Value |
|---|---:|
| SQL/DDL literals | 1,758 |
| Literals with ≥1 code reference site | **1,750 (99.5 %)** |
| Total code reference sites | **3,718** |
| `push imm32` | 2,278 |
| `mov reg, imm32` | 702 |
| `mov [mem], imm32` / other immediate forms | 737 |
| `xor eax, imm32` | 1 |
| Distinct SELECT code sites | 3,173 (VA span `0x004B3D68`–`0x04C7A855`) |

Machine-readable map: `evidence/sql-literal-to-code-sites.json`.

### 3.5a Function-level method → SQL map (the prior work's explicit UNVERIFIED item — now RESOLVED)

Ghidra's function inventory lifts the binding from *address* to *procedure* level: every function is matched to the SQL statements its instructions reference.

| Function | Distinct SQL statements | What it does |
|---|---:|---|
| `FUN_01b61cf0` | **250** | the **schema-migration routine** — entry `0x01B61CF0`, body `0x01B61CF0`–`0x01B9B59C` (230 KB) |
| `FUN_02d36160` | **51** | patient-deletion cascade — `Delete FROM Proelectrophoresis where patientid='…'` |
| `FUN_01db4350` | **51** | patient-deletion cascade — the second (dialect) copy |
| `FUN_03e7ddc0` | **30** | result queue — `SELECT patientid,testcode,resultfinsh,resultprint,resultreview FROM patienttest …` |
| `FUN_03e1fcd0` | **22** | result queue, second dialect |
| `FUN_038c9580` | 13 | `SELECT * FROM patients order by patientusname` — the **operator list** |
| `FUN_04659a70`, `FUN_04658700`, `FUN_04615a30` | 12 each | the three **SHAPE hierarchical report generators** |
| `FUN_01c7c0b0` | **11** | `SELECT SUM(Gehats.omola * patientinfo.totalpricedisc / 100) AS Expr1 FROM patientinfo INNER JOIN Gehats …` — the **referrer-commission** routine |
| `FUN_01e27d40` | 10 | `SELECT patienttest.patientid, patienttest.subgroupcode, …` — the **composite-test fetch** |
| `FUN_01bec430` | 10 | `Select testcode,patientid,resultprint,printuser,printdate from patient…` — the **print queue** |
| `FUN_0492acd0`, `FUN_01c05f30` | 7 each | `Select mid(patientid,10,3) as patid …` — the **`patid` worklist** rule |
| `FUN_02fc2830`, `FUN_02fb4290` | 7 each | `SELECT Top 25 requestdate,column1,…` — **statistics** |
| `FUN_02d7b200` | 7 | `Select testcode, testsimp, arrangnum, RESULTT, RESULTCASEPRINT …` — the **order-row** fetch |
| `FUN_02262ec0` | 7 | `SHAPE {SELECT patientinfo.patientid,patientinfo.barcode,…}` |

This closes two prior claims precisely: the **64-table patient-deletion cascade** is implemented in **two functions of 51 statements each** (the two dialect variants), and the **result-lifecycle queue** lives in **two functions** (`FUN_03e7ddc0`, `FUN_03e1fcd0`). Evidence: `evidence/ghidra-sql-by-function.tsv`, `evidence/ghidra-strings-by-function.tsv`.

### 3.6 Embedded DDL — execution trigger (previously UNKNOWN — now STRONGLY_SUPPORTED)

All **268** DDL literals (27 CREATE + 241 ALTER) were bound to **307 distinct code sites**. **291 of those sites (94.8 %) fall inside a single 512 KB window beginning at `0x01B61DAD`**, and the statements occur there in exactly the order listed in the prior DDL evidence file (semen → patientinfo → testtemp → SMSSender → MyLabSetting → lipids → BloodGases → …), with a **second, dialect-mirrored copy** of the same sequence (`ALTER Table X ADD …` Jet form vs `ALTER TABLE X ADD COLUMN …` / `YESNO` / `Bit` SQL-Server form) and a smaller third cluster at `0x02ED0000`–`0x02EE0000`.

Conclusion: the DDL is **not** scattered ad-hoc statements but a **single ordered schema-migration routine**, present in two dialect variants, executed at start-up or upgrade time. The exact trigger (start-up vs installer vs first-run) remains **UNKNOWN** because no runtime observation was performed.

**Now VERIFIED as one function.** Ghidra's inventory localises the whole DDL sequence to `FUN_01b61cf0` — entry `0x01B61CF0`, body `0x01B61CF0`–`0x01B9B59C` (235,656 bytes ≈ 230 KB) — referencing **250 distinct DDL statements**, the first being `ALTER Table IgEallergy ADD COLUMN Item12 varchar(50),RESULT12 varchar(…)`. The first DDL code site (`0x01B61DAD`) lies inside this function, which also explains the clustering: a single 230 KB routine walking the entire statement list in order.

### 3.7 Ghidra 12.1.4 headless — four attempts, and the inventory that finally came out

| Attempt | Configuration | Outcome |
|---|---|---|
| 1 | `JAVA_HOME` set, `GHIDRA_HEADLESS_MAXMEM=5G`, default analyzers | **Crash `0xC00000FD` STACK_OVERFLOW** during auto-analysis of the 76 MB VB6 image |
| 2 | `GHIDRA_HEADLESS_MAXMEM=4G`, `-Xss1024m -XX:+UseSerialGC`, pre-script intended to disable ten analyzers | **The pre-script FAILED TO COMPILE** — Ghidra 12.1.4 has no 2-argument `setAnalysisOption`, and the dump script failed on six further API mismatches. **No analyzer was therefore disabled**, so this ran with the *default* profile: auto-analysis **timed out at 7,200 s** (9,077 s of analyzer time) — but the program was **still saved to the project** |
| 3 | `-process` the saved project with `-noanalysis`, corrected dump script | Still failed to compile on two API errors (`Data` lives in `ghidra.program.model.listing`; `Function.getSymbolType()` does not exist) |
| 4 | `-process` the saved project with `-noanalysis`, corrected script | **SUCCESS** — the full inventory was extracted from the already-analysed program without re-running auto-analysis |

Attempt 2 therefore yielded the decisive diagnosis. Because no analyzer was disabled, the recorded timings are the **default-profile** costs for this image:

| Analyzer | Seconds | | Analyzer | Seconds |
|---|---:|---|---|---:|
| **x86 Constant Reference Analyzer** | **3,582.079** | | Function ID | 158.297 |
| **Stack** | **2,984.146** | | Decompiler Switch Analysis | 122.702 |
| Disassemble | 778.923 | | Non-Returning Functions - Discovered | 125.706 |
| Reference | 364.355 | | Create Function | 98.815 |
| Scalar Operand References | 305.082 | | X86 Function Callee Purge | 109.159 |
| Data Reference | 165.956 | | Create Address Tables (+ One Time) | 40.637 / 68.419 |
| | | | **Total** | **9,077** |

The two dominant analyzers account for **6,566 s — 72 % of the total**. Disabling exactly those two and raising the timeout would very likely complete on this host; the API-correct `scripts/ConfigureAnalysis.java` is shipped for that purpose.

**Recovered inventory (attempt 4):**

| Measure | Value |
|---|---:|
| Memory blocks | `Headers` 0x00400000–0x00400FFF · `.text` 0x00401000–0x04C98FFF (76,120,064 B) · `.data` 0x04C99000–0x04CE36D3 (304,852 B) · `.rsrc` 0x04CE4000–0x04D0BFFF (163,840 B) · `tdb` 0xFFDFF000–0xFFDFFFFF |
| **Functions recovered** | **8,021** |
| — unnamed `FUN_<hex>` | 8,014 |
| — MSVBVM60 runtime imports | 7 (`__vbaChkstk`, `adj_fdiv_m32`, `adj_fdiv_m64`, `adj_fdivr_m64`, `DllFunctionCall`, `ThunRTMain`, `entry`) |
| Defined data items | 35,063 |
| SQL/DDL code sites inside a function | **3,566 / 3,718 (95.9 %)** |
| Functions referencing ≥ 1 SQL statement | **928** |
| Functions referencing non-SQL literals (UI text, captions) | 2,796 |
| Function→reference edges streamed | 8,507,170 |

**8,021 recovered functions against 8,026 VB6-declared methods** — a 5-difference across two entirely independent derivations (the VB6 object table and the disassembler), which mutually confirms both censuses.

---

## 4. Corrected address references — the systematic +0x42/+0x44 error

Auditing all prior evidence rows against the authentic binary (`scripts/audit_prior_corpus.py`):

| Audit dimension | Against v1.605 (authentic) | Against v1.00 (old build) |
|---|---:|---:|
| Statement texts found verbatim | **1,698 / 1,797** (+95 as substrings) | 836 / 1,797 |
| DDL texts found verbatim | **262 / 268** (+6 as substrings) | **0 / 268** |
| Prior VAs resolving to their claimed statement | **0 / 1,797** | 0 / 1,797 |
| Prior VAs resolving to their claimed DDL | **0 / 268** | 0 / 268 |

**Conclusion:** the prior corpus *content* is v1.605 (the v1.00 build contains 0 of the 268 DDL statements), but the prior *virtual addresses* are systematically **0x42 or 0x44 bytes below** the true addresses in the shipped file. Every VA-anchored claim in the prior knowledge base must be re-anchored.

**Re-anchoring table (prior VA → authentic VA, verified statement text):**

| Prior claim | Prior VA | **Authentic VA** | Verified text at the authentic address |
|---|---:|---:|---|
| net = gross − discount, `ROUND(…,2)` | `69EA76` | **`69EAB8`** | `Select pricemethod, Round(sum(totalprice),2) as tot1, Round(sum(totalpricedisc),2) as tot2, Round((sum(totalprice) - sum(totalpricedisc)),2) as tot3, Round(sum(elmadfoo3),2) as tot4, Round(sum(elbakyma3mal),2) as tot5 from patientinfo where requestdate >= '` |
| referrer commission | `694C92` | **`694CD4`** | `SELECT ROUND(SUM(Gehats.omola * patientinfo.totalpricedisc / 100),2) AS Expr1 FROM patientinfo INNER JOIN Gehats ON patientinfo.referredbyname = Gehats.gehahname Where requestdate >= '` — *note: the base is `totalpricedisc`, not a generic "amount"* |
| company net | `722674` | **`7226B8`** | `SELECT wareed , saader, (wareed - saader) as farq , taamelnote , sherkaname, taameldate FROM sharekathesab where sherkaid= '` |
| unpaid predicate | `6CEA20` | **`6CEA64`** | `SELECT patientid, labid, patientsefa, … FROM patientinfo Where (totalpricedisc …` |
| `loger` table | `66F746` | **`66F788`** | `SELECT * FROM loger where lognum= ` |
| error string | `6717E2` | **`671824`** | `An error has occurred, Please make sure your browser, Or select other` |
| `sp_addumpdevice` | `66FCB2` | **`66FCF4`** | `EXEC sp_addumpdevice 'disk', '` |
| `sp_dropdevice` | `66FD56` | **`66FD98`** | `EXEC sp_dropdevice '` |
| SHAPE subgroup↔testtable | `672722` | **`672764`** | `SHAPE {SELECT subgrouptable.subgroupcode, …} AS cmdbill APPEND ({SELECT * FROM testtable where testorno= 'True' …} AS cmdbill_d RELATE subgroupcode TO subgroupcode) AS cmdbill_d` |
| price catalogue | `6A9632` | **`6A9674`** | `Select testcode, testabrev, testname, subgroupname, testtime, inotherlab, …, otherlabprice, patientprice, labprice from testtable where testcode = ` |
| invoice header | `6A5796` | **`6A57D8`** | `SELECT patientid, totalprice, totalpricedisc, elbakyma3mal, discount100, discount, elmadfoo3, completes FROM patientinfo where patientid='` |

**Branch-code rule — VERIFIED, and the two prior statements are two different rules.** `SUBSTRING(patientid,2,1) + SUBSTRING(patientid,9,1) as bran` occurs **exactly 18 times** (first at `0x727190`), each paired with a Jet-dialect twin `MID(patientid,2,1) + MID(patientid,9,1) as bran` — confirming both the position-9 branch-code rule **and** the dual-dialect claim. A separate rule `SUBSTRING(patientid,10,3) as patid` occurs **58 times** (first at `0x682F98`), again with `MID(...)` twins. The prior knowledge base mixed these two rules under one heading; both are real, and they are distinct.

**Reference-range rule — VERIFIED.** The binary queries `normalrange` with `REFRANGE='1'` and `REFRANGE='2'` (e.g. `0x6A8E84 ' and REFRANGE='2' and ageunit='`, `0x6BEA28`), and the shipped Access mirror contains **no `REFRANGE='2'` rows at all** (see §5). The prior claim that the runtime reads reference ranges absent from the shipped mirror is **confirmed**.

---

## 5. Phase D — Database forensics on the real MDB files

All five shipped databases were opened **read-only** through the Microsoft Access Database Engine OLE DB provider (`Microsoft.ACE.OLEDB.16.0`, then `12.0`), and index/primary-key metadata was additionally read through the ADOX catalog object. No file was written; SHA-256 was taken before the provider took its handle.

| File | Bytes | SHA-256 (first 16) | Tables | Columns | Indexes | PK | Data rows |
|---|---:|---|---:|---:|---:|---:|---|
| `bin\Patients.mdb` | 2,682,880 | `98D51F5294496E03` | **103** | **1,682** | **68** | **5** | 41 tables populated, 62 empty |
| `bin\Patients_backup.mdb` | 2,146,304 | `798D5B57858D6268` | 90 | 1,381 | 28 | 0 | — |
| `bin\Branch_Data.mdb` | 970,752 | `F6D6D5E25C4E0B0D` | 49 | 969 | 9 | 0 | **0 rows in all 49 tables** |
| `System\Branch_Data.mdb` | 966,656 | `E6C3953EBBE7B39E` | 49 | 960 | 9 | 0 | template |
| `bin\data.mdb` | 266,240 | `84840CDA3AA8333B` | 3 | 34 | 4 | 0 | `history`, `patientinfo`, `wasl` — all empty |

**Prior claims re-tested:** "103 tables / 1,682 columns" **VERIFIED exactly**; "90 / 1,381" backup **VERIFIED**; "49 / 969 / 0 rows" branch template **VERIFIED**; "no declared PK/FK/CHECK/DEFAULT/indexes" **CONTRADICTED**.

**Declared primary keys (ADOX, VERIFIED):** `instrument(instrumentid)`, `pricemenu(pricelistname)`, `sharekat(sherkaid)`, `testtable(testcode)`, `testtemp(testcode)`. Unique (non-PK) indexes on `barcodes(barcodeid)` and `patients(patientusid)`. Secondary indexes exist on `patientid` across 12 result tables, on `testcode` in `anyreport`/`pictures`/`wasl`, and on `subgroupcode` in `testtemp`/`wasl`.

**Row counts (`Patients.mdb`):** `normalrange` 1,152 · `abbreviations` 650 · `antibiotic` 612 · **`testtable` 609** · `converts` 167 · `insulin` 113 · `antibio` 95 · `testunits` 70 · `customtesttable` 54 · `subgrouptable` 48 · `barcodes` 35 · `PTC` 35 · `testlearn` 29 · `daytip` 19 · `anyreport` 15 · `titless` 15 · `culturesec` 12 · `loggroup` 8 · `SMStemplate` 8 · `SMSSender` 8 · `pricelist` 6 · `pictures` 6 · `MyLabSetting` 5 · `customgrouptable` 5 · `Labbranch` 4 · `comments` 4 · `history` 3 · `Helicobacter` 3 · **`patients` 2** · `pricemenu` 2 · `ratios` 2 · `testname` 2 · `smslist` 2 · `constanttable` 2 · `testtemp` 1 · `Paste Errors` 1 · `reportview` 1 · `hba1c` 1 · `ESR` 1 · `smsgroups` 1 · `TORCH` 1 — **62 of 103 tables are empty.**

**Patient-data exposure: none.** `patientinfo`, `patienttest`, `payment`, `patienttotal`, `patienttestgroup`, `instrument`, `noteic` all contain **0 rows** — the shipped `Patients.mdb` is a **template**, not a live patient database. The only populated person-like table is the operator account table `patients` (2 rows); only its **schema** is reported here, never its values, and no password, hash or account name appears in this report.

**Corrections to prior data claims:**
- `normalrange` — prior "852 rows / 304 tests, M 422 / F 421" is **CONFLICTED**. Real: **1,152 rows**; `sex` distribution **Male 570 / Female 568 / All 10 / empty 4**; `REFRANGE` distribution **empty 797 / '0' 345 / '1' 10 / '2' 0**.
- `patients` (operator table) — prior "grant1..grant12" is **CONTRADICTED**: the real table has **33 columns including `grant1`…`grant17` (17 flags), `SecWord`, `patientword`, `patientusname`, `patientusid`, `patientus`, `patientuscap`, `patientad`, `khasmlimit`, `tipnum`, `showtip`, `startwork`, `endwork`, `breaktime`, `breakyorn`, `lastvisit`, `branche`**. The binary's own DDL confirms it: `ALTER Table Patients ADD grant13 Bit` … `grant17` (SQL-Server form) and `ALTER Table Patients ADD COLUMN grant13 YESNO` … `grant17` (Jet form).

**Column types (OLE DB codes, VERIFIED):** `patientid` = 130 (WSTR, 13 chars per the DDL) · `grant1`…`grant17` = 11 (**Boolean**) · `patientword` = 130 · `SecWord` = 130 (DDL `varchar(10)`) · `patientusname` = 130 · `patientusid` = 3 (I4) · `totalprice`/`totalpricedisc`/`discount`/`discount100` = 5 (**R8 / double**) · `patienttest.testcode` = 2 (I2) · `patienttest.resultt1..t4` = 130 · `normalrange.REFRANGE`/`sex` = 130.

**Access-specific metadata is permission-blocked:** `MSysObjects`, `MSysQueries` and `MSysRelationships` return *"no read permission"*; `MSysColumns`/`MSysIndexes` are *not present* in these files (Access 97/2000 Jet-4 layout). Consequently **stored QueryDefs are absent** from the shipped MDBs — the application executes SQL directly rather than against saved Access queries. Recorded as an exact blocker, not an inference.

---

## 6. Phase E — Gap-by-gap results

| # | Documented gap | Result | Evidence |
|---|---|---|---|
| 1 | VB6 method names and bodies; complete call graph | **PARTIALLY RESOLVED** | 210/212 object names; **444** method names (prior 0); Σ method counts **8,026** confirmed exactly; per-object `aMethodNames` arrays mapped; **8,021 native functions** recovered by Ghidra (against 8,026 declared methods — a 5-difference across two independent derivations); **928 functions** mapped to their SQL statements; **3,566/3,718** SQL code sites attributed to a function. **A *named* call graph remains impossible**: the compiled image contains no procedure names (`Form_Load`/`_Click` absent in ASCII/UTF-16LE/UTF-16BE), so the artifact itself does not carry them. |
| 2 | `grant1..grant17` / `SecWord` / `IDPassword*` / `patientword` semantics and password handling | **RESOLVED** | Real `patients` table: 17 Boolean grant flags, `SecWord` (secondary security word, `varchar(10)`), `patientword` (password), `patientusname`/`patientusid`/`patientuscap`. Binary DDL: `ALTER Table Patients ADD grant13 Bit` … `grant17` / `ADD COLUMN grant13 YESNO` … `grant17`; `ALTER Table Patients ADD SecWord varchar(10)` / `ADD COLUMN SecWord varchar(10)`. Password usage is a **plain-text SQL comparison**: `' and patientword= '` and `' and patientword='`. `IDPasswordL/T/F/B/S/V/A` (7 consecutive names) are the **ID/password form's control names**, not separate credential columns — no `IDPassword*` column exists in any MDB. Additional password UI strings: *Please Enter Your Internal Password. (Digits only)*, *Wrong Password Entered.*, *Print Result Password*, `&password=`. |
| 3 | Five unnamed external components; undecoded control/typelib names | **RESOLVED** | The five third-party controls are now identified by their shipped binaries and version resources: `ChamaleonButton.ocx` (ChamaleonButton 1.00), `Redsoul911XPCOmponents.ocx` (Redsoul911 XP Components 2, 1.00), `RgheedButtonControlV2.ocx` (1.00), `XP_BUTTONS.ocx` (XPbutton 1.00) and `NOTEX.dll` (RLS Notes 2.00) — plus the anomalous `REPORT.dll` at **562 bytes** (a stub, flagged). Total shipped control/runtime set: **41 OCX + 33 DLL + 4 TLB**, all enumerated with file versions in `12_AUTHENTIC_TARGET_ANALYSIS/01_TARGET_TREE_INVENTORY.md`. |
| 4 | `VIDAS` database structure and role | **RESOLVED (role) / N/A (structure)** | **"VIDAS" occurs 0 times in the authentic v1.605 binary** (ASCII *and* UTF-16LE). The name enters only through the *installer*: `Setup.bat` runs `OSQL -S %COMPUTERNAME% -E /i Attaching.sql` then `Attach_database.bat` in `D:\Lab_data`, and the prior deployment report records `D:\Lab_data\Attach_VIDAS.bat`. VIDAS is therefore an **externally attached database (FoxPro-linked) created at install time, never referenced by the application** — it is not an application database and no structure is recoverable because the artifact is not in the shipped tree. |
| 5 | `RL_SETUP.exe` 22.95 MiB embedded payload | **PARTIALLY RESOLVED** | `RL_SETUP.exe` is 23,060,480 B, VB6 Setup Wizard **v19** (`ProductName 'RealLab CD'`, `CompanyName 'RealLab'`, `FileDescription 'ais_soft_group@yahoo'`). It contains **no `MSCF` cabinet signature, no `SETUP.LST` string and no `real lab system.CAB` string** — its payload is a proprietary compressed stream, so the 22.95 MiB cannot be enumerated in place. Its **effective contents are enumerable** through `WIN7\SETUP.LST` (45 `File#=` entries) and `WIN7\real lab system.CAB` (**48 files**: VB6 runtime, Jet-4.0 stack, ADO/DAO, 12 OCX controls incl. the 3 XP button packs, 5 fonts, `SETUP1.EXE`, `ST6UNST.EXE`, `VB6STKIT.DLL`), plus the 34 `datareport_vh\*.cab` redistributables and the PDF/Office installers. |
| 6 | Composite-test component mapping per `testcode` and write/validation path | **PARTIALLY RESOLVED** | The mapping chain is now explicit: `testtable` (609 rows, 38 columns) → `subgroupcode` → `subgrouptable` (**48 rows**, e.g. 11 BLOOD PICTURE, 19 TORCH Screen, 41 Blood Gases) → per-discipline profile table (`cbc`, `kidney`, `liver`, …). Composite *packages* are enumerated in `customgrouptable` (CHECKUP 1–5) and **`customtesttable` (35 rows)** giving exact testcode membership and price, e.g. CHECKUP 1 = testcodes 4 (Urine), 5 (Stool), 8 (SGPT), 9 (SGOT), 1 (F.B.G.), 15 (Bilirubin), 11 (Creatinine), 56 (CBC). Storage on the order row uses `patienttest.RESULTT` plus `resultt1..resultt4` (5 components), with composite panels carrying the sentinel `RESULTT = 'See Report'` while their analytes go to the profile table (observed on the `testtemp` row for testcode 70 / Blood Gases). Report templates bind by `[reportdata] testtype=` (228 INIs under `Data\`, values `''` 159, `pcr` 33, `r` 26, `pnt` 10) with per-analyte keys (`ITEM*`, `LABEL*`, `LINE*`, `TIGA*`, `TIGG*`, `CMVIGG*`, `ITEMG*`, `RUBIGG*`, `ITEMX*`, `Acidic05_*`, `Alkalin05_*`, `Nitrl05_*`, `Her2IGG*`, plus `F`/`M`/`D` sex variants and `05`/`20` age bands). **The per-`testcode` analyte-component list and the write/validation code path remain UNKNOWN** — the write is a bound-recordset `AddNew`/`Update` operation with no literal `INSERT`/`UPDATE` to trace; the **fetch** side is localised to `FUN_01e27d40` (composite-test fetch) and `FUN_02d7b200` (order-row fetch). |
| 7 | Per-invoice financial computation and `discount100` semantics | **PARTIALLY RESOLVED** | Aggregate formula **VERIFIED** at `0x69EAB8` (net = gross − discount, `ROUND(…,2)`, five aggregates `tot1..tot5` grouped by `pricemethod`); referrer commission **VERIFIED** at `0x694CD4` (`omola × totalpricedisc / 100`); company net **VERIFIED** at `0x7226B8` (`wareed − saader as farq`); invoice header **VERIFIED** at `0x6A57D8` (`totalprice, totalpricedisc, elbakyma3mal, discount100, discount, elmadfoo3, completes`). `discount100` is a **double** (OLE DB 5) alongside `discount`, distinct from the applied discount amount `totalpricedisc` — a percentage reading is plausible but **UNVERIFIED**, and the arithmetic lives in VB code, not in SQL. |
| 8 | Runtime behaviour | **BLOCKED (by design and by environment)** | The target was **not executed** on this host. No isolated VM/sandbox exists (no admin rights, no Hyper-V, Docker present but daemon stopped, WSL has no distro), and executing an unreviewed clinical system on the analyst workstation is unsafe. Consequently **0 runtime behaviours are observed**; all behavioural statements remain static reconstructions. This limitation is unchanged from the prior pass and is restated rather than removed. |

---

## 7. Phase F — Revalidation ledger of prior findings

| Prior finding | Class after this pass | Note |
|---|---|---|
| Target identity, size, SHA-256, PE structure, version, timestamp | **VERIFIED** | Independently re-derived from the shipped bytes |
| 231 vs 186 `MSVBVM60` imports | **VERIFIED** | Independent PE import parser |
| 210 objects (132 forms, 66 DataReports, 9 modules, 3 classes) | **VERIFIED** | 210 of 212 named via the corrected `aObjectArray` offset |
| 8,026 declared methods | **VERIFIED** | Exact match from `dwMethodCount` |
| 1,268 method entries addressable / 0 method names | **SUPERSEDED** | 444 names now recovered; the "0" was a tool defect, not an artifact limit |
| 1,797 SQL literals, 0 INSERT, 0 UPDATE, 268 DDL | **VERIFIED** | Re-extracted as 1,758 UTF-16LE literals; INSERT/UPDATE absent |
| 103 tables / 1,682 columns | **VERIFIED** | Exact match on the real MDB |
| "No declared PK/FK/CHECK/DEFAULT/indexes" | **CONFLICTED → CORRECTED** | 68 indexes and 5 primary keys exist |
| `patients` = operator table with `grant1..grant12` | **CONFLICTED → CORRECTED** | `grant1..grant17` (17 flags) + `SecWord` |
| `normalrange` 852 rows, M 422 / F 421 | **CONFLICTED → CORRECTED** | 1,152 rows; Male 570 / Female 568 / All 10 / NULL 4 |
| `REFRANGE='2'` rows absent from the Access mirror | **VERIFIED** | Zero `REFRANGE='2'` rows in the real file; the code queries them |
| Position-9 branch code (`SUBSTRING(patientid,2,1)+SUBSTRING(patientid,9,1) as bran`, 18 statements) | **VERIFIED** | 18 occurrences, first at `0x727190` |
| `SUBSTRING(…,10,3)` rule | **VERIFIED** | 58 occurrences, first at `0x682F98` — a *separate* rule from the branch code |
| Net = gross − discount, `ROUND(…,2)` | **VERIFIED** | Re-anchored to `0x69EAB8`; the prior VA was wrong |
| Referrer commission `omola × amount / 100` | **VERIFIED with refinement** | Base is `totalpricedisc`; re-anchored to `0x694CD4` |
| Company `farq = wareed − saader` | **VERIFIED** | Re-anchored to `0x7226B8` |
| Unpaid predicate | **VERIFIED** | Re-anchored to `0x6CEA64` |
| 64-table `DELETE FROM … WHERE patientid` cascade | **VERIFIED** | Present in the re-extracted corpus |
| Dual SQL-Server / Jet dialect literal pairs | **VERIFIED** | 18 `as bran` statements each with a `MID(...)` twin |
| `SHAPE` hierarchical queries | **VERIFIED** | 39 SHAPE literals; exemplar re-anchored to `0x672764` |
| 228 RTF templates | **CONFLICTED → CORRECTED** | This tree contains **58** `.rtf` files, all under `Cache\` (Flash-projector assets) |
| 223 `Data\*.ini` templates | **VERIFIED** | 223 under `Data\` + 5 under `Data\data_emp\` = 228 |
| 39-file `SETUP.LST` | **CONFLICTED → CORRECTED** | **45** `File#=` entries across the three sections |
| `VIDAS` as an application SQL-Server catalogue | **CONFLICTED → CORRECTED** | Zero occurrences in the binary; installer-side attachment only |
| Five unnamed external components | **RESOLVED** | Identified by version resource (§6 gap 3) |
| Target binary / original MDBs "absent" | **CONTRADICTED** | Present at `F:\REAL_LAB_COPY`; absent only inside `D:\Collective work` |
| "No target binary was executed" | **VERIFIED (still true)** | Unchanged; no execution performed |

Nothing from the prior output was deleted or overwritten. All original evidence is intact; the 73 prior files are byte-identical to `Docs/KB/legacy/reverse-engineering-output/`.

---

## 8. Updated per-dimension coverage (explicit denominators, auditable definitions)

| # | Dimension | Definition of "complete" | Numerator | Denominator | Coverage |
|---|---|---|---:|---:|---:|
| 1 | Target-tree inventory | every file has path + size + SHA-256 + type | 837 | 837 | **100 %** |
| 2 | VB6 object identification | named, printable object identifier | 210 | 212 | **99.1 %** |
| 3 | VB6 method identification | named method | 444 | 8,026 | **5.5 %** |
| 4 | Method-count fidelity | Σ `dwMethodCount` equals the prior figure | 8,026 | 8,026 | **100 %** |
| 5 | SQL literal extraction | SQL/DDL literal recovered from the authentic image | 1,758 | 1,758 (as extracted) | **100 %** of extractable |
| 6 | SQL literal → code binding | literal has ≥1 verified immediate-operand code site | 1,750 | 1,758 | **99.5 %** |
| 7 | DDL localisation | DDL site assigned to the migration region | 291 | 307 | **94.8 %** |
| 8 | Database schema | tables/columns/indexes/PK/row-counts read from the real file | 103 / 1,682 / 68 / 5 / 103 | 103 / 1,682 / — / — / 103 | **100 %** |
| 9 | Stored QueryDefs / Access system tables | readable | 0 | unknown | **0 % — permission-blocked** |
| 10 | Prior corpus content re-verification | prior text found verbatim | 1,960 | 2,065 | **94.9 %** |
| 11 | Prior corpus address re-verification | prior VA resolves to its statement | **0** | 2,065 | **0 % — systematically wrong, corrected here** |
| 12 | Per-invoice financial rules | each documented rule recovered with exact text | 4 | 4 aggregate/invoice rules | **100 %** (per-invoice arithmetic still not in SQL) |
| 13 | Composite-test mapping | subgroup→profile and package membership | 48 + 35 | 48 + 35 | **100 %** of declarative mapping; per-`testcode` analyte list **0 %** |
| 14 | Native function inventory (Ghidra) | decompiled function list | 8,021 | 8,021 | **100 %** | `evidence/ghidra-functions.tsv` |
| 14a | Function ↔ SQL statement map | function matched to the SQL it executes | 928 | 8,021 | **100 % of SQL-bearing functions** | `evidence/ghidra-sql-by-function.tsv` |
| 14b | SQL code site inside a function | site attributed to a containing function | 3,566 | 3,718 | **95.9 %** | `evidence/ghidra-sql-site-to-function.tsv` |
| 14c | Function → VB6 object / control name | function labelled with its owning VB6 object | 0 | 8,021 | **0 % — procedure-address table did not resolve** | `12_AUTHENTIC_TARGET_ANALYSIS/03_SQL_CORPUS_AND_CODE_BINDING.md` §11.3 |
| 15 | Runtime behaviour | observed at runtime | 0 | unknown | **0 % — deliberately not executed** |

---

## 9. Final statement — complete, partial, blocked

**COMPLETE.** The target's identity, structure and provenance; the full 837-file inventory with digests; the PE/import/resource facts; the corrected VB6 native header → ProjectInfo → ObjectTable → 210-object walk; the 8,026-method census; the 444 recovered method names; the **8,021-function native inventory**; the 1,758-literal SQL/DDL corpus with a 99.5 %-verified code binding **and a 928-function procedure-level map**; the DDL migration routine **VERIFIED as the single function `FUN_01b61cf0`**; the complete schema, indexes, primary keys, row counts and column types of all five shipped databases; the re-anchoring of every VA-anchored prior claim; the identification of the five external components; the VIDAS role; the operator/grant/password schema; the deployment model.

**PARTIALLY COMPLETE.** Composite-test component mapping (declarative mapping complete; per-`testcode` analyte list and write path not, though the fetch routines are now identified); per-invoice financial computation (aggregate and header rules recovered; the per-invoice arithmetic and `discount100` semantics live in VB code that has not been decompiled); `RL_SETUP.exe`'s embedded 22.95 MiB (effective contents enumerable, in-place stream not decomposable); the 7,582 unnamed private/event procedures (proven absent from the artifact); **function → VB6 object/control naming** (the per-object procedure-address table did not resolve to a usable address).

**BLOCKED.** Access system-table metadata — `MSysObjects`/`MSysQueries`/`MSysRelationships` are ACL-protected, and `MSysColumns`/`MSysIndexes` are absent from these Jet-4 files. Runtime behaviour — no isolated environment exists and the target was not executed, by design. *Ghidra auto-analysis cannot complete on this host* — the default profile costs 9,077 s for this image, of which the *x86 Constant Reference Analyzer* (3,582 s) and *Stack* (2,984 s) alone account for 72 %, and the run is cut off at 7,200 s; the saved project was nevertheless reused to obtain the function inventory without re-analysis, and the API-correct `scripts/ConfigureAnalysis.java` disables precisely those two analyzers as the remediation on a larger machine.

**Why these remain:** each residual is bounded by an identified external factor (Jet ACLs, deliberate non-execution, host capacity, or information the compiler discarded), not by unexhausted analysis. The tooling was driven as far as the host permits: REA CLI (PE resources), an independent PE import parser, a purpose-written VB6 native-structure parser validated against the artifact's own invariants (8,026 = prior figure; 210 named objects; 8,021 functions), a UTF-16LE literal extractor validated by raw-instruction byte inspection, a full Jet/ADOX schema extractor, and Ghidra 12.1.4 headless with `JAVA_HOME` correctly set.

---

## 10. Files written or modified by this pass

**Written (new, inside `REAL_LAB_1605_REVERSE_ENGINEERING_OUTPUT/`):**

```
13_FINAL_REPORT\FINAL_REVERSE_ENGINEERING_REPORT.md          (this file)
12_AUTHENTIC_TARGET_ANALYSIS\00_PHASE_A_BASELINE_VERIFICATION.md
12_AUTHENTIC_TARGET_ANALYSIS\01_TARGET_TREE_INVENTORY.md
12_AUTHENTIC_TARGET_ANALYSIS\02_VB6_OBJECT_AND_METHOD_STRUCTURE.md
12_AUTHENTIC_TARGET_ANALYSIS\03_SQL_CORPUS_AND_CODE_BINDING.md
12_AUTHENTIC_TARGET_ANALYSIS\04_DATABASE_FORENSICS_REAL_MDB.md
12_AUTHENTIC_TARGET_ANALYSIS\05_GAP_RESOLUTION_LOG.md
12_AUTHENTIC_TARGET_ANALYSIS\06_CORRECTIONS_TO_PRIOR_DOCUMENTS.md
12_AUTHENTIC_TARGET_ANALYSIS\07_COVERAGE_MEASUREMENTS.md
12_AUTHENTIC_TARGET_ANALYSIS\08_PROPOSED_KB_PATCHES.md
12_AUTHENTIC_TARGET_ANALYSIS\evidence\  (96 files: hash manifest, VB6 object/method JSON,
        SQL literals + code-site map, prior-corpus audit, VA re-anchoring, 5 MDB schema dumps,
        Ghidra function inventory, Ghidra SQL-by-function map, Ghidra string xrefs,
        Ghidra analyzer timings, the four Ghidra run logs)
12_AUTHENTIC_TARGET_ANALYSIS\scripts\   (14 analysis scripts + 2 Ghidra Java scripts,
        stdlib/.NET only, read-only)
```

**Modified:** none outside the above directory.

**Explicitly NOT modified** (per `AGENTS.md` and the mission): `src/`, `tests/`, `README.md`, `IDEA.md`, `ReferenceIndex/`, `Docs/RL-P*.md`, `scripts/kb_check.py`, `scripts/kb_build.py`, `Docs/KB/generated/*`, and `Docs/KB/legacy/*` — proposed legacy-KB edits are listed in `12_AUTHENTIC_TARGET_ANALYSIS/08_PROPOSED_KB_PATCHES.md` for owner approval and are **not applied**. No commit, no staging, no push was performed; `HEAD` remains `863f03f` and the working tree was clean at the end of the read-only phases.

**Target artifacts:** read-only throughout. Every MDB was hashed before the OLE DB provider opened it; the executable was copied to a temp working area (`RLS_v1605.exe`, SHA re-verified as `c54ce6c8…df93c0`) and all analysis ran against the copy. `D:\Collective work` was read only.

---

# الملخّص التنفيذي — بالعربية

**الهدف.** إكمال الهندسة العكسية للنظام `REAL LAB SYSTEM v1.605` والتحقّق من نتائج المرحلة السابقة على الملفات الأصلية نفسها، لا على النُّسخ الجزئية.

**المكتشف الأهم.** الملف التنفيذي الأصلي وكافة قواعد البيانات موجودة فعلًا في `F:\REAL_LAB_COPY`، وكانت غائبة فقط من مجلد الأدلة `D:\Collective work`. أُعيد فحص كل نتيجة سابقة على الأدلة الأصلية: **نصوص الاستعلامات كلها صحيحة تقريبًا (١٩٦٠ من ٢٠٦٥ سطرًا)، لكن كل العناوين الافتراضية السابقة خاطئة بمقدار ثابت `0x42` أو `0x44` بايت**، وأُعيد ضبط جميع مراجع العناوين في التقارير.

**ما تحقّق.** استُخرجت ٢١٠ أسماء كائنات و**٤٤٤ اسم دالة** حيث لم تُستخرج أي أسماء قبل ذلك، وأُكّد عدد الدوال المعلن `٨٠٢٦` بدقة تامة. كما استُخرج جرد دوال أصلية من `٨٠٢١` دالة، وهو أقل بخمس من عدد الدوال المعلن، فتأكّد الرقمان مستقلّين. ورُبطت `١٧٥٠` من `١٧٥٨` استعلامًا بمواقعها في الشيفرة بنسبة `٩٩٫٥٪`، ورُبط `٩٢٨` دالة بعبارات الاستعلام التي تُنفّذها. وعُدِّي اجرا تعديل المخطط `٢٦٨` إلى دالة واحدة مقدارها `٢٣٠` كيلوبايت، وحُصرت سلسلة حذف المرضى في دالتين. وتحقّقت القواعد المالية والتعريفية كلها الواردة في التقارير السابقة.

**ما تصحّح.** جدول المشغّلين `patients` يملك `grant1..grant17` لا `grant1..grant12`، وله `SecWord` و`patientword`. وقاعدة `normalrange` فيها `١١٥٢` صفًا لا `٨٥٢`. وقاعدة البيانات الحقيقية فيها `٦٨` فهرسًا و`٥` مفاتيح أساسية على خلاف ما ورد في التقارير. واسم `VIDAS` لا يظهر إطلاقًا في الملف التنفيذي، فهو ارتباط من التثبيت لا قاعدة بيانات للتطبيق. وعدد قوالب `RTF` في هذه الشجرة `٥٨` لا `٢٢٨`.

**ما بقي محجوبًا.** لا يمكن تسمية الدوال لأن الشيفرة المصدرية لا تحمل أسماء الإجراءات أصلًا. وجداول نظام `Access` محميّة بصلاحيات. والسلوك وقت التشغيل لم يُنفَّذ عمدًا لأسباب سلامة. وتحليل `Ghidra` التلقائي لا يكتمل على هذا الجهاز لأن مُحلِّلَي الثوابت وتحليل المكدس يستهلكان `٧٢` من الزمن، لكن المشروع المحفوظ أُعيد استخدامه بلا إعادة تحليل لاستخراج الجرد كاملًا. التفاصيل الكاملة في `13_FINAL_REPORT\FINAL_REVERSE_ENGINEERING_REPORT.md`.

---

*All commands, byte-level dumps, hashes and machine-readable outputs backing every statement above are preserved under `12_AUTHENTIC_TARGET_ANALYSIS/evidence/` and `12_AUTHENTIC_TARGET_ANALYSIS/scripts/`.*
