using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetFinder.Areas.Identity;
using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    public interface IApplicationUserService
    {

        Task<IEnumerable<ApplicationUser>> GetAll();
        Task<bool> Downgrade(string userId);
        Task<bool> Upgrade(string userId);
        Task<ApplicationUser> GetCurrent();
        Task<ApplicationUser> getById(string Id);
        Task<IdentityResult> DeleteAsync(ApplicationUser user);
    }
}
