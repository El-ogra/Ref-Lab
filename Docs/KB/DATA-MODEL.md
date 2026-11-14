# DATA-MODEL — المخطط/current 27 جدولاً

> **المرجع:** `src/RefLab.Infrastructure/Persistence/` · **المولّد:**
> `Docs/KB/generated/db-entity-index.csv` (الأعمدة والأدوار والمفاتيح مولّدة من الكود).

**المراجعة:** `f4daec94396796076082442e1cdcb81917293910`

> ⚠ **قاعدة المالك:** تطبيق المخطط على قاعدة المعمل **صلاحية المالك**.
> التطبيق لا يحدث من داخل التطبيق أبداً. (`DatabaseInitializer` مسجَّل ولا يُحلّ.)

---

## 1. النموذج العام

**27 جدولاً** موزّعة على ستّ مجموعات وظيفية. الجداول الرئيسية:

| المجموعة | الجداول |
|---|---|
| **المعرفة السريرية** | `TestDefinitions` · `ReferenceRanges` · `ClinicalConstants` · `ClinicalSubGroups` · `TestUnits` · `ConversionFactors` · `Abbreviations` · `ProthrombinControlCalibrations` |
| **المريض** | `Patients` · `PatientOrders` · `OrderTests` · `PatientHistories` · `PatientPictures` |
| **الفوترة** | `Payments` · `PriceLists` · `ReferringParties` · `TreatingPhysicians` |
| **التدقيق** | `PrintAudits` · `AttendanceEntries` |
| **الإرسال والأحياء** | `SendOutRequests` · `SampleDispositions` · `CultureResults` · `Antibiotics` · `Farms` · `FarmTransactions` |
| **الإعدادات** | `LaboratorySettings` · `Operators` |

**المعمل موقع واحد.** لا جدول فروع، ولا عمود فرع، ولا مخطط متعدد الفروع —
أُزيلت بقرار المالك في Phase 4.

## 2. بنية العلاقات (المفاتيح الأجنبية)

```
Patients
  ├─(FK)─▶ PatientOrders ──(FK)──▶ OrderTests ──┬─(FK)─▶ PrintAudits
  │            │                                   ├─(FK)─▶ SampleDispositions
  │            └──(FK)─▶ Payments                  └─(FK)─▶ SendOutRequests
  ├─(FK)─▶ PatientHistories
  ├─(FK)─▶ PatientPictures
  └─(FK)─▶ CultureResults

Farms ──(FK)──▶ FarmTransactions
```

**تسعة جداول** لها مفتاح أجنبي. الجداول الباقية مستقلة (كتالوج، إعدادات، سجلات).

**السلسلة الطويلة:** `Patient` → `PatientOrder` → `OrderTest` → الطلبات التابعة.
`OrderTests` يحمل **مفتاحين** إلى `PatientOrders`.

## 3. كائنات القيمة المملوكة

ستة تجميعات تملك كائنات قيمة. **كل عمود لكائن قيمة يُملَك يعيش في جدول
المالك** — لا جدول مستقل.

| الجدول | كائنات القيمة المملوكة | الخصائص (بادئات الأعمدة) |
|---|---|---|
| `OrderTests` | `ResultValue` · `Money` · `AgeBand` · `ReferenceInterval` · `ReferenceSnapshot` | `Result` · `Price` · `AgeBand` · `Interval` · `ReferenceSnapshot` |
| `PatientOrders` | `OrderIdentifier` · `Money` · `Percentage` | `LegacyIdentifier` · `TotalPrice` · `TotalPriceAfterDiscount` · `CommissionBaseAmount` · `Discount` |
| `Payments` | `Money` | `Amount` |
| `ReferenceRanges` | `AgeBand` · `ReferenceInterval` | `AgeBand` · `Interval` |
| `ReferringParties` | `Percentage` | `CommissionRate` |
| `TestDefinitions` | `CriticalInterval` | `CriticalInterval` |

### ⚠ قاعدة البادئة — مصدر التصادم

**نفس كائن القيمة يُملَك من أكثر من كيان.** `Money` مثلاً على `OrderTest`
(خاصية `Price`) وعلى `PatientOrder` (ثلاث خصائص: `TotalPrice`،
`TotalPriceAfterDiscount`، `CommissionBaseAmount`) وعلى `Payment` (`Amount`).

ولولا البادئة لتصادمت أعمدة `Amount` في جدول واحد. لذلك **كل إعداد `OwnsOne`
يأخذ بادئة اسم الخاصية**، والدالة المشتركة تضيفها:

```csharp
builder.OwnsOne(order => order.TotalPrice, money => money.ConfigureMoney("TotalPrice"));
// ينتج: TotalPrice_Amount , TotalPrice_CurrencyCode
```

