#!/usr/bin/env python3
"""sqlbind.py - definitive SQL-literal -> code-site binding for the v1.605
binary, with correct x86 immediate-operand classification, plus DDL clustering
to localise the migration routine."""
import struct, sys, json
from collections import Counter, defaultdict

AUTH = sys.argv[1]
OUT = sys.argv[2]
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
blob = d[TRA:TRA + TRS]


def off_to_va(off):
    if TRA <= off < TRA + TRS:
        return image_base + TVA + (off - TRA)
    return None


def classify(ctx):
    """ctx = up to 8 bytes preceding the immediate operand."""
    n = len(ctx)
    if n >= 1 and ctx[-1] == 0x68:
        return 'push_imm32'
    if n >= 6 and ctx[-5] == 0xC7 and ctx[-4] == 0x45:
        return 'mov_dword_ptr_ebp8_imm32'
    if n >= 6 and ctx[-5] == 0xC7 and (ctx[-4] & 0xC0) == 0x40:
        return 'mov_dword_ptr_disp8_imm32'
    if n >= 6 and ctx[-5] == 0xC7 and ctx[-4] == 0x85:
        return 'mov_dword_ptr_ebp32_imm32'
    if n >= 1 and 0xB8 <= ctx[-1] <= 0xBF:
        return 'mov_reg_imm32'
    if n >= 2 and ctx[-2] == 0x05:
        return 'add_eax_imm32'
    if n >= 2 and ctx[-2] == 0x0D:
        return 'add_ecx_imm32'
    if n >= 2 and ctx[-2] == 0x35:
        return 'xor_eax_imm32'
    if n >= 1 and ctx[-1] == 0x3D:
        return 'cmp_eax_imm32'
    if n >= 2 and ctx[-2] == 0x25:
        return 'and_eax_imm32'
    if n >= 2 and ctx[-2] == 0x2D:
        return 'sub_eax_imm32'
    return 'unknown'


items = json.load(open(sys.argv[3], encoding='utf-8'))
out = []
kinds = Counter()
bound = 0
sites_all = []
for it in items:
    va = it['va']
    needle = struct.pack('<I', va)
    sites = []
    start = 0
    while True:
        k = blob.find(needle, start)
        if k < 0:
            break
        start = k + 1
        ctx = blob[max(0, k - 8):k]
        kind = classify(ctx)
        kinds[kind] += 1
        site_va = off_to_va(TRA + k)
        sites.append({'code_va': site_va, 'kind': kind,
                      'bytes': blob[max(0, k - 6):k + 4].hex(' ')})
        sites_all.append((site_va, it))
    if sites:
        bound += 1
    out.append({'literal_va': va, 'keyword': it['text'].lstrip().split()[0].upper()
                if it['text'].strip() else '',
                'text': it['text'], 'sites': sites})

print(f"SQL/DDL literals: {len(out)}   with >=1 code site: {bound} ({100.0*bound/len(out):.1f}%)")
print(f"total code sites: {sum(len(o['sites']) for o in out)}")
print("site kinds:")
for k, v in kinds.most_common():
    print(f"   {k:38s} {v}")

# DDL clustering
ddl = [o for o in out if o['keyword'] in ('ALTER', 'CREATE', 'DROP')]
dsites = sorted({(s['code_va'], o['literal_va']) for o in ddl for s in o['sites']})
print(f"\nDDL literals: {len(ddl)}  distinct DDL code sites: {len(dsites)}")
if dsites:
    lo, hi = dsites[0][0], dsites[-1][0]
    print(f"DDL code-site VA range: 0x{lo:08x} .. 0x{hi:08x}")
    buckets = Counter(v >> 16 for v, _ in dsites)
    print("DDL site clusters (64 KB buckets), top 10:")
    for b, n in buckets.most_common(10):
        print(f"   0x{b << 16:08x}..0x{(b + 1) << 16:08x}  {n}")
    # how many DDL sites fall inside the single densest 512KB window
    best = (0, 0)
    for i, (v, _) in enumerate(dsites):
        j = i
        while j < len(dsites) and dsites[j][0] - v < 0x80000:
            j += 1
        if j - i > best[0]:
            best = (j - i, v)
    print(f"densest 512 KB window starts at 0x{best[1]:08x} containing {best[0]} DDL sites")

# SELECT site distribution
sel = [o for o in out if o['keyword'] == 'SELECT']
ssites = sorted({s['code_va'] for o in sel for s in o['sites']})
print(f"\nSELECT literals: {len(sel)}  distinct code sites: {len(ssites)}")
if ssites:
    print(f"SELECT code-site VA range: 0x{ssites[0]:08x} .. 0x{ssites[-1]:08x}")
    buckets = Counter(v >> 16 for v in ssites)
    for b, n in buckets.most_common(6):
        print(f"   0x{b << 16:08x}..0x{(b + 1) << 16:08x}  {n}")

json.dump({'literals': out, 'kinds': dict(kinds)}, open(OUT, 'w', encoding='utf-8'), indent=1)
print("wrote", OUT)
