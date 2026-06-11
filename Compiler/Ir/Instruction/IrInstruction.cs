namespace Compiler.Ir.Instruction;

public abstract class IrInstruction
{
    public abstract IrKind Kind { get; }
}