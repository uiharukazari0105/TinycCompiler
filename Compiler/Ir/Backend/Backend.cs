using Compiler.Ir.Backend.X86;
using Compiler.Ir.Instruction;

namespace Compiler.Ir.Backend;

public abstract class Backend
{
    protected IReadOnlyList<IrInstruction> Ir { get; }
    
    public Backend(IReadOnlyList<IrInstruction> ir)
    {
        Ir = ir;
    }

    public static Backend Create(IReadOnlyList<IrInstruction> ir, BackendKind kind)
    {
        return kind switch
        {
            BackendKind.X86 or _ => new X86Backend(ir)
        };
    }
    
    public abstract string Generate();
}
