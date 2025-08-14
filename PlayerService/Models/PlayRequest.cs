using FunGame.Common;
using System.ComponentModel.DataAnnotations;

namespace PlayerService.Models
{
    public class PlayRequest
    {
        [Required(ErrorMessage = "PlayerChoice is required.")]
        public GameChoice PlayerChoice { get; set; } = GameChoice.None;
        public string UserId { get; set; } = string.Empty;
    }
}
