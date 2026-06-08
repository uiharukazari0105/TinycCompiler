namespace Compiler.Tokens.Syntax;

public enum SyntaxKind
{
    //Tokens
    Number,
    WhiteSpace,
    Plus,
    Minus,
    Star,
    Slash,
    OpenParenthesis,
    CloseParenthesis,
    Bad,
    EndOfFile,
    
    //Expressions
    BinaryExpression,
    LiteralExpression,
    ParenthesizedExpression,
    UnaryExpression
}

public static class SyntaxKindExtensions
{
    extension(SyntaxKind kind)
    {
        public int GetBinaryPrecedence()
        {
            return kind switch
            {
                SyntaxKind.Plus or SyntaxKind.Minus => 1,
                SyntaxKind.Star or SyntaxKind.Slash => 2,
                _ => 0
            };
        }

        public int GetUnaryPrecedence()
        {
            return kind switch
            {
                SyntaxKind.Plus or SyntaxKind.Minus => 3,
                _ => 0
            };
        }
    }
}