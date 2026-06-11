using Compiler.Tokens.Binding.Expression;

namespace Compiler.Tokens.Binding.Statement;

public sealed class BoundContinueStatement : BoundStatement
{
    public override BoundNodeKind Kind => BoundNodeKind.ContinueStatement;

    public override IEnumerable<BoundNode> GetChildren()
    {
        yield break;
    }
}
