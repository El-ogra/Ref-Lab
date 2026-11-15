# EXECUTION DIRECTIVE — PHASE 0: GROUND-TRUTH ESTABLISHMENT

## RefLab · Reference-Driven Reverse Engineering Programme

**Execution mode:** Autonomous, evidence-driven, local filesystem operation
**Target release:** `V-CD-2019` (v1.605) and predecessor `V-ROOT-1.00`
**Execution environment:** Windows 10 / PowerShell 7 / Python 3.13
**Executing agent:** the local agent operating on the analyst workstation (REA-capable host)
**Reviewer:** Cloud review agent — the external reviewer of this phase
**Issued:** 2026-11-14
**Directive revision:** 2 — incorporates reviewer amendments of 2026-11-14 (revision 1 superseded)
**Priority:** Data integrity > Evidence accuracy > Completeness > Speed

**Naming policy:** this directive names **roles only**. No individual, agent
product, model, model version or vendor is named anywhere in it, and none may be
introduced in any deliverable, ledger field, comment or report. Refer to
participants by role — *the reviewer*, *the executing agent*, *the analyst
workstation* — and nothing else. The specification applies to whoever executes
it, independent of what implements it.

---

# 0. FIRST ACTION — PERSIST THIS DIRECTIVE

**Before any other operation, write this directive verbatim to:**

```
C:\Users\LAP LINK\source\repos\Ref Lab\evidence\reverse-engineering\phase0\PHASE-0-SPEC.md
```

**If `PHASE-0-SPEC.md` already exists at the target path:**

- **Byte-identical to the directive you were given** — do not rewrite it. Verify by
  hash, record the verification, and continue.
- **Different in any way** — **STOP and report the difference.** A different
  specification means a different authorisation, and this phase must not be executed
  against an unverified contract.
- **Unreadable or unwritable** — **STOP and report.** Do not attempt a workaround that
  writes elsewhere; the output boundary in §3.4 is absolute.

Then treat **that file on disk — not the conversation — as the authoritative
specification** for the remainder of this phase.

Re-read `PHASE-0-SPEC.md`:

- immediately after writing it, to confirm the write succeeded;
- at the start of every milestone;
- after any context interruption, compaction, or resumption.

The conversation is a launcher. The file on disk is the contract. If the
conversation and the file ever disagree, **the file wins**, because it is the
version this phase was authorised under.

`PHASE-0-SPEC.md` is a verbatim copy of this directive. It is **not** an evidence
file and is excluded from the nine-file evidence count in §7.

---

# 1. YOUR ROLE

You are the lead evidence-collection and reconciliation engineer for the RefLab
reverse-engineering programme.

Your assignment is to execute Phase 0: establish an independently verifiable,
machine-readable record of what exists, what was previously analysed, what remains
incomplete, and what must happen next.

This is an **evidence-establishment operation, NOT a reverse-engineering operation.**

You are responsible for:

1. Discovering the actual filesystem state.
2. Independently verifying previously recorded artefact identities.
3. Locating and inventorying previous analysis outputs.
4. Reconciling documented completion claims against actual files.
5. Establishing file-level analysis coverage.
6. Comparing release variants by byte identity.
7. Identifying unresolved gaps and stale status records.
8. Producing reproducible, machine-readable evidence.
9. Verifying every generated deliverable before declaring completion.

You must work from actual filesystem evidence — not from assumptions, not from
conversation history, not from previous agent assertions, not from undocumented
recollections.

**The filesystem is the primary source of truth.**

Previous documentation is a source of **claims to be tested**, not an authority that
overrides filesystem evidence.

10. **NOT nominating the next analysis target.** You report coverage state; you do
    not propose scope. Any statement about what should be analysed next must appear
    only under §13.9 *"Observations for the reviewer"*, and must not appear in any
    deliverable field, ledger row, `NextAction` rationale, or section title. The
    selection of the next analysis target belongs to the reviewer, and the reason
    this phase exists is precisely that an agent proposing scope caused a
    re-analysis of already-processed artefacts.

---

# 2. MISSION CONTEXT

A previous local-agent session was interrupted before its outputs could be confirmed
complete.

Consequently, the programme does not have a trustworthy answer to the following:

- Which reference artefacts were fully analysed?
- Which were only partially analysed?
- Which were never analysed?
- Which previous outputs actually exist?
- Which outputs are complete and internally consistent?
- Which repository status records are stale?
- Which analyses apply to both release versions?
- Which apparently identical files actually contain different bytes?

A subsequent cloud review proposed files that may already have been processed.

Phase 0 must resolve this uncertainty before any further reverse engineering begins.

**The central deliverable is a coverage ledger that enables any subsequent agent —
local or cloud — to establish the analysis status of every reference file, tied to its
exact SHA-256 identity.**

Do not perform new reverse engineering to answer these questions.

---

# 3. AUTHORITATIVE PATHS, PERMISSIONS AND PRECEDENCE

## 3.1 Paths

```
source_root    F:\REAL_LAB_COPY
repository_root C:\Users\LAP LINK\source\repos\Ref Lab
output_root    C:\Users\LAP LINK\source\repos\Ref Lab\evidence\reverse-engineering\phase0
```

The absolute location of the previous analysis tree is **unknown** and must be
discovered (see P0-1).

## 3.2 Source protection

`F:\REAL_LAB_COPY` is an immutable reference source. Treat it as strictly **READ-ONLY**.

You **may**:

- Enumerate directories and files.
- Read file metadata.
- Read file contents using read-only access.
- Compute cryptographic hashes.
- Compare file identities.
- Read text files without modifying them.

You must **never**:

- Create files or directories under `F:`.
- Rename, move, delete or modify files.
- Change attributes, permissions or timestamps.
- Run applications or installers from the source.
- Copy files in a way that changes the source.
- Open databases through a write-capable database connection.

All file-reading operations must use read-only access. Do not use commands with
ambiguous or destructive filesystem semantics.

