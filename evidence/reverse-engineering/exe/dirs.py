import pefile, json, os
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
pe=pefile.PE(p)
d={}
d["e_lfanew"]=hex(pe.DOS_HEADER.e_lfanew)
d["pe_header_file_offset"]=hex(pe.DOS_HEADER.e_lfanew)
d["coff_header_offset"]=hex(pe.DOS_HEADER.e_lfanew+4)
d["optional_header_offset"]=hex(pe.OPTIONAL_HEADER.get_file_offset())
d["section_table_offset"]=hex(pe.sections[0].get_file_offset())
d["export_dir"]=hex(pe.OPTIONAL_HEADER.DATA_DIRECTORY[0].VirtualAddress)+" size "+str(pe.OPTIONAL_HEADER.DATA_DIRECTORY[0].Size)
d["import_dir_rva"]=hex(pe.OPTIONAL_HEADER.DATA_DIRECTORY[1].VirtualAddress)+" size "+str(pe.OPTIONAL_HEADER.DATA_DIRECTORY[1].Size)
d["import_dir_file_offset"]=hex(pe.get_offset_from_rva(pe.OPTIONAL_HEADER.DATA_DIRECTORY[1].VirtualAddress))
d["resource_dir_rva"]=hex(pe.OPTIONAL_HEADER.DATA_DIRECTORY[2].VirtualAddress)+" size "+str(pe.OPTIONAL_HEADER.DATA_DIRECTORY[2].Size)
d["resource_dir_file_offset"]=hex(pe.get_offset_from_rva(pe.OPTIONAL_HEADER.DATA_DIRECTORY[2].VirtualAddress))
d["exception_dir"]=hex(pe.OPTIONAL_HEADER.DATA_DIRECTORY[3].VirtualAddress)
d["security_dir"]=hex(pe.OPTIONAL_HEADER.DATA_DIRECTORY[4].VirtualAddress)+" size "+str(pe.OPTIONAL_HEADER.DATA_DIRECTORY[4].Size)
d["basereloc_dir_rva"]=hex(pe.OPTIONAL_HEADER.DATA_DIRECTORY[5].VirtualAddress)+" size "+str(pe.OPTIONAL_HEADER.DATA_DIRECTORY[5].Size)
d["debug_dir_rva"]=hex(pe.OPTIONAL_HEADER.DATA_DIRECTORY[6].VirtualAddress)+" size "+str(pe.OPTIONAL_HEADER.DATA_DIRECTORY[6].Size)
d["iat_dir_rva"]=hex(pe.OPTIONAL_HEADER.DATA_DIRECTORY[12].VirtualAddress)+" size "+str(pe.OPTIONAL_HEADER.DATA_DIRECTORY[12].Size)
d["clr_dir_rva"]=hex(pe.OPTIONAL_HEADER.DATA_DIRECTORY[14].VirtualAddress)+" size "+str(pe.OPTIONAL_HEADER.DATA_DIRECTORY[14].Size)
d["entry_point_file_offset"]=hex(pe.get_offset_from_rva(pe.OPTIONAL_HEADER.AddressOfEntryPoint))
for s in pe.sections:
    print(s.Name.rstrip(b"\x00").decode(), "raw_off", hex(s.PointerToRawData), "raw_size", s.SizeOfRawData, "rva", hex(s.VirtualAddress), "vsize", s.Misc_VirtualSize, "chars", hex(s.Characteristics), "entropy", round(s.get_entropy(),3))
print(json.dumps(d, indent=1))
# rva->offset map
print("file offset == RVA for .text?", pe.sections[0].PointerToRawData==pe.sections[0].VirtualAddress)
json.dump(d, open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\pe_dirs.json","w",encoding="utf-8"), indent=1)
