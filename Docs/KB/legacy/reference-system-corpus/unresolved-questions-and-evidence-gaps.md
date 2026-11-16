# Unresolved Questions and Evidence Gaps — RealLab CD 2019 v1.605

**Consolidated:** 2026-10-31 (UTC) · **Static analysis only.**
Machine-readable: `machine-readable/unified-unresolved-questions.csv` and `.json`.

> **Every entry below is a recorded limitation, not an omission.** Nothing was
> reconstructed from assumption, and no missing artefact was invented.

---

## 1. The register

| UNR-001 | Which executable copy is authoritative? | RESOLVED |
| UNR-002 | What does each of the 17 grantN columns on the Patients table mean? | UNRESOLVED |
| UNR-003 | What are the seven IDPassword* values and what do they unlock? | UNRESOLVED |
| UNR-004 | What are the two 32-character uppercase literals (9OYOJAUGJ6LHS8NHT5IPI00YHC52WY7DT6HK, 13ADE9E2Z0VXJAJJ1D230PP5TRHZ1IB8ZEXF)? | UNRESOLVED |
| UNR-005 | Is the password stored in accuntpw hashed before it is written? | UNRESOLVED |
| UNR-006 | Is the B1 target archive available anywhere? | NO |
| UNR-007 | Was B2 independently investigated in this project? | NO |
| UNR-008 | Which of the 1,370 embedded SELECT statements are executed against which backend? | PARTIALLY RESOLVED |
| UNR-009 | What are the 25 external components declared in the VB6 header? | UNRESOLVED |
| UNR-010 | What is the compiled code path of the report renderer? | PARTIALLY RESOLVED |
| UNR-011 | Does Data/REPORT.ini exist anywhere? | NO |
| UNR-012 | What is unrar.dll for? | NO |
| UNR-013 | Does the analysis of the 76 MB executable reach its decompiled code? | PARTIALLY RESOLVED |
| UNR-014 | How is the _S template variant selected? | PARTIALLY RESOLVED |
| UNR-015 | Did the Ghidra headless run produce any usable decompilation? | NO — ANSWERED |
| UNR-016 | Is a 1.2 GB -Xmx sufficient to analyse this binary in this container? | NO — ANSWERED |
| UNR-017 | Why do B1 and B2 have no byte-identical overlap with each other? | ANSWERED |

---

## 2. Evidence gaps — material that was sought and not found

| # | Missing material | Consequence | Status |
| ---: | --- | --- | --- |
| 1 | **The B1 target archive** (28,314,816 B, SHA-256 `aedb61ca…5ba3a78`) | The B1 *analysis package* survives and its `file-inventory.csv` preserves the digest of every file the archive held — including the primary executable — so the executable's identity was confirmable. The archive's **other** contents are known only through B1's reports | **Unavailable.** Not in the workspace or the attachment directories |
| 2 | **The Access / Jet mirror database** | The mirror's schema was reconstructed from B1's work and the executable's DDL, but no `.mdb` was ever inspected. The **complete** table and column inventory is therefore never directly observed | **Unavailable in every portion** |
| 3 | **The 25 VB6 external component names** | The count is known; the names are not. The *installed* set is known from B2 and B4 | **Open (UNR-009)** |
| 4 | **Per-object method names** | Only method *counts* were recovered. The 8,026 methods are numbered, not named | **Open** |
| 5 | **`Data/REPORT.ini`** | B1 and B2 both establish that the main executable reads this path for the global report colour theme. It is in **no** investigated portion | **Open (UNR-011)** |
| 6 | **The `unrar.dll` payload** | B2's orphaned 2003-era library. No later investigation shed light on it | **Open (UNR-012)** |
| 7 | **The installer payloads B4 references** — `SQLEXPR.EXE`, the MySQL connector, both .NET installers, `D:\Lab_data\Attaching.sql` | Their provenance, versions and signatures could not be assessed | **Open (B4 DEP-B4-U01)** |
| 8 | **Any dynamic evidence whatsoever** | No component was executed, no database contacted, no URL fetched, no credential validated | **Out of scope by design** |

---

## 3. The contradictions on record

### 3.1 The manual is version 1.0; the product is version 1.605

