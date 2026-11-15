import json, os, hashlib, datetime, collections
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
P=r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
pe=json.load(open(os.path.join(BASE,"pe_identity.json"),encoding="utf-8"))
res=json.load(open(os.path.join(BASE,"resources.json"),encoding="utf-8"))
imp=json.load(open(os.path.join(BASE,"imports.json"),encoding="utf-8"))
vbobj=json.load(open(os.path.join(BASE,"vb_objects.json"),encoding="utf-8"))
vbmem=json.load(open(os.path.join(BASE,"vb_members.json"),encoding="utf-8"))
sc=json.load(open(os.path.join(BASE,"strings_classified.json"),encoding="utf-8"))
ver=json.load(open(os.path.join(BASE,"version_info.json"),encoding="utf-8"))
h=hashlib.sha256()
with open(P,"rb") as f:
    for chunk in iter(lambda: f.read(1<<20), b""): h.update(chunk)
inv={
 "target":{"path":P,"size_bytes":os.path.getsize(P),
   "sha256":h.hexdigest(),
   "sha256_expected":"C54CE6C8C98A679EFC5AB485BC4B313B452026D6A40F9BDA36ECA7EA18DF93C0",
   "sha256_match": h.hexdigest().upper()=="C54CE6C8C98A679EFC5AB485BC4B313B452026D6A40F9BDA36ECA7EA18DF93C0",
   "hash_algorithm":"SHA-256"},
 "pe":{"machine":pe["machine"],"machine_name":"IMAGE_FILE_MACHINE_I386 (x86)",
   "format":"PE32 (32-bit)","native_or_managed":"NATIVE (no CLI/COM descriptor)",
   "number_of_sections":pe["number_of_sections"],"characteristics":pe["characteristics"],
   "timestamp_unix":pe["timestamp"],"timestamp_utc":pe["timestamp_utc"],
   "magic":pe["magic"],"image_base":pe["image_base"],
   "entry_point_rva":pe["entry_point_rva"],"entry_point_va":pe["entry_point_va"],
   "section_alignment":pe["section_alignment"],"file_alignment":pe["file_alignment"],
   "size_of_image":pe["size_of_image"],"size_of_headers":pe["size_of_headers"],
   "subsystem":pe["subsystem"],"subsystem_name":"IMAGE_SUBSYSTEM_WINDOWS_GUI",
   "dll_characteristics":pe["dll_characteristics"],
   "linker_version":"%d.%d"%(pe["linker_major"],pe["linker_minor"]),
   "os_version":pe["os_version"],"subsystem_version":pe["subsystem_version"],
   "checksum":pe["checksum"],"relocation_entry_count":pe["reloc_count"],
   "is_dotnet":pe["is_dotnet"],
   "sections":pe["sections"],"data_directories":pe["data_directories"]},
 "resources":{"entry_count":len(res),
   "by_type":dict(collections.Counter(r["type"] for r in res)),
   "entries":res},
 "version_info":ver,
 "imports":{"dll_count":imp["dll_count"],"total_functions":imp["total_imported_functions"],
   "dlls":{k:{"count":v["count"],"functions":[f["name"] for f in v["functions"]]} for k,v in imp["dlls"].items()},
   "delay_imports":imp["delay_imports"]},
 "vb6":{
   "runtime":"MSVBVM60.DLL (Visual Basic 6 native-compiled)",
   "header_signature_file_offset":"0xb8e80","header_signature_va":"0x004b8e80",
   "header_magic":"VB5!","runtime_build":9782,"runtime_revision":10,"lcid":"0x409 (en-US)",
   "entry_point_pushes_header_va":"0x004b8e80 (push 0x4b8e80; call ThunRTMain) @ file offset 0x7142c",
   "project_data_va":"0x004bd64c","form_count_header":198,"external_count_header":25,
   "thunk_count_header":233,
   "object_table_file_offset":"0x260c04","object_table_stride_bytes":48,
   "object_count_parsed":len(vbobj),
   "objects":[{"index":r["index"],"name":r["name"],"member_count":r["f04_member_count"],
               "record_file_offset":r["record_file_offset"],
               "native_code_va":r["f18_module_public"]} for r in vbobj],
   "member_descriptor_tables":len(vbmem["tables"]),
   "member_descriptor_records":len(vbmem["records"]),
   "member_descriptor_stride_bytes":40,
   "literal_pool_region_file_offsets":["0x269000","0x1780000"],
   "object_name_blob_file_offsets":["0x26aebc","0x26b980"]},
 "strings":sc["meta"],
}
json.dump(inv, open(os.path.join(BASE,"exe_inventory.json"),"w",encoding="utf-8"), indent=1, ensure_ascii=False)
print("written", os.path.getsize(os.path.join(BASE,"exe_inventory.json")))
print("sha match:", inv["target"]["sha256_match"])
print("sections:")
for s in pe["sections"]: print("  ", s)
print("resources by type:", inv["resources"]["by_type"])
