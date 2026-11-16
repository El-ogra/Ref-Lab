# Complete Database Schema — REAL LAB SYSTEM v1.605 (V-CD-2019)

## Evidence basis and scope

This document reconstructs the **shipped `Patients.mdb`** from the authoritative
MDB-tools schema dump `v2019-Patients.mdb.sql` (47,464 B), cross-checked against
`schema-Patients.sql` / `v1-Patients.mdb.sql` (42,934 B, the v1.00 schema) and the
restated counts in `Version_Identity_and_Scope.md` §3.3 and
`Data_Model_and_Workflow_Entities_V2.md` §2.1. **No live `.mdb` was opened.**

| Property | Value | Source |
|---|---:|---|
| Tables | **103** | `v2019-Patients.mdb.sql` — 103 `CREATE TABLE` statements |
| Columns | **1,682** | parsed column lines in `v2019-Patients.mdb.sql` |
| Declared `NOT NULL` columns | 132 | `NOT NULL` tokens in `v2019-Patients.mdb.sql` |
| Declared PRIMARY / FOREIGN / UNIQUE / CHECK / DEFAULT constraints | 0 | keyword search of all DDL evidence files |
| Declared indexes (`CREATE INDEX`) | 0 | keyword search of `v2019-Patients.mdb.sql`, `ddl-all.txt`, `ddl-evidence__[analysis_BATCH-04_sql_v1605].txt` |

**Physical Jet 4 type families** in the 1,682 shipped columns (parsed from
`v2019-Patients.mdb.sql`):

| Jet type family | Columns |
|---|---:|
| `Text` | 1269 |
| `Memo/Hyperlink` | 99 |
| `Boolean` | 80 |
| `DateTime` | 71 |
| `Integer` | 62 |
| `Long Integer` | 51 |
| `Double` | 46 |
| `Byte` | 3 |
| `Single` | 1 |

> **CONFLICTED (prior reports).** `00-baseline-summary.md` states "103 tables ·
> 1,617 columns" and "+10 tables · +90 columns". The primary dump
> `v2019-Patients.mdb.sql` yields **1,682** columns and a direct v1→v2019 diff of
> **+155**; `Version_Identity_and_Scope.md` §3.3 and
> `Data_Model_and_Workflow_Entities_V2.md` §2.1 independently restate +155. This
> document follows the primary dump. `Business_Rules_V2.md` /
> `Functional_Specification_V2.md` describe a **17-new-table** set from the embedded
> DDL; the shipped-dump diff is **10** new tables (see `Tables_And_Columns.md`).

## Keys and constraints

The dump declares **no** `PRIMARY KEY`, `FOREIGN KEY`, `UNIQUE`, `CHECK`, `DEFAULT`
or `CREATE INDEX` clause. Candidate keys below are therefore **INFERRED from naming
and SQL usage, not declared**: `patientinfo.patientid`, `patienttest.patientid`,
`testtable.testcode`, `normalrange.normalid`, `patients.patientusid`,
`Labbranch.branchid`. Jet 4 does not enforce FKs; the application enforces
relationships through SQL joins and `SHAPE … RELATE` commands
(see `Relationships.md`, `Constraints_And_Indexes.md`).

## Tables by domain

Each entry lists **every column in the shipped dump** with its Jet type;
`NOT NULL` is shown only where the dump declares it.

### Patients, orders and result workflow (core transactional) (11 tables)

#### `barcodes` — 2 columns

`barcode` Text (50) NOT NULL; `barcodeid` Long Integer

#### `history` — 25 columns

`selector` Integer; `labid` Text (50); `requestdate` DateTime; `column1` Text (25); `column2` Text (25); `column3` Text (25); `column4` Text (25); `column5` Text (25); `column6` Text (25); `column7` Text (25); `column8` Text (25); `column9` Text (25); `column10` Text (25); `column11` Text (25); `column12` Text (25); `column13` Text (25); `column14` Text (25); `column15` Text (25); `column16` Text (25); `column17` Text (25); `column18` Text (25); `column19` Text (25); `column20` Text (25); `printflag` Boolean NOT NULL; `noofcol` Integer

#### `Paste Errors` — 6 columns — **NEW in v1.605**

`Field0` Memo/Hyperlink (255); `Field1` Memo/Hyperlink (255); `Field2` Memo/Hyperlink (255); `Field3` Memo/Hyperlink (255); `Field4` Memo/Hyperlink (255); `Field5` Memo/Hyperlink (255)

#### `patientinfo` — 39 columns

`patientsefa` Text (20); `patientnamear` Text (40); `patientsex` Text (7); `patientaddress` Text (70); `patientphone` Text (12); `PFax` Text (15); `PatRPW` Text (20); `SMSMob` Text (15); `requestdate` DateTime; `outdate` DateTime; `timein` Text (10); `referredbysefa` Text (20); `referredbyname` Text (40); `gehaName` Text (50); `pricemethod` Text (10); `totalprice` Double; `patientageunit` Text (5); `elmadfoo3` Double; `elbakyma3mal` Double; `totalpricedisc` Double; `patientid` Text (13); `labid` Text (13); `patientage` Double; `PBirthDay` DateTime; `discount100` Double; `discount` Double; `mowzaf` Text (40); `editdate` Text (40); `completes` Text (1); `recepnote` Text (100); `moreinfo` Text (22); `barcode` Text (25); `patienttel` Text (12); `email` Text (40); `kawmy` Text (14); `edituser2` Text (40); `editdate2` Text (40); `editnum` Integer; `moreinfo2` Text (30)

#### `patienttest` — 46 columns

`requestdate` DateTime; `patientid` Text (13); `testcode` Integer; `testsimp` Text (50); `subgroupcode` Integer; `testtime` Integer; `testprice` Double; `printwith` Boolean NOT NULL; `resultfinsh` Boolean NOT NULL; `resultprint` Boolean NOT NULL; `testpriceout` Double; `RESULTCASEPRINT` Text (15); `RESULTT` Text (50); `resultt1` Text (50); `resultt2` Text (50); `resultt3` Text (50); `resultt4` Text (50); `arrangnum` Integer; `give` Boolean NOT NULL; `edituser` Text (40); `printuser` Text (40); `editdate` Text (40); `printdate` Text (40); `printnum` Double; `prevuser` Text (40); `prevdate` Text (40); `taslimuser` Text (40); `taslimdate` Text (40); `loggroup` Text (50); `resultreview` Boolean NOT NULL; `sentout` Boolean NOT NULL; `otherlabname` Text (40); `testpriceoutmadfo` Double; `send` Boolean NOT NULL; `commentx` Memo/Hyperlink (255); `testabrev` Text (50); `testname` Text (50); `historyname` Text (50); `normalrang` Text (255); `testunit` Text (15); `lowlimit` Double; `highlimit` Double; `nolowexe` Text (20); `nohighexe` Text (20); `Timeout` Text (10); `Taketime` Integer

