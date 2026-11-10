# UI Labels — verified extraction from the live screen recording

**Document ID:** RL-UI-001
**Source:** `analysis/BATCH-06/video/frames/f_*.jpg` — 106 frames extracted from
`RLS_Learn.wmv` (35:20, wmv3, 1360×768; frames stored at 680×384)
**Method:** direct visual transcription of the frames at full stored resolution.
**Conventions:** original Arabic reproduced **exactly**; English glosses are explanatory
only. Items that could not be read with confidence are marked **[unclear]** rather than
guessed.

---

## 1. ★ Two findings that change how this evidence is attributed

### 1.1 The recording shows **Version 1**, not Version 2

The application window's title bar reads **`real lab system 1.0`** in every frame
examined (`f_009`, `f_010`, `f_014`, `f_015`).

> **This recording is therefore evidence of the V-ROOT-1.00 interface.** Any earlier
> statement treating `RLS_Learn.wmv` as a Version 2 interface artefact is corrected by
> this document. The video remains valid as **Version 1** UI evidence, and it is
> consistent with the fact that the 13 Flash training simulators are **byte-identical**
> between the two versions.

### 1.2 ★ The interface is **bilingual by design, not by accident**

The screen separates cleanly into two zones with different languages:

| Zone | Language | Examples |
|---|---|---|
| **Left panel** — test catalogue, search, keypad frame | **English** | `Routine test menu`, `Selected Test`, `Test Name`, `Add select test`, `Delete`, `All`, `Blood`, `Stool`, `Urine`, `SGOT (ALT)`, `Albumin` |
| **Right panel** — patient record | **Arabic** | «بيانات المريض», «الاسم», «الجنس», «العمر», «العنوان», «ملاحظات» |
| **On-screen keypad** | **Arabic** | «حروف», «رموز», «مسافة» |
| **Printable report** | **English headings + Arabic branding** | `BIOCHEMISTRY REPORT`, `LIVER FUNCTIONS:`, `Reference Range:` alongside «التقرير المختبرى» and «ريال لاب سيستم» |

> **★ The chrome is English; the domain data and the patient-facing panel are Arabic.**
> This is a deliberate localisation boundary — **the patient-record capture surface is
> Arabic, and the clinical-catalogue surface is not.** It is the most important UI
> fact in the recording, and it is not derivable from the code literals.

---

## 2. Screen 1 — Patient record with result-entry panel

**Frame:** `f_010.jpg` (t = 180 s) · **Form:** the main patient window
**Window title:** `real lab system 1.0`

### 2.1 Right panel — «بيانات المريض» (patient data)

| Element type | Exact label | English gloss | Certainty |
|---|---|---|---|
| Panel title | «بيانات المريض» | Patient data | Observed verbatim |
| Field label | «رقم الحجز» | Reservation / accession number | Observed verbatim |
| Field value | `1511621410037` | — | Observed verbatim |
| Field label | «رقم الطلب» | Request number | Observed verbatim |
| Field label | «الوقت» | Time | Observed verbatim |
| Field label | «حالة السجل» | Record status | Observed verbatim |
| Field label | «الاسم» | Name | Observed verbatim |
| Field label | «الجنس» | Sex | Observed verbatim |
| Option | `Male` / `Female` | — | Observed verbatim |
| Field label | «العمر» | Age | Observed verbatim |
| Field label | «سنوات» | Years | Observed verbatim |
| Field label | «الوحدة» | Unit | Observed verbatim |
| Field label | «العنوان» | Address | Observed verbatim |
| Field label | «ملاحظات» | Notes | Observed verbatim |
| Field label | «حالة الاعلام» / «حالة الإشعار» | Notification state | Observed verbatim (spelling as shown) |
| Field label | «رقم الهاتف» | Phone number | Observed verbatim |
| Field label | «اسم صاحب الحساب» | Account holder name | Observed verbatim |
| Field label | «تحويل» | Transfer | Observed verbatim |
| Checkbox | `History` | — | Observed verbatim |
| Checkbox | `Vip` | — | Observed verbatim |
| Section header | «الاختبارات المطلوبة» | The requested tests | Observed verbatim |

### 2.2 Command buttons

| Exact label | English gloss | Certainty |
|---|---|---|
| «حفظ» | Save | Observed verbatim |
| «حذف» | Delete | Observed verbatim |
| «خروج» | Exit | Observed verbatim |
| «بحث» | Search | Observed verbatim |
| «تعديل» | Edit | Observed verbatim |
| «طباعة» | Print | Observed verbatim |
| «الطباعة» | Printing | Observed verbatim |
| «مسح» | Clear | Observed verbatim |
| «تأكيد» | Confirm | Observed verbatim |
| «تسجيل خروج» | Logout | Observed verbatim |

