using PathsOfPower.Interfaces;
using PathsOfPower.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace PathsOfPower;

public class Graphics
{
    private const int PadRight = 80;
    private const int MaxLength = PadRight - 10;
    private const int PadLeft = PadRight / 2;
    private const string NewLine = "\r\n";
    private const char BorderCharacter = '*';
    private const char InventoryBorder = '+';

    private readonly string _rowDeliminator = "".PadRight(PadRight, BorderCharacter);
    private readonly string _rowDeliminatorInventory = "".PadRight(PadRight, InventoryBorder);

    private readonly string _continueText = $"Press any key to continue{NewLine}";
    private readonly string _emptyinventory = " E M P T Y ";
    private readonly string _inventory = "~ Inventory ~";

    private readonly string _menu = $"[1] Start new game{NewLine}" +
        $"[2] Load game{NewLine}" +
        $"[3] Quit game";

    private readonly string _gameMenu = $"[1] Continue game{NewLine}" +
        $"[2] Save game{NewLine}" +
        $"[3] Main Menu{NewLine}" +
        $"[4] Quit game";

    private readonly string _gameMenuButton = $"{NewLine}[M] Game Menu{NewLine}";

    public string GetMenu() => _menu;

    public string GetGameMenuString() => _gameMenu;

    public string GetGameMenuButton() => _gameMenuButton;

    public string GetQuestWithOptions(Quest quest)
    {
        var text = GetCurrentChapterAsString(quest.Index);
        text += NewLine;
        text += $"{_rowDeliminator}{NewLine}";
        if (quest.Description.Length > MaxLength)
        {
            var splitPattern = @"(?<=[.,?!])";
            var description = Regex.Split(quest.Description, splitPattern);
            foreach (var line in description)
            {
                text += $"{BorderCharacter}{line.PadLeft(PadLeft + line.Length / 2).PadRight(PadRight)}{BorderCharacter}{NewLine}";
            }
        }
        else
        {
            text += $"{BorderCharacter}{quest.Description.PadLeft(PadLeft + quest.Description.Length / 2).PadRight(PadRight)}{BorderCharacter}{NewLine}";
        }
        if (quest.Options is not null)
        {
            foreach (var option in quest.Options)
            {
                var optionDescription = $"[{option.Index}] - {option.Name}";
                text += $"{BorderCharacter}{optionDescription.PadLeft(PadLeft + optionDescription.Length / 2).PadRight(PadRight)}{BorderCharacter}{NewLine}";
            }
        }
        text += $"{_rowDeliminator}";
        return text;
    }

    public string GetPlayerInventoryAsString(Player player)
    {
        var text = $"{_rowDeliminatorInventory}{NewLine}";
        if (player.InventoryItems != null && player.InventoryItems.Count == 0)
        {
            text += $"{InventoryBorder}";
            text += $"{_inventory.PadLeft(PadLeft + _emptyinventory.Length / 2).PadRight(PadRight)}{InventoryBorder}{NewLine}";
            text += $"{_emptyinventory.PadLeft(PadLeft + _emptyinventory.Length / 2).PadRight(PadRight)}{InventoryBorder}{NewLine}";
        }
        else
        {

            text += $"{InventoryBorder}";
            text += $"{_inventory.PadLeft(PadLeft + _emptyinventory.Length / 2).PadRight(PadRight)}{InventoryBorder}{NewLine}";
            foreach (var item in player.InventoryItems)
            {
                text += $"{InventoryBorder}{item.Name.PadLeft(PadLeft + item.Name.Length / 2).PadRight(PadRight)}{InventoryBorder}{NewLine}";
            }
        }
        text += $"{_rowDeliminatorInventory}";
        return text;
    }

    public string GetCharacterStatisticsString(ICharacter character)
    {
        return $"Current health: {character.HealthPoints} | Power: {character.Power}{NewLine}";
    }

    public string GetSavedGamesString(List<SavedGame> savedGames)
    {
        var text = ($"Choose slot{NewLine}");
        for (int i = 0; i < savedGames.Count; i++)
        {
            text += $"[{i + 1}] ";
            text += savedGames[i].Player != null ?
                $"{savedGames[i].Player.Name} | {GetCurrentChapterAsString(savedGames[i].QuestIndex)}":
                "Empty slot";
            text += NewLine;
            text += "---------";
            text += NewLine;
        }
        return text;
    }

    public string GetConfirmationStringForSavedGame(SavedGame savedGame)
    {
        return $"Successfully saved game for {savedGame.Player.Name}.{NewLine}{_continueText}";
    }

    public string GetMoralityScaleFromPlayerMoralitySpectrum(int moralitySpectrum)
    {
        var text = "You are: ";
        switch (moralitySpectrum)
        {
            case int n when n < -10:
                return text += "Super evil" + NewLine;
            case int n when n < -5:
                return text += "Evil" + NewLine;
            case int n when n <= 0:
                return text += "Neutral" + NewLine;
            case int n when n < 5:
                return text += "Decent" + NewLine;
            case int n when n < 10:
                return text += "Good" + NewLine;
            case int n when n < 15:
                return text += "Saint" + NewLine;
            default:
                return "No data available" + NewLine;
        }
    }

    public string GetEnemyForFightLog(Enemy enemy)
    {
        return $"Fight against {enemy.Name}!{NewLine}";
    }
    public string GetActionForFightLog(ICharacter attacker, ICharacter attacked)
    {
        return $"{attacker.Name} attacks for {attacker.Power} damage. {attacked.Name} now has {attacked.HealthPoints} healthpoints left.{NewLine}";
    }

    public string GetSurvivorForFightLog(ICharacter character)
    {
        return $"{character.Name} wins, with {character.HealthPoints} healthpoints remaining!{NewLine}{_continueText}";
    }

    public string GetCurrentChapterAsString(string index)
    {
        var questIndex = index.Split('.');
        var currentChapter = questIndex.FirstOrDefault();
        var text = $"Chapter {currentChapter}";
        return text;
    }
}

