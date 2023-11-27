using PathsOfPower.Cli;
using PathsOfPower.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

        var menuChoice = _userInteraction.GetChar();
        switch (menuChoice)
        {
            case '1':
                Setup();
                StartGame(1);
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

    private void QuitGame()
    {
        Environment.Exit(0);
    }

    public void SaveGame(string questIndex)
    {
        // userinteraction
        PrintSavedGames();
        var choice = _userInteraction.GetChar();

        //Prepare objekt
        var jsonString = SerializeSavedGame(questIndex);

        // save game
        WriteToFile(choice, jsonString);
    }

    public void PrintSavedGames()
    {
        string savedGamesDirectory = _basePath + "SavedGameFiles/";
        List<SavedGame> savedGames = new List<SavedGame>();

        foreach (string filePath in Directory.GetFiles(savedGamesDirectory, "*.json"))
        {
            string jsonContent = File.ReadAllText(filePath);
            var savedGame = new SavedGame();
            if (!string.IsNullOrEmpty(jsonContent))
            {
                savedGame = JsonSerializer.Deserialize<SavedGame>(jsonContent);
            }
            else
            {
                savedGame = new SavedGame();
            }
            savedGames.Add(savedGame);
        }

        _userInteraction.Print("Choose slot\r\n");
        var counter = 1;
        foreach (SavedGame savedGame in savedGames)
        {
            if (savedGame.Character != null)
            {
                _userInteraction.Print($"[{counter}] {savedGame.Character.Name}");
            }
            else
            {
                _userInteraction.Print($"[{counter}] Empty slot");
            }
            _userInteraction.Print("-------");
            counter++;
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

    private void LoadGame()
    {
        PrintSavedGames();
    }

    private void PrintMenu()
    {
        _userInteraction.Print($"[1] Start new game \r\n" +
            $"[2] Load game \r\n" +
            $"[3] Quit game");
    }

    private void StartGame(int chapter)
    {
        Quests = GetQuests(chapter);

        var quest = GetQuestFromIndex(chapter.ToString(), Quests);

        var isRunning = true;
        while (isRunning)
        {
            _userInteraction.ClearConsole();

            PrintQuest(quest);

            if (quest.Options is not null /* || quest.Options.Count() > 0*/)
            {
                var choice = _userInteraction.GetChar();
                var index = CreateQuestIndex(quest.Index, choice);
                quest = GetQuestFromIndex(index, Quests);
            }
            else if (File.Exists($"{_baseQuestPath}{chapter + 1}.json"))
            {
                chapter++;
                Quests = GetQuests(chapter);
                quest = GetQuestFromIndex(chapter.ToString(), Quests);
                _userInteraction.GetChar();
            }
            else
            {
                isRunning = false;
                _userInteraction.Print("The end");
            }

        }
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
        //Quests = GetQuests(1);
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
