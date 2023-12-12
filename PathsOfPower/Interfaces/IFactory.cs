﻿namespace PathsOfPower.Core.Interfaces;

public interface IFactory
{
    Game CreateGame(List<Quest> quests, Player player, Quest quest,
        IFactory factory, IUserInteraction _userInteraction, IStringHelper _stringHelper, IFileHelper _fileHelper, IQuestService _questService, ISavedGameService _savedGameService);
    SavedGame CreateSavedGame(IFactory factory);
    List<SavedGame> CreateSavedGames();
    Player CreatePlayer(string name);
    List<InventoryItem> CreateInventoryItems();
}
