namespace PathsOfPower.Core.Interfaces;

public interface ISavedGameService
{
    List<SavedGame> GetSavedGames();
    SavedGame? GetSavedGame(string jsonContent);
    (bool isSaved, string message) SaveGame(Player player, char input, string questIndex);
    (SavedGame? savedGame, string message) LoadGame(char slotNumber);
    bool CheckForValidSlotNumber(char input);
}
