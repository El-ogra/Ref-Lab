# Database forensics on the real shipped MDB files

**Method:** Microsoft Access Database Engine OLE DB (`Microsoft.ACE.OLEDB.16.0`, fallback `12.0`) for schema, column metadata and row counts; the ADOX catalog object for index and primary-key definitions; direct binary hashing before any provider opened a file. **Read-only throughout** — no file was written, and the SHA-256 of each database was taken *before* the provider took its handle.

**Scripts:** `scripts/Dump-MdbForensics.ps1`, `scripts/Query-Mdb.ps1`.
**Machine-readable output:** `evidence/mdb-forensics/<database>/` — `dbinfo.json`, `tables.json`, `columns.json`, `indexes.json`, `primarykeys.json`, `foreignkeys.json`, `rowcounts.json`, `msysobjects.json`, `msyscolumns.json`, `msysindexes.json`, `msysrelationships.json`, `adox_indexes.tsv`, `errors.txt`, `counts.txt`, `catalogue.txt`.

---

## 1. File manifest

| File | Bytes | SHA-256 |
|---|---:|---|
| `bin\Patients.mdb` | 2,682,880 | `98D51F5294496E03F352FF329F099572915CCB2E670C1F143D6BA368EEE898A3` |
| `bin\Patients_backup.mdb` | 2,146,304 | `798D5B57858D626815AB104B80147D4CF53ED0578B8AF2A1BDD97BC62B3E34D8` |
| `bin\Branch_Data.mdb` | 970,752 | `F6D6D5E25C4E0B0DB1C9FEA156E0CF12C32AE2F6DCD18D80F1C8859E9CA1ABD9` |
| `System\Branch_Data.mdb` | 966,656 | `E6C3953EBBE7B39E58DF8628A371FD7B77BDE8BAFE42EB251BCD895169C4C37D` |
| `bin\data.mdb` | 266,240 | `84840CDA3AA8333BBD9E04631E20F2ECB7329EF9CA588329E05629C3E7512902` |

---

## 2. Shape comparison — prior claims re-tested

| Database | Tables | Columns | Indexes | PK | Populated tables | Prior claim | Status |
|---|---:|---:|---:|---:|---:|---|---|
| `bin\Patients.mdb` | **103** | **1,682** | **68** | **5** | 41 of 103 | 103 / 1,682 | **VERIFIED exactly** |
| `bin\Patients_backup.mdb` | 90 | 1,381 | 28 | 0 | — | 90 / 1,381 | **VERIFIED** |
| `bin\Branch_Data.mdb` | 49 | 969 | 9 | 0 | **0 of 49** | 49 / 969 / 0 rows | **VERIFIED** |
| `System\Branch_Data.mdb` | 49 | 960 | 9 | 0 | template | — | second branch template |
| `bin\data.mdb` | 3 | 34 | 4 | 0 | **0 of 3** | — | `history`, `patientinfo`, `wasl` |

**CORRECTION:** the prior knowledge base reports *"no declared PK/FK/CHECK/DEFAULT/indexes"*. That was an artefact of reading DDL dumps. The real `Patients.mdb` carries **68 indexes and 5 declared primary keys**.

---

## 3. Declared primary keys (ADOX, VERIFIED)

```
instrument     PK=[instrumentid]
pricemenu      PK=[pricelistname]
sharekat       PK=[sherkaid]
testtable      PK=[testcode]
testtemp       PK=[testcode]
```

Unique but not primary: `barcodes(barcodeid)`, `patients(patientusid)`.

Secondary indexes: `patientid` across 12 result tables (`anyreport`, `BONEMAR`, `CSFT`, `hba1c`, `Helicobacter`, `pictures`, `postcoitalt`, `ratios`, `renalfailure`, `testtemp`, `TUBERCULINT`, `ZN`); `testcode` in `anyreport`, `pictures`, `wasl`; `subgroupcode` in `testtemp`, `wasl`; `arrangnum` in `testtemp`; `sherkaid` in `sharekat`, `sharekathesab`; `history(selector, labid)`; `insulin(id)`; `MyLabSetting(accuntID, LabID, phonenum, SMSbakey)`; `pictures(numsx1…numsx15, numsxt1…numsxt15)`; `CSFT(chlorid)`; `ZN(Numberbacilli)`.

