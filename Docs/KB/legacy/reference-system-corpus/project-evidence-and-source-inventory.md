# Project Evidence and Source Inventory — RealLab CD 2019 v1.605

**Consolidated:** 2026-10-31 (UTC) · **Static analysis only.**
Machine-readable: `machine-readable/project-source-inventory.csv` (855 rows) and
`machine-readable/source-inventory-summary.json`.

---

## 1. Discovery method

The persistent workspace `/workspace/reallab-v1.605-engineering` was walked
in full — every directory, every file — and the attachment directories were
searched for the original uploads. **Nothing was requested from the user,
because nothing needed to be.**

```
find /workspace/reallab-v1.605-engineering -type f      → 855 artefacts
find /workspace/attachments -type f                     → 4 original uploads
```

Each artefact was then classified by **area → investigation → file type →
technical purpose**, hashed, and checked against every other artefact for
byte-identity.

---

## 2. Inventory summary

| Metric | Value |
| --- | ---: |
| Artefacts located | **855** |
| Distinct SHA-256 digests | **671** |
| Artefacts with **unique** evidence | **545** |
| Artefacts that duplicate another artefact | 310 |
| Total size | 359,668,756 bytes |

### By investigation

| ID | Investigation | Artefacts | Bytes | File types |
| --- | --- | ---: | ---: | ---: || **B1** | B1 — original investigation (supplied via the handover archive) | 12 | 209,525 | 11 |
| **B2** | B2 — original investigation (supplied via the handover archive) | 24 | 1,019,639 | 22 |
| **B3** | B3 — report layouts, reference library, fonts, graphics, designer assets | 300 | 138,999,033 | 29 |
| **B4** | B4 — installer, VS6 SP6 redistributable, PDF printer, Arabic manual | 443 | 127,372,959 | 37 |
| **EXE** | Primary executable — VB6 structural analysis, PE analysis, string corpus, SQL | 27 | 167,264,871 | 10 |
| **FIN** | Consolidation pass 1 — integrated engineering understanding | 25 | 4,296,357 | 14 |

### By file type

| File type | Count |
| --- | ---: || `bulk-extracted-content` | 251 |
| `other` | 232 |
| `target-asset` | 85 |
| `log-or-text` | 70 |
| `tool` | 36 |
| `target-binary` | 15 |
| `report` | 14 |
| `json-dataset` | 12 |
| `target-font` | 10 |
| `target-script` | 9 |
| `archive` | 8 |
| `target-config` | 8 |
| `readme` | 7 |
| `csv-register` | 6 |
| `pe-analysis` | 6 |
| `findings-register` | 5 |
| `findings-dataset` | 5 |
| `architecture` | 5 |
| `hash-manifest` | 5 |
| `manifest` | 4 |
| `coverage-report` | 4 |
| `evidence-index` | 4 |
| `security-review` | 4 |
| `dependency-analysis` | 4 |
| `target-document` | 4 |
| `archive-inventory` | 3 |
| `component-reference` | 3 |
| `components-dataset` | 3 |
| `configuration-analysis` | 3 |
| `coverage-dataset` | 3 |
| `data-flow` | 3 |
| `deliverables-index` | 3 |
| `dependencies-dataset` | 3 |
| `per-file-analysis` | 3 |
| `methodology` | 3 |
| `inventory-summary` | 2 |
| `correlation-dataset` | 2 |
| `string-corpus` | 2 |
| `vb6-object-dataset` | 2 |
| `chm-analysis` | 1 |
| `chm-toc` | 1 |
| `open-questions` | 1 |
| `project-index` | 1 |

---

## 3. Narrative documents located

**68 narrative documents** across the five investigations — every one read
during this consolidation.

| Investigation | Narrative documents |
| --- | ---: |
| B1 | 8 |
| B2 | 16 |
| B3 | 15 |
| B4 | 16 |
| Consolidation pass 1 | 12 |
| Early cross-batch note | 1 |

---

## 4. Source materials: what each investigation contributes, and what is unique

### 4.1 B1 — the original investigation (supplied, not independently re-investigated)

13 files in `context/B1/`. The **unique contribution** is the data model and
the digest of the primary executable.

