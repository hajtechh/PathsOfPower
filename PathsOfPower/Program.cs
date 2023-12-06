using PathsOfPower;
using PathsOfPower.Cli;
using PathsOfPower.Helpers;
using PathsOfPower.Interfaces;

IConsoleWrapper consoleWrapper  = new ConsoleWrapper();
IUserInteraction userInteraction = new UserInteraction(consoleWrapper);
IFileHelper fileHelper = new FileHelper();
IJsonHelper jsonHelper = new JsonHelper();
var game = new Game(userInteraction, fileHelper, jsonHelper);
game.Run();