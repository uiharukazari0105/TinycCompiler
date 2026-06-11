namespace Compiler.Ir.Instruction;

public sealed class ReturnIr : IrInstruction
{
    public override IrKind Kind => IrKind.Return;
    public string? Value { get; }

    public ReturnIr(string? value) { Value = value; }
    public override string ToString() => Value is null ? "return" : $"return {Value}";
}
