
using System.ComponentModel.DataAnnotations;

namespace FunGame.Common
{
    public class ServiceUrls
    {
        [Required(ErrorMessage = "GameService URL is required.")]
        public string GameService { get; set; } = string.Empty;

        [Required(ErrorMessage = "ScoreService URL is required.")]
        public string ScoreService { get; set; } = string.Empty;

        [Required(ErrorMessage = "RandomNumberService URL is required.")]
        public string RandomNumberService { get; set; } = string.Empty;
    }
}
