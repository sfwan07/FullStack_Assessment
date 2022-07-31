using Identity.Model;
using Identity.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;

        public AuthController(IAuthRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _repository.Login(model);
            switch (result.Status)
            {
                case LoginStatus.RequiresTwoFactor:
                    return BadRequest();
                case LoginStatus.IsNotAllowed:
                    return NoContent();
                case LoginStatus.IsLockedOut:
                    return Forbid();
                case LoginStatus.Succeeded:
                    return Ok(new { Token = result.Data });
                default:
                    return NotFound();
            }
        }

        [HttpGet]
        [Authorize]
        public IAsyncEnumerable<UserViewModel> GetAllUsers()
        {
            return  _repository.GetUsers();
        }
    }
}
