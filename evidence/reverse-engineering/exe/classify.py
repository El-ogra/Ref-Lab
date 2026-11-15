import json, re, collections, os
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
raw = json.load(open(os.path.join(BASE,"strings_raw.json"),encoding="utf-8"))
vbobj = json.load(open(os.path.join(BASE,"vb_objects.json"),encoding="utf-8"))
vbmem = json.load(open(os.path.join(BASE,"vb_members.json"),encoding="utf-8"))
form_names = {r["name"] for r in vbobj}
member_names = set()
for t in vbmem["tables"]: member_names.update(t["names"])

SQLKW = re.compile(r"(?i)\b(select\s|insert\s+into|update\s+\w+\s+set|delete\s+from|create\s+table|alter\s+table|drop\s+table|where\b|group\s+by|order\s+by|inner\s+join|left\s+join|right\s+join|full\s+join|union\b)\b")
SQLSTRONG = re.compile(r"(?i)\b(select\s+[\w\[\]\.\*]+\s+(from|where)\b|insert\s+into\s|update\s+[\w\[\]]+\s+set\b|delete\s+from\s|create\s+table\s|alter\s+table\s)")
URL = re.compile(r"(?i)\b(https?|ftp|file)://|\bwww\.[a-z0-9.-]+\b")
REG = re.compile(r"(?i)(HKEY_[A-Z_]+|Software\\[A-Za-z0-9 _\\.-]+|CurrentVersion\\[A-Za-z0-9 _\\.-]+)")
WINPATH = re.compile(r"(?i)([A-Z]:\\[\\/][^<>|\"\*?]{2,200}|[A-Za-z0-9_\\\\\.\-]+\.(mdb|accdb|ini|dat|xml|txt|csv|dll|olb|oca|exe|ocx|log|bak|pdf|jpg|bmp|gif|wav|mp3|xls|doc))")
INIKEY = re.compile(r"(?i)^[A-Za-z0-9_ ]{2,40}$")
PRINTER = re.compile(r"(?i)(reportprinter|reportprinterx|zarfprinter|barcodeprinter|waslprinter|idprinter|printer)")
REPORTRX = re.compile(r"(?i)\b(report|rpt|print)\w*\b")
ERRMSG = re.compile(r"(?i)\b(error|invalid|failed|failure|cannot|can't|not found|wrong|denied|required|must |try again|exception)\b")
CRED = re.compile(r"(?i)(password|passwd|pwd\s*=|secret|apikey|api_key|token\s*=|userid\s*=.*password|DPB=|CMG=)")
DBXREF = re.compile(r"(?i)(provider\s*=|data source\s*=|initial catalog|jet oledb|oledb|odbc|dsn\s*=|\.mdb|\.accdb|DataSource\s*=|OpenDatabase|CREATE TABLE)")

def has_alpha(s): return sum(1 for c in s if c.isalpha()) >= 2

cands = [d for d in raw if len(d["text"])>=4 and has_alpha(d["text"])]
# drop obvious binary garbage: long runs of non-printable-adjacent random
def garbage(s):
    if " " in s: return False
    # heuristic: real identifiers have consistent case patterns
    alpha=[c for c in s if c.isalpha()]; alpha="".join(alpha)
    if not alpha: return True
    vowels=sum(1 for c in alpha.lower() if c in "aeiou")
    if vowels/len(alpha) < 0.12 and len(alpha)>6: return True
    if re.search(r"[^A-Za-z0-9_ .:;,\-/\\\(\)\[\]@#$%&*+=<>?!'\"{}|~^`]", s): return True
    return False
cands=[d for d in cands if not garbage(d["text"])]
print("candidate meaningful strings:", len(cands))
print("enc:", collections.Counter(d["enc"] for d in cands))

classified=[]
for d in cands:
    s=d["text"]; cats=[]
    if CRED.search(s): cats.append("credential_like")
    if DBXREF.search(s): cats.append("db_connection_or_ddl")
    if SQLSTRONG.search(s): cats.append("sql_statement")
    elif SQLKW.search(s): cats.append("sql_fragment")
    if URL.search(s): cats.append("url")
    if REG.search(s): cats.append("registry_path")
    if WINPATH.search(s): cats.append("file_path_or_extension")
    if PRINTER.search(s): cats.append("printer_role")
    if REPORTRX.search(s) and not cats: cats.append("report_name_candidate")
    if ERRMSG.search(s) and " " in s: cats.append("error_or_message")
    if s in form_names: cats.append("vb6_form_or_module_name")
    elif s in member_names: cats.append("vb6_member_name")
    if re.search(r"\.(OLB|OCA|ocx|tlb)$", s, re.I): cats.append("ole_reference")
    if re.match(r"^(kernel32|user32|gdi32|advapi32|shell32|ole32|comctl32|winmm|msvbvm60)\.?$", s, re.I): cats.append("win32_dll")
    if " " in s and not cats: cats.append("ui_caption_or_message")
    if not cats: cats.append("identifier_or_other")
    classified.append({"offset":d["offset"],"offset_hex":hex(d["offset"]),"enc":d["enc"],
                        "length":d["length"],"text":s,"categories":cats})
print("classified total:", len(classified))
cnt=collections.Counter()
for c in classified:
    for k in c["categories"]: cnt[k]+=1
for k,v in cnt.most_common(): print(" ", k, v)
json.dump(classified, open(os.path.join(BASE,"strings_classified.json"),"w",encoding="utf-8"), ensure_ascii=False, indent=0)

