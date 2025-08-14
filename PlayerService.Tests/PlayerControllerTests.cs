using System.Net;
using FunGame.Common.Constants;
using FunGame.Common.Helpers;
using FunGame.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using PlayerService.Controllers;
using PlayerService.Models;
using PlayerService.Models.Requests;
using PlayerService.Models.Responses;

namespace PlayerService.Tests;

public class PlayerControllerTests
{
    private readonly PlayerController _controller;
    private readonly HttpClient _httpClient;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<IOptions<ServiceUrls>> _serviceUrlsMock;

    public PlayerControllerTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _httpClient = new HttpClient(new MockHttpMessageHandler());
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_httpClient);

        _serviceUrlsMock = new Mock<IOptions<ServiceUrls>>();
        _serviceUrlsMock.Setup(o => o.Value).Returns(new ServiceUrls
        {
            GameService = "http://localhost:5001/api/game/compute",
            ScoreService = "http://localhost:5002/api/score/add",
            RandomNumberService = "http://codechallenge.boohma.com/random"
        });

        _controller = new PlayerController(_httpClientFactoryMock.Object, _serviceUrlsMock.Object);
    }

    [Fact]
    public void GetAllChoices_ReturnsAllChoicesExcludingNone()
    {
        // Act
        var result = _controller.GetAllChoices() as OkObjectResult;
        var choices = result?.Value as List<ChoiceResponse>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(choices);
        Assert.Equal(5, choices.Count);

        var expectedNames = new[] { "rock", "paper", "scissors", "lizard", "spock" };
        var actualNames = choices.Select(c => c.Name).ToArray();
        Assert.Equal(expectedNames, actualNames);

        Assert.DoesNotContain(choices, c => c.Name == "none");
    }

    [Fact]
    public async Task GetRandomChoice_ReturnsValidChoice()
    {
        // Act
        var result = await _controller.GetRandomChoice() as OkObjectResult;
        var response = (result?.Value as RandomChoiceResponse)!;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains(response.Name, GameChoiceExtensions.GetValidChoiceNames());
    }

    [Fact]
    public async Task Play_EmptyBody_ReturnsBadRequest()
    {
        // Arrange
        var request = new PlayRequest(); // Simulates {} with PlayerChoiceId = None

        // Act
        var result = await _controller.Play(request) as BadRequestObjectResult;
        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(JsonConvert.SerializeObject(result!.Value));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("PlayerChoiceId is required.", errorResponse!.Error);
    }

    [Fact]
    public async Task Play_InvalidPlayerChoice_ReturnsBadRequest()
    {
        // Arrange
        var request = new PlayRequest { PlayerChoiceId = -2, UserId = "Alice" };

        // Act
        var result = await _controller.Play(request) as BadRequestObjectResult;
        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(JsonConvert.SerializeObject(result!.Value));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Invalid choice", errorResponse!.Error);
    }

    [Fact]
    public async Task Play_DefaultPlayerChoiceNone_ReturnsBadRequest()
    {
        // Arrange
        var request = new PlayRequest { PlayerChoiceId = (int)GameChoice.None, UserId = "Alice" };

        // Act
        var result = await _controller.Play(request) as BadRequestObjectResult;
        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(JsonConvert.SerializeObject(result!.Value));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("PlayerChoiceId is required.", errorResponse!.Error);
    }
}

// Mock HttpMessageHandler for HttpClient
public class MockHttpMessageHandler : HttpMessageHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.RequestUri.ToString().Contains("random"))
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"random_number\": 1}")
            };
        if (request.RequestUri.ToString().Contains("game/compute"))
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"Results\": \"win\"}")
            };
        if (request.RequestUri.ToString().Contains("score/add"))
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            };
        return new HttpResponseMessage(HttpStatusCode.NotFound);
    }
}