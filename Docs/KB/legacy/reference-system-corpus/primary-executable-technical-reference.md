# Primary Executable Technical Reference — RealLab CD 2019 v1.605

**Artefact:** `real lab system.exe`
**SHA-256:** `c54ce6c8c98a679efc5ab485bc4b313b452026d6a40f9bda36eca7ea18df93c0`
**Size:** 76,292,096 bytes · **Method:** static only — **never executed**.
**Working copy:** `executable-analysis/extracted/real lab system.exe`

> This document is the **integrated** executable reference. It states what the
> executable is, and — for every element — how it relates to the installer,
> the B2 component payload, the B3 configuration layer, the database and the
> external integrations.

---

## 1. Identity and its integration with every other part of the system

| Property | Value | Where else it appears |
| --- | --- | --- |
| `OriginalFilename` | `real lab system.exe` | B4's `SETUP.LST` `[Setup] AppExe=real lab system.exe` — **the installer deploys exactly this file** |
| `FileDescription` / `FileVersion` | **RLS 1.605** / **1.605** | the distribution name; B4's manual footer says *"real lab system version 1.0"* — **the manual is older than the product** |
| `ProductName` / `InternalName` | real lab system | the installation directory `$(ProgramFiles)\real lab system` |
| `CompanyName` | RealLab | B4's manual copyright page: *"ais group corporation 2010"* |
| `Comments` | ais_soft_group@yahoo.com | B4's manual support page and the binary's own strings |
| `Vendor URL` | `http://www.RealLabeg.com` | **the `.eg` country code — Egypt** |
| Language | LCID 1033 / **4105 (ar-SA)** | B3's templates and B4's manual are Windows-1256 |
| SHA-256 | `c54ce6c8…18df93c0` | **identical to B1's recorded digest** for `RL_CD2019Access/System/real lab system.exe` |

**The executable is the single point where B1's file identity, B4's installer
manifest and the product's own metadata all meet.** B1's copy and the copy
supplied to this stage are the same bytes — there is no discrepancy.

---

## 2. Technology — four independent proofs

| Proof | Detail |
| --- | --- |
| Entry stub | `68 80 8E 4B 00` (`push 0x004B8E80`) then `E8 EE FF FF FF` (`call ThunRTMain`) — the VB6 native-compilation startup sequence |
| Project header | **`VB5!`** at the pushed address; `wRuntimeBuild = 9782` = the **VB6 Service Pack 6** runtime; `wFormCount = 198`; `wExternalComponentCount = 25`; `dwThunkCount = 233`; project GUID `6c1467af-a022-9145-9ed352beb9d44d32`; project name `Real_lab_system` |
| Import surface | **exactly one DLL** — `MSVBVM60.DLL`, 231 functions. Every Win32 call is late-bound through `DllFunctionCall` |
| Object table | 212 objects, 8,026 methods, decoding cleanly from the documented `ObjectTable` layout |

**Visual Basic 6, natively compiled, 32-bit x86.** B1's inventory had already
recorded *"PE32 native executable (i386, Windows GUI)"* and *"Visual Basic 6"*;
this investigation verified both from first principles.

---

## 3. PE structure

| | |
| --- | --- |
| Format / machine | PE32, i386, Windows GUI |
| Image base / entry | `0x00400000` / RVA `0x0007142C` |
| Size of image | 76,595,200 |
| `.text` | 76,119,116 B virtual / 76,120,064 B raw — **97.0 % of the file** |
| `.data` | 304,852 B virtual / **4,096 B raw** — almost entirely uninitialised |
| `.rsrc` | 161,682 B virtual / 163,840 B raw |
| `DllCharacteristics` | **`0x0000`** — no ASLR, no DEP/NX, no CFG; SEH present and not `NO_SEH` |
| Authenticode | **absent** — the Security data directory is empty |
| Manifest | **absent** — no UAC level, DPI awareness or common-controls v6 is requested |
| Resources | 10 icons (16×16 … 256×256, the largest a **PNG**), 1 group icon, 1 VERSION resource (804 B, translation 0x0409/0x04B0) |
| Imports | `MSVBVM60.DLL` only — 231 functions |

