# Methodology and Tools — Batch B3 of RealLab CD 2019 v1.605

**Batch:** B3 · **Archive:** `v1.605-B3.zip` · 3,523,212 bytes
**SHA-256:** `2fb942c377d1e06b6ba473302e3ad8e8ba8a6faa39513bb9a296a73d64ee3a7b`
**Analysis date:** 2026-10-31 (UTC) · **Method:** static analysis only

---

## 1. Execution environment as found

The container had been **recreated** since the previous session. The evidence:

| Check | Finding |
| --- | --- |
| `/workspace/cloud-environment-bootstrap` | **absent** — the entire previously documented infrastructure workspace was gone |
| Node.js | v22.19.0 (previously documented as v22.23.4) — **different build** |
| npm | 10.9.3 (previously 10.9.9) — **different build** |
| .NET | 9.0.316 SDK (previously ".NET 8 runtime") — **different version** |
| Eclipse Temurin JDK | **absent** (no `java` on PATH) |
| Ghidra 12.1.4 | **absent** |
| ilspycmd 9.1.0.7988 | **absent** |
| gdb 13.1 | **absent** |
| xxd | **absent** |
| mdbtools | **absent** |
| `/workspace` | present, **NFS** — the persistent volume |
| `/tmp` | `overlay` — container-local and **ephemeral** |

The persistent NFS volume had survived; the **installed executables had not**.
That is exactly the failure mode the previous session's recovery infrastructure
exists to address, and it is why the recovery scripts are stored on the volume
rather than in the image.

### Resource profile

| Resource | Value | Consequence |
| --- | --- | --- |
| Memory | 3,072 MiB, **0 MiB swap** | Any JVM-based analysis must be heap-capped; the B1-era Ghidra OOM failure mode is live |
| CPUs | 2 | No meaningful parallelism; all analysis was run serially |
| Disk | 500 GB NFS | No pressure; the 42.9 MiB extracted batch and the 544 MiB Ghidra payload both fit |

---

## 2. Infrastructure recovery

The four required scripts were **reconstructed** — the previous versions did
not survive the container recreation, and no attempt was made to pretend
otherwise.

| Script | Path | Purpose |
| --- | --- | --- |
| `bootstrap.sh` | `/workspace/cloud-environment-bootstrap/scripts/bootstrap.sh` | OS packages, language runtimes, Python modules; records versions |
| `restore.sh` | `/workspace/cloud-environment-bootstrap/scripts/restore.sh` | restores the analysis toolchain (Ghidra, ilspycmd) idempotently |
| `verify.sh` | `/workspace/cloud-environment-bootstrap/scripts/verify.sh` | 21 functional checks with a PASS/FAIL summary and exit code |
| `audit.sh` | `/workspace/cloud-environment-bootstrap/scripts/audit.sh` | environment audit; writes `versions.json` |

All four are executable, idempotent, safe to re-run, free of credentials, and
stored on the **persistent** volume.

### What the recovery actually did

| Step | Outcome |
| --- | --- |
| `apt-get install` of `xxd`, `mdbtools`, `p7zip-full`, `gdb`, `binutils`, `default-jre-headless` | **succeeded** — OpenJDK 17.0.21 headless |
| Node / npm | already present (v22.19.0 / 10.9.3) — skipped, no reinstall |
| .NET | already present (SDK 9.0.316) — skipped |
| **.NET 8.0.33 runtime** | **installed** — `ilspycmd` 9.1.0.7988 targets `net8.0` and could not run on the .NET 9 runtime alone |
| Ghidra 12.1.4 | **restored** — the release asset is a **ZIP** (`ghidra_12.1.4_PUBLIC_20260921.zip`, 544 MiB), not a tarball; `restore.sh` now resolves the real asset URL through the GitHub API rather than guessing a filename |
| ilspycmd 9.1.0.7988 | **restored** via `dotnet tool install --global` |
| Pillow 12.2.0 | **installed** via pip |
| `verify.sh` | **21 / 21 checks pass**, exit code 0 |

The Ghidra heap ceiling is written to
`config-templates/ghidra.env` as `MAXMEM=1500M`, below the container's 3 GiB.

### Tool-availability discipline applied to this batch

Ghidra and ilspycmd were restored and verified, then **deliberately not
used**: B3 contains no PE and no .NET assembly. `mdbtools` was installed and
left unused: B3 contains no database. This is the resource-aware choice the
previous batch's OOM history called for — the expensive path was available and
was not walked for no gain.

---

## 3. Analysis procedure

### Step 1 — archive identification and safety assessment

The archive was fingerprinted before anything was extracted, and its 251
central-directory entries were inspected for absolute paths, drive-letter
paths, UNC paths, `..` traversal, symlink and other non-regular entries,
encryption, and compression ratios.

**Finding:** 0 violations of every kind; maximum single-entry compression
ratio 40.76:1 (a legitimately compressible RTF), no archive-bomb condition.

