import json, os, collections
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
vb=json.load(open(os.path.join(BASE,"vb_objects.json"),encoding="utf-8"))
c=collections.Counter(r["f10"] for r in vb)
print("f10 distribution:", dict(c))
for f in c:
    names=[r["name"] for r in vb if r["f10"]==f]
    print(f, len(names), names[:14])
mc=[r["f04_member_count"] for r in vb]
print("member counts: min",min(mc),"max",max(mc),"sum",sum(mc),"zero-member objects:",sum(1 for x in mc if x==0))
print("objects with 0 members:", [r["name"] for r in vb if r["f04_member_count"]==0][:40])
