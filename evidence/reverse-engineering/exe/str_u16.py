import json, re, collections
raw = json.load(open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\strings_raw.json",encoding="utf-8"))
# Look at full set of distinct UTF-16 strings (VB6 unicode literals) in the pool region
reg=[d for d in raw if d["enc"]=="utf16le"]
print("utf16 total", len(reg))
c=collections.Counter(d["text"] for d in reg)
print("unique utf16", len(c))
# print a sample of non-sql utf16 strings
non_sql=[d for d in reg if not re.search(r"(?i)\b(select|insert|update|delete|create|alter)\b", d["text"])]
print("non-sql utf16", len(non_sql))
for d in non_sql[:60]:
    print(hex(d["offset"]), repr(d["text"][:120]))
