# Ref Lab — Phase 5 Documentation Corrigendum

**Document ID:** `RL-P5-COR-001`
**Issued:** 2026-11-10
**Applies to:** `After completing the fourth stage Handoff.md` (`RL-HANDOFF-003`, version 3.0)
**Authority:** the project owner, in the Phase 5 execution directive of 2026-11-10

---

## 1. What this corrigendum is for

`RL-HANDOFF-003` states the size of the Application layer twice, and both
statements are wrong. The total of **70 use cases across 14 feature areas** is
correct; the split between commands and queries is not.

This document carries the **exact corrected wording** for the two occurrences.
The handoff file itself is **not** edited — it is an accepted record of the
project state, and the owner has directed that the correction travels beside
it rather than inside it. Apply the two replacements below to the owner's copy
of `RL-HANDOFF-003` when convenient, or keep this corrigendum beside it and
treat it as the authority on the two figures.

---

## 2. The verified figure

| Measure | Value | Where it is verified |
|---|---:|---|
| Use cases | **70** | `python scripts/generate_use_case_inventory.py` → `use cases found: 70 (expected 70)`, exit code 0 |
| Commands | **45** | The same generator, `Docs/USE_CASE_INVENTORY.md` §1 "Totals", and a direct scan of `src/RefLab.Application/Features/` |
| Queries | **25** | The same three sources |
| Feature areas | **14** | The same three sources |
| Total | **70** | 45 + 25 = 70 ✓ |

Three independent sources agree, and the repository's own drift guard — which
fails with a non-zero exit code when the count or the coverage drifts — reports
the expected total. The two occurrences in the handoff disagree with all
three.

> **Note on the Phase 3 figure.** The superseded handoff
> `From-Phase-0-to-3-Handoff.md` records *"46 commands and 27 queries"* for
> **73** use cases. That statement was correct for the Phase 3 state and is
> **not** affected by this corrigendum. The branch removal of Phase 4 deleted
> three use cases, of which one was a command and two were queries, which is
> how 46/27 became 45/25.

---

## 3. The two corrections

### 3.1 Section 7.1.2 — "Commands, queries and handlers"

**Occurrence.** Line 457 of `RL-HANDOFF-003`:

> There are 43 commands and 27 queries (70 total).

**Replace that sentence with:**

> There are 45 commands and 25 queries (70 total).

Nothing else in the section changes. The sentence that follows it — *"The
final use case count is 70"* and the count history it records — is correct and
stays as it is.

### 3.2 Section 15.1 — the architecture diagram

**Occurrence.** Line 1428 of `RL-HANDOFF-003`, inside the `RefLab.Application`
box of the four-layer diagram:

```
│  Features/ — 70 use cases (43 commands · 27 queries)               │
```

**Replace that line with:**

```
│  Features/ — 70 use cases (45 commands · 25 queries)               │
```

The line is inside an ASCII box, so the replacement must keep the same width
or the right-hand border of the box will not line up. The replacement above is
the same number of characters as the original, because `43` and `45` and `27`
and `25` are all two digits.

---

## 4. What is **not** corrected

- The **total of 70 use cases** and the **14 feature areas** are correct in
  both occurrences and are left unchanged.
- The counts of validators (70), named permissions (15), abstractions (8),
  pipeline behaviours (4), tables (27) and columns (338) are unchanged.
- The **535-test baseline** and the **538-test owner figure** are unchanged.
  The difference between them is not a runner artefact: it is the three
  `[Fact]` tests of `DeliveryRecordTests`, which the Phase 4 handoff lists as
  deleted but which were still present in the pinned commit and which the
  Phase 5 delivery removes. See §7.8 of the Phase 5 delivery manifest.
- **No other document is affected.** The Phase 4 manifest already states
  `45 / 25`, and `Docs/USE_CASE_INVENTORY.md` is generated from the source and
  is therefore always correct.

---

## 5. How the correction was established

| Step | Command | Result |
|---|---|---|
| 1 | `python scripts/generate_use_case_inventory.py` | `use cases found: 70 (expected 70)`, exit code 0; the regenerated `Docs/USE_CASE_INVENTORY.md` is byte-identical to the committed copy |
| 2 | Scan of `src/RefLab.Application/Features/` for `public sealed record …Command` and `…Query` | 45 commands, 25 queries |
| 3 | `Docs/USE_CASE_INVENTORY.md` §1 "Totals" | `Commands 45`, `Queries 25`, `Use cases implemented 70`, `Feature areas 14` |
| 4 | `Docs/RL-P4-MAN-001-Phase4-Delivery-Manifest.md` §1 verification table | `Commands / queries 45 / 25` |

Four sources, one figure.

---

**End of `RL-P5-COR-001`.**
