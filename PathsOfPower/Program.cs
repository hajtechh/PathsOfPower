using PathsOfPower;
using PathsOfPower.Cli;
using PathsOfPower.Helpers;
using PathsOfPower.Interfaces;

IConsoleWrapper consoleWrapper  = new ConsoleWrapper();
IUserInteraction userInteraction = new UserInteraction(consoleWrapper);
var graphics = new Graphics();
IFileHelper fileHelper = new FileHelper();
var game = new Game(userInteraction, fileHelper, graphics);
game.Run();