# Configuration Analysis — REAL LAB SYSTEM v1.605

**Scope.** Static reconstruction of the configuration surface. **No secret value and
no patient identifier is reproduced anywhere in this document.** Where a field is
credential-bearing, only its *name and purpose* are recorded. Where a prior report
exposed personal data, that report is described, not quoted.

**Confidence legend.** VERIFIED / STRONGLY_SUPPORTED / INFERRED / UNVERIFIED /
CONFLICTED / UNKNOWN.

---

## 1. Evidence inventory

| Evidence file | What it supplies |
| --- | --- |
| `ini-analysis.json` | 146 INI files; `sections_observed` = `reportdata` 146, `setupx` 146, `FORM` 100, `report` 34; `distinct_key_count` 1394; `keys_observed` frequency map |
| `ini-key-delta.csv` | Columns `file, kind, key, v1, v2019` — per-key V1→V2019 changes |
| `ini-diff-summary.md` / `ini-diff-summary.json` | Clinical configuration delta; category counts; per-file `status`, `added`, `removed`, `changed` |
| `analyse_ini.py` | The structural INI analyser (encoding detection, `[section]`/`key=value` parse, frequency collection) |
| `configuration-and-resources.md` | B3 report: INI vocabulary, RTF cache, images, fonts, hygiene summary |
| `06-configuration.md` | `BASE.ini`, `data_connection*.udl`, `SETTING.ini`, `IDCARD.INI`, `REPORT.dll`, `NOTE.ini`, `Data\*.ini` |
| `Credential-And-Fix-Report_2026-11-01_00-51.md` | Credential-discovery scan and environment fixes — **described only**; no values copied |
| `database-and-data-access-analysis.md` | Connection templates and credential-assembly shape |
| `08-deployment-installer.md`, `12-install-media.md` | `D:\Lab_data` layout, install-time database selection |

---

## 2. Configuration mechanisms present (VERIFIED)

| Mechanism | Files | Notes |
| --- | --- | --- |
| Windows INI | `BASE.ini`, `SETTING.ini`, `IDCARD.INI`, `NOTE.ini`, 223 cumulative `Data\*.ini` | primary config surface |
| UDL connection descriptor | `data_connection.udl`, `data_connection1.udl` | OLE DB provider strings |
| DLL extension used as INI | `REPORT.dll` | plain text `[reportdata]` + `color1..color19`, not a PE DLL |
| RTF | 58 files / 29 distinct documents | clinical narrative templates |
| Fonts / images | 14 TTF (7 distinct), 14 GIF, 19 JPEG | report assets |

`configuration-and-resources.md` §1 states B3 contains **no XML, JSON, YAML,
`.config`, registry hive, environment-variable script or environment file**.
Scope note: the B3 archive holds **146** INI files (`ini-analysis.json`); the
cumulative deliverable set holds **223** `Data\*.ini` files (`06-configuration.md` §7,
`ini-diff-summary.md` header). Both denominators are correct for their scope.

---

## 3. `bin\BASE.ini` — fields only (VERIFIED field names)

Source: `06-configuration.md` §1.

| Section | Field | Purpose |
| --- | --- | --- |
| `[oledb]` | `Databasetype` | selects Access vs other back end |
| `[oledb]` | `loca` | configured data location (an `E:\Lab_data` path in the sampled file) |
| `[oledb]` | `DataSource` | server/instance name |
| `[oledb]` | `PersistSecurityInfo` | controls whether credentials travel with the connection object |
| `[oledb]` | `UserID`, `Password` | credential fields — **values deliberately not reproduced** |
| `[oledb]` | `InitialCatalog` | database name (`Patients` in the sampled file) |
| `[Initial]` | `Initialw` | initial setting |
| `[Initial]` | `nrls` | release marker — value `605` in the sampled file, corroborating the v1.605 identity |
| `[Initial]` | `drls` | secondary release flag |

