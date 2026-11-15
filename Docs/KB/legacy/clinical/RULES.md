# Legacy Clinical Rules — V-ROOT-1.00 and V-CD-2019

**Purpose:** Curated clinical and laboratory business rules from the legacy systems.
**Scope:** Result classification, critical limits, reference ranges, calculation constants, thresholds.
**Audience:** Developers, clinicians.
**Version applicability:** Most rules are SHARED (identical in both versions); PDF/printing rules are V2 ONLY.

---

## 1. How to read this catalogue

- Rule IDs: `V1-BR-nnn` (V-ROOT-1.00) and `V2-BR-nnn` (V-CD-2019). A rule applying to both
  carries two identifiers; the applicability column records shared scope.
- Evidence types: ET-DOC, ET-DOC+DB, ET-DB, ET-CODE, ET-DDL, ET-CFG, ET-INST, ET-DERIVED, ET-PRES.
- Confidence: CONFIRMED · STRONGLY SUPPORTED · INFERRED · UNKNOWN · CONFLICTING.
- Status: CONFIRMED PRESENT · CONFIRMED ABSENT · VERSION-SPECIFIC · NOT ESTABLISHED · CONFLICTING.

**★ Absence of evidence is not evidence of absence.** Every CONFIRMED ABSENT entry names the
search that established it.

**Evidence:** external: Business_Rules_V1.md §1; `Docs/Business_Rules_V2.md` §1.

## 2. Rule index (clinical subset)

| ID | Rule name | Domain | Evidence type | Confidence | Status |
|---|---|---|---|---|---|
| V1-BR-009 / V2-BR-034 | Result classification is driven by the reference-range table | Results | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT (both) |
| V1-BR-010 / V2-BR-035 | A critical limit overrides the abnormal classification | Results / Clinical safety | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT (both) |
| V1-BR-011 / V2-BR-036 | Reference ranges are banded by sex and age | Clinical knowledge | ET-DOC+DB | CONFIRMED (banding); UNKNOWN (selection) | CONFIRMED PRESENT (both) |
| V1-BR-012 | The abnormality exception text is stored on the test row | Results | ET-DB | STRONGLY SUPPORTED | CONFIRMED PRESENT |
| V1-BR-013 | A result comment may be sourced from three distinct places | Results | ET-DOC | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-018 | Exception text and critical comment are stored per range row | Clinical knowledge | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-019 | The urine analysis's reference range is bound to immutable test number 4 | Clinical knowledge | ET-DOC | STRONGLY SUPPORTED | CONFIRMED PRESENT |
| V1-BR-035 | Critical thresholds are stated in the method sheet, separately from `normalrange` | Clinical knowledge | ET-PRES+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-037 / V2-BR-037 | Haematocrit is derived as `Hct = Hgb × 3.3` | Clinical | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-038 / V2-BR-037 | Haemoglobin percentage derived from `Hgb` and an age/sex factor | Clinical | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-039 / V2-BR-037 | ISI computed as `PT control / patient PT` with control time 12 s (secondary 35 s) | Clinical | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-040 / V2-BR-037 | Westgard `2CVJW` rejection rule applied to coagulation control values | Clinical | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT |
| V1-BR-041 | Per-analyte critical-value thresholds stated in method sheets | Clinical | ET-PRES | CONFIRMED | CONFIRMED PRESENT |
| V2-BR-038 | Only reviewed results are eligible for release | Results / Quality | ET-DOC+DB | CONFIRMED | CONFIRMED PRESENT (both) |

## 3. Mandatory clinical constants (`constanttable`)

