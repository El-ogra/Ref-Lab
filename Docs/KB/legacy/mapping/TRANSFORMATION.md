# Engineering Transformation Blueprint

**Purpose:** A dependency-aware technical plan for evolving the modern `Ref Lab` project so that it
reproduces the behaviour and capabilities of the latest identifiable reference-system version
(**V-CD-2019 / v1.605**) — and, where the legacy behaviour was defective, deliberately improves on it.
**Scope:** Architecture, components, database, business rules, workflows, integrations,
configuration, security, testing and migration.
**Audience:** the engineering team that will execute the transformation.
**Status:** **planning only. Nothing in this document has been implemented.**
**Companion:** `mapping/GAP-ANALYSIS.md` (the assessment this plan answers) · `SPECIFICATION.md`
(the target) · `data/DATABASE.md` (the target schema) · `Docs/KB/legacy/evidence/UNRESOLVED.md` (the unknowns).

---

## 1. What "reproduce the reference system" actually means here

RealLab is **not** a legacy system to be ported. Between V-ROOT-1.00 and v1.605 the vendor changed
**only** the data and workflow layer: 43/43 ActiveX components, 42/42 training artefacts and the
entire 17-topic help CHM are **byte-identical** between the two releases, while 174 files changed in
`Data\` and `bin\`. The executable was never recompiled after 2013-12-27.

Therefore the target is **not** "the same VB6 program". The target is:

> **A laboratory information system that produces the same observable clinical, commercial and
> operational outcomes as RealLab v1.605 — with the legacy's defects fixed and its undocumented
> behaviour made explicit.**

Three categories of work follow, and they must not be confused:

| Category | Definition | Examples |
|---|---|---|
| **Parity** | Reproduce a legacy behaviour that is confirmed and still wanted | Reference-range classification, the result lifecycle, report categories, stock-take, send-out |
| **Repair** | Reproduce the *intent* of a legacy behaviour that was broken or unreachable in the shipped product | Critical-value escalation (absent), NATIGH (code-present, schema-absent), the SMS gateway (plain HTTP), the partial schema migration |
| **Retire** | Deliberately not reproduce | Multi-branch, `loger`, the generic `history` table, `Paste Errors`, remote-support tooling |

The modern project has already made most of the scope decisions. This blueprint records what
remains.

---

## 2. Principles that constrain the work

These are the project's existing, test-guarded conventions. They are **not** negotiable for this
programme, and every step below complies with them.

| # | Principle | Enforcement |
|---|---|---|
| P-1 | **Migrations are never edited after application, and never auto-applied.** A schema change means a *new* migration via `dotnet ef migrations add` | `AGENTS.md` rule 4; guard `The_composition_root_never_resolves_the_database_initialiser`; `Docs/KB/CONVENTIONS.md` §3 |
| P-2 | Applying the schema to the laboratory database is the **owner's** decision, made from the command line | `Docs/KB/CONVENTIONS.md` §3 |
| P-3 | The `Domain` layer carries **no package references** | guard `Domain_references_only_the_base_class_library` |
| P-4 | Every Arabic user-interface string must appear in `Docs/ui-labels-verified.md` | guard `B9_every_arabic_string_is_authorised_by_the_verified_label_source` |
| P-5 | Curated documents stay inside their character budgets | `scripts/kb_check.py` V8 |
| P-6 | The knowledge base may never lie about the source tree | `scripts/kb_check.py` V1, V2, V9, V11 |
| P-7 | The laboratory currency is EGP | guard `The_laboratory_currency_is_egp` |

---

## 3. Mandatory work — prioritised by dependency and risk

### M-0 — Close the three data-model gaps before any data migration

**Why first.** `pregsmok`, `REFRANGE` and the method-sheet critical values are all *inputs to
reference-range selection*. If the modern schema cannot express them, every migrated range row is
silently incomplete, and no later step can detect the loss.

| Step | Change | Notes |
|---|---|---|
| M-0.1 | **Decide `pregsmok`.** It is populated on **all 852** legacy range rows and the legacy `SELECT` includes it, but no source states its use | If it is a clinical dimension, add `PregnancySmokingFactor` to `ReferenceRange` and to the selector. If it is a data-entry artefact, record the decision in `Docs/KB/legacy/evidence/UNRESOLVED.md` and in the domain XML-doc. **Do not silently drop it.** |
| M-0.2 | **Decide `REFRANGE`.** The column marks bands the runtime selects that the Access mirror does not contain (C-05) | Either model it (`IsRuntimeBand`) or document why the modern deterministic selector makes it redundant |
| M-0.3 | **Seed the clinical constants and the critical values.** `constanttable` (2 groups × 7 slots) and the 29 `Cache\*.rtf` method sheets must become seed data | The nine constants are already implemented as `ClinicalConstant`; the **per-analyte critical values from the method sheets are not** |

**Exit criteria.** Every one of the 852 legacy range rows can be represented, and every one of the
29 method sheets has contributed its critical values to the seed data. **Owner decision required**
for M-0.1 and M-0.2.

### M-1 — Reconcile the 22 unreferenced `V2-REQ-*` identifiers

**Why.** 50 of 72 legacy requirements are referenced by the 73 modern use cases. The other 22 are
either out of scope or unimplemented, and nobody currently knows which.

| Step | Action |
|---|---|
| M-1.1 | List the 22 unreferenced identifiers against the V2 requirements matrix's own status column |
| M-1.2 | Classify each as **OUT OF SCOPE** (with the reason), **IMPLEMENTED UNDER ANOTHER ID**, or **NOT IMPLEMENTED** |
| M-1.3 | Write the classification into `mapping/GAP-ANALYSIS.md` §6 and into the knowledge base's requirement traceability |

**Exit criteria.** Zero unreferenced `V2-REQ-*` identifiers without a documented disposition.

### M-2 — Build the profile-to-normalised-result migration

**Why.** The 96 profile tables are the largest structural change (`GAP-ANALYSIS.md` §4).

| Step | Action | Dependency |
|---|---|---|
| M-2.1 | Publish the **analyte map**: the 49-row `branch-profile-schemas.csv` gives each profile table's analyte columns; extend it to all 96 profile tables from `cumulative-schema.csv` and the DDL | — |
| M-2.2 | Define the pivot: `(patientid, requestdate, profile, analyte, value)` → `OrderTest` + result rows + `ResultValue` | M-2.1 |
| M-2.3 | Write the pivot as a **new EF migration** (data migration, applied by the owner) | P-1, P-2 |
| M-2.4 | Reconcile counts after the pivot: 3,809 legacy rows → modern order/result rows, with a variance report | M-2.3 |

**Risks.** The profile tables are keyed by `(patientid, requestdate)` with **no test code**; the
modern model is keyed by `OrderTest`. The join must be reconstructed from `patienttest` — which is
itself the 46-column order table. **This is the highest-risk step in the programme** and needs a
reversible, restartable script with a dry-run mode and a row-count reconciliation gate.

### M-3 — Rebuild the report layer on the normalised model

| Step | Action |
|---|---|
| M-3.1 | Catalogue the **223** `Data\*.ini` templates: which template, which profile, which fields, which layout |
| M-3.2 | Derive the modern equivalent: a QuestPDF document per report family, with the legacy INI's field selection, margins, header/footer and colour rules |
| M-3.3 | Preserve the four filing categories (`ReportCategory` already exists) |
| M-3.4 | Preserve the legacy's **critical-range printing toggle** — *"print critical ranges"* in the report settings — and the background shading of out-of-range results |

**Risk.** 172 of the 223 templates changed between V1 and v1.605, so the template set is a *living*
artefact of the vendor's configuration practice, not a stable specification. The catalogue must be
built from the **v1.605** templates only.

### M-4 — Add the missing operational capabilities

| # | Capability | Legacy evidence | Priority |
|---:|---|---|---|
| M-4.1 | **Reference-range administration** | The manual's editor, six-band rule and commands | High — a safety-critical screen with no modern equivalent |
| M-4.2 | **Database maintenance / backup** | The 8-part screen, `Patient_yyyymmdd.bak`, restore-by-work-code | High — the modern system has no backup path |
| M-4.3 | **Stock-take reconciliation** | `«الجرد وحساب الدرج»`, F7 | Medium |
| M-4.4 | **Critical-result escalation** | **Not a legacy capability** — a repair | Medium — needs clinical governance, not just code |

M-4.4 is **not parity work**. It must be specified with the laboratory, and it must not be
silently inferred from the legacy.

### M-5 — Resolve the three remaining unknowns that block design

| Unknown | Blocks | How to resolve |
|---|---|---|
| `pricemethod` value domain (C-15) | The modern `Payment`/`PriceList` model | Ask the vendor, or recover an unredacted `patientinfo` sample |
| `loger` table role | Whether `AttendanceEntry` is correctly modelled | Vendor confirmation; the table is otherwise unattributable |
| Legacy runtime behaviour | Every behavioural claim in the knowledge base | **Not resolvable from the corpus.** Only a running system would settle it |

**★ The third cannot be closed.** No amount of further documentation will produce the legacy's
runtime behaviour. Every plan that assumes otherwise will be wrong somewhere.

---

## 4. Optional and lower-priority work

| # | Item | Rationale |
|---:|---|---|
| O-1 | **SMS delivery** | Removed from scope. If reinstated, build it as an HTTPS client with a secret store — **not** as the legacy's `InternetExplorer.Application` + plain-HTTP-query-string pattern |
| O-2 | **Multi-branch** | Removed from scope; the identifier position is already repurposed. Reinstatement is a schema + identifier change, not a feature |
| O-3 | **NATIGH.COM delivery** | The legacy feature was code-present and schema-absent; it never worked against the shipped database. Rebuild only if the vendor confirms the contract |
| O-4 | **Database-engine portability** | The modern system targets SQL Server only. The legacy also ran on Access and MySQL. Supporting Access would mean abandoning EF Core's provider model |
| O-5 | **In-process query optimisation** | 43 % of legacy SQL traffic hit `patientinfo` + `patienttest`. The modern normalised model will need index work, but it should follow the data, not the legacy |

---

## 5. Data migration and integrity risks

| # | Risk | Severity | Mitigation |
|---:|---|---|---|
| R-1 | **Profile-table pivot loses or mis-attributes analytes.** 96 tables × up to 84 columns, keyed only by `(patientid, requestdate)` | **Critical** | Analyte map from the DDL and `cumulative-schema.csv`; dry-run; row-count reconciliation; restartable; owner-approved cutover |
| R-2 | **Duplicate rows across profiles.** The same analyte may be reachable through more than one profile table | High | Deduplicate on `(patientid, requestdate, analyte)`; report every collision rather than dropping silently |
| R-3 | **The 42 never-applied migration columns.** `BloodGases` (8), `cultures.Few5..8` (4), `prostaticx.crystalp*` (4), `urine.T56/T57/testid` (3), `patienttest.NatighU`, `patientinfo.gehaname`, `pregnancy.normalrange3`, `lipids.nonhdl`, `Proelectrophoresis.Beta2/immunolgo`, `Kidney.egfr`, `Helicobacter.hpab/hpbiop/hpiga`, `useraccounts.Gtype/HDD1/HDD2`, `IgEallergy.Item12/24/36`, and the seven SQL-Server-only `Patients` columns | High | These features were **never available** in the shipped product. Do **not** invent them. Record each as "not present in the shipped reference system" |
| R-4 | **`Patients_backup.mdb` is a different schema generation** (22-column `normalrange` with `critical*`) | Medium | Never migrate from the backup. It is evidence, not data |
| R-5 | **Critical values live in RTF text, not in the database.** Parsing 29 method sheets is required | High | Parse once, review the output manually, seed as reviewed data |
| R-6 | **No primary keys in the source.** Every legacy row identity must be reconstructed | Medium | Use the natural keys documented in `data/DATABASE.md` §3 and `data/IDENTIFIERS.md`; where no key exists, assign a surrogate and record the rule |
| R-7 | **The `patientid` branch digit has been repurposed** in the modern system | Medium | Map explicitly; never copy the 13-character legacy id verbatim |
| R-8 | **Pseudonymised identifiers.** The shipped exports contain `<PID-nnnn>` substitutions and an 18-entry `pseudonym_map.txt` | Low for production data; **high for the evidence exports** | The pseudonym map must never be applied to production data; it exists only to de-identify the archived evidence |

---

## 6. Compatibility considerations

| Consideration | Position |
|---|---|
| **Legacy data format** | No legacy file format is consumed by the modern system. There is no import path and none should be built for the `.mdb` files — the pivot in M-2 is the only sanctioned route |
| **Report output** | The legacy's PDF file-naming and filing-directory convention (`D:\PDF\<Category>\`) is **not** reproduced. `ReportCategory` carries the taxonomy; the modern system owns the storage layout |
| **Arabic labels** | P-4 applies. Any label taken from the legacy manual or the CHM must be added to `Docs/ui-labels-verified.md` first. The manual's 17 topics are the source of many such strings |
| **Identifiers** | The modern identifier is not the legacy `patientid`. Do not present them as interchangeable |
| **Method names** | The legacy's per-analyte names (`testsimp`, `testabrev`, `testnamear`) have three distinct purposes in one schema; the modern model must keep them distinct |
| **Database engines** | SQL Server only. The legacy's Access and MySQL support is retired, not ported |

---

## 7. Testing and validation requirements

| Layer | Requirement |
|---|---|
| **Domain** | Every rule in `Docs/KB/DOMAIN-RULES.md` needs a test that states its **legacy evidence** in the test name. The existing convention (`A_work_start_is_recorded_on_the_operator_and_in_the_log`) is the model |
| **Application** | Each use case keeps its validator, its permission check and its feature-file link — enforced by `kb_check.py` V5 |
| **Architecture** | The five guards listed in P-3 … P-7 must keep passing; `kb_check.py` V10 verifies they still exist |
| **Knowledge base** | `python scripts/kb_check.py` must exit 0 before any commit. A non-zero exit is a blocker, not a warning |
| **Data** | Every migration needs a **row-count reconciliation** against the source and a **restartable** forward path; no destructive down-migrations against laboratory data |
| **Legacy parity** | For each rule classified ALREADY ALIGNED in the gap analysis, a test that asserts the modern behaviour **and cites the legacy evidence** — the pattern already used in `ReferenceRange.cs` and `ReferenceRangeSelector.cs` |
| **Coverage** | `kb_check.py` V5 requires every source file and every use case to be claimed by exactly one feature file. New files must be claimed in the same commit that adds them |

---

## 8. Technical risks and unresolved decisions

| # | Risk or decision | Owner | Impact if wrong |
|---:|---|---|---|
| TR-1 | The **legacy's runtime behaviour is unknown**. Every behavioural statement is inferred | Laboratory / vendor | Silent behavioural divergence that no test can catch |
| TR-2 | `pregsmok` — clinical dimension or artefact? | Clinical owner | Range selection wrong for every patient |
| TR-3 | `REFRANGE` — is it still needed under a deterministic selector? | Architecture | Lost or spurious range bands |
| TR-4 | `pricemethod` — value domain unknown | Vendor | Pricing and commission model wrong |
| TR-5 | `loger` — attendance or audit? | Vendor | `AttendanceEntry` mis-modelled |
| TR-6 | The legacy `NOTEX.dll`/`NOTEXS.dll` family is undocumented | — | Non-clinical; low impact |
| TR-7 | `VIDAS` structure unknown | Vendor | The SQL Server catalogue may have hidden objects |
| TR-8 | Whether the legacy migration is retried on a later run (V2-BR-023) and how it fails (V2-BR-024) | — | Affects how a modern installation must handle schema drift |
| TR-9 | Whether the modern `ReferenceRangeSelector` tie-break order matches the laboratory's expectation | Clinical owner | Deterministic but *wrong* selection |
| TR-10 | 22 unreferenced `V2-REQ-*` identifiers | Product owner | Silent scope gap |

---

## 9. Suggested execution order

```
M-0  data-model gaps closed ──┬── M-1  requirement triage ──┐
   (owner decisions)          │                             │
                              └── M-2  profile pivot ────────┼── M-3  report layer
                                  (highest risk)             │
                                                               └── M-4  missing capabilities
                                                                    │
