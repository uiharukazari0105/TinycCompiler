using System.Collections.Immutable;
using Compiler.Output;
using Compiler.Tokens.Binding.Expression;

namespace Compiler.Tokens.Binding;

public class BoundGlobalScope
{
    public BoundGlobalScope? Previous { get; }
    public List<LogDefinition> Diagnostics { get; }
    public ImmutableArray<VariableSymbol> Variables { get; }
    public BoundExpression Expression { get; }

    public BoundGlobalScope(BoundGlobalScope? previous, List<LogDefinition> diagnostics,
        ImmutableArray<VariableSymbol> variables, BoundExpression expression)
    {
        Previous = previous;
        Diagnostics = diagnostics;
        Variables = variables;
        Expression = expression;
    }
}