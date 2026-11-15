import json, random, collections
raw = json.load(open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\strings_raw.json",encoding="utf-8"))
a=[d for d in raw if d["enc"]=="ascii"]
u=[d for d in raw if d["enc"]=="utf16le"]
random.seed(11)
for lo,hi in ((0x269000,0x400000),(0x400000,0x1800000),(0x1800000,0x4890000)):
    sel=[d for d in a if lo<=d["offset"]<hi]
    print("=== ASCII region", hex(lo), hex(hi), "count", len(sel))
    for d in random.sample(sel, min(14,len(sel))):
        print("   ", hex(d["offset"]), repr(d["text"][:90]))
su=[d for d in u if d["offset"]>=0x1780000]
print("utf16 beyond pool:", len(su))
for d in random.sample(su, min(10,len(su))):
    print("   ", hex(d["offset"]), repr(d["text"][:90]))
