namespace Compiler.Tokens.Binding.Expression;

public class BoundAssignmentExpression: BoundExpression
{
    public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;
    public override Type Type => Expression.Type;
    public BoundExpression Expression { get; }
    public VariableSymbol Variable { get; }

    public BoundAssignmentExpression(VariableSymbol variable, BoundExpression expression)
    {
        Variable = variable;
        Expression = expression;
    }
}