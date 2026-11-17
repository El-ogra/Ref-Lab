# Corrections to the prior reverse-engineering documentation

**Policy followed.** Nothing in the prior output was deleted, overwritten or silently edited. Every correction below is recorded here with its rationale and the evidence that supports it. The prior 73 files remain byte-identical to `Docs/KB/legacy/reverse-engineering-output/` (verified 73/73).

Two classes of correction appear:

- **CONTRADICTED** — a prior statement is false against the authentic artifact and is replaced.
- **SUPERSEDED** — a prior statement was true *about the prior tooling's limitations* but is now obsolete because the tooling was fixed.

---

## C1 — "The v1.605 binary is absent locally" → **CONTRADICTED**

*Prior text:* `10_UNIFIED_KNOWLEDGE_BASE/MASTER_SYSTEM_KNOWLEDGE.md` §4 — *"This binary is absent locally; characterized from `real lab system.exe (2).json`."* and §20 — *"target binary absent; no isolated environment"*.

*Correction:* the binary is **present** at
`F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe`
(76,292,096 B, SHA-256 `c54ce6c8…df93c0`), and it is the basis of every new finding in this pass.

*Evidence:* `evidence/target-tree-sha256-manifest.csv`; `evidence/vb6-objects-and-method-names.json`; `evidence/sql-literal-to-code-sites.json`.

*Precise distinction to preserve:* the binary was absent **from `D:\Collective work`** — the evidence repository the prior pass worked from — not from the product tree. The prior analysis was therefore conducted on PE dumps and extracted artefacts rather than on the executable.

---

## C2 — "No declared PK/FK/CHECK/DEFAULT/indexes" → **CONTRADICTED**

*Prior text:* `04_DATABASE_FORENSICS/Constraints_And_Indexes.md` and the corresponding `MASTER_SYSTEM_KNOWLEDGE.md` §6.

*Correction:* the shipped `Patients.mdb` declares **68 indexes** and **5 primary keys**:

| Table | Primary key | Other indexes |
|---|---|---|
| `instrument` | `instrumentid` | — |
| `pricemenu` | `pricelistname` | — |
| `sharekat` | `sherkaid` | `sherkaid` |
| `testtable` | `testcode` | `testcode` |
| `testtemp` | `testcode` | `arrangnum`, `patientid`, `subgroupcode` |
| `barcodes` | — | unique `barcodeid` |
| `patients` | — | unique `patientusid` |
| 12 result tables | — | `patientid` |
| `anyreport`, `pictures`, `wasl` | — | `testcode` |
| `sharekathesab`, `history`, `insulin`, `MyLabSetting`, `CSFT`, `ZN`, `pictures` | — | various |

No foreign keys and no check constraints are declared anywhere — that part of the prior statement stands.

*Why the prior pass missed it:* it read DDL dumps, which cannot express Access index metadata.

*Evidence:* `evidence/mdb-forensics/bin_Patients/adox_indexes.tsv`, `indexes.json`.

---

## C3 — `grant1..grant12` → **CONTRADICTED** (now `grant1..grant17`)

*Correction:* the operator table `patients` has **33 columns** including **17** Boolean grant flags. The binary's own DDL proves the count:

```
ALTER Table Patients ADD grant13 Bit            … ADD grant17 Bit
ALTER Table Patients ADD COLUMN grant13 YESNO   … ADD COLUMN grant17 YESNO
```

*Evidence:* `evidence/mdb-forensics/bin_Patients/columns.json`; binary literals at `0x2dc360`–`0x2dc690`.

---

## C4 — `normalrange` row counts and sex split → **CONFLICTED**

| Prior | Real file |
|---|---|
| 852 rows | **1,152 rows** |
| M 422 / F 421 | **Male 570 / Female 568 / All 10 / empty 4** |

