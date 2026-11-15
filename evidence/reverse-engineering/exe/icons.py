import json, os, collections
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
res=json.load(open(os.path.join(BASE,"resources.json"),encoding="utf-8"))
for r in res:
    print("%-14s id=%-6s lang=%-5s size=%-7d off=%-10s rva=%-10s sha256=%s" % (r["type"], r["name"], r["lang"], r["size"], hex(r["file_offset"]), r["rva"], r["sha256"][:16]))
print()
# icon group details
import struct
grp=[r for r in res if r["type"]=="RT_GROUP_ICON"][0]
data=open(r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe","rb").read()[grp["file_offset"]:grp["file_offset"]+grp["size"]]
reserved,typ,cnt=struct.unpack_from("<HHH",data,0)
print("GROUP_ICON reserved=%d type=%d count=%d" % (reserved,typ,cnt))
for i in range(cnt):
    w,h,cc,r,planes,bpp,size,idv=struct.unpack_from("<BBBBHHIH",data,6+i*14)
    print("  icon: %dx%d colors=%d planes=%d bpp=%d bytes=%d id=%d" % (w or 256,h or 256,cc,planes,bpp,size,idv))
