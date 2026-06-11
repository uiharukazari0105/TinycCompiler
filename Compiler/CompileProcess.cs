using Compiler.Evaluation;
using Compiler.Ir;
using Compiler.Ir.Backend;
using Compiler.Output;
using Compiler.Tokens;
using Compiler.Tokens.Binding;
using Compiler.Tokens.Syntax;

namespace Compiler;

public class CompileProcess : IDisposable
{
    public StreamReader InputFileReader { get; }
    public StreamWriter OutputFileWriter { get; }
    public SyntaxTree? SyntaxTree { get; set; }

    public BoundGlobalScope? GlobalScope { get; set; }

    public Dictionary<VariableSymbol, dynamic> Variables { get; } = new();

    private readonly BackendKind _backendKind;
    private readonly FileStream _inputFile;
    private readonly FileStream _outputFile;

    public CompileProcess(string[] args, BackendKind backendKind)
    {
        _backendKind = backendKind;
        if (args.Length == 0)
            LogDefinition.InvalidFileArgsLog.Raise();

        var inputFilePath = args[0];
        var outputFilePath = args.Length >= 2 ? args[1] : "./output";

        if (!File.Exists(inputFilePath))
            LogDefinition.InputFileNotExistLog.Raise();

        var outputFolder = Directory.GetParent(outputFilePath);
        if (outputFolder is null || !outputFolder.Exists)
            LogDefinition.OutputFolderNotExistLog.Raise();

        _inputFile = new FileStream(inputFilePath, FileMode.Open);
        InputFileReader = new StreamReader(_inputFile);

        _outputFile = new FileStream(outputFilePath, FileMode.Create);
        OutputFileWriter = new StreamWriter(_outputFile);

        LogDefinition.StartCompileLog.Raise();
    }

    public EvaluationResult Evaluate(string? input = null)
    {
        var raw = input ?? InputFileReader.ReadToEnd();
        
        Console.WriteLine("=== 原始源代码 ===");
        Console.WriteLine(raw);
        
        var source = new Preprocessor(raw).Process();
        Console.WriteLine("=== 预处理后 ===");
        Console.WriteLine(source);
        
        SyntaxTree = new SyntaxTree(source);
        Console.WriteLine("=== Token 列表 ===");
        foreach (var token in SyntaxTree.Tokens)
            Console.WriteLine($"  {token.Kind,-22} {token.Text}");
        
        GlobalScope = Binder.BindGlobalScope(GlobalScope, SyntaxTree.Root);
        var boundStatement = GlobalScope.Statement;

        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(PrettyPrint.OutDoubleTree(SyntaxTree, boundStatement, 3, true));
        Console.ForegroundColor = color;
        
        var irGen = new IrGenerator();
        var ir = irGen.Generate(boundStatement);
        Console.WriteLine("=== 三地址码中间代码 ===");
        foreach (var inst in ir)
            Console.WriteLine($"  {inst}");
        
        Console.WriteLine("=== 目标代码 ===");
        var asm = Backend.Create(ir, _backendKind).Generate();
        
        Console.WriteLine(asm);
        
        List<LogDefinition> diagnostics = [];
        diagnostics.AddRange(SyntaxTree.Diagnostics);
        diagnostics.AddRange(GlobalScope.Diagnostics);
        Logger.RaiseMany(diagnostics);
        
        var evaluator = new Evaluator(boundStatement, Variables);
        return new EvaluationResult(diagnostics, evaluator.Evaluate());
    }

    public void Dispose()
    {
        InputFileReader.Dispose();
        OutputFileWriter.Dispose();
        _inputFile.Dispose();
        _outputFile.Dispose();
    }
}
