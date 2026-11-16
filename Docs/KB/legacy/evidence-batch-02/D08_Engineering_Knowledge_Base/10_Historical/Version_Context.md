# Version Context — RealLab v1.605

## Two shipped versions

- **V-ROOT-1.00** (2013): origin build; 59,150,336-byte main exe (FileVersion 1.00, Company RealLab); 169 VB6 objects / 163 forms; 49 shared Access tables.
- **V-CD-2019 / v1.605** (2020): target of this reconstruction; 76,292,096-byte main exe; 212 VB6 objects / 198 forms; 93 Access tables (44 added beyond the shared 49).

## Cross-version delta (837 file relationships)

- `RELOCATED_IDENTICAL`: 41
- `REL_IDENTICAL`: 127
- `REL_DIFFERENT`: 190 (files changed between versions)
- `NO_PARTNER_IN_OTHER_VERSION`: 479

190 files differ; 168 are byte-identical across versions. Most change is in the clinical data layer and report templates.

## Schema evolution (V1 → V2019)

103 table/column deltas; new tables `FMF`, `Helicobacter`; new columns `Abuse.tramadol/ecstasy`, `BloodGases.pO2AA1/ctCO2`, etc.

## Historical findings preserved

The prior investigation's findings (F-01..F-09, FND-F-001..F-011, U-001..U-041) are preserved in `03_Independent_Analysis/Evidence_Type_3/` and the registries; they are not renumbered or erased. Historical versions (V-ROOT-1.00) are distinguished from the current target (v1.605) throughout.