#!/usr/bin/env python3
"""audit_prior_corpus.py - verify the prior reverse-engineering corpus
(statements CSV + DDL evidence) against the AUTHENTIC v1.605 binary.

For every prior row we:
  1. interpret the prior 'va' column as a VA in the v1.605 image and resolve
     which UTF-16LE literal actually occupies that address;
  2. locate the prior statement text in the authentic binary and report its
     real address;
  3. report agreement / mismatch statistics.
"""
import struct, sys, json, re, csv, io, os
from collections import Counter

AUTH = sys.argv[1]
PRIOR_CSV = sys.argv[2]
PRIOR_DDL = sys.argv[3]
OUT = sys.argv[4]

d = open(AUTH, 'rb').read()
e = struct.unpack_from('<I', d, 0x3c)[0]
coff = e + 4
nsec = struct.unpack_from('<H', d, coff + 2)[0]
opt_size = struct.unpack_from('<H', d, coff + 16)[0]
opt = coff + 20
image_base = struct.unpack_from('<I', d, opt + 28)[0]
so = opt + opt_size
sections = []
for i in range(nsec):
    o = so + i * 40
    nm = d[o:o+8].rstrip(b'\0').decode('latin-1')
    vs, va, rs, ra = struct.unpack_from('<IIII', d, o + 8)
    sections.append((nm, va, vs, ra, rs))
t = [s for s in sections if s[0] == '.text'][0]
TN, TVA, TVS, TRA, TRS = t


def va_to_off(va):
    rva = va - image_base
    if TVA <= rva < TVA + max(TVS, TRS):
        return TRA + (rva - TVA)
    return None


def off_to_va(off):
    if TRA <= off < TRA + TRS:
        return image_base + TVA + (off - TRA)
    return None


# ---- index every UTF-16LE literal in .text -------------------------------
lit = []  # (va, off, text)
i = TRA
end = TRA + TRS
while i < end - 1:
    if (32 <= d[i] < 127 or d[i] in (9, 10, 13)) and d[i + 1] == 0:
        j = i
        chars = []
        while j + 1 < end and (32 <= d[j] < 127 or d[j] in (9, 10, 13)) and d[j + 1] == 0:
            chars.append(chr(d[j]))
            j += 2
        if len(chars) >= 4:
            lit.append((off_to_va(i), i, ''.join(chars)))
        i = j if j > i else i + 2
    else:
        i += 2
lit.sort()
print(f"authentic v1.605: {len(lit)} UTF-16LE literals in .text")

starts = [x[0] for x in lit]


def literal_containing(va):
    """literal whose VA range contains va"""
    lo, hi = 0, len(lit) - 1
    best = None
    for k in range(len(lit)):
        sva, soff, stxt = lit[k]
        if sva <= va:
            best = lit[k]
        else:
            break
    if best is None:
        return None
    sva, soff, stxt = best
    length = len(stxt) * 2
    if va < sva + length:
        return (sva, stxt)
    return None


# index by text for locating the prior statement text
text_index = {}
for sva, soff, stxt in lit:
    text_index.setdefault(stxt, []).append(sva)
print(f"distinct literal texts: {len(text_index)}")


def norm(s):
    return re.sub(r'\s+', ' ', s).strip().lower()


norm_index = {}
for sva, soff, stxt in lit:
    norm_index.setdefault(norm(stxt), []).append(sva)

# ---- read prior corpus ---------------------------------------------------
rows = []
with open(PRIOR_CSV, 'r', encoding='utf-8-sig', errors='replace', newline='') as fh:
    rdr = csv.DictReader(fh)
    for r in rdr:
        rows.append(r)
print(f"prior statements csv rows: {len(rows)}")

ddl_rows = []
with open(PRIOR_DDL, 'r', encoding='utf-8-sig', errors='replace') as fh:
    for line in fh:
        if line.startswith('#') or not line.strip():
            continue
        parts = line.rstrip('\n').split('\t', 1)
        if len(parts) == 2:
            ddl_rows.append({'va': parts[0].strip(), 'text': parts[1]})
print(f"prior ddl rows: {len(ddl_rows)}")

report = {'authentic': AUTH, 'literal_count': len(lit)}
stats = Counter()
detail = []

for src, items, key in (('csv', rows, 'text'), ('ddl', ddl_rows, 'text')):
    for it in items:
        prior_va_raw = it.get('va') or it.get('va') or ''
        try:
            prior_va = int(str(prior_va_raw).strip(), 16)
        except Exception:
            stats[f'{src}_badva'] += 1
            continue
        txt = it.get(key) or ''
        ntxt = norm(txt)
        # (a) what actually lives at that VA
        lit_at = literal_containing(prior_va)
        # (b) where the prior text really is
        hits = norm_index.get(ntxt, [])
        # partial: prior texts are sometimes prefixes/substrings of real literals
        partial = []
        if not hits and len(ntxt) >= 25:
            for k, v in norm_index.items():
                if ntxt in k:
                    partial = v
                    break
        if hits:
            stats[f'{src}_text_found'] += 1
            same_va = (prior_va in hits)
            stats[f'{src}_va_exact'] += 1 if same_va else 0
            stats[f'{src}_va_mismatch'] += 0 if same_va else 1
        elif partial:
            stats[f'{src}_text_partial'] += 1
            stats[f'{src}_va_mismatch_partial'] += 1
        else:
            stats[f'{src}_text_absent'] += 1
        detail.append({
            'source': src, 'prior_va': prior_va,
            'literal_at_prior_va': (lit_at[1][:220] if lit_at else None),
            'prior_text': txt[:220],
            'real_vas': (hits or partial)[:6],
            'found': bool(hits), 'partial': bool(partial),
        })

report['stats'] = dict(stats)
report['details'] = detail
json.dump(report, open(OUT, 'w', encoding='utf-8'), indent=1)
for k in sorted(stats):
    print(f"  {k:32s} {stats[k]}")
print("wrote", OUT)
