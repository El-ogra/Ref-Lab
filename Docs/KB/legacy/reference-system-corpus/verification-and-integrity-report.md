# Verification and Integrity Report — RealLab CD 2019 v1.605

**Consolidated:** 2026-10-31 (UTC) · **Static analysis only.**

---

## 1. Verification performed during this consolidation

| Check | Scope | Result |
| --- | --- | --- |
| Workspace discovery | every file under `/workspace/reallab-v1.605-engineering` | **855 artefacts**, 671 distinct digests |
| Handover integrity | the 36 B1 + B2 files | **36 / 36 byte-identical** |
| B3 self-consistency | B3's 25 recorded artefacts | **24 / 25** — the one exception is the documented self-reference |
| B3 modification check | every file under `batches/B3` | **0** modified after the B4 stage began |
| B4 self-consistency | B4's 69 inventory rows | **69 / 69** hashes re-verified |
| B4 modification check | every file under `analysis/v1.605-B4` | **0** modified after the B4 stage began |
| Executable identity | the supplied copy vs B1's recorded digest | **identical** (`c54ce6c8…18df93c0`) |
| JSON validity | every register in this package | **all parse** |
| CSV structure | every register in this package | **single column count throughout** |
| Control characters | every text artefact in this package | **0** |
| Internal links | every relative link in this package | **0 broken** |
| Evidence references | every evidence ID cited by a finding | **0 dangling** |
| Infrastructure | `verify.sh` | **22 / 22 pass** |
| Infrastructure | `checksums.txt` | **8 / 8** |
| Delivered archives | `sha256sum -c` | **all OK** |

---

## 2. The original uploads — all four unmodified

| Upload | Size | SHA-256 | Verified |
| --- | ---: | --- | :-: |
| `real lab system.zip` | 10,490,454 | `17a266e0d6e68b07513bf66366d8c0f4c47c9ecdbf6d7679f85d026cea734bb6` | ✅ |
| `v1.605-B3.zip` | 3,523,212 | `2fb942c377d1e06b6ba473302e3ad8e8ba8a6faa39513bb9a296a73d64ee3a7b` | ✅ |
| `v1.605-B4.zip` | 63,839,512 | `7aff65e12e5334e6ebaad6f1ab19965d665a9d4ec968c396ed9b2f294f5e4a6c` | ✅ |
| `RealLab-v1.605-Engineering-Handover.zip` | 864,737 | `ed81ff46a28f3c8441234019eb391deeef77365987813aad5c05ba6c9cb27f0f` | ✅ |

**The B1 target archive is the one upload that is absent** — see
`project-evidence-and-source-inventory.md` §5. It was **not** reconstructed.

---

## 3. Prior consolidation packages — all unchanged

| Archive | SHA-256 |
| --- | --- |
| `v1.605-B3-reports.tar.gz` | `9d62861fbffb5fb11bc3ccab5ce6acd113cc31379b4bac8a384ebe454396dbd9` |
| `v1.605-B3-analysis-package.tar.gz` | `83536f1b5d1c29c000bfef5bc872d315632dcc0059c315e839fd59e7bf1c0892` |
| `v1.605-B4-reports.tar.gz` | `4adc2f6f85e9cf7934be5b786d2a254bf85fc6883806be876eaf2fe075b0243d` |
| `v1.605-B4-analysis-package.tar.gz` | `68c433b717293aa884b74155dccfc4361670f12bd1531966198268a064d6d7e8` |
| `v1.605-final-integrated-analysis.tar.gz` | recorded in `final-integrated-analysis-checksums.txt` |

---

## 4. Infrastructure state

| Item | State |
| --- | --- |
| `scripts/bootstrap.sh` | present, executable, syntax-checked |
| `scripts/restore.sh` | present, executable, syntax-checked |
| `scripts/verify.sh` | present, executable, syntax-checked — **22 / 22** |
| `scripts/audit.sh` | present, executable, syntax-checked |
| `config-templates/ghidra.env` | present — now sets `GHIDRA_HEADLESS_MAXMEM` |
| `config-templates/jdk.env` | present — Temurin JDK 21 |
| `versions.json`, `checksums.txt` | present and current |
| `MANIFEST.md`, `README.md` | present, with the change log |

**Two defects were found in this infrastructure during the executable
investigation and fixed**, with the reasons recorded in the infrastructure's
own `README.md` §8 and `MANIFEST.md` §4:

1. `bootstrap.sh` installed **Java 17**; Ghidra 12.1.4 requires **JDK 21+**.
2. The heap cap was set through **`MAXMEM`**, which Ghidra 12.1.4's
   `analyzeHeadless` **ignores** — it reads `GHIDRA_HEADLESS_MAXMEM`. Every
   Ghidra run in this project therefore used a **2 GB heap on a 3 GiB
   swap-less container**, which is the most likely single cause of the
   project's recorded OOM history.

---

## 5. What was deliberately NOT done

| Not done | Why |
| --- | --- |
| B3's manifest self-reference was **not** corrected | the instruction is to preserve earlier deliverables; it is documented with a proposed fix instead |
| B4's `pe_analyse.py` entry-point offset was **not** patched | no B4 finding depends on it; the correct value was established independently and is documented |
| The B1 target archive was **not** reconstructed | it is absent; reconstructing it would be fabrication |
| Nothing from any batch was **executed** | the static-analysis constraint applies throughout the project |
| No credential was **validated**, no URL fetched, no database contacted | the same constraint |
| No patient data was **extracted** | none was available, and none was needed |

---

## 6. The one correction this consolidation made

**Ghidra's outcome.** Consolidation pass 1 stated that the headless run was
*"left running in the background rather than killed"*. That was accurate when
written; the observed final state is a **kernel SIGKILL, exit 137** during
auto-analysis, with a 23 KB project and no function inventory. The
correction is recorded in
`final-integrated-analysis/reports/primary-executable-reverse-engineering.md`
§9, in the session log, and in `primary-executable-technical-reference.md` §7.

**No other statement in any earlier deliverable was found to be wrong.**
