using System.Text;
using Compiler.Ir.Backend.Tools;
using Compiler.Ir.Instruction;

namespace Compiler.Ir.Backend.X86;

public class X86Backend : Backend
{
    public X86Backend(IReadOnlyList<IrInstruction> ir) : base(ir) { }

    protected override string DataSection => "section .data";
    protected override string TextSection => "section .text";
    protected override string EntryPoint => "main:";
    protected override string MainReg => "eax";
    protected override string ReturnInsn => "ret";
    protected override string VarDecl(string name) => $"{name} dd 0";

    protected override string JumpMnemonic => "jmp";
    protected override string CondJumpMnemonic => "je";
    

    protected override void EmitAssign(StringBuilder builder, AssignIr a)
    {
        if (IsLiteral(a.Value))
            builder.AppendLineWithIndent($"mov eax, {a.Value}");
        else
            builder.AppendLineWithIndent($"mov eax, [{a.Value}]");
        builder.AppendLineWithIndent($"mov [{a.Result}], eax");
    }

    protected override void EmitBinary(StringBuilder builder, BinaryIr b)
    {
        if (string.IsNullOrEmpty(b.Left))
            EmitUnary(builder, b);
        else if (b.Operator is "==" or "!=" or "<" or "<=" or ">" or ">=")
            EmitCmp(builder, b);
        else if (b.Operator is "&&" or "||")
            EmitLogical(builder, b);
        else
            EmitArith(builder, b, b.Operator is "/" or "%");
    }
    
    protected override void EmitReturn(StringBuilder builder, ReturnIr r)
    {
        if (r.Value is not null)
            EmitLoad(builder, r.Value);
        else
            builder.AppendLineWithIndent("xor eax, eax");
        builder.AppendLineWithIndent("ret");
    }

    private void EmitUnary(StringBuilder builder, BinaryIr b)
    {
        EmitLoad(builder, b.Right);
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

    private void EmitCmp(StringBuilder builder, BinaryIr b)
    {
        EmitLoad(builder, b.Left);
        if (IsLiteral(b.Right))
            builder.AppendLineWithIndent($"cmp eax, {b.Right}");
        else
        {
            EmitLoad2(builder, b.Right);
            builder.AppendLineWithIndent("cmp eax, ebx");
        }
        builder.AppendLineWithIndent($"set{CondSuffix(b.Operator)} al");
        builder.AppendLineWithIndent("movzx eax, al");
        builder.AppendLineWithIndent($"mov [{b.Result}], eax");
    }

    private void EmitLogical(StringBuilder builder, BinaryIr b)
    {
        EmitLoad(builder, b.Left);
        builder.AppendLineWithIndent("cmp eax, 0");
        builder.AppendLineWithIndent("setne al");
        builder.AppendLineWithIndent("movzx eax, al");
        EmitLoad2(builder, b.Right);
        builder.AppendLineWithIndent("cmp ebx, 0");
        builder.AppendLineWithIndent("setne bl");
        builder.AppendLineWithIndent("movzx ebx, bl");
        builder.AppendLineWithIndent(b.Operator == "&&" ? "and eax, ebx" : "or eax, ebx");
        builder.AppendLineWithIndent($"mov [{b.Result}], eax");
    }

    private void EmitArith(StringBuilder builder, BinaryIr b, bool isDivMod)
    {
        EmitLoad(builder, b.Left);
        if (isDivMod)
        {
            EmitLoad2(builder, b.Right);
            builder.AppendLineWithIndent("cdq");
            builder.AppendLineWithIndent("idiv ebx");
            if (b.Operator == "%")
                builder.AppendLineWithIndent("mov eax, edx");
        }
        else if (IsLiteral(b.Right))
        {
            var m = ArithMnemonic(b.Operator);
            builder.AppendLineWithIndent($"{m} eax, {b.Right}");
        }
        else
        {
            EmitLoad2(builder, b.Right);
            var m = ArithMnemonic(b.Operator);
            builder.AppendLineWithIndent($"{m} eax, ebx");
        }
        builder.AppendLineWithIndent($"mov [{b.Result}], eax");
    }

    private void EmitLoad2(StringBuilder builder, string operand)
    {
        if (IsLiteral(operand))
            builder.AppendLineWithIndent($"mov ebx, {operand}");
        else
            builder.AppendLineWithIndent($"mov ebx, [{operand}]");
    }
}