No foreign keys are declared anywhere — referential integrity is enforced by the application and by the manual 64-table `DELETE FROM … WHERE patientid` cascade.

---

## 4. Row counts — `Patients.mdb`

| Table | Rows | Table | Rows | Table | Rows |
|---|---:|---|---:|---|---:|
| `normalrange` | **1,152** | `anyreport` | 15 | `customgrouptable` | 5 |
| `abbreviations` | 650 | `titless` | 15 | `Labbranch` | 4 |
| `antibiotic` | 612 | `culturesec` | 12 | `comments` | 4 |
| **`testtable`** | **609** | `loggroup` | 8 | `history` | 3 |
| `converts` | 167 | `SMStemplate` | 8 | `Helicobacter` | 3 |
| `insulin` | 113 | `SMSSender` | 8 | **`patients`** | **2** |
| `antibio` | 95 | `pricelist` | 6 | `pricemenu` | 2 |
| `testunits` | 70 | `pictures` | 6 | `ratios` | 2 |
| `customtesttable` | 54 | `MyLabSetting` | 5 | `testname` / `smslist` | 2 each |
| `subgrouptable` | 48 | `constanttable` | 2 | `testtemp`, `Paste Errors`, `reportview`, `hba1c`, `ESR`, `smsgroups`, `TORCH` | 1 each |

**62 of 103 tables are empty.** `patientinfo`, `patienttest`, `patienttestgroup`, `payment`, `patienttotal`, `instrument`, `noteic` are among the empty ones.

---

## 5. `normalrange` — correction of the prior figures

| Dimension | Prior claim | **Real file** |
|---|---|---|
| Rows | 852 | **1,152** |
| Sex split | M 422 / F 421 | **Male 570 / Female 568 / All 10 / empty 4** |
| `REFRANGE` | (not counted) | **empty 797 · `'0'` 345 · `'1'` 10 · `'2'` 0** |

The absence of any `REFRANGE='2'` row is **verified** and is what makes the binary's `REFRANGE='2'` queries (§8 of `03_SQL_CORPUS_AND_CODE_BINDING.md`) point at data that the shipped mirror does not contain.

---

## 6. The operator/account table `patients` — 33 columns

> Only the **schema** is reported. No account name, password, hash or security word is reproduced anywhere in this output.

`branche`, `breaktime`, `breakyorn`, `endwork`, **`grant1` … `grant17`** (17 flags), `khasmlimit`, `lastvisit`, `patientad`, `patientus`, `patientuscap`, `patientusid`, `patientusname`, **`patientword`**, **`SecWord`**, `showtip`, `startwork`, `tipnum`.

**CORRECTION:** the prior knowledge base records `grant1..grant12`. The real table has **17** grant flags. The binary's own DDL confirms it:

```
0x2dc360  ALTER Table Patients ADD grant13 Bit
0x2dc4a4  ALTER Table Patients ADD grant17 Bit
0x2dc4f4  ALTER Table Patients ADD COLUMN grant13 YESNO
0x2dc690  ALTER Table Patients ADD COLUMN grant17 YESNO
0x2dc300  ALTER Table Patients ADD SecWord varchar(10)
0x2dc758  ALTER Table Patients ADD COLUMN SecWord varchar(10)
```

**Password handling.** `patientword` is compared **in cleartext inside a SQL predicate**:

```
0x2f0038  ' and patientword= '
0x2f01d0  ' and patientword='
```

`SecWord` is the secondary security word (DDL `varchar(10)`), referenced as the column `secWord` at `0x2ef6f8`. Additional password UI strings: *Please Enter Your Internal Password. (Digits only)*, *Internal Password*, *No Password Entered.*, *You must enter a Password.*, *Wrong Password Entered.*, *Enter The Correct Password.*, *Print Result Password*, `&password=`.

