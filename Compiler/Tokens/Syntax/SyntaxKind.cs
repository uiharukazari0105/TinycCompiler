namespace Compiler.Tokens.Syntax;

public enum SyntaxKind
{
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
    BinaryExpression,
    NumberExpression,
    ParenthesizedExpression
}