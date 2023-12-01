using PathsOfPower.Interfaces;

namespace PathsOfPower.Helpers;

public class FileHelper : IFileHelper
{
    const string _basePath = "../../../../PathsOfPower/";
    private readonly string _baseQuestPath = _basePath + "Quests/chapter";
    private readonly string _baseSavePath = _basePath + "SavedGameFiles/slot";

    /// <summary>
    /// Returns the names of all .json files (including their paths) in directory SavedGameFiles.
    /// </summary>
    /// <returns>A string array of the full names (including paths) for all the .json files in SavedGameFiles directory or an empty string array if no files are found</returns>
    public string[]? GetAllSavedGameFilesFromDirectory()
    {
        var path = $"{_basePath}SavedGameFiles/";
        return Directory.GetFiles(path, "*.json");
    }

    public bool IsNextChapterExisting(string currentChapter) =>
        File.Exists($"{_baseQuestPath}{currentChapter + 1}.json");

    public void WriteAllText(string jsonContent, char slotNumber) =>
        File.WriteAllText($"{_baseSavePath}{slotNumber}.json", jsonContent);

    public string? GetSavedGameFromFile(int slotNumber) =>
        File.ReadAllText($"{_baseSavePath}{slotNumber}.json");

    public string? GetSavedGameFromFile(string fullPath) =>
        File.ReadAllText(fullPath);
}