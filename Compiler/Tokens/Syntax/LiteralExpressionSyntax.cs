using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax;

public sealed class LiteralExpressionSyntax: ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

    public SyntaxToken LiteralToken { get; }
    public dynamic? Value { get; }

    public LiteralExpressionSyntax(SyntaxToken literalToken, dynamic? value)
    {
        LiteralToken = literalToken;
        Value = value;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return LiteralToken;
    }
}