using Microsoft.AspNetCore.Mvc;

namespace PlayerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
