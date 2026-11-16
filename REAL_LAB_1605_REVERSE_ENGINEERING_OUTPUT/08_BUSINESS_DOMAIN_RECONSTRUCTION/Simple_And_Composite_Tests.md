# Simple and Composite Tests — REAL LAB SYSTEM v1.605 (V-CD-2019)

> **Reading rule.** This document distinguishes *catalogue entries* from *result storage*.
> A single-analyte test (Creatinine, Urea, Uric Acid) can still be stored in a wide,
> multi-column panel table. Conversely, a multi-component panel (CBC) is both a catalogue
> concept and a wide table. Do not assume the two layers use the same processing.

**Evidence basis.** `v2019-Patients.mdb.sql` (CREATE TABLE for profile tables),
`testtable.csv` (header) and `testtable-stats.json` (609-test statistics),
`template-corpus-228.json` (228 INI templates / 117 identities),
`test-catalogue.csv` (114-row catalogue of template identities), `normalrange.csv`
(852 rows / 304 test codes), `constanttable.csv`, `converts.csv`, `testunits.csv`,
`statements__[analysis_BATCH-04_sql_v1605].csv`, `ui-labels-verified.md`.

## 1. The catalogue layer — `testtable`

`testtable` declares **38 columns** in `v2019-Patients.mdb.sql` (the redacted `testtable.csv`
sample exposes the same header). `testtable-stats.json` gives the authoritative counts:

| Measure | Value | Confidence |
|---|---|---|
| Total tests | **609** | STRONGLY_SUPPORTED (derived statistic; same figure in `Version_Identity_and_Scope.md` §2.4) |
| Distinct subgroups used | **48** | STRONGLY_SUPPORTED (note: `04-clinical-rules.md` §6 and `Version_Identity_and_Scope.md` §2.4 say **46** `subgrouptable` rows — see `Conflicting_Findings.md`) |
| Active | **94 of 609** | STRONGLY_SUPPORTED |
| Delete-protected | **605 of 609** | STRONGLY_SUPPORTED |
| With `patientprice` | **591 of 609** | STRONGLY_SUPPORTED |
| With `otherlab_price` | **591 of 609** | STRONGLY_SUPPORTED |
| Send-out enrolments | **1** | STRONGLY_SUPPORTED |
| `seereport='See Report'` | 48 rows | STRONGLY_SUPPORTED (`1605-phase2-findings.md` U-04) |
| `REFRANGE` values | `''` 538 · `'0'` 46 · `'1'` 23 · `'3'` 2 | STRONGLY_SUPPORTED (U-04) |

The catalogue is **flat**: every test has its own `testcode`. Composite membership is
*not* recorded by a "parent test" column; it is implied by subgroup and by the report
template. `testname` (`groupcode`, `subgroupcode`, `arrangnum`, `testcode`, `num`) is the
report-ordering map. Confidence: VERIFIED (DDL) / INFERRED (absence of a parent column).

## 2. The result-storage layer — wide profile tables

`v2019-Patients.mdb.sql` declares many "profile" tables keyed by `patientid` + `requestdate`
with one *column per analyte*. Exact column groups recovered:

| Profile table | Analyte columns (verbatim) | Count | Reading |
|---|---|---:|---|
| `cbc` | `hgb, rbc, hct, plt, hgbper, mcv, mch, mchc, pct, mpv, wbc, neut, lymph, mono, eosino, baso, othercell, RDW, PDW, seg, bandx, Juvenile, Myelocytes, Promyelocyte, Blast, NRBCWBC` | 26 | **Composite panel** — whole blood count + differential |
| `kidney` | `creat, uric, urea, bun, UVolume, Creatu, CreatClear, Microalbu, Proteinu, Bencejp, Stonecal, egfr` | 12 | Panel table shared by the single-analyte tests Creatinine / Uric Acid / Urea |
| `liver` | `sgpt, sgot, ASTALT, Alp, ggt, totbil, dirbil, indbil, tp, alb, glob, agratio, comment1` | 13 | Composite panel |
| `pt` | `bleedingm, bleedings, bleeding, clottingm, clottings, clotting, pt, concent, inr, ratios, ptt, fibro, fdp` | 13 | Coagulation panel; `pt`, `inr`, `ratios` are derivation outputs |
| `Electrolytes` | 17 serum/urine/CSF ion columns | 17 | Composite panel |
| `hormones` | `t3, t4, freet3, freet4, tsh, fti, t3up, pbi, ThyroidAuto, AntiMicrosomal, PTH, Calcitonin, Thyroglobulin` | 13 | Composite panel |
| `Lipids`, `iron`, `coagfactors`, `CRP`, `ABO`, `sugar` | 8, 6, 8, 8, 5, 9 respectively | — | Panels of varying width |

