namespace Compiler.Tokens.Syntax.Expression;

public class CompilationUnitSyntax: SyntaxNode
{
    public override SyntaxKind Kind => SyntaxKind.CompilationUnit;
    public ExpressionSyntax Expression { get; }
    public SyntaxToken EndOfFileToken { get; }

    public CompilationUnitSyntax(ExpressionSyntax expression, SyntaxToken endOfFileToken)
    {
        Expression = expression;
        EndOfFileToken = endOfFileToken;
    }
    
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Expression;
        yield return EndOfFileToken;
    }
}