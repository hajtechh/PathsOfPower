using PathsOfPower.Models;

namespace PathsOfPower.Interfaces;

public interface IQuestService
{
    List<Quest>? GetQuests(string jsonContent);
}
