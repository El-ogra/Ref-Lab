# Legacy Evidence Register — Index

**Purpose:** Navigable index of evidence artefacts supporting legacy-system findings.
**Scope:** Evidence items, SHA-256, paths, version applicability, domain, status.
**Audience:** Auditors, developers, analysts.
**Version applicability:** Mixed (noted per entry).

---

## 1. Source evidence files (preserved, not replaced)

| Artefact | Location | Type | Version | SHA-256 (from `FINAL_EXECUTION_REPORT_AR.md` / `Database documentation.md`) |
|---|---|---|---|---|
| `Patients.mdb` | `F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\bin\Patients.mdb` (external, read-only) | Access Jet 4 database | V2 | `98D51F5294496E03F352FF329F099572915CCB2E670C1F143D6BA368EEE898A3` (2,682,880 B) |
| `real lab system.exe` | `F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe` (external, read-only) | PE32 VB6 executable | V2 | `C54CE6C8C98A679EFC5AB485BC4B313B452026D6A40F9BDA36ECA7EA18DF93C0` (76,292,096 B) |
| `RefLab_Reference_V2_Consolidated.zip` | repo root | 12-member cumulative knowledge base | V1+V2 | `603823F0D9ED3E7A3D60847A74A3C5D6443F16A23E5CBCBE085624A9A905C028` (477,540 B) |
| `Database documentation.md` | repo root | Deliverable 1 (209,465 B) | V2 | digest recorded but **truncated in this register** — recompute before use |
| `Documenting the Executable File.md` | repo root | Deliverable 2 (172,287 B) | V2 | digest recorded but **truncated in this register** — recompute before use |

> **CORRECTION (2026-11-16).** The two digests above were truncated in an earlier revision of this
> register, which made them unusable for verification. They are now marked as truncated rather
> than presented as complete SHA-256 values. Recompute them from the files themselves.

## 2. Evidence registers (preserved, referenced)

| Register | Location | Entries | Status |
|---|---|---:|---|
| `reverse-engineering-evidence-register.csv` | `evidence/reverse-engineering/` | 36 | PRESERVED (REFERENCE) |
| `reverse-engineering-file-accountability.csv` | `evidence/reverse-engineering/` | 28 | PRESERVED (REFERENCE) |
| `export_ledger.csv` | `evidence/reverse-engineering/metadata/` | 103 tables | PRESERVED (REFERENCE) |
| `pseudonym_map.txt` | `evidence/reverse-engineering/metadata/` | 18 pseudonyms | PRESERVED (REFERENCE, restricted) |

## 3. Companion exports

- `evidence/reverse-engineering/exports/tables/*.json` — 103 JSON files (one per table), 3,809 records,
  redacted per policy v3 (credentials redacted by column name, phones masked, 13-digit identifiers
  pseudonymised by value shape).
- `evidence/reverse-engineering/exe/*` — PE/VB6 analysis artefacts: `exe_findings.md` (42,370 B),
  `README.md`, `tool_log.md`, `exe_inventory.json`, `strings_classified.json` (101,812 strings),
  `resources.json`, `imports.json`, `pe_identity.json`, `pe_dirs.json`, `sql_inventory.json`,
  `vb_objects.json`, `vb_members.json`, `vb_code_entries.json`, `db_xref.json`, `version_info.json`,
  plus 14 PE resource files. **There are no Ghidra outputs in this directory** — the earlier
  revision of this register listed `*.functions.txt`, `*.xrefs.txt`, `*.strings.txt`,
  `*.program.json` and `*.ghidra.log`, none of which exist. *(Corrected 2026-11-16.)*
- `evidence/reverse-engineering/phase0/*` — **18 files**, not 3: `PHASE-0-SPEC.md`,
  `PHASE-0-REPORT.md`, `PHASE-0-SPEC-ADDENDUM-01.md`, `collect_phase0.py`, `phase0-summary.json`,
  and 13 CSV registers (837-row coverage matrix, 125-row gap register, 5,186-row prior-output
  ledger, file ledger, release membership, artefact-version matrix, claimed-vs-present,
  shared-identical, target-verification, artefact-basename index, coverage matrix, file ledger,
  gap register).
- `evidence/reverse-engineering/scripts/*` — **5 scripts: 3 Python (`mdb_detail.py`,
  `mdb_quality.py`, `mdb_verify.py`) and 2 PowerShell (`mdb_phaseA.ps1`, `mdb_phaseB2.ps1`)** —
  not "5 Python". *(Corrected 2026-11-16.)*

## 4. Evidence type codes

