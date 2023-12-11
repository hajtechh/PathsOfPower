namespace PathsOfPower.Core.Helpers;

public class StringHelper : IStringHelper
{
    private const int PAD_RIGHT = 110;
    private const int MAX_LENGTH = PAD_RIGHT - 10;
    private const int PAD_LEFT = PAD_RIGHT / 2;
    private const string NEW_LINE = "\r\n";
    private const char BORDER_CHARACTER = '*';
    private const char INVENTORY_BORDER = '+';

    private readonly string _rowDeliminator = "".PadRight(PAD_RIGHT, BORDER_CHARACTER);
    private readonly string _rowDeliminatorInventory = "".PadRight(PAD_RIGHT, INVENTORY_BORDER);

    private readonly string _continueText = $"Press any key to continue{NEW_LINE}";

    private readonly string _emptyinventory = "E M P T Y ";
    private readonly string _inventory = "~ Inventory ~";

    private readonly string _gameMenuButton = $"{NEW_LINE}[M] Game Menu{NEW_LINE}";

    private readonly string _menu = $"{NEW_LINE}" +
        $"[1] Start new game{NEW_LINE}" +
        $"[2] Load game{NEW_LINE}" +
        $"[3] Quit game";

    private readonly string _gameMenu = $"{NEW_LINE}" +
        $"[1] Continue game{NEW_LINE}" +
        $"[2] Save game{NEW_LINE}" +
        $"[3] Exit to main menu{NEW_LINE}" +
        $"[4] Quit game";

    private readonly string _inputNameMessage = "Choose the name of your character.";

    private readonly string _haveToHaveNameMessage = "Your character have to have a name.";

    public string GetMainMenu() => _menu;

    public string GetGameMenu() => _gameMenu;

    public string GetGoToGameMenu() => _gameMenuButton;

    public string GetContinueText() => _continueText;

    public string GetPlayerNameMessage() => _inputNameMessage;

    public string GetNoNameInputMessage() => _haveToHaveNameMessage;

    public string GetQuestWithOptions(Quest quest)
    {
        var strings = new List<string>
        {
            GetCurrentChapter(quest.Index) + NEW_LINE,
            $" {_rowDeliminator}{NEW_LINE}"
        };

        if (quest.Description.Length > MAX_LENGTH)
        {
            var splitPattern = @"(?<=[.,?!])";
            var description = Regex.Split(quest.Description, splitPattern);
            foreach (var line in description)
            {
                strings.Add($"{BORDER_CHARACTER}{line.PadLeft(PAD_LEFT + line.Length / 2).PadRight(PAD_RIGHT)}{BORDER_CHARACTER}{NEW_LINE}");
            }
        }
        else
        {
            strings.Add($"{BORDER_CHARACTER}{quest.Description.PadLeft(PAD_LEFT + quest.Description.Length / 2).PadRight(PAD_RIGHT)}{BORDER_CHARACTER}{NEW_LINE}");
        }
        if (quest.Options is not null)
        {
            foreach (var option in quest.Options)
            {
                var optionDescription = $"[{option.Index}] - {option.Name}";
                strings.Add($"{BORDER_CHARACTER}{optionDescription.PadLeft(PAD_LEFT + optionDescription.Length / 2).PadRight(PAD_RIGHT)}{BORDER_CHARACTER}{NEW_LINE}");
            }
        }
        strings.Add($" {_rowDeliminator}");
        var text = BuildString(strings);
        return text;
    }

