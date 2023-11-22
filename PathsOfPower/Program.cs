
using PathsOfPower.Models;

var quests = new List<Quest>()
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


var questOne = quests.FirstOrDefault(x => x.Index == "1");
Console.WriteLine(questOne.Description);
Console.WriteLine("---------------------");
foreach (var option in questOne.Options)
{
    Console.WriteLine($"[{option.Index}] - {option.Name}");
}

var choice = Console.ReadLine();

var finishingQuest = quests.FirstOrDefault(x => x.Index == questOne.Index + "." + choice);
Console.WriteLine(finishingQuest.Description);
Console.WriteLine("The end");
