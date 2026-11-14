# CONVENTIONS — الفخاخ وقواعد العمل

> **اقرأه ثانياً بعد `START-HERE.md`.** هذا الملف يجمع ما كلّف وقتاً حقيقياً.
> كل قاعدة هنا لها **حارس مُختبَر** في الكود؛ إن حذفت الحارس أو كسرته،
> يفشل `kb_check.py` (المسار V10).

**المراجعة التي تحقّق منها:** `f4daec94396796076082442e1cdcb81917293910`

---

## 1. فخّ البناء الأكبر: `Application` مُختبَنة

**المشكلة.** داخل `src/RefLab.Presentation`، مساحة الاسم `RefLab.Application`
تُخفي نوع WPF ‏`System.Windows.Application`. أي ذكر غير مُؤهَّل لـ `Application`
يُحلّ إلى **مساحة الاسم** ويُنتج خطأ **CS0118**. و`TreatWarningsAsErrors=true`
في `Directory.Build.props` يحوّل هذا التحذير إلى **فشل بناء**.

**القاعدة.** كل ملف في طبقة العرض يحتاج نوع WPF يكتبه **كاملاً**:

```csharp
public partial class App : System.Windows.Application   // ✅
public partial class App : Application                  // ❌ CS0118
```

**احذر أيضاً** كتابة `Application.ICurrentUser` — اكتب `RefLab.Application.Abstractions.ICurrentUser`.

> الحارس: `B8_the_wpf_application_type_is_always_fully_qualified`
> في `tests/RefLab.Application.Tests/Architecture/PresentationLayerBoundaryTests.cs`.
> موثّق أيضاً في `src/RefLab.Presentation/Bootstrap/CompositionRoot.cs`
> و`src/RefLab.Presentation/App.xaml.cs`.

## 2. فخّ المجلدات: `Docs` لا `docs`

**المشكلة.** المستودع يستخدم `Docs` بحرف كبير **D**. في نظام ملفات Windows
غير الحسّاس لحالة الأحرف لا فرق — لكن على نظام حسّاس (Linux، أو مستودع
مستضاف) ينشأ **مجلد ثانٍ** ويبقى الملف المتتبَّع بلا تحديث.

**القاعدة.** كل المسارات داخل المستودع capitalized تماماً. `Docs` لا `docs`.

> **مثال حيّ:** كان `scripts/generate_use_case_inventory.py` يكتب إلى `docs/`
> بينما الملف المتتبَّع في `Docs/`. النتيجة: المولّد كان «ينجح» ويُحدّث مجلداً
> ميتاً، ويبقى `Docs/USE_CASE_INVENTORY.md` قديماً. حُسمت في 2026-11-14.

## 3. الترحيل (migration): لا تُعدِّل ولا تُطبِّق

**قاعدة صارمة.** الترحيل **لا يُعدَّل بعد تطبيقه**، و**لا يُطبَّق تلقائياً**.

- الترحيلات الثلاثة الحالية: `20261110103458_InitialCreate` (كل الجداول),
  `20261111083113_AddReportFooterAndLayoutText`,
  `20261111122338_AddReportBrandingAndSampleOrigin`.
- لتغيير المخطط: أنشئ ترحيلاً **جديداً** عبر `dotnet ef migrations add`.
- **تطبيق المخطط على قاعدة المعمل قرار المالك**، لا قرار المبرمج.
- التطبيق يتم من سطر الأوامر، **لا من داخل التطبيق**.

> **لماذا لا يطبّق التطبيق الترحيل بنفسه؟** `DatabaseInitializer` مسجَّل في
> حاوية الاعتماديات لكنه **لا يُحلّ أبداً** — وهذا مقصود.
> الحارس: `The_composition_root_never_resolves_the_database_initialiser`.

## 4. طبقة `Domain` بلا مراجع حزم — رفض أي `PackageReference`

**قاعدة صارمة.** `src/RefLab.Domain/RefLab.Domain.csproj` **لا يحتوي** أي
`PackageReference` ولا أي `ProjectReference`. لا EF Core، ولا MediatR، ولا WPF.

