using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using PetFinder.Data;
using Serilog;

namespace PetFinder.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly IEmailSender _emailSender;
        private readonly IAuthJwtService _jwtService;
        private readonly ILogger _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            IAuthJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        [BindProperty] public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                    {UserName = Input.Email, Email = Input.Email, Name = Input.Name, Surname = Input.Surname};
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            null,
                            new {area = "Identity", userId = user.Id, code, returnUrl},
                            Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        return RedirectToPage("RegisterConfirmation", new {email = Input.Email, returnUrl});
                    }

                    var RoleResult = await _roleManager.FindByNameAsync(ApplicationUserService.ROLE_ADMIN);
                    if (RoleResult == null)
                        // Create ROLE_ADMIN Role
                        await _roleManager.CreateAsync(new IdentityRole(ApplicationUserService.ROLE_ADMIN));
                    RoleResult = await _roleManager.FindByNameAsync(ApplicationUserService.ROLE_USER);
                    if (RoleResult == null)
                        // Create ROLE_USER Role
                        await _roleManager.CreateAsync(new IdentityRole(ApplicationUserService.ROLE_USER));

                    if (_userManager.Users.Count() == 1)
                    {
                        //si es el primer usuario registrado le asigno el rol admin
                        await _userManager.AddToRoleAsync(user, ApplicationUserService.ROLE_ADMIN);
                        _logger.Information("User: {User} created a new account with {Role} role.", user,
                            ApplicationUserService.ROLE_ADMIN);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, ApplicationUserService.ROLE_USER);
                        _logger.Information("User: {User} created a new account with {Role} role.", user,
                            ApplicationUserService.ROLE_USER);
                    }


                    var resultJwt = await _jwtService.GetJwt(Input.Email, Input.Password);
                    if (resultJwt.Success) await _userManager.AddClaimAsync(user, new Claim("JWT", resultJwt.value));
                    await _signInManager.SignInAsync(user, false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public class InputModel
        {
            [Required(ErrorMessage = "El nombre es obligatorio")]
            [StringLength(100, ErrorMessage = "El {0} debe tener al menos {2} y un máximo de {1} caracteres.",
                MinimumLength = 2)]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required(ErrorMessage = "El apellido es obligatorio")]
            [StringLength(100, ErrorMessage = "El {0} debe tener al menos {2} y un máximo de {1} caracteres.",
                MinimumLength = 2)]
            [Display(Name = "Surname")]
            public string Surname { get; set; }

            [Required(ErrorMessage = "El email es obligatorio")]
            [EmailAddress(ErrorMessage = "La dirección ingresada no es una dirección de correo electrónico válida.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria")]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Debe confirmar la contraseña")]
            [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
            public string ConfirmPassword { get; set; }
        }
    }
}