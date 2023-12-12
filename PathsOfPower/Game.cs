namespace PathsOfPower.Core;

public class Game
{
    #region PrivateVariables
    private const int MAX_HEALTH_POINTS = 100;

    private readonly IFactory _factory;
    private readonly IUserInteraction _userInteraction;
    private readonly IStringHelper _stringHelper;
    private readonly IFileHelper _fileHelper;
    private readonly IQuestService _questService;
    private readonly ISavedGameService _savedGameService;
    #endregion

    #region Properties
    public List<Quest> Quests { get; set; }
    public Quest Quest { get; set; }
    public Player Player { get; set; }
    public bool IsExitingGameLoop { get; set; }
    #endregion

    public Game(List<Quest> quests, Player player, Quest quest,
        IFactory factory,
        IUserInteraction userInteraction,
        IStringHelper stringHelper,
        IFileHelper fileHelper,
        IQuestService questService,
        ISavedGameService savedGameService)
    {
        _factory = factory;
        _userInteraction = userInteraction;
        _stringHelper = stringHelper;
        _fileHelper = fileHelper;
        _questService = questService;
        _savedGameService = savedGameService;

        Player = player;
        Quests = quests;
        Quest = quest;
    }

    public Dictionary<ConsoleKey, Action> SetupkeyActionsGoToGameMenu() =>
        new()
        {
            { ConsoleKey.D2, SaveGame },
            { ConsoleKey.NumPad2, SaveGame },
            { ConsoleKey.D3, QuitToMainMenu },
            { ConsoleKey.NumPad3, QuitToMainMenu },
            { ConsoleKey.D4, QuitGame },
            { ConsoleKey.NumPad4, QuitGame }
        };

    /// <summary>
    /// 
    /// Gets the action from Dictionary list of actions.
    /// If input match any ConsoleKey from Dictionary list of ConsoleKeys then invoke the action.
    /// </summary>
    /// <param name="keyActions">Dictionary of ConsoleKey and Action (key = Consolekey, value = Action)</param>
    /// <param name="input">ConsoleKeyInfo of any key pressed by the user</param>
    public void CheckKeyPressFromKeyActions(Dictionary<ConsoleKey, Action> keyActions, ConsoleKeyInfo input)
    {
        var action = GetAction(keyActions, input);
        action?.Invoke();
    }

    public Action? GetAction(Dictionary<ConsoleKey, Action> keyActions, ConsoleKeyInfo input)
    {
        if (keyActions.TryGetValue(input.Key, out var action))
            return action;
        return null;
    }

    public void RunLoop(ref int chapter, Dictionary<ConsoleKey, Action> keyActions)
    {
        while (IsExitingGameLoop is false)
        {
            if (Player is null || Quest is null)
                return;

            _userInteraction.ClearConsole();

            PrintQuestWithOverlayAndInventory();
            HandleQuestEvents();

            if (Quest.Options is not null)
                RunQuestWithOptions(keyActions);
            else if (_fileHelper.IsNextChapterExisting(chapter))
                RunQuestWithoutOptions(chapter + 1, keyActions);
            else
                TheEnd();
        }
    }

    private void RunQuestWithoutOptions(int chapter, Dictionary<ConsoleKey, Action> keyActions)
    {
        Quests = _questService.GetQuestsFromChapter(chapter);

        if (Quests is not null)
            Quest = _questService.GetQuestFromIndex(chapter.ToString(), Quests);

        HandleQuestEvents();

        PrintText(_stringHelper.GetContinueText());

        var input = _userInteraction.GetChar();
        CheckKeyPressFromKeyActions(keyActions, input);
    }

    private void RunQuestWithOptions(Dictionary<ConsoleKey, Action> keyActions)
    {
        var choice = _userInteraction.GetChar();
        if (char.IsDigit(choice.KeyChar))
        {
            GetNextQuestBasenOnChosenOption(choice);
            HandleOptionEventsInQuest(choice);
        }
        else
        {
            CheckKeyPressFromKeyActions(keyActions, choice);
        }
    }

