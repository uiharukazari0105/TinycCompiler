using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax.Statement;

public class FunctionDeclarationSyntax : StatementSyntax
{
    public override SyntaxKind Kind => SyntaxKind.FunctionDeclaration;
    public SyntaxToken ReturnType { get; }
    public SyntaxToken Identifier { get; }
    public SyntaxToken OpenParenthesis { get; }
    public SyntaxToken CloseParenthesis { get; }
    public BlockStatementSyntax Body { get; }

    public FunctionDeclarationSyntax(SyntaxToken returnType, SyntaxToken identifier,
        SyntaxToken openParenthesis, SyntaxToken closeParenthesis, BlockStatementSyntax body)
    {
        ReturnType = returnType;
        Identifier = identifier;
        OpenParenthesis = openParenthesis;
        CloseParenthesis = closeParenthesis;
        Body = body;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return ReturnType;
        yield return Identifier;
        yield return OpenParenthesis;
        yield return CloseParenthesis;
        yield return Body;
    }
}
