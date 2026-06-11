namespace Compiler.Tokens.Binding;

public enum BoundBinaryOperatorKind
{
    Addition,
    Subtraction,
    Multiplication,
    Division,
    BitwiseAnd,
    LogicalAnd,
    BitwiseOr,
    LogicalOr,
    Equality,
    Inequality,
    Less,
    LessOrEquals,
    Greater,
    GreaterOrEquals,
    ExclusiveOr
}