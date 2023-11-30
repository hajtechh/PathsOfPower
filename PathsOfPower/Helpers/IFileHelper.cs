namespace PathsOfPower.Helpers;

public interface IFileHelper
{
    bool IsExisting(string path);
    string? ReadAllText(string path);
    bool WriteAllText(string path);
    string[]? GetFiles(string path);
}
