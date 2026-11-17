#!/usr/bin/env python3
"""vb6_controls.py - enumerate every VB6 form's control table in the authentic
v1.605 binary.

Verified layout: ObjectInfo+0x34 -> control-table pointer; each control is a
16-byte record (event_descriptor_ptr, handler_code_address, control_name_ptr,
control_flags) inside a heterogeneous 16-byte record array.
"""
import struct, sys, json, re

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
CODE_LO = image_base + TVA
CODE_HI = image_base + TVA + TVS


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


def astr(va, maxn=80):
    o = off(va)
    if o is None:
        return None
    end = d.find(b'\0', o, o + maxn)
    if end < 0:
        return None
    s = d[o:end]
    if not s or len(s) > maxn:
        return None
    try:
        return s.decode('ascii')
    except UnicodeDecodeError:
        return None


def plausible_code(va):
    if va is None or not (CODE_LO <= va < CODE_HI):
        return False
    o = off(va)
    if o is None:
        return False
    b = d[o:o + 3]
    if not b:
        return False
    return b[0] in (0x55, 0x68, 0x56, 0x57, 0x53, 0x51, 0x52, 0x50, 0x83, 0x8B,
                    0x89, 0xE8, 0x6A, 0xFF, 0x33, 0x31, 0x85, 0x0F, 0x48)


eo = r2o(struct.unpack_from('<I', d, opt + 16)[0])
vb_va = struct.unpack_from('<I', d[eo:eo + 16], 1)[0]
pi_va = u32(vb_va + 0x30)
ot_va = u32(pi_va + 0x04)
arr_va = u32(ot_va + 0x30)
obj_count = struct.unpack_from('<H', d, off(ot_va) + 0x2c)[0]
STRIDE, NAME_OFF = 0x30, 0x18

objs = []
tot = 0
for i in range(obj_count):
    base = arr_va + i * STRIDE
    nmv = u32(base + NAME_OFF)
    nmb = astr(nmv)
    name = nmb if nmb else None
    oi = u32(base + 0x00)
    ctl_ptr = u32(oi + 0x34) if oi else None
    controls = []
    if ctl_ptr and off(ctl_ptr) is not None:
        for rec in range(0, 0x6000, 4):
            p = ctl_ptr + rec
            addr = u32(p + 4)
            if not plausible_code(addr):
                continue
            nmp = u32(p + 8)
            if not nmp:
                continue
            cn = astr(nmp)
            if not cn or len(cn) > 40 or not cn[0].isalpha():
                continue
            fl = u32(p + 12)
            if fl is None or not (0x30000 <= fl <= 0x3FFFF):
                continue
            controls.append({'control': cn, 'flags': fl, 'handler_va': addr,
                             'record_va': p})
    objs.append({'i': i, 'name': name, 'object_info_va': oi,
                 'control_table_va': ctl_ptr, 'controls': controls})
    tot += len(controls)

print(f"objects={obj_count}  control records with a handler address={tot}")
for o in objs[:10]:
    print(f"  {o['i']:3d} {str(o['name']):<22} ctl_ptr=0x{(o['control_table_va'] or 0):08x} "
          f"controls={len(o['controls'])} "
          f"{[(c['control'], hex(c['handler_va'])) for c in o['controls'][:4]]}")
json.dump(objs, open(OUT, 'w', encoding='utf-8'), indent=1)
print("wrote", OUT)
