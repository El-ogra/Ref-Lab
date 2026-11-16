# Data Model — V-ROOT-1.00 (derived from SQL literals)

**المصدر:** 1337 عبارة SQL مستخرجة من الملف التنفيذي `real lab system.exe` (v1.00) عبر أداة `vb6decompiler`.
**جداول مُشار إليها:** 100 · **أعمدة مُسمّاة مؤهَّلة:** 480

> **ملاحظة منهجية:** هذه ليست مخططًا (DDL) — النظام لا يحتوي عبارات DDL. ما يلي **استنتاج بنيوي عالي الثقة** من 1,337 استعلامًا فعليًا: الجدول مُشار إليه مرة أو أكثر في SQL فعلي، وأعمدته من `table.column` المذكورة صريحًا.

## 1. الجداول الجوهرية

| الجدول | عدد الاستشهادات | SELECT | DELETE | أعمدة مُسمّاة | مثال على استخدام |
|---|---:|---:|---:|---:|---|
| `patientinfo` | 307 | 290 | 0 | 22 | Select patientid,recepnote from patientinfo where patientid= ' |
| `patienttest` | 216 | 185 | 0 | 31 | SELECT * FROM patienttest where patientid= ' |
| `testtable` | 352 | 346 | 0 | 23 | SHAPE {SELECT subgrouptable.subgroupcode, subgrouptable.subgroupname FROM subgrouptable GROUP BY subgrouptable |
| `normalrange` | 52 | 52 | 0 | 4 | SELECT normalrange.normalrang, normalrange.testunit, testtable.testabrev, testtable.testname FROM normalrange  |
| `payment` | 13 | 12 | 0 | 6 | Select * from payment where patientid = ' |
| `gehats` | 16 | 16 | 0 | 2 | SELECT * FROM Gehats where gehahname= ' |
| `subgrouptable` | 10 | 9 | 0 | 2 | SHAPE {SELECT subgrouptable.subgroupcode, subgrouptable.subgroupname FROM subgrouptable GROUP BY subgrouptable |
| `pricelist` | 11 | 9 | 0 | 4 | SELECT * FROM pricelist where testsimp=' |
| `useraccounts` | 6 | 6 | 0 | 0 | select * FROM useraccounts where usersnum= |
| `labbranch` | 7 | 7 | 0 | 0 | SELECT * FROM labbranch order by branchname |
| `history` | 12 | 11 | 0 | 0 | SELECT *  FROM History WHERE selector = 1 and labid= ' |

## 2. جداول النتائج لكل تحليل (نمط «جدول لكل بروفايل»)

النتائج لا تُخزَّن في جدول واحد؛ بل لكل اختبار/بروفايل **جدول مستقل** مرتبط بـ`patientid`. هذا هو سبب وجود 227 ملف `.ini` في `Data\` و45 تقرير DataReport.

| الجدول | استشهادات | أعمدة مُسمّاة |
|---|---:|---:|
| `cbc` | 10 | 18 |
| `abuse` | 9 | 11 |
| `bloodgases` | 9 | 9 |
| `cardiac` | 9 | 8 |
| `coagfactors` | 9 | 10 |
| `crp` | 9 | 10 |
| `drugs` | 9 | 13 |
| `fertlity` | 9 | 16 |
| `glucosetolerance` | 9 | 7 |
| `hgbectrophoresis` | 9 | 7 |
| `hormones` | 9 | 15 |
| `immunoglobulin` | 9 | 9 |
| `iron` | 9 | 8 |
| `kidney` | 9 | 13 |
| `lipids` | 9 | 10 |
| `liver` | 9 | 13 |
| `pancreas` | 9 | 12 |
| `pregnancy` | 9 | 5 |
| `proelectrophoresis` | 9 | 7 |
| `pt` | 9 | 9 |
| `renalfailure` | 9 | 10 |
| `sugar` | 9 | 5 |
| `torch` | 9 | 12 |
| `tumors` | 9 | 13 |
| `vitamins` | 9 | 14 |
| `cultureanti` | 7 | 2 |
| `immunology` | 7 | 18 |
| `suprarenal` | 7 | 15 |
| `electrolytes` | 5 | 19 |
| `trace` | 5 | 24 |
| `abo` | 3 | 0 |
| `bilh` | 3 | 0 |
| `crossmat` | 3 | 0 |
| `csft` | 3 | 0 |
| `esr` | 3 | 0 |
| `hepatitis` | 3 | 0 |
| `semen` | 3 | 20 |
| `bonemar` | 2 | 0 |
| `postcoitalt` | 2 | 0 |
| `prostaticx` | 2 | 0 |
| `stone` | 2 | 0 |
| `tuberculint` | 2 | 0 |
| `urine` | 2 | 0 |
| `widal` | 2 | 0 |
| `zn` | 2 | 0 |
| `widal2` | 1 | 0 |

## 3. جداول الدعم والتشغيل

| الجدول | استشهادات |
|---|---:|
| `comments` | 25 |
| `sharekathesab` | 15 |
| `patients` | 12 |
| `patienttotal` | 12 |
| `phone` | 11 |
| `titless` | 10 |
| `customtesttable` | 9 |
| `hba1c` | 8 |
| `ratios` | 8 |
| `barcodes` | 7 |
| `loggroup` | 7 |
| `seana` | 7 |
| `testunits` | 7 |
| `abbreviations` | 6 |
| `customgrouptable` | 6 |
| `shopping` | 6 |
| `testname` | 6 |
| `wasl` | 6 |
| `anyreport` | 5 |
| `instrument` | 5 |
| `sharekat` | 5 |
| `noteic` | 4 |
| `tasgel` | 4 |
| `antibiotic` | 3 |
| `cultures` | 3 |
| `constanttable` | 2 |
| `converts` | 2 |
| `daytip` | 2 |
| `patient` | 2 |
| `pictures` | 2 |
| `pricemenu` | 2 |
| `ptc` | 2 |
| `sarfeda3` | 2 |
| `stool` | 2 |
| `culturesec` | 1 |
| `immunolo` | 1 |
| `immunolog` | 1 |
| `p` | 1 |
| `pa` | 1 |
| `patien` | 1 |
| `supr` | 1 |
| `supra` | 1 |
| `t` | 1 |

## 4. الأعمدة الأكثر استخدامًا (مؤشرات تصميمية)

| العمود | المراجع | الدلالة |
|---|---:|---|
| `patientinfo.patientid` | 273 | المفتاح الأساسي للمريض (و المركّب: `mid(patientid,10,3)` لمعرّف عرض مختصر) |
| `patientinfo.patientnamear` | 233 | اسم المريض بالعربية (يكشف أن قاعدة البيانات تحمل بيانات عربية) |
| `patientinfo.labid` | 198 | المختبر/الفرع الذي أنشأ السجل |
| `patienttest.patientid` | 71 | ربط النتيجة بالمريض |
| `patientinfo.requestdate` | 58 | تاريخ الطلب — محور كل التقارير-periodic |
| `patientinfo.referredbyname` | 32 | جهة الإحالة (الطبيب/المختبر) — أساس التقارير والعمولات |
| `patienttest.requestdate` | 24 | — |
| `patienttest.testsimp` | 24 | اسم التحليل المبسّط (الظاهر في التقارير) |
| `patientinfo.patientsex` | 23 | الجنس — مدخل في حساب النطاق المرجعي |
| `patientinfo.patientageunit` | 23 | وحدة العمر (سنة/يوم/شهر) — ثلاثة نطاقات عمرية |
| `patientinfo.patientage` | 23 | العمر — مدخل في حساب النطاق المرجعي |
| `patientinfo.barcode` | 22 | الباركود المطبوع على الوعاء |
| `patienttest.testcode` | 21 | رمز التحليل |
| `patientinfo.completes` | 16 | حالة اكتمال الطلب |
| `normalrange.testcode` | 16 | التحليل المرتبط بالنطاق |
| `patientinfo.totalpricedisc` | 13 | إجمالي السعر بعد الخصم |
| `patienttest.resultt` | 12 | قيمة النتيجة (result + T) |
| `patienttest.testpriceout` | 12 | سعر التحليل قبل الخصم |
| `cbc.patientid` | 12 | — |
| `patienttest.resultcaseprint` | 11 | حالة الطباعة لكل حالة |
| `testtable.testabrev` | 11 | الاختصار المعروض |
| `testtable.testname` | 11 | اسم التحليل |
| `testtable.testcode` | 10 | — |
| `testtable.testsimp` | 10 | — |
| `subgrouptable.subgroupname` | 9 | — |
| `patientinfo.elmadfoo3` | 9 | المدفوع (اسم عمودي مُرحَّل بالحروف العربية) |
| `patienttest.otherlabname` | 9 | المختبر الذي أُرسلت إليه العينة |
| `patienttest.testpriceoutmadfo` | 9 | سعر التحليل بعد الخصم |
| `patientinfo.referredbysefa` | 9 | رمز جهة الإحالة |
| `subgrouptable.subgroupcode` | 8 | — |
| `normalrange.normalrang` | 8 | نص النطاق المرجعي المطبوع |
| `normalrange.testunit` | 8 | وحدة القياس المعروضة في التقرير |
| `normalrange.sex` | 8 | الجنس الذي ينطبق عليه النطاق |
| `liver.patientid` | 8 | — |
| `sugar.patientid` | 8 | — |
| `kidney.patientid` | 8 | — |
| `lipids.patientid` | 8 | — |
| `cbc.requestdate` | 8 | — |
| `torch.patientid` | 8 | — |
| `crp.patientid` | 8 | — |

## 5. دورة حياة النتيجة (مُستنتجة من الأعمدة)

| العمود | الدلالة | دليل الدليل الرسمي |
|---|---|---|
| `patienttest.resultfinsh` | اكتمل إدخال النتيجة | زر «حفظ» (F9) |
| `patienttest.resultreview` | تمت المراجعة والاعتماد | F8 «اعتماد تحاليل المريض المختارة تمت مراجعتها» |
| `patienttest.resultprint` | تمت الطباعة | F12 «طباعة تقرير النتيجة» |
| `patienttest.give` | سُلِّمت النتيجة للمريض | 005.htm «تسليم نتائج التحاليل للمريض» |
| `patienttest.sentout` / `.send` | التحليل يُرسَل خارجًا / أُرسل فعلًا | 010.htm |
| `patienttest.edituser` / `.editdate` | من عدّل ومتى | — |
| `patienttest.prevuser` / `.prevdate` | من راجع ومتى | — |
| `patienttest.printuser` / `.printdate` | من طبع ومتى | — |
| `patienttest.arrangnum` | رقم ترتيب التحليل في الطلب | — |
| `patientinfo.pricemethod` | `Lab To Lab` / `Individual` / `Free` | 010.htm |

## 6. آليات Jet الخاصة

- **`SHAPE {...} AS alias APPEND ({...})`** — استعلامات اتحاد Jet، استُخرجت 20 استعلامًا منها 21 استخدامًا لـ`APPEND` (تقارير مجمّعة).
- **`#` كـdate literal** — صيغة تاريخ Jet داخل SQL (`where requestdate >= #`).
- **`mid(patientid,10,3)`** — استخراج مقطع من مفتاح مركّب.
- **`patientnamear <> '<<< Blank >>>'`** — قيمة حراسة للمريض غير المسمّى.
- **`SHAPE ... GROUP BY ... ORDER BY`** — بناء تقارير مجمّعة على مستوى المخزون.