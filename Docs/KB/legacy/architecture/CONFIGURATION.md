# Legacy Configuration Surface — V-ROOT-1.00 and V-CD-2019

**Purpose:** Curated reference for all legacy configuration files and keys.
**Scope:** `BASE.ini`, `SETTING.ini`, `IDCARD.INI`, `REPORT.dll`, `NOTE.ini`, `data_connection*.udl`, `Data\*.ini`.
**Audience:** Developers, deployers.
**Version applicability:** Mixed (keys noted per version).

---

## 1. `bin\BASE.ini` — database selection

```ini
[oledb]
Databasetype=XMSACCESSX      ; MS Access (X = enabled)
loca=E:\Lab_data              ; laboratory data directory
DataSource=DESKELTEL          ; laboratory workstation name (V1) / REAL_LAB (V2)
PersistSecurityInfo=True
UserID=sa
Password=12345678             ; ⚠ cleartext
InitialCatalog=Patients
[Initial]
Initialw=123
nrls=605                      ; last modified range id
drls=0
```

V2 adds `[lastlog]` section logging last 10 connection attempts including password in cleartext
(F-09 security finding). `DataSource=REAL_LAB` in V2 matches the default server name in the installer.

**Evidence:** external: 06-configuration.md §1; external: 11-deployment-payload-2019.md §3.

## 2. `bin\data_connection.udl` — SQL Server connection

```ini
[oledb]
Provider=SQLOLEDB.1;Integrated Security=SSP;Persist Security Info=False;
User ID=sa;
Initial Catalog=C:\PROGRAM FILES\AUTOMATIC HEMATOLOGY ANALYZER\
                  AUTOMATIC HEMATOLOGY ANALYZERDATABASE\BF6500.DBMDF;
Data Source=REALLAB_AIS;
Initial File Name=D:\Lab_data\bf6500.db.mdf
```

**★** `BF6500` / `AUTOMATIC HEMATOLOGY ANALYZER` reveals the pre-rebranding product name.

## 3. `bin\data_connection1.udl` — shape (UNION) connection

```ini
[oledb]
Provider=MSDataShape.1;Persist Security Info=True;
User ID=sa;Password=12345678;              ; ⚠ cleartext
Initial Catalog=Patients;
Data Provider=SQLOLEDB.1;Data Source=REAL_LAB
```

`MSDataShape.1` is the "shape" provider that executes `SHAPE {…} APPEND (…)` — confirming the
20 SHAPE statements in the SQL corpus. The consolidated report capability appears bound to the
SQL Server path (V2-BR-017).

**Evidence:** external: 06-configuration.md §2–3.

## 4. `bin\SETTING.ini` — printing and paper

- `[selectprinter]`: `reportport=LPT1:` · `barcodeport=LPT1:` · `reportprinter=Send To OneNote 2007` (V1) / `HP Officejet 6000 E609a Series` (V2) · `barcodeprinter` · `zarfprinter` · (V2) `waslprinter` · (V2) `IDprinter`.
- `[setup]`: `papersize=A4` · `reportstart=1984` · `Timeouts=09:00 PM` · `historytype=0` · `labbranchx=1` · `barcodeauto=1` · `printworkpaper=1`.
- Arabic header text: `Textr1txt=المنظومة البرمجية` · `Textr2txt=ريال لاب سيستم`.
- 12 text positions (Textr1..4, Textl1..4, Textc1..4) with font/size + paper margins.

**V1 vs V2 changes:** `reportprinter` changed from `Send To OneNote 2007` to a real HP printer;
`waslprinter` and `IDprinter` added; `reporthead`/`zarfhead` 2 → 0 (headers disabled); fonts
unified to Times New Roman 11.25.

**Evidence:** external: 06-configuration.md §4; external: 11-deployment-payload-2019.md §4.

## 5. `bin\IDCARD.INI` — patient card / invoice

- `[IDdata]`: position (`L`/`T`), font (`F`), bold (`B`), size (`S`) for three fields: name, barcode, password.
- `[Billdata]`: invoice strings — "Welcome To RealLab", "Thank you for visiting RealLab!", working hours, address, telephone, website, e-mail, Facebook page.

## 6. `bin\REPORT.dll` — NOT a DLL

A 562-byte **text file** (despite the extension). `[reportdata]` + `color1..color19` (BGR integers:
`14737632`=0xE0E0E0, `12632256`=0xC0C0C0, `16777215`=0xFFFFFF, `46444020`=0x2C4C64,
`4210752`=0x404040). This is the **report colour palette** that VB6 opens as text.

**Evidence:** external: 06-configuration.md §6 (ISS-12 resolved).

## 7. `bin\NOTE.ini`

`[NOTICZ] STATUS=1 · NID=22 · LOID=831` — last-notice state. V2 expanded to 76 bytes (UTF-16LE)
with new `orls`/`LOID` key (last notice ID).

## 8. `Data\*.ini` — report layout profiles

223 files covering 114 clinical categories. Each file:
- `[reportdata]`: report headings, 7 column positions, 7 column widths, 24 colour values, one font
  per position, print flags.
- `[setupx]`: `reportprinterx=HP LaserJet 1100 (MS)` (the printer bound to that profile).

Printers bound: `HP LaserJet 1100 (MS)` (108 profiles), `HP LaserJet 1100` (2), `Send To OneNote 2007` (2).

V2: 173 of 223 files changed (column reflow for third organism), 50 identical. `signature`/`signature2`/
`signature3` removed from 167 files (centralised to `SETTING.ini`/`IDCARD.INI`).

**Evidence:** external: 06-configuration.md §7; external: 11-deployment-payload-2019.md §2.6, §5.

## 9. Configuration invariants (modern Ref Lab)

The modern system does **not** reproduce this configuration surface. It uses:
- Environment variable `REFLAB_CONNECTION_STRING` (no `appsettings.json`).
- EF Core code-first model (no `.ini` layout files).
- QuestPDF for reports (no per-profile printer binding).
- `ReportBranding.cs` compile-time constants (no `.ini` branding).

**Evidence:** Comprehensive Local Development Environment, Tools, Runtime, and Database Infrastructure Audit.md §A.2, §A.10.

## 10. Version applicability

- `BASE.ini`, `SETTING.ini`, `IDCARD.INI`, `REPORT.dll`, `NOTE.ini`, `data_connection*.udl`: **SHARED** (same structure, V2 extends).
- `Data\*.ini`: **VERSION-DEPENDENT** (173 of 223 changed in V2).
- `lastlog` section in `BASE.ini`: **V2 ONLY**.

## 11. Related documents

- `Docs/KB/legacy/architecture/OVERVIEW.md`, `DEPLOYMENT.md`, `PDF-PRINTING.md`.
- `Docs/KB/legacy/data/MODEL.md`.
- Original source evidence: external: 06-configuration.md, external: 11-deployment-payload-2019.md,
  `Docs/Complete_Reverse_Engineering_Reference_V2.md` §5.
- `Database documentation.md` (root).
- `Docs/ui-labels-verified.md` (authoritative Arabic labels).
