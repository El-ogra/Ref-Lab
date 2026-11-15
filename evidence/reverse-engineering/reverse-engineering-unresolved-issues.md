# Unresolved issues register — RefLab V2 (version 1.605) forensic investigation

**Mission:** forensic reverse engineering of `Patients.mdb` and `real lab system.exe`
**Stage 3 (2026-11-14):** read-only investigation of the authorised V1.605 source at `F:\REAL_LAB_COPY`
**Status:** open issues are carried forward from the cumulative specification and extended by the Stage 3 findings.

This register records what remains **unresolved** after Stage 3. It is the companion of
`reverse-engineering-evidence-register.csv` (what *is* established) and of the gap catalogue in
archive member `07_GAPS_AND_LIMITS.md`.

---

## A. Issues opened by Stage 3

These are new. Each states the precise limitation, its consequence, and what would close it.

| ID | Subject | The precise limitation | Consequence | What would close it |
|---|---|---|---|---|
| **UQ-DB-01** | Provenance of the residual patient-level rows | Nine tables in the shipped image carry **18 distinct 13-digit patient identifiers** (`anyreport` 15, `SMSSender` 8, `pictures` 6, `Helicobacter` 3, `ratios` 2, `ESR` 1, `hba1c` 1, `testtemp` 1, `TORCH` 1). Whether these are demonstration data or real patient records **cannot be determined from the image**. | Any decision to reuse, seed or republish the shipped reference data must treat them as potentially real. | A statement from the publisher, or provenance from the deployment that produced the image. |
| **UQ-DB-02** | The 852-range catalogue figure | The shipped `normalrange` holds **1,152** rows. The catalogue's **852** is not reproducible under any tested reading (1,152 rows; 1,148 excluding 4 placeholder rows; 943 rows carrying a limit; 622 distinct `(testcode, subgroupcode, sex)` triples; 846 distinct `(testcode, subgroupcode, sex, ageunit)` tuples). | The catalogue's per-table row counts cannot be used as a description of the shipped store. | The catalogue's own counting method, or a second live installation to compare against. |
| **UQ-DB-03** | The `samples` relation | The application's SQL addresses `samples` in 4 statements and the `Patient_samples` form matches it exactly, but `samples` is **neither** among the 103 shipped tables **nor** created by the embedded DDL. | A replacement must decide whether `samples` exists and in which deployment mode it is expected. | The SQL-Server schema of the `Patients` catalog, or a live SQL-Server installation. |
| **UQ-DB-04** | The password-encoding scheme | `patients.patientword` holds a **length-preserving, non-ASCII obfuscated** value (5 characters for `admin`, 4 for `user` — in both cases the same length as the account name). The encoding is **not recoverable** from the available evidence. | The shipped credential format cannot be reproduced, and an existing installation's stored passwords cannot be migrated. | Decompilation of the comparison routine, or a vendor statement of the scheme. |
| **UQ-DB-05** | `constant8` in `constanttable` | `constant8` is **empty in both rows**. Whether it is reserved, unused, or holds a value in a populated installation is unknown. | The clinical-constant contract has one undefined slot. | A populated installation, or the DDL that creates the table. |
| **UQ-DB-06** | The `pregsmok` flag | The column is populated on every row — **1,149 rows `0.0`, 3 rows `1.0`** — so it is a two-valued flag on a minority of rows. Its **meaning** (pregnancy? smoking? a factor?) is still **not established**. | A reference-range rule that depends on this flag cannot be reimplemented with certainty. | The rule that consumes the flag, i.e. decompilation or a vendor statement. |
| **UQ-DB-07** | `normalrange.subgroupcode = 0` | 14 rows use subgroup code `0`, which has no matching row in `subgrouptable`. Whether `0` is a deliberate "no subgroup" sentinel or a dangling reference is not established. | A range lookup must decide how to treat subgroup `0`. | The range-lookup logic from the executable. |
| **UQ-DB-08** | `SMSSender.patientid` semantics | The 8 `SMSSender` rows carry the values 100, 200 and 300 in `patientid` — the same values as `MyLabSetting.LabID` — so the column is used as a **lab reference**, not a patient key. Whether application code ever reads it as a patient key is not established. | A data model that treats it as a foreign key to `patients` would be wrong. | The SQL that reads `SMSSender`. |
| **UQ-DB-09** | The orphan `normalrange` subgroup rows | 14 rows carry a `testcode` of `""` (empty) and `testsimp` of `xxx` — placeholder rows that do not join to `testtable`. | Seed data quality; a range lookup must not assume every row joins. | A statement of what the placeholder rows are for. |
| **UQ-EX-01** | Procedure bodies | The **349** recovered compiled-code entry points are prologue-verified but **not name-linked** to the 11,280 member descriptors, and no decompilation was achieved. | The behaviour of individual procedures cannot be stated with certainty. | A working decompiler over the 349 addresses, or a VB6-aware tool. |
| **UQ-EX-02** | The startup form | The VB header carries `lpSubMain = 0`, so the application starts with a startup **form**. The candidates are `start`, `login` and `server`; the selection is a runtime property and was **not settled statically**. | The exact boot sequence is inferred, not established. | Decompilation of the startup path, or a run trace. |
| **UQ-EX-03** | `patienttestgroup` | The table exists in the shipped store (0 rows) but is **never named** in the extracted SQL corpus, unlike every other table except `Paste Errors`. | How grouped orders are reached is unexplained. | The code path that addresses it. |
| **UQ-EX-04** | The `admin` literal's account-name role | **RESOLVED in Stage 3** — the single UTF-16LE `admin` literal at file offset `0x2DA262` belongs to the phrase *"On win7 right click on file and press run as administrator"*, an installation instruction, not an account-name string. The consequence is that the **account-name literal is absent from the binary**, so whether the application ever compares a password against a literal cannot be established from the executable alone. | Login behaviour must be inferred from the database and the UI captions. | The credential-comparison routine. |
| **UQ-EX-05** | The DDL execution trigger | 241 `ALTER TABLE … ADD COLUMN` and 22 `CREATE TABLE` statements are present, but **when** they run — at startup, on demand, or on first use of a form — is not established. | The schema-evolution policy of a replacement must be chosen, not copied. | Decompilation of the migration call site. |
| **UQ-EX-06** | The write-side SQL | **No `INSERT INTO` and no `UPDATE … SET` literal exists** in the pool; row writes are parameterised, so the write SQL text is unrecoverable. | The write path cannot be compared against a replacement's write path. | Decompilation of the recordset code paths. |
| **UQ-EX-07** | Per-form control attribution | The ANSI control-name blob and the 247 member tables are two **independent orderings**; the slots that would join them are zero-filled in the file image. | Per-screen control inventories cannot be produced from the executable alone. | A VB6-aware decompiler, or the original project. |
| **UQ-EX-08** | The `z_*` maintenance forms | `z_logs`, `z_gehats`, `z_titles`, `z_group`, `z_customgroup`, `z_units`, `z_barcodes`, `z_branchs`, `z_pricelists` and `z_labaccount` are named but their individual table mappings are not established. | Each needs its own mapping before a replacement can implement it. | Decompilation, or per-form SQL attribution. |
| **UQ-EX-09** | `reportview.strip1..4` layout | The table is read by the application (`SELECT stripnum,stripdiscrp,strip1 FROM reportview where stripdiscrp='MAIN'`), but the mapping from the strip columns to report regions is not recoverable. | The report-strip configuration contract is undefined. | Decompilation of the report writer. |
| **UQ-EX-10** | Third-party control versions | `ChamaleonButton` and `Redsoul911XPControls` are referenced through `.oca` type libraries that carry **no version information**. | The correct control versions for a replacement cannot be identified from the binary. | The original build environment or the vendor. |

