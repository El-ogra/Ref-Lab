import json, os, capstone, struct
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
p=r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data=open(p,"rb").read()
mem=json.load(open(os.path.join(BASE,"vb_members.json"),encoding="utf-8"))
recs=mem["records"]
md=capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_32)
import random
random.seed(3)
sample=random.sample([r for r in recs if r["code_va"]], 4)
for r in sample:
    off=r["code_file_off"]
    off=int(off,16)
    code=data[off:off+48]
    print("=== %s @ code VA %s (file %s) ===" % (r["name"], r["code_va"], r["code_file_off"]))
    for i in md.disasm(code, r["code_va"]):
        print("   %08x  %-8s %s" % (i.address, i.mnemonic, i.op_str))
    print()
