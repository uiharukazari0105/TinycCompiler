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
    

    public override IReadOnlyList<InsnEntry> GetInstructionTable() => new InsnEntry[]
    {
        new("数据传送", "x = c",       "mov",  "eax, imm / [mem]",    "立即数或内存值载入寄存器"),
        new("数据传送", "x = y",       "mov",  "[mem], eax",          "寄存器值写回内存变量"),
        new("算术运算", "x = a + b",   "add",  "eax, imm / reg",     "整数加法"),
        new("算术运算", "x = a - b",   "sub",  "eax, imm / reg",     "整数减法"),
        new("算术运算", "x = a * b",   "imul", "eax, imm / reg",     "有符号整数乘法"),
        new("算术运算", "x = a / b",   "idiv", "ebx (cdq 前缀)",     "有符号整数除法 (eax=商, edx=余)"),
        new("算术运算", "x = a % b",   "idiv", "ebx → mov eax, edx", "有符号整数取模"),
        new("位运算",   "x = a & b",   "and",  "eax, imm / reg",     "按位与"),
        new("位运算",   "x = a | b",   "or",   "eax, imm / reg",     "按位或"),
        new("位运算",   "x = a ^ b",   "xor",  "eax, imm / reg",     "按位异或"),
        new("位运算",   "x = ~a",      "not",  "eax",                "按位取反"),
        new("一元运算", "x = -a",      "neg",  "eax",                "算术取负"),
        new("一元运算", "x = !a",      "cmp/sete/movzx", "eax",      "逻辑取反 (比较后置位并零扩展)"),
        new("比较运算", "x = a == b",  "cmp + sete",  "eax, imm/reg", "相等比较 → 0/1"),
        new("比较运算", "x = a != b",  "cmp + setne", "eax, imm/reg", "不等比较 → 0/1"),
        new("比较运算", "x = a < b",   "cmp + setl",  "eax, imm/reg", "小于比较 → 0/1"),
        new("比较运算", "x = a <= b",  "cmp + setle", "eax, imm/reg", "小于等于比较 → 0/1"),
        new("比较运算", "x = a > b",   "cmp + setg",  "eax, imm/reg", "大于比较 → 0/1"),
        new("比较运算", "x = a >= b",  "cmp + setge", "eax, imm/reg", "大于等于比较 → 0/1"),
        new("逻辑运算", "x = a && b",  "cmp/setne + and", "eax, ebx", "逻辑与 (两个操作数分别布尔化后按位与)"),
        new("逻辑运算", "x = a || b",  "cmp/setne + or",  "eax, ebx", "逻辑或 (两个操作数分别布尔化后按位或)"),
        new("控制流",   "if x == 0 goto L", "cmp + je", "eax, label", "条件为假时跳转"),
        new("控制流",   "goto L",      "jmp",  "label",              "无条件跳转"),
        new("控制流",   "return x",    "mov + ret", "eax",            "返回值载入 eax 并返回"),
        new("控制流",   "L:",          "L{n}:", "—",                  "跳转标签"),
        new("栈帧",     "函数入口",    "—",    "main:",               "入口标签，无显式栈帧"),
        
        new("复合赋值", "x += c",      "add",  "[mem], imm / reg",   "自增赋值 (先计算再store)"),
        new("复合赋值", "x -= c",      "sub",  "[mem], imm / reg",   "自减赋值"),
        new("复合赋值", "x *= c",      "imul", "[mem], imm / reg",   "自乘赋值"),
        new("复合赋值", "x /= c",      "idiv", "[mem], imm / reg",   "自除赋值"),
        new("复合赋值", "x %= c",      "idiv", "[mem], imm / reg",   "自模赋值"),
        
        new("自增自减", "++x",         "add + store", "eax → [mem]",  "前缀自增：先加1再返回新值"),
        new("自增自减", "--x",         "sub + store", "eax → [mem]",  "前缀自减"),
        new("自增自减", "x++",         "mov + add + store", "eax → [mem]", "后缀自增：保存旧值后加1，返回旧值"),
        new("自增自减", "x--",         "mov + sub + store", "eax → [mem]", "后缀自减"),
    };
}
