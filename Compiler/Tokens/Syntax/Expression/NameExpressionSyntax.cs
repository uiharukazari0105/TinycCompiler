namespace Compiler.Tokens.Syntax.Expression;

public class NameExpressionSyntax: ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.NameExpression;
    
    public SyntaxToken IdentifierToken { get; }

    public NameExpressionSyntax(SyntaxToken identifierToken)
    {
        IdentifierToken = identifierToken;
    }

    
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return IdentifierToken;
    }
}