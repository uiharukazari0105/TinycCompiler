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
    Exclamation,
    Ampersand,
    DoubleAmpersand,
    Pipe,
    DoublePipe,
    OpenParenthesis,
    CloseParenthesis,
    Equals,
    DoubleEquals,
    AmpersandEquals,
    Bad,
    EndOfFile,
    
    //Expressions
    BinaryExpression,
    LiteralExpression,
    ParenthesizedExpression,
    UnaryExpression,
    
    //Keywords
    TrueKeyword,
    FalseKeyword,
    Identifier
}

public static class SyntaxKindExtensions
{
    extension(SyntaxKind kind)
    {
        public int GetBinaryPrecedence()
        {
            return kind switch
            {
                SyntaxKind.Pipe or SyntaxKind.DoublePipe => 1,
                SyntaxKind.Ampersand or SyntaxKind.DoubleAmpersand => 2,
                SyntaxKind.DoubleEquals or SyntaxKind.AmpersandEquals => 3,
                SyntaxKind.Plus or SyntaxKind.Minus => 4,
                SyntaxKind.Star or SyntaxKind.Slash => 5,
                _ => 0
            };
        }

        public int GetUnaryPrecedence()
        {
            return kind switch
            {
                SyntaxKind.Plus or SyntaxKind.Minus or SyntaxKind.Exclamation => 6,
                _ => 0
            };
        }
    }
}