# Method to SQL Mapping — REAL LAB SYSTEM v1.605 (V-CD-2019)

**Scope.** Maps extracted SQL string literals (VAs) to issuing code sites and, where
evidence permits, to VB6 objects. The v1.605 binary is **absent locally**; all
statements derive from read-only inventories under `D:\Collective work`. No binary,
installer or script was executed; no database was contacted. Static evidence only.

**Confidence legend.** VERIFIED = directly present in a named evidence field/row;
STRONGLY_SUPPORTED = multiple independent evidence classes agree; INFERRED =
reasoned reconstruction without a decompiled call site; UNVERIFIED = asserted by an
earlier report but not re-derived here; CONFLICTED = two evidence sets disagree;
UNKNOWN = evidence insufficient.

---

## 1. Statement inventory re-derived (VERIFIED)

Source: `statements__[analysis_BATCH-04_sql_v1605].csv` (columns
`va,verb,tables,has_star,length,text`), 1,797 data rows. Re-counted in this session:

| Verb | Rows | `has_star=True` | Notes |
| --- | ---: | ---: | --- |
| `SELECT` | 1,381 | 500 | dominant read path |
| `ALTER` | 241 | 0 | runtime schema migration |
| `DELETE FROM` | 72 | 0 | includes the manual patient cascade |
| `OTHER` | 49 | 1 | backup commands + SQL fragments |
| `CREATE` | 27 | 0 | runtime schema creation |
| `SHAPE` | 27 | 3 | hierarchical MDAC recordsets |
| `INSERT INTO` | **0** | — | VERIFIED negative |
| `UPDATE … SET` | **0** | — | VERIFIED negative |
| `BEGIN/COMMIT/ROLLBACK TRAN` | **0** | — | VERIFIED negative |

`has_star` totals: 504 True / 1,293 False. 117 distinct table tokens occur in the
`tables` column (pipe-separated). The zero `INSERT`/`UPDATE` result is the single
most important mapping fact: **no literal write statement exists in the inventory**,
so all persistence is attributed to bound recordset operations (INFERRED).

---

## 2. What "method mapping" can and cannot be established

`objects-full__[analysis_BATCH-04_vb6_v1605].txt` header records:
`objects 210`, `methods 8026`, `method_entries_read 1268`, `unique_entries 0`,
`methods_without_address 1268`. `vb6-objects.json` gives each object a
`method_names_va`; for the data-access modules `labdb` (index 3, 24 methods),
`labdb1` (index 20, 20), `labfunction` (index 78, 25), `saveIOtemp` (index 174, 10)
and `printerselect` (index 140, 11) that VA is `0x0`, and no method bodies were
recovered (VERIFIED). Consequently:

- A statement can be tied to a **procedure base address**, but **not to a named VB6
  method** — the unresolved gap recorded as UNR-008 in the prior reports.
- `normalrange-coderefs.json` supplies the only direct string→code-site bridge in
  the corpus: `procedures` maps procedure base addresses to arrays of
  `{string, site, target}` where `target` is the literal's VA and `site` is the
  reference address. 7,936 `prologue_candidates` were examined.

**Status of method-name attribution: UNVERIFIED.** All object associations in this
document and in `Method_To_Table_Mapping.md` are structural correlations, not
decompiled call graphs.

---

## 3. Verified string → code-site examples (`normalrange-coderefs.json`)

| String literal | Literal VA | Example procedure base | Example site(s) | Confidence |
| --- | --- | --- | --- | --- |
| `SELECT * FROM normalrange where testcode=` | `0x006a8ddc` | `0x024d85f0` | `0x024e1568`, `0x024e15ce`, `0x024e1730`, … | VERIFIED |
| `SELECT * FROM normalrange where testcode=` | `0x006be9a8` / `0x006bf080` | `0x01ddcdc0`, `0x024d85f0` | `0x01de0098`, `0x024e1568`, … | VERIFIED |
| `REFRANGE='1' and pregsmok` | `0x006a8e42` | — (string corpus) | 28 listed code sites | VERIFIED |
| `REFRANGE='2' and ageunit='` | `0x006a8e8e` | — | 20 listed code sites | VERIFIED |
| `' and firstold <= ` | `0x006a2c88` | `0x024d85f0` | `0x024e15e7`, `0x024e17b1`, … | VERIFIED |
| `' and ageunit='` | `0x006a2c64` | `0x024d85f0` | `0x024e178b`, `0x024e2e09`, … | VERIFIED |
| `ageunit` | `0x006b9d28` | `0x0231df80`, `0x02321940` | `0x0231f192`, `0x023239c0`, … | VERIFIED |
| `firstold` | `0x006b9cf8` | `0x0231df80`, `0x02321940` | `0x0231ef2d`, `0x02323188`, … | VERIFIED |
| `Years` / `Days` / `Mons.` | `0x006a6c98` / `0x006a6c34` / `0x006a6c88` | multiple | e.g. `0x01d9b483`, `0x01d9b185`, `0x01d9b35f` | VERIFIED |

The concentration of `normalrange` string references across many procedures is
consistent with **one reference-range lookup routine duplicated into each `Result*`
and profile module**, rather than a single shared helper (STRONGLY_SUPPORTED).
The prior report's qualitative conclusion at site `0x024e1568` is confirmed here.

---

## 4. Result-state and audit literals (VERIFIED)

These VAs in `statements__[analysis_BATCH-04_sql_v1605].csv` select the four
workflow flags from `patienttest` and their audit columns:

| VA | Literal (as stored) | Purpose |
| --- | --- | --- |
| `681ED2` | `Select patientid,printwith,resultfinsh,resultreview,resultprint from patienttest where patientid= '` | all four result flags in one read |
| `6860BE`, `6861D2` | `Select testcode,patientid,resultfinsh,edituser,editdate from patienttest where patientid= '` | finish flag + editor/date (two spacing variants) |
| `686312`, `68647E` | `Select testcode,patientid,resultreview,prevuser,prevdate from patienttest where patientid= '` | review flag + reviewer/date |
| `6865A6`, `68666E` | `Select testcode,patientid,resultprint,printuser,printdate from patienttest where patientid= '` | print flag + printer/date |

Each is a `SELECT` with a `patientid` predicate; the paired VAs differ only by
whitespace. This is direct evidence of a **per-result audit trail** implemented as
columns on `patienttest`, not as a separate log table (VERIFIED at SQL level).

Note: the task brief cites `6843E2` as a `SUBSTRING(...,9,1)` variant; that VA is
**not present** in the inventory. The verified neighbours are `6843F2`
(`SUBSTRING(patientinfo.patientid,10,3)`) and `68463A`
(`mid(patientinfo.patientid,10,3)`) — CONFLICTED with the brief, inventory wins.

---

## 5. Worklist and branch-discriminator literals (VERIFIED)

| VA | Literal prefix | Role |
| --- | --- | --- |
| `682F56` | `Select SUBSTRING(patientid,10,3) as patid , patientid , patientnamear , completes from patientinfo where requestdate = '` | daily worklist by request date |
| `683C72` | `Select SUBSTRING(patientid,10,3) as patid, … from patientinfo where requestdate >= '` | worklist by date range |
| `6843F2` | `SELECT SUBSTRING(patientinfo.patientid,10,3) … FROM patientinfo INNER JOIN patienttest …` | joined worklist (SQL Server dialect) |
| `68463A` | same with `mid(patientinfo.patientid,10,3)` | Jet/Access dialect twin |

The `10,3` slice of the 13-character patient ID is the branch discriminator carried
in the ID (`08-deployment-installer.md` describes `SUBSTRING(patientid,9,1)`; the
two descriptions are CONFLICTED by offset and must not be merged).

---

## 6. SHAPE hierarchy literals (VERIFIED)

27 `SHAPE` rows. The two dialects are explicit:

| VA | Parent → child | Dialect marker |
| --- | --- | --- |
| `672722` | `subgrouptable` → `testtable` | `testorno= 'True'` |
| `6729D6` | `subgrouptable` → `testtable` | `testorno=True` (Jet) |
| `672E3E` | `testtable` → `normalrange` | `'True'`, `patientprice` |
| `673172` | `testtable` → `normalrange` | `True`, `patientprice` (Jet) |
| `673662` | `testtable` → `normalrange` | `labprice` variant |
| `673986` | `testtable` → `normalrange` | `labprice`, Jet |
| `68A74E` | `wasl` self-hierarchy | — |
| `6B44F0`…`6B70F0` | `patientinfo` → `patienttest` (worklist parent/child) | `'`/`#` date variants |
| `6F2544`, `709B84` | `subgrouptable`+`testtable` → `pricelist`/`customtesttable` | — |
| `70ECE4`, `70F278` | `testname` → `patienttest` | — |

These are consumed by VB6 DataReport objects (`ReportFatora`, `ReportTestTime`,
`ReportPCR`, `Reportworkp`, … in `objects-full__[analysis_BATCH-04_vb6_v1605].txt`),
all of which have **zero or one declared method** — the report template effectively
is the data plan (STRONGLY_SUPPORTED).

---

## 7. Write and maintenance literals (the only "write" surface)

No `INSERT`/`UPDATE` exists. The write-like literals are:

| VA(s) | Literal | Meaning |
| --- | --- | --- |
| `6A740A`–`6A84EE` | `Delete FROM <table> where patientid='` (51 contiguous statements) | manual patient-deletion cascade across `patientinfo`, `payment`, `patienttest` and every result table |
| `6894F2`, `6A868A`, `6B7B84`, `6DEB40`, `7352CC`, … | `DELETE FROM wasl/testtemp/testname/history/SMSSender` | bulk maintenance clears |
| `6F6360`, `6F6C44` | `DELETE FROM payment where patientid/paymentnum` | targeted payment removal |
| `72EFD0`, `733468` | `DELETE FROM pictures/samples where patientid` | attachment/sample cleanup |
| `66FCB2`, `66FD56` | `EXEC sp_addumpdevice 'disk', '` / `EXEC sp_dropdevice '` | SQL Server backup device management |
| `67413A`–`681456`, `6DC264`–`6DC714` | `ALTER TABLE` / `CREATE TABLE` | runtime schema migration (268 literals in `ddl-evidence__[analysis_BATCH-04_sql_v1605].txt`) |

All are attributed to the application's own maintenance commands; **ordinary data
entry has no literal statement at all** and must be bound-recordset driven
(INFERRED).

---

## 8. Scope conflict recorded

`database-and-data-access-analysis.md` (prior report) counts 1,370 embedded
SELECTs, 14 CREATE and 120 ALTER and ranks `patienttest` first (138 targets).
The BATCH-04 CSV counts 1,381 / 27 / 241 and ranks `patientinfo` first. The two use
different attribution rules (string prefix vs. every table named). Both are
retained; the CSV is authoritative for this document. Status: CONFLICTED by method,
not by evidence.

---

## 9. Not evidenced

- No decompiled method body naming the issuer of any statement (UNR-008).
- No proof that `labdb`/`labdb1` issue the statements attributed to them — their
  `method_names_va` is `0x0` and no addresses were recovered (UNKNOWN).
- No runtime trace, network capture or live-server observation.

See `Application_Data_Access_Architecture.md`, `Method_To_Table_Mapping.md`,
`Database_Transactions.md` and `Data_Flow_Reconstruction.md` in this directory.
