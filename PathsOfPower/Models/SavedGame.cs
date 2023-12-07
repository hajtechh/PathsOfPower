namespace PathsOfPower.Models;

public class SavedGame
{
    public Player Player { get; set; }
    public string QuestIndex { get; set; }

    public SavedGame(Player player, string questIndex)
    {
        Player = player;
        QuestIndex = questIndex;
    }

    public SavedGame()
    {
        Player = new Player(string.Empty);
        QuestIndex = string.Empty;
    }
}