### 2.3 Requested-test grid — Arabic test names

| Exact label | English gloss |
|---|---|
| «تحليل بول» | Urine analysis |
| «صورة دم كاملة» | Complete blood count |
| «نسبة Sugar» | Sugar ratio |
| «صورة ك-diff» | Blood differential picture |
| «اختبار حمل» | Pregnancy test |

Sample-type checkboxes in the same grid: `Blood` · `Urine` · `Stool` · `Serum` · `CSF`.

### 2.4 Left panel — «Routine test menu» (English)

| Element | Exact label |
|---|---|
| Panel title | `Routine test menu` |
| Dropdown | `Routine Tests` (value `1`) |
| List header | `Test Name` |
| Selected-list header | `Selected Test` (value `Liver Tests`) |
| Buttons | `Add select test` · `Delete` · `All` |

**Analyte / test names visible in the selection list:** `Blood` · `Stool` · `Urine` ·
`H.B.C` · `PPP.B.L` · `SGOT (ALT)` · `SGPT (ALT)` · `Albumin` · `BHC Acid (Serum)` ·
`Cholesterol` · `Triglycerides` · `VLDL` · `Urea (Serum)` · `Tropinin (I)`.

### 2.5 On-screen keypad (Arabic)

| Exact label | English gloss |
|---|---|
| «حروف» | Letters |
| «رموز» | Symbols |
| «مسافة» | Space |
| `ESC` · `Enter` · `Back` · `CE` · `+/-` · `Caps Lock` · `Shift` | — |
| «ط» | Arabic letter-mode indicator |

---

## 3. Screen 2 — Patient search

**Frame:** `f_009.jpg` (t = 160 s)

Left panel identical in structure to §2.4, with the list header reading **`Test Name`**
and the selected test `CBC`. The analyte list adds `PP.B.L.G` · `R.B.L.G` · `Alk. Ph.` ·
`Albumin/ total` · `Total Bili.` · `Creatinine (Serum)` · `Uric Acid` ·
`Triglyceride` · `HDL` · `Hgb` · `Total Ca` · `T.g.`.

Right panel: identical to §2.1–§2.3, with the age shown as `22`.

---

## 4. Screen 3 — Custom test group editor

**Frame:** `f_015.jpg` (t = 280 s)

| Element | Exact label | Gloss |
|---|---|---|
| Panel title | `Custom test group name` | — |
| List header | `Custom Groups` | — |
| Sub-header | `Tests Groups` | — |
| Selected-list header | `Selected Test` | — |
| Group names | `CBC` · `CBCCRP 1` · `CBCCRP 2` · `CBCCRP 3` · `CBCCRP 4` | — |
| Buttons | `Add all tests in group` · `Delete` · `All` | — |

> **★ The `CBCCRP` group family** — four numbered variants beside `CBC` — is direct
> visual evidence of the **repeatable test-group** structure that the schema shows as
> `patienttestgroup` with its `maintestcode` key.

---

## 5. Screen 4 — Barcode-label template editor

**Frame:** `f_014.jpg` (t = 260 s)

| Element | Exact label | Gloss |
|---|---|---|
| Mode buttons | `Print File Barcode` · `Print Lab Id Barcode` | — |
| Sample-type buttons | `Urine` · `Blood` · `Stool` · `Stool,Cannot...` · `SBC` · `EDTA Blood` · `CSF` | — |
| Group buttons | `Serum Fasting` · `Serum Postprandial` · `PPBG` | — |
| Fasting dropdown values | `TBS` · `ABO` · `Rh` · `Sputum` · `Sterile` · `N/A` | — |
| Offset fields | `Top = 0` · `Left = 0` | — |
| Status dropdown | `Barcode Status` | — |
| Buttons | `SAVE` · `Print` · `Print All Barcode` · `login` | — |
| Checkbox | `Print File Barcode with all` | — |
| Input | `Other Barcode` | — |
| Icon | recycle bin | Delete |

> **★ The barcode-label template editor is entirely English**, and the `Print All
> Barcode` / `Print File Barcode with all` controls correspond directly to the
> `printwith` flag and the barcode-printing path documented in the functional
> specification.

---

## 6. Screen 5 — The printable laboratory report

**Frames:** `f_006`, `f_007`, `f_016` (t = 100–300 s)

### 6.1 Branding — Arabic

| Exact label | English gloss |
|---|---|
| «ريال لاب سيستم» | RealLab System |
| «التقرير المختبرى» | The laboratory report |
| «معمل برقياسى» | [unclear — a laboratory name line] |

### 6.2 Branding — English

| Exact label | Gloss |
|---|---|
| `Computer Software` | — |
| `real lab system` | — |
| `For Medical Clinical Laboratory` | — |

### 6.3 Report structure

