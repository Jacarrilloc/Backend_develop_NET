using Microsoft.AspNetCore.Mvc;
using SwaggerExample.Models;

namespace SwaggerExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet("{id}")]
        public ActionResult<User> GetUserById(int id)
        {
            var user = new User
            {
                Id = id,
                Name = "User Name"
            };
            return Ok(user);
        }
    }
}
