using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PetFinder.Areas.Identity.Pages.Account
{
    public class CustomRegisterModel : RegisterModel
    {
        public CustomRegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser>
            signInManager, ILogger<RegisterModel> logger, IEmailSender emailSender) :
            base(userManager, signInManager, logger, emailSender)
        { }

        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }

        public class InputModel
        {
            
        }

    }
}





