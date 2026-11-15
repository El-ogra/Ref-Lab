# Legacy Branch Model — V-CD-2019 (v1.605)

**Purpose:** Curated reference for the multi-branch model.
**Scope:** Master/reception branches, branch seeding, two template copies, branch digit.
**Audience:** Developers, architects.
**Version applicability:** V2 ONLY (V1 is single-site; no branch column, no branch table).

---

## 1. Architectural model

- **Master laboratory + reception branches.**
- SQL Server installed **once** in the laboratory; database on the master computer only.
- Branches may connect **through the internet**.
- `Branch_Data.mdb` (49 schema-only tables, 0 rows) seeds a new branch's profile tables.
- Branch scoping via `SETTING.ini [setup] labbranchx=1`.

**Evidence:** external: 08-deployment-installer.md §5; `Docs/Complete_Reverse_Engineering_Reference_V2.md` §3.5.

## 2. Branch digit in the patient identifier

- The branch code occupies **position 9** of the 13-character `patientid`.
- Extracted via `SUBSTRING(patientid,9,1)` / `MID(patientid,9,1) as bran`.
- The branch value written there comes from the **signed-in operator's own branch**
  (`patients.branche`, Integer). The branch is a property of the workstation's operator,
  not of the patient (V2-BR-008).
- `Labbranch` holds branch master data.
- `Patient.branche` names the branch on the operator record.

**★ The most consequential data contract in the product.** A record imported without its branch
digit in position 9 is silently attributed to the wrong branch. The identifier layout is a
hard inter-version contract.

**Evidence:** `Docs/Complete_Reverse_Engineering_Reference_V2.md` §3.5, §6.4; `Docs/Business_Rules_V2.md` V2-BR-008.

## 3. Branch seeding

- `Branch_Data.mdb` contains **49 tables with 0 rows in every table** — a pure schema carrier
  used to create a branch's profile tables.
- A new branch starts with **no reference data** (no test catalogue, no ranges, no prices) until
  those are loaded separately.
- The mechanism by which the schema is applied is NOT ESTABLISHED (V2-BR-009).

**Evidence:** `Docs/Complete_Reverse_Engineering_Reference_V2.md` §6.2; `Docs/Business_Rules_V2.md` V2-BR-009.

## 4. Two template copies on the 2019 CD

| Copy | Size | SHA-256 | Notes |
|---|---:|---|---|
| `bin\Branch_Data.mdb` | 970,752 B | `f6d6d5e2…` | byte-identical to V1 |
| `System\Branch_Data.mdb` | 966,656 B | `e6c3953e…` | narrower (35 vs 46 columns in `patienttest`) |

The `bin\` copy is the **old template** (V1 byte-identical). The `System\` copy is the **newer,
narrower** template. The installer copies `System\` over the installation root, so the final
template depends on install order.

**★ CORRECTION to the earlier narrative:** the "template narrowed by 9 columns" conclusion was
drawn against the `System\` copy, not against the copy the application actually loads from `bin\`.
Which copy the running application uses is **NOT ESTABLISHED** (V2-BR-025).

The 9 columns absent from the narrower `System\` template: `highlimit`, `historyname`, `lowlimit`,
`nohighexe`, `nolowexe`, `normalrang`, `testabrev`, `testname`, `testunit`.

**Evidence:** external: 11-deployment-payload-2019.md §6.1; `Docs/Business_Rules_V2.md` V2-BR-025.

## 5. Stock-take across branches (V2-BR-010)

A stock-take may span a master laboratory and its linked branches. The version-1 stock-take asks
only whether collections are restricted to the period's patients; V2 adds a **branch dimension**.
The accounting model therefore has two aggregation dimensions — time period and branch — both
optional operator decisions.

**Evidence:** external: 09-vendor-capability-catalogue.md (5-2); `Docs/Business_Rules_V2.md` V2-BR-010.

## 6. Version applicability

- Multi-branch model, branch digit, `Branch_Data.mdb`, `Labbranch`, `Patient.branche`: **V2 ONLY**.
- V1 is single-site (no branch column, no branch table, no per-branch schema).

## 7. Known limitations

- Which `Branch_Data.mdb` copy is loaded: NOT ESTABLISHED.
- Mechanism by which branch schema is applied: NOT ESTABLISHED.
- Branch-filtering mechanism in stock-take: NOT ESTABLISHED.

## 8. Related documents

- `Docs/KB/legacy/data/IDENTIFIERS.md` — `patientid` positional contract.
- `Docs/KB/legacy/data/MODEL.md` — data model.
- `Docs/KB/legacy/data/MIGRATION.md` — schema migration.
- `Docs/KB/legacy/architecture/DEPLOYMENT.md` — deployment.
- `Docs/KB/legacy/business/WORKFLOWS.md` — workflows.
- Original source evidence: external: 08-deployment-installer.md §5,
  `Docs/Complete_Reverse_Engineering_Reference_V2.md` §3.5, §6.2, §6.4,
  external: 11-deployment-payload-2019.md §6.1, `Docs/Business_Rules_V2.md` V2-BR-008/009/025/010.
