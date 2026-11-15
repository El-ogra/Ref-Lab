import json, re, collections, os
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
sc=json.load(open(os.path.join(BASE,"strings_classified.json"),encoding="utf-8"))
sql=[s for s in sc["strings"] if "sql_statement" in s["c"] or "sql_fragment" in s["c"] or "sql_ddl" in s["c"]]
# dedupe by text
seen={}
for s in sql: seen.setdefault(s["t"], s)
uniq=list(seen.values())
print("unique SQL-bearing strings:", len(uniq))
TABLE_RX = re.compile(r"(?i)\b(from|join|into|update|table)\s+([A-Za-z_][A-Za-z0-9_ ]*)")
tables=collections.Counter(); cols=collections.Counter(); qualified=collections.Counter()
for s in uniq:
    t=s["t"]
    for m in TABLE_RX.finditer(t):
        name=m.group(2).strip().rstrip(";,) ").split()[0]
        if name and not name.upper() in ("SET","VALUES","AS","ON","WHERE","SELECT","IF","EXISTS"):
            tables[name]+=1
    for m in re.finditer(r"\b([A-Za-z_][A-Za-z0-9_]*)\.([A-Za-z_][A-Za-z0-9_]*)\b", t):
        qualified[(m.group(1),m.group(2))]+=1
    for m in re.finditer(r"(?i)\b(?:select|set|where|order\s+by|group\s+by)\s+(.*?)(?:\bfrom\b|\bwhere\b|\bgroup\b|\border\b|$)", t):
        for tok in re.findall(r"\b[A-Za-z_][A-Za-z0-9_]*\b", m.group(1)):
            cols[tok]+=1
print("distinct table identifiers:", len(tables))
for k,v in tables.most_common(200): print("  T", k, v)
json.dump({"unique_sql_strings":[{"offset":s["h"],"enc":s["e"],"length":s["l"],"text":s["t"]} for s in uniq],
           "tables":dict(tables),
           "qualified_columns":[{"table":a,"column":b,"count":c} for (a,b),c in qualified.most_common()],
           "column_tokens":dict(cols)},
          open(os.path.join(BASE,"sql_inventory.json"),"w",encoding="utf-8"), ensure_ascii=False)
print("qualified table.column pairs:", len(qualified))
