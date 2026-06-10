using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax.Statement;

public sealed class ExpressionStatementSyntax: StatementSyntax
{
    public override SyntaxKind Kind => SyntaxKind.ExpressionStatement;
    
    public ExpressionSyntax Expression { get; }

    public ExpressionStatementSyntax(ExpressionSyntax expression)
    {
        Expression = expression;
    }
    
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return  Expression;
    }
}