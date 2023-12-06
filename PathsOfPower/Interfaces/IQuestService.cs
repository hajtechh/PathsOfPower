using PathsOfPower.Models;

namespace PathsOfPower.Interfaces;

public interface IQuestService
{
    List<Quest> GetQuests();
    Quest GetQuest();
}
