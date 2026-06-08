using Compiler.Output;
using Compiler.Tokens.Syntax;

namespace Compiler;

public class Parser
{
    public List<LogDefinition> Diagnostics { get; } = [];
    private readonly SyntaxToken[] _tokens;
    private int _position;
    
    public Parser(string text)
    {
        var tokens = new List<SyntaxToken>();
        var lexer = new Lexer(text);
        List<SyntaxKind> filter = [SyntaxKind.WhiteSpace, SyntaxKind.Bad]; 
        SyntaxToken token;
        do
        {
            token = lexer.NextToken();
            if (filter.Contains(token.Kind)) continue;
            tokens.Add(token);
        } while (token.Kind != SyntaxKind.EndOfFile);
        
        _tokens = tokens.ToArray(); 
        Diagnostics.AddRange(lexer.Diagnostics);
    }

    public SyntaxTree Parse()
    {
        var expression = ParseTerm();
        var endOfFileToken = Match(SyntaxKind.EndOfFile);
        return new SyntaxTree(expression, endOfFileToken);
    }

    public ExpressionSyntax ParseTerm()
    {
        var left = ParseFactor();
        
        List<SyntaxKind> whiteList = [SyntaxKind.Plus, SyntaxKind.Minus];
        while (whiteList.Contains(Current.Kind))
        {
            var operatorToken = NextToken();
            var right = ParseFactor();
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }
        return left;
    }

    public ExpressionSyntax ParseFactor()
    {
        var left = ParsePrimaryExpression();
        
        List<SyntaxKind> whiteList = [SyntaxKind.Star, SyntaxKind.Slash];
        while (whiteList.Contains(Current.Kind))
        {
            var operatorToken = NextToken();
            var right = ParsePrimaryExpression();
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }
        return left;
    }

    private SyntaxToken Match(SyntaxKind kind)
    {
        if(Current.Kind == kind)
            return NextToken();
        Diagnostics.Add(new LogDefinition(LogLevel.Error,$"不合理的类型 <{Current.Kind}>，应该为 <{kind}>", true));
        return new SyntaxToken(kind, Current.Position, "");
    }
    
    private ExpressionSyntax ParsePrimaryExpression()
    {
        if (Current.Kind == SyntaxKind.OpenParenthesis)
        {
            var left = NextToken();
            var expression = ParseTerm();
            var right = Match(SyntaxKind.CloseParenthesis);
            return new ParenthesizedExpressionSyntax(left, expression, right);
        }
        var numberToken = Match(SyntaxKind.Number);
        return new NumberExpressionSyntax(numberToken);
    }

    private SyntaxToken NextToken()
    {
        var current = Current;
        _position++;
        return current;
    }
    
    private SyntaxToken Peek(int offset)
    {
        var index = _position + offset;
        if (index >= _tokens.Length) return _tokens[^1];
        return _tokens[index];
    }
    
    private SyntaxToken Current=>Peek(0);
}