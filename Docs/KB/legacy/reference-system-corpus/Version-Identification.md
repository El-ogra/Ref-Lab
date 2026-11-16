# Version-Identification.md — التعرّف الأولي على نسخ النظام المرجعي

**المرحلة:** Stage 1 — Foundation (تحديد أولي فقط، بلا فحص ملفات)
**مصدر الأدلة:** `REAL_LAB_COPY_Inventory_Report.md` فقط (E-002)
**مخرجات قابلة للمعالجة:** `inventory/version_map.json`, `inventory/files.json`

---

## 1. الخلاصة

**عدد مرشّحي الإصدارات الحاملين لملفات: 2 (CONFIRMED).**

| # | مُعرّف الإصدار | الجذر | الملفات | الحجم | الثقة |
|---|---|---|---:|---:|---|
| **V-ROOT-1.00** | الشجرة المُثبَّتة | `real lab system\` | 364 | 207.78 MiB | STRONGLY SUPPORTED |
| **V-CD-2019** | حزمة القرص 2019 | `real lab system\RL_CD2019Access\` | 473 | 532.61 MiB | CONFIRMED |

**ومرشّح ثالث مرفوض كإصدار:**

| # | مُعرّف | الحالة | السبب |
|---|---|---|---|
| ~~V-DEMO-2011~~ | `DEMO\RLS_Demo2011\RL_CD2019Access\` | **مرفوض — لا يحمل ملفات** | 3 مجلدات فارغة، 0 ملف |

> **مهم:**Instruction الأصلي ينبّه إلى «عدم افتراض وجود نسختين فقط». الفحص
> المتّصل بتقرير الجرد والبيانات الوصفية يُنتج **2** لا أكثر ولا أقل. الاسم
> `RLS_Demo2011` يشير إلى *شيء*، لكن لا دليل ماديًا على وجود ملفات هناك.

---

## 2. قواعد التصنيف المستخدمة

صُنِّفت كل صورة من الـ837 إلى **مكوّن (component)** ثم إلى **إصدار (version)**
بمطابقة بادئات المسار، ثم دُعمت أو خُذِبت بالبيانات الوصفية. القواعد كاملة
في `tools/classify_versions.py`، والنتائج في `inventory/version_map.json`.

| المكوّن | الإصدار | الملفات | الحجم | الثقة | المنطق |
|---|---|---:|---:|---|---|
| `V1-INSTALLED-APP` | V-ROOT-1.00 | 364 | 207.78 MiB | strongly-supported | الشجرة المُثبَّتة: `real lab system\` وجذرها + `bin` + `Cache` + `Data` + `Fonts` + `Images`. الملف `real lab system.exe` فيها يحمل `FileVersion 1.00`. |
| `V2-SYSTEM-STAGING` | V-CD-2019 | 13 | 77.99 MiB | confirmed | `RL_CD2019Access\System\` — منطقة تجهيز تُضيف `real lab system.exe` بـ`FileVersion 1.605`. |
| `V2-DEPLOY-PAYLOAD` | V-CD-2019 | 361 | 104.74 MiB | confirmed | `RL_CD2019Access\{bin,Cache,Data,Fonts,OCX}` — حمولة النشر القابلة للتثبيت. |
| `V2-REDIST-VB6RUNTIME` | V-CD-2019 | 73 | 29.70 MiB | confirmed | `System\datareport_vh\` — حزمة تشغيل VB6 SP6 (Microsoft). |
| `V2-REDIST-PDF` | V-CD-2019 | 7 | 74.87 MiB | confirmed | `System\PDF\` — مُثبِّتات PDF/الطباعة (طرف ثالث). |
| `V2-REDIST-MDAC` | V-CD-2019 | 1 | 7.57 MiB | confirmed | `System\MDAC_TYP\MDAC_TYP.EXE`. |
| `V2-INSTALLMEDIA-WIN7` | V-CD-2019 | 3 | 6.86 MiB | confirmed | `WIN7\` — وسائط تثبيت موجّهة. |
| `V2-INSTALLMEDIA-WINXP` | V-CD-2019 | 3 | 6.71 MiB | confirmed | `WINXP\` — وسائط تثبيت موجّهة. |
| `V2-CD-TOOLS` | V-CD-2019 | 12 | 182.92 MiB | confirmed | أدوات على جذر القرص (AnyDesk، RL_SETUP، Supremo، TeamViewerQS، …). |
| **الإجمالي** | | **837** | **740.41 MiB** | | |

---

## 3. الدليل الحاسم على وجود نسختين

### 3.1 الملف التنفيذي الرئيسي — اختلف رقم الإصدار

| المسار | `FileVersion` | `ProductName` | `Description` | `CompanyName` | الحجم |
|---|---|---|---|---|---:|
| `real lab system\real lab system.exe` | **1.00** | `real lab system` | `ais_soft_group@yahoo` | `RealLab` | 59,150,336 B |
| `real lab system\RL_CD2019Access\System\real lab system.exe` | **1.605** | `real lab system` | **`RLS 1.605`** | `RealLab` | 76,292,096 B |

**VERIFIED FACT:** نفس `ProductName` ونفس `CompanyName`، لكن `FileVersion` مختلف
و`Description` يحمل صراحةً رقم الإصدار (`RLS 1.605`). هذا أقوى دليل متاح في
التقرير.

**STRONGLY SUPPORTED:** الرقمان **1.00** و**1.605** يقعان على مقياس متّسق
(«1» ثم أرقام عشرية)، وهو نمط إصدارات VB6 داخلي. \n

### 3.2 مكوّن تابع: `NOTEX.dll` ترقّى من 1.00 إلى 2.00

| المسار | `FileVersion` | `ProductName` | `Description` | `CompanyName` |
|---|---|---|---|---|
| `real lab system\bin\NOTEX.dll` | 1.00 | `NOTES` | (فارغ) | `ais group` |
| `real lab system\RL_CD2019Access\bin\NOTEX.dll` | **2.00** | **`RLS Notes`** | **`RLS Notes 2.0`** | **`RealLab`** |

**VERIFIED FACT:** نفس اسم الملف، مع تغيّر في الشركة (`ais group` → `RealLab`)
وفي التسمية (`NOTES` → `RLS Notes`) ورقم الإصدار. هذا مؤشّر مستقل عن
الملف التنفيذي، ويؤكّد أن الشجرتين تختلفان ليجريتين زمنيتين مختلفتين.

**ملاحظة على `NOTEXS.dll`:** يظهر في `real lab system\bin\` فقط (لا نسخة في
`RL_CD2019Access\bin\`). هذا فارق بنيوي بين الإصدارين وليس نقصًا في الجرد
(الجرد يسرد كل الملفات).

### 3.3 أداة مرفقة أُعيدت وسمها

| الأداة | V-ROOT-1.00 | V-CD-2019 |
|---|---|---|
| `RLUTool.exe` | 2.00.0001 — `RealLab` | 2.00.0001 — `RealLab` *(متطابقة)* |
| `RLS SQL Database Tools.exe` | 6.00 — `RealLab` | 6.00 — `RealLab` *(متطابقة)* |
| `RLS ACCESS Database Tools.exe` | 3.00 — `ais group` | 3.00 — `ais group` *(متطابقة)* |
| `RealLab E-Mail Sender.exe` | 1.0.0.0 — `ais group` | 1.0.0.0 — `ais group` *(متطابقة)* |
| `real lab timer.exe` | 1.00 — `ais group` | 1.00 — `ais group` *(متطابقة)* |
| `RealLab_Show_Info.exe` | (غير موجود) | **6.00** — `RealLab` |
| `RL_SETUP.exe` | (غير موجود) | **19.00** — `RealLab CD` |

**OBSERVED:** الأدوات المشتركة تحمل نفس الأرقام في الإصدارين — أي أن الشركة
نقلت الأدوات دون تحديث أرقام إصداراتها. هذا نمط شائع، ويجعل الاعتماد على
`FileVersionInfo` وحده **غير كافٍ** لتحديد الإصدار (انظر §6).

---

## 4. رفض «إصدار» DEMO/RLS_Demo2011

**VERIFIED FACT:** لا يوجد **ملف واحد** تحت `DEMO\`.

البرهان:
1. شجرة §2 تسرد `DEMO` ← `RLS_Demo2011` ← `RL_CD2019Access` كثلاثة مجلدات
   متتالية، وبعد `RL_CD2019Access` لا يوجد أي إدخال ملف أو مجلد فرعي.
2. جدول §3.3 (قائمة الملفات الكاملة، 837 صفًا) **لا يحتوي أي مسار يبدأ بـ
   `DEMO\`** — الفحص البرمجي أعطى 0.
3. التحقق من الاتساق: 23 مجلدًا في الشجرة مقابل 19 مشتقًا من المسارات، والفرق
   4 = `DEMO` + `RLS_Demo2011` + `RL_CD2019Access` (تحت DEMO) + `Images`.

**الاستنتاج (VERIFIED FACT):** `DEMO\RLS_Demo2011\RL_CD2019Access` هيكل مجلدات
فارغ. **لا يمكن أن يمثّل إصدارًا** لأن لا مادة دراسية فيه.

**ماذا يُحتمل (HYPOTHESIS — يتطلب دليلًا إضافيًا):** إما أن تفريغ المجلد ناتج
عن نسخ/تنظيف يدوي قبل تسليم النسخة، أو أن المحلّل نسخ محتويات القرص دون
المجلدات الفارغة. في الحالتين، وجود نسخة تجريبية 2011 **ممكن تاريخيًا لكنه غير
مُثبت في هذه الأدلة**. إن أراد المستخدم الحصول عليها، فالملفات غير موجودة
عندنا ولا يمكن استنتاج محتواها.

**قرار المساءلة:** المجلدات الأربعة الفارغة تُسجَّل في
`File-Accountability-Ledger.md` كمدخلات مجلدات (وليس ملفات)، بحالة
`NON_ANALYZABLE — لا يحتوي مادة` مع التبرير.

---

## 5. حدود النسخة داخل V-CD-2019 — subdivide مقصود

**STRONGLY SUPPORTED:** `RL_CD2019Access\` ليست شجرة تشغيل واحدة، بل **حزمة
توزيع** تحوي أربعة أنواع مختلفة من المواد. هذا مهم لأن «إصدارًا واحدًا» بلا
تمييز سيخلط بين:

| النوع | الموقع | الغرض | أولوية التحليل |
|---|---|---|---|
| **حمولة التطبيق** | `bin\`, `Data\`, `Fonts\`, `OCX\` | ما يُثبَّت فعليًا | **عالية** |
| **منطقة تجهيز النظام** | `System\` | النسخة 1.605 من `real lab system.exe` + الدليل + سكربتات النشر | **عالية** |
| **مواد طرف ثالث** | `System\datareport_vh\`, `System\PDF\`, `System\MDAC_TYP\` | VB6 SP6، مكدّس PDF، MDAC | منخفضة (لكنها تُحدّد عقد التشغيل) |
| **وسائط التثبيت** | `WIN7\`, `WINXP\` | حمولة CAB لكل هدف ويندوز | **حرجة لتحديد الإصدار** |

### 5.1 لماذا وسائط التثبيت حرجة

**OBSERVED:** كل من `WIN7\real lab system.CAB` (6.86 MiB) و
`WINXP\real lab system.CAB` (6.71 MiB) **ملفان مبهمةما الصيغة داخليًا** من
منظور الجرد. التقرير لم يستخرج محتوياتهما.

**STRONGLY SUPPORTED:** `SETUP.LST` (مُدرج) هو ملف قائمة تثبيت VB6 Setup
Toolkit، ومن ConventinaliuListingConvention يسرد الملفات التي يثبّتها CAB.
فحصه **في Stage 2** سيخبرنا مباشرةً: هل الـCAB الخاص بـWIN7 يحمل `real lab
system.exe` بنسخة 1.605 أم غيرها؟ هذا هو الدليل القاطع على أن نسختي القرص
(1.00 و1.605) تُشحنان عبر مسارين مختلفين.

**قيد:** حتى ذلك الحين، أيٌّ من الـCAB قد يحتوي بنسخة ثالثة غير معروفة.
**UNKNOWN — يتطلب الأدلة.**

---

## 6. مقارنة ثنائية الإصدارين — من البصمات وحدها

**VERIFIED FACT:** بُنيت مقارنة على 317 مسارًا مشتركًا بين
`real lab system\` و`real lab system\RL_CD2019Access\`.

| النتيجة | العدد | الدلالة |
|---|---:|---|
| مسارات مشتركة | 317 | حمولة القرص تعكس الشجرة المُثبَّتة |
| **بصمة متطابقة** | **127** | ملفات لم تتغير بايتًا |
| **بصمة مختلفة** | **190** | ملفات تغيّرت بايتًا |

### 6.1 توزيع الـ127 المتطابقة (حسب المجلد) — VERIFIED FACT

| المجلد | عدد الملفات ذات البصمة المتطابقة |
|---|---:|
| `Cache\` | 42 (13 ملف Flash EXE + 29 ملف RTF) |
| `Data\` | 53 |
| `Fonts\` | 7 |
| `bin\` | 25 |
| **الإجمالي** | **127** |

ملفات `bin\` المتطابقة (25): `ASNS.log`, `Branch_Data.mdb`, `IDCARD.INI`,
`MCI32.OCX`, `PCRD.jpg`, `PCRF.jpg`, `PCRH.jpg`, `PCRL.jpg`, `PCRM.jpg`,
`Patients_backup.mdb`, `REPORT.dll`, `RLS ACCESS Database Tools.exe`,
`RLS SQL Database Tools.exe`, `RLUTool.exe`, `RealLab E-Mail Sender.exe`,
`SOUND01..04.wav`, `Thumbs.db`, `autorun.cdd`, `calculator.exe`,
`data_connection.udl`, `data_connection1.udl`, `real lab timer.exe`.

### 6.2 توزيع الـ190 المتغيّرة (حسب المجلد) — VERIFIED FACT

| المجلد | عدد الملفات ذات البصمة المختلفة |
|---|---:|
| `Data\` | 174 |
| `bin\` | 16 |
| **الإجمالي** | **190** |

ملفات `bin\` المتغيّرة (16): `BASE.ini`, `Header.jpg`, `Logo.jpg`, `Logo1.jpg`,
`Logo2.jpg`, `MCI32.DEP`, `NOTE.ini`, `NOTEX.dll`, `Patients.mdb`, `SETING.ini`,
`Tailer.jpg`, `Text.jpg`, `Wasl.jpg`, `ZHeader.jpg`, `ZTailer.jpg`, `data.mdb`.

**ملاحظة دقيقة (VERIFIED FACT):** عدد المسارات المشتركة ذات البصمة المختلفة
هو 190، وجميعها تحت `Data\` و`bin\`. أمثلة:

| الملف | V-ROOT-1.00 | V-CD-2019 |
|---|---:|---:|
| `bin\BASE.ini` | 381 B | 200 B |
| `bin\NOTE.ini` | 40 B | 76 B |
| `bin\SETING.ini` | 3,644 B | 4,886 B |
| `bin\Patients.mdb` | 2,531,328 B | 2,682,880 B |
| `bin\data.mdb` | 249,856 B | 266,240 B |

### 6.3 مقارنة إضافية بمطابقة الاسم (لا بالمسار) — VERIFIED FACT

المقارنة في §6.1 تعتمد المسار النسبي، لذا لا تشمل `OCX\` (لأنها في V1 على
جذر الشجرة، وفي V2 داخل مجلد `OCX\`). بإعادة المطابقة **حسب اسم الملف**:

| المجموعة | النتيجة |
|---|---|
| `real lab system\*` (46 ملفًا على الجذر) ↔ `RL_CD2019Access\OCX\*` (43) | **43/43 متطابقة بايتًا** — طبقة ActiveX/VB6 لم تتغيّر إطلاقًا |
| الملففات الثلاثة الأولى في جذر V1 | `real lab system.exe`, `real lab system help.chm`, `real lab Image Gallery.exe` — **لا وجود لها في V2** |
| `Data\` (V1: 227 / V2: 228) | V2 يضيف ملفًا واحدًا: `data_emp\R_S.ini` |
| `bin\` (V1: 42 / V2: 41) | V1 يحتوي ملفًا لا يوجد في V2: `NOTEXS.dll` |

**STRONGLY SUPPORTED:** الفرق بين الإصدارين محصور في:
(a) الملف التنفيذي الرئيسي والدليل وصورة المعرض، (b) طبقة الإعدادات
والكتالوج السريري، (c) إضافة `data_emp\R_S.ini`، (d) حذف `NOTEXS.dll`.
طبقة ActiveX/ActiveX-Control runtime (43 ملفًا) **ثابتة بايتًا**.

### 6.4 ما الذي تعنيه هذه الأرقام

**STRONGLY SUPPORTED:** الـ190 ملفًا المتغيّرًا تقع أساسًا في **طبقة البيانات
والإعدادات**، بينما **طبقة الأدوات (OCX/ActiveX/RTL) ثابتة**. أي أن
التغيير بين 1.00 و1.605 كان **تطوّرًا في نموذج البيانات وقواعد الإعداد**،
لا إعادة كتابة.engineering جديدة كاملة.

**STRONGLY SUPPORTED:** `Patients.mdb` أكبر بـ151,552 bytes في الإصدار الأحدث،
و`data.mdb` أكبر بـ16,384 bytes (وهي مضاعف 4KB — أي صفحة Access إضافية).
وهذا يتّسق مع توسّع مخطط قاعدة البيانات (حقول/جداول/سجلات جديدة).

**تحذير منهجي (يُطبَّق في كل مرحلة):** اختلاف البصمة **لا** يكافئ اختلاف
الوظيفة، وتطابقها **لا** يكافئ التكافؤ الوظيفي. الفحص الفعلي للملفات في
Stage 2 هو الحَكَم.

**مثال تحذيري محدّد من هذه الأدلة:** `Patients_backup.mdb` **متطابق** بين
الإصدارين، بينما `Patients.mdb` **مختلف**. من البديهي أن تبدو النسخة
الاحتياطية «أقدم» ومن الطبيعي أن تختلف عن النسخة الحيّة — لكن التطابق يعني
أن النسخة الاحتياطية **لم تتحدّث مع النسخة الحيّة**. هذا بالضبط النوع الذي
لا يمكن حسمه بالبصمة وحدها.

## 7. الملفات التي لا يمكن إسنادها بثقة

| الفئة | العدد | الحالة | ملاحظة |
|---|---:|---|---|
| ملفات بلا `FileVersionInfo` أصلًا | 709 | `version_id` مُسنَد عبر قاعدة المسار، لا عبر بيانات الإصدار | 705 منها `.ini`/`.rtf`/`.jpg`… التي لا تحمل موارد PE بطبيعة الحال. الثقة للاشتقاق هنا **متوسطة** لأنها قاعدة مسار، لا بيانات مباشرة. |
| `.oca` (20) | 20 | `CONFIRMED` (Microsoft VB6 OCX cache) | لكن `FileVersionInfo` فارغ — موارد الإصدار في `.oca` غير مستخرجة في التقرير. |
| `REPORT.dll` ×2 | 2 | `BLOCKED — شكل مجهول` | 562 bytes لكل منهما، وكل صف بيانات الإصدار فيها فارغ. **مرشّح قوي لغير-PE** (ربما نص أو ملف تفتيح). لا يمكن حسمه من الجرد. |
| `datareport_vh\RICHTX32.OCX` | 1 | `BLOCKED — شكل مجهول` | 675 bytes، نفس الحالة. |
| `TeamViewerQS.exe` | 1 | `CONFIRMED` (مُثبِّت TeamViewer) | بلا موارد إصدار — طبيعي. |
| `Cache\056.rtf` | 2 | `UNKNOWN` | 16.17 MB لكل منهما — أكبر قالب تقرير بفارق كبير (بقية RTF بين 3 KB و25 KB). يحتاج فحصًا. |

**إجمالي الملفات التي لا يمكن إسنادها بإصدار بالبيانات الوصفية وحدها: 709
ملفًا (84.7%)** — سببها بنية: معظمها ملفات `.ini`/`.rtf`/`.jpg` لا تحمل
`FileVersionInfo` أصلًا. إسنادها يعتمد على **سياق المسار** (وهو سُلّم إسناد
قوي في بنية محزومة معروفة) وليس على بيانات داخل الملف.

---

## 8. جدول التصنيف النهائي

| مُعرّف | التصنيف | الثقة | الدليل |
|---|---|---|---|
| `real lab system\real lab system.exe` (1.00) | **CONFIRMED** | عالية | `FileVersion 1.00` + `Description: ais_soft_group@yahoo` + `CompanyName: RealLab` |
| `real lab system\RL_CD2019Access\System\real lab system.exe` (1.605) | **CONFIRMED** | عالية | `FileVersion 1.605` + `Description: RLS 1.605` |
| `real lab system\bin\*` | **STRONGLY SUPPORTED** | متوسطة-عالية | موقعه في الشجرة المُثبَّتة + بصمات مختلفة عن نظيره في القرص |
| `real lab system\Data\*` | **STRONGLY SUPPORTED** | متوسطة-عالية | نفس المنطق + بصمات مختلفة لكل ملف |
| `real lab system\Cache\*` | **STRONGLY SUPPORTED** | متوسطة | الشجرة المُثبَّتة + بصمات **متطابقة** مع حمولة القرص |
| `real lab system\Fonts\*` | **STRONGLY SUPPORTED** | متوسطة | نفس المنطق |
| `real lab system\*.OCX/.OCA/.DLL/.TLB` | **STRONGLY SUPPORTED** | متوسطة-عالية | بيئة تشغيل VB6/ActiveX موثّقة بالإصدارات (6.01.9782 = VB6 SP6) |
| `real lab system\Images\` | **NON-ANALYZABLE** | — | مجلد فارغ |
| `DEMO\*\*` | **NON-ANALYZABLE** | — | 3 مجلدات فارغة |
| `RL_CD2019Access\bin\Data\Fonts\OCX` | **CONFIRMED** | عالية | حمولة نشر الـCD |
| `RL_CD2019Access\System\*` | **CONFIRMED** | عالية | منطقة تجهيز + v1.605 |
| `RL_CD2019Access\System\datareport_vh\*` | **CONFIRMED** | عالية | Microsoft VB6 SP6 redistributable |
| `RL_CD2019Access\System\PDF\*` | **CONFIRMED** | عالية | مُثبِّتات طرف ثالث موثّقة |
| `RL_CD2019Access\WIN7\`, `WINXP\` | **CONFIRMED** | عالية | وسائط تثبيت، لكن **محتوى CAB مجهول** |
| `RL_CD2019Access\{AnyDesk,RL_SETUP,Supremo,TeamViewerQS,RealLab_Show_Info}.exe` | **CONFIRMED** | عالية | أدوات القرص |
| `REPORT.dll` ×2, `datareport_vh\RICHTX32.OCX` | **BLOCKED** | — |_shape غير معروف؛ يُحل في Stage 2 بـ`file`/`pefile` |
| محتوى `WIN7/WINXP *.CAB` | **UNKNOWN** | — | يُحل في Stage 2 بـ`cabextract` + قراءة `SETUP.LST` |
| أي «إصدار ثالث» محتمل داخل CABs | **UNKNOWN** | — | يُحل في Stage 2 |

---

## 9. الأدلة الإضافية المطلوبة لحسم الغموض

| # | الغموض | الدليل المطلوب | الأداة |
|---|---|---|---|
| A-1 | هل الـCAB في WIN7/WINXP يحملان 1.00 أم 1.605 أم نسخة أخرى؟ | استخراج CAB + قراءة `SETUP.LST` | `cabextract`, `7z l` |
| A-2 | ما طبيعة `REPORT.dll` (562 B) و`RICHTX32.OCX` (675 B)؟ | فحص الصيغة الداخلية | `file`, `pefile`, `xxd` |
| A-3 | هل `real lab system.exe` (56/72 MiB) ملف VB6 عادي أم حاوية ذاتية الفك؟ | فحص PE + الموارد + مسحCARVE | `pefile`, `lief`, `binwalk` |
| A-4 | هل نسخ `.ini` المختلف بين الإصدارين تعكس قواعد سريرية مختلفة فعلًا؟ | استخراج النص وDiff | `dos2unix` + مقارنة نصية |
| A-5 | ما مخطط قاعدة البيانات الفعلي في كل إصدار؟ | فحص Jet read-only | `mdb-tables`, `mdb-schema` |
| A-6 | ما الفروق بين `Patients.mdb` بين الإصدارين؟ | تصدير الجداول + مقارنة | `mdb-export` (قراءة فقط) |
| A-7 | هل يوجد إصدار ثالث داخل `Cache\056.rtf` أو داخل CABs؟ | فحص RTF + CAB | `unrtf`, `cabextract` |
| A-8 | ما الخوارزمية/السلوك الفعلي؟ | تحليل تفصيلي | يُصرَّح به في Stage 2 |

---

## 10. ترتيب التنفيذ المقترح (يُعتمد في Stage 2)

| الترتيب | الإصدار | الدفعة | السبب |
|---|---|---|---|
| 1 | V-ROOT-1.00 | BATCH-01 | الملف التنفيذي الرئيسي + الدليل + بيئة التشغيل |
| 2 | V-ROOT-1.00 | BATCH-02 | قواعد البيانات + الإعدادات + الكتالوج السريري |
| 3 | V-ROOT-1.00 | BATCH-03 | قوالب التقارير RTF |
| 4 | V-ROOT-1.00 | BATCH-04 | نسخ Flash (منخفضة الأولوية) |
| 5 | V-CD-2019 | BATCH-05 | `System\` + v1.605 + أدوات القرص |
| 6 | V-CD-2019 | BATCH-07 | حمولة النشر + أدوات الدعم |
| 7 | V-CD-2019 | BATCH-08/09 | قوالب التقارير + Flash |
| 8 | V-CD-2019 | BATCH-10 | وسائط التثبيت + حزم التوزيع (A-1) |
| 9 | V-CD-2019 | BATCH-11 | مكدّس PDF (منخفضة الأولوية) |

**قاعدة:** يُكتمل إصدار واحد قبل الانتقال للذي يليه، إلا إذا ظهر تبعية تقنية
صراحة (مثل A-1 الذي يتطلب BATCH-10 لفهم BATCH-05). أي استثناء يُوثَّق ويُطلب
تفويضه.
