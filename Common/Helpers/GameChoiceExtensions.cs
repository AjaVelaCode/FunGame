using FunGame.Common.Constants;

namespace FunGame.Common.Helpers
{
    public class GameChoiceExtensions
    {
        public static GameChoice[] GetValidChoices()
        {
            return Enum.GetValues<GameChoice>()
                .Where(c => c != GameChoice.None)
                .ToArray();
        }

        public static string[] GetValidChoiceNames()
        {
            return GetValidChoices()
                .Select(c => c.ToString().ToLowerInvariant())
                .ToArray();
        }
    }
}
