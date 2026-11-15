# Static RE findings — `real lab system.exe` (RealLab / RLS 1.605)

**Target** `F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe`
**Analysis mode** read-only static. The target was never executed, debugged, or modified. Nothing under `F:\` was written.
**Output directory** `C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\`

---

## 0. Identity verification (performed before any analysis)

| Property | Expected | Observed | Verdict |
|---|---|---|---|
| Size | 76,292,096 bytes | **76,292,096 bytes** | PASS |
| SHA-256 | `C54CE6C8C98A679EFC5AB485BC4B313B452026D6A40F9BDA36ECA7EA18DF93C0` | **`C54CE6C8C98A679EFC5AB485BC4B313B452026D6A40F9BDA36ECA7EA18DF93C0`** | PASS |

Hash computed with `Get-FileHash -Algorithm SHA256` over the whole file, and independently re-computed in Python (`hashlib.sha256`, 1 MiB chunks) while building `exe_inventory.json` — both agree.
Analysis proceeded only after both checks passed.

---

## 1. PE identity

| Field | Value | Source / file offset |
|---|---|---|
| Format | PE32, 32-bit x86 | `pe_identity.json` |
| Machine | `0x014c` `IMAGE_FILE_MACHINE_I386` | COFF header @ `0xbc` |
| Managed? | **NATIVE** — `DATA_DIRECTORY[14]` (CLR) RVA `0x0`, size `0` | Optional header @ `0xd0` |
| `e_lfanew` | `0xb8` | DOS header @ `0x0` |
| COFF header | `0xbc` (20 bytes) | |
| Optional header | `0xd0` (224 bytes, PE32) | |
| Section table | `0x1b0` (3 × 40 bytes) | |
| Linker version | **6.0** (`MajorLinkerVersion=6`, `MinorLinkerVersion=0`) | |
| PE timestamp | `1595435962` → **2020-07-22T05:59:22Z** | COFF `TimeDateStamp` @ `0xbc+4` |
| ImageBase | `0x00400000` | |
| Entry point | RVA `0x7142c`, VA `0x0047142c`, **file offset `0x7142c`** | |
| SectionAlignment / FileAlignment | `0x1000` / `0x1000` | |
| SizeOfImage | `0x048b1000` | |
| SizeOfHeaders | `0x1000` | |
| Subsystem | `2` = `IMAGE_SUBSYSTEM_WINDOWS_GUI` | |
| DllCharacteristics | `0x0` (no ASLR, no DEP, no SEH, no CFG) | |
| Checksum (header) | `0x0` (not set) | |
| OS / Subsystem version | 4.0 / 4.0 | |
| Relocations | **NONE** — base-relocation directory RVA `0x0`, size `0` | the image is bound to `0x400000` and cannot be relocated |
| Export directory | RVA `0x0` (none) | |
| Debug directory | RVA `0x0` (no PDB) | |
| Certificate directory | RVA `0x0` (unsigned) | |
| IAT | RVA `0x1000`, size `928` | |
| Import directory | RVA `0x4897d04`, size `40`, file offset `0x4897d04` | |

### Section map (file offsets)

| # | Name | File offset | Raw size | RVA | Virtual size | Characteristics | Entropy |
|---|---|---|---|---|---|---|---|
| 0 | `.text` | `0x1000` | 76,120,064 (`0x48A0000`) | `0x1000` | 76,119,116 | `0x60000020` (CODE, EXEC, READ) | 6.230 |
| 1 | `.data` | `0x4899000` | 4,096 (`0x1000`) | `0x4899000` | 304,852 | `0xc0000040` (INITIALIZED_DATA, READ, WRITE) | 0.000 |
| 2 | `.rsrc` | `0x489a000` | 163,840 (`0x28000`) | `0x48e4000` | 161,682 | `0x40000040` (INITIALIZED_DATA, READ) | 6.361 |

Note: `.text` raw offset == `.text` RVA (`0x1000`), so **file offset == RVA** for the whole code/data range; `VA = 0x400000 + RVA` and `file offset = VA - 0x400000` hold everywhere inside `.text`.

The `.text` section is 72.6 MiB — 95 % of the file. This is native-compiled VB6 code plus the VB6 literal/object pools (see §5).

---

## 2. Resources

Resource directory root: RVA `0x48e4000`, **file offset `0x489a000`**, size 161,682 bytes.
**12 resource entries in 3 types.** Nothing else exists — in particular **there is no `RT_MANIFEST`**, no `RT_DIALOG`, no `RT_STRING`, no `RT_RCDATA`.

| Type | ID | Lang | Size | File offset | SHA-256 (16) |
|---|---|---|---|---|---|
| `RT_GROUP_ICON` | 1 | NEUTRAL | 146 | `0x489a5c4` | `7e320f50e1b6a169` |
| `RT_ICON` | 30001 | NEUTRAL | 744 | `0x48c14aa` | `0f3155254f1faedc` |
| `RT_ICON` | 30002 | NEUTRAL | 296 | `0x48c1382` | `b9689c2910e13de8` |
| `RT_ICON` | 30003 | NEUTRAL | 3,752 | `0x48c04da` | `74372ebd83690d74` |
| `RT_ICON` | 30004 | NEUTRAL | 2,216 | `0x48bfc32` | `2dc5e0f5870d9434` |
| `RT_ICON` | 30005 | NEUTRAL | 1,384 | `0x48bf6ca` | `82d65e41655502c9` |
| `RT_ICON` | 30006 | NEUTRAL | 69,012 | `0x48ae936` | `4f0fea0eae8035d5` |
| `RT_ICON` | 30007 | NEUTRAL | 67,624 | `0x489e10e` | `c0b58290f3fd464a` |
| `RT_ICON` | 30008 | NEUTRAL | 9,640 | `0x489bb66` | `23e3481ff110ab77` |
| `RT_ICON` | 30009 | NEUTRAL | 4,264 | `0x489aabe` | `eb700b729088815f` |
| `RT_ICON` | 30010 | NEUTRAL | 1,128 | `0x489a656` | `372b32ebf8c81450` |
| `RT_VERSION` | 1 | **1033 (en-US)** | 804 | `0x489a2a0` | `8683fe0deee1ceb3` |

`RT_GROUP_ICON` id 1 declares 10 images: 16×16 / 32×32 / 48×48 / 128×128 / 256×256, in 4-bpp (16-colour) and 32-bpp (alpha) variants.
All twelve payloads were extracted to `...\exe\resources\` (one file per entry, named `<TYPE>_<ID>_<LANG>.<ext>`); each is hashed in `resources.json`.

**Manifest: absent.** There is no embedded application manifest, therefore no requestedExecutionLevel, no DPI-awareness declaration, no `<supportedOS>` list, and no UAC/Common-Controls declaration. This is consistent with a VB6 build that declares its controls in code.

---

## 3. Version information (`VS_VERSIONINFO`, lang 1033, at file offset `0x489a2a0`)

```
CompanyName      = RealLab
FileDescription  = RLS 1.605
FileVersion      = 1.605
ProductName      = real lab system
ProductVersion   = 1.605
InternalName     = real lab system
OriginalFilename = real lab system.exe
LegalCopyright   = RealLab
LegalTrademarks  = RealLab
Comments         = ais_soft_group@yahoo
```

`Comments` contains the developer/vendor e-mail address. Full structure in `version_info.json`.

---

## 4. Imports / dependencies

**Exactly one imported DLL: `MSVBVM60.DLL`, 231 imported functions.**
Import descriptor at file offset `0x4897d04`; IAT at RVA `0x1000` (file offset `0x1000`).

* This is the **Visual Basic 6 runtime import block** — the canonical signature of a VB6 native-compiled EXE.
* There are **no** statically linked libraries beyond the VB runtime, **no delay-loaded imports**, **no bound imports**, and **no exported symbols**.
* Win32 APIs are *not* in the import table. They are reached through VB6 `Declare` statements resolved at run time (confirmed by the `DllFunctionCall` import and by the literal pool containing `kernel32` / `user32` + API names). Recovered `Declare` names: `Sleep`, `GetTickCount`, `LoadKeyboardLayoutA`, `UnloadKeyboardLayout`, `ActivateKeyboardLayout`, `GetKeyboardLayout`, `GetKeyboardLayoutList`, `GetKeyboardLayoutNameA`, `ExitWindowsEx`, `GetPrivateProfileStringA`, `GetPrivateProfileIntA`, `WritePrivateProfileStringA`, `SendMessageA`, `DrawMenuBar`, `GetMenuItemCount`, `GetSystemMenu`, `RemoveMenu`, `EnumPrintersA`, `SetDefaultPrinterA` (file offsets `0x26c634`–`0x26f740`).
* `__vbaLateIdCall` / `__vbaLateIdSt` / `__vbaLateMemCall` / `__vbaLateMemSt` (and their `Ld`/`St`/`Ad` variants) are imported → the application performs **late-bound** calls into COM/ActiveX objects (the ADODB/ADODC/MSDataShape providers, and the third-party OCXs).
* `Zombie_GetTypeInfo`, `Zombie_GetTypeInfoCount` and the `EVENT_SINK_*` IDispatch exports are present → the EXE hosts an ActiveX event sink.

Full list of the 231 function names (with ordinals, hints and thunk RVAs) is in `imports.json`.

### Database-provider usage (observed in literals, not in the import table)

| Offset | Encoding | Literal |
|---|---|---|
| `0x272418` | UTF-16LE | `PROVIDER=MSDataShape;Data PROVIDER=Microsoft.Jet.OLEDB.4.0;Data Source=` |
| `0x2724ac` | UTF-16LE | `D:\real lab system\bin\Patients.mdb;` |
| `0x28a640` | UTF-16LE | `PROVIDER=MSDataShape;Data PROVIDER=Microsoft.Jet.OLEDB.4.0;Data Source=D:\real lab system\bin\Patients.mdb;` |
| `0x28cc8c` | UTF-16LE | `Microsoft.JET.OLEDB.4.0` |
| `0x28ccc0` | UTF-16LE | `D:\real lab system\bin\Patients_backup.mdb` |
| `0x2da75c` | UTF-16LE | `Provider=SQLOLEDB.1;Persist Security Info=True;Initial Catalog=Patients;Data Source=` |
| `0x2da928` | UTF-16LE | `Provider=MSDataShape.1;Persist Security Info=True;Initial Catalog=Patients;Data Provider=SQLOLEDB.1;Data Source=` |
| `0x2daa10` | UTF-16LE | `Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Patients;Data Source=` |
| `0x2dab0c` | UTF-16LE | `Provider=MSDataShape.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Patients;Data Provider=SQLOLEDB.1;Data Source=` |
| `0x26fb88` | UTF-16LE | `D:\real lab system\bin\Patients.mdb` |
| `0x2eeea4` | UTF-16LE | `D:\real lab system\bin\patients.mdb` |
| `0x2eef14` | UTF-16LE | `D:\real lab system\bin\repairedDB.mdb` |
| `0x2ef028` | UTF-16LE | `Patients.mdb` |
| `0x271074` | ASCII | `ADODB` |
| `0x1229d8` | ASCII | `MSADODC.OCX` |
| `0x2bfaac` | ASCII | `C:\WINDOWS\system32\MSADODC.oca` |
| `0x2bfacc` | ASCII | `MSAdodcLib` |
| `0x5b9e6a` | ASCII | `MSAdodcLib.Adodc` |

⇒ The app supports **three** back-ends for the same `Patients` database: **Jet OLEDB 4.0** (Access .mdb), **MSDataShape** (shape/rollup queries over either), and **SQL Server via SQLOLEDB.1** (`Initial Catalog=Patients`). The legacy **MSADODC** (ODBC data control) is also referenced.

### Third-party controls / OLE references

`C:\Program Files\Microsoft Visual Studio\VB98\VB6.OLB` (`0x26bb20`), `ChamaleonButton.oca` (`0x26bc50`) → control class `ChamaleonButton`, `Redsoul911XPCOmponents.oca` (`0x26c0f6`) → `Redsoul911XPControls` / `ZCRedsoulFrame` / `RedsoulFrame*`, plus controls named `RgheedButtonV2xx`, `XPButton*`, `DTPicker*`, `ChameleonBtn*`.

---

## 5. String inventory

Extraction method (no external tool needed): a whole-file sweep for ASCII runs `[\x20-\x7e]{4,}` and UTF-16LE runs `(?:[\x20-\x7e]\x00){4,}` → **369,877 raw hits**, **31,014 distinct texts**.

The 72.6 MiB `.text` section is mostly compiled x86, and compiled code bytes produce large numbers of spurious printable runs. Density analysis shows the genuine VB6 literal / object pools occupy the file range **`0x269000` – `0x1780000`** (≈ 21 MiB): inside that range UTF-16 density is ≥120 strings per 64 KiB block, whereas outside it the residue is uniform code noise (e.g. `j<hD-g`, `h4-g`, `LSVW`). Two further dense pockets were checked: `0x489xxxx` = `.rsrc` version block; nothing else.

Final retained set: **101,812 classified entries / 15,143 unique texts** (34,745 in-pool noise strings dropped; 40 out-of-pool strings kept because they matched a strong pattern).

| Category | Entries | Unique texts |
|---|---|---|
| `identifier_or_other` (control/property names, `Tahoma`, `MS Sans Serif`, `caption`, `font`, …) | 72,723 | 7,659 |
| `vb6_member_name` | 15,259 | 2,213 |
| `ui_caption_or_message` | 10,908 | 2,600 |
| `sql_statement` | 1,539 | 1,534 |
| `sql_fragment` | 757 | 742 |
| `sql_ddl` | 438 | 365 |
| `vb6_form_or_module_name` | 421 | 209 (of the 210 object names; `ais` is 3 bytes and falls below the 4-character string-extraction floor) |
| `file_path` | 342 | 333 (236 unique `.ini`) |
| `report_name` | 148 | 82 |
| `url` | 133 | 21 |
| `ole_reference` | 39 | 32 |
| `credential_like` | 29 | 25 |
| `error_or_message` | 22 | 22 |
| `db_connection_or_driver` | 20 | 19 |
| `printer_role` | 10 | 10 |
| `registry_path` | **0** | **0** |

Full inventory with file offsets and encodings: **`strings_classified.json`** (12.2 MB, `strings[]` array + `unique_text_index`).
Intermediate raw dump: `strings_raw.json`. SQL-derived artefacts: `sql_inventory.json`. Cross-reference: `db_xref.json`.

### Notable non-SQL categories

* **URLs (21 unique)** — `http://ns.adobe.com/xap/1.0/…`, `http://www.iec.ch` (`0x47b6df`), `Website: http://www.RealLabeg.com` (`0xd0f834`), `https://www.facebook.com/RealLab.Systems` (`0x2717cc`), `https://www.facebook.com/RealLab.SPRT` (`0x271910`), `www.Facebook.com/RealLab.SPRT` (`0x16f3e53`), `WWW.NATIGH.COM` (`0x2ab824`, `0x2aca24`, `0x2b1650`).
* **Registry paths: none.** No `HKEY_*` or `Software\…` literal occurs anywhere in the file — the application does not use the registry for its own configuration.
* **Config = INI files under `D:\real lab system\`** — 236 distinct `.ini` paths: `bin\note.ini`, `bin\SETING.ini`, `bin\BASE.ini`, `bin\IDCARD.INI`, `Data\MEMORY.ini`, `Data\data.ini`, `Data\TOTAL.ini`, and a per-analyte pair `<TEST>.ini` / `<TEST>_S.ini` for ~55 analytes (CBC, WIDAL, LIVER, URINE, STOOL, KIDNEY, LIPIDS, SEMEN, ABO, HEPATITIS, ESR, TORCH, CRP, PT, HORMONES, ANYREPORT, INSULIN, REPORT, IRON, BILH, DEFUALT *(sic)*, Electrolytes, CARDIAC, TUMORS, VITAMINS, TRACE, ALL, PREGNANCY, FERTLITY, SUPRARENAL, ABUSE, DRUGS, COFACTOR, IMMUNOGLOB, IMMUNOLOGY, HBELECTRO, PROTELECTRO, BLOODGAS, CROSS, SUGARCURVE, STONE, PROSTATA, CSF, ZN, TUBERCULIN, BONEMAR, POSTCOIT, RENALF, 400, WIDAL2, RATIO, Helicobacter, SEMEN5, URINEauto).
* **Error / message captions (22)** — e.g. `No Password Entered.` `0x26dad0`, `You must enter a Password.` `0x26db04`, `Wrong Password Entered.` `0x26dd5c`, `Enter The Correct Password.` `0x26dd90`, `Can't find any printer.` `0x2d7e04`, `Wrong connection` `0x26edc4`, `Modify data Connection` `0x26ed90`, `Error ... Enter the Accont ID and Password first` `0x334b78` *(sic)*, `An error has occurred, Please make sure your browser, Or select other` `0x271824`.
* **OLE references (32 unique)** — `.OLB` / `.OCA` paths listed in §4.

