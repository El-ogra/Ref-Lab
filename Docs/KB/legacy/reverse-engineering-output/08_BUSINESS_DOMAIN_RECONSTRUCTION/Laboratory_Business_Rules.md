# Laboratory Business Rules — Real Lab System 1.605

- Catalogue & grouping: 609 tests, 48 subgroups, hierarchical group→subgroup→test.
- Reference-range validation: 852 ranges, sex/age stratified.
- Derived values: PT/INR (PTCONS) and CBC (CBCCONS) constants, user-editable (U-035).
- Pricing: patient/lab/other-lab per test; send-out routing.
- Authorisation: grant1..grant12 + IDPassword* (coarse, undocumented).
- Multi-branch: Branch_Data template + branch column.
- Microbiology: cultures/cultureanti/culturesec; C&S subgroup.
- QC: Reportqc. Delete protection 605/609; active 94/609; 1 send-out. See Business_Rules.md.