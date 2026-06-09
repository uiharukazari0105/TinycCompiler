using Compiler.Tokens.Syntax;

namespace Compiler.Tokens.Binding.Operator;

public sealed class BoundUnaryOperator
{
    public SyntaxKind SyntaxKind { get; }
    public BoundUnaryOperatorKind Kind { get; }
    public Type OperandType { get; }
    public Type ResultType { get; }

    private BoundUnaryOperator(SyntaxKind syntaxKind, Type operandType, Type resultType, BoundUnaryOperatorKind kind)
    {
        SyntaxKind = syntaxKind;
        OperandType = operandType;
        ResultType = resultType;
        Kind = kind;
    }
    
    private BoundUnaryOperator(SyntaxKind syntaxKind, Type operatorType, BoundUnaryOperatorKind kind): this(syntaxKind, operatorType, operatorType, kind)
    {
    }

    private static readonly BoundUnaryOperator[] _rules =
    [
        new(SyntaxKind.Exclamation, typeof(bool), BoundUnaryOperatorKind.LogicalNegation),
        new(SyntaxKind.Plus, typeof(int), BoundUnaryOperatorKind.Identity),
        new(SyntaxKind.Minus, typeof(int), BoundUnaryOperatorKind.Negation)
    ];

    public static BoundUnaryOperator? Bind(SyntaxKind syntaxKind, Type operandType)
    {
        foreach (var rule in _rules)
            if (rule.SyntaxKind == syntaxKind && rule.OperandType == operandType)
                return rule;
        return null;
    }
}