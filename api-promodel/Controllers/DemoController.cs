using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api_promodel.Controllers
{
    [Route("api/demo")]
    [ApiController]
    public class DemoController : ControllerBase
    {

        [HttpGet]
        public IActionResult Demo() {

            return Ok("x");
        }

    }
}
