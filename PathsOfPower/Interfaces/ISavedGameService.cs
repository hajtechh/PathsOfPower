using PathsOfPower.Core.Models;

namespace PathsOfPower.Core.Interfaces;

public interface ISavedGameService
{
    List<SavedGame>? GetSavedGames(string jsonContent);
    SavedGame? GetSavedGame(string jsonContent);
    string? CreateSavedGame(SavedGame savedGame);
}