#### `patienttestgroup` — 46 columns — **NEW in v1.605**

`requestdate` DateTime; `patientid` Text (13); `maintestcode` Integer; `testcode` Integer; `testsimp` Text (50); `subgroupcode` Integer; `testtime` Integer; `printwith` Boolean NOT NULL; `resultfinsh` Boolean NOT NULL; `resultprint` Boolean NOT NULL; `testpriceout` Double; `RESULTCASEPRINT` Text (15); `RESULTT` Text (50); `resultt1` Text (50); `resultt2` Text (50); `resultt3` Text (50); `resultt4` Text (50); `arrangnum` Integer; `give` Boolean NOT NULL; `edituser` Text (40); `printuser` Text (40); `editdate` Text (40); `printdate` Text (40); `printnum` Double; `prevuser` Text (40); `prevdate` Text (40); `taslimuser` Text (40); `taslimdate` Text (40); `loggroup` Text (50); `resultreview` Boolean NOT NULL; `sentout` Boolean NOT NULL; `otherlabname` Text (40); `testpriceoutmadfo` Double; `send` Boolean NOT NULL; `commentx` Memo/Hyperlink (255); `testabrev` Text (50); `testname` Text (50); `historyname` Text (50); `normalrang` Text (255); `testunit` Text (15); `lowlimit` Double; `highlimit` Double; `nolowexe` Text (20); `nohighexe` Text (20); `Timeout` Text (10); `Taketime` Integer

#### `patienttotal` — 15 columns

`patientid` Text (13) NOT NULL; `patientname` Text (50); `requestdate` DateTime; `t01` Text (50); `t02` Text (50); `t03` Text (50); `t04` Text (50); `t05` Text (50); `t06` Text (50); `t07` Text (50); `t08` Text (50); `t09` Text (50); `t10` Text (50); `t11` Text (50); `t12` Text (50)

#### `phone` — 19 columns

`contactid` Long Integer; `sefa` Text (20); `name` Text (50); `phonenumber` Text (20); `company` Text (50); `birthday` DateTime; `mail1a` Text (40); `mail1b` Text (20); `mail2a` Text (40); `mail2b` Text (20); `city` Text (30); `fax` Text (20); `tel1` Text (20); `tel2` Text (20); `webs` Text (50); `land1` Text (20); `land2` Text (20); `note` Text (100); `address` Text (100)

#### `reportview` — 42 columns

`stripnum` Integer; `stripdiscrp` Memo/Hyperlink (255); `strip1` Memo/Hyperlink (255); `strip2` Memo/Hyperlink (255); `strip3` Memo/Hyperlink (255); `strip4` Memo/Hyperlink (255); `strip5` Memo/Hyperlink (255); `strip6` Memo/Hyperlink (255); `strip7` Memo/Hyperlink (255); `strip8` Memo/Hyperlink (255); `strip9` Memo/Hyperlink (255); `strip10` Memo/Hyperlink (255); `strip11` Memo/Hyperlink (255); `strip12` Memo/Hyperlink (255); `strip13` Memo/Hyperlink (255); `strip14` Memo/Hyperlink (255); `strip15` Memo/Hyperlink (255); `strip16` Memo/Hyperlink (255); `strip17` Memo/Hyperlink (255); `strip18` Memo/Hyperlink (255); `strip19` Memo/Hyperlink (255); `strip20` Memo/Hyperlink (255); `strip21` Memo/Hyperlink (255); `strip22` Memo/Hyperlink (255); `strip23` Memo/Hyperlink (255); `strip24` Memo/Hyperlink (255); `strip25` Memo/Hyperlink (255); `strip26` Memo/Hyperlink (255); `strip27` Memo/Hyperlink (255); `strip28` Memo/Hyperlink (255); `strip29` Memo/Hyperlink (255); `strip30` Memo/Hyperlink (255); `stripb1` Integer; `stripb2` Integer; `stripb3` Integer; `stripb4` Integer; `stripb5` Integer; `stripb6` Integer; `stripb7` Integer; `stripb8` Integer; `stripb9` Integer; `stripb10` Integer

#### `testtemp` — 28 columns — **NEW in v1.605**

`patientid` Text (13); `requestdate` DateTime; `testcode` Long Integer; `subgroupcode` Long Integer; `testsimp` Text (50); `loggroup` Text (50); `arrangnum` Long Integer; `RESULTT` Text (50); `testprice` Double; `sentout` Boolean NOT NULL; `testpriceout` Double; `otherlabname` Text (40); `testtime` Integer; `printwith` Boolean NOT NULL; `testabrev` Text (50); `testname` Text (50); `historyname` Text (50); `testunit` Text (15); `lowlimit` Double; `highlimit` Double; `nolowexe` Text (20); `nohighexe` Text (20); `detelethis` Long Integer; `resultfinsh` Boolean NOT NULL; `resultreview` Boolean NOT NULL; `resultprint` Boolean NOT NULL; `give` Boolean NOT NULL; `normalrang` Memo/Hyperlink (255)

#### `wasl` — 12 columns

`subgroupcode` Long Integer; `testcode` Long Integer; `waslname` Text (255); `waslname2` Text (255); `testprice` Double; `testno` Integer; `resultt` Text (50); `testunit` Text (50); `RESULTCASEPRINT` Text (50); `normalrang` Memo/Hyperlink (255); `testcodex` Text (50); `subgroupname` Text (50)

### Operators, access and audit (3 tables)

#### `loggroup` — 2 columns

`logid` Long Integer; `logname` Text (50) NOT NULL

#### `noteic` — 6 columns

`noteid` Long Integer; `noteuser` Text (50); `notedate` DateTime; `noteend` Boolean NOT NULL; `fromuser` Text (50); `thenote` Text (250)

#### `patients` — 33 columns

`patientusid` Long Integer; `patientusname` Text (40); `lastvisit` Text (40); `patientuscap` Text (40); `patientword` Text (40); `patientad` Boolean NOT NULL; `patientus` Boolean NOT NULL; `grant1` Boolean NOT NULL; `grant2` Boolean NOT NULL; `grant3` Boolean NOT NULL; `grant4` Boolean NOT NULL; `grant5` Boolean NOT NULL; `grant6` Boolean NOT NULL; `grant7` Boolean NOT NULL; `grant8` Boolean NOT NULL; `grant9` Boolean NOT NULL; `grant10` Boolean NOT NULL; `grant11` Boolean NOT NULL; `grant12` Boolean NOT NULL; `showtip` Boolean NOT NULL; `tipnum` Long Integer; `startwork` Text (15); `endwork` Text (15); `breakyorn` Boolean NOT NULL; `breaktime` Text (15); `khasmlimit` Long Integer; `branche` Integer; `SecWord` Text (10); `grant13` Boolean NOT NULL; `grant14` Boolean NOT NULL; `grant15` Boolean NOT NULL; `grant16` Boolean NOT NULL; `grant17` Boolean NOT NULL

