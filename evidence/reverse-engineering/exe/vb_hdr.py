import struct, json, re
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data = open(p,"rb").read()
off = 0xb8e80
hdr = data[off:off+0x200]
def u16(o): return struct.unpack_from("<H", hdr, o)[0]
def u32(o): return struct.unpack_from("<I", hdr, o)[0]
def s(o,n): return hdr[o:o+n].split(b"\x00")[0].decode("latin-1")
IB = 0x400000
print("magic", hdr[0:4])
print("runtime build", u16(4))
print("lang dll", s(6,14))
print("sec lang dll", s(0x14,14))
print("runtime revision", u16(0x22))
print("lcid", hex(u32(0x24)), "sec lcid", hex(u32(0x28)))
print("lpSubMain", hex(u32(0x2C)))
print("lpProjectData", hex(u32(0x30)))
print("fMdlIntCtls", hex(u32(0x34)), "fMdlIntCtls2", hex(u32(0x38)))
print("threadFlags", hex(u32(0x3C)), "threadCount", u32(0x40))
print("wFormCount", u16(0x44), "wExternalCount", u16(0x46))
print("dwThunkCount", u32(0x48))
print("lpGuiTable", hex(u32(0x4C)))
print("lpExternalTable", hex(u32(0x50)))
print("lpComRegisterData", hex(u32(0x54)))
print("projDesc ptr", hex(u32(0x58)))
print("projExeName ptr", hex(u32(0x5C)))
print("projHelpFile ptr", hex(u32(0x60)))
print("projName ptr", hex(u32(0x64)))
print("dwHelpContext", u32(0x68), "dwHelpFile", hex(u32(0x6C)))
print("ProjectObject", hex(u32(0x70)))
print("dwReserved", hex(u32(0x74)), u32(0x78))
print("fCompileType", hex(u32(0x7C)))
print("dwObjectCount", u16(0x80))
print("dwCompiledCode", u16(0x82))
print("dwUsedCount", u16(0x84))
print("dwUnusedCount", u16(0x86))
print("dwNativeCode", hex(u32(0x88)))
print("szLangID", s(0x8C,4), "szSecLangID", s(0x90,4))
print("dwLCID2", hex(u32(0x94)), "dwSecLCID2", hex(u32(0x98)))
print("lpSubMain2", hex(u32(0x9C)))
print("dwProjectData2", hex(u32(0xA0)))
print("threadFlags2", hex(u32(0xAC)), "threadCount2", u32(0xB0))
print("wFormCount2", u16(0xB4), "wExternalCount2", u16(0xB6))
print("dwThunkCount2", u32(0xB8))
print("lpGuiTable2", hex(u32(0xBC)))
print("lpExternalTable2", hex(u32(0xC0)))
print("lpComRegData2", hex(u32(0xC4)))
print("projDesc2", hex(u32(0xC8)))
print("projExeName2", hex(u32(0xCC)))
print("projHelpFile2", hex(u32(0xD0)))
print("projName2", hex(u32(0xD4)))
print("HelpContext2", u32(0xD8), "HelpFile2", hex(u32(0xDC)))
print("ProjectObject2", hex(u32(0xE0)))
# strings at pointers
def cstr(va):
    o = va - IB
    if 0 <= o < len(data):
        e = data.find(b"\x00", o)
        return data[o:e].decode("latin-1", "replace")
    return None
for nm, va in [("projDesc", u32(0x58)),("projExeName", u32(0x5C)),("projHelpFile", u32(0x60)),("projName", u32(0x64)),
               ("projDesc2", u32(0xC8)),("projExeName2", u32(0xCC)),("projHelpFile2", u32(0xD0)),("projName2", u32(0xD4))]:
    print("STR", nm, hex(va), repr(cstr(va)) if va else None)
pd = u32(0x30)
print("ProjectData VA", hex(pd), "file off", hex(pd-IB))
open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\vb_header.txt","w",encoding="utf-8").write(json.dumps({"off":hex(off)},indent=1))
