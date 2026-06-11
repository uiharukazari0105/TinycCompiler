using Compiler.Tokens.Binding.Expression;

namespace Compiler.Tokens.Binding.Statement;

public sealed class BoundWhileStatement: BoundStatement
{
    public override BoundNodeKind Kind => BoundNodeKind.WhileStatement;
    public BoundExpression Condition { get; }
    public BoundStatement Statement { get; }

    public BoundWhileStatement(BoundExpression condition, BoundStatement statement)
    {
        Condition = condition;
        Statement = statement;
    }

    public override IEnumerable<BoundNode> GetChildren()
    {
        yield return Condition;
    }
}