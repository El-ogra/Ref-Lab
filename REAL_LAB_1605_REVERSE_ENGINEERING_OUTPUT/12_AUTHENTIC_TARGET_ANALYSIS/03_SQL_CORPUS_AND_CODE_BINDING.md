# SQL / DDL corpus re-extraction and code binding — authentic v1.605

**Target:** `F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe`
**Scripts:** `scripts/vb6_sql2.py` (extraction), `scripts/bind_verify.py` (byte-level validation), `scripts/sqlbind.py` (definitive map), `scripts/audit_prior_corpus.py` (prior-corpus audit), `scripts/reanchor.py` / `scripts/verify_claims.py` (re-anchoring).
**Machine-readable output:** `evidence/sql-literals-utf16.json`, `evidence/sql-literal-to-code-sites.json`, `evidence/prior-corpus-audit-vs-authentic-binary.json`, `evidence/va-reanchoring.json`.

---

## 1. The encoding discovery

An ASCII scan of `.text` finds only **6** SQL-looking literals. The reason: **every SQL statement literal in this build is a UTF-16LE (Unicode) string**, which the prior evidence never documented. Concretely, the byte sequence at the prior VA `0x69EA76` reads

```
74 00 69 00 6e 00 66 00 6f 00 2e 00 …   →  "tinfo.patientid,patientinfo.requestdate,…"
```

and the prior VA convention turns out to be a **file offset into a differently-laid-out copy**, not a virtual address of the shipped image.

Scanning the authentic `.text` as UTF-16LE yields **38,809 Unicode literals**, of which **1,758 are SQL/DDL statements**.

---

## 2. Re-extracted corpus

| Class | Authenticator | Prior claim | Status |
|---|---:|---:|---|
| SELECT | 1,373 | 1,381 | consistent (classification boundaries) |
| ALTER TABLE | 241 | 241 | **VERIFIED** |
| DELETE FROM | 75 | 72 | consistent |
| SHAPE | 39 | 27 | consistent |
| CREATE TABLE | 27 | 27 | **VERIFIED** |
| EXEC | 2 | (in "OTHER 49") | consistent |
| WITH | 1 | (in "OTHER 49") | consistent |
| **INSERT INTO** | **0** | 0 | **VERIFIED negative — bound-recordset writes** |
| **UPDATE … SET** | **0** | 0 | **VERIFIED negative — bound-recordset writes** |
| **Total SQL/DDL literals** | **1,758** | 1,797 | the 39-row delta is classification, not loss |

Both prior *negatives* — no literal `INSERT INTO` and no literal `UPDATE … SET` — are reproduced exactly against the authentic binary, which independently confirms the bound-recordset write model.

---

## 3. SQL literal → code site binding (previously UNVERIFIED — now RESOLVED)

Each literal's virtual address was located as an **immediate operand** inside `.text`; the enclosing instruction bytes were classified and the classification was **validated against raw bytes** before any figure was published (`scripts/bind_verify.py`, sample outputs below).

```
literal VA=0x0066f788 -> code VA=0x01b4357d kind=None
    bytes[-10:+6]: 12 00 c7 45 fc 0a 00 00 00 68 88 f7 66 00 8d 55
    stmt: 'SELECT * FROM loger where lognum= '
literal VA=0x0066e940 -> code VA=0x01b427c7 kind=mov_reg_imm32
    bytes[-10:+6]: ec 10 40 00 e8 5a 7e 12 00 ba 40 e9 66 00 b9 4c
    stmt: 'SELECT pricemethod, patientnamear, patientphone, patienttel FROM patientinfo GROUP BY …'
```

| Measure | Value |
|---|---:|
| SQL/DDL literals | 1,758 |
| Literals with ≥ 1 code reference site | **1,750 (99.5 %)** |
| Total code reference sites | **3,718** |
| `push imm32` | 2,278 |
| `mov reg, imm32` | 702 |
| `mov [mem], imm32` and other immediate forms | 737 |
| `xor eax, imm32` | 1 |
| Distinct SELECT code sites | 3,173 (VA span `0x004B3D68` – `0x04C7A855`) |

