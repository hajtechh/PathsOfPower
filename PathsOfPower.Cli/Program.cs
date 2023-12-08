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

        IStringHelper stringHelper = new StringHelper();
        IFileHelper fileHelper = new FileHelper();
        IJsonHelper jsonHelper = new JsonHelper();

        IQuestService questService = new QuestService(jsonHelper, fileHelper);
        ISavedGameService savedGameService = new SavedGameService(jsonHelper);

        var pathsOfPowerApp = new PathsOfPowerApp(userInteraction, stringHelper, questService, fileHelper, savedGameService);
        pathsOfPowerApp.Run();
    }
}
