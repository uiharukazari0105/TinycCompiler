namespace Compiler.Tokens.Binding.Expression;

public abstract class BoundNode
{
    public abstract BoundNodeKind Kind { get; }
}