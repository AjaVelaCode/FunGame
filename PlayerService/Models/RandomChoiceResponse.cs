using FunGame.Common.Constants;

namespace PlayerService.Models
{
    public class RandomChoiceResponse
    {
        public int Id { get; set; }
        public GameChoice Choice { get; set; }
    }
}
