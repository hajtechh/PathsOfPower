using PathsOfPower.Cli;
using PathsOfPower.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        var menuChoice = _userInteraction.GetKey();
        switch (menuChoice)
        {
            case ConsoleKey.D1:
            case ConsoleKey.NumPad1:
                StartGame();
                break;
            case ConsoleKey.D2:
            case ConsoleKey.NumPad2:
                LoadGame();
                break;
            case ConsoleKey.D3:
            case ConsoleKey.NumPad3:
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
        var quests = CreateSomeQuests();
        var quest = GetQuestFromIndex("1", quests);

        var isRunning = true;
        while (isRunning)
        {
            _userInteraction.ClearConsole();

            PrintQuest(quest);

            if (quest.Options is not null /* || quest.Options.Count() > 0*/)
            {
                var choice = _userInteraction.GetKeyChar();
                var index = CreateQuestIndex(quest.Index, choice.ToString());
                quest = GetQuestFromIndex(index, quests);
            }
            else
            {
                isRunning = false;
                _userInteraction.Print("The end");
            }

        }
    }

    private string CreateQuestIndex(string parentQuestIndex, string choice)
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

    private List<Quest> CreateSomeQuests()
    {
        return new List<Quest>()
        {
            new Quest()
            {
                Index = "1",
                Description = "You are in a classroom at Hogwarts. What do you want to teach your students today?",
                Options = new List<Option>()
                {
                    new Option()
                    {
                        Index = 1,
                        Name = "Boggarts"
                    },
                    new Option()
                    {
                        Index = 2,
                        Name = "Unforgivable curses"
                    }
                }
            },
            new Quest()
            {
                Index = "1.1",
                Description = "You teach the class about boggarts."
            },
            new Quest()
            {
                Index = "1.2",
                Description = "You teach the class about the unforgivable curses."
            }
        };
    }
}
