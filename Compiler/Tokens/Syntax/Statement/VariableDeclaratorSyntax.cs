using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax.Statement;

public sealed class VariableDeclaratorSyntax : SyntaxNode
{
    public override SyntaxKind Kind => SyntaxKind.VariableDeclarator;
    public SyntaxToken Identifier { get; }
    public SyntaxToken? EqualsToken { get; }
    public ExpressionSyntax? Initializer { get; }

    public VariableDeclaratorSyntax(SyntaxToken identifier,
        SyntaxToken? equalsToken, ExpressionSyntax? initializer)
    {
        Identifier = identifier;
        EqualsToken = equalsToken;
        Initializer = initializer;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Identifier;
        if (EqualsToken != null)
            yield return EqualsToken;
        if (Initializer is not null)
            yield return Initializer;
    }
}
