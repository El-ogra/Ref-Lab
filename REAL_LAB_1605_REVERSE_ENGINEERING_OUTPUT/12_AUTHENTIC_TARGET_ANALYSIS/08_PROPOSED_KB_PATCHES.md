# Proposed edits to `Docs/KB/legacy/` — NOT APPLIED

**Status: proposals only.** Per the mission brief and `AGENTS.md`, nothing under `Docs/KB/legacy/` was edited. This document states each proposed edit so the owner can approve, amend or reject it. Applying any of these requires an owner decision; `scripts/kb_check.py` and `scripts/kb_build.py` were likewise not touched, and no commit, staging or push was performed.

**What was actually written by this pass:** only `REAL_LAB_1605_REVERSE_ENGINEERING_OUTPUT/12_AUTHENTIC_TARGET_ANALYSIS/` and `…/13_FINAL_REPORT/`.

---

## Proposed edit 1 — `Docs/KB/legacy/INDEX.md`: add a pointer to the authentic-binary analysis

*Rationale:* the legacy knowledge base is the navigation layer for the two legacy systems; it must expose where the v1.605 evidence now lives.

*Proposed addition (one block, no existing text removed):*

```markdown
> **أدلة الملف التنفيذي الأصلي (v1.605).** التحليل العميق للمل التنفيذي الأصلي وقواعد البيانات
> الحقيقية في `REAL_LAB_1605_REVERSE_ENGINEERING_OUTPUT/12_AUTHENTIC_TARGET_ANALYSIS/`
> و`13_FINAL_REPORT/`.
```

*(The Arabic block above is a placeholder only — the owner will supply or approve the final wording; per `AGENTS.md` §7 all Arabic text must come from `Docs/ui-labels-verified.md`, so no Arabic string may be added without that source.)*

---

## Proposed edit 2 — `Docs/KB/legacy/SPECIFICATION.md`: five factual corrections

Each item below is a **localised factual correction**, not a rewrite. The current text stays except for the fragment named.

| § | Current fragment | Proposed replacement | Why |
|---|---|---|---|
| Database section | `Patients.mdb` listed without constraints | add: 103 tables / 1,682 columns / **68 indexes / 5 primary keys** | the prior text derived from DDL dumps; the real file declares indexes and PKs |
| Permissions section | `grant1..grant12` | `grant1..grant17` (17 Boolean flags) plus `SecWord` and `patientword` | the shipped `patients` table has 17 grant columns; the binary's DDL adds `grant13`–`grant17` |
| Reference-ranges section | `normalrange` = 852 rows, M 422 / F 421 | **1,152 rows; Male 570 / Female 568 / All 10 / empty 4; `REFRANGE` = empty 797 / '0' 345 / '1' 10 / '2' 0** | measured on the real file |
| Reference-ranges section | reference ranges "absent from the Access mirror" | keep, and add the measured distribution above | the absence of `REFRANGE='2'` is now a verified fact, not an inference |
| Deployment section | `VIDAS` treated as an application database | **`VIDAS` is not referenced by the application binary (zero occurrences); it is attached at install time by `Attaching.sql` / `Attach_database.bat` in `D:\Lab_data`** | whole-file string search of the authentic binary |

---

## Proposed edit 3 — `Docs/KB/legacy/VERSIONS.md`: version-identification corrections

| Item | Proposed change |
|---|---|
| v1.605 availability | record that the authentic executable and all five shipped databases are present at `F:\REAL_LAB_COPY` (with the SHA-256 manifest), and that they were absent only from `D:\Collective work` |
| address convention | add a warning that the 2,065 address references carried by the prior evidence are offset by **+0x42 / +0x44 bytes** from the addresses in the shipped image, and point to the re-anchoring table `12_AUTHENTIC_TARGET_ANALYSIS/03_SQL_CORPUS_AND_CODE_BINDING.md` §6 |
| method names | record **444** recovered method names (prior 0) and the corrected cause: the object-array pointer is at `ObjectTable+0x30`, not `+0x38` |
| template formats | record that the 228 report templates are the `.ini` files under `Data\`; the 58 `.rtf` files in this tree belong to the Flash projector assets |

---

## Proposed edit 4 — `Docs/KB/legacy/SPECIFICATION.md` §21-style guard: cross-system confusion

Add one explicit paragraph, because the risk grew with this pass:

> The authentic v1.605 artefacts live outside the repository, at `F:\REAL_LAB_COPY`. Facts about that tree — its 837 files, its 68 database indexes, its 17 grant flags — describe **REAL LAB SYSTEM v1.605** and must never be attributed to the local C#/.NET 8 reconstruction in this repository.

---

## Edits deliberately **not** proposed

| Not proposed | Why |
|---|---|
| Touching `src/`, `tests/`, `README.md`, `IDEA.md`, `ReferenceIndex/`, `Docs/RL-P*.md` | forbidden by `AGENTS.md` and by the mission brief |
| Editing `scripts/kb_check.py` or `scripts/kb_build.py` | requires explicit owner approval; the checker passes as-is |
| Regenerating `Docs/KB/generated/` | the scripts write only there, but no index was stale — `kb_check.py` reported `V3 fingerprint OK fcf01a5c08806c9d` before and after this pass |
| Adding Arabic labels | `AGENTS.md` §7 — every Arabic string must come from `Docs/ui-labels-verified.md`; no new Arabic product text is introduced by this pass, so nothing needs to be sourced |
| Replacing or deleting any of the 73 prior reverse-engineering files | prior evidence is preserved; corrections are additive |
| Any commit / staging / push | excluded by the mission brief; `HEAD` remains `863f03f` |

---

## Owner checklist for approval

1. Read `12_AUTHENTIC_TARGET_ANALYSIS/06_CORRECTIONS_TO_PRIOR_DOCUMENTS.md` (12 numbered corrections, each with evidence).
2. Decide which of proposed edits 1–4 to apply.
3. Approve Arabic wording, sourcing every string from `Docs/ui-labels-verified.md`.
4. Re-run `python scripts/kb_build.py` then `python scripts/kb_check.py` and confirm `SUCCESS` with exit code 0.
5. Commit separately from this analysis output.
