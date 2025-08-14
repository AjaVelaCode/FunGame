using FunGame.Common.Constants;

namespace ScoreService.Model;

public class GameResult
{
    public string UserId { get; set; } = "Anonymous";
    public GameChoice PlayerChoice { get; set; }
    public GameChoice ComputerChoice { get; set; }
    public string Result { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}