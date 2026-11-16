# Unreadable, Ambiguous, and Degenerate Artifacts

This document records artifacts that could not be fully interpreted, are ambiguous, or are degenerate. It deliberately does **not** list every binary file: a binary format is not "unreadable" merely because it is not plain text.

## 1. Hashing and filesystem accessibility

- All **973 / 973** files in `D:\Collective work` were successfully opened and hashed with SHA-256 (`Get-FileHash`). **0** access failures, **0** zero-byte files.
- Therefore no artifact is unreadable at the byte level.

## 2. Artifacts whose original binary form is absent from the repository

These are the most important "unreadable" items in the investigative sense: the evidence repository contains *derivatives*, not the originals.

| Missing original | Derivative present | Consequence | Confidence |
|---|---|---|---|
| **v1.605 main executable** (`real lab system.exe`, 76,292,096 B, SHA-256 `C54CE6C8…`) | `real lab system.exe (2).json` PE dump; object/SQL/literal dumps; Ghidra function/string dumps | No direct binary decompilation, debugging, or runtime observation of the target version is possible | VERIFIED |
| v1.605 Access database `Patients.mdb` (103 tables / 1,682 columns) | `v2019-Patients.mdb.sql`, schema/DDL dumps, per-table CSV exports | Schema reconstructed from DDL; no live Jet-engine validation of data pages, indexes, or query plans | VERIFIED |
| `Branch_Data.mdb` (49 tables, schema-only) | `schema-Branch_Data.sql` | Same as above; additionally, original had 0 rows so no relationship semantics from data | VERIFIED |
| `data.mdb` (history/patientinfo/wasl) | `schema__[analysis_BATCH-02_db_data].sql`, `history.csv`, `patientinfo.csv`, `wasl.csv` | Dynamic 20-column `history` table understood structurally only | VERIFIED |
| `VIDAS` database | Referenced in `Attach_VIDAS.bat` mentions and prior docs | Nature unestablished; UNKNOWN in prior work and remains UNKNOWN here | UNVERIFIED |
| `RL_SETUP.exe` 22.95 MiB embedded payload | characterised in prior docs (`Version_Identity_and_Scope.md` §5.3) | Payload not decomposed; contents undocumented | UNKNOWN |
| 5 large Microsoft installers referenced by `Setup*.bat` | absent | Install-media reconstruction is partial | VERIFIED (absence) |
| `RLS_Learn.pdf` / `RLS_Setup.pdf` / `RL_Show.pdf` | prior extracted text in prior docs; not present as files locally | Arabic glyph extraction issues documented by prior work; cannot re-verify from local bytes | VERIFIED (absence) |

## 3. Filename encoding damage

**61 files** have non-ASCII display names, many containing mojibake (mis-decoded Arabic), e.g. `00-baseline-summary__[�� ���� ....md`. These were copied from an Arabic-named archive context into a mis-decoded Windows filename. Handling rules applied:

- Files were addressed by exact literal path (`-LiteralPath`) throughout; no rename, normalization, or "repair" was attempted.
- Content is unaffected; only the **filename** is damaged.
- The exact original bytes of the names are preserved in `Complete_Artifact_Inventory.csv` (UTF-8) so that cross-references remain auditable.

## 4. Formats that require specialized interpretation (not errors)

| Artifact type | Examples | Why not plain-text readable | Treatment |
|---|---|---|---|
| PE executables / OCX / DLL | 37 `.exe` + control JSON dumps | binary | PE parsing, version resources, hash comparison |
| Flash projector simulations | `001.exe`–`012.exe`, `007a.exe` (Macromedia Flash Player 8) | compiled SWF inside a projector stub | identified by version resource; not executed |
| `.emf` / `.wmf` / `.png` | `056_pict1.emf`, `datareport_vh_tags.png` | binary image/vector | enumerated; not decoded beyond metadata |
| `.xlsx` | 2 files | ZIP container | read-only entry enumeration |
| `.chm` dumps | `chm-*.json/.txt` | original CHM absent locally | prior extracted text used as evidence with explicit attribution |
| Arabic-text files | `help-manual-ar.md`, `ui-labels-verified.md` | encoding-sensitive | read as UTF-8; exact labels preserved, never invented (AGENTS.md rule 7) |

## 5. Known interpretive limitations recorded by prior work and revalidated

| Limitation | Prior status | Present status | Evidence |
|---|---|---|---|
| `normalrange.pregsmok` populated on all 852 rows but calculation use untraced | OBSERVED, not a working rule | UNVERIFIED — unchanged | `Version_Identity_and_Scope.md` §5.3; `normalrange.csv` |
| `Branch_Data.mdb` schema-only (0 rows) | INFERRED relationships | INFERRED — unchanged | `schema-Branch_Data.sql` |
| `Patients_backup.mdb` version attribution | UNKNOWN | UNKNOWN — unchanged | `Version_Identity_and_Scope.md` §4.2 |
| v1.605 shipped DB out of step with 268 embedded DDL statements | CONFIRMED | CONFIRMED by `ddl-evidence__[analysis_BATCH-04_sql_v1605].txt` vs `v2019-Patients.mdb.sql` | both files |

**Conclusion:** there are no byte-level unreadable artifacts. The material limitations are *absence of original binaries/databases* and *encoding damage in a subset of filenames*, both of which are fully enumerated above.
