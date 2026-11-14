# PHASES — سجل المراحل

> **هذا فهرس pointers، لا تكرار.** كل تفصيل في وثيقته الأصلية.
> اقرأه عند سؤال «متى نُجزّم هذا ولماذا؟».

**المراجعة المثبّتة:** `f4daec94396796076082442e1cdcb81917293910` · 9 التزامات، 2026-11-10 → 2026-11-14

---

## 1. خريطة المراحل

| المرحلة | الالتزام | ما نُجزّم | الوثيقة |
|---|---|---|---|
| **0** — الاستكشاف | `f14edc8` | استيعاب وثائق النظامين المرجعين، تقييم البيئة والأدوات، خطة الأدوار | `Docs/RL-P0-RPT-001-Discovery-Report.md` |
| **1** — الهيكل | `aff4f2f` | الحل والمشاريع الأربعة، `Directory.Build.props`، `global.json` | (ضمن تقرير Phase 3) |
| **2** — Domain | `498244e` | 65 ملفاً: كيانات · كائنات قيمة · تعدادات · **8 قواعد** | `Docs/RL-P2-MAN-001-Domain-Layer-Manifest.md` |
| **3** — Application | `4b40c6a` | 73 حالة استخدام · 4 سلوكيات · 8 تجريدات · 14 مجالاً | `Docs/RL-P3-MAN-001-Application-Layer-Manifest.md` · `Docs/RL-P3-Completion-Report.md` |
| **4** — Persistence | `4d7cdb0` | 27 جدولاً · 3 ترحيلات · QuestPDF · **إزالة الفروع** | `Docs/RL-P4-MAN-001-Phase4-Delivery-Manifest.md` · `Docs/RL-P4-Completion-Report.md` |
| **5** — Presentation | `876f397` | 14 شاشة + لوحتان · 17 نموذج عرض · تنقّل · توطين | `Docs/RL-P5-MAN-001-Phase5-Delivery-Manifest.md` · `Docs/LOCAL_AGENT_IMPLEMENTATION_GUIDE.md` |
| **الأدلة** | `f4daec9` | مستند `Patients.mdb` (103 جداول) · تحليل `real lab system.exe` (VB6) · `ReferenceIndex/` | `Database documentation.md` · `Documenting the Executable File.md` · `evidence/` |
| **قاعدة المعرفة** | (الجاري) | `Docs/KB/` + `scripts/kb_build.py` + `scripts/kb_check.py` | هذا المجلد |

> ⚠ **المرحلة 0 ← التوثيق في `Docs/RL-P0-RPT-001` عربي، ووثائق V2 إنجليزية.**
> النظام المرجعي القديم بـ **VB6 + Access Jet 4** مع مسار SQL Server.

## 2. قرارات المالك التي غيّرت الشكل

| القرار | الأثر | أين |
|---|---|---|
| **الموقع الواحد** — إلغاء الفروع | حُذف جدول `Labbranch` وعمود الفرع ورقم الفرع في المعرّف؛ المقطع الأوسط صار 5–9 | `README.md` §6 |
| **Q1** — جدول نتائج موحّد | حلّ 51 جدول نتائج قديم | `README.md` §6 |
| **Q2** — إصلاح الثغرات فقط | سلوك الأعمال مطابق للنظام القديم ما لم يكن خلل أمني أو سلامة بيانات | `README.md` §6 |
| **Q3** — MediatR على 12.x | تفادي الترخيص التجاري عند 13+ | `README.md` §6 |
| **Q4** — استبدال مسار PDF | لم يُنسخ سائق `Bullzip PDF Printer` | `README.md` §6 |
| **Q5** — Domain بلا مراجع | نموذج أعمال نقي قابل للاختبار | `README.md` §6 |
| **Q6** — النقود بـ `decimal` | تصحيح سلامة بيانات (الأصل كان `Double`) | `README.md` §6 |
| **Q7** — النطاق مرتبط بالجنس والعمر | اختيار حتمي (الأصل غير مُثبَت) | `README.md` §6 |
| **إزالة SMS / NATIGH.COM** | أُلغيت 5 متطلبات: `V2-REQ-047/048/049/050/072` | `Docs/USE_CASE_INVENTORY.md` §5 |

## 3. precedent التصحيح: التصحيح يسير **بجانب** الوثيقة

`Docs/RL-P5-COR-001-Documentation-Corrigendum.md` precedent مقبول:
الوثائق **سجلات مقبولة** — التصحيح يُنشر **بجانبها** لا داخلها.

لذلك:
- `ReferenceIndex/` **مجمَّد**؛ حُمي بـ `ReferenceIndex/README.md` فقط.
- `From-Phase-0-to-3-Handoff.md` **مُستبدَل** ولم يُحذف.
- `LOGIN_FAILURE_INVESTIGATION_REPORT_HISTORICAL.md` أُبقي بجانب التقرير الحيّ.

## 4. الفجوات المعروفة (مرشّحة لمرحلة لاحقة)

| الفجوة | المصدر |
|---|---|
| محرّر باركود القوالب القديم | `Docs/RL-P5-MAN-001-Phase5-Delivery-Manifest.md` §6 |
| مصمّم تخطيط التقارير لكل ملف تعريف | المرجع نفسه §6 |
| **تسلسل الطباعة** غير مُخزَّن بشكل دائم | المرجع نفسه §9 |
| **جلسة المشغّل** غير مُخزَّنة بشكل دائم | المرجع نفسه §9 |
| صيغة الـ ISI غير مُسترجَعة من الأدلة | `Docs/KB/DOMAIN-RULES.md` §9 |
| دلالات مقاطع مُعرّف الطلب (1–2، 5–9، 13) | المرجع نفسه §9 |
| قاعدة اختيار النطاق عند التداخل | المرجع نفسه §9 |

## 5. حالة التوثيق في هذه المراجعة

| البند | الحالة |
|---|---|
| `Docs/USE_CASE_INVENTORY.md` | **أُصلح** — 70 ← 73 (2026-11-14) |
| `scripts/generate_use_case_inventory.py` | **أُصلح** — خطأ `docs`/`Docs` و`EXPECTED_TOTAL` |
| `README.md` | **صُحّح** — `docs/`، عدد مشاريع الاختبار، عدد الاختبارات |
| `ReferenceIndex/` | **مجمَّد** + حارس `README.md` |
| `Docs/KB/` | **جديد** — 4 طبقات، 14 ملف مجال، 7 فهارس مولّدة، بوابة تحقق |

---

*لكل مرحلة وثيقتها. للتفاصيل افتحها؛ هنا المؤشر فقط.*
