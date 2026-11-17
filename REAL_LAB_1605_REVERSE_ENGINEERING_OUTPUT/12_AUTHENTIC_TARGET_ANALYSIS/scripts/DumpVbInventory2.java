// DumpVbInventory2.java - corrected for the Ghidra 12.1.4 API.
// Runs against the already-saved project (no auto-analysis) and emits:
//   blocks.tsv, functions.tsv, strings.tsv, site_to_function.tsv, func_refs.tsv
//
// arg[0] = output directory
// arg[1] (optional) = file listing code-site addresses (one hex VA per line)
import ghidra.app.script.GhidraScript;
import ghidra.program.model.address.Address;
import ghidra.program.model.address.AddressSpace;
import ghidra.program.model.listing.Data;
import ghidra.program.model.listing.Function;
import ghidra.program.model.listing.FunctionIterator;
import ghidra.program.model.listing.Instruction;
import ghidra.program.model.listing.InstructionIterator;
import ghidra.program.model.listing.Listing;
import ghidra.program.model.mem.MemoryBlock;
import ghidra.program.model.symbol.Reference;
import ghidra.program.model.symbol.ReferenceIterator;
import ghidra.program.model.symbol.Symbol;
import java.io.File;
import java.io.FileOutputStream;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.util.ArrayList;
import java.util.List;

public class DumpVbInventory2 extends GhidraScript {

    private static String esc(String s) {
        if (s == null) {
            return "";
        }
        StringBuilder b = new StringBuilder();
        for (char c : s.toCharArray()) {
            if (c == '\r') b.append("\\r");
            else if (c == '\n') b.append("\\n");
            else if (c == '\t') b.append("\\t");
            else if (c == '\\') b.append("\\\\");
            else if (c < 32 || c > 126) b.append(String.format("\\u%04x", (int) c));
            else b.append(c);
        }
        return b.toString();
    }

    private static PrintWriter open(File dir, String name) throws Exception {
        return new PrintWriter(new OutputStreamWriter(
                new FileOutputStream(new File(dir, name)), "UTF-8"));
    }

