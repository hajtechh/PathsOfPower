﻿using PathsOfPower.Models;
using PathsOfPower.Interfaces;
using PathsOfPower.Exceptions;
using PathsOfPower.Cli.Interfaces;
using System.Text;
using Microsoft.VisualBasic;
using PathsOfPower.Helpers;

namespace PathsOfPower;

public class Game
{
    private readonly IUserInteraction _userInteraction;
    private readonly IFileHelper _fileHelper;
    private readonly IQuestService _questService;
    private readonly ISavedGameService _savedGameService;
    private readonly Graphics _graphics;

    private const int MaxHealthPoints = 100;
    private const char MinSlotNumber = '1';
    private const char MaxSlotNumber = '3';

    public List<Quest>? Quests { get; set; }
    public Player? Player { get; set; }

    public Game(IUserInteraction userInteraction,
        Graphics graphics,
        IFileHelper fileHelper,
        IQuestService questService,
        ISavedGameService savedGameService)
    {
        _userInteraction = userInteraction;
        _graphics = graphics;
        _fileHelper = fileHelper;
        _questService = questService;
        _savedGameService = savedGameService;
    }
    public void Run()
    {
        _userInteraction.ClearConsole();
        PrintMenu();

        var menuChoice = _userInteraction.GetChar().KeyChar;
        switch (menuChoice)
        {
            case '1':
                Setup();
                StartGame("1");
                break;
            case '2':
                LoadGame();
                break;
            case '3':
                QuitGame();
                break;
            default:
                break;
        }
    }

    private void StartGame(string questIndex)
    {
        var currentChapter = questIndex[..1];
        int chapter = int.Parse(currentChapter);
        Quests = GetQuests(chapter);

        var quest = new Quest();
        if (Quests is not null)
            quest = GetQuestFromIndex(questIndex, Quests);

        // Setup keyActions
        var keyActions = new Dictionary<ConsoleKey, Action>
        {
            { ConsoleKey.M, () => GameMenu(quest.Index) },
        };

        var isRunning = true;
        while (isRunning)
        {
            if (Player is null)
                return;

            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true);

                if (keyActions.TryGetValue(key.Key, out var action))
                {
                    action.Invoke();
                }
            }
            _userInteraction.ClearConsole();

            var menuButton = _graphics.GetGameMenuButton();
            _userInteraction.Print(menuButton);

            var statisticsText = Graphics.GetCharacterStatisticsString(Player);
            _userInteraction.Print(statisticsText);

            var moralityText = Graphics.GetMoralityScaleFromPlayerMoralitySpectrum(Player.MoralitySpectrum);
            _userInteraction.Print(moralityText);

            if (quest is null)
                return;

            PrintQuest(quest);

            var inventory = _graphics.GetPlayerInventoryAsString(Player);
            _userInteraction.Print(inventory);