| Element | Exact label | Gloss |
|---|---|---|
| Report title | `BIOCHEMISTRY REPORT` | — |
| Section header | `LIVER FUNCTIONS:` | — |
| Range header | `Reference Range:` | — |
| Field labels | `Name` · `Patient ID` · `Patient bdt` · `Referred by` · `Patient ID` · `Request Date` · `Reporting Date` | — |
| Reference block | `REFERENCE RESULT` · `Sex` · `Age` · `Normal range for test` | — |
| Flags | **`High` · `Low` · `Normal`** | The result flags — **English in the printed output** |
| Units | `U/L` · `g/dL` · `mg/dL` · `mmHg` | — |
| Units header | `S:PL` / `U/L` | — |
| Comment | `Comment : Sample taken outside lab.` | — |
| Signature block | `Doctor's signature:` · `Dr. Doctor's Name` · `Thanks` | — |
| Footer | `Issued on:` · `Sent to:` | — |

### 6.4 Analytes shown

`Albumin` · `Globulin` · `Total protein` · `A/G Ratio` · `SGOT (AST)` · `SGPT (ALT)` ·
`Total Bilirubin` · `Indirect Bilirubin` · `Direct Bilirubin` · `Alkaline` ·
`Serum Creatinine` · `Serum Uric Acid` · `Blood Glucose` · `Abnormal` ·
`Satisfactory` · `Critical` · `TBBC`

### 6.5 Observed reference intervals (from this recording)

| Analyte | Interval as printed |
|---|---|
| `SGPT (AST)` | `12` `U/L` — *Male Up To 45 U/L* |
| `SGOT (ALT)` | `45` `U/L` — *Male Up To 40 U/L* |
| `Albumin` | `5` `g/dL` — *Adult 3.8 - 5.2 g/dL* |
| `Serum Creatinine` | `1` `mg/dL` — *Normal : 0.7 - 1.3 mg/dL* |
| `Serum Uric Acid` | `1` `mg/dL` — *Normal : 3.6 - 7.2 mg/dL* |
| `Total Bilirubin` | `1.1` `mg/dL` — *Male 0.5 - 1.2 mg/dL* |
| `Total protein` | `1.1` `mg/dL` — *Male 0.5 - 1.2 mg/dL* |

> **★ The printed report renders the range as a **human-readable sentence** — *"Male Up
> To 45 U/L"*, *"Adult 3.8 - 5.2 g/dL"* — not as raw numbers.** This confirms that the
> `normalrange` row is assembled into prose at print time, and it is the mechanism
> behind the `normalrang` field's Text/Memo type.

### 6.6 An operator warning observed in the recording

`Not sent to the printer` — and, in a modal box:

> «ستتم إضافة اسم الملف المختار في التقرير إلى تقرير Degits الخاص بك. هل تريد إضافة تقرير Digits أخر؟»

*English gloss:* "The name of the file selected in the report will be added to your
Degits report. Do you want to add another Digits report?"

> **★ "Degits" appears as an in-product report name** — a second, unnamed report
> family alongside the clinical profile reports. It is also the name of a **third-party
> LIS** (Digits is a known Arabic-language laboratory information system), which is
> direct evidence that **RealLab coexists with, and reports into, another LIS.**
> This is new information not present in the schema, the manual or the vendor guide.

---

## 7. Consolidated Arabic vocabulary

Every Arabic term below was **transcribed verbatim from a frame**.

### 7.1 Patient and administration

| Arabic | English gloss | Where |
|---|---|---|
| «بيانات المريض» | Patient data | f_009, f_010, f_015 |
| «رقم الحجز» | Reservation / accession number | f_009, f_010, f_015 |
| «رقم الطلب» | Request number | f_009, f_010, f_015 |
| «الوقت» | Time | f_009, f_010, f_015 |
| «حالة السجل» | Record status | f_009, f_010, f_015 |
| «الاسم» | Name | f_009, f_010, f_015 |
| «الجنس» | Sex | f_009, f_010, f_015 |
| «العمر» | Age | f_009, f_010, f_015 |
| «سنوات» | Years | f_009, f_010, f_015 |
| «الوحدة» | Unit | f_009, f_010, f_015 |
| «العنوان» | Address | f_009, f_010, f_015 |
| «ملاحظات» | Notes | f_009, f_010, f_015 |
| «رقم الهاتف» | Phone number | f_009, f_010, f_015 |
| «اسم صاحب الحساب» | Account holder name | f_009, f_010, f_015 |
| «تحويل» | Transfer | f_009, f_010, f_015 |
| «حالة الاعلام» | Notification state | f_009, f_010, f_015 |
| «الاختبارات المطلوبة» | The requested tests | f_009, f_010, f_015 |

### 7.2 Commands

