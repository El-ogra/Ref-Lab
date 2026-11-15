import json, os, collections
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
p=r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data=open(p,"rb").read()
mem=json.load(open(os.path.join(BASE,"vb_members.json"),encoding="utf-8"))
recs=mem["records"]
IB=0x400000
def cstr(va):
    o=va-IB
    if o<0 or o>len(data): return None
    e=data.find(b"\x00",o)
    if e<0 or e-o>80: return None
    t=data[o:e].decode("latin-1")
    return t if t and all(32<=ord(c)<127 for c in t) else None
# check +0x20 field
c=collections.Counter()
for r in recs[:2000]:
    va=r["code_va"]; o=va-IB
    b=data[o] if 0<=o<len(data) else None
    c[b]+=1
print("byte at code_va (first 2000 records):", c.most_common(12))
# what string is at 0x66bb80 and 0x66cc60?
for va in (0x66bb80, 0x66cc60, 0x66bb90, 0x66dedc):
    print(hex(va), repr(cstr(va)))
# raw bytes around 0xf92a4
import binascii
for off in (0xf9244, 0xf92a4, 0xf9308, 0xf9360):
    print(hex(off), binascii.hexlify(data[off:off+32]," ").decode())
