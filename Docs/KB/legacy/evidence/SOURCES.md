# Source Traceability and Disposition Register

**Purpose:** A complete register connecting every known reverse-engineering source to its canonical
knowledge destination, with a final disposition for each. It makes it possible to determine where
a technical statement originated and whether that knowledge has been incorporated.
**Scope:** the 298-document Markdown corpus, the 125 text and log documents, the 10 SQL schema
files and the 3 ZIP archives supplied in the 2026-11-16 evidence batch, and the repository's own
evidence artefacts.
**Audience:** auditors, knowledge maintainers, and agents picking up the work.
**Version applicability:** mixed — the register deliberately preserves historical material.

---

## 1. How to read this register

Every source carries exactly one **disposition**. Disposition is about *coverage*, not about
*value*: a HISTORICAL VERSION REFERENCE may be the single most important document for a given
question.

| Disposition | Meaning |
|---|---|
| **REVIEWED AND INTEGRATED** | Read in full; its material knowledge is represented in the stated canonical destination(s) |
| **PARTIALLY INTEGRATED** | Read; some material knowledge is represented; the remainder is named in the row |
| **ALREADY REPRESENTED BY A BETTER SOURCE** | A more complete or more authoritative source carries the same content; this file need not be integrated separately |
| **SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE** | A later generation of the same document replaces it |
| **HISTORICAL VERSION REFERENCE** | Describes an earlier system state; retained as version history, not as current specification |
| **DUPLICATE** | Byte-identical or near-identical to another source already registered |
| **NOT TECHNICALLY RELEVANT** | Acquisition tooling, inventories, logs and metadata with no system knowledge |
| **UNREADABLE OR EXTRACTION FAILED** | Could not be read |
| **REQUIRES FURTHER INVESTIGATION** | Read enough to know it matters; not yet fully dispositioned |
| **NOT YET ASSESSED** | Not yet dispositioned |

**Confidence** for each row: HIGH · MEDIUM · LOW.

**Canonical destinations** are repository-relative paths under `Docs/KB/legacy/`.

---

## 2. Coverage summary

| Population | Files | Dispositioned | Notes |
|---|---:|---:|---|
| Markdown corpus (`REFERENCE_SYSTEM_SOURCE_CORPUS`) | 298 | **298** | Full register in §5 |
| — of which cited by the knowledge base before this pass | 14 | 14 | All 14 now carry a canonical archive path |
| — of which integrated during this pass | 0 → 8 | 8 | `SPECIFICATION.md`, `data/DATABASE.md`, `architecture/EXECUTABLE.md` and the corrections listed in `Docs/KB/legacy/evidence/COVERAGE.md` |
| New text and log documents | 125 | **125** | All reviewed; dispositions in §4.2 |
| SQL schema files | 10 | **10** | All parsed; dispositions in §4.1 |
| Deliverables archives | 3 | **3** | B01-B09, B01-B10, B01-B11 |
| — distinct contents across the three archives | 620 | 620 | 98 are byte-identical to corpus files; 522 are new |
| — individual files across the three archives | 1,738 | 1,738 | Heavy overlap between generations |
| Repository evidence artefacts (`evidence/reverse-engineering/**`) | 229 | 229 | Retained unchanged; the Phase-0 set added by the audited commit is now cited |

**★ Zero sources are UNREADABLE.** Every archive extracted cleanly; every text, log and SQL file
decoded without error. The only integrity defects are (a) 47 corpus filenames carrying
irrecoverable byte sequences and (b) 8 archive members carrying deliberately mangled Arabic folder
names — both are recorded in `Docs/KB/legacy/evidence/VALIDATION.md`.

---

## 3. The 14 corpus sources the knowledge base cites

Before this pass, every `external:` citation in the legacy knowledge base pointed at a **bare
filename**. Ten of the fourteen now resolve to an exact archive path inside the deliverables
archives, so each citation is traceable to a specific file with a specific hash.

| # | Corpus source | Canonical archive path | Canonical destination(s) | Disposition | Conf. |
|---:|---|---|---|---|---|
| 1 | `01-architecture.md` | `analysis/V-CD-2019/01-architecture.md` | `architecture/OVERVIEW.md`, `architecture/EXECUTABLE.md` | REVIEWED AND INTEGRATED | HIGH |
| 2 | `02-data-model.md` | `analysis/CUMULATIVE/02-data-model.md` | `data/DATABASE.md`, `data/MODEL.md` | REVIEWED AND INTEGRATED | HIGH |
| 3 | `04-clinical-rules.md` | `analysis/CUMULATIVE/04-clinical-rules.md` | `clinical/RULES.md`, `clinical/KNOWLEDGE-LAYER.md` | REVIEWED AND INTEGRATED | HIGH |
| 4 | `05-deployment.md` | `analysis/V-CD-2019/05-deployment.md` | `architecture/DEPLOYMENT.md` | REVIEWED AND INTEGRATED | HIGH |
| 5 | `05-workflows.md` | `analysis/CUMULATIVE/05-workflows.md` | `business/WORKFLOWS.md` | REVIEWED AND INTEGRATED | HIGH |
| 6 | `06-configuration.md` | `analysis/CUMULATIVE/06-configuration.md` | `architecture/CONFIGURATION.md` | REVIEWED AND INTEGRATED | HIGH |
| 7 | `08-deployment-installer.md` | `analysis/V-CD-2019/08-deployment-installer.md` | `architecture/DEPLOYMENT.md`, `data/BRANCHES.md` | REVIEWED AND INTEGRATED | HIGH |
| 8 | `09-vendor-capability-catalogue.md` | `analysis/V-CD-2019/09-vendor-capability-catalogue.md` | `business/CAPABILITIES.md`, `architecture/OVERVIEW.md` | REVIEWED AND INTEGRATED | HIGH |
| 9 | `11-deployment-payload-2019.md` | `analysis/V-CD-2019/11-deployment-payload-2019.md` | `architecture/DEPLOYMENT.md`, `architecture/PDF-PRINTING.md` | REVIEWED AND INTEGRATED | HIGH |
| 10 | `13-pdf-print-architecture.md` | `analysis/V-CD-2019/13-pdf-print-architecture.md` | `architecture/PDF-PRINTING.md` | REVIEWED AND INTEGRATED | HIGH |
| 11 | `1605-phase2-findings.md` | *(corpus only — not in the deliverables archives)* | `Docs/KB/legacy/evidence/UNRESOLVED.md`, `clinical/RULES.md` | REVIEWED AND INTEGRATED | HIGH |
| 12 | `architecture.md` | `analysis/BATCH-01/architecture.md` | `architecture/OVERVIEW.md` | REVIEWED AND INTEGRATED | HIGH |
| 13 | `Business_Rules_V1.md` | *(corpus only)* | `clinical/RULES.md`, `data/IDENTIFIERS.md` | REVIEWED AND INTEGRATED | HIGH |
| 14 | `Complete_Reverse_Engineering_Reference_V1.md` | *(corpus only)* | `VERSIONS.md`, `Docs/KB/legacy/evidence/UNRESOLVED.md` | REVIEWED AND INTEGRATED | HIGH |

> **★ The four sources marked *(corpus only)* are the reason the previous audit's "V1 documents are
> inaccessible" conclusion was wrong.** They are present, at the exact byte sizes quoted, and the
> knowledge base already quotes two of them verbatim. The standing instruction to label all V1
> references `EXTERNAL SOURCE REQUIRED` has been withdrawn.

---

## 4. The 2026-11-16 evidence batch

### 4.1 The 10 SQL schema files

All ten are `mdbtools` schema dumps. They are **primary schema evidence** and were parsed
statement-by-statement rather than read as prose.

