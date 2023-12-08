namespace PathsOfPower.Core;

public class PathsOfPowerApp
{
    private readonly IUserInteraction _userInteraction;
    private readonly IStringHelper _stringHelper;
    private readonly IQuestService _questService;
    private readonly IFileHelper _fileHelper;
    private readonly ISavedGameService _savedGameService;

    public PathsOfPowerApp(
        IUserInteraction userInteraction,
        IStringHelper stringHelper,
        IQuestService questService,
        IFileHelper fileHelper,
        ISavedGameService savedGameService)
    {
        _userInteraction = userInteraction;
        _stringHelper = stringHelper;
        _questService = questService;
        _fileHelper = fileHelper;
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
                StartNewGame("1");
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

    private void StartNewGame(string questIndex)
    {
        var (player, quests) = SetupNewGame();
        if (quests is null)
            return;
        var game = new Game(quests, player, _userInteraction, _stringHelper, _fileHelper, _questService, _savedGameService);

        var quest = _questService.GetQuestFromIndex(questIndex, quests);

        var keyActions = SetupKeyActionsInGame(quest, game);

        var currentChapter = int.Parse(questIndex);

        game.GameLoop(ref currentChapter, ref quest, keyActions);
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
        var savedGame = _savedGameService.GetSavedGame(text);
        
        if (savedGame is null)
            return;

        var player = savedGame.Player;
        var chapter = int.Parse(savedGame.QuestIndex[..1]);
        var quests = _questService.GetQuestsFromChapter(chapter);
        var quest = _questService.GetQuestFromIndex(savedGame.QuestIndex, quests);

        var game = new Game(quests, player, _userInteraction, _stringHelper, _fileHelper, _questService, _savedGameService);
        
        var keyActions = SetupKeyActionsInGame(quest, game);
        
        game.GameLoop(ref chapter, ref quest, keyActions);
    }
    private void QuitGame()
    {
        _userInteraction.Print("Game is shutting down");
        Environment.Exit(0);
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
    private Dictionary<ConsoleKey, Action> SetupKeyActionsInGame(Quest quest, Game game) =>
        new() { { ConsoleKey.M, () => game.GameMenu(quest.Index) } };

    public (Player player, List<Quest>? quests) SetupNewGame()
    {
        var player = CreatePlayer();
        var quests = _questService.GetQuestsFromChapter(1);
        return (player, quests);
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

    private void PrintMenu()
    {
        var menu = _stringHelper.GetMenu();
        _userInteraction.Print(menu);
    }
}
