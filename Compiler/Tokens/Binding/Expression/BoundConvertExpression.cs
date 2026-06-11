namespace Compiler.Tokens.Binding.Expression;

public sealed class BoundConversionExpression : BoundExpression
{
    public override BoundNodeKind Kind => BoundNodeKind.ConversionExpression;
    public override Type Type { get; }
    public BoundExpression Expression { get; }

    public BoundConversionExpression(Type targetType, BoundExpression expression)
    {
        Type = targetType;
        Expression = expression;
    }
    
    public override IEnumerable<BoundNode> GetChildren()
    {
        yield return Expression;
    }
}