| Group | Field | Value | Documented rule |
|---|---|---|---|
| `CBCCONS` | constant1 | **3.3** | `Hct = Hgb × 3.3` |
| `CBCCONS` | constant2 | **6.25** | `Hgb% = Hgb × 6.25` (male > 12 yr) |
| `CBCCONS` | constant3 | **6.75** | `Hgb% = Hgb × 6.75` (female > 12 yr) |
| `CBCCONS` | constant4 | **7.5** | `Hgb% = Hgb × 7.50` (1–12 yr) |
| `CBCCONS` | constant5 | **8.25** | `Hgb% = Hgb × 8.25` (< 1 yr) |
| `PTCONS` | constant1 | **12** | PT Control Time |
| `PTCONS` | constant2 | **35** | PT Control Time (secondary) |
| `PTCONS` | constant3 | **1.33** | **ISI** |
| `PTCONS` | constant4 | **2CVJW** | Westgard 2CV rejection rule |

**★ VERIFIED FACT:** exact quantitative concordance between the official manual and the values stored in the
database. This closed the "CAP-13 without code evidence" gap from BATCH-01.

**Version applicability:** SHARED (identical values in both versions).

**Evidence:** external: 04-clinical-rules.md §1; external: Business_Rules_V1.md V1-BR-037..040; `Docs/Business_Rules_V2.md` V2-BR-037.

## 4. PT control curve (PTC) — 33 points

`control` (seconds) ↔ `concent` (concentration): 12→100, 12.5→92, 12.7→88, 13→84, 13.3→80,
13.7→76, 14→72, 14.4→68, 14.8→64, 15.6→60, 16.2→56, …

**Evidence:** external: 04-clinical-rules.md §2.

## 5. Reference ranges (`normalrange`) — 852 ranges

- Across **304** tests.
- Sex distribution: Male 422 / Female 421 / All 5 / empty 4.
- Age unit: Years 708 / Days 70 / Mons. 65 / empty 9.
- Age span: Years 0–120, Days 1–120, Mons. 0–24.
- Numeric low limit: 459 rows; high limit: 643 rows.
- Exception text: 511 low / 581 high.
- Comments: 19 distinct low / 19 distinct high.
- **51 distinct measurement units.**
- `pregsmok` populated on all 852 rows (OBSERVED, not documented).

**Rule (V1-BR-009 / V2-BR-034):** classification is driven by `normalrange` limits, not
hard-coded values. An entered value is compared against the low/high limits held in `normalrange`
for the analyte. The limits are data, administered through the «المعدل الطبيعي» editor.

**Critical limit (V1-BR-010 / V2-BR-035):** three-valued ordered classification:
normal → abnormal (high/low) → critical. A value breaching the critical limit is presented as
critical. **★ No escalation workflow is evidenced** — no callback, no repeat, no critical-result
log separate from the report. The critical state is visual only. For a laboratory this is the
single most important functional gap in the system.

**Banding (V1-BR-011 / V2-BR-036):** each `normalrange` row scoped by `Sex` (Male/Female/All/empty),
age window (`Age From`, `Age To`), and age unit (Years/Days/Mons.). Only 5 of 852 rows are `All`
and 4 carry empty sex — so a patient whose sex is unrecorded may match only those 9 rows.

**★ Selection algorithm UNKNOWN:** the runtime selector emits
`SELECT Top 1 * FROM normalrange WHERE testcode=<n> AND sex='<s>' AND ageunit='<u>'
AND firstold < <age> AND secondold >= <age> AND pregsmok=<p>` with **no ORDER BY**. Selection at
an overlapping boundary is engine-arbitrary. (1605-phase2 U-02, CONFIRMED.)

**Range freeze:** `patienttest` carries a frozen copy of the range at order time (`normalrang`,
`lowlimit`, `highlimit`, `nolowexe`, `nohighexe`) — 112 of 222 rows carry `normalrang`. A range
change does not affect already-ordered tests.

**Version applicability:** SHARED (852 rows identical in both versions).

**Evidence:** external: 04-clinical-rules.md §3; external: Business_Rules_V1.md V1-BR-009/010/011;
`Docs/Business_Rules_V2.md` V2-BR-034/035/036; external: 1605-phase2-findings.md U-01..U-03.

## 6. Unit conversions (`converts`) — 167 rows

