﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PetFinder.Areas.Identity;
using PetFinder.Models;
using PetFinderApi.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PetFinderApi.Controllers
{
    [Route("api/")]
    public class AuthController : Controller
    {
        private readonly ILogger<CommentsController> _logger;
        private readonly PetFinderContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJWTService _jwtService;

        public AuthController(
            ILogger<CommentsController> logger,
            PetFinderContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJWTService jwtService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        // Logea al usuario
        /// <summary>
        /// Logs in a user
        /// </summary>
        /// <remarks>
        /// Logs in a user by sending his email and password, it returns a Token required to authenticate with JWT
        /// </remarks>
        /// <param name="model">User's email and password</param>
        /// <response code="200">Returns the Token</response>
        /// <response code="404">If the user wasn't found</response>
        /// <response code="409">If the user couldn't be logged in</response>
        [Produces("application/json")]
        [HttpPost("auth")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            IActionResult response = Unauthorized();

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            
            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                //string Token = await GenerateJwtToken(model.Email, appUser);
                var jwt = await _jwtService.GenerateJwtToken(appUser.Email, appUser);
                response = Ok(new { token = jwt});
            }
            return response;
        }
    }
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}