namespace Compiler.Tokens.Binding.Expression;

public sealed class BoundPostfixExpression : BoundExpression
{
    public override BoundNodeKind Kind => BoundNodeKind.PostfixExpression;
    public override Type Type => Variable.Type;
    public VariableSymbol Variable { get; }
    public BoundUnaryOperatorKind OperatorKind { get; }

    public BoundPostfixExpression(VariableSymbol variable, BoundUnaryOperatorKind operatorKind)
    {
        Variable = variable;
        OperatorKind = operatorKind;
    }

    public override IEnumerable<BoundNode> GetChildren()
    {
        yield break;
    }
}