---

## B. Carried forward from the cumulative specification

These remain open at the cumulative level and are unaffected by Stage 3. The full statements, with both
readings and their consequences, are in archive member `07_GAPS_AND_LIMITS.md` (§3.2, §3.3) and
`08_DECISIONS_REQUIRED.md`.

| Gap | Subject | Status after Stage 3 |
|---|---|---|
| GAP-02 | Extracted CAB runtime binaries | Partly answered in Stage 2; the 96 extracted members remain absent from the deliverable tree. |
| GAP-03 | The shipped credential in configuration files | **Enlarged by Stage 3** — a seventh location was found, inside the database itself (`reportview.strip3`/`strip4`). Routed to **DEC-13**. |
| GAP-05 | Provenance of the published "1,617 columns" figure | Open. The re-counted **1,682** is the usable figure. |
| GAP-06 | `RL_Show.pdf` (77 pages) content | Open — no extractable text layer. |
| GAP-07 | `RLS_Setup.pdf` pages 2–33 | Open — image-only. |
| GAP-08 | Body text of `RLS_Learn.pdf` | Open — no `ToUnicode` CMap. |
| GAP-09 | Any runtime behaviour of the product | Open. Stage 3 performed **no dynamic analysis**; every finding remains static. |
| GAP-11 | Contents of the five absent Microsoft installers | Open. |
| GAP-14 | `VIDAS` | Open — no `VIDAS` artefact exists in the source. |
| GAP-15 | The three columns of `Helicobacter` absent from the shipped table | Open — deployment-scoped. |
| GAP-16 | `Interface` catalog schema | Open. |
| GAP-17 | The `patientid` encoding rule | Open — the 13 substring extractors are known, the encoding rule is not. |
| GAP-18 | `FMF` columns beyond `patientid` | Open — table absent from the shipped store. |
| GAP-19 | The final item count of `IgEallergy` | Open — the DDL literals are truncated; Stage 3 recovered the *pattern* but not the exact final index. |
| GAP-20 | `useraccounts` full column set | Open — SQL-Server-only table. |
| GAP-21 | `MyLabSetting` scheduling semantics | Partly resolved; `FridaySend`, `shortsms`, `Hourflag` and `Timess` are now observed in the shipped rows, but their interaction is not established. |
| GAP-22 | The `loger` full column set | Open — SQL-Server-only table. |
| GAP-23 | `samples` full column set | **Enlarged by Stage 3** — confirmed absent from the shipped store and not DDL-created (UQ-DB-03). |
| GAP-24 | `LBC` columns beyond the 18 in the Jet DDL | Partly resolved. |
| GAP-25 | The orphaned `SetupFN4.bat` | Open. |
| GAP-26 | The consolidation rule for the documentation layers | Open — routed to **DEC-11**. |
| GAP-29 | Whether the branch digit is position 9 alone or the 2-char pair | Partly resolved. |

