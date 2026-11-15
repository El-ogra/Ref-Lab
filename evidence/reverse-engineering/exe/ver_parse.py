import pefile, json
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
pe = pefile.PE(p)
out={}
for f in pe.FileInfo:
    for st in f:
        if st.Key == b"StringFileInfo":
            for tb in st.StringTable:
                for k,v in tb.entries.items():
                    out[k.decode("utf-8","replace")]=v.decode("utf-8","replace")
        elif st.Key == b"VarFileInfo":
            for var in st.Var:
                out["var:"+var.entry.get("key","?")]=[str(x) for x in var.entry.get("value",[])]
json.dump(out, open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\version_info.json","w",encoding="utf-8"), indent=2, ensure_ascii=False)
for k,v in out.items(): print(k,"=",repr(v))
