using System.Text.Json.Serialization;

namespace FunGame.Common
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GameChoice
    {
        Rock,
        Paper,
        Scissors,
        Lizard,
        Spock
    }

    public static class GameConstants
    {
        public static readonly Dictionary<GameChoice, GameChoice[]> WinsAgainst = new()
        {
            { GameChoice.Rock, new[] { GameChoice.Scissors, GameChoice.Lizard } },
            { GameChoice.Paper, new[] { GameChoice.Rock, GameChoice.Spock } },
            { GameChoice.Scissors, new[] { GameChoice.Paper, GameChoice.Lizard } },
            { GameChoice.Lizard, new[] { GameChoice.Paper, GameChoice.Spock } },
            { GameChoice.Spock, new[] { GameChoice.Rock, GameChoice.Scissors } }
        };

        public static readonly Dictionary<(GameChoice, GameChoice), string> FunFacts = new()
        {
            { (GameChoice.Rock, GameChoice.Scissors), "Rock crushes scissors... sharp edges beware!" },
            { (GameChoice.Rock, GameChoice.Lizard), "Rock crushes lizard... ouch, flat as a pancake!" },
            { (GameChoice.Paper, GameChoice.Rock), "Paper covers rock... simple yet effective!" },
            { (GameChoice.Paper, GameChoice.Spock), "Paper disproves Spock... logic can't beat bureaucracy!" },
            { (GameChoice.Scissors, GameChoice.Paper), "Scissors cuts paper... snip snip!" },
            { (GameChoice.Scissors, GameChoice.Lizard), "Scissors decapitates lizard... ouch!" },
            { (GameChoice.Lizard, GameChoice.Paper), "Lizard eats paper... nom nom nom!" },
            { (GameChoice.Lizard, GameChoice.Spock), "Lizard poisons Spock... live long and prosper? Not today!" },
            { (GameChoice.Spock, GameChoice.Rock), "Spock vaporizes rock... poof, it's gone!" },
            { (GameChoice.Spock, GameChoice.Scissors), "Spock smashes scissors... Vulcan strength wins!" }
        };

        private static readonly string[] TieFunFacts =
        [
            "It's a tie! Even the universe can't decide!",
            "A draw! The cosmos is in balance!"
        ];

        private static readonly string[] GeneralFunFacts =
        [
            "Rock, Paper, Scissors, Lizard, Spock: The ultimate test of strategy!",
            "Did you know? This game was popularized by The Big Bang Theory!"
        ];

        public static string GetFunFact(GameChoice playerChoice, GameChoice computerChoice, string gameResult)
        {
            if (gameResult == "Tie!")
                return TieFunFacts[Random.Shared.Next(TieFunFacts.Length)];
            return FunFacts.TryGetValue((playerChoice, computerChoice), out var fact) ? fact : GeneralFunFacts[Random.Shared.Next(GeneralFunFacts.Length)];
        }
    }


}