---

## 4. Embedded DDL — execution trigger (previously UNKNOWN — now STRONGLY_SUPPORTED)

All **268** DDL literals (27 CREATE + 241 ALTER) bind to **307 distinct code sites**.

| Cluster (64 KB bucket) | Sites |
|---|---:|
| `0x01B60000`–`0x01B70000` | 52 |
| `0x01B70000`–`0x01B80000` | 88 |
| `0x01B80000`–`0x01B90000` | 89 |
| `0x01B90000`–`0x01BA0000` | 62 |
| `0x02ED0000`–`0x02EE0000` | 10 |
| `0x02EE0000`–`0x02EF0000` | 4 |
| two isolated sites near `0x01910000` | 2 |

**291 of 307 sites (94.8 %) lie inside a single 512 KB window beginning at `0x01B61DAD`.** Within that window the statements occur in exactly the order recorded in the prior DDL evidence file:

```
0x01b61dad  ALTER Table semen ADD testcode Smallint
0x01b62090  ALTER Table patientinfo ADD gehaname varchar(50)
0x01b62374  Create Table testtemp (patientid varchar(13)
0x01b62658  ALTER Table testtemp ALTER COLUMN normalrang MEMO
0x01b6293b  ALTER Table SMSSender ALTER COLUMN username varchar(50)
0x01b62c1f  ALTER Table SMSSender ALTER COLUMN smsMobile varchar(100)
0x01b62f03  ALTER Table semen ALTER COLUMN Liquefaction varchar(10)
0x01b631e6  Create Table MyLabSetting (LabID Smallint
0x01b681be  ALTER Table lipids ADD nonhdl varchar(10)
0x01b684c1  ALTER Table BloodGases ADD pO2AA1 varchar(10)
   … continuing through Create Table LBC / IgEallergy / smsgroups / smslist /
      SMStemplate / SMSSender / patienttest NatighU / …
```

A **second, dialect-mirrored copy** of the same sequence follows (e.g. `ALTER Table SMSSender ADD username varchar(50)` vs `ALTER Table SMSSender ADD COLUMN username varchar(50)`; `ADD grant13 Bit` vs `ADD COLUMN grant13 YESNO`), and a smaller third cluster sits at `0x02ED0000`–`0x02EF0000`.

**Conclusion.** The DDL is not scattered ad-hoc statements but **one ordered schema-migration routine**, present in two dialect variants, applied at start-up or upgrade time. The exact trigger event remains **UNKNOWN** (no runtime observation was performed).

### 4.1 The migration routine is now a single, named function (VERIFIED)

Ghidra's function inventory localises the whole DDL sequence to **one function**:

| Fact | Value |
|---|---|
| Function | `FUN_01b61cf0` |
| Entry | `0x01B61CF0` |
| Body | `0x01B61CF0` – `0x01B9B59C` (235,656 bytes ≈ 230 KB) |
| Distinct SQL/DDL statements referenced | **250** |
| First statement referenced | `ALTER Table IgEallergy ADD COLUMN Item12 varchar(50),RESULT12 varchar(…)` |

The first DDL code site (`0x01B61DAD`) lies **inside** this function, and 301 of the 3,718 total SQL code sites belong to it. The schema-migration routine is therefore **VERIFIED** as a single procedure rather than inferred from clustering — and its size explains the clustering: a single 230 KB routine that walks the entire statement list in order.

Evidence: `evidence/ghidra-sql-site-to-function.tsv`, `evidence/ghidra-sql-by-function.tsv`.

---

## 5. Audit of the prior corpus

```
scripts/audit_prior_corpus.py <image> <statements.csv> <ddl.txt> <out.json>
```

