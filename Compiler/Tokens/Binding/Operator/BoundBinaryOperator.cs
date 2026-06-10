using Compiler.Tokens.Syntax;

namespace Compiler.Tokens.Binding.Operator;

public sealed class BoundBinaryOperator
{
    public Type LeftType { get; }
    public SyntaxKind SyntaxKind { get; }
    public Type RightType { get; }
    public Type ResultType { get; }
    public BoundBinaryOperatorKind Kind { get; }


    private BoundBinaryOperator(Type leftType, SyntaxKind syntaxKind, Type rightType, Type resultType, BoundBinaryOperatorKind kind)
    {
        LeftType = leftType;
        SyntaxKind = syntaxKind;
        RightType = rightType;
        ResultType = resultType;
        Kind = kind;
    }
    
    private BoundBinaryOperator(Type leftType, SyntaxKind syntaxKind, Type rightType, BoundBinaryOperatorKind kind): this(leftType, syntaxKind, rightType, leftType, kind)
    {
    }
    
    private BoundBinaryOperator(Type type, SyntaxKind syntaxKind, BoundBinaryOperatorKind kind): this(type, syntaxKind, type, type, kind)
    {
    }

    private static readonly BoundBinaryOperator[] _rules =
    [
        new(typeof(int), SyntaxKind.Plus, BoundBinaryOperatorKind.Addition),
        new(typeof(int), SyntaxKind.Minus, BoundBinaryOperatorKind.Subtraction),
        new(typeof(int), SyntaxKind.Star, BoundBinaryOperatorKind.Multiplication),
        new(typeof(int), SyntaxKind.Slash, BoundBinaryOperatorKind.Division),
        new(typeof(bool), SyntaxKind.Ampersand, BoundBinaryOperatorKind.BitwiseAnd),
        new(typeof(bool), SyntaxKind.DoubleAmpersand, BoundBinaryOperatorKind.LogicalAnd),
        new(typeof(bool), SyntaxKind.Pipe, BoundBinaryOperatorKind.BitwiseOr),
        new(typeof(bool), SyntaxKind.DoublePipe, BoundBinaryOperatorKind.LogicalOr),
        new(typeof(int), SyntaxKind.DoubleEquals, typeof(int), typeof(bool), BoundBinaryOperatorKind.Equality),
        new(typeof(int), SyntaxKind.ExclamationEquals, typeof(int), typeof(bool), BoundBinaryOperatorKind.Inequality),
        new(typeof(bool), SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.Equality),
        new(typeof(bool), SyntaxKind.ExclamationEquals, BoundBinaryOperatorKind.Inequality),
        new(typeof(int), SyntaxKind.Less, typeof(int), typeof(bool), BoundBinaryOperatorKind.Less),
        new(typeof(int), SyntaxKind.LessOrEquals, typeof(int), typeof(bool), BoundBinaryOperatorKind.LessOrEquals),
        new(typeof(int), SyntaxKind.Greater, typeof(int), typeof(bool), BoundBinaryOperatorKind.Greater),
        new(typeof(int), SyntaxKind.GreaterOrEquals, typeof(int), typeof(bool), BoundBinaryOperatorKind.GreaterOrEquals)
    ];

    public static BoundBinaryOperator? Bind(Type leftType, SyntaxKind operatorTokenKind, Type rightType)
    {
        foreach (var rule in _rules)
            if (rule.LeftType == leftType && 
                rule.SyntaxKind == operatorTokenKind &&
                rule.RightType == rightType)
                return rule;
        return null;
    }
}