## 3.3 Execution prohibition

The following are strictly outside the scope of Phase 0:

- Executing `real lab system.exe`.
- Executing its payload.
- Executing installers.
- Loading, attaching to, debugging or emulating the target process.
- Running REA.
- Running Ghidra.
- Running any decompiler or disassembler.
- Using VB6 analysis tools.
- Querying reference MDB databases through ACE, OLEDB, ADO, ADOX or any database engine.
- Performing any new static or dynamic reverse engineering.

Filesystem enumeration, cryptographic hashing, and plain file reading are the only
permitted evidence-collection operations.

**Rationale for the prohibition:** the coverage ledger must be independent of any tool
that may itself have produced the prior outputs. A ledger built with the same tool
chain that produced the artefacts cannot independently adjudicate them.

## 3.4 Repository write boundary

You may create or modify files **ONLY** inside:

```
C:\Users\LAP LINK\source\repos\Ref Lab\evidence\reverse-engineering\phase0\
```

Do not modify any other repository file. Do not perform Git commits, resets,
checkouts, staging operations, or any other repository-state change.

The deliverable script must be stored alongside the evidence files.

If the output directory already exists, inspect it before writing. Do not silently
overwrite previous evidence. Preserve existing files; stop if a collision cannot be
resolved safely within the authorised output directory.

**No administrator privileges are available or required.**

## 3.5 Source / prior-output precedence

The authorised source tree and the prior-analysis tree are **disjoint categories**. A
file cannot be both a reference artefact and a prior output.

If the discovered prior-output root lies **beneath** `F:\REAL_LAB_COPY`, the following
rule applies:

- Files under a prior-output root are inventoried in **P0-3 only**.
- They are **excluded** from the source inventory (P0-2).
- They receive **no rows** in the coverage matrix (P0-5).
- The excluded prefix is recorded in `source_root_exclusions` in
  `phase0-summary.json`.
- A file that could be either category is recorded under **both** candidates with
  `PriorAnalysisStatus=UNKNOWN`, and the ambiguity is reported as a gap. It is never
  silently assigned.

If the prior-output root lies **outside** `F:\REAL_LAB_COPY`, no exclusion applies and
`source_root_exclusions` is an empty list.

---

# 4. EXECUTION PRINCIPLES

These principles override any instruction that could otherwise encourage an unsafe
shortcut.

## 4.1 Evidence before conclusions

Every factual assertion must be supported by an observable source. Every ledger row
must carry sufficient evidence to justify its classification.

**Never infer completion from:**

- a filename,
- a directory name,
- a phase number,
- a documentation heading,
- a previous agent's assertion,
- a file's existence alone,
- an apparently successful command whose result was not inspected.

## 4.2 Unknown is a valid result

Use `UNKNOWN` whenever evidence is insufficient. Never invent missing hashes, file
counts, analysis relationships, completion markers, documentation references, release
membership, or historical execution status.

**An accurately documented unknown is preferable to an unsupported conclusion.**

## 4.3 Independent verification

Recompute seed hashes independently. Do not copy existing hash values into the new
ledgers without recomputation. Do not use an existing ledger as the sole evidence for
another ledger.

## 4.4 Preserve provenance

Record absolute paths, relative paths, file sizes, SHA-256 values, UTC timestamps,
evidence locations, verification methods, execution timestamps, and relevant
uncertainty. Use deterministic sorting and stable output schemas.

## 4.5 Do not overstep

If a task cannot be completed under the authorised restrictions, report the limitation.
Do not invent an alternative operation that violates the original safety boundary.

**A documented stop with evidence is a valid outcome.**

## 4.6 Do not nominate scope

No deliverable may contain a ranked list, recommendation, or shortlist of files to
analyse next. `NextAction` is a **state classification** derived from the enumerated
vocabulary in P0-5 — it is not a proposal. If you believe a particular file matters,
that observation belongs in §13.9 only.

---

# 5. SEED VALUES TO VERIFY

The values in §5.1–§5.4 are **historical claims recorded in this repository**. Their
provenance is given per block. Independently recompute and classify each as
`CONFIRMED`, `CONTRADICTED`, or `UNKNOWN`.

**Do not assume these values are correct. Do not copy them into a ledger without
recomputation.**

## 5.1 Target A — clinical database

**Seed source:** `evidence\reverse-engineering\reverse-engineering-file-accountability.csv`
(row `SOURCE`), the Phase 6–9 rows of `evidence\reverse-engineering\reverse-engineering-progress.md`,
and the `targets` array of `evidence\reverse-engineering\reverse-engineering-execution-state.json`.

Expected actual path:

```
F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\bin\Patients.mdb
```

Expected size: `2682880` bytes

Expected SHA-256:

```
98D51F5294496E03F352FF329F099572915CCB2E670C1F143D6BA368EEE898A3
```

Previously recorded characteristics:

| Characteristic | Recorded value |
|---|---|
| Format | Jet 4 / Access 2000 MDB |
| Encrypted | No |
| Corruption indicators | None |
| Catalog objects | 114 |
| User tables | 103 |
| System tables | 11 |
| User columns | 1,682 |
| Indexes | 72 |
| Declared relationships | 0 |
| Saved queries | 0 |
| Records extracted | 3,809 of 3,809 |
| Failed records | 0 |
| Partial records | 0 |

Historical tooling caveat: ADOX `position`, `size` and `required` properties were not
populated for this Jet 4 file; field metadata was reportedly obtained from an
independent `mdbtools` dump of the same file.

**Do not use database tooling to verify these characteristics during Phase 0.** Record
them as historical claims whose independent verification is outside this phase.

### Incorrect brief path

The mission brief previously stated:

```
F:\REAL_LAB_COPY\RL_CD2019Access\bin\Patients.mdb
```

