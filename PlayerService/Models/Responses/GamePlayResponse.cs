using System.Text.Json.Serialization;

namespace PlayerService.Models.Responses;

public class GamePlayResponse
{
    [JsonPropertyName("player")]
    public int PlayerChoice { get; set; }
    
    [JsonPropertyName("computer")]
    public int ComputerChoice { get; set; }

    [JsonPropertyName("results")]
    public string Results { get; set; } = string.Empty;

    [JsonPropertyName("funfact")]
    public string FunFact { get; set; } = string.Empty;
}