# Legacy Versions — V-ROOT-1.00 vs V-CD-2019 (v1.605)

**Purpose:** Preserve the distinction between the two legacy systems that Ref Lab replaces.
**Scope:** Identity, version-specific characteristics, shared characteristics, deltas, and applicability rules.
**Audience:** Developers, architects, analysts.

---

## 1. Identity

| Attribute | V-ROOT-1.00 | V-CD-2019 (v1.605) |
|---|---|---|
| Application | `real lab system` | `real lab system` (distribution folder `RL_CD2019Access`) |
| Main executable | `real lab system.exe`, 59,150,336 bytes (56.41 MiB) | `real lab system.exe`, 76,292,096 bytes (72.76 MiB) |
| File version | 1.00 | 1.605 |
| Product description (PE) | (in-app help) "real lab system version 1.0" | `RLS 1.605` |
| Company name (PE) | RealLab | RealLab |
| Build | PE32, 3 sections, linker 6.0, timestamp 2013-12-27 | PE32, 3 sections, linker 6.0, timestamp 2013-12-27 |
| Runtime | `MSVBVM60.DLL` (186 imported functions) | `MSVBVM60.DLL` (231 imported functions) |
| Installer | (none in V1 artefact set) | `RL_SETUP.exe` v19.00, build 2020-09-01, 23,060,480 bytes |
| Installation media | (none) | `WIN7\` and `WINXP\` targets (CAB + SETUP.LST) |
| Official learning guide | `real lab system help.chm` (15 topics) | `RLS_Learn.pdf` (212 pp.), `RLS_Setup.pdf` (33 pp.) |
| Vendor identity | RealLab / AIS ("ais group"), El-Mansoura, Egypt | RealLab / AIS ("ais group"), El-Mansoura, Egypt |
| Second executable | `real lab Image Gallery.exe` (10.22 MB, 2010-06-30) | (same, plus `Patient Data Transport.exe`, `RLUTool.exe`, etc.) |

**★ Note on the build date.** The PE timestamp is 2013-12-27 in **both** versions. The PE timestamp
is **not** a version discriminator. V2's product is materially newer (installer 2020-09-01,
guide 2017-04-26, payload 2019–2020) but the executable was not recompiled after the shared
build base.

**Evidence:** external: Complete_Reverse_Engineering_Reference_V1.md §1.1; `Docs/Complete_Reverse_Engineering_Reference_V2.md` §1.1.

## 2. Version applicability classification

Every fact in this knowledge base must carry one of:

| Code | Meaning |
|---|---|
| V1 ONLY | Established for V-ROOT-1.00 only. |
| V2 ONLY | Established for V-CD-2019 only. |
| SHARED | Established for both versions. |
| VERSION-DEPENDENT | Applies to both but behaves differently. |
| UNKNOWN | Not established for either version. |

**Rules:**
- Never assume a V2 observation applies to V1.
- Never assume a V1 finding applies to V2.
- Where a fact is SHARED, both versions must be cited.

## 3. Shared characteristics (SHARED)

- Same vendor (RealLab / AIS), same product name, same help.chm (byte-identical, SHA-256 `62431…`).
- Same PE timestamp (2013-12-27), same linker 6.0, same `MSVBVM60.DLL` family.
- Same database engine set (Access Jet 4 default; SQL Server Express; MySQL).
- Same default `sa`/`12345678` credential model.
- Same `Branch_Data.mdb` template (byte-identical in `bin\` in both versions).
- Same clinical knowledge layer: 609 tests, 852 reference ranges, identical constants (`CBCCONS`, `PTCONS`), 69 units, 167 conversions, 650 abbreviations, 46 sub-groups.
- Same 29 `Cache\*.rtf` method sheets (byte-identical).
- Same 13 `Cache\*.exe` Flash training simulators (byte-identical).
- Same core tables (`patientinfo`, `patienttest`, `testtable`, `normalrange`, `payment`, `Gehats`, `history`, `constanttable`, `converts`, `abbreviations`, `subgrouptable`, `testunits`).
- Same result-classification model (normal → abnormal → critical, driven by `normalrange`).
- Same keyboard shortcuts (F8 confirm, F9 save, F11 preview, F12 print, Enter next-and-save, Esc cancel, F7 accounting/send-out).
- Same report numbering origin (`SETTING.ini [setup] reportstart=1984`).
- Same `patientid` positional identifier mechanism (person key at positions 3–4; display code at 10–12). V2 adds branch at position 9.
- Same `history` 20 free-form column design.
- Same `normalrange.pregsmok` populated on all 852 rows (OBSERVED, not documented).

**Evidence:** external: Complete_Reverse_Engineering_Reference_V1.md §6.6, §6.7, §10; `Docs/Complete_Reverse_Engineering_Reference_V2.md` §6.8, §6.10.

## 4. V2-only characteristics (V2 ONLY)

- **Schema self-migration:** 268 embedded DDL statements (**27 CREATE TABLE** → 14 distinct tables, **228 ALTER ADD**, **13 ALTER COLUMN**) applied at start-up. V1 has zero DDL.
  *(Corrected 2026-11-16: the earlier text read “12 CREATE TABLE”, a statement-row count, not a statement-class count; 27 + 228 + 13 = 268. See `Docs/KB/legacy/evidence/CONTRADICTIONS.md` C-16.)*
- **Multi-branch model:** master laboratory + reception branches; branch code at position 9 of `patientid`; `Labbranch`, `Patient.branche`, `Branch_Data.mdb` seeding.
- **Digital delivery:** NATIGH.COM result delivery + SMS subsystem (`SMSSender`, `SMStemplate`, `smsgroups`, `smslist`, `SMSfrmsenderos`, `ReportSMS`, `Natigh` installed file).
- **PDF export:** via external `Bullzip PDF Printer` (name and version 11.1 hardcoded). Four output categories (`Contracts`, `Free`, `Individual`, `LabToLab`). V1 has zero PDF capability.
- **Statistics and performance monitoring:** `statisReport`, `loger` (attendance).
- **Work sheets:** `workpaper`, `workpaper2`, `wasl`.
- **Farm (microbiology) reporting:** `sharekat`, `sharekathesab`, `sarfz`, `antibiotic` (38 rows/25 cols), `antibio` (96 rows).
- **Antibiotic catalogue:** `antibiotic`, `antibio`, `cultures`.
- **Sample splitting/withdrawal:** `cultures`, `cultureanti`, `sample`.
- **Default account type:** `pricemethod` (value domain NOT ESTABLISHED).
- **Connection settings (receipt header/footer):** `IDCARD.INI`, `SETTING.ini`.
- **Payment-method selection:** `payment` type column, `pricemethod`.
- **Extended permission model:** 17 bits (`grant13..grant17`) + `branche` + `SecWord` on the operator table (V1 had 12 bits).
- **Separate `loger` event table** (V1 recorded attendance on the operator record).
- **New profile windows:** IgE allergy (231 methods), H. pylori (83), PCR (37), LBC (33), FMF (48), automated urine (184).
- **New tables:** `Helicobacter`, `insulin`, `IgEallergy`, `patienttestgroup`, `SMSSender`, `SMStemplate`, `smsgroups`, `smslist`, `MyLabSetting`, `testtemp`, `Paste Errors`.
- **Extended `patienttest`:** `Timeout`, `Taketime`, `natighu` (DDL-added, absent from shipped DB).
- **Extended `patientinfo`:** `gehaname` (referring-party name), `branch`.
- **Extended `Gehats`:** `ReferredPW`, `ReferredID`.
- **`useraccounts` table** (SQL Server only, not in any `.mdb`): `Gtype`, `HDD1`, `HDD2`.
- **`constanttable.constant7`** (new column).
- **`cultures.organism3`** (third organism) + `condition2`.
- **`cultureanti.Sensitivityvalue`** changed Double → Text(10).
- **6 new printer roles:** `waslprinter`, `IDprinter` (V2 only); `zarfprinter` exists in both.
- **14 new DataReport objects** (45 → 66).
- **`pricelistsN`** (V2 only).
- **Specialty reports:** `ReportPCR`, `ReportIGE1`, `ReportFMF`, `ReportLBC`, `ReporturineAuto`, `ReportZN`, `ReportTestTime`, `ReportPatPhone`, `Reportliver7his`, `Reportmony01/11/5`.
- **Patient ID card:** `CardFrm`, `CardFrmSetx`.
- **Patient visit report:** `RPTPatientVisit`.
- **SMS report:** `ReportSMS`.
- **Installer `RL_SETUP.exe` v19.00** (VB6, hardware-bound licence, 57 objects, 599 methods).
- **Installation media** `WIN7\`/`WINXP\` (CAB + SETUP.LST, VB6 SP6 redistributable, MDAC).
- **`NOTEX.dll` v2.00** ("RLS Notes 2.0"); `NOTEXS.dll` removed.
- **`lastlog` section in `BASE.ini`** (logs last 10 connection attempts including password).

**Evidence:** `Docs/Complete_Reverse_Engineering_Reference_V2.md` §3.3–3.6, §6.3–6.7, §7.1; external: 09-vendor-capability-catalogue.md; external: 11-deployment-payload-2019.md.

## 5. Material deltas (V1 → V2)

| Layer | V1 | V2 | Delta |
|---|---:|---:|---:|
| `Patients.mdb` tables | 93 | 103 | +10 |
| `Patients.mdb` columns | 1,527 | 1,682 | +155 |
| `Patients.mdb` rows | 3,123 | **3,809** | +686 |
| VB6 objects | 169 | 210 | +41 |
| Declared methods | 6,528 | 8,026 | +1,498 |
| String literals | 104,997 | 270,166 | +165,169 |
| SQL statements | 1,337 | 1,797 | +460 |
| DataReport objects | 45 | 66 | +21 |
| Forms | 118 | 132 | +14 |
| Modules | 5 | 9 | +4 |
| Permission bits | 12 | 17 | +5 |
| VB header `wFormCount` | 163 | **198** | +35 |
| VB header external components | 21 | **25** | +4 |
| VB header `dwLCID` | `0x00000409` (en-US only) | **1033 / 4105 (ar-SA)** | localised |
| PE timestamp | 2013-12-27 | 2013-12-27 | **identical — not a discriminator** |
| Embedded DDL | 0 | 268 | +268 |
| `Data\*.ini` files | 223 | 223 | **172 changed, 51 identical** |
| `Cache\*.rtf` | 29 | 29 | 0 changed (byte-identical) |
| `Cache\*.exe` | 13 | 13 | 0 changed (byte-identical) |
| `help.chm` | **17 topics** (15 documented capabilities + copyright page) | 17 topics | byte-identical |
| `Branch_Data.mdb` (bin) | 970,752 B | 970,752 B | byte-identical |

**Architectural conclusion (STRONGLY SUPPORTED):** RealLab evolved the *data and workflow* layer only
(+41 objects, +1,498 methods, +10 tables, +155 columns, full SMS/NATIGH subsystem, permission model,
branch model) while leaving the *runtime, documentation, and training* frozen at 2010–2011.

**Evidence:** external: 11-deployment-payload-2019.md §1; `Docs/Complete_Reverse_Engineering_Reference_V2.md` §1.4, §6.2.

## 6. Version-specific conflicts and asymmetries

| # | Item | V1 | V2 | Status |
|---|---|---|---|---|
| 1 | PE timestamp | 2013-12-27 | 2013-12-27 (same) | CONFIRMED (not a discriminator) |
| 2 | `Branch_Data.mdb` (bin) | 970,752 B | 970,752 B (byte-identical) | CONFIRMED |
| 3 | `Branch_Data.mdb` (System, V2 only) | (n/a) | 966,656 B (narrower) | CONFIRMED (two templates ship) |
| 4 | `Patients_backup.mdb` | 2,146,304 B | 2,146,304 B (stale in both) | CONFIRMED |
| 5 | `NOTEX.dll` | v1.00 | v2.00 ("RLS Notes 2.0") | CONFIRMED |
| 6 | `NOTEXS.dll` | present | removed | CONFIRMED |
| 7 | `help.chm` | 15 topics | byte-identical (stale) | CONFIRMED |
| 8 | `Cache\*.rtf` | 29 | 29 (byte-identical) | CONFIRMED |
| 9 | `Cache\*.exe` | 13 | 13 (byte-identical) | CONFIRMED |
| 10 | Runtime (ActiveX) | 43 controls | 43 (byte-identical) | CONFIRMED |
| 11 | `MSVBVM60.DLL` | 1,384,479 B (2004 build) | 1,386,496 B (2009 build, in CAB) vs 1,384,479 B (in OCX\`) | CONFIRMED (non-deterministic install) |
| 12 | Permission bits | 12 | 17 | CONFIRMED (extension) |
| 13 | Branch segment in `patientid` | not documented | position 9 | CONFIRMED (V2 addition at same position) |
| 14 | PDF capability | absent (V1-BR-046) | present (Bullzip) | CONFIRMED (V2 ONLY) |
| 15 | DDL / schema migration | 0 | 268 statements | CONFIRMED (V2 ONLY) |
| 16 | `useraccounts` table | referenced, absent from all `.mdb` | referenced, SQL-Server-only | CONFIRMED |

## 7. V1 source accessibility (EXTERNAL)

The V1 consolidated documents (`Complete_Reverse_Engineering_Reference_V1.md`, `Business_Rules_V1.md`,
`Data_Model_and_Workflow_Entities_V1.md`, `Functional_Specification_V1.md`) are **not present in this
repository**. They exist only in the external corpus (`E:\Markdown Source Files`).

- **Do not** assume they are accessible from this repo.
- **Do not** fabricate their contents.
- **Do not** copy information from memory or filenames.
- Label all V1 references as **EXTERNAL**.
- Record missing accessibility as an unresolved documentation limitation (see `Docs/KB/legacy/evidence/UNRESOLVED.md`).

The knowledge architecture supports V1 through `VERSIONS.md` and cross-references, but V1 facts must
be tagged `EXTERNAL SOURCE REQUIRED` until the external documents are explicitly consulted.

## 8. Evidence and traceability

Sources cited in this knowledge base use these evidence-type codes (inherited from `Business_Rules_V1/V2`):

| Code | Meaning |
|---|---|
| ET-DOC | Stated in a shipped document (help.chm, RLS_Learn.pdf, RLS_Setup.pdf) |
| ET-DOC+DB | Stated in a document and present as stored data |
| ET-DB | Present as stored data in the extracted schema |
| ET-CODE | Present in the executable's literals, SQL, or object model |
| ET-DDL | Present in the 268 embedded schema-migration statements |
| ET-CFG | Present in a shipped configuration file |
| ET-INST | Present in the installation media or installer |
| ET-DERIVED | Derived by cross-comparing two evidence classes |
| ET-PRES | Present in the presentation library (RTF sheets, Flash simulators) |

Confidence levels: CONFIRMED · STRONGLY SUPPORTED · INFERRED · UNKNOWN · CONFLICTING.

Each important claim carries: **Evidence** (what the source establishes) · **Finding** (what the RE
documentation reports) · **Interpretation** (reasonable conclusion) · **Uncertainty** (what is not
established). A statement in an existing report is not automatically an independently verified fact.

## 9. Related documents

- `Docs/KB/legacy/INDEX.md` — legacy entry point.
- `Docs/KB/legacy/architecture/*` — architecture, deployment, configuration, PDF/printing.
- `Docs/KB/legacy/data/*` — model, migration, identifiers, branches.
- `Docs/KB/legacy/clinical/*` — rules, knowledge layer, profiles.
- `Docs/KB/legacy/business/*` — workflows, security, capabilities.
- `Docs/KB/legacy/evidence/*` — evidence register, contradictions, unresolved.
- `Docs/KB/legacy/mapping/LEGACY-TO-MODERN.md` — legacy vs modern comparison.
- `Docs/KB/legacy/MAINTENANCE.md` — governance and update rules.
- Original source evidence (preserved, not replaced): `Database documentation.md`, `Documenting the Executable File.md`, `Docs/Complete_Reverse_Engineering_Reference_V2.md`, `Docs/Business_Rules_V2.md`, `Docs/Data_Model_and_Workflow_Entities_V2.md`, `Docs/Functional_Specification_V2.md`, `evidence/reverse-engineering/*`.
