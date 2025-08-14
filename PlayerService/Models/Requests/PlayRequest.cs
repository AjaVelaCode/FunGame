using FunGame.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace PlayerService.Models.Requests
{
    public class PlayRequest
    {
        [Required(ErrorMessage = "PlayerChoice is required.")]
        public GameChoice PlayerChoice { get; set; } = GameChoice.None;
        public string UserId { get; set; } = string.Empty;
    }
}