**A real property was found and handled:** the archive stores its directory
structure with **backslash** separators inside the entry names, so a naive
Linux `unzip` flattens all 251 files into one directory with literal
backslashes in their names. The extractor normalises `\` → `/` before
resolving each destination and re-checks the resolved path against the
extraction root.

### Step 2 — safe extraction

`manifests/extract_safe.py` performed the extraction:

* 251 files written, **0 skipped**, **251 / 251 CRC32-verified**;
* regular files at mode 0644;
* nothing written outside the destination;
* the original archive preserved unmodified in the attachment directory.

### Step 3 — cryptographic inventory

`manifests/build_inventory.py` produced per-file SHA-256, size, extension,
magic-byte classification, content type and the duplicate map (143 distinct
digests; 71 duplicate groups covering 179 files).

### Step 4 — format-specific analysis

Four custom Python parsers were written for this batch, because the formats are
unusual and no standard tool parses them:

| Script | Purpose | Notable implementation detail |
| --- | --- | --- |
| `analyse_ini.py` | sections, keys, values, encoding, line endings | counts key families by normalising digit runs, so `ITEM01_1` and `ITEM14_3` collapse to one family |
| `analyse_rtf.py` | control-word parse, text extraction, `\info`, font table | resolves `\'hh` against the document's own `\ansicpg`; consumes numeric control-word parameters so `\f1` does not leak into the text; skips destination-only groups (`fonttbl`, `colortbl`, `stylesheet`, `listtable`, `info`) |
| `analyse_assets.py` | GIF/JPEG/TTF structure | parses the TTF table directory, the `name` table (both UTF-16BE and Latin-1 encodings) and OS/2 `fsSelection` |
| `extract_rtf_pictures.py` | embedded picture extraction | brace-matches the `\pict` group, strips the EMF header record, and identifies the placeable-WMF 22-byte header |

Two implementation traps were hit and fixed during this work, and both are
worth recording because they produced **silent** wrong answers:

1. **Numeric control-word parameters.** The first RTF text extractor emitted
   the digits of control words (`\f1`, `\fs20`, `\lang1033`) into the output
   text, so reference ranges read "Adult: 42136 U/L" instead of "Adult:
   42–136 U/L". Fixed by consuming the optional numeric parameter and one
   trailing space after every control word.
2. **Whole-string slicing in a hot loop.** The corrected parser used
   `re.match(pattern, text[i:])`, which copies the entire 16.9 MB document on
   every iteration — the run exceeded 30 seconds and then timed out. Fixed by
   using `re.compile(...).match(text, i)`, which takes a position argument.

### Step 5 — visual identification

Three asset groups could not be identified from structure alone — the
report-designer GIFs, the PCR JPEGs and the stationery JPEGs. Each was
**rendered to a PNG montage and visually inspected**, and the montages are
kept as evidence:

| Evidence file | What it shows |
| --- | --- |
| `evidence/preview/datareport_vh_toolbar.png` | the 8 HTML-editor toolbar buttons at 6× |
| `evidence/preview/datareport_vh_tags.png` | the 6 HTML tag markers at 6× |
| `evidence/preview/pcr_group.png` | the 5 PCR graphics with their amplification bands and the principle schematic |
| `evidence/preview/stationery_group.png` | the 9 stationery bands with their placeholder text |

This is what turned "14 small GIF files" into "an HTML authoring toolbar and
an HTML element palette" — the single most consequential finding in the
batch.

### Step 6 — cross-batch correlation

Every one of the 251 B3 digests was compared against B1's 140 published
digests and B2's 70. This is what produced the `data_emp/R.ini` identity
result and the confirmation that B3 shares **no** file with B2.

### Step 7 — security scanning

* Credential/secret scan over all 251 files — one false positive ("secreted").
* RTF active-content scan over all 58 documents — zero hits for OLE objects,
  DDE links, macros, fields or embedded packages.
* Duplicate-key analysis within every INI section.
* Encoding detection with three-way candidate decoding for the 12 non-ASCII
  files.

### Step 8 — documentation and artefacts

15 Markdown reports and 7 machine-readable artefacts, generated where the
content is systematic (`file-by-file-analysis.md`, all JSON) and written by
hand where it requires judgement (the analytical reports).

---

## 4. Tool inventory and versions

| Tool | Version | Role in B3 |
| --- | --- | --- |
| Python | 3.11.2 | all parsing and analysis |
| Pillow | 12.2.0 | image decoding, rendering, montage construction |
| ImageMagick | 6.x | montage rendering |
| `file` | 5.44 | independent confirmation of the extracted EMF/WMF |
| `unzip` / Python `zipfile` | — | archive structure, safety assessment, CRC verification |
| Ghidra | 12.1.4 | **restored and verified, deliberately unused** — no PE in the batch |
| ilspycmd | 9.1.0.7988 | **restored and verified, deliberately unused** — no .NET assembly |
| mdbtools | 1.0.0 | **restored, deliberately unused** — no database in the batch |
| gdb | 13.1 | restored, unused |
| xxd | — | restored, unused |
| Node.js | v22.19.0 | restored, unused |
| .NET | 9.0.316 (+ 8.0.33 runtime) | restored, used only to host ilspycmd |

---

## 5. Reproducibility

All analysis scripts are in `/workspace/reallab-v1.605-engineering/manifests/`
and are copied into `batches/B3/tools/`. They are deterministic and
idempotent; the only non-deterministic field is the `generated_utc` timestamp
in `analysis-manifest.json`.

The full reproduction sequence is listed in `coverage-and-limitations.md` §8.

---

## 6. Discipline observed

* **Nothing was executed** from the batch — no binary, no script, no document.
* **No external system was contacted** and no endpoint in the batch was tested.
* **No credential was validated** — none exists in the batch.
* **No advisory or CVE is asserted** — the batch contains no executable
  third-party code.
* **No external source was consulted** for a product-specific claim; the
  licence and provenance statements in FND-B3-008 are limited to what the font
  name tables state.
* **B1's and B2's reports were not modified.** The B3 documents cite them and
  record the relationships; they do not rewrite them.
* **The original uploaded archives were preserved unmodified.**
