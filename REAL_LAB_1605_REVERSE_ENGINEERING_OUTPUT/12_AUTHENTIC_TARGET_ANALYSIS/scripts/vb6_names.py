#!/usr/bin/env python3
"""vb6_names.py - harvest VB6 object + method names from the authentic v1.605
binary using the verified PublicObjectDescriptor layout
(name @ +0x18, methodCount @ +0x1C, aMethodNames @ +0x20, stride 0x30)."""
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


def astr(va, maxn=250):
    o = off(va)
    if o is None:
        return None
    end = d.find(b'\0', o, o + maxn)
    if end < 0:
        return None
    s = d[o:end]
    return s if s else None


eo = r2o(struct.unpack_from('<I', d, opt + 16)[0])
vb_va = struct.unpack_from('<I', d[eo:eo + 16], 1)[0]
pi_va = u32(vb_va + 0x30)
ot_va = u32(pi_va + 0x04)
arr_va = u32(ot_va + 0x30)
obj_count = struct.unpack_from('<H', d, off(ot_va) + 0x2c)[0]
compiled = struct.unpack_from('<H', d, off(ot_va) + 0x2e)[0]
STRIDE, NAME_OFF = 0x30, 0x18

VTYPES = {1: 'Form', 2: 'Module', 3: 'Class', 4: 'Designer', 5: 'PropertyPage',
          6: 'UserControl', 7: 'MSForm', 8: 'RelatedDocument',
          9: 'PropertyProcedure', 10: 'Event', 11: 'EventHandler',
          12: 'EventStub', 13: 'MenuControl'}

objs = []
tot_methods = 0
tot_named = 0
for i in range(obj_count):
    base = arr_va + i * STRIDE
    nmv = u32(base + NAME_OFF)
    nmb = astr(nmv)
    name = nmb.decode('latin-1') if nmb else None
    mc = u32(base + 0x1C)
    p20 = u32(base + 0x20)
    p00 = u32(base + 0x00)
    p08 = u32(base + 0x08)
    p10 = u32(base + 0x10)
    p14 = u32(base + 0x14)
    f28 = u32(base + 0x28)
    methods = []
    if p20:
        limit = (mc if mc and mc < 4000 else 64) + 24
        for k in range(min(limit, 1024)):
            v = u32(p20 + k * 4)
            if not v:
                continue
            s = astr(v)
            if s and 0 < len(s) < 200 and all(32 <= c < 127 for c in s):
                methods.append({'index': k, 'name': s.decode('ascii'), 'name_va': v})
    tot_methods += (mc or 0)
    tot_named += len(methods)
    objs.append({'i': i, 'name': name, 'method_count': mc, 'methods': methods,
                 'desc_va': base, 'p00': p00, 'p08': p08, 'p10': p10, 'p14': p14,
                 'f28': f28})

print(f"objects={obj_count} compiled={compiled} sum(methodCount)={tot_methods} "
      f"named_methods_recovered={tot_named}")
print(f"objects_with_any_named_method={sum(1 for o in objs if o['methods'])}")
for o in objs[:12]:
    print(f"  {o['i']:3d} {str(o['name']):<20} mc={o['method_count']:<5} "
          f"n={len(o['methods'])} {[m['name'] for m in o['methods'][:6]]}")
json.dump({'vb_header_va': vb_va, 'object_table_va': ot_va, 'object_array_va': arr_va,
           'object_count': obj_count, 'compiled_objects': compiled,
           'sum_method_count': tot_methods, 'named_methods_recovered': tot_named,
           'objects': objs}, open(sys.argv[2], 'w', encoding='utf-8'), indent=1)
