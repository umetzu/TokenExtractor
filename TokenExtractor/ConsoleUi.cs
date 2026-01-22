internal static class ConsoleUi
{
    private static void WriteColored(string text, ConsoleColor color, bool newline = true)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        if (newline)
        {
            Console.WriteLine(text);
        }
        else
        {
            Console.Write(text);
        }
        Console.ForegroundColor = previous;
    }

    internal static void Header(string text) => WriteColored(text, ConsoleColor.Cyan);
    internal static void Info(string text) => WriteColored(text, ConsoleColor.Gray);
    internal static void Step(string text) => WriteColored(text, ConsoleColor.White);
    internal static void Success(string text) => WriteColored(text, ConsoleColor.Green);
    internal static void Warning(string text) => WriteColored(text, ConsoleColor.Yellow);
    internal static void Error(string text) => WriteColored(text, ConsoleColor.Red);
    internal static void Prompt(string text) => WriteColored(text, ConsoleColor.Yellow, newline: false);
}
