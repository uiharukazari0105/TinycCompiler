using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax.Statement;

public sealed class VariableDeclarationStatementSyntax: StatementSyntax
{
    public SyntaxToken Keyword { get; }
    public SyntaxToken Identifier { get; }
    public SyntaxToken? EqualsToken { get; }
    public ExpressionSyntax? Initializer { get; }
    public override SyntaxKind Kind => SyntaxKind.VariableDeclarationStatement;

    public VariableDeclarationStatementSyntax(SyntaxToken keyword, SyntaxToken identifier, SyntaxToken? equalsToken, ExpressionSyntax? initializer)
    {
        Keyword = keyword;
        Identifier = identifier;
        EqualsToken = equalsToken;
        Initializer = initializer;
    }
    
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Keyword;
        yield return Identifier;
        if(EqualsToken != null)
            yield return EqualsToken;
        if(Initializer is not null)
            yield return Initializer;
    }
}