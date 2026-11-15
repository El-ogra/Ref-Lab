import binascii, capstone, json
p=r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data=open(p,"rb").read()
md=capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_32)
for off,label in ((0xf8238,"0x004f8238"),(0xf8560,"0x004f8560"),(0x4713e8,"0x004713e8 thunk?")):
    print("===",label,"file",hex(off))
    print("   ",binascii.hexlify(data[off:off+48]," ").decode())
    for i in md.disasm(data[off:off+48], 0x400000+off):
        print("   %08x  %-8s %s" % (i.address,i.mnemonic,i.op_str))
    print()
