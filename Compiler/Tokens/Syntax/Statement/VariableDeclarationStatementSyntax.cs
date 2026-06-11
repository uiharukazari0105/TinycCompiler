using System.Collections.Immutable;
using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax.Statement;

public sealed class VariableDeclarationStatementSyntax: StatementSyntax
{
    public SyntaxToken Keyword { get; }
    public ImmutableArray<SyntaxToken> Identifiers { get; }
    public ImmutableArray<SyntaxToken> Commas { get; }
    public SyntaxToken? EqualsToken { get; }
    public ExpressionSyntax? Initializer { get; }
    public override SyntaxKind Kind => SyntaxKind.VariableDeclarationStatement;

    public VariableDeclarationStatementSyntax(SyntaxToken keyword,
        ImmutableArray<SyntaxToken> identifiers,
        ImmutableArray<SyntaxToken> commas,
        SyntaxToken? equalsToken,
        ExpressionSyntax? initializer)
    {
        Keyword = keyword;
        Identifiers = identifiers;
        Commas = commas;
        EqualsToken = equalsToken;
        Initializer = initializer;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Keyword;
        for (var i = 0; i < Identifiers.Length; i++)
        {
            yield return Identifiers[i];
            if (i < Commas.Length)
                yield return Commas[i];
        }
        if (EqualsToken != null)
            yield return EqualsToken;
        if (Initializer is not null)
            yield return Initializer;
    }
}
