import struct, json
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data=open(p,"rb").read()
IB=0x400000
def cstr(va, maxlen=80):
    o=va-IB
    if o<0 or o>len(data)-2: return None
    e=data.find(b"\x00",o)
    if e<0 or e-o>maxlen: return None
    t=data[o:e].decode("latin-1")
    if not t or not all(32<=ord(c)<127 for c in t): return None
    return t
STR=40
recs=[]
N=len(data)
for o in range(0, N-STR, 4):
    nmptr=struct.unpack_from("<I",data,o)[0]
    if not (0x00400000 <= nmptr < 0x048A1000): continue
    flags=struct.unpack_from("<I",data,o+4)[0]
    if flags & 0xFFF00000: continue
    if not (flags & 0x00030000): continue
    code=struct.unpack_from("<I",data,o+0x20)[0]
    if not (0x00401000 <= code < 0x048A1000): continue
    nm=cstr(nmptr)
    if not nm: continue
    recs.append({"off":o,"va":o+IB,"name":nm,"name_va":nmptr,"flags":hex(flags),
                 "code_va":code,"code_rva":hex(code-IB),"code_file_off":hex(code-IB),
                 "typeflags":hex(struct.unpack_from("<I",data,o+8)[0]),
                 "offs":hex(struct.unpack_from("<I",data,o+0xC)[0]),
                 "lp":hex(struct.unpack_from("<I",data,o+0x10)[0]),
                 "flags2":hex(struct.unpack_from("<I",data,o+0x14)[0]),
                 "extra":hex(struct.unpack_from("<I",data,o+0x24)[0])})
print("member records:", len(recs))
# group into contiguous tables
recs.sort(key=lambda r:r["off"])
tables=[]; cur=[recs[0]]
for r in recs[1:]:
    if r["off"] - cur[-1]["off"] == STR:
        cur.append(r)
    else:
        if len(cur)>=2: tables.append(cur)
        cur=[r]
if len(cur)>=2: tables.append(cur)
print("tables (>=2 records):", len(tables))
tot=sum(len(t) for t in tables)
print("records in tables:", tot)
for t in tables[:12]:
    print(" table @%08x n=%d : %s" % (t[0]["off"], len(t), [r["name"] for r in t[:8]]))
json.dump({"records":recs,"tables":[{"start":hex(t[0]["off"]),"count":len(t),"names":[r["name"] for r in t]} for t in tables]},
          open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\vb_members.json","w",encoding="utf-8"), ensure_ascii=False)
