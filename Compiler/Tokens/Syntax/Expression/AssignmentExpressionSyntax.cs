namespace Compiler.Tokens.Syntax.Expression;

public class AssignmentExpressionSyntax: ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;

    public SyntaxToken IdentifierToken { get; }
    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Expression { get; }

    public AssignmentExpressionSyntax(SyntaxToken identifierToken, SyntaxToken operatorToken, ExpressionSyntax expression)
    {
        IdentifierToken = identifierToken;
        OperatorToken = operatorToken;
        Expression = expression;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return  IdentifierToken;
        yield return OperatorToken;
        yield return Expression;
    }
}