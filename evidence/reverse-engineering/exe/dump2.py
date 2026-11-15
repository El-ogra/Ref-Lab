import struct, binascii
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data = open(p,"rb").read()
IB=0x400000
def va2off(va): return va-IB
def hexdump(off,n,label=""):
    print("=== %s @ %08x ===" % (label, off))
    for i in range(0,n,16):
        c=data[off+i:off+i+16]
        print("%08x  %-48s |%s|" % (off+i, binascii.hexlify(c," ").decode(), "".join(chr(x) if 32<=x<127 else "." for x in c)))
# ProjectData
pd = 0x4bd64c
hexdump(va2off(pd), 0x120, "ProjectData")
