using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SocialMedia.Api.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                mensaje = "Conexión exitosa",
                fecha = DateTime.Now
            });
        }

        [HttpGet("con")]
        public IActionResult Conexion()
        {
            return Ok(new { mensaje = "Conexión exitosa" });
        }
    }
}
