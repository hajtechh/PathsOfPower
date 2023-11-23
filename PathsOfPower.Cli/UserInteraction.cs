using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace PathsOfPower.Cli
{
    public class UserInteraction : IUserInteraction
    {
        private readonly IConsoleWrapper _consoleWrapper;

        public UserInteraction(IConsoleWrapper consoleWrapper)
        {
            _consoleWrapper = consoleWrapper;
        }

        public void Print(string message)
        {
            _consoleWrapper.WriteLine(message);
        }
        public void ClearConsole()
        {
            _consoleWrapper.Clear();
        }

        public string GetInput(string message)
        {
            _consoleWrapper.WriteLine(message);
            return _consoleWrapper.ReadLine() ?? string.Empty;
        }

        public char GetChar()
        {
            return _consoleWrapper.ReadChar();
        }
    }
}
