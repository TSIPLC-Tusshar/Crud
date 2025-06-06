using Microsoft.AspNetCore.Mvc;

namespace MySqlAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTest()
        {
            return Ok(new
            {
                Name = "Test",
                Email = "Test@gmail.com",
                Phone = "1236543224325"
            });
        }
    }
}
