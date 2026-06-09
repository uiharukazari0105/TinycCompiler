using Compiler.Tokens.Binding.Operator;

namespace Compiler.Tokens.Binding.Expression;

public class BoundBinaryExpression: BoundExpression
{
    public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;
    public override Type Type => Operator.ResultType;
    public BoundExpression Left { get; }
    public BoundBinaryOperator Operator { get; }
    public BoundExpression Right { get; }

    public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator boundBinaryOperator, BoundExpression right)
    {
        Left = left;
        Operator = boundBinaryOperator;
        Right = right;
    }
}