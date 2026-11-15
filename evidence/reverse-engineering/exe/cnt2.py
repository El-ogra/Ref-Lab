import json, struct
recs=json.load(open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\vb_objects.json",encoding="utf-8"))
tot=sum(r["f04_member_count"] for r in recs)
print("sum member counts:", tot)
raw=json.load(open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\strings_raw.json",encoding="utf-8"))
asc=[d for d in raw if d["enc"]=="ascii"]
blob=[d for d in asc if 0x26bb08 <= d["offset"] <= 0x342000]
print("ascii names in blob:", len(blob))
# count all ascii strings between 0x26bb08 and end of names region
print("first:", blob[0]["text"], "last:", blob[-1]["text"], hex(blob[-1]["offset"]))
# check reference pointer search
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data=open(p,"rb").read()
for nm in ("Image9","DataGrid","Label34"):
    off=data.find(nm.encode())
    va=off+0x400000
    ndl=struct.pack("<I",va)
    pos=[]; s=0
    while True:
        j=data.find(ndl,s)
        if j<0: break
        pos.append(hex(j)); s=j+1
    print(nm, hex(off), hex(va), "ptr refs:", pos[:6], "count", len(pos))