This path is believed to be incorrect because it omits the intermediate `real lab
system` directory. Check the path directly and record **both** paths. If the brief
path does not exist, classify it `BRIEF_PATH_ABSENT`. Do not silently correct or
replace the historical record.

## 5.2 Target B — executable

**Seed source:** `evidence\reverse-engineering\reverse-engineering-file-accountability.csv`
(row `SOURCE`), the Phase 10 row of `evidence\reverse-engineering\reverse-engineering-progress.md`,
and the `targets` array of `evidence\reverse-engineering\reverse-engineering-execution-state.json`.

Expected actual path:

```
F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe
```

Expected size: `76292096` bytes

Expected SHA-256:

```
C54CE6C8C98A679EFC5AB485BC4B313B452026D6A40F9BDA36ECA7EA18DF93C0
```

Previously recorded characteristics:

| Characteristic | Recorded value |
|---|---|
| Format | PE32 |
| Architecture | x86 |
| Nature | Native executable, not .NET |
| Runtime | VB6 native (`MSVBVM60.DLL`) |
| Linker version | 6.0 |
| Entry RVA | `0x7142c` |
| PE timestamp | `1595435962` (2020-07-22) |
| Deployment root | `real lab system\RL_CD2019Access\` |
| Default runtime database path | `D:\real lab system\bin\Patients.mdb` |

**Do not perform PE inspection to re-derive these characteristics.** Verify file
identity and artefact existence only.

## 5.3 Recorded as NOT analysed

**Seed source:** `L-EXE-14` in `Documenting the Executable File.md` §30.

Locate and verify the following:

- `Patients_backup.mdb`
- `Branch_Data.mdb`
- `data.mdb`
- the payload installer

For each, record: absolute path, release membership, size, SHA-256, timestamp,
whether it exists, whether prior analysis outputs exist, and whether its recorded
NOT-ANALYSED status remains supportable.

**Do not assume that an absent analysis output proves that an artefact was never
analysed.** Absence of output is evidence of *absence of evidence* — classify it
`UNKNOWN` unless an output demonstrably covers the artefact.

> **Note on partial prior work.** The repository references prior analysis fragments
> at paths such as `analysis/BATCH-02/db/Branch_Data/rowcounts.csv` and
> `analysis/BATCH-02/db/Patients_backup/columns.csv`. If those exist, the
> NOT-ANALYSED status for `Branch_Data.mdb` and `Patients_backup.mdb` is **already
> suspect**. Establish the facts from the filesystem; do not assume either state.

## 5.4 Recorded as NOT performed — standing safety constraint

The historical record states that **no dynamic analysis** was performed: the target
was never executed, attached to, debugged or emulated.

Historical exclusion rationale, which is to be **preserved as a standing constraint**
for all subsequent phases:

- The application resolves its runtime store by default to a live patient database.
- `SQLOLEDB.1` connection strings could potentially reach a live SQL Server
  `Patients` database.
- The target release is **not installed** on the analyst workstation, so dynamic
  analysis of it is not possible in this environment.

Do not attempt to verify the historical non-execution claim through target execution.
Classify it as a historical assertion and state what evidence is available or
unavailable.

## 5.5 Reviewer's preliminary observations — UNVERIFIED, confirm or refute

The reviewer measured the following on **2026-11-14**, from the repository alone,
using plain file reading. **These are the reviewer's measurements, not historical
claims, and they are not authoritative.** Re-derive each independently; the filesystem
wins if you disagree. If you confirm a discrepancy, record it under P0-4 as a
`PARTIALLY-CONFIRMED` or `CONTRADICTED` claim with your own measured values.

**Observation 1 — the documentation deliverables do not self-declare their figures.**
A search for the strings `3,299`, `31 sections` and `2,459` across both
`Database documentation.md` and `Documenting the Executable File.md` returns **zero**
occurrences. The figures therefore live **only** in
`reverse-engineering-progress.md` (Phase 9 row) and must be reconciled against that
file and the execution-state JSON — not against the documents.

**Observation 2 — `Database documentation.md` against the Phase 9 claim
`204 KB / 3,299 lines / 31 sections`:**

| Measure | Claim | Reviewer measurement | Note |
|---|---:|---:|---|
| bytes | 204 KB | 204,008 | consistent (decimal KB) |
| lines | 3,299 | 3,298 | off by one — confirm the counting method and the file's final newline |
| sections | 31 | 29 `##` headings; 113 `###`; 142 combined | "31 sections" matches neither count — determine what the 31 refers to |

**Observation 3 — `Documenting the Executable File.md`:** the reviewer counted
**38 `##` headings in total, of which 32 are numbered sections and 6 are appendices**
(Appendix A … Appendix F); 172,287 bytes; 2,459 lines. The phrase *"38 sections and
6 appendices"* is **ambiguous** — the 6 appendices are inside the 38, not additional
to it. Resolve the ambiguity and state the exact section count under both readings.

**Observation 4 — a stale status is already visible.** The Phase 13 row of
`reverse-engineering-progress.md` records
`Generate Documenting the Executable File.md | PENDING`, while the deliverable
demonstrably exists and is 172,287 bytes / 2,459 lines. This is the canonical
`STALE-STATUS` case and must be reported as such — **not** as `CONFIRMED`.

---

# 6. REQUIRED WORKSTREAMS

Execute in order. Independent read-only discovery and verification operations may be
parallelised where doing so does not compromise filesystem safety, determinism or
evidence provenance. **Do not parallelise dependent operations.**

## P0-0 — Preflight and safety verification

### P0-0.1 — Capability verification (hard gate)

Before creating any file, verify that every capability this phase depends on is
actually present **in this execution environment**. Probe each one. Report the
results explicitly before performing any other work.

