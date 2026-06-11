using System.Collections.Immutable;
using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax.Statement;

public sealed class VariableDeclarationStatementSyntax : StatementSyntax
{
    public SyntaxToken Keyword { get; }
    public ImmutableArray<VariableDeclaratorSyntax> Declarators { get; }
    public ImmutableArray<SyntaxToken> Commas { get; }
    public override SyntaxKind Kind => SyntaxKind.VariableDeclarationStatement;

    public VariableDeclarationStatementSyntax(SyntaxToken keyword,
        ImmutableArray<VariableDeclaratorSyntax> declarators,
        ImmutableArray<SyntaxToken> commas)
    {
        Keyword = keyword;
        Declarators = declarators;
        Commas = commas;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Keyword;
        for (var i = 0; i < Declarators.Length; i++)
        {
            yield return Declarators[i];
            if (i < Commas.Length)
                yield return Commas[i];
        }
    }
}
