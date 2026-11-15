import json, os, re, collections
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
sc=json.load(open(os.path.join(BASE,"strings_classified.json"),encoding="utf-8"))
texts=[(s["h"],s["e"],s["t"]) for s in sc["strings"]]
# report / form names from VB6 object table
vbobj=json.load(open(os.path.join(BASE,"vb_objects.json"),encoding="utf-8"))
rep=[r["name"] for r in vbobj if re.match(r"(?i)^(report|rpt)", r["name"])]
res=[r["name"] for r in vbobj if re.match(r"(?i)^(result)", r["name"])]
print("Report* forms:", len(rep)); print(sorted(rep))
print()
print("Result* forms:", len(res)); print(sorted(res))
print()
# print/PDF stack
pdf=[t for h,e,t in texts if re.search(r"(?i)(pdf|bullzip|printer|print$|\.prn|spool)", t)]
uniq=sorted(set(pdf))
print("print/PDF related strings:", len(uniq))
for t in uniq[:120]: print("   ", repr(t[:120]))
