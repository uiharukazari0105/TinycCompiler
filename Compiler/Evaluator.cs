using Compiler.Output;
using Compiler.Tokens.Syntax;

namespace Compiler;

public class Evaluator
{
    private readonly ExpressionSyntax _root;
    
    public Evaluator(ExpressionSyntax root)
    {
        _root = root;
    }

    public int Evaluate()
    {
        return EvaluateExpression(_root);
    }

    private int EvaluateExpression(ExpressionSyntax node)
    {
        if (node is LiteralExpressionSyntax { LiteralToken.Value: not null } n)
            return n.LiteralToken.Value;

        if (node is UnaryExpressionSyntax u)
        {
            var operand = EvaluateExpression(u.Operand);
            if(u.OperatorToken.Kind == SyntaxKind.Plus)
                return operand; 
            if(u.OperatorToken.Kind == SyntaxKind.Minus)
                return -operand;
            new LogDefinition(LogLevel.Error, $"不合理的一元运算符 <{u.OperatorToken.Kind}>", true).Raise();
            return 0;
        }
        
        if (node is BinaryExpressionSyntax b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            switch (b.OperatorToken.Kind)
            {
                case SyntaxKind.Plus:
                    return left + right;
                case SyntaxKind.Minus:
                    return left - right;
                case SyntaxKind.Star:
                    return left * right;
                case SyntaxKind.Slash:
                    return left / right;
                default:
                    new LogDefinition(LogLevel.Error, $"非预期运算符 <{b.OperatorToken.Kind}>", true).Raise();
                    return 0;
            }
        }

        if (node is ParenthesizedExpressionSyntax p)
            return EvaluateExpression(p.Expression);
        new LogDefinition(LogLevel.Error, $"无法解析的节点 <{node.Kind}>", true).Raise();
        return 0;
    }
}