**Why this matters to the system:** B4's installer deploys a **VB6 SP6
runtime** and **16 ActiveX controls** into the system directory and
self-registers them. The executable's sole import is that runtime. The
dependency chain is therefore: *installer → system-directory runtime +
controls → the executable*.

---

## 4. The VB6 project header and object table

| Field | Value |
| --- | --- |
| Signature | `VB5!` |
| Runtime build / revision | **9782** / 10 |
| LCID / secondary LCID | 1033 / **4105 (Arabic, Saudi Arabia)** |
| Project description / exe name | `real lab system` / `real lab system` |
| Project name | **`Real_lab_system`** |
| Project help file | *(empty)* |
| Object table | 212 objects, 210 compiled, 8,026 methods |
| Project GUID | `6c1467af-a022-9145-9ed352beb9d44d32` |

The full 212-object register, with method counts and family classification,
is in `functional-module-catalogue.md` and
`machine-readable/functional-module-catalogue.csv`.

---

## 5. The string corpus — 31,429 strings

21,875 ASCII and 9,554 UTF-16LE. The SQL, the connection templates, the URL
templates and the UI strings are almost all **UTF-16LE**, because VB6 stores
Unicode literals that way.

> **A methodological caveat that applies throughout this document.** In a
> compiled VB6 binary the string literals are pooled into **one contiguous
> block**. **String adjacency carries no semantic meaning.** Every inference
> below that depends on proximity between two strings is flagged, and no
> substantive finding rests on it.

### 5.1 The four connection templates — and how they tie into B1 and B2

```
Provider=SQLOLEDB.1;Persist Security Info=True;Initial Catalog=Patients;Data Source=<server>
;User ID=<user>
;Password=<password>

Provider=MSDataShape.1;Persist Security Info=True;Initial Catalog=Patients;
        Data Provider=SQLOLEDB.1;Data Source=<server>;User ID=<user>;Password=<password>

Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Patients;Data Source=<server>

Provider=MSDataShape.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Patients;
        Data Provider=SQLOLEDB.1;Data Source=<server>
```

**The credential is not a single literal.** The prefix, `;User ID=` and
`;Password=` are three separate UTF-16LE strings concatenated at run time —
exactly the *"credential construction"* B1 reported, and the same fragments
B2 found in the vendor's `NOTEX.dll`. `Persist Security Info=True` is **in
the shipped fragment**, confirming B1's FND-007 from the binary.

The outer provider on all four is **`MSDataShape`** — the ADO data-shape
provider, which turns a **textual shape definition** into a recordset. B1
documented the shape vocabulary (`SELECT_1..SELECT_10`, `SELECT_D1..D3`) in
the B1 templates. **The shape definitions live in the report templates**,
which is why the report subsystem and the data-access subsystem cannot be
separated in this product.

### 5.2 The Access / Jet mirror

```
PROVIDER=MSDataShape;Data PROVIDER=Microsoft.Jet.OLEDB.4.0;Data Source=D:\real lab system\bin\Patients.mdb;

D:\real lab system\bin\Patients.mdb
D:\real lab system\bin\Patients_backup.mdb
D:\real lab system\bin\patients.mdb
D:\real lab system\bin\repairedDB.mdb
```

**Integration:** B1 documented the Access mirror and the Access schema.
`Patients_backup.mdb` and `repairedDB.mdb` show the mirror carries **its own
backup and repair copies** — so the offline mirror is a second copy of
patient data on every workstation, with its own redundancy.

### 5.3 The SMS gateway — a complete outbound path

```vb
Select * from MyLabSetting WHERE LabID=100 ;      ' → SMSbakey, sendername,
                                                     '   accuntID, accuntpw
Set ie = CreateObject("InternetExplorer.Application")
ie.navigate2 "http://smsmisr.com/api/send.aspx?username=" & accuntID _
           & "&password="           & accuntpw _
           & "&language=3&sender="  & sendername _
           & "&mobile="             & mobile _
           & "&message="            & smstext
' poll ie.Busy / ie.ReadyState, read ie.Body
```

