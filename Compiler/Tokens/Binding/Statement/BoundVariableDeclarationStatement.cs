using Compiler.Tokens.Binding.Expression;

namespace Compiler.Tokens.Binding.Statement;

public class BoundVariableDeclarationStatement: BoundStatement
{
    public override BoundNodeKind Kind => BoundNodeKind.VariableDeclarationStatement;
    public VariableSymbol Variable { get; }
    public BoundExpression Initializer { get; }

    public BoundVariableDeclarationStatement(VariableSymbol variable, BoundExpression initializer)
    {
        Variable = variable;
        Initializer = initializer;
    }

    public override IEnumerable<BoundNode> GetChildren()
    {
        yield return Initializer;
    }
}