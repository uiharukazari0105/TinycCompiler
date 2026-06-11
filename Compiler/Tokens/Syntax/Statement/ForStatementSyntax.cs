using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Tokens.Syntax.Statement;

public class ForStatementSyntax: StatementSyntax
{
    public SyntaxToken ForKeyword { get; }
    public SyntaxToken OpenParenthesisToken { get; }
    public List<StatementSyntax> Initializers { get; }
    public ExpressionSyntax? Condition { get; }
    public ExpressionSyntax? StepExpression { get; }
    public SyntaxToken CloseParenthesisToken { get; }
    public StatementSyntax ThenStatement { get; }
    public override SyntaxKind Kind => SyntaxKind.ForStatement;

    public ForStatementSyntax(SyntaxToken forKeyword,
        SyntaxToken openParenthesisToken,
        List<StatementSyntax> initializers,
        ExpressionSyntax? condition,
        ExpressionSyntax? stepExpression,
        SyntaxToken closeParenthesisToken,
        StatementSyntax thenStatement)
    {
        ForKeyword = forKeyword;
        OpenParenthesisToken = openParenthesisToken;
        Initializers = initializers;
        Condition = condition;
        StepExpression = stepExpression;
        CloseParenthesisToken = closeParenthesisToken;
        ThenStatement = thenStatement;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return ForKeyword;
        yield return OpenParenthesisToken;
        foreach(var initializer in Initializers)
            yield return initializer;
        if(Condition is not null)
            yield return Condition;
        if(StepExpression is not null)
            yield return StepExpression;
        yield return CloseParenthesisToken;
        yield return ThenStatement;
    }
}