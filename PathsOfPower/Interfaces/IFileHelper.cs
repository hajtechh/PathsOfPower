namespace PathsOfPower.Core.Interfaces;

public interface IFileHelper
{
    /// <summary>
    /// Returns the names of all .json files (including their paths) in directory SavedGameFiles.
    /// </summary>
    /// <returns>A string array of the full names (including paths) for all the .json files in SavedGameFiles directory or an empty string array if no files are found</returns>
    string[]? GetAllSavedGameFilesFromDirectory();
    bool IsNextChapterExisting(int currentChapter);
    void WriteAllText(string jsonContent, char slotNumber);
    string? GetSavedGameFromFile(int slotNumber);
    string? GetSavedGameFromFile(string fullPath);
    string? GetQuestsFromFile(int chapterNumber);
}
