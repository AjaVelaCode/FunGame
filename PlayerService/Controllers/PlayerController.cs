using FunGame.Common.Constants;
using FunGame.Common.Helpers;
using FunGame.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NLog;
using System.Net;
using FunGame.Common.Requests;
using ServiceUrls = PlayerService.Models.ServiceUrls;
using PlayerService.Models.Requests;
using PlayerService.Models.Responses;

namespace PlayerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase  // Use primary constructor syntax for clarity
    {
        private readonly HttpClient _httpClient;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _gameServiceUrl;
        private readonly string _scoreServiceUrl;
        private readonly string _randomNumberServiceUrl;

        public PlayerController(IHttpClientFactory httpClientFactory, IOptions<ServiceUrls> serviceUrls)
        {
            _httpClient = httpClientFactory.CreateClient();
            _gameServiceUrl = serviceUrls.Value.GameService;
            _scoreServiceUrl = serviceUrls.Value.ScoreService;
            _randomNumberServiceUrl = serviceUrls.Value.RandomNumberService;
        }

        [HttpGet("choices")]
        public IActionResult GetAllChoices()
        {
            try
            {
                var choices = GameChoiceExtensions.GetValidChoices()
                    .Select(c => new ChoiceResponse
                    {
                        Id = (int)c,
                        Name = c.ToString().ToLowerInvariant()
                    })
                    .ToList();

                Logger.Info($"Retrieved all game choices: {string.Join(", ", choices.Select(choice => choice.Name))}");
                return Ok(choices);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error retrieving game choices");
                throw; // Middleware handles
            }
        }

        [HttpGet("choice")]
        public async Task<IActionResult> GetRandomChoice()
        {
            try
            {
                var choices = GameChoiceExtensions.GetValidChoices();
                var randomChoice = await GetComputerChoiceAsync(choices);
                Logger.Info($"Generated random choice: {randomChoice}");
                return Ok(new RandomChoiceResponse
                {
                    Id = (int)randomChoice,
                    Name = randomChoice.ToString().ToLowerInvariant()
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error generating random choice");
                throw; // Middleware handles
            }
        }


        [HttpPost("play")]
        public async Task<IActionResult> Play([FromBody] PlayRequest request)
        {
            if (request.PlayerChoiceId == -1)
            {
                Logger.Warn("PlayerChoiceId is required");
                return BadRequest(new ErrorResponse { Error = "PlayerChoiceId is required." });
            }

            if (!Enum.IsDefined(typeof(GameChoice), request.PlayerChoiceId))
            {
                Logger.Warn($"Invalid player choice: {request.PlayerChoiceId}");
                return BadRequest(new ErrorResponse { Error = $"Invalid choice. Choose {string.Join(", ", GameChoiceExtensions.GetValidChoiceNames())}." });
            }

            try
            {
                var choices = GameChoiceExtensions.GetValidChoices();
                
                var computerChoice = await GetComputerChoiceAsync(choices);

                var gameResult = await CalculateGameResultAsync((GameChoice)request.PlayerChoiceId, computerChoice);

                await SaveScoreAsync(request, computerChoice, gameResult.Result);

                return GetGameResult(request, computerChoice, gameResult.Result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Unexpected error in Play endpoint for user {request.UserId}.");
                throw; // Middleware handles
            }
        }
        
        private async Task<GameChoice> GetComputerChoiceAsync(GameChoice[] choices)
        {
            GameChoice computerChoice;
            try
            {
                var response = await _httpClient.GetFromJsonAsync<RandomNumberResponse>(_randomNumberServiceUrl)
                    .TimeoutAfter(TimeSpan.FromSeconds(5));
                if (response == null || response.RandomNumber < 1 || response.RandomNumber > 100)
                {
                    Logger.Warn($"Invalid random number response: {response?.RandomNumber}");
                    throw new HttpRequestException("Invalid response from random number service.");
                }
                // Map 1-100 to 0-4 (5 choices)
                computerChoice = choices[(response.RandomNumber - 1) % choices.Length];
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Random number service failed, falling back to local random generation.");
                computerChoice = choices[Random.Shared.Next(choices.Length)];
            }

            return computerChoice;
        }

        private async Task<GameResponse> CalculateGameResultAsync(GameChoice playerChoice, GameChoice computerChoice)
        {
            var computeRequest = new ComputeRequest
            {
                PlayerChoice = playerChoice, 
                ComputerChoice = computerChoice
            };
            HttpResponseMessage gameResponse;
            try
            {
                gameResponse = await _httpClient.PostAsJsonAsync(_gameServiceUrl, computeRequest)
                    .TimeoutAfter(TimeSpan.FromSeconds(5));
                gameResponse.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Logger.Error(ex, "GameService call failed.");
                throw new HttpRequestException("Game service is currently unavailable.", ex, HttpStatusCode.ServiceUnavailable);
            }
            catch (TimeoutException ex)
            {
                Logger.Error(ex, "GameService timed out.");
                throw new HttpRequestException("Game service timed out.", ex, HttpStatusCode.ServiceUnavailable);
            }

            try
            {
                var gameResult = await gameResponse.Content.ReadFromJsonAsync<GameResponse>();
                if (gameResult == null || string.IsNullOrEmpty(gameResult.Result))
                {
                    Logger.Error("GameService returned null or invalid result.");
                    throw new HttpRequestException("Invalid response from game service.", null, HttpStatusCode.InternalServerError);
                }
                return gameResult;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to deserialize GameService response.");
                throw new HttpRequestException("Invalid response from game service.", ex, HttpStatusCode.InternalServerError);
            }
        }
        
        private async Task SaveScoreAsync(PlayRequest request, GameChoice computerChoice, string result)
        {
            var scoreResult = new
            {
                UserId = string.IsNullOrEmpty(request.UserId) ? "Anonymous" : request.UserId,
                PlayerChoice = request.PlayerChoiceId,
                ComputerChoice = computerChoice,
                Result = result
            };

            try
            {
                var scoreResponse = await _httpClient.PostAsJsonAsync(_scoreServiceUrl, scoreResult)
                    .TimeoutAfter(TimeSpan.FromSeconds(5));
                scoreResponse.EnsureSuccessStatusCode();
                Logger.Info($"Score saved for user {scoreResult.UserId}: {result}");
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, $"ScoreService call failed for user {scoreResult.UserId}, continuing without saving score.");
            }
        }

        private IActionResult GetGameResult(PlayRequest request, GameChoice computerChoice, string result)
        {
            var funFact = GameConstants.GetFunFact((GameChoice)request.PlayerChoiceId, computerChoice, result);

            return Ok(new GamePlayResponse
            {
                PlayerChoice = (GameChoice)request.PlayerChoiceId,
                ComputerChoice = computerChoice,
                Result = result,
                FunFact = funFact
            });
        }



    }
}
