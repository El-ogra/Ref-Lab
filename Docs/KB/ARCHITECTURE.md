# ARCHITECTURE — بنية النظام وتوصيله

> كيف يعمل النظام فعلاً. **ليس** دليل برمجة: هو العقد المعماري الذي يحميه
> 18 اختبار حارس.

**المراجعة:** `f4daec94396796076082442e1cdcb81917293910` · **الدقة:** كل مسار
ومسار في هذه الوثيقة مولّد من `Docs/KB/generated/`.

---

## 1. القاعدة المعمارية

```
Presentation ──▶ Application ──▶ Domain ◀── Infrastructure
        └──────────── نقطة التركيب وحدها ────────────┘
```

| السهم | الاتجاه | من ينفّذه |
|---|---|---|
| Presentation → Application | عبر `MediatR` حصراً | `RequestExecutor` |
| Application → Domain | استدعاء نوع عادي | معالجات `Features/` |
| Infrastructure → Application | تنفيذ التجريدات | `InfrastructureServiceCollectionExtensions` |
| أي شيء → Infrastructure | **ممنوع** | حارس `Application_does_not_reference_the_infrastructure_layer` |

**نقطة التركيب واحدة فقط:** `src/RefLab.Presentation/Bootstrap/CompositionRoot.cs`.

## 2. دورة الحياة ومسار الطلب

```
App.OnStartup
  └─ CompositionRoot.BuildHost()
       ├─ Host.CreateApplicationBuilder()
       ├─ AddApplicationLayer()      ← MediatR + 4 سلوكيات + التحقق
       ├─ AddInfrastructureLayer()   ← IClock, IPasswordHasher, ICurrentUser,
       │                               IReportRenderer, IPdfExporter, IPrintSequenceGenerator
       ├─ AddPersistenceLayer(conn)  ← RefLabDbContext + IAppDbContext + IUnitOfWork
       │                               + DatabaseInitializer (لا يُحلّ أبداً)
       └─ AddPresentationLayer()     ← التنقّل + النماذج + العروض
  └─ SignInWindow ← من الحاوية
```

ثم `INavigationService.NavigateTo(ScreenKeys.SignIn)`.

**مسار طلب MediatR — الترتيب مقصود:**

```
ValidationBehavior → AuthorizationBehavior → LoggingBehavior → DomainExceptionBehavior → Handler
```

| السلوك | الملف | ما يفعله |
|---|---|---|
| `ValidationBehavior` | `src/RefLab.Application/Behaviors/ValidationBehavior.cs` | FluentValidation → `RequestValidationException` |
| `AuthorizationBehavior` | `src/RefLab.Application/Behaviors/AuthorizationBehavior.cs` | يفحص `IRequirePermission` → `ForbiddenException` |
| `LoggingBehavior` | `src/RefLab.Application/Behaviors/LoggingBehavior.cs` | تسجيل الطلب والاستجابة |
| `DomainExceptionBehavior` | `src/RefLab.Application/Behaviors/DomainExceptionBehavior.cs` | `DomainException` → `ConflictException` |

**الترتيب مقصود:** لا يُسجَّل طلب مرفوض صلاحية، ولا تُترجم استثناءات النطاق
قبل التحقق من الصلاحية.

**الاستثناءات المطبَّقة صراحةً** (`IUnitOfWork`): `Add` · `Update` · `Remove` ·
`SaveChangesAsync` — ولهذا لا يظهر CS0108.

## 3. تجريدات طبقة `Application` الثمانية

| التجريد | التنفيذ | الملف |
|---|---|---|
| `IAppDbContext` | `RefLabDbContext` (قراءة فقط عبر `IQueryable<T>`) | `src/RefLab.Application/Abstractions/IAppDbContext.cs` |
| `IUnitOfWork` | `RefLabDbContext` (نفس النسخة scope) | `src/RefLab.Application/Abstractions/IUnitOfWork.cs` |
| `ICurrentUser` | `SessionCurrentUser` | `src/RefLab.Application/Abstractions/ICurrentUser.cs` |
| `IClock` | `SystemClock` | `src/RefLab.Application/Abstractions/IClock.cs` |
| `IPasswordHasher` | `Pbkdf2PasswordHasher` | `src/RefLab.Application/Abstractions/IPasswordHasher.cs` |
| `IReportRenderer` | `QuestPdfReportRenderer` | `src/RefLab.Application/Abstractions/IReportRenderer.cs` |
| `IPdfExporter` | `QuestPdfExporter` | `src/RefLab.Application/Abstractions/IPdfExporter.cs` |
| `IPrintSequenceGenerator` | `PrintSequenceGenerator` | `src/RefLab.Application/Abstractions/IPrintSequenceGenerator.cs` |

