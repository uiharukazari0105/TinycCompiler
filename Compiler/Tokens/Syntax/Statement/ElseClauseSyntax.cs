using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax.Statement;

public class ElseClauseSyntax: SyntaxNode
{
    public override SyntaxKind Kind => SyntaxKind.ElseClause;
    public SyntaxToken ElseKeyword { get; }
    public StatementSyntax ElseStatement { get; }

    public ElseClauseSyntax(SyntaxToken elseKeyword, StatementSyntax elseStatement)
    {
        ElseKeyword = elseKeyword;
        ElseStatement = elseStatement;
    }
    
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return ElseKeyword;
        yield return ElseStatement;
    }
}