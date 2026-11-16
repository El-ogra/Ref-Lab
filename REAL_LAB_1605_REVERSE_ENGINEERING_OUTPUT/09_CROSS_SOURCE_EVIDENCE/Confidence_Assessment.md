# Confidence Assessment — Real Lab System v1.605 (V-CD-2019)

## 1. Methodology

**Evidence categories** (same codes as `Evidence_Catalog.csv`):
A = executable/object dump; B = database dump (SQL/literal/CSV/schema); C = prior RE output;
D = configuration (INI/JSON/corpus); E = runtime artefact (RTF/CHM/Flash); F = tooling audit.

**Labels applied:**

| Label | Meaning |
|---|---|
| **VERIFIED** | Two or more independent sources agree; no primary dump contradicts. |
| **STRONGLY_SUPPORTED** | One primary dump plus at least one consistent prior report. |
| **INFERRED** | Reasonable reconstruction from fields/predicates; control flow not observed. |
| **UNVERIFIED** | Asserted in prior reports but not found in the primary dumps consulted. |
| **CONFLICTED** | Sources disagree irreconcilably; both sides preserved in `Conflicting_Findings.md`. |
| **UNKNOWN** | Evidence insufficient; no claim made. |

Prior registers also use the term **CONFIRMED**; where this deliverable re-identifies the
underlying fact in a primary dump it is written **VERIFIED**, and where it rests on a prior
report alone it is written **STRONGLY_SUPPORTED**. No prior label is silently upgraded.

**Rules used throughout this deliverable:**
1. No behaviour claim is upgraded to VERIFIED merely because a prior report calls it
   "VERIFIED" — the underlying evidence must be re-identifiable in a primary dump.
2. A field's existence is not evidence of its business meaning. `grantN`, `REFRANGE`,
   `moreinfo`, `selector` and `payment.type` exist; their semantics are separately labelled.
