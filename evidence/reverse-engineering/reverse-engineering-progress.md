# Reverse-engineering progress log — RefLab V2 (version 1.605)

**Mission:** forensic reverse engineering of `Patients.mdb` and `real lab system.exe`
**Source of truth:** `F:\REAL_LAB_COPY` (read-only)
**Started / last updated:** 2026-11-14

---

## Phase status

| Phase | Operation | Status | Result |
|---|---|---|---|
| 1 | Read the complete environment audit | `COMPLETED` | 452 lines read in full; 207 REA tools, Ghidra 12.1.4, ACE/OLEDB, SQL Server 2022 Express, no admin rights, no containers, no C/C++ toolchain |
| 2 | Verify filesystem and tool accessibility | `COMPLETED` | Both targets located; PS 7.6.10, .NET 8/9, Python 3.13.1, Java 21, `rea` 6.7.0 all callable |
| 3 | Identify and test REA capabilities | `COMPLETED` | `rea --version` → 6.7.0; `rea doctor` shows Ghidra healthy **once `JAVA_HOME` is set for the invocation** (the audit's documented fix, applied process-scoped only); Hopper/IDA/JADX/Kuna unconfigured and not required |
| 4 | Review the cumulative documentation | `COMPLETED` | `RefLab_Reference_V2_Consolidated.zip` (9 documents), the three reference-index files, and the P11 analysis tree (`analysis/BATCH-04/{vb6,sql_v1605,pe-reports,schema}`, `analysis/BATCH-07/db`) reviewed for prior findings on both targets |
| 5 | Establish execution tracking | `COMPLETED` | `reverse-engineering-execution-state.json`, this log, the evidence register, the file-accountability ledger and the unresolved-issues register created under `evidence/reverse-engineering/` |
| 6 | Verify `Patients.mdb` identity and integrity | `COMPLETED` | 2,682,880 B; SHA-256 `98D51F52…E898A3`; Jet 4 / Access 2000 container; not encrypted; no corruption indicators |
| 7 | Database structural examination | `COMPLETED` | 114 catalog objects (103 user tables + 11 system), 1,682 user columns, 72 indexes, **0 declared relationships**, **0 saved queries**; ADOX position/size/required properties are **not populated** for this Jet 4 file, so field metadata was taken from the independent `mdbtools` dump of the same file |
| 8 | Complete record extraction and reconciliation | `COMPLETED` | 3,809 of 3,809 records, **0 failed**, 0 partial; three-way count reconciliation (engine / ledger / export file) with zero mismatches; per-table export SHA-256 recorded |
| 9 | Generate `Database documentation.md` | `COMPLETED` | 204 KB / 3,299 lines / 31 sections |
| 10 | Verify `real lab system.exe` identity and integrity | `COMPLETED` | 76,292,096 B; SHA-256 `C54CE6C8…DF93C0`; PE32 x86 native; VB6 (MSVBVM60.DLL); linker 6.0; entry RVA `0x7142c`; PE timestamp `1595435962` (2020-07-22) |
| 11 | Executable static reverse engineering | `IN PROGRESS` | delegated workstream; PE identity, resource directory, classified strings (ASCII + UTF-16), VB6 object table and literal pool extracted |
| 12 | Dynamic analysis | `NOT PERFORMED` | not authorised for this stage; the static evidence answers the open questions |
| 13 | Generate `Documenting the Executable File.md` | `PENDING` | — |
| 14 | Reconcile with the cumulative knowledge base | `PENDING` | — |
| 15 | Update evidence and accountability records | `IN PROGRESS** | database side complete; executable side pending |
| 16 | Independent verification | `PENDING` | — |
| 17 | Final Arabic report | `PENDING` | — |

---

## Milestone log

**M1 — Environment verified.** PowerShell 7.6.10 is the session shell; `rea` resolves to
`C:\Users\LAP LINK\npm-global\rea.ps1` at 6.7.0. `rea doctor` reports `healthy: false` overall because
Hopper, IDA, JADX and Kuna are unconfigured — but every **Ghidra** check (`ghidra`, `ghidra-platform`,
`ghidra-architecture`, `ghidra-installation`, `ghidra-version`, `ghidra-headless`,
`ghidra-native_decompiler`, `ghidra-java`, `ghidra-native_authority`) passes once `JAVA_HOME` points at
the installed JDK 21. That is the audit's own remediation ("fixable without installation by defining
JAVA_HOME to the existing JDK 21"), applied per-invocation; no system setting was changed.

**M2 — Database identity established.** The mission brief's path
(`F:\REAL_LAB_COPY\RL_CD2019Access\bin\Patients.mdb`) does not exist; the file is at
`F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\bin\Patients.mdb` — the intermediate
`real lab system\` level was simply omitted. The V1.605 payload lives under `RL_CD2019Access\`; the
sibling tree is the earlier build and was excluded from substantive analysis.

**M3 — Format determined from the bytes, not the extension.** The file carries the `Standard Jet DB`
magic at offset 0x04 and the Jet-4 format indicator `0x00000001` at 0x14. It opens with no password, so
it is not encrypted. The ACE 12.0 provider reads it as an Access database; ADOX enumerates 114 objects.

**M4 — Complete extraction achieved.** 103 tables, 3,809 records, 0 failures. The first extraction pass
failed on 41 rows because Jet date/time columns are surfaced as `adDouble`; the conversion was made
value-driven (DateTime → ISO-8601, byte[] → base64, numeric → JSON number, NULL preserved as `null`,
empty string preserved as `""`) and the pass was re-run to 0 failures.

**M5 — Confidentiality assessed and a policy applied.** The shipped image contains **no patient rows**
in `patientinfo` or `patienttest`, but it does contain residual patient-level rows in nine tables
(`anyreport` 15, `SMSSender` 8, `pictures` 6, `Helicobacter` 3, `ratios` 2, `ESR` 1, `hba1c` 1,
`testtemp` 1, `TORCH` 1 — 16 distinct 13-character patient identifiers) and hard-coded credentials in
`patients.patientword`, `patients.SecWord`, `MyLabSetting.accuntpw` / `accuntID` / `SMSbakey` and
`reportview.strip3` / `strip4`. The **published** exports therefore carry a declared per-column
transformation (credentials redacted, patient identifiers pseudonymised, telephone numbers masked); the
untransformed extract was written only to the restricted local workspace and is **not** inside the
repository.

**M6 — New findings that the cumulative specification did not contain.**

1. `reportview` — a single row of 42 columns — **stores the SQL Server account name and password in
   `strip3` / `strip4`**, i.e. the credential previously documented only from `BASE.ini` and the
   installer script is also present *inside the database*.
2. The shipped database contains **1,152 `normalrange` rows**, not the 852 published in the V2
   capability catalogue; `antibiotic` holds **612** rows against 38 published, `antibio` 95 against 96,
   `testunits` 70 against 69, `subgrouptable` 48 against 46, `PTC` **35** rows against 33.
   `testtable` (609), `abbreviations` (650) and `converts` (167) agree.
3. `constanttable` holds **both** constant groups with their values — `PTCONS` = 12, 35, 1.33, `2CVJW`,
   0, " 4021", 94 and `CBCCONS` = 0, 6.25, 6.75, 7.5, 8.25, 100, `BD111`. The cumulative specification
   recorded the CBC factors as 3.3 / 6.25 / 6.75 / 7.5 / 8.25; **3.3 is not stored in the table** — it is
   the manual's `Hct = Hgb × 3.3` factor.
4. `patients` holds the **shipped permission seed**: account `admin` with `patientad` = true and **all
   seventeen `grant` bits false**, and account `user` with `patientus` = true and `grant1-4`, `grant6`,
   `grant8`, `grant10-12` and `grant13` true. Authorisation for the administrator is therefore carried by
   the administrator flag, not by the grant bits.
5. `SMSSender.patientid` stores **lab identifiers 100 / 200 / 300** — the same values as
   `MyLabSetting.LabID` — not patient identifiers: a naming trap.
6. `MyLabSetting` holds **five rows**, one per branch slot (`LabID` 100/200/300/400/500), with the
   SMS-gateway credential **populated only in the `LabID` = 100 row**; `Timess` is a **48-character**
   schedule string.
7. The application references a table named **`samples`** that is neither in the shipped database nor
   created by the embedded DDL.

**M7 — Cross-check against the application's own SQL.** The cumulative corpus of 1,797 statements
extracted from the v1.605 executable was matched case-insensitively against the live inventory:
**101 of the 103 live tables are referenced**; only `Paste Errors` and `patienttestgroup` are never named;
and **seven referenced tables are absent** from the shipped database — `FMF`, `IgEallergy`, `LBC`,
`loger`, `useraccounts`, `win32_processor` and `samples`. The first six confirm the cumulative
specification's account of the DDL-created tables that do not ship and of the SQL-Server-resident
relations; `samples` is new. A naive case-sensitive comparison makes 59 tables look absent — the
case-insensitivity trap the cumulative specification already flags, reproduced and corrected.

**M8 — Extraction methodology hardened.** All 103 tables were exported with explicit type fidelity, and
the exported files were then re-counted independently and reconciled with the engine's own `COUNT(*)`
and with the extraction ledger. The 1,682-column total reproduces the figure published in the
cumulative specification exactly.

---

## Artefact locations

| Artefact | Location |
|---|---|
| Working scripts (not part of the deliverable) | `C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\` |
| Restricted, untransformed database extract | `…\reflabv2\mdb\exports_full\tables\` — **outside the repository** |
| Published (transformed) per-table exports | `evidence\reverse-engineering\exports\tables\` (103 files) |
| Object / column / index metadata | `evidence\reverse-engineering\metadata\` (7 CSVs) |
| Reproduction scripts | `evidence\reverse-engineering\scripts\` (5 scripts) |
| Executable workstream outputs | `…\reflabv2\exe\` (working directory) |

---

## Open items carried into the unresolved-issues register

`DQ-09` `SMSSender.patientid` semantics · `DQ-10` `normalrange.subgroupcode = 0` sentinel or dangling
reference · `DQ-11` provenance of the 16 residual patient identifiers in the shipped image ·
`L-DB-01` forms/reports/macros/modules not exposed by the available tooling · `ISS-07` dynamic behaviour
not established.
