using Microsoft.AspNetCore.Http;
using PetFinder.Areas.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinderApi.Data.Interfaces
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
}
