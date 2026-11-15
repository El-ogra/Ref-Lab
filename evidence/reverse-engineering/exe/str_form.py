import json, re, collections
raw = json.load(open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\strings_raw.json",encoding="utf-8"))
asc = [d for d in raw if d["enc"]=="ascii"]
pat = re.compile(r"(?i)^(frm|form|main|mdl|mod|cls)[_a-z0-9]*$")
hits = sorted({(d["offset"], d["text"]) for d in asc if pat.match(d["text"])})
print("form-like ASCII:", len(hits))
for o,t in hits[:200]: print(hex(o), t)
