import json, re, collections
raw = json.load(open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\strings_raw.json",encoding="utf-8"))
SQLKW = re.compile(r"(?i)\b(select\s+.+?\s+from|insert\s+into|update\s+\w+\s+set|delete\s+from|create\s+table|alter\s+table|drop\s+table|where\s|group\s+by|order\s+by|inner\s+join|left\s+join|right\s+join|union\s+select)\b")
hits=[d for d in raw if SQLKW.search(d["text"])]
print("sql-ish strings:", len(hits))
u = {}
for d in hits: u.setdefault(d["text"], d)
print("unique sql strings:", len(u))
# where do they live?
print("enc:", collections.Counter(d["enc"] for d in hits))
print("min off", hex(min(d["offset"] for d in hits)), "max off", hex(max(d["offset"] for d in hits)))
# generic keyword presence
for kw in ["SELECT","INSERT","UPDATE","DELETE","CREATE TABLE","ALTER TABLE","DROP","WHERE","VALUES","OPEN ","CONNECTION","PROVIDER","DSN","ODBC","JET","ADO","Recordset","DataSource","DataBase","SQL"]:
    c=sum(1 for d in hits if kw.lower() in d["text"].lower())
    print(kw, c)
