using Compiler.Output;
using Compiler.Tokens.Binding.Expression;
using Compiler.Tokens.Binding.Operator;
using Compiler.Tokens.Syntax;
using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Binding;

public sealed class Binder
{
    public List<LogDefinition> Diagnostics { get; } = [];
    private readonly Dictionary<VariableSymbol, dynamic> _variables;

    public Binder(Dictionary<VariableSymbol, dynamic> variables)
    {
        _variables = variables;
    }
    
    public BoundExpression BindExpression(ExpressionSyntax syntax)
    {
        switch (syntax.Kind)
        {
            case SyntaxKind.LiteralExpression:
                return BindLiteralExpression((LiteralExpressionSyntax)syntax);
            case SyntaxKind.UnaryExpression:
                return BindUnaryExpression((UnaryExpressionSyntax)syntax);
            case SyntaxKind.BinaryExpression:
                return BindBinaryExpression((BinaryExpressionSyntax)syntax);
            case SyntaxKind.ParenthesizedExpression:
                return BindExpression(((ParenthesizedExpressionSyntax)syntax).Expression);
            case SyntaxKind.NameExpression:
                return BindNameExpression((NameExpressionSyntax)syntax);
            case SyntaxKind.AssignmentExpression:
                return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
        }

        Diagnostics.Add(new LogDefinition(LogLevel.Error, $"没有这样的表达式类型 <{syntax.Kind}>", true));
        throw new Exception($"没有这样的表达式类型 <{syntax.Kind}>");
    }

    private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
    {
        var value = syntax.Value ?? 0;
        return new BoundLiteralExpression(value);
    }
    
    private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
    {
        var boundOperand = BindExpression(syntax.Operand);
        var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);
        if (boundOperator is null)
        {
            Diagnostics.Add(new LogDefinition(LogLevel.Error, $"一元运算符 <{syntax.OperatorToken.Text}> 不能运算 <{boundOperand.Type}>", true));
            return boundOperand;
        }
        return new BoundUnaryExpression(boundOperator, boundOperand);
    }

    private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
    {
        var boundLeft = BindExpression(syntax.Left);
        var boundRight = BindExpression(syntax.Right);
        var boundOperator = BoundBinaryOperator.Bind(boundLeft.Type, syntax.OperatorToken.Kind, boundRight.Type);
        if (boundOperator is null)
        {
            Diagnostics.Add(new LogDefinition(LogLevel.Error, $"二元运算符 <{syntax.OperatorToken.Text}> 不能放在 <{boundLeft.Type}> 和 <{boundRight.Type}> 之间", true));
            return boundLeft;
        }
        return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
    }
    
    private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
    {
        var name = syntax.IdentifierToken.Text;
        
        var variable = _variables.Keys.FirstOrDefault(v=>v.Name == name);
        
        if (variable is null)
        {
            Diagnostics.Add(new LogDefinition(LogLevel.Error, $"变量 <{name}> 在该作用域没有声明", true));
            return new BoundLiteralExpression(0);
        }
        
        return new BoundVariableExpression(variable);
    }
    
    private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
    {
        var name = syntax.IdentifierToken.Text;
        var boundExpression = BindExpression(syntax.Expression);
        
        // var existingVariable = _variables.Keys.FirstOrDefault(v => v.Name == name);
        // if (existingVariable is not null)
        //     _variables.Remove(existingVariable);
        
        var variable = new VariableSymbol(name, boundExpression.Type);
        
        return new BoundAssignmentExpression(variable, boundExpression);
    }
}