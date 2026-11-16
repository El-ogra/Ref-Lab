# Knowledge Integration and Coverage Report

**Purpose:** A measurable account of how the 298-document Markdown corpus, the 125 text and log
documents, the 10 SQL schema files and the three ZIP archives have been reconciled into the legacy
knowledge base: what was already represented, what was missing, what was improved, what was
superseded, and what remains unresolved.
**Scope:** the whole legacy knowledge base (`Docs/KB/legacy/**`) as it stands after the
2026-11-16 evidence pass.
**Audience:** auditors, maintainers, and anyone planning further reverse-engineering work.
**Companion documents:** `Docs/KB/legacy/evidence/SOURCES.md` (per-source disposition) · `Docs/KB/legacy/evidence/VALIDATION.md`
(checks) · `Docs/KB/legacy/evidence/CONTRADICTIONS.md` (conflicts) · `Docs/KB/legacy/evidence/UNRESOLVED.md` (open questions).

---

## 1. What was already represented before this pass

The knowledge base that existed at commit `e42bfca0ebad33752c383ba0e17353b78410664b` comprised 21
documents under `Docs/KB/legacy/` plus `LEGACY_SYSTEM_UNDERSTANDING_REPORT.md` at the repository
root. It was **accurate within its scope**: every quantitative claim in the domain documents was
checked against its source and matched exactly — the clinical constants, the 852 reference ranges
and their 13 sub-populations, the 167 unit conversions, the 103 tables / 1,682 columns / 3,809
records, the 210 objects / 8,026 methods / 2,375 SQL-bearing literals, the 268 embedded DDL
statements, the 15 contradictions and the 3 still open.

What it did **not** have was breadth, provenance, or primary schema evidence:

| Gap | Size of the gap before this pass |
|---|---|
| Corpus coverage | 14 of 298 corpus documents cited (4.7 %) |
| Archive provenance | **Every** `external:` citation was a bare filename — no archive path, no hash, no version |
| Primary schema evidence | None in the repository; the schema was known only through prose |
| Executable evidence | None beyond two prose deliverables |
| The official manual | Present in the corpus but never cited — although C-01 is defined *against* it |
| The V1 documents | Labelled `EXTERNAL SOURCE REQUIRED` and treated as inaccessible, although two were already quoted verbatim |
| Migration reconciliation | Internally contradictory (§3 and §4 of `MIGRATION.md` disagreed with each other) |
| Known errors | `180.rtf` mislabelled as sodium rather than chloride; module numbering wrong; "15 CHM topics" instead of 17 |
| Validation | `kb_check.py` failing at the audited commit, unnoticed |

---

## 2. Coverage by knowledge domain

Twelve domains. *Covered* means the domain's material knowledge is present and traceable; *partial*
means it is present but incomplete or with a known defect; *gap* means the evidence exists but is
not represented.

| # | Domain | Before | After | What changed |
|---:|---|---|---|---|
| 1 | System identity and versioning | Covered | **Covered + strengthened** | LCID discriminator added; 268-statement census corrected; the "two versions, no third" conclusion carried into `SPECIFICATION.md` §9 |
| 2 | Architecture (layers, modules, dependencies) | Partial | **Covered** | Module numbering corrected against the vendor guide; the object-delta evidence integrated; `architecture/EXECUTABLE.md` created |
| 3 | Application implementation (objects, methods) | Partial | **Covered** | New canonical document with the 41-added / 0-removed / 88-changed object delta and the five newly-enabled objects |
| 4 | Database architecture | Partial | **Covered** | New canonical `data/DATABASE.md` built from primary DDL; the 103/1,682/3,809 schema, the 96 profile tables, the 0-PK finding, the table→SQL usage profile |
| 5 | Schema migration | Partial + contradictory | **Covered** | §3 corrected to the reconciliation evidence; 12/14/13 corrected to 27/228/13; the 42 absent targets enumerated |
| 6 | Business rules (clinical) | Covered | **Covered + corrected** | `180.rtf` corrected from sodium to chloride; the six-band duplication rule documented; the critical-limit schema conflict established |
| 7 | Workflows and operational behaviour | Partial | **Covered** | Reference-range administration, result entry, backup (password `123`, `Patient_yyyymmdd.bak`), stock-take (F7), start-up sequence and the English (Canada) locale remedy all integrated from the manual |
| 8 | Interfaces and integrations | Partial | **Covered** | The SMS path (plain HTTP via browser automation), the four connection templates and `PersistSecurityInfo`, remote-support tooling |
| 9 | Configuration and runtime | Partial | **Covered** | `BASE.ini` V1/V2 sizes pinned; the 223 `Data\*.ini` templates with the 172/51 split; the 29 method sheets |
| 10 | Security and data protection | Covered | **Covered + extended** | Maintenance password `123`, the `[lastlog]` password logging, and the branch-leaking identifier added |
| 11 | Version evolution and reconciliation | Partial | **Covered** | `VERSIONS.md` extended; the new contradictions registered |
| 12 | Provenance and traceability | **Gap** | **Covered** | `Docs/KB/legacy/evidence/SOURCES.md` created — 10 of 14 citations now resolve to an exact archive path with a hash |