| Audit dimension | Against **v1.605** (authentic) | Against **v1.00** (old build) |
|---|---:|---:|
| Statement texts found verbatim | **1,698 / 1,797** (+95 as substrings) | 836 / 1,797 |
| DDL texts found verbatim | **262 / 268** (+6 as substrings) | **0 / 268** |
| Prior VAs resolving to their claimed statement | **0 / 1,797** | 0 / 1,797 |
| Prior VAs resolving to their claimed DDL | **0 / 268** | 0 / 268 |

**The prior corpus content is v1.605** (the old build contains *none* of the 268 DDL statements), **but every prior virtual address is wrong** — systematically **0x42 or 0x44 bytes below** the true address in the shipped file. Hence every VA-anchored claim in the prior knowledge base has been re-anchored.

---

## 6. Re-anchoring table (verified statement text at the authentic address)

| Prior claim | Prior VA | **Authentic VA** | Verified text |
|---|---:|---:|---|
| net = gross − discount, `ROUND(…,2)` | `69EA76` | **`69EAB8`** | `Select pricemethod, Round(sum(totalprice),2) as tot1, Round(sum(totalpricedisc),2) as tot2, Round((sum(totalprice) - sum(totalpricedisc)),2) as tot3, Round(sum(elmadfoo3),2) as tot4, Round(sum(elbakyma3mal),2) as tot5 from patientinfo where requestdate >= '` |
| referrer commission | `694C92` | **`694CD4`** | `SELECT ROUND(SUM(Gehats.omola * patientinfo.totalpricedisc / 100),2) AS Expr1 FROM patientinfo INNER JOIN Gehats ON patientinfo.referredbyname = Gehats.gehahname Where requestdate >= '` — base is **`totalpricedisc`**, not a generic amount |
| company net `farq` | `722674` | **`7226B8`** | `SELECT wareed , saader, (wareed - saader) as farq , taamelnote , sherkaname, taameldate FROM sharekathesab where sherkaid= '` |
| unpaid predicate | `6CEA20` | **`6CEA64`** | `SELECT patientid, labid, patientsefa, patientnamear, patientsex, patientage, patientageunit, requestdate, referredbysefa, referredbyname, completes FROM patientinfo Where (totalpricedisc …` |
| `loger` table | `66F746` | **`66F788`** | `SELECT * FROM loger where lognum= ` |
| error string | `6717E2` | **`671824`** | `An error has occurred, Please make sure your browser, Or select other` |
| `sp_addumpdevice` | `66FCB2` | **`66FCF4`** | `EXEC sp_addumpdevice 'disk', '` |
| `sp_dropdevice` | `66FD56` | **`66FD98`** | `EXEC sp_dropdevice '` |
| SHAPE subgroup↔testtable | `672722` | **`672764`** | `SHAPE {SELECT subgrouptable.subgroupcode, subgrouptable.subgroupname FROM subgrouptable GROUP BY …} AS cmdbill APPEND ({SELECT * FROM testtable where testorno= 'True' and testsimp <> '' order by testsimp} AS cmdbill_d RELATE subgroupcode TO subgroupcode) AS cmdbill_d` |
| price catalogue | `6A9632` | **`6A9674`** | `Select testcode, testabrev, testname, subgroupname, testtime, inotherlab, testnamear, typeofsample, otherlabname, otherlabprice, patientprice, labprice from testtable where testcode = ` |
| invoice header | `6A5796` | **`6A57D8`** | `SELECT patientid, totalprice, totalpricedisc, elbakyma3mal, discount100, discount, elmadfoo3, completes FROM patientinfo where patientid='` |

---

## 7. Branch-code rule — both prior statements are true, and they are different rules

`SUBSTRING(patientid,2,1) + SUBSTRING(patientid,9,1) as bran` occurs **exactly 18 times** (first at `0x727190`):

