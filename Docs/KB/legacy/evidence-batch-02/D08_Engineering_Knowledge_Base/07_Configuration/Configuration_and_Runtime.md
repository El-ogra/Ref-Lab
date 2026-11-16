# Configuration Architecture and Runtime Behavior — RealLab v1.605

## Configuration files

- `BASE.ini` — `Initial Catalog=Patients`, `DataSource` (REAL_LAB/DESKELTEL/REALLAB_AIS).
- `data_connection*.udl` — `Provider=SQLOLEDB.1`.
- `SETING.ini`, `IDCARD.INI`, `NOTE.ini`.
- `Data\*.ini` (146 files) — per-test report config: `reportdata`/`setupx`/`FORM`/`report` sections; reportmaintitle, subtitles, symbols, fonts, print flags, left/width/top/bottom values.
- `Data\*.ini` keys: `testtype` (r/pcr/pnt), `printer` (default `HP LaserJet 1100 (MS)`).

## Report main titles (from 146 INI files)

MICROBIOLOGY REPORT, MOLECULAR BIOLOGY REPORT, HEMATOLOGY REPORT, IMMUNOLOGY REPORT, ENDOCRINOLOGY REPORT, CYTOPATHOLOGY REPORT, SEROLOGY REPORT, BIOCHEMISTRY REPORT, "Immunoglobulin E - Allergy Screen", plus generic "Report".

## Fonts referenced

Arial (675), Tahoma (597), Times New Roman (328) occurrences across INI files; barcode fonts FREE3OF9, WBCW_UPCEAN, WBCWS_UPCEAN; Arabic fonts MCS Taybah, ais lab, ais symbol; Digiface.

## Runtime behavior

- **Startup:** runs its own schema-migration engine (DDL) against the clinical DB.
- **Report layout is per-workstation** (U-034) — identical tests can print differently on different machines.
- **Default operator account:** `admin`/`admin` (U-001).
- **Authorisation:** grant1..grant12 per operator + per-section numeric print passwords.

## Deployment

- `Setup*.bat` scripts execute unverified payloads by absolute path; attach DB from hardcoded `D:\Lab_data` (U-006).
- VS6 Setup Wizard; Bullzip PDF Printer (evaluation mode, expired cert — U-033).
- Avira AV engine descriptors (orphaned — U-015).
- unrar 3.50 (U-019); unsigned local system DLL copies (U-020).