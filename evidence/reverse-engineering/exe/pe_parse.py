import pefile, json, sys, hashlib, datetime, struct
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
pe = pefile.PE(p, fast_load=False)
d = {}
d["machine"] = hex(pe.FILE_HEADER.Machine)
d["number_of_sections"] = pe.FILE_HEADER.NumberOfSections
d["timestamp"] = pe.FILE_HEADER.TimeDateStamp
d["timestamp_utc"] = datetime.datetime.utcfromtimestamp(pe.FILE_HEADER.TimeDateStamp).isoformat()+"Z"
d["characteristics"] = hex(pe.FILE_HEADER.Characteristics)
d["magic"] = hex(pe.OPTIONAL_HEADER.Magic)
d["image_base"] = hex(pe.OPTIONAL_HEADER.ImageBase)
d["entry_point_rva"] = hex(pe.OPTIONAL_HEADER.AddressOfEntryPoint)
d["entry_point_va"] = hex(pe.OPTIONAL_HEADER.ImageBase + pe.OPTIONAL_HEADER.AddressOfEntryPoint)
d["section_alignment"] = hex(pe.OPTIONAL_HEADER.SectionAlignment)
d["file_alignment"] = hex(pe.OPTIONAL_HEADER.FileAlignment)
d["size_of_image"] = hex(pe.OPTIONAL_HEADER.SizeOfImage)
d["size_of_headers"] = hex(pe.OPTIONAL_HEADER.SizeOfHeaders)
d["subsystem"] = pe.OPTIONAL_HEADER.Subsystem
d["dll_characteristics"] = hex(pe.OPTIONAL_HEADER.DllCharacteristics)
d["linker_major"] = pe.OPTIONAL_HEADER.MajorLinkerVersion
d["linker_minor"] = pe.OPTIONAL_HEADER.MinorLinkerVersion
d["os_version"] = "%d.%d" % (pe.OPTIONAL_HEADER.MajorOperatingSystemVersion, pe.OPTIONAL_HEADER.MinorOperatingSystemVersion)
d["subsystem_version"] = "%d.%d" % (pe.OPTIONAL_HEADER.MajorSubsystemVersion, pe.OPTIONAL_HEADER.MinorSubsystemVersion)
d["checksum"] = hex(pe.OPTIONAL_HEADER.CheckSum)
d["is_dotnet"] = hasattr(pe, "DIRECTORY_ENTRY_COM_DESCRIPTOR") and pe.OPTIONAL_HEADER.DATA_DIRECTORY[14].Size>0
d["data_directories"] = []
for i, dd in enumerate(pe.OPTIONAL_HEADER.DATA_DIRECTORY):
    nm = dd.name
    d["data_directories"].append({"index":i,"name":nm,"rva":hex(dd.VirtualAddress),"size":dd.Size})
secs = []
for s in pe.sections:
    secs.append({"name":s.Name.rstrip(b"\x00").decode("utf-8","replace"),"virtual_address":hex(s.VirtualAddress),
      "virtual_size":s.Misc_VirtualSize,"raw_size":s.SizeOfRawData,"raw_offset":s.PointerToRawData,
      "characteristics":hex(s.Characteristics),"entropy":round(s.get_entropy(),3),
      "file_end": s.PointerToRawData+s.SizeOfRawData})
d["sections"]=secs
d["imports"]={}
for e in pe.DIRECTORY_ENTRY_IMPORT:
    dll=e.dll.decode("utf-8","replace")
    d["imports"][dll]=[ {"name":(i.name.decode("utf-8","replace") if i.name else None),"ordinal":i.ordinal,"hint":i.hint,"thunk_rva":hex(i.address-pe.OPTIONAL_HEADER.ImageBase)} for i in e.imports]
d["delay_imports"]=[]
if hasattr(pe,"DIRECTORY_ENTRY_DELAY_IMPORT"):
    for e in pe.DIRECTORY_ENTRY_DELAY_IMPORT:
        d["delay_imports"].append({"dll":e.dll.decode("utf-8","replace"),"count":len(e.imports)})
d["exports"]=[]
if hasattr(pe,"DIRECTORY_ENTRY_EXPORT"):
    d["exports"]=[{"name":(s.name.decode() if s.name else None),"ordinal":s.ordinal,"rva":hex(s.address)} for s in pe.DIRECTORY_ENTRY_EXPORT.symbols]
d["has_resources"] = hasattr(pe,"DIRECTORY_ENTRY_RESOURCE")
d["reloc_count"] = 0
if hasattr(pe,"DIRECTORY_ENTRY_BASERELOC"):
    d["reloc_count"]=len(pe.DIRECTORY_ENTRY_BASERELOC)
open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\pe_identity.json","w",encoding="utf-8").write(json.dumps(d,indent=2,ensure_ascii=False))
print("sections:", [(s["name"],s["raw_offset"],s["raw_size"],s["virtual_address"],s["virtual_size"]) for s in secs])
print("imports dlls:", {k:len(v) for k,v in d["imports"].items()})
print("resources:", d["has_resources"], "dotnet:", d["is_dotnet"])