**Domain coverage: 12 of 12 domains now covered** (8 were partial or absent before).

---

## 3. Coverage by evidence class

The confidence of a claim is limited by the strength of its evidence. The corpus is 100 % static
analysis; no runtime behaviour was ever observed. Classifying the knowledge base by the *strongest*
evidence that can support each claim:

| Evidence class | Claim type | Share of the knowledge base's material claims | Example |
|---|---|---|---|
| **VERIFIED** | Directly established by primary evidence — schema, hash, PE structure, official manual text | ~35 % | `normalrange` has 18 columns and no `critical*` column; `DDL` = 27 + 228 + 13 = 268; `180.rtf` is chloride |
| **CORROBORATED** | Two or more independent sources agree | ~30 % | The nine clinical constants (database + manual + KB); V-CD-2019 is the later version (PE version + `NOTEX.dll` + installer date) |
| **INFERRED** | Reasonable conclusion from indirect evidence | ~20 % | That the 88 changed objects indicate a shift from result-entry to patient-administration work |
| **PARTIALLY VERIFIED** | Some elements supported, important details uncertain | ~10 % | The band-selection algorithm; the `REFRANGE` semantics; the SMS request/response contract |
| **UNRESOLVED** | Evidence insufficient or contradictory | ~5 % | `VIDAS` structure; `pricemethod` domain; whether the migration is retried |

**★ The single largest evidence-class limitation is unchanged and cannot be fixed by documentation:
no method body was ever recovered.** 1,268 of 8,026 declared methods were read, none were
decompiled. Every behavioural statement in this knowledge base — including the ones in this pass —
is inferred from declarations, string literals, SQL, schema and the official manual. This is
recorded in `architecture/EXECUTABLE.md` §6 and is the first constraint in
`mapping/TRANSFORMATION.md` §8.

---

## 4. Source integration — the measurable figures

### 4.1 Source population and disposition

| Population | Count | Dispositioned | Integrated | Partially integrated | Better source exists | Superseded / duplicate | Historical | Not technically relevant | Unreadable |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Markdown corpus | 298 | **298 (100 %)** | 128 | 4 | 25 | 120 | 8 | 13 | **0** |
| Text and log documents | 125 | **125 (100 %)** | 86 | — | — | — | — | 39 | **0** |
| SQL schema files | 10 | **10 (100 %)** | 5 | — | 5 | — | — | — | **0** |
| Deliverables archives | 3 | **3 (100 %)** | 3 | — | — | — | — | — | **0** |
| **Total** | **436** | **436 (100 %)** | **222** | **4** | **30** | **120** | **8** | **52** | **0** |

The 125 text and log documents decompose as: **86 integrated** (29 clinical method sheets · 13
batch inventories · 11 VB6 object-model extractions · 8 hash and checksum manifests · 5 DDL evidence
files · 4 CHM cross-references · 3 Ghidra outputs on RealLab binaries · 3 vendor documents · 3 PE and
container analyses · 3 vendor deployment and version records · 3 further analysis files · 1 string
mining) and **39 not technically relevant** (21 Ghidra outputs on third-party redistributables ·
18 tool-execution and session logs, retained as provenance).

**★ Zero sources are left undispositioned.** Every one of the 436 sources carries a final
disposition and, where it is not integrated, a named reason and — where applicable — the preferred
source. The full 298-row corpus register is `Docs/KB/legacy/evidence/SOURCES.md` §5.

### 4.2 Citation resolution

