import json, os, pefile, hashlib, datetime
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
P=r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
pe=pefile.PE(P)
imp={}
for e in pe.DIRECTORY_ENTRY_IMPORT:
    dll=e.dll.decode()
    items=[]
    for i in e.imports:
        items.append({"name": i.name.decode() if i.name else None, "ordinal": i.ordinal,
                      "hint": i.hint, "thunk_rva": hex(i.address-pe.OPTIONAL_HEADER.ImageBase),
                      "thunk_file_offset": hex(i.address-pe.OPTIONAL_HEADER.ImageBase)})
    imp[dll]={"dll":dll,"descriptor_file_offset":hex(e.struct.get_file_offset()),
              "original_first_thunk_rva":hex(e.struct.OriginalFirstThunk),
              "first_thunk_rva":hex(e.struct.FirstThunk),
              "count":len(items),"functions":items}
out={"import_table_directory":{"rva":hex(pe.OPTIONAL_HEADER.DATA_DIRECTORY[1].VirtualAddress),
      "size":pe.OPTIONAL_HEADER.DATA_DIRECTORY[1].Size},
     "dll_count":len(imp),
     "total_imported_functions":sum(v["count"] for v in imp.values()),
     "delay_imports":[], "bound_imports":[], "dlls":imp}
for e in getattr(pe,"DIRECTORY_ENTRY_DELAY_IMPORT",[]):
    out["delay_imports"].append({"dll":e.dll.decode(),"count":len(e.imports),
        "functions":[ (i.name.decode() if i.name else None) for i in e.imports]})
json.dump(out, open(os.path.join(BASE,"imports.json"),"w",encoding="utf-8"), indent=1, ensure_ascii=False)
names=[i["name"] for i in imp["MSVBVM60.DLL"]["functions"]]
print("total imports:", out["total_imported_functions"])
print(sorted(n for n in names if n))
