import json, os
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
sc=json.load(open(os.path.join(BASE,"strings_classified.json"),encoding="utf-8"))
for s in sc["strings"]:
    t=s["t"]
    if t.upper().startswith("D:\\PDF") or "Bullzip" in t or "winspool" in t or t in ("EnumPrintersA","SetDefaultPrinterA"):
        print(s["h"], s["e"], repr(t))