| Measure | Before | After |
|---|---:|---:|
| KB citations to an external corpus source | 14 | 14 (unchanged) |
| …that resolve to an **exact archive path + hash** | **0** | **10 of 14 (71 %)** |
| …that resolve to a corpus-only path with a hash | 0 | **4 of 14** |
| Citations to a **repository-resident** source with a path | 13 | 27 |
| Claims in the legacy KB carrying a section-level locator | most | most |

The remaining four unresolved-by-path citations (`1605-phase2-findings.md`,
`Business_Rules_V1.md`, `Complete_Reverse_Engineering_Reference_V1.md`, and the corpus-only copy of
`architecture.md` that resolves to `analysis/BATCH-01/architecture.md`) are corpus-resident; their
hashes are recorded in `Docs/KB/legacy/evidence/SOURCES.md` §3.

### 4.3 Schema coverage — the strongest single measure

| Object | Known before | Known after |
|---|---|---|
| Tables with a full column list and type | 0 | **103 of 103** |
| Tables with a verified row count | 0 | **103 of 103** |
| Tables with a business-role classification | 0 | **103 of 103** (96 profile / 33 config / 6 special) |
| Tables with a per-table SQL access profile | 0 | **117 of 117** |
| Tables with a V1→V2 column delta | 0 | **103 of 103** |
| Profile tables with an analyte list | 0 | **49 of 49** |
| Embedded DDL statements individually classified | 0 | **268 of 268** |
| Embedded DDL statements reconciled against the shipped DB | ~0 | **268 of 268** |

**Before this pass, the knowledge base described the schema in prose. After it, the schema is
fully enumerated and reconciled.** This is the largest single coverage gain and it is the reason
`data/DATABASE.md` could be created.

### 4.4 Clinical-evidence coverage

| Object | Known before | Known after |
|---|---|---|
| Method sheets (`Cache\*.rtf`) read | 0 | **29 of 29** |
| Method sheets represented in the KB | 6 | **6** (corrected — one was mislabelled) |
| Method sheets available for future work | 0 | **29** |
| Reference-range rows described | aggregate only | aggregate **plus** the six-band duplication rule |
| CHM topics | "15" | **17** (15 capabilities + copyright page) |

---

## 5. What was improved, superseded and withdrawn

### 5.1 Corrections made to existing knowledge

| # | Document | Correction | Evidence |
|---:|---|---|---|
| 1 | `data/MIGRATION.md` §1 | `CREATE TABLE` class count 12 → **27** (14 distinct tables); the breakdown now sums to 268 | `ddl-all.txt` |
| 2 | `data/MIGRATION.md` §3 | "109 of 132 column targets applied", "19 unapplied" (a table listing 23) | **90 of 132 present**, **42 absent**; the list extended to 42 | `migration-proof.json` |
| 3 | `data/MIGRATION.md` §3 | Created-table table rewritten: `MyLabSetting` added, `hba1c`/`pictures`/`ratios`/`widal2` reclassified as ALREADY-EXISTED, `IgEallergy` added to the absent set | `migration-proof.json` |
| 4 | `clinical/RULES.md` §7 | **`180.rtf` is chloride (Cl), not sodium** | `Cache\180.rtf` rendered text |
| 5 | `clinical/RULES.md` §5 | The six-band duplication rule for range administration | `help-manual-ar.md` §10 |
| 6 | `clinical/RULES.md` §7 | V1-BR-035 upgraded from a documentation claim to a **schema-level VERIFIED** fact | `Patients_backup` DDL vs live DDL |
| 7 | `business/WORKFLOWS.md` §6 | "14 range attributes, all present in `normalrange` schema" | Corrected: all present in the **manual**, five **not** in the live schema |
| 8 | `architecture/OVERVIEW.md` §6 | Module numbering off by one module | Corrected against the vendor guide's own TOC |
| 9 | `business/CAPABILITIES.md` §1, §2 | "15 CHM topics"; "510 headings" | **17 topics / 15 capabilities**; the "510 headings" figure **withdrawn** as unreproducible |
| 10 | `VERSIONS.md` §4, §5 | DDL breakdown; `Data\*.ini` split 173/50; PE timestamp not marked as non-discriminating | All corrected; LCID and header counters added |
| 11 | `Docs/KB/legacy/evidence/EVIDENCE-REGISTER.md` | Listed **five Ghidra output files that do not exist**; "5 Python scripts"; two truncated digests presented as SHA-256; the V1 documents labelled inaccessible | All corrected |
| 12 | `Docs/KB/legacy/evidence/UNRESOLVED.md` §6 | Standing instruction to label every V1 reference `EXTERNAL SOURCE REQUIRED` and not to search | **Withdrawn** — the documents are accessible and already integrated |
| 13 | `Docs/KB/legacy/evidence/CONTRADICTIONS.md` | 15 contradictions; two known source conflicts unregistered | Now **22** (C-16 … C-22 added) |
| 14 | `business/SECURITY.md` | Maintenance password, clear-text SMS transport and the identifier leak unrecorded | V-18, V-19, V-20 added |