```
0x727190  SELECT  SUBSTRING(patientid, 5, 2) + ' <-- ' + SUBSTRING(patientid, 2, 1) + SUBSTRING(patientid, 9, 1) as mmmdx ,
           SUBSTRING(patientid, 2, 1) + SUBSTRING(patientid, 9, 1) as bran , …
0x727690  SELECT  MID(patientid, 5, 2) + ' <-- '  + MID(patientid, 2, 1) + MID(patientid, 9, 1) as mmmdx ,
           MID(patientid, 2, 1) + MID(patientid, 9, 1) as bran , …
```

Each SQL-Server statement has a Jet-dialect twin — the **dual-dialect claim is verified**.

A **separate** rule, `SUBSTRING(patientid,10,3) as patid`, occurs **58 times** (first at `0x682F98`), again with `MID(...)` twins:

```
0x682f98  Select SUBSTRING(patientid,10,3) as patid , patientid , patientnamear , completes from patientinfo where requestdate = '
0x683180  Select mid(patientid,10,3) as patid , patientid , patientnamear , completes from patientinfo where requestdate = '#'
```

The prior knowledge base carried both rules but presented them under one heading; they are distinct derived values: **`bran` = position 2 + position 9** (branch code) and **`patid` = positions 10–12**.

---

## 8. Reference-range rule — VERIFIED

The binary issues `REFRANGE='1'` / `REFRANGE='2'` / `REFRANGE='3'` predicates, e.g.

```
0x6a8e84   ' and REFRANGE='2' and ageunit='
0x6bea28   "SELECT * FROM normalrange where testcode=33 and REFRANGE='2' and ageunit='"
0x6bee00   "SELECT * FROM normalrange where testcode=199 and REFRANGE='2' and ageunit='"
```

and the shipped Access mirror contains **no `REFRANGE='2'` rows at all** (distribution: empty 797, `'0'` 345, `'1'` 10, `'2'` **0**). The prior claim that the runtime reads reference ranges that the shipped mirror lacks is therefore **confirmed**.

The DDL adds the column in both dialects: `ALTER Table normalrange ADD REFRANGE varchar(1)` (`0x67ABDC`) and `ALTER Table normalrange ADD COLUMN REFRANGE varchar(1)` (`0x680F64`).

---

## 9. Result-lifecycle predicates — re-anchored

| Flag set | Authentic occurrences |
|---|---:|
| `resultfinsh` + `edituser` / `editdate` | 92 literals contain `resultfinsh` |
| `resultreview` + `prevuser` / `prevdate` | 68 literals contain `resultreview` |
| `resultprint` + `printuser` / `printdate` | 90 literals contain `resultprint` |
| `give` + `taslimuser` / `taslimdate` | 6 literals contain `taslimuser` |
| `NatighU` (result value) | 8 literals contain `natighu` |

The four-stage audit chain (`resultfinsh` → `resultreview` → `resultprint` → `give`) plus the six audit columns is therefore **present in the authentic corpus**, with corrected addresses.

---

## 10. Residual limitation

The binding stops at the **code address**: the image retains no procedure names (§6 of `02_VB6_OBJECT_AND_METHOD_STRUCTURE.md`), so a code site cannot be labelled with a handler name. What is additionally recoverable is the **form/control attribution**: each form's control table carries the code address of every control's event handler, so a code site can be attributed to the nearest enclosing control handler of a named form, using the control names that *are* stored. That attribution was not computed here because the form→control tables were only partially mapped; it is the natural next step once a function inventory is available.

---

## 11. Function-level method → SQL map (the prior work's explicit UNVERIFIED item — now RESOLVED)

Ghidra's function inventory turns the address-level binding into a **procedure-level** map: every function is matched to the SQL statements its instructions reference.

| Measure | Value |
|---|---:|
| SQL/DDL code sites inside a function | **3,566 / 3,718 (95.9 %)** |
| Functions referencing ≥ 1 SQL statement | **928** |
| Functions referencing non-SQL string literals (UI text, captions, report labels) | **2,796** |
| Total function→reference edges streamed | 8,507,170 |

Evidence: `evidence/ghidra-sql-by-function.tsv` (function → its SQL statements), `evidence/ghidra-strings-by-function.tsv` (function → its other literals), `scripts/sql_by_function.py`.

