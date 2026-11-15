# Legacy Profile Tables and Report Template Binding

**Purpose:** Curated reference for the 46 clinical profile result tables and their report-template binding.
**Scope:** Profile table inventory, analyte patterns, template binding rules.
**Audience:** Developers.
**Version applicability:** SHARED (V2 adds `hba1c`, `ratios`, `widal2`, and code-referenced `insulin`, `IgEallergy`, `culturesec`; `FMF` and `LBC` are DDL-created but absent from shipped DB).

---

## 1. Profile result tables (46 in V1)

Each profile table carries the analyte set for one clinical discipline. Representative members with
column counts:

| Profile | Columns | Analyte examples |
|---|---:|---|
| `crossmat` | 84 | `patabo`, `patrh`, `donname1..8` |
| `semen` | 59 | `collection`, `volume`, `Consistency`, `Liquefaction`, `Viscosity` |
| `urine` / `stool` | 58 | `t01..t55` (semi-structured analysis) |
| `prostaticx` | 45 | `wbc11..32`, `rbc11..32`, `Trichom`, `Monilia` |
| `GLUCOSETOLERANCE` | 25 | `FBG`, `FBGG`, `FBGA`, `bg30`, `bg30g`, `bg30a`, `bg1..bg15` |
| `cultures` | 25 | `cultid`, `organism`, `organism2`, `condition`, `colony`, `reactions` |
| `trace` | 25 | `sAluminium`/`uAluminium` … (serum/urine pairs) |
| `BONEMAR` | 23 | `T01..T20` |
| `CSFT` | 22 | `materi`, `color`, `aspect`, `deposit`, `Viscos`, `PH`, `SG`, `gluc`, `prot` |
| `Electrolytes` | 20 | `scatot`, `scaion`, `sph`, `una`, `uk`, `uph`, `csfmg` |
| `cbc` | 20 | `hgb`, `rbc`, `hct`, `plt`, `hgbper`, `mcv`, `mch`, `mchc`, `pct`, `mpv`, `wbc`, `neut`, `lymph`, `mono`, `eosino`, `baso` |
| `hepatitis` | 20 | `navigm`, `havigg`, `hbsag`, `hbsab`, `hbeag`, `hbcigm`, `hcvigg`, `hdvtotal` |
| `IMMUNOLOGY` | 19 | `ANA`, `DNA`, `LKM`, `SSA`, `SSB`, `AntSmith`, `ANCA`, `RNP`, `ASMA`, `AMA` |

Full catalogue and per-table field detail: `Data_Model_and_Workflow_Entities_V1.md` §3 (external)
and `Docs/Data_Model_and_Workflow_Entities_V2.md`.

## 2. V2 profile changes

V2 keeps all V1 profile tables and adds (via DDL or base schema):
- `hba1c` (HbA1c) — present (DDL-created).
- `ratios` — present (DDL-created).
- `widal2` — present (DDL-created).
- `FMF` — **ABSENT** (DDL-created but not in shipped DB).
- `LBC` — **ABSENT** (DDL-created but not in shipped DB).
- `IgEallergy` — **ABSENT** (code + DDL referenced, never present). Repeatable-group pattern
  (`Item1..Item40+`, each with `RESULT`, `Class`, `TypeF`); DDL adds 4 items at a time.
- `insulin` — present (ships in base schema, not DDL-created).
- `culturesec` — code-referenced.

**Evidence:** `Docs/Complete_Reverse_Engineering_Reference_V2.md` §6.6–6.7.

## 3. Report template binding

Two disjoint tiers (1605-phase2 U-12, CONFIRMED):

1. **Alphabetic template ids** are result-table names: 36 of 62 alphabetic identities are
   exactly a table in the live `Patients.mdb` (ABO, CBC, CRP, HEPATITIS, IMMUNOLOGY, …); a fuzzy
   match resolves 6 more (BLOODGAS→BloodGases, CSF→CSFT, CROSS→crossmat, HBELECTRO→Hgbectrophoresis,
   IMMUNOGLOB→Immunoglobulin, PROTELECTRO→Proelectrophoresis), giving 42 of 50 result tables
   that have a template.
