using PathsOfPower;
using PathsOfPower.Cli;
using PathsOfPower.Cli.Interfaces;
using PathsOfPower.Helpers;
using PathsOfPower.Interfaces;

IConsoleWrapper consoleWrapper  = new ConsoleWrapper();
IUserInteraction userInteraction = new UserInteraction(consoleWrapper);
IFileHelper fileHelper = new FileHelper();
var game = new Game(userInteraction, fileHelper);
game.Run();