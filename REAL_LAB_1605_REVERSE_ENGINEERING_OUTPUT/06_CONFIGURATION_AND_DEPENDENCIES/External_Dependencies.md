# External Dependencies — REAL LAB SYSTEM v1.605

**Scope.** Third-party runtimes, controls, services and bundled utilities evidenced
in the v1.605 deliverable set. **Explicit boundary:** remote-access tools shipped on
the CD (AnyDesk, TeamViewerQS, Supremo) are **NOT part of target application logic**.
No binary was executed and no network request was made.

**Confidence legend.** VERIFIED / STRONGLY_SUPPORTED / INFERRED / UNVERIFIED /
CONFLICTED / UNKNOWN.

---

## 1. Evidence files consulted

- `dependency-inventory.json` — `runtime_dependencies`, `external_components`,
  `framework_dependencies`, `external_services`.
- `control-typelib-surface.json` — keys for 5 OCA sets in two trees
  (`ChamaleonButton`, `RgheedButtonControlV2`, `XP_BUTTONS`, `vkUserControlsXP`,
  `Redsoul911XPCOmponents`).
- `*.ocx.json`, `*.oca.json`, `*.tlb.json`, `*.dep.json` in `D:\Collective work`
  (33 such files — enumerated in `Application_Data_Access_Architecture.md` §1 set).
- `12-install-media.md` §§1, 10; `08-deployment-installer.md` §§1, 3, 6;
  `BATCH-03.md`, `BATCH-07.md`, `BATCH-11.md`, `09-presentation-library.md`,
  `13-pdf-print-architecture.md`.

---

## 2. Microsoft VB6 / MDAC redistributables (VERIFIED)

| Component | Purpose | Evidence |
| --- | --- | --- |
| `MSADODC.OCX` | ADO Data Control — database access | `dependency-inventory.json` `external_components[0]`; `MSADODC.OCX.json` |
| `msado27.tlb` | ADO 2.7 type library (2.81.1132.0) | `msado27.tlb.json` |
| `MSDATGRD.OCX`, `MSDATLST.OCX` | ADO Data Grid / Data List — bound result entry | `dependency-inventory.json`; `MSDATGRD.OCX.json`, `MSDATLST.OCX.json` |
| `MSDBRPTR.DLL` | Microsoft DataReport runtime | `MSDBRPTR.DLL.json` |
| `MSFLXGRD.OCX`, `DBGRID32.OCX` | grid controls | respective `.ocx.json` |
| `MSCHRT20.OCX` | charting | `MSCHRT20.OCX.json` |
| `MSCOMCTL.OCX` (19? see note), `MSCOMCT2.OCX`, `TABCTL32.OCX` | common controls, dates, tabs | respective `.ocx.json` / `.oca.json` |
| `COMDLG32.OCX` | common dialogs | `COMDLG32.OCX.json` |
| `RICHTX32.OCX`, `RICHED32.DLL` | rich-text editing | `RICHTX32.OCX.json`, `RICHTX32.oca.json`, `RICHED32.DLL.json` |
| `MSCOMM32.OCX` | serial I/O for analysers | `MSCOMM32.OCX.json`, `MSCOMM32.oca.json` |
| `MSWINSCK.OCX` | Winsock / TCP | `MSWINSCK.OCX.json`, `MSWINSCK.oca.json`, `MSWINSCK.DEP.json` |
| `MCI32.OCX` | multimedia | `MCI32.OCX.json`, `MCI32.DEP.json` |
| `MSSTDFMT.DLL`, `MSBIND.DLL`, `VB6STKIT.DLL`, `VB5DB.DLL` | formatting, data binding, setup | `12-install-media.md` §1; `MSSTDFMT.DLL.json`, `MSBIND.DLL.json` |
| `OLEAUT32.DLL`, `OLEPRO32.DLL`, `ASYCFILT.DLL`, `COMCAT.DLL`, `STDOLE2.TLB` | OLE automation infrastructure | `12-install-media.md` §1; corresponding `.json` files |

`12-install-media.md` §1 enumerates **19 ActiveX controls** in the CAB (including
the Microsoft set plus the four third-party packs below). Note: one earlier
placeholder described `MSCOMCTL.OCX` as "19"; the actual CAB count of ActiveX
components is 19 — the number is a component count, not a per-file count.

