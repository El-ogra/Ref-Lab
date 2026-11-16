# Controlled Experiments — DESIGN ONLY (NONE EXECUTED)

**This document contains proposed experiments, not results.** No target binary was executed (see `Runtime_Observations.md`). Experiment outcomes are deliberately left blank; filling them with plausible-sounding observations would be fabrication.

## 0. Mandatory preconditions before ANY experiment is run

1. Obtain the authentic v1.605 binary (SHA-256 `C54CE6C8C98A679EFC5AB485BC4B313B452026D6A40F9BDA36ECA7EA18DF93C0`) or a licensed deployment.
2. Create a disposable Windows VM with snapshot/reset; no network route to production; block `reallabeg.com` and any hard-coded hosts.
3. Register `MSVBVM60.DLL` and the exact OCX set from the vendor media; install .NET Framework 4.0 for the Bullzip stub.
4. Seed a purpose-created SQL Server (or Jet) instance with **synthetic** data only. Never use shipped credentials; never attach a production database.
5. Mock analysers (serial/TCP), SMTP sink, and print-to-file/PDF printer.
6. Hash the working copies; keep originals read-only; record every command and observation.

## Experiment E-1 — Startup and connection selection

- **Objective:** determine which configuration keys select Jet vs SQL Server, and what happens on connection failure.
- **Initial state:** clean VM snapshot; synthetic config; no DB.
- **Action:** launch the application; vary `BASE.ini` / UDL settings between runs.
- **Capture:** process/API trace (registry, file, Winsock), screen, error dialogs.
- **Static hypothesis to test:** `GetPrivateProfileStringA` reads INI; `SQLOLEDB.1`/`MSDataShape.1`/Jet providers are used; documented English date-format requirement matters.
- **Expected result:** UNKNOWN — not run.

## Experiment E-2 — Result status lifecycle

- **Objective:** verify whether `resultfinsh` → `resultreview` → `resultprint` transition in that order, allowed reversals, and exact value encoding.
- **Initial state:** synthetic patient with one ordered simple test and one composite test.
- **Action:** enter result; attempt review before entry; recheck after review; reprint; inspect `patienttest` after each action.
- **Capture:** DB row deltas, audit columns (`edituser/editdate`, `prevuser/prevdate`, `printuser/printdate`).
- **Static hypothesis:** ordered sequence; audit columns populated by the acting operator.
- **Expected result:** UNKNOWN — not run.

## Experiment E-3 — Simple vs composite test handling

- **Objective:** determine whether composite tests (e.g. CBC) produce one `patienttest` row with many result components or several rows; how ranges and units are attached.
- **Initial state:** catalogue seeded from `testtable`, `normalrange`, `template-corpus-228.json`.
- **Action:** order and result a simple test (Creatinine) and a composite panel (CBC).
- **Capture:** inserted rows in `patienttest`, result component storage, print output.
- **Static hypothesis:** unknown — this is one of the highest-value open questions.
- **Expected result:** UNKNOWN — not run.

## Experiment E-4 — Write path

- **Objective:** confirm that inserts/updates use bound recordsets rather than literal SQL.
- **Action:** capture DB traffic (ODBC/OLEDB tracing) during registration and result entry.
- **Static hypothesis:** 0 `INSERT INTO` and 0 `UPDATE … SET` among 1,797 statements ⇒ recordset-based writes.
- **Expected result:** UNKNOWN — not run.

## Experiment E-5 — Patient deletion cascade

- **Objective:** verify whether the 64-table `DELETE FROM … where patientid='…'` block executes as one transaction and whether any rows survive.
- **Action:** delete a synthetic patient; inspect all 64 tables before/after; inject a failure midway.
- **Static hypothesis:** application-level cascade, transaction semantics unknown.
- **Expected result:** UNKNOWN — not run.

## Experiment E-6 — Pricing and financial formulas

- **Objective:** recover the exact discount/payment/balance formula and rounding.
- **Action:** enter synthetic prices/discounts/partial payments; observe `patientinfo.totalprice`, `totalpricedisc`, `elmadfoo3`, `elbakyma3mal`, `payment`.
- **Static hypothesis:** columns identified; formula not recovered.
- **Expected result:** UNKNOWN — not run.

## Experiment E-7 — Printing/reporting

- **Objective:** verify DataReport → spooler/Bullzip pipeline, multi-component layout, pagination and headers.
- **Action:** print a composite result to a PDF printer; inspect output and spooler API calls.
- **Expected result:** UNKNOWN — not run.

## Experiment E-8 — Permissions

- **Objective:** verify login/authorization model derived from `patients` table and form gating.
- **Action:** create synthetic users with distinct privileges; attempt restricted operations.
- **Expected result:** UNKNOWN — not run.

## Experiment E-9 — Embedded schema migration (268 DDL statements)

- **Objective:** determine when the 27 `CREATE TABLE` + 241 `ALTER TABLE` statements execute, and against which engine (Jet vs SQL Server).
- **Action:** launch against an old-schema synthetic DB; capture DDL execution and errors.
- **Static hypothesis:** shipped DB is out of step with embedded DDL (38 applicable changes not applied).
- **Expected result:** UNKNOWN — not run.

## Experiment status ledger

| ID | Objective | Status | Blocker |
|---|---|---|---|
| E-1 … E-9 | as above | **NOT EXECUTED** | Target binary absent; no isolated disposable environment with the full VB6/OCX/DB stack; evidence-integrity and safety rules (MASTER DIRECTIVE §3.3, §9.1) |

Nothing in this document may be cited as a runtime observation.
