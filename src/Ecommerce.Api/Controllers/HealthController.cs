using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {

            //throw new InvalidOperationException("Testing global error handling.");

            return Ok(new
            {
                Status = "Healthy",
                Application = "Ecommerce API",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