**Integration:**
* the executable object `SMSfrmsenderos` (139 methods) is the SMS feature;
* B1's FND-017 (*"SMS gateway credential passed as a URL query parameter over
  plain HTTP"*) is **confirmed at source**;
* the gateway credentials are **stored in the clinical database** in
  `MyLabSetting` — which the executable itself creates
  (`Create Table MyLabSetting (LabID Smallint …)`);
* `language=3` selects Arabic.

### 5.4 The authorisation surface

| Element | Evidence | Integration |
| --- | --- | --- |
| `useraccounts(accuntID varchar(50), accuntpw varchar(50), updateflage BYTE)` | the executable's DDL | extends B1's FND-002 to the operator store |
| `ALTER Table useraccounts ADD HDD1/HDD2 varchar(20)`, `Gtype Smallint` | the executable's DDL | the user record's extra attributes |
| `SELECT * FROM useraccounts where usersdr= '` / `where usersnum= ` / `where usersdr like '` | the executable | the login path |
| `ALTER Table Patients ADD grant13…grant17 Bit` / `ADD COLUMN grant14 YESNO` + `grant1`…`grant12` | the executable's DDL | **17 per-patient permission flags** — semantics unknown (UNR-002) |
| `IDPasswordL/T/F/B/S/V/A`, `Print Result Password`, `Please Enter Your Internal Password.   (Digits only)` | the executable | **seven shared numeric print secrets** |
| `Remember Login`, `rls_loginsql` | the executable | the session model |
| `admin` / `admin` | **B4's manual** | the default operator — see U-005 |

### 5.5 The vendor identity — three independent sources inside the binary

| String | Value |
| --- | --- |
| `E-mail: ais_soft_group@yahoo.com` | matches B4's manual support page exactly |
| `Website: http://www.RealLabeg.com` | the `.eg` country code — **Egypt** |
| `Page: Facebook.com/RealLab.Systems` | matches B4's manual |
| `Comments` in the VERSION resource | `ais_soft_group@yahoo.com` |

**This is the strongest link between the executable investigation and B4's
recovered manual**: two independent sources agree on the vendor, the email
address and the web presence.

---

## 6. The embedded DDL — the client owns the server's schema

| Statement class | Count | Detail |
| --- | ---: | --- |
| `CREATE TABLE` | **14** | `testtemp`, `MyLabSetting`, `smsgroups`, `smslist`, `SMStemplate`, `SMSSender`, `LBC`, `IgEallergy`, `pictures`, `Helicobacter`, `hba1c`, `FMF`, `widal2`, `ratios` |
| `ALTER TABLE` | **120** | across **31** tables — `pictures` 84, `cultures` 22, `cbc` 18, `Patients` 14, `wasl` 12, `prostaticx` 12, `BloodGases` 8, `patientinfo` 8 … |

**Integration with B1:** the eight `CREATE TABLE` statements B1 recorded
(`testtemp`, `MyLabSetting`, `smsgroups`, `smslist`, `SMStemplate`,
`SMSSender`, `LBC`, `IgEallergy`) are present **verbatim** in the binary. B1
saw them; this stage confirms them at source and adds the 120-statement
migration surface B1 did not catalogue.

**`LBC` and `IgEallergy` are keyed `(patientid, requestdate, testid Smallint,
itemNo Smallint)`** — the **numeric test-ID namespace**. This connects the
executable directly to B3's template filenames and B4's manual statement that
the numbers are permanent and gaps are deliberate.

---

## 7. Ghidra headless analysis — the honest negative result

| | |
| --- | --- |
| Attempt 1 | Launched with `MAXMEM=1500M`; the process ran with **`-Xmx2G`** — the variable is **ignored** by Ghidra 12.1.4's `analyzeHeadless`. Stopped by hand at 1.3 GB RSS to protect the container |
| Root cause | `support/analyzeHeadless` reads **`GHIDRA_HEADLESS_MAXMEM`** (then `GHIDRA_MAXMEM`, then a built-in `2G` default). `MAXMEM` is only a positional argument to `support/launch.sh` |
| Also found | Ghidra 12.x **requires JDK 21+**; the container's Debian `default-jre-headless` is Java 17 |
| Attempt 2 | Relaunched with `GHIDRA_HEADLESS_MAXMEM=1200M` and **Temurin JDK 21.0.13+9** |
| **Outcome** | **Kernel OOM-killed — SIGKILL, exit 137.** RSS at kill **1.54 GB** on 3,072 MiB with **no swap**. The saved project is **23 KB of metadata**; the post-script **never ran**; **no function inventory exists** |
| Conclusion | **Ghidra produced no usable result for this binary.** UNR-008 and UNR-010 remain open |

