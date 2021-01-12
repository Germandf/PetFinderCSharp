using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PetFinder.Areas.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace PetFinderApi.Data
{
    public interface IJWTService
    {
        /// <summary>
        /// Generates a JWT by a User's credentials
        /// </summary>
        /// <returns>
        /// The JWT in string format
        /// </returns>
        string GenerateJwtToken(ApplicationUser user);

        /// <summary>
        /// Gets a User's email through the HttpContext
        /// </summary>
        /// <returns>
        /// A string with the User's email
        /// </returns>
        string GetUserEmail(HttpContext httpContext);
    }

    public class JWTService : IJWTService
    {
        private readonly IConfiguration _configuration;

        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetUserEmail(HttpContext httpContext)
        {
            ClaimsPrincipal principal = httpContext.User;
            string userEmail = principal.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
            return userEmail;
        }

        public string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));
            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
