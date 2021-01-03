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
        Task<object> GenerateJwtToken(string email, ApplicationUser user);
        string GetUserEmail(HttpContext httpContext);
    }
}
