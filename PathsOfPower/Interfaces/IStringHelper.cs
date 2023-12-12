namespace PathsOfPower.Core.Interfaces;

public interface IStringHelper
{
    string BuildString(List<string> strings);
    string TrimInput(string name);
    string GetMainMenu();
    string GetGameMenu();
    string GetExitGame();
    string GetTheEndText();
    string GetContinueText();
    string GetGoToGameMenu();
    string GetPlayerNameMessage();
    string GetNoNameInputMessage();
    string GetOptionDoesNotExist();
    string GetCurrentChapter(string index);
    string GetEnemyForFightLog(Enemy enemy);
    string GetQuestWithOptions(Quest quest);
    string GetPlayerInventory(Player player);
    string GetMoralitySpectrum(int moralitySpectrum);
    string GetSavedGames(List<SavedGame> savedGames);
    string GetCharacterStatistics(ICharacter character);
    string GetSurvivorForFightLog(ICharacter character);
    string GetConfirmationForSavedGame(SavedGame savedGame);
    string GetQuestIndex(string parentQuestIndex, char choice);
    string GetActionForFightLog(ICharacter attacker, ICharacter attacked);
}
