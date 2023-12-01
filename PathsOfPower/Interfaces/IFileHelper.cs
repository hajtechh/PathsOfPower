namespace PathsOfPower.Interfaces;

public interface IFileHelper
{
    bool IsNextChapterExisting(string currentChapter);
    string[]? GetAllSavedGameFilesFromDirectory();
    void WriteAllText(string jsonContent, char slotNumber);
    string? GetSavedGameFromFile(int slotNumber);
    string? GetSavedGameFromFile(string fullPath);
}