    private void GetNextQuestBasenOnChosenOption(ConsoleKeyInfo choice)
    {
        var index = _stringHelper.GetQuestIndexString(Quest.Index, choice.KeyChar);
        Quest = _questService.GetQuestFromIndex(index, Quests);
    }

    private void HandleOptionEventsInQuest(ConsoleKeyInfo choice)
    {
        if (Player is null)
            return;

        var index = int.Parse(choice.KeyChar.ToString());
        var option = Quest.Options?.FirstOrDefault(x => x.Index == index);
        if (option is null || option.MoralityScore is not 0)
            return;

        Player.ApplyMoralityScore(option.MoralityScore);
    }

    private void HandleQuestEvents()
    {
        if (Player is null)
            return;

        if (Quest.Enemy is not null)
            FightEnemy(Quest.Enemy);

        if (Quest.PowerUpScore is not 0)
            Player.ApplyPowerUpScore(Quest.PowerUpScore);

        if (Quest is not null && Quest.Item is not null)
            Player.AddInventoryItem(Quest.Item);
    }

    public void GoToGameMenu()
    {
        _userInteraction.ClearConsole();
        PrintText(_stringHelper.GetGameMenuString());

        var keyActionsGoToGameMenu = SetupkeyActionsGoToGameMenu();

        var input = _userInteraction.GetChar();

        if (keyActionsGoToGameMenu.TryGetValue(input.Key, out var action))
            action.Invoke();
    }

    public void FightEnemy(Enemy enemy)
    {
        if (Player is null)
            return;

        var fightLog = new List<string>
        {
            _stringHelper.GetEnemyForFightLog(enemy)
        };

        while (Player.HealthPoints > 0 && enemy.HealthPoints > 0)
        {
            Player.PerformAttack(enemy);
            fightLog.Add(_stringHelper.GetActionForFightLog(Player, enemy));

            enemy.PerformAttack(Player);
            fightLog.Add(_stringHelper.GetActionForFightLog(enemy, Player));
        }

        if (Player.HealthPoints <= 0)
        {
            fightLog.Add(_stringHelper.GetSurvivorForFightLog(enemy));
            PrintText(_stringHelper.BuildString(fightLog));

            Player.HealthPoints = MAX_HEALTH_POINTS;
            _userInteraction.GetChar();
            SaveGame();
            QuitGame();
        }
        else
        {
            fightLog.Add(_stringHelper.GetSurvivorForFightLog(Player));
            PrintText(_stringHelper.BuildString(fightLog));
        }
    }

    public void QuitGame()
    {
        PrintText(_stringHelper.GetExitGame());
        Environment.Exit(0);
    }

    public void SaveGame()
    {
        var savedGames = _savedGameService.GetSavedGames();
        PrintText(_stringHelper.GetSavedGamesString(savedGames));

        var choice = _userInteraction.GetChar().KeyChar;

        // Saving the game
        var (isSaved, message) = _savedGameService.SaveGame(Player, choice, Quest.Index);

        PrintText($"{message}\r\n{_stringHelper.GetContinueText()}");

        _userInteraction.GetChar();
        GoToGameMenu();
    }

    public void QuitToMainMenu() =>
        IsExitingGameLoop = true;

    private void TheEnd()
    {
        PrintText(_stringHelper.GetTheEndText());
        IsExitingGameLoop = true;
    }

    private void PrintQuestWithOverlayAndInventory()
    {
        PrintText(_stringHelper.GetGameMenuButton());
        PrintText(_stringHelper.GetCharacterStatisticsString(Player));
        PrintText(_stringHelper.GetMoralityScaleFromPlayerMoralitySpectrum(Player.MoralitySpectrum));
        PrintText(_stringHelper.GetQuestWithOptions(Quest));
        PrintText(_stringHelper.GetPlayerInventoryAsString(Player));
    }

    public void PrintText(string text) =>
        _userInteraction.Print(text);
}