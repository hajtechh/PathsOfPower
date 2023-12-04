using PathsOfPower.Interfaces;

namespace PathsOfPower.Helpers;

public class FileHelper : IFileHelper
{
    const string _basePath = "../../../../PathsOfPower/";
    private readonly string _baseQuestPath = _basePath + "Quests/chapter";
    private readonly string _baseSavePath = _basePath + "SavedGameFiles/slot";

    public string[]? GetAllSavedGameFilesFromDirectory()
    {
        var path = $"{_basePath}SavedGameFiles/";
        return Directory.GetFiles(path, "*.json");
    }

    public bool IsNextChapterExisting(int currentChapter) =>
        File.Exists($"{_baseQuestPath}{currentChapter + 1}.json");

    public void WriteAllText(string jsonContent, char slotNumber) =>
        File.WriteAllText($"{_baseSavePath}{slotNumber}.json", jsonContent);

    public string? GetSavedGameFromFile(int slotNumber) =>
        File.ReadAllText($"{_baseSavePath}{slotNumber}.json");

    public string? GetSavedGameFromFile(string fullPath) =>
        File.ReadAllText(fullPath);
    public string? GetQuestsFromFile(int chapterNumber) =>
    File.ReadAllText($"{_baseQuestPath}{chapterNumber}.json");
}