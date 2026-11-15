import json, os
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
sc=json.load(open(os.path.join(BASE,"strings_classified.json"),encoding="utf-8"))
vb=json.load(open(os.path.join(BASE,"vb_objects.json"),encoding="utf-8"))
texts={s["t"] for s in sc["strings"]}
missing=[r["name"] for r in vb if r["name"] not in texts]
print("object names missing from classified set:", len(missing), missing)
# any object names with odd chars
print([r["name"] for r in vb if not r["name"].isalnum()][:30])
