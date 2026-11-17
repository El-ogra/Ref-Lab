# Gap-resolution log — every documented gap, with its verdict

Each row states the verdict, the evidence that produced it, and what remains outstanding. Verdicts: **RESOLVED** · **PARTIALLY RESOLVED** · **STILL UNKNOWN** · **BLOCKED** · **CONFLICTED**.

---

## Gap 1 — VB6 method names, bodies, and the complete call graph

**Verdict: PARTIALLY RESOLVED** (call graph: **STILL UNKNOWN**, and shown to be unattainable from this artifact).

*Recovered this pass*
- 210 of 212 objects named (99.1 %) — the prior pass named none.
- **444 method names** recovered (prior: 0).
- Σ declared methods = **8,026**, matching the prior figure exactly.
- Per-object `aMethodNames` arrays mapped to every object; 93 objects carry at least one named member.
- Per-form control tables located, giving each control's **event-handler code address** alongside its control name.
- **A native function inventory: 8,021 functions** recovered by Ghidra, with entry points, sizes, instruction counts and body extents — against the 8,026 VB6-declared methods, a 5-difference across two independent derivations.
- **928 functions** mapped to the SQL statements they execute, and **3,566 of 3,718** SQL code sites (95.9 %) attributed to a specific function.

*Why the rest cannot be recovered*
The compiled image contains no procedure names at all. `Form_Load`, `_Click`, `Attribute VB_Name`, `End Sub`, `Private Sub ` and `Option Explicit` are absent in ASCII, UTF-16LE and UTF-16BE, while control names (`cmdOK` ×2, `Command1` ×35, `Text1` ×54) are present in the same search. The compiler discarded the names; the information no longer exists in the shipped artifact. A per-method call graph therefore cannot be *reconstructed* — only *re-derived at address level*, which is what §3 of `03_SQL_CORPUS_AND_CODE_BINDING.md` does.

*Evidence:* `evidence/vb6-objects-and-method-names.json`; `02_VB6_OBJECT_AND_METHOD_STRUCTURE.md` §5–§6.

---

## Gap 2 — `grant1..grant17` / `SecWord` / `IDPassword*` / `patientword` semantics and password handling

**Verdict: RESOLVED.**

| Element | Finding | Evidence |
|---|---|---|
| `grant1..grant17` | **17** Boolean permission flags (OLE DB type 11) on the operator table `patients` — the prior "grant1..grant12" is wrong | `evidence/mdb-forensics/bin_Patients/columns.json`; binary DDL `ALTER Table Patients ADD grant13 Bit` … `grant17`, and `ADD COLUMN grant13 YESNO` … `grant17` |
| `SecWord` | secondary security word, Unicode string, DDL `varchar(10)`; referenced as the column `secWord` | binary `0x2dc300`, `0x2dc758`, `0x2ef6f8` |
| `patientword` | the operator **password**, compared in cleartext inside a SQL predicate: `' and patientword= '` / `' and patientword='` | binary `0x2f0038`, `0x2f01d0` |
| `IDPassword*` | **not credential columns** — the seven names `IDPasswordL/T/F/B/S/V/A` are *control names* on the internal-ID/password form; no `IDPassword*` column exists in any MDB | binary `0x2a5f50`–`0x2a5ff8`; column dumps of all five databases |
| Other operator attributes | `patientusname`, `patientusid`, `patientus`, `patientuscap`, `patientad`, `khasmlimit`, `tipnum`, `showtip`, `startwork`, `endwork`, `breaktime`, `breakyorn`, `lastvisit`, `branche` | `evidence/mdb-forensics/bin_Patients/columns.json` |
| Password UX | *Please Enter Your Internal Password. (Digits only)*, *Wrong Password Entered.*, *Print Result Password*, `&password=` | binary literals |

No credential value was read or recorded. The `patients` table's 2 rows are operator accounts, not patients.

---

## Gap 3 — Five unnamed external components; undecoded control / typelib names

**Verdict: RESOLVED.**

The shipped control/runtime set is **41 OCX + 33 DLL + 4 TLB**, all enumerated with product names and versions in `01_TARGET_TREE_INVENTORY.md` §4.5. The five previously unnamed third-party components are:

