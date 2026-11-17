#!/usr/bin/env python3
"""vb6_deep.py - recover VB6 native object and METHOD names from the authentic
v1.605 executable by brute-forcing the PublicObjectDescriptor layout.

Read-only; stdlib only; never executes the target.
"""
import struct, sys, json, re

IDENT = re.compile(rb'^[A-Za-z_][A-Za-z0-9_]{1,63}$')


class PE:
    def __init__(self, path):
        self.data = open(path, 'rb').read()
        d = self.data
        e = struct.unpack_from('<I', d, 0x3c)[0]
        coff = e + 4
        nsec = struct.unpack_from('<H', d, coff + 2)[0]
        opt_size = struct.unpack_from('<H', d, coff + 16)[0]
        opt = coff + 20
        self.image_base = struct.unpack_from('<I', d, opt + 28)[0]
        self.ep_rva = struct.unpack_from('<I', d, opt + 16)[0]
        so = opt + opt_size
        self.sections = []
        for i in range(nsec):
            o = so + i * 40
            nm = d[o:o+8].rstrip(b'\0').decode('latin-1')
            vs, va, rs, ra = struct.unpack_from('<IIII', d, o + 8)
            self.sections.append((nm, va, vs, ra, rs))

    def r2o(self, rva):
        for nm, va, vs, ra, rs in self.sections:
            if va <= rva < va + max(vs, rs):
                return ra + (rva - va)
        return None

    def off(self, va):
        if va is None or va <= 0x400000 or va > 0x400000 + 0x8000000:
            return None
        return self.r2o(va - self.image_base)

    def u32(self, va):
        o = self.off(va)
        if o is None or o + 4 > len(self.data):
            return None
        return struct.unpack_from('<I', self.data, o)[0]

    def u16(self, va):
        o = self.off(va)
        if o is None or o + 2 > len(self.data):
            return None
        return struct.unpack_from('<H', self.data, o)[0]

    def astr(self, va, maxn=256):
        o = self.off(va)
        if o is None:
            return None
        end = self.data.find(b'\0', o, o + maxn)
        if end < 0:
            return None
        return self.data[o:end]

    def astr_u16(self, va, maxn=256):
        """UTF-16LE string (VB6 stores many names as Unicode)."""
        o = self.off(va)
        if o is None:
            return None
        chunk = self.data[o:o + maxn * 2]
        end = chunk.find(b'\0\0')
        if end < 0:
            return None
        raw = chunk[:end + 1]
        if len(raw) % 2:
            raw += b'\0'
        try:
            return raw.decode('utf-16-le').encode('utf-8')
        except Exception:
            return None


def main():
    pe = PE(sys.argv[1])
    d = pe.data
    eo = pe.r2o(pe.ep_rva)
    stub = d[eo:eo + 16]
    assert stub[0] == 0x68 and stub[5] == 0xE8, "not a VB6 entry stub"
    vb_va = struct.unpack_from('<I', stub, 1)[0]
    vbo = pe.off(vb_va)
    assert d[vbo:vbo+4] == b'VB5!'

    pi_va = pe.u32(vb_va + 0x30)
    ot_va = pe.u32(pi_va + 0x04)
    oto = pe.off(ot_va)

    print(f"VBHeader VA=0x{vb_va:08x} ProjectInfo=0x{pi_va:08x} ObjectTable=0x{ot_va:08x}")

    obj_count = struct.unpack_from('<H', d, oto + 0x2c)[0]
    compiled = struct.unpack_from('<H', d, oto + 0x2e)[0]
    print(f"object_count={obj_count} compiled={compiled}")

    # candidate object-array pointers: every dword in the object table that is a
    # plausible VA, plus the known +0x30
    cand = []
    for off in range(0, 0x60, 4):
        v = struct.unpack_from('<I', d, oto + off)[0]
        if pe.off(v) is not None:
            cand.append((off, v))
    print("candidate object-array pointer slots:", [(hex(o), hex(v)) for o, v in cand])

    best = None
    for slot_off, arr_va in cand:
        for stride in (0x30, 0x38, 0x28, 0x40, 0x34, 0x2C):
            for name_off in range(0x00, 0x34, 4):
                hits = 0
                for i in range(min(obj_count, 212)):
                    base = arr_va + i * stride
                    nv = pe.u32(base + name_off)
                    if not nv:
                        continue
                    s = pe.astr(nv)
                    if s and IDENT.match(s):
                        hits += 1
                if hits >= 100:
                    score = (hits, stride, name_off)
                    if best is None or hits > best[0]:
                        best = (hits, arr_va, stride, name_off, slot_off)
    print("BEST:", best)
    if best:
        hits, arr_va, stride, name_off, slot_off = best
        print(f"array_va=0x{arr_va:08x} stride=0x{stride:x} name_off=0x{name_off:x} "
              f"(from OT+0x{slot_off:x}) hits={hits}")
        objs = []
        for i in range(obj_count):
            base = arr_va + i * stride
            nv = pe.u32(base + name_off)
            nm = pe.astr(nv).decode('latin-1') if nv and pe.astr(nv) else None
            objs.append({'i': i, 'name': nm, 'base': base})
        json.dump({'objects': objs, 'arr_va': arr_va, 'stride': stride,
                   'name_off': name_off, 'ot_slot': slot_off},
                  open(sys.argv[2], 'w', encoding='utf-8'), indent=1)
        print("first 12 names:", [o['name'] for o in objs[:12]])


if __name__ == '__main__':
    main()
