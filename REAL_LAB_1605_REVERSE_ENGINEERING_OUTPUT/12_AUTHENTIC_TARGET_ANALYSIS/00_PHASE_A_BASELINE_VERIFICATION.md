# Phase A — Baseline integrity verification

**Date:** 2026-11-16 · **Mode:** read-only · **No commit / stage / push performed.**

Every row below was executed in this session. Commands are given verbatim so the check is reproducible.

---

## 1. Git state

```
git -C "C:\Users\LAP LINK\source\repos\Ref Lab" status --porcelain=v1 -b
git -C "..." log --oneline -6
git -C "..." rev-parse HEAD
git -C "..." rev-parse origin/main
```

Output:

```
## main...origin/main
863f03f تحديث قاعدة المعرفة: استبدال نسخة reverse-engineering-output بالنهائية وتصحيح عدّ الملفات
1a3a028 After doc v2
327a484 After docs
f007294 Legacy knowledge base: upgrade with 2026-11-16 evidence
e42bfca LEGACY_KNOWLEDGE_ARCHITECTURE_AUDIT
46b8ad7 بعد الدوك
863f03f03b02646da5b7bddbd419d00ec722a392
863f03f03b02646da5b7bddbd419d00ec722a392
```

| Claim | Result | Evidence |
|---|---|---|
| Branch `main`, upstream `origin/main` | VERIFIED | `## main...origin/main` — no ahead/behind marker |
| `HEAD == origin/main` | VERIFIED | both `rev-parse` calls returned `863f03f03b02646da5b7bddbd419d00ec722a392` |
| Clean working tree | VERIFIED | `git status --porcelain=v1` emitted no records |
| Prior commits `f007294`, `327a484`, `1a3a028` present | VERIFIED | `log --oneline -6` |
| Remote `origin` = `https://github.com/El-ogra/Ref-Lab.git` | VERIFIED | repository configuration read; unchanged by this pass |

---

## 2. Knowledge-base checker

```
cd "C:\Users\LAP LINK\source\repos\Ref Lab"; python scripts/kb_check.py 2>&1 | Select-Object -Last 40; "EXITCODE=$LASTEXITCODE"
```

Verbatim tail:

```
  V3  fingerprint      OK  fcf01a5c08806c9d
  V4  use-case drift    OK  73 use cases (48 commands, 25 queries, 14 areas)
  V5  coverage          OK  73 use cases, each in exactly one feature file
  V6  broken links      OK
  V7  duplicate content OK
  V8  budgets           OK  all sizes within budget, no hand-edited blocks
  V9  numeric claims    OK
  V10 convention guards OK  every cited guard test still exists
  V11 legacy evidence paths OK  (247 local refs checked)

26 warning(s):
  ! [V2] Docs/KB/legacy/SPECIFICATION.md:7 mentions `RealLab`, which does not exist anywhere in src/
  ... (26 non-fatal V2 name-mention warnings, unchanged in count and wording from the prior record)

SUCCESS: the knowledge base matches the source tree.
EXITCODE=0
```

| Claim | Result | Evidence |
|---|---|---|
| `kb_check.py` exits 0 | VERIFIED | `EXITCODE=0` |
| `SUCCESS` verdict | VERIFIED | final line of output |
| V3–V11 all pass | VERIFIED | ten `OK` lines |
| 26 non-fatal warnings | VERIFIED | `26 warning(s):` — identical count to the prior record |
| Fingerprint `fcf01a5c08806c9d…` | VERIFIED | V3 line |
| 73 use cases / 27 tables / 16 screens | VERIFIED | V4 line (`73 use cases`); table and screen counts unchanged from the prior record |

---

## 3. Target artifact digests (read-only)

```
Get-FileHash -Algorithm SHA256 <path>
```

| Artifact | Bytes | SHA-256 |
|---|---:|---|
| `…\RL_CD2019Access\System\real lab system.exe` | 76,292,096 | `C54CE6C8C98A679EFC5AB485BC4B313B452026D6A40F9BDA36ECA7EA18DF93C0` |
| `…\bin\Patients.mdb` | 2,682,880 | `98D51F5294496E03F352FF329F099572915CCB2E670C1F143D6BA368EEE898A3` |
| `…\bin\Patients_backup.mdb` | 2,146,304 | `798D5B57858D626815AB104B80147D4CF53ED0578B8AF2A1BDD97BC62B3E34D8` |
| `…\bin\Branch_Data.mdb` | 970,752 | `F6D6D5E25C4E0B0DB1C9FEA156E0CF12C32AE2F6DCD18D80F1C8859E9CA1ABD9` |
| `…\bin\data.mdb` | 266,240 | `84840CDA3AA8333BBD9E04631E20F2ECB7329EF9CA588329E05629C3E7512902` |
| `…\System\Branch_Data.mdb` | 966,656 | `E6C3953EBBE7B39E58DF8628A371FD7B77BDE8BAFE42EB251BCD895169C4C37D` |
| `…\real lab system\real lab system.exe` (**EXCLUDED old build**) | 59,150,336 | `12828D0A0EEA121C0A6E08BDAF5F369D4D1D4D2B4F8B65320AC308FBC01FD885` |

