# START-HERE — نقطة الدخول الوحيدة

> اقرأ هذا الملف **وحده** قبل أي عمل. كل ما تحتاجه للوصول إلى أي كود موجود **بعده**.
> **الكود هو المرجع.** هذه الوثائق طبقة تنقّل، لا مصدر بديل.

`El-ogra/Ref-Lab.git` · مراجعة `f4daec94396796076082442e1cdcb81917293910` · الفهرسة: `scripts/kb_build.py` · التحقق: `scripts/kb_check.py`

---

## 1. ثلاثة عوالم — أيّها تقرأ؟

| العالم | أين | المرجع فيه | متى |
|---|---|---|---|
| **التنفيذ الحالي** | `src/` · `tests/` | **كل شيء** | دائماً |
| **قاعدة المعرفة** | `Docs/KB/` | النيّة · القيود · الفخاخ | دائماً |
| **الأدلة القديمة** | `evidence/` · `ReferenceIndex/` · `Docs/*_V2.md` | **لماذا** وُجدت قاعدة | عند سؤال «لماذا؟» فقط |

⚠ العالم الثالث يوثّق نظامين قديمين (`V-ROOT-1.00` و `V-CD-2019` — VB6 + Access) هما **مصدر** إعادة البناء. اقرأه للبحث عن **إثبات**، لا لتحديد موقع كود.

## 2. الطبقات الأربع

```
Presentation ──▶ Application ──▶ Domain ◀── Infrastructure
        └──────────── نقطة التركيب وحدها ────────────┘
```

| الطبقة | ما يحمله | قاعدة صارمة |
|---|---|---|
| `src/RefLab.Domain` | الكيانات · كائنات القيمة · **القواعد** | **صفر** مرجع حزمة (مغطاة باختبار) |
| `src/RefLab.Application` | 73 حالة استخدام · `MediatR` · التحقق | لا يعرف `DbSet` ولا WPF |
| `src/RefLab.Infrastructure` | EF Core · SQL Server · QuestPDF · PBKDF2 | لا تتسرّب للأعلى |
| `src/RefLab.Presentation` | 14 شاشة + لوحتان · MVVM | لا تلمس الحاوية |

الأرقام الحيّة في `Docs/KB/generated/` — لا من الذاكرة: **73** حالة استخدام · **27** جدول · **14** شاشة · **14** مجالاً · **15** صلاحية · **4** سلوكيات · **18** حارساً.

## 3. خريطة المجالات — المفتاح الأساسي

| المجال | حالات | الشاشات | الملف |
|---|---:|---|---|
| Auditing — التدقيق والحضور | 2 | Audit · Reception | `Docs/KB/features/Auditing.md` |
| Billing — الفوترة والتحصيل | 7 | Accounting · PartiesPricing | `Docs/KB/features/Billing.md` |
| Catalogue — كتالوج الفحوص | 11 | CatalogueAdministration · TestCatalogue · PatientWork | `Docs/KB/features/Catalogue.md` |
| Configuration — إعدادات المعمل | 3 | LaboratorySettings · ReportSetup | `Docs/KB/features/Configuration.md` |
| Derivation — الاشتقاقات | 4 | لوحة Derivation | `Docs/KB/features/Derivation.md` |
| Identity — المستخدمون والصلاحيات | 6 | SignIn · OperatorAdministration | `Docs/KB/features/Identity.md` |
| Microbiology — الأحياء الدقيقة | 4 | Microbiology | `Docs/KB/features/Microbiology.md` |
| Parties — الأطباء والأسعار | 3 | PartiesPricing | `Docs/KB/features/Parties.md` |
| Patients — المرضى والطلبيات | 9 | PatientWork · Reception | `Docs/KB/features/Patients.md` |
| Reporting — التقارير والطباعة | 6 | ReportSetup | `Docs/KB/features/Reporting.md` |
| Results — النتائج | 9 | PatientWork · لوحة MethodSheet | `Docs/KB/features/Results.md` |
| SendOut — الإرسال لمعامل أخرى | 5 | SendOut | `Docs/KB/features/SendOut.md` |
| Statistics — الإحصائيات | 2 | Statistics | `Docs/KB/features/Statistics.md` |
| Worksheets — أوراق العمل | 2 | — | `Docs/KB/features/Worksheets.md` |

## 4. جدول التوجيه: نوع المهمة ← الوثائق ← الكود

