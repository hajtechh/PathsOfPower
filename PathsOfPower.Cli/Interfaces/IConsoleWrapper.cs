
namespace PathsOfPower.Cli.Interfaces;

public interface IConsoleWrapper
{
    void WriteLine(string s);
    string? ReadLine();
    ConsoleKeyInfo ReadChar();
    void Clear();
}
