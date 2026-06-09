using Compiler.Output;

namespace Compiler;

public static class Program
{
    public static int Main(string[] args)
    {
        args = ["./main.c"]; //MOCK
        
        var compileProcess = new CompileProcess(args);

        while (true)
        {
            Console.Write(">>");
            var evaluation = compileProcess.Evaluate(Console.ReadLine());
            Logger.RaiseMany(evaluation.Diagnostics);
            Console.WriteLine(evaluation.Value);
        }
        
        compileProcess.Dispose();
        return 0;
    }
}