| Artefact | Purpose | Verification |
| --- | --- | --- |
| `file-inventory.csv` | 158 rows, 140 distinct digests — **the project's canonical file-identity reference** | re-verified 36/36 with the handover |
| `findings.json` / `finding-register.md` | 19 findings with evidence | inherited |
| `software-architecture.md` | the data model, the Access/Jet schema, the discipline tables | inherited |
| `analysis-manifest.json` | archive identity, environment, artefact hashes | inherited |
| `security-review.md`, `behavioural-analysis.md`, `coverage-and-limitations.md`, `evidence-index.md`, `README.md` | the supporting analysis | inherited |

### 4.2 B2 — the original investigation (supplied, not independently re-investigated)

26 files in `context/B2/`. The **unique contribution** is the platform
inventory.

| Artefact | Purpose | Verification |
| --- | --- | --- |
| `file-by-file-analysis.md` | per-file analysis of all **70** B2 files | inherited |
| `file-hashes.sha256` | 70 digests — the platform's file-identity reference | re-verified with the handover |
| `components.json` / `component-reference.md` | 45 components with version and signing status | inherited |
| `findings.json` / `findings-register.md` | 13 findings | inherited |
| `v1.605-B2-analysis-package.tar.gz`, `v1.605-B2-reports.tar.gz` | the original B2 delivery packages | digests recorded |
| `coverage.json`, `dependencies.json`, `data-flow-and-control-flow.md`, `database-analysis.md`, `methodology-and-tools.md`, `evidence-index.md`, `archive-inventory.md`, `configuration-and-resources.md`, `security-review.md`, `README.md`, `executive-summary.md` | the supporting analysis | inherited |

### 4.3 B3 — executed by this agent

`batches/B3/` — 251 files analysed, 15 reports, 7 machine-readable artefacts.

| Artefact | Purpose | Verification |
| --- | --- | --- |
| `reports/*.md` (15) | the B3 report package | 24/25 artefacts match B3's own manifest; 1 documented self-reference exception |
| `reports/{components,dependencies,findings,coverage}.json` + `analysis-manifest.json` | machine-readable records | as above |
| `inventory/file-inventory.csv`, `file-hashes.sha256` | 251 rows / 251 digests | 251/251 re-verified |
| `evidence/ini-analysis.json` | the 1,394-key INI vocabulary | — |
| `evidence/rtf-analysis.json`, `chm-*` (n/a — CHM is B4) | the 29-document reference library | — |
| `evidence/rtf-pictures/`, `evidence/emf/` | 6 embedded vector images extracted from the cached RTFs | — |
| `evidence/preview/` | 4 rendered montages used for visual identification | — |
| `source/` | the 251 extracted files | 251/251 hashes re-verified |
| `tools/` | the 5 scripts written for B3 | — |

### 4.4 B4 — executed by this agent

`analysis/v1.605-B4/` — 69 files analysed, 15 reports, 8 machine-readable
artefacts, plus 333 files reached inside nested archives.

| Artefact | Purpose | Verification |
| --- | --- | --- |
| `reports/*.md` (15) | the B4 report package | present and read |
| `reports/*.json` (5) + `analysis-manifest.json` | machine-readable records | present and read |
| `file-inventory.csv`, `file-hashes.sha256` | 69 rows / 69 digests | 69/69 re-verified |
| `evidence/chm-analysis.json`, `chm-toc.json`, `chm-topics.txt` | the **recovered Arabic user manual** — 17 topics, 79 images, full text | present and read |
| `evidence/font-dsig-analysis.txt` | the Microsoft DSIG / PKCS#7 analysis behind B3's font finding | present and read |
| `evidence/cross-batch-identity.json` | the B4 ↔ B2/B3 digest correlation | present and read |
| `evidence/pe-analysis.json`, `pe-bullzip.json` | the PE analyses of `acmsetup.exe` and the Bullzip installer | present and read |
| `extracted/`, `extracted-cab/`, `extracted-chm/`, `extracted-inno/` | 69 batch files + 333 nested-archive members | hashed for the correlation matrix |
| `tools/` | 8 scripts written for B4 | — |

### 4.5 The primary executable investigation — executed by this agent

`executable-analysis/` — the deepest single-body of evidence in the project.

