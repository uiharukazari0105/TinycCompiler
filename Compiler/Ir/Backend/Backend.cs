using System.Text;
using Compiler.Ir.Backend.ARM;
using Compiler.Ir.Backend.Tools;
using Compiler.Ir.Backend.X86;
using Compiler.Ir.Instruction;

namespace Compiler.Ir.Backend;

public abstract class Backend
{
    protected IReadOnlyList<IrInstruction> Ir { get; }

    protected Backend(IReadOnlyList<IrInstruction> ir) { Ir = ir; }

    public static Backend Create(IReadOnlyList<IrInstruction> ir, BackendKind kind) => kind switch
    {
        BackendKind.ARM => new ArmBackend(ir),
        _ => new X86Backend(ir)
    };

    protected abstract string DataSection { get; }
    protected abstract string TextSection { get; }
    protected abstract string EntryPoint { get; }
    protected abstract string MainReg { get; }
    protected abstract string ReturnInsn { get; }
    protected abstract string VarDecl(string name);

    protected virtual string JumpMnemonic => "jmp";
    protected virtual string CondJumpMnemonic => "je";

    protected static bool IsLiteral(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        if (s is "True" or "False") return true;
        return int.TryParse(s, out _) || s.StartsWith('-') && int.TryParse(s[1..], out _);
    }

    protected HashSet<string> CollectVariables()
    {
        var vars = new HashSet<string>();
        foreach (var inst in Ir)
        {
            switch (inst)
            {
                case AssignIr a:
                    vars.Add(a.Result);
                    if (!IsLiteral(a.Value)) vars.Add(a.Value);
                    break;
                case BinaryIr b:
                    vars.Add(b.Result);
                    if (!IsLiteral(b.Left)) vars.Add(b.Left);
                    if (!IsLiteral(b.Right)) vars.Add(b.Right);
                    break;
                case ReturnIr r:
                    if (r.Value is not null && !IsLiteral(r.Value)) vars.Add(r.Value);
                    break;
                case CondJumpIr c:
                    if (!IsLiteral(c.Condition)) vars.Add(c.Condition);
                    break;
            }
        }
        return vars;
    }

    public string Generate()
    {
        var variables = CollectVariables();
        var builder = new StringBuilder();

        builder.AppendLine(DataSection);
        foreach (var v in variables.OrderBy(x => x))
            builder.AppendLineWithIndent(VarDecl(v));
        builder.AppendLine();

        builder.AppendLine(TextSection);
        builder.AppendLine(EntryPoint);

        foreach (var inst in Ir)
            Dispatch(builder, inst);

        if (Ir.Count == 0 || Ir[^1] is not ReturnIr)
        {
            builder.AppendLineWithIndent($"mov {MainReg}, 0");
            builder.AppendLineWithIndent(ReturnInsn);
        }

        return builder.ToString();
    }
    

    protected virtual void Dispatch(StringBuilder builder, IrInstruction inst)
    {
        switch (inst)
        {
            case LabelIr l:    builder.AppendLine($"L{l.Id}:"); break;
            case AssignIr a:   EmitAssign(builder, a); break;
            case BinaryIr b:   EmitBinary(builder, b); break;
            case CondJumpIr c:
                EmitLoad(builder, c.Condition);
                builder.AppendLineWithIndent($"cmp {MainReg}, 0");
                builder.AppendLineWithIndent($"{CondJumpMnemonic} L{c.Target}");
                break;
            case JumpIr j:     builder.AppendLineWithIndent($"{JumpMnemonic} L{j.Target}"); break;
            case ReturnIr r:   EmitReturn(builder, r); break;
        }
    }

    protected virtual void EmitLoad(StringBuilder builder, string operand)
    {
        if (IsLiteral(operand))
            builder.AppendLineWithIndent($"mov {MainReg}, {operand}");
        else
            builder.AppendLineWithIndent($"mov {MainReg}, [{operand}]");
    }
    
    protected virtual string ArithMnemonic(string op) => op switch
    {
        "+" => "add", "-" => "sub", "*" => "imul",
        "&" => "and", "|" => "or", "^" => "xor",
        _ => "?"
    };
    
    protected virtual string CondSuffix(string op) => op switch
    {
        "==" => "e", "!=" => "ne", "<" => "l",
        "<=" => "le", ">" => "g", ">=" => "ge",
        _ => "?"
    };
    

    protected abstract void EmitAssign(StringBuilder builder, AssignIr a);
    protected abstract void EmitBinary(StringBuilder builder, BinaryIr b);
    protected abstract void EmitReturn(StringBuilder builder, ReturnIr r);
}
