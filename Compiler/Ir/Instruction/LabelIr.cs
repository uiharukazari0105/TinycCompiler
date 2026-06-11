namespace Compiler.Ir.Instruction;

public sealed class LabelIr : IrInstruction
{
    public override IrKind Kind => IrKind.Label;
    public int Id { get; }
    public LabelIr(int id) => Id = id;
    public override string ToString() => $"L{Id}:";
}