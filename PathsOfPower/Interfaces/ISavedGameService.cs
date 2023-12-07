using PathsOfPower.Models;

namespace PathsOfPower.Interfaces;

public interface ISavedGameService
{
    List<SavedGame> GetSavedGames();
    SavedGame GetSavedGame(int slotNumber);
}