---

## 3. Third-party ActiveX / OCX controls (VERIFIED names, INFERRED purpose)

From `dependency-inventory.json` `external_components` and `control-typelib-surface.json`:

| Control | Purpose per evidence | Notes |
| --- | --- | --- |
| `ChamaleonButton.ocx` (+ `.oca`) | third-party button control | `ChamaleonButton.ocx.json`, `.oca.json` |
| `RgheedButtonControlV2.ocx` (+ `.oca`) | third-party button control | `RgheedButtonControlV2.ocx.json`, `.oca.json` |
| `XP_BUTTONS.ocx` (+ `.oca`) | themed XP buttons | `XP_BUTTONS.ocx.json`, `.oca.json` |
| `vkUserControlsXP.oca` | user-control pack | **OCA only**; no `vkUserControlsXP.ocx.json` in the corpus and no VB6 object of that name — presence VERIFIED as typelib surface, runtime role UNVERIFIED |
| `Redsoul911XPCOmponents.ocx` (+ `.oca`) | third-party control pack | `Redsoul911XPCOmponents.ocx.json`, `.oca.json` |

`control-typelib-surface.json` lists these five in two roots (top-level and
`RL_CD2019Access/OCX/`), i.e. 10 keys. All require `$(DLLSelfRegister)`-style
registration; `SETUP.LST` specifies 24 self-registrations overall
(`12-install-media.md` §2.2).

---

## 4. Reporting / "Crystal" question (VERIFIED negative for Crystal)

| Component | Evidence | Status |
| --- | --- | --- |
| `MSDBRPTR.DLL` (Microsoft DataReport) | `MSDBRPTR.DLL.json`; `objects-full__[…_v1605].txt` lists ~60 `Report*` objects typed `datareport`; `datareport_vh` resources | VERIFIED |
| `REPORT.dll` | Vendor report component in `dependency-inventory.json`; on disk it is a **plain-text INI** with `[reportdata]`/`color1..19` (`06-configuration.md` §6) | VERIFIED |
| `NOTEX.dll` | Vendor note component | `dependency-inventory.json`; Ghidra artefacts `RL_CD2019Access__bin__NOTEX.*` |
| Crystal Reports (`CRAXDRT`, `crystal`, `.rpt`) | no such token in `dependency-inventory.json`, `SETUP.LST` analysis or `rl-setup-analysis.md` finding 8 (MSDBRPTR/DataReport token hits 0 in RL_SETUP — different claim) | **UNVERIFIED** — no Crystal dependency evidenced; do not assume one |

The correct statement is: reporting is built on **VB6 DataReport + vendor REPORT.dll +
INI/RTF templates**, not on Crystal. Any Crystal claim would be an invention.

---

## 5. PDF / print stack (VERIFIED file identities from `12-install-media.md` §10.1)

| Payload | Size (B) | Identified product | Role |
| --- | ---: | --- | --- |
| `System\PDF\dotNetFx40_Full_x86_x64.exe` | 48,111,808 | .NET Framework 4.0 | prerequisite for Bullzip stub |
| `System\PDF\Setup_BullzipPDFPrinter_11_1_0_2600_PRO_EXP.exe` | 10,806,881 | **Bullzip PDF Printer 11.1.0.2600** | virtual PDF printer; `RL_Show.pdf` Producer is "PDF Printer / www.bullzip.com" |
| `System\PDF\gslite.exe` | 14,786,945 | **Ghostscript Lite 9.14.17** | PDF engine behind Bullzip |
| `System\PDF\xpdfsetup.exe` | 1,037,344 | **Xpdf Lite 3.3.4** | PDF utilities (includes `pdftops.exe`) |
| `System\PDF\pdfpowertool_setup.exe` | 2,181,282 | **PDF Power Tool 4.0.0.11** | PDF post-processing |
| `System\PDF\SetupP.bat`, `SetupFN4.bat` | 566 / 153 | installer batch scripts | invoked by `RL_SETUP.exe` |

`13-pdf-print-architecture.md` §3 states these components carry **no first-party
logic** and finds **0 call references** from the VB6 executable to
Ghostscript/Xpdf/PDF Power Tool — they are reached through the printer driver.
`12-install-media.md` §10.1 also notes three of the shipped PDF installers are
unsigned / evaluation-mode with an expired certificate (finding ISS-33 context).
No installer was run.

