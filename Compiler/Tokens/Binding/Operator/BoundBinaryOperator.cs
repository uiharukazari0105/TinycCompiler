using System.Numerics;
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
        new(typeof(INumber<>), SyntaxKind.Plus, BoundBinaryOperatorKind.Addition),
        new(typeof(INumber<>), SyntaxKind.Minus, BoundBinaryOperatorKind.Subtraction),
        new(typeof(INumber<>), SyntaxKind.Star, BoundBinaryOperatorKind.Multiplication),
        new(typeof(INumber<>), SyntaxKind.Slash, BoundBinaryOperatorKind.Division),
        new(typeof(INumber<>), SyntaxKind.Ampersand, BoundBinaryOperatorKind.BitwiseAnd),
        new(typeof(bool), SyntaxKind.Ampersand, BoundBinaryOperatorKind.BitwiseAnd),
        new(typeof(IComparable), SyntaxKind.DoubleAmpersand, BoundBinaryOperatorKind.LogicalAnd),
        new(typeof(INumber<>), SyntaxKind.Pipe, BoundBinaryOperatorKind.BitwiseOr),
        new(typeof(bool), SyntaxKind.Pipe, BoundBinaryOperatorKind.BitwiseOr),
        new(typeof(IComparable), SyntaxKind.DoublePipe, BoundBinaryOperatorKind.LogicalOr),
        new(typeof(IComparable), SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.Equality),
        new(typeof(IComparable), SyntaxKind.ExclamationEquals, BoundBinaryOperatorKind.Inequality),
        new(typeof(IComparable), SyntaxKind.Less, BoundBinaryOperatorKind.Less),
        new(typeof(IComparable), SyntaxKind.LessOrEquals, BoundBinaryOperatorKind.LessOrEquals),
        new(typeof(IComparable), SyntaxKind.Greater, BoundBinaryOperatorKind.Greater),
        new(typeof(IComparable), SyntaxKind.GreaterOrEquals, BoundBinaryOperatorKind.GreaterOrEquals),
        new(typeof(short), SyntaxKind.Caret, BoundBinaryOperatorKind.ExclusiveOr),
        new(typeof(int), SyntaxKind.Caret, BoundBinaryOperatorKind.ExclusiveOr),
        new(typeof(long), SyntaxKind.Caret, BoundBinaryOperatorKind.ExclusiveOr),
        new(typeof(bool), SyntaxKind.Caret, BoundBinaryOperatorKind.ExclusiveOr)
    ];

    public static BoundBinaryOperator? Bind(Type leftType, SyntaxKind operatorTokenKind, Type rightType)
    {
        foreach (var rule in _rules)
            if (MatchType(rule.LeftType, leftType) && 
                rule.SyntaxKind == operatorTokenKind &&
                MatchType(rule.RightType, rightType))
                return rule;
        return null;
    }

    private static bool MatchType(Type ruleType, Type actualType)
    {
        if (ruleType.IsGenericTypeDefinition)
            return actualType.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == ruleType);
        return ruleType.IsAssignableFrom(actualType);
    }
}