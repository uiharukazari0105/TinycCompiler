namespace Compiler.Output;

public class LogDefinition
{
    public LogLevel Level { get; init; }
    public string Message { get; init; }
    
    public TextSpan? Span { get; init; }
    
    public bool Halt {get; init; }
    
    public LogDefinition(LogLevel level, string message, bool halt = false, TextSpan? span = null)
    {
        Level = level;
        Message = message;
        Halt = halt;
        Span = span;
    }

    public void Raise()
    {
        Logger.Raise(this);
    }
    
    public static readonly LogDefinition StartCompileLog = new(LogLevel.Info, "开始编译");
    
    public static readonly LogDefinition InvalidFileArgsLog = new(LogLevel.Error, "无法解析输出输出文件目录", true);
    public static readonly LogDefinition InputFileNotExistLog = new(LogLevel.Error, "找不到输入的源码文件", true);
    public static readonly LogDefinition OutputFolderNotExistLog = new(LogLevel.Error, "输出目录不存在", true);
}