## 4. الأدوات المشتركة في طبقة `Application`

| الملف | الدور |
|---|---|
| `src/RefLab.Application/ApplicationServiceCollectionExtensions.cs` | تسجيل MediatR والسلوكيات والتحقق |
| `src/RefLab.Application/Common/ApplicationException.cs` | أساس استثناءات التطبيق |
| `src/RefLab.Application/Common/ConcurrencyException.cs` | تعديل متزامن |
| `src/RefLab.Application/Common/ConflictException.cs` | تعارض نطاق ← هدف الترجمة |
| `src/RefLab.Application/Common/ForbiddenException.cs` | صلاحية غير كافية |
| `src/RefLab.Application/Common/NotFoundException.cs` | كيان غير موجود |
| `src/RefLab.Application/Common/NoResult.cs` | استجابة بلا قيمة |
| `src/RefLab.Application/Common/PageRequest.cs` | طلب صفحة (رقم + حجم) |
| `src/RefLab.Application/Common/PagedResult.cs` | نتيجة صفحة |
| `src/RefLab.Application/Common/ReportModels.cs` | نماذج التقارير المشتركة |
| `src/RefLab.Application/Common/RequestValidationException.cs` | أخطاء التحقق |

## 5. `Domain` — الطبقات المشتركة

### 5.1 القواعد الثماني (المركز الحقيقي للمعرفة)

| الملف | القاعدة |
|---|---|
| `src/RefLab.Domain/Rules/ClinicalDerivationPolicy.cs` | اشتقاق الهيماتوكريت والنسبة والـ ISI |
| `src/RefLab.Domain/Rules/ClinicalConstantResolver.cs` | قراءة العوامل كبيانات وقت التشغيل |
| `src/RefLab.Domain/Rules/ReferenceRangeSelector.cs` | اختيار النطاق المرجعي الجنس/العمر |
| `src/RefLab.Domain/Rules/ResultClassificationPolicy.cs` | تصنيف النتيجة (حرج/طبيعي/شاذ) |
| `src/RefLab.Domain/Rules/CommissionCalculator.cs` | حساب عمولة الإحالة |
| `src/RefLab.Domain/Rules/IdentifierAllocator.cs` | بناء مُعرّف الطلب (13 خانة) |
| `src/RefLab.Domain/Rules/ProthrombinControlPoint.cs` | نقطة معايرة الـ PTC |
| `src/RefLab.Domain/Rules/InvalidClinicalDerivationException.cs` | فشل اشتقاق |

> التفاصيل والصيغ في `Docs/KB/DOMAIN-RULES.md`.

### 5.2 كائنات القيمة

`src/RefLab.Domain/ValueObjects/PatientId.cs` ·
`src/RefLab.Domain/ValueObjects/OrderIdentifier.cs` ·
`src/RefLab.Domain/ValueObjects/Money.cs` ·
`src/RefLab.Domain/ValueObjects/Percentage.cs` ·
`src/RefLab.Domain/ValueObjects/ResultValue.cs` ·
`src/RefLab.Domain/ValueObjects/RatioResult.cs` ·
`src/RefLab.Domain/ValueObjects/ReferenceInterval.cs` ·
`src/RefLab.Domain/ValueObjects/CriticalInterval.cs` ·
`src/RefLab.Domain/ValueObjects/AgeBand.cs`

### 5.3 التعدادات

