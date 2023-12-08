using PathsOfPower.Core.Models;

namespace PathsOfPower.Core.Interfaces;

public interface IQuestService
{
    List<Quest>? GetQuests(string jsonContent);
}
