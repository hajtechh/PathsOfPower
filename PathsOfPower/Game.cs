﻿namespace PathsOfPower.Core;

public class Game
{
    #region PrivateVariables
    private const int MAX_HEALTH_POINTS = 100;
    private const char MIN_SLOT_NUMBER = '1';
    private const char MAX_SLOT_NUMBER = '3';

    private readonly IUserInteraction _userInteraction;
    private readonly IStringHelper _stringHelper;
    private readonly IFileHelper _fileHelper;
    private readonly IQuestService _questService;
    private readonly ISavedGameService _savedGameService;
    #endregion

    public List<Quest> Quests { get; set; }
    public Player Player { get; set; }

    public Game(List<Quest> quests, Player player,
        IUserInteraction userInteraction,
        IStringHelper stringHelper,
        IFileHelper fileHelper,
        IQuestService questService,
        ISavedGameService savedGameService)
    {
        _userInteraction = userInteraction;
        _stringHelper = stringHelper;
        _fileHelper = fileHelper;
        _questService = questService;
        _savedGameService = savedGameService;

        Player = player;
        Quests = quests;
    }

    //private void StartGame(string questIndex)
    //{
    //    var currentChapter = questIndex[..1];
    //    var chapter = int.Parse(currentChapter);
    //    Quests = GetQuests(chapter);

    //    var quest = new Quest();
    //    if (Quests is not null)
    //        quest = GetQuestFromIndex(questIndex, Quests);

    //    var keyActions = SetupKeyActionsInGame(quest);

    //    GameLoop(ref chapter, ref quest, keyActions);
    //}

    private Dictionary<ConsoleKey, Action> SetupKeyActionsInGameMenu(string questIndex) =>
        new()
        {
            //{ ConsoleKey.D1, () => StartGame(questIndex) },
            { ConsoleKey.D2, () => SaveGame(questIndex) },
            //{ ConsoleKey.D3, Run },
            { ConsoleKey.D4, () => QuitGame() },
            //{ ConsoleKey.NumPad1, () => StartGame(questIndex) },
            { ConsoleKey.NumPad2, () => SaveGame(questIndex) },
            //{ ConsoleKey.NumPad3, Run },
            { ConsoleKey.NumPad4, () => QuitGame() }
        };

    public void GameLoop(ref int chapter, ref Quest quest, Dictionary<ConsoleKey, Action> keyActions)
    {
        var isQuittingGame = false;
        while (isQuittingGame is false)
        {
            if (Player is null || quest is null)
                return;

            _userInteraction.ClearConsole();

            PrintQuestWithOverlayAndInventory(quest);
            HandleQuestEvents(quest);

            if (quest.Options is not null)
                quest = RunQuestWithOptions(quest, keyActions);
            else if (_fileHelper.IsNextChapterExisting(chapter))
                quest = RunQuestWithoutOptions(chapter + 1, quest, keyActions);
            else
                isQuittingGame = PrintTheEnd();
        }
    }

    private Quest RunQuestWithoutOptions(int chapter, Quest quest, Dictionary<ConsoleKey, Action> keyActions)
    {;
        Quests = _questService.GetQuestsFromChapter(chapter);

        if (Quests is not null)
            quest = _questService.GetQuestFromIndex(chapter.ToString(), Quests);

        HandleQuestEvents(quest);

        PrintContinueText();

        var input = _userInteraction.GetChar();
        CheckIfUserWantsToGoToGameMenu(keyActions, input);
        return quest;
    }

    private Quest? RunQuestWithOptions(Quest quest, Dictionary<ConsoleKey, Action> keyActions)
    {
        // 4.1.1. Get input
        var choice = _userInteraction.GetChar();
        if (char.IsDigit(choice.KeyChar))
        {
            // 4.1.1.1. Get next quest based on what option is chosen
            quest = GetNextQuestBasenOnChosenOption(quest, choice);

            // 4.1.1.2. HandleOptionEvents
            HandleOptionEventsInQuest(quest, choice);
        }
        // 4.1.2. User did not type in any digits
        else
        {
            // 4.1.2.1 Check if user wants to go to menu
            CheckIfUserWantsToGoToGameMenu(keyActions, choice);
        }

        return quest;
    }

