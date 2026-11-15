import struct, binascii, json
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data=open(p,"rb").read()
recs=json.load(open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\vb_objects.json",encoding="utf-8"))
IB=0x400000
def cstr(va):
    o=va-IB
    if o<0 or o>len(data): return None
    e=data.find(b"\x00",o); return data[o:e].decode("latin-1","replace")
def u32(o): return struct.unpack_from("<I",data,o)[0]
# f08 -> per-object struct?
for r in recs[:6]:
    v=int(r["f08"],16); o=v-IB
    print(r["name"], "f08 va",hex(v),"off",hex(o), [hex(u32(o+k)) for k in range(0,0x20,4)])
    for k in range(0,0x20,4):
        w=u32(o+k)
        if 0x400000<w<0x4900000:
            t=cstr(w)
            if t and len(t)<40 and all(32<=ord(c)<127 for c in t): print("     +%02x -> %r" % (k,t))
print()
print("=== dump 0x263364 ===")
for i in range(0,0xa0,4):
    w=u32(0x263364+i)
    s=""
    if 0x400000<w<0x4900000:
        t=cstr(w)
        if t and len(t)<40 and all(32<=ord(c)<127 for c in t): s="  -> "+repr(t)
    print("+%03x %08x%s" % (i,w,s))
