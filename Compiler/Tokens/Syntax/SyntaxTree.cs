using Compiler.Output;
using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax;

public class SyntaxTree
{
    public string Text { get; }
    public CompilationUnitSyntax Root { get; }
    public SyntaxToken[] Tokens { get; }

    public List<LogDefinition> Diagnostics { get; }

    public SyntaxTree(string text)
    {
        var parser = new Parser(text);
        var root = parser.ParseCompilationUnit();
        Diagnostics = parser.Diagnostics;
        Tokens = parser.Tokens;

        Text = text;
        Root = root;
    }
}