3. Every count carries its denominator and source scope (e.g. "1 of 609 tests", "852 ranges
   in the exported knowledge layer").
4. Static-only constraint: nothing was executed, so no runtime observation exists.
5. The prior claim of "~80–85% recovered knowledge" is **not adopted**: it has no stated
   denominator or definition. It is replaced by the per-domain table below.

---

## 2. Per-domain assessment

| Domain | Overall label | CONFIRMED / VERIFIED core | INFERRED / UNKNOWN remainder |
|---|---|---|---|
| Patient management | **STRONGLY_SUPPORTED** | 39 `patientinfo` columns in `v2019-Patients.mdb.sql`; 13-char `Text(13)` ID; branch extraction from positions 2+9; search predicates; `history` 20-column structure | Other ID positions UNKNOWN; automated duplicate detection UNKNOWN (no matching predicate in 1,797 SQL); `selector`/`moreinfo` semantics UNKNOWN |
| Visits & orders | **STRONGLY_SUPPORTED** | 46-column `patienttest`; `(patientid, requestdate)` join; `give`/`taslim*`; two DELETE statements; send-out fields and `testpriceout−testpriceoutmadfo` formula | No INSERT/UPDATE in the SQL corpus → creation and status writes INFERRED; cancellation as physical deletion INFERRED; no void flag CONFIRMED |
| Simple vs composite tests | **CONFLICTED / PARTLY INFERRED** | 609 tests / 38 columns in the primary schema dump (37 in the prior export); 49 profile tables in a prior inventory with 2–81 analytes; `RESULTT1..4`; 36 distinct `joined_table` values in the INI corpus; two template tiers | No "is composite" flag; simple-code ↔ profile-column mapping UNKNOWN; per-component ranges UNKNOWN; "Creatinine/Urea/Uric Acid are simple" only partly supported (they are also `kidney` columns) |
| Results management | **STRONGLY_SUPPORTED** | `resultfinsh`, `resultreview`, `resultprint`, `give` + audit columns; four exact queue predicates; range selection logic from disassembly; critical values authored in RTFs | No `approved`/`released`/`recheck` column; `printnum` as reprint count INFERRED; critical flag storage UNKNOWN; write paths invisible |
| Laboratory business rules | **PARTLY VERIFIED, PARTLY INFERRED** | 852 ranges/304 tests with sex/age/unit distributions; 167 conversions; PTCONS/CBCCONS constants; microbiology joins and anti-biogram exclusion; named subgroups | Conversion arithmetic direction INFERRED; CBC formulas STRONGLY_SUPPORTED only; INR equation INFERRED; `pregsmok` use OBSERVED but not traced; authoritative range rows depend on unresolved C-03/C-07 |
| Financial management | **STRONGLY_SUPPORTED** | All money columns; net/paid/balance formulas; `ROUND(...,2)` in SQL Server; payment sign convention 0/2 positive, 1/3 negative; commission `omola×amount/100`; `wareed−saader` | Access-dialect rounding UNKNOWN; type *names* INFERRED; `Khasmlimit` and `moneypaid` UNVERIFIED in primary dumps; tax/invoice numbering UNKNOWN |
| Printing & reporting | **VERIFIED** | 66 DataReports; `MSDBRPTR.DLL` engine; `printerselect` + six printer roles; hard-coded Bullzip 11.1 name; four PDF dirs; 228 INI/117 identities; 217/228 printer override; 29 RTF sheets | App-level preview path UNVERIFIED; the three application print paths' differences UNVERIFIED; flash animations explicitly NOT report components |
| Security & permissions | **STRONGLY_SUPPORTED schema, UNKNOWN semantics** | `patients` operator table; `grant13..17`, `SecWord`, `branche`; `useraccounts.HDD1/HDD2/Gtype`; `loger` fields + date purge; admin literal; no client-side print password | `grant1..17` meanings UNKNOWN (UNR-002); `patientword` hashing UNVERIFIED (UNR-005); config credentials STRONGLY_SUPPORTED from prior file-level reports; file/folder ACLs UNKNOWN |

---

## 3. Confidence by evidence category (denominator: 60 catalogued items)

The `category` column of `Evidence_Catalog.csv` uses descriptive names; they group into
the following six families:

| Category family | Items | Role in this reconstruction |
|---|---:|---|
| A — executable / object / PE / dependency | 11 | Object counts, version resources, printer roles, install payload |
| B — primary database / SQL / schema / knowledge | 20 | 1,797 SQL literals, schemas, counts, formulas — the highest-value source |
| C — prior RE output / registry / handoff | 18 | Workflow interpretation, disassembly results, contradiction register |
| D — template / catalogue configuration | 2 | 228-INI corpus and per-test template index |
| E — runtime / documentation artefacts | 5 | RTF/CHM content, verified labels, transport utility |
| F — installer payloads | 4 | RL_SETUP and third-party installers; provenance only |

Counts are reproducible by grouping the `category` column of `Evidence_Catalog.csv`.
The catalog carries 28 `VERIFIED` and 32 `STRONGLY_SUPPORTED` rows; it deliberately
contains no bare-percentage confidence figure.

---

## 4. What is *not* established (explicit gaps)

1. **No runtime database.** The live SQL Server catalogues (`patients`, `Interface`) and the
   `VIDAS` database were never collected; all range/pricing data is from exports or mirrors
   (Conflicting Findings C-03, C-05, C-07).
2. **No write-path SQL.** 1,797 statements contain zero `INSERT` and zero `UPDATE`; all state
   transitions are reconstructed from read predicates and schema (Chains C2, C9).
3. **No statement-to-caller mapping.** The VA of a SQL literal identifies the string, not the
   form/procedure that executes it (prior UNR-008).
4. **No per-object method names.** Only method counts were recovered (prior §2.2 item 5).
5. **Version timing.** Build timestamp is CONFLICTED (C-01).
6. **Unresolved prior questions:** UNR-002 (`grantN` meanings), UNR-003 (`IDPassword*`),
   UNR-004 (two uppercase literals), UNR-005 (`accuntpw` hashing), plus RL-UNC-001…005.

---

## 5. Correctly qualified summary

- **VERIFIED domains:** 1 of 8 (Printing & Reporting).
- **STRONGLY_SUPPORTED domains:** 5 of 8 (Patient Management, Visits & Orders, Results
  Management, Financial Management, and — schema-only — Security & Permissions).
- **PARTLY VERIFIED / INFERRED domains:** 1 of 8 (Laboratory Business Rules).
- **CONFLICTED / PARTLY INFERRED domains:** 1 of 8 (Simple vs Composite Tests).
- These are labels over *domains*, not a percentage of facts. Any completeness figure would
  require a defined fact inventory and denominator, which the prior work does not supply.
