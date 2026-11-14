# RefLab V2 Complete Index

Generated: 2026-11-13 15:01:05

## 1. Scope and Sources

- Archive 1 (Patches): `C:\Users\LAP LINK\OneDrive\Desktop\batches\Reference_V2_Patches_04-11.zip`
- Archive 2 (Consolidated): `C:\Users\LAP LINK\OneDrive\Desktop\batches\RefLab_Reference_V2_Consolidated.zip`
- Output directory: `C:\Users\LAP LINK\OneDrive\Desktop\ReferenceIndex`
- Sidecar open-items map: `C:\Users\LAP LINK\OneDrive\Desktop\ReferenceIndex\RefLab_V2_OPEN_ITEMS_MAP.csv`
- No batch classification is produced. Files are identified by ArchiveName, FolderOrdinal, and FolderNameDecoded.
- **Navigation guidance:** always use `StableExtractionPath` (`Archive1/OrdN/FileName` or `Archive2/FileName`) to locate files. Do **not** use `EntryPath` when it contains non-recoverable characters.
- Nested ZIPs (e.g. `RealLab-Deliverables-*.zip`) are indexed as binary files only.

## 2. Statistical Summary

| Metric | Value |
|---|---:|
| Total files indexed | 100 |
| Patches files | 91 |
| Consolidated files | 9 |
| Distinct SHA-256 contents | 76 |
| Singleton files | 62 |
| Files in duplicate groups | 38 |
| Duplicate groups | 14 |
| Canonical references | 14 |
| Redundant duplicate copies | 24 |
| Markdown documents | 70 |
| Files containing at least one identifier | 30 |
| Files with raw-byte corruption (name recovered externally) | 91 |

## 3. Folder Discovery

The raw bytes of the seven top-level folder names in Archive 1 are corrupted at the archive level (before compression), so no codepage can recover them. The correct Arabic names have been verified externally — each folder is anchored to a unique Stage report it contains — and stored in the CSV column `FolderNameRecovered`.

