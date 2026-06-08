namespace Compiler;

public class Position
{
    public int Line { get; set; }
    public int Column { get; set; }
    public string FileName { get; init; }
    
    public Position(int line, int column, string fileName)
    {
        Line = line;
        Column = column;
        FileName = fileName;
    }
}