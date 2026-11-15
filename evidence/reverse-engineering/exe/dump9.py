import struct, json
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data=open(p,"rb").read()
IB=0x400000
def cstr(va):
    o=va-IB
    if o<0 or o>len(data): return None
    e=data.find(b"\x00",o)
    return data[o:e].decode("latin-1","replace")
# find pointer to blob VA 0x66bb08 ('Image9' in blob)
for target,label in ((0x66bb08,"Image9@blob"),(0x66e034,"DataGrid@blob"),(0x66bb90,"Label34@blob")):
    ndl=struct.pack("<I",target); s=0; pos=[]
    while True:
        j=data.find(ndl,s)
        if j<0: break
        pos.append(j); s=j+1
    print(label, hex(target), "refs:", [hex(x) for x in pos][:10], len(pos))
print()
for off in (0xf8540,):
    print("=== dump", hex(off))
    for i in range(0,0xa0,4):
        w=struct.unpack_from("<I",data,off+i)[0]
        s=""
        if 0x400000<w<0x4900000:
            t=cstr(w)
            if t and len(t)<50 and all(32<=ord(ch)<127 for ch in t): s="  -> "+repr(t)
        print(" +%02x %08x%s" % (i,w,s))
