import re
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data = open(p,"rb").read()
for pat in [rb"Document=", rb"Module=", rb"Class=", rb"ID=\{", rb"\[Host Extender Info\]", rb"\[Workspace\]", rb"CMG=", rb"DPB=", rb"Attribute VB_Name"]:
    ms = [m.start() for m in re.finditer(pat, data)]
    print(pat, "count", len(ms), "first", [hex(x) for x in ms[:8]])
