namespace Compiler.Tokens.Binding.Expression;

public enum BoundNodeKind
{
    //Expressions
    UnaryExpression,
    LiteralExpression,
    BinaryExpression,
    VariableExpression,
    AssignmentExpression,
    ConversionExpression,
    
    //Statements
    BlockStatement,
    ExpressionStatement,
    VariableDeclarationStatement,
    IfStatement,
    WhileStatement,
    ForStatement,
    EmptyStatement
}