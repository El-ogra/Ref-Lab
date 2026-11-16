# Coverage and Limitations — Batch B3 of RealLab CD 2019 v1.605

**Batch:** B3 · **Archive:** `v1.605-B3.zip` · 3,523,212 bytes
**SHA-256:** `2fb942c377d1e06b6ba473302e3ad8e8ba8a6faa39513bb9a296a73d64ee3a7b`
**Analysis date:** 2026-10-31 (UTC) · **Method:** static only; nothing was executed

---

## 1. Per-file coverage

| Metric | Value |
| --- | ---: |
| Files in the archive | 251 |
| Files extracted | **251 (100 %)** |
| Files inventoried with SHA-256 | **251 (100 %)** |
| Files assigned an analysis status | **251 (100 %)** |
| Files analysed by a format-appropriate method | **251 (100 %)** |
| Files not analysed | **0** |

Every file carries the analysis state **`ANALYSED`** in
`inventory/file-analysis.json` and in `inventory/file-inventory.csv`. There is
no `NOT-ANALYSED`, `PARTIAL` or `SKIPPED` entry — because the batch contains
no file for which a format-appropriate method was unavailable.

### By format

| Format | Files | Method | Coverage |
| --- | ---: | --- | ---: |
| INI | 146 | section/key/value parse, encoding detection, line-ending analysis, duplicate-key detection | 146 / 146 |
| RTF | 58 | control-word parse, text extraction, `\info` metadata, font-table extraction, picture extraction | 58 / 58 |
| GIF | 14 | header, frame and palette parse + pixel rendering | 14 / 14 |
| JPEG | 19 | segment scan + pixel rendering | 19 / 19 |
| TTF | 14 | table directory, `name` table, OS/2 `fsSelection` | 14 / 14 |

---

## 2. Per-stage coverage

| Stage | Status | Coverage |
| --- | --- | --- |
| Archive fingerprinting (size, SHA-256, MD5, format) | **COMPLETE** | 1 / 1 archive |
| Archive safety assessment (traversal, absolute paths, symlinks, encryption, ratios) | **COMPLETE** | 251 / 251 entries |
| CRC verification of every entry | **COMPLETE** | 251 / 251 |
| Safe extraction with backslash-path normalisation | **COMPLETE** | 251 / 251, 0 skipped |
| Cryptographic inventory + duplicate grouping | **COMPLETE** | 251 / 251 |
| INI structural analysis | **COMPLETE** | 146 / 146 |
| RTF document analysis | **COMPLETE** | 58 / 58 |
| Embedded RTF picture extraction and format validation | **COMPLETE** | 6 / 6 pictures |
| GIF analysis and visual identification | **COMPLETE** | 14 / 14 |
| JPEG analysis and visual identification | **COMPLETE** | 19 / 19 |
| TrueType analysis | **COMPLETE** | 14 / 14 |
| Cross-batch hash correlation against B1 and B2 | **COMPLETE** | 251 B3 vs 140 B1 + 70 B2 digests |
| Credential and secret scanning | **COMPLETE** | 251 / 251 |
| RTF active-content scanning | **COMPLETE** | 58 / 58 |
| Duplicate-key analysis within INI sections | **COMPLETE** | 146 / 146 |
| Visual rendering of all image assets | **COMPLETE** | 33 / 33 |
| **Static disassembly / decompilation** | **NOT APPLICABLE** | The batch contains **no** PE, .NET assembly, OCX or script file |
| **Ghidra deep analysis** | **NOT APPLICABLE** | No PE in the batch |
| **Dynamic / runtime analysis** | **NOT PERFORMED** | No isolated Windows host exists; out of scope for this batch |
| **Database analysis** | **NOT APPLICABLE** | The batch contains **no** database file. B2's definitive negative result therefore stands unchanged. |
| **External vulnerability research** | **NOT PERFORMED** | Out of scope: the batch contains no executable third-party code for an advisory to attach to |

### Why there is no `database-analysis.md`

B2 produced a database-analysis report because B2's trees needed a
**definitive negative** on the question. B3 contains no database file at all —
no Access/Jet `.mdb`, no SQL dump, no SQLite, no DSN — so a dedicated report
would restate an absence without adding evidence. The negative is recorded
here and in `executive-summary.md` §6 instead, and `mdbtools` was installed and
deliberately left unused.

---

## 3. Methodology limitations — what "static" means here

