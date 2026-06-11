namespace Compiler.Tokens.Binding.Operator;

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
    ExclusiveOr,
    Molding,
    SelfAddition,
    SelfSubtraction,
    SelfMultiplication,
    SelfDivision,
    SelfBitwiseAnd,
    SelfBitwiseOr,
    SelfExclusiveOr,
    SelfMolding
}

public static class BoundBinaryOperatorKindExtensions
{
    public static string OperatorString(this BoundBinaryOperatorKind k) => k switch
    {
        BoundBinaryOperatorKind.Addition => "+",
        BoundBinaryOperatorKind.Subtraction => "-",
        BoundBinaryOperatorKind.Multiplication => "*",
        BoundBinaryOperatorKind.Division => "/",
        BoundBinaryOperatorKind.BitwiseAnd => "&",
        BoundBinaryOperatorKind.BitwiseOr => "|",
        BoundBinaryOperatorKind.ExclusiveOr => "^",
        BoundBinaryOperatorKind.LogicalAnd => "&&",
        BoundBinaryOperatorKind.LogicalOr => "||",
        BoundBinaryOperatorKind.Equality => "==",
        BoundBinaryOperatorKind.Inequality => "!=",
        BoundBinaryOperatorKind.Less => "<",
        BoundBinaryOperatorKind.LessOrEquals => "<=",
        BoundBinaryOperatorKind.Greater => ">",
        BoundBinaryOperatorKind.GreaterOrEquals => ">=",
        BoundBinaryOperatorKind.Molding => "%",
        BoundBinaryOperatorKind.SelfAddition => "+",
        BoundBinaryOperatorKind.SelfSubtraction => "-",
        BoundBinaryOperatorKind.SelfMultiplication => "*",
        BoundBinaryOperatorKind.SelfDivision => "/",
        BoundBinaryOperatorKind.SelfBitwiseAnd => "&",
        BoundBinaryOperatorKind.SelfBitwiseOr => "|",
        BoundBinaryOperatorKind.SelfExclusiveOr => "^",
        BoundBinaryOperatorKind.SelfMolding => "%",
        _ => "?",
    };
}