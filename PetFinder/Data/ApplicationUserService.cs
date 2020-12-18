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

        public ApplicationUserService(PetFinderContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
