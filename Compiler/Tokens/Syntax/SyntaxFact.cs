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
            default:
                return SyntaxKind.Identifier;
        }
    }
}