using FunGame.Common.Constants;
using FunGame.Common.Helpers;
using FunGame.Common.Requests;
using FunGame.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using NLog;


namespace GameService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [HttpPost("compute")]
        public IActionResult Compute([FromBody] ComputeRequest request)
        {
            try
            {
                if (request.PlayerChoice == GameChoice.None || request.ComputerChoice == GameChoice.None)
                {
                    Logger.Warn($"Invalid game request: {request}");
                    return BadRequest(new ErrorResponse { Error = "Invalid game choices." });
                }

                if (!GameChoiceExtensions.GetValidChoices().Contains(request.PlayerChoice) ||
                    !GameChoiceExtensions.GetValidChoices().Contains(request.ComputerChoice))
                {
                    Logger.Warn($"Invalid choices: PlayerChoice={request.PlayerChoice}, ComputerChoice={request.ComputerChoice}");
                    return BadRequest(new ErrorResponse { Error = $"Invalid choice. Choose {string.Join(", ", GameChoiceExtensions.GetValidChoiceNames())}." });
                }

                if (request.PlayerChoice == request.ComputerChoice)
                {
                    Logger.Info($"Game tied: {request.PlayerChoice} vs {request.ComputerChoice}");
                    return Ok(new GameResponse { Result = "Tie" });
                }

                if (GameConstants.WinsAgainst[request.PlayerChoice].Contains(request.ComputerChoice))
                {
                    Logger.Info($"Player wins: {request.PlayerChoice} beats {request.ComputerChoice}");
                    return Ok(new GameResponse { Result = "Player wins!" });
                }

                Logger.Info($"Computer wins: {request.PlayerChoice} loses to { request.ComputerChoice}");
                return Ok(new GameResponse { Result = "Computer wins!" });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error computing game result for PlayerChoice={request.PlayerChoice}, ComputerChoice={request.ComputerChoice}");
                return StatusCode(500, new ErrorResponse { Error = "Internal server error." });
            }
        }
    }
}