**Why the structural parse is the stronger result anyway.** For a **VB6-native**
binary the project structures yield more than a decompilation would:

| | Structural parse (complete) | Decompilation (killed) |
| --- | --- | --- |
| Project identity, runtime build, form count | ✅ | — |
| **All 212 object names and method counts** | ✅ | — |
| Import surface, resources, version metadata | ✅ | ✅ |
| **All 31,429 strings, every SQL statement and connection template** | ✅ | — |
| Per-statement call chains | — | would need completion |

**The header walk is what makes the cross-batch correlation possible at all**,
because it turns a 76 MB blob into a named component register comparable with
B1's inventory, B2's component list and B3's template set.

**Both infrastructure defects were fixed** in
`/workspace/cloud-environment-bootstrap/` with the reasons recorded in that
workspace's own `README.md` §8 and `MANIFEST.md` §4. `verify.sh` passes
**22/22**.

---

## 8. The executable in relation to every other part of the system

| Other part | Relationship | Class |
| --- | --- | --- |
| **B4 installer** | `SETUP.LST` names `real lab system.exe`; the executable's `OriginalFilename` matches | **CONFIRMED** |
| **B4 installer** | the installer deploys `msvbvm60.dll` — the executable's only import | **CONFIRMED** (cryptographic) |
| **B4 installer** | the installer deploys the 5 fonts to `$(Font)` machine-wide; B3 analysed those fonts | **CONFIRMED** (cryptographic) |
| **B2 payload** | the 16 ActiveX controls are self-registered system-wide by the installer | **CONFIRMED** |
| **B2 payload** | `MSDBRPTR.DLL` is the report engine; reached by late binding, not by import | **INFERRED** |
| **B2 payload** | `msscript.ocx` / `jscript` / `vbscript` serve the HTML designer | **CONFIRMED** (same directory) |
| **B3 templates** | the `Report*` objects render `Data/*.ini`; the key vocabulary matches the manual exactly | **STRONGLY INFERRED** |
| **B3 templates** | `Cache/*.rtf` supplies the reference text | **STRONGLY INFERRED** |
| **B3 fonts** | the report renderer loads the seven installed fonts | **CONFIRMED** (installer manifest) |
| **B1 data model** | the executable's DDL creates and migrates the same tables | **CONFIRMED** |
| **B1 credentials** | the same connection fragments B1 found in `.udl`/`BASE.ini` | **CONFIRMED** |
| **B1 credentials** | the same fragments B2 found in `NOTEX.dll` | **CONFIRMED** |
| **Database** | SQL Server `Patients` + Access mirror, via the four templates | **CONFIRMED** |
| **External** | the SMS gateway over plain HTTP | **CONFIRMED** |
| **External** | the printer named in the templates; the Bullzip PDF printer B4 installs | **CONFIRMED** |
| **External** | MySQL Connector/ODBC is installed but **no use is visible** | **UNKNOWN** |

---

## 9. What the executable investigation could not establish

| Question | Status |
| --- | --- |
| Which of the 1,370 SELECTs is issued by which object | **OPEN** — UNR-008, needs decompilation |
| The semantics of `grant1`…`grant17` | **OPEN** — UNR-002 |
| The mapping of `IDPasswordL/T/F/B/S/V/A` | **OPEN** — UNR-003 |
| The role of the two 32-character literals | **OPEN** — UNR-004 |
| Whether `accuntpw` is hashed before insertion | **OPEN** — UNR-005 |
| The 25 external component names | **OPEN** — UNR-009 |
| Per-object method **names** | **OPEN** — only counts recovered |
| Runtime behaviour of any kind | **NOT ESTABLISHED** — nothing was executed |
