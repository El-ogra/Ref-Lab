import capstone, struct, binascii
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data = open(p,"rb").read()
IB=0x400000
ep_rva = 0x7142c
off = ep_rva  # since raw offset == rva for .text here
md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_32)
md.detail=False
code = data[off:off+64]
print("entry bytes:", binascii.hexlify(code[:32]," ").decode())
for i in md.disasm(code, IB+ep_rva):
    print("%08x  %-24s %s" % (i.address, i.mnemonic, i.op_str))
