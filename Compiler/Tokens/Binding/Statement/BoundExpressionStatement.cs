using Compiler.Tokens.Binding.Expression;

namespace Compiler.Tokens.Binding.Statement;

public class BoundExpressionStatement: BoundStatement
{
    public override BoundNodeKind Kind => BoundNodeKind.ExpressionStatement;
    public BoundExpression Expression { get; }

    public BoundExpressionStatement(BoundExpression expression)
    {
        Expression = expression;
    }
}