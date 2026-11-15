import struct, binascii
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data = open(p,"rb").read()
IB=0x400000
def cstr(va):
    o=va-IB
    if o<0 or o>len(data): return None
    e=data.find(b"\x00",o)
    return data[o:e].decode("latin-1","replace")
for va,label,n in ((0x66334c,"labmain.f08",0x60),(0x66ed80,"labmain.f20",0x60)):
    o=va-IB
    print("=== %s va=%08x off=%08x ===" % (label,va,o))
    for i in range(0,n,4):
        w=struct.unpack_from("<I",data,o+i)[0]
        s=""
        if 0x400000<w<0x4900000:
            t=cstr(w)
            if t and len(t)<50 and all(32<=ord(ch)<127 for ch in t): s="  -> "+repr(t)
        print(" +%02x %08x%s" % (i,w,s))
