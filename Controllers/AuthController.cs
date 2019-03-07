using System.Threading.Tasks;
using aaaNew.Data;
using aaaNew.Models;
using Microsoft.AspNetCore.Mvc;

namespace aaaNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;

        }

        [HttpPost("register")]
        public async Task<IActionResult>Register(string username, string password)
        {
            // validate request
            username = username.ToLower();
            if (await _repo.UserExists(username))
                return BadRequest("Username already exists");

            var userToCreate = new User    
            {
                Username = username
            };
            var crreatedUser = await _repo.Register(userToCreate, password);

            return StatusCode(201);
        }
    }
}