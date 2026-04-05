/// <summary>
/// Provides line length tracking when redrawing boards. This allows us to move the cursor to 0,0 rather than clearing the console,
/// as we are redrawing entire lines including trailing spaces. This greatly reduces swcreen flicker.
/// </summary>
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