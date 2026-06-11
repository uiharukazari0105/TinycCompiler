using System.Text;
using Compiler.Tokens.Binding.Expression;
using Compiler.Tokens.Binding.Statement;
using Compiler.Tokens.Syntax;
using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Output;

public static class PrettyPrint
{
    public static StringBuilder Out(SyntaxNode node, string indent = "", bool isLast = true)
    {
        StringBuilder builder = new();

        var marker = isLast ? "└──" : "├──";
        builder.Append(indent).Append(marker);

        if (node is SyntaxToken { Value: not null } token)
            builder.Append(token.Kind).Append(' ').Append(token.Text);
        else
            builder.Append(node.Kind);

        builder.AppendLine();

        indent += isLast ? "    " : "│   ";

        var lastChild = node.GetChildren().LastOrDefault();
        foreach (var child in node.GetChildren())
            builder.Append(Out(child, indent, child == lastChild).ToString());
        return builder;
    }

    public static StringBuilder Out(BoundNode node, string indent = "", bool isLast = true)
    {
        StringBuilder builder = new();

        var marker = isLast ? "└──" : "├──";
        builder.Append(indent).Append(marker).Append(node.Kind).Append(' ');

        foreach (var property in node.GetProperties())
            builder.Append(property.Name).Append(':').Append(property.Value).Append(' ');

        builder.AppendLine();

        indent += isLast ? "    " : "│   ";

        var lastChild = node.GetChildren().LastOrDefault();
        foreach (var child in node.GetChildren())
            builder.Append(Out(child, indent, child == lastChild));
        return builder;
    }

    public static StringBuilder OutDoubleTree(SyntaxTree tree, BoundStatement boundStatement, int margin = 3,
        bool translate = false)
    {
        var abstractSyntaxTree = translate ? Translate(Out(tree.Root)) : Out(tree.Root).ToString();
        var boundTree = translate ? Translate(Out(boundStatement)) : Out(boundStatement).ToString();

        var astLines = abstractSyntaxTree.Split('\n');
        var btLines = boundTree.Split('\n');

        var astMaxWidth = 0;
        foreach (var line in astLines)
            astMaxWidth = Math.Max(astMaxWidth, DisplayWidth(line));

        StringBuilder builder = new();
        int curAstLine = 0, curBtLine = 0;
        while (curAstLine < astLines.Length || curBtLine < btLines.Length)
        {
            int curAstLineWidth = 0;
            if (curAstLine < astLines.Length)
            {
                var target = astLines[curAstLine];
                builder.Append(target);
                curAstLineWidth = DisplayWidth(target);
                curAstLine++;
            }

            if (curBtLine < btLines.Length)
            {
                var target = btLines[curBtLine];
                builder.Append(new String(' ', astMaxWidth - curAstLineWidth + margin)).Append(target);
                curBtLine++;
            }

            builder.AppendLine();
        }

        return builder;
    }

    public static string Translate(StringBuilder input)
    {
        var source = input.ToString();
        List<(string, string)> termTable =
        [
            ("CompilationUnit", "编译单元"),
            ("BlockStatement", "代码块"),
            ("ExpressionStatement", "表达式语句"),
            ("EmptyStatement", "空语句"),

            ("IfStatement", "if语句"),
            ("WhileStatement", "while循环"),
            ("ForStatement", "for循环"),

            ("VariableDeclarationStatement", "变量声明"),

            ("UnaryExpression", "一元运算表达式"),
            ("BinaryExpression", "二元运算表达式"),
            ("LiteralExpression", "字面量表达式"),
            ("VariableExpression", "变量引用表达式"),
            ("AssignmentExpression", "赋值表达式"),
            ("ParenthesizedExpression", "括号表达式"),
            ("NameExpression", "变量名"),

            ("Type", "类型"),
            ("Int32", "32位整型"),
            ("Boolean", "布尔型"),

            ("Value", "值"),
            ("Number", "数字"),
            ("WhiteSpace", "空白"),
            ("Plus", "加号"),
            ("DoublePlus", "自增"),
            ("Minus", "减号"),
            ("Star", "乘号"),
            ("Slash", "除号"),
            ("Exclamation", "感叹号"),
            ("Ampersand", "按位与"),
            ("DoubleAmpersand", "逻辑与"),
            ("Pipe", "按位或"),
            ("DoublePipe", "逻辑或"),
            ("OpenParenthesis", "左圆括号"),
            ("CloseParenthesis", "右圆括号"),
            ("OpenBrace", "左大括号"),
            ("CloseBrace", "右大括号"),
            ("Equals", "等号"),
            ("DoubleEquals", "双等号"),
            ("ExclamationEquals", "不等号"),
            ("LessOrEquals", "小于等于"),
            ("Less", "小于"),
            ("GreaterOrEquals", "大于等于"),
            ("Greater", "大于"),
            ("Semicolon", "分号"),
            ("Comma", "逗号"),
            ("Caret", "异或"),
            ("Tilde", "按位取反"),
            ("Bad", "错误"),
            ("EndOfFile", "文件结尾"),

            ("ShortKeyword", "短整型关键字"),
            ("IntKeyword", "整型关键字"),
            ("LongKeyword", "长整型关键字"),
            ("FloatKeyword", "单精度浮点型关键字"),
            ("DoubleKeyword", "双精度浮点型关键字"),
            ("TrueKeyword", "真关键字"),
            ("FalseKeyword", "假关键字"),
            ("Identifier", "标识符"),
            ("IfKeyWord", "如果关键字"),
            ("ElseKeyword", "否则关键字"),
            ("ForKeyword", "for循环关键字"),
            ("WhileKeyword", "while循环关键字"),

            ("True", "真"),
            ("False", "假"),
            ("Double", "双"),
        ];

        foreach (var term in termTable)
            source = source.Replace(term.Item1, term.Item2);
        return source;
    }

    private static int DisplayWidth(string text)
    {
        int width = 0;
        foreach (var c in text)
        {
            if (c <= 127) width += 1;
            else if (c is >= '\u2500' and <= '\u257F') width += 1;
            else width += 2;
        }

        return width;
    }
}