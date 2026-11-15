import pefile, json, os
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
outdir = r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\resources"
os.makedirs(outdir, exist_ok=True)
pe = pefile.PE(p)
data = open(p,"rb").read()
RT = {1:"RT_CURSOR",2:"RT_BITMAP",3:"RT_ICON",4:"RT_MENU",5:"RT_DIALOG",6:"RT_STRING",7:"RT_FONTDIR",8:"RT_FONT",9:"RT_ACCELERATOR",10:"RT_RCDATA",11:"RT_MESSAGETABLE",12:"RT_GROUP_CURSOR",14:"RT_GROUP_ICON",16:"RT_VERSION",17:"RT_DLGINCLUDE",19:"RT_PLUGPLAY",20:"RT_VXD",21:"RT_ANICURSOR",22:"RT_ANIICON",23:"RT_HTML",24:"RT_MANIFEST"}
res=[]
def walk(d, path, ):
    for e in d.entries:
        if e.id is not None:
            nm = RT.get(e.id, str(e.id)) if e.id in RT else str(e.id)
        else:
            nm = str(e.name)
        if e.struct.DataIsDirectory:
            walk(e.directory, path+[nm])
        else:
            de = e.data.struct
            blob = data[de.OffsetToData:de.OffsetToData+de.Size]
            res.append({"path":path+[nm],"type_id": e.id,"name":nm,"lang": None,"rva":hex(de.OffsetToData),
                        "size":de.Size,"file_offset":de.OffsetToData,"code_page":de.Reserved,
                        "sha256": __import__("hashlib").sha256(blob).hexdigest()})
    return
for t in pe.DIRECTORY_ENTRY_RESOURCE.entries:
    tn = RT.get(t.id, str(t.id)) if t.id is not None else str(t.name)
    for n in t.directory.entries:
        for l in n.directory.entries:
            de = l.data.struct
            blob = data[de.OffsetToData:de.OffsetToData+de.Size]
            res.append({"path":[tn, str(n.id) if n.id is not None else str(n.name), str(l.id)],
                        "type":tn,"type_id":t.id,"name_id": n.id if n.id is not None else str(n.name),
                        "name": str(n.name) if n.name else str(n.id),
                        "lang": l.id, "rva":hex(de.OffsetToData),"size":de.Size,"file_offset":de.OffsetToData,
                        "code_page":de.Reserved,
                        "sha256":__import__("hashlib").sha256(blob).hexdigest()})
res.sort(key=lambda r:(r["type"],str(r["name_id"]),r["lang"]))
# extract
for r in res:
    ext=".bin"
    if r["type"]=="RT_ICON": ext=".ico"
    elif r["type"]=="RT_GROUP_ICON": ext=".grp"
    elif r["type"]=="RT_MANIFEST": ext=".xml"
    elif r["type"]=="RT_VERSION": ext=".ver"
    elif r["type"]=="RT_DIALOG": ext=".dlg"
    elif r["type"]=="RT_MENU": ext=".menu"
    elif r["type"]=="RT_STRING": ext=".str"
    else: ext=".bin"
    safe = "_".join([str(x) for x in r["path"]]) + ext
    safe = "".join(ch if ch.isalnum() or ch in "._-" else "_" for ch in safe)
    fn = os.path.join(outdir, safe)
    open(fn,"wb").write(data[r["file_offset"]:r["file_offset"]+r["size"]])
    r["extracted"] = fn
json.dump(res, open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\resources.json","w",encoding="utf-8"), indent=2, ensure_ascii=False)
from collections import Counter
print("total entries:", len(res))
print(Counter(r["type"] for r in res))
for r in res:
    print(r["type"], r["name"], "lang=",r["lang"], "size=",r["size"], "off=",hex(r["file_offset"]))
