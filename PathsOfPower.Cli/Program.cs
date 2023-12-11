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
        ISavedGameService savedGameService = new SavedGameService(jsonHelper, fileHelper);

        //IFactory factory = new Factory();

        var pathsOfPowerApp = new PathsOfPowerApp(/*factory,*/ userInteraction, stringHelper, questService, fileHelper, savedGameService);
        pathsOfPowerApp.Run();
    }
}
