# OPERATIONS — البناء والاختبار والتشغيل

> الأرقام هنا **مُقيسة** على المراجعة `f4daec94396796076082442e1cdcb81917293910`
> وليست مُقتبسة من وثيقة أقدم.

---

## 1. المتطلبات

| المتطلب | الإصدار | ملاحظة |
|---|---|---|
| Windows | 10 بناء 1809 فأحدث | `net8.0-windows10.0.17763.0` |
| Visual Studio | 2022 (17.11+) مع عبء **.NET desktop development** | |
| .NET SDK | 8.0.100 فأحدث | `global.json` يسمح بأي إصدار أحدث (`rollForward: latestMajor`) |
| SQL Server | 2019 فأحدث | مطلوب من Phase 4 |
| `dotnet-ef` | 8.0.x | `dotnet tool install --global dotnet-ef --version 8.*` |
| Python | 3.9+ | لأدوات `scripts/` — بلا اعتماديات خارجية |

## 2. البناء والاختبار

```powershell
dotnet restore RefLab.sln
dotnet build   RefLab.sln -c Debug
dotnet build   RefLab.sln -c Release
dotnet test    RefLab.sln -c Release
```

### النتيجة المُقيسة على المراجعة المثبّتة

| البند | النتيجة |
|---|---|
| `dotnet build -c Release` | **نجح — 0 تحذير، 0 خطأ** (~115 ثانية) |
| `RefLab.Domain.Tests` | **317 ناجح** · 0 فاشل |
| `RefLab.Application.Tests` | **277 ناجح** · 0 فاشل |
| `RefLab.Infrastructure.Tests` | **65 ناجح** · 0 فاشل |
| `RefLab.Integration.Tests` | **63 ناجح** · 0 فاشل |
| `RefLab.Presentation.Tests` | **16 اختباراً — لم تُشغَّل**: تحتاج Windows |
| **الإجمالي القابل للتشغيل** | **722 ناجح، 0 فاشل** |

> **`TreatWarningsAsErrors=true`** مفعّل في `Directory.Build.props` لكل مشروع.
> أي تحذير = فشل بناء. لا تُطفئه.

### تشغيل جزء واحد فقط

```powershell
dotnet test tests/RefLab.Domain.Tests -c Release
dotnet test tests/RefLab.Application.Tests -c Release --filter "FullyQualifiedName~Results"
```

## 3. تشغيل التطبيق

```powershell
$env:REFLAB_CONNECTION_STRING = "Server=.;Database=RefLabDb;Trusted_Connection=True;TrustServerCertificate=True"
dotnet run --project src/RefLab.Presentation -c Release
```

**ترتيب قراءة سلسلة الاتصال** (في `CompositionRoot.ResolveConnectionString`):

1. المُعامل المُمرَّر إلى `BuildHost(connectionString)`
2. المتغيّر البيئي `REFLAB_CONNECTION_STRING`
3. الاحتياطي: `Server=(localdb)\mssqllocaldb;Database=RefLab;Trusted_Connection=True;TrustServerCertificate=True`

> ⚠ **سلسلة الاتصال لا تُشحن أبداً في حزمة تسليم.** حُددها على جهاز المالك.

## 4. الترحيلات (migrations)

```powershell
# سرد الترحيلات
dotnet ef migrations list --project src/RefLab.Infrastructure/RefLab.Infrastructure.csproj `
                          --startup-project src/RefLab.Infrastructure/RefLab.Infrastructure.csproj

# إنشاء ترحيل جديد (لا يطبّقه)
dotnet ef migrations add <Name> --project src/RefLab.Infrastructure/RefLab.Infrastructure.csproj `
                               --startup-project src/RefLab.Infrastructure/RefLab.Infrastructure.csproj

# التطبيق — قرار المالك فقط
dotnet ef database update --project src/RefLab.Infrastructure/RefLab.Infrastructure.csproj `
                          --startup-project src/RefLab.Infrastructure/RefLab.Infrastructure.csproj
```

**الترحيلات الثلاثة الحالية:**

| الترحيل | المحتوى |
|---|---|
| `20261110103458_InitialCreate` | الجداول الـ 27 + المفاتيح الأجنبية + الفهارس |
| `20261111083113_AddReportFooterAndLayoutText` | `LayoutText` · `ReportFooterText` |
| `20261111122338_AddReportBrandingAndSampleOrigin` | العلامة التجارية + `SampleTakenOutsideLab` + التوقيع + الشعار |

> ❌ **لا تُعدِّل ترحيلاً مُطبَّقاً.** ❌ **لا يطبّق التطبيق الترحيل تلقائياً** —
> `DatabaseInitializer` مسجَّل لكنه لا يُحلّ أبداً، ويحرسه اختبار.

## 5. أدوات المعرفة في المشروع

```powershell
# توليد فهارس قاعدة المعرفة من الكود
python scripts/kb_build.py