| Limitation | Consequence |
| --- | --- |
| **Nothing was executed.** No file was opened by the application, no binary was run, no document was opened in Word, no RTF was rendered by a Rich Text control. | Every statement about behaviour is an inference from content. The batch's own runtime behaviour is entirely unobserved. |
| **No Windows host was available.** The analysis ran on Debian 12 / x86_64 with 3 GiB RAM. | The report renderer's actual output, the INI reader's semantics, the HTML designer's script behaviour and the barcode generator are all unverified. |
| **The consuming code is in B1.** The main executable — which reads these templates, selects the numeric ID, chooses the `_S` variant, and opens the files with a given codepage — is not in this batch. | Template-selection and reader semantics remain **[UNRESOLVED]** (DEP-B3-U03, DEP-B3-P01). |
| **B2's components were not re-analysed.** The `jscript.dll`, `vbscript.dll`, `msscript.ocx` and `mswless.ocx` evidence in FND-B3-002 is **inherited** from B2's published analysis, not reproduced here. | The linkage rests on B2's published findings plus B3's own assets and the identical directory path. |
| **No external sources were consulted for product-specific claims.** No web search, vendor advisory, CVE database or licence repository was used to assert anything in this package. | Licence and provenance statements (FND-B3-008) are limited to what the font name tables themselves state. |
| **The `wasl` correspondence is a name match.** | Recorded at Medium confidence (DEP-B3-009), not as a fact. |

---

## 4. What was NOT examined, item by item

