﻿namespace PathsOfPower.Core.Interfaces;

public interface IUserInteraction
{
    string GetInput(string message);
    void ClearConsole();
    void Print(string message);
    ConsoleKeyInfo GetChar();
}
