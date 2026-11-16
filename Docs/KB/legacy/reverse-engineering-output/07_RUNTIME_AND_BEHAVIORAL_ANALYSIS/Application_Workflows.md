# Application Workflows — Real Lab System 1.605

1. Login & authorisation (grant1..grant12 + IDPassword* print passwords).
2. Patient registration (patientinfo).
3. Test ordering (testtable → patienttest).
4. Result entry per discipline (Result* → profile table).
5. Validation against normalrange (sex/age).
6. Derivation (PT/INR, CBC via constants).
7. Pricing & send-out (inotherlab, otherlabprice).
8. Reporting (Report* DataReport + RTF + INI + barcode).
9. Notification (SMS via smsmisr.com, e-mail via SMTP).
10. Multi-branch sync (Branch_Data template).
11. Backup (.bat/.sql to configurable server/path).
12. Startup schema migration (DDL). See Workflows.md.