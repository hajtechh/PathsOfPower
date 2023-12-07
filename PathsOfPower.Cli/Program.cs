using PathsOfPower.Core;
using PathsOfPower.Core.Helpers;
using PathsOfPower.Core.Services;

namespace PathsOfPower.Cli;

public class Program
{
    public static void Main()
    {
        IConsoleWrapper consoleWrapper = new ConsoleWrapper();
        IUserInteraction userInteraction = new UserInteraction(consoleWrapper);
        var graphics = new Graphics();
        IFileHelper fileHelper = new FileHelper();
        IJsonHelper jsonHelper = new JsonHelper();
        IQuestService questService = new QuestService(jsonHelper);
        var game = new Game(userInteraction, graphics, fileHelper, questService, jsonHelper);
        game.Run();
    }
}
