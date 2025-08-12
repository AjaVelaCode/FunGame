using GameService.Model;
using Microsoft.AspNetCore.Mvc;
using NLog;


namespace GameService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly string[] Choices = ["rock", "paper", "scissors", "lizard", "spock"];
        private static readonly Dictionary<string, string[]> WinsAgainst = new()
        {
            { "rock", new[] { "scissors", "lizard" } },
            { "paper", new[] { "rock", "spock" } },
            { "scissors", new[] { "paper", "lizard" } },
            { "lizard", new[] { "paper", "spock" } },
            { "spock", new[] { "rock", "scissors" } }
        };

        [HttpPost("compute")]
        public IActionResult Compute([FromBody] ComputeRequest request)
        {
            if (!Choices.Any(choice =>
                    choice.Equals(request.PlayerChoice, StringComparison.InvariantCultureIgnoreCase))
                || !Choices.Any(choice =>
                    choice.Equals(request.ComputerChoice, StringComparison.InvariantCultureIgnoreCase)))
                return BadRequest(new { Error = "Invalid choices." });
                
            try
            {
                string result;
                if (request.PlayerChoice.Equals(request.ComputerChoice, StringComparison.InvariantCultureIgnoreCase))
                    result = "It's a tie!";
                else if (WinsAgainst[request.PlayerChoice.ToLowerInvariant()].Contains(request.ComputerChoice.ToLowerInvariant()))
                    result = "You win!";
                else
                    result = "You lose!";
                    
                Logger.Info($"Computed result: {request.PlayerChoice} vs {request.ComputerChoice} -> {result}");
                return Ok(new { Result = result });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unexpected error in Compute endpoint.");
                throw; // Middleware handles
            }
        }
    }
}
