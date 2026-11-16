# Dependency Analysis — Batch B3 of RealLab CD 2019 v1.605

**Batch:** B3 · **Archive:** `v1.605-B3.zip` · **SHA-256:** `2fb942c377d1e06b6ba473302e3ad8e8ba8a6faa39513bb9a296a73d64ee3a7b`
**Analysis date:** 2026-10-31 (UTC) · **Method:** static only; nothing was executed

> B3 contains no executable, so it has **no import table, no dependency list and
> no load order**. What follows are the *content-level* relationships B3's
> files evidence: what they require to function, what they reference, and what
> references them. Machine-readable form: `dependencies.json`.

---

## 1. Confirmed dependencies

| ID | From | To | Kind | Evidence |
| --- | --- | --- | --- | --- |
| DEP-B3-001 | `Cache/*.rtf` | a Word-2002-era RTF consumer (Rich Text control or Word) | document format | `\*\generator Microsoft Word 10.0.2627`; `\ansicpg1256`; `\vern 16437` — identical in all 58 files |
| DEP-B3-002 | `RL_CD2019Access/System/datareport_vh/*.gif` | B2's `datareport_vh` engine group | resource → code | **Identical directory path** in B2's `archive-inventory.md` |
| DEP-B3-003 | `Data/*.ini` `[reportdata].colorXfn` | Windows GDI fonts Arial / Tahoma / Times New Roman | font | 675 / 597 / 328 slot references |
| DEP-B3-004 | `Data/*.ini` `[setupx].reportprinterx` | Windows printer driver `HP LaserJet 1100 (MS)` | device | 138 of 146 templates; the other 8 use `HP LaserJet 1100` |
| DEP-B3-005 | `Fonts/FREE3OF9.TTF`, `Fonts/WBCW_UPCEAN.ttf`, `Fonts/WBCWS_UPCEAN.ttf` | Code 39 and UPC-A/EAN symbologies | symbology | font family names `FREE3OF9` / `WBCW_UPCEAN`; version string "Version 1.00 December 22, 2009" |
| DEP-B3-006 | `Fonts/MCS Taybah.ttf` | an Arabic + Latin clinical glyph set | font | name table "MCS Taybah S_U normal.", version "1.0 Tue Jan 07 17:41:25 1997" |
| DEP-B3-007 | `Data/65.ini`, `66.ini`, `418.ini` (+ `_S`, both trees) | B1's `RL_CD2019Access/Data/data_emp/R.ini` / `R_S.ini` | content identity | **SHA-256 equality** across all 12 files |
| DEP-B3-008 | `RL_CD2019Access/Data/450…465*.ini` (`testtype=pcr`) | `bin/PCR*.jpg` amplification graphics | asset → template | 16 molecular-biology templates; exactly 5 PCR graphics, shipped in both trees |
| DEP-B3-009 | `RL_CD2019Access/bin/Wasl.jpg` | B1's Access reference table `wasl` | name correspondence | B1's `software-architecture.md` lists `wasl` among the reference tables; the image carries the laboratory-identity fields |

### 1.1 Notes on individual dependencies

**DEP-B3-002 is the strongest form of cross-batch evidence in this package.**
It is not a name match or an inference: B2's own archive inventory places
`jscript.dll`, `vbscript.dll`, `msscript.ocx`, `mswless.ocx`, `mswless.dep`,
`RICHTX32.OCX`, `selfreg.dll`, `sp698vbo.dll`, `dispex.dll` and `mssetup.dll`
in `RL_CD2019Access/System/datareport_vh/`, and B3 places 14 GIF files in
**the same path**. The two batches are complementary halves of one directory.

**DEP-B3-007 is cryptographic, not inferred.** Twelve B3 files share a
SHA-256 digest with B1's inventory rows. B1's prototypes are the *source*;
B3 carries the *instantiations* under three distinct numeric test IDs.

**DEP-B3-004 is a hard device binding.** Every one of the 146 templates names
a printer driver that has not shipped with Windows for over a decade. The
absolute twip geometry in `[reportdata]` is calibrated against it.

---

## 2. Possible dependencies