Additionally established: `REFRANGE` distribution **empty 797 · `'0'` 345 · `'1'` 10 · `'2'` 0** — the *absence* of `REFRANGE='2'` is now a verified fact about the shipped mirror, matching the binary's `REFRANGE='2'` queries.

*Evidence:* `evidence/mdb-forensics/bin_Patients/rowcounts.json`, `counts.txt`.

---

## C5 — Every prior virtual address → **CONFLICTED, corrected**

*Prior state:* 2,065 address references (1,797 statement VAs + 268 DDL VAs) across the prior documents, anchoring specific claims such as "VA `69EA76`" for the net formula, "VA `681ED2`" for the result-lifecycle predicates, "VA `72714C`" for the branch-code rule, "VA `6860BE`", "VA `6865A6`", "VA `6867AA`", "VA `686EC2`", "VA `6C3AFC`", "VA `694C92`", "VA `6A5796`", "VA `6A9632`", "VA `6843F2`", "VA `672722`", "VA `66F746`", "VA `66FCB2`", "VA `6717E2`", "VA `722674`", "VA `6CEA20`".

*Audit result:*

| Dimension | Against v1.605 | Against v1.00 |
|---|---:|---:|
| Prior statement texts found verbatim | **1,698 / 1,797** (+95 partial) | 836 / 1,797 |
| Prior DDL texts found verbatim | **262 / 268** (+6 partial) | **0 / 268** |
| Prior VAs resolving to their claimed statement | **0 / 1,797** | 0 / 1,797 |

The prior **content** is v1.605 (the old build contains none of the 268 DDL statements), but the prior **addresses** are systematically **0x42 or 0x44 bytes below** the truth in the shipped file.

*Correction:* all VA references are re-anchored in `03_SQL_CORPUS_AND_CODE_BINDING.md` §6. The **semantic claims** they supported are independently re-verified at the new addresses and stand, with one refinement: the referrer commission's base is `patientinfo.totalpricedisc`, not a generic amount.

*Evidence:* `evidence/prior-corpus-audit-vs-authentic-binary.json`, `evidence/va-reanchoring.json`.

---

## C6 — "0 method names recovered" → **SUPERSEDED**

*Prior state:* 8,026 declared methods, 1,268 addressable, **0 names** — and the prior KB concludes that *"method names for most of the 8,026 declared methods were not stored in the evidence"*, a statement about the *evidence repository*, not about the binary.

*Correction:* the cause was a **tooling defect**, not an artifact limit. The prior `vb6_parse.py` reads the object-array pointer from `ObjectTable+0x38` (which is zero); the correct field is `ObjectTable+0x30`. With that fixed:

- 210 of 212 objects named;
- **444 method names** recovered;
- Σ declared methods = **8,026**, confirming the prior figure exactly.

*Evidence:* `evidence/vb6-objects-and-method-names.json`; `scripts/vb6_names.py`.

---

## C7 — "228 RTF templates" → **CONFLICTED for this tree**

The prior documents report 228 RTF templates. This tree contains **58** `.rtf` files, all under `real lab system\Cache\` — i.e. assets of the 13 Flash projector executables, **not** report templates. The report templates are the **228 `.ini` files under `Data\`** (223 directly + 5 under `Data\data_emp\`), each with a `[reportdata]` section and a `testtype=` binding.

The prior figure of 228 is therefore correct for the **template** corpus and was mis-attributed to the RTF format.

*Evidence:* `01_TARGET_TREE_INVENTORY.md` §2, §4.6.

---

## C8 — "39-file `SETUP.LST`" → **CONFLICTED**

The shipped `WIN7\SETUP.LST` has **102 lines** and **45 `File#=` entries** across three sections (`[Bootstrap Files]` 6, `[Setup1 Files]` 36, Jet-4.0 group 3).

*Evidence:* `01_TARGET_TREE_INVENTORY.md` §6.

---

## C9 — "`VIDAS` is an application database" → **CONTRADICTED**

*Prior state:* the SQL Server side is described as carrying a `patients` / `Interface` / `VIDAS` catalogue.

