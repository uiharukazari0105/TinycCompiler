using Compiler.Output;
using Compiler.Tokens.Binding;

namespace Compiler;

public static class Program
{
    public static int Main(string[] args)
    {
        args = ["./main.c"]; //MOCK
        
        var compileProcess = new CompileProcess(args);

        var parser = new Parser(compileProcess.InputFileReader.ReadToEnd());
        var syntaxTree = parser.Parse();
        var binder = new Binder();
        var boundExpression = binder.BindExpression(syntaxTree.Root);
        Logger.RaiseMany(parser.Diagnostics);
        Logger.RaiseMany(binder.Diagnostics);
        
        
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        PrettyPrint.Out(syntaxTree.Root);
        Console.ForegroundColor = color;

        var evaluator = new Evaluator(boundExpression);
        Console.WriteLine(evaluator.Evaluate());
        
        compileProcess.Dispose();
        return 0;
    }
}