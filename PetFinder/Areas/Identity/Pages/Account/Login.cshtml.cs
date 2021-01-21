using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetFinder.Data;
using PetFinder.Models;
using Serilog;

namespace PetFinder.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly PetFinderContext _context;
        private readonly IAuthJwtService _jwtService;
        private readonly ILogger _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
            ILogger logger,
            UserManager<ApplicationUser> userManager,
            IAuthJwtService jwtService,
            PetFinderContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _jwtService = jwtService;
            _context = context;
        }

        [BindProperty] public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData] public string ErrorMessage { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage)) ModelState.AddModelError(string.Empty, ErrorMessage);

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                var user = await _userManager.FindByNameAsync(Input.Email);

                if (user != null && await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    var resultJwt = await _jwtService.GetJwt(Input.Email, Input.Password);
                    if (resultJwt.Success)
                    {
                        // Si el usuario tiene el claim de JWT lo elimino y agrego uno nuevo
                        var claimJwt = _context.UserClaims.Where(x => x.ClaimType == "JWT" && x.UserId == user.Id)
                            .FirstOrDefault();

                        if (claimJwt != null)
                        {
                            _context.UserClaims.Remove(claimJwt);
                            await _context.SaveChangesAsync();
                        }

                        await _userManager.AddClaimAsync(user, new Claim("JWT", resultJwt.Value));
                    }
                }


                var result =
                    await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, false);
                if (result.Succeeded)
                {
                    _logger.Information("{User} logged in.", user.Email);
                    return LocalRedirect(returnUrl);
                }

                if (result.RequiresTwoFactor)
                    return RedirectToPage("./LoginWith2fa", new {ReturnUrl = returnUrl, Input.RememberMe});
                if (result.IsLockedOut)
                {
                    _logger.Warning("{User} account locked out.", user.Email);
                    return RedirectToPage("./Lockout");
                }

                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                return Page();
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public class InputModel
        {
            [Required] [EmailAddress] public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Recordar")] public bool RememberMe { get; set; }
        }
    }
}