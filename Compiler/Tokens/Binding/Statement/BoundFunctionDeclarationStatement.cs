using Compiler.Tokens.Binding.Expression;

namespace Compiler.Tokens.Binding.Statement;

public sealed class BoundFunctionDeclarationStatement : BoundStatement
{
    public override BoundNodeKind Kind => BoundNodeKind.FunctionDeclarationStatement;
    public BoundStatement Body { get; }

    public BoundFunctionDeclarationStatement(BoundStatement body)
    {
        Body = body;
    }

    public override IEnumerable<BoundNode> GetChildren()
    {
        yield return Body;
    }
}
