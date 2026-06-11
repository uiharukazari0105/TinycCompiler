namespace Compiler.Ir.Instruction;

/// <summary>goto L{n}</summary>
public sealed class JumpIr : IrInstruction
{
    public override IrKind Kind => IrKind.Jump;
    public int Target { get; }

    public JumpIr(int target) { Target = target; }
    public override string ToString() => $"goto L{Target}";
}