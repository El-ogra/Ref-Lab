import json, os, collections
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
inv=json.load(open(os.path.join(BASE,"exe_inventory.json"),encoding="utf-8"))
vb=json.load(open(os.path.join(BASE,"vb_objects.json"),encoding="utf-8"))
ce=json.load(open(os.path.join(BASE,"vb_code_entries.json"),encoding="utf-8"))
inv["vb6"].update({
 "project_data_va":"0x004bd64c","project_data_file_offset":"0xbd64c",
 "object_table_va":"0x00660b98","object_table_file_offset":"0x260b98",
 "object_table_guid":"{AF67146C-22A0-4591-9ED3-52BEB9D44D32}",
 "object_table_wObjectCount":210,"object_table_wCompiledCount":210,"object_table_wObjectsInUse":212,
 "object_array_va":"0x00660bec","object_array_file_offset":"0x260bec","object_table_stride_bytes":48,
 "project_name":"Real_lab_system",
 "compiled_code_entry_table_va":"0x00460b98","compiled_code_entry_table_file_offset":"0x60b98",
 "compiled_code_entry_count":ce["count"],
 "compiled_code_entries_with_vb6_prologue":sum(1 for e in ce["entries"] if e["vb6_prologue"]),
})
inv["vb6"]["objects"]=[{"index":r["index"],"name":r["name"],"member_count":r["dwMethodCount"],
  "object_type":r["fObjectType"],"record_file_offset":r["base_file_offset"],
  "lpObjectInfo":r["lpObjectInfo"],"lpPublicBytes":r["lpPublicBytes"]} for r in vb]
inv["vb6"]["object_type_distribution"]=dict(collections.Counter(r["fObjectType"] for r in vb))
inv["vb6"]["member_count_stats"]={"min":min(r["dwMethodCount"] for r in vb),
  "max":max(r["dwMethodCount"] for r in vb),"sum":sum(r["dwMethodCount"] for r in vb),
  "objects_with_zero_members":sum(1 for r in vb if r["dwMethodCount"]==0)}
inv["vb6"]["report_forms"]=[r["name"] for r in vb if r["name"].lower().startswith(("report","rpt"))]
inv["vb6"]["result_forms"]=[r["name"] for r in vb if r["name"].lower().startswith("result")]
inv["vb6"]["module_objects"]=[r["name"] for r in vb if r["fObjectType"]=="0x18001"]
inv["vb6"]["class_objects"]=[r["name"] for r in vb if r["fObjectType"]=="0x118003"]
json.dump(inv, open(os.path.join(BASE,"exe_inventory.json"),"w",encoding="utf-8"), indent=1, ensure_ascii=False)
print("ok", os.path.getsize(os.path.join(BASE,"exe_inventory.json")))
print("code entries:", ce["count"], "prologue ok:", inv["vb6"]["compiled_code_entries_with_vb6_prologue"])
