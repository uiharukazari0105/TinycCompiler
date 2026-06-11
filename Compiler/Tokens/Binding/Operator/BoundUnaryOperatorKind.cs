namespace Compiler.Tokens.Binding.Operator;

public enum BoundUnaryOperatorKind
{
    Identity,
    Negation,
    LogicalNegation,
    PrefixIncrement,
    PrefixDecrement,
    PostfixIncrement,
    PostfixDecrement,
    BitwiseNot
}

public static class BoundUnaryOperatorKindExtensions
{
    public static string OperatorString(this BoundUnaryOperatorKind k) => k switch
    {
        BoundUnaryOperatorKind.Identity => "+",
        BoundUnaryOperatorKind.Negation => "-",
        BoundUnaryOperatorKind.LogicalNegation => "!",
        BoundUnaryOperatorKind.BitwiseNot => "~",
        _ => "?",
    };
}