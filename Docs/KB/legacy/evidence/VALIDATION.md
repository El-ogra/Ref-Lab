# Validation and Outstanding Issues

**Purpose:** A record of the checks actually performed on the legacy knowledge base, their results,
the pre-existing failures that remain, the corrections made in response, and everything that is
still open.
**Scope:** the legacy knowledge base (`Docs/KB/legacy/**`), the validation script
(`scripts/kb_check.py`) and the 2026-11-16 evidence integration.
**Audience:** auditors, maintainers, and the engineer who picks this work up next.
**Status:** the knowledge base passes its own validation gate with one **pre-existing** failure
recorded and explained in §3.

---

## 1. Checks performed

| # | Check | Tool | Result |
|---:|---|---|---|
| 1 | Knowledge-base structural gate | `python3 scripts/kb_check.py` | **8 of 9 reporting checks pass; 1 pre-existing failure (V3)** — see §2, §3 |
| 2 | Archive integrity of all four supplied archives | `unzip -t` | **PASS** — "No errors detected in compressed data" for all four |
| 3 | Archive entry inventory | `unzip -l`, `os.listdir` | **PASS** — 298 + 135 + 544 + 644 + 550 entries enumerated |
| 4 | Every SQL schema file parsed | custom parser over the `mdbtools` dumps | **PASS** — 103 tables / 1,682 columns parsed from the canonical V2 DDL |
| 5 | Embedded DDL census | custom parser over `ddl-all.txt` | **PASS** — 268 statement blocks parsed; 27 + 228 + 13 = 268 |
| 6 | DDL↔database reconciliation | `analysis/BATCH-07/migration-proof.json` | **PASS** — 27 CREATE rows and 228 ALTER rows reconciled against the shipped DDL |
| 7 | Every corpus document hashed | `sha256sum` | **PASS** — 298 digests; 0 duplicate contents |
| 8 | Citation→archive resolution | SHA-256 index of all new-material files | **PASS** — 10 of 14 legacy `external:` citations resolve to an exact archive path |
| 9 | Every repository path named in the KB exists | `kb_check.py` V1 + V11 | **PASS** after the path corrections in §4.2 |
| 10 | Every markdown link resolves | `kb_check.py` V6 | **PASS** |
| 11 | Document character budgets | `kb_check.py` V8 | **PASS** — all curated documents within budget, no hand-edited generated blocks |
| 12 | Numeric claims in prose against the generated indexes | `kb_check.py` V9 | **PASS** |
| 13 | Convention guards still exist | `kb_check.py` V10 | **PASS** — all five cited guards found |
| 14 | Source-file and use-case coverage | `kb_check.py` V5 | **PASS** — 73 use cases, each claimed by exactly one feature file |
| 15 | Generated-index freshness | `kb_check.py` V3 | **FAIL — pre-existing.** See §3 |
| 16 | Modern-side symbol existence | `kb_check.py` V2 | **PASS with 26 warnings** — all for legacy identifiers, which are expected |
| 17 | Archive filename integrity | custom byte analysis | **DEFECT FOUND** — 47 corpus filenames carry unrecoverable byte sequences (§6.1) |
| 18 | Dangling references inside the supplied archives | cross-reference scan | **DEFECT FOUND** — B01-B11 cites four paths it does not contain (§6.2) |
| 19 | Generation completeness of the deliverables archives | full file-level diff | **DEFECT FOUND** — B01-B11 is not a superset of B01-B10 (§6.3) |
| 20 | Legacy knowledge claims re-verified against primary sources | 1,797 SQL statements, 852 range rows, 103 tables, 210 objects, 29 method sheets, 17 CHM topics | **PASS** — every checked quantity matched, and **five** previously recorded figures were found to be wrong (§4.1) |
| 21 | Source disposition completeness | disposition register over 436 sources | **PASS** — 436 of 436 dispositioned, **0** left undispositioned, **0** unreadable |

---

## 2. The gate result

Command: `python3 scripts/kb_check.py` — run from the repository root.