`src/RefLab.Domain/Enums/AgeUnit.cs` ·
`src/RefLab.Domain/Enums/AttendanceEventType.cs` ·
`src/RefLab.Domain/Enums/RecordStatus.cs` ·
`src/RefLab.Domain/Enums/ReportCategory.cs` ·
`src/RefLab.Domain/Enums/ReportHeadingMode.cs` ·
`src/RefLab.Domain/Enums/ResultClassification.cs` ·
`src/RefLab.Domain/Enums/ResultState.cs` ·
`src/RefLab.Domain/Enums/SampleDispositionAction.cs` ·
`src/RefLab.Domain/Enums/SendOutState.cs` ·
`src/RefLab.Domain/Enums/Sex.cs` ·
`src/RefLab.Domain/Enums/SexScope.cs`

### 5.4 الاستثناءات

`src/RefLab.Domain/Exceptions/DomainException.cs` (الأساس) ·
`src/RefLab.Domain/Exceptions/BillingExceptions.cs` ·
`src/RefLab.Domain/Exceptions/CatalogueExceptions.cs` ·
`src/RefLab.Domain/Exceptions/PatientExceptions.cs` ·
`src/RefLab.Domain/Exceptions/ReferenceRangeExceptions.cs` ·
`src/RefLab.Domain/Exceptions/SecurityExceptions.cs` ·
`src/RefLab.Domain/Exceptions/SendOutExceptions.cs`

## 6. `Infrastructure`

| الملف | الدور |
|---|---|
| `src/RefLab.Infrastructure/InfrastructureServiceCollectionExtensions.cs` | تسجيل المحوّلات وطبقة persistence |
| `src/RefLab.Infrastructure/Persistence/RefLabDbContext.cs` | السياق: 27 `DbSet` + 27 `Configure*()` |
| `src/RefLab.Infrastructure/Persistence/RefLabDbContextFactory.cs` | مصنع زمن التصميم |
| `src/RefLab.Infrastructure/Persistence/DatabaseInitializer.cs` | **مسجَّل ولا يُحلّ أبداً** |
| `src/RefLab.Infrastructure/Persistence/Configurations/OperationalConfigurations.cs` | إعدادات العمليات |
| `src/RefLab.Infrastructure/Persistence/Configurations/ValueObjectConfigurations.cs` | عرض الأعمدة وال bupادئات |
| `src/RefLab.Infrastructure/Persistence/Converters/PersistenceConverters.cs` | 7 محوّلات قيم |
| `src/RefLab.Infrastructure/Persistence/Migrations/20261110103458_InitialCreate.cs` | المخطط الكامل |
| `src/RefLab.Infrastructure/Persistence/Migrations/20261111083113_AddReportFooterAndLayoutText.cs` | تذييل ونص التخطيط |
| `src/RefLab.Infrastructure/Persistence/Migrations/20261111122338_AddReportBrandingAndSampleOrigin.cs` | العلامة التجارية + عيّنة خارج المعمل |
| `src/RefLab.Infrastructure/Persistence/Migrations/RefLabDbContextModelSnapshot.cs` | لقطة المخطط |
| `src/RefLab.Infrastructure/Reports/QuestPdfReportRenderer.cs` | بناء مستند PDF |
| `src/RefLab.Infrastructure/Reports/QuestPdfExporter.cs` | كتابة الملف + العلامة التجارية |
| `src/RefLab.Infrastructure/Reports/ReportBranding.cs` | العلامة العربية والإنجليزية + الخط |
| `src/RefLab.Infrastructure/Reports/PrintSequenceGenerator.cs` | تسلسل الطباعة |
| `src/RefLab.Infrastructure/Security/Pbkdf2PasswordHasher.cs` | تجزئة كلمة المرور |
| `src/RefLab.Infrastructure/Session/SessionCurrentUser.cs` | المستخدم الحالي |
| `src/RefLab.Infrastructure/Time/SystemClock.cs` | الساعة |

## 7. `Presentation`

