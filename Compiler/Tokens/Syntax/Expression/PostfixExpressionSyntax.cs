namespace Compiler.Tokens.Syntax.Expression;

public sealed class PostfixExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.PostfixExpression;
    public ExpressionSyntax Operand { get; }
    public SyntaxToken OperatorToken { get; }

    public PostfixExpressionSyntax(ExpressionSyntax operand, SyntaxToken operatorToken)
    {
        Operand = operand;
        OperatorToken = operatorToken;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Operand;
        yield return OperatorToken;
    }
}