---

## 6. VB6 project structure (recovered directly from the file)

The VB6 structures were decoded by hand from the binary (the REA Ghidra provider does not expose a VB6-aware parser, and no VB6 decompiler was available).

### 6.1 VB header

`VB5!` signature found at **file offset `0xb8e80`** (VA `0x004b8e80`), inside `.text`.
Confirmed by the entry point: at RVA `0x7142c` (file offset `0x7142c`) the code is

```
0047142c  push  0x4b8e80        ; -> VB header
00471431  call  0x471424        ; -> ThunRTMain (MSVBVM60.DLL)
```

| Offset (rel. `0xb8e80`) | Field | Value |
|---|---|---|
| `+0x00` | `szVbMagic` | `VB5!` |
| `+0x04` | `wRuntimeBuild` | 9782 |
| `+0x22` | `wRuntimeRevision` | 10 |
| `+0x24` | `dwLCID` | `0x409` (en-US) |
| `+0x28` | `dwSecLCID` | `0x0` |
| `+0x2C` | `lpSubMain` | `0x0` (no `Sub Main`; startup form is used) |
| `+0x30` | `lpProjectData` | `0x004bd64c` |
| `+0x34` / `+0x38` | `fMdlIntCtls` / `fMdlIntCtls2` | `0x01f8ffff` / `0xffffff00` |
| `+0x40` | `dwThreadCount` | 1 |
| `+0x44` | `wFormCount` | **198** |
| `+0x46` | `wExternalCount` | **25** |
| `+0x48` | `dwThunkCount` | **233** |
| `+0x4C` | `lpGuiTable` | `0x00572554` |
| `+0x50` | `lpExternalTable` | `0x00521778` |
| `+0x54` | `lpComRegisterData` | `0x00471438` |
| `+0x58`–`+0x64` | project description / exe name / help file / name (offsets into the header blob) | `0x78`, `0x88`, `0x98`, `0x99` |
| `+0x78` | string blob | `real lab system` (×3) |
| `+0xa8` | project/App title | `Real_lab_system` |

