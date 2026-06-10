using System.Collections.Immutable;
using Compiler.Output;
using Compiler.Tokens.Binding.Statement;

namespace Compiler.Tokens.Binding;

public class BoundGlobalScope
{
    public BoundGlobalScope? Previous { get; }
    public List<LogDefinition> Diagnostics { get; }
    public ImmutableArray<VariableSymbol> Variables { get; }
    public BoundStatement Statement { get; }

    public BoundGlobalScope(BoundGlobalScope? previous, List<LogDefinition> diagnostics,
        ImmutableArray<VariableSymbol> variables, BoundStatement statement)
    {
        Previous = previous;
        Diagnostics = diagnostics;
        Variables = variables;
        Statement = statement;
    }
}