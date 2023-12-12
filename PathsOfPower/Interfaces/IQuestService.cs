namespace PathsOfPower.Core.Interfaces;

public interface IQuestService
{
    List<Quest>? GetQuestsFromChapter(int chapter);
    Quest GetQuestFromIndex(string index, List<Quest> quests);
    bool CheckIfOptionExists(char optionIndex, Quest quest);
}