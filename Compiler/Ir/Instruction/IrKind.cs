namespace Compiler.Ir.Instruction;

public enum IrKind
{
    Label,
    Assign,
    Binary,
    CondJump,
    Jump,
    Return,
}