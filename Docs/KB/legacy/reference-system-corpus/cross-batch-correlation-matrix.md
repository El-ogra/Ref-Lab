# Cross-Batch Correlation Matrix — RealLab CD 2019 v1.605

**Consolidated:** 2026-10-31 (UTC) · **Static analysis only.**
Machine-readable: `machine-readable/cross-batch-correlation-matrix.csv` and `.json`.

> **The purpose of this document is to show how the completed investigations
> relate to one another within a single product — not to compare versions.**
> Every relationship below is a **SHA-256 identity** or a **path identity**
> established from the actual files.

---

## 1. The digest sets compared

| Set | Distinct digests | File entries | Source |
| --- | ---: | ---: | --- |
| **B1** | 140 | 158 | `context/B1/file-inventory.csv` |
| **B2** | 53 | 70 | `context/B2/file-hashes.sha256` |
| **B3** | 143 | 251 | `batches/B3/inventory/file-hashes.sha256` |
| **B4** | 66 | 69 | `analysis/v1.605-B4/file-hashes.sha256` — the 69 batch files |
| **B4-derived** | 64 | 96 | the members extracted from B4's two product cabinets, hashed during the B4 analysis |
| **EXE** | 1 | 1 | the primary executable |

**Note on B4-derived.** B4's published manifest covers only its 69 archive
members. The 96 members extracted from the two product cabinets were hashed
during the B4 analysis and recorded in
`analysis/v1.605-B4/evidence/cross-batch-identity.json`. They are listed as a
separate set so that the provenance of each overlap stays visible.

---

## 2. The pairwise result — four proven overlaps

| Pair | Byte-identical files | What it proves |
| --- | ---: | --- |
| **B1 ↔ EXE** | **1** | The executable supplied for the final stage **is** the file B1 analysed. Same bytes, same digest `c54ce6c8…18df93c0` |
| **B1 ↔ B3** | **2** | B3's live templates `65/66/418` (+ `_S`) are **instances of B1's generic prototype** `data_emp/R.ini` |
| **B2 ↔ B4-derived** | **25** | B4's installer payload contains **the same component binaries B2 catalogued** — the installer is their origin |
| **B3 ↔ B4-derived** | **5** | The **five product fonts** B3 analysed are **the ones the installer deploys machine-wide** |

**All other pairs are empty — and that is itself informative.** B1 is the
application, B2 the platform, B3 the content and B4 the deployment: they
overlap only where one genuinely ships the other's artefacts.

---

## 3. The overlaps in detail

| Pair | File in the first set | File in the second set | Shared digest |
| --- | --- | --- | --- || B1 ↔ B3 | `RL_CD2019Access/Data/data_emp/R_S.ini` | `Data/418_S.ini` | `3cae813787c1c249…` |
| B1 ↔ B3 | `RL_CD2019Access/Data/data_emp/R.ini` | `Data/418.ini` | `c0ac220f214d09a6…` |
| B1 ↔ EXE | `RL_CD2019Access/System/real lab system.exe` | `executable-analysis/extracted/real lab system.exe` | `c54ce6c8c98a679e…` |
| B2 ↔ B4-derived | `./RL_CD2019Access/OCX/MSCOMCT2.OCX` | `analysis/v1.605-B4/extracted-cab/WIN7/MSCOMCT2.OCX` | `160b0cef5e9ed57c…` |
| B2 ↔ B4-derived | `./RL_CD2019Access/OCX/MSDBRPTR.DLL` | `analysis/v1.605-B4/extracted-cab/WIN7/MSDBRPTR.DLL` | `2d7d1659ea576e38…` |
| B2 ↔ B4-derived | `./RL_CD2019Access/OCX/RICHTX32.OCX` | `analysis/v1.605-B4/extracted-cab/WIN7/RICHTX32.OCX` | `318cc48cbcfaba95…` |
| B2 ↔ B4-derived | `./MSCOMM32.OCX` | `analysis/v1.605-B4/extracted-cab/WINXP/MSCOMM32.OCX` | `38779de94e9aa40e…` |
| B2 ↔ B4-derived | `./RL_CD2019Access/OCX/MSSTDFMT.DLL` | `analysis/v1.605-B4/extracted-cab/WIN7/MSSTDFMT.DLL` | `3d5c3074fc645da3…` |
| B2 ↔ B4-derived | `./RL_CD2019Access/OCX/MSDATLST.OCX` | `analysis/v1.605-B4/extracted-cab/WIN7/MSDATLST.OCX` | `46a5a0ea76e8e873…` |
| B2 ↔ B4-derived | `./RL_CD2019Access/OCX/DBGRID32.OCX` | `analysis/v1.605-B4/extracted-cab/WIN7/DBGRID32.OCX` | `6cc3e18193118e5d…` |
| B2 ↔ B4-derived | `./RL_CD2019Access/OCX/STDOLE2.TLB` | `analysis/v1.605-B4/extracted-cab/WIN7/STDOLE2.TLB` | `6ea525bface5467c…` |
| B3 ↔ B4-derived | `Fonts/WBCW_UPCEAN.ttf` | `analysis/v1.605-B4/extracted-cab/WIN7/WBCW_UPCEAN.ttf` | `1827a77ff26fd2ec…` |
| B3 ↔ B4-derived | `Fonts/ais symbol.ttf` | `analysis/v1.605-B4/extracted-cab/WIN7/ais symbol.ttf` | `80d3fdb04904305d…` |
| B3 ↔ B4-derived | `Fonts/MCS Taybah.ttf` | `analysis/v1.605-B4/extracted-cab/WIN7/MCS Taybah.ttf` | `b93c3c3e0cbdbc3e…` |
| B3 ↔ B4-derived | `Fonts/ais lab.ttf` | `analysis/v1.605-B4/extracted-cab/WIN7/ais lab.ttf` | `be1aee92d2751f8b…` |
| B3 ↔ B4-derived | `Fonts/WBCWS_UPCEAN.ttf` | `analysis/v1.605-B4/extracted-cab/WIN7/WBCWS_UPCEAN.ttf` | `e631211461e7bab6…` |