    private Quest GetNextQuestBasenOnChosenOption(Quest quest, ConsoleKeyInfo choice)
    {
        var index = _stringHelper.GetQuestIndexString(quest.Index, choice.KeyChar);
        if (Quests is not null)
            quest = _questService.GetQuestFromIndex(index, Quests);
        return quest;
    }

    private void CheckIfUserWantsToGoToGameMenu(Dictionary<ConsoleKey, Action> keyActions, ConsoleKeyInfo input)
    {
        if (keyActions.TryGetValue(input.Key, out var action))
            action.Invoke();
    }

    private void HandleOptionEventsInQuest(Quest quest, ConsoleKeyInfo choice)
    {
        if (Player is null)
            return;

        var index = int.Parse(choice.KeyChar.ToString());
        var option = quest.Options?.FirstOrDefault(x => x.Index == index);
        if (option is null || option.MoralityScore is not 0)
            return;

        Player.ApplyMoralityScore(option.MoralityScore);
    }

    private void HandleQuestEvents(Quest quest)
    {
        if (Player is null)
            return;

        if (quest.Enemy is not null)
            FightEnemy(quest.Enemy, quest.Index);

        if (quest.PowerUpScore is not 0)
            Player.ApplyPowerUpScore(quest.PowerUpScore);

        if (quest is not null && quest.Item is not null)
            Player.AddInventoryItem(quest.Item);
    }

    public void GameMenu(string questIndex)
    {
        _userInteraction.ClearConsole();
        _userInteraction.Print(_stringHelper.GetGameMenuString());

        var keyActions = SetupKeyActionsInGameMenu(questIndex);

        var input = _userInteraction.GetChar();

        if (keyActions.TryGetValue(input.Key, out var action))
            //if (input.Key is ConsoleKey.D4 || input.Key is ConsoleKey.NumPad4)
            //    return QuitGame();
            //else
                action.Invoke();
        else
            GameMenu(questIndex);
    }

    public void LoadGame()
    {
    //    PrintSavedGames();

        int slotNumber = CheckForValidSlotNumber();
        var text = string.Empty;
        try
        {
            text += _fileHelper.GetSavedGameFromFile(slotNumber);
        }
        catch (FileNotFoundException ex)
        {
            _userInteraction.Print($"File doesn't exist: {ex.Message}");
            return;
        }
        var chosenGame = DeserializeSavedGame(text);
        if (chosenGame is null)
            return;
        Player = chosenGame.Player;
        StartGame(chosenGame.QuestIndex);
    }

    private int CheckForValidSlotNumber()
    {
        int slotNumber;
        while (true)
        {
            var input = _userInteraction.GetChar().KeyChar;
            slotNumber = (int)char.GetNumericValue(input);

            if (input >= MinSlotNumber && input <= MaxSlotNumber)
            {
                break;
            }
            else
            {
                _userInteraction.Print($"Invalid slot number. \r\nPlease enter a number between 1 and 3");
            }
        }
        return slotNumber;
    }

    public void FightEnemy(Enemy enemy, string questIndex)
    {
        if (Player is null)
            return;

        var strings = new List<string>
        {
            _stringHelper.GetEnemyForFightLog(enemy)
        };

        while (Player.HealthPoints > 0 && enemy.HealthPoints > 0)
        {
            Player.PerformAttack(enemy);
            strings.Add(_stringHelper.GetActionForFightLog(Player, enemy));

            enemy.PerformAttack(Player);
            strings.Add(_stringHelper.GetActionForFightLog(enemy, Player));
        }

        if (Player.HealthPoints <= 0)
        {
            strings.Add(_stringHelper.GetSurvivorForFightLog(enemy));
            var fightLog = _stringHelper.BuildString(strings);
            _userInteraction.Print(fightLog);

            Player.HealthPoints = MAX_HEALTH_POINTS;
            _userInteraction.GetChar();
            SaveGame(questIndex);
            QuitGame();
        }
        else
        {

            strings.Add(_stringHelper.GetSurvivorForFightLog(Player));
            var fightLog = _stringHelper.BuildString(strings);
            _userInteraction.Print(fightLog);
        }
    }

