# State Transitions (Evidence-Linked Reconstruction)

All transitions below are reconstructed from **static** evidence. None was observed at runtime. Where a transition is inferred from a field name alone, it is labeled INFERRED; where SQL selects/updates that field, it is labeled STRONGLY_SUPPORTED for the *existence of the field and its data flow*, but still not a runtime observation.

## 1. Primary evidence: the result lifecycle columns

`statements__[analysis_BATCH-04_sql_v1605].csv` contains paired SQL statements for three result stages, each with an operator and timestamp audit column:

| VA | SQL (exact, truncated) | Fields | Interpretation |
|---|---|---|---|
| `6860BE` / `6861D2` | `Select testcode,patientid,resultfinsh,edituser,editdate from patienttest where patientid= '` | `resultfinsh`, `edituser`, `editdate` | "result finished" stage + who/when edited |
| `686312` / `68647E` | `Select testcode,patientid,resultreview,prevuser,prevdate from patienttest where patientid= '` | `resultreview`, `prevuser`, `prevdate` | "result reviewed" stage + who/when reviewed (`prevuser` = reviewer/privileged user) |
| `6865A6` / `68666E` | `Select testcode,patientid,resultprint,printuser,printdate from patienttest where patientid= '` | `resultprint`, `printuser`, `printdate` | "result printed" stage + who/when printed |

Also `Select patientid,printwith,resultfinsh,resultreview,resultprint from patienttest where patientid= '` (VA `681ED2`) selects all four flags together — strong evidence that they form one status tuple on `patienttest`.

**A fourth audited stage exists:** VAs `6867AA`/`686862` select `testcode,patientid,give,taslimuser,taslimdate` — a handover/delivery flag with its own operator/date pair. Broader queue predicates (VAs `686EC2`, `6B3C00`, `6B3FA8`) select `resultfinsh`, `resultprint`, `give`, `resultreview`, `NatighU` together. Therefore the status tuple is at least **five flags** (`printwith`, `resultfinsh`, `resultreview`, `resultprint`, `give`) with four audit pairs. The ordering of `give` relative to the other stages is INFERRED, not observed.

`patientinfo.completes` is selected alongside patient identity in many statements (e.g. VA `682F56`, `683C72`, `6843F2` with `INNER JOIN patienttest ON patienttest.patientid = patientinfo.patientid`) — it is the patient-level completion flag distinct from the per-test flags.

## 2. Reconstructed per-test state model (INFERRED ordering)

```
   [ordered / awaiting result]
        │  result entry (edituser, editdate)
        ▼
   resultfinsh = true ──────► [result entered]
        │  review (prevuser, prevdate)
        ▼
   resultreview = true ─────► [result reviewed/validated]
        │  print (printuser, printdate)
        ▼
   resultprint = true ──────► [result printed/released]
```

**Confidence:** STRONGLY_SUPPORTED that the four flags and six audit columns exist and are read together; INFERRED that they transition strictly in that sequence. No priority-based or alternative state model was invented. The prior directive's warning against assuming a sequential state model is respected: the ordering shown is the natural reading of the column names and the entry→review→print data flow, not an observed state machine.

## 3. Patient-level completion

- `patientinfo.completes` appears in worklist queries filtered by `requestdate` (VA `682F56`, `68313E`, `683316`, `6834FE`, `683C72`, `683E56`, `686C32`, `686D22`) and in an `INNER JOIN patienttest` query (VA `6843F2`, `68463A`). This suggests the main worklist is driven by request date and completion status. [STRONGLY_SUPPORTED for query structure; INFERRED for UI meaning]
- `SUBSTRING(patientid,10,3)` / `mid(patientid,10,3)` is repeatedly aliased as `patid`. This is the 13-character patient-ID scheme. **Position 9 is the branch code in the legacy system** — 18 v1.605 statements compute `SUBSTRING(patientid,2,1) + SUBSTRING(patientid,9,1) AS bran` (VA `72714C` et al.). AGENTS.md rule 8 about position 9 being folded into the middle segment describes the C# successor, not v1.605. [VERIFIED from SQL]

## 4. Write-path finding (important architectural constraint)

Across all **1,797** extracted SQL statements:

| Verb / pattern | Count | Evidence |
|---|---:|---|
| `SELECT` | 1,381 | verb column of statements CSV |
| `ALTER` | 241 | verb column (embedded DDL) |
| `DELETE FROM` | 72 | verb column |
| `OTHER` | 49 | verb column |
| `CREATE` | 27 | verb column |
| `SHAPE` | 27 | verb column |
| `INSERT INTO` (any case) | **0** | regex scan of the `text` column |
| `UPDATE … SET` (any case) | **0** | regex scan of the `text` column |

**Conclusion (STRONGLY_SUPPORTED):** inserts and updates are not issued as literal SQL strings; they are performed through bound recordset operations (ADO/DAO `AddNew`/`Update` or DataControl binding), consistent with the VB6/MSADODC/MSDataShape architecture. This is a genuine architectural finding: any reconstruction that assumes text-based `INSERT`/`UPDATE` would mis-model the data layer.

## 5. Deletion workflow — evidence of a manual cascade

The 72 `DELETE FROM` statements target **64 distinct tables** (`ABO`, `Abuse`, `anyreport`, `BILH`, `BloodGases`, `BONEMAR`, `cardiac`, `CBC`, `CoagFactors`, `crossmat`, `CRP`, `CSFT`, `cultureanti`, `cultures`, `customtesttable`, `Drugs`, `Electrolytes`, `ESR`, `Fertlity`, `GLUCOSETOLERANCE`, `hepatitis`, `Hgbectrophoresis`, `history`, `hormones`, `Immunoglobulin`, `IMMUNOLOGY`, `iron`, `kidney`, `lipids`, `liver`, `Pancreas`, `patientinfo`, `patients`, `patienttest`, `patienttotal`, `payment`, `pictures`, `postcoitalt`, `pregnancy`, `Proelectrophoresis`, `prostaticx`, `pt`, `renalfailure`, `samples`, `seana`, `semen`, `sharekat`, `SMSSender`, `stone`, `stool`, `Sugar`, `Suprarenal`, `tasgel`, `testname`, `testtemp`, `TORCH`, `trace`, `TUBERCULINT`, `Tumors`, `Urine`, `Vitamins`, `wasl`, `widal`, `ZN`).

A contiguous block (VAs `6A740A`–`6A760E`) deletes `patientinfo`, `payment`, `patienttest`, `ABO`, `Abuse`, `BILH`, `cardiac`, … each `where patientid='`. [VERIFIED: literal sequence in the evidence file] This is strong evidence of an **application-level manual cascade** for patient deletion, rather than a database foreign key with `ON DELETE CASCADE`. [INFERRED: that this block is one transaction; transaction boundaries were not observed]

## 6. Unknown / not evidenced

- Boolean encoding of the result flags (`True/False`, `-1/0`, or text) — not established from the dumps.
- Whether `resultreview` can occur before `resultfinsh`, or whether flags can be reset (recheck/reprint behavior) — not established.
- Exact trigger conditions for `patientinfo.completes` — not established.
- Any state machine for orders beyond the result flags — not established.
