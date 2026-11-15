import struct, json
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data = open(p,"rb").read()
IB=0x400000
arr = 0x460b98 - IB
i=0; ptrs=[]
while i<3000:
    w = struct.unpack_from("<I", data, arr+i*4)[0]
    if 0x00401000 < w < 0x048A1000:
        ptrs.append(w); i+=1
    else: break
print("pointer-array entries:", len(ptrs), "first", hex(ptrs[0]), "last", hex(ptrs[-1]))
print("deltas:", [hex(ptrs[k+1]-ptrs[k]) for k in range(6)])
names = ["labmain","patientmain","Real_lab_system","Resulturine","otherfun","Frmcomment","login","userspw"]
for nm in names:
    off = data.find(nm.encode())
    va = off + IB
    needle = struct.pack("<I", va)
    positions=[]; s=0
    while True:
        j = data.find(needle, s)
        if j<0: break
        positions.append(j); s=j+1
    print(nm, "fileoff", hex(off), "VA", hex(va), "ptr occurrences:", [hex(x) for x in positions][:8])
