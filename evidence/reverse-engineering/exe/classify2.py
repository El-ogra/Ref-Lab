import json, re, collections, os, hashlib
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
raw = json.load(open(os.path.join(BASE,"strings_raw.json"),encoding="utf-8"))
vbobj = json.load(open(os.path.join(BASE,"vb_objects.json"),encoding="utf-8"))
vbmem = json.load(open(os.path.join(BASE,"vb_members.json"),encoding="utf-8"))
form_names = {r["name"] for r in vbobj}
member_names = set()
for t in vbmem["tables"]: member_names.update(t["names"])
POOL_LO, POOL_HI = 0x269000, 0x1780000

SQLSTRONG = re.compile(r"(?i)\b(select\s+[\w\[\]\.\*\s,=']+\s+from\b|insert\s+into\b|update\s+[\w\[\]\.]+\s+set\b|delete\s+from\b|create\s+table\b|alter\s+table\b|drop\s+table\b)")
SQLWEAK   = re.compile(r"(?i)\b(select\b|insert\b|update\b|delete\b|where\b|group\s+by\b|order\s+by\b|inner\s+join\b|left\s+join\b|right\s+join\b|union\s+select\b)")
URL   = re.compile(r"(?i)(https?://|ftp://|file://|\bwww\.[a-z0-9.-]+\b)")
REG   = re.compile(r"(?i)(HKEY_[A-Z_]+|\\Software\\[A-Za-z0-9 _\\.-]+|CurrentVersion\\[A-Za-z0-9 _\\.-]+)")
PATH  = re.compile(r"(?i)([A-Z]:\\[\\/][^<>|\"\*?\r\n]{2,180}|\\\\[A-Za-z0-9_.\-]+\\[^<>|\"\*?\r\n]{2,180}|\b[A-Za-z0-9_]+\.(mdb|accdb|ini|dat|xml|txt|csv|log|bak|pdf|jpg|jpeg|bmp|gif|wav|mp3|xls|xlsx|doc|docx|dll|olb|oca|exe|ocx|tlb|rtf|chm)\b)")
CRED  = re.compile(r"(?i)(password|passwd|pwd\s*=|pwd=|secret|api[_\- ]?key|token\s*=|\bDPB=|\bCMG=|credential)")
PRINTER = re.compile(r"(?i)(reportprinter|reportprinterx|zarfprinter|barcodeprinter|waslprinter|idprinter|\bprinter\b)")
REPORT = re.compile(r"(?i)^(report|rpt)[a-z0-9_]*$")
ERRMSG = re.compile(r"(?i)\b(error|invalid|failed|failure|cannot|can't|not found|wrong|denied|required|must\b|try again|exception|already exists)\b")
DDL    = re.compile(r"(?i)\b(create\s+table|alter\s+table|drop\s+table|primary\s+key|varchar\(|smallint|integer|smalldatetime|datetime|bit\b|ntext|text\b)")
DBX    = re.compile(r"(?i)(provider\s*=|data\s+source\s*=|initial\s+catalog|jet\s+oledb|oledb|odbc|\bdsn\s*=|\.mdb|\.accdb|opendatabase|adodb|adodc|dao\b)")
OLE    = re.compile(r"(?i)\.(OLB|OCA|ocx|tlb)$")
IDENT  = re.compile(r"^[A-Za-z][A-Za-z0-9_]{1,48}$")
IDENTX = re.compile(r"^[A-Za-z][A-Za-z0-9_]{0,48}[ .:/\\@#\-]{1}[A-Za-z0-9_ .:/\\@#\-]{0,60}$")

def alpha_count(s): return sum(1 for c in s if c.isalpha())
def meaningful(s):
    if alpha_count(s) < 2: return False
    if " " in s:
        letters=[c for c in s if c.isalpha()]
        return len(letters)>=3 and sum(1 for c in letters if c.lower() in "aeiou")/len(letters) >= 0.15
    if IDENT.match(s): return True
    if IDENTX.match(s) and alpha_count(s)>=3: return True
    return False

def classify(s, inpool):
    cats=[]
    if CRED.search(s): cats.append("credential_like")
    if SQLSTRONG.search(s): cats.append("sql_statement")
    elif SQLWEAK.search(s): cats.append("sql_fragment")
    if DDL.search(s): cats.append("sql_ddl")
    if DBX.search(s): cats.append("db_connection_or_driver")
    if URL.search(s): cats.append("url")
    if REG.search(s): cats.append("registry_path")
    if PATH.search(s): cats.append("file_path")
    if PRINTER.search(s): cats.append("printer_role")
    if REPORT.match(s): cats.append("report_name")
    if ERRMSG.search(s) and " " in s: cats.append("error_or_message")
    if OLE.search(s): cats.append("ole_reference")
    if s in form_names: cats.append("vb6_form_or_module_name")
    elif s in member_names: cats.append("vb6_member_name")
    if " " in s and "sql_statement" not in cats and "sql_fragment" not in cats: cats.append("ui_caption_or_message")
    if not cats: cats.append("identifier_or_other")
    return cats

out=[]; skipped_noise=0; outside_kept=0
for d in raw:
    s=d["text"]; off=d["offset"]; inpool = POOL_LO <= off < POOL_HI
    if inpool:
        if not meaningful(s):
            skipped_noise+=1; continue
    else:
        if not (SQLSTRONG.search(s) or SQLWEAK.search(s) or URL.search(s) or REG.search(s) or PATH.search(s) or CRED.search(s)):
            continue
        outside_kept+=1
    out.append({"o":off,"h":hex(off),"e":d["enc"],"l":len(s),"t":s,"c":classify(s,inpool)})
print("kept:", len(out), "skipped_noise_in_pool:", skipped_noise, "kept_outside_pool:", outside_kept)
cnt=collections.Counter()
for r in out:
    for k in r["c"]: cnt[k]+=1
for k,v in cnt.most_common(): print("  ", k, v)
uniq={}
for r in out: uniq.setdefault(r["t"],[]).append(r["h"])
print("unique texts:", len(uniq))
json.dump({"meta":{"pool_region_file_offsets":[hex(POOL_LO),hex(POOL_HI)],
                   "total_extracted":len(raw),"retained":len(out),
                   "skipped_noise_in_pool":skipped_noise,"retained_outside_pool":outside_kept,
                   "unique_texts":len(uniq)},
           "strings":out,
           "unique_text_index":{k:v[:50] for k,v in uniq.items()}},
          open(os.path.join(BASE,"strings_classified.json"),"w",encoding="utf-8"), ensure_ascii=False)
print("size", os.path.getsize(os.path.join(BASE,"strings_classified.json")))
