# System Identity and Version — RealLab v1.605

## Identity

| Property | Value | Evidence |
|---|---|---|
| Product name | "real lab system" (RealLab) | real lab system.exe.json:pe.version.ProductName |
| Vendor | ais group corporation | chm-analysis.json (X000X topic) |
| Copyright | ais group corporation 2010 | chm-analysis.json |
| Version | v1.605 ("RealLab CD 2019") | analysis-manifest.json |
| Domain | Clinical Laboratory Information System (LIS) | Type 2 catalogue + Type 1 help |

## Shipped versions (two builds present)

| Build | File | Size | PE timestamp | SHA-256 (head) | Notes |
|---|---|---:|---|---|---|
| V-ROOT-1.00 | real lab system.exe | 59,150,336 B | 1388267701 (2013-12-28) | 12828D0A… | origin; FileVersion 1.00; Company RealLab |
| v1.605 (V-CD-2019) | real lab system.exe (2) | 76,292,096 B | 1595435962 (2020-07-22) | C54CE6C8… | target of this reconstruction |

- Both are PE32 i386 GUI, image base 0x400000, sections .text/.data/.rsrc.
- The v1.605 executable is the authoritative copy (UNR-001 RESOLVED: sha matches B1's recorded digest for RL_CD2019Access/System/real lab system.exe).
- Cross-version matrix: 837 file relationships; 190 REL_DIFFERENT; 168 byte-identical.

## Platform & runtime

- 32-bit Windows GUI, VB6 compiled to **native code** (F-01, 6 evidence lines).
- VB6 runtime build 9782 (SP6), LCID 1033 (en-US), project name `Real_lab_system`.
- Out-of-support runtime (MSVBVM60, VBA6).

## Language & locale

- UI text and help are Arabic (RTL); some report templates carry Arabic labels in Windows-1256 (U-027).
- Product manual is HTML Help 4.74 (CHM), 17 topics, 79 images.