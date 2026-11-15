import re
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data = open(p,"rb").read()
print("size", len(data))
for m in re.finditer(rb"VB5!", data):
    print("VB5! at", hex(m.start()))
for m in re.finditer(rb"VB5!", data[:0x200000]):
    pass
