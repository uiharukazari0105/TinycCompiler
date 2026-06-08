using Compiler.Output;
using Compiler.Tokens.Syntax;

namespace Compiler;

public class Lexer
{
    public List<LogDefinition> Diagnostics { get; } = [];
    private readonly string _text;
    private int _position;
    
    public Lexer(string text)
    {
        _text = text;
    }
    
    private char Current => _position >= _text.Length ? '\0' : _text[_position];
    private void Next() => _position++;
    
    public SyntaxToken NextToken()
    {
        if (Current == '\0') //处理结束
            return new SyntaxToken(SyntaxKind.EndOfFile, _position, "\0");
        
        if (char.IsDigit(Current)) //处理数字
        {
            var start = _position;
            while (char.IsDigit(Current) || Current == '_')
                Next();
            var length = _position - start;
            var text = _text.Substring(start, length);

            if (text[^1] == '_' || text.Contains("__"))
            {
                Diagnostics.Add(new LogDefinition(LogLevel.Error, $"不合理的数字格式标识 <{text}>", true));
                return new SyntaxToken(SyntaxKind.Bad, start, text);
            }
            
            int.TryParse(text.Replace("_",""), out var number);
            
            return new SyntaxToken(SyntaxKind.Number, start, text, number);
        }

        if (char.IsWhiteSpace(Current)) //处理空格
        {
            var start = _position;
            
            while(char.IsWhiteSpace(Current))
                Next();
            var length = _position - start;
            var text = _text.Substring(start, length);
            return new SyntaxToken(SyntaxKind.WhiteSpace, start, text);
        }

        switch (Current) //处理运算符
        {
            case '+':
                return new SyntaxToken(SyntaxKind.Plus, _position++, "+");
            case '-':
                return new SyntaxToken(SyntaxKind.Minus, _position++, "-");
            case '*':
                return new SyntaxToken(SyntaxKind.Star, _position++, "*");
            case '/':
                return new SyntaxToken(SyntaxKind.Slash, _position++, "/");
            case '(':
                return new SyntaxToken(SyntaxKind.OpenParenthesis, _position++, "(");
            case ')':
                return new SyntaxToken(SyntaxKind.CloseParenthesis, _position++, ")");
            default:
                Diagnostics.Add(new LogDefinition(LogLevel.Error, $"非预期令牌 <{Current}>", true));
                return new SyntaxToken(SyntaxKind.Bad, _position++, _text[_position-1].ToString());
        }
    }
}