1. `ChamaleonButton.ocx` — ChamaleonButton 1.00
2. `Redsoul911XPCOmponents.ocx` — Redsoul911 XP Components 2, 1.00
3. `RgheedButtonControlV2.ocx` — RgheedButtonControlV2 1.00
4. `XP_BUTTONS.ocx` — XPbutton 1.00
5. `NOTEX.dll` — RLS Notes 2.00

Plus one anomaly worth flagging: **`REPORT.dll` is 562 bytes** with no version resource — a stub rather than a real component.

Because the target imports only `MSVBVM60.DLL`, the remaining components are bound at run time; their identities therefore come from the shipped binaries and version resources, not from an import table. The binary's own external-component blocks were located and sampled (for example `labmain`'s block at `0x0066C620` names `kernel32` and `Sleep`), confirming the mechanism.

---

## Gap 4 — `VIDAS` database structure and role

**Verdict: RESOLVED as to role; structure N/A.**

**"VIDAS" occurs 0 times in the authentic v1.605 binary** — checked as ASCII and as UTF-16LE across the whole file. It enters the picture only through the installation chain: `System\Setup.bat` runs `OSQL -S %COMPUTERNAME% -E /i Attaching.sql` and then `Attach_database.bat` in `D:\Lab_data`, and the prior deployment report records `D:\Lab_data\Attach_VIDAS.bat`.

Therefore **VIDAS is an externally attached database created at install time — it is never referenced by the application.** No structure is recoverable because the artifact is not in the shipped tree, and none is needed by the application.

**Consequence for the prior knowledge base:** the claim that the SQL Server side carries a `patients` / `Interface` / `VIDAS` catalogue must be corrected — `VIDAS` is not an application database.

---

## Gap 5 — `RL_SETUP.exe` 22.95 MiB embedded payload

**Verdict: PARTIALLY RESOLVED.**

| Fact | Value |
|---|---|
| Size | 23,060,480 B |
| Identity | VB6 Setup Wizard **v19**; `ProductName 'RealLab CD'`; `CompanyName 'RealLab'`; `FileDescription 'ais_soft_group@yahoo'` |
| Cabinet signature (`MSCF`) | **none** |
| `SETUP.LST` string inside | **none** |
| `real lab system.CAB` string inside | **none** |

The payload is a proprietary compressed stream, so it cannot be enumerated in place. Its **effective contents are enumerable**:

