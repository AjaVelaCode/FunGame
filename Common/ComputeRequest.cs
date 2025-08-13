using System.ComponentModel.DataAnnotations;

namespace FunGame.Common
{
    public class ComputeRequest
    {
        [Required(ErrorMessage = "PlayerChoice is required.")]
        public GameChoice PlayerChoice { get; set; }

        [Required(ErrorMessage = "ComputerChoice is required.")]
        public GameChoice ComputerChoice { get; set; }
    }
}
