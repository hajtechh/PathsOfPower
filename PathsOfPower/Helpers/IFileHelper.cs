namespace PathsOfPower.Helpers;

public interface IFileHelper
{
    bool IsNextChapterExisting(string currentChapter);
    string[]? GetSavedGameFilesFromDirectory();
    bool WriteAllText(string path);
    string? GetSavedGameTextFromFile(int slotNumber);
}
