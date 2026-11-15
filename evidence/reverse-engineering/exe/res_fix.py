import pefile, json, os, hashlib, struct, collections
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
outdir = r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\resources"
os.makedirs(outdir, exist_ok=True)
for f in os.listdir(outdir):
    os.remove(os.path.join(outdir,f))
pe = pefile.PE(p)
data = open(p,"rb").read()
RT = {1:"RT_CURSOR",2:"RT_BITMAP",3:"RT_ICON",4:"RT_MENU",5:"RT_DIALOG",6:"RT_STRING",7:"RT_FONTDIR",8:"RT_FONT",9:"RT_ACCELERATOR",10:"RT_RCDATA",11:"RT_MESSAGETABLE",12:"RT_GROUP_CURSOR",14:"RT_GROUP_ICON",16:"RT_VERSION",17:"RT_DLGINCLUDE",19:"RT_PLUGPLAY",20:"RT_VXD",21:"RT_ANICURSOR",22:"RT_ANIICON",23:"RT_HTML",24:"RT_MANIFEST"}
res=[]
for t in pe.DIRECTORY_ENTRY_RESOURCE.entries:
    tn = RT.get(t.id, str(t.id)) if t.id is not None else str(t.name)
    for n in t.directory.entries:
        for l in n.directory.entries:
            de = l.data.struct
            fo = pe.get_offset_from_rva(de.OffsetToData)
            blob = data[fo:fo+de.Size]
            res.append({"type":tn,"type_id":t.id,
                        "name_id": n.id if n.id is not None else str(n.name),
                        "name": str(n.name) if n.name else str(n.id),
                        "lang": l.id, "lang_name": {1033:"en-US",0:"NEUTRAL"}.get(l.id,str(l.id)),
                        "rva":hex(de.OffsetToData), "file_offset":hex(fo), "size":de.Size,
                        "code_page":de.Reserved,
                        "sha256": hashlib.sha256(blob).hexdigest(),
                        "magic": blob[:8].hex()})
res.sort(key=lambda r:(r["type"], str(r["name_id"]), r["lang"]))
extmap={"RT_ICON":".ico","RT_GROUP_ICON":".grp","RT_VERSION":".ver","RT_MANIFEST":".xml","RT_DIALOG":".dlg","RT_STRING":".str","RT_MENU":".menu","RT_BITMAP":".bmp","RT_RCDATA":".bin"}
for r in res:
    fo=int(r["file_offset"],16); blob=data[fo:fo+r["size"]]
    ext=extmap.get(r["type"],".bin")
    safe="".join(ch if ch.isalnum() or ch in "._-" else "_" for ch in ("%s_%s_%s%s"%(r["type"],r["name_id"],r["lang"],ext)))
    fn=os.path.join(outdir,safe); open(fn,"wb").write(blob); r["extracted_file"]=fn
json.dump(res, open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\resources.json","w",encoding="utf-8"), indent=1, ensure_ascii=False)
for r in res: print("%-14s id=%-6s lang=%-5s size=%-7d file_off=%-12s sha=%s" % (r["type"],r["name_id"],r["lang_name"],r["size"],r["file_offset"],r["sha256"][:16]))
print()
grp=[r for r in res if r["type"]=="RT_GROUP_ICON"]
if grp:
    fo=int(grp[0]["file_offset"],16); d=data[fo:fo+grp[0]["size"]]
    reserved,typ,cnt=struct.unpack_from("<HHH",d,0)
    print("GROUP_ICON id=%s reserved=%d type=%d icons=%d" % (grp[0]["name_id"],reserved,typ,cnt))
    for i in range(cnt):
        w,h,cc,r2,planes,bpp,size,idv=struct.unpack_from("<BBBBHHIH",d,6+i*14)
        print("   %dx%d colors=%d planes=%d bpp=%d bytes=%d res_id=%d" % (w or 256,h or 256,cc,planes,bpp,size,idv))