---

## C. Tooling limitations (not evidence gaps)

| ID | Limitation | Effect | Evidence |
|---|---|---|---|
| **TL-01** | **REA's Ghidra bridge cannot process this binary.** Four runs were attempted and logged: the provider startup deadline was exceeded while importing the 76 MB image; two runs aborted with `IllegalArgumentException: Unknown Ghidra analyzer for this program`; a fourth run with a 7,200,000 ms deadline **timed out**. | No decompilation was produced. | `evidence/reverse-engineering/exe/tool_log.md`; `Documenting the Executable File.md` §29.1 |
| **TL-02** | **The REA MCP path is hard-blocked for this artefact.** `inspect_pe_resources` rejects the file against a caller-selected 64 MiB budget, and the long-lived MCP server process has no `JAVA_HOME`, so its Ghidra provider is unavailable — the CLI path can set it per invocation, the MCP process cannot. | Only identity could be confirmed through MCP; the PE and resource work was done with an independent parser. | `Documenting the Executable File.md` §29.1 |
| **TL-03** | **The ACE OLEDB provider returns an unusable schema table on this Jet 4 file**, and refuses `MSysObjects` outright. | Object enumeration had to go through ADOX. Two methods, one denied, one working — the object inventory is complete either way. | `Database documentation.md` §6 |
| **TL-04** | **No Jet object-catalogue reader was available**, so forms, reports, macros and modules stored inside the `.mdb` cannot be listed. | Carried as `L-DB-01`. The probability of stored form definitions is low — the application is a VB6 program whose forms live in the executable — but this is **not established**. | `Database documentation.md` §29 |
| **TL-05** | **The REA file-backed readers cap at 64 MiB**; the executable is 76,292,096 bytes. | Every PE, resource and VB6 structure result was produced by an independent parser written for this mission and is reproducible from the scripts stored with the evidence. | `Documenting the Executable File.md` §8 |

