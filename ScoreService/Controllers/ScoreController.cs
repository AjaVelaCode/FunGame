using System.Collections.Concurrent;
using FunGame.Common.Constants;
using FunGame.Common.Helpers;
using FunGame.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using NLog;
using ScoreService.Model;
using ScoreService.Model.Responses;

namespace ScoreService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScoreController : ControllerBase
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static readonly ConcurrentBag<GameResult> GameResults = [];


    [HttpPost("add")]
    public IActionResult Add([FromBody] GameResult request)
    {
        if (string.IsNullOrEmpty(request.UserId)
            || request.PlayerChoice == GameChoice.None
            || request.ComputerChoice == GameChoice.None
            || string.IsNullOrEmpty(request.Result))
        {
            Logger.Warn($"Invalid game result received: {request}");
            return BadRequest(new ErrorResponse { Error = "Invalid game result." });
        }

        if (!GameChoiceExtensions.GetValidChoices().Contains(request.PlayerChoice) ||
            !GameChoiceExtensions.GetValidChoices().Contains(request.ComputerChoice))
        {
            Logger.Warn(
                $"Invalid choices in game result: Player={request.PlayerChoice}, Computer={request.ComputerChoice}");
            return BadRequest(new
                { Error = $"Invalid choice. Choose {string.Join(", ", GameChoiceExtensions.GetValidChoiceNames())}." });
        }

        try
        {
            GameResults.Add(request);
            Logger.Info(
                $"Score added for user {request.UserId}: {request.PlayerChoice} vs {request.ComputerChoice}, Result: {request.Result}");
            return Ok();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Error adding score for user {request.UserId}");
            throw; // Middleware handles
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
                .Select(r => new RecentResponse
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
        while (GameResults.TryTake(out _))
        {
        } // Clear all items
    }
}