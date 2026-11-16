# Legacy Unresolved Questions Register

**Purpose:** Consolidated register of open questions, hypotheses, and unknowns.
**Scope:** All unresolved items from the legacy reverse-engineering corpus.
**Audience:** All.
**Version applicability:** Mixed.

---

## Status vocabulary

- **CONFIRMED** — established by evidence.
- **STRONGLY SUPPORTED** — strong but not conclusive evidence.
- **INFERRED** — derived, not directly evidenced.
- **UNKNOWN** — not established.
- **CONFLICTING** — sources disagree.

## Knowledge-status vocabulary

- **CONFIRMED** · **STRONGLY SUPPORTED** · **INFERRED** · **UNKNOWN** · **CONFLICTING** (as above).

---

## 1. V2 unresolved questions (from `Complete_Reverse_Engineering_Reference_V2.md` §1.6)

| ID | Question | Status | Affected domain |
|---|---|---|---|
| RL-V2-Q-01 | What is the `VIDAS` database? | UNKNOWN | data, deployment |
| RL-V2-Q-02 | What is the `win32_processor` table for? | UNKNOWN (likely WMI licensing probe) | data, security |
| RL-V2-Q-03 | What is the `loger` table? | UNKNOWN (attendance or audit) | data, business |
| RL-V2-Q-04 | Is `Patient_samples` a tracking table or a sample store? | UNKNOWN | data |
| RL-V2-Q-05 | How are `grant13..grant17` and `useraccounts.gtype/hdd1/hdd2` used? | UNKNOWN (schema only) | security |
| RL-V2-Q-06 | Is `LBC` a clinical profile? | UNKNOWN | clinical |
| RL-V2-Q-07 | Does `loger` record clinical events or operator attendance? | UNKNOWN | data, business |
| RL-V2-Q-08 | What is the difference between SQL `patients` and Access `patientinfo`? | UNKNOWN | data |
| RL-V2-Q-09 | What are `insulin` and `antibio`? | UNKNOWN | clinical |
| RL-V2-Q-10 | What is the 22.95 MiB payload inside `RL_SETUP.exe`? | UNKNOWN (custom VB6 container, not decomposed) | deployment |
| RL-V2-Q-11 | Are `Samples` / `reportview` / `mylabsetting` shared with V1? | RESOLVED — all three are V2 additions | data |
| RL-V2-Q-12 | Which of the 4 PDF output categories is the default? | UNKNOWN | pdf-printing |

## 2. V1 unresolved questions (from `Complete_Reverse_Engineering_Reference_V1.md` §1.6)

