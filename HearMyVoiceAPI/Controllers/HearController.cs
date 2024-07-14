using HeayMyVoiceCommon.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HearMyVoiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HearController : ControllerBase
    {
        private readonly IHearService _hearService;
        public HearController(IHearService hearService)
        {
            _hearService = hearService;
        }
        [HttpGet]
        [Route("possibleAnswer")]
        public IActionResult PossibleAnswer([FromQuery] string text)
        {
            try
            {
                var possibleText = _hearService.PossibleAnswer(text);
                return new JsonResult(new { message = possibleText }) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Error speaking text: {ex.Message}" });
            }
        }


    }

}

