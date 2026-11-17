#!/usr/bin/env python3
"""vb6_deep2.py - annotated deep dump of VB6 per-object structures and a
scan of every object's desc+0x20 candidate pointer array."""
import struct, sys, json, re

d = open(sys.argv[1], 'rb').read()
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


def r2o(rva):
    for nm, va, vs, ra, rs in sections:
        if va <= rva < va + max(vs, rs):
            return ra + (rva - va)
    return None


def off(va):
    if va is None or va < image_base or va > image_base + 0x8000000:
        return None
    return r2o(va - image_base)


def u32(va):
    o = off(va)
    if o is None or o + 4 > len(d):
        return None
    return struct.unpack_from('<I', d, o)[0]


def astr(va, maxn=200):
    o = off(va)
    if o is None:
        return None
    end = d.find(b'\0', o, o + maxn)
    if end < 0:
        return None
    s = d[o:end]
    if len(s) > maxn:
        return None
    return s


eo = r2o(struct.unpack_from('<I', d, opt + 16)[0])
vb_va = struct.unpack_from('<I', d[eo:eo + 16], 1)[0]
pi_va = u32(vb_va + 0x30)
ot_va = u32(pi_va + 0x04)
arr_va = u32(ot_va + 0x30)
obj_count = struct.unpack_from('<H', d, off(ot_va) + 0x2c)[0]
STRIDE, NAME_OFF = 0x30, 0x18


def annot(va, label, count=64):
    o = off(va)
    print(f"\n===== {label} @VA 0x{va:08x} (off 0x{o:x}) =====")
    for i in range(count):
        v = u32(va + i * 4)
        s = ''
        if v is not None:
            sb = astr(v)
            if sb and 0 < len(sb) < 90 and all(32 <= c < 127 for c in sb):
                s = f"  -> {sb.decode('ascii')!r}"
        if v is None and s == '':
            print(f"   [{i:3d}] +0x{i*4:03x} = <unmapped>")
        else:
            print(f"   [{i:3d}] +0x{i*4:03x} = 0x{v:08x} ({v}){s}")


if len(sys.argv) > 2 and sys.argv[2] == 'range':
    annot(int(sys.argv[3], 0), sys.argv[4], int(sys.argv[5]) if len(sys.argv) > 5 else 64)
    sys.exit(0)

# per-object: dump desc+0x20 array head
print(f"objects={obj_count} arr=0x{arr_va:08x}")
tot = 0
rows = []
for i in range(obj_count):
    base = arr_va + i * STRIDE
    nmv = u32(base + NAME_OFF)
    nm = astr(nmv).decode('latin-1') if nmv and astr(nmv) else None
    mcount = u32(base + 0x1C)
    p20 = u32(base + 0x20)
    p00 = u32(base + 0x00)
    p08 = u32(base + 0x08)
    named = []
    if p20:
        for k in range(24):
            v = u32(p20 + k * 4)
            sb = astr(v) if v else None
            if sb and 0 < len(sb) < 90 and all(32 <= c < 127 for c in sb):
                named.append((k, sb.decode('ascii')))
    rows.append({'i': i, 'name': nm, 'method_count': mcount, 'p00': p00, 'p08': p08,
                 'p20': p20, 'p20_strings': named})
    tot += len(named)
print(f"objects with >=1 resolvable string at desc+0x20: {sum(1 for r in rows if r['p20_strings'])}"
      f"  total strings={tot}")
for r in rows[:20]:
    print(f"  {r['i']:3d} {str(r['name']):<22} mc={r['method_count']:<5} "
          f"p20=0x{(r['p20'] or 0):08x} strs={[s for _, s in r['p20_strings'][:6]]}")
json.dump(rows, open(sys.argv[3], 'w', encoding='utf-8'), indent=1)