**`IDPasswordL/T/F/B/S/V/A`** — seven consecutive ANSI names at file offsets `0x2A5F50`–`0x2A5FF8` — are **control names on the internal-ID/password form**, not credential columns; no `IDPassword*` column exists in any shipped database.

---

## 7. Column types (OLE DB codes, VERIFIED)

| Column | OLE DB type | Interpretation |
|---|---|---|
| `patientinfo.patientid` | 130 | `DBTYPE_WSTR` — Unicode string, 13 characters per the DDL |
| `patients.grant1` … `grant17` | 11 | `DBTYPE_BOOL` — Boolean permission flags |
| `patients.patientword` | 130 | Unicode string — the operator password |
| `patients.SecWord` | 130 | Unicode string, `varchar(10)` per DDL |
| `patients.patientusname` | 130 | Unicode string — login name |
| `patients.patientusid` | 3 | `DBTYPE_I4` — numeric account id |
| `patients.khasmlimit`, `tipnum` | 3 | `DBTYPE_I4` |
| `patientinfo.totalprice`, `totalpricedisc`, `discount`, `discount100` | 5 | `DBTYPE_R8` — double |
| `patienttest.testcode` | 2 | `DBTYPE_I2` — smallint |
| `patienttest.resultt1` … `resultt4`, `RESULTT` | 130 | Unicode string — result value |
| `normalrange.REFRANGE`, `normalrange.sex` | 130 | Unicode string |

`discount100` is a **double**, stored alongside a separate `discount` and distinct from the applied discount amount `totalpricedisc`. That a "100" suffix denotes a percentage remains **INFERRED**; the arithmetic lives in VB code and is not present as a literal.

---

## 8. `patientinfo` — 39 columns (prior count VERIFIED)

`barcode`, `completes`, `discount`, `discount100`, `editdate`, `editdate2`, `editnum`, `edituser2`, `elbakyma3mal` (balance), `elmadfoo3` (paid), `email`, `gehaName`, `kawmy`, `labid`, `moreinfo`, `moreinfo2`, `mowzaf`, `outdate`, `patientaddress`, `patientage`, `patientageunit`, `patientid`, `patientnamear`, `patientphone`, `patientsefa`, `patientsex`, `patienttel`, `PatRPW`, `PBirthDay`, `PFax`, `pricemethod`, `recepnote`, `referredbyname`, `referredbysefa`, `requestdate`, `SMSMob`, `timein`, `totalprice`, `totalpricedisc`.

`patienttest` — 46 columns: `arrangnum`, `commentx`, `editdate`, `edituser`, `give`, `highlimit`, `historyname`, `loggroup`, `lowlimit`, `nohighexe`, `nolowexe`, `normalrang`, `otherlabname`, `patientid`, `prevdate`, `prevuser`, `printdate`, `printnum`, `printuser`, `printwith`, `requestdate`, `RESULTCASEPRINT`, `resultfinsh`, `resultprint`, `resultreview`, `RESULTT`, `resultt1`–`resultt4`, `send`, `sentout`, `subgroupcode`, `Taketime`, `taslimdate`, `taslimuser`, `testabrev`, `testcode`, `testname`, `testprice`, `testpriceout`, `testpriceoutmadfo`, `testsimp`, `testtime`, `testunit`, `Timeout`.

`testtable` — 38 columns including `testcode` (PK), `subgroupcode`, `waslname`/`waslname2` (the panel name), `testorno`, `patientprice`, `labprice`, `otherlabprice`, `REFRANGE`, `printwith`, `seereport`, `normaltype`, `branch`, `deleteprotect`, `testcodebill`.

---

## 9. Catalogue data (non-identifying)

