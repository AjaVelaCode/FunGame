using FunGame.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using PlayerService.Controllers;
using PlayerService.Models;

namespace PlayerService.Tests
{
    public class PlayerControllerTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IOptions<ServiceUrls>> _serviceUrlsMock;
        private readonly PlayerController _controller;
        private readonly HttpClient _httpClient;

        public PlayerControllerTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpClient = new HttpClient(new MockHttpMessageHandler());
            _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_httpClient);

            _serviceUrlsMock = new Mock<IOptions<ServiceUrls>>();
            _serviceUrlsMock.Setup(o => o.Value).Returns(new ServiceUrls
            {
                GameService = "http://localhost:5148/api/game/compute",
                ScoreService = "http://localhost:5064/api/score/add",
                RandomNumberService = "http://codechallenge.boohma.com/random"
            });

            _controller = new PlayerController(_httpClientFactoryMock.Object, _serviceUrlsMock.Object);
        }

        [Fact]
        public void GetAllChoices_ReturnsAllChoicesExcludingNone()
        {
            // Act
            var result = _controller.GetAllChoices() as OkObjectResult;
            var choices = result?.Value as GameChoice[];

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(new[] { GameChoice.Rock, GameChoice.Paper, GameChoice.Scissors, GameChoice.Lizard, GameChoice.Spock}, choices);
            if (choices != null) Assert.DoesNotContain(GameChoice.None, choices);
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
            Assert.Contains(response.Choice, GameChoice.None.GetValuesWithout());
            Assert.NotEqual(GameChoice.None, response.Choice);
        }

        [Fact]
        public async Task Play_ValidRequest_ReturnsGameResult()
        {
            // Arrange
            var request = new PlayRequest { PlayerChoice = GameChoice.Paper, UserId = "Alice" };

            // Act
            var result = await _controller.Play(request) as OkObjectResult;
            var response = (result?.Value as GamePlayResponse)!;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(GameChoice.Paper, response.PlayerChoice);
            Assert.Contains(response.ComputerChoice, GameChoice.None.GetValuesWithout());
            Assert.NotEqual(GameChoice.None, response.ComputerChoice);
            Assert.NotEmpty(response.Result);
        }

        [Fact]
        public async Task Play_EmptyBody_ReturnsBadRequest()
        {
            // Arrange
            var request = new PlayRequest(); // Simulates {} with PlayerChoice = None

            // Act
            var result = await _controller.Play(request) as BadRequestObjectResult;
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(JsonConvert.SerializeObject(result!.Value));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("PlayerChoice is required.", errorResponse!.Error);
        }

        [Fact]
        public async Task Play_InvalidPlayerChoice_ReturnsBadRequest()
        {
            // Arrange
            var request = new PlayRequest { PlayerChoice = (GameChoice)(-2), UserId = "Alice" };

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
            var request = new PlayRequest { PlayerChoice = GameChoice.None, UserId = "Alice" };

            // Act
            var result = await _controller.Play(request) as BadRequestObjectResult;
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(JsonConvert.SerializeObject(result!.Value));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("PlayerChoice is required.", errorResponse!.Error);
        }

    }

    // Mock HttpMessageHandler for HttpClient
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (request.RequestUri.ToString().Contains("random"))
            {
                return new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"random_number\": 1}")
                };
            }
            else if (request.RequestUri.ToString().Contains("game/compute"))
            {
                return new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"Result\": \"Player wins!\"}")
                };
            }
            else if (request.RequestUri.ToString().Contains("score/add"))
            {
                return new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("")
                };
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }
    }


}