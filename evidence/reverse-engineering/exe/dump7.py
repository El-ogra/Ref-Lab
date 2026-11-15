import struct, binascii
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data = open(p,"rb").read()
IB=0x400000
def cstr(va):
    o=va-IB
    if o<0 or o>len(data): return None
    e=data.find(b"\x00",o)
    return data[o:e].decode("latin-1","replace")
for off in range(0x260b80, 0x260d20, 16):
    c=data[off:off+16]
    print("%08x  %-48s |%s|" % (off, binascii.hexlify(c," ").decode(), "".join(chr(x) if 32<=x<127 else "." for x in c)))
print("--- interpreted as DWORD array from 0x260b80 ---")
for i in range(0, 0x1a0, 4):
    w=struct.unpack_from("<I",data,0x260b80+i)[0]
    s=""
    if 0x400000<w<0x4900000:
        t=cstr(w)
        if t and len(t)<40 and all(32<=ord(ch)<127 for ch in t): s="  -> "+repr(t)
    print("+%03x %08x%s" % (i,w,s))