*(The complete list — every shared digest with every path it appears under —
is in `machine-readable/cross-batch-correlation-matrix.csv`, one row per digest,
with the `present_in` column naming the investigations that hold it.)*

### 3.1 B2 ↔ B4-derived — 25 components

The ActiveX control register and the runtime stack, as the installer deploys
them: `msvbvm60.dll`, `MSDBRPTR.DLL`, `MSSTDFMT.DLL`, `MSBIND.DLL`,
`OLEAUT32.DLL`, `OLEPRO32.DLL`, `RICHED32.DLL`, `RICHTX32.OCX`, `STDOLE2.TLB`,
`COMCAT.DLL`, `ASYCFILT.DLL`, `VB6STKIT.DLL`, `msado27.tlb` (XP variant),
`mscomctl.ocx`, `MSCOMM32.OCX`, `MSFLXGRD.OCX`, and the controls
`COMDLG32.OCX`, `DBGRID32.OCX`, `MCI32.OCX`, `MSADODC.OCX`, `MSCHRT20.OCX`,
`MSCOMCT2.OCX`, `MSDATGRD.OCX`, `MSDATLST.OCX`, `TABCTL32.OCX`,
`XP_BUTTONS.ocx`, `Redsoul911XPCOmponents.ocx`, `RgheedButtonControlV2.ocx`.

**This resolves B2's FND-B2-011.** B2 found a 675-byte PE-shaped *stub* named
`RICHTX32.OCX` in `datareport_vh/` and could not explain it. B4's cabinets
carry the **real 212,240-byte control** — byte-identical to B2's
`RL_CD2019Access/OCX/RICHTX32.OCX`. Two different files share the name; the
`datareport_vh` copy is a marker in the redistributable folder, and the OCX
register holds the working control the installer deploys.

### 3.2 B3 ↔ B4-derived — 5 fonts

`ais lab.ttf`, `ais symbol.ttf`, `MCS Taybah.ttf`, `WBCW_UPCEAN.ttf`,
`WBCWS_UPCEAN.ttf`.

**This closes B3's FND-B3-008.** B3 found the renamed Microsoft Tahoma
*shipped* in `Fonts/`. B4's `SETUP.LST` shows it installed to `$(Font)` with
`$(Shared)` — **into every user's font directory on every clinical
workstation**. The provenance risk became a deployment risk.

### 3.3 B1 ↔ B3 — 2 template instances

B3's `Data/65.ini`, `66.ini`, `418.ini` and their `_S` variants are
byte-identical to B1's `RL_CD2019Access/Data/data_emp/R.ini` and `R_S.ini` —
the generic prototypes in B1's `data_emp/` repository.

**The deployment model is now explicit:** a generic prototype is instantiated
under three different numeric test IDs. B4's manual explains why the numbers
matter — *"the analysis numbers, and the identity of the number specified for
each analysis, must not be changed"* — and B3's template-ID space contains
**deliberate gaps**.

---

## 4. Component-level correspondence — beyond byte identity

Some relationships are established by **path or name agreement**, not by
digest. Those are recorded here and labelled as such.

