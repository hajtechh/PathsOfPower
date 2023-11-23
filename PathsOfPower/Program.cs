using PathsOfPower;
using PathsOfPower.Cli;

IConsoleWrapper consoleWrapper  = new ConsoleWrapper();
IUserInteraction userInteraction = new UserInteraction(consoleWrapper);
var game = new Game(userInteraction);
game.Run();