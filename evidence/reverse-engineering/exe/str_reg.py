import json, re, collections
raw = json.load(open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\strings_raw.json",encoding="utf-8"))
reg = [d for d in raw if 0x26e000 <= d["offset"] <= 0x340000]
print("strings in region:", len(reg))
print(collections.Counter(d["enc"] for d in reg))
for d in reg[:40]:
    print(hex(d["offset"]), d["enc"], len(d["text"]), repr(d["text"][:110]))