- `WIN7\SETUP.LST` — **45** `File#=` entries (six VB6/OLE bootstrap files, 36 application/control/font files, 3 Jet-4.0 components).
- `WIN7\real lab system.CAB` — **48 files** (7,211,180 B): `msvbvm60.dll`, `OLEAUT32.DLL`, `OLEPRO32.DLL`, `ASYCFILT.DLL`, `STDOLE2.TLB`, `COMCAT.DLL`, `MSDBRPTR.DLL`, `MSSTDFMT.DLL`, `MSBIND.DLL`, `msado27.tlb`, `dao360.dll`, `msjet40.dll`, `msjter40.dll`, `msjint40.dll`, `vbajet32.dll`, `expsrv.dll`, `mswstr10.dll`, `mswdat10.dll`, `msrd2x40.dll`, `msrd3x40.dll`, `msrepl40.dll`, `msjtes40.dll`, `VB5DB.DLL`, the 4 third-party/XP control packs, `DBGRID32.OCX`, `MSDATGRD.OCX`, `MSADODC.OCX`, `MSCOMCT2.OCX`, `TABCTL32.OCX`, `MSDATLST.OCX`, `COMDLG32.OCX`, `MSCHRT20.OCX`, `MCI32.OCX`, `RICHTX32.OCX`, `RICHED32.DLL`, `MSCOMCTL.OCX`, `MSFLXGRD.OCX`, `MSCOMM32.OCX`, `SETUP1.EXE`, `ST6UNST.EXE`, `VB6STKIT.DLL`, and 5 fonts (`MCS Taybah.ttf`, `ais symbol.ttf`, `ais lab.ttf`, `WBCW_UPCEAN.ttf`, `WBCWS_UPCEAN.ttf`).
- `WINXP\real lab system.CAB` — 7,057,945 B (WinXP variant).
- `System\datareport_vh\*.cab` (32 CABs) — the VB6 SP6 / OCX redistributables, `VS6sp6B1.cab` 9,036,800 B, `VS6sp6B2.cab` 10,010,624 B, `VS6sp6B3.cab` 995,042 B, `vbrun60.cab` 989,512 B.
- `System\PDF\` — Bullzip PDF Printer 11.1, Ghostscript Lite 9.14.17, Xpdf Lite 3.3.4, PDF Power Tool 4.0, .NET 4.0.
- `System\MDAC_TYP\MDAC_TYP.EXE` — MDAC 2.8 / Jet 4.0.

*Outstanding:* the compressed stream inside `RL_SETUP.exe` itself is not byte-decompressible with the tooling available here (no VB6 Setup-Wizard decompressor, no 7-Zip CAB/MSI writer for this proprietary format). The remaining difference between the 22.95 MiB payload and the 7.2 MB CAB is the compression plus the runtime material that the CAB references but does not carry.

---

## Gap 6 — Composite-test component mapping per `testcode`, and the write/validation path

**Verdict: PARTIALLY RESOLVED.**

*Now established*
- The mapping chain is explicit: `testtable` (609 rows, 38 columns) → `subgroupcode` → `subgrouptable` (48 rows) → per-discipline profile table (`cbc`, `kidney`, `liver`, `lipids`, `BloodGases`, `urine`, `stool`, …).
- Composite **packages** are fully enumerated: `customgrouptable` (CHECKUP 1–5) and `customtesttable` (**54 rows**) give each package's exact `testcode` membership and price. Example — CHECKUP 1 = testcode 4 (Urine) · 5 (Stool) · 8 (SGPT) · 9 (SGOT) · 1 (F.B.G.) · 15 (Bilirubin Total) · 11 (Creatinine) · 56 (CBC).
- Order-row storage is confirmed: `patienttest` carries `RESULTT` plus `resultt1..resultt4` — up to **five** result components on one row — and composite panels store the sentinel `RESULTT = 'See Report'` while their analytes go to the profile table (observed on the `testtemp` row for testcode 70 / Blood Gases, `testprice = 175`).
- Report templates bind by `[reportdata] testtype=` across **228** INIs under `Data\` (`''` 159, `pcr` 33, `r` 26, `pnt` 10), with a per-analyte key vocabulary (`ITEM*`, `LABEL*`, `LINE*`, `TIGA*`, `TIGG*`, `CMVIGG*`, `ITEMG*`, `RUBIGG*`, `ITEMX*`, `ITEMw*`, `Acidic05_*`, `Alkalin05_*`, `Nitrl05_*`, `Her2IGG*`, `Her2IGM*`, `CMVIGM*`, `TIGM*`, `ITEMD*`, `ITEMH*`, `ITEML*`, `ITEMR*`, `ITEMT*`, `ITEMU*`, `ITEMV*`, `ITEMS*`) including `F`/`M`/`D` sex variants and `05`/`20` age bands.

*Still outstanding*
- The **per-`testcode` analyte-component list** (which analyte columns of which profile table belong to which `testcode`) — the `testtable` schema has **no** composite-declaration column; the mapping is implicit.
- The **write/validation path** — writes are bound-recordset `AddNew`/`Update` operations; there is **no literal `INSERT`/`UPDATE`** to trace (verified negative). The composite-test **fetch** side is however now localised to the function `FUN_01e27d40` (`SELECT patienttest.patientid, patienttest.subgroupcode, patienttest.test…`), and the **order-row** fetch to `FUN_02d7b200` (`Select testcode, testsimp, arrangnum, RESULTT, RESULTCASEPRINT …`).

---

## Gap 7 — Per-invoice financial computation and `discount100` semantics

**Verdict: PARTIALLY RESOLVED.**

*Verified verbatim from the authentic binary*
- Aggregate net formula at **`0x69EAB8`**:
  `Select pricemethod, Round(sum(totalprice),2) as tot1, Round(sum(totalpricedisc),2) as tot2, Round((sum(totalprice) - sum(totalpricedisc)),2) as tot3, Round(sum(elmadfoo3),2) as tot4, Round(sum(elbakyma3mal),2) as tot5 from patientinfo where requestdate >= '`
  → net = gross − discount, rounded to two decimals.
