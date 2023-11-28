using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PathsOfPower.Cli
{
    public class ConsoleWrapper : IConsoleWrapper
    {
        public void Clear() => Console.Clear();

        public ConsoleKeyInfo ReadChar()
        {
            return  Console.ReadKey(true);
        }

        public string? ReadLine() => Console.ReadLine();

        public void WriteLine(string s) => Console.WriteLine(s);
    }
}
