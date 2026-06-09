using Compiler.Output;
using Compiler.Tokens.Syntax;
using Compiler.Tokens.Syntax.Expression;

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
        var expression = ParseExpression();
        var endOfFileToken = Match(SyntaxKind.EndOfFile);
        return new SyntaxTree(expression, endOfFileToken);
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
        switch (Current.Kind)
        {
            case SyntaxKind.OpenParenthesis:
                var left = NextToken();
                var expression = ParseExpression();
                var right = Match(SyntaxKind.CloseParenthesis);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            case SyntaxKind.TrueKeyword or SyntaxKind.FalseKeyword:
                var keywordToken = NextToken();
                var value = keywordToken.Kind == SyntaxKind.TrueKeyword;
                return new LiteralExpressionSyntax(keywordToken, value);
            case SyntaxKind.Identifier:
                var identifierToken = NextToken();
                return new NameExpressionSyntax(identifierToken);
            default:
            {
                var numberToken = Match(SyntaxKind.Number);
                return new LiteralExpressionSyntax(numberToken);
            }
        }
    }

    private ExpressionSyntax ParseExpression()
    {
        return ParseAssignmentExpression();
    }
    
    private ExpressionSyntax ParseAssignmentExpression()
    {
        if (Current.Kind == SyntaxKind.Identifier && Peek(1).Kind == SyntaxKind.Equals)
        {
            var identifierToken = NextToken();
            var operatorToken = NextToken();
            var right = ParseAssignmentExpression();
            return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
        }

        return ParseBinaryExpression();
    }
    
    private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
    {
        ExpressionSyntax left;
        var unaryOperatorPrecedence = Current.Kind.GetUnaryPrecedence();
        if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence) //特别应对--1的情况（特别难找）
        {
            var operatorToken = NextToken();
            var operand = ParseBinaryExpression(unaryOperatorPrecedence);
            left = new UnaryExpressionSyntax(operatorToken, operand);
        }
        else
        {
            left = ParsePrimaryExpression();
        }
        
        while (true)
        {
            var precedence = Current.Kind.GetBinaryPrecedence();
            if(precedence == 0 || precedence <= parentPrecedence)
                break;
            var operatorToken = NextToken();
            var right = ParseBinaryExpression(precedence);
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }
        
        return left;
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