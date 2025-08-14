using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PlayerService.Models.Requests;

public class PlayRequest
{
    [Required(ErrorMessage = "PlayerChoiceId is required.")]
    //public GameChoice PlayerChoiceId { get; set; } = GameChoice.None;
    [JsonPropertyName("player")]
    public int PlayerChoiceId { get; set; } = -1;

    public string UserId { get; set; } = string.Empty;
}