| المهمة | اقرأ بالترتيب | ثم افتح |
|---|---|---|
| إضافة/تعديل حالة استخدام | `features/<المجال>.md` ← `CONVENTIONS.md` | `src/RefLab.Application/Features/<المجال>/` |
| تغيير قاعدة عمل أو حساب سريري | `DOMAIN-RULES.md` ← `features/<المجال>.md` | `src/RefLab.Domain/Rules/` |
| تغيير مخطط قاعدة البيانات | `DATA-MODEL.md` ← `OPERATIONS.md` | `src/RefLab.Infrastructure/Persistence/` |
| تعديل شاشة أو واجهة | `features/<المجال>.md` ← `CONVENTIONS.md` | `src/RefLab.Presentation/` |
| إصلاح عيب | `CONVENTIONS.md` ← `features/<المجال>.md` | الملف الذي يحدّده `generated/usecase-index.csv` |
| تغيير عبر طبقتين أو أكثر | `ARCHITECTURE.md` ← `CONVENTIONS.md` | كل الطبقات المشاركة |
| كتابة أو تعديل اختبار | `features/<المجال>.md` §6 ← `generated/usecase-index.csv` | `tests/…/Features/<المجال>/` |
| تتبّع متطلب قديم | `generated/usecase-index.csv` ← `Docs/Functionality_Requirements_Matrix_V2.md` | `src/` |

**الحدّ الأدنى للقراءة:** هذا الملف + `CONVENTIONS.md` + **ملف مجال واحد** ← ثم الكود. لا شيء غير ذلك.

## 5. ثمانية ثوابت لا تُكسر

1. **الكود هو المرجع** — الوثائق تنقّل لا بديل.
2. **قواعد العمل في `Domain` فقط** ولا تُكرَّر أبداً.
3. **لا تُعدِّل ترحيلاً مُطبَّقاً** — أنشئ ترحيلاً جديداً.
4. **لا تُطبّق الترحيل تلقائياً** — تطبيق المخطط **صلاحية المالك**.
5. **`RefLab.Application` يُخفي `System.Windows.Application`** داخل طبقة العرض — اكتب النوع كاملاً وإلا كسر البناء (CS0118).
6. **MediatR يبقى على 12.x** — الفرع 13+ يتطلب ترخيصاً تجارياً.
7. **النصوص العربية لا تُخترع** — مصدرها `Docs/ui-labels-verified.md` وحده.
8. **المعمل موقع واحد** — لا فرع ولا عمود فرع. (الموضع 9 من مُعرّف الطلب (13 خانة) كان رمز الفرع؛ صار الآن جزءاً من المقطع الأوسط.)

## 6. البناء والاختبار

```powershell
dotnet restore RefLab.sln
dotnet build   RefLab.sln -c Release     # 0 تحذير، 0 خطأ
dotnet test    RefLab.sln -c Release     # 722 ناجح (Presentation.Tests يحتاج Windows)
```

التفاصيل والقيود: `Docs/KB/OPERATIONS.md`.

## 7. قائمة «لا تقرأ»

❌ **`ReferenceIndex/`** — فهرس الأرشيف القديم، **ليس** فهرس الكود (`ReferenceIndex/README.md`).
❌ **`evidence/`** · `Database documentation.md` · `Documenting the Executable File.md` · `Docs/*_V2.md` — لتتبّع الإثبات فقط.
❌ **`IDEA.md`** — مواصفة المشروع الأصلية، لا أداة تنقّل.
❌ **`Docs/RL-P*.md`** — سجلات مراحل تاريخية، عند سؤال «متى ولماذا؟».
❌ **كل `Docs/KB/` دفعة واحدة** — اقرأ ما يوجّهك إليه §4 فقط.

> الفهرس المولّد **قابل للبحث**: `grep` على `generated/usecase-index.csv` يجد الملف والصلاحية والمُتحقّق والاختبار في خطوة واحدة.

## 8. بقية الوثائق

| الوثيقة | المحتوى |
|---|---|
| `Docs/KB/CONVENTIONS.md` | **الفخاخ** — أخطاء كلّفت وقتاً حقيقياً. اقرأه ثانياً. |
| `Docs/KB/ARCHITECTURE.md` | الطبقات · نقطة التركيب · المسار · الحُرّاس · وصفات الإضافة |
| `Docs/KB/OPERATIONS.md` | البناء · الاختبار · التشغيل · الترحيلات · صلاحيات المالك |
| `Docs/KB/DOMAIN-RULES.md` | القواعد الثماني في `Domain/Rules/` بصيغها وإثباتها |
| `Docs/KB/DATA-MODEL.md` | 27 جدولاً · العلاقات · كائنات القيمة المملوكة · الترحيلات |
| `Docs/KB/features/*.md` | ملف لكل مجال: الغرض · الحالات · الملفات · الفخاخ |
| `Docs/KB/history/PHASES.md` | ماذا نُجزّم في كل مرحلة |

## 9. كيف تتأكد أن هذه المعرفة حيّة؟

```powershell
python scripts/kb_build.py    # أعِد توليد الفهارس من الكود
python scripts/kb_check.py    # بصمة · انجراف · تغطية · روابط · ميزانيات
```

`kb_check.py` **يفشل بخطأ غير صفري** إذا قَدُم أي فهرس أو انكسر أي مسار. هذه هي الضمانة الوحيدة ضد الشرح الذي يقارب الكود بدل أن يصفّه.