| # | Required capability | Probe | If absent |
|---|---|---|---|
| 1 | Python 3 with the standard library | `python --version`, else `py -3 --version` | **STOP** |
| 2 | A second, independent hash implementation | Python `hashlib` — a distinct code path from PowerShell | **STOP** |
| 3 | PowerShell `Get-FileHash` | `Get-Command Get-FileHash` | **STOP** |
| 4 | Read access to the source root | list `F:\REAL_LAB_COPY` | **STOP** |
| 5 | Write access to the output root | create and delete a probe file **inside** it | **STOP** |
| 6 | A usable PowerShell | `$PSVersionTable.PSVersion` | substitute acceptable — record it |
| 7 | Sufficient free disk space for the ledgers | check the output volume | **STOP** if insufficient |

**Rules for this gate:**

1. **If a required capability is absent, STOP and report.** State: the missing
   capability, the exact probe that failed and its verbatim output, the workstreams
   affected, the acceptance criteria that cannot be satisfied, and the minimum safe
   next action.
2. **Do not substitute** a different implementation, hashing method, or weaker
   verification and then continue. A silent substitution is a defect.
3. If a substitution is genuinely unavoidable and you take it, it **must** be recorded
   in `phase0-summary.json` under `capability_substitutions`, **and** the affected
   acceptance criterion **must** be reported as `BLOCKED` or `NOT-VERIFIABLE` —
   **never** as `PASS`.
4. A capability reported as present but later found absent is a Phase 0 failure.
   Record it as a gap with severity `HIGH`.
5. Record the **verbatim probe output** for every capability — this is evidence.
6. The directive's tool versions (PowerShell 7.6.10, Python 3.13) describe the
   reference workstation. **A different version is not a failure** — only an
   *absent capability* is. Record the version actually present.

### P0-0.2 — Workspace and filesystem safety

Before creating any files:

1. Confirm the source root exists.
2. Confirm the repository root exists.
3. Confirm the authorised output parent exists.
4. Confirm the current process can read the source.
5. Confirm the authorised output location is writable.
6. Record the current working directory.
7. Record the hostname and operating system.
8. Record the PowerShell and Python versions.
9. Record the UTC start timestamp.
10. Inspect whether the Phase 0 output directory already exists.
11. Identify potential output-name collisions.
12. Confirm the permitted operation plan requires no reverse-engineering tool and no
    administrative privilege.

**Write `PHASE-0-SPEC.md` (§0) as the very first action**, then continue with the
checks above.

Do not create other outputs until all preflight checks have completed. If any
mandatory safety prerequisite fails, stop and report.

## P0-1 — Resolve all working roots

### 1.1 Identify the two release roots

