namespace Compiler.Tokens.Syntax.Expression;

public class AssignmentExpressionSyntax: ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;

    public SyntaxToken IdentifierToken { get; }
    public SyntaxToken EqualsToken { get; }
    public ExpressionSyntax Expression { get; }

    public AssignmentExpressionSyntax(SyntaxToken identifierToken, SyntaxToken equalsToken, ExpressionSyntax expression)
    {
        IdentifierToken = identifierToken;
        EqualsToken = equalsToken;
        Expression = expression;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return  IdentifierToken;
        yield return EqualsToken;
        yield return Expression;
    }
}