نموذج العمل يجب أن يكون قابلاً للاختبار بلا أي إطار عمل.
> الحارس: `Domain_references_only_the_base_class_library` و
> `Domain_does_not_reference_framework_dependencies`.

## 5. `MediatR` مثبَّت على 12.x

`MediatR` 13 وما بعده **يتطلب ترخيصاً تجارياً**. يبقى الفرع 12.x.
القيد نفسه في `README.md` §6 (قرار Q3).

## 6. `Application` لا يعرف `DbSet`

`IAppDbContext` يكشف `IQueryable<T>` **لا** `DbSet<T>`، و`IUnitOfWork` منفصل.
هكذا تبقى طبقة `Application` خالية من محرك persistence وقابلة للاختبار بلا قاعدة بيانات.

> الحارس: `Application_does_not_reference_persistence_or_presentation_frameworks`
> و`Application_does_not_reference_the_infrastructure_layer`.

## 7. الاستثناءات: ترجمة قبل أن تصل إلى المشغّل

مسار الطلب يمرّ بأربعة سلوكيات **بهذا الترتيب**:

```
Validation → Authorization → Logging → DomainException
```

`DomainExceptionBehavior` يحوّل `DomainException` إلى `ConflictException` قبل أن
تغادر طبقة `Application`. `RequestExecutor` (طبقة العرض) يترجم الباقي إلى
`UiValidationException` و`UiOperationException`.
**استثناء نطاقي يجب ألّا يصل إلى المشغّل أبداً.**

> الحارس: `DomainExceptionBehavior<,>` + `tests/RefLab.Application.Tests/Behaviors/BehaviorTests.cs`.

## 8. الصلاحيات: 15 صلاحية مُسمّاة، لا بتات

`src/RefLab.Domain/Identity/Permission.cs` يعرّف **15** صلاحية وظيفية
(`RegisterPatient` … `ManageAccounting`). النظام القديم كان يحمل 17 عموداً
منطقياً `grant1..grant17` **لم تُربط أي واحدة منها بقدرة محددة أبداً** — وBit
رقم 17 (نشر النتائج رقمياً) أُلغي مع SMS/NATIGH.COM، وBit 16 (الفروع) أُلغي
مع قرار الموقع الواحد.

طلب واحد يطلب صلاحيةً واحدة فقط عبر واجهة `IRequirePermission`.
**44 من 73** حالة استخدام تطلب صلاحية.

## 9. النصوص العربية: مصدر واحد معتمد

كل نص عربي في الواجهة يجب أن يكون مصدره `Docs/ui-labels-verified.md`.
**لا تختلق نصاً عربياً.** النصوص الإنجليزية في
`src/RefLab.Presentation/Resources/Strings.xaml`، والعربية في `Strings.ar.xaml`،
ومجموعتا المفاتيح **مفككتان** (لا مفتاح مشترك).

> الحارسان: `B9_every_arabic_string_is_authorised_by_the_verified_label_source`
> و`The_arabic_and_english_dictionaries_use_disjoint_key_sets`.

## 10. المعمل موقع واحد — لا فرع

أُزيلت الفروع بقرار المالك في Phase 4: جدول `Labbranch`، عمود الفرع في جدول
المستخدمين، **الموضع التاسع** من مُعرّف الطلب (13 خانة)، والجرد متعدد الفروع.

**الأثر على المعرّف:** المعرّف يبقى 13 خانة، لكن المقطع الأوسط صار
(5–9) بدل أن يكون (5–8) + فرع.

> الحارس: `B6_no_presentation_source_brings_back_the_removed_branch_capability`.

## 11. المال: `decimal` لا `double`

النظام القديم استخدم `Double` للنقود. `Money` و`Percentage` تستخدم `decimal`
(دقة 18، مقياس 4). هذا **تصحيح لسلامة البيانات**، لا تغيير في السلوك.
> القرار Q6 في `README.md` §6.

## 12. كائنات القيمة المملوكة: البادئة تمنع التصادم

نفس كائن القيمة قد يُملَك من أكثر من كيان — `Money` مثلاً على `OrderTest`
وعلى `PatientOrder`. لذلك كل إعداد `OwnsOne` **يأخذ بادئة عمود**،
والبادئات معلنة مرة واحدة في
`src/RefLab.Infrastructure/Persistence/Configurations/ValueObjectConfigurations.cs`.