### 6.2 Project data, object table and object descriptors

**`ProjectData`** at VA `0x004bd64c` (file offset `0xbd64c`), pointed to by the VB header `+0x30`:

| Offset | Field | Value |
|---|---|---|
| `+0x00` | `dwVersion` | `0x1f4` (500) |
| `+0x04` | `lpObjectTable` | `0x00660b98` |
| `+0x08` | `dwNull` | `0` |
| `+0x0c` | `lpCodeStart` | `0x01b41260` |
| `+0x10` | `lpCodeEnd` | `0x04c97d00` |
| `+0x14` | `dwDataSize` | `305,356` |
| `+0x18` | `lpThreadSpace` | `0x04c99008` |
| `+0x1c` | `lpVbaSeh` | `0x00470ec6` |
| `+0x20` | `lpNativeCode` | `0x04c99000` |

**`ObjectTable`** at VA `0x00660b98` (**file offset `0x260b98`**):

| Offset | Field | Value |
|---|---|---|
| `+0x04` | `lpExecProj` | `0x04c9a4b8` |
| `+0x08` | `lpProjectInfo2` | `0x01b40cac` |
| `+0x14` | `lpProjectObject` | `0x04c9a4a8` |
| `+0x18` | `uuidObject` | **`{AF67146C-22A0-4591-9ED3-52BEB9D44D32}`** (project GUID) |
| `+0x28` | `wCompileType` / `wObjectCount` | `0x000a` / **`0x00d2` = 210** |
| `+0x2c` | `wCompiledCount` / `wObjectsInUse` | `210` / `212` |
| `+0x30` | `lpObjectArray` | `0x00660bec` |
| `+0x40` | `lpszProjectName` | **`Real_lab_system`** (`0x0066aebc`) |

