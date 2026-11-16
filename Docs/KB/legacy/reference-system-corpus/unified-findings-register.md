# Unified Findings Register — RealLab CD 2019 v1.605

**Consolidated:** 2026-10-31 (UTC) · **Static analysis only.** No exploit was
attempted, no credential validated, no external system contacted.
Machine-readable: `machine-readable/unified-findings-register.csv` and `.json`.

---

## 1. How the register is built

**66 original findings** from five investigations are merged into **41 unified
subjects**. **No original identifier was renumbered or discarded.**

| Origin | Findings | Note |
| --- | ---: | --- |
| **B1** | 19 | original investigation, supplied via the handover archive |
| **B2** | 13 | original investigation, supplied via the same archive — **not independently re-investigated** |
| **B3** | 10 | executed by this agent |
| **B4** | 13 | executed by this agent |
| **FIN** | 11 | the primary-executable investigation, executed by this agent |
| **Total** | **66** | → **41 unified subjects** |

**Severity profile across the originals:** High **17** · Medium **27** · Low
**15** · Info **7**.

**8 subjects were reported by more than one investigation.** Those are
consolidated into one entry with **every original identifier and the full
per-origin record retained** in the JSON register. The remaining 33 are
single-source.

---

## 2. The consolidated High findings

| Unified | Subject | Severity | Confidence | Original IDs | Origins |
| --- | --- | --- | --- | --- | --- |
| **U-001** | The vendor's own manual documents default operator credentials 'admin' / 'admi | High | Confirmed | FND-001, FND-B4-002 | B1, B4 |
| **U-002** | Patient-database backup executed through generated .bat/.sql files with a conf | High | Confirmed | FND-002, FND-006, FND-018, FND-B2-003, FND-B4-006 | B1, B2, B4 |
| **U-003** | Hardcoded remote patient-database credential in the .NET e-mail component | High | Confirmed | FND-003 | B1 |
| **U-004** | The installer creates the SQL Server 'sa' account with a hardcoded password an | High | Confirmed | FND-004, FND-B4-001, FND-B4-003, FND-F-003, FND-F-007, FND-F-008 | B1, B4, FIN |
| **U-005** | Bulk-deletion maintenance tools targeting every clinical table | High | Confirmed | FND-005 | B1 |
| **U-006** | Installer scripts execute unverified payloads by absolute path and attach the  | High | Confirmed | FND-007, FND-B2-001, FND-B3-002, FND-B4-005, FND-B4-008 | B1, B2, B3, B4 |
| **U-016** | The SMS gateway is called over plain HTTP with the account credential in the U | High | Confirmed | FND-017, FND-F-002 | B1, FIN |
| **U-018** | 41 of 62 PE binaries are unsigned, including every vendor and third-party bina | High | Confirmed | FND-B2-002 | B2 |
| **U-037** | The SQL Server sa password is assembled at run time from separate literal frag | High | Confirmed | FND-F-001 | FIN |

---

## 3. The eight cross-investigation subjects

These are where the investigations genuinely converged — the same defect seen
from different directions. **Each retains every original identifier.**

| Unified | Subject | Original IDs | Origins |
| --- | --- | --- | --- || **U-001** | admin | FND-001, FND-B4-002 | B1, B4 |
| **U-002** | sql | FND-002, FND-006, FND-018, FND-B2-003, FND-B4-006 | B1, B2, B4 |
| **U-004** | password | FND-004, FND-B4-001, FND-B4-003, FND-F-003, FND-F-007, FND-F-008 | B1, B4, FIN |
| **U-006** | script | FND-007, FND-B2-001, FND-B3-002, FND-B4-005, FND-B4-008 | B1, B2, B3, B4 |
| **U-007** | aslr | FND-008, FND-B2-006, FND-B4-004, FND-F-005 | B1, B2, B4, FIN |
| **U-008** | font | FND-009, FND-B3-006, FND-B3-008 | B1, B3 |
| **U-015** | dep | FND-016, FND-B2-005, FND-B2-008, FND-B4-011, FND-B4-012, FND-F-011 | B1, B2, B4, FIN |
| **U-016** | http | FND-017, FND-F-002 | B1, FIN |

**The most instructive of these is the credential subject.** B1 found
cleartext SQL credentials in shipped `.udl` and `BASE.ini` files; B2 found the
vendor's `NOTEX.dll` reproducing the same credential-assembly logic; the
executable investigation found the three literal fragments that are
concatenated at run time. **Three investigations, one defect, now traced to
its mechanism.**

---

## 4. How later work affected each earlier finding

| Relationship | Examples |
| --- | --- |
| **Confirmed at source** | B1 FND-001/002/003/007 (SQL credentials) — the executable's literal fragments. B1 FND-008 and B2 FND-B2-006 (no ASLR/DEP) — `DllCharacteristics 0x0000`. B1 FND-017 (SMS credential in a URL) — the reconstructed `navigate2` call. B2 FND-B2-001 (script engines) — the HTML tag assets and the SP6 redistributable in the same directory |
| **Extended** | B3 FND-B3-008 (renamed Tahoma) — B4's `SETUP.LST` shows it installed to `$(Font)` with `$(Shared)`. B2 FND-B2-011 (the `RICHTX32.OCX` stub) — B4's cabinets carry the **real** control. B3 DEP-B3-U03/U05 (test-ID mapping) — the manual's *"Arrange number"* plus the `testid Smallint` column in `LBC`/`IgEallergy` |
| **Corrected** | **Ghidra's outcome.** Consolidation pass 1 stated the run was *"left running in the background rather than killed"*. The observed final state is a **kernel SIGKILL, exit 137**. The earlier statement is superseded, and the correction is recorded in `primary-executable-technical-reference.md` §7 and in the session log |
| **Not superseded** | Every earlier finding stands as recorded. Where later work extends one, the extension is recorded **in the new finding**, and the original is left untouched |

---

## 5. The register's structural caveats

1. **VB6 pools string literals into one contiguous block.** Adjacency between
   two recovered strings proves nothing. Every inference that depends on
   proximity is flagged in the source documents, and **no finding in this
   register rests on it**.
2. **B2's 13 findings are inherited, not re-derived.** They were read from the
   supplied package. Where this register states a B2 finding as fact, it means
   *"recorded by the B2 analysis and not contradicted by later work"*.
3. **Severity is the highest severity among the consolidated records.** Where
   one investigation rated a subject Low and another Medium, the entry shows
   Medium — the original ratings are preserved in the JSON.
4. **No finding asserts exploitability.** Every entry is a statement about
   content or about vendor documentation.
5. **`Data/REPORT.ini` and `unrar.dll` remain open** — they are questions, not
   findings, and appear in the unresolved register.

---

## 6. Where the per-finding detail lives

| Need | Go to |
| --- | --- |
| The unified subject and its original IDs | this document, §2–§3 |
| The **full text** of every finding per origin | `machine-readable/unified-findings-register.json` → `unified[].per_origin_records[]` |
| The narrative write-up | `security-and-technical-risk-register.md` |
| The original report | `context/B1/`, `context/B2/`, `batches/B3/reports/`, `analysis/v1.605-B4/reports/`, `final-integrated-analysis/reports/` |
