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

    public string GetContinueText() => _continueText;

    public string GetQuestWithOptions(Quest quest)
    {
        var strings = new List<string>
        {
            GetCurrentChapterAsString(quest.Index) + NewLine,
            $"{_rowDeliminator}{NewLine}"
        };

        if (quest.Description.Length > MaxLength)
        {
            var splitPattern = @"(?<=[.,?!])";
            var description = Regex.Split(quest.Description, splitPattern);
            foreach (var line in description)
            {
                strings.Add($"{BorderCharacter}{line.PadLeft(PadLeft + line.Length / 2).PadRight(PadRight)}{BorderCharacter}{NewLine}");
            }
        }
        else
        {
            strings.Add($"{BorderCharacter}{quest.Description.PadLeft(PadLeft + quest.Description.Length / 2).PadRight(PadRight)}{BorderCharacter}{NewLine}");
        }
        if (quest.Options is not null)
        {
            foreach (var option in quest.Options)
            {
                var optionDescription = $"[{option.Index}] - {option.Name}";
                strings.Add($"{BorderCharacter}{optionDescription.PadLeft(PadLeft + optionDescription.Length / 2).PadRight(PadRight)}{BorderCharacter}{NewLine}");
            }
        }
        strings.Add($"{_rowDeliminator}");
        var text = BuildString(strings);
        return text;
    }

    public string GetPlayerInventoryAsString(Player player)
    {
        var strings = new List<string>()
        {
            $"{_rowDeliminatorInventory}{NewLine}"
        };
        if (player.InventoryItems != null && player.InventoryItems.Count == 0)
        {
            strings.Add($"{InventoryBorder}" +
                $"{_inventory.PadLeft(PadLeft + _emptyinventory.Length / 2).PadRight(PadRight)}{InventoryBorder}{NewLine}" +
                $"{_emptyinventory.PadLeft(PadLeft + _emptyinventory.Length / 2).PadRight(PadRight)}{InventoryBorder}{NewLine}");
        }
        else
        {
            strings.Add($"{InventoryBorder}" +
                $"{_inventory.PadLeft(PadLeft + _emptyinventory.Length / 2).PadRight(PadRight)}{InventoryBorder}{NewLine}");

            if (player.InventoryItems is not null)
            {
                foreach (var name in player.InventoryItems.Select(x => x.Name))
                {
                    strings.Add($"{InventoryBorder}{name.PadLeft(PadLeft + name.Length / 2).PadRight(PadRight)}{InventoryBorder}{NewLine}");
                }
            }
        }
        strings.Add($"{_rowDeliminatorInventory}");
        var text = BuildString(strings);
        return text;
    }

    public static string GetCharacterStatisticsString(ICharacter character) =>
        $"Current health: {character.HealthPoints} | Power: {character.Power}{NewLine}";

    public string GetSavedGamesString(List<SavedGame> savedGames)
    {
        var strings = new List<String>()
        {
            $"Choose slot{NewLine}"
        };
        for (int i = 0; i < savedGames.Count; i++)
        {
            strings.Add($"[{i + 1}] " +
                (savedGames[i].Player != null ? $"{savedGames[i].Player.Name} | {GetCurrentChapterAsString(savedGames[i].QuestIndex)}" : "Empty slot") +
                $"{NewLine}{_rowDeliminator}{NewLine}");
        }
        var text = BuildString(strings);
        return text;
    }

    public string GetConfirmationStringForSavedGame(SavedGame savedGame)
    {
        return $"Successfully saved game for {savedGame.Player.Name}.{NewLine}{_continueText}";
    }

    public static string GetMoralityScaleFromPlayerMoralitySpectrum(int moralitySpectrum)
    {
        var text = "You are:";
        return moralitySpectrum switch
        {
            int n when n < -10 => $"{text} Super evil{NewLine}",
            int n when n >= -10 && n < -5 => $"{text} Evil{NewLine}",
            int n when n >= -5 && n <= 0 => $"{text} Neutral{NewLine}",
            int n when n > 0 && n < 5 => $"{text} Super evil{NewLine}",
            int n when n >= 5 && n < 10 => $"{text} Good{NewLine}",
            int n when n >= 10 && n < 15 => $"{text} Saint{NewLine}",
            _ => $"No data available{NewLine}",
        };
    }

    public static string GetEnemyForFightLog(Enemy enemy)
    {
        return $"Fight against {enemy.Name}!{NewLine}";
    }

    public static string GetActionForFightLog(ICharacter attacker, ICharacter attacked)
    {
        return $"{attacker.Name} attacks for {attacker.Power} damage. {attacked.Name} now has {attacked.HealthPoints} healthpoints left.{NewLine}";
    }

    public string GetSurvivorForFightLog(ICharacter character)
    {
        return $"{character.Name} wins, with {character.HealthPoints} healthpoints remaining!{NewLine}{_continueText}";
    }

    public static string GetCurrentChapterAsString(string index)
    {
        var questIndex = index.Split('.');
        var currentChapter = questIndex.FirstOrDefault();
        var text = $"Chapter {currentChapter}";
        return text;
    }

    public static string BuildString(List<string> strings)
    {
        var stringBuilder = new StringBuilder();
        for (int i = 0; i < strings.Count; ++i)
        {
            stringBuilder.Append(strings[i]);
        }
        return stringBuilder.ToString();
    }
}

