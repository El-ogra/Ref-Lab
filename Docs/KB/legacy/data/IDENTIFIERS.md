# Legacy Identifiers — Data Contracts

**Purpose:** Document the positional identifier contracts of the legacy systems.
**Scope:** `patientid` layout, branch derivation, person key, display code, barcode.
**Audience:** Developers (critical — data contract).
**Version applicability:** VERSION-DEPENDENT (branch segment is V2 addition).

---

## 1. The `patientid` contract

`patientid` is **Text(13) NOT NULL** in `patientinfo`, and is **not** a patient key.
It is a composite **order/visit identifier**. There is no patient master table.

| Position | Segment | Derivation | Evidence |
|---|---|---|---|
| 1–2 | (unknown) | — | not established |
| 3–4 | **Person key** | `MID(patientid,3,2)` — used both as the patient filter and as `COUNT(MID(patientid,3,2))` in turnover reports | ET-CODE |
| 5–8 | (unknown) | — | not established |
| 9 | **Branch code (V2)** | `MID(patientid,9,1) as bran` — V2 only; V1 has positional semantics but no documented branch segment | ET-CODE |
| 10–12 | **Display code** | `mid(patientid,10,3) as patid` — short display code | ET-CODE |
| 13 | (unknown) | — | not established |

**★ The most consequential data contract in the product.** A record imported without its branch
digit in position 9 is silently attributed to the wrong branch. Any data migration that reformats
the identifier silently breaks three separate functions at once: patient identity, branch
attribution, and the turnover counts.

**Version applicability:** SHARED for the mechanism (positional derivation); V2 adds the branch
segment at position 9 (V1-BR-005, V2-BR-008, V2-BR-040).

**Evidence:** `MID(patientid,3,2)`, `MID(patientid,9,1) as bran`, `mid(patientid,10,3) as patid`
in the extracted SQL (`analysis/BATCH-01/sql/statements.csv`); CHM `006.htm`.

## 2. Branch derivation

- The branch value written into position 9 comes from the **signed-in operator's own branch**
  (`patients.branche`, Integer). The branch is a property of the workstation's operator, not
  of the patient (V2-BR-008).
- `Labbranch` holds branch master data.
- `SUBSTRING(patientid,9,1)` extracts the branch in SQL.
- `Patient.branche` names the branch on the operator record.

**Evidence:** `Docs/Complete_Reverse_Engineering_Reference_V2.md` §3.5, §6.4.

## 3. Barcode

The patient barcode is the direct patient identifier carried on a physical artefact (V1-BR-006,
V2-BR-040). A barcode identifying the patient is printed on a label accompanying the specimen.
The `barcodes` table holds allocated barcodes (35 rows) and the patient record carries the barcode.
The barcode is a direct identifier on an uncontrolled physical artefact — a label removed from the
laboratory discloses the patient's identity.

**Evidence:** CHM `003A.htm`; `barcodes` table; `IDCARD.INI` barcode field layout; form `barcode`
(71 controls). Symbology not established.

## 4. Identifier allocation

`IdentifierAllocator` (modern) allocates from leading segment, person key, middle segment, display
code, and trailing character — **no branch parameter** (branch removed in the modern system).
The modern system uses GUIDs for `Patient`/`PatientOrder`/`OrderTest` IDs (see
`src/RefLab.Infrastructure/Persistence/Configurations/PatientConfigurations.cs`), eliminating the
positional contract entirely.

## 5. Related contracts

- `patientinfo.patientid` — Text(13) NOT NULL, the order identifier.
- `patienttest` references `patientid` positionally.
- `history` — one row per patient, 20 free-form slots.
- `mid(patientid,10,3)` — short display code used in UI and reports.

## 6. Version applicability

| Segment | V1 | V2 |
|---|---|---|
| Person key (positions 3–4) | present | present |
| Branch code (position 9) | positional semantics, no documented branch segment | branch code from operator's `branche` |
| Display code (positions 10–12) | present | present |
| Remaining positions | not established | not established |

## 7. Known limitations

- Positions 1–2, 5–8, and 13 semantics are not established (RL-V1-Q-11).
- The modern system does not reproduce this contract (uses GUIDs) — see `LEGACY-TO-MODERN.md`.

**Evidence:** external: Business_Rules_V1.md V1-BR-005; `Docs/Business_Rules_V2.md` V2-BR-008, V2-BR-040;
external: Complete_Reverse_Engineering_Reference_V1.md §6.3; `Docs/Complete_Reverse_Engineering_Reference_V2.md` §6.4.