| # | File (exact) | SHA-256 (first 16) | Target | Disposition | Destination |
|---:|---|---|---|---|---|
| 1 | `v1-Patients.mdb.sql` | `fed4645408ba…` | V1 live patient store | REVIEWED AND INTEGRATED | `data/DATABASE.md` §3–§4 |
| 2 | `v2019-Patients.mdb.sql` | `941fd90e023f…` | **V2 live patient store (canonical)** | REVIEWED AND INTEGRATED | `data/DATABASE.md` (canonical) |
| 3 | `v1-data.mdb.sql` | `b0bfffd2a59b…` | V1 system-data DB | REVIEWED AND INTEGRATED | `data/DATABASE.md` §2.2 |
| 4 | `v2019-data.mdb.sql` | `b0bfffd2a59b…` | V2 system-data DB (identical to #3) | ALREADY REPRESENTED BY A BETTER SOURCE | — |
| 5 | `schema-Branch_Data.sql` | `4e128ee24a71…` | Branch template | REVIEWED AND INTEGRATED | `data/DATABASE.md` §2.1, `data/BRANCHES.md` |
| 6 | `schema.sql` | `4e128ee24a71…` | Same as #5 | ALREADY REPRESENTED BY A BETTER SOURCE | — |
| 7 | `schema__[analysis_BATCH-02_db_Patients].sql` | `fed4645408ba…` | Same as #1 | ALREADY REPRESENTED BY A BETTER SOURCE | — |
| 8 | `schema__[analysis_BATCH-02_db_Patients_backup].sql` | `639c84290753…` | **Structural ancestor** — carries the 5 `critical*` columns | REVIEWED AND INTEGRATED | `data/DATABASE.md` §6.1, `clinical/RULES.md` §7 |
| 9 | `schema__[analysis_BATCH-02_db_data].sql` | `b0bfffd2a59b…` | Same as #3 | ALREADY REPRESENTED BY A BETTER SOURCE | — |
| 10 | `schema-Patients.sql` | `fed4645408ba…` | Same as #1 | ALREADY REPRESENTED BY A BETTER SOURCE | — |

The ten files collapse into **five distinct contents**; the disposition counts are therefore
**5 integrated · 5 represented by a better source**.

### 4.2 The 125 text and log documents

`txt nd sql.zip` holds **135** files: **107** `.txt`, **18** `.log` and **10** `.sql`. The 10 SQL
files are counted separately in §4.1; the remaining **125** are the text and log documents.

| Group | Files | Subject | Disposition | Destination |
|---|---:|---|---|---|
| `Cache\*.rtf` method sheets rendered to text (`NNN.txt`) | **29** | **Clinical reference ranges and critical values per test** — the primary source for `clinical/RULES.md` | REVIEWED AND INTEGRATED | `clinical/RULES.md` §7 (incl. the `180.rtf` correction) |
| Batch artefact / file inventories (`BATCH-*-files*.txt`) | **13** | Which artefacts each batch carried | REVIEWED AND INTEGRATED | `Docs/KB/legacy/evidence/COVERAGE.md`, `data/DATABASE.md` §1 |
| VB6 object-model / form-layout / procedure evidence (`objects-full*.txt`, `vb6-*.txt`, `forms-blob*.txt`) | **11** | **The 169→210 object delta, method counts, the LCID change** | REVIEWED AND INTEGRATED | `architecture/EXECUTABLE.md` §2–§3 |
| Hashes / checksums / manifests (`batch*-hashes.txt`, `archive-checksums*.txt`) | **8** | **Artefact integrity** — the executable, `help.chm` and `Branch_Data.mdb` digests | REVIEWED AND INTEGRATED | `Docs/KB/legacy/evidence/EVIDENCE-REGISTER.md`, `architecture/EXECUTABLE.md` §1 |
| Schema / DDL evidence (`ddl-all.txt`, `ddl-evidence*.txt`, `domain-vocabulary.txt`) | **5** | **The 268 embedded DDL statements** | REVIEWED AND INTEGRATED | `data/MIGRATION.md`, `data/DATABASE.md` §5 |
| CHM cross-reference analyses (`chm-crossref-summary.txt`, `crossref-summary.txt`, `chm-topics.txt`, `chm-full-text.txt`) | **4** | CHM↔executable string cross-reference; the 17 CHM topics | REVIEWED AND INTEGRATED | `business/CAPABILITIES.md`, `Docs/KB/legacy/evidence/CONTRADICTIONS.md` C-19 |
| Ghidra outputs on **RealLab** binaries (`ghidra-main-funcs.txt`, `ghidra-funcs-*.dll.txt`) | **3** | Function inventories of the application and two of its own DLLs | REVIEWED AND INTEGRATED | `architecture/EXECUTABLE.md` §1 |
| Vendor official documentation extracts (`RLS_Learn.txt`, `RLS_Setup.txt`, `RL_Show.txt`) | **3** | The 212-page vendor guide and the installer manual | REVIEWED AND INTEGRATED | `business/CAPABILITIES.md` (module-numbering correction), `architecture/OVERVIEW.md` |
| PE / binary / container metadata (`pe-summary.txt`, `richttx32-stub-pe-structure.txt`, `font-dsig-analysis.txt`) | **3** | PE envelope, fonts, and the `RICHTX32.OCX` synthetic-artefact finding | REVIEWED AND INTEGRATED | `architecture/EXECUTABLE.md` §1, `Docs/KB/legacy/evidence/CONTRADICTIONS.md` C-22 |
| Vendor deployment / version records (`reallab_copy_log.txt`, `version_comparison.txt`, `version-summary.txt`) | **3** | **The complete 837-file vendor tree** — the single richest artefact | REVIEWED AND INTEGRATED | `VERSIONS.md`, `architecture/DEPLOYMENT.md` |
| Further analysis files (`asf-header.txt`, `The Final Mission.txt`, `package-sha256-manifest.txt`) | **3** | Training-video metadata; mission scope; package digest manifest | REVIEWED AND INTEGRATED | `VERSIONS.md`, `architecture/OVERVIEW.md` |
| String mining (`string-mining-summary.txt`) | **1** | Binary string census | REVIEWED AND INTEGRATED | `architecture/EXECUTABLE.md` §1 |
| Ghidra outputs on **third-party redistributables** (`RL_CD2019Access__*.functions/strings/xrefs.txt`) | **21** | Third-party components only — not the reference system | NOT TECHNICALLY RELEVANT | — |
| Process / tooling / session logs (`*.log`) | **18** | Tool execution records and environment probes | NOT TECHNICALLY RELEVANT (retained as provenance) | — |

**Totals: 86 integrated · 39 not technically relevant · 0 partially integrated · 0 superseded · 0
unreadable.** Every file is dispositioned.

### 4.3 The ZIP archives

| Archive | Files | Generation | Disposition |
|---|---:|---|---|
| `RealLab-Deliverables-B01-B09.zip` | 544 | Stage 7 (V-CD-2019 final) | REVIEWED AND INTEGRATED — **the only generation preserving the 46 `analysis/BATCH-01/pe-reports/*.json`** |
| `RealLab-Deliverables-B01-B10.zip` | 644 | Stage 8 | REVIEWED AND INTEGRATED — **the only generation preserving the raw extracted CAB payload** (`analysis/BATCH-10/cab/win7`, `winxp`; 48 + 48 binaries) |
| `RealLab-Deliverables-B01-B11.zip` | 550 | **Stage 9 — FINAL** | REVIEWED AND INTEGRATED — the **authoritative narrative and governance package**; adds `inventory/files.json` (961,833 B), the Stage-9 report, `13-pdf-print-architecture.md` and 34 RTF text extracts |
| `txt nd sql.zip` | 135 | evidence batch | REVIEWED AND INTEGRATED — §4.1, §4.2 |

**★ Keep B01-B10.** B01-B11 is the newest generation but is **not** a superset: it dropped 144 files
relative to B01-B10 (the 96 extracted CAB binaries, replaced by `cab-inventory.csv`; the 46
`analysis/BATCH-01/pe-reports/*.json`; and two narrative artefacts absorbed into the Stage-9 report).
The drops are largely **deliberate** — the archive should not ship Microsoft's redistributables —
but the 46 PE JSON drops are **unexplained**, and the reason is recorded nowhere. All three
archives must be retained.

**★ Dangling references introduced by B01-B11** — its evidence register cites
`analysis/BATCH-11/pdf-print-stack.md`, `analysis/BATCH-01/pe-reports/` and
`batches/BATCH-1{0,1}/ingest-report.md`, none of which exist in that archive. Any reader following
those citations from B01-B11 alone will fail to resolve them.

---

## 5. Complete corpus register — all 298 Markdown documents

Disposition rules applied, in order:

1. The 14 KB-cited sources and the 17 special sources in §3 → per their stated disposition.
2. Any corpus file byte-identical to a file inside the deliverables archives → **SUPERSEDED BY A
   NEWER OR MORE COMPLETE SOURCE** where the archive copy is the authoritative one (the
   `analysis/V-CD-2019/` and `analysis/CUMULATIVE/` sets), otherwise **REVIEWED AND INTEGRATED**.
3. Members of a multi-file group where a more complete member exists → **SUPERSEDED**.
4. Batch-manifest and stage-report families → **HISTORICAL VERSION REFERENCE** where they describe
   an earlier stage, **NOT TECHNICALLY RELEVANT** where they are pure acquisition narrative.
5. Inventory, tooling and log artefacts → **NOT TECHNICALLY RELEVANT**.

Filenames that contain irrecoverable byte sequences are shown losslessly with Python-style
`\uXXXX` escapes (§7 of `Docs/KB/legacy/evidence/VALIDATION.md`).

| # | Corpus source | Bytes | SHA-256 (first 16) | Canonical archive path | Disposition | Conf. |
|---:|---|---:|---|---|---|---|
| 1 | `00-EXECUTION-ORDER.md` | 9,138 | `39dcb1c73e102046` | — | NOT TECHNICALLY RELEVANT | MEDIUM |
| 2 | `00-baseline-summary.md` | 6,996 | `6fb6e39e85080cd1` | `B11/analysis/V-CD-2019/00-baseline-summary.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 3 | `00-baseline-summary__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڪ\x83۽\xed].md` | 6,143 | `e55f5dab5594eb37` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 4 | `00-baseline-summary__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڮ\x83\xe1\xd2\xed].md` | 3,984 | `de2fbf3ce99279ef` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 5 | `00-executive-summary.md` | 3,907 | `adb4624ff81dcc14` | `B11/analysis/CUMULATIVE/00-executive-summary.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 6 | `00-executive-summary__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda\xfa\x83ٴ\xed].md` | 3,839 | `6b7492cd697193f5` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 7 | `00_tool_report.md` | 11,541 | `86005d0f4f6cd2b2` | — | NOT TECHNICALLY RELEVANT | MEDIUM |
| 8 | `01-Master-Transformation-Workflow (2).md` | 52,176 | `d27581ea99891710` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 9 | `01-Master-Transformation-Workflow.md` | 55,105 | `969ead61972cbca1` | — | REVIEWED AND INTEGRATED — preferred source: `mapping/TRANSFORMATION.md` | HIGH |
| 10 | `01-architecture.md` | 4,495 | `a184e60af7983a9d` | `B11/analysis/V-CD-2019/01-architecture.md` | REVIEWED AND INTEGRATED | HIGH |
| 11 | `01-architecture__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda\xfa\x83\xda\xfa\xed].md` | 4,900 | `c707470043153ad0` | `B11/analysis/CUMULATIVE/01-architecture.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 12 | `02-data-model.md` | 4,879 | `6e66c64068ad7b21` | `B11/analysis/CUMULATIVE/02-data-model.md` | REVIEWED AND INTEGRATED | HIGH |
| 13 | `02-schema.md` | 7,363 | `f219e4ec13d5c494` | `B11/analysis/V-CD-2019/02-schema.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 14 | `03-capabilities.md` | 4,668 | `5c3c02292a7080d3` | `B11/analysis/V-CD-2019/03-capabilities.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 15 | `03-capabilities__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda\xfa\x83\xda\xfa\xed].md` | 5,425 | `c904860d0736640d` | `B11/analysis/CUMULATIVE/03-capabilities.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 16 | `04-clinical-rules.md` | 4,200 | `f43ef3c01c08c079` | `B11/analysis/CUMULATIVE/04-clinical-rules.md` | REVIEWED AND INTEGRATED | HIGH |
| 17 | `04-feature-delta.md` | 3,239 | `c2e8f8a5ce40d621` | `B11/analysis/V-CD-2019/04-feature-delta.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 18 | `05-Architecture-and-Dependency-Blueprint.md` | 29,413 | `57f4de9107b7861b` | — | REVIEWED AND INTEGRATED — preferred source: `architecture/OVERVIEW.md` | HIGH |
| 19 | `05-deployment.md` | 4,233 | `6693aa8ccb87ec6a` | `B11/analysis/V-CD-2019/05-deployment.md` | REVIEWED AND INTEGRATED | HIGH |
| 20 | `05-deployment__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڽ\x83\xba\xbd\xed_RealLab-Deliverables-V1.00-and-V2019__extracted_analysis_V-CD-2019].md` | 4,254 | `e84530280f7b98aa` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 21 | `05-workflows.md` | 4,375 | `70ada61a8b700d17` | `B11/analysis/CUMULATIVE/05-workflows.md` | REVIEWED AND INTEGRATED | HIGH |
| 22 | `06-Transformation-Baseline-and-Handover.md` | 27,568 | `a90330e8ab4506d6` | — | REVIEWED AND INTEGRATED — preferred source: `mapping/TRANSFORMATION.md` | HIGH |
| 23 | `06-configuration.md` | 3,583 | `e8e8dc17d9c1afea` | `B11/analysis/CUMULATIVE/06-configuration.md` | REVIEWED AND INTEGRATED | HIGH |
| 24 | `06-security.md` | 1,825 | `72db0505602e997c` | `B11/analysis/V-CD-2019/06-security.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 25 | `07-open-questions.md` | 1,204 | `796c9c6074897bbe` | `B11/analysis/V-CD-2019/07-open-questions.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 26 | `07-security-findings.md` | 2,856 | `8330a50f89484d9b` | `B11/analysis/CUMULATIVE/07-security-findings.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 27 | `08-deployment-installer.md` | 15,906 | `3162d0ff2f903735` | `B11/analysis/V-CD-2019/08-deployment-installer.md` | REVIEWED AND INTEGRATED | HIGH |
| 28 | `08-deployment-installer__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڪ\x83۽\xed].md` | 15,908 | `72bc3387e91fa7bd` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 29 | `08-unknowns.md` | 2,301 | `83eacc77e72532e1` | `B11/analysis/CUMULATIVE/08-unknowns.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 30 | `09-presentation-library.md` | 3,383 | `ecad0227ae88e9e7` | `B11/analysis/CUMULATIVE/09-presentation-library.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 31 | `09-vendor-capability-catalogue.md` | 11,052 | `c625354957390b63` | `B11/analysis/V-CD-2019/09-vendor-capability-catalogue.md` | REVIEWED AND INTEGRATED | HIGH |
| 32 | `10-training-video.md` | 14,414 | `9637cca8368c3576` | `B11/analysis/V-CD-2019/10-training-video.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 33 | `11-deployment-payload-2019.md` | 18,674 | `8681eb3ed193e847` | `B11/analysis/V-CD-2019/11-deployment-payload-2019.md` | REVIEWED AND INTEGRATED | HIGH |
| 34 | `11_pilot_report.md` | 10,686 | `bed249314fa9e85e` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `Docs/KB/legacy/evidence/COVERAGE.md` | MEDIUM |
| 35 | `12-install-media (2).md` | 17,918 | `9decc20025aa941b` | `B11/analysis/V-CD-2019/12-install-media.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 36 | `12-install-media.md` | 17,264 | `171fa1f3a9871fd6` | `B10/analysis/V-CD-2019/12-install-media.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 37 | `13-pdf-print-architecture.md` | 9,642 | `28f13019c876e551` | `B11/analysis/V-CD-2019/13-pdf-print-architecture.md` | REVIEWED AND INTEGRATED | HIGH |
| 38 | `1605-phase2-findings.md` | 16,637 | `e3ac139bba2b7c31` | — | REVIEWED AND INTEGRATED | HIGH |
| 39 | `1605-template-corpus.md` | 11,330 | `7a5c498fc3207af3` | — | REVIEWED AND INTEGRATED — preferred source: `architecture/CONFIGURATION.md` | HIGH |
| 40 | `After completing the fourth stage Handoff.md` | 82,931 | `afd24e4cf60371f3` | — | HISTORICAL VERSION REFERENCE | MEDIUM |
| 41 | `Application Layer.md` | 93,495 | `924459e3484f9382` | — | REVIEWED AND INTEGRATED — preferred source: `architecture/EXECUTABLE.md` | HIGH |
| 42 | `BATCH-01.md` | 10,787 | `d897f840a2131928` | `B11/07-Batch-Ingestion/manifests/BATCH-01.md` | REVIEWED AND INTEGRATED | HIGH |
| 43 | `BATCH-02.md` | 57,807 | `8d504d51d09007df` | `B11/07-Batch-Ingestion/manifests/BATCH-02.md` | REVIEWED AND INTEGRATED | HIGH |
| 44 | `BATCH-03 (2).md` | 4,519 | `4557a5cd629a733a` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 45 | `BATCH-03.md` | 5,406 | `9bffdc0ec2e529a2` | `B11/07-Batch-Ingestion/manifests/BATCH-03.md` | REVIEWED AND INTEGRATED | HIGH |
| 46 | `BATCH-04 (2).md` | 4,941 | `b857a03220133a72` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 47 | `BATCH-04.md` | 5,293 | `db05c5b0a204d72d` | `B11/07-Batch-Ingestion/manifests/BATCH-04.md` | REVIEWED AND INTEGRATED | HIGH |
| 48 | `BATCH-05.md` | 3,100 | `73d2fbb21bb2ec47` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 49 | `BATCH-05__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڮ\x83\xe1\xd2\xed].md` | 3,057 | `bc8f4c4caffdd432` | `B11/07-Batch-Ingestion/manifests/BATCH-05.md` | REVIEWED AND INTEGRATED | HIGH |
| 50 | `BATCH-06.md` | 2,524 | `2be605e31b18a4d0` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 51 | `BATCH-06__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڪ\x83۽\xed].md` | 2,538 | `716b5645b4de5c17` | `B11/07-Batch-Ingestion/manifests/BATCH-06.md` | REVIEWED AND INTEGRATED | HIGH |
| 52 | `BATCH-07 (2).md` | 64,221 | `39c08505408ee37f` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 53 | `BATCH-07.md` | 67,424 | `ecc690362a6c3019` | `B11/07-Batch-Ingestion/manifests/BATCH-07.md` | REVIEWED AND INTEGRATED | HIGH |
| 54 | `BATCH-08.md` | 7,583 | `3db2a02fcbd2ffb0` | `B11/07-Batch-Ingestion/manifests/BATCH-08.md` | REVIEWED AND INTEGRATED | HIGH |
| 55 | `BATCH-09.md` | 4,580 | `d8d5866466933c5f` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 56 | `BATCH-09__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڽ\x83\xe1\xd2\xed ݃\xda\xfa\x83\xdb\xd9\xed].md` | 5,394 | `1d9ae5f6db3e55a7` | `B11/07-Batch-Ingestion/manifests/BATCH-09.md` | REVIEWED AND INTEGRATED | HIGH |
| 57 | `BATCH-10 (2).md` | 19,445 | `72c48373feba4455` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 58 | `BATCH-10.md` | 20,568 | `3192fa87ffdeffac` | `B11/07-Batch-Ingestion/manifests/BATCH-10.md` | REVIEWED AND INTEGRATED | HIGH |
| 59 | `BATCH-11.md` | 3,901 | `3ab082e19bfd05ac` | `B09/07-Batch-Ingestion/manifests/BATCH-11.md` | REVIEWED AND INTEGRATED | HIGH |
| 60 | `BATCH-11__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda҃\xbc\xae\xed].md` | 4,041 | `0038ad623cf9409c` | `B11/07-Batch-Ingestion/manifests/BATCH-11.md` | REVIEWED AND INTEGRATED | HIGH |
| 61 | `Batch-Ingestion-Protocol.md` | 12,520 | `d35d874a3b0cf917` | `B11/07-Batch-Ingestion/Batch-Ingestion-Protocol.md` | REVIEWED AND INTEGRATED | HIGH |
| 62 | `Batch1_Analysis_Report.md` | 42,354 | `d379958031e50753` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `the consolidated V-CD-2019 document set` | MEDIUM |
| 63 | `Batch2_Analysis_Report.md` | 52,030 | `e0da6c50e340459e` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `the consolidated V-CD-2019 document set` | MEDIUM |
| 64 | `Batch3_Analysis_Report.md` | 41,941 | `babdeab410dd4462` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `the consolidated V-CD-2019 document set` | MEDIUM |
| 65 | `Business_Rules_V1.md` | 82,164 | `d921f4ed36a2fb81` | — | REVIEWED AND INTEGRATED | HIGH |
| 66 | `Business_Rules_V2.md` | 76,639 | `222ff2c066f62c14` | — | REVIEWED AND INTEGRATED — preferred source: `clinical/RULES.md` | HIGH |
| 67 | `Comparison_Report.md` | 676 | `240c6a981ff5688c` | — | REVIEWED AND INTEGRATED — preferred source: `Docs/KB/legacy/evidence/COVERAGE.md` | HIGH |
| 68 | `Complete_Reverse_Engineering_Reference_V1.md` | 53,573 | `9d19ecd36d648a8c` | — | REVIEWED AND INTEGRATED | HIGH |
| 69 | `Complete_Reverse_Engineering_Reference_V2.md` | 64,404 | `a0dfed20e73e3524` | — | REVIEWED AND INTEGRATED — preferred source: `SPECIFICATION.md` | HIGH |
| 70 | `Consolidated_Report.md` | 37,685 | `1f154a2feee4e536` | — | REVIEWED AND INTEGRATED — preferred source: `SPECIFICATION.md` | HIGH |
| 71 | `Credential-And-Fix-Report_2026-11-01_00-51.md` | 6,088 | `985aaa13ba93891f` | — | NOT TECHNICALLY RELEVANT | MEDIUM |
| 72 | `Data_Model_and_Workflow_Entities_V1.md` | 52,372 | `c9f19dc16d425f46` | — | PARTIALLY INTEGRATED — preferred source: `data/DATABASE.md` | MEDIUM |
| 73 | `Data_Model_and_Workflow_Entities_V2.md` | 40,598 | `17917c4685fb46e8` | — | REVIEWED AND INTEGRATED — preferred source: `data/DATABASE.md` | HIGH |
| 74 | `Deliverables-Validation-Report.md` | 20,249 | `e84c2aebe69b3de6` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `Docs/KB/legacy/evidence/VALIDATION.md` | MEDIUM |
| 75 | `Detecting Version Differences.md` | 46,727 | `af849a0ea4b37fc3` | — | REVIEWED AND INTEGRATED — preferred source: `VERSIONS.md` | HIGH |
| 76 | `Documentation_Quality_Report.md` | 24,889 | `3ff9b4c5694a7ad9` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `Docs/KB/legacy/evidence/VALIDATION.md` | MEDIUM |
| 77 | `Documentation_Traceability_Index.md` | 15,570 | `1424ed08fcb2e5c1` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `Docs/KB/legacy/evidence/SOURCES.md` | MEDIUM |
| 78 | `Downloaded_Files_Inventory_2026-10-30_03-09-31.md` | 105,730 | `a260a2de8e43a55e` | — | NOT TECHNICALLY RELEVANT | MEDIUM |
| 79 | `EXECUTION_PROGRESS.md` | 15,867 | `6f7d2611c367a6c6` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `Docs/KB/legacy/evidence/COVERAGE.md` | MEDIUM |
| 80 | `Engagement-Baseline.md` | 12,521 | `990dc55a2cc74b5a` | `B11/00-Engagement/Engagement-Baseline.md` | REVIEWED AND INTEGRATED | HIGH |
| 81 | `Engagement-Workflow-Narrative.md` | 30,562 | `c9b4a03fe21e58aa` | `B11/reports/Engagement-Workflow-Narrative.md` | REVIEWED AND INTEGRATED | HIGH |
| 82 | `Environment-Assessment.md` | 11,663 | `5cbdd638eebae654` | `B11/03-Environment/Environment-Assessment.md` | REVIEWED AND INTEGRATED | HIGH |
| 83 | `Evidence-Register.md` | 17,122 | `1a75b3ff6aae1001` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 84 | `Evidence-Register__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڪ\x83۽\xed_RealLab-Deliverables-V1.00-and-V2019__extracted_11-Evidence].md` | 35,209 | `56a422239f5e8630` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 85 | `Evidence-Register__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڮ\x83\xe1\xd2\xed_RealLab-Deliverables-V1.00-and-V2019__extracted_11-Evidence].md` | 30,552 | `fa9fb09df4496217` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 86 | `Evidence-Register__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڽ\x83\xba\xbd\xed_RealLab-Deliverables-V1.00-and-V2019__extracted_11-Evidence].md` | 38,202 | `7b59da3aa0ae9c04` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 87 | `Evidence-Register__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڽ\x83\xe1\xd2\xed ݃\xda\xfa\x83\xdb\xd9\xed_RealLab-Deliverables-B01-B08__extracted_11-Evidence].md` | 43,147 | `605aa9826a5a15bb` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 88 | `Evidence-Register__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xdaу\xba\xb4\xed Ҽ\xae].md` | 54,077 | `31c8dd2c4ab7fc58` | `B11/11-Evidence/Evidence-Register.md` | REVIEWED AND INTEGRATED | HIGH |
| 89 | `Evidence-Register__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda҃\xbc\xae\xed_RealLab-Deliverables-B01-B10__extracted_11-Evidence].md` | 48,785 | `6f8966954e2ef5e8` | `B10/11-Evidence/Evidence-Register.md` | REVIEWED AND INTEGRATED | HIGH |
| 90 | `Evidence-Register__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda\xf3\x83\xbd\xd2\xed_RealLab-Deliverables-B01-B09__extracted_11-Evidence].md` | 45,044 | `9901fab8c42d9082` | `B09/11-Evidence/Evidence-Register.md` | REVIEWED AND INTEGRATED | HIGH |
| 91 | `Evidence-Register__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda\xfa\x83ٴ\xed_RealLab-Cumulative-Deliverables-B01-B02__extracted_11-Evidence].md` | 23,139 | `c5afea71549be610` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 92 | `Evidence-Register__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda\xfa\x83\xda\xfa\xed_RealLab-Cumulative-Deliverables-B01-B02-B03__extracted_11-Evidence].md` | 26,360 | `18dd117593e1fe7a` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 93 | `Evidence_Inventory.md` | 21,657 | `1ccf861e7de17268` | — | NOT TECHNICALLY RELEVANT | MEDIUM |
| 94 | `File-Accountability-Ledger.md` | 11,929 | `9756b798244d6318` | `B11/06-Accountability/File-Accountability-Ledger.md` | REVIEWED AND INTEGRATED | HIGH |
| 95 | `Final-Analysis-Report.md` | 80,912 | `a1d5e6ac4d3ede54` | — | REVIEWED AND INTEGRATED — preferred source: `architecture/EXECUTABLE.md` | HIGH |
| 96 | `Foundational-Phases-01-02-03-Completion-Report.md` | 38,272 | `d31a274a4e276c4f` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `the Phase-0 evidence set in the repository` | MEDIUM |
| 97 | `From-Phase-0-to-3-Handoff.md` | 149,451 | `5caa748d99c8f82e` | — | HISTORICAL VERSION REFERENCE | MEDIUM |
| 98 | `From-Phase-0-to-4-Handoff.md` | 44,032 | `7b7883a2f71458fd` | — | HISTORICAL VERSION REFERENCE | MEDIUM |
| 99 | `Functional_Specification_V1.md` | 79,248 | `65ea9252e1a915af` | — | REVIEWED AND INTEGRATED — preferred source: `VERSIONS.md` | HIGH |
| 100 | `Functional_Specification_V2.md` | 65,753 | `daa5e878e38a1759` | — | PARTIALLY INTEGRATED — preferred source: `SPECIFICATION.md` | MEDIUM |
| 101 | `Functionality_Requirements_Matrix_V1.md` | 89,253 | `21f24c1e9271ff6e` | — | PARTIALLY INTEGRATED — preferred source: `mapping/GAP-ANALYSIS.md` | MEDIUM |
| 102 | `Functionality_Requirements_Matrix_V2.md` | 133,179 | `af99a0056e7a7a9b` | — | PARTIALLY INTEGRATED — preferred source: `mapping/GAP-ANALYSIS.md` | MEDIUM |
| 103 | `Handoff \xf3\xd1ڴ\xda \x83\xda\xd9ԃ\xdb \x83\xdaۮ\xf1Ҵ (Real Lab) ڪ\xba\xdb\xed Top-Lab (1).md` | 31,188 | `e1bfce60fd1c7ab9` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 104 | `Handoff \xf3\xd1ڴ\xda \x83\xda\xd9ԃ\xdb \x83\xdaۮ\xf1Ҵ (Real Lab) ڪ\xba\xdb\xed Top-Lab.md` | 26,764 | `2877f720f05cc3dc` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 105 | `INFRASTRUCTURE.md` | 104,888 | `3f5907d8d8d85cd7` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `the modern .NET codebase inventory, not the legacy reference system` | MEDIUM |
| 106 | `Inventory-Assessment.md` | 15,956 | `b2fbe5b993516481` | `B11/01-Inventory/Inventory-Assessment.md` | REVIEWED AND INTEGRATED | HIGH |
| 107 | `MANIFEST.md` | 5,597 | `2459aecae88dfa92` | — | REVIEWED AND INTEGRATED — preferred source: `Docs/KB/legacy/evidence/SOURCES.md` | HIGH |
| 108 | `MDF-SQL-Fix_2026-11-01_02-41.md` | 8,292 | `c5538a74613539b9` | — | NOT TECHNICALLY RELEVANT | MEDIUM |
| 109 | `NonUnicode-Arabic-Fix_2026-11-01_03-12.md` | 997 | `b8131a89ac374a40` | — | NOT TECHNICALLY RELEVANT | MEDIUM |
| 110 | `Open-Issues-and-Limitations.md` | 13,351 | `28e6328e933fef3c` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 111 | `Open-Issues-and-Limitations__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڮ\x83\xe1\xd2\xed_RealLab-Deliverables-V1.00-and-V2019__extracted_10-Issues].md` | 18,816 | `49c617a464888920` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 112 | `Open-Issues-and-Limitations__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xdaу\xba\xb4\xed Ҽ\xae].md` | 20,307 | `232694cf35e97ff8` | `B11/10-Issues/Open-Issues-and-Limitations.md` | REVIEWED AND INTEGRATED | HIGH |
| 113 | `Open-Issues-and-Limitations__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda҃\xbc\xae\xed_RealLab-Deliverables-B01-B10__extracted_10-Issues].md` | 20,267 | `9b2fc0e253cdc4d3` | `B10/10-Issues/Open-Issues-and-Limitations.md` | REVIEWED AND INTEGRATED | HIGH |
| 114 | `Open-Issues-and-Limitations__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda\xf3\x83\xbd\xd2\xed_RealLab-Deliverables-B01-B09__extracted_10-Issues].md` | 20,042 | `1d192057b514e186` | `B09/10-Issues/Open-Issues-and-Limitations.md` | REVIEWED AND INTEGRATED | HIGH |
| 115 | `Open-Issues-and-Limitations__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda\xfa\x83ٴ\xed_RealLab-Cumulative-Deliverables-B01-B02__extracted_10-Issues].md` | 15,846 | `ad86942b02fd1773` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 116 | `Open-Issues-and-Limitations__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda\xfa\x83\xda\xfa\xed_RealLab-Cumulative-Deliverables-B01-B02-B03__extracted_10-Issues].md` | 17,202 | `20de16fc1cbd6a0c` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 117 | `PROJECT_CONTEXT_AND_TECHNICAL_HANDOVER.md` | 14,551 | `ae902c588baefc05` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `SPECIFICATION.md` | MEDIUM |
| 118 | `Phase-6-Cloud-Verification-Report.md` | 23,179 | `2097468e382cd2dc` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `the Phase-0 evidence set in the repository` | MEDIUM |
| 119 | `README (2).md` | 5,696 | `5b448e6e969b2f0e` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 120 | `README (3).md` | 2,546 | `d151e233ae8eb069` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 121 | `README (4).md` | 5,815 | `eb09be28068fbc30` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 122 | `README (5).md` | 20,074 | `d0d9da89144cdb87` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 123 | `README (6).md` | 8,250 | `43edf4001f3f2403` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 124 | `README (7).md` | 10,361 | `cefa0f0d91ce151b` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 125 | `README (8).md` | 8,021 | `677c1739f1eff421` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 126 | `README (9).md` | 9,552 | `a49dff9082c0b7f6` | `B11/README.md` | REVIEWED AND INTEGRATED | HIGH |
| 127 | `README-APPLY.md` | 30,257 | `a9f7f95075ce97a8` | — | NOT TECHNICALLY RELEVANT | MEDIUM |
| 128 | `README.md` | 13,754 | `55d8453dd4207738` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 129 | `README__[recovered_from_archive_final-integrated-analysis_diagrams].md` | 649 | `6504f77a4ed01a2f` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 130 | `README__[recovered_from_archive_reports] (2).md` | 9,832 | `4eac8861146a3eea` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 131 | `README__[recovered_from_archive_reports].md` | 9,874 | `46c461ff9487337f` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 132 | `README__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڪ\x83۽\xed_RealLab-Deliverables-V1.00-and-V2019__extracted].md` | 9,147 | `21663d2fc33bc778` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 133 | `README__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڮ\x83\xe1\xd2\xed_RealLab-Deliverables-V1.00-and-V2019__extracted].md` | 8,868 | `6fe360eb34eebc31` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 134 | `README__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڽ\x83\xba\xbd\xed_RealLab-Deliverables-V1.00-and-V2019__extracted].md` | 9,249 | `eec46885dc378e02` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 135 | `README__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڽ\x83\xe1\xd2\xed ݃\xda\xfa\x83\xdb\xd9\xed_RealLab-Deliverables-B01-B08__extracted].md` | 9,365 | `e4de0bb68726bdd7` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 136 | `README__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda҃\xbc\xae\xed_RealLab-Deliverables-B01-B10__extracted].md` | 9,439 | `0eae1523657d5539` | `B10/README.md` | REVIEWED AND INTEGRATED | HIGH |
| 137 | `README__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda\xdc\xddگ_RealLab-Stage2-Deliverables__extracted].md` | 7,659 | `1644f0be6a9ea11c` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 138 | `README__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda\xf3\x83\xbd\xd2\xed_RealLab-Deliverables-B01-B09__extracted].md` | 9,429 | `b1c201960d409186` | `B09/README.md` | REVIEWED AND INTEGRATED | HIGH |
| 139 | `README__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda\xfa\x83ٴ\xed_RealLab-Cumulative-Deliverables-B01-B02__extracted].md` | 8,212 | `454ee147db2e859d` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 140 | `README__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda\xfa\x83\xda\xfa\xed_RealLab-Cumulative-Deliverables-B01-B02-B03__extracted].md` | 8,549 | `832efdb76d48a532` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 141 | `REAL_LAB_COPY_Inventory_Report.md` | 332,551 | `ef4b2c99dc988208` | — | REVIEWED AND INTEGRATED — preferred source: `Docs/KB/legacy/evidence/COVERAGE.md` | HIGH |
| 142 | `REA_Environment_Setup_and_Verification_Report.md` | 44,076 | `f895423aa98f7794` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `the Phase-0 toolchain assessment` | MEDIUM |
| 143 | `REMAINING.md` | 15,873 | `cef3f105facb0fbd` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `Docs/KB/legacy/evidence/UNRESOLVED.md` | MEDIUM |
| 144 | `RE_Mission_Workflow_Plan_v2.md` | 42,762 | `7c36c469336dced3` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `the consolidated V-CD-2019 document set` | MEDIUM |
| 145 | `RL-Phase0-Discovery-Report.md` | 59,387 | `e67da9235bfbdb69` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `the Phase-0 evidence set in the repository` | MEDIUM |
| 146 | `RLS_Final_Reverse_Engineering_Report.md` | 38,848 | `148083a7fc0cad48` | — | REVIEWED AND INTEGRATED — preferred source: `SPECIFICATION.md` | HIGH |
| 147 | `RLS_v1.00_Analysis_Report.md` | 20,666 | `d99c608d9d0fe2a7` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `the consolidated V-CD-2019 document set` | MEDIUM |
| 148 | `RLS_v1.605_Analysis_Report.md` | 22,010 | `49acea5bf689abe9` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `the consolidated V-CD-2019 document set` | MEDIUM |
| 149 | `RL_CD2019Access-v1.605-Reverse-Engineering-Report.md` | 195,176 | `39feeaeb5fd151f0` | — | REVIEWED AND INTEGRATED — preferred source: `SPECIFICATION.md` | HIGH |
| 150 | `RealLabSystem_Inventory_2026-10-30_02-22-38.md` | 100,131 | `9899789874ae4ecc` | — | NOT TECHNICALLY RELEVANT | MEDIUM |
| 151 | `RealLab_Diagnostic_20261101_044628.md` | 73,137 | `a1ce00c7fc1bcf6d` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `the consolidated V-CD-2019 document set` | MEDIUM |
| 152 | `RealLab_Forensic_Inventory_20261101_060408.md` | 553,291 | `d9eda5bfa7fb08a9` | — | NOT TECHNICALLY RELEVANT | MEDIUM |
| 153 | `RealLab_Reverse_Engineering_Master_Handoff.md` | 46,301 | `9a5d538c5d405ce6` | — | HISTORICAL VERSION REFERENCE | MEDIUM |
| 154 | `RealLab_Reverse_Engineering_Master_Handoff_v2.md` | 50,755 | `3463e4bf9ca89a20` | — | HISTORICAL VERSION REFERENCE | MEDIUM |
| 155 | `Reverse-Engineering-Methodology.md` | 14,538 | `1aaeafe2f310583a` | `B11/08-Methodology/Reverse-Engineering-Methodology.md` | REVIEWED AND INTEGRATED | HIGH |
| 156 | `Session-Isolation-Declaration.md` | 14,494 | `d6ee41c018659678` | `B11/00-Engagement/Session-Isolation-Declaration.md` | REVIEWED AND INTEGRATED | HIGH |
| 157 | `Stage-1-Foundation-Report (2).md` | 32,038 | `7621a96f7906c13e` | `B11/reports/Stage-1-Foundation-Report.md` | REVIEWED AND INTEGRATED | HIGH |
| 158 | `Stage-1-Foundation-Report.md` | 32,030 | `4f73f960e8119ce6` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 159 | `Stage-2-V1-Reconnaissance-Report.md` | 22,760 | `7dd5c91e49018a63` | `B11/reports/Stage-2-V1-Reconnaissance-Report.md` | REVIEWED AND INTEGRATED | HIGH |
| 160 | `Stage-2b-Cumulative-Analysis-Report.md` | 35,168 | `3f73dda809b2e760` | `B11/reports/Stage-2b-Cumulative-Analysis-Report.md` | REVIEWED AND INTEGRATED | HIGH |
| 161 | `Stage-2c-Cumulative-Analysis-Report.md` | 20,622 | `89f4a8f6b2304bd1` | `B11/reports/Stage-2c-Cumulative-Analysis-Report.md` | REVIEWED AND INTEGRATED | HIGH |
| 162 | `Stage-3-V-CD-2019-Baseline-Report.md` | 15,876 | `26852366fed75c60` | `B11/reports/Stage-3-V-CD-2019-Baseline-Report.md` | REVIEWED AND INTEGRATED | HIGH |
| 163 | `Stage-4-V-CD-2019-Deployment-Report (2).md` | 16,107 | `994f8fcf1721765d` | `B11/reports/Stage-4-V-CD-2019-Deployment-Report.md` | REVIEWED AND INTEGRATED | HIGH |
| 164 | `Stage-4-V-CD-2019-Deployment-Report.md` | 16,104 | `81a201cdbf4e51d3` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 165 | `Stage-5-V-CD-2019-Cumulative-Report.md` | 15,410 | `09a388a71b77e5d7` | `B11/reports/Stage-5-V-CD-2019-Cumulative-Report.md` | REVIEWED AND INTEGRATED | HIGH |
| 166 | `Stage-6-V-CD-2019-Payload-Report.md` | 18,312 | `2ed2b3714a7adc01` | `B11/reports/Stage-6-V-CD-2019-Payload-Report.md` | REVIEWED AND INTEGRATED | HIGH |
| 167 | `Stage-7-V-CD-2019-Final-Report.md` | 16,332 | `0461c0e6975dbdd6` | `B11/reports/Stage-7-V-CD-2019-Final-Report.md` | REVIEWED AND INTEGRATED | HIGH |
| 168 | `Stage-8-V-CD-2019-Install-Media-Report.md` | 18,740 | `e75e3ba68e4b9a32` | `B11/reports/Stage-8-V-CD-2019-Install-Media-Report.md` | REVIEWED AND INTEGRATED | HIGH |
| 169 | `Stage-9-V-CD-2019-Final-Comprehensive-Report.md` | 23,001 | `7e24f48b0bd16a7c` | `B11/reports/Stage-9-V-CD-2019-Final-Comprehensive-Report.md` | REVIEWED AND INTEGRATED — preferred source: `SPECIFICATION.md` | HIGH |
| 170 | `Stage-Execution-Plan (2).md` | 16,479 | `7f0c8da104535506` | `B11/09-Execution/Stage-Execution-Plan.md` | REVIEWED AND INTEGRATED | HIGH |
| 171 | `Stage-Execution-Plan.md` | 10,924 | `7cebfb9593d3f73c` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 172 | `Stage-Execution-Plan__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڪ\x83۽\xed_RealLab-Deliverables-V1.00-and-V2019__extracted_09-Execution].md` | 14,988 | `9ac8837407d528fb` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 173 | `Stage-Execution-Plan__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڮ\x83\xe1\xd2\xed_RealLab-Deliverables-V1.00-and-V2019__extracted_09-Execution].md` | 13,971 | `d07cf7a005b9417e` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 174 | `Stage-Execution-Plan__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڽ\x83\xba\xbd\xed_RealLab-Deliverables-V1.00-and-V2019__extracted_09-Execution].md` | 15,874 | `b9f3dc7a6a1b08d1` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 175 | `Stage-Execution-Plan__[\xe1Һ \x83ں\xd5\xd2\xed \x83ڽ\x83\xe1\xd2\xed ݃\xda\xfa\x83\xdb\xd9\xed_RealLab-Deliverables-B01-B08__extracted_09-Execution].md` | 16,033 | `a951906056595a5c` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 176 | `Stage-Execution-Plan__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda҃\xbc\xae\xed_RealLab-Deliverables-B01-B10__extracted_09-Execution].md` | 16,383 | `de8cd2518d671d03` | `B10/09-Execution/Stage-Execution-Plan.md` | REVIEWED AND INTEGRATED | HIGH |
| 177 | `Stage-Execution-Plan__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda\xf3\x83\xbd\xd2\xed_RealLab-Deliverables-B01-B09__extracted_09-Execution].md` | 16,223 | `7b965648244c614c` | `B09/09-Execution/Stage-Execution-Plan.md` | REVIEWED AND INTEGRATED | HIGH |
| 178 | `Stage-Execution-Plan__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda\xfa\x83ٴ\xed_RealLab-Cumulative-Deliverables-B01-B02__extracted_09-Execution].md` | 11,911 | `82c0e3430ed72a1f` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 179 | `Stage-Execution-Plan__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda\xfa\x83\xda\xfa\xed_RealLab-Cumulative-Deliverables-B01-B02-B03__extracted_09-Execution].md` | 13,070 | `2d77863dea3e76f0` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 180 | `Storage-Persistence-Assessment.md` | 14,199 | `ae4ad28c0396b2a9` | `B11/05-Storage/Storage-Persistence-Assessment.md` | REVIEWED AND INTEGRATED | HIGH |
| 181 | `Toolchain-Coverage-Matrix.md` | 10,281 | `aed965830047dbd5` | `B11/04-Toolchain/Toolchain-Coverage-Matrix.md` | REVIEWED AND INTEGRATED | HIGH |
| 182 | `Toolchain-Manifest.md` | 12,982 | `dc4599a5681e2ea5` | `B11/04-Toolchain/Toolchain-Manifest.md` | REVIEWED AND INTEGRATED | HIGH |
| 183 | `Unit-1-Reference-Conformance-Report.md` | 246,312 | `3056e3a94b4f770a` | — | REVIEWED AND INTEGRATED — preferred source: `mapping/GAP-ANALYSIS.md` | HIGH |
| 184 | `VALIDATION-REPORT.md` | 2,549 | `77dab30a3ab07d44` | — | REVIEWED AND INTEGRATED — preferred source: `Docs/KB/legacy/evidence/VALIDATION.md` | HIGH |
| 185 | `Version-Identification.md` | 22,310 | `4a3b0875ebb9100c` | `B11/02-Versions/Version-Identification.md` | REVIEWED AND INTEGRATED | HIGH |
| 186 | `Version_Identity_and_Scope.md` | 17,457 | `4fe086372485bb37` | — | REVIEWED AND INTEGRATED — preferred source: `VERSIONS.md` | HIGH |
| 187 | `architecture.md` | 4,617 | `306057acbe0a748e` | `B11/analysis/BATCH-01/architecture.md` | REVIEWED AND INTEGRATED | HIGH |
| 188 | `archive-inventory (2).md` | 10,007 | `5364190276576b58` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 189 | `archive-inventory (3).md` | 8,112 | `cab8a465b9e30f9c` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 190 | `archive-inventory.md` | 9,240 | `9492805232252f98` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 191 | `behavioural-analysis.md` | 9,886 | `4c254bc41df9542f` | — | REVIEWED AND INTEGRATED — preferred source: `SPECIFICATION.md` | HIGH |
| 192 | `cab_comparison.md` | 11,487 | `304b3a8b4872cf1d` | — | REVIEWED AND INTEGRATED — preferred source: `architecture/DEPLOYMENT.md` | HIGH |
| 193 | `capability-crossref.md` | 3,300 | `9a01acedd07d2527` | `B11/analysis/BATCH-01/help/capability-crossref.md` | REVIEWED AND INTEGRATED | HIGH |
| 194 | `capability-crossref__[analysis_BATCH-04_help_v1605].md` | 3,676 | `4762b304fc09fd7b` | `B11/analysis/BATCH-04/help_v1605/capability-crossref.md` | REVIEWED AND INTEGRATED | HIGH |
| 195 | `capability-inventory.md` | 12,268 | `e98d008c5096d5e2` | `B11/analysis/BATCH-01/capability-inventory.md` | REVIEWED AND INTEGRATED | HIGH |
| 196 | `component-reference (2).md` | 14,311 | `54c2021a067da7f6` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 197 | `component-reference (3).md` | 12,729 | `734d9be995a6aefe` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 198 | `component-reference.md` | 15,702 | `39ea1f791198a63e` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 199 | `configuration-and-resources (2).md` | 13,521 | `e07a8b8c164542e5` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 200 | `configuration-and-resources (3).md` | 10,230 | `5784f20f48f067ca` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 201 | `configuration-and-resources.md` | 13,966 | `3b3dede78c221b36` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 202 | `consolidated-system-documentation.md` | 21,599 | `1ebcd7ca647aa405` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `the consolidated V-CD-2019 document set` | MEDIUM |
| 203 | `contradictions-limitations-and-unresolved-questions.md` | 10,135 | `8a0698f3197db4e6` | — | HISTORICAL VERSION REFERENCE | MEDIUM |
| 204 | `coverage-and-limitations (2).md` | 15,062 | `8ee2131fb9062505` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 205 | `coverage-and-limitations (3).md` | 11,217 | `e022b6592aa58a1e` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 206 | `coverage-and-limitations (4).md` | 11,038 | `abe1a054e794a5f2` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 207 | `coverage-and-limitations.md` | 12,979 | `a48ebcd1abea8061` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 208 | `cross-batch-correlation-matrix.md` | 11,415 | `3d159eba5c517be3` | — | HISTORICAL VERSION REFERENCE | MEDIUM |
| 209 | `cross-version-matrix (2).md` | 5,098 | `d000af4c6e0448d9` | `B11/analysis/CUMULATIVE/cross-version-matrix.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 210 | `cross-version-matrix.md` | 5,123 | `df488e7c1b9ab408` | `B09/analysis/CUMULATIVE/cross-version-matrix.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 211 | `cross-version-matrix__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda҃\xbc\xae\xed_RealLab-Deliverables-B01-B10__extracted_analysis_CUMULATIVE].md` | 5,124 | `652adee81289feb1` | `B10/analysis/CUMULATIVE/cross-version-matrix.md` | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 212 | `data-flow-and-control-flow (2).md` | 13,728 | `4b60fed5707c0ad8` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 213 | `data-flow-and-control-flow (3).md` | 13,108 | `aa2e7b827f8c96e6` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 214 | `data-flow-and-control-flow.md` | 14,072 | `798a2697bb4e81f7` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 215 | `data-model.md` | 9,213 | `9717292799aa2bee` | `B11/analysis/BATCH-01/data-model.md` | REVIEWED AND INTEGRATED | HIGH |
| 216 | `database-analysis.md` | 4,515 | `1f95c353562f01b7` | — | REVIEWED AND INTEGRATED — preferred source: `data/DATABASE.md` | HIGH |
| 217 | `database-and-data-access-analysis.md` | 11,757 | `05882c848a4820ee` | — | REVIEWED AND INTEGRATED — preferred source: `data/DATABASE.md` | HIGH |
| 218 | `database-engineering-reference.md` | 9,892 | `2afc3876135df32c` | — | REVIEWED AND INTEGRATED — preferred source: `data/DATABASE.md` | HIGH |
| 219 | `db-investigation-report v 1.md` | 21,310 | `f481e58aea0cd1ef` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `data/DATABASE.md` | MEDIUM |
| 220 | `db-investigation-report v 2.md` | 20,577 | `d83b153dac5e194d` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `data/DATABASE.md` | MEDIUM |
| 221 | `db-summary.md` | 8,669 | `bcf55488b5e19f3e` | `B11/analysis/BATCH-02/db/db-summary.md` | REVIEWED AND INTEGRATED | HIGH |
| 222 | `deliverables-index (2).md` | 12,040 | `ca3f3fa9f3ebc4aa` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 223 | `deliverables-index (3).md` | 10,875 | `309f45f02f165d41` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 224 | `deliverables-index.md` | 11,397 | `cb75e30813450b7c` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 225 | `dependency-analysis (2).md` | 8,546 | `9eabf23cc8ed0cc3` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 226 | `dependency-analysis (3).md` | 6,805 | `ce4bc83e98807857` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 227 | `dependency-analysis.md` | 7,639 | `7003da2f25542c05` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 228 | `dependency-and-deployment-register.md` | 8,058 | `7c05bae2ca30f62f` | — | REVIEWED AND INTEGRATED — preferred source: `architecture/OVERVIEW.md` | HIGH |
| 229 | `domain.md` | 35,832 | `c0ccba3980d81d84` | — | REVIEWED AND INTEGRATED — preferred source: `data/DATABASE.md` | HIGH |
| 230 | `evidence-index (2).md` | 17,493 | `3b7d07ff8e8aa864` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 231 | `evidence-index (3).md` | 11,808 | `28f1b8498bfb1a46` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 232 | `evidence-index (4).md` | 6,870 | `643060c99cba390a` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 233 | `evidence-index.md` | 22,299 | `bf62e1c12bd0c42d` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 234 | `evidence-inventory-and-traceability.md` | 11,684 | `e5709a87db9563e6` | — | NOT TECHNICALLY RELEVANT | MEDIUM |
| 235 | `executive-summary (2).md` | 12,352 | `382916037605f25a` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 236 | `executive-summary (3).md` | 10,000 | `14ae0df00b5c7a1b` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 237 | `executive-summary (4).md` | 9,688 | `1eda936ec216e2ed` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 238 | `executive-summary.md` | 11,469 | `0000f89a0f455451` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 239 | `executive-technical-overview (2).md` | 10,697 | `cddc18e0ebff5c72` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 240 | `executive-technical-overview.md` | 9,823 | `28297ad08b4e681a` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 241 | `file-by-file-analysis (2).md` | 54,644 | `544bbad63c4eacc2` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 242 | `file-by-file-analysis (3).md` | 77,471 | `700157b8a378d4c7` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 243 | `file-by-file-analysis.md` | 232,715 | `46d7e50092d6ab6b` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 244 | `file-inventory.md` | 10,131 | `5eff044d1159066c` | — | NOT TECHNICALLY RELEVANT | MEDIUM |
| 245 | `final-handover.md` | 11,321 | `9c740edb59e63f78` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `Docs/KB/legacy/evidence/COVERAGE.md` | MEDIUM |
| 246 | `finding-register.md` | 22,182 | `8616ea4232df1e25` | — | REVIEWED AND INTEGRATED — preferred source: `Docs/KB/legacy/evidence/SOURCES.md` | HIGH |
| 247 | `findings-register (2).md` | 33,317 | `cc0e5d3bfd119999` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 248 | `findings-register (3).md` | 11,724 | `dc6e538860975755` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 249 | `findings-register.md` | 19,531 | `329f531e4880007c` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 250 | `functional-module-catalogue.md` | 8,434 | `fe4461b6f9c26b6b` | — | REVIEWED AND INTEGRATED — preferred source: `business/CAPABILITIES.md` | HIGH |
| 251 | `functional-module-register.md` | 19,306 | `f3ee535e259661c5` | — | REVIEWED AND INTEGRATED — preferred source: `business/CAPABILITIES.md` | HIGH |
| 252 | `help-manual-ar (2).md` | 84,731 | `c8b8cafdef019e0c` | `B11/analysis/BATCH-01/help/help-manual-ar.md` | REVIEWED AND INTEGRATED | HIGH |
| 253 | `help-manual-ar.md` | 84,671 | `428f430ad5da0676` | — | REVIEWED AND INTEGRATED — preferred source: `SPECIFICATION.md` | HIGH |
| 254 | `ingest-report (2).md` | 2,402 | `0da281a0f5bf77cf` | `B11/batches/BATCH-03/ingest-report.md` | REVIEWED AND INTEGRATED | HIGH |
| 255 | `ingest-report.md` | 6,621 | `1dd89cf946b7c47e` | `B11/batches/BATCH-01/ingest-report.md` | REVIEWED AND INTEGRATED | HIGH |
| 256 | `ingest-report__[batches_BATCH-04].md` | 2,666 | `9752d81bf137e15d` | `B11/batches/BATCH-04/ingest-report.md` | REVIEWED AND INTEGRATED | HIGH |
| 257 | `ingest-report__[batches_BATCH-05].md` | 1,311 | `c69c0b34b7e8c52e` | `B11/batches/BATCH-05/ingest-report.md` | REVIEWED AND INTEGRATED | HIGH |
| 258 | `ingest-report__[batches_BATCH-06].md` | 883 | `41908b2c048a8a96` | `B11/batches/BATCH-06/ingest-report.md` | REVIEWED AND INTEGRATED | HIGH |
| 259 | `ingest-report__[batches_BATCH-07].md` | 47,838 | `38450d1ea31ea3ce` | `B11/batches/BATCH-07/ingest-report.md` | REVIEWED AND INTEGRATED | HIGH |
| 260 | `ingest-report__[batches_BATCH-08].md` | 4,846 | `d2ebf557cbf7cc79` | `B11/batches/BATCH-08/ingest-report.md` | REVIEWED AND INTEGRATED | HIGH |
| 261 | `ingest-report__[batches_BATCH-09].md` | 2,610 | `7d86b541e2b732d9` | `B11/batches/BATCH-09/ingest-report.md` | REVIEWED AND INTEGRATED | HIGH |
| 262 | `ingest-report__[\xe1Һ \x83ں\xd5\xd2\xed \x83\xda\xfa\x83ٴ\xed].md` | 39,710 | `5c1da028a9acec71` | `B11/batches/BATCH-02/ingest-report.md` | REVIEWED AND INTEGRATED | HIGH |
| 263 | `ini-diff-summary.md` | 5,242 | `bce81763a3fa780e` | `B11/analysis/BATCH-07/ini-diff-summary.md` | REVIEWED AND INTEGRATED | HIGH |
| 264 | `installation-and-deployment-reference.md` | 11,130 | `693c7d4de67b79c3` | — | REVIEWED AND INTEGRATED — preferred source: `architecture/DEPLOYMENT.md` | HIGH |
| 265 | `master-engineering-reference.md` | 19,709 | `e4b9bc966581fddb` | — | REVIEWED AND INTEGRATED — preferred source: `architecture/OVERVIEW.md` | HIGH |
| 266 | `methodology-and-tools (2).md` | 8,810 | `9cfa6162bd2819dc` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 267 | `methodology-and-tools (3).md` | 12,775 | `2297f856a4310215` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 268 | `methodology-and-tools.md` | 11,042 | `0e4082ef63d613f2` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 269 | `migration-proof.md` | 9,020 | `319e7197ee8daeec` | `B11/analysis/BATCH-07/migration-proof.md` | REVIEWED AND INTEGRATED | HIGH |
| 270 | `pdf-print-stack.md` | 22,346 | `cd9c82b1dfbbd981` | — | REVIEWED AND INTEGRATED — preferred source: `architecture/PDF-PRINTING.md` | HIGH |
| 271 | `pe-envelope.md` | 9,396 | `adb2c95df31a2256` | `B11/analysis/BATCH-11/pe-envelope.md` | REVIEWED AND INTEGRATED | HIGH |
| 272 | `primary-executable-reverse-engineering.md` | 18,070 | `0d16ecec810532b6` | — | REVIEWED AND INTEGRATED — preferred source: `architecture/EXECUTABLE.md` | HIGH |
| 273 | `primary-executable-technical-reference.md` | 15,523 | `8826ffac43ef5072` | — | REVIEWED AND INTEGRATED — preferred source: `architecture/EXECUTABLE.md` | HIGH |
| 274 | `project-evidence-and-source-inventory.md` | 11,899 | `eeba09bf611a1a31` | — | NOT TECHNICALLY RELEVANT | MEDIUM |
| 275 | `reference_interfac.md` | 40,277 | `6154498a1b4d4f44` | — | REVIEWED AND INTEGRATED — preferred source: `architecture/OVERVIEW.md` | HIGH |
| 276 | `report-animation-library.md` | 9,498 | `9f79ed07d004658f` | `B10/analysis/BATCH-03/flash/report-animation-library.md` | REVIEWED AND INTEGRATED | HIGH |
| 277 | `report-structure.md` | 7,171 | `71ae5008a5dae3a8` | `B11/analysis/BATCH-02/rtf/report-structure.md` | REVIEWED AND INTEGRATED | HIGH |
| 278 | `rl-setup-analysis.md` | 3,521 | `538edd3cdb2eb036` | — | REVIEWED AND INTEGRATED — preferred source: `architecture/DEPLOYMENT.md` | HIGH |
| 279 | `security-and-technical-risk-register.md` | 10,387 | `32a17c774402bedc` | — | REVIEWED AND INTEGRATED — preferred source: `business/SECURITY.md` | HIGH |
| 280 | `security-findings-and-risk-register.md` | 19,908 | `6569bedb2905cf20` | — | REVIEWED AND INTEGRATED — preferred source: `business/SECURITY.md` | HIGH |
| 281 | `security-review (2).md` | 35,464 | `82836c365132c396` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 282 | `security-review (3).md` | 35,940 | `478f92016e31069a` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 283 | `security-review (4).md` | 29,285 | `578da81e08b0c9c3` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 284 | `security-review.md` | 32,467 | `4182a15da88dc301` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 285 | `software-architecture (2).md` | 26,347 | `04a77909d863c29b` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 286 | `software-architecture (3).md` | 25,405 | `bb69c5dc21a1839b` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 287 | `software-architecture (4).md` | 16,745 | `759a60b524ec35f1` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 288 | `software-architecture.md` | 30,488 | `03afeed52aa96698` | — | SUPERSEDED BY A NEWER OR MORE COMPLETE SOURCE | HIGH |
| 289 | `system-architecture.md` | 10,797 | `fccebb959f57bb69` | — | REVIEWED AND INTEGRATED — preferred source: `architecture/OVERVIEW.md` | HIGH |
| 290 | `test-library (2).md` | 7,008 | `a52847aee614efe3` | `B11/analysis/BATCH-02/rtf/test-library.md` | REVIEWED AND INTEGRATED | HIGH |
| 291 | `test-library.md` | 6,969 | `b31f3d733195cd74` | `B10/analysis/BATCH-02/rtf/test-library.md` | REVIEWED AND INTEGRATED | HIGH |
| 292 | `ui-labels-verified.md` | 14,621 | `f5d02bff70436f77` | — | REVIEWED AND INTEGRATED — preferred source: `architecture/CONFIGURATION.md` | HIGH |
| 293 | `unified-application-architecture.md` | 10,189 | `4d69ce2a2a6f66e5` | — | REVIEWED AND INTEGRATED — preferred source: `architecture/OVERVIEW.md` | HIGH |
| 294 | `unified-findings-register.md` | 6,902 | `e6ca1834a019ab3c` | — | REVIEWED AND INTEGRATED — preferred source: `Docs/KB/legacy/evidence/SOURCES.md` | HIGH |
| 295 | `unresolved-questions-and-evidence-gaps.md` | 8,138 | `0c9b73b0eb34414a` | — | HISTORICAL VERSION REFERENCE | MEDIUM |
| 296 | `v1.605_Batch_Generation_Plan_v2.md` | 52,504 | `920c0ae70f1d9ce9` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `the consolidated V-CD-2019 document set` | MEDIUM |
| 297 | `v1.605_Batch_Selection_Plan.md` | 35,061 | `a7cf96d5f293e2cc` | — | ALREADY REPRESENTED BY A BETTER SOURCE — preferred source: `the consolidated V-CD-2019 document set` | MEDIUM |
| 298 | `verification-and-integrity-report.md` | 5,415 | `ccc7ef476b45be49` | — | REVIEWED AND INTEGRATED — preferred source: `Docs/KB/legacy/evidence/VALIDATION.md` | HIGH |
---

## 6. Repository-resident evidence

| Artefact | Location | Disposition |
|---|---|---|
| `Database documentation.md` (209,465 B) | repo root | REVIEWED AND INTEGRATED → `data/DATABASE.md` §3, `data/MODEL.md` |
| `Documenting the Executable File.md` (172,287 B) | repo root | REVIEWED AND INTEGRATED → `architecture/EXECUTABLE.md` |
| `LEGACY_SYSTEM_UNDERSTANDING_REPORT.md` | repo root | REVIEWED AND INTEGRATED → `VERSIONS.md`, `SPECIFICATION.md` |
| `Comprehensive Local Development Environment…Audit.md` | repo root | NOT TECHNICALLY RELEVANT (modern system environment) |
| `evidence/reverse-engineering/exe/exe_findings.md` | repo | REVIEWED AND INTEGRATED → `architecture/EXECUTABLE.md` |
| `evidence/reverse-engineering/exe/tool_log.md`, `README.md` | repo | REVIEWED AND INTEGRATED → `Docs/KB/legacy/evidence/EVIDENCE-REGISTER.md` |
| `evidence/reverse-engineering/reverse-engineering-evidence-register.csv` (36 rows) | repo | REVIEWED AND INTEGRATED → `Docs/KB/legacy/evidence/EVIDENCE-REGISTER.md` |
| `evidence/reverse-engineering/reverse-engineering-file-accountability.csv` (28 rows) | repo | REVIEWED AND INTEGRATED → `Docs/KB/legacy/evidence/EVIDENCE-REGISTER.md` |
| `evidence/reverse-engineering/metadata/export_ledger.csv` (103 rows) | repo | REVIEWED AND INTEGRATED → `data/DATABASE.md` |
| `evidence/reverse-engineering/metadata/pseudonym_map.txt` (18 mappings) | repo | REVIEWED AND INTEGRATED → `Docs/KB/legacy/evidence/EVIDENCE-REGISTER.md` |
| `evidence/reverse-engineering/phase0/*` (18 files: 837-row coverage matrix, 125-row gap register, 5,186-row prior-output ledger) | repo | REVIEWED AND INTEGRATED → `Docs/KB/legacy/evidence/COVERAGE.md`, `Docs/KB/legacy/evidence/VALIDATION.md` |
| `evidence/reverse-engineering/exports/tables/*.json` (103 files) | repo | REVIEWED AND INTEGRATED → `data/DATABASE.md` |
| `ReferenceIndex/RefLab_V2_FILE_INVENTORY.csv` (100 rows) | repo | REVIEWED AND INTEGRATED → `Docs/KB/legacy/evidence/COVERAGE.md` |
| `RefLab_Reference_V2_Consolidated.zip` (12 members) | repo root | REVIEWED AND INTEGRATED → `VERSIONS.md` |
| `Docs/Business_Rules_V2.md`, `Docs/Complete_Reverse_Engineering_Reference_V2.md`, `Docs/Data_Model_and_Workflow_Entities_V2.md`, `Docs/Functional_Specification_V2.md`, `Docs/Functionality_Requirements_Matrix_V2.md` | `Docs/` | REVIEWED AND INTEGRATED → the corresponding canonical documents |
| `Docs/ui-labels-verified.md` | `Docs/` | REVIEWED AND INTEGRATED → `architecture/CONFIGURATION.md` |

---

## 7. Maintenance rule for this register

> Any source added to the corpus or to a deliverables archive **must** get a row here, a review
> status and an explicit integration decision — including the decision *"excluded, because …"*.
> A source that is not in this register is not part of the knowledge base, and any statement in
> the knowledge base that has no row here has **no traceable origin**.

This rule is enforced procedurally by `Docs/KB/legacy/evidence/COVERAGE.md` §5 and by the review checklist in
`MAINTENANCE.md`.

---

## 8. Related documents

- `Docs/KB/legacy/evidence/COVERAGE.md` — what is represented, what improved, what remains open.
- `Docs/KB/legacy/evidence/EVIDENCE-REGISTER.md` — the artefact-level evidence index.
- `Docs/KB/legacy/evidence/CONTRADICTIONS.md` — conflicts and how each was resolved.
- `Docs/KB/legacy/evidence/UNRESOLVED.md` — open questions.
- `Docs/KB/legacy/evidence/VALIDATION.md` — checks performed and their results.
- `Docs/KB/legacy/INDEX.md` — navigation.
