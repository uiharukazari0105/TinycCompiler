using Compiler.Output;
using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax;

public class SyntaxTree
{
    public string Text { get; }
    public CompilationUnitSyntax Root { get; }
    
    public List<LogDefinition> Diagnostics { get; }
    
    public SyntaxTree(string text)
    {
        var parser = new Parser(text);
        var root = parser.ParseCompilationUnit();
        Diagnostics = parser.Diagnostics;
        
        Text = text;
        Root = root;
    }

    public static SyntaxTree Parse(string text)
    {
        return new SyntaxTree(text);
    }

    public static IEnumerable<SyntaxToken> ParseTokens(string text)
    {
        var lexer = new Lexer(text);
        while (true)
        {
            var token = lexer.NextToken();
            if(token.Kind == SyntaxKind.EndOfFile)
                break;
            yield return token;
        }
    }
}