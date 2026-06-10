using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax.Statement;

public sealed class WhileStatementSyntax: StatementSyntax
{
    public override SyntaxKind Kind => SyntaxKind.WhileStatement;
    public SyntaxToken WhileKeyword { get; }
    public ExpressionSyntax Condition { get; }
    public StatementSyntax Statement { get; }

    public WhileStatementSyntax(SyntaxToken whileKeyword, ExpressionSyntax condition, StatementSyntax statement)
    {
        WhileKeyword = whileKeyword;
        Condition = condition;
        Statement = statement;
    }
    
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return WhileKeyword;
        yield return Condition;
        yield return Statement;
    }
}