using PathsOfPower.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathsOfPower;

public class Game
{
    public void Run()
    {
        PrintMenu();

        var menuChoice = GetInput();
        switch (menuChoice)
        {
            case "1":
                StartGame();
                break;
            case "2":
                LoadGame();
                break;
            case "3":
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

    private static void PrintMenu()
    {
        Console.WriteLine($"[1] Start new game \r\n" +
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
            Console.Clear();

            PrintQuest(quest);

            if (quest.Options is not null /* || quest.Options.Count() > 0*/)
            {
                var choice = GetInput();
                var index = CreateQuestIndex(quest.Index, choice);
                quest = GetQuestFromIndex(index, quests);
            }
            else
            {
                isRunning = false;
                Console.WriteLine("The end");
            }

        }
    }

    private string CreateQuestIndex(string parentQuestIndex, string choice)
    {
        return $"{parentQuestIndex}.{choice}";
    }

    private string GetInput()
    {
        return Console.ReadKey(true).KeyChar.ToString();
    }

    private void PrintQuest(Quest quest)
    {
        Console.WriteLine(quest.Description);
        Console.WriteLine("---------------------");

        if (quest.Options is null)
            return;

        foreach (var option in quest.Options)
        {
            Console.WriteLine($"[{option.Index}] - {option.Name}");
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