**`PublicObjectDescriptor[]`** at VA `0x00660bec` (**file offset `0x260bec`**), **48-byte stride**, **210 records** — exactly the `wObjectCount` of 210:

```
+0x00  lpObjectInfo     (VA of the per-object ObjectInfo structure)
+0x04  dwReserved       (0xFFFFFFFF)
+0x08  lpPublicBytes    (VA into the literal pool)
+0x0C  lpStaticBytes    (0)
+0x10  lpModulePublic   (non-zero for only 9 of the 210 records)
+0x14  lpModuleStatic   (0)
+0x18  lpszObjectName   (VA of the ANSI object name)
+0x1C  dwMethodCount    (members; 0 for the 65 report templates)
+0x20  lpMethodNames    (VA; zero-filled in the file image for most objects)
+0x24  bStaticVars      (0xFFFF)
+0x28  fObjectType      (0x18083 / 0x18001 / 0x1018083 / 0x118003)
+0x2C  dwNull
```

All 210 `lpObjectInfo` pointers resolve to a valid `ObjectInfo` structure (each contains a back-pointer to `0x00660b98`). `ObjectInfo` layout confirmed at file `0xbdb5c`: `+0x00 wRefCount/wObjectIndex` · `+0x04 lpObjectTable` · `+0x08 lpIdeData` · `+0x0c lpPrivateObject` · `+0x18 lpObject` (self) · `+0x1c lpProjectData` · `+0x34 lpIdeData3` · `+0x3c lpConstants`.

All 210 names are in `exe_inventory.json → vb6.objects` and `vb_objects.json`. First/last few:

`labmain` (71 members, code VA `0x4bdb5c`) · `labxxyyzz` (3) · `patientmain` (84) · `labdb` (24) · `frm_money` (38) · `tools` (1) · `Resultliver` (101) · `moneypaid` (18) · … · `otherfun` (2) · `ResulturineAuto` (184).

> **Note on counts:** the VB header's `wFormCount` is **198** and `wExternalCount` is **25** (198 + 25 = 223), while the object-descriptor array yields **210** records. The 12-record difference is consistent with `wExternalCount` describing something other than "remaining objects" (VB6's `wExternalCount` counts external/ActiveX references, not modules) — the 210 descriptors are the authoritative set of project objects (forms + `.bas`/`.cls` modules).
>
> The `fObjectType` field classifies them cleanly (132 + 66 + 9 + 3 = 210):
>
> | `fObjectType` | Count | Interpretation | Examples |
> |---|---|---|---|
> | `0x18083` | 132 | ordinary forms | `labmain`, `patientmain`, `frm_money`, `Resultliver`, `tools`, `addpatient` |
> | `0x1018083` | 66 | report / print templates (**0 member entries each**) | `Reporturine`, `Reportliver`, `ReportFatora`, `Reportworkp`, `Reportmony1` |
> | `0x18001` | 9 | standard modules (`.bas`) | `labdb`, `labdb1`, `BCWFonts`, `labfunction`, `printerselect`, `saveIOtemp`, `boisais`, `mInputbox`, `otherfun` |
> | `0x118003` | 3 | class modules (`.cls`) | `aaaClass`, `CRect`, `BiosData` |
>
> Member counts: min 0, max 231 (`resultIgeAllergy`), sum **8,026**; **65 objects have 0 member entries** (all of them `Report*`/`rpt*` templates whose layout lives in the binary form data rather than in the member table).

Name blob: **file offsets `0x26aebc` – `0x26b980`** (project title `Real_lab_system` at `0x26aebc`, then the 210 object names `0x26aecc` – `0x26b970`).

Semantic breakdown of the 210 object names:

