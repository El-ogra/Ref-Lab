# Legacy Clinical Knowledge Layer — V-ROOT-1.00 and V-CD-2019

**Purpose:** Curated reference for the clinical knowledge tables.
**Scope:** Tests, ranges, constants, units, conversions, abbreviations, sub-groups.
**Audience:** Developers.
**Version applicability:** SHARED (identical in both versions).

---

## 1. Test catalogue (`testtable`) — 609 rows, 37 columns

- Test name, abbreviation, price, profile, send-out flag.
- `testcode` is the key (and the name of the report template for tests with their own report:
  54 of 55 numerically named template identities are exactly a `testtable.testcode` value).
- `REFRANGE` per-test reference-range mode: '' 538, '0' 46, '1' 23, '3' 2.
- `seereport`: 48 rows say "See Report".
- `printwith`, `normaltype` (empty for all 609 rows).
- `inotherlab` flag enrols a test into the send-out workflow (V1-BR-017, V2-BR-044).
- Test number 4 (urine analysis) is immutable in both number and name (V1-BR-016, V2-BR-043).

**Evidence:** external: 04-clinical-rules.md; external: 1605-phase2-findings.md (U-04, U-05).

## 2. Reference ranges (`normalrange`) — 852 rows, 18 columns

See `clinical/RULES.md` §5 for full distribution and banding.

Per range row: test name, test unit, sex, age from/to, age unit, normal range, low/high limit,
low/high flag, low/high comment, critical comment, critical range, critical low/high limit,
critical flag, `pregsmok`.

## 3. Clinical constants (`constanttable`) — 9 rows, 7 columns

Two groups: `CBCCONS` (5 values) and `PTCONS` (4 values). Stored as Text(50) so that a
non-numeric token such as `2CVJW` can coexist with numbers. See `clinical/RULES.md` §3.

## 4. Units (`testunits`) — 69 rows, 2 columns

Unit dictionary.

## 5. Conversions (`converts`) — 167 rows, 6 columns

Unit conversion factors, stored per analyte-and-unit-pair (not globally). See `clinical/RULES.md` §6.

## 6. Abbreviations (`abbreviations`) — 650 rows, 3 columns

Abbreviation dictionary, maintained independently of the test catalogue (V1-BR-030).

## 7. Sub-groups (`subgrouptable`) — 46 rows, 4 columns

Clinical sub-group catalogue. The clinical sub-group is the routing key for both the entry
window and the report layout (V1-BR-031, INFERRED). 46 sub-groups exist while 223 `.ini`
layout files exist; the mapping is many-to-one and is not established (RL-V1-Q-07).

## 8. PT control (`PTC`) — 33 rows, 2 columns

PT control curve: `control` (seconds) ↔ `concent` (concentration). See `clinical/RULES.md` §4.

## 9. Antibiotics (V2)

- `antibiotic`: 38 rows, 25 columns (symbol, scientific name, culture type, common names,
  pregnancy/children allowed, sensitivity, sensitivity unit).
- `antibio`: 96 rows, 2 columns (antibiotic scenario).
- `cultures`: 25 columns (culture results, organisms, condition, colony, reactions,
  susceptibilities).
- `cultureanti`: sensitivity (changed Double → Text(10) in V2 — sensitivity became textual).
- V2 adds a **third organism** (`organism3`) to `cultures` (cross-confirmed across `.ini` templates,
  SQL schema, and embedded DDL).

**Evidence:** external: 11-deployment-payload-2019.md §2.4.

## 10. Custom profiles

- `customtesttable`: 54 rows, 5 columns.
- `customgrouptable`: 6 rows, 3 columns.

## 11. Catalogue interlock

See business/WORKFLOWS.md section 7 (WF-06) for the catalogue interlock diagram.


**Evidence:** external: Complete_Reverse_Engineering_Reference_V1.md §4 (WF-V1-06).

## 12. Version applicability

All tables and counts listed here are **SHARED** (identical in both versions). V2 adds new
tables (see `data/MODEL.md` §4) but does not change the knowledge layer.

## 13. Related documents

- `Docs/KB/legacy/clinical/RULES.md` — clinical rules.
- `Docs/KB/legacy/clinical/PROFILES.md` — profile tables and template binding.
- `Docs/KB/legacy/data/MODEL.md` — data model.
- Original source evidence: external: 02-data-model.md, external: 04-clinical-rules.md,
  external: Complete_Reverse_Engineering_Reference_V1.md, `Docs/Complete_Reverse_Engineering_Reference_V2.md`.
- `Database documentation.md` (root).
- Modern implementation: `src/RefLab.Domain/ClinicalKnowledge/*`, `src/RefLab.Infrastructure/Persistence/Configurations/ClinicalKnowledgeConfigurations.cs`.
