# DOMAIN-RULES — قواعد العمل في طبقة `Domain`

> **هذه هي المعرفة السريرية والفوترة الحقيقية للنظام.** كل قاعدة هنا مُنفَّذة
> في `src/RefLab.Domain/Rules/`، وكل واحدة منها تحمل في `XML doc comment` فقرة
> «Legacy evidence» تشرح أصلها في النظام المرجعي القديم.

**المراجعة:** `f4daec94396796076082442e1cdcb81917293910`

> ⚠ **القاعدة الذهبية:** قواعد العمل **لا تُكرَّر** خارج `Domain`، ولا تُكتب
> في `Application` ولا في العرض. إن وجدت منطق عمل مكرَّراً في مكان آخر، فهو خطأ.

---

## 1. مجموعتا الثوابت السريرية

الثوابت **تُحفظ كبيانات في قاعدة البيانات** لا كشيفرة — حتى يستطيع المعمل
تصحيح عامل دون تغيير برمجي.

| المجموعة | الثابت | القيمة | الدلالة |
|---|---:|---:|---|
| `CBCCONS` | `constant1` | 3.3 | Hct = Hgb × 3.3 |
| `CBCCONS` | `constant2` | 6.25 | Hgb% = Hgb × 6.25 (ذكر > 12 سنة) |
| `CBCCONS` | `constant3` | 6.75 | Hgb% = Hgb × 6.75 (أنثى > 12 سنة) |
| `CBCCONS` | `constant4` | 7.5 | Hgb% = Hgb × 7.5 (من 1 إلى 12 سنة) |
| `CBCCONS` | `constant5` | 8.25 | Hgb% = Hgb × 8.25 (أقل من سنة) |
| `PTCONS` | `constant3` | 1.33 | وجود الـ ISI والحامل لمُعامل تحكّم معروف |

الملف: `src/RefLab.Domain/Rules/ClinicalConstantResolver.cs` ·
`src/RefLab.Domain/ClinicalKnowledge/ClinicalConstant.cs` (اسم المجموعة:
`ClinicalConstant.CbcGroupName` = `CBCCONS`، `ClinicalConstant.PtGroupName` = `PTCONS`).

**القراءة:** `Resolve(constants, groupName)` · `ResolveNumber(constants, groupName, slot)` ·
`ResolveText(constants, groupName, slot)`.

---

## 2. `ClinicalDerivationPolicy` — الاشتقاقات

**الملف:** `src/RefLab.Domain/Rules/ClinicalDerivationPolicy.cs`

### 2.1 الهيماتوكريت
```
Hct = Hgb × factor[slot 1]        factor = 3.3  (CBCCONS.constant1)
```

### 2.2 نسبة الهيموغلوبين
العامل يُختار حسب الجنس والعمر عبر `SelectHemoglobinFactorSlot(sex, age)`:

| الحالة | العامل | الخانة |
|---|---:|---|
| ذكر > 12 سنة | 6.25 | `constant2` (`AdultMaleFactorSlot`) |
| أنثى > 12 سنة | 6.75 | `constant3` (`AdultFemaleFactorSlot`) |
| من 1 إلى 12 سنة | 7.5 | `constant4` (`ChildFactorSlot`) |
| أقل من سنة | 8.25 | `constant5` (`InfantFactorSlot`) |

حدود العمر: `InfantAgeLimitYears = 1` · `AdultAgeLimitYears = 12`.

### 2.3 النسبة المشتقّة (AST/ALT · Albumin/Globulin)
```
DeriveRatio(numeratorAnalyte, denominatorAnalyte, numerator, denominator, unit)
  → RatioResult
```
**لا يكتب المشغّل القيمة** — تُحسب من نتيجتين موجودتين على نفس الطلب،
فتكون النسبة **قابلة لإعادة الإنتاج دائماً** من نتيجتيها.

**ترمي استثناءً** إذا كان اسم المُحلّل فارغاً أو المقام صفراً
(`InvalidResultValueException`).
→ `src/RefLab.Domain/Rules/ClinicalDerivationPolicy.cs` · الدالة `DeriveRatio`.
→ حالة الاستخدام `EnterDerivedRatioCommand` (مجال Results).