* **61** `Report*` forms — `Reporturine(s)`, `Reportliver`, `Reportlivers`, `ReportFatora(C|S|reset|reset8)`, `ReportIGE1(s)`, `ReportLBC(s)`, `ReportPCR(s)`, `ReportZN(s)`, `Reportcross(s)`, `ReportSugars`, `Reportanyreport(s)`, `ReportSMS`, `Reportqc`, `Reportlog`, `Reportmony1..5`, `Reportmony01`, `Reportmony11`, `ReportPatPhone`, `ReportTestTime`, `Reportinstrument`, `Reportshareka`, `Reportworkp`, `Reportwp(s|m|t|ot)`, `Reporttotal(s)`, `Reporturines`, `Reporthistory(2)`, `Reportsemen(1|1s|2|2s)`, `repotrwidal(s)`, `reportculutre(s)`, `reportsetup(2)`, `reportstatic1`, `rpttotaliss(s)`, `RPTPatientVisit`, …
* **57** `Result*` forms — `Resulturine`, `ResulturineAuto`, `Resultliver`, `Resultsugar`, `Resultkidney`, `Resultlipids`, `Resultsemen(5)`, `resultABO`, `resultwidal(2)`, `resulthepatitis`, `resultTORCH`, `resultcrp`, `resultpt`, `resulthormon`, `resultculutre`, `resultcbc`, `resultstool`, `resultbloodgas`, `resultcrossm`, `resultTumors`, `resultVitamins`, `resultTrace`, `resultanytest`, `resultpregnancy`, `resultiron`, `resultFertlity`, `resultSuprarenal`, `resultbilharzias`, `resultESR`, `resultabuse`, `resultdrugs`, `resultcoagfactor`, `resultImmunoglob`, `resultimmun`, `resulthgbelec`, `resultprotelec`, `resultprostata`, `resultcsf`, `resultzn`, `resultTUBERCULIN`, `resultmarrow`, `resultpostcoital`, `resultrenal`, `resultretios`, `resulthplori`, `resultPCR`, `resultFMF`, `resultLBC`, `resultIgeAllergy`, `resultha1c`, `resultselectro`, `resultcardiac`, …
* plus `labmain`, `patientmain`, `labdb`, `labdb1`, `labfunction`, `labmainTools`, `labtolab`, `server`, `login`, `logindetect`, `logindete`, `users`, `userspw`, `setup(1|2|3)`, `setup`, `reportsetup(2)`, `barcode`, `barcodeform`, `phone`, `instrument`, `pricelists(N)`, `shoppingfrm`, `antibioticfrm`, `qcfrm`, `printerselect`, `frmTip`, `Frmcomment`, `Frmac`, `Frm_pic(2)`, `Frme_*`, `Frm_*`, `z_*` (`z_units`, `z_barcodes`, `z_branchs`, `z_logs`, `z_gehats`, `z_pricelists`, `z_titles`, `z_group`, `z_customgroup`, `z_labaccount`), `CardFrm`, `CardFrmSetx`, `SMSfrmsenderos`, `Patient_samples`, `RPTPatientVisit`, `mInputbox`, `saveIOtemp`, `sarfz`, `zarf`, `biosais`, `BMSData`, `noteic`, `otherfun`, `aaaClass`, `static1..6`, `MOODY`/`MODY`, …

### 6.3 Compiled-code entry directory (recovered procedure addresses)

A table of **349 code addresses** at **VA `0x00460b98` / file offset `0x60b98`**. Every one of the 349 targets was disassembled and **349/349 begin with a VB6 native-compilation prologue** (`C7 45 xx` = `mov dword ptr [ebp-N], imm`, or `55 8B EC` / `83 EC`):

| index | code VA | code file offset | first bytes |
|---|---|---|---|
| 0 | `0x0045e8140` | `0x05e8140` | `c7 45 fc ae 00 00 00` (`mov dword [ebp-4], 0xAE`) |
| 1 | `0x0045e83a1` | `0x05e83a1` | VB6 frame setup |
| … | … | … | … |
| 348 | `0x00460d2de` | `0x060d2de` | VB6 frame setup |

Block-to-block deltas are the compiled sizes (`0x88`, `0x18a`, `0x21c`, `0x23f`, `0x241`, `0x261`, …). The complete list — index, VA, RVA, file offset, prologue flag — is in **`vb_code_entries.json`**.

### 6.4 Member / control descriptor tables

A second, independent descriptor array (**40-byte stride**) was found in **247 contiguous tables** holding **11,076 records**:

```
+0x00  lpszMemberName   (e.g. 'Label34', 'Combo3', 'cmd_cancel', 'XPButton1')
+0x04  dwFlags          (0x00030009 / 0x0003000C / 0x00030030 / 0x0003002F …)
+0x08  dwTypeFlags      (0x00100040 / 0x00120040 / 0x00130040 …)
+0x0C  dwOffset         (control offset inside the form)
+0x10  lpData           (VA, points into the literal pool)
+0x14  dwFlags2
+0x18  dwNull
+0x1C  dwNull
+0x20  lpDescriptor     (VA of a VB6 data record — NOT a code address; verified by
                        disassembly: the target begins with 00 00 00 00 + pointers)
+0x24  dwExtra
```

Example table at file offset `0xf8558`: `Label34` (flags `0x00030009`, type `0x00130040`, control offset `0x88`, descriptor `0x004f92a4`) → `Combo3` (`0x0003000C`, `0x00100040`, `0x8c`) → `cmd_cancel` (`0x00030030`, `0x00100040`, `0x90`) → `XPButton1` (`0x0003002F`, `0x00120040`, `0x94`).

These are **form control / event-handler descriptors**. **Important correction:** the `+0x20` field is *not* a procedure address — the values were disassembled and land in zero-filled data tables, not code. The recovered procedure *entry points* are the 349 addresses of §6.3; the mapping from an individual control to its own handler body was **not** established. All 11,076 records (name, flags, type flags, control offset, descriptor pointer) are in `vb_members.json`.

### 6.5 Other recovered VB6 structures

* **VB6 `Declare` (external API) table** — file offsets `0x26c634`–`0x26f740` (see §4).
* **OCX / type-library references** — `0x26bb20` (`VB6.OLB`), `0x26bc50` (`ChamaleonButton.oca`), `0x26c0f6` (`Redsoul911XPCOmponents.oca`).
* **Literal pool** — file range `0x269000`–`0x1780000`. Each VB6 literal is length-prefixed (DWORD = byte length including NUL) followed by ANSI or UTF-16LE payload (verified by hand at `0x26d76c`, `0x26db40`, `0x26f4b4`, `0x26b984`). This is what makes SQL/UI text recoverable verbatim.
* `PROJECT` / `PROJECTwm` **text streams are absent** — this is a native-compiled EXE; the project stream, `CMG=`/`DPB=`/`GC=` protection blobs and `[Workspace]` block were **not** found (searched for `ID={`, `Module=`, `Class=`, `Document=`, `CMG=`, `DPB=`, `[Workspace]` — 0 hits each).

---

## 7. Database integration

### 7.1 SQL

**2,375 unique SQL-bearing strings** (1,534 full statements + 742 fragments + 365 DDL statements), all UTF-16LE literals inside the VB6 literal pool.

Verb breakdown: `SELECT` 1,143 · `ALTER TABLE` 241 · `CREATE TABLE` 22 · `DELETE FROM` 72 · `SHAPE` (MSDataShape rollup) 21 · other 35.

