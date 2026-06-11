namespace Compiler.Tokens.Binding.Expression;

public sealed class BoundLiteralExpression: BoundExpression
{
    public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
    public override Type Type => Value.GetType();
    public dynamic Value { get; }

    public BoundLiteralExpression(dynamic value)
    {
        Value = value;
    }

    public override IEnumerable<BoundNode> GetChildren()
    {
        return [];
    }
}