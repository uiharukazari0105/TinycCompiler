namespace Compiler.Tokens.Binding.Expression;

public sealed class BoundVariableExpression: BoundExpression
{
    public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
    public override Type Type => Variable.Type;
    public VariableSymbol Variable { get; }   

    public BoundVariableExpression(VariableSymbol variable)
    {
        Variable = variable;
    }
}