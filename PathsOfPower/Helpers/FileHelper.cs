namespace PathsOfPower.Helpers;

public class FileHelper : IFileHelper
{
    const string _basePath = "../../../../PathsOfPower/";
    private readonly string _baseQuestPath = _basePath + "Quests/chapter";
    private readonly string _baseSavePath = _basePath + "SavedGameFiles/slot";

    public bool IsNextChapterExisting(string currentChapter) =>
        File.Exists($"{_baseQuestPath}{currentChapter + 1}.json");

    public string[]? GetSavedGameFilesFromDirectory()
    {
        var path = $"{_basePath}SavedGameFiles/";
        return Directory.GetFiles(path, "*.json");
    }

    public bool WriteAllText(string path)
    {
        throw new NotImplementedException();
    }

    public string? GetSavedGameTextFromFile(int slotNumber) =>
        File.ReadAllText($"{_baseSavePath}{slotNumber}.json");
}
