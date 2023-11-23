﻿using System;
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

        public char GetKeyChar()
        {
            return _consoleWrapper.ReadChar();
        }

        public ConsoleKey GetKey()
       {
            return _consoleWrapper.ReadKey();
        }

        public int ParseInputToInt()
        {
            int number;
            while (int.TryParse(_consoleWrapper.ReadLine(), out number) is false)
            {
                _consoleWrapper.WriteLine("You must enter digits, please try again!");
            }
            return number;
        }
    }
}
