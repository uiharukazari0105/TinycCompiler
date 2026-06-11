namespace Compiler.Ir.Instruction;

public sealed class AssignIr : IrInstruction
{
    public override IrKind Kind => IrKind.Assign;
    public string Result { get; }
    public string Value { get; }

    public AssignIr(string result, string value) { Result = result; Value = value; }
    public override string ToString() => $"{Result} = {Value}";
}