---

## 6. Flash projection runtime (VERIFIED, third-party)

`BATCH-03.md` and `09-presentation-library.md`: **13 byte-identical copies** of
Macromedia **Flash Player 8.0 r22** (`SAFlashPlayer.exe`) re-used as projector stubs
(`Cache.exe`…`012.exe`, PE overlay, ~1,687,552-byte tail) each carrying an SWF
(`FWS`, v5) overlay. Role: animation host for the interactive report viewer. This
is a bundled third-party runtime, not first-party lab logic. No Flash content was
executed.

---

## 7. External services (VERIFIED strings; NO credentials or PII)

| Service | Evidence | Boundary note |
| --- | --- | --- |
| SMTP relay | `dependency-inventory.json` `external_services[0]`; `SmtpClient` in `RealLab E-Mail Sender.exe` | host supplied by operator |
| `smsmisr.com` SMS gateway | `dependency-inventory.json` `external_services[1]`; `database-and-data-access-analysis.md` §5 | cleartext HTTP with credential fields in the query string — **values withheld**; `language=3` selects Arabic |
| Facebook vendor pages | `dependency-inventory.json` `external_services[2]` | opened in Chrome; marketing, not clinical |
| Automatic Hematology Analyzer BF6500 | `06-configuration.md` §2; `dependency-inventory.json` `external_services[3]` | instrument database path in UDL — vendor artefact |
| SQL Server backup devices | two `EXEC sp_*` literals (VAs `66FCB2`, `66FD56`) | see `Database_Transactions.md` |
| Remote hosted SQL `DB_87427_patients` | `dependency-inventory.json` `database_dependencies[2]` | connection constructed but **never queried** in the .NET component (0 CommandText/Execute*) — dead reference |

---

## 8. Vendor / system components (VERIFIED names; internal behaviour UNVERIFIED)

- `NOTEX.dll`, `REPORT.dll` — vendor note/report components; native analysis
  artefacts exist (`RL_CD2019Access__System__datareport_vh__*`).
- `unrar.dll` / embedded `unrar` 3.50 code (`RL_CD2019Access__OCX__unrar.*`) —
  archive extraction support; not lab logic.
- `Avira` engine remnants (`avipbb.cat`, `avipbb.sys`) — prior reports identify an
  orphaned/vestigial antivirus descriptor; not required by application logic.
- Unsigned copies of system DLLs (`12-install-media.md` §9) — packaging concern,
  not a functional dependency.

---

## 9. Bundled remote-access tools — explicitly NOT part of target logic

| File | Size (B) | Evidence |
| --- | ---: | --- |
| `AnyDesk.exe` | 3,668,944 | `BATCH-07.md` row 2; `ingest-report__[batches_BATCH-07].md` MATCH |
| `TeamViewerQS.exe` | 17,696,568 | `BATCH-07.md` row 284 |
| `Supremo.exe` | 7,861,016 | `BATCH-07.md` row 283 |

`BATCH-07.md` §17 classifies these as "third-party payload, inventoried and
accounted for, but **functionally peripheral to the laboratory product**";
`cross-version-matrix.md` row 41 groups them as remote-support tools.
`08-deployment-installer.md` §5 mentions RDP port 3389 for branch access — also
infrastructure, not lab logic. **None of these three is referenced by any of the
1,797 SQL literals, by the VB6 object inventory, or by `SETUP.LST`** (VERIFIED
negative against the cited inventories). They must not be modelled as modules of
REAL LAB SYSTEM v1.605.

---

## 10. Confidence and gaps

| Area | Confidence |
| --- | ---: |
| File names, sizes, hashes, product versions | VERIFIED |
| Component purposes from `dependency-inventory.json` | VERIFIED as prior-report claims |
| Third-party control purposes | INFERRED from naming/typelib surface |
| Absence of Crystal dependency | UNVERIFIED (negative evidence only) |
| Runtime behaviour of vendor DLLs | UNVERIFIED (no execution) |
| Remote tools not part of logic | VERIFIED against inventories; STRONGLY_SUPPORTED overall |

Not evidenced: licence terms for the third-party controls; whether all 24 self-
registrations succeed on a clean machine; which PDF installers actually ran at a
given site; any communication between the VB6 client and the remote-access tools.
