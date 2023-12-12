namespace PathsOfPower.Core;

public class PathsOfPowerApp
{
    private readonly IFactory _factory;
    private readonly IUserInteraction _userInteraction;
    private readonly IStringHelper _stringHelper;
    private readonly IQuestService _questService;
    private readonly IFileHelper _fileHelper;
    private readonly ISavedGameService _savedGameService;

    public PathsOfPowerApp(
        IFactory factory,
        IUserInteraction userInteraction,
        IStringHelper stringHelper,
        IQuestService questService,
        IFileHelper fileHelper,
        ISavedGameService savedGameService)
    {
        _factory = factory;
        _userInteraction = userInteraction;
        _stringHelper = stringHelper;
        _questService = questService;
        _fileHelper = fileHelper;
        _savedGameService = savedGameService;
    }

    public void Run()
    {
        var isExiting = false;
        while (isExiting is false)
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
                    ExitApplication();
                    isExiting = true;
                    break;
                default:
                    break;
            }
        }
    }

    private void StartNewGame(string questIndex)
    {
        var (player, quests) = SetupNewGame();
        if (quests is null)
            return;
        var quest = _questService.GetQuestFromIndex(questIndex, quests);
        var game = _factory.CreateGame(quests, player, quest, _userInteraction, _stringHelper, _fileHelper, _questService, _savedGameService);

        var keyActionsGoToGameMenu = GetKeyActionsGoToGameMenu(game);

        var currentChapter = int.Parse(questIndex);

        game.RunLoop(ref currentChapter, keyActionsGoToGameMenu);
    }

    public void LoadGame()
    {
        PrintSavedGames();
        var input = _userInteraction.GetChar().KeyChar;
        (var savedGame, var message) = _savedGameService.LoadGame(input);

        while (savedGame is null)
        {
            _userInteraction.ClearConsole();
            PrintSavedGames();
            _userInteraction.Print(message);
            input = _userInteraction.GetChar().KeyChar;
            (savedGame, message) = _savedGameService.LoadGame(input);
        }

        var player = savedGame.Player;
        var chapter = int.Parse(savedGame.QuestIndex[..1]);
        var quests = _questService.GetQuestsFromChapter(chapter);
        var quest = _questService.GetQuestFromIndex(savedGame.QuestIndex, quests);

        var game = _factory.CreateGame(quests, player, quest, _userInteraction, _stringHelper, _fileHelper, _questService, _savedGameService);

        var keyActions = GetKeyActionsGoToGameMenu(game);

        game.RunLoop(ref chapter, keyActions);
    }

    private void ExitApplication()
    {
        _userInteraction.Print("Game is shutting down");
        Environment.Exit(0);
    }

    public void PrintSavedGames()
    {
        var savedGames = _factory.CreateSavedGames();

        var files = _fileHelper.GetAllSavedGameFilesFromDirectory();
        if (files is null)
            return;
        foreach (var filePath in files)
        {
            var jsonContent = _fileHelper.GetSavedGameFromFile(filePath);
            var savedGame = _factory.CreateSavedGame();
            if (!string.IsNullOrEmpty(jsonContent))
            {
                savedGame = _savedGameService.GetSavedGame(jsonContent);
            }
            savedGames.Add(savedGame ?? _factory.CreateSavedGame());
        }

        var text = _stringHelper.GetSavedGames(savedGames);
        _userInteraction.Print(text);
    }

    private Dictionary<ConsoleKey, Action> GetKeyActionsGoToGameMenu(Game game) =>
        new() { { ConsoleKey.M, game.GoToGameMenu } };

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
        name = _stringHelper.TrimInput(name);
        

        while (string.IsNullOrEmpty(name))
        {
            _userInteraction.ClearConsole();
            var noInputMessage = _stringHelper.GetNoNameInputMessage();
            name = _userInteraction.GetInput(noInputMessage);
            name = _stringHelper.TrimInput(name);
        }

        return _factory.CreatePlayer(name);
    }

    private void PrintMenu()
    {
        var menu = _stringHelper.GetMainMenu();
        _userInteraction.Print(menu);
    }
}
