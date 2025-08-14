using System.Text.Json.Serialization;

namespace FunGame.Common.Responses;

public class RandomNumberResponse
{
    [JsonPropertyName("random_number")] public int RandomNumber { get; set; }
}