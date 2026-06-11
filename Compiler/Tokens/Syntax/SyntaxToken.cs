using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax;

public class SyntaxToken: SyntaxNode 
{
    public override SyntaxKind Kind { get; }

    public int Position { get; }
    public string Text { get; }

    public SyntaxToken(SyntaxKind kind, int position, string text)
    {
        Kind = kind;
        Position = position;
        Text = text;
    }
    
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return [];
    }
}