ET-DOC, ET-DOC+DB, ET-DB, ET-CODE, ET-DDL, ET-CFG, ET-INST, ET-DERIVED, ET-PRES.
(See `Docs/KB/legacy/VERSIONS.md` §8 for definitions.)

## 5. Confidence levels

CONFIRMED · STRONGLY SUPPORTED · INFERRED · UNKNOWN · CONFLICTING.

## 6. Traceability standard

Every important technical claim in the curated knowledge base must cite:
- Repository-relative path for internal sources.
- Explicitly labelled `EXTERNAL` for sources outside the repository.
- Evidence type code and confidence level.
- The original source's verification status (do not upgrade an interpretation to a verified fact).

## 7. External V1 material — RESOLVED 2026-11-16

**The V1 consolidated documents are accessible.** `Complete_Reverse_Engineering_Reference_V1.md`,
`Business_Rules_V1.md`, `Data_Model_and_Workflow_Entities_V1.md` and `Functional_Specification_V1.md`
are all present in the reverse-engineering corpus and have already been used — the unresolved
register reproduces `RL-V1-Q-01…09` verbatim from `Complete_Reverse_Engineering_Reference_V1.md`
§1.6, and `data/MODEL.md` and `clinical/PROFILES.md` cite
`Data_Model_and_Workflow_Entities_V1.md`.

The earlier `EXTERNAL SOURCE REQUIRED` labelling was based on the documents being absent **from the
repository**, not from them being unavailable. That labelling has been withdrawn; the documents are
registered with their dispositions in `Docs/KB/legacy/evidence/SOURCES.md` §3 and their per-version facts in
`VERSIONS.md`.

The V1 documents are **not** preserved inside the repository. Preserving them is an open item in
`Docs/KB/legacy/evidence/VALIDATION.md` §7.

## 8. Related documents

- `Docs/KB/legacy/evidence/CONTRADICTIONS.md` — contradiction register.
- `Docs/KB/legacy/evidence/UNRESOLVED.md` — unresolved questions register.
- `evidence/reverse-engineering/*` — original evidence artefacts (preserved).
- `Docs/KB/legacy/mapping/LEGACY-TO-MODERN.md` — legacy vs modern.
- Original detailed sources: `Database documentation.md`, `Documenting the Executable File.md` (root).


---

## Evidence Batch 02 (2026-11-16)

Reverse-engineering knowledge base for the reference system (V-ROOT-1.00 + V-CD-2019) is available under
`Docs/KB/legacy/evidence-batch-02/`. Entry point: `D12_Executive_Summary/Executive_Summary.md`.

Key new artifacts:
- `D03_Type1_Analysis/Stage1_Technical_Findings.md` — 13 findings (program/binary evidence).
- `D04_Type2_Analysis/Stage2_Technical_Findings.md` — 12 findings + CON-001 (database/data evidence).
- `D05_Type3_Analysis/Stage3_Technical_Findings.md` — 14 findings + CON-002/003 (synthesis/process evidence).
- `D06_Unified_Registries/` — findings (39), evidence (461), relationships (15), contradictions (5).
- `D08_Engineering_Knowledge_Base/` — 22-file reconstructed KB.
- `D10_Unresolved_Issues/Open_Questions_and_Gaps.md` — UNR-002..009 + evidence gaps.
- `D13_Final_Project_Manifest/` — 62-file manifest.


---

## Reverse Engineering Output (MASTER DIRECTIVE, 2026-11-16)

Full-spectrum RE output for RealLab 1.605 is available under
`Docs/KB/legacy/reverse-engineering-output/`. Entry point:
`Docs/KB/legacy/reverse-engineering-output/10_UNIFIED_KNOWLEDGE_BASE/MASTER_SYSTEM_KNOWLEDGE.md`.