- Referrer commission at **`0x694CD4`**:
  `SELECT ROUND(SUM(Gehats.omola * patientinfo.totalpricedisc / 100),2) AS Expr1 FROM patientinfo INNER JOIN Gehats ON patientinfo.referredbyname = Gehats.gehahname Where requestdate >= '`
  → the base is **`totalpricedisc`** (refinement over the prior "amount" wording).
- Company net at **`0x7226B8`**: `SELECT wareed , saader, (wareed - saader) as farq , … FROM sharekathesab where sherkaid= '`
- Invoice header at **`0x6A57D8`**: `SELECT patientid, totalprice, totalpricedisc, elbakyma3mal, discount100, discount, elmadfoo3, completes FROM patientinfo where patientid='`
- Unpaid predicate at **`0x6CEA64`**: `… FROM patientinfo Where (totalpricedisc …`

*Still outstanding*
- `discount100` is a **double** (OLE DB 5) stored beside a separate `discount` and distinct from the applied amount `totalpricedisc`. A percentage reading is plausible but **UNVERIFIED** — the computation is in VB code, not in SQL, and the target was not executed.
- The per-invoice arithmetic (how `totalprice`, `totalpricedisc`, `discount100`, `elmadfoo3` and `elbakyma3mal` are derived for one invoice) remains **UNKNOWN** for the same reason.

---

## Gap 8 — Runtime behaviour

**Verdict: BLOCKED, by design and by environment.**

The target was **not executed**. Reasons, precisely:

1. **Safety** — an unreviewed clinical-laboratory system was not to be run on the analyst workstation.
2. **No isolated environment** — the host has **no administrative rights**, Hyper-V is absent, the Docker daemon is stopped with no CLI on `PATH`, and WSL2 has only a stopped `docker-desktop` distro. There is no VM, sandbox or emulator available to host the execution.
3. **Dependency surface** — the target requires `MSVBVM60.DLL`, 12 OCX controls, the Jet-4.0 stack, ADO/DAO, the Bullzip PDF printer and a SQL Server Express instance; reproducing that faithfully is itself a project.

**Consequence:** zero runtime behaviours are observed. All behavioural statements in this output remain static reconstructions. This limitation is **unchanged** from the prior pass and is restated rather than removed. The one materially new datum about runtime is the **DDL migration region**, which is a static finding that identifies *where* start-up work happens even though the trigger event itself is not observed.

---

## Additional gaps discovered by this pass (not in the original list)

| New gap | Verdict | Note |
|---|---|---|
| Ghidra-derived native function inventory | **RESOLVED** (after four attempts) | attempt 1 crashed `0xC00000FD` STACK_OVERFLOW; attempt 2 ran with the **default** analyzer profile because the pre-script failed to compile, timing out at 7,200 s (9,077 s of analyzer time) but still **saving the program**; attempt 3 exposed two further API errors; attempt 4 re-processed the saved project with `-noanalysis` and produced **8,021 functions**, 35,063 defined data items and the complete string cross-reference set. The saved project is reusable. |
| Function → SQL statement mapping | **RESOLVED** | 928 functions carry SQL statements; `FUN_01b61cf0` alone carries the 250-statement schema migration, `FUN_02d36160`/`FUN_01db4350` the 51-statement deletion cascades, `FUN_03e7ddc0`/`FUN_03e1fcd0` the result-lifecycle queues. |
| Function → VB6 object / control name | **STILL UNKNOWN** | the per-object `ObjectInfo` was fully decoded, but the field holding the procedure→code mapping holds values below the image base (e.g. `0x001A5DD4`) that are neither VAs nor file offsets. |
| Access system-table metadata (defaults, required flags, validation rules, relations) | **BLOCKED (ACL)** | `MSysObjects`, `MSysQueries`, `MSysRelationships` return *"no read permission"*; `MSysColumns`/`MSysIndexes` are not present in these Jet-4 files. |
| Stored QueryDefs | **RESOLVED as absent** | no saved queries in any of the five databases; the application issues SQL directly. |
| Per-form control → event-handler address map | **PARTIALLY RESOLVED** | the 16-byte control records were decoded (event descriptor, index, control name, flags) and sampled per form; a full map over all 210 objects was not completed. |
| Prior virtual addresses | **CONFLICTED → CORRECTED** | all 2,065 prior VAs are wrong by +0x42/+0x44; the re-anchoring table is in `03_SQL_CORPUS_AND_CODE_BINDING.md` §6. |