### Test catalogue, reference knowledge and lookups (17 tables)

#### `abbreviations` — 3 columns

`abbid` Long Integer; `abbre` Text (30); `mean` Text (100)

#### `antibio` — 2 columns

`antisymbol` Text (20) NOT NULL; `scientificname` Text (50)

#### `antibiotic` — 25 columns

`antiid` Long Integer; `culutrenum` Integer; `culutretype` Text (50); `antisymbol` Text (50); `scientificname` Text (50); `anticomname1` Text (50); `anticomname2` Text (50); `anticomname3` Text (50); `anticomname4` Text (50); `anticomname5` Text (50); `anticomname6` Text (50); `anticomname7` Text (50); `anticomname8` Text (50); `anticomprint1` Boolean NOT NULL; `anticomprint2` Boolean NOT NULL; `anticomprint3` Boolean NOT NULL; `anticomprint4` Boolean NOT NULL; `anticomprint5` Boolean NOT NULL; `anticomprint6` Boolean NOT NULL; `anticomprint7` Boolean NOT NULL; `anticomprint8` Boolean NOT NULL; `prgnancy` Boolean NOT NULL; `child` Boolean NOT NULL; `Sensitivity` Text (50); `Sensitiveunit` Text (10)

#### `comments` — 5 columns

`commentid` Long Integer; `testcode` Integer; `subgroupcode` Integer; `testsimp` Text (50); `comments` Memo/Hyperlink (255)

#### `constanttable` — 8 columns

`constantdiscrp` Text (50); `constant1` Text (50); `constant2` Text (50); `constant3` Text (50); `constant4` Text (50); `constant5` Text (50); `constant6` Text (50); `constant7` Text (50)

#### `converts` — 6 columns

`testsimp` Text (100); `Funit` Text (50); `Sunit` Text (50); `Factors` Double; `Ffix` Integer; `Sfix` Integer

#### `customgrouptable` — 2 columns

`customgroupcode` Long Integer; `customgroupname` Text (50)

#### `customtesttable` — 5 columns

`testnum` Long Integer; `customgroupcode` Integer NOT NULL; `testcode` Integer NOT NULL; `testsimp` Text (50); `patientprice` Double

#### `insulin` — 2 columns — **NEW in v1.605**

`id` Integer NOT NULL; `insulin` Text (100) NOT NULL

#### `normalrange` — 18 columns

`normalid` Long Integer; `testcode` Long Integer; `testsimp` Text (50); `subgroupcode` Integer; `sex` Text (10); `firstold` Double; `secondold` Double; `ageunit` Text (10); `pregsmok` Single; `normalrang` Text (255); `testunit` Text (15); `lowlimit` Double; `highlimit` Double; `nolowexe` Text (20); `nohighexe` Text (20); `lowcomment` Memo/Hyperlink (255); `highcomment` Memo/Hyperlink (255); `REFRANGE` Text (1)

#### `PTC` — 3 columns

`control` Double NOT NULL; `concent` Double NOT NULL; `ConstID` Long Integer

#### `subgrouptable` — 4 columns

`subgroupcode` Long Integer; `subgroupname` Text (50); `arname` Text (50); `deleteprotect` Boolean NOT NULL

#### `testlearn` — 3 columns

`testcode` Integer NOT NULL; `testsimp` Text (50); `testlearnid` Text (50) NOT NULL

#### `testname` — 8 columns

`groupcode` Long Integer; `subgroupcode` Long Integer; `arrangnum` Long Integer; `testcode` Long Integer; `testname` Text (50); `testsimp` Text (50); `testunit` Text (50); `num` Long Integer

#### `testtable` — 38 columns

`testcode` Long Integer; `testcodebill` Text (13); `arrangnum` Integer; `subgroupcode` Integer; `testsimp` Text (50); `testabrev` Text (50); `testname` Text (50); `waslname` Text (50); `waslname2` Text (50); `historyname` Text (50); `barcodesymb` Text (50); `testnamear` Text (50); `subgroupname` Text (40); `loggroup` Text (50); `active` Boolean NOT NULL; `inotherlab` Boolean NOT NULL; `otherlabname` Text (50); `otherlabprice` Double; `patientprice` Double; `labprice` Double; `anyqus` Boolean NOT NULL; `thequse` Text (150); `testdiscr` Text (100); `testeffect` Text (100); `testusage` Text (100); `typeofsample` Text (100); `deleteprotect` Boolean NOT NULL; `testtime` Integer; `printwith` Boolean NOT NULL; `barcode1` Text (50); `barcode2` Text (50); `barcode3` Text (50); `branch` Text (50); `seereport` Text (50); `normaltype` Text (50); `addorno` Boolean NOT NULL; `testorno` Boolean NOT NULL; `REFRANGE` Text (1)

#### `testunits` — 2 columns

`unitsid` Long Integer; `unitsname` Text (15)

#### `titless` — 3 columns

`titlenum` Long Integer; `titlesex` Text (10); `titlename` Text (20)

### Billing, finance and referring parties (8 tables)

#### `Gehats` — 21 columns

`gehahid` Long Integer; `gehahsefa` Text (20); `gehahname` Text (40); `masolsefa` Text (10); `masolname` Text (40); `madina` Text (40); `addressxc` Text (100); `phone` Text (12); `mobil` Text (12); `fax` Text (12); `mail` Text (50); `web` Text (50); `flose` Boolean NOT NULL; `khasm` Double; `omola` Double; `morsel` Integer; `pricemethod` Text (20); `masolphone` Text (12); `printorno` Boolean NOT NULL; `ReferredPW` Text (20); `ReferredID` Long Integer

#### `payment` — 10 columns

`patientid` Text (13); `paiddate` DateTime; `type` Integer; `gehah` Text (50); `testsimp` Text (50); `money` Double; `mowzaf` Text (40); `editdate` Text (40); `comment` Text (100); `paymentnum` Long Integer

#### `pricelist` — 4 columns

`pricenum` Long Integer; `testsimp` Text (50); `pricelistname` Text (50); `testprice` Double

#### `pricemenu` — 1 columns

`pricelistname` Text (50)

#### `sarfeda3` — 3 columns

`sarfnum` Long Integer; `type` Long Integer; `sarfname` Text (50)

#### `sharekat` — 6 columns

`sherkaid` Long Integer; `sherkaname` Text (50); `sherkaadre` Text (120); `sherkaphone1` Text (12); `sherkaphone2` Text (12); `sherkanote` Text (250)

#### `sharekathesab` — 7 columns

`taamelid` Long Integer; `sherkaid` Long Integer; `sherkaname` Text (50); `taameldate` DateTime; `wareed` Double; `saader` Double; `taamelnote` Memo/Hyperlink (255)

#### `shopping` — 7 columns

