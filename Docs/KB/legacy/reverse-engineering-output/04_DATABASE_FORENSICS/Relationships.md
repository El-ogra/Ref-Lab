# Relationships — Real Lab System 1.605

Inferred (no declared FKs evidenced; application-enforced via SHAPE/RELATE):
- patientinfo.patientid → patienttest.patientid
- patienttest.patientid+requestdate → profile tables
- testtable.testcode/testsimp → normalrange.testcode, pricelist.testsimp
- grouptable.groupcode → subgrouptable; subgrouptable.subgroupcode → testtable.subgroupcode
- patients.patientusname → patientinfo.labid/mowzaf (operator linkage)
Cross-DB: SQL Server Patients ↔ Access mirror; Branch_Data.mdb (0 rows, template); data.mdb (shared masters).