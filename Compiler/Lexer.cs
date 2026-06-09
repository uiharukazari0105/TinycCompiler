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

    private char Current => Peek();
    private char AHead => Peek(1);
    
    private void Next() => _position++;

    private char Peek(int offset = 0)
    {
        var index = _position + offset;
        if (index >= _text.Length)
            return '\0';
        return _text[index];
    }
    
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

        if (char.IsLetter(Current) || Current == '_') //处理布尔和关键字
        {
            var start = _position;
            while (char.IsLetter(Current) || Current == '_' || char.IsDigit(Current))
                Next();
            var length = _position - start;
            var text = _text.Substring(start, length);
            var kind = SyntaxFact.GetKeywordKind(text);
            return new SyntaxToken(kind, start, text);
        }

        switch (Current) //处理运算符
        {
            case '+':
                if(AHead == '+')
                    return new SyntaxToken(SyntaxKind.DoublePlus, _position+=2, "++");
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
            case '!':
                if (AHead == '=')
                    return new SyntaxToken(SyntaxKind.ExclamationEquals, _position += 2, "!=");
                return new SyntaxToken(SyntaxKind.Exclamation, _position++, "!");
            case '&':
                if (AHead == '&')
                    return new SyntaxToken(SyntaxKind.DoubleAmpersand, _position += 2, "&&");
                return new SyntaxToken(SyntaxKind.Ampersand, _position++, "&");
            case '|':
                if (AHead == '|')
                    return new SyntaxToken(SyntaxKind.DoublePipe, _position += 2, "||");
                return new SyntaxToken(SyntaxKind.Pipe, _position++, "|");
            case '=':
                if(AHead == '=')
                    return new SyntaxToken(SyntaxKind.DoubleEquals, _position += 2, "==");
                return new SyntaxToken(SyntaxKind.Equals, _position++, "=");
            default:
                Diagnostics.Add(new LogDefinition(LogLevel.Error, $"非预期令牌 <{Current}>", true));
                return new SyntaxToken(SyntaxKind.Bad, _position++, _text[_position-1].ToString());
        }
    }
}