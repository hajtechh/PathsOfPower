using PathsOfPower.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathsOfPower.Interfaces
{
    public interface IStringHelper
    {
        string GetMenu();
        string GetGameMenuString();
        string GetGameMenuButton();
        string GetContinueText();
        string GetPlayerNameMessage();
        string GetNoNameInputMessage();
        string GetQuestWithOptions(Quest quest);
        string GetPlayerInventoryAsString(Player player);
        //string GetCharacterStatisticsString(ICharacter character);
        string GetSavedGamesString(List<SavedGame> savedGames);
        string GetConfirmationStringForSavedGame(SavedGame savedGame);
        //string GetMoralityScaleFromPlayerMoralitySpectrum(int moralitySpectrum);
        //string GetEnemyForFightLog(Enemy enemy);
        //string GetActionForFightLog(ICharacter attacker, ICharacter attacked);
        string GetSurvivorForFightLog(ICharacter character);
        //string GetCurrentChapterAsString(string index);
        //string BuildString(List<string> strings);
        string GetQuestIndexString(string parentQuestIndex, char choice);
    }
}
