using Compiler.Tokens.Binding.Operator;

namespace Compiler.Tokens.Binding.Expression;

public class BoundSelfOperatorExpression: BoundExpression
{
    public override BoundNodeKind Kind => BoundNodeKind.SelfOperatorExpression;
    public override Type Type => Expression.Type;
    
    public BoundBinaryOperator Operator { get; }
    public BoundExpression Expression { get; }
    public VariableSymbol Variable { get; }

    public BoundSelfOperatorExpression(VariableSymbol variable, BoundBinaryOperator boundBinaryOperator, BoundExpression expression)
    {
        Variable = variable;
        Operator = boundBinaryOperator;
        Expression = expression;
    }

    public override IEnumerable<BoundNode> GetChildren()
    {
        yield return Expression;
    }
}