import struct, binascii
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data = open(p,"rb").read()
IB=0x400000
def hexdump(off,n,label=""):
    print("=== %s @ %08x ===" % (label, off))
    for i in range(0,n,16):
        c=data[off+i:off+i+16]
        print("%08x  %-48s |%s|" % (off+i, binascii.hexlify(c," ").decode(), "".join(chr(x) if 32<=x<127 else "." for x in c)))
def cstr(va):
    o=va-IB
    if o<0 or o>len(data): return None
    e=data.find(b"\x00",o)
    return data[o:e].decode("latin-1","replace")
for va in (0x45e8140, 0x45e88a6, 0x45e9c40):
    hexdump(va-IB, 0x60, "VA %08x" % va)
    for i in range(0,0x40,4):
        w=struct.unpack_from("<I",data,va-IB+i)[0]
        if 0x400000 < w < 0x4900000:
            s=cstr(w)
            if s and all(32<=ord(c)<127 for c in s[:40]):
                print("   +%02x -> %08x  str=%r" % (i,w,s))
