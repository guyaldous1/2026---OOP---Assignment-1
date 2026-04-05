static class ConsoleHelper
{
    private const int LINE_LENGTH = 120;
    private static int lineLengthSoFar = 0;

    public static void Write(string output)
    {
        Console.Write(output);
        lineLengthSoFar += output.Length;
    }

    public static void WriteLine(string output)
    {
        Write(output);
        int paddingSize = LINE_LENGTH - lineLengthSoFar;
        Console.WriteLine(paddingSize >=0 ? new string(' ', paddingSize) : string.Empty);
        lineLengthSoFar = 0;
    }

    public static void WriteLine()
    {
        WriteLine(string.Empty);
    }
}