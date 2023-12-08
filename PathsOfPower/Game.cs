namespace PathsOfPower.Core;

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

    public List<Quest>? Quests { get; set; }
    public Player? Player { get; set; }

    public Game(IUserInteraction userInteraction,
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
    }

    public void Run()
    {
        _userInteraction.ClearConsole();
        PrintMenu();

        var menuChoice = _userInteraction.GetChar().KeyChar;
        switch (menuChoice)
        {
            case '1':
                Setup();
                StartGame("1");
                break;
            case '2':
                LoadGame();
                break;
            case '3':
                QuitGame();
                break;
            default:
                break;
        }
    }

    private void StartGame(string questIndex)
    {
        var currentChapter = questIndex[..1];
        var chapter = int.Parse(currentChapter);
        Quests = GetQuests(chapter);

        var quest = new Quest();
        if (Quests is not null)
            quest = GetQuestFromIndex(questIndex, Quests);

        var keyActions = SetupKeyActionsInGame(quest);

        GameLoop(ref chapter, ref quest, keyActions);
    }

    private Dictionary<ConsoleKey, Action> SetupKeyActionsInGame(Quest quest) =>
        new() { { ConsoleKey.M, () => GameMenu(quest.Index) } };

    private Dictionary<ConsoleKey, Action> SetupKeyActionsInGameMenu(string questIndex) =>
        new()
        {
            { ConsoleKey.D1, () => StartGame(questIndex) },
            { ConsoleKey.D2, () => SaveGame(questIndex) },
            { ConsoleKey.D3, Run },
            { ConsoleKey.D4, QuitGame },
            { ConsoleKey.NumPad1, () => StartGame(questIndex) },
            { ConsoleKey.NumPad2, () => SaveGame(questIndex) },
            { ConsoleKey.NumPad3, Run },
            { ConsoleKey.NumPad4, QuitGame }
        };

    private void GameLoop(ref int chapter, ref Quest? quest, Dictionary<ConsoleKey, Action> keyActions)
    {
        /*  1. Clear Console
        *   2. Print
        *   3. HandleQuestEvents
        *   4. Options ?
        *       4.1. If true
        *           4.1.1. Get input
        *               4.1.1.1. Get next quest
        *               4.1.1.2. HandleEvent
        *       4.2. else
        *           4.2.1. Check for next chapter
        *               4.2.1.1. Get quests in next chapter
        *               4.2.1.2. Get quest in chapter
        *               4.2.1.3. HandleQuestEvents
        *               4.2.1.4. Print continuetext
        *               4.2.1.5. Check if user wants to go to game menu
        *           4.2.2. No next chapter exists. The end
        *
        */
        var isRunning = true;
        while (isRunning)
        {
            if (Player is null)
                return;

            // 1. Clear console
            _userInteraction.ClearConsole();

            // 2. Prints
            if (quest is null)
                return;
            PrintQuestWithOverlayAndInventory(quest);

            // 3. Handle quest events
            HandleQuestEvents(quest);

            // 4.1. If options exists
            if (quest.Options is not null)
                quest = RunQuestWithOptions(quest, keyActions);
            // 4.2. If option does not exist
            // 4.2.1. Check for next chapter
            else if (_fileHelper.IsNextChapterExisting(chapter))
                quest = RunQuestWithoutOptions(chapter, quest, keyActions);
            // 4.2.2. No next chapter exists, end the game.
            else
                isRunning = PrintTheEnd();
        }
    }

    private Quest RunQuestWithoutOptions(int chapter, Quest quest, Dictionary<ConsoleKey, Action> keyActions)
    {
        // 4.2.1.1. Get quests in next chapter
        Quests = GetQuestInNextChapter(chapter);

        // 4.2.1.2. Get quest in chapter
        if (Quests is not null)
            quest = GetQuestFromIndex(chapter.ToString(), Quests);

        // 4.2.1.3. HandleQuestEvents
        HandleQuestEvents(quest);

        // 4.2.1.4. Print continuetext
        PrintContinueText();

        // 4.2.1.5. Check if user wants to go to game menu
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

    private List<Quest>? GetQuestInNextChapter(int currentChapter) =>
        Quests = GetQuests(currentChapter++);

    private Quest GetNextQuestBasenOnChosenOption(Quest quest, ConsoleKeyInfo choice)
    {
        var index = _stringHelper.GetQuestIndexString(quest.Index, choice.KeyChar);
        if (Quests is not null)
            quest = GetQuestFromIndex(index, Quests);
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

        var choice = _userInteraction.GetChar();

        if (keyActions.TryGetValue(choice.Key, out var action))
        {
            action.Invoke();
        }
        else
        {
            GameMenu(questIndex);
        }
    }

    public void LoadGame()
    {
        PrintSavedGames();

        var input = _userInteraction.GetChar().KeyChar;
        var slotNumber = (int)char.GetNumericValue(input);
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
        Environment.Exit(0);
    }

    public void SaveGame(string questIndex)
    {
        PrintSavedGames();

        var choice = _userInteraction.GetChar().KeyChar;

        var (isSaved, message) = _savedGameService.SaveGame(Player, choice, questIndex);

        if (isSaved)
            _userInteraction.Print(message);

        _userInteraction.GetChar();
        GameMenu(questIndex);
    }


    public bool WriteToFile(char choice, string jsonString)
    {
        if (choice >= MIN_SLOT_NUMBER && choice <= MAX_SLOT_NUMBER)
        {
            _fileHelper.WriteAllText(jsonString, choice);
            return true;
        }
        throw new SlotNumberOutOfBoundsException("Slot number was out of bounds");
    }

    public SavedGame? DeserializeSavedGame(string jsonString) =>
        _savedGameService.GetSavedGame(jsonString);

    private Quest GetQuestFromIndex(string index, List<Quest> quests)
    {
        return quests.First(x => x.Index == index);
    }

    public List<Quest>? GetQuests(int chapterNumber)
    {
        var jsonContent = _fileHelper.GetQuestsFromFile(chapterNumber);
        if (jsonContent is null)
            return null;
        return _questService.GetQuests(jsonContent);
    }

    public void Setup()
    {
        Player = CreatePlayer();
    }

    public Player CreatePlayer()
    {
        _userInteraction.ClearConsole();
        var nameMessage = _stringHelper.GetPlayerNameMessage();
        var name = _userInteraction.GetInput(nameMessage);
        while (string.IsNullOrEmpty(name))
        {
            _userInteraction.ClearConsole();
            var noInputMessage = _stringHelper.GetNoNameInputMessage();
            name = _userInteraction.GetInput(noInputMessage);
        }

        return new Player(name);
    }

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

    public void PrintSavedGames()
    {
        var savedGames = new List<SavedGame>();

        var files = _fileHelper.GetAllSavedGameFilesFromDirectory();
        if (files is null)
            return;
        foreach (var filePath in files)
        {
            var jsonContent = _fileHelper.GetSavedGameFromFile(filePath);
            var savedGame = new SavedGame();
            if (!string.IsNullOrEmpty(jsonContent))
            {
                savedGame = _savedGameService.GetSavedGame(jsonContent);
            }
            savedGames.Add(savedGame ?? new SavedGame());
        }

        var text = _stringHelper.GetSavedGamesString(savedGames);
        _userInteraction.Print(text);
    }

    private void PrintMenu()
    {
        var menu = _stringHelper.GetMenu();
        _userInteraction.Print(menu);
    }

    private void PrintQuest(Quest quest)
    {
        var text = _stringHelper.GetQuestWithOptions(quest);
        _userInteraction.Print(text);
    }
    #endregion
}