using PathsOfPower.Core.Models;

namespace PathsOfPower.Core;

public class Game
{
    #region PrivateVariables
    private const int MAX_HEALTH_POINTS = 100;

    private readonly IUserInteraction _userInteraction;
    private readonly IStringHelper _stringHelper;
    private readonly IFileHelper _fileHelper;
    private readonly IQuestService _questService;
    private readonly ISavedGameService _savedGameService;
    #endregion

    public List<Quest> Quests { get; set; }
    public Quest Quest { get; set; }
    public Player Player { get; set; }
    public bool IsExitingGameLoop { get; set; }

    public Game(List<Quest> quests, Player player, Quest quest,
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
        Quest = quest;
    }

    private Dictionary<ConsoleKey, Action> SetupKeyActionsInGameMenu() =>
        new()
        {
            { ConsoleKey.D2, SaveGame },
            { ConsoleKey.NumPad2, SaveGame },
            { ConsoleKey.D3, QuitToMainMenu },
            { ConsoleKey.NumPad3, QuitToMainMenu },
            { ConsoleKey.D4, QuitGame },
            { ConsoleKey.NumPad4, QuitGame }
        };

    public void GameLoop(ref int chapter, Dictionary<ConsoleKey, Action> keyActions)
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
        ;
        Quests = _questService.GetQuestsFromChapter(chapter);

        if (Quests is not null)
            Quest = _questService.GetQuestFromIndex(chapter.ToString(), Quests);

        HandleQuestEvents();

        PrintContinueText();

        var input = _userInteraction.GetChar();
        CheckIfUserWantsToGoToGameMenu(keyActions, input);
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
            CheckIfUserWantsToGoToGameMenu(keyActions, choice);
        }
    }

    private void GetNextQuestBasenOnChosenOption(ConsoleKeyInfo choice)
    {
        var index = _stringHelper.GetQuestIndexString(Quest.Index, choice.KeyChar);
        if (Quests is not null)
            Quest = _questService.GetQuestFromIndex(index, Quests);
    }

    private void CheckIfUserWantsToGoToGameMenu(Dictionary<ConsoleKey, Action> keyActions, ConsoleKeyInfo input)
    {
        if (keyActions.TryGetValue(input.Key, out var action))
            action.Invoke();
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

    public void GameMenu()
    {
        _userInteraction.ClearConsole();
        _userInteraction.Print(_stringHelper.GetGameMenuString());

        var keyActions = SetupKeyActionsInGameMenu();

        var input = _userInteraction.GetChar();

        if (keyActions.TryGetValue(input.Key, out var action))
            //if (input.Key is ConsoleKey.D4 || input.Key is ConsoleKey.NumPad4)
            //    return QuitGame();
            //else
            action.Invoke();
        //else
        //    GameMenu(questIndex);
    }

    //public void LoadGame()
    //{
    //    PrintSavedGames();

    //    var input = _userInteraction.GetChar().KeyChar;
    //    var slotNumber = (int)char.GetNumericValue(input);
    //    var text = string.Empty;
    //    try
    //    {
    //        text += _fileHelper.GetSavedGameFromFile(slotNumber);
    //    }
    //    catch (FileNotFoundException ex)
    //    {
    //        _userInteraction.Print($"File doesn't exist: {ex.Message}");
    //        return;
    //    }
    //    var chosenGame = DeserializeSavedGame(text);
    //    if (chosenGame is null)
    //        return;
    //    Player = chosenGame.Player;
    //    //StartGame(chosenGame.QuestIndex);
    //}

    public void FightEnemy(Enemy enemy)
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
            SaveGame();
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

    public void SaveGame()
    {
        PrintSavedGames();

        var choice = _userInteraction.GetChar().KeyChar;

        var (isSaved, message) = _savedGameService.SaveGame(Player, choice, Quest.Index);

        if (isSaved)
            _userInteraction.Print(message);

        _userInteraction.Print(_stringHelper.GetContinueText());
        _userInteraction.GetChar();
        GameMenu();
    }

    public void QuitToMainMenu() =>
        IsExitingGameLoop = true;

    #region Prints
    private void PrintQuestWithOverlayAndInventory()
    {
        PrintOverlay();
        PrintQuest();
        PrintInventory();
    }

    private void TheEnd()
    {
        _userInteraction.Print("The end");
        IsExitingGameLoop = true;
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

    public void PrintSavedGames()
    {
        var savedGames = _savedGameService.GetSavedGames();
        var output = _stringHelper.GetSavedGamesString(savedGames);
        _userInteraction.Print(output);
    }

    private void PrintQuest()
    {
        var text = _stringHelper.GetQuestWithOptions(Quest);
        _userInteraction.Print(text);
    }
    #endregion
}