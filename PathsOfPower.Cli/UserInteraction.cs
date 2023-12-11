namespace PathsOfPower.Cli;

public class UserInteraction : IUserInteraction
{
    private readonly IConsoleWrapper _consoleWrapper;

    public UserInteraction(IConsoleWrapper consoleWrapper) =>
        _consoleWrapper = consoleWrapper;

    public void Print(string message) =>
        _consoleWrapper.WriteLine(message);

    public void ClearConsole() =>
        _consoleWrapper.Clear();

    public string GetInput(string message)
    {
        _consoleWrapper.WriteLine(message);
        return _consoleWrapper.ReadLine() ?? string.Empty;
    }

    public ConsoleKeyInfo GetChar() =>
        _consoleWrapper.ReadChar();
}