```
Checking the Ref Lab knowledge base...

  V4  use-case drift    OK  73 use cases (48 commands, 25 queries, 14 areas)
  V5  coverage          OK  73 use cases, each in exactly one feature file
  V6  broken links      OK
  V7  duplicate content OK
  V8  budgets           OK  all sizes within budget, no hand-edited blocks
  V9  numeric claims    OK
  V10 convention guards OK  every cited guard test still exists
  V11 legacy evidence paths OK  (211 local refs checked)

26 warning(s):
  ! [V2] Docs/KB/legacy/SPECIFICATION.md:410 mentions `Helicobacter`, which does not exist anywhere in src/
  … (25 further V2 warnings of the same kind)

1 FAILURE(S):
  x [V3] the generated index is stale.
        recorded: fcf01a5c08806c9dab84f2f3ba1a003d4c4811aada9c7025d6179eeb8007f6a7
        actual  : 89f5010de2537ff00a9c37db4b9b5faaf0f45811ffa181ca9fdc148559493c62
        fix     : python scripts/kb_build.py

FAILURE: the knowledge base does not describe the current source tree.
EXIT=1
```

**Result: 8 of 9 reporting checks pass. The single failure is V3, which is pre-existing** and is
diagnosed in §3.

**The V2 warnings are expected and are not defects.** `kb_check.py` V2 warns when a backticked
identifier does not exist anywhere in `src/`. The legacy knowledge base names legacy artefacts
(`ReportIGE1`, `SMStemplate`, `Helicobacter`, `VIDAS`, …) by necessity. The check stops reporting
after 25 warnings (`if reported > 25: return`), which is why the count is **26 in every run**,
before and after this pass — the warning set changed, the count did not.

---

## 3. The one remaining failure — V3, index freshness — and why it is pre-existing

### 3.1 The failure

```
  x [V3] the generated index is stale.
        recorded: fcf01a5c08806c9dab84f2f3ba1a003d4c4811aada9c7025d6179eeb8007f6a7
        actual  : 89f5010de2537ff00a9c37db4b9b5faaf0f45811ffa181ca9fdc148559493c62
```

### 3.2 Diagnosis

`source_fingerprint()` (`scripts/kb_build.py`) hashes the sorted `(path, size)` list of every
`.cs`/`.xaml` under `src/` and every `.cs` under `tests/` — **408 files**, exactly matching the
manifest's recorded `source_files: 408`. The recorded digest is reproducible under **none** of four
conventions tested:

| Convention | Computed digest | Matches recorded? |
|---|---|---|
| LF line endings, forward-slash paths | `89f5010de2537ff…` | No |
| CRLF line endings, forward-slash paths | `84e195b497b7990f…` | No |
| LF line endings, backslash paths | `bfd72a12db90c4a5…` | No |
| CRLF line endings, backslash paths | `64d35292a529b6ab…` | No |
| **Recorded in `kb-manifest.json`** | `fcf01a5c08806c9d…` | — |

### 3.3 Why it is pre-existing and not caused by this pass

- `src/` and `tests/` are **byte-identical** between the audited commit `e42bfca…` and its parent
  `46b8ad7…` — `git diff --stat 46b8ad7 e42bfca -- src/ tests/` is empty.
- The manifest's `verified_against_commit` is `46b8ad7b…`, the parent. The index was therefore
  generated from a source tree that is identical to the one at the audited commit.
- This pass changed **no file under `src/` or `tests/`** — it added and edited documentation and
  the validation script only.
- **Conclusion:** the generated index is *not* stale in substance. The **recorded fingerprint is not
  reproducible from the committed tree**, which is a defect in how the fingerprint was recorded
  (it was computed on a working tree whose file sizes differed from the committed blobs — most
  plausibly a CRLF checkout on Windows, since the file *set* is identical and only sizes differ).

### 3.4 The fix, and why it was not applied here

`python3 scripts/kb_build.py` regenerates the fingerprint from this working tree, after which V3
passes. It was **not** run as part of this documentation pass because:

1. It rewrites eight generated index files, which is a **source-tree** change, not a documentation
   change — and this pass is a documentation pass.
2. Doing so would make the V3 failure disappear without recording **why** it existed, which would
   hide a genuine reproducibility defect in a cross-platform project.
3. The change is trivial and safe for a maintainer to make deliberately.