| Not examined | Where it would be | What it would resolve |
| --- | --- | --- |
| The main executable's template-selection logic | B1 | Which numeric INI is loaded for a given test; when the `_S` variant is chosen (DEP-B3-U03) |
| The INI reader's duplicate-key semantics | B1 | Whether FND-B3-010's benign duplicates could become ambiguous (FND-B3-010) |
| The INI reader's encoding handling | B1 | Whether the Arabic files are read as cp1256 explicitly or against the host ANSI codepage (FND-B3-003) |
| Whether templates are copied to a writable per-user location | B1 | The real precondition of FND-B3-001 |
| The barcode generator and its check-digit logic | B1 | Whether FND-B3-006 is Confirmed or dismissed |
| The `testtable` catalogue and the test-ID ↔ row mapping | B1's Access mirror | Which clinical test each of the 146 templates renders (DEP-B3-U05) |
| The `wasl` table schema and its contents | B1's Access mirror | Whether `Wasl.jpg` is rendered from that table (DEP-B3-U07) |
| The real `RICHTX32.OCX` (B2's copy is a 675-byte stub) | B2 / the target machine | The actual memory cost of the two oversized RTFs (FND-B3-007) |
| `mswless.ocx` and `mswless.dep` internals | B2 | Whether it hosts the HTML designer whose assets B3 supplies (DEP-B3-P02) |
| `Data/REPORT.ini` | B4/B5, or the installer | Whether the global report theme loads (FND-B3-009, DEP-B3-U01) |
| `RL_CD2019Access/Data/data_emp/` | B1 | The generic-template repository B3 instantiates (DEP-B3-U02) |
| The binding of the 5 PCR graphics to the 16 PCR templates | B1 or the instrument configuration | Which graphic appears on which molecular-biology report (DEP-B3-U04) |
| Runtime execution of client script in the HTML designer | a Windows host | Whether FND-B3-002 is a live surface or a latent one |
| Per-site identity substitution for the stationery | the installer / deployment process | Whether FND-B3-005 is a default or a deployment gap |

---

## 5. Coverage of the security review

| Review area | Status |
| --- | --- |
| Credential / secret scanning | **COMPLETE** — 251/251, one false positive |
| RTF active content (OLE, DDE, macros, fields) | **COMPLETE** — 58/58, zero hits |
| Archive safety (traversal, absolute, symlink, encryption, bombs) | **COMPLETE** — 251/251 |
| Integrity / tamperability of the clinical content | **COMPLETE** — 204 affected files examined |
| Encoding and localisation risk | **COMPLETE** — 12 non-ASCII files decoded under 3 candidate codepages |
| Font provenance and embedding permissions | **COMPLETE** — 14/14 `name` and OS/2 tables parsed; the one DSIG table decoded as ASN.1 |
| Barcode symbology review | **COMPLETE** for the fonts; **incomplete** for the generator (B1) |
| Scripting-surface review | **COMPLETE** for B3's assets; **inherited** for B2's engines |
| Vulnerability research on third-party components | **NOT APPLICABLE** — no executable third-party code in the batch |
| Dynamic exploitability testing | **NOT PERFORMED** — out of scope, and no Windows host |

---

## 6. Tool-availability notes

| Tool | Availability during this batch | Used? |
| --- | --- | --- |
| Python 3.11.2 | present | **yes** — all analysis |
| Pillow 12.2.0 | installed during recovery | **yes** — image decoding and rendering |
| ImageMagick 6.x | present | **yes** — montage rendering for visual identification |
| `file` 5.44 | present | **yes** — independent EMF/WMF confirmation |
| `unzip` / `zipfile` | present | **yes** — archive inspection and CRC verification |
| Ghidra 12.1.4 | **restored during this session** (the container had been recreated) | **no** — B3 contains no PE |
| ilspycmd 9.1.0.7988 | **restored during this session**, with the .NET 8.0.33 runtime it requires | **no** — B3 contains no .NET assembly |
| `mdbtools` 1.0.0 | installed during recovery | **no** — B3 contains no database file |
| `gdb` 13.1, `xxd` | installed during recovery | **no** — no binary to debug or hex-dump |

**Resource discipline.** The container provides 3 GiB RAM, 2 CPUs and **no
swap**. Ghidra was restored and verified but deliberately **not** exercised:
the deep-decompilation path documented for B1 — where a ~76 MB executable
repeatedly triggered a kernel OOM kill — is not needed for a batch with no
executable, and repeating it would have risked the container for no gain. The
Ghidra heap ceiling is recorded in
`/workspace/cloud-environment-bootstrap/config-templates/ghidra.env`
(`MAXMEM=1500M`) for any future batch that does contain PEs.

---

## 7. Confidence summary across the package

| Claim type | Confidence | Example |
| --- | --- | --- |
| File identity, size, hash | **Certain** — cryptographically verified | the archive digest, all 251 file digests |
| File format and structure | **Certain** — parsed, not guessed | INI sections, RTF control words, TTF tables |
| Image content | **High** — rendered and visually inspected | the HTML toolbar, the PCR charts, the letterhead |
| Font provenance | **High** — stated by the font's own name table | "Tahomax by Matthew Carter" |
| Template → discipline mapping | **High** — the templates state their own titles and subtitles | the 16 microbiology cultures |
| Cross-batch content identity | **Certain** — SHA-256 equality | the 12 `data_emp/R.ini` instantiations |
| Cross-batch *path* identity | **Certain** — identical directory path | `RL_CD2019Access/System/datareport_vh/` |
| Runtime behaviour | **None** — no evidence exists | which template is selected; whether script executes |
| Template-selection rule | **None** — not observed | the `_S` criterion |
| Font licence terms | **Low** — no licence file, no external source consulted | FND-B3-008 |
| `wasl` ↔ `Wasl.jpg` | **Medium** — name correspondence | DEP-B3-009 |

---

## 8. Reproduction

Every artefact in this package can be regenerated from the original archive:

```
# 1. safe extraction (normalises backslash paths, verifies CRC)
python3 manifests/extract_safe.py \
    /workspace/attachments/70c7c33f32ec35d4/v1.605-B3.zip \
    batches/B3/source/ --report manifests/b3-extraction-report.json

# 2. inventory + hashes + duplicate map
python3 manifests/build_inventory.py \
    "batches/B3/source/real lab system Version 1.605" \
    batches/B3/inventory/file-inventory.csv \
    batches/B3/inventory/file-hashes.sha256 \
    batches/B3/inventory/inventory-summary.json

# 3. format-specific analyses
python3 manifests/analyse_ini.py    "batches/B3/source/real lab system Version 1.605" batches/B3/evidence/ini-analysis.json
python3 manifests/analyse_rtf.py    "batches/B3/source/real lab system Version 1.605" batches/B3/evidence/rtf-analysis.json
python3 manifests/analyse_assets.py "batches/B3/source/real lab system Version 1.605" batches/B3/evidence/asset-analysis.json
python3 manifests/extract_rtf_pictures.py "batches/B3/source/real lab system Version 1.605/Cache/056.rtf" batches/B3/evidence/rtf-pictures

# 4. per-file analysis, machine-readable artefacts and the per-file report
python3 manifests/build_file_analysis.py batches/B3
python3 manifests/build_artefacts.py     batches/B3
python3 manifests/build_file_by_file.py   batches/B3
```

The scripts live in `/workspace/reallab-v1.605-engineering/manifests/` and are
copied into `batches/B3/tools/` inside the analysis package. They are
idempotent and deterministic: re-running them reproduces byte-identical
outputs except for the timestamp fields in `analysis-manifest.json`.