            if (quest.Options is not null)
            {
                if (quest.Enemy != null)
                {
                    FightEnemy(quest.Enemy, quest.Index);
                }
                if (Player is not null && quest.PowerUpScore != 0)
                {
                    Player.ApplyPowerUpScore(quest.PowerUpScore);
                }

                var choice = _userInteraction.GetChar();
                if (char.IsDigit(choice.KeyChar))
                {
                    var test2 = int.Parse(choice.KeyChar.ToString());
                    var option = quest.Options.FirstOrDefault(x => x.Index == test2);
                    if (Player is not null && option != null && option.MoralityScore != 0)
                    {
                        Player.ApplyMoralityScore(option.MoralityScore);
                    }
                    var index = CreateQuestIndex(quest.Index, choice.KeyChar);

                    if (Quests is not null)
                        quest = GetQuestFromIndex(index, Quests);

                    if (Player is not null && quest is not null && quest.Item is not null)
                    {
                        Player.AddInventoryItem(quest.Item);
                    }
                }
                else
                {
                    if (keyActions.TryGetValue(choice.Key, out var action))
                    {
                        action.Invoke();
                    }
                }
            }
            else if (_fileHelper.IsNextChapterExisting(chapter))
            {
                if (quest.Enemy != null)
                {
                    FightEnemy(quest.Enemy, quest.Index);
                }
                if (Player is not null && quest.PowerUpScore != 0)
                {
                    Player.ApplyPowerUpScore(quest.PowerUpScore);
                }

                chapter++;
                Quests = GetQuests(chapter);
                if (Quests is not null)
                    quest = GetQuestFromIndex(chapter.ToString(), Quests);

                if (Player is not null && quest != null && quest.Item is not null)
                {
                    Player.AddInventoryItem(quest.Item);
                }

                var continueText = _graphics.GetContinueText();
                _userInteraction.Print(continueText);
                var input = _userInteraction.GetChar();

                if (keyActions.TryGetValue(input.Key, out var action))
                {
                    action.Invoke();
                }
                else
                {
                    continue;
                }
            }
            else
            {
                isRunning = false;
                _userInteraction.Print("The end");
            }
        }
    }

    public void ApplyMoralityScore(int? moralityScore)
    {
        Player.MoralitySpectrum += moralityScore ?? 0;
    }

    public void GameMenu(string questIndex)
    {
        _userInteraction.ClearConsole();
        var text = _graphics.GetGameMenuString();
        _userInteraction.Print(text);

        Dictionary<ConsoleKey, Action> keyActions = new Dictionary<ConsoleKey, Action>
        {
          { ConsoleKey.D1, () => StartGame(questIndex) },
          { ConsoleKey.D2, () => SaveGame(questIndex) },
          { ConsoleKey.D3, Run },
          { ConsoleKey.D4, QuitGame },
        };

        var choice = _userInteraction.GetChar();

        if (keyActions.TryGetValue(choice.Key, out var action))
        {
            action.Invoke();
        }
        else
        {
            GameMenu(questIndex);
        }
    }

    public void LoadGame()
    {
        PrintSavedGames();

        var input = _userInteraction.GetChar().KeyChar;
        var slotNumber = (int)char.GetNumericValue(input);
        var text = string.Empty;
        try
        {
            text += _fileHelper.GetSavedGameFromFile(slotNumber);
        }
        catch (FileNotFoundException ex)
        {
            _userInteraction.Print($"File doesn't exist: {ex.Message}");
            return;
        }
        var chosenGame = DeserializeSavedGame(text);
        if (chosenGame is null)
            return;
        Player = chosenGame.Player;
        StartGame(chosenGame.QuestIndex);
    }

    public void FightEnemy(Enemy enemy, string questIndex)
    {
        if (Player is null)
            return;

        var strings = new List<string>
        {
            Graphics.GetEnemyForFightLog(enemy)
        };

        while (Player.HealthPoints > 0 && enemy.HealthPoints > 0)
        {
            Player.PerformAttack(enemy);
            strings.Add(Graphics.GetActionForFightLog(Player, enemy));

            enemy.PerformAttack(Player);
            strings.Add(Graphics.GetActionForFightLog(enemy, Player));
        }

        if (Player.HealthPoints <= 0)
        {
            strings.Add(_graphics.GetSurvivorForFightLog(enemy));
            var fightLog = Graphics.BuildString(strings);
            _userInteraction.Print(fightLog);

            Player.HealthPoints = MaxHealthPoints;
            _userInteraction.GetChar();
            SaveGame(questIndex);
            QuitGame();
        }
        else
        {

            strings.Add(_graphics.GetSurvivorForFightLog(Player));
            var fightLog = Graphics.BuildString(strings);
            _userInteraction.Print(fightLog);
        }
    }

    private void QuitGame()
    {
        Environment.Exit(0);
    }

    public void SaveGame(string questIndex)
    {
        PrintSavedGames();
        var choice = _userInteraction.GetChar().KeyChar;

        var jsonString = SerializeSavedGame(questIndex);

        if (jsonString is not null)
        {
            var isSaved = WriteToFile(choice, jsonString);

            if (isSaved)
            {
                var savedGame = DeserializeSavedGame(jsonString);
                if (savedGame is not null)
                {
                    var text = _graphics.GetConfirmationStringForSavedGame(savedGame);
                    _userInteraction.Print(text);
                }
            }
        }
        _userInteraction.GetChar();
        GameMenu(questIndex);
    }

    public void PrintSavedGames()
    {
        var savedGames = new List<SavedGame>();

        var files = _fileHelper.GetAllSavedGameFilesFromDirectory();
        if (files is null)
            return;
        foreach (var filePath in files)
        {
            var jsonContent = _fileHelper.GetSavedGameFromFile(filePath);
            var savedGame = new SavedGame();
            if (!string.IsNullOrEmpty(jsonContent))
            {
                savedGame = _savedGameService.GetSavedGame(jsonContent);
            }
            savedGames.Add(savedGame ?? new SavedGame());
        }

        var text = _graphics.GetSavedGamesString(savedGames);
        _userInteraction.Print(text);
    }

    public bool WriteToFile(char choice, string jsonString)
    {
        try
        {
            if (choice >= MinSlotNumber && choice <= MaxSlotNumber)
            {
                _fileHelper.WriteAllText(jsonString, choice);
                return true;
            }
            throw new SlotNumberOutOfBoundsException("Slot number was out of bounds");
        }
        catch (SlotNumberOutOfBoundsException ex)
        {
            _userInteraction.Print(ex.Message);
            throw;
        }
    }

    public string? SerializeSavedGame(string questIndex)
    {
        if (Player is null)
            return null;

        var savedGame = new SavedGame(Player, questIndex);
        return _savedGameService.S(savedGame);
        //return _jsonHelper.Serialize(savedGame);
    }

    public SavedGame? DeserializeSavedGame(string jsonString)
    {
        return _jsonHelper.Deserialize<SavedGame>(jsonString);
    }

    private void PrintMenu()
    {
        var menu = _graphics.GetMenu();
        _userInteraction.Print(menu);
    }

    private string CreateQuestIndex(string parentQuestIndex, char choice)
    {
        return $"{parentQuestIndex}.{choice}";
    }

    private void PrintQuest(Quest quest)
    {
        var text = _graphics.GetQuestWithOptions(quest);
        _userInteraction.Print(text);
    }

    private Quest GetQuestFromIndex(string index, List<Quest> quests)
    {
        return quests.First(x => x.Index == index);
    }

    public List<Quest>? GetQuests(int chapterNumber)
    {
        var jsonContent = _fileHelper.GetQuestsFromFile(chapterNumber);
        if (jsonContent is null)
            return null;
        return _questService.GetQuests(jsonContent);
    }

    public void Setup()
    {
        Player = CreatePlayer();
    }

    public Player CreatePlayer()
    {
        _userInteraction.ClearConsole();
        var name = _userInteraction.GetInput("Choose the name of your character.");
        while (string.IsNullOrEmpty(name))
        {
            _userInteraction.ClearConsole();
            name = _userInteraction.GetInput("Your character have to have a name.");
        }

        return new Player(name);
    }
}