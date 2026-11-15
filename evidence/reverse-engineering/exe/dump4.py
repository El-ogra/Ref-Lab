import struct, binascii
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data = open(p,"rb").read()
IB=0x400000
def hexdump(off,n,label=""):
    print("=== %s @ %08x ===" % (label, off))
    for i in range(0,n,16):
        c=data[off+i:off+i+16]
        print("%08x  %-48s |%s|" % (off+i, binascii.hexlify(c," ").decode(), "".join(chr(x) if 32<=x<127 else "." for x in c)))
def u32(o): return struct.unpack_from("<I", data, o)[0]
def u16(o): return struct.unpack_from("<H", data, o)[0]
for va in (0x460b98, 0x4bd64c+0x4):
    hexdump(va-IB, 0x100, "VA %08x" % va)
ot = 0x460b98
print("OT fields:")
for i in range(0,0x60,4):
    print(" +%02x = %08x" % (i, u32(ot-IB+i)))
