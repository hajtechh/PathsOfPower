using PathsOfPower.Cli;
using PathsOfPower.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PathsOfPower;

public class Game
{
    private readonly IUserInteraction _userInteraction;
    const string _basePath = "../../../../PathsOfPower/";
    private readonly string _baseQuestPath = _basePath + "Quests/chapter";
    private readonly string _baseSavePath = _basePath + "SavedGameFiles/slot";
    public List<Quest> Quests { get; set; }
    public Character Character { get; set; }

    public Game(IUserInteraction userInteraction)
    {
        _userInteraction = userInteraction;
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
        Dictionary<ConsoleKey, Action> keyActions = new Dictionary<ConsoleKey, Action>
        {
            { ConsoleKey.Q, QuitGame },
            {ConsoleKey.S, () => SaveGame(quest.Index) } // funkar inte på slutquests
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

            PrintQuest(quest);

            if (quest.Options is not null /* || quest.Options.Count() > 0*/)
            {
                var choice = _userInteraction.GetChar();
                if (char.IsDigit(choice.KeyChar))
                {
                    var index = CreateQuestIndex(quest.Index, choice.KeyChar);
                    quest = GetQuestFromIndex(index, Quests);
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
                chapter++;
                Quests = GetQuests(chapter);
                quest = GetQuestFromIndex(chapter.ToString(), Quests);
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
        var path = $"{_baseSavePath}{slotNumber}.json";
        var text = ReadFromFile(path);
        var chosenGame = DeserializeSavedGame(text);
        Character = chosenGame.Character;
        StartGame(chosenGame.QuestIndex);
    }

    public string? ReadFromFile(string path)
    {
        return File.ReadAllText(path);
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

        WriteToFile(choice, jsonString);
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

        _userInteraction.Print("Choose slot\r\n");
        for (int i = 0; i < savedGames.Count; i++)
        {
            var text = $"[{i + 1}] ";
            text += savedGames[i].Character != null ?
                savedGames[i].Character.Name :
                "Empty slot";
            _userInteraction.Print($"{text} \r\n -------");
        }
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
                Character = Character,
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
        var jsonText = File.ReadAllText($"{_baseQuestPath}{chapterNumber}.json");
        return JsonSerializer.Deserialize<List<Quest>>(jsonText);
    }

    public void Setup()
    {
        Character = CreateCharacter();
    }

    public Character CreateCharacter()
    {
        _userInteraction.ClearConsole();
        var name = _userInteraction.GetInput("Choose the name of your character.");
        while (string.IsNullOrEmpty(name))
        {
            _userInteraction.ClearConsole();
            name = _userInteraction.GetInput("Your character have to have a name.");
        }

        return new Character()
        {
            Name = name
        };
    }
}