    private void QuitGame()
    {
        _userInteraction.Print("Game is shutting down");
        Environment.Exit(0);
    }

    public void SaveGame(string questIndex)
    {
        //var choice = _userInteraction.GetChar().KeyChar;

        //var jsonString = SerializeSavedGame(questIndex);

        //if (jsonString is not null)
        //{
        //    var isSaved = WriteToFile(choice, jsonString);

        //    if (isSaved)
        //    {
        //        var savedGame = DeserializeSavedGame(jsonString);
        //        if (savedGame is not null)
        //        {
        //            var text = _stringHelper.GetConfirmationStringForSavedGame(savedGame);
        //            _userInteraction.Print(text);
        //        }
        //    }
        //}
        //_userInteraction.GetChar();
        //GameMenu(questIndex);
    }

    public bool WriteToFile(char choice, string jsonString)
    {
        try
        {
            if (choice >= MIN_SLOT_NUMBER && choice <= MAX_SLOT_NUMBER)
            {
                _fileHelper.WriteAllText(jsonString, choice);
                return true;
            }
            throw new SlotNumberOutOfBoundsException("Slot number was out of bounds");
        }
        catch (SlotNumberOutOfBoundsException ex)
        {
            _userInteraction.Print(ex.Message);
            throw;
        }
    }

    public string? SerializeSavedGame(string questIndex)
    {
        if (Player is null)
            return null;

        var savedGame = new SavedGame(Player, questIndex);
        return _savedGameService.CreateSavedGame(savedGame);
    }

    //public SavedGame? DeserializeSavedGame(string jsonString) =>
    //    _savedGameService.GetSavedGame(jsonString);


    #region Prints
    private void PrintQuestWithOverlayAndInventory(Quest quest)
    {
        PrintOverlay();
        PrintQuest(quest);
        PrintInventory();
    }

    private bool PrintTheEnd()
    {
        _userInteraction.Print("The end");
        return false;
    }

    private void PrintContinueText() =>
        _userInteraction.Print(_stringHelper.GetContinueText());

    private void PrintInventory()
    {
        var inventory = _stringHelper.GetPlayerInventoryAsString(Player);
        _userInteraction.Print(inventory);
    }

    private void PrintOverlay()
    {
        var menuButton = _stringHelper.GetGameMenuButton();
        _userInteraction.Print(menuButton);

        var statisticsText = _stringHelper.GetCharacterStatisticsString(Player);
        _userInteraction.Print(statisticsText);

        var moralityText = _stringHelper.GetMoralityScaleFromPlayerMoralitySpectrum(Player.MoralitySpectrum);
        _userInteraction.Print(moralityText);
    }

    //public void PrintSavedGames()
    //{
    //    var savedGames = new List<SavedGame>();

    //    var files = _fileHelper.GetAllSavedGameFilesFromDirectory();
    //    if (files is null)
    //        return;
    //    foreach (var filePath in files)
    //    {
    //        var jsonContent = _fileHelper.GetSavedGameFromFile(filePath);
    //        var savedGame = new SavedGame();
    //        if (!string.IsNullOrEmpty(jsonContent))
    //        {
    //            savedGame = _savedGameService.GetSavedGame(jsonContent);
    //        }
    //        savedGames.Add(savedGame ?? new SavedGame());
    //    }

    //    var text = _stringHelper.GetSavedGamesString(savedGames);
    //    _userInteraction.Print(text);
    //}

    public Player CreatePlayer()
    {
        _userInteraction.ClearConsole();
        var nameMessage = _stringHelper.GetPlayerNameMessage();
        var name = _userInteraction.GetInput(nameMessage);
        name = _stringHelper.TrimInputString(name);
        while (string.IsNullOrEmpty(name))
        {
            _userInteraction.ClearConsole();
            var noInputMessage = _stringHelper.GetNoNameInputMessage();
            name = _userInteraction.GetInput(noInputMessage);
            name = _stringHelper.TrimInputString(name);
        }

        return new Player(name);
    }
    #endregion
}