Key artifacts:
- `00_EXECUTIVE_REPORT/` — Investigation_Overview, Final_Reverse_Engineering_Report, Evidence_Coverage_Report, Remaining_Knowledge_Gaps.
- `01_ENVIRONMENT_AND_TOOLS/` — Environment_Verification, Installed_Tools_Inventory, Tool_Capability_Matrix, Tool_Usage_Log.
- `02_SOURCE_INVENTORY/` — Complete_Artifact_Inventory (2,843 rows), SHA256_Manifest, Archive_Extraction_Report, Unreadable_Artifacts.
- `03_EXECUTABLE_REVERSE_ENGINEERING/` — Executable_Inventory, Binary_Architecture, Managed_Assembly_Analysis, Native_Code_Analysis, Reconstructed_Code_Structure, Class_And_Method_Catalog, Dependency_Graph, Resources_And_Assets, Executable_Business_Logic.
- `04_DATABASE_FORENSICS/` — Database_Inventory, Complete_Database_Schema, Tables_And_Columns, Relationships, Constraints_And_Indexes, Stored_Procedures_And_Triggers, Database_Data_Analysis, Database_Business_Logic.
- `05_APPLICATION_DATABASE_MAPPING/` — Application_Data_Access_Architecture, Method_To_SQL_Mapping, Method_To_Table_Mapping, Database_Transactions, Data_Flow_Reconstruction.
- `06_CONFIGURATION_AND_DEPENDENCIES/` — Configuration_Analysis, Runtime_Requirements, External_Dependencies, Deployment_Architecture.
- `07_RUNTIME_AND_BEHAVIORAL_ANALYSIS/` — Startup_And_Initialization, Runtime_Observations, UI_And_Event_Flows, Application_Workflows, State_Transitions, Controlled_Experiments.
- `08_BUSINESS_DOMAIN_RECONSTRUCTION/` — Patient_Management, Visits_And_Orders, Simple_And_Composite_Tests, Results_Management, Laboratory_Business_Rules, Financial_Management, Printing_And_Reporting, Security_And_Permissions.
- `09_CROSS_SOURCE_EVIDENCE/` — Evidence_Catalog, Cross_Source_Correlation, Verified_Findings, Conflicting_Findings, Confidence_Assessment.
- `10_UNIFIED_KNOWLEDGE_BASE/` — MASTER_SYSTEM_KNOWLEDGE, MASTER_SYSTEM_ARCHITECTURE, MASTER_DATABASE_KNOWLEDGE, MASTER_BEHAVIORAL_KNOWLEDGE, MASTER_BUSINESS_LOGIC, MASTER_TECHNICAL_SPECIFICATION, MASTER_REVERSE_ENGINEERING_EVIDENCE.
- `11_INVESTIGATION_LOGS/` — Investigation_Progress, Tool_Execution_Log, Failed_Analysis_Operations, Knowledge_Gap_Resolution_Log, Resume_Checkpoint.

New REA verification (2026-11-16): open_binary confirmed v1.605 exe (PE x86, sha256 c54ce6c8...). inspect_managed_artifact confirmed NOT managed (no CLI directory). Native providers (rizin/Ghidra/objdump) unavailable for PE. No DB row reading (no mdbtools/olefile/pyodbc). No runtime execution (safety).


---

## Reference System Source Corpus (Agent B, 2026-11-16)

A second independent analysis corpus (298 reports) is available under
`Docs/KB/legacy/reference-system-corpus/`. Entry point:
`Docs/KB/legacy/reference-system-corpus/master-engineering-reference.md`.

Key files (35 copied):
- `master-engineering-reference.md` — central engineering reference.
- `unified-application-architecture.md` — components, relationships, diagrams.
- `functional-module-catalogue.md` — 212-object register with method counts.
- `primary-executable-technical-reference.md` — executable in full.
- `database-engineering-reference.md` — schema, tables, SQL, connections.
- `installation-and-deployment-reference.md` — installers, manifests, dependencies.
- `security-and-technical-risk-register.md` — consolidated risk view.
- `unified-findings-register.md` — every finding with origin and cross-references.
- `cross-batch-correlation-matrix.md` — batch relationships with SHA-256.
- `unresolved-questions-and-evidence-gaps.md` — 17 open questions.
- `verification-and-integrity-report.md` — hashes, archives, validation.
- `project-evidence-and-source-inventory.md` — every source artefact.
- `Complete_Reverse_Engineering_Reference_V1.md` / `V2.md` — full reference.
- `Business_Rules_V1.md` / `V2.md`, `Data_Model_and_Workflow_Entities_V1.md` / `V2.md`.
- `Functional_Specification_V1.md` / `V2.md`, `Functionality_Requirements_Matrix_V1.md` / `V2.md`.
- `software-architecture.md`, `data-flow-and-control-flow.md`, `dependency-analysis.md`.
- `evidence-index.md`, `coverage-and-limitations.md`, `methodology-and-tools.md`.
- `Version_Identity_and_Scope.md`, `Version-Identification.md`.
- `Open-Issues-and-Limitations.md`.

Independent verification: Agent B's corpus confirms and expands on Batch-02 findings (native code, Jet 4, 212 objects, 6,522 Ghidra functions, 190 changed files V1->V2019, 41 consolidated findings). Cross-verification pending comparison of per-finding PASS/FAIL.
