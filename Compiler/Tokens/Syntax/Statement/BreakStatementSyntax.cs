using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax.Statement;

public sealed class BreakStatementSyntax : StatementSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BreakStatement;
    public SyntaxToken Keyword { get; }

    public BreakStatementSyntax(SyntaxToken keyword)
    {
        Keyword = keyword;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Keyword;
    }
}