| الملف | الدور |
|---|---|
| `src/RefLab.Presentation/App.xaml` · `src/RefLab.Presentation/App.xaml.cs` | تطبيق WPF (نوع `Application` **مُؤهَّل**) |
| `src/RefLab.Presentation/MainWindow.xaml` · `src/RefLab.Presentation/MainWindow.xaml.cs` | الهيكل: شريط الوحدات + شريط الحالة |
| `src/RefLab.Presentation/SignInWindow.xaml` · `src/RefLab.Presentation/SignInWindow.xaml.cs` | نافذة الدخول الأولى |
| `src/RefLab.Presentation/Bootstrap/CompositionRoot.cs` | **نقطة التركيب الوحيدة** |
| `src/RefLab.Presentation/Bootstrap/PresentationServiceCollectionExtensions.cs` | تسجيل التنقّل والنماذج والعروض |
| `src/RefLab.Presentation/Common/RequestExecutor.cs` | **قناة الاتصال الوحيدة** بـ Application |
| `src/RefLab.Presentation/Common/IErrorPresenter.cs` · `src/RefLab.Presentation/Common/ErrorPresenter.cs` | عرض الأخطاء للمشغّل |
| `src/RefLab.Presentation/Common/OperatorSession.cs` | جلسة المشغّل الحالي |
| `src/RefLab.Presentation/Common/LabCurrency.cs` | العملة: EGP |
| `src/RefLab.Presentation/Common/UiExceptions.cs` | استثناءات الواجهة |
| `src/RefLab.Presentation/Common/ViewModelBase.cs` | الأساس: busy · خطأ · حالة |
| `src/RefLab.Presentation/Navigation/ScreenKeys.cs` | 14 معرّف شاشة |
| `src/RefLab.Presentation/Navigation/ScreenRegistry.cs` | خريطة الشاشة ← العرض |
| `src/RefLab.Presentation/Navigation/INavigationService.cs` · `src/RefLab.Presentation/Navigation/NavigationService.cs` | التنقّل مع سجل رجوع |
| `src/RefLab.Presentation/Navigation/IViewLocator.cs` · `src/RefLab.Presentation/Navigation/ViewLocator.cs` | حلّ العرض من الحاوية |
| `src/RefLab.Presentation/Navigation/INavigationAware.cs` | استقبال معامل التنقّل |
| `src/RefLab.Presentation/Resources/Strings.xaml` | النصوص الإنجليزية |
| `src/RefLab.Presentation/Resources/Strings.ar.xaml` | النصوص العربية (مفاتيح منفصلة) |
| `src/RefLab.Presentation/Resources/Themes/Light.xaml` · `src/RefLab.Presentation/Resources/Themes/Dark.xaml` | السمات |
| `src/RefLab.Presentation/Resources/Converters/LabConverters.cs` | محوّلات العرض |
| `src/RefLab.Presentation/ViewModels/ShellViewModel.cs` | نموذج الهيكل |
| `src/RefLab.Presentation/ViewModels/EnumSource.cs` | تغذية قوائم التعدادات |
| `src/RefLab.Presentation/ViewModels/Panels/DerivationPanelViewModel.cs` | لوحة الاشتقاقات |
| `src/RefLab.Presentation/ViewModels/Panels/MethodSheetPanelViewModel.cs` | لوحة ورقة المنهج |
| `src/RefLab.Presentation/Views/Controls/CataloguePanelControl.xaml` · `src/RefLab.Presentation/Views/Controls/CataloguePanelControl.xaml.cs` | لوحة الكتالوج (يسار) |
| `src/RefLab.Presentation/Views/Controls/PatientPanelControl.xaml` · `src/RefLab.Presentation/Views/Controls/PatientPanelControl.xaml.cs` | لوحة المريض (يمين، RTL) |
| `src/RefLab.Presentation/Views/Controls/ResultsGridControl.xaml` · `src/RefLab.Presentation/Views/Controls/ResultsGridControl.xaml.cs` | شبكة النتائج |

**العرض المادي:** لوحة المريض **عربية على اليمين (RTL)** ولوحة الكتالوج
**إنجليزية على اليسار (LTR)** داخل النافذة نفسها.

## 8. الحُرّاس المعماريون — 18 اختباراً

`tests/RefLab.Domain.Tests/Architecture/DomainLayerBoundaryTests.cs`:
`Domain_does_not_reference_framework_dependencies` ·
`Domain_references_only_the_base_class_library` ·
`Domain_assembly_is_deployed_to_the_test_output`

