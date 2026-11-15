import re, json, os, collections
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data = open(p,"rb").read()
N=len(data)
out=[]
# ASCII printable >=4
ascii_re = re.compile(rb"[\x20-\x7e]{4,}")
for m in ascii_re.finditer(data):
    out.append({"enc":"ascii","offset":m.start(),"length":m.end()-m.start(),"text":m.group().decode("latin-1")})
# UTF-16LE printable >=4
u16_re = re.compile(rb"(?:[\x20-\x7e]\x00){4,}")
for m in u16_re.finditer(data):
    out.append({"enc":"utf16le","offset":m.start(),"length":m.end()-m.start(),"text":m.group().decode("utf-16le")})
out.sort(key=lambda d:(d["offset"], d["enc"]))
json.dump(out, open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\strings_raw.json","w",encoding="utf-8"), ensure_ascii=False)
print("total", len(out), "ascii", sum(1 for d in out if d["enc"]=="ascii"), "utf16", sum(1 for d in out if d["enc"]=="utf16le"))
