namespace PathsOfPower.Cli;

public class ConsoleWrapper : IConsoleWrapper
{
    public void Clear() => Console.Clear();

    public ConsoleKeyInfo ReadChar() => Console.ReadKey(true);

    public string? ReadLine() => Console.ReadLine();

    public void WriteLine(string s) => Console.WriteLine(s);
}
