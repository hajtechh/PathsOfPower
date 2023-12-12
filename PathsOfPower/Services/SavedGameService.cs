namespace PathsOfPower.Core.Services;

public class SavedGameService : ISavedGameService
{
    private const char MIN_SLOT_NUMBER = '1';
    private const char MAX_SLOT_NUMBER = '3';

    private readonly IJsonHelper _jsonHelper;
    private readonly IFileHelper _fileHelper;
    private readonly IFactory _factory;

    public SavedGameService(IJsonHelper jsonHelper, IFileHelper fileHelper, IFactory factory)
    {
        _jsonHelper = jsonHelper;
        _fileHelper = fileHelper;
        _factory = factory;
    }

    public List<SavedGame> GetSavedGames()
    {
        var savedGames = new List<SavedGame>();

        var files = _fileHelper.GetAllSavedGameFilesFromDirectory();
        if (files is null)
            // THROW EXCEPTION there are no files
            return savedGames;

        foreach (var filePath in files)
        {
            var jsonContent = _fileHelper.GetSavedGameFromFile(filePath);
            var savedGame = _factory.CreateSavedGame();

            if (!string.IsNullOrEmpty(jsonContent))
                savedGame = GetSavedGame(jsonContent);

            savedGames.Add(savedGame ?? _factory.CreateSavedGame());
        }
        return savedGames;
    }

    public (SavedGame? savedGame, string message) LoadGame(char input)
    {
        var savedGame = _factory.CreateSavedGame();
        try
        {
            if (CheckForValidSlotNumber(input) is false)
                throw new SlotNumberOutOfBoundsException($"Slot number must be equal to or greater than {MIN_SLOT_NUMBER} and equal to or less than {MAX_SLOT_NUMBER}.");

            var slotNumber = (int)char.GetNumericValue(input);
            var jsonContent = _fileHelper.GetSavedGameFromFile(slotNumber);

            if (jsonContent is null)
                throw new ArgumentNullException(nameof(jsonContent));

            savedGame = _jsonHelper.Deserialize<SavedGame>(jsonContent);

            if (savedGame is null)
                throw new FileHelperUnableToDeserialize("Error.. FileHelper can't deserialize SavedGame object.");
        }
        catch (SlotNumberOutOfBoundsException slotNumberOutOfBoundException)
        {
            return (null, slotNumberOutOfBoundException.Message);
        }
        catch (ArgumentNullException argumentNullException)
        {
            return (null, argumentNullException.Message);
        }
        catch (FileNotFoundException ex)
        {
            return (null, $"File does not exists. {nameof(FileNotFoundException)} message: {ex.Message}");
        }
        catch (FileHelperUnableToDeserialize fileHelperUnableToDeserialize)
        {
            return (null, fileHelperUnableToDeserialize.Message);
        }
        return (savedGame, "Successfully got the saved game");
    }

    public (bool isSaved, string message) SaveGame(Player player, char slotNumber, string questIndex)
    {
        try
        {
            var savedGame = new SavedGame(player, questIndex);
            var jsonContent = _jsonHelper.Serialize(savedGame);

            if (jsonContent is null)
                throw new ArgumentNullException(nameof(jsonContent));

            var choice = char.GetNumericValue(slotNumber);

            if (CheckForValidSlotNumber(slotNumber) is false)
                throw new SlotNumberOutOfBoundsException($"Slot number must be equal to or greater than {MIN_SLOT_NUMBER} and equal to or less than {MAX_SLOT_NUMBER}.");

            _fileHelper.WriteAllText(jsonContent, slotNumber);
            return (true, $"Successfully saved game for {savedGame.Player.Name}.");
        }
        catch (SlotNumberOutOfBoundsException slotNumberOutOfBoundException)
        {
            return (false, slotNumberOutOfBoundException.Message);
        }
        catch (ArgumentNullException argumentNullException)
        {
            return (false, argumentNullException.Message);
        }
        catch (OutOfMemoryException outOfMemoryException)
        {
            return (false, outOfMemoryException.Message);
        }
        catch (PathTooLongException pathTooLongException)
        {
            return (false, pathTooLongException.Message);
        }
        catch (DirectoryNotFoundException directoryNotFoundException)
        {
            return (false, directoryNotFoundException.Message);
        }
        catch (NotSupportedException notSupportedException)
        {
            return (false, notSupportedException.Message);
        }
    }

    public string? CreateSavedGame(SavedGame savedGame) =>
        _jsonHelper.Serialize(savedGame);

    public SavedGame? GetSavedGame(string jsonContent) =>
        _jsonHelper.Deserialize<SavedGame>(jsonContent);
    public bool CheckForValidSlotNumber(char input) => 
        (input >= MIN_SLOT_NUMBER && input <= MAX_SLOT_NUMBER);
    
}
