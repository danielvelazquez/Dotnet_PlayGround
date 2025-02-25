using Microsoft.AspNetCore.Mvc;

namespace PersonasWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonasController : ControllerBase
    {
        public IActionResult Index()
        {
            return Ok("Hello, World!");
        }
    }
}
