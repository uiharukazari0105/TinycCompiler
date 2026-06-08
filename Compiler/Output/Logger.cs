namespace Compiler.Output;

public static class Logger
{
    public static void Raise(LogDefinition logDefinition, bool ignoreHalt = false)
    {
        var colorMem = Console.ForegroundColor;
        switch (logDefinition.Level)
        {
         case LogLevel.Info:
             Console.ForegroundColor = ConsoleColor.Green;
             Console.WriteLine(logDefinition.Message);
             break;
         case LogLevel.Warning:
             Console.ForegroundColor = ConsoleColor.Yellow;
             Console.WriteLine(logDefinition.Message);
             break;
         case LogLevel.Error:
             Console.ForegroundColor = ConsoleColor.Red;
             Console.Error.WriteLine(logDefinition.Message);
             break;
        }
        if (logDefinition.Halt && !ignoreHalt)
           Halt();
        Console.ForegroundColor = colorMem;
    }

    public static void RaiseMany(IEnumerable<LogDefinition> logDefinitions)
    {
        bool shouldHalt = false;
        foreach (var logDefinition in logDefinitions)
        {
            if(logDefinition.Halt) shouldHalt = true;
            Raise(logDefinition, true);
        }
        if(shouldHalt)
            Halt();
    }

    private static void Halt()
    {
        throw new Exception("由于发生了一个致命错误，编译终止");
    }
}