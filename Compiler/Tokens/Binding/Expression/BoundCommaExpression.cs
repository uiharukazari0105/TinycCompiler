namespace Compiler.Tokens.Binding.Expression;

public sealed class BoundCommaExpression : BoundExpression
{
    public override BoundNodeKind Kind => BoundNodeKind.CommaExpression;
    public override Type Type => Right.Type;
    public BoundExpression Left { get; }
    public BoundExpression Right { get; }

    public BoundCommaExpression(BoundExpression left, BoundExpression right)
    {
        Left = left;
        Right = right;
    }

    public override IEnumerable<BoundNode> GetChildren()
    {
        yield return Left;
        yield return Right;
    }
}
