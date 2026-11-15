import json, os, collections
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
inv=json.load(open(os.path.join(BASE,"exe_inventory.json"),encoding="utf-8"))
vb=json.load(open(os.path.join(BASE,"vb_objects.json"),encoding="utf-8"))
sq=json.load(open(os.path.join(BASE,"sql_inventory.json"),encoding="utf-8"))
xr=json.load(open(os.path.join(BASE,"db_xref.json"),encoding="utf-8"))
mem=json.load(open(os.path.join(BASE,"vb_members.json"),encoding="utf-8"))
inv["vb6"]["object_type_flag_distribution"]=dict(collections.Counter(r["f10"] for r in vb))
inv["vb6"]["member_count_stats"]={"min":min(r["f04_member_count"] for r in vb),
  "max":max(r["f04_member_count"] for r in vb),
  "sum":sum(r["f04_member_count"] for r in vb),
  "objects_with_zero_members":sum(1 for r in vb if r["f04_member_count"]==0)}
inv["vb6"]["report_forms"]=[r["name"] for r in vb if r["name"].lower().startswith(("report","rpt"))]
inv["vb6"]["result_forms"]=[r["name"] for r in vb if r["name"].lower().startswith("result")]
inv["vb6"]["module_objects"]=[r["name"] for r in vb if r["f10"]=="0x18001"]
inv["vb6"]["class_objects"]=[r["name"] for r in vb if r["f10"]=="0x118003"]
inv["database"]={
  "unique_sql_strings":len(sq["unique_sql_strings"]),
  "distinct_table_identifiers":len(sq["tables"]),
  "distinct_qualified_table_column_pairs":len(sq["qualified_columns"]),
  "distinct_column_tokens":len(sq["column_tokens"]),
  "known_103_tables_referenced":sorted(xr["known_103_matched_via_sql"].keys()),
  "known_103_tables_referenced_count":len(xr["known_103_matched_via_sql"]),
  "known_103_not_referenced":["Paste Errors"],
  "additional_table_identifiers_not_in_supplied_list":xr["unknown_table_identifiers"]}
inv["printing"]={"printer_roles":{"reportprinter":"0x2d6194","reportprinterx":"0x2821b4","zarfprinter":"0x285150",
   "barcodeprinter":"0x2b175c","waslprinter":"0x2ab334","IDprinter":"0x2a5ea4"},
  "pdf_stack":["Bullzip PDF Printer (0x2a24c8)","Bullzip PDF Printer 11.1 - Create File (0x2a25a0)",
   "winspool.drv (0x26e924)","EnumPrintersA (0x26eb1c)","SetDefaultPrinterA (0x26ebb0)"],
  "pdf_output_directories":["D:\\PDF (0x28237c)","D:\\PDF\\ (0x282390)","D:\\PDF\\Free\\ (0x298e88)",
   "D:\\PDF\\Contracts\\ (0x2a2384)","D:\\PDF\\Individual\\ (0x2a2510)","D:\\PDF\\LabToLab\\ (0x2a2570)"]}
inv["authentication"]={"hard_coded_credential_literals_found":0,
  "notes":"29 credential-related strings, all UI captions / control names / INI keys. No password, PIN, hash, DPB/CMG blob or Password= connection-string clause found. SQL Server strings use Integrated Security=SSPI or Persist Security Info=True without credentials.",
  "ini_password_keys":["IDPasswordL (0x2a5f50)","IDPasswordT (0x2a5f6c)","IDPasswordF (0x2a5f88)","IDPasswordB (0x2a5fa4)","IDPasswordS (0x2a5fc0)","IDPasswordV (0x2a5fdc)","IDPasswordA (0x2a5ff8)"]}
inv["tooling_notes"]={"rea_version":"6.7.0","ghidra_version":"12.1.4","jdk":"21.0.13.21-hotspot",
  "rea_mcp_inspect_pe_resources":"blocked: max_file_bytes 67108864 < target size 76292096",
  "rea_mcp_open_binary":"blocked: REA MCP server process has no JAVA_HOME; resolves java.exe as a relative command",
  "ghidra_analysis":"see tool_log.md"}
json.dump(inv, open(os.path.join(BASE,"exe_inventory.json"),"w",encoding="utf-8"), indent=1, ensure_ascii=False)
print("ok", os.path.getsize(os.path.join(BASE,"exe_inventory.json")))
print("known referenced:", len(inv["database"]["known_103_tables_referenced"]))
