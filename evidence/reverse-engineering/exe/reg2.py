import json, re
raw = json.load(open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\strings_raw.json",encoding="utf-8"))
sel=[d for d in raw if 0x269000 <= d["offset"] <= 0x26e100]
print(len(sel))
for d in sel:
    print(hex(d["offset"]), d["enc"], repr(d["text"]))
