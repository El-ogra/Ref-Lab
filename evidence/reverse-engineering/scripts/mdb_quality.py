"""Focused data-quality analysis: true duplicates, referential orphans,
duplicate business identifiers, and the patients/operator table profile."""
import io
import json
import os
from collections import Counter, defaultdict

EXP = r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\mdb\exports\tables"

T = {}
for name in sorted(os.listdir(EXP)):
    if not name.endswith(".json"):
        continue
    with io.open(os.path.join(EXP, name), encoding="utf-8") as fh:
        d = json.load(fh)
    T[d["table"]] = d


def rows(t):
    return T[t]["rows"]


def col(t, c):
    return [c["name"] for c in T[t]["columns"]]


print("=" * 78)
print("A. WHOLE-ROW DUPLICATES (identical rows in the same table)")
print("=" * 78)
for t in sorted(T):
    rs = rows(t)
    if len(rs) < 2:
        continue
    keys = [tuple(sorted((k, str(v)) for k, v in r.items())) for r in rs]
    dup = {k: c for k, c in Counter(keys).items() if c > 1}
    if dup:
        n = sum(c - 1 for c in dup.values())
        print("   %-20s %d rows, %d duplicated row group(s), %d surplus row(s)" %
              (t, len(rs), len(dup), n))

print()
print("=" * 78)
print("B. DUPLICATE BUSINESS IDENTIFIERS")
print("=" * 78)
print("B1. testtable.testname duplicates (exact string, case-insensitive)")
tt = rows("testtable")
names = [r.get("testname") for r in tt if r.get("testname")]
dupn = {k: c for k, c in Counter(n.lower() for n in names).items() if c > 1}
print("   testtable rows=%d distinct test names=%d duplicated names=%d" %
      (len(tt), len(set(n.lower() for n in names)), len(dupn)))
for k, c in sorted(dupn.items())[:15]:
    codes = [r.get("testcode") for r in tt if (r.get("testname") or "").lower() == k]
    print("      '%s' x%d  testcodes=%s" % (k[:46], c, codes))
print("   testtable.testcode duplicates: %s" %
      ({k: c for k, c in Counter([r.get("testcode") for r in tt]).items() if c > 1} or "none"))

print()
print("B2. customtesttable / customgrouptable")
for t in ("customtesttable", "customgrouptable"):
    rs = rows(t)
    cs = col(t, None)
    for c in cs:
        vals = [r.get(c) for r in rs if r.get(c) not in (None, "")]
        d = {k: v for k, v in Counter(vals).items() if v > 1}
        if d:
            print("   %-18s column %-14s %d duplicate value(s): %s" %
                  (t, c, len(d), list(d.items())[:6]))

print()
print("=" * 78)
print("C. REFERENTIAL ORPHAN SCREEN (child -> parent)")
print("=" * 78)


def distinct(t, c):
    return set(r.get(c) for r in rows(t) if r.get(c) not in (None, ""))


parents = {
    "testtable.testcode": ("testtable", "testcode"),
    "subgrouptable.subgroupcode": ("subgrouptable", "subgroupcode"),
    "normalrange.testcode": ("testtable", "testcode"),
    "normalrange.subgroupcode": ("subgrouptable", "subgroupcode"),
    "patienttest.testcode": ("testtable", "testcode"),
    "patienttest.subgroupcode": ("subgrouptable", "subgroupcode"),
    "patientinfo.patientid": ("patients", "patientid"),
    "comments.testcode": ("testtable", "testcode"),
    "customtesttable.testcode": ("testtable", "testcode"),
}
for child, (pt, pc) in parents.items():
    ct, cc = child.split(".")
    if ct not in T or pt not in T:
        continue
    if cc not in col(ct, None):
        continue
    child_vals = distinct(ct, cc)
    if not child_vals:
        print("   %-34s -> %-26s : child table empty" % (child, pt + "." + pc))
        continue
    pvals = distinct(pt, pc)
    orphan = child_vals - pvals
    print("   %-34s -> %-26s : %d distinct child value(s), %d orphan(s)%s" %
          (child, pt + "." + pc, len(child_vals), len(orphan),
           (" e.g. " + ", ".join(str(x)[:16] for x in list(orphan)[:5])) if orphan else ""))

print()
print("=" * 78)
print("D. OPERATOR ACCOUNT TABLE 'patients' (2 rows) — profile only")
print("=" * 78)
for r in rows("patients"):
    shown = {k: (v if k not in ("patientword", "SecWord") else "<REDACTED>") for k, v in r.items() if v not in (None, "")}
    print("   " + json.dumps(shown, ensure_ascii=False)[:400])

print()
print("=" * 78)
print("E. EMPTY-STRING DENSITY BY TABLE (top 12)")
print("=" * 78)
dens = []
for t in sorted(T):
    rs = rows(t)
    if not rs:
        continue
    cells = sum(len(r) for r in rs)
    empt = sum(1 for r in rs for v in r.values() if v == "")
    dens.append((empt / cells, t, cells, empt, len(rs)))
dens.sort(reverse=True)
for pct, t, cells, empt, n in dens[:12]:
    print("   %-20s rows=%-5d cells=%-7d empty=%-6d (%.1f%%)" % (t, n, cells, empt, pct * 100))

print()
print("=" * 78)
print("F. TABLES WITH THE LARGEST ROW COUNTS")
print("=" * 78)
big = sorted(((len(v["rows"]), k) for k, v in T.items()), reverse=True)[:15]
for n, t in big:
    print("   %-20s %6d rows  %3d cols" % (t, n, len(T[t]["columns"])))
