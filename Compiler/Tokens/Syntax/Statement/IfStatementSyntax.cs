using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax.Statement;

public sealed class IfStatementSyntax: StatementSyntax
{
    public SyntaxToken IfKeyword { get; }
    public ExpressionSyntax Condition { get; }
    public StatementSyntax ThenStatement { get; }
    public ElseClauseSyntax? ElseClause { get; }
    
    public override SyntaxKind Kind => SyntaxKind.IfStatement;

    public IfStatementSyntax(SyntaxToken ifKeyword, ExpressionSyntax condition, StatementSyntax thenStatement,
        ElseClauseSyntax? elseClause)
    {
        IfKeyword = ifKeyword;
        Condition = condition;
        ThenStatement = thenStatement;
        ElseClause = elseClause;
    }
    
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return IfKeyword;
        yield return Condition;
        yield return ThenStatement;
        if(ElseClause is not null)
            yield return ElseClause;
    }
}