`matlobid` Long Integer; `elmatlob` Text (100); `notes` Text (100); `mowzaf` Text (40); `talbdate` DateTime; `tamtalb` Boolean NOT NULL; `tasleem` Boolean NOT NULL

### Laboratory configuration and infrastructure (5 tables)

#### `daytip` — 2 columns

`daytipnum` Integer NOT NULL; `daytip2` Text (255)

#### `instrument` — 20 columns

`instrumentid` Long Integer; `name` Text (50); `sn` Text (50); `masna3` Text (50); `manshaa` Text (50); `wazifa` Text (100); `molahazat1` Text (100); `dateshera` DateTime; `datesalem` DateTime; `dateshoghl` DateTime; `sherka` Text (50); `phone1` Text (20); `phone2` Text (20); `fax` Text (20); `address` Text (100); `molahazat2` Text (100); `email` Text (50); `website` Text (50); `masool` Text (50); `phone3` Text (20)

#### `Labbranch` — 2 columns

`branchid` Long Integer; `branchname` Text (50) NOT NULL

#### `MyLabSetting` — 20 columns — **NEW in v1.605**

`LabID` Integer; `sendername` Text (15); `accuntID` Text (50); `accuntpw` Text (50); `updateflage` Byte; `Hourflag` Integer; `SMSNo` Integer; `totalsms` Double; `SMSSend` Double; `SMSbakey` Double; `phoneNo1` Text (15); `phoneNo2` Text (15); `phoneNo3` Text (15); `phoneNo4` Text (15); `phoneNo5` Text (15); `phoneNo6` Text (15); `phonenum` Integer; `Timess` Text (100); `FridaySend` Byte; `shortsms` Byte

#### `pictures` — 73 columns

`patientid` Text (13); `requestdate` DateTime; `testcode` Long Integer; `pictpix1` Text (50); `pictpix2` Text (50); `pictpix3` Text (50); `pictpix4` Text (50); `Label1V` Text (50); `Label1T` Text (75); `Label2V` Text (50); `Label2T` Text (75); `Label3V` Text (50); `Label3T` Text (75); `Label4V` Text (50); `Label4T` Text (75); `Label5V` Text (50); `Label5T` Text (75); `Label6V` Text (50); `Label6T` Text (75); `Label7V` Text (50); `Label7T` Text (75); `Label8V` Text (50); `Label8T` Text (75); `Label9V` Text (50); `Label9T` Text (75); `Label10V` Text (50); `Label10T` Text (75); `Label11V` Text (50); `Label11T` Text (75); `Label12V` Text (50); `Label12T` Text (75); `numsx1` Text (50); `numsx2` Text (50); `numsx3` Text (50); `numsx4` Text (50); `numsx5` Text (50); `numsx6` Text (50); `numsx7` Text (50); `numsx8` Text (50); `numsx9` Text (50); `numsx10` Text (50); `numsx11` Text (50); `numsx12` Text (50); `numsx13` Text (50); `numsx14` Text (50); `numsx15` Text (50); `shapesx1` Text (50); `shapesx2` Text (50); `shapesx3` Text (50); `shapesx4` Text (50); `shapesx5` Text (50); `shapesx6` Text (50); `shapesx7` Text (50); `shapesx8` Text (50); `shapesx9` Text (50); `shapesx10` Text (50); `shapesx11` Text (50); `shapesx12` Text (50); `numsxt1` Text (5); `numsxt2` Text (5); `numsxt3` Text (5); `numsxt4` Text (5); `numsxt5` Text (5); `numsxt6` Text (5); `numsxt7` Text (5); `numsxt8` Text (5); `numsxt9` Text (5); `numsxt10` Text (5); `numsxt11` Text (5); `numsxt12` Text (5); `numsxt13` Text (5); `numsxt14` Text (5); `numsxt15` Text (5)

### Messaging / SMS (new in v1.605) (4 tables)

#### `smsgroups` — 3 columns — **NEW in v1.605**

`ID` Long Integer; `grouptype` Integer; `smsgroup` Text (50)

#### `smslist` — 6 columns — **NEW in v1.605**

`ID` Long Integer; `smstype` Integer; `smsphone` Text (20); `smsname` Text (50); `smsgroup` Text (50); `send` Integer

#### `SMSSender` — 8 columns — **NEW in v1.605**

`SMSautonum` Long Integer; `username` Text (50); `patientid` Text (13); `PatDrName` Text (60); `requestdate` DateTime; `smsMobile` Text (100); `SMSLength` Integer; `SMStext` Text (255)

#### `SMStemplate` — 2 columns — **NEW in v1.605**

`templatenum` Long Integer; `smstext` Text (255)

### Per-test profile result tables (one wide table per test panel) (53 tables)

#### `ABO` — 8 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `abo` Text (30); `rhfactor` Text (30); `direct` Text (30); `indirect` Text (30); `rhtitre` Text (30); `comment` Text (254)

#### `Abuse` — 12 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `Alcohol` Text (10); `Cannabinoid` Text (10); `Opitate` Text (10); `Cocaine` Text (10); `Barbtiurate` Text (10); `Benzodiazipine` Text (10); `Amphetamine` Text (10); `comment` Memo/Hyperlink (255); `tramadol` Text (10); `ecstasy` Text (10)

#### `BILH` — 9 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `bilh` Text (30); `fasc` Text (30); `amob` Text (30); `hydat` Text (30); `bilhagurine` Text (30); `bilhagserum` Text (30); `comment` Memo/Hyperlink (255)

#### `BloodGases` — 10 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `pH` Text (10); `PaO2` Text (10); `PaCO2` Text (10); `O2` Text (10); `HCO3` Text (10); `SO2` Text (10); `BaseExcess` Text (10); `comment` Text (250)

#### `BONEMAR` — 23 columns

`patientid` Text (13); `requestdate` DateTime; `T01` Text (50); `T02` Text (50); `T03` Text (50); `T04` Text (50); `T05` Text (50); `T06` Text (50); `T07` Text (50); `T08` Text (50); `T09` Text (50); `T10` Text (50); `T11` Text (50); `T12` Text (50); `T13` Text (50); `T14` Text (50); `T15` Text (50); `T16` Text (50); `T17` Text (50); `T18` Text (50); `T19` Text (50); `T20` Text (50); `comment` Memo/Hyperlink (255)

#### `cardiac` — 9 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `ldh` Text (10); `cktotal` Text (10); `ckmb` Text (10); `Troponin` Text (10); `TroponinT` Text (10); `Homocystein` Text (10); `comment` Memo/Hyperlink (255)

#### `cbc` — 29 columns

