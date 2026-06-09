using Compiler.Tokens.Syntax;
using Compiler.Tokens.Syntax.Expression;

namespace Compiler.Output;

public static class PrettyPrint
{
    public static void Out(SyntaxNode node, string indent = "", bool isLast = true)
    {
        var marker = isLast ? "└──" : "├──";
        Console.Write(indent);
        Console.Write(marker);
        Console.Write(node.Kind);

        if (node is SyntaxToken { Value: not null } token)
        {
            Console.Write(" ");
            Console.Write(token.Text);
        }
        
        Console.WriteLine();
        
        indent += isLast ? "    ": "│   ";

        var lastChild = node.GetChildren().LastOrDefault();
        foreach (var child in node.GetChildren())
            Out(child, indent, child == lastChild);
    }
}