Sample conversions: Acetaminophen µg/mL → µmol/L (6.62); Acetone mg/dL → mmol/L (0.172);
Albumin g/dL → g/L (10); Aldosterone ng/dL → nmol/L (0.0277); Ammonia µg/dL → µmol/L (0.587).

Stored per analyte-and-unit-pair, not globally (V1-BR-029).

**Evidence:** external: 04-clinical-rules.md §5.

## 7. Critical-value thresholds (from `Cache\*.rtf`)

| Test | Thresholds |
|---|---|
| CBC (056.rtf) | Hgb < 5 or > 20 g/dL · Hct < 15% or > 60% · WBC < 500 or > 50,000/mm³ · Platelets < 50,000 or > 999,000/mm³ |
| BUN/Urea (013.rtf) | > 40 mg/dL (no dehydration/renal history) · > 100 mg/dL (renal history) · > 20 mg/dL rise in 24 h (acute renal failure) |
| Bilirubin (015.rtf) | Newborn > 15 mg/dL · SI > 257 µmol/L |
| Ionised Calcium (175.rtf) | > 6.29 mg/dL (SI > 1.57) · < 3.1 mg/dL (SI < 0.78) |
| Total Calcium (176.rtf) | > 12 mg/dL (SI > 2.99 — coma/death) · < 7 mg/dL (SI < 1.75 — tetany/death) |
| Sodium (180.rtf) | < 80 mEq/L · > 115 mEq/L |

Critical thresholds are stated in the method sheet, **separately** from `normalrange` (V1-BR-035).
The exact flag precedence when a value breaches both limits is not established.

**Evidence:** external: 04-clinical-rules.md §4.

## 8. Comment sources (V1-BR-013)

A result comment may be sourced from three places: typed by the operator, selected from the stored
`comments` table (12 rows), or taken from the normal-ranges list via the orange comment button.

## 9. Reference-range administration (V1-BR-009..011, V1-BR-018/019)

- Ranges are banded by sex and age (Male/Female/All/empty; Years/Days/Mons.).
- Exception text and critical comment stored per range row (19 distinct low / 19 distinct high).
- The urine analysis (test number 4) is immutable in both number and name (V1-BR-016, V2-BR-043).
- `normalrange` is the system's principal safety-critical knowledge asset.

## 10. Result-entry interface

- Profile-scoped entry; automatic abnormal/critical classification.
- `patienttest.resultreview` — explicit Boolean review flag (release gate).
- New profile windows in V2: IgE allergy (231 methods), H. pylori (83), PCR (37), LBC (33),
  FMF (48), automated urine (184).
- `IgEallergy` item model: `Item1..Item40+` each with `RESULT`, `Class`, `TypeF` — a
  repeatable-group pattern, not a flat analyte list.
- Result lifecycle: `resultfinsh` (entry complete) → `resultreview` (reviewed/approved) →
  `resultprint` (printed) → `give` (handed over) → `sentout`/`send` (sent out).

## 11. Version applicability

- Clinical constants, reference ranges, classification model, comment sources, critical thresholds:
  **SHARED** (identical in both versions).
- IgE allergy, H. pylori, PCR, LBC, FMF, automated urine profile windows: **V2 ONLY**.
- `resultreview` as explicit field: **V2** (V1 had the storage field not isolated; V2 made it explicit).

## 12. Related documents

- `Docs/KB/legacy/clinical/KNOWLEDGE-LAYER.md` — knowledge tables.
- `Docs/KB/legacy/clinical/PROFILES.md` — profile tables and template binding.
- `Docs/KB/legacy/data/MODEL.md` — data model.
- `Docs/KB/legacy/business/WORKFLOWS.md` — workflows.
- Original source evidence: external: 04-clinical-rules.md, external: Business_Rules_V1.md, `Docs/Business_Rules_V2.md`,
  external: Complete_Reverse_Engineering_Reference_V1.md, `Docs/Complete_Reverse_Engineering_Reference_V2.md`.
- `Database documentation.md` (root).
- Modern implementation: `src/RefLab.Domain/Rules/*`, `src/RefLab.Application/Features/Results/*`.
