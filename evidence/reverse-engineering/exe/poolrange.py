import json, collections
raw = json.load(open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\strings_raw.json",encoding="utf-8"))
u=collections.Counter()
for d in raw:
    if d["enc"]=="utf16le": u[(d["offset"]>>16)<<16]+=1
run=[]
for b in sorted(u):
    if u[b]>=120: run.append(b)
print("blocks with utf16 density>=120:", len(run))
if run:
    print("first", hex(run[0]), "last", hex(run[-1]))
    # contiguous?
    gaps=[(hex(run[i]),hex(run[i+1])) for i in range(len(run)-1) if run[i+1]-run[i]!=0x10000]
    print("non-contiguous transitions:", gaps[:20])
