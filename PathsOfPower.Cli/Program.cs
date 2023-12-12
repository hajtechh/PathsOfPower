namespace PathsOfPower.Cli;

public class Program
{
    public static void Main()
    {
        IFactory factory = new Factory();

        IConsoleWrapper consoleWrapper = new ConsoleWrapper();
        IUserInteraction userInteraction = new UserInteraction(consoleWrapper);

        IStringHelper stringHelper = new StringHelper();
        IFileHelper fileHelper = new FileHelper();
        IJsonHelper jsonHelper = new JsonHelper();

        IQuestService questService = new QuestService(jsonHelper, fileHelper);
        ISavedGameService savedGameService = new SavedGameService(jsonHelper, fileHelper, factory);

        var pathsOfPowerApp = new PathsOfPowerApp(factory, userInteraction, stringHelper, questService, fileHelper, savedGameService);
        pathsOfPowerApp.Run();
    }
}
