import json, re, os, collections
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
sc=json.load(open(os.path.join(BASE,"strings_classified.json"),encoding="utf-8"))
def show(cat, limit=200):
    print("=== %s ===" % cat)
    n=0
    for s in sc["strings"]:
        if cat in s["c"]:
            n+=1
            if n<=limit: print("  ", s["h"], s["e"], repr(s["t"][:160]))
    print("  total:", n)
show("credential_like")
show("printer_role")
show("db_connection_or_driver")
