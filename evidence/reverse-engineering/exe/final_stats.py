import json, os, collections
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
sc=json.load(open(os.path.join(BASE,"strings_classified.json"),encoding="utf-8"))
S=sc["strings"]
def uniq(cat):
    s=set()
    for r in S:
        if cat in r["c"]: s.add(r["t"])
    return s
for cat in ["sql_statement","sql_fragment","sql_ddl","db_connection_or_driver","file_path","url","registry_path",
            "printer_role","report_name","error_or_message","credential_like","ole_reference",
            "vb6_form_or_module_name","vb6_member_name","ui_caption_or_message","identifier_or_other"]:
    u=uniq(cat)
    print("%-28s entries=%-7d unique=%d" % (cat, sum(1 for r in S if cat in r["c"]), len(u)))
print()
u=uniq("file_path")
print("unique paths sample:")
for t in sorted(u)[:0]: pass
print(" total unique paths:", len(u))
# ini unique
ini=sorted(t for t in u if t.lower().endswith(".ini"))
print(" unique .ini paths:", len(ini))
# sql statement count by verb
sql=sorted(uniq("sql_statement"))
print(" unique sql statements:", len(sql))
import re
verbs=collections.Counter()
for t in sql:
    m=re.match(r"(?i)\s*(select|insert\s+into|update|delete\s+from|create\s+table|alter\s+table|shape|shaped)", t)
    verbs[m.group(1).upper() if m else "OTHER"]+=1
print(" sql verbs:", dict(verbs))
# unique SQL tables
sq=json.load(open(os.path.join(BASE,"sql_inventory.json"),encoding="utf-8"))
print(" unique table identifiers:", len(sq["tables"]))
print(" unique qualified table.column pairs:", len(sq["qualified_columns"]))
print(" unique column tokens:", len(sq["column_tokens"]))
