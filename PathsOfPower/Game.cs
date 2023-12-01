using PathsOfPower.Cli;
using PathsOfPower.Models;
using PathsOfPower.Interfaces;
using System.Text.Json;

namespace PathsOfPower;

public class Game
{
    private readonly IUserInteraction _userInteraction;
    private readonly Graphics _graphics;
    const string _basePath = "../../../../PathsOfPower/";
    private readonly string _baseQuestPath = _basePath + "Quests/chapter";
    private readonly string _baseSavePath = _basePath + "SavedGameFiles/slot";
    public List<Quest> Quests { get; set; }
    public Player Player { get; set; }

    public Game(IUserInteraction userInteraction, Graphics graphics)
    {
        _userInteraction = userInteraction;
        _graphics = graphics;
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
        var currentChapter = questIndex.Substring(0, 1);
        int chapter = int.Parse(currentChapter);
        Quests = GetQuests(chapter);

        var quest = GetQuestFromIndex(questIndex, Quests);

        var isRunning = true;
        Dictionary<ConsoleKey, Action> keyActions = new Dictionary<ConsoleKey, Action>
        {
            { ConsoleKey.M, () => GameMenu(quest.Index) },
        };
        while (isRunning)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(intercept: true);

                if (keyActions.TryGetValue(key.Key, out Action action))
                {
                    action.Invoke();
                }
            }
            _userInteraction.ClearConsole();

            var menuButton = _graphics.GetGameMenuButton();
            _userInteraction.Print(menuButton);

            var moralityText = _graphics.GetMoralityScaleFromPlayerMoralitySpectrum(Player.MoralitySpectrum);
            _userInteraction.Print(moralityText);

            PrintQuest(quest);

