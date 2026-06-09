using Compiler.Output;

namespace Compiler;

public class CompileProcess: IDisposable
{
    public StreamReader InputFileReader { get; }
    public StreamWriter OutputFileWriter { get; }
    
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

    public void Dispose()
    {
        InputFileReader.Dispose();
        OutputFileWriter.Dispose();
        _inputFile.Dispose();
        _outputFile.Dispose();
    }
}