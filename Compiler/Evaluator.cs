using Compiler.Output;
using Compiler.Tokens.Binding;
using Compiler.Tokens.Binding.Expression;

namespace Compiler;

public sealed class Evaluator
{
    private readonly BoundExpression _root;
    
    public Evaluator(BoundExpression root)
    {
        _root = root;
    }

    public dynamic Evaluate()
    {
        return EvaluateExpression(_root);
    }

    private dynamic EvaluateExpression(BoundExpression node)
    {
        if (node is BoundLiteralExpression n)
            return n.Value;

        if (node is BoundUnaryExpression u)
        {
            var operand = EvaluateExpression(u.Operand);
            switch (u.Operator.Kind)
            {
                case BoundUnaryOperatorKind.Identity:
                    return operand;
                case BoundUnaryOperatorKind.Negation:
                    return -operand;
                case BoundUnaryOperatorKind.LogicalNegation:
                    return !operand;
                default:
                    new LogDefinition(LogLevel.Error, $"不合理的一元运算符 <{u.Operator.Kind}>", true).Raise();
                    return 0;
            }
        }
        
        if (node is BoundBinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            switch (b.Operator.Kind)
            {
                case BoundBinaryOperatorKind.Addition:
                    return left + right;
                case BoundBinaryOperatorKind.Subtraction:
                    return left - right;
                case BoundBinaryOperatorKind.Multiplication:
                    return left * right;
                case BoundBinaryOperatorKind.Division:
                    return left / right;
                case BoundBinaryOperatorKind.BitwiseAnd:
                    return left & right;
                case BoundBinaryOperatorKind.LogicalAnd:
                    return left && right;
                case BoundBinaryOperatorKind.BitwiseOr:
                    return left | right;
                case BoundBinaryOperatorKind.LogicalOr:
                    return left || right;
                case BoundBinaryOperatorKind.Equality:
                    return left == right;
                case BoundBinaryOperatorKind.Inequality:
                    return left != right;
                default:
                    new LogDefinition(LogLevel.Error, $"非预期运算符 <{b.Operator.Kind}>", true).Raise();
                    return 0;
            }
        }
        
        new LogDefinition(LogLevel.Error, $"无法解析的节点 <{node.Kind}>", true).Raise();
        return 0;
    }
}