import json, re
raw = json.load(open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\strings_raw.json",encoding="utf-8"))
names = [d for d in raw if d["enc"]=="ascii" and 0x26aebc <= d["offset"] <= 0x26b980]
print("block1 count:", len(names))
print("first:", names[0], "last:", names[-1])
print("block1 names:", [d["text"] for d in names])
print()
ctl = [d for d in raw if d["enc"]=="ascii" and 0x26bb00 <= d["offset"] <= 0x26e100]
print("block2 count:", len(ctl))
