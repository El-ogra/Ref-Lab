#!/usr/bin/env python3
"""bind_verify.py - verify the SQL-literal -> code-site binding by locating
the literal's virtual address as an immediate operand in .text and printing the
surrounding instruction bytes."""
import struct, sys, json

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
blob = d[TRA:TRA + TRS]


def off_to_va(off):
    if TRA <= off < TRA + TRS:
        return image_base + TVA + (off - TRA)
    return None


def va_to_off(va):
    rva = va - image_base
    if TVA <= rva < TVA + max(TVS, TRS):
        return TRA + (rva - TVA)
    return None


items = json.load(open(sys.argv[2], encoding='utf-8'))
checked = 0
kinds = {}
for it in items:
    va = it['va']
    needle = struct.pack('<I', va)
    start = 0
    while True:
        k = blob.find(needle, start)
        if k < 0:
            break
        start = k + 1
        # classify by the instruction bytes preceding the immediate
        ctx = blob[max(0, k - 6):k]
        kind = None
        if len(ctx) >= 5 and ctx[-5] == 0x68:
            kind = 'push_imm32'
        elif len(ctx) >= 6 and ctx[0] == 0xC7 and ctx[1] == 0x45:
            kind = 'mov_dword_ptr_ebp_disp_imm32'
        elif len(ctx) >= 1 and 0xB8 <= ctx[-1] <= 0xBF:
            kind = 'mov_reg_imm32'
        elif len(ctx) >= 6 and ctx[0] == 0xC7 and (ctx[1] & 0xC0) == 0x40:
            kind = 'mov_dword_ptr_eax_disp_imm32'
        elif len(ctx) >= 2 and ctx[-2] == 0x05:
            kind = 'add_eax_imm32'
        elif len(ctx) >= 2 and ctx[-2] == 0x0D:
            kind = 'add_ecx_imm32'
        elif len(ctx) >= 6 and ctx[0] == 0x3D:
            kind = 'cmp_eax_imm32'
        elif len(ctx) >= 2 and ctx[-2] == 0x35:
            kind = 'xor_eax_imm32'
        kinds[kind] = kinds.get(kind, 0) + 1
        checked += 1
        if checked <= 12:
            site = off_to_va(TRA + k)
            print(f"literal VA=0x{va:08x} -> code VA=0x{site:08x} kind={kind}")
            print(f"    bytes[-10:+6]: {blob[max(0,k-10):k+6].hex(' ')}")
            print(f"    stmt: {it['text'][:120]!r}")
print(f"\nchecked {checked} immediate-operand occurrences")
for k2, v in sorted(kinds.items(), key=lambda x: -x[1]):
    print(f"   {str(k2):42s} {v}")