Confidence: VERIFIED for the table/column declarations in `v2019-Patients.mdb.sql`
(counted above; `cbc` has 29 columns in that dump). A prior derived inventory
(`branch-profile-schemas.csv`, cited in `09_CROSS_SOURCE_EVIDENCE/Verified_Findings.md` V-11)
reports narrower shapes — `cbc` 17 analytes, `crossmat` up to 81 columns — which likely
describes the branch template rather than the live mirror. The difference is recorded in
`09_CROSS_SOURCE_EVIDENCE/Conflicting_Findings.md`; neither count is asserted for the other
file. `Database_Business_Logic.md` §10 records **53 profile tables** of this shape —
STRONGLY_SUPPORTED.

### 2.1 Single-component tests are catalogue rows, not separate tables

`Serum Creatinine`, `Serum Uric Acid` and (by the panel column list) `Urea` are distinct
`testtable` entries, but their result values map into the **same `kidney` table** columns
`creat`, `uric`, `urea`. `ui-labels-verified.md` §6.4–6.5 shows them printed as individual
analytes with their own prose reference intervals (`Serum Creatinine` · "Normal : 0.7 -
1.3 mg/dL"; `Serum Uric Acid` · "Normal : 3.6 - 7.2 mg/dL" in the recorded walkthrough).
The screen evidence is from recorded frames, **not** from executing the binary.

This is the central nuance: **"simple" describes the ordering/result granularity, not the
physical storage**. Confidence: STRONGLY_SUPPORTED for the column mapping and the printed
labels; INFERRED for the exact per-test write path (the writing code was not recovered —
0 literal `INSERT`/`UPDATE` statements exist in the SQL corpus).

### 2.2 Composite panels are not processed uniformly

CBC, Liver Functions, Kidney Functions, PT and the microbiology cultures each have their
own table shape and their own report template; 29 cached RTF method sheets exist
(`report-structure.md`), with `056.rtf` (CBC, 16,953,057 bytes) carrying critical-level
text and four embedded images. `13-pdf-print-architecture.md` §4 records the hard-coded
restriction `testcode in (6,7,8,9,14,15,16,17,18,19,20,21)` for one print path — evidence
that **different test classes take different code paths**.

**How composite components are written, validated as a group, or printed as one block is
UNVERIFIED.** `MASTER_BUSINESS_LOGIC.md` §3 states this explicitly and it is preserved here:
the write path was not recovered, so no uniform "component" algorithm is asserted.

## 3. The template layer — `template-corpus-228.json`

| Measure | Value | Source / confidence |
|---|---|---|
| INI template files | **228** (117 base + `_S` variant identities) | `template-corpus-228.json`; `1605-template-corpus.md` — VERIFIED |
| Files joining to a result table | **72** (36 identities) | same — VERIFIED |
| Distinct joined profile tables | **36** | direct re-count of `template-corpus-228.json` `joined_table` — VERIFIED |
| Previously examined / newly examined | 119 / 109 | `1605-template-corpus.md` — VERIFIED |

Joined tables recovered directly from the JSON include `ABO, Abuse, anyreport, BILH,
BONEMAR, cardiac, cbc, CRP, Drugs, Electrolytes, ESR, Fertlity, Helicobacter, hepatitis,
hormones, IMMUNOLOGY, insulin, iron, kidney, Lipids, liver, pregnancy, pt, semen, stone,
stool, sugar, Suprarenal, TORCH, trace, Tumors, urine, Vitamins, widal, widal2, ZN`.
Notable: `Kidney.ini` → `kidney` (subtitle "Kidney Functions"), `LIVER.ini` → `liver`,
`CBC.ini` → `cbc` (subtitle "Blood Picture"), `PT.ini` → `pt` ("Coagulation Profile").

Two disjoint binding tiers are established in `1605-phase2-findings.md` U-12 and
`1605-template-corpus.md` §4: **alphabetic** template ids are result-table names;
**numeric** template ids are `testtable.testcode` values (54 of 55 numerically named
identities match a testcode; only `613` has no match). No alphabetic id is a testcode and
no numeric id is a table name. Confidence: STRONGLY_SUPPORTED.

## 4. Reference ranges and units apply to both classes

- `normalrange.csv`: **852 rows** across **304 test codes**; columns `testcode`, `sex`,
  `firstold`, `secondold`, `ageunit`, `pregsmok`, `normalrang`, `lowlimit`, `highlimit`,
  `nolowexe`, `nohighexe`, `lowcomment`, `highcomment`, `REFRANGE` (header verified).
  Sex distribution (prior report `04-clinical-rules.md` §3): Male 422 · Female 421 · All 5 · blank 4.
- `SHAPE … RELATE testcode TO testcode` at VA `672E3E` joins `testtable` to `normalrange`
  filtered by `normalrang <> ''`; per-analyte range lookup at VAs `68AB1E`–`68C3B6` joins
  `normalrange.testcode = testtable.testcode` and filters `normalrange.sex='…'`.
- `converts.csv` (167 rows: `testsimp, Funit, Sunit, Factors, Ffix, Sfix`) and
  `testunits.csv` (12 rows in the flat sample; prior reports cite 69 units in the full
  knowledge layer) provide per-analyte units and conversion factors.
- **Conflict preserved:** `1605-phase2-findings.md` NEW-3 reports the Access mirror holds
  1,152 `normalrange` rows with no `REFRANGE='2'`, while `normalrange.csv` holds 852. The
  852 figure is retained as the shipped-dump figure per the handoff; both are cited and the
  divergence is recorded in `09_CROSS_SOURCE_EVIDENCE/Conflicting_Findings.md`.

## 5. Derivation constants for panel outputs

`constanttable.csv` (columns `constantdiscrp, constant1…constant6`) holds:
`PTCONS` = `12, 35, 1.33, 2CVJW, 0, 0` and `CBCCONS` = `3.3, 6.25, 6.75, 7.5, 8.25, 0`;
a third row `RATIOS` is queried at VA `72F81C`. The manual-documented formulas
(`04-clinical-rules.md` §1) are `Hct = Hgb × 3.3`, sex/age-weighted `Hgb%` factors, PT
control 12/35 and ISI 1.33; the stored numbers match the manual exactly — a VERIFIED
numeric concordance. **Exact in-code derivation paths remain UNKNOWN.**

## 6. Prior knowledge preserved

| Prior statement | Source | Revalidated status |
|---|---|---|
| Simple tests Creatinine/Urea/Uric Acid; composite CBC | original stub + `MASTER_BUSINESS_LOGIC.md` §3 | STRONGLY_SUPPORTED (analytes exist; note shared `kidney` storage) |
| 609 tests, 48 subgroups | stub + `testtable-stats.json` | STRONGLY_SUPPORTED (subgroup count CONFLICTED with 46) |
| Profile tables carry their own analyte columns | stub + schema | VERIFIED |
| Exact representation differs per profile — do not assume uniform handling | stub + `MASTER_BUSINESS_LOGIC.md` §3 | VERIFIED (36 tables, 26-vs-5 columns) |
| Composite component storage/print UNVERIFIED | `MASTER_BUSINESS_LOGIC.md` §3 | Preserved — UNVERIFIED |

**Cross-references:** `Simple_And_Composite_Tests.md` is the companion to
`Results_Management.md` and `Printing_And_Reporting.md`.
