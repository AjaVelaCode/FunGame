using FunGame.Common;

namespace ScoreService.Model
{
    public class GameResult
    {
        public string UserId { get; set; } = "Anonymous";
        public GameChoice PlayerChoice { get; set; }
        public GameChoice ComputerChoice { get; set; }
        public string Result { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