| ID | Question | Status | Affected domain |
|---|---|---|---|
| RL-V1-Q-01 | Is `MSDataShape.1` the default connection path, or an alternative? | UNKNOWN | data, configuration |
| RL-V1-Q-02 | Why does `history` differ between `Patients.mdb` (4 rows) and `data.mdb` (0 rows)? | UNKNOWN | data |
| RL-V1-Q-03 | Is `normalrange.pregsmok` actually consulted during range calculation? | UNKNOWN (OBSERVED, not traced) | clinical |
| RL-V1-Q-04 | What are the 60 `Data\*.ini` files with no numeric profile name? | UNKNOWN | clinical, configuration |
| RL-V1-Q-05 | What do `zarf` / `zarfprinter` denote? | UNKNOWN | pdf-printing, configuration |
| RL-V1-Q-06 | How are `patienttest.nolowexe` / `nohighexe` used — stored exception text or computed? | UNKNOWN | clinical, data |
| RL-V1-Q-07 | Why does `Data\` hold 223 `.ini` files against 46 sub-groups in `subgrouptable`? | UNKNOWN | clinical, configuration |
| RL-V1-Q-08 | What distinguishes `antibio` (95 rows, 2 cols) from `antibiotic` (38 rows, 25 cols)? | UNKNOWN | clinical, data |
| RL-V1-Q-09 | What are `customtesttable` / `customgrouptable`? | UNKNOWN | clinical, data |

## 3. Phase-2 findings open items (from `1605-phase2-findings.md`)

| ID | Question | Status | Affected domain |
|---|---|---|---|
| U-01 | How is the patient's age reconciled with the age bands' units? | RESOLVED — no unit conversion; runtime pins band to patient's own unit | clinical |
| U-02 | Which band wins when two bands contain the patient's age? | RESOLVED — no tie-break; `SELECT Top 1 …` with no ORDER BY; engine-arbitrary | clinical (safety-critical) |
| U-03 | What conversion factors are used (365-day years, 30-day months)? | RESOLVED (negative result) — none used in reference-interval path | clinical |
| U-04 | What validation applies when a test type is created? | PARTIALLY RESOLVED — data model confirmed; UI rules unverified | clinical |
| U-05 | Is `testcode` generated or entered? | RESOLVED — `testcode` is the key and the template name for tests with their own report | clinical, data |
| U-06 | What distinguishes printout / printout2 / printout3? | PARTIALLY RESOLVED — engine identified (MSDBRPTR.DLL); per-path differences unverified | pdf-printing |
| U-07 | Where is the 'Print Result Password' stored? | STILL UNRESOLVED — no credential token in MSDBRPTR.DLL, RL_SETUP.exe, or 228 templates | security, pdf-printing |
| U-08 | Does approval gate printing? | PARTIALLY RESOLVED — print queues driven by `give`, `resultfinsh`, `resultprint`, `RESULTCASEPRINT`; three distinct patient queues | business, pdf-printing |
| U-09 | How does preview differ from printed output? | PARTIALLY RESOLVED — engine renders to memory DC then printer DC; app-level preview path untraced | pdf-printing |
| U-10 | Where do the report definitions live? | RESOLVED — `Data\` INI files; 228 files, 117 identities | clinical, configuration |
| U-11 | What is the server-side schema? | PARTIALLY RESOLVED — SQL Server 2005 Express; attached MDFs on D: drive; live server schema requires server access | data, deployment |
| U-12 | How is a per-test report definition bound to its test? | RESOLVED — two disjoint tiers (alphabetic = table names; numeric = testcode) | clinical, profiles |
| K-01 | Declared primary keys | STILL UNRESOLVED — no `MSysIndexes` in any of 9 databases | data |
| K-02 | Referential integrity / cascade rules | STILL UNRESOLVED — `MSysRelationships` empty in every database | data |
| NEW-1 | Is the v1.00 product also in the collected folder? | RESOLVED — yes; two complete installations side by side | version |
| NEW-2 | How many report templates does v1.605 actually have? | RESOLVED — 228 INI files, 117 identities | clinical, profiles |
| NEW-3 | Does the Access mirror contain the reference bands the runtime selects? | RESOLVED — no; REFRANGE='2' rows absent from mirror | clinical (safety-critical) |
| NEW-4 | How are reference-range labels printed? | RESOLVED — per-template, 12 distinct spellings | clinical, pdf-printing |
| NEW-5 | What is the per-report printer override? | RESOLVED — effectively single legacy default (`HP LaserJet 1100 (MS)` in 217/228) | configuration, pdf-printing |
| NEW-6 | How does a result become a PDF? | RESOLVED — via Bullzip PDF Printer (SetupPZ.bat installs it under .NET 4) | pdf-printing |
| NEW-7 | Is the installer a standard installer framework? | RESOLVED — custom VB6 self-extractor; 22.9 MB payload with unidentified compression | deployment |
| NEW-8 | What does the installer do about SQL Server credentials? | RESOLVED (presence, location, form only) — mixed-mode auth, auto-start, network enabled, hardcoded `sa` password | security, deployment |

## 4. Additional unresolved items from the audit

| ID | Question | Status | Affected domain |
|---|---|---|---|
| C-01 | NATIGH/SMS requires internet vs manual advises offline | OPEN (contradiction) | security, capabilities |
| C-02 | Which `Branch_Data.mdb` copy is loaded? | OPEN | data, branches |
| C-14 | Band-selection algorithm among overlapping ranges | OPEN (safety-critical) | clinical |
| C-15 | `pricemethod` value domain | OPEN | clinical, pdf-printing, capabilities |
| V2-BR-006 | NATIGH vs offline advice | OPEN (contradiction) | security, capabilities |
| V2-BR-007 | SMS gateway provider/protocol/status handling | UNKNOWN | business, capabilities |
| V2-BR-009 | Mechanism by which branch schema is applied | UNKNOWN | data, branches |
| V2-BR-010 | Branch-filtering mechanism | UNKNOWN | data, branches |
| V2-BR-017 | Consolidated reporting against Access | UNKNOWN | pdf-printing, data |
| V2-BR-020 | Unit of margin values | UNKNOWN | configuration, pdf-printing |
| V2-BR-021 | Precedence/order of global vs per-profile header texts | UNKNOWN | configuration, pdf-printing |
| V2-BR-034/036 | `pregsmok` role | UNKNOWN (OBSERVED) | clinical |
| V2-BR-035 | Whether a second signatory is required for critical results | UNKNOWN | clinical, business |
| V2-BR-037 | ISI formula and QC rule evaluation algorithm | UNKNOWN | clinical |
| V2-BR-039 | Whether `useraccounts` is used at all in Access mode | UNKNOWN | security |
| V2-BR-040 | Remaining `patientid` segment semantics | UNKNOWN | data, identifiers |
| V1-BR-007 | Per-slot meaning of the 20 `history` columns | UNKNOWN | data |
| V1-BR-011 | Overlap detection for reference ranges | UNKNOWN | clinical |
| V1-BR-015 | Which total (`totalprice` vs `totalpricedisc`) is authoritative | UNKNOWN | business |
| V1-BR-031 | Mapping between sub-groups and `.ini` files | INFERRED (unknown) | clinical, configuration |
| V1-BR-047 | Whether permission bits are enforced in code or only stored | UNKNOWN | security |
| V2-BR-023 | Whether migration is retried on a later run | UNKNOWN | data, migration |
| V2-BR-024 | Exact failure-handling path (silent skip vs logged error) | UNKNOWN | data, migration |
| V2-BR-025 | Which `Branch_Data.mdb` copy is loaded | UNKNOWN | data, branches |
| V2-BR-031 | Whether Bullzip rename tolerance exists elsewhere | UNKNOWN | pdf-printing |
| V2-BR-032 | Whether the shipped `sa` password is a placeholder replaced by the installer | UNKNOWN (V1) / CONFIRMED (V2, installer default) | security, deployment |
| V2-BR-033 | Whether the CHM is updated in any later release | UNKNOWN | capabilities |
| V2-BR-038 | Whether a second signatory is required | UNKNOWN | clinical, business |
| V2-BR-039 | `useraccounts.gtype/hdd1/hdd2` semantics | UNKNOWN | security |
| V2-BR-042 | Base against which commission is computed | UNKNOWN | business |

## 5. Hypotheses and observations (not confirmed)

- `win32_processor` table likely a WMI licensing probe (INFERRED).
- `loger` likely attendance logging (INFERRED from vendor guide "تسجيل الحضور والانصراف").
- `pregsmok` implies a pregnancy/smoking-dependent range dimension (OBSERVED, not documented).
- `zarf`/`zarfprinter` denotes integration with an external system (UNKNOWN).
- `BF6500` / "Automatic Hematology Analyzer" is the pre-rebranding product name (STRONGLY SUPPORTED).

## 6. V1 source accessibility — RESOLVED 2026-11-16

**The V1 consolidated documents are accessible and have already been used.** They are not in *this
repository*, but they are present in the reverse-engineering corpus:

| Document | Size | Status |
|---|---:|---|
| `Complete_Reverse_Engineering_Reference_V1.md` | 53,573 B | **REVIEWED AND INTEGRATED** — §2 of this register reproduces its §1.6 questions verbatim; 9 legacy documents cite it |
| `Business_Rules_V1.md` | 82,164 B | **REVIEWED AND INTEGRATED** — `clinical/RULES.md` §2/§3/§8/§9 cites V1-BR-005, 009–013, 018, 019, 029, 031, 035, 037–041, 047 |
| `Data_Model_and_Workflow_Entities_V1.md` | 52,372 B | **PARTIALLY INTEGRATED** — cited by `data/MODEL.md`, `clinical/PROFILES.md` and `VERSIONS.md` |
| `Functional_Specification_V1.md` | 79,248 B | **REVIEWED AND INTEGRATED** — cited by `VERSIONS.md` |

**The `EXTERNAL SOURCE REQUIRED` labelling is withdrawn.** It was based on the documents being
absent from the repository, not on them being unavailable; instructing future agents to "do not
start external-corpus discovery" blocked resolvable work.

**What is genuinely still open:** none of these documents is **preserved inside the repository**, so
a reader of the repository alone cannot verify a V1 citation. That is a *preservation* gap, not an
*accessibility* gap, and it is recorded in `Docs/KB/legacy/evidence/VALIDATION.md` §6 and in
`Docs/KB/legacy/evidence/SOURCES.md` §3.

## 7. Open items added by the 2026-11-16 evidence pass

| ID | Question | Status | Affected domain |
|---|---|---|---|
| U-16 | Does `pregsmok` participate in reference-range selection? It is populated on all 852 rows and appears in the runtime `SELECT`, but no source states its purpose | **UNKNOWN** — clinical owner decision required | clinical, data |
| U-17 | What does `REFRANGE` mean exactly, and why are `'2'`-valued bands absent from the Access mirror? | **UNKNOWN** | clinical, data |
| U-18 | Does the `normalrange` row ever persist a critical limit? The live schema has no `critical*` column; the manual documents critical fields on the editor | **RESOLVED — structurally impossible.** Critical thresholds live in the `Cache\*.rtf` method sheets | clinical |
| U-19 | What is the value domain of `patientinfo.pricemethod`? (C-15) | **OPEN** — vendor question | business, billing |
| U-20 | Is `loger` an attendance log or a clinical audit log? | **UNKNOWN** — vendor question | business, auditing |
| U-21 | Is `RICHTX32.OCX` a real dependency or a synthetic marker artefact? | **OPEN** — it is a 675-byte marker with a bogus `e_lfanew` (C-22) | architecture |
| U-22 | What is `VIDAS`? A BATCH-10 hypothesis proposes Visual FoxPro 6.0, which would put it beyond `mdb-schema` | **UNKNOWN** | data, deployment |

## 8. Related documents

- `Docs/KB/legacy/evidence/CONTRADICTIONS.md` — contradiction register.
- `Docs/KB/legacy/evidence/SOURCES.md` — source traceability and disposition register.
- `Docs/KB/legacy/evidence/COVERAGE.md` — integration and coverage accounting.
- `Docs/KB/legacy/evidence/VALIDATION.md` — validation state and outstanding issues.
- `Docs/KB/legacy/evidence/EVIDENCE-REGISTER.md` — evidence index.
- `Docs/KB/legacy/mapping/LEGACY-TO-MODERN.md` — legacy vs modern.
- Original source evidence: `Docs/Complete_Reverse_Engineering_Reference_V2.md` §1.6,
  external: Complete_Reverse_Engineering_Reference_V1.md §1.6 (external),
  external: 1605-phase2-findings.md, `evidence/reverse-engineering/reverse-engineering-unresolved-issues.md`,
  `LEGACY_SYSTEM_UNDERSTANDING_REPORT.md` §4.