M-5 unknowns ───────────────────────────────────────────────────────┘
   (vendor questions, run in parallel from day 1)
```

**Critical path:** M-0 → M-2 → M-3. M-2 cannot start before M-0 is closed, because the analyte map
and the range model must both be settled before any row can be pivoted.

**Start immediately, in parallel, because it has the longest lead time:** M-5. Every answer that
arrives from the vendor removes a row from the unresolved register and unblocks a design decision.
**None of them can be answered from the corpus** — that is the point of asking.

---

## 10. What this blueprint does not do

- It does **not** implement anything. Every item above is a plan.
- It does **not** re-derive the legacy's behaviour. That work was completed by the reverse
  engineering and is captured in `SPECIFICATION.md` and `data/DATABASE.md`.
- It does **not** assume the legacy's defects should be reproduced. The four **Repair** items —
  escalation, NATIGH, the SMS transport, the schema migration — are deliberately *not* ported as-is.
- It does **not** propose a new architecture. The modern solution's layering, its migration policy
  and its validation gate are sound and are preserved.

---

## 11. Related documents

- `mapping/GAP-ANALYSIS.md` — the assessment.
- `mapping/LEGACY-TO-MODERN.md` — the earlier feature-level comparison.
- `SPECIFICATION.md` — the target system.
- `data/DATABASE.md` — the target schema and the migration engine.
- `clinical/RULES.md` — the clinical rules.
- `Docs/KB/legacy/evidence/UNRESOLVED.md` — the open questions.
- `Docs/KB/legacy/evidence/VALIDATION.md` — the validation state.
