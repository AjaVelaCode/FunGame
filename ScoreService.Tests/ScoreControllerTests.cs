using FunGame.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScoreService.Controllers;
using ScoreService.Model;

public class ScoreControllerTests
{
    private readonly TestScoreController _controller;

    public ScoreControllerTests()
    {
        _controller = new TestScoreController();
    }

    [Fact]
    public void Add_ValidRequest_ReturnsOk()
    {
        // Arrange
        _controller.ClearGameResultsForTesting();
        var request = new GameResult
        {
            UserId = "Alice",
            PlayerChoice = GameChoice.Rock,
            ComputerChoice = GameChoice.Scissors,
            Result = "Player wins!",
            Timestamp = DateTime.UtcNow
        };

        // Act
        var result = _controller.Add(request) as OkResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void Add_InvalidRequest_ReturnsBadRequest()
    {
        var request = new GameResult
        {
            UserId = null,
            PlayerChoice = GameChoice.None,
            ComputerChoice = GameChoice.Scissors,
            Result = null
        };

        var result = _controller.Add(request) as BadRequestObjectResult;
        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(JsonConvert.SerializeObject(result!.Value));


        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Invalid game result.", errorResponse!.Error);
    }

    [Fact]
    public void GetRecent_NoResults_ReturnsEmptyList()
    {
        // Arrange
        _controller.ClearGameResultsForTesting();

        // Act
        var result = _controller.GetRecent() as OkObjectResult;
        var results = result?.Value as List<RecentResponse>;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public void GetRecent_MultipleResults_ReturnsRecentTen()
    {
        // Arrange
        _controller.ClearGameResultsForTesting();
        for (int i = 0; i < 15; i++)
        {
            _controller.Add(new GameResult
            {
                UserId = $"User{i}",
                PlayerChoice = GameChoice.Rock,
                ComputerChoice = GameChoice.Paper,
                Result = "Computer wins!",
                Timestamp = DateTime.UtcNow.AddSeconds(-i)
            });
        }

        // Act
        var result = _controller.GetRecent() as OkObjectResult;
        var results = result?.Value as List<RecentResponse>;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(results);
        Assert.Equal(10, results.Count); // Only 10 most recent
        Assert.Equal("User0", results.First().UserId); // Most recent
        Assert.Equal("User9", results.Last().UserId); // 10th most recent
    }

    public class TestScoreController : ScoreController
    {
        public new void ClearGameResultsForTesting()
        {
            base.ClearGameResultsForTesting();
        }
    }
}