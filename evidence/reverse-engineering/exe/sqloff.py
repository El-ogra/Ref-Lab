import json, os, re
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
sq=json.load(open(os.path.join(BASE,"sql_inventory.json"),encoding="utf-8"))
L=sq["unique_sql_strings"]
def find(pat, n=3):
    rx=re.compile(pat, re.I)
    out=[]
    for s in L:
        if rx.search(s["text"]):
            out.append(s); 
            if len(out)>=n: break
    return out
for pat,label in ((r"ALTER\s+Table\s+IgEallergy","ALTER IgEallergy"),(r"Create\s+Table\s+IgEallergy","CREATE IgEallergy"),
                  (r"SELECT\s+Top\s+10\s+Electrolytes","Top10 Electrolytes"),(r"SELECT\s+patientinfo\.requestdate","patientinfo.requestdate"),
                  (r"FROM\s+patients\b","patients"),(r"DELETE\s+FROM","DELETE")):
    print("---",label)
    for s in find(pat):
        print("   ", s["offset"], repr(s["text"][:190]))
# counts
top=sum(1 for s in L if re.match(r"(?i)\s*select\s+top\s+\d+", s["text"]))
print("SELECT Top N statements:", top)
print("total unique SQL strings:", len(L))