### 5.2 Superseded knowledge

| Superseded | By | Reason |
|---|---|---|
| "V1 consolidated documents are inaccessible" | The corpus itself | All four are present at the quoted sizes; two are already quoted verbatim |
| "12 CREATE TABLE" | 27 statements → 14 distinct tables | Direct statement census |
| "109 of 132 columns applied" | 90 of 132 | Direct reconciliation |
| "15 CHM topics" | 17 topics / 15 capabilities | Direct CHM enumeration |
| "15 topics in the CHM, byte-identical" as a *capability* count | Chapter-level topic count | Confused a file count with a capability count |
| "510 headings" in the vendor guide | 56 numbered markers / 47 distinct numbers | The figure is not reproducible from the extracted PDF text |
| The `INFRASTRUCTURE.md` → three-legacy-documents integration claim | *(withdrawn)* | `INFRASTRUCTURE.md` is an inventory of the **modern** .NET codebase and is cited nowhere |

### 5.3 Historical material preserved

Nothing was deleted. The superseded versions remain available in git history at
`e42bfca0ebad33752c383ba0e17353b78410664b`, and the 120 superseded / duplicate corpus documents
plus the 8 historical references are registered in `Docs/KB/legacy/evidence/SOURCES.md` §5 with their disposition.
The `Patients_backup.mdb` ancestor schema — the only evidence of the critical-range columns — is
registered as REVIEWED AND INTEGRATED precisely because it is a *historical* artefact with unique
technical value.

---

## 6. What remains unresolved after this pass

| Area | Open items | Register |
|---|---:|---|
| Band-selection algorithm | 1 (safety-critical) | `Docs/KB/legacy/evidence/UNRESOLVED.md` §3, C-14 |
| `pricemethod` value domain | 1 | C-15 |
| `VIDAS` structure | 1 | `Docs/KB/legacy/evidence/UNRESOLVED.md` §6 |
| Migration retry and failure-handling behaviour | 2 | V2-BR-023, V2-BR-024 |
| `RICHTX32.OCX` attribution | 1 | C-22 |
| `pregsmok` and `REFRANGE` semantics | 2 | U-16, U-17 |
| `loger` role | 1 | U-20 |
| Method bodies / runtime behaviour | 1 class, unbounded | `architecture/EXECUTABLE.md` §6 |
| Runtime execution of *either* version | — | never observed; the corpus is 100 % static |
| 709 of 837 files version-attributed by path rule only | 84.7 % | `VERSIONS.md` §9 |

---

## 7. Coverage honesty statement

> **File-count coverage is not knowledge coverage.** 436 sources are dispositioned, but the
> knowledge base was never a 436-document deliverable and must not become one. The metric that
> matters is the one in §4.3: **103 of 103 tables enumerated, 268 of 268 migration statements
> classified and reconciled, 117 of 117 tables access-profiled, 29 of 29 method sheets read.** Those
> are the numbers that determine whether a future engineer can trust the knowledge base.
>
> Conversely, the knowledge base is **not** complete: the behaviour of 8,026 declared methods is
> unknown, no screen has been observed running, and 42 of the migration's 132 column targets are
> permanently absent from the shipped database — a gap no amount of documentation can close.

---

## 8. Related documents

- `Docs/KB/legacy/evidence/SOURCES.md` — the per-source disposition register.
- `Docs/KB/legacy/evidence/VALIDATION.md` — the checks performed and their results.
- `Docs/KB/legacy/evidence/CONTRADICTIONS.md` — 22 conflicts with resolutions.
- `Docs/KB/legacy/evidence/UNRESOLVED.md` — open questions.
- `Docs/KB/legacy/SPECIFICATION.md` — the canonical reference-system specification.
- `Docs/KB/legacy/INDEX.md` — navigation.
