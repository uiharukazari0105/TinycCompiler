namespace Compiler.Tokens.Syntax;

public static class SyntaxFact
{
    public static SyntaxKind GetKeywordKind(string text)
    {
        switch (text)
        {
            case "true":
                return SyntaxKind.TrueKeyword;
            case "false":
                return SyntaxKind.FalseKeyword;
            case "int":
                return SyntaxKind.IntKeyword;
            case "if":
                return SyntaxKind.IfKeyWord;
            case "else":
                return SyntaxKind.ElseKeyword;
            case "for":
                return SyntaxKind.ForKeyword;
            case "while":
                return SyntaxKind.WhileKeyword;
            default:
                return SyntaxKind.Identifier;
        }
    }
}