2. **Numeric template ids** are `testtable.testcode` values: 54 of 55.

No alphabetic id is a testcode and no numeric id is a table name — the tiers never overlap.

**Total:** 228 INI files forming 117 template identities (a base template plus a `_S` variant).

**Evidence:** external: 1605-phase2-findings.md (U-05, U-12, NEW-2).

## 4. Per-template printer override

Each template carries `[setupx] reportprinterx`. Across all 228 files:
- `HP LaserJet 1100 (MS)` in 217.
- `HP LaserJet 1100` in 8.
- `HP LaserJet P1005` in 1.
- Absent in 2.

Effectively a single legacy default baked into the templates, not a per-discipline choice.

**Evidence:** external: 1605-phase2-findings.md (NEW-5).

## 5. Reference-range labels (per-template)

The reference-range heading is per-template, not global. Across 228 templates the `normal` key
takes 12 distinct spellings, including 'Reference Range :' (161), 'Reference Range:' (19),
'Reference Range' (11), 'Ref Range (Class):' (6), 'Evaluation Criteria :' (6),
'Interpretation of Viraemia:' (2), 'Viral Screen :' (2), 'Normal' (2),
'Ref. value according to WHO' (2), 'Nolrmals according to WHO' (2 — vendor typo), 'Normal Range:' (1),
blank (14). The comment label has 4 spellings.

**Evidence:** external: 1605-phase2-findings.md (NEW-4).

## 6. Template analyte keys

The 109 numerically named (discipline-level) templates are composite report layouts:
- 18 microbiology culture-and-sensitivity reports (100–117, 325–338 result-item keys each).
- 4 IgE allergy panels (164–167, 560–561 keys).
- 16 molecular-biology panels (450–465).
- Plus Coagulation Profile (198), Autoimmune diseases (220), HbA1C (400), Liquid Based Pap (480)
  and smaller sets.

**Evidence:** external: 1605-phase2-findings.md (NEW-2).

## 7. V2 third-organism change (cross-confirmed)

Three independent layers agree on the "third organism" feature:
- `.ini` templates (32 files): `LabelX002=Organism C`, `ITEMXV002`, `ITEMXD2=Gram Positive Cocci##…`,
  `Check6f=1`.
- SQL schema: `cultures.organism3` Text(80); `organism`/`organism2` → Text(80) (was 50).
- Embedded DDL: `ALTER TABLE cultures ALTER COLUMN organism3 → varchar(80)`.

**★ STRONGLY SUPPORTED:** a single feature confirmed across three independent layers — templates,
schema, and migration. This is the strongest evidence that the three layers are maintained
consistently.

**Evidence:** external: 11-deployment-payload-2019.md §2.4, §9.

## 8. Version applicability

- Profile tables and binding mechanism: **SHARED** (V2 extends with new profile windows and tables).
- `IgEallergy`, `FMF`, `LBC`, `insulin`, `culturesec`, `hba1c`, `ratios`, `widal2`: **V2 ONLY**
  (some storage-absent).

## 9. Related documents

- `Docs/KB/legacy/clinical/RULES.md` — clinical rules.
- `Docs/KB/legacy/clinical/KNOWLEDGE-LAYER.md` — knowledge tables.
- `Docs/KB/legacy/architecture/PDF-PRINTING.md` — printing.
- `Docs/KB/legacy/data/MODEL.md` — data model.
- Original source evidence: external: 1605-phase2-findings.md, external: 11-deployment-payload-2019.md,
  `Docs/Complete_Reverse_Engineering_Reference_V2.md` §6.6–6.7, §7.
- Modern implementation: `src/RefLab.Application/Features/Reporting/*`, `src/RefLab.Application/Features/Results/*`.
