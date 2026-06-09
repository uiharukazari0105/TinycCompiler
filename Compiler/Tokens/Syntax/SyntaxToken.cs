using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax;

public class SyntaxToken: SyntaxNode 
{
    public override SyntaxKind Kind { get; }

    public int Position { get; }
    public string Text { get; }
    
    public dynamic? Value { get; }

    public SyntaxToken(SyntaxKind kind, int position, string text, dynamic? value = null)
    {
        Kind = kind;
        Position = position;
        Text = text;
        Value = value;
    }
    
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return [];
    }
}