namespace Compiler.Tokens.Syntax.Expression;

public sealed class BinaryExpressionSyntax: ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

    public ExpressionSyntax Right { get; }
    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Left { get; }
    
    public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
    {
        Left = left;
        OperatorToken = operatorToken;
        Right = right;
    }
    
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }
}