using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using FunGame.Common;
using NLog;
using ScoreService.Model;

namespace ScoreService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScoreController : ControllerBase
    {
        private static readonly ConcurrentQueue<GameResult> RecentResults = new();
        private const int MaxResults = 10;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [HttpPost("add")]
        public IActionResult Add([FromBody] GameResult result)
        {
            if (!Enum.IsDefined(typeof(GameChoice), result.PlayerChoice) || !Enum.IsDefined(typeof(GameChoice), result.ComputerChoice) || string.IsNullOrEmpty(result.Result))
            {
                Logger.Warn($"Invalid game result: PlayerChoice={result.PlayerChoice}, ComputerChoice={result.ComputerChoice}, Result={result.Result}");
                return BadRequest(new { Error = "Invalid game result data." });
            }

            try
            {
                RecentResults.Enqueue(result);
                while (RecentResults.Count > MaxResults)
                {
                    RecentResults.TryDequeue(out _);
                }
                Logger.Info("Added score for user {0}: {1}", result.UserId, result.Result);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error adding score for user {result.UserId}.");
                throw; // Middleware handles
            }
        }

        [HttpGet("recent")]
        public IActionResult GetRecent()
        {
            try
            {
                Logger.Info($"Retrieved recent scores, count: {RecentResults.Count}");
                return Ok(RecentResults.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error retrieving scores.");
                throw; // Middleware handles
            }
        }

        [HttpDelete("reset")]
        public IActionResult Reset()
        {
            try
            {
                RecentResults.Clear();
                Logger.Info("Scoreboard reset.");
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error resetting scoreboard.");
                throw; // Middleware handles
            }
        }
    }
}
