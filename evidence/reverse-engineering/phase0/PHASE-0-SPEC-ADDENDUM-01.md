# PHASE-0 SPECIFICATION ADDENDUM 01

**Binding amendment to:** `PHASE-0-SPEC.md` (revision 3, SHA-256
`168906121c05d8b153591b9a273cdebd7286524642f2a7f636bc4ad96bb16a1f`)

**Issued by:** the reviewer, at the owner's request — 2026-11-15

**Status:** this addendum is **part of the specification** and is to be read
together with it. Where this addendum conflicts with `PHASE-0-SPEC.md`, **this
addendum prevails.** Every clause of the specification not named in §A-3 below
remains in force unchanged.

**Scope of amendment:** P0-1.1 · P0-3 · P0-5 · P0-6 · P0-7 (re-verification
only) · P0-8 · P0-9 · P0-10 · §7 (deliverable list) · §10 (A1, A3, A6, A7).

---

## A-0 — Context and why this addendum exists

### A-0.1 Observed facts on the analyst workstation

These were observed during the completed Phase 0 run and are **verified
filesystem observations**, not assumptions:

- `F:\REAL_LAB_COPY` has exactly **two** immediate children: `DEMO` and
  `real lab system`.
- The confirmed Target B executable is at
  `F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe`.
- A **second executable** is present at the root of
  `F:\REAL_LAB_COPY\real lab system\`.
- The `DEMO` tree is not named anywhere in the repository's documentation.
- Multiple instances of `Patients.mdb`, `Branch_Data.mdb`, `data.mdb` and
  `Patients_backup.mdb` exist under **different** subdirectories of
  `real lab system`.

### A-0.2 The defect in the specification

The two releases are therefore **nested and interleaved**, not siblings. The
specification's P0-1.1 assumed a sibling release root and P0-6 assumed a
relative-path comparison across two roots. **Neither assumption holds against
this filesystem, and neither can be satisfied as written.**

### A-0.3 The phase behaved correctly

P0-6 required an evidence-based release root and forbade guessing. The agent
reported `A7 BLOCKED` and `phase_complete = false` rather than fabricating a
sibling tree. **That is the specification working as designed, not a failure.**
This addendum repairs the specification's model; it does not criticise the
execution.

---

## A-1 — Replacement for P0-1.1: artefact-level release attribution

> **Supersedes P0-1.1 in full.**

**Governing principle.** Directory layout is **not** evidence of release
membership. The releases are interleaved in one tree, so membership is
determined by **artefact identity and byte evidence**, never by path depth or
folder name.

### A-1.1 Basename index (mandatory)

Build an index over every file in the source inventory:

```
ArtefactName, RelativePath, Bytes, SHA256
```

- `ArtefactName` is the file's **basename**, compared **case-insensitively**
  (Windows path semantics). Preserve the original casing in the output column
  and record the normalised form in a separate column `ArtefactKey`.
- Emit this as a new deliverable: **`artefact-basename-index.csv`**.

### A-1.2 Multi-instance classification

For every `ArtefactKey` with **more than one instance**:

- group the instances by SHA-256;
- each distinct SHA-256 is one distinct **version** of that logical artefact;
- record the instance count, the version count, and every path.

### A-1.3 Version-pair rows — the cross-release difference matrix

For each multi-instance artefact, emit **one row per pair of distinct
versions**:

```
ArtefactKey, ArtefactName, VersionIdA, PathA, HashA, SizeA,
           VersionIdB, PathB, HashB, SizeB, ByteIdentical, DeltaBytes, Notes