# التحقق: بصمة · انجراف · تغطية · روابط · ميزانيات
python scripts/kb_check.py

# جرد حالات الاستخدام (حارس الانجراف)
python scripts/generate_use_case_inventory.py

# التحقق من حزمة تسليم مقابل بياناتها
python tools/verify_manifest.py --root <extracted-root> --manifest <manifest.md>
```

| الأداة | الدور | يدخل | يخرج |
|---|---|---|---|
| `scripts/kb_build.py` | توليد فهارس `Docs/KB/generated/` | `src/` + `tests/` | 7 CSV/JSON + جداول ملفات المجالات |
| `scripts/kb_check.py` | بوابة التحقق | قاعدة المعرفة + الكود | رمز خروج غير صفري عند أي خلل |
| `scripts/generate_use_case_inventory.py` | جرد حالات الاستخدام | `src/RefLab.Application/Features/` | `Docs/USE_CASE_INVENTORY.md` |
| `tools/verify_manifest.py` | تحقّق من حزمة تسليم | مجلد مستخرج + بيان | PASS/FAIL لكل ملف |

**دورة العمل:** عدّل الكود ← `kb_build.py` ← `kb_check.py` ← `dotnet build` ← `dotnet test`.

## 6. حارس انجراف حالات الاستخدام

`scripts/generate_use_case_inventory.py` يفشل بخطأ غير صفري إذا:

| رمز الخروج | السبب |
|---|---|
| `1` | عدد حالات الاستخدام ≠ `EXPECTED_TOTAL` |
| `2` | حالة استخدام بلا اختبار يشير إليها |
| `3` | حالة استخدام بلا ربط في قاموس `LEGACY` |

**عند إضافة حالة استخدام: غيّر `EXPECTED_TOTAL` وأضف الربط في `LEGACY`.**
(حُسمت هذه الحارسة في 2026-11-14: كانت 70 بينما الكود فيه 73.)

## 7. قيود بيئة السحابة (Linux / WSL / CI)

| المسألة | الأثر | الحل |
|---|---|---|
| `RefLab.Presentation.Tests` | لا تعمل على Linux (مضيف WPF) | شغّلها على Windows |
| `dotnet test` بـ SDK 9 | لا يشغّل مضيف `net8.0` | `DOTNET_ROLL_FORWARD=LatestMajor` |
| بناء `RefLab.Presentation` | ينجح على Linux | `EnableWindowsTargeting=true` مفعّل |
| `dotnet ef` | يحتاج Windows للعمل الكامل | استخدم Windows للترحيلات |

**بناء WPF على Linux نجح فعلاً** (0 تحذير، 0 خطأ) — `EnableWindowsTargeting` يتيحها.

## 8. تحقّق ما بعد التغيير — قائمة

```powershell
dotnet build RefLab.sln -c Release        # 0 تحذير، 0 خطأ
dotnet test  RefLab.sln -c Release        # كل المجموعات ناجحة
python scripts/kb_build.py                # أعِد توليد الفهارس
python scripts/kb_check.py                # SUCCESS
```

## 9. قرارات المالك المطلوبة قبل أي تغيير حسّاس

| القرار | متى يلزم |
|---|---|
| تطبيق ترحيل على قاعدة المعمل | أي تغيير مخطط |
| إضافة جدول أو عمود | أي تغيير مخطط |
| نص عربي جديد | أي نص جديد في الواجهة — مع توثيقه في `Docs/ui-labels-verified.md` |
| إضافة حزمة `PackageReference` | أي إضافة في `Domain` (مرفوضة) أو غيرها |
| ترقية MediatR إلى 13+ | مرفوض — ترخيص تجاري |
| رفع إطار العمل المستهدف من .NET 8 | قرار مستقبلي موثّق في `README.md` §8 |
| تغيير السعر /구조 الفوترة | قرار عمل لا هندسي |

---

*آخر مُقاس: 2026-11-14. أي رقم في هذه الوثيقة يُعاد قياسه بـ `dotnet test` — لا تُنقل أرقام من وثائق أقدم.*
