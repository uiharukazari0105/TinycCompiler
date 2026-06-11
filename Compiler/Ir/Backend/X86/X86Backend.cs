using System.Text;
using Compiler.Ir.Backend.Tools;
using Compiler.Ir.Instruction;

namespace Compiler.Ir.Backend.X86;

public class X86Backend : Backend
{
    public X86Backend(IReadOnlyList<IrInstruction> ir) : base(ir)
    {
    }

    public override string Generate()
    {
        var variables = new HashSet<string>();
        foreach (var inst in Ir)
        {
            switch (inst)
            {
                case AssignIr a:
                    variables.Add(a.Result);
                    if (!IsLiteral(a.Value)) variables.Add(a.Value);
                    break;
                case BinaryIr b:
                    variables.Add(b.Result);
                    if (!IsLiteral(b.Left)) variables.Add(b.Left);
                    if (!IsLiteral(b.Right)) variables.Add(b.Right);
                    break;
                case ReturnIr r:
                    if (r.Value is not null && !IsLiteral(r.Value))
                        variables.Add(r.Value);
                    break;
                case CondJumpIr c:
                    if (!IsLiteral(c.Condition)) variables.Add(c.Condition);
                    break;
            }
        }

        var builder = new StringBuilder();

        //数据区
        builder.AppendLine("section .data");
        foreach (var v in variables.OrderBy(x => x))
            builder.AppendLineWithIndent($"{v} dd 0");
        builder.AppendLine();

        //静态区
        builder.AppendLine("section .text");
        builder.AppendLine("main:");

        foreach (var inst in Ir)
            Emit(builder, inst);

        if (Ir.Count == 0 || Ir[^1] is not ReturnIr)
            builder.AppendLineWithIndent("ret");

        return builder.ToString();
    }

    private static void Emit(StringBuilder builder, IrInstruction inst)
    {
        switch (inst)
        {
            case LabelIr l:
                builder.AppendLine($"L{l.Id}:");
                break;

            case AssignIr a:
                if (IsLiteral(a.Value))
                {
                    builder.AppendLineWithIndent($"mov eax, {a.Value}");
                    builder.AppendLineWithIndent($"mov [{a.Result}], eax");
                }
                else
                {
                    builder.AppendLineWithIndent($"mov eax, [{a.Value}]");
                    builder.AppendLineWithIndent($"mov [{a.Result}], eax");
                }
                break;

            case BinaryIr b:
                if (string.IsNullOrEmpty(b.Left))
                {
                    LoadOperand(builder, "eax", b.Right);
                    switch (b.Operator)
                    {
                        case "-": builder.AppendLineWithIndent("neg eax"); break;
                        case "!":
                            builder.AppendLineWithIndent("cmp eax, 0");
                            builder.AppendLineWithIndent("sete al");
                            builder.AppendLineWithIndent("movzx eax, al");
                            break;
                        case "~": builder.AppendLineWithIndent("not eax"); break;
                    }
                    builder.AppendLineWithIndent($"mov [{b.Result}], eax");
                }
                else if (b.Operator is "==" or "!=" or "<" or "<=" or ">" or ">=")
                {
                    LoadOperand(builder, "eax", b.Left);
                    if (IsLiteral(b.Right))
                        builder.AppendLineWithIndent($"cmp eax, {b.Right}");
                    else
                    {
                        LoadOperand(builder, "ebx", b.Right);
                        builder.AppendLineWithIndent("cmp eax, ebx");
                    }
                    builder.AppendLineWithIndent(b.Operator switch
                    {
                        "==" => "sete al",
                        "!=" => "setne al",
                        "<" => "setl al",
                        "<=" => "setle al",
                        ">" => "setg al",
                        ">=" => "setge al",
                        _ => "; unknown cmp"
                    });
                    builder.AppendLineWithIndent("movzx eax, al");
                    builder.AppendLineWithIndent($"mov [{b.Result}], eax");
                }
                else if (b.Operator is "&&" or "||")
                {
                    LoadOperand(builder, "eax", b.Left);
                    builder.AppendLineWithIndent("cmp eax, 0");
                    builder.AppendLineWithIndent("setne al");
                    builder.AppendLineWithIndent("movzx eax, al");
                    LoadOperand(builder, "ebx", b.Right);
                    builder.AppendLineWithIndent("cmp ebx, 0");
                    builder.AppendLineWithIndent("setne bl");
                    builder.AppendLineWithIndent("movzx ebx, bl");
                    builder.AppendLineWithIndent(b.Operator == "&&" ? "and eax, ebx" : "or eax, ebx");
                    builder.AppendLineWithIndent($"mov [{b.Result}], eax");
                }
                else
                {
                    LoadOperand(builder, "eax", b.Left);
                    if (b.Operator is "/" or "%")
                    {
                        LoadOperand(builder, "ebx", b.Right);
                        builder.AppendLineWithIndent("cdq");
                        builder.AppendLineWithIndent("idiv ebx");
                        if (b.Operator == "%")
                            builder.AppendLineWithIndent("mov eax, edx");
                    }
                    else if (IsLiteral(b.Right))
                    {
                        builder.AppendLineWithIndent(b.Operator switch
                        {
                            "+" => $"add eax, {b.Right}",
                            "-" => $"sub eax, {b.Right}",
                            "*" => $"imul eax, {b.Right}",
                            "&" => $"and eax, {b.Right}",
                            "|" => $"or eax, {b.Right}",
                            "^" => $"xor eax, {b.Right}",
                            _ => "; unknown op"
                        });
                    }
                    else
                    {
                        LoadOperand(builder, "ebx", b.Right);
                        builder.AppendLineWithIndent(b.Operator switch
                        {
                            "+" => "add eax, ebx",
                            "-" => "sub eax, ebx",
                            "*" => "imul eax, ebx",
                            "&" => "and eax, ebx",
                            "|" => "or eax, ebx",
                            "^" => "xor eax, ebx",
                            _ => "; unknown op"
                        });
                    }
                    builder.AppendLineWithIndent($"mov [{b.Result}], eax");
                }
                break;

            case CondJumpIr c:
                LoadOperand(builder, "eax", c.Condition);
                builder.AppendLineWithIndent("cmp eax, 0");
                builder.AppendLineWithIndent($"je L{c.Target}");
                break;

            case JumpIr j:
                builder.AppendLineWithIndent($"jmp L{j.Target}");
                break;

            case ReturnIr r:
                if (r.Value is not null)
                    LoadOperand(builder, "eax", r.Value);
                else
                    builder.AppendLineWithIndent("xor eax, eax");
                builder.AppendLineWithIndent("ret");
                break;
        }
    }

    private static void LoadOperand(StringBuilder builder, string reg, string operand)
    {
        if (IsLiteral(operand))
            builder.AppendLineWithIndent($"mov {reg}, {operand}");
        else
            builder.AppendLineWithIndent($"mov {reg}, [{operand}]");
    }

    private static bool IsLiteral(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        if (s == "True" || s == "False") return true;
        return int.TryParse(s, out _) || s.StartsWith('-') && int.TryParse(s[1..], out _);
    }
}
