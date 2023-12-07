using PathsOfPower.Models;

namespace PathsOfPower.Interfaces;

public interface ISavedGameService
{
    List<SavedGame>? GetSavedGames(string jsonContent);
    SavedGame? GetSavedGame(string jsonContent);
    string? CreateSavedGame(SavedGame savedGame);
}
