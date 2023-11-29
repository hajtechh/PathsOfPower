using PathsOfPower;
using PathsOfPower.Cli;

IConsoleWrapper consoleWrapper  = new ConsoleWrapper();
IUserInteraction userInteraction = new UserInteraction(consoleWrapper);
var graphics = new Graphics();
var game = new Game(userInteraction, graphics);
game.Run();