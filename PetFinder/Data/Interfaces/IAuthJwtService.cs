using PetFinder.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data.Interfaces
{
    public interface IAuthJwtService
    {
        Task<GenericResult<string>> GetJwt(string email, string password);
    }
}
