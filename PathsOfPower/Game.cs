using PathsOfPower.Cli;
using PathsOfPower.Models;
using System.Text.Json;
using PathsOfPower.Interfaces;

namespace PathsOfPower;

public class Game
{
    private readonly IUserInteraction _userInteraction;
    private readonly IFileHelper _fileHelper;

    public List<Quest> Quests { get; set; }
    public Player Player { get; set; }

    public Game(IUserInteraction userInteraction, IFileHelper fileHelper)
    {
        _userInteraction = userInteraction;
        _fileHelper = fileHelper;
    }
    public void Run()
    {
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
        var keyActions = new Dictionary<ConsoleKey, Action>
        {
            { ConsoleKey.Q, QuitGame },
            {ConsoleKey.S, () => SaveGame(quest.Index) }
        };
        while (isRunning)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true);

                if (keyActions.TryGetValue(key.Key, out Action action))
                {
                    action.Invoke();
                }
            }
            _userInteraction.ClearConsole();

            PrintQuest(quest);

            if (quest.Options is not null)
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
            else if (_fileHelper.IsNextChapterExisting(chapter))
            {
                if (quest.Enemy != null)
                {
                    _userInteraction.GetChar();
                    FightEnemy(quest.Enemy, quest.Index);
                }
                if(quest.PowerUpScore != null)
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

    public void LoadGame()
    {
        PrintSavedGames();

        var slotNumber = _userInteraction.GetChar().KeyChar;
        string? text;
        try
        {
            text = _fileHelper.GetSavedGameFromFile(slotNumber);
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

    public void ApplyMoralityScore(int? moralityScore)
    {
        Player.MoralitySpectrum += moralityScore ?? 0;
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
    }

    public void PrintSavedGames()
    {
        var savedGames = new List<SavedGame>();

        var files = _fileHelper.GetAllSavedGameFilesFromDirectory();
        foreach (var filePath in files)
        {
            var jsonContent = _fileHelper.GetSavedGameFromFile(filePath);
            var savedGame = new SavedGame();
            if (!string.IsNullOrEmpty(jsonContent))
            {
                savedGame = JsonSerializer.Deserialize<SavedGame>(jsonContent);
            }
            savedGames.Add(savedGame ?? new SavedGame());
        }

        _userInteraction.Print("Choose slot\r\n");
        for (int i = 0; i < savedGames.Count; i++)
        {
            var text = $"[{i + 1}] ";
            text += savedGames[i].Player != null ?
                savedGames[i].Player.Name :
                "Empty slot";
            _userInteraction.Print($"{text} \r\n -------");
        }
    }

    public void WriteToFile(char choice, string jsonString)
    {
        _fileHelper.WriteAllText(jsonString, choice);
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
        _userInteraction.Print($"[1] Start new game \r\n" +
            $"[2] Load game \r\n" +
            $"[3] Quit game");
    }

    private string CreateQuestIndex(string parentQuestIndex, char choice)
    {
        return $"{parentQuestIndex}.{choice}";
    }

    private void PrintQuest(Quest quest)
    {
        _userInteraction.Print(quest.Description);
        _userInteraction.Print("---------------------");

        if (quest.Options is null)
            return;

        foreach (var option in quest.Options)
        {
            _userInteraction.Print($"[{option.Index}] - {option.Name}");
        }
    }

    private Quest? GetQuestFromIndex(string index, List<Quest> quests)
    {
        return quests.FirstOrDefault(x => x.Index == index);
    }

    public List<Quest> GetQuests(int chapterNumber)
    {
        var jsonText = _fileHelper.GetSavedGameFromFile(chapterNumber);
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