**عند إضافة كائن قيمة مملوك جديد: مرّر بادئة.**

## 13. القيود السريرية: ثوابت بيانات لا شيفرة

عوامل الاشتقاق (`CBCCONS`: ‎3.3 / 6.25 / 6.75 / 7.5 / 8.25) تُقرأ **من قاعدة
البيانات وقت التشغيل** عبر `ClinicalConstantResolver` — لا تُكتب في الكود.
وذلك حتى يستطيع المعمل تصحيح عامل دون تغيير برمجي.

**لا تُ hard-code عاملاً سريرياً.** راجع `Docs/KB/DOMAIN-RULES.md`.

## 14. تسمية ملفات التحقق: ثلاثة استثناءات

ملفات التحقق في `src/RefLab.Application/Validation/` لا تتبع تسمية موحّدة:

| المجلد | ملف التحقق |
|---|---|
| `Patients` | `PatientValidators.cs` (مفرد) |
| `Results` | `ResultValidators.cs` (مفرد) |
| `Worksheets` | `WorksheetValidators.cs` (مفرد) |

المجلدات الباقية تتبع `<المجلد>Validators.cs` تماماً.
**أي مولّد يفترض `{المجلد}Validators.cs` سيفشل في ثلاثة مجلدات من أربعة عشر.**
عالجها في `scripts/kb_build.py` (جدول `VALIDATOR_AREA`).

## 15. العملة EGP — `LabCurrency`

العملة **مفردة**: EGP، معلنة في
`src/RefLab.Presentation/Common/LabCurrency.cs`. لا عملة ثانية.
> الحارس: `The_laboratory_currency_is_egp`.

## 16. أسماء ملفات الـ feature = ملف لكل حالة استخدام

`src/RefLab.Application/Features/<المجال>/` — **ملف واحد لكل حالة استخدام**،
في الملف نفسه السجلُ (record) والمُعالِج (handler). ملف `Dtos.cs` مشترك في 11
مجلداً من 14 (الأجزاء الثلاثة بلا `Dtos.cs`: `Auditing`, `Derivation`,
`Statistics`).

## 17. `IQueryable` للقراءة و`IUnitOfWork` للكتابة

الاستعلام يمرّ عبر `_db.Set<T>()`، والكتابة عبر `_unitOfWork.Add/Update/Remove`.
المُعالِج لا يستدعي `SaveChangesAsync` إلا عبر `_unitOfWork.SaveChangesAsync`.
الواجهة `IUnitOfWork` لا تعرّض أي عملية حفظ غير متزامنة بغير `SaveChangesAsync`.
**لا تستخدم `DbContext` مباشرة في طبقة `Application`.**

## 18. الترحيلات والإعدادات: قائمة القراءة الوحيدة

سلسلة الاتصال تُقرأ بالترتيب: المُعامل المُمرَّر ← المتغيّر البيئي
`REFLAB_CONNECTION_STRING` ← قيمة الاحتياطية `(localdb)\mssqllocaldb;Database=RefLab`.
كل التفاصيل والقيود في `Docs/KB/OPERATIONS.md`.

---

## 19. قائمة تحقّق سريعة قبل الاعتماد

```powershell
dotnet build RefLab.sln -c Release        # يجب: 0 تحذير، 0 خطأ
dotnet test  RefLab.sln -c Release        # يجب: كل المجموعات ناجحة
python scripts/generate_use_case_inventory.py   # يجب: exit 0، العدد مطابق
python scripts/kb_check.py                # يجب: SUCCESS
```

**إن أضفت حالة استخدام جديدة:** عدّل `EXPECTED_TOTAL` في
`scripts/generate_use_case_inventory.py`، وأضف ربطها في قاموس `LEGACY`،
ثم شغّل `kb_build.py` — **ولا تكتب سطراً في التوثيق يدوياً**.

---

*هذه القواعد موثّقة في الكود نفسه. عند الشك، اقرأ XML doc comment بجوار الكود:
كل قاعدة نطقية تحمل فقرة «Legacy evidence» تشرح أصلها.*
