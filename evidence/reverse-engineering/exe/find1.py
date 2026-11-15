import re
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data = open(p,"rb").read()
for m in re.finditer(rb"Resulturine\w*", data):
    print(hex(m.start()), m.group())
for m in re.finditer(rb"ABCDEFGHIJ", data):
    print("ABCDEFGHIJ", hex(m.start()))
