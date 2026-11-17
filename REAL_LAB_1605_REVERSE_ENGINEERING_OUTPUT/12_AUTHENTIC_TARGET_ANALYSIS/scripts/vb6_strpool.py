#!/usr/bin/env python3
"""vb6_strpool.py - dump every NUL-terminated ASCII/UTF-16 string inside a VA
range, with its virtual address, so VB6 name pools can be read directly."""
import struct, sys, re

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


lo = int(sys.argv[2], 0)
hi = int(sys.argv[3], 0)
o0 = r2o(lo - image_base)
o1 = r2o(hi - image_base)
print(f"VA 0x{lo:08x}..0x{hi:08x}  off 0x{o0:x}..0x{o1:x}")

i = o0
while i < o1:
    j = d.find(b'\0', i, o1)
    if j < 0:
        break
    s = d[i:j]
    if len(s) >= 2 and all(32 <= c < 127 for c in s):
        va = lo + (i - o0)
        print(f"  0x{va:08x}  {s.decode('ascii')!r}")
    i = j + 1
