namespace Compiler.Tokens.Binding;

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