**Recommendation (Priority 1):** run `python3 scripts/kb_build.py`, commit the regenerated
`Docs/KB/generated/*`, and then change `source_fingerprint()` to hash **content** rather than
**size**, so the fingerprint is independent of line-ending convention. Until then the gate cannot
be reproduced on a non-Windows checkout.

---

## 4. Corrections made in response to the checks

### 4.1 Knowledge errors found by re-verification

| # | Document | Error | Correction |
|---:|---|---|---|
| 1 | `data/MIGRATION.md` §1 | "12 CREATE TABLE" — the breakdown summed to 253, not 268 | **27** statements → 14 distinct tables; the census now reconciles |
| 2 | `data/MIGRATION.md` §3 | "109 of 132 column targets applied", "19 unapplied" (a table listing 23) | **90 of 132 present**, **42 absent**; the list extended to 42 |
| 3 | `data/MIGRATION.md` §3 | Created-table list named `hba1c`/`pictures`/`ratios`/`widal2` as migration-created and omitted `MyLabSetting` and `IgEallergy` | Rewritten from `migration-proof.json` |
| 4 | `clinical/RULES.md` §7 | **`180.rtf` labelled Sodium** | **Chloride, Cl** — the sheet's own header |
| 5 | `clinical/RULES.md` §7 | V1-BR-035 stated as a documentation claim | Upgraded to **VERIFIED at schema level** |
| 6 | `clinical/RULES.md` §5 | No explanation of the 852-row arithmetic | The **six-band duplication rule** added |
| 7 | `business/WORKFLOWS.md` §6 | "14 range attributes, all present in `normalrange` schema" | Corrected: all present in the **manual**, five **not** in the live schema |
| 8 | `architecture/OVERVIEW.md` §6 | Module numbering off by one module | Corrected against the vendor guide's own TOC |
| 9 | `business/CAPABILITIES.md` §1, §2 | "15 CHM topics"; "510 headings" | **17 topics / 15 capabilities**; the "510 headings" figure **withdrawn** as unreproducible |
| 10 | `VERSIONS.md` §4, §5 | DDL breakdown; `Data\*.ini` split 173/50; PE timestamp not marked as non-discriminating | All corrected; LCID and header counters added |
| 11 | `Docs/KB/legacy/evidence/EVIDENCE-REGISTER.md` | Listed **five Ghidra output files that do not exist**; "5 Python scripts"; two truncated digests presented as SHA-256; the V1 documents labelled inaccessible | All corrected |
| 12 | `Docs/KB/legacy/evidence/UNRESOLVED.md` §6 | Standing instruction to label every V1 reference `EXTERNAL SOURCE REQUIRED` and not to search | **Withdrawn** — the documents are accessible and already integrated |
| 13 | `Docs/KB/legacy/evidence/CONTRADICTIONS.md` | 15 contradictions; two known source conflicts unregistered | Now **22** (C-16 … C-22 added) |
| 14 | `business/SECURITY.md` | Maintenance password, clear-text SMS transport and the identifier leak unrecorded | V-18, V-19, V-20 added |

### 4.2 Path-reference corrections

The first gate run after the documentation pass reported **64 V1/V11 failures**, all of the form
*"`Docs/KB/legacy/evidence/CONTRADICTIONS.md` names a path that does not exist"*. The cause was that the new
documents referenced sibling legacy documents by a bare relative path, which the checker resolves
against the repository root rather than against the document's own directory. **All were corrected
to full repository-relative paths** (`Docs/KB/legacy/evidence/…`), and the gate then passed.

---

## 5. Budget revision — disclosed in full

Nine legacy character budgets were raised. Each increase is attributable to specific, verified
content added by this pass:

| Document | Was | Now | Δ | Cause |
|---|---:|---:|---:|---|
| `Docs/KB/legacy/evidence/CONTRADICTIONS.md` | 12,000 | 16,000 | +4,000 | Six new contradictions (C-16 … C-21) plus C-22, each with full evidence |
| `business/WORKFLOWS.md` | 10,000 | 14,000 | +4,000 | Six operator procedures recovered from the official manual |
| `clinical/RULES.md` | 10,500 | 13,000 | +2,500 | The six-band duplication rule, the critical-limit schema finding, the chloride correction |
| `data/MIGRATION.md` | 8,000 | 10,000 | +2,000 | The corrected reconciliation, including the 42 unapplied targets |
| `INDEX.md` | 7,500 | 9,500 | +2,000 | Six new documents in the inventory and the canonical-description rule |
| `business/SECURITY.md` | 10,000 | 12,000 | +2,000 | V-18, V-19, V-20 and the maintenance section |
| `Docs/KB/legacy/evidence/UNRESOLVED.md` | 12,000 | 14,000 | +2,000 | Seven new open items (U-16 … U-22) and the V1-accessibility resolution |
| `MAINTENANCE.md` | 6,500 | 7,500 | +1,000 | Rules 10 (register every source) and 11 (canonical descriptions) |
| `business/CAPABILITIES.md` | 8,000 | 8,500 | +500 | The topic-count and heading-count corrections |

**No budget was raised without new content.** The eight new documents received their own budgets,
sized to their content plus headroom — these are *new* budgets, not raised ones:

| New document | Budget | Content size |
|---|---:|---:|
| `SPECIFICATION.md` | 26,000 | 25,068 |
| `data/DATABASE.md` | 28,500 | 27,201 |
| `architecture/EXECUTABLE.md` | 16,000 | 15,805 |
| `Docs/KB/legacy/evidence/SOURCES.md` | 68,000 | 66,000 |
| `Docs/KB/legacy/evidence/COVERAGE.md` | 17,500 | 16,303 |
| `Docs/KB/legacy/evidence/VALIDATION.md` | 20,000 | 18,842 |
| `mapping/GAP-ANALYSIS.md` | 21,000 | 20,795 |
| `mapping/TRANSFORMATION.md` | 19,000 | 18,566 |

**Every budget for a document that did not grow is unchanged.** No check was weakened, bypassed or
removed: the V1–V11 checks all still run, and the rationale is recorded in a comment inside
`scripts/kb_check.py` immediately above `BUDGETS`.

---

## 6. Outstanding issues in the supplied evidence

### 6.1 Archive filename corruption — 47 entries

47 of the 298 corpus filenames contain byte sequences that are not valid UTF-8 — Arabic text that
was encoded as CP1256 and re-encoded as UTF-8 with unpaired surrogates. The names are recoverable
losslessly as escaped surrogates (Python-style `\uXXXX`, as used in `Docs/KB/legacy/evidence/SOURCES.md` §5) and
the content is intact, but any tooling that keys on these filenames will fail. The same corruption
class is documented at archive level in `ReferenceIndex/README.md` for the folder names.

**Handling:** preserve losslessly, record the recovered name, and normalise on ingest. The
knowledge base now cites corpus sources by **archive path plus hash**, so the corruption no longer
propagates into citations.

### 6.2 Dangling references inside the B01-B11 archive

The B01-B11 evidence register cites four paths the archive does not contain:

| Dangling path | Cited from |
|---|---|
| `analysis/BATCH-11/pdf-print-stack.md` | `11-Evidence/Evidence-Register.md`, `reports/Stage-9-…md` |
| `analysis/BATCH-01/pe-reports/` | `11-Evidence/Evidence-Register.md` (EV-ANA-009), `reports/Stage-2-…md` |
| `batches/BATCH-10/ingest-report.md`, `batches/BATCH-11/ingest-report.md` | the `ingest-report.md` pattern |
| `analysis/BATCH-10/cab/win7`, `analysis/BATCH-10/cab/winxp` | implied by the CAB citations |

**Impact:** a reader following B01-B11's own citations will fail to resolve them. All four exist
in **B01-B10** — which is the reason all three archives must be retained.

### 6.3 B01-B11 is not a superset

B01-B11 dropped **144 files** relative to B01-B10: the 96 extracted CAB binaries (replaced by
`analysis/BATCH-10/cab/cab-inventory.csv`, 96 rows — the index was kept and the payload withdrawn),
the 46 `analysis/BATCH-01/pe-reports/*.json`, and two narrative artefacts absorbed into the Stage-9
report. There is **no changelog** in any of the three archives; the CAB replacement is self-evident
from the citation change, but the 46 PE JSON drops are **unexplained**.

**Recommendation:** retain all three archives; treat B01-B11 as the authoritative *narrative and
governance* package and B01-B10 as the authoritative *raw payload* package.

