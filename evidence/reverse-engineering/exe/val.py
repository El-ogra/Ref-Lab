import json, os, glob
BASE=r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe"
for f in sorted(glob.glob(os.path.join(BASE,"*.json"))):
    try:
        d=json.load(open(f,encoding="utf-8"))
        n=len(d) if hasattr(d,"__len__") else "?"
        print("OK  %-26s %10d bytes  len=%s" % (os.path.basename(f), os.path.getsize(f), n))
    except Exception as e:
        print("FAIL %-26s %s" % (os.path.basename(f), e))
print()
for f in sorted(glob.glob(os.path.join(BASE,"*.md"))):
    print("MD  %-26s %10d bytes" % (os.path.basename(f), os.path.getsize(f)))
print()
print("resources extracted:", len(os.listdir(os.path.join(BASE,"resources"))))
for n in sorted(os.listdir(os.path.join(BASE,"resources"))): print("   ", n, os.path.getsize(os.path.join(BASE,"resources",n)))
