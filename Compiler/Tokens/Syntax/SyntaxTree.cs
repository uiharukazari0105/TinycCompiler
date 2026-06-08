namespace Compiler.Tokens.Syntax;

public class SyntaxTree
{
    public ExpressionSyntax Root { get; }
    public SyntaxToken EndOfFileToken { get; }
    
    public SyntaxTree(ExpressionSyntax root, SyntaxToken endOfFileToken)
    {
        Root = root;
        EndOfFileToken = endOfFileToken;
    }
}