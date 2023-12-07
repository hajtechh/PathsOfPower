namespace PathsOfPower.Services;

public class QuestService : IQuestService
{
    private readonly IJsonHelper _jsonHelper;

    public QuestService(IJsonHelper jsonHelper)
    {
        _jsonHelper = jsonHelper;
    }

    public List<Quest>? GetQuests(string jsonContent) =>
        _jsonHelper.Deserialize<List<Quest>>(jsonContent);

}
