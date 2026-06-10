using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax.Statement;

internal class EmptyStatementSyntax : StatementSyntax
{
    public override SyntaxKind Kind => SyntaxKind.EmptyStatement;
    public SyntaxToken EndOfLineToken { get; }
    public EmptyStatementSyntax(SyntaxToken endOfLineToken)
    {
        EndOfLineToken = endOfLineToken;
    }
    
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return EndOfLineToken;
    }
}