namespace PathsOfPower.Core.Models;

public class SavedGame
{
    public Player Player { get; set; }
    public string QuestIndex { get; set; }

    public SavedGame(Player player, string questIndex)
    {
        Player = player;
        QuestIndex = questIndex;
    }

    public SavedGame(IFactory factory)
    {
        Player = factory.CreatePlayer(string.Empty);
        QuestIndex = string.Empty;
    }
}
