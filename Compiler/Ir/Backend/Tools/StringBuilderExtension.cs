using System.Text;

namespace Compiler.Ir.Backend.Tools;

public static class StringBuilderExtension
{
    private const int Indent = 4;

    private static readonly string IndentStr;
    
    static StringBuilderExtension()
    {
        IndentStr = new string(' ', Indent);
    }

    public static void AppendLineWithIndent(this StringBuilder builder, string? value)
    {
        builder.Append(IndentStr).AppendLine(value);
    }
}