namespace Compiler.Tokens.Syntax.Expression;

public sealed class CommaExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.CommaExpression;
    public ExpressionSyntax Left { get; }
    public SyntaxToken CommaToken { get; }
    public ExpressionSyntax Right { get; }

    public CommaExpressionSyntax(ExpressionSyntax left, SyntaxToken commaToken, ExpressionSyntax right)
    {
        Left = left;
        CommaToken = commaToken;
        Right = right;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Left;
        yield return CommaToken;
        yield return Right;
    }
}
