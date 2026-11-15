import json, re, os, collections
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
sc=json.load(open(os.path.join(BASE,"strings_classified.json"),encoding="utf-8"))
S=sc["strings"]
# INI keys / config
ini=[s for s in S if s["t"].lower().endswith(".ini") or "ini" in s["t"].lower() and "\\\\" in s["t"]]
print("=== INI / config file paths ===")
for s in sc["unique_text_index"]:
    pass
seen=set()
for s in S:
    t=s["t"]
    if re.search(r"(?i)\.ini\b", t) and ("\\" in t or "/" in t):
        if t in seen: continue
        seen.add(t); print("  ", s["h"], repr(t))
print()
print("=== registry paths ===")
seen=set()
for s in S:
    if "registry_path" in s["c"] and s["t"] not in seen:
        seen.add(s["t"]); print("  ", s["h"], repr(s["t"][:150]))
print()
print("=== URLs (unique) ===")
seen=set(); n=0
for s in S:
    if "url" in s["c"] and s["t"] not in seen:
        seen.add(s["t"]); n+=1
        if n<=40: print("  ", s["h"], repr(s["t"][:130]))
print("  unique urls:", n)
print()
print("=== sample SQL statements ===")
sql=json.load(open(os.path.join(BASE,"sql_inventory.json"),encoding="utf-8"))
for s in sql["unique_sql_strings"][:8]:
    print("  ", s["offset"], repr(s["text"][:230]))