**Drive-letter conflict (CONFLICTED):** `BASE.ini` `loca` points to `E:\Lab_data`,
while `08-deployment-installer.md` §2–3 and the `Attach_*.bat` names use
`D:\Lab_data`. This is installation-dependent, not a single canonical path.

---

## 4. UDL connection descriptors — no values reproduced

Source: `06-configuration.md` §§2–3; `database-and-data-access-analysis.md` §2.

| File | Fields present | Purpose (VERIFIED) |
| --- | --- | --- |
| `data_connection.udl` | `Provider=SQLOLEDB.1`, `Integrated Security`, `Persist Security Info`, `User ID`, `Initial Catalog`, `Data Source`, `Initial File Name` | SQL Server connection; `Initial File Name` targets a BF6500 automatic-haematology-analyser database under `D:\Lab_data` — an **instrument-vendor artefact**, not first-party lab logic |
| `data_connection1.udl` | `Provider=MSDataShape.1`, `Data Provider=SQLOLEDB.1`, `Persist Security Info`, `User ID`, `Password`, `Initial Catalog`, `Data Source` | shaping connection used for hierarchical recordsets; database `Patients` |

`dependency-inventory.json` → `framework_dependencies[0]` independently cites
`Provider=SQLOLEDB.1` in UDL/INI. `database-and-data-access-analysis.md` §2 explains
that credentials are assembled from separate UTF-16LE literals at run time, which is
why a naive string scan yields no single usable secret — the *shape* is evidenced,
the *value* is not recorded here.

---

## 5. `SETTING.ini`, `IDCARD.INI`, `REPORT.dll`, `NOTE.ini`

Source: `06-configuration.md` §§4–8.

- **`SETTING.ini`**
  - `[selectprinter]`: `reportport`, `barcodeport` (both `LPT1:` in the sampled
    file), `reportprinter`, `barcodeprinter`, `zarfprinter`. The sampled
    `reportprinter` value names a note-taking printer, not a lab printer — an
    operator convenience, recorded as a field purpose only.
  - `[setup]`: `papersize` (`A4`), `reportstart`, `Timeouts`, `historytype`,
    `labbranchx`, `barcodeauto`, `printworkpaper`.
  - Twelve text slots `Textr1..4`, `Textl1..4`, `Textc1..4` plus report footnotes.
- **`IDCARD.INI`**: `[IDdata]` maps ID-card field positions (fields labelled
  `L`/`T`, `F`, `B`, `S` in the source) to printed identifiers; `[Billdata]` holds
  welcome/thank-you captions. No personal data reproduced.
- **`REPORT.dll`**: despite the extension, a plain-text INI with `[reportdata]` and
  `color1..color19` (BGR integer values). `configuration-and-resources.md` §1 treats
  it as report layout configuration, not a loadable DLL.
- **`NOTE.ini`**: `[NOTICZ]` with `STATUS`, `NID`, `LOID` — notice/version metadata.
  Values not reproduced.

---

## 6. `Data\*.ini` — report templates (VERIFIED)

From `configuration-and-resources.md` §§2–6 and `ini-analysis.json`:

| Section | Files (of 146 in B3) | Role |
| --- | ---: | --- |
| `[reportdata]` | 146 | layout geometry, colours, typography, report titles, print flags |
| `[setupx]` | 146 | one key only: `reportprinterx` |
| `[FORM]` | 100 | result-item schema: labels, values, defaults, references, limits, flags |
| `[report]` | 34 | interpretive note text with per-line formatting |

Key families (abbreviated; full catalogue in `configuration-and-resources.md` §§3–5):

- identity: `reportmaintitle`, `formsubtitle`, `reportsubtitle1..3`,
  `reportsubtitleF1..3`, `normal`, `comment`, `main_symb`, `sub_symb`,
  `signature`, `signature2`, `signature3`.
- print flags: `printcritical`, `printhistory`, `printsen`, `printcommer`,
  `statusflag`, `backcolor`, `reportstart`.
- geometry: `titleleft`, `left1..7`, `width1..7`, `topvalue`, `bottomvalue`,
  `leftvalue`, `rightvalue`, `spacevalue`, `spacetitle`.