    @Override
    public void run() throws Exception {
        String[] args = getScriptArgs();
        String outDir = args.length > 0 ? args[0] : "/tmp";
        File dir = new File(outDir);
        dir.mkdirs();

        Listing listing = currentProgram.getListing();
        AddressSpace defSpace = currentProgram.getAddressFactory().getDefaultAddressSpace();

        // ---- memory blocks --------------------------------------------------
        PrintWriter pw = open(dir, "blocks.tsv");
        pw.println("name\tstart\tend\tsize\trwx\tinitialized");
        for (MemoryBlock mb : currentProgram.getMemory().getBlocks()) {
            pw.println(mb.getName() + "\t" + mb.getStart() + "\t" + mb.getEnd() + "\t"
                    + mb.getSize() + "\t" + (mb.isExecute() ? "x" : "-")
                    + (mb.isWrite() ? "w" : "-") + (mb.isRead() ? "r" : "-") + "\t"
                    + mb.isInitialized());
        }
        pw.close();

        // ---- functions ------------------------------------------------------
        List<Function> funcs = new ArrayList<Function>();
        FunctionIterator fit = currentProgram.getFunctionManager().getFunctions(true);
        while (fit.hasNext()) {
            funcs.add(fit.next());
        }
        pw = open(dir, "functions.tsv");
        pw.println("entry\tname\tsymboltype\tsize\tninstr\tthunk\tmin\tmax");
        for (Function f : funcs) {
            long ninstr = 0;
            InstructionIterator ii = listing.getInstructions(f.getBody(), true);
            while (ii.hasNext()) {
                ii.next();
                ninstr++;
            }
            pw.println(f.getEntryPoint() + "\t" + esc(f.getName()) + "\t"
                    + f.getSymbol().getSymbolType()
                    + "\t" + f.getBody().getNumAddresses() + "\t" + ninstr + "\t"
                    + f.isThunk() + "\t" + f.getBody().getMinAddress() + "\t"
                    + f.getBody().getMaxAddress());
        }
        pw.close();
        println("functions=" + funcs.size());

        // ---- defined data / strings -----------------------------------------
        pw = open(dir, "strings.tsv");
        pw.println("address\tlength\ttype\ttext");
        int ndata = 0;
        for (Data d : listing.getDefinedData(true)) {
            ndata++;
            Object v = d.getValue();
            if (v instanceof String) {
                pw.println(d.getAddress() + "\t" + d.getLength() + "\t"
                        + d.getDataType().getName() + "\t" + esc((String) v));
            }
        }
        pw.close();
        println("definedData=" + ndata);

        // ---- function -> referenced data ------------------------------------
        pw = open(dir, "func_refs.tsv");
        pw.println("func_entry\tfunc_name\tins_addr\tref_type\ttarget_addr\ttarget_text");
        for (Function f : funcs) {
            InstructionIterator ii = listing.getInstructions(f.getBody(), true);
            while (ii.hasNext()) {
                Instruction ins = ii.next();
                Reference[] refs = ins.getReferencesFrom();
                for (int i = 0; i < refs.length; i++) {
                    Reference ref = refs[i];
                    Address to = ref.getToAddress();
                    String text = "";
                    Data d = listing.getDataAt(to);
                    if (d != null) {
                        Object v = d.getValue();
                        if (v instanceof String) {
                            text = esc((String) v);
                        }
                    }
                    if (text.length() == 0) {
                        Symbol sym = currentProgram.getSymbolTable().getPrimarySymbol(to);
                        if (sym != null) {
                            text = "<SYM:" + sym.getName() + ">";
                        }
                    }
                    pw.println(f.getEntryPoint() + "\t" + esc(f.getName()) + "\t"
                            + ins.getAddress() + "\t" + ref.getReferenceType() + "\t"
                            + to + "\t" + text);
                }
            }
        }
        pw.close();

        // ---- code site -> containing function --------------------------------
        if (args.length > 1) {
            List<String> sites = new ArrayList<String>();
            java.io.BufferedReader br = new java.io.BufferedReader(
                    new java.io.InputStreamReader(new java.io.FileInputStream(args[1]), "UTF-8"));
            String line;
            while ((line = br.readLine()) != null) {
                line = line.trim();
                if (line.length() > 0) {
                    sites.add(line);
                }
            }
            br.close();
            pw = open(dir, "site_to_function.tsv");
            pw.println("site\tfunc_entry\tfunc_name\tfunc_size\tfunc_min\tfunc_max");
            int hit = 0;
            for (int i = 0; i < sites.size(); i++) {
                String s = sites.get(i);
                long v;
                try {
                    v = Long.parseLong(s.startsWith("0x") ? s.substring(2) : s, 16);
                } catch (Exception e) {
                    continue;
                }
                Address a = currentProgram.getAddressFactory().getDefaultAddressSpace()
                        .getAddress(v);
                Function f = currentProgram.getFunctionManager().getFunctionContaining(a);
                if (f != null) {
                    hit++;
                    pw.println(s + "\t" + f.getEntryPoint() + "\t" + esc(f.getName()) + "\t"
                            + f.getBody().getNumAddresses() + "\t"
                            + f.getBody().getMinAddress() + "\t" + f.getBody().getMaxAddress());
                } else {
                    pw.println(s + "\t\t\t\t\t");
                }
            }
            pw.close();
            println("sites=" + sites.size() + " insideFunctions=" + hit);
        }

        // ---- xrefs to strings (iterator form) --------------------------------
        pw = open(dir, "string_xrefs.tsv");
        pw.println("target_addr\ttarget_text\tfrom_addr\tfrom_func");
        for (Data d : listing.getDefinedData(true)) {
            Object v = d.getValue();
            if (!(v instanceof String)) {
                continue;
            }
            Address a = d.getAddress();
            String text = esc((String) v);
            ReferenceIterator ri = currentProgram.getReferenceManager().getReferencesTo(a);
            while (ri.hasNext()) {
                Reference r = ri.next();
                Function ff = listing.getFunctionContaining(r.getFromAddress());
                pw.println(a + "\t" + text + "\t" + r.getFromAddress() + "\t"
                        + (ff == null ? "" : ff.getName()));
            }
        }
        pw.close();

        println("DumpVbInventory2 done");
    }
}
