using PetFinder.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data.Interfaces
{
    public interface IAuthJwtService
    {
        /// <summary>
        /// Does a POST method to /api/auth with the User's email and password to obtain its JWT temporary Token
        /// </summary>
        /// <returns>
        /// The User's temporary Token if the data was correct or a list of errors in case it was not
        /// </returns>
        Task<GenericResult<string>> GetJwt(string email, string password);
    }
}
