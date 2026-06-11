using System.Text;

namespace Compiler;

public class Preprocessor
{
    private string _text;
    private readonly StringBuilder _resultBuilder = new();
    private int _position;

    public Preprocessor(string text)
    {
        _text = text;
    }

    public string Process()
    {
        // FirstCleanup();
        while (!Eof)
            NextChar();
        return _resultBuilder.ToString();
    }

    private char Current => Peek();
    private char AHead => Peek(1);

    private bool Eof => Current == '\0';

    private void Next() => _position++;

    private char Peek(int offset = 0)
    {
        var index = _position + offset;
        if (index >= _text.Length)
            return '\0';
        return _text[index];
    }

    private void FirstCleanup()
    {
        var lines = _text.Split('\n').ToList();
        for (int i = lines.Count - 1; i >= 0; i--)
        {
            lines[i] = lines[i].Trim();
            if (lines[i].Length == 0)
                lines.RemoveAt(i);
        }

        _text = string.Join('\n', lines);
    }

    private void NextChar()
    {
        if (Current == '/' && AHead == '/')
        {
            _position += 2;
            while (Current != '\n' && !Eof)
                Next();
        }

        else if (Current == '/' && AHead == '*')
        {
            _position += 2;
            while (!(Current == '*' && AHead == '/') && !Eof)
                Next();
            _position += 2;
        }
        else if (Current == '"')
        {
            _resultBuilder.Append(Current);
            Next();
            while (Current != '"' && !Eof)
            {
                if (Current == '\\' && !Eof)
                {
                    _resultBuilder.Append(Current);
                    Next();
                }
                if (!Eof)
                {
                    _resultBuilder.Append(Current);
                    Next();
                }
            }
            if (!Eof)
            {
                _resultBuilder.Append(Current);
                Next();
            }
        }
        else if (Current == '\'')
        {
            _resultBuilder.Append(Current);
            Next();
            while (Current != '\'' && !Eof)
            {
                if (Current == '\\' && !Eof)
                {
                    _resultBuilder.Append(Current);
                    Next();
                }
                if (!Eof)
                {
                    _resultBuilder.Append(Current);
                    Next();
                }
            }
            if (!Eof)
            {
                _resultBuilder.Append(Current);
                Next();
            }
        }
        else
        {
            _resultBuilder.Append(Current);
            Next();
        }
    }
}