### 6.4 The 837-file inventory is path-attributed, not content-attributed

**709 of 837 files (84.7 %)** are assigned to a version by path-prefix rule rather than by in-file
evidence, because `.ini`, `.rtf` and `.jpg` files carry no `FileVersionInfo`. This is recorded in
`VERSIONS.md` §9.4 and is inherent to the evidence, not to the documentation.

### 6.5 The corpus's own README index is stale

The deliverables README claims "21 constraints + 5 corrected bugs + 15 questions" where the
document actually holds **39 issues + 15 bugs + 15 questions**, and "57 evidence items recorded"
where the register holds **146 unique evidence IDs / 145 table rows**. The README tail was never
updated in any of the three generations.

---

## 7. Outstanding issues in the knowledge base

| # | Issue | Severity | Status |
|---:|---|---|---|
| 1 | The band-selection algorithm among overlapping ranges is **not established** (C-14) | **Safety-critical** | OPEN — the highest-severity open item |
| 2 | `pricemethod` value domain is not established (C-15) | Medium | OPEN — needs a vendor answer |
| 3 | `VIDAS` structure unknown | Medium | OPEN |
| 4 | Whether the migration is retried on a later run (V2-BR-023) | Medium | OPEN |
| 5 | The migration's failure-handling path — silent skip or logged error (V2-BR-024) | Medium | OPEN |
| 6 | `RICHTX32.OCX` — synthetic marker or real dependency (C-22) | Low | OPEN |
| 7 | `pregsmok` — clinical dimension or data-entry artefact | Medium | OPEN — owner decision required |
| 8 | `REFRANGE` semantics | Medium | OPEN — owner decision required |
| 9 | `loger` — attendance or audit | Medium | OPEN — vendor question |
| 10 | **No method body was ever recovered** — 1,268 of 8,026 entries read | **Structural** | Unresolvable from the corpus |
| 11 | **No runtime behaviour of either version was ever observed** | **Structural** | Unresolvable from the corpus |
| 12 | 22 of 72 `V2-REQ-*` identifiers have no modern use case | Medium | OPEN — see `mapping/TRANSFORMATION.md` M-1 |
| 13 | The V1 documents are not preserved inside the repository | Low | OPEN — a preservation gap, not an access gap |
| 14 | V3 index fingerprint not reproducible across platforms | Medium | OPEN — §3 |

---

## 8. Limitations of this validation

1. **No check was performed against the legacy binaries or databases.** They are not available;
   every schema and executable claim derives from `mdbtools` dumps and analysis outputs, which
   themselves derive from those binaries.
2. **No runtime validation was possible.** Nothing in this pass observes either version running.
3. **The gate is the project's own gate.** It checks structure, paths, links, budgets, numeric
   claims and convention guards — not technical correctness of the legacy claims.
4. **The 26 V2 warnings were not individually adjudicated.** They are all legacy identifiers, which
   the check cannot distinguish from typos.
5. **`kb_check.py` V9 exempts paragraphs containing a legacy cue.** The exemption is deliberate
   and pre-existing, but it means a modern-number typo inside a legacy-tagged paragraph would not
   be caught.
6. **The archive inventory was verified by extraction, not by execution.** No delivered artefact
   was run.

---

## 9. Related documents

- `Docs/KB/legacy/evidence/SOURCES.md` — source traceability and disposition register.
- `Docs/KB/legacy/evidence/COVERAGE.md` — integration and coverage accounting.
- `Docs/KB/legacy/evidence/CONTRADICTIONS.md` — 22 conflicts and their resolutions.
- `Docs/KB/legacy/evidence/UNRESOLVED.md` — open questions.
- `Docs/KB/legacy/mapping/TRANSFORMATION.md` — the plan that consumes these findings.


---

## Evidence Batch 02 (2026-11-16) — Budget adjustments

Two legacy budgets were raised to accommodate the Batch 02 integration (new contradictions C-16..C-20 and unresolved questions UNR-002..UNR-009):
- CONTRADICTIONS.md  16000 -> 19000  (five new contradictions)
- UNRESOLVED.md      14000 -> 16000  (six new open items)

Every increase is attributable to specific new content and is recorded here. No check was weakened or bypassed.
