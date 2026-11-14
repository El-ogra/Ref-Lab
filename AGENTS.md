# AGENTS.md — تعليمات للوكلاء

**اقرأ `Docs/KB/START-HERE.md` قبل أي عمل.** فيه خريطة المشروع وجدول التوجيه.

## القواعد الثماني

1. **الكود هو المرجع.** التوثيق طبقة تنقّل، لا مصدر بديل.
2. **قواعد العمل في `src/RefLab.Domain/Rules/` فقط** — لا تُكرّرها في `Application` ولا في العرض.
3. **لا تُعدِّل ترحيلاً مُطبَّقاً** — أنشئ ترحيلاً جديداً.
4. **لا تُطبّق ترحيلاً.** تطبيق المخطط قرار المالك.
5. **داخل `src/RefLab.Presentation`**: اكتب `System.Windows.Application` كاملاً — `RefLab.Application` يخفيه ويُنتج CS0118 (و`TreatWarningsAsErrors=true` يحوّله إلى فشل بناء).
6. **MediatR يبقى على 12.x** — الفرع 13+ ترخيص تجاري.
7. **النصوص العربية من `Docs/ui-labels-verified.md` وحده** — لا تخترع نصاً.
8. **المعمل موقع واحد** — لا فرع ولا عمود فرع.

## عند تغيير الكود

```powershell
dotnet build RefLab.sln -c Release    # يجب: 0 تحذير، 0 خطأ
dotnet test  RefLab.sln -c Release    # يجب: كل المجموعات ناجحة
python scripts/kb_build.py            # أعِد توليد فهارس المعرفة
python scripts/kb_check.py            # يجب: SUCCESS
```

`kb_check.py` يفشل إن قَدُم فهرس أو انكسر مسار. **لا تتجاوزه ولا تعطّله.**

##Formatting عربي

العناوين وقصص التوثيق بالعربية، والمعرّفات التقنية والمسارات بالأرقام
والإنجليزية كما هي في الكود. لا تخلط الكلمات العربية بالإنجليزية داخل الجملة.

## حدود الكتابة

هذه الأدوات تكتب في `Docs/KB/generated/` و`Docs/USE_CASE_INVENTORY.md` فقط.
`ReferenceIndex/` و`Docs/RL-P*.md` و`README.md` و`IDEA.md` و`src/` و`tests/`
**ليست** ضمن نطاق الكتابة التلقائية.