Representative statements (file offsets):

| Offset | Statement (truncated) |
|---|---|
| `0x26e940` | `SELECT pricemethod, patientnamear, patientphone, patienttel FROM patientinfo GROUP BY pricemethod, patientnamear, patientphone, patienttel order by pricemethod, patientnamear` |
| `0x26f788` | `SELECT * FROM loger where lognum= ` (parameterised at run time) |
| `0x26f8c4` | `SELECT stripnum,stripdiscrp,strip1 FROM reportview where stripdiscrp='MAIN' ` |
| `0x271678` | `SELECT * FROM insulin where id= ` |
| `0x272764` | `SHAPE {SELECT subgrouptable.subgroupcode, subgrouptable.subgroupname FROM subgrouptable GROUP BY … } AS cmdbill APPEND ({SELECT * FROM testtable where testo…` |
| `0x2913f8` | `SELECT patientinfo.requestdate, patientinfo.patientid, round(patientinfo.elmadfoo3, 1) , round(sum(payment.money),1) as ta7seel FROM patientinfo INNER JOIN payment ON patientinfo.patientid …` |
| `0x293774` | `SELECT patientinfo.requestdate, patientinfo.patientid, patientinfo.patientnamear, patienttest.testsimp, patienttest.otherlabname, Round(SUM(patienttest.testpriceout),2) AS Expr1, …` |
| `0x2d6f5c` | `select startwork,endwork,breakyorn,breaktime from patients where patientusname= '` (username ⇒ staff-schedule table, shared with `patients`) |
| `0x2e1a3c` | `SELECT Top 10 Electrolytes.requestdate, Electrolytes.scatot, Electrolytes.scaion, Electrolytes.sph, Electrolytes.sna, …` |
| `0x289534` / `0x2a744c` / `0x2a74c8` | `DELETE FROM wasl` · `Delete FROM patientinfo where patientid='` · `Delete FROM payment where patientid='` |
| `0x275a38` / `0x27bb8c` | `Create Table IgEallergy (patientid varchar(13), requestdate smalldatetime, testid Smallint, itemNo Smallint, Item1 varchar(50),RESULT1 varchar(25),Class1 Smallint,TypeF1 Smallint, …)` |
| `0x27c2a8`, `0x27ca14`, `0x27d180` | `ALTER Table IgEallergy ADD COLUMN Item12 varchar(50),RESULT12 varchar(25),Class12 Smallint, TypeF12 Smallint, Item13 …` (repeated for Item24, Item36, …) |

The SQL uses **Jet/Access dialect** (`Smallint`, `smalldatetime`, `varchar(n)`), `Top N` (**155** `SELECT Top N` statements), `SHAPE … APPEND` (MSDataShape), and MS Access `SHAPE`/rollup syntax.

**Extracted schema objects:** 127 distinct table identifiers, **508 distinct qualified `table.column` pairs**, 921 distinct column tokens. Full lists in `sql_inventory.json`.

### 7.2 Cross-reference against the known `Patients.mdb` table list

Of the table names supplied in the brief, **42 of the 43 named are referenced by the executable**:

`patients` (28) · `patientinfo` (447) · `patienttest` (304) · `normalrange` (99) · `testtable` (62) · `constanttable` (3) · `converts` (2) · `testunits` (7) · `abbreviations` (6) · `subgrouptable` (14) · `cultures` (25) · `antibiotic` (13) · `antibio` (2) · `history` (13) · `MyLabSetting` (2) · `daytip` (2) · `comments` (28) · `barcodes` (7) · `PTC` (8) · `ratios` (5) · `reportview` (2) · `customtesttable` (8) · `customgrouptable` (6) · `testtemp` (7) · `hba1c` (3) · `insulin` (23) · `testlearn` (2) · `loggroup` (7) · `pictures` (88) · `pricelist` (9) · `pricemenu` (2) · `smslist` (13) · `smsgroups` (10) · `SMSSender` (15) · `SMStemplate` (6) · `titless` (10) · `anyreport` (5) · `culturesec` (11) · `Helicobacter` (7) · `ESR` (5) · `TORCH` (4) · `Labbranch` (7)

(counts = number of SQL statements in which the identifier appears).

**`Paste Errors` is NOT referenced anywhere in the executable** — searched the whole file for `paste`, `Paste Errors`, `Pasterror` (case-insensitive): **0 hits**.

### 7.3 Tables referenced by the EXE that are NOT in the supplied list (DB-side verification requested)

78 additional identifiers, e.g. `payment` (28), `Gehats` (24), `cbc` (19), `wasl` (19), `sharekathesab` (19), `prostaticx` (14), `phone` (11), `BloodGases` (10), `patienttotal` (10), `useraccounts` (9), `IgEallergy` (8), `stool` (8), `Proelectrophoresis` (7), `cultureanti` (7), `Abuse` (7), `seana` (7), `CSFT` (6), `testname` (6), `tasgel` (6), `shopping` (6), `loger` (5), `semen` (5), `urine` (5), `pregnancy` (5), `noteic` (5), `instrument` (5), `sharekat` (5), `LBC`, `lipids`, `Kidney`, `samples`, `liver`, `cardiac`, `CoagFactors`, `Drugs`, `Electrolytes`, `Fertlity`, `hepatitis`, `IMMUNOLOGY`, `hormones`, `Immunoglobulin`, `Pancreas`, `Suprarenal`, `trace`, `Tumors`, `Vitamins`, `Hgbectrophoresis`, `crossmat`, `GLUCOSETOLERANCE`, `renalfailure`, `FMF`, `widal`, `widal2`, `ABO`, `BILH`, `CRP`, `iron`, `kidney`, `stone`, `TUBERCULINT`, `BONEMAR`, `postcoitalt`, `sarfeda3`, `CBC`, `Sugar`, `Urine`, `ZN`, `sugar`, `SUGAR`, `abo`, `crp`, `bilh`, `bloodgases`, `csft`, `zn`, `Samples`.

**Inference (labelled):** most of these are almost certainly **analyte-specific result tables** (one table per test family) plus support tables (`payment`, `useraccounts`, `loger`, `shopping`, `instrument`, `noteic`, `sharekat*`), because (a) the EXE also creates `IgEallergy` with `ALTER TABLE … ADD COLUMN ItemN/RESULTN/ClassN/TypeFN` DDL, and (b) each of them has a matching `Data\<NAME>.ini` / `<NAME>_S.ini` pair (§5). The DB agent should confirm the true table list and whether these are user tables, linked tables, or MS Access tables produced by the app itself. Note `Win32_Processor` is a false positive from a WMI-ish string, not a table.

