using Compiler.Output;

namespace Compiler;

public static class Program
{
    public static int Main(string[] args)
    {
        args = ["./main.c"]; //MOCK
        
        var compileProcess = new CompileProcess(args);

        var parser = new Parser(compileProcess.InputFileReader.ReadToEnd());
        var syntaxTree = parser.Parse();
        Logger.RaiseMany(parser.Diagnostics);
        
        
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        PrettyPrint.Out(syntaxTree.Root);
        Console.ForegroundColor = color;

        var evaluator = new Evaluator(syntaxTree.Root);
        Console.WriteLine(evaluator.Evaluate());
        
        compileProcess.Dispose();
        return 0;
    }
}