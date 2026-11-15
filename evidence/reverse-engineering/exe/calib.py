import json, random, collections
raw = json.load(open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\strings_raw.json",encoding="utf-8"))
u=[d for d in raw if d["enc"]=="utf16le"]
random.seed(7)
for lo,hi in ((0x269000,0x3c0000),(0x400000,0x800000),(0x800000,0x1000000),(0x1000000,0x1800000),(0x1800000,0x3000000)):
    sel=[d for d in u if lo<=d["offset"]<hi]
    print("=== region", hex(lo), hex(hi), "count", len(sel))
    for d in random.sample(sel, min(12,len(sel))):
        print("   ", hex(d["offset"]), repr(d["text"][:90]))