- typography: `colora..colort` (20 BGR colours), `colorXfn` (font name),
  `colorXfs` (size), `colorXfb` (bold). Font usage across the set: **Arial 675,
  Tahoma 597, Times New Roman 328** occurrences (denominator = all font-slot
  assignments counted in the report).
- `[FORM]` families: `LabelT01..15` (1,200 instances), `ITEM01_1..ITEM15_N`
  (8,140), `ITEMS1_1..ITEMS9_N` (6,396), `ITEM01_Ref` (480), `Check1x..Check7x`
  (396), `CheckT01..08` (584), plus pH/titration vocabularies. Total distinct
  keys across the set: **1,394**.

**Printer binding:** `[setupx] reportprinterx` — `HP LaserJet 1100 (MS)` in 138 of
146 B3 files and `HP LaserJet 1100` in 8 (the `_S` microbiology variants).
`06-configuration.md` §7 reports 108 / 2 / 2 across a different (223-file)
cumulative scope — CONFLICTED by denominator, both values retained.

**Encoding (VERIFIED):** 134 of 146 files ASCII, **12 Windows-1256** (the four
IgE-allergy panel families `164`, `166`, `167` and their `_S` variants in two trees);
144 CRLF, 2 mixed (`103.ini`, `103_S.ini`). Seven files repeat a key inside a
section, always with an identical value. A minority uses a space before `=`
(e.g. `reportstart =2000`) — a hand-assembly marker.

---

## 7. `D:\Lab_data` folder layout and database attachment

Source: `08-deployment-installer.md` §§2–3; `12-install-media.md` §§7–8.

```
D:\Lab_data\
  Patients.mdf          → attached by Attach_Patients.bat   (database "patients")
  Interface.mdf         → attached by Attach_Interface.bat  (database "Interface")
  VIDAS (mdf)           → attached by Attach_VIDAS.bat
  Attaching.sql         → referenced, NOT present in inventoried tree
  Attach_database.bat   → referenced, NOT present in inventoried tree

D:\real lab system\     (installed application tree)
  real lab system.exe
  bin\                  (Patients.mdb, Branch_Data.mdb, tools)
  Data\ , Data\data_emp\ , Cache\ , Images\ , Fonts\
```

`Interface` is a **separate application database** alongside `patients`, carrying
settings/lookup tables; the prior report notes the third `VIDAS` database without
a described role. VERIFIED as names/paths; role details INFERRED.

---

## 8. RTF and flash resources (VERIFIED from `configuration-and-resources.md`)

- **RTF cache:** 58 files, 29 distinct documents, byte-identical mirrors. Headers:
  `\rtf1`, `\adeflang 3073` (Arabic, Saudi Arabia), `\ansicpg 1256`, `\deflang 1033`.
  `\info` metadata: `\author real lab`, `\company ais group`, `\vern 16437`
  (Word 2002); `\operator` alternates between `real lab` and `ais`. Embedded
  pictures: `056.rtf` 4 (2 EMF, 2 placeable WMF), `070.rtf` 2 (1 EMF, 1 WMF).
- **GIF (14):** toolbar buttons and HTML-tag markers in
  `System/datareport_vh/`.
- **JPEG (19):** PCR graphics, stationery (header/footer/wasl), logo placeholders;
  no EXIF metadata in any of the 19.
- **Fonts (14 files, 7 distinct):** `ais lab.ttf` (Tahomax), `ais symbol.ttf`,
  `Digiface.ttf`, `FREE3OF9.TTF`, `MCS Taybah.ttf`, `WBCW_UPCEAN.ttf`,
  `WBCWS_UPCEAN.ttf`. One DSIG signature (`ais lab.ttf`); two carry a
  redistribution-prohibited notice.
- **Flash:** 13 byte-identical Macromedia Flash Player 8.0 r22 projector stubs
  (`Cache.exe`…`012.exe`) with SWF overlays, used as an animation host for the
  interactive report viewer (`BATCH-03.md`, `09-presentation-library.md`). Third-party
  runtime, inventoried, not first-party logic.

