using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax.Statement;

public sealed class ReturnStatementSyntax : StatementSyntax
{
    public override SyntaxKind Kind => SyntaxKind.ReturnStatement;
    public SyntaxToken ReturnKeyword { get; }
    public ExpressionSyntax? Expression { get; }

    public ReturnStatementSyntax(SyntaxToken returnKeyword, ExpressionSyntax? expression)
    {
        ReturnKeyword = returnKeyword;
        Expression = expression;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return ReturnKeyword;
        if (Expression is not null)
            yield return Expression;
    }
}