المساعدات المشتركة في
`src/RefLab.Infrastructure/Persistence/Configurations/ValueObjectConfigurations.cs`:
`ConfigureMoney` · `ConfigurePercentage` · `ConfigureOrderIdentifier` ·
`ConfigureAgeBand` · `ConfigureReferenceInterval` · `ConfigureCriticalInterval` ·
`ConfigureResultValue`.

> **عند إضافة كائن قيمة مملوك جديد: مرّر بادئة.**

## 4. عرض الأعمدة والدقة

مُعلَن مرة واحدة في `ValueObjectConfigurations.cs` (فئة `ColumnWidths`):

| الاسم | القيمة | الاستخدام |
|---|---:|---|
| `Short` | 16 | رمز قصير (عملة، رمز وحدة) |
| `Name` | 128 | اسم أو عنوان |
| `Description` | 512 | وصف حر |
| `Note` | 2048 | ملاحظة طويلة |
| `OrderIdentifier` | 13 | **مُعرّف الطلب — 13 خانة** |
| `MoneyPrecision` / `MoneyScale` | 18 / 4 | دقة النقود |
| `PercentagePrecision` / `PercentageScale` | 9 / 4 | دقة النسبة |
| `FactorPrecision` / `FactorScale` | 18 / 8 | دقة معامل التحويل |

> **الأعرض من النظام القديم.** أعمدة Access كانت نصية حرة غير متسقة؛
> الأبعاد هنا مبنيّة على المحتوى الذي يخزّنه المعمل فعلاً.

## 5. محوّلات القيم (7)

`src/RefLab.Infrastructure/Persistence/Converters/PersistenceConverters.cs`

| المحوّل | من / إلى | لماذا |
|---|---|---|
| `PatientIdConverter` | `PatientId` ↔ `Guid` | مُعرّفstrongly-typed |
| `NullableStringListConverter` | `IReadOnlyList<string?>` ↔ `string` | JSON |
| `StringListConverter` | `IReadOnlyList<string>` ↔ `string` | JSON |
| `StringArrayConverter` | `string?[]` ↔ `string` | JSON — ثابتة الطول |
| `PermissionCollectionConverter` | `IReadOnlyCollection<Permission>` ↔ `string` | الصلاحيات **مفصولة بفواصل** |
| `SusceptibilityListConverter` | `IReadOnlyList<AntibioticSensitivity>` ↔ `string` | JSON |
| `PrinterRoleMapConverter` | `IReadOnlyDictionary<PrinterRole,string>` ↔ `string` | خريطة الطابعات |

## 6. سِلسلة الترحيلات

| الترحيل | ما يضيفه |
|---|---|
| `20261110103458_InitialCreate` | الجداول الـ 27 + المفاتيح الأجنبية + الفهارس |
| `20261111083113_AddReportFooterAndLayoutText` | `LayoutText` · `ReportFooterText` |
| `20261111122338_AddReportBrandingAndSampleOrigin` | `SampleTakenOutsideLab` · `DoctorName` · `DoctorSignature` · `HeadingMode` · `LabLogoPath` · `LogoWidthPx` · `LogoHeightPx` · `ReportLogoPath` · `ThanksText` |

`RefLabDbContextModelSnapshot.cs` لقطة المخطط الحالية — **تُحدَّث آلياً** مع كل ترحيل.

> **لا تُعدِّل ترحيلاً مُطبَّقاً. أنشئ ترحيلاً جديداً.** (§3 في `OPERATIONS.md`)

## 7. المخطط القديم — مرجع لا مصدر

المخطط القديم كان Access/Jet 4 (`Patients.mdb`) بـ **103 جداول** (منها 51 جدول
فحوص و39 جدول نتائج لكل ملف تعريف). توثيقه الكامل في `Database documentation.md`
و`evidence/reverse-engineering/metadata/adox_tables.csv`.

**قرار Q1:** جدول نتائج موحّد (`OrderTests` + `TestDefinitions`) يحلّ **تضخّم
البنية القديمة مع الحفاظ على البيانات السريرية.

> ⚠ **الوثائق القديمة تصف النظام القديم، لا هذا.** للبحث عن **إثبات** فقط.

## 8. وصفة إضافة جدول

1. كيان في `src/RefLab.Domain/<المجال>/` (بلا مراجع حزم).
2. `DbSet` + `Configure*()` في
   `src/RefLab.Infrastructure/Persistence/RefLabDbContext.cs` — الـ `DbSet`
   يُضاف إلى `OnModelCreating` أيضاً.
3. إعداد fluent في `src/RefLab.Infrastructure/Persistence/Configurations/`.
4. كائنات القيمة المملوكة ** ببادئة**.
5. `dotnet ef migrations add <Name>` — **ثم توقّف**.
6. `python scripts/kb_build.py && python scripts/kb_check.py` — الفهرس يتحدث تلقائياً.

**الترحيل يُطبَّق بقرار المالك.**

---

*الأرقام والأسماء في هذه الوثيقة مولّدة من الكود. أي جدول جديد يظهر في
`generated/db-entity-index.csv` فور توليد الفهارس.*