- The **V-CD-2019 (v1.605) payload root** is expected at
  `F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\`.
- The **sibling tree holding the earlier build (`V-ROOT-1.00`) is not recorded
  anywhere in the repository.** The historical record refers to it only as "the
  sibling tree" and it was "excluded from substantive analysis".

Enumerate the **immediate children** of `F:\REAL_LAB_COPY` and classify each as:

`RELEASE-PAYLOAD` · `SUPPORT-FILE` · `INSTALL-MEDIA` · `DOCUMENTATION` · `UNKNOWN`

Record the **evidence** for each classification (directory name, contained files,
presence of the deployment root, size profile, presence of known payload artefacts).

**If the sibling release root cannot be established with evidence:** `ReleaseMembership`
for ambiguous files MUST be `UNKNOWN`, and P0-6 MUST be reported as `BLOCKED` for that
subtree. **It must never be guessed.**

### 1.2 Locate the prior analysis tree

Search in this priority order:

1. `F:\REAL_LAB_COPY\` and subdirectories, looking for a directory named `analysis`.
2. `C:\Users\LAP LINK\Desktop`
3. `C:\Users\LAP LINK\OneDrive\Desktop`
4. The repository and its parent directories.

Search for these known relative fragments:

```
analysis/BATCH-01/architecture.md
analysis/BATCH-01/capability-inventory.md
analysis/BATCH-01/data-model.md
analysis/BATCH-01/sql/statements.csv
analysis/BATCH-02/db/Branch_Data/rowcounts.csv
analysis/BATCH-02/db/Patients_backup/columns.csv
analysis/BATCH-04/vb6
analysis/BATCH-04/sql_v1605
analysis/BATCH-04/pe-reports
analysis/BATCH-04/schema
analysis/BATCH-07/db
```

Also search for:

```
Reference_V2_Patches_04-11.zip
RefLab_Reference_V2_Consolidated.zip
```

For every candidate tree record: absolute path, discovery location, file count, total
bytes, directory depth, presence of known relative fragments, and confidence that it
is the actual previous analysis tree.

**Do not extract ZIP archives during Phase 0.**

If multiple candidate trees exist, report all of them and identify which has the
strongest evidence of being authoritative. If none is found, record `NOT_FOUND`.

**Do not silently substitute an alternative tree.**

## P0-2 — Complete authorised-source inventory

Recursively enumerate every file beneath `F:\REAL_LAB_COPY` — **excluding any subtree
designated a prior-output root under §3.5**.

Include hidden files, system files accessible to the process, zero-byte files, files
with no extension, binary files, large files, and files nested at arbitrary depth.

For every file record:

```
Path, RelativePath, Bytes, SHA256, LastWriteTimeUtc, Extension, ReleaseMembership, PriorAnalysisStatus, Notes
```

Requirements:

1. Compute SHA-256 for **every** file. Do not skip on size or extension.
2. Record inaccessible files and hashing errors explicitly.
3. **Never replace a failed hash with an empty string presented as a valid hash.**
4. Preserve the exact relative path.
5. Sort paths deterministically using ordinal comparison.
6. Use uppercase hexadecimal SHA-256.
7. Record byte counts as integers.

`ReleaseMembership` ∈ `V-ROOT-1.00` · `V-CD-2019` · `SHARED` · `UNKNOWN`
— derived from P0-1.1. **Do not infer shared membership from matching filenames
alone.**

`PriorAnalysisStatus` ∈ `ANALYSED-DOCUMENTED` · `PARTIAL` · `NOT-ANALYSED` · `UNKNOWN`
— assigned by the evidence rules of P0-3, P0-4 and P0-5.

Report: total file count, total bytes, hash failures, permission failures,
enumeration failures, files by extension, files by release membership.

A complete inventory requires **zero** unresolved enumeration failures and **zero**
hashing failures.

## P0-3 — Prior-output inventory

Inventory every file in:

1. the discovered prior analysis tree;
2. every analysis-like directory found beneath the authorised source;
3. other verified previous-output locations directly relevant to RefLab.

**Do not indiscriminately inventory unrelated user files.**

Record:

```
Path, RelativePath, Bytes, SHA256, LastWriteTimeUtc, SubjectArtefact, AnalysisType, CompletionStatus, Notes
```

`SubjectArtefact` — the reference artefact the output is about (`real lab system.exe`,
`Patients.mdb`, `data.mdb`, `Branch_Data.mdb`, `Patients_backup.mdb`, `BASE.ini`,
`RLS_Setup.txt`, report templates, installer, …). Use `UNKNOWN` when the association
cannot be established confidently.

`AnalysisType` ∈ `identity` · `structure` · `strings` · `sql` · `pe` ·
`vb6-object-table` · `code-entries` · `records` · `documentation` · `scripts` · `other`

`CompletionStatus` ∈ `COMPLETE` · `PARTIAL` · `EMPTY` · `TRUNCATED` · `UNKNOWN`

Classification rules:

- Zero-byte outputs are `EMPTY` or `PARTIAL`, never `COMPLETE`.
- Outputs ending mid-record are `TRUNCATED`.
- Outputs lacking expected structural completeness are `PARTIAL`.
- An absent completion marker is a **warning, not proof** of incompleteness.
- A completion marker is **not sufficient proof** of completion by itself.
- Completeness must be assessed against the output's expected format and the
  available evidence.
- **Never label an output complete merely because its filename implies completion.**

## P0-4 — Claimed-versus-present reconciliation

Identify every repository claim of completed or pending analysis. At minimum examine:

1. `evidence\reverse-engineering\reverse-engineering-progress.md` — the phase table
2. `evidence\reverse-engineering\reverse-engineering-execution-state.json` — the targets array
3. every row of `evidence\reverse-engineering\reverse-engineering-file-accountability.csv`
4. `Database documentation.md` — existence, size, line count, section count
5. `Documenting the Executable File.md` — existence, size, line count, section count,
   appendices

**These two documents do not self-declare their figures** (§5.5, Observation 1). Their
figures are claims made by the progress log and the execution-state JSON, and must be
reconciled against **those** sources.

**Do not use these documents to construct the coverage matrix.** They are
reconciliation targets only.

For every identified claim record:

```
ClaimSource, ClaimLocation, ClaimText, ClaimedStatus, EvidencePath, EvidenceBytes, EvidenceSHA256, EvidenceMatchesClaim, Verdict, Reason
```

`Verdict` ∈ `CONFIRMED` · `CONTRADICTED` · `PARTIALLY-CONFIRMED` · `UNVERIFIABLE` ·
`STALE-STATUS`

### The critical STALE-STATUS rule

A claim may be **correct in substance but carry an outdated status.** Example: a
phase recorded as `PENDING` whose complete deliverable exists on disk and passes the
relevant integrity checks.

This MUST be classified `STALE-STATUS`. It MUST NOT be classified `CONFIRMED`.

For every stale status, identify the **exact correction recommended**. Do not edit
the original claim sources. Preserve all historical records.

## P0-5 — File-level coverage matrix

**This is the primary deliverable of Phase 0.**

Create exactly one coverage row for every file in the **source inventory** (P0-2,
after §3.5 exclusions). Prior outputs are **not** reference artefacts and receive no
rows here.

Schema:

```
RelativePath, ReleaseMembership, Bytes, SHA256, PriorAnalysisStatus, EvidenceOutputs, DocumentationReference, OpenQuestions, NextAction, Confidence
```

- `EvidenceOutputs` — the actual paths of outputs that **demonstrably cover** the file.
  A document mentioning an artefact does not automatically constitute analysis coverage.
  Every path here must exist as a row in `prior-output-ledger.csv`.
- `DocumentationReference` — the specific section or document that describes the
  artefact, if verified. The referenced document must exist.
- `OpenQuestions` — relevant existing unresolved identifiers (`L-EXE-*`, `UQ-EXE-*`).
  **Do not invent existing identifiers.**

`NextAction` ∈ `NONE` · `VERIFY-HASH` · `ANALYSE` · `FINISH-PARTIAL` · `RE-ANALYSE` ·
`DOCUMENT` · `OUT-OF-SCOPE`

`Confidence` ∈ `HIGH` · `MEDIUM` · `LOW` — use `LOW` whenever prior analysis
completion cannot be established.

### Mandatory consistency rules

1. `ANALYSED-DOCUMENTED` must not carry `NextAction=ANALYSE` unless a specific
   integrity contradiction justifies `RE-ANALYSE`.
2. `PARTIAL` must normally map to `FINISH-PARTIAL`.
3. `NOT-ANALYSED` must normally map to `ANALYSE`, unless explicitly out of scope.
4. `UNKNOWN` must not be silently promoted to a completed state.
5. A hash mismatch between a reference artefact and the artefact used by a previous
   analysis **invalidates shared coverage** until reconciled.
6. `EvidenceOutputs` must reference actual `prior-output-ledger.csv` rows.
7. A documentation reference must point to an existing document.
8. Every source file has exactly one row.
9. No unsupported coverage relationship may be created.

Include **automated consistency checks** in the collection script (§9, items 10–17).

## P0-6 — Release membership and byte identity

Compare every relative path that exists in **both** release roots.

Record:

```
RelativePath, PathRelease1, HashRelease1, PathRelease2, HashRelease2, ByteIdentical, SharedAnalysisApplicable, Notes
```

`ByteIdentical` ∈ `YES` · `NO` · `UNKNOWN`

`SharedAnalysisApplicable`:

- `YES` **only** when byte identity is established;
- `NO` when the copies differ;
- `UNKNOWN` when the comparison cannot be completed.

For every differing pair:

1. Record both sizes.
2. Record both hashes.
3. Compute the size delta.
4. Record the exact paths.
5. Establish the nature of the difference **only** where plain byte-level inspection
   permits it.
6. **Do not assume** differences are caused by timestamps or line endings.
7. **Do not claim** semantic equivalence from superficial similarity.

Files with matching names but different content are **HIGH-PRIORITY findings**. List
every such pair individually in the final report.

**Analysis of one release variant must not automatically cover another.**

If the sibling release root was not established with evidence (P0-1.1), report P0-6
for that subtree as `BLOCKED` — not as an empty result.

## P0-7 — Independent target verification

Independently verify Target A and Target B using **two separate implementations**.

- Implementation 1: PowerShell `Get-FileHash -Algorithm SHA256`
- Implementation 2: Python 3 standard-library `hashlib.sha256`, chunked reads

**Do not treat two calls to the same implementation as independent verification.** Do
not load either file into a reverse-engineering or database tool.

Record:

```
Artefact, Path, SizeDeclared, SizeObserved, SHA256_Impl1, SHA256_Impl2, HashesAgree, Verdict, Timestamp, SeedSource, Notes
```

`SeedSource` — the exact file and line of the repository record that carries the
expected value, so that a contradiction can be adjudicated against a source rather
than against an assertion.

Additionally record: start and finish timestamps, wall-clock duration, read method,
whether the complete file was read, whether the file size changed during verification
if detectable, and any I/O errors.

**If the two implementations disagree: STOP immediately.** Do not choose one hash. Do
not continue with dependent classification work.

**If either seed hash contradicts the observed hash:** record the contradiction and
activate the stop procedure. Do not classify the target as merely `CONTRADICTED` and
continue.

## P0-8 — Gap register

Create a comprehensive register of every uncovered artefact, unresolved issue,
incomplete output, stale status and evidence deficiency discovered during Phase 0.

Schema:

```
GapId, Subject, Description, Type, Severity, BlocksNextPhase, EvidencePath, RecommendedNextAction, RequiresTooling, Notes
```

`Type` ∈ `NOT-ANALYSED` · `PARTIAL` · `INCOMPLETE-EVIDENCE` · `STALE-STATUS` ·
`UNVERIFIABLE` · `KNOWN-UNANSWERABLE`

`Severity` ∈ `HIGH` · `MEDIUM` · `LOW`

`BlocksNextPhase` ∈ `YES` · `NO`

> **`BlocksNextPhase=YES` means: no analysis work may begin on any reference
> artefact until this gap is resolved.** The scope of the next phase is set by the
> reviewer, not by this phase.

`RequiresTooling` ∈ `NONE` · `REA` · `GHIDRA` · `DB-ENGINE` · `DYNAMIC` ·
`HUMAN-OWNER`

- Use existing identifiers where already established.
- For newly discovered Phase 0 findings use `L-P0-*` for limitations and `UQ-P0-*`
  for unresolved questions.
- **Never renumber, overwrite or reuse an existing identifier.**
- Include every historical seed limitation and unresolved question identifiable from
  the available evidence, then add newly discovered gaps.
- **Do not create duplicate records for the same underlying issue.**
- Prioritise gaps that could cause incorrect analysis of a release or artefact.

## P0-9 — Machine-readable summary

Create `phase0-summary.json`.

Requirements: UTF-8 without BOM · valid JSON · deterministic key ordering · stable
schema · explicit `null` or `UNKNOWN` where appropriate · **no unsupported numeric
estimates**.

Include at minimum:

```
phase
issued
executed_at
host
source_root
analysis_root
repository_root
output_root
release_roots              # {V-ROOT-1.00: <path>|null, V-CD-2019: <path>}
source_root_exclusions     # list of prior-output prefixes excluded under §3.5
capability_probes         # verbatim probe output per capability (P0-0.1)
capability_substitutions   # any substitution made, with the criterion it invalidates
source_file_count
source_total_bytes
hash_failures
enumeration_failures
prior_output_count
coverage_counts            # by PriorAnalysisStatus
next_action_counts         # by NextAction
release_membership_counts  # by ReleaseMembership
stale_status_claims
byte_identical_shared_files
shared_but_differing_files
target_verification
gaps_high
gaps_medium
gaps_low
blocking_gaps
phase_complete             # Boolean
```

`phase_complete` may be `true` **only** if every mandatory acceptance criterion in
§10 passes. **Never set it `true` because the script ran successfully or because the
output files exist.**

## P0-10 — Factual human-readable report

Create `PHASE-0-REPORT.md`, maximum approximately 400 lines.

Required sections:

1. Purpose and scope
2. Resolved filesystem roots (including both release roots and their classification evidence)
3. Source inventory summary
4. Prior-output inventory summary
5. Claimed-versus-present reconciliation
6. Every `STALE-STATUS` finding
7. Coverage distribution
8. Release membership findings
9. Shared-but-differing files
10. Target verification results
11. Gap register summary
12. Blocking gaps
13. What the next phase may start with
14. What the next phase must not start with
15. Exact activities not performed
16. Acceptance criteria results
17. Overall phase completion verdict

Use concise, factual English. **Do not write Arabic deliverable documents.** Do not add
narrative padding or unsupported conclusions.

**§13 "What the next phase may start with" must state scope constraints, not
recommend files.** Name no candidate file as a next analysis target.

---

# 7. DELIVERABLES

Create exactly the following **nine evidence files and one reproducibility script**
inside the authorised Phase 0 directory, plus the persisted directive copy from §0.

| File | Workstream |
|---|---|
| `PHASE-0-SPEC.md` | §0 — verbatim copy of this directive (not counted below) |
| `file-ledger.csv` | P0-2 |
| `prior-output-ledger.csv` | P0-3 |
| `claimed-vs-present.csv` | P0-4 |
| `coverage-matrix.csv` | P0-5 |
| `release-membership.csv` | P0-6 |
| `target-verification.csv` | P0-7 |
| `gap-register.csv` | P0-8 |
| `phase0-summary.json` | P0-9 |
| `PHASE-0-REPORT.md` | P0-10 |
| `collect_phase0.py` | reproducible collection and validation script |

**No additional deliverable files are authorised.** Temporary files may be created only
inside the Phase 0 directory and must be removed before final verification. Do not
create logs, caches, backups, checkpoints or auxiliary documentation elsewhere.

The script must use Python standard-library modules only and must be executable with
the available Python 3.13 installation. Record the exact command line used in the
final report.

**Do not perform a Git commit.**

---

# 8. DATA AND FORMATTING STANDARDS

## 8.1 CSV format

- UTF-8 **without BOM**
- **LF** line endings
- RFC 4180-compatible quoting
- **every header cell quoted**
- **every data cell quoted**, for consistency
- decimal integers for byte counts
- uppercase hexadecimal SHA-256
- empty fields only where genuinely appropriate
- no trailing spaces
- no blank trailing rows
- exactly one final newline
- deterministic ordinal path sorting

Use the existing house ledgers as the naming and style reference:

```
evidence\reverse-engineering\reverse-engineering-file-accountability.csv
  "Kind","Path","Bytes","SHA256","Status","Description"

