using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using FunGame.Common;
using FunGame.Common.Responses;
using NLog;
using ScoreService.Model;

namespace ScoreService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScoreController : ControllerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly ConcurrentBag<GameResult> GameResults = [];


        [HttpPost("add")]
        public IActionResult Add([FromBody] GameResult request)
        {
            {
                if (string.IsNullOrEmpty(request.Result))
                {
                    Logger.Warn($"Invalid score request: {request}");
                    return BadRequest(new ErrorResponse { Error = "Invalid game result." });
                }

                try
                {
                    GameResults.Add(request);
                    Logger.Info($"Score added for user {request.UserId}: {request.PlayerChoice} vs {request.ComputerChoice}, Result: {request.Result}");
                    return Ok();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Error adding score for user {request.UserId}");
                    throw; // Middleware handles
                }
            }
        }

        [HttpGet("recent")]
        public IActionResult GetRecent()
        {
            try
            {
                var recentResults = GameResults
                    .OrderByDescending(r => r.Timestamp)
                    .Take(10)
                    .Select(r => new RecentResponse()
                    {
                        UserId = r.UserId,
                        PlayerChoice = r.PlayerChoice,
                        ComputerChoice = r.ComputerChoice,
                        Result = r.Result,
                        Timestamp = r.Timestamp
                    })
                    .ToList();

                Logger.Info($"Retrieved {recentResults.Count} recent game results");
                return Ok(recentResults);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error retrieving recent game results");
                throw; // Middleware handles
            }
        }

        [HttpDelete("reset")]
        public IActionResult Reset()
        {
            try
            {
                GameResults.Clear();
                Logger.Info("Scoreboard reset.");
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error resetting scoreboard.");
                throw; // Middleware handles
            }
        }

        // Added for testing
        protected void ClearGameResultsForTesting()
        {
            while (GameResults.TryTake(out _)) { } // Clear all items
        }
    }
}