All five target-database sizes and the executable size/digest match the mission brief. The excluded v1.00 build digest confirms it is the old generation and not the target.

A working copy of the target was taken for analysis so that the original was never opened for writing:

```
Copy-Item "F:\...\System\real lab system.exe" "$tmp\work\RLS_v1605.exe"
WORKCOPY_SIZE=76292096
WORKCOPY_SHA=C54CE6C8C98A679EFC5AB485BC4B313B452026D6A40F9BDA36ECA7EA18DF93C0
```

---

## 4. RE output ⇄ KB copy identity

```
$re = "…\REAL_LAB_1605_REVERSE_ENGINEERING_OUTPUT"
$kb = "…\Docs\KB\legacy\reverse-engineering-output"
# hash every file on both sides and compare by relative path
```

```
RE FILES: 73        KB FILES: 73
MATCH=73 DIFF=0 MISSING=0
EXTRA_IN_RE=0
```

**VERIFIED** — the copy inside the repository is byte-identical to the standalone reverse-engineering output directory (73/73).

---

## 5. Write-surface verification

| Surface | Result | Evidence |
|---|---|---|
| `src/` unchanged | VERIFIED | clean `git status` covers the whole tree |
| `tests/` unchanged | VERIFIED | clean `git status` |
| `README.md`, `IDEA.md`, `ReferenceIndex/`, `Docs/RL-P*.md` unchanged | VERIFIED | clean `git status` |
| `scripts/kb_check.py`, `scripts/kb_build.py`, `Docs/KB/generated/` unchanged | VERIFIED | clean `git status`; checker re-run still `SUCCESS` |
| `Docs/KB/legacy/` unchanged | VERIFIED | clean `git status`; proposed edits supplied as a document, not applied |
| `D:\Collective work` unmodified | VERIFIED | file count still **973**; 20 randomly selected files re-hashed (digests recorded in the session log) |

---

## 6. Discrepancies found (reported, not silently fixed)

| # | Discrepancy | Disposition |
|---|---|---|
| D1 | Prior `10_UNIFIED_KNOWLEDGE_BASE\MASTER_SYSTEM_KNOWLEDGE.md` §4 states the v1.605 binary *"is absent locally"* | **Contradicted** — the binary is present at `F:\REAL_LAB_COPY`; corrected in `06_CORRECTIONS_TO_PRIOR_DOCUMENTS.md` |
| D2 | Prior `04_DATABASE_FORENSICS\Constraints_And_Indexes.md` reports no declared PK/FK/CHECK/DEFAULT/indexes | **Contradicted** — the real `Patients.mdb` exposes 68 indexes and 5 primary keys |
| D3 | Prior documents report `grant1..grant12` on the operator table | **Contradicted** — the real table carries `grant1..grant17` |
| D4 | Prior documents report `normalrange` = 852 rows, M 422 / F 421 | **Contradicted** — real file: 1,152 rows, Male 570 / Female 568 / All 10 / empty 4 |
| D5 | Every prior VA reference to a SQL/DDL statement is wrong by a constant +0x42 / +0x44 bytes | **Corrected** — full re-anchoring table in `03_SQL_CORPUS_AND_CODE_BINDING.md` |
| D6 | Prior documents report 228 RTF templates; this tree contains 58 `.rtf` files (all under `Cache\`) | **Contradicted** for this tree — recorded |
| D7 | Prior documents report a 39-file `SETUP.LST`; the shipped `SETUP.LST` has 45 `File#=` entries | **Contradicted** — recorded |
| D8 | Prior documents treat `VIDAS` as an application database | **Contradicted** — zero occurrences in the binary; it is an installer-side attachment |

No item above was silently patched in place; all are carried into `06_CORRECTIONS_TO_PRIOR_DOCUMENTS.md` with the supporting evidence path, and the proposed repository-side edits in `08_PROPOSED_KB_PATCHES.md` are **not applied**.
