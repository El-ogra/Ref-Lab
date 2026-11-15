import json, collections
raw = json.load(open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\strings_raw.json",encoding="utf-8"))
buckets=collections.Counter(); ub=collections.Counter()
for d in raw:
    b=(d["offset"]>>16)<<16
    buckets[b]+=1
    if d["enc"]=="utf16le": ub[b]+=1
for b in sorted(buckets):
    if 0x240000 <= b <= 0x360000:
        print(hex(b), "all", buckets[b], "utf16", ub[b])
print()
# find last string offset overall and density map coarse
for b in sorted(buckets):
    if buckets[b]>500 and not (0x240000 <= b <= 0x360000):
        print("other dense", hex(b), buckets[b])