            if (quest.Options is not null /* || quest.Options.Count() > 0*/)
            {
                if (quest.Enemy != null)
                {
                    _userInteraction.GetChar();
                    FightEnemy(quest.Enemy, quest.Index);
                }
                if (quest.PowerUpScore != null)
                {
                    ApplyPowerUpScoreToPlayer(quest.PowerUpScore);
                }

                var choice = _userInteraction.GetChar();
                if (char.IsDigit(choice.KeyChar))
                {
                    var test2 = int.Parse(choice.KeyChar.ToString());
                    var option = quest.Options.FirstOrDefault(x => x.Index == test2);
                    if (option != null && option.MoralityScore != 0)
                    {
                        ApplyMoralityScore(option.MoralityScore);
                    }
                    var index = CreateQuestIndex(quest.Index, choice.KeyChar);
                    quest = GetQuestFromIndex(index, Quests);
                    if (quest.Item is not null)
                    {
                        AddInventoryItem(quest.Item);
                    }
                }
                else
                {
                    if (keyActions.TryGetValue(choice.Key, out Action action))
                    {
                        action.Invoke();
                    }
                }
            }
            else if (File.Exists($"{_baseQuestPath}{chapter + 1}.json"))
            {
                if (quest.Enemy != null)
                {
                    _userInteraction.GetChar();
                    FightEnemy(quest.Enemy, quest.Index);
                }
                if (quest.PowerUpScore != null)
                {
                    ApplyPowerUpScoreToPlayer(quest.PowerUpScore);
                }

                chapter++;
                Quests = GetQuests(chapter);
                quest = GetQuestFromIndex(chapter.ToString(), Quests);
                if (quest != null && quest.Item is not null)
                {
                    AddInventoryItem(quest.Item);
                }
                var input = _userInteraction.GetChar();
                if (keyActions.TryGetValue(input.Key, out Action action))
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

    public void ApplyMoralityScore(int? moralityScore)
    {
        Player.MoralitySpectrum += moralityScore ?? 0;
    }

    public void GameMenu(string questIndex)
    {
        _userInteraction.ClearConsole();
        var text = _graphics.GetGameMenuString();
        _userInteraction.Print(text);

        Dictionary<ConsoleKey, Action> keyActions = new Dictionary<ConsoleKey, Action>
        {
          { ConsoleKey.D1, () => StartGame(questIndex) },
          { ConsoleKey.D2, () => SaveGame(questIndex) },
          { ConsoleKey.D3, Run },
          { ConsoleKey.D4, QuitGame },
        };

        var choice = _userInteraction.GetChar();

        if (keyActions.TryGetValue(choice.Key, out Action action))
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

        var slotNumber = _userInteraction.GetChar().KeyChar;
        var path = $"{_baseSavePath}{slotNumber}.json";
        string? text;
        try
        {
            text = ReadFromFile(path);
        }
        catch (FileNotFoundException ex)
        {
            _userInteraction.Print($"File doesn't exist: {ex.Message}");
            return;
        }
        var chosenGame = DeserializeSavedGame(text);
        Player = chosenGame.Player;
        StartGame(chosenGame.QuestIndex);
    }

    public void ApplyPowerUpScoreToPlayer(int? powerUpScore)
    {
        Player.Power += powerUpScore ?? 0;
    }

    public bool FightEnemy(Enemy enemy, string questIndex)
    {
        while (Player.CurrentHealthPoints > 0 && enemy.CurrentHealthPoints > 0)
        {
            PerformAttack(Player, enemy);
            PerformAttack(enemy, Player);
        }

        if (Player.CurrentHealthPoints <= 0)
        {
            Player.CurrentHealthPoints = Player.MaxHealthPoints;
            SaveGame(questIndex);
            QuitGame();
        }
        return true;
    }

    private void PerformAttack(ICharacter attacker, ICharacter target)
    {
        target.CurrentHealthPoints -= attacker.Power;
    }

    public string? ReadFromFile(string path)
    {
        return File.ReadAllText(path);
    }

    private void QuitGame()
    {
        Environment.Exit(0);
    }

    public void AddInventoryItem(InventoryItem item)
    {
        Player.InventoryItems.Add(item);
    }

    public void SaveGame(string questIndex)
    {
        PrintSavedGames();
        var choice = _userInteraction.GetChar().KeyChar;

        var jsonString = SerializeSavedGame(questIndex);

        WriteToFile(choice, jsonString);

        var savedGame = DeserializeSavedGame(jsonString);
        if (savedGame != null)
        {
            var text = _graphics.GetConfirmationForSavedGame(savedGame);
            _userInteraction.Print(text);
        }
        _userInteraction.GetChar();
        GameMenu(questIndex);
    }

    public void PrintSavedGames()
    {
        var savedGamesDirectory = _basePath + "SavedGameFiles/";
        var savedGames = new List<SavedGame>();

        foreach (var filePath in Directory.GetFiles(savedGamesDirectory, "*.json"))
        {
            var jsonContent = File.ReadAllText(filePath);
            var savedGame = new SavedGame();
            if (!string.IsNullOrEmpty(jsonContent))
            {
                savedGame = JsonSerializer.Deserialize<SavedGame>(jsonContent);
            }
            savedGames.Add(savedGame ?? new SavedGame());
        }

        var text = _graphics.GetSavedGamesString(savedGames);
        _userInteraction.Print(text);
    }

    public void WriteToFile(char choice, string jsonString)
    {
        var path = $"{_baseSavePath}{choice}.json";
        File.WriteAllText(path, jsonString);
    }

    public string SerializeSavedGame(string questIndex)
    {
        return JsonSerializer.Serialize(
            new SavedGame
            {
                Player = Player,
                QuestIndex = questIndex
            });
    }

    public SavedGame? DeserializeSavedGame(string jsonString)
    {
        return JsonSerializer.Deserialize<SavedGame>(jsonString);
    }

    private void PrintMenu()
    {
        var menu = _graphics.GetMenu();
        _userInteraction.Print(menu);
    }

    private string CreateQuestIndex(string parentQuestIndex, char choice)
    {
        return $"{parentQuestIndex}.{choice}";
    }

    private void PrintQuest(Quest quest)
    {
        var text = _graphics.GetQuestWithOptions(quest);
        _userInteraction.Print(text);
    }

    private Quest? GetQuestFromIndex(string index, List<Quest> quests)
    {
        return quests.FirstOrDefault(x => x.Index == index);
    }

    public List<Quest> GetQuests(int chapterNumber)
    {
        var jsonText = File.ReadAllText($"{_baseQuestPath}{chapterNumber}.json");
        return JsonSerializer.Deserialize<List<Quest>>(jsonText);
    }

    public void Setup()
    {
        Player = CreateCharacter();
    }

    public Player CreateCharacter()
    {
        _userInteraction.ClearConsole();
        var name = _userInteraction.GetInput("Choose the name of your character.");
        while (string.IsNullOrEmpty(name))
        {
            _userInteraction.ClearConsole();
            name = _userInteraction.GetInput("Your character have to have a name.");
        }

        return new Player()
        {
            Name = name,
            MoralitySpectrum = 0,
            InventoryItems = new List<InventoryItem>()
        };
    }
}