    public string GetPlayerInventory(Player player)
    {
        var strings = new List<string>()
        {
            $" {_rowDeliminatorInventory}{NEW_LINE}"
        };
        if (player.InventoryItems != null && player.InventoryItems.Count == 0)
        {
            strings.Add(
                $"{INVENTORY_BORDER}{_inventory.PadLeft(PAD_LEFT + _emptyinventory.Length / 2).PadRight(PAD_RIGHT)}{INVENTORY_BORDER}{NEW_LINE}" +
                $"{INVENTORY_BORDER}{_emptyinventory.PadLeft(PAD_LEFT + _emptyinventory.Length / 2).PadRight(PAD_RIGHT)}{INVENTORY_BORDER}{NEW_LINE}");
        }
        else
        {
            strings.Add($"{INVENTORY_BORDER}" +
                $"{_inventory.PadLeft(PAD_LEFT + _emptyinventory.Length / 2).PadRight(PAD_RIGHT)}{INVENTORY_BORDER}{NEW_LINE}");

            if (player.InventoryItems is not null)
            {
                foreach (var name in player.InventoryItems.Select(x => x.Name))
                {
                    strings.Add($"{INVENTORY_BORDER}{name.PadLeft(PAD_LEFT + name.Length / 2).PadRight(PAD_RIGHT)}{INVENTORY_BORDER}{NEW_LINE}");
                }
            }
        }
        strings.Add($" {_rowDeliminatorInventory}");
        var text = BuildString(strings);
        return text;
    }

    public string GetCharacterStatistics(ICharacter character) =>
        $"Name: {character.Name} | Health: {character.HealthPoints} | Power: {character.Power}{NEW_LINE}";

    public string GetSavedGames(List<SavedGame> savedGames)
    {
        var strings = new List<string>()
        {
            $"Choose slot{NEW_LINE}"
        };
        for (int i = 0; i < savedGames.Count; i++)
        {
            strings.Add($"[{i + 1}] " +
                (savedGames[i].Player != null ? $"{savedGames[i].Player.Name} | {GetCurrentChapter(savedGames[i].QuestIndex)}" : "Empty slot") +
                $"{NEW_LINE}{_rowDeliminator}{NEW_LINE}");
        }
        var text = BuildString(strings);
        return text;
    }

    public string GetConfirmationForSavedGame(SavedGame savedGame)
    {
        return $"Successfully saved game for {savedGame.Player.Name}.{NEW_LINE}{_continueText}";
    }

    public string GetMoralitySpectrum(int moralitySpectrum)
    {
        var text = "You are:";
        return moralitySpectrum switch
        {
            int n when n < -10 => $"{text} Super evil{NEW_LINE}",
            int n when n >= -10 && n < -5 => $"{text} Evil{NEW_LINE}",
            int n when n >= -5 && n <= 0 => $"{text} Neutral{NEW_LINE}",
            int n when n > 0 && n < 5 => $"{text} Decent{NEW_LINE}",
            int n when n >= 5 && n < 10 => $"{text} Good{NEW_LINE}",
            int n when n >= 10 && n < 15 => $"{text} Saint{NEW_LINE}",
            _ => $"No data available{NEW_LINE}",
        };
    }

    public string GetEnemyForFightLog(Enemy enemy)
    {
        return $"Fight against {enemy.Name}!{NEW_LINE}";
    }

    public string GetActionForFightLog(ICharacter attacker, ICharacter attacked)
    {
        return $"{attacker.Name} attacks for {attacker.Power} damage. {attacked.Name} now has {attacked.HealthPoints} healthpoints left.{NEW_LINE}";
    }

    public string GetSurvivorForFightLog(ICharacter character)
    {
        return $"{character.Name} wins, with {character.HealthPoints} healthpoints remaining!{NEW_LINE}{_continueText}";
    }

    public string GetCurrentChapter(string index)
    {
        var questIndex = index.Split('.');
        var currentChapter = questIndex.FirstOrDefault();
        var text = $"Chapter {currentChapter}";
        return text;
    }

    public string BuildString(List<string> strings)
    {
        var stringBuilder = new StringBuilder();
        for (int i = 0; i < strings.Count; ++i)
        {
            stringBuilder.Append(strings[i]);
        }
        return stringBuilder.ToString();
    }

    public string GetQuestIndex(string parentQuestIndex, char choice)
    {
        return $"{parentQuestIndex}.{choice}";
    }

    public string GetTheEndText() =>
        "The end";

    public string GetExitGame() =>
        "Game is shutting down";

    public string TrimInput(string message)
    {
        char[] charsToTrim = { '_', '.', '\0', '!', '*' };
        message = message.Trim(charsToTrim);
        return message;
    }

    public string GetOptionDoesNotExist() => $"Option doesn't exist.{NEW_LINE}Please try again.";
}

