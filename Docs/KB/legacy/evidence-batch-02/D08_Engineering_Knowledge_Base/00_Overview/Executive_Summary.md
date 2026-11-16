# Executive Summary — RealLab v1.605 Engineering Knowledge Base

**Target version:** RealLab v1.605 ("RealLab CD 2019"), Clinical Laboratory Information System, *ais group corporation*
**Evidence basis:** `RealLab-Reverse-Engineering-Evidence-Batch-02.zip` (461 entries; 3 principal evidence families; 3 sequential stages completed)
**Status:** Evidence-traceable engineering reconstruction. Facts, inferences, hypotheses and unresolved matters are explicitly distinguished.

## 1. What RealLab is

RealLab is a **two-tier clinical laboratory information system (LIS)**: a rich 32-bit Windows desktop client written in **Visual Basic 6 (compiled to native code)**, backed by **SQL Server** as the system of record with a **local Access (Jet 4) mirror** for branch/offline work. It is **report- and data-entry-heavy**, Arabic (RTL) UI, with barcode support, serial-port analyser integration, SMS and e-mail notification, and a 66-DataReport print subsystem.

## 2. What is now known

- **Application:** `real lab system.exe` — PE32 i386 GUI, VB6 native (runtime build 9782, LCID 1033). 212 VB6 objects (132 forms + 66 DataReports + 9 modules + 3 classes), 8,026 methods. Per-domain `Result*` / `Report*` naming maps to clinical disciplines.
- **Versions:** two builds present — V-ROOT-1.00 (2013, 56 MB) and v1.605 (2020, 73 MB). 190 files differ between them; 168 are byte-identical.
- **Data layer:** SQL Server primary + 4 Access Jet 4 MDBs. Schema = master core (patientinfo, patienttest, testtable, normalrange, …) + 96 per-test profile result tables.
- **Catalogue:** 609 tests in 48 subgroups; 852 reference ranges (sex/age stratified); 591 priced; 605 delete-protected.
- **Reporting:** 66 DataReports, 228 cached RTF templates, 146 per-test INI configs, barcode fonts, Arabic fonts.
- **Integrations:** SMTP, smsmisr.com SMS (cleartext), serial analysers (BF6500), IE COM automation, possible third (MySQL) backend (hypothesis).
- **Security posture:** documented default `admin`/`admin`; hardcoded SQL `sa` password; cleartext operator passwords; no ASLR/DEP; 41 of 62 binaries unsigned; bulk-deletion maintenance tools; 41 consolidated findings (9 High).

## 3. What was corrected

- **Jet version:** Access MDBs are **Jet 4**, not Jet 3 (CON-001).
- **Native code:** the executable is **native**, not p-code (CON-004).
- **`patients` table** is the **operator account** table, not patient demographics (FIND-T2-003).
- **p-code/native and method-name gaps** are now explained by the prior investigation.

## 4. What remains unresolved

- grant1..grant12 permission bit meanings (UNR-002)
- seven IDPassword* values (UNR-003)
- two 32-char literals (UNR-004)
- accuntpw hashing (UNR-005)
- ~1,370 SELECT-to-backend mapping (UNR-008)
- 5 unnamed external components (UNR-009)
- V1.00 vs v1.605 object-count reconciliation (CON-002, provisional)

## 5. Evidence traceability

All statements are traceable via `08_Evidence_and_Traceability/` (Findings_Registry.csv, Evidence_Registry.csv, Relationship_Registry.csv, Contradiction_Registry.csv). Each finding cites `EVD-nnnnnn` IDs resolving to the original archive entries.<|end|>