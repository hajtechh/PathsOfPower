using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathsOfPower.Cli.Interfaces
{
    public interface IConsoleWrapper
    {
        void WriteLine(string s);
        string? ReadLine();
        ConsoleKeyInfo ReadChar();
        void Clear();
    }
}
