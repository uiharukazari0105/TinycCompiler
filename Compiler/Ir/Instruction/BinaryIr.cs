namespace Compiler.Ir.Instruction;

public sealed class BinaryIr : IrInstruction
{
    public override IrKind Kind => IrKind.Binary;
    public string Result { get; }
    public string Left { get; }
    public string Operator { get; }
    public string Right { get; }

    public BinaryIr(string result, string left, string op, string right)
    {
        Result = result; Left = left; Operator = op; Right = right;
    }

    public override string ToString() => $"{Result} = {Left} {Operator} {Right}";
}