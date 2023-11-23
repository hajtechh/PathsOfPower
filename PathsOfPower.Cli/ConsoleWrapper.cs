using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathsOfPower.Cli
{
    internal class ConsoleWrapper : IConsoleWrapper
    {
        public void Clear() => Console.Clear();

        public ConsoleKey ReadKey() => Console.ReadKey(true).Key;

        public string? ReadLine() => Console.ReadLine();

        public void WriteLine(string s) => Console.WriteLine(s);
    }
}