**Verification table** (each ordinal is anchored to a specific Stage report whose presence proves the folder's identity):

| FolderOrdinal | Verification file (must be present) | FolderNameRecovered |
|---:|---|---|
| 1 | `Stage-7-V-CD-2019-Final-Report.md` | بعد الدفعة التاسعة |
| 2 | `Stage-9-V-CD-2019-Final-Comprehensive-Report.md` | بعد الدفعة الحادية عشر |
| 3 | `Stage-4-V-CD-2019-Deployment-Report.md` | بعد الدفعة الخامسة |
| 4 | `Stage-3-V-CD-2019-Baseline-Report.md` | بعد الدفعة الرابعة |
| 5 | `Stage-6-V-CD-2019-Payload-Report.md` | بعد الدفعة السابعة والثامنة |
| 6 | `Stage-5-V-CD-2019-Cumulative-Report.md` | بعد الدفعة السادسة |
| 7 | `Stage-8-V-CD-2019-Install-Media-Report.md` | بعد الدفعة العاشرة |

Columns in the CSV:
- `FolderNameRaw` — corrupted raw bytes (Latin-1 round-trip), preserved for forensic reference
- `FolderNameDecoded` — best-effort decode attempt (may still be corrupted)
- `FolderNameRecovered` — verified correct Arabic name (authoritative for human-readable identification)
- `NameDecodedWith` — "NotRecoverable" for all 7 folders (raw bytes cannot be decoded)
- `NameDecodedSuccessfully` — "False" for all 7 folders (honest)
- `StableExtractionPath` — valid regardless of name state (use for navigation)

Navigation guidance: use `StableExtractionPath` (e.g. `Archive1/Ord4/Stage-3-V-CD-2019-Baseline-Report.md`) for tool-level navigation. Use `FolderNameRecovered` for human-readable display.

## 4. File Distribution by Folder

| FolderOrdinal | File Count |
|---:|---:|
| 1 | 19 |
| 2 | 12 |
| 3 | 12 |
| 4 | 14 |
| 5 | 13 |
| 6 | 15 |
| 7 | 6 |
| (Consolidated) | 9 |

## 5. Complete File Inventory

### Archive 1 - Patches

#### FolderOrdinal 1 - ` ã§ é§åã¡ é¢«ã¡`

- `Archive1/Ord1/00-baseline-summary.md` (6996 B) [CANONICAL DUP-0001]
- `Archive1/Ord1/01-architecture.md` (4495 B) [CANONICAL DUP-0002]
- `Archive1/Ord1/02-schema.md` (7363 B) [CANONICAL DUP-0003]
- `Archive1/Ord1/03-capabilities.md` (4668 B) [CANONICAL DUP-0004]
- `Archive1/Ord1/04-feature-delta.md` (3239 B) [CANONICAL DUP-0005]
- `Archive1/Ord1/05-deployment.md` (4233 B) [CANONICAL DUP-0006]
- `Archive1/Ord1/06-security.md` (1825 B) [CANONICAL DUP-0007]
- `Archive1/Ord1/08-deployment-installer.md` (15906 B) [CANONICAL DUP-0008]
- `Archive1/Ord1/09-vendor-capability-catalogue.md` (11052 B) [CANONICAL DUP-0009]
- `Archive1/Ord1/10-training-video.md` (14414 B) [CANONICAL DUP-0010]
- `Archive1/Ord1/11-deployment-payload-2019.md` (18674 B) [CANONICAL DUP-0011]
- `Archive1/Ord1/BATCH-10-files-Windows-paths.txt` (5430 B)
- `Archive1/Ord1/BATCH-10.md` (20568 B)
- `Archive1/Ord1/cross-version-matrix.csv` (243789 B)
- `Archive1/Ord1/cross-version-matrix.md` (5123 B)
- `Archive1/Ord1/ini-diff-summary.md` (5242 B) [CANONICAL DUP-0012]
- `Archive1/Ord1/migration-proof.md` (9020 B) [CANONICAL DUP-0013]
- `Archive1/Ord1/RealLab-Deliverables-B01-B09.zip` (12072996 B)
- `Archive1/Ord1/Stage-7-V-CD-2019-Final-Report.md` (16332 B)

#### FolderOrdinal 2 - ` ã§ é§åã¡ é¥§ï¡ ã¬©`

- `Archive1/Ord2/13-pdf-print-architecture.md` (9642 B)
- `Archive1/Ord2/cab-inventory.csv` (11404 B)
- `Archive1/Ord2/Engagement-Workflow-Narrative.md` (30562 B)
- `Archive1/Ord2/Evidence-Register.md` (54077 B)
- `Archive1/Ord2/ledger.csv` (320989 B)
- `Archive1/Ord2/Open-Issues-and-Limitations.md` (20307 B)
- `Archive1/Ord2/pdf-print-stack.md` (22346 B)
- `Archive1/Ord2/pe-envelope.csv` (2502 B)
- `Archive1/Ord2/pe-envelope.md` (9396 B)
- `Archive1/Ord2/RealLab-Deliverables-B01-B11.zip` (12091120 B)
- `Archive1/Ord2/Stage-9-V-CD-2019-Final-Comprehensive-Report.md` (23001 B)
- `Archive1/Ord2/strings-reallab.csv` (1471 B)

#### FolderOrdinal 3 - ` ã§ é§åã¡ é¦ê«¡`

- `Archive1/Ord3/00-baseline-summary.md` (6143 B) [CANONICAL DUP-0014]
- `Archive1/Ord3/03-capabilities.md` (4668 B) [dup-of Archive1/Ord1/03-capabilities.md]
- `Archive1/Ord3/05-deployment.md` (4233 B) [dup-of Archive1/Ord1/05-deployment.md]
- `Archive1/Ord3/06-security.md` (1825 B) [dup-of Archive1/Ord1/06-security.md]
- `Archive1/Ord3/08-deployment-installer.md` (15908 B)
- `Archive1/Ord3/09-vendor-capability-catalogue.md` (11052 B) [dup-of Archive1/Ord1/09-vendor-capability-catalogue.md]
- `Archive1/Ord3/BATCH-06-files-Windows-paths.txt` (50 B)
- `Archive1/Ord3/BATCH-06.md` (2538 B)
- `Archive1/Ord3/learn_toc.jpg` (213440 B)
- `Archive1/Ord3/RealLab-Deliverables-V1.00-and-V2019.zip` (7303814 B)
- `Archive1/Ord3/RLS_Setup.txt` (6559 B)
- `Archive1/Ord3/Stage-4-V-CD-2019-Deployment-Report.md` (16104 B)

#### FolderOrdinal 4 - ` ã§ é§åã¡ é© ã¡`

- `Archive1/Ord4/00-baseline-summary.md` (3984 B)
- `Archive1/Ord4/01-architecture.md` (4495 B) [dup-of Archive1/Ord1/01-architecture.md]
- `Archive1/Ord4/02-schema.md` (7363 B) [dup-of Archive1/Ord1/02-schema.md]
- `Archive1/Ord4/03-capabilities.md` (4668 B) [dup-of Archive1/Ord1/03-capabilities.md]
- `Archive1/Ord4/04-feature-delta.md` (3239 B) [dup-of Archive1/Ord1/04-feature-delta.md]
- `Archive1/Ord4/05-deployment.md` (4233 B) [dup-of Archive1/Ord1/05-deployment.md]
- `Archive1/Ord4/06-security.md` (1825 B) [dup-of Archive1/Ord1/06-security.md]
- `Archive1/Ord4/BATCH-05-files-Windows-paths.txt` (197 B)
- `Archive1/Ord4/BATCH-05.md` (3057 B)
- `Archive1/Ord4/column-evolution.csv` (14570 B)
- `Archive1/Ord4/ddl-all.txt` (22216 B)
- `Archive1/Ord4/RealLab-Deliverables-V1.00-and-V2019.zip` (6716814 B)
- `Archive1/Ord4/Stage-3-V-CD-2019-Baseline-Report.md` (15876 B)
- `Archive1/Ord4/version-baseline.csv` (373 B)

#### FolderOrdinal 5 - ` ã§ é§åã¡ é« ã¡ íé£êë¡`

- `Archive1/Ord5/00-baseline-summary.md` (6996 B) [dup-of Archive1/Ord1/00-baseline-summary.md]
- `Archive1/Ord5/02-schema.md` (7363 B) [dup-of Archive1/Ord1/02-schema.md]
- `Archive1/Ord5/10-training-video.md` (14414 B) [dup-of Archive1/Ord1/10-training-video.md]
- `Archive1/Ord5/11-deployment-payload-2019.md` (18674 B) [dup-of Archive1/Ord1/11-deployment-payload-2019.md]
- `Archive1/Ord5/BATCH-09-files-Windows-paths.txt` (651 B)
- `Archive1/Ord5/BATCH-09.md` (5394 B)
- `Archive1/Ord5/branding-compare.jpg` (37826 B)
- `Archive1/Ord5/ini-diff-summary.md` (5242 B) [dup-of Archive1/Ord1/ini-diff-summary.md]
- `Archive1/Ord5/ini-key-delta.csv` (166496 B)
- `Archive1/Ord5/migration-proof.md` (9020 B) [dup-of Archive1/Ord1/migration-proof.md]
- `Archive1/Ord5/RealLab-Deliverables-B01-B08.zip` (12005405 B)
- `Archive1/Ord5/Stage-6-V-CD-2019-Payload-Report.md` (18312 B)
- `Archive1/Ord5/v2019-Patients.mdb.sql` (47464 B)

#### FolderOrdinal 6 - ` ã§ é§åã¡ é«§«¡`

- `Archive1/Ord6/00_overview.jpg` (954746 B)
- `Archive1/Ord6/00-baseline-summary.md` (6143 B) [dup-of Archive1/Ord3/00-baseline-summary.md]
- `Archive1/Ord6/01-architecture.md` (4495 B) [dup-of Archive1/Ord1/01-architecture.md]
- `Archive1/Ord6/02_detail_0020s.jpg` (693467 B)
- `Archive1/Ord6/02-schema.md` (7363 B) [dup-of Archive1/Ord1/02-schema.md]
- `Archive1/Ord6/03-capabilities.md` (4668 B) [dup-of Archive1/Ord1/03-capabilities.md]
- `Archive1/Ord6/06_detail_0100s.jpg` (582108 B)
- `Archive1/Ord6/06-security.md` (1825 B) [dup-of Archive1/Ord1/06-security.md]
- `Archive1/Ord6/08-deployment-installer.md` (15906 B) [dup-of Archive1/Ord1/08-deployment-installer.md]
- `Archive1/Ord6/09-vendor-capability-catalogue.md` (11052 B) [dup-of Archive1/Ord1/09-vendor-capability-catalogue.md]
- `Archive1/Ord6/10-training-video.md` (14414 B) [dup-of Archive1/Ord1/10-training-video.md]
- `Archive1/Ord6/BATCH-07-files-Windows-paths.txt` (17057 B)
- `Archive1/Ord6/BATCH-07.md` (67424 B)
- `Archive1/Ord6/RealLab-Deliverables-V1.00-and-V2019.zip` (11617165 B)
- `Archive1/Ord6/Stage-5-V-CD-2019-Cumulative-Report.md` (15410 B)

#### FolderOrdinal 7 - ` ã§ é§åã¡ éã¬©¡`

- `Archive1/Ord7/12-install-media.md` (17264 B)
- `Archive1/Ord7/BATCH-11-files-Windows-paths.txt` (477 B)
- `Archive1/Ord7/BATCH-11-files.csv` (2010 B)
- `Archive1/Ord7/BATCH-11.md` (4041 B)
- `Archive1/Ord7/RealLab-Deliverables-B01-B10.zip` (26410462 B)
- `Archive1/Ord7/Stage-8-V-CD-2019-Install-Media-Report.md` (18740 B)

### Archive 2 - Consolidated

- `Archive2/00_INDEX.md` (20056 B)
- `Archive2/01_EXECUTIVE_UNDERSTANDING.md` (28738 B)
- `Archive2/02_FUNCTIONAL_SPECIFICATION.md` (270405 B)
- `Archive2/03_BUSINESS_RULES.md` (216742 B)
- `Archive2/04_DATA_MODEL.md` (154789 B)
- `Archive2/05_WORKFLOWS_AND_UI.md` (226068 B)
- `Archive2/06_PERMISSIONS_AND_SECURITY.md` (78981 B)
- `Archive2/07_GAPS_AND_LIMITS.md` (107132 B)
- `Archive2/08_DECISIONS_REQUIRED.md` (52793 B)

## 6. Unique and Duplicate Files

### 6.1 Singletons (62)

- `Archive1/Ord1/BATCH-10-files-Windows-paths.txt`
- `Archive1/Ord1/BATCH-10.md`
- `Archive1/Ord1/cross-version-matrix.csv`
- `Archive1/Ord1/cross-version-matrix.md`
- `Archive1/Ord1/RealLab-Deliverables-B01-B09.zip`
- `Archive1/Ord1/Stage-7-V-CD-2019-Final-Report.md`
- `Archive1/Ord2/13-pdf-print-architecture.md`
- `Archive1/Ord2/cab-inventory.csv`
- `Archive1/Ord2/Engagement-Workflow-Narrative.md`
- `Archive1/Ord2/Evidence-Register.md`
- `Archive1/Ord2/ledger.csv`
- `Archive1/Ord2/Open-Issues-and-Limitations.md`
- `Archive1/Ord2/pdf-print-stack.md`
- `Archive1/Ord2/pe-envelope.csv`
- `Archive1/Ord2/pe-envelope.md`
- `Archive1/Ord2/RealLab-Deliverables-B01-B11.zip`
- `Archive1/Ord2/Stage-9-V-CD-2019-Final-Comprehensive-Report.md`
- `Archive1/Ord2/strings-reallab.csv`
- `Archive1/Ord3/08-deployment-installer.md`
- `Archive1/Ord3/BATCH-06-files-Windows-paths.txt`
- `Archive1/Ord3/BATCH-06.md`
- `Archive1/Ord3/learn_toc.jpg`
- `Archive1/Ord3/RealLab-Deliverables-V1.00-and-V2019.zip`
- `Archive1/Ord3/RLS_Setup.txt`
- `Archive1/Ord3/Stage-4-V-CD-2019-Deployment-Report.md`
- `Archive1/Ord4/00-baseline-summary.md`
- `Archive1/Ord4/BATCH-05-files-Windows-paths.txt`
- `Archive1/Ord4/BATCH-05.md`
- `Archive1/Ord4/column-evolution.csv`
- `Archive1/Ord4/ddl-all.txt`
- `Archive1/Ord4/RealLab-Deliverables-V1.00-and-V2019.zip`
- `Archive1/Ord4/Stage-3-V-CD-2019-Baseline-Report.md`
- `Archive1/Ord4/version-baseline.csv`
- `Archive1/Ord5/BATCH-09-files-Windows-paths.txt`
- `Archive1/Ord5/BATCH-09.md`
- `Archive1/Ord5/branding-compare.jpg`
- `Archive1/Ord5/ini-key-delta.csv`
- `Archive1/Ord5/RealLab-Deliverables-B01-B08.zip`
- `Archive1/Ord5/Stage-6-V-CD-2019-Payload-Report.md`
- `Archive1/Ord5/v2019-Patients.mdb.sql`
- `Archive1/Ord6/00_overview.jpg`
- `Archive1/Ord6/02_detail_0020s.jpg`
- `Archive1/Ord6/06_detail_0100s.jpg`
- `Archive1/Ord6/BATCH-07-files-Windows-paths.txt`
- `Archive1/Ord6/BATCH-07.md`
- `Archive1/Ord6/RealLab-Deliverables-V1.00-and-V2019.zip`
- `Archive1/Ord6/Stage-5-V-CD-2019-Cumulative-Report.md`
- `Archive1/Ord7/12-install-media.md`
- `Archive1/Ord7/BATCH-11-files-Windows-paths.txt`
- `Archive1/Ord7/BATCH-11-files.csv`
- `Archive1/Ord7/BATCH-11.md`
- `Archive1/Ord7/RealLab-Deliverables-B01-B10.zip`
- `Archive1/Ord7/Stage-8-V-CD-2019-Install-Media-Report.md`
- `Archive2/00_INDEX.md`
- `Archive2/01_EXECUTIVE_UNDERSTANDING.md`
- `Archive2/02_FUNCTIONAL_SPECIFICATION.md`
- `Archive2/03_BUSINESS_RULES.md`
- `Archive2/04_DATA_MODEL.md`
- `Archive2/05_WORKFLOWS_AND_UI.md`
- `Archive2/06_PERMISSIONS_AND_SECURITY.md`
- `Archive2/07_GAPS_AND_LIMITS.md`
- `Archive2/08_DECISIONS_REQUIRED.md`

### 6.2 Duplicate Groups (14)

#### DUP-0001 (size 2)

- Canonical: `Archive1/Ord1/00-baseline-summary.md`
- Duplicate: `Archive1/Ord5/00-baseline-summary.md`

#### DUP-0002 (size 3)

- Canonical: `Archive1/Ord1/01-architecture.md`
- Duplicate: `Archive1/Ord4/01-architecture.md`
- Duplicate: `Archive1/Ord6/01-architecture.md`

#### DUP-0003 (size 4)

- Canonical: `Archive1/Ord1/02-schema.md`
- Duplicate: `Archive1/Ord4/02-schema.md`
- Duplicate: `Archive1/Ord5/02-schema.md`
- Duplicate: `Archive1/Ord6/02-schema.md`

#### DUP-0004 (size 4)

- Canonical: `Archive1/Ord1/03-capabilities.md`
- Duplicate: `Archive1/Ord3/03-capabilities.md`
- Duplicate: `Archive1/Ord4/03-capabilities.md`
- Duplicate: `Archive1/Ord6/03-capabilities.md`

#### DUP-0005 (size 2)

- Canonical: `Archive1/Ord1/04-feature-delta.md`
- Duplicate: `Archive1/Ord4/04-feature-delta.md`

#### DUP-0006 (size 3)

- Canonical: `Archive1/Ord1/05-deployment.md`
- Duplicate: `Archive1/Ord3/05-deployment.md`
- Duplicate: `Archive1/Ord4/05-deployment.md`

#### DUP-0007 (size 4)

- Canonical: `Archive1/Ord1/06-security.md`
- Duplicate: `Archive1/Ord3/06-security.md`
- Duplicate: `Archive1/Ord4/06-security.md`
- Duplicate: `Archive1/Ord6/06-security.md`

#### DUP-0008 (size 2)

- Canonical: `Archive1/Ord1/08-deployment-installer.md`
- Duplicate: `Archive1/Ord6/08-deployment-installer.md`

#### DUP-0009 (size 3)

- Canonical: `Archive1/Ord1/09-vendor-capability-catalogue.md`
- Duplicate: `Archive1/Ord3/09-vendor-capability-catalogue.md`
- Duplicate: `Archive1/Ord6/09-vendor-capability-catalogue.md`

#### DUP-0010 (size 3)

- Canonical: `Archive1/Ord1/10-training-video.md`
- Duplicate: `Archive1/Ord5/10-training-video.md`
- Duplicate: `Archive1/Ord6/10-training-video.md`

#### DUP-0011 (size 2)

- Canonical: `Archive1/Ord1/11-deployment-payload-2019.md`
- Duplicate: `Archive1/Ord5/11-deployment-payload-2019.md`

#### DUP-0012 (size 2)

- Canonical: `Archive1/Ord1/ini-diff-summary.md`
- Duplicate: `Archive1/Ord5/ini-diff-summary.md`

#### DUP-0013 (size 2)

- Canonical: `Archive1/Ord1/migration-proof.md`
- Duplicate: `Archive1/Ord5/migration-proof.md`

#### DUP-0014 (size 2)

- Canonical: `Archive1/Ord3/00-baseline-summary.md`
- Duplicate: `Archive1/Ord6/00-baseline-summary.md`

## 7. Document Titles and Identifier Mentions

| Metric | Value |
|---|---:|
| Markdown documents | 70 |
| Markdown documents with H1 title | 70 |
| Files with at least one identifier | 30 |
| Sum of distinct IDs (per file) | 489 |
| Sum of raw mentions | 1029 |

### 7.1 Identifier normalization

- GAP-N / C-N / DEC-N / ISS-N normalized to 2-digit padding.
- BR-GROUP-N normalized to 3-digit padding on the numeric suffix.
- Binary and non-text files are excluded from identifier extraction.

### 7.2 Top files by identifier density

| File | Distinct IDs | Raw mentions | Title |
|---|---:|---:|---|
| `Archive2/03_BUSINESS_RULES.md` | 160 | 385 | RefLab Version 2 (V2) — Business Rule Catalogue |
| `Archive2/07_GAPS_AND_LIMITS.md` | 60 | 175 | 07 — Gaps, Limits and Unresolved Conflicts |
| `Archive2/08_DECISIONS_REQUIRED.md` | 51 | 160 | 08 — Decisions Required |
| `Archive1/Ord2/Open-Issues-and-Limitations.md` | 40 | 48 | Open-Issues-and-Limitations.md — القيود والأسئلة المفتوحة |
| `Archive1/Ord2/Stage-9-V-CD-2019-Final-Comprehensive-Report.md` | 24 | 40 | ★ التقرير النهائي الشامل المُراكِم — V-CD-2019 (v1.605) |
| `Archive2/06_PERMISSIONS_AND_SECURITY.md` | 19 | 41 | RefLab V2 — Permissions, Roles & Security |
| `Archive2/00_INDEX.md` | 15 | 16 | 00 — Index, Scope and Reading Guide |
| `Archive1/Ord6/Stage-5-V-CD-2019-Cumulative-Report.md` | 14 | 15 | Stage 5 — V-CD-2019 Cumulative Report (Training Video Confirmed) |
| `Archive1/Ord2/Engagement-Workflow-Narrative.md` | 12 | 14 | ★ سرد سيرورة العمل الكاملة — مشروع RealLab Reference Laboratory System |
| `Archive1/Ord7/Stage-8-V-CD-2019-Install-Media-Report.md` | 12 | 17 | Stage 8 — V-CD-2019 FINAL COMPREHENSIVE REPORT |
| `Archive1/Ord1/Stage-7-V-CD-2019-Final-Report.md` | 10 | 15 | Stage 7 — V-CD-2019 Final Cumulative Report |
| `Archive1/Ord5/Stage-6-V-CD-2019-Payload-Report.md` | 10 | 12 | Stage 6 — V-CD-2019 Cumulative Report (Deployment Payload + Test Library) |
| `Archive2/02_FUNCTIONAL_SPECIFICATION.md` | 8 | 13 | RefLab V2 — Functional Specification (Functions & Capabilities Inventory) |
| `Archive1/Ord2/13-pdf-print-architecture.md` | 7 | 13 | V-CD-2019 — بنية الطباعة وإخراج PDF |
| `Archive1/Ord2/Evidence-Register.md` | 7 | 8 | Evidence-Register.md — سجل الأدلة |
| `Archive2/01_EXECUTIVE_UNDERSTANDING.md` | 6 | 7 | 01 — Executive Understanding: RefLab Version 2 (V2) |
| `Archive1/Ord5/11-deployment-payload-2019.md` | 4 | 5 | V-CD-2019 — حمولة النشر 2019 + مكتبة الأوصاف (BATCH-07 + BATCH-08) |
| `Archive1/Ord3/Stage-4-V-CD-2019-Deployment-Report.md` | 4 | 5 | Stage 4 — V-CD-2019 Deployment & Capability Report |
| `Archive1/Ord1/11-deployment-payload-2019.md` | 4 | 5 | V-CD-2019 — حمولة النشر 2019 + مكتبة الأوصاف (BATCH-07 + BATCH-08) |
| `Archive1/Ord6/10-training-video.md` | 3 | 3 | V-CD-2019 — فيديو التدريب (RLS_Learn.wmv) |

## 8. Consolidated Reference Documents

| File | Size (B) | Title |
|---|---:|---|
| `00_INDEX.md` | 20056 | 00 — Index, Scope and Reading Guide |
| `01_EXECUTIVE_UNDERSTANDING.md` | 28738 | 01 — Executive Understanding: RefLab Version 2 (V2) |
| `02_FUNCTIONAL_SPECIFICATION.md` | 270405 | RefLab V2 — Functional Specification (Functions & Capabilities Inventory) |
| `03_BUSINESS_RULES.md` | 216742 | RefLab Version 2 (V2) — Business Rule Catalogue |
| `04_DATA_MODEL.md` | 154789 | RefLab Version 2 — Data Model Specification |
| `05_WORKFLOWS_AND_UI.md` | 226068 | 05 — Workflows, User Interface, Printing & Reporting — RefLab Version 2 (V2) |
| `06_PERMISSIONS_AND_SECURITY.md` | 78981 | RefLab V2 — Permissions, Roles & Security |
| `07_GAPS_AND_LIMITS.md` | 107132 | 07 — Gaps, Limits and Unresolved Conflicts |
| `08_DECISIONS_REQUIRED.md` | 52793 | 08 — Decisions Required |

No claim is made that a source file was consumed to produce any of these documents without explicit evidence.

## 9. Files Most Relevant to Each Open Item

Full per-candidate map in `C:\Users\LAP LINK\OneDrive\Desktop\ReferenceIndex\RefLab_V2_OPEN_ITEMS_MAP.csv`. This table shows the first candidate per item.

| Item | Family | Present? | First candidate | Status |
|---|---|---|---|---|
| GAP-01 | GAP | Yes | Open-Issues-and-Limitations.md | Present |
| GAP-02 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-03 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-04 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-05 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-06 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-07 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-08 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-09 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-10 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-11 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-12 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-13 | GAP | Yes | 00_INDEX.md | Present |
| GAP-14 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-15 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-16 | GAP | Yes | 00_INDEX.md | Present |
| GAP-17 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-18 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-19 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-20 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-21 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-22 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-23 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-24 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-25 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-26 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-27 | GAP | No | (none) | Absent |
| GAP-28 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-29 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-30 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| GAP-31 | GAP | Yes | 07_GAPS_AND_LIMITS.md | Present |
| C-01 | C | Yes | 00_INDEX.md | Present |
| C-02 | C | Yes | 07_GAPS_AND_LIMITS.md | Present |
| C-03 | C | Yes | 07_GAPS_AND_LIMITS.md | Present |
| C-04 | C | Yes | 07_GAPS_AND_LIMITS.md | Present |
| C-05 | C | Yes | 07_GAPS_AND_LIMITS.md | Present |
| C-06 | C | Yes | 07_GAPS_AND_LIMITS.md | Present |
| C-07 | C | Yes | 07_GAPS_AND_LIMITS.md | Present |
| C-08 | C | Yes | 07_GAPS_AND_LIMITS.md | Present |
| C-09 | C | Yes | 01_EXECUTIVE_UNDERSTANDING.md | Present |
| C-10 | C | Yes | 07_GAPS_AND_LIMITS.md | Present |
| C-11 | C | Yes | 00_INDEX.md | Present |
| C-12 | C | Yes | 08_DECISIONS_REQUIRED.md | Present |
| C-13 | C | Yes | 00_INDEX.md | Present |
| C-14 | C | Yes | 07_GAPS_AND_LIMITS.md | Present |
| C-15 | C | Yes | 07_GAPS_AND_LIMITS.md | Present |
| C-16 | C | Yes | 07_GAPS_AND_LIMITS.md | Present |
| C-17 | C | Yes | 00_INDEX.md | Present |
| DEC-01 | DEC | Yes | 00_INDEX.md | Present |
| DEC-02 | DEC | Yes | 07_GAPS_AND_LIMITS.md | Present |
| DEC-03 | DEC | Yes | 00_INDEX.md | Present |
| DEC-04 | DEC | Yes | 08_DECISIONS_REQUIRED.md | Present |
| DEC-05 | DEC | Yes | 00_INDEX.md | Present |
| DEC-06 | DEC | Yes | 08_DECISIONS_REQUIRED.md | Present |
| DEC-07 | DEC | Yes | 00_INDEX.md | Present |
| DEC-08 | DEC | Yes | 07_GAPS_AND_LIMITS.md | Present |
| DEC-09 | DEC | Yes | 07_GAPS_AND_LIMITS.md | Present |
| DEC-10 | DEC | Yes | 08_DECISIONS_REQUIRED.md | Present |
| DEC-11 | DEC | Yes | 07_GAPS_AND_LIMITS.md | Present |
| DEC-12 | DEC | Yes | 00_INDEX.md | Present |
| ISS-01 | ISS | Yes | Stage-7-V-CD-2019-Final-Report.md | Present |
| ISS-02 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-03 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-04 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-05 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-06 | ISS | Yes | Evidence-Register.md | Present |
| ISS-07 | ISS | Yes | Stage-7-V-CD-2019-Final-Report.md | Present |
| ISS-08 | ISS | Yes | Evidence-Register.md | Present |
| ISS-09 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-10 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-11 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-12 | ISS | Yes | Evidence-Register.md | Present |
| ISS-13 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-14 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-15 | ISS | Yes | 10-training-video.md | Present-CanonicalRef |
| ISS-16 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-17 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-18 | ISS | Yes | 11-deployment-payload-2019.md | Present-CanonicalRef |
| ISS-19 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-20 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-21 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-22 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-23 | ISS | Yes | Stage-7-V-CD-2019-Final-Report.md | Present |
| ISS-24 | ISS | Yes | Stage-7-V-CD-2019-Final-Report.md | Present |
| ISS-25 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-26 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-27 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-28 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-29 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-30 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-31 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-32 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-33 | ISS | Yes | Stage-7-V-CD-2019-Final-Report.md | Present |
| ISS-34 | ISS | Yes | Stage-7-V-CD-2019-Final-Report.md | Present |
| ISS-35 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-36 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-37 | ISS | Yes | 08-deployment-installer.md | Present-CanonicalRef |
| ISS-38 | ISS | Yes | Stage-7-V-CD-2019-Final-Report.md | Present |
| ISS-39 | ISS | Yes | Open-Issues-and-Limitations.md | Present |
| ISS-40 | ISS | Yes | 10-training-video.md | Present-CanonicalRef |
| ISS-41 | ISS | Yes | 10-training-video.md | Present-CanonicalRef |
| ISS-42 | ISS | Yes | Stage-9-V-CD-2019-Final-Comprehensive-Report.md | Present |
| ISS-43 | ISS | Yes | 11-deployment-payload-2019.md | Present-CanonicalRef |
| ISS-44 | ISS | Yes | 11-deployment-payload-2019.md | Present-CanonicalRef |
| ISS-45 | ISS | Yes | 11-deployment-payload-2019.md | Present-CanonicalRef |
| ISS-46 | ISS | Yes | Stage-7-V-CD-2019-Final-Report.md | Present |
| ISS-47 | ISS | Yes | Engagement-Workflow-Narrative.md | Present |
| ISS-48 | ISS | Yes | Engagement-Workflow-Narrative.md | Present |
| ISS-49 | ISS | Yes | 13-pdf-print-architecture.md | Present |
| ISS-50 | ISS | Yes | 13-pdf-print-architecture.md | Present |
| ISS-51 | ISS | Yes | 13-pdf-print-architecture.md | Present |
| ISS-52 | ISS | Yes | 13-pdf-print-architecture.md | Present |
| ISS-53 | ISS | Yes | 13-pdf-print-architecture.md | Present |
| BR-BILL-001 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-BILL-002 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-BILL-003 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-BILL-004 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-BILL-005 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-BILL-006 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-BILL-007 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-BILL-008 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-BILL-009 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-BILL-010 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-BILL-011 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CAT-001 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CAT-002 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CAT-003 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CAT-004 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CAT-005 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CAT-006 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CAT-007 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CAT-008 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CAT-009 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CAT-010 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CAT-011 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CAT-012 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CAT-013 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CAT-014 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CAT-015 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CAT-016 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CAT-017 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CLIN-001 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CLIN-002 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CLIN-003 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CLIN-004 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CLIN-005 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CLIN-006 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CLIN-007 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CLIN-008 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CLIN-009 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CLIN-010 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CLIN-011 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CLIN-012 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CLIN-013 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-CLIN-014 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-ID-001 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-ID-002 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-ID-003 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-ID-004 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-ID-005 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-ID-006 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-ID-007 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-ID-008 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-ID-009 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-001 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-002 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-003 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-004 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-005 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-006 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-007 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-008 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-009 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-010 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-011 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-012 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-013 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-014 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-015 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-016 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-017 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-018 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-LIFE-019 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-MIG-001 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-MIG-002 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-MIG-003 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-MIG-004 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-MIG-005 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-MIG-006 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-MIG-007 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-MIG-008 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-MIG-009 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-MIG-010 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-MIG-011 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-MIG-012 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-MIG-013 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-MIG-014 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RANGE-001 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RANGE-002 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RANGE-003 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RANGE-004 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RANGE-005 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RANGE-006 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RANGE-007 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RANGE-008 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RANGE-009 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RANGE-010 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RANGE-011 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RANGE-012 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RANGE-013 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RANGE-014 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RANGE-015 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RANGE-016 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RANGE-017 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RPT-001 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RPT-002 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RPT-003 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RPT-004 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RPT-005 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RPT-006 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RPT-007 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RPT-008 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RPT-009 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RPT-010 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RPT-011 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RPT-012 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RPT-013 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RPT-014 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RPT-015 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RPT-016 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-RPT-017 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SETUP-001 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SETUP-002 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SETUP-003 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SETUP-004 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SETUP-005 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SETUP-006 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SETUP-007 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SETUP-008 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SETUP-009 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SETUP-010 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SETUP-011 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SETUP-012 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SETUP-013 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SETUP-014 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SETUP-015 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SETUP-016 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SETUP-017 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SMS-001 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SMS-002 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SMS-003 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SMS-004 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SMS-005 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SMS-006 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SMS-007 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SMS-008 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SMS-009 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SMS-010 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| BR-SMS-011 | BR | Yes | 03_BUSINESS_RULES.md | Present |
| SETUP.LST | File | No | (none) | InsideNestedZip |
| BASE.ini | File | No | (none) | InsideNestedZip |

## 10. Final Verification Report

Values are measured by re-reading the generated CSV after writing it. Hashes, titles, and identifiers were computed from the archives at indexing time; they were NOT independently verified against a separate copy of the source archives.

| Check | Expected | Actual | Status |
|---|---|---|---|
| Total CSV rows | 100 | 100 | PASS |
| Patches rows | 91 | 91 | PASS |
| Consolidated rows | 9 | 9 | PASS |
| First column name | ArchiveName | ArchiveName | PASS |
| CSV has UTF-8 BOM | No | No | PASS |
| SHA-256 OK | 100 | 100 | PASS |
| SHA-256 failed | 0 | 0 | PASS |
| Patches files with empty FolderOrdinal | 0 | 0 | PASS |
| Canonical references with non-empty DuplicateOf | 0 | 0 | PASS |
| Binary files with identifiers | 0 | 0 | PASS |
| Files with raw-byte corruption (name recovered externally) | 91 | 91 | INFO |

