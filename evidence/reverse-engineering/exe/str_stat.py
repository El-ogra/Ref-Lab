import json, re, collections
raw = json.load(open(r"C:\Users\LAP LINK\AppData\Local\Temp\opencode\reflabv2\exe\strings_raw.json",encoding="utf-8"))
print("total", len(raw))
lens = collections.Counter()
for d in raw:
    lens[min(len(d["text"])//10*10, 100)] += 1
print(sorted(lens.items()))
# how many look like "real words" (>=2 alpha chars)
good = [d for d in raw if re.search(r"[A-Za-z]{3,}", d["text"])]
print("with 3+ alpha:", len(good))
uniq = {}
for d in raw:
    uniq.setdefault(d["text"], []).append(d["offset"])
print("unique texts:", len(uniq))
# show 60 random-ish unique sorted by length desc top
tops = sorted(uniq.items(), key=lambda kv: -len(kv[0]))[:30]
for t,o in tops: print(len(t), repr(t[:120]))
