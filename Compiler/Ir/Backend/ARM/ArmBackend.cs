using System.Text;
using Compiler.Ir.Backend.Tools;
using Compiler.Ir.Instruction;

namespace Compiler.Ir.Backend.ARM;

public class ArmBackend : Backend
{
    public ArmBackend(IReadOnlyList<IrInstruction> ir) : base(ir) { }

    protected override string DataSection => "section .data";
    protected override string TextSection => "section .text";
    protected override string EntryPoint => ".global main\nmain:\n    push {lr}";
    protected override string MainReg => "r0";
    protected override string ReturnInsn => "pop {pc}";
    protected override string VarDecl(string name) => $"{name}: .word 0";

    protected override string JumpMnemonic => "b";
    protected override string CondJumpMnemonic => "beq";
    
    protected override string ArithMnemonic(string op) => op switch
    {
        "|" => "orr", "^" => "eor", "*" => "mul",
        _ => base.ArithMnemonic(op)
    };
    
    protected override void EmitLoad(StringBuilder builder, string operand)
    {
        if (IsLiteral(operand))
            builder.AppendLineWithIndent($"ldr {MainReg}, ={operand}");
        else
        {
            builder.AppendLineWithIndent($"ldr {MainReg}, ={operand}");
            builder.AppendLineWithIndent($"ldr {MainReg}, [{MainReg}]");
        }
    }

    private void EmitLoad2(StringBuilder builder, string operand)
    {
        if (IsLiteral(operand))
            builder.AppendLineWithIndent($"ldr r1, ={operand}");
        else
        {
            builder.AppendLineWithIndent("ldr r1, =" + operand);
            builder.AppendLineWithIndent("ldr r1, [r1]");
        }
    }

    private void EmitStore(StringBuilder builder, string variable)
    {
        builder.AppendLineWithIndent($"ldr r2, ={variable}");
        builder.AppendLineWithIndent($"str {MainReg}, [r2]");
    }
    

    protected override void EmitAssign(StringBuilder builder, AssignIr a)
    {
        if (IsLiteral(a.Value))
            builder.AppendLineWithIndent($"ldr {MainReg}, ={a.Value}");
        else
            EmitLoad(builder, a.Value);
        EmitStore(builder, a.Result);
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
            EmitArith(builder, b);
    }

    protected override void EmitReturn(StringBuilder builder, ReturnIr r)
    {
        if (r.Value is not null)
            EmitLoad(builder, r.Value);
        else
            builder.AppendLineWithIndent($"mov {MainReg}, #0");
        builder.AppendLineWithIndent(ReturnInsn);
    }

    private void EmitUnary(StringBuilder builder, BinaryIr b)
    {
        EmitLoad(builder, b.Right);
        switch (b.Operator)
        {
            case "-": builder.AppendLineWithIndent("rbuilder r0, r0, #0"); break;
            case "!":
                builder.AppendLineWithIndent("cmp r0, #0");
                builder.AppendLineWithIndent("mov r0, #0");
                builder.AppendLineWithIndent("moveq r0, #1");
                break;
            case "~": builder.AppendLineWithIndent("mvn r0, r0"); break;
        }
        EmitStore(builder, b.Result);
    }

    private void EmitCmp(StringBuilder builder, BinaryIr b)
    {
        EmitLoad(builder, b.Left);
        if (IsLiteral(b.Right))
            builder.AppendLineWithIndent($"cmp {MainReg}, #{b.Right}");
        else
        {
            EmitLoad2(builder, b.Right);
            builder.AppendLineWithIndent($"cmp {MainReg}, r1");
        }
        builder.AppendLineWithIndent($"mov {MainReg}, #0");
        builder.AppendLineWithIndent($"mov{CondSuffix(b.Operator)} {MainReg}, #1");
        EmitStore(builder, b.Result);
    }

    private void EmitLogical(StringBuilder builder, BinaryIr b)
    {
        EmitLoad(builder, b.Left);
        builder.AppendLineWithIndent("cmp r0, #0");
        builder.AppendLineWithIndent("movne r0, #1");
        EmitLoad2(builder, b.Right);
        builder.AppendLineWithIndent("cmp r1, #0");
        builder.AppendLineWithIndent("movne r1, #1");
        builder.AppendLineWithIndent(b.Operator == "&&" ? "and r0, r0, r1" : "orr r0, r0, r1");
        EmitStore(builder, b.Result);
    }

    private void EmitArith(StringBuilder builder, BinaryIr b)
    {
        EmitLoad(builder, b.Left);
        if (b.Operator is "/" or "%")
        {
            builder.AppendLineWithIndent($"; division not implemented for ARM: {b}");
            EmitStore(builder, b.Result);
            return;
        }
        var m = ArithMnemonic(b.Operator);
        if (IsLiteral(b.Right))
            builder.AppendLineWithIndent($"{m} {MainReg}, {MainReg}, #{b.Right}");
        else
        {
            EmitLoad2(builder, b.Right);
            builder.AppendLineWithIndent($"{m} {MainReg}, {MainReg}, r1");
        }
        EmitStore(builder, b.Result);
    }
}
