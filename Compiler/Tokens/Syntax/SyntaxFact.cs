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
            default:
                return SyntaxKind.Identifier;
        }
    }
}