---

## D. Deliberate non-actions

| ID | Action not taken | Reason |
|---|---|---|
| **NA-01** | **No dynamic analysis of the executable.** | Not authorised for this stage, not safe in this environment (the application resolves its store by default to the real patient database, and a launch could attempt to reach a live SQL Server `Patients` catalog), and not required — the static evidence answered the open questions. Recorded as L-DB-08 and L-EXE-15. |
| **NA-02** | **Credential values were not reproduced** in any deliverable, export or report. | Presence, type, location, length and populated/empty state only. Applies to `patients.patientword`, `patients.SecWord`, `MyLabSetting.accuntID` / `accuntpw` / `SMSbakey` and `reportview.strip3` / `strip4`. |
| **NA-03** | **Patient identifiers were pseudonymised** in the published exports. | The mapping is retained only in the restricted local workspace, outside the repository. |
| **NA-04** | **No source row counts were harvested for the 62 empty tables.** | The tables carry no rows; the ledger records the authoritative `COUNT(*)` for every one of the 103 user tables regardless. |
| **NA-05** | **Nothing under `F:\REAL_LAB_COPY` was modified**, and no binary or installer was executed. | Verified independently by re-hashing every source artefact after the investigation: all sixteen hashes are unchanged. |

---

## E. Owner decisions still outstanding

All thirteen remain open. **Stage 3 closed none of them** — a decision is an owner's choice, not an
evidence question, and new evidence can inform a decision without making it.

| Decision | Subject | Stage 3 effect |
|---|---|---|
| DEC-01 | Which database engine(s) the replacement supports | Informed — the executable's three back-ends (Jet OLEDB 4.0, SQLOLEDB.1, MSDataShape) are now documented. |
| DEC-02 | Which application root and data root are supported | Informed — the executable hard-codes `D:\real lab system\bin\Patients.mdb`. |
| DEC-03 | Whether the four PDF output roots stay hard-coded | Informed — the four roots and the Bullzip PDF printer are confirmed in the binary. |
| DEC-04 | Whether the shipped PDF engine dependency is kept | Unchanged. |
| DEC-05 | Third-party licence posture of the print stack | Unchanged. |
| DEC-06 | Supported operating-system baseline and runtime payload | Unchanged. |
| DEC-07 | How the SMS gateway credentials are stored | **Enlarged** — the credential is populated only on the `LabID` = 100 row, and a second, larger exposure (the SQL Server credential inside the database) now belongs to the same question. |
| DEC-08 | Whether `VIDAS` is part of the supported data path | Unchanged — no `VIDAS` artefact exists in the source. |
| DEC-09 | Installation-completeness policy for the five absent installers | Unchanged. |
| DEC-10 | Whether the `English (Canada)` display-language prerequisite stays | Unchanged. |
| DEC-11 | Which documentation layer is the consolidated record | Unchanged. |
| DEC-12 | Installer behaviour: font step, repair path, elevation model | Unchanged. |
| **DEC-13** | **How the SQL Server credential stored inside the database is handled** | **New in Stage 3.** |
