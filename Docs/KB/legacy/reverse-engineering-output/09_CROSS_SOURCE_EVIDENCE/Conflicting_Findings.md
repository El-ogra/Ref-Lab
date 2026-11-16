# Conflicting Findings — Real Lab System v1.605 (V-CD-2019)

**Method.** Each entry gives Side A, Side B, the evidence IDs/filenames for both, and a verdict:
**RESOLVED** (both statements true under different definitions), **UNRESOLVED** (genuine
open question), or **CONFLICTED** (irreconcilable with available evidence — both preserved).
Nothing here is silently discarded.

---

## C-01 — Build timestamp: 2013-12-27 vs a 2020 value — **CONFLICTED; corrected from primary dump**

| Side | Statement | Source |
|---|---|---|
| A | Both v1.00 and v1.605 executables carry PE `TimeDateStamp` **2013-12-27**; "do not use the PE timestamp as a version discriminator" | `Version_Identity_and_Scope.md` §3.1 note and §4.2 (E056); `pe-metadata.json` (E012) |
| B | v1.605 PE link timestamp is `0x5F186BBA`, but its calendar date is stated as **2020-07-24** | `contradictions-limitations-and-unresolved-questions.md` §1.2 (E057, FND-F-011/REL-011) |
| C — **primary dump, preferred** | `real lab system.exe (2).json` and `pe-structure.json` both give `timestamp = 1595435962` = `0x5F186BBA` = **2020-07-22T16:39:22Z** (18:39 in the host's +02:00 zone). The v1.00 dump gives `1388267701` = **2013-12-28T21:55:01Z**. | `real lab system.exe (2).json`, `real lab system.exe.json`, `pe-structure.json` (E012, E015) |
| D | v1.605 installer `RL_SETUP.exe` is dated **2020-09-01**; learning guide **2017-04-26**; payload 2019–2020 | `Version_Identity_and_Scope.md` §3.1/§3.5 (E056); `1605-phase2-findings.md` (E055) |

**Verdict.** The **hex value and primary dumps agree** (`0x5F186BBA` = 1595435962); side B's
*calendar conversion* (2020-07-24) is arithmetically wrong by two days — the correct UTC date is
**2020-07-22**. Side A ("both 2013-12-27") is contradicted by the v1.605 primary dump and is
therefore corrected: the two builds have **different** PE timestamps. Side C/D remain consistent
with a 2020 product. Version chronology is still not provable from PE timestamps alone, but the
timestamp values themselves are VERIFIED.

## C-02 — CHM byte-identical vs 41 new objects — **RESOLVED (both true)**

| Side | Statement | Source |
|---|---|---|
| A | `real lab system help.chm` is byte-identical between versions: 5,193,985 bytes, SHA-256 `624308649E523400…`, 15 topics, **zero changed characters** | `Version_Identity_and_Scope.md` §3.5/§6.1 (E056); `chm-analysis.json` (E089), `chm-full-text.txt` (E087) |
| B | v1.605 adds **41 new objects**, a complete SMS subsystem and 17 new tables, with "not a single line of user-manual change" | same file §3.5 (E056); `object-diff-vs-v1.json` (E005); SQL `smsgroups/smslist/SMStemplate/SMSSender` DDL (E001) |

**Verdict.** Not a contradiction: the manual is an **older artefact shipped unchanged** inside
the newer product (its footer says "version 1.0", copyright 2010). Documentation coverage of
v1.605 is therefore **incomplete**, which is why workflow reconstruction relies on SQL/objects.
Severity: documentation risk, not evidence invalidity.

## C-03 — Shipped database out of step with embedded DDL — **RESOLVED (both true)**

| Side | Statement | Source |
|---|---|---|
| A | v1.605 embeds **268 DDL statements** (27 CREATE + 241 ALTER) and the executable can migrate schema | `statements__[analysis_BATCH-04_sql_v1605].csv` (E001, counts independently reproduced); `Version_Identity_and_Scope.md` §3.2 (E056) |
| B | The shipped `Patients.mdb` does **not** reflect all of them: **61 changes absent** — 23 SQL-Server-only syntax that cannot apply to Jet 4, and 38 Access-applicable changes never executed | `Version_Identity_and_Scope.md` §3.5 (E056) |
| C | The Access mirror's `normalrange` has no `REFRANGE='2'` row, though runtime queries demand it | `1605-phase2-findings.md` NEW-3 (E055); `normalrange.csv` / `normalrange-stats.json` (E029/E030) |

**Verdict.** Consistent: embedded DDL is a *migration capability*, not a description of the
shipped data file. Consequence: the CSV dumps describe a **knowledge-layer/mirror snapshot**,
while live runtime data lives on SQL Server (uncollected). This is the single largest evidence
gap for business-rule confirmation.

## C-04 — Object count: 210 vs 212 (and 169 vs 212) — **PARTLY RESOLVED**

| Side | Statement | Source |
|---|---|---|
| A | v1.605 = **210** VB6 objects (132 forms, 66 DataReports, 9 modules, 3 classes) | `Version_Identity_and_Scope.md` §3.2 (E056); object dump `objects-full__[analysis_BATCH-04_vb6_v1605].txt` (E004) |
| B | "Full **212-object** register" and "25 VB6 external components" | `contradictions-limitations-and-unresolved-questions.md` §2.2 item 4 / UNR-009 (E057) |
| C | v1.00 = **169** objects; CON-002 "169 vs 212 objects — provisional: version delta" | `Version_Identity_and_Scope.md` §2.2 (E056); prior contradictions register |

**Verdict.** 169 vs 210/212 is explained by **different products** (C). The residual 210 vs 212
is likely a counting-scope difference (objects vs components declared) and remains
**UNVERIFIED**: no primary dump enumerates 212. Use 210 as the object-model figure and treat
212 as an unqualified register count.

## C-05 — VIDAS database: referenced but uncharacterised — **UNKNOWN**

| Side | Statement | Source |
|---|---|---|
| A | `Attach_VIDAS.bat` attaches a third database named `VIDAS`; a Visual FoxPro 6.0 runtime appears in the shipped VB6 SP6 redistributable manifest | `Version_Identity_and_Scope.md` §3.3, §5.3, RL-UNC-003 (E056); `1605-phase2-findings.md` U-11 (E055) |
| B | No structural description of `VIDAS` exists in any collected artefact; its nature (including whether it is Visual FoxPro) is **not established** | same sources |

**Verdict.** Both sides agree it is referenced; the "Visual FoxPro" reading is a hypothesis only.
No business rule may be attributed to `VIDAS`. **UNKNOWN.**

## C-06 — RL_SETUP.exe 22.95 MiB payload — **PARTIALLY RESOLVED**

| Side | Statement | Source |
|---|---|---|
| A | `RL_SETUP.exe` (23,060,480 bytes; SHA-256 `4a89958e1a32…`) carries a **22.95 MiB** compressed payload in `.text` at entropy 7.39; purpose UNKNOWN | `Version_Identity_and_Scope.md` §5.3 RL-UNC-004 (E056); `1605-phase2-findings.md` NEW-7 (E055); `rl-setup-analysis.md` |
| B | The payload was "characterised but not decomposed", its contents "not documented"; the installer is a custom VB6 self-extractor with an unidentified compression scheme; deploy manifest recoverable (XCOPY targets, UDL fragments, SQL attach scripts, printer step) | same sources (E020) |

**Verdict.** Consistent: the *container* is characterised, the *contents* are not. Enumerating
individual embedded files would require executing the installer, which is prohibited. **UNKNOWN**
for payload contents; do not claim the 22.95 MiB is missing data or malware.

## C-07 — normalrange: 852 vs 1,152 rows, and absent `REFRANGE='2'` — **RESOLVED (different scopes)**

| Side | Statement | Source |
|---|---|---|
| A | `normalrange.csv` export = **852** ranges / 304 tests; `normalrange-stats.json` distributions computed over 852 | `normalrange.csv` (E029), `normalrange-stats.json` (E030) |
| B | Live `Patients.mdb` export = **1,152** rows; `REFRANGE` values `''` 797, `'0'` 345, `'1'` 10, **none `'2'`** | `1605-phase2-findings.md` U-04 and NEW-3 (E055) |
| C | Runtime selects `REFRANGE='2'` and `REFRANGE='1'` with `pregsmok` | SQL VAs `6BEBD0`, `6BE964`, `716A38`… (E001) |

**Verdict.** Different snapshots/backends: the 852-row file is the exported knowledge layer; the
1,152-row mirror is the shipped Access DB; the `'2'` rows the runtime needs live on SQL Server
(per NEW-3). Report every statistic with its denominator and source scope. No single
"normalrange count" exists.

## C-08 — Template counts: 223 vs 228 INI files — **PARTLY RESOLVED**

| Side | Statement | Source |
|---|---|---|
| A | Configuration enumerated as **223** `Data\*.ini` files (both versions) | `Version_Identity_and_Scope.md` §2.4/§3.4 (E056) |
| B | Report corpus has **228** INI files = 117 identities; 119 previously examined, 109 new | `template-corpus-228.json` (E051), `1605-template-corpus.md` (E054), `1605-phase2-findings.md` NEW-2 (E055) |

**Verdict.** Likely different directory scopes/config sets or a count taken before the
phase-2 receipt of `02-report-definitions.zip` (which supplied the 109 numeric files). The exact
reconciliation of 223 vs 228 is **UNVERIFIED**. Use 228 for the report-template corpus and 223
as the prior configuration-inventory figure, each with its own label.

## C-09 — "Manual requires English (Canada); product stores Arabic" — **UNRESOLVED configuration tension**

| Side | Statement | Source |
|---|---|---|
| A | Manual topic 2 requires Regional Options → English (Canada) | `contradictions-limitations-and-unresolved-questions.md` §1.4 (E057) |
| B | Executable project header `dwLCID=1033`, `dwSecLCID=4105` (ar-SA); templates are Windows-1256; `patientnamear` etc. are Arabic | same (E057); `ui-labels-verified.md` (E053); `patientinfo.csv` (E038) |

**Verdict.** Compatible only if templates are read explicitly as cp1256 rather than ANSI; this
could not be determined statically. Both preserved. **UNKNOWN.**

## C-10 — MySQL driver vs SQLOLEDB/Jet connection strings — **UNKNOWN**

| Side | Statement | Source |
|---|---|---|
| A | `Setup5.bat` installs MySQL Connector/ODBC 3.51.30; `BASE.ini` has `DataSource=DESKELTEL`, `loca=E:\Lab_data` | `06-security.md` F-04 (E081); `contradictions-…` §1.6 (E057) |
| B | Connection strings use only `SQLOLEDB.1` and `Microsoft.Jet.OLEDB.4.0` | same; `06-security.md` |

**Verdict.** Either vestigial dependency or an unobserved configuration path. **UNKNOWN**; do not
assert a third live backend. The "possible MySQL third backend" appears in prior Class-3 lists
and is not adopted here.

## C-11 — `testorno`/subgroup count: 46 vs 48 — **PARTLY RESOLVED**

| Side | Statement | Source |
|---|---|---|
| A | `subgrouptable` has **46** rows/categories | `04-clinical-rules.md` §6 (E058) |
| B | `distinct_subgroups_used = 48` in `testtable` | `testtable-stats.json` (E028) |

**Verdict.** Different denominators: lookup rows vs distinct values actually used by 609 tests.
Both can be true simultaneously. The two extra values are visible in `testtable-stats.json`
(e.g. `?0` = 118, `?` = 6, `Separate` = 50 are display/placeholder categories). **RESOLVED as
scope difference**, exact identity of the extra two **UNVERIFIED**.

## C-12 — Chart of `grantN` / `SecWord` / IDPassword meaning — **UNKNOWN**

| Side | Statement | Source |
|---|---|---|
| A | Prior summaries call authorisation "coarse" and list `grant1..grant12` + `IDPassword*` | prior business-rule summaries (E060) |
| B | v1.605 DDL adds `grant13..17`, `SecWord`, `branche`; UNR-002/003 explicitly leave all meanings unresolved; U-07 finds no client-side print password | SQL (E001), `1605-phase2-findings.md` (E055), `contradictions-…` (E057) |

**Verdict.** Column existence CONFIRMED; **no permission model is evidenced**. Do not map grants
to features. **UNKNOWN.**

---

## Summary

- 12 conflicts documented: 5 RESOLVED (C-02, C-03, C-07, C-11, and partly C-06), 3 PARTLY
  RESOLVED (C-04, C-08, C-11 shares), 3 UNKNOWN (C-05, C-09, C-10, C-12), 1 CONFLICTED
  (C-01 build timestamp).
- The most consequential for business reconstruction: **C-03** (shipped DB ≠ runtime backend)
  and **C-07** (which `normalrange` rows are authoritative).
