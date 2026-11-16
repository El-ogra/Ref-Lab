# UI and Event Flows (Static Reconstruction — NOT Runtime Observed)

## 1. Evidence base

| Evidence | What it provides |
|---|---|
| `objects-full__[analysis_BATCH-04_vb6_v1605].txt` | 210 VB6 objects: 132 Forms, 66 DataReports, 9 Modules, 3 Classes; per-object method counts; `labmain` at index 0 with 71 methods |
| `forms-blob__[analysis_BATCH-04_vb6_v1605].txt`, `forms-blob.txt` | VB6 form/control descriptor blobs |
| `vb6-objects.json`, `vb6-structure.json` | parsed object model |
| `capability-crossref__[analysis_BATCH-04_help_v1605].md`, `help-topics.csv`, `help-manual-ar.md` | official help topics mapped to objects/capabilities |
| `ui-labels-verified.md` | the *only* authoritative source of Arabic UI labels (AGENTS.md rule 7) — labels are quoted, never invented |
| `frames-index.csv`, `template-corpus-228.json`, `056.txt`, `070.txt` etc. | OCR/reference interface inventory and report templates |
| `10-training-video.md`, `Stage-5-V-CD-2019-Cumulative-Report.md` | training-video-derived screen inventory (`labmain` main menu, 184 controls) |
| `005`/`006` etc. `.txt` dumps | extracted string/literal context |

## 2. Structural UI model (VERIFIED)

```
labmain  (main menu form, 71 methods; ~184 controls per training evidence)
 ├── patientmain (84 methods) ──► addpatient (192 methods), patient (form), patientview (33), workpaper
 ├── Result forms (e.g. Resultliver) ──► result entry/verification/printing
 ├── frm_money, moneypaid ──► billing/cashier
 ├── Report* forms + 66 DataReport designer objects ──► preview/print
 ├── labtolab (32 methods) ──► send-out to another laboratory
 ├── Microbiology form(s) ──► culture/antibiotic workflow
 ├── labmainTools (6 methods) ──► utilities; documented F7 shortcut
 └── 9 standard modules: labdb, labdb1, labfunction, printerselect, BCWFonts, …
```

The object names above are all present in the object dumps / prior reports; the *navigation edges* (the tree) are INFERRED from naming and capability cross-references, not from observed event handlers.

## 3. Event-flow reconstruction (INFERRED)

VB6 is event-driven. The recovered method names include typical VB6 event handlers (`Form_Load`, `cmdOK_Click`, etc.) — control descriptors such as `cmdOK` and `Command1` are explicitly noted in `Consolidated_Report.md` §224. The defensible flow model is:

1. **Form load** → read INI configuration and open data connection (see `Startup_And_Initialization.md`).
2. **User action on a command button/menu** → VB6 event handler on the active form.
3. **Handler → data module** (`labdb`, `labdb1`) → SQL literal or bound recordset operation.
4. **Result binding** → `MSADODC.OCX`/`MSDBRPTR.DLL`/DataReport object → screen grid (`DBGRID32.OCX`, `MSFLXGRD.OCX`) or printer.
5. **Write side** → recordset `AddNew`/`Update` (no literal `INSERT`/`UPDATE` exists in the 1,797-statement corpus — see `State_Transitions.md` §4).

**Confidence:** STRONGLY_SUPPORTED for steps 1–2 and 5 (structure + SQL evidence); INFERRED for the exact handler-to-method mapping in steps 3–4. The original method names of the 1,605 build are partly unavailable: prior work records that only 1,268 of 8,026 declared methods had resolvable addresses, and that names were not stored for the remainder (`Consolidated_Report.md` §224). Any per-form "click → method" table would therefore be fabricated and is deliberately not produced.

## 4. Presentation assets

- 29 RTF method sheets (identical across both versions) — report/worksheet templates.
- 13 Macromedia Flash Player 8 projector simulations (`001.exe`–`012.exe`, `007a.exe`) — training/interface simulations, not application logic.
- 66 DataReport designer objects — the actual VB6 reporting mechanism.
- `datareport_vh_*.png`, `pcr_group.png`, `stationery_group.png`, `.emf`/`.wmf` pictures — UI/report resources.
- Arabic strings come from `ui-labels-verified.md` and the INI/REPORT.dll resources; no label is invented in this repository.

## 5. Explicitly not established

- Live navigation sequence, modal dialogs, focus behavior, keyboard shortcuts beyond documented F7.
- Whether all 132 forms are reachable in the shipped build.
- Exact event names bound to each control.
- Any screenshot-derived flow beyond what prior OCR/reference-interface documents record.

No runtime UI observation exists (see `Runtime_Observations.md`), so this document is a reconstruction, not an observation.