`patientid` Text (13); `requestdate` DateTime; `hgb` Text (10); `rbc` Text (10); `hct` Text (10); `plt` Text (10); `hgbper` Text (10); `mcv` Text (10); `mch` Text (10); `mchc` Text (10); `pct` Text (10); `mpv` Text (10); `wbc` Text (10); `neut` Text (10); `lymph` Text (10); `mono` Text (10); `eosino` Text (10); `baso` Text (10); `othercell` Text (10); `comment` Memo/Hyperlink (255); `RDW` Text (10); `PDW` Text (10); `seg` Text (10); `bandx` Text (10); `Juvenile` Text (10); `Myelocytes` Text (10); `Promyelocyte` Text (10); `Blast` Text (10); `NRBCWBC` Text (10)

#### `CoagFactors` — 11 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `FactVIII` Text (10); `FactIX` Text (10); `FactXI` Text (10); `FactXII` Text (10); `FactII` Text (10); `FactV` Text (10); `FactVII` Text (10); `FactX` Text (10); `comment` Text (250)

#### `crossmat` — 84 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `patabo` Text (5); `patrh` Text (10); `donname1` Text (40); `donname2` Text (40); `donname3` Text (40); `donname4` Text (40); `donname5` Text (40); `donname6` Text (40); `donname7` Text (40); `donname8` Text (40); `donname9` Text (40); `donname10` Text (40); `donabo1` Text (5); `donabo2` Text (5); `donabo3` Text (5); `donabo4` Text (5); `donabo5` Text (5); `donabo6` Text (5); `donabo7` Text (5); `donabo8` Text (5); `donabo9` Text (5); `donabo10` Text (5); `donrh1` Text (10); `donrh2` Text (10); `donrh3` Text (10); `donrh4` Text (10); `donrh5` Text (10); `donrh6` Text (10); `donrh7` Text (10); `donrh8` Text (10); `donrh9` Text (10); `donrh10` Text (10); `doncom1` Text (20); `doncom2` Text (20); `doncom3` Text (20); `doncom4` Text (20); `doncom5` Text (20); `doncom6` Text (20); `doncom7` Text (20); `doncom8` Text (20); `doncom9` Text (20); `doncom10` Text (20); `donhcv1` Text (20); `donhcv2` Text (20); `donhcv3` Text (20); `donhcv4` Text (20); `donhcv5` Text (20); `donhcv6` Text (20); `donhcv7` Text (20); `donhcv8` Text (20); `donhcv9` Text (20); `donhcv10` Text (20); `donhbv1` Text (20); `donhbv2` Text (20); `donhbv3` Text (20); `donhbv4` Text (20); `donhbv5` Text (20); `donhbv6` Text (20); `donhbv7` Text (20); `donhbv8` Text (20); `donhbv9` Text (20); `donhbv10` Text (20); `donhiv1` Text (20); `donhiv2` Text (20); `donhiv3` Text (20); `donhiv4` Text (20); `donhiv5` Text (20); `donhiv6` Text (20); `donhiv7` Text (20); `donhiv8` Text (20); `donhiv9` Text (20); `donhiv10` Text (20); `dontf2` Boolean NOT NULL; `dontf3` Boolean NOT NULL; `dontf4` Boolean NOT NULL; `dontf5` Boolean NOT NULL; `dontf6` Boolean NOT NULL; `dontf7` Boolean NOT NULL; `dontf8` Boolean NOT NULL; `dontf9` Boolean NOT NULL; `dontf10` Boolean NOT NULL; `comment` Memo/Hyperlink (255)

#### `CRP` — 11 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `aso` Text (30); `crp` Text (30); `rf` Text (30); `Rosewaller` Text (30); `Monospot` Text (30); `VDRL` Text (30); `rpr` Text (30); `THPA` Text (30); `comment` Memo/Hyperlink (255)

#### `CSFT` — 24 columns

`patientid` Text (13); `requestdate` DateTime; `materi` Text (50); `color` Text (50); `aspect` Text (50); `deposit` Text (50); `Viscos` Text (50); `clotting` Text (50); `PH` Text (50); `SG` Text (50); `gluc` Text (50); `prot` Text (50); `chlorid` Text (50); `ldh` Text (50); `wbc` Text (50); `wbc2` Text (10); `rbc` Text (50); `rbc2` Text (10); `epithi` Text (50); `other` Text (50); `gramdfilm` Text (50); `znfilm` Text (50); `Lishman` Text (50); `comment` Memo/Hyperlink (255)

#### `cultureanti` — 12 columns

`antinum` Long Integer; `patientid` Text (13); `culutrenum` Integer; `antiid` Integer; `atisens` Text (2); `arrangnum` Integer; `antisymbol` Text (50); `scientificname` Text (50); `anticomname` Text (150); `Sensitivityvalue` Text (10); `Sensitiveunit` Text (10); `Sensitivity` Text (50)

#### `cultures` — 35 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `cultid` Integer NOT NULL; `sample` Text (50); `organism` Text (80); `organism2` Text (80); `condition` Text (50); `condition2` Text (50); `colony` Text (50); `reactions` Text (50); `pus1` Text (10); `pus2` Text (10); `rbc1` Text (10); `rbc2` Text (10); `epeth` Text (50); `fungi` Text (50); `cryst` Text (50); `bact` Text (50); `other` Text (50); `few1` Text (10); `few2` Text (10); `few3` Text (10); `few4` Text (10); `direct1` Text (50); `direct2` Text (50); `comment` Memo/Hyperlink (255); `other1` Text (50); `other2` Text (50); `other3` Text (50); `other4` Text (50); `few5` Text (10); `few6` Text (10); `few7` Text (10); `few8` Text (10); `organism3` Text (80)

#### `culturesec` — 3 columns

`secnum` Integer; `secabre` Text (2); `secname` Text (50)

#### `Drugs` — 14 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `Digoxin` Text (10); `Tegretol` Text (10); `Phenytoin` Text (10); `Depakene` Text (10); `Phenobarbitone` Text (10); `Tacrolimus` Text (10); `Theophylline` Text (10); `Cyclosporin` Text (10); `Methotrexate` Text (10); `Lithium` Text (10); `Vancomycin` Text (10); `comment` Text (250)

#### `Electrolytes` — 20 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `scatot` Text (10); `scaion` Text (10); `sph` Text (10); `sna` Text (10); `sk` Text (10); `scl` Text (10); `smg` Text (10); `sosmolality` Text (51); `una` Text (10); `uk` Text (10); `ucl` Text (10); `uca` Text (10); `uph` Text (10); `umg` Text (10); `uosmolality` Text (10); `csfmg` Text (10); `csfcl` Text (10); `comment` Text (250)

#### `ESR` — 5 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `Fhour` Text (10); `Shour` Text (10); `comment` Text (250)

#### `Fertlity` — 17 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `FSH` Text (10); `LH` Text (10); `Prolactin` Text (10); `TestostT` Text (10); `TestostF` Text (10); `DHT` Text (10); `Progesterone` Text (10); `E1` Text (10); `E2` Text (10); `E3` Text (10); `DHEAS` Text (10); `DHEA` Text (10); `SHBG` Text (10); `Androstendione` Text (10); `comment` Text (250)

