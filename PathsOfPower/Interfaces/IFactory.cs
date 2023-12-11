namespace PathsOfPower.Core.Interfaces;

public interface IFactory
{
    Game CreateGame();
    SavedGame CreateSavedGame();
    List<SavedGame> CreateSavedGames();
    Player CreatePlayer();
}