### 2.4 الحساسية الدولية (ISI)
```
ISI = (patientPT − controlPT) / (normalPT − controlPT) × controlIsi
```
**ترمي استثناءً** إذا كان `normalPT == controlPT` (المؤشر غير معرَّف).

> ⚠ **معلومة مهمّة للمالك:** صيغة النظام القديم **لم تكن قابلة للاسترجاع** من
> الأدلة الساكنة. هذه الصيغة هي **التعريف المخبري القياسي**، والسلوك
> **معلَّم بانتظار تأكيد المالك** — ومُوثَّق في الكود نفسه.

### 2.5 استيفاء تركيز الضابط (منحنى الـ PTC)
```
InterpolateControlConcentration(calibration, controlTime)
```
يستوفي منحنى المعايرة (نقاط `ProthrombinControlPoint`: زمن الضابط ↔ التركيز).
**يرمي استثناءً** إذا كان المنحنى فارغاً أو خرج زمن الضابط عن المدى المعاير.

> جدول `PTC` القديم يحوي **33 نقطة** (مثال: 12 ثانية ↔ 100 · 12.5 ثانية ↔ 92).

**الاختبارات:** `tests/RefLab.Domain.Tests/Rules/ClinicalDerivationPolicyTests.cs` ·
`tests/RefLab.Domain.Tests/Rules/ClinicalConstantResolverTests.cs` ·
`tests/RefLab.Application.Tests/Features/Derivation/DerivationTests.cs`

---

## 3. `ReferenceRangeSelector` — اختيار النطاق المرجعي

**الملف:** `src/RefLab.Domain/Rules/ReferenceRangeSelector.cs`

**أداة:** `Select(ranges, testCode, sex, age, ageUnit)` → أنسب نطاق ·
`SelectAll(ranges, testCode, sex, age, ageUnit)` → كل النطاقات المنطبقة، مرتّبةً من
الأكثر تحديداً إلى الأقل.

**القاعدة الحاكمة (حتمية):**

1. النطاق المقيَّد بـ**جنس واحد** يتقدّم على النطاق المقيَّد بال**جنسين**.
2. بين نطاقين متساويي التحديد: **أضيق نافذة عمرية** تتقدّم.
3. أي تعادل متبقٍ: **أصغر `Id`** للنطاق يكسر التعادل.

الترتيب في الشيفرة: `OrderByDescending(SexSpecificity)` ثم `ThenBy(AgeSpan)`
ثم `ThenBy(Id)`.

> **لماذا الحتمية؟** قاعدة الاختيار في النظام القديم **غير مُثبَتة**.
> لكن النطاق غير الحتمي يعني أن نفس المريض يحصل على نطاقين مختلفين في
> يومين مختلفين — وهو خطر سريري حقيقي. لذلك اختيرت قاعدة حتمية وموثّقة.

**الاختبار:** `tests/RefLab.Domain.Tests/Rules/ReferenceRangeSelectorTests.cs`

---

## 4. `ResultClassificationPolicy` — تصنيف النتيجة

**الملف:** `src/RefLab.Domain/Rules/ResultClassificationPolicy.cs`

```
Classify(ResultValue? result, ReferenceSnapshot? snapshot) → ResultClassification?
```
يقارن القيمة بالحدود الأدنى والأعلى **المجمَّدة** في النطاق المرجعي، ويصنّفها.
`ReferenceSnapshot` هو **نسخة مقفلة** من النطاق وقت الطلب — فتغيير النطاق
لاحقاً **لا يغيّر** تصنيف نتيجة سابقة.

> **النطاق المقفلة** قرار تصميمي: النتيجة المسجّلة يجب أن تُقرأ بنفس المعنى
> بعد سنوات، مهما تغيّرت النطاقات.

**الاختبار:** `tests/RefLab.Domain.Tests/Rules/ResultClassificationPolicyTests.cs`

---

## 5. `CommissionCalculator` — عمولة الإحالة

**الملف:** `src/RefLab.Domain/Rules/CommissionCalculator.cs`

```
الموروث من النظام القديم:
  SUM(Gehats.omola × patientinfo.elmadfoo3 / 100)
```

**التطبيق:**

