import struct, binascii
p=r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data=open(p,"rb").read()
IB=0x400000
def cstr(va,maxlen=60):
    o=va-IB
    if o<0 or o>len(data): return None
    e=data.find(b"\x00",o)
    if e<0 or e-o>maxlen: return None
    t=data[o:e].decode("latin-1")
    return t if t and all(32<=ord(c)<127 for c in t) else None
def dump(off,n,label):
    print("=== %s file=%08x ===" % (label,off))
    for i in range(0,n,4):
        w=struct.unpack_from("<I",data,off+i)[0]
        s=""
        if 0x400000<w<0x4900000:
            t=cstr(w)
            if t: s="  -> "+repr(t)
        print("  +%02x %08x%s" % (i,w,s))
    print()
dump(0x260b98,0x60,"ObjectTable")
dump(0x260bec,0x60,"POD[0] labmain")
dump(0x260c5c8,0x60,"labmain lpObjectInfo")
dump(0xbdb5c,0x40,"labxxyyzz lpObjectInfo")