### 11.1 The twenty busiest SQL-bearing functions

| Distinct SQL statements | Function | What it does |
|---:|---|---|
| **250** | `FUN_01b61cf0` | the **schema-migration routine** (`0x01B61CF0`–`0x01B9B59C`) |
| **51** | `FUN_02d36160` | patient-deletion cascade — `Delete FROM Proelectrophoresis where patientid='…'` |
| **51** | `FUN_01db4350` | patient-deletion cascade — same statement set |
| **30** | `FUN_03e7ddc0` | result queue — `SELECT patientid,testcode,resultfinsh,resultprint,resultreview FROM patienttest …` |
| **22** | `FUN_03e1fcd0` | result queue (second dialect) |
| 18 | `FUN_0241ee80` | — |
| 14 | `FUN_01ddcdc0` | — |
| 13 | `FUN_038c9580` | `SELECT * FROM patients order by patientusname` — the **operator list** |
| 12 | `FUN_04659a70`, `FUN_04658700`, `FUN_04615a30` | the three **SHAPE hierarchical report generators** (`Shape 01`) |
| 12 | `FUN_02ee3100` | `SELECT * FROM insulin where id=10` |
| **11** | `FUN_01c7c0b0` | `SELECT SUM(Gehats.omola * patientinfo.totalpricedisc / 100) AS Expr1 FROM patientinfo INNER JOIN Gehats …` — the **referrer-commission** routine |
| 10 | `FUN_01e27d40` | `SELECT patienttest.patientid, patienttest.subgroupcode, patienttest.test…` — the **composite-test** fetch |
| 10 | `FUN_01bec430` | `Select testcode,patientid,resultprint,printuser,printdate from patient…` — the **print queue** |
| 7 | `FUN_0492acd0`, `FUN_01c05f30` | `Select mid(patientid,10,3) as patid …` — the **`patid` worklist** rule |
| 7 | `FUN_02fc2830`, `FUN_02fb4290` | `SELECT Top 25 requestdate,column1,…` — **statistics** |
| 7 | `FUN_02d7b200` | `Select testcode, testsimp, arrangnum, RESULTT, RESULTCASEPRINT …` — the **order row** fetch |
| 7 | `FUN_02262ec0` | `SHAPE {SELECT patientinfo.patientid,patientinfo.barcode,…}` |

### 11.2 What this closes

- **The 64-table patient-deletion cascade** is not 64 scattered statements: it is implemented in **two** functions of **51 statements each** (`FUN_02d36160`, `FUN_01db4350`), both beginning with the same `Delete FROM Proelectrophoresis where patientid='…'` — the two dialect variants of the same cascade.
- **The result-lifecycle queue** (`resultfinsh` / `resultreview` / `resultprint` together with `patientid`, `testcode`) lives in **two** functions (`FUN_03e7ddc0`, `FUN_03e1fcd0`) — again the SQL-Server and Jet variants of one routine.
- **The referrer commission**, **the operator list**, **the composite-test fetch**, **the print queue** and **the `patid` worklist rule** each have a single owning function.

### 11.3 What remains open

Attaching a *name* to each function requires the per-object procedure-address table, which this build does not expose in a directly resolvable form: the per-form control table stores, beside each control name and its flags, a value such as `0x001A5DD4` that is **below the image base** and is therefore neither a VA nor a file offset (it behaves as an index into a per-image descriptor table). The per-object `ObjectInfo` structure was fully decoded — `+0x00` type flags, `+0x04` object table, `+0x0C` per-object descriptor blob, `+0x18` own descriptor, `+0x1C` data block, `+0x34` control table, `+0x3C` external block, `+0x58` control count, `+0x60` per-object code pointer — but the field holding the procedure→code mapping did not resolve to a usable address. **Function → VB6-object attribution therefore remains UNKNOWN**, while **function → SQL statement** is now fully established.
