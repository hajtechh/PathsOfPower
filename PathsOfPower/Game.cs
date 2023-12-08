namespace PathsOfPower.Core;

public class Game
{
    private const int MaxHealthPoints = 100;
    private const char MinSlotNumber = '1';
    private const char MaxSlotNumber = '3';

    public List<Quest>? Quests { get; set; }
    public Player? Player { get; set; }

    private readonly IUserInteraction _userInteraction;
    private readonly IStringHelper _stringHelper;
    private readonly IFileHelper _fileHelper;
    private readonly IQuestService _questService;
    private readonly ISavedGameService _savedGameService;

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
        int chapter = int.Parse(currentChapter);
        Quests = GetQuests(chapter);

        var quest = new Quest();
        if (Quests is not null)
            quest = GetQuestFromIndex(questIndex, Quests);

        // Setup keyActions
        var keyActions = new Dictionary<ConsoleKey, Action>
        {
            { ConsoleKey.M, () => GameMenu(quest.Index) },
        };
        GameLoop(ref chapter, ref quest, keyActions);
    }

    private void GameLoop(ref int chapter, ref Quest? quest, Dictionary<ConsoleKey, Action> keyActions)
    {
        /*  Clear Console
        *   Print
        *   Options ?
        *      If
        *           Get input
        *               HandleEvent
        *           Get next quest
        *           
        *      else
        *           Check for next chapter
        *               if
        *                   Get next chapter
        *               else
        *                   The end
        *
        */
        var isRunning = true;
        while (isRunning)
        {
            if (Player is null)
                return;

            _userInteraction.ClearConsole();

            PrintOverlay();

            if (quest is null)
                return;

            PrintQuest(quest);

            var inventory = _stringHelper.GetPlayerInventoryAsString(Player);
            _userInteraction.Print(inventory);

            HandleQuestEvents(quest);

            if (quest.Options is not null)
            {
                var choice = _userInteraction.GetChar();
                if (char.IsDigit(choice.KeyChar))
                {
                    var index = _stringHelper.GetQuestIndexString(quest.Index, choice.KeyChar);
                    if (Quests is not null)
                        quest = GetQuestFromIndex(index, Quests);

                    HandleOptionEvents(quest, choice);
                }
                else
                {
                    if (keyActions.TryGetValue(choice.Key, out var action))
                    {
                        action.Invoke();
                    }
                }
            }
            else if (_fileHelper.IsNextChapterExisting(chapter))
            {

                chapter++;
                Quests = GetQuests(chapter);
                if (Quests is not null)
                    quest = GetQuestFromIndex(chapter.ToString(), Quests);

                if (Player is not null && quest is not null && quest.Item is not null)
                {
                    Player.AddInventoryItem(quest.Item);
                }

                var continueText = _stringHelper.GetContinueText();
                _userInteraction.Print(continueText);
                var input = _userInteraction.GetChar();

                if (keyActions.TryGetValue(input.Key, out var action))
                {
                    action.Invoke();
                }
                else
                {
                    continue;
                }
            }
            else
            {
                isRunning = false;
                _userInteraction.Print("The end");
            }
        }
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

    private void HandleOptionEvents(Quest quest, ConsoleKeyInfo choice)
    {
        var index = int.Parse(choice.KeyChar.ToString());
        var option = quest.Options?.FirstOrDefault(x => x.Index == index);
        if (Player is not null && option is not null && option.MoralityScore is not 0)
        {
            Player.ApplyMoralityScore(option.MoralityScore);
        }
    }

    private void HandleQuestEvents(Quest quest)
    {
        if (quest.Enemy is not null)
        {
            FightEnemy(quest.Enemy, quest.Index);
        }
        if (Player is not null && quest.PowerUpScore is not 0)
        {
            Player.ApplyPowerUpScore(quest.PowerUpScore);
        }
        if (Player is not null && quest is not null && quest.Item is not null)
        {
            Player.AddInventoryItem(quest.Item);
        }
    }


    public void GameMenu(string questIndex)
    {
        _userInteraction.ClearConsole();
        var text = _stringHelper.GetGameMenuString();
        _userInteraction.Print(text);

        var keyActions = new Dictionary<ConsoleKey, Action>
        {
          { ConsoleKey.D1, () => StartGame(questIndex) },
          { ConsoleKey.D2, () => SaveGame(questIndex) },
          { ConsoleKey.D3, Run },
          { ConsoleKey.D4, QuitGame },
          { ConsoleKey.NumPad1, () => StartGame(questIndex) },
          { ConsoleKey.NumPad2, () => SaveGame(questIndex) },
          { ConsoleKey.NumPad3, Run },
          { ConsoleKey.NumPad4, QuitGame },
        };

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

            Player.HealthPoints = MaxHealthPoints;
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

        var jsonString = SerializeSavedGame(questIndex);

        if (jsonString is not null)
        {
            var isSaved = WriteToFile(choice, jsonString);

            if (isSaved)
            {
                var savedGame = DeserializeSavedGame(jsonString);
                if (savedGame is not null)
                {
                    var text = _stringHelper.GetConfirmationStringForSavedGame(savedGame);
                    _userInteraction.Print(text);
                }
            }
        }
        _userInteraction.GetChar();
        GameMenu(questIndex);
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

    public bool WriteToFile(char choice, string jsonString)
    {
        try
        {
            if (choice >= MinSlotNumber && choice <= MaxSlotNumber)
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

    public SavedGame? DeserializeSavedGame(string jsonString) =>
        _savedGameService.GetSavedGame(jsonString);

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

}