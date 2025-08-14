using FunGame.Common;
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
            if (!ModelState.IsValid)
            {
                Logger.Warn($"Invalid compute request: {ModelState}");
                return BadRequest(ModelState);
            }

            try
            {
                if (!Enum.IsDefined(typeof(GameChoice), request.PlayerChoice) ||
                    !Enum.IsDefined(typeof(GameChoice), request.ComputerChoice))
                {
                    Logger.Warn($"Invalid choices: PlayerChoice={request.PlayerChoice}, ComputerChoice={request.ComputerChoice}");
                    return BadRequest(new { Error = "Invalid player or computer choice." });
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
                return StatusCode(500, new { Error = "Internal server error." });
            }
        }
    }
}
