import struct, json, os, collections
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
p=r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data=open(p,"rb").read()
IB=0x400000
def u32(o): return struct.unpack_from("<I",data,o)[0]
def cstr(va,maxlen=60):
    o=va-IB
    if o<0 or o>len(data): return None
    e=data.find(b"\x00",o)
    if e<0 or e-o>maxlen: return None
    t=data[o:e].decode("latin-1")
    return t if t and all(32<=ord(c)<127 for c in t) else None
def isvb6(va):
    o=va-IB
    if o<0 or o+3>len(data): return False
    b=data[o:o+3]
    return b[0]==0xC7 and b[1]==0x45 or b[:2]==b"\x55\x8b" or b[:2]==b"\x83\xec"
# code-pointer array: file offset = VA - IB
arr_file = 0x60b98
i=0; ptrs=[]
while i<3000:
    w=u32(arr_file+i*4)
    if 0x00401000 <= w < 0x048A1000: ptrs.append(w); i+=1
    else: break
print("code-pointer array file=0x60b98 (VA 0x460b98): entries", len(ptrs), "first", hex(ptrs[0]), "last", hex(ptrs[-1]))
print("  VB6-style prologue at target:", sum(1 for v in ptrs if isvb6(v)))
print("  deltas[0:12]:", [hex(ptrs[k+1]-ptrs[k]) for k in range(min(12,len(ptrs)-1))])
# POD array
POD=0x260bec; recs=[]
for i in range(210):
    b=POD+i*48
    nm=cstr(u32(b+0x18)); oi=u32(b+0x00); oi_ok=False
    if IB<=oi<IB+0x8A1000:
        o=oi-IB
        for k in range(0,0x60,4):
            if u32(o+k)==0x00660b98: oi_ok=True; break
    recs.append({"index":i,"base_file_offset":hex(b),"name":nm,
      "lpObjectInfo":hex(oi),"lpObjectInfo_valid":oi_ok,
      "lpPublicBytes":hex(u32(b+0x08)),"lpStaticBytes":hex(u32(b+0x0c)),
      "lpModulePublic":hex(u32(b+0x10)),"lpModuleStatic":hex(u32(b+0x14)),
      "dwMethodCount":u32(b+0x1c),"lpMethodNames":hex(u32(b+0x20)),
      "bStaticVars":hex(u32(b+0x24)),"fObjectType":hex(u32(b+0x28)),"dwNull":hex(u32(b+0x2c))})
print("POD records:", len(recs), "lpObjectInfo points at a valid ObjectInfo:", sum(1 for r in recs if r["lpObjectInfo_valid"]))
print("fObjectType:", dict(collections.Counter(r["fObjectType"] for r in recs)))
print("lpModulePublic non-zero:", sum(1 for r in recs if int(r["lpModulePublic"],16)))
for r in recs[:2]: print("  ", r)
json.dump(recs, open(os.path.join(BASE,"vb_objects.json"),"w",encoding="utf-8"), indent=1, ensure_ascii=False)
json.dump({"file_offset":hex(arr_file),"va":hex(arr_file+IB),"count":len(ptrs),
           "entries":[{"index":i,"code_va":hex(v),"code_rva":hex(v-IB),"code_file_offset":hex(v-IB),
                       "vb6_prologue":isvb6(v)} for i,v in enumerate(ptrs)]},
          open(os.path.join(BASE,"vb_code_entries.json"),"w",encoding="utf-8"), indent=1)
print("wrote vb_code_entries.json")
