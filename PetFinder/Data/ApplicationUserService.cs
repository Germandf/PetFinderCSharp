using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetFinder.Areas.Identity;
using PetFinder.Models;
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

        public ApplicationUserService(PetFinderContext context, 
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Downgrade(String userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            IdentityResult resultRemoveRole = await _userManager.AddToRoleAsync(user, "Normal");
            if (resultRemoveRole.Succeeded)
            {
                IdentityResult resultAdd = await _userManager.RemoveFromRoleAsync(user, "Administrator");
                return resultAdd.Succeeded;
            }
            return false;
        }
        public async Task<bool> Upgrade(String userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            IdentityResult resultAdd = await _userManager.RemoveFromRoleAsync(user, "Normal");
            if (resultAdd.Succeeded)
            {
                IdentityResult resultRemoveRole = await _userManager.AddToRoleAsync(user, "Administrator");
                return resultRemoveRole.Succeeded;
            }
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

    }
}
