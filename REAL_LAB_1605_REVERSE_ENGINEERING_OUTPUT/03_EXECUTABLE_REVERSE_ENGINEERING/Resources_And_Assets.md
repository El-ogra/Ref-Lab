# Resources and Assets — embedded PE resources and shipped media

**Evidence:** `real lab system.exe (2).json`, `real lab system.exe.json`, `pe-resources.json`, `pe-summary.csv`, `v1.605-package-manifest.csv`, `version-resources.json`, `rtf-analysis.json`, `rtf-info-metadata.json`, `parsed__[analysis_BATCH-02_rtf].csv`, `Version_Identity_and_Scope.md`, `primary-executable-reverse-engineering.md`, and the flat asset files in `D:\Collective work\`. All paths under that directory unless stated.

## 1. Embedded PE resources (VERIFIED)

`real lab system.exe (2).json → pe.resources` and `pe-resources.json` agree for 1.605:

| Type | Count | Detail |
|---|---:|---|
| `RT_ICON` (dump) / `ICON` (pe-resources) | **10** | IDs `30001`–`30010`; sizes 744, 296, 3,752, 2,216, 1,384, 69,012, 67,624, 9,640, 4,264, 1,128 B; MD5s listed in the dump |
| `RT_GROUP_ICON` / `GROUP_ICON` | **1** | ID `1`, 146 B, magic `000001000a002020` |
| `RT_VERSION` / `VERSION` | **1** | 804 B, lang 1033, version `1.605` / `RLS 1.605` |
| **Total** | **12** | `pe-resources.json → by_type {ICON:10, GROUP_ICON:1, VERSION:1}` |

Notable: resource `30006` is 69,012 B with PNG magic `89504e470d0a1a0a` — a 256×256 PNG-format icon. The standalone `icon-embedded.png` in `D:\Collective work\` is also 69,012 B, matching that resource. `primary-executable-reverse-engineering.md §7` additionally reports embedded Adobe Photoshop CS3 XMP metadata in the string corpus (report artwork authored in Photoshop). The 1.00 baseline has the same 12-resource shape with a 760 B VERSION resource (`real lab system.exe.json`).

**No manifest resource** in either executable: `pe.resource_types` contains only the three types above. Consequences noted in `primary-executable-reverse-engineering.md §7`: no requested execution level, no DPI awareness, no common-controls v6 declaration, no supported-OS list.

## 2. CHM help file (VERIFIED)

| Property | Value | Evidence |
|---|---|---|
| Name | `real lab system help.chm` | `v1.605-package-manifest.csv` (B1 "Application Core") |
| Size | 5,193,985 B | same; `pe-summary.csv` lists it as `NON-PE` |
| SHA-256 | `624308649E523400F6672389FC877543DB9D286F27E743C1581B1CF507902A39` | `v1.605-package-manifest.csv` |
| Paths | `real lab system Version 1.605\real lab system help.chm` and `real lab system Version 1.605\RL_CD2019Access\System\real lab system help.chm` | same (two manifest rows, identical SHA) |
| Topics / cross-version | 15 topics; byte-identical between 1.00 and 1.605 | `Version_Identity_and_Scope.md §3.5, §6.1` |

The CHM is **not embedded** in the executable; it is a shipped companion file. `Version_Identity_and_Scope.md §3.5` records the finding that 41 new objects, an SMS subsystem and 17 new tables were added in 1.605 without any user-manual change — the CHM is unchanged from 1.00.

## 3. RTF method sheets (VERIFIED file evidence; semantics INFERRED)

- `v1.605-package-manifest.csv` contains **58 unique `.rtf` source paths** — the same 29 filenames under both `Cache\` and `RL_CD2019Access\Cache\`, with identical SHAs. Examples: `006.rtf` 9,691 B SHA `8B13425A…`; `007.rtf` 6,929 B; `056.rtf` **16,953,057 B** SHA `909CD0F4…`; `070.rtf` 3,495,666 B; `500.rtf` 25,352 B.
- `Version_Identity_and_Scope.md §6.1` lists "`Cache\*.rtf` (29 of 29 identical)" among shared byte-identical artefacts and §2.4 says "29 RTF method sheets". The 58 manifest paths = 29 per tree × 2.
- Analysis dumps present: `rtf-analysis.json` (80,417,391 B), `rtf-info-metadata.json`, `parsed__[analysis_BATCH-02_rtf].csv`, `rtf_analyse.py`. These are extraction products, not target code.
- The large RTF files embed vector images — flat work-dir files `056_pict1.emf` (1,901,528 B), `056_pict2.wmf` (3,337,858 B), `056_pict3.emf` (1,121,456 B), `056_pict4.wmf` (1,969,330 B), `070_pict1.emf` (619,616 B), `070_pict2.wmf` (1,087,998 B) corroborate EMF/WMF payloads inside method sheets `056` and `070`. The mapping from a given RTF to a `Result*` form is INFERRED (names only; no code binding recovered).

## 4. Flash simulators (VERIFIED names/sizes; internal type UNVERIFIED)

`v1.605-package-manifest.csv` (B3 "Report Engine & Visual Assets") lists 13 executable names in both `Cache\` and `RL_CD2019Access\Cache\` (26 manifest rows, identical SHAs per pair):

`001.exe`, `002.exe`, `003.exe`, `004.exe`, `005.exe`, `006.exe`, `007.exe`, `007a.exe`, `008.exe`, `009.exe`, `010.exe`, `011.exe`, `012.exe`.

Representative SHA-256 values (from `SHA256_Manifest.csv`, ids `A0008`–`A0028`): `001.exe` `2BE865A4…E7E31` (7,001,308 B), `007a.exe` `F409242A…CEC3` (6,920,620 B), `012.exe` `3BDE3DDB…2E9C` (4,733,541 B). `Version_Identity_and_Scope.md §6.1` lists "Flash simulators (13 of 13 identical)" as shared byte-identical artefacts. No PE/format dump exists for these files in the evidence set, so their internal format is **UNVERIFIED**; they are training content, not application logic.

## 5. Images and report artwork (VERIFIED manifest entries)

`v1.605-package-manifest.csv` contains 33 image rows. Groups:

| Group | Files | Location |
|---|---|---|
| JPEG report artwork | `Header.jpg` 64,039 B; `Logo.jpg`/`Logo1.jpg`/`Logo2.jpg` 24,542 B each; `Tailer.jpg` 27,546 B; `Text.jpg` 27,804 B; `Wasl.jpg` 27,228 B; `ZHeader.jpg` 66,802 B; `ZTailer.jpg` 26,803 B | `RL_CD2019Access\bin\` |
| PCR image set | `PCRD.jpg` 173,332 B; `PCRF.jpg` 45,236 B; `PCRH.jpg` 172,425 B; `PCRL.jpg` 171,360 B; `PCRM.jpg` 175,877 B | `bin\` and `RL_CD2019Access\bin\` |
| GIF assets (HTML designer) | `anchorwi.gif` 207 B, `br.gif` 183 B, `clientsc.gif` 216 B, `comments.gif` 227 B, `divbgn.gif` 160 B, `divend.gif` 164 B, … (under `System\datareport_vh\`) | same |
| Flat work-dir derivatives | `pcr_group.png` 226,519 B; `stationery_group.png` 56,073 B; `icon-embedded.png` 69,012 B; `datareport_vh_tags.png` 1,434 B; `datareport_vh_toolbar.png` 2,341 B | `D:\Collective work\` (analysis extracts) |

The JPEG names (`Header`, `Logo`, `Tailer`, `Wasl`, `ZHeader`, `ZTailer`) correspond to report-frame regions; which `Report*` object consumes which file is **INFERRED** from naming and the report-template literature in `primary-executable-reverse-engineering.md §4`, not from an observed call chain.

## 6. Report templates and DataReport definitions (VERIFIED counts; binding INFERRED)

- 66 DataReport objects in the VB6 object table (`objects-full__[…v1605].txt`), mostly `methods = 0` and `type_flags 0x1018083` (`vb6-objects.json`) — data-driven layout definitions. Catalogued in `Class_And_Method_Catalog.md §4`.
- `primary-executable-reverse-engineering.md §4` states the 146 `Data/*.ini` templates are the layout each `Report*` object consumes; `Version_Identity_and_Scope.md §2.4, §3.4` mention 223 `Data\*.ini` files and that 104 of 223 differ between versions; `Version_Identity_and_Scope.md §6.1` records per-file comparison. `BASE.ini`, `SETTING.ini`, `IDCARD.INI`, `REPORT.dll`, `NOTE.ini` are named configuration files.
- `primary-executable-technical-reference.md §6` reports that the report renderer loads seven installed fonts (consistent with the `BCWFonts` module, 7 methods, in `vb6-objects.json [65]`).
- The `smsmisr.com` URL template and print-password literals (`IDPasswordL/T/F/B/S/V/A`) are string evidence (`primary-executable-reverse-engineering.md §6.3–6.4`), not embedded binary resources.

## 7. Confidence summary

| Item | Label |
|---|---|
| PE icon/group/version resource counts and sizes | VERIFIED (two dumps agree) |
| CHM identity, size, SHA, two paths | VERIFIED (`v1.605-package-manifest.csv`) |
| 29 RTF sheets per tree × 2, 13 Flash simulators × 2 | VERIFIED (manifest + cross-version identity report) |
| EMF/WMF payloads in sheets 056/070 | VERIFIED as flat extracted files; RTF↔form binding INFERRED |
| Image/JPEG/GIF file inventory | VERIFIED (manifest); report-slot semantics INFERRED |
| 66 DataReport objects, 223 `Data/*.ini` files | VERIFIED counts from object dump / version report; object↔template binding INFERRED |
| Internal format of Flash simulators and un-dumped EXEs | UNVERIFIED |
