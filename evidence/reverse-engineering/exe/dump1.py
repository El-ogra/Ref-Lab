p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data = open(p,"rb").read()
off = 0xb8e80
import binascii
for i in range(0, 0x140, 16):
    chunk = data[off+i:off+i+16]
    print("%08x  %-48s |%s|" % (off+i, binascii.hexlify(chunk," ").decode(),
        "".join(chr(c) if 32<=c<127 else "." for c in chunk)))