### 7.4 DB operations inferred (from the statement mix)

* **Read** — the bulk: `SELECT … FROM patientinfo / patienttest / normalrange / testtable / pictures …` with `GROUP BY` / `ORDER BY` aggregations for reports, `WHERE <pk>=` parameterised single-row lookups.
* **Schema migration** — 241 `ALTER TABLE <t> ADD COLUMN …` + 22 `CREATE TABLE …` statements → the EXE **creates and extends tables at run time** (the `IgEallergy` table is created with `patientid`, `requestdate`, `testid`, `itemNo`, `Item1..N`, `RESULT1..N`, `Class1..N`, `TypeF1..N`).
* **Delete** — 72 `DELETE FROM …` statements (log/report cleanup).
* **MSDataShape roll-ups** — 21 `SHAPE {SELECT …} AS alias APPEND ({SELECT …})` = hierarchical group-by rollups used by the reports.
* No `INSERT INTO` or `UPDATE … SET` literal was found in the string pool ⇒ row writes are almost certainly performed through **parameterised ADODB/ADODC command objects** (`Recordset.AddNew`/`UPDATE`/`INSERT` built at run time) rather than literal SQL text. *Inference — cannot be confirmed statically without decompiling the recordset code paths.*

---

## 8. Reports / printing

* **61 `Report*` forms** and 57 `Result*` forms (see §6.2). Additional report-ish object names: `rpttotaliss`, `rpttotalisss`, `RPTPatientVisit`, `statisReport`, `reportstatic1`, `reportsetup(2)`, `Reportqc`, `Reportlog`.
* **Printer-role names (all present, 10 hits):**

| Role name | Offset |
|---|---|
| `reportprinter` | `0x2d6194` |
| `reportprinterx` | `0x2821b4` |
| `zarfprinter` | `0x285150` |
| `barcodeprinter` | `0x2b175c` |
| `waslprinter` | `0x2ab334` |
| `IDprinter` | `0x2a5ea4` |

  These are **INI keys** (the app reads the configured printer per role via `GetPrivateProfileStringA`).