*Correction:* **"VIDAS" occurs zero times in the authentic v1.605 binary** (ASCII and UTF-16LE, whole-file search). The name belongs to the **installation chain**: `System\Setup.bat` runs `OSQL -S %COMPUTERNAME% -E /i Attaching.sql` and `Attach_database.bat` in `D:\Lab_data`, and the prior deployment report records `D:\Lab_data\Attach_VIDAS.bat`. `VIDAS` is an externally attached database that the application never references, and no structure for it is recoverable or required.

*Evidence:* whole-file string scan of the authentic binary; `01_TARGET_TREE_INVENTORY.md` §6.

---

## C10 — Two distinct `patientid` substring rules conflated → **VERIFIED, separated**

The prior documents carry both *"position 9 is the branch code (`SUBSTRING(patientid,2,1)+SUBSTRING(patientid,9,1) AS bran`, 18 statements)"* and *"13-character patient ID with `SUBSTRING(...,10,3)`"* — and the prior knowledge base quotes them under one heading.

*Correction:* they are **two different rules**, both present:

| Rule | Occurrences | First address | Meaning |
|---|---:|---:|---|
| `SUBSTRING(patientid,2,1) + SUBSTRING(patientid,9,1) as bran` | 18 | `0x727190` | branch code — position 2 + position 9 |
| `SUBSTRING(patientid,10,3) as patid` | 58 | `0x682F98` | positions 10–12 |

Both have Jet-dialect `MID(...)` twins, confirming the dual-dialect claim.

*Evidence:* `03_SQL_CORPUS_AND_CODE_BINDING.md` §7.

---

## C11 — "No target binary was executed" → **still VERIFIED (unchanged)**

The target was not executed in this pass either. The static finding that is new is the **DDL migration region** (291 of 307 DDL code sites inside one contiguous region beginning `0x01B61DAD`, in source order, with a dialect-mirrored second copy) — a static locator for start-up work, not an observation.

*Evidence:* `03_SQL_CORPUS_AND_CODE_BINDING.md` §4.

---

## C12 — Prior statements that stand unchanged (explicitly re-verified)

| Prior claim | Verdict |
|---|---|
| 103 tables / 1,682 columns in `Patients.mdb` | **VERIFIED exactly** against the real file |
| 90 / 1,381 in `Patients_backup.mdb` | **VERIFIED** |
| 49 / 969 / 0 rows branch template | **VERIFIED** |
| 0 literal `INSERT INTO`, 0 literal `UPDATE … SET` | **VERIFIED** (bound-recordset writes) |
| 268 embedded DDL (27 CREATE + 241 ALTER) | **VERIFIED** — re-extracted as 268 |
| 231 vs 186 `MSVBVM60` imports | **VERIFIED** by an independent PE import parser |
| PE timestamp `1595435962` = 2020-07-22 | **VERIFIED** |
| 64-table `DELETE FROM … WHERE patientid` cascade | **VERIFIED** |
| `SHAPE` hierarchical queries | **VERIFIED** (39 SHAPE literals) |
| Dual SQL-Server / Jet dialect pairs | **VERIFIED** (18 `as bran` statements each with a `MID` twin) |
| Result-lifecycle flags and six audit columns | **VERIFIED**, addresses re-anchored |
| Net = gross − discount with `ROUND(…,2)` | **VERIFIED**, address re-anchored to `0x69EAB8` |
| `farq = wareed − saader` | **VERIFIED**, address re-anchored to `0x7226B8` |
| Unpaid predicate | **VERIFIED**, address re-anchored to `0x6CEA64` |
| `REFRANGE='2'` absent from the Access mirror | **VERIFIED** (zero such rows) |
| 223 `Data\*.ini` templates | **VERIFIED** (223 + 5 = 228) |
| `loger` table, `sp_addumpdevice` / `sp_dropdevice`, error string | **VERIFIED**, addresses re-anchored |