```

- `VersionId` = `V1`…`Vn`, assigned in ascending order of first appearance in
  the ordinally sorted path list — deterministic.
- `ByteIdentical` = `YES` when the two hashes are equal (the two "versions"
  are actually the same content); `NO` when they differ.
- `DeltaBytes` = signed difference `SizeB − SizeA`.
- Emit as a new deliverable: **`artefact-version-matrix.csv`**.

### A-1.4 Per-file release membership

Re-derive `ReleaseMembership` for **every** source file using this evidence
hierarchy, and record the basis in the `Notes` column of
`coverage-matrix.csv`:

| Basis | Condition | Membership |
|---|---|---|
| `CONFIRMED-BY-PAYLOAD` | the file lies under a deployment root established by the confirmed Target A / Target B paths or their evidenced sibling payload | the evidenced release |
| `CONFIRMED-BY-VERSION-PAIR` | the file's `ArtefactKey` has another instance elsewhere with a **different** hash, and the file's own directory has been attributed to a release by evidence | that release |
| `UNATTRIBUTED` | no evidence available | `UNKNOWN` |

**Two hard rules:**

1. **Never default a file into a release membership.** A file whose
   `ArtefactKey` is unique in the tree and which lies outside every evidenced
   payload root is `UNKNOWN`.
2. **An outcome in which every file carries the same membership value is not
   acceptable and will be treated as an over-claim.** The Phase 0 run reported
   837 of 837 files as `V-CD-2019`; that classification is not supported and
   must be re-derived.

Record the count of each basis in `release_membership_evidence_counts` in the
summary JSON.

### A-1.5 The `DEMO` tree

`DEMO` is not named in any repository document. Classify it with the same
evidence discipline: determine from its contents, size profile and payload
artefacts whether it is (a) the `V-ROOT-1.00` earlier build, (b) a demonstration
or sample deployment, or (c) something else. Record the verdict and its
evidence. **Do not assume it is the other release because it is the other
directory.**

---

## A-2 — Replacement for P0-6: version comparison

> **Supersedes P0-6 in full.**

P0-6 is executed over the **A-1.3 version-pair rows**, not over two release
roots. The comparison unit is the **artefact** (basename), because the two
releases occupy interleaved paths.

**Deliverables:**

| File | Content |
|---|---|
| `artefact-version-matrix.csv` | the A-1.3 version-pair matrix — **new** |
| `shared-identical.csv` | **new** — every pair of instances of the same `ArtefactKey` with **equal** SHA-256, listing both paths and the shared hash. Analysis performed on one such instance is valid for the other; record that fact |
| `release-membership.csv` | **retained, new schema** — per-file membership with its evidence basis |

`release-membership.csv` schema:

```
RelativePath, ArtefactKey, ArtefactName, Bytes, SHA256,
ReleaseMembership, MembershipBasis, VersionCount, VersionIds, Notes
```

`MembershipBasis` ∈ `CONFIRMED-BY-PAYLOAD` · `CONFIRMED-BY-VERSION-PAIR` ·
`UNATTRIBUTED`

**For every differing pair**, as in the original P0-6:

1. record both sizes, both hashes and the size delta;
2. record the exact paths;
3. establish the nature of the difference **only** where plain byte-level
   inspection permits it;
4. **do not assume** the difference is caused by timestamps or line endings;
5. **do not claim** semantic equivalence from superficial similarity.

**Files whose content differs between versions are HIGH-PRIORITY findings** and
must each be listed individually in the final report. This list is the
cross-release difference matrix and is the primary technical input to the next
phase.

---

## A-3 — Extension to P0-3: prior outputs that reside inside ZIP archives

The completed run inventoried **257 prior outputs**, which are the loose files
of the analysis root. The remaining prior outputs are **inside ZIP archives**
that P0-3 did not extract, and this is the direct cause of the 820
`UNKNOWN` coverage rows.

**Do not extract any archive to disk.** Read the archives **in memory** using
Python's standard-library `zipfile` module:

- enumerate every member of every `.zip` file under the analysis root;
- for each member compute the SHA-256 of its **decompressed bytes held in
  memory** — never written to disk;
- record for each member: container archive path, member path, uncompressed
  size, SHA-256, subject artefact, analysis type, completion status.

**Member classification:**

- Text and data members — `.md`, `.csv`, `.json`, `.txt`, `.py`, `.ps1`, `.xml`,
  `.yaml`, `.yml`, `.ini` — are classified and merged into
  `prior-output-ledger.csv`.
- Binary members — `.zip`, `.jpg`, `.png`, `.gif`, `.mdb`, `.exe`, `.dll`,
  `.swf`, `.mp4`, `.wmv` — are recorded with **identity data only**:
  `AnalysisType = other`, `CompletionStatus = UNKNOWN`.

**Schema change:** `prior-output-ledger.csv` gains a tenth column, `Container`
— the archive path for members, empty for loose files. Column order is
otherwise unchanged.

---

## A-4 — Re-execution scope

Re-execute **only** the following, in this order:

| Step | Workstream |
|---|---|
| 1 | A-1.1 — build the basename index from the existing `file-ledger.csv` |
| 2 | A-1.2 / A-1.3 — multi-instance classification and version-pair matrix |
| 3 | A-1.4 / A-1.5 — per-file membership with evidence; classify `DEMO` |
| 4 | A-3 — in-memory ZIP reading; merge into the prior-output ledger |
| 5 | A-1.4 (again) — re-derive `coverage-matrix.csv` from the merged prior outputs |
| 6 | A-2 — produce the version matrix, the shared-identical list and the new membership ledger |
| 7 | P0-7 — re-verify both targets; record the prior CONFIRMED result and confirm the hashes are unchanged |
| 8 | P0-8 — add the gaps this addendum exposes; **do not renumber or reuse** existing identifiers |
| 9 | P0-9 — regenerate `phase0-summary.json` |
| 10 | P0-10 — regenerate `PHASE-0-REPORT.md` |

**Do not re-hash the source tree.** The inventory and its hashes are established
and verified — 837 files, 0 hash failures, 0 enumeration failures. **Reuse
`file-ledger.csv` as the input to the re-derivation.** This keeps the re-run
cheap and preserves the verified evidence chain.

---

## A-5 — Amended deliverables (§7)

The nine evidence files plus the script remain. **Three files are added and one
schema changes:**

| File | Status |
|---|---|
| `artefact-basename-index.csv` | **NEW** — A-1.1 |
| `artefact-version-matrix.csv` | **NEW** — A-1.3 / A-2 |
| `shared-identical.csv` | **NEW** — A-2 |
| `release-membership.csv` | **SCHEMA CHANGED** — A-2 |
| `prior-output-ledger.csv` | **SCHEMA CHANGED** — one column added (`Container`) |
| all others | unchanged |

`PHASE-0-SPEC-ADDENDUM-01.md` — this document — is stored beside them and
recorded in the summary JSON under `specification_addenda`.

---

## A-6 — Amended acceptance criteria (§10)

| ID | Revised criterion |
|---|---|
| **A1** | Every file beneath the authorised source root is enumerated; zero hash failures; **and no source file is defaulted into a release membership without recorded evidence.** |
| **A3** | Every discoverable relevant prior analysis output is inventoried and classified, **including prior outputs that reside inside ZIP archives, read in memory without extraction.** |
| **A6** | The coverage matrix contains exactly one row per source file; no prior output appears in it; **and every file's `ReleaseMembership` is supported by a recorded evidence basis, or is `UNKNOWN`.** |
| **A7** | **Every multi-instance artefact has an explicit byte-identity verdict for every pair of its distinct versions, and every content-identical cross-path pair is listed.** |

**A7 is now achievable.** `UNKNOWN` membership for artefacts that cannot be
attributed is a **valid documented outcome** and is not a blocker. The blocker
was the absence of a comparison unit — the comparison unit is now the artefact.

All other criteria (A2, A4, A5, A8–A16) remain unchanged.

---

## A-7 — Completion rule

`phase_complete = true` only when **all** of A1–A16 PASS under the amended
definitions.

`phase_complete` **may** be `true` while some files remain `UNKNOWN` or
`UNATTRIBUTED` — provided the reason is recorded and the artefact was
genuinely unattributable from the evidence. What blocks completion is an
**unresolved comparison**, not an unresolved attribution.

---

## A-8 — Reporting additions

Add to §13.9 of the specification:

- the count of artefacts with more than one instance;
- the count of version pairs, split into byte-identical and differing;
- the **HIGH-PRIORITY list of artefacts whose content differs between
  versions**, each with both hashes, both sizes and both paths;
- the count of files by membership basis, showing that no file was defaulted;
- the classification verdict for the `DEMO` tree and its evidence.

State coverage state only. **Do not nominate, rank or shortlist the next
analysis target.**

---

*End of Addendum 01.*
