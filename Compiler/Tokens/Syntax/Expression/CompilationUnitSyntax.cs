using Compiler.Tokens.Syntax.Statement;

namespace Compiler.Tokens.Syntax.Expression;

public class CompilationUnitSyntax: SyntaxNode
{
    public override SyntaxKind Kind => SyntaxKind.CompilationUnit;
    public StatementSyntax Statement { get; }
    public SyntaxToken EndOfFileToken { get; }

    public CompilationUnitSyntax(StatementSyntax statement, SyntaxToken endOfFileToken)
    {
        Statement = statement;
        EndOfFileToken = endOfFileToken;
    }
    
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Statement;
        yield return EndOfFileToken;
    }
}