| ID | From | To | Kind | Evidence | Why only "possible" |
| --- | --- | --- | --- | --- | --- |
| DEP-B3-P01 | the 12 Arabic allergen templates | the Windows-1256 system ANSI codepage | locale | 12 files carry Arabic text encoded as cp1256; RTF headers independently declare `\ansicpg1256` | the file reader is in B1's main executable and is not in this batch |
| DEP-B3-P02 | `Cache/056.rtf`, `Cache/070.rtf` | B2's `RICHTX32.OCX` Rich Text control | control | B2 placed a `RICHTX32.OCX` stub in the same directory; the RTFs carry EMF/WMF pictures a Rich Text control would host | B2's copy is a 675-byte stub, so the real control's behaviour is unknown |
| DEP-B3-P03 | `[reportdata].colorXfn` | the three GDI fonts | font | as DEP-B3-003 | the renderer may map these names to a private font set rather than the system fonts |

---

## 3. Unresolved dependencies

| ID | Statement | Where the answer would be |
| --- | --- | --- |
| DEP-B3-U01 | `Data/REPORT.ini` — the global report colour theme the main executable reads for `color1..color19` — is **not in B3**. The only copy in the installation is B2's `REPORT.dll`, an INI wearing a `.dll` extension. | B4/B5, or the installer |
| DEP-B3-U02 | `RL_CD2019Access/Data/data_emp/` — B1's generic-template repository — is **not in B3**; only its instantiated copies are. | B1's own portion |
| DEP-B3-U03 | The code that selects the numeric test-ID template and the `_S` variant. | B1's main executable |
| DEP-B3-U04 | Which of the 5 PCR graphics binds to which of the 16 PCR templates. | B1 or the instrument-interface configuration |
| DEP-B3-U05 | The mapping of test IDs (60–66, 116–117, 164–167, 198, 220, 279, 418, 480, 613) to `testtable` rows. | the database (B1) |
| DEP-B3-U06 | Whether the application copies templates to a writable per-user location at start-up. | B1's main executable |
| DEP-B3-U07 | Whether `Wasl.jpg` is genuinely rendered from the `wasl` table, or is a separate hand-made asset. | B1's Access schema and B1's executable |
| DEP-B3-U08 | Whether the 20 base colours (`colora`…`colort`) are read from `Data/REPORT.ini` or defined locally. | B1's executable; B2's `REPORT.dll` content |

---

## 4. What B3 depends on that it does not contain

```
B3 content
  ├─ needs  the VB6 main executable ...................... B1
  ├─ needs  Microsoft Data Report (MSDBRPTR.DLL) ......... B2
  ├─ needs  the HTML report designer engine .............. B2  (same directory)
  ├─ needs  a Rich Text control for the cache ............ B2  (stub only)
  ├─ needs  a SQL Server or Access/Jet back end ......... B1  (runtime)
  ├─ needs  Data/REPORT.ini ............................. NOT IN ANY ANALYSED BATCH
  └─ needs  the printer driver "HP LaserJet 1100 (MS)" .. the target host
```

---

## 5. Analyst tooling used for B3

| Tool | Version | Use |
| --- | --- | --- |
| Python | 3.11.2 | all analysis: `extract_safe.py`, `build_inventory.py`, `analyse_ini.py`, `analyse_rtf.py`, `analyse_assets.py`, `extract_rtf_pictures.py`, `build_file_analysis.py`, `build_artefacts.py`, `build_file_by_file.py` |
| Pillow | 12.2.0 | image decoding, pixel rendering for visual identification, montage generation |
| ImageMagick | 6.x | montage rendering of the report-designer toolbar and the PCR graphics |
| `file` | 5.44 | independent format confirmation of the extracted EMF/WMF |
| `unzip` / `zipfile` | — | archive structure inspection and CRC verification |

**No reverse-engineering tool was required.** Ghidra 12.1.4 and ilspycmd
9.1.0.7988 were restored and verified during the session's infrastructure
recovery, but B3 contains no PE, no .NET assembly and no script, so neither was
exercised. `mdbtools` was installed and not used: B3 contains no database file.

---

## 6. Limitations

1. Dependency relationships are inferred from **content and path agreement**.
   With no executable in the batch there is no import table to confirm them.
2. The **runtime host** for every B3 asset is unobserved — no Windows
   environment was available and none was in scope.
3. The relationships to B1 and B2 are **cross-batch corroborations**, not
   runtime traces: the consuming code lives in B1 and was not re-analysed here.