---

## 9. V1 → V2019 configuration delta (VERIFIED counts, denominators explicit)

`ini-diff-summary.md`: V1 **223** files, V2019 **223** files; **50 identical**,
173 changed. Category counts: `signature_moved_to_global` 167, `keys_added` 86,
`value_changed` 83, `check_flags_disabled` 18, `check_flags_enabled` 13,
`only_in_one` 1.

Selected per-key deltas from `ini-key-delta.csv` (columns `file, kind, key, v1, v2019`):

| Key | Category | Count | Direction |
| --- | --- | ---: | --- |
| `reportdata.signature` | `signature_moved_to_global` | 167 | moved out of per-file templates |
| `reportdata.signature2` | `signature_moved_to_global` | 152 | same |
| `reportdata.signature3` | `signature_moved_to_global` | 145 | same |
| `form.LabelX002` | added | 32 | microbiology "Organism C" column |
| `form.Check*` | disabled/enabled | 18 / 13 | visibility switches changed |
| `reportdata.left*/width*` | value_changed | up to 40 each | geometry retuning |

A cross-reference in `ini-diff-summary.md` §3 links the added `Organism C` label to
the v1.605 DDL `ALTER Table cultures ALTER COLUMN organism / organism2 / organism3
varchar(80)` — a report-schema ↔ database-schema coupling (STRONGLY_SUPPORTED).

---

## 10. Handling of the credential/fix report (NO values reproduced)

`Credential-And-Fix-Report_2026-11-01_00-51.md` is a 2026-11-01 report that:

- scanned four `.mdb` files (`Branch_Data.mdb`, `data.mdb`, `Patients_backup.mdb`,
  `Patients.mdb`) across V1 and V2019 for credential-related tables/columns;
- found **no credential table**, only a `username`/work-time table `tasgel` (empty)
  and an SMS queue table whose rows contain **operator names, patient identifiers,
  mobile numbers and message text**;
- recorded two environment corrections: an Arabic marshalling error message and a
  date-format check (`sShortDate`, `iDate`) already set to the required value.

**This document deliberately does not reproduce any value from that report.** The
SMS rows are patient-identifying data; their presence in a sample database is
recorded as a data-handling observation only. Related fix notes
(`MDF-SQL-Fix_2026-11-01_02-41.md`, `NonUnicode-Arabic-Fix_2026-11-01_03-12.md`)
exist in the evidence corpus and are likewise not quoted. `rl-setup-analysis.md`
finding 9 and `08-deployment-installer.md` §2.2 independently confirm that
cleartext credential literals exist in installer scripts and configuration files;
**field names and presence are evidenced, values are withheld**.

---

## 11. Configuration hygiene summary (VERIFIED from `configuration-and-resources.md` §11)

| Check | Result |
| --- | --- |
| Credentials/connection strings in B3 resource files | 0 (they live in `bin\` config and installer scripts) |
| RTF OLE objects, embedded packages, DDE links, macros, field codes | 0 |
| Duplicate keys within a section | 7 files, all same-value |
| Mixed line endings within a file | 2 files |
| Non-ASCII encodings in the ASCII set | 12 files (cp1256) |
| Files with any integrity metadata | 0 of 251 |
| Section names outside the four known | 0 |
| Unrecognised key families | 0 |

---

## 12. What is NOT evidenced / UNKNOWN

- Runtime precedence among `BASE.ini`, UDL files and `SETTING.ini` (UNKNOWN).
- Whether the `E:\Lab_data` / `D:\Lab_data` conflict is resolved by the installer
  or by operator action (UNKNOWN).
- Exact meaning of `grantN` columns and `historytype` (INFERRED).
- Whether the 12 cp1256 INI files render correctly under every Windows locale
  (UNVERIFIED; the installer imposes English (Canada) per `08-deployment-installer.md`
  §6).
- No live configuration was read from a running system.
