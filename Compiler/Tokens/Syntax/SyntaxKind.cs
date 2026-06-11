namespace Compiler.Tokens.Syntax;

public enum SyntaxKind
{
    //Tokens
    Number,
    WhiteSpace,
    Plus,
    DoublePlus,
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
    OpenBrace,
    CloseBrace,
    Equals,
    DoubleEquals,
    ExclamationEquals,
    LessOrEquals,
    Less,
    GreaterOrEquals,
    Greater,
    Comma,
    Caret,
    Tilde,
    Bad,
    EndOfFile,
    
    //Expressions
    BinaryExpression,
    LiteralExpression,
    ParenthesizedExpression,
    UnaryExpression,
    NameExpression,
    AssignmentExpression,
    CompilationUnit,
    Semicolon,
    
    //Keywords
    TrueKeyword,
    FalseKeyword,
    IntKeyword,
    Identifier,
    IfKeyWord,
    ElseKeyword,
    ForKeyword,
    WhileKeyword,
    
    //Statements
    BlockStatement,
    ExpressionStatement,
    VariableDeclarationStatement,
    IfStatement,
    ElseClause,
    WhileStatement,
    ForStatement,
    EmptyStatement
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
                SyntaxKind.Caret => 3,
                SyntaxKind.DoubleEquals or SyntaxKind.ExclamationEquals or 
                SyntaxKind.Less or SyntaxKind.LessOrEquals or 
                SyntaxKind.Greater or SyntaxKind.GreaterOrEquals => 4,
                SyntaxKind.Plus or SyntaxKind.Minus => 5,
                SyntaxKind.Star or SyntaxKind.Slash => 6,
                _ => 0
            };
        }

        public int GetUnaryPrecedence()
        {
            return kind switch
            {
                SyntaxKind.Plus or SyntaxKind.Minus or SyntaxKind.Exclamation or SyntaxKind.Tilde => 7,
                SyntaxKind.DoublePlus => 8,
                _ => 0
            };
        }
    }
}