| Artefact | Purpose | Verification |
| --- | --- | --- |
| `extracted/real lab system.exe` | **the primary executable**, 76,292,096 B | SHA-256 **identical** to B1's recorded digest |
| `evidence/vb6-objects.json` | the VB6 header and the **212-object** table with method counts | regenerated and re-verified |
| `evidence/strings.json` | the **31,429-string** corpus with categories and file offsets | regenerated and re-verified |
| `evidence/pe-structure.json` | PE headers, sections, imports, hardening | regenerated |
| `evidence/pe-resources.json`, `icon-embedded.png` | the 12 resource entries and the extracted PNG icon | regenerated |
| `tools/` | 10 scripts: VB6 object parser, string extractor, PE parser, cross-batch correlators, register builders | — |
| `ghidra-out/` | the Ghidra project and logs — **the analysis was OOM-killed** | the honest negative result |

### 4.6 Consolidation pass 1 — this project's own earlier integration

`final-integrated-analysis/` — 10 reports and 6 registers integrating the
executable with B1–B4. **Retained, not superseded**: the present package
extends it, and its `primary-executable-reverse-engineering.md` carries the
correction of the Ghidra outcome that this consolidation confirms.

---

## 5. Original uploads — all four present, all unmodified

| Upload | Size | SHA-256 |
| --- | ---: | --- |
| `real lab system.zip` | 10,490,454 | `17a266e0d6e68b07513bf66366d8c0f4c47c9ecdbf6d7679f85d026cea734bb6` |
| `v1.605-B3.zip` | 3,523,212 | `2fb942c377d1e06b6ba473302e3ad8e8ba8a6faa39513bb9a296a73d64ee3a7b` |
| `v1.605-B4.zip` | 63,839,512 | `7aff65e12e5334e6ebaad6f1ab19965d665a9d4ec968c396ed9b2f294f5e4a6c` |
| `RealLab-v1.605-Engineering-Handover.zip` | 864,737 | `ed81ff46a28f3c8441234019eb391deeef77365987813aad5c05ba6c9cb27f0f` |

**The B1 target archive is the one original upload that is not present.**
B1's *analysis package* survives, and its `file-inventory.csv` preserves the
SHA-256 of every file the B1 archive contained — which is how the executable's
identity was confirmed. The archive itself (28,314,816 bytes, SHA-256
`aedb61cae4721e7eded3c9073a32e1539f2d822b6a4230636a9b552fa5ba3a78`) is
**not** in the workspace or the attachment directories. Recorded as
unavailable; **not reconstructed from assumption**.

---

## 6. Uniqueness analysis

**545 of 855 artefacts carry evidence unique to them.** The 310 duplicates
fall into three groups:

1. **Structural mirroring** — B3 and B4 each ship two trees (root and
   `RL_CD2019Access/`) whose files are byte-identical. 179 B3 files and 6 B4
   files are in duplicate groups.
2. **Cross-investigation identity** — the 33 digests that appear in more than
   one investigation's digest set (the B2↔B4-derived components, the
   B3↔B4-derived fonts, the B1↔B3 template prototypes, the B1↔executable
   binary). See `cross-batch-correlation-matrix.md`.
3. **Delivery packaging** — the same report appears inside the per-batch
   directory *and* inside the batch's `.tar.gz`.

**Nothing unique was discarded.** Where a later investigation supersedes an
earlier one, both records are retained and the relationship is recorded.

---

## 7. Machine-readable registers produced by this consolidation

| File | Rows | Content |
| --- | ---: | --- |
| `machine-readable/project-source-inventory.csv` | 855 | every artefact with path, investigation, type, purpose, size, digest, verification and uniqueness |
| `machine-readable/source-inventory-summary.json` | — | counts and breakdowns |
| `machine-readable/cross-batch-correlation-matrix.csv` | every digest | which investigations share each digest |
| `machine-readable/cross-batch-correlation-matrix.json` | — | pairwise counts and the shared-file detail |
| `machine-readable/unified-findings-register.csv` | 41 | unified subjects with their original identifiers and origins |
| `machine-readable/unified-findings-register.json` | — | the same, with every per-origin record retained in full |
| `machine-readable/unified-unresolved-questions.csv` | 17 | the consolidated open-question register |
| `machine-readable/functional-module-catalogue.csv` | 212 | the VB6 object register with family classification |
