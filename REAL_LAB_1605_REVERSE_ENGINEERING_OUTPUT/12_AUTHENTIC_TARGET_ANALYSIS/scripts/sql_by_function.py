#!/usr/bin/env python3
"""sql_by_function.py - stream Ghidra's function-reference dump and produce a
compact method->SQL mapping: for each function, the SQL statements it
references, with counts."""
import sys, re, json
from collections import defaultdict

FUNC_REFS = sys.argv[1]
FUNCS = sys.argv[2]
OUT = sys.argv[3]
SQLPAT = re.compile(r'^\s*(select|insert|update|delete|alter|create|drop|shape|exec|with)\b', re.I)

# function metadata
meta = {}
with open(FUNCS, encoding='utf-8') as fh:
    next(fh)
    for line in fh:
        p = line.rstrip('\n').split('\t')
        if len(p) >= 3:
            meta[p[0]] = (p[1], p[2], p[3] if len(p) > 3 else '')

sql_by_fn = defaultdict(list)
other_by_fn = defaultdict(list)
total_refs = 0
with open(FUNC_REFS, encoding='utf-8', errors='replace') as fh:
    header = next(fh)
    for line in fh:
        p = line.rstrip('\n').split('\t')
        if len(p) < 6:
            continue
        total_refs += 1
        fn_entry, fn_name, ins, rtype, target, text = p[0], p[1], p[2], p[3], p[4], p[5]
        if not text or text.startswith('<SYM:'):
            continue
        if SQLPAT.match(text):
            sql_by_fn[(fn_entry, fn_name)].append((target, text))
        else:
            other_by_fn[(fn_entry, fn_name)].append(text)

print(f"streamed {total_refs} function references")
print(f"functions referencing >=1 SQL statement: {len(sql_by_fn)}")
print(f"functions referencing non-SQL strings: {len(other_by_fn)}")

rows = []
for (entry, name), items in sql_by_fn.items():
    uniq = {}
    for target, text in items:
        uniq.setdefault(target, text)
    size = meta.get(entry, ('', '', ''))[1]
    rows.append((len(uniq), entry, name, size,
                 sorted(uniq.items(), key=lambda kv: -len(kv[1]))))
rows.sort(reverse=True)
with open(OUT, 'w', encoding='utf-8') as fh:
    fh.write("func_entry\tfunc_name\tfunc_size\tdistinct_sql_statements\tstatements\n")
    for n, entry, name, size, items in rows:
        fh.write(f"{entry}\t{name}\t{size}\t{n}\t")
        fh.write(" ;; ".join(f"{t}: {txt[:300]}" for t, txt in items[:40]))
        fh.write("\n")
print(f"wrote {OUT}  ({len(rows)} functions with SQL)")

# top functions by number of distinct SQL statements
print("\nTop 20 functions by distinct SQL statements:")
for n, entry, name, size, items in rows[:20]:
    print(f"  {n:4d}  {entry}  {name}  size={size}  first={items[0][1][:70]!r}")

# functions referencing non-SQL literals (UI text etc.) - the reporting layer
with open(OUT + '.nonsql.tsv', 'w', encoding='utf-8') as fh:
    fh.write("func_entry\tfunc_name\tdistinct_strings\tsample\n")
    for (entry, name), items in sorted(other_by_fn.items(),
                                      key=lambda kv: -len(set(kv[1]))):
        uniq = sorted(set(items), key=lambda s: -len(s))
        fh.write(f"{entry}\t{name}\t{len(uniq)}\t" + " ;; ".join(s[:200] for s in uniq[:25]) + "\n")
print(f"wrote {OUT}.nonsql.tsv")
