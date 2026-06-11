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
            case "-": builder.AppendLineWithIndent("rsb r0, r0, #0"); break;
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

    public override IReadOnlyList<InsnEntry> GetInstructionTable() => new InsnEntry[]
    {
        new("数据传送", "x = c",       "ldr",  "r0, =imm / [addr]",  "立即数或地址载入寄存器 (需字面量池)"),
        new("数据传送", "x = y",       "ldr + str", "r0, [addr] → r2, [addr]", "内存变量加载后存至目标地址"),
        new("算术运算", "x = a + b",   "add",  "r0, r0, imm / reg", "整数加法 (三操作数格式)"),
        new("算术运算", "x = a - b",   "sub",  "r0, r0, imm / reg", "整数减法"),
        new("算术运算", "x = a * b",   "mul",  "r0, r0, reg",       "整数乘法 (ARM mul 不支持立即数)"),
        new("算术运算", "x = a / b",   "(未实现)", "—",             "ARM 无硬件除法指令，需软件除或库调用"),
        new("算术运算", "x = a % b",   "(未实现)", "—",             "同上"),
        new("位运算",   "x = a & b",   "and",  "r0, r0, imm / reg", "按位与"),
        new("位运算",   "x = a | b",   "orr",  "r0, r0, imm / reg", "按位或 (ARM 用 orr 非 or)"),
        new("位运算",   "x = a ^ b",   "eor",  "r0, r0, imm / reg", "按位异或 (ARM 用 eor 非 xor)"),
        new("位运算",   "x = ~a",      "mvn",  "r0, r0",            "按位取反 (MVN = MoVe Not)"),
        new("一元运算", "x = -a",      "rsb",  "r0, r0, #0",        "算术取负 (RSB = Reverse SuBtract)"),
        new("一元运算", "x = !a",      "cmp + mov/moveq", "r0",     "逻辑取反: 与0比较后条件置1/0"),
        new("比较运算", "x = a == b",  "cmp + moveq", "r0, imm/reg", "相等比较 → 0/1 (条件传送)"),
        new("比较运算", "x = a != b",  "cmp + movne", "r0, imm/reg", "不等比较 → 0/1"),
        new("比较运算", "x = a < b",   "cmp + movlt", "r0, imm/reg", "小于比较 → 0/1"),
        new("比较运算", "x = a <= b",  "cmp + movle", "r0, imm/reg", "小于等于比较 → 0/1"),
        new("比较运算", "x = a > b",   "cmp + movgt", "r0, imm/reg", "大于比较 → 0/1"),
        new("比较运算", "x = a >= b",  "cmp + movge", "r0, imm/reg", "大于等于比较 → 0/1"),
        new("逻辑运算", "x = a && b",  "cmp/movne + and", "r0, r1", "逻辑与 (布尔化后按位与)"),
        new("逻辑运算", "x = a || b",  "cmp/movne + orr", "r0, r1", "逻辑或 (布尔化后按位或)"),
        new("控制流",   "if x == 0 goto L", "cmp + beq", "r0, label", "条件为假时跳转"),
        new("控制流",   "goto L",      "b",    "label",              "无条件跳转 (Branch)"),
        new("控制流",   "return x",    "ldr + pop {pc}", "r0",       "返回值载入 r0，通过出栈恢复 PC 返回"),
        new("控制流",   "L:",          "L{n}:", "—",                  "跳转标签"),
        new("栈帧",     "函数入口",    "push {lr}", "lr",             "保存返回地址到栈"),
        new("栈帧",     "函数返回",    "pop {pc}", "pc",              "从栈恢复程序计数器实现返回"),
        
        new("复合赋值", "x += c",      "add + store", "r0 → [mem]",  "自增赋值 (先计算再store)"),
        new("复合赋值", "x -= c",      "sub + store", "r0 → [mem]",  "自减赋值"),
        new("复合赋值", "x *= c",      "mul + store", "r0 → [mem]",  "自乘赋值"),

        new("自增自减", "++x",         "add + store", "r0 → [mem]",  "前缀自增：先加1再返回新值"),
        new("自增自减", "--x",         "sub + store", "r0 → [mem]",  "前缀自减"),
        new("自增自减", "x++",         "ldr + add + store", "r0 → [mem]", "后缀自增：保存旧值后加1"),
        new("自增自减", "x--",         "ldr + sub + store", "r0 → [mem]", "后缀自减"),
    };
}
