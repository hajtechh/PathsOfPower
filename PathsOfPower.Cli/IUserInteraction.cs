using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathsOfPower.Cli
{
    public interface IUserInteraction
    {
        string GetInput(string message);
        void ClearConsole();
        void Print(string message);
        ConsoleKeyInfo GetChar();
    }
}
