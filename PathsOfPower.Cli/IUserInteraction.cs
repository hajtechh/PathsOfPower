using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathsOfPower.Cli
{
    internal interface IUserInteraction
    {
        string GetInput(string message);
        int ParseInputToInt();
        void ClearConsole();
        ConsoleKey GetKey();
    }
}
