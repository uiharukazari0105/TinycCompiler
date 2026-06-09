using Compiler.Output;

namespace Compiler.Evaluation;

public class EvaluationResult
{
    public List<LogDefinition> Diagnostics { get; }
    public dynamic Value { get; }

    public EvaluationResult(List<LogDefinition> diagnostics, dynamic value)
    {
        Diagnostics = diagnostics;
        Value = value;
       
        
    }
}