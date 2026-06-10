using System.Collections.Immutable;
using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax.Statement;

public class BlockStatementSyntax: StatementSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BlockStatement;
    public SyntaxToken OpenBraceToken { get; }
    public ImmutableArray<StatementSyntax> Statements { get; }
    public SyntaxToken CloseBraceToken { get; }

    public BlockStatementSyntax(SyntaxToken openBraceToken, ImmutableArray<StatementSyntax> statements, SyntaxToken closeBraceToken)
    {
        OpenBraceToken = openBraceToken;
        Statements = statements;
        CloseBraceToken = closeBraceToken;
    }
    
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return OpenBraceToken;
        foreach (var statement in Statements)
            yield return statement;
        yield return CloseBraceToken;
    }
}