`tests/RefLab.Application.Tests/Architecture/ApplicationLayerBoundaryTests.cs`:
`Application_references_the_domain_layer` ·
`Application_does_not_reference_persistence_or_presentation_frameworks` ·
`Application_does_not_reference_the_infrastructure_layer` ·
`Application_assembly_is_deployed_to_the_test_output`

`tests/RefLab.Application.Tests/Architecture/PresentationLayerBoundaryTests.cs`:
`B1_the_presentation_project_declares_no_persistence_or_pdf_package` ·
`B2_the_presentation_project_references_only_the_three_layers` ·
`B3_the_only_mvvm_framework_is_the_approved_community_toolkit` ·
`B4_no_presentation_source_reaches_a_persistence_concern` ·
`B5_no_presentation_source_brings_back_the_removed_delivery_capabilities` ·
`B6_no_presentation_source_brings_back_the_removed_branch_capability` ·
`B7_no_presentation_type_exposes_a_persistence_type` ·
`B8_the_wpf_application_type_is_always_fully_qualified` ·
`B9_every_arabic_string_is_authorised_by_the_verified_label_source` ·
`The_composition_root_registers_the_three_approved_layers` ·
`The_composition_root_never_resolves_the_database_initialiser` ·
`The_application_does_not_declare_a_startup_uri` ·
`The_application_class_derives_from_the_fully_qualified_wpf_type` ·
`The_arabic_and_english_dictionaries_use_disjoint_key_sets` ·
`The_binding_keyboard_shortcuts_are_declared` ·
`The_laboratory_currency_is_egp` ·
`The_presentation_project_is_the_only_project_that_references_the_mvvm_toolkit` ·
`The_report_branding_carries_the_verified_branding`

## 9. وصفات الإضافة — أين تلمس

### 9.1 إضافة حالة استخدام
1. `src/RefLab.Application/Features/<المجال>/<Name>.cs` — السجل والمُعالِج في ملف واحد.
2. `src/RefLab.Application/Validation/<المجال>Validators.cs` — أضف `public sealed class <Name>Validator`.
3. حدّث `EXPECTED_TOTAL` و`LEGACY` في `scripts/generate_use_case_inventory.py`.
4. `python scripts/kb_build.py` ثم `python scripts/kb_check.py`.

### 9.2 إضافة شاشة
1. `src/RefLab.Presentation/ViewModels/<Name>ViewModel.cs`
2. `src/RefLab.Presentation/Views/<Name>View.xaml` + `.xaml.cs`
3. `ScreenKeys` ← ثابت جديد · `ScreenRegistry` ← `[ScreenKeys.X] = typeof(Views.XView)`
4. `PresentationServiceCollectionExtensions` ← `AddTransient` للنموذج والعرض
5. **الاختصارات**: `The_binding_keyboard_shortcuts_are_declared` — عرّفها أو سيكسر الاختبار.
6. النصوص العربية: من `Docs/ui-labels-verified.md` فقط.

### 9.3 إضافة جدول
1. كيان في `src/RefLab.Domain/<المجال>/`
2. `DbSet` + `Configure*()` في `src/RefLab.Infrastructure/Persistence/RefLabDbContext.cs`
3. إعداد fluent في `src/RefLab.Infrastructure/Persistence/Configurations/`
4. **ترحيل جديد** — لا تُعدّل القديم
5. `dotnet ef migrations add <Name>` — ثم **توقّف**، التطبيق قرار المالك

### 9.4 إضافة قاعدة عمل
1. `src/RefLab.Domain/Rules/<Policy>.cs` — `static class` نقي بلا أي مرجع إطار عمل
2. اختبار في `tests/RefLab.Domain.Tests/Rules/`
3. وثّقها في `Docs/KB/DOMAIN-RULES.md`

## 10. خريطة الاعتماديات

`Docs/KB/generated/dependency-edges.csv` — 244 حافة على مستوى مساحة الاسم
والمشروع. ويُختصر إلى:

```
RefLab.Presentation ──▶ RefLab.Application ──▶ RefLab.Domain
        │                       ▲
        └───────────────────────┴──▶ RefLab.Infrastructure ──▶ RefLab.Domain
```

---

*عند الشك في أي جملة هنا: الكود هو الحكم، و`kb_check.py` هو الضامن.*
