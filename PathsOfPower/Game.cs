using PathsOfPower.Cli;
using PathsOfPower.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PathsOfPower;

public class Game
{
    private readonly IUserInteraction _userInteraction;
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
                StartGame();
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

    private void LoadGame()
    {
        throw new NotImplementedException();
    }

    private void PrintMenu()
    {
        _userInteraction.Print($"[1] Start new game \r\n" +
            $"[2] Load game \r\n" +
            $"[3] Quit game");
    }

    private void StartGame()
    {
        //setup game
        var quests = GetQuests(1);
        var quest = GetQuestFromIndex("1", quests);

        var isRunning = true;
        while (isRunning)
        {
            _userInteraction.ClearConsole();

            PrintQuest(quest);

            if (quest.Options is not null /* || quest.Options.Count() > 0*/)
            {
                var choice = _userInteraction.GetChar();
                var index = CreateQuestIndex(quest.Index, choice);
                quest = GetQuestFromIndex(index, quests);
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
        //var text = File.ReadAllText($"./Quests/chapter{chapterNumber}.json");
        var jsonText = File.ReadAllText($"../../../../PathsOfPower/Quests/chapter{chapterNumber}.json");
        return JsonSerializer.Deserialize<List<Quest>>(jsonText);
    }
}
