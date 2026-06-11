using Compiler.Tokens.Binding.Expression;

namespace Compiler.Tokens.Binding.Statement;

public class BoundForStatement: BoundStatement
{
    public override BoundNodeKind Kind => BoundNodeKind.ForStatement;

    public List<BoundStatement> Initializers { get; }
    public BoundExpression? Condition { get; }
    public BoundExpression? StepExpression { get; }
    public BoundStatement Statement { get; }

    public BoundForStatement(List<BoundStatement> initializers, BoundExpression? condition,
        BoundExpression? stepExpression, BoundStatement statement)
    {
        Initializers = initializers;
        Condition = condition;
        StepExpression = stepExpression;
        Statement = statement;
    }

    public override IEnumerable<BoundNode> GetChildren()
    {
        foreach (var initializer in Initializers)
            yield return initializer;
        if (Condition is not null)
            yield return Condition;
        if (StepExpression is not null)
            yield return StepExpression;
        yield return Statement;
    }
}