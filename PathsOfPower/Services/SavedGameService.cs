namespace PathsOfPower.Core.Services;

public class SavedGameService : ISavedGameService
{
    private const char MIN_SLOT_NUMBER = '1';
    private const char MAX_SLOT_NUMBER = '3';

    private readonly IJsonHelper _jsonHelper;
    private readonly IFileHelper _fileHelper;

    public SavedGameService(IJsonHelper jsonHelper, IFileHelper fileHelper)
    {
        _jsonHelper = jsonHelper;
        _fileHelper = fileHelper;
    }

    public List<SavedGame>? GetSavedGames(string jsonContent) =>
        _jsonHelper.Deserialize<List<SavedGame>>(jsonContent);

    public SavedGame? GetSavedGame(string jsonContent) =>
        _jsonHelper.Deserialize<SavedGame>(jsonContent);

    public (bool isSaved, string message) SaveGame(Player player, char slotNumber, string questIndex)
    {
        var savedGame = new SavedGame(player, questIndex);
        var jsonContent = _jsonHelper.Serialize(savedGame);

        try
        {
            if (jsonContent is null)
                throw new ArgumentNullException(nameof(jsonContent));

            if (slotNumber >= MIN_SLOT_NUMBER && slotNumber <= MAX_SLOT_NUMBER)
                throw new SlotNumberOutOfBoundsException("Slot number was out of bounds");

            _fileHelper.WriteAllText(jsonContent, slotNumber);
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

        return (true, $"Successfully saved game for {savedGame.Player.Name}.");
    }
}
