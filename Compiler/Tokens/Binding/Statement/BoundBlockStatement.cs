using System.Collections.Immutable;
using Compiler.Tokens.Binding.Expression;

namespace Compiler.Tokens.Binding.Statement;

public sealed class BoundBlockStatement: BoundStatement
{
    public override BoundNodeKind Kind => BoundNodeKind.BlockStatement;
    public ImmutableArray<BoundStatement> Statements { get; }

    public BoundBlockStatement(ImmutableArray<BoundStatement> statements)
    {
        Statements = statements;
    }
}