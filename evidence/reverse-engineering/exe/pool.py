import struct, json
p = r"F:\REAL_LAB_COPY\real lab system\RL_CD2019Access\System\real lab system.exe"
data = open(p,"rb").read()

def decode_entry(off):
    """Interpret a VB6 length-prefixed literal at off. Returns dict or None."""
    if off < 0 or off+4 > len(data): return None
    n = struct.unpack_from("<I", data, off)[0]
    if n < 2 or n > 0x40000 or off+4+n > len(data): return None
    body = data[off+4:off+4+n]
    ansi_ok = False; uni_ok = False
    a_txt=None; u_txt=None
    if body[-1]==0:
        a_txt = body[:-1].decode("latin-1")
        ansi_ok = bool(a_txt) and all((32<=ord(c)<=255) for c in a_txt) and any(c.isalnum() for c in a_txt)
    if n % 2 == 0 and body[-2:]==b"\x00\x00":
        try:
            u = body.decode("utf-16le")
            if u.endswith("\x00"): u = u[:-1]
            u_txt = u
            uni_ok = bool(u) and all((0x20<=ord(c)<=0x7e) or (0x600<=ord(c)<=0x6ff) or (0xfb00<=ord(c)<=0xfeff) or (0x2010<=ord(c)<=0x201f) or (0x40<=ord(c)<=0x5f) for c in u) and any(c.isalnum() for c in u)
        except Exception:
            uni_ok = False
    interior_zeros = body[:-2].count(0) if n>=2 else 0
    if uni_ok and (not ansi_ok or interior_zeros>=1):
        return {"enc":"utf16le","byte_len":n,"text":u_txt,"next":off+4+n}
    if ansi_ok:
        return {"enc":"ansi","byte_len":n,"text":a_txt,"next":off+4+n}
    if uni_ok:
        return {"enc":"utf16le","byte_len":n,"text":u_txt,"next":off+4+n}
    return None

# find pool starts: offsets where a long chain begins
def chain_len(start, cap=20000):
    o=start; c=0
    while c<cap:
        e = decode_entry(o)
        if not e: return c, o
        o = e["next"]; c+=1
    return c, o

best=[]
for off in range(0x269000, 0x26c000):
    c,_ = chain_len(off, 40)
    if c>=40: best.append(off)
print("pool start candidates:", len(best), [hex(x) for x in best[:10]])
pool_start = best[0] if best else 0x26b96c

literals=[]; gaps=[]
o = pool_start
while o < len(data)-8:
    e = decode_entry(o)
    if not e:
        nxt=None
        for d in range(1, 4096):
            if o+d > 0x48a0000: break
            e2 = decode_entry(o+d)
            if e2:
                nxt=o+d; break
        if nxt is None: break
        gaps.append({"from":hex(o),"to":hex(nxt),"size":nxt-o})
        o = nxt
        continue
    literals.append({"offset":o,"enc":e["enc"],"byte_len":e["byte_len"],"text":e["text"]})
    o = e["next"]
print("literals:", len(literals), "gaps:", len(gaps))
json.dump({"literals":literals,"gaps":gaps}, open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\vb_literal_pool.json","w",encoding="utf-8"), ensure_ascii=False)
for l in literals[:8]: print(hex(l["offset"]), l["enc"], repr(l["text"][:60]))
print("...")
for l in literals[-8:]: print(hex(l["offset"]), l["enc"], repr(l["text"][:60]))
print("gaps:", [(g["from"],g["to"]) for g in gaps[:20]])
