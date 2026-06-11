namespace Compiler.Ir.Instruction;

public sealed class CondJumpIr : IrInstruction
{
    public override IrKind Kind => IrKind.CondJump;
    public string Condition { get; }
    public int Target { get; }

    public CondJumpIr(string condition, int target) { Condition = condition; Target = target; }
    public override string ToString() => $"if {Condition} == 0 goto L{Target}";
}