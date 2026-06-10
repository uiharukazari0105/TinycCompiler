using Compiler.Evaluation;
using Compiler.Output;
using Compiler.Tokens;
using Compiler.Tokens.Binding;
using Compiler.Tokens.Syntax;

namespace Compiler;

public class CompileProcess: IDisposable
{
    public StreamReader InputFileReader { get; }
    public StreamWriter OutputFileWriter { get; }
    public SyntaxTree SyntaxTree { get; set; }

    public BoundGlobalScope? GlobalScope { get; set; } = null;

    public Dictionary<VariableSymbol, dynamic> Variables { get; } = new();
    
    private readonly FileStream _inputFile;
    private readonly FileStream _outputFile;
    
    public CompileProcess(string[] args)
    {
        if (args.Length == 0)
            LogDefinition.InvalidFileArgsLog.Raise();

        var inputFilePath = args[0];
        var outputFilePath = args.Length >= 2 ? args[1] : "./output";
        
        if (!File.Exists(inputFilePath))
            LogDefinition.InputFileNotExistLog.Raise();

        var outputFolder = Directory.GetParent(outputFilePath);
        if(outputFolder is null || !outputFolder.Exists)
            LogDefinition.OutputFolderNotExistLog.Raise();
        
        _inputFile = new FileStream(inputFilePath, FileMode.Open);
        InputFileReader = new StreamReader(_inputFile);
        
        _outputFile = new FileStream(outputFilePath, FileMode.Create);
        OutputFileWriter = new StreamWriter(_outputFile);
        
        LogDefinition.StartCompileLog.Raise();
    }

    public EvaluationResult Evaluate(string? input = null)
    {
        SyntaxTree = new SyntaxTree(input ?? InputFileReader.ReadToEnd());
        GlobalScope = Binder.BindGlobalScope(GlobalScope ,SyntaxTree.Root);

        var boundStatement = GlobalScope.Statement;
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        PrettyPrint.Out(SyntaxTree.Root);
        Console.ForegroundColor = color;

        var evaluator = new Evaluator(boundStatement, Variables);

        List<LogDefinition> diagnostics = [];
        diagnostics.AddRange(SyntaxTree.Diagnostics);
        diagnostics.AddRange(GlobalScope.Diagnostics);
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