using PathsOfPower;
using PathsOfPower.Cli;
using PathsOfPower.Helpers;

IConsoleWrapper consoleWrapper  = new ConsoleWrapper();
IUserInteraction userInteraction = new UserInteraction(consoleWrapper);
IFileHelper fileHelper = new FileHelper();
var game = new Game(userInteraction, fileHelper);
game.Run();