**`subgrouptable` — 48 rows** (`subgroupcode`, `subgroupname`, `arname`, `deleteprotect`): 1 BLOOD GLUCOSE · 2 KIDNEY FUNCTIONS · 3 LIVER FUNCTIONS · 4 LIPIDS PROFILE · 5 URINE EXAMINATION · 6 STOOL EXAMINATION · 7 Autoimmune diseases · 8 BLOOD GROUP · 9 ESR · 10 CULTURE AND SENSITIVITY · 11 BLOOD PICTURE · 12 SERODIAGNOSIS · 13 Hepatitis Markers · 14 TOXOPLASMOSOIS · 15 HEMATOLOGY · 16 Thyroid Hormones · 17 Antiphospholipid antibody syndrome · 18 Cardiac Markers · 19 TORCH Screen · 20 Coagulation Profile · 21 Pancreas Hormones · 22 Separate · 23 Semen Examination · 24 Tumors Markers · 25 Serum Electrolytes · 26 Urine Electrolytes · 27 CSF Electrolytes · 28 Vitamins · 29 Trace Elements · 30 Iron Tests · 31 Pregnancy Tests · 32 Fertlity Hormones · 33 Suprarenal Hormones · 34 VIROLOGY · 35 MOLECULAR BIOLOGY · 37 Drug Monitoring · 38 Drugs of Abuse · 39 Coagulation Factors · 40 Immunoglobulins · 41 Blood Gases · 42 Hemoglobin electrophoresis · 43 Protein electrophoresis · 44 Aminogram · 45 WBCs function · 47 Renal Failure · 48 Diabetes profile · 49 Allergy Screen · 50 CYTOPATHOLOGY.

**`customgrouptable` — 5 rows:** CHECKUP 1 … CHECKUP 5 (codes 1, 2, 3, 4, 7).

**`customtesttable` — 54 rows** giving exact package membership and price, e.g. CHECKUP 1 = testcode 4 (Urine, 4) · 5 (Stool, 4) · 8 (SGPT/ALT, 10) · 9 (SGOT/AST, 10) · 1 (F.Bl.G., 5) · 15 (Bilirubin Total, 10) · 11 (Creatinine, 10) · 56 (CBC, 20).

**`constanttable` — 2 rows:** `PTCONS` = 12, 35, 1.33, `2CVJW`, 0, '', 4021, 94 · `CBCCONS` = 0, 6.25, 6.75, 7.5, 8.25, 100, `BD111`.

**`pricelist` — 6 rows:** price lists *price list 1*, *price list 2*, *seed*.

**`testtemp` — 1 row:** testcode 70 / Blood Gases with `RESULTT = 'See Report'` and `testprice = 175` — direct evidence that a **composite test carries a sentinel result on the order row while its analytes live in the per-discipline profile table**.

---

## 10. Patient-data exposure: NONE

`patientinfo`, `patienttest`, `patienttestgroup`, `payment`, `patienttotal`, `instrument`, `noteic` contain **zero rows**; `patients` (operator accounts) contains 2 rows; every other populated table holds catalogue or configuration content. The shipped `Patients.mdb` is a **template**, not a live patient database, so no patient-identifying value existed to expose. Operator credential values were never read into any artefact in this pass.

Arabic-text columns (for example `testtable.testnamear`) are stored in a Windows Arabic code page; the ACE provider returns them mis-decoded when read with the default 1252 mapping. This is a codepage issue in the *reader*, not in the data.

---

## 11. Exact blockers

| Attempted access | Result |
|---|---|
| `SELECT … FROM MSysObjects` | `Record(s) cannot be read; no read permission on 'MSysObjects'.` |
| `SELECT … FROM MSysQueries` | `Record(s) cannot be read; no read permission on 'MSysQueries'.` |
| `SELECT … FROM MSysRelationships` | `Record(s) cannot be read; no read permission on 'MSysRelationships'.` |
| `SELECT … FROM MSysColumns` | `The Microsoft Access database engine cannot find the input table or query 'MSysColumns'.` — the table is not present in these Jet-4 files |
| `SELECT … FROM MSysIndexes` | likewise not present |
| `GetOleDbSchemaTable(PrimaryKeys)` / `(ForeignKeys)` | provider rejects the call; index and primary-key metadata was instead read through the **ADOX catalog**, which succeeded |

**Consequence:** Access-specific column properties (defaults, required flags, validation rules) and Access relationships cannot be read from these files. **Stored QueryDefs are absent** from all five databases — the application executes SQL directly rather than against saved Access queries — which is consistent with the 1,758-literal SQL corpus and with the absence of any literal `INSERT`/`UPDATE`.
