using Compiler.Output;
using Compiler.Tokens;
using Compiler.Tokens.Binding;
using Compiler.Tokens.Binding.Expression;
using Compiler.Tokens.Binding.Statement;

namespace Compiler.Evaluation;

public sealed class Evaluator
{
    private readonly BoundStatement _root;
    private readonly Dictionary<VariableSymbol, dynamic> _variables;

    private dynamic? _lastValue;
    
    public Evaluator(BoundStatement root, Dictionary<VariableSymbol, dynamic> variables)
    {
        _root = root;
        _variables = variables;
    }

    public dynamic? Evaluate()
    {
        EvaluateStatement(_root);
        return _lastValue;
    }

    private void EvaluateStatement(BoundStatement node)
    {
        switch (node.Kind)
        {
            case BoundNodeKind.VariableDeclarationStatement:
                EvaluateVariableDeclarationStatement((BoundVariableDeclarationStatement)node);
                break;
            case BoundNodeKind.BlockStatement: 
                EvaluateBlockStatement((BoundBlockStatement)node);
                break;
            case BoundNodeKind.ExpressionStatement:
                EvaluateExpressionStatement((BoundExpressionStatement)node);
                break;
            default:
                new LogDefinition(LogLevel.Error, $"无法解析的表达式 <{node.Kind}>", true).Raise();
                break;
        }
    }

    private void EvaluateVariableDeclarationStatement(BoundVariableDeclarationStatement node)
    {
        var value = EvaluateExpression(node.Initializer);
        _variables[node.Variable] = value;
        _lastValue = value;
    }

    private void EvaluateBlockStatement(BoundBlockStatement node)
    {
        foreach (var variable in node.Statements)
            EvaluateStatement(variable);
    }
    
    private void EvaluateExpressionStatement(BoundExpressionStatement node)
    {
        _lastValue = EvaluateExpression(node.Expression);
    }

    private dynamic EvaluateExpression(BoundExpression node)
    {
        switch (node)
        {
            case BoundLiteralExpression n:
                return EvaluateLiteralExpression(n);
            case BoundVariableExpression v:
                return EvaluateVariableExpression(v);
            case BoundAssignmentExpression a:
                return EvaluateAssignmentExpression(a);
            case BoundUnaryExpression u:
                return EvaluateUnaryExpression(u);
            case BoundBinaryExpression b:
                return EvaluateBinaryExpression(b);
            default:
                new LogDefinition(LogLevel.Error, $"无法解析的节点 <{node.Kind}>", true).Raise();
                return 0;
        }
    }

    private static dynamic EvaluateLiteralExpression(BoundLiteralExpression n)
    {
        return n.Value;
    }
    
    private dynamic EvaluateVariableExpression(BoundVariableExpression v)
    {
        return _variables[v.Variable];
    }
    
    private dynamic EvaluateAssignmentExpression(BoundAssignmentExpression a)
    {
        var value = EvaluateExpression(a.Expression);
        _variables[a.Variable] = value;
        return value;
    }
    
    private dynamic EvaluateUnaryExpression(BoundUnaryExpression u)
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
    
    private dynamic EvaluateBinaryExpression(BoundBinaryExpression b)
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
            case BoundBinaryOperatorKind.Less:
                return left < right;
            case BoundBinaryOperatorKind.LessOrEquals:
                return left <= right;
            case BoundBinaryOperatorKind.Greater:
                return left > right;
            case BoundBinaryOperatorKind.GreaterOrEquals:
                return left >= right;
            default:
                new LogDefinition(LogLevel.Error, $"非预期运算符 <{b.Operator.Kind}>", true).Raise();
                return 0;
        }
    }
}