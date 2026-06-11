namespace Compiler.Tokens.Syntax;

public static class SyntaxFact
{
    public static SyntaxKind GetKeywordKind(string text)
    {
        foreach (var rule in _rules)
            if(rule.Item1 == text)
                return  rule.Item2;
        
        return SyntaxKind.Identifier;
    }

    private static readonly List<(string, SyntaxKind)> _rules =
    [
        ("short", SyntaxKind.ShortKeyword),
        ("int", SyntaxKind.IntKeyword),
        ("long", SyntaxKind.LongKeyword),
        ("float", SyntaxKind.FloatKeyword),
        ("double", SyntaxKind.DoubleKeyword),
        ("bool", SyntaxKind.BoolKeyword),
        ("char", SyntaxKind.CharKeyword),
        
        ("true", SyntaxKind.TrueKeyword),
        ("false", SyntaxKind.FalseKeyword),
        
        ("if", SyntaxKind.IfStatement),
        ("else", SyntaxKind.ElseKeyword),
        
        ("for", SyntaxKind.ForKeyword),
        ("while", SyntaxKind.WhileKeyword)
    ];
}