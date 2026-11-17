#!/usr/bin/env python3
"""verify_claims.py - print the actual text of specific authentic-binary
literals and search for the disputed branch-code / financial / reference-range
statements."""
import struct, sys, json, re

AUTH = sys.argv[1]
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


def off_to_va(off):
    return image_base + TVA + (off - TRA) if TRA <= off < TRA + TRS else None


lit = []
i = TRA
end = TRA + TRS
while i < end - 1:
    if (32 <= d[i] < 127 or d[i] in (9, 10, 13)) and d[i + 1] == 0:
        j = i
        chars = []
        while j + 1 < end and (32 <= d[j] < 127 or d[j] in (9, 10, 13)) and d[j + 1] == 0:
            chars.append(chr(d[j]))
            j += 2
        if len(chars) >= 3:
            lit.append((off_to_va(i), ''.join(chars)))
        i = j if j > i else i + 2
    else:
        i += 2
lit.sort()
byva = {va: s for va, s in lit}


def show(va):
    print(f"  0x{va:06x}: {byva.get(va, '<not a literal start>')!r}")


def search(needle, limit=40, show_all=False):
    n = needle.lower()
    hits = [(va, s) for va, s in lit if n in s.lower()]
    print(f"\n### search {needle!r}: {len(hits)} hits")
    for va, s in hits[:limit]:
        print(f"   0x{va:06x}  {s[:190]!r}")
    return hits


print("=== re-anchored financial / lifecycle statements ===")
for va in (0x69EAB8, 0x694CD4, 0x7226B8, 0x66F788, 0x671824, 0x66FCF4, 0x66FD98,
           0x672764, 0x6A9674, 0x6A57D8):
    show(va)

search("substring(patientid", 40)
search("as bran", 40)
search("patientid,9,1", 20)
search("patientid,10,3", 20)
search("totalpricedisc <> elmadfoo3", 10)
search("REFRANGE", 30)
search("discount100", 20)
search("omola", 12)
search("wareed", 12)
