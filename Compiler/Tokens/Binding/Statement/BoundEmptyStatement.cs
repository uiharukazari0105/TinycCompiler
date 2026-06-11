using Compiler.Tokens.Binding.Expression;

namespace Compiler.Tokens.Binding.Statement;

public class BoundEmptyStatement: BoundStatement
{
    public override BoundNodeKind Kind => BoundNodeKind.EmptyStatement;

    public override IEnumerable<BoundNode> GetChildren()
    {
        return [];
    }
}