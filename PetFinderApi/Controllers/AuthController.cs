using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PetFinder.Areas.Identity;
using PetFinderApi.Data;

namespace PetFinderApi.Controllers
{
    [Route("api/")]
    public class AuthController : Controller
    {
        private readonly IJwtService _jwtService;
        private readonly ILogger<CommentsController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(
            ILogger<CommentsController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        /// <summary>
        ///     Logs In a user
        /// </summary>
        /// <remarks>
        ///     Logs in a user by sending his email and password, it returns a Token required to authenticate with JWT
        /// </remarks>
        /// <param name="model">User's email and password</param>
        /// <response code="200">Returns the Token</response>
        /// <response code="401">If the user couldn't be logged in</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("auth")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            IActionResult response = Unauthorized();
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                //string Token = await GenerateJwtToken(model.Email, appUser);
                var jwt = _jwtService.GenerateJwtToken(appUser);
                response = Ok(new {token = jwt});
            }

            return response;
        }
    }

    public class LoginModel
    {
        [Required] public string Email { get; set; }

        [Required] public string Password { get; set; }
    }
}