| From | To | Basis | Class |
| --- | --- | --- | --- |
| The executable's `Result*` / `Report*` objects | B3's per-test INI templates | object naming + template key vocabulary + the manual's UI | **STRONGLY INFERRED** |
| The executable's `reportsetup` / `normal_test` | B4's manual topics 9 and 11 | object names + the manual's described dialogs | **STRONGLY INFERRED** |
| The executable's `barcode` (76 methods) | B4's manual topic 5 | object name + the manual's three-code scheme | **STRONGLY INFERRED** |
| The executable's `labtolab` / `moenysharekat` | B4's manual topic 13 (send-out samples) | object names | **STRONGLY INFERRED** |
| The executable's `users` / `login` | B1's `useraccounts` finding | object names + the executable's own DDL | **CONFIRMED** (the DDL) |
| The executable's `instrument` (29 methods) | B1's `instrument` table | object name + B1's schema | **CONFIRMED** (the schema) |
| The executable's `CREATE TABLE` statements | B1's recorded DDL | **verbatim** | **CONFIRMED** |
| The executable's connection fragments | B1's `.udl` / `BASE.ini` credentials; B2's `NOTEX.dll` fragments | identical text | **CONFIRMED** |
| B4's `SETUP.LST` `$(WinSysPath)` entries | B2's `RL_CD2019Access/OCX/` register | the manifest + digest identity | **CONFIRMED** |
| B4's `SETUP.LST` `$(Font)` entries | B3's `Fonts/` | the manifest + digest identity | **CONFIRMED** |
| B4's `datareport_vh/` redistributables | B2's engine group; B3's designer assets | **the same directory path** | **CONFIRMED** (path) |
| The executable's `Patients.grantN` | B1's `patientinfo` pricing columns | name + type | **TENTATIVE** |
| The executable's `MyLabSetting` gateway fields | B4's manual's SMS description | field names + the reconstructed call | **CONFIRMED** |
| B4's `WINXP` `msado27.tlb` 2.81.1132.0 | B2's catalogued `msado27.tlb` | version + digest | **CONFIRMED** |

---

## 5. The findings-provenance matrix

| Finding family | B1 | B2 | B3 | B4 | EXE |
| --- | :-: | :-: | :-: | :-: | :-: |
| SQL / database credentials | ●● | ● | | | ●● |
| `sa` account creation | | | | ●● | ● |
| Operator credentials | ● | | | ● | ●● |
| Maintenance / erasure | ● | | | ●● | ● |
| Binary hardening (ASLR/DEP/signing) | ● | ●● | | ● | ●● |
| Font provenance | | | ●● | ● | ● |
| Script surfaces | | ●● | ●● | ●● | ●● |
| Machine-wide deployment / COM registration | | ●● | | ●● | ● |
| Report-template integrity | | | ●● | | ● |
| Per-workstation report layout | | | | ● | ●● |
| Schema migration by the client | | | | | ●● |
| SMS / external communication | ● | | | | ●● |
| Authorisation model (`grantN`, print secrets) | | | | ● | ●● |
| Installer payload integrity | | | | ●● | |
| Undocumented MySQL back end | | | | ●● | |
| Avira orphan descriptors / symbol files | | | | ●● | |
| Vendor identity | | | | ● | ●● |
| Product version `1.605` | | | | ● | ●● |
| Product executable | ●● | | | ● | ●● |

**● = contributes · ●● = primary source**

---

## 6. Defects preserved, not silently fixed

| Defect | Where | Why preserved |
| --- | --- | --- |
| **B3's manifest self-reference exception** — `batches/B3/reports/analysis-manifest.json` records the pre-regeneration digest of B3's own `deliverables-index.md` | B3's own artefact | The instruction is to preserve earlier deliverables. Documented in `contradictions-limitations-and-unresolved-questions.md` §1.1 and in B4's `coverage-and-limitations.md` §9, with a proposed fix that was **not applied** |
| **B4's `pe_analyse.py` entry-point offset** — for a PE32 image it reads the entry point at `optional_header + 44` instead of `+ 16` | `analysis/v1.605-B4/tools/pe_analyse.py` | No B4 finding depends on the field, so no B4 conclusion is affected. Documented with the correct value established by a direct header read (RVA `0x7142C`) |
| **Ghidra's `MAXMEM` variable is ignored** | Ghidra 12.1.4's own `support/analyzeHeadless` | A third-party behaviour, not a project artefact. The project's side of it **was** fixed, and the fix is recorded in the infrastructure's own change log |

---

## 7. How to reproduce the correlation

```bash
cd /workspace/reallab-v1.605-engineering
python3 executable-analysis/tools/build_correlation_matrix.py
python3 executable-analysis/tools/build_source_inventory.py
```

Both are deterministic and read-only over the workspace.
