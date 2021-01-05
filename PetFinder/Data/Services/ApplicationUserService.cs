using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetFinder.Areas.Identity;
using PetFinder.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{

    public class ApplicationUserService : IApplicationUserService
    {
        public static string ROLE_ADMIN = "Administrator";
        public static string ROLE_USER = "Normal";

        private readonly PetFinderContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _loger;

        public ApplicationUserService(PetFinderContext context, 
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger loger)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _loger = loger;
        }

        public async Task<bool> Downgrade(String userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            IdentityResult resultRemoveRole = await _userManager.AddToRoleAsync(user, "Normal");
            var currUser = await GetCurrent();

            if (resultRemoveRole.Succeeded)
            {
                IdentityResult resultAdd = await _userManager.RemoveFromRoleAsync(user, "Administrator");

                if (resultAdd.Succeeded)
                {
                    _loger.Information("{user} role changed to NORMAL USER by {currUser}", user.Email, currUser.Email);
                    return true;
                }
            }
            _loger.Warning("{currUser} try to change role to NORMAL USER of {user} without success", currUser.Email, user.Email);
            return false;
        }
        public async Task<bool> Upgrade(String userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            IdentityResult resultAdd = await _userManager.RemoveFromRoleAsync(user, "Normal");
            var currUser = await GetCurrent();

            if (resultAdd.Succeeded)
            {
                IdentityResult resultRemoveRole = await _userManager.AddToRoleAsync(user, "Administrator");
                if (resultRemoveRole.Succeeded)
                {
                    _loger.Information("{user} role changed to ADMIN by {currUser}", user.Email, currUser.Email);

                    return true;
                }
            }
            _loger.Warning("{currUser} try to change role to ADMIN of {user} without success", currUser.Email, user.Email);

            return false;
        }
        public async Task<ApplicationUser> GetCurrent()
        {
            ApplicationUser currUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            if(currUser == null)
            {
                return null;
            }

            if (await _userManager.IsInRoleAsync(currUser, ApplicationUserService.ROLE_ADMIN))
            {
                currUser.setAdmin(true);
            }

            return currUser;

        }
        public async Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            IdentityResult result = await _userManager.DeleteAsync(await _userManager.FindByIdAsync(user.Id));

            var currUser = await GetCurrent();
            if (result.Succeeded)
                _loger.Information("{currUser} deleted by {user} successfully", user.Email, currUser.Email );
            else
                _loger.Warning("{currUser} try delete  {user} without success", currUser.Email, user.Email);
            return result;
        }
        public async Task<IEnumerable<ApplicationUser>> GetAll()
        {

            IEnumerable<ApplicationUser> users = await _context.AspNetUsers.ToListAsync();

            foreach(ApplicationUser user in users)
            {
                if (await _userManager.IsInRoleAsync(user, ROLE_ADMIN))
                {
                    user.setAdmin(true);
                }
                else
                {
                    user.setAdmin(false);
                }
            }
            return users;
        }

        public async Task<ApplicationUser> GetById(string Id)
        {
            return await _context.AspNetUsers.FindAsync(Id);
        }
    }
}
