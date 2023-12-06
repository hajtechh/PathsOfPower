using PathsOfPower;
using PathsOfPower.Cli;
using PathsOfPower.Cli.Interfaces;
using PathsOfPower.Helpers;
using PathsOfPower.Interfaces;
using PathsOfPower.Services;

IConsoleWrapper consoleWrapper  = new ConsoleWrapper();
IUserInteraction userInteraction = new UserInteraction(consoleWrapper);
var graphics = new Graphics();
IFileHelper fileHelper = new FileHelper();
IJsonHelper jsonHelper = new JsonHelper();
IQuestService questService = new QuestService(jsonHelper);
var game = new Game(userInteraction, graphics, fileHelper, questService, jsonHelper);
game.Run();