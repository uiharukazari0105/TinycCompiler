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
            case BoundNodeKind.IfStatement:
                EvaluateIfStatement((BoundIfStatement)node);
                break;
            case BoundNodeKind.WhileStatement:
                EvaluateWhileStatement((BoundWhileStatement)node);
                break;
            case BoundNodeKind.ForStatement:
                EvaluateForStatement((BoundForStatement)node);
                break;
            case BoundNodeKind.EmptyStatement:
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
    
    private void EvaluateIfStatement(BoundIfStatement node)
    {
        if (ToBool(EvaluateExpression(node.Condition)))
            EvaluateStatement(node.ThenStatement);
        else if(node.ElseStatement is not null)
            EvaluateStatement(node.ElseStatement);
    }
    
    private void EvaluateWhileStatement(BoundWhileStatement node)
    {
        while (ToBool(EvaluateExpression(node.Condition)))
            EvaluateStatement(node.Statement);
    }
    
    private void EvaluateForStatement(BoundForStatement node)
    {
        foreach (var initializer in node.Initializers)
            EvaluateStatement(initializer);
        for (; node.Condition is null?true:ToBool(EvaluateExpression(node.Condition));)
        {
            EvaluateStatement(node.Statement);
            foreach (var statement in node.StepStatements)
                EvaluateStatement(statement);
        }
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
            case BoundConversionExpression c:
                return EvaluateConversionExpression(c);
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
                return !ToBool(operand);
            case BoundUnaryOperatorKind.BitwiseNot:
                return ~operand;
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
                return ToBool(left) && ToBool(right);
            case BoundBinaryOperatorKind.BitwiseOr:
                return left | right;
            case BoundBinaryOperatorKind.LogicalOr:
                return ToBool(left) || ToBool(right);
            case BoundBinaryOperatorKind.Equality:
                return ConvertBool(left) == ConvertBool(right);
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
            case BoundBinaryOperatorKind.ExclusiveOr:
                return left ^ right;
            default:
                new LogDefinition(LogLevel.Error, $"非预期运算符 <{b.Operator.Kind}>", true).Raise();
                return 0;
        }
    }
    
    private dynamic EvaluateConversionExpression(BoundConversionExpression boundConversionExpression)
    {
        var operand = EvaluateExpression(boundConversionExpression.Expression);
        return Conversion(operand, boundConversionExpression.Type);
    }

    private dynamic Conversion(dynamic operand, Type targetType)
    {
        try
        {
            return Convert.ChangeType(operand, targetType);
        }
        catch (OverflowException)
        {
            new LogDefinition(LogLevel.Warning, $"从 <{operand.GetType()}> 到 <{targetType}> 的转换溢出")
                .Raise();
            if (targetType == typeof(char))
                return (char)operand;
            if (targetType == typeof(short))
                return (short)operand;
            if (targetType == typeof(int))
                return (int)operand;
            if (targetType == typeof(long))
                return (long)operand;
            if (targetType == typeof(float))
                return (float)operand;
            if (targetType == typeof(double))
                return (double)operand;
        }
        catch (InvalidCastException)
        {
            if (operand is bool)
                return Conversion(ConvertBool(operand), targetType);
            if (targetType ==  typeof(bool))
                return Conversion(ToBool(operand), targetType);
        }
        new LogDefinition(LogLevel.Error, $"不能隐式转换类型 <{operand.GetType()}> 到 <{targetType}>", true).Raise();
        return 0;
    }

    private bool ToBool(dynamic operand)
    {
        if (operand is char || operand is short || operand is int || operand is long || operand is float || operand is double)
            return operand != 0;
        if(operand is bool)
            return operand;
        new LogDefinition(LogLevel.Error, $"不能隐式转换类型 <{operand.GetType()}> 到 <Boolean>", true).Raise();
        return false;
    }
    
    private dynamic ConvertBool(dynamic operand)
    {
        if(operand is bool)
            return operand?1:0;
        return operand;
    }
}