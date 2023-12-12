namespace PathsOfPower.Core;

public class Factory : IFactory
{
    public Game CreateGame(
        List<Quest> quests,
        Player player,
        Quest quest,
        IFactory factory,
        IUserInteraction _userInteraction,
        IStringHelper _stringHelper,
        IFileHelper _fileHelper,
        IQuestService _questService,
        ISavedGameService _savedGameService)
    {
        return new(quests, player, quest, factory, _userInteraction, _stringHelper, _fileHelper, _questService, _savedGameService);
    }

    public List<InventoryItem> CreateInventoryItems() => new();

    public Player CreatePlayer(string name) => new(name);

    public SavedGame CreateSavedGame() => new();

    public List<SavedGame> CreateSavedGames() => new();
}
