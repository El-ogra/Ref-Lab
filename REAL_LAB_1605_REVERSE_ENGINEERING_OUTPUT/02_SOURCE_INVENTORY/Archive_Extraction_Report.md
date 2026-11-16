# Archive Extraction Report

**Scope:** All archive-like artifacts in `D:\Collective work`.  
**Method:** Read-only enumeration via .NET `System.IO.Compression.ZipFile.OpenRead` — no archive was modified, and no payload was extracted to the evidence directory. Any temporary analysis output was confined to `C:\Users\LAP LINK\AppData\Local\Temp\opencode\`.

## 1. Archive inventory (8 ZIP files)

| # | Filename | Size (bytes) | Entry count | Directory entries | Extension composition (top) | Assessment |
|---|---|---:|---:|---:|---|---|
| 1 | `RealLab-Cumulative-Deliverables-B01-B02-B03.zip` | 6,545,194 | 458 | 29 | 276 .csv, 57 .json, 51 .md, 21 .py, 14 .jpg, 4 .sql | Analysis deliverables only |
| 2 | `RealLab-Deliverables-B01-B08.zip` | 12,005,405 | 576 | 40 | 290 .csv, 79 .json, 76 .md, 38 .jpg, 26 .py, 8 .sql | Analysis deliverables only |
| 3 | `RealLab-Deliverables-B01-B09.zip` | 12,072,996 | 584 | 40 | 292 .csv, 80 .json, 79 .md, 38 .jpg, 27 .py, 8 .sql | Analysis deliverables only |
| 4 | `RealLab-Deliverables-B01-B10.zip` | 26,410,462 | 687 | 43 | 293 .csv, 81 .md, 80 .json, **46 .dll, 32 .ocx, 4 .exe**, 10 .ttf, 8 .sql | Contains recovered install-media dependencies (WIN7/WINXP CAB members) |
| 5 | `RealLab-Deliverables-B01-B11.zip` | 12,091,120 | 597 | 47 | 295 .csv, 85 .md, 55 .txt, 39 .json, 38 .jpg, 28 .py, 8 .sql | Analysis deliverables only |
| 6 | `RealLab-Deliverables-V1.00-and-V2019__[garbled Arabic].zip` | 6,716,814 | 515 | 37 | 286 .csv, 73 .json, 63 .md, 23 .py, 14 .jpg, 4 .sql | Analysis deliverables only |
| 7 | `RealLab-Deliverables-V1.00-and-V2019__[garbled Arabic].zip` | 11,617,165 | 547 | 37 | 288 .csv, 75 .json, 70 .md, 31 .jpg, 24 .py, 4 .sql | Analysis deliverables only |
| 8 | `RealLab-Deliverables-V1.00-and-V2019.zip` | 7,303,814 | 531 | 36 | 287 .csv, 73 .json, 67 .md, 24 .py, 23 .jpg, 4 .sql | Analysis deliverables only |

The two filenames containing garbled Arabic are reproduced descriptively; the originals are preserved byte-for-byte in `D:\Collective work` and are listed exactly in `Complete_Artifact_Inventory.csv`.

## 2. Nested archives

Every entry name in all 8 ZIPs was tested against `.zip`, `.rar`, `.7z`, `.cab`, `.tar`, `.gz`, `.mdb`, `.accdb`. **Result: zero nested archives.** No recursive extraction was therefore required or performed. This is a negative finding with a complete search boundary (all 4,395 entries across 8 archives).

## 3. High-value embedded content (B01–B10)

Archive #4 contains two recovered CAB member sets under `analysis/BATCH-10/cab/win7/` and `.../winxp/`. These are the VB6 runtime and ActiveX dependencies of the v1.605 installer (e.g. `msvbvm60.dll`, `msjet40.dll`, `dao360.dll`, `MSADODC.OCX`, `MSDBRPTR.DLL`, `MSCOMCTL.OCX`, `RICHTX32.OCX`, `VB6STKIT.DLL`, `SETUP1.EXE`, `ST6UNST.EXE`). They are **supporting dependencies**, not target application logic. No `real lab system.exe` (76,292,096-byte v1.605 binary) exists inside any archive.

## 4. Extraction operations

| Operation | Target | Result |
|---|---|---|
| Read-only entry enumeration | all 8 ZIPs | SUCCESS (4,395 entries) |
| Signature/magic scan | all 973 top-level files | SUCCESS; no mismatches except the two `.xlsx` files, which are legitimately ZIP container formats |
| Recursive nested-archive test | all ZIP entries | SUCCESS — none found |
| Payload extraction into evidence directory | — | NOT PERFORMED (evidence integrity) |
| Binary execution of any archive payload | — | NOT PERFORMED (safety) |

## 5. Interpretation

The archives are cumulative **analysis deliverable packages**, not original installation media. They repackage the same 11-batch investigation outputs already present flat in `D:\Collective work`, which explains the large number of byte-identical duplicate files recorded in `SHA256_Manifest.csv`. Their evidentiary value is corroboration of prior work, not a source of new primary artifacts.
