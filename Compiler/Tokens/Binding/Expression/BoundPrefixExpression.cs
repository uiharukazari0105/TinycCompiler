namespace Compiler.Tokens.Binding.Expression;

public sealed class BoundPrefixExpression : BoundExpression
{
    public override BoundNodeKind Kind => BoundNodeKind.PrefixExpression;
    public override Type Type => Variable.Type;
    public VariableSymbol Variable { get; }
    public BoundUnaryOperatorKind OperatorKind { get; }

    public BoundPrefixExpression(VariableSymbol variable, BoundUnaryOperatorKind operatorKind)
    {
        Variable = variable;
        OperatorKind = operatorKind;
    }

    public override IEnumerable<BoundNode> GetChildren()
    {
        yield break;
    }
}
