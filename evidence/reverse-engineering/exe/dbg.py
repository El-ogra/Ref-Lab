import struct
p=r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data=open(p,"rb").read()
print(len(data))
for a in (0x460b98,0x460b9c):
    w=struct.unpack_from("<I",data,a)[0]
    print(hex(a), hex(w), 0x00401000 <= w < 0x048A1000)