#### `GLUCOSETOLERANCE` — 25 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `given` Text (50); `FBG` Text (10); `FBGG` Text (10); `FBGA` Text (10); `bg30` Text (10); `bg30g` Text (10); `bg30a` Text (10); `bg1` Text (10); `bg1g` Text (10); `bg1a` Text (10); `bg2` Text (10); `bg2g` Text (10); `bg2a` Text (10); `bg3` Text (10); `bg3g` Text (10); `bg3a` Text (10); `bg15` Text (10); `bg15g` Text (10); `bg15a` Text (10); `bg25` Text (10); `bg25g` Text (10); `bg25a` Text (10); `comment` Memo/Hyperlink (255)

#### `hba1c` — 5 columns

`patientid` Text (13); `requestdate` DateTime; `hba1c` Double; `eag` Double; `comment` Memo/Hyperlink (255)

#### `Helicobacter` — 6 columns — **NEW in v1.605**

`patientid` Text (13); `requestdate` DateTime; `hpigi` Text (50); `hpigm` Text (50); `hpag` Text (50); `comment` Memo/Hyperlink (255)

#### `hepatitis` — 20 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `havigm` Text (30); `havigg` Text (30); `hbsag` Text (30); `hbsab` Text (30); `hbeag` Text (30); `hbeab` Text (30); `hbcigm` Text (30); `hbcigg` Text (30); `hcvigg` Text (30); `hcvigm` Text (30); `hdvtotal` Text (30); `hdvigm` Text (30); `hdvigg` Text (30); `hevigg` Text (30); `hevigm` Text (30); `hcvigg2` Text (30); `hiv` Text (30); `comment` Text (250)

#### `Hgbectrophoresis` — 8 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `A1` Text (10); `A2` Text (10); `F` Text (10); `C` Text (10); `S` Text (10); `Comment` Text (250)

#### `hormones` — 16 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `t3` Text (10); `t4` Text (10); `freet3` Text (10); `freet4` Text (10); `tsh` Text (10); `fti` Text (10); `t3up` Text (10); `pbi` Text (10); `ThyroidAuto` Text (10); `AntiMicrosomal` Text (10); `PTH` Text (10); `Calcitonin` Text (10); `Thyroglobulin` Text (10); `comment` Text (250)

#### `Immunoglobulin` — 10 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `IgG` Text (10); `IgA` Text (10); `IgM` Text (10); `IgE` Text (10); `IgD` Text (10); `C3` Text (10); `C4` Text (10); `comment` Text (250)

#### `IMMUNOLOGY` — 19 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `ANA` Text (25); `DNA` Text (25); `LKM` Text (25); `SSA` Text (25); `SSB` Text (25); `AntSmith` Text (25); `AntCentromere` Text (25); `ANCA` Text (25); `RNP` Text (25); `AntScl` Text (25); `ASMA` Text (25); `AMA` Text (25); `AntKeratin` Text (25); `AntReticulin` Text (25); `AntSperm` Text (25); `AntJo1` Text (25); `comment` Text (250)

#### `iron` — 9 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `iron` Text (10); `tibc` Text (10); `uibc` Text (10); `Saturation` Text (10); `STransferrin` Text (10); `Ferritin` Text (10); `comment` Text (250)

#### `kidney` — 15 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `creat` Text (10); `uric` Text (10); `urea` Text (10); `bun` Text (10); `UVolume` Text (10); `Creatu` Text (10); `CreatClear` Text (10); `Microalbu` Text (10); `Proteinu` Text (10); `Bencejp` Text (10); `Stonecal` Text (10); `egfr` Text (10); `comment` Text (250)

#### `Lipids` — 11 columns

`patientid` Text (13); `requestdate` DateTime; `cholest` Text (10); `trig` Text (10); `hdl` Text (10); `ldl` Text (10); `risk1` Text (10); `risk2` Text (10); `vldl` Text (10); `totallipids` Text (10); `comment` Text (250)

#### `liver` — 15 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `sgpt` Text (10); `sgot` Text (10); `ASTALT` Text (10); `Alp` Text (10); `ggt` Text (10); `totbil` Text (10); `dirbil` Text (10); `indbil` Text (10); `tp` Text (10); `alb` Text (10); `glob` Text (10); `agratio` Text (10); `comment1` Text (250)

#### `Pancreas` — 13 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `InsulinFast` Text (10); `Insulin30` Text (10); `Insulin60` Text (10); `Insulin120` Text (10); `Insulin180` Text (10); `Cpeptidefast` Text (10); `CpeptidePP` Text (10); `Cpeptideurine` Text (10); `Glucagon` Text (10); `Gastrin` Text (10); `comment` Text (250)

#### `postcoitalt` — 17 columns

`patientid` Text (13); `requestdate` DateTime; `T01` Text (50); `T02` Text (50); `T03` Text (50); `T04` Text (50); `T05` Text (50); `T06` Text (50); `T07` Text (50); `T08` Text (50); `T09` Text (50); `T009` Text (50); `T10` Text (50); `T010` Text (50); `T11` Text (50); `T12` Text (50); `comment` Memo/Hyperlink (255)

#### `pregnancy` — 6 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `urine` Text (30); `serum` Text (30); `bhcg` Text (30); `comment` Memo/Hyperlink (255)

#### `Proelectrophoresis` — 8 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `Albumin` Text (10); `Alpha1` Text (10); `Alpha2` Text (10); `Beta` Text (10); `Gamma` Text (10); `comment` Memo/Hyperlink (255)

#### `prostaticx` — 53 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `wbc11` Text (25); `wbc12` Text (25); `wbc21` Text (25); `wbc22` Text (25); `wbc31` Text (25); `wbc32` Text (25); `rbc11` Text (25); `rbc12` Text (25); `rbc21` Text (25); `rbc22` Text (25); `rbc31` Text (25); `rbc32` Text (25); `Trichom` Text (50); `Monilia` Text (50); `Epith1` Text (50); `Epith2` Text (50); `Epith3` Text (50); `Other1` Text (50); `Other2` Text (50); `Other3` Text (50); `Other01` Text (25); `Other02` Text (25); `Other03` Text (25); `Other1t` Boolean NOT NULL; `Other2t` Boolean NOT NULL; `Other3t` Boolean NOT NULL; `Otherr1` Text (50); `Otherr2` Text (50); `Otherr3` Text (50); `Otherr01` Text (25); `Otherr02` Text (25); `Otherr03` Text (25); `Crystal1` Text (50); `Crystal2` Text (50); `Crystal01` Text (25); `Crystal02` Text (25); `Crystal1t` Boolean NOT NULL; `Crystal2t` Boolean NOT NULL; `Crystalr1` Text (50); `Crystalr2` Text (50); `Crystalr01` Text (25); `Crystalr02` Text (25); `comment` Memo/Hyperlink (255); `reactp` Text (50); `react1` Text (50); `react2` Text (50); `Crystalp` Text (50); `Crystalpr` Text (25); `Crystalp2` Text (50); `Crystalp2r` Text (25); `crystalpt` Boolean NOT NULL

