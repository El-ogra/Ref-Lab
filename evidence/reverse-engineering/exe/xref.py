import json, re, collections, os
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
sq=json.load(open(os.path.join(BASE,"sql_inventory.json"),encoding="utf-8"))
sc=json.load(open(os.path.join(BASE,"strings_classified.json"),encoding="utf-8"))
KNOWN = ["patients","patientinfo","patienttest","normalrange","testtable","constanttable","converts","testunits",
"abbreviations","subgrouptable","cultures","antibiotic","antibio","history","MyLabSetting","daytip","comments",
"barcodes","PTC","ratios","reportview","customtesttable","customgrouptable","testtemp","hba1c","insulin",
"testlearn","loggroup","pictures","pricelist","pricemenu","smslist","smsgroups","SMSSender","SMStemplate",
"titless","anyreport","culturesec","Helicobacter","ESR","TORCH","Paste Errors","Labbranch"]
known_lower={k.lower():k for k in KNOWN}
tables=sq["tables"]
matched={}; unmatched=[]
for t,c in tables.items():
    if t.lower() in known_lower:
        matched[known_lower[t.lower()]]=matched.get(known_lower[t.lower()],0)+c
    else:
        unmatched.append((t,c))
print("KNOWN-103 tables referenced by the EXE:", len(matched))
for k in sorted(matched): print("   ", k, matched[k])
print()
print("NOT in the supplied known-list (need DB-side verification):", len(unmatched))
for t,c in sorted(unmatched, key=lambda x:-x[1]): print("   ", t, c)
print()
# also check known tables appearing as plain strings anywhere (not only SQL)
alltext = {s["t"] for s in sc["strings"]}
extra={}
for k in KNOWN:
    hits=[s for s in sc["strings"] if s["t"].lower()==k.lower()]
    if hits: extra[k]={"occurrences":len(hits),"first_offset":hits[0]["h"],"example":hits[0]["t"]}
print("known tables appearing as standalone strings (any context):", len(extra))
for k,v in sorted(extra.items()): print("   %-18s %-4s %s" % (k, v["occurrences"], v["first_offset"]))
json.dump({"known_103_matched_via_sql":matched,"unknown_table_identifiers":sorted(unmatched),
           "known_standalone_string_hits":extra},
          open(os.path.join(BASE,"db_xref.json"),"w",encoding="utf-8"), indent=1, ensure_ascii=False)
