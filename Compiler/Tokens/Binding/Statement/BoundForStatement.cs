using Compiler.Tokens.Binding.Expression;

namespace Compiler.Tokens.Binding.Statement;

public class BoundForStatement: BoundStatement
{
    public override BoundNodeKind Kind => BoundNodeKind.ForStatement;
    
    public List<BoundStatement> Initializers { get; }
    public BoundExpression? Condition { get; }
    public List<BoundStatement> StepStatements { get; }
    public BoundStatement Statement { get; }

    public BoundForStatement(List<BoundStatement> initializers, BoundExpression? condition,
        List<BoundStatement> stepStatements, BoundStatement statement)
    {
        Initializers = initializers;
        Condition = condition;
        StepStatements = stepStatements;
        Statement = statement;
    }
}