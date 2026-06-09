namespace Compiler.Tokens.Binding.Expression;

public abstract class BoundExpression: BoundNode
{
    public abstract Type Type { get; }
}