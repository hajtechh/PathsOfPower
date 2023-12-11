namespace PathsOfPower.Core.Services;

public class QuestService : IQuestService
{
    private readonly IJsonHelper _jsonHelper;
    private readonly IFileHelper _fileHelper;

    public QuestService(IJsonHelper jsonHelper, IFileHelper fileHelper)
    {
        _jsonHelper = jsonHelper;
        _fileHelper = fileHelper;
    }

    public List<Quest>? GetQuestsFromChapter(int chapter)
    {
        var jsonContent = _fileHelper.GetQuestsFromFile(chapter);
        if (jsonContent is null)
            return null;
        return _jsonHelper.Deserialize<List<Quest>>(jsonContent);
    }

    public Quest GetQuestFromIndex(string index, List<Quest> quests) =>
        quests.First(x => x.Index == index);
}