evidence\reverse-engineering\metadata\export_ledger.csv
  "Table","Expected","Exported","Failed","Columns","ExportFile","ExportSHA256","ExportBytes","Status","Error"

evidence\reverse-engineering\metadata\rowcounts.csv
  "Table","Rows","Columns","Indexes","Error"
```

**Do not copy those example columns into unrelated ledgers.** Use the exact schemas
defined in §6.

## 8.2 JSON format

UTF-8 without BOM · valid JSON · sorted keys · consistent indentation · no comments ·
no trailing commas · explicit booleans · consistent null handling.

## 8.3 Determinism

Identical source trees and identical execution conditions must produce equivalent
evidence files, excluding explicitly time-dependent metadata.

**Do not use locale-dependent sorting.** Do not use unstable ordering from filesystem
enumeration.

---

# 9. REPRODUCIBLE SCRIPT REQUIREMENTS

`collect_phase0.py` is part of the evidence. It must:

1. Use Python standard-library modules only.
2. Validate all configured paths.
3. Enumerate source files recursively, honouring the §3.5 exclusions.
4. Compute SHA-256 using chunked reads.
5. Record file metadata.
6. Inventory prior analysis outputs.
7. Support deterministic ordering.
8. Generate the required CSV and JSON files.
9. Validate all generated schemas.
10. Validate row counts and coverage consistency.
11. Validate SHA-256 formatting.
12. Detect duplicate relative paths.
13. Validate release comparison coverage.
14. Validate that all `EvidenceOutputs` references exist.
15. Validate that all documentation references exist.
16. Validate coverage / `NextAction` consistency.
17. Validate the summary counts against the actual ledger rows.
18. Detect missing or unexpectedly empty outputs.
19. Return a non-zero exit code if a mandatory validation fails.
20. **Never write to the authorised source.**

### Implementation rule

Separate **collection**, **classification** and **validation** into logically distinct
stages. A failed collection step must never silently generate a successful-looking
report. Do not suppress exceptions in a way that converts failures into apparently
valid empty results. Record every exception with its associated path.

### Independent hash verification

The primary collection script may use Python `hashlib`. Target verification must
**additionally** use PowerShell `Get-FileHash`. Two calls to the same implementation
are not independent verification.

### Script safety — review before executing

- Review every write operation.
- Verify that all output paths resolve **inside** the authorised output directory.
- Verify that no source path can be used as a write destination.
- Verify that the script executes no target binaries.
- Verify that it does not import or invoke reverse-engineering tools.

---

# 10. ACCEPTANCE CRITERIA

The phase is complete only when every criterion below has an explicit result.

| ID | Criterion |
|---|---|
| A1 | Every file beneath the authorised source root — excluding any subtree designated a prior-output root under §3.5 — is enumerated. |
| A2 | SHA-256 is computed for every source file; zero hash failures. |
| A3 | Every discoverable relevant prior analysis output is inventoried and classified. |
| A4 | Every phase claim in the repository status table is reconciled against filesystem evidence. |
| A5 | Every `STALE-STATUS` finding is individually documented. |
| A6 | The coverage matrix contains exactly one row per source file, and no prior output appears in it. |
| A7 | Every shared-by-path file pair has an explicit byte-identity verdict; every non-identical pair is listed individually. |
| A8 | Both target artefacts pass independent two-implementation hash verification. |
| A9 | The gap register includes historical limitations and newly identified gaps, with no identifier reuse. |
| A10 | The eight CSV/JSON files parse successfully and conform to their schemas; `PHASE-0-REPORT.md` contains the seventeen required sections. |
| A11 | `phase0-summary.json` contains a valid Boolean `phase_complete`. |
| A12 | `collect_phase0.py` exists and runs end-to-end on this host. |
| A13 | The authorised source remains unchanged — supported by before-and-after target hashes, the read-only execution design, inspection of the collection script, the recorded operations, and the absence of any source-writing operation. |
| A14 | No reverse-engineering tool was invoked. |
| A15 | No deliverable contains a nomination, ranking or shortlist of files to analyse next. |
| A16 | The P0-0.1 capability gate was executed, every probe result is recorded verbatim, and no required capability was silently substituted. |

For every criterion, report `PASS` · `FAIL` · `BLOCKED` · `NOT-VERIFIABLE`.

**Do not convert `BLOCKED` or `NOT-VERIFIABLE` into `PASS`.**

### Integrity clarification on A13

Before-and-after hashes of Target A and Target B demonstrate that **those two files**
remained byte-identical across the verification interval. They do **not**, by
themselves, prove that every other source file was untouched. The source-protection
claim must therefore also be supported by the read-only execution design, inspection
of the collection script, the recorded operations, and the absence of any
source-writing operation.

**Do not claim stronger verification than the evidence supports.**

### Completion rule

`phase_complete = true` only when all mandatory criteria pass. If a criterion cannot be
satisfied without violating the safety restrictions, **preserve the evidence and report
the phase as incomplete**.

---

# 11. STOP CONDITIONS

Immediately stop dependent execution if any of the following occurs:

1. `F:\REAL_LAB_COPY` is absent.
2. The recorded deployment root cannot be reconciled with the actual filesystem.
3. A target hash contradicts its expected seed.
4. Two independent hash implementations disagree.
5. The repository or output location is not writable.
6. An operation would require administrator privileges.
7. An operation would require writing to `F:`.
8. A step would require executing or analysing the target.
9. The source appears to be changing during collection.
10. A critical path or permission error makes the evidence unreliable.

When a stop condition occurs:

- Record the attempted operation.
- Record the observed result.
- Preserve already collected evidence.
- Identify the dependent workstreams that were not executed.
- State the exact reason for stopping.
- Recommend the minimum safe next action.

**Do not improvise around a stop condition. A stop with evidence is a valid result.**

---

# 12. LONG-RUN EXECUTION AND RECOVERY

This is a multi-stage task that may exceed a single context window.

**Maintain execution state through the generated evidence and the authorised Phase 0
directory — not through conversational memory.**

## 12.1 Progress discipline

At each major milestone, record:

- completed workstreams
- pending workstreams
- failed checks
- unresolved decisions
- files successfully generated
- last verified operation
- next safe operation

**Do not claim a workstream is complete until its outputs have passed validation.**

## 12.2 Context interruption

If execution is interrupted:

1. Re-read `PHASE-0-SPEC.md` from disk.
2. Re-inspect the Phase 0 output directory.
3. Read the existing `collect_phase0.py`.
4. Inspect the existing outputs.
5. Recompute the relevant integrity checks.
6. Determine which operations actually completed.
7. Resume from the first **unverified** operation.

**Never assume that an operation completed because a previous agent said it was about
to run.** Never restart a destructive or potentially modifying operation. Never
overwrite existing evidence without first determining whether it is valid and whether
replacement is authorised.

## 12.3 Parallel execution

Use parallel operations for independent read-only tasks where practical — for example
hashing independent target files, reading independent repository status documents,
inspecting separate prior-output directories.

Use sequential execution where operations depend on earlier results. **Do not launch
uncontrolled parallel jobs** that could overload the filesystem or compromise evidence
consistency.

---

# 13. FINAL REPORTING TO THE REVIEWER

## 13.1 Executive result

- Whether Phase 0 completed.
- Whether `phase_complete` is true or false.
- Total source files · total source bytes · hash failures · prior output count.
- High-severity blocking gaps.

## 13.2 Summary JSON

Present the **actual contents** of `phase0-summary.json` inline. Do not substitute a
manually reconstructed summary.

## 13.3 Stale-status findings

| Claim | Recorded status | Filesystem evidence | Recommended correction |
|---|---|---|---|

Every finding, individually.

## 13.4 Release differences

Every shared-by-path file that is not byte-identical — both hashes, both sizes, size
delta, and the implications for analysis reuse.

## 13.5 Gap register

Grouped by HIGH / MEDIUM / LOW, blocking gaps first.

## 13.6 Coverage distribution

Counts by `PriorAnalysisStatus`, `NextAction` and `ReleaseMembership`, reconciling
with the source inventory totals.

## 13.7 Verification status

Target A hash verification · Target B hash verification · independent implementation
agreement · source integrity evidence · script validation · output validation ·
acceptance-criteria results.

## 13.8 Explicit non-activities

- No execution of the target.
- No dynamic analysis.
- No static reverse engineering.
- No database queries.
- No modification of the authorised source.
- No changes to repository files outside the Phase 0 directory.
- No changes to historical status records.
- No Git commit.

## 13.9 Reviewer handoff — state only

- What is now established as ground truth.
- What remains uncertain.
- What must be resolved before Phase 1.
- The coverage state of every file.
- Which release variants must be treated separately.
- Which existing claims require correction.

**Do not nominate, rank or shortlist files for subsequent analysis.** Scope selection
belongs to the reviewer. Observations that appear to bear on scope may be stated here as
observations, clearly labelled as such, and nowhere else.

**Do not write the final Arabic human-readable programme documentation.** That is the
reviewer's responsibility.

**Naming policy applies to your report as well.** Name no individual, agent
product, model, model version or vendor. Refer to participants by role only.

---

# 14. FINAL EXECUTION INSTRUCTION

Proceed autonomously within the authorised boundaries.

**Do not stop to ask for permission for routine, reversible, explicitly authorised
read-only operations, or for the creation of the specified outputs.** Do not perform
any operation outside the authorised scope.

Prioritise correctness and evidence integrity over speed.

**Do not merely describe how the task could be completed. Execute the permitted work,
create the deliverables, validate them, and report the actual results.**

> **Never report an action as completed unless a tool result or a verified filesystem
> artefact proves that it was completed.**
>
> **Never report a file as analysed unless its exact SHA-256 identity is tied to
> verifiable analysis evidence.**
>
> **Never report Phase 0 as complete unless every mandatory acceptance criterion has
> passed.**
>
> **Never nominate the next analysis target.**

Begin with §0 — write `PHASE-0-SPEC.md` — then P0-0: Preflight and safety
verification.

---

*End of directive, revision 2.*
