#!/usr/bin/env python3
"""vb6_sql2.py - extract UTF-16LE SQL statement literals from a VB6 native
binary, classify them by keyword, and bind each to referencing code sites."""
import struct, sys, json, re
from collections import Counter


class Img:
    def __init__(self, path):
        self.d = open(path, 'rb').read()
        d = self.d
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
        t = [s for s in self.sections if s[0] == '.text'][0]
        self.tn, self.tva, self.tvs, self.tra, self.trs = t

    def off_to_va(self, off):
        if self.tra <= off < self.tra + self.trs:
            return self.image_base + self.tva + (off - self.tra)
        return None

    def va_to_off(self, va):
        rva = va - self.image_base
        if self.tva <= rva < self.tva + max(self.tvs, self.trs):
            return self.tra + (rva - self.tva)
        return None

    def u32_at_off(self, off):
        if off + 4 > len(self.d):
            return None
        return struct.unpack_from('<I', self.d, off)[0]


SQL_HEAD = re.compile(
    r'^\s*(SELECT|INSERT|UPDATE|DELETE|ALTER|CREATE|DROP|SHAPE|EXEC|EXECUTE|'
    r'TRUNCATE|GRANT|REVOKE|COMMIT|ROLLBACK|VALUES|WITH)\b', re.I)


def utf16_strings(img, minlen=6):
    """Return [(va, text)] for maximal UTF-16LE printable runs in .text."""
    d = img.d
    out = []
    tra, trs = img.tra, img.trs
    i = tra
    end = tra + trs
    while i < end - 1:
        # find a run of (printable, 0x00) pairs
        if (32 <= d[i] < 127 or d[i] in (9, 10, 13)) and d[i + 1] == 0:
            j = i
            chars = []
            while j + 1 < end and (32 <= d[j] < 127 or d[j] in (9, 10, 13)) and d[j + 1] == 0:
                chars.append(chr(d[j]))
                j += 2
            if len(chars) >= minlen:
                va = img.off_to_va(i)
                out.append((va, ''.join(chars)))
            i = j if j > i else i + 2
        else:
            i += 2
    return out


def main():
    img = Img(sys.argv[1])
    label = sys.argv[2] if len(sys.argv) > 2 else 'img'
    outjson = sys.argv[3] if len(sys.argv) > 3 else None
    allstr = utf16_strings(img)
    print(f"[{label}] UTF-16LE strings in .text: {len(allstr)}")
    sql = [(va, t) for va, t in allstr if SQL_HEAD.match(t)]
    kw = Counter(SQL_HEAD.match(t).group(1).upper() for _, t in sql)
    print(f"[{label}] SQL-like literals: {len(sql)}")
    for k, v in kw.most_common():
        print(f"    {k:9s} {v}")

    # bind to code sites
    blob = img.d[img.tra:img.tra + img.trs]
    items = []
    bound = 0
    sites_total = 0
    for va, t in sql:
        needle = struct.pack('<I', va)
        sites = []
        start = 0
        while True:
            k = blob.find(needle, start)
            if k < 0:
                break
            start = k + 1
            prev = blob[k - 1] if k >= 1 else None
            kind = None
            if prev == 0x68:
                kind = 'push_imm32'
            elif prev is not None and 0xB8 <= prev <= 0xBF:
                kind = 'mov_reg_imm32'
            elif prev is not None and prev in (0x05, 0x0D, 0x15, 0x1D, 0x25, 0x2D, 0x35, 0x3D):
                kind = 'add_reg_imm32'
            if kind:
                sites.append({'site_va': img.off_to_va(img.tra + k), 'kind': kind})
        if sites:
            bound += 1
        sites_total += len(sites)
        items.append({'va': va, 'text': t, 'sites': sites})
    print(f"[{label}] literals with >=1 code site: {bound}/{len(sql)}  total sites={sites_total}")
    kinds = Counter(s['kind'] for it in items for s in it['sites'])
    print(f"[{label}] site kinds: {dict(kinds)}")

    if outjson:
        json.dump(items, open(outjson, 'w', encoding='utf-8'), indent=1)
        print(f"[{label}] wrote {outjson}")
    return items


if __name__ == '__main__':
    main()
