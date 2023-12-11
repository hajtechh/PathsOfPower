using PathsOfPower.Core.Interfaces;
using PathsOfPower.Core.Models;

namespace PathsOfPower.Core.Services;

public class SavedGameService : ISavedGameService
{
    private readonly IJsonHelper _jsonHelper;

    public SavedGameService(IJsonHelper jsonHelper)
    {
        _jsonHelper = jsonHelper;
    }

    public List<SavedGame>? GetSavedGames(string jsonContent) =>
        _jsonHelper.Deserialize<List<SavedGame>>(jsonContent);

    public SavedGame? GetSavedGame(string jsonContent) =>
        _jsonHelper.Deserialize<SavedGame>(jsonContent);

    public string? CreateSavedGame(SavedGame savedGame) =>
        _jsonHelper.Serialize<SavedGame>(savedGame);
}
