using FunGame.Common;

namespace PlayerService.Models
{
    public class PlayRequest
    {
        public GameChoice PlayerChoice { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
