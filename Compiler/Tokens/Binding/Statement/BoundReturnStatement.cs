using Compiler.Tokens.Binding.Expression;

namespace Compiler.Tokens.Binding.Statement;

public sealed class BoundReturnStatement : BoundStatement
{
    public override BoundNodeKind Kind => BoundNodeKind.ReturnStatement;
    public BoundExpression? Expression { get; }

    public BoundReturnStatement(BoundExpression? expression)
    {
        Expression = expression;
    }

    public override IEnumerable<BoundNode> GetChildren()
    {
        if (Expression is not null)
            yield return Expression;
    }
}