* **Print stack / PDF** — `Bullzip PDF Printer` (`0x2a24c8`), `Bullzip PDF Printer 11.1 - Create File` (`0x2a25a0`), `winspool.drv` (`0x26e924`), `EnumPrintersA` (`0x26eb1c`), `SetDefaultPrinterA` (`0x26ebb0`), plus controls `cmd_pdfExport`, `cmd_pdfExport2`, `Chk_PDFpath`, `Chk_PDFpath2`, `PDFpaths`, `pdfhead`.
* **PDF output directories** — `D:\PDF` (`0x28237c`), `D:\PDF\` (`0x282390`), `D:\PDF\Free\` (`0x298e88`), `D:\PDF\Contracts\` (`0x2a2384`), `D:\PDF\Individual\` (`0x2a2510`), `D:\PDF\LabToLab\` (`0x2a2570`).
* Barcode printing: `cmd_bar1print` … `cmd_bar15print`, `cmd_notprint`; printer pickers: `printerselect`, `selectprinter`, `cboPrinters`…`cboPrinters5`, `imlPrinters`, `check_onePrinter`, `onePrinter`, `oneesalprint`.

---

## 9. Authentication

**No password or secret value is reproduced anywhere in this report.** Findings are presence/type/offset only.

| Type | Offset(s) | Observation |
|---|---|---|
| UI caption — internal password prompt | `0x26d76c` | `Please Enter Your Internal Password.   (Digits only)` — prompt text, digits only |
| UI caption | `0x26d7dc` | `Internal Password` — dialog caption |
| UI caption | `0x26dad0` | `No Password Entered.` — validation message |
| UI caption | `0x26db04` | `You must enter a Password.` |
| UI caption | `0x26dd5c` | `Wrong Password Entered.` |
| UI caption | `0x26dd90` | `Enter The Correct Password.` |
| UI caption | `0x284e08` / `0x1054031` | `Print Result Password` |
| UI caption | `0x2ee678` | `Enter login password` |
| UI caption | `0x334b78` | `Error ... Enter the Accont ID and Password first` |
| UI caption | `0x2ab878` / `0x2b1638` | `     By password: ` / `Password ` — report footer label |
| UI caption | `0x3fa0a1` | `Account Password` |
| Control name | `0x26becc`, `0x26ccef`, `0x270128`, `0x373635`, `0x4e9cb4` | `cmd_passwordch`, `dcmd_printPassword`, `Txt_password` (×3) |
| Config key | `0x2a5f50`–`0x2a5ff8` | `IDPasswordL/T/F/B/S/V/A` — seven INI keys for the per-workstation ID-card/password configuration |
| Form name | `0x26afe8` | `userspw` — the user-password maintenance form |

**No hard-coded password, PIN, hash, DPB/CMG project-protection blob, or `Password=`/`Pwd=` clause was found in any literal.** The four SQL Server connection strings use `Integrated Security=SSPI` or `Persist Security Info=True` **without** a `Password=` segment, and the Jet OLEDB strings carry no credentials. The application's password checks are therefore server-side / database-side, or performed against `useraccounts` / `z_labaccount` rows. *Inference — not statically confirmable.*

---

## 10. Decompilation

See `tool_log.md` (§Ghidra and the REA-MCP table) for the exact attempts. Summary of outcome:

* The only deep-analysis engine present is **Ghidra 12.1.4** (Hopper / IDA / jadx / kuna are not installed).
* REA's Ghidra wrapper failed three times for **tooling** reasons (provider startup deadline exceeded while importing a 76 MB binary; then two runs aborted by `IllegalArgumentException: Unknown Ghidra analyzer for this program: <name>` because the wrapper validates analyzer names against the installed Ghidra build). A fourth run with a valid analyzer list and a 2-hour startup deadline was launched.
* **Independent of Ghidra**, the equivalent information was recovered by decoding the VB6 structures directly (§6) — and that route yielded *more* than a decompiler would have: the `ProjectData`/`ObjectTable`/`PublicObjectDescriptor` chain (210 named objects, project GUID `{AF67146C-22A0-4591-9ED3-52BEB9D44D32}`, member counts, object types), 11,076 control/member descriptors, a 349-entry compiled-code directory whose every entry was prologue-verified, 2,375 SQL statements, and the full literal pool. A generic decompiler sees only opaque MSVBVM60-driven x86.

**Assessment (inference):** even with a successful Ghidra run, decompilation of a VB6 native-compiled binary yields little beyond what was recovered here — the VB6 runtime does the object/procedure dispatch, control flow is threaded through `__vbaLateIdCall`/`__vbaLateMemCall` late-bound dispatches, and there are no symbols, no PDB and no export table to anchor names. The practical value of decompilation would be limited to individual event-handler bodies reachable from the 349 recovered entry addresses in `vb_code_entries.json`.

---

## 11. Not recovered / limitations

1. **No `PROJECT` / `PROJECTwm` text stream.** The project stream, `ID={GUID}`, `Module=`/`Class=`/`Document=` lines, `CMG=`/`DPB=`/`GC=` protection blobs and `[Workspace]` block are all absent (0 hits). The EXE is native-compiled; only the binary object table survives.
2. **Procedure (method) *names* are not stored.** VB6 native compilation discards procedure names; only *control* names and *object* names survive. The 11,076 recovered member descriptors are control/event-handler descriptors; their `+0x20` field was **verified by disassembly to be a data pointer, not a code address**. The recovered procedure *addresses* are the 349 entries of the compiled-code directory (§6.3, all prologue-verified), but they are **not** name-linked to the control descriptors. Internal `.bas` procedure names are therefore **not recoverable** from this file.
3. **Per-form attribution of the flat control-name blob was not established.** The ANSI control-name blob (`0x26bb08`–`0x26e100`+) and the 247 member tables are two independent orderings; the per-object `lpObjectInfo`/`lpMethodNames` slots that would join them are zero-filled in the file image. Control names are therefore reported as a flat set (2,213 unique members) rather than assigned to individual forms. *This is the single largest structural gap.*
4. **`lpPublicBytes`** pointers were resolved for all 210 objects, but the pointed-to data is the *literal pool* (unicode/ANSI constants), not a name table — the per-object constant blocks were not segmented.
5. **`lpCodeStart` / `lpCodeEnd` / `dwDataSize`** in `ProjectData` (`0x01b41260` / `0x04c97d00` / `305,356`) do not correspond to a `.text` VA range and could not be interpreted as a code span; the real code extent was instead established empirically from the 349-entry compiled-code directory.
6. **Ghidra analysis** — three REA runs failed on tooling/timeout grounds (see §10 and `tool_log.md`); a fourth run with a valid analyzer list and a 2-hour startup deadline was launched and had not completed when this report was finalised (Ghidra headless still consuming CPU on the 76 MB import, project not yet saved). Even on success, expected yield is low (§10).
7. **`.data` section is almost entirely zero** (entropy 0.000, raw size 4,096 vs virtual size 304,852) — the section exists but carries no recoverable static content; anything it holds is written at run time.
8. **No relocation table** — the image is fixed at `0x400000`; address-dependent analysis must assume the preferred base.
9. **The string sweep is byte-level, not semantic.** 34,745 in-pool byte runs were dropped as compiled-code noise by heuristics (vowel-ratio and character-class filters). A small number of genuine literals could in principle have been dropped; the 2,600 unique UI captions and 1,534 SQL statements are the verified-high-value set. `identifier_or_other` (7,659 unique) is dominated by VB6 control-property names (`caption`, `font`, `backcolor`, `TabIndex`, `Picture`, `Appearance`, `BorderStyle`, …) and font names.
10. **SQL text is verbatim but not always whole.** Many statements are stored as *fragments* with the table name or predicate supplied at run time (e.g. `SELECT * FROM insulin where id= `), so the full predicate text for parameterised queries is not in the binary. Column names referenced only through such parameters are missing from the 921-token column set.
11. **No INSERT/UPDATE literals** — writes are parameterised; the write-side SQL text cannot be recovered.
12. **Registry usage**: none found (0 `HKEY_*`/`Software\…` literals). This is a *negative* result, but it is a whole-file literal sweep, so registry paths built at run time would not be visible.
13. **No manifest** — the manifest content requested in the brief does not exist in this binary; nothing was reported because nothing is present.
14. **Installer / payload not analysed** — out of scope for this workstream; the target is the single EXE.

---

## 12. What was hashed / extracted, and where

All outputs are under `C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\`.

| File | Size | Content |
|---|---|---|
| `exe_findings.md` | this document | the report |
| `exe_inventory.json` | 49 KB | identity + PE + sections + data dirs + resources + version info + imports + VB6 structure + string statistics |
| `strings_classified.json` | 12.2 MB | 101,812 classified strings with file offsets, encodings and categories + 15,143-entry unique-text index |
| `strings_raw.json` | 26.2 MB | 369,877 unfiltered ASCII + UTF-16LE string hits with offsets (pre-classification) |
| `resources.json` | 6 KB | resource tree with type/ID/lang/size/RVA/**file offset**/SHA-256 + extraction path |
| `imports.json` | 35 KB | full import table (231 functions, ordinals, hints, thunk RVAs), delay-import section |
| `sql_inventory.json` | 476 KB | 2,375 unique SQL strings with offsets + 127 table identifiers + 508 qualified column pairs + 921 column tokens |
| `db_xref.json` | 4 KB | cross-reference: known-table matches, unknown-table list, standalone-string hits |
| `vb_objects.json` | 78 KB | the 210 VB6 objects (name, member count, ObjectInfo pointer, PublicBytes pointer, object type, record offset) |
| `vb_members.json` | 3.2 MB | 11,280 member descriptor records + 247 tables (name, flags, type flags, control offset, descriptor pointer) |
| `vb_code_entries.json` | 132 KB | 349 compiled-code entry addresses (VA / RVA / file offset + VB6-prologue verification) |
| `version_info.json` | 365 B | VS_VERSIONINFO strings |
| `pe_identity.json`, `pe_dirs.json` | 33 KB / 1 KB | raw PE header, sections, data directories |
| `tool_log.md` | — | every tool invoked, version, success/failure |
| `resources/` | 12 files | extracted resource payloads (icon group, 10 icons, version block) |
| `*.py` | — | the analysis scripts used (reproducible) |

**Hashes computed:** SHA-256 of the whole target (2 independent computations, both `C54CE6C8…DF93C0`), and SHA-256 of each of the 12 extracted resource payloads (recorded in `resources.json`).
