using FunGame.Common.Constants;

namespace PlayerService.Models.Responses
{
    public class GamePlayResponse
    {
        public GameChoice PlayerChoice { get; set; }
        public GameChoice ComputerChoice { get; set; }
        public string Result { get; set; } = string.Empty;
        public string FunFact { get; set; } = string.Empty;

    }
}
