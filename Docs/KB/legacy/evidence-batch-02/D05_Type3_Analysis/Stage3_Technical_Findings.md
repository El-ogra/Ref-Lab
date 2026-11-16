# Stage 3 — Evidence Type 3 Analysis: Analytical Synthesis & Process Evidence

**Product:** RealLab v1.605 ("RealLab CD 2019")
**Evidence type:** 3 of 3 — prior investigators' consolidated findings, unresolved questions, coverage/traceability, cross-version matrices, ledgers, tooling records
**Files:** 160 · **Bytes:** 6,767,476 (74 tooling scripts, 17 batch manifests, 55 registers/matrices)
**Method:** static read of the synthesis artefacts; no re-derivation of target evidence. Machine-readable output in `Type3_Synthesis_Findings.json`.

---

## 1. Consolidated findings register (FIND-T3-001)

`unified-findings-register.csv` holds **41** consolidated findings (9 High · 16 Medium · 11 Low · 5 Info), each traceable to origin batches B1|B2|B3|B4|FIN.

**High severity (9):**
- **U-001** default `admin`/`admin` operator credentials documented in the vendor manual
- **U-002** patient-DB backup via generated `.bat`/`.sql` with configurable server/destination
- **U-003** hardcoded remote patient-DB credential in the .NET e-mail component
- **U-004** installer creates SQL Server `sa` with a hardcoded password
- **U-005** bulk-deletion maintenance tools over every clinical table
- **U-006** installer scripts execute unverified payloads by absolute path; attach DB from hardcoded `D:\Lab_data`
- **U-016** SMS gateway over plain HTTP with credentials in the URL
- **U-018** 41 of 62 PE binaries unsigned
- **U-037** SQL `sa` password assembled at runtime from fragments, shipped cleartext

Notable Medium items include U-015 (orphaned Avira AV kernel-driver descriptors), U-019 (orphaned unrar 3.50), U-008 (redistributed fonts with prohibitive licences), U-033 (PDF printer in evaluation mode, expired certificate), U-038 (schema-migration engine runs DDL at start-up), U-039 (IE COM automation).

## 2. What Type 3 resolves that Type 1 could not

- **NATIVE code (FIND-T3-002):** F-01 proves the exe is VB6 compiled to **native code** (6 evidence lines). Type 1 left p-code/native open — now resolved.
- **Names absent by design (FIND-T3-003):** F-07 proves procedure names were never stored. This explains Type 1's method-name gap.
- **Object-count version delta (FIND-T3-004 / CON-002):** B1 reports **169** objects (163 forms) for V-ROOT-1.00; `vb6-objects.json` reports **212** (198 forms) for v1.605. Most plausibly a **version difference**, flagged provisional.

## 3. Unresolved questions (FIND-T3-005)

`unified-unresolved-questions.csv` — 17 questions: 1 RESOLVED, 5 UNRESOLVED, 4 NO, 4 PARTIALLY RESOLVED, 2 NO-ANSWERED, 1 ANSWERED. Genuinely open:
- **UNR-002** meaning of the 17 `grantN` operator-permission columns
- **UNR-003** the seven `IDPassword*` values
- **UNR-004** two 32-char uppercase literals (possible obfuscated credentials)
- **UNR-005** whether `accuntpw` is hashed (evidence points to cleartext)
- **UNR-008** mapping ~1,370 embedded SELECTs to specific backends

## 4. Methodology, coverage and gaps (FIND-T3-006/007/010)

- **Tools (13):** REA (PE corroborator only), Ghidra 12.1.4 (function census; no pseudocode), mdbtools 1.0.0 (**schema only, no rows by policy**), 7-Zip, cabextract 1.9, python3 custom parsers, file(1), sha256sum, openssl, olefile, strings/grep/diff, pdfplumber/pypdf, dotnet.
- **Coverage:** B1/B2/B3 per-category coverage (16 rows); `.mdb` categories explicitly **schema-only**; some `.cat` partial.
- **Gaps:** `RL_CD2019Access/System/real lab system.exe` (single-vs-multiple builds), `calculator.exe`, `real lab timer.exe`, `real lab Image Gallery.exe`, `vkUserControlsXP.ocx`, Avira engine, `D:\Lab_data\Attaching.sql`, VS6sp6B cabinets.

## 5. Version and process records (FIND-T3-008/009)

- **Cross-version matrix (837 rows):** V-ROOT-1.00 364 / V-CD-2019 473. `RELOCATED_IDENTICAL` 41, `REL_IDENTICAL` 127, `REL_DIFFERENT` 190, `NO_PARTNER` 479 → **190 files changed** between versions, 168 byte-identical.
- **Master ledger (837 rows):** 9 component tags (V1-INSTALLED-APP 364, V2-DEPLOY-PAYLOAD 361, V2-REDIST-VB6RUNTIME 73, …). Its integrity column reads `NOT_YET_RECEIVED` — a forward plan from a different snapshot, **not** this batch's state.

## 6. Hypotheses carried forward (Class 3)

- **FIND-T3-011:** possible **third (MySQL) backend** — a MySQL ODBC connector is referenced; not present in this batch. Low confidence.
- **FIND-T3-012:** runtime schema-migration engine runs DDL at start-up; IE COM automation (Class 2, static).
- **FIND-T3-013:** coarse authorisation model (per-patient grant bits + per-section print passwords) (Class 2, Suspected).

## 7. Contradictions

| ID | Subject | A | B | Resolution |
|---|---|---|---|---|
| CON-002 | VB6 object/form count | 169/163 (V1.00) | 212/198 (v1.605) | version difference (provisional) |
| CON-003 | Embedded SELECT count | 1,337 (statements.csv) | 1,370 (UNR-008) | different extraction passes; immaterial |

## 8. Findings

| ID | Finding | Class | Confidence |
|---|---|---|---|
| FIND-T3-001 | 41 consolidated findings (9 High) | 1 | High |
| FIND-T3-002 | Native code (resolves p-code/native) | 1 | High |
| FIND-T3-003 | Procedure names absent by design | 1 | High |
| FIND-T3-004 | 169 vs 212 objects = version delta (CON-002) | 3 | Medium |
| FIND-T3-005 | 17 unresolved questions, 5 open | 1 | High |
| FIND-T3-006 | Methodology/tool limits (13 tools) | 1 | High |
| FIND-T3-007 | Coverage + schema-only MDB policy | 1 | High |
| FIND-T3-008 | Cross-version matrix (190 changed) | 1 | High |
| FIND-T3-009 | Master ledger (837 files) | 1 | High |
| FIND-T3-010 | Evidence-gap register | 1 | High |
| FIND-T3-011 | Possible MySQL 3rd backend | 3 | Low |
| FIND-T3-012 | Schema-migration engine + IE automation | 2 | Medium |
| FIND-T3-013 | Coarse authorisation model | 2 | Medium |
| FIND-T3-014 | Bundled risky components (Avira/unrar/PDF) | 1 | High |