#### `pt` — 16 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `bleedingm` Text (5); `bleedings` Text (5); `bleeding` Text (10); `clottingm` Text (5); `clottings` Text (5); `clotting` Text (10); `pt` Text (10); `concent` Text (10); `inr` Text (10); `ratios` Text (10); `ptt` Text (10); `fibro` Text (10); `fdp` Text (10); `comment` Memo/Hyperlink (255)

#### `ratios` — 12 columns

`patientid` Text (13); `requestdate` DateTime; `cacreat` Text (10); `ptcreat` Text (10); `uriccreat` Text (10); `creat` Text (10); `ca` Text (10); `pt` Text (10); `uric` Text (10); `comment` Memo/Hyperlink (255); `albu` Text (10); `albcreat` Text (10)

#### `renalfailure` — 11 columns

`patientid` Text (13); `requestdate` DateTime; `creatbef` Text (10); `creataft` Text (10); `ureabef` Text (10); `ureaaft` Text (10); `nare` Text (10); `kre` Text (10); `care` Text (10); `hgbre` Text (10); `comment` Memo/Hyperlink (255)

#### `semen` — 60 columns

`requestdate` DateTime; `patientid` Text (13) NOT NULL; `collection` Text (20); `period` Text (20); `timeofcollec` Text (20); `volume` Text (5); `color` Text (20); `Consistency` Text (20); `Reaction` Text (10); `Viscosity` Text (15); `Concentration` Text (5); `totalsperm` Text (10); `rbc1` Text (5); `rbc2` Text (5); `agglutination` Text (10); `Spermatogenic` Text (5); `Spermatogenic2` Text (5); `Epithelial` Text (10); `wbc1` Text (5); `wbc2` Text (5); `after30` Text (5); `after1` Text (5); `after2` Text (5); `after3` Text (5); `after4` Text (5); `Concentration2` Text (5); `fgradea` Text (5); `fgradeb` Text (5); `fgradec` Text (5); `fgraded` Text (5); `fgradeab` Text (5); `sgradea` Text (5); `sgradeb` Text (5); `sgradec` Text (5); `sgraded` Text (5); `sgradeab` Text (5); `fdead` Text (5); `sdead` Text (5); `abnormal` Text (5); `head` Text (5); `mid` Text (5); `tail` Text (5); `micro` Text (5); `comment` Memo/Hyperlink (255); `abnx1` Text (5); `abnx2` Text (5); `abnx3` Text (5); `abnx4` Text (5); `abnx5` Text (5); `abnx6` Text (5); `abnx7` Text (5); `abnx8` Text (5); `abnx9` Text (5); `abnx10` Text (5); `abnx11` Text (5); `abnx12` Text (5); `abnx13` Text (5); `fructos` Text (20); `testcode` Integer; `Liquefaction` Text (10)

#### `stone` — 19 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `type` Text (50); `shape` Text (50); `color` Text (50); `size` Text (50); `texture` Text (50); `consis` Text (50); `fibr` Text (50); `cyst` Text (50); `Ammon` Text (50); `uric` Text (50); `calc` Text (50); `oxal` Text (50); `mag` Text (50); `phos` Text (50); `Carb` Text (50); `urat` Text (50); `comment` Memo/Hyperlink (255)

#### `stool` — 61 columns

`requestdate` DateTime; `patientid` Text (13) NOT NULL; `t01` Text (50); `t02` Text (50); `t03` Text (50); `t04` Text (50); `t05` Text (50); `t06` Text (50); `t07` Text (50); `t08` Text (50); `t09` Text (50); `t10` Text (50); `t11` Text (50); `t12` Text (50); `t13` Text (50); `t14` Text (50); `t15` Text (50); `t16` Text (50); `t17` Text (50); `t18` Text (50); `t19` Text (50); `t20` Text (50); `t21` Text (50); `t22` Text (50); `t23` Text (50); `t24` Text (50); `t25` Text (50); `t26` Text (50); `t27` Text (50); `t28` Text (50); `t29` Text (50); `t30` Text (50); `t31` Text (50); `t32` Text (50); `t33` Text (50); `t34` Text (50); `t35` Text (50); `t36` Text (50); `t37` Text (50); `t38` Text (50); `t39` Text (50); `t40` Text (50); `t41` Text (50); `t42` Text (50); `t43` Text (50); `t44` Text (50); `t45` Text (50); `t46` Text (50); `t47` Text (50); `t48` Text (50); `t49` Text (50); `t50` Text (50); `t51` Text (50); `t52` Text (50); `t53` Text (50); `t54` Text (50); `t55` Text (50); `t56` Text (50); `t57` Text (50); `t58` Text (50); `tcomment` Memo/Hyperlink (255)

#### `sugar` — 12 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `FBG` Text (10); `PBG` Text (10); `RBG` Text (10); `glo1` Text (50); `glo2` Text (50); `glo3` Text (50); `act1` Text (50); `act2` Text (50); `act3` Text (50); `comment` Memo/Hyperlink (255)

#### `Suprarenal` — 16 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `ACTH9AM` Text (10); `ACTH10PM` Text (10); `Cortisol9AM` Text (10); `Cortisol10PM` Text (10); `CortisolUrine` Text (10); `CatecholUrine` Text (10); `Adrenaline` Text (10); `Noradrenaline` Text (10); `Dopamine` Text (10); `Renin` Text (10); `Aldosterone` Text (10); `Progesterone17OH` Text (10); `VMAUrine` Text (10); `comment` Memo/Hyperlink (255)

#### `tasgel` — 11 columns

`tasgelnum` Long Integer; `tasgeldate` DateTime; `username` Text (40); `startwork` Text (15); `endwork` Text (15); `startbreak` Text (15); `endbreak` Text (15); `overtsec` Integer; `delaysec` Integer; `overtime` Text (10); `delaytime` Text (10)

#### `TORCH` — 13 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `Toxoigm` Text (30); `Toxoigg` Text (30); `RubellaIgM` Text (30); `RubellaIgG` Text (30); `CMVIgM` Text (30); `CMVIgG` Text (30); `HerpesIgMI` Text (30); `HerpesIgGI` Text (30); `HerpesIgMII` Text (30); `HerpesIgGII` Text (30); `comment` Memo/Hyperlink (255)

