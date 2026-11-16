# Relationships — RealLab v1.605 (Database)

## Key foreign-key-style relationships (inferred from column names + SQL)

- `patientinfo.patientid` → `patienttest.patientid` (orders)
- `patienttest.patientid` + `requestdate` → profile tables (`ABO`, `cbc`, …)
- `testtable.testcode` / `testsimp` → `normalrange.testcode`, `pricelist.testsimp`
- `grouptable.groupcode` → `subgrouptable` (groupcode)
- `subgrouptable.subgroupcode` → `testtable.subgroupcode`
- `testtable.subgroupcode` → profile tables (via subgroup name/abrev)
- `patients.patientusname` → `patientinfo.labid` / `mowzaf` (operator linkage, Class 2)

## Referential integrity

No declared FK constraints are evidenced in the exports; relationships are application-enforced via SQL strings (SHAPE/RELATE). SQL Server-side referential integrity is not evidenced.

## Cross-database relationships

- SQL Server `Patients` ↔ Access `Patients.mdb` (mirror)
- `Branch_Data.mdb` (0 rows) ↔ template for branch configuration
- `data.mdb` ↔ shared master data (testtable, normalrange, constanttable, pricelist)

See `04_Cross_Evidence_Integration/relationship_registry.csv` for the full artifact-level relationship register (15 entries).