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
        
        if (char.IsDigit(Current) || Current == '.') //处理数字
        {
            var start = _position;
            while (char.IsDigit(Current) || Current == '_' || Current == '.' || Current == 'f' || Current == 'L')
                Next();
            var length = _position - start;
            var text = _text.Substring(start, length);

            if (text[^1] == '_' || text.Contains("__") || text.Count('.') > 1 || text == "." || text.Count('f') > 1 || text.Count('L') > 1)
            {
                Diagnostics.Add(new LogDefinition(LogLevel.Error, $"不合理的数字格式标识 <{text}>", true));
                return new SyntaxToken(SyntaxKind.Bad, start, text);
            }
            
            return new SyntaxToken(SyntaxKind.Number, start, text.Replace("_",""));
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

        if (char.IsLetter(Current) || Current == '_' || Current == '$') //处理布尔和关键字
        {
            var start = _position;
            while (char.IsLetter(Current) || Current == '_' || Current == '$' || char.IsDigit(Current))
                Next();
            var length = _position - start;
            var text = _text.Substring(start, length);
            var kind = SyntaxFact.GetKeywordKind(text);
            return new SyntaxToken(kind, start, text);
        }

        if (Current == '\'') //处理字符
        {
            var start = _position;
            Next();
            while (Current != '\'' && _position < _text.Length)
            {
                if (Current >= 256)
                {
                    Diagnostics.Add(new LogDefinition(LogLevel.Error, $"不合理的字符 <{Current}>", true));
                    return new SyntaxToken(SyntaxKind.Bad, start, Current.ToString());
                }
                Next();
            }
                    
            if(Current == '\'')
                Next();
            else
            {
                Diagnostics.Add(new LogDefinition(LogLevel.Error, "单引号未闭合", true));
                return new SyntaxToken(SyntaxKind.Bad, start, Current.ToString());
            }
            
            var length = _position - start;
            var text = _text.Substring(start, length);
            
            if(text.Length == 2)
            {
                Diagnostics.Add(new LogDefinition(LogLevel.Error, "字符表达式为空", true));
                return new SyntaxToken(SyntaxKind.Bad, start, Current.ToString());
            }
            return new SyntaxToken(SyntaxKind.Character, start, text);
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
            case '{':
                return new SyntaxToken(SyntaxKind.OpenBrace, _position++, "{");
            case '}':
                return new SyntaxToken(SyntaxKind.CloseBrace, _position++, "}");
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
            case '<':
                if (AHead == '=')
                    return new SyntaxToken(SyntaxKind.LessOrEquals, _position += 2, "<=");
                return new SyntaxToken(SyntaxKind.Less, _position++, "<");
            case '>':
                if (AHead == '=')
                    return new SyntaxToken(SyntaxKind.GreaterOrEquals, _position += 2, ">=");
                return new SyntaxToken(SyntaxKind.Greater, _position++, ">");
            case ';':
                return new SyntaxToken(SyntaxKind.Semicolon, _position++, ";");
            case ',':
                return new SyntaxToken(SyntaxKind.Comma, _position++, ",");
            case '^':
                return new SyntaxToken(SyntaxKind.Caret, _position++, "^");
            case '~':
                return new SyntaxToken(SyntaxKind.Tilde, _position++, "~");
            default:
                Diagnostics.Add(new LogDefinition(LogLevel.Error, $"非预期令牌 <{Current}>", true));
                return new SyntaxToken(SyntaxKind.Bad, _position++, _text[_position-1].ToString());
        }
    }
}