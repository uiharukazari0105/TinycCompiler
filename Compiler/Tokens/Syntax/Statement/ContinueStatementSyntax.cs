using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax.Statement;

public sealed class ContinueStatementSyntax : StatementSyntax
{
    public override SyntaxKind Kind => SyntaxKind.ContinueStatement;
    public SyntaxToken Keyword { get; }

    public ContinueStatementSyntax(SyntaxToken keyword)
    {
        Keyword = keyword;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Keyword;
    }
}