#### `trace` — 25 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `sAluminium` Text (10); `uAluminium` Text (10); `sArsenic` Text (10); `uArsenic` Text (10); `sCadmium` Text (10); `uCadmium` Text (10); `sChromium` Text (10); `uChromium` Text (10); `sCobalt` Text (10); `uCobalt` Text (10); `sCopper` Text (10); `uCopper` Text (10); `sLead` Text (10); `uLead` Text (10); `sMercury` Text (10); `uMercury` Text (10); `sNickel` Text (10); `uNickel` Text (10); `sZinc` Text (10); `uZinc` Text (10); `sSelenium` Text (10); `uSelenium` Text (10); `comment` Memo/Hyperlink (255)

#### `TUBERCULINT` — 6 columns

`patientid` Text (13); `requestdate` DateTime; `Dose` Text (50); `Diameter` Text (50); `Resultr` Text (50); `comment` Memo/Hyperlink (255)

#### `Tumors` — 14 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `CEA` Text (10); `AFP` Text (10); `PSAT` Text (10); `PSAF` Text (10); `PSAR` Text (10); `CA15` Text (10); `CA19` Text (10); `CA125` Text (10); `SMicroglobulin` Text (10); `UMicroglobulin` Text (10); `NSE` Text (10); `comment` Memo/Hyperlink (255)

#### `urine` — 60 columns

`requestdate` DateTime; `patientid` Text (13) NOT NULL; `t01` Text (50); `t02` Text (50); `t03` Text (50); `t04` Text (50); `t05` Text (50); `t06` Text (50); `t07` Text (50); `t08` Text (50); `t09` Text (50); `t10` Text (50); `t11` Text (50); `t12` Text (50); `t13` Text (50); `t14` Text (50); `t15` Text (50); `t16` Text (50); `t17` Text (50); `t18` Text (50); `t19` Text (50); `t20` Text (50); `t21` Text (50); `t22` Text (50); `t23` Text (50); `t24` Text (50); `t25` Text (50); `t26` Text (50); `t27` Text (50); `t28` Text (50); `t29` Text (50); `t30` Text (50); `t31` Text (50); `t32` Text (50); `t33` Text (50); `t34` Text (50); `t35` Text (50); `t36` Text (50); `t37` Text (50); `t38` Text (50); `t39` Text (50); `t40` Text (50); `t41` Text (50); `t42` Text (50); `t43` Text (50); `t44` Text (50); `t45` Text (50); `t46` Text (50); `t47` Text (50); `t48` Text (50); `t49` Text (50); `t50` Text (50); `t51` Text (50); `t52` Text (50); `t53` Text (50); `t54` Text (50); `t55` Text (50); `t56` Text (50); `t57` Text (50); `tcomment` Memo/Hyperlink (255)

#### `Vitamins` — 15 columns

`patientid` Text (13) NOT NULL; `requestdate` DateTime; `VitaminA` Text (10); `VitaminD` Text (10); `VitaminD3` Text (10); `VitaminE` Text (10); `VitaminK` Text (10); `VitaminB1` Text (10); `VitaminB2` Text (10); `VitaminB6` Text (10); `VitaminB12` Text (10); `Folic` Text (10); `Biotin` Text (10); `VitaminC` Text (10); `comment` Memo/Hyperlink (255)

#### `widal` — 16 columns

`patientid` Text (13); `requestdate` DateTime; `widal1a` Text (20); `widal1b` Text (20); `widal2a` Text (20); `widal2b` Text (20); `widal3a` Text (20); `widal3b` Text (20); `widal4a` Text (20); `widal4b` Text (20); `bruc1a` Text (20); `bruc1b` Text (20); `bruc2a` Text (20); `bruc2b` Text (20); `widalresult` Text (50); `comment` Memo/Hyperlink (255)

#### `widal2` — 26 columns

`patientid` Text (13); `requestdate` DateTime; `widal1a` Text (20); `widal1b` Text (20); `widal2a` Text (20); `widal2b` Text (20); `widal3a` Text (20); `widal3b` Text (20); `widal4a` Text (20); `widal4b` Text (20); `widal5a` Text (20); `widal5b` Text (20); `widal6a` Text (20); `widal6b` Text (20); `widal7a` Text (20); `widal7b` Text (20); `widal8a` Text (20); `widal8b` Text (20); `widalresult` Text (50); `weilfelix1a` Text (20); `weilfelix1b` Text (20); `weilfelix2a` Text (20); `weilfelix2b` Text (20); `weilfelix3a` Text (20); `weilfelix3b` Text (20); `comment` Text (255)

#### `ZN` — 8 columns

`patientid` Text (13); `requestdate` DateTime; `sample` Text (50); `Specimen` Text (50); `Stainfilm` Text (50); `Numberbacilli` Text (50); `resultr` Text (50); `comment` Memo/Hyperlink (255)

### Other / ancillary (semantic assignment UNVERIFIED) (2 tables)

#### `anyreport` — 26 columns

`patientid` Text (13); `requestdate` DateTime; `testcode` Integer; `testcodex` Integer; `text1` Memo/Hyperlink (255); `text2` Memo/Hyperlink (255); `text3` Memo/Hyperlink (255); `text4` Memo/Hyperlink (255); `text5` Memo/Hyperlink (255); `text6` Memo/Hyperlink (255); `text7` Memo/Hyperlink (255); `text8` Memo/Hyperlink (255); `text9` Memo/Hyperlink (255); `text10` Memo/Hyperlink (255); `text11` Memo/Hyperlink (255); `text12` Memo/Hyperlink (255); `text13` Memo/Hyperlink (255); `text14` Memo/Hyperlink (255); `text15` Memo/Hyperlink (255); `text16` Memo/Hyperlink (255); `text17` Memo/Hyperlink (255); `text18` Memo/Hyperlink (255); `text19` Memo/Hyperlink (255); `text20` Memo/Hyperlink (255); `text21` Memo/Hyperlink (255); `text22` Memo/Hyperlink (255)

#### `seana` — 10 columns

`seanaid` Long Integer; `instrumentid` Long Integer; `gehah` Text (50); `mohands` Text (50); `seanadate` DateTime; `parts` Text (100); `price` Double; `notes` Text (100); `sabab` Text (100); `phone` Text (20)

## Separate store: `Branch_Data.mdb` (schema only)

Reconstructed from `schema-Branch_Data.sql` (= `schema.sql`, 26,262 B):
**49 tables / 969 columns**, 0 rows. It is a per-branch template, not a populated
clinical store (`db-summary.csv`; `Data_Model_and_Workflow_Entities_V2.md` §2.1).
`Version_Identity_and_Scope.md` §3.3 records that the `bin\` copy shipped with 2019
is byte-identical to v1.00, while a second `System\` copy (966,656 B) is 9 columns
narrower in `patienttest`. Two distinct templates therefore coexist — marked
CONFLICTED until the files themselves are available.

## Separate store: `data.mdb`

`v1-data.mdb.sql` and `v2019-data.mdb.sql` (1,420 B each): **3 tables / 34 columns**,
0 rows — `history`, `patientinfo`, `wasl` (`rowcounts__[analysis_BATCH-02_db_data].csv`).