| Source | Statement |
| --- | --- |
| B4's manual, every page footer | *"real lab system version 1.0"* |
| B4's manual copyright page | *"ais group corporation 2010"* |
| The executable's VERSION resource | `FileDescription = "RLS 1.605"`, `FileVersion = 1.605` |
| The PE link timestamp | `0x5F186BBA` — 2020-07-24 |

**Not a contradiction — an ageing document.** The manual is a **version 1.0
document shipped inside a version 1.605 installation** and was never updated.
That is itself worth recording: the product's only user-facing documentation
describes a version ten releases behind.

### 3.2 The install directory disagrees with the compiled-in paths

| Source | Path |
| --- | --- |
| B4's `SETUP.LST` `DefaultDir` | `$(ProgramFiles)\real lab system` |
| The executable's literals | `D:\real lab system\bin\Patients.mdb` (+3 siblings) |
| B1's recorded deployed paths | `D:\real lab system\…` |

**Resolution.** The installer allows a different location and the deployed
machine had been installed on `D:`. The executable's hard-coded `D:` literals
are the *authoring* default, not the only supported layout — which is the
basis of U-024.

### 3.3 The manual mandates English (Canada); the product stores Arabic

| Source | Statement |
| --- | --- |
| B4's manual, topic 2 | Regional Options → Standards and formats **must** be **English (Canada)** |
| The executable's project header | LCID 1033 / **4105 (ar-SA)** |
| B3's templates and B4's manual | Windows-1256 (Arabic ANSI) |

**Unresolved.** The regional-format setting governs number and date
rendering; the codepage governs the Arabic text. Whether the application
opens its templates *explicitly* as cp1256 or against the host ANSI codepage
**could not be determined**, and it is the mechanism behind B3's FND-B3-003.

### 3.4 The MySQL driver versus the connection strings

B4's `Setup5.bat` installs the MySQL Connector/ODBC 3.51.30. The executable's
connection templates use **only** `SQLOLEDB.1` and
`Microsoft.Jet.OLEDB.4.0`. **Unresolved** — an installed driver with no
observed use.

### 3.5 The two B3/B4 defects preserved on purpose

| Defect | Handling |
| --- | --- |
| B3's manifest self-reference exception | **Documented, not fixed** — the instruction is to preserve earlier deliverables |
| B4's `pe_analyse.py` entry-point offset | **Documented, not patched** — no B4 finding depends on it; the correct value was established independently |

---

## 4. What would close each open question

| Question | Evidence required | Feasible where? |
| --- | --- | --- |
| UNR-002 `grantN` semantics | The `Patients` schema from a deployed instance, or decompilation of `patientmain` | A licensed deployment |
| UNR-003 `IDPassword*` mapping | Decompilation of the report objects | Ghidra, given more memory |
| UNR-004 the two 32-character literals | A controlled experiment against a lab instance | A deployment |
| UNR-005 `accuntpw` hashing | The INSERT path | Decompilation |
| UNR-008 statement-to-caller mapping | Completed decompilation | Ghidra with swap or ≥ 6 GiB |
| UNR-009 the 25 external components | The external-component descriptor layout | Further static work on the VB header |
| UNR-010 the report call chain | Completed decompilation | as UNR-008 |
| UNR-011 `Data/REPORT.ini` | The file, or its generator | The vendor |
| UNR-012 `unrar.dll` | Its import table in context | Extended PE analysis of the B2 payload |
| UNR-014 the `_S` variant selection | Decompilation of the report-setup objects | as UNR-008 |
| UNR-015/016 Ghidra viability | More memory, or a different analyser configuration | The container, or a larger host |

**None of these can be closed by more string extraction.** Each needs
decompilation, a licensed deployment, or the vendor.

---

## 5. A note on what is *not* an evidence gap

Some things are absent from the evidence **by design**, and their absence is
not a limitation of the analysis:

* **Runtime behaviour** — the project was scoped as static analysis
  throughout, and the dynamic restrictions are explicit.
* **Patient data** — no patient record was ever available; the schema was
  reconstructed from DDL and never from data. **No identifiable clinical
  content was extracted or reproduced.**
* **B5** — out of scope for this assignment.