| الدالة | المعنى |
|---|---|
| `Calculate(rate, baseAmount)` | عمولة على مبلغ أساس واحد |
| `Calculate(referringParty, baseAmount)` | العمولة بنسبة الجهة المحيلة |
| `CalculateTotal(rate, baseAmounts)` | المجموع على عدة مبالغ |

**انتبه:** في صفّ الطلب القديم ثلاثة حقول منفصلة — `totalprice` و
`totalpricedisc` و`elmadfoo3` — و**`elmadfoo3` وحده** يدخل في الصيغة.

**الاختبارات:** `tests/RefLab.Domain.Tests/Rules/CommissionCalculatorTests.cs` ·
`tests/RefLab.Application.Tests/Features/Billing/CommissionTests.cs`

---

## 6. `IdentifierAllocator` — مُعرّف الطلب

**الملف:** `src/RefLab.Domain/Rules/IdentifierAllocator.cs`

**13 خانة**، خمسة مقاطع:

| الخانات | المقطع | الدلالة |
|---|---|---|
| 1–2 | `leadingSegment` | **غير مُثبَت** |
| 3–4 | `personKey` | مفتاح الشخص |
| 5–9 | `middleSegment` | **غير مُثبَت** (كان 5–8 + رمز فرع في 9) |
| 10–12 | `displayCode` | رمز العرض القصير |
| 13 | `trailingCharacter` | **غير مُثبَت** |

> ⚠ **هذا أخطر عقد بيانات في النظام.** إعادة تنسيقه تكسر هوية المريض
> ورمز العرض وتقارير الإيراد في آن واحد. لذلك **يفرض المخصّصُ التخطيطَ**
> ولا يقبل نصاً عشوائياً.

> **أثر قرار الموقع الواحد:** الموضع 9 كان رمز الفرع؛ صار الآن جزءاً من
> المقطع الأوسط. البنية 13 خانة باقية.

**الاختبارات:** `tests/RefLab.Domain.Tests/Rules/IdentifierAllocatorTests.cs` ·
`tests/RefLab.Domain.Tests/ValueObjects/OrderIdentifierTests.cs` ·
`tests/RefLab.Application.Tests/Features/Patients/OrderIdentityTests.cs`

---

## 7. `ProthrombinControlPoint` و`InvalidClinicalDerivationException`

| الملف | الدور |
|---|---|
| `src/RefLab.Domain/Rules/ProthrombinControlPoint.cs` | نقطة معايرة واحدة: `ControlTime` + `Concentration` |
| `src/RefLab.Domain/Rules/InvalidClinicalDerivationException.cs` | فشل اشتقاق (يرث `DomainException`) |

---

## 8. أين توضع القاعدة الجديدة؟

1. `src/RefLab.Domain/Rules/<Policy>.cs` — `static class` نقي، **بلا أي مرجع إطار عمل**.
2. اختبار في `tests/RefLab.Domain.Tests/Rules/`.
3. سطر واحد في `Docs/KB/DOMAIN-RULES.md` (هذا الملف).
4. **لا** تكتبها في `Application`، ولا في العرض، ولا في DTO.

---

## 9. القواعد التي تتطلّب تأكيد المالك

| القاعدة | الحالة |
|---|---|
| صيغة الـ ISI | **غير مُسترجَعة** من الأدلة — طُبِّق التعريف القياسي، بانتظار التأكيد |
| دلالات مقاطع مُعرّف الطلب (1–2، 5–9، 13) | **غير مُثبَتة** — محفوظة كما هي |
| قاعدة اختيار النطاق عند التداخل | **غير مُثبَتة** — استُبدلت بقاعدة حتمية موثّقة |

> هذه الثلاثة موثّقة في `Docs/Business_Rules_V2.md` و`Docs/Functionality_Requirements_Matrix_V2.md`
> و`Docs/RL-P5-MAN-001-Phase5-Delivery-Manifest.md` §6 (الفجوات المعروفة).
> راجعها قبل تغيير أي صيغة.

---

*لمحة سريعة: 8 سياسات · ثوابت = بيانات · النطاق المرجعي حتمي · النتيجة مُصنَّفة على نطاق مقفلة · العمولة على `elmadfoo3` · المعرّف 13 خانة.*