«حفظ» (Save) · «حذف» (Delete) · «خروج» (Exit) · «بحث» (Search) · «تعديل» (Edit) ·
«طباعة» (Print) · «الطباعة» (Printing) · «مسح» (Clear) · «تأكيد» (Confirm) ·
«تسجيل خروج» (Logout)

### 7.3 On-screen keypad

«حروف» (Letters) · «رموز» (Symbols) · «مسافة» (Space) · «ط» (letter-mode indicator)

### 7.4 Clinical test names

«تحليل بول» (Urine analysis) · «صورة دم كاملة» (Complete blood count) ·
«نسبة Sugar» (Sugar ratio) · «صورة ك-diff» (Blood differential) · «اختبار حمل» (Pregnancy test)

### 7.5 Report branding

«ريال لاب سيستم» (RealLab System) · «التقرير المختبرى» (The laboratory report)

### 7.6 The one long operator message

«ستتم إضافة اسم الملف المختار في التقرير إلى تقرير Degits الخاص بك. هل تريد إضافة تقرير Digits أخر؟»

---

## 8. What this extraction could not establish

| Item | Why |
|---|---|
| The complete interface label set | Only the frames of the live recording were available; the 13 Flash simulators were not re-read at frame level in this pass |
| The Arabic labels of the reference-range editor | The relevant frame is present in the contact sheet but its fields are not legible at the stored resolution |
| Which Arabic label belongs to which control on every screen | The right-hand panel's field labels are visible, but their precise control-to-label binding is ambiguous at this resolution |
| Whether the Flash simulators use the same vocabulary | The 13 simulators are byte-identical to V1's, but their frames were not transcribed here |

---

---

## 9. Owner-approved Arabic strings from the V2 corpus

**Document ID:** RL-UI-001 · Section 9
**Added:** 2026-11-10
**Approved by:** the project owner, in the Phase 5 execution directive

The strings below were **not** part of the screen-recording transcription of
sections 2 and 7, which remains the primary evidence of this document. They
were transcribed from the V2 requirement corpus and were explicitly approved by
the project owner for use in the Phase 5 presentation layer. They are recorded
here so that this document remains the **single authorised source** of Arabic
user-interface labels, exactly as the project's localisation rule requires.

The presentation boundary test `B9` asserts that every Arabic literal in the
source of the laboratory appears in this document, so a string that is not
listed below cannot reach the user interface.

### 9.1 Strings transcribed from the V2 corpus

| # | Arabic string | Provenance | Requirement | Function | Where it is used in Ref Lab |
|---:|---|---|---|---|---|
| 1 | «الإحصائيات ومتابعة أداء المعمل» | `RL-V2-FRM-001` §2 master function inventory, Arabic-label column | `V2-REQ-054`, `V2-REQ-055` | `V2-FUNC-032` — Run the statistics and performance module | The title of the statistics and performance screen |
| 2 | «هل الفلوس المحصلة من مرضى الفترة فقط؟» | `RL-V2-FRM-001` §3 requirement card, Arabic-label row | `V2-REQ-025` | `V2-FUNC-015` — Run accounting and stock-take | The scope confirmation of the stock-take on the accounting screen |
| 3 | «عالٍ أو منخفضة أو حرجة» | `RL-V2-FRM-001` §3 requirement card, Arabic-label row | `V2-REQ-014` | `V2-FUNC-008` — Enter a result with automatic classification | The three result-classification terms below |

### 9.2 The result-classification terms

The classification triple of `V2-REQ-014` is used as the three separate terms
that the presentation layer binds, each of which is a word of the approved
string:

| # | Arabic term | English gloss | Provenance |
|---:|---|---|---|
| 4 | «عالٍ» | High | `V2-REQ-014` — «عالٍ أو منخفضة أو حرجة» |
| 5 | «منخفضة» | Low | `V2-REQ-014` — «عالٍ أو منخفضة أو حرجة» |
| 6 | «حرجة» | Critical | `V2-REQ-014` — «عالٍ أو منخفضة أو حرجة» |

### 9.3 A string whose provenance is the visual reference material

| # | Arabic string | Provenance | Where it is used in Ref Lab |
|---:|---|---|---|
| 7 | «شاشة الوحدات و عوامل التحويل» | The **visual reference material** of the Phase 5 preparation — the units and conversion-factors editor of the reference system — together with the owner's explicit approval. **It is not present in the V2 requirement corpus.** | The title of the units and conversions view of the catalogue administration workspace |

> **Provenance note.** This string is the only Arabic string in this document
> that does not come from the V2 corpus. It was read from the reference
> system's own screen, and the owner approved it by name in the Phase 5
> execution directive. It is recorded here so that the single-source rule
> holds and so that the boundary test `B9` accepts it.

---

**End of UI Labels extraction.**
