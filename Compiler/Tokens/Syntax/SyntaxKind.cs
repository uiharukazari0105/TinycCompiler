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
    
    //Keywords
    TrueKeyword,
    FalseKeyword,
    IntKeyword,
    Identifier,
    
    //Statements
    BlockStatement,
    ExpressionStatement,
    VariableDeclarationStatement
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
                SyntaxKind.DoubleEquals or SyntaxKind.ExclamationEquals => 3,
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
                SyntaxKind.DoublePlus => 7,
                _ => 0
            };
        }
    }
}