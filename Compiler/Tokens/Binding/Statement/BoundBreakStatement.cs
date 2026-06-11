using Compiler.Tokens.Binding.Expression;

namespace Compiler.Tokens.Binding.Statement;

public sealed class BoundBreakStatement : BoundStatement
{
    public override BoundNodeKind Kind => BoundNodeKind.BreakStatement;

    public override IEnumerable<BoundNode> GetChildren()
    {
        yield break;
    }
}
