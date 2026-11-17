// ConfigureAnalysis.java - Ghidra 12.1.4 API-correct pre-script.
// Disables the two analyzers that dominate the profile for this image:
//   x86 Constant Reference Analyzer  3582 s
//   Stack                            2984 s
// together 6566 s of the 9077 s total (72 %).
//
// NOTE the API correction against the first attempt:
//   setAnalysisOption(Program, String, String)   -- the 2-argument overload
//   does not exist in Ghidra 12.1.4; that compile error meant NO analyzer was
//   ever disabled in attempt 2 and the timings recorded are DEFAULT-profile
//   timings.
import ghidra.app.script.GhidraScript;

public class ConfigureAnalysis extends GhidraScript {

    private void off(String opt) {
        try {
            setAnalysisOption(currentProgram, opt, "false");
            println("disabled analysis option: " + opt);
        } catch (Throwable t) {
            println("could not disable " + opt + ": " + t);
        }
    }

    @Override
    public void run() throws Exception {
        off("Stack");
        off("x86 Constant Reference Analyzer");
        off("Aggressive Instruction Finder");
        off("Decompiler Parameter ID");
        off("Non-Returning Functions - Discovered");
        off("Decompiler");
        off("Windows x86 Constant Analysis");
        off("Decompiler Switch Analysis");
        off("Function ID");
        off("X86 Function Callee Purge");
        off("Scalar Operand References");
        off("Reference");
        off("Data Reference");
        off("Subroutine References");
        off("Create Address Tables - One Time");
        println("ConfigureAnalysis done");
    }
}
