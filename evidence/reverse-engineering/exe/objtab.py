import struct, json
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data = open(p,"rb").read()
IB=0x400000
def cstr(va):
    o=va-IB
    if o<0 or o>len(data): return None
    e=data.find(b"\x00",o)
    return data[o:e].decode("latin-1","replace")
def u32(o): return struct.unpack_from("<I",data,o)[0]
recs=[]
base=0x260c04
i=0
while i < 500:
    o = base + i*48
    nmptr = u32(o)
    if not (0x00400000 < nmptr < 0x04900000): break
    nm = cstr(nmptr)
    if nm is None or len(nm)>80: break
    recs.append({"index":i,"record_file_offset":hex(o),"name":nm,"name_va":hex(nmptr),
        "f04_member_count":u32(o+4),"f08":hex(u32(o+8)),"f0c":hex(u32(o+0xc)),
        "f10":hex(u32(o+0x10)),"f14":hex(u32(o+0x14)),"f18_module_public":hex(u32(o+0x18)),
        "f1c":hex(u32(o+0x1c)),"f20_public_bytes":hex(u32(o+0x20)),"f24":hex(u32(o+0x24)),
        "f28":hex(u32(o+0x28)),"f2c":hex(u32(o+0x2c))})
    i+=1
print("parsed objects:", len(recs))
for r in recs[:8]: print(r)
print("...")
for r in recs[-8:]: print(r)
json.dump(recs, open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\vb_objects.json","w",encoding="utf-8"), indent=1, ensure_ascii=False)
