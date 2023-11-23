using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathsOfPower.Cli
{
    internal interface IConsoleWrapper
    {
        void WriteLine(string s);
        string? ReadLine();
        ConsoleKey ReadKey();
        void Clear();
    }
}
