# MASTER_BUSINESS_LOGIC.md — Real Lab System 1.605

Unified business logic reference. See MASTER_SYSTEM_KNOWLEDGE.md §5 for full detail.

Key rules:
- Catalogue: 609 tests, 48 subgroups, hierarchical group→subgroup→test.
- Reference ranges: 852, sex/age stratified (M 422/F 421; Years 708/Days 70/Mons 65).
- Derived values: PT/INR (PTCONS: 12,35,1.33,2CVJW) and CBC (CBCCONS: 3.3,6.25,6.75,7.5,8.25) constants, user-editable (U-035).
- Pricing: patient/lab/other-lab per test; send-out routing (inotherlab, otherlabprice).
- Authorisation: grant1..grant12 per operator + per-section IDPassword* (coarse, undocumented).
- Multi-branch: Branch_Data.mdb template + branch column.
- Microbiology: cultures/cultureanti/culturesec; CULTURE AND SENSITIVITY subgroup.
- QC: Reportqc. Delete protection 605/609; active 94/609; 1 send-out test.
- Simple vs composite tests: per-profile analyte columns.
- Result states: entered → validated → printed → finished.
- Workflows: login → registration → ordering → result entry → validation → derivation → pricing → report → notification → sync → backup. Startup DDL migration.