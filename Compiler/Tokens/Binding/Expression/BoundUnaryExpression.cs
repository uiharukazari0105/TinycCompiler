using Compiler.Tokens.Binding.Operator;

namespace Compiler.Tokens.Binding.Expression;

public sealed class BoundUnaryExpression: BoundExpression
{
    
    public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
    public override Type Type => Operator.ResultType;
    public BoundUnaryOperator Operator { get; }
    public BoundExpression Operand { get; }

    public BoundUnaryExpression(BoundUnaryOperator boundUnaryOperator, BoundExpression operand)
    {
        Operator = boundUnaryOperator;
        Operand = operand;
    }

    public override IEnumerable<BoundNode> GetChildren()
    {
        yield return Operand;
    }
}