namespace Compiler;

public static class Preprocessor
{
    public static string Process(string source, bool removeNewlines = false)
    {
        var result = new System.Text.StringBuilder(source.Length);
        var i = 0;

        while (i < source.Length)
        {
            if (i + 1 < source.Length && source[i] == '/' && source[i + 1] == '/')
            {
                i += 2;
                while (i < source.Length && source[i] != '\n')
                    i++;
                continue;
            }

            if (i + 1 < source.Length && source[i] == '/' && source[i + 1] == '*')
            {
                i += 2;
                while (i + 1 < source.Length && !(source[i] == '*' && source[i + 1] == '/'))
                    i++;
                i += 2;
                continue;
            }

            switch (source[i])
            {
                case '"':
                {
                    result.Append(source[i++]);
                    while (i < source.Length && source[i] != '"')
                    {
                        if (source[i] == '\\' && i + 1 < source.Length)
                        {
                            result.Append(source[i++]);
                            result.Append(source[i++]);
                        }
                        else
                            result.Append(source[i++]);
                    }

                    if (i < source.Length)
                        result.Append(source[i++]);
                    continue;
                }
                case '\'':
                {
                    result.Append(source[i++]); //前
                    while (i < source.Length && source[i] != '\'')
                    {
                        if (source[i] == '\\' && i + 1 < source.Length)
                        {
                            result.Append(source[i++]);
                            result.Append(source[i++]);
                        }
                        else
                            result.Append(source[i++]);
                    }

                    if (i < source.Length)
                        result.Append(source[i++]); //后
                    continue;
                }
            }

            if (removeNewlines && source[i] == '\n' || source[i] == '\r')
            {
                i++;
                continue;
            }

            result.Append(source[i++]);
        }

        return result.ToString();
    }
}