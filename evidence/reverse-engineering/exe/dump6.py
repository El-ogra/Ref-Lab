import struct, binascii
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data = open(p,"rb").read()
for off in (0x26d760, 0x26b970, 0x26f4a0, 0x26e930, 0x340000, 0x340200, 0x342000):
    c = data[off:off+0x60]
    print("%08x  %s" % (off, binascii.hexlify(c," ").decode()))
    print("       |%s|" % "".join(chr(x) if 32<=x<127 else "." for x in c))
# check 4 bytes before each utf16 literal for length prefix
for off in (0x26d76c, 0x26d7dc, 0x26dad0, 0x26db04, 0x26db40, 0x26dd5c, 0x26ed90, 0x26f4b4):
    pre = data[off-8:off]
    print(hex(off), "pre:", binascii.hexlify(pre," ").decode(), "len_u32:", struct.unpack_from("<I",data,off-4)[0], "char_count:", (len(data[off